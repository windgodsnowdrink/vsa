#!/usr/bin/env bash
# =====================================================================
#  push-dev-via-ssh-once.sh — 生成/登记 SSH key 并推送 dev (443 穿墙版)
#
#  用法:
#    bash scripts/transfer/push-dev-via-ssh-once.sh
#    REMOTE=origin BRANCH=dev bash scripts/transfer/push-dev-via-ssh-once.sh
#
#  - 生成 ed25519 key -> 打印公钥 -> 提示粘贴到 GitHub Settings/SSH keys;
#  - 默认走 ssh.github.com:443 官方专用端口绕过企业 22 出口封禁;
#  - 若本机可用 nc + 代理, 自动附加 ProxyCommand (HTTP CONNECT 穿墙);
#  - ~/.ssh/config 使用幂等 Include 片段写入 github-vsa-plc 别名;
#  - push 完成后 origin URL 在 EXIT trap 自动还原为 HTTPS.
# =====================================================================
set -u

ESC_GREEN=$'\033[32m'
ESC_RED=$'\033[31m'
ESC_YELLOW=$'\033[33m'
ESC_BLUE=$'\033[36m'
ESC_OFF=$'\033[0m'

ok(){   printf "  %s[OK]%s %s\n"   "$ESC_GREEN"  "$ESC_OFF" "$1"; }
warn(){ printf "  %s[!!]%s %s\n"   "$ESC_YELLOW" "$ESC_OFF" "$1"; }
die(){  printf "  %s[XX]%s %s\n"   "$ESC_RED"    "$ESC_OFF" "$1"; exit 1; }
log(){  printf "%s▶ %s%s\n"       "$ESC_BLUE"   "$ESC_OFF" "$1"; }

# 进入 plc 仓库根 (scripts/transfer -> plc root)
SCRIPT_DIR=$(cd "$(dirname "$0")" && pwd)
cd "$SCRIPT_DIR/../.." || exit 1
if [ ! -d .git ]; then
  ALT_ROOT=$(git rev-parse --show-toplevel 2>/dev/null || true)
  if [ -n "$ALT_ROOT" ]; then
    cd "$ALT_ROOT" || die "无法进入仓库 $ALT_ROOT"
  else
    die "当前不是 git 仓库, 请在 plc 仓库内运行"
  fi
fi

REMOTE="${REMOTE:-origin}"
BRANCH="${BRANCH:-dev}"
KEY_PATH="${KEY_PATH:-$HOME/.ssh/id_ed25519_github_vsa}"

log "Repo = $(pwd)"
log "Remote=${REMOTE}  Branch=${BRANCH}  Key=${KEY_PATH}"

mkdir -p "$HOME/.ssh" && chmod 700 "$HOME/.ssh" || die "无法 mkdir $HOME/.ssh"

# ---- 1. 生成 ed25519 key (幂等) ----
if [ ! -f "$KEY_PATH" ]; then
  log "生成 ed25519 key (无口令) ..."
  STAMP=$(date +%s)
  ssh-keygen -t ed25519 -C "vsa-plc-push-once-${STAMP}" -f "$KEY_PATH" -N '' -q \
    || die "ssh-keygen 失败"
  ok "生成完毕: $KEY_PATH / $KEY_PATH.pub"
else
  ok "Key 已存在: $KEY_PATH"
fi

PUB=$(cat "$KEY_PATH.pub") || die "无法读取 $KEY_PATH.pub"
printf '\n'
printf '==================================================================\n'
printf '  复制下面 ed25519 公钥到 GitHub:\n'
printf '    路径: Settings -> SSH and GPG keys -> New SSH key\n'
printf '    Title: 任意 (比如 vsa-plc push-once)\n'
printf '    Key type: Authentication Key\n'
printf '    Key: (粘贴下一行内容)\n\n'
printf '%s\n' "$PUB"
printf '==================================================================\n\n'
if [ -t 0 ]; then
  printf '已在 GitHub 点下 Add SSH key? 按回车继续... '
  read -r _ || true
fi

# ---- 2. 组装 ~/.ssh/config 片段 (幂等 Include) ----
FRAG="$HOME/.ssh/config.github-vsa-plc"
PROXY_AUTO=""
for port in 18080 7890 10809 1080; do
  if (echo >"/dev/tcp/127.0.0.1/${port}") 2>/dev/null; then
    PROXY_AUTO="http://127.0.0.1:${port}"
    break
  fi
done
if [ -z "$PROXY_AUTO" ]; then
  PROXY_AUTO="${HTTPS_PROXY:-${https_proxy:-${HTTP_PROXY:-${http_proxy:-}}}}"
fi

PROXY_HOST=""
PROXY_PORT=""
PROXY_CMD_LINE=""
if command -v nc >/dev/null 2>&1 && [ -n "$PROXY_AUTO" ]; then
  PROXY_HOST=$(printf '%s' "$PROXY_AUTO" | sed -E 's#^https?://([^:/]+):([0-9]+).*$#\1#')
  PROXY_PORT=$(printf '%s' "$PROXY_AUTO" | sed -E 's#^https?://([^:/]+):([0-9]+).*$#\2#')
fi

