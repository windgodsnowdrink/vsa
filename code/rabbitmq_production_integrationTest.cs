#load "rabbitmq_production_integration.cs"

Console.WriteLine("=== rabbitmq_production_integration.cs Test ===");

try
{
    // 验证 class: AdvancedRabbitMQFeatures
    var type_AdvancedRabbitMQFeatures = Type.GetType("AdvancedRabbitMQFeatures");
    if (type_AdvancedRabbitMQFeatures != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedRabbitMQFeatures (class) 存在");
        var ctors_AdvancedRabbitMQFeatures = type_AdvancedRabbitMQFeatures.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedRabbitMQFeatures 构造函数数量: {ctors_AdvancedRabbitMQFeatures.Length}");
        var methods_AdvancedRabbitMQFeatures = type_AdvancedRabbitMQFeatures.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedRabbitMQFeatures 公开方法数量: {methods_AdvancedRabbitMQFeatures.Length}");
        foreach (var m in methods_AdvancedRabbitMQFeatures)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedRabbitMQFeatures 未找到，尝试无命名空间...");
        type_AdvancedRabbitMQFeatures = Type.GetType("AdvancedRabbitMQFeatures");
        if (type_AdvancedRabbitMQFeatures != null)
            Console.WriteLine("[PASS] 类型 AdvancedRabbitMQFeatures (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedRabbitMQFeatures 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RabbitMQOptions
    var type_RabbitMQOptions = Type.GetType("RabbitMQOptions");
    if (type_RabbitMQOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RabbitMQOptions (class) 存在");
        var ctors_RabbitMQOptions = type_RabbitMQOptions.GetConstructors();
        Console.WriteLine($"[PASS] RabbitMQOptions 构造函数数量: {ctors_RabbitMQOptions.Length}");
        var methods_RabbitMQOptions = type_RabbitMQOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RabbitMQOptions 公开方法数量: {methods_RabbitMQOptions.Length}");
        foreach (var m in methods_RabbitMQOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RabbitMQOptions 未找到，尝试无命名空间...");
        type_RabbitMQOptions = Type.GetType("RabbitMQOptions");
        if (type_RabbitMQOptions != null)
            Console.WriteLine("[PASS] 类型 RabbitMQOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RabbitMQOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RabbitMQProducer
    var type_RabbitMQProducer = Type.GetType("RabbitMQProducer");
    if (type_RabbitMQProducer != null)
    {
        Console.WriteLine("[PASS] 类型 RabbitMQProducer (class) 存在");
        var ctors_RabbitMQProducer = type_RabbitMQProducer.GetConstructors();
        Console.WriteLine($"[PASS] RabbitMQProducer 构造函数数量: {ctors_RabbitMQProducer.Length}");
        var methods_RabbitMQProducer = type_RabbitMQProducer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RabbitMQProducer 公开方法数量: {methods_RabbitMQProducer.Length}");
        foreach (var m in methods_RabbitMQProducer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RabbitMQProducer 未找到，尝试无命名空间...");
        type_RabbitMQProducer = Type.GetType("RabbitMQProducer");
        if (type_RabbitMQProducer != null)
            Console.WriteLine("[PASS] 类型 RabbitMQProducer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RabbitMQProducer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RabbitMQConsumer
    var type_RabbitMQConsumer = Type.GetType("RabbitMQConsumer");
    if (type_RabbitMQConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 RabbitMQConsumer (class) 存在");
        var ctors_RabbitMQConsumer = type_RabbitMQConsumer.GetConstructors();
        Console.WriteLine($"[PASS] RabbitMQConsumer 构造函数数量: {ctors_RabbitMQConsumer.Length}");
        var methods_RabbitMQConsumer = type_RabbitMQConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RabbitMQConsumer 公开方法数量: {methods_RabbitMQConsumer.Length}");
        foreach (var m in methods_RabbitMQConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RabbitMQConsumer 未找到，尝试无命名空间...");
        type_RabbitMQConsumer = Type.GetType("RabbitMQConsumer");
        if (type_RabbitMQConsumer != null)
            Console.WriteLine("[PASS] 类型 RabbitMQConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RabbitMQConsumer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
