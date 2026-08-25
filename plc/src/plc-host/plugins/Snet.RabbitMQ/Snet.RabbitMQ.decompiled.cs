using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Snet.Core.@abstract;
using Snet.Core.extend;
using Snet.Model.attribute;
using Snet.Model.data;
using Snet.Model.@enum;
using Snet.Model.@interface;
using Snet.Utility;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("传输协议：RabbitMQ（支持数据的生产与消费）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Mq")]
[assembly: AssemblyTitle("Snet.RabbitMQ")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.RabbitMQ;

/// <summary>
/// 数据
/// </summary>
public class RabbitMQData
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class Basics
	{
		[CompilerGenerated]
		private ResponseType <ResponseType>k__BackingField;

		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Category("基础数据")]
		[Description("唯一标识符")]
		public string SN { get; set; }

		/// <summary>
		/// 交换机名称
		/// </summary>
		[Description("交换机名称")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string ExChangeName { get; set; }

		/// <summary>
		/// Ip地址
		/// </summary>
		[Description("Ip地址")]
		[Verify("^((25[0-5]|2[0-4]\\d|[01]?\\d\\d?)\\.){3}(25[0-5]|2[0-4]\\d|[01]?\\d\\d?)$", "输入有误")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? IpAddress { get; set; }

		/// <summary>
		/// 端口
		/// </summary>
		[Description("端口")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Port { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[Description("用户名")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? UserName { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		[Description("密码")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? Password { get; set; }

		/// <summary>
		/// 消息过期时间
		/// </summary>
		[Description("消息过期时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int MessageExpirationTime { get; set; }

		/// <summary>
		/// 响应类型
		/// </summary>
		[Description("响应类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public ResponseType ResponseType
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <ResponseType>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<ResponseType>k__BackingField = value;
			}
		}

		public Basics()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			ExChangeName = "exchang";
			IpAddress = "127.0.0.1";
			Port = 6688;
			UserName = "shunnet";
			Password = "shunnet";
			MessageExpirationTime = 86400000;
			ResponseType = (ResponseType)1;
			base..ctor();
		}
	}
}
/// <summary>
/// RabbitMQ 消息队列操作类<br />
/// <br />
/// 功能：<br />
/// - 基于 RabbitMQ.Client 实现消息的发布与消费<br />
/// - 支持多种交换机类型（direct/fanout/headers/topic）<br />
/// - 支持消息持久化、过期时间设置<br />
/// - 支持手动/自动消息确认
/// </summary>
public class RabbitMQOperate : MqAbstract<RabbitMQOperate, RabbitMQData.Basics>, IMq, IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// 通道集合（按交换机名称索引，线程安全）
	/// </summary>
	private readonly ConcurrentDictionary<string, IChannel> Channels = new ConcurrentDictionary<string, IChannel>();

	/// <summary>
	/// 已消费的队列集合（用于防止重复消费，线程安全）
	/// </summary>
	private readonly ConcurrentDictionary<string, string> Queues = new ConcurrentDictionary<string, string>();

	/// <summary>
	/// 队列与消息属性的映射关系（包含队列声明结果和消息基本属性）
	/// </summary>
	private readonly ConcurrentDictionary<string, (QueueDeclareOk QueueDeclareOk, BasicProperties BasicProperties)> QueueAndBasicProperties = new ConcurrentDictionary<string, (QueueDeclareOk, BasicProperties)>();

	/// <inheritdoc />
	protected override string CN => "RabbitMQ";

	/// <inheritdoc />
	protected override string CD => "一个功能强大的开源消息代理软件，广泛应用于各种需要消息传递和队列管理的场景";

	/// <summary>
	/// RabbitMQ 连接对象（管理与服务端的 TCP 连接）
	/// </summary>
	private IConnection? connection { get; set; }

	/// <summary>
	/// 有参构造函数
	/// </summary>
	/// <param name="basics">基础数据</param>
	public RabbitMQOperate(RabbitMQData.Basics basics)
		: base(basics)
	{
	}

	/// <summary>
	/// 无参构造函数
	/// </summary>
	public RabbitMQOperate()
	{
	}

	/// <summary>
	/// 发布 一个交换机可以发布多个消息
	/// </summary>
	/// <param name="Content">消息内容</param>
	/// <param name="Topic">主题不为空，则队列、路由设置为主题</param>
	/// <param name="Queue">队列名称</param>
	/// <param name="RoutingKey">路由键</param>
	/// <param name="Type">
	/// 通道类型
	/// direct  //用于AMQP直接交换的交换类型。
	/// fanout  //用于AMQP扇出交换的交换类型。
	/// headers //用于AMQP报头交换的交换类型。
	/// topic   //用于AMQP主题交换的交换类型。
	/// </param>
	/// <param name="Durable">持久化</param>
	/// <param name="Exclusive">独有的</param>
	/// <param name="AutoDelete">自动删除</param>
	/// <returns>统一出参</returns>
	public OperateResult Publish(string Content, string? Topic = null, string? Queue = null, string? RoutingKey = null, string Type = "topic", bool Durable = true, bool Exclusive = false, bool AutoDelete = false)
	{
		return PublishAsync(Encoding.UTF8.GetBytes(Content), Topic, Queue, RoutingKey, Type, Durable, Exclusive, AutoDelete).GetAwaiter().GetResult();
	}

	/// <summary>
	/// 发布 一个交换机可以发布多个消息
	/// </summary>
	/// <param name="Content">消息内容</param>
	/// <param name="Topic">主题不为空，则队列、路由设置为主题</param>
	/// <param name="Queue">队列名称</param>
	/// <param name="RoutingKey">路由键</param>
	/// <param name="Type">
	/// 通道类型
	/// direct  //用于AMQP直接交换的交换类型。
	/// fanout  //用于AMQP扇出交换的交换类型。
	/// headers //用于AMQP报头交换的交换类型。
	/// topic   //用于AMQP主题交换的交换类型。
	/// </param>
	/// <param name="Durable">持久化</param>
	/// <param name="Exclusive">独有的</param>
	/// <param name="AutoDelete">自动删除</param>
	/// <returns>统一出参</returns>
	public OperateResult Publish(byte[] Content, string? Topic = null, string? Queue = null, string? RoutingKey = null, string Type = "topic", bool Durable = true, bool Exclusive = false, bool AutoDelete = false)
	{
		return PublishAsync(Content, Topic, Queue, RoutingKey, Type, Durable, Exclusive, AutoDelete).GetAwaiter().GetResult();
	}

	/// <summary>
	/// 发布 一个交换机可以发布多个消息
	/// </summary>
	/// <param name="Content">消息内容</param>
	/// <param name="Topic">主题不为空，则队列、路由设置为主题</param>
	/// <param name="Queue">队列名称</param>
	/// <param name="RoutingKey">路由键</param>
	/// <param name="Type">
	/// 通道类型
	/// direct  //用于AMQP直接交换的交换类型。
	/// fanout  //用于AMQP扇出交换的交换类型。
	/// headers //用于AMQP报头交换的交换类型。
	/// topic   //用于AMQP主题交换的交换类型。
	/// </param>
	/// <param name="Durable">持久化</param>
	/// <param name="Exclusive">独有的</param>
	/// <param name="AutoDelete">自动删除</param>
	/// <param name="token">传播消息取消</param>
	/// <returns>统一出参</returns>
	public async Task<OperateResult> PublishAsync(string Content, string? Topic = null, string? Queue = null, string? RoutingKey = null, string Type = "topic", bool Durable = true, bool Exclusive = false, bool AutoDelete = false, CancellationToken token = default(CancellationToken))
	{
		return await PublishAsync(Encoding.UTF8.GetBytes(Content), Topic, Queue, RoutingKey, Type, Durable, Exclusive, AutoDelete, token);
	}

	/// <summary>
	/// 发布 一个交换机可以发布多个消息
	/// </summary>
	/// <param name="Content">消息内容</param>
	/// <param name="Topic">主题不为空，则队列、路由设置为主题</param>
	/// <param name="Queue">队列名称</param>
	/// <param name="RoutingKey">路由键</param>
	/// <param name="Type">
	/// 通道类型
	/// direct  //用于AMQP直接交换的交换类型。
	/// fanout  //用于AMQP扇出交换的交换类型。
	/// headers //用于AMQP报头交换的交换类型。
	/// topic   //用于AMQP主题交换的交换类型。
	/// </param>
	/// <param name="Durable">持久化</param>
	/// <param name="Exclusive">独有的</param>
	/// <param name="AutoDelete">自动删除</param>
	/// <param name="token">传播消息取消</param>
	/// <returns>统一出参</returns>
	public async Task<OperateResult> PublishAsync(byte[] Content, string? Topic = null, string? Queue = null, string? RoutingKey = null, string Type = "topic", bool Durable = true, bool Exclusive = false, bool AutoDelete = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "PublishAsync");
		try
		{
			if (connection == null || !connection.IsOpen)
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, "RabbitMQ连接未打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "PublishAsync", 152);
			}
			if (!string.IsNullOrWhiteSpace(Topic))
			{
				Queue = Topic;
				RoutingKey = Topic;
			}
			else if (string.IsNullOrWhiteSpace(Queue) || string.IsNullOrWhiteSpace(RoutingKey))
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, "Queue、RoutingKey 不能为空", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "PublishAsync", 166);
			}
			if (!Channels.TryGetValue(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, out IChannel _))
			{
				IChannel channel = await connection.CreateChannelAsync((CreateChannelOptions)null, token);
				await channel.ExchangeDeclareAsync(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, Type, Durable, AutoDelete, (IDictionary<string, object>)null, false, false, token);
				Channels.GetOrAdd(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, channel);
			}
			if (!QueueAndBasicProperties.ContainsKey(Queue))
			{
				QueueDeclareOk queueDeclareOk = await Channels[((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName].QueueDeclareAsync(Queue, Durable, Exclusive, AutoDelete, (IDictionary<string, object>)null, false, false, token);
				await Channels[((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName].QueueBindAsync(Queue, ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, RoutingKey, (IDictionary<string, object>)null, false, token);
				BasicProperties val = new BasicProperties();
				val.Persistent = Durable;
				val.Timestamp = new AmqpTimestamp(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
				val.Expiration = ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.MessageExpirationTime.ToString();
				QueueAndBasicProperties.TryAdd(Queue, (queueDeclareOk, val));
			}
			await Channels[((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName].BasicPublishAsync<BasicProperties>(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, RoutingKey, true, QueueAndBasicProperties[Queue].BasicProperties, (ReadOnlyMemory<byte>)Content, token);
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "PublishAsync", 197);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "PublishAsync", 202);
		}
	}

	/// <summary>
	/// 消费 一个交换机可以消费多个消息
	/// </summary>
	/// <param name="Topic">主题不为空，则队列、路由设置为主题</param>
	/// <param name="Queue">队列名称</param>
	/// <param name="RoutingKey">路由键</param>
	/// <param name="Type">
	/// 通道类型
	/// direct  //用于AMQP直接交换的交换类型。
	/// fanout  //用于AMQP扇出交换的交换类型。
	/// headers //用于AMQP报头交换的交换类型。
	/// topic   //用于AMQP主题交换的交换类型。
	/// </param>
	/// <param name="AutoAck">自动确认</param>
	/// <param name="Durable">持久化</param>
	/// <param name="Exclusive">独有的</param>
	/// <param name="AutoDelete">自动删除</param>
	/// <returns>统一出参</returns>
	public OperateResult Consume(string? Topic = null, string? Queue = null, string? RoutingKey = null, string Type = "topic", bool AutoAck = false, bool Durable = true, bool Exclusive = false, bool AutoDelete = false)
	{
		return ConsumeAsync(Topic, Queue, RoutingKey, Type, AutoAck, Durable, Exclusive, AutoDelete).GetAwaiter().GetResult();
	}

	/// <summary>
	/// 消费 一个交换机可以消费多个消息
	/// </summary>
	/// <param name="Topic">主题不为空，则队列、路由设置为主题</param>
	/// <param name="Queue">队列名称</param>
	/// <param name="RoutingKey">路由键</param>
	/// <param name="Type">
	/// 通道类型
	/// direct  //用于AMQP直接交换的交换类型。
	/// fanout  //用于AMQP扇出交换的交换类型。
	/// headers //用于AMQP报头交换的交换类型。
	/// topic   //用于AMQP主题交换的交换类型。
	/// </param>
	/// <param name="AutoAck">自动确认</param>
	/// <param name="Durable">持久化</param>
	/// <param name="Exclusive">独有的</param>
	/// <param name="AutoDelete">自动删除</param>
	/// <param name="token">传播消息取消</param>
	/// <returns>统一出参</returns>
	public async Task<OperateResult> ConsumeAsync(string? Topic = null, string? Queue = null, string? RoutingKey = null, string Type = "topic", bool AutoAck = false, bool Durable = true, bool Exclusive = false, bool AutoDelete = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "ConsumeAsync");
		try
		{
			if (connection == null || !connection.IsOpen)
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, "RabbitMQ连接未打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ConsumeAsync", 254);
			}
			if (!string.IsNullOrWhiteSpace(Topic))
			{
				Queue = Topic;
				RoutingKey = Topic;
			}
			else if (string.IsNullOrWhiteSpace(Queue) || string.IsNullOrWhiteSpace(RoutingKey))
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, "Queue、RoutingKey 不能为空", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ConsumeAsync", 268);
			}
			if (!Channels.TryGetValue(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, out IChannel _))
			{
				IChannel channel = await connection.CreateChannelAsync((CreateChannelOptions)null, token);
				await channel.ExchangeDeclareAsync(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, Type, Durable, AutoDelete, (IDictionary<string, object>)null, false, false, token);
				Channels.GetOrAdd(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, channel);
			}
			if (!Queues.ContainsKey(Queue))
			{
				await Channels[((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName].QueueDeclareAsync(Queue, Durable, Exclusive, AutoDelete, (IDictionary<string, object>)null, false, false, token);
				await Channels[((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName].QueueBindAsync(Queue, ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, RoutingKey, (IDictionary<string, object>)null, false, token);
				AsyncEventingBasicConsumer val = new AsyncEventingBasicConsumer(Channels[((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName]);
				AsyncEventHandler<BasicDeliverEventArgs> val2 = async delegate(object sender, BasicDeliverEventArgs e)
				{
					await Consumer_ReceivedAsync(sender, e, AutoAck);
				};
				val.ReceivedAsync += val2;
				string value2 = await IChannelExtensions.BasicConsumeAsync(Channels[((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName], Queue, AutoAck, (IAsyncBasicConsumer)(object)val, token);
				Queues.TryAdd(Queue, value2);
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ConsumeAsync", 300);
			}
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, "已存在 " + Queue + " 的消费", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ConsumeAsync", 304);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ConsumeAsync", 310);
		}
	}

	/// <summary>
	/// 消费者消息接收回调<br />
	/// 解析消息内容并根据响应类型分发事件<br />
	/// 非自动确认模式下手动提交 ACK
	/// </summary>
	/// <param name="sender">事件源</param>
	/// <param name="e">消息事件参数（包含路由键、消息体等）</param>
	/// <param name="AutoAck">是否自动确认（false 时需手动 ACK）</param>
	/// <returns>异步任务</returns>
	private async Task Consumer_ReceivedAsync(object? sender, BasicDeliverEventArgs e, bool AutoAck)
	{
		try
		{
			string routingKey = e.RoutingKey;
			byte[] array = e.Body.ToArray();
			string text = ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).TAG + " 接收到 ( " + routingKey + " ) 主题消息";
			ResponseType responseType = ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ResponseType;
			switch ((int)responseType)
			{
			case 0:
				await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)array));
				break;
			case 1:
				await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)Encoding.UTF8.GetString(array)));
				break;
			case 2:
				await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)new ResponseModel(routingKey, (object)Encoding.UTF8.GetString(array))));
				break;
			}
			if (!AutoAck && Channels.TryGetValue(e.Exchange, out IChannel value))
			{
				await value.BasicAckAsync(e.DeliveryTag, false, default(CancellationToken));
			}
		}
		catch (Exception ex)
		{
			await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费异常:" + ex.Message));
			if (!AutoAck && Channels.TryGetValue(e.Exchange, out IChannel value2))
			{
				try
				{
					await value2.BasicRejectAsync(e.DeliveryTag, true, default(CancellationToken));
				}
				catch
				{
				}
			}
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((MqAbstract<RabbitMQOperate, RabbitMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "OnAsync", 373);
			}
			connection = await new ConnectionFactory
			{
				HostName = ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.IpAddress,
				Port = ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.Port,
				UserName = ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.UserName,
				Password = ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.Password
			}.CreateConnectionAsync(token);
			if (connection.IsOpen)
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "OnAsync", 387);
			}
			await ((MqAbstract<RabbitMQOperate, RabbitMQData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, "RabbitMQ连接打开失败", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "OnAsync", 390);
		}
		catch (Exception ex)
		{
			await ((MqAbstract<RabbitMQOperate, RabbitMQData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "OnAsync", 395);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((MqAbstract<RabbitMQOperate, RabbitMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "OffAsync", 409);
			}
			foreach (IChannel channel in Channels.Values)
			{
				try
				{
					await IChannelExtensions.CloseAsync(channel, token);
					((IDisposable)channel).Dispose();
				}
				catch
				{
				}
			}
			if (connection != null)
			{
				await IConnectionExtensions.CloseAsync(connection, token);
				((IDisposable)connection).Dispose();
				connection = null;
			}
			Channels.Clear();
			Queues.Clear();
			QueueAndBasicProperties.Clear();
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "OffAsync", 436);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "OffAsync", 440);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
		string text = default(string);
		if (!((ResultModel)(await ((MqAbstract<RabbitMQOperate, RabbitMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ProduceAsync", 450);
		}
		return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(await PublishAsync(content, topic, null, null, "topic", Durable: true, Exclusive: false, AutoDelete: false, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ProduceAsync", 452);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "ConsumeAsync");
		string text = default(string);
		if (!((ResultModel)(await ((MqAbstract<RabbitMQOperate, RabbitMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ConsumeAsync", 461);
		}
		return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(await ConsumeAsync(topic, null, null, "topic", AutoAck: false, Durable: true, Exclusive: false, AutoDelete: false, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "ConsumeAsync", 463);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "UnConsumeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<RabbitMQOperate, RabbitMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "UnConsumeAsync", 473);
			}
			if (!Queues.ContainsKey(topic))
			{
				return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, topic + " 此主题未在消费", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "UnConsumeAsync", 477);
			}
			if (Channels.TryGetValue(((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).basics.ExChangeName, out IChannel value) && !string.IsNullOrWhiteSpace(Queues[topic]))
			{
				await value.BasicCancelAsync(Queues[topic], false, token);
			}
			Queues.TryRemove(topic, out string _);
			QueueAndBasicProperties.TryRemove(topic, out (QueueDeclareOk, BasicProperties) _);
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "UnConsumeAsync", 486);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "UnConsumeAsync", 490);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
		if (connection == null || !connection.IsOpen)
		{
			return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(false, "未连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "GetStatusAsync", 500);
		}
		return await ((CoreUnify<RabbitMQOperate, RabbitMQData.Basics>)(object)this).EndOperateAsync(true, "已连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RabbitMQ\\RabbitMQOperate.cs", "GetStatusAsync", 502);
	}
}
