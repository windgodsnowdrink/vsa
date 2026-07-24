#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package Microsoft.Extensions.Hosting@10.0.0-rc.1.25451.107
#:package System.Threading.Channels@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@9.0.9
#:package Microsoft.AspNetCore.DataProtection.StackExchangeRedis@9.0.9
#:package Microsoft.AspNetCore.SignalR.StackExchangeRedis@9.0.9
#:package Swashbuckle.AspNetCore@9.0.4
#:package Swashbuckle.AspNetCore.Swagger@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.4
#:package Scalar.AspNetCore@2.8.0
#:package Microsoft.AspNetCore.OpenApi@9.0.9
#:package Ardalis.ListStartupServices@1.1.4
#:package Serilog@4.3.0
#:package Serilog.AspNetCore@9.0.0
#:package Serilog.Enrichers.Environment@3.0.1
#:package Serilog.Enrichers.Process@3.0.0
#:package Serilog.Enrichers.Span@3.1.0
#:package Serilog.Enrichers.Thread@4.0.0
#:package Serilog.Exceptions@8.4.0
#:package Serilog.Extensions.Hosting@9.0.0
#:package Serilog.Extensions.Logging@9.0.2
#:package Serilog.Formatting.Compact@3.0.0
#:package Serilog.Settings.Configuration@9.0.0
#:package Serilog.Sinks.Async@2.1.0
#:package Serilog.Sinks.Console@6.0.0
#:package Serilog.Sinks.EventLog@4.0.0
#:package Serilog.Sinks.Http@9.2.0
#:package Serilog.Sinks.RollingFileAlternate@2.0.9
#:package Serilog.Sinks.Seq@9.0.0
#:package Serilog.Sinks.SpectreConsole@0.3.3
#:package Serilog.Sinks.Trace@4.0.0
#:package Carter@9.0.0
#:package Carter.Analyzers@9.0.0
#:package CarterTemplate@9.0.0
#:package Carter.SirenNegotiator@2.0.0
#:package Scrutor@6.1.0
#:package Scrutor.AspNetCore@3.3.0
#:package Mapster@7.4.0
#:package Mapster.Core@1.2.1
#:package Mapster.DependencyInjection@1.0.1
#:package Mapster.EFCore@5.1.1
#:package Mapster.Async@2.0.1
#:package Mapster.Immutable@1.0.1
#:package FluentValidation@12.0.0
#:package FluentValidation.AspNetCore@11.3.1
#:package FluentValidation.DependencyInjectionExtensions@12.0.0
#:package FluentValidation.ValidatorAttribute@8.6.1
#:package MediatR@12.4.1
#:package MediatR.Contracts@2.0.1
#:package MediatR.Extensions.FluentValidation.AspNetCore@5.1.0
#:package PublicTransit@8.5.2
#:package PublicTransit.RabbitMQ@8.5.2
#:package WolverineFx@4.11.0
#:package WolverineFx.Marten@4.11.0
#:package WolverineFx.RDBMS@4.11.0
#:package WolverineFx.Postgresql@4.11.0
#:package WolverineFx.FluentValidation@4.11.0
#:package WolverineFx.Http@4.11.0
#:package WolverineFx.RabbitMQ@4.11.0
#:package WolverineFx.AzureServiceBus@4.11.0
#:package WolverineFx.Http.FluentValidation@4.11.0
#:package WolverineFx.Http.Marten@4.11.0
#:package WolverineFx.AmazonSqs@4.11.0
#:package WolverineFx.EntityFrameworkCore@4.11.0
#:package WolverineFx.SqlServer@4.11.0
#:package WolverineFx.Kafka@4.11.0
#:package WolverineFx.MemoryPack@4.11.0
#:package WolverineFx.MessagePack@4.11.0
#:package WolverineFx.MQTT@4.11.0
#:package WolverineFx.Pubsub@4.11.0
#:package WolverineFx.Pulsar@4.11.0
#:package WolverineFx.RavenDb@4.11.0
#:package ModelContextProtocol@0.3.0-preview.4
#:package ModelContextProtocol.Core@0.3.0-preview.4
#:package Keycloak.AuthServices.Authentication@2.7.0
#:package Keycloak.AuthServices.Authorization@2.7.0
#:package Keycloak.AuthServices.Sdk@2.7.0
#:package Keycloak.AuthServices.Common@2.7.0
#:package Microsoft.EntityFrameworkCore.Design@10.0.0-rc.1.25451.107
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.22.1
#:package Npgsql.EntityFrameworkCore.PostgreSQL@10.0.0-rc.1
#:package Npgsql.EntityFrameworkCore.PostgreSQL.NodaTime@10.0.0-rc.1
#:package Npgsql.NodaTime@9.0.3
#:package Npgsql.DependencyInjection@9.0.3
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property GenerateDocumentationFile=true
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.SampleMcpServer
#:property PackageVersion=0.0.1-beta
#:property PackageTags=AI; MCP; server; stdio
#:property Description=An MCP server using the MCP C# SDK.
#:property DockerDefaultTargetOS=Linux
#:property DockerfileContext=.
#:property Dockerfile=Dockerfile
#:property DockerComposeProjectPath=docker-compose.dcproj

using App;
using App.Modules;
using App.Modules.Basket;
using App.Modules.Catalog;
using App.Modules.Ordering;
using App.Shared;
using Ardalis.ListStartupServices;
using Carter;
using FluentValidation;
using Keycloak;
using Mapster;
using Marten;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ModelContextProtocol.Server;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.AzureServiceBus;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Kafka;
using Wolverine.Marten;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100MB
    options.ListenLocalhost(5000); // http
    // options.ListenAnyIP(5000, listenOptions => listenOptions.UseConnectionLogging());
    options.ListenAnyIP(5001, o => o.UseHttps()); // https，若有证书
});

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;
builder.Logging.ClearProviders();               // 只保留下面的 ConsoleProvider，避免干扰
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);                   // 使用颜色化的控制台日志
builder.Logging.SetMinimumLevel(LogLevel.Trace); // 设为 Trace，最细粒度
builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.ServiceDiscovery", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.Resilience", LogLevel.Trace);
builder.Logging.AddFilter("RestEase.HttpClientFactory", LogLevel.Trace);
builder.Logging.AddFilter("App.ServiceDiscoveryHandler", LogLevel.Trace);
Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "error", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Fatal)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "fatal", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "info", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "warning", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog(Log.Logger, true).ConfigureLogging((context, logging) =>
{
    // config.ReadFrom.Configuration(context.Configuration));
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration);
    logging.AddSerilog(Log.Logger, true);
});
builder.Services.AddOptions();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination", "X-Platform-Type", "X-CSRF-TOKEN-HEADERNAME", "X-CSRF-TOKEN", "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Forwarded-Prefix", "X-Body-Hash")));

// 添加Carter模块
var catalogAssembly = typeof(App.Modules.Catalog.ServiceCollectionExtensions).Assembly;
var basketAssembly = typeof(App.Modules.Basket.ServiceCollectionExtensions).Assembly;
var orderingAssembly = typeof(App.Modules.Ordering.ServiceCollectionExtensions).Assembly;
builder.Services.AddCarterWithAssemblies(catalogAssembly, basketAssembly, orderingAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly, basketAssembly, orderingAssembly);
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("Redis"); });
builder.Services.AddMassTransitWithAssemblies(builder.Configuration, catalogAssembly, basketAssembly, orderingAssembly);
// builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCatalogModule(builder.Configuration).AddBasketModule(builder.Configuration).AddOrderingModule(builder.Configuration);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
// 添加OpenApi服务，这是Scalar所需的
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "My API",
            Version = "v1",
            Description = ".NET 10 MCPServer 集成测试"
        };
        return Task.CompletedTask;
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// 测试MCP
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<App.RandomNumberTools>();

builder.Services.Configure<Ardalis.ListStartupServices.ServiceConfig>(config =>
{
    config.Services = [.. builder.Services];
    config.Path = "/services";
});

