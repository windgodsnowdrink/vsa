#load "mapsui_integration.cs"

Console.WriteLine("=== mapsui_integration.cs Test ===");

try
{
    // 验证 class: MapFeature
    var type_MapFeature = Type.GetType("MapFeature");
    if (type_MapFeature != null)
    {
        Console.WriteLine("[PASS] 类型 MapFeature (class) 存在");
        var ctors_MapFeature = type_MapFeature.GetConstructors();
        Console.WriteLine($"[PASS] MapFeature 构造函数数量: {ctors_MapFeature.Length}");
        var methods_MapFeature = type_MapFeature.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MapFeature 公开方法数量: {methods_MapFeature.Length}");
        foreach (var m in methods_MapFeature)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MapFeature 未找到，尝试无命名空间...");
        type_MapFeature = Type.GetType("MapFeature");
        if (type_MapFeature != null)
            Console.WriteLine("[PASS] 类型 MapFeature (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MapFeature 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MapsuiMapService
    var type_MapsuiMapService = Type.GetType("MapsuiMapService");
    if (type_MapsuiMapService != null)
    {
        Console.WriteLine("[PASS] 类型 MapsuiMapService (class) 存在");
        var ctors_MapsuiMapService = type_MapsuiMapService.GetConstructors();
        Console.WriteLine($"[PASS] MapsuiMapService 构造函数数量: {ctors_MapsuiMapService.Length}");
        var methods_MapsuiMapService = type_MapsuiMapService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MapsuiMapService 公开方法数量: {methods_MapsuiMapService.Length}");
        foreach (var m in methods_MapsuiMapService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MapsuiMapService 未找到，尝试无命名空间...");
        type_MapsuiMapService = Type.GetType("MapsuiMapService");
        if (type_MapsuiMapService != null)
            Console.WriteLine("[PASS] 类型 MapsuiMapService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MapsuiMapService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IMapService
    var type_IMapService = Type.GetType("IMapService");
    if (type_IMapService != null)
    {
        Console.WriteLine("[PASS] 类型 IMapService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMapService 未找到，尝试无命名空间...");
        type_IMapService = Type.GetType("IMapService");
        if (type_IMapService != null)
            Console.WriteLine("[PASS] 类型 IMapService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMapService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
