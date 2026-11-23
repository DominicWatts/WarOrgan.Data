# Warhammer 40,000 11th Edition dataset

Unofficial, fan-maintained data for personal and community use. Warhammer 40,000 and all associated names, rules and points values are the intellectual property of Games Workshop. This dataset is not affiliated with, licensed by or endorsed by Games Workshop.

## Coverage in 1.3.6.0

- Genestealer Cults: all 24 native datasheets, plus the Tectonic Fragdrill as a Warhammer Legends unit. Switch on Legends for a list to select it.
- Invulnerable save values (4+ or 5+) are shown as an ability on each GSC unit that has one.
- Tyranids: all 14 Vanguard Invader datasheets for use as Final Day allies: Winged Tyranid Prime, Gargoyles, Raveners, Hyperadapted Raveners, Lictor, Neurolictor, Von Ryan's Leapers, Mawloc, Trygon, Tyrannocyte, Winged Hive Tyrant, Deathleaper, The Red Terror and Parasite of Mortrex. Deathleaper cannot be your Warlord. The one-per-army limit for Epic Heroes is not enforced. Tyranids appear as their own faction, but are not a complete army. The Winged Tyranid Prime (Alpha Warrior) and Hyperadapted Raveners (Alpha Invader) give [SUSTAINED HITS 1] to the unit they lead.
- Unit and weapon profiles, model compositions, abilities and points tiers.
- Duplicate-unit prices are represented where the first/second copies and third copy have different costs.
- Datasheets are grouped as Characters, Battleline, Dedicated Transports and Other Datasheets. Allied Units will appear when allied rosters are added.
- Acolyte Hybrids with Hand Flamers and with Autopistols enforce their model count and per-unit/per-five-model wargear limits.
- Hybrid Metamorphs enforce their model count, hand flamer replacements and single cult icon.
- Atalan Jackals enforce 4-8 Jackals, 1-2 Wolfquads and their per-four-Jackal weapon limits. The rule that 2 Wolfquads require 8 Jackals cannot be enforced; build these units to the datasheet.
- Neophyte Hybrids enforce their 10 or 20 model sizes, leader weapon swap and up to two heavy and two special weapons per ten models. The one-of-each-weapon-per-ten limit cannot be enforced; build these units to the datasheet.
- The Patriarch carries the must-be-Warlord flag, but WarOrgan does not currently enforce it. Select it as Warlord yourself.
- Biophagus, Clamavus, Locus and Nexos attach as Support characters alongside a Leader.
- Leader effects are applied to the led unit: Might From Beyond, Twisted Science, Bio-horror Disruption, Sudden Assault and Nexus of Devotion. Cult Demagogue (+1 to Hit), The Chosen One and the Locus's Bodyguard ability are shown as text only.

- The Cult Ambush army rule and seven detachments: Heroes of the Uprising, Purestrain Broodswarm, Xenocult Masses, Final Day, Host of Ascension, Xenocreed Congregation and Outlander Claw, each with its rule, Force Disposition, enhancements and stratagems. Heroes of the Uprising adds KILLER to the Kelermorph, Locus, Reductus Saboteur and Sanctus. Enhancements that change profiles adjust them; Upgrades (Mark of the Star Children, Devious Disguises) can go on non-CHARACTER units. WarOrgan does not enforce the limit of three copies of the same Upgrade; build to the core rules. Host of Ascension costs 3 DP, so WarOrgan cannot offer it in Incursion, although the core rules let an Incursion army take a single 3 DP detachment.
- Battle sizes use the 11th Edition detachment points and enhancement limits: Incursion 2 DP and 2 enhancements, Strike Force 3 DP and 4. Onslaught is not in the 11th Edition core rules, so it uses the Strike Force limits. Unit limits are not enforced.

Final Day lets you include Tyranids Vanguard Invader units (excluding AIRCRAFT, BROODLORD and GENESTEALERS) up to half your points. They appear under Allied Units and cannot be your Warlord.

This version does not yet include the other two detachments (Biosanctic Broodsurge, Brood Brother Auxilia), core stratagems or allied Astra Militarum units.

## Installation

In WarOrgan's dataset importer, use:

`https://github.com/DominicWatts/WarOrgan.Data/releases/latest/download/DominicWatts-WH40K11e.zip`

The URL always targets the most recent published release. The downloaded ZIP should be imported directly and not extracted.

## Sources

- IRONBUILT community dataset `wh40k-11e`, version `2026.11.21-0401-mfm16-tempered`, manifest updated 21 November 2026.
- Source bundle SHA-256: `e19b68b0860ec5a6074325ea22dbec5654da0953590429450306094fe7e6dd71`.
- Wahapedia Genestealer Cults 11th Edition datasheets, checked 23 November 2026.
- Games Workshop Genestealer Cults Faction Pack, version 1.0, legal for matched play from 20 June 2026. Checked 23 November 2026.
- Games Workshop Warhammer Community faction packs and Munitorum Field Manual are the canonical authority for published changes and points.

Community datasets are used as structured authoring aids, not as unquestioned authorities. Values should be rechecked following every Games Workshop balance or faction update.

## Known limitations

- The WarOrgan builder validates schema and internal references; it cannot prove that game data matches the current rules.
- Wargear limits are being audited unit by unit. Users should consult the current faction rules for units not yet called out as constraint-verified in the coverage notes.
- Stable unit IDs must not be renamed after release because saved army lists refer to them.
