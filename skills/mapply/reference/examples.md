# mapply - 使用示例

## 快速开始

### 1. Mapster 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mapster;
using MapsterMapper;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        Console.WriteLine("mapply Mapster 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 定义源对象
        var source = new User {
            Id = 1,
            FirstName = "张",
            LastName = "三",
            Age = 30,
            Email = "zhangsan@example.com"
        };
        
        Console.WriteLine("源对象:");
        Console.WriteLine($"Id: {source.Id}, 姓名: {source.FirstName} {source.LastName}, 年龄: {source.Age}, 邮箱: {source.Email}");
        
        // 使用 Mapster 进行对象映射
        var userDto = mapper.Map<UserDto>(source);
        
        Console.WriteLine("\n映射结果 (UserDto):");
        Console.WriteLine($"用户ID: {userDto.UserId}, 全名: {userDto.FullName}, 年龄: {userDto.Age}, 邮箱: {userDto.Email}");
        
        // 测试列表映射
        var users = new List<User> {
            new User { Id = 1, FirstName = "张", LastName = "三", Age = 30 },
            new User { Id = 2, FirstName = "李", LastName = "四", Age = 25 }
        };
        
        var userDtos = mapper.Map<List<UserDto>>(users);
        Console.WriteLine("\n列表映射结果:");
        foreach (var dto in userDtos)
        {
            Console.WriteLine($"ID: {dto.UserId}, 姓名: {dto.FullName}, 年龄: {dto.Age}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.UserId, src => src.Id)
            .Map(dest => dest.FullName, src => $"{src.FirstName}{src.LastName}");
        
        // 注册 Mapster
        builder.AddSingleton(config);
        builder.AddScoped<IMapper, ServiceMapper>();
        
        return builder.BuildServiceProvider();
    }
}

// 源对象
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}

// 目标对象
public class UserDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}
```

### 2. Mapperly 使用示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("mapply Mapperly 使用示例");
        Console.WriteLine("=" * 50);
        
        // 定义源对象
        var customer = new Customer {
            Id = 1,
            Name = "王五",
            Address = "北京市朝阳区",
            Phone = "13800138000",
            Email = "wangwu@example.com"
        };
        
        Console.WriteLine("源对象 (Customer):");
        Console.WriteLine($"ID: {customer.Id}, 姓名: {customer.Name}, 地址: {customer.Address}, 电话: {customer.Phone}, 邮箱: {customer.Email}");
        
        // 使用 Mapperly 生成的映射器
        var customerDto = CustomerMapper.MapToDto(customer);
        
        Console.WriteLine("\n映射结果 (CustomerDto):");
        Console.WriteLine($"客户ID: {customerDto.CustomerId}, 姓名: {customerDto.Name}, 地址: {customerDto.Address}, 联系电话: {customerDto.PhoneNumber}, 邮箱: {customerDto.Email}");
    }
}

// 源对象
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// 目标对象
public class CustomerDto
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Mapperly 映射器
[Mapperly.Abstractions.Mapper]
public static partial class CustomerMapper
{
    [Mapperly.Abstractions.MapProperty("Id", "CustomerId")]
    [Mapperly.Abstractions.MapProperty("Phone", "PhoneNumber")]
    public static partial CustomerDto MapToDto(Customer customer);
}
```

### 3. Autofac 依赖注入示例

```csharp
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("mapply Autofac 依赖注入示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ContainerBuilder();
        
        // 注册服务
        builder.RegisterModule<CoreModule>();
        
        // 构建容器
        var container = builder.Build();
        
        // 解析服务
        using var scope = container.BeginLifetimeScope();
        var orderService = scope.Resolve<IOrderService>();
        var productService = scope.Resolve<IProductService>();
        
        // 使用服务
        var order = orderService.CreateOrder(1, "测试订单");
        Console.WriteLine($"创建订单: {order}");
        
        var product = productService.GetProduct(1);
        Console.WriteLine($"获取产品: {product}");
    }
}

// 核心模块
public class CoreModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册服务
        builder.RegisterType<OrderService>().As<IOrderService>();
        builder.RegisterType<ProductService>().As<IProductService>();
        builder.RegisterType<CustomerService>().As<ICustomerService>();
        
        // 注册单例服务
        builder.RegisterType<CacheService>().As<ICacheService>().SingleInstance();
        
        // 注册泛型服务
        builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
    }
}

// 服务接口
public interface IOrderService { string CreateOrder(int id, string name); }
public interface IProductService { string GetProduct(int id); }
public interface ICustomerService { string GetCustomer(int id); }
public interface ICacheService { void Set(string key, object value); }
public interface IRepository<T> { T Get(int id); }

// 服务实现
public class OrderService : IOrderService
{
    public string CreateOrder(int id, string name) => $"订单 #{id}: {name}";
}

public class ProductService : IProductService
{
    public string GetProduct(int id) => $"产品 #{id}";
}

public class CustomerService : ICustomerService
{
    public string GetCustomer(int id) => $"客户 #{id}";
}

public class CacheService : ICacheService
{
    public void Set(string key, object value) => Console.WriteLine($"缓存设置: {key} = {value}");
}

public class Repository<T> : IRepository<T>
{
    public T Get(int id) => default;
}
```

