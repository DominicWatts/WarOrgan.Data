#!/usr/bin/env python3
import argparse
import json
import shutil
import subprocess
import sys
from pathlib import Path


def merge_roster(draft_path: Path, existing_path: Path) -> bool:
    draft = json.loads(draft_path.read_text(encoding="utf-8"))
    if not existing_path.exists():
        existing_path.write_text(json.dumps(draft, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="")
        return True

    existing = json.loads(existing_path.read_text(encoding="utf-8"))
    known_ids = {unit.get("Id") for unit in existing.get("Units", [])}
    additions = [unit for unit in draft.get("Units", []) if unit.get("Id") not in known_ids]
    if not additions:
        return False
    existing.setdefault("Units", []).extend(additions)
    existing["Units"] = sorted(existing["Units"], key=lambda unit: (str(unit.get("DisplayName") or unit.get("Id") or "")).casefold())
    existing_path.write_text(json.dumps(existing, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="")
    return True


def main() -> int:
    parser = argparse.ArgumentParser(description="Generate all committed draft factions from BSData")
    parser.add_argument("--source", default="bsdata.zip")
    parser.add_argument("--out", default="/tmp/warorgan-bulk-draft")
    parser.add_argument("--definition", default="WarOrgan.Data.BuildDataset/Assets/WH40K11e/DataSetDefinition.json")
    parser.add_argument("--rosters", default="WarOrgan.Data.BuildDataset/Assets/WH40K11e/Rosters")
    parser.add_argument("--all", action="store_true", help="generate every faction in factions.json")
    args = parser.parse_args()
    if not args.all:
        parser.error("bulk drafting requires --all")

    script_dir = Path(__file__).resolve().parent
    repo = Path.cwd()
    source_arg = Path(args.source)
    source = source_arg if source_arg.is_absolute() else (script_dir / source_arg)
    if not source.exists():
        source = repo / source_arg
    if not source.exists():
        print(f"Source not found: {source}", file=sys.stderr)
        return 1

    factions = json.loads((script_dir / "factions.json").read_text(encoding="utf-8"))
    retired = set(json.loads((script_dir / "retired-datasheets.json").read_text(encoding="utf-8")))
    output = Path(args.out)
    if output.exists():
        shutil.rmtree(output)
    output.mkdir(parents=True)

    rosters = (repo / args.rosters) if not Path(args.rosters).is_absolute() else Path(args.rosters)
    changed = 0
    for faction in factions:
        name = faction["name"]
        command = [
            "dotnet", "run", "--project", "WarOrgan.Data.BsdataImport", "--",
            "--source", str(source), "--catalogue", faction["catalogue"],
            "--faction", faction.get("factionKeyword", name), "--definition", str(repo / args.definition), "--out", str(output),
        ]
        result = subprocess.run(command, cwd=repo)
        if result.returncode:
            return result.returncode

        faction_keyword = faction.get("factionKeyword", name)
        draft_path = output / f"{faction_keyword}.json"
        draft = json.loads(draft_path.read_text(encoding="utf-8"))
        draft["Units"] = [unit for unit in draft.get("Units", []) if (unit.get("DisplayName") or unit.get("Id") or "") not in retired]
        draft_path.write_text(json.dumps(draft, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="")

        existing_path = rosters / f"{name}.json"
        if merge_roster(draft_path, existing_path):
            changed += 1

        draft_legends = output / f"{faction_keyword} Legends.json"
        if draft_legends.exists():
            legends = json.loads(draft_legends.read_text(encoding="utf-8"))
            legends["Units"] = [unit for unit in legends.get("Units", []) if (unit.get("DisplayName") or unit.get("Id") or "") not in retired]
            draft_legends.write_text(json.dumps(legends, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="")
            existing_legends = rosters / f"{name} Legends.json"
            if merge_roster(draft_legends, existing_legends):
                changed += 1

    print(f"Bulk draft complete: {len(factions)} factions, {changed} roster files changed/created")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
