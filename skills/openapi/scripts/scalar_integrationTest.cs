#load "scalar_integration.cs"

Console.WriteLine("=== scalar_integration Test ===");

try
{
    var scalarOptionsType = Type.GetType("ScalarOptions");
    Console.WriteLine(scalarOptionsType != null ? "[PASS] ScalarOptions 类型存在" : "[FAIL] ScalarOptions 类型未找到");

    var iScalarServiceType = Type.GetType("IScalarService");
    Console.WriteLine(iScalarServiceType != null ? "[PASS] IScalarService 接口存在" : "[FAIL] IScalarService 接口未找到");

    var scalarServiceType = Type.GetType("ScalarService");
    Console.WriteLine(scalarServiceType != null ? "[PASS] ScalarService 类型存在" : "[FAIL] ScalarService 类型未找到");

    var scalarExtensionsType = Type.GetType("ScalarExtensions");
    Console.WriteLine(scalarExtensionsType != null ? "[PASS] ScalarExtensions 类型存在" : "[FAIL] ScalarExtensions 类型未找到");

    var scalarControllerType = Type.GetType("ScalarController");
    Console.WriteLine(scalarControllerType != null ? "[PASS] ScalarController 类型存在" : "[FAIL] ScalarController 类型未找到");

    if (iScalarServiceType != null)
    {
        Console.WriteLine(iScalarServiceType.GetMethod("ProcessAsync") != null ? "[PASS] IScalarService.ProcessAsync 方法存在" : "[FAIL] IScalarService.ProcessAsync 方法未找到");
    }

    if (scalarOptionsType != null)
    {
        Console.WriteLine(scalarOptionsType.GetProperty("BatchSize") != null ? "[PASS] ScalarOptions.BatchSize 属性存在" : "[FAIL] ScalarOptions.BatchSize 属性未找到");
        Console.WriteLine(scalarOptionsType.GetProperty("CircuitBreakerThreshold") != null ? "[PASS] ScalarOptions.CircuitBreakerThreshold 属性存在" : "[FAIL] ScalarOptions.CircuitBreakerThreshold 属性未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}