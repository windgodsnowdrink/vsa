# PagedList - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 注册 PagedList 服务
        services.AddPagedListServices(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.EnableCache = true;
            options.CacheDuration = TimeSpan.FromSeconds(300);
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 准备数据源
        var source = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
        
        // 获取分页数据
        var pageNumber = 1;
        var pageSize = 20;
        var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        
        // 使用分页结果
        Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
        Console.WriteLine($"总页数: {pagedList.PageCount}");
        Console.WriteLine($"当前页: {pagedList.PageNumber}");
        Console.WriteLine($"每页大小: {pagedList.PageSize}");
        Console.WriteLine($"是否有上一页: {pagedList.HasPreviousPage}");
        Console.WriteLine($"是否有下一页: {pagedList.HasNextPage}");
        
        // 遍历当前页数据
        Console.WriteLine("\n当前页数据:");
        foreach (var item in pagedList)
        {
            Console.WriteLine($"{item.Id}: {item.Name}");
        }
        
        Console.WriteLine("\n基本使用示例完成！");
    }
}
```

### 2. 高级配置示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        // 配置 PagedList 设置
        services.Configure<PagedListOptions>(options => {
            options.DefaultPageSize = 50;
            options.MaxPageSize = 200;
            options.EnableCache = true;
            options.CacheDuration = TimeSpan.FromMinutes(10);
            options.EnablePerformanceMetrics = true;
            options.EnableZeroCopy = true;
            options.ThreadLocalCacheSize = 2048;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;
            options.EnableBatching = true;
            options.BatchSize = 200;
        });
        
        // 注册 PagedList 服务
        services.AddPagedListServices();
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<PagedListOptions>>().Value;
        Console.WriteLine($"配置: DefaultPageSize={settings.DefaultPageSize}, MaxPageSize={settings.MaxPageSize}");
        Console.WriteLine($"缓存: EnableCache={settings.EnableCache}, CacheDuration={settings.CacheDuration}");
        Console.WriteLine($"性能: EnableParallelProcessing={settings.EnableParallelProcessing}, MaxDegreeOfParallelism={settings.MaxDegreeOfParallelism}");
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 准备数据源
        var source = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
        
        // 获取分页数据
        var pageNumber = 1;
        var pageSize = 50;
        var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        
        // 使用分页结果
        Console.WriteLine($"\n总记录数: {pagedList.TotalItemCount}");
        Console.WriteLine($"总页数: {pagedList.PageCount}");
        Console.WriteLine($"当前页数据量: {pagedList.Count}");
        
        Console.WriteLine("\n高级配置示例完成！");
    }
}
```

