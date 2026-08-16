#!/usr/bin/env bash
# P0 自动门禁：emoji 功能图标 / 硬编码实色 / 货币红线 / 紫→粉渐变
# 由 CI review-gates job 调用；任一命中即退出 1。
set -uo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

FAIL=0
report() { echo "::error::$1"; FAIL=1; }

echo "== P0 Gate: emoji functional icons =="
# 排除箭头/方框绘制（→ 等属注释可接受），聚焦彩色 emoji 图符；跳过构建产物 obj/bin
if grep -rPn "[\x{1F000}-\x{1FAFF}\x{2600}-\x{27BF}\x{2B00}-\x{2BFF}]" \
     plc-saas wwwroot --include=*.cs --include=*.html --include=*.css --include=*.js 2>/dev/null \
     | grep -v "/obj/" | grep -v "/bin/"; then
  report "emoji 功能图标命中（应为 Lucide <use href=\"#i-xxx\"/>）"
fi

echo "== P0 Gate: currency red line (ADR-108) =="
# 排除构建产物(obj/bin) 与红线自检测试本身(VerifyTests.cs)；排除注释行
if grep -rnEi "\b(price|currency|amount|money|cost|fee|charge|invoice)\b" \
     plc-saas --include=*.cs 2>/dev/null \
     | grep -v "/obj/" | grep -v "/bin/" | grep -v "VerifyTests.cs" \
     | grep -vE "//|/\*|\*"; then
  report "源代码出现货币标识符（ADR-108 仅计量不计费）"
fi
# 前端 billing 页与 mock 数据不应含货币符号/月费字段
if grep -rnE "[\$¥]|monthlyFee|priceTier" wwwroot/js/data.json wwwroot/pages/account/billing.html 2>/dev/null; then
  report "前端 billing 出现货币符号/费用字段"
fi

echo "== P0 Gate: hardcoded solid colors outside :root tokens =="
# 仅扫实色 hex；排除：自定义属性定义行(^ *--name:)、注释行、#000/#fff 例外
if grep -rnEi "#[0-9a-f]{3,8}\b" wwwroot/css --include=*.css 2>/dev/null \
     | grep -vE -- "--[A-Za-z0-9_-]+[[:space:]]*:|gradient" \
     | grep -vE "//|/\*|\*/" \
     | grep -vE "#000\b|#000000\b|#fff\b|#ffffff\b"; then
  report "CSS 出现硬编码实色（应使用 Design Tokens / var(--*)）"
fi

echo "== P0 Gate: purple→pink gradient =="
if grep -rniE "linear-gradient\([^)]*(purple|#(7c3aed|8b5cf6|a855f7|c084fc|d946ef|ec4899|f472b6))" \
     wwwroot --include=*.css --include=*.html 2>/dev/null; then
  report "出现紫→粉渐变（P0 禁用）"
fi

if [ "$FAIL" -ne 0 ]; then
  echo "P0 GATE FAILED"
  exit 1
fi
echo "P0 GATE PASSED"
