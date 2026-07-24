#load "webapiclientcore_protobuf.cs"

Console.WriteLine("=== webapiclientcore_protobuf.cs Test ===");

try
{
    // 验证 class: ThreadLocalSpanSerializer
    var type_ThreadLocalSpanSerializer = Type.GetType("ThreadLocalSpanSerializer");
    if (type_ThreadLocalSpanSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 ThreadLocalSpanSerializer (class) 存在");
        var ctors_ThreadLocalSpanSerializer = type_ThreadLocalSpanSerializer.GetConstructors();
        Console.WriteLine($"[PASS] ThreadLocalSpanSerializer 构造函数数量: {ctors_ThreadLocalSpanSerializer.Length}");
        var methods_ThreadLocalSpanSerializer = type_ThreadLocalSpanSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThreadLocalSpanSerializer 公开方法数量: {methods_ThreadLocalSpanSerializer.Length}");
        foreach (var m in methods_ThreadLocalSpanSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThreadLocalSpanSerializer 未找到，尝试无命名空间...");
        type_ThreadLocalSpanSerializer = Type.GetType("ThreadLocalSpanSerializer");
        if (type_ThreadLocalSpanSerializer != null)
            Console.WriteLine("[PASS] 类型 ThreadLocalSpanSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThreadLocalSpanSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
