# TODO

The work plan for the `DominicWatts-WH40K11e` dataset, written so an AI agent (or a person) can pick up any open task without other context. `FACTIONS.md` tracks each faction's progress through the sources; this file says what to do next and how.

## Before you start

- **Read first:** `FACTIONS.md` (source order and per-faction status), `CHANGELOG.md` (release history), the dataset's `WarOrgan.Data.BuildDataset/Assets/WH40K11e/README.md` (coverage and limitations), and the READMEs of `WarOrgan.Data.BsdataImport` and `WarOrgan.Data.WahapediaCheck`.
- **Never change** the dataset ID (`DominicWatts-WH40K11e`) or a published unit `Id`: saved army lists refer to both. Change `DisplayName` instead. Do not change `WarOrgan.Data.Model` without the owner's agreement; the app reads it.
- **Sources, in priority order:** the November 2026 BSData dump (`momper14/wh40k-11e` at `f015101d4c495173283e1b97e9d59f2befb877ad`, saved as `bsdata.zip` in the repository root, gitignored) is the base; Wahapedia's 11th Edition pages override it; 10th Edition codexes fill gaps; GW's 11th Edition faction packs apply corrections. Flag conflicts to the owner rather than overwriting checked data.
- **Branches:** origin is authoritative and shared branches are long-lived. Faction data goes on the existing `origin/faction/<slug>` branch (for example `faction/space-marines`); do not recreate it from develop or merge develop back into it. Work from the faction branch as-is; if a task appears to require changes that exist only on develop, ask the owner before changing the branch flow. Push the faction commit, then merge the faction branch into `dataset/develop` with `--no-ff` and leave its pointer on its own commit. Never fast-forward, rebase, reset, force-move or force-push shared branches. Dataset-wide content such as core stratagems or the keyword registry goes on `core/<topic>`; tooling and docs on `chore/<topic>`; cross-faction ally wiring on `allies/<owner>-<ally>`. Never commit directly on `dataset/develop` except the one "Prepare x.y.z" commit per release, and never on `dataset/wh40k11e`.
- **Releases:** the Prepare commit on `dataset/develop` bumps `defaultVersion` in `WarOrgan.Data.BuildDataset/Program.cs` (third field for data changes), the `## Coverage in` heading in the dataset README, adds one `CHANGELOG.md` line (`YYYY-MM-DD : x.y.z.0 - summary`, with a blank line between entries) and updates this file and `FACTIONS.md`. Build, upload the ZIP to the `develop-build` pre-release for the owner to test in WarOrgan, and only after they pass it merge `dataset/develop` into `dataset/wh40k11e`, tag `vX.Y.Z` and verify the public download.
- **Checks before every commit:** `dotnet run --project WarOrgan.Data.BuildDataset -c Release` must end with `0 errors`, and `git diff --check` must be clean. Commit messages are a single line.
- **Editing roster JSON:** keep UTF-8, two-space indentation and the file's existing line endings. Load and dump with a JSON library (`indent=2`, `ensure_ascii=False`) rather than hand-editing large files.
- **Wahapedia URLs:** `https://wahapedia.ru/wh40k11ed/factions/<slug>/datasheets.html`. Most slugs are the faction name in kebab case; exceptions are `imperial-agents` (Agents of the Imperium), `t-au-empire` and `emperor-s-children`. Space Marine chapter datasheets are on the `space-marines` page.

## Ready for an AI

Ordered by value. Each task lists where to work and when it is done.

### 1. Check factions against Wahapedia

Check and release each faction on its own `faction/<slug>` branch. Wahapedia wins over BSData. Fix names, statlines, weapons, keywords, abilities, composition, wargear options, points, Leader attachments, Support attachments and detachment differences; work through the importer report and preserve hand fixes.

Done for a faction when: the check report has only expected notes, `FACTIONS.md` marks Wahapedia layer 2 as done, and the dataset README moves it from Draft coverage to checked coverage.

**Priority**

