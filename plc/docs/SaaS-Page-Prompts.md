# SaaS 页面提示词（Phase 2 · 前端可消费）

> 依据：SaaS-Spec.md §7 页面清单 / §5 API 端点 / §8 设计 Token + DESIGN.md §10（SSOT）
> 设计寄存器：Product（产品型）。MVP 单品牌，禁第二主色、禁紫→粉渐变、禁硬编码色、禁模板味文案、禁千篇一律 Hero。
> 图标：Lucide（`<svg class="ico"><use href="#i-xxx"/></svg>`），尺寸 16/20/24px，全程无 emoji。
> 复用组件：`.card` `.drawer` `.badge`(ok/warn/bad/info/tier) `.meter` `.kpi` `.tbl` `.seg` `.btn`(.btn-primary 用 `--brand-600`) `.switch`；玻璃态 `blur(14px) saturate(140%)`，暗色 `--bg #020617`。

---

## 0. 全局：租户切换器（顶栏常驻，所有 SaaS 页共享）

- **布局**：顶栏搜索左侧玻璃胶囊；左 `组织标`(`i-building-2`，`--tenant-dot`) + 当前租户名（等宽不强制）+ `展开标`(`i-chevrons-up-down`)；点击展开 `role="listbox"` 玻璃下拉（含搜索框 + 租户列表，已选标 `i-check`）。
- **切换为「高成本操作」**：二次确认弹窗(`role="alertdialog" aria-modal`) → 回首页 `#/` → 按新租户上下文重渲 → 内容区顶部常驻 `.scope-bar`「当前数据范围：<租户名>」(`--border-strong`)。
- **令牌**：`--tenant-dot`、`--border-strong`、`--surface-sunken`。图标：`i-building-2` `i-chevrons-up-down` `i-check`。
- **可达性**：下拉项 `aria-selected`；确认弹窗焦点陷入 + `Esc` 关闭；移动端置于侧栏抽屉内品牌下方（<1024px 可达）。
- **文案**：胶囊「当前组织：<名>」；确认「切换组织后将返回首页并刷新该组织数据，确定继续？」；边界条「当前数据范围：<名>（仅展示该组织数据）」。

---

## 1. 账户 · 用量概览  `#account/usage`

- **绑定 API**：`GET /api/v1/billing/usage`（设备数 / 遥测用量 / 配额剩余）；`GET /api/v1/billing/quota`（阈值与超限策略）。
- **布局结构**：页头（h1「用量与配额」+ 副标题「本组织当期资源消耗与配额剩余」）→ `.kpi-grid`（基准 1 列 / ≥576 两列 / ≥992 四列）4 张 KPI 卡（设备数 / 席位 / API 调用 / 存储）→ 下方一张主用量趋势 `.card`（折线 SVG，沿用 `--brand-*`）。
- **复用组件**：`.kpi`（28px 数值 + `.meter`）+ `.meter`(`role="progressbar" aria-valuenow aria-valuemax`) + `.card` + `.seg`（7天/30天/90天，激活 `--brand-600`）。
- **状态与空态**：
  - Loading：骨架屏占位（`.kpi` 高度占位 + `.spin`）。
  - Populated：每卡数值 + meter 文本「142 / 200 席位 · 71%」；阈值 ≥80% meter 加 `is-warn`（amber），≥95% `is-bad`（rose）。
  - Empty（新租户无设备）：引导卡「注册首台设备」（CTA 跳设备页），不空白报错。
  - Edge（超配额）：meter `is-bad` + 角标「已超限，按策略限流」。
- **令牌与 Lucide 映射**：`--brand-400`(正常填充) / `--warn` / `--bad` / `--quota-track`；`i-gauge`(用量) `i-bar-chart-3`(趋势) `i-building-2`(租户标)。
- **文案（真实语义，禁占位）**：页头「用量与配额」；KPI 标签「设备数 / 席位 / 遥测点数 / 存储」；趋势卡「用量趋势」；空态「本组织暂无设备，注册首台设备后即可查看用量」。

---

