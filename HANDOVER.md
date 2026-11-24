# Handover

Unofficial WarOrgan dataset for Warhammer 40,000 11th Edition. Repository `DominicWatts/WarOrgan.Data`, dataset ID `DominicWatts-WH40K11e`.

**Start with `TODO.md`.** It holds the rules (IDs, sources, branches, release steps), the prioritised tasks and the open app tests. `FACTIONS.md` has per-faction status. This file adds the day-to-day flow and how to work with the owner.

## Core flow

1. Start with `git fetch origin`. Shared branches are long-lived and the matching `origin/...` ref is authoritative. For faction work, check out the existing `faction/<slug>` tracking branch; if it exists on origin, never replace it with a new branch from `dataset/develop`.
2. If the faction branch needs current develop changes, merge `origin/dataset/develop` into it with an explicit `--no-ff` merge commit, then tell the owner it was updated. Do not fast-forward it to develop. Never rebase, use `pull --rebase`, reset, recreate, force-move or force-push a shared branch.
3. Make the change on that branch and commit there with a one-line message and no attribution lines. Build: `dotnet run --project WarOrgan.Data.BuildDataset -c Release` must end with `0 errors`; `git diff --check` must be clean. Push the faction branch before merging it.
4. Switch to `dataset/develop`, confirm it matches `origin/dataset/develop`, and merge the work branch with `--no-ff`. Leave the faction branch pointing at its own last commit; never move it to the resulting develop merge commit.
5. After all work branches for a test version are merged, make one "Prepare x.y.z.0" commit on `dataset/develop` (version, README coverage heading, CHANGELOG line with a blank line between entries, TODO.md and FACTIONS.md). Push.
6. Upload the test build and check it serves the same bytes:
   `gh release upload develop-build WarOrgan.Data.BuildDataset/bin/Release/net10.0/Output/DominicWatts-WH40K11e.zip --clobber --repo DominicWatts/WarOrgan.Data`, then update the pre-release notes with the version and commit.
7. The owner tests in the WarOrgan app with `https://github.com/DominicWatts/WarOrgan.Data/releases/download/develop-build/DominicWatts-WH40K11e.zip`.
8. Only when they say release: merge `dataset/develop` into `dataset/wh40k11e` with `--no-ff` ("Release x.y.z.0"), push, tag `vX.Y.Z` and push the tag. Watch the `release-dataset.yml` run, then download `https://github.com/DominicWatts/WarOrgan.Data/releases/latest/download/DominicWatts-WH40K11e.zip` anonymously and check `DataSetInfo.json` shows the new version (the `latest` redirect can lag a minute).

## Working with the owner

- Ask before releasing, deleting, force-pushing or changing checked data (Genestealer Cults, and the Tyranid and Astra Militarum allies). Tell them when a faction branch had to be updated from `dataset/develop`.
- Explain in plain language what changed and what they should test; say plainly what is unverified.
- Check claims against the source (BSData, Wahapedia) before stating them.
- Other agents may be working in the main checkout. Use `git worktree` for your own branches, never a bare `git stash`, and leave their uncommitted files alone. A worktree is only another checkout of the existing shared branch; do not invent a replacement branch for it.
- GitHub issues are the owner's bug reports: fix on the right branch, comment with the commit and test version, and close only when they agree.
- `CLAUDE.md`, `CODEX.md` and `AGENTS.md` are local-only (excluded in `.git/info/exclude`) and simply point here. `bsdata.zip` in the repository root is the pinned BSData dump (gitignored).
