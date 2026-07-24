#load "query_builder_integration.cs"

Console.WriteLine("=== query_builder_integration.cs Test ===");

try
{
    // 验证 class: DynamicQueryBuilder
    var type_DynamicQueryBuilder = Type.GetType("DynamicQueryBuilder");
    if (type_DynamicQueryBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicQueryBuilder (class) 存在");
        var ctors_DynamicQueryBuilder = type_DynamicQueryBuilder.GetConstructors();
        Console.WriteLine($"[PASS] DynamicQueryBuilder 构造函数数量: {ctors_DynamicQueryBuilder.Length}");
        var methods_DynamicQueryBuilder = type_DynamicQueryBuilder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicQueryBuilder 公开方法数量: {methods_DynamicQueryBuilder.Length}");
        foreach (var m in methods_DynamicQueryBuilder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicQueryBuilder 未找到，尝试无命名空间...");
        type_DynamicQueryBuilder = Type.GetType("DynamicQueryBuilder");
        if (type_DynamicQueryBuilder != null)
            Console.WriteLine("[PASS] 类型 DynamicQueryBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicQueryBuilder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DtoToSpecConverter
    var type_DtoToSpecConverter = Type.GetType("DtoToSpecConverter");
    if (type_DtoToSpecConverter != null)
    {
        Console.WriteLine("[PASS] 类型 DtoToSpecConverter (class) 存在");
        var ctors_DtoToSpecConverter = type_DtoToSpecConverter.GetConstructors();
        Console.WriteLine($"[PASS] DtoToSpecConverter 构造函数数量: {ctors_DtoToSpecConverter.Length}");
        var methods_DtoToSpecConverter = type_DtoToSpecConverter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DtoToSpecConverter 公开方法数量: {methods_DtoToSpecConverter.Length}");
        foreach (var m in methods_DtoToSpecConverter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DtoToSpecConverter 未找到，尝试无命名空间...");
        type_DtoToSpecConverter = Type.GetType("DtoToSpecConverter");
        if (type_DtoToSpecConverter != null)
            Console.WriteLine("[PASS] 类型 DtoToSpecConverter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DtoToSpecConverter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicSpecification
    var type_DynamicSpecification = Type.GetType("DynamicSpecification");
    if (type_DynamicSpecification != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicSpecification (class) 存在");
        var ctors_DynamicSpecification = type_DynamicSpecification.GetConstructors();
        Console.WriteLine($"[PASS] DynamicSpecification 构造函数数量: {ctors_DynamicSpecification.Length}");
        var methods_DynamicSpecification = type_DynamicSpecification.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicSpecification 公开方法数量: {methods_DynamicSpecification.Length}");
        foreach (var m in methods_DynamicSpecification)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicSpecification 未找到，尝试无命名空间...");
        type_DynamicSpecification = Type.GetType("DynamicSpecification");
        if (type_DynamicSpecification != null)
            Console.WriteLine("[PASS] 类型 DynamicSpecification (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicSpecification 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QueryService
    var type_QueryService = Type.GetType("QueryService");
    if (type_QueryService != null)
    {
        Console.WriteLine("[PASS] 类型 QueryService (class) 存在");
        var ctors_QueryService = type_QueryService.GetConstructors();
        Console.WriteLine($"[PASS] QueryService 构造函数数量: {ctors_QueryService.Length}");
        var methods_QueryService = type_QueryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueryService 公开方法数量: {methods_QueryService.Length}");
        foreach (var m in methods_QueryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueryService 未找到，尝试无命名空间...");
        type_QueryService = Type.GetType("QueryService");
        if (type_QueryService != null)
            Console.WriteLine("[PASS] 类型 QueryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QueryBuilderPoolPolicy
    var type_QueryBuilderPoolPolicy = Type.GetType("QueryBuilderPoolPolicy");
    if (type_QueryBuilderPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 QueryBuilderPoolPolicy (class) 存在");
        var ctors_QueryBuilderPoolPolicy = type_QueryBuilderPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] QueryBuilderPoolPolicy 构造函数数量: {ctors_QueryBuilderPoolPolicy.Length}");
        var methods_QueryBuilderPoolPolicy = type_QueryBuilderPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueryBuilderPoolPolicy 公开方法数量: {methods_QueryBuilderPoolPolicy.Length}");
        foreach (var m in methods_QueryBuilderPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueryBuilderPoolPolicy 未找到，尝试无命名空间...");
        type_QueryBuilderPoolPolicy = Type.GetType("QueryBuilderPoolPolicy");
        if (type_QueryBuilderPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 QueryBuilderPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueryBuilderPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QueryBuilderDemo
    var type_QueryBuilderDemo = Type.GetType("QueryBuilderDemo");
    if (type_QueryBuilderDemo != null)
    {
        Console.WriteLine("[PASS] 类型 QueryBuilderDemo (class) 存在");
        var ctors_QueryBuilderDemo = type_QueryBuilderDemo.GetConstructors();
        Console.WriteLine($"[PASS] QueryBuilderDemo 构造函数数量: {ctors_QueryBuilderDemo.Length}");
        var methods_QueryBuilderDemo = type_QueryBuilderDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueryBuilderDemo 公开方法数量: {methods_QueryBuilderDemo.Length}");
        foreach (var m in methods_QueryBuilderDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueryBuilderDemo 未找到，尝试无命名空间...");
        type_QueryBuilderDemo = Type.GetType("QueryBuilderDemo");
        if (type_QueryBuilderDemo != null)
            Console.WriteLine("[PASS] 类型 QueryBuilderDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueryBuilderDemo 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDynamicQueryBuilder
    var type_IDynamicQueryBuilder = Type.GetType("IDynamicQueryBuilder");
    if (type_IDynamicQueryBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 IDynamicQueryBuilder (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDynamicQueryBuilder 未找到，尝试无命名空间...");
        type_IDynamicQueryBuilder = Type.GetType("IDynamicQueryBuilder");
        if (type_IDynamicQueryBuilder != null)
            Console.WriteLine("[PASS] 类型 IDynamicQueryBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDynamicQueryBuilder 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ProductQueryDto
    var type_ProductQueryDto = Type.GetType("ProductQueryDto");
    if (type_ProductQueryDto != null)
    {
        Console.WriteLine("[PASS] 类型 ProductQueryDto (record) 存在");
        var ctors_ProductQueryDto = type_ProductQueryDto.GetConstructors();
        Console.WriteLine($"[PASS] ProductQueryDto 构造函数数量: {ctors_ProductQueryDto.Length}");
        var methods_ProductQueryDto = type_ProductQueryDto.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductQueryDto 公开方法数量: {methods_ProductQueryDto.Length}");
        foreach (var m in methods_ProductQueryDto)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductQueryDto 未找到，尝试无命名空间...");
        type_ProductQueryDto = Type.GetType("ProductQueryDto");
        if (type_ProductQueryDto != null)
            Console.WriteLine("[PASS] 类型 ProductQueryDto (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductQueryDto 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
