# WarOrgan.Data Fork

An unofficial Warhammer 40,000 11th Edition dataset for WarOrgan, for personal and community use. It is a fork of [WarOrgan.Data](https://github.com/zenchovey/WarOrgan.Data); the shared WarOrgan model is unchanged, so the dataset works in the standard app. The fork adds stricter validation to the builder, plus two tools for producing and checking the data.

## Installing the dataset

Paste this URL into WarOrgan's dataset importer:

`https://github.com/DominicWatts/WarOrgan.Data/releases/latest/download/DominicWatts-WH40K11e.zip`

It always points at the latest release. Import the ZIP directly; do not extract it. The dataset ID, `DominicWatts-WH40K11e`, never changes, so saved army lists keep working across updates. To install an older version, use the ZIP attached to that release on [GitHub Releases](https://github.com/DominicWatts/WarOrgan.Data/releases).

To try unreleased changes, import the development build instead: `https://github.com/DominicWatts/WarOrgan.Data/releases/download/develop-build/DominicWatts-WH40K11e.zip`. It replaces the released dataset in the app, so re-import the release URL afterwards to go back.

## What it covers

All 34 armies, with each Space Marine chapter as its own faction, plus the 11th Edition core stratagems.

- **Checked:** Genestealer Cults in full, the Tyranid Vanguard Invaders that Genestealer Cults can take through Final Day, and the Astra Militarum datasheets they can take through Brood Brother Auxilia. These have been checked line by line against Wahapedia and GW's faction packs.
- **Unchecked drafts:** every other datasheet, detachment and enhancement, converted automatically from BSData. They have no stratagems yet, rule effects are text only, and some wargear options are looser than the datasheet. Build units to the datasheet.

The dataset's [README](WarOrgan.Data.BuildDataset/Assets/WH40K11e/README.md) lists coverage, sources and known limitations in detail. [FACTIONS.md](FACTIONS.md) tracks each faction's progress.

## How the data is made

Each faction is built up from four sources, in this order:

1. **BSData** (the November 2026 dump) provides the base data. `WarOrgan.Data.BsdataImport` converts it into a draft roster.
2. **List Forge**'s BSData export, a newer copy of the same catalogues, replaces the base where they differ.
3. **Wahapedia** overrides both and adds stratagems. `WarOrgan.Data.WahapediaCheck` lists every difference.
4. **GW's 11th Edition faction packs** apply the latest corrections.

Codexes are not used as a source.

Each faction is developed on its own `faction/<name>` branch. Work is merged into `dataset/develop` for testing, and releases are merged into `dataset/wh40k11e` and tagged.

- Release history: [CHANGELOG.md](CHANGELOG.md).
- Outstanding work: [GitHub issues](https://github.com/DominicWatts/WarOrgan.Data/issues).

## What's included

- **WarOrgan.Data.Model** — the data model shared with the WarOrgan app.
- **WarOrgan.Data.BuildDataset** — loads, validates and packages the dataset.
- **WarOrgan.Data.BsdataImport** — drafts a faction roster from a BSData dump. See its [README](WarOrgan.Data.BsdataImport/README.md).
- **WarOrgan.Data.WahapediaCheck** — compares rosters with Wahapedia and reports every difference. See its [README](WarOrgan.Data.WahapediaCheck/README.md).

## Licence

The model, build tools and project documentation are available under the [MIT License](LICENSE). They are provided as is, without warranty, with the liability disclaimer set out in the licence.
