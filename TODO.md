# TODO

Outstanding work for the `DominicWatts-WH40K11e` dataset. Tick items off in the same commit as the change.

## 1.1.2.0 release

- [x] Rebuild Neophyte Hybrids unit size and wargear limits
- [x] Rebuild Acolyte Hybrids with Autopistols unit size and wargear limits
- [x] Rebuild Hybrid Metamorphs unit size and wargear limits
- [x] Rebuild Atalan Jackals unit size and wargear limits
- [x] Merge into `dataset/wh40k11e`, tag `v1.1.2` and verify the release ZIP
- [ ] Test the rebuilt units in WarOrgan

## Genestealer Cults datasheet checks

Compare each datasheet line by line with Wahapedia: statline, weapons, abilities, unit composition, wargear options, points, keywords, Led By and Supported By. Release in small batches of about five units so each app test stays short.

- [x] Aberrants
- [x] Abominant
- [x] Achilles Ridgerunners
- [x] Acolyte Hybrids with Autopistols
- [x] Acolyte Hybrids with Hand Flamers
- [x] Acolyte Iconward
- [x] Atalan Jackals
- [x] Benefictus
- [x] Biophagus
- [x] Clamavus
- [x] Goliath Rockgrinder
- [x] Goliath Truck
- [x] Hybrid Metamorphs
- [x] Jackal Alphus
- [x] Kelermorph
- [x] Locus
- [x] Magus
- [x] Neophyte Hybrids
- [x] Nexos
- [ ] Patriarch
- [ ] Primus
- [ ] Purestrain Genestealers
- [ ] Reductus Saboteur
- [ ] Sanctus
- [ ] Tectonic Fragdrill

## Genestealer Cults rules content

- [ ] Army rule (Cult Ambush)
- [ ] Nine detachments and their Force Dispositions
- [ ] Enhancements
- [ ] Stratagems
- [ ] Enable the matching dataset `Features` and battle-size limits
- [ ] Brood Brothers and Tyranid allies with detachment inclusion rules
- [ ] Add the Allied Units category with the first allied roster

## Blocked or needs app confirmation

- [ ] Patriarch must be Warlord: `Flags: 2` is published but WarOrgan does not enforce it. Needs the app author's guidance or an app test with a flag-reading experiment.
- [ ] Leader effects: confirm in WarOrgan that Might From Beyond, Twisted Science, Bio-horror Disruption, Sudden Assault and Nexus of Devotion apply to the led unit, and that a Support character can join a unit alongside a Leader.
- [ ] Find out whether WarOrgan can import a ZIP from a local file or a non-stable URL for pre-release testing.

## Tooling

- [ ] Add repeatable source-comparison checks for names, model counts and points
- [ ] Add builder validation for duplicate weapon IDs within a unit

## Later

- [ ] Import and verify the remaining 11th Edition factions, one per `faction/<slug>` branch
