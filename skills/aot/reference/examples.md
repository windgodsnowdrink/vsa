# AOT Compilation - 使用示例

## 快速开始

### 1. AOT 反射基础使用

```csharp
using System;
using AotReflection;

// 定义一个示例类
public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
    
    public bool IsAdult()
    {
        return Age >= 18;
    }
}

// 主程序
public class Program
{
    public static void Main()
    {
        Console.WriteLine("AOT反射使用示例");
        Console.WriteLine("=" * 40);
        
        // 初始化AOT反射系统
        AotReflection.Initialize();
        
        // 创建示例对象
        var user = new User
        {
            FirstName = "张三",
            LastName = "李四",
            Email = "zhangsan@example.com",
            Age = 30
        };
        
        // 获取类型信息
        var typeInfo = AotReflection.GetTypeInfo(typeof(User));
        Console.WriteLine($"类型: {typeInfo.Name}");
        Console.WriteLine($"命名空间: {typeInfo.Namespace}");
        Console.WriteLine($"是否为类: {typeInfo.IsClass}");
        
        // 获取并调用方法
        var methodInfo = AotReflection.GetMethod(typeof(User), "GetFullName");
        var fullName = methodInfo.Invoke(user, null);
        Console.WriteLine($"\n调用 GetFullName() 方法:");
        Console.WriteLine($"  结果: {fullName}");
        
        // 访问属性
        var emailProperty = AotReflection.GetProperty(typeof(User), "Email");
        Console.WriteLine($"\n访问 Email 属性:");
        Console.WriteLine($"  当前值: {emailProperty.GetValue(user)}");
        
        // 设置属性
        emailProperty.SetValue(user, "new.email@example.com");
        Console.WriteLine($"  更新后: {emailProperty.GetValue(user)}");
        
        // 调用带条件的方法
        var isAdultMethod = AotReflection.GetMethod(typeof(User), "IsAdult");
        var isAdult = (bool)isAdultMethod.Invoke(user, null);
        Console.WriteLine($"\n调用 IsAdult() 方法:");
        Console.WriteLine($"  结果: {isAdult}");
    }
}
```

### 2. 静态反射示例

```csharp
using System;
using AotReflection;

// 使用静态反射属性注册类型
[StaticTypeInfo(typeof(User))]
public class UserProcessor
{
    public void ProcessUser(User user)
    {
        Console.WriteLine("\n静态反射示例:");
        
        // 使用预编译的类型信息
        var typeInfo = StaticTypeCache<User>.TypeInfo;
        Console.WriteLine($"  类型名称: {typeInfo.Name}");
        Console.WriteLine($"  方法数量: {typeInfo.Methods.Count}");
        Console.WriteLine($"  属性数量: {typeInfo.Properties.Count}");
        
        // 直接访问预编译的元数据
        Console.WriteLine("\n  预编译的方法列表:");
        foreach (var method in typeInfo.Methods)
        {
            Console.WriteLine($"    • {method.Name}");
        }
        
        Console.WriteLine("\n  预编译的属性列表:");
        foreach (var property in typeInfo.Properties)
        {
            Console.WriteLine($"    • {property.Name} (类型: {property.PropertyType.Name})");
        }
    }
}

// 在主程序中使用
public class Program
{
    public static void Main()
    {
        // ... 之前的代码 ...
        
        // 使用静态反射处理器
        var processor = new UserProcessor();
        processor.ProcessUser(user);
    }
}
```

### 3. 元数据缓存示例

```csharp
using System;
using System.Diagnostics;
using AotReflection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("\n" + "=" * 40);
        Console.WriteLine("元数据缓存性能测试");
        Console.WriteLine("=" * 40);
        
        // 测试反射性能
        var stopwatch = Stopwatch.StartNew();
        
        // 第一次访问 - 会创建缓存
        for (int i = 0; i < 1000; i++)
        {
            var typeInfo = AotReflection.GetTypeInfo(typeof(User));
        }
        
        stopwatch.Stop();
        Console.WriteLine($"第一次访问 (1000次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 第二次访问 - 从缓存获取
        stopwatch.Restart();
        for (int i = 0; i < 1000; i++)
        {
            var typeInfo = AotReflection.GetTypeInfo(typeof(User));
        }
        stopwatch.Stop();
        Console.WriteLine($"第二次访问 (1000次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 测试方法调用缓存
        Console.WriteLine("\n方法调用缓存测试:");
        var methodInfo = AotReflection.GetMethod(typeof(User), "GetFullName");
        stopwatch.Restart();
        for (int i = 0; i < 1000; i++)
        {
            methodInfo.Invoke(user, null);
        }
        stopwatch.Stop();
        Console.WriteLine($"方法调用 (1000次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 查看缓存统计
        var cacheStats = AotReflection.GetCacheStatistics();
        Console.WriteLine("\n缓存统计:");
        Console.WriteLine($"  类型缓存项数量: {cacheStats.TypeCacheCount}");
        Console.WriteLine($"  方法缓存项数量: {cacheStats.MethodCacheCount}");
        Console.WriteLine($"  属性缓存项数量: {cacheStats.PropertyCacheCount}");
        Console.WriteLine($"  缓存命中率: {cacheStats.CacheHitRate:P2}");
    }
}
```

