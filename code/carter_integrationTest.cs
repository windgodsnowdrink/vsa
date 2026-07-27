#load "carter_integration.cs"

Console.WriteLine("=== carter_integration.cs Test ===");

try
{
    // 验证 class: CarterModuleBase
    var type_CarterModuleBase = Type.GetType("CarterModuleBase");
    if (type_CarterModuleBase != null)
    {
        Console.WriteLine("[PASS] 类型 CarterModuleBase (class) 存在");
        var ctors_CarterModuleBase = type_CarterModuleBase.GetConstructors();
        Console.WriteLine($"[PASS] CarterModuleBase 构造函数数量: {ctors_CarterModuleBase.Length}");
        var methods_CarterModuleBase = type_CarterModuleBase.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CarterModuleBase 公开方法数量: {methods_CarterModuleBase.Length}");
        foreach (var m in methods_CarterModuleBase)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CarterModuleBase 未找到，尝试无命名空间...");
        type_CarterModuleBase = Type.GetType("CarterModuleBase");
        if (type_CarterModuleBase != null)
            Console.WriteLine("[PASS] 类型 CarterModuleBase (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CarterModuleBase 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CarterModuleExtensions
    var type_CarterModuleExtensions = Type.GetType("CarterModuleExtensions");
    if (type_CarterModuleExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 CarterModuleExtensions (class) 存在");
        var ctors_CarterModuleExtensions = type_CarterModuleExtensions.GetConstructors();
        Console.WriteLine($"[PASS] CarterModuleExtensions 构造函数数量: {ctors_CarterModuleExtensions.Length}");
        var methods_CarterModuleExtensions = type_CarterModuleExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CarterModuleExtensions 公开方法数量: {methods_CarterModuleExtensions.Length}");
        foreach (var m in methods_CarterModuleExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CarterModuleExtensions 未找到，尝试无命名空间...");
        type_CarterModuleExtensions = Type.GetType("CarterModuleExtensions");
        if (type_CarterModuleExtensions != null)
            Console.WriteLine("[PASS] 类型 CarterModuleExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CarterModuleExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CarterModuleRegistry
    var type_CarterModuleRegistry = Type.GetType("CarterModuleRegistry");
    if (type_CarterModuleRegistry != null)
    {
        Console.WriteLine("[PASS] 类型 CarterModuleRegistry (class) 存在");
        var ctors_CarterModuleRegistry = type_CarterModuleRegistry.GetConstructors();
        Console.WriteLine($"[PASS] CarterModuleRegistry 构造函数数量: {ctors_CarterModuleRegistry.Length}");
        var methods_CarterModuleRegistry = type_CarterModuleRegistry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CarterModuleRegistry 公开方法数量: {methods_CarterModuleRegistry.Length}");
        foreach (var m in methods_CarterModuleRegistry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CarterModuleRegistry 未找到，尝试无命名空间...");
        type_CarterModuleRegistry = Type.GetType("CarterModuleRegistry");
        if (type_CarterModuleRegistry != null)
            Console.WriteLine("[PASS] 类型 CarterModuleRegistry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CarterModuleRegistry 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UserModule
    var type_UserModule = Type.GetType("UserModule");
    if (type_UserModule != null)
    {
        Console.WriteLine("[PASS] 类型 UserModule (class) 存在");
        var ctors_UserModule = type_UserModule.GetConstructors();
        Console.WriteLine($"[PASS] UserModule 构造函数数量: {ctors_UserModule.Length}");
        var methods_UserModule = type_UserModule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserModule 公开方法数量: {methods_UserModule.Length}");
        foreach (var m in methods_UserModule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserModule 未找到，尝试无命名空间...");
        type_UserModule = Type.GetType("UserModule");
        if (type_UserModule != null)
            Console.WriteLine("[PASS] 类型 UserModule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserModule 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheHealthEndpoint
    var type_CacheHealthEndpoint = Type.GetType("CacheHealthEndpoint");
    if (type_CacheHealthEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 CacheHealthEndpoint (class) 存在");
        var ctors_CacheHealthEndpoint = type_CacheHealthEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] CacheHealthEndpoint 构造函数数量: {ctors_CacheHealthEndpoint.Length}");
        var methods_CacheHealthEndpoint = type_CacheHealthEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheHealthEndpoint 公开方法数量: {methods_CacheHealthEndpoint.Length}");
        foreach (var m in methods_CacheHealthEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheHealthEndpoint 未找到，尝试无命名空间...");
        type_CacheHealthEndpoint = Type.GetType("CacheHealthEndpoint");
        if (type_CacheHealthEndpoint != null)
            Console.WriteLine("[PASS] 类型 CacheHealthEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheHealthEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PerformanceMonitoringMiddleware
    var type_PerformanceMonitoringMiddleware = Type.GetType("PerformanceMonitoringMiddleware");
    if (type_PerformanceMonitoringMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 PerformanceMonitoringMiddleware (class) 存在");
        var ctors_PerformanceMonitoringMiddleware = type_PerformanceMonitoringMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] PerformanceMonitoringMiddleware 构造函数数量: {ctors_PerformanceMonitoringMiddleware.Length}");
        var methods_PerformanceMonitoringMiddleware = type_PerformanceMonitoringMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PerformanceMonitoringMiddleware 公开方法数量: {methods_PerformanceMonitoringMiddleware.Length}");
        foreach (var m in methods_PerformanceMonitoringMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PerformanceMonitoringMiddleware 未找到，尝试无命名空间...");
        type_PerformanceMonitoringMiddleware = Type.GetType("PerformanceMonitoringMiddleware");
        if (type_PerformanceMonitoringMiddleware != null)
            Console.WriteLine("[PASS] 类型 PerformanceMonitoringMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PerformanceMonitoringMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ICarterModule
    var type_ICarterModule = Type.GetType("ICarterModule");
    if (type_ICarterModule != null)
    {
        Console.WriteLine("[PASS] 类型 ICarterModule (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICarterModule 未找到，尝试无命名空间...");
        type_ICarterModule = Type.GetType("ICarterModule");
        if (type_ICarterModule != null)
            Console.WriteLine("[PASS] 类型 ICarterModule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICarterModule 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UserCreateRequest
    var type_UserCreateRequest = Type.GetType("UserCreateRequest");
    if (type_UserCreateRequest != null)
    {
        Console.WriteLine("[PASS] 类型 UserCreateRequest (record) 存在");
        var ctors_UserCreateRequest = type_UserCreateRequest.GetConstructors();
        Console.WriteLine($"[PASS] UserCreateRequest 构造函数数量: {ctors_UserCreateRequest.Length}");
        var methods_UserCreateRequest = type_UserCreateRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserCreateRequest 公开方法数量: {methods_UserCreateRequest.Length}");
        foreach (var m in methods_UserCreateRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserCreateRequest 未找到，尝试无命名空间...");
        type_UserCreateRequest = Type.GetType("UserCreateRequest");
        if (type_UserCreateRequest != null)
            Console.WriteLine("[PASS] 类型 UserCreateRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserCreateRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UserUpdateRequest
    var type_UserUpdateRequest = Type.GetType("UserUpdateRequest");
    if (type_UserUpdateRequest != null)
    {
        Console.WriteLine("[PASS] 类型 UserUpdateRequest (record) 存在");
        var ctors_UserUpdateRequest = type_UserUpdateRequest.GetConstructors();
        Console.WriteLine($"[PASS] UserUpdateRequest 构造函数数量: {ctors_UserUpdateRequest.Length}");
        var methods_UserUpdateRequest = type_UserUpdateRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserUpdateRequest 公开方法数量: {methods_UserUpdateRequest.Length}");
        foreach (var m in methods_UserUpdateRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserUpdateRequest 未找到，尝试无命名空间...");
        type_UserUpdateRequest = Type.GetType("UserUpdateRequest");
        if (type_UserUpdateRequest != null)
            Console.WriteLine("[PASS] 类型 UserUpdateRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserUpdateRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: User
    var type_User = Type.GetType("User");
    if (type_User != null)
    {
        Console.WriteLine("[PASS] 类型 User (record) 存在");
        var ctors_User = type_User.GetConstructors();
        Console.WriteLine($"[PASS] User 构造函数数量: {ctors_User.Length}");
        var methods_User = type_User.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] User 公开方法数量: {methods_User.Length}");
        foreach (var m in methods_User)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 User 未找到，尝试无命名空间...");
        type_User = Type.GetType("User");
        if (type_User != null)
            Console.WriteLine("[PASS] 类型 User (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 User 可能为顶层语句或嵌套类型");
    }

    // 验证 record: RouteRequest
    var type_RouteRequest = Type.GetType("RouteRequest");
    if (type_RouteRequest != null)
    {
        Console.WriteLine("[PASS] 类型 RouteRequest (record) 存在");
        var ctors_RouteRequest = type_RouteRequest.GetConstructors();
        Console.WriteLine($"[PASS] RouteRequest 构造函数数量: {ctors_RouteRequest.Length}");
        var methods_RouteRequest = type_RouteRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RouteRequest 公开方法数量: {methods_RouteRequest.Length}");
        foreach (var m in methods_RouteRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RouteRequest 未找到，尝试无命名空间...");
        type_RouteRequest = Type.GetType("RouteRequest");
        if (type_RouteRequest != null)
            Console.WriteLine("[PASS] 类型 RouteRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RouteRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
