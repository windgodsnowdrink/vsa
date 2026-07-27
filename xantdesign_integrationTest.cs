#load "xantdesign_integration.cs"

Console.WriteLine("=== xantdesign_integration Test ===");

try
{
    var t0 = typeof(XAntDesignIntegration.XAntDesignOptions);
    Console.WriteLine($"[PASS] XAntDesignOptions 存在");
    var t1 = typeof(XAntDesignIntegration.XAntDesignService);
    Console.WriteLine($"[PASS] XAntDesignService 存在");
    var t2 = typeof(XAntDesignIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(XAntDesignIntegration.WebAssemblyHostBuilderExtensions);
    Console.WriteLine($"[PASS] WebAssemblyHostBuilderExtensions 存在");
    var t4 = typeof(XAntDesignIntegration.IXAntDesignService);
    Console.WriteLine($"[PASS] IXAntDesignService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}