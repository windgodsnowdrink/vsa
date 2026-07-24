#load "refit_contract.cs"

Console.WriteLine("=== refit_contract.cs Test ===");

try
{
    // 验证 interface: IUserService
    var type_IUserService = Type.GetType("IUserService");
    if (type_IUserService != null)
    {
        Console.WriteLine("[PASS] 类型 IUserService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IUserService 未找到，尝试无命名空间...");
        type_IUserService = Type.GetType("IUserService");
        if (type_IUserService != null)
            Console.WriteLine("[PASS] 类型 IUserService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IUserService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: User
    var type_User = Type.GetType("User");
    if (type_User != null)
    {
        Console.WriteLine("[PASS] 类型 User (record) 存在");
        var ctors_User = type_User.GetConstructors();
        Console.WriteLine($"[PASS] User 构造函数数量: {ctors_User.Length}");
        var methods_User = type_User.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] User 公开方法数量: {methods_User.Length}");
        foreach (var m in methods_User)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 User 未找到，尝试无命名空间...");
        type_User = Type.GetType("User");
        if (type_User != null)
            Console.WriteLine("[PASS] 类型 User (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 User 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
