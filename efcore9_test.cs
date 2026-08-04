#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore@9.0.8
#:package Microsoft.EntityFrameworkCore.SqlServer@9.0.8
#:package Microsoft.EntityFrameworkCore.Tools@9.0.8
#:package Microsoft.Extensions.Caching.Memory@8.0.8
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
// performance_interceptor_integration

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// DI 注册选项模式
builder.Services.AddOptions<MySettings>()
       .BindConfiguration()
       .Validate();

// DbContext池化
const string connString = "DataSource=QueryFilterSample.db";
var connectionString = "Server=server;Database=db;User Id=user;Password=pass;Min Pool Size=5;Max Pool Size=100;Connection Timeout=30;Pooling=true;MultipleActiveResultSets=true";
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.CommandTimeout(30);
    options.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null);
    options.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)) // 全局拆分查询配置
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)) // ReadOnlyDbContext只读上下文的全局无跟踪配置
        .AddInterceptors(new PerformanceInterceptor(logger)));
    options.EnableSensitiveDataLogging(false); // 生产环境中禁用
    options.EnableServiceProviderCaching();
}, poolSize: 128); // 大多数场景的最佳池大小

await using var services = builder.Services
    .AddLogging(lb => lb.AddDefaultDelegateLogger())
    .AddDbContext<QueryFilterContext>((provider, options) =>
    {
        options.UseSqlite(connString);
    })
    .BuildServiceProvider();
using var scope = services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<QueryFilterContext>();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
{
    context.Posts.Add(new BlogPost
    {
        Title = "test",
        UpdatedAt = DateTimeOffset.Now,
        UpdatedBy = "test"
    });
    context.Posts.Add(new BlogPost
    {
        Title = "[Disabled]test",
        UpdatedAt = DateTimeOffset.Now,
        UpdatedBy = "test"
    });
    context.Posts.Add(new BlogPost
    {
        Title = "[Deleted]test",
        UpdatedAt = DateTimeOffset.Now,
        UpdatedBy = "test"
    });
    await context.SaveChangesAsync();
}
{
    var posts = await context.Posts.AsNoTracking().ToArrayAsync(); // 第一次查询会把两个 filter 都带上
    // .IgnoreQueryFilters().ToArrayAsync(); // 默认会忽略所有的 query filter
    // .IgnoreQueryFilters(["non-deleted"]).ToArrayAsync(); // 忽略了指定的 filter non-deleted,而non-disabled没有被忽略
    Console.WriteLine(posts.Length);
    Console.WriteLine();
}

var app = builder.Build();

// 极简API管道模式 (Minimal API Pipeline Pattern)
// 路由组
var products = app.MapGroup("/products")
    .WithTags("Products")
    .WithOpenApi();
// 模块化和集中管理终结点
products.MapGet("/", GetAllProductsAsync);
products.MapPost("/", CreateProductAsync)
     .RequireAuthorization()
     .WithValidator<CreateUserRequest>();

app.Run();

// 选项模式(Options Pattern)与源生成(Source Generation)
// source generator生成
[JsonSerializable(typeof(List<ProductDto>))]
[JsonSerializable(typeof(ProductDto))]
internal partial class AppJsonSerializerContext: JsonSerializerContext
{
}

[OptionsBuilder("MySettings")]
[OptionsValidator]
internal partial class MySettings
{
    public required string ApiKey { get; init; }
    public int Timeout { get; set; } = 30;
}


// 产品服务
public class ProductService
{
    // C# 14 开始可以将实例构造函数和事件声明为部分成员（partial members）,包含一个定义声明和一个实现声明
    public partial ProductService() { }

    private static readonly Func<AppDbContext, Task<List<ProductDto>>> _getAllProductsCompiled =
        EF.CompileAsyncQuery((AppDbContext ctx) =>
            ctx.Products
                .AsNoTracking()
                .Select(p => new ProductDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                }));

    public async Task<List<ProductDto>> GetAllProductsAsync()
        => await _getAllProductsCompiled(_context);

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        // EF Core 9 中的优化 JSON 查询
        // 从 C# 14 开始，可以在不指定参数类型的情况下，向 lambda 表达式参数添加参数修饰符，例如：scoped、ref、in、out或 ref readonly.
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.Metadata.Tags.Contains("electronics"))
            .Where(p => p.Metadata.Attributes["warranty"] == "2years")
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Tags = p.Metadata.Tags,
                HasWarranty = p.Metadata.Attributes.ContainsKey("warranty")
            })
            .ToListAsync();
        }
    }

    // 异步编译查询
    public class OrderService
{
    private static readonly Func<AppDbContext, DateTime, CancellationToken, Task<List<Order>>> _getRecentOrdersAsync =
        EF.CompileAsyncQuery(
            (AppDbContext ctx, DateTime fromDate, CancellationToken ct) =>
                ctx.Orders
                   .Where(o => o.CreatedDate >= fromDate)
                   .Include(o => o.OrderItems)
                   .OrderByDescending(o => o.CreatedDate));


    public async Task<List<Order>> GetRecentOrdersAsync(DateTime fromDate, CancellationToken ct = default)
        => await _getRecentOrdersAsync(_context, fromDate, ct);

