# Faction progress

Tracks each faction through the source layers set out in `CLAUDE.md`. Update a faction's row and notes in the same Prepare commit on `dataset/develop` that releases its work.

## Source layers

Work through these in order for each faction:

1. **BSData** builds the initial base data. Draft a roster with `WarOrgan.Data.BsdataImport` (see its README), then work through its import report. Use the November 2026 dump: `momper14/wh40k-11e` at commit `f015101d4c495173283e1b97e9d59f2befb877ad`, dated 11 November 2026.
2. **Wahapedia** supplies the core data and overrides BSData. Check a roster with `WarOrgan.Data.WahapediaCheck` (see its README), then work through its report.
3. **10th Edition codex** fills gaps that neither earlier source covers.
4. **Faction pack** applies GW's 11th Edition updates.

Key: `done`, `partial`, `-` (not started), `n/a`.

## Summary

| Faction | Branch | Scope | 1 BSData | 2 Wahapedia | 3 Codex 10e | 4 Faction pack | Datasheets | Detachments | Enhancements | Stratagems | Army rule |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Genestealer Cults | `faction/genestealer-cults` | Full | done | done | done | done (1.0) | 25/25 | 9/9 | 30/30 | 45/45 | done |
| Tyranids | `faction/tyranids` | Full (allies checked, rest draft) | done | done (allies) | - | partial (1.3) | 52 + 5 Legends | 10 | 34 | - | text only |
| Astra Militarum | `faction/astra-militarum` | Full (allies checked, rest draft) | done | done (allies) | - | - | 72 + 61 Legends | 11 | 34 | - | text only |

| Adepta Sororitas | `faction/adepta-sororitas` | Draft | done (draft) | - | - | - | 35 | 8 | 24 | - | text only |
| Adeptus Custodes | `faction/adeptus-custodes` | Draft | done (draft) | - | - | - | 30 | 7 | 17 | - | text only |
| Adeptus Mechanicus | `faction/adeptus-mechanicus` | Draft | done (draft) | - | - | - | 36 | 10 | 33 | - | text only |
| Agents of the Imperium | `faction/agents-of-the-imperium` | Draft | done (draft) | - | - | - | 34 | 5 | 16 | - | text only |
| Grey Knights | `faction/grey-knights` | Draft | done (draft) | - | - | - | 31 | 9 | 30 | - | text only |
| Imperial Knights | `faction/imperial-knights` | Draft | done (draft) | - | - | - | 22 | 8 | 26 | - | text only |
| Space Marines | `faction/space-marines` | Draft | done (draft) | - | - | - | 131 | 16 | 32 | - | text only |
| Black Templars | `faction/black-templars` | Draft | done (draft) | - | - | - | 15 | 18 | 36 | - | text only |
| Blood Angels | `faction/blood-angels` | Draft | done (draft) | - | - | - | 21 | 19 | 40 | - | text only |
| Dark Angels | `faction/dark-angels` | Draft | done (draft) | - | - | - | 18 | 19 | 40 | - | text only |
| Deathwatch | `faction/deathwatch` | Draft | done (draft) | - | - | - | 7 | 16 | 36 | - | text only |
| Imperial Fists | `faction/imperial-fists` | Draft | done (draft) | - | - | - | 3 | 17 | 36 | - | text only |
| Iron Hands | `faction/iron-hands` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | - | text only |
| Raven Guard | `faction/raven-guard` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | - | text only |
| Salamanders | `faction/salamanders` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | - | text only |
| Space Wolves | `faction/space-wolves` | Draft | done (draft) | - | - | - | 32 | 19 | 40 | - | text only |
| Ultramarines | `faction/ultramarines` | Draft | done (draft) | - | - | - | 16 | 17 | 36 | - | text only |
| White Scars | `faction/white-scars` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | - | text only |
| Chaos Space Marines | `faction/chaos-space-marines` | Draft | done (draft) | - | - | - | 82 | 17 | 62 | - | text only |
| Chaos Daemons | `faction/chaos-daemons` | Draft | done (draft) | - | - | - | 66 | 9 | 27 | - | text only |
| Chaos Knights | `faction/chaos-knights` | Draft | done (draft) | - | - | - | 19 | 8 | 28 | - | text only |
| Death Guard | `faction/death-guard` | Draft | done (draft) | - | - | - | 40 | 9 | 26 | - | text only |
| Emperor's Children | `faction/emperors-children` | Draft | done (draft) | - | - | - | 18 | 10 | 31 | - | text only |
| Thousand Sons | `faction/thousand-sons` | Draft | done (draft) | - | - | - | 30 | 9 | 29 | - | text only |
| World Eaters | `faction/world-eaters` | Draft | done (draft) | - | - | - | 26 | 8 | 22 | - | text only |
| Aeldari | `faction/aeldari` | Draft | done (draft) | - | - | - | 75 | 15 | 52 | - | text only |
| Drukhari | `faction/drukhari` | Draft | done (draft) | - | - | - | 27 | 9 | 30 | - | text only |
| Leagues of Votann | `faction/leagues-of-votann` | Draft | done (draft) | - | - | - | 22 | 10 | 32 | - | text only |
| Necrons | `faction/necrons` | Draft | done (draft) | - | - | - | 59 | 12 | 33 | - | text only |
| Orks | `faction/orks` | Draft | done (draft) | - | - | - | 68 | 15 | 38 | - | text only |
| T'au Empire | `faction/tau-empire` | Draft | done (draft) | - | - | - | 60 | 7 | 23 | - | text only |
Datasheet counts include Legends. Core stratagems live in `DataSetDefinition.json` and are not counted per faction.

