# TPL 任务并行库技能

## 技能概述

TPL 任务并行库技能是一个基于 .NET Task Parallel Library 的并行计算解决方案，专为 .NET 10 平台优化，支持 AOT（Ahead-of-Time）编译，提供卓越的并行处理性能和内存效率。

### 主要特性

- **并行任务**：支持创建和管理并行任务，提高计算密集型操作的性能
- **数据流**：支持基于数据流的并行处理，适用于复杂的管道处理场景
- **并行 LINQ**：支持并行 LINQ 查询，简化数据处理代码
- **异步编程**：支持异步和等待模式，提高应用程序响应性
- **Scrutor 集成**：支持高级服务注册和装饰模式，提高代码可维护性
- **AOT 编译优化**：支持 .NET 10 AOT 编译，减少启动时间和内存占用
- **Docker 部署**：提供 Docker 容器化部署方案
- **云服务集成**：支持 Azure 和 AWS 云部署

### 技术栈

- **核心框架**：.NET 10、System.Threading.Tasks
- **并行处理**：System.Threading.Tasks.Dataflow、System.Linq.Parallel
- **依赖注入**：Microsoft.Extensions.DependencyInjection、Scrutor
- **命令行**：System.CommandLine
- **日志系统**：Microsoft.Extensions.Logging
- **缓存**：Microsoft.Extensions.Caching.Memory
- **文件系统**：System.IO.Abstractions
- **JSON 处理**：Newtonsoft.Json
- **响应式编程**：System.Reactive
- **不可变集合**：System.Collections.Immutable

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- Windows 10/11（64 位）
- Docker（可选，用于容器化部署）

### 安装

1. **克隆技能仓库**

   ```bash
   git clone https://github.com/your-repo/tpl-skill.git
   cd tpl-skill
   ```

2. **安装依赖**

   ```bash
   dotnet restore
   ```

3. **编译技能**

   ```bash
   dotnet build -c Release
   ```

### 基本使用

#### 任务操作

```bash
# 创建 10 个并行任务
dotnet run -- task create 10

# 执行任务等待
dotnet run -- task wait
```

#### 数据流操作

```bash
# 创建数据流管道
dotnet run -- dataflow pipeline 100

# 执行数据流块操作
dotnet run -- dataflow block 50
```

#### 并行操作

```bash
# 执行并行 For 循环
dotnet run -- parallel for 100

# 执行并行 LINQ 查询
dotnet run -- parallel linq 50
```

#### Scrutor 演示

```bash
# 执行 Scrutor 装饰器演示
dotnet run -- scrutor decorator
```

## 核心功能

### 1. 并行任务

- **任务创建**：支持创建和管理并行任务
- **任务等待**：支持等待单个或多个任务完成
- **任务取消**：支持取消正在执行的任务
- **任务延续**：支持任务完成后的延续操作
- **任务组合**：支持组合多个任务的结果

### 2. 数据流

- **数据流块**：支持不同类型的数据流块（转换、动作、批处理等）
- **管道构建**：支持构建复杂的数据流管道
- **并行处理**：支持数据流块的并行处理
- **错误处理**：支持数据流中的错误处理
- **完成传播**：支持数据流完成信号的传播

### 3. 并行 LINQ

- **并行查询**：支持并行 LINQ 查询
- **聚合操作**：支持并行聚合操作
- **排序操作**：支持并行排序操作
- **过滤操作**：支持并行过滤操作
- **映射操作**：支持并行映射操作

### 4. 异步编程

- **异步方法**：支持异步方法和等待模式
- **异步流**：支持 IAsyncEnumerable 异步流
- **异步任务**：支持异步任务的创建和管理
- **异步协调**：支持异步操作的协调

### 5. Scrutor 集成

- **服务注册**：支持基于约定的服务注册
- **装饰器模式**：支持服务装饰，实现横切关注点
- **服务筛选**：支持基于条件的服务注册
- **生命周期管理**：支持多种服务生命周期

## API 参考

### ITaskService

```csharp
public interface ITaskService
{
    // 创建并行任务
    Task<T[]> CreateTasks<T>(int count, Func<int, T> taskFunc);
    
    // 等待所有任务完成
    Task WaitAll(Task[] tasks);
    
    // 等待任意任务完成
    Task<Task> WaitAny(Task[] tasks);
    
    // 取消任务
    Task CancelTasks(CancellationTokenSource cts);
    
    // 组合任务结果
    Task<T[]> WhenAll<T>(Task<T>[] tasks);
}
```

### IDataflowService

