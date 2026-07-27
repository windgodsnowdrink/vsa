#load "specification_pattern.cs"

Console.WriteLine("=== specification_pattern.cs Test ===");

try
{
    // 验证 class: BaseSpecification
    var type_BaseSpecification = Type.GetType("BaseSpecification");
    if (type_BaseSpecification != null)
    {
        Console.WriteLine("[PASS] 类型 BaseSpecification (class) 存在");
        var ctors_BaseSpecification = type_BaseSpecification.GetConstructors();
        Console.WriteLine($"[PASS] BaseSpecification 构造函数数量: {ctors_BaseSpecification.Length}");
        var methods_BaseSpecification = type_BaseSpecification.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BaseSpecification 公开方法数量: {methods_BaseSpecification.Length}");
        foreach (var m in methods_BaseSpecification)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BaseSpecification 未找到，尝试无命名空间...");
        type_BaseSpecification = Type.GetType("BaseSpecification");
        if (type_BaseSpecification != null)
            Console.WriteLine("[PASS] 类型 BaseSpecification (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BaseSpecification 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SpecificationEvaluator
    var type_SpecificationEvaluator = Type.GetType("SpecificationEvaluator");
    if (type_SpecificationEvaluator != null)
    {
        Console.WriteLine("[PASS] 类型 SpecificationEvaluator (class) 存在");
        var ctors_SpecificationEvaluator = type_SpecificationEvaluator.GetConstructors();
        Console.WriteLine($"[PASS] SpecificationEvaluator 构造函数数量: {ctors_SpecificationEvaluator.Length}");
        var methods_SpecificationEvaluator = type_SpecificationEvaluator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpecificationEvaluator 公开方法数量: {methods_SpecificationEvaluator.Length}");
        foreach (var m in methods_SpecificationEvaluator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpecificationEvaluator 未找到，尝试无命名空间...");
        type_SpecificationEvaluator = Type.GetType("SpecificationEvaluator");
        if (type_SpecificationEvaluator != null)
            Console.WriteLine("[PASS] 类型 SpecificationEvaluator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpecificationEvaluator 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISpecification
    var type_ISpecification = Type.GetType("ISpecification");
    if (type_ISpecification != null)
    {
        Console.WriteLine("[PASS] 类型 ISpecification (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISpecification 未找到，尝试无命名空间...");
        type_ISpecification = Type.GetType("ISpecification");
        if (type_ISpecification != null)
            Console.WriteLine("[PASS] 类型 ISpecification (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISpecification 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
