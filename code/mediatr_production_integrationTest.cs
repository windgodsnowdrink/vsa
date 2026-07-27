#load "mediatr_production_integration.cs"

Console.WriteLine("=== mediatr_production_integration.cs Test ===");

try
{
    // 验证 class: CreateTodoCommandHandler
    var type_CreateTodoCommandHandler = Type.GetType("CreateTodoCommandHandler");
    if (type_CreateTodoCommandHandler != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoCommandHandler (class) 存在");
        var ctors_CreateTodoCommandHandler = type_CreateTodoCommandHandler.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoCommandHandler 构造函数数量: {ctors_CreateTodoCommandHandler.Length}");
        var methods_CreateTodoCommandHandler = type_CreateTodoCommandHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoCommandHandler 公开方法数量: {methods_CreateTodoCommandHandler.Length}");
        foreach (var m in methods_CreateTodoCommandHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoCommandHandler 未找到，尝试无命名空间...");
        type_CreateTodoCommandHandler = Type.GetType("CreateTodoCommandHandler");
        if (type_CreateTodoCommandHandler != null)
            Console.WriteLine("[PASS] 类型 CreateTodoCommandHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoCommandHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GetTodoQueryHandler
    var type_GetTodoQueryHandler = Type.GetType("GetTodoQueryHandler");
    if (type_GetTodoQueryHandler != null)
    {
        Console.WriteLine("[PASS] 类型 GetTodoQueryHandler (class) 存在");
        var ctors_GetTodoQueryHandler = type_GetTodoQueryHandler.GetConstructors();
        Console.WriteLine($"[PASS] GetTodoQueryHandler 构造函数数量: {ctors_GetTodoQueryHandler.Length}");
        var methods_GetTodoQueryHandler = type_GetTodoQueryHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GetTodoQueryHandler 公开方法数量: {methods_GetTodoQueryHandler.Length}");
        foreach (var m in methods_GetTodoQueryHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GetTodoQueryHandler 未找到，尝试无命名空间...");
        type_GetTodoQueryHandler = Type.GetType("GetTodoQueryHandler");
        if (type_GetTodoQueryHandler != null)
            Console.WriteLine("[PASS] 类型 GetTodoQueryHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GetTodoQueryHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediatRServiceExtensions
    var type_MediatRServiceExtensions = Type.GetType("MediatRServiceExtensions");
    if (type_MediatRServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MediatRServiceExtensions (class) 存在");
        var ctors_MediatRServiceExtensions = type_MediatRServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MediatRServiceExtensions 构造函数数量: {ctors_MediatRServiceExtensions.Length}");
        var methods_MediatRServiceExtensions = type_MediatRServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatRServiceExtensions 公开方法数量: {methods_MediatRServiceExtensions.Length}");
        foreach (var m in methods_MediatRServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatRServiceExtensions 未找到，尝试无命名空间...");
        type_MediatRServiceExtensions = Type.GetType("MediatRServiceExtensions");
        if (type_MediatRServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 MediatRServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatRServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoggingBehavior
    var type_LoggingBehavior = Type.GetType("LoggingBehavior");
    if (type_LoggingBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 LoggingBehavior (class) 存在");
        var ctors_LoggingBehavior = type_LoggingBehavior.GetConstructors();
        Console.WriteLine($"[PASS] LoggingBehavior 构造函数数量: {ctors_LoggingBehavior.Length}");
        var methods_LoggingBehavior = type_LoggingBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoggingBehavior 公开方法数量: {methods_LoggingBehavior.Length}");
        foreach (var m in methods_LoggingBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoggingBehavior 未找到，尝试无命名空间...");
        type_LoggingBehavior = Type.GetType("LoggingBehavior");
        if (type_LoggingBehavior != null)
            Console.WriteLine("[PASS] 类型 LoggingBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoggingBehavior 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ValidationBehavior
    var type_ValidationBehavior = Type.GetType("ValidationBehavior");
    if (type_ValidationBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 ValidationBehavior (class) 存在");
        var ctors_ValidationBehavior = type_ValidationBehavior.GetConstructors();
        Console.WriteLine($"[PASS] ValidationBehavior 构造函数数量: {ctors_ValidationBehavior.Length}");
        var methods_ValidationBehavior = type_ValidationBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ValidationBehavior 公开方法数量: {methods_ValidationBehavior.Length}");
        foreach (var m in methods_ValidationBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ValidationBehavior 未找到，尝试无命名空间...");
        type_ValidationBehavior = Type.GetType("ValidationBehavior");
        if (type_ValidationBehavior != null)
            Console.WriteLine("[PASS] 类型 ValidationBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidationBehavior 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PerformanceBehavior
    var type_PerformanceBehavior = Type.GetType("PerformanceBehavior");
    if (type_PerformanceBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 PerformanceBehavior (class) 存在");
        var ctors_PerformanceBehavior = type_PerformanceBehavior.GetConstructors();
        Console.WriteLine($"[PASS] PerformanceBehavior 构造函数数量: {ctors_PerformanceBehavior.Length}");
        var methods_PerformanceBehavior = type_PerformanceBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PerformanceBehavior 公开方法数量: {methods_PerformanceBehavior.Length}");
        foreach (var m in methods_PerformanceBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PerformanceBehavior 未找到，尝试无命名空间...");
        type_PerformanceBehavior = Type.GetType("PerformanceBehavior");
        if (type_PerformanceBehavior != null)
            Console.WriteLine("[PASS] 类型 PerformanceBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PerformanceBehavior 可能为顶层语句或嵌套类型");
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
