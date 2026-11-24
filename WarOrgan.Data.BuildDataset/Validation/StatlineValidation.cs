using WarOrgan.Data.BuildDataset.Utils;
using WarOrgan.Model.Faction;
using WarOrgan.Model.Unit;

namespace WarOrgan.Data.BuildDataset.Validation;

public static class StatlineValidation
{
    public static void ValidateStatlines(this Unit unit, FactionRoster faction)
    {
        var context = $"{faction.Name}.{unit.Id}";

        if (!unit.StatLines.Any())
            LoggingBuffer.Error($"{context}: missing statlines.");

        if (unit.StatLines.Select(s => s.StatName).Distinct().Count() != unit.StatLines.Count)
            LoggingBuffer.Error($"{context}: statline names are not unique.");

        var matchedStatlines = unit.GetMatchedStatlines(context);

        foreach (var statline in unit.StatLines)
        {
            statline.Stats.ValidateCharacteristics(context);

            if (!matchedStatlines.Contains(statline) && !statline.Features.HasFlag(StatFeature.AlwaysShow))
                LoggingBuffer.Error($"{context}: unmatched statline '{statline.StatName}'.");
        }

        unit.ValidateWeaponStats(context);
    }

    private static HashSet<StatLine> GetMatchedStatlines(this Unit unit, string context)
    {
        var matchedStatlines = new HashSet<StatLine>();

        foreach (var model in unit.UnitComposition.ModelCompositions)
        {
            if (string.IsNullOrWhiteSpace(model.StatlineRef) && unit.StatLines.Count > 1)
                LoggingBuffer.Error($"{context}.{model.ModelName}: requires a StatlineRef.");

            var statline = unit.StatLines.FirstOrDefault(s => s.StatName == model.StatlineRef);
            if (statline == null)
                LoggingBuffer.Error($"{context}.{model.ModelName}: unknown StatlineRef '{model.StatlineRef}'.");
            else matchedStatlines.Add(statline);
        }

        return matchedStatlines;
    }

    private static void ValidateWeaponStats(this Unit unit, string context)
    {
        unit.Weapons.Select(w => w.Id).ValidateUnique($"{context} weapon sets");

        var weaponIdsAcrossSets = unit.Weapons
            .SelectMany((weaponSet, setIndex) => weaponSet.Weapons
                .Where(weapon => !string.IsNullOrWhiteSpace(weapon.Id))
                .Select(weapon => (WeaponId: weapon.Id, WeaponSetId: weaponSet.Id, weapon.DisplayName, SetIndex: setIndex)))
            .GroupBy(weapon => weapon.WeaponId, StringComparer.Ordinal)
            .Where(group => group.Select(weapon => weapon.SetIndex).Distinct().Skip(1).Any());

        foreach (var duplicate in weaponIdsAcrossSets)
        {
            var entries = duplicate.ToList();
            // A ranged and melee profile with one name is the same selectable weapon.
            var isSharedRangedMeleeProfile = entries.Count == 2
                && entries.Select(weapon => weapon.WeaponSetId)
                    .ToHashSet(StringComparer.Ordinal)
                    .SetEquals(new[] { "RANGED WEAPONS", "MELEE WEAPONS" })
                && entries.All(weapon => !string.IsNullOrWhiteSpace(weapon.DisplayName))
                && entries[0].DisplayName == entries[1].DisplayName;

            if (isSharedRangedMeleeProfile)
                continue;

            LoggingBuffer.Error($"{context}: weapon ID \"{duplicate.Key}\" appears in multiple weapon sets.");
        }

        foreach (var weaponSet in unit.Weapons)
        {
            // Firing modes share an equipment ID so selecting it enables every profile.
            weaponSet.Weapons.Select(w => w.Id).ValidateIds($"{context}.{weaponSet.Id} weapons");

            foreach (var weapon in weaponSet.Weapons)
                weapon.Stats.ValidateCharacteristics($"{context}.{weapon.Id}");
        }
    }

    private static void ValidateCharacteristics(this List<Characteristic> stats, string context)
    {
        if (!stats.Any())
            LoggingBuffer.Error($"{context}: missing characteristics.");

        stats.Select(s => s.Id).ValidateUnique($"{context} characteristics");

        if (stats.Any(s => string.IsNullOrWhiteSpace(s.Value)))
            LoggingBuffer.Error($"{context}: empty characteristic value.");
    }
}
