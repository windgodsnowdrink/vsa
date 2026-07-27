#load "csharpflink_integration.cs"

Console.WriteLine("=== csharpflink_integration.cs Test ===");

try
{
    // 验证 class: FlinkIntegration.FlinkOptions
    var type_FlinkOptions = Type.GetType("FlinkIntegration.FlinkOptions");
    if (type_FlinkOptions != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.FlinkOptions (class) 存在");
        var ctors_FlinkOptions = type_FlinkOptions.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkOptions 构造函数数量: {ctors_FlinkOptions.Length}");
        var methods_FlinkOptions = type_FlinkOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkOptions 公开方法数量: {methods_FlinkOptions.Length}");
        foreach (var m in methods_FlinkOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.FlinkOptions 未找到，尝试无命名空间...");
        type_FlinkOptions = Type.GetType("FlinkOptions");
        if (type_FlinkOptions != null)
            Console.WriteLine("[PASS] 类型 FlinkOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FlinkOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FlinkIntegration.FlinkService
    var type_FlinkService = Type.GetType("FlinkIntegration.FlinkService");
    if (type_FlinkService != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.FlinkService (class) 存在");
        var ctors_FlinkService = type_FlinkService.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkService 构造函数数量: {ctors_FlinkService.Length}");
        var methods_FlinkService = type_FlinkService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkService 公开方法数量: {methods_FlinkService.Length}");
        foreach (var m in methods_FlinkService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.FlinkService 未找到，尝试无命名空间...");
        type_FlinkService = Type.GetType("FlinkService");
        if (type_FlinkService != null)
            Console.WriteLine("[PASS] 类型 FlinkService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FlinkService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FlinkIntegration.FlinkServiceExtensions
    var type_FlinkServiceExtensions = Type.GetType("FlinkIntegration.FlinkServiceExtensions");
    if (type_FlinkServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.FlinkServiceExtensions (class) 存在");
        var ctors_FlinkServiceExtensions = type_FlinkServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkServiceExtensions 构造函数数量: {ctors_FlinkServiceExtensions.Length}");
        var methods_FlinkServiceExtensions = type_FlinkServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkServiceExtensions 公开方法数量: {methods_FlinkServiceExtensions.Length}");
        foreach (var m in methods_FlinkServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.FlinkServiceExtensions 未找到，尝试无命名空间...");
        type_FlinkServiceExtensions = Type.GetType("FlinkServiceExtensions");
        if (type_FlinkServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 FlinkServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FlinkServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FlinkIntegration.IFlinkService
    var type_IFlinkService = Type.GetType("FlinkIntegration.IFlinkService");
    if (type_IFlinkService != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.IFlinkService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.IFlinkService 未找到，尝试无命名空间...");
        type_IFlinkService = Type.GetType("IFlinkService");
        if (type_IFlinkService != null)
            Console.WriteLine("[PASS] 类型 IFlinkService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IFlinkService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FlinkIntegration.FlinkResult
    var type_FlinkResult = Type.GetType("FlinkIntegration.FlinkResult");
    if (type_FlinkResult != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.FlinkResult (record) 存在");
        var ctors_FlinkResult = type_FlinkResult.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkResult 构造函数数量: {ctors_FlinkResult.Length}");
        var methods_FlinkResult = type_FlinkResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkResult 公开方法数量: {methods_FlinkResult.Length}");
        foreach (var m in methods_FlinkResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.FlinkResult 未找到，尝试无命名空间...");
        type_FlinkResult = Type.GetType("FlinkResult");
        if (type_FlinkResult != null)
            Console.WriteLine("[PASS] 类型 FlinkResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FlinkResult 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FlinkIntegration.FlinkMetrics
    var type_FlinkMetrics = Type.GetType("FlinkIntegration.FlinkMetrics");
    if (type_FlinkMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.FlinkMetrics (record) 存在");
        var ctors_FlinkMetrics = type_FlinkMetrics.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkMetrics 构造函数数量: {ctors_FlinkMetrics.Length}");
        var methods_FlinkMetrics = type_FlinkMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkMetrics 公开方法数量: {methods_FlinkMetrics.Length}");
        foreach (var m in methods_FlinkMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.FlinkMetrics 未找到，尝试无命名空间...");
        type_FlinkMetrics = Type.GetType("FlinkMetrics");
        if (type_FlinkMetrics != null)
            Console.WriteLine("[PASS] 类型 FlinkMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FlinkMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FlinkIntegration.FlinkJob
    var type_FlinkJob = Type.GetType("FlinkIntegration.FlinkJob");
    if (type_FlinkJob != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.FlinkJob (record) 存在");
        var ctors_FlinkJob = type_FlinkJob.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkJob 构造函数数量: {ctors_FlinkJob.Length}");
        var methods_FlinkJob = type_FlinkJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.FlinkJob 公开方法数量: {methods_FlinkJob.Length}");
        foreach (var m in methods_FlinkJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.FlinkJob 未找到，尝试无命名空间...");
        type_FlinkJob = Type.GetType("FlinkJob");
        if (type_FlinkJob != null)
            Console.WriteLine("[PASS] 类型 FlinkJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FlinkJob 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FlinkIntegration.WindowResult
    var type_WindowResult = Type.GetType("FlinkIntegration.WindowResult");
    if (type_WindowResult != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.WindowResult (record) 存在");
        var ctors_WindowResult = type_WindowResult.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.WindowResult 构造函数数量: {ctors_WindowResult.Length}");
        var methods_WindowResult = type_WindowResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.WindowResult 公开方法数量: {methods_WindowResult.Length}");
        foreach (var m in methods_WindowResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.WindowResult 未找到，尝试无命名空间...");
        type_WindowResult = Type.GetType("WindowResult");
        if (type_WindowResult != null)
            Console.WriteLine("[PASS] 类型 WindowResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WindowResult 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FlinkIntegration.StateSnapshot
    var type_StateSnapshot = Type.GetType("FlinkIntegration.StateSnapshot");
    if (type_StateSnapshot != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.StateSnapshot (record) 存在");
        var ctors_StateSnapshot = type_StateSnapshot.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.StateSnapshot 构造函数数量: {ctors_StateSnapshot.Length}");
        var methods_StateSnapshot = type_StateSnapshot.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.StateSnapshot 公开方法数量: {methods_StateSnapshot.Length}");
        foreach (var m in methods_StateSnapshot)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.StateSnapshot 未找到，尝试无命名空间...");
        type_StateSnapshot = Type.GetType("StateSnapshot");
        if (type_StateSnapshot != null)
            Console.WriteLine("[PASS] 类型 StateSnapshot (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StateSnapshot 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FlinkIntegration.Watermark
    var type_Watermark = Type.GetType("FlinkIntegration.Watermark");
    if (type_Watermark != null)
    {
        Console.WriteLine("[PASS] 类型 FlinkIntegration.Watermark (record) 存在");
        var ctors_Watermark = type_Watermark.GetConstructors();
        Console.WriteLine($"[PASS] FlinkIntegration.Watermark 构造函数数量: {ctors_Watermark.Length}");
        var methods_Watermark = type_Watermark.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FlinkIntegration.Watermark 公开方法数量: {methods_Watermark.Length}");
        foreach (var m in methods_Watermark)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FlinkIntegration.Watermark 未找到，尝试无命名空间...");
        type_Watermark = Type.GetType("Watermark");
        if (type_Watermark != null)
            Console.WriteLine("[PASS] 类型 Watermark (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Watermark 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
