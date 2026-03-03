# Silky 技能参考文档

## 架构概述

Silky 技能基于 .NET 10 构建，采用 AOT 编译技术，提供高性能的微服务开发框架。本技能包含以下核心组件：

1. **核心功能**：微服务框架、RPC 调用、API 网关、分布式配置、服务治理
2. **工具链**：项目创建、代码生成、服务注册管理
3. **配置文件**：编译配置、运行配置
4. **文档**：使用指南、API 参考、示例代码

## 核心 API

### 服务注册与发现

#### `AddRegistryCenter()`
- **功能**：添加服务注册中心
- **参数**：无
- **返回值**：配置选项对象
- **示例**：
  ```csharp
  builder.Services.AddSilkyServices(builder.Configuration, options =>
  {
      options.AddRegistryCenter();
  });
  ```

#### `Configure<RegistryOptions>()`
- **功能**：配置服务注册选项
- **参数**：配置委托
- **返回值**：无
- **示例**：
  ```csharp
  builder.Services.Configure<RegistryOptions>(options =>
  {
      options.RegistryCenterType = RegistryCenterType.Zookeeper;
      options.ConnectionString = "localhost:2181";
  });
  ```

### RPC 调用

#### `Configure<RpcOptions>()`
- **功能**：配置 RPC 选项
- **参数**：配置委托
- **返回值**：无
- **示例**：
  ```csharp
  builder.Services.Configure<RpcOptions>(options =>
  {
      options.TimeoutMilliseconds = 3000;
      options.RetryCount = 3;
  });
  ```

### API 网关

#### `AddSilkyGateway()`
- **功能**：添加 Silky 网关服务
- **参数**：配置对象、配置选项委托
- **返回值**：服务集合
- **示例**：
  ```csharp
  builder.Services.AddSilkyGateway(builder.Configuration, options =>
  {
      options.AddRegistryCenter();
      options.AddSwaggerDocument();
  });
  ```

#### `Configure<GatewayOptions>()`
- **功能**：配置网关选项
- **参数**：配置委托
- **返回值**：无
- **示例**：
  ```csharp
  builder.Services.Configure<GatewayOptions>(options =>
  {
      options.RouteCacheExpireTime = 30;
  });
  ```

### 分布式配置

#### `AddDistributedConfiguration()`
- **功能**：添加分布式配置
- **参数**：无
- **返回值**：服务集合
- **示例**：
  ```csharp
  builder.Services.AddDistributedConfiguration();
  ```

#### `Configure<ConfigurationOptions>()`
- **功能**：配置配置中心选项
- **参数**：配置委托
- **返回值**：无
- **示例**：
  ```csharp
  builder.Services.Configure<ConfigurationOptions>(options =>
  {
      options.ConfigCenterType = ConfigCenterType.Consul;
      options.ConnectionString = "localhost:8500";
  });
  ```

### 服务治理

#### `Configure<GovernanceOptions>()`
- **功能**：配置服务治理选项
- **参数**：配置委托
- **返回值**：无
- **示例**：
  ```csharp
  builder.Services.Configure<GovernanceOptions>(options =>
  {
      options.TimeoutMilliseconds = 3000;
      options.RetryCount = 3;
      options.CircuitBreakerFallbackPolicy = CircuitBreakerFallbackPolicy.FailFast;
  });
  ```

## 配置选项

### 编译配置

在 `*.setting.json` 文件中配置编译选项：

```json
{
  "compilationOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": true,
    "implicitUsings": true
  },
  "publishOptions": {
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  }
}
```

### 运行配置

在 `*.run.json` 文件中配置运行选项：

```json
{
  "profiles": {
    "ProfileName": {
      "commandName": "Project",
      "commandLineArgs": "command arguments",
      "workingDirectory": "working directory"
    }
  }
}
```

## CLI 命令

### silky_core

#### `create`
- **功能**：创建项目
- **参数**：
  - `--type`：项目类型（service/gateway/module）
  - `--name`：项目名称
  - `--output`：输出目录
- **示例**：
  ```bash
  silky_core create --type service --name UserService --output ./services
  ```

#### `start`
- **功能**：启动项目
- **参数**：
  - `--project`：项目路径
  - `--environment`：环境变量
- **示例**：
  ```bash
  silky_core start --project ./services/UserService --environment Development
  ```

#### `build`
- **功能**：构建项目
- **参数**：
  - `--project`：项目路径
  - `--configuration`：构建配置
- **示例**：
  ```bash
  silky_core build --project ./services/UserService --configuration Release
  ```

#### `registry`
- **功能**：服务注册管理
- **参数**：
  - `--list`：列出所有注册的服务
  - `--health`：检查服务健康状态
- **示例**：
  ```bash
  silky_core registry --list
  silky_core registry --health UserService
  ```

### silky_generator

#### `generate`
- **功能**：生成代码
- **参数**：
  - `--type`：生成类型（api/impl/dto）
  - `--service`：服务名称
  - `--output`：输出目录
- **示例**：
  ```bash
  silky_generator generate --type api --service UserService --output ./services/UserService/Services/Interfaces
  ```

## 扩展指南

### 自定义服务注册中心

1. **实现接口**：
  ```csharp
  public class CustomRegistryCenter : IRegistryCenter
  {
      // 实现方法
  }
  ```

2. **注册服务**：
  ```csharp
  builder.Services.AddSingleton<IRegistryCenter, CustomRegistryCenter>();
  ```

### 自定义负载均衡策略

1. **实现接口**：
  ```csharp
  public class CustomLoadBalancer : ILoadBalancer
  {
      // 实现方法
  }
  ```

