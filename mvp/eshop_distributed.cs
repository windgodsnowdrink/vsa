#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package Microsoft.Extensions.Hosting@10.0.0-rc.1.25451.107
#:package System.Threading.Channels@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
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
#:package ModelContextProtocol@0.3.0-preview.4
#:package ModelContextProtocol.Core@0.3.0-preview.4
#:package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL@9.5.0
#:package CommunityToolkit.Aspire.OllamaSharp@9.8.0
#:package Microsoft.EntityFrameworkCore.Tools@9.0.9
#:package Microsoft.Extensions.VectorData.Abstractions@9.7.0
#:package Microsoft.SemanticKernel.Connectors.InMemory@1.65.0-preview
#:package Aspire.Keycloak.Authentication@9.5.0-preview.1.25474.7
#:package Aspire.StackExchange.Redis.DistributedCaching@9.5.0
#:package PublicTransit@8.5.2
#:package PublicTransit.RabbitMQ@8.5.2
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.12.0
#:package OpenTelemetry.Extensions.Hosting@1.12.0
#:package OpenTelemetry.Instrumentation.AspNetCore@1.12.0
#:package OpenTelemetry.Instrumentation.Http@1.12.0
#:package OpenTelemetry.Instrumentation.Runtime@1.12.0
#:package Microsoft.Extensions.Http.Resilience@9.9.0
#:package Microsoft.Extensions.ServiceDiscovery@9.5.0
#:package Aspire.StackExchange.Redis.OutputCaching@9.5.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.SampleMcpServer
#:property PackageVersion=0.0.1-beta
#:property PackageTags=AI; MCP; server; stdio
#:property Description=An MCP server using the MCP C# SDK.

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Ardalis.ListStartupServices;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ModelContextProtocol.Server;
using Microsoft.SemanticKernel;
global using ServiceDefaults.Messaging;
global using System.Reflection;
global using Microsoft.EntityFrameworkCore;
using ServiceDefaults;
using Catalog;
using Basket;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100MB
    options.ListenLocalhost(5003); // http
    // options.ListenAnyIP(5000, listenOptions => listenOptions.UseConnectionLogging());
    options.ListenAnyIP(5004, o => o.UseHttps()); // https,若有证书
});

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;
builder.AddServiceDefaults();

builder.Logging.ClearProviders();               // 只保留下面的 ConsoleProvider,避免干扰
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);                   // 使用颜色化的控制台日志
builder.Logging.SetMinimumLevel(LogLevel.Trace); // 设为 Trace,最细粒度
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
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration);
    logging.AddSerilog(Log.Logger, true);
});
builder.Services.AddOptions();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination", "X-Platform-Type", "X-CSRF-TOKEN-HEADERNAME", "X-CSRF-TOKEN", "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Forwarded-Prefix", "X-Body-Hash")));

builder.AddRedisDistributedCache(connectionName: "cache");
builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<BasketService>();
builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new("https+http://catalog");
});

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "keycloak",
        realm: "eshop",
        configureOptions: options =>
        {
            options.RequireHttpsMetadata = false;
            options.Audience = "account";
        });

builder.AddOllamaSharpChatClient("ollama-llama3-2");
builder.AddOllamaSharpEmbeddingGenerator("ollama-all-minilm");
builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddAuthorization();

// 添加OpenApi服务,这是Scalar所需的
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
var app = builder.Build();
app.UseSerilogRequestLogging(opt =>
{
    opt.IncludeQueryInRequestPath = true;
});
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
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
app.MapDefaultEndpoints();
app.UseMigration();
app.MapProductEndpoints();
app.MapBasketEndpoints();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

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
Console.WriteLine("Routes registered");
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
        public int GetRandomNumber(
                [Description("Minimum value (inclusive)")] int min = 1,
                [Description("Maximum value (exclusive)")] int max = 100)
        {
            return Random.Shared.Next(min, max);
        }
    }
}

