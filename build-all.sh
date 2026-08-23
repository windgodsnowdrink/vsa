#!/usr/bin/env bash
# build-all.sh — Build every Linux-buildable csproj under plc/src + plc/tests + plc root.
# Skips: external csharpflink-spike/repo/** (already built as POC dependency)
set -u
export PATH="/opt/dotnet:$PATH"
export DOTNET_ROOT="/opt/dotnet"
export DOTNET_ROLL_FORWARD=Major
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=true

PLC=/workspace/plc
LOG=/workspace/build-results.txt
: > "$LOG"

# Projects to build, in dependency order where possible.
mapfile -t PROJECTS < <(find "$PLC" -name '*.csproj' \
  -not -path '*/csharpflink-spike/repo/*' \
  -not -path '*/csgo-spike/CsGo-master/*' \
  -not -path '*/csgo-spike/src/*' \
  | sort)

total=${#PROJECTS[@]}
i=0
ok=0
fail=0
skipped=0
FAILED_LIST=()
SKIPPED_LIST=()

for proj in "${PROJECTS[@]}"; do
  i=$((i+1))
  rel="${proj#$PLC/}"
  printf "[%d/%d] %s ... " "$i" "$total" "$rel" | tee -a "$LOG"
  if grep -Eq 'Microsoft\.NET\.Sdk\.(WindowsDesktop|Razor)|<UseWindowsForms>true|<UseMaui>true|net[0-9]+-windows' "$proj"; then
    echo "SKIP (Windows-only)" | tee -a "$LOG"
    SKIPPED_LIST+=("$rel")
    skipped=$((skipped+1))
    continue
  fi
  out=$(cd "$(dirname "$proj")" && dotnet build -nologo -clp:NoSummary 2>&1)
  rc=$?
  if [ $rc -eq 0 ]; then
    echo "OK" | tee -a "$LOG"
    ok=$((ok+1))
  else
    errs=$(echo "$out" | grep -E 'error[: ]' | head -5)
    echo "FAIL" | tee -a "$LOG"
    echo "$errs" | sed 's/^/      /' | tee -a "$LOG"
    FAILED_LIST+=("$rel")
    fail=$((fail+1))
  fi
done

echo "------------------------------------------" | tee -a "$LOG"
echo "TOTAL=$total OK=$ok FAIL=$fail SKIPPED=$skipped" | tee -a "$LOG"
if [ ${#FAILED_LIST[@]} -gt 0 ]; then
  echo "FAILED:" | tee -a "$LOG"
  for f in "${FAILED_LIST[@]}"; do echo "  - $f" | tee -a "$LOG"; done
fi
if [ ${#SKIPPED_LIST[@]} -gt 0 ]; then
  echo "SKIPPED (Windows-only):" | tee -a "$LOG"
  for s in "${SKIPPED_LIST[@]}"; do echo "  - $s" | tee -a "$LOG"; done
fi
