# Orleans 技能

## 技能概述

Orleans 技能是一个基于 .NET 10 的高性能技能，提供与 Orleans 框架的无缝集成和扩展功能。Orleans 是一个现代化的 .NET 分布式计算框架，基于 Actor 模型，专为构建可扩展、高可用的分布式系统而设计。

本技能旨在简化 Orleans 应用的开发、部署和管理流程，提供一系列工具和功能，帮助开发者更高效地使用 Orleans 框架。

## 主要功能

- **Orleans 集成**：与 Orleans 框架的无缝集成，支持最新版本的 Orleans
- **事件溯源**：支持 Orleans 事件溯源模式，实现状态的持久化和回放
- **状态机**：支持 Orleans 状态机模式，实现复杂业务流程的状态管理
- **集群管理**：管理 Orleans 集群节点，包括节点的添加、移除和监控
- **部署工具**：提供 Orleans 应用的部署工具，简化部署流程
- **API 文档**：生成 Orleans API 文档
- **性能优化**：提供性能优化工具和建议
- **AOT 编译**：支持 .NET 10 AOT 编译，提升运行性能

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- Orleans 8.0 或更高版本

### 安装方法

1. **克隆技能仓库**：
   ```bash
   git clone <repository-url>
   cd skills/orleans
   ```

2. **安装依赖**：
   ```bash
   dotnet restore
   ```

3. **配置技能**：
   编辑 `scripts/orleans_eventsourcing_integration.setting.json` 或 `scripts/orleans_statemachine_integration.setting.json` 文件，根据需要修改配置。

4. **运行技能**：
   ```bash
   dotnet run --project scripts/orleans_eventsourcing_integration.cs
   ```

## 使用指南

### 事件溯源

Orleans 技能提供了事件溯源功能，可以帮助你实现状态的持久化和回放。

```csharp
// 获取事件溯源服务
var eventSourcingService = serviceProvider.GetRequiredService<IOrleansEventSourcingService>();

// 保存事件
await eventSourcingService.SaveEventAsync("GrainId", new UserCreatedEvent { UserId = 1, Username = "user1" });

// 获取事件历史
var events = await eventSourcingService.GetEventsAsync("GrainId");
```

### 状态机

管理 Orleans 状态机，实现复杂业务流程的状态管理。

```csharp
// 获取状态机服务
var stateMachineService = serviceProvider.GetRequiredService<IOrleansStateMachineService>();

// 创建状态机
await stateMachineService.CreateStateMachineAsync("Order123", "Pending");

// 触发状态转换
await stateMachineService.TriggerEventAsync("Order123", "PaymentReceived");

// 获取当前状态
var currentState = await stateMachineService.GetCurrentStateAsync("Order123");
```

### 集群管理

管理 Orleans 集群节点，包括节点的添加、移除和监控。

```csharp
// 获取集群管理服务
var clusterService = serviceProvider.GetRequiredService<IOrleansClusterService>();

// 获取集群状态
var clusterStatus = await clusterService.GetClusterStatusAsync();

// 添加节点
await clusterService.AddNodeAsync("new-node-1", "192.168.1.100:30000");
```

### 配置管理

管理 Orleans 应用配置。

```csharp
// 获取配置服务
var configService = serviceProvider.GetRequiredService<IOrleansConfigurationService>();

// 获取当前配置
var config = await configService.GetConfigurationAsync();

// 更新配置
await configService.UpdateConfigurationAsync(new OrleansConfiguration
{
    ClusterId = "my-cluster",
    ServiceId = "my-service",
    SiloPort = 11111,
    GatewayPort = 30000
});
```

### 部署工具

提供 Orleans 应用的部署工具。

```csharp
// 获取部署服务
var deploymentService = serviceProvider.GetRequiredService<IOrleansDeploymentService>();

// 打包应用
await deploymentService.PackageApplicationAsync("output/path");

// 部署到服务器
await deploymentService.DeployApplicationAsync("server-url", "username", "password");
```

## AOT 编译支持

Orleans 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

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

- **官方网站**：https://vsa-arch.com
- **GitHub**：https://github.com/vsa-arch/orleans-skill
- **文档**：https://docs.vsa-arch.com/orleans-skill

---

**版本**：1.0.0
**发布日期**：2026-01-24
**作者**：VSA Architecture Team