### 3. 性能优化示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 配置 PagedList 性能优化设置
        services.AddPagedListServices(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.EnableCache = true;
            options.CacheDuration = TimeSpan.FromMinutes(5);
            options.EnablePerformanceMetrics = true;
            options.EnableZeroCopy = true;
            options.ThreadLocalCacheSize = 1024;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 准备测试数据
        var source = Enumerable.Range(1, 10000).Select(i => new { Id = i, Name = $"Item {i}", Description = $"Description for item {i}" }).AsQueryable();
        
        // 性能测试
        const int iterations = 100;
        var stopwatch = new Stopwatch();
        
        // 测试第一次请求（无缓存）
        Console.WriteLine("测试第一次请求（无缓存）...");
        stopwatch.Start();
        var pageNumber = 1;
        var pageSize = 20;
        var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        stopwatch.Stop();
        Console.WriteLine($"第一次请求时间: {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
        
        // 测试第二次请求（有缓存）
        Console.WriteLine("\n测试第二次请求（有缓存）...");
        stopwatch.Reset();
        stopwatch.Start();
        pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        stopwatch.Stop();
        Console.WriteLine($"第二次请求时间: {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
        
        // 测试多次请求
        Console.WriteLine($"\n测试 {iterations} 次请求...");
        stopwatch.Reset();
        stopwatch.Start();
        
        for (int i = 0; i < iterations; i++)
        {
            // 随机页码
            var randomPage = new Random().Next(1, 10);
            await pagedListService.GetPagedListAsync(source, randomPage, pageSize);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"{iterations} 次请求总时间: {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
        Console.WriteLine($"平均每次请求时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2}ms");
        
        Console.WriteLine("\n性能优化示例完成！");
    }
}
```

### 4. 错误处理示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 注册 PagedList 服务
        services.AddPagedListServices(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.EnableCache = true;
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 测试 1: 无效的页码
        Console.WriteLine("\n测试 1: 无效的页码");
        try
        {
            var source = Enumerable.Range(1, 100).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
            var pagedList = await pagedListService.GetPagedListAsync(source, 0, 20);
            Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        // 测试 2: 无效的每页大小
        Console.WriteLine("\n测试 2: 无效的每页大小");
        try
        {
            var source = Enumerable.Range(1, 100).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
            var pagedList = await pagedListService.GetPagedListAsync(source, 1, 0);
            Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        // 测试 3: 空数据源
        Console.WriteLine("\n测试 3: 空数据源");
        try
        {
            var source = Enumerable.Empty<object>().AsQueryable();
            var pagedList = await pagedListService.GetPagedListAsync(source, 1, 20);
            Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
            Console.WriteLine($"总页数: {pagedList.PageCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        // 测试 4: 正常情况
        Console.WriteLine("\n测试 4: 正常情况");
        try
        {
            var source = Enumerable.Range(1, 100).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
            var pagedList = await pagedListService.GetPagedListAsync(source, 1, 20);
            Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
            Console.WriteLine($"总页数: {pagedList.PageCount}");
            Console.WriteLine($"当前页: {pagedList.PageNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        Console.WriteLine("\n错误处理示例完成！");
    }
}
```

## 集成示例

### 5. 与 ASP.NET Core 集成示例

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                {
                    // 注册 PagedList 服务
                    services.AddPagedListServices(options =>
                    {
                        options.DefaultPageSize = 20;
                        options.MaxPageSize = 100;
                        options.EnableCache = true;
                        options.CacheDuration = TimeSpan.FromMinutes(5);
                        options.EnablePerformanceMetrics = true;
                    });
                });
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/api/items", async context =>
                        {
                            // 获取查询参数
                            var pageNumber = int.TryParse(context.Request.Query["page"], out var p) ? p : 1;
                            var pageSize = int.TryParse(context.Request.Query["size"], out var s) ? s : 20;
                            
                            // 获取 PagedList 服务
                            var pagedListService = context.RequestServices.GetRequiredService<IPagedListService>();
                            
                            // 准备数据源
                            var source = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
                            
                            // 获取分页数据
                            var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
                            
                            // 构建响应
                            var response = new
                            {
                                TotalCount = pagedList.TotalItemCount,
                                PageCount = pagedList.PageCount,
                                PageNumber = pagedList.PageNumber,
                                PageSize = pagedList.PageSize,
                                HasPreviousPage = pagedList.HasPreviousPage,
                                HasNextPage = pagedList.HasNextPage,
                                Items = pagedList.ToList()
                            };
                            
                            // 返回 JSON 响应
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(response);
                        });
                    });
                });
            });
}
```

### 6. 与 Entity Framework Core 集成示例

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagedList;
using System;
using System.Linq;
using System.Threading.Tasks;

// 实体模型
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 数据库上下文
public class AppDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PagedListDemo;Trusted_Connection=True;");
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 与 Entity Framework Core 集成示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 注册数据库上下文
        services.AddDbContext<AppDbContext>();
        
        // 注册 PagedList 服务
        services.AddPagedListServices(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.EnableCache = true;
            options.CacheDuration = TimeSpan.FromMinutes(5);
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 初始化数据库
        await InitializeDatabase(serviceProvider);
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 使用 Entity Framework Core 查询
        using var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        
        // 测试 1: 基本查询
        Console.WriteLine("\n测试 1: 基本查询");
        var pageNumber = 1;
        var pageSize = 20;
        var pagedList = await pagedListService.GetPagedListAsync(
            dbContext.Items.OrderBy(i => i.Id), pageNumber, pageSize);
        
        Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
        Console.WriteLine($"总页数: {pagedList.PageCount}");
        Console.WriteLine($"当前页数据量: {pagedList.Count}");
        
        // 测试 2: 带条件查询
        Console.WriteLine("\n测试 2: 带条件查询");
        var filteredPagedList = await pagedListService.GetPagedListAsync(
            dbContext.Items.Where(i => i.Name.Contains("Item 1")).OrderBy(i => i.Id), pageNumber, pageSize);
        
        Console.WriteLine($"总记录数: {filteredPagedList.TotalItemCount}");
        Console.WriteLine($"总页数: {filteredPagedList.PageCount}");
        
        // 测试 3: 排序查询
        Console.WriteLine("\n测试 3: 排序查询");
        var sortedPagedList = await pagedListService.GetPagedListAsync(
            dbContext.Items.OrderByDescending(i => i.CreatedAt), pageNumber, pageSize);
        
        Console.WriteLine($"总记录数: {sortedPagedList.TotalItemCount}");
        Console.WriteLine($"第一条记录: {sortedPagedList.First().Name}, 创建时间: {sortedPagedList.First().CreatedAt}");
        
        Console.WriteLine("\n与 Entity Framework Core 集成示例完成！");
    }
    
    private static async Task InitializeDatabase(IServiceProvider serviceProvider)
    {
        using var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        
        // 确保数据库已创建
        await dbContext.Database.EnsureCreatedAsync();
        
        // 检查是否已有数据
        if (!await dbContext.Items.AnyAsync())
        {
            // 添加测试数据
            Console.WriteLine("正在添加测试数据...");
            for (int i = 1; i <= 100; i++)
            {
                dbContext.Items.Add(new Item
                {
                    Id = i,
                    Name = $"Item {i}",
                    Description = $"Description for Item {i}",
                    CreatedAt = DateTime.Now.AddDays(-i)
                });
            }
            await dbContext.SaveChangesAsync();
            Console.WriteLine("测试数据添加完成！");
        }
    }
}
```

