# Faction progress

Source order: **BSData → ListForge → GW → Faction packs**. Later sources take precedence for applicable records. Keep existing fixes and data a source doesn’t cover.

Roster counts are provisional matched/expected native datasheets as of 25 November 2026. The inventory combines shipped entries, the cached BSData inventory and GW coverage. It excludes inherited units, Combat Patrol and known retirements, and retains existing entries and classifications where sources differ. ListForge and faction packs have not been reconciled in this pass. `0/0` means no native datasheets were identified for that tier. Percentages measure inventory coverage. [WarOrgan tests](https://github.com/DominicWatts/WarOrgan.Data/issues?q=is%3Aopen+label%3Aneeds-testing) are separate.
Issue counts are closed/total faction-specific GitHub issues.

| Faction | Rosters Base | Rosters Legends | Army Rules | Detachments | Disposition | Enhancements | Stratagems | Issues pass / total |
|---|---:|---:|---|---|---|---|---|---:|
| Genestealer Cults | 24/24 (100%) | 1/1 (100%) | Complete | Complete | Complete | Complete | Complete | 2/2 (100%) |
| Tyranids | 52/52 (100%) | 5/5 (100%) | Complete | Complete | Complete | Complete | Complete | — |
| Astra Militarum | 72/72 (100%) | 63/63 (100%) | Complete | Complete | Complete | Complete | Complete | 2/2 (100%) |
| Adepta Sororitas | 33/33 (100%) | 5/5 (100%) | Complete | Incomplete | Incomplete | Incomplete | Incomplete | — |
| Adeptus Custodes | 29/29 (100%) | 7/7 (100%) | Complete | Complete | Complete | Complete | Complete | 1/3 (33%) |
| Adeptus Mechanicus | 34/34 (100%) | 3/3 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Agents of the Imperium | 29/29 (100%) | 11/11 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Grey Knights | 26/26 (100%) | 5/5 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Imperial Knights | 22/22 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Space Marines | 70/71 (99%) | 64/72 (89%) | Complete | Incomplete | Incomplete | Incomplete | Complete | — |
| Black Templars | 16/16 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Blood Angels | 14/14 (100%) | 8/8 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Dark Angels | 16/16 (100%) | 2/2 (100%) | Complete | Incomplete | Incomplete | Incomplete | Complete | — |
| Deathwatch | 10/10 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Imperial Fists | 3/3 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Iron Hands | 2/2 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Raven Guard | 2/2 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Salamanders | 2/2 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Space Wolves | 20/20 (100%) | 14/14 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Ultramarines | 11/11 (100%) | 5/5 (100%) | Complete | Incomplete | Incomplete | Incomplete | Complete | 3/3 (100%) |
| White Scars | 2/2 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Chaos Space Marines | 57/58 (98%) | 26/26 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Chaos Daemons | 56/56 (100%) | 12/12 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Chaos Knights | 19/20 (95%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Death Guard | 30/30 (100%) | 11/11 (100%) | Incomplete | Incomplete | Incomplete | Complete | Complete | — |
| Emperor's Children | 18/18 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Complete | Complete | — |
| Thousand Sons | 29/29 (100%) | 2/2 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| World Eaters | 25/25 (100%) | 2/2 (100%) | Incomplete | Incomplete | Incomplete | Complete | Complete | — |
| Aeldari | 57/57 (100%) | 21/21 (100%) | Complete | Incomplete | Incomplete | Incomplete | Complete | — |
| Drukhari | 23/23 (100%) | 7/7 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | — |
| Leagues of Votann | 22/22 (100%) | 0/0 (n/a) | Incomplete | Incomplete | Incomplete | Complete | Complete | — |
| Necrons | 52/52 (100%) | 12/12 (100%) | Incomplete | Incomplete | Incomplete | Complete | Complete | — |
| Orks | 55/55 (100%) | 15/15 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Incomplete | — |
| T'au Empire | 43/43 (100%) | 20/20 (100%) | Incomplete | Incomplete | Incomplete | Incomplete | Complete | 1/1 (100%) |

## Status notes

- Candidate gaps (11): Space Marines — Drop Pod (Base), plus Cerberus, Falchion, Kratos, Mastodon, Rapier Carrier, Sicaran, Terrax-pattern Termite and Typhon (Legends). Chaos Space Marines — Fabius Bile (Base). Chaos Knights — Chaos Questoris Knight Magaera (Base).
- Space Marines: 1.5.27 fixes Sergeant and regular model composition limits across the roster and adds the correct 5-model and 10-model Assault Intercessor point tiers (issue 272).
- Space Marines: 1.5.25 adds the Captain in Gravis Armour, which the inventory had missed, and fixes Infernus and Intercessor squad sizes and points (issue 66).
- 1.5.24 removes stray inline bold markers from rule text on every faction branch and at build time (issue 65).
- Adepta Sororitas: army rules audited against the November BSData dump and GW data; no source-supported army-rule correction was required (issue 68).
- Adepta Sororitas: Codex errata dated 14 October 2026 supersedes a separate faction-pack pass; clear errata corrections are already present, with no speculative roster change (issue 73).
- Adeptus Custodes: 1.5.22 adds the Anathema Psykana datasheets and Aleya (app checks in issue 63). 1.5.23 completes issue 16: 13 Codex detachments, army rules, GW points and Praesidium Shield wargear. Faction Pack 1.1 (22 July 2026) is skipped because the Codex errata of 14 October 2026 in the GW app data replaces it. The Shield Host limit and Upgrade enhancements are tracked in issue 64.
- Classification review: Captain on Bike and Wolf Scouts remain in their shipped Legends rosters. Harlequins are tracked separately as an eight-datasheet prerequisite for Reaper’s Wager.
- 1.5.20 wires attach targets for 43 Support characters (issue 10).
- 1.5.21 fills attach targets for 57 Leaders that had none (issue 11). The five Death Guard specialists join Plague Marines as Support, since they can join a unit that already has a Leader.
- 1.5.19 adds the Traitor Enforcer and 21 Shadow Legion allies. Reaper’s Wager still needs eight Harlequin datasheets.
- 1.5.18 adds detachment-specific Daemon allies for World Eaters, Death Guard, Thousand Sons and Emperor’s Children. WarOrgan checks are pending.
- 1.5.14 adds 572 stratagems and 12 enhancements. 1.5.15 adds 153 stratagems across Aeldari, Orks and Space Marine chapters. Empty lists remain for Sanctified Orators, Brute Bosses and Wurrband.
- Genestealer Cults: signed off. Faction Pack 1.1 applied in 1.5.13.
- Tyranids: roster and WarOrgan smoke checks passed. Faction Pack 1.3 coverage is partial.
- Astra Militarum: 51 Brood Brother Auxilia allies retained. App tests passed (issue 57).
- Ultramarines: Blade of Ultramar’s six stratagems displayed in WarOrgan. Version 1.5.16 removes raw formatting tags. Issue 7 is closed.
- 1.5.17 fills seven enhancement lists. Three stratagem lists remain empty without matching entries.

- T'au Empire: Faction Pack 1.2 (26 August 2026) reviewed. Added Ethereal's For the Greater Good faction ability and the Stealth/FRAME keyword corrections.
