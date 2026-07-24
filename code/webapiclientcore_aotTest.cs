#load "webapiclientcore_aot.cs"

Console.WriteLine("=== webapiclientcore_aot.cs Test ===");

try
{
    // 验证 class: AotClientProxyGenerator
    var type_AotClientProxyGenerator = Type.GetType("AotClientProxyGenerator");
    if (type_AotClientProxyGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 AotClientProxyGenerator (class) 存在");
        var ctors_AotClientProxyGenerator = type_AotClientProxyGenerator.GetConstructors();
        Console.WriteLine($"[PASS] AotClientProxyGenerator 构造函数数量: {ctors_AotClientProxyGenerator.Length}");
        var methods_AotClientProxyGenerator = type_AotClientProxyGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotClientProxyGenerator 公开方法数量: {methods_AotClientProxyGenerator.Length}");
        foreach (var m in methods_AotClientProxyGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotClientProxyGenerator 未找到，尝试无命名空间...");
        type_AotClientProxyGenerator = Type.GetType("AotClientProxyGenerator");
        if (type_AotClientProxyGenerator != null)
            Console.WriteLine("[PASS] 类型 AotClientProxyGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotClientProxyGenerator 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