## 2. 账户 · 套餐与订阅  `#account/plan`

- **绑定 API**：`GET /api/v1/billing/plan`（当前档）；`POST /api/v1/billing/plan`（切换套餐档，免费档间，管理员）。
- **布局结构**：当前套餐卡（`.badge.tier`「专业版」+ 核心配额 meter 组 + CTA「升级到商业版以提升设备上限」）→ 套餐对比区 `.grid-4`（≥768 四列 / 移动 1 列）4 张 `.card`（免费/专业/商业/企业，含能力清单 `i-check` 含 / `i-lock` 不含），当前档高亮 `--brand-600` 边框 + 「当前方案」标。
- **复用组件**：`.badge.tier`（品牌蓝，文字区分档位，禁上色）、`.meter`、`.card.hover`、`.btn-primary`(CTA `i-arrow-up-right`)、`.seg` 或分段按钮做档位切换。
- **状态与空态**：Loading 骨架；Populated 正常；Success 切换后 inline toast「已切换至专业版」；Disabled（分析师角色）切换控件 `aria-disabled` + 提示「仅组织管理员可切换套餐」。
- **令牌与 Lucide 映射**：`--tier-badge-fg` / `--brand-600` / `--ok`；`i-credit-card` `i-arrow-up-right` `i-check` `i-lock`。
- **文案**：页头「套餐与订阅」；副标题「套餐档仅决定配额上限，内网免费使用」；CTA「升级到商业版以提升设备上限」；禁货币/价格字段（Spec §10 免费约束）。

---

## 3. 账户 · 成员管理  `#account/members`

- **绑定 API**：`GET /api/v1/tenant/members`（列成员与角色）；`POST /api/v1/tenant/members`（添加+分配角色，管理员）；`POST /api/v1/tenant/members/{id}/role`（改角色，四角色内）。
- **布局结构**：页头 + 右侧「邀请成员」`.btn-primary`(`i-plus`) → 打开右侧 `.drawer`（邮箱输入 + 角色 `.select`：管理员/分析师/操作员/设备工程师）→ `.tbl`（成员 / 角色(`.badge.info` 文字) / 状态(`--ok` 活跃) / 最近活跃(等宽时间) / 操作(`i-more-horizontal`→移除/改角色)）。
- **复用组件**：`.tbl`(`th scope`) + `.badge.info`(角色) + `.drawer`(`role=dialog aria-modal`) + `.select` + `.btn-primary` + `.btn-ghost`。
- **状态与空态**：Loading 骨架行；Populated 正常；Empty「本组织暂无其他成员，邀请同事协作」；Permission（非管理员，如分析师）表格只读 + 空态「你仅可查看成员，邀请需由组织管理员操作」。
- **令牌与 Lucide 映射**：`--ok` / `--info` / `--brand-600`；`i-users` `i-plus` `i-pencil` `i-more-horizontal` `i-shield-check`。
- **文案**：页头「成员与角色」；邀请按钮「邀请成员」；角色名「管理员 / 分析师 / 操作员 / 设备工程师」；权限空态「你仅可查看成员，邀请需由组织管理员操作」。

---

## 4. 账户 · 账单与发票  `#account/billing`

- **绑定 API**：本期无账单（Spec §7 标注「—」）；只读摘要可复用 `GET /api/v1/billing/usage` 回顾当期用量。**严禁货币/价格/发票链路**（Spec §3/§10：内网免费，只计量不收费，AC-10）。
- **布局结构**：页头「账单与发票」+ 说明条「计费功能上线前不生成账单，当前按用量计量免费使用」→ 只读用量回顾 `.card`（设备数/遥测点数/配额剩余，来自 usage）→ 空发票表区（`.tbl` 表头 账期/金额/状态/操作，body 显示「当前账期暂无账单」）。
- **复用组件**：`.scope-bar` 风格说明条 + `.card`(用量回顾) + `.tbl`(空态)。
- **状态与空态**：Empty（默认且正确态）「当前账期暂无账单」+ 说明「内网免费使用，计费上线后此处展示发票」；ReadOnly 整页 `aria-disabled` 提示无写入口。
- **令牌与 Lucide 映射**：`--surface-sunken` / `--text-2` / `--ok`；`i-receipt` `i-circle-check`。
- **文案（真实语义，禁占位）**：页头「账单与发票」；说明「计费功能上线前不生成账单，当前按用量计量免费使用」；空态「当前账期暂无账单」。