## Genestealer Cults

- Initial import from IRONBUILT bundle `2026.11.21-0401-mfm16-tempered`, then checked line by line against Wahapedia.
- 2026-11-23: compared with the November BSData dump. It matched statlines, weapons, points, leaders, detachment costs, Force Dispositions and all 30 enhancements and their points. Wahapedia supported our data on every difference. BSData is stale for Goliath Truck (Goliath wheels), A Plan Generations in the Making, Primed and Ready, Demolition Run, Mind Control and Atalan Jackals' 6-9 model points. BSData spells two enhancements Biomorph Adaption and Fire-point Commander. BSData has no stratagems.
- 2026-11-23: first Wahapedia check run. 23 of 33 datasheets and detachments matched with no differences. Its findings were fixed in 1.4.1 (Scouts, Deadly Demise and Feel No Pain values, Biophagus alchemicus familiar, Fragdrill Fortification); 30 of 33 now match. The remaining notes are expected: bio-weapons, and Wahapedia typos in two stratagems and the Fragdrill's GENESTEALER CULT INFANTRY.
- Skipped: Cult Unveiled and Genespawn Onslaught (Boarding Actions detachments), and Crucible entries.
- Open: Patriarch must be Warlord (app enforcement); leader effect app checks. See `TODO.md`.

## Tyranids

- Allied datasheets for Final Day only: all 14 Vanguard Invader units. On 2026-11-23 they matched the November BSData dump on points, stats and weapons. BSData spells it distendible jaw; Wahapedia has distensible jaw. The Wahapedia check matched 13 of 14; Wahapedia omits the HYPERADAPTED RAVENERS keyword.
- 2026-11-24 (1.5.2): filled to a full faction from BSData: 38 more datasheets, 5 Legends, 10 detachments, 34 enhancements and the Synapse and Shadow in the Warp army rules. The 14 allied datasheets were kept unchanged. Carnifex and Mucolid Spores points came from Wahapedia (BSData sets them by condition). Spawned units (Spore Mines and Mucolid Spores from other units, Parasite of Mortrex rippers) were left out. Final Day still offers only the 14 Vanguard Invaders: the four new Vanguard Invader units are Aircraft, Broodlord or Genestealers.

## Astra Militarum

- Allied datasheets for Brood Brother Auxilia only. Cadian Shock Troops pilot matched the November BSData dump on 2026-11-23.
- 2026-11-23 trial: the BSData importer drafted 52 units; the Wahapedia check matched 16 with no differences and listed 93 notes for the rest, including Orders missing from six officers.
- 2026-11-23: expanded the roster to 15 infantry and mounted datasheets. All 15 match Wahapedia for profiles, abilities, points, composition, starting equipment, leaders and keywords; wargear options were checked manually. The batch adds Artillery Team, Attilan Rough Riders, Cadian Castellan, both Heavy Weapons Squads, Cadian Recon Squad, Catachan Jungle Fighters, Death Korps of Krieg, Death Riders, Field Ordnance Battery, Kasrkin, Krieg Combat Engineers, Krieg Heavy Weapons Squad and Primaris Psyker alongside Cadian Shock Troops.
- 2026-11-23: added the Cadian, Catachan and Krieg Command Squads. The Cadian and Krieg squads use separate fixed specialist model entries so WarOrgan cannot assign their mandatory equipment to the wrong veteran; the source check's aggregate model-name notes are expected for those two units.
- 2026-11-23: completed all 51 non-AIRCRAFT datasheets eligible for Brood Brother Auxilia. The final 33 add the transports, Sentinels, artillery vehicles, Leman Russ and Rogal Dorn variants, super-heavy tanks, Aegis Defence Line and Cyclops Demolition Vehicle. All wargear options were checked manually; the Wahapedia ally-page check reports 48 exact matches and only the expected command-squad aggregation, Cyclops empty-loadout and excluded Valkyrie notes.

