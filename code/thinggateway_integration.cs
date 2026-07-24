#:sdk Microsoft.NET.Sdk.Web
#:package ThingGateway.Foundation@2.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:package System.IO.Ports@8.0.0
#:package MemoryPack@2.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Microsoft.EntityFrameworkCore@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.IO.Ports;
using System.Threading.Channels;
using ThingGateway.Foundation;
using MemoryPack;
using Microsoft.Extensions.ObjectPool;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

[SkipLocalsInit]
public sealed class IotGatewayService : BackgroundService
{
    private readonly Channel<IotDeviceData> _dataChannel;
    private readonly ObjectPool<SerialPort> _serialPortPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly ILogger<IotGatewayService> _logger;
    private readonly Dictionary<string, IDeviceProtocol> _protocols;
    private readonly ObjectPool<DeviceContext> _contextPool;
    
    // 新增字段
    private readonly Channel<DeviceNotification> _notificationChannel;
    private readonly IDistributedCache _cache;
    private readonly IotDbContext _dbContext;
    private readonly FileSystemWatcher _protocolWatcher;
    private readonly Dictionary<string, DeviceConfigVersion> _configVersions = new();

    public IotGatewayService(
        ILogger<IotGatewayService> logger,
        IEnumerable<IDeviceProtocol> protocols,
        IDistributedCache cache,
        IotDbContext dbContext)
    {
        _logger = logger;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // 初始化协议字典
        _protocols = protocols.ToDictionary(p => p.ProtocolName);
        
        _dataChannel = Channel.CreateBounded<IotDeviceData>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
            
        _serialPortPool = new DefaultObjectPool<SerialPort>(
            new SerialPortPooledPolicy(), 
            Environment.ProcessorCount * 2);
            
        _contextPool = new DefaultObjectPool<DeviceContext>(
            new DeviceContextPooledPolicy(),
            Environment.ProcessorCount * 2);
        _cache = cache;
        _dbContext = dbContext;
        
        // 初始化通知通道
        _notificationChannel = Channel.CreateBounded<DeviceNotification>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
            
        // 初始化协议热加载监视器
        _protocolWatcher = new FileSystemWatcher(Path.Combine(AppContext.BaseDirectory, "Protocols"));
        _protocolWatcher.NotifyFilter = NotifyFilters.LastWrite;
        _protocolWatcher.Changed += OnProtocolChanged;
        _protocolWatcher.EnableRaisingEvents = true;
    }

    // 设备状态管理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task UpdateDeviceStatusAsync(string deviceId, DeviceStatus status)
    {
        using var context = _contextPool.Get();
        context.DeviceId = deviceId;
        context.Status = status;
        
        // 更新缓存状态
        await _cache.SetAsync($"device:{deviceId}:status", 
            MemoryPackSerializer.Serialize(status));
            
        // 发布状态变更通知
        await _notificationChannel.Writer.WriteAsync(new DeviceNotification
        {
            DeviceId = deviceId,
            Type = NotificationType.StatusChanged,
            Data = MemoryPackSerializer.Serialize(status)
        });
    }

    // 协议热加载处理
    private void OnProtocolChanged(object sender, FileSystemEventArgs e)
    {
        var protocolName = Path.GetFileNameWithoutExtension(e.Name);
        if (_protocols.TryGetValue(protocolName, out var protocol) && 
            protocol is IHotReloadableProtocol reloadable)
        {
            reloadable.Reload();
            _logger.LogInformation("Protocol {Protocol} reloaded", protocolName);
        }
    }

    // 数据持久化处理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessDataAsync(IotDeviceData data, CancellationToken ct)
    {
        using var context = _contextPool.Get();
        context.DeviceId = data.DeviceId;
        context.Timestamp = DateTime.UtcNow;
        
        // 持久化到数据库
        await _dbContext.DeviceData.AddAsync(new DeviceDataEntity
        {
            DeviceId = data.DeviceId,
            Timestamp = DateTime.UtcNow,
            Data = MemoryPackSerializer.Serialize(data.Payload)
        }, ct);
        
        await _dbContext.SaveChangesAsync(ct);
    }

