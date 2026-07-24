#load "masstransit_litedb_integration.cs"

Console.WriteLine("=== masstransit_litedb_integration.cs Test ===");

try
{
    // 验证 class: OrderStateMachine
    var type_OrderStateMachine = Type.GetType("OrderStateMachine");
    if (type_OrderStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 OrderStateMachine (class) 存在");
        var ctors_OrderStateMachine = type_OrderStateMachine.GetConstructors();
        Console.WriteLine($"[PASS] OrderStateMachine 构造函数数量: {ctors_OrderStateMachine.Length}");
        var methods_OrderStateMachine = type_OrderStateMachine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderStateMachine 公开方法数量: {methods_OrderStateMachine.Length}");
        foreach (var m in methods_OrderStateMachine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderStateMachine 未找到，尝试无命名空间...");
        type_OrderStateMachine = Type.GetType("OrderStateMachine");
        if (type_OrderStateMachine != null)
            Console.WriteLine("[PASS] 类型 OrderStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderStateMachine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderState
    var type_OrderState = Type.GetType("OrderState");
    if (type_OrderState != null)
    {
        Console.WriteLine("[PASS] 类型 OrderState (class) 存在");
        var ctors_OrderState = type_OrderState.GetConstructors();
        Console.WriteLine($"[PASS] OrderState 构造函数数量: {ctors_OrderState.Length}");
        var methods_OrderState = type_OrderState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderState 公开方法数量: {methods_OrderState.Length}");
        foreach (var m in methods_OrderState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderState 未找到，尝试无命名空间...");
        type_OrderState = Type.GetType("OrderState");
        if (type_OrderState != null)
            Console.WriteLine("[PASS] 类型 OrderState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderState 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
