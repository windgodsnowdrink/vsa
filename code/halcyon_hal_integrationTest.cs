#load "halcyon_hal_integration.cs"

Console.WriteLine("=== halcyon_hal_integration.cs Test ===");

try
{
    // 验证 class: ProductResource
    var type_ProductResource = Type.GetType("ProductResource");
    if (type_ProductResource != null)
    {
        Console.WriteLine("[PASS] 类型 ProductResource (class) 存在");
        var ctors_ProductResource = type_ProductResource.GetConstructors();
        Console.WriteLine($"[PASS] ProductResource 构造函数数量: {ctors_ProductResource.Length}");
        var methods_ProductResource = type_ProductResource.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductResource 公开方法数量: {methods_ProductResource.Length}");
        foreach (var m in methods_ProductResource)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductResource 未找到，尝试无命名空间...");
        type_ProductResource = Type.GetType("ProductResource");
        if (type_ProductResource != null)
            Console.WriteLine("[PASS] 类型 ProductResource (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductResource 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
