#:sdk Microsoft.NET.Sdk
#:package ProcessX@3.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace ProcessXIntegration
{
    public class ProcessMonitorOptions
    {
        public TimeSpan CpuUsageCheckInterval { get; set; } = TimeSpan.FromSeconds(5);
        public float MaxCpuUsagePercentage { get; set; } = 80;
        public long MaxMemoryBytes { get; set; } = 1024 * 1024 * 1024; // 1GB
        public bool EnableCrossPlatformSupport { get; set; } = true;
    }

    public class GarnetOptions
    {
        public string ExecutablePath { get; set; } = "garnet.exe";
        public string ConfigPath { get; set; } = "garnet.conf";
        public int Port { get; set; } = 6379;
        public bool EnableTls { get; set; } = false;
        public string TlsCertificatePath { get; set; } = string.Empty;
    }

    public class MySqlOptions
    {
        public string DataDir { get; set; } = "C:\\mysql\\data";
        public string BackupDir { get; set; } = "C:\\mysql\\backup";
        public string Port { get; set; } = "3306";
        public string RootPassword { get; set; } = "password";
        public TimeSpan BackupInterval { get; set; } = TimeSpan.FromHours(24);
        public int MaxBackupCount { get; set; } = 7;
    }

    public class MqttOptions
    {
        public string ExecutablePath { get; set; } = "mqttnet.exe";
        public int Port { get; set; } = 1883;
        public bool EnableWebSockets { get; set; } = false;
        public int WebSocketPort { get; set; } = 8080;
        public bool EnableTls { get; set; } = false;
        public string TlsCertificatePath { get; set; } = string.Empty;
    }
    public enum ProcessPriority
    {
        Normal,
        BelowNormal,
        AboveNormal,
        High,
        RealTime
    }

    public class ProcessXOptions
    {
        public string DefaultWorkingDirectory { get; set; } = ".";
        public ProcessPriority DefaultPriority { get; set; } = ProcessPriority.Normal;
        public int MaxConcurrentProcesses { get; set; } = 4;
        public bool EnablePerformanceCounters { get; set; } = true;
        public bool EnableDistributedTracing { get; set; } = false;
    }

    public interface IProcessMonitorService
    {
        Task StartMonitoringAsync(int processId, ProcessMonitorOptions options, CancellationToken cancellationToken = default);
        Task StopMonitoringAsync(int processId);
        Task<bool> IsProcessHealthyAsync(int processId);
    }

    public interface IProcessXService
    {
        Task<Process> StartAsync(string fileName, string arguments = null, ProcessPriority? priority = null, string workingDirectory = null, CancellationToken cancellationToken = default);
        Task<int> RunAsync(string fileName, string arguments = null, ProcessPriority? priority = null, string workingDirectory = null, CancellationToken cancellationToken = default);
        Task KillAllAsync();
    }

    public class ProcessMonitorService : IProcessMonitorService, IDisposable
    {
        private readonly ConcurrentDictionary<int, (Process, CancellationTokenSource)> _monitoredProcesses = new();
        private readonly ILogger<ProcessMonitorService> _logger;

        public ProcessMonitorService(ILogger<ProcessMonitorService> logger)
        {
            _logger = logger;
        }

        public async Task StartMonitoringAsync(int processId, ProcessMonitorOptions options, CancellationToken cancellationToken = default)
        {
            // 实现监控逻辑
        }

        public async Task StopMonitoringAsync(int processId)
        {
            // 实现停止监控逻辑
        }

        public async Task<bool> IsProcessHealthyAsync(int processId)
        {
            // 实现健康检查逻辑
            return true;
        }

        public void Dispose()
        {
            // 实现资源释放
        }
    }

    public class ProcessXService : IProcessXService, IDisposable
    {
        private readonly ILogger<ProcessXService> _logger;
        private readonly ProcessXOptions _options;
        private readonly Channel<Process> _processChannel;
        private readonly List<Process> _activeProcesses = new();
        private readonly object _lock = new();

        public ProcessXService(IOptions<ProcessXOptions> options, ILogger<ProcessXService> logger)
        {
            _logger = logger;
            _options = options.Value;
            _processChannel = Channel.CreateBounded<Process>(_options.MaxConcurrentProcesses);
        }

        public async Task<Process> StartAsync(string fileName, string arguments = null, ProcessPriority? priority = null, string workingDirectory = null, CancellationToken cancellationToken = default)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments ?? string.Empty,
                    WorkingDirectory = workingDirectory ?? _options.DefaultWorkingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            await _processChannel.Writer.WriteAsync(process, cancellationToken);

            try
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException($"Failed to start process {fileName}");
                }

                SetProcessPriority(process, priority ?? _options.DefaultPriority);

                lock (_lock)
                {
                    _activeProcesses.Add(process);
                }

                process.Exited += (sender, e) =>
                {
                    lock (_lock)
                    {
                        _activeProcesses.Remove(process);
                    }
                    _processChannel.Writer.TryWrite(process);
                };

                _logger.LogInformation("Started process {FileName} with PID {ProcessId}", fileName, process.Id);
                return process;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start process {FileName}", fileName);
                throw;
            }
        }

        public async Task<int> RunAsync(string fileName, string arguments = null, ProcessPriority? priority = null, string workingDirectory = null, CancellationToken cancellationToken = default)
        {
            using var process = await StartAsync(fileName, arguments, priority, workingDirectory, cancellationToken);
            await process.WaitForExitAsync(cancellationToken);
            return process.ExitCode;
        }

        public Task KillAllAsync()
        {
            lock (_lock)
            {
                foreach (var process in _activeProcesses)
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            _logger.LogInformation("Killed process {ProcessId}", process.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to kill process {ProcessId}", process.Id);
                    }
                }
                _activeProcesses.Clear();
            }
            return Task.CompletedTask;
        }

        private static void SetProcessPriority(Process process, ProcessPriority priority)
        {
            switch (priority)
            {
                case ProcessPriority.BelowNormal:
                    process.PriorityClass = ProcessPriorityClass.BelowNormal;
                    break;
                case ProcessPriority.AboveNormal:
                    process.PriorityClass = ProcessPriorityClass.AboveNormal;
                    break;
                case ProcessPriority.High:
                    process.PriorityClass = ProcessPriorityClass.High;
                    break;
                case ProcessPriority.RealTime:
                    process.PriorityClass = ProcessPriorityClass.RealTime;
                    break;
                default:
                    process.PriorityClass = ProcessPriorityClass.Normal;
                    break;
            }
        }

        public void Dispose()
        {
            KillAllAsync().Wait();
            GC.SuppressFinalize(this);
        }
    }

    public static class ProcessMonitorExtensions
    {
        public static IServiceCollection AddProcessMonitor(this IServiceCollection services, Action<ProcessMonitorOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IProcessMonitorService, ProcessMonitorService>();
            return services;
        }
    }

    public static class MySqlExtensions
    {
        public static IServiceCollection AddMySqlServer(this IServiceCollection services, Action<MySqlOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IMySqlService, MySqlService>();
            return services;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProcessX(this IServiceCollection services, Action<ProcessXOptions> configure = null)
        {
            services.Configure(configure ?? (opt => { }));
            services.AddSingleton<IProcessXService, ProcessXService>();
            return services;
        }

        public static IServiceCollection AddGarnetServer(this IServiceCollection services, Action<GarnetOptions> configure = null)
        {
            services.Configure(configure ?? (opt => { }));
            services.AddSingleton<IGarnetService, GarnetService>();
            return services;
        }

        public static IServiceCollection AddMqttServer(this IServiceCollection services, Action<MqttOptions> configure = null)
        {
            services.Configure(configure ?? (opt => { }));
            services.AddSingleton<IMqttService, MqttService>();
            return services;
        }

        public static IServiceCollection AddSqliteServer(this IServiceCollection services, Action<SqliteOptions> configure = null)
        {
            services.Configure(configure ?? (opt => { }));
            services.AddSingleton<ISqliteService, SqliteService>();
            return services;
        }

        public static IServiceCollection AddLiteDbServer(this IServiceCollection services, Action<LiteDbOptions> configure = null)
        {
            services.Configure(configure ?? (opt => { }));
            services.AddSingleton<ILiteDbService, LiteDbService>();
            return services;
        }
    }

    public interface IGarnetService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
    }

    public interface IMySqlService
    {
        Task StartAsync(MySqlOptions options, CancellationToken cancellationToken = default);
        Task StopAsync();
        Task CreateSnapshotAsync(string snapshotName);
        Task RestoreSnapshotAsync(string snapshotName);
        Task BackupDatabaseAsync(string backupName);
        Task RestoreDatabaseAsync(string backupName);
        Task<IEnumerable<string>> ListSnapshotsAsync();
        Task<IEnumerable<string>> ListBackupsAsync();
    }

    public interface IMqttService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
    }

    public class GarnetService : IGarnetService
    {
        private readonly IProcessXService _processService;
        private readonly IOptions<GarnetOptions> _options;
        private readonly ILogger<GarnetService> _logger;
        private Process _process;

        public GarnetService(IProcessXService processService, IOptions<GarnetOptions> options, ILogger<GarnetService> logger)
        {
            _processService = processService;
            _options = options;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var args = new StringBuilder();
            args.Append($"--port {_options.Value.Port} ");
            
            if (!string.IsNullOrEmpty(_options.Value.ConfigPath))
                args.Append($"--config {_options.Value.ConfigPath} ");
            
            if (_options.Value.EnableTls && !string.IsNullOrEmpty(_options.Value.TlsCertificatePath))
                args.Append($"--tls-cert {_options.Value.TlsCertificatePath} ");

            _process = await _processService.StartAsync(_options.Value.ExecutablePath, args.ToString(), cancellationToken: cancellationToken);
            _logger.LogInformation("Started Garnet server on port {Port}", _options.Value.Port);
        }

        public async Task StopAsync()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                await _process.WaitForExitAsync();
                _logger.LogInformation("Stopped Garnet server");
            }
        }
    }

    public class MySqlService : IMySqlService, IDisposable
    {
        private readonly IProcessXService _processService;
        private readonly ILogger<MySqlService> _logger;
        private Process _mysqlProcess;

        public MySqlService(IProcessXService processService, ILogger<MySqlService> logger)
        {
            _processService = processService;
            _logger = logger;
        }

    public class SqliteService : ISqliteService, IDisposable
    {
        private readonly IProcessXService _processService;
        private readonly ILogger<SqliteService> _logger;
        private Process _sqliteProcess;

        public SqliteService(IProcessXService processService, ILogger<SqliteService> logger)
        {
            _processService = processService;
            _logger = logger;
        }

        public async Task StartAsync(MySqlOptions options, CancellationToken cancellationToken = default)
        {
            if (_sqliteProcess != null && !_sqliteProcess.HasExited)
                throw new InvalidOperationException("SQLite server is already running");

            Directory.CreateDirectory(options.DataDirectory);
            Directory.CreateDirectory(options.BackupDirectory);

            var connectionString = $"Data Source={Path.Combine(options.DataDirectory, "database.db")}";
            _logger.LogInformation("Starting SQLite with connection string: {ConnectionString}", connectionString);
            
            _sqliteProcess = await _processService.StartProcessAsync(
                "sqlite3", 
                Path.Combine(options.DataDirectory, "database.db"),
                new ProcessXOptions { WorkingDirectory = options.DataDirectory });
        }

        public async Task StopAsync()
        {
            if (_sqliteProcess == null || _sqliteProcess.HasExited)
                return;

            await _processService.StopProcessAsync(_sqliteProcess);
            _sqliteProcess = null;
            _logger.LogInformation("Stopped SQLite server");
        }

        public async Task CreateSnapshotAsync(string snapshotName)
        {
            if (_sqliteProcess == null || _sqliteProcess.HasExited)
                throw new InvalidOperationException("SQLite server is not running");

            var snapshotPath = Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "snapshots", $"{snapshotName}.db");
            Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath));
            File.Copy(Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "database.db"), snapshotPath, true);
        }

        public async Task RestoreSnapshotAsync(string snapshotName)
        {
            if (_sqliteProcess == null || _sqliteProcess.HasExited)
                throw new InvalidOperationException("SQLite server is not running");

            var snapshotPath = Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "snapshots", $"{snapshotName}.db");
            if (!File.Exists(snapshotPath))
                throw new FileNotFoundException("Snapshot file not found", snapshotPath);

            File.Copy(snapshotPath, Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "database.db"), true);
        }

        public async Task BackupDatabaseAsync(string backupName)
        {
            if (_sqliteProcess == null || _sqliteProcess.HasExited)
                throw new InvalidOperationException("SQLite server is not running");

            var backupPath = Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "backups", $"{backupName}.db");
            Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
            File.Copy(Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "database.db"), backupPath, true);
        }

        public async Task RestoreDatabaseAsync(string backupName)
        {
            if (_sqliteProcess == null || _sqliteProcess.HasExited)
                throw new InvalidOperationException("SQLite server is not running");

            var backupPath = Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "backups", $"{backupName}.db");
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found", backupPath);

            File.Copy(backupPath, Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "database.db"), true);
        }

        public async Task<IEnumerable<string>> ListSnapshotsAsync()
        {
            if (_sqliteProcess == null)
                return Enumerable.Empty<string>();

            var snapshotsDir = Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "snapshots");
            return Directory.Exists(snapshotsDir) 
                ? Directory.GetFiles(snapshotsDir, "*.db").Select(Path.GetFileNameWithoutExtension)
                : Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<string>> ListBackupsAsync()
        {
            if (_sqliteProcess == null)
                return Enumerable.Empty<string>();

            var backupsDir = Path.Combine(_sqliteProcess.StartInfo.WorkingDirectory, "backups");
            return Directory.Exists(backupsDir) 
                ? Directory.GetFiles(backupsDir, "*.db").Select(Path.GetFileNameWithoutExtension)
                : Enumerable.Empty<string>();
        }

        public void Dispose()
        {
            _sqliteProcess?.Dispose();
        }
    }

    public class LiteDbService : ILiteDbService, IDisposable
    {
        private readonly IProcessXService _processService;
        private readonly ILogger<LiteDbService> _logger;
        private Process _liteDbProcess;

        public LiteDbService(IProcessXService processService, ILogger<LiteDbService> logger)
        {
            _processService = processService;
            _logger = logger;
        }

        public async Task StartAsync(MySqlOptions options, CancellationToken cancellationToken = default)
        {
            if (_liteDbProcess != null && !_liteDbProcess.HasExited)
                throw new InvalidOperationException("LiteDB server is already running");

            Directory.CreateDirectory(options.DataDirectory);
            Directory.CreateDirectory(options.BackupDirectory);

            var connectionString = $"Filename={Path.Combine(options.DataDirectory, "database.db")};Connection=shared";
            _logger.LogInformation("Starting LiteDB with connection string: {ConnectionString}", connectionString);
            
            _liteDbProcess = await _processService.StartProcessAsync(
                "litedb", 
                Path.Combine(options.DataDirectory, "database.db"),
                new ProcessXOptions { WorkingDirectory = options.DataDirectory });
        }

        public async Task StopAsync()
        {
            if (_liteDbProcess == null || _liteDbProcess.HasExited)
                return;

            await _processService.StopProcessAsync(_liteDbProcess);
            _liteDbProcess = null;
            _logger.LogInformation("Stopped LiteDB server");
        }

        public async Task CreateSnapshotAsync(string snapshotName)
        {
            if (_liteDbProcess == null || _liteDbProcess.HasExited)
                throw new InvalidOperationException("LiteDB server is not running");

            var snapshotPath = Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "snapshots", $"{snapshotName}.db");
            Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath));
            File.Copy(Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "database.db"), snapshotPath, true);
        }

        public async Task RestoreSnapshotAsync(string snapshotName)
        {
            if (_liteDbProcess == null || _liteDbProcess.HasExited)
                throw new InvalidOperationException("LiteDB server is not running");

            var snapshotPath = Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "snapshots", $"{snapshotName}.db");
            if (!File.Exists(snapshotPath))
                throw new FileNotFoundException("Snapshot file not found", snapshotPath);

            File.Copy(snapshotPath, Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "database.db"), true);
        }

        public async Task BackupDatabaseAsync(string backupName)
        {
            if (_liteDbProcess == null || _liteDbProcess.HasExited)
                throw new InvalidOperationException("LiteDB server is not running");

            var backupPath = Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "backups", $"{backupName}.db");
            Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
            File.Copy(Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "database.db"), backupPath, true);
        }

        public async Task RestoreDatabaseAsync(string backupName)
        {
            if (_liteDbProcess == null || _liteDbProcess.HasExited)
                throw new InvalidOperationException("LiteDB server is not running");

            var backupPath = Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "backups", $"{backupName}.db");
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found", backupPath);

            File.Copy(backupPath, Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "database.db"), true);
        }

        public async Task<IEnumerable<string>> ListSnapshotsAsync()
        {
            if (_liteDbProcess == null)
                return Enumerable.Empty<string>();

            var snapshotsDir = Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "snapshots");
            return Directory.Exists(snapshotsDir) 
                ? Directory.GetFiles(snapshotsDir, "*.db").Select(Path.GetFileNameWithoutExtension)
                : Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<string>> ListBackupsAsync()
        {
            if (_liteDbProcess == null)
                return Enumerable.Empty<string>();

            var backupsDir = Path.Combine(_liteDbProcess.StartInfo.WorkingDirectory, "backups");
            return Directory.Exists(backupsDir) 
                ? Directory.GetFiles(backupsDir, "*.db").Select(Path.GetFileNameWithoutExtension)
                : Enumerable.Empty<string>();
        }

        public void Dispose()
        {
            _liteDbProcess?.Dispose();
        }
    {
        private readonly IProcessXService _processService;
        private readonly ILogger<MySqlService> _logger;
        private Process _mysqlProcess;

        public MySqlService(IProcessXService processService, ILogger<MySqlService> logger)
        {
            _processService = processService;
            _logger = logger;
        }

        public async Task StartAsync(MySqlOptions options, CancellationToken cancellationToken = default)
        {
            if (_mysqlProcess != null && !_mysqlProcess.HasExited)
                throw new InvalidOperationException("MySQL server is already running");

            var args = new StringBuilder()
                .Append($"--datadir={options.DataDirectory}")
                .Append($" --port={options.Port}")
                .Append($" --default_authentication_plugin=mysql_native_password")
                .Append($" --skip-grant-tables");

            if (!string.IsNullOrEmpty(options.RootPassword))
                args.Append($" --init-file={Path.Combine(options.DataDirectory, "init.sql")}");

            _mysqlProcess = await _processService.StartProcessAsync(
                "mysqld",
                args.ToString(),
                new ProcessXOptions
                {
                    WorkingDirectory = options.DataDirectory,
                    Priority = ProcessPriority.Normal
                });

            _logger.LogInformation("Started MySQL server on port {Port}", options.Port);
        }

        public async Task StopAsync()
        {
            if (_mysqlProcess == null || _mysqlProcess.HasExited)
                return;

            await _processService.StopProcessAsync(_mysqlProcess);
            _mysqlProcess = null;
            _logger.LogInformation("Stopped MySQL server");
        }

        public async Task CreateSnapshotAsync(string snapshotName)
        {
            if (_mysqlProcess == null || _mysqlProcess.HasExited)
                throw new InvalidOperationException("MySQL server is not running");

            var snapshotDir = Path.Combine(Path.GetDirectoryName(_mysqlProcess.StartInfo.WorkingDirectory), "snapshots", snapshotName);
            Directory.CreateDirectory(snapshotDir);

            await _processService.RunProcessAsync(
                "mysqldump",
                $"--all-databases --result-file={Path.Combine(snapshotDir, "snapshot.sql")}",
                new ProcessXOptions { WorkingDirectory = _mysqlProcess.StartInfo.WorkingDirectory });

            _logger.LogInformation("Created MySQL snapshot: {SnapshotName}", snapshotName);
        }

        public async Task RestoreSnapshotAsync(string snapshotName)
        {
            if (_mysqlProcess == null || _mysqlProcess.HasExited)
                throw new InvalidOperationException("MySQL server is not running");

            var snapshotFile = Path.Combine(
                Path.GetDirectoryName(_mysqlProcess.StartInfo.WorkingDirectory),
                "snapshots",
                snapshotName,
                "snapshot.sql");

            if (!File.Exists(snapshotFile))
                throw new FileNotFoundException("Snapshot file not found", snapshotFile);

            await _processService.RunProcessAsync(
                "mysql",
                $"< {snapshotFile}",
                new ProcessXOptions { WorkingDirectory = _mysqlProcess.StartInfo.WorkingDirectory });

            _logger.LogInformation("Restored MySQL from snapshot: {SnapshotName}", snapshotName);
        }

        public async Task BackupDatabaseAsync(string backupName)
        {
            if (_mysqlProcess == null || _mysqlProcess.HasExited)
                throw new InvalidOperationException("MySQL server is not running");

            var backupDir = Path.Combine(Path.GetDirectoryName(_mysqlProcess.StartInfo.WorkingDirectory), "backups", backupName);
            Directory.CreateDirectory(backupDir);

            await _processService.RunProcessAsync(
                "mysqldump",
                $"--all-databases --single-transaction --quick --lock-tables=false --result-file={Path.Combine(backupDir, "backup.sql")}",
                new ProcessXOptions { WorkingDirectory = _mysqlProcess.StartInfo.WorkingDirectory });

            _logger.LogInformation("Created MySQL backup: {BackupName}", backupName);
        }

        public async Task RestoreDatabaseAsync(string backupName)
        {
            if (_mysqlProcess == null || _mysqlProcess.HasExited)
                throw new InvalidOperationException("MySQL server is not running");

            var backupFile = Path.Combine(
                Path.GetDirectoryName(_mysqlProcess.StartInfo.WorkingDirectory),
                "backups",
                backupName,
                "backup.sql");

            if (!File.Exists(backupFile))
                throw new FileNotFoundException("Backup file not found", backupFile);

            await _processService.RunProcessAsync(
                "mysql",
                $"< {backupFile}",
                new ProcessXOptions { WorkingDirectory = _mysqlProcess.StartInfo.WorkingDirectory });

            _logger.LogInformation("Restored MySQL from backup: {BackupName}", backupName);
        }

        public async Task<IEnumerable<string>> ListSnapshotsAsync()
        {
            if (_mysqlProcess == null)
                return Enumerable.Empty<string>();

            var snapshotsDir = Path.Combine(Path.GetDirectoryName(_mysqlProcess.StartInfo.WorkingDirectory), "snapshots");
            return Directory.Exists(snapshotsDir) 
                ? Directory.GetDirectories(snapshotsDir).Select(Path.GetFileName)
                : Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<string>> ListBackupsAsync()
        {
            if (_mysqlProcess == null)
                return Enumerable.Empty<string>();

            var backupsDir = Path.Combine(Path.GetDirectoryName(_mysqlProcess.StartInfo.WorkingDirectory), "backups");
            return Directory.Exists(backupsDir) 
                ? Directory.GetDirectories(backupsDir).Select(Path.GetFileName)
                : Enumerable.Empty<string>();
            return Enumerable.Empty<string>();
        }

        public void Dispose()
        {
            // 实现资源释放
        }
    }

    public class MqttService : IMqttService
    {
        private readonly IProcessXService _processService;
        private readonly IOptions<MqttOptions> _options;
        private readonly ILogger<MqttService> _logger;
        private Process _process;

        public MqttService(IProcessXService processService, IOptions<MqttOptions> options, ILogger<MqttService> logger)
        {
            _processService = processService;
            _options = options;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var args = new StringBuilder();
            args.Append($"--port {_options.Value.Port} ");
            
            if (_options.Value.EnableWebSockets)
                args.Append($"--websocket-port {_options.Value.WebSocketPort} ");
            
            if (_options.Value.EnableTls && !string.IsNullOrEmpty(_options.Value.TlsCertificatePath))
                args.Append($"--tls-cert {_options.Value.TlsCertificatePath} ");

            _process = await _processService.StartAsync(_options.Value.ExecutablePath, args.ToString(), cancellationToken: cancellationToken);
            _logger.LogInformation("Started MQTT server on port {Port}", _options.Value.Port);
        }

        public async Task StopAsync()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                await _process.WaitForExitAsync();
                _logger.LogInformation("Stopped MQTT server");
            }
        }
    }
}