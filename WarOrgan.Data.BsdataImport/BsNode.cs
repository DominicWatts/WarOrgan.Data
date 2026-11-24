using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace WarOrgan.Data.BsdataImport;

/// <summary>An entry in a BSData selection tree: the link as placed in the tree and the entry it resolves to.</summary>
public record BsNode(JsonObject Link, JsonObject Target)
{
    // Crusade, Warlord, Enhancement and option-toggle groups are not part of a datasheet
    private static readonly HashSet<string> SkippedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Crusade", "Warlord", "Enhancements", "Enhancements - Upgrades", "Show/Hide Options", "Weapon Modifications", "Battle Honours",
    };

    public string Name => Link.Name();
    public string Id => Target.Str("id") ?? string.Empty;
    public string LinkId => Link.Str("id") ?? string.Empty;
    public string Type => Target.Str("type") ?? "group";
    public bool IsGroup => Type is not ("unit" or "model" or "upgrade");
    public bool IsModel => Type == "model";

    public int? Min => Link.Constraint("min") ?? (ReferenceEquals(Link, Target) ? null : Target.Constraint("min"));
    public int? Max => Link.Constraint("max") ?? (ReferenceEquals(Link, Target) ? null : Target.Constraint("max"));

    /// <summary>True when a modifier changes one of this entry's selection limits.</summary>
    public bool HasLimitModifiers
    {
        get
        {
            var constraintIds = Link.Arr("constraints").Concat(Target.Arr("constraints")).Select(c => c.Str("id")).ToHashSet();
            return Link.Arr("modifiers").Concat(Target.Arr("modifiers")).Any(m => constraintIds.Contains(m.Str("field")));
        }
    }

    /// <summary>Name with " w/ ..." loadout suffixes removed, e.g. "Acolyte Hybrid w/ Cult Icon" becomes "Acolyte Hybrid".</summary>
    public string BaseModelName => Regex.Replace(Name, @"\s+w/.*$", string.Empty).Trim();

    public IEnumerable<BsNode> Children(BsData data)
    {
        foreach (var key in new[] { "selectionEntries", "selectionEntryGroups", "entryLinks" })
        {
            foreach (var link in Target.Arr(key))
            {
                if (SkippedNames.Contains(link.Name()) || link.Name().Contains("Crusade", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (data.Resolve(link) is { } target)
                    yield return new BsNode(link, target);
            }
        }
    }

    public BsNode? DefaultChild(BsData data)
    {
        var defaultId = Target.Str("defaultSelectionEntryId");
        return defaultId == null ? null : Children(data).FirstOrDefault(c => c.LinkId == defaultId || c.Id == defaultId);
    }

    public IEnumerable<BsNode> Descendants(BsData data)
    {
        foreach (var child in Children(data))
        {
            yield return child;
            foreach (var descendant in child.Descendants(data))
                yield return descendant;
        }
    }

    public IEnumerable<JsonObject> Profiles(BsData data)
    {
        foreach (var profile in Target.Arr("profiles"))
            yield return profile;
        foreach (var info in Target.Arr("infoLinks").Where(i => i.Str("type") == "profile"))
            if (data.Resolve(info) is { } profile)
                yield return profile;
    }

    public override string ToString() => Name;
}
