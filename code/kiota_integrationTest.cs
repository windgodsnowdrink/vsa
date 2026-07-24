#load "kiota_integration.cs"

Console.WriteLine("=== kiota_integration.cs Test ===");

try
{
    // 验证 class: ChannelKiotaCodeGenerator
    var type_ChannelKiotaCodeGenerator = Type.GetType("ChannelKiotaCodeGenerator");
    if (type_ChannelKiotaCodeGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelKiotaCodeGenerator (class) 存在");
        var ctors_ChannelKiotaCodeGenerator = type_ChannelKiotaCodeGenerator.GetConstructors();
        Console.WriteLine($"[PASS] ChannelKiotaCodeGenerator 构造函数数量: {ctors_ChannelKiotaCodeGenerator.Length}");
        var methods_ChannelKiotaCodeGenerator = type_ChannelKiotaCodeGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelKiotaCodeGenerator 公开方法数量: {methods_ChannelKiotaCodeGenerator.Length}");
        foreach (var m in methods_ChannelKiotaCodeGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelKiotaCodeGenerator 未找到，尝试无命名空间...");
        type_ChannelKiotaCodeGenerator = Type.GetType("ChannelKiotaCodeGenerator");
        if (type_ChannelKiotaCodeGenerator != null)
            Console.WriteLine("[PASS] 类型 ChannelKiotaCodeGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelKiotaCodeGenerator 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