namespace ServiceDefaults
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using MassTransit;
    using System.Reflection;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.ServiceDiscovery;
    using OpenTelemetry;
    using OpenTelemetry.Metrics;
    using OpenTelemetry.Trace;

    public record IntegrationEvent
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime OccurredOn => DateTime.Now;
        public string EventType => GetType().AssemblyQualifiedName;
    }

    public record ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public int ProductId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
    }

    public static class MassTransitExtentions
    {
        public static IServiceCollection AddMassTransitWithAssemblies
            (this IServiceCollection services, params Assembly[] assemblies)
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
                    var configuration = context.GetRequiredService<IConfiguration>();
                    var connectionString = configuration.GetConnectionString("rabbitmq");

                    configurator.Host(connectionString);
                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }

    // Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
    // This project should be referenced by each service project in your solution.
    // To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
    public static class Extensions
    {
        public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

            // Uncomment the following to restrict the allowed schemes for service discovery.
            // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
            // {
            //     options.AllowedSchemes = ["https"];
            // });

            return builder;
        }

        public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation()
                        // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                        //.AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource("MassTransit");
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
            //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
            //{
            //    builder.Services.AddOpenTelemetry()
            //       .UseAzureMonitor();
            //}

            return builder;
        }

        public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Adding health checks endpoints to applications in non-development environments has security implications.
            // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
            if (app.Environment.IsDevelopment())
            {
                // All health checks must pass for app to be considered ready to accept traffic after starting
                app.MapHealthChecks("/health");

                // Only health checks tagged with the "live" tag must pass for app to be considered alive
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }

            return app;
        }
    }
}

namespace Catalog
{
    using Microsoft.Extensions.VectorData;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.VectorData;
    using MassTransit;
    using ServiceDefaults.Messaging.Events;

    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/products");
            // GET all
            group.MapGet("/", async (ProductService service) =>
            {
                var products = await service.GetProductsAsync();
                return Results.Ok(products);
            })
            .WithName("GetAllProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);
            // GET by ID
            group.MapGet("/{id}", async (int id, ProductService service) =>
            {
                var product = await service.GetProductByIdAsync(id);
                if (product is null) return Results.NotFound();

                return Results.Ok(product);
            })
            .WithName("GetProductById")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
            // POST (Create)
            group.MapPost("/", async (Product product, ProductService service) =>
            {
                await service.CreateProductAsync(product);
                return Results.Created($"/products/{product.Id}", product);
            })
            .WithName("CreateProduct")
            .Produces<Product>(StatusCodes.Status201Created);
            // PUT (Update)
            group.MapPut("/{id}", async (int id, Product inputProduct, ProductService service) =>
            {
                var updatedProduct = await service.GetProductByIdAsync(id);
                if (updatedProduct is null) return Results.NotFound();

                await service.UpdateProductAsync(updatedProduct, inputProduct);
                return Results.NoContent();
            })
            .WithName("UpdateProduct")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent);
            // DELETE
            group.MapDelete("/{id}", async (int id, ProductService service) =>
            {
                var deletedProduct = await service.GetProductByIdAsync(id);
                if (deletedProduct is null) return Results.NotFound();

                await service.DeleteProductAsync(deletedProduct);
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent);
            // Support AI
            group.MapGet("/support/{query}", async (string query, ProductAIService service) =>
            {
                var response = await service.SupportAsync(query);

                return Results.Ok(response);
            })
            .WithName("Support")
            .Produces(StatusCodes.Status200OK);
            // Traditional Search
            group.MapGet("search/{query}", async (string query, ProductService service) =>
            {
                var products = await service.SearchProductsAsync(query);

                return Results.Ok(products);
            })
            .WithName("SearchProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);
            // AI Search
            group.MapGet("aisearch/{query}", async (string query, ProductAIService service) =>
            {
                var products = await service.SearchProductsAsync(query);

                return Results.Ok(products);
            })
            .WithName("AISearchProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);
        }
    }

    public static class Extensions
    {
        public static void UseMigration(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

            context.Database.Migrate();
            DataSeeder.Seed(context);
        }
    }

    public class DataSeeder
    {
        public static void Seed(ProductDbContext dbContext)
        {
            if (dbContext.Products.Any())
                return;

            dbContext.Products.AddRange(Products);
            dbContext.SaveChanges();
        }

        public static IEnumerable<Product> Products =>
        [
            new Product { Name = "Solar Powered Flashlight", Description = "A fantastic product for outdoor enthusiasts", Price = 19.99m, ImageUrl = "product1.png" },
            new Product { Name = "Hiking Poles", Description = "Ideal for camping and hiking trips", Price = 24.99m, ImageUrl = "product2.png" },
            new Product { Name = "Outdoor Rain Jacket", Description = "This product will keep you warm and dry in all weathers", Price = 49.99m, ImageUrl = "product3.png" },
            new Product { Name = "Survival Kit", Description = "A must-have for any outdoor adventurer", Price = 99.99m, ImageUrl = "product4.png" },
            new Product { Name = "Outdoor Backpack", Description = "This backpack is perfect for carrying all your outdoor essentials", Price = 39.99m, ImageUrl = "product5.png" },
            new Product { Name = "Camping Cookware", Description = "This cookware set is ideal for cooking outdoors", Price = 29.99m, ImageUrl = "product6.png" },
            new Product { Name = "Camping Stove", Description = "This stove is perfect for cooking outdoors", Price = 49.99m, ImageUrl = "product7.png" },
            new Product { Name = "Camping Lantern", Description = "This lantern is perfect for lighting up your campsite", Price = 19.99m, ImageUrl = "product8.png" },
            new Product { Name = "Camping Tent", Description = "This tent is perfect for camping trips", Price = 99.99m, ImageUrl = "product9.png" }
        ];
    }

    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = default!;
    }