builder.Services.AddTransient<IMyService, MyService>(); // 添加一个具体的服务实现
// builder.Services.Decorate<IMyService, LoggingDecorator>(); // 使用服务装饰添加行为
builder.Services.Decorate<IMyService>((inner, provider) => // 使用工厂方法添加行为
{
    var logger = provider.GetRequiredService<ILogger<LoggingDecorator>>();
    return new LoggingDecorator(inner, logger);
});
// 程序集扫描
builder.Services.Scan(scan => scan
    // .FromAssemblyOf<Program>()           // 扫描包含此接口的程序集
    // .FromApplicationDependencies()         // 从所有依赖项
    // .FromAssemblies(params Assembly[]) // 扫描明确指定的程序集
    .FromEntryAssembly()                   // 从入口程序集
    .AddClasses(classes => classes.AssignableTo<ITransientService>()).AsImplementedInterfaces().WithTransientLifetime()
    // .InNamespace("App") // 只在特定命名空间中
    // .WithAttribute<DependAttribute>() // 选择具有特定属性的类型
    // .WithAttribute<CompilerGeneratedAttribute>() // 包含编译器生成的类型
    // .WithoutAttribute<App.ObsoleteAttribute>()
    // .UsingAttributes() // 属性注入服务[ServiceDescriptor(typeof(IMyService), ServiceLifetime.Scoped)]
    .AddClasses(classes => classes.AssignableTo<IScopedService>()).AsImplementedInterfaces().WithScopedLifetime()
    // .AsSelf() // 将每个类型注册为其本身
    // .As<IScopedService>()              // 注册为特定类型
    // .AsSelfWithInterfaces() // 将每个类型同时注册为其本身及其接口
    // .AsMatchingInterface() // 将每个类型注册为其匹配的接口
    // .AsImplementedInterfaces(i => i.Name.StartsWith("I")) // 匹配谓词接口以 I 开头的接口
    // .WithLifetime(type => { if (type.Name.EndsWith("Repository")) return ServiceLifetime.Scoped; if (type.Name.EndsWith("Service")) return ServiceLifetime.Singleton; return ServiceLifetime.Transient;})
    // Append默认追加, Skip跳过, Replace替换,TryAdd 未注册添加, TryAddEnumerable 未注册添加到可枚举注册项
    .AddClasses(classes => classes.AssignableTo<ISingletonService>()).AsImplementedInterfaces().WithSingletonLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(IOpenGeneric<>))).AsImplementedInterfaces().WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(IMyService))).AsImplementedInterfaces().WithScopedLifetime() // 查找所有非抽象类
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>))).AsImplementedInterfaces().WithScopedLifetime() // 将它们注册为实现的接口
    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>))).AsImplementedInterfaces().WithTransientLifetime());// 使用瞬态生命周期
var app = builder.Build();
app.UseSerilogRequestLogging(opt =>
{
    opt.IncludeQueryInRequestPath = true;
});
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
// app.MapCarter();
app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot API V1.1.0.0");
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseAuthentication();
// app.UseAuthorization();
app.UseCatalogModule().UseBasketModule().UseOrderingModule();
app.MapOpenApi(); // 映射OpenApi文档路径,http://localhost:5000/openapi/v1.json
// app.MapOpenApi("/scalar/v1/openapi.json", "v1'); // http://localhost:5000/scalar/v1/openapi.json
app.MapScalarApiReference(); // 映射Scalar的API参考文档路径,http://localhost:5000/scalar
//app.MapScalarApiReference(options =>
//{
//    options.Path = "/scalar"; // UI 仍然是 /scalar
//    options.DocumentPath = "/scalar/v1/openapi.json"; // 告诉 UI 去哪里拿 OpenAPI 文档
//}); // http://localhost:5000/scalar/v1/openapi.json
app.UseShowAllServicesMiddleware();
app.MapGet("/", () => "Mcp Agent Integration!");
app.UseEndpoints(options =>
{
    options.MapControllers();
});
await app.RunAsync();

namespace App
{
    public partial class Program;

    /// <summary>
    /// Sample MCP tools for demonstration purposes.
    /// These tools can be invoked by MCP clients to perform various operations.
    /// </summary>
    internal class RandomNumberTools
    {
        [McpServerTool]
        [Description("Generates a random number between the specified minimum and maximum values.")]
        public int GetRandomNumber([Description("Minimum value (inclusive)")] int min = 1, [Description("Maximum value (exclusive)")] int max = 100) { return Random.Shared.Next(min, max); }
    }
    public static partial class ServiceCollectionExtensions { }
    public interface ITransientService { }
    public interface IScopedService { }
    public interface ISingletonService { }
    public interface IOpenGeneric<T> { }
    public interface IReadableRepository { }
    public interface IWritableRepository { }
    public interface IMyService { string GetData(); }
    public class DependAttribute : Attribute { }
    public class ObsoleteAttribute : Attribute { }
    public class MyService : IMyService { public string GetData() { return "MyService"; } }
    public class LoggingDecorator(IMyService inner, ILogger<LoggingDecorator> logger) : IMyService { public string GetData() { inner.GetData(); return "MyService"; } }
}

namespace App.Modules
{
    using global::App.Modules.Basket;
    using global::App.Modules.Catalog;
    using global::App.Modules.Ordering;
    using global::App.Shared;
    using global::Carter;
    using global::Serilog;
    using global::FluentValidation;
    using global::Mapster;
    using global::MassTransit;
    using global::MediatR;
    using global::Microsoft.AspNetCore.Builder;
    using global::Microsoft.AspNetCore.Http;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Routing;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.EntityFrameworkCore.Diagnostics;
    using global::Microsoft.EntityFrameworkCore.Metadata.Builders;
    using global::Microsoft.Extensions.Caching.Distributed;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Npgsql.EntityFrameworkCore.PostgreSQL;
    using global::Microsoft.Extensions.Hosting;
    using global::Microsoft.Extensions.Logging;
    using global::System.Reflection;
    using global::System.Text.Json;
    using global::System.Text.Json.Serialization;
    using global::Keycloak.AuthServices.Authentication;

    // Gateway
    // BFF
    // Saga
}

namespace App.Modules.Catalog
{
    using global::Carter;
    using global::FluentValidation;
    using global::Mapster;
    using global::MassTransit;
    using global::MediatR;
    using global::Microsoft.AspNetCore.Builder;
    using global::Microsoft.AspNetCore.Http;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Routing;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.EntityFrameworkCore.Diagnostics;
    using global::Microsoft.EntityFrameworkCore.Metadata.Builders;
    using global::Npgsql.EntityFrameworkCore.PostgreSQL;
    using global::Microsoft.Extensions.Hosting;
    using global::Microsoft.Extensions.Logging;
    using global::System.Reflection;
    using global::System.Text.Json;
    using global::System.Text.Json.Serialization;

