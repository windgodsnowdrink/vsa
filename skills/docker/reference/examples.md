# docker - 使用示例

## 快速开始

### 1. 基本 Docker 操作示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DockerAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建主机和服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("docker_aot.setting.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // 配置 Docker 选项
                    services.Configure<Docker.AOT.DockerOptions>(context.Configuration.GetSection("Docker"));
                    
                    // 注册 Docker 服务
                    services.AddDocker();
                })
                .Build();

            Console.WriteLine("Docker 基本操作示例");
            Console.WriteLine("=" * 50);
            
            // 获取 Docker 服务实例
            var dockerService = host.Services.GetRequiredService<Docker.AOT.IDockerService>();
            
            // 1. 获取 Docker 版本信息
            Console.WriteLine("\n1. 获取 Docker 版本信息");
            var versionResult = await dockerService.GetDockerVersionAsync();
            if (versionResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {versionResult.ExecutionTimeMs}ms)");
                foreach (var versionInfo in versionResult.Results)
                {
                    Console.WriteLine($"  - {versionInfo}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {versionResult.ErrorMessage}");
            }
            
            // 2. 获取 Docker 系统信息
            Console.WriteLine("\n2. 获取 Docker 系统信息");
            var infoResult = await dockerService.GetDockerInfoAsync();
            if (infoResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {infoResult.ExecutionTimeMs}ms)");
                foreach (var info in infoResult.Results)
                {
                    Console.WriteLine($"  - {info}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {infoResult.ErrorMessage}");
            }
            
            // 3. 获取镜像列表
            Console.WriteLine("\n3. 获取镜像列表");
            var imagesResult = await dockerService.GetImagesAsync();
            if (imagesResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {imagesResult.ExecutionTimeMs}ms)");
                foreach (var image in imagesResult.Results)
                {
                    Console.WriteLine($"  - {image}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {imagesResult.ErrorMessage}");
            }
            
            // 4. 获取容器列表
            Console.WriteLine("\n4. 获取容器列表");
            var containersResult = await dockerService.GetContainersAsync(true);
            if (containersResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {containersResult.ExecutionTimeMs}ms)");
                foreach (var container in containersResult.Results)
                {
                    Console.WriteLine($"  - {container}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {containersResult.ErrorMessage}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 2. 容器生命周期管理示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DockerAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Docker 容器生命周期管理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 手动配置 Docker 选项
                    services.Configure<Docker.AOT.DockerOptions>(options => {
                        options.DockerEndpoint = "npipe://./pipe/docker_engine";
                        options.TimeoutMs = 30000;
                        options.DefaultImage = "nginx:latest";
                        options.ContainerNamePrefix = "demo-";
                    });
                    
                    // 注册 Docker 服务
                    services.AddDocker();
                })
                .Build();
            
            // 获取 Docker 服务实例
            var dockerService = host.Services.GetRequiredService<Docker.AOT.IDockerService>();
            
            string containerName = "demo-nginx";
            
            try
            {
                // 1. 运行一个新的 Nginx 容器
                Console.WriteLine($"\n1. 运行 Nginx 容器: {containerName}");
                var runResult = await dockerService.RunContainerAsync(
                    "nginx:latest",
                    containerName,
                    new System.Collections.Generic.Dictionary<string, string> { { "80", "8080" } } // 内部 80 端口映射到外部 8080 端口
                );
                
                if (runResult.Success)
                {
                    Console.WriteLine($"成功 (耗时: {runResult.ExecutionTimeMs}ms)");
                    foreach (var result in runResult.Results)
                    {
                        Console.WriteLine($"  {result}");
                    }
                    
                    // 等待 5 秒，让容器完全启动
                    Console.WriteLine("\n等待 5 秒，让容器完全启动...");
                    await Task.Delay(5000);
                    
                    // 2. 再次获取容器列表，确认容器已启动
                    Console.WriteLine("\n2. 检查容器运行状态");
                    var containersResult = await dockerService.GetContainersAsync(true);
                    if (containersResult.Success)
                    {
                        Console.WriteLine($"成功 (耗时: {containersResult.ExecutionTimeMs}ms)");
                        foreach (var container in containersResult.Results)
                        {
                            if (container.Contains(containerName))
                            {
                                Console.WriteLine($"  - {container}");
                            }
                        }
                    }
                    
                    // 3. 停止容器
                    Console.WriteLine($"\n3. 停止容器: {containerName}");
                    var stopResult = await dockerService.StopContainerAsync(containerName);
                    if (stopResult.Success)
                    {
                        Console.WriteLine($"成功 (耗时: {stopResult.ExecutionTimeMs}ms)");
                        foreach (var result in stopResult.Results)
                        {
                            Console.WriteLine($"  {result}");
                        }
                    }
                    
                    // 4. 删除容器
                    Console.WriteLine($"\n4. 删除容器: {containerName}");
                    var removeResult = await dockerService.RemoveContainerAsync(containerName, true);
                    if (removeResult.Success)
                    {
                        Console.WriteLine($"成功 (耗时: {removeResult.ExecutionTimeMs}ms)");
                        foreach (var result in removeResult.Results)
                        {
                            Console.WriteLine($"  {result}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"失败: {runResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 3. 镜像管理示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DockerAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Docker 镜像管理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Docker.AOT.DockerOptions>(options => {
                        options.DockerEndpoint = "npipe://./pipe/docker_engine";
                        options.TimeoutMs = 60000; // 延长超时时间，以便拉取镜像
                    });
                    services.AddDocker();
                })
                .Build();
            
            var dockerService = host.Services.GetRequiredService<Docker.AOT.IDockerService>();
            string testImage = "hello-world:latest";
            
            try
            {
                // 1. 先检查镜像是否已存在
                Console.WriteLine($"\n1. 检查镜像: {testImage}");
                var imagesResult = await dockerService.GetImagesAsync();
                bool imageExists = false;
                if (imagesResult.Success)
                {
                    foreach (var image in imagesResult.Results)
                    {
                        if (image.Contains(testImage))
                        {
                            imageExists = true;
                            Console.WriteLine($"  镜像已存在: {image}");
                            break;
                        }
                    }
                }
                
                if (!imageExists)
                {
                    // 2. 拉取镜像
                    Console.WriteLine($"\n2. 拉取镜像: {testImage}");
                    var pullResult = await dockerService.PullImageAsync(testImage);
                    if (pullResult.Success)
                    {
                        Console.WriteLine($"成功 (耗时: {pullResult.ExecutionTimeMs}ms)");
                        foreach (var result in pullResult.Results)
                        {
                            Console.WriteLine($"  {result}");
                        }
                        
                        // 3. 再次检查镜像是否已存在
                        Console.WriteLine($"\n3. 确认镜像已拉取");
                        imagesResult = await dockerService.GetImagesAsync();
                        if (imagesResult.Success)
                        {
                            foreach (var image in imagesResult.Results)
                            {
                                if (image.Contains(testImage))
                                {
                                    Console.WriteLine($"  镜像已拉取: {image}");
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"拉取失败: {pullResult.ErrorMessage}");
                        return;
                    }
                }
                
                // 4. 运行 hello-world 容器
                Console.WriteLine($"\n4. 运行 {testImage} 容器");
                var runResult = await dockerService.RunContainerAsync(testImage);
                if (runResult.Success)
                {
                    Console.WriteLine($"成功 (耗时: {runResult.ExecutionTimeMs}ms)");
                    foreach (var result in runResult.Results)
                    {
                        Console.WriteLine($"  {result}");
                    }
                }
                
                // 5. 查看所有容器（包括已停止的）
                Console.WriteLine("\n5. 查看所有容器");
                var containersResult = await dockerService.GetContainersAsync(true);
                if (containersResult.Success)
                {
                    Console.WriteLine($"成功 (耗时: {containersResult.ExecutionTimeMs}ms)");
                    foreach (var container in containersResult.Results)
                    {
                        if (container.Contains("hello-world"))
                        {
                            Console.WriteLine($"  - {container}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常: {ex.Message}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 4. 命令行工具示例

```bash
# 查看帮助信息
docker_aot.exe help
docker_aot.exe --help
docker_aot.exe -h

# 查看 Docker 版本信息
docker_aot.exe version

# 查看 Docker 系统信息
docker_aot.exe info

# 列出所有容器
docker_aot.exe ps -a
docker_aot.exe containers -a
docker_aot.exe container -a

# 列出所有镜像
docker_aot.exe images
docker_aot.exe images -a
docker_aot.exe image

# 拉取镜像
docker_aot.exe pull nginx:latest
docker_aot.exe pull ubuntu:22.04

# 运行容器
docker_aot.exe run nginx:latest
docker_aot.exe run --name mynginx -p 8080:80 nginx:latest
docker_aot.exe run --name myubuntu -it ubuntu:22.04 bash

# 停止容器
docker_aot.exe stop mynginx
docker_aot.exe stop <container-id>

# 删除容器
docker_aot.exe rm mynginx
docker_aot.exe remove <container-id>
docker_aot.exe rm -f <container-id>  # 强制删除

# 删除镜像
docker_aot.exe rmi nginx:latest
docker_aot.exe rmi -f <image-id>  # 强制删除
```

### 5. 错误处理示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DockerAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Docker 错误处理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器，使用无效的 Docker 端点
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Docker.AOT.DockerOptions>(options => {
                        options.DockerEndpoint = "npipe://./pipe/invalid-docker-engine"; // 无效的 Docker 端点
                        options.TimeoutMs = 5000; // 短超时，便于测试
                    });
                    services.AddDocker();
                })
                .Build();
            
            var dockerService = host.Services.GetRequiredService<Docker.AOT.IDockerService>();
            
            Console.WriteLine("\n1. 尝试连接无效的 Docker 端点");
            try
            {
                var result = await dockerService.GetDockerVersionAsync();
                if (result.Success)
                {
                    Console.WriteLine("成功: 这是预期之外的结果");
                }
                else
                {
                    Console.WriteLine($"预期失败: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获异常: {ex.GetType().Name}: {ex.Message}");
            }
            
            // 构建使用有效 Docker 端点的服务容器
            var validHost = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Docker.AOT.DockerOptions>(options => {
                        options.DockerEndpoint = "npipe://./pipe/docker_engine";
                        options.TimeoutMs = 5000;
                    });
                    services.AddDocker();
                })
                .Build();
            
            var validDockerService = validHost.Services.GetRequiredService<Docker.AOT.IDockerService>();
            
            Console.WriteLine("\n2. 尝试操作不存在的容器");
            try
            {
                var result = await validDockerService.StopContainerAsync("non-existent-container-12345");
                if (result.Success)
                {
                    Console.WriteLine("成功: 这是预期之外的结果");
                }
                else
                {
                    Console.WriteLine($"预期失败: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获异常: {ex.GetType().Name}: {ex.Message}");
            }
            
            Console.WriteLine("\n3. 尝试使用无效的镜像名称");
            try
            {
                var result = await validDockerService.RunContainerAsync("invalid-image-name-12345:latest");
                if (result.Success)
                {
                    Console.WriteLine("成功: 这是预期之外的结果");
                }
                else
                {
                    Console.WriteLine($"预期失败: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获异常: {ex.GetType().Name}: {ex.Message}");
            }
            
            await host.RunAsync();
        }
    }
}
```

## 总结

以上示例展示了 Docker 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手 Docker 基本操作
2. 管理容器的完整生命周期（运行、停止、删除）
3. 进行镜像管理（拉取、删除、列出）
4. 使用命令行工具进行便捷操作
5. 处理各种错误情况

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。所有示例都支持 .NET 10 AOT 编译，提供极致的性能表现。