2. **注册服务**：
  ```csharp
  builder.Services.AddSingleton<ILoadBalancer, CustomLoadBalancer>();
  ```

### 自定义序列化方式

1. **实现接口**：
  ```csharp
  public class CustomSerializer : ISerializer
  {
      // 实现方法
  }
  ```

2. **注册服务**：
  ```csharp
  builder.Services.AddSingleton<ISerializer, CustomSerializer>();
  ```

### 自定义 API 网关过滤器

1. **实现接口**：
  ```csharp
  public class CustomGatewayFilter : IGatewayFilter
  {
      // 实现方法
  }
  ```

2. **注册服务**：
  ```csharp
  builder.Services.AddSingleton<IGatewayFilter, CustomGatewayFilter>();
  ```

## 性能优化

1. **AOT 编译**：使用 `publishAot: true` 启用 AOT 编译，提高启动速度和运行性能
2. **序列化优化**：使用 MessagePack 序列化，减少网络传输开销
3. **连接池**：使用连接池管理网络连接，减少连接建立开销
4. **缓存**：合理使用缓存，减少重复计算和数据库查询
5. **负载均衡**：选择合适的负载均衡策略，提高系统吞吐量
6. **服务治理**：合理配置服务治理策略，提高系统可靠性

## 部署指南

### 本地开发

1. **安装依赖**：
  ```bash
  dotnet restore
  ```

2. **构建项目**：
  ```bash
  dotnet build -c Release
  ```

3. **运行服务**：
  ```bash
  dotnet run --project ./services/UserService
  ```

### 生产部署

1. **AOT 编译**：
  ```bash
  dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:PublishSingleFile=true
  ```

2. **部署到服务器**：
  - 将生成的可执行文件复制到服务器
  - 配置环境变量和配置文件
  - 启动服务

3. **容器化部署**：
  ```dockerfile
  FROM mcr.microsoft.com/dotnet/runtime:8.0-windowsservercore-ltsc2022
  WORKDIR /app
  COPY ./publish .
  ENTRYPOINT ["./UserService.exe"]
  ```

## 监控与告警

### 集成 Prometheus

1. **添加依赖**：
  ```csharp
  #add_package Prometheus.AspNetCore
  ```

2. **配置监控**：
  ```csharp
  var app = builder.Build();
  app.UseMetricServer();
  app.UseHttpMetrics();
  ```

### 集成 Grafana

1. **创建仪表盘**：导入 Prometheus 数据源
2. **配置告警**：设置服务健康状态告警

## 安全最佳实践

1. **认证授权**：使用 JWT 或 OAuth2 进行身份验证和授权
2. **加密传输**：使用 HTTPS 加密网络传输
3. **输入验证**：对所有输入进行验证，防止注入攻击
4. **日志脱敏**：对敏感信息进行脱敏处理
5. **权限控制**：实现细粒度的权限控制
6. **安全审计**：记录关键操作的审计日志

## 故障排查

### 常见问题

1. **服务注册失败**
   - 检查服务注册中心是否正常运行
   - 检查网络连接是否正常
   - 检查服务名称是否正确

2. **RPC 调用失败**
   - 检查目标服务是否正常运行
   - 检查网络连接是否正常
   - 检查服务接口定义是否一致
   - 检查超时设置是否合理

3. **API 网关路由失败**
   - 检查路由配置是否正确
   - 检查目标服务是否正常运行
   - 检查网关配置是否正确

4. **分布式配置更新失败**
   - 检查配置中心是否正常运行
   - 检查配置权限是否正确
   - 检查配置格式是否正确

5. **服务健康检查失败**
   - 检查服务依赖是否正常
   - 检查服务自身是否正常运行
   - 检查健康检查配置是否正确

6. **AOT 编译错误**
   - 检查代码是否符合 AOT 编译要求
   - 检查依赖项是否支持 AOT 编译
   - 检查编译配置是否正确

### 日志排查

1. **配置日志级别**：
  ```csharp
  builder.Logging.SetMinimumLevel(LogLevel.Debug);
  ```

2. **查看日志**：
  - 控制台日志
  - 文件日志
  - 分布式日志系统

3. **日志分析**：
  - 查找错误信息
  - 分析调用链路
  - 定位性能瓶颈

## 版本兼容性

### .NET 版本

- **支持的版本**：.NET 10.0
- **推荐版本**：.NET 10.0.100 或更高版本

### 依赖项版本

| 依赖项 | 版本 |
|--------|------|
| Silky.Core | 3.3.0 |
| Silky.Http.Core | 3.3.0 |
| Silky.Rpc | 3.3.0 |
| Silky.Registry | 3.3.0 |
| Silky.Swagger | 3.3.0 |
| Microsoft.Extensions.DependencyInjection | 10.0.0 |
| Microsoft.Extensions.Logging | 10.0.0 |
| System.CommandLine | 2.0.0 |
| System.Text.Json | 10.0.0 |

### 操作系统

- **Windows**：Windows 10/11、Windows Server 2019/2022
- **Linux**：Ubuntu 20.04+、CentOS 7+、Debian 10+
- **macOS**：macOS 12+（仅开发环境）

## 总结

Silky 技能提供了完整的微服务开发框架，支持服务注册与发现、RPC 调用、API 网关、分布式配置、服务治理等核心功能。通过 AOT 编译技术，提供了更高的性能和更小的部署包。本参考文档涵盖了 Silky 技能的核心 API、配置选项、CLI 命令、扩展指南、性能优化、部署指南、监控与告警、安全最佳实践、故障排查和版本兼容性等内容，希望能帮助您更好地使用 Silky 技能开发微服务应用。