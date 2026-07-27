#load "fluentvalidation_aot.cs"

Console.WriteLine("=== fluentvalidation_aot Test ===");

try
{
    var t0 = typeof(FluentValidation.AOT.FluentValidationOptions);
    Console.WriteLine($"[PASS] FluentValidationOptions 存在");
    var t1 = typeof(FluentValidation.AOT.ValidationResultDto);
    Console.WriteLine($"[PASS] ValidationResultDto 存在");
    var t2 = typeof(FluentValidation.AOT.ValidationErrorDto);
    Console.WriteLine($"[PASS] ValidationErrorDto 存在");
    var t3 = typeof(FluentValidation.AOT.ValidationCommandResult);
    Console.WriteLine($"[PASS] ValidationCommandResult 存在");
    var t4 = typeof(FluentValidation.AOT.TestModel);
    Console.WriteLine($"[PASS] TestModel 存在");
    var t5 = typeof(FluentValidation.AOT.TestModelValidator);
    Console.WriteLine($"[PASS] TestModelValidator 存在");
    var t6 = typeof(FluentValidation.AOT.FluentValidationService);
    Console.WriteLine($"[PASS] FluentValidationService 存在");
    var t7 = typeof(FluentValidation.AOT.FluentValidationAotEngine);
    Console.WriteLine($"[PASS] FluentValidationAotEngine 存在");
    var t8 = typeof(FluentValidation.AOT.IFluentValidationService);
    Console.WriteLine($"[PASS] IFluentValidationService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(FluentValidation.AOT.ValidationCommandType);
    Console.WriteLine($"[PASS] ValidationCommandType enum 存在 (IsEnum: {t9.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}