    // Catalog
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container. Api Endpoint services; Application Use Case services; Data - Infrastructure services;
            var connectionString = configuration.GetConnectionString("Database");
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
            services.AddDbContext<CatalogDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(connectionString);
            });
            services.AddScoped<IDataSeeder, CatalogDataSeeder>();
            return services;
        }
        public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
        {
            // Configure the HTTP request pipeline. 1. Use Api Endpoint services; 2. Use Application Use Case services; 3. Use Data - Infrastructure services;
            app.UseMigration<CatalogDbContext>();
            return app;
        }
    }
    // Catalog-Domain-Dtos
    public record ProductDto(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price);
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(ProductDto Product);
    // Catalog-Domain-EventHandler
    public class ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger) : INotificationHandler<ProductCreatedEvent>
    {
        public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
            return Task.CompletedTask;
        }
    }
    public class ProductPriceChangedEventHandler(IBus bus, ILogger<ProductPriceChangedEventHandler> logger) : INotificationHandler<ProductPriceChangedEvent>
    {
        public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
            // Publish product price changed integration event for update basket prices
            var integrationEvent = new ProductPriceChangedIntegrationEvent
            {
                ProductId = notification.Product.Id,
                Name = notification.Product.Name,
                Category = notification.Product.Category,
                Description = notification.Product.Description,
                ImageFile = notification.Product.ImageFile,
                Price = notification.Product.Price //set updated product price
            };
            await bus.Publish(integrationEvent, cancellationToken);
        }
    }
    // Catalog-Domain-Events
    public record ProductCreatedEvent(Product Product) : IDomainEvent;
    public record ProductPriceChangedEvent(Product Product) : IDomainEvent;
    // Catalog-Domain-Exceptions
    public class ProductNotFoundException : NotFoundException { public ProductNotFoundException(Guid id) : base("Product", id) { } }
    // Catalog-Domain-Features-Create
    public record CreateProductRequest(ProductDto Product);
    public record CreateProductResponse(Guid Id);
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create Product");
        }
    }
    public record CreateProductCommand(ProductDto Product) : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Product.Category).NotEmpty().WithMessage("Category is required");
            RuleFor(x => x.Product.ImageFile).NotEmpty().WithMessage("ImageFile is required");
            RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
    internal class CreateProductHandler(CatalogDbContext dbContext) : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            //create Product entity from command object; save to database; return result;
            var product = CreateNewProduct(command.Product);
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new CreateProductResult(product.Id);
        }
        private Product CreateNewProduct(ProductDto productDto)
        {
            var product = Product.Create(Guid.NewGuid(), productDto.Name, productDto.Category, productDto.Description, productDto.ImageFile, productDto.Price);
            return product;
        }
    }
    // Catalog-Domain-Features-Delete
    public record DeleteProductRequest(Guid Id);
    public record DeleteProductResponse(bool IsSuccess);
    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductCommand(id));
                var response = result.Adapt<DeleteProductResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteProduct")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Product")
            .WithDescription("Delete Product");
        }
    }
    public record DeleteProductCommand(Guid ProductId) : ICommand<DeleteProductResult>;
    public record DeleteProductResult(bool IsSuccess);
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand> { public DeleteProductCommandValidator() { RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product Id is required"); } }
    internal class DeleteProductHandler(CatalogDbContext dbContext) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            // Delete Product entity from command object; save to database; return result
            var product = await dbContext.Products.FindAsync([command.ProductId], cancellationToken: cancellationToken);
            if (product is null) { throw new ProductNotFoundException(command.ProductId); }
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new DeleteProductResult(true);
        }
    }
    // Catalog-Domain-Features-Update
    public record UpdateProductRequest(ProductDto Product);
    public record UpdateProductResponse(bool IsSuccess);
    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async (UpdateProductRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateProductResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateProduct")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Product")
            .WithDescription("Update Product");
        }
    }
    public record UpdateProductCommand(ProductDto Product) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Product.Id).NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
    internal class UpdateProductHandler(CatalogDbContext dbContext) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            // Update Product entity from command object; save to database; return result
            var product = await dbContext.Products.FindAsync([command.Product.Id], cancellationToken: cancellationToken);
            if (product is null) { throw new ProductNotFoundException(command.Product.Id); }
            UpdateProductWithNewValues(product, command.Product);
            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new UpdateProductResult(true);
        }
        private void UpdateProductWithNewValues(Product product, ProductDto productDto)
        {
            product.Update(productDto.Name, productDto.Category, productDto.Description, productDto.ImageFile, productDto.Price);
        }
    }
    // Catalog-Domain-Features-GetByCategory
    public record GetProductByCategoryRequest(string Category);
    public record GetProductByCategoryResponse(IEnumerable<ProductDto> Products);
    public class GetProductByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{category}", async (string category, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByCategoryQuery(category));
                var response = result.Adapt<GetProductByCategoryResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProductByCategory")
            .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Category")
            .WithDescription("Get Product By Category");
        }
    }
    public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<ProductDto> Products);
    internal class GetProductByCategoryHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            // get products by category using dbContext; return result
            var products = await dbContext.Products.AsNoTracking().Where(p => p.Category.Contains(query.Category)).OrderBy(p => p.Name).ToListAsync(cancellationToken);
            //mapping product entity to productdto
            var productDtos = products.Adapt<List<ProductDto>>();
            return new GetProductByCategoryResult(productDtos);
        }
    }
    // Catalog-Domain-Features-Get
    public record GetProductByIdRequest(Guid Id);
    public record GetProductByIdResponse(ProductDto Product);
    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));
                var response = result.Adapt<GetProductByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProductById")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id")
            .WithDescription("Get Product By Id");
        }
    }
    internal class GetProductByIdHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            // get products by id using dbContext; return result
            var product = await dbContext.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);
            if (product is null) { throw new ProductNotFoundException(query.Id); }
            //mapping product entity to productdto
            var productDto = product.Adapt<ProductDto>();
            return new GetProductByIdResult(productDto);
        }
    }
    // Catalog-Domain-Features-List
    public record GetProductsRequest(PaginationRequest PaginationRequest);
    public record GetProductsResponse(PaginatedResult<ProductDto> Products);
    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetProductsQuery(request));
                var response = result.Adapt<GetProductsResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
        }
    }
    public record GetProductsQuery(PaginationRequest PaginationRequest) : IQuery<GetProductsResult>;
    public record GetProductsResult(PaginatedResult<ProductDto> Products);
    internal class GetProductsHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            // get products using dbContext; return result
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;
            var totalCount = await dbContext.Products.LongCountAsync(cancellationToken);
            var products = await dbContext.Products.AsNoTracking().OrderBy(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync(cancellationToken);
            // mapping product entity to ProductDto using Mapster
            var productDtos = products.Adapt<List<ProductDto>>();
            return new GetProductsResult(new PaginatedResult<ProductDto>(pageIndex, pageSize, totalCount, productDtos));
        }
    }
    // Catalog-Domain-Models
    public class Product : Aggregate<Guid>
    {
        public string Name { get; private set; } = default!;
        public List<string> Category { get; private set; } = new();
        public string Description { get; private set; } = default!;
        public string ImageFile { get; private set; } = default!;
        public decimal Price { get; private set; }
        public static Product Create(Guid id, string name, List<string> category, string description, string imageFile, decimal price)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
            var product = new Product { Id = id, Name = name, Category = category, Description = description, ImageFile = imageFile, Price = price };
            product.AddDomainEvent(new ProductCreatedEvent(product));
            return product;
        }
        public void Update(string name, List<string> category, string description, string imageFile, decimal price)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
            // Update Product entity fields
            Name = name;
            Category = category;
            Description = description;
            ImageFile = imageFile;
            // if price has changed, raise ProductPriceChanged domain event
            if (Price != price)
            {
                Price = price;
                AddDomainEvent(new ProductPriceChangedEvent(this));
            }
        }
    }
    // Catalog-Domain-ValueObjects
    // Catalog-Data-Configurations
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Category).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(200);
            builder.Property(p => p.ImageFile).HasMaxLength(100);
            builder.Property(p => p.Price).IsRequired();
        }
    }
    // Catalog-Data-Seed
    public class CatalogDataSeeder(CatalogDbContext dbContext) : IDataSeeder
    {
        public async Task SeedAllAsync()
        {
            if (!await dbContext.Products.AnyAsync())
            {
                await dbContext.Products.AddRangeAsync(InitialData.Products);
                await dbContext.SaveChangesAsync();
            }
        }
    }
    public static class InitialData
    {
        public static IEnumerable<Product> Products =>
        [
            Product.Create(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"), "IPhone X", ["category1"], "Long description", "imagefile", 500),
            Product.Create(new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"), "Samsung 10", ["category1"], "Long description", "imagefile", 400),
            Product.Create(new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8"), "Huawei Plus", ["category2"], "Long description", "imagefile", 650),
            Product.Create(new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27"), "Xiaomi Mi", ["category2"], "Long description", "imagefile", 450)
        ];
    }
    // Catalog-Data-DbContext
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("catalog");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
    // Catalog-Data-Migrations
}

namespace App.Modules.Basket
{
    using App.Modules.Catalog;
    using global::Carter;
    using global::FluentValidation;
    using global::Mapster;
    using global::MassTransit;
    using global::MediatR;
    using global::Microsoft.AspNetCore.Builder;
    using global::Microsoft.AspNetCore.Http;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Routing;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.EntityFrameworkCore.Diagnostics;
    using global::Microsoft.EntityFrameworkCore.Metadata.Builders;
    using global::Microsoft.Extensions.Caching.Distributed;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Npgsql.EntityFrameworkCore.PostgreSQL;
    using global::Microsoft.Extensions.Hosting;
    using global::Microsoft.Extensions.Logging;
    using global::System.Reflection;
    using global::System.Text.Json;
    using global::System.Text.Json.Serialization;

    // Basket
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container. 1. Api Endpoint services; 2. Application Use Case services; 3. Data - Infrastructure services;
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.Decorate<IBasketRepository, CachedBasketRepository>();
            var connectionString = configuration.GetConnectionString("Database");
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
            services.AddDbContext<BasketDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(connectionString);
            });
            services.AddHostedService<OutboxProcessor>();
            return services;
        }
        public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
        {
            // Configure the HTTP request pipeline. 1. Use Api Endpoint services; 2. Use Application Use Case services; 3. Use Data - Infrastructure services;
            app.UseMigration<BasketDbContext>();
            return app;
        }
    }
    // Basket-Domain-Dtos
    public record BasketCheckoutDto(string UserName, Guid CustomerId, decimal TotalPrice, string FirstName, string LastName, string EmailAddress, string AddressLine, string Country, string State, string ZipCode, string CardName, string CardNumber, string Expiration, string Cvv, int PaymentMethod
    // Shipping and BillingAddress,// Payment
    );
    public record ShoppingCartDto(Guid Id, string UserName, List<ShoppingCartItemDto> Items);
    public record ShoppingCartItemDto(Guid Id, Guid ShoppingCartId, Guid ProductId, int Quantity, string Color, decimal Price, string ProductName);
    // Basket-Domain-EventHandler
    public class ProductPriceChangedIntegrationEventHandler(ISender sender, ILogger<ProductPriceChangedIntegrationEventHandler> logger) : IConsumer<ProductPriceChangedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
        {
            logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);
            // mediatr new command and handler to find products on basket and update price
            var command = new UpdateItemPriceInBasketCommand(context.Message.ProductId, context.Message.Price);
            var result = await sender.Send(command);
            if (!result.IsSuccess) { logger.LogError("Error updating price in basket for product id: {ProductId}", context.Message.ProductId); }
            logger.LogInformation("Price for product id: {ProductId} updated in basket", context.Message.ProductId);
        }
    }
    // Basket-Domain-Events
    // Basket-Domain-Exceptions
    public class BasketNotFoundException(string userName) : NotFoundException("ShoppingCart", userName) { }
    // Basket-Domain-Features-Create
    public record CreateBasketRequest(ShoppingCartDto ShoppingCart);
    public record CreateBasketResponse(Guid Id);
    public class CreateBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket", async (CreateBasketRequest request, ISender sender, ClaimsPrincipal user) =>
            {
                var userName = user.Identity!.Name;
                var updatedShoppingCart = request.ShoppingCart with { UserName = userName };
                var command = new CreateBasketCommand(updatedShoppingCart);
                var result = await sender.Send(command);
                var response = result.Adapt<CreateBasketResponse>();
                return Results.Created($"/basket/{response.Id}", response);
            })
            .Produces<CreateBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Basket")
            .WithDescription("Create Basket")
            .RequireAuthorization();
        }
    }
    public record CreateBasketCommand(ShoppingCartDto ShoppingCart) : ICommand<CreateBasketResult>;
    public record CreateBasketResult(Guid Id);
    public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand> { public CreateBasketCommandValidator() { RuleFor(x => x.ShoppingCart.UserName).NotEmpty().WithMessage("UserName is required"); } }
    internal class CreateBasketHandler(IBasketRepository repository) : ICommandHandler<CreateBasketCommand, CreateBasketResult>
    {
        public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            // create Basket entity from command object; save to database; return result
            var shoppingCart = CreateNewBasket(command.ShoppingCart);
            await repository.CreateBasket(shoppingCart, cancellationToken);
            return new CreateBasketResult(shoppingCart.Id);
        }
        private ShoppingCart CreateNewBasket(ShoppingCartDto shoppingCartDto)
        {
            // create new basket
            var newBasket = ShoppingCart.Create(Guid.NewGuid(), shoppingCartDto.UserName);
            shoppingCartDto.Items.ForEach(item =>
            {
                newBasket.AddItem(item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
            });
            return newBasket;
        }
    }

    public record AddItemIntoBasketRequest(string UserName, ShoppingCartItemDto ShoppingCartItem);
    public record AddItemIntoBasketResponse(Guid Id);
    public class AddItemIntoBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket/{userName}/items", async ([FromRoute] string userName, [FromBody] AddItemIntoBasketRequest request, ISender sender) =>
            {
                var command = new AddItemIntoBasketCommand(userName, request.ShoppingCartItem);
                var result = await sender.Send(command);
                var response = result.Adapt<AddItemIntoBasketResponse>();
                return Results.Created($"/basket/{response.Id}", response);
            })
            .Produces<AddItemIntoBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Item Into Basket")
            .WithDescription("Add Item Into Basket")
            .RequireAuthorization();
        }
    }
    public record AddItemIntoBasketCommand(string UserName, ShoppingCartItemDto ShoppingCartItem) : ICommand<AddItemIntoBasketResult>;
    public record AddItemIntoBasketResult(Guid Id);
    public class AddItemIntoBasketCommandValidator : AbstractValidator<AddItemIntoBasketCommand>
    {
        public AddItemIntoBasketCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
            RuleFor(x => x.ShoppingCartItem.ProductId).NotEmpty().WithMessage("ProductId is required");
            RuleFor(x => x.ShoppingCartItem.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        }
    }
    internal class AddItemIntoBasketHandler(IBasketRepository repository, ISender sender) : ICommandHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
    {
        public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
        {
            // Add shopping cart item into shopping cart
            var shoppingCart = await repository.GetBasket(command.UserName, false, cancellationToken);
            // TODO: Before AddItem into SC, we should call Catalog Module GetProductById method
            // Get latest product information and set Price and ProductName when adding item into SC
            var result = await sender.Send(new GetProductByIdQuery(command.ShoppingCartItem.ProductId));
            shoppingCart.AddItem(command.ShoppingCartItem.ProductId, command.ShoppingCartItem.Quantity, command.ShoppingCartItem.Color, result.Product.Price, result.Product.Name);
            // command.ShoppingCartItem.Price,
            // command.ShoppingCartItem.ProductName);
            await repository.SaveChangesAsync(command.UserName, cancellationToken);
            return new AddItemIntoBasketResult(shoppingCart.Id);
        }
    }
    // Basket-Domain-Features-Delete
    public record DeleteBasketResponse(bool IsSuccess);
    public class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
            {
                var result = await sender.Send(new DeleteBasketCommand(userName));
                var response = result.Adapt<DeleteBasketResponse>();
                return Results.Ok(response);
            })
            .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Basket")
            .WithDescription("Delete Basket")
            .RequireAuthorization();
        }
    }
    public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
    public record DeleteBasketResult(bool IsSuccess);
    internal class DeleteBasketHandler(IBasketRepository repository) : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
    {
        public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
        {
            //Delete Basket entity from command object; save to database; return result
            await repository.DeleteBasket(command.UserName, cancellationToken);
            return new DeleteBasketResult(true);
        }
    }

    public record RemoveItemFromBasketResponse(Guid Id);
    public class RemoveItemFromBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{userName}/items/{productId}", async ([FromRoute] string userName, [FromRoute] Guid productId, ISender sender) =>
            {
                var command = new RemoveItemFromBasketCommand(userName, productId);
                var result = await sender.Send(command);
                var response = result.Adapt<RemoveItemFromBasketResponse>();
                return Results.Ok(response);
            })
            .Produces<RemoveItemFromBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Remove Item From Basket")
            .WithDescription("Remove Item From Basket")
            .RequireAuthorization();
        }
    }
    public record RemoveItemFromBasketCommand(string UserName, Guid ProductId) : ICommand<RemoveItemFromBasketResult>;
    public record RemoveItemFromBasketResult(Guid Id);
    public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
    {
        public RemoveItemFromBasketCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
        }
    }
    internal class RemoveItemFromBasketHandler(IBasketRepository repository) : ICommandHandler<RemoveItemFromBasketCommand, RemoveItemFromBasketResult>
    {
        public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
        {
            var shoppingCart = await repository.GetBasket(command.UserName, false, cancellationToken);
            shoppingCart.RemoveItem(command.ProductId);
            await repository.SaveChangesAsync(command.UserName, cancellationToken);
            return new RemoveItemFromBasketResult(shoppingCart.Id);
        }
    }
    // Basket-Domain-Features-Update
    public record UpdateItemPriceInBasketCommand(Guid ProductId, decimal Price) : ICommand<UpdateItemPriceInBasketResult>;
    public record UpdateItemPriceInBasketResult(bool IsSuccess);
    public class UpdateItemPriceInBasketCommandValidator : AbstractValidator<UpdateItemPriceInBasketCommand>
    {
        public UpdateItemPriceInBasketCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
    internal class UpdateItemPriceInBasketHandler(BasketDbContext dbContext) : ICommandHandler<UpdateItemPriceInBasketCommand, UpdateItemPriceInBasketResult>
    {
        public async Task<UpdateItemPriceInBasketResult> Handle(UpdateItemPriceInBasketCommand command, CancellationToken cancellationToken)
        {
            // Find Shopping Cart Items with a give ProductId ;Iterate items and Update Price of every item with incoming command.Price; save to database; return result;
            var itemsToUpdate = await dbContext.ShoppingCartItems.Where(x => x.ProductId == command.ProductId).ToListAsync(cancellationToken);
            if (!itemsToUpdate.Any()) { return new UpdateItemPriceInBasketResult(false); }
            foreach (var item in itemsToUpdate)
            {
                item.UpdatePrice(command.Price);
            }
            await dbContext.SaveChangesAsync(cancellationToken);
            return new UpdateItemPriceInBasketResult(true);
        }
    }
    // Basket-Domain-Features-Get
    public record GetBasketResponse(ShoppingCartDto ShoppingCart);
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {
                var result = await sender.Send(new GetBasketQuery(userName));
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            })
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Basket")
            .WithDescription("Get Basket")
            .RequireAuthorization();
        }
    }
    public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
    public record GetBasketResult(ShoppingCartDto ShoppingCart);
    internal class GetBasketHandler(IBasketRepository repository) : IQueryHandler<GetBasketQuery, GetBasketResult>
    {
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            // get basket with userName
            var basket = await repository.GetBasket(query.UserName, true, cancellationToken);
            //mapping basket entity to shoppingcartdto
            var basketDto = basket.Adapt<ShoppingCartDto>();
            return new GetBasketResult(basketDto);
        }
    }
    // Basket-Domain-Features-Checkout
    public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckout);
    public record CheckoutBasketResponse(bool IsSuccess);
    public class CheckoutBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender) =>
            {
                var command = request.Adapt<CheckoutBasketCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CheckoutBasketResponse>();
                return Results.Ok(response);
            })
            .WithName("CheckoutBasket")
            .Produces<CheckoutBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Checkout Basket")
            .WithDescription("Checkout Basket")
            .RequireAuthorization();
        }
    }
    public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout) : ICommand<CheckoutBasketResult>;
    public record CheckoutBasketResult(bool IsSuccess);
    public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
    {
        public CheckoutBasketCommandValidator()
        {
            RuleFor(x => x.BasketCheckout).NotNull().WithMessage("BasketCheckoutDto can't be null");
            RuleFor(x => x.BasketCheckout.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }
    internal class CheckoutBasketHandler(BasketDbContext dbContext) : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            // get existing basket with total price
            // Set totalprice on basketcheckout event message
            // send basket checkout event to rabbitmq using masstransit
            // delete the basket
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get existing basket with total price
                var basket = await dbContext.ShoppingCarts.Include(x => x.Items).SingleOrDefaultAsync(x => x.UserName == command.BasketCheckout.UserName, cancellationToken);
                if (basket == null) { throw new BasketNotFoundException(command.BasketCheckout.UserName); }
                // Set total price on basket checkout event message
                var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
                eventMessage.TotalPrice = basket.TotalPrice;
                // Write a message to the outbox
                var outboxMessage = new OutboxMessage { Id = Guid.NewGuid(), Type = typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName!, Content = JsonSerializer.Serialize(eventMessage), OccuredOn = DateTime.UtcNow };
                dbContext.OutboxMessages.Add(outboxMessage);
                // Delete the basket
                dbContext.ShoppingCarts.Remove(basket);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new CheckoutBasketResult(true);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return new CheckoutBasketResult(false);
            }

            // CHECKOUT BASKET WITHOUT OUTBOX
            // var basket = await repository.GetBasket(command.BasketCheckout.UserName, true, cancellationToken);
            // var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
            // eventMessage.TotalPrice = basket.TotalPrice;
            // await bus.Publish(eventMessage, cancellationToken);
            // await repository.DeleteBasket(command.BasketCheckout.UserName, cancellationToken);
            // return new CheckoutBasketResult(true);
            // CHECKOUT BASKET WITHOUT OUTBOX
        }
    }
    // Basket-Domain-Features-List
    // Basket-Domain-Models
    public class OutboxMessage : Entity<Guid>
    {
        public string Type { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime OccuredOn { get; set; } = default!;
        public DateTime? ProcessedOn { get; set; } = default!;
    }
    public class ShoppingCart : Aggregate<Guid>
    {
        public string UserName { get; private set; } = default!;
        private readonly List<ShoppingCartItem> _items = new();
        public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
        public static ShoppingCart Create(Guid id, string userName)
        {
            ArgumentException.ThrowIfNullOrEmpty(userName);
            var shoppingCart = new ShoppingCart
            {
                Id = id,
                UserName = userName
            };
            return shoppingCart;
        }
        public void AddItem(Guid productId, int quantity, string color, decimal price, string productName)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
            var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new ShoppingCartItem(Id, productId, quantity, color, price, productName);
                _items.Add(newItem);
            }
        }
        public void RemoveItem(Guid productId)
        {
            var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                _items.Remove(existingItem);
            }
        }
    }
    public class ShoppingCartItem : Entity<Guid>
    {
        public Guid ShoppingCartId { get; private set; } = default!;
        public Guid ProductId { get; private set; } = default!;
        public int Quantity { get; internal set; } = default!;
        public string Color { get; private set; } = default!;
        // will comes from Catalog module
        public decimal Price { get; private set; } = default!;
        public string ProductName { get; private set; } = default!;
        internal ShoppingCartItem(Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
            Color = color;
            Price = price;
            ProductName = productName;
        }
        [JsonConstructor]
        public ShoppingCartItem(Guid id, Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
        {
            Id = id;
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
            Color = color;
            Price = price;
            ProductName = productName;
        }
        public void UpdatePrice(decimal newPrice)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newPrice);
            Price = newPrice;
        }
    }
    // Basket-Domain-ValueObjects
    // Basket-Data-Configurations
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.UserName).IsUnique();
            builder.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            builder.HasMany(s => s.Items).WithOne().HasForeignKey(si => si.ShoppingCartId);
        }
    }
    public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(oi => oi.ProductId).IsRequired();
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.Color);
            builder.Property(oi => oi.Price).IsRequired();
            builder.Property(oi => oi.ProductName).IsRequired();
        }
    }
    // Basket-Data-DbContext
    public class BasketDbContext : DbContext
    {
        public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("basket");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
    // Basket-Data-Converter
    public class ShoppingCartConverter : JsonConverter<ShoppingCart>
    {
        public override ShoppingCart? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDocument = JsonDocument.ParseValue(ref reader);
            var rootElement = jsonDocument.RootElement;
            var id = rootElement.GetProperty("id").GetGuid();
            var userName = rootElement.GetProperty("userName").GetString()!;
            var itemsElement = rootElement.GetProperty("items");
            var shoppingCart = ShoppingCart.Create(id, userName);
            var items = itemsElement.Deserialize<List<ShoppingCartItem>>(options);
            if (items != null)
            {
                var itemsField = typeof(ShoppingCart).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
                itemsField?.SetValue(shoppingCart, items);
            }
            return shoppingCart;
        }
        public override void Write(Utf8JsonWriter writer, ShoppingCart value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("id", value.Id.ToString());
            writer.WriteString("userName", value.UserName);
            writer.WritePropertyName("items");
            JsonSerializer.Serialize(writer, value.Items, options);
            writer.WriteEndObject();
        }
    }
    public class ShoppingCartItemConverter : JsonConverter<ShoppingCartItem>
    {
        public override ShoppingCartItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDocument = JsonDocument.ParseValue(ref reader);
            var rootElement = jsonDocument.RootElement;
            var id = rootElement.GetProperty("id").GetGuid();
            var shoppingCartId = rootElement.GetProperty("shoppingCartId").GetGuid();
            var productId = rootElement.GetProperty("productId").GetGuid();
            var quantity = rootElement.GetProperty("quantity").GetInt32();
            var color = rootElement.GetProperty("color").GetString()!;
            var price = rootElement.GetProperty("price").GetDecimal();
            var productName = rootElement.GetProperty("productName").GetString()!;
            return new ShoppingCartItem(id, shoppingCartId, productId, quantity, color, price, productName);
        }
        public override void Write(Utf8JsonWriter writer, ShoppingCartItem value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("id", value.Id.ToString());
            writer.WriteString("shoppingCartId", value.ShoppingCartId.ToString());
            writer.WriteString("productId", value.ProductId.ToString());
            writer.WriteNumber("quantity", value.Quantity);
            writer.WriteString("color", value.Color);
            writer.WriteNumber("price", value.Price);
            writer.WriteString("productName", value.ProductName);
            writer.WriteEndObject();
        }
    }
    // Basket-Data-Migrations
    // Basket-Data-Processors
    public class OutboxProcessor(IServiceProvider serviceProvider, IBus bus, ILogger<OutboxProcessor> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
                    var outboxMessages = await dbContext.OutboxMessages.Where(m => m.ProcessedOn == null).ToListAsync(stoppingToken);
                    foreach (var message in outboxMessages)
                    {
                        var eventType = Type.GetType(message.Type);
                        if (eventType == null)
                        {
                            logger.LogWarning("Could not resolve type: {Type}", message.Type);
                            continue;
                        }
                        var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);
                        if (eventMessage == null)
                        {
                            logger.LogWarning("Could not deserialize message: {Content}", message.Content);
                            continue;
                        }
                        await bus.Publish(eventMessage, stoppingToken);
                        message.ProcessedOn = DateTime.UtcNow;
                        logger.LogInformation("Successfully processed outbox message with ID: {Id}", message.Id);
                    }
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing outbox messages");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Adjust the delay as needed
            }
        }
    }
    // Basket-Data-Repository
    public class BasketRepository(BasketDbContext dbContext) : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = dbContext.ShoppingCarts.Include(x => x.Items).Where(x => x.UserName == userName);
            if (asNoTracking) { query.AsNoTracking(); }
            var basket = await query.SingleOrDefaultAsync(cancellationToken);
            return basket ?? throw new BasketNotFoundException(userName);
        }
        public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            dbContext.ShoppingCarts.Add(basket);
            await dbContext.SaveChangesAsync(cancellationToken);
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            var basket = await GetBasket(userName, false, cancellationToken);
            dbContext.ShoppingCarts.Remove(basket);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new ShoppingCartConverter(), new ShoppingCartItemConverter() }
        };
        public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            if (!asNoTracking) { return await repository.GetBasket(userName, false, cancellationToken); }
            var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
            if (!string.IsNullOrEmpty(cachedBasket)) { return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket, _options)!; }
            var basket = await repository.GetBasket(userName, asNoTracking, cancellationToken);
            await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket, _options), cancellationToken);
            return basket;
        }
        public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            await repository.CreateBasket(basket, cancellationToken);
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket, _options), cancellationToken);
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            await repository.DeleteBasket(userName, cancellationToken);
            await cache.RemoveAsync(userName, cancellationToken);
            return true;
        }
        public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
        {
            var result = await repository.SaveChangesAsync(userName, cancellationToken);
            if (userName is not null) { await cache.RemoveAsync(userName, cancellationToken); }
            return result;
        }
    }
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default);
        Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default);
        Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default);
    }
}

