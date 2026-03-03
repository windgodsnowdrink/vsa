
using System;
//代表一条命令数据
public class CommandData
{
    // 用于标识命令的开始标示
    public string StartInstruction { get; set; }
    // 使用枚举类型来标识不同类型的命令，提高代码可读性和维护性
    public CommandType CommandType { get; set; }
    // 表示命令数据长度
    public int Length { get; set; }
    // 存储实际的命令数据，使用 byte[] 存储，以支持数据格式的灵活性和二进制传输
    public byte[] Data { get; set; }
    // 用于存储校验值
    public string CheckValue { get; set; } // 
    // 用于存储命令执行的结果
    public string Result { get; set; }

    // 发送回复标识
    // 发送时间表示
    // 设备编号标识

    // 枚举类型，用于标识不同的命令类型
    public enum CommandType
    {
        Start = 1,
        GetData = 2,
        SetPara = 3,
        // ...其他命令类型
    }
}

// 用于描述命令数据中的单个指标信息
public class DataModel
{
    public string Name { get; set; } // 指标名称
    public string Unit { get; set; } // 单位 
    public float? MinValue { get; set; } // 最小值
    public float? MaxValue { get; set; } // 最大值
    public DataType ValueType { get; set; } // 数据类型
    public string Description { get; set; } // 说明
    public byte ValueLength { get; set; } // 值长度

    // 指标ID
    // 是否必填
    //默认值

    public enum DataType
    {
        Float,
        Int,
        Bool,
        // ...其他数据类型
    }
}

// 实现一个命令队列，用于管理待发送的命令数据
public class CommandQueue
{
    // 存储待发送的命令数据
    private readonly List<CommandData> _queue = new List<CommandData>();
    // 用于向队列中添加一条命令数据
    public void Enqueue(CommandData commandData)
    {
        lock (_queue)
        {
            _queue.Add(commandData);
        }
    }
    // 从队列的首部取出一条命令数据
    public CommandData Dequeue()
    {
        lock (_queue)//保线程安全，避免并发访问导致的数据不一致
        {
            if (_queue.Count > 0)
            {
                return _queue.First();
            }
            return null;
        }
    }
    // 返回队列中的命令数据数量
    public int Count => _queue.Count;
}
var commandQueue = new CommandQueue();
var commandData = new CommandData(); // ... 初始化 commandData 数据
commandQueue.Enqueue(commandData);
var dequeueCommand = commandQueue.Dequeue();

using Cysharp.R3.MessagePipe;

public class DataGenerator : IAsyncDisposable
{
    // 数据生成逻辑
    public async ValueTask GenerateDataAsync()
    {
        while (true)
        {
            // 模拟生成数据 ...
            var data = /*  Data Generation Logic  */ ;

            // 发送数据到 MessagePipe 通道
            await _pipe.SendAsync(data);

            //  等待一定时间再生成数据
            await Task.Delay(1000);
        }
    }

    // MessagePipe 通道用于发送数据
    private readonly MessagePipe<DataModel> _pipe;

    public DataGenerator()
    {
        _pipe = new MessagePipe<DataModel>();
    }

    public ValueTask DisposeAsync() => _pipe.DisposeAsync(); // 实现异步释放
}

using Cysharp.R3.MessagePipe;

public class DataConsumer : IAsyncDisposable
{
    private readonly CommandDataConverter _converter = new CommandDataConverter(); // 使用数据转换器
    private readonly IServiceProvider _serviceProvider;

    public DataConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask StartAsync()
    {
        await _pipe.ReceiveAsync(HandleData); // 订阅接收数据
    }

    private async ValueTask HandleData(DataModel data)
    {
        // ... 处理数据 ...

        // 使用转换器将数据模型转换为命令数据
        var commandData = _converter.Convert(data);

        // 将命令数据存储到队列...

        // 在这里需要调用外部服务进行处理...

    }

    private readonly MessagePipe<DataModel> _pipe;

    public DataConsumer(MessagePipe<DataModel> pipe)
    {
        _pipe = pipe;
    }

    public ValueTask DisposeAsync() => _pipe.DisposeAsync(); // 实现异步释放

}

public class CommandSender
{
    private readonly IServiceProvider _serviceProvider; // 此处用于注入数据处理组件


    public CommandSender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public async ValueTask SendCommandAsync(CommandData commandData)
    {
        // 使用 固定的串口名称、端口号、波特率等信息，开启连接
        var serialPort = new SerialPort("COM1", 9600);
        serialPort.ReadTimeout = 1000;
        serialPort.Open();

        try
        {
            // 将命令数据发送到串口
            serialPort.WriteLine(commandData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Send command failed: {ex.Message}");
            // 处理异常情况
        }
        finally
        {
            serialPort.Close();
        }
    }
}

using System;

public class CommandData
{
    // ... 其他属性同上 ...

    public string CalculateCheckValue()
    {
        // 采用异或方法计算校验值
        byte xorValue = 0;
        foreach (byte b in this.Data)
        {
            xorValue ^= b;
        }
        return Convert.ToBase64String(xorValue.ToByteArray());
    }

    public bool ValidateCheckValue(string checkValue)
    {
        return CalculateCheckValue() == checkValue;
    }

}

using Cysharp.R3.MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // 引入线程异步操作

public class CommandResponseBuilder
{
    private readonly IServiceProvider _serviceProvider; // 使用 DI 获取服务的实例

    public CommandResponseBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ResponseDTO> BuildResponseAsync(CommandData commandData)
    {
        // 1. 校验命令数据
        if (!commandData.ValidateCheckValue(commandData.CheckValue))
        {
            return new ResponseDTO { Status = "CheckValueFailed" };
        }

        // 2. 处理命令数据，根据内部逻辑生成响应数据
        // 例如: 将命令数据解析到 DataModel，然后获取对应的信息
        DataModel dataModel = /* ...解析数据获取对应信息    */;


        // 3. 创建响应DTO对象
        ResponseDTO responseDTO = new ResponseDTO
        {
            Status = "Success",
            // ... 其他响应数据
            Data = dataModel // 将处理后的数据填充到 DTO
        };

        return responseDTO;
    }

