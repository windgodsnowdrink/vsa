# Phase 4 验证报告 · PLC·AIOT 多租户 SaaS

> 验证日期：2026-08-16 · 验证方式：总监独立复跑（信任但验证，原 QA 子代理结论未落入上下文）
> 工程：`.NET 11 preview.6 (11.0.100-preview.6.26359.118)` + `plc-saas.verify` 类型检查工程

## 1. 构建与测试（独立复跑）

| 项 | 命令 | 结果 |
|---|---|---|
| 类型检查/构建 | `dotnet test plc-saas/plc-saas.verify/plc-saas.verify.csproj` | **0 错误** |
| 单元测试 | 同上（含 ADR-108 货币红线扫描） | **通过 2 / 失败 0** |

良性提示（非阻断）：
- `NETSDK1057`：使用 .NET 预览版（信息级）。
- `MSG0005`：`TelemetryIngested` 无注册 Handler —— 该事件走自建 Outbox，预期行为。

> SDK 解析坑：本机 `global.json` 钉 `11.0.100`，宿主默认不选 preview。验证时临时改为精确钉 `11.0.100-preview.6` 跑通，**已还原**，仓库无残留改动。

## 2. P0 红线门禁（scripts/review-gates.sh，退出码 0）

| 门禁 | 结果 |
|---|---|
| 功能 emoji 图标 | PASS（零命中，仅注释 `→` 箭头） |
| ADR-108 货币红线 | PASS（无 price/currency/amount/money 标识符） |
| 硬编码色（:root 外） | PASS（仅令牌定义区） |
| 紫→粉渐变 | PASS（零命中） |

## 3. 端点 ↔ OpenAPI 交叉核对

- 实装 **21 个 method+path 组合**（slices/*.cs 中 `api.MapXxx`）；
- `docs/openapi.yaml` 声明 **19 个路径模板**（含 `/billing/plan`、`/tenant/members` 双方法）；
- 前缀 `/api/v1` 由 `app.MapGroup` 注入；
- 结论：**一一对应，无缺漏、无多余**，角色/403 语义与 Spec §5/§9 一致。

## 4. DevOps 部署验证（运行期）

| 项 | 状态 | 说明 |
|---|---|---|
| health 端点 | 已实现 | `GET /ops/health`（`slices/ops.health.cs`，超管），返回 api/postgres/signalr/mqtt-broker 四组件状态 |
| 运行期冒烟 | **阻塞** | 启动须 PostgreSQL（`EnsureCreated`+`rls.sql`+`Seed`）；本沙箱无法探测 5432（EPERM），EMQX 亦未就绪 |
| CI 流水线 | 已落地 | `.github/workflows/ci.yml` 覆盖 build/test/P0/安全/审查关卡，待接入带 Postgres 服务容器的 Runner |

**部署运行手册（目标环境）**：
```bash
# 1) 准备 PostgreSQL（含 plcsaas 角色），按 appsettings.json 连接串建库
# 2) 启动应用（File-based App，根无 .csproj，用 dotnet run Program.cs）
cd plc-saas
dotnet run Program.cs
# 3) 探针
curl -s http://localhost:5000/api/v1/ops/health
```

## 5. 结论

- **编译/测试/P0/契约门禁：全部 PASS**，Phase 4 质量关卡达成。
- **运行期端到端冒烟：留作部署环境前置项**（需 Postgres + EMQX），CI 已编排、待 Runner 接服务容器。
- 待办：① 部署环境跑通 health + 核心多租户流程；② 是否补 `plc-saas` 根 `.csproj`（`dotnet build`/`publish` 直跑）；③ 见 README 分层索引的未提交 SaaS 文档需纳入版本控制。
