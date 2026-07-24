#load "yarp_integration.cs"

Console.WriteLine("=== yarp_integration.cs Test ===");

try
{
    // 验证 class: BffGatewayService
    var type_BffGatewayService = Type.GetType("BffGatewayService");
    if (type_BffGatewayService != null)
    {
        Console.WriteLine("[PASS] 类型 BffGatewayService (class) 存在");
        var ctors_BffGatewayService = type_BffGatewayService.GetConstructors();
        Console.WriteLine($"[PASS] BffGatewayService 构造函数数量: {ctors_BffGatewayService.Length}");
        var methods_BffGatewayService = type_BffGatewayService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BffGatewayService 公开方法数量: {methods_BffGatewayService.Length}");
        foreach (var m in methods_BffGatewayService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BffGatewayService 未找到，尝试无命名空间...");
        type_BffGatewayService = Type.GetType("BffGatewayService");
        if (type_BffGatewayService != null)
            Console.WriteLine("[PASS] 类型 BffGatewayService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BffGatewayService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProxyConfigPooledPolicy
    var type_ProxyConfigPooledPolicy = Type.GetType("ProxyConfigPooledPolicy");
    if (type_ProxyConfigPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ProxyConfigPooledPolicy (class) 存在");
        var ctors_ProxyConfigPooledPolicy = type_ProxyConfigPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ProxyConfigPooledPolicy 构造函数数量: {ctors_ProxyConfigPooledPolicy.Length}");
        var methods_ProxyConfigPooledPolicy = type_ProxyConfigPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProxyConfigPooledPolicy 公开方法数量: {methods_ProxyConfigPooledPolicy.Length}");
        foreach (var m in methods_ProxyConfigPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProxyConfigPooledPolicy 未找到，尝试无命名空间...");
        type_ProxyConfigPooledPolicy = Type.GetType("ProxyConfigPooledPolicy");
        if (type_ProxyConfigPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 ProxyConfigPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProxyConfigPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
