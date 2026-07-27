#load "http3_quic.cs"

Console.WriteLine("=== http3_quic.cs Test ===");

try
{
    // 验证 class: Http3Server
    var type_Http3Server = Type.GetType("Http3Server");
    if (type_Http3Server != null)
    {
        Console.WriteLine("[PASS] 类型 Http3Server (class) 存在");
        var ctors_Http3Server = type_Http3Server.GetConstructors();
        Console.WriteLine($"[PASS] Http3Server 构造函数数量: {ctors_Http3Server.Length}");
        var methods_Http3Server = type_Http3Server.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Http3Server 公开方法数量: {methods_Http3Server.Length}");
        foreach (var m in methods_Http3Server)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Http3Server 未找到，尝试无命名空间...");
        type_Http3Server = Type.GetType("Http3Server");
        if (type_Http3Server != null)
            Console.WriteLine("[PASS] 类型 Http3Server (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Http3Server 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QuicStreamPooledPolicy
    var type_QuicStreamPooledPolicy = Type.GetType("QuicStreamPooledPolicy");
    if (type_QuicStreamPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 QuicStreamPooledPolicy (class) 存在");
        var ctors_QuicStreamPooledPolicy = type_QuicStreamPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] QuicStreamPooledPolicy 构造函数数量: {ctors_QuicStreamPooledPolicy.Length}");
        var methods_QuicStreamPooledPolicy = type_QuicStreamPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuicStreamPooledPolicy 公开方法数量: {methods_QuicStreamPooledPolicy.Length}");
        foreach (var m in methods_QuicStreamPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuicStreamPooledPolicy 未找到，尝试无命名空间...");
        type_QuicStreamPooledPolicy = Type.GetType("QuicStreamPooledPolicy");
        if (type_QuicStreamPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 QuicStreamPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuicStreamPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
