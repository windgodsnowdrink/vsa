#load "sshnet_blazor_integration.cs"

Console.WriteLine("=== sshnet_blazor_integration.cs Test ===");

try
{
    // 验证 class: TerminalHub
    var type_TerminalHub = Type.GetType("TerminalHub");
    if (type_TerminalHub != null)
    {
        Console.WriteLine("[PASS] 类型 TerminalHub (class) 存在");
        var ctors_TerminalHub = type_TerminalHub.GetConstructors();
        Console.WriteLine($"[PASS] TerminalHub 构造函数数量: {ctors_TerminalHub.Length}");
        var methods_TerminalHub = type_TerminalHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TerminalHub 公开方法数量: {methods_TerminalHub.Length}");
        foreach (var m in methods_TerminalHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TerminalHub 未找到，尝试无命名空间...");
        type_TerminalHub = Type.GetType("TerminalHub");
        if (type_TerminalHub != null)
            Console.WriteLine("[PASS] 类型 TerminalHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TerminalHub 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TerminalComponent
    var type_TerminalComponent = Type.GetType("TerminalComponent");
    if (type_TerminalComponent != null)
    {
        Console.WriteLine("[PASS] 类型 TerminalComponent (class) 存在");
        var ctors_TerminalComponent = type_TerminalComponent.GetConstructors();
        Console.WriteLine($"[PASS] TerminalComponent 构造函数数量: {ctors_TerminalComponent.Length}");
        var methods_TerminalComponent = type_TerminalComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TerminalComponent 公开方法数量: {methods_TerminalComponent.Length}");
        foreach (var m in methods_TerminalComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TerminalComponent 未找到，尝试无命名空间...");
        type_TerminalComponent = Type.GetType("TerminalComponent");
        if (type_TerminalComponent != null)
            Console.WriteLine("[PASS] 类型 TerminalComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TerminalComponent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