namespace App.Modules.Ordering
{
    using global::Carter;
    using global::FluentValidation;
    using global::Mapster;
    using global::MassTransit;
    using global::MediatR;
    using global::Microsoft.AspNetCore.Builder;
    using global::Microsoft.AspNetCore.Http;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Routing;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.EntityFrameworkCore.Diagnostics;
    using global::Microsoft.EntityFrameworkCore.Metadata.Builders;
    using global::Microsoft.Extensions.Caching.Distributed;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Npgsql.EntityFrameworkCore.PostgreSQL;
    using global::Microsoft.Extensions.Hosting;
    using global::Microsoft.Extensions.Logging;
    using global::System.Reflection;
    using global::System.Text.Json;
    using global::System.Text.Json.Serialization;

    // Ordering
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container. 1. Api Endpoint services; 2. Application Use Case services; 3. Data - Infrastructure services;
            var connectionString = configuration.GetConnectionString("Database");
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
            services.AddDbContext<OrderingDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(connectionString);
            });
            return services;
        }
        public static IApplicationBuilder UseOrderingModule(this IApplicationBuilder app)
        {
            // Configure the HTTP request pipeline. 1. Use Api Endpoint services; 2. Use Application Use Case services; 3. Use Data - Infrastructure services;
            app.UseMigration<OrderingDbContext>();
            return app;
        }
    }
    // Ordering-Domain-Dtos
    public record AddressDto(string FirstName, string LastName, string EmailAddress, string AddressLine, string Country, string State, string ZipCode);
    public record OrderDto(Guid Id, Guid CustomerId, string OrderName, AddressDto ShippingAddress, AddressDto BillingAddress, PaymentDto Payment, List<OrderItemDto> Items);
    public record OrderItemDto(Guid OrderId, Guid ProductId, int Quantity, decimal Price);
    public record PaymentDto(string CardName, string CardNumber, string Expiration, string Cvv, int PaymentMethod);
    // Ordering-Domain-EventHandler
    public class BasketCheckoutIntegrationEventHandler(ISender sender, ILogger<BasketCheckoutIntegrationEventHandler> logger) : IConsumer<BasketCheckoutIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
        {
            logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);
            var createOrderCommand = MapToCreateOrderCommand(context.Message); // Create new order and start order fullfillment process
            await sender.Send(createOrderCommand);
        }
        private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutIntegrationEvent message)
        {
            // Create full order with incoming event data
            var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine, message.Country, message.State, message.ZipCode);
            var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.Cvv, message.PaymentMethod);
            var orderId = Guid.NewGuid();
            var orderDto = new OrderDto(Id: orderId, CustomerId: message.CustomerId, OrderName: message.UserName, ShippingAddress: addressDto, BillingAddress: addressDto, Payment: paymentDto,
                Items: [new OrderItemDto(orderId, new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"), 2, 500), new OrderItemDto(orderId, new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"), 1, 400)]);
            return new CreateOrderCommand(orderDto);
        }
    }
    public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger) : INotificationHandler<OrderCreatedEvent>
    {
        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
            return Task.CompletedTask;
        }
    }
    // Ordering-Domain-Events
    public record OrderCreatedEvent(Order Order) : IDomainEvent;
    // Ordering-Domain-Exceptions
    public class OrderNotFoundException(Guid orderId) : NotFoundException("Order", orderId) { }
    // Ordering-Domain-Features-Create
    public record CreateOrderRequest(OrderDto Order);
    public record CreateOrderResponse(Guid Id);
    public class CreateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/orders", async (CreateOrderRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateOrderCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateOrderResponse>();
                return Results.Created($"/Orders/{response.Id}", response);
            })
            .WithName("CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Order")
            .WithDescription("Create Order");
        }
    }
    public record CreateOrderCommand(OrderDto Order) : ICommand<CreateOrderResult>;
    public record CreateOrderResult(Guid Id);
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand> { public CreateOrderCommandValidator() { RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderName is required"); } }
    internal class CreateOrderHandler(OrderingDbContext dbContext) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = CreateNewOrder(command.Order);
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new CreateOrderResult(order.Id);
        }
        private Order CreateNewOrder(OrderDto orderDto)
        {
            var shippingAddress = Address.Of(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
            var billingAddress = Address.Of(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);
            var newOrder = Order.Create(id: Guid.NewGuid(), customerId: orderDto.CustomerId, orderName: $"{orderDto.OrderName}_{new Random().Next()}", shippingAddress: shippingAddress, billingAddress: billingAddress, payment: Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod));
            orderDto.Items.ForEach(item => { newOrder.Add(item.ProductId, item.Quantity, item.Price); });
            return newOrder;
        }
    }
    // Ordering-Domain-Features-Delete
    public record DeleteOrderResponse(bool IsSuccess);
    public class DeleteOrderEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/orders/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteOrderCommand(id));
                var response = result.Adapt<DeleteOrderResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteOrder")
            .Produces<DeleteOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Order")
            .WithDescription("Delete Order");
        }
    }
    public record DeleteOrderCommand(Guid OrderId) : ICommand<DeleteOrderResult>;
    public record DeleteOrderResult(bool IsSuccess);
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand> { public DeleteOrderCommandValidator() { RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderName is required"); } }
    internal class DeleteOrderHandler(OrderingDbContext dbContext) : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
    {
        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await dbContext.Orders.FindAsync([command.OrderId], cancellationToken: cancellationToken);
            if (order is null) { throw new OrderNotFoundException(command.OrderId); }
            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new DeleteOrderResult(true);
        }
    }
    // Ordering-Domain-Features-Update
    // Ordering-Domain-Features-Get
    public record GetOrderByIdResponse(OrderDto Order);
    public class GetOrderByIdEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetOrderByIdQuery(id));
                var response = result.Adapt<GetOrderByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetOrderById")
            .Produces<GetOrderByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Order By Id")
            .WithDescription("Get Order By Id");
        }
    }
    public record GetOrderByIdQuery(Guid Id) : IQuery<GetOrderByIdResult>;
    public record GetOrderByIdResult(OrderDto Order);
    internal class GetOrderByIdHandler(OrderingDbContext dbContext) : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResult>
    {
        public async Task<GetOrderByIdResult> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await dbContext.Orders.AsNoTracking().Include(x => x.Items).SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);
            if (order is null) { throw new OrderNotFoundException(query.Id); }
            var orderDto = order.Adapt<OrderDto>();
            return new GetOrderByIdResult(orderDto);
        }
    }
    // Ordering-Domain-Features-List
    public record GetOrdersResponse(PaginatedResult<OrderDto> Orders);
    public class GetOrdersEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetOrdersQuery(request));
                GetOrdersResponse response = result.Adapt<GetOrdersResponse>();
                return Results.Ok(response);
            })
            .WithName("GetOrders")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Orders")
            .WithDescription("Get Orders");
        }
    }
    public record GetOrdersQuery(PaginationRequest PaginationRequest) : IQuery<GetOrdersResult>;
    public record GetOrdersResult(PaginatedResult<OrderDto> Orders);
    internal class GetOrdersHandler(OrderingDbContext dbContext) : IQueryHandler<GetOrdersQuery, GetOrdersResult>
    {
        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;
            var totalCount = await dbContext.Orders.LongCountAsync(cancellationToken);
            var orders = await dbContext.Orders.AsNoTracking().Include(x => x.Items).OrderBy(p => p.OrderName).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync(cancellationToken);
            var orderDtos = orders.Adapt<List<OrderDto>>();
            return new GetOrdersResult(new PaginatedResult<OrderDto>(pageIndex, pageSize, totalCount, orderDtos));
        }
    }
    // Ordering-Domain-Models
    public class Order : Aggregate<Guid>
    {
        private readonly List<OrderItem> _items = new();
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
        public Guid CustomerId { get; private set; } = default!;
        public string OrderName { get; private set; } = default!;
        public Address ShippingAddress { get; private set; } = default!;
        public Address BillingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
        public static Order Create(Guid id, Guid customerId, string orderName, Address shippingAddress, Address billingAddress, Payment payment)
        {
            var order = new Order { Id = id, CustomerId = customerId, OrderName = orderName, ShippingAddress = shippingAddress, BillingAddress = billingAddress, Payment = payment };
            order.AddDomainEvent(new OrderCreatedEvent(order));
            return order;
        }
        public void Add(Guid productId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
            var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var orderItem = new OrderItem(Id, productId, quantity, price);
                _items.Add(orderItem);
            }
        }
        public void Remove(Guid productId)
        {
            var orderItem = _items.FirstOrDefault(x => x.ProductId == productId);
            if (orderItem is not null)
            {
                _items.Remove(orderItem);
            }
        }
    }
    public class OrderItem : Entity<Guid>
    {
        internal OrderItem(Guid orderId, Guid productId, int quantity, decimal price)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }
        public Guid OrderId { get; private set; } = default!;
        public Guid ProductId { get; private set; } = default!;
        public int Quantity { get; internal set; } = default!;
        public decimal Price { get; private set; } = default!;
    }
    // Ordering-Domain-ValueObjects
    public record Address
    {
        public string FirstName { get; } = default!;
        public string LastName { get; } = default!;
        public string? EmailAddress { get; } = default!;
        public string AddressLine { get; } = default!;
        public string Country { get; } = default!;
        public string State { get; } = default!;
        public string ZipCode { get; } = default!;
        protected Address() { }
        private Address(string firstName, string lastName, string emailAddress, string addressLine, string country, string state, string zipCode)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            AddressLine = addressLine;
            Country = country;
            State = state;
            ZipCode = zipCode;
        }
        public static Address Of(string firstName, string lastName, string emailAddress, string addressLine, string country, string state, string zipCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine);
            return new Address(firstName, lastName, emailAddress, addressLine, country, state, zipCode);
        }
    }
    public record Payment
    {
        public string? CardName { get; } = default!;
        public string CardNumber { get; } = default!;
        public string Expiration { get; } = default!;
        public string CVV { get; } = default!;
        public int PaymentMethod { get; } = default!;
        protected Payment() { }
        private Payment(string cardName, string cardNumber, string expiration, string cvv, int paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CVV = cvv;
            PaymentMethod = paymentMethod;
        }
        public static Payment Of(string cardName, string cardNumber, string expiration, string cvv, int paymentMethod)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName);
            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(cvv);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cvv.Length, 3);
            return new Payment(cardName, cardNumber, expiration, cvv, paymentMethod);
        }
    }
    // Ordering-Data-Configurations
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(o => o.CustomerId);
            builder.HasIndex(e => e.OrderName).IsUnique();
            builder.Property(e => e.OrderName).IsRequired().HasMaxLength(100);
            builder.HasMany(s => s.Items).WithOne().HasForeignKey(si => si.OrderId);
            builder.ComplexProperty(o => o.ShippingAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.EmailAddress).HasMaxLength(50);
                addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
                addressBuilder.Property(a => a.Country).HasMaxLength(50);
                addressBuilder.Property(a => a.State).HasMaxLength(50);
                addressBuilder.Property(a => a.ZipCode).HasMaxLength(5).IsRequired();
            });
            builder.ComplexProperty(o => o.BillingAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.EmailAddress).HasMaxLength(50);
                addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
                addressBuilder.Property(a => a.Country).HasMaxLength(50);
                addressBuilder.Property(a => a.State).HasMaxLength(50);
                addressBuilder.Property(a => a.ZipCode).HasMaxLength(5).IsRequired();
            });
            builder.ComplexProperty(o => o.Payment, paymentBuilder =>
            {
                paymentBuilder.Property(p => p.CardName).HasMaxLength(50);
                paymentBuilder.Property(p => p.CardNumber).HasMaxLength(24).IsRequired();
                paymentBuilder.Property(p => p.Expiration).HasMaxLength(10);
                paymentBuilder.Property(p => p.CVV).HasMaxLength(3);
                paymentBuilder.Property(p => p.PaymentMethod);
            });
        }
    }
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(oi => oi.ProductId).IsRequired();
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.Price).IsRequired();
        }
    }
    // Ordering-Data-DbContext
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("ordering");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
    // Ordering-Data-Migrations
}

