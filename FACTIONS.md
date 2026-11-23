# Faction progress

Tracks each faction through the source layers set out in `CLAUDE.md`. Update a faction's row and notes in the same Prepare commit on `dataset/develop` that releases its work.

## Source layers

Work through these in order for each faction:

1. **BSData** builds the initial base data. Use the November 2026 dump: `momper14/wh40k-11e` at commit `f015101d4c495173283e1b97e9d59f2befb877ad`, dated 11 November 2026.
2. **Wahapedia** supplies the core data and overrides BSData.
3. **10th Edition codex** fills gaps that neither earlier source covers.
4. **Faction pack** applies GW's 11th Edition updates.

Key: `done`, `partial`, `-` (not started), `n/a`.

## Summary

| Faction | Branch | Scope | 1 BSData | 2 Wahapedia | 3 Codex 10e | 4 Faction pack | Datasheets | Detachments | Enhancements | Stratagems | Army rule |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Genestealer Cults | `faction/genestealer-cults` | Full | done | done | done | done (1.0) | 25/25 | 9/9 | 30/30 | 45/45 | done |
| Tyranids | `faction/tyranids` | GSC allies only | done (allies) | done (allies) | - | partial (1.3) | 14 Vanguard Invader | - | - | - | - |
| Astra Militarum | `faction/astra-militarum` | GSC allies only | done (pilot) | done (pilot) | - | - | 1 of 51 | - | - | - | - |

Datasheet counts include Legends. Core stratagems live in `DataSetDefinition.json` and are not counted per faction.

## Genestealer Cults

- Initial import from IRONBUILT bundle `2026.11.21-0401-mfm16-tempered`, then checked line by line against Wahapedia.
- 2026-11-23: compared with the November BSData dump. It matched statlines, weapons, points, leaders, detachment costs, Force Dispositions and 28 of 30 enhancements. Wahapedia supported our data on every difference. BSData is stale for Goliath Truck (Goliath wheels), A Plan Generations in the Making, Primed and Ready, Demolition Run, Mind Control and Atalan Jackals' 6-9 model points. BSData misspells Biomorph Adaption and is missing Firepoint Commander. BSData has no stratagems.
- Skipped: Cult Unveiled and Genespawn Onslaught (Boarding Actions detachments), and Crucible entries.
- Open: Patriarch must be Warlord (app enforcement); leader effect app checks. See `TODO.md`.

## Tyranids

- Allied datasheets for Final Day only: all 14 Vanguard Invader units. On 2026-11-23 they matched the November BSData dump on points, stats and weapons. BSData spells it distendible jaw; Wahapedia has distensible jaw.
- Full faction (army rule, detachments, remaining datasheets) not started.

## Astra Militarum

- Allied datasheets for Brood Brother Auxilia only. Cadian Shock Troops pilot matched the November BSData dump on 2026-11-23.
- Next: the other 50 current datasheets, starting from the BSData base. The GSC catalogue links every one of them. Skip Legends.
