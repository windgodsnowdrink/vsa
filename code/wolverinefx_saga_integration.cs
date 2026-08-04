#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx@1.10.0
#:package WolverineFx.EntityFrameworkCore@1.10.0
#:package Microsoft.EntityFrameworkCore@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0

using Wolverine;
using Wolverine.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Saga状态实体
public class SagaState
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}

// Saga数据库上下文
public class SagaDbContext : DbContext
{
    public DbSet<SagaState> SagaStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SagaState>().HasKey(x => x.CorrelationId);
    }
}

// Saga步骤接口
public interface ISagaStep<TCommand>
{
    Task Execute(TCommand command, SagaState state, CancellationToken ct);
    Task Compensate(TCommand command, SagaState state, CancellationToken ct);
}

// 具体Saga步骤实现
public class CreateOrderStep : ISagaStep<CreateOrderCommand>
{
    private readonly OrderDbContext _dbContext;

    public CreateOrderStep(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Execute(CreateOrderCommand command, SagaState state, CancellationToken ct)
    {
        var order = new Order(command.UserId, command.Amount);
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task Compensate(CreateOrderCommand command, SagaState state, CancellationToken ct)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.UserId == command.UserId && o.Amount == command.Amount, ct);
        
        if (order != null)
        {
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

// Wolverine Saga协调器
public abstract class SagaCoordinator<TCommand>
{
    private readonly SagaDbContext _dbContext;
    private readonly IMessageBus _bus;
    private readonly IEnumerable<ISagaStep<TCommand>> _steps;

    protected SagaCoordinator(
        SagaDbContext dbContext, 
        IMessageBus bus,
        IEnumerable<ISagaStep<TCommand>> steps)
    {
        _dbContext = dbContext;
        _bus = bus;
        _steps = steps;
    }

    public async Task Handle(TCommand command, CancellationToken ct)
    {
        var state = new SagaState
        {
            CorrelationId = Guid.NewGuid(),
            CurrentState = "Started",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.SagaStates.Add(state);
        await _dbContext.SaveChangesAsync(ct);

        try
        {
            foreach (var step in _steps)
            {
                await step.Execute(command, state, ct);
                state.CurrentState = $"{step.GetType().Name}Completed";
                await _dbContext.SaveChangesAsync(ct);
            }

            state.IsCompleted = true;
            state.CompletedAt = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(ct);
        }
        catch
        {
            // 执行补偿
            foreach (var step in _steps.Reverse())
            {
                try
                {
                    await step.Compensate(command, state, ct);
                    state.CurrentState = $"{step.GetType().Name}Compensated";
                    await _dbContext.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    // 记录补偿失败
                    state.CurrentState = $"{step.GetType().Name}CompensationFailed";
                    await _dbContext.SaveChangesAsync(ct);
                    throw new SagaCompensationException(step.GetType().Name, ex);
                }
            }
            throw;
        }
    }
}

// Wolverine Saga状态机
public class OrderProcessingSaga : Wolverine.Saga
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid InventoryId { get; set; }
    public bool PaymentCompleted { get; set; }
    public bool InventoryReserved { get; set; }

    public void Start(OrderCreated created)
    {
        OrderId = created.OrderId;
        CurrentState = "Processing";
    }

    public void Handle(PaymentCompleted completed)
    {
        PaymentCompleted = true;
        PaymentId = completed.PaymentId;
        CurrentState = PaymentCompleted && InventoryReserved ? "Completed" : "Processing";
    }

    public void Handle(InventoryReserved reserved)
    {
        InventoryReserved = true;
        InventoryId = reserved.InventoryId;
        CurrentState = PaymentCompleted && InventoryReserved ? "Completed" : "Processing";
    }

    public void Handle(OrderFailed failed)
    {
        CurrentState = "Compensating";
        
        if (PaymentCompleted)
            Send(new RefundPaymentCommand(PaymentId));
            
        if (InventoryReserved)
            Send(new ReleaseInventoryCommand(InventoryId));
    }
}

// 命令和事件
public record OrderCreated(Guid OrderId);
public record ProcessPaymentCommand(Guid OrderId);
public record PaymentCompleted(Guid OrderId, Guid PaymentId);
public record ReserveInventoryCommand(Guid OrderId);
public record InventoryReserved(Guid OrderId, Guid InventoryId);
public record OrderFailed(Guid OrderId);
public record RefundPaymentCommand(Guid PaymentId);
public record ReleaseInventoryCommand(Guid InventoryId);

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWolverineSaga(this IServiceCollection services)
    {
        services.AddDbContext<SagaDbContext>();
        
        // 注册所有Saga步骤
        services.Scan(scan => scan
            .FromAssemblyOf<CreateOrderStep>()
            .AddClasses(c => c.AssignableTo(typeof(ISagaStep<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddWolverine(opts =>
        {
            opts.UseEntityFrameworkCoreTransactionsWith<SagaDbContext>();
            
            // 配置重试策略
            opts.Policies.OnException<Exception>()
                .RetryWithCooldown(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 5);
                
            opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
        });
        
        return services;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();
builder.Services.AddWolverineSaga();

var app = builder.Build();
app.Run();