- [ ] Tyranids — remaining draft datasheets and detachments, while keeping the checked Vanguard Invader allies
- [ ] Astra Militarum — remaining draft datasheets and detachments, while keeping the checked Brood Brother Auxilia allies
- [ ] Adepta Sororitas
- [ ] Adeptus Custodes
- [ ] Adeptus Mechanicus
- [ ] Agents of the Imperium
- [ ] Grey Knights
- [ ] Imperial Knights
- [ ] Space Marines
- [ ] Black Templars
- [ ] Blood Angels
- [ ] Dark Angels
- [ ] Deathwatch
- [ ] Imperial Fists
- [ ] Iron Hands
- [ ] Raven Guard
- [ ] Salamanders
- [ ] Space Wolves
- [ ] Ultramarines
- [ ] White Scars
- [ ] Chaos Space Marines
- [ ] Chaos Daemons
- [ ] Chaos Knights
- [ ] Death Guard
- [ ] Emperor's Children
- [ ] Thousand Sons
- [ ] World Eaters
- [ ] Aeldari
- [ ] Drukhari
- [ ] Leagues of Votann
- [ ] Necrons
- [ ] Orks
- [ ] T'au Empire

`FACTIONS.md` lists the faction branch and source-layer state for each item.

**Progress notes — 2026-11-24**

- **Tyranids** (`faction/tyranids`, `94da392`): corrected Harridan and Hierophant to Deadly Demise 2D6 and added the Harpooned keyword to the Norn Assimilator's Toxinjector Harpoon. The shared registry now accepts these abilities (`114c8e5`, merged as `4d8af3e`). The check reports 58/62 main-roster entries clean with 5 notes; Legends are 5/5 clean. Remaining notes are the Winged Tyranid Prime's two missing Warrior Leader options, empty starting wargear on spawned Mucolid Spores and Spore Mines, and a Synaptic Nexus wording difference. Checked Vanguard Invader ally records were not changed.
- **Astra Militarum** (`faction/astra-militarum`, `d26c9d4`, `f377e41`): added source-backed Orders to nine officers, added Sly Marbo's Loner rule, corrected Commissar Graves on Foot's keyword, removed the unsupported Tempestus Aquilons Signal Flares ability and corrected the Hot-shot long-las display name. The check reports 57/83 main-roster entries clean with 51 notes; Legends are 21/61 clean with 87 notes. The checked Brood Brother Auxilia ally records were not changed.
- **Astra Militarum follow-up** (`faction/astra-militarum`, `0eb17a3`, merged as `cd2606f`): corrected Bullgryn Bone 'ead's default equipment and swaps, added Commissar Graves' Aquiline Prow to starting wargear, corrected Gaunt's Ghosts' keywords and Ibram Gaunt's invulnerable save, and corrected the Enginseer's conditional Lone Operative and Grenadier Squad leader option. The check reports 59/83 main-roster entries clean with 44 notes; Legends remain 21/61 clean with 87 notes. Checked Brood Brother Auxilia datasheets remain unchanged; Astra Militarum is still in progress.
- **Adepta Sororitas** (`faction/adepta-sororitas`, `6f1705f`): corrected Arco-Flagellants' 10-model points, Insidiants/Paragons/Repentia composition names, the Daemonifuge weapon name, Dominion boltgun keywords and Sanctifiers' size, points and plasma BS. The check reports 12/41 main-roster entries clean with 106 notes; Legends are 0/5 clean with 8 notes.
- These are incremental corrections only; none of these factions meets the completion criteria above. Keep their Wahapedia status open and continue with the remaining draft differences.

### 2. Add stratagems for every faction except Genestealer Cults

BSData has no stratagems. Genestealer Cults and ten draft factions now have them; the remaining factions and chapter-specific detachments are still empty.

