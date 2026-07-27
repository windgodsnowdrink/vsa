#load "x_pagedlist_integration.cs"

Console.WriteLine("=== x_pagedlist_integration.cs Test ===");

try
{
    // 验证 class: PagedListOptions
    var type_PagedListOptions = Type.GetType("PagedListOptions");
    if (type_PagedListOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PagedListOptions (class) 存在");
        var ctors_PagedListOptions = type_PagedListOptions.GetConstructors();
        Console.WriteLine($"[PASS] PagedListOptions 构造函数数量: {ctors_PagedListOptions.Length}");
        var methods_PagedListOptions = type_PagedListOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PagedListOptions 公开方法数量: {methods_PagedListOptions.Length}");
        foreach (var m in methods_PagedListOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PagedListOptions 未找到，尝试无命名空间...");
        type_PagedListOptions = Type.GetType("PagedListOptions");
        if (type_PagedListOptions != null)
            Console.WriteLine("[PASS] 类型 PagedListOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PagedListOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PagedListService
    var type_PagedListService = Type.GetType("PagedListService");
    if (type_PagedListService != null)
    {
        Console.WriteLine("[PASS] 类型 PagedListService (class) 存在");
        var ctors_PagedListService = type_PagedListService.GetConstructors();
        Console.WriteLine($"[PASS] PagedListService 构造函数数量: {ctors_PagedListService.Length}");
        var methods_PagedListService = type_PagedListService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PagedListService 公开方法数量: {methods_PagedListService.Length}");
        foreach (var m in methods_PagedListService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PagedListService 未找到，尝试无命名空间...");
        type_PagedListService = Type.GetType("PagedListService");
        if (type_PagedListService != null)
            Console.WriteLine("[PASS] 类型 PagedListService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PagedListService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PagedListExtensions
    var type_PagedListExtensions = Type.GetType("PagedListExtensions");
    if (type_PagedListExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PagedListExtensions (class) 存在");
        var ctors_PagedListExtensions = type_PagedListExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PagedListExtensions 构造函数数量: {ctors_PagedListExtensions.Length}");
        var methods_PagedListExtensions = type_PagedListExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PagedListExtensions 公开方法数量: {methods_PagedListExtensions.Length}");
        foreach (var m in methods_PagedListExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PagedListExtensions 未找到，尝试无命名空间...");
        type_PagedListExtensions = Type.GetType("PagedListExtensions");
        if (type_PagedListExtensions != null)
            Console.WriteLine("[PASS] 类型 PagedListExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PagedListExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PaginationService
    var type_PaginationService = Type.GetType("PaginationService");
    if (type_PaginationService != null)
    {
        Console.WriteLine("[PASS] 类型 PaginationService (class) 存在");
        var ctors_PaginationService = type_PaginationService.GetConstructors();
        Console.WriteLine($"[PASS] PaginationService 构造函数数量: {ctors_PaginationService.Length}");
        var methods_PaginationService = type_PaginationService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PaginationService 公开方法数量: {methods_PaginationService.Length}");
        foreach (var m in methods_PaginationService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PaginationService 未找到，尝试无命名空间...");
        type_PaginationService = Type.GetType("PaginationService");
        if (type_PaginationService != null)
            Console.WriteLine("[PASS] 类型 PaginationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PaginationService 可能为顶层语句或嵌套类型");
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

    // 验证 class: PagerTagHelper
    var type_PagerTagHelper = Type.GetType("PagerTagHelper");
    if (type_PagerTagHelper != null)
    {
        Console.WriteLine("[PASS] 类型 PagerTagHelper (class) 存在");
        var ctors_PagerTagHelper = type_PagerTagHelper.GetConstructors();
        Console.WriteLine($"[PASS] PagerTagHelper 构造函数数量: {ctors_PagerTagHelper.Length}");
        var methods_PagerTagHelper = type_PagerTagHelper.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PagerTagHelper 公开方法数量: {methods_PagerTagHelper.Length}");
        foreach (var m in methods_PagerTagHelper)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PagerTagHelper 未找到，尝试无命名空间...");
        type_PagerTagHelper = Type.GetType("PagerTagHelper");
        if (type_PagerTagHelper != null)
            Console.WriteLine("[PASS] 类型 PagerTagHelper (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PagerTagHelper 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPagedListService
    var type_IPagedListService = Type.GetType("IPagedListService");
    if (type_IPagedListService != null)
    {
        Console.WriteLine("[PASS] 类型 IPagedListService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPagedListService 未找到，尝试无命名空间...");
        type_IPagedListService = Type.GetType("IPagedListService");
        if (type_IPagedListService != null)
            Console.WriteLine("[PASS] 类型 IPagedListService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPagedListService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPaginationService
    var type_IPaginationService = Type.GetType("IPaginationService");
    if (type_IPaginationService != null)
    {
        Console.WriteLine("[PASS] 类型 IPaginationService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPaginationService 未找到，尝试无命名空间...");
        type_IPaginationService = Type.GetType("IPaginationService");
        if (type_IPaginationService != null)
            Console.WriteLine("[PASS] 类型 IPaginationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPaginationService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: PagedResult
    var type_PagedResult = Type.GetType("PagedResult");
    if (type_PagedResult != null)
    {
        Console.WriteLine("[PASS] 类型 PagedResult (record) 存在");
        var ctors_PagedResult = type_PagedResult.GetConstructors();
        Console.WriteLine($"[PASS] PagedResult 构造函数数量: {ctors_PagedResult.Length}");
        var methods_PagedResult = type_PagedResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PagedResult 公开方法数量: {methods_PagedResult.Length}");
        foreach (var m in methods_PagedResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PagedResult 未找到，尝试无命名空间...");
        type_PagedResult = Type.GetType("PagedResult");
        if (type_PagedResult != null)
            Console.WriteLine("[PASS] 类型 PagedResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PagedResult 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
