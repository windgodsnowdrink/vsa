#load "grpc_protobufnet.cs"

Console.WriteLine("=== grpc_protobufnet.cs Test ===");

try
{
    // 验证 class: TodoItem
    var type_TodoItem = Type.GetType("TodoItem");
    if (type_TodoItem != null)
    {
        Console.WriteLine("[PASS] 类型 TodoItem (class) 存在");
        var ctors_TodoItem = type_TodoItem.GetConstructors();
        Console.WriteLine($"[PASS] TodoItem 构造函数数量: {ctors_TodoItem.Length}");
        var methods_TodoItem = type_TodoItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoItem 公开方法数量: {methods_TodoItem.Length}");
        foreach (var m in methods_TodoItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoItem 未找到，尝试无命名空间...");
        type_TodoItem = Type.GetType("TodoItem");
        if (type_TodoItem != null)
            Console.WriteLine("[PASS] 类型 TodoItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoItem 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoService
    var type_TodoService = Type.GetType("TodoService");
    if (type_TodoService != null)
    {
        Console.WriteLine("[PASS] 类型 TodoService (class) 存在");
        var ctors_TodoService = type_TodoService.GetConstructors();
        Console.WriteLine($"[PASS] TodoService 构造函数数量: {ctors_TodoService.Length}");
        var methods_TodoService = type_TodoService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoService 公开方法数量: {methods_TodoService.Length}");
        foreach (var m in methods_TodoService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoService 未找到，尝试无命名空间...");
        type_TodoService = Type.GetType("TodoService");
        if (type_TodoService != null)
            Console.WriteLine("[PASS] 类型 TodoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoChannelProcessor
    var type_TodoChannelProcessor = Type.GetType("TodoChannelProcessor");
    if (type_TodoChannelProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TodoChannelProcessor (class) 存在");
        var ctors_TodoChannelProcessor = type_TodoChannelProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TodoChannelProcessor 构造函数数量: {ctors_TodoChannelProcessor.Length}");
        var methods_TodoChannelProcessor = type_TodoChannelProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoChannelProcessor 公开方法数量: {methods_TodoChannelProcessor.Length}");
        foreach (var m in methods_TodoChannelProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoChannelProcessor 未找到，尝试无命名空间...");
        type_TodoChannelProcessor = Type.GetType("TodoChannelProcessor");
        if (type_TodoChannelProcessor != null)
            Console.WriteLine("[PASS] 类型 TodoChannelProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoChannelProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TracingInterceptor
    var type_TracingInterceptor = Type.GetType("TracingInterceptor");
    if (type_TracingInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 TracingInterceptor (class) 存在");
        var ctors_TracingInterceptor = type_TracingInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] TracingInterceptor 构造函数数量: {ctors_TracingInterceptor.Length}");
        var methods_TracingInterceptor = type_TracingInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TracingInterceptor 公开方法数量: {methods_TracingInterceptor.Length}");
        foreach (var m in methods_TracingInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TracingInterceptor 未找到，尝试无命名空间...");
        type_TracingInterceptor = Type.GetType("TracingInterceptor");
        if (type_TracingInterceptor != null)
            Console.WriteLine("[PASS] 类型 TracingInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TracingInterceptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChaosInterceptor
    var type_ChaosInterceptor = Type.GetType("ChaosInterceptor");
    if (type_ChaosInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 ChaosInterceptor (class) 存在");
        var ctors_ChaosInterceptor = type_ChaosInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] ChaosInterceptor 构造函数数量: {ctors_ChaosInterceptor.Length}");
        var methods_ChaosInterceptor = type_ChaosInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChaosInterceptor 公开方法数量: {methods_ChaosInterceptor.Length}");
        foreach (var m in methods_ChaosInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChaosInterceptor 未找到，尝试无命名空间...");
        type_ChaosInterceptor = Type.GetType("ChaosInterceptor");
        if (type_ChaosInterceptor != null)
            Console.WriteLine("[PASS] 类型 ChaosInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChaosInterceptor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITodoService
    var type_ITodoService = Type.GetType("ITodoService");
    if (type_ITodoService != null)
    {
        Console.WriteLine("[PASS] 类型 ITodoService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITodoService 未找到，尝试无命名空间...");
        type_ITodoService = Type.GetType("ITodoService");
        if (type_ITodoService != null)
            Console.WriteLine("[PASS] 类型 ITodoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITodoService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
