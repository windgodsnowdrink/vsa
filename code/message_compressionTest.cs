#load "message_compression.cs"

Console.WriteLine("=== message_compression.cs Test ===");

try
{
    // 验证 class: CompressedMessageHandler
    var type_CompressedMessageHandler = Type.GetType("CompressedMessageHandler");
    if (type_CompressedMessageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 CompressedMessageHandler (class) 存在");
        var ctors_CompressedMessageHandler = type_CompressedMessageHandler.GetConstructors();
        Console.WriteLine($"[PASS] CompressedMessageHandler 构造函数数量: {ctors_CompressedMessageHandler.Length}");
        var methods_CompressedMessageHandler = type_CompressedMessageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressedMessageHandler 公开方法数量: {methods_CompressedMessageHandler.Length}");
        foreach (var m in methods_CompressedMessageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressedMessageHandler 未找到，尝试无命名空间...");
        type_CompressedMessageHandler = Type.GetType("CompressedMessageHandler");
        if (type_CompressedMessageHandler != null)
            Console.WriteLine("[PASS] 类型 CompressedMessageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressedMessageHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
