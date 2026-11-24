using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using WarOrgan.Data.BsdataImport;
using WarOrgan.Model.Faction;
using WarOrgan.Model.Unit;

// Draft a WarOrgan roster from a BSData dump. The output is a starting point: check it against Wahapedia,
// work through the import report, then copy it onto the faction branch.
var options = ParseArgs(args);
if (options == null)
{
    Console.Error.WriteLine("""
        Usage: dotnet run --project WarOrgan.Data.BsdataImport -- --source <zip or folder> --catalogue "<catalogue file name>" [options]

          --faction <keyword>         Faction keyword (default: catalogue name after the last " - ")
          --definition <path>         DataSetDefinition.json (default: WarOrgan.Data.BuildDataset/Assets/WH40K11e/DataSetDefinition.json)
          --out <folder>              Output folder (default: BsdataImport-Output)
          --units "A;B"               Only these units
          --exclude-keywords "A;B"    Skip units with any of these keywords
          --skip-legends              Skip [Legends] units
          --no-detachments            Units only (for allied rosters)
        """);
    return 1;
}

var data = BsData.Load(options["source"]);
if (!data.Catalogues.TryGetValue(options["catalogue"], out var catalogue))
{
    Console.Error.WriteLine($"Catalogue \"{options["catalogue"]}\" not found. Available: {string.Join(", ", data.Catalogues.Keys.Order())}");
    return 1;
}

var faction = options.GetValueOrDefault("faction") ?? Regex.Replace(options["catalogue"], @"^.* - ", string.Empty);
var definition = JsonNode.Parse(File.ReadAllText(options.GetValueOrDefault("definition") ?? "WarOrgan.Data.BuildDataset/Assets/WH40K11e/DataSetDefinition.json"))!.AsObject();
var settings = new ImportSettings(
    FactionKeywords: definition.Arr("Factions").Select(f => f.Str("FactionKeyword")!).Append(faction).ToHashSet(StringComparer.OrdinalIgnoreCase),
    CoreAbilityPatterns: definition.Arr("CoreAbilities").Select(a => new Regex(a.Str("Regex")!, RegexOptions.IgnoreCase)).ToList(),
    WeaponAbilityPatterns: definition.Arr("WeaponAbilities").Select(a => new Regex(a.Str("Regex")!, RegexOptions.IgnoreCase)).ToList(),
    // Faction rules can live in a parent catalogue (Combat Doctrines for chapters), so collect every catalogue's
    ArmyRuleIds: data.Catalogues.Where(c => !c.Key.Equals("Warhammer 40,000", StringComparison.OrdinalIgnoreCase))
        .SelectMany(c => c.Value.Arr("sharedRules")).Select(r => r.Str("id")!).ToHashSet());

var report = new ImportReport();
var entries = UnitEntries(catalogue, faction)
    .Where(e => !e.Name().Contains("[Crucible]"))
    .Where(e => !(options.ContainsKey("skip-legends") && e.Name().Contains("[Legends]")))
    .Where(e => !options.TryGetValue("units", out var units) || Split(units).Contains(UnitImporter.UnitName(e), StringComparer.OrdinalIgnoreCase))
    .Where(e => !options.TryGetValue("exclude-keywords", out var excluded)
        || !e.Arr("categoryLinks").Any(c => Split(excluded).Contains(c.Name(), StringComparer.OrdinalIgnoreCase)))
    .GroupBy(UnitImporter.UnitName).Select(g => g.First())
    .OrderBy(e => e.Name(), StringComparer.OrdinalIgnoreCase)
    .ToList();

var knownIds = entries.Select(e => UnitImporter.UnitName(e).ToUpperInvariant()).ToHashSet();
var importer = new UnitImporter(data, settings, report);
var roster = new FactionRoster { Name = faction };
// Legends rosters keep the faction name; only the file name differs
var legends = new FactionRoster { Name = faction };
foreach (var entry in entries)
{
    var unit = importer.Convert(entry, knownIds);
    (entry.Name().Contains("[Legends]") ? legends : roster).Units.Add(unit);
}

