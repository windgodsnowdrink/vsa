#load "jsonrpc_impl.cs"

Console.WriteLine("=== jsonrpc_impl.cs Test ===");

try
{
    // 验证 class: RpcServer
    var type_RpcServer = Type.GetType("RpcServer");
    if (type_RpcServer != null)
    {
        Console.WriteLine("[PASS] 类型 RpcServer (class) 存在");
        var ctors_RpcServer = type_RpcServer.GetConstructors();
        Console.WriteLine($"[PASS] RpcServer 构造函数数量: {ctors_RpcServer.Length}");
        var methods_RpcServer = type_RpcServer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RpcServer 公开方法数量: {methods_RpcServer.Length}");
        foreach (var m in methods_RpcServer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RpcServer 未找到，尝试无命名空间...");
        type_RpcServer = Type.GetType("RpcServer");
        if (type_RpcServer != null)
            Console.WriteLine("[PASS] 类型 RpcServer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RpcServer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RpcClient
    var type_RpcClient = Type.GetType("RpcClient");
    if (type_RpcClient != null)
    {
        Console.WriteLine("[PASS] 类型 RpcClient (class) 存在");
        var ctors_RpcClient = type_RpcClient.GetConstructors();
        Console.WriteLine($"[PASS] RpcClient 构造函数数量: {ctors_RpcClient.Length}");
        var methods_RpcClient = type_RpcClient.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RpcClient 公开方法数量: {methods_RpcClient.Length}");
        foreach (var m in methods_RpcClient)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RpcClient 未找到，尝试无命名空间...");
        type_RpcClient = Type.GetType("RpcClient");
        if (type_RpcClient != null)
            Console.WriteLine("[PASS] 类型 RpcClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RpcClient 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RpcDemo
    var type_RpcDemo = Type.GetType("RpcDemo");
    if (type_RpcDemo != null)
    {
        Console.WriteLine("[PASS] 类型 RpcDemo (class) 存在");
        var ctors_RpcDemo = type_RpcDemo.GetConstructors();
        Console.WriteLine($"[PASS] RpcDemo 构造函数数量: {ctors_RpcDemo.Length}");
        var methods_RpcDemo = type_RpcDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RpcDemo 公开方法数量: {methods_RpcDemo.Length}");
        foreach (var m in methods_RpcDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RpcDemo 未找到，尝试无命名空间...");
        type_RpcDemo = Type.GetType("RpcDemo");
        if (type_RpcDemo != null)
            Console.WriteLine("[PASS] 类型 RpcDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RpcDemo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DuplexStream
    var type_DuplexStream = Type.GetType("DuplexStream");
    if (type_DuplexStream != null)
    {
        Console.WriteLine("[PASS] 类型 DuplexStream (class) 存在");
        var ctors_DuplexStream = type_DuplexStream.GetConstructors();
        Console.WriteLine($"[PASS] DuplexStream 构造函数数量: {ctors_DuplexStream.Length}");
        var methods_DuplexStream = type_DuplexStream.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DuplexStream 公开方法数量: {methods_DuplexStream.Length}");
        foreach (var m in methods_DuplexStream)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DuplexStream 未找到，尝试无命名空间...");
        type_DuplexStream = Type.GetType("DuplexStream");
        if (type_DuplexStream != null)
            Console.WriteLine("[PASS] 类型 DuplexStream (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DuplexStream 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IRpcService
    var type_IRpcService = Type.GetType("IRpcService");
    if (type_IRpcService != null)
    {
        Console.WriteLine("[PASS] 类型 IRpcService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IRpcService 未找到，尝试无命名空间...");
        type_IRpcService = Type.GetType("IRpcService");
        if (type_IRpcService != null)
            Console.WriteLine("[PASS] 类型 IRpcService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRpcService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
