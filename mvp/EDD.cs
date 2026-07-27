#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package System.Threading.Channels@9.0.9
#:package Microsoft.Extensions.ObjectPool@9.0.9
#:package Scalar.AspNetCore@2.8.0
#:package System.Linq.Async@6.0.3
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(); // /scalar

var app = builder.Build();

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowFrontend");
    app.MapScalarApiReference();
}

//await foreach (var n in FibonacciAsync(10))
//    Console.Write("{0} ", n);

IAsyncEnumerable<int> query =
from i in FibonacciAsync(10)
where i % 2 == 0
select i * 10;
await foreach (var i in query)
    Console.Write("{0} ", n);

app.MapGet("", (CancellationToken ct) =>
{
    using var dbContext = new BookContext();
    await foreach (var title in dbContext.Books
            .Select(b => b.Title)
            .AsAsyncEnumerable())
        yield return title;
});

app.Run();

async IAsyncEnumerable<int> FibonacciAsync(int count)
{
    int prev = 1;
    int curr = 1;
    Random r = new();
    for (int i = 0; i < count; i++)
    {
        yield return prev;
        await Task.Delay(1000);
        int temp = prev + curr;
        prev = curr;
        curr = temp;
    }
}

/// <summary>
/// �¼������ӿ� - �����¼��ĸ��ӿ�
/// </summary>
public interface IEvent
{
    DateTime Timestamp { get; }  // �¼�����ʱ��
    string EventId { get; }      // Ψһ�¼���ʶ
}

/// <summary>
/// �¼����������� - �ṩͨ��ʵ��
/// </summary>
public abstract class BaseEvent : IEvent
{
    public DateTime Timestamp { get; }
    public string EventId { get; }

    protected BaseEvent()
    {
        Timestamp = DateTime.Now;           // �Զ���¼ʱ���
        EventId = Guid.NewGuid().ToString(); // ����ΨһID
    }
}

public interface IEventBus
{
    void Subscribe<T>(Action<T> handler) where T : IEvent;    // �����¼�
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;  // ȡ������
    void Publish<T>(T eventItem) where T : IEvent;           // �����¼�
    void Clear();                                             // ������ж���
}

/// <summary>
/// �̰߳�ȫ���¼�����ʵ��
/// </summary>
public class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<object>> _handlers;
    private readonly object _lock = new object();

    public EventBus()
    {
        _handlers = new ConcurrentDictionary<Type, List<object>>();
    }

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        lock (_lock)
        {
            var eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<object>();
            }
            _handlers[eventType].Add(handler);
        }
    }

    public void Publish<T>(T eventItem) where T : IEvent
    {
        var eventType = typeof(T);
        if (_handlers.ContainsKey(eventType))
        {
            var handlers = _handlers[eventType].ToList();

            // �ؼ���ȷ����UI�߳���ִ�У�������߳��쳣
            if (Application.OpenForms.Count > 0)
            {
                var mainForm = Application.OpenForms[0];
                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new Action(() => ExecuteHandlers(handlers, eventItem)));
                }
                else
                {
                    ExecuteHandlers(handlers, eventItem);
                }
            }
        }
    }

    private void ExecuteHandlers<T>(List<object> handlers, T eventItem) where T : IEvent
    {
        foreach (var handler in handlers)
        {
            try
            {
                ((Action<T>)handler)(eventItem);
            }
            catch (Exception ex)
            {
                // ��Ҫ���쳣���룬һ���������쳣��Ӱ������������
                System.Diagnostics.Debug.WriteLine($"�¼�������ִ���쳣: {ex.Message}");
            }
        }
    }
}

/// <summary>
/// ͳһ�豸�ӿ� - ����IoT�豸�ı�׼
/// </summary>
public interface IDevice : IDisposable
{
    string DeviceId { get; }      // �豸Ψһ��ʶ
    string DeviceName { get; }    // �豸����
    bool IsConnected { get; }     // ����״̬
    bool IsRunning { get; }       // ����״̬

    void Initialize(IEventBus eventBus);  // ��ʼ���¼�����
    void Start();        // �����豸
    void Stop();         // ֹͣ�豸
    void Connect();      // �����豸
    void Disconnect();   // �Ͽ��豸
}

/// <summary>
/// �¶ȴ����� - �¼��������豸ʵ�ֵ䷶
/// </summary>
public class TemperatureSensor : IDevice
{
    private readonly Random _random;
    private IEventBus _eventBus;
    private CancellationTokenSource _cancellationTokenSource;
    private Task _dataGenerationTask;

    publics tring DeviceId { get; }
    public string DeviceName { get; }
    public bool IsConnected { get; privateset; }
    public bool IsRunning { get; privateset; }

