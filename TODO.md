# TODO

See [FACTIONS.md](FACTIONS.md) for coverage and [CHANGELOG.md](CHANGELOG.md) for release history.

## Sources

**BSData → ListForge → GW → Faction packs.** Use BSData as the baseline, apply later sources where they cover the same 11th Edition records, and carry forward existing fixes and records without a replacement. Flag ambiguous matches and conflicts with checked data. Routine online audits are optional.

## Workflow

- Follow [HANDOVER.md](HANDOVER.md). Fetch first and use the existing `faction/<slug>` branch. Build with zero errors, run `git diff --check`, push the faction commit, then merge it into `dataset/develop` with `--no-ff`.
- Use `chore/<topic>` for tools and docs, `core/<topic>` for shared dataset rules, and `allies/<owner>-<ally>` for cross-faction links. Never back-merge develop into faction branches or rewrite shared refs.
- Preserve the dataset ID and published unit IDs. Ask before changing the data model or checked Genestealer Cults and ally records. Keep UTF-8, two-space indentation and existing line endings.
- Direct commits to `dataset/develop` are reserved for the single Prepare commit per test version. Update the version, coverage, changelog and tracker, then upload and verify `develop-build`. Publish only after owner approval.

## Next

1. Batch missing stratagems and enhancements on the existing faction branches. Skip Boarding Actions and preserve intentional empty entries.
2. Fill missing base and Legends rosters, army rules, detachment rules, dispositions, points, paid wargear and attachment links. Preserve published IDs and hand fixes. Keep unsupported effects as rule text.
   - Start with the 12 candidate roster gaps and two classification conflicts in [FACTIONS.md](FACTIONS.md#status-notes). Reconcile the provisional totals against ListForge and faction packs as those inputs become available.
3. Add remaining allies with explicit eligibility and points caps where needed. Cover Agents of the Imperium, Imperial Knights, Chaos Daemons and Chaos Knights.
4. Apply faction-pack corrections and record the pack versions. Genestealer Cults 1.1 is already applied.

## Progress

- Genestealer Cults and Tyranids are signed off.
- **1.5.15.0** adds 153 stratagems, including Blade of Ultramar, on top of the 1.5.14 batch. Three empty lists remain without matching entries: Sanctified Orators, Brute Bosses and Wurrband.
- **1.5.16.0** shipped the plain-text export fix. **1.5.17.0** fills the seven empty enhancement lists. Deathwatch Support remains intentionally empty.
- **1.5.18.0** wires Daemon allies for World Eaters, Death Guard, Thousand Sons and Emperor’s Children.
- **1.5.20.0** wires attach targets for 43 Support characters across 12 factions (issue 10). 57 Leaders, mostly Space Marine characters, still have empty attach lists.
- **1.5.19.0** adds the Traitor Enforcer and 21 Shadow Legion allies. Reaper’s Wager still needs eight Harlequin datasheets.
- Astra Militarum includes Hell’s Last and Vendetta Gunship. The Brood Brother Auxilia list still contains 51 IDs. App checks below remain open.
- Sororitas and T’au have roster and wargear fixes on develop, including the latest weapon surcharges. Both remain in progress.
- Marine army-rule display passed in 1.5.12. Chapter stratagems are in place and await WarOrgan display testing.

## Needs the owner (app testing)

- [ ] Support attachments (1.5.20, issue 10): a Master of Executions joins Chosen or Legionaries, alongside a Leader such as a Chaos Lord. Spot-check a Lieutenant on Intercessors, a Painboy on Boyz and a Warlock on Guardian Defenders.
- [ ] Shadow Legion (1.5.19): confirm the 21 allies appear only in this detachment, enforce the half-army points cap, and can take eligible enhancements. In Chaos Space Marines, verify that the 70-point Traitor Enforcer leads Traitor Guardsmen.
- [ ] Daemon allies (1.5.18): Khorne Daemonkin and Carnival of Excess show five eligible units each. Tallyband Summoners and Changehost of Deceit show six each. Verify detachment gating, the half-army points cap, and that allies can’t be Warlord or take enhancements.
- [ ] Enhancements (1.5.17): confirm each new Upgrade appears on eligible units only. Check both Lancet of the Worldsore recipients and all four Parasitic Woe-reaper recipients.
- [ ] Ultramarines: Blade of Ultramar’s six stratagems displayed in 1.5.15. Recheck plain text on 1.5.16 or later. Issue 7 is closed.
- [ ] Astra Militarum (1.5.10): both 110-point Grenadier lineups and rejection of mixed invalid counts.
- [ ] Astra Militarum (1.5.10): Tempestus special-weapon limits and both medi-pack choices.
- [x] Astra Militarum (1.5.10): Ratling Battlemutt and Demolition Gear on a ten-model unit only. Passed.
- [ ] Astra Militarum (1.5.10): Hell’s Last and Vendetta Gunship selectable with Legends enabled.

Only the owner can run these, in the WarOrgan app, using the development build URL. Record results here.

- [x] Tyranids (1.5.10): standalone roster and Legends app checks passed.
- [x] Army rules (1.5.12, issue 9): Ultramarines shows Combat Doctrines, Transhuman Strategist, Librarius and Special Move Types; Dark Angels also show The Ravenwing and The Deathwing; Aeldari show Battle Focus with its token table readable. Passed.
- [ ] Astra Militarum (1.5.11, issue 8): a Cadian Shock Troops unit takes two Leaders but not two Command Squads.
- [x] Astra Militarum (1.5.11, issue 8): a Leman Russ Battle Tank costs 165 with its lascannon and 175 with two multi-meltas. Passed.
- [ ] Astra Militarum (1.5.11, issue 8): a Leman Russ Commander with a demolisher battle cannon costs 15 more.
- [ ] Astra Militarum (1.5.11, issue 8): a Recon Squad takes two long-las.
- [x] Astra Militarum (1.5.11, issue 8): all six Bullgryns can swap wargear. Passed.
- [ ] Astra Militarum (1.5.11, issue 8): an Ogryn Bodyguard and Nork Deddog can each join a Command Squad, one per squad.
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
