# Dataset changelog

This file is the canonical public release log for the `DominicWatts-WH40K11e` dataset. GitHub release notes should copy the matching version section and link back here for the live roadmap.

The version is a four-part .NET `Version`, not strict Semantic Versioning. WarOrgan uses the minor and build fields: increment the third field for data changes and the second field for a dataset-format or feature change.

## Unreleased

### Planned

- Import and verify the remaining Warhammer 40,000 11th Edition factions one at a time.
- Add Genestealer Cults army rules.
- Add all nine Genestealer Cults detachments and their Force Dispositions.
- Add detachment enhancements and stratagems.
- Add Brood Brothers and Tyranid allies with detachment-specific inclusion rules.
- Strengthen model-option constraints where WarOrgan's data model can express them.
- Add repeatable source-comparison checks for names, model counts and points.

## 1.1.1.0 - 2026-11-23

### Fixed

- Grouped Genestealer Cults datasheets under GW-style Characters, Battleline, Dedicated Transports and Other Datasheets headings.
- Shortened displayed base sizes to numeric values so they fit the datasheet panel.
- Rebuilt Acolyte Hybrids with Hand Flamers as one leader and 4-9 hybrids, with enforced cult icon, demolition charge and leader bio-weapon replacements.
- Verified Acolyte Hybrids with Hand Flamers at the current 75-point and 140-point unit costs while preserving the 65-point and 120-point Autopistol unit costs.
- Enforced the Patriarch's must-be-Warlord rule and applied Might From Beyond to the melee weapons of attached Purestrain Genestealers.
- Added validation requiring every unit to match exactly one configured datasheet category.

### Still planned

- Add the Allied Units category alongside the first allied roster import.
- Convert and verify the remaining complex unit-composition and wargear constraints.

## 1.1.0.0 - 2026-11-23

### Added

- Created the stable `DominicWatts-WH40K11e` dataset lineage.
- Added all 25 native Genestealer Cults datasheets.
- Added model profiles, weapon profiles, unit abilities, core abilities and configurable wargear represented by the source data.
- Added current model-count points tiers and first/second versus third-copy prices where applicable.
- Added source provenance and known-limit documentation to the packaged dataset README.

### Scope limitations

- This release is intended for Genestealer Cults unit selection and points-based list construction.
- Detachments, Force Dispositions, enhancements, stratagems and allied units are not enabled yet.

## Release checklist

1. Confirm the source snapshot/version and update the packaged dataset README.
2. Run `dotnet run --project WarOrgan.Data.BuildDataset` from the repository root.
3. Require zero validation errors and review every warning.
4. Inspect the ZIP's `DataSetInfo.json` and confirm the intended dataset version.
5. Import the ZIP into WarOrgan and reproduce a known list and points total.
6. Tag the verified commit as `v<major>.<minor>.<build>` (for example `v1.1.0`) and push the tag.
7. Confirm the release workflow created the GitHub release, copied this version's changelog section and attached the import-ready ZIP.
8. Confirm `https://github.com/DominicWatts/WarOrgan.Data/releases/latest/download/DominicWatts-WH40K11e.zip` resolves to the new asset.
