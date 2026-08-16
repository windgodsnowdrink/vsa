## 变更摘要
<!-- 本 PR 做了什么 -->

## 关联
- 关联 ADR / Issue：

## P0 自动门禁（CI 执行，人工确认）
- [ ] 无 emoji 功能图标（Lucide `<use>` 除外）
- [ ] 无硬编码色（用 Design Tokens `var(--*)`）
- [ ] Billing 路径无 `price/currency/amount/money`（ADR-108）
- [ ] 无紫→粉渐变

## 多租户隔离（见 Security-Review.md 清单）
- [ ] 查询经 EF 全局过滤器 / RLS / `tid` 中间件
- [ ] SignalR `/hubs/faults` 按 `tid` 作用域分发
- [ ] 新增横切关注点已提 ADR

## 测试
- [ ] `dotnet test plc-saas.verify` 本地通过
- [ ] 前端 `node --check wwwroot/js/*.js` 通过

## 文档同步
- [ ] 架构图 / 规格 / ADR 随代码更新
