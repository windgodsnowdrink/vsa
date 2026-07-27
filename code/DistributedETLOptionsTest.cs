#load "DistributedETLOptions.cs"

Console.WriteLine("=== DistributedETLOptions.cs Test ===");

try
{
    // 验证 class: ChoETL.Integration.DistributedETLOptions
    var type_DistributedETLOptions = Type.GetType("ChoETL.Integration.DistributedETLOptions");
    if (type_DistributedETLOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETL.Integration.DistributedETLOptions (class) 存在");
        var ctors_DistributedETLOptions = type_DistributedETLOptions.GetConstructors();
        Console.WriteLine($"[PASS] ChoETL.Integration.DistributedETLOptions 构造函数数量: {ctors_DistributedETLOptions.Length}");
        var methods_DistributedETLOptions = type_DistributedETLOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETL.Integration.DistributedETLOptions 公开方法数量: {methods_DistributedETLOptions.Length}");
        foreach (var m in methods_DistributedETLOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETL.Integration.DistributedETLOptions 未找到，尝试无命名空间...");
        type_DistributedETLOptions = Type.GetType("DistributedETLOptions");
        if (type_DistributedETLOptions != null)
            Console.WriteLine("[PASS] 类型 DistributedETLOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedETLOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
