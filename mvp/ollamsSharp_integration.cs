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
#:package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL:9.1.0
#:package CommunityToolkit.Aspire.OllamaSharp@9.2.1-beta.207
#:package Microsoft.EntityFrameworkCore.Tools@9.0.2
#:package Microsoft.Extensions.VectorData.Abstractions@9.0.0-preview.1.25078.1
#:package Microsoft.SemanticKernel.Connectors.InMemory@1.40.0-preview
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.OllamsSharp
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
using Microsoft.SemanticKernel;
using Microsoft.EntityFrameworkCore;
using ServiceDefaults.Messaging;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100MB
    options.ListenLocalhost(7233); // http
    // options.ListenAnyIP(5000, listenOptions => listenOptions.UseConnectionLogging());
    options.ListenAnyIP(7234, o => o.UseHttps()); // https，若有证书
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
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration);
    logging.AddSerilog(Log.Logger, true);
});
builder.Services.AddOptions();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination", "X-Platform-Type", "X-CSRF-TOKEN-HEADERNAME", "X-CSRF-TOKEN", "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Forwarded-Prefix", "X-Body-Hash")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
// 自定义服务
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

builder.AddOllamaSharpChatClient("ollama-llama3-2");
builder.AddOllamaSharpEmbeddingGenerator("ollama-all-minilm");

builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

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
app.UseAuthorization();
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
app.MapOpenApi(); // 映射OpenApi文档路径,http://localhost:5000/openapi/v1.json
app.MapScalarApiReference(); // 映射Scalar的API参考文档路径,http://localhost:5000/scalar
app.UseShowAllServicesMiddleware();

app.MapDefaultEndpoints();
app.UseMigration();
app.MapProductEndpoints();
app.UseHttpsRedirection();

app.MapGet("/", () => "Ollama Agent Integration!");
app.UseEndpoints(options =>
{
    options.MapControllers();
});
Console.WriteLine("Routes registered");
await app.RunAsync();

namespace ServiceDefaults
{

}

namespace Catalgo
{
    using System.Numerics;
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.VectorData;
    using Microsoft.Extensions.VectorData;
    using System.ComponentModel.DataAnnotations.Schema;

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

    public class ProductAIService(ProductDbContext dbContext, IChatClient chatClient, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IVectorStoreRecordCollection<int, ProductVector> productVectorCollection)
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
            var chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage(ChatRole.User, query)
            };

            var resultPrompt = await chatClient.GetResponseAsync(chatHistory);
            return resultPrompt.Message.Contents[0].ToString()!;
        }
        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {
            if (!await productVectorCollection.CollectionExistsAsync())
            {
                await InitEmbeddingsAsync();
            }
            var queryEmbedding = await embeddingGenerator.GenerateEmbeddingVectorAsync(query);
            var vectorSearchOptions = new VectorSearchOptions
            {
                Top = 1,
                VectorPropertyName = "Vector"
            };
            var results = await productVectorCollection.VectorizedSearchAsync(queryEmbedding, vectorSearchOptions);
            List<Product> products = [];
            await foreach (var resultItem in results.Results)
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
            await productVectorCollection.CreateCollectionIfNotExistsAsync();
            var products = await dbContext.Products.ToListAsync();
            foreach (var product in products)
            {
                var productInfo = $"[{product.Name}] is a product that costs [{product.Price}] and is described as [{product.Description}]";
                var productVector = new ProductVector
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Vector = await embeddingGenerator.GenerateEmbeddingVectorAsync(productInfo),
                };
                await productVectorCollection.UpsertAsync(productVector);
            }
        }
    }

    public class ProductService(ProductDbContext dbContext, INumberBase bus)
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
                    ImageUrl = inputProduct.ImageUrl,
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
        [VectorStoreRecordKey]
        public int Id { get; set; }
        [VectorStoreRecordData]
        public string Name { get; set; } = default!;
        [VectorStoreRecordData]
        public string Description { get; set; } = default!;
        [VectorStoreRecordData]
        public decimal Price { get; set; }
        [VectorStoreRecordData]
        public string ImageUrl { get; set; } = default!;
        [NotMapped]
        [VectorStoreRecordVector(384, DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
}