    // 定义 DTO 结构体
    public class ResponseDTO
    {
        public string Status { get; set; }
        public DataModel Data { get; set; }
        // ...其他响应数据属性... 
    }
}

using Cysharp.R3.Pipe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ... 其他库

public class CommandResponseBuilder : IHostedService
{
    private readonly Pipe<CommandData> _commandPipe;
    private readonly IServiceProvider _serviceProvider;

    public CommandResponseBuilder(Pipe<CommandData> commandPipe, IServiceProvider serviceProvider)
    {
        _commandPipe = commandPipe;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _commandPipe.ReceiveAsync(HandleCommand).Forget();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task HandleCommand(CommandData commandData)
    {
        try
        {
            // 1. 加密和校验命令数据 (确保安全传输)
            commandData.Data = EncryptData(commandData.Data);
            commandData.CheckValue = commandData.CalculateCheckValue();

            // 2. 校验命令数据
            if (!commandData.ValidateCheckValue(commandData.CheckValue))
            {
                Console.WriteLine("Command data validation failed!");
                return;
            }
            // 3. 处理命令数据，根据内部逻辑生成响应数据    
            DataModel dataModel = /* ...解析数据获取对应信息    */;

            // 4. 生成响应DTO，并发送到另一个Pipe
            ResponseDTO responseDTO = new ResponseDTO
            {
                Status = "Success",
                Data = dataModel
            };
            responseDTO.Data = EncryptData(responseDTO.Data); // 加密响应数据

            Pipe<ResponseDTO> _responsePipe = _serviceProvider.GetRequiredService<Pipe<ResponseDTO>>(); // 通过 DI 获取响应管道
            await _responsePipe.SendAsync(responseDTO);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing command: {ex.Message}");
        }
    }

    // ... 加密逻辑和 DI 配置 ...
}


using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

// ... 其他代码 ...

public class CommandHandler
{
    private TcpListener server;

    public CommandHandler(int port)
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        ReceiveCommands();
    }

    private void ReceiveCommands()
    {
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

            HandleClient(client);
        }
    }

    private async void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);

        try
        {
            string message = await reader.ReadLineAsync(); // 读取命令数据

            // ... 处理命令数据 ...
            Console.WriteLine($"Received command from client: {message}");

            writer.WriteLine("Command received successfully.");
            await writer.FlushAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

}


using Cysharp.R3.MessagePipe; 

// ... 其他代码

public class CommandHandler
{
    private readonly IMessagePipe<CommandData> _commandPipe;
    private readonly IMessagePipe<SuccessEvent> _successEventPipe;
    private readonly IMessagePipe<ErrorEvent> _errorEventPipe;

    public CommandHandler(IMessagePipe<CommandData> commandPipe,
                        IMessagePipe<SuccessEvent> successEventPipe,
                        IMessagePipe<ErrorEvent> errorEventPipe)
    {
        _commandPipe = commandPipe;
        _successEventPipe = successEventPipe;
        _errorEventPipe = errorEventPipe;
    }

    public async Task StartAsync()
    {
        // 订阅命令
        _commandPipe.ReceiveAsync(async commandData =>
        {
            // ... 业务逻辑校验

            if (commandData.ValidateCheckValue(commandData.CheckValue) == false)
            {
                await _errorEventPipe.SendAsync(new ErrorEvent { Message = "Command validation failed" });
                return;
            }

            // ... 根据 CommandType 执行不同的功能
            await DoCommand(commandData); //
            if (Success)
            {
                await _successEventPipe.SendAsync(new SuccessEvent { Data = commandData.Data });
            }
        }).Forget();
    }

    private async Task DoCommand(CommandData commandData) // ... 你的命令处理逻辑 ...
    {
        // ... 根据 CommandType 执行不同逻辑 
    }

}


public class QueryHandler : IHostedService
{
    private readonly IMessagePipe<SuccessEvent> _successEventPipe;
    private readonly IMessagePipe<ErrorEvent> _errorEventPipe;

    public QueryHandler(IMessagePipe<SuccessEvent> successEventPipe,
                        IMessagePipe<ErrorEvent> errorEventPipe)
    {
        _successEventPipe = successEventPipe;
        _errorEventPipe = errorEventPipe;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _successEventPipe.ReceiveAsync(handleSuccessEvent).Forget();
        _errorEventPipe.ReceiveAsync(handleErrorEvent).Forget();
        return Task.CompletedTask;
    }

    private async Task handleSuccessEvent(SuccessEvent eventData)
    {
        // ... 处理成功事件
    }

    private async Task handleErrorEvent(ErrorEvent errorData)
    {
        // ... 处理错误事件
    }
}

using System;
using StackExchange.Redis;

// ... 其他代码 ...

public class DataStore
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IMemoryCache _cache;

    public DataStore(string redisConnectionString, IMemoryCache cache)
    {
        _redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _cache = cache;
    }

    public async Task StoreData(string key, string value)
    {
        // 使用Redis存储数据
        var db = _redis.GetDatabase();
        await db.StringSetAsync(key, value);

        // 同步缓存数据
        _cache.Set(key, value);
    }

    public async Task<string> GetData(string key)
    {
        // 先从缓存中读取数据
        if (_cache.TryGetValue(key, out var value))
        {
            return value;
        }

        // 从Redis数据库读取数据
        var db = _redis.GetDatabase();
        var redisValue = await db.StringGetAsync(key);

        // 将数据放入缓存
        _cache.Set(key, redisValue);
        return redisValue;
    }

    // ... 其他存储和查询方法
}


public class CommandHandler
{
    // ... 其他代码 ...
    private readonly DataStore _dataStore;

