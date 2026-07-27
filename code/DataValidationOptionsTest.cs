#load "DataValidationOptions.cs"

Console.WriteLine("=== DataValidationOptions.cs Test ===");

try
{
    // 验证 class: ChoETL.Integration.DataValidationOptions
    var type_DataValidationOptions = Type.GetType("ChoETL.Integration.DataValidationOptions");
    if (type_DataValidationOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETL.Integration.DataValidationOptions (class) 存在");
        var ctors_DataValidationOptions = type_DataValidationOptions.GetConstructors();
        Console.WriteLine($"[PASS] ChoETL.Integration.DataValidationOptions 构造函数数量: {ctors_DataValidationOptions.Length}");
        var methods_DataValidationOptions = type_DataValidationOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETL.Integration.DataValidationOptions 公开方法数量: {methods_DataValidationOptions.Length}");
        foreach (var m in methods_DataValidationOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETL.Integration.DataValidationOptions 未找到，尝试无命名空间...");
        type_DataValidationOptions = Type.GetType("DataValidationOptions");
        if (type_DataValidationOptions != null)
            Console.WriteLine("[PASS] 类型 DataValidationOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataValidationOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
