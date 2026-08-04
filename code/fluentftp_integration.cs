#:sdk Microsoft.NET.Sdk
#:package FluentFTP@42.0.3
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable

using FluentFTP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class FtpConnectionPool : IDisposable
{
    private readonly ObjectPool<FtpClient> _pool;
    private readonly FtpConfig _config;

    public FtpConnectionPool(FtpConfig config)
    {
        _config = config;
        var policy = new FtpClientPooledObjectPolicy(_config);
        _pool = new DefaultObjectPool<FtpClient>(policy, maxRetained: 10);
    }

    public FtpClient Get() => _pool.Get();
    public void Return(FtpClient client) => _pool.Return(client);
    public void Dispose() => _pool?.Dispose();
}

public class FtpClientPooledObjectPolicy : IPooledObjectPolicy<FtpClient>
{
    private readonly FtpConfig _config;

    public FtpClientPooledObjectPolicy(FtpConfig config)
    {
        _config = config;
    }

    public FtpClient Create() => new FtpClient(_config.Host, _config.Username, _config.Password);

    public bool Return(FtpClient obj)
    {
        if (obj.IsConnected)
            return true;

        obj.Dispose();
        return false;
    }
}

public class FtpConfig
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; } = 21;
}

public interface IFtpService
{
    Task UploadFileAsync(string localPath, string remotePath, CancellationToken ct = default);
    Task DownloadFileAsync(string remotePath, string localPath, CancellationToken ct = default);
    Task UploadFileWithResumeAsync(string localPath, string remotePath, CancellationToken ct = default);
    
    // 目录管理
    Task CreateDirectoryAsync(string remotePath, CancellationToken ct = default);
    Task DeleteDirectoryAsync(string remotePath, CancellationToken ct = default);
    Task<bool> DirectoryExistsAsync(string remotePath, CancellationToken ct = default);
    Task<IEnumerable<string>> ListDirectoryAsync(string remotePath, CancellationToken ct = default);
    
    // 文件管理
    Task DeleteFileAsync(string remotePath, CancellationToken ct = default);
    Task<bool> FileExistsAsync(string remotePath, CancellationToken ct = default);
    Task<long> GetFileSizeAsync(string remotePath, CancellationToken ct = default);
    Task<DateTime> GetModifiedTimeAsync(string remotePath, CancellationToken ct = default);
    Task RenameFileAsync(string fromPath, string toPath, CancellationToken ct = default);
}

public class FtpService : IFtpService, IDisposable
{
    private readonly FtpConnectionPool _pool;
    private readonly ThreadLocal<Memory<byte>> _uploadBuffer;

    public FtpService(FtpConnectionPool pool)
    {
        _pool = pool;
        _uploadBuffer = new ThreadLocal<Memory<byte>>(() => new Memory<byte>(new byte[81920]));
    }

    public async Task UploadFileAsync(string localPath, string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            using var fileStream = File.OpenRead(localPath);
            await client.UploadAsync(fileStream, remotePath, token: ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task UploadFileWithResumeAsync(string localPath, string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            var fileInfo = new FileInfo(localPath);
            var remoteSize = await client.GetFileSizeAsync(remotePath, -1, ct);
            
            if (remoteSize == fileInfo.Length)
                return;
                
            using var fileStream = File.OpenRead(localPath);
            if (remoteSize > 0)
                fileStream.Seek(remoteSize, SeekOrigin.Begin);
                
            await client.UploadAsync(fileStream, remotePath, FtpRemoteExists.Resume, token: ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task DownloadFileAsync(string remotePath, string localPath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            await client.DownloadFileAsync(localPath, remotePath, token: ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task CreateDirectoryAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            await client.CreateDirectoryAsync(remotePath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task DeleteDirectoryAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            await client.DeleteDirectoryAsync(remotePath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task<bool> DirectoryExistsAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            return await client.DirectoryExistsAsync(remotePath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task<IEnumerable<string>> ListDirectoryAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            var items = await client.GetListingAsync(remotePath, ct);
            return items.Select(x => x.FullName);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task DeleteFileAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            await client.DeleteFileAsync(remotePath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task<bool> FileExistsAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            return await client.FileExistsAsync(remotePath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task<long> GetFileSizeAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            return await client.GetFileSizeAsync(remotePath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task<DateTime> GetModifiedTimeAsync(string remotePath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            return await client.GetModifiedTimeAsync(remotePath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }

    public async Task RenameFileAsync(string fromPath, string toPath, CancellationToken ct = default)
    {
        var client = _pool.Get();
        try
        {
            await client.ConnectAsync(ct);
            await client.RenameAsync(fromPath, toPath, ct);
        }
        finally
        {
            _pool.Return(client);
        }
    }
}

    public void Dispose()
    {
        _uploadBuffer?.Dispose();
        _pool?.Dispose();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentFTPServices(this IServiceCollection services, Action<FtpConfig> configure)
    {
        var config = new FtpConfig();
        configure(config);
        
        services.AddSingleton(config);
        services.AddSingleton<FtpConnectionPool>();
        services.AddSingleton<IFtpService, FtpService>();
        
        return services;
    }
}

// 在Startup.cs或Program.cs中配置
builder.Services.AddFluentFTPServices(options => 
{
    options.Host = "ftp.example.com";
    options.Username = "user";
    options.Password = "password";
});

// 在控制器或服务中注入使用
public class MyService
{
    private readonly IFtpService _ftpService;
    
    public MyService(IFtpService ftpService)
    {
        _ftpService = ftpService;
    }
    
    public async Task UploadFile(string path)
    {
        await _ftpService.UploadFileAsync(path, "/remote/path/file.txt");
    }
}