    public CommandHandler(IMessagePipe<CommandData> commandPipe,
                        IMessagePipe<SuccessEvent> successEventPipe,
                        IMessagePipe<ErrorEvent> errorEventPipe,
                        DataStore dataStore)
    {
        // ...  
        _dataStore = dataStore;
    }

    private async Task DoCommand(CommandData commandData)
    {
        // ... 业务逻辑 ...

        // 将解析的结果设置为键值对存入数据库
        await _dataStore.StoreData($"command_{commandData.Id}", commandData.Data);
    }
}

using ZLogger;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Seq;

public class MyLoggerConfig
{
    public static void Configure()
    {
        var seqServerUrl = Environment.GetEnvironmentVariable("SEQ_SERVER_URL") ?? "http://localhost:5341"; // your Seq server url

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Seq(new SeqSinkOptions
            {
                SeqServerUrl = seqServerUrl
            })
            .CreateLogger();

        Log.Logger = logger;

        ZLoggerConfiguration.Instance.UseLogger(logger); // config ZLogger to use Serilog
    }
}



public static class ZLoggerOutputExtensions
{
    public static LogEvent WriteLogEvent(this ZLogger zLogger, string message,
                                      LogLevel logLevel = LogLevel.Information)
    {
        return Serilog.Log.Information(zLogger.TraceMessage, level: logLevel);
    }
}

// ... 其他代码 ...


public class CommandHandler
{
    // ... 其他代码 ...
    public async Task DoCommand(CommandData commandData)
    {


        try
        {
            // ... 业务逻辑 ...
            Log.Information("Command '{CommandId}' executed successfully!", commandData.CommandId); // Write success log in Serilog
            await StoreData();
            // ... 其他操作 ...
        }
        catch (Exception ex)
        {
            // Write error log in Serilog
            Log.Warning(ex, "An error occured while executing command '{CommandId}'!", commandData.CommandId);
            // ... 处理错误逻辑 ...
        }
    }

    // Example Usage
    private async Task StoreData()
    {
        Log.Information("Storing data..."); // Log storing data
        await _dataStore.StoreData(commandData.Key, commandData.Data);  // ... 其他代码 ...
    }
}

public class DataParser
{
    private readonly bool _isBigEndian;

    public DataParser(bool isBigEndian)
    {
        _isBigEndian = isBigEndian;
    }

    public DataStruct Parse(byte[] data)
    {
        // ... 解析数据的逻辑 ...

        // 示例：解析 int32 变量
        int offset = 0;
        int value = 0;
        if (_isBigEndian)
        {
            value = BitConverter.ToInt32(data, offset, Endianness.BigEndian);
        }
        else
        {
            value = BitConverter.ToInt32(data, offset, Endianness.LittleEndian);
        }

        // ... 解析其他数据类型，例如:  uint16, float32, string...

        return new DataStruct
        {
            IntValue = value, // ... 其他字段 ... };
        }
}



    // ... 其他代码 ... 

    public class CommandHandler
    {
        private readonly DataParser _dataParser;

        public CommandHandler(bool isBigEndian)
        {
            _dataParser = new DataParser(isBigEndian);
        }

        public async Task DoCommand(CommandData commandData)
        {
            // ... 
            // 解析数据
            var dataStruct = _dataParser.Parse(commandData.Data);

            // ... 
        }
    }

    public static class ByteUtil
    {
        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string must have even length");
            }

            byte[] result = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                result[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return result;
        }

        public static string ByteArrayToHexString(byte[] byteArray)
        {
            return BitConverter.ToString(byteArray).Replace("-", "");
        }
    }

    //byte数组转换为16进制字符串
    byte[] data = { 0x12, 0x34, 0x56, 0x78 };
    string hex = ByteUtil.ByteArrayToHexString(data);
    Console.WriteLine(hex); // 输出: 12345678

// 16进制字符串转换为byte数组
string hexString = "12345678";
    byte[] convertedData = ByteUtil.HexStringToByteArray(hexString);


using System.Threading.Tasks;
using Serilog; // Use Serilog for logging

public class CommandHandlerManager
{
    private readonly TaskScheduler _taskScheduler = TaskScheduler.Default; // 自定义任务调度程序

    public async Task ProcessCommandAsync(CommandData commandData)
    {
        // Use Task.Run to create a new task 
        await Task.Run(() =>
        {
            try
            {
                var commandHandler = // ...  创建CommandHandler 对象...

                commandHandler.DoCommand(commandData); // Execute command logic here
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(ex, "Error processing command {CommandId}: {Message}",
                    commandData.CommandId, ex.Message);

                // ... 或者处理异常，例如: 记录到数据库，发送邮件通知等...
            }
        }, _taskScheduler);
    }
}
//其他代码


using System;
using System.Collections.Generic;
using System.Linq;
using echarts; 

public class EchartDataGenerator
{
    private readonly TimeSpan _timeSpan;
    private readonly IDataRepository _dataRepository;

    public EchartDataGenerator(TimeSpan timeSpan, IDataRepository dataRepository)
    {
        _timeSpan = timeSpan;
        _dataRepository = dataRepository;
    }

    public EchartsOptions GenerateEchartsData()
    {
        var startTime = DateTime.UtcNow.Subtract(_timeSpan); // 获取时间段开始时间
        var endTime = DateTime.UtcNow; // 获取当前时间


        // 读取数据 
        var dataPoints = _dataRepository.GetData(startTime, endTime);

        // 构建Echarts数据
        var option = new EchartsOptions();
        option.xAxis = new List<ECharts.xAxis>
        {
            new ECharts.xAxis
            {
                type = "time",
                // ... 其他配置
            }
        };

        option.yAxis = new List<ECharts.yAxis>
        {
            new ECharts.yAxis
            {
                type = "value", 
                // ... 其他配置
            }
        };

        option.series = new List<ECharts.Series>
        { // 构建数据系列
            new ECharts.Series
            {
                name = "指标",
                type = "line",
                data = dataPoints.Select(dp => new
                {
                    x = dp.Timestamp, // 将时间戳存入x轴数据
                    y = dp.Value     // 将指标值存入y轴数据 
                }).ToArray()
            }
        };

        return option;
    }
}