    public class ProductVector
    {
        [Microsoft.Extensions.VectorData.VectorStoreKey]
        public int Id { get; set; }
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public string Name { get; set; } = default!;
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public string Description { get; set; } = default!;
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public decimal Price { get; set; }
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public string ImageUrl { get; set; } = default!;

        [NotMapped]
        [Microsoft.Extensions.VectorData.VectorStoreVector(384)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }

    public class ProductAIService(
    ProductDbContext dbContext,
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    VectorStoreCollection<int, ProductVector> productVectorCollection)
    {
        public async Task<string> SupportAsync(string query)
        {
            var systemPrompt = """
            You are a useful assistant. 
            You always reply with a short and funny message. 
            If you do not know an answer, you say 'I don't know that.' 
            You only answer questions related to outdoor camping products. 
            For any other type of questions, explain to the user that you only answer outdoor camping products questions.
            At the end, Offer one of our products: Hiking Poles-$24, Outdoor Rain Jacket-$12, Outdoor Backpack-$32, Camping Tent-$22
            Do not store memory of the chat conversation.
            """;

            List<ChatMessage> chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage(ChatRole.User, query)
            };

            ChatResponse resultPrompt = await chatClient.GetResponseAsync(chatHistory);
            return resultPrompt.Messages[0].ToString()!;        
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {
            if (!await productVectorCollection.CollectionExistsAsync())
            {
                await InitEmbeddingsAsync();
            }

            ReadOnlyMemory<float> queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

            VectorSearchOptions<ProductVector> vectorSearchOptions = new VectorSearchOptions<ProductVector>();
            {
                // Top = 1,
                // VectorPropertyName = "Vector"
            };

            IAsyncEnumerable<VectorSearchResult<ProductVector>> results = productVectorCollection.SearchAsync<ReadOnlyMemory<float>>(queryEmbedding, 1, vectorSearchOptions, CancellationToken.None);

            List<Product> products = [];
            await foreach (VectorSearchResult<ProductVector> resultItem in results)
            {
                products.Add(new Product
                {
                    Id = resultItem.Record.Id,
                    Name = resultItem.Record.Name,
                    Description = resultItem.Record.Description,
                    Price = resultItem.Record.Price,
                    ImageUrl = resultItem.Record.ImageUrl,
                });
            }        

            return products;
        }

        private async Task InitEmbeddingsAsync()
        {
            await productVectorCollection.EnsureCollectionExistsAsync();

            List<Product> products = await dbContext.Products.ToListAsync();
            foreach (Product product in products)
            {
                string productInfo = $"[{product.Name}] is a product that costs [{product.Price}] and is described as [{product.Description}]";

                ProductVector productVector = new ProductVector
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Vector = await embeddingGenerator.GenerateVectorAsync(productInfo),
                };

                await productVectorCollection.UpsertAsync(productVector);
            }
        }
    }

