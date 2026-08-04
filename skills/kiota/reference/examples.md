# Kiota 技能使用示例

## 快速开始

### 1. 基本使用示例

#### 1.1 生成 API 客户端示例

```bash
# 生成 C# 客户端
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./PetStoreClient csharp PetStore

# 使用命令别名
kiota_aot g https://petstore.swagger.io/v2/swagger.json ./PetStoreClient csharp PetStore

# 生成 TypeScript 客户端
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./PetStoreClient typescript PetStore

# 生成 Java 客户端
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./PetStoreClient java com.petstore
```

#### 1.2 验证 OpenAPI 规范示例

```bash
# 验证远程规范
kiota_aot validate https://petstore.swagger.io/v2/swagger.json

# 验证本地规范
kiota_aot validate ./swagger.json

# 使用命令别名
kiota_aot v https://petstore.swagger.io/v2/swagger.json
```

#### 1.3 列出支持的语言示例

```bash
# 列出支持的语言
kiota_aot list-languages

# 使用命令别名
kiota_aot ll
```

#### 1.4 下载 OpenAPI 规范示例

```bash
# 下载规范到本地文件
kiota_aot download-spec https://petstore.swagger.io/v2/swagger.json ./swagger.json

# 使用命令别名
kiota_aot ds https://petstore.swagger.io/v2/swagger.json ./swagger.json

# 下载到指定目录
kiota_aot download-spec https://petstore.swagger.io/v2/swagger.json ./specs/petstore.json
```

#### 1.5 缓存管理示例

```bash
# 清除缓存
kiota_aot cache-clear

# 显示缓存信息
kiota_aot cache-info

# 使用命令别名
kiota_aot cc
kiota_aot ci
```

#### 1.6 性能测试示例

```bash
# 运行性能测试
kiota_aot benchmark https://petstore.swagger.io/v2/swagger.json 5

# 使用命令别名
kiota_aot bm https://petstore.swagger.io/v2/swagger.json 5
```

## 高级使用示例

### 2. 编程集成示例

