#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web #:package MongoDB.Driver@2.28.0 #:package MongoDB.Bson@2.28.0 #:package Microsoft.Extensions.DependencyInjection@10.0.0 #:package Microsoft.Extensions.Logging@10.0.0 #:property LangVersion=preview #:property TargetFramework=net10.0 #:property Nullable=enable #:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// MongoDB 集成服务
public static class MongoDBIntegration
{
    public static IServiceCollection AddMongoDB(this IServiceCollection services, string connectionString, string databaseName)
    {
        // 注册 MongoDB 客户端
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        
        // 注册 MongoDB 数据库
        services.AddSingleton<IMongoDatabase>(sp => 
            sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        
        // 注册通用存储库
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        
        // 注册 MongoDB 服务
        services.AddSingleton<IMongoService, MongoService>();
        
        return services;
    }
}

// 通用 MongoDB 存储库接口
public interface IMongoRepository<T>
{
    Task InsertAsync(T document);
    Task<T> FindByIdAsync(ObjectId id);
    Task UpdateAsync(T document);
    Task DeleteAsync(ObjectId id);
    Task<List<T>> FindAllAsync();
    Task<List<T>> FindAsync(FilterDefinition<T> filter);
}

// 通用 MongoDB 存储库实现
public class MongoRepository<T> : IMongoRepository<T>
{
    private readonly IMongoCollection<T> _collection;
    private readonly ILogger<MongoRepository<T>> _logger;

    public MongoRepository(IMongoDatabase database, ILogger<MongoRepository<T>> logger)
    {
        _collection = database.GetCollection<T>(typeof(T).Name.ToLower() + "s");
        _logger = logger;
    }

    public async Task InsertAsync(T document)
    {
        try
        {
            await _collection.InsertOneAsync(document);
            _logger.LogInformation($"插入文档成功: {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"插入文档失败: {typeof(T).Name}");
            throw;
        }
    }

    public async Task<T> FindByIdAsync(ObjectId id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"查询文档失败: {typeof(T).Name}");
            throw;
        }
    }

    public async Task UpdateAsync(T document)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq("_id", GetId(document));
            await _collection.ReplaceOneAsync(filter, document);
            _logger.LogInformation($"更新文档成功: {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"更新文档失败: {typeof(T).Name}");
            throw;
        }
    }

    public async Task DeleteAsync(ObjectId id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
            _logger.LogInformation($"删除文档成功: {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"删除文档失败: {typeof(T).Name}");
            throw;
        }
    }

    public async Task<List<T>> FindAllAsync()
    {
        try
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"查询所有文档失败: {typeof(T).Name}");
            throw;
        }
    }

    public async Task<List<T>> FindAsync(FilterDefinition<T> filter)
    {
        try
        {
            return await _collection.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"条件查询文档失败: {typeof(T).Name}");
            throw;
        }
    }

    private ObjectId GetId(T document)
    {
        var property = typeof(T).GetProperty("Id");
        if (property != null)
        {
            return (ObjectId)property.GetValue(document);
        }
        throw new InvalidOperationException($"Document {typeof(T).Name} does not have an Id property");
    }
}

// MongoDB 服务接口
public interface IMongoService
{
    Task<string> HealthCheckAsync();
    Task<List<string>> GetCollectionsAsync();
    Task DropCollectionAsync(string collectionName);
}

// MongoDB 服务实现
public class MongoService : IMongoService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoService> _logger;

    public MongoService(IMongoDatabase database, ILogger<MongoService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<string> HealthCheckAsync()
    {
        try
        {
            // 测试连接
            await _database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            _logger.LogInformation("MongoDB health check passed");
            return "MongoDB connection is healthy";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MongoDB health check failed");
            return $"MongoDB connection failed: {ex.Message}";
        }
    }

    public async Task<List<string>> GetCollectionsAsync()
    {
        try
        {
            var collections = await _database.ListCollectionNamesAsync();
            var collectionNames = new List<string>();
            await foreach (var name in collections)
            {
                collectionNames.Add(name);
            }
            _logger.LogInformation($"Found {collectionNames.Count} collections");
            return collectionNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get collections");
            throw;
        }
    }

    public async Task DropCollectionAsync(string collectionName)
    {
        try
        {
            await _database.DropCollectionAsync(collectionName);
            _logger.LogInformation($"Dropped collection: {collectionName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to drop collection: {collectionName}");
            throw;
        }
    }
}

// 示例模型
public class User
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("MongoDB Integration Service");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 添加日志
        services.AddLogging(builder => 
            builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 添加 MongoDB 集成
        services.AddMongoDB("mongodb://localhost:27017", "myDatabase");
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 测试 MongoDB 服务
        var mongoService = serviceProvider.GetRequiredService<IMongoService>();
        
        // 健康检查
        var healthStatus = await mongoService.HealthCheckAsync();
        Console.WriteLine($"Health Check: {healthStatus}");
        
        // 获取集合
        var collections = await mongoService.GetCollectionsAsync();
        Console.WriteLine($"Collections: {string.Join(", ", collections)}");
        
        // 测试用户存储库
        var userRepository = serviceProvider.GetRequiredService<IMongoRepository<User>>();
        
        // 创建用户
        var user = new User
        {
            Id = ObjectId.GenerateNewId(),
            Name = "张三",
            Email = "zhangsan@example.com",
            Age = 30
        };
        
        await userRepository.InsertAsync(user);
        Console.WriteLine($"Created user: {user.Name}");
        
        // 查询用户
        var foundUser = await userRepository.FindByIdAsync(user.Id);
        if (foundUser != null)
        {
            Console.WriteLine($"Found user: {foundUser.Name}, Email: {foundUser.Email}");
        }
        
        // 更新用户
        foundUser.Age = 31;
        await userRepository.UpdateAsync(foundUser);
        Console.WriteLine($"Updated user age to: {foundUser.Age}");
        
        // 查询所有用户
        var allUsers = await userRepository.FindAllAsync();
        Console.WriteLine($"Total users: {allUsers.Count}");
        foreach (var u in allUsers)
        {
            Console.WriteLine($"- {u.Name}, Age: {u.Age}");
        }
        
        // 删除用户
        await userRepository.DeleteAsync(user.Id);
        Console.WriteLine($"Deleted user: {user.Name}");
        
        Console.WriteLine("\nMongoDB Integration Test Complete");
    }
}
