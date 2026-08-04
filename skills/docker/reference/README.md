# docker - 参考文档

## 概述

docker 是基于 .NET 10 AOT 架构的高性能 Docker 技能，专为 .NET 开发者设计，提供强大的 Docker 容器和镜像管理功能。

## 核心组件

### 1. DockerService（Docker 服务）
- **位置**: scripts/docker_aot.cs
- **功能**: Docker 查询核心服务，负责容器管理、镜像管理和系统信息查询
- **特性**: 
  - 基于 .NET 10 AOT 编译，高性能
  - 支持完整的容器生命周期管理
  - 支持镜像管理操作
  - 异步编程模型
  - 多 Docker 服务器支持
  - 详细的错误处理和日志记录

### 2. DockerAotEngine（Docker AOT 引擎）
- **位置**: scripts/docker_aot.cs
- **功能**: 管理 Docker 功能调用的引擎，提供简洁的 API 接口
- **特性**: 
  - 简化的 API 调用
  - 统一的错误处理
  - 支持命令行操作
  - 完整的 Docker 功能支持

## 核心接口

### IDockerService
Docker 服务的核心接口，定义了所有 Docker 操作方法：

| 方法名 | 描述 | 参数 | 返回值 |
|--------|------|------|--------|
| ExecuteCommandAsync | 执行 Docker 命令 | commandType: DockerCommandType, parameters: Dictionary<string, string>? | Task<DockerCommandResult> |
| GetContainersAsync | 获取容器列表 | showAll: bool | Task<DockerCommandResult> |
| GetImagesAsync | 获取镜像列表 | showAll: bool | Task<DockerCommandResult> |
| RunContainerAsync | 运行容器 | image: string, containerName: string?, ports: Dictionary<string, string>? | Task<DockerCommandResult> |
| StopContainerAsync | 停止容器 | containerIdOrName: string | Task<DockerCommandResult> |
| RemoveContainerAsync | 删除容器 | containerIdOrName: string, force: bool | Task<DockerCommandResult> |
| PullImageAsync | 拉取镜像 | image: string | Task<DockerCommandResult> |
| RemoveImageAsync | 删除镜像 | imageIdOrName: string, force: bool | Task<DockerCommandResult> |
| GetDockerInfoAsync | 获取 Docker 信息 | 无 | Task<DockerCommandResult> |
| GetDockerVersionAsync | 获取 Docker 版本 | 无 | Task<DockerCommandResult> |

## 数据结构

### DockerCommandType（Docker 命令类型）
```csharp
public enum DockerCommandType
{
    ContainerList,  // 容器列表
    ImageList,      // 镜像列表
    ContainerRun,   // 运行容器
    ContainerStop,  // 停止容器
    ContainerRemove,// 删除容器
    ImagePull,      // 拉取镜像
    ImageRemove,    // 删除镜像
    DockerInfo,     // Docker 信息
    DockerVersion   // Docker 版本
}
```

### DockerCommandResult（Docker 命令结果）
```csharp
public class DockerCommandResult
{
    public bool Success { get; set; }              // 命令是否成功
    public DockerCommandType CommandType { get; set; } // 命令类型
    public List<string> Results { get; set; }       // 结果数据
    public long ExecutionTimeMs { get; set; }       // 执行时间（毫秒）
    public string? ErrorMessage { get; set; }       // 错误信息
    public string? DockerEndpoint { get; set; }    // Docker API 端点
}
```

### DockerOptions（Docker 配置选项）
```csharp
public class DockerOptions
{
    public string DockerEndpoint { get; set; } = "npipe://./pipe/docker_engine"; // 默认 Docker API 端点
    public bool UseTls { get; set; } = false;      // 是否使用 TLS
    public string? TlsCertPath { get; set; }       // TLS 证书路径
    public int TimeoutMs { get; set; } = 30000;    // 查询超时时间（毫秒）
    public bool EnableDetailedLogging { get; set; } = false; // 是否启用详细日志
    public bool EnablePerformanceMonitoring { get; set; } = true; // 是否启用性能监控
    public string DefaultImage { get; set; } = "nginx:latest"; // 默认镜像
    public string ContainerNamePrefix { get; set; } = "vsa-"; // 默认容器名称前缀
}
```

## 配置选项

### Docker 配置（docker_aot.setting.json）

```json
{
  "Docker": {
    "DockerEndpoint": "npipe://./pipe/docker_engine", // Docker API 端点
    "UseTls": false,                               // 是否使用 TLS
    "TlsCertPath": null,                           // TLS 证书路径
    "TimeoutMs": 30000,                           // 查询超时时间（毫秒）
    "EnableDetailedLogging": false,                // 是否启用详细日志
    "EnablePerformanceMonitoring": true,           // 是否启用性能监控
    "DefaultImage": "nginx:latest",               // 默认镜像
    "ContainerNamePrefix": "vsa-"                 // 默认容器名称前缀
  }
}
```

