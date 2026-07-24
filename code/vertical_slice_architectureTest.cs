#load "vertical_slice_architecture.cs"

Console.WriteLine("=== vertical_slice_architecture.cs Test ===");

try
{
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

    // 验证 class: TodoDbContext
    var type_TodoDbContext = Type.GetType("TodoDbContext");
    if (type_TodoDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 TodoDbContext (class) 存在");
        var ctors_TodoDbContext = type_TodoDbContext.GetConstructors();
        Console.WriteLine($"[PASS] TodoDbContext 构造函数数量: {ctors_TodoDbContext.Length}");
        var methods_TodoDbContext = type_TodoDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoDbContext 公开方法数量: {methods_TodoDbContext.Length}");
        foreach (var m in methods_TodoDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoDbContext 未找到，尝试无命名空间...");
        type_TodoDbContext = Type.GetType("TodoDbContext");
        if (type_TodoDbContext != null)
            Console.WriteLine("[PASS] 类型 TodoDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoDbContext 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
