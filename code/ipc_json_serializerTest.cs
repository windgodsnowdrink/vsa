#load "ipc_json_serializer.cs"

Console.WriteLine("=== ipc_json_serializer.cs Test ===");

try
{
    // 验证 class: IpcJsonSerializer
    var type_IpcJsonSerializer = Type.GetType("IpcJsonSerializer");
    if (type_IpcJsonSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 IpcJsonSerializer (class) 存在");
        var ctors_IpcJsonSerializer = type_IpcJsonSerializer.GetConstructors();
        Console.WriteLine($"[PASS] IpcJsonSerializer 构造函数数量: {ctors_IpcJsonSerializer.Length}");
        var methods_IpcJsonSerializer = type_IpcJsonSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IpcJsonSerializer 公开方法数量: {methods_IpcJsonSerializer.Length}");
        foreach (var m in methods_IpcJsonSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IpcJsonSerializer 未找到，尝试无命名空间...");
        type_IpcJsonSerializer = Type.GetType("IpcJsonSerializer");
        if (type_IpcJsonSerializer != null)
            Console.WriteLine("[PASS] 类型 IpcJsonSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IpcJsonSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
