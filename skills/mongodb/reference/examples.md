# MongoDB - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;

// 用户模型
public class User
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// MongoDB 存储库接口
public interface IMongoRepository
{
    Task InsertAsync(User user);
    Task<User> FindByIdAsync(ObjectId id);
    Task UpdateAsync(User user);
    Task DeleteAsync(ObjectId id);
    Task<List<User>> FindAllAsync();
}

// MongoDB 存储库实现
public class MongoRepository : IMongoRepository
{
    private readonly IMongoCollection<User> _collection;

    public MongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<User>("users");
    }

    public async Task InsertAsync(User user)
    {
        await _collection.InsertOneAsync(user);
    }

    public async Task<User> FindByIdAsync(ObjectId id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await _collection.ReplaceOneAsync(x => x.Id == user.Id, user);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<List<User>> FindAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
}

// 依赖注入扩展
public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDB(this IServiceCollection services, string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        services.AddSingleton<IMongoClient>(client);
        services.AddSingleton<IMongoDatabase>(database);
        services.AddSingleton<IMongoRepository, MongoRepository>();

        return services;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MongoDB 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddMongoDB("mongodb://localhost:27017", "myDatabase");
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取 MongoDB 存储库
        var repository = serviceProvider.GetRequiredService<IMongoRepository>();
        
        try
        {
            // 创建用户
            var user = new User
            {
                Id = ObjectId.GenerateNewId(),
                Name = "张三",
                Email = "zhangsan@example.com",
                Age = 30
            };
            
            // 插入用户
            await repository.InsertAsync(user);
            Console.WriteLine($"用户 {user.Name} 创建成功");
            
            // 查询用户
            var foundUser = await repository.FindByIdAsync(user.Id);
            if (foundUser != null)
            {
                Console.WriteLine($"找到用户: {foundUser.Name}, 年龄: {foundUser.Age}");
            }
            
            // 更新用户
            foundUser.Age = 31;
            await repository.UpdateAsync(foundUser);
            Console.WriteLine($"用户 {foundUser.Name} 年龄更新为 {foundUser.Age}");
            
            // 列出所有用户
            var allUsers = await repository.FindAllAsync();
            Console.WriteLine($"总用户数: {allUsers.Count}");
            foreach (var u in allUsers)
            {
                Console.WriteLine($"- {u.Name} ({u.Email})");
            }
            
            // 删除用户
            await repository.DeleteAsync(user.Id);
            Console.WriteLine($"用户 {user.Name} 删除成功");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

// MongoDB 配置选项
public class MongoDbOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "myDatabase";
    public int MaxConnectionPoolSize { get; set; } = 100;
    public int MinConnectionPoolSize { get; set; } = 10;
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan ServerSelectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan SocketTimeout { get; set; } = TimeSpan.FromMinutes(2);
    public TimeSpan WaitQueueTimeout { get; set; } = TimeSpan.FromSeconds(2);
    public bool EnableDetailedLogging { get; set; } = false;
}

// 依赖注入扩展
public static class MongoDbAdvancedExtensions
{
    public static IServiceCollection AddMongoDB(this IServiceCollection services, Action<MongoDbOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        services.AddSingleton<IMongoClient>(sp => {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            
            var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);
            settings.MaxConnectionPoolSize = options.MaxConnectionPoolSize;
            settings.MinConnectionPoolSize = options.MinConnectionPoolSize;
            settings.ConnectTimeout = options.ConnectTimeout;
            settings.ServerSelectionTimeout = options.ServerSelectionTimeout;
            settings.SocketTimeout = options.SocketTimeout;
            settings.WaitQueueTimeout = options.WaitQueueTimeout;
            
            if (options.EnableDetailedLogging)
            {
                settings.ClusterConfigurator = builder => {
                    builder.Subscribe<MongoDB.Driver.Core.Events.CommandStartedEvent>(e => {
                        Console.WriteLine($"MongoDB Command: {e.CommandName}");
                    });
                };
            }
            
            return new MongoClient(settings);
        });
        
        services.AddSingleton<IMongoDatabase>(sp => {
            var client = sp.GetRequiredService<IMongoClient>();
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return client.GetDatabase(options.DatabaseName);
        });
        
        services.AddSingleton<IMongoRepository, MongoRepository>();
        
        return services;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MongoDB 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置 MongoDB
        services.AddMongoDB(options => {
            options.ConnectionString = "mongodb://localhost:27017";
            options.DatabaseName = "myDatabase";
            options.MaxConnectionPoolSize = 150;
            options.MinConnectionPoolSize = 20;
            options.ConnectTimeout = TimeSpan.FromSeconds(45);
            options.ServerSelectionTimeout = TimeSpan.FromSeconds(45);
            options.SocketTimeout = TimeSpan.FromMinutes(3);
            options.WaitQueueTimeout = TimeSpan.FromSeconds(5);
            options.EnableDetailedLogging = true;
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取配置
        var options = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
        Console.WriteLine($"配置: 连接池大小={options.MinConnectionPoolSize}-{options.MaxConnectionPoolSize}");
        Console.WriteLine($"超时设置: 连接={options.ConnectTimeout}, Socket={options.SocketTimeout}");
        
        // 获取 MongoDB 客户端
        var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
        Console.WriteLine($"MongoDB 客户端创建成功");
        
        // 获取数据库
        var database = serviceProvider.GetRequiredService<IMongoDatabase>();
        Console.WriteLine($"数据库: {database.DatabaseNamespace.DatabaseName}");
        
        // 获取存储库
        var repository = serviceProvider.GetRequiredService<IMongoRepository>();
        Console.WriteLine($"存储库创建成功");
        
        Console.WriteLine("高级配置示例完成");
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;

// 订单模型
public class Order
{
    public ObjectId Id { get; set; }
    public ObjectId UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
}

// 高性能 MongoDB 服务
public class HighPerformanceMongoService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Order> _orderCollection;

    public HighPerformanceMongoService(IMongoDatabase database)
    {
        _database = database;
        _userCollection = database.GetCollection<User>("users");
        _orderCollection = database.GetCollection<Order>("orders");
    }

    // 批量插入用户
    public async Task BatchInsertUsersAsync(IEnumerable<User> users)
    {
        if (users == null || !users.Any())
            return;

        // 使用无序插入提高性能
        await _userCollection.InsertManyAsync(users, new InsertManyOptions { IsOrdered = false });
    }

    // 批量插入订单
    public async Task BatchInsertOrdersAsync(IEnumerable<Order> orders)
    {
        if (orders == null || !orders.Any())
            return;

        // 使用无序插入提高性能
        await _orderCollection.InsertManyAsync(orders, new InsertManyOptions { IsOrdered = false });
    }

    // 批量更新用户
    public async Task BatchUpdateUsersAsync(IEnumerable<(FilterDefinition<User>, UpdateDefinition<User>)> updates)
    {
        if (updates == null || !updates.Any())
            return;

        var bulkOps = updates.Select(u => new UpdateOneModel<User>(u.Item1, u.Item2)).ToList();
        await _userCollection.BulkWriteAsync(bulkOps, new BulkWriteOptions { IsOrdered = false });
    }

    // 高效查询 - 使用索引和投影
    public async Task<List<User>> FindUsersWithProjectionAsync(int minAge)
    {
        var filter = Builders<User>.Filter.Gt(u => u.Age, minAge);
        var projection = Builders<User>.Projection.Include(u => u.Name).Include(u => u.Email).Include(u => u.Age);

        return await _userCollection.Find(filter).Project<User>(projection).ToListAsync();
    }

    // 聚合查询示例
    public async Task<List<BsonDocument>> GetOrderStatisticsAsync()
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$Status" },
                { "count", new BsonDocument("$sum", 1) },
                { "totalAmount", new BsonDocument("$sum", "$Amount") }
            }),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };

        return await _orderCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MongoDB 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddMongoDB("mongodb://localhost:27017", "myDatabase");
        services.AddSingleton<HighPerformanceMongoService>();
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取高性能服务
        var service = serviceProvider.GetRequiredService<HighPerformanceMongoService>();
        
        // 性能测试 - 批量插入
        Console.WriteLine("测试批量插入性能...");
        var users = new List<User>();
        for (int i = 0; i < 1000; i++)
        {
            users.Add(new User
            {
                Id = ObjectId.GenerateNewId(),
                Name = $"用户{i}",
                Email = $"user{i}@example.com",
                Age = 20 + (i % 30)
            });
        }
        
        var stopwatch = Stopwatch.StartNew();
        await service.BatchInsertUsersAsync(users);
        stopwatch.Stop();
        Console.WriteLine($"批量插入 1000 个用户耗时: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        
        // 性能测试 - 批量更新
        Console.WriteLine("测试批量更新性能...");
        var updates = new List<(FilterDefinition<User>, UpdateDefinition<User>)>();
        for (int i = 0; i < 100; i++)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Name, $"用户{i}");
            var update = Builders<User>.Update.Set(u => u.Age, 30);
            updates.Add((filter, update));
        }
        
        stopwatch.Restart();
        await service.BatchUpdateUsersAsync(updates);
        stopwatch.Stop();
        Console.WriteLine($"批量更新 100 个用户耗时: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        
        // 性能测试 - 投影查询
        Console.WriteLine("测试投影查询性能...");
        stopwatch.Restart();
        var filteredUsers = await service.FindUsersWithProjectionAsync(25);
        stopwatch.Stop();
        Console.WriteLine($"查询年龄大于 25 的用户耗时: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"找到 {filteredUsers.Count} 个用户");
        
        // 性能测试 - 聚合查询
        Console.WriteLine("测试聚合查询性能...");
        // 先插入一些订单数据
        var orders = new List<Order>();
        for (int i = 0; i < 500; i++)
        {
            orders.Add(new Order
            {
                Id = ObjectId.GenerateNewId(),
                UserId = users[i % 1000].Id,
                Amount = 100 + (i % 900),
                OrderDate = DateTime.Now.AddDays(-(i % 30)),
                Status = i % 3 == 0 ? "Pending" : i % 3 == 1 ? "Completed" : "Canceled"
            });
        }
        await service.BatchInsertOrdersAsync(orders);
        
        stopwatch.Restart();
        var stats = await service.GetOrderStatisticsAsync();
        stopwatch.Stop();
        Console.WriteLine($"聚合查询订单统计耗时: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        
        foreach (var stat in stats)
        {
            Console.WriteLine($"状态: {stat["_id"]}, 数量: {stat["count"]}, 总金额: {stat["totalAmount"]}");
        }
        
        Console.WriteLine("性能优化示例完成");
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Exceptions;

public class MongoDbErrorHandler
{
    private readonly IMongoRepository _repository;

    public MongoDbErrorHandler(IMongoRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleOperationWithRetryAsync(Func<Task> operation, int maxRetries = 3)
    {
        int retryCount = 0;
        while (true)
        {
            try
            {
                await operation();
                break;
            }
            catch (MongoConnectionException ex) when (retryCount < maxRetries)
            {
                retryCount++;
                Console.WriteLine($"连接错误 (重试 {retryCount}/{maxRetries}): {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(Math.Min(30, 2 * retryCount)));
            }
            catch (MongoCommandException ex) when (retryCount < maxRetries && IsRetryableCommandError(ex))
            {
                retryCount++;
                Console.WriteLine($"命令错误 (重试 {retryCount}/{maxRetries}): {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(Math.Min(30, 2 * retryCount)));
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"MongoDB 错误: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"一般错误: {ex.Message}");
                throw;
            }
        }
    }

    private bool IsRetryableCommandError(MongoCommandException ex)
    {
        // 检查是否是可重试的错误
        var errorCode = ex.Code;
        return errorCode == 50 || // Network error
               errorCode == 11600 || // Interrupted
               errorCode == 11602 || // Interrupted due to socket exception
               errorCode == 10107 || // Not master
               errorCode == 13435; // Not master and slave ok=false
    }

    // 安全的用户操作
    public async Task SafeUserOperationsAsync()
    {
        var user = new User
        {
            Id = ObjectId.GenerateNewId(),
            Name = "李四",
            Email = "lisi@example.com",
            Age = 25
        };

        // 安全插入
        await HandleOperationWithRetryAsync(async () => {
            await _repository.InsertAsync(user);
            Console.WriteLine($"用户 {user.Name} 安全插入成功");
        });

        // 安全查询
        await HandleOperationWithRetryAsync(async () => {
            var foundUser = await _repository.FindByIdAsync(user.Id);
            if (foundUser != null)
            {
                Console.WriteLine($"安全查询到用户: {foundUser.Name}");
            }
        });

        // 安全更新
        await HandleOperationWithRetryAsync(async () => {
            var foundUser = await _repository.FindByIdAsync(user.Id);
            if (foundUser != null)
            {
                foundUser.Age = 26;
                await _repository.UpdateAsync(foundUser);
                Console.WriteLine($"用户 {foundUser.Name} 安全更新成功");
            }
        });

        // 安全删除
        await HandleOperationWithRetryAsync(async () => {
            await _repository.DeleteAsync(user.Id);
            Console.WriteLine($"用户 {user.Name} 安全删除成功");
        });
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MongoDB 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddMongoDB("mongodb://localhost:27017", "myDatabase");
        services.AddSingleton<MongoDbErrorHandler>();
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取错误处理器
        var errorHandler = serviceProvider.GetRequiredService<MongoDbErrorHandler>();
        
        try
        {
            // 执行安全操作
            await errorHandler.SafeUserOperationsAsync();
            Console.WriteLine("错误处理示例完成");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"最终错误: {ex.Message}");
        }
    }
}
```

### 5. 事务示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;

// 产品模型
public class Product
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// 订单详情模型
public class OrderItem
{
    public ObjectId ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

// 事务服务
public class MongoTransactionService
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoTransactionService(IMongoClient client, IMongoDatabase database)
    {
        _client = client;
        _database = database;
    }

    // 执行订单事务
    public async Task<bool> ProcessOrderAsync(ObjectId userId, List<(ObjectId ProductId, int Quantity)> items)
    {
        using var session = await _client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var productCollection = _database.GetCollection<Product>("products", session);
            var orderCollection = _database.GetCollection<Order>("orders", session);

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            // 检查库存并更新
            foreach (var item in items)
            {
                var product = await productCollection.Find(p => p.Id == item.ProductId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception($"产品不存在: {item.ProductId}");
                }

                if (product.Stock < item.Quantity)
                {
                    throw new Exception($"产品 {product.Name} 库存不足");
                }

                // 更新库存
                var update = Builders<Product>.Update.Set(p => p.Stock, product.Stock - item.Quantity);
                await productCollection.UpdateOneAsync(p => p.Id == item.ProductId, update);

                // 添加到订单
                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity
                });

                totalAmount += product.Price * item.Quantity;
            }

            // 创建订单
            var order = new Order
            {
                Id = ObjectId.GenerateNewId(),
                UserId = userId,
                Amount = totalAmount,
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            await orderCollection.InsertOneAsync(order);

            // 提交事务
            await session.CommitTransactionAsync();
            Console.WriteLine($"订单处理成功，总金额: {totalAmount}");
            return true;
        }
        catch (Exception ex)
        {
            // 回滚事务
            await session.AbortTransactionAsync();
            Console.WriteLine($"订单处理失败: {ex.Message}");
            return false;
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MongoDB 事务示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddMongoDB("mongodb://localhost:27017", "myDatabase");
        services.AddSingleton<MongoTransactionService>();
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取服务
        var transactionService = serviceProvider.GetRequiredService<MongoTransactionService>();
        var database = serviceProvider.GetRequiredService<IMongoDatabase>();
        var productCollection = database.GetCollection<Product>("products");
        
        // 准备测试数据
        Console.WriteLine("准备测试数据...");
        var products = new List<Product>
        {
            new Product { Id = ObjectId.GenerateNewId(), Name = "产品1", Price = 100, Stock = 10 },
            new Product { Id = ObjectId.GenerateNewId(), Name = "产品2", Price = 200, Stock = 5 },
            new Product { Id = ObjectId.GenerateNewId(), Name = "产品3", Price = 50, Stock = 20 }
        };
        
        await productCollection.InsertManyAsync(products);
        foreach (var p in products)
        {
            Console.WriteLine($"- {p.Name}, 价格: {p.Price}, 库存: {p.Stock}");
        }
        
        // 测试事务 - 正常订单
        Console.WriteLine("\n测试正常订单...");
        var userId = ObjectId.GenerateNewId();
        var orderItems = new List<(ObjectId ProductId, int Quantity)>
        {
            (products[0].Id, 2),
            (products[1].Id, 1)
        };
        
        var success = await transactionService.ProcessOrderAsync(userId, orderItems);
        
        // 检查库存更新
        Console.WriteLine("\n检查库存更新...");
        var updatedProducts = await productCollection.Find(_ => true).ToListAsync();
        foreach (var p in updatedProducts)
        {
            Console.WriteLine($"- {p.Name}, 库存: {p.Stock}");
        }
        
        // 测试事务 - 库存不足
        Console.WriteLine("\n测试库存不足的订单...");
        var orderItemsInsufficient = new List<(ObjectId ProductId, int Quantity)>
        {
            (products[0].Id, 10) // 超出库存
        };
        
        success = await transactionService.ProcessOrderAsync(userId, orderItemsInsufficient);
        
        // 检查库存未被更新
        Console.WriteLine("\n检查库存未被更新...");
        var finalProducts = await productCollection.Find(_ => true).ToListAsync();
        foreach (var p in finalProducts)
        {
            Console.WriteLine($"- {p.Name}, 库存: {p.Stock}");
        }
        
        Console.WriteLine("事务示例完成");
    }
}
```

## 总结

以上示例展示了 MongoDB 智能体技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速上手基本操作**：了解如何进行 MongoDB 文档的增删改查
2. **配置高级选项**：学习如何优化 MongoDB 连接和配置
3. **优化性能**：掌握批量操作、索引使用、投影查询和聚合查询等性能优化技巧
4. **处理错误情况**：了解如何优雅地处理 MongoDB 错误和实现重试机制
5. **使用事务**：学习如何在 MongoDB 中使用事务确保数据一致性

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。通过 AOT 编译优化，进一步提高了应用程序的性能和部署便捷性。