```csharp
public interface IDataflowService
{
    // 创建数据流管道
    Task ProcessPipeline<T>(IEnumerable<T> items, Func<T, T> transformFunc);
    
    // 创建转换块
    TransformBlock<TInput, TOutput> CreateTransformBlock<TInput, TOutput>(Func<TInput, TOutput> transform);
    
    // 创建动作块
    ActionBlock<T> CreateActionBlock<T>(Action<T> action);
    
    // 创建批处理块
    BatchBlock<T> CreateBatchBlock<T>(int batchSize);
    
    // 链接数据流块
    void LinkBlocks<T>(ISourceBlock<T> source, ITargetBlock<T> target);
}
```

### IParallelService

```csharp
public interface IParallelService
{
    // 执行并行 For 循环
    void ParallelFor(int fromInclusive, int toExclusive, Action<int> body);
    
    // 执行并行 ForEach
    void ParallelForEach<T>(IEnumerable<T> source, Action<T> body);
    
    // 执行并行 LINQ 查询
    TResult[] ParallelLinq<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector);
    
    // 执行并行聚合
    TAccumulate ParallelAggregate<TSource, TAccumulate>(IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> updateAccumulator);
}
```

### IAsyncService

```csharp
public interface IAsyncService
{
    // 执行异步操作
    Task<T> ExecuteAsync<T>(Func<Task<T>> asyncFunc);
    
    // 执行异步流操作
    Task ProcessAsyncStream<T>(IAsyncEnumerable<T> stream, Func<T, Task> processFunc);
    
    // 异步延迟
    Task Delay(int millisecondsDelay);
    
    // 异步超时
    Task<T> WithTimeout<T>(Task<T> task, TimeSpan timeout);
}
```

### IScrutorDemoService

```csharp
public interface IScrutorDemoService
{
    // 执行基本服务注册演示
    void BasicRegistrationDemo();
    
    // 执行装饰器模式演示
    void DecoratorPatternDemo();
    
    // 执行服务筛选演示
    void ServiceFilteringDemo();
    
    // 执行生命周期管理演示
    void LifetimeManagementDemo();
}
```

## 命令行接口

### task 命令

用于执行任务相关操作。

**语法**：
```bash
dotnet run -- task <operation> [options]
```

**操作**：
- `create`：创建并行任务
- `wait`：等待任务完成
- `cancel`：取消任务
- `whenall`：等待所有任务完成

**选项**：
- `--count`：任务数量

### dataflow 命令

用于执行数据流相关操作。

**语法**：
```bash
dotnet run -- dataflow <operation> [options]
```

**操作**：
- `pipeline`：创建数据流管道
- `block`：创建数据流块
- `link`：链接数据流块

**选项**：
- `--items`：数据项数量

### parallel 命令

用于执行并行相关操作。

**语法**：
```bash
dotnet run -- parallel <operation> [options]
```

**操作**：
- `for`：执行并行 For 循环
- `foreach`：执行并行 ForEach
- `linq`：执行并行 LINQ 查询
- `aggregate`：执行并行聚合

**选项**：
- `--items`：项目数量

### scrutor 命令

用于执行 Scrutor 演示。

**语法**：
```bash
dotnet run -- scrutor <demo>
```

**演示**：
- `basic`：基本服务注册演示
- `decorator`：装饰器模式演示
- `filter`：服务筛选演示
- `lifetime`：生命周期管理演示

## AOT 编译

### 配置

AOT 编译配置已在 `index.yaml` 文件中设置：

```yaml
compilation:
  target_framework: net11.0
  publish_aot: true
  trim_mode: partial
  self_contained: true
  publish_single_file: true
  runtime_identifier: win-x64
  additional_options: -p:UseAppHost=true
```

### 执行 AOT 编译

```bash
# 发布为 AOT 编译的单文件可执行文件
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
```

### AOT 编译优势

- **更快的启动时间**：减少 JIT 编译开销
- **更小的内存占用**：移除未使用的代码
- **更好的安全性**：减少可攻击面
- **无需运行时依赖**：单文件可执行，易于分发

## 示例

### 1. 基本任务操作

```csharp
using System;
using System.Threading.Tasks;
using TPL.Skill;

var taskService = new TaskService();

// 创建 10 个并行任务
var tasks = taskService.CreateTasks(10, i => {
    Console.WriteLine($"Task {i} started");
    Task.Delay(100).Wait();
    Console.WriteLine($"Task {i} completed");
    return i;
});

// 等待所有任务完成
await taskService.WaitAll(tasks);
Console.WriteLine("All tasks completed!");
```

### 2. 数据流操作

```csharp
using System;
using System.Threading.Tasks.Dataflow;
using TPL.Skill;

var dataflowService = new DataflowService();

// 创建数据流管道
var items = Enumerable.Range(1, 100);
await dataflowService.ProcessPipeline(items, item => {
    Console.WriteLine($"Processing item {item}");
    return item * 2;
});

Console.WriteLine("Dataflow pipeline completed!");
```

