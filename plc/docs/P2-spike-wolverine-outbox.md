# P2 Spike：Wolverine Outbox 采用评估（ADR-110）

> 状态：Design Spike（未落地代码） · 阶段：P2/Later · 关联：ADR-102（自建 Outbox）、ADR-110（Deferred→P2）、ADR-113（真实 EMQX）
> 目的：在真正替换自建 Outbox 前，把迁移成本 / 风险 / 收益讲清，定出"何时采用"的客观触发线，避免盲目 churn 已跑通的系统。

## 1. 现状（baseline）
- **CQRS 派发**：Mediator(MIT) 源生成 `[EventHandler]`，已在用且测试通过。
- **事务 Outbox**：`OutboxMessages` 表 + `MeteringAgent` 轮询消费者（ADR-102）；当前 `dotnet test` 全绿（6/6）。
- **对外传输**：ADR-113 已接真实 EMQX（HTTPS 摄取端点为主路径）；MQTTnet 边缘中继可选。

## 2. 拟采用方案（若采纳）
- 引入 Wolverine 的 **EF Core 事务 Outbox**：与 `SaveChanges` 同事务自动派发，消除手搓轮询 Agent + 调度逻辑。
- CQRS 派发**仍用 Mediator**，不替换（Wolverine 可管 Outbox，但不必接管 Handler）。
- 消息语义：at-least-once + inbox 去重（Wolverine 内置），替代当前"轮询 + 幂等由业务保证"。

## 3. 迁移与风险
- **依赖面**：新增 Wolverine（及其 EF Core 集成）。需确认版本兼容 **EF Core 11 preview / Npgsql 11 preview / .NET 11 preview** —— 这是最大不确定项，先验证再定。
- **Schema 迁移**：现有 `OutboxMessages` 表结构 + `MeteringAgent` 消费者代码整体替换；历史未派发消息需一次性排空或双写过渡。
- **行为变化**：由"定时轮询"变"提交即派发"，对下游（EMQX / SignalR / InfluxDB 写入）的幂等要求不变，但时序更紧。
- **回滚**：Wolverine 与 Mediator 并存期较长，回滚需保留旧 Outbox 表一段时间。

## 4. 采用触发线（客观判据）
满足任一即建议启动正式落地：
1. 需要**成熟 inbox / 死信 / 重试调度**（当前手搓无）；
2. 需要**延迟 / 定时消息**（如计费账单定时派发）；
3. 轮询 Agent 维护成本显著上升或成瓶颈（当前未到）。
仅"接了真实 EMQX"（ADR-113）**不构成**采用理由 —— 那只是让消息有去处，不要求换 Outbox 引擎。

## 5. 开放问题
- Wolverine 在 .NET 11 preview 栈的可用版本与已知坑？
- 是否与 Mediator 长期并存，还是最终统一到 Wolverine 派发？

## 6. 建议
**暂不采用**。当前自建 Outbox 运行正常、测试通过、零额外依赖。P2 保留，待上述触发线任一命中时再启动落地（届时本 Spike 升格为实施方案）。
