#load "hateoas_integration.cs"

Console.WriteLine("=== hateoas_integration.cs Test ===");

try
{
    // 验证 class: Resource
    var type_Resource = Type.GetType("Resource");
    if (type_Resource != null)
    {
        Console.WriteLine("[PASS] 类型 Resource (class) 存在");
        var ctors_Resource = type_Resource.GetConstructors();
        Console.WriteLine($"[PASS] Resource 构造函数数量: {ctors_Resource.Length}");
        var methods_Resource = type_Resource.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Resource 公开方法数量: {methods_Resource.Length}");
        foreach (var m in methods_Resource)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Resource 未找到，尝试无命名空间...");
        type_Resource = Type.GetType("Resource");
        if (type_Resource != null)
            Console.WriteLine("[PASS] 类型 Resource (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Resource 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProductsController
    var type_ProductsController = Type.GetType("ProductsController");
    if (type_ProductsController != null)
    {
        Console.WriteLine("[PASS] 类型 ProductsController (class) 存在");
        var ctors_ProductsController = type_ProductsController.GetConstructors();
        Console.WriteLine($"[PASS] ProductsController 构造函数数量: {ctors_ProductsController.Length}");
        var methods_ProductsController = type_ProductsController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductsController 公开方法数量: {methods_ProductsController.Length}");
        foreach (var m in methods_ProductsController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductsController 未找到，尝试无命名空间...");
        type_ProductsController = Type.GetType("ProductsController");
        if (type_ProductsController != null)
            Console.WriteLine("[PASS] 类型 ProductsController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductsController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Link
    var type_Link = Type.GetType("Link");
    if (type_Link != null)
    {
        Console.WriteLine("[PASS] 类型 Link (record) 存在");
        var ctors_Link = type_Link.GetConstructors();
        Console.WriteLine($"[PASS] Link 构造函数数量: {ctors_Link.Length}");
        var methods_Link = type_Link.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Link 公开方法数量: {methods_Link.Length}");
        foreach (var m in methods_Link)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Link 未找到，尝试无命名空间...");
        type_Link = Type.GetType("Link");
        if (type_Link != null)
            Console.WriteLine("[PASS] 类型 Link (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Link 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Product
    var type_Product = Type.GetType("Product");
    if (type_Product != null)
    {
        Console.WriteLine("[PASS] 类型 Product (record) 存在");
        var ctors_Product = type_Product.GetConstructors();
        Console.WriteLine($"[PASS] Product 构造函数数量: {ctors_Product.Length}");
        var methods_Product = type_Product.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Product 公开方法数量: {methods_Product.Length}");
        foreach (var m in methods_Product)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Product 未找到，尝试无命名空间...");
        type_Product = Type.GetType("Product");
        if (type_Product != null)
            Console.WriteLine("[PASS] 类型 Product (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Product 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