### 3. 并行 LINQ 操作

```csharp
using System;
using System.Linq;
using TPL.Skill;

var parallelService = new ParallelService();

// 执行并行 LINQ 查询
var items = Enumerable.Range(1, 50);
var results = parallelService.ParallelLinq(items, item => {
    Console.WriteLine($"Processing item {item}");
    return item * item;
});

Console.WriteLine("Parallel LINQ results:");
foreach (var result in results)
{
    Console.WriteLine(result);
}
```

### 4. Scrutor 装饰器模式

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义服务接口和实现
interface ICalculator { int Add(int a, int b); }
class Calculator : ICalculator { public int Add(int a, int b) { Console.WriteLine($"Calculator.Add({a}, {b})"); return a + b; } }
class LoggingCalculator : ICalculator 
{
    private readonly ICalculator _calculator;
    public LoggingCalculator(ICalculator calculator) { _calculator = calculator; }
    public int Add(int a, int b) 
    {
        Console.WriteLine($"Logging before: {a} + {b}");
        var result = _calculator.Add(a, b);
        Console.WriteLine($"Logging after: {result}");
        return result;
    }
}

// 注册服务和装饰器
var services = new ServiceCollection();
services.AddSingleton<ICalculator, Calculator>();
services.Decorate<ICalculator, LoggingCalculator>();

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 解析服务
var calculator = serviceProvider.GetRequiredService<ICalculator>();

// 使用服务
var result = calculator.Add(5, 3);
Console.WriteLine($"Final result: {result}");
```

### 5. 完整的数据流管道

```csharp
using System;
using System.Threading.Tasks.Dataflow;
using System.Linq;

// 创建转换块
var transformBlock = new TransformBlock<int, int>(item => {
    Console.WriteLine($"Transforming item {item}");
    return item * 2;
});

// 创建动作块
var actionBlock = new ActionBlock<int>(item => {
    Console.WriteLine($"Processing item {item}");
});

// 链接块
transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

// 发布数据
foreach (var item in Enumerable.Range(1, 10))
{
    transformBlock.Post(item);
}

// 标记完成
transformBlock.Complete();

// 等待完成
actionBlock.Completion.Wait();

Console.WriteLine("Dataflow pipeline completed!");
```

## 故障排除

### 常见问题

1. **任务取消错误**

   **症状**：`TaskCanceledException`
   **原因**：任务被取消或超时
   **解决方案**：检查取消令牌使用是否正确，增加超时时间

2. **内存不足错误**

   **症状**：`OutOfMemoryException`
   **原因**：并行任务数量过多，内存占用过高
   **解决方案**：减少并行任务数量，使用数据流限制并发度

3. **死锁**

   **症状**：程序卡住，无响应
   **原因**：任务等待关系循环
   **解决方案**：避免嵌套的同步等待，使用 async/await

4. **AOT 编译错误**

   **症状**：`Trim analysis error`
   **原因**：反射或动态代码使用导致的裁剪错误
   **解决方案**：在项目文件中添加裁剪排除规则

5. **Docker 构建错误**

   **症状**：`failed to solve: process "..." did not complete successfully`
   **原因**：Dockerfile 配置错误或依赖缺失
   **解决方案**：检查 Dockerfile 配置并确保所有依赖都正确安装

### 调试技巧

1. **启用详细日志**

   ```bash
   dotnet run --verbosity detailed
   ```

2. **使用任务管理器**

   监控 CPU 和内存使用情况，识别性能瓶颈

3. **使用并行可视化工具**

   Visual Studio 中的并行任务窗口可以帮助调试并行任务

4. **检查 .NET 版本**

   ```bash
   dotnet --version
   ```

## 性能优化

### 1. 任务优化

- **任务粒度**：合理设置任务粒度，避免过多小任务
- **任务取消**：及时取消不需要的任务
- **任务延续**：使用任务延续替代嵌套等待
- **异步模式**：优先使用 async/await 模式

### 2. 数据流优化

- **并发度**：合理设置数据流块的 `MaxDegreeOfParallelism`
- **批处理**：使用批处理块减少上下文切换
- **缓冲区**：合理设置缓冲区大小
- **完成传播**：正确传播完成信号

### 3. 并行 LINQ 优化

- **分区策略**：选择合适的分区策略
- **负载均衡**：确保工作负载均匀分布
- **顺序保留**：仅在必要时保留顺序
- **聚合操作**：使用并行聚合减少同步开销

### 4. 内存优化

- **对象池**：使用对象池减少内存分配
- **不可变集合**：使用不可变集合减少内存占用
- **延迟加载**：使用延迟加载减少初始内存使用
- **内存监控**：监控内存使用情况，及时释放不需要的资源

## 扩展和定制

### 1. 添加自定义任务处理器

1. **创建任务处理器类**

   ```csharp
   public class CustomTaskProcessor : ITaskProcessor
   {
       public Task Process(int taskId)
       {
           Console.WriteLine($"Processing custom task {taskId}");
           return Task.Delay(50);
       }
   }
   ```

2. **注册任务处理器**

   ```csharp
   var services = new ServiceCollection();
   services.AddSingleton<ITaskProcessor, CustomTaskProcessor>();
   ```

### 2. 自定义数据流块

1. **创建自定义数据流块**

   ```csharp
   public class CustomTransformBlock<TInput, TOutput> : TransformBlock<TInput, TOutput>
   {
       public CustomTransformBlock(Func<TInput, TOutput> transform, int maxDegreeOfParallelism)
           : base(transform, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism })
       {
       }
   }
   ```

2. **使用自定义块**

   ```csharp
   var block = new CustomTransformBlock<int, int>(item => item * 2, 4);
   ```

### 3. 集成第三方库

1. **安装 NuGet 包**

   ```bash
   dotnet add package <PackageName>
   ```

2. **更新 index.yaml**

   ```yaml
   dependencies:
     - name: <PackageName>
       version: <Version>
   ```

3. **使用第三方库**

   ```csharp
   // 示例：使用 Reactive Extensions
   using System.Reactive.Linq;
   
   Observable.Range(1, 10)
       .Where(x => x % 2 == 0)
       .Select(x => x * 2)
       .Subscribe(x => Console.WriteLine(x));
   ```

## 部署指南

### 1. 本地部署

**步骤**：
1. 编译技能：`dotnet build -c Release`
2. 运行技能：`dotnet run -- <command> [options]`

### 2. AOT 编译部署

**步骤**：
1. 执行 AOT 编译：
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
   ```
