# 安全审查框架

> 版本：Sec v1.0 · 状态：已落地（CI `security` job + 本框架）
> 目标：多租户 SaaS 的安全左移——密钥、依赖、代码、隔离四层审查。

---

## 1. 审查层级与 CI 映射

| 层 | 工具 | CI 关卡 | 阻断规则 |
|----|------|---------|----------|
| 密钥 | gitleaks | `security` job | 发现密钥即红 |
| 依赖 CVE | `dotnet list package --vulnerable` + `npm audit` | `security` job | 有 Known Vulnerability 即红 |
| SAST | Semgrep（自定义规则） | `security` job | 命中高危规则即红 |
| 多租户隔离 | 人工 + 清单核查 | PR 审查 + 部署前 | 任一项不满足不发布 |

## 2. Semgrep 规则要点（自建，存入 `scripts/semgrep/`）

- **硬编码密钥**：`(password|connectionstring|apikey|secret)\s*=\s*"..."` 字面量。
- **SQL 注入**：字符串拼接进 `ExecuteSqlRaw`/`FromSqlRaw`（务必参数化；RLS 引导除外，已注释豁免）。
- **RLS 绕过**：禁止对租户表直接 `ExecuteSqlRaw` 且无 `SET app.tenant_id`；禁止 `BYPASSRLS` 角色用于请求路径。
- **tid 缺失**：新增端点/Handler 未从 `ICurrentTenant` 取租户即写 `ITenantEntity`（应经 EF 全局过滤器）。
- **跨租户写**：出现对 `ITenantEntity` 的写操作绕过 `TenantStampInterceptor`（应抛 `CrossTenantWriteException`）。

## 3. 多租户隔离审查清单（PR 必勾）

- [ ] 所有 `ITenantEntity` 查询经 EF 全局查询过滤器（无 `IgnoreQueryFilters` 绕过）。
- [ ] PostgreSQL RLS 策略存在且 `SET app.tenant_id` 由 `TenantConnectionInterceptor` 每连接设置。
- [ ] JWT `tid` 声明由 `TenantMiddleware` 校验并注入 `ICurrentTenant`（Scoped）。
- [ ] SignalR `/hubs/faults` 连接鉴权且消息按 `tid` 作用域分发。
- [ ] 缓存键前缀 `tenant:{tid}:{key}`，后台作业 payload 显式带 `tid` 并 restore。
- [ ] 订阅 `Suspended` 时写阻 409、读降级、零删除。
- [ ] ADR-108：Billing 路径无 `price/currency/amount/money` 标识符。

## 4. AuthN / AuthZ

- [ ] JWT 校验：签名/过期/`tid` 存在；access 15min / refresh 7d。
- [ ] 四角色（超管/租户管理员/运维/分析师）RBAC 经 ASP.NET Identity 角色声明。
- [ ] 跨租户/分析师只读/非超管操作返回 403（openapi.yaml 已标注）。

## 5. OWASP Top 10 映射

| 风险 | 缓解 |
|------|------|
| A01 失效访问控制 | RLS + EF 过滤器 + tid 中间件 + 角色 403 |
| A02 加密失败 | TLS（YARP/入口）+ JWT 签名 |
| A03 注入 | 参数化查询 + Semgrep 规则 |
| A05 安全配置错误 | 密钥扫描 + 依赖 CVE + 默认关 AI flag |
| A07 认证失效 | JWT 短期 + refresh 轮换 + refresh_tokens 表 |
| A09 日志监控不足 | 审计 `audit_logs`（平台级）+ SignalR 实时告警 |

## 6. 频率限制 / 配额

- 全局限流（YARP/中间件）喂给配额 429；设备注册超配额 409；遥测背压（见 `SaaS-Metering.md`）。