- **Branch:** `faction/<slug>`, or a `chore/` branch first if you extend a tool.
- **Do:** read each detachment's stratagems from the Wahapedia faction page (`https://wahapedia.ru/wh40k11ed/factions/<slug>/`; stratagems sit in `div.str11Wrap` blocks after each detachment's `h2`). `WahapediaPages.ParseFactionPage` in `WarOrgan.Data.WahapediaCheck` already parses them for comparison; reuse it to write `Stratagems` into each `Detachment`, following the shape of the Genestealer Cults roster (`Name`, `Category`, `CPCost`, `Phase`, `When`, `Target`, `Effect`, optional `Restrictions`). Skip Boarding Actions stratagems.
- **Done when:** every detachment in a faction has its stratagems, the check report shows no stratagem differences, and the build passes.
- **Completed so far:** Genestealer Cults (45), generic Space Marines (52, copied into chapter rosters), T'au Empire (31), Tyranids (51), Astra Militarum (57), Adepta Sororitas (36), Adeptus Custodes (23), Adeptus Mechanicus (45), Agents of the Imperium (30), Grey Knights (45) and Imperial Knights (39). Sanctified Orators and Haloscreed Battleclade have no Wahapedia stratagems.

### 3. Fill the detachments BSData has no enhancements for

Armoured Trailblazers, Brazen Engines, Butchers of Khorne, Contagion Engines, Flyblown Host, Hand of the Dynasty, Pantheon of Woe, Skyshroud Spearhead, Spectacle of Slaughter, Veiled Blade Elimination Force and Warptide have none in BSData. (Deathwatch Support is an allied option and should stay empty.)

- **Branch:** the owning `faction/<slug>`.
- **Do:** take each enhancement's name, points, restriction (`RequiredKeywords` and similar) and text from the Wahapedia faction page (`ul.EnhancementsPts`).
- **Done when:** each listed detachment has its enhancements or a confirmed note that it has none.

### 4. Turn simple rule effects into data

Enhancement, wargear, leader and detachment rules are text only in the drafts, so the app does not apply them. Many are simple stat changes the model can represent.

