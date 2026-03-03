# db - 使用示例

## AOT 架构示例

### 1. 基本数据库操作示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Db.AOT;

public class BasicDatabaseExample
{
    public static async Task Run()
    {
        Console.WriteLine("数据库基本操作示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机和服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置数据库选项
        builder.Configuration.AddJsonFile("db_aot.setting.json", optional: true);
        builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
        
        // 注册数据库服务
        builder.Services.AddDatabase();
        
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取数据库引擎
        var engine = serviceProvider.GetRequiredService<DatabaseAotEngine>();
        
        // 创建测试表
        Console.WriteLine("1. 创建测试表...");
        var createTableResult = await engine.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Email TEXT NOT NULL,
            Age INTEGER,
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
        )");
        Console.WriteLine($"   结果: {(createTableResult.Success ? "成功" : "失败")}");
        
        // 插入数据
        Console.WriteLine("\n2. 插入测试数据...");
        var insertResult = await engine.ExecuteNonQueryAsync(
            "INSERT INTO Users (Name, Email, Age) VALUES (@Name, @Email, @Age)",
            new { Name = "张三", Email = "zhangsan@example.com", Age = 30 }
        );
        Console.WriteLine($"   结果: {(insertResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   插入行数: {insertResult.ResultData}");
        
        // 查询数据
        Console.WriteLine("\n3. 查询数据...");
        var queryResult = await engine.ExecuteQueryAsync<dynamic>("SELECT * FROM Users");
        Console.WriteLine($"   结果: {(queryResult.Success ? "成功" : "失败")}");
        if (queryResult.Success && queryResult.ResultData != null)
        {
            Console.WriteLine($"   查询到 {queryResult.ResultData.Count} 条记录:");
            foreach (var user in queryResult.ResultData)
            {
                Console.WriteLine($"   - Id: {user.Id}, Name: {user.Name}, Email: {user.Email}, Age: {user.Age}");
            }
        }
        
        // 更新数据
        Console.WriteLine("\n4. 更新数据...");
        var updateResult = await engine.ExecuteNonQueryAsync(
            "UPDATE Users SET Age = @Age WHERE Name = @Name",
            new { Age = 31, Name = "张三" }
        );
        Console.WriteLine($"   结果: {(updateResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   更新行数: {updateResult.ResultData}");
        
        // 删除数据
        Console.WriteLine("\n5. 删除数据...");
        var deleteResult = await engine.ExecuteNonQueryAsync(
            "DELETE FROM Users WHERE Name = @Name",
            new { Name = "张三" }
        );
        Console.WriteLine($"   结果: {(deleteResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   删除行数: {deleteResult.ResultData}");
        
        Console.WriteLine("\n数据库基本操作示例完成！");
    }
}
```

### 2. 事务处理示例

```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Db.AOT;
using Dapper;

public class TransactionExample
{
    public static async Task Run()
    {
        Console.WriteLine("数据库事务处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DatabaseOptions>(options =>
        {
            options.DatabaseType = DatabaseType.Sqlite;
            options.ConnectionString = "Data Source=transaction_test.db";
            options.EnableTransactions = true;
        });
        builder.Services.AddDatabase();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DatabaseAotEngine>();
        
        // 创建测试表
        await engine.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS Accounts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Balance DECIMAL(18,2) NOT NULL DEFAULT 0
        )");
        
        // 插入测试数据
        await engine.ExecuteNonQueryAsync("INSERT OR IGNORE INTO Accounts (Id, Name, Balance) VALUES (1, '账户A', 1000)");
        await engine.ExecuteNonQueryAsync("INSERT OR IGNORE INTO Accounts (Id, Name, Balance) VALUES (2, '账户B', 500)");
        
        Console.WriteLine("事务开始前的账户余额:");
        var accountsBefore = await engine.ExecuteQueryAsync<dynamic>("SELECT * FROM Accounts");
        foreach (var account in accountsBefore.ResultData)
        {
            Console.WriteLine($"   {account.Name}: {account.Balance}");
        }
        
        // 定义事务中的操作
        var transferAmount = 200m;
        var actions = new List<Func<IDbConnection, Task<bool>>>();
        
