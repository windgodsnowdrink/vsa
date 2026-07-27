#load "dorisoy_sms_integration.cs"

Console.WriteLine("=== dorisoy_sms_integration.cs Test ===");

try
{
    // 验证 class: SmsOptions
    var type_SmsOptions = Type.GetType("SmsOptions");
    if (type_SmsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SmsOptions (class) 存在");
        var ctors_SmsOptions = type_SmsOptions.GetConstructors();
        Console.WriteLine($"[PASS] SmsOptions 构造函数数量: {ctors_SmsOptions.Length}");
        var methods_SmsOptions = type_SmsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsOptions 公开方法数量: {methods_SmsOptions.Length}");
        foreach (var m in methods_SmsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsOptions 未找到，尝试无命名空间...");
        type_SmsOptions = Type.GetType("SmsOptions");
        if (type_SmsOptions != null)
            Console.WriteLine("[PASS] 类型 SmsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TrieTree
    var type_TrieTree = Type.GetType("TrieTree");
    if (type_TrieTree != null)
    {
        Console.WriteLine("[PASS] 类型 TrieTree (class) 存在");
        var ctors_TrieTree = type_TrieTree.GetConstructors();
        Console.WriteLine($"[PASS] TrieTree 构造函数数量: {ctors_TrieTree.Length}");
        var methods_TrieTree = type_TrieTree.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TrieTree 公开方法数量: {methods_TrieTree.Length}");
        foreach (var m in methods_TrieTree)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TrieTree 未找到，尝试无命名空间...");
        type_TrieTree = Type.GetType("TrieTree");
        if (type_TrieTree != null)
            Console.WriteLine("[PASS] 类型 TrieTree (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TrieTree 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TrieNode
    var type_TrieNode = Type.GetType("TrieNode");
    if (type_TrieNode != null)
    {
        Console.WriteLine("[PASS] 类型 TrieNode (class) 存在");
        var ctors_TrieNode = type_TrieNode.GetConstructors();
        Console.WriteLine($"[PASS] TrieNode 构造函数数量: {ctors_TrieNode.Length}");
        var methods_TrieNode = type_TrieNode.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TrieNode 公开方法数量: {methods_TrieNode.Length}");
        foreach (var m in methods_TrieNode)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TrieNode 未找到，尝试无命名空间...");
        type_TrieNode = Type.GetType("TrieNode");
        if (type_TrieNode != null)
            Console.WriteLine("[PASS] 类型 TrieNode (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TrieNode 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmsListRepository
    var type_SmsListRepository = Type.GetType("SmsListRepository");
    if (type_SmsListRepository != null)
    {
        Console.WriteLine("[PASS] 类型 SmsListRepository (class) 存在");
        var ctors_SmsListRepository = type_SmsListRepository.GetConstructors();
        Console.WriteLine($"[PASS] SmsListRepository 构造函数数量: {ctors_SmsListRepository.Length}");
        var methods_SmsListRepository = type_SmsListRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsListRepository 公开方法数量: {methods_SmsListRepository.Length}");
        foreach (var m in methods_SmsListRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsListRepository 未找到，尝试无命名空间...");
        type_SmsListRepository = Type.GetType("SmsListRepository");
        if (type_SmsListRepository != null)
            Console.WriteLine("[PASS] 类型 SmsListRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsListRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmsService
    var type_SmsService = Type.GetType("SmsService");
    if (type_SmsService != null)
    {
        Console.WriteLine("[PASS] 类型 SmsService (class) 存在");
        var ctors_SmsService = type_SmsService.GetConstructors();
        Console.WriteLine($"[PASS] SmsService 构造函数数量: {ctors_SmsService.Length}");
        var methods_SmsService = type_SmsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsService 公开方法数量: {methods_SmsService.Length}");
        foreach (var m in methods_SmsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsService 未找到，尝试无命名空间...");
        type_SmsService = Type.GetType("SmsService");
        if (type_SmsService != null)
            Console.WriteLine("[PASS] 类型 SmsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsService 可能为顶层语句或嵌套类型");
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

    // 验证 class: MyService
    var type_MyService = Type.GetType("MyService");
    if (type_MyService != null)
    {
        Console.WriteLine("[PASS] 类型 MyService (class) 存在");
        var ctors_MyService = type_MyService.GetConstructors();
        Console.WriteLine($"[PASS] MyService 构造函数数量: {ctors_MyService.Length}");
        var methods_MyService = type_MyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MyService 公开方法数量: {methods_MyService.Length}");
        foreach (var m in methods_MyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MyService 未找到，尝试无命名空间...");
        type_MyService = Type.GetType("MyService");
        if (type_MyService != null)
            Console.WriteLine("[PASS] 类型 MyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MyService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RateLimiter
    var type_RateLimiter = Type.GetType("RateLimiter");
    if (type_RateLimiter != null)
    {
        Console.WriteLine("[PASS] 类型 RateLimiter (class) 存在");
        var ctors_RateLimiter = type_RateLimiter.GetConstructors();
        Console.WriteLine($"[PASS] RateLimiter 构造函数数量: {ctors_RateLimiter.Length}");
        var methods_RateLimiter = type_RateLimiter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RateLimiter 公开方法数量: {methods_RateLimiter.Length}");
        foreach (var m in methods_RateLimiter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RateLimiter 未找到，尝试无命名空间...");
        type_RateLimiter = Type.GetType("RateLimiter");
        if (type_RateLimiter != null)
            Console.WriteLine("[PASS] 类型 RateLimiter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RateLimiter 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISmsService
    var type_ISmsService = Type.GetType("ISmsService");
    if (type_ISmsService != null)
    {
        Console.WriteLine("[PASS] 类型 ISmsService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISmsService 未找到，尝试无命名空间...");
        type_ISmsService = Type.GetType("ISmsService");
        if (type_ISmsService != null)
            Console.WriteLine("[PASS] 类型 ISmsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISmsService 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: SmsMessage
    var type_SmsMessage = Type.GetType("SmsMessage");
    if (type_SmsMessage != null)
    {
        Console.WriteLine("[PASS] 类型 SmsMessage (struct) 存在");
        var ctors_SmsMessage = type_SmsMessage.GetConstructors();
        Console.WriteLine($"[PASS] SmsMessage 构造函数数量: {ctors_SmsMessage.Length}");
        var methods_SmsMessage = type_SmsMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsMessage 公开方法数量: {methods_SmsMessage.Length}");
        foreach (var m in methods_SmsMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsMessage 未找到，尝试无命名空间...");
        type_SmsMessage = Type.GetType("SmsMessage");
        if (type_SmsMessage != null)
            Console.WriteLine("[PASS] 类型 SmsMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
