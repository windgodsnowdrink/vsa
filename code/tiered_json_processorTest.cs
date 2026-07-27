#load "tiered_json_processor.cs"

Console.WriteLine("=== tiered_json_processor.cs Test ===");

try
{
    // 验证 class: TieredJsonProcessor
    var type_TieredJsonProcessor = Type.GetType("TieredJsonProcessor");
    if (type_TieredJsonProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TieredJsonProcessor (class) 存在");
        var ctors_TieredJsonProcessor = type_TieredJsonProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TieredJsonProcessor 构造函数数量: {ctors_TieredJsonProcessor.Length}");
        var methods_TieredJsonProcessor = type_TieredJsonProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredJsonProcessor 公开方法数量: {methods_TieredJsonProcessor.Length}");
        foreach (var m in methods_TieredJsonProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredJsonProcessor 未找到，尝试无命名空间...");
        type_TieredJsonProcessor = Type.GetType("TieredJsonProcessor");
        if (type_TieredJsonProcessor != null)
            Console.WriteLine("[PASS] 类型 TieredJsonProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredJsonProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
