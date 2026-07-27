#:sdk Microsoft.NET.Sdk
#:package Tamir.SharpSSH@1.1.1.13
#:property TargetFramework net11.0
#:property Nullable enable

using System;
using Tamir.SharpSsh;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class RttyOptions {
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; } = 22;
    public int MaxConnections { get; set; } = 5;
}

public interface IRttyService {
    Task<string> ExecuteCommandAsync(string command);
    Task UploadFileAsync(string localPath, string remotePath);
    Task DownloadFileAsync(string remotePath, string localPath);
}

public class RttyService : IRttyService, IDisposable {
    private readonly Channel<SshExec> _connectionPool;
    private readonly ILogger<RttyService> _logger;
    private readonly RttyOptions _options;
    
    public RttyService(IOptions<RttyOptions> options, ILogger<RttyService> logger) {
        _options = options.Value;
        _logger = logger;
        
        _connectionPool = Channel.CreateBounded<SshExec>(_options.MaxConnections);
        
        // 初始化连接池
        for (int i = 0; i < _options.MaxConnections; i++) {
            var ssh = new SshExec(_options.Host, _options.Username, _options.Password);
            ssh.Connect(_options.Port);
            _connectionPool.Writer.TryWrite(ssh);
        }
    }
    
    public async Task<string> ExecuteCommandAsync(string command) {
        var ssh = await GetConnectionAsync();
        try {
            _logger.LogInformation("Executing command: {Command}", command);
            return ssh.RunCommand(command);
        } finally {
            await ReturnConnectionAsync(ssh);
        }
    }
    
    public async Task UploadFileAsync(string localPath, string remotePath) {
        using var scp = new Scp(_options.Host, _options.Username, _options.Password);
        scp.Connect(_options.Port);
        scp.Push(localPath, remotePath);
    }
    
    public async Task DownloadFileAsync(string remotePath, string localPath) {
        using var scp = new Scp(_options.Host, _options.Username, _options.Password);
        scp.Connect(_options.Port);
        scp.Get(remotePath, localPath);
    }
    
    private async Task<SshExec> GetConnectionAsync() {
        return await _connectionPool.Reader.ReadAsync();
    }
    
    private async Task ReturnConnectionAsync(SshExec ssh) {
        await _connectionPool.Writer.WriteAsync(ssh);
    }
    
    public void Dispose() {
        while (_connectionPool.Reader.TryRead(out var ssh)) {
            ssh.Close();
        }
    }
}

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddRttyService(this IServiceCollection services, Action<RttyOptions> configure) {
        services.Configure(configure);
        services.AddSingleton<IRttyService, RttyService>();
        return services;
    }
}