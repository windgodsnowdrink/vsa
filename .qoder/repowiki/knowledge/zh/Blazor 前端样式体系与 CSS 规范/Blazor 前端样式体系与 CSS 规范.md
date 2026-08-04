---
kind: frontend_style
name: Blazor 前端样式体系与 CSS 规范
category: frontend_style
scope:
    - '**'
source_files:
    - plc/App.razor
    - plc/pages/Index.razor
    - plc/wwwroot/app.css
    - code/blazor_crud.razor
    - code/Terminal.razor
---

该仓库的前端样式基于 Blazor WebAssembly 框架，采用 Bootstrap CSS 作为主要样式库，并结合自定义 CSS 文件进行主题定制。

**样式系统架构：**
- 使用 Blazor WebAssembly 框架（Microsoft.AspNetCore.Components.Web@8.0.0）
- 通过 `App.razor` 和 `Index.razor` 页面引入全局样式文件：`app.css` 和 `razorapp.styles.css`
- 样式文件位于 `plc/wwwroot/app.css`，包含基础样式、按钮样式、表单控件样式等
- 使用 `Assets["文件名"]` 模式引用静态资源

**CSS 方法论：**
- 采用 Bootstrap 类名命名约定（如 `btn-primary`、`form-control`、`table`、`btn-sm`、`btn-info`、`btn-danger`）
- 自定义样式遵循 BEM 风格，使用语义化类名（如 `terminal`、`output`、`form-group`）
- 响应式设计通过 viewport meta 标签配置：`width=device-width, initial-scale=1.0`

**设计令牌与主题：**
- 颜色变量：主色调 `#006bb7`（链接色）、`#1b6ec2`（按钮背景）、`#258cfb`（焦点边框）
- 字体栈：`'Helvetica Neue', Helvetica, Arial, sans-serif`
- 验证状态：绿色 `#26b050`（有效）、红色 `#e50000`（无效）
- 错误边界：使用内联 SVG 图标和 `#b32121` 背景色

**组件样式约定：**
- Blazor 组件使用 Razor 语法编写，样式通过 `class` 属性应用
- 表单组件统一使用 `InputText` 配合 `form-control` 类
- 按钮组件遵循 Bootstrap 按钮样式规范
- 表格使用 Bootstrap `table` 类实现响应式布局

**构建与打包：**
- 样式文件通过 Blazor 的静态资源管理机制自动打包
- 支持 CSS 优化和压缩（参考 tailwindcss skill 中的构建流程）
- 使用 `_framework/blazor.web.js` 加载运行时

**代码规范：**
- 使用 StyleCop 进行 C# 代码风格检查（`stylecop.json` 配置文件）
- Razor 组件遵循 Blazor 最佳实践，样式与逻辑分离
- 全局样式集中在 `app.css`，避免样式重复定义