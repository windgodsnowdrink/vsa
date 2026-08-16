# ADR-112 实现说明：MEAI + MCP + Qdrant（P1 旗舰）

> 项目：PLC·AIOT 多租户 SaaS · 状态：**Accepted（P1 已落地，2026-08-16）**  
> 基线：.NET 11 Preview 7 + VSA + DDD + CQRS + File-based App  
> 关联：ADR-103（租户隔离）、ADR-108（仅计量不计费）、ADR-113（MQTT 数据平面）

## 1. 目标与范围

为 SaaS 控制面提供**差异化 AI 能力**：设备运维故障诊断（RAG）+ 对外暴露 MCP Server 供外部 AI Agent 安全问诊。

- **MEAI（Microsoft.Extensions.AI）**：作 LLM 抽象层（`IChatClient`），与具体 Provider 解耦。
- **MCP Server**：将设备/故障/遥测暴露为工具，外部 Agent 经 `/mcp` 调用，`tid` 作用域隔离。
- **Qdrant**：存故障模式向量（租户分区 collection）；未启用时回退内存实现。

所有能力均**经 `ICurrentTenant` + EF 全局过滤器保障租户隔离**，并受 feature flag 门控（默认关闭）。

## 2. 隔离模型（沿用 ADR-103）

| 层      | 机制                                                                   |
| ------ | -------------------------------------------------------------------- |
| 数据读取   | `BaseDbContext` 全局查询过滤器 `TenantId == ICurrentTenant.TenantId`        |
| 写入     | `TenantStampInterceptor` 自动盖戳 + 拒绝跨租户写入                              |
| 行级安全   | PG RLS（启动时 `rls.sql` 幂等应用）                                           |
| MCP 工具 | `PlcMcpTools.Resolve()` 强制校验 `ICurrentTenant.TenantId != Empty`，否则抛错 |
| 向量库    | Qdrant collection 命名 `faults_{tid:N}`，天然按租户分区                        |

MCP 端点 `/mcp` 经全局 `UseAuthentication` + `UseTenantContext` 中间件，JWT(`tid`) 注入 `ICurrentTenant`，工具内强隔离。

## 3. 代码结构

| 文件                           | 内容                                                                                                                                                                                           |
| ---------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `infra.cs`                   | `IFaultVectorStore` 抽象、`InMemoryFaultVectorStore`（内存兜底）、`QdrantFaultVectorStore`（租户分区 collection）、`FaultEmbedding`（确定性伪嵌入）、`PlcDiagnosisChatClient`（`IChatClient` 离线启发式回退）、`AddSaasAi` DI 扩展 |
| `slices/ai.assistant.cs`     | `AiAssistantApi`（诊断端点）、`PlcMcpTools`（`[McpServerTool]` 工具集）                                                                                                                                  |
| `Program.cs`                 | `#include` 新切片、`AddSaasAi`、`AiAssistantApi.Map`、`app.MapMcp("/mcp")`（按 `Mcp:Enabled`）                                                                                                        |
| `appsettings.json`           | `Ai` / `Qdrant` / `Mcp` 配置段                                                                                                                                                                  |
| `plc-saas.csproj` / `verify` | 新增 `Microsoft.Extensions.AI 10.9.0`、`ModelContextProtocol 2.2.0`、`ModelContextProtocol.AspNetCore 0.1.0-preview.14`、`Qdrant.Client 1.19.0`                                                   |

### 3.1 诊断端点（主路径）

`POST /api/v1/ai/diagnose`（需 `TenantUser` 鉴权，`Ai:Enabled` 门控）：

- 取设备 + 近期故障（EF，租户内）。
- 编码当前故障模式 → `IFaultVectorStore.SearchSimilarAsync` 检索同租户相似模式（RAG）。
- 构造问诊上下文 → `IChatClient.GetResponseAsync` → 返回诊断 + 相似模式。
- best-effort 将本次模式 `UpsertPatternAsync` 入库。

### 3.2 MCP 工具（外部 Agent 问诊）

`/mcp`（Streamable HTTP + SSE，仅 `Mcp:Enabled` 时暴露）：

