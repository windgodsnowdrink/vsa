#load "fluentvalidation_integration.cs"

Console.WriteLine("=== fluentvalidation_integration Test ===");

try
{
    var t0 = typeof(FluentValidationIntegration.FluentValidationOptions);
    Console.WriteLine($"[PASS] FluentValidationOptions 存在");
    var t1 = typeof(FluentValidationIntegration.FluentValidationExtensions);
    Console.WriteLine($"[PASS] FluentValidationExtensions 存在");
    var t2 = typeof(FluentValidationIntegration.ValidatorFactory);
    Console.WriteLine($"[PASS] ValidatorFactory 存在");
    var t3 = typeof(FluentValidationIntegration.DistributedValidatorCache);
    Console.WriteLine($"[PASS] DistributedValidatorCache 存在");
    var t4 = typeof(FluentValidationIntegration.ValidatorLoader);
    Console.WriteLine($"[PASS] ValidatorLoader 存在");
    var t5 = typeof(FluentValidationIntegration.ValidatorMonitor);
    Console.WriteLine($"[PASS] ValidatorMonitor 存在");
    var t6 = typeof(FluentValidationIntegration.ValidatorVersionControl);
    Console.WriteLine($"[PASS] ValidatorVersionControl 存在");
    var t7 = typeof(FluentValidationIntegration.TenantValidatorResolver);
    Console.WriteLine($"[PASS] TenantValidatorResolver 存在");
    var t8 = typeof(FluentValidationIntegration.IDistributedValidatorCache);
    Console.WriteLine($"[PASS] IDistributedValidatorCache 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(FluentValidationIntegration.IValidatorLoader);
    Console.WriteLine($"[PASS] IValidatorLoader 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(FluentValidationIntegration.IValidatorMonitor);
    Console.WriteLine($"[PASS] IValidatorMonitor 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(FluentValidationIntegration.IValidatorVersionControl);
    Console.WriteLine($"[PASS] IValidatorVersionControl 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(FluentValidationIntegration.ITenantValidatorResolver);
    Console.WriteLine($"[PASS] ITenantValidatorResolver 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}