    public TemperatureSensor(string deviceId, string deviceName)
{
    DeviceId = deviceId;
    DeviceName = deviceName;
    _random = new Random();
}

public void Initialize(IEventBus eventBus)
{
    _eventBus = eventBus ?? thrownew ArgumentNullException(nameof(eventBus));
}

public void Start()
{
    if (!IsConnected) Connect();

    if (!IsRunning)
    {
        IsRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _dataGenerationTask = Task.Run(GenerateData, _cancellationTokenSource.Token);

        // ����ϵͳ��־�¼�
        _eventBus?.Publish(new SystemLogEvent($"�¶ȴ����� {DeviceName} ��ʼ�ɼ�����", LogLevel.Info));
    }
}

/// <summary>
/// ���ķ������첽�������ɣ����������¼�
/// </summary>
private async Task GenerateData()
{
    double currentTemp = 25.0; // �����¶�

    while (!_cancellationTokenSource.Token.IsCancellationRequested)
    {
        try
        {
            // ģ����ʵ���¶Ȳ���
            currentTemp += (_random.NextDouble() - 0.5) * 2.0;
            currentTemp = Math.Max(-10, Math.Min(50, currentTemp));

            // �������ݸ����¼� - �����������¼������ĺ���
            _eventBus?.Publish(new DeviceDataUpdatedEvent(
                DeviceId, DeviceName, Math.Round(currentTemp, 1), "��C", "Temperature"));

            // ���ܱ����߼�
            if (currentTemp > 35)
            {
                _eventBus?.Publish(new DeviceAlarmEvent(
                    DeviceId, DeviceName, $"�¶ȹ���: {currentTemp:F1}��C", AlarmLevel.Warning));
            }

            await Task.Delay(2000, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            break; // ����ȡ��
        }
        catch (Exception ex)
        {
            // �쳣����Ҳͨ���¼�����
            _eventBus?.Publish(new SystemLogEvent($"�¶ȴ��������������쳣: {ex.Message}", LogLevel.Error));
            await Task.Delay(5000, _cancellationTokenSource.Token);
        }
    }
}
}

/// <summary>
/// �豸���ݸ����¼� - ��Ƶ�����¼�����
/// </summary>
public class DeviceDataUpdatedEvent : BaseEvent
{
    public string DeviceId { get; }
    public string DeviceName { get; }
    public double Value { get; }
    public string Unit { get; }
    public string DataType { get; }

    public DeviceDataUpdatedEvent(string deviceId, string deviceName, double value, string unit, string dataType)
    {
        DeviceId = deviceId;
        DeviceName = deviceName;
        Value = value;
        Unit = unit;
        DataType = dataType;
    }
}

/// <summary>
/// �豸�����¼� - �ؼ�ҵ���¼�
/// </summary>
public class DeviceAlarmEvent : BaseEvent
{
    public string DeviceId { get; }
    public string DeviceName { get; }
    public string AlarmMessage { get; }
    public AlarmLevel Level { get; }

    public DeviceAlarmEvent(string deviceId, string deviceName, string alarmMessage, AlarmLevel level)
    {
        DeviceId = deviceId;
        DeviceName = deviceName;
        AlarmMessage = alarmMessage;
        Level = level;
    }
}

public enum AlarmLevel
{
    Info,      // ��Ϣ
    Warning,   // ����
    Error,     // ����
    Critical   // ����
}

public partial class FrmMain : Form
{
    private readonly IEventBus _eventBus;
    private readonly DeviceManager _deviceManager;
    private readonly Dictionary<string, double> _latestDeviceData;

    public FrmMain()
    {
        InitializeComponent();
        _eventBus = new Events.EventBus();
        _deviceManager = new DeviceManager(_eventBus);
        _latestDeviceData = new Dictionary<string, double>();

        SubscribeToEvents();  // �������и���Ȥ���¼�
        InitializeDevices();  // ��ʼ���豸
    }

    /// <summary>
    /// �¼����� - ϵͳ��Ӧ�����ĺ���
    /// </summary>
    private void SubscribeToEvents()
    {
        _eventBus.Subscribe<DeviceDataUpdatedEvent>(OnDeviceDataUpdated);
        _eventBus.Subscribe<DeviceConnectionChangedEvent>(OnDeviceConnectionChanged);
        _eventBus.Subscribe<DeviceAlarmEvent>(OnDeviceAlarm);
        _eventBus.Subscribe<SystemLogEvent>(OnSystemLog);
    }

    /// <summary>
    /// �����豸���ݸ��� - ʵʱ��Ӧ���ݱ仯
    /// </summary>
    private void OnDeviceDataUpdated(DeviceDataUpdatedEvent eventData)
    {
        _latestDeviceData[eventData.DeviceId] = eventData.Value;
        UpdateDeviceList();  // �Զ����½���

        // ���ӵ�������־�б�
        var logItem = new ListViewItem(eventData.Timestamp.ToString("HH:mm:ss"));
        logItem.SubItems.Add(eventData.DeviceName);
        logItem.SubItems.Add(eventData.DataType);
        logItem.SubItems.Add($"{eventData.Value:F1} {eventData.Unit}");

        dataLogListView.Items.Insert(0, logItem);

        // �����Ż���������ʾ�����������ڴ�й©
        while (dataLogListView.Items.Count > 100)
        {
            dataLogListView.Items.RemoveAt(dataLogListView.Items.Count - 1);
        }
    }

    /// <summary>
    /// �����豸���� - �û�����Ĺؼ�
    /// </summary>
    private void OnDeviceAlarm(DeviceAlarmEvent eventData)
    {
        var logItem = new ListViewItem(eventData.Timestamp.ToString("HH:mm:ss"));
        logItem.SubItems.Add(eventData.DeviceName);
        logItem.SubItems.Add(eventData.Level.ToString());
        logItem.SubItems.Add(eventData.AlarmMessage);

        // ���ݱ������������Ӿ���ʾ
        logItem.BackColor = eventData.Level switch
        {
            AlarmLevel.Warning => Color.Yellow,
            AlarmLevel.Error => Color.Orange,
            AlarmLevel.Critical => Color.Red,
            _ => Color.White
        };

        alarmListView.Items.Insert(0, logItem);

        // ϵͳ������������
        notifyIcon.ShowBalloonTip(3000, "�豸����",
            $"{eventData.DeviceName}: {eventData.AlarmMessage}",
            ToolTipIcon.Warning);
    }
}