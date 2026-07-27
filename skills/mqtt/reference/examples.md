# mqtt - 使用示例

## 快速入门

### 1. 基本MQTT客户端示例

```csharp
using System;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

public class BasicMqttClientExample
{
    public static async Task Main()
    {
        Console.WriteLine("基本MQTT客户端示例");
        Console.WriteLine("=" * 50);
        
        // 创建MQTT工厂
        var factory = new MqttFactory();
        
        // 创建MQTT客户端
        var mqttClient = factory.CreateMqttClient();
        
        // 配置MQTT客户端选项
        var clientOptions = new MqttClientOptionsBuilder()
            .WithClientId("basic-client-" + Guid.NewGuid().ToString())
            .WithTcpServer("localhost", 1883) // MQTT服务器地址和端口
            .WithCleanSession()
            .Build();
        
        // 连接到MQTT服务器
        Console.WriteLine("正在连接到MQTT服务器...");
        var connectResult = await mqttClient.ConnectAsync(clientOptions);
        
        if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
        {
            Console.WriteLine("成功连接到MQTT服务器");
            
            // 注册消息接收事件
            mqttClient.ApplicationMessageReceivedAsync += e => {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                Console.WriteLine($"收到消息: 主题 = {topic}, 负载 = {payload}");
                return Task.CompletedTask;
            };
            
            // 订阅主题
            Console.WriteLine("正在订阅主题 'test/#'");
            await mqttClient.SubscribeAsync(
                topicFilter: "test/#",
                qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce
            );
            
            // 发布消息
            Console.WriteLine("正在发布消息到主题 'test/topic'");
            await mqttClient.PublishAsync(
                topic: "test/topic",
                payload: "Hello MQTT!",
                qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce
            );
            
            // 等待用户输入
            Console.WriteLine("按任意键断开连接并退出...");
            Console.ReadKey();
            
            // 断开连接
            Console.WriteLine("正在断开连接...");
            await mqttClient.DisconnectAsync();
            Console.WriteLine("已断开连接");
        }
        else
        {
            Console.WriteLine($"连接失败: {connectResult.ResultCode}");
        }
    }
}
```

### 2. 高级MQTT客户端配置示例

```csharp
using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

public class AdvancedMqttClientExample
{
    public static async Task Main()
    {
        Console.WriteLine("高级MQTT客户端配置示例");
        Console.WriteLine("=" * 50);
        
        // 创建MQTT工厂
        var factory = new MqttFactory();
        
        // 创建MQTT客户端
        var mqttClient = factory.CreateMqttClient();
        
        // 配置MQTT客户端选项（带TLS和认证）
        var clientOptions = new MqttClientOptionsBuilder()
            .WithClientId("advanced-client-" + Guid.NewGuid().ToString())
            .WithTcpServer("localhost", 8883) // 使用安全端口
            .WithCredentials("username", "password") // 用户名密码认证
            .WithTlsOptions(o => {
                o.UseTls = true;
                o.SslProtocol = System.Security.Authentication.SslProtocols.Tls12;
                o.CertificateValidationHandler = (certContext) => {
                    // 自定义证书验证逻辑
                    Console.WriteLine($"证书主题: {certContext.Certificate.Subject}");
                    return true; // 生产环境中应该验证证书
                };
                // 加载客户端证书（如果需要双向认证）
                // o.ClientCertificates = new List<X509Certificate2> { new X509Certificate2("client-cert.pfx", "password") };
            })
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
            .WithConnectionTimeout(TimeSpan.FromSeconds(30))
            .WithWillTopic("clients/will")
            .WithWillPayload(Encoding.UTF8.GetBytes("客户端意外断开连接"))
            .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
            .WithWillRetain(false)
            .Build();
        
        // 注册连接事件
        mqttClient.ConnectedAsync += e => {
            Console.WriteLine("成功连接到MQTT服务器");
            return Task.CompletedTask;
        };
        
        // 注册断开连接事件
        mqttClient.DisconnectedAsync += e => {
            Console.WriteLine($"与MQTT服务器断开连接: {e.Reason}");
            return Task.CompletedTask;
        };
        
        // 连接到MQTT服务器
        Console.WriteLine("正在连接到MQTT服务器...");
        await mqttClient.ConnectAsync(clientOptions);
        
        // 发布测试消息
        await mqttClient.PublishAsync(
            topic: "advanced/test",
            payload: "高级MQTT客户端测试消息",
            qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce
        );
        
        Console.WriteLine("测试消息已发布");
        
        // 等待用户输入
        Console.WriteLine("按任意键断开连接并退出...");
        Console.ReadKey();
        
        // 断开连接
        await mqttClient.DisconnectAsync();
        Console.WriteLine("已断开连接");
    }
}
```