#### 2.1 基本集成示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class KiotaIntegrationExample
{
    public static async Task Main()
    {
        Console.WriteLine("Kiota 集成示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<KiotaService>()
            .BuildServiceProvider();
        
        var kiotaService = serviceProvider.GetRequiredService<KiotaService>();
        var logger = serviceProvider.GetRequiredService<ILogger<KiotaIntegrationExample>>();
        
        try
        {
            // 验证规范
            logger.LogInformation("验证 OpenAPI 规范...");
            await kiotaService.ValidateSpecAsync("https://petstore.swagger.io/v2/swagger.json");
            
            // 生成客户端
            logger.LogInformation("\n生成 API 客户端...");
            await kiotaService.GenerateClientAsync(
                "https://petstore.swagger.io/v2/swagger.json",
                "./GeneratedClient",
                "csharp",
                "PetStoreClient"
            );
            
            // 列出支持的语言
            logger.LogInformation("\n支持的语言:");
            await kiotaService.ListSupportedLanguagesAsync();
            
        } catch (Exception ex)
        {
            logger.LogError(ex, "操作失败");
        }
    }
}
```

#### 2.2 批量操作示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class KiotaBatchExample
{
    public static async Task Main()
    {
        Console.WriteLine("Kiota 批量操作示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<KiotaService>()
            .BuildServiceProvider();
        
        var kiotaService = serviceProvider.GetRequiredService<KiotaService>();
        var logger = serviceProvider.GetRequiredService<ILogger<KiotaBatchExample>>();
        
        // 批量生成客户端
        var specs = new List<(string url, string output, string language, string ns)>
        {
            ("https://petstore.swagger.io/v2/swagger.json", "./Clients/PetStore", "csharp", "PetStore"),
            ("https://api.github.com/openapi", "./Clients/GitHub", "typescript", "GitHub"),
            ("https://api.microsoft.com/v1.0/openapi", "./Clients/Microsoft", "java", "com.microsoft")
        };
        
        foreach (var (url, output, language, ns) in specs)
        {
            try
            {
                logger.LogInformation($"生成 {language} 客户端 for {url}");
                await kiotaService.GenerateClientAsync(url, output, language, ns);
                logger.LogInformation($"成功生成 {language} 客户端");
            } catch (Exception ex)
            {
                logger.LogError(ex, $"生成 {language} 客户端失败");
            }
            Console.WriteLine();
        }
    }
}
```

## 使用场景示例

### 3. API 客户端生成场景

#### 3.1 多语言客户端生成

```bash
# 为同一 API 生成多种语言的客户端
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./Clients/csharp csharp PetStore
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./Clients/typescript typescript PetStore
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./Clients/java java com.petstore

# 查看生成的客户端
ls -la ./Clients/
```

#### 3.2 类型安全客户端

```bash
# 生成类型安全的 C# 客户端
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./TypeSafeClient csharp PetStore

# 使用生成的客户端
# 在项目中引用生成的客户端代码
# using PetStore;
# var client = new ApiClient();
# var pets = await client.GetPetsAsync();
```

### 4. 微服务通信场景

#### 4.1 服务间通信客户端

```bash
# 为微服务 API 生成客户端
kiota_aot generate http://service-a:8080/openapi.json ./ServiceAClient csharp ServiceA
kiota_aot generate http://service-b:8080/openapi.json ./ServiceBClient csharp ServiceB

# 在服务中使用客户端
# var serviceAClient = new ServiceA.ApiClient("http://service-a:8080");
# var result = await serviceAClient.GetDataAsync();
```

#### 4.2 接口标准化

```bash
# 下载并验证服务接口规范
kiota_aot download-spec http://service-a:8080/openapi.json ./specs/service-a.json
kiota_aot validate ./specs/service-a.json

# 为所有服务生成统一风格的客户端
kiota_aot generate ./specs/service-a.json ./Clients/service-a csharp Company.ServiceA
kiota_aot generate ./specs/service-b.json ./Clients/service-b csharp Company.ServiceB
```

### 5. 第三方 API 集成场景

#### 5.1 快速集成第三方 API

```bash
# 生成 GitHub API 客户端
kiota_aot generate https://api.github.com/openapi ./GitHubClient csharp GitHub

# 生成 Azure API 客户端
kiota_aot generate https://management.azure.com/swagger.json ./AzureClient csharp Azure

# 生成 Google API 客户端
kiota_aot generate https://api.google.com/openapi.json ./GoogleClient csharp Google
```

#### 5.2 文档同步

```bash
# 创建定时任务，定期更新 API 客户端
# 示例 PowerShell 脚本
# $specUrl = "https://api.example.com/openapi.json"
# $outputDir = "./ApiClient"
# $language = "csharp"
# $namespace = "Example.Api"
# kiota_aot generate $specUrl $outputDir $language $namespace

# 运行脚本
# .\Update-ApiClient.ps1
```

### 6. CI/CD 集成场景

#### 6.1 自动化生成客户端

```bash
# 在 CI/CD 流程中添加客户端生成步骤
# 示例 GitHub Actions 工作流
# name: Generate API Client
# on: [push]
# jobs:
#   generate-client:
#     runs-on: ubuntu-latest
#     steps:
#       - uses: actions/checkout@v3
#       - name: Setup .NET
#         uses: actions/setup-dotnet@v3
#         with:
#           dotnet-version: '10.0.x'
#       - name: Generate Client
#         run: |
#           dotnet run --project ./kiota_aot.cs generate https://api.example.com/openapi.json ./Client csharp Example.Api
#       - name: Commit Changes
#         run: |
#           git config --global user.name 'github-actions[bot]'
#           git config --global user.email 'github-actions[bot]@users.noreply.github.com'
#           git add ./Client
#           git commit -m 'Update API client'
#           git push
```

#### 6.2 规范验证

```bash
# 在 CI/CD 流程中添加规范验证步骤
# 示例 GitHub Actions 工作流
# name: Validate OpenAPI Spec
# on: [push]
# jobs:
#   validate-spec:
#     runs-on: ubuntu-latest
#     steps:
#       - uses: actions/checkout@v3
#       - name: Setup .NET
#         uses: actions/setup-dotnet@v3
#         with:
#           dotnet-version: '10.0.x'
#       - name: Validate Spec
#         run: |
#           dotnet run --project ./kiota_aot.cs validate ./openapi.json
```

### 7. 自动化测试场景

#### 7.1 测试客户端生成

```bash
# 为测试环境 API 生成测试客户端
kiota_aot generate https://test-api.example.com/openapi.json ./TestClient csharp Test.Api

# 在测试中使用客户端
# var testClient = new Test.Api.ApiClient("https://test-api.example.com");
# var response = await testClient.GetDataAsync();
# Assert.Equal(200, response.StatusCode);
```

#### 7.2 性能测试

```bash
# 运行性能测试
kiota_aot benchmark https://api.example.com/openapi.json 10

# 分析测试结果
# 查看平均响应时间、最大响应时间等指标
# 根据测试结果优化 API 性能
```

## 性能优化示例

### 8. 客户端生成优化

#### 8.1 缓存优化

```bash
# 第一次生成客户端（会缓存规范）
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./Client1 csharp PetStore

# 第二次生成客户端（使用缓存，速度更快）
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./Client2 csharp PetStore

# 清除缓存
kiota_aot cache-clear

# 第三次生成客户端（重新下载规范）
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./Client3 csharp PetStore
```

#### 8.2 内存优化

```bash
# 设置较大的内存限制以提高生成速度
# 在 kiota_aot.run.json 中设置
# "memory": {
#   "maximum": 2048
# }

# 运行生成
kiota_aot generate https://large-api.example.com/openapi.json ./LargeClient csharp LargeApi
```

### 9. 规范处理优化

#### 9.1 本地缓存

```bash
# 下载规范到本地
kiota_aot download-spec https://petstore.swagger.io/v2/swagger.json ./local-spec.json

# 使用本地规范生成客户端（速度更快）
kiota_aot generate ./local-spec.json ./LocalClient csharp PetStore

# 定期更新本地规范
# kiota_aot download-spec https://petstore.swagger.io/v2/swagger.json ./local-spec.json
```

#### 9.2 增量验证

```bash
# 验证本地规范
kiota_aot validate ./local-spec.json

# 修改规范后再次验证
# kiota_aot validate ./local-spec.json
```

## 错误处理示例

### 10. 常见错误处理

#### 10.1 网络错误处理

```bash
# 尝试连接到不可访问的 URL
kiota_aot validate https://nonexistent-api.example.com/openapi.json

# 输出示例：
# Error: No such host is known. (nonexistent-api.example.com:443)

# 处理方法：检查网络连接，确认 URL 是否正确
```

#### 10.2 无效规范处理

```bash
# 验证无效的规范
kiota_aot validate ./invalid-spec.json

# 输出示例：
# Error: OpenAPI规范验证失败: 无效的规范文件

# 处理方法：检查规范文件是否完整，是否符合 OpenAPI 规范要求
```

#### 10.3 权限错误处理

```bash
# 尝试访问需要认证的规范
kiota_aot validate https://secure-api.example.com/openapi.json

# 输出示例：
# Error: Response status code does not indicate success: 401 (Unauthorized).

# 处理方法：提供正确的认证信息，或使用已认证的客户端
```

## 总结

本文档提供了 Kiota 技能的各种使用示例，涵盖了从基本操作到高级场景的各种应用。通过这些示例，您可以：

1. 快速上手 Kiota 的基本操作
2. 掌握 API 客户端生成的各种场景
3. 了解如何在微服务通信中使用 Kiota
4. 学习如何集成第三方 API
5. 掌握 CI/CD 集成和自动化测试的方法
6. 优化 Kiota 的性能
7. 处理常见的错误情况

Kiota 技能采用 .NET 10 的 AOT 编译技术，实现了高性能运行，支持单文件执行，适用于各种规模和复杂度的项目。通过合理使用 Kiota，您可以大大减少 API 集成的工作量，提高代码质量和开发效率。