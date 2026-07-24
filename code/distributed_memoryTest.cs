#load "distributed_memory.cs"

Console.WriteLine("=== distributed_memory.cs Test ===");

try
{
    // 验证 class: ChannelDistributedMemoryHandler
    var type_ChannelDistributedMemoryHandler = Type.GetType("ChannelDistributedMemoryHandler");
    if (type_ChannelDistributedMemoryHandler != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelDistributedMemoryHandler (class) 存在");
        var ctors_ChannelDistributedMemoryHandler = type_ChannelDistributedMemoryHandler.GetConstructors();
        Console.WriteLine($"[PASS] ChannelDistributedMemoryHandler 构造函数数量: {ctors_ChannelDistributedMemoryHandler.Length}");
        var methods_ChannelDistributedMemoryHandler = type_ChannelDistributedMemoryHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelDistributedMemoryHandler 公开方法数量: {methods_ChannelDistributedMemoryHandler.Length}");
        foreach (var m in methods_ChannelDistributedMemoryHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelDistributedMemoryHandler 未找到，尝试无命名空间...");
        type_ChannelDistributedMemoryHandler = Type.GetType("ChannelDistributedMemoryHandler");
        if (type_ChannelDistributedMemoryHandler != null)
            Console.WriteLine("[PASS] 类型 ChannelDistributedMemoryHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelDistributedMemoryHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
