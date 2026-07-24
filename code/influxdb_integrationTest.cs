#load "influxdb_integration.cs"

Console.WriteLine("=== influxdb_integration.cs Test ===");

try
{
    // 验证 class: InfluxDbExtensions
    var type_InfluxDbExtensions = Type.GetType("InfluxDbExtensions");
    if (type_InfluxDbExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 InfluxDbExtensions (class) 存在");
        var ctors_InfluxDbExtensions = type_InfluxDbExtensions.GetConstructors();
        Console.WriteLine($"[PASS] InfluxDbExtensions 构造函数数量: {ctors_InfluxDbExtensions.Length}");
        var methods_InfluxDbExtensions = type_InfluxDbExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InfluxDbExtensions 公开方法数量: {methods_InfluxDbExtensions.Length}");
        foreach (var m in methods_InfluxDbExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InfluxDbExtensions 未找到，尝试无命名空间...");
        type_InfluxDbExtensions = Type.GetType("InfluxDbExtensions");
        if (type_InfluxDbExtensions != null)
            Console.WriteLine("[PASS] 类型 InfluxDbExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InfluxDbExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: InfluxDbBackgroundWriter
    var type_InfluxDbBackgroundWriter = Type.GetType("InfluxDbBackgroundWriter");
    if (type_InfluxDbBackgroundWriter != null)
    {
        Console.WriteLine("[PASS] 类型 InfluxDbBackgroundWriter (class) 存在");
        var ctors_InfluxDbBackgroundWriter = type_InfluxDbBackgroundWriter.GetConstructors();
        Console.WriteLine($"[PASS] InfluxDbBackgroundWriter 构造函数数量: {ctors_InfluxDbBackgroundWriter.Length}");
        var methods_InfluxDbBackgroundWriter = type_InfluxDbBackgroundWriter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InfluxDbBackgroundWriter 公开方法数量: {methods_InfluxDbBackgroundWriter.Length}");
        foreach (var m in methods_InfluxDbBackgroundWriter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InfluxDbBackgroundWriter 未找到，尝试无命名空间...");
        type_InfluxDbBackgroundWriter = Type.GetType("InfluxDbBackgroundWriter");
        if (type_InfluxDbBackgroundWriter != null)
            Console.WriteLine("[PASS] 类型 InfluxDbBackgroundWriter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InfluxDbBackgroundWriter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: InfluxDbQueryService
    var type_InfluxDbQueryService = Type.GetType("InfluxDbQueryService");
    if (type_InfluxDbQueryService != null)
    {
        Console.WriteLine("[PASS] 类型 InfluxDbQueryService (class) 存在");
        var ctors_InfluxDbQueryService = type_InfluxDbQueryService.GetConstructors();
        Console.WriteLine($"[PASS] InfluxDbQueryService 构造函数数量: {ctors_InfluxDbQueryService.Length}");
        var methods_InfluxDbQueryService = type_InfluxDbQueryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InfluxDbQueryService 公开方法数量: {methods_InfluxDbQueryService.Length}");
        foreach (var m in methods_InfluxDbQueryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InfluxDbQueryService 未找到，尝试无命名空间...");
        type_InfluxDbQueryService = Type.GetType("InfluxDbQueryService");
        if (type_InfluxDbQueryService != null)
            Console.WriteLine("[PASS] 类型 InfluxDbQueryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InfluxDbQueryService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: InfluxDbOptions
    var type_InfluxDbOptions = Type.GetType("InfluxDbOptions");
    if (type_InfluxDbOptions != null)
    {
        Console.WriteLine("[PASS] 类型 InfluxDbOptions (record) 存在");
        var ctors_InfluxDbOptions = type_InfluxDbOptions.GetConstructors();
        Console.WriteLine($"[PASS] InfluxDbOptions 构造函数数量: {ctors_InfluxDbOptions.Length}");
        var methods_InfluxDbOptions = type_InfluxDbOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InfluxDbOptions 公开方法数量: {methods_InfluxDbOptions.Length}");
        foreach (var m in methods_InfluxDbOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InfluxDbOptions 未找到，尝试无命名空间...");
        type_InfluxDbOptions = Type.GetType("InfluxDbOptions");
        if (type_InfluxDbOptions != null)
            Console.WriteLine("[PASS] 类型 InfluxDbOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InfluxDbOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 record: in
    var type_in = Type.GetType("in");
    if (type_in != null)
    {
        Console.WriteLine("[PASS] 类型 in (record) 存在");
        var ctors_in = type_in.GetConstructors();
        Console.WriteLine($"[PASS] in 构造函数数量: {ctors_in.Length}");
        var methods_in = type_in.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] in 公开方法数量: {methods_in.Length}");
        foreach (var m in methods_in)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 in 未找到，尝试无命名空间...");
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
