#load "bogus_advanced_integration.cs"

using System;
using Microsoft.Extensions.DependencyInjection;
using Bogus;

Console.WriteLine("=== bogus_advanced_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: OrderContext类型存在
    var ctxType = typeof(OrderContext);
    Report("OrderContext class exists", ctxType.IsClass);

    // Test 2: MultiLingualDataGenerator类型存在
    var genType = typeof(MultiLingualDataGenerator);
    Report("MultiLingualDataGenerator class exists", genType.IsClass);

    // Test 3: MultiLingualDataGenerator可实例化
    var gen = new MultiLingualDataGenerator();
    Report("MultiLingualDataGenerator instantiated", gen != null);

    // Test 4: GenerateUser方法存在
    var genUserMethod = genType.GetMethod("GenerateUser");
    Report("GenerateUser method exists", genUserMethod != null);

    // Test 5: 生成用户数据
    var user = gen.GenerateUser("en");
    Report("Generated user not null", user != null);
    Report("Generated user has Name", !string.IsNullOrWhiteSpace(user.Name));
    Report("Generated user has Email", !string.IsNullOrWhiteSpace(user.Email));

    // Test 6: CustomBogusRules扩展方法存在
    var extType = typeof(CustomBogusRules);
    Report("CustomBogusRules class exists", extType.IsClass);

    var withCustomRuleMethod = extType.GetMethod("WithCustomRule");
    Report("WithCustomRule extension method exists", withCustomRuleMethod != null);

    // Test 7: BogusServiceCollectionExtensions存在
    var svcExtType = typeof(BogusServiceCollectionExtensions);
    Report("BogusServiceCollectionExtensions class exists", svcExtType.IsClass);

    var addMethod = svcExtType.GetMethod("AddAdvancedBogusServices");
    Report("AddAdvancedBogusServices method exists", addMethod != null);

    // Test 8: DI集成
    var services = new ServiceCollection();
    services.AddAdvancedBogusServices();
    var provider = services.BuildServiceProvider();
    var mlGen = provider.GetService<MultiLingualDataGenerator>();
    Report("DI: MultiLingualDataGenerator resolved", mlGen != null);

    var orderFaker = provider.GetService<Faker<OrderContext>>();
    Report("DI: Faker<OrderContext> resolved", orderFaker != null);

    // Test 9: BogusAdvancedDemo类型存在
    var demoType = typeof(BogusAdvancedDemo);
    Report("BogusAdvancedDemo class exists", demoType.IsClass);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== bogus_advanced_integration Test Complete ===");