- 2026-11-24 (1.5.2): filled to a full faction from BSData: 21 more datasheets, 61 Legends, 11 detachments, 34 enhancements and the Voice of Command army rule. The 51 allied datasheets were kept unchanged. Hippogriff AFV and Legends weapon battery points came from Wahapedia. Hell's Last (Legends) is held back: its statline does not match. Brood Brother Auxilia now lists the 51 eligible datasheets by ID, so the new units (including Ratlings, the Valkyrie and the Legends units, which the keyword filter missed) are not offered to Genestealer Cults.
## BSData drafts

- 2026-11-24 (1.5.0): 31 factions drafted with `WarOrgan.Data.BsdataImport` from the pinned dump and merged unchecked. Chapters are their own factions with the Space Marines roster included (`IsIncludedInPrimaryRoster`); each chapter roster also repeats the generic Space Marine detachments, because BSData lists them for every chapter.
- Next for each faction: run `WarOrgan.Data.WahapediaCheck`, add stratagems, enhancement and leader effects, and work through the import report (regenerate it with the importer; it is not committed).
- 2026-11-24 (1.5.1): enhancements imported for every draft faction (most catalogues tie them to a detachment by a visibility condition). Still without enhancements, because BSData has none: Armoured Trailblazers, Brazen Engines, Butchers of Khorne, Contagion Engines, Flyblown Host, Hand of the Dynasty, Pantheon of Woe, Skyshroud Spearhead, Spectacle of Slaughter, Veiled Blade Elimination Force and Warptide. Chapters now get only the Space Marine detachments plus their own.
- Datasheets left out because they failed dataset validation (mostly weapons that no wargear option can select, or model counts the importer could not rebuild). Leader lists that named them were trimmed.
  - Adepta Sororitas: Battle Sisters Squad, Crusaders, Dominion Squad
  - Adeptus Mechanicus: Pteraxii Sterylizors
  - Agents of the Imperium: Aquila Kill Team, Deathwatch Kill Team, Imperial Navy Breachers, Sisters Of Battle Squad, Subductor Squad, Vigilant Squad
  - Space Marines: Captain, Captain In Gravis Armour, Tactical Squad, Terminator Assault Squad
  - Black Templars: Crusader Squad
  - Blood Angels: Death Company Intercessors, Death Company Marines, Death Company Marines With Bolt Rifles, Death Company Marines With Boltguns, Death Company Marines With Boltguns And Jump Packs, Furioso Dreadnought
  - Dark Angels: Deathwing Command Squad
  - Deathwatch: Deathwatch Terminator Squad, Decimus Kill Team, Talonstrike Kill Team
  - Space Wolves: Wolf Guard Battle Leader In Terminator Armour, Wolf Guard Battle Leader On Thunderwolf, Wolf Guard Pack Leader, Wolf Guard Pack Leader In Terminator Armour, Wolf Guard Pack Leader With Jump Pack, Wolf Guard Terminators, Wolf Lord On Thunderwolf, Wolf Scouts
  - Chaos Daemons: Blue Horrors, Pink Horrors
  - Death Guard: Plague Marines
  - Thousand Sons: Scarab Occult Terminators
  - World Eaters: Jakhals
  - Aeldari: Corsair Voidscarred, Hemlock Wraithfighter, Hornet
  - Drukhari: Scourges With Heavy Weapons, Scourges With Shardcarbines, Wracks
  - Necrons: C'Tan Shard Of The Deceiver, C'Tan Shard Of The Nightbringer, C'Tan Shard Of The Void Dragon, Canoptek Tomb Crawlers, Transcendent C'Tan
  - Orks: Grot Tanks, Kill Tank
  - T'au Empire: Broadside Battlesuits, Pathfinder Team, The Twin Lance
