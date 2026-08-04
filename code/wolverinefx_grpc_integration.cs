#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx@1.10.0
#:package WolverineFx.Grpc@1.10.0
#:package DTM.Client@2.6.0
#:package DTM.Grpc@2.6.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable

using Wolverine;
using Grpc.Core;
using Grpc.AspNetCore.Server;
using Microsoft.Extensions.DependencyInjection;

// 1. 定义gRPC服务契约
[GrpcService(Name = "WolverineGateway")]
public interface IWolverineGateway
{
    Task<GrpcResponse> ExecuteCommand(GrpcRequest request);
}

// 2. 定义gRPC请求/响应模型
public class GrpcRequest
{
    public string CommandType { get; set; }
    public string CommandData { get; set; }
}

public class GrpcResponse
{
    public bool Success { get; set; }
    public string ResponseData { get; set; }
}

// 3. 实现gRPC服务
public class WolverineGatewayService : IWolverineGateway
{
    private readonly IMessageBus _bus;
    private readonly ISerializer _serializer;

    public WolverineGatewayService(IMessageBus bus, ISerializer serializer)
    {
        _bus = bus;
        _serializer = serializer;
    }

    public async Task<GrpcResponse> ExecuteCommand(GrpcRequest request)
    {
        try
        {
            // 反序列化命令
            var commandType = Type.GetType(request.CommandType);
            var command = _serializer.Deserialize(request.CommandData, commandType);
            
            // 使用Wolverine的消息总线发送命令
            var result = await _bus.InvokeAsync<object>(command);
            
            return new GrpcResponse
            {
                Success = true,
                ResponseData = _serializer.Serialize(result)
            };
        }
        catch (Exception ex)
        {
            return new GrpcResponse
            {
                Success = false,
                ResponseData = ex.Message
            };
        }
    }
}

// 4. 分布式事务拦截器
public class DistributedTransactionInterceptor : Interceptor
{
    private readonly IDtmClient _dtmClient;
    
    public DistributedTransactionInterceptor(IDtmClient dtmClient)
    {
        _dtmClient = dtmClient;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var transactionId = context.RequestHeaders
            .FirstOrDefault(h => h.Key == "dtm-transaction-id")?.Value;

        if (!string.IsNullOrEmpty(transactionId))
        {
            await _dtmClient.RegisterBranchAsync(
                transactionId,
                context.Method,
                JsonSerializer.Serialize(request));

            try
            {
                var result = await continuation(request, context);
                await _dtmClient.ReportBranchResultAsync(transactionId, true);
                return result;
            }
            catch
            {
                await _dtmClient.ReportBranchResultAsync(transactionId, false);
                throw;
            }
        }

        return await continuation(request, context);
    }
}

// 5. DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWolverineGrpcGateway(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionInterceptor>();
            options.Interceptors.Add<DistributedTransactionInterceptor>();
        });

        services.AddDtmClient(options =>
        {
            options.DtmUrl = "http://localhost:36789";
            options.BranchName = "wolverine-grpc-service";
        });

        services.AddSingleton<IWolverineGateway, WolverineGatewayService>();
        services.AddSingleton<ISerializer, JsonSerializer>();
        
        return services;
    }
}

// 6. 异常拦截器
public class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}

// 7. 启动配置
var builder = WebApplication.CreateBuilder();

// 配置Wolverine
builder.Host.UseWolverine(opts =>
{
    // 启用gRPC集成
    opts.UseGrpc();
    
    // 配置重试策略
    opts.Policies.RetryOnException<Exception>()
        .MaximumAttempts(3)
        .PauseBetweenAttempts(250.Milliseconds());
    
    // 配置死信队列
    opts.Policies.OnException<Exception>()
        .MoveToErrorQueue("dead-letters");
});

builder.Services.AddWolverineGrpcGateway();

var app = builder.Build();
app.MapGrpcService<WolverineGatewayService>();
app.Run();

// 8. 使用示例 - 跨服务事务
public async Task<GrpcResponse> TransferFunds(GrpcRequest request)
{
    var transactionId = Guid.NewGuid().ToString();
    
    // 创建DTM事务
    await _dtmClient.ExecuteTransactionAsync(transactionId, async (dtmClient) =>
    {
        // 调用账户服务
        await _bus.InvokeAsync(new DebitAccountCommand());
        
        // 通过gRPC调用支付服务
        var paymentRequest = new GrpcRequest
        {
            CommandType = typeof(ProcessPaymentCommand).AssemblyQualifiedName,
            CommandData = JsonSerializer.Serialize(new ProcessPaymentCommand())
        };
        
        var headers = new Metadata
        {
            { "dtm-transaction-id", transactionId }
        };
        
        await _paymentClient.ExecuteCommandAsync(paymentRequest, headers);
    });
    
    return new GrpcResponse { Success = true };
}