    // 配置版本控制
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<DeviceConfigVersion> UpdateDeviceConfigAsync(
        string deviceId, ReadOnlyMemory<byte> config)
    {
        var version = new DeviceConfigVersion
        {
            VersionId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            ConfigData = config.ToArray()
        };
        
        _configVersions[deviceId] = version;
        
        // 持久化版本记录
        await _dbContext.DeviceConfigVersions.AddAsync(new DeviceConfigVersionEntity
        {
            DeviceId = deviceId,
            VersionId = version.VersionId,
            Timestamp = version.Timestamp,
            ConfigData = version.ConfigData
        });
        
        await _dbContext.SaveChangesAsync();
        return version;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 合并处理数据通道和通知通道
        await Task.WhenAll(
            ProcessDataChannelAsync(stoppingToken),
            ProcessNotificationsAsync(stoppingToken));
    }
    
    private async Task ProcessNotificationsAsync(CancellationToken ct)
    {
        await foreach (var notification in _notificationChannel.Reader.ReadAllAsync(ct))
        {
            // 处理设备通知逻辑
            // ...
        }
    }

    // 新增设备管理方法
    public async Task AddDeviceAsync(DeviceDefinition device)
    {
        // ... existing code ...
    }

    // 新增协议解析方法
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private DeviceData ParseProtocolData(ReadOnlySpan<byte> rawData, string protocolName)
    {
        if (_protocols.TryGetValue(protocolName, out var protocol))
        {
            var context = _contextPool.Get();
            try
            {
                return protocol.Parse(rawData, context);
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
        throw new KeyNotFoundException($"Protocol {protocolName} not found");
    }

    // 增强的数据处理方法
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var data in _dataChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var port = _serialPortPool.Get();
                // 使用零拷贝技术处理数据
                var parsedData = ParseProtocolData(data.RawData, data.ProtocolName);
                
                // 处理解析后的数据...
                _logger.LogInformation("Processed data: {Data}", parsedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing device data");
            }
        }
    }
}

// 新增设备上下文池策略
internal sealed class DeviceContextPooledPolicy : PooledObjectPolicy<DeviceContext>
{
    public override DeviceContext Create() => new DeviceContext();

    public override bool Return(DeviceContext obj)
    {
        obj.Reset();
        return true;
    }
}

// 新增设备上下文类
[MemoryPackable]
public partial class DeviceContext
{
    public Dictionary<string, object> Tags { get; } = new();
    public DateTimeOffset Timestamp { get; private set; }

    public void Reset()
    {
        Tags.Clear();
        Timestamp = default;
    }
}

// 增强的串口池策略
internal sealed class SerialPortPooledPolicy : IPooledObjectPolicy<SerialPort>
{
    public SerialPort Create()
    {
        var port = new SerialPort
        {
            BaudRate = 9600,
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = 8,
            Handshake = Handshake.None,
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        return port;
    }

    public bool Return(SerialPort obj)
    {
        if (obj.IsOpen)
        {
            obj.Close();
        }
        return true;
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<IotGatewayService>();
builder.Services.AddSingleton<ObjectPool<SerialPort>>(sp => 
    new DefaultObjectPool<SerialPort>(
        new SerialPortPooledPolicy(), 
        Environment.ProcessorCount * 2));

var app = builder.Build();
app.MapGet("/", () => "IoT Gateway Service");
app.Run();


// 新增数据结构
[MemoryPackable]
public partial record DeviceNotification
{
    public required string DeviceId { get; init; }
    public NotificationType Type { get; init; }
    public required byte[] Data { get; init; }
}

public enum NotificationType
{
    StatusChanged,
    DataAlert,
    ConfigUpdated
}

[MemoryPackable]
public partial record DeviceConfigVersion
{
    public Guid VersionId { get; init; }
    public DateTime Timestamp { get; init; }
    public byte[] ConfigData { get; init; } = Array.Empty<byte>();
}

// 数据库上下文
public class IotDbContext : DbContext
{
    public DbSet<DeviceDataEntity> DeviceData { get; set; }
    public DbSet<DeviceConfigVersionEntity> DeviceConfigVersions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=iotgateway.db");
}

public class DeviceDataEntity
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public byte[] Data { get; set; } = null!;
}

public class DeviceConfigVersionEntity
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = null!;
    public Guid VersionId { get; set; }
    public DateTime Timestamp { get; set; }
    public byte[] ConfigData { get; set; } = null!;
}


#region 新增数据协议抽象层
[SkipLocalsInit]
public interface IDeviceModelProtocol
{
    string ProtocolName { get; }
    ValueTask<DeviceModel> ParseModelAsync(ReadOnlyMemory<byte> payload);
    ValueTask<byte[]> BuildRequestAsync(DeviceCommand command);
}

[MemoryPackable]
public partial record DeviceModel
{
    public required string ModelId { get; init; }
    public Dictionary<string, object> Properties { get; init; } = new();
    public Dictionary<string, object> Telemetry { get; init; } = new();
}

[MemoryPackable]
public partial record DeviceCommand
{
    public required string CommandId { get; init; }
    public Dictionary<string, object> Parameters { get; init; } = new();
}

[SkipLocalsInit]
public class DeviceModelPublisher
{
    private readonly Channel<DeviceModel> _modelChannel;
    private readonly ObjectPool<IDeviceModelProtocol> _protocolPool;

