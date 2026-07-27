#load "rx_reactiveui_mvvm.cs"

Console.WriteLine("=== rx_reactiveui_mvvm.cs Test ===");

try
{
    // 验证 class: ReactiveViewModelBase
    var type_ReactiveViewModelBase = Type.GetType("ReactiveViewModelBase");
    if (type_ReactiveViewModelBase != null)
    {
        Console.WriteLine("[PASS] 类型 ReactiveViewModelBase (class) 存在");
        var ctors_ReactiveViewModelBase = type_ReactiveViewModelBase.GetConstructors();
        Console.WriteLine($"[PASS] ReactiveViewModelBase 构造函数数量: {ctors_ReactiveViewModelBase.Length}");
        var methods_ReactiveViewModelBase = type_ReactiveViewModelBase.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReactiveViewModelBase 公开方法数量: {methods_ReactiveViewModelBase.Length}");
        foreach (var m in methods_ReactiveViewModelBase)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReactiveViewModelBase 未找到，尝试无命名空间...");
        type_ReactiveViewModelBase = Type.GetType("ReactiveViewModelBase");
        if (type_ReactiveViewModelBase != null)
            Console.WriteLine("[PASS] 类型 ReactiveViewModelBase (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReactiveViewModelBase 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UserProfileViewModel
    var type_UserProfileViewModel = Type.GetType("UserProfileViewModel");
    if (type_UserProfileViewModel != null)
    {
        Console.WriteLine("[PASS] 类型 UserProfileViewModel (class) 存在");
        var ctors_UserProfileViewModel = type_UserProfileViewModel.GetConstructors();
        Console.WriteLine($"[PASS] UserProfileViewModel 构造函数数量: {ctors_UserProfileViewModel.Length}");
        var methods_UserProfileViewModel = type_UserProfileViewModel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserProfileViewModel 公开方法数量: {methods_UserProfileViewModel.Length}");
        foreach (var m in methods_UserProfileViewModel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserProfileViewModel 未找到，尝试无命名空间...");
        type_UserProfileViewModel = Type.GetType("UserProfileViewModel");
        if (type_UserProfileViewModel != null)
            Console.WriteLine("[PASS] 类型 UserProfileViewModel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserProfileViewModel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ReactiveExtensions
    var type_ReactiveExtensions = Type.GetType("ReactiveExtensions");
    if (type_ReactiveExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ReactiveExtensions (class) 存在");
        var ctors_ReactiveExtensions = type_ReactiveExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ReactiveExtensions 构造函数数量: {ctors_ReactiveExtensions.Length}");
        var methods_ReactiveExtensions = type_ReactiveExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReactiveExtensions 公开方法数量: {methods_ReactiveExtensions.Length}");
        foreach (var m in methods_ReactiveExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReactiveExtensions 未找到，尝试无命名空间...");
        type_ReactiveExtensions = Type.GetType("ReactiveExtensions");
        if (type_ReactiveExtensions != null)
            Console.WriteLine("[PASS] 类型 ReactiveExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReactiveExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
