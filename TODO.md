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
- [x] Nine detachments and their Force Dispositions (all nine, 1.4.0)
- [x] Core stratagems (Command Re-roll, Fire Overwatch and so on) in `DataSetDefinition.json` (1.3.9)
- [ ] Unit limits per battle size, once the app's handling of the Battleline and Dedicated Transport doubling is known
- [ ] Enhancements
- [ ] Stratagems
- [x] Enable the matching dataset `Features` and battle-size limits
- [x] Tyranid allies for Final Day: all 14 Vanguard Invader datasheets added (1.3.0 to 1.3.3)
- [x] Show invulnerable save values on GSC units (1.3.4)
- [ ] Astra Militarum allies for Brood Brother Auxilia: pilot Cadian Shock Troops in 1.4.0, then the other 50 current datasheets (Legends skipped)
- [x] 1.4.0: confirmed Cadian Shock Troops only appear with Brood Brother Auxilia, sit under Allied Units, cannot be Warlord, count toward the 50% cap, and that their weapon options work.
- [x] Add the Allied Units category with the first allied roster

## Blocked or needs app confirmation

- [ ] Patriarch must be Warlord: `Flags: 2` is published but WarOrgan does not enforce it. The 1.3.0 test showed the app does read unit flags (`NotWarlord` blocked the Winged Tyranid Prime), so `MustBeWarlord` is probably read but not validated as a list rule. Needs the app author's guidance.
- [x] Heroes of the Uprising: confirmed in WarOrgan (1.2.0) that the detachment, its stratagems and enhancements appear, KILLER shows on the four units, and the enhancements change weapon profiles.
- [x] 1.2.1: confirmed in WarOrgan that Force Dispositions can be chosen, several 1 DP detachments fit within 3 DP, the PURESTRAIN and HOSTS tags show, and Upgrades go on non-CHARACTER units and change profiles. The app does not enforce the three-copies limit for Upgrades.
- [ ] 1.2.2: confirm Final Day costs 2 DP, shows Purge the Foe and its six stratagems, and its rule text (with bold sub-headings and the battle-size list) reads correctly.
- [x] 1.3.0: confirmed in WarOrgan that the Winged Tyranid Prime only appears in a GSC list once Final Day is chosen, sits under Allied Units, counts toward the 50% cap, cannot be Warlord, and can take Synaptic Auger. Also checked the new Tyranids faction on its own. The 50% ally cap could not be clearly confirmed.
- [x] 1.3.1 and 1.3.2: confirmed in WarOrgan that the Tyranid allies appear under Allied Units with Final Day (including Battleline Gargoyles and the Dedicated Transport Tyrannocyte) and that the 50% ally cap applies.
- [ ] 1.3.5: confirm Host of Ascension costs 3 DP, offers Take and Hold and Reconnaissance, and cannot be taken with Xenocult Masses (HOSTS tag).
- [x] 1.3.8: Outlander Claw +1 OC and the 2 DP detachment limit confirmed in WarOrgan.
- [x] Core abilities added by data: confirmed in WarOrgan that leader effects (Locus Fights First) and the Xenocreed detachment-added leader ability (Feel No Pain 3+ while leading) work.
- [x] Predatory Instincts (Biosanctic Broodsurge enhancement): confirmed the bearer's attached unit gains Infiltrators with AttachedUnit scope.
- [x] 1.3.12: confirmed wargear labels (Magus stave, Demolition charges) and leader sections without duplicated text.
- [x] Core stratagems confirmed in WarOrgan (1.3.9).
- [x] 1.3.10: confirmed the Ridgerunner spotter sets ranged BS to 3+ and the flare launcher adds SMOKE.
- [ ] Leader effects: confirm in WarOrgan that Might From Beyond, Twisted Science, Bio-horror Disruption, Sudden Assault and Nexus of Devotion apply to the led unit, and that a Support character can join a unit alongside a Leader.
- [ ] Find out whether WarOrgan can import a ZIP from a local file or a non-stable URL for pre-release testing.

## Tooling

- [ ] Add repeatable source-comparison checks for names, model counts and points
- [ ] Add builder validation for duplicate weapon IDs within a unit

## Later

- [ ] Import and verify the remaining 11th Edition factions, one per `faction/<slug>` branch
