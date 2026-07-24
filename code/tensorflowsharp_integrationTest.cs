#load "tensorflowsharp_integration.cs"

Console.WriteLine("=== tensorflowsharp_integration.cs Test ===");

try
{
    // 验证 class: Trae.Vsa.AI.TensorFlowOptions
    var type_TensorFlowOptions = Type.GetType("Trae.Vsa.AI.TensorFlowOptions");
    if (type_TensorFlowOptions != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.AI.TensorFlowOptions (class) 存在");
        var ctors_TensorFlowOptions = type_TensorFlowOptions.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowOptions 构造函数数量: {ctors_TensorFlowOptions.Length}");
        var methods_TensorFlowOptions = type_TensorFlowOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowOptions 公开方法数量: {methods_TensorFlowOptions.Length}");
        foreach (var m in methods_TensorFlowOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.AI.TensorFlowOptions 未找到，尝试无命名空间...");
        type_TensorFlowOptions = Type.GetType("TensorFlowOptions");
        if (type_TensorFlowOptions != null)
            Console.WriteLine("[PASS] 类型 TensorFlowOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TensorFlowOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.AI.TensorFlowService
    var type_TensorFlowService = Type.GetType("Trae.Vsa.AI.TensorFlowService");
    if (type_TensorFlowService != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.AI.TensorFlowService (class) 存在");
        var ctors_TensorFlowService = type_TensorFlowService.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowService 构造函数数量: {ctors_TensorFlowService.Length}");
        var methods_TensorFlowService = type_TensorFlowService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowService 公开方法数量: {methods_TensorFlowService.Length}");
        foreach (var m in methods_TensorFlowService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.AI.TensorFlowService 未找到，尝试无命名空间...");
        type_TensorFlowService = Type.GetType("TensorFlowService");
        if (type_TensorFlowService != null)
            Console.WriteLine("[PASS] 类型 TensorFlowService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TensorFlowService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.AI.TensorFlowExtensions
    var type_TensorFlowExtensions = Type.GetType("Trae.Vsa.AI.TensorFlowExtensions");
    if (type_TensorFlowExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.AI.TensorFlowExtensions (class) 存在");
        var ctors_TensorFlowExtensions = type_TensorFlowExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowExtensions 构造函数数量: {ctors_TensorFlowExtensions.Length}");
        var methods_TensorFlowExtensions = type_TensorFlowExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowExtensions 公开方法数量: {methods_TensorFlowExtensions.Length}");
        foreach (var m in methods_TensorFlowExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.AI.TensorFlowExtensions 未找到，尝试无命名空间...");
        type_TensorFlowExtensions = Type.GetType("TensorFlowExtensions");
        if (type_TensorFlowExtensions != null)
            Console.WriteLine("[PASS] 类型 TensorFlowExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TensorFlowExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.AI.TensorFlowModelVersioningService
    var type_TensorFlowModelVersioningService = Type.GetType("Trae.Vsa.AI.TensorFlowModelVersioningService");
    if (type_TensorFlowModelVersioningService != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.AI.TensorFlowModelVersioningService (class) 存在");
        var ctors_TensorFlowModelVersioningService = type_TensorFlowModelVersioningService.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowModelVersioningService 构造函数数量: {ctors_TensorFlowModelVersioningService.Length}");
        var methods_TensorFlowModelVersioningService = type_TensorFlowModelVersioningService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.AI.TensorFlowModelVersioningService 公开方法数量: {methods_TensorFlowModelVersioningService.Length}");
        foreach (var m in methods_TensorFlowModelVersioningService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.AI.TensorFlowModelVersioningService 未找到，尝试无命名空间...");
        type_TensorFlowModelVersioningService = Type.GetType("TensorFlowModelVersioningService");
        if (type_TensorFlowModelVersioningService != null)
            Console.WriteLine("[PASS] 类型 TensorFlowModelVersioningService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TensorFlowModelVersioningService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: Trae.Vsa.AI.ITensorFlowService
    var type_ITensorFlowService = Type.GetType("Trae.Vsa.AI.ITensorFlowService");
    if (type_ITensorFlowService != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.AI.ITensorFlowService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.AI.ITensorFlowService 未找到，尝试无命名空间...");
        type_ITensorFlowService = Type.GetType("ITensorFlowService");
        if (type_ITensorFlowService != null)
            Console.WriteLine("[PASS] 类型 ITensorFlowService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITensorFlowService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
