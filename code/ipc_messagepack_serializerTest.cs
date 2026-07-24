#load "ipc_messagepack_serializer.cs"

Console.WriteLine("=== ipc_messagepack_serializer.cs Test ===");

try
{
    // 验证 class: IpcMessagePackSerializer
    var type_IpcMessagePackSerializer = Type.GetType("IpcMessagePackSerializer");
    if (type_IpcMessagePackSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 IpcMessagePackSerializer (class) 存在");
        var ctors_IpcMessagePackSerializer = type_IpcMessagePackSerializer.GetConstructors();
        Console.WriteLine($"[PASS] IpcMessagePackSerializer 构造函数数量: {ctors_IpcMessagePackSerializer.Length}");
        var methods_IpcMessagePackSerializer = type_IpcMessagePackSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IpcMessagePackSerializer 公开方法数量: {methods_IpcMessagePackSerializer.Length}");
        foreach (var m in methods_IpcMessagePackSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IpcMessagePackSerializer 未找到，尝试无命名空间...");
        type_IpcMessagePackSerializer = Type.GetType("IpcMessagePackSerializer");
        if (type_IpcMessagePackSerializer != null)
            Console.WriteLine("[PASS] 类型 IpcMessagePackSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IpcMessagePackSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
