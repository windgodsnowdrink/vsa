#load "choetl_integration.cs"

Console.WriteLine("=== choetl_integration.cs Test ===");

try
{
    // 验证 class: ChoETLIntegration.ChoETLOptions
    var type_ChoETLOptions = Type.GetType("ChoETLIntegration.ChoETLOptions");
    if (type_ChoETLOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETLIntegration.ChoETLOptions (class) 存在");
        var ctors_ChoETLOptions = type_ChoETLOptions.GetConstructors();
        Console.WriteLine($"[PASS] ChoETLIntegration.ChoETLOptions 构造函数数量: {ctors_ChoETLOptions.Length}");
        var methods_ChoETLOptions = type_ChoETLOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETLIntegration.ChoETLOptions 公开方法数量: {methods_ChoETLOptions.Length}");
        foreach (var m in methods_ChoETLOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETLIntegration.ChoETLOptions 未找到，尝试无命名空间...");
        type_ChoETLOptions = Type.GetType("ChoETLOptions");
        if (type_ChoETLOptions != null)
            Console.WriteLine("[PASS] 类型 ChoETLOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChoETLOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChoETLIntegration.DataProcessingResult
    var type_DataProcessingResult = Type.GetType("ChoETLIntegration.DataProcessingResult");
    if (type_DataProcessingResult != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETLIntegration.DataProcessingResult (class) 存在");
        var ctors_DataProcessingResult = type_DataProcessingResult.GetConstructors();
        Console.WriteLine($"[PASS] ChoETLIntegration.DataProcessingResult 构造函数数量: {ctors_DataProcessingResult.Length}");
        var methods_DataProcessingResult = type_DataProcessingResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETLIntegration.DataProcessingResult 公开方法数量: {methods_DataProcessingResult.Length}");
        foreach (var m in methods_DataProcessingResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETLIntegration.DataProcessingResult 未找到，尝试无命名空间...");
        type_DataProcessingResult = Type.GetType("DataProcessingResult");
        if (type_DataProcessingResult != null)
            Console.WriteLine("[PASS] 类型 DataProcessingResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataProcessingResult 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChoETLIntegration.ETLService
    var type_ETLService = Type.GetType("ChoETLIntegration.ETLService");
    if (type_ETLService != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETLIntegration.ETLService (class) 存在");
        var ctors_ETLService = type_ETLService.GetConstructors();
        Console.WriteLine($"[PASS] ChoETLIntegration.ETLService 构造函数数量: {ctors_ETLService.Length}");
        var methods_ETLService = type_ETLService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETLIntegration.ETLService 公开方法数量: {methods_ETLService.Length}");
        foreach (var m in methods_ETLService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETLIntegration.ETLService 未找到，尝试无命名空间...");
        type_ETLService = Type.GetType("ETLService");
        if (type_ETLService != null)
            Console.WriteLine("[PASS] 类型 ETLService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ETLService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChoETLIntegration.ChoETLServiceCollectionExtensions
    var type_ChoETLServiceCollectionExtensions = Type.GetType("ChoETLIntegration.ChoETLServiceCollectionExtensions");
    if (type_ChoETLServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETLIntegration.ChoETLServiceCollectionExtensions (class) 存在");
        var ctors_ChoETLServiceCollectionExtensions = type_ChoETLServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ChoETLIntegration.ChoETLServiceCollectionExtensions 构造函数数量: {ctors_ChoETLServiceCollectionExtensions.Length}");
        var methods_ChoETLServiceCollectionExtensions = type_ChoETLServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETLIntegration.ChoETLServiceCollectionExtensions 公开方法数量: {methods_ChoETLServiceCollectionExtensions.Length}");
        foreach (var m in methods_ChoETLServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETLIntegration.ChoETLServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ChoETLServiceCollectionExtensions = Type.GetType("ChoETLServiceCollectionExtensions");
        if (type_ChoETLServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ChoETLServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChoETLServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ChoETLIntegration.IETLService
    var type_IETLService = Type.GetType("ChoETLIntegration.IETLService");
    if (type_IETLService != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETLIntegration.IETLService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETLIntegration.IETLService 未找到，尝试无命名空间...");
        type_IETLService = Type.GetType("IETLService");
        if (type_IETLService != null)
            Console.WriteLine("[PASS] 类型 IETLService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IETLService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ChoETLIntegration.in
    var type_in = Type.GetType("ChoETLIntegration.in");
    if (type_in != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETLIntegration.in (record) 存在");
        var ctors_in = type_in.GetConstructors();
        Console.WriteLine($"[PASS] ChoETLIntegration.in 构造函数数量: {ctors_in.Length}");
        var methods_in = type_in.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETLIntegration.in 公开方法数量: {methods_in.Length}");
        foreach (var m in methods_in)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETLIntegration.in 未找到，尝试无命名空间...");
        type_in = Type.GetType("in");
        if (type_in != null)
            Console.WriteLine("[PASS] 类型 in (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 in 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