using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRDemo
{
    public class DataHub : Hub
    {
        public async Task SendDataPoints(List<DataPoint> dataPoints)
        {
            await Clients.All.SendAsync("ReceiveDataPoints", dataPoints);
        }

        public async Task SendSingleRequest(DateTime startTime, DateTime endTime)
        {
            // 获取数据段内的数据
            var dataPoints = GetRelevantData(startTime, endTime);

            // 广播数据
            await Clients.All.SendAsync("ReceiveDataPoints", dataPoints);
        }

        public async Task StartStream(Action<DataPoint> onNewData)
        {
            // 实现流式推送逻辑， 
            // 此处示例简化，假设有外部事件触发数据更新
            var newPoint = new DataPoint() { Timestamp = DateTime.UtcNow, Value = 42 };
            onNewData(newPoint); // 将新数据调用 onNewData 方法
        }
    }
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/DataHub")
    .build();

connection.on("ReceiveDataPoints", function(data) {
    // 数据处理逻辑
    console.log("Received data:", data);
});

connection.start();


// 在你的 Maui 应用程序中
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls;

public class MainPage : ContentPage
{
    public MainPage()
    {
        WebView webView = new WebView
        {
            Source = "your\_blazor\_app\_entry\_point\_url",
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        Content = webView;
    }
}

// 在你的 Blazor WASM 项目中
// ... 导入必要的 SignalR 
@page "/index" 

<div>
    <h1>My Data Dashboard</h1>
    <div id="echartContainer"></div> 
</div>

@code {
    private HubConnection connection;

protected override async void OnInitialized()
{
    connection = new HubConnectionBuilder()
        .WithUrl("/dataHub")
        .Build();

    connection.On("ReceiveDataPoints", (List<DataPoint> data) =>
    {
        // 将接收到的数据赋予 echarts 数据源
        GenerateEcharts(data); // ... 使用你的逻辑生成 echarts 数据源  
    });

    await connection.StartAsync();
}

private void GenerateEcharts(List<DataPoint> data)
{
    // 使用 echarts 库生成图表
    // 在 echartContainer 元素中渲染图表
}

// 定义 gRPC 服务契约（服务端）

public interface IDataStreamingService
{
    [rpc.Method(nameof(GetDataStream))]
    IObservable<DataPoint> GetDataStream([rpc.Parameter(typeof(Period))] Period period);
}

public class DataStreamingService : IDataStreamingService
{
    private readonly IDataRepository _dataRepository; // 数据源

    public DataStreamingService(IDataRepository dataRepository)
    {
        _dataRepository = dataRepository;
    }
    public IObservable<DataPoint> GetDataStream(Period period)
    {
        return Observable.Interval(TimeSpan.FromMilliseconds(period.Interval))
            .Select(_ => _dataRepository.GetNewDataPoint())
            .Where(_ => _ != null);
    }
}

// 定义 gRPC 服务契约（客户端）
public class DataPoint
{
    public DateTime Timestamp { get; set; }
    public int Value { get; set; }
}

public class Period
{
    public int Interval { get; set; } // 例如: 1000 毫秒 = 1 秒
}

// 服务端端配置
public Startup
{
    public void ConfigureServices(IServiceCollection services)
{
    // ... 其他服务配置

    var builder = services.AddMagicOnion();
    builder.UseEndpoint("http://localhost:5001");
}
} 


// 依赖 MagicOnion JavaScript 库

const MagicOnionClient = require('magic-onion');

const GrpcService = new MagicOnionClient.GrpcService({ framework: 'none', url: 'ws://localhost:5001' });
const service = GrpcService.create(IDataStreamingService);

service.GetDataStream(new Period({ interval: 1000 })).subscribe(data => {
    console.log("Received data:", data);
    // 更新图表
});


// 命令接口
public interface ICommand
{
    void Execute();
}

// 命令具体实现
public class AddCommand : ICommand
{
    private readonly int _value;

    public AddCommand(int value)
    {
        _value = value;
    }

    public void Execute()
    {
        // 应用逻辑: 将 _value 添加到某个值或数据结构中
        Console.WriteLine($"Adding {_value} to the data.");
    }
}

// 命令具体实现
public class SubtractCommand : ICommand
{
    private readonly int _value;

    public SubtractCommand(int value)
    {
        _value = value;
    }

    public void Execute()
    {
        // 应用逻辑: 将 _value 从某个值或数据结构中减去
        Console.WriteLine($"Subtracting {_value} from the data.");
    }
}

// 命令接收者 (负责执行命令)
public class CommandReceiver
{
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
    }
}

// 命令解析程序
public class CommandParser
{
    private readonly CommandReceiver _commandReceiver;

    public CommandParser(CommandReceiver commandReceiver)
    {
        _commandReceiver = commandReceiver;
    }

    public void ParseCommand(string commandString)
    {
        // 解析命令字符串，例如: "ADD 5" or "SUBTRACT 3"
        var parts = commandString.Split(' ');

        if (parts.Length == 2 && parts[0] == "ADD")
        {
            int value = int.Parse(parts[1]);
            ICommand command = new AddCommand(value);
            _commandReceiver.ExecuteCommand(command);
        }
        else if (parts.Length == 2 && parts[0] == "SUBTRACT")
        {
            int value = int.Parse(parts[1]);
            ICommand command = new SubtractCommand(value);
            _commandReceiver.ExecuteCommand(command);
        }
        else
        {
            Console.WriteLine("Invalid command format.");
        }
    }
}

// 使用
public class Program
{
    public static void Main(string[] args)
    {
        CommandReceiver receiver = new CommandReceiver();
        CommandParser parser = new CommandParser(receiver);

        parser.ParseCommand("ADD 5");
        parser.ParseCommand("SUBTRACT 3");
    }
}


// 命令
public class CreateItemCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// 命令处理器 - 处理命令并更新数据
public class ItemCommandHandler
{
    private readonly IItemRepository _itemRepository; // 数据存储

    public ItemCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public void Handle(CreateItemCommand command)
    {
        var newItem = new Item
        {
            Name = command.Name,
            Price = command.Price
        };
        _itemRepository.Add(newItem); // 将新项添加到数据源
    }
}

// 查询 - 获取特定数据
public interface IItemQuery
{
    Task<Item> GetItemByIdAsync(int id); // 示例
}

// 查询处理器
public class ItemQueryHandler : IItemQuery
{
    private readonly IItemRepository _itemRepository;

    public ItemQueryHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Item> GetItemByIdAsync(int id)
    {
        return await _itemRepository.GetByIdAsync(id);
    }
}


// 接口定义
public interface IEmailNotifier
{
    void SendEmail(string to, string subject, string body);
}

// 策略实现 - 使用SMTP
public class SmtpEmailNotifier : IEmailNotifier
{
    public void SendEmail(string to, string subject, string body)
    {
        // 发送邮件逻辑 (可以使用第三方库，例如 Mail Kit 或 SendGrid)
        Console.WriteLine($"Sending email to {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Body: {body}");
    }
}

// 策略实现 - 使用第三方邮件服务
public class ThirdPartyEmailNotifier : IEmailNotifier
{
    public void SendEmail(string to, string subject, string body)
    {
        // 发送邮件逻辑 (使用第三方邮件服务API)
        Console.WriteLine($"Sending email via third party service to {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Body: {body}");
    }
}

// 策略工厂类
public class EmailNotifierFactory
{
    public IEmailNotifier GetNotifier(string type)
    {
        switch (type)
        {
            case "SMTP":
                return new SmtpEmailNotifier();
            case "ThirdParty":
                return new ThirdPartyEmailNotifier();
            default:
                throw new ArgumentException("Invalid email notifier type.");
        }
    }
}

// 使用策略模式
public class OrderService
{
    private readonly IEmailNotifier _emailNotifier;

    public OrderService(EmailNotifierFactory emailNotifierFactory)
    {
        _emailNotifier = emailNotifierFactory.GetNotifier("ThirdParty"); // 使用第三方的邮件服务
    }

    public void PlaceOrder(Order order)
    {
        // 处理订单逻辑

        _emailNotifier.SendEmail("customer@example.com", "Order Confirmed", $"Your order {order.Id} has been placed.");
    }
} 


using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Seq;
using Serilog.Sinks.File; // 用于本地 JSON 文件存储
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

// Serilog 日志配置 -  SeSEQ 远程存储
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog(dispose: true,
        options => options
            .WriteTo.Seq("http://your-seq-server:5341"))
});