---

## 5. 运营 · 组织管理  `#ops/tenants`

- **绑定 API**：`GET /api/v1/ops/tenants`（全部租户+健康，超管）；`POST /api/v1/ops/tenants/{id}/suspend`（暂停）；`POST /api/v1/ops/tenants/{id}/resume`（恢复）。开通组织走落地页 `POST /api/v1/tenants`。
- **布局结构**：页头 + 「开通组织」`.btn-primary`(`i-plus`) → 编辑/开通走 `.drawer` → `.tbl`（组织名(`i-building-2` + `--tenant-dot`) / 套餐档(`.badge.tier` 文字) / 席位用量`.meter` / 状态(`--ok`活跃 / `--text-3`暂停 / `--warn`试用) / 续费日(等宽) / 操作(`i-pencil` `i-more-horizontal`)）；行内 暂停/恢复 用 `.switch` 或按钮（`i-ban` 暂停 / `i-circle-check` 恢复）。
- **复用组件**：`.tbl` + `.badge.tier` + `.meter` + `.drawer` + `.switch` + `.btn-primary` + `.btn-ghost`。
- **状态与空态**：Loading 骨架；Populated 正常；Confirm（暂停）二次确认弹窗「确认暂停该组织？暂停后其用户将无法登录，数据保留。」；Empty「尚无租户，点击开通组织创建首个租户」。
- **令牌与 Lucide 映射**：`--tenant-dot` / `--ok` / `--warn` / `--text-3` / `--brand-600`；`i-building-2` `i-plus` `i-pencil` `i-ban` `i-circle-check` `i-more-horizontal`。
- **文案**：页头「组织管理」；主操作「开通组织」；暂停确认「确认暂停该组织？暂停后其用户将无法登录，数据保留。」；恢复「恢复该组织访问」。

---

## 6. 运营 · 计费策略  `#ops/pricing`

- **绑定 API**：`GET /api/v1/ops/pricing`（套餐档定义 / 配额差异化）。**无价格字段**（Spec §6 `Plans` 无价格；§3 免费）。
- **布局结构**：页头「计费策略」+ 说明条「套餐档仅作配额差异化，不含价格（内网免费）」→ `.tbl`（档位(`.badge.tier` 文字 免费/专业/商业/企业) / 设备配额 / 遥测配额 / 能力开关）。MVP 策略以只读展示为主。
- **复用组件**：`.tbl`(`th scope`) + `.badge.tier` + `.scope-bar` 风格说明条。
- **状态与空态**：Loading 骨架；Populated 正常；ReadOnly（MVP 策略调参进 Backlog）整页只读提示。
- **令牌与 Lucide 映射**：`--tier-badge-fg` / `--text-2` / `--ok`；`i-credit-card` `i-gauge`。
- **文案**：页头「计费策略」；说明「套餐档仅作配额差异化，不含价格（内网免费）」；列头「档位 / 设备配额 / 遥测配额 / 能力」。

---

## 7. 运营 · 全局角色与权限  `#ops/roles`

- **绑定 API**：`GET /api/v1/ops/roles`（全局角色与权限矩阵）。
- **布局结构**：页头「全局角色与权限」+ 说明「四角色：管理员 / 分析师 / 操作员 / 设备工程师」→ 权限矩阵 `.tbl`：行=资源/动作（设备查看/指令下发/告警确认/成员管理/计费查看），列=四角色；单元格用 `i-check`（有权）/`i-minus`（无，禁纯色块）+ 文字 label；分析师只读行明确标记。
- **复用组件**：`.tbl`(矩阵) + `.badge.info`(角色) + 图标 `i-check` `i-minus`（配 `aria-label`「允许/不允许」）。
- **状态与空态**：Loading 骨架；Populated 正常；无空态（平台必有四角色）。
- **令牌与 Lucide 映射**：`--ok` / `--text-3` / `--info` / `--brand-600`；`i-shield-check` `i-key-round` `i-check` `i-minus`。
- **文案**：页头「全局角色与权限」；角色「管理员 / 分析师 / 操作员 / 设备工程师」；单元格 label「允许 / 不允许」；说明「分析师对设备指令 Tab 仅只读（AC-04）」。

