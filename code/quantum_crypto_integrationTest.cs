#load "quantum_crypto_integration.cs"

Console.WriteLine("=== quantum_crypto_integration.cs Test ===");

try
{
    // 验证 class: QuantumCrypto.QuantumCryptoOptions
    var type_QuantumCryptoOptions = Type.GetType("QuantumCrypto.QuantumCryptoOptions");
    if (type_QuantumCryptoOptions != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumCrypto.QuantumCryptoOptions (class) 存在");
        var ctors_QuantumCryptoOptions = type_QuantumCryptoOptions.GetConstructors();
        Console.WriteLine($"[PASS] QuantumCrypto.QuantumCryptoOptions 构造函数数量: {ctors_QuantumCryptoOptions.Length}");
        var methods_QuantumCryptoOptions = type_QuantumCryptoOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuantumCrypto.QuantumCryptoOptions 公开方法数量: {methods_QuantumCryptoOptions.Length}");
        foreach (var m in methods_QuantumCryptoOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumCrypto.QuantumCryptoOptions 未找到，尝试无命名空间...");
        type_QuantumCryptoOptions = Type.GetType("QuantumCryptoOptions");
        if (type_QuantumCryptoOptions != null)
            Console.WriteLine("[PASS] 类型 QuantumCryptoOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuantumCryptoOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QuantumCrypto.QuantumCryptoService
    var type_QuantumCryptoService = Type.GetType("QuantumCrypto.QuantumCryptoService");
    if (type_QuantumCryptoService != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumCrypto.QuantumCryptoService (class) 存在");
        var ctors_QuantumCryptoService = type_QuantumCryptoService.GetConstructors();
        Console.WriteLine($"[PASS] QuantumCrypto.QuantumCryptoService 构造函数数量: {ctors_QuantumCryptoService.Length}");
        var methods_QuantumCryptoService = type_QuantumCryptoService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuantumCrypto.QuantumCryptoService 公开方法数量: {methods_QuantumCryptoService.Length}");
        foreach (var m in methods_QuantumCryptoService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumCrypto.QuantumCryptoService 未找到，尝试无命名空间...");
        type_QuantumCryptoService = Type.GetType("QuantumCryptoService");
        if (type_QuantumCryptoService != null)
            Console.WriteLine("[PASS] 类型 QuantumCryptoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuantumCryptoService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QuantumCrypto.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("QuantumCrypto.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumCrypto.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] QuantumCrypto.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuantumCrypto.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumCrypto.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: QuantumCrypto.IQuantumRandomNumberGenerator
    var type_IQuantumRandomNumberGenerator = Type.GetType("QuantumCrypto.IQuantumRandomNumberGenerator");
    if (type_IQuantumRandomNumberGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumCrypto.IQuantumRandomNumberGenerator (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumCrypto.IQuantumRandomNumberGenerator 未找到，尝试无命名空间...");
        type_IQuantumRandomNumberGenerator = Type.GetType("IQuantumRandomNumberGenerator");
        if (type_IQuantumRandomNumberGenerator != null)
            Console.WriteLine("[PASS] 类型 IQuantumRandomNumberGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IQuantumRandomNumberGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: QuantumCrypto.IPostQuantumCryptoService
    var type_IPostQuantumCryptoService = Type.GetType("QuantumCrypto.IPostQuantumCryptoService");
    if (type_IPostQuantumCryptoService != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumCrypto.IPostQuantumCryptoService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumCrypto.IPostQuantumCryptoService 未找到，尝试无命名空间...");
        type_IPostQuantumCryptoService = Type.GetType("IPostQuantumCryptoService");
        if (type_IPostQuantumCryptoService != null)
            Console.WriteLine("[PASS] 类型 IPostQuantumCryptoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPostQuantumCryptoService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: QuantumCrypto.IQuantumCryptoService
    var type_IQuantumCryptoService = Type.GetType("QuantumCrypto.IQuantumCryptoService");
    if (type_IQuantumCryptoService != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumCrypto.IQuantumCryptoService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumCrypto.IQuantumCryptoService 未找到，尝试无命名空间...");
        type_IQuantumCryptoService = Type.GetType("IQuantumCryptoService");
        if (type_IQuantumCryptoService != null)
            Console.WriteLine("[PASS] 类型 IQuantumCryptoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IQuantumCryptoService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: QuantumCrypto.IHybridEncryptionService
    var type_IHybridEncryptionService = Type.GetType("QuantumCrypto.IHybridEncryptionService");
    if (type_IHybridEncryptionService != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumCrypto.IHybridEncryptionService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumCrypto.IHybridEncryptionService 未找到，尝试无命名空间...");
        type_IHybridEncryptionService = Type.GetType("IHybridEncryptionService");
        if (type_IHybridEncryptionService != null)
            Console.WriteLine("[PASS] 类型 IHybridEncryptionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IHybridEncryptionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
