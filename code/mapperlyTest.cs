#load "mapperly.cs"

Console.WriteLine("=== mapperly.cs Test ===");

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

    // 验证 class: TodoItemDto
    var type_TodoItemDto = Type.GetType("TodoItemDto");
    if (type_TodoItemDto != null)
    {
        Console.WriteLine("[PASS] 类型 TodoItemDto (class) 存在");
        var ctors_TodoItemDto = type_TodoItemDto.GetConstructors();
        Console.WriteLine($"[PASS] TodoItemDto 构造函数数量: {ctors_TodoItemDto.Length}");
        var methods_TodoItemDto = type_TodoItemDto.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoItemDto 公开方法数量: {methods_TodoItemDto.Length}");
        foreach (var m in methods_TodoItemDto)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoItemDto 未找到，尝试无命名空间...");
        type_TodoItemDto = Type.GetType("TodoItemDto");
        if (type_TodoItemDto != null)
            Console.WriteLine("[PASS] 类型 TodoItemDto (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoItemDto 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoMapper
    var type_TodoMapper = Type.GetType("TodoMapper");
    if (type_TodoMapper != null)
    {
        Console.WriteLine("[PASS] 类型 TodoMapper (class) 存在");
        var ctors_TodoMapper = type_TodoMapper.GetConstructors();
        Console.WriteLine($"[PASS] TodoMapper 构造函数数量: {ctors_TodoMapper.Length}");
        var methods_TodoMapper = type_TodoMapper.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoMapper 公开方法数量: {methods_TodoMapper.Length}");
        foreach (var m in methods_TodoMapper)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoMapper 未找到，尝试无命名空间...");
        type_TodoMapper = Type.GetType("TodoMapper");
        if (type_TodoMapper != null)
            Console.WriteLine("[PASS] 类型 TodoMapper (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoMapper 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
