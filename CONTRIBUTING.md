# Contributing

Thanks for helping improve the dataset. Outstanding work is tracked as [GitHub issues](https://github.com/DominicWatts/WarOrgan.Data/issues), and faction changes go to the faction's group branch (`faction/imperium`, `faction/space-marines`, `faction/chaos` or `faction/xenos`).

## Formatting JSON files

Keep the existing JSON format so pull requests show only real data changes. Many editors reformat JSON on save, for example by putting short arrays on one line, and that can turn a small fix into thousands of changed lines.

The repository format is what `jq --indent 2` produces:

- two-space indentation
- every array item and object property on its own line
- keys in their existing order
- UTF-8, keeping the byte order mark (BOM) if the file already has one

Before committing, run the command below on each JSON file you changed. It only changes layout, never values.

### Install jq

You need jq 1.7 or newer. Check with `jq --version`.

**Linux**

```bash
sudo apt install jq
```

On Fedora use `sudo dnf install jq`, and on Arch `sudo pacman -S jq`. If your distribution has an older version, download a binary from the [jq releases page](https://github.com/jqlang/jq/releases).

**Windows**

```powershell
winget install jqlang.jq
```

Open a new terminal afterwards so `jq` is on your path. Run the format command in Git Bash, which comes with [Git for Windows](https://git-scm.com/download/win), not in PowerShell or Command Prompt.

**macOS**

```bash
brew install jq
```

### Format a file

From the repository root, set `f` to the file you changed, for example the Drukhari roster:

```bash
f="WarOrgan.Data.BuildDataset/Assets/WH40K11e/Rosters/Drukhari.json"; { head -c3 "$f" | cmp -s - <(printf '\xef\xbb\xbf') && printf '\xef\xbb\xbf'; jq --indent 2 . "$f"; } > "$f.tmp" && mv "$f.tmp" "$f" || rm -f "$f.tmp"
```

If jq reports a parse error, the file is not valid JSON and is left unchanged; fix the error and run it again.

Then check the diff contains only your intended changes:

```bash
git diff --stat
```

A few roster files have no newline at the end, so formatting them may also add one. That is fine to commit. Line endings are handled by Git, so CRLF or LF in your working copy makes no difference.

### Build

Check the dataset still builds with `0 errors`:

```bash
dotnet run --project WarOrgan.Data.BuildDataset -c Release
```