// Serilog 日志配置 -  本地JSON文件存储
// 设置本地文件存储路径
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "application.log");

// 配置 Serilog 日志输出到本地 JSON 文件
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
       .ReadFrom.Configuration(context.Configuration)
       .WriteTo.File(logFilePath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}"
       )
});


// ZLogger 日志配置
builder.Services.AddZlogger(config =>
{
    config.LogPath = logFilePath;
});

var app = builder.Build();

// ... 其它应用程序代码 ...

// 示例日志输出
app.Logger.LogInformation("应用程序启动成功");
ZLogger.Debug("这是一个来自ZLogger的调试日志");

app.Run();

using EasyCaching.Core;
using EasyCaching.Core.Internal.Cache;
using EasyCaching.Core.Local;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEasyCaching(options =>
        {
            // 在此配置本地内存缓存
            options.UseLocalRedis(lrd =>
            {
                lrd.CacheProviderName = "localMemCache";
            });

            // 设置缓存名前缀 (可选) 
            options.CacheNamePrefix = "MyCache_";
        });

        // ... 其他服务注册

    }
}

public class ProductController : ControllerBase
{
    private readonly ICache _cache;

    public ProductController(ICache cache)
    {
        _cache = cache;
    }

    [HttpGet("products/{productId}")]
    public async Task<IActionResult> GetProduct(int productId)
    {
        var cachedProduct = _cache.Get<Product>(productId.ToString());

        if (cachedProduct != null)
        {
            return Ok(cachedProduct);
        }

        // 从数据库或其他数据源获取产品数据
        var product = await GetProductFromSourceAsync(productId);

        // 将产品数据缓存起来
        _cache.Set(productId.ToString(), product, TimeSpan.FromMinutes(5)); // 设置缓存过期时间为 5 分钟

        return Ok(product);
    }

    private async Task<Product> GetProductFromSourceAsync(int productId)
    {
        // ... 数据访问逻辑 ...
    }
}


using Microsoft.AspNetCore.SignalR;
// ... 其他引用
using EasyCaching.Serialization.MessagePack;
using Microsoft.AspNetCore.SignalR.Protocols.MessagePack;

public class ChartHub : Hub
{
    public async Task SendChartData(ChartData data)
    {
        await Clients.All.SendAsync("ReceiveChartData", data);
    }
}


@page "/chart"

<h1>图表</h1>

<div id="chartContainer" style="width: 600px; height: 400px;"></div>

@code {
    private Chart _chart;
private ChartData _data;

HubConnection hubConnection;

protected override async Task OnInitializedAsync()
{
    hubConnection = new HubConnectionBuilder()
        .WithUrl(NavigationManager.ToAbsoluteUri("/chartHub"))
        .AddMessagePackProtocol() // 指定使用 MessagePack 协议

        .Build();

    hubConnection.On<ChartData>("ReceiveChartData", (chartData) =>
    {
        _data = chartData;
        InvokeAsync(() => UpdateChart());
    });

    await hubConnection.StartAsync();
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // 在页面渲染完成后初始化 Chart
        await LoadChartData();
    }
}

