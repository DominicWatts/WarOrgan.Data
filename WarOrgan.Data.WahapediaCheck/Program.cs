using System.Text.Json;
using WarOrgan.Data.WahapediaCheck;
using WarOrgan.Model.Faction;

// Compare WarOrgan rosters with Wahapedia: step 2 of the source order in FACTIONS.md.
var rosters = new List<string>();
var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
for (var i = 0; i < args.Length; i++)
{
    if (args[i] == "--refresh")
        options["refresh"] = "true";
    else if (args[i].StartsWith("--") && i + 1 < args.Length)
    {
        if (args[i] == "--roster")
            rosters.Add(args[++i]);
        else
            options[args[i][2..]] = args[++i];
    }
    else
    {
        rosters.Clear();
        break;
    }
}

if (rosters.Count == 0 || !options.ContainsKey("faction-slug"))
{
    Console.Error.WriteLine("""
        Usage: dotnet run --project WarOrgan.Data.WahapediaCheck -- --faction-slug <slug> --roster <roster.json> [--roster <another.json>] [options]

          --faction-slug <slug>   Wahapedia faction, e.g. genestealer-cults (its datasheet page also lists allied datasheets)
          --faction <keyword>     Faction keyword of the roster (default: the roster's Name)
          --datasheets <url|file> Override the datasheets page
          --faction-page <url|file> Override the faction page (detachments)
          --out <folder>          Report folder (default: WahapediaCheck-Output)
          --cache <folder>        Page cache (default: system temp); pages are reused for the day
          --refresh               Download the pages again
        """);
    return 1;
}

var slug = options["faction-slug"];
var cache = new PageCache(options.GetValueOrDefault("cache") ?? Path.Combine(Path.GetTempPath(), "warorgan-wahapedia"), options.ContainsKey("refresh"));
var datasheetSource = options.GetValueOrDefault("datasheets") ?? $"https://wahapedia.ru/wh40k11ed/factions/{slug}/datasheets.html";
var factionSource = options.GetValueOrDefault("faction-page") ?? $"https://wahapedia.ru/wh40k11ed/factions/{slug}/";

var sheets = WahapediaPages.ParseDatasheets(await cache.GetAsync(datasheetSource));
var (detachments, _) = WahapediaPages.ParseFactionPage(await cache.GetAsync(factionSource));
var outFolder = Directory.CreateDirectory(options.GetValueOrDefault("out") ?? "WahapediaCheck-Output").FullName;

foreach (var path in rosters)
{
    var roster = JsonSerializer.Deserialize<FactionRoster>(File.ReadAllText(path))
        ?? throw new InvalidDataException($"Could not read roster {path}");
    var fileName = Path.GetFileNameWithoutExtension(path);
    var legends = fileName.EndsWith(" Legends", StringComparison.OrdinalIgnoreCase);
    var factionKeyword = options.GetValueOrDefault("faction") ?? roster.Name;

    var report = new Report();
    var comparer = new RosterComparer(report);
    comparer.CompareUnits(roster, sheets, factionKeyword, legends);
    if (roster.Detachments.Count > 0)
        comparer.CompareDetachments(roster, detachments);

    var header = new[]
    {
        $"- Roster: `{path}`",
        $"- Wahapedia: {datasheetSource} and {factionSource}, fetched {cache.FetchedOn(datasheetSource):yyyy-MM-dd}",
        $"- {report.CheckedCount} datasheets and detachments checked, {report.CleanCount} with no differences, {report.Count} notes.",
        "- Wahapedia is the source of truth for names, stats, weapons, abilities, composition, points, detachments, enhancements and stratagems. Some notes are expected, such as deliberate dataset conventions.",
    };
    var reportPath = Path.Combine(outFolder, $"{fileName} wahapedia-check.md");
    File.WriteAllText(reportPath, report.ToMarkdown($"{fileName}: Wahapedia check", header));
    Console.WriteLine($"{fileName}: {report.CheckedCount} checked, {report.CleanCount} clean, {report.Count} notes -> {reportPath}");
}
return 0;

/// <summary>Downloads each Wahapedia page once a day; local files are read directly.</summary>
class PageCache(string folder, bool refresh)
{
    private static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; WarOrgan-dataset-check)");
        return client;
    }

    private string PathFor(string url) => Path.Combine(folder, $"{DateTime.UtcNow:yyyy-MM-dd}-{string.Concat(url.Split(Path.GetInvalidFileNameChars()))}.html");

    public DateTime FetchedOn(string source)
        => File.Exists(source) ? File.GetLastWriteTimeUtc(source) : File.GetLastWriteTimeUtc(PathFor(source));

    public async Task<string> GetAsync(string source)
    {
        if (File.Exists(source))
            return await File.ReadAllTextAsync(source);
        Directory.CreateDirectory(folder);
        var path = PathFor(source);
        if (refresh || !File.Exists(path))
            await File.WriteAllTextAsync(path, await Http.GetStringAsync(source));
        return await File.ReadAllTextAsync(path);
    }
}
