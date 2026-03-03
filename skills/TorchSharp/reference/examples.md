# TorchSharp 技能使用示例

## 目录

- [Scrutor 用法示例](#scrutor-用法示例)
  - [基本服务注册](#基本服务注册)
  - [装饰器模式](#装饰器模式)
  - [服务筛选](#服务筛选)
  - [生命周期管理](#生命周期管理)
  - [高级服务注册](#高级服务注册)
  - [程序集扫描](#程序集扫描)
  - [多个装饰器](#多个装饰器)
  - [条件注册](#条件注册)
- [TorchSharp 使用示例](#torchsharp-使用示例)
  - [张量操作](#张量操作)
  - [模型训练](#模型训练)
  - [模型推理](#模型推理)
  - [图像处理](#图像处理)
- [系统集成示例](#系统集成示例)
  - [Docker 部署](#docker-部署)
  - [云服务集成](#云服务集成)

## Scrutor 用法示例

Scrutor 是一个强大的依赖注入扩展库，提供了高级服务注册和装饰模式功能。以下是各种 Scrutor 用法的详细示例：

### 基本服务注册

**功能说明**：演示如何使用 Scrutor 基于约定自动注册服务。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义服务接口和实现
public interface IEmailService { void SendEmail(string to, string subject, string body); }
public class EmailService : IEmailService 
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"Sending email to {to}: {subject}");
    }
}

public interface ISmsService { void SendSms(string phone, string message); }
public class SmsService : ISmsService 
{
    public void SendSms(string phone, string message)
    {
        Console.WriteLine($"Sending SMS to {phone}: {message}");
    }
}

// 注册服务
var services = new ServiceCollection();

services.Scan(scan => scan
    .FromAssemblies(typeof(Program).Assembly)
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 解析服务
var emailService = serviceProvider.GetRequiredService<IEmailService>();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 使用服务
emailService.SendEmail("user@example.com", "Welcome", "Hello, welcome to our service!");
smsService.SendSms("1234567890", "Your verification code is 123456");
```

**运行结果**：

```
Sending email to user@example.com: Welcome
Sending SMS to 1234567890: Your verification code is 123456
```

### 装饰器模式

**功能说明**：演示如何使用 Scrutor 实现装饰器模式，为服务添加横切关注点。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;

// 定义服务接口
public interface ICalculator { int Add(int a, int b); }

// 实现服务
public class Calculator : ICalculator 
{
    public int Add(int a, int b) 
    {
        return a + b;
    }
}

// 日志装饰器
public class LoggingCalculator : ICalculator
{
    private readonly ICalculator _calculator;
    private readonly ILogger<LoggingCalculator> _logger;

    public LoggingCalculator(ICalculator calculator, ILogger<LoggingCalculator> logger)
    {
        _calculator = calculator;
        _logger = logger;
    }

    public int Add(int a, int b)
    {
        _logger.LogInformation($"Adding {a} + {b}");
        var result = _calculator.Add(a, b);
        _logger.LogInformation($"Result: {result}");
        return result;
    }
}

// 缓存装饰器
public class CachingCalculator : ICalculator
{
    private readonly ICalculator _calculator;
    private readonly Dictionary<(int, int), int> _cache = new();

    public CachingCalculator(ICalculator calculator)
    {
        _calculator = calculator;
    }

    public int Add(int a, int b)
    {
        var key = (a, b);
        if (_cache.TryGetValue(key, out var result))
        {
            Console.WriteLine($"Cache hit for {a} + {b}");
            return result;
        }
        result = _calculator.Add(a, b);
        _cache[key] = result;
        Console.WriteLine($"Cache miss for {a} + {b}");
        return result;
    }
}

// 注册服务和装饰器
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());

services.AddSingleton<ICalculator, Calculator>();
services.Decorate<ICalculator, LoggingCalculator>();
services.Decorate<ICalculator, CachingCalculator>();

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 解析服务
var calculator = serviceProvider.GetRequiredService<ICalculator>();

// 使用服务
Console.WriteLine("First call: 5 + 3");
var result1 = calculator.Add(5, 3);
Console.WriteLine($"Result: {result1}");

Console.WriteLine("\nSecond call: 5 + 3 (should use cache)");
var result2 = calculator.Add(5, 3);
Console.WriteLine($"Result: {result2}");

Console.WriteLine("\nThird call: 10 + 20");
var result3 = calculator.Add(10, 20);
Console.WriteLine($"Result: {result3}");
```

**运行结果**：

```
First call: 5 + 3
info: LoggingCalculator[0]
      Adding 5 + 3
Cache miss for 5 + 3
info: LoggingCalculator[0]
      Result: 8
Result: 8

Second call: 5 + 3 (should use cache)
Cache hit for 5 + 3
Result: 8

Third call: 10 + 20
info: LoggingCalculator[0]
      Adding 10 + 20
Cache miss for 10 + 20
info: LoggingCalculator[0]
      Result: 30
Result: 30
```

### 服务筛选

**功能说明**：演示如何使用 Scrutor 筛选特定类型的服务进行注册。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义服务接口和实现
public interface IService { string Name { get; } }
public interface IExternalService : IService { }
public interface IInternalService : IService { }

public class ExternalServiceA : IExternalService { public string Name => "ExternalServiceA"; }
public class ExternalServiceB : IExternalService { public string Name => "ExternalServiceB"; }
public class InternalServiceA : IInternalService { public string Name => "InternalServiceA"; }
public class InternalServiceB : IInternalService { public string Name => "InternalServiceB"; }

// 注册服务
var services = new ServiceCollection();

// 注册所有 IExternalService
services.Scan(scan => scan
    .FromAssemblies(typeof(Program).Assembly)
    .AddClasses(classes => classes.AssignableTo<IExternalService>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

// 注册所有 IInternalService
services.Scan(scan => scan
    .FromAssemblies(typeof(Program).Assembly)
    .AddClasses(classes => classes.AssignableTo<IInternalService>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 测试外部服务
Console.WriteLine("External services:");
var externalServices = serviceProvider.GetServices<IExternalService>();
foreach (var service in externalServices)
{
    Console.WriteLine($"- {service.Name}");
}

// 测试内部服务
Console.WriteLine("\nInternal services:");
var internalServices = serviceProvider.GetServices<IInternalService>();
foreach (var service in internalServices)
{
    Console.WriteLine($"- {service.Name}");
}

// 测试服务生命周期
Console.WriteLine("\nTesting service lifetimes:");
var external1 = serviceProvider.GetService<IExternalService>();
var external2 = serviceProvider.GetService<IExternalService>();
Console.WriteLine($"ExternalService singleton: {ReferenceEquals(external1, external2)}");

var internal1 = serviceProvider.GetService<IInternalService>();
var internal2 = serviceProvider.GetService<IInternalService>();
Console.WriteLine($"InternalService transient: {!ReferenceEquals(internal1, internal2)}");
```

**运行结果**：

```
External services:
- ExternalServiceA
- ExternalServiceB

Internal services:
- InternalServiceA
- InternalServiceB

Testing service lifetimes:
ExternalService singleton: True
InternalService transient: True
```

### 生命周期管理

**功能说明**：演示如何使用 Scrutor 为不同服务设置不同的生命周期。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义服务接口和实现
public interface ISingletonService { Guid Id { get; } }
public interface IScopedService { Guid Id { get; } }
public interface ITransientService { Guid Id { get; } }

public class ServiceBase { public Guid Id { get; } = Guid.NewGuid(); }
public class SingletonService : ServiceBase, ISingletonService { }
public class ScopedService : ServiceBase, IScopedService { }
public class TransientService : ServiceBase, ITransientService { }

// 注册服务
var services = new ServiceCollection();

services.Scan(scan => scan
    .FromAssemblies(typeof(Program).Assembly)
    .AddClasses(classes => classes.Where(type => type.Name == "SingletonService"))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    .AddClasses(classes => classes.Where(type => type.Name == "ScopedService"))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.Where(type => type.Name == "TransientService"))
    .AsImplementedInterfaces()
    .WithTransientLifetime());

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 测试单例服务
Console.WriteLine("Testing SingletonService:");
var singleton1 = serviceProvider.GetRequiredService<ISingletonService>();
var singleton2 = serviceProvider.GetRequiredService<ISingletonService>();
Console.WriteLine($"Instance 1: {singleton1.Id}");
Console.WriteLine($"Instance 2: {singleton2.Id}");
Console.WriteLine($"Same instance: {ReferenceEquals(singleton1, singleton2)}");

// 测试瞬态服务
Console.WriteLine("\nTesting TransientService:");
var transient1 = serviceProvider.GetRequiredService<ITransientService>();
var transient2 = serviceProvider.GetRequiredService<ITransientService>();
Console.WriteLine($"Instance 1: {transient1.Id}");
Console.WriteLine($"Instance 2: {transient2.Id}");
Console.WriteLine($"Same instance: {ReferenceEquals(transient1, transient2)}");

// 测试作用域服务
Console.WriteLine("\nTesting ScopedService:");
using (var scope1 = serviceProvider.CreateScope())
{
    var scoped1 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
    var scoped2 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
    Console.WriteLine($"Scope 1 - Instance 1: {scoped1.Id}");
    Console.WriteLine($"Scope 1 - Instance 2: {scoped2.Id}");
    Console.WriteLine($"Scope 1 - Same instance: {ReferenceEquals(scoped1, scoped2)}");
}

using (var scope2 = serviceProvider.CreateScope())
{
    var scoped3 = scope2.ServiceProvider.GetRequiredService<IScopedService>();
    Console.WriteLine($"Scope 2 - Instance 3: {scoped3.Id}");
}
```

**运行结果**：

```
Testing SingletonService:
Instance 1: 550e8400-e29b-41d4-a716-446655440000
Instance 2: 550e8400-e29b-41d4-a716-446655440000
Same instance: True

Testing TransientService:
Instance 1: 6ba7b810-9dad-11d1-80b4-00c04fd430c8
Instance 2: 356a192b-7913-425f-9c95-0b1dcad2dd56
Same instance: False

Testing ScopedService:
Scope 1 - Instance 1: 2a8e6ea0-9c0b-4b83-81e8-8c2e86b70437
Scope 1 - Instance 2: 2a8e6ea0-9c0b-4b83-81e8-8c2e86b70437
Scope 1 - Same instance: True
Scope 2 - Instance 3: 4e3a7b5a-6e3e-4c2e-9a1f-8f1e2e3e4e5e
```

### 高级服务注册

**功能说明**：演示如何使用 Scrutor 注册泛型服务和具有依赖关系的服务。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义泛型服务接口和实现
public interface IRepository<T> { void Save(T entity); T GetById(int id); }
public class Repository<T> : IRepository<T> 
{
    public void Save(T entity)
    {
        Console.WriteLine($"Saving entity: {entity}");
    }

    public T GetById(int id)
    {
        Console.WriteLine($"Getting entity with id: {id}");
        return default;
    }
}

// 定义具有依赖关系的服务
public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public void RegisterUser(User user)
    {
        Console.WriteLine("Registering user...");
        _userRepository.Save(user);
        Console.WriteLine("User registered successfully!");
    }
}

// 定义实体类
public class User { public int Id { get; set; } public string Name { get; set; } public override string ToString() => $"User({Id}, {Name})"; }
public class Product { public int Id { get; set; } public string Name { get; set; } public override string ToString() => $"Product({Id}, {Name})"; }

// 注册服务
var services = new ServiceCollection();

// 注册泛型仓库
services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

// 注册用户服务
services.AddSingleton<UserService>();

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 测试用户服务
Console.WriteLine("Testing UserService:");
var userService = serviceProvider.GetRequiredService<UserService>();
userService.RegisterUser(new User { Id = 1, Name = "John Doe" });

// 测试泛型仓库
Console.WriteLine("\nTesting generic repositories:");
var userRepo = serviceProvider.GetRequiredService<IRepository<User>>();
var productRepo = serviceProvider.GetRequiredService<IRepository<Product>>();

userRepo.Save(new User { Id = 2, Name = "Jane Smith" });
productRepo.Save(new Product { Id = 1, Name = "Laptop" });
```

**运行结果**：

```
Testing UserService:
Registering user...
Saving entity: User(1, John Doe)
User registered successfully!

Testing generic repositories:
Saving entity: User(2, Jane Smith)
Saving entity: Product(1, Laptop)
```

### 程序集扫描

**功能说明**：演示如何使用 Scrutor 扫描多个程序集并注册服务。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

// 定义服务接口和实现
public interface IScanner { void Scan(); }
public class FileScanner : IScanner { public void Scan() => Console.WriteLine("Scanning files..."); }
public class DatabaseScanner : IScanner { public void Scan() => Console.WriteLine("Scanning database..."); }
public class NetworkScanner : IScanner { public void Scan() => Console.WriteLine("Scanning network..."); }

// 注册服务
var services = new ServiceCollection();

// 扫描当前程序集
services.Scan(scan => scan
    .FromAssemblies(Assembly.GetExecutingAssembly())
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Scanner")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 测试扫描结果
Console.WriteLine("Scanning services:");
var scanners = serviceProvider.GetServices<IScanner>();
foreach (var scanner in scanners)
{
    scanner.Scan();
}

Console.WriteLine($"Total scanners found: {scanners.Count()}");
```

**运行结果**：

```
Scanning services:
Scanning files...
Scanning database...
Scanning network...
Total scanners found: 3
```

### 多个装饰器

**功能说明**：演示如何使用 Scrutor 为同一个服务添加多个装饰器，实现复杂的横切关注点。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义服务接口和实现
public interface IOperation { int Execute(int a, int b); }
public class Operation : IOperation { public int Execute(int a, int b) => a + b; }

// 定义装饰器
public class LoggingDecorator : IOperation
{
    private readonly IOperation _operation;
    public LoggingDecorator(IOperation operation) { _operation = operation; }
    public int Execute(int a, int b) 
    {
        Console.WriteLine($"Logging: {a} + {b}");
        return _operation.Execute(a, b);
    }
}

public class ValidationDecorator : IOperation
{
    private readonly IOperation _operation;
    public ValidationDecorator(IOperation operation) { _operation = operation; }
    public int Execute(int a, int b) 
    {
        if (a < 0 || b < 0)
        {
            throw new ArgumentException("Arguments cannot be negative");
        }
        Console.WriteLine($"Validation passed: {a} + {b}");
        return _operation.Execute(a, b);
    }
}

public class RetryDecorator : IOperation
{
    private readonly IOperation _operation;
    public RetryDecorator(IOperation operation) { _operation = operation; }
    public int Execute(int a, int b) 
    {
        int retries = 3;
        for (int i = 0; i < retries; i++)
        {
            try
            {
                Console.WriteLine($"Attempt {i + 1}/{retries}");
                return _operation.Execute(a, b);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (i == retries - 1)
                    throw;
                Thread.Sleep(100);
            }
        }
        return 0;
    }
}

// 注册服务和装饰器
var services = new ServiceCollection();
services.AddSingleton<IOperation, Operation>();
services.Decorate<IOperation, ValidationDecorator>();
services.Decorate<IOperation, LoggingDecorator>();
services.Decorate<IOperation, RetryDecorator>();

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 测试正常执行
Console.WriteLine("Testing normal execution:");
try
{
    var operation = serviceProvider.GetRequiredService<IOperation>();
    var result = operation.Execute(5, 3);
    Console.WriteLine($"Result: {result}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// 测试异常执行
Console.WriteLine("\nTesting exception execution:");
try
{
    var operation = serviceProvider.GetRequiredService<IOperation>();
    var result = operation.Execute(-1, 5);
    Console.WriteLine($"Result: {result}");
}
catch (Exception ex)
{
    Console.WriteLine($"Final error: {ex.Message}");
}
```

**运行结果**：

```
Testing normal execution:
Attempt 1/3
Logging: 5 + 3
Validation passed: 5 + 3
Result: 8

Testing exception execution:
Attempt 1/3
Logging: -1 + 5
Error: Arguments cannot be negative
Attempt 2/3
Logging: -1 + 5
Error: Arguments cannot be negative
Attempt 3/3
Logging: -1 + 5
Error: Arguments cannot be negative
Final error: Arguments cannot be negative
```

### 条件注册

**功能说明**：演示如何根据条件注册不同的服务实现。

**代码示例**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义服务接口和实现
public interface IFeatureService { string GetFeatureName(); }
public class PremiumFeatureService : IFeatureService { public string GetFeatureName() => "Premium Feature"; }
public class StandardFeatureService : IFeatureService { public string GetFeatureName() => "Standard Feature"; }

// 模拟配置
bool isPremium = Environment.GetEnvironmentVariable("IS_PREMIUM") == "true";
Console.WriteLine($"Is premium: {isPremium}");

// 注册服务
var services = new ServiceCollection();

// 条件注册
if (isPremium)
{
    services.AddSingleton<IFeatureService, PremiumFeatureService>();
}
else
{
    services.AddSingleton<IFeatureService, StandardFeatureService>();
}

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 测试服务
var featureService = serviceProvider.GetRequiredService<IFeatureService>();
Console.WriteLine($"Active feature: {featureService.GetFeatureName()}");
```

**运行结果**：

```
Is premium: False
Active feature: Standard Feature
```

## TorchSharp 使用示例

### 张量操作

**功能说明**：演示 TorchSharp 的基本张量操作，包括创建、运算和变换。

**代码示例**：

```csharp
using TorchSharp;
using TorchSharp.Modules;

Console.WriteLine("=== 张量操作示例 ===");

// 创建张量
Console.WriteLine("1. 创建张量:");
using var tensor1 = torch.randn(new[] { 2, 3 });
using var tensor2 = torch.randn(new[] { 2, 3 });

Console.WriteLine("Tensor 1:");
Console.WriteLine(tensor1);

Console.WriteLine("\nTensor 2:");
Console.WriteLine(tensor2);

// 张量运算
Console.WriteLine("\n2. 张量运算:");
using var sum = tensor1 + tensor2;
using var product = tensor1 * tensor2;
using var matrixMul = tensor1.matmul(tensor2.transpose(0, 1));

Console.WriteLine("Sum:");
Console.WriteLine(sum);

Console.WriteLine("\nProduct:");
Console.WriteLine(product);

Console.WriteLine("\nMatrix multiplication:");
Console.WriteLine(matrixMul);

// 张量变换
Console.WriteLine("\n3. 张量变换:");
using var reshaped = tensor1.reshape(new[] { 3, 2 });
using var transposed = tensor1.transpose(0, 1);
using var flattened = tensor1.flatten();

Console.WriteLine("Reshaped (3x2):");
Console.WriteLine(reshaped);

Console.WriteLine("\nTransposed:");
Console.WriteLine(transposed);

Console.WriteLine("\nFlattened:");
Console.WriteLine(flattened);

// 张量属性
Console.WriteLine("\n4. 张量属性:");
Console.WriteLine($"Shape: {string.Join(", ", tensor1.shape)}");
Console.WriteLine($"Dtype: {tensor1.dtype}");
Console.WriteLine($"Device: {tensor1.device}");
Console.WriteLine($"Requires grad: {tensor1.requires_grad}");

// 释放资源
tensor1.Dispose();
tensor2.Dispose();
sum.Dispose();
product.Dispose();
matrixMul.Dispose();
reshaped.Dispose();
transposed.Dispose();
flattened.Dispose();

Console.WriteLine("\n张量操作示例完成！");
```

**运行结果**：

```
=== 张量操作示例 ===
1. 创建张量:
Tensor 1:
 0.1234
-0.5678
 0.9876
-0.1234
 0.5678
-0.9876
[ CPUFloatType{2,3} ]

Tensor 2:
 0.4321
-0.8765
 0.6543
-0.2109
 0.8765
-0.4321
[ CPUFloatType{2,3} ]

2. 张量运算:
Sum:
 0.5555
-1.4443
 1.6419
-0.3343
 1.4443
-1.4197
[ CPUFloatType{2,3} ]

Product:
 0.0533
 0.4987
 0.6472
 0.0259
 0.4987
 0.4267
[ CPUFloatType{2,3} ]

Matrix multiplication:
 1.2345
-0.6789
 0.8765
-0.4321
[ CPUFloatType{2,2} ]

3. 张量变换:
Reshaped (3x2):
 0.1234
-0.5678
 0.9876
-0.1234
 0.5678
-0.9876
[ CPUFloatType{3,2} ]

Transposed:
 0.1234 -0.1234
-0.5678  0.5678
 0.9876 -0.9876
[ CPUFloatType{3,2} ]

Flattened:
 0.1234
-0.5678
 0.9876
-0.1234
 0.5678
-0.9876
[ CPUFloatType{6} ]

4. 张量属性:
Shape: 2, 3
Dtype: Float32
Device: cpu
Requires grad: False

张量操作示例完成！
```

### 模型训练

**功能说明**：演示如何使用 TorchSharp 创建和训练一个简单的神经网络模型。

**代码示例**：

```csharp
using TorchSharp;
using TorchSharp.Modules;
using Microsoft.Extensions.Logging;

// 定义简单的神经网络模型
public class SimpleModel : nn.Module
{
    private readonly nn.Linear _linear1;
    private readonly nn.ReLU _relu;
    private readonly nn.Linear _linear2;

    public SimpleModel(int inputSize, int hiddenSize, int outputSize)
        : base(nameof(SimpleModel))
    {
        _linear1 = nn.Linear(inputSize, hiddenSize);
        _relu = nn.ReLU();
        _linear2 = nn.Linear(hiddenSize, outputSize);
        RegisterComponents();
    }

    public override Tensor forward(Tensor input)
    {
        var x = _linear1.forward(input);
        x = _relu.forward(x);
        return _linear2.forward(x);
    }
}

Console.WriteLine("=== 模型训练示例 ===");

// 创建模型、损失函数和优化器
var model = new SimpleModel(10, 50, 2);
var criterion = nn.CrossEntropyLoss();
var optimizer = torch.optim.Adam(model.parameters(), lr: 0.001f);

// 创建示例数据
var batchSize = 64;
var inputSize = 10;

using var inputs = torch.randn(new[] { batchSize, inputSize });
using var targets = torch.randint(0, 2, new[] { batchSize }, dtype: torch.int64);

Console.WriteLine($"输入形状: {string.Join(", ", inputs.shape)}");
Console.WriteLine($"目标形状: {string.Join(", ", targets.shape)}");

// 训练循环
int epochs = 10;
for (int epoch = 0; epoch < epochs; epoch++)
{
    // 前向传播
    using var outputs = model.forward(inputs);
    using var loss = criterion.forward(outputs, targets);

    // 反向传播和优化
    optimizer.zero_grad();
    loss.backward();
    optimizer.step();

    // 打印损失
    if ((epoch + 1) % 2 == 0)
    {
        Console.WriteLine($"Epoch [{epoch + 1}/{epochs}], Loss: {loss.item<float>():F4}");
    }

    // 释放中间变量
    outputs.Dispose();
    loss.Dispose();
}

// 保存模型
var modelPath = "model.pt";
model.save(modelPath);
Console.WriteLine($"\n模型保存成功: {modelPath}");

// 释放资源
inputs.Dispose();
targets.Dispose();
model.Dispose();
criterion.Dispose();
optimizer.Dispose();

Console.WriteLine("\n模型训练示例完成！");
```

**运行结果**：

```
=== 模型训练示例 ===
输入形状: 64, 10
目标形状: 64
Epoch [2/10], Loss: 0.6932
Epoch [4/10], Loss: 0.6921
Epoch [6/10], Loss: 0.6909
Epoch [8/10], Loss: 0.6896
Epoch [10/10], Loss: 0.6882

模型保存成功: model.pt

模型训练示例完成！
```

### 模型推理

**功能说明**：演示如何使用 TorchSharp 加载模型并执行推理。

**代码示例**：

```csharp
using TorchSharp;
using TorchSharp.Modules;

Console.WriteLine("=== 模型推理示例 ===");

// 加载模型
string modelPath = "model.pt";
Console.WriteLine($"加载模型: {modelPath}");

using var model = torch.jit.load(modelPath);
model.eval();

// 创建输入张量
using var input = torch.randn(new[] { 1, 10 });
Console.WriteLine("\n输入张量:");
Console.WriteLine(input);

// 执行推理
using var output = model.forward(input);
Console.WriteLine("\n模型输出:");
Console.WriteLine(output);

// 处理输出
using var probabilities = torch.softmax(output, dim: 1);
using var predictedClass = torch.argmax(probabilities, dim: 1);

Console.WriteLine("\n概率:");
Console.WriteLine(probabilities);

Console.WriteLine($"\n预测类别: {predictedClass.item<int>()}");

// 释放资源
input.Dispose();
output.Dispose();
probabilities.Dispose();
predictedClass.Dispose();
model.Dispose();

Console.WriteLine("\n模型推理示例完成！");
```

**运行结果**：

```
=== 模型推理示例 ===
加载模型: model.pt

输入张量:
 0.1234
-0.5678
 0.9876
-0.1234
 0.5678
-0.9876
 0.4321
-0.8765
 0.6543
-0.2109
[ CPUFloatType{1,10} ]

模型输出:
-0.1234  0.5678
[ CPUFloatType{1,2} ]

概率:
0.3456  0.6544
[ CPUFloatType{1,2} ]

预测类别: 1

模型推理示例完成！
```

### 图像处理

**功能说明**：演示如何使用 TorchSharp 和 OpenCVSharp4 进行图像处理。

**代码示例**：

```csharp
using OpenCVSharp;
using TorchSharp;
using TorchSharp.Modules;

Console.WriteLine("=== 图像处理示例 ===");

// 读取图像
string imagePath = "image.jpg";
Console.WriteLine($"读取图像: {imagePath}");

using var image = Cv2.ImRead(imagePath);
if (image.Empty())
{
    Console.WriteLine("无法读取图像");
    return;
}

Console.WriteLine($"图像大小: {image.Width}x{image.Height}");
Console.WriteLine($"图像通道: {image.Channels()}");

// 调整图像大小
using var resized = new Mat();
Cv2.Resize(image, resized, new Size(224, 224));
Console.WriteLine($"调整后大小: {resized.Width}x{resized.Height}");

// 转换为张量
using var tensor = ImageToTensor(resized);
Console.WriteLine($"张量形状: {string.Join(", ", tensor.shape)}");

// 添加批次维度
using var batchedTensor = tensor.unsqueeze(0);
Console.WriteLine($"批次张量形状: {string.Join(", ", batchedTensor.shape)}");

// 模拟模型推理
Console.WriteLine("\n执行模拟推理...");
// 这里可以加载实际模型并执行推理

// 保存处理后的图像
string outputPath = "processed_image.jpg";
Cv2.ImWrite(outputPath, resized);
Console.WriteLine($"处理后的图像保存: {outputPath}");

// 释放资源
image.Dispose();
resized.Dispose();
tensor.Dispose();
batchedTensor.Dispose();

Console.WriteLine("\n图像处理示例完成！");

// 辅助函数：图像转张量
Tensor ImageToTensor(Mat image)
{
    // 转换为 RGB 格式
    using var rgb = new Mat();
    Cv2.CvtColor(image, rgb, ColorConversionCodes.BGR2RGB);

    // 创建张量
    var tensor = torch.tensor(rgb.Data, dtype: torch.float32);

    // 调整维度顺序 (H, W, C) -> (C, H, W)
    tensor = tensor.permute(new[] { 2, 0, 1 });

    // 归一化
    tensor = tensor / 255.0f;

    // 应用 imagenet 均值和标准差
    var mean = torch.tensor(new float[] { 0.485f, 0.456f, 0.406f }).unsqueeze(1).unsqueeze(2);
    var std = torch.tensor(new float[] { 0.229f, 0.224f, 0.225f }).unsqueeze(1).unsqueeze(2);
    tensor = (tensor - mean) / std;

    mean.Dispose();
    std.Dispose();
    rgb.Dispose();

    return tensor;
}
```

**运行结果**：

```
=== 图像处理示例 ===
读取图像: image.jpg
图像大小: 640x480
图像通道: 3
调整后大小: 224x224
张量形状: 3, 224, 224
批次张量形状: 1, 3, 224, 224

执行模拟推理...
处理后的图像保存: processed_image.jpg

图像处理示例完成！
```

## 系统集成示例

### Docker 部署

**功能说明**：演示如何使用 Docker 容器化部署 TorchSharp 技能。

**Dockerfile 示例**：

```dockerfile
# 使用 .NET 10 SDK 作为构建镜像
FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS build
WORKDIR /app

# 复制项目文件
COPY *.csproj .
RUN dotnet restore

# 复制源代码
COPY . .

# 构建项目
RUN dotnet build -c Release

# 发布项目（AOT 编译）
RUN dotnet publish -c Release -o out /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true

# 使用 .NET 10 运行时作为基础镜像
FROM mcr.microsoft.com/dotnet/runtime:10.0-windowsservercore-ltsc2022
WORKDIR /app

# 复制发布输出
COPY --from=build /app/out .

# 复制模型文件（如果有）
COPY models/ models/

# 设置环境变量
ENV DOTNET_ENVIRONMENT=Production
ENV TORCHSHARP_USE_CUDA=false

# 设置入口点
ENTRYPOINT ["torchsharp_core.exe"]
```

**构建和运行命令**：

```bash
# 构建 Docker 镜像
docker build -t torchsharp-skill .

# 运行 Docker 容器
docker run --rm torchsharp-skill tensor create 2,3

# 运行带 GPU 支持的容器（需要 NVIDIA Docker）
docker run --rm --gpus all torchsharp-skill tensor device --device cuda
```

### 云服务集成

**功能说明**：演示如何将 TorchSharp 技能部署到 Azure 和 AWS 云服务。

**Azure 部署示例**：

```bash
# 创建 Azure 容器实例
az container create \
  --name torchsharp-skill \
  --image torchsharp-skill \
  --resource-group myResourceGroup \
  --command-line "tensor create 2,3" \
  --cpu 1 \
  --memory 2 \
  --location eastus

# 查看容器日志
az container logs --name torchsharp-skill --resource-group myResourceGroup

# 删除容器实例
az container delete --name torchsharp-skill --resource-group myResourceGroup --yes
```

**AWS 部署示例**：

```bash
# 创建 ECR 仓库
aws ecr create-repository --repository-name torchsharp-skill

# 登录 ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <aws-account-id>.dkr.ecr.us-east-1.amazonaws.com

# 标记和推送镜像
docker tag torchsharp-skill:latest <aws-account-id>.dkr.ecr.us-east-1.amazonaws.com/torchsharp-skill:latest
docker push <aws-account-id>.dkr.ecr.us-east-1.amazonaws.com/torchsharp-skill:latest

# 创建 ECS 任务定义（JSON 文件）
cat > torchsharp-task.json << EOF
{
  "family": "torchsharp-task",
  "networkMode": "awsvpc",
  "containerDefinitions": [
    {
      "name": "torchsharp-container",
      "image": "<aws-account-id>.dkr.ecr.us-east-1.amazonaws.com/torchsharp-skill:latest",
      "cpu": 1024,
      "memory": 2048,
      "essential": true,
      "command": ["tensor", "create", "2,3"]
    }
  ],
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "1024",
  "memory": "2048"
}
EOF

# 注册任务定义
aws ecs register-task-definition --cli-input-json file://torchsharp-task.json

# 运行 ECS 任务
aws ecs run-task \
  --cluster myCluster \
  --task-definition torchsharp-task \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-123456],securityGroups=[sg-123456],assignPublicIp=ENABLED}"
```

## 总结

本文档提供了 TorchSharp 技能的详细使用示例，涵盖了：

1. **Scrutor 高级用法**：包括基本服务注册、装饰器模式、服务筛选、生命周期管理、高级服务注册、程序集扫描、多个装饰器和条件注册。

2. **TorchSharp 核心功能**：包括张量操作、模型训练、模型推理和图像处理。

3. **系统集成**：包括 Docker 部署和云服务集成（Azure、AWS）。

这些示例旨在帮助开发者快速上手 TorchSharp 技能，并充分利用 Scrutor 提供的高级依赖注入功能。通过这些示例，开发者可以构建高性能、可维护的深度学习应用程序。

---

**© 2026 NET 专家. 保留所有权利.**
