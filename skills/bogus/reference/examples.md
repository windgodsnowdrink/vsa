# bogus - 使用示例

## 快速开始

### 1. 基本模拟数据生成示例

```csharp
using Bogus;
using System;

namespace BogusDemo
{
    // 定义用户模型
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Bogus 基本模拟数据生成示例");
            Console.WriteLine("=" * 50);
            
            // 创建 Bogus 实例
            var faker = new Faker("zh_CN");
            
            // 配置用户数据生成规则
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Age, f => f.Random.Int(18, 65))
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(3));
            
            // 生成单个用户
            var user = userFaker.Generate();
            
            // 输出结果
            Console.WriteLine("生成的用户信息：");
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"姓名: {user.Name}");
            Console.WriteLine($"邮箱: {user.Email}");
            Console.WriteLine($"电话: {user.PhoneNumber}");
            Console.WriteLine($"年龄: {user.Age}");
            Console.WriteLine($"创建时间: {user.CreatedAt}");
        }
    }
}
```

### 2. 批量模拟数据生成示例

```csharp
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BogusDemo
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Bogus 批量模拟数据生成示例");
            Console.WriteLine("=" * 50);
            
            // 创建 Bogus 实例
            var faker = new Faker("zh_CN");
            
            // 产品类别列表
            var categories = new[] { "电子产品", "家居用品", "服装", "食品", "书籍" };
            
            // 配置产品数据生成规则
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.IndexFaker)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => f.Finance.Amount(10, 10000, 2))
                .RuleFor(p => p.Category, f => f.PickRandom(categories))
                .RuleFor(p => p.Stock, f => f.Random.Int(0, 1000));
            
            // 批量生成 100 个产品
            var products = productFaker.Generate(100);
            
            // 输出结果统计
            Console.WriteLine($"批量生成了 {products.Count} 个产品");
            Console.WriteLine();
            
            // 按类别统计
            var productsByCategory = products.GroupBy(p => p.Category).Select(g => new { Category = g.Key, Count = g.Count() });
            Console.WriteLine("按类别统计：");
            foreach (var group in productsByCategory)
            {
                Console.WriteLine($"  {group.Category}: {group.Count} 个");
            }
            Console.WriteLine();
            
            // 输出前 5 个产品
            Console.WriteLine("前 5 个产品详细信息：");
            foreach (var product in products.Take(5))
            {
                Console.WriteLine($"  ID: {product.Id}, 名称: {product.Name}, 价格: {product.Price} 元, 类别: {product.Category}, 库存: {product.Stock}");
            }
        }
    }
}
```

### 3. AOT 优化的模拟数据生成示例

```csharp
using Bogus;
using System;
using System.Runtime.CompilerServices;

namespace BogusDemo
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
    }

    public class Program
    {
        // 使用 AggressiveOptimization 提示编译器优化
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void Main(string[] args)
        {
            Console.WriteLine("Bogus AOT 优化的模拟数据生成示例");
            Console.WriteLine("=" * 50);
            
            // 创建 Bogus 实例（使用固定种子，便于测试）
            var faker = new Faker("zh_CN").UseSeed(12345);
            
            // 订单状态列表
            var statuses = new[] { "待付款", "待发货", "已发货", "已完成", "已取消" };
            
            // 配置订单数据生成规则
            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.Id, f => f.Random.Guid())
                .RuleFor(o => o.OrderNumber, f => f.Random.Replace("ORD-####-????"))
                .RuleFor(o => o.OrderDate, f => f.Date.Past(1))
                .RuleFor(o => o.TotalAmount, f => f.Finance.Amount(100, 5000, 2))
                .RuleFor(o => o.Status, f => f.PickRandom(statuses))
                .RuleFor(o => o.CustomerName, f => f.Name.FullName());
            
            // 测量生成 1000 个订单的时间
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var orders = orderFaker.Generate(1000);
            stopwatch.Stop();
            
            // 输出结果
            Console.WriteLine($"生成 1000 个订单耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每个订单生成耗时: {stopwatch.Elapsed.TotalMilliseconds / 1000:F3} ms");
            Console.WriteLine();
            
            // 输出前 3 个订单
            Console.WriteLine("前 3 个订单详细信息：");
            foreach (var order in orders.Take(3))
            {
                Console.WriteLine($"  订单号: {order.OrderNumber}, 日期: {order.OrderDate:yyyy-MM-dd}, 金额: {order.TotalAmount} 元, 状态: {order.Status}, 客户: {order.CustomerName}");
            }
        }
    }
}
```

### 4. 关联数据生成示例

```csharp
using Bogus;
using System;
using System.Collections.Generic;

namespace BogusDemo
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Bogus 关联数据生成示例");
            Console.WriteLine("=" * 50);
            
            // 创建 Bogus 实例
            var faker = new Faker("zh_CN");
            
            // 配置类别数据生成规则
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Id, f => f.IndexFaker)
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph());
            
            // 配置产品数据生成规则
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.IndexFaker)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Finance.Amount(10, 1000, 2))
                .RuleFor(p => p.CategoryId, f => f.Random.Int(0, 4)) // 假设只有 5 个类别
                .Ignore(p => p.Category);
            
            // 生成 5 个类别
            var categories = categoryFaker.Generate(5);
            
            // 生成 50 个产品
            var products = productFaker.Generate(50);
            
            // 建立关联关系
            foreach (var product in products)
            {
                // 确保 CategoryId 有效
                var categoryIndex = Math.Abs(product.CategoryId) % categories.Count;
                var category = categories[categoryIndex];
                product.CategoryId = category.Id;
                product.Category = category;
                category.Products.Add(product);
            }
            
            // 输出结果
            Console.WriteLine($"生成了 {categories.Count} 个类别和 {products.Count} 个产品");
            Console.WriteLine();
            
            // 输出每个类别的产品数量
            foreach (var category in categories)
            {
                Console.WriteLine($"类别: {category.Name}, 产品数量: {category.Products.Count}");
            }
            Console.WriteLine();
            
            // 输出第一个类别的产品
            var firstCategory = categories.First();
            Console.WriteLine($"第一个类别 '{firstCategory.Name}' 的产品：");
            foreach (var product in firstCategory.Products)
            {
                Console.WriteLine($"  - {product.Name}, 价格: {product.Price} 元");
            }
        }
    }
}
```

## 总结

以上示例展示了 Bogus 模拟数据生成技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始基本的模拟数据生成
2. 批量生成大量模拟数据
3. 实现 AOT 优化的模拟数据生成
4. 生成具有关联关系的复杂数据

Bogus 技能设计遵循 .NET 10 最佳实践，支持 AOT 编译，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。无论是测试数据生成、演示数据生成还是性能测试数据生成，Bogus 技能都能提供高效、灵活的解决方案。

所有示例都基于 Bogus 库，提供了详细的数据生成规则和配置选项，帮助您快速生成符合业务需求的模拟数据。通过调整数据生成规则和配置，您可以生成各种类型的模拟数据，满足不同场景的需求。
