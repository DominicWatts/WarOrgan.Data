# BSData importer

Drafts a WarOrgan roster from a BSData dump: step 1 of the source order in `FACTIONS.md`. The output is a starting point, not a release. Check it against Wahapedia and work through the import report before copying it onto the faction branch.

```bash
curl -L -o /tmp/bsdata.zip https://codeload.github.com/momper14/wh40k-11e/zip/f015101d4c495173283e1b97e9d59f2befb877ad
dotnet run --project WarOrgan.Data.BsdataImport -- --source /tmp/bsdata.zip --catalogue "Genestealer Cults" --out /tmp/bsdata-out
```

Allied rosters take units only, filtered by the detachment's exclusions:

```bash
dotnet run --project WarOrgan.Data.BsdataImport -- --source /tmp/bsdata.zip --catalogue "Imperium - Astra Militarum" \
  --no-detachments --skip-legends --exclude-keywords "Aircraft;Commissar;Epic Hero;Militarum Tempestus;Ogryn;Ratling;Tech-Priest Enginseer;Ministorum Priest" \
  --out /tmp/bsdata-out
```

Run it from the repository root so it finds `DataSetDefinition.json`, or pass `--definition`. It writes `<Faction>.json`, `<Faction> Legends.json` when there are Legends units, and `<Faction> import-report.md`.

## What it converts

- Statlines (identical ones merged), compact base sizes, and an `Inv` characteristic plus ability text for invulnerable saves.
- Weapons, with keywords sorted and checked against the registry.
- Abilities, Transport capacity and the Unit Composition note.
- Core abilities, checked against the registry. Rules that only apply under a unit condition are reported, not added.
- Leader and Support attachments, matched to unit IDs in the same roster.
- Points by unit size, and the extra cost for later copies of a unit.
- Model compositions. BSData's per-loadout models ("Shock Trooper w/ Flamer") are folded into one composition with wargear options, following the conventions in `CLAUDE.md`. Size choices ("1 Sergeant and 9 Troopers" or "2 and 18") become model ranges, with option limits expressed per X models.
- Sibling loadouts that replace the same starting weapon are merged into one mutually exclusive WarOrgan choice. Multi-profile and repeated weapons remain together as one choice bundle.
- Leader lists written either as bullets or inline after the Leader introduction.
- Detachments with a Detachment Points cost: Force Dispositions, rule, enhancements and their unit restrictions. Other detachment categories become Tags.
- Army rules from the catalogue's shared rules.

## What it leaves to you

- Stratagems (BSData has none) and detachment effects.
- Stat and weapon changes from enhancements, leader effects and wargear.
- Limits on a single choice inside an option, and unusual per-model-count limits set by modifiers. The report lists each one.
- Anything BSData has wrong or out of date. Comparing GSC on 2026-11-23 found stale rule texts, a stale weapon, a missing Leader list and an inconsistent model count.
