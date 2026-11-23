using System.Text.RegularExpressions;
using WarOrgan.Model.Faction;
using WarOrgan.Model.Unit;

namespace WarOrgan.Data.WahapediaCheck;

/// <summary>Compares a WarOrgan roster with Wahapedia and lists every difference found.</summary>
public class RosterComparer(Report report)
{
    // Headings the app needs that are not Wahapedia keywords
    private static readonly HashSet<string> DatasetOnlyKeywords = new(StringComparer.OrdinalIgnoreCase) { "Other Datasheets", "Allied Units" };

    public void CompareUnits(FactionRoster roster, List<WDatasheet> sheets, string factionKeyword, bool legends)
    {
        var faction = Norm(factionKeyword);
        var factionSheets = sheets.Where(s => s.Legends == legends && s.FactionKeywords.Any(k => Norm(k) == faction)).ToList();
        var byName = sheets.GroupBy(s => Norm(s.Name)).ToDictionary(g => g.Key, g => g.First());

        foreach (var unit in roster.Units)
        {
            var name = unit.DisplayName ?? unit.Id;
            if (!byName.TryGetValue(Norm(name), out var sheet))
            {
                report.Add(name, "Not found on Wahapedia (check the name, or whether it has been removed).");
                continue;
            }
            var section = name;
            CompareStatlines(section, unit, sheet);
            CompareWeapons(section, unit, sheet);
            CompareCoreAbilities(section, unit, sheet);
            CompareAbilities(section, unit, sheet);
            CompareComposition(section, unit, sheet);
            ComparePoints(section, unit, sheet);
            CompareLeader(section, unit, sheet);
            CompareKeywords(section, unit, sheet);
            report.Checked(section);
        }

        var local = roster.Units.Select(u => Norm(u.DisplayName ?? u.Id)).ToHashSet();
        foreach (var missing in factionSheets.Where(s => !local.Contains(Norm(s.Name))))
            report.Add(legends ? "Legends datasheets not in this roster" : "Datasheets not in this roster", missing.Name);
    }

    private void CompareStatlines(string section, Unit unit, WDatasheet sheet)
    {
        foreach (var (profile, index) in sheet.Profiles.Select((p, i) => (p, i)))
        {
            var statline = unit.StatLines.Count == 1 ? unit.StatLines[0]
                : unit.StatLines.FirstOrDefault(s => profile.ModelName != null && SameModel(s.StatName, profile.ModelName, exact: true))
                  ?? unit.StatLines.FirstOrDefault(s => profile.ModelName != null && SameModel(s.StatName, profile.ModelName))
                  ?? unit.StatLines.ElementAtOrDefault(index);
            if (statline == null)
            {
                report.Add(section, $"Statline for {profile.ModelName ?? "the unit"} missing.");
                continue;
            }
            var label = profile.ModelName ?? statline.StatName;
            foreach (var (id, value) in profile.Stats)
            {
                var localId = id == "SV" ? "Sv" : id;
                var local = statline.Stats.FirstOrDefault(s => string.Equals(s.Id, localId, StringComparison.OrdinalIgnoreCase))?.Value;
                if (Norm(local) != Norm(value))
                    report.Add(section, $"{label} {localId}: Wahapedia {value}, dataset {local ?? "missing"}.");
            }

            var baseSize = CompactBase(profile.Base);
            var localBase = statline.Stats.FirstOrDefault(s => s.Id == "Base")?.Value ?? string.Empty;
            if (baseSize != null && !localBase.Split(',').Select(b => b.Trim()).Contains(baseSize, StringComparer.OrdinalIgnoreCase))
                report.Add(section, $"{label} base: Wahapedia {baseSize}, dataset {localBase}.");
        }

        var invulnerable = unit.UnitAbilities.FirstOrDefault(a => a.Title.Equals("Invulnerable Save", StringComparison.OrdinalIgnoreCase));
        var localSave = invulnerable == null ? null : Regex.Match(invulnerable.Text, @"\d\+").Value;
        var wahapediaSave = sheet.Invulnerable == null ? null : Regex.Match(sheet.Invulnerable, @"\d\+").Value;
        if (localSave != wahapediaSave)
            report.Add(section, $"Invulnerable save: Wahapedia {wahapediaSave ?? "none"}, dataset {localSave ?? "none"}.");
    }

