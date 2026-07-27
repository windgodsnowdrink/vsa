#load "bitplatform_integration.cs"

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

Console.WriteLine("=== bitplatform_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: BitPlatformOptions默认值
    var opts = new BitPlatformOptions();
    Report("BitPlatformOptions created", opts != null);
    Report("BitPlatformOptions default DefaultTheme", opts.DefaultTheme == "light");
    Report("BitPlatformOptions default EnableAOT", opts.EnableAOT == true);
    Report("BitPlatformOptions default MaxConcurrentOperations", opts.MaxConcurrentOperations == 100);
    Report("BitPlatformOptions default OperationTimeout", opts.OperationTimeout == TimeSpan.FromSeconds(30));

    // Test 2: IBitPlatformService接口存在
    var ifaceType = typeof(IBitPlatformService);
    Report("IBitPlatformService interface exists", ifaceType.IsInterface);

    // Test 3: BitPlatformService类型存在
    var serviceType = typeof(BitPlatformService);
    Report("BitPlatformService class exists", serviceType.IsClass);

    // Test 4: BitPlatformService实现IBitPlatformService
    Report("BitPlatformService implements IBitPlatformService", typeof(IBitPlatformService).IsAssignableFrom(serviceType));

    // Test 5: BitPlatformService实现IDisposable
    Report("BitPlatformService implements IDisposable", typeof(IDisposable).IsAssignableFrom(serviceType));

    // Test 6: BitPlatformExtensions扩展方法存在
    var extType = typeof(BitPlatformExtensions);
    Report("BitPlatformExtensions class exists", extType.IsClass);

    var addMethod = extType.GetMethod("AddBitPlatform");
    Report("AddBitPlatform extension method exists", addMethod != null);

    // Test 7: DI集成 - 注册服务
    var services = new ServiceCollection();
    services.AddBitPlatform(options =>
    {
        options.DefaultTheme = "dark";
        options.EnableAOT = false;
        options.MaxConcurrentOperations = 200;
    });
    var provider = services.BuildServiceProvider();
    var resolved = provider.GetService<IBitPlatformService>();
    Report("DI: IBitPlatformService resolved", resolved != null);

    // Test 8: BitPlatformExample类型存在
    var exampleType = typeof(BitPlatformExample);
    Report("BitPlatformExample class exists", exampleType.IsClass);

    var demoMethod = exampleType.GetMethod("Demo");
    Report("BitPlatformExample.Demo method exists", demoMethod != null);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== bitplatform_integration Test Complete ===");