        // 1. 从账户A扣除金额
        actions.Add(async (connection) =>
        {
            var result = await connection.ExecuteAsync(
                "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @Id AND Balance >= @Amount",
                new { Amount = transferAmount, Id = 1 }
            );
            return result > 0;
        });
        
        // 2. 向账户B添加金额
        actions.Add(async (connection) =>
        {
            var result = await connection.ExecuteAsync(
                "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @Id",
                new { Amount = transferAmount, Id = 2 }
            );
            return result > 0;
        });
        
        // 执行事务
        Console.WriteLine($"\n执行转账事务: 从账户A转移 {transferAmount} 到账户B...");
        var transactionResult = await engine.ExecuteTransactionAsync(actions);
        Console.WriteLine($"   事务结果: {(transactionResult.Success ? "成功" : "失败")}");
        
        // 查看事务后的账户余额
        Console.WriteLine("\n事务结束后的账户余额:");
        var accountsAfter = await engine.ExecuteQueryAsync<dynamic>("SELECT * FROM Accounts");
        foreach (var account in accountsAfter.ResultData)
        {
            Console.WriteLine($"   {account.Name}: {account.Balance}");
        }
        
        Console.WriteLine("\n数据库事务处理示例完成！");
    }
}
```

### 3. 批量操作示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Db.AOT;

public class BatchOperationExample
{
    public static async Task Run()
    {
        Console.WriteLine("数据库批量操作示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DatabaseOptions>(options =>
        {
            options.DatabaseType = DatabaseType.Sqlite;
            options.ConnectionString = "Data Source=batch_test.db";
            options.BatchSize = 500; // 设置批量操作大小
        });
        builder.Services.AddDatabase();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DatabaseAotEngine>();
        
        // 创建测试表
        await engine.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS Products (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Category TEXT NOT NULL,
            Price DECIMAL(18,2) NOT NULL,
            Stock INTEGER NOT NULL
        )");
        
        // 生成大量测试数据
        Console.WriteLine("1. 生成测试数据...");
        var products = new List<Product>();
        for (int i = 1; i <= 1000; i++)
        {
            products.Add(new Product
            {
                Name = $"产品{i}",
                Category = i % 3 == 0 ? "电子产品" : i % 3 == 1 ? "服装" : "食品",
                Price = i * 1.5m,
                Stock = i * 10
            });
        }
        Console.WriteLine($"   生成了 {products.Count} 个产品数据");
        
        // 执行批量插入
        Console.WriteLine("\n2. 执行批量插入...");
        var batchResult = await engine.BulkInsertAsync("Products", products);
        Console.WriteLine($"   批量插入结果: {(batchResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   插入行数: {batchResult.ResultData}");
        Console.WriteLine($"   执行时间: {batchResult.ExecutionTimeMs} ms");
        
        // 验证插入结果
        Console.WriteLine("\n3. 验证插入结果...");
        var countResult = await engine.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Products");
        Console.WriteLine($"   表中总记录数: {countResult.ResultData}");
        
        Console.WriteLine("\n数据库批量操作示例完成！");
    }
    
    private class Product
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
```

### 4. 多数据库支持示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Db.AOT;

public class MultiDatabaseExample
{
    public static async Task Run()
    {
        Console.WriteLine("多数据库支持示例");
        Console.WriteLine("=" * 50);
        
        // SQLite 示例
        Console.WriteLine("\n--- SQLite 数据库示例 ---");
        await RunDatabaseExample(DatabaseType.Sqlite, "Data Source=sqlite_example.db");
        
        // 其他数据库类型可以类似配置
        // MySQL 示例
        // await RunDatabaseExample(DatabaseType.MySql, "Server=localhost;Database=mysql_example;User=root;Password=password;");
        
        // PostgreSQL 示例  
        // await RunDatabaseExample(DatabaseType.PostgreSql, "Host=localhost;Database=pgsql_example;Username=postgres;Password=password;");
        
        Console.WriteLine("\n多数据库支持示例完成！");
    }
    
    private static async Task RunDatabaseExample(DatabaseType dbType, string connectionString)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DatabaseOptions>(options =>
        {
            options.DatabaseType = dbType;
            options.ConnectionString = connectionString;
        });
        builder.Services.AddDatabase();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DatabaseAotEngine>();
        