    private void CompareWeapons(string section, Unit unit, WDatasheet sheet)
    {
        var local = unit.Weapons.SelectMany(s => s.Weapons)
            .GroupBy(w => Norm(w.DisplayName ?? w.Id)).ToDictionary(g => g.Key, g => g.First());
        var seen = new HashSet<string>();
        foreach (var weapon in sheet.Weapons)
        {
            var key = Norm(weapon.Name);
            if (!seen.Add(key))
                continue;
            if (!local.TryGetValue(key, out var mine))
            {
                report.Add(section, $"Weapon \"{weapon.Name}\" missing.");
                continue;
            }
            foreach (var (id, value) in weapon.Stats)
            {
                var localValue = mine.Stats.FirstOrDefault(s => s.Id == id)?.Value;
                if (NormStat(localValue) != NormStat(value))
                    report.Add(section, $"{weapon.Name} {id}: Wahapedia {value}, dataset {localValue ?? "missing"}.");
            }
            var wahapediaKeywords = weapon.Keywords.Select(Norm).OrderBy(k => k).ToList();
            var localKeywords = (mine.Keywords ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Norm).OrderBy(k => k).ToList();
            if (!wahapediaKeywords.SequenceEqual(localKeywords))
                report.Add(section, $"{weapon.Name} keywords: Wahapedia [{string.Join(", ", wahapediaKeywords)}], dataset [{string.Join(", ", localKeywords)}].");
        }
        foreach (var (key, weapon) in local.Where(l => !seen.Contains(l.Key)))
            if (!weapon.Flags.HasFlag(WeaponFlags.NeverFilter))
                report.Add(section, $"Weapon \"{weapon.DisplayName ?? weapon.Id}\" is not on Wahapedia.");
    }

    private void CompareCoreAbilities(string section, Unit unit, WDatasheet sheet)
    {
        var local = unit.CoreAbilities.Where(a => !a.Equals("Invulnerable Save", StringComparison.OrdinalIgnoreCase)).ToList();
        foreach (var ability in sheet.CoreAbilities)
        {
            if (local.Any(l => Norm(l) == Norm(ability)))
                continue;
            // Dataset convention: the core ability without its value, plus an ability that carries the value
            if (local.Any(l => Norm(l) == RuleName(ability)) && unit.UnitAbilities.Any(a => Norm(a.Title) == Norm(ability)))
                continue;
            var sameRule = local.FirstOrDefault(l => RuleName(l) == RuleName(ability));
            report.Add(section, sameRule != null
                ? $"Core ability \"{ability}\" is \"{sameRule}\" in the dataset (value differs)."
                : $"Core ability \"{ability}\" missing.");
        }
        foreach (var ability in local.Where(l => !sheet.CoreAbilities.Any(w => RuleName(w) == RuleName(l))))
        {
            // Leader and Support are shown as sections on Wahapedia rather than core abilities
            if (RuleName(ability) is "leader" or "support" && sheet.LeaderOf != null)
                continue;
            report.Add(section, $"Core ability \"{ability}\" is not on Wahapedia.");
        }
    }

