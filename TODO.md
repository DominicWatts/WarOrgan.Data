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
- [x] Patriarch
- [x] Primus
- [x] Purestrain Genestealers
- [x] Reductus Saboteur
- [x] Sanctus
- [x] Tectonic Fragdrill

## Genestealer Cults rules content

- [x] Army rule (Cult Ambush)
- [ ] Nine detachments and their Force Dispositions (done: Heroes of the Uprising, Purestrain Broodswarm, Xenocult Masses, Final Day)
- [ ] Core stratagems (Command Re-roll, Fire Overwatch and so on) in `DataSetDefinition.json`
- [ ] Unit limits per battle size, once the app's handling of the Battleline and Dedicated Transport doubling is known
- [ ] Enhancements
- [ ] Stratagems
- [x] Enable the matching dataset `Features` and battle-size limits
- [ ] Tyranid allies for Final Day: done Winged Tyranid Prime, Gargoyles, Raveners, Hyperadapted Raveners, Lictor, Neurolictor, Von Ryan's Leapers, Mawloc, Trygon, Tyrannocyte; to add Winged Hive Tyrant, Deathleaper, The Red Terror, Parasite of Mortrex
- [ ] Show invulnerable save values on GSC units (only the 'Invulnerable Save' core ability is listed, not 4+ or 5+); the Neurolictor records its value as an ability entry
- [ ] Brood Brothers (Astra Militarum) allies for Brood Brother Auxilia
- [x] Add the Allied Units category with the first allied roster

## Blocked or needs app confirmation

- [ ] Patriarch must be Warlord: `Flags: 2` is published but WarOrgan does not enforce it. The 1.3.0 test showed the app does read unit flags (`NotWarlord` blocked the Winged Tyranid Prime), so `MustBeWarlord` is probably read but not validated as a list rule. Needs the app author's guidance.
- [x] Heroes of the Uprising: confirmed in WarOrgan (1.2.0) that the detachment, its stratagems and enhancements appear, KILLER shows on the four units, and the enhancements change weapon profiles.
- [x] 1.2.1: confirmed in WarOrgan that Force Dispositions can be chosen, several 1 DP detachments fit within 3 DP, the PURESTRAIN and HOSTS tags show, and Upgrades go on non-CHARACTER units and change profiles. The app does not enforce the three-copies limit for Upgrades.
- [ ] 1.2.2: confirm Final Day costs 2 DP, shows Purge the Foe and its six stratagems, and its rule text (with bold sub-headings and the battle-size list) reads correctly.
- [x] 1.3.0: confirmed in WarOrgan that the Winged Tyranid Prime only appears in a GSC list once Final Day is chosen, sits under Allied Units, counts toward the 50% cap, cannot be Warlord, and can take Synaptic Auger. Also checked the new Tyranids faction on its own. The 50% ally cap could not be clearly confirmed.
- [ ] Leader effects: confirm in WarOrgan that Might From Beyond, Twisted Science, Bio-horror Disruption, Sudden Assault and Nexus of Devotion apply to the led unit, and that a Support character can join a unit alongside a Leader.
- [ ] Find out whether WarOrgan can import a ZIP from a local file or a non-stable URL for pre-release testing.

## Tooling

- [ ] Add repeatable source-comparison checks for names, model counts and points
- [ ] Add builder validation for duplicate weapon IDs within a unit

## Later

- [ ] Import and verify the remaining 11th Edition factions, one per `faction/<slug>` branch
