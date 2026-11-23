using WarOrgan.Model.Unit.Wargear;

namespace WarOrgan.Data.BsdataImport;

/// <summary>
/// Reads a model's starting loadout and wargear choices from its BSData entry. Items are weapon IDs where the
/// wargear has weapon profiles, otherwise a wargear definition key.
/// </summary>
public class WargearCatalog(BsData data)
{
    public HashSet<string> KnownWeapons { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, WargearDefinition> Definitions { get; } = new(StringComparer.Ordinal);

    /// <summary>Wargear every model of this loadout starts with.</summary>
    public List<string> Mandatory(BsNode model)
    {
        var items = new List<string>();
        foreach (var child in model.Children(data))
        {
            if (IsContainer(child))
                items.AddRange(Mandatory(child));
            else if (child.IsGroup)
            {
                var chosen = child.DefaultChild(data) ?? (child.Children(data).Count() == 1 ? child.Children(data).First() : null);
                if (chosen != null && (child.Min ?? 0) >= 1)
                    items.AddRange(ItemsOf(chosen));
            }
            else if (!child.IsModel && (child.Min ?? 0) >= 1)
                for (var i = 0; i < child.Min; i++)
                    items.AddRange(ItemsOf(child));
        }
        return items;
    }

    /// <summary>Choice groups and optional extras on the base loadout, as wargear options.</summary>
    public IEnumerable<WargearOption> Choices(BsNode model, int modelCount, string section, ImportReport report)
    {
        foreach (var child in model.Children(data))
        {
            if (child.IsModel)
                continue;
            if (child.Link.Arr("constraints").Concat(child.Target.Arr("constraints")).Any(c => c.Str("scope") is "unit" or "roster" or "force"))
                report.Add(section, $"\"{child.Name}\" has a unit- or army-wide limit; check the option's Max.");

            if (IsContainer(child))
            {
                foreach (var option in Choices(child, modelCount, section, report))
                    yield return option;
                continue;
            }

            var max = (child.Max ?? 1) * Math.Max(modelCount, 1);
            if (child.IsGroup && child.Children(data).Count() > 1)
            {
                var defaultChild = child.DefaultChild(data);
                var replaces = defaultChild != null && (child.Min ?? 0) >= 1 ? ItemsOf(defaultChild) : null;
                var choices = child.Children(data)
                    .Where(c => replaces == null || !ReferenceEquals(c.Target, defaultChild!.Target))
                    .Select(c => new WargearChoice { Items = ItemsOf(c) })
                    .Where(c => c.Items.Count > 0)
                    .ToList();
                if (choices.Count > 0)
                    yield return new WargearOption { Max = max, Replaces = replaces, Options = choices };

                // Choices nested inside the default loadout, e.g. swapping only the pistol
                if (defaultChild != null)
                    foreach (var nested in Choices(defaultChild, modelCount, section, report))
                        yield return nested;
            }
            else if ((child.Min ?? 0) == 0)
            {
                var items = child.IsGroup ? child.Children(data).SelectMany(ItemsOf).ToList() : ItemsOf(child);
                if (items.Count > 0)
                    yield return new WargearOption { Max = max, Options = [new WargearChoice { Items = items }] };
            }
        }
    }

    /// <summary>Required choice groups with no default, e.g. a heavy weapon model choosing which heavy weapon it carries.</summary>
    public List<BsNode> RequiredChoiceGroups(BsNode model)
        => model.Children(data).Where(c => c.IsGroup && (c.Min ?? 0) >= 1 && c.DefaultChild(data) == null && c.Children(data).Count() > 1).ToList();

    /// <summary>Groups that only organise entries, such as "Wargear Options", with no limit or default of their own.</summary>
    private bool IsContainer(BsNode node)
        => node.IsGroup && node.Min == null && node.Max == null && node.DefaultChild(data) == null
            && node.Children(data).Any(c => c.IsGroup || (c.Min ?? 0) >= 1);

    public List<string> ItemsOf(BsNode node)
    {
        var profiles = node.Profiles(data).ToList();
        var weapons = profiles.Where(p => p.Str("typeName") is "Ranged Weapons" or "Melee Weapons")
            .Select(p => TextUtils.ToId(p.Name())).Distinct().ToList();
        if (weapons.Count > 0)
            return weapons;

        var abilities = profiles.Where(p => p.Str("typeName") == "Abilities").Select(p => p.Name()).ToList();
        if (abilities.Count == 0 && node.Children(data).Any())
        {
            var items = Mandatory(node);
            if (items.Count > 0)
                return items;
        }

        var key = TextUtils.ToId(node.Name);
        if (!Definitions.ContainsKey(key))
            Definitions[key] = new WargearDefinition
            {
                Key = key,
                DisplayName = TextUtils.CleanName(node.Name),
                Cost = (int)(node.Target.Cost(BsData.PointsTypeId) ?? 0),
                GrantsAbilities = abilities.Count > 0 ? abilities : null,
            };
        return [key];
    }
}
