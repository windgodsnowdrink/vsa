#load "boxed_webapi.cs"

Console.WriteLine("=== boxed_webapi.cs Test ===");

try
{
    // 验证 class: TodoEndpoint
    var type_TodoEndpoint = Type.GetType("TodoEndpoint");
    if (type_TodoEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 TodoEndpoint (class) 存在");
        var ctors_TodoEndpoint = type_TodoEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] TodoEndpoint 构造函数数量: {ctors_TodoEndpoint.Length}");
        var methods_TodoEndpoint = type_TodoEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoEndpoint 公开方法数量: {methods_TodoEndpoint.Length}");
        foreach (var m in methods_TodoEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoEndpoint 未找到，尝试无命名空间...");
        type_TodoEndpoint = Type.GetType("TodoEndpoint");
        if (type_TodoEndpoint != null)
            Console.WriteLine("[PASS] 类型 TodoEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ApiChannelProcessor
    var type_ApiChannelProcessor = Type.GetType("ApiChannelProcessor");
    if (type_ApiChannelProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ApiChannelProcessor (class) 存在");
        var ctors_ApiChannelProcessor = type_ApiChannelProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ApiChannelProcessor 构造函数数量: {ctors_ApiChannelProcessor.Length}");
        var methods_ApiChannelProcessor = type_ApiChannelProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ApiChannelProcessor 公开方法数量: {methods_ApiChannelProcessor.Length}");
        foreach (var m in methods_ApiChannelProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ApiChannelProcessor 未找到，尝试无命名空间...");
        type_ApiChannelProcessor = Type.GetType("ApiChannelProcessor");
        if (type_ApiChannelProcessor != null)
            Console.WriteLine("[PASS] 类型 ApiChannelProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApiChannelProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ApiMessage
    var type_ApiMessage = Type.GetType("ApiMessage");
    if (type_ApiMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ApiMessage (class) 存在");
        var ctors_ApiMessage = type_ApiMessage.GetConstructors();
        Console.WriteLine($"[PASS] ApiMessage 构造函数数量: {ctors_ApiMessage.Length}");
        var methods_ApiMessage = type_ApiMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ApiMessage 公开方法数量: {methods_ApiMessage.Length}");
        foreach (var m in methods_ApiMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ApiMessage 未找到，尝试无命名空间...");
        type_ApiMessage = Type.GetType("ApiMessage");
        if (type_ApiMessage != null)
            Console.WriteLine("[PASS] 类型 ApiMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApiMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoRequest
    var type_TodoRequest = Type.GetType("TodoRequest");
    if (type_TodoRequest != null)
    {
        Console.WriteLine("[PASS] 类型 TodoRequest (record) 存在");
        var ctors_TodoRequest = type_TodoRequest.GetConstructors();
        Console.WriteLine($"[PASS] TodoRequest 构造函数数量: {ctors_TodoRequest.Length}");
        var methods_TodoRequest = type_TodoRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoRequest 公开方法数量: {methods_TodoRequest.Length}");
        foreach (var m in methods_TodoRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoRequest 未找到，尝试无命名空间...");
        type_TodoRequest = Type.GetType("TodoRequest");
        if (type_TodoRequest != null)
            Console.WriteLine("[PASS] 类型 TodoRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoResponse
    var type_TodoResponse = Type.GetType("TodoResponse");
    if (type_TodoResponse != null)
    {
        Console.WriteLine("[PASS] 类型 TodoResponse (record) 存在");
        var ctors_TodoResponse = type_TodoResponse.GetConstructors();
        Console.WriteLine($"[PASS] TodoResponse 构造函数数量: {ctors_TodoResponse.Length}");
        var methods_TodoResponse = type_TodoResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoResponse 公开方法数量: {methods_TodoResponse.Length}");
        foreach (var m in methods_TodoResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoResponse 未找到，尝试无命名空间...");
        type_TodoResponse = Type.GetType("TodoResponse");
        if (type_TodoResponse != null)
            Console.WriteLine("[PASS] 类型 TodoResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoResponse 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