private async Task LoadChartData()
{
    // 获取数据 (模拟数据)

    _data = new ChartData
    {
        // ... 数据结构 ...
    };

    UpdateChart();
}

private void UpdateChart()
{
    // ... 使用 _data 创建 Chart 对象 ...

}
} 

public class ChartData
{
    // ... 属性定义 ...
}

using MessagePack;
using Microsoft.Extensions.Logging;

public class PersonSerializer
{
    private readonly ILogger<PersonSerializer> _logger;

    public PersonSerializer(ILogger<PersonSerializer> logger)
    {
        _logger = logger;
    }

    public byte[] Serialize(Person person)
    {
        _logger.LogInformation($"Serializing person: {person}");

        return MessagePackSerializer.Serialize(person);
    }

    public Person Deserialize(byte[] data)
    {
        _logger.LogInformation($"Deserializing data: {data.Length}");

        return MessagePackSerializer.Deserialize<Person>(data);
    }
}


public class ExampleHelper
{
    public static void Main(string[] args)
    {
        var person = new Person { Name = "John Doe", Age = 30, City = "New York" };

        var serializer = new PersonSerializer(LoggerFactory.Create(builder => builder.AddConsole()));

        // 序列化
        var serializedData = serializer.Serialize(person);

        // 反序列化
        var deserializedPerson = serializer.Deserialize(serializedData);

        Console.WriteLine($"Deserialized Person: {deserializedPerson.Name} {deserializedPerson.Age} {deserializedPerson.City}");
    }
}

using MessagePack;
using MessagePack.Resolvers;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
}

public class MessagePackExample
{
    public static void Main(string[] args)
    {
        var person = new Person { Name = "John Doe", Age = 30, City = "New York" };

        // 序列化
        byte[] serialized = MessagePackSerializer.Serialize(person, StandardResolver.Default);

        // 输出序列化后的数据 (二进制形式)
        Console.WriteLine("Serialized Data: ");
        foreach (byte b in serialized)
        {
            Console.Write($" {b:X2}"); // 使用十六进制输出
        }
        Console.WriteLine();

        // 反序列化
        var deserializedPerson = MessagePackSerializer.Deserialize<Person>(serialized, StandardResolver.Default);

        // 打印反序列化后的对象
        Console.WriteLine($"Deserialized Person: {deserializedPerson.Name}, {deserializedPerson.Age}, {deserializedPerson.City}");

        Console.ReadKey();
    }
}


public class UserValidator : AbstractValidator<CreateUserCommand>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");
    }
}


using MediatR;

public class MyService : IService
{
    private readonly IMediator _mediator;

    public MyService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task CreateUser(CreateUserCommand command)
    {
        await _mediator.Send(command);
    }
}


public record CreateUserCommand(string FirstName, string LastName, string Email, Ulid Id)
{
    // ... 其他属性 ...
}

public record GetUserQuery(Guid UserId, Ulid CorrelationId)
{
    // ... 其他属性 ...
}


public class CommandHandlerDecorator<TCommand> : IRequestHandler<TCommand, Ulid>
    where TCommand : IRequest<Ulid>

{
    private readonly IRequestHandler<TCommand, Unit> _handler;
    private readonly UlidGenerator _ulidGenerator;

    public CommandHandlerDecorator(IRequestHandler<TCommand, Unit> handler, UlidGenerator ulidGenerator)
    {
        _handler = handler;
        _ulidGenerator = ulidGenerator;
    }

    public async Task<Ulid> Handle(TCommand request, CancellationToken cancellationToken)
    {
        // 添加一个 Ulid 到你的命令，如果它没有
        // 然后调用 IRequestHandler
        request.Id = _ulidGenerator.GenerateNewULID();

        await _handler.Handle(request, cancellationToken);

        // 返回处理结果的 Ulid
        return request.Id;
    }
}

services.AddMediatR(typeof(MyCommandHandler).Assembly);
services.AddScoped<IRequestHandler<TCommand, Unit>, CommandHandlerDecorator<TCommand>>();

public class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig.GlobalSettings.Scan(s =>
        {
            s.ScanFromAssemblyContaining<UserDTO>();  //  查找 UserDTO 所在的程序集 获取映射关系
            s.CompileAdapters(options =>
            {
                options.AddProfile<UserMappingProfile>(); // 追加你的 custom 映射规则
            });
        });

    }
}


public class UserMappingProfile : ITypeAdapterConfig
{
    public void Configure()
    {
        TypeAdapter.Adapt<User, UserDTO>() // 用户到 DTO 映射

            .Map(dest => dest.FirstName, source => source.FirstName)
            .Map(dest => dest.LastName, source => source.LastName)
            .Map(dest => dest.Email, source => source.Email);

        TypeAdapter.Adapt<UserDTO, User>() // DTO 到用户映射
            .Map(dest => dest.Id, source => 1) // 设置 ID 
            .Map(dest => dest.FirstName, source => source.FirstName)
            .Map(dest => dest.LastName, source => source.LastName)
            .Map(dest => dest.Email, source => source.Email);
    }
}

var userDTO = user.Adapt<UserDTO>();


public class CreateUserCommand : IRequest<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

public class GetUserQuery : IRequest<User>
{
    public Guid UserId { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}


public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository; // 你的数据库操作接口

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        _userRepository.Add(user);
        // ... 保存到数据库 ...

        return user.Id;
    }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(request.UserId);
    }
}

public class DependencyInjectionConfig : IStartup
{
    public void ConfigureServices(ServiceCollection services)
    {
        services.AddMediatR(typeof(CreateUserCommandHandler).Assembly);


        services.AddScoped<IUserRepository, UserRepository>();  // 注册你的仓储接口实现
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<CreateUserCommandHandler>();
        services.AddScoped<GetUserQueryHandler>();

    }
}


