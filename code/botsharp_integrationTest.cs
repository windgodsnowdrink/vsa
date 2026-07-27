#load "botsharp_integration.cs"

using System;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== botsharp_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: BotSharpOptions默认值
    var opts = new BotSharpOptions();
    Report("BotSharpOptions created", opts != null);
    Report("BotSharpOptions default ModelName", opts.ModelName == "gpt-3.5-turbo");
    Report("BotSharpOptions default Timeout", opts.Timeout == TimeSpan.FromSeconds(30));
    Report("BotSharpOptions default MaxRetryCount", opts.MaxRetryCount == 3);
    Report("BotSharpOptions default EnableTracing", opts.EnableTracing == true);
    Report("BotSharpOptions default EnableMetrics", opts.EnableMetrics == true);
    Report("BotSharpOptions default EnableResponseCache", opts.EnableResponseCache == true);
    Report("BotSharpOptions default CacheExpiration", opts.CacheExpiration == TimeSpan.FromMinutes(5));
    Report("BotSharpOptions default EnableRateLimiting", opts.EnableRateLimiting == true);
    Report("BotSharpOptions default RateLimitPerMinute", opts.RateLimitPerMinute == 60);

    // Test 2: Validate with empty AgentId throws
    try
    {
        opts.Validate();
        Report("Validate with empty AgentId throws", false);
    }
    catch (ArgumentException)
    {
        Report("Validate with empty AgentId throws ArgumentException", true);
    }

    // Test 3: Validate with valid AgentId and ModelName
    var validOpts = new BotSharpOptions { AgentId = "test-agent", ModelName = "gpt-4" };
    try
    {
        validOpts.Validate();
        Report("Validate with valid options succeeds", true);
    }
    catch
    {
        Report("Validate with valid options succeeds", false);
    }

    // Test 4: IBotSharpPipeline接口存在
    var pipelineType = typeof(IBotSharpPipeline);
    Report("IBotSharpPipeline interface exists", pipelineType.IsInterface);

    // Test 5: IBotSharpPipeline extends IAsyncDisposable
    Report("IBotSharpPipeline extends IAsyncDisposable", typeof(IAsyncDisposable).IsAssignableFrom(pipelineType));

    // Test 6: BotSharpProcessor类型存在
    var processorType = typeof(BotSharpProcessor);
    Report("BotSharpProcessor class exists", processorType.IsClass);

    // Test 7: BotSharpProcessor implements IBotSharpPipeline
    Report("BotSharpProcessor implements IBotSharpPipeline", typeof(IBotSharpPipeline).IsAssignableFrom(processorType));

    // Test 8: BotSharpServiceCollectionExtensions存在
    var extType = typeof(BotSharpServiceCollectionExtensions);
    Report("BotSharpServiceCollectionExtensions class exists", extType.IsClass);

    var addMethod = extType.GetMethod("AddBotSharpIntegration");
    Report("BotSharpServiceCollectionExtensions.AddBotSharpIntegration exists", addMethod != null);

    // Test 9: ServiceCollectionExtensions存在
    var svcExtType = typeof(ServiceCollectionExtensions);
    Report("ServiceCollectionExtensions class exists", svcExtType.IsClass);

    var svcAddMethod = svcExtType.GetMethod("AddBotSharpIntegration");
    Report("ServiceCollectionExtensions.AddBotSharpIntegration exists", svcAddMethod != null);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== botsharp_integration Test Complete ===");