        // 创建测试表并插入数据
        await engine.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS Test (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Value TEXT NOT NULL
        )");
        
        await engine.ExecuteNonQueryAsync(
            "INSERT INTO Test (Value) VALUES (@Value)",
            new { Value = $"来自{dbType}的数据" }
        );
        
        // 查询数据
        var result = await engine.ExecuteQueryAsync<dynamic>("SELECT * FROM Test");
        if (result.Success && result.ResultData != null)
        {
            Console.WriteLine($"   查询结果 ({dbType}):");
            foreach (var item in result.ResultData)
            {
                Console.WriteLine($"   - Id: {item.Id}, Value: {item.Value}");
            }
        }
    }
}
```

### 5. 状态监控示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Db.AOT;

public class StatusMonitoringExample
{
    public static async Task Run()
    {
        Console.WriteLine("数据库状态监控示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DatabaseOptions>(options =>
        {
            options.DatabaseType = DatabaseType.Sqlite;
            options.ConnectionString = "Data Source=status_monitor.db";
            options.EnablePerformanceMonitoring = true;
        });
        builder.Services.AddDatabase();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DatabaseAotEngine>();
        
        // 执行一些数据库操作以生成状态数据
        Console.WriteLine("1. 执行一些数据库操作...");
        for (int i = 0; i < 5; i++)
        {
            await engine.ExecuteNonQueryAsync(
                "CREATE TABLE IF NOT EXISTS TestTable_" + i + " (Id INTEGER PRIMARY KEY, Name TEXT)");
            await engine.ExecuteNonQueryAsync(
                "INSERT INTO TestTable_" + i + " (Name) VALUES (@Name)",
                new { Name = "Test" + i });
            await engine.ExecuteQueryAsync<dynamic>("SELECT * FROM TestTable_" + i);
        }
        
        // 获取数据库状态
        Console.WriteLine("\n2. 获取数据库状态...");
        var status = await engine.GetStatusAsync();
        
        Console.WriteLine($"   服务运行状态: {(status.IsRunning ? "正常" : "异常")}");
        Console.WriteLine($"   数据库类型: {status.DatabaseType}");
        Console.WriteLine($"   连接字符串: {status.ConnectionString}");
        Console.WriteLine($"   已处理请求数: {status.ProcessedRequests}");
        Console.WriteLine($"   成功请求数: {status.SuccessfulRequests}");
        Console.WriteLine($"   失败请求数: {status.FailedRequests}");
        Console.WriteLine($"   平均执行时间: {status.AverageExecutionTimeMs} ms");
        Console.WriteLine($"   服务启动时间: {status.StartTime.ToLocalTime()}");
        Console.WriteLine($"   当前活动连接数: {status.ActiveConnections}/{status.MaxPoolSize}");
        
        // 重置状态
        Console.WriteLine("\n3. 重置数据库状态...");
        var resetResult = await engine.ResetStatusAsync();
        Console.WriteLine($"   重置结果: {(resetResult ? "成功" : "失败")}");
        
        // 再次获取状态，验证重置结果
        Console.WriteLine("\n4. 重置后的状态:");
        var statusAfterReset = await engine.GetStatusAsync();
        Console.WriteLine($"   已处理请求数: {statusAfterReset.ProcessedRequests}");
        Console.WriteLine($"   成功请求数: {statusAfterReset.SuccessfulRequests}");
        Console.WriteLine($"   失败请求数: {statusAfterReset.FailedRequests}");
        
        Console.WriteLine("\n数据库状态监控示例完成！");
    }
}
```

## 总结

以上示例展示了基于 .NET 10 AOT 架构的数据库技能的主要功能和使用方法：

1. **基本数据库操作**：创建表、插入、查询、更新、删除数据
2. **事务处理**：确保多个数据库操作的原子性
3. **批量操作**：高效处理大量数据
4. **多数据库支持**：支持 SQLite、MySQL、PostgreSQL 等多种数据库
5. **状态监控**：实时监控数据库服务状态和性能指标

该系统设计遵循了 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。通过 AOT 编译技术，提供了极致的性能和启动速度，非常适合需要高性能数据库操作的场景。

所有示例代码都可以直接在支持 .NET 10 的环境中运行，只需要配置相应的数据库连接字符串和选项即可。