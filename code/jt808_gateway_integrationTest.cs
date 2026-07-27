#load "jt808_gateway_integration.cs"

Console.WriteLine("=== jt808_gateway_integration.cs Test ===");

try
{
    // 验证 class: JT808GatewayService
    var type_JT808GatewayService = Type.GetType("JT808GatewayService");
    if (type_JT808GatewayService != null)
    {
        Console.WriteLine("[PASS] 类型 JT808GatewayService (class) 存在");
        var ctors_JT808GatewayService = type_JT808GatewayService.GetConstructors();
        Console.WriteLine($"[PASS] JT808GatewayService 构造函数数量: {ctors_JT808GatewayService.Length}");
        var methods_JT808GatewayService = type_JT808GatewayService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JT808GatewayService 公开方法数量: {methods_JT808GatewayService.Length}");
        foreach (var m in methods_JT808GatewayService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JT808GatewayService 未找到，尝试无命名空间...");
        type_JT808GatewayService = Type.GetType("JT808GatewayService");
        if (type_JT808GatewayService != null)
            Console.WriteLine("[PASS] 类型 JT808GatewayService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JT808GatewayService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IJT808GatewayService
    var type_IJT808GatewayService = Type.GetType("IJT808GatewayService");
    if (type_IJT808GatewayService != null)
    {
        Console.WriteLine("[PASS] 类型 IJT808GatewayService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IJT808GatewayService 未找到，尝试无命名空间...");
        type_IJT808GatewayService = Type.GetType("IJT808GatewayService");
        if (type_IJT808GatewayService != null)
            Console.WriteLine("[PASS] 类型 IJT808GatewayService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IJT808GatewayService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
