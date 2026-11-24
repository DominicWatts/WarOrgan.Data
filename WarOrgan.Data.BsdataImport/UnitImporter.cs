using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using WarOrgan.Model.Unit;
using WarOrgan.Model.Unit.Wargear;

namespace WarOrgan.Data.BsdataImport;

/// <summary>
/// Converts one BSData unit entry into a draft WarOrgan unit. Per-loadout model entries are folded into one
/// model composition per model type, with the loadouts expressed as wargear options.
/// </summary>
public class UnitImporter(BsData data, ImportSettings settings, ImportReport report)
{
    // Legal model totals for each composition choice of the unit being converted
    private List<(int Min, int Max)> _sizeRanges = [];

    private static readonly string[] CategoryKeywords = ["Character", "Battleline", "Dedicated Transport"];
    private static readonly string[] StatIds = ["M", "T", "Sv", "W", "LD", "OC"];

    public Unit Convert(JsonObject entry, IReadOnlySet<string> knownUnitIds)
    {
        var root = new BsNode(entry, entry);
        var name = UnitName(entry);
        var section = name;
        var unit = new Unit { Id = name.ToUpperInvariant(), DisplayName = name };

        AddKeywords(unit, root, section);
        var statlines = AddStatlines(unit, root, section);
        var wargear = new WargearCatalog(data);
        AddWeapons(unit, root, wargear, section);
        AddAbilities(unit, root, statlines, section, knownUnitIds);
        AddComposition(unit, root, wargear, section);
        AddPoints(unit, root, section);

        unit.UnitComposition.WargearDefinitions = wargear.Definitions.Values.OrderBy(d => d.Key).ToList();
        return unit;
    }

    public static string UnitName(JsonObject entry) => Regex.Replace(entry.Name(), @"\s*\[Legends\]$", string.Empty).Trim();

