#!/usr/bin/env bash
# strip-sources.sh — cross-platform port of strip-sources.ps1.
# Strips File-based '#include' / '#r' directives from .cs sources so they compile
# as a normal csproj (typecheck harness). Skips Program.cs. Does not modify originals.
# Args: $1 = source root (the plc-saas dir, i.e. $(MSBuildThisFileDirectory)..), $2 = output root.
set -euo pipefail

SrcRoot="$1"
OutRoot="$2"

SrcRoot="$(cd "$SrcRoot" && pwd)"
OutRoot="$(cd "$(dirname "$OutRoot")" && pwd)/$(basename "$OutRoot")"

echo "Stripped sources preparing at SRC=[$SrcRoot] OUT=[$OutRoot]"

rm -rf "$OutRoot"
mkdir -p "$OutRoot"

strip_file() {
  local inPath="$1"
  local outPath="$2"
  awk '{
    line=$0
    t=line; sub(/^[ \t\r\n]+/,"",t)
    if (substr(t,1,8)=="#include") next
    if (substr(t,1,3)=="#r ") next
    if (substr(t,1,2)=="#r\"") next
    print line
  }' "$inPath" > "$outPath"
}

for f in "$SrcRoot"/*.cs; do
  [ -f "$f" ] || continue
  name="$(basename "$f")"
  if [ "$name" = "Program.cs" ]; then continue; fi
  strip_file "$f" "$OutRoot/$name"
done

slicesDir="$SrcRoot/slices"
if [ -d "$slicesDir" ]; then
  destDir="$OutRoot/slices"
  mkdir -p "$destDir"
  for f in "$slicesDir"/*.cs; do
    [ -f "$f" ] || continue
    strip_file "$f" "$destDir/$(basename "$f")"
  done
fi
echo "Stripped sources prepared at $OutRoot"