---

## 8. 运营 · 审计日志  `#ops/audit`

- **绑定 API**：`GET /api/v1/ops/audit`（Backlog 启用，Spec §3/§7）。
- **布局结构**：页头「审计日志」+ `.scope-bar`「当前租户：<名>」(超管可切全局) → 搜索框 + 筛选 `.seg` → `.tbl`（组织 / 操作人 / 操作类型 / 时间(等宽) / 对象）。
- **复用组件**：`.tbl`(`th scope`) + `.search` + `.seg` + `.scope-bar`。
- **状态与空态**：Backlog 态（本期未启用）→ 真实说明空态「审计日志将于平台正式运营前启用，届时记录跨租户操作」；**非占位文案**。ReadOnly 提示「功能待启用」。
- **令牌与 Lucide 映射**：`--border-strong` / `--surface-sunken` / `--text-2`；`i-scroll-text` `i-search`。
- **文案**：页头「审计日志」；空态「审计日志将于平台正式运营前启用，届时记录跨租户操作便于追溯」。

---

## 9. 运营 · 平台健康  `#ops/health`

- **绑定 API**：`GET /api/v1/ops/health`（平台健康 SLO）。
- **布局结构**：页头「平台健康」+ 副标题「服务等级目标（SLO）」→ `.grid-4`/`.grid-2` 服务状态卡（API 网关 / 数据库 / 缓存 / 队列）：每张 `.card` 含服务名 + 语义色点(`.dot`：`--ok`正常/`--warn`降级/`--bad`故障) + 关键指标(延迟 p95 / 错误率，等宽) + 趋势 `i-activity`。
- **复用组件**：`.card` + 语义色点(复用既有 `.dot` 模式) + `.kpi`(指标) + `i-activity` `i-gauge`。
- **状态与空态**：Loading 骨架；Populated 正常；Degraded（部分 `--warn`/`--bad`）卡片高亮 `is-warn/is-bad` + 顶部汇总条「1 项服务降级」；无空态（平台恒有服务）。
- **令牌与 Lucide 映射**：`--ok` / `--warn` / `--bad` / `--brand-400`；`i-activity` `i-gauge` `i-circle-check` `i-alert-triangle`。
- **文案**：页头「平台健康」；副标题「服务等级目标（SLO）」；降级汇总「1 项服务降级，已触发告警」；指标标签「p95 延迟 / 错误率」。

---

## 跨页一致性清单（前端落地前核对）

1. 所有页头 h1 用 `--text`，副标题用 `--text-2`/`--text-3`（禁 `--text-3` 作正文）。
2. 状态「色 + 字」双信号：徽章用 `.badge.ok/warn/bad/info/tier`（AA 安全深色字），禁纯色块。
3. 配额 meter 必带 `role="progressbar"` + `aria-valuenow/max` + 文本值（AC-11）。
4. 抽屉 `role="dialog" aria-modal`，打开焦点移入、Esc 关闭；确认弹窗 `role="alertdialog"`。
5. 切换器/确认弹窗焦点陷入；全局 `:focus-visible` 2px 天蓝焦点环；`prefers-reduced-motion` 继承。
6. 移动优先：表格 <768px 转卡片堆叠，配额 meter 纵向；切换器 <1024px 在侧栏抽屉内。
7. 图标全 Lucide，尺寸 16/20/24px，无 emoji；文案真实中文业务语义，无 Welcome to / Lorem / Sign up today 之类。
8. 金额/价格字段本期一律不出现（免费约束，AC-10）；账单页仅用量回顾 + 空态说明。
