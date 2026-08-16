# 代码 CoreReview 框架

> 版本：CR v1.0 · 状态：已落地（PR 模板 + CODEOWNERS + `scripts/review-gates.sh`）
> 目标：把 P0 绝对规则与多租户隔离审查固化为「人工 + 自动」双关卡。

---

## 1. 流程

1. **开 PR** → 自动触发 CI（`build-test` / `security` / `review-gates`）。
2. **自动门禁**：`review-gates.sh` 拦截 emoji/硬编码色/货币红线/紫粉渐变；`security` 拦截密钥/依赖 CVE/SAST。
3. **必需审批**：`CODEOWNERS` 指定模块负责人（后端/前端/架构/DevOps）至少 1 人 approve。
4. **架构评审**：任何新增横切关注点（新中间件/新隔离层/新传输）**必须先提 ADR**，PR 关联 ADR 编号。
5. **合并前清单**：PR 模板全勾，关联测试通过。

## 2. P0 自动检查清单（CI 执行，详见 `review-gates.sh`）

- [ ] 无 emoji 功能图标（Lucide `<use href="#i-xxx"/>` 除外）
- [ ] 无硬编码色（用 Design Tokens，`var(--*)`）
- [ ] Billing 路径无 `price/currency/amount/money`（ADR-108）
- [ ] 无紫→粉渐变
- [ ] 货币红线权威校验在 `plc-saas.verify` 测试（2/2）

## 3. 人工审查清单（PR 模板）

- [ ] 多租户隔离：EF 全局过滤器 / RLS / `tid` 中间件（见 `Security-Review.md` 清单）
- [ ] 新增横切关注点已提 ADR
- [ ] 已加/更新测试（`dotnet test plc-saas.verify` 本地通过）
- [ ] 文档同步（架构图/规格/ADR 随代码变更更新）

## 4. 分级

- **核心**（隔离/认证/计费/摄取）：需 2 审批 + 架构师 ratify。
- **普通**（页面/端点/样式）：1 审批即可。

## 5. 违规处置

- CI 红：禁止合并，修复后重跑。
- 绕过关卡（如 `--no-verify`）：视为严重违规，回退并记入审计。
