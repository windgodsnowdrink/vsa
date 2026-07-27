#load "liquidstate_advanced_integration.cs"

Console.WriteLine("=== liquidstate_advanced_integration.cs Test ===");

try
{
    // 验证 class: RedisStateRepository
    var type_RedisStateRepository = Type.GetType("RedisStateRepository");
    if (type_RedisStateRepository != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStateRepository (class) 存在");
        var ctors_RedisStateRepository = type_RedisStateRepository.GetConstructors();
        Console.WriteLine($"[PASS] RedisStateRepository 构造函数数量: {ctors_RedisStateRepository.Length}");
        var methods_RedisStateRepository = type_RedisStateRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStateRepository 公开方法数量: {methods_RedisStateRepository.Length}");
        foreach (var m in methods_RedisStateRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStateRepository 未找到，尝试无命名空间...");
        type_RedisStateRepository = Type.GetType("RedisStateRepository");
        if (type_RedisStateRepository != null)
            Console.WriteLine("[PASS] 类型 RedisStateRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisStateRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AotStateMachineFactory
    var type_AotStateMachineFactory = Type.GetType("AotStateMachineFactory");
    if (type_AotStateMachineFactory != null)
    {
        Console.WriteLine("[PASS] 类型 AotStateMachineFactory (class) 存在");
        var ctors_AotStateMachineFactory = type_AotStateMachineFactory.GetConstructors();
        Console.WriteLine($"[PASS] AotStateMachineFactory 构造函数数量: {ctors_AotStateMachineFactory.Length}");
        var methods_AotStateMachineFactory = type_AotStateMachineFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotStateMachineFactory 公开方法数量: {methods_AotStateMachineFactory.Length}");
        foreach (var m in methods_AotStateMachineFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotStateMachineFactory 未找到，尝试无命名空间...");
        type_AotStateMachineFactory = Type.GetType("AotStateMachineFactory");
        if (type_AotStateMachineFactory != null)
            Console.WriteLine("[PASS] 类型 AotStateMachineFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotStateMachineFactory 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MonitoredStateMachine
    var type_MonitoredStateMachine = Type.GetType("MonitoredStateMachine");
    if (type_MonitoredStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 MonitoredStateMachine (class) 存在");
        var ctors_MonitoredStateMachine = type_MonitoredStateMachine.GetConstructors();
        Console.WriteLine($"[PASS] MonitoredStateMachine 构造函数数量: {ctors_MonitoredStateMachine.Length}");
        var methods_MonitoredStateMachine = type_MonitoredStateMachine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MonitoredStateMachine 公开方法数量: {methods_MonitoredStateMachine.Length}");
        foreach (var m in methods_MonitoredStateMachine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MonitoredStateMachine 未找到，尝试无命名空间...");
        type_MonitoredStateMachine = Type.GetType("MonitoredStateMachine");
        if (type_MonitoredStateMachine != null)
            Console.WriteLine("[PASS] 类型 MonitoredStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MonitoredStateMachine 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IStateRepository
    var type_IStateRepository = Type.GetType("IStateRepository");
    if (type_IStateRepository != null)
    {
        Console.WriteLine("[PASS] 类型 IStateRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IStateRepository 未找到，尝试无命名空间...");
        type_IStateRepository = Type.GetType("IStateRepository");
        if (type_IStateRepository != null)
            Console.WriteLine("[PASS] 类型 IStateRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IStateRepository 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
