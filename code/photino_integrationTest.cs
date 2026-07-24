#load "photino_integration.cs"

Console.WriteLine("=== photino_integration.cs Test ===");

try
{
    // 验证 class: PhotinoOptions
    var type_PhotinoOptions = Type.GetType("PhotinoOptions");
    if (type_PhotinoOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PhotinoOptions (class) 存在");
        var ctors_PhotinoOptions = type_PhotinoOptions.GetConstructors();
        Console.WriteLine($"[PASS] PhotinoOptions 构造函数数量: {ctors_PhotinoOptions.Length}");
        var methods_PhotinoOptions = type_PhotinoOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PhotinoOptions 公开方法数量: {methods_PhotinoOptions.Length}");
        foreach (var m in methods_PhotinoOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PhotinoOptions 未找到，尝试无命名空间...");
        type_PhotinoOptions = Type.GetType("PhotinoOptions");
        if (type_PhotinoOptions != null)
            Console.WriteLine("[PASS] 类型 PhotinoOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PhotinoOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PhotinoService
    var type_PhotinoService = Type.GetType("PhotinoService");
    if (type_PhotinoService != null)
    {
        Console.WriteLine("[PASS] 类型 PhotinoService (class) 存在");
        var ctors_PhotinoService = type_PhotinoService.GetConstructors();
        Console.WriteLine($"[PASS] PhotinoService 构造函数数量: {ctors_PhotinoService.Length}");
        var methods_PhotinoService = type_PhotinoService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PhotinoService 公开方法数量: {methods_PhotinoService.Length}");
        foreach (var m in methods_PhotinoService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PhotinoService 未找到，尝试无命名空间...");
        type_PhotinoService = Type.GetType("PhotinoService");
        if (type_PhotinoService != null)
            Console.WriteLine("[PASS] 类型 PhotinoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PhotinoService 可能为顶层语句或嵌套类型");
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

    // 验证 class: MainViewModel
    var type_MainViewModel = Type.GetType("MainViewModel");
    if (type_MainViewModel != null)
    {
        Console.WriteLine("[PASS] 类型 MainViewModel (class) 存在");
        var ctors_MainViewModel = type_MainViewModel.GetConstructors();
        Console.WriteLine($"[PASS] MainViewModel 构造函数数量: {ctors_MainViewModel.Length}");
        var methods_MainViewModel = type_MainViewModel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MainViewModel 公开方法数量: {methods_MainViewModel.Length}");
        foreach (var m in methods_MainViewModel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MainViewModel 未找到，尝试无命名空间...");
        type_MainViewModel = Type.GetType("MainViewModel");
        if (type_MainViewModel != null)
            Console.WriteLine("[PASS] 类型 MainViewModel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MainViewModel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LocalStorageContext
    var type_LocalStorageContext = Type.GetType("LocalStorageContext");
    if (type_LocalStorageContext != null)
    {
        Console.WriteLine("[PASS] 类型 LocalStorageContext (class) 存在");
        var ctors_LocalStorageContext = type_LocalStorageContext.GetConstructors();
        Console.WriteLine($"[PASS] LocalStorageContext 构造函数数量: {ctors_LocalStorageContext.Length}");
        var methods_LocalStorageContext = type_LocalStorageContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LocalStorageContext 公开方法数量: {methods_LocalStorageContext.Length}");
        foreach (var m in methods_LocalStorageContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LocalStorageContext 未找到，尝试无命名空间...");
        type_LocalStorageContext = Type.GetType("LocalStorageContext");
        if (type_LocalStorageContext != null)
            Console.WriteLine("[PASS] 类型 LocalStorageContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LocalStorageContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LocalStorageItem
    var type_LocalStorageItem = Type.GetType("LocalStorageItem");
    if (type_LocalStorageItem != null)
    {
        Console.WriteLine("[PASS] 类型 LocalStorageItem (class) 存在");
        var ctors_LocalStorageItem = type_LocalStorageItem.GetConstructors();
        Console.WriteLine($"[PASS] LocalStorageItem 构造函数数量: {ctors_LocalStorageItem.Length}");
        var methods_LocalStorageItem = type_LocalStorageItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LocalStorageItem 公开方法数量: {methods_LocalStorageItem.Length}");
        foreach (var m in methods_LocalStorageItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LocalStorageItem 未找到，尝试无命名空间...");
        type_LocalStorageItem = Type.GetType("LocalStorageItem");
        if (type_LocalStorageItem != null)
            Console.WriteLine("[PASS] 类型 LocalStorageItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LocalStorageItem 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IPhotinoService
    var type_IPhotinoService = Type.GetType("IPhotinoService");
    if (type_IPhotinoService != null)
    {
        Console.WriteLine("[PASS] 类型 IPhotinoService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPhotinoService 未找到，尝试无命名空间...");
        type_IPhotinoService = Type.GetType("IPhotinoService");
        if (type_IPhotinoService != null)
            Console.WriteLine("[PASS] 类型 IPhotinoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPhotinoService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
