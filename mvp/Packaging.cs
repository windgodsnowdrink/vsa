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
using System.IO.Packaging;
using System.Linq;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Xml;

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

    // 1. 文档包内容模型 - 包内容结构定义
    public class DocumentPackageContent
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DocumentPart> Parts { get; set; } = new List<DocumentPart>();
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class DocumentPart
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public List<PartRelationship> Relationships { get; set; } = new List<PartRelationship>();
    }

    public class PartRelationship
    {
        public string Id { get; set; }
        public string Target { get; set; }
        public string Type { get; set; }
        public TargetMode TargetMode { get; set; }
    }

    // 2. 包装服务异常定义
    public class PackagingException : Exception
    {
        public PackagingException(string message) : base(message) { }
        public PackagingException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class PackageValidationException : Exception
    {
        public PackageValidationException(string message) : base(message) { }
    }

    // 3. 包验证器接口 - 生产环境完整性验证
    public interface IPackageValidator
    {
        Task<ValidationResult> ValidatePackageAsync(string packagePath);
        Task<bool> ValidatePackageIntegrityAsync(Package package);
    }

    public class PackageValidator : IPackageValidator
    {
        private readonly ILogger<PackageValidator> _logger;
        private readonly IHashService _hashService;

        public PackageValidator(ILogger<PackageValidator> logger, IHashService hashService)
        {
            _logger = logger;
            _hashService = hashService;
        }

        /// <summary>
        /// 验证包文件完整性和结构
        /// </summary>
        public async Task<ValidationResult> ValidatePackageAsync(string packagePath)
        {
            _logger.LogInformation("Starting package validation for {PackagePath}", packagePath);

            var result = new ValidationResult
            {
                PackagePath = packagePath,
                ValidationTime = DateTime.UtcNow
            };

            try
            {
                // 检查文件存在性
                if (!File.Exists(packagePath))
                {
                    result.IsValid = false;
                    result.Errors.Add("Package file does not exist");
                    return result;
                }

                using var package = Package.Open(packagePath, FileMode.Open, FileAccess.Read);

                // 验证包完整性
                result.IsValid = await ValidatePackageIntegrityAsync(package);

                if (result.IsValid)
                {
                    result.Message = "Package validation completed successfully";
                    _logger.LogInformation("Package {PackagePath} validation passed", packagePath);
                }
                else
                {
                    result.Message = "Package validation failed";
                    _logger.LogWarning("Package {PackagePath} validation failed - {Errors}",
                        packagePath, string.Join(", ", result.Errors));
                }

                result.PackagePartsCount = package.GetParts().Count();
                result.PackageRelationshipsCount = package.GetRelationships().Count();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating package {PackagePath}", packagePath);
                result.IsValid = false;
                result.Errors.Add($"Validation error: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// 验证包的内部完整性
        /// </summary>
        public async Task<bool> ValidatePackageIntegrityAsync(Package package)
        {
            var errors = new List<string>();

            try
            {
                // 检查必需部件是否存在
                var requiredParts = new[]
                {
                "/core/document.xml",
                "/core/metadata.xml"
            };

                var existingParts = package.GetParts()
                    .Select(p => p.Uri.ToString())
                    .ToArray();

                foreach (var requiredPart in requiredParts)
                {
                    if (!existingParts.Contains(requiredPart, StringComparer.OrdinalIgnoreCase))
                    {
                        errors.Add($"Required part not found: {requiredPart}");
                    }
                }

                // 验证关系完整性
                ValidateRelationships(package, errors);

                // 验证内容校验和 (如果包包含校验和信息)
                await ValidateChecksums(package, errors);

                if (errors.Any())
                {
                    _logger.LogWarning("Package integrity validation errors: {Errors}",
                        string.Join(", ", errors));
                }

                return !errors.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during package integrity validation");
                errors.Add(ex.Message);
                return false;
            }
        }

        private void ValidateRelationships(Package package, List<string> errors)
        {
            foreach (var relationship in package.GetRelationships())
            {
                // 检查关系目标是否存在
                if (relationship.TargetMode == TargetMode.Internal)
                {
                    try
                    {
                        var targetUri = PackUriHelper.ResolvePartUri(relationship.SourceUri, relationship.TargetUri);
                        if (!package.PartExists(targetUri))
                        {
                            errors.Add($"Relationship target does not exist: {targetUri}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Invalid relationship: {ex.Message}");
                    }
                }
            }
        }

        private async Task ValidateChecksums(Package package, List<string> errors)
        {
            // 如果包中包含校验和信息，则验证每个部件的校验和
            var checksumPartUri = PackUriHelper.CreatePartUri(new Uri("/checksums.json", UriKind.Relative));

            if (package.PartExists(checksumPartUri))
            {
                var checksumPart = package.GetPart(checksumPartUri);
                using var stream = checksumPart.GetStream(FileMode.Open, FileAccess.Read);

                var checksums = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream);

                foreach (var partUri in checksums.Keys)
                {
                    if (package.PartExists(new Uri(partUri, UriKind.Relative)))
                    {
                        var part = package.GetPart(new Uri(partUri, UriKind.Relative));
                        using var partStream = part.GetStream(FileMode.Open, FileAccess.Read);
                        var actualChecksum = await _hashService.ComputeChecksumAsync(partStream);

                        if (!actualChecksum.Equals(checksums[partUri], StringComparison.OrdinalIgnoreCase))
                        {
                            errors.Add($"Checksum mismatch for part: {partUri}");
                        }
                    }
                }
            }
        }
    }

    public class ValidationResult
    {
        public string PackagePath { get; set; }
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Message { get; set; }
        public DateTime ValidationTime { get; set; }
        public int PackagePartsCount { get; set; }
        public int PackageRelationshipsCount { get; set; }
    }

    // 4. 哈希服务接口 - 内容完整性验证服务
    public interface IHashService
    {
        Task<string> ComputeChecksumAsync(Stream stream);
        string ComputeChecksum(byte[] data);
    }

    public class HashService : IHashService
    {
        private readonly ILogger<HashService> _logger;

        public async Task<string> ComputeChecksumAsync(Stream stream)
        {
            try
            {
                // 使用SHA256计算校验和
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var hashBytes = await sha256.ComputeHashAsync(stream);
                return Convert.ToBase64String(hashBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing checksum");
                throw new PackagingException("Failed to compute checksum", ex);
            }
        }

        public string ComputeChecksum(byte[] data)
        {
            try
            {
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var hashBytes = sha256.ComputeHash(data);
                return Convert.ToBase64String(hashBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing checksum");
                throw new PackagingException("Failed to compute checksum", ex);
            }
        }
    }

    // 5. 文档包管理器 - 核心生产级包管理服务
    public class DocumentPackageManager
    {
        private readonly ILogger<DocumentPackageManager> _logger;
        private readonly IPackageValidator _validator;
        private readonly IHashService _hashService;

        public DocumentPackageManager(
            ILogger<DocumentPackageManager> logger,
            IPackageValidator validator,
            IHashService hashService)
        {
            _logger = logger;
            _validator = validator;
            _hashService = hashService;
        }

        /// <summary>
        /// 创建包含多个部件的文档包
        /// </summary>
        public async Task<string> CreateDocumentPackageAsync(
            DocumentPackageContent content,
            string outputPath,
            bool includeChecksums = true)
        {
            _logger.LogInformation("Creating document package at {OutputPath} with {PartCount} parts",
                outputPath, content.Parts.Count);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                using var package = Package.Open(outputPath, FileMode.Create);

                // Add core parts
                await AddCorePartsAsync(package, content);

                // Add custom content parts
                await AddContentPartsAsync(package, content);

                // Add custom relationships
                AddPartRelationships(package, content);

                // Add checksums part for integrity verification
                if (includeChecksums)
                {
                    await AddChecksumsPartAsync(package);
                }

                package.Close();

                _logger.LogInformation("Document package created successfully at {OutputPath}", outputPath);

                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document package at {OutputPath}", outputPath);
                throw new PackagingException($"Failed to create document package: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 读取文档包内容
        /// </summary>
        public async Task<DocumentPackageContent> ReadDocumentPackageAsync(string packagePath)
        {
            _logger.LogInformation("Reading document package from {PackagePath}", packagePath);

            try
            {
                if (!File.Exists(packagePath))
                {
                    throw new FileNotFoundException($"Package file not found: {packagePath}");
                }

                using var package = Package.Open(packagePath, FileMode.Open, FileAccess.Read);

                var content = new DocumentPackageContent();

                // Read core parts
                await ReadCorePartsAsync(package, content);

                // Read custom content parts
                await ReadContentPartsAsync(package, content);

                // Read relationships
                ReadPartRelationships(package, content);

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading document package from {PackagePath}", packagePath);
                throw new PackagingException($"Failed to read document package: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 修改现有文档包
        /// </summary>
        public async Task<bool> ModifyDocumentPackageAsync(
            string packagePath,
            Func<DocumentPackageContent, DocumentPackageContent> modifier,
            bool validateBeforeModification = true)
        {
            _logger.LogInformation("Modifying document package at {PackagePath}", packagePath);

            try
            {
                // 验证包完整性
                if (validateBeforeModification)
                {
                    var validationResult = await _validator.ValidatePackageAsync(packagePath);
                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning("Package validation failed before modification: {Errors}",
                            string.Join(", ", validationResult.Errors));
                        throw new PackageValidationException(
                            $"Package validation failed: {string.Join(", ", validationResult.Errors)}");
                    }
                }

                // 读取现有包
                var existingContent = await ReadDocumentPackageAsync(packagePath);

                // 应用修改器
                var modifiedContent = modifier(existingContent);

                // 创建临时文件路径
                var tempPath = Path.GetTempFileName();

                try
                {
                    // 创建新包
                    await CreateDocumentPackageAsync(modifiedContent, tempPath);

                    // 验证新包
                    var newValidationResult = await _validator.ValidatePackageAsync(tempPath);
                    if (!newValidationResult.IsValid)
                    {
                        _logger.LogError("Validation failed for modified package: {Errors}",
                            string.Join(", ", newValidationResult.Errors));
                        return false;
                    }

                    // 替换原始文件
                    File.Delete(packagePath);
                    File.Move(tempPath, packagePath);

                    _logger.LogInformation("Document package modified successfully");
                    return true;
                }
                catch
                {
                    // 清理临时文件
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error modifying document package at {PackagePath}", packagePath);
                throw new PackagingException($"Failed to modify document package: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 解压文档包到目录
        /// </summary>
        public async Task<bool> ExtractPackageAsync(string packagePath, string extractDirectory)
        {
            _logger.LogInformation("Extracting package {PackagePath} to {ExtractDirectory}",
                packagePath, extractDirectory);

            try
            {
                Directory.CreateDirectory(extractDirectory);

                using var package = Package.Open(packagePath, FileMode.Open, FileAccess.Read);

                foreach (var part in package.GetParts())
                {
                    var partPath = Path.Combine(extractDirectory,
                        part.Uri.ToString().TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    var partDirectory = Path.GetDirectoryName(partPath);
                    if (!string.IsNullOrEmpty(partDirectory))
                    {
                        Directory.CreateDirectory(partDirectory);
                    }

                    using var partStream = part.GetStream(FileMode.Open, FileAccess.Read);
                    using var fileStream = File.Create(partPath);

                    await partStream.CopyToAsync(fileStream);

                    _logger.LogDebug("Extracted part: {PartPath}", partPath);
                }

                _logger.LogInformation("Package extraction completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting package {PackagePath} to {ExtractDirectory}",
                    packagePath, extractDirectory);
                return false;
            }
        }

        /// <summary>
        /// 从目录创建文档包
        /// </summary>
        public async Task<string> CreatePackageFromDirectoryAsync(
            string sourceDirectory,
            string outputPath)
        {
            _logger.LogInformation("Creating package from directory {SourceDirectory} to {OutputPath}",
                sourceDirectory, outputPath);

            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");
                }

                var packageContent = new DocumentPackageContent();

                var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var relativePath = file.Substring(sourceDirectory.Length)
                        .TrimStart(Path.DirectorySeparatorChar)
                        .Replace(Path.DirectorySeparatorChar, '/');

                    using var fileStream = File.OpenRead(file);
                    using var memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);

                    var part = new DocumentPart
                    {
                        Name = relativePath,
                        ContentType = GetContentTypeFromFileExtension(file),
                        Content = memoryStream.ToArray()
                    };

                    packageContent.Parts.Add(part);
                }

                await CreateDocumentPackageAsync(packageContent, outputPath);

                _logger.LogInformation("Package creation from directory completed successfully");

                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package from directory {SourceDirectory}",
                    sourceDirectory);
                throw new PackagingException($"Failed to create package from directory: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取包的详细信息
        /// </summary>
        public async Task<PackageInfo> GetPackageInfoAsync(string packagePath)
        {
            _logger.LogDebug("Getting package info for {PackagePath}", packagePath);

            try
            {
                using var package = Package.Open(packagePath, FileMode.Open, FileAccess.Read);

                var packageInfo = new PackageInfo
                {
                    Path = packagePath,
                    Size = new FileInfo(packagePath).Length,
                    Parts = package.GetParts().Select(p => new PackagePartInfo
                    {
                        Uri = p.Uri.ToString(),
                        ContentType = p.ContentType,
                        Size = p.Size
                    }).ToList(),
                    Relationships = package.GetRelationships().Select(r => new PackageRelationshipInfo
                    {
                        Id = r.Id,
                        Type = r.RelationshipType,
                        Target = r.TargetUri.ToString(),
                        TargetMode = r.TargetMode
                    }).ToList()
                };

                return packageInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package info for {PackagePath}", packagePath);
                throw new PackagingException($"Failed to get package info: {ex.Message}", ex);
            }
        }

        #region 辅助方法

        private async Task AddCorePartsAsync(Package package, DocumentPackageContent content)
        {
            // Add document metadata part
            var metadataUri = PackUriHelper.CreatePartUri(new Uri("/core/metadata.xml", UriKind.Relative));
            var metadataPart = package.CreatePart(metadataUri, "application/xml");

            var metadataContent = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<documentMetadata>
    <title>{XmlEncode(content.Title)}</title>
    <author>{XmlEncode(content.Author)}</author>
    <createdAt>{content.CreatedAt:yyyy-MM-ddTHH:mm:ssZ}</createdAt>
    <tags>{string.Join(",", content.Tags ?? new List<string>())}</tags>
</documentMetadata>";

            using var metadataStream = metadataPart.GetStream(FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(metadataStream, Encoding.UTF8);
            await writer.WriteAsync(metadataContent);

            // Add document properties part
            var propertiesUri = PackUriHelper.CreatePartUri(new Uri("/core/properties.json", UriKind.Relative));
            var propertiesPart = package.CreatePart(propertiesUri, "application/json");

            var properties = new Dictionary<string, object>
            {
                ["version"] = "1.0",
                ["created"] = content.CreatedAt,
                ["author"] = content.Author,
                ["customMetadata"] = content.Metadata
            };

            using var propsStream = propertiesPart.GetStream(FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(propsStream, properties, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            _logger.LogDebug("Core parts added to package");
        }

        private async Task AddContentPartsAsync(Package package, DocumentPackageContent content)
        {
            foreach (var part in content.Parts)
            {
                var partUri = PackUriHelper.CreatePartUri(new Uri($"/content/{part.Name}", UriKind.Relative));

                if (package.PartExists(partUri))
                {
                    package.DeletePart(partUri);
                }

                var contentType = string.IsNullOrEmpty(part.ContentType) ?
                    "application/octet-stream" : part.ContentType;

                var packagePart = package.CreatePart(partUri, contentType);

                if (part.Content != null && part.Content.Length > 0)
                {
                    using var partStream = packagePart.GetStream(FileMode.Create, FileAccess.Write);
                    await partStream.WriteAsync(part.Content, 0, part.Content.Length);
                }

                _logger.LogDebug("Content part '{PartName}' added to package", part.Name);
            }
        }

        private void AddPartRelationships(Package package, DocumentPackageContent content)
        {
            foreach (var part in content.Parts)
            {
                var partUri = PackUriHelper.CreatePartUri(new Uri($"/content/{part.Name}", UriKind.Relative));
                var packagePart = package.GetPart(partUri);

                // Add custom relationships
                foreach (var relationship in part.Relationships)
                {
                    packagePart.CreateRelationship(
                        new Uri(relationship.Target, UriKind.Relative),
                        relationship.TargetMode,
                        relationship.Type,
                        relationship.Id);
                }

                // Add thumbnail relationship if present
                if (part.Properties.ContainsKey("thumbnail"))
                {
                    var thumbnailUri = PackUriHelper.CreatePartUri(
                        new Uri($"/thumbnails/{part.Properties["thumbnail"]}", UriKind.Relative));
                    packagePart.CreateRelationship(thumbnailUri, TargetMode.Internal,
                        "http://schemas.openxmlformats.org/officeDocument/2006/relationships/thumbnail");
                }
            }
        }

        private async Task AddChecksumsPartAsync(Package package)
        {
            var checksums = new Dictionary<string, string>();

            foreach (var part in package.GetParts())
            {
                if (part.Uri.ToString() != "/checksums.json") // 避免循环依赖
                {
                    using var stream = part.GetStream(FileMode.Open, FileAccess.Read);
                    var checksum = await _hashService.ComputeChecksumAsync(stream);
                    checksums[part.Uri.ToString()] = checksum;
                }
            }

            var checksumUri = PackUriHelper.CreatePartUri(new Uri("/checksums.json", UriKind.Relative));
            var checksumPart = package.CreatePart(checksumUri, "application/json");

            using var checksumStream = checksumPart.GetStream(FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(checksumStream, checksums, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            _logger.LogDebug("Checksums part added to package with {ChecksumCount} entries", checksums.Count);
        }

        private async Task ReadCorePartsAsync(Package package, DocumentPackageContent content)
        {
            // Read metadata
            var metadataUri = PackUriHelper.CreatePartUri(new Uri("/core/metadata.xml", UriKind.Relative));
            if (package.PartExists(metadataUri))
            {
                var metadataPart = package.GetPart(metadataUri);
                using var metadataStream = metadataPart.GetStream(FileMode.Open, FileAccess.Read);
                using var reader = XmlReader.Create(metadataStream);

                while (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "title":
                                content.Title = await reader.ReadElementContentAsStringAsync();
                                break;
                            case "author":
                                content.Author = await reader.ReadElementContentAsStringAsync();
                                break;
                            case "createdAt":
                                content.CreatedAt = DateTime.Parse(await reader.ReadElementContentAsStringAsync());
                                break;
                            case "tags":
                                var tagsString = await reader.ReadElementContentAsStringAsync();
                                if (!string.IsNullOrEmpty(tagsString))
                                {
                                    content.Tags = tagsString.Split(',').ToList();
                                }
                                break;
                        }
                    }
                }
            }

            _logger.LogDebug("Core parts read from package");
        }

        private async Task ReadContentPartsAsync(Package package, DocumentPackageContent content)
        {
            foreach (var part in package.GetParts())
            {
                if (part.Uri.ToString().StartsWith("/content/"))
                {
                    var documentPart = new DocumentPart
                    {
                        Name = part.Uri.ToString().Substring("/content/".Length),
                        ContentType = part.ContentType
                    };

                    using var partStream = part.GetStream(FileMode.Open, FileAccess.Read);
                    using var memoryStream = new MemoryStream();
                    await partStream.CopyToAsync(memoryStream);
                    documentPart.Content = memoryStream.ToArray();

                    content.Parts.Add(documentPart);
                }
            }

            _logger.LogDebug("Content parts read from package");
        }

        private void ReadPartRelationships(Package package, DocumentPackageContent content)
        {
            foreach (var part in package.GetParts())
            {
                if (part.Uri.ToString().StartsWith("/content/"))
                {
                    var partName = part.Uri.ToString().Substring("/content/".Length);
                    var documentPart = content.Parts.FirstOrDefault(p => p.Name == partName);

                    if (documentPart != null)
                    {
                        foreach (var relationship in part.GetRelationships())
                        {
                            documentPart.Relationships.Add(new PartRelationship
                            {
                                Id = relationship.Id,
                                Target = relationship.TargetUri.ToString(),
                                Type = relationship.RelationshipType,
                                TargetMode = relationship.TargetMode
                            });
                        }
                    }
                }
            }
        }

        private string XmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return System.Security.SecurityElement.Escape(text) ?? text;
        }

        private string GetContentTypeFromFileExtension(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".txt" => "text/plain",
                ".xml" => "application/xml",
                ".json" => "application/json",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        #endregion
    }

    // 6. 包信息模型
    public class PackageInfo
    {
        public string Path { get; set; }
        public long Size { get; set; }
        public List<PackagePartInfo> Parts { get; set; } = new List<PackagePartInfo>();
        public List<PackageRelationshipInfo> Relationships { get; set; } = new List<PackageRelationshipInfo>();
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    }

    public class PackagePartInfo
    {
        public string Uri { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
    }

    public class PackageRelationshipInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Target { get; set; }
        public TargetMode TargetMode { get; set; }
    }

    // 7. 扩展关系类型定义
    public static class RelationshipTypes
    {
        public const string Document = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
        public const string Thumbnail = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/thumbnail";
        public const string Image = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
        public const string Styles = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
        public const string CustomXml = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/customXml";
        public const string AlternateContent = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/aF";
    }

    // 8. 安全包管理服务 - 生产环境安全处理
    public class SecureDocumentPackageService
    {
        private readonly DocumentPackageManager _packageManager;
        private readonly IHashService _hashService;
        private readonly ILogger<SecureDocumentPackageService> _logger;
        private readonly string _tempDirectory;

        public SecureDocumentPackageService(
            DocumentPackageManager packageManager,
            IHashService hashService,
            ILogger<SecureDocumentPackageService> logger)
        {
            _packageManager = packageManager;
            _hashService = hashService;
            _logger = logger;
            _tempDirectory = Path.Combine(Path.GetTempPath(), "SecurePackages");
            Directory.CreateDirectory(_tempDirectory);
        }

        /// <summary>
        /// 安全创建包 - 在临时位置创建，验证后再移动到最终位置
        /// </summary>
        public async Task<string> CreateSecurePackageAsync(
            DocumentPackageContent content,
            string finalOutputPath)
        {
            _logger.LogInformation("Creating secure document package at {OutputPath}", finalOutputPath);

            var tempPath = Path.Combine(_tempDirectory, Guid.NewGuid().ToString() + ".tmp");

            try
            {
                // 在临时位置创建包
                await _packageManager.CreateDocumentPackageAsync(content, tempPath);

                // 验证创建的包
                var validationResult = await _packageManager.GetPackageInfoAsync(tempPath);
                if (validationResult == null)
                {
                    throw new PackagingException("Failed to validate created package");
                }

                // 确保目标目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(finalOutputPath));

                // 安全移动到最终位置
                if (File.Exists(finalOutputPath))
                {
                    File.Delete(finalOutputPath);
                }
                File.Move(tempPath, finalOutputPath);

                _logger.LogInformation("Secure package created at {OutputPath}", finalOutputPath);

                return finalOutputPath;
            }
            catch (Exception ex)
            {
                // 清理临时文件
                try
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Failed to cleanup temporary file {TempPath}", tempPath);
                }

                _logger.LogError(ex, "Error creating secure package at {OutputPath}", finalOutputPath);
                throw new PackagingException($"Failed to create secure package: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 安全读取包 - 先验证完整性再读取
        /// </summary>
        public async Task<DocumentPackageContent> ReadSecurePackageAsync(string packagePath)
        {
            _logger.LogInformation("Reading secure document package from {PackagePath}", packagePath);

            try
            {
                // 验证包完整性
                var validationResult = await ValidatePackageIntegrityAsync(packagePath);
                if (!validationResult.IsValid)
                {
                    throw new PackageValidationException(
                        $"Package integrity validation failed: {string.Join(", ", validationResult.Errors)}");
                }

                // 读取包内容
                var content = await _packageManager.ReadDocumentPackageAsync(packagePath);

                _logger.LogInformation("Secure package read completed successfully");

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading secure package from {PackagePath}", packagePath);
                throw new PackagingException($"Failed to read secure package: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 验证包完整性安全方法
        /// </summary>
        public async Task<ValidationResult> ValidatePackageIntegrityAsync(string packagePath)
        {
            _logger.LogInformation("Validating package integrity for {PackagePath}", packagePath);

            return await ValidatePackageAsync(packagePath);
        }

        /// <summary>
        /// 包验证方法
        /// </summary>
        public async Task<ValidationResult> ValidatePackageAsync(string packagePath)
        {
            return await new PackageValidator(_logger, _hashService).ValidatePackageAsync(packagePath);
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(_tempDirectory))
                {
                    Directory.Delete(_tempDirectory, true);
                    Directory.CreateDirectory(_tempDirectory);
                    _logger.LogInformation("Secure package temp directory cleaned up");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cleanup secure package temp directory");
            }
        }
    }

    // 9. 包版本管理服务 - 生产环境版本控制
    public class PackageVersioningService
    {
        private readonly ILogger<PackageVersioningService> _logger;

        /// <summary>
        /// 创建不同版本的包处理
        /// </summary>
        public async Task<string> CreatePackageWithVersionAsync(
            DocumentPackageContent content,
            string outputPath,
            string version)
        {
            _logger.LogInformation("Creating versioned package at {OutputPath} with version {Version}",
                outputPath, version);

            var fileName = Path.GetFileNameWithoutExtension(outputPath);
            var fileExtension = Path.GetExtension(outputPath);
            var directory = Path.GetDirectoryName(outputPath);

            var versionedPath = Path.Combine(directory, $"{fileName}_v{version}{fileExtension}");

            using var package = Package.Open(versionedPath, FileMode.Create);

            // 添加版本信息到核心部件
            await AddVersionToCoreMetadata(package, version);

            // 复制原始包内容...

            return versionedPath;
        }

        private async Task AddVersionToCoreMetadata(Package package, string version)
        {
            var metadataUri = PackUriHelper.CreatePartUri(new Uri("/core/version.xml", UriKind.Relative));
            var metadataPart = package.CreatePart(metadataUri, "application/xml");

            var versionContent = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<packageVersion>
    <version>{version}</version>
    <createdAt>{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}</createdAt>
</packageVersion>";

            using var versionStream = metadataPart.GetStream(FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(versionStream, Encoding.UTF8);
            await writer.WriteAsync(versionContent);
        }
    }

    // 10. 包压缩服务接口
    public interface IPackageCompressionService
    {
        Task<string> CompressPackageAsync(string sourcePackagePath, string compressedPath);
        Task<string> DecompressPackageAsync(string compressedPackagePath, string outputPath);
    }

    // 11. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.IO.Packaging Production Demo");
            Console.WriteLine("===================================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册核心服务
            builder.Services.AddSingleton<IHashService, HashService>();
            builder.Services.AddSingleton<IPackageValidator, PackageValidator>();
            builder.Services.AddSingleton<DocumentPackageManager>();
            builder.Services.AddSingleton<SecureDocumentPackageService>();
            builder.Services.AddSingleton<PackageVersioningService>();

            #endregion

            var host = builder.Build();
            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var packageManager = services.GetRequiredService<DocumentPackageManager>();
            var secureService = services.GetRequiredService<SecureDocumentPackageService>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            Console.WriteLine("1. Creating Document Package:");

            var content = new DocumentPackageContent
            {
                Title = "Sample Document Package",
                Author = "John Developer",
                CreatedAt = DateTime.UtcNow,
                Tags = new List<string> { "sample", "test", "demo" },
                Metadata = new Dictionary<string, string>
            {
                { "description", "This is a sample document package for demonstration" },
                { "category", "Documentation" }
            },
                Parts = new List<DocumentPart>
            {
                new DocumentPart
                {
                    Name = "content.txt",
                    ContentType = "text/plain",
                    Content = Encoding.UTF8.GetBytes("This is the main document content."),
                    Properties = new Dictionary<string, string> { { "language", "en" } }
                },
                new DocumentPart
                {
                    Name = "data.json",
                    ContentType = "application/json",
                    Content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { name = "test", value = 42 }))
                },
                new DocumentPart
                {
                    Name = "image.png",
                    ContentType = "image/png",
                    Content = GenerateSampleImageData() // 模拟图片数据
                }
            }
            };

            var outputPath = Path.Combine(Path.GetTempPath(), "sample_package.zip");
            try
            {
                var createdPackagePath = await packageManager.CreateDocumentPackageAsync(content, outputPath);
                Console.WriteLine($"   Package created successfully at: {createdPackagePath}");
                Console.WriteLine($"   Package size: {new FileInfo(createdPackagePath).Length} bytes");
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error creating package: {ex.Message}");
            }

            Console.WriteLine("\n2. Reading Document Package:");
            try
            {
                var readContent = await packageManager.ReadDocumentPackageAsync(outputPath);
                Console.WriteLine($"   Title: {readContent.Title}");
                Console.WriteLine($"   Author: {readContent.Author}");
                Console.WriteLine($"   Parts count: {readContent.Parts.Count}");

                foreach (var part in readContent.Parts.Take(3))
                {
                    Console.WriteLine($"   - {part.Name} ({part.ContentType}) - {part.Content?.Length ?? 0} bytes");
                }
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error reading package: {ex.Message}");
            }

            Console.WriteLine("\n3. Modifying Document Package:");
            try
            {
                var wasModified = await packageManager.ModifyDocumentPackageAsync(
                    outputPath,
                    existingContent =>
                    {
                        existingContent.Title = "Modified Sample Document Package";
                        existingContent.Metadata["lastModified"] = DateTime.UtcNow.ToString("O");
                        existingContent.Parts.Add(new DocumentPart
                        {
                            Name = "additional.txt",
                            ContentType = "text/plain",
                            Content = Encoding.UTF8.GetBytes("This is additional content added during modification.")
                        });
                        return existingContent;
                    });

                Console.WriteLine($"   Package modified: {wasModified}");
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error modifying package: {ex.Message}");
            }

            Console.WriteLine("\n4. Validating Document Package:");
            try
            {
                var validator = services.GetRequiredService<IPackageValidator>();
                var validationResult = await validator.ValidatePackageAsync(outputPath);

                Console.WriteLine($"   Validation result: {(validationResult.IsValid ? "PASSED" : "FAILED")}");
                Console.WriteLine($"   Parts count: {validationResult.PackagePartsCount}");
                Console.WriteLine($"   Relationships count: {validationResult.PackageRelationshipsCount}");

                if (!validationResult.IsValid)
                {
                    Console.WriteLine($"   Validation errors: {string.Join(", ", validationResult.Errors)}");
                }
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error validating package: {ex.Message}");
            }

            Console.WriteLine("\n5. Extracting Document Package:");
            var extractDir = Path.Combine(Path.GetTempPath(), "extracted_package");
            try
            {
                var wasExtracted = await packageManager.ExtractPackageAsync(outputPath, extractDir);
                Console.WriteLine($"   Package extracted to: {extractDir}");
                Console.WriteLine($"   Extraction success: {wasExtracted}");

                if (wasExtracted && Directory.Exists(extractDir))
                {
                    var extractedFiles = Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories);
                    Console.WriteLine($"   Extracted {extractedFiles.Length} files");
                    foreach (var file in extractedFiles.Take(3))
                    {
                        var relativePath = file.Substring(extractDir.Length);
                        Console.WriteLine($"   - {relativePath}");
                    }
                }
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error extracting package: {ex.Message}");
            }

            Console.WriteLine("\n6. Creating Package from Directory:");
            var directoryPath = Path.Combine(Path.GetTempPath(), "source_directory");
            var packageFromDirPath = Path.Combine(Path.GetTempPath(), "package_from_directory.zip");

            try
            {
                // 创建示例目录结构
                Directory.CreateDirectory(directoryPath);
                Directory.CreateDirectory(Path.Combine(directoryPath, "subfolder"));

                File.WriteAllText(Path.Combine(directoryPath, "readme.txt"),
                    "This is a sample file for packaging demo.");
                File.WriteAllText(Path.Combine(directoryPath, "subfolder", "data.json"),
                    JsonSerializer.Serialize(new { content = "nested file" }));

                var createdPath = await packageManager.CreatePackageFromDirectoryAsync(
                    directoryPath, packageFromDirPath);
                Console.WriteLine($"   Package created from directory: {createdPath}");
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error creating package from directory: {ex.Message}");
            }
            finally
            {
                // 清理示例目录
                try
                {
                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine($"   Error cleaning up directory: {cleanupEx.Message}");
                }
            }

            Console.WriteLine("\n7. Getting Package Information:");
            try
            {
                var packageInfo = await packageManager.GetPackageInfoAsync(outputPath);
                Console.WriteLine($"   Package path: {packageInfo.Path}");
                Console.WriteLine($"   Package size: {packageInfo.Size} bytes");
                Console.WriteLine($"   Parts in package: {packageInfo.Parts.Count}");
                Console.WriteLine($"   Relationships in package: {packageInfo.Relationships.Count}");
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error getting package info: {ex.Message}");
            }

            Console.WriteLine("\n8. Secure Package Operations:");
            var securityPath = Path.Combine(Path.GetTempPath(), "secure_sample.zip");
            try
            {
                var securePath = await secureService.CreateSecurePackageAsync(content, securityPath);
                Console.WriteLine($"   Secure package created: {securePath}");

                var secureContent = await secureService.ReadSecurePackageAsync(securePath);
                Console.WriteLine($"   Secure package read successfully - Parts: {secureContent.Parts.Count}");
            }
            catch (PackagingException ex)
            {
                Console.WriteLine($"   Error in secure package operation: {ex.Message}");
            }

            // 清理演示文件
            CleanupDemoFiles(outputPath, packageFromDirPath, extractDir, securityPath, logger);

            Console.WriteLine("\n=== Demo Complete ===");
        }

        private static byte[] GenerateSampleImageData()
        {
            // 生成简单的PNG格式示例数据
            return new byte[]
            {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, // IHDR chunk header
            0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, // Width and height (16x16)
            0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0xF3, 0xFF, // Bit depth, color type, etc.
            0x61, 0x00, 0x00, 0x00, 0x0C, 0x49, 0x44, 0x41, // IDAT chunk with sample data
            0x54, 0x78, 0xDA, 0x63, 0xFC, 0xCF, 0x80, 0x00,
            0x00, 0x00, 0xFF, 0xFF, 0x03, 0x00, 0x6C, 0xFF,
            0x38, 0x80, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45,
            0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82  // IEND chunk
            };
        }

        private static void CleanupDemoFiles(
            string outputPath,
            string packageFromDirPath,
            string extractDir,
            string securityPath,
            ILogger logger)
        {
            try
            {
                var filesToRemove = new[] { outputPath, packageFromDirPath, securityPath };
                foreach (var file in filesToRemove)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        logger.LogInformation("Cleaned up demo file: {FilePath}", file);
                    }
                }

                if (Directory.Exists(extractDir))
                {
                    Directory.Delete(extractDir, true);
                    logger.LogInformation("Cleaned up demo directory: {DirectoryPath}", extractDir);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error cleaning up demo files");
            }
        }

        public void Test()
        {
            // 包核心操作
            // 创建新的包
            using var package = Package.Open(outputPath, FileMode.Create);

            // 打开现有包
            using var package = Package.Open(packagePath, FileMode.Open, FileAccess.Read);

            // 部件管理操作
            // 创建部件
            var partUri = PackUriHelper.CreatePartUri(new Uri("/content/document.xml", UriKind.Relative));
            var part = package.CreatePart(partUri, "application/xml");

            // 部件关系创建
            part.CreateRelationship(
                targetUri,           // 目标URI
                TargetMode.Internal, // 内部或外部目标
                relationshipType,   // 关系类型
                relationshipId);    // 关系ID

            // 内容流读写
            // 写入部件内容
            using var partStream = part.GetStream(FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(partStream, Encoding.UTF8);
            await writer.WriteAsync(content);

            // 读取部件内容
            using var partStream = part.GetStream(FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(partStream, Encoding.UTF8);
            var content = await reader.ReadToEndAsync();
        }

        // 生产级应用模式
        // 安全包处理
        public class SecureDocumentPackageService
        {
            // 在临时位置操作包文件，验证后再移动到最终位置
            // 避免损坏原始文件或创建不完整的包
        }

        // 包完整性验
        public interface IPackageValidator
        {
            Task<ValidationResult> ValidatePackageAsync(string packagePath);
            Task<bool> ValidatePackageIntegrityAsync(Package package);
        }

        // 版本管理
        public class PackageVersioningService
        {
            // 创建带版本信息的包文件
            // 管理包的不同版本

            public void Test()
            {
                // 包信息获取
                foreach (var part in package.GetParts())
                {
                    Console.WriteLine($"Part: {part.Uri}, Type: {part.ContentType}");
                }

                // 关系枚举
                foreach (var relationship in package.GetRelationships())
                {
                    Console.WriteLine($"Relationship: {relationship.Id} -> {relationship.TargetUri}");
                }

                // 文件解压/压缩操作
                // 目录到包
                public async Task<string> CreatePackageFromDirectoryAsync(
                    string sourceDirectory,
                    string outputPath) { };

            // 包到目录
            public async Task<bool> ExtractPackageAsync(
                string packagePath,
                string extractDirectory){ };
        }
    }
}

    // 生产环境最佳实践
    //异常处理
    public class PackagingException : Exception
    {
        public PackagingException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    // 日志和遥测
    private readonly ILogger<DocumentPackageManager> _logger;
    _logger.LogInformation("Package operation completed - Parts: {Count}", partCount);
    _logger.LogError(ex, "Error during package creation");
}

/**
Package Structure:
├── / core /
│   ├── metadata.xml       # 文档元数据
│   ├── properties.json    # 附加属性
│   └── version.xml        # 版本信息
├── / content /
│   ├── document.xml       # 主文档内容
│   ├── styles.xml         # 样式定义
│   └── [其他内容文件]
├── / thumbnails /
│   └── preview.png        # 预览缩略图
├── / resources /
│   ├── images /
│   └── data /
└── / checksums.json        # 完整性校验和
**/

// 安全性考虑：
// 在临时目录操作包文件
// 验证文件路径避免目录遍历攻击
// 实施完整性校验
// 清理临时文件和资源