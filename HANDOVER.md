# Handover

Unofficial WarOrgan dataset for Warhammer 40,000 11th Edition. Repository `DominicWatts/WarOrgan.Data`, dataset ID `DominicWatts-WH40K11e`.

**Start with `TODO.md`.** It holds the rules (IDs, sources, branches, release steps), the prioritised tasks and the open app tests. `FACTIONS.md` has per-faction status. This file adds the day-to-day flow and how to work with the owner.

## Core flow

1. Branch from `dataset/develop`: `faction/<slug>`, `core/<topic>`, `chore/<topic>` or `allies/<owner>-<ally>`. Commit there; one-line messages, no attribution lines.
2. Build: `dotnet run --project WarOrgan.Data.BuildDataset -c Release` must end with `0 errors`; `git diff --check` must be clean.
3. Merge into `dataset/develop` with `--no-ff`, then one "Prepare x.y.z.0" commit there (version, README coverage heading, CHANGELOG line with a blank line between entries, TODO.md and FACTIONS.md). Push.
4. Upload the test build and check it serves the same bytes:
   `gh release upload develop-build WarOrgan.Data.BuildDataset/bin/Release/net10.0/Output/DominicWatts-WH40K11e.zip --clobber --repo DominicWatts/WarOrgan.Data`, then update the pre-release notes with the version and commit.
5. The owner tests in the WarOrgan app with `https://github.com/DominicWatts/WarOrgan.Data/releases/download/develop-build/DominicWatts-WH40K11e.zip`.
6. Only when they say release: merge `dataset/develop` into `dataset/wh40k11e` with `--no-ff` ("Release x.y.z.0"), push, tag `vX.Y.Z` and push the tag. Watch the `release-dataset.yml` run, then download `https://github.com/DominicWatts/WarOrgan.Data/releases/latest/download/DominicWatts-WH40K11e.zip` anonymously and check `DataSetInfo.json` shows the new version (the `latest` redirect can lag a minute).

## Working with the owner

- Ask before releasing, deleting, force-pushing or changing checked data (Genestealer Cults, and the Tyranid and Astra Militarum allies). Tell them when a faction branch had to be updated from `dataset/develop`.
- Explain in plain language what changed and what they should test; say plainly what is unverified.
- Check claims against the source (BSData, Wahapedia) before stating them.
- Other agents may be working in the main checkout. Use `git worktree` for your own branches, never a bare `git stash`, and leave their uncommitted files alone.
- GitHub issues are the owner's bug reports: fix on the right branch, comment with the commit and test version, and close only when they agree.
- `CLAUDE.md`, `CODEX.md` and `AGENTS.md` are local-only (excluded in `.git/info/exclude`) and simply point here. `bsdata.zip` in the repository root is the pinned BSData dump (gitignored).