    public DeviceModelPublisher(
        ObjectPool<IDeviceModelProtocol> protocolPool,
        BoundedChannelOptions? options = null)
    {
        _protocolPool = protocolPool;
        _modelChannel = Channel.CreateBounded<DeviceModel>(
            options ?? new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask PublishAsync(DeviceModel model)
        => await _modelChannel.Writer.WriteAsync(model);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public IAsyncEnumerable<DeviceModel> SubscribeAsync(CancellationToken ct)
        => _modelChannel.Reader.ReadAllAsync(ct);
}

[SkipLocalsInit]
public class DeviceCommandSubscriber
{
    private readonly Channel<DeviceCommand> _commandChannel;
    private readonly ObjectPool<IDeviceModelProtocol> _protocolPool;

    public DeviceCommandSubscriber(
        ObjectPool<IDeviceModelProtocol> protocolPool,
        BoundedChannelOptions? options = null)
    {
        _protocolPool = protocolPool;
        _commandChannel = Channel.CreateBounded<DeviceCommand>(
            options ?? new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<byte[]> ProcessCommandAsync(DeviceCommand command)
    {
        var protocol = _protocolPool.Get();
        try
        {
            return await protocol.BuildRequestAsync(command);
        }
        finally
        {
            _protocolPool.Return(protocol);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask SubscribeAsync(Func<DeviceCommand, ValueTask> handler, CancellationToken ct)
    {
        await foreach (var cmd in _commandChannel.Reader.ReadAllAsync(ct))
        {
            await handler(cmd);
        }
    }
}
#endregion

// 在IotGatewayService中添加以下字段
private readonly DeviceModelPublisher _modelPublisher;
private readonly DeviceCommandSubscriber _commandSubscriber;

// 在构造函数中添加
_modelPublisher = new DeviceModelPublisher(
    new DefaultObjectPool<IDeviceModelProtocol>(
        new DeviceModelProtocolPooledPolicy(), 
        Environment.ProcessorCount * 2));
        
_commandSubscriber = new DeviceCommandSubscriber(
    new DefaultObjectPool<IDeviceModelProtocol>(
        new DeviceModelProtocolPooledPolicy(),
        Environment.ProcessorCount * 2));

// 新增协议池策略
internal sealed class DeviceModelProtocolPooledPolicy : PooledObjectPolicy<IDeviceModelProtocol>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override IDeviceModelProtocol Create() => new DefaultDeviceModelProtocol();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(IDeviceModelProtocol obj) => true;
}

// 默认协议实现
[SkipLocalsInit]
internal sealed class DefaultDeviceModelProtocol : IDeviceModelProtocol
{
    public string ProtocolName => "Default";

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ValueTask<DeviceModel> ParseModelAsync(ReadOnlyMemory<byte> payload)
    {
        // 实现默认解析逻辑
        // ...
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ValueTask<byte[]> BuildRequestAsync(DeviceCommand command)
    {
        // 实现默认请求构建逻辑
        // ...
    }
}