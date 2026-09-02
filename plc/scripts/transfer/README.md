# scripts/transfer/ — 离线 / Windows 快速恢复目录

本目录包含**可版本控制**（能直接 clone 下来就有）的恢复辅助脚本与校验清单。真正体积大的二进制交付物（bundle）不进 git，而是放在仓库根 `plc/*.bundle`，并在本目录的 `SHA256SUMS.txt` 里登记 sha256 便于传输后手工校验。

## 文件清单

| 文件 | 作用 |
|---|---|
| `fix-github-dev-push.cmd` | **Windows 一键修复 CMD**（双击运行 / CMD 里运行都行）：自动探测 Clash/v2rayN 端口 → 写 repo 级 proxy → 测 GitHub HTTPS → ls-remote → fetch → 可带 `WITH_PAT` 参数真正 push。对 PowerShell 执行策略受限的老 Windows 机器尤其友好。 |
| `SHA256SUMS.txt` | 所有离线交付物（脚本 / patch / 增量 bundle / 全量 bundle / stash bundle）的 SHA256 + 文件大小清单，格式兼容 `sha256sum -c`（Linux/macOS/WSL）。 |

## 常用操作速查

### (1) 最省心：Windows 一键
```cmd
cd /d E:\WorkSpace\windgodsnowdrink\vsa\plc
scripts\transfer\fix-github-dev-push.cmd
:: 想同时真正 push origin dev（带 PAT 不弹登录框）：
scripts\transfer\fix-github-dev-push.cmd WITH_PAT
```

### (2) PowerShell 版（功能更完整）
```powershell
cd E:\WorkSpace\windgodsnowdrink\vsa\plc
powershell -ExecutionPolicy Bypass -File .\scripts\06-fix-github-dev-push.ps1
```

### (3) 离线恢复（完全无网络，只靠 bundle 文件）
```bash
# Linux / macOS / WSL
sha256sum -c scripts/transfer/SHA256SUMS.txt   # 先校验

# 推荐：自包含完整历史 bundle（70MB，不依赖任何前置 commit）
git clone dev-latest-full.bundle plcvsa-latest

# 增量：需要本地已经有 prerequisite commit (3f0cd66)
git bundle unbundle dev-ahead.bundle
git cherry-pick 9e8dde5    # restore + push 即可

# Stash 快照：把当时的暂存内容恢复到 git stash
git bundle unbundle stash-plcvsa.bundle
git stash store -m "plc-vsa-micromonolith" 6f3a99151c08f42b538c8fba5eb48576e6d14f52
```

### (4) Windows PowerShell 手动校验 hash
```powershell
Get-ChildItem dev-latest-full.bundle, stash-plcvsa.bundle, dev-ahead.bundle `
  | Get-FileHash -Algorithm SHA256 `
  | Select-Object Hash, Length, Path
```
