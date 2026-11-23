using WarOrgan.Data.BuildDataset.Utils;
using WarOrgan.Model.DataSet;
using WarOrgan.Model.Unit;

namespace WarOrgan.Data.BuildDataset.Validation;

public static class KeywordValidation
{
    public static void ValidateCategoryKeywords(this DataSetDefinition definition, IEnumerable<Unit> units)
    {
        var unitList = units.ToList();
        var keywords = unitList
            .SelectMany(unit => unit.Keywords.Concat(
                unit.UnitComposition.ModelCompositions.SelectMany(model => model.Keywords)))
            .ToHashSet(StringComparer.Ordinal);

        // Keywords added to allied units when they are included in another faction's roster
        keywords.UnionWith(definition.Factions
            .SelectMany(f => f.Allies ?? [])
            .SelectMany(a => a.UnitIncludeRules ?? [])
            .SelectMany(r => r.AddedKeywords ?? []));

        foreach (var category in definition.CategoryKeywords)
            if (!keywords.Contains(category.Keyword))
                LoggingBuffer.Error($"Category keyword '{category.Keyword}' does not match any unit.");

        if (!definition.CategoryKeywords.Any())
            return;

        foreach (var unit in unitList)
        {
            var unitKeywords = unit.Keywords.Concat(
                unit.UnitComposition.ModelCompositions.SelectMany(model => model.Keywords));
            var matches = definition.CategoryKeywords
                .Where(category => unitKeywords.Contains(category.Keyword, StringComparer.Ordinal))
                .ToList();

            if (matches.Count != 1)
                LoggingBuffer.Error($"{unit.Id}: expected exactly one category keyword, found {matches.Count}.");
        }
    }

    public static void ValidateModelKeywords(this Unit unit, string rosterName)
    {
        foreach (var model in unit.UnitComposition.ModelCompositions)
            foreach (var keyword in model.Keywords.Intersect(unit.Keywords, StringComparer.OrdinalIgnoreCase))
                LoggingBuffer.Error($"{rosterName}.{unit.Id}.{model.ModelName}: model keyword '{keyword}' is already on the unit.");
    }
}