### 4. 动态方法调用示例

```csharp
using System;
using AotReflection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("\n" + "=" * 40);
        Console.WriteLine("动态方法调用示例");
        Console.WriteLine("=" * 40);
        
        // 注册自定义类型
        Console.WriteLine("注册自定义类型...");
        AotReflection.RegisterType(typeof(Order));
        
        // 创建订单对象
        var order = new Order
        {
            OrderId = "ORD-2026-0001",
            CustomerName = "张三",
            TotalAmount = 1500.50m,
            Status = OrderStatus.Pending
        };
        
        // 动态获取并调用方法
        var processMethod = AotReflection.GetMethod(typeof(Order), "ProcessOrder");
        Console.WriteLine($"\n调用 ProcessOrder() 方法:");
        processMethod.Invoke(order, null);
        Console.WriteLine($"  订单状态: {order.Status}");
        
        // 调用带有参数的方法
        var updateAmountMethod = AotReflection.GetMethod(typeof(Order), "UpdateAmount");
        Console.WriteLine($"\n调用 UpdateAmount(decimal) 方法:");
        updateAmountMethod.Invoke(order, new object[] { 2000.75m });
        Console.WriteLine($"  更新后金额: {order.TotalAmount:C}");
        
        // 获取所有方法
        var orderTypeInfo = AotReflection.GetTypeInfo(typeof(Order));
        Console.WriteLine($"\nOrder 类型的所有方法:");
        foreach (var method in orderTypeInfo.Methods)
        {
            Console.WriteLine($"  • {method.Name}()");
        }
    }
}

// 订单示例类
public class Order
{
    public string OrderId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    
    public void ProcessOrder()
    {
        Status = OrderStatus.Processing;
    }
    
    public void UpdateAmount(decimal newAmount)
    {
        TotalAmount = newAmount;
    }
    
    public void CompleteOrder()
    {
        Status = OrderStatus.Completed;
    }
    
    public void CancelOrder()
    {
        Status = OrderStatus.Cancelled;
    }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Cancelled
}
```

### 5. AOT 配置示例

```csharp
using System;
using AotReflection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("\n" + "=" * 40);
        Console.WriteLine("AOT 配置示例");
        Console.WriteLine("=" * 40);
        
        // 自定义配置
        var settings = new AotReflectionSettings
        {
            EnableCache = true,
            CacheSize = 2000,
            EnableDetailedLogging = true,
            MaxReflectionDepth = 3,
            EnableTypeValidation = true
        };
        
        // 使用自定义配置初始化
        Console.WriteLine("使用自定义配置初始化 AOT 反射:");
        Console.WriteLine($"  启用缓存: {settings.EnableCache}");
        Console.WriteLine($"  缓存大小: {settings.CacheSize}");
        Console.WriteLine($"  启用详细日志: {settings.EnableDetailedLogging}");
        Console.WriteLine($"  最大反射深度: {settings.MaxReflectionDepth}");
        Console.WriteLine($"  启用类型验证: {settings.EnableTypeValidation}");
        
        AotReflection.Initialize(settings);
        
        // 执行一些操作以生成日志
        var typeInfo = AotReflection.GetTypeInfo(typeof(User));
        var methodInfo = AotReflection.GetMethod(typeof(User), "GetFullName");
        
        // 查看最近的日志
        Console.WriteLine("\n最近的日志记录:");
        var logs = AotReflection.GetRecentLogs(TimeSpan.FromSeconds(10));
        foreach (var log in logs.Take(5)) // 只显示最近5条
        {
            Console.WriteLine($"  [{log.Timestamp:HH:mm:ss.fff}] {log.Message}");
        }
        
        // 清除特定类型的缓存
        Console.WriteLine($"\n清除 User 类型的缓存:");
        AotReflection.ClearCache(typeof(User));
        
        // 检查缓存状态
        var cacheStats = AotReflection.GetCacheStatistics();
        Console.WriteLine($"  缓存项数量: {cacheStats.TypeCacheCount}");
    }
}
```

### 6. 性能对比示例