## 运行配置（docker_aot.run.json）

```json
{
  "$schema": "https://dot.net/v1/dotnet.run.schema.json",
  "framework": "net10.0",
  "options": {
    "PublishAot": true,                   // 启用 AOT 编译
    "InvariantGlobalization": true,       // 启用不变全球化
    "EnableCompilationRelaxations": true, // 启用编译松弛
    "PublishReadyToRun": true,            // 启用 ReadyToRun
    "LangVersion": "preview",            // 语言版本
    "Nullable": true,                     // 启用可空引用类型
    "ImplicitUsings": true                // 启用隐式 using
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Hosting": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "Microsoft.Extensions.Options": "10.0.0",
    "Docker.DotNet": "3.125.15",
    "Docker.DotNet.BasicAuth": "3.125.15",
    "Docker.DotNet.X509": "3.125.15"
  }
}
```

## 性能优化

1. **启用 AOT 编译**：AOT 编译可以提供极致的启动速度和运行性能
2. **异步编程**：使用异步 API 可以提高系统的并发处理能力
3. **合理配置超时时间**：根据网络状况调整 Docker API 调用超时时间
4. **资源管理**：及时清理不再使用的容器和镜像，释放系统资源
5. **日志级别控制**：在生产环境中，将日志级别设置为 Information 或更高，减少日志开销
6. **使用连接池**：Docker.DotNet 内部已实现连接池，无需额外配置

## 故障排除

### 常见问题

1. **Docker 连接失败**
   - 检查 Docker 服务是否正在运行
   - 验证 Docker API 端点配置是否正确
   - 检查网络连接是否正常
   - 查看日志获取详细错误信息

2. **容器运行失败**
   - 检查镜像是否存在
   - 验证端口映射是否正确
   - 检查容器配置是否有误
   - 查看 Docker 日志获取详细错误信息

3. **镜像拉取失败**
   - 检查网络连接是否正常
   - 验证镜像名称是否正确
   - 检查 Docker Hub 访问权限
   - 查看日志获取详细错误信息

4. **命令执行超时**
   - 调整超时时间配置
   - 检查网络连接质量
   - 检查 Docker 服务负载情况
   - 考虑使用异步操作避免阻塞

5. **服务异常**
   - 查看日志获取详细错误信息
   - 检查配置文件是否正确
   - 重启 Docker 服务
   - 重启应用程序

## 扩展开发

### 自定义 Docker 服务

1. 实现 IDockerService 接口
2. 重写需要自定义的方法
3. 注册自定义服务

```csharp
builder.Services.AddSingleton<IDockerService, CustomDockerService>();
```

### 扩展命令类型

1. 在 DockerCommandType 枚举中添加新的命令类型
2. 在 ExecuteCommandAsync 方法中添加相应的处理逻辑
3. 添加对应的便捷方法

## AOT 编译注意事项

1. **依赖项**：确保所有依赖项都支持 AOT 编译
2. **反射**：避免在运行时使用反射，或使用 AOT 友好的反射方式
3. **动态类型**：谨慎使用 dynamic 类型，可能会影响 AOT 编译效果
4. **配置文件**：AOT 编译后，配置文件路径可能需要调整
5. **测试**：在 AOT 模式下进行充分测试，确保所有功能正常工作
6. **Docker.DotNet 兼容性**：Docker.DotNet 3.125.15 版本已验证支持 AOT 编译

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `ps, container, containers`: 列出容器
- `images, image`: 列出镜像
- `run <image> [options]`: 运行容器
- `stop <container>`: 停止容器
- `rm, remove <container>`: 删除容器
- `pull <image>`: 拉取镜像
- `rmi <image>`: 删除镜像
- `info`: 显示 Docker 系统信息
- `version`: 显示 Docker 版本信息
- `help, --help, -h`: 显示帮助信息

使用示例：
```bash
docker_aot.exe ps -a           # 列出所有容器
docker_aot.exe run nginx       # 运行一个新的 nginx 容器
docker_aot.exe stop mynginx    # 停止名为 mynginx 的容器
docker_aot.exe pull ubuntu     # 拉取最新的 ubuntu 镜像
docker_aot.exe images          # 列出所有镜像
docker_aot.exe info            # 显示 Docker 系统信息
```

## 版本历史

| 版本 | 日期 | 描述 |
|------|------|------|
| 1.0.0 | 2026-01-03 | 初始版本，基于 .NET 10 AOT 架构 |

## 许可证

MIT License

