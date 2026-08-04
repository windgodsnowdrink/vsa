#:sdk Microsoft.NET.Sdk
#:package SharpZipLib@1.4.2
#:property LangVersion preview
#:property TargetFramework net10.0

using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 配置选项
public class ZipOptions
{
    public int BufferSize { get; set; } = 81920;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public int CompressionLevel { get; set; } = 9; // 默认最高压缩级别
}

// 解压缩服务接口
public interface IZipService
{
    Task CreateZipAsync(string zipPath, string sourcePath, CancellationToken ct = default);
    Task CreateZipFromDirectoryAsync(string zipPath, string directoryPath, CancellationToken ct = default);
    Task CreateZipParallelAsync(string zipPath, string[] sourceFiles, CancellationToken ct = default);
}

// 生产级解压缩服务实现
public class ZipService : IZipService
{
    public async Task CreateZipAsync(string zipPath, string sourcePath, CancellationToken ct = default)
    {
        using var fs = File.Create(zipPath);
        using var zipStream = new ZipOutputStream(fs)
        {
            Level = _options.CompressionLevel
        };
        
        var entry = new ZipEntry(Path.GetFileName(sourcePath))
        {
            DateTime = DateTime.Now
        };
        
        zipStream.PutNextEntry(entry);
        
        using var sourceStream = File.OpenRead(sourcePath);
        await sourceStream.CopyToAsync(zipStream, _options.BufferSize, ct);
        
        zipStream.CloseEntry();
    }
    
    public async Task CreateZipFromDirectoryAsync(string zipPath, string directoryPath, CancellationToken ct = default)
    {
        using var fs = File.Create(zipPath);
        using var zipStream = new ZipOutputStream(fs)
        {
            Level = _options.CompressionLevel
        };
        
        var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        
        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();
            
            var relativePath = Path.GetRelativePath(directoryPath, file);
            var entry = new ZipEntry(relativePath)
            {
                DateTime = File.GetLastWriteTime(file)
            };
            
            zipStream.PutNextEntry(entry);
            
            using var fileStream = File.OpenRead(file);
            await fileStream.CopyToAsync(zipStream, _options.BufferSize, ct);
            
            zipStream.CloseEntry();
        }
    }
    
    public async Task CreateZipParallelAsync(string zipPath, string[] sourceFiles, CancellationToken ct = default)
    {
        using var fs = File.Create(zipPath);
        using var zipStream = new ZipOutputStream(fs)
        {
            Level = _options.CompressionLevel
        };
        
        var channel = Channel.CreateBounded<(string filePath, byte[] buffer)>(new BoundedChannelOptions(100)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });
        
        // 生产者任务
        var producer = Task.Run(async () =>
        {
            var bufferPool = ArrayPool<byte>.Shared;
            
            foreach (var file in sourceFiles)
            {
                ct.ThrowIfCancellationRequested();
                
                var buffer = bufferPool.Rent(_options.BufferSize);
                await channel.Writer.WriteAsync((file, buffer), ct);
            }
            
            channel.Writer.Complete();
        }, ct);
        
        // 消费者任务
        var consumers = Enumerable.Range(0, _options.MaxDegreeOfParallelism)
            .Select(_ => Task.Run(async () =>
            {
                await foreach (var (filePath, buffer) in channel.Reader.ReadAllAsync(ct))
                {
                    try
                    {
                        var entry = new ZipEntry(Path.GetFileName(filePath))
                        {
                            DateTime = File.GetLastWriteTime(filePath)
                        };
                        
                        lock (zipStream)
                        {
                            zipStream.PutNextEntry(entry);
                        }
                        
                        using var fileStream = File.OpenRead(filePath);
                        int bytesRead;
                        while ((bytesRead = await fileStream.ReadAsync(buffer, ct)) > 0)
                        {
                            lock (zipStream)
                            {
                                await zipStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                            }
                        }
                        
                        lock (zipStream)
                        {
                            zipStream.CloseEntry();
                        }
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(buffer);
                    }
                }
            }, ct));
        
        await Task.WhenAll(consumers.Append(producer));
    }
}

// DI扩展方法
public static class ZipServiceCollectionExtensions
{
    public static IServiceCollection AddZipServices(this IServiceCollection services, Action<ZipOptions> configure = null)
    {
        services.AddOptions<ZipOptions>()
            .Configure(configure ?? (opt => { }))
            .ValidateDataAnnotations();
            
        services.AddSingleton<IZipService, ZipService>();
        return services;
    }
}

// 示例用法
public class ZipDemo
{
    private readonly IZipService _zipService;
    
    public ZipDemo(IZipService zipService)
    {
        _zipService = zipService;
    }
    
    public async Task RunAsync()
    {
        // 单文件压缩
        await _zipService.CreateZipAsync("single.zip", "file.txt");
        
        // 目录压缩
        await _zipService.CreateZipFromDirectoryAsync("folder.zip", "my_folder");
        
        // 多线程压缩
        var files = Directory.GetFiles("large_files", "*", SearchOption.AllDirectories);
        await _zipService.CreateZipParallelAsync("parallel.zip", files);
    }
}