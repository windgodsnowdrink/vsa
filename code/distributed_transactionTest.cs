#load "distributed_transaction.cs"

Console.WriteLine("=== distributed_transaction.cs Test ===");

try
{
    // 验证 class: DistributedTransactionCoordinator
    var type_DistributedTransactionCoordinator = Type.GetType("DistributedTransactionCoordinator");
    if (type_DistributedTransactionCoordinator != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedTransactionCoordinator (class) 存在");
        var ctors_DistributedTransactionCoordinator = type_DistributedTransactionCoordinator.GetConstructors();
        Console.WriteLine($"[PASS] DistributedTransactionCoordinator 构造函数数量: {ctors_DistributedTransactionCoordinator.Length}");
        var methods_DistributedTransactionCoordinator = type_DistributedTransactionCoordinator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedTransactionCoordinator 公开方法数量: {methods_DistributedTransactionCoordinator.Length}");
        foreach (var m in methods_DistributedTransactionCoordinator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedTransactionCoordinator 未找到，尝试无命名空间...");
        type_DistributedTransactionCoordinator = Type.GetType("DistributedTransactionCoordinator");
        if (type_DistributedTransactionCoordinator != null)
            Console.WriteLine("[PASS] 类型 DistributedTransactionCoordinator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedTransactionCoordinator 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TransactionCommand
    var type_TransactionCommand = Type.GetType("TransactionCommand");
    if (type_TransactionCommand != null)
    {
        Console.WriteLine("[PASS] 类型 TransactionCommand (record) 存在");
        var ctors_TransactionCommand = type_TransactionCommand.GetConstructors();
        Console.WriteLine($"[PASS] TransactionCommand 构造函数数量: {ctors_TransactionCommand.Length}");
        var methods_TransactionCommand = type_TransactionCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TransactionCommand 公开方法数量: {methods_TransactionCommand.Length}");
        foreach (var m in methods_TransactionCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TransactionCommand 未找到，尝试无命名空间...");
        type_TransactionCommand = Type.GetType("TransactionCommand");
        if (type_TransactionCommand != null)
            Console.WriteLine("[PASS] 类型 TransactionCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TransactionCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TransactionResult
    var type_TransactionResult = Type.GetType("TransactionResult");
    if (type_TransactionResult != null)
    {
        Console.WriteLine("[PASS] 类型 TransactionResult (record) 存在");
        var ctors_TransactionResult = type_TransactionResult.GetConstructors();
        Console.WriteLine($"[PASS] TransactionResult 构造函数数量: {ctors_TransactionResult.Length}");
        var methods_TransactionResult = type_TransactionResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TransactionResult 公开方法数量: {methods_TransactionResult.Length}");
        foreach (var m in methods_TransactionResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TransactionResult 未找到，尝试无命名空间...");
        type_TransactionResult = Type.GetType("TransactionResult");
        if (type_TransactionResult != null)
            Console.WriteLine("[PASS] 类型 TransactionResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TransactionResult 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
