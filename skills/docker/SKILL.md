# docker Agent Skill - Docker 技能

## 技能概述

基于 .NET 10 构建的高性能 Docker 技能，为 .NET 开发者提供强大的 Docker 功能支持。该技能采用 AOT（预编译）技术，提供极致的性能表现和启动速度，适用于各种 Docker 管理场景。

## 快速入门指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Docker.DotNet@3.125.15
```

### 注册服务

在主应用程序中注册 Docker 服务：

```csharp
// 配置 Docker 选项
builder.Configuration.AddJsonFile("docker_aot.setting.json", optional: true);
builder.Services.Configure<DockerOptions>(builder.Configuration.GetSection("Docker"));

// 注册 Docker 服务
builder.Services.AddDocker();
```

### 使用示例

```csharp
// 获取 Docker 引擎实例
var engine = serviceProvider.GetRequiredService<DockerAotEngine>();

// 查询容器列表
var containersResult = await engine.GetContainersAsync(true);
if (containersResult.Success)
{
    Console.WriteLine("容器列表：");
    foreach (var container in containersResult.Results)
    {
        Console.WriteLine($"  {container}");
    }
}

// 查询镜像列表
var imagesResult = await engine.GetImagesAsync();
if (imagesResult.Success)
{
    Console.WriteLine("\n镜像列表：");
    foreach (var image in imagesResult.Results)
    {
        Console.WriteLine($"  {image}");
    }
}
```

## 导航地图

```
docker/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── docker_aot.cs          # Docker 核心实现（AOT）
    ├── docker_aot.run.json    # 运行配置
    └── docker_aot.setting.json # 应用设置
```

## 主要功能

1. **高性能 AOT 编译**：基于 .NET 10 AOT 技术，提供极致性能和启动速度
2. **完整的容器生命周期管理**：支持容器的运行、停止、删除等操作
3. **镜像管理**：支持镜像的拉取、删除和列出
4. **Docker 系统信息**：查询 Docker 系统信息和版本
5. **命令行工具支持**：提供便捷的命令行管理工具
6. **异步编程模型**：提高系统响应性和并发能力
7. **灵活的配置选项**：支持通过配置文件自定义各种参数
8. **完善的错误处理**：详细的错误信息和日志记录

## 扩展说明

该技能提供了完整的 Docker 解决方案，您可以根据需要进行扩展：

1. **自定义 Docker 服务**：实现 IDockerService 接口，自定义 Docker 操作逻辑
2. **扩展命令类型**：添加新的 Docker 命令类型和处理逻辑
3. **集成其他系统**：与其他系统和框架集成，实现更复杂的 Docker 功能
4. **性能优化**：针对特定场景优化 Docker API 调用性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **合理配置**：根据实际需求调整 Docker 配置参数
4. **错误处理**：妥善处理 Docker 操作中的各种异常情况
5. **日志记录**：适当添加日志记录，便于调试和监控
6. **性能监控**：启用性能监控，实时了解 Docker 操作性能
7. **资源管理**：及时清理不再使用的容器和镜像，释放资源

## AOT 编译说明

该技能支持 .NET 10 AOT 编译，通过预编译将应用程序编译为本地机器代码，提供以下优势：

- **极致的启动速度**：无需 JIT 编译，直接运行本地代码
- **减少内存占用**：更小的运行时占用
- **提高安全性**：减少可攻击面，提高应用程序安全性
- **跨平台支持**：支持多种操作系统和架构

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
```
docker_aot.exe ps -a           # 列出所有容器
docker_aot.exe run nginx       # 运行一个新的 nginx 容器
docker_aot.exe run -p 8080:80 --name mynginx nginx
docker_aot.exe stop mynginx    # 停止名为 mynginx 的容器
docker_aot.exe rm mynginx      # 删除名为 mynginx 的容器
docker_aot.exe pull ubuntu     # 拉取最新的 ubuntu 镜像
docker_aot.exe images          # 列出所有镜像
docker_aot.exe info            # 显示 Docker 系统信息
docker_aot.exe version         # 显示 Docker 版本信息
```