    private void CompareAbilities(string section, Unit unit, WDatasheet sheet)
    {
        var local = unit.UnitAbilities.Where(a => !a.Title.Equals("Invulnerable Save", StringComparison.OrdinalIgnoreCase))
            .GroupBy(a => Norm(a.Title)).ToDictionary(g => g.Key, g => g.First());
        foreach (var (name, text) in sheet.Abilities)
        {
            if (!local.TryGetValue(Norm(name), out var mine))
            {
                report.Add(section, $"Ability \"{name}\" missing.");
                continue;
            }
            if (TextDiff(mine.Text, text) is { } diff)
                report.Add(section, $"Ability \"{name}\" text differs: {diff}");
        }
        foreach (var (key, ability) in local.Where(l => !sheet.Abilities.Keys.Any(k => Norm(k) == l.Key) && !sheet.CoreAbilities.Any(c => Norm(c) == l.Key)))
            report.Add(section, $"Ability \"{ability.Title}\" is not on Wahapedia.");
    }

    private void CompareComposition(string section, Unit unit, WDatasheet sheet)
    {
        var models = unit.UnitComposition.ModelCompositions;
        foreach (var range in sheet.Composition)
        {
            var model = models.FirstOrDefault(m => SameModel(m.ModelName, range.Name, exact: true)) ?? models.FirstOrDefault(m => SameModel(m.ModelName, range.Name));
            if (model == null)
            {
                report.Add(section, $"Model \"{range.Name}\" ({range.Min}-{range.Max}) has no matching model composition.");
                continue;
            }

            // A fixed loadout can be split out, e.g. "Ravener" and "Ravener with venom bolt" together make the 4 Raveners
            var variants = models.Where(m => m == model || Norm(m.ModelName).StartsWith(Norm(model.ModelName) + " with ")).ToList();
            var min = variants.Sum(m => m.Limit?.Min ?? 1);
            var max = variants.Sum(m => m.Limit?.Max ?? 1);
            if (min != range.Min || max != range.Max)
                report.Add(section, $"{range.Name}: Wahapedia {range.Min}-{range.Max}, dataset {min}-{max}.");
        }

        if (sheet.SizeChoices.Count > 0)
        {
            var min = models.Sum(m => m.Limit?.Min ?? 1);
            var max = models.Sum(m => m.Limit?.Max ?? 1);
            var totals = sheet.SizeChoices.Select(c => Regex.Matches(c, @"\d+").Sum(m => int.Parse(m.Value))).ToList();
            if (totals.Min() != min || totals.Max() != max)
                report.Add(section, $"Unit size: Wahapedia {string.Join(" or ", sheet.SizeChoices)}, dataset {min}-{max} models.");
        }

        foreach (var (who, items) in sheet.EquippedWith)
        {
            var exactModels = models.Where(m => SameModel(m.ModelName, who, exact: true)).ToList();
            var targets = Regex.IsMatch(who, @"^models?$", RegexOptions.IgnoreCase) || Norm(who) == Norm(unit.DisplayName ?? unit.Id)
                ? models
                : exactModels.Count > 0 ? exactModels : models.Where(m => SameModel(m.ModelName, who)).ToList();
            // Mixed loadouts are checked against every split-out variant of the model together
            if (sheet.MixedLoadouts.Contains(who) && targets.Count > 0)
            {
                var name = Norm(targets[0].ModelName);
                var group = models.Where(m => Norm(m.ModelName) == name || Norm(m.ModelName).StartsWith(name + " with ")).ToList();
                var all = group.SelectMany(m => m.Wargear.SelectMany(w => w.InitialWargear)).Select(Norm).ToList();
                var absent = items.Select(i => Norm(Regex.Replace(i, @"^\d+\s+", string.Empty))).Where(e => !all.Any(i => SameItem(i, e))).ToList();
                if (absent.Count > 0)
                    report.Add(section, $"{targets[0].ModelName} starting wargear lacks {string.Join(", ", absent)} (Wahapedia lists it for some of these models).");
                continue;
            }

            foreach (var model in targets)
            {
                var initial = model.Wargear.SelectMany(w => w.InitialWargear).Select(Norm).ToList();
                var expected = items.Select(i => Norm(Regex.Replace(i, @"^\d+\s+", string.Empty))).ToList();
                var missing = expected.Where(e => !initial.Any(i => SameItem(i, e))).ToList();
                // Mixed loadouts list several models' gear together, so only missing items are reliable
                var extra = sheet.MixedLoadouts.Contains(who) ? [] : initial.Where(i => !expected.Any(e => SameItem(i, e))).Distinct().ToList();
                if (missing.Count > 0 || extra.Count > 0)
                    report.Add(section, $"{model.ModelName} starting wargear: Wahapedia [{string.Join("; ", items)}], dataset [{string.Join("; ", model.Wargear.SelectMany(w => w.InitialWargear))}].");
            }
        }
    }

