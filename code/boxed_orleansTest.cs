#load "boxed_orleans.cs"

Console.WriteLine("=== boxed_orleans.cs Test ===");

try
{
    // 验证 class: TodoGrain
    var type_TodoGrain = Type.GetType("TodoGrain");
    if (type_TodoGrain != null)
    {
        Console.WriteLine("[PASS] 类型 TodoGrain (class) 存在");
        var ctors_TodoGrain = type_TodoGrain.GetConstructors();
        Console.WriteLine($"[PASS] TodoGrain 构造函数数量: {ctors_TodoGrain.Length}");
        var methods_TodoGrain = type_TodoGrain.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoGrain 公开方法数量: {methods_TodoGrain.Length}");
        foreach (var m in methods_TodoGrain)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoGrain 未找到，尝试无命名空间...");
        type_TodoGrain = Type.GetType("TodoGrain");
        if (type_TodoGrain != null)
            Console.WriteLine("[PASS] 类型 TodoGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrleansMessage
    var type_OrleansMessage = Type.GetType("OrleansMessage");
    if (type_OrleansMessage != null)
    {
        Console.WriteLine("[PASS] 类型 OrleansMessage (class) 存在");
        var ctors_OrleansMessage = type_OrleansMessage.GetConstructors();
        Console.WriteLine($"[PASS] OrleansMessage 构造函数数量: {ctors_OrleansMessage.Length}");
        var methods_OrleansMessage = type_OrleansMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrleansMessage 公开方法数量: {methods_OrleansMessage.Length}");
        foreach (var m in methods_OrleansMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrleansMessage 未找到，尝试无命名空间...");
        type_OrleansMessage = Type.GetType("OrleansMessage");
        if (type_OrleansMessage != null)
            Console.WriteLine("[PASS] 类型 OrleansMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrleansMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITodoGrain
    var type_ITodoGrain = Type.GetType("ITodoGrain");
    if (type_ITodoGrain != null)
    {
        Console.WriteLine("[PASS] 类型 ITodoGrain (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITodoGrain 未找到，尝试无命名空间...");
        type_ITodoGrain = Type.GetType("ITodoGrain");
        if (type_ITodoGrain != null)
            Console.WriteLine("[PASS] 类型 ITodoGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITodoGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoCommand
    var type_TodoCommand = Type.GetType("TodoCommand");
    if (type_TodoCommand != null)
    {
        Console.WriteLine("[PASS] 类型 TodoCommand (record) 存在");
        var ctors_TodoCommand = type_TodoCommand.GetConstructors();
        Console.WriteLine($"[PASS] TodoCommand 构造函数数量: {ctors_TodoCommand.Length}");
        var methods_TodoCommand = type_TodoCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoCommand 公开方法数量: {methods_TodoCommand.Length}");
        foreach (var m in methods_TodoCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoCommand 未找到，尝试无命名空间...");
        type_TodoCommand = Type.GetType("TodoCommand");
        if (type_TodoCommand != null)
            Console.WriteLine("[PASS] 类型 TodoCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoCommand 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