### 4. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mapster;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("mapply 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        // 准备测试数据
        var users = new List<User>();
        for (int i = 0; i < 1000; i++)
        {
            users.Add(new User {
                Id = i,
                FirstName = $"用户{i}",
                LastName = "测试",
                Age = 30 + i % 20,
                Email = $"user{i}@example.com"
            });
        }
        
        Console.WriteLine($"测试数据: {users.Count} 个用户对象");
        
        // 性能测试
        const int iterations = 10;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            // 批量映射测试
            var userDtos = mapper.Map<List<UserDto>>(users);
            Console.WriteLine($"迭代 {i+1}/{iterations}: 映射 {userDtos.Count} 个对象");
        }
        
        stopwatch.Stop();
        Console.WriteLine($"\n性能测试结果:");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均每次执行: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        Console.WriteLine($"总映射对象数: {users.Count * iterations}");
        Console.WriteLine($"映射速度: {(users.Count * iterations) / stopwatch.Elapsed.TotalSeconds:F2} 对象/秒");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 Mapster 全局设置
        var config = TypeAdapterConfig.GlobalSettings;
        config.Default.PreserveReferenceBehavior = PreserveReferenceBehavior.All;
        config.Default.IgnoreNullValues = true;
        
        // 注册 Mapster
        builder.AddSingleton(config);
        builder.AddScoped<IMapper, ServiceMapper>();
        
        return builder.BuildServiceProvider();
    }
}

// 源对象
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}

// 目标对象
public class UserDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}

// 配置映射
TypeAdapterConfig<User, UserDto>.NewConfig()
    .Map(dest => dest.UserId, src => src.Id)
    .Map(dest => dest.FullName, src => $"{src.FirstName}{src.LastName}");
```

### 5. AOT 编译示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package Mapster@8.5.0 
#:package Mapperly@3.11.0 
#:package Autofac@8.0.0 
#:property LangVersion=preview 
#:property TargetFramework=net10.0 
#:property Nullable=enable 
#:property ImplicitUsings=enable 
#:property PublishAot=true 
#:property IncludeNativeLibrariesForSelfExtract=true 
#:property EnableCppCodeGen=true 
#:property PublishSingleFile=true 
#:property SelfContained=true 
#:property RuntimeIdentifier=win-x64 

using System;
using Microsoft.Extensions.DependencyInjection;
using Mapster;

public class AotExample
{
    public static void Main()
    {
        Console.WriteLine("mapply AOT 编译示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("使用 AOT 编译的单文件可执行程序");
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        // 测试对象映射
        var source = new SourceObject {
            Id = 1,
            Name = "测试对象",
            Description = "这是一个 AOT 编译测试",
            Value = 123.45
        };
        
        Console.WriteLine("源对象:");
        Console.WriteLine($"Id: {source.Id}, Name: {source.Name}, Description: {source.Description}, Value: {source.Value}");
        
        // 执行映射
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var target = mapper.Map<TargetObject>(source);
        stopwatch.Stop();
        
        Console.WriteLine("\n映射结果:");
        Console.WriteLine($"ObjectId: {target.ObjectId}, ObjectName: {target.ObjectName}, Description: {target.Description}, NumericValue: {target.NumericValue}");
        Console.WriteLine($"\n映射执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        Console.WriteLine("\nAOT 编译示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.NewConfig<SourceObject, TargetObject>()
            .Map(dest => dest.ObjectId, src => src.Id)
            .Map(dest => dest.ObjectName, src => src.Name)
            .Map(dest => dest.NumericValue, src => src.Value);
        
        // 注册服务
        builder.AddSingleton(config);
        builder.AddScoped<IMapper, ServiceMapper>();
        
        return builder.BuildServiceProvider();
    }
}

// 源对象
public class SourceObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Value { get; set; }
}

// 目标对象
public class TargetObject
{
    public int ObjectId { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double NumericValue { get; set; }
}
```

## 总结

以上示例展示了 mapply 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本的对象映射操作
2. 使用 Mapster 进行灵活的运行时映射
3. 使用 Mapperly 进行高性能的编译时映射
4. 使用 Autofac 进行依赖注入和 AOP
5. 优化映射性能
6. 使用 AOT 编译提升性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 支持的功能

- **对象映射**：支持简单对象和复杂对象的映射
- **集合映射**：支持列表、数组等集合类型的映射
- **自定义映射**：支持自定义映射规则和转换逻辑
- **依赖注入**：与 Autofac 集成，支持复杂的依赖注入场景
- **AOP 拦截**：支持方法拦截和日志记录
- **性能优化**：支持对象池、缓存等性能优化技术
- **AOT 编译**：支持 AOT 编译，提升启动速度和运行性能

### 性能优化特点

- **编译时映射**：使用 Mapperly 生成编译时映射代码，消除反射开销
- **运行时缓存**：使用 Mapster 的缓存机制，减少重复映射开销
- **对象池**：减少对象创建和垃圾回收
- **内存优化**：使用 Span 和 Memory 减少内存分配
- **并行处理**：支持批量映射的并行处理