## 高级示例

### 7. 自定义分页服务示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// 自定义分页服务
public class CustomPagedListService : IPagedListService
{
    private readonly ILogger<CustomPagedListService> _logger;
    private readonly PagedListOptions _options;
    private readonly Dictionary<string, object> _customCache;
    
    public CustomPagedListService(ILogger<CustomPagedListService> logger, IOptions<PagedListOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _customCache = new Dictionary<string, object>();
    }
    
    public async Task<IPagedList<T>> GetPagedListAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"自定义分页服务: 页码 {pageNumber}, 每页大小 {pageSize}");
        
        // 验证参数
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = _options.DefaultPageSize;
        if (pageSize > _options.MaxPageSize) pageSize = _options.MaxPageSize;
        
        // 生成缓存键
        var cacheKey = $"CustomPagedList:{typeof(T).FullName}:{pageNumber}:{pageSize}:{source.GetHashCode()}";
        
        // 尝试从缓存获取
        if (_options.EnableCache && _customCache.TryGetValue(cacheKey, out var cachedResult))
        {
            _logger.LogDebug($"缓存命中: {cacheKey}");
            return (IPagedList<T>)cachedResult;
        }
        
        // 执行分页查询
        var pagedList = await Task.Run(() =>
        {
            // 自定义分页逻辑
            var totalCount = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new StaticPagedList<T>(items, pageNumber, pageSize, totalCount);
        }, cancellationToken);
        
        // 存入缓存
        if (_options.EnableCache)
        {
            _customCache[cacheKey] = pagedList;
            // 限制缓存大小
            if (_customCache.Count > _options.ThreadLocalCacheSize)
            {
                var oldestKey = _customCache.Keys.First();
                _customCache.Remove(oldestKey);
            }
        }
        
        return pagedList;
    }
    
    public async Task<IPagedList<T>> GetPagedListAsync<T>(IEnumerable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPagedListAsync(source.AsQueryable(), pageNumber, pageSize, cancellationToken);
    }
    
    public async Task<PagedResult<T>> GetPagedResultAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var pagedList = await GetPagedListAsync(source, pageNumber, pageSize, cancellationToken);
        return new PagedResult<T>(pagedList, _options);
    }
}

