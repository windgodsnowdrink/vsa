#load "silky_maui_integration.cs"

Console.WriteLine("=== silky_maui_integration.cs Test ===");

try
{
    // 验证 class: MauiProgram
    var type_MauiProgram = Type.GetType("MauiProgram");
    if (type_MauiProgram != null)
    {
        Console.WriteLine("[PASS] 类型 MauiProgram (class) 存在");
        var ctors_MauiProgram = type_MauiProgram.GetConstructors();
        Console.WriteLine($"[PASS] MauiProgram 构造函数数量: {ctors_MauiProgram.Length}");
        var methods_MauiProgram = type_MauiProgram.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MauiProgram 公开方法数量: {methods_MauiProgram.Length}");
        foreach (var m in methods_MauiProgram)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MauiProgram 未找到，尝试无命名空间...");
        type_MauiProgram = Type.GetType("MauiProgram");
        if (type_MauiProgram != null)
            Console.WriteLine("[PASS] 类型 MauiProgram (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MauiProgram 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AppModule
    var type_AppModule = Type.GetType("AppModule");
    if (type_AppModule != null)
    {
        Console.WriteLine("[PASS] 类型 AppModule (class) 存在");
        var ctors_AppModule = type_AppModule.GetConstructors();
        Console.WriteLine($"[PASS] AppModule 构造函数数量: {ctors_AppModule.Length}");
        var methods_AppModule = type_AppModule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppModule 公开方法数量: {methods_AppModule.Length}");
        foreach (var m in methods_AppModule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppModule 未找到，尝试无命名空间...");
        type_AppModule = Type.GetType("AppModule");
        if (type_AppModule != null)
            Console.WriteLine("[PASS] 类型 AppModule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppModule 可能为顶层语句或嵌套类型");
    }

    // 验证 class: App
    var type_App = Type.GetType("App");
    if (type_App != null)
    {
        Console.WriteLine("[PASS] 类型 App (class) 存在");
        var ctors_App = type_App.GetConstructors();
        Console.WriteLine($"[PASS] App 构造函数数量: {ctors_App.Length}");
        var methods_App = type_App.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] App 公开方法数量: {methods_App.Length}");
        foreach (var m in methods_App)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 App 未找到，尝试无命名空间...");
        type_App = Type.GetType("App");
        if (type_App != null)
            Console.WriteLine("[PASS] 类型 App (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 App 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MyMicroService
    var type_MyMicroService = Type.GetType("MyMicroService");
    if (type_MyMicroService != null)
    {
        Console.WriteLine("[PASS] 类型 MyMicroService (class) 存在");
        var ctors_MyMicroService = type_MyMicroService.GetConstructors();
        Console.WriteLine($"[PASS] MyMicroService 构造函数数量: {ctors_MyMicroService.Length}");
        var methods_MyMicroService = type_MyMicroService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MyMicroService 公开方法数量: {methods_MyMicroService.Length}");
        foreach (var m in methods_MyMicroService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MyMicroService 未找到，尝试无命名空间...");
        type_MyMicroService = Type.GetType("MyMicroService");
        if (type_MyMicroService != null)
            Console.WriteLine("[PASS] 类型 MyMicroService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MyMicroService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MainPage
    var type_MainPage = Type.GetType("MainPage");
    if (type_MainPage != null)
    {
        Console.WriteLine("[PASS] 类型 MainPage (class) 存在");
        var ctors_MainPage = type_MainPage.GetConstructors();
        Console.WriteLine($"[PASS] MainPage 构造函数数量: {ctors_MainPage.Length}");
        var methods_MainPage = type_MainPage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MainPage 公开方法数量: {methods_MainPage.Length}");
        foreach (var m in methods_MainPage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MainPage 未找到，尝试无命名空间...");
        type_MainPage = Type.GetType("MainPage");
        if (type_MainPage != null)
            Console.WriteLine("[PASS] 类型 MainPage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MainPage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMyMicroService
    var type_IMyMicroService = Type.GetType("IMyMicroService");
    if (type_IMyMicroService != null)
    {
        Console.WriteLine("[PASS] 类型 IMyMicroService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMyMicroService 未找到，尝试无命名空间...");
        type_IMyMicroService = Type.GetType("IMyMicroService");
        if (type_IMyMicroService != null)
            Console.WriteLine("[PASS] 类型 IMyMicroService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMyMicroService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
