#!/usr/bin/env bash
# strip-app.sh — cross-platform port of strip-app.ps1.
# Strips File-based '#include' / '#r' directives from .cs sources so the whole
# app (incl. Program.cs) compiles as a normal csproj. Does not modify originals.
# Args: $1 = source root, $2 = output root.
set -euo pipefail

SrcRoot="$1"
OutRoot="$2"

# Resolve to absolute paths (MSBuild may pass relative paths on Windows-style builds).
SrcRoot="$(cd "$SrcRoot" && pwd)"
OutRoot="$(cd "$(dirname "$OutRoot")" && pwd)/$(basename "$OutRoot")"

echo "STRIP SRC=[$SrcRoot] OUT=[$OutRoot]"

rm -rf "$OutRoot"
mkdir -p "$OutRoot"

strip_file() {
  local inPath="$1"
  local outPath="$2"
  # Keep every line whose first non-space char does not start a File-based directive.
  awk '{
    line=$0
    # leading whitespace trim for test
    t=line; sub(/^[ \t\r\n]+/,"",t)
    if (substr(t,1,8)=="#include" || substr(t,1,3)=="#r " || substr(t,1,2)=="#r\"") next
    print line
  }' "$inPath" > "$outPath"
}

topCount=0
for f in "$SrcRoot"/*.cs; do
  [ -f "$f" ] || continue
  topCount=$((topCount+1))
  name="$(basename "$f")"
  strip_file "$f" "$OutRoot/$name"
done
echo "topCount=$topCount"

sd="$SrcRoot/slices"
if [ -d "$sd" ]; then
  dd="$OutRoot/slices"
  mkdir -p "$dd"
  sliceCount=0
  for f in "$sd"/*.cs; do
    [ -f "$f" ] || continue
    sliceCount=$((sliceCount+1))
    strip_file "$f" "$dd/$(basename "$f")"
  done
  echo "sliceCount=$sliceCount"
else
  echo "slicesDirExists=false"
fi
echo "STRIP DONE"