    public class ProductService(ProductDbContext dbContext, IBus bus)
    {
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }

        public async Task CreateProductAsync(Product product)
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
        {
            // if price has changed, raise ProductPriceChanged integration event
            if (updatedProduct.Price != inputProduct.Price)
            {
                // Publish product price changed integration event for update basket prices
                var integrationEvent = new ProductPriceChangedIntegrationEvent
                {
                    ProductId = updatedProduct.Id, // Id only comes from db entity
                    Name = inputProduct.Name,
                    Description = inputProduct.Description,
                    Price = inputProduct.Price, //set updated product price
                    ImageUrl = inputProduct.ImageUrl
                };
                await bus.Publish(integrationEvent);
            }

            // update product with new values
            updatedProduct.Name = inputProduct.Name;
            updatedProduct.Description = inputProduct.Description;
            updatedProduct.ImageUrl = inputProduct.ImageUrl;
            updatedProduct.Price = inputProduct.Price;

            dbContext.Products.Update(updatedProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product deletedProduct)
        {
            dbContext.Products.Remove(deletedProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {
            return await dbContext.Products
                .Where(p => p.Name.Contains(query))
                .ToListAsync();
        }
    }
}

namespace Basket
{
    using Catalgo;
    using MassTransit;
    using Microsoft.Extensions.Caching.Distributed;
    using System.Text.Json;
    using ServiceDefaults;
    using Product = Catalgo.Product;

    public class CatalogApiClient(HttpClient httpClient)
    {
        public async Task<Product> GetProductById(int id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"/products/{id}");
            return response!;
        }
    }

    public static class BasketEndpoints
    {
        public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("basket");

            // GET by userName
            group.MapGet("/{userName}", async (string userName, BasketService service) =>
            {
                var shoppingCart = await service.GetBasket(userName);

                if (shoppingCart is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(shoppingCart);
            })
            .WithName("GetBasket")
            .Produces<ShoppingCart>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

            // POST (Upsert)
            group.MapPost("/", async (ShoppingCart shoppingCart, BasketService service) =>
            {
                await service.UpdateBasket(shoppingCart);
                return Results.Created("GetBasket", shoppingCart);
            })
            .WithName("UpdateBasket")
            .Produces<ShoppingCart>(StatusCodes.Status201Created)
            .RequireAuthorization();

            // DELETE
            group.MapDelete("/{userName}", async (string userName, BasketService service) =>
            {
                await service.DeleteBasket(userName);
                return Results.NoContent();
            })
            .WithName("DeleteBasket")
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization();
        }
    }

    public class ProductPriceChangedIntegrationEventHandler(BasketService service)
    : IConsumer<ProductPriceChangedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
        {
            // find products on basket and update price
            await service.UpdateBasketItemProductPrices
                (context.Message.ProductId, context.Message.Price);
        }
    }

    public class ShoppingCart
    {
        public string UserName { get; set; } = default!;
        public List<ShoppingCartItem> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
    }

    public class ShoppingCartItem
    {
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int ProductId { get; set; } = default!;

        // will comes from Catalog module
        public decimal Price { get; set; } = default!;
        public string ProductName { get; set; } = default!;
    }

    public class BasketService(IDistributedCache cache, CatalogApiClient catalogApiClient)
    {
        public async Task<ShoppingCart?> GetBasket(string userName)
        {
            var basket = await cache.GetStringAsync(userName);
            return string.IsNullOrEmpty(basket) ? null :
                JsonSerializer.Deserialize<ShoppingCart>(basket);
        }
        public async Task UpdateBasket(ShoppingCart basket)
        {
            // Before update(Add/remove Item) into SC, we should call Catalog ms GetProductById method
            // Get latest product information and set Price and ProductName when adding item into SC
            foreach (var item in basket.Items)
            {
                var product = await catalogApiClient.GetProductById(item.ProductId);
                item.Price = product.Price;
                item.ProductName = product.Name;
            }

            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
        }
        public async Task DeleteBasket(string userName)
        {
            await cache.RemoveAsync(userName);
        }

