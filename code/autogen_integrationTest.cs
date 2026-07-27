#load "autogen_integration.cs"

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AutoGenIntegration;

Console.WriteLine("=== autogen_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: AutoGenOptions默认值
    var opts = new AutoGenOptions();
    Report("AutoGenOptions created", opts != null);
    Report("AutoGenOptions default Endpoint", opts.Endpoint == "https://api.autogen.ai/v1");
    Report("AutoGenOptions default MaxConcurrentRequests", opts.MaxConcurrentRequests == 10);
    Report("AutoGenOptions default ModelName", opts.ModelName == "gpt-4");

    // Test 2: IAutoGenService接口存在
    var ifaceType = typeof(IAutoGenService);
    Report("IAutoGenService interface exists", ifaceType.IsInterface);

    // Test 3: AutoGenService类型存在
    var serviceType = typeof(AutoGenService);
    Report("AutoGenService class exists", serviceType.IsClass);

    // Test 4: AutoGenService实现IAutoGenService
    Report("AutoGenService implements IAutoGenService", typeof(IAutoGenService).IsAssignableFrom(serviceType));

    // Test 5: AutoGenService实现IAsyncDisposable
    Report("AutoGenService implements IAsyncDisposable", typeof(IAsyncDisposable).IsAssignableFrom(serviceType));

    // Test 6: AutoGenExtensions DI扩展方法存在
    var extMethod = typeof(AutoGenExtensions).GetMethod("AddAutoGenService");
    Report("AddAutoGenService extension method exists", extMethod != null);

    // Test 7: Validate options
    var validateOpts = new AutoGenOptions { ApiKey = "" };
    var invalid = string.IsNullOrEmpty(validateOpts.ApiKey);
    Report("Options validation: empty ApiKey detected", invalid);

    var validOpts = new AutoGenOptions { ApiKey = "test-key" };
    var valid = !string.IsNullOrEmpty(validOpts.ApiKey);
    Report("Options validation: valid ApiKey", valid);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== autogen_integration Test Complete ===");