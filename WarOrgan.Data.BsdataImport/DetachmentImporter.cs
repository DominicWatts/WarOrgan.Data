using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using WarOrgan.Model.Faction;

namespace WarOrgan.Data.BsdataImport;

/// <summary>
/// Converts a catalogue's detachments: cost, Force Dispositions, rule and enhancements. BSData has no stratagems,
/// so those are left for Wahapedia.
/// </summary>
public class DetachmentImporter(BsData data, ImportReport report)
{
    public List<Detachment> Convert(IEnumerable<JsonObject> catalogues)
    {
        var detachments = new List<Detachment>();
        var dispositions = ForceDispositionNames();
        var enhancementGroups = EnhancementGroups();

        foreach (var entry in DetachmentEntries(catalogues))
        {
            var name = entry.Name();
            var cost = entry.Cost(BsData.DetachmentPointsTypeId);
            if (cost is null or 0)
            {
                report.Add("Detachments", $"Skipped \"{name}\": no Detachment Points cost (Boarding Actions or another mode).");
                continue;
            }

            var detachment = new Detachment { Name = name, Cost = (int)cost.Value };
            var categories = entry.Arr("categoryLinks").Select(c => c.Name())
                .Where(c => !Regex.IsMatch(c, @"^\d+DP Detachment$")).ToList();
            detachment.ForceDispositions = categories.Where(dispositions.Contains).ToList();
            var tags = categories.Except(detachment.ForceDispositions).Select(t => t.ToUpperInvariant()).ToList();
            if (tags.Count > 0)
            {
                detachment.Tags = tags;
                report.Add(name, $"Categories {string.Join(", ", tags)} were added as Tags; check what they restrict.");
            }

            var rules = entry.Arr("rules").Concat(entry.Arr("infoLinks").Where(i => i.Str("type") == "rule").Select(i => data.Resolve(i)!))
                .Where(r => r != null).ToList();
            if (rules.Count > 0)
            {
                // The dataset holds one rule per detachment; further rules follow under bold headings
                var text = TextUtils.CleanRuleText(rules[0].Str("description"));
                foreach (var extra in rules.Skip(1))
                    text += $"\n\n**{extra.Name()}**\n\n{TextUtils.CleanRuleText(extra.Str("description"))}";
                detachment.Rule = new DetachmentRule { Title = rules[0].Name(), Text = text };
            }
            else
                report.Add(name, "No detachment rule found.");

            if (enhancementGroups.TryGetValue(name, out var groups))
                foreach (var (group, isUpgrade) in groups)
                    detachment.Enhancements.AddRange(group.Children(data).Where(c => c.Type == "upgrade").Select(c => ToEnhancement(c, isUpgrade, name)));
            else
                report.Add(name, "No enhancements found.");

            report.Add(name, "Add stratagems from Wahapedia (BSData has none), plus any detachment effects the app should apply.");
            detachments.Add(detachment);
        }
        return detachments;
    }

    private IEnumerable<JsonObject> DetachmentEntries(IEnumerable<JsonObject> catalogues)
    {
        foreach (var catalogue in catalogues)
        {
            var primaryId = catalogue.Str("id");
            var seen = new HashSet<string>();
            foreach (var root in DetachmentRoots(catalogue, []))
            {
                var node = new BsNode(root, root);
                foreach (var group in node.Children(data).Where(c => c.IsGroup))
                    foreach (var detachment in group.Children(data).Where(c => c.Type == "upgrade"))
                        if (seen.Add(detachment.Id) && !HiddenFor(detachment, primaryId))
                            yield return detachment.Target;
            }
        }
    }

    /// <summary>
    /// The catalogue's own Detachment entry, or the one it links to: a library (Aeldari, Knights, Daemons) or,
    /// for Space Marine chapters, the Space Marines catalogue they import.
    /// </summary>
    private IEnumerable<JsonObject> DetachmentRoots(JsonObject catalogue, HashSet<string> visited)
    {
        if (!visited.Add(catalogue.Str("id") ?? string.Empty))
            yield break;
        var own = catalogue.Arr("sharedSelectionEntries").Where(e => e.Name() == "Detachment")
            .Concat(catalogue.Arr("entryLinks").Where(l => l.Name() == "Detachment").Select(l => data.Get(l.Str("targetId"))).OfType<JsonObject>())
            .ToList();
        if (own.Count > 0)
        {
            foreach (var root in own.DistinctBy(r => r.Str("id")))
                yield return root;
            yield break;
        }
        foreach (var link in catalogue.Arr("catalogueLinks").Where(l => l["importRootEntries"]?.GetValue<bool>() == true))
            if (data.Catalogues.Values.FirstOrDefault(c => c.Str("id") == link.Str("targetId")) is { } linked)
                foreach (var root in DetachmentRoots(linked, visited))
                    yield return root;
    }