- `list_devices`：当前租户设备列表。
- `get_faults`：当前租户故障事件（可按设备过滤）。
- `get_telemetry_summary`：指定设备遥测摘要。
- 任一工具未鉴权/跨租户访问直接拒绝。

## 4. 配置示例

```json
{
  "Ai":     { "Enabled": false, "Provider": "", "Model": "", "Endpoint": "", "ApiKey": "" },
  "Qdrant": { "Enabled": false, "Url": "http://localhost:6334", "ApiKey": "" },
  "Mcp":    { "Enabled": false }
}
```

- 默认全部关闭，编译/启动均不依赖外部 AI/Qdrant 服务。
- 开启 AI 诊断：设 `Ai:Enabled=true`；接入真实 LLM 时在 `AddSaasAi` 构造 `OpenAI/Azure/Ollama` 的 `IChatClient` 作为 `upstream`（预留接入点，未引入额外 Provider 包以保持 P1 蓝本轻量）。
- 开启向量库：设 `Qdrant:Enabled=true`（需本地/可达 Qdrant 实例，见 §6）。
- 开启 MCP：设 `Mcp:Enabled=true`，外部 Agent 连接 `http://localhost:5000/mcp`。

## 5. 门禁结果

- `dotnet build plc-saas` → **0 警告 0 错误**
- `dotnet test plc-saas.verify` → **6/6 通过，0 警告**
  - ADR-108 红线扫描（无 price/currency/amount/money）
  - 切片存在性（含 `ai.assistant.cs`）
  - 摄取契约（设备 Id / 遥测归一化）
  - **新增**：故障模式向量库租户隔离（`InMemoryFaultVectorStore` 跨租户不可见）
  - **新增**：`PlcDiagnosisChatClient` 离线启发式回退（无网络依赖）

## 6. 本地联调

> 注意：本沙箱 `registry-1.docker.io` **不可达**，无法 `wslc pull` 镜像；以下需在具 Hub 访问权的本机执行。

1. 起基础设施（wslc，不用 docker）：
   ```powershell
   pwsh plc-saas/start-dev-env.ps1   # PostgreSQL localhost:5432 postgres/postgres + EMQX 1883/18083
   ```
2. （可选）起 Qdrant：
   ```powershell
   wslc run -d --name plc-saas-qdrant -p 6334:6334 qdrant/qdrant:latest
   ```

3. 启动后端：
   ```bash
   dotnet run --project plc-saas/plc-saas.csproj
   ```
4. 开启能力：编辑 `appsettings.json` 设 `Ai:Enabled` / `Qdrant:Enabled` / `Mcp:Enabled` 为 `true`。
5. 诊断：
   ```bash
   curl -X POST localhost:5000/api/v1/ai/diagnose \
     -H "Authorization: Bearer <tenant-jwt>" \
     -H "Content-Type: application/json" \
     -d '{"deviceId":"<guid>","faultCode":"OVER_TEMP","severity":"high"}'
   ```
6. MCP：外部 Agent 连接 `http://localhost:5000/mcp`（携带租户 JWT）。

## 7. 已知约束 / 后续

- **Qdrant 元数据**：本版 `Qdrant.Client 1.19.0` 的 grpc `PointStruct.Payload` 为只读、无便捷构造函数，故 `QdrantFaultVectorStore` 用进程内 `_meta` 映射缓存 code/severity（已在代码注释标明）。**生产接入时应改存 `PointStruct.Payload`**，并移除 `_meta` 缓存。
- `QdrantClient.SearchAsync` 在本版本标记 Obsolete（建议 `QueryAsync`），当前以 `#pragma warning disable CS0618` 抑制并保持可用；后续可迁移到 `QueryAsync`。
- **真实 LLM 接入点**已在 `AddSaasAi` 预留（`upstream` 参数）；生产引入 `Microsoft.Extensions.AI.OpenAI` 等并在该处构造即可，无需改动调用方。
- 伪嵌入 `FaultEmbedding` 仅用于离线相似度演示；生产应替换为真实 `IEmbeddingGenerator`（如 text-embedding-3-small）以保证检索质量。
