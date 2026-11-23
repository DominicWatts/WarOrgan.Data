using System.IO.Compression;
using System.Text.Json.Nodes;

namespace WarOrgan.Data.BsdataImport;

/// <summary>
/// Every catalogue in a BSData JSON dump, indexed by entry ID so links resolve across files.
/// </summary>
public class BsData
{
    public const string PointsTypeId = "51b2-306e-1021-d207";
    public const string DetachmentPointsTypeId = "82ae-1066-5107-6ae0";

    private readonly Dictionary<string, JsonObject> _byId = new();
    private readonly Dictionary<string, string> _fileById = new();

    public Dictionary<string, JsonObject> Catalogues { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static BsData Load(string path)
    {
        var data = new BsData();
        if (File.Exists(path))
        {
            using var zip = ZipFile.OpenRead(path);
            foreach (var entry in zip.Entries.Where(e => e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
            {
                using var stream = entry.Open();
                data.Add(Path.GetFileNameWithoutExtension(entry.Name), JsonNode.Parse(stream)!.AsObject());
            }
        }
        else if (Directory.Exists(path))
        {
            foreach (var file in Directory.EnumerateFiles(path, "*.json"))
                data.Add(Path.GetFileNameWithoutExtension(file), JsonNode.Parse(File.ReadAllText(file))!.AsObject());
        }
        else
            throw new FileNotFoundException($"BSData source not found: {path}");

        return data;
    }

    private void Add(string fileName, JsonObject root)
    {
        var body = root["catalogue"] as JsonObject ?? root["gameSystem"] as JsonObject ?? root;
        Catalogues[fileName] = body;
        Index(body, fileName);
    }

    private void Index(JsonNode? node, string fileName)
    {
        switch (node)
        {
            case JsonObject obj:
                if (obj["id"] is JsonValue idValue && idValue.TryGetValue<string>(out var id))
                {
                    _byId.TryAdd(id, obj);
                    _fileById.TryAdd(id, fileName);
                }
                foreach (var (_, child) in obj)
                    Index(child, fileName);
                break;
            case JsonArray array:
                foreach (var child in array)
                    Index(child, fileName);
                break;
        }
    }

    public JsonObject? Get(string? id) => id != null && _byId.TryGetValue(id, out var obj) ? obj : null;
    public string? FileOf(string? id) => id != null && _fileById.TryGetValue(id, out var file) ? file : null;

    /// <summary>Returns the link target for an entry link or info link, otherwise the object itself.</summary>
    public JsonObject? Resolve(JsonObject obj) => obj.Str("targetId") is { } target ? Get(target) : obj;
}

public static class JsonExtensions
{
    public static string? Str(this JsonObject obj, string key)
        => obj[key] is JsonValue value ? value.ToString() : null;

    public static bool Bool(this JsonObject obj, string key)
        => obj[key] is JsonValue value && value.TryGetValue<bool>(out var b) && b;

    public static double? Num(this JsonObject obj, string key)
        => obj[key] is JsonValue value && value.TryGetValue<double>(out var d) ? d : null;

    public static IEnumerable<JsonObject> Arr(this JsonObject obj, string key)
        => obj[key] is JsonArray array ? array.OfType<JsonObject>() : [];

    public static string Name(this JsonObject obj) => obj.Str("name") ?? string.Empty;

    /// <summary>Selection constraints of the given type at parent scope, from the link and its target.</summary>
    public static int? Constraint(this JsonObject obj, string type, string scope = "parent")
    {
        var match = obj.Arr("constraints")
            .FirstOrDefault(c => c.Str("field") == "selections" && c.Str("type") == type && c.Str("scope") == scope);
        return match?.Num("value") is { } value ? (int)value : null;
    }

    public static double? Cost(this JsonObject obj, string typeId)
        => obj.Arr("costs").FirstOrDefault(c => c.Str("typeId") == typeId)?.Num("value");

    public static IEnumerable<string> Characteristics(this JsonObject profile, string name)
        => profile.Arr("characteristics").Where(c => c.Name() == name).Select(c => c.Str("$text") ?? string.Empty);

    public static string Characteristic(this JsonObject profile, string name)
        => profile.Characteristics(name).FirstOrDefault()?.Trim() ?? string.Empty;
}