    /// <summary>
    /// Shared detachment lists hide entries per army with primary-catalogue conditions (chapter detachments in
    /// Space Marines, Drukhari ones in the Aeldari library). Only those conditions are evaluated.
    /// </summary>
    private static bool HiddenFor(BsNode detachment, string? primaryId)
    {
        foreach (var modifier in detachment.Link.Arr("modifiers").Concat(ReferenceEquals(detachment.Link, detachment.Target) ? [] : detachment.Target.Arr("modifiers")))
        {
            if (modifier.Str("field") != "hidden" || modifier["value"]?.ToString() is not ("true" or "True"))
                continue;
            var conditions = modifier.Arr("conditions").ToList();
            if (conditions.Count == 0 || conditions.Any(c => c.Str("scope") != "primary-catalogue"))
                continue;
            if (conditions.All(c => c.Str("type") switch
                {
                    "instanceOf" => c.Str("childId") == primaryId,
                    "notInstanceOf" => c.Str("childId") != primaryId,
                    _ => false,
                }))
                return true;
        }
        return false;
    }

    /// <summary>Categories used by detachments in more than one catalogue are Force Dispositions; faction-specific ones are tags.</summary>
    private HashSet<string> ForceDispositionNames()
    {
        var usage = new Dictionary<string, HashSet<string>>();
        // Count only catalogues that own a detachment list; chapters reuse the Space Marines one
        foreach (var (file, catalogue) in data.Catalogues.Where(c => c.Value.Arr("sharedSelectionEntries").Any(e => e.Name() == "Detachment")))
            foreach (var entry in DetachmentEntries([catalogue]))
                foreach (var category in entry.Arr("categoryLinks").Select(c => c.Name()))
                    (usage.TryGetValue(category, out var files) ? files : usage[category] = new()).Add(file);
        return usage.Where(u => u.Value.Count > 1 && !Regex.IsMatch(u.Key, @"^\d+DP Detachment$")).Select(u => u.Key).ToHashSet();
    }

    /// <summary>Groups named "&lt;Detachment&gt; Enhancements" or "&lt;Detachment&gt; Upgrades" across every catalogue.</summary>
    private Dictionary<string, List<(BsNode Group, bool IsUpgrade)>> EnhancementGroups()
    {
        var result = new Dictionary<string, List<(BsNode, bool)>>(StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>();
        foreach (var catalogue in data.Catalogues.Values)
        {
            void Walk(JsonNode? node)
            {
                if (node is JsonArray array)
                    foreach (var item in array) Walk(item);
                if (node is not JsonObject obj)
                    return;
                var match = Regex.Match(obj.Name(), @"^(.+?) (Enhancements|Upgrades)$");
                if (match.Success && obj["selectionEntries"] is JsonArray && seen.Add(obj.Str("id") ?? obj.Name()))
                {
                    var key = match.Groups[1].Value;
                    (result.TryGetValue(key, out var list) ? list : result[key] = new()).Add((new BsNode(obj, obj), match.Groups[2].Value == "Upgrades"));
                }
                foreach (var (_, child) in obj) Walk(child);
            }
            Walk(catalogue);
        }
        return result;
    }

    private Enhancement ToEnhancement(BsNode node, bool isUpgrade, string detachment)
    {
        var description = node.Profiles(data).FirstOrDefault(p => p.Str("typeName") == "Abilities");
        var enhancement = new Enhancement
        {
            Name = node.Name,
            Cost = (int)(node.Target.Cost(BsData.PointsTypeId) ?? 0),
            Description = TextUtils.CleanRuleText(description?.Characteristic("Description")),
            Features = isUpgrade ? EnhancementFeature.Upgrade : EnhancementFeature.None,
        };

        // "Hidden unless the unit is one of these" conditions are the equip requirements
        foreach (var modifier in node.Target.Arr("modifiers").Where(m => m.Str("field") == "hidden"))
        {
            var conditions = modifier.Arr("conditions").Concat(modifier.Arr("conditionGroups").SelectMany(g => g.Arr("conditions"))).ToList();
            var required = conditions.Where(c => c.Str("type") == "notInstanceOf").Select(ConditionName).OfType<string>().ToList();
            var excluded = conditions.Where(c => c.Str("type") == "instanceOf" && c.Str("scope") == "ancestor").Select(ConditionName).OfType<string>().ToList();
            if (required.Count > 1)
                enhancement.RequiredOneOfKeywords.AddRange(required);
            else
                enhancement.RequiredKeywords.AddRange(required);
            enhancement.ExcludedKeywords.AddRange(excluded);
        }

        if (node.Target.Arr("modifierGroups").Any() || node.Target.Arr("modifiers").Any(m => m.Str("field") != "hidden"))
            report.Add(detachment, $"Enhancement \"{enhancement.Name}\" changes stats or weapons; add its adjustments by hand.");
        if (enhancement.RequiredKeywords.Count + enhancement.RequiredOneOfKeywords.Count == 0)
            report.Add(detachment, $"Enhancement \"{enhancement.Name}\" has no unit restriction in BSData; check who can take it.");
        return enhancement;
    }

    private string? ConditionName(JsonObject condition)
        => condition.Str("childName") ?? data.Get(condition.Str("childId"))?.Name();
}
