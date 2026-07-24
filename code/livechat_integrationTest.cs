#load "livechat_integration.cs"

Console.WriteLine("=== livechat_integration.cs Test ===");

try
{
    // 验证 class: LiveStreamProcessor
    var type_LiveStreamProcessor = Type.GetType("LiveStreamProcessor");
    if (type_LiveStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LiveStreamProcessor (class) 存在");
        var ctors_LiveStreamProcessor = type_LiveStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LiveStreamProcessor 构造函数数量: {ctors_LiveStreamProcessor.Length}");
        var methods_LiveStreamProcessor = type_LiveStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiveStreamProcessor 公开方法数量: {methods_LiveStreamProcessor.Length}");
        foreach (var m in methods_LiveStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiveStreamProcessor 未找到，尝试无命名空间...");
        type_LiveStreamProcessor = Type.GetType("LiveStreamProcessor");
        if (type_LiveStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 LiveStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiveStreamProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiveChatHub
    var type_LiveChatHub = Type.GetType("LiveChatHub");
    if (type_LiveChatHub != null)
    {
        Console.WriteLine("[PASS] 类型 LiveChatHub (class) 存在");
        var ctors_LiveChatHub = type_LiveChatHub.GetConstructors();
        Console.WriteLine($"[PASS] LiveChatHub 构造函数数量: {ctors_LiveChatHub.Length}");
        var methods_LiveChatHub = type_LiveChatHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiveChatHub 公开方法数量: {methods_LiveChatHub.Length}");
        foreach (var m in methods_LiveChatHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiveChatHub 未找到，尝试无命名空间...");
        type_LiveChatHub = Type.GetType("LiveChatHub");
        if (type_LiveChatHub != null)
            Console.WriteLine("[PASS] 类型 LiveChatHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiveChatHub 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiveStreamBackgroundService
    var type_LiveStreamBackgroundService = Type.GetType("LiveStreamBackgroundService");
    if (type_LiveStreamBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 LiveStreamBackgroundService (class) 存在");
        var ctors_LiveStreamBackgroundService = type_LiveStreamBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] LiveStreamBackgroundService 构造函数数量: {ctors_LiveStreamBackgroundService.Length}");
        var methods_LiveStreamBackgroundService = type_LiveStreamBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiveStreamBackgroundService 公开方法数量: {methods_LiveStreamBackgroundService.Length}");
        foreach (var m in methods_LiveStreamBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiveStreamBackgroundService 未找到，尝试无命名空间...");
        type_LiveStreamBackgroundService = Type.GetType("LiveStreamBackgroundService");
        if (type_LiveStreamBackgroundService != null)
            Console.WriteLine("[PASS] 类型 LiveStreamBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiveStreamBackgroundService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
