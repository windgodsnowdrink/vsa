#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package System.Threading.Channels@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
#:package Microsoft.OpenApi@1.6.25
#:package Microsoft.OpenApi.Readers@1.6.25
#:package Microsoft.AspNetCore.OpenApi@9.0.9
#:package Swashbuckle.AspNetCore@9.0.4
#:package Swashbuckle.AspNetCore.Swagger@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.4
#:package Scalar.AspNetCore@2.8.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scalar.AspNetCore;

// #:project ../vsa
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(); // /scalar

// DI ע��
builder.Services.AddOptions<MySettings>()
       .BindConfiguration()
       .Validate();
// ע����ʵ��
builder.Services.AddTransient<IPaymentProcessor, StripeProcessor>("Stripe");
builder.Services.AddTransient<IPaymentProcessor, PayPalProcessor>("PayPal");

// ׷�� (Tracing) ����
builder.Services.AddOpenTelemetryTracing(b =>
    b.AddAspNetCoreInstrumentation()
     .AddHttpClientInstrumentation()
     .AddSource("MyApp"));

// ����ͨ��(Channel)��֪ͨģʽ (Channel-Based Notification Pattern)ģʽ����
// Channel<T> �������첽��ʽ������Ϣ�����ṩ��Ȼ�ı�ѹ��back-pressure������
var channel = Channel.CreateUnbounded<UserEvent>(); // �����¼���������̨��ҵ������ʽ�ܹ��ǳ���Ч

// ������ (Producer)
await channel.Writer.WriteAsync(new UserEvent("SignedIn"));

// ������ (Consumer)
await foreach (var evt in channel.Reader.ReadAllAsync())
    await HandleAsync(evt);

Console.WriteLine("From [CallerFilePath] attribute:");
Console.WriteLine($" - Entry-point path: {Path.EntryPointFilePath()}");
Console.WriteLine($" - Entry-point directory: {Path.EntryPointFileDirectoryPath()}");

Console.WriteLine("From AppContext data:");
Console.WriteLine($" - Entry-point path: {AppContext.EntryPointFilePath()}");
Console.WriteLine($" - Entry-point directory: {AppContext.EntryPointFileDirectoryPath()}");

var app = builder.Build();

var group = app.MapGroup("/group")
    .WithTags("Groups")
    .WithOpenApi();

// ����API�ܵ�ģʽ (Minimal API Pipeline Pattern)
// ���·����(Route Groups)�Ĺ��ܿ���ģʽ (Feature Flag Pattern)
group.MapGet("/", async () =>
{
    await foreach (var item in FetchItems())
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: {item}");
    }

    return Results.Ok();
});
//.RequireAuthorization("admin")
//.WithValidator<CreateUserRequest>();
// group.MapGet("/search", NewSearchHandler);

// �˵������ģʽ (Endpoint Filter Pattern)
app.MapPost("/orders", HandleOrder)
   .AddEndpointFilter(async (context, next) =>
   {
       var request = context.GetArgument<OrderRequest>(0);
       if (!IsValid(request)) return Results.BadRequest();

       return await next(context); // ����ִ��
   });

app.MapGet("/hello", () => "Hello world!");

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.Run();

static async IAsyncEnumerable<int> FetchItems()
{
    for (int i = 1; i <= 10; i++)
    {
        await Task.Delay(100);
        yield return i;
    }
}

static class PathEntryPointExtensions
{
    extension(Path)
    {
        public static string EntryPointFilePath() => EntryPointImpl();

    public static string EntryPointFileDirectoryPath() => Path.GetDirectoryName(EntryPointImpl()) ?? "";

    private static string EntryPointImpl([System.Runtime.CompilerServices.CallerFilePath] string filePath = "") => filePath;
}
}

static class AppContextExtensions
{
    extension(AppContext)
    {
        public static string? EntryPointFilePath() => AppContext.GetData("EntryPointFilePath") as string;
    public static string? EntryPointFileDirectoryPath() => AppContext.GetData("EntryPointFileDirectoryPath") as string;
}
}

// ѡ��ģʽ(Options Pattern)��Դ����(Source Generation)
[OptionsBuilder("MySettings")]
[OptionsValidator]
internal partial class MySettings
{
    public required string ApiKey { get; init; }
    public int Timeout { get; set; } = 30;
}

// �����������ģʽ (Strategy Pattern with Named Services)
public interface IPaymentProcessor
{
    Task ProcessAsync(Order order);
}

// MediatR �ܵ���Ϊģʽ (MediatR Pipeline Behavior)
public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        LogRequest(request);
        return await next();
    }
}

// �ṹ����־(Structured Logging) + OpenTelemetry �Ŀɹ۲���ģʽ (Observability)
[LoggerMessage(EventId = 200, Level = LogLevel.Information, Message = "User {UserId} logged in from {IpAddress}")]
    public static partial void UserLoggedIn(ILogger logger, string userId, string ip);

public class StripeProcessor : IPaymentProcessor
{
    public Task ProcessAsync(Order order) => Task.CompletedTask;
}

public class PayPalProcessor : IPaymentProcessor
{
    public Task ProcessAsync(Order order) => Task.CompletedTask;
}

// ��¼����(Records)����չ����(Extension Methods)�ĺ���ʽ��� (Functional Composition)
public record Order() { }
public record UserEvent(string str) { }
public record Customer(string Name, decimal Balance);
public static class CustomerExtensions
{
    public static Customer ApplyDiscount(this Customer c) =>
        c with { Balance = c.Balance * 0.9M };
    public static Customer AddLoyalty(this Customer c, int points) => c;
}
// ʹ��
// var updated = customer.ApplyDiscount().AddLoyalty(100);

public class CheckoutService
{
    private readonly Func<string, IPaymentProcessor> _resolve;
    public CheckoutService(Func<string, IPaymentProcessor> resolve) => _resolve = resolve;

    public Task CheckoutAsync(Order order, string provider) => _resolve(provider).ProcessAsync(order);
}
