#load "scalar_api_server.cs"

Console.WriteLine("=== scalar_api_server Test ===");

try
{
    var scalarApiOptionsType = Type.GetType("ScalarApiOptions");
    Console.WriteLine(scalarApiOptionsType != null ? "[PASS] ScalarApiOptions 类型存在" : "[FAIL] ScalarApiOptions 类型未找到");

    var scalarApiExtensionsType = Type.GetType("ScalarApiExtensions");
    Console.WriteLine(scalarApiExtensionsType != null ? "[PASS] ScalarApiExtensions 类型存在" : "[FAIL] ScalarApiExtensions 类型未找到");

    var scalarControllerType = Type.GetType("ScalarController");
    Console.WriteLine(scalarControllerType != null ? "[PASS] ScalarController 类型存在" : "[FAIL] ScalarController 类型未找到");

    var scalarRequestType = Type.GetType("ScalarRequest");
    Console.WriteLine(scalarRequestType != null ? "[PASS] ScalarRequest 类型存在" : "[FAIL] ScalarRequest 类型未找到");

    if (scalarApiExtensionsType != null)
    {
        Console.WriteLine(scalarApiExtensionsType.GetMethod("AddScalarApi") != null ? "[PASS] ScalarApiExtensions.AddScalarApi 方法存在" : "[FAIL] ScalarApiExtensions.AddScalarApi 方法未找到");
        Console.WriteLine(scalarApiExtensionsType.GetMethod("UseScalarApi") != null ? "[PASS] ScalarApiExtensions.UseScalarApi 方法存在" : "[FAIL] ScalarApiExtensions.UseScalarApi 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}