    private void ComparePoints(string section, Unit unit, WDatasheet sheet)
    {
        var maxLocalCopy = unit.Points.Max(p => p.UnitCount) ?? 1;
        foreach (var row in sheet.Points)
        {
            var last = row.LastCopy ?? Math.Max(row.FirstCopy, maxLocalCopy);
            for (var copy = row.FirstCopy; copy <= last; copy++)
            {
                var local = unit.Points.Where(p => p.ModelCount == row.Models)
                    .Where(p => p.UnitCount == null || p.UnitCount == Math.Min(copy, maxLocalCopy))
                    .Select(p => (int?)p.Cost).FirstOrDefault();
                if (local != row.Cost)
                {
                    var copies = row.LastCopy == null ? $"{row.FirstCopy}+" : row.FirstCopy == row.LastCopy ? $"{row.FirstCopy}" : $"{row.FirstCopy}-{row.LastCopy}";
                    report.Add(section, $"Points for {row.Models} models (unit copy {copies}): Wahapedia {row.Cost}, dataset {local?.ToString() ?? "missing"}.");
                    break;
                }
            }
        }
        foreach (var size in unit.Points.Select(p => p.ModelCount).Distinct().Where(m => sheet.Points.Count > 0 && sheet.Points.All(w => w.Models != m)))
            report.Add(section, $"Points for {size} models are not a Wahapedia unit size.");
    }

    private void CompareLeader(string section, Unit unit, WDatasheet sheet)
    {
        var local = unit.LeaderInfo?.UnitNames.Select(Norm).ToHashSet() ?? [];
        var wahapedia = sheet.LeaderOf?.Select(Norm).ToHashSet() ?? [];
        foreach (var missing in wahapedia.Except(local))
            report.Add(section, $"Can lead \"{missing.ToUpperInvariant()}\" on Wahapedia but not in the dataset.");
        foreach (var extra in local.Except(wahapedia))
            report.Add(section, $"Can lead \"{extra.ToUpperInvariant()}\" in the dataset but not on Wahapedia.");
    }

    private void CompareKeywords(string section, Unit unit, WDatasheet sheet)
    {
        var local = unit.Keywords.Concat(unit.FactionKeywords).Concat(unit.UnitComposition.ModelCompositions.SelectMany(m => m.Keywords)).Where(k => !DatasetOnlyKeywords.Contains(k)).Select(Norm).ToHashSet();
        var wahapedia = sheet.Keywords.Concat(sheet.FactionKeywords).Select(Norm).ToHashSet();
        var missing = wahapedia.Except(local).ToList();
        var extra = local.Except(wahapedia).ToList();
        if (missing.Count > 0)
            report.Add(section, $"Keywords missing: {string.Join(", ", missing.Select(k => k.ToUpperInvariant()))}.");
        if (extra.Count > 0)
            report.Add(section, $"Keywords not on Wahapedia: {string.Join(", ", extra.Select(k => k.ToUpperInvariant()))}.");
    }