        internal async Task UpdateBasketItemProductPrices(int productId, decimal price)
        {
            // IDistributedCache not supported list of keys function
            // https://github.com/dotnet/runtime/issues/36402

            var basket = await GetBasket("swn");

            var item = basket!.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Price = price;
                await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
            }
        }
    }

}

namespace AppHost
{
    // IsAspireHost
    // Aspire.Hosting.AppHost@9.1.0
    // Aspire.Hosting.Keycloak@9.1.0-preview.1.25121.10
    // Aspire.Hosting.PostgreSQL@9.1.0
    // Aspire.Hosting.RabbitMQ@9.1.0
    // Aspire.Hosting.Redis@9.1.0
    // CommunityToolkit.Aspire.Hosting.Ollama@9.2.0
    using Basket;
    using Catalgo;
    using WebApp;

    public partial class Program
    {

        var builder = DistributedApplication.CreateBuilder(args);

        // Backing Services
        var postgres = builder
                .AddPostgres("postgres")
                .WithPgAdmin()
                //.WithDataVolume()
                .WithLifetime(ContainerLifetime.Persistent);

        var catalogDb = postgres.AddDatabase("catalogdb");

        var cache = builder
            .AddRedis("cache")
            .WithRedisInsight()
            //.WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);

        var rabbitmq = builder
            .AddRabbitMQ("rabbitmq")
            .WithManagementPlugin()
            //.WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);

        var keycloak = builder
            .AddKeycloak("keycloak", 8080)
            //.WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);

        if (builder.ExecutionContext.IsRunMode)
        {
            // Data volumes don't work on ACA for Postgres so only add when running
            postgres.WithDataVolume();
            rabbitmq.WithDataVolume();
            keycloak.WithDataVolume();
        }

        var ollama = builder
            .AddOllama("ollama", 11434)
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithOpenWebUI();

        var llama = ollama.AddModel("llama3.2");
        var embedding = ollama.AddModel("all-minilm");

        // Projects
        var catalog = builder
            .AddProject<Projects.Catalog>("catalog")
            .WithReference(catalogDb)
            .WithReference(rabbitmq)
            .WithReference(llama)
            .WithReference(embedding)
            .WaitFor(catalogDb)
            .WaitFor(rabbitmq)
            .WaitFor(llama)
            .WaitFor(embedding);

        var basket = builder
            .AddProject<Projects.Basket>("basket")
            .WithReference(cache)
            .WithReference(catalog)
            .WithReference(rabbitmq)
            .WithReference(keycloak)
            .WaitFor(cache)
            .WaitFor(rabbitmq)
            .WaitFor(keycloak);

        var webapp = builder
            .AddProject<Projects.WebApp>("webapp")
            .WithExternalHttpEndpoints()
            .WithReference(cache)
            .WithReference(catalog)
            .WithReference(basket)
            .WaitFor(catalog)
            .WaitFor(basket);

        builder.Build().Run();
}
}

namespace WebApp
{
    using Catalgo;
    using Product = Catalgo.Product;
    using ServiceDefaults;

    public class CatalogApiClient(HttpClient httpClient)
    {
        public async Task<List<Product>> GetProducts()
        {
            var response = await httpClient.GetFromJsonAsync<List<Product>>($"/products");
            return response!;
        }

        public async Task<Product> GetProductById(int id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"/products/{id}");
            return response!;
        }

        public async Task<string> SupportProducts(string query)
        {
            var response = await httpClient.GetFromJsonAsync<string>($"/products/support/{query}");
            return response!;
        }

        public async Task<List<Product>?> SearchProducts(string query, bool aiSearch)
        {
            if (aiSearch)
            {
                return await httpClient.GetFromJsonAsync<List<Product>>($"/products/aisearch/{query}");
            }
            else
            {
                return await httpClient.GetFromJsonAsync<List<Product>>($"/products/search/{query}");
            }
        }
    }

    public partial class Program
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.AddServiceDefaults();

        builder.Services.AddHttpClient<CatalogApiClient>(client =>
        {
            client.BaseAddress = new("https+http://catalog");
        });

        builder.AddRedisOutputCache("cache");

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapDefaultEndpoints();

        app.UseOutputCache();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}