    // 事务
    public async Task<Result> ProcessOrderAsync(OrderRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 批量处理多个操作
            var order = new Order(request);
            context.Orders.Add(order);

            // 在事务中使用批量操作
            await _context.Products
                .Where(p => request.ProductIds.Contains(p.Id))
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.Stock, x => x.Stock - 1));

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Success();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

// 基本编译查询实现
public class ProductRepository
{
    // 静态编译查询，实现最大程度的复用
    private static readonly Func<AppDbContext, int, Product?> _getProductById =
        EF.CompileQuery((AppDbContext ctx, int id) =>
            ctx.Products
               // .Include(p => p.Category)
               .FirstOrDefault(p => p.Id == id));


    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public Product? GetProductById(int id) => _getProductById(_context, id);

    // 无跟踪
    // 拆分查询 - 多个优化的查询
    // 投影查询
    public List<ProductDto> GetList()
    {
        var products = await _context.Products
        .AsNoTracking()
        // .Include(p => p.Category)
        .AsSplitQuery()
        .Select(x => new ProductDto { Id = x.Id, })
        .ToListAsync();
    }

    // 批量更新
    public async Task<bool> BatchUpdate(Guid id)
    {
        await context.Products
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.Price, x => x.Price * 1.1m));
        return true;
    }

    // 批量删除
    public async Task<bool> BatchDelete(Guid[] ids)
    {
        await context.Orders
            .Where(o => o.CreatedDate < DateTime.Now.AddYears(-2))
            .ExecuteDeleteAsync();
        return true;
    }

    // 分页
    public async Task<PaginatedResult<ProductDto>> GetProductsAsync(int? lastProductId = null, int pageSize = 50)
    {
        var query = context.Products.AsNoTracking();

        if (lastProductId.HasValue)
        {
            query = query.Where(p => p.Id > lastProductId.Value);
        }

        var products = await query
            .OrderBy(p => p.Id)
            .Take(pageSize + 1) // 多取一条以检查是否有更多数据
            .Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
            })
            .ToListAsync();

        var hasMore = products.Count > pageSize;
        if (hasMore)
        {
            products.RemoveAt(pageSize);
        }

        returnnew PaginatedResult<ProductDto>
        {
            Items = products,
            HasMore = hasMore,
            LastId = products.LastOrDefault()?.Id,
        }
        ;
    }

    // 基于偏移量的分页
    public async Task<PagedResult<T>> GetPagedAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }
}

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Order> Orders; { get; set; } = null;
    public DbSet<Product> Products { get; set; } = null;
    // 配置
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .OwnsOne(p => p.Metadata, metadata =>
            {
                metadata.ToJson(); // 存储为 JSON 列
                metadata.OwnsOne(m => m.ShippingAddress);
            });
    }
}

file sealed class QueryFilterContext(DbContextOptions<QueryFilterContext> options) : DbContext(options)
{
    public DbSet<BlogPost> Posts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 可以配置多个 query filter 并指定名称
        modelBuilder.Entity<BlogPost>()
            .HasQueryFilter("non-deleted", p => !p.Title.StartsWith("[Deleted]"))
            .HasQueryFilter("non-disabled", p => !p.Title.StartsWith("[Disabled]"));
        base.OnModelCreating(modelBuilder);
    }
}

public class Product { public Guid Id { get; set; } public string Name { get; set; } public double Price { get; set; } public int Stock { get; set; } public List<OrderItem> OrderItems { get; set; } public DateTime CreatedDate { get; set; } public ProductMetadata Metadata { get; set; } }
public class ProductMetadata { public List<string> Tags { get; set; } public Dictionary<string, object> Attributes { get; set; } public Address ShippingAddress { get; set; } }
public record ProductDto { public Guid Id { get; set; } public string Name { get; set; } public double Price { get; set; } public List<string> Tags { get; set; } public bool HasWarranty { get; set; } }
public class Order { public Guid Id { get; set; } public DateTime CreatedDate { get; set; } }
public class OrderItem { public Guid Id { get; set; } }
public class Product { 
    public int Id { get; set; }
    [StringLength(64)]
    public required string Title { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    [StringLength(64)]
    public required string UpdatedBy { get; set; }
}
public record PaginatedResult<T> { public List<T> Items { get; set; } = []; public bool HasMore { get; set; } public Guid? LastId { get; set; } }
public record PagedResult<T> { public List<T> Items { get; set; } = []; public int TotalCount { get; set; } public int PageNumber { get; set; } public int PageSize { get; set; } }
public class Result { public bool Success() { return true; } }
public record OrderRequest { public List<Guid> ProductIds { get; => field = value ?? throw new ArgumentNullException(nameof(value)); } } // field 关键字

// 扩展成员（Extension Members）
public static class MyExtensions
{
    extension(string str)
    {
        public int WordCount() =>
            str.Split([' ', '.', '?'], StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

// Null 条件成员访问运算符“?.”和“?[]”现在可在赋值或复合赋值的左侧使用
// userInfo?.Name = CalculateAge(userInfo); // 运算符 = 的右侧仅在左侧不为 null 时才会被计算