    public void CompareDetachments(FactionRoster roster, List<WDetachment> detachments)
    {
        foreach (var detachment in roster.Detachments)
        {
            var section = $"Detachment: {detachment.Name}";
            var theirs = detachments.FirstOrDefault(d => Norm(d.Name) == Norm(detachment.Name));
            if (theirs == null)
            {
                report.Add(section, "Not found on Wahapedia.");
                continue;
            }
            if (theirs.Cost != detachment.Cost)
                report.Add(section, $"Cost: Wahapedia {theirs.Cost} DP, dataset {detachment.Cost} DP.");

            var localRules = new List<string>();
            if (detachment.Rule != null)
            {
                localRules.Add(detachment.Rule.Title);
                localRules.AddRange(Regex.Matches(detachment.Rule.Text, @"^\*\*(.+?)\*\*\s*$", RegexOptions.Multiline).Select(m => m.Groups[1].Value));
            }
            foreach (var rule in theirs.RuleTitles.Where(r => !localRules.Any(l => Norm(l) == Norm(r))))
                report.Add(section, $"Rule \"{rule}\" missing.");

            foreach (var enhancement in theirs.Enhancements)
            {
                var mine = detachment.Enhancements.FirstOrDefault(e => Norm(e.Name) == Norm(enhancement.Name));
                if (mine == null)
                    report.Add(section, $"Enhancement \"{enhancement.Name}\" missing.");
                else if (mine.Cost != enhancement.Cost)
                    report.Add(section, $"Enhancement \"{enhancement.Name}\": Wahapedia {enhancement.Cost} pts, dataset {mine.Cost} pts.");
                // The dataset starts Upgrade descriptions with "Upgrade." for the app
                else if (mine.Description != null && TextDiff(Regex.Replace(mine.Description, @"^\s*\**upgrade\**[.:]?\s*", string.Empty, RegexOptions.IgnoreCase), enhancement.Text) is { } diff)
                    report.Add(section, $"Enhancement \"{enhancement.Name}\" text differs: {diff}");
            }
            foreach (var extra in detachment.Enhancements.Where(e => !theirs.Enhancements.Any(t => Norm(t.Name) == Norm(e.Name))))
                report.Add(section, $"Enhancement \"{extra.Name}\" is not on Wahapedia.");

            foreach (var stratagem in theirs.Stratagems)
            {
                var mine = detachment.Stratagems.FirstOrDefault(s => Norm(s.Name) == Norm(stratagem.Name));
                if (mine == null)
                {
                    report.Add(section, $"Stratagem \"{stratagem.Name}\" missing.");
                    continue;
                }
                if (mine.CPCost != stratagem.CP)
                    report.Add(section, $"Stratagem \"{mine.Name}\": Wahapedia {stratagem.CP}CP, dataset {mine.CPCost}CP.");
                foreach (var (part, local, wahapedia) in new[] { ("WHEN", mine.When, stratagem.When), ("TARGET", mine.Target ?? string.Empty, stratagem.Target), ("EFFECT", mine.Effect, stratagem.Effect), ("RESTRICTIONS", mine.Restrictions ?? string.Empty, stratagem.Restrictions) })
                    if (TextDiff(local, wahapedia) is { } diff)
                        report.Add(section, $"Stratagem \"{mine.Name}\" {part} differs: {diff}");
            }
            foreach (var extra in detachment.Stratagems.Where(s => !theirs.Stratagems.Any(t => Norm(t.Name) == Norm(s.Name))))
                report.Add(section, $"Stratagem \"{extra.Name}\" is not on Wahapedia.");
            report.Checked(section);
        }

        foreach (var missing in detachments.Where(d => !roster.Detachments.Any(l => Norm(l.Name) == Norm(d.Name))))
            report.Add("Detachments not in this roster", $"{missing.Name} ({missing.Cost} DP)");
    }

    // ---------------------------------------------------------------- normalisation