- **Branch:** `chore/` for an importer change, then `faction/<slug>` for the data.
- **Do:** recognise simple patterns such as "Add 1 to the bearer's Wounds characteristic", "+1 Strength to melee weapons" or "improve the Armour Penetration characteristic by 1", and emit `StatAdjustments` or `WeaponStatAdjustments` (`Scope: Model` for the bearer). Report anything conditional or unit-wide instead of guessing. Worked examples: the Vanguard Tyrant enhancement in `Rosters/Genestealer Cults.json` (S and AP) and the T'au shield drone in `Rosters/T'au Empire.json` (W, characters only: squads keep it as text because WarOrgan shows one statline per model type).
- **Done when:** the recognised effects apply in a test build, and FACTIONS.md notes which effects remain text.

### 5. Wire the remaining allies

Only Genestealer Cults (Tyranids, Astra Militarum) and the Space Marine chapters (Space Marines) have allies. Others are missing, for example Agents of the Imperium for Imperium armies, Chaos Daemons and Chaos Knights for Chaos armies, and Imperial Knights.

- **Branch:** `allies/<owner>-<ally>`, taken from `dataset/develop`.
- **Do:** add `Allies` entries in `DataSetDefinition.json` with the rule's points cap, allowed detachments and unit filters (`UnitIncludeRules`, with `AddedKeywords: ["Allied Units"]`). Prefer `AffectedUnitsFilter` with explicit unit IDs when keywords are unreliable; BSData keyword spellings vary (see the Brood Brother Auxilia entry).
- **Done when:** each ally appears only where its rule allows, under Allied Units, and the owner has app-tested it.

### 6. Correct the T'au drone option lists

BSData offers drones that Wahapedia does not allow, for example guardian drones on the Cadre Fireblade, Ethereal and Commanders. It also lists Stealth Battlesuits, but not the Hazard Battlesuits, as able to take shield drones. Fix this as part of task 2 for T'au, against each datasheet's wargear options on Wahapedia.

### 7. Add builder validation for duplicate weapon IDs within a unit

- **Branch:** `chore/validate-duplicate-weapons`.
- **Do:** in `WarOrgan.Data.BuildDataset/Validation/`, fail the build when a unit has two weapons with the same `Id` across its weapon sets.
- **Done when:** the check exists, the current dataset still builds, and a deliberately duplicated weapon fails.

## Needs the owner (app testing)

Only the owner can run these, in the WarOrgan app, using the development build URL. Record results here.

- [x] Shield drones: a Cadre Fireblade with one shield drone gains 1 W, with two gains 2 W (1.5.4). Reopen issue 4 if two do not stack.
- [x] Chapters (1.5.7): Ultramarines groups Space Marines datasheets under Characters, Battleline, Dedicated Transports and Other Datasheets, not "Allies: Adeptus Astartes" (issue 6). Passed.
- [ ] Draft factions (1.5.3): a Battle Sisters Squad and a Crusader Squad for their composition and wargear.
- [ ] Brood Brother Auxilia still offers exactly the 51 eligible Astra Militarum datasheets after the 1.5.2 fill.
- [ ] Command Squads (1.4.3): Cadian, Catachan and Krieg Command Squads appear only with Brood Brother Auxilia, under Allied Units, and cannot be Warlord; their specialists, equipment and weapon swaps are right; each can lead its listed unit and shows Orders.
- [ ] Leader effects: Might From Beyond, Twisted Science, Bio-horror Disruption, Sudden Assault and Nexus of Devotion apply to the led unit, and a Support character can join alongside a Leader.
- [ ] Final Day costs 2 DP, shows Purge the Foe and its six stratagems, and its rule text reads correctly (1.2.2).
- [ ] Host of Ascension costs 3 DP, offers Take and Hold and Reconnaissance, and cannot be taken with Xenocult Masses (1.3.5).
- [ ] The rebuilt Neophyte Hybrids, Acolyte Hybrids, Hybrid Metamorphs and Atalan Jackals wargear limits (1.1.2).

## Blocked on the app

These need the WarOrgan author's guidance or an app change; do not publish speculative fixes.

- [ ] **Patriarch must be Warlord:** `Flags: 2` (`MustBeWarlord`) is published, but WarOrgan does not enforce it. The app does read unit flags (`NotWarlord` works), so it probably does not validate this as a list rule.
- [ ] **Unit limits per battle size:** `UnitLimit` is 0 for every battle size until the app's handling of the Battleline and Dedicated Transport limits is known.
- [ ] **Bold in notes:** Detachment rules use 85 `**Heading**` lines to introduce a second rule. If the app shows the asterisks, switch them to plain headings.

## Done

- **Genestealer Cults:** all 25 datasheets checked line by line against Wahapedia and the faction pack; army rule; nine detachments with 30 enhancements and 45 stratagems; unit wargear limits rebuilt; leader effects; invulnerable saves as an INV characteristic (1.1 to 1.4).
- **Allies:** Tyranid Vanguard Invaders for Final Day (1.3); 51 Astra Militarum datasheets for Brood Brother Auxilia, pinned by ID (1.4, 1.5.2).
- **Core:** ten 11th Edition core stratagems (1.3.9); battle sizes and features; the 11th Edition weapon keyword and core ability registry (1.5.0).
- **Tools:** `WarOrgan.Data.BsdataImport` and `WarOrgan.Data.WahapediaCheck` (1.4.x); importer support for libraries, per-army detachments, enhancements and a repair pass (1.5.x); Wahapedia stratagem import support (1.5.8); committed bulk-draft pipeline and faction manifest (1.5.9).
- **Draft factions:** 31 factions drafted (1.5.0), enhancements added (1.5.1), Tyranids and Astra Militarum filled out around the checked allies (1.5.2), held-back datasheets fixed or retired (1.5.3).
- **Fixes from issues:** Vanguard Tyrant S and AP (issue 1), single-choice weapon swaps (issue 2), INV characteristic (issue 3), shield drone Wounds on characters (issue 4), stray bold markers removed (issue 5).
- **App confirmations:** detachments, Force Dispositions, stratagems, enhancements and their weapon changes, Upgrades, allies under Allied Units with the 50% cap, wargear stat changes (Ridgerunner spotter), leader-added core abilities, and Scouts, Deadly Demise and Feel No Pain values all work in WarOrgan. Pre-release testing works through the `develop-build` URL.
