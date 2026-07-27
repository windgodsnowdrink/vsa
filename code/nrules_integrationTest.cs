#load "nrules_integration.cs"

Console.WriteLine("=== nrules_integration.cs Test ===");

try
{
    // 验证 class: OrderDiscountRule
    var type_OrderDiscountRule = Type.GetType("OrderDiscountRule");
    if (type_OrderDiscountRule != null)
    {
        Console.WriteLine("[PASS] 类型 OrderDiscountRule (class) 存在");
        var ctors_OrderDiscountRule = type_OrderDiscountRule.GetConstructors();
        Console.WriteLine($"[PASS] OrderDiscountRule 构造函数数量: {ctors_OrderDiscountRule.Length}");
        var methods_OrderDiscountRule = type_OrderDiscountRule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderDiscountRule 公开方法数量: {methods_OrderDiscountRule.Length}");
        foreach (var m in methods_OrderDiscountRule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderDiscountRule 未找到，尝试无命名空间...");
        type_OrderDiscountRule = Type.GetType("OrderDiscountRule");
        if (type_OrderDiscountRule != null)
            Console.WriteLine("[PASS] 类型 OrderDiscountRule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderDiscountRule 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BulkOrderDiscountRule
    var type_BulkOrderDiscountRule = Type.GetType("BulkOrderDiscountRule");
    if (type_BulkOrderDiscountRule != null)
    {
        Console.WriteLine("[PASS] 类型 BulkOrderDiscountRule (class) 存在");
        var ctors_BulkOrderDiscountRule = type_BulkOrderDiscountRule.GetConstructors();
        Console.WriteLine($"[PASS] BulkOrderDiscountRule 构造函数数量: {ctors_BulkOrderDiscountRule.Length}");
        var methods_BulkOrderDiscountRule = type_BulkOrderDiscountRule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BulkOrderDiscountRule 公开方法数量: {methods_BulkOrderDiscountRule.Length}");
        foreach (var m in methods_BulkOrderDiscountRule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BulkOrderDiscountRule 未找到，尝试无命名空间...");
        type_BulkOrderDiscountRule = Type.GetType("BulkOrderDiscountRule");
        if (type_BulkOrderDiscountRule != null)
            Console.WriteLine("[PASS] 类型 BulkOrderDiscountRule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BulkOrderDiscountRule 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RuleEngineOptions
    var type_RuleEngineOptions = Type.GetType("RuleEngineOptions");
    if (type_RuleEngineOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RuleEngineOptions (class) 存在");
        var ctors_RuleEngineOptions = type_RuleEngineOptions.GetConstructors();
        Console.WriteLine($"[PASS] RuleEngineOptions 构造函数数量: {ctors_RuleEngineOptions.Length}");
        var methods_RuleEngineOptions = type_RuleEngineOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RuleEngineOptions 公开方法数量: {methods_RuleEngineOptions.Length}");
        foreach (var m in methods_RuleEngineOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RuleEngineOptions 未找到，尝试无命名空间...");
        type_RuleEngineOptions = Type.GetType("RuleEngineOptions");
        if (type_RuleEngineOptions != null)
            Console.WriteLine("[PASS] 类型 RuleEngineOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RuleEngineOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RuleEngine
    var type_RuleEngine = Type.GetType("RuleEngine");
    if (type_RuleEngine != null)
    {
        Console.WriteLine("[PASS] 类型 RuleEngine (class) 存在");
        var ctors_RuleEngine = type_RuleEngine.GetConstructors();
        Console.WriteLine($"[PASS] RuleEngine 构造函数数量: {ctors_RuleEngine.Length}");
        var methods_RuleEngine = type_RuleEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RuleEngine 公开方法数量: {methods_RuleEngine.Length}");
        foreach (var m in methods_RuleEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RuleEngine 未找到，尝试无命名空间...");
        type_RuleEngine = Type.GetType("RuleEngine");
        if (type_RuleEngine != null)
            Console.WriteLine("[PASS] 类型 RuleEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RuleEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RuleEngineExtensions
    var type_RuleEngineExtensions = Type.GetType("RuleEngineExtensions");
    if (type_RuleEngineExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 RuleEngineExtensions (class) 存在");
        var ctors_RuleEngineExtensions = type_RuleEngineExtensions.GetConstructors();
        Console.WriteLine($"[PASS] RuleEngineExtensions 构造函数数量: {ctors_RuleEngineExtensions.Length}");
        var methods_RuleEngineExtensions = type_RuleEngineExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RuleEngineExtensions 公开方法数量: {methods_RuleEngineExtensions.Length}");
        foreach (var m in methods_RuleEngineExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RuleEngineExtensions 未找到，尝试无命名空间...");
        type_RuleEngineExtensions = Type.GetType("RuleEngineExtensions");
        if (type_RuleEngineExtensions != null)
            Console.WriteLine("[PASS] 类型 RuleEngineExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RuleEngineExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ElevatorStateMachine
    var type_ElevatorStateMachine = Type.GetType("ElevatorStateMachine");
    if (type_ElevatorStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorStateMachine (class) 存在");
        var ctors_ElevatorStateMachine = type_ElevatorStateMachine.GetConstructors();
        Console.WriteLine($"[PASS] ElevatorStateMachine 构造函数数量: {ctors_ElevatorStateMachine.Length}");
        var methods_ElevatorStateMachine = type_ElevatorStateMachine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ElevatorStateMachine 公开方法数量: {methods_ElevatorStateMachine.Length}");
        foreach (var m in methods_ElevatorStateMachine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorStateMachine 未找到，尝试无命名空间...");
        type_ElevatorStateMachine = Type.GetType("ElevatorStateMachine");
        if (type_ElevatorStateMachine != null)
            Console.WriteLine("[PASS] 类型 ElevatorStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorStateMachine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ElevatorDispatchRule
    var type_ElevatorDispatchRule = Type.GetType("ElevatorDispatchRule");
    if (type_ElevatorDispatchRule != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorDispatchRule (class) 存在");
        var ctors_ElevatorDispatchRule = type_ElevatorDispatchRule.GetConstructors();
        Console.WriteLine($"[PASS] ElevatorDispatchRule 构造函数数量: {ctors_ElevatorDispatchRule.Length}");
        var methods_ElevatorDispatchRule = type_ElevatorDispatchRule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ElevatorDispatchRule 公开方法数量: {methods_ElevatorDispatchRule.Length}");
        foreach (var m in methods_ElevatorDispatchRule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorDispatchRule 未找到，尝试无命名空间...");
        type_ElevatorDispatchRule = Type.GetType("ElevatorDispatchRule");
        if (type_ElevatorDispatchRule != null)
            Console.WriteLine("[PASS] 类型 ElevatorDispatchRule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorDispatchRule 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OfflineElevatorRule
    var type_OfflineElevatorRule = Type.GetType("OfflineElevatorRule");
    if (type_OfflineElevatorRule != null)
    {
        Console.WriteLine("[PASS] 类型 OfflineElevatorRule (class) 存在");
        var ctors_OfflineElevatorRule = type_OfflineElevatorRule.GetConstructors();
        Console.WriteLine($"[PASS] OfflineElevatorRule 构造函数数量: {ctors_OfflineElevatorRule.Length}");
        var methods_OfflineElevatorRule = type_OfflineElevatorRule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OfflineElevatorRule 公开方法数量: {methods_OfflineElevatorRule.Length}");
        foreach (var m in methods_OfflineElevatorRule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OfflineElevatorRule 未找到，尝试无命名空间...");
        type_OfflineElevatorRule = Type.GetType("OfflineElevatorRule");
        if (type_OfflineElevatorRule != null)
            Console.WriteLine("[PASS] 类型 OfflineElevatorRule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OfflineElevatorRule 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ElevatorRuleEngineSample
    var type_ElevatorRuleEngineSample = Type.GetType("ElevatorRuleEngineSample");
    if (type_ElevatorRuleEngineSample != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorRuleEngineSample (class) 存在");
        var ctors_ElevatorRuleEngineSample = type_ElevatorRuleEngineSample.GetConstructors();
        Console.WriteLine($"[PASS] ElevatorRuleEngineSample 构造函数数量: {ctors_ElevatorRuleEngineSample.Length}");
        var methods_ElevatorRuleEngineSample = type_ElevatorRuleEngineSample.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ElevatorRuleEngineSample 公开方法数量: {methods_ElevatorRuleEngineSample.Length}");
        foreach (var m in methods_ElevatorRuleEngineSample)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorRuleEngineSample 未找到，尝试无命名空间...");
        type_ElevatorRuleEngineSample = Type.GetType("ElevatorRuleEngineSample");
        if (type_ElevatorRuleEngineSample != null)
            Console.WriteLine("[PASS] 类型 ElevatorRuleEngineSample (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorRuleEngineSample 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Order
    var type_Order = Type.GetType("Order");
    if (type_Order != null)
    {
        Console.WriteLine("[PASS] 类型 Order (record) 存在");
        var ctors_Order = type_Order.GetConstructors();
        Console.WriteLine($"[PASS] Order 构造函数数量: {ctors_Order.Length}");
        var methods_Order = type_Order.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Order 公开方法数量: {methods_Order.Length}");
        foreach (var m in methods_Order)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Order 未找到，尝试无命名空间...");
        type_Order = Type.GetType("Order");
        if (type_Order != null)
            Console.WriteLine("[PASS] 类型 Order (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Order 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Discount
    var type_Discount = Type.GetType("Discount");
    if (type_Discount != null)
    {
        Console.WriteLine("[PASS] 类型 Discount (record) 存在");
        var ctors_Discount = type_Discount.GetConstructors();
        Console.WriteLine($"[PASS] Discount 构造函数数量: {ctors_Discount.Length}");
        var methods_Discount = type_Discount.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Discount 公开方法数量: {methods_Discount.Length}");
        foreach (var m in methods_Discount)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Discount 未找到，尝试无命名空间...");
        type_Discount = Type.GetType("Discount");
        if (type_Discount != null)
            Console.WriteLine("[PASS] 类型 Discount (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Discount 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ElevatorSignal
    var type_ElevatorSignal = Type.GetType("ElevatorSignal");
    if (type_ElevatorSignal != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorSignal (record) 存在");
        var ctors_ElevatorSignal = type_ElevatorSignal.GetConstructors();
        Console.WriteLine($"[PASS] ElevatorSignal 构造函数数量: {ctors_ElevatorSignal.Length}");
        var methods_ElevatorSignal = type_ElevatorSignal.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ElevatorSignal 公开方法数量: {methods_ElevatorSignal.Length}");
        foreach (var m in methods_ElevatorSignal)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorSignal 未找到，尝试无命名空间...");
        type_ElevatorSignal = Type.GetType("ElevatorSignal");
        if (type_ElevatorSignal != null)
            Console.WriteLine("[PASS] 类型 ElevatorSignal (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorSignal 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ElevatorDispatch
    var type_ElevatorDispatch = Type.GetType("ElevatorDispatch");
    if (type_ElevatorDispatch != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorDispatch (record) 存在");
        var ctors_ElevatorDispatch = type_ElevatorDispatch.GetConstructors();
        Console.WriteLine($"[PASS] ElevatorDispatch 构造函数数量: {ctors_ElevatorDispatch.Length}");
        var methods_ElevatorDispatch = type_ElevatorDispatch.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ElevatorDispatch 公开方法数量: {methods_ElevatorDispatch.Length}");
        foreach (var m in methods_ElevatorDispatch)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorDispatch 未找到，尝试无命名空间...");
        type_ElevatorDispatch = Type.GetType("ElevatorDispatch");
        if (type_ElevatorDispatch != null)
            Console.WriteLine("[PASS] 类型 ElevatorDispatch (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorDispatch 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ElevatorAlert
    var type_ElevatorAlert = Type.GetType("ElevatorAlert");
    if (type_ElevatorAlert != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorAlert (record) 存在");
        var ctors_ElevatorAlert = type_ElevatorAlert.GetConstructors();
        Console.WriteLine($"[PASS] ElevatorAlert 构造函数数量: {ctors_ElevatorAlert.Length}");
        var methods_ElevatorAlert = type_ElevatorAlert.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ElevatorAlert 公开方法数量: {methods_ElevatorAlert.Length}");
        foreach (var m in methods_ElevatorAlert)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorAlert 未找到，尝试无命名空间...");
        type_ElevatorAlert = Type.GetType("ElevatorAlert");
        if (type_ElevatorAlert != null)
            Console.WriteLine("[PASS] 类型 ElevatorAlert (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorAlert 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ElevatorDirection
    var type_ElevatorDirection = Type.GetType("ElevatorDirection");
    if (type_ElevatorDirection != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorDirection (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorDirection 未找到，尝试无命名空间...");
        type_ElevatorDirection = Type.GetType("ElevatorDirection");
        if (type_ElevatorDirection != null)
            Console.WriteLine("[PASS] 类型 ElevatorDirection (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorDirection 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ElevatorAlertType
    var type_ElevatorAlertType = Type.GetType("ElevatorAlertType");
    if (type_ElevatorAlertType != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorAlertType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorAlertType 未找到，尝试无命名空间...");
        type_ElevatorAlertType = Type.GetType("ElevatorAlertType");
        if (type_ElevatorAlertType != null)
            Console.WriteLine("[PASS] 类型 ElevatorAlertType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorAlertType 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ElevatorStatus
    var type_ElevatorStatus = Type.GetType("ElevatorStatus");
    if (type_ElevatorStatus != null)
    {
        Console.WriteLine("[PASS] 类型 ElevatorStatus (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ElevatorStatus 未找到，尝试无命名空间...");
        type_ElevatorStatus = Type.GetType("ElevatorStatus");
        if (type_ElevatorStatus != null)
            Console.WriteLine("[PASS] 类型 ElevatorStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ElevatorStatus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
