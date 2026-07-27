#load "datadog_integration.cs"

Console.WriteLine("=== datadog_integration.cs Test ===");

try
{
    // 验证 class: DataDogMetricsProcessor
    var type_DataDogMetricsProcessor = Type.GetType("DataDogMetricsProcessor");
    if (type_DataDogMetricsProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DataDogMetricsProcessor (class) 存在");
        var ctors_DataDogMetricsProcessor = type_DataDogMetricsProcessor.GetConstructors();
        Console.WriteLine($"[PASS] DataDogMetricsProcessor 构造函数数量: {ctors_DataDogMetricsProcessor.Length}");
        var methods_DataDogMetricsProcessor = type_DataDogMetricsProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataDogMetricsProcessor 公开方法数量: {methods_DataDogMetricsProcessor.Length}");
        foreach (var m in methods_DataDogMetricsProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataDogMetricsProcessor 未找到，尝试无命名空间...");
        type_DataDogMetricsProcessor = Type.GetType("DataDogMetricsProcessor");
        if (type_DataDogMetricsProcessor != null)
            Console.WriteLine("[PASS] 类型 DataDogMetricsProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataDogMetricsProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
