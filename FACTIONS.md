# Faction progress

Tracks each faction through the source layers set out in `CLAUDE.md`. Update a faction's row and notes in the same Prepare commit on `dataset/develop` that releases its work.

## Source layers

Work through these in order for each faction:

1. **BSData** builds the initial base data. Draft a roster with `WarOrgan.Data.BsdataImport` (see its README), then work through its import report. Use the November 2026 dump: `momper14/wh40k-11e` at commit `f015101d4c495173283e1b97e9d59f2befb877ad`, dated 11 November 2026.
2. **List Forge** is a newer copy of the same BSData catalogues, published at `https://files.data-ark.dev/wh40k-11e/` (version `71860bdbe486ddeed7fd46e80df2735450dcc8a6`, fetched 24 November 2026). It replaces the base where they differ. Its source commit is not in a public repository.
3. **Wahapedia** supplies the core data and overrides both. Check a roster with `WarOrgan.Data.WahapediaCheck` (see its README), then work through its report.
4. **Faction pack** applies GW's 11th Edition updates. Download packs from `https://www.warhammer-community.com/en-gb/downloads/warhammer-40000/`.

10th and 11th Edition codexes are not used.

Key: `done`, `partial`, `-` (not started), `n/a`.

## Summary

| Faction | Branch | Scope | 1 BSData | 2 List Forge | 3 Wahapedia | 4 Faction pack | Datasheets | Detachments | Enhancements | Stratagems | Army rule |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Genestealer Cults | `faction/genestealer-cults` | Full | done | - | done | done (1.1) | 25/25 | 9/9 | 30/30 | 45/45 | done |
| Tyranids | `faction/tyranids` | Full (WarOrgan passed) | done | - | done | partial (1.3) | 52 + 5 Legends | 10 | 34 | 51/51 added | text only |
| Astra Militarum | `faction/astra-militarum` | Full (WarOrgan sign-off pending) | done | - | partial (allies checked) | - | 72 + 63 Legends | 11 | 34 | 57/57 added | text only |
| Adepta Sororitas | `faction/adepta-sororitas` | Draft | done (draft) | - | - | - | 38 | 8 | 24 | 36 added (7/8) | text only |
| Adeptus Custodes | `faction/adeptus-custodes` | Draft | done (draft) | - | - | - | 30 | 7 | 17 | 23/23 added | text only |
| Adeptus Mechanicus | `faction/adeptus-mechanicus` | Draft | done (draft) | - | - | - | 37 | 10 | 33 | 45 added (9/10) | text only |
| Agents of the Imperium | `faction/agents-of-the-imperium` | Draft | done (draft) | - | - | - | 40 | 5 | 16 | 30/30 added | text only |
| Grey Knights | `faction/grey-knights` | Draft | done (draft) | - | - | - | 31 | 9 | 30 | 45/45 added | text only |
| Imperial Knights | `faction/imperial-knights` | Draft | done (draft) | - | - | - | 22 | 8 | 26 | 39/39 added | text only |
| Space Marines | `faction/space-marines` | Draft | done (draft) | - | - | - | 133 | 16 | 32 | 52/52 added | text only |
| Black Templars | `faction/black-templars` | Draft | done (draft) | - | - | - | 16 | 18 | 36 | 52 generic | text only |
| Blood Angels | `faction/blood-angels` | Draft | done (draft) | - | - | - | 22 | 19 | 40 | 52 generic | text only |
| Dark Angels | `faction/dark-angels` | Draft | done (draft) | - | - | - | 18 | 19 | 40 | 52 generic | text only |
| Deathwatch | `faction/deathwatch` | Draft | done (draft) | - | - | - | 10 | 16 | 36 | 48 generic | text only |
| Imperial Fists | `faction/imperial-fists` | Draft | done (draft) | - | - | - | 3 | 17 | 36 | 52 generic | text only |
| Iron Hands | `faction/iron-hands` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | 52 generic | text only |
| Raven Guard | `faction/raven-guard` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | 52 generic | text only |
| Salamanders | `faction/salamanders` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | 52 generic | text only |
| Space Wolves | `faction/space-wolves` | Draft | done (draft) | - | - | - | 34 | 19 | 40 | 52 generic | text only |
| Ultramarines | `faction/ultramarines` | Draft | done (draft) | - | - | - | 16 | 17 | 36 | 52 generic | text only |
| White Scars | `faction/white-scars` | Draft | done (draft) | - | - | - | 2 | 17 | 36 | 52 generic | text only |
| Chaos Space Marines | `faction/chaos-space-marines` | Draft | done (draft) | - | - | - | 82 | 17 | 62 | - | text only |
| Chaos Daemons | `faction/chaos-daemons` | Draft | done (draft) | - | - | - | 68 | 9 | 27 | - | text only |
| Chaos Knights | `faction/chaos-knights` | Draft | done (draft) | - | - | - | 19 | 8 | 28 | - | text only |
| Death Guard | `faction/death-guard` | Draft | done (draft) | - | - | - | 41 | 9 | 26 | - | text only |
| Emperor's Children | `faction/emperors-children` | Draft | done (draft) | - | - | - | 18 | 10 | 31 | - | text only |
| Thousand Sons | `faction/thousand-sons` | Draft | done (draft) | - | - | - | 31 | 9 | 29 | - | text only |
| World Eaters | `faction/world-eaters` | Draft | done (draft) | - | - | - | 27 | 8 | 22 | - | text only |
| Aeldari | `faction/aeldari` | Draft | done (draft) | - | - | - | 78 | 15 | 52 | - | text only |
| Drukhari | `faction/drukhari` | Draft | done (draft) | - | - | - | 30 | 9 | 30 | - | text only |
| Leagues of Votann | `faction/leagues-of-votann` | Draft | done (draft) | - | - | - | 22 | 10 | 32 | - | text only |
| Necrons | `faction/necrons` | Draft | done (draft) | - | - | - | 64 | 12 | 33 | - | text only |
| Orks | `faction/orks` | Draft | done (draft) | - | - | - | 70 | 15 | 38 | - | text only |
| T'au Empire | `faction/tau-empire` | Draft | done (draft) | - | - | - | 63 | 7 | 23 | 31/31 added | text only |

