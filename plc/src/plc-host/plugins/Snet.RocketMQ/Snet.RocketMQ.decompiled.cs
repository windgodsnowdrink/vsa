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
using Org.Apache.Rocketmq;
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
[assembly: AssemblyDescription("传输协议：RocketMQ（支持数据的生产与消费）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Mq")]
[assembly: AssemblyTitle("Snet.RocketMQ")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.RocketMQ;

/// <summary>
/// RocketMQ 数据配置类，封装 RocketMQ 5.x 生产者/消费者的连接参数（Broker 地址、ACL 认证、消费组、SSL 等）。
/// </summary>
public class RocketMQData
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
		/// Broker 地址
		/// </summary>
		[Description("Broker 地址")]
		[Verify("^((25[0-5]|2[0-4]\\d|[01]?\\d\\d?)\\.){3}(25[0-5]|2[0-4]\\d|[01]?\\d\\d?)$", "输入有误")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? IpAddress { get; set; }

		/// <summary>
		/// 端口（RocketMQ 5.x 客户端通过 gRPC 协议连接 Proxy，默认端口 8081）
		/// </summary>
		[Description("端口")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Port { get; set; }

		/// <summary>
		/// AccessKey（开启 ACL 认证时必填）
		/// </summary>
		[Description("AccessKey")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? AccessKey { get; set; }

		/// <summary>
		/// SecretKey（开启 ACL 认证时必填）
		/// </summary>
		[Description("SecretKey")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? SecretKey { get; set; }

		/// <summary>
		/// 消费组
		/// </summary>
		[Description("消费组")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string ConsumerGroup { get; set; }

		/// <summary>
		/// 启用 SSL（客户端默认开启 TLS，本地自建无 TLS 的 Broker 需关闭）
		/// </summary>
		[Description("启用SSL")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool SslEnabled { get; set; }

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
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			IpAddress = "127.0.0.1";
			Port = 6688;
			ConsumerGroup = "snet";
			ResponseType = (ResponseType)1;
			base..ctor();
		}
	}
}
/// <summary>
/// RocketMQ 消息队列操作类<br />
/// <br />
/// 功能：<br />
/// - 基于 RocketMQ.Client（Apache RocketMQ 5.x .NET 客户端，gRPC 协议连接 Proxy）实现消息的生产与消费<br />
/// - 支持主题的动态订阅与取消订阅<br />
/// - 支持多主题并发消费<br />
/// - 集成 ACL 认证（AccessKey/SecretKey）与 SSL 开关
/// </summary>
public class RocketMQOperate : MqAbstract<RocketMQOperate, RocketMQData.Basics>, IMq, IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// RocketMQ 消息监听器，收到消息后按响应类型经事件推送<br />
	/// 推送失败返回 FAILURE，由 RocketMQ 按重试策略重投
	/// </summary>
	private class RocketMQMessageListener : IMessageListener
	{
		private readonly RocketMQOperate operate;

		public RocketMQMessageListener(RocketMQOperate operate)
		{
			this.operate = operate;
		}

		public ConsumeResult Consume(MessageView messageView)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected I4, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Expected O, but got Unknown
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				string topic = messageView.Topic;
				byte[] body = messageView.Body;
				string text = $"{((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)operate).TAG} 接收到 ( {topic} ) 主题消息，消息ID：{messageView.MessageId}";
				ResponseType responseType = ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)operate).basics.ResponseType;
				switch ((int)responseType)
				{
				case 0:
					((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)operate).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)body));
					break;
				case 1:
					((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)operate).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)Encoding.UTF8.GetString(body)));
					break;
				case 2:
					((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)operate).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)new ResponseModel(topic, (object)Encoding.UTF8.GetString(body))));
					break;
				}
				return (ConsumeResult)0;
			}
			catch (Exception ex)
			{
				try
				{
					((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)operate).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费异常:" + ex.Message));
				}
				catch
				{
				}
				return (ConsumeResult)1;
			}
		}
	}

	/// <summary>
	/// RocketMQ 生产者实例，用于发送消息
	/// </summary>
	private Producer? producer;

	/// <summary>
	/// RocketMQ 推送消费者实例，用于接收消息
	/// </summary>
	private PushConsumer? consumer;

	/// <summary>
	/// 已订阅的主题集合（PushConsumer 的 Build 要求非空初始订阅，故消费者在首个 ConsumeAsync 时懒创建）
	/// </summary>
	private readonly ConcurrentDictionary<string, FilterExpression> TopicArray = new ConcurrentDictionary<string, FilterExpression>();

	/// <summary>
	/// 异步锁，保护消费者/生产者的创建与销毁（Build/Subscribe/DisposeAsync 均为异步网络调用，不能用 lock）
	/// </summary>
	private readonly SemaphoreSlim AsyncLock = new SemaphoreSlim(1, 1);

	/// <inheritdoc />
	protected override string CN => "RocketMQ";

	/// <inheritdoc />
	protected override string CD => "一个功能强大的开源分布式消息中间件，广泛应用于各种需要高性能异步解耦和可靠消息队列管理的业务场景";

	/// <summary>
	/// 是否打开
	/// </summary>
	private bool IsOpen { get; set; }

	/// <summary>
	/// 正在关闭（OffAsync 进行中），用于 GetStatusAsync 区分"未连接"与"关闭中"，避免半开状态误报
	/// </summary>
	private bool IsClosing { get; set; }

	/// <summary>
	/// 有参构造函数
	/// </summary>
	/// <param name="basics">基础数据</param>
	public RocketMQOperate(RocketMQData.Basics basics)
		: base(basics)
	{
	}

	/// <summary>
	/// 无参构造函数
	/// </summary>
	public RocketMQOperate()
	{
	}

	/// <summary>
	/// 构建客户端配置：RocketMQ 5.x 客户端经 gRPC 协议连接 Proxy
	/// </summary>
	/// <returns>客户端配置</returns>
	private ClientConfig BuildClientConfig()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		Builder val = new Builder().SetEndpoints($"{((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.IpAddress}:{((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.Port}").EnableSsl(((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.SslEnabled);
		if (!string.IsNullOrEmpty(((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.AccessKey) && !string.IsNullOrEmpty(((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.SecretKey))
		{
			val.SetCredentialsProvider((ISessionCredentialsProvider)new StaticSessionCredentialsProvider(((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.AccessKey, ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.SecretKey));
		}
		return val.Build();
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((MqAbstract<RocketMQOperate, RocketMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "OnAsync", 89);
			}
			if (string.IsNullOrEmpty(((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.AccessKey) != string.IsNullOrEmpty(((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.SecretKey))
			{
				return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, "AccessKey 与 SecretKey 需同时配置", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "OnAsync", 91);
			}
			producer = await new Builder().SetClientConfig(BuildClientConfig()).Build();
			IsOpen = true;
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "OnAsync", 98);
		}
		catch (Exception ex)
		{
			await ((MqAbstract<RocketMQOperate, RocketMQData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "OnAsync", 103);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((MqAbstract<RocketMQOperate, RocketMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "OffAsync", 116);
			}
			IsOpen = false;
			IsClosing = true;
			await AsyncLock.WaitAsync(token);
			PushConsumer val;
			Producer disposedProducer;
			try
			{
				val = consumer;
				consumer = null;
				disposedProducer = producer;
				producer = null;
				TopicArray.Clear();
			}
			finally
			{
				IsClosing = false;
				AsyncLock.Release();
			}
			if (val != null)
			{
				await val.DisposeAsync();
			}
			if (disposedProducer != null)
			{
				await disposedProducer.DisposeAsync();
			}
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "OffAsync", 143);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "OffAsync", 147);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		bool isOpen = IsOpen;
		string text = (IsOpen ? "已连接" : (IsClosing ? "关闭中" : "未连接"));
		return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(isOpen, text, (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync"), 154);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<RocketMQOperate, RocketMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "ProduceAsync", 164);
			}
			Message val = new Builder().SetTopic(topic).SetKeys(new string[1] { topic }).SetBody(content)
				.Build();
			ISendReceipt val2 = await producer.Send(val);
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(true, "主题 [ " + topic + " ] 发送成功，消息ID：" + val2.MessageId, (object)val2, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "ProduceAsync", 174);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "ProduceAsync", 178);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).BegOperateAsync(token, "ConsumeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<RocketMQOperate, RocketMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "ConsumeAsync", 189);
			}
			bool success = true;
			string message = null;
			await AsyncLock.WaitAsync(token);
			try
			{
				if (!IsOpen)
				{
					success = false;
					message = "未打开";
				}
				else if (TopicArray.ContainsKey(topic))
				{
					string text2 = topic + " 此主题已订阅";
					message = text2;
					success = false;
				}
				else if (consumer == null)
				{
					consumer = await new Builder().SetClientConfig(BuildClientConfig()).SetConsumerGroup(((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).basics.ConsumerGroup).SetSubscriptionExpression(new Dictionary<string, FilterExpression> { 
					{
						topic,
						FilterExpression.SubAll
					} })
						.SetMessageListener((IMessageListener)(object)new RocketMQMessageListener(this))
						.Build();
					TopicArray[topic] = FilterExpression.SubAll;
				}
				else
				{
					await consumer.Subscribe(topic, FilterExpression.SubAll);
					TopicArray[topic] = FilterExpression.SubAll;
				}
			}
			finally
			{
				AsyncLock.Release();
			}
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(success, message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "ConsumeAsync", 222);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "ConsumeAsync", 226);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).BegOperateAsync(token, "UnConsumeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<RocketMQOperate, RocketMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "UnConsumeAsync", 237);
			}
			bool success = true;
			string resultMessage = null;
			PushConsumer disposedConsumer = null;
			await AsyncLock.WaitAsync(token);
			try
			{
				if (!TopicArray.ContainsKey(topic))
				{
					string text2 = topic + " 此主题不存在";
					resultMessage = text2;
					success = false;
				}
				else
				{
					PushConsumer? obj = consumer;
					if (obj != null)
					{
						obj.Unsubscribe(topic);
					}
					TopicArray.TryRemove(topic, out FilterExpression _);
					if (TopicArray.IsEmpty && consumer != null)
					{
						disposedConsumer = consumer;
						consumer = null;
					}
				}
			}
			finally
			{
				AsyncLock.Release();
			}
			if (disposedConsumer != null)
			{
				await disposedConsumer.DisposeAsync();
			}
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(success, resultMessage, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "UnConsumeAsync", 267);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<RocketMQOperate, RocketMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.RocketMQ\\RocketMQOperate.cs", "UnConsumeAsync", 271);
		}
	}
}
