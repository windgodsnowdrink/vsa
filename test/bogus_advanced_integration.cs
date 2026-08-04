#:sdk Microsoft.NET.Sdk.Web
#:package Bogus@35.4.0
#:property LangVersion=preview
#:property TargetFramework=net11.0

using Bogus;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

// 上下文感知数据生成
public class OrderContext
{
    public User User { get; set; }
    public List<Product> Products { get; set; }
}

// 多语言数据生成器
public class MultiLingualDataGenerator
{
    private readonly Faker<User> _userFaker;
    
    public MultiLingualDataGenerator()
    {
        _userFaker = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
            .UseSeed(1234);
    }
    
    public User GenerateUser(string locale = "en") => 
        _userFaker.UseLocale(locale).Generate();
}

// 自定义规则扩展
public static class CustomBogusRules
{
    public static Faker<T> WithCustomRule<T>(this Faker<T> faker, string ruleName, Func<Faker, T, object> ruleFunc) 
        where T : class => faker.RuleFor(ruleName, ruleFunc);
}

// 生产级DI集成
public static class BogusServiceCollectionExtensions
{
    public static IServiceCollection AddAdvancedBogusServices(this IServiceCollection services)
    {
        services.AddSingleton<MultiLingualDataGenerator>();
        services.AddScoped<Faker<OrderContext>>(sp => 
            new Faker<OrderContext>()
                .RuleFor(o => o.User, f => sp.GetRequiredService<MultiLingualDataGenerator>().GenerateUser())
                .RuleFor(o => o.Products, f => f.MakeLazy(5, () => new Faker<Product>().Generate()))
                .WithCustomRule("CustomField", (f, o) => f.Random.Word()));
                
        return services;
    }
}

// 示例用法
public class BogusAdvancedDemo
{
    public static void Main()
    {
        var services = new ServiceCollection();
        services.AddAdvancedBogusServices();
        
        using var sp = services.BuildServiceProvider();
        var orderFaker = sp.GetRequiredService<Faker<OrderContext>>();
        
        // 生成上下文关联数据
        var order = orderFaker.Generate();
        Console.WriteLine($"User: {order.User.Name}, Product Count: {order.Products.Count}");
        
        // 多语言生成
        var mlGenerator = sp.GetRequiredService<MultiLingualDataGenerator>();
        var jpUser = mlGenerator.GenerateUser("ja");
        Console.WriteLine($"Japanese User: {jpUser.Name}");
    }
}