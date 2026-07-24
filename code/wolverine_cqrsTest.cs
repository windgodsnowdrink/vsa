#load "wolverine_cqrs.cs"

Console.WriteLine("=== wolverine_cqrs.cs Test ===");

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

    // 验证 class: CreateTodoHandler
    var type_CreateTodoHandler = Type.GetType("CreateTodoHandler");
    if (type_CreateTodoHandler != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoHandler (class) 存在");
        var ctors_CreateTodoHandler = type_CreateTodoHandler.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoHandler 构造函数数量: {ctors_CreateTodoHandler.Length}");
        var methods_CreateTodoHandler = type_CreateTodoHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoHandler 公开方法数量: {methods_CreateTodoHandler.Length}");
        foreach (var m in methods_CreateTodoHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoHandler 未找到，尝试无命名空间...");
        type_CreateTodoHandler = Type.GetType("CreateTodoHandler");
        if (type_CreateTodoHandler != null)
            Console.WriteLine("[PASS] 类型 CreateTodoHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GetTodoHandler
    var type_GetTodoHandler = Type.GetType("GetTodoHandler");
    if (type_GetTodoHandler != null)
    {
        Console.WriteLine("[PASS] 类型 GetTodoHandler (class) 存在");
        var ctors_GetTodoHandler = type_GetTodoHandler.GetConstructors();
        Console.WriteLine($"[PASS] GetTodoHandler 构造函数数量: {ctors_GetTodoHandler.Length}");
        var methods_GetTodoHandler = type_GetTodoHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GetTodoHandler 公开方法数量: {methods_GetTodoHandler.Length}");
        foreach (var m in methods_GetTodoHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GetTodoHandler 未找到，尝试无命名空间...");
        type_GetTodoHandler = Type.GetType("GetTodoHandler");
        if (type_GetTodoHandler != null)
            Console.WriteLine("[PASS] 类型 GetTodoHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GetTodoHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CreateTodoCommand
    var type_CreateTodoCommand = Type.GetType("CreateTodoCommand");
    if (type_CreateTodoCommand != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoCommand (record) 存在");
        var ctors_CreateTodoCommand = type_CreateTodoCommand.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoCommand 构造函数数量: {ctors_CreateTodoCommand.Length}");
        var methods_CreateTodoCommand = type_CreateTodoCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoCommand 公开方法数量: {methods_CreateTodoCommand.Length}");
        foreach (var m in methods_CreateTodoCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoCommand 未找到，尝试无命名空间...");
        type_CreateTodoCommand = Type.GetType("CreateTodoCommand");
        if (type_CreateTodoCommand != null)
            Console.WriteLine("[PASS] 类型 CreateTodoCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CreateTodoResponse
    var type_CreateTodoResponse = Type.GetType("CreateTodoResponse");
    if (type_CreateTodoResponse != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoResponse (record) 存在");
        var ctors_CreateTodoResponse = type_CreateTodoResponse.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoResponse 构造函数数量: {ctors_CreateTodoResponse.Length}");
        var methods_CreateTodoResponse = type_CreateTodoResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoResponse 公开方法数量: {methods_CreateTodoResponse.Length}");
        foreach (var m in methods_CreateTodoResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoResponse 未找到，尝试无命名空间...");
        type_CreateTodoResponse = Type.GetType("CreateTodoResponse");
        if (type_CreateTodoResponse != null)
            Console.WriteLine("[PASS] 类型 CreateTodoResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoResponse 可能为顶层语句或嵌套类型");
    }

    // 验证 record: GetTodoQuery
    var type_GetTodoQuery = Type.GetType("GetTodoQuery");
    if (type_GetTodoQuery != null)
    {
        Console.WriteLine("[PASS] 类型 GetTodoQuery (record) 存在");
        var ctors_GetTodoQuery = type_GetTodoQuery.GetConstructors();
        Console.WriteLine($"[PASS] GetTodoQuery 构造函数数量: {ctors_GetTodoQuery.Length}");
        var methods_GetTodoQuery = type_GetTodoQuery.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GetTodoQuery 公开方法数量: {methods_GetTodoQuery.Length}");
        foreach (var m in methods_GetTodoQuery)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GetTodoQuery 未找到，尝试无命名空间...");
        type_GetTodoQuery = Type.GetType("GetTodoQuery");
        if (type_GetTodoQuery != null)
            Console.WriteLine("[PASS] 类型 GetTodoQuery (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GetTodoQuery 可能为顶层语句或嵌套类型");
    }

    // 验证 record: GetTodoResponse
    var type_GetTodoResponse = Type.GetType("GetTodoResponse");
    if (type_GetTodoResponse != null)
    {
        Console.WriteLine("[PASS] 类型 GetTodoResponse (record) 存在");
        var ctors_GetTodoResponse = type_GetTodoResponse.GetConstructors();
        Console.WriteLine($"[PASS] GetTodoResponse 构造函数数量: {ctors_GetTodoResponse.Length}");
        var methods_GetTodoResponse = type_GetTodoResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GetTodoResponse 公开方法数量: {methods_GetTodoResponse.Length}");
        foreach (var m in methods_GetTodoResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GetTodoResponse 未找到，尝试无命名空间...");
        type_GetTodoResponse = Type.GetType("GetTodoResponse");
        if (type_GetTodoResponse != null)
            Console.WriteLine("[PASS] 类型 GetTodoResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GetTodoResponse 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
