# DESIGN.md — PLC·AIOT 设计系统

> PLC 边缘设备与 AIOT 物联网平台的产品级设计系统规范。
> 风格：现代简约 · **科技天蓝** · **玻璃态（Glassmorphism）** · 亮/暗双主题。
> 设计影响：Linear.app 的暗色玻璃密度 + Stripe 的色彩角色精度，落地为工业控制中枢自有视觉。
> 适配策略：**移动优先（Mobile-First）**，从 320px 渐进增强；无障碍基线 **WCAG 2.2 AA**。
> 本文件为 AI 编程代理可直接消费的单一事实来源（Single Source of Truth）。

---

## 1. Visual Theme & Atmosphere（视觉主题与氛围）

**品牌设计哲学**：为「工业物联网控制中枢」而生——在高度数据密集的监控场景下，保持精密、可信、克制的观感；用玻璃态让多层信息浮于深色环境之上，降低视觉噪声。

**视觉基调**：技术可信感（industrial-tech）、通透、冷静、信息优先。

**核心视觉特征关键词**：
1. **科技天蓝** — 唯一主色，贯穿品牌、交互、数据高亮。
2. **玻璃态通透** — 半透明表面 + 背景光晕，制造纵深而不喧宾夺主。
3. **暗色优先** — 默认暗色环境（`#020617`），亮色为无障碍/外勤兜底。
4. **数据密度** — 紧凑栅格、等距对齐、信息分组清晰。
5. **克制精致** — 微阴影、细腻边框、克制的动效（≤300ms）。

**光影与质感倾向**：毛玻璃（`backdrop-filter: blur(14px) saturate(140%)`）；深色环境采用双径向光晕（右上天蓝 + 左上来蓝）模拟设备舱氛围光；表面用 1px 低透明边框替代重阴影，保持轻盈。

---

## 2. Color Palette & Roles（调色板与角色）

### Primary Colors（主色 — 科技天蓝）
| 角色 | HEX | CSS 变量 | 使用场景 |
| --- | --- | --- | --- |
| Brand 400 | `#60a5fa` | `--brand-400` | 浅色态交互、图表辅助线 |
| Brand 500 | `#3b82f6` | `--brand-500` | 强调色块、图标、进度条（**不作白字底色**） |
| **Brand 600** | `#2563eb` | `--brand-600` | **主按钮底色 / 激活态 / 分段控件选中**（白字 AA 达标） |
| Brand 700 | `#1d4ed8` | `--brand-700` | 主按钮 hover、按压态 |
| Brand 900 | `#1e3a8a` | `--brand-900` | 暗色环境光晕核心 |

### Brand & Dark（品牌暗变体）
| 角色 | 值 | CSS 变量 | 说明 |
| --- | --- | --- | --- |
| 暗色背景 | `#020617` | `--bg`(.dark) | 应用根背景 |
| 暗色表面 | `rgba(15,23,42,0.55)` | `--surface`(.dark) | 玻璃卡片在暗色态的底 |
| 暗色实底 | `#0f172a` | `--surface-solid`(.dark) | 抽屉/对话框实底 |

### Accent / Interactive（强调 / 交互）
| 角色 | HEX | 使用场景 |
| --- | --- | --- |
| Indigo 渐变 | `#6366f1` | 头像、品牌 Logo 渐变收尾 |

### Neutral / Gray Scale（中性灰阶）
| 角色 | 亮色 HEX | 暗色 HEX | CSS 变量 |
| --- | --- | --- | --- |
| 文本主色（AA ✅） | `#0f172a` | `#e2e8f0` | `--text` |
| 文本次色（AA ✅） | `#475569` | `#94a3b8` | `--text-2`（**标签/次级文字用此**） |
| 文本弱色（装饰 ❌） | `#94a3b8` | `#64748b` | `--text-3`（仅装饰/禁用/图表轴，禁作正文） |

### Surface & Borders（表面与边框）
| 角色 | 亮色 | 暗色 | CSS 变量 |
| --- | --- | --- | --- |
| 表面（玻璃） | `rgba(255,255,255,0.72)` | `rgba(15,23,42,0.55)` | `--surface` |
| 实底 | `#ffffff` | `#0f172a` | `--surface-solid` |
| 边框 | `rgba(15,23,42,0.08)` | `rgba(148,163,184,0.14)` | `--border` |