Datasheet counts include Legends. Core stratagems live in `DataSetDefinition.json` and are not counted per faction.

## Genestealer Cults

- Initial import from IRONBUILT bundle `2026.11.21-0401-mfm16-tempered`, then checked line by line against Wahapedia.
- 2026-11-23: compared with the November BSData dump. It matched statlines, weapons, points, leaders, detachment costs, Force Dispositions and all 30 enhancements and their points. Wahapedia supported our data on every difference. BSData is stale for Goliath Truck (Goliath wheels), A Plan Generations in the Making, Primed and Ready, Demolition Run, Mind Control and Atalan Jackals' 6-9 model points. BSData spells two enhancements Biomorph Adaption and Fire-point Commander. BSData has no stratagems.
- 2026-11-23: first Wahapedia check run. 23 of 33 datasheets and detachments matched with no differences. Its findings were fixed in 1.4.1 (Scouts, Deadly Demise and Feel No Pain values, Biophagus alchemicus familiar, Fragdrill Fortification); 30 of 33 now match. The remaining notes are expected: bio-weapons, and Wahapedia typos in two stratagems and the Fragdrill's GENESTEALER CULT INFANTRY.
- 2026-11-24 (1.5.13): checked against Faction Pack 1.1 (legal from 22 July 2026). Its six Rules Updates changes (Cult Ambush character exclusion and 8" marker distance; 8" in Evasive Vanguard, In the Shadow of Iron and Rapid Feint; Along Shadowed Trails' new When) were already in the data. Cult Ambush now says "identical to that destroyed unit" as in the pack; Wahapedia says "your", so the Wahapedia check may report it as an expected text difference.
- Skipped: Cult Unveiled and Genespawn Onslaught (Boarding Actions detachments), and Crucible entries.
- Open: Patriarch must be Warlord (app enforcement); leader effect app checks. See `TODO.md`.

## Tyranids

- 2026-11-24 (1.5.10): standalone roster and Legends app checks passed.
- Allied datasheets for Final Day only: all 14 Vanguard Invader units. On 2026-11-23 they matched the November BSData dump on points, stats and weapons. BSData spells it distendible jaw; Wahapedia has distensible jaw. The Wahapedia check matched 13 of 14; Wahapedia omits the HYPERADAPTED RAVENERS keyword.
- 2026-11-24 (1.5.2): filled to a full faction from BSData: 38 more datasheets, 5 Legends, 10 detachments, 34 enhancements and the Synapse and Shadow in the Warp army rules. The 14 allied datasheets were kept unchanged. Carnifex and Mucolid Spores points came from Wahapedia (BSData sets them by condition). Spawned units (Spore Mines and Mucolid Spores from other units, Parasite of Mortrex rippers) were left out. Final Day still offers only the 14 Vanguard Invaders: the four new Vanguard Invader units are Aircraft, Broodlord or Genestealers.

## Astra Militarum

- 2026-11-24 (1.5.10): added Hell’s Last and Vendetta Gunship and reviewed main and Legends wargear. Ratling Battlemutt and Demolition Gear passed their WarOrgan checks. Sign-off is still pending for the Grenadier Squad, the Tempestus loadouts and the 51-unit ally list.

- Allied datasheets for Brood Brother Auxilia only. Cadian Shock Troops pilot matched the November BSData dump on 2026-11-23.
- 2026-11-23 trial: the BSData importer drafted 52 units; the Wahapedia check matched 16 with no differences and listed 93 notes for the rest, including Orders missing from six officers.
- 2026-11-23: expanded the roster to 15 infantry and mounted datasheets. All 15 match Wahapedia for profiles, abilities, points, composition, starting equipment, leaders and keywords; wargear options were checked manually. The batch adds Artillery Team, Attilan Rough Riders, Cadian Castellan, both Heavy Weapons Squads, Cadian Recon Squad, Catachan Jungle Fighters, Death Korps of Krieg, Death Riders, Field Ordnance Battery, Kasrkin, Krieg Combat Engineers, Krieg Heavy Weapons Squad and Primaris Psyker alongside Cadian Shock Troops.
- 2026-11-23: added the Cadian, Catachan and Krieg Command Squads. The Cadian and Krieg squads use separate fixed specialist model entries so WarOrgan cannot assign their mandatory equipment to the wrong veteran; the source check's aggregate model-name notes are expected for those two units.
- 2026-11-23: completed all 51 non-AIRCRAFT datasheets eligible for Brood Brother Auxilia. The final 33 add the transports, Sentinels, artillery vehicles, Leman Russ and Rogal Dorn variants, super-heavy tanks, Aegis Defence Line and Cyclops Demolition Vehicle. All wargear options were checked manually; the Wahapedia ally-page check reports 48 exact matches and only the expected command-squad aggregation, Cyclops empty-loadout and excluded Valkyrie notes.

- 2026-11-24 (1.5.2): filled to a full faction from BSData: 21 more datasheets, 61 Legends, 11 detachments, 34 enhancements and the Voice of Command army rule. The 51 allied datasheets were kept unchanged. Hippogriff AFV and Legends weapon battery points came from Wahapedia. Hell's Last (Legends) is left out: Wahapedia has no 11th Edition datasheet for it. Brood Brother Auxilia now lists the 51 eligible datasheets by ID, so the new units (including Ratlings, the Valkyrie and the Legends units, which the keyword filter missed) are not offered to Genestealer Cults.
- 2026-11-24 (1.5.11, issue 8): two Leaders for Cadian Shock Troops, Catachan Jungle Fighters, Death Korps of Krieg and Tempestus Scions (at most one Command Squad); Wahapedia wargear costs on every Leman Russ (+5 per lascannon, including the hull lascannon, multi-melta and plasma cannon; +15 demolisher battle cannon on the Commander); Recon Squads can take two long-las (the datasheet makes the second an alternative to the missile team, which WarOrgan cannot express); Bullgryn wargear swaps cover all six models; Ogryn Bodyguard and Nork Deddog join a Command Squad as Bodyguard-type leaders. Orders text for the Commissars, Dreir, Leontus, Creed and the Tempestus Command Squad was already added in 1.5.10.
- 2026-11-24: WarOrgan checks passed for Leman Russ lascannon and multi-melta costs and all six Bullgryn wargear swaps. The Commander's demolisher cost and the other issue 8 cases remain unconfirmed.
## BSData drafts

- 2026-11-24 (1.5.0): 31 factions drafted with `WarOrgan.Data.BsdataImport` from the pinned dump and merged unchecked. Chapters are their own factions with the Space Marines roster included (`IsIncludedInPrimaryRoster`); each chapter roster also repeats the generic Space Marine detachments, because BSData lists them for every chapter.
- Next for each faction: run `WarOrgan.Data.WahapediaCheck`, add stratagems, enhancement and leader effects, and work through the import report (regenerate it with the importer; it is not committed).
- 2026-11-24 (1.5.1): enhancements imported for every draft faction (most catalogues tie them to a detachment by a visibility condition). Still without enhancements, because BSData has none: Armoured Trailblazers, Brazen Engines, Butchers of Khorne, Contagion Engines, Flyblown Host, Hand of the Dynasty, Pantheon of Woe, Skyshroud Spearhead, Spectacle of Slaughter, Veiled Blade Elimination Force and Warptide. Chapters now get only the Space Marine detachments plus their own.
- 2026-11-24 (1.5.3): the 54 held-back datasheets were resolved. 40 now build, after an importer repair pass (weapons no option gave out became swaps for a starting weapon of the same type or starting equipment; unused statlines, "(ref. only)" profiles, zero-limit options and choices offered twice were removed) and hand fixes from Wahapedia for Crusader Squad, Crusaders, Corsair Voidscarred and Grot Tanks. Leader lists that had been trimmed were restored. Repaired wargear is permissive: some units allow loadouts their datasheet does not.
- Left out because Wahapedia has no 11th Edition datasheet for them (BSData leftovers):
  - Space Marines: Tactical Squad, Captain In Gravis Armour
  - Blood Angels: Death Company Intercessors, Death Company Marines With Bolt Rifles, Death Company Marines With Boltguns, Death Company Marines With Boltguns And Jump Packs, Furioso Dreadnought
  - Dark Angels: Deathwing Command Squad
  - Space Wolves: Wolf Guard Battle Leader In Terminator Armour, Wolf Guard Battle Leader On Thunderwolf, Wolf Guard Pack Leader, Wolf Guard Pack Leader In Terminator Armour, Wolf Guard Pack Leader With Jump Pack, Wolf Lord On Thunderwolf
- 2026-11-24 (1.5.4): a shield drone adds 1 to the Wounds of the T'au character carrying it (Cadre Fireblade, Ethereal, Coldstar and Enforcer Commanders). Squads keep it as text, because WarOrgan shows one statline per model type. No T'au enhancement needs a drone or changes stats through one. The drafted drone option lists differ from Wahapedia (for example guardian drones offered to characters); fix them in the T'au Wahapedia check.
- 2026-11-24 (1.5.9): added the Wahapedia stratagems for T'au Empire, Tyranids, Astra Militarum, Adepta Sororitas, Adeptus Custodes, Adeptus Mechanicus, Agents of the Imperium, Grey Knights and Imperial Knights. Copied the generic Space Marine stratagems into the matching detachment copies in every chapter roster, fixing issue 7.
- 2026-11-24 (1.5.12, issue 9): army rules taken from the Wahapedia faction pages for the 17 factions BSData left without them or with only part of them (it keeps them in parent or library catalogues): Space Marines and every chapter (Combat Doctrines, Transhuman Strategist, Librarius except Black Templars, Special Move Types, plus Heirs of Sigismund, The Ravenwing and The Deathwing, Veteran Recruits and Curse of the Wulfen for their chapters), Aeldari, Drukhari, Chaos Daemons, Chaos Knights and Imperial Knights. Super-heavy Walker is a core ability, so it is not repeated as an army rule.
