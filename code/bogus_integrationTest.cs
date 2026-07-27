#load "bogus_integration.cs"

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== bogus_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: User record类型存在
    var userType = typeof(User);
    Report("User record exists", userType != null);

    // Test 2: Product record类型存在
    var productType = typeof(Product);
    Report("Product record exists", productType != null);

    // Test 3: Order record类型存在
    var orderType = typeof(Order);
    Report("Order record exists", orderType != null);

    // Test 4: OrderItem record类型存在
    var orderItemType = typeof(OrderItem);
    Report("OrderItem record exists", orderItemType != null);

    // Test 5: IDataGenerator接口存在
    var ifaceType = typeof(IDataGenerator);
    Report("IDataGenerator interface exists", ifaceType.IsInterface);

    // Test 6: BogusDataGenerator类型存在
    var genType = typeof(BogusDataGenerator);
    Report("BogusDataGenerator class exists", genType.IsClass);

    // Test 7: BogusDataGenerator实现IDataGenerator
    Report("BogusDataGenerator implements IDataGenerator", typeof(IDataGenerator).IsAssignableFrom(genType));

    // Test 8: BogusServiceExtensions存在
    var extType = typeof(BogusServiceExtensions);
    Report("BogusServiceExtensions class exists", extType.IsClass);

    var addMethod = extType.GetMethod("AddBogusDataGenerator");
    Report("AddBogusDataGenerator method exists", addMethod != null);

    // Test 9: DI集成
    var services = new ServiceCollection();
    services.AddBogusDataGenerator();
    var provider = services.BuildServiceProvider();
    var generator = provider.GetService<IDataGenerator>();
    Report("DI: IDataGenerator resolved", generator != null);

    // Test 10: 生成数据
    var users = generator.Generate<User>(10);
    Report("Generate 10 users", users.Count() == 10);

    var firstUser = users.First();
    Report("First user has Id", firstUser.Id > 0);
    Report("First user has Name", !string.IsNullOrWhiteSpace(firstUser.Name));
    Report("First user has Email", !string.IsNullOrWhiteSpace(firstUser.Email));

    var products = generator.Generate<Product>(5);
    Report("Generate 5 products", products.Count() == 5);

    var firstProduct = products.First();
    Report("First product has Name", !string.IsNullOrWhiteSpace(firstProduct.Name));
    Report("First product has Price", firstProduct.Price > 0);

    // Test 11: GenerateOne
    var singleUser = generator.GenerateOne<User>();
    Report("GenerateOne returns user", singleUser != null);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== bogus_integration Test Complete ===");