    private void AddKeywords(Unit unit, BsNode root, string section)
    {
        var categories = root.Target.Arr("categoryLinks").Select(c => c.Name()).ToList();
        foreach (var category in categories)
        {
            if (category.StartsWith("Faction: ", StringComparison.Ordinal))
            {
                var faction = category["Faction: ".Length..];
                if (settings.FactionKeywords.Contains(faction, StringComparer.OrdinalIgnoreCase))
                    unit.FactionKeywords.Add(faction);
                else
                    unit.Keywords.Add(faction);
            }
            else if (settings.IsCoreAbility(category))
                continue; // BSData marks Leader and Support as categories; the dataset holds them as core abilities
            else if (!unit.Keywords.Contains(category, StringComparer.OrdinalIgnoreCase))
                unit.Keywords.Add(category);
        }

        var matched = CategoryKeywords.Where(k => unit.Keywords.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList();
        if (matched.Count == 0)
            unit.Keywords.Add("Other Datasheets");
        else if (matched.Count > 1)
            report.Add(section, $"Has several category keywords ({string.Join(", ", matched)}); keep exactly one category keyword.");

        if (unit.FactionKeywords.Count == 0)
            report.Add(section, "No registered faction keyword found; set FactionKeywords.");
    }

    private List<StatLine> AddStatlines(Unit unit, BsNode root, string section)
    {
        var profiles = AllProfiles(root).Where(p => p.Str("typeName") == "Unit")
            .GroupBy(p => p.Name()).Select(g => g.First()).ToList();
        var invulnerable = new List<string>();

        foreach (var profile in profiles)
        {
            var statline = new StatLine { StatName = profile.Name(), Features = StatFeature.None };
            foreach (var id in StatIds)
            {
                statline.Stats.Add(new Characteristic { Id = id, Value = profile.Characteristic(id) });
                if (id == "Sv")
                {
                    var save = profile.Characteristic("InSv");
                    if (save.Length > 0 && save != "-")
                        statline.Stats.Add(new Characteristic { Id = "Inv", Value = save });
                    invulnerable.Add(save.Length > 0 && save != "-" ? save : "none");
                }
            }
            statline.Stats.Add(new Characteristic { Id = "Base", Value = TextUtils.CompactBase(profile.Characteristic("Base")) });
            unit.StatLines.Add(statline);
        }

        // Models with identical characteristics share one statline named after the unit
        if (unit.StatLines.Count > 1 && unit.StatLines.Select(StatsKey).Distinct().Count() == 1)
        {
            unit.StatLines[0].StatName = UnitName(root.Target);
            unit.StatLines.RemoveRange(1, unit.StatLines.Count - 1);
        }

        var distinctInvulnerable = invulnerable.Distinct().ToList();
        if (distinctInvulnerable.Count > 1)
            report.Add(section, $"Statlines have different invulnerable saves ({string.Join(", ", distinctInvulnerable)}); add model-specific ability text by hand if needed.");
        if (invulnerable.Any(save => save != "none"))
        {
            unit.CoreAbilities.Add("Invulnerable Save");
            if (distinctInvulnerable is [var save] && save != "none")
            {
                var subject = root.IsModel ? "This model has" : "Models in this unit have";
                unit.UnitAbilities.Add(new UnitInfoSection { Title = "Invulnerable Save", Text = $"{subject} a {save} invulnerable save." });
            }
        }

        if (unit.StatLines.Count == 0)
            report.Add(section, "No statline found.");
        return unit.StatLines;
    }

    private static string StatsKey(StatLine statline) => string.Join("|", statline.Stats.Select(s => $"{s.Id}={s.Value}"));

    private void AddWeapons(Unit unit, BsNode root, WargearCatalog wargear, string section)
    {
        var sets = new (string Type, string Id, string Display, string Skill)[]
        {
            ("Ranged Weapons", "RANGED WEAPONS", "Ranged Weapons", "BS"),
            ("Melee Weapons", "MELEE WEAPONS", "Melee Weapons", "WS"),
        };

        foreach (var (type, id, display, skill) in sets)
        {
            var weapons = AllProfiles(root).Where(p => p.Str("typeName") == type)
                .GroupBy(p => TextUtils.ToId(p.Name())).Select(g => g.First())
                .Select(p => ToWeapon(p, skill))
                .OrderBy(w => w.Id, StringComparer.Ordinal)
                .ToList();
            if (weapons.Count > 0)
                unit.Weapons.Add(new WeaponSet { Id = id, DisplayName = display, Weapons = weapons });
        }

        // Weapon IDs are what wargear items refer to
        foreach (var weapon in unit.Weapons.SelectMany(s => s.Weapons))
        {
            wargear.KnownWeapons.Add(weapon.Id!);
            foreach (var keyword in (weapon.Keywords ?? string.Empty).Split(", ", StringSplitOptions.RemoveEmptyEntries))
                if (!settings.IsWeaponAbility(keyword))
                    report.Add(section, $"Weapon keyword \"{keyword}\" on {weapon.DisplayName} is not registered; register it or correct it.");
        }
    }

    private static Weapon ToWeapon(JsonObject profile, string skill)
    {
        var displayName = TextUtils.CleanName(profile.Name());
        var weapon = new Weapon { Id = TextUtils.ToId(profile.Name()), DisplayName = displayName, Flags = WeaponFlags.None };
        foreach (var stat in new[] { "Range", "A", skill, "S", "AP", "D" })
            weapon.Stats.Add(new Characteristic { Id = stat, Value = profile.Characteristic(stat) });
        var keywords = TextUtils.SortKeywords(profile.Characteristic("Keywords"));
        weapon.Keywords = keywords.Length > 0 ? keywords : null;
        return weapon;
    }

    private void AddAbilities(Unit unit, BsNode root, List<StatLine> statlines, string section, IReadOnlySet<string> knownUnitIds)
    {
        // Core abilities are rule links on the unit and its models
        var ruleHolders = new[] { root }.Concat(root.Descendants(data).Where(d => d.IsModel || d.IsGroup));
        foreach (var info in ruleHolders.SelectMany(n => n.Target.Arr("infoLinks")).Where(i => i.Str("type") == "rule"))
        {
            var ruleName = ApplyNameModifiers(info);
            if (info.Arr("modifiers").Any(IsUnitConditionalHide))
            {
                report.Add(section, $"Rule \"{ruleName}\" only applies under a condition (for example while led by a given character); model it as a leader or detachment effect if needed.");
                continue;
            }
            if (settings.ArmyRuleIds.Contains(info.Str("targetId") ?? string.Empty))
                continue;
            if (unit.CoreAbilities.Contains(ruleName, StringComparer.OrdinalIgnoreCase))
                continue;
            if (settings.IsCoreAbility(ruleName))
                unit.CoreAbilities.Add(ruleName);
            else if (settings.IsCoreAbility(info.Name()))
            {
                unit.CoreAbilities.Add(info.Name());
                report.Add(section, $"Rule \"{ruleName}\" is not registered with that value; used \"{info.Name()}\". Add the value to the registry pattern if the app should show it.");
            }
            else
                report.Add(section, $"Rule \"{ruleName}\" is not a registered core ability; register it, rename it or drop it.");
        }

        foreach (var profile in AllProfiles(root).Where(p => p.Str("typeName") is "Abilities" or "Transport"))
        {
            var title = profile.Str("typeName") == "Transport" ? "Transport" : profile.Name();
            var text = TextUtils.CleanRuleText(profile.Characteristic(profile.Str("typeName") == "Transport" ? "Capacity" : "Description"));

            if (title.Equals("Leader", StringComparison.OrdinalIgnoreCase))
            {
                AddLeader(unit, text, section, knownUnitIds);
                continue;
            }
            if (title.Equals("Unit Composition", StringComparison.OrdinalIgnoreCase))
            {
                unit.Infos.Add(new UnitInfoSection { Title = "Unit composition", Text = text.Trim('*').Trim() });
                continue;
            }
            if (unit.UnitAbilities.Any(a => a.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
                continue;
            unit.UnitAbilities.Add(new UnitInfoSection { Title = title, Text = text });
        }

        if (unit.CoreAbilities.Contains("Support", StringComparer.OrdinalIgnoreCase) && unit.LeaderInfo != null)
        {
            // Support characters attach through the Support rule rather than Leader
            unit.LeaderInfo.LeaderType = LeaderInfo.SupportLeaderId;
            unit.CoreAbilities.RemoveAll(a => a.Equals("Leader", StringComparison.OrdinalIgnoreCase));
        }

        var leaderCategory = root.Target.Arr("categoryLinks").Any(c => c.Name() is "Leader" or "Support");
        if (leaderCategory && unit.LeaderInfo == null)
            report.Add(section, "BSData marks this as a Leader or Support character but lists no units it can attach to; add LeaderInfo from Wahapedia.");
    }

    private void AddLeader(Unit unit, string text, string section, IReadOnlySet<string> knownUnitIds)
    {
        var info = new LeaderInfo();
        var listed = Regex.Matches(text, @"^\s*[-■•]\s*(.+?)\s*\**\s*$", RegexOptions.Multiline)
            .Select(m => m.Groups[1].Value)
            .ToList();

        // Some catalogues put a short Leader list on the same line as the introductory sentence instead of
        // using bullets. Treat both forms identically so ordinary leaders do not require manual repair.
        var header = Regex.Match(text,
            @"^[ \t]*This (?:model|unit) can be attached to the following units?:[ \t]*(?<targets>[^\r\n]*)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        if (header.Success && header.Groups["targets"].Value.Trim().Length > 0)
            listed.AddRange(header.Groups["targets"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        foreach (var listedName in listed)
        {
            var name = listedName.Replace("*", string.Empty).Trim().TrimEnd('.').ToUpperInvariant();
            var normalised = name.Replace("HANDFLAMERS", "HAND FLAMERS");
            var matches = knownUnitIds.Contains(normalised)
                ? [normalised]
                : knownUnitIds.Where(id => id.StartsWith(normalised + " ", StringComparison.Ordinal)).ToList();
            if (matches.Count == 0)
                report.Add(section, $"Leader target \"{name}\" is not in this roster; add its unit ID by hand if it lives elsewhere.");
            info.UnitNames.AddRange(matches.Except(info.UnitNames));
        }

        var extra = Regex.Replace(text, @"^\s*[-■•].*$", string.Empty, RegexOptions.Multiline);
        extra = Regex.Replace(extra,
            @"^\s*This (?:model|unit) can be attached to the following units?:.*$",
            string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.Multiline).Trim();
        if (extra.Length > 0)
            report.Add(section, $"Leader has extra attachment rules to model by hand: \"{extra}\"");

        unit.LeaderInfo = info;
        if (!unit.CoreAbilities.Contains("Leader", StringComparer.OrdinalIgnoreCase))
            unit.CoreAbilities.Add("Leader");
    }

    /// <summary>
    /// Hidden-unless modifiers tied to the unit itself (such as "while led by a Locus"). Army-wide conditions,
    /// such as hiding Deep Strike in Boarding Actions, are ignored.
    /// </summary>
    private static bool IsUnitConditionalHide(JsonObject modifier)
    {
        if (modifier.Str("field") != "hidden")
            return false;
        var conditions = modifier.Arr("conditions").Concat(modifier.Arr("conditionGroups").SelectMany(g => g.Arr("conditions"))).ToList();
        return conditions.Count > 0 && conditions.All(c => c.Str("scope") is not ("force" or "roster" or "primary-catalogue"));
    }

    private static string ApplyNameModifiers(JsonObject info)
    {
        var name = info.Name();
        foreach (var modifier in info.Arr("modifiers").Where(m => m.Str("field") == "name"))
        {
            var value = modifier.Str("value") ?? string.Empty;
            name = modifier.Str("type") switch
            {
                "append" => $"{name} {value}",
                "set" => value,
                _ => name,
            };
        }
        return name.Trim();
    }

    private IEnumerable<JsonObject> AllProfiles(BsNode root)
        => new[] { root }.Concat(root.Descendants(data)).SelectMany(n => n.Profiles(data));

    // ---------------------------------------------------------------- composition

    private void AddComposition(Unit unit, BsNode root, WargearCatalog wargear, string section)
    {
        var alternatives = CompositionAlternatives(root);
        var byModel = new Dictionary<string, List<ModelSlot>>(StringComparer.OrdinalIgnoreCase);
        var altTotals = new List<(int Min, int Max)>();

        foreach (var alternative in alternatives)
        {
            var slots = ModelSlots(alternative, root);
            foreach (var slot in slots)
            {
                if (!byModel.TryGetValue(slot.BaseName, out var list))
                    byModel[slot.BaseName] = list = new List<ModelSlot>();
                list.Add(slot);
            }
            altTotals.Add((slots.Sum(s => s.Min), slots.Sum(s => s.Max)));
        }

        _sizeRanges = altTotals;
        var smallestTotal = altTotals.Count > 0 ? altTotals.Min(t => t.Max) : 1;
        foreach (var (baseName, slots) in byModel)
        {
            var composition = new ModelComposition
            {
                ModelName = baseName,
                Limit = new ModelCountLimits { Min = slots.Min(s => s.Min), Max = slots.Max(s => s.Max) },
                StatlineRef = StatlineFor(unit, baseName, section),
            };

            var first = slots[0];
            var baseVariant = first.BaseVariant;
            var modelWargear = new ModelWargear { InitialWargear = wargear.Mandatory(baseVariant) };
            modelWargear.Options.AddRange(wargear.Choices(baseVariant, composition.Limit.Max, section, report));

            // Loadout variants become options against the base loadout
            foreach (var variantGroup in first.Variants)
            {
                var maxes = slots.Select(s => s.Variants.FirstOrDefault(v => v.Key == variantGroup.Key)?.Max).ToList();
                var option = VariantOption(baseVariant, variantGroup, wargear, section);
                if (option == null)
                    continue;

                var perAlt = maxes.Zip(altTotals, (max, total) => (max, total.Max)).Where(x => x.max != null).ToList();
                if (perAlt.Count > 1 && perAlt.All(x => x.max * smallestTotal == perAlt[0].max * x.Item2))
                {
                    option.Max = perAlt.First(x => x.Item2 == smallestTotal).max;
                    option.PerXModels = smallestTotal;
                }
                else
                {
                    option.Max = maxes.Max();
                    if (perAlt.Count > 1)
                        report.Add(section, $"Option \"{variantGroup.Key}\" has limits that do not scale evenly with unit size; check Max and PerXModels.");
                }
                if (variantGroup.HasLimitModifiers)
                    report.Add(section, $"Option \"{variantGroup.Key}\" limit changes with modifiers (usually per-X-models); check Max and PerXModels.");
                modelWargear.Options.Add(option);
            }

            modelWargear.Options = MergeReplacementChoices(modelWargear.Options, composition.Limit.Max);

            composition.Wargear.Add(modelWargear);
            unit.UnitComposition.ModelCompositions.Add(composition);
        }

        unit.UnitComposition.ModelCompositions = unit.UnitComposition.ModelCompositions.OrderBy(m => m.Limit?.Max ?? 1).ToList();
        if (alternatives.Count > 1)
            report.Add(section, $"Unit sizes come from {alternatives.Count} composition choices ({string.Join("; ", alternatives.Select(a => a.Name))}); model ranges were merged, so check that impossible mixes are not allowed.");
    }

    private List<BsNode> CompositionAlternatives(BsNode root)
    {
        // A required one-of group whose choices contain models, e.g. "1 Sergeant and 9 Troopers" or "2 Sergeants and 18 Troopers"
        var choice = root.Children(data).FirstOrDefault(c => c.IsGroup && c.Min == 1 && c.Max == 1
            && c.Children(data).Count() > 1 && c.Children(data).All(a => a.Descendants(data).Any(d => d.IsModel)));
        return choice != null ? choice.Children(data).ToList() : [root];
    }

    private List<ModelSlot> ModelSlots(BsNode alternative, BsNode root)
    {
        if (alternative.IsModel && ReferenceEquals(alternative, root))
            return [new ModelSlot(UnitName(root.Target), 1, 1, alternative, [])];

        var found = new List<(BsNode Model, List<BsNode> Groups)>();
        void Walk(BsNode node, List<BsNode> groups)
        {
            foreach (var child in node.Children(data))
            {
                if (child.IsModel)
                    found.Add((child, groups));
                else if (child.IsGroup || child.Type == "upgrade")
                    Walk(child, child.IsGroup ? [.. groups, child] : groups);
            }
        }
        Walk(alternative, []);

        var slots = new List<ModelSlot>();
        foreach (var group in found.GroupBy(f => f.Model.BaseModelName, StringComparer.OrdinalIgnoreCase))
        {
            var models = group.ToList();
            var baseVariant = models.FirstOrDefault(m => !m.Model.Name.Contains(" w/ ")).Model
                ?? models.OrderByDescending(m => m.Model.Max ?? 0).ThenByDescending(m => m.Model.Min ?? 0).First().Model;

            // The deepest group that holds every variant of this model and sets a count gives the model range
            var common = models.Select(m => m.Groups).Aggregate((a, b) => a.Zip(b).TakeWhile(p => p.First.Id == p.Second.Id).Select(p => p.First).ToList());
            var countGroup = common.LastOrDefault(g => g.Min != null || g.Max != null);
            var min = countGroup?.Min ?? models.Sum(m => m.Model.Min ?? 0);
            var max = countGroup?.Max ?? models.Sum(m => m.Model.Max ?? m.Model.Min ?? 1);
            if (countGroup == null && models.Count == 1)
                (min, max) = (baseVariant.Min ?? 1, baseVariant.Max ?? baseVariant.Min ?? 1);

            // Some groups only state their range in the name, e.g. "1-2 Atalan Wolfquads"
            var namedRange = common.Select(g => Regex.Match(g.Name, @"^(\d+)-(\d+)\s")).LastOrDefault(m => m.Success);
            if (countGroup == null && namedRange != null && int.Parse(namedRange.Groups[2].Value) != max)
            {
                report.Add(UnitName(root.Target), $"\"{group.Key}\" count {min}-{max} disagrees with its group name; used the name ({namedRange.Value.Trim()}). Check it.");
                (min, max) = (int.Parse(namedRange.Groups[1].Value), int.Parse(namedRange.Groups[2].Value));
            }

            // Other loadouts: either a single variant model or a group of alternative variant models
            var variants = new List<VariantGroup>();
            foreach (var (model, groups) in models.Where(m => !ReferenceEquals(m.Model, baseVariant)))
            {
                var parent = groups.Count > common.Count ? groups[common.Count] : null;
                if (parent != null && parent.Max != null)
                {
                    var existing = variants.FirstOrDefault(v => v.Key == parent.Name);
                    if (existing == null)
                        variants.Add(existing = new VariantGroup(parent.Name, parent.Max, parent.HasLimitModifiers, []));
                    existing.Models.Add(model);
                }
                else
                    variants.Add(new VariantGroup(model.Name, model.Max, model.HasLimitModifiers, [model]));
            }

            slots.Add(new ModelSlot(group.Key, min, max, baseVariant, variants));
        }
        return slots;
    }

    private WargearOption? VariantOption(BsNode baseVariant, VariantGroup variants, WargearCatalog wargear, string section)
    {
        var baseItems = wargear.Mandatory(baseVariant);
        var replaced = new List<List<string>>();
        var choices = new List<WargearChoice>();
        foreach (var model in variants.Models)
        {
            var items = wargear.Mandatory(model);
            replaced.Add(MultisetExcept(baseItems, items));
            var added = MultisetExcept(items, baseItems);

            // A loadout that picks its weapon from a list gives one choice per weapon
            var picks = wargear.RequiredChoiceGroups(model);
            if (picks.Count > 1)
                report.Add(section, $"\"{model.Name}\" picks from several weapon lists; only the first was read.");
            if (picks.Count > 0)
                choices.AddRange(picks[0].Children(data).Select(c => new WargearChoice { Items = [.. added, .. wargear.ItemsOf(c)] }));
            else if (added.Count > 0)
                choices.Add(new WargearChoice { Items = added });

            foreach (var limited in picks.SelectMany(p => p.Children(data)).Where(c => c.Link.Arr("constraints").Concat(c.Target.Arr("constraints")).Any(k => k.Str("scope") != "parent")))
                report.Add(section, $"\"{limited.Name}\" has its own unit-wide limit; WarOrgan options cannot cap a single choice, so note it or split the option.");
        }

        if (choices.Count == 0)
            return null;
        var replaces = replaced.First();
        if (replaced.Any(r => !r.SequenceEqual(replaces)))
            report.Add(section, $"Option \"{variants.Key}\" choices replace different items ({string.Join(" / ", replaced.Select(r => string.Join(", ", r)))}); split it into separate options.");
        return new WargearOption { Replaces = replaces.Count > 0 ? replaces : null, Options = choices };
    }

    private static List<string> MultisetExcept(List<string> source, List<string> remove)
    {
        var result = new List<string>(source);
        foreach (var item in remove)
            result.Remove(item);
        return result;
    }

    /// <summary>
    /// BSData often represents alternatives as sibling loadout entries (one entry for an autocannon, another
    /// for a lascannon, and so on). WarOrgan must receive those as one option with several choices; separate
    /// options would allow the same starting weapon to be replaced more than once.
    /// </summary>
    private static List<WargearOption> MergeReplacementChoices(List<WargearOption> options, int modelCount)
    {
        var result = new List<WargearOption>();
        foreach (var option in options)
        {
            // A lower limit can represent a fixed specialist slot. Those choices must remain separate: several
            // different specialists may each replace identical basic wargear. A full-model-count limit instead
            // means every model is choosing from the same weapon list and is safe to combine.
            if (option.Replaces is not { Count: > 0 } || option.Max != modelCount)
            {
                result.Add(option);
                continue;
            }

            var existing = result.FirstOrDefault(candidate =>
                candidate.Max == option.Max
                && candidate.PerXModels == option.PerXModels
                && SameMultiset(candidate.Replaces, option.Replaces));
            if (existing == null)
            {
                result.Add(option);
                continue;
            }

            foreach (var choice in option.Options)
                if (!existing.Options.Any(current => current.Items.SequenceEqual(choice.Items)))
                    existing.Options.Add(choice);
        }
        return result;
    }

    private static bool SameMultiset(IEnumerable<string>? left, IEnumerable<string>? right)
        => (left ?? []).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(
            (right ?? []).OrderBy(x => x, StringComparer.Ordinal), StringComparer.Ordinal);

    private string? StatlineFor(Unit unit, string modelName, string section)
    {
        if (unit.StatLines.Count <= 1)
            return unit.StatLines.FirstOrDefault()?.StatName;
        var match = unit.StatLines.FirstOrDefault(s => string.Equals(s.StatName, modelName, StringComparison.OrdinalIgnoreCase))
            ?? unit.StatLines.FirstOrDefault(s => modelName.Contains(s.StatName!, StringComparison.OrdinalIgnoreCase)
                || s.StatName!.Contains(modelName, StringComparison.OrdinalIgnoreCase));
        if (match == null)
            report.Add(section, $"Model \"{modelName}\" has no matching statline; set StatlineRef.");
        return (match ?? unit.StatLines[0]).StatName;
    }

    // ---------------------------------------------------------------- points

    private void AddPoints(Unit unit, BsNode root, string section)
    {
        var baseCost = root.Target.Cost(BsData.PointsTypeId) ?? 0;
        var sizeModifiers = new List<(int Value, List<JsonObject> Conditions)>();
        var copyIncrements = new List<(int AfterCopies, int Value)>();

        foreach (var modifier in root.Target.Arr("modifiers").Where(m => m.Str("field") == BsData.PointsTypeId))
        {
            var value = (int)(modifier.Num("value") ?? 0);
            var conditions = modifier.Arr("conditions").ToList();
            var groups = modifier.Arr("conditionGroups").ToList();

            var copyCondition = groups.SelectMany(g => g.Arr("localConditionGroups"))
                .FirstOrDefault(l => l.Arr("conditions").Any(c => c.Str("type") == "before"));
            if (modifier.Str("type") == "increment" && copyCondition != null)
            {
                copyIncrements.Add(((int)(copyCondition.Num("value") ?? 0), value));
                continue;
            }

            conditions.AddRange(groups.Where(g => g.Str("type") == "and").SelectMany(g => g.Arr("conditions")));
            if (modifier.Str("type") == "set" && conditions.Count > 0 && conditions.All(IsModelCountCondition)
                && groups.All(g => g.Str("type") == "and"))
                sizeModifiers.Add((value, conditions));
            else
                report.Add(section, $"Points modifier ({modifier.Str("type")} {value}) has conditions this importer does not read; add it by hand.");
        }

        // Points apply at each legal size: the ends of every composition choice, plus thresholds inside a range
        var ranges = _sizeRanges.Count > 0 ? _sizeRanges : [(1, 1)];
        // Points apply at the ends of each size range; BSData thresholds such as "more than 5 models" mark the larger size
        var counts = new SortedSet<int>(ranges.SelectMany(r => new[] { r.Min, r.Max }));
        var interior = sizeModifiers.SelectMany(m => m.Conditions)
            .Select(c => c.Str("type") switch { "greaterThan" => (int)(c.Num("value") ?? 0) + 1, "atLeast" or "equalTo" => (int)(c.Num("value") ?? 0), "lessThan" => (int)(c.Num("value") ?? 0) - 1, _ => -1 })
            .Where(n => ranges.Any(r => n > r.Min && n < r.Max) && ranges.All(r => n != r.Min + 1 || n == r.Max))
            .Distinct().ToList();
        if (interior.Count > 0)
            report.Add(section, $"BSData sets a points threshold inside the unit size range (at {string.Join(", ", interior)} models); add intermediate sizes from Wahapedia if they are legal.");

        int CostAt(int models)
        {
            var cost = (int)baseCost;
            foreach (var (value, conditions) in sizeModifiers)
                if (conditions.All(c => ConditionHolds(c, models)))
                    cost = value;
            return cost;
        }

        var sizes = counts.Select(n => (Models: n, Cost: CostAt(n))).ToList();
        if (copyIncrements.Count == 0)
        {
            unit.Points.AddRange(sizes.Select(s => new PointsDefinition { ModelCount = s.Models, Cost = s.Cost }));
        }
        else
        {
            // Copies after the threshold cost more, e.g. the fourth Goliath Truck
            var lastCopy = copyIncrements.Max(i => i.AfterCopies) + 1;
            for (var copy = 1; copy <= lastCopy; copy++)
                foreach (var size in sizes)
                    unit.Points.Add(new PointsDefinition
                    {
                        UnitCount = copy,
                        ModelCount = size.Models,
                        Cost = size.Cost + copyIncrements.Where(i => copy > i.AfterCopies).Sum(i => i.Value),
                    });
        }

    }

    private static bool IsModelCountCondition(JsonObject condition)
        => condition.Str("childId") == "model" && condition.Str("field") == "selections"
            && condition.Str("type") is "greaterThan" or "atLeast" or "lessThan" or "atMost" or "equalTo" or "notEqualTo";

    private static bool ConditionHolds(JsonObject condition, int models)
    {
        var n = condition.Num("value") ?? 0;
        return condition.Str("type") switch
        {
            "greaterThan" => models > n,
            "atLeast" => models >= n,
            "lessThan" => models < n,
            "atMost" => models <= n,
            "equalTo" => models == n,
            "notEqualTo" => models != n,
            _ => false,
        };
    }

    private record ModelSlot(string BaseName, int Min, int Max, BsNode BaseVariant, List<VariantGroup> Variants);

    private record VariantGroup(string Key, int? Max, bool HasLimitModifiers, List<BsNode> Models);
}