# ---- 写 fragment 到 $FRAG 文件 ----
{
  printf '# --- PlcVsa push-once (scripts/transfer/push-dev-via-ssh-once.sh) ---\n'
  printf 'Host github-vsa-plc github.com ssh.github.com\n'
  printf '  HostName ssh.github.com\n'
  printf '  Port 443\n'
  printf '  User git\n'
  printf '  IdentityFile %s\n'   "$KEY_PATH"
  printf '  IdentitiesOnly yes\n'
  printf '  StrictHostKeyChecking accept-new\n'
  printf '  UserKnownHostsFile %s/.ssh/known_hosts\n' "$HOME"
  printf '  ServerAliveInterval 30\n'
  printf '  ServerAliveCountMax 5\n'
  if [ -n "$PROXY_HOST" ] && [ -n "$PROXY_PORT" ]; then
    printf '  ProxyCommand nc -X connect -x %s:%s %%h %%p\n' "$PROXY_HOST" "$PROXY_PORT"
  fi
  printf '# --- end PlcVsa push-once ---\n'
} > "$FRAG" || die "写入 $FRAG 失败"
chmod 600 "$FRAG" || die "chmod $FRAG 失败"

MAIN="$HOME/.ssh/config"
if [ ! -f "$MAIN" ] || ! grep -q "config.github-vsa-plc" "$MAIN" 2>/dev/null; then
  {
    printf 'Include %s\n\n' "$FRAG"
    if [ -f "$MAIN" ]; then cat "$MAIN"; fi
  } > "${MAIN}.tmp.plcvsa" || die "写入 MAIN tmp 失败"
  mv "${MAIN}.tmp.plcvsa" "$MAIN" || die "mv MAIN 失败"
fi
chmod 600 "$MAIN" || true
log "已幂等写入 ~/.ssh/config 片段: $FRAG"
if [ -n "$PROXY_HOST" ] && [ -n "$PROXY_PORT" ]; then
  ok "启用 443 + HTTP CONNECT: nc 经 ${PROXY_HOST}:${PROXY_PORT}"
else
  warn "未检测 nc 或可用代理. 直连 ssh.github.com:443 TCP, 可能被防火墙阻断"
fi

# ---- 3. 握手验证 (最多 3 次, BatchMode 防死锁) ----
log "握手: ssh -T git@github.com (GitHub 打印 Hi <user>! 后退出=1 视为通过)"
set +e
SSH_OK=0
OUT=""
for i in 1 2 3; do
  OUT=$(timeout 15 ssh -o BatchMode=yes -T git@github.com 2>&1)
  if printf '%s\n' "$OUT" | grep -qiE "successfully authenticated|Hi .*!"; then
    SSH_OK=1; break
  fi
  if [ "$i" -lt 3 ]; then
    warn "第 $i 次未通过, 1s 后重试..."
    sleep 1
  fi
done
printf '%s\n' "$OUT" | tail -3
set -e
if [ "$SSH_OK" -ne 1 ]; then
  die "SSH 握手失败. 原因通常: (a) 公钥未添加; (b) ssh.github.com:443 被阻断; (c) nc 或 HTTP CONNECT 失败"
fi
ok "SSH 已通过 GitHub 校验"

# ---- 4. 切换 remote 到 SSH URL 并 push ----
OLD_URL=$(git remote get-url "$REMOTE") || die "无法读取 $REMOTE url"
log "Backup origin URL = $OLD_URL"
# shellcheck disable=SC2064
cleanup(){
  log "EXIT trap: 还原 origin URL"
  git remote set-url "$REMOTE" "$OLD_URL" >/dev/null 2>&1 || true
}
trap cleanup EXIT

git remote set-url "$REMOTE" "git@github-vsa-plc:windgodsnowdrink/vsa.git" \
  || die "无法设置 $REMOTE 为 SSH URL"

log "ls-remote $REMOTE refs/heads/$BRANCH"
git ls-remote "$REMOTE" "refs/heads/$BRANCH" | head -3

log "fetch --no-tags $REMOTE"
git fetch --no-tags "$REMOTE" && ok "FETCH OK"

A=$(git rev-list --count "refs/remotes/${REMOTE}/${BRANCH}..HEAD" 2>/dev/null || echo 0)
log "ahead = $A"

log "push --dry-run origin $BRANCH"
set +e
git push --dry-run --verbose "$REMOTE" "$BRANCH" 2>&1 | tail -10
set -e

if [ "$A" -gt 0 ]; then
  log "真实 push --verbose $REMOTE $BRANCH"
  git push --verbose "$REMOTE" "$BRANCH"
else
  ok "ahead=0, 无需 push, 远端 ${REMOTE}/${BRANCH} 已经最新"
fi

log "Final refs"
HEAD_REF=$(git rev-parse HEAD 2>/dev/null || echo MISSING)
BR_REF=$(git rev-parse "refs/heads/${BRANCH}" 2>/dev/null || echo MISSING)
RB_REF=$(git rev-parse "refs/remotes/${REMOTE}/${BRANCH}" 2>/dev/null || echo MISSING)
LS_REF=$(git ls-remote "$REMOTE" "refs/heads/${BRANCH}" 2>/dev/null | awk '{print $1}')
printf '  HEAD                    = %s\n' "$HEAD_REF"
printf '  refs/heads/%s      = %s\n' "$BRANCH" "$BR_REF"
printf '  remotes/%s/%s = %s\n' "$REMOTE" "$BRANCH" "$RB_REF"
printf '  ls-remote %s       = %s\n' "$BRANCH" "$LS_REF"
ok "push-dev-via-ssh-once.sh 流程跑完 (EXIT trap 会还原 origin URL)"