    public static string Norm(string? text)
    {
        if (text == null)
            return string.Empty;
        var result = text.Replace('’', '\'').Replace('‘', '\'').Replace('–', '-').Replace('—', '-').Replace('\u00a0', ' ').Replace("➤", string.Empty);
        result = result.Replace('”', '"').Replace('“', '"').Replace('″', '"');
        // Wahapedia sometimes spaces the inch mark: 6 ” becomes 6"
        result = Regex.Replace(result, @"(\d)\s+""", "$1\"");
        result = Regex.Replace(result, @"(\w)\(", "$1 (");
        return Regex.Replace(result, @"\s+", " ").Trim().ToLowerInvariant();
    }

    private static string NormStat(string? value)
    {
        var result = Norm(value).Replace("\"", string.Empty);
        return result is "n/a" ? "-" : result;
    }

    private static string RuleName(string ability) => Regex.Replace(Norm(ability), @"\s+(\d\+|d\d|\d+""?|\d+)$", string.Empty);

    /// <summary>Model names match ignoring case and a plural "s".</summary>
    private static bool SameModel(string? a, string? b, bool exact = false)
    {
        static string Single(string? s) => Regex.Replace(Norm(s), @"s$", string.Empty);
        return Single(a) == Single(b) || (!exact && (Single(a).StartsWith(Single(b) + " ") || Single(b).StartsWith(Single(a) + " ")));
    }

    /// <summary>"grenade launcher" on Wahapedia covers the dataset's "grenade launcher - frag" and "- krak" profiles.</summary>
    private static bool SameItem(string local, string wahapedia)
        => local == wahapedia || local.StartsWith(wahapedia + " - ") || Regex.Replace(local, @"s$", string.Empty) == Regex.Replace(wahapedia, @"s$", string.Empty);

    private static string? CompactBase(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        if (text.Contains("model", StringComparison.OrdinalIgnoreCase))
            return "Hull";
        var result = Regex.Replace(text, @"[()⌀]|mm|Oval|Base", string.Empty, RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\s*x\s*", "x");
        return result.Trim();
    }

    /// <summary>Word-level comparison; returns a short excerpt around the first difference, or null when equal.</summary>
    public static string? TextDiff(string? local, string? wahapedia)
    {
        static string[] Words(string? s) => Regex.Replace(Norm(s).Replace("*", string.Empty), @"[^\w+\-""']+", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w != "-").ToArray();
        var a = Words(local);
        var b = Words(wahapedia);
        if (a.SequenceEqual(b) || b.Length == 0)
            return null;
        var i = 0;
        while (i < a.Length && i < b.Length && a[i] == b[i])
            i++;
        var start = Math.Max(0, i - 5);
        string Excerpt(string[] words) => (start > 0 ? "… " : string.Empty) + string.Join(" ", words.Skip(start).Take(16)) + (start + 16 < words.Length ? " …" : string.Empty);
        return $"Wahapedia \"{Excerpt(b)}\" / dataset \"{Excerpt(a)}\"";
    }
}

public class Report
{
    private readonly List<(string Section, string Note)> _notes = new();
    private readonly HashSet<string> _checked = new();

    public void Add(string section, string note) => _notes.Add((section, note));
    public void Checked(string section) => _checked.Add(section);

    public int Count => _notes.Count;
    public int CheckedCount => _checked.Count;
    public int CleanCount => _checked.Count(c => _notes.All(n => n.Section != c));

    public string ToMarkdown(string title, IEnumerable<string> header)
    {
        var lines = new List<string> { $"# {title}", string.Empty };
        lines.AddRange(header);
        lines.Add(string.Empty);
        var clean = _checked.Where(c => _notes.All(n => n.Section != c)).Order().ToList();
        if (clean.Count > 0)
            lines.AddRange([$"Matched Wahapedia with no differences: {string.Join(", ", clean)}.", string.Empty]);
        foreach (var group in _notes.GroupBy(n => n.Section))
        {
            lines.AddRange([$"## {group.Key}", string.Empty]);
            lines.AddRange(group.Select(n => $"- {n.Note}"));
            lines.Add(string.Empty);
        }
        return string.Join("\n", lines);
    }
}
