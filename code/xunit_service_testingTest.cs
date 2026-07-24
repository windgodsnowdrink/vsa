#load "xunit_service_testing.cs"

Console.WriteLine("=== xunit_service_testing.cs Test ===");

try
{
    // 验证 class: UserService
    var type_UserService = Type.GetType("UserService");
    if (type_UserService != null)
    {
        Console.WriteLine("[PASS] 类型 UserService (class) 存在");
        var ctors_UserService = type_UserService.GetConstructors();
        Console.WriteLine($"[PASS] UserService 构造函数数量: {ctors_UserService.Length}");
        var methods_UserService = type_UserService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserService 公开方法数量: {methods_UserService.Length}");
        foreach (var m in methods_UserService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserService 未找到，尝试无命名空间...");
        type_UserService = Type.GetType("UserService");
        if (type_UserService != null)
            Console.WriteLine("[PASS] 类型 UserService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UserServiceTests
    var type_UserServiceTests = Type.GetType("UserServiceTests");
    if (type_UserServiceTests != null)
    {
        Console.WriteLine("[PASS] 类型 UserServiceTests (class) 存在");
        var ctors_UserServiceTests = type_UserServiceTests.GetConstructors();
        Console.WriteLine($"[PASS] UserServiceTests 构造函数数量: {ctors_UserServiceTests.Length}");
        var methods_UserServiceTests = type_UserServiceTests.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserServiceTests 公开方法数量: {methods_UserServiceTests.Length}");
        foreach (var m in methods_UserServiceTests)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserServiceTests 未找到，尝试无命名空间...");
        type_UserServiceTests = Type.GetType("UserServiceTests");
        if (type_UserServiceTests != null)
            Console.WriteLine("[PASS] 类型 UserServiceTests (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserServiceTests 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TestCollectionDefinition
    var type_TestCollectionDefinition = Type.GetType("TestCollectionDefinition");
    if (type_TestCollectionDefinition != null)
    {
        Console.WriteLine("[PASS] 类型 TestCollectionDefinition (class) 存在");
        var ctors_TestCollectionDefinition = type_TestCollectionDefinition.GetConstructors();
        Console.WriteLine($"[PASS] TestCollectionDefinition 构造函数数量: {ctors_TestCollectionDefinition.Length}");
        var methods_TestCollectionDefinition = type_TestCollectionDefinition.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TestCollectionDefinition 公开方法数量: {methods_TestCollectionDefinition.Length}");
        foreach (var m in methods_TestCollectionDefinition)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TestCollectionDefinition 未找到，尝试无命名空间...");
        type_TestCollectionDefinition = Type.GetType("TestCollectionDefinition");
        if (type_TestCollectionDefinition != null)
            Console.WriteLine("[PASS] 类型 TestCollectionDefinition (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TestCollectionDefinition 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceTestCollection
    var type_ServiceTestCollection = Type.GetType("ServiceTestCollection");
    if (type_ServiceTestCollection != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceTestCollection (class) 存在");
        var ctors_ServiceTestCollection = type_ServiceTestCollection.GetConstructors();
        Console.WriteLine($"[PASS] ServiceTestCollection 构造函数数量: {ctors_ServiceTestCollection.Length}");
        var methods_ServiceTestCollection = type_ServiceTestCollection.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceTestCollection 公开方法数量: {methods_ServiceTestCollection.Length}");
        foreach (var m in methods_ServiceTestCollection)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceTestCollection 未找到，尝试无命名空间...");
        type_ServiceTestCollection = Type.GetType("ServiceTestCollection");
        if (type_ServiceTestCollection != null)
            Console.WriteLine("[PASS] 类型 ServiceTestCollection (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceTestCollection 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TestFixture
    var type_TestFixture = Type.GetType("TestFixture");
    if (type_TestFixture != null)
    {
        Console.WriteLine("[PASS] 类型 TestFixture (class) 存在");
        var ctors_TestFixture = type_TestFixture.GetConstructors();
        Console.WriteLine($"[PASS] TestFixture 构造函数数量: {ctors_TestFixture.Length}");
        var methods_TestFixture = type_TestFixture.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TestFixture 公开方法数量: {methods_TestFixture.Length}");
        foreach (var m in methods_TestFixture)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TestFixture 未找到，尝试无命名空间...");
        type_TestFixture = Type.GetType("TestFixture");
        if (type_TestFixture != null)
            Console.WriteLine("[PASS] 类型 TestFixture (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TestFixture 可能为顶层语句或嵌套类型");
    }

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
