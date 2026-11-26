# Handover

Unofficial WarOrgan dataset for Warhammer 40,000 11th Edition. Repository `DominicWatts/WarOrgan.Data`, dataset ID `DominicWatts-WH40K11e`.

Work is tracked as [GitHub issues](https://github.com/DominicWatts/WarOrgan.Data/issues). This file holds the rules and the day-to-day flow. `FACTIONS.md` has per-faction status and `CHANGELOG.md` the release history.

## Issues

| Label | Meaning | Who |
|---|---|---|
| `task` | Dataset or tooling work, usually one faction or one ally link | Agents |
| `in-progress` | Claimed by an agent; leave it alone | Agents |
| `needs-testing` | Checks the owner runs in the WarOrgan app | Owner |
| `blocked` | Waiting on the WarOrgan author or an owner decision; don't publish speculative fixes | Owner |
| `army-rules`, `detachments`, `roster`, `leader-support`, `faction-pack` | The task's dataset scope; a task may have more than one | Agents |
| `faction:<slug>` | The faction the task covers, for example `faction:orks`; its group (below) gives the branch | Agents |
| no label | A new bug report from the owner; fix it like a `task` | Agents |

To work on an issue:

1. Pick one open `task` (or unlabelled bug) without `in-progress`. One issue per session. For a faction task, first check that no other issue for any faction in the same group (see [Faction groups](#faction-groups)) has `in-progress`; only one agent may write to a group branch at a time.
2. Before changing anything, add `in-progress` and comment the branch you'll use: `gh issue edit <n> --add-label in-progress` and `gh issue comment <n> --body "Working on faction/<group>"`.
3. Follow the core flow below up to step 4 (merge into `dataset/develop`). Tick the issue's checkboxes as you go.
4. Comment the commit, the merge, and what the owner should test, then remove `in-progress`. Leave the issue open; the owner closes it when it's tested.
5. If you can't finish, comment what's done and what's left, and remove `in-progress`.

Faction work is split into standalone, independently claimable issues rather than parent trackers. Every faction task has `task`, one `faction:<slug>` label and at least one scope label. When opening follow-up work, apply all three kinds of label. Keep the issue small enough for one session; detachment audits should normally cover no more than five detachments. Do not widen an issue when the audit finds unrelated work.

`FACTIONS.md` is the central faction-status ledger. Maintain it on `dataset/develop` and carry the published copy on `dataset/wh40k11e` through the release flow. Faction branches must not add or edit it; remove any remaining copy from a faction branch.

The Prepare commit, the `develop-build` upload and releases (steps 5 to 8) are run by one session only, when the owner asks for a test version. That session bundles everything already merged and adds a `needs-testing` issue for the new checks.

If you find new work outside your issue, open a new `task` issue for it rather than widening your change.

## Faction groups

Faction work happens on four long-lived group branches, one per group. Issues keep their per-faction `faction:<slug>` label; the group decides the branch.

| Branch | Factions |
|---|---|
| `faction/imperium` | Adepta Sororitas, Adeptus Custodes, Adeptus Mechanicus, Agents of the Imperium, Astra Militarum, Grey Knights, Imperial Knights |
| `faction/space-marines` | Space Marines, Black Templars, Blood Angels, Dark Angels, Deathwatch, Imperial Fists, Iron Hands, Raven Guard, Salamanders, Space Wolves, Ultramarines, White Scars |
| `faction/chaos` | Chaos Space Marines, Chaos Daemons, Chaos Knights, Death Guard, Emperor's Children, Thousand Sons, World Eaters |
| `faction/xenos` | Aeldari, Drukhari, Genestealer Cults, Leagues of Votann, Necrons, Orks, T'au Empire, Tyranids |

- The old per-faction branches (`faction/orks`, `faction/necrons` and so on) were merged and deleted. Do not recreate them; use the group branch.
- A new faction joins the group that matches its grand alliance; ask the owner if unsure.
- `faction/chaos`, `faction/imperium` and `faction/xenos` were started from develop merge commit `3b6dcda`. That starting point is accepted as-is; from here on the normal rules apply and they must never be moved again. They still carry `FACTIONS.md` from that start, so remove it in the first commit on each.

## Rules

- **Source order: BSData → ListForge → GW → Faction packs.** Use BSData as the baseline, apply later sources where they cover the same 11th Edition records, and carry forward existing fixes and records without a replacement. Flag ambiguous matches and conflicts with checked data. Routine online audits are optional.
- Preserve the dataset ID and published unit IDs. Ask before changing the data model or checked data: Genestealer Cults, Tyranids, and the Tyranid and Astra Militarum allies. Keep UTF-8, two-space indentation and existing line endings.
- Skip Boarding Actions. Preserve intentional empty entries. Keep unsupported effects as rule text.
- Branches: `faction/<group>` for faction data (see [Faction groups](#faction-groups)), `chore/<topic>` for tools and docs, `core/<topic>` for shared dataset rules, and `allies/<owner>-<ally>` for cross-faction links.
- Direct commits to `dataset/develop` are reserved for the single Prepare commit per test version. Publish only after owner approval.

## Core flow

1. Start with `git fetch origin`. Shared branches are long-lived and the matching `origin/...` ref is authoritative. For faction work, check out the existing `faction/<group>` tracking branch for the faction's group; never replace it with a new branch from `dataset/develop`.
2. Do not merge `dataset/develop` (or `origin/dataset/develop`) back into faction branches. Work from the existing faction branch as-is; if a task appears to require changes that exist only on develop, ask the owner before changing the branch flow. Never fast-forward or rebase a shared branch, use `pull --rebase`, reset, recreate, force-move or force-push it.
3. Make the change on that branch and commit there with a one-line message and no attribution lines. Build: `dotnet run --project WarOrgan.Data.BuildDataset -c Release` must end with `0 errors`; `git diff --check` must be clean. Push the faction branch before merging it.
4. Switch to `dataset/develop`, confirm it matches `origin/dataset/develop`, and merge the work branch with `--no-ff`. If the faction branch's removal of `FACTIONS.md` conflicts or deletes the central copy, restore the pre-merge develop version before completing the merge: `git restore --source=ORIG_HEAD --staged --worktree -- FACTIONS.md`. Leave the faction branch pointing at its own last commit; never move it to the resulting develop merge commit. Push.
5. After all work branches for a test version are merged, make one "Prepare x.y.z.0" commit on `dataset/develop` (version, README coverage heading, CHANGELOG line with a blank line between entries, and FACTIONS.md). Push.
6. Upload the test build and check it serves the same bytes:
   `gh release upload develop-build WarOrgan.Data.BuildDataset/bin/Release/net10.0/Output/DominicWatts-WH40K11e.zip --clobber --repo DominicWatts/WarOrgan.Data`, then update the pre-release notes with the version and commit.
7. The owner tests in the WarOrgan app with `https://github.com/DominicWatts/WarOrgan.Data/releases/download/develop-build/DominicWatts-WH40K11e.zip`.
8. Only when they say release: merge `dataset/develop` into `dataset/wh40k11e` with `--no-ff` ("Release x.y.z.0"), push, tag `vX.Y.Z` and push the tag. Watch the `release-dataset.yml` run, then download `https://github.com/DominicWatts/WarOrgan.Data/releases/latest/download/DominicWatts-WH40K11e.zip` anonymously and check `DataSetInfo.json` shows the new version (the `latest` redirect can lag a minute).

## Working with the owner

- Ask before releasing, deleting, force-pushing or changing checked data.
- Explain in plain language what changed and what they should test; say plainly what is unverified.
- Preserve existing fixes and records without a replacement.
- Import rule text as plain text. Keep the words inside formatting tags and preserve list breaks. WarOrgan shows raw tags literally. The builder normalizes known tags in exported rule text as a fallback.
- Other agents may be working in the main checkout. Use `git worktree` for your own branches, never a bare `git stash`, and leave their uncommitted files alone. A worktree is only another checkout of the existing shared branch; do not invent a replacement branch for it.
- GitHub issues are the owner's bug reports and the task list: fix on the right branch, comment with the commit and test version, and close only when they agree.
- `CLAUDE.md`, `CODEX.md` and `AGENTS.md` are local-only (excluded in `.git/info/exclude`) and simply point here. `bsdata.zip` in the repository root is the pinned BSData dump (gitignored).
