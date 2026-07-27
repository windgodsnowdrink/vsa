#:sdk Microsoft.NET.Sdk
#:package Microsoft.EntityFrameworkCore@8.0.4
#:package Dapper@2.0.123
#:package Dapper.Contrib@2.0.123
#:property LangVersion preview
#:property TargetFramework net11.0

public interface IReadOnly{entityName}Repository
{
    Task<{entityName}> GetByIdAsync(int id);
    Task<IEnumerable<{entityName}>> GetAllAsync();
}

public class ReadOnly{entityName}Repository : IReadOnly{entityName}Repository
{
    private readonly ReadOnlyDbContext _context;

    public ReadOnly{entityName}Repository(ReadOnlyDbContext context)
    {
        _context = context;
    }

    public async Task<{entityName}> GetByIdAsync(int id) => 
        await _context.Set<{entityName}>().FindAsync(id);

    public async Task<IEnumerable<{entityName}>> GetAllAsync() => 
        await _context.Set<{entityName}>().ToListAsync();
}

public class ReadWriteDbContext : DbContext
{
    public DbSet<{entityName}> {entityName}s { get; set; }
    
    public ReadWriteDbContext(DbContextOptions<ReadWriteDbContext> options)
        : base(options) { }
}

public class ReadOnlyDbContext : DbContext
{
    public DbSet<{entityName}> {entityName}s { get; set; }
    
    public ReadOnlyDbContext(DbContextOptions<ReadOnlyDbContext> options)
        : base(options) { }
}

// 添加Dapper高性能查询实现
public class DapperReadOnlyRepository : IReadOnly{entityName}Repository
{
    private readonly IDbConnection _connection;

    public DapperReadOnlyRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<{entityName}> GetByIdAsync(int id)
    {
        // 使用Dapper的零拷贝查询
        using var command = new CommandDefinition(
            "SELECT * FROM {entityName}s WHERE Id = @id",
            new { id },
            flags: CommandFlags.NoCache);
        
        return await _connection.QueryFirstOrDefaultAsync<{entityName}>(command);
    }

    public async Task<IEnumerable<{entityName}>> GetAllAsync()
    {
        // 批量查询优化
        using var command = new CommandDefinition(
            "SELECT * FROM {entityName}s",
            flags: CommandFlags.Pipelined);
            
        return await _connection.QueryAsync<{entityName}>(command);
    }
}