using System.Globalization;
using System.Text.RegularExpressions;

namespace WarOrgan.Data.BsdataImport;

public static class TextUtils
{
    /// <summary>Converts BSData markup to the dataset's style: ^^keywords^^ become upper case, non-breaking spaces become spaces.</summary>
    public static string CleanRuleText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var result = text.Replace('\u00a0', ' ').Replace("\r\n", "\n");
        result = Regex.Replace(result, @"\*\*\^\^(.+?)\^\^\*\*", m => m.Groups[1].Value.ToUpperInvariant());
        result = Regex.Replace(result, @"\^\^(.+?)\^\^", m => m.Groups[1].Value.ToUpperInvariant());
        result = Regex.Replace(result, @"^■\s*", "- ", RegexOptions.Multiline);
        result = Regex.Replace(result, @"[ \t]+\n", "\n");
        return result.Trim();
    }

    /// <summary>Weapon and wargear display names without BSData's "➤" profile markers.</summary>
    public static string CleanName(string name) => Regex.Replace(name, @"^[\s➤►•-]+", string.Empty).Trim();

    public static string ToId(string name) => CleanName(name).Replace('’', '\'').ToLowerInvariant();

    /// <summary>Compact base sizes as the app panel expects: "32", "60x35.5", "25, 32" or "Hull".</summary>
    public static string CompactBase(string value)
    {
        var text = value.Trim();
        if (text.Length == 0 || text.Contains("model", StringComparison.OrdinalIgnoreCase) || text.Equals("Hull", StringComparison.OrdinalIgnoreCase))
            return "Hull";
        text = Regex.Replace(text, @"\s*mm\b", string.Empty, RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\s*(Oval|Round|Flying)?\s*Base\b", string.Empty, RegexOptions.IgnoreCase);
        return Regex.Replace(text, @"\s*,\s*", ", ").Trim();
    }

    public static string SortKeywords(string keywords)
    {
        var parts = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(k => k != "-")
            .Select(NormaliseKeyword)
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase);
        return string.Join(", ", parts);
    }

    private static readonly string[] CanonicalKeywords =
    [
        "Assault", "Blast", "Close-quarters", "Cleave", "Devastating Wounds", "Extra Attacks", "Hazardous", "Heavy",
        "Ignores Cover", "Indirect Fire", "Lance", "Lethal Hits", "Melta", "One Shot", "Pistol", "Precision", "Psychic",
        "Rapid Fire", "Sustained Hits", "Torrent", "Twin-linked",
    ];

    /// <summary>
    /// BSData mixes capitals ("BLAST 2", "Anti-INFANTRY 4+", "Twin Linked"); write the core rule spellings, with
    /// targets and conditions in title case ("Anti-Monster/Vehicle 4+", "Lethal Hits: non-Monster/Vehicle").
    /// </summary>
    public static string NormaliseKeyword(string keyword)
    {
        keyword = Regex.Replace(keyword.Replace('\u00a0', ' ').Replace('\u2011', '-'), @"\s+", " ").Trim();
        var colon = keyword.IndexOf(':');
        var head = colon < 0 ? keyword : keyword[..colon].Trim();
        var condition = colon < 0 ? null : CaseWords(keyword[(colon + 1)..].Trim());

        head = Regex.Replace(head, @"^twin[ -]linked\b", "Twin-linked", RegexOptions.IgnoreCase);
        var anti = Regex.Match(head, @"^anti-(.+?) (\S+)$", RegexOptions.IgnoreCase);
        if (anti.Success)
            head = $"Anti-{CaseWords(anti.Groups[1].Value)} {anti.Groups[2].Value.ToUpperInvariant()}";
        else if (CanonicalKeywords.OrderByDescending(k => k.Length)
                 .FirstOrDefault(k => Regex.IsMatch(head, $@"^{Regex.Escape(k)}(?: |$)", RegexOptions.IgnoreCase)) is { } name)
            head = name + head[name.Length..].ToUpperInvariant();
        return condition == null ? head : $"{head}: {condition}";
    }

    // "non-MONSTER/VEHICLE" -> "non-Monster/Vehicle"
    private static string CaseWords(string text) =>
        Regex.Replace(text, @"[A-Za-z']+", m => m.Value.Equals("non", StringComparison.OrdinalIgnoreCase) ? "non" : TitleCase(m.Value));

    public static string TitleCase(string text) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());
}

/// <summary>Collects the manual follow-ups each converted item needs.</summary>
public class ImportReport
{
    private readonly List<(string Section, string Note)> _notes = new();

    public void Add(string section, string note) => _notes.Add((section, note));

    public int Count => _notes.Distinct().Count();

    public string ToMarkdown(string title, IEnumerable<string> header)
    {
        var lines = new List<string> { $"# {title}", string.Empty };
        lines.AddRange(header);
        lines.Add(string.Empty);
        var notes = _notes.Distinct().ToList();
        lines.Add("## Manual follow-up summary");
        lines.Add(string.Empty);
        lines.Add(notes.Count == 0
            ? "- No known conversion gaps. Source verification is still required."
            : $"- {notes.Count} notes across {notes.Select(n => n.Section).Distinct().Count()} units or sections need review.");
        lines.Add(string.Empty);
        foreach (var group in notes.GroupBy(n => n.Section))
        {
            lines.Add($"## {group.Key}");
            lines.Add(string.Empty);
            lines.AddRange(group.Select(n => $"- {n.Note}"));
            lines.Add(string.Empty);
        }
        return string.Join("\n", lines);
    }
}
