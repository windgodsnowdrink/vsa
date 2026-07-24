#:sdk Microsoft.NET.Sdk
#:package Ardalis.Specification@6.1.0
#:package Ardalis.Specification.EntityFrameworkCore@6.1.0
#:package System.Linq.Dynamic.Core@1.3.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;

// 1. 动态查询构建器接口
public interface IDynamicQueryBuilder<T>
{
    IDynamicQueryBuilder<T> Where(string predicate, params object[] args);
    IDynamicQueryBuilder<T> OrderBy(string ordering);
    IDynamicQueryBuilder<T> ThenBy(string ordering);
    IDynamicQueryBuilder<T> Skip(int count);
    IDynamicQueryBuilder<T> Take(int count);
    IQueryable<T> Build(IQueryable<T> source);
}

// 2. 高性能动态查询构建器实现
public class DynamicQueryBuilder<T> : IDynamicQueryBuilder<T>
{
    private readonly List<string> _whereClauses = new();
    private readonly List<string> _orderClauses = new();
    private int? _skip;
    private int? _take;

    private readonly List<string> _includes = new();
    private readonly List<(string, object[])> _parameterizedWheres = new();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public IDynamicQueryBuilder<T> Where(string predicate, params object[] args)
    {
        _whereClauses.Add(predicate);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDynamicQueryBuilder<T> OrderBy(string ordering)
    {
        _orderClauses.Add(ordering);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDynamicQueryBuilder<T> ThenBy(string ordering)
    {
        if (_orderClauses.Count == 0)
            throw new InvalidOperationException("Must call OrderBy before ThenBy");
        
        _orderClauses.Add(ordering);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDynamicQueryBuilder<T> Skip(int count)
    {
        _skip = count;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDynamicQueryBuilder<T> Take(int count)
    {
        _take = count;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDynamicQueryBuilder<T> Include(string navigationPropertyPath)
    {
        _includes.Add(navigationPropertyPath);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public IDynamicQueryBuilder<T> WhereParameterized(string predicate, params object[] args)
    {
        _parameterizedWheres.Add((predicate, args));
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public IQueryable<T> Build(IQueryable<T> source)
    {
        // 应用Where条件
        foreach (var clause in _whereClauses)
        {
            source = source.Where(clause);
        }

        // 应用参数化Where条件
        foreach (var (predicate, args) in _parameterizedWheres)
        {
            source = source.Where(predicate, args);
        }

        // 应用Include导航属性
        foreach (var include in _includes)
        {
            source = source.Include(include);
        }
        
        // 应用排序
        if (_orderClauses.Count > 0)
        {
            var orderBy = _orderClauses[0];
            var thenBy = _orderClauses.Count > 1 
                ? string.Join(",", _orderClauses.Skip(1)) 
                : null;
            
            source = source.OrderBy(orderBy);
            if (!string.IsNullOrEmpty(thenBy))
            {
                source = ((IOrderedQueryable<T>)source).ThenBy(thenBy);
            }
        }

        // 应用分页
        if (_skip.HasValue)
        {
            source = source.Skip(_skip.Value);
        }

        if (_take.HasValue)
        {
            source = source.Take(_take.Value);
        }

        return source;
    }
}

// DTO到实体规约转换器
public class DtoToSpecConverter<TEntity, TDto>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ISpecification<TEntity> Convert(
        TDto dto, 
        Action<IDynamicQueryBuilder<TEntity>> config)
    {
        var builder = new AdvancedQueryBuilder<TEntity>();
        
        // 自动映射DTO属性到查询条件
        foreach (var prop in typeof(TDto).GetProperties())
        {
            var value = prop.GetValue(dto);
            if (value != null)
            {
                builder.WhereParameterized($"{prop.Name} == @0", value);
            }
        }
        
        // 应用自定义配置
        config?.Invoke(builder);
        
        return new DynamicSpecification<TEntity>(builder);
    }
}

// 3. 与规范模式集成
public class DynamicSpecification<T> : Specification<T>
{
    private readonly IDynamicQueryBuilder<T> _queryBuilder;

    public DynamicSpecification(IDynamicQueryBuilder<T> queryBuilder)
    {
        _queryBuilder = queryBuilder;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override IQueryable<T> GetQuery(IQueryable<T> query)
    {
        return _queryBuilder.Build(query);
    }
}

// 4. 高性能查询服务
public class QueryService<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly ObjectPool<IDynamicQueryBuilder<T>> _queryBuilderPool;

    public QueryService(DbContext dbContext)
    {
        _dbContext = dbContext;
        _queryBuilderPool = new DefaultObjectPool<IDynamicQueryBuilder<T>>(
            new QueryBuilderPoolPolicy<T>(), 
            Environment.ProcessorCount * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<List<T>> QueryAsync(Action<IDynamicQueryBuilder<T>> configure)
    {
        var builder = _queryBuilderPool.Get();
        try
        {
            configure(builder);
            var spec = new DynamicSpecification<T>(builder);
            return await _dbContext.Set<T>()
                .WithSpecification(spec)
                .ToListAsync();
        }
        finally
        {
            _queryBuilderPool.Return(builder);
        }
    }

    private class QueryBuilderPoolPolicy<T> : IPooledObjectPolicy<IDynamicQueryBuilder<T>>
    {
        public IDynamicQueryBuilder<T> Create() => new DynamicQueryBuilder<T>();
        public bool Return(IDynamicQueryBuilder<T> obj) => true;
    }
}

// 5. 使用示例
public static class QueryBuilderDemo
{
    public record ProductQueryDto(
        string Name, 
        decimal? MinPrice, 
        decimal? MaxPrice,
        string CategoryName);

    public static async Task RunAsync()
    {
        var dbContext = new AppDbContext();
        var queryService = new QueryService<Product>(dbContext);
        var converter = new DtoToSpecConverter<Product, ProductQueryDto>();

        // 使用DTO构建查询
        var dto = new ProductQueryDto(
            Name: "Pro", 
            MinPrice: 100,
            MaxPrice: 1000,
            CategoryName: "Electronics");
        
        var spec = converter.Convert(dto, builder => 
        {
            builder.Include("Category")
                   .Include("Supplier")
                   .OrderBy("Price desc")
                   .Skip(10)
                   .Take(20);
        });

        var convertResults = await dbContext.Products
            .WithSpecification(spec)
            .ToListAsync();

        // 动态构建查询
        var results = await queryService.QueryAsync(builder =>
        {
            builder.Where("Price > @0", 100)
                   .Where("Name.Contains(@0)", "Premium")
                   .OrderBy("Price desc")
                   .ThenBy("Name")
                   .Skip(10)
                   .Take(20);
        });

        // 更复杂的动态查询
        var complexResults = await queryService.QueryAsync(builder =>
        {
            var minPrice = 50;
            var maxPrice = 200;
            var searchTerm = "Pro";
            
            builder.Where($"Price >= {minPrice} && Price <= {maxPrice}")
                   .Where($"Name.StartsWith(\"{searchTerm}\")")
                   .OrderBy("CategoryId")
                   .ThenBy("Price");
        });
    }
}