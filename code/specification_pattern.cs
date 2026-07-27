#:sdk Microsoft.NET.Sdk
#:package System.Linq.Async@6.0.1
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using System.Linq.Expressions;

// 基础规约接口
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }
    IReadOnlyCollection<string> IncludeStrings { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    Expression<Func<T, object>> GroupBy { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}

// 基础规约实现
public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public Expression<Func<T, object>> GroupBy { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression) =>
        Includes.Add(includeExpression);

    protected virtual void AddInclude(string includeString) =>
        IncludeStrings.Add(includeString);

    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) =>
        OrderBy = orderByExpression;

    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression) =>
        OrderByDescending = orderByDescExpression;

    protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression) =>
        GroupBy = groupByExpression;

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}

// 规约求值器
public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(
        IQueryable<T> inputQuery,
        ISpecification<T> specification)
    {
        var query = inputQuery;

        // 过滤条件
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // 包含导航属性
        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        // 包含字符串导航属性
        query = specification.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        // 排序
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // 分组
        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
        }

        // 分页
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}