public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { userId }, userId);
    }


    // ... 其他控制器方法 ...
}


public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("InMemoryDb");
    }
}


public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
    Task<User> CreateAsync(User user);
}


public class UserInMemoryRepository : IUserRepository
{
    private readonly UserDbContext _userDbContext;

    public UserInMemoryRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<User> CreateAsync(User user)
    {
        _userDbContext.Users.Add(user);
        await _userDbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _userDbContext.Users.FindAsync(userId);
    }
}

public class DependencyInjectionConfig : IStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDb");
        });
        services.AddScoped<IUserRepository, UserInMemoryRepository>();
        // ... 其他注册
    }
}

dotnet add package Microsoft.Extensions.Caching.Memory 

public class ProductMemoryCache
{
    private readonly IMemoryCache _memoryCache;

    public ProductMemoryCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void AddProduct(Product product)
    {
        _memoryCache.Set(product.Id.ToString(), product, TimeSpan.FromHours(1));
    }

    public Product GetProductById(Guid productId)
    {
        return _memoryCache.TryGetValue(productId.ToString(), out Product product) ? product : null;
    }
}


public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ProductMemoryCache>();
        services.AddMemoryCache();
    }
}

public class ProductController : ControllerBase
{
    private readonly ProductMemoryCache _productCache;

    public ProductController(ProductMemoryCache productCache)
    {
        _productCache = productCache;
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(Guid id)
    {
        var product = _productCache.GetProductById(id);

        if (product == null)
        {
            return NotFound(); // 如果产品不存在
        }

        return Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] Product product)
    {
        _productCache.AddProduct(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}  


using System.Security.Cryptography;
using System.Text;

public class AesEncryption
{
    private readonly string _key = "YourStrongSecretKeyHere"; // 替换成您的真实秘钥 

    public string Encrypt(string data)
    {
        byte[] key = UTF8Encoding.UTF8.GetBytes(_key);
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = aes.Key;
            using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                        cs.Close();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    public string Decrypt(string encryptedData)
    {
        byte[] key = UTF8Encoding.UTF8.GetBytes(_key);
        byte[] dataToDecrypt = Convert.FromBase64String(encryptedData);
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = aes.Key;
            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                using (MemoryStream ms = new MemoryStream(dataToDecrypt))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] result = new byte[1024];
                        StringBuilder sb = new StringBuilder();
                        int len;
                        while ((len = cs.Read(result, 0, result.Length)) > 0)
                        {
                            sb.Append(Encoding.UTF8.GetString(result, 0, len));
                        }
                        return sb.ToString();
                    }
                }
            }
        }
    }
} 


using System.Security.Cryptography;

public class Sha256Hasher
{
    public static string CalculateHash(string data)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = sha256Hash.ComputeHash(bytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}


var aes = new AesEncryption();

string encryptString = aes.Encrypt("Secret Information");
string decryptString = aes.Decrypt(encryptString); // 解密

string textToSend = "Hello, Serial Port!";
string hash = Sha256Hasher.CalculateHash(textToSend);

using Serilog;
using Serilog.Formatting.CompactJson;
using Serilog.Events;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                new SpectreConsoleFormatter(),
                outputTemplate: "[bold green]##{Timestamp} {Level:u4}:[/] {Message}"
            )
            .CreateLogger();

        // 在您的应用程序中记录日志信息
        Log.Information("Hello, world!");
    }
}

outputTemplate: "[bold green]##{Timestamp:dd-MM-yyyy HH:mm:ss.fff} {Level:u4} [{gray background:yellow}] {Source}:[/] {Message}"


using AutoBogus;
using Bogus;

 public class DeviceCommandFaker
{
    private readonly Faker _faker = new Faker();

    public List<DeviceCommand> GenerateDeviceCommands(int count)
    {
        return Enumerable.Range(0, count)
                     .Select(i =>
                    new DeviceCommandBuilder()
                        .With(c => c.CommandId = Guid.NewGuid())
                        .With(c => c.DeviceName = _faker.Commerce.ProductName())
                        .With(c => c.CommandType = _faker.Random.Enum<CommandType>())
                        .With(c => c.Data = _faker.Internet.Url())
                        .With(c => c.Timestamp = _faker.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now))
                        .Build())
                     .ToList();
    }



    public enum CommandType
    {
        Read,
        Write,
        Control
    }
}


var deviceCommandFaker = new DeviceCommandFaker();
var commands = deviceCommandFaker.GenerateDeviceCommands(10);

// 现在您可以使用 `commands` 列表模拟设备命令数据;

// 设备命令模型
public enum CommandType
{
    Read = 1,
    Write = 2,
    Control = 3,
    // 添加其他命令类型
}

using Cysharp.R3;
using Cysharp.R3.Ui; 

public class MyViewModel : IStateful, ICanReactiveLife
{
    private string _message = "Hello from ViewModel!";
    public string Message { get => _message; set => _message = value; }

    public IReactiveDisposables Disposal { get; set; }

    public void OnInit()
    {
        // 在 ViewModel 初始化时执行操作
        R3.Logger.Log("MyViewModel 初始化了");
    }

    public void OnDestroy()
    {
        // 在 ViewModel 销毁时执行操作
        R3.Logger.Log("MyViewModel 销毁了");
    }
    public void DoSomething()
    {
        Message = "Something Happened!"; // 模拟数据更新操作
    }
}


using Cysharp.R3;
using Cysharp.R3.Ui;

public class MyView : BindableClass<MyViewModel>
{

    [Bind] public string Message { get; set; }

    public void OnButtonPressed()
    {
        ViewModel.DoSomething();
    }

    private void OnValidate()
    { // 使用R3进行自动数据绑定
        // ViewModel 的数据变化会自动更新到 View 中 
    }
}


using Cysharp.R3;

