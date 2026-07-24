#load "bouncycastle_advanced.cs"

Console.WriteLine("=== bouncycastle_advanced.cs Test ===");

try
{
    // 验证 class: BouncyCastleAdvancedService
    var type_BouncyCastleAdvancedService = Type.GetType("BouncyCastleAdvancedService");
    if (type_BouncyCastleAdvancedService != null)
    {
        Console.WriteLine("[PASS] 类型 BouncyCastleAdvancedService (class) 存在");
        var ctors_BouncyCastleAdvancedService = type_BouncyCastleAdvancedService.GetConstructors();
        Console.WriteLine($"[PASS] BouncyCastleAdvancedService 构造函数数量: {ctors_BouncyCastleAdvancedService.Length}");
        var methods_BouncyCastleAdvancedService = type_BouncyCastleAdvancedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BouncyCastleAdvancedService 公开方法数量: {methods_BouncyCastleAdvancedService.Length}");
        foreach (var m in methods_BouncyCastleAdvancedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BouncyCastleAdvancedService 未找到，尝试无命名空间...");
        type_BouncyCastleAdvancedService = Type.GetType("BouncyCastleAdvancedService");
        if (type_BouncyCastleAdvancedService != null)
            Console.WriteLine("[PASS] 类型 BouncyCastleAdvancedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BouncyCastleAdvancedService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