namespace App.Shared
{
    using global::Carter;
    using global::FluentValidation;
    using global::MassTransit;
    using global::MassTransit.RabbitMqTransport;
    using global::MediatR;
    using global::Microsoft.AspNetCore.Builder;
    using global::Microsoft.AspNetCore.Diagnostics;
    using global::Microsoft.AspNetCore.Http;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.EntityFrameworkCore.ChangeTracking;
    using global::Microsoft.EntityFrameworkCore.Diagnostics;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Logging;
    using global::System.Diagnostics;
    using global::System.Reflection;

    // Behaviors
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse> where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={RequestData}", typeof(TRequest).Name, typeof(TResponse).Name, request);
            var timer = new Stopwatch();
            timer.Start();
            var response = await next();
            timer.Stop();
            var timeTaken = timer.Elapsed;
            // if the request is greater than 3 seconds, then log the warnings
            if (timeTaken.Seconds > 3) { logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken} seconds.", typeof(TRequest).Name, timeTaken.Seconds); }
            logger.LogInformation("[END] Handled {Request} with {Response}", typeof(TRequest).Name, typeof(TResponse).Name);
            return response;
        }
    }
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();
            if (failures.Any()) throw new ValidationException(failures);
            return await next();
        }
    }
    // DDD
    public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        public IDomainEvent[] ClearDomainEvents()
        {
            IDomainEvent[] dequeuedEvents = _domainEvents.ToArray();
            _domainEvents.Clear();
            return dequeuedEvents;
        }
    }
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
    }
    public interface IAggregate<T> : IAggregate, IEntity<T> { }
    public interface IAggregate : IEntity { IReadOnlyList<IDomainEvent> DomainEvents { get; } IDomainEvent[] ClearDomainEvents(); }
    public interface IEntity<T> : IEntity { public T Id { get; set; } }
    public interface IEntity { public DateTime? CreatedAt { get; set; } public string? CreatedBy { get; set; } public DateTime? LastModified { get; set; } public string? LastModifiedBy { get; set; } }
    public interface IDomainEvent : INotification { Guid EventId => Guid.NewGuid(); public DateTime OccurredOn => DateTime.Now; public string EventType => GetType().AssemblyQualifiedName!; }
    // Data
    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        private void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = "mehmet";
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    entry.Entity.LastModifiedBy = "mehmet";
                    entry.Entity.LastModified = DateTime.UtcNow;
                }
            }
        }
    }
    public class DispatchDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        private async Task DispatchDomainEvents(DbContext? context)
        {
            if (context == null) return;
            var aggregates = context.ChangeTracker
                .Entries<IAggregate>()
                .Where(a => a.Entity.DomainEvents.Any())
                .Select(a => a.Entity);
            var domainEvents = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();
            aggregates.ToList().ForEach(a => a.ClearDomainEvents());
            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
    public interface IDataSeeder { Task SeedAllAsync(); }
    public static class Extentions
    {
        public static IApplicationBuilder UseMigration<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();
            SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();
            return app;
        }
        private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await context.Database.MigrateAsync();
        }
        private static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
            foreach (var seeder in seeders)
            {
                await seeder.SeedAllAsync();
            }
        }
    }
    // Exceptions
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, string details) : base(message) { Details = details; }
        public string? Details { get; }
    }
    public class NotFoundException : Exception { public NotFoundException(string message) : base(message) { } public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { } }
    public class InternalServerException : Exception { public InternalServerException(string message) : base(message) { } public InternalServerException(string message, string details) : base(message) { Details = details; } public string? Details { get; } }
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}", exception.Message, DateTime.UtcNow);
            (string Detail, string Title, int StatusCode) details = exception switch
            {
                InternalServerException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status500InternalServerError),
                ValidationException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status400BadRequest),
                BadRequestException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status400BadRequest),
                NotFoundException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status404NotFound),
                _ => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status500InternalServerError)
            };
            ProblemDetails problemDetails = new() { Title = details.Title, Detail = details.Detail, Status = details.StatusCode, Instance = context.Request.Path };
            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
            if (exception is ValidationException validationException) { problemDetails.Extensions.Add("ValidationErrors", validationException.Errors); }
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
            return true;
        }
    }
    // Extensions
    public static class CarterExtentions
    {
        public static IServiceCollection AddCarterWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddCarter(configurator: config =>
            {
                foreach (Assembly assembly in assemblies)
                {
                    var modules = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();
                    config.WithModules(modules);
                }
            });
            return services;
        }
    }
    public static class MediatRExtentions
    {
        public static IServiceCollection AddMediatRWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(assemblies);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            services.AddValidatorsFromAssemblies(assemblies);
            return services;
        }
    }
    // Pagination
    public record PaginationRequest(int PageIndex = 0, int PageSize = 10);
    public class PaginatedResult<TEntity>(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data) where TEntity : class
    {
        public int PageIndex { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public long Count { get; } = count;
        public IEnumerable<TEntity> Data { get; } = data;
    }
    // Contracts-CQRS
    public interface ICommand : ICommand<Unit> { }
    public interface ICommand<out TResponse> : MediatR.IRequest<TResponse> { }
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand<Unit> { }
    public interface ICommandHandler<in TCommand, TResponse> : MediatR.IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse> where TResponse : notnull { }
    public interface IQuery<out T> : MediatR.IRequest<T> where T : notnull { }
    public interface IQueryHandler<in TQuery, TResponse> : MediatR.IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> where TResponse : notnull { }
    // Message-Events
    public record IntegrationEvent
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime OccurredOn => DateTime.Now;
        public string EventType => GetType().AssemblyQualifiedName;
    }
    public record BasketCheckoutIntegrationEvent : IntegrationEvent
    {
        public string UserName { get; set; } = default!;
        public Guid CustomerId { get; set; } = default!;
        public decimal TotalPrice { get; set; } = default!;
        // Shipping and BillingAddress
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string EmailAddress { get; set; } = default!;
        public string AddressLine { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        // Payment
        public string CardName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string Expiration { get; set; } = default!;
        public string Cvv { get; set; } = default!;
        public int PaymentMethod { get; set; } = default!;
    }
    public record ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public Guid ProductId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<string> Category { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageFile { get; set; } = default!;
        public decimal Price { get; set; } = default!;
    }
    // Message-Extensions
    public static class MassTransitExtentions
    {
        public static IServiceCollection AddMassTransitWithAssemblies(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();
                config.SetInMemorySagaRepositoryProvider();
                config.AddConsumers(assemblies);
                config.AddSagaStateMachines(assemblies);
                config.AddSagas(assemblies);
                config.AddActivities(assemblies);
                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]!);
                        host.Password(configuration["MessageBroker:Password"]!);
                    });
                    configurator.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}