```csharp
using System;
using System.Diagnostics;
using System.Reflection;
using AotReflection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("\n" + "=" * 40);
        Console.WriteLine("性能对比测试");
        Console.WriteLine("=" * 40);
        
        // 初始化AOT反射
        AotReflection.Initialize();
        
        var user = new User
        {
            FirstName = "张三",
            LastName = "李四",
            Email = "test@example.com",
            Age = 30
        };
        
        const int iterations = 100000;
        
        // 标准反射性能测试
        Console.WriteLine("\n标准反射性能测试:");
        var standardStopwatch = Stopwatch.StartNew();
        var standardType = typeof(User);
        var standardMethod = standardType.GetMethod("GetFullName");
        
        for (int i = 0; i < iterations; i++)
        {
            standardMethod.Invoke(user, null);
        }
        
        standardStopwatch.Stop();
        Console.WriteLine($"  {iterations}次调用耗时: {standardStopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  平均每次调用: {standardStopwatch.Elapsed.TotalMilliseconds / iterations * 1000:F3} ns");
        
        // AOT反射性能测试
        Console.WriteLine("\nAOT反射性能测试:");
        var aotStopwatch = Stopwatch.StartNew();
        var aotMethod = AotReflection.GetMethod(typeof(User), "GetFullName");
        
        for (int i = 0; i < iterations; i++)
        {
            aotMethod.Invoke(user, null);
        }
        
        aotStopwatch.Stop();
        Console.WriteLine($"  {iterations}次调用耗时: {aotStopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  平均每次调用: {aotStopwatch.Elapsed.TotalMilliseconds / iterations * 1000:F3} ns");
        
        // 直接调用性能测试（作为基准）
        Console.WriteLine("\n直接调用性能测试（基准）:");
        var directStopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            user.GetFullName();
        }
        
        directStopwatch.Stop();
        Console.WriteLine($"  {iterations}次调用耗时: {directStopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  平均每次调用: {directStopwatch.Elapsed.TotalMilliseconds / iterations * 1000:F3} ns");
        
        // 计算性能提升
        var performanceImprovement = standardStopwatch.Elapsed.TotalMilliseconds / aotStopwatch.Elapsed.TotalMilliseconds;
        Console.WriteLine($"\n性能对比:");
        Console.WriteLine($"  AOT反射 vs 标准反射: {performanceImprovement:F2}x 性能提升");
        Console.WriteLine($"  直接调用 vs AOT反射: {(aotStopwatch.Elapsed.TotalMilliseconds / directStopwatch.Elapsed.TotalMilliseconds):F2}x 性能差异");
    }
}
```

### 7. 自定义扩展示例

```csharp
using System;
using System.Collections.Generic;
using AotReflection;

// 自定义反射提供者
public class CustomReflectionProvider : IReflectionProvider
{
    public TypeInfo GetTypeInfo(Type type)
    {
        Console.WriteLine($"自定义提供者处理类型: {type.Name}");
        
        // 实现自定义类型信息获取逻辑
        var methods = new List<MethodInfo>();
        var properties = new List<PropertyInfo>();
        
        // 获取所有公共方法
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (!method.IsSpecialName) // 排除属性访问器
            {
                methods.Add(new MethodInfo
                {
                    Name = method.Name,
                    DeclaringType = method.DeclaringType,
                    ReturnType = method.ReturnType,
                    Parameters = method.GetParameters().Select(p => p.ParameterType).ToList(),
                    Invoke = (instance, args) => method.Invoke(instance, args)
                });
            }
        }
        
        // 获取所有公共属性
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            properties.Add(new PropertyInfo
            {
                Name = property.Name,
                DeclaringType = property.DeclaringType,
                PropertyType = property.PropertyType,
                GetValue = instance => property.GetValue(instance),
                SetValue = (instance, value) => property.SetValue(instance, value)
            });
        }
        
        return new TypeInfo
        {
            Name = type.Name,
            Namespace = type.Namespace,
            IsClass = type.IsClass,
            IsAbstract = type.IsAbstract,
            IsInterface = type.IsInterface,
            Methods = methods,
            Properties = properties
        };
    }
    
    // 其他方法实现...
    public MethodInfo GetMethod(Type type, string methodName)
    {
        var typeInfo = GetTypeInfo(type);
        return typeInfo.Methods.FirstOrDefault(m => m.Name == methodName);
    }
    
    public PropertyInfo GetProperty(Type type, string propertyName)
    {
        var typeInfo = GetTypeInfo(type);
        return typeInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("\n" + "=" * 40);
        Console.WriteLine("自定义反射提供者示例");
        Console.WriteLine("=" * 40);
        
        // 注册自定义提供者
        Console.WriteLine("注册自定义反射提供者...");
        AotReflection.RegisterProvider(new CustomReflectionProvider());
        
        // 使用自定义提供者
        var user = new User
        {
            FirstName = "张三",
            LastName = "李四"
        };
        
        Console.WriteLine("\n使用自定义提供者获取类型信息:");
        var typeInfo = AotReflection.GetTypeInfo(typeof(User));
        Console.WriteLine($"  方法数量: {typeInfo.Methods.Count}");
        Console.WriteLine($"  属性数量: {typeInfo.Properties.Count}");
        
        Console.WriteLine("\n使用自定义提供者调用方法:");
        var fullName = AotReflection.GetMethod(typeof(User), "GetFullName")?.Invoke(user, null);
        Console.WriteLine($"  结果: {fullName}");
    }
}
```

## 总结

以上示例展示了 AOT Compilation System 的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手 AOT 反射基础操作
2. 利用静态反射提升性能
3. 管理和优化元数据缓存
4. 动态调用方法和访问属性
5. 配置和调整 AOT 反射系统
6. 对比 AOT 反射与标准反射的性能
7. 实现自定义扩展和提供者

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种 AOT 编译场景使用。