if (!options.ContainsKey("no-detachments"))
{
    roster.Detachments = new DetachmentImporter(data, report).Convert([catalogue]);
    roster.ArmyRules = catalogue.Arr("sharedRules")
        .Select(r => new UnitInfoSection { Title = r.Name(), Text = TextUtils.CleanRuleText(r.Str("description")) }).ToList();
}

var outFolder = Directory.CreateDirectory(options.GetValueOrDefault("out") ?? "BsdataImport-Output").FullName;
var json = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
};
File.WriteAllText(Path.Combine(outFolder, $"{roster.Name}.json"), JsonSerializer.Serialize(roster, json));
if (legends.Units.Count > 0)
    File.WriteAllText(Path.Combine(outFolder, $"{faction} Legends.json"), JsonSerializer.Serialize(legends, json));

var header = new List<string>
{
    $"- Source: `{options["source"]}`{SourceHash(options["source"])}",
    $"- Catalogue: `{options["catalogue"]}`, faction keyword `{faction}`",
    $"- Converted {DateTime.UtcNow:yyyy-MM-dd}: {roster.Units.Count} units, {legends.Units.Count} Legends units, {roster.Detachments.Count} detachments",
    "- Every value still needs checking against Wahapedia. The notes below are the known gaps.",
};
File.WriteAllText(Path.Combine(outFolder, $"{faction} import-report.md"), report.ToMarkdown($"{faction} BSData import", header));

Console.WriteLine($"Wrote {roster.Units.Count} units, {legends.Units.Count} Legends units and {roster.Detachments.Count} detachments to {outFolder} ({report.Count} report notes).");
return 0;

IEnumerable<JsonObject> UnitEntries(JsonObject source, string factionKeyword)
{
    // Libraries linked with importRootEntries (Daemons, Knights, Heresy Legends) contribute their root units too
    var libraries = source.Arr("catalogueLinks").Where(l => l["importRootEntries"]?.GetValue<bool>() == true)
        .Select(l => data.Catalogues.Values.FirstOrDefault(c => c.Str("id") == l.Str("targetId"))).OfType<JsonObject>();
    var candidates = source.Arr("sharedSelectionEntries")
        .Concat(source.Arr("entryLinks").Select(l => data.Get(l.Str("targetId"))).OfType<JsonObject>())
        .Concat(libraries.SelectMany(l => l.Arr("sharedSelectionEntries")));
    return candidates.Where(e => e.Str("type") is "unit" or "model")
        .Where(e => e.Arr("categoryLinks").Any(c => c.Name().Equals($"Faction: {factionKeyword}", StringComparison.OrdinalIgnoreCase)));
}

static string[] Split(string value) => value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

static string SourceHash(string path)
{
    if (!File.Exists(path))
        return string.Empty;
    using var stream = File.OpenRead(path);
    return $", SHA-256 `{Convert.ToHexStringLower(SHA256.HashData(stream))}`";
}

static Dictionary<string, string>? ParseArgs(string[] args)
{
    var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var i = 0; i < args.Length; i++)
    {
        if (!args[i].StartsWith("--"))
            return null;
        var key = args[i][2..];
        if (key is "skip-legends" or "no-detachments")
            result[key] = "true";
        else if (i + 1 < args.Length)
            result[key] = args[++i];
        else
            return null;
    }
    return result.ContainsKey("source") && result.ContainsKey("catalogue") ? result : null;
}

public record ImportSettings(HashSet<string> FactionKeywords, List<Regex> CoreAbilityPatterns, List<Regex> WeaponAbilityPatterns, HashSet<string> ArmyRuleIds)
{
    public bool IsCoreAbility(string name) => CoreAbilityPatterns.Any(p => p.IsMatch(name));
    public bool IsWeaponAbility(string name) => WeaponAbilityPatterns.Any(p => p.IsMatch(name));
}