// 静态分页列表实现
public class StaticPagedList<T> : IPagedList<T>
{
    private readonly List<T> _items;
    
    public StaticPagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
    {
        _items = items.ToList();
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItemCount = totalCount;
    }
    
    public int Count => _items.Count;
    public int TotalItemCount { get; }
    public int PageCount => (int)Math.Ceiling(TotalItemCount / (double)PageSize);
    public int PageNumber { get; }
    public int PageSize { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < PageCount;
    public bool IsFirstPage => PageNumber == 1;
    public bool IsLastPage => PageNumber == PageCount;
    public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;
    public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalItemCount);
    
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 自定义分页服务示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 配置 PagedList 选项
        services.Configure<PagedListOptions>(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.EnableCache = true;
            options.ThreadLocalCacheSize = 100;
        });
        
        // 注册自定义分页服务
        services.AddSingleton<IPagedListService, CustomPagedListService>();
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取自定义分页服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 准备数据源
        var source = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
        
        // 获取分页数据
        var pageNumber = 1;
        var pageSize = 20;
        var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        
        // 使用分页结果
        Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
        Console.WriteLine($"总页数: {pagedList.PageCount}");
        Console.WriteLine($"当前页: {pagedList.PageNumber}");
        Console.WriteLine($"每页大小: {pagedList.PageSize}");
        
        // 遍历当前页数据
        Console.WriteLine("\n当前页数据:");
        foreach (var item in pagedList.Take(5))
        {
            Console.WriteLine($"{item.Id}: {item.Name}");
        }
        
        Console.WriteLine("\n自定义分页服务示例完成！");
    }
}
```

### 7. 批量处理示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 批量处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 注册 PagedList 服务
        services.AddPagedListServices(options =>
        {
            options.DefaultPageSize = 100;
            options.MaxPageSize = 500;
            options.EnableCache = true;
            options.CacheDuration = TimeSpan.FromMinutes(5);
            options.EnableBatching = true;
            options.BatchSize = 100;
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 准备大数据源
        Console.WriteLine("准备大数据源...");
        var source = Enumerable.Range(1, 10000).Select(i => new { Id = i, Name = $"Item {i}", Value = i * 2 }).AsQueryable();
        Console.WriteLine($"数据源大小: {source.Count()}");
        
        // 批量处理
        Console.WriteLine("\n开始批量处理...");
        var batchSize = 100;
        var currentPage = 1;
        var totalProcessed = 0;
        
        while (true)
        {
            // 获取当前批次数据
            var pagedList = await pagedListService.GetPagedListAsync(source, currentPage, batchSize);
            
            // 处理当前批次
            Console.WriteLine($"处理批次 {currentPage}, 记录数: {pagedList.Count}");
            
            // 模拟处理逻辑
            foreach (var item in pagedList)
            {
                // 这里可以添加实际的处理逻辑
                if (totalProcessed < 10) // 只显示前 10 条
                {
                    Console.WriteLine($"  处理: {item.Id}: {item.Name}, Value: {item.Value}");
                }
                totalProcessed++;
            }
            
            // 检查是否还有下一页
            if (!pagedList.HasNextPage)
            {
                break;
            }
            
            currentPage++;
        }
        
        Console.WriteLine($"\n批量处理完成！总处理记录数: {totalProcessed}");
    }
}
```

