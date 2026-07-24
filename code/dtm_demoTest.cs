#load "dtm_demo.cs"

Console.WriteLine("=== dtm_demo.cs Test ===");

try
{
    // 验证 class: TransferDbContext
    var type_TransferDbContext = Type.GetType("TransferDbContext");
    if (type_TransferDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 TransferDbContext (class) 存在");
        var ctors_TransferDbContext = type_TransferDbContext.GetConstructors();
        Console.WriteLine($"[PASS] TransferDbContext 构造函数数量: {ctors_TransferDbContext.Length}");
        var methods_TransferDbContext = type_TransferDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TransferDbContext 公开方法数量: {methods_TransferDbContext.Length}");
        foreach (var m in methods_TransferDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TransferDbContext 未找到，尝试无命名空间...");
        type_TransferDbContext = Type.GetType("TransferDbContext");
        if (type_TransferDbContext != null)
            Console.WriteLine("[PASS] 类型 TransferDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TransferDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Account
    var type_Account = Type.GetType("Account");
    if (type_Account != null)
    {
        Console.WriteLine("[PASS] 类型 Account (class) 存在");
        var ctors_Account = type_Account.GetConstructors();
        Console.WriteLine($"[PASS] Account 构造函数数量: {ctors_Account.Length}");
        var methods_Account = type_Account.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Account 公开方法数量: {methods_Account.Length}");
        foreach (var m in methods_Account)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Account 未找到，尝试无命名空间...");
        type_Account = Type.GetType("Account");
        if (type_Account != null)
            Console.WriteLine("[PASS] 类型 Account (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Account 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TransferController
    var type_TransferController = Type.GetType("TransferController");
    if (type_TransferController != null)
    {
        Console.WriteLine("[PASS] 类型 TransferController (class) 存在");
        var ctors_TransferController = type_TransferController.GetConstructors();
        Console.WriteLine($"[PASS] TransferController 构造函数数量: {ctors_TransferController.Length}");
        var methods_TransferController = type_TransferController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TransferController 公开方法数量: {methods_TransferController.Length}");
        foreach (var m in methods_TransferController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TransferController 未找到，尝试无命名空间...");
        type_TransferController = Type.GetType("TransferController");
        if (type_TransferController != null)
            Console.WriteLine("[PASS] 类型 TransferController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TransferController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TransferRequest
    var type_TransferRequest = Type.GetType("TransferRequest");
    if (type_TransferRequest != null)
    {
        Console.WriteLine("[PASS] 类型 TransferRequest (class) 存在");
        var ctors_TransferRequest = type_TransferRequest.GetConstructors();
        Console.WriteLine($"[PASS] TransferRequest 构造函数数量: {ctors_TransferRequest.Length}");
        var methods_TransferRequest = type_TransferRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TransferRequest 公开方法数量: {methods_TransferRequest.Length}");
        foreach (var m in methods_TransferRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TransferRequest 未找到，尝试无命名空间...");
        type_TransferRequest = Type.GetType("TransferRequest");
        if (type_TransferRequest != null)
            Console.WriteLine("[PASS] 类型 TransferRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TransferRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
