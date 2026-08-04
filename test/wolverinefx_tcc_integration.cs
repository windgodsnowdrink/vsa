#:sdk Microsoft.NET.Sdk.Web
#:package Dtmcli@2.6.0
#:package WolverineFx@2.7.0
#:property TargetFramework=net11.0
#:property Nullable=enable

using Dtmcli;
using Wolverine;

// TCC事务协调器
public class TccCoordinator : IMiddleware
{
    private readonly DtmClient _dtmClient;
    private readonly ILogger<TccCoordinator> _logger;

    public TccCoordinator(DtmClient dtmClient, ILogger<TccCoordinator> logger)
    {
        _dtmClient = dtmClient;
        _logger = logger;
    }

    public async Task BeforeAsync(IMessageContext context, CancellationToken cancellationToken)
    {
        if (context.Envelope.Message is not ITccTransaction tccRequest)
            return;

        var gid = await _dtmClient.GenGid();
        tccRequest.SetGid(gid);

        try
        {
            // Try阶段
            await _dtmClient.TccGlobalTransaction(gid, async (tcc) =>
            {
                await tccRequest.TryPhase();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TCC事务执行失败");
            throw;
        }
    }

    public Task AfterAsync(IMessageContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

// TCC事务接口
public interface ITccTransaction
{
    string Gid { get; }
    void SetGid(string gid);
    Task TryPhase();
    Task ConfirmPhase();
    Task CancelPhase();
}

// 示例：订单创建TCC处理器
public class CreateOrderTccHandler : ITccTransaction
{
    public string Gid { get; private set; } = null!;
    
    public void SetGid(string gid) => Gid = gid;

    public async Task Handle(CreateOrderCommand command)
    {
        // 主业务逻辑
    }

    public async Task TryPhase()
    {
        // 预留资源
    }

    public async Task ConfirmPhase()
    {
        // 确认资源
    }

    public async Task CancelPhase()
    {
        // 取消预留
    }
}

// DI扩展
public static class WolverineDependencyInjectionExtensions
{
    public static WolverineOptions UseDtmTccSupport(this WolverineOptions options, IConfiguration configuration)
    {
        options.Services.AddSingleton<DtmClient>(sp => 
            new DtmClient(sp.GetRequiredService<IConfiguration>()[\"Dtm:Url\"]));
            
        options.Handlers.AddMiddleware<TccCoordinator>();
        return options;
    }
}