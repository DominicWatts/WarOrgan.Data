# Warhammer 40,000 11th Edition dataset

Unofficial, fan-maintained data for personal and community use. Warhammer 40,000 and all associated names, rules and points values are the intellectual property of Games Workshop. This dataset is not affiliated with, licensed by or endorsed by Games Workshop.

## Coverage in 1.1.2.0

- Genestealer Cults: all 25 native datasheets.
- Unit and weapon profiles, model compositions, abilities and points tiers.
- Duplicate-unit prices are represented where the first/second copies and third copy have different costs.
- Datasheets are grouped as Characters, Battleline, Dedicated Transports and Other Datasheets. Allied Units will appear when allied rosters are added.
- Acolyte Hybrids with Hand Flamers and with Autopistols enforce their model count and per-unit/per-five-model wargear limits.
- Hybrid Metamorphs enforce their model count, hand flamer replacements and single cult icon.
- Atalan Jackals enforce 4-8 Jackals, 1-2 Wolfquads and their per-four-Jackal weapon limits. The rule that 2 Wolfquads require 8 Jackals cannot be enforced; build these units to the datasheet.
- Neophyte Hybrids enforce their 10 or 20 model sizes, leader weapon swap and one-of-each-per-ten heavy and special weapon limits. The two-per-ten total for each weapon list cannot be enforced; build these units to the datasheet.
- The Patriarch carries the must-be-Warlord flag, but WarOrgan does not currently enforce it. Select it as Warlord yourself.
- The Patriarch's Might From Beyond leader effect is encoded for attached Purestrain Genestealers.

This version does not yet include detachments, Force Dispositions, enhancements, stratagems or allied Astra Militarum/Tyranid units.

## Installation

In WarOrgan's dataset importer, use:

`https://github.com/DominicWatts/WarOrgan.Data/releases/latest/download/DominicWatts-WH40K11e.zip`

The URL always targets the most recent published release. The downloaded ZIP should be imported directly and not extracted.

## Sources

- IRONBUILT community dataset `wh40k-11e`, version `2026.11.21-0401-mfm16-tempered`, manifest updated 21 November 2026.
- Source bundle SHA-256: `e19b68b0860ec5a6074325ea22dbec5654da0953590429450306094fe7e6dd71`.
- Wahapedia Genestealer Cults 11th Edition datasheets, checked 23 November 2026.
- Games Workshop Warhammer Community faction packs and Munitorum Field Manual are the canonical authority for published changes and points.

Community datasets are used as structured authoring aids, not as unquestioned authorities. Values should be rechecked following every Games Workshop balance or faction update.

## Known limitations

- The WarOrgan builder validates schema and internal references; it cannot prove that game data matches the current rules.
- Wargear limits are being audited unit by unit. Users should consult the current faction rules for units not yet called out as constraint-verified in the coverage notes.
- Stable unit IDs must not be renamed after release because saved army lists refer to them.