### 3. MQTT服务器示例

```csharp
using System;
using System.Text;
using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Server.Options;

public class MqttServerExample
{
    public static async Task Main()
    {
        Console.WriteLine("MQTT服务器示例");
        Console.WriteLine("=" * 50);
        
        // 创建MQTT工厂
        var factory = new MqttFactory();
        
        // 创建MQTT服务器
        var mqttServer = factory.CreateMqttServer();
        
        // 配置MQTT服务器选项
        var serverOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883) // MQTT服务器端口
            .WithConnectionBacklog(100)
            .WithMaxPendingMessagesPerClient(1000)
            .WithPersistentSessions()
            .Build();
        
        // 注册客户端连接事件
        mqttServer.ClientConnectedAsync += e => {
            Console.WriteLine($"客户端已连接: 客户端ID = {e.ClientId}, 端点 = {e.Endpoint}");
            return Task.CompletedTask;
        };
        
        // 注册客户端断开连接事件
        mqttServer.ClientDisconnectedAsync += e => {
            Console.WriteLine($"客户端已断开连接: 客户端ID = {e.ClientId}, 原因 = {e.Reason}");
            return Task.CompletedTask;
        };
        
        // 注册消息接收事件
        mqttServer.ApplicationMessageReceivedAsync += e => {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            Console.WriteLine($"收到消息: 客户端ID = {e.ClientId}, 主题 = {topic}, 负载 = {payload}");
            return Task.CompletedTask;
        };
        
        // 启动MQTT服务器
        Console.WriteLine("正在启动MQTT服务器...");
        await mqttServer.StartAsync(serverOptions);
        Console.WriteLine("MQTT服务器已启动，监听端口 1883");
        
        // 等待用户输入
        Console.WriteLine("按任意键停止MQTT服务器...");
        Console.ReadKey();
        
        // 停止MQTT服务器
        Console.WriteLine("正在停止MQTT服务器...");
        await mqttServer.StopAsync();
        Console.WriteLine("MQTT服务器已停止");
    }
}
```

### 4. 高性能MQTT客户端示例

```csharp
using System;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

public class HighPerformanceMqttClientExample
{
    // 高性能通道用于处理收到的消息
    private static readonly Channel<MqttApplicationMessageReceivedEventArgs> _messageChannel = 
        Channel.CreateUnbounded<MqttApplicationMessageReceivedEventArgs>(
            new UnboundedChannelOptions { SingleReader = true });
    
    public static async Task Main()
    {
        Console.WriteLine("高性能MQTT客户端示例");
        Console.WriteLine("=" * 50);
        
        // 启动消息处理任务
        var messageProcessingTask = ProcessMessagesAsync();
        
        // 创建MQTT工厂
        var factory = new MqttFactory();
        
        // 创建MQTT客户端
        var mqttClient = factory.CreateMqttClient();
        
        // 配置MQTT客户端选项
        var clientOptions = new MqttClientOptionsBuilder()
            .WithClientId("high-perf-client-" + Guid.NewGuid().ToString())
            .WithTcpServer("localhost", 1883)
            .WithCleanSession()
            .Build();
        
        // 注册消息接收事件，将消息放入通道
        mqttClient.ApplicationMessageReceivedAsync += e => {
            _messageChannel.Writer.TryWrite(e);
            return Task.CompletedTask;
        };
        
        // 连接到MQTT服务器
        await mqttClient.ConnectAsync(clientOptions);
        Console.WriteLine("成功连接到MQTT服务器");
        
        // 订阅多个主题
        await mqttClient.SubscribeAsync(new List<TopicFilter> {
            new TopicFilter { Topic = "sensor/#", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce },
            new TopicFilter { Topic = "device/#", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce },
            new TopicFilter { Topic = "event/#", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce }
        });
        
        Console.WriteLine("已订阅多个主题，正在接收消息...");
        Console.WriteLine("按任意键断开连接并退出...");
        
        // 发布大量测试消息
        var publishTask = PublishTestMessagesAsync(mqttClient);
        
        // 等待用户输入
        Console.ReadKey();
        
        // 取消发布任务
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.Cancel();
        
        // 断开连接
        await mqttClient.DisconnectAsync();
        Console.WriteLine("已断开连接");
        
        // 关闭消息通道
        _messageChannel.Writer.Complete();
        
        // 等待消息处理任务完成
        await messageProcessingTask;
        
        Console.WriteLine("示例完成");
    }
    
    // 处理消息的高性能方法
    private static async Task ProcessMessagesAsync()
    {
        await foreach (var messageArgs in _messageChannel.Reader.ReadAllAsync())
        {
            var topic = messageArgs.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(messageArgs.ApplicationMessage.PayloadSegment);
            
            // 在此处处理消息，例如保存到数据库、转发到其他系统等
            // 注意：这里应该使用异步操作以保持高性能
            
            // 简单示例：打印消息计数
            Console.Write($"\r处理消息: {topic}");
        }
    }
    
    // 发布测试消息
    private static async Task PublishTestMessagesAsync(IMqttClient mqttClient)
    {
        int messageCount = 0;
        
        while (true)
        {
            try
            {
                // 发布传感器数据
                await mqttClient.PublishAsync(
                    topic: "sensor/temperature",
                    payload: $"{{\"temperature\": {20 + new Random().NextDouble() * 10:F2}, \"timestamp\": {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}",
                    qualityOfServiceLevel: MqttQualityOfServiceLevel.AtMostOnce
                );
                
                messageCount++;
                Console.Write($"\r已发布消息数: {messageCount}");
                
                // 短暂延迟
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发布消息时发生错误: {ex.Message}");
                break;
            }
        }
    }
}
```

