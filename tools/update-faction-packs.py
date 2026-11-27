#!/usr/bin/env python3
"""Refresh the local cache of GW Warhammer 40,000 faction packs in FactionPack/.

Lists the English faction packs (and Legends packs) from the Warhammer Community downloads
API, downloads any PDF not already cached, extracts a `pdftotext -layout` text copy next to
it, removes PDFs that are no longer listed, and writes FactionPack/index.json.

Usage: python3 tools/update-faction-packs.py [--out FactionPack] [--dry-run]
"""
import argparse
import json
import re
import shutil
import subprocess
import sys
import urllib.request
from datetime import date
from pathlib import Path

API = "https://www.warhammer-community.com/api/search/downloads/"
ASSETS = "https://assets.warhammer-community.com/"
CATEGORIES = ("faction-packs", "legends")
HEADERS = {"User-Agent": "Mozilla/5.0", "Content-Type": "application/json"}


def request(url, body=None):
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(url, data=data, headers=HEADERS, method="POST" if data else "GET")
    with urllib.request.urlopen(req, timeout=120) as response:
        return response.read()


def list_packs():
    body = {"index": "downloads_v2", "searchTerm": "", "gameSystem": "warhammer-40000", "language": "english"}
    hits = json.loads(request(API, body))["hits"]
    packs = []
    for hit in hits:
        categories = [c if isinstance(c, str) else c.get("slug") for c in hit.get("download_categories", [])]
        category = next((c for c in CATEGORIES if c in categories), None)
        if category is None:
            continue
        entry = hit["id"]
        packs.append({
            "title": entry["title"],
            "slug": entry["slug"],
            "category": category,
            "last_updated": entry["last_updated"],
            "file_size": entry["file_size"],
            "file": entry["file"],
            "url": ASSETS + entry["file"],
        })
    return sorted(packs, key=lambda p: p["title"].lower())


def pack_details(text):
    version = re.search(r"VERSION\s+([0-9][0-9.]*)", text)
    legal = re.search(r"Legal for matched play from ([^\n]+)", text)
    return {
        "version": version.group(1) if version else None,
        "legal_from": legal.group(1).strip() if legal else None,
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--out", default=str(Path(__file__).resolve().parent.parent / "FactionPack"))
    parser.add_argument("--dry-run", action="store_true", help="List what would change without downloading")
    args = parser.parse_args()

    if not args.dry_run and shutil.which("pdftotext") is None:
        sys.exit("pdftotext not found (install poppler-utils)")

    out = Path(args.out)
    packs = list_packs()
    print(f"{len(packs)} packs listed")
    if not args.dry_run:
        out.mkdir(exist_ok=True)

    listed = {p["file"] for p in packs}
    for stale in sorted(out.glob("*.pdf")) if out.exists() else []:
        if stale.name not in listed:
            print(f"remove  {stale.name}")
            if not args.dry_run:
                stale.unlink()
                stale.with_suffix(".txt").unlink(missing_ok=True)

    for pack in packs:
        pdf = out / pack["file"]
        txt = pdf.with_suffix(".txt")
        if not pdf.exists():
            print(f"fetch   {pack['title']} ({pack['file_size']})")
            if args.dry_run:
                continue
            pdf.write_bytes(request(pack["url"]))
        if not args.dry_run and (not txt.exists() or txt.stat().st_mtime < pdf.stat().st_mtime):
            subprocess.run(["pdftotext", "-layout", str(pdf), str(txt)], check=True)
        if txt.exists():
            pack.update(pack_details(txt.read_text(encoding="utf-8", errors="replace")))
        pack["pdf"] = pdf.name
        pack["text"] = txt.name

    if not args.dry_run:
        index = {"refreshed": date.today().isoformat(), "source": API, "packs": packs}
        (out / "index.json").write_text(json.dumps(index, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
        print(f"wrote {out / 'index.json'}")


if __name__ == "__main__":
    main()
