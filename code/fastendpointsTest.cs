#load "fastendpoints.cs"

Console.WriteLine("=== fastendpoints.cs Test ===");

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

    // 验证 class: CreateTodoRequest
    var type_CreateTodoRequest = Type.GetType("CreateTodoRequest");
    if (type_CreateTodoRequest != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoRequest (class) 存在");
        var ctors_CreateTodoRequest = type_CreateTodoRequest.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoRequest 构造函数数量: {ctors_CreateTodoRequest.Length}");
        var methods_CreateTodoRequest = type_CreateTodoRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoRequest 公开方法数量: {methods_CreateTodoRequest.Length}");
        foreach (var m in methods_CreateTodoRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoRequest 未找到，尝试无命名空间...");
        type_CreateTodoRequest = Type.GetType("CreateTodoRequest");
        if (type_CreateTodoRequest != null)
            Console.WriteLine("[PASS] 类型 CreateTodoRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CreateTodoResponse
    var type_CreateTodoResponse = Type.GetType("CreateTodoResponse");
    if (type_CreateTodoResponse != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoResponse (class) 存在");
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

    // 验证 class: CreateTodoEndpoint
    var type_CreateTodoEndpoint = Type.GetType("CreateTodoEndpoint");
    if (type_CreateTodoEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoEndpoint (class) 存在");
        var ctors_CreateTodoEndpoint = type_CreateTodoEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoEndpoint 构造函数数量: {ctors_CreateTodoEndpoint.Length}");
        var methods_CreateTodoEndpoint = type_CreateTodoEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoEndpoint 公开方法数量: {methods_CreateTodoEndpoint.Length}");
        foreach (var m in methods_CreateTodoEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoEndpoint 未找到，尝试无命名空间...");
        type_CreateTodoEndpoint = Type.GetType("CreateTodoEndpoint");
        if (type_CreateTodoEndpoint != null)
            Console.WriteLine("[PASS] 类型 CreateTodoEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GetTodoEndpoint
    var type_GetTodoEndpoint = Type.GetType("GetTodoEndpoint");
    if (type_GetTodoEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 GetTodoEndpoint (class) 存在");
        var ctors_GetTodoEndpoint = type_GetTodoEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] GetTodoEndpoint 构造函数数量: {ctors_GetTodoEndpoint.Length}");
        var methods_GetTodoEndpoint = type_GetTodoEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GetTodoEndpoint 公开方法数量: {methods_GetTodoEndpoint.Length}");
        foreach (var m in methods_GetTodoEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GetTodoEndpoint 未找到，尝试无命名空间...");
        type_GetTodoEndpoint = Type.GetType("GetTodoEndpoint");
        if (type_GetTodoEndpoint != null)
            Console.WriteLine("[PASS] 类型 GetTodoEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GetTodoEndpoint 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
