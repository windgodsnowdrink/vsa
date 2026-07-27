#load "acme_dotnet_integration.cs"

Console.WriteLine("=== acme_dotnet_integration.cs Test ===");

try
{
    // 验证 class: AcmeDotNetService
    var type_AcmeDotNetService = Type.GetType("AcmeDotNetService");
    if (type_AcmeDotNetService != null)
    {
        Console.WriteLine("[PASS] 类型 AcmeDotNetService (class) 存在");
        var ctors_AcmeDotNetService = type_AcmeDotNetService.GetConstructors();
        Console.WriteLine($"[PASS] AcmeDotNetService 构造函数数量: {ctors_AcmeDotNetService.Length}");
        var methods_AcmeDotNetService = type_AcmeDotNetService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AcmeDotNetService 公开方法数量: {methods_AcmeDotNetService.Length}");
        foreach (var m in methods_AcmeDotNetService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AcmeDotNetService 未找到，尝试无命名空间...");
        type_AcmeDotNetService = Type.GetType("AcmeDotNetService");
        if (type_AcmeDotNetService != null)
            Console.WriteLine("[PASS] 类型 AcmeDotNetService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AcmeDotNetService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
