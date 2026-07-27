#load "minio_caller.cs"

Console.WriteLine("=== minio_caller.cs Test ===");

try
{
    // 验证 class: DataProcessedHandler
    var type_DataProcessedHandler = Type.GetType("DataProcessedHandler");
    if (type_DataProcessedHandler != null)
    {
        Console.WriteLine("[PASS] 类型 DataProcessedHandler (class) 存在");
        var ctors_DataProcessedHandler = type_DataProcessedHandler.GetConstructors();
        Console.WriteLine($"[PASS] DataProcessedHandler 构造函数数量: {ctors_DataProcessedHandler.Length}");
        var methods_DataProcessedHandler = type_DataProcessedHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataProcessedHandler 公开方法数量: {methods_DataProcessedHandler.Length}");
        foreach (var m in methods_DataProcessedHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataProcessedHandler 未找到，尝试无命名空间...");
        type_DataProcessedHandler = Type.GetType("DataProcessedHandler");
        if (type_DataProcessedHandler != null)
            Console.WriteLine("[PASS] 类型 DataProcessedHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataProcessedHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISharedService
    var type_ISharedService = Type.GetType("ISharedService");
    if (type_ISharedService != null)
    {
        Console.WriteLine("[PASS] 类型 ISharedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISharedService 未找到，尝试无命名空间...");
        type_ISharedService = Type.GetType("ISharedService");
        if (type_ISharedService != null)
            Console.WriteLine("[PASS] 类型 ISharedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISharedService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ProcessCommand
    var type_ProcessCommand = Type.GetType("ProcessCommand");
    if (type_ProcessCommand != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessCommand (record) 存在");
        var ctors_ProcessCommand = type_ProcessCommand.GetConstructors();
        Console.WriteLine($"[PASS] ProcessCommand 构造函数数量: {ctors_ProcessCommand.Length}");
        var methods_ProcessCommand = type_ProcessCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessCommand 公开方法数量: {methods_ProcessCommand.Length}");
        foreach (var m in methods_ProcessCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessCommand 未找到，尝试无命名空间...");
        type_ProcessCommand = Type.GetType("ProcessCommand");
        if (type_ProcessCommand != null)
            Console.WriteLine("[PASS] 类型 ProcessCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessCommand 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
