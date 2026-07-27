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

using App;
using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
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
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
// });

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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(); // scalar
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseSerilogRequestLogging(opt =>
{
    opt.IncludeQueryInRequestPath = true;
});
app.UseRouting();
app.UseCors();
// app.UseAuthorization();
// app.UseSwagger();
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot API V1.1.0.0");
// });
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

    // 1. 元数据实体模型
    public class AssemblyInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Culture { get; set; }
        public string PublicKeyToken { get; set; }
        public bool IsRetargetable { get; set; }
        public List<TypeInfo> Types { get; set; } = new List<TypeInfo>();
        public List<string> ReferencedAssemblies { get; set; } = new List<string>();
        public AssemblyStats Stats { get; set; } = new AssemblyStats();
    }

    public class TypeInfo
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public TypeAttributes Attributes { get; set; }
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();
        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();
        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();
        public string BaseType { get; set; }
        public List<string> Interfaces { get; set; } = new List<string>();
    }

    public class MethodInfo
    {
        public string Name { get; set; }
        public MethodAttributes Attributes { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
        public string ReturnType { get; set; }
        public bool IsPublic => Attributes.HasFlag(MethodAttributes.Public);
        public bool IsStatic => Attributes.HasFlag(MethodAttributes.Static);
        public string Signature { get; set; }
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public ushort Sequence { get; set; }
    }

    public class PropertyInfo
    {
        public string Name { get; set; }
        public PropertyAttributes Attributes { get; set; }
        public string Type { get; set; }
        public List<MethodInfo> Accessors { get; set; } = new List<MethodInfo>();
    }

    public class FieldInfo
    {
        public string Name { get; set; }
        public FieldAttributes Attributes { get; set; }
        public string Type { get; set; }
        public bool IsPublic => Attributes.HasFlag(FieldAttributes.Public);
        public bool IsStatic => Attributes.HasFlag(FieldAttributes.Static);
    }

    // 2. 元数据统计信息
    public class AssemblyStats
    {
        public int TotalTypeCount { get; set; }
        public int PublicTypeCount { get; set; }
        public int TotalMethodCount { get; set; }
        public int PublicMethodCount { get; set; }
        public int TotalPropertyCount { get; set; }
        public int TotalFieldCount { get; set; }
        public long MetadataSize { get; set; }
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    }

    // 3. 异常定义
    public class MetadataAnalysisException : Exception
    {
        public string AssemblyPath { get; }

        public MetadataAnalysisException(string message, string assemblyPath)
            : base(message)
        {
            AssemblyPath = assemblyPath;
        }

        public MetadataAnalysisException(string message, Exception innerException, string assemblyPath)
            : base(message, innerException)
        {
            AssemblyPath = assemblyPath;
        }
    }

    public class MetadataValidationException : Exception
    {
        public string ErrorType { get; }
        public string Details { get; }

        public MetadataValidationException(string errorType, string details)
            : base($"{errorType}: {details}")
        {
            ErrorType = errorType;
            Details = details;
        }
    }

    // 4. 生产级元数据分析服务
    public class MetadataAnalysisService
    {
        private readonly ILogger<MetadataAnalysisService> _logger;
        private readonly SemaphoreSlim _fileAccessSemaphore;

        public MetadataAnalysisService(ILogger<MetadataAnalysisService> logger)
        {
            _logger = logger;
            _fileAccessSemaphore = new SemaphoreSlim(5, 5); // 限制并发文件访问
        }

        /// <summary>
        /// 异步读取和分析程序集元数据
        /// </summary>
        public async Task<AssemblyInfo> AnalyzeAssemblyAsync(
            string assemblyPath,
            MetadataAnalysisOptions options = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting metadata analysis for assembly: {AssemblyPath}", assemblyPath);

            if (options == null)
                options = new MetadataAnalysisOptions();

            await _fileAccessSemaphore.WaitAsync(cancellationToken);

            try
            {
                // 验证文件存在性和可读性
                await ValidateFileAsync(assemblyPath, cancellationToken);

                // 打开PE文件进行元数据读取
                using var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);
                using var peReader = new PEReader(stream);

                // 验证PE文件有效性
                await ValidatePEFileAsync(peReader, assemblyPath);

                // 读取元数据
                var metadataReader = peReader.GetMetadataReader();

                // 分析程序集信息
                var assemblyInfo = await AnalyzeCoreAsync(metadataReader, assemblyPath, options, cancellationToken);

                _logger.LogInformation("Metadata analysis completed for assembly: {AssemblyPath} - Types: {TypeCount}",
                    assemblyPath, assemblyInfo.Types.Count);

                return assemblyInfo;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Metadata analysis cancelled for assembly: {AssemblyPath}", assemblyPath);
                throw new MetadataAnalysisException("Analysis cancelled", assemblyPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing assembly metadata: {AssemblyPath}", assemblyPath);
                throw new MetadataAnalysisException($"Metadata analysis failed: {ex.Message}", ex, assemblyPath);
            }
            finally
            {
                _fileAccessSemaphore.Release();
            }
        }

        /// <summary>
        /// 分析多个程序集元数据
        /// </summary>
        public async Task<List<AssemblyInfo>> AnalyzeAssembliesAsync(
            List<string> assemblyPaths,
            MetadataAnalysisOptions options = null,
            int maxConcurrency = 5,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Analyzing batch of {AssemblyCount} assemblies", assemblyPaths.Count);

            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            var results = new List<Task<AssemblyInfo>>();

            try
            {
                foreach (var assemblyPath in assemblyPaths)
                {
                    // 并发分析程序集
                    var task = Task.Run(async () =>
                    {
                        await semaphore.WaitAsync(cancellationToken);
                        try
                        {
                            return await AnalyzeAssemblyAsync(assemblyPath, options, cancellationToken);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellationToken);

                    results.Add(task);
                }

                // 等待所有分析完成
                var assemblyInfos = await Task.WhenAll(results);

                _logger.LogInformation("Batch metadata analysis completed - Successfully analyzed: {SuccessCount}",
                    assemblyInfos.Count(info => info != null));

                return new List<AssemblyInfo>(assemblyInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch assembly metadata analysis");
                throw new MetadataAnalysisException("Batch analysis failed", ex, "batch");
            }
        }

        /// <summary>
        /// 验证文件可访问性
        /// </summary>
        private async Task ValidateFileAsync(string assemblyPath, CancellationToken cancellationToken)
        {
            await Task.Yield(); // 异步上下文切换

            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new MetadataValidationException("FileValidation", "Assembly path cannot be null or empty");
            }

            if (!File.Exists(assemblyPath))
            {
                throw new MetadataValidationException("FileValidation", $"Assembly file does not exist: {assemblyPath}");
            }

            var fileInfo = new FileInfo(assemblyPath);
            if (fileInfo.Length == 0)
            {
                throw new MetadataValidationException("FileValidation", "Assembly file is empty");
            }
        }

        /// <summary>
        /// 验证PE文件有效性
        /// </summary>
        private async Task ValidatePEFileAsync(PEReader peReader, string assemblyPath)
        {
            await Task.Yield();

            if (peReader.HasMetadata)
            {
                _logger.LogDebug("PE file contains metadata: {AssemblyPath}", assemblyPath);
            }
            else
            {
                throw new MetadataValidationException("PEValidation", "File does not contain .NET metadata");
            }

            try
            {
                var isManaged = peReader.PEHeaders.CorHeader != null;
                if (!isManaged)
                {
                    throw new MetadataValidationException("PEValidation", "File is not a managed .NET assembly");
                }
            }
            catch (BadImageFormatException ex)
            {
                throw new MetadataValidationException("PEValidation", $"Invalid PE file format: {ex.Message}");
            }
        }

        /// <summary>
        /// 核心元数据分析逻辑
        /// </summary>
        private async Task<AssemblyInfo> AnalyzeCoreAsync(
            MetadataReader metadataReader,
            string assemblyPath,
            MetadataAnalysisOptions options,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Analyzing core metadata for {AssemblyPath}", assemblyPath);

            try
            {
                var assemblyInfo = new AssemblyInfo();

                // 读取程序集定义信息
                var assemblyDef = metadataReader.GetAssemblyDefinition();
                assemblyInfo.Name = metadataReader.GetString(assemblyDef.Name);
                assemblyInfo.Version = assemblyDef.Version.ToString();
                assemblyInfo.Culture = metadataReader.GetString(assemblyDef.Culture);
                assemblyInfo.PublicKeyToken = GetPublicKeyToken(assemblyDef.PublicKey);
                assemblyInfo.IsRetargetable = assemblyDef.Flags.HasFlag(AssemblyFlags.Retargetable);

                // 统计元数据大小
                assemblyInfo.Stats.MetadataSize = metadataReader.MetadataLength;

                // 读取程序集引用
                await ReadAssemblyReferencesAsync(metadataReader, assemblyInfo, cancellationToken);

                // 读取类型定义
                await ReadTypeDefinitionsAsync(metadataReader, assemblyInfo, options, cancellationToken);

                // 计算统计信息
                CalculateAssemblyStats(assemblyInfo);

                await cancellationToken.ThrowIfCancellationRequested();

                return assemblyInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in core metadata analysis for {AssemblyPath}", assemblyPath);
                throw new MetadataAnalysisException("Core analysis failed", ex, assemblyPath);
            }
        }

        /// <summary>
        /// 读取程序集引用信息
        /// </summary>
        private async Task ReadAssemblyReferencesAsync(
            MetadataReader metadataReader,
            AssemblyInfo assemblyInfo,
            CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                foreach (var handle in metadataReader.AssemblyReferences)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var assemblyRef = metadataReader.GetAssemblyReference(handle);
                    var name = metadataReader.GetString(assemblyRef.Name);
                    var version = assemblyRef.Version.ToString();
                    var culture = metadataReader.GetString(assemblyRef.Culture);
                    var publicKeyToken = GetPublicKeyToken(assemblyRef.PublicKeyOrToken);

                    var assemblyRefInfo = $"{name}, Version={version}, Culture={culture}, PublicKeyToken={publicKeyToken}";
                    assemblyInfo.ReferencedAssemblies.Add(assemblyRefInfo);
                }

                _logger.LogDebug("Read {ReferenceCount} assembly references", assemblyInfo.ReferencedAssemblies.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Warning: Error reading assembly references - Some metadata may be corrupted");
            }
        }

        /// <summary>
        /// 读取类型定义信息
        /// </summary>
        private async Task ReadTypeDefinitionsAsync(
            MetadataReader metadataReader,
            AssemblyInfo assemblyInfo,
            MetadataAnalysisOptions options,
            CancellationToken cancellationToken)
        {
            try
            {
                foreach (var handle in metadataReader.TypeDefinitions)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (options.ReadMethods || options.ReadProperties || options.ReadFields)
                    {
                        var typeInfo = await ReadTypeDefinitionAsync(metadataReader, handle, options, cancellationToken);
                        if (typeInfo != null)
                        {
                            assemblyInfo.Types.Add(typeInfo);
                        }
                    }
                    else
                    {
                        // 仅读取基本类型信息以提高性能
                        var typeDef = metadataReader.GetTypeDefinition(handle);
                        var typeName = metadataReader.GetString(typeDef.Name);
                        var typeNamespace = metadataReader.GetString(typeDef.Namespace);

                        assemblyInfo.Types.Add(new TypeInfo
                        {
                            Name = typeName,
                            Namespace = typeNamespace,
                            Attributes = typeDef.Attributes
                        });
                    }
                }

                _logger.LogDebug("Read {TypeCount} type definitions", assemblyInfo.Types.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading type definitions");
                throw new MetadataAnalysisException("Type definitions analysis failed", ex, assemblyInfo.Name);
            }
        }

        /// <summary>
        /// 读取单个类型定义的详细信息
        /// </summary>
        private async Task<TypeInfo> ReadTypeDefinitionAsync(
            MetadataReader metadataReader,
            TypeDefinitionHandle handle,
            MetadataAnalysisOptions options,
            CancellationToken cancellationToken)
        {
            try
            {
                var typeDef = metadataReader.GetTypeDefinition(handle);

                // 基本类型信息
                var typeInfo = new TypeInfo
                {
                    Name = metadataReader.GetString(typeDef.Name),
                    Namespace = metadataReader.GetString(typeDef.Namespace),
                    Attributes = typeDef.Attributes
                };

                // 基类型信息
                if (!typeDef.BaseType.IsNil)
                {
                    typeInfo.BaseType = GetTypeString(metadataReader, typeDef.BaseType);
                }

                // 接口实现
                if (options.ReadInterfaces)
                {
                    foreach (var interfaceImplHandle in typeDef.GetInterfaceImplementations())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var interfaceImpl = metadataReader.GetInterfaceImplementation(interfaceImplHandle);
                        var interfaceName = GetTypeString(metadataReader, interfaceImpl.Interface);
                        typeInfo.Interfaces.Add(interfaceName);
                    }
                }

                // 方法定义
                if (options.ReadMethods)
                {
                    var methodHandles = typeDef.GetMethods();
                    foreach (var methodHandle in methodHandles)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var methodInfo = await ReadMethodAsync(metadataReader, methodHandle, cancellationToken);
                        if (methodInfo != null)
                        {
                            typeInfo.Methods.Add(methodInfo);
                        }
                    }
                }

                // 属性定义
                if (options.ReadProperties)
                {
                    var propertyHandles = typeDef.GetProperties();
                    foreach (var propertyHandle in propertyHandles)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var propertyInfo = await ReadPropertyAsync(metadataReader, propertyHandle, cancellationToken);
                        if (propertyInfo != null)
                        {
                            typeInfo.Properties.Add(propertyInfo);
                        }
                    }
                }

                // 字段定义
                if (options.ReadFields)
                {
                    var fieldHandles = typeDef.GetFields();
                    foreach (var fieldHandle in fieldHandles)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var fieldInfo = await ReadFieldAsync(metadataReader, fieldHandle, cancellationToken);
                        if (fieldInfo != null)
                        {
                            typeInfo.Fields.Add(fieldInfo);
                        }
                    }
                }

                return typeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read type definition - skipping type");
                return null;
            }
        }

        /// <summary>
        /// 读取方法信息
        /// </summary>
        private async Task<MethodInfo> ReadMethodAsync(
            MetadataReader metadataReader,
            MethodDefinitionHandle handle,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var methodDef = metadataReader.GetMethodDefinition(handle);
                var methodName = metadataReader.GetString(methodDef.Name);

                var methodInfo = new MethodInfo
                {
                    Name = methodName,
                    Attributes = methodDef.Attributes,
                    ReturnType = GetTypeString(metadataReader, methodDef.Signature.ReturnType)
                };

                // 参数信息
                var parameters = methodDef.GetParameters();
                foreach (var paramHandle in parameters)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var param = metadataReader.GetParameter(paramHandle);
                    var paramName = metadataReader.GetString(param.Name);

                    methodInfo.Parameters.Add(new ParameterInfo
                    {
                        Name = paramName,
                        Type = GetTypeString(metadataReader, param.Type),
                        Sequence = param.SequenceNumber
                    });
                }

                // 方法签名
                methodInfo.Signature = BuildMethodSignature(methodDef, metadataReader);

                return methodInfo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read method definition - skipping method");
                return null;
            }
        }

        /// <summary>
        /// 读取属性信息
        /// </summary>
        private async Task<PropertyInfo> ReadPropertyAsync(
            MetadataReader metadataReader,
            PropertyDefinitionHandle handle,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var propertyDef = metadataReader.GetPropertyDefinition(handle);
                var propertyName = metadataReader.GetString(propertyDef.Name);

                return new PropertyInfo
                {
                    Name = propertyName,
                    Attributes = propertyDef.Attributes,
                    Type = GetTypeString(metadataReader, propertyDef.Signature)
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read property definition - skipping property");
                return null;
            }
        }

        /// <summary>
        /// 读取字段信息
        /// </summary>
        private async Task<FieldInfo> ReadFieldAsync(
            MetadataReader metadataReader,
            FieldDefinitionHandle handle,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var fieldDef = metadataReader.GetFieldDefinition(handle);
                var fieldName = metadataReader.GetString(fieldDef.Name);

                return new FieldInfo
                {
                    Name = fieldName,
                    Attributes = fieldDef.Attributes,
                    Type = GetTypeString(metadataReader, fieldDef.Signature)
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read field definition - skipping field");
                return null;
            }
        }

        /// <summary>
        /// 获取类型字符串表示
        /// </summary>
        private string GetTypeString(MetadataReader reader, EntityHandle handle)
        {
            try
            {
                return handle.Kind switch
                {
                    HandleKind.TypeDefinition =>
                        reader.GetString(reader.GetTypeDefinition((TypeDefinitionHandle)handle).Name),
                    HandleKind.TypeReference =>
                        reader.GetString(reader.GetTypeReference((TypeReferenceHandle)handle).Name),
                    HandleKind.TypeSpecification =>
                        "TypeSpecification", // 复杂类型规范
                    _ => "Unknown"
                };
            }
            catch
            {
                return "ErrorReadingType";
            }
        }

        /// <summary>
        /// 获取公钥令牌
        /// </summary>
        private string GetPublicKeyToken(BlobHandle publicKeyHandle)
        {
            try
            {
                if (publicKeyHandle.IsNil)
                    return "null";

                // 160位SHA-1哈希
                using var sha = SHA1.Create();
                var publicKeyBytes = publicKeyHandle.ToBlobReader().ReadBytes();
                var hash = sha.ComputeHash(publicKeyBytes);

                // 取最后8字节作为令牌
                var tokenBytes = new byte[8];
                Array.Copy(hash, hash.Length - 8, tokenBytes, 0, 8);
                Array.Reverse(tokenBytes);

                return BitConverter.ToString(tokenBytes).Replace("-", "").ToLower();
            }
            catch
            {
                return "invalid";
            }
        }

        /// <summary>
        /// 构建方法签名字符串
        /// </summary>
        private string BuildMethodSignature(MethodDefinition methodDef, MetadataReader reader)
        {
            try
            {
                var signature = methodDef.Signature.ToBlobReader(reader);
                var signatureReader = new SignatureDecoder<string, object>(new TypeProvider(), reader);
                return signatureReader.DecodeMethodSignature(ref signature, null);
            }
            catch
            {
                return "ErrorDecodingSignature";
            }
        }

        /// <summary>
        /// 计算程序集统计信息
        /// </summary>
        private void CalculateAssemblyStats(AssemblyInfo assemblyInfo)
        {
            assemblyInfo.Stats.TotalTypeCount = assemblyInfo.Types.Count;
            assemblyInfo.Stats.PublicTypeCount = assemblyInfo.Types.Count(t =>
                t.Attributes.HasFlag(TypeAttributes.Public));

            assemblyInfo.Stats.TotalMethodCount = assemblyInfo.Types.Sum(t => t.Methods.Count);
            assemblyInfo.Stats.PublicMethodCount = assemblyInfo.Types.Sum(t =>
                t.Methods.Count(m => m.IsPublic));

            assemblyInfo.Stats.TotalPropertyCount = assemblyInfo.Types.Sum(t => t.Properties.Count);
            assemblyInfo.Stats.TotalFieldCount = assemblyInfo.Types.Sum(t => t.Fields.Count);
        }
    }

    // 5. 元数据分析选项配置
    public class MetadataAnalysisOptions
    {
        public bool ReadMethods { get; set; } = true;           // 是否读取方法信息
        public bool ReadProperties { get; set; } = true;        // 是否读取属性信息
        public bool ReadFields { get; set; } = false;           // 是否读取字段信息（默认关闭性能优化）
        public bool ReadInterfaces { get; set; } = true;        // 是否读取接口实现
        public bool IncludePrivateMembers { get; set; } = false; // 是否包含私有成员
        public int MaxTypesToRead { get; set; } = int.MaxValue; // 最大读取类型数限制
        public List<string> IncludeNamespaces { get; set; } = new List<string>(); // 限定读取命名空间
        public List<string> ExcludeNamespaces { get; set; } = new List<string>(); // 排除的命名空间
    }

    // 6. 类型信息提供器（用于签名解码）
    public class TypeProvider : ISignatureTypeProvider<string, object>
    {
        public string GetSZArrayType(string elementType)
        {
            return elementType + "[]";
        }

        public string GetPointerType(string elementType)
        {
            return elementType + "*";
        }

        public string GetByReferenceType(string elementType)
        {
            return elementType + "&";
        }

        public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
        {
            return $"{genericType}<{string.Join(", ", typeArguments)}>";
        }

        public string GetArrayType(string elementType, ArrayShape shape)
        {
            var dimensions = string.Join(", ", shape.Sizes.Select(s => s.ToString()));
            return $"{elementType}[{dimensions}]";
        }

        public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            var typeDef = reader.GetTypeDefinition(handle);
            var name = reader.GetString(typeDef.Name);
            var ns = reader.GetString(typeDef.Namespace);
            return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
        }

        public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            var typeRef = reader.GetTypeReference(handle);
            var name = reader.GetString(typeRef.Name);
            var ns = reader.GetString(typeRef.Namespace);
            return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
        }

        public string GetTypeFromSpecification(MetadataReader reader, object genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
        {
            return "TypeSpec";
        }

        public string GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return typeCode switch
            {
                PrimitiveTypeCode.Boolean => "bool",
                PrimitiveTypeCode.Char => "char",
                PrimitiveTypeCode.SByte => "sbyte",
                PrimitiveTypeCode.Byte => "byte",
                PrimitiveTypeCode.Int16 => "short",
                PrimitiveTypeCode.UInt16 => "ushort",
                PrimitiveTypeCode.Int32 => "int",
                PrimitiveTypeCode.UInt32 => "uint",
                PrimitiveTypeCode.Int64 => "long",
                PrimitiveTypeCode.UInt64 => "ulong",
                PrimitiveTypeCode.Single => "float",
                PrimitiveTypeCode.Double => "double",
                PrimitiveTypeCode.String => "string",
                PrimitiveTypeCode.Object => "object",
                _ => typeCode.ToString()
            };
        }

        public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
        {
            return isRequired ? $"{modifier} {unmodifiedType}" : $"{unmodifierType}";
        }

        public string GetPinnedType(string elementType)
        {
            return $"pinned {elementType}";
        }

        public string GetGenericMethodParameter(object genericContext, int index)
        {
            return $"!!{index}";
        }

        public string GetGenericTypeParameter(object genericContext, int index)
        {
            return $"!{index}";
        }
    }

    // 7. 元数据比较和差异分析服务
    public class MetadataComparisonService
    {
        private readonly ILogger<MetadataComparisonService> _logger;

        public MetadataComparisonService(ILogger<MetadataComparisonService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 比较两个程序集的元数据差异
        /// </summary>
        public async Task<AssemblyComparisonResult> CompareAssembliesAsync(
            string assemblyPath1,
            string assemblyPath2,
            MetadataAnalysisService analysisService,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Comparing metadata between {Assembly1} and {Assembly2}",
                assemblyPath1, assemblyPath2);

            try
            {
                // 分析两个程序集
                var assembly1 = await analysisService.AnalyzeAssemblyAsync(assemblyPath1, cancellationToken: cancellationToken);
                var assembly2 = await analysisService.AnalyzeAssemblyAsync(assemblyPath2, cancellationToken: cancellationToken);

                var result = new AssemblyComparisonResult
                {
                    Assembly1 = assembly1,
                    Assembly2 = assembly2,
                    Assembly1Path = assemblyPath1,
                    Assembly2Path = assemblyPath2,
                    ComparisonStarted = DateTime.UtcNow
                };

                // 比较引用差异
                await CompareAssemblyReferencesAsync(assembly1, assembly2, result, cancellationToken);

                // 比较类型差异
                await CompareTypesAsync(assembly1, assembly2, result, cancellationToken);

                // 比较公共API变化
                await ComparePublicApiChangesAsync(assembly1, assembly2, result, cancellationToken);

                result.ComparisonCompleted = DateTime.UtcNow;
                result.ComparisonDuration = result.ComparisonCompleted - result.ComparisonStarted;

                _logger.LogInformation("Assembly comparison completed in {Duration}ms",
                    result.ComparisonDuration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing assemblies: {Assembly1} vs {Assembly2}",
                    assemblyPath1, assemblyPath2);
                throw new MetadataAnalysisException("Assembly comparison failed", ex, "comparison");
            }
        }

        private async Task CompareAssemblyReferencesAsync(
            AssemblyInfo assembly1,
            AssemblyInfo assembly2,
            AssemblyComparisonResult result,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var refs1 = assembly1.ReferencedAssemblies.ToHashSet();
                var refs2 = assembly2.ReferencedAssemblies.ToHashSet();

                result.NewReferences = refs2.Except(refs1).ToList();
                result.RemovedReferences = refs1.Except(refs2).ToList();
                result.ChangedReferences = refs1.Intersect(refs2)
                    .Where(r => !r.Equals(refs2.FirstOrDefault(r2 => r2.StartsWith(r.Split(',')[0])),
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _logger.LogDebug("Reference comparison completed - New: {NewCount}, Removed: {RemovedCount}",
                    result.NewReferences.Count, result.RemovedReferences.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Warning: Error comparing assembly references");
            }
        }

        private async Task CompareTypesAsync(
            AssemblyInfo assembly1,
            AssemblyInfo assembly2,
            AssemblyComparisonResult result,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var types1 = assembly1.Types.ToDictionary(t => $"{t.Namespace}.{t.Name}", t => t);
                var types2 = assembly2.Types.ToDictionary(t => $"{t.Namespace}.{t.Name}", t => t);

                // 新增的类型
                result.NewTypes = types2.Keys.Except(types1.Keys)
                    .Select(key => types2[key]).ToList();

                // 移除的类型
                result.RemovedTypes = types1.Keys.Except(types2.Keys)
                    .Select(key => types1[key]).ToList();

                // 改变的类型（仅比较公共类型）
                var commonTypes = types1.Keys.Intersect(types2.Keys);
                result.ChangedTypes = new List<TypeChangeInfo>();

                foreach (var typeName in commonTypes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var type1 = types1[typeName];
                    var type2 = types2[typeName];

                    var typeChanges = AnalyzeTypeChanges(type1, type2, typeName);
                    if (typeChanges.HasChanges)
                    {
                        result.ChangedTypes.Add(typeChanges);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Warning: Error comparing type definitions");
            }
        }

        private TypeChangeInfo AnalyzeTypeChanges(TypeInfo type1, TypeInfo type2, string typeName)
        {
            var changes = new TypeChangeInfo { TypeName = typeName };

            try
            {
                // 比较方法变化
                var methods1 = type1.Methods.ToDictionary(m => m.Signature, m => m);
                var methods2 = type2.Methods.ToDictionary(m => m.Signature, m => m);

                changes.NewMethods = methods2.Keys.Except(methods1.Keys)
                    .Select(key => methods2[key]).ToList();

                changes.RemovedMethods = methods1.Keys.Except(methods2.Keys)
                    .Select(key => methods1[key]).ToList();

                // 比较属性变化
                var properties1 = type1.Properties.ToDictionary(p => p.Name, p => p);
                var properties2 = type2.Properties.ToDictionary(p => p.Name, p => p);

                changes.NewProperties = properties2.Keys.Except(properties1.Keys)
                    .Select(key => properties2[key]).ToList();

                changes.RemovedProperties = properties1.Keys.Except(properties2.Keys)
                    .Select(key => properties1[key]).ToList();

                // 比较字段变化
                var fields1 = type1.Fields.ToDictionary(f => f.Name, f => f);
                var fields2 = type2.Fields.ToDictionary(f => f.Name, f => f);

                changes.NewFields = fields2.Keys.Except(fields1.Keys)
                    .Select(key => fields2[key]).ToList();

                changes.RemovedFields = fields1.Keys.Except(fields2.Keys)
                    .Select(key => fields1[key]).ToList();

                changes.HasChanges = changes.NewMethods.Any() || changes.RemovedMethods.Any() ||
                                    changes.NewProperties.Any() || changes.RemovedProperties.Any() ||
                                    changes.NewFields.Any() || changes.RemovedFields.Any();
            }
            catch (Exception ex)
            {
                changes.HasChanges = true;
                changes.ErrorDescription = ex.Message;
            }

            return changes;
        }

        private async Task ComparePublicApiChangesAsync(
            AssemblyInfo assembly1,
            AssemblyInfo assembly2,
            AssemblyComparisonResult result,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var publicTypes1 = assembly1.Types.Where(t => t.Attributes.HasFlag(TypeAttributes.Public)).ToList();
                var publicTypes2 = assembly2.Types.Where(t => t.Attributes.HasFlag(TypeAttributes.Public)).ToList();

                result.PublicApiNewTypes = result.NewTypes.Where(t =>
                    t.Attributes.HasFlag(TypeAttributes.Public)).ToList();

                result.PublicApiRemovedTypes = result.RemovedTypes.Where(t =>
                    t.Attributes.HasFlag(TypeAttributes.Public)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Warning: Error comparing public API changes");
            }
        }
    }

    // 8. 比较结果模型
    public class AssemblyComparisonResult
    {
        public AssemblyInfo Assembly1 { get; set; }
        public AssemblyInfo Assembly2 { get; set; }
        public string Assembly1Path { get; set; }
        public string Assembly2Path { get; set; }
        public List<string> NewReferences { get; set; } = new List<string>();
        public List<string> RemovedReferences { get; set; } = new List<string>();
        public List<string> ChangedReferences { get; set; } = new List<string>();
        public List<TypeInfo> NewTypes { get; set; } = new List<TypeInfo>();
        public List<TypeInfo> RemovedTypes { get; set; } = new List<TypeInfo>();
        public List<TypeChangeInfo> ChangedTypes { get; set; } = new List<TypeChangeInfo>();
        public List<TypeInfo> PublicApiNewTypes { get; set; } = new List<TypeInfo>();
        public List<TypeInfo> PublicApiRemovedTypes { get; set; } = new List<TypeInfo>();
        public DateTime ComparisonStarted { get; set; }
        public DateTime ComparisonCompleted { get; set; }
        public TimeSpan ComparisonDuration { get; set; }

        public int TotalApiChanges => NewReferences.Count + RemovedReferences.Count +
                                     NewTypes.Count + RemovedTypes.Count +
                                     ChangedTypes.Sum(ct => ct.TotalChanges);
    }

    public class TypeChangeInfo
    {
        public string TypeName { get; set; }
        public List<MethodInfo> NewMethods { get; set; } = new List<MethodInfo>();
        public List<MethodInfo> RemovedMethods { get; set; } = new List<MethodInfo>();
        public List<PropertyInfo> NewProperties { get; set; } = new List<PropertyInfo>();
        public List<PropertyInfo> RemovedProperties { get; set; } = new List<PropertyInfo>();
        public List<FieldInfo> NewFields { get; set; } = new List<FieldInfo>();
        public List<FieldInfo> RemovedFields { get; set; } = new List<FieldInfo>();
        public bool HasChanges { get; set; }
        public string ErrorDescription { get; set; }

        public int TotalChanges => NewMethods.Count + RemovedMethods.Count +
                                  NewProperties.Count + RemovedProperties.Count +
                                  NewFields.Count + RemovedFields.Count;
    }

    // 9. 元数据查询和搜索服务
    public class MetadataQueryService
    {
        private readonly ILogger<MetadataQueryService> _logger;

        public MetadataQueryService(ILogger<MetadataQueryService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 根据名称搜索类型
        /// </summary>
        public List<TypeSearchResult> SearchTypes(
            AssemblyInfo assembly,
            string searchTerm,
            bool includeMethods = true,
            bool includeProperties = true)
        {
            _logger.LogDebug("Searching types in assembly {AssemblyName} for term '{SearchTerm}'",
                assembly.Name, searchTerm);

            var results = new List<TypeSearchResult>();

            try
            {
                foreach (var type in assembly.Types)
                {
                    if (type.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        type.Namespace.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add(new TypeSearchResult
                        {
                            Type = type,
                            MatchScore = CalculateMatchScore(type, searchTerm),
                            MatchType = SearchMatchType.TypeName,
                            AssemblyName = assembly.Name
                        });
                    }

                    if (includeMethods)
                    {
                        foreach (var method in type.Methods.Where(m =>
                            m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                        {
                            results.Add(new TypeSearchResult
                            {
                                Type = type,
                                Method = method,
                                MatchScore = CalculateMethodMatchScore(method, searchTerm),
                                MatchType = SearchMatchType.MethodName,
                                AssemblyName = assembly.Name
                            });
                        }
                    }

                    if (includeProperties)
                    {
                        foreach (var property in type.Properties.Where(p =>
                            p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                        {
                            results.Add(new TypeSearchResult
                            {
                                Type = type,
                                Property = property,
                                MatchScore = CalculatePropertyMatchScore(property, searchTerm),
                                MatchType = SearchMatchType.PropertyName,
                                AssemblyName = assembly.Name
                            });
                        }
                    }
                }

                // 按匹配分数排序
                results = results.OrderByDescending(r => r.MatchScore).ToList();

                _logger.LogInformation("Search completed - Found {ResultCount} matches", results.Count);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching types in assembly {AssemblyName}", assembly.Name);
                throw new MetadataAnalysisException("Type search failed", ex, assembly.Name);
            }
        }

        /// <summary>
        /// 根据条件查询类型
        /// </summary>
        public List<TypeInfo> QueryTypes(AssemblyInfo assembly, TypeQueryFilter filter)
        {
            _logger.LogDebug("Querying types in assembly {AssemblyName} with filter", assembly.Name);

            var query = assembly.Types.AsQueryable();

            if (filter.PublicOnly)
            {
                query = query.Where(t => t.Attributes.HasFlag(TypeAttributes.Public));
            }

            if (!string.IsNullOrEmpty(filter.Namespace))
            {
                query = query.Where(t => t.Namespace.Equals(filter.Namespace, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.HasMethods)
            {
                query = query.Where(t => t.Methods.Any());
            }

            if (filter.InheritFrom != null)
            {
                query = query.Where(t => t.BaseType != null &&
                    t.BaseType.Contains(filter.InheritFrom, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.ImplementInterface != null)
            {
                query = query.Where(t => t.Interfaces.Any(i =>
                    i.Contains(filter.ImplementInterface, StringComparison.OrdinalIgnoreCase)));
            }

            return query.ToList();
        }

        private double CalculateMatchScore(TypeInfo type, string searchTerm)
        {
            var score = 0.0;

            if (type.Name.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                score += 10.0;
            else if (type.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                score += 5.0;

            if (type.Namespace.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                score += 2.0;

            return score;
        }

        private double CalculateMethodMatchScore(MethodInfo method, string searchTerm)
        {
            if (method.Name.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                return 8.0;
            else if (method.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                return 4.0;

            return 0.0;
        }

        private double CalculatePropertyMatchScore(PropertyInfo property, string searchTerm)
        {
            if (property.Name.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                return 6.0;
            else if (property.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                return 3.0;

            return 0.0;
        }
    }

    // 10. 类型搜索相关模型
    public class TypeSearchResult
    {
        public TypeInfo Type { get; set; }
        public MethodInfo Method { get; set; }
        public PropertyInfo Property { get; set; }
        public double MatchScore { get; set; }
        public SearchMatchType MatchType { get; set; }
        public string AssemblyName { get; set; }
    }

    public enum SearchMatchType
    {
        TypeName,
        MethodName,
        PropertyName,
        Namespace
    }

    public class TypeQueryFilter
    {
        public bool PublicOnly { get; set; } = true;
        public string Namespace { get; set; }
        public bool HasMethods { get; set; } = false;
        public string InheritFrom { get; set; }
        public string ImplementInterface { get; set; }
        public string SearchTerm { get; set; }
    }

    // 11. 元数据安全和完整性验证服务
    public class MetadataValidationService
    {
        private readonly ILogger<MetadataValidationService> _logger;

        public MetadataValidationService(ILogger<MetadataValidationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 验证程序集元数据完整性
        /// </summary>
        public async Task<MetadataValidationResult> ValidateAssemblyMetadataAsync(
            string assemblyPath,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Validating metadata integrity for assembly: {AssemblyPath}", assemblyPath);

            try
            {
                using var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);
                using var peReader = new PEReader(stream);

                var result = new MetadataValidationResult
                {
                    AssemblyPath = assemblyPath,
                    ValidationStarted = DateTime.UtcNow,
                    IsPEValid = true,
                    HasMetadata = peReader.HasMetadata
                };

                if (!result.HasMetadata)
                {
                    result.IsValid = false;
                    result.ValidationErrors.Add("Assembly does not contain .NET metadata");
                    return result;
                }

                var metadataReader = peReader.GetMetadataReader();

                // 验证基本元数据
                await ValidateBasicMetadataAsync(metadataReader, result, cancellationToken);

                // 验证引用完整性
                await ValidateReferencesAsync(metadataReader, result, cancellationToken);

                // 验证类型完整性
                await ValidateTypesAsync(metadataReader, result, cancellationToken);

                result.IsValid = !result.ValidationErrors.Any() && !result.ValidationWarnings.Any();
                result.ValidationCompleted = DateTime.UtcNow;
                result.ValidationDuration = result.ValidationCompleted - result.ValidationStarted;

                _logger.LogInformation("Metadata validation completed for {AssemblyPath} - Valid: {IsValid}",
                    assemblyPath, result.IsValid);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating assembly metadata: {AssemblyPath}", assemblyPath);

                return new MetadataValidationResult
                {
                    AssemblyPath = assemblyPath,
                    IsValid = false,
                    IsPEValid = false,
                    ValidationErrors = new List<string> { $"PE validation failed: {ex.Message}" }
                };
            }
        }

        private async Task ValidateBasicMetadataAsync(
            MetadataReader reader,
            MetadataValidationResult result,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // 验证程序集定义
                var assemblyDef = reader.GetAssemblyDefinition();
                var assemblyName = reader.GetString(assemblyDef.Name);

                if (string.IsNullOrEmpty(assemblyName))
                {
                    result.ValidationErrors.Add("Assembly name is missing or invalid");
                }

                // 验证字符串堆
                if (reader.StringHeap.IsInvalid)
                {
                    result.ValidationErrors.Add("String heap is corrupted");
                }

                // 验证用户字符串堆
                if (reader.UserStringHeap.IsInvalid)
                {
                    result.ValidationErrors.Add("User string heap is corrupted");
                }

                // 验证Blob堆
                if (reader.BlobHeap.IsInvalid)
                {
                    result.ValidationErrors.Add("Blob heap is corrupted");
                }

                _logger.LogDebug("Basic metadata validation completed");
            }
            catch (Exception ex)
            {
                result.ValidationErrors.Add($"Basic metadata validation error: {ex.Message}");
            }
        }

        private async Task ValidateReferencesAsync(
            MetadataReader reader,
            MetadataValidationResult result,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                foreach (var assemblyRefHandle in reader.AssemblyReferences)
                {
                    var assemblyRef = reader.GetAssemblyReference(assemblyRefHandle);
                    var name = reader.GetString(assemblyRef.Name);

                    if (string.IsNullOrEmpty(name))
                    {
                        result.ValidationWarnings.Add("Found assembly reference with empty name");
                    }

                    // 验证版本信息
                    if (assemblyRef.Version.Major == 0 && assemblyRef.Version.Minor == 0)
                    {
                        result.ValidationWarnings.Add($"Assembly reference '{name}' has invalid version");
                    }
                }

                _logger.LogDebug("Assembly references validation completed");
            }
            catch (Exception ex)
            {
                result.ValidationErrors.Add($"Assembly references validation failed: {ex.Message}");
            }
        }

        private async Task ValidateTypesAsync(
            MetadataReader reader,
            MetadataValidationResult result,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                foreach (var typeDefHandle in reader.TypeDefinitions)
                {
                    // 跳过特殊类型（如模块类型等）
                    if (typeDefHandle.IsNil || typeDefHandle == default(TypeDefinitionHandle))
                        continue;

                    var typeDef = reader.GetTypeDefinition(typeDefHandle);
                    var typeName = reader.GetString(typeDef.Name);

                    if (string.IsNullOrEmpty(typeName))
                    {
                        result.ValidationWarnings.Add("Found type definition with empty name");
                        continue;
                    }

                    // 验证类型属性
                    if (!Enum.IsDefined(typeof(TypeAttributes), typeDef.Attributes))
                    {
                        result.ValidationWarnings.Add($"Type '{typeName}' has invalid attributes");
                    }
                }

                _logger.LogDebug("Type definitions validation completed");
            }
            catch (Exception ex)
            {
                result.ValidationErrors.Add($"Type definitions validation failed: {ex.Message}");
            }
        }
    }

    // 12. 元数据验证结果模型
    public class MetadataValidationResult
    {
        public string AssemblyPath { get; set; }
        public bool IsValid { get; set; }
        public bool IsPEValid { get; set; }
        public bool HasMetadata { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<string> ValidationWarnings { get; set; } = new List<string>();
        public DateTime ValidationStarted { get; set; }
        public DateTime ValidationCompleted { get; set; }
        public TimeSpan ValidationDuration { get; set; }

        public string Summary => IsValid ? "Metadata validation successful" :
            $"Validation failed with {ValidationErrors.Count} errors and {ValidationWarnings.Count} warnings";
    }

    // 13. 元数据监控和遥测服务
    public class MetadataTelemetryService
    {
        private readonly ILogger<MetadataTelemetryService> _logger;
        private readonly Dictionary<string, AssemblyMetadataMetrics> _assemblyMetrics = new Dictionary<string, AssemblyMetadataMetrics>();

        /// <summary>
        /// 跟踪元数据分析操作
        /// </summary>
        public async Task TrackAnalysisAsync(
            string assemblyPath,
            TimeSpan duration,
            long metadataSize,
            int typeCount,
            bool success)
        {
            await Task.Yield();

            var metrics = GetOrAddMetrics(assemblyPath);
            metrics.AnalysisCount++;
            metrics.TotalAnalysisTime += duration.TotalMilliseconds;
            metrics.LastMetadataSize = metadataSize;
            metrics.LastTypeCount = typeCount;

            if (success)
            {
                metrics.SuccessfulAnalyses++;
            }
            else
            {
                metrics.FailedAnalyses++;
            }

            metrics.LastAnalysisTime = DateTime.UtcNow;
            metrics.AverageAnalysisTime = metrics.SuccessfulAnalyses > 0 ?
                metrics.TotalAnalysisTime / metrics.SuccessfulAnalyses : 0;

            _logger.LogDebug("TRACKING - Assembly {Assembly}: Analysis took {Duration}ms, Success {Success}",
                assemblyPath, duration.TotalMilliseconds, success);
        }

        /// <summary>
        /// 跟踪元数据查询操作
        /// </summary>
        public async Task TrackQueryAsync(string assemblyPath, TimeSpan duration, int resultCount)
        {
            await Task.Yield();

            var metrics = GetOrAddMetrics(assemblyPath);
            metrics.QueryCount++;
            metrics.TotalQueryTime += duration.TotalMilliseconds;
            metrics.LastQueryResultCount = resultCount;
            metrics.LastQueryTime = DateTime.UtcNow;

            metrics.AverageQueryTime = metrics.QueryCount > 0 ?
                metrics.TotalQueryTime / metrics.QueryCount : 0;
        }

        /// <summary>
        /// 获取程序集元数据性能指标
        /// </summary>
        public AssemblyMetadataMetrics GetMetrics(string assemblyPath)
        {
            return _assemblyMetrics.GetValueOrDefault(assemblyPath, new AssemblyMetadataMetrics());
        }

        /// <summary>
        /// 获取所有监控指标
        /// </summary>
        public MetadataPerformanceMetrics GetAllMetrics()
        {
            return new MetadataPerformanceMetrics
            {
                AssemblyMetrics = new Dictionary<string, AssemblyMetadataMetrics>(_assemblyMetrics),
                TotalAnalyses = _assemblyMetrics.Values.Sum(m => m.AnalysisCount),
                SuccessfulAnalyses = _assemblyMetrics.Values.Sum(m => m.SuccessfulAnalyses),
                TotalQueries = _assemblyMetrics.Values.Sum(m => m.QueryCount),
                RetrievedAt = DateTime.UtcNow
            };
        }

        private AssemblyMetadataMetrics GetOrAddMetrics(string assemblyPath)
        {
            if (!_assemblyMetrics.TryGetValue(assemblyPath, out var metrics))
            {
                metrics = new AssemblyMetadataMetrics { AssemblyPath = assemblyPath };
                _assemblyMetrics[assemblyPath] = metrics;
            }

            return metrics;
        }
    }

    // 14. 监控指标模型
    public class AssemblyMetadataMetrics
    {
        public string AssemblyPath { get; set; }
        public long AnalysisCount { get; set; }
        public long SuccessfulAnalyses { get; set; }
        public long FailedAnalyses { get; set; }
        public double TotalAnalysisTime { get; set; }
        public double AverageAnalysisTime { get; set; }
        public long LastMetadataSize { get; set; }
        public int LastTypeCount { get; set; }
        public DateTime LastAnalysisTime { get; set; }
        public long QueryCount { get; set; }
        public double TotalQueryTime { get; set; }
        public double AverageQueryTime { get; set; }
        public int LastQueryResultCount { get; set; }
        public DateTime LastQueryTime { get; set; }
    }

    public class MetadataPerformanceMetrics
    {
        public Dictionary<string, AssemblyMetadataMetrics> AssemblyMetrics { get; set; } = new Dictionary<string, AssemblyMetadataMetrics>();
        public long TotalAnalyses { get; set; }
        public long SuccessfulAnalyses { get; set; }
        public long TotalQueries { get; set; }
        public DateTime RetrievedAt { get; set; }

        public double SuccessRate => TotalAnalyses > 0 ?
            (double)SuccessfulAnalyses / TotalAnalyses * 100 : 0;

        public double AverageAnalysisTime => AssemblyMetrics.Values.Any() ?
            AssemblyMetrics.Values.Average(m => m.AverageAnalysisTime) : 0;
    }

    // 15. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.Reflection.Metadata Production Demo");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册核心分析服务
            builder.Services.AddSingleton<MetadataAnalysisService>();
            builder.Services.AddSingleton<MetadataComparisonService>();
            builder.Services.AddSingleton<MetadataQueryService>();
            builder.Services.AddSingleton<MetadataValidationService>();
            builder.Services.AddSingleton<MetadataTelemetryService>();

            #endregion

            var host = builder.Build();

            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var analysisService = services.GetRequiredService<MetadataAnalysisService>();
            var comparisonService = services.GetRequiredService<MetadataComparisonService>();
            var queryService = services.GetRequiredService<MetadataQueryService>();
            var validationService = services.GetRequiredService<MetadataValidationService>();
            var telemetryService = services.GetRequiredService<MetadataTelemetryService>();

            Console.WriteLine("=== Running on .NET Runtime Assemblies ===");

            // 获取一些系统程序集路径
            var systemAssemblies = GetSystemAssemblies();

            Console.WriteLine("\n1. Basic Assembly Metadata Analysis:");
            try
            {
                if (systemAssemblies.Any())
                {
                    var assemblyPath = systemAssemblies.First();
                    var startTime = DateTime.UtcNow;

                    var assemblyInfo = await analysisService.AnalyzeAssemblyAsync(assemblyPath);

                    var duration = DateTime.UtcNow - startTime;
                    await telemetryService.TrackAnalysisAsync(
                        assemblyPath, duration, assemblyInfo.Stats.MetadataSize,
                        assemblyInfo.Types.Count, true);

                    Console.WriteLine($"   Analyzed assembly: {assemblyInfo.Name}");
                    Console.WriteLine($"   Version: {assemblyInfo.Version}");
                    Console.WriteLine($"   Metadata size: {assemblyInfo.Stats.MetadataSize} bytes");
                    Console.WriteLine($"   Types count: {assemblyInfo.Types.Count}");
                    Console.WriteLine($"   Public types: {assemblyInfo.Stats.PublicTypeCount}");
                    Console.WriteLine($"   Referenced assemblies: {assemblyInfo.ReferencedAssemblies.Count}");

                    // 显示前几个类型
                    foreach (var type in assemblyInfo.Types.Take(3))
                    {
                        Console.WriteLine($"   - Type: {type.Namespace}.{type.Name}");
                        Console.WriteLine($"     Public methods: {type.Methods.Count(m => m.IsPublic)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n2. Batch Metadata Analysis:");
            try
            {
                var batchAssemblies = systemAssemblies.Take(3).ToList();
                var batchStartTime = DateTime.UtcNow;

                var options = new MetadataAnalysisOptions
                {
                    ReadMethods = true,
                    ReadProperties = false, // 性能优化：跳过属性读取
                    ReadFields = false,     // 性能优化：跳过字段读取
                    IncludePrivateMembers = false,
                    MaxTypesToRead = 1000
                };

                var batchResults = await analysisService.AnalyzeAssembliesAsync(
                    batchAssemblies, options, maxConcurrency: 3);

                var batchDuration = DateTime.UtcNow - batchStartTime;
                Console.WriteLine($"   Batch analysis completed in {batchDuration.TotalMilliseconds:F0}ms");
                Console.WriteLine($"   Successfully analyzed {batchResults.Count} assemblies");

                foreach (var result in batchResults)
                {
                    Console.WriteLine($"   - {result.Name}: {result.Types.Count} types");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n3. Type Search and Query Demo:");
            try
            {
                if (systemAssemblies.Any())
                {
                    var assemblyInfo = await analysisService.AnalyzeAssemblyAsync(
                        systemAssemblies.First(), new MetadataAnalysisOptions { ReadMethods = true });

                    // 搜索类型
                    var searchResults = queryService.SearchTypes(assemblyInfo, "String");
                    Console.WriteLine($"   Search results for 'String': {searchResults.Count}");

                    foreach (var result in searchResults.Take(3))
                    {
                        Console.WriteLine($"   - Match: {result.Type.Name} ({result.MatchScore} points)");
                        if (result.Method != null)
                        {
                            Console.WriteLine($"     Method: {result.Method.Name}");
                        }
                    }

                    // 查询类型
                    var queryFilter = new TypeQueryFilter
                    {
                        PublicOnly = true,
                        HasMethods = true
                    };

                    var queryResults = queryService.QueryTypes(assemblyInfo, queryFilter);
                    Console.WriteLine($"   Query results: {queryResults.Count} types with methods");

                    var queryDuration = TimeSpan.FromMilliseconds(5); // 模拟查询时间
                    await telemetryService.TrackQueryAsync(
                        systemAssemblies.First(), queryDuration, queryResults.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n4. Assembly Metadata Validation:");
            try
            {
                if (systemAssemblies.Any())
                {
                    var validationResults = await validationService.ValidateAssemblyMetadataAsync(
                        systemAssemblies.First());

                    Console.WriteLine($"   Validation result: {validationResults.IsValid}");
                    Console.WriteLine($"   Has metadata: {validationResults.HasMetadata}");
                    Console.WriteLine($"   Validation errors: {validationResults.ValidationErrors.Count}");
                    Console.WriteLine($"   Validation warnings: {validationResults.ValidationWarnings.Count}");

                    if (validationResults.ValidationErrors.Any())
                    {
                        Console.WriteLine($"   Errors: {string.Join(", ", validationResults.ValidationErrors.Take(3))}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n5. Assembly Comparison (Simulated):");
            try
            {
                // 由于我们只能分析一个文件，创建模拟比较场景
                var comparison = new AssemblyComparisonResult
                {
                    Assembly1 = new AssemblyInfo { Name = "SampleAssembly1", Version = "1.0.0" },
                    Assembly2 = new AssemblyInfo { Name = "SampleAssembly2", Version = "1.1.0" },
                    NewReferences = new List<string> { "System.Text.Json, Version=6.0.0" },
                    RemovedReferences = new List<string> { "Newtonsoft.Json, Version=12.0.0" },
                    ChangedTypes = new List<TypeChangeInfo>
                {
                    new TypeChangeInfo
                    {
                        TypeName = "SampleClass",
                        NewMethods = new List<MethodInfo> { new MethodInfo { Name = "NewMethod" } },
                        RemovedMethods = new List<MethodInfo> { new MethodInfo { Name = "OldMethod" } }
                    }
                }
                };

                Console.WriteLine($"   Comparison of {comparison.Assembly1.Name} vs {comparison.Assembly2.Name}");
                Console.WriteLine($"   API changes detected: {comparison.TotalApiChanges}");
                Console.WriteLine($"   New references: {comparison.NewReferences.Count}");
                Console.WriteLine($"   Removed references: {comparison.RemovedReferences.Count}");
                Console.WriteLine($"   Changed types: {comparison.ChangedTypes.Count}");

                foreach (var typeChange in comparison.ChangedTypes)
                {
                    Console.WriteLine($"   - Type changes in {typeChange.TypeName}: {typeChange.TotalChanges} changes");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n6. Metadata Performance Telemetry:");
            try
            {
                var performanceMetrics = telemetryService.GetAllMetrics();

                Console.WriteLine($"   Total metadata analyses: {performanceMetrics.TotalAnalyses}");
                Console.WriteLine($"   Successful analyses: {performanceMetrics.SuccessfulAnalyses}");
                Console.WriteLine($"   Success rate: {performanceMetrics.SuccessRate:F2}%");
                Console.WriteLine($"   Average analysis time: {performanceMetrics.AverageAnalysisTime:F2}ms");

                foreach (var metrics in performanceMetrics.AssemblyMetrics.Take(3))
                {
                    Console.WriteLine($"   - {metrics.Key}: {metrics.Value.AnalysisCount} analyses, " +
                                    $"avg {metrics.Value.AverageAnalysisTime:F2}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n7. Advanced Metadata Analysis (Custom Options):");
            try
            {
                if (systemAssemblies.Any())
                {
                    var advancedOptions = new MetadataAnalysisOptions
                    {
                        ReadMethods = true,
                        ReadProperties = true,
                        ReadFields = true,
                        ReadInterfaces = true,
                        IncludePrivateMembers = false,
                        IncludeNamespaces = new List<string> { "System", "System.Collections" }
                    };

                    var advancedAnalysis = await analysisService.AnalyzeAssemblyAsync(
                        systemAssemblies.Skip(1).First(), advancedOptions);

                    Console.WriteLine($"   Advanced analysis of {advancedAnalysis.Name}");
                    Console.WriteLine($"   All metadata elements read: Methods={advancedAnalysis.Stats.TotalMethodCount}, " +
                                    $"Properties={advancedAnalysis.Stats.TotalPropertyCount}, Fields={advancedAnalysis.Stats.TotalFieldCount}");

                    // 读取类型详细信息
                    var complexTypes = advancedAnalysis.Types.Where(t =>
                        t.Interfaces.Any() || t.Methods.Count > 5).Take(3).ToList();

                    foreach (var type in complexTypes)
                    {
                        Console.WriteLine($"   - {type.Namespace}.{type.Name}");
                        if (type.Interfaces.Any())
                            Console.WriteLine($"     Implements: {string.Join(", ", type.Interfaces.Take(3))}");
                        Console.WriteLine($"     Methods: {type.Methods.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }
        }

        private static List<string> GetSystemAssemblies()
        {
            var assemblies = new List<string>();

            try
            {
                // 获取运行时程序集目录
                var runtimeAssembliesPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
                if (!string.IsNullOrEmpty(runtimeAssembliesPath))
                {
                    assemblies.AddRange(Directory.GetFiles(runtimeAssembliesPath, "*.dll")
                        .Where(file => !Path.GetFileName(file).StartsWith("api-ms-win-"))
                        .Take(5));
                }
            }
            catch
            {
                // Fallback to common assembly
                assemblies.Add(typeof(object).Assembly.Location);
            }

            return assemblies;
        }
    }

    public class Test
    {
        public void TaseReflection()
        {
            // 1. 结构化读取模式
            // 基本元数据读取流程
            // 使用PEReader读取PE文件
            using var peReader = new PEReader(fileStream);

            // 获取元数据读取器
            var metadataReader = peReader.GetMetadataReader();

            // 读取程序集定义
            var assemblyDef = metadataReader.GetAssemblyDefinition();

            // 2.零拷贝高性能特性
            // Handle模式读取
            // 通过Handle高效读取元数据元素
            foreach (var typeHandle in metadataReader.TypeDefinitions)
            {
                var typeDef = metadataReader.GetTypeDefinition(typeHandle);
                var typeName = metadataReader.GetString(typeDef.Name); // 字符串索引读取
                var typeNamespace = metadataReader.GetString(typeDef.Namespace);
            }

            // 3. 生产级资源管理和性能优化
            // 并发控制和文件访问限制
            private readonly SemaphoreSlim _fileAccessSemaphore = new SemaphoreSlim(maxConcurrentFiles, maxConcurrentFiles);
            await _fileAccessSemaphore.WaitAsync(cancellationToken);
            try
            {
                using var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);
                // 执行元数据分析
            }
            finally
            {
                _fileAccessSemaphore.Release();
            }

            // 6. 完整生命周期管理
            // 取消令牌支持
            public async Task<AssemblyInfo> AnalyzeAssemblyAsync(
            string assemblyPath,
            CancellationToken cancellationToken = default)
            {
                // 支持操作取消
                await cancellationToken.ThrowIfCancellationRequested();

                // 长时间操作中定期检查
                foreach (var handle in metadataReader.TypeDefinitions)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // 处理类型定义
                }
            }

            // 7. 错误处理和恢复
            // 生产级异常处理
            try
            {
                var assemblyInfo = await analysisService.AnalyzeAssemblyAsync(path);
            }
            catch (BadImageFormatException)
            {
                // 处理无效PE格式
            }
            catch (MetadataValidationException)
            {
                // 处理元数据验证错误
            }
            catch (IOException)
            {
                // 处理文件访问错误
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // 处理用户取消
            }

            // 11. 推荐生产环境实践
            // 安全文件访问
            // 验证文件存在性
            if (!File.Exists(assemblyPath))
                throw new FileNotFoundException();

            // 验证文件可读性
            var fileInfo = new FileInfo(assemblyPath);
            if (fileInfo.Length > MaxFileSize)
                throw new InvalidOperationException("File too large");

            // 使用安全的文件流访问
            using var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);

            // 内存效率处理
            // 限制读取元素数量避免内存溢出
            if (metadataReader.TypeDefinitions.Count() > MaxTypeCount)
            {
                _logger.LogWarning("Assembly has excessive type definitions - limiting read");
                // 启用流式读取或其他优化策略
            }

            // 缓存和重用策
            // 对于频繁访问的程序集启用缓存
            private readonly MemoryCache<string, AssemblyInfo> _analysisCache = new MemoryCache<string, AssemblyInfo>();

            // 查询缓存，避免重复分析
            var cachedResult = _analysisCache.TryGetValue(assemblyPath, out var cachedInfo);
            if (cachedResult) return cachedInfo;

            // 分析结果缓存
            _analysisCache.Set(assemblyPath, result, cacheDuration);
        }
    }

    // 4. 可配置的元数据读取选项
    // 灵活的分析配置
    public class MetadataAnalysisOptions
    {
        public bool ReadMethods { get; set; } = true;    // 动态开关详细度
        public bool ReadProperties { get; set; } = false; // 性能优化：跳过不必要的读取
        public bool ReadFields { get; set; } = false;
        public List<string> IncludeNamespaces { get; set; } = new List<string>();  // 限定范围读取
    }

    // 5. 类型安全的信息模型
    // 定义清晰的信息结构
    public class TypeInfo
    {
        public string Name { get; set; }
        public TypeAttributes Attributes { get; set; }  // 强类型属性
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();
        public List<string> Interfaces { get; set; } = new List<string>();
    }

    // 8. 比较和差异分析
    // 程序集差异对
    public class MetadataComparisonService
    {
        public async Task<AssemblyComparisonResult> CompareAssembliesAsync(...)
        {
            var assembly1 = await analysisService.AnalyzeAssemblyAsync(path1);
            var assembly2 = await analysisService.AnalyzeAssemblyAsync(path2);

            return new AssemblyComparisonResult
            {
                NewTypes = CompareLists(types1, types2),
                RemovedReferences = CompareLists(refs1, refs2)
            };
        }
    }

    // 9. 验证和完整性检查
    // 元数据完整性验证
    public class MetadataValidationService
    {
        public async Task<MetadataValidationResult> ValidateAssemblyMetadataAsync(string path)
        {
            // PE文件有效性检查
            // 元数据完整性验证
            // 引用一致性检查
            // 类型定义验证
        }
    }

    // 10. 监控和遥测
    // 性能指标跟踪
    public interface IMetadataTelemetryService
    {
        Task TrackAnalysisAsync(string assembly, TimeSpan duration, long metadataSize, bool success);
        Task TrackQueryAsync(string assembly, TimeSpan duration, int resultCount);
        MetadataPerformanceMetrics GetAllMetrics();
    }

    public class AssemblyMetadataMetrics
    {
        public double AverageAnalysisTime { get; set; }
        public long SuccessfulAnalyses { get; set; }
        public long FailedAnalyses { get; set; }
    }
}