2. 分发生成的单文件可执行文件

### 3. Docker 部署

**步骤**：
1. 创建 Dockerfile：
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS build
   WORKDIR /app
   
   # 复制项目文件
   COPY *.csproj .
   RUN dotnet restore
   
   # 复制源代码
   COPY . .
   
   # 构建项目
   RUN dotnet build -c Release
   
   # 发布项目
   RUN dotnet publish -c Release -o out
   
   # 运行时镜像
   FROM mcr.microsoft.com/dotnet/runtime:10.0-windowsservercore-ltsc2022
   WORKDIR /app
   COPY --from=build /app/out .
   
   # 设置入口点
   ENTRYPOINT ["tpl_core.exe"]
   ```

2. 构建 Docker 镜像：
   ```bash
   docker build -t tpl-skill .
   ```

3. 运行 Docker 容器：
   ```bash
   docker run --rm tpl-skill task create 5
   ```

### 4. 云部署

#### Azure 部署

**步骤**：
1. 创建 Azure 容器实例：
   ```bash
   az container create --name tpl-skill --image tpl-skill --resource-group myResourceGroup --command-line "task create 10"
   ```

2. 查看容器日志：
   ```bash
   az container logs --name tpl-skill --resource-group myResourceGroup
   ```

#### AWS 部署

**步骤**：
1. 创建 ECR 仓库：
   ```bash
   aws ecr create-repository --repository-name tpl-skill
   ```

2. 推送镜像到 ECR：
   ```bash
   docker tag tpl-skill:latest <aws-account-id>.dkr.ecr.<region>.amazonaws.com/tpl-skill:latest
   docker push <aws-account-id>.dkr.ecr.<region>.amazonaws.com/tpl-skill:latest
   ```

3. 运行 ECS 任务：
   ```bash
   aws ecs run-task --cluster myCluster --task-definition tpl-skill-task
   ```

## 贡献指南

### 开发流程

1. **Fork 仓库**
2. **创建特性分支**：`git checkout -b feature/your-feature`
3. **提交更改**：`git commit -m "Add your feature"`
4. **推送分支**：`git push origin feature/your-feature`
5. **创建 Pull Request**

### 代码规范

- 遵循 .NET 编码规范
- 使用 C# 10 语法特性
- 提供详细的代码注释
- 编写单元测试
- 确保代码通过 CI/CD 流程

### 测试

```bash
# 运行单元测试
dotnet test

# 运行集成测试
dotnet test --filter Category=Integration
```

### 文档

- 更新 SKILL.md 文档
- 为新功能添加示例
- 更新 API 参考
- 提供详细的使用说明

## 许可证

本技能采用 MIT 许可证。详见 [LICENSE](LICENSE) 文件。

## 联系方式

- **作者**：NET 专家
- **邮箱**：contact@net-expert.com
- **GitHub**：https://github.com/net-expert
- **网站**：https://net-expert.com

---

**© 2026 NET 专家. 保留所有权利.**
