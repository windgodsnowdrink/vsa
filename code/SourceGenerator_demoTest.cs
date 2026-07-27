#load "SourceGenerator_demo.cs"

Console.WriteLine("=== SourceGenerator_demo.cs Test ===");

try
{
    // 验证 class: DtoGenerator
    var type_DtoGenerator = Type.GetType("DtoGenerator");
    if (type_DtoGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 DtoGenerator (class) 存在");
        var ctors_DtoGenerator = type_DtoGenerator.GetConstructors();
        Console.WriteLine($"[PASS] DtoGenerator 构造函数数量: {ctors_DtoGenerator.Length}");
        var methods_DtoGenerator = type_DtoGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DtoGenerator 公开方法数量: {methods_DtoGenerator.Length}");
        foreach (var m in methods_DtoGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DtoGenerator 未找到，尝试无命名空间...");
        type_DtoGenerator = Type.GetType("DtoGenerator");
        if (type_DtoGenerator != null)
            Console.WriteLine("[PASS] 类型 DtoGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DtoGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DtoSyntaxReceiver
    var type_DtoSyntaxReceiver = Type.GetType("DtoSyntaxReceiver");
    if (type_DtoSyntaxReceiver != null)
    {
        Console.WriteLine("[PASS] 类型 DtoSyntaxReceiver (class) 存在");
        var ctors_DtoSyntaxReceiver = type_DtoSyntaxReceiver.GetConstructors();
        Console.WriteLine($"[PASS] DtoSyntaxReceiver 构造函数数量: {ctors_DtoSyntaxReceiver.Length}");
        var methods_DtoSyntaxReceiver = type_DtoSyntaxReceiver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DtoSyntaxReceiver 公开方法数量: {methods_DtoSyntaxReceiver.Length}");
        foreach (var m in methods_DtoSyntaxReceiver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DtoSyntaxReceiver 未找到，尝试无命名空间...");
        type_DtoSyntaxReceiver = Type.GetType("DtoSyntaxReceiver");
        if (type_DtoSyntaxReceiver != null)
            Console.WriteLine("[PASS] 类型 DtoSyntaxReceiver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DtoSyntaxReceiver 可能为顶层语句或嵌套类型");
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
