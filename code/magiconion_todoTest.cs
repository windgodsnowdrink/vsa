#load "magiconion_todo.cs"

Console.WriteLine("=== magiconion_todo.cs Test ===");

try
{
    // 验证 class: ChannelTodoProcessor
    var type_ChannelTodoProcessor = Type.GetType("ChannelTodoProcessor");
    if (type_ChannelTodoProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelTodoProcessor (class) 存在");
        var ctors_ChannelTodoProcessor = type_ChannelTodoProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelTodoProcessor 构造函数数量: {ctors_ChannelTodoProcessor.Length}");
        var methods_ChannelTodoProcessor = type_ChannelTodoProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelTodoProcessor 公开方法数量: {methods_ChannelTodoProcessor.Length}");
        foreach (var m in methods_ChannelTodoProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelTodoProcessor 未找到，尝试无命名空间...");
        type_ChannelTodoProcessor = Type.GetType("ChannelTodoProcessor");
        if (type_ChannelTodoProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelTodoProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelTodoProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MetricsSnapshot
    var type_MetricsSnapshot = Type.GetType("MetricsSnapshot");
    if (type_MetricsSnapshot != null)
    {
        Console.WriteLine("[PASS] 类型 MetricsSnapshot (class) 存在");
        var ctors_MetricsSnapshot = type_MetricsSnapshot.GetConstructors();
        Console.WriteLine($"[PASS] MetricsSnapshot 构造函数数量: {ctors_MetricsSnapshot.Length}");
        var methods_MetricsSnapshot = type_MetricsSnapshot.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MetricsSnapshot 公开方法数量: {methods_MetricsSnapshot.Length}");
        foreach (var m in methods_MetricsSnapshot)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MetricsSnapshot 未找到，尝试无命名空间...");
        type_MetricsSnapshot = Type.GetType("MetricsSnapshot");
        if (type_MetricsSnapshot != null)
            Console.WriteLine("[PASS] 类型 MetricsSnapshot (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MetricsSnapshot 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoItem
    var type_TodoItem = Type.GetType("TodoItem");
    if (type_TodoItem != null)
    {
        Console.WriteLine("[PASS] 类型 TodoItem (class) 存在");
        var ctors_TodoItem = type_TodoItem.GetConstructors();
        Console.WriteLine($"[PASS] TodoItem 构造函数数量: {ctors_TodoItem.Length}");
        var methods_TodoItem = type_TodoItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoItem 公开方法数量: {methods_TodoItem.Length}");
        foreach (var m in methods_TodoItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoItem 未找到，尝试无命名空间...");
        type_TodoItem = Type.GetType("TodoItem");
        if (type_TodoItem != null)
            Console.WriteLine("[PASS] 类型 TodoItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoItem 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITodoService
    var type_ITodoService = Type.GetType("ITodoService");
    if (type_ITodoService != null)
    {
        Console.WriteLine("[PASS] 类型 ITodoService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITodoService 未找到，尝试无命名空间...");
        type_ITodoService = Type.GetType("ITodoService");
        if (type_ITodoService != null)
            Console.WriteLine("[PASS] 类型 ITodoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITodoService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
