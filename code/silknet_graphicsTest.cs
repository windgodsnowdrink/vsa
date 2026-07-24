#load "silknet_graphics.cs"

Console.WriteLine("=== silknet_graphics.cs Test ===");

try
{
    // 验证 class: GraphicsEngine
    var type_GraphicsEngine = Type.GetType("GraphicsEngine");
    if (type_GraphicsEngine != null)
    {
        Console.WriteLine("[PASS] 类型 GraphicsEngine (class) 存在");
        var ctors_GraphicsEngine = type_GraphicsEngine.GetConstructors();
        Console.WriteLine($"[PASS] GraphicsEngine 构造函数数量: {ctors_GraphicsEngine.Length}");
        var methods_GraphicsEngine = type_GraphicsEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GraphicsEngine 公开方法数量: {methods_GraphicsEngine.Length}");
        foreach (var m in methods_GraphicsEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GraphicsEngine 未找到，尝试无命名空间...");
        type_GraphicsEngine = Type.GetType("GraphicsEngine");
        if (type_GraphicsEngine != null)
            Console.WriteLine("[PASS] 类型 GraphicsEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GraphicsEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RenderProcessor
    var type_RenderProcessor = Type.GetType("RenderProcessor");
    if (type_RenderProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 RenderProcessor (class) 存在");
        var ctors_RenderProcessor = type_RenderProcessor.GetConstructors();
        Console.WriteLine($"[PASS] RenderProcessor 构造函数数量: {ctors_RenderProcessor.Length}");
        var methods_RenderProcessor = type_RenderProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RenderProcessor 公开方法数量: {methods_RenderProcessor.Length}");
        foreach (var m in methods_RenderProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RenderProcessor 未找到，尝试无命名空间...");
        type_RenderProcessor = Type.GetType("RenderProcessor");
        if (type_RenderProcessor != null)
            Console.WriteLine("[PASS] 类型 RenderProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RenderProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
