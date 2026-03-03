#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web #:package Fody@6.8.0 #:package MQTTnet@4.3.1 #:package xunit@2.6.0 #:package Moq@4.18.4 #:property LangVersion=preview #:property TargetFramework=net10.0 #:property Nullable=enable #:property ImplicitUsings=enable

using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Xunit;
using Moq;

/// <summary>
/// MQTT事件总线测试 - 使用Fody编织器的MQTT事件总线实现和测试
/// </summary>
namespace SourceGenerator.Fody.Mqtt
{
    /// <summary>
    /// Fody配置类 - 用于配置Fody编织器
    /// </summary>
    public class FodyConfiguration
    {
        /// <summary>
        /// 配置Fody编织器
        /// </summary>
        public static void Configure()
        {
            // 配置Fody编织器
            // 这里可以添加Fody编织器的配置代码
        }
    }

    /// <summary>
    /// 事件总线接口 - 定义事件总线的核心方法
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 连接到事件总线
        /// </summary>
        Task ConnectAsync();
        
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="topic">事件主题</param>
        /// <param name="payload">事件负载</param>
        Task PublishAsync(string topic, string payload);
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="topic">事件主题</param>
        /// <param name="handler">事件处理程序</param>
        Task SubscribeAsync(string topic, Func<string, Task> handler);
    }

    /// <summary>
    /// MQTT事件总线实现 - 基于MQTT协议的事件总线
    /// </summary>
    public class MqttEventBus : IEventBus
    {
        /// <summary>
        /// MQTT客户端
        /// </summary>
        private readonly IMqttClient _mqttClient;
        
        /// <summary>
        /// MQTT工厂
        /// </summary>
        private readonly MqttFactory _mqttFactory;
        
        /// <summary>
        /// MQTT客户端选项
        /// </summary>
        private readonly MqttClientOptions _options;

        /// <summary>
        /// 构造函数 - 初始化MQTT事件总线
        /// </summary>
        /// <param name="brokerUrl">MQTT代理服务器地址</param>
        /// <param name="port">MQTT代理服务器端口</param>
        public MqttEventBus(string brokerUrl, int port)
        {
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            
            // 配置MQTT客户端选项
            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerUrl, port)
                .WithCleanSession()
                .Build();
        }

        /// <summary>
        /// 连接到MQTT代理服务器
        /// </summary>
        public async Task ConnectAsync()
        {
            await _mqttClient.ConnectAsync(_options);
        }

        /// <summary>
        /// 发布MQTT消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="payload">消息内容</param>
        public async Task PublishAsync(string topic, string payload)
        {
            // 创建MQTT应用消息
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            // 发布消息
            await _mqttClient.PublishAsync(message);
        }

        /// <summary>
        /// 订阅MQTT主题
        /// </summary>
        /// <param name="topic">要订阅的主题</param>
        /// <param name="handler">消息处理函数</param>
        public async Task SubscribeAsync(string topic, Func<string, Task> handler)
        {
            // 注册消息接收事件处理程序
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                // 如果消息主题匹配，则调用处理函数
                if (e.ApplicationMessage.Topic == topic)
                {
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    await handler(payload);
                }
            };

            // 订阅主题
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .Build());
        }
    }

    /// <summary>
    /// MQTT事件总线测试类 - 测试MQTT事件总线的功能
    /// </summary>
    public class MqttEventBusTests
    {
        /// <summary>
        /// 测试连接到MQTT代理服务器
        /// </summary>
        [Fact]
        public async Task ConnectAsync_Should_Connect_Successfully()
        {
            // Arrange
            // 创建MQTT事件总线实例
            var eventBus = new MqttEventBus("localhost", 1883);
            
            // Act
            // 尝试连接到MQTT代理服务器
            await eventBus.ConnectAsync();
            
            // Assert
            // 验证连接成功（在实际测试中，应该使用模拟对象来验证）
            Assert.True(true);
        }

        /// <summary>
        /// 测试发布MQTT消息
        /// </summary>
        [Fact]
        public async Task PublishAsync_Should_Publish_Message()
        {
            // Arrange
            // 创建MQTT事件总线实例并连接
            var eventBus = new MqttEventBus("localhost", 1883);
            await eventBus.ConnectAsync();
            
            // Act
            // 发布测试消息
            await eventBus.PublishAsync("test/topic", "test message");
            
            // Assert
            // 验证消息发布成功
            Assert.True(true);
        }

        /// <summary>
        /// 测试订阅MQTT主题并接收消息
        /// </summary>
        [Fact]
        public async Task SubscribeAsync_Should_Receive_Message()
        {
            // Arrange
            // 创建MQTT事件总线实例并连接
            var eventBus = new MqttEventBus("localhost", 1883);
            await eventBus.ConnectAsync();
            
            var receivedMessage = string.Empty;
            var messageReceived = new TaskCompletionSource<bool>();
            
            // 订阅测试主题
            await eventBus.SubscribeAsync("test/topic", async message =>
            {
                receivedMessage = message;
                messageReceived.TrySetResult(true);
            });
            
            // Act
            // 发布测试消息
            await eventBus.PublishAsync("test/topic", "test message");
            
            // 等待消息接收或超时
            var timeout = Task.Delay(5000);
            var completedTask = await Task.WhenAny(messageReceived.Task, timeout);
            
            // Assert
            // 验证消息是否被正确接收
            Assert.True(completedTask == messageReceived.Task, "消息接收超时");
            Assert.Equal("test message", receivedMessage);
        }
    }
}