public class App
{
    public void Run()
    {
        StartWindow = R3.Window(new MyView()); // 显示页面
    }
    public Window StartWindow { get; set; }
}



public class MyMessage
{
    public string Payload { get; set; }
}


public class MyService : IDisposable
{
    private readonly MessagePipe<MyMessage> _messagePipe = new MessagePipe<MyMessage>();

    public void SendMessage(string payload)
    {
        _messagePipe.Send(new MyMessage { Payload = payload });
    }

    public void ReceiveMessage(Action<MyMessage> messageHandler)
    {
        _messagePipe.Receive(messageHandler);
    }

    public void Dispose()
    {
        _messagePipe.Dispose();
    }
}


public class MyViewModel : IStateful
{
    private readonly MyService _messageService;

    public MyViewModel(MyService messageService)
    {
        _messageService = messageService;
        _messageService.ReceiveMessage(OnMessageReceived);
    }

    private void OnMessageReceived(MyMessage message)
    {
        // 处理接收到的消息
        Console.WriteLine($"Received message: {message.Payload}");
    }
}


public class AnotherViewModel : IStateful
{
    private readonly MyService _messageService;

    public AnotherViewModel(MyService messageService)
    {
        _messageService = messageService;
    }

    public void SendData()
    {
        _messageService.SendMessage("Hello from another ViewModel!");
    }
}


// 定义 gRPC 的服务接口
public interface IWebSocketService
{
    [ServerStreaming]
    IAsyncEnumerable<string> SubscribeMessages();
}

// 定义 gRPC  服务实现
public class WebSocketServiceImpl : IWebSocketService
{
    public async IAsyncEnumerable<string> SubscribeMessages()
    {
        // 获取WebSocket 连接 
        // 这里需要根据您具体的 WebSocket 桥接方案来获取 WebSocket 连接。 
        var webSocketConnnection = GetWebSocketConnection(); // 

        // 从 WebSocket 连接读取数据
        // 此处需要根据您使用的 WebSocket 库来实现读取数据的逻辑。 
        await foreach (var message in webSocketConnnection.Receive())
        {
            yield return message;
        }
    }
}


// Create a gRPC channel and create a proxy to the Grpc service
var channel = GrpcChannel.ForAddress("本地服务地址");
var service = new Grpc.Net.Client.WebSocketService.WebSocketServiceClient(channel);


// 使用 grpc-web 发送消息
var messageRequest = new MessageRequest() { Payload = "Hello" };
var response = service.SendMessage(messageRequest);


using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

public class TcpConnectionManager
{
    private readonly ConcurrentDictionary<Guid, TcpConnection> _connections = new ConcurrentDictionary<Guid, TcpConnection>();
    // 可以根据需要调整连接池的大小
    private const int MaxConnections = 10;

    public TcpConnection GetConnection(string serverAddress, int port)
    {
        // 使用 Guid 生成唯一标识符
        var connectionId = Guid.NewGuid();

        // 从连接池获取连接，如果满了则创建一个新的连接
        lock (_connections)
        {
            if (_connections.Count >= MaxConnections)
                return null;

            if (!_connections.TryGetValue(connectionId, out var connection))
            {
                connection = new TcpConnection(serverAddress, port, connectionId);
                _connections.TryAdd(connectionId, connection);
            }
            return connection;
        }
    }

    public void ReleaseConnection(Guid connectionId)
    {
        // 释放连接
        _connections.TryRemove(connectionId, out _);
    }

    // 进程退出时清理连接池
    public void Dispose()
    {
        foreach (var connection in _connections.Values)
        {
            connection.Dispose();
        }
    }
}

public class TcpConnection : IDisposable
{
    private readonly TcpClient _tcpClient;
    public Guid ConnectionId { get; }

    public TcpConnection(string serverAddress, int port, Guid connectionId)
    {
        ConnectionId = connectionId;
        _tcpClient = new TcpClient(serverAddress, port);
    }

    public void Dispose()
    {
        _tcpClient.Close();
    }
}


// 混淆算法
string generateConfuse(string token, long timestamp)
{
    string confuseStr = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{token}{timestamp}"));
    confuseStr = confuseStr[5..];
    return confuseStr[..^5];
}

// 加密
string EncryptWithRSA(string publicKey, string content)
{
    using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

    return BytesToHexString(rsa.Encrypt(Encoding.UTF8.GetBytes(content), false));
}

#pragma warning disable CS8321 // 已声明本地函数，但从未使用过
// 解密
string DecryptWithRSA(string privateKey, string content)
{
    byte[] encryptedData = HexStringToBytes(content);
    using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
    rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);

    return Encoding.UTF8.GetString(rsa.Decrypt(encryptedData, false));
}
#pragma warning restore CS8321 // 已声明本地函数，但从未使用过

// 字节转小写十六进制
string BytesToHexString(byte[] bytes)
{
    StringBuilder hexString = new StringBuilder();
    foreach (byte b in bytes)
    {
        hexString.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0:x2}", b);
    }

    return hexString.ToString();
}

// 十六进制转字节
byte[] HexStringToBytes(string hexString)
{
    ArgumentNullException.ThrowIfNull(hexString);

    if (hexString.Length % 2 != 0)
    {
        throw new ArgumentException("二进制密钥不能有奇数位数");
    }

    byte[] data = new byte[hexString.Length / 2];
    for (int i = 0; i < data.Length; i++)
    {
        string hex = hexString.Substring(i * 2, 2);
        data[i] = Convert.ToByte(hex, 16);
    }

    return data;
}

// 获取公钥和私钥
(string publicKey, string privateKey) GetKeyPair()
{
    using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
    byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
    byte[] privateKeyBytes = rsa.ExportPkcs8PrivateKey();
    string npuk = Convert.ToBase64String(publicKeyBytes);
    string nprk = Convert.ToBase64String(privateKeyBytes);
    return (npuk, nprk);
}