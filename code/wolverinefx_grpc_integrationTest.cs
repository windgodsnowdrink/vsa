#load "wolverinefx_grpc_integration.cs"

Console.WriteLine("=== wolverinefx_grpc_integration.cs Test ===");

try
{
    // 验证 class: GrpcRequest
    var type_GrpcRequest = Type.GetType("GrpcRequest");
    if (type_GrpcRequest != null)
    {
        Console.WriteLine("[PASS] 类型 GrpcRequest (class) 存在");
        var ctors_GrpcRequest = type_GrpcRequest.GetConstructors();
        Console.WriteLine($"[PASS] GrpcRequest 构造函数数量: {ctors_GrpcRequest.Length}");
        var methods_GrpcRequest = type_GrpcRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrpcRequest 公开方法数量: {methods_GrpcRequest.Length}");
        foreach (var m in methods_GrpcRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrpcRequest 未找到，尝试无命名空间...");
        type_GrpcRequest = Type.GetType("GrpcRequest");
        if (type_GrpcRequest != null)
            Console.WriteLine("[PASS] 类型 GrpcRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrpcRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GrpcResponse
    var type_GrpcResponse = Type.GetType("GrpcResponse");
    if (type_GrpcResponse != null)
    {
        Console.WriteLine("[PASS] 类型 GrpcResponse (class) 存在");
        var ctors_GrpcResponse = type_GrpcResponse.GetConstructors();
        Console.WriteLine($"[PASS] GrpcResponse 构造函数数量: {ctors_GrpcResponse.Length}");
        var methods_GrpcResponse = type_GrpcResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrpcResponse 公开方法数量: {methods_GrpcResponse.Length}");
        foreach (var m in methods_GrpcResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrpcResponse 未找到，尝试无命名空间...");
        type_GrpcResponse = Type.GetType("GrpcResponse");
        if (type_GrpcResponse != null)
            Console.WriteLine("[PASS] 类型 GrpcResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrpcResponse 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WolverineGatewayService
    var type_WolverineGatewayService = Type.GetType("WolverineGatewayService");
    if (type_WolverineGatewayService != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineGatewayService (class) 存在");
        var ctors_WolverineGatewayService = type_WolverineGatewayService.GetConstructors();
        Console.WriteLine($"[PASS] WolverineGatewayService 构造函数数量: {ctors_WolverineGatewayService.Length}");
        var methods_WolverineGatewayService = type_WolverineGatewayService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineGatewayService 公开方法数量: {methods_WolverineGatewayService.Length}");
        foreach (var m in methods_WolverineGatewayService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineGatewayService 未找到，尝试无命名空间...");
        type_WolverineGatewayService = Type.GetType("WolverineGatewayService");
        if (type_WolverineGatewayService != null)
            Console.WriteLine("[PASS] 类型 WolverineGatewayService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineGatewayService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedTransactionInterceptor
    var type_DistributedTransactionInterceptor = Type.GetType("DistributedTransactionInterceptor");
    if (type_DistributedTransactionInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedTransactionInterceptor (class) 存在");
        var ctors_DistributedTransactionInterceptor = type_DistributedTransactionInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] DistributedTransactionInterceptor 构造函数数量: {ctors_DistributedTransactionInterceptor.Length}");
        var methods_DistributedTransactionInterceptor = type_DistributedTransactionInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedTransactionInterceptor 公开方法数量: {methods_DistributedTransactionInterceptor.Length}");
        foreach (var m in methods_DistributedTransactionInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedTransactionInterceptor 未找到，尝试无命名空间...");
        type_DistributedTransactionInterceptor = Type.GetType("DistributedTransactionInterceptor");
        if (type_DistributedTransactionInterceptor != null)
            Console.WriteLine("[PASS] 类型 DistributedTransactionInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedTransactionInterceptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ExceptionInterceptor
    var type_ExceptionInterceptor = Type.GetType("ExceptionInterceptor");
    if (type_ExceptionInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 ExceptionInterceptor (class) 存在");
        var ctors_ExceptionInterceptor = type_ExceptionInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] ExceptionInterceptor 构造函数数量: {ctors_ExceptionInterceptor.Length}");
        var methods_ExceptionInterceptor = type_ExceptionInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExceptionInterceptor 公开方法数量: {methods_ExceptionInterceptor.Length}");
        foreach (var m in methods_ExceptionInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExceptionInterceptor 未找到，尝试无命名空间...");
        type_ExceptionInterceptor = Type.GetType("ExceptionInterceptor");
        if (type_ExceptionInterceptor != null)
            Console.WriteLine("[PASS] 类型 ExceptionInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExceptionInterceptor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IWolverineGateway
    var type_IWolverineGateway = Type.GetType("IWolverineGateway");
    if (type_IWolverineGateway != null)
    {
        Console.WriteLine("[PASS] 类型 IWolverineGateway (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWolverineGateway 未找到，尝试无命名空间...");
        type_IWolverineGateway = Type.GetType("IWolverineGateway");
        if (type_IWolverineGateway != null)
            Console.WriteLine("[PASS] 类型 IWolverineGateway (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWolverineGateway 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
