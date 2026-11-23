using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace WarOrgan.Data.WahapediaCheck;

public record WProfile(string? ModelName, Dictionary<string, string> Stats, string? Base);
public record WWeapon(string Name, bool Melee, Dictionary<string, string> Stats, List<string> Keywords);
public record WModelRange(int Min, int Max, string Name);
public record WPoints(int FirstCopy, int? LastCopy, int Models, int Cost);

public class WDatasheet
{
    public string Name { get; set; } = string.Empty;
    public bool Legends { get; set; }
    public List<WProfile> Profiles { get; } = new();
    public string? Invulnerable { get; set; }
    public List<WWeapon> Weapons { get; } = new();
    public List<string> CoreAbilities { get; } = new();
    public Dictionary<string, string> Abilities { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<WModelRange> Composition { get; } = new();
    public List<string> SizeChoices { get; } = new();
    public HashSet<string> MixedLoadouts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, List<string>> EquippedWith { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<WPoints> Points { get; } = new();
    public List<string>? LeaderOf { get; set; }
    public List<string> Keywords { get; } = new();
    public List<string> FactionKeywords { get; } = new();
}

public record WEnhancement(string Name, int Cost, string Text);
public record WStratagem(string Name, int CP, string Type, string When, string Target, string Effect, string Restrictions);

public class WDetachment
{
    public string Name { get; set; } = string.Empty;
    public int Cost { get; set; }
    public List<string> RuleTitles { get; } = new();
    public List<WEnhancement> Enhancements { get; } = new();
    public List<WStratagem> Stratagems { get; } = new();
}

/// <summary>Reads Wahapedia 11th Edition datasheet and faction pages.</summary>
public static class WahapediaPages
{
    // Datasheet headings that hold structure rather than an ability
    private static readonly HashSet<string> StructuralHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "ABILITIES", "WARGEAR ABILITIES", "UNIT COMPOSITION", "STRATAGEMS", "DETACHMENT ABILITY", "MELEE WEAPONS", "RANGED WEAPONS",
        "WARGEAR OPTIONS", "ENHANCEMENTS", "LEADER", "LED BY", "SUPPORT", "SUPPORTED BY",
    };

    public static List<WDatasheet> ParseDatasheets(string html)
    {
        var document = new HtmlParser().ParseDocument(html);
        return document.QuerySelectorAll("div.dsOuterFrame.datasheet").Select(ParseDatasheet).ToList();
    }

    private static WDatasheet ParseDatasheet(IElement frame)
    {
        var sheet = new WDatasheet
        {
            Name = Text(frame.QuerySelector(".dsH2Header > div")),
            Legends = frame.ClassList.Contains("sLegendary"),
        };

        // Statlines: names come from the first profile's headers
        var statNames = frame.QuerySelectorAll(".dsProfileBaseWrap").FirstOrDefault()?.QuerySelectorAll(".dsCharName").Select(Text).ToList() ?? [];
        foreach (var wrap in frame.QuerySelectorAll(".dsProfileBaseWrap"))
        {
            var values = wrap.QuerySelectorAll(".dsCharValue").Select(Text).ToList();
            var stats = statNames.Zip(values).ToDictionary(p => p.First.ToUpperInvariant(), p => p.Second);
            var model = wrap.QuerySelector(".dsModelName");
            var baseSize = wrap.QuerySelector(".dsModelBase, .dsModelBase2");
            sheet.Profiles.Add(new WProfile(model == null ? null : Text(model), stats, baseSize == null ? null : Text(baseSize)));
        }
        if (frame.QuerySelector(".dsCharInvulValue") is { } invulnerable)
            sheet.Invulnerable = Text(invulnerable);

        foreach (var row in frame.QuerySelectorAll("tr.dsWeaponRow"))
        {
            var nameCell = row.QuerySelector("td.wTable2_short");
            var name = Text(nameCell?.QuerySelector(".dsWeaponName"));
            var keywords = nameCell?.QuerySelectorAll(".kwbw").Select(Text).Where(k => k.Length > 0).ToList() ?? [];
            var values = row.QuerySelectorAll("div.ct.pad2626").Select(Text).ToList();
            if (values.Count < 6 || name.Length == 0)
                continue;
            var melee = values[0].Equals("Melee", StringComparison.OrdinalIgnoreCase);
            var stats = new Dictionary<string, string>
            {
                ["Range"] = values[0], ["A"] = values[1], [melee ? "WS" : "BS"] = values[2], ["S"] = values[3], ["AP"] = values[4], ["D"] = values[5],
            };
            sheet.Weapons.Add(new WWeapon(name, melee, stats, keywords));
        }

        foreach (var row in frame.QuerySelectorAll("table.dsCoreArmy tr"))
        {
            var label = Text(row.QuerySelector(".dsCoreArmyLabel"));
            var value = row.QuerySelector(".dsCoreArmyValue");
            if (label.StartsWith("CORE", StringComparison.OrdinalIgnoreCase) && value != null)
                sheet.CoreAbilities.AddRange(Text(value).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        foreach (var (header, content) in Sections(frame))
        {
            switch (header)
            {
                case "ABILITIES":
                case "WARGEAR ABILITIES":
                    foreach (var ability in content.SelectMany(c => c.Matches(".dsAbility") ? new[] { c } : c.QuerySelectorAll(".dsAbility").ToArray()))
                        AddNamedAbility(sheet, ability);
                    break;
                case "UNIT COMPOSITION":
                    ParseComposition(sheet, content);
                    break;
                case "LEADER":
                case "SUPPORT":
                    sheet.LeaderOf = content.SelectMany(c => c.QuerySelectorAll("li")).Select(Text).Where(t => t.Length > 0).ToList();
                    break;
                default:
                    // Any other heading is an ability shown as its own section, such as SUPREME COMMANDER,
                    // HUNTER ORGANISM, TRANSPORT or DAMAGED: 1-4 WOUNDS REMAINING
                    if (!StructuralHeaders.Contains(header))
                        sheet.Abilities[TitleCase(header)] = StripNotes(string.Join(" ", content.Select(Text)));
                    break;
            }
        }

        var unitKeywords = frame.QuerySelectorAll("div").FirstOrDefault(d => d.ClassName?.StartsWith("dsLeft") == true && d.ClassName.Contains("olKW"));
        if (unitKeywords != null)
        {
            // "KEYWORDS - ALL MODELS: INFANTRY, ... | RAVENER PRIME: CHARACTER" lists per-model keywords in segments
            var text = Regex.Replace(Lines(unitKeywords), @"^KEYWORDS\s*([-–]\s*ALL MODELS)?\s*:", string.Empty, RegexOptions.IgnoreCase);
            foreach (var segment in text.Split(['\n', ';']))
                sheet.Keywords.AddRange(Regex.Replace(segment, @"^[^,:]+:", string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }
        var factionKeywords = frame.QuerySelectorAll("div").FirstOrDefault(d => d.ClassName?.StartsWith("dsRight") == true && d.ClassName.Contains("olKW"));
        if (factionKeywords != null)
            sheet.FactionKeywords.AddRange(SplitKeywords(Text(factionKeywords), "FACTION KEYWORDS:"));
        return sheet;
    }

    /// <summary>Datasheet sections: each right-hand dsHeader and the sibling elements up to the next header.</summary>
    private static IEnumerable<(string Header, List<IElement> Content)> Sections(IElement frame)
    {
        foreach (var header in frame.QuerySelectorAll("div.dsHeader").Where(h => h.ParentElement?.LocalName != "td"))
        {
            var content = new List<IElement>();
            for (var sibling = header.NextElementSibling; sibling != null && !sibling.ClassList.Contains("dsHeader"); sibling = sibling.NextElementSibling)
                content.Add(sibling);
            yield return (Text(header).ToUpperInvariant(), content);
        }
    }

    private static void AddNamedAbility(WDatasheet sheet, IElement ability)
    {
        var bold = ability.QuerySelector("b");
        var text = Text(ability);
        if (bold == null || !Text(bold).EndsWith(':'))
            return;
        var name = Text(bold).TrimEnd(':').Trim();
        sheet.Abilities[name] = StripNotes(text[(text.IndexOf(':') + 1)..].Trim());
    }

    private static void ParseComposition(WDatasheet sheet, List<IElement> content)
    {
        foreach (var block in content)
        {
            foreach (var item in block.QuerySelectorAll("ul.dsUl > li").Select(Text))
            {
                var match = Regex.Match(item, @"^(\d+)(?:-(\d+))?\s+(.+?)\**$");
                if (item.Contains(" and ") || item.Contains(" or "))
                    sheet.SizeChoices.Add(item);
                else if (match.Success)
                    sheet.Composition.Add(new WModelRange(int.Parse(match.Groups[1].Value),
                        int.Parse(match.Groups[2].Success ? match.Groups[2].Value : match.Groups[1].Value), match.Groups[3].Value.Trim()));
            }

            foreach (var line in Lines(block).Split('\n'))
            {
                var match = Regex.Match(line, @"^(?:Every|The|This|\d+)\s+(.*?)\s+(?:is|are) equipped with:\s*(.+?)\.?$");
                if (!match.Success)
                    continue;
                var who = Regex.Replace(match.Groups[1].Value.Trim(), @"s$", string.Empty);
                var items = match.Groups[2].Value.Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                // Several lines for one model type (e.g. "3 Raveners ..." and "1 Ravener ...") are combined
                if (sheet.EquippedWith.TryGetValue(who, out var existing))
                {
                    existing.AddRange(items.Except(existing));
                    sheet.MixedLoadouts.Add(who);
                }
                else
                    sheet.EquippedWith[who] = items.ToList();
            }

            (int First, int? Last) copies = (1, null);
            foreach (var row in block.QuerySelectorAll("tr"))
            {
                var header = row.QuerySelector(".dsUnitCostHeader");
                if (header != null)
                {
                    copies = CopyRange(Text(header));
                    continue;
                }
                var cells = row.QuerySelectorAll("td").Select(Text).ToList();
                var models = cells.Count > 0 ? Regex.Match(cells[0], @"^(\d+) models?") : Match.Empty;
                var cost = row.QuerySelector(".PriceTag");
                if (models.Success && cost != null && int.TryParse(Text(cost), out var points))
                    sheet.Points.Add(new WPoints(copies.First, copies.Last, int.Parse(models.Groups[1].Value), points));
            }
        }
    }

    /// <summary>"YOUR 1ST TO 2ND UNITS COST" is copies 1-2, "YOUR 3RD+ UNIT COSTS" is copies 3 and up, "YOUR UNIT COSTS" is every copy.</summary>
    private static (int, int?) CopyRange(string header)
    {
        var numbers = Regex.Matches(header, @"(\d+)(?:ST|ND|RD|TH)", RegexOptions.IgnoreCase).Select(m => int.Parse(m.Groups[1].Value)).ToList();
        return numbers.Count switch
        {
            0 => (1, null),
            1 => header.Contains('+') ? (numbers[0], null) : (numbers[0], numbers[0]),
            _ => (numbers[0], numbers[1]),
        };
    }

    public static (List<WDetachment> Detachments, List<string> ArmyRules) ParseFactionPage(string html)
    {
        var document = new HtmlParser().ParseDocument(html);
        var detachments = new List<WDetachment>();
        var armyRules = new List<string>();
        WDetachment? current = null;
        var inArmyRules = false;

        foreach (var element in document.QuerySelectorAll("h2, h3, ul.EnhancementsPts > li, div.str11Wrap"))
        {
            if (element.LocalName == "h2")
            {
                var heading = Text(element);
                var dp = Regex.Match(heading, @"^(.*?)\s*(\d+)\s*DP$");
                inArmyRules = heading.Equals("Army Rules", StringComparison.OrdinalIgnoreCase);
                if (dp.Success)
                    detachments.Add(current = new WDetachment { Name = dp.Groups[1].Value.Trim(), Cost = int.Parse(dp.Groups[2].Value) });
                else if (heading is "Crusade Rules" or "Boarding Actions")
                    current = null;
                continue;
            }

            if (element.LocalName == "h3")
            {
                if (inArmyRules)
                    armyRules.Add(Text(element));
                else if (current != null && current.Enhancements.Count == 0 && current.Stratagems.Count == 0
                    && !Text(element).Contains("Errata", StringComparison.OrdinalIgnoreCase))
                    current.RuleTitles.Add(Text(element));
                continue;
            }

            if (current == null)
                continue;

            if (element.LocalName == "li")
            {
                var spans = element.Children.Where(c => c.LocalName == "span").Select(Text).ToList();
                var cost = Regex.Match(spans.LastOrDefault() ?? string.Empty, @"(\d+)\s*pts");
                var name = Regex.Replace(spans.FirstOrDefault() ?? string.Empty, @"\s*UPGRADE$", string.Empty);
                var cell = (IElement?)element.Closest("td")?.Clone();
                // Leave out the name line, flavour text and collapsed errata boxes
                foreach (var skip in cell?.QuerySelectorAll("ul.EnhancementsPts, .ShowFluff, [class*='Errata']").ToList() ?? [])
                    skip.Remove();
                current.Enhancements.Add(new WEnhancement(name, cost.Success ? int.Parse(cost.Groups[1].Value) : 0, StripNotes(Text(cell))));
            }
            else
            {
                var text = Text(element.QuerySelector(".str11Text"));
                current.Stratagems.Add(new WStratagem(
                    Text(element.QuerySelector(".str11Name")),
                    int.TryParse(Regex.Match(Text(element.QuerySelector(".str11CP")), @"\d+").Value, out var cp) ? cp : 0,
                    Text(element.QuerySelector(".str11Type")),
                    Between(text, "WHEN:", "TARGET:"), Between(text, "TARGET:", "EFFECT:"), Between(text, "EFFECT:", "RESTRICTIONS:"), Between(text, "RESTRICTIONS:", null)));
            }
        }
        return (detachments, armyRules);
    }

    /// <summary>Designer's notes explain tokens and are not part of the rule.</summary>
    public static string StripNotes(string text) => Regex.Replace(text, @"\s*Designer.s Note:.*$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline).Trim();

    private static string Between(string text, string start, string? end)
    {
        var from = text.IndexOf(start, StringComparison.Ordinal);
        if (from < 0)
            return string.Empty;
        from += start.Length;
        var to = end == null ? -1 : text.IndexOf(end, from, StringComparison.Ordinal);
        return (to < 0 ? text[from..] : text[from..to]).Trim();
    }

    private static IEnumerable<string> SplitKeywords(string text, string label)
    {
        var start = text.IndexOf(label, StringComparison.OrdinalIgnoreCase);
        var body = start >= 0 ? text[(start + label.Length)..] : text;
        return body.Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Where(k => k.Length > 0);
    }

    private static string TitleCase(string text) => System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());

    /// <summary>Visible text with tooltip templates removed and whitespace collapsed.</summary>
    public static string Text(IElement? element)
    {
        if (element == null)
            return string.Empty;
        return Regex.Replace(Lines(element).Replace('\n', ' '), @"\s+", " ").Trim();
    }

    /// <summary>Visible text keeping line breaks at br, li, p and div boundaries.</summary>
    public static string Lines(IElement? element)
    {
        if (element == null)
            return string.Empty;
        var clone = (IElement)element.Clone();
        foreach (var hidden in clone.QuerySelectorAll(".tooltip_templates, script, style"))
            hidden.Remove();
        var owner = clone.Owner!;
        foreach (var br in clone.QuerySelectorAll("br").ToList())
            br.Replace(owner.CreateTextNode("\n"));
        foreach (var block in clone.QuerySelectorAll("li, p, div, ul, tr").ToList())
            block.AppendChild(owner.CreateTextNode("\n"));
        var text = clone.TextContent.Replace('\u00a0', ' ');
        text = Regex.Replace(text, @"[ \t\r]+", " ");
        return Regex.Replace(text, @"\s*\n\s*", "\n").Trim();
    }
}
