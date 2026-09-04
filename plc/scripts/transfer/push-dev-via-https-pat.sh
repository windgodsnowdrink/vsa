#!/usr/bin/env bash
# =====================================================================
#  push-dev-via-https-pat.sh —— 『本地已登录 GitHub』终端一键推送 dev
#  适用场景:
#    1) 沙箱 GIT_TERMINAL_PROMPT=0 无法弹框;
#    2) Windows/macOS/Linux 任意终端, 想完全不触发 credential 弹框;
#    3) 只需持有 Fine-grained PAT (Contents R/W + Metadata R)。
#
#  用法 1 — 环境变量传入（推荐 CI / 私密终端）:
#       GITHUB_USER=windgodsnowdrink \
#       GITHUB_TOKEN=github_pat_xxx \
#       bash push-dev-via-https-pat.sh
#
#  用法 2 — 交互式提示:
#       bash push-dev-via-https-pat.sh
#
#  特点:
#    - 自动探测本地 Clash / v2rayN / 沙箱代理端口并写仓库级 proxy;
#    - 探测 HTTPS CONNECT 再工作;
#    - 恢复 origin URL 为匿名 HTTPS (推送后立即抹去 origin 中的 user:token, 不落明文);
#    - ls-remote / fetch / push --dry-run / push  四连证据输出;
#    - 仅仓库级配置, 不污染 --global;
#    - 可选参数 BRANCH=name 指定非 dev 分支, REMOTE=origin2 等。
# =====================================================================
set -euo pipefail

BRANCH="${BRANCH:-dev}"
REMOTE="${REMOTE:-origin}"

log(){ printf "\e[36m▶ %s\e[0m\n" "$*"; }
ok(){ printf "  \e[32m[OK]\e[0m %s\n" "$*"; }
warn(){ printf "  \e[33m[!!]\e[0m %s\n" "$*" >&2; }
die(){ printf "  \e[31m[XX]\e[0m %s\n" "$*" >&2; exit 1; }

cd "$(dirname "$0")/../.."  # scripts/transfer -> plc repo root
[ -d .git ] || cd "$(git rev-parse --show-toplevel 2>/dev/null)" || die "不在 git 仓库里, 请把此脚本放在 plc/scripts/transfer/"
[ -d .git ] || die "找不到 .git (当前目录 $(pwd))"

log "Repo root: $(pwd)"
log "Remote=$REMOTE  Branch=$BRANCH"

# —— 1. 凭据 (env 优先, 否则读取) ——
if [ -z "${GITHUB_USER:-}" ] || [ -z "${GITHUB_TOKEN:-}" ]; then
  if [ -t 0 ]; then
    printf 'GITHUB_USER (windgodsnowdrink): '; read -r GITHUB_USER || true
    printf 'GITHUB_TOKEN (fine-grained PAT with Contents R/W + Metadata R): '; IFS= read -rs GITHUB_TOKEN || true; echo
  fi
fi
[ -n "${GITHUB_USER:-}"  ] || die "缺少 GITHUB_USER"
[ -n "${GITHUB_TOKEN:-}" ] || die "缺少 GITHUB_TOKEN"

# —— 2. 代理自动探测 ——
PROXY=""
for port in 18080 7890 10809 1080; do
  (echo >/dev/tcp/127.0.0.1/$port) 2>/dev/null && PROXY="http://127.0.0.1:$port" && break
done
if [ -z "$PROXY" ]; then
  PROXY="${HTTPS_PROXY:-${https_proxy:-${HTTP_PROXY:-${http_proxy:-}}}}"
fi
if [ -n "$PROXY" ]; then
  log "Using proxy: $PROXY"
  git config --local http.proxy  "$PROXY"
  git config --local https.proxy "$PROXY"
fi
git config --local http.version    HTTP/1.1
git config --local http.sslVerify  true
git config --local http.postBuffer 524288000

# —— 3. GitHub HTTPS probe ——
log "Probe GitHub https via proxy"
CURL_PROXY=()
[ -n "$PROXY" ] && CURL_PROXY=(-x "$PROXY")
curl -sS -m 10 -o /dev/null -w "  HTTP=%{http_code}  time=%{time_total}s\n" \
  "${CURL_PROXY[@]}" "https://github.com/windgodsnowdrink/vsa.git/info/refs?service=git-upload-pack" \
  || die "CONNECT GitHub failed, check proxy/PROXY env or network"

# —— 4. 备份旧 remote URL, 写入带 PAT 的 URL ——
OLD_URL=$(git remote get-url "$REMOTE")
log "Backup origin URL: $OLD_URL"
restore(){ log "Restoring origin URL (strip credentials)"; SAFE_URL=$(echo "$OLD_URL" | sed -E 's#(https?://)[^:/@]+:[^@/]+@#\1#'); git remote set-url "$REMOTE" "$SAFE_URL" || true; }
trap restore EXIT

AUTH_URL="https://${GITHUB_USER}:${GITHUB_TOKEN}@github.com/windgodsnowdrink/vsa.git"
git remote set-url "$REMOTE" "$AUTH_URL"

# —— 5. 四连: ls-remote / fetch / push-dry-run / push ——
log "git ls-remote $REMOTE HEAD $BRANCH main"
git ls-remote --symref "$REMOTE" HEAD "refs/heads/$BRANCH" refs/heads/main | head -4

log "git fetch --no-tags $REMOTE"
git fetch --no-tags "$REMOTE" && ok "FETCH OK"

log "git push --dry-run --verbose $REMOTE $BRANCH"
git push --dry-run --verbose "$REMOTE" "$BRANCH" 2>&1 | tail -10 \
  || warn "dry-run 失败 (若 ahead=0, 则属正常 'Everything up-to-date' 判断)"

A=$(git rev-list --count "refs/remotes/${REMOTE}/${BRANCH}..HEAD" 2>/dev/null || echo 0)
log "ahead count=$A"
if [ "$A" -gt 0 ]; then
  log "git push --verbose $REMOTE $BRANCH (真实推送)"
  git push --verbose "$REMOTE" "$BRANCH"
  ok "PUSH 命令已执行 (请检查上面输出是否 Writing objects / done)"
else
  ok "ahead=0, 无需真实 push. 远端 ${REMOTE}/${BRANCH} 已经最新."
fi

# —— 6. 最终一致性 ——
log "Final refs:"
echo "  HEAD                = $(git rev-parse HEAD)"
echo "  refs/heads/$BRANCH  = $(git rev-parse "refs/heads/$BRANCH")"
echo "  remotes/${REMOTE}/$BRANCH = $(git rev-parse "refs/remotes/${REMOTE}/${BRANCH}" 2>/dev/null || echo MISSING)"
echo "  ls-remote $BRANCH   = $(git ls-remote "$REMOTE" "refs/heads/$BRANCH" 2>/dev/null | awk '{print $1}')"
ok "push-dev-via-https-pat 完成 (凭证会在 EXIT trap 立即从 origin URL 抹去)"
