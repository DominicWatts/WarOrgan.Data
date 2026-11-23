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
            .Select(k => k.Replace('\u00a0', ' '))
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase);
        return string.Join(", ", parts);
    }

    public static string TitleCase(string text) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());
}

/// <summary>Collects the manual follow-ups each converted item needs.</summary>
public class ImportReport
{
    private readonly List<(string Section, string Note)> _notes = new();

    public void Add(string section, string note) => _notes.Add((section, note));

    public int Count => _notes.Count;

    public string ToMarkdown(string title, IEnumerable<string> header)
    {
        var lines = new List<string> { $"# {title}", string.Empty };
        lines.AddRange(header);
        lines.Add(string.Empty);
        foreach (var group in _notes.GroupBy(n => n.Section))
        {
            lines.Add($"## {group.Key}");
            lines.Add(string.Empty);
            lines.AddRange(group.Select(n => $"- {n.Note}"));
            lines.Add(string.Empty);
        }
        return string.Join("\n", lines);
    }
}