### 8. 缓存策略示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 缓存策略示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 注册 PagedList 服务（启用缓存）
        services.AddPagedListServices(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.EnableCache = true;
            options.CacheDuration = TimeSpan.FromSeconds(30); // 短缓存时间，便于测试
            options.ThreadLocalCacheSize = 100;
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 准备数据源
        var source = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();
        
        // 测试缓存效果
        var pageNumber = 1;
        var pageSize = 20;
        
        // 第一次请求（无缓存）
        Console.WriteLine("第一次请求（无缓存）...");
        var startTime = DateTime.UtcNow;
        var pagedList1 = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        var firstRequestTime = DateTime.UtcNow - startTime;
        Console.WriteLine($"第一次请求时间: {firstRequestTime.TotalMilliseconds:F2}ms");
        
        // 第二次请求（有缓存）
        Console.WriteLine("\n第二次请求（有缓存）...");
        startTime = DateTime.UtcNow;
        var pagedList2 = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        var secondRequestTime = DateTime.UtcNow - startTime;
        Console.WriteLine($"第二次请求时间: {secondRequestTime.TotalMilliseconds:F2}ms");
        
        // 验证是否为同一对象
        Console.WriteLine($"\n是否为同一对象: {ReferenceEquals(pagedList1, pagedList2)}");
        Console.WriteLine($"缓存是否生效: {secondRequestTime < firstRequestTime}");
        
        // 测试不同页码
        Console.WriteLine("\n测试不同页码...");
        pageNumber = 2;
        startTime = DateTime.UtcNow;
        var pagedList3 = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        var thirdRequestTime = DateTime.UtcNow - startTime;
        Console.WriteLine($"请求第 {pageNumber} 页时间: {thirdRequestTime.TotalMilliseconds:F2}ms");
        
        // 测试缓存过期
        Console.WriteLine("\n测试缓存过期...");
        Console.WriteLine("等待 35 秒让缓存过期...");
        await Task.Delay(35000);
        
        startTime = DateTime.UtcNow;
        var pagedList4 = await pagedListService.GetPagedListAsync(source, 1, pageSize);
        var fourthRequestTime = DateTime.UtcNow - startTime;
        Console.WriteLine($"缓存过期后请求时间: {fourthRequestTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"缓存是否过期: {fourthRequestTime > secondRequestTime}");
        
        Console.WriteLine("\n缓存策略示例完成！");
    }
}
```

### 9. 并行处理示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PagedList 并行处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 注册 PagedList 服务（启用并行处理）
        services.AddPagedListServices(options =>
        {
            options.DefaultPageSize = 100;
            options.MaxPageSize = 500;
            options.EnableCache = true;
            options.CacheDuration = TimeSpan.FromMinutes(5);
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 PagedList 服务
        var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
        
        // 准备大数据源
        Console.WriteLine("准备大数据源...");
        var source = Enumerable.Range(1, 100000).Select(i => new 
        {
            Id = i, 
            Name = $"Item {i}",
            Value = i * 2,
            Description = $"Description for item {i}"
        }).AsQueryable();
        Console.WriteLine($"数据源大小: {source.Count()}");
        
        // 测试并行处理
        Console.WriteLine("\n测试并行处理...");
        var pageNumber = 1;
        var pageSize = 1000;
        
        var startTime = DateTime.UtcNow;
        var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
        var elapsedTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"处理时间: {elapsedTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
        Console.WriteLine($"总页数: {pagedList.PageCount}");
        Console.WriteLine($"当前页数据量: {pagedList.Count}");
        
        // 测试多线程并行请求
        Console.WriteLine("\n测试多线程并行请求...");
        var tasks = new List<Task>();
        var threadCount = Environment.ProcessorCount;
        
        startTime = DateTime.UtcNow;
        for (int i = 0; i < threadCount; i++)
        {
            var taskPageNumber = i + 1;
            tasks.Add(Task.Run(async () =>
            {
                var result = await pagedListService.GetPagedListAsync(source, taskPageNumber, pageSize);
                Console.WriteLine($"线程 {Task.CurrentId} 完成页码 {taskPageNumber}, 记录数: {result.Count}");
            }));
        }
        
        await Task.WhenAll(tasks);
        elapsedTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"\n多线程处理时间: {elapsedTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"线程数: {threadCount}");
        
        Console.WriteLine("\n并行处理示例完成！");
    }
}
```

## 总结

以上示例展示了 PagedList 技能的主要功能和使用方法。通过这些示例，你可以：

1. 快速开始使用基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 与 ASP.NET Core 集成
6. 与 Entity Framework Core 集成
7. 实现自定义分页服务
8. 进行批量处理
9. 应用缓存策略
10. 利用并行处理提高性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

所有示例都使用了中文注释和说明，并且与我们之前实现的 PagedList 功能保持一致。如果你有任何问题或建议，请参考 SKILL.md 文件或联系 VSA Architecture Team。
