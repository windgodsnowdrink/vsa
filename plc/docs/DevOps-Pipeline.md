# DevOps 基础架构流水线设计

> 版本：DevOps v1.0 · 状态：已落地（`.github/workflows/ci.yml`）
> 目标：把 Phase 4 的「人工门禁」固化为 CI 自动关卡，覆盖 构建 → 测试 → 安全 → 代码审查 → 部署。

---

## 1. 运行器与 SDK 约束

- **.NET 11 Preview 6**（`11.0.100-preview.6`）：GitHub Hosted Runner 默认不含 preview SDK。`ci.yml` 用 `actions/setup-dotnet@v4` 指定该版本；若该精确构建在 runner 不可用，**改跑自托管 Runner（已预装 preview SDK）**——见 `ci.yml` 顶部注释。
- **前端**：纯静态 `wwwroot/`，无 `package.json`。CI 仅做 `node --check` 语法校验（不打包）。
- **后端类型检查**：`plc-saas` 是 File-based App（无根 `.csproj`），全量类型检查走 `plc-saas.verify/plc-saas.verify.csproj`（剥离 `#include/#r` 后编译全部切片）。

## 2. 流水线阶段（Jobs）

| Job | 关卡 | 失败即阻断 |
|-----|------|-----------|
| `build-test` | restore → `dotnet build plc-saas.verify` → `dotnet test`（含 ADR-108 货币红线扫描）→ `node --check wwwroot/js/*.js` | ✅ |
| `security` | gitleaks（密钥）→ `dotnet list package --vulnerable`（依赖 CVE）→ Semgrep（SAST） | ✅ |
| `review-gates` | `scripts/review-gates.sh`（emoji / 硬编码色 / 货币红线 / 紫粉渐变 P0 扫描） | ✅ |
| `deploy`（手动） | 仅 `main` 且上述全绿后 `workflow_dispatch` 触发，推内网/预发 | — |

## 3. 质量门禁映射

- **P0 绝对规则**（见 `SaaS-Spec.md` §11）：emoji 功能图标禁用、硬编码色禁用（用 Design Tokens）、货币字段禁用（ADR-108）、紫→粉渐变禁用、通用 Hero 禁用 —— 由 `review-gates.sh` 自动拦截。
- **安全门禁**：密钥/依赖 CVE/SAST 任一非零即红。
- **测试门禁**：`plc-saas.verify` 测试必须全绿（含 2/2 红线扫描）。

## 4. 部署目标

- V1 内网：自托管 Runner 把 `plc-saas` 以 `dotnet run Program.cs` 起在边缘/内网节点；静态 `wwwroot` 由 YARP/静态服务托管（见 ADR-111）。
- 镜像化可选：后续补 `Dockerfile` + Trivy 扫描。

## 5. 后续增强

- 接 `Dockerfile` + 镜像推送 + Trivy。
- `deploy` 接具体环境（内网 k3s / 预发）。
- 覆盖率门禁（ `dotnet test --collect:"XPlat Code Coverage"` ）。