### Semantic Colors（语义色）
| 角色 | HEX | CSS 变量 | 使用场景 |
| --- | --- | --- | --- |
| 在线 / 成功 | `#10b981` | `--ok` | 设备在线、正向趋势、确认（大号/UI ≥3:1） |
| 预警 / 警告 | `#f59e0b` | `--warn` | 阈值预警、待处理 |
| 告警 / 错误 | `#f43f5e` | `--bad` | 故障、负向趋势、危险操作 |
| 信息 | `#64748b` | `--info` | 中性状态、占位 |

### Shadow Colors（阴影色）
| 角色 | 亮色 | 暗色 | CSS 变量 |
| --- | --- | --- | --- |
| 主阴影 | `0 10px 30px -12px rgba(15,23,42,0.25)` | `0 18px 40px -18px rgba(0,0,0,0.7)` | `--shadow` |
| 轻阴影 | `0 4px 14px -8px rgba(15,23,42,0.3)` | `0 6px 18px -10px rgba(0,0,0,0.6)` | `--shadow-sm` |
| 焦点环 | `rgba(59,130,246,0.35)` | `rgba(96,165,250,0.45)` | `--ring` |

### WCAG 2.2 AA 对比度核对（Foreground / Background）
| 前景 | 背景 | 比值 | 判定 | 用途 |
| --- | --- | --- | --- | --- |
| `--text` `#0f172a` | 表面(≈#fff) | ~15:1 | ✅ | 标题/正文 |
| `--text-2` `#475569` | 表面(≈#fff) | ~7.0:1 | ✅ | 标签/次级文字 |
| `#94a3b8` | 表面 | ~2.8:1 | ❌ | 仅装饰/图表轴 |
| `#e2e8f0`(.dark) | `#0f172a` | ~13:1 | ✅ | 暗色正文 |
| `#64748b`(.dark) | `#0f172a` | ~3.9:1 | ❌ | 仅装饰/禁用 |
| 白字 | `--brand-600` `#2563eb` | ~4.6:1 | ✅ | 主按钮 |
| 白字 | `--brand-500` `#3b82f6` | ~3.6:1 | ❌ | 不可作白字底 |
| 徽章文字 `#047857/#b45309/#be123c/#1d4ed8` | 各自 18% 淡底 | 4.9–6.3:1 | ✅ | 语义徽章 |

### AA 安全徽章文字色（淡底 + 深色字，均 ≥4.5:1）
```css
.badge.ok   { color:#047857; } .badge.warn { color:#b45309; }
.badge.bad  { color:#be123c; } .badge.info { color:#1d4ed8; }
.badge.muted{ color:var(--text-2); }
```

---

## 3. Typography Rules（排版规则）

**Font Family**
- 主字体：`"Segoe UI", "Microsoft YaHei", system-ui, -apple-system, sans-serif`
- 等宽：`"JetBrains Mono", ui-monospace, "SFMono-Regular", Menlo, monospace`（代码 / 设备 ID / 主题路径）
- **字体上限：最多两种**（无衬线 + 等宽），禁止引入第三字族。

**Type Scale（8px 模块化刻度，12–48px）**
| 层级 | 字号 | 字重 | 行高 | 字距 | 用途 |
| --- | --- | --- | --- | --- | --- |
| Display | 40px | 800 | 1.05 | -0.02em | Hero / 大屏标题（可选） |
| Page Title | 24px | 800 | 1.1 | -0.01em | 页面头 `<h1>` |
| KPI Value | 28px | 800 | 1.0 | 0 | KPI 大数字 |
| Section Title | 15px | 700 | 1.3 | -0.01em | 卡片标题 `<h3>` |
| Body | 14px | 400 | 1.55 | 0 | 正文、表格、列表（AA 文本） |
| Caption | 12px | 500 | 1.4 | 0 | 标签、副标题（用 `--text-2`） |
| Nano | 11px | 600 | 1.2 | 0 | 徽章、时间戳（装饰元数据） |
| Mono | 11px | 400 | 1.3 | 0 | 代码、设备拓扑路径 |

**设计哲学**：以 `-0.01em` 微负字距收紧大字号（标题/KPI），提升工业仪表的精密感；正文保持 1.55 行高保障可读性；所有标题 `font-weight ≥ 700`，绝不依赖颜色 alone 传递层级。**对比度基线**：正文/标签 ≥ 4.5:1（用 `--text`/`--text-2`），大字号与 UI 组件 ≥ 3:1。

---

## 4. Component Stylings（组件样式）

### Buttons（按钮）
```css
.btn          { font-size:13px; font-weight:600; padding:9px 14px; border-radius:11px;
                transition: background .2s, transform .2s, box-shadow .2s; }
.btn-primary  { background:#2563eb; color:#fff; box-shadow:0 8px 18px -8px #2563eb; } /* brand-600，AA 达标 */
.btn-primary:hover { background:#1d4ed8; transform:translateY(-1px) scale(1.02); }
.btn:active   { transform:scale(.97); }
.btn-ghost    { background:var(--surface); border:1px solid var(--border); color:var(--text); }
.btn-ghost:hover  { background:var(--brand-50, #eff6ff); }
```
变体：Primary / Ghost / Danger；圆角 11px；hover 做 1px 上移 + 2% 放大，active 收缩 3%；动效 ≤200ms。

### Cards（卡片 — 玻璃态）
```css
.card { background:var(--surface); backdrop-filter:blur(14px) saturate(140%);
        border:1px solid var(--border); border-radius:16px;
        box-shadow:var(--shadow-sm); padding:18px; }
.card.hover:hover { transform:translateY(-3px) scale(1.01); box-shadow:var(--shadow); }
```
圆角 `--radius:16px`，内边距 18px；`.hover` 卡片悬浮抬升 3px + 1% 放大。

### Inputs（输入）
```css
.search input, .select {
  padding:10px 14px; border-radius:12px; border:1px solid var(--border);
  background:var(--surface-solid); color:var(--text); font-size:14px; outline:none; }
.search input:focus { border-color:#60a5fa; box-shadow:0 0 0 3px var(--ring); }
```
聚焦态：边框转 `--brand-400` + 3px 焦点环（`--ring`）；所有输入均可通过键盘聚焦且焦点可见。

### Navigation（导航）
```css
.nav a        { padding:11px 14px; border-radius:12px; color:var(--text-2); font-size:14px; }
.nav a:hover  { background:var(--brand-50); color:var(--text); }
.nav a.active { color:#2563eb; background:linear-gradient(90deg,
                color-mix(in srgb,#3b82f6 16%,transparent), transparent);
                border-left:3px solid #3b82f6; }
```
激活态 = 左侧 3px 天蓝条 + 左向渐变底 + `aria-current="page"`，绝不靠纯色块区分。

### Badges / Tags（徽章）
```css
.badge       { font-size:11px; padding:3px 9px; border-radius:999px; font-weight:600; }
.badge.ok    { background:color-mix(in srgb,var(--ok) 18%,transparent);   color:#047857; }
.badge.warn  { background:color-mix(in srgb,var(--warn) 18%,transparent); color:#b45309; }
.badge.bad   { background:color-mix(in srgb,var(--bad) 18%,transparent);  color:#be123c; }
.badge.info  { background:color-mix(in srgb,var(--brand-500) 18%,transparent); color:#1d4ed8; }
```
语义色徽章统一用 `color-mix(... 18%, transparent)` 淡底 + **AA 安全深色文字**，文字可独立读。

### Modals / Dialogs（对话框 — 右侧抽屉）
```css
.mask   { position:fixed; inset:0; background:rgba(2,6,23,.45); z-index:50;
          opacity:0; transition:opacity .25s; }
.drawer { position:fixed; top:0; right:0; height:100%; width:min(440px,92vw); z-index:55;
          transform:translateX(100%); transition:transform .28s ease;
          background:var(--surface-solid); border-left:1px solid var(--border); }
.drawer.open { transform:translateX(0); }
```
设备详情用右侧抽屉（`role="dialog" aria-modal="true" aria-labelledby`），宽度 `min(440px, 92vw)`，`.28s` 滑入；遮罩 `.25s` 淡入；打开时焦点移入关闭键，Esc 关闭。

---

## 5. Layout Principles（布局原则）

**Spacing System（间距基数 8px）**：`--gap:16px`、`--radius:16px`、`--radius-sm:10px`、内容内边距随断点 14→18→22px；组件内距多用 8 的倍数。

**Grid System（栅格）**
| 类 | 移动端(基准) | ≥992px |
| --- | --- | --- |
| `.kpi-grid` | `1fr` | `repeat(4, 1fr)`（≥576 → 2 列） |
| `.grid-2` | `1fr` | `2fr 1fr` |
| `.grid-3` | `1fr` | `repeat(3, 1fr)`（≥768 → 3 列） |
| `.grid-4` | `1fr` | `repeat(4, 1fr)` |
| `.dev-grid` | `repeat(auto-fill, minmax(240px,1fr))` | 同（免媒体查询自适应） |

**Container**：`--sidebar-w:248px`（桌面常驻）、`--topbar-h:64px`（顶栏高）；主区 `flex:1; min-width:0`。

**Section Spacing**：`.content` 用 `display:flex; flex-direction:column; gap:18px` 统一区块间距。

**留白哲学**：玻璃卡片之间留 16px 呼吸；页面头 `h1(24px)` 与副标题（13px 弱色）形成主次；图表区内边距 18px 保证数据与边界不贴边。

---

## 6. Depth & Elevation（深度与层级）

**Shadow System（多层阴影）**
| 层级 | 亮色 box-shadow | 暗色 box-shadow |
| --- | --- | --- |
| shadow-sm | `0 4px 14px -8px rgba(15,23,42,0.3)` | `0 6px 18px -10px rgba(0,0,0,0.6)` |
| shadow | `0 10px 30px -12px rgba(15,23,42,0.25)` | `0 18px 40px -18px rgba(0,0,0,0.7)` |
| 主按钮 | `0 8px 18px -8px #2563eb` | 同左（品牌投影） |

**Surface Layers（表面层级）**
1. `bg` — 应用根背景 + 双径向光晕
2. `surface` — 玻璃卡片（半透明 + blur）
3. `surface-solid` — 抽屉 / 对话框实底
4. `overlay` — 遮罩 / 通知浮层（最上）

**Z-index Scale**
| 元素 | z-index |
| --- | --- |
| 提示 tip | 60 |
| 抽屉 drawer | 55 |
| 遮罩 mask | 50 |
| 通知 notif | 45 |
| 侧栏 sidebar | 40 |
| 侧栏遮罩 overlay | 39 |
| 顶栏 topbar | 30 |

**Backdrop Effects**：`backdrop-filter: blur(14px) saturate(140%)`（玻璃态统一参数）；暗色态保留 blur 同时降低表面不透明度以凸显背景光晕。

**Micro-interactions（微交互）**
- **仅动画 `transform` 与 `opacity`**（GPU 加速），禁止动画 `width/height/background-position/layout`。
- **Hover**：按钮 `scale(1.02)` + `translateY(-1px)`；图标按钮 `scale(1.05)`；卡片 `scale(1.01)` + `translateY(-3px)`；时长 `.2s ease`。
- **Active/Press**：按钮 `scale(.97)`、图标按钮 `scale(.96)`；时长 `.12s`。
- **Focus**：`:focus-visible` 显示 2px 天蓝焦点环（`outline-offset:2px`），键盘可达性。
- **加载**：骨架屏占位 + `.spin` 旋转；成功/错误以 badge / 内联反馈呈现，不阻塞主流程。
- **页面切换**：`.view-anim` 淡入（`.28s`）；遵守 `prefers-reduced-motion` 全局降级。

---

## 7. Do's and Don'ts（设计规范与禁忌）

**Do's**
1. 所有页面共用 `--brand-*` 与 `--surface` 令牌，禁止硬编码色值（仅语义色可直写）。
2. 暗色为默认主题；切换主题只换 `.dark` 类，不重写组件样式。
3. 卡片一律用玻璃态 + `--radius:16px`，保持表面语言统一。
4. 状态传达用「颜色 + 文字/图标」双重信号（无障碍，徽章文字用 AA 安全深色）。
5. 操作深度 ≤ 3 层（驾驶舱 → 模块 → Tab）；导航路径统一。
6. **移动优先**：基础样式按 320px 单列编写，再用 `min-width` 渐进增强。
7. 交互反馈 ≤ 300ms，用 transform/opacity，避免 layout 抖动。
8. **键盘可达**：所有交互元素可 Tab 抵达，焦点环可见，抽屉可 Esc 关闭。

**Don'ts**
1. 不要用纯红/纯绿大色块强调，语义色仅用于徽章/状态点/趋势箭头。
2. 不要引入第二主色——除 indigo 头像渐变外，禁止其他品牌色喧宾夺主。
3. 不要在暗色态用白色实底卡片（破坏玻璃纵深）。
4. 不要使用重投影（`box-shadow` 模糊 > 40px），保持轻盈克制。
5. 不要用 `box-shadow` 做边框替代——边框统一用 1px `--border`。
6. 不要用 `--text-3` 渲染正文/标签（对比度不达标），它仅用于装饰/图表轴。
7. 不要用 `--brand-500` 作白字按钮底色（对比度 3.6:1 不达标），主按钮用 `--brand-600`。
8. 不要为动效使用 `ease-in` 长缓动，统一 `ease` 且 ≤300ms；且勿动 layout 属性。

---

## 8. Responsive Behavior（响应式行为 · 移动优先）

**断点（Min-Width 渐进增强）**
| 断点 | 范围 | 关键变化 |
| --- | --- | --- |
| 基准 320 | `< 576px` | 单列；侧栏为固定抽屉（汉堡唤出）；KPI 1 列；`#topNav` 横向滚动导航；content 14px |
| 576 | `576–767px` | KPI → 2 列 |
| 768 | `768–991px` | `grid-3` → 3 列；content 18px；search 360px |
| 992 | `992–1023px` | KPI → 4 列；`grid-2`(2fr1fr)/`grid-4` → 4 列；content 22px；search 460px |
| 1024 | `≥ 1024px` | 侧栏 `sticky` 常驻；汉堡隐藏；`#topNav` 隐藏；桌面完整布局 |
| 1200 | `≥ 1200px`（可选） | 容器最大宽约束（当前未强制） |

**策略**：基础 CSS = 移动端；所有增强用 `@media (min-width: N)`。侧栏在 <1024px 为 `translateX(-100%)` 抽屉，≥1024px 切回 `sticky` 常驻。

**Touch Targets**：图标按钮 `.icon-btn` 40×40px；导航项 `11px 14px` padding；徽章/标签 ≥ 28px 高；满足指尖命中。

**折叠策略**
- `< 1024px`：`.sidebar` 固定抽屉（`translateX(-100%)` → `.open` 归位），配 `.side-overlay` 遮罩；`.hamburger` 显示；`#topNav` 横向滚动兜底。
- `< 576px`：KPI 单列，搜索框去最大宽限制占满顶栏。

**Font Scaling**：标题/KPI 在移动端保持 24/28px（仪表核心信息不缩）；正文 14px 恒定；仅 padding 与栅格列数随断点变化，避免字号抖动。

**无障碍与 WCAG 2.2（贯穿响应式）**
- **对比度**：正文/标签 ≥ 4.5:1（用 `--text`/`--text-2`）；大字号与 UI 组件 ≥ 3:1（brand-500 色块、语义状态点）。
- **键盘**：全局 `:focus-visible` 2px 天蓝焦点环；`.skip-link` 跳到 `#view` 主内容；抽屉打开聚焦关闭键、Esc 关闭。
- **语义 & ARIA**：语义标签（`aside`/`nav`/`header`/`main`）；激活导航 `aria-current="page"`；抽屉 `role="dialog" aria-modal aria-labelledby`；遮罩 `aria-hidden`。
- **状态双信号**：徽章「淡底 + AA 安全深色字」，绝不只靠颜色区分。
- **动效降级**：`@media (prefers-reduced-motion: reduce)` 全局关闭动画/过渡。

---

## 10. SaaS 多租户层设计扩展（新增 · 升级为多租户 SaaS）

> 本节在既有「科技天蓝 / 玻璃态 / 亮暗双主题」系统之上，定义多租户 SaaS 层（Tenant + Identity + Billing）的设计语言。
> 设计寄存器：**Product（产品型）**——设计服务于控制台本身，色彩克制（单一强调色 ≤10%），无衬线为主，功能性动效（≤300ms），以数据可视化 / 图标 / UI 元素替代照片。
> 对标：Datadog（高密度暗色数据控制台）、Grafana/Saga（暗色可观测性标准）、Linear（暗色玻璃密度）、Samsara（工业 IoT 资产/车队控制台）。

### 10.1 图标系统锁定（P0 合规 · 禁止 emoji）

**锁定库：Lucide（ISC 许可，24×24 网格，2px 描边，`currentColor`，可 tree-shake）。**
理由：2026 年 SaaS 默认图标集（shadcn/ui、Vercel、Resend 同源），源自 Feather，线条克制微圆角，与本系统「克制精致」气质一致；`currentColor` 让它随 `--text/--text-2/--brand-*` 自动换色，零硬编码。

**交付形态（适配无构建静态 wwwroot）**：生成 `wwwroot/css/icons.svg` 雪碧图 `<symbol>`，页面用 `<svg class="ico"><use href="#i-xxx"/></use></svg>` 引用。仅内联本系统实际用到的子集（约 60–80 个），离线可用，与既有 `vendor/` 本地依赖哲学一致。

**尺寸规范（全项目统一，禁止混用其他库）**：
| 用途 | 类名 | 尺寸 | 描边 |
| --- | --- | --- | --- |
| 行内/列表图标 | `.ico-sm` | 16px | 2px |
| 按钮内/导航项 | `.ico` | 20px | 2px |
| 独立图标/空状态 | `.ico-lg` | 24px | 2px |

**既有 emoji → Lucide 替换映射（阻塞项，前端迁移前必须清除）**：
| 现有 emoji | 位置 | 替换为 Lucide |
| --- | --- | --- |
| ☰ | 汉堡菜单 | `Menu` |
| 🔍 | 搜索放大镜 | `Search` |
| 🌗 | 主题切换 | `Sun-Moon` |
| 🔔 | 通知铃 | `Bell`（未读态 `BellRing` + `.ping`） |
| ✕ | 关闭按钮 | `X` |
| 🔴/🟡/🤖 | 通知状态点 | 改用 `.badge.ok/.warn/.info`（已是 AA 安全淡底+深色字，删 emoji） |
| 🛰️ | 驾驶舱 | `LayoutDashboard` |
| 🏭 | 设备 | `Cpu` |
| 🔔 | 告警 | `BellRing` |
| 📡 | 实时看板 | `Activity` |
| 🗺️ | 地图 | `Map` |
| 📊 | 统计 | `BarChart3` |
| 📣 | 社媒矩阵 | `Megaphone` |
| 📋 | 团队协作 | `Kanban` |
| ⚙️ | 系统 | `SlidersHorizontal` |
| 🧩 | 后端管理 | `Boxes` |

**SaaS 新增导航/功能图标**：租户 `Building-2` · 计费 `CreditCard` · 角色权限 `ShieldCheck`/`KeyRound` · 配额 `Gauge` · 用户 `Users` · 切换器展开 `ChevronsUpDown` · 已选 `Check` · 发票 `Receipt` · 升级 `ArrowUpRight` · 暂停 `CircleSlash` · 激活 `CircleCheck` · 新建 `Plus` · 编辑 `Pencil` · 更多 `MoreHorizontal` · 锁 `Lock` · 钥匙 `KeyRound`。

### 10.2 SaaS 层新增页面 / 组件设计方向（独立分组，不混入既有三组）

> **导航分组硬约束（来自 PM + UX-ARCHITECTURE §6）**：SaaS 管理面作为**独立分组**，不与「运营监控 / 分析洞察 / 系统治理」三组混排。按受众分两类：
> - **账户（租户管理员）**：用量概览 · 套餐与订阅 · 成员管理 · 账单与发票。
> - **运营后台（平台超管）**：组织管理 · 计费策略 · 全局成员与角色 · 审计日志 · 平台健康。
> 仍遵守：操作 ≤3 层（分组 → 页 → 抽屉 Tab）、单一入口（租户切换器为唯一上下文开关）、状态可恢复（hash 深链 + `ctx`）。

| 模块 | 角色 | 设计方向 |
| --- | --- | --- |
| **租户切换器 Tenant Switcher** | 全局控件 | 顶栏搜索左侧常驻玻璃胶囊，左 `Building-2` 标（用 `--tenant-dot` 品牌色点）+ 当前租户名 + `ChevronsUpDown`；点击展开 `role="listbox"` 玻璃下拉（含搜索/已选 `Check`）。切换为「高成本操作」：二次确认弹窗 → 回首页 `#/` → 按新租户上下文重渲 → 内容区顶部常驻「当前数据范围：<租户名>」边界条（`--border-strong`）。移动端置于抽屉内品牌下方。 |
| **租户管理 Tenants**（平台管理员） | 平台治理 | 表格：租户名(`Building-2` 标，用 `--tenant-dot` 品牌色点) / 套餐档徽章(`.badge.info` 品牌蓝，文字区分 免费/专业/商业/企业) / 席位用量 meter / 状态徽章(active=`--ok` / suspended=`--text-3` / trial=`--warn`) / 续费日 / 操作(`Pencil` `MoreHorizontal`)。新建/编辑走右侧 `.drawer`（`role=dialog`）。激活-暂停用 `Switch` 开关（`--ok`/`--text-3`）。 |
| **订阅计费 Billing** | 租户管理员 | 当前套餐卡（套餐档徽章 `.badge.info` 品牌蓝 + 核心配额 meter 组）+ 套餐对比（4 档 free/pro/biz/ent，卡片内含能力清单 `Check`/`Lock`；档位用文字区分，不按档位上色）+ 用量超标预警（warn≥80% / bad≥95%，用既有语义色）+ 发票表(`Receipt` 行 + 下载) + 支付方式。金额必须带币种上下文，禁裸数字。升级按钮 `ArrowUpRight`。 |
| **角色权限 RBAC** | 租户管理员 | 角色列表（`ShieldCheck`）+ 权限矩阵（角色×能力，用 `Check`/`Minus` 而非纯色块，配文字 label）+ 用户-角色指派抽屉。权限感知空态：「你仅可查看成员，仅管理员可邀请」。敏感区（计费/审计/角色）显示当前租户名边界条。 |
| **配额可视化 Quota** | 全角色 | 复用 `.meter`/`.kpi .bar`：设备数 / 席位 / API 调用 / 存储，每项 用量% + 文本数值（禁仅色条，AA 需文字）。阈值：正常 `--brand-400`、warn `--warn`(≥80%)、bad `--bad`(≥95%)。卡片化分组，驾驶舱式高密度。 |

### 10.3 令牌扩展建议（追加到 `:root` / `.dark`）

```css
/* —— 租户上下文（MVP 单品牌：禁止第二主色，无租户自定义色相） —— */
/* 租户身份用「名称文字 + 中性 Building-2 标（品牌色）」表达，不引入每租户色相；边界用中性强边框 */
--tenant-dot: var(--brand-500);                            /* 当前租户指示点，统一用品牌色，非新色相 */
--border-strong: color-mix(in srgb, var(--text-2) 30%, var(--border)); /* 计费/审计/角色/租户敏感边界 */
--surface-sunken: color-mix(in srgb, var(--bg) 55%, #000 10%);        /* 表单内嵌/代码块/数据范围条 */

/* —— 套餐档徽章：仅用既有品牌色，禁紫/粉；档位以文字区分，不按档位引入额外色相 —— */
/* 统一复用 .badge.info（品牌蓝 #1d4ed8 文字 + 18% brand 淡底），仅改标签文字 免费/专业/商业/企业 */
--quota-track: color-mix(in srgb, var(--text-3) 22%, transparent);    /* 复用既有 meter 轨道 */

/* —— 图标尺寸令牌（全项目统一，Lucide 锁定） —— */
--icon-sm: 16px; --icon-md: 20px; --icon-lg: 24px; --icon-stroke: 2px;

/* —— 状态扩展（复用既有语义色，命名清晰化；MVP 单品牌不改色相） —— */
--state-active: var(--ok); --state-suspended: var(--text-3); --state-trial: var(--warn);
```

> **MVP 单品牌硬约束（来自 PM）**：SaaS 后台不引入任何第二主色，紫→粉渐变绝对禁止；租户/套餐/企业版均不产生独立色相。图表/进度/状态点一律用既有语义色（在线 `--ok` emerald / 预警 `--warn` amber / 告警 `--bad` rose / 信息 `--info` slate）。套餐档位用单一品牌蓝徽章 + 文字区分。企业版白标（自定义 Logo/配色）列入路线图，MVP 不做。

### 10.4 SaaS 层移动优先 & WCAG 2.2 AA 新增注意点

1. **租户切换器在 <1024px 必须可达**：置于侧栏抽屉内品牌下方，不可因顶栏空间紧张而隐藏。
2. **表格 → 卡片响应式**：租户表 / 发票表 / 权限矩阵在 <768px 转为卡片堆叠，配额 meter 纵向排布，禁横向滚动。
3. **配额 meter 必须附文本数值**（如「142 / 200 席位 · 71%」），仅色条不达标（AA：信息不止靠颜色）。
4. **开关/角色指派触控 ≥44×44px**；权限矩阵 `Check`/`Minus` 需 `aria-label`，配文字不纯图标。
5. **计费金额带币种与千分位**：`formatTenantCurrency(amount, tenant.locale)`，禁裸数字与 `1234567` 式占位。
6. **权限感知空态文案**：说明「为何无操作权」而非「No data」，降低挫败（参考 multi-tenant UX 最佳实践）。
7. **租户切换确认弹窗**：`role="alertdialog" aria-modal`，焦点陷入、Esc 关闭、切后回首页，避免数据混淆。
8. **敏感区边界条**：计费 / 审计 / 角色页顶部常驻「当前租户：<名>」(`--border-strong`)，防误操作错租户。
9. 既有 `--focus-visible` / `prefers-reduced-motion` / `:focus` 策略全继承，SaaS 新组件不得退化。

---

## 11. 信息架构与导航模型（见 UX-ARCHITECTURE.md）

视觉系统之上，导航按**用户任务心智**分三组（运营监控 / 分析洞察 / 系统治理），共 10 个目的地；
采用 `location.hash` 深链 + 跨页上下文 `ctx`（`deviceId` / `alarmFilter`），并设全局 `openDevice()` 作为唯一设备入口，
保证地图 / 驾驶舱 / 告警 / 通知 / 抽屉地图均可 ≤3 次操作钻取到同一设备。详细断点诊断、用户流与落地清单见同目录 `UX-ARCHITECTURE.md`。

---

## 9. Agent Prompt Guide（AI 代理提示指南）

**Quick Reference**
- 主色 `#3b82f6`（`--brand-500`），**主按钮底色用 `#2563eb`（`--brand-600`）** 以满足 AA；暗色背景 `#020617`，玻璃 `blur(14px) saturate(140%)`。
- 主题切换 = 根 `<html>` 加/去 `.dark` 类，组件样式零改动。
- 字体：Segoe UI / Microsoft YaHei + JetBrains Mono（≤ 两种）；8px 模块化刻度 12–48px。
- 圆角 16px（卡片）/ 11–12px（按钮、输入）/ 999px（胶囊）；间距基数 8px，区块 gap 18px。
- **移动优先**：基础 320px 单列，增强 `@media (min-width:576/768/992/1024)`；`<1024px` 侧栏为抽屉 + `#topNav` 横向导航。

**Component Prompts（可直接复制）**
1. `基于 DESIGN.md 生成一个 .card 玻璃态 KPI 卡片，含 label(用 --text-2) / 28px 数值 / 趋势箭头 / 进度条`
2. `生成一个 .nav 侧栏导航项，激活态为左侧 3px 天蓝条 + 左向渐变底 + aria-current="page"`
3. `生成一个 .search 顶栏搜索框，聚焦态 3px --ring 焦点环，带放大镜图标与 :focus-visible`
4. `生成一个右侧 .drawer 设备详情抽屉，宽度 min(440px,92vw)，role=dialog aria-modal，含 8 个 Tab`
5. `生成一个 .badge 语义徽章（ok/warn/bad/info），用 color-mix 18% 淡底 + AA 安全深色文字`
6. `生成一个响应式 kpi-grid：基准 1 列 / ≥576 两列 / ≥992 四列（移动优先 min-width）`
7. `生成一个 .seg 分段控件（7天/30天/90天），激活态 --brand-600 实底 + 白字`
8. `生成一个 .skip-link 跳到主内容，:focus 时 top 滑入，默认移出视口`

**Iteration Guide（迭代建议）**
1. 先锁 `--brand-500` 与暗色 `--bg`，再扩展派生色，避免色阶漂移。
2. 任何新组件先复用 `.glass`/`.card`，不要新建表面语言。
3. 改色先改 CSS 变量，不要在标记里硬编码十六进制。
4. **移动优先**：先写 320px 基准样式，再用 `min-width` 增强，勿用 `max-width` 桌面优先。
5. 每加一个页面，核对导航深度是否 ≤ 3 层、路径是否统一。
6. 按钮白字底色必须用 `--brand-600`（非 500），保证 AA 对比度。
7. 小文字（标签/副标题）一律用 `--text-2`，`--text-3` 仅作装饰/图表轴。
8. 语义状态务必「色 + 字」双信号，勿仅靠颜色区分。
9. 图表 SVG 沿用 `--brand-*` 与语义色，避免引入图表专属配色。
10. 交付前跑 `prefers-reduced-motion` 分支，并 Tab 走查焦点环与 Esc 关闭。