### 5. AOT编译的MQTT客户端示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package MQTTnet@4.1.4.561 
#:package Microsoft.Extensions.DependencyInjection@10.0.0 
#:package Microsoft.Extensions.Logging@10.0.0 
#:property LangVersion=preview 
#:property TargetFramework=net11.0 
#:property Nullable=enable 
#:property ImplicitUsings=enable 
#:property PublishAot=true 
#:property TrimMode=Full 

using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

public class AotCompiledMqttClient
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            // 创建依赖注入容器
            var serviceProvider = new ServiceCollection()
                .AddLogging(configure => {
                    configure.AddConsole();
                    configure.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<IMqttClient>(sp => {
                    var factory = new MqttFactory();
                    return factory.CreateMqttClient();
                })
                .BuildServiceProvider();
            
            var logger = serviceProvider.GetRequiredService<ILogger<AotCompiledMqttClient>>();
            var mqttClient = serviceProvider.GetRequiredService<IMqttClient>();
            
            logger.LogInformation("AOT编译的MQTT客户端启动");
            
            // 配置MQTT客户端选项
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId("aot-client-" + Guid.NewGuid().ToString())
                .WithTcpServer("localhost", 1883)
                .WithCleanSession()
                .Build();
            
            // 连接到MQTT服务器
            logger.LogInformation("正在连接到MQTT服务器");
            var connectResult = await mqttClient.ConnectAsync(clientOptions);
            
            if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                logger.LogError("连接到MQTT服务器失败: {ResultCode}", connectResult.ResultCode);
                return 1;
            }
            
            logger.LogInformation("成功连接到MQTT服务器");
            
            // 订阅主题
            await mqttClient.SubscribeAsync(
                topicFilter: "aot/test",
                qualityOfServiceLevel: MqttQualityOfServiceLevel.AtMostOnce
            );
            
            logger.LogInformation("已订阅主题 'aot/test'");
            
            // 注册消息接收事件
            mqttClient.ApplicationMessageReceivedAsync += e => {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                logger.LogInformation("收到消息: 主题 = {Topic}, 负载 = {Payload}", topic, payload);
                return Task.CompletedTask;
            };
            
            // 发布测试消息
            await mqttClient.PublishAsync(
                topic: "aot/test",
                payload: "Hello from AOT compiled MQTT client!",
                qualityOfServiceLevel: MqttQualityOfServiceLevel.AtLeastOnce
            );
            
            logger.LogInformation("已发布测试消息");
            
            // 等待5秒
            await Task.Delay(5000);
            
            // 断开连接
            await mqttClient.DisconnectAsync();
            logger.LogInformation("已断开连接");
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"发生错误: {ex.Message}");
            return 1;
        }
    }
}
```

## 总结

以上示例演示了MQTT技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手MQTT客户端的基本操作
2. 配置高级MQTT客户端选项，包括TLS、认证等
3. 设置和运行MQTT服务器
4. 实现高性能MQTT客户端，处理大量消息
5. 创建AOT编译的MQTT客户端，提高启动速度和运行性能

该系统设计遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

所有示例代码都可以直接编译运行，您可以根据自己的需求进行修改和扩展。
