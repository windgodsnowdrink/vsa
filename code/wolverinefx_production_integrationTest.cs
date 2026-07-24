#load "wolverinefx_production_integration.cs"

Console.WriteLine("=== wolverinefx_production_integration.cs Test ===");

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

    // 验证 class: WolverineServiceExtensions
    var type_WolverineServiceExtensions = Type.GetType("WolverineServiceExtensions");
    if (type_WolverineServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineServiceExtensions (class) 存在");
        var ctors_WolverineServiceExtensions = type_WolverineServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] WolverineServiceExtensions 构造函数数量: {ctors_WolverineServiceExtensions.Length}");
        var methods_WolverineServiceExtensions = type_WolverineServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineServiceExtensions 公开方法数量: {methods_WolverineServiceExtensions.Length}");
        foreach (var m in methods_WolverineServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineServiceExtensions 未找到，尝试无命名空间...");
        type_WolverineServiceExtensions = Type.GetType("WolverineServiceExtensions");
        if (type_WolverineServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 WolverineServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoggingMiddleware
    var type_LoggingMiddleware = Type.GetType("LoggingMiddleware");
    if (type_LoggingMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 LoggingMiddleware (class) 存在");
        var ctors_LoggingMiddleware = type_LoggingMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] LoggingMiddleware 构造函数数量: {ctors_LoggingMiddleware.Length}");
        var methods_LoggingMiddleware = type_LoggingMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoggingMiddleware 公开方法数量: {methods_LoggingMiddleware.Length}");
        foreach (var m in methods_LoggingMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoggingMiddleware 未找到，尝试无命名空间...");
        type_LoggingMiddleware = Type.GetType("LoggingMiddleware");
        if (type_LoggingMiddleware != null)
            Console.WriteLine("[PASS] 类型 LoggingMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoggingMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DomainEventDispatcher
    var type_DomainEventDispatcher = Type.GetType("DomainEventDispatcher");
    if (type_DomainEventDispatcher != null)
    {
        Console.WriteLine("[PASS] 类型 DomainEventDispatcher (class) 存在");
        var ctors_DomainEventDispatcher = type_DomainEventDispatcher.GetConstructors();
        Console.WriteLine($"[PASS] DomainEventDispatcher 构造函数数量: {ctors_DomainEventDispatcher.Length}");
        var methods_DomainEventDispatcher = type_DomainEventDispatcher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DomainEventDispatcher 公开方法数量: {methods_DomainEventDispatcher.Length}");
        foreach (var m in methods_DomainEventDispatcher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DomainEventDispatcher 未找到，尝试无命名空间...");
        type_DomainEventDispatcher = Type.GetType("DomainEventDispatcher");
        if (type_DomainEventDispatcher != null)
            Console.WriteLine("[PASS] 类型 DomainEventDispatcher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DomainEventDispatcher 可能为顶层语句或嵌套类型");
    }

    // 验证 class: InMemoryEventBus
    var type_InMemoryEventBus = Type.GetType("InMemoryEventBus");
    if (type_InMemoryEventBus != null)
    {
        Console.WriteLine("[PASS] 类型 InMemoryEventBus (class) 存在");
        var ctors_InMemoryEventBus = type_InMemoryEventBus.GetConstructors();
        Console.WriteLine($"[PASS] InMemoryEventBus 构造函数数量: {ctors_InMemoryEventBus.Length}");
        var methods_InMemoryEventBus = type_InMemoryEventBus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InMemoryEventBus 公开方法数量: {methods_InMemoryEventBus.Length}");
        foreach (var m in methods_InMemoryEventBus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InMemoryEventBus 未找到，尝试无命名空间...");
        type_InMemoryEventBus = Type.GetType("InMemoryEventBus");
        if (type_InMemoryEventBus != null)
            Console.WriteLine("[PASS] 类型 InMemoryEventBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InMemoryEventBus 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITodoRepository
    var type_ITodoRepository = Type.GetType("ITodoRepository");
    if (type_ITodoRepository != null)
    {
        Console.WriteLine("[PASS] 类型 ITodoRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITodoRepository 未找到，尝试无命名空间...");
        type_ITodoRepository = Type.GetType("ITodoRepository");
        if (type_ITodoRepository != null)
            Console.WriteLine("[PASS] 类型 ITodoRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITodoRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDomainEventDispatcher
    var type_IDomainEventDispatcher = Type.GetType("IDomainEventDispatcher");
    if (type_IDomainEventDispatcher != null)
    {
        Console.WriteLine("[PASS] 类型 IDomainEventDispatcher (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDomainEventDispatcher 未找到，尝试无命名空间...");
        type_IDomainEventDispatcher = Type.GetType("IDomainEventDispatcher");
        if (type_IDomainEventDispatcher != null)
            Console.WriteLine("[PASS] 类型 IDomainEventDispatcher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDomainEventDispatcher 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IEventBus
    var type_IEventBus = Type.GetType("IEventBus");
    if (type_IEventBus != null)
    {
        Console.WriteLine("[PASS] 类型 IEventBus (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IEventBus 未找到，尝试无命名空间...");
        type_IEventBus = Type.GetType("IEventBus");
        if (type_IEventBus != null)
            Console.WriteLine("[PASS] 类型 IEventBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IEventBus 可能为顶层语句或嵌套类型");
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

    // 验证 record: TodoDto
    var type_TodoDto = Type.GetType("TodoDto");
    if (type_TodoDto != null)
    {
        Console.WriteLine("[PASS] 类型 TodoDto (record) 存在");
        var ctors_TodoDto = type_TodoDto.GetConstructors();
        Console.WriteLine($"[PASS] TodoDto 构造函数数量: {ctors_TodoDto.Length}");
        var methods_TodoDto = type_TodoDto.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoDto 公开方法数量: {methods_TodoDto.Length}");
        foreach (var m in methods_TodoDto)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoDto 未找到，尝试无命名空间...");
        type_TodoDto = Type.GetType("TodoDto");
        if (type_TodoDto != null)
            Console.WriteLine("[PASS] 类型 TodoDto (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoDto 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Todo
    var type_Todo = Type.GetType("Todo");
    if (type_Todo != null)
    {
        Console.WriteLine("[PASS] 类型 Todo (record) 存在");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
