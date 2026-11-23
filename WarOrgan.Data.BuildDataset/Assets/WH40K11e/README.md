# Warhammer 40,000 11th Edition dataset

Unofficial, fan-maintained data for personal and community use. Warhammer 40,000 and all associated names, rules and points values are the intellectual property of Games Workshop. This dataset is not affiliated with, licensed by or endorsed by Games Workshop.

## Coverage in 1.1.0.0

- Genestealer Cults: all 25 native datasheets.
- Unit and weapon profiles, model compositions, abilities and points tiers.
- Duplicate-unit prices are represented where the first/second copies and third copy have different costs.

This version does not yet include detachments, Force Dispositions, enhancements, stratagems or allied Astra Militarum/Tyranid units.

## Sources

- IRONBUILT community dataset `wh40k-11e`, version `2026.11.21-0401-mfm16-tempered`, manifest updated 21 November 2026.
- Source bundle SHA-256: `e19b68b0860ec5a6074325ea22dbec5654da0953590429450306094fe7e6dd71`.
- Wahapedia Genestealer Cults 11th Edition datasheets, checked 23 November 2026.
- Games Workshop Warhammer Community faction packs and Munitorum Field Manual are the canonical authority for published changes and points.

Community datasets are used as structured authoring aids, not as unquestioned authorities. Values should be rechecked following every Games Workshop balance or faction update.

## Known limitations

- The WarOrgan builder validates schema and internal references; it cannot prove that game data matches the current rules.
- Some complex per-model wargear limits cannot be enforced perfectly by the current data model. Their available choices are represented, but users should consult the current faction rules when constructing unusual unit configurations.
- Stable unit IDs must not be renamed after release because saved army lists refer to them.
