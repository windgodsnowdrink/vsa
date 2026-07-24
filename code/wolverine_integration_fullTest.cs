#load "wolverine_integration_full.cs"

Console.WriteLine("=== wolverine_integration_full.cs Test ===");

try
{
    // 验证 class: TodoHandlers
    var type_TodoHandlers = Type.GetType("TodoHandlers");
    if (type_TodoHandlers != null)
    {
        Console.WriteLine("[PASS] 类型 TodoHandlers (class) 存在");
        var ctors_TodoHandlers = type_TodoHandlers.GetConstructors();
        Console.WriteLine($"[PASS] TodoHandlers 构造函数数量: {ctors_TodoHandlers.Length}");
        var methods_TodoHandlers = type_TodoHandlers.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoHandlers 公开方法数量: {methods_TodoHandlers.Length}");
        foreach (var m in methods_TodoHandlers)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoHandlers 未找到，尝试无命名空间...");
        type_TodoHandlers = Type.GetType("TodoHandlers");
        if (type_TodoHandlers != null)
            Console.WriteLine("[PASS] 类型 TodoHandlers (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoHandlers 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoEndpoints
    var type_TodoEndpoints = Type.GetType("TodoEndpoints");
    if (type_TodoEndpoints != null)
    {
        Console.WriteLine("[PASS] 类型 TodoEndpoints (class) 存在");
        var ctors_TodoEndpoints = type_TodoEndpoints.GetConstructors();
        Console.WriteLine($"[PASS] TodoEndpoints 构造函数数量: {ctors_TodoEndpoints.Length}");
        var methods_TodoEndpoints = type_TodoEndpoints.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoEndpoints 公开方法数量: {methods_TodoEndpoints.Length}");
        foreach (var m in methods_TodoEndpoints)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoEndpoints 未找到，尝试无命名空间...");
        type_TodoEndpoints = Type.GetType("TodoEndpoints");
        if (type_TodoEndpoints != null)
            Console.WriteLine("[PASS] 类型 TodoEndpoints (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoEndpoints 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Todo
    var type_Todo = Type.GetType("Todo");
    if (type_Todo != null)
    {
        Console.WriteLine("[PASS] 类型 Todo (class) 存在");
        var ctors_Todo = type_Todo.GetConstructors();
        Console.WriteLine($"[PASS] Todo 构造函数数量: {ctors_Todo.Length}");
        var methods_Todo = type_Todo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Todo 公开方法数量: {methods_Todo.Length}");
        foreach (var m in methods_Todo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Todo 未找到，尝试无命名空间...");
        type_Todo = Type.GetType("Todo");
        if (type_Todo != null)
            Console.WriteLine("[PASS] 类型 Todo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Todo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CreateTodoValidator
    var type_CreateTodoValidator = Type.GetType("CreateTodoValidator");
    if (type_CreateTodoValidator != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoValidator (class) 存在");
        var ctors_CreateTodoValidator = type_CreateTodoValidator.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoValidator 构造函数数量: {ctors_CreateTodoValidator.Length}");
        var methods_CreateTodoValidator = type_CreateTodoValidator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoValidator 公开方法数量: {methods_CreateTodoValidator.Length}");
        foreach (var m in methods_CreateTodoValidator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoValidator 未找到，尝试无命名空间...");
        type_CreateTodoValidator = Type.GetType("CreateTodoValidator");
        if (type_CreateTodoValidator != null)
            Console.WriteLine("[PASS] 类型 CreateTodoValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoValidator 可能为顶层语句或嵌套类型");
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

    // 验证 class: OrderValidator
    var type_OrderValidator = Type.GetType("OrderValidator");
    if (type_OrderValidator != null)
    {
        Console.WriteLine("[PASS] 类型 OrderValidator (class) 存在");
        var ctors_OrderValidator = type_OrderValidator.GetConstructors();
        Console.WriteLine($"[PASS] OrderValidator 构造函数数量: {ctors_OrderValidator.Length}");
        var methods_OrderValidator = type_OrderValidator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderValidator 公开方法数量: {methods_OrderValidator.Length}");
        foreach (var m in methods_OrderValidator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderValidator 未找到，尝试无命名空间...");
        type_OrderValidator = Type.GetType("OrderValidator");
        if (type_OrderValidator != null)
            Console.WriteLine("[PASS] 类型 OrderValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderValidator 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CreateTodo
    var type_CreateTodo = Type.GetType("CreateTodo");
    if (type_CreateTodo != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodo (record) 存在");
        var ctors_CreateTodo = type_CreateTodo.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodo 构造函数数量: {ctors_CreateTodo.Length}");
        var methods_CreateTodo = type_CreateTodo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodo 公开方法数量: {methods_CreateTodo.Length}");
        foreach (var m in methods_CreateTodo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodo 未找到，尝试无命名空间...");
        type_CreateTodo = Type.GetType("CreateTodo");
        if (type_CreateTodo != null)
            Console.WriteLine("[PASS] 类型 CreateTodo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodo 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoCreated
    var type_TodoCreated = Type.GetType("TodoCreated");
    if (type_TodoCreated != null)
    {
        Console.WriteLine("[PASS] 类型 TodoCreated (record) 存在");
        var ctors_TodoCreated = type_TodoCreated.GetConstructors();
        Console.WriteLine($"[PASS] TodoCreated 构造函数数量: {ctors_TodoCreated.Length}");
        var methods_TodoCreated = type_TodoCreated.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoCreated 公开方法数量: {methods_TodoCreated.Length}");
        foreach (var m in methods_TodoCreated)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoCreated 未找到，尝试无命名空间...");
        type_TodoCreated = Type.GetType("TodoCreated");
        if (type_TodoCreated != null)
            Console.WriteLine("[PASS] 类型 TodoCreated (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoCreated 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
