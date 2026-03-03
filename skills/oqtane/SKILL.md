# Oqtane 技能

## 技能概述

Oqtane 技能是一个基于 .NET 10 的 AOT 编译技能，提供与 Oqtane 框架的无缝集成和扩展功能。Oqtane 是一个现代化的 .NET CMS（内容管理系统）和应用框架，专为构建企业级 Web 应用和网站而设计。

本技能旨在简化 Oqtane 应用的开发、部署和管理流程，提供一系列工具和功能，帮助开发者更高效地使用 Oqtane 框架。

## 主要功能

- **Oqtane 集成**：与 Oqtane 框架的无缝集成，支持最新版本的 Oqtane
- **模块管理**：管理 Oqtane 模块的安装、更新和卸载
- **主题管理**：管理 Oqtane 主题的安装和切换
- **用户管理**：管理 Oqtane 用户和角色
- **配置管理**：管理 Oqtane 应用配置
- **部署工具**：提供 Oqtane 应用的部署工具
- **API 文档**：生成 Oqtane API 文档
- **性能优化**：提供性能优化工具和建议
- **AOT 编译**：支持 .NET 10 AOT 编译，提升运行性能

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- Oqtane 4.0 或更高版本（可选）

### 安装方法

1. **克隆技能仓库**：
   ```bash
   git clone <repository-url>
   cd skills/oqtane
   ```

2. **安装依赖**：
   ```bash
   dotnet restore
   ```

3. **配置技能**：
   编辑 `scripts/oqtane_extensions.setting.json` 文件，根据需要修改配置。

4. **运行技能**：
   ```bash
   dotnet run --project scripts/oqtane_extensions.cs
   ```

## 使用指南

### 模块管理

Oqtane 技能提供了模块管理功能，可以帮助你安装、更新和卸载 Oqtane 模块。

```csharp
// 安装模块
await oqtaneService.InstallModuleAsync("ModuleName", "1.0.0");

// 更新模块
await oqtaneService.UpdateModuleAsync("ModuleName", "1.1.0");

// 卸载模块
await oqtaneService.UninstallModuleAsync("ModuleName");
```

### 主题管理

管理 Oqtane 主题，包括安装和切换主题。

```csharp
// 安装主题
await oqtaneService.InstallThemeAsync("ThemeName", "1.0.0");

// 切换主题
await oqtaneService.SetDefaultThemeAsync("ThemeName");
```

### 用户管理

管理 Oqtane 用户和角色。

```csharp
// 创建用户
await oqtaneService.CreateUserAsync("username", "email@example.com", "password");

// 添加角色
await oqtaneService.AddUserToRoleAsync("username", "Administrators");
```

### 配置管理

管理 Oqtane 应用配置。

```csharp
// 获取配置
var config = await oqtaneService.GetConfigurationAsync();

// 更新配置
await oqtaneService.UpdateConfigurationAsync(new OqtaneConfiguration
{
    SiteName = "My Site",
    DefaultLanguage = "zh-CN"
});
```

### 部署工具

提供 Oqtane 应用的部署工具。

```csharp
// 打包应用
await oqtaneService.PackageApplicationAsync("output/path");

// 部署到服务器
await oqtaneService.DeployApplicationAsync("server-url", "username", "password");
```

## AOT 编译支持

Oqtane 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

### 编译选项

- `PublishAot=true`：启用 AOT 编译
- `ReadyToRun=true`：启用 ReadyToRun 编译
- `TieredCompilation=true`：启用分层编译

### 支持的运行时

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

## 性能特性

- **快速启动**：AOT 编译减少了应用启动时间
- **内存占用低**：优化的内存使用
- **响应速度快**：提升了请求处理速度
- **部署简便**：单文件执行模式，部署更简单

## 参考文档

- **README.md**：详细参考文档，包含核心组件和配置说明
- **examples.md**：使用示例文档，包含各种使用场景和代码示例

## 许可证

本技能采用 MIT 许可证，详情请参阅 LICENSE 文件。

## 联系方式

- **官方网站**：https://oqtane.org
- **GitHub**：https://github.com/oqtane/oqtane.framework
- **文档**：https://docs.oqtane.org

---

**版本**：1.0.0
**发布日期**：2026-01-24
**作者**：Oqtane Community
