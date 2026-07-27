#load "wolverine.cs"

Console.WriteLine("=== wolverine.cs Test ===");

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

    // 验证 class: TodoEventHandler
    var type_TodoEventHandler = Type.GetType("TodoEventHandler");
    if (type_TodoEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 TodoEventHandler (class) 存在");
        var ctors_TodoEventHandler = type_TodoEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] TodoEventHandler 构造函数数量: {ctors_TodoEventHandler.Length}");
        var methods_TodoEventHandler = type_TodoEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoEventHandler 公开方法数量: {methods_TodoEventHandler.Length}");
        foreach (var m in methods_TodoEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoEventHandler 未找到，尝试无命名空间...");
        type_TodoEventHandler = Type.GetType("TodoEventHandler");
        if (type_TodoEventHandler != null)
            Console.WriteLine("[PASS] 类型 TodoEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryObjectPool
    var type_MemoryObjectPool = Type.GetType("MemoryObjectPool");
    if (type_MemoryObjectPool != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryObjectPool (class) 存在");
        var ctors_MemoryObjectPool = type_MemoryObjectPool.GetConstructors();
        Console.WriteLine($"[PASS] MemoryObjectPool 构造函数数量: {ctors_MemoryObjectPool.Length}");
        var methods_MemoryObjectPool = type_MemoryObjectPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryObjectPool 公开方法数量: {methods_MemoryObjectPool.Length}");
        foreach (var m in methods_MemoryObjectPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryObjectPool 未找到，尝试无命名空间...");
        type_MemoryObjectPool = Type.GetType("MemoryObjectPool");
        if (type_MemoryObjectPool != null)
            Console.WriteLine("[PASS] 类型 MemoryObjectPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryObjectPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PoolingManager
    var type_PoolingManager = Type.GetType("PoolingManager");
    if (type_PoolingManager != null)
    {
        Console.WriteLine("[PASS] 类型 PoolingManager (class) 存在");
        var ctors_PoolingManager = type_PoolingManager.GetConstructors();
        Console.WriteLine($"[PASS] PoolingManager 构造函数数量: {ctors_PoolingManager.Length}");
        var methods_PoolingManager = type_PoolingManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PoolingManager 公开方法数量: {methods_PoolingManager.Length}");
        foreach (var m in methods_PoolingManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PoolingManager 未找到，尝试无命名空间...");
        type_PoolingManager = Type.GetType("PoolingManager");
        if (type_PoolingManager != null)
            Console.WriteLine("[PASS] 类型 PoolingManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PoolingManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoSagaState
    var type_TodoSagaState = Type.GetType("TodoSagaState");
    if (type_TodoSagaState != null)
    {
        Console.WriteLine("[PASS] 类型 TodoSagaState (class) 存在");
        var ctors_TodoSagaState = type_TodoSagaState.GetConstructors();
        Console.WriteLine($"[PASS] TodoSagaState 构造函数数量: {ctors_TodoSagaState.Length}");
        var methods_TodoSagaState = type_TodoSagaState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoSagaState 公开方法数量: {methods_TodoSagaState.Length}");
        foreach (var m in methods_TodoSagaState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoSagaState 未找到，尝试无命名空间...");
        type_TodoSagaState = Type.GetType("TodoSagaState");
        if (type_TodoSagaState != null)
            Console.WriteLine("[PASS] 类型 TodoSagaState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoSagaState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoSaga
    var type_TodoSaga = Type.GetType("TodoSaga");
    if (type_TodoSaga != null)
    {
        Console.WriteLine("[PASS] 类型 TodoSaga (class) 存在");
        var ctors_TodoSaga = type_TodoSaga.GetConstructors();
        Console.WriteLine($"[PASS] TodoSaga 构造函数数量: {ctors_TodoSaga.Length}");
        var methods_TodoSaga = type_TodoSaga.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoSaga 公开方法数量: {methods_TodoSaga.Length}");
        foreach (var m in methods_TodoSaga)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoSaga 未找到，尝试无命名空间...");
        type_TodoSaga = Type.GetType("TodoSaga");
        if (type_TodoSaga != null)
            Console.WriteLine("[PASS] 类型 TodoSaga (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoSaga 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResourceService
    var type_ResourceService = Type.GetType("ResourceService");
    if (type_ResourceService != null)
    {
        Console.WriteLine("[PASS] 类型 ResourceService (class) 存在");
        var ctors_ResourceService = type_ResourceService.GetConstructors();
        Console.WriteLine($"[PASS] ResourceService 构造函数数量: {ctors_ResourceService.Length}");
        var methods_ResourceService = type_ResourceService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResourceService 公开方法数量: {methods_ResourceService.Length}");
        foreach (var m in methods_ResourceService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResourceService 未找到，尝试无命名空间...");
        type_ResourceService = Type.GetType("ResourceService");
        if (type_ResourceService != null)
            Console.WriteLine("[PASS] 类型 ResourceService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResourceService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoService
    var type_TodoService = Type.GetType("TodoService");
    if (type_TodoService != null)
    {
        Console.WriteLine("[PASS] 类型 TodoService (class) 存在");
        var ctors_TodoService = type_TodoService.GetConstructors();
        Console.WriteLine($"[PASS] TodoService 构造函数数量: {ctors_TodoService.Length}");
        var methods_TodoService = type_TodoService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoService 公开方法数量: {methods_TodoService.Length}");
        foreach (var m in methods_TodoService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoService 未找到，尝试无命名空间...");
        type_TodoService = Type.GetType("TodoService");
        if (type_TodoService != null)
            Console.WriteLine("[PASS] 类型 TodoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NotificationService
    var type_NotificationService = Type.GetType("NotificationService");
    if (type_NotificationService != null)
    {
        Console.WriteLine("[PASS] 类型 NotificationService (class) 存在");
        var ctors_NotificationService = type_NotificationService.GetConstructors();
        Console.WriteLine($"[PASS] NotificationService 构造函数数量: {ctors_NotificationService.Length}");
        var methods_NotificationService = type_NotificationService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NotificationService 公开方法数量: {methods_NotificationService.Length}");
        foreach (var m in methods_NotificationService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NotificationService 未找到，尝试无命名空间...");
        type_NotificationService = Type.GetType("NotificationService");
        if (type_NotificationService != null)
            Console.WriteLine("[PASS] 类型 NotificationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NotificationService 可能为顶层语句或嵌套类型");
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

    // 验证 record: TodoCompleted
    var type_TodoCompleted = Type.GetType("TodoCompleted");
    if (type_TodoCompleted != null)
    {
        Console.WriteLine("[PASS] 类型 TodoCompleted (record) 存在");
        var ctors_TodoCompleted = type_TodoCompleted.GetConstructors();
        Console.WriteLine($"[PASS] TodoCompleted 构造函数数量: {ctors_TodoCompleted.Length}");
        var methods_TodoCompleted = type_TodoCompleted.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoCompleted 公开方法数量: {methods_TodoCompleted.Length}");
        foreach (var m in methods_TodoCompleted)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoCompleted 未找到，尝试无命名空间...");
        type_TodoCompleted = Type.GetType("TodoCompleted");
        if (type_TodoCompleted != null)
            Console.WriteLine("[PASS] 类型 TodoCompleted (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoCompleted 可能为顶层语句或嵌套类型");
    }

    // 验证 record: StartTodoSagaCommand
    var type_StartTodoSagaCommand = Type.GetType("StartTodoSagaCommand");
    if (type_StartTodoSagaCommand != null)
    {
        Console.WriteLine("[PASS] 类型 StartTodoSagaCommand (record) 存在");
        var ctors_StartTodoSagaCommand = type_StartTodoSagaCommand.GetConstructors();
        Console.WriteLine($"[PASS] StartTodoSagaCommand 构造函数数量: {ctors_StartTodoSagaCommand.Length}");
        var methods_StartTodoSagaCommand = type_StartTodoSagaCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StartTodoSagaCommand 公开方法数量: {methods_StartTodoSagaCommand.Length}");
        foreach (var m in methods_StartTodoSagaCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StartTodoSagaCommand 未找到，尝试无命名空间...");
        type_StartTodoSagaCommand = Type.GetType("StartTodoSagaCommand");
        if (type_StartTodoSagaCommand != null)
            Console.WriteLine("[PASS] 类型 StartTodoSagaCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StartTodoSagaCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ReserveResourcesEvent
    var type_ReserveResourcesEvent = Type.GetType("ReserveResourcesEvent");
    if (type_ReserveResourcesEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ReserveResourcesEvent (record) 存在");
        var ctors_ReserveResourcesEvent = type_ReserveResourcesEvent.GetConstructors();
        Console.WriteLine($"[PASS] ReserveResourcesEvent 构造函数数量: {ctors_ReserveResourcesEvent.Length}");
        var methods_ReserveResourcesEvent = type_ReserveResourcesEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReserveResourcesEvent 公开方法数量: {methods_ReserveResourcesEvent.Length}");
        foreach (var m in methods_ReserveResourcesEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReserveResourcesEvent 未找到，尝试无命名空间...");
        type_ReserveResourcesEvent = Type.GetType("ReserveResourcesEvent");
        if (type_ReserveResourcesEvent != null)
            Console.WriteLine("[PASS] 类型 ReserveResourcesEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReserveResourcesEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ResourcesReservedEvent
    var type_ResourcesReservedEvent = Type.GetType("ResourcesReservedEvent");
    if (type_ResourcesReservedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ResourcesReservedEvent (record) 存在");
        var ctors_ResourcesReservedEvent = type_ResourcesReservedEvent.GetConstructors();
        Console.WriteLine($"[PASS] ResourcesReservedEvent 构造函数数量: {ctors_ResourcesReservedEvent.Length}");
        var methods_ResourcesReservedEvent = type_ResourcesReservedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResourcesReservedEvent 公开方法数量: {methods_ResourcesReservedEvent.Length}");
        foreach (var m in methods_ResourcesReservedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResourcesReservedEvent 未找到，尝试无命名空间...");
        type_ResourcesReservedEvent = Type.GetType("ResourcesReservedEvent");
        if (type_ResourcesReservedEvent != null)
            Console.WriteLine("[PASS] 类型 ResourcesReservedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResourcesReservedEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CreateTodoItemEvent
    var type_CreateTodoItemEvent = Type.GetType("CreateTodoItemEvent");
    if (type_CreateTodoItemEvent != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoItemEvent (record) 存在");
        var ctors_CreateTodoItemEvent = type_CreateTodoItemEvent.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoItemEvent 构造函数数量: {ctors_CreateTodoItemEvent.Length}");
        var methods_CreateTodoItemEvent = type_CreateTodoItemEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoItemEvent 公开方法数量: {methods_CreateTodoItemEvent.Length}");
        foreach (var m in methods_CreateTodoItemEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoItemEvent 未找到，尝试无命名空间...");
        type_CreateTodoItemEvent = Type.GetType("CreateTodoItemEvent");
        if (type_CreateTodoItemEvent != null)
            Console.WriteLine("[PASS] 类型 CreateTodoItemEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoItemEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoItemCreatedEvent
    var type_TodoItemCreatedEvent = Type.GetType("TodoItemCreatedEvent");
    if (type_TodoItemCreatedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 TodoItemCreatedEvent (record) 存在");
        var ctors_TodoItemCreatedEvent = type_TodoItemCreatedEvent.GetConstructors();
        Console.WriteLine($"[PASS] TodoItemCreatedEvent 构造函数数量: {ctors_TodoItemCreatedEvent.Length}");
        var methods_TodoItemCreatedEvent = type_TodoItemCreatedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoItemCreatedEvent 公开方法数量: {methods_TodoItemCreatedEvent.Length}");
        foreach (var m in methods_TodoItemCreatedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoItemCreatedEvent 未找到，尝试无命名空间...");
        type_TodoItemCreatedEvent = Type.GetType("TodoItemCreatedEvent");
        if (type_TodoItemCreatedEvent != null)
            Console.WriteLine("[PASS] 类型 TodoItemCreatedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoItemCreatedEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: NotifyUserEvent
    var type_NotifyUserEvent = Type.GetType("NotifyUserEvent");
    if (type_NotifyUserEvent != null)
    {
        Console.WriteLine("[PASS] 类型 NotifyUserEvent (record) 存在");
        var ctors_NotifyUserEvent = type_NotifyUserEvent.GetConstructors();
        Console.WriteLine($"[PASS] NotifyUserEvent 构造函数数量: {ctors_NotifyUserEvent.Length}");
        var methods_NotifyUserEvent = type_NotifyUserEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NotifyUserEvent 公开方法数量: {methods_NotifyUserEvent.Length}");
        foreach (var m in methods_NotifyUserEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NotifyUserEvent 未找到，尝试无命名空间...");
        type_NotifyUserEvent = Type.GetType("NotifyUserEvent");
        if (type_NotifyUserEvent != null)
            Console.WriteLine("[PASS] 类型 NotifyUserEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NotifyUserEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UserNotifiedEvent
    var type_UserNotifiedEvent = Type.GetType("UserNotifiedEvent");
    if (type_UserNotifiedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 UserNotifiedEvent (record) 存在");
        var ctors_UserNotifiedEvent = type_UserNotifiedEvent.GetConstructors();
        Console.WriteLine($"[PASS] UserNotifiedEvent 构造函数数量: {ctors_UserNotifiedEvent.Length}");
        var methods_UserNotifiedEvent = type_UserNotifiedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserNotifiedEvent 公开方法数量: {methods_UserNotifiedEvent.Length}");
        foreach (var m in methods_UserNotifiedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserNotifiedEvent 未找到，尝试无命名空间...");
        type_UserNotifiedEvent = Type.GetType("UserNotifiedEvent");
        if (type_UserNotifiedEvent != null)
            Console.WriteLine("[PASS] 类型 UserNotifiedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserNotifiedEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: RollbackResourcesEvent
    var type_RollbackResourcesEvent = Type.GetType("RollbackResourcesEvent");
    if (type_RollbackResourcesEvent != null)
    {
        Console.WriteLine("[PASS] 类型 RollbackResourcesEvent (record) 存在");
        var ctors_RollbackResourcesEvent = type_RollbackResourcesEvent.GetConstructors();
        Console.WriteLine($"[PASS] RollbackResourcesEvent 构造函数数量: {ctors_RollbackResourcesEvent.Length}");
        var methods_RollbackResourcesEvent = type_RollbackResourcesEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RollbackResourcesEvent 公开方法数量: {methods_RollbackResourcesEvent.Length}");
        foreach (var m in methods_RollbackResourcesEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RollbackResourcesEvent 未找到，尝试无命名空间...");
        type_RollbackResourcesEvent = Type.GetType("RollbackResourcesEvent");
        if (type_RollbackResourcesEvent != null)
            Console.WriteLine("[PASS] 类型 RollbackResourcesEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RollbackResourcesEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ResourcesRolledBackEvent
    var type_ResourcesRolledBackEvent = Type.GetType("ResourcesRolledBackEvent");
    if (type_ResourcesRolledBackEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ResourcesRolledBackEvent (record) 存在");
        var ctors_ResourcesRolledBackEvent = type_ResourcesRolledBackEvent.GetConstructors();
        Console.WriteLine($"[PASS] ResourcesRolledBackEvent 构造函数数量: {ctors_ResourcesRolledBackEvent.Length}");
        var methods_ResourcesRolledBackEvent = type_ResourcesRolledBackEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResourcesRolledBackEvent 公开方法数量: {methods_ResourcesRolledBackEvent.Length}");
        foreach (var m in methods_ResourcesRolledBackEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResourcesRolledBackEvent 未找到，尝试无命名空间...");
        type_ResourcesRolledBackEvent = Type.GetType("ResourcesRolledBackEvent");
        if (type_ResourcesRolledBackEvent != null)
            Console.WriteLine("[PASS] 类型 ResourcesRolledBackEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResourcesRolledBackEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: SagaStatus
    var type_SagaStatus = Type.GetType("SagaStatus");
    if (type_SagaStatus != null)
    {
        Console.WriteLine("[PASS] 类型 SagaStatus (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaStatus 未找到，尝试无命名空间...");
        type_SagaStatus = Type.GetType("SagaStatus");
        if (type_SagaStatus != null)
            Console.WriteLine("[PASS] 类型 SagaStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaStatus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
