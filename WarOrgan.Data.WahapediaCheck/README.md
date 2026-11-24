# Wahapedia check

Compares WarOrgan rosters with Wahapedia's 11th Edition pages: step 3 of the source order in `FACTIONS.md`. It writes one Markdown report per roster, listing every difference it finds and which datasheets and detachments matched with none.

```bash
dotnet run --project WarOrgan.Data.WahapediaCheck -- --faction-slug genestealer-cults \
  --roster "WarOrgan.Data.BuildDataset/Assets/WH40K11e/Rosters/Genestealer Cults.json" \
  --roster "WarOrgan.Data.BuildDataset/Assets/WH40K11e/Rosters/Genestealer Cults Legends.json" \
  --out /tmp/wahapedia-check
```

`--faction-slug` is the Wahapedia URL name (`genestealer-cults`, `tyranids`, `astra-militarum`). A faction's datasheet page also lists the allied datasheets it can take, so the GSC page checks the Brood Brothers and Tyranid allies too. Draft rosters from `WarOrgan.Data.BsdataImport` can be checked before they are copied into the dataset.

Pages are downloaded once a day into a cache (system temp by default; `--cache` to choose, `--refresh` to download again). `--datasheets` and `--faction-page` accept local files. `--write-stratagems` adds missing stratagems from Wahapedia to matching detachment names in the roster and updates their category.

## What it compares

- Statlines, base sizes and invulnerable saves.
- Weapon profiles and keywords.
- Core abilities and abilities, including word-level text differences (designer's notes are ignored).
- Model counts, unit size choices and starting wargear.
- Points for each unit size and unit copy.
- Leader and Support attachments, keywords and faction keywords.
- Detachments: DP cost, rules, enhancements (cost and text) and stratagems (CP, When, Target, Effect, Restrictions).
- Datasheets and detachments on Wahapedia that the roster does not have yet.

## Reading the report

Wahapedia is the source of truth, but not every note is a fault. Expected notes include:

- Deliberate dataset choices, such as Hybrid Metamorphs' bio-weapons, which follow Wahapedia's weapon table rather than its equipment line.
- Wahapedia typos, which appear as text differences.
- Core ability values the dataset leaves off, such as `Scouts 9"` held as `Scouts`.

Wargear options are not compared; check them by hand against the Wahapedia datasheet.
