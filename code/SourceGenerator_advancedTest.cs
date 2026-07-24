#load "SourceGenerator_advanced.cs"

Console.WriteLine("=== SourceGenerator_advanced.cs Test ===");

try
{
    // 验证 class: AdvancedDtoGenerator
    var type_AdvancedDtoGenerator = Type.GetType("AdvancedDtoGenerator");
    if (type_AdvancedDtoGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedDtoGenerator (class) 存在");
        var ctors_AdvancedDtoGenerator = type_AdvancedDtoGenerator.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedDtoGenerator 构造函数数量: {ctors_AdvancedDtoGenerator.Length}");
        var methods_AdvancedDtoGenerator = type_AdvancedDtoGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedDtoGenerator 公开方法数量: {methods_AdvancedDtoGenerator.Length}");
        foreach (var m in methods_AdvancedDtoGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedDtoGenerator 未找到，尝试无命名空间...");
        type_AdvancedDtoGenerator = Type.GetType("AdvancedDtoGenerator");
        if (type_AdvancedDtoGenerator != null)
            Console.WriteLine("[PASS] 类型 AdvancedDtoGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedDtoGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DtoInfo
    var type_DtoInfo = Type.GetType("DtoInfo");
    if (type_DtoInfo != null)
    {
        Console.WriteLine("[PASS] 类型 DtoInfo (class) 存在");
        var ctors_DtoInfo = type_DtoInfo.GetConstructors();
        Console.WriteLine($"[PASS] DtoInfo 构造函数数量: {ctors_DtoInfo.Length}");
        var methods_DtoInfo = type_DtoInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DtoInfo 公开方法数量: {methods_DtoInfo.Length}");
        foreach (var m in methods_DtoInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DtoInfo 未找到，尝试无命名空间...");
        type_DtoInfo = Type.GetType("DtoInfo");
        if (type_DtoInfo != null)
            Console.WriteLine("[PASS] 类型 DtoInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DtoInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PropertyInfo
    var type_PropertyInfo = Type.GetType("PropertyInfo");
    if (type_PropertyInfo != null)
    {
        Console.WriteLine("[PASS] 类型 PropertyInfo (class) 存在");
        var ctors_PropertyInfo = type_PropertyInfo.GetConstructors();
        Console.WriteLine($"[PASS] PropertyInfo 构造函数数量: {ctors_PropertyInfo.Length}");
        var methods_PropertyInfo = type_PropertyInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PropertyInfo 公开方法数量: {methods_PropertyInfo.Length}");
        foreach (var m in methods_PropertyInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PropertyInfo 未找到，尝试无命名空间...");
        type_PropertyInfo = Type.GetType("PropertyInfo");
        if (type_PropertyInfo != null)
            Console.WriteLine("[PASS] 类型 PropertyInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PropertyInfo 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
