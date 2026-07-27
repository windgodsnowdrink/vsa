#:sdk Microsoft.NET.Sdk
#:package SSH.NET@2023.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Text.Json@10.0.0
#:package System.Threading.Channels@8.0.0
#:package System.IO.Pipelines@8.0.0
#:property TargetFramework=net11.0
#:property LangVersion=preview
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace SSH.Client
{
    public class SshConnection : IDisposable
    {
        public SshClient Client { get; }
        public string Host { get; }
        public int Port { get; }
        public string Username { get; }

        public SshConnection(SshClient client, string host, int port, string username)
        {
            Client = client;
            Host = host;
            Port = port;
            Username = username;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }

    public class SshSession : IDisposable
    {
        public SshConnection Connection { get; }
        public DateTime CreatedAt { get; }
        public DateTime LastUsedAt { get; set; }

        public SshSession(SshConnection connection)
        {
            Connection = connection;
            CreatedAt = DateTime.UtcNow;
            LastUsedAt = DateTime.UtcNow;
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }

    public class SshConfig
    {
        public List<SshConfigEntry> Entries { get; set; } = new();
    }

    public class SshConfigEntry
    {
        public string Host { get; set; }
        public int Port { get; set; } = 22;
        public string Username { get; set; }
        public string Password { get; set; }
        public string PrivateKeyPath { get; set; }
        public string Passphrase { get; set; }
        public string Alias { get; set; }
    }

    public interface ISshService
    {
        Task<SshConnection> ConnectAsync(string host, int port, string username, string password);
        Task<SshConnection> ConnectAsync(string host, int port, string username, string privateKeyPath, string passphrase = null);
        Task<string> ExecuteCommandAsync(SshConnection connection, string command, int timeout = 30000);
        Task UploadFileAsync(SshConnection connection, string localPath, string remotePath);
        Task DownloadFileAsync(SshConnection connection, string remotePath, string localPath);
        Task CloseConnectionAsync(SshConnection connection);
    }

    public class SshService : ISshService
    {
        private readonly ILogger<SshService> _logger;

        public SshService(ILogger<SshService> logger)
        {
            _logger = logger;
        }

        public async Task<SshConnection> ConnectAsync(string host, int port, string username, string password)
        {
            _logger.LogInformation("Connecting to {Host}:{Port} as {Username}", host, port, username);

            var client = new SshClient(host, port, username, password);
            await Task.Run(() => client.Connect());

            if (!client.IsConnected)
            {
                throw new Exception("Failed to connect to SSH server");
            }

            _logger.LogInformation("Connected successfully to {Host}:{Port}", host, port);
            return new SshConnection(client, host, port, username);
        }

        public async Task<SshConnection> ConnectAsync(string host, int port, string username, string privateKeyPath, string passphrase = null)
        {
            _logger.LogInformation("Connecting to {Host}:{Port} as {Username} using private key", host, port, username);

            var keyFiles = new List<PrivateKeyFile>();
            if (!string.IsNullOrEmpty(privateKeyPath))
            {
                if (!File.Exists(privateKeyPath))
                {
                    throw new FileNotFoundException("Private key file not found", privateKeyPath);
                }

                if (!string.IsNullOrEmpty(passphrase))
                {
                    keyFiles.Add(new PrivateKeyFile(privateKeyPath, passphrase));
                }
                else
                {
                    keyFiles.Add(new PrivateKeyFile(privateKeyPath));
                }
            }

            var client = new SshClient(host, port, username, keyFiles.ToArray());
            await Task.Run(() => client.Connect());

            if (!client.IsConnected)
            {
                throw new Exception("Failed to connect to SSH server");
            }

            _logger.LogInformation("Connected successfully to {Host}:{Port} using private key", host, port);
            return new SshConnection(client, host, port, username);
        }

        public async Task<string> ExecuteCommandAsync(SshConnection connection, string command, int timeout = 30000)
        {
            _logger.LogInformation("Executing command on {Host}: {Command}", connection.Host, command);

            var cmd = connection.Client.CreateCommand(command);
            cmd.CommandTimeout = TimeSpan.FromMilliseconds(timeout);

            var result = await Task.Run(() => cmd.Execute());
            var exitStatus = cmd.ExitStatus;

            if (exitStatus != 0)
            {
                _logger.LogWarning("Command executed with non-zero exit status {ExitStatus}: {Command}", exitStatus, command);
            }

            _logger.LogInformation("Command executed successfully on {Host}", connection.Host);
            return result;
        }

        public async Task UploadFileAsync(SshConnection connection, string localPath, string remotePath)
        {
            _logger.LogInformation("Uploading file from {LocalPath} to {Host}:{RemotePath}", localPath, connection.Host, remotePath);

            if (!File.Exists(localPath))
            {
                throw new FileNotFoundException("Local file not found", localPath);
            }

            using var sftp = new SftpClient(connection.Client.ConnectionInfo);
            await Task.Run(() => sftp.Connect());

            try
            {
                // 确保远程目录存在
                var remoteDir = Path.GetDirectoryName(remotePath);
                if (!string.IsNullOrEmpty(remoteDir))
                {
                    await Task.Run(() =>
                    {
                        var dirs = remoteDir.Split('/');
                        var currentPath = string.Empty;

                        foreach (var dir in dirs)
                        {
                            if (string.IsNullOrEmpty(dir)) continue;

                            currentPath += "/" + dir;
                            try
                            {
                                sftp.CreateDirectory(currentPath);
                            }
                            catch (SftpPathNotFoundException)
                            {
                                // 目录已存在，忽略
                            }
                        }
                    });
                }

                // 上传文件
                using var fileStream = new FileStream(localPath, FileMode.Open);
                await Task.Run(() => sftp.UploadFile(fileStream, remotePath));

                _logger.LogInformation("File uploaded successfully to {Host}:{RemotePath}", connection.Host, remotePath);
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }

        public async Task DownloadFileAsync(SshConnection connection, string remotePath, string localPath)
        {
            _logger.LogInformation("Downloading file from {Host}:{RemotePath} to {LocalPath}", connection.Host, remotePath, localPath);

            using var sftp = new SftpClient(connection.Client.ConnectionInfo);
            await Task.Run(() => sftp.Connect());

            try
            {
                // 确保本地目录存在
                var localDir = Path.GetDirectoryName(localPath);
                if (!string.IsNullOrEmpty(localDir) && !Directory.Exists(localDir))
                {
                    Directory.CreateDirectory(localDir);
                }

                // 下载文件
                using var fileStream = new FileStream(localPath, FileMode.Create);
                await Task.Run(() => sftp.DownloadFile(remotePath, fileStream));

                _logger.LogInformation("File downloaded successfully to {LocalPath}", localPath);
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }

        public async Task CloseConnectionAsync(SshConnection connection)
        {
            _logger.LogInformation("Closing connection to {Host}:{Port}", connection.Host, connection.Port);

            await Task.Run(() =>
            {
                if (connection.Client.IsConnected)
                {
                    connection.Client.Disconnect();
                }
                connection.Client.Dispose();
            });

            _logger.LogInformation("Connection closed successfully");
        }
    }

    public interface ISshSessionManager
    {
        Task<SshSession> CreateSessionAsync(string host, int port, string username, string password);
        Task<SshSession> CreateSessionAsync(string host, int port, string username, string privateKeyPath, string passphrase = null);
        Task CloseSessionAsync(SshSession session);
        Task<IEnumerable<SshSession>> GetActiveSessionsAsync();
    }

    public class SshSessionManager : ISshSessionManager
    {
        private readonly ISshService _sshService;
        private readonly List<SshSession> _sessions = new();
        private readonly object _lock = new();

        public SshSessionManager(ISshService sshService)
        {
            _sshService = sshService;
        }

        public async Task<SshSession> CreateSessionAsync(string host, int port, string username, string password)
        {
            var connection = await _sshService.ConnectAsync(host, port, username, password);
            var session = new SshSession(connection, host, port, username);

            lock (_lock)
            {
                _sessions.Add(session);
            }

            return session;
        }

        public async Task<SshSession> CreateSessionAsync(string host, int port, string username, string privateKeyPath, string passphrase = null)
        {
            var connection = await _sshService.ConnectAsync(host, port, username, privateKeyPath, passphrase);
            var session = new SshSession(connection, host, port, username);

            lock (_lock)
            {
                _sessions.Add(session);
            }

            return session;
        }

        public async Task CloseSessionAsync(SshSession session)
        {
            await _sshService.CloseConnectionAsync(session.Connection);

            lock (_lock)
            {
                _sessions.Remove(session);
            }

            session.Dispose();
        }

        public Task<IEnumerable<SshSession>> GetActiveSessionsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_sessions.AsEnumerable());
            }
        }
    }

    public interface ISshFileTransferService
    {
        Task UploadAsync(SshConnection connection, string localPath, string remotePath, bool recursive = false);
        Task DownloadAsync(SshConnection connection, string remotePath, string localPath, bool recursive = false);
        Task<IEnumerable<string>> ListFilesAsync(SshConnection connection, string remotePath);
        Task CreateDirectoryAsync(SshConnection connection, string remotePath);
        Task DeleteFileAsync(SshConnection connection, string remotePath);
    }

    public class SshFileTransferService : ISshFileTransferService
    {
        private readonly ILogger<SshFileTransferService> _logger;

        public SshFileTransferService(ILogger<SshFileTransferService> logger)
        {
            _logger = logger;
        }

        public async Task UploadAsync(SshConnection connection, string localPath, string remotePath, bool recursive = false)
        {
            using var sftp = new SftpClient(connection.Client.ConnectionInfo);
            await Task.Run(() => sftp.Connect());

            try
            {
                if (Directory.Exists(localPath) && recursive)
                {
                    await UploadDirectoryAsync(sftp, localPath, remotePath);
                }
                else if (File.Exists(localPath))
                {
                    await UploadSingleFileAsync(sftp, localPath, remotePath);
                }
                else
                {
                    throw new FileNotFoundException("Local path not found", localPath);
                }
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }

        private async Task UploadDirectoryAsync(SftpClient sftp, string localDir, string remoteDir)
        {
            // 创建远程目录
            await Task.Run(() =>
            {
                try
                {
                    sftp.CreateDirectory(remoteDir);
                }
                catch (SftpPathNotFoundException)
                {
                    // 目录已存在，忽略
                }
            });

            // 上传文件
            var files = Directory.GetFiles(localDir);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var remoteFilePath = Path.Combine(remoteDir, fileName).Replace('\\', '/');
                await UploadSingleFileAsync(sftp, file, remoteFilePath);
            }

            // 递归上传子目录
            var subDirs = Directory.GetDirectories(localDir);
            foreach (var subDir in subDirs)
            {
                var dirName = Path.GetFileName(subDir);
                var remoteSubDir = Path.Combine(remoteDir, dirName).Replace('\\', '/');
                await UploadDirectoryAsync(sftp, subDir, remoteSubDir);
            }
        }

        private async Task UploadSingleFileAsync(SftpClient sftp, string localFile, string remoteFile)
        {
            _logger.LogInformation("Uploading file: {LocalFile} -> {RemoteFile}", localFile, remoteFile);

            using var fileStream = new FileStream(localFile, FileMode.Open);
            await Task.Run(() => sftp.UploadFile(fileStream, remoteFile));

            _logger.LogInformation("File uploaded successfully: {RemoteFile}", remoteFile);
        }

        public async Task DownloadAsync(SshConnection connection, string remotePath, string localPath, bool recursive = false)
        {
            using var sftp = new SftpClient(connection.Client.ConnectionInfo);
            await Task.Run(() => sftp.Connect());

            try
            {
                var fileAttributes = await Task.Run(() => sftp.GetAttributes(remotePath));

                if (fileAttributes.IsDirectory && recursive)
                {
                    await DownloadDirectoryAsync(sftp, remotePath, localPath);
                }
                else
                {
                    await DownloadSingleFileAsync(sftp, remotePath, localPath);
                }
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }

        private async Task DownloadDirectoryAsync(SftpClient sftp, string remoteDir, string localDir)
        {
            // 创建本地目录
            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            // 下载文件
            var files = await Task.Run(() => sftp.ListDirectory(remoteDir).Where(f => !f.IsDirectory).ToList());
            foreach (var file in files)
            {
                var localFilePath = Path.Combine(localDir, file.Name);
                await DownloadSingleFileAsync(sftp, file.FullName, localFilePath);
            }

            // 递归下载子目录
            var subDirs = await Task.Run(() => sftp.ListDirectory(remoteDir).Where(f => f.IsDirectory && f.Name != "." && f.Name != "..").ToList());
            foreach (var subDir in subDirs)
            {
                var localSubDir = Path.Combine(localDir, subDir.Name);
                await DownloadDirectoryAsync(sftp, subDir.FullName, localSubDir);
            }
        }

        private async Task DownloadSingleFileAsync(SftpClient sftp, string remoteFile, string localFile)
        {
            _logger.LogInformation("Downloading file: {RemoteFile} -> {LocalFile}", remoteFile, localFile);

            // 确保本地目录存在
            var localDir = Path.GetDirectoryName(localFile);
            if (!string.IsNullOrEmpty(localDir) && !Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            using var fileStream = new FileStream(localFile, FileMode.Create);
            await Task.Run(() => sftp.DownloadFile(remoteFile, fileStream));

            _logger.LogInformation("File downloaded successfully: {LocalFile}", localFile);
        }

        public async Task<IEnumerable<string>> ListFilesAsync(SshConnection connection, string remotePath)
        {
            using var sftp = new SftpClient(connection.Client.ConnectionInfo);
            await Task.Run(() => sftp.Connect());

            try
            {
                var files = await Task.Run(() => sftp.ListDirectory(remotePath)
                    .Select(f => f.FullName)
                    .ToList());

                return files;
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }

        public async Task CreateDirectoryAsync(SshConnection connection, string remotePath)
        {
            using var sftp = new SftpClient(connection.Client.ConnectionInfo);
            await Task.Run(() => sftp.Connect());

            try
            {
                await Task.Run(() => sftp.CreateDirectory(remotePath));
                _logger.LogInformation("Directory created successfully: {RemotePath}", remotePath);
            }
            catch (SftpPathNotFoundException)
            {
                // 目录已存在，忽略
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }

        public async Task DeleteFileAsync(SshConnection connection, string remotePath)
        {
            using var sftp = new SftpClient(connection.Client.ConnectionInfo);
            await Task.Run(() => sftp.Connect());

            try
            {
                await Task.Run(() => sftp.Delete(remotePath));
                _logger.LogInformation("File deleted successfully: {RemotePath}", remotePath);
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }
    }

    public interface ISshConfigService
    {
        Task<SshConfig> LoadConfigAsync(string configPath);
        Task SaveConfigAsync(SshConfig config, string configPath);
        Task<SshConfigEntry> GetConfigEntryAsync(string host);
    }

    public class SshConfigService : ISshConfigService
    {
        private readonly ILogger<SshConfigService> _logger;

        public SshConfigService(ILogger<SshConfigService> logger)
        {
            _logger = logger;
        }

        public async Task<SshConfig> LoadConfigAsync(string configPath)
        {
            _logger.LogInformation("Loading config from: {ConfigPath}", configPath);

            if (!File.Exists(configPath))
            {
                _logger.LogWarning("Config file not found: {ConfigPath}, returning empty config", configPath);
                return new SshConfig();
            }

            var json = await File.ReadAllTextAsync(configPath, Encoding.UTF8);
            var config = System.Text.Json.JsonSerializer.Deserialize<SshConfig>(json) ?? new SshConfig();

            _logger.LogInformation("Config loaded successfully from: {ConfigPath}", configPath);
            return config;
        }

        public async Task SaveConfigAsync(SshConfig config, string configPath)
        {
            _logger.LogInformation("Saving config to: {ConfigPath}", configPath);

            var json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(configPath, json, Encoding.UTF8);

            _logger.LogInformation("Config saved successfully to: {ConfigPath}", configPath);
        }

        public Task<SshConfigEntry> GetConfigEntryAsync(string host)
        {
            // 这里可以实现从配置文件或其他来源获取配置项
            // 暂时返回空实现
            return Task.FromResult(new SshConfigEntry
            {
                Host = host,
                Port = 22,
                Username = Environment.UserName
            });
        }
    }

    public interface ISshKeyManager
    {
        Task<(string PublicKey, string PrivateKey)> GenerateKeyPairAsync(string algorithm = "RSA", int keySize = 2048);
        Task<string> LoadPrivateKeyAsync(string privateKeyPath, string passphrase = null);
        Task SaveKeyPairAsync(string publicKey, string privateKey, string publicKeyPath, string privateKeyPath);
    }

    public class SshKeyManager : ISshKeyManager
    {
        private readonly ILogger<SshKeyManager> _logger;

        public SshKeyManager(ILogger<SshKeyManager> logger)
        {
            _logger = logger;
        }

        public async Task<(string PublicKey, string PrivateKey)> GenerateKeyPairAsync(string algorithm = "RSA", int keySize = 2048)
        {
            _logger.LogInformation("Generating key pair with algorithm: {Algorithm}, key size: {KeySize}", algorithm, keySize);

            // 这里应该实现密钥对生成
            // 暂时返回模拟数据
            await Task.Delay(100);

            var privateKey = $"-----BEGIN {algorithm} PRIVATE KEY-----\nMIIEpQIBAAKCAQEA...\n-----END {algorithm} PRIVATE KEY-----";
            var publicKey = $"ssh-{algorithm.ToLower()} AAAAB3NzaC1yc2EAAA... user@host";

            _logger.LogInformation("Key pair generated successfully");
            return (PublicKey: publicKey, PrivateKey: privateKey);
        }

        public async Task<string> LoadPrivateKeyAsync(string privateKeyPath, string passphrase = null)
        {
            _logger.LogInformation("Loading private key from: {PrivateKeyPath}", privateKeyPath);

            if (!File.Exists(privateKeyPath))
            {
                throw new FileNotFoundException("Private key file not found", privateKeyPath);
            }

            var privateKey = await File.ReadAllTextAsync(privateKeyPath, Encoding.UTF8);

            _logger.LogInformation("Private key loaded successfully from: {PrivateKeyPath}", privateKeyPath);
            return privateKey;
        }

        public async Task SaveKeyPairAsync(string publicKey, string privateKey, string publicKeyPath, string privateKeyPath)
        {
            _logger.LogInformation("Saving key pair to public: {PublicKeyPath}, private: {PrivateKeyPath}", publicKeyPath, privateKeyPath);

            // 保存公钥
            await File.WriteAllTextAsync(publicKeyPath, publicKey, Encoding.UTF8);

            // 保存私钥并设置权限
            await File.WriteAllTextAsync(privateKeyPath, privateKey, Encoding.UTF8);

            // 在Windows上，我们不能直接设置Unix权限，但可以尝试
            try
            {
                var fileInfo = new FileInfo(privateKeyPath);
                fileInfo.Attributes = FileAttributes.Normal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to set file permissions: {Exception}", ex.Message);
            }

            _logger.LogInformation("Key pair saved successfully");
        }
    }

    public interface ISshBatchService
    {
        Task<Dictionary<string, string>> ExecuteBatchCommandAsync(IEnumerable<string> hosts, string command);
        Task<Dictionary<string, bool>> UploadBatchFileAsync(IEnumerable<string> hosts, string localPath, string remotePath);
        Task<Dictionary<string, bool>> DownloadBatchFileAsync(IEnumerable<string> hosts, string remotePath, string localPath);
    }

    public class SshBatchService : ISshBatchService
    {
        private readonly ISshService _sshService;
        private readonly ILogger<SshBatchService> _logger;

        public SshBatchService(ISshService sshService, ILogger<SshBatchService> logger)
        {
            _sshService = sshService;
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> ExecuteBatchCommandAsync(IEnumerable<string> hosts, string command)
        {
            _logger.LogInformation("Executing batch command on {HostCount} hosts: {Command}", hosts.Count(), command);

            var tasks = hosts.Select(async host =>
            {
                try
                {
                    // 这里应该从配置中获取连接信息
                    var connection = await _sshService.ConnectAsync(host, 22, Environment.UserName, "password");
                    var result = await _sshService.ExecuteCommandAsync(connection, command);
                    await _sshService.CloseConnectionAsync(connection);
                    return (Host: host, Result: result, Error: (Exception)null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing command on {Host}", host);
                    return (Host: host, Result: string.Empty, Error: ex);
                }
            });

            var results = await Task.WhenAll(tasks);
            var resultDict = new Dictionary<string, string>();

            foreach (var (host, result, error) in results)
            {
                if (error != null)
                {
                    resultDict[host] = $"Error: {error.Message}