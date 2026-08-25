using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
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
[assembly: AssemblyDescription("传输协议：Kafka（支持数据的生产与消费）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Mq")]
[assembly: AssemblyTitle("Snet.Kafka")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Kafka;

/// <summary>
/// Kafka 数据配置类，封装 Kafka 生产者/消费者的连接参数（服务器地址、安全协议、SASL 认证等）。
/// </summary>
public class KafkaData
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class Basics
	{
		[CompilerGenerated]
		private SecurityProtocol <SecurityProtocol>k__BackingField;

		[CompilerGenerated]
		private AutoOffsetReset <AutoOffsetReset>k__BackingField;

		[CompilerGenerated]
		private SaslMechanism <SaslMechanism>k__BackingField;

		[CompilerGenerated]
		private ResponseType <ResponseType>k__BackingField;

		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Category("基础数据")]
		[Description("唯一标识符")]
		public string SN { get; set; }

		/// <summary>
		/// 服务器地址
		/// </summary>
		[Description("服务器地址")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? BootstrapServers { get; set; }

		/// <summary>
		/// 安全协议
		/// </summary>
		[Description("安全协议")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public SecurityProtocol SecurityProtocol
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <SecurityProtocol>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<SecurityProtocol>k__BackingField = value;
			}
		}

		/// <summary>
		/// 自动偏移复位
		/// </summary>
		[Description("自动偏移复位")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public AutoOffsetReset AutoOffsetReset
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <AutoOffsetReset>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<AutoOffsetReset>k__BackingField = value;
			}
		}

		/// <summary>
		/// SASL 认证机制，指定 Kafka 客户端与服务端的认证方式。<br />
		/// 可选类型：
		/// <list type="bullet">
		/// <item><see cref="F:Confluent.Kafka.SaslMechanism.Gssapi" />：Kerberos 认证（GSSAPI，常用于企业级安全场景）</item>
		/// <item><see cref="F:Confluent.Kafka.SaslMechanism.Plain" />：PLAIN 认证（用户名密码认证）</item>
		/// <item><see cref="F:Confluent.Kafka.SaslMechanism.ScramSha256" />：SCRAM-SHA-256 认证</item>
		/// <item><see cref="F:Confluent.Kafka.SaslMechanism.ScramSha512" />：SCRAM-SHA-512 认证</item>
		/// <item><see cref="F:Confluent.Kafka.SaslMechanism.OAuthBearer" />：OAuth 2.0 Bearer Token 认证</item>
		/// </list>
		/// 默认值为 <see cref="F:Confluent.Kafka.SaslMechanism.Gssapi" />。
		/// </summary>
		[Description("SASL 认证机制")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public SaslMechanism SaslMechanism
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <SaslMechanism>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<SaslMechanism>k__BackingField = value;
			}
		}

		/// <summary>
		/// Kerberos 服务名，需与服务端配置一致。<br />
		/// 仅在使用 <see cref="F:Confluent.Kafka.SaslMechanism.Gssapi" />（Kerberos）时生效。
		/// </summary>
		[Description("Kerberos 服务名")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string SaslKerberosServiceName { get; set; }

		/// <summary>
		/// 消费组（固定值：同组内消息分摊消费，偏移量延续；不同组各自全量消费）
		/// </summary>
		[Description("消费组")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string GroupId { get; set; }

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
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			SaslKerberosServiceName = "snet";
			GroupId = "snet";
			ResponseType = (ResponseType)1;
			base..ctor();
		}
	}
}
/// <summary>
/// Kafka 消息队列操作类<br />
/// <br />
/// 功能：<br />
/// - 基于 Confluent.Kafka 实现消息的生产与消费<br />
/// - 支持主题的创建、订阅与取消订阅<br />
/// - 支持多主题并发消费<br />
/// - 集成 SASL/Kerberos 安全认证
/// </summary>
public class KafkaOperate : MqAbstract<KafkaOperate, KafkaData.Basics>, IMq, IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// Kafka 生产者实例，用于发送消息
	/// </summary>
	private IProducer<string, byte[]>? producer;

	/// <summary>
	/// Kafka 消费者实例，用于接收消息
	/// </summary>
	private IConsumer<string, byte[]>? consumer;

	/// <summary>
	/// Kafka 管理员客户端实例
	/// </summary>
	private IAdminClient? adminClient;

	/// <summary>
	/// 已订阅的主题集合（ConcurrentDictionary 保证线程安全）
	/// </summary>
	private readonly ConcurrentDictionary<string, bool> TopicArray = new ConcurrentDictionary<string, bool>();

	/// <summary>
	/// 订阅令牌
	/// </summary>
	private CancellationTokenSource? tokenSource;

	/// <summary>
	/// 异步锁，保护消费者的创建/订阅/销毁与轮询启动（Subscribe/Consume 并发时防止实例泄漏与双轮询）
	/// </summary>
	private readonly SemaphoreSlim AsyncLock = new SemaphoreSlim(1, 1);

	/// <inheritdoc />
	protected override string CN => "Kafka";

	/// <inheritdoc />
	protected override string CD => "一个功能强大且灵活的消息系统，能够满足各种分布式系统中对消息传递的需求";

	/// <summary>
	/// 生产者配置（连接参数、幂等性等）
	/// </summary>
	private ProducerConfig? producerConfig { get; set; }

	/// <summary>
	/// 消费者配置（偏移策略、消费组等）
	/// </summary>
	private ConsumerConfig? consumerConfig { get; set; }

	/// <summary>
	/// 客户端管理员配置（用于创建/删除主题等管理操作）
	/// </summary>
	private AdminClientConfig? adminClientConfig { get; set; }

	/// <summary>
	/// 是否打开
	/// </summary>
	private bool IsOpen { get; set; }

	/// <summary>
	/// 有参构造函数
	/// </summary>
	/// <param name="basics">基础数据</param>
	public KafkaOperate(KafkaData.Basics basics)
		: base(basics)
	{
	}

	/// <summary>
	/// 无参构造函数
	/// </summary>
	public KafkaOperate()
	{
	}

	/// <summary>
	/// 轮询消费消息<br />
	/// 持续从 Kafka 消费者拉取消息，根据响应类型分发事件<br />
	/// 消息处理完成后手动提交偏移量，确保消息不丢失
	/// </summary>
	/// <param name="source">取消令牌源，用于控制轮询生命周期</param>
	/// <returns>异步任务</returns>
	private async Task PollingAsync(CancellationTokenSource source)
	{
		while (!source.IsCancellationRequested)
		{
			try
			{
				ConsumeResult<string, byte[]> result = consumer?.Consume(source.Token);
				if (result != null)
				{
					string topic = result.Topic;
					byte[] value = result.Message.Value;
					string key = result.Message.Key;
					Offset offset = result.TopicPartitionOffset.Offset;
					long value2 = ((Offset)(ref offset)).Value;
					string text = $"{((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).TAG} 接收到 ( {topic} ) 主题消息， 键 ( {key} ) 偏移量 ( {value2} )";
					ResponseType responseType = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.ResponseType;
					switch ((int)responseType)
					{
					case 0:
						await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)value));
						break;
					case 1:
						await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)Encoding.UTF8.GetString(value)));
						break;
					case 2:
						await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)new ResponseModel(topic, (object)Encoding.UTF8.GetString(value))));
						break;
					}
					consumer?.Commit(result);
				}
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception ex2)
			{
				if (source.IsCancellationRequested)
				{
					break;
				}
				await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费异常:" + ex2.Message));
				await Task.Delay(1000, source.Token);
			}
		}
	}

	/// <summary>
	/// 异步创建主题（内部委托给同步方法）
	/// </summary>
	/// <param name="Topics">需要创建的主题名称列表</param>
	/// <returns>操作结果</returns>
	public async Task<OperateResult> CreateTopicsAsync(List<string> Topics, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "CreateTopicsAsync");
		try
		{
			if (IsOpen && adminClient != null)
			{
				Metadata metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5.0));
				List<string> list = new List<string>();
				foreach (TopicMetadata topic in metadata.Topics)
				{
					list.Add(topic.Topic);
				}
				List<string> list2 = Topics.Except(list).ToList();
				List<TopicSpecification> list3 = new List<TopicSpecification>();
				foreach (string item in list2)
				{
					list3.Add(new TopicSpecification
					{
						Name = item
					});
				}
				adminClient.CreateTopicsAsync((IEnumerable<TopicSpecification>)list3, (CreateTopicsOptions)null).Wait(TimeSpan.FromSeconds(5.0), token);
				return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(true, "主题集合创建成功", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "CreateTopicsAsync", 176);
			}
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, "未打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "CreateTopicsAsync", 180);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "CreateTopicsAsync", 185);
		}
	}

	/// <summary>
	/// 创建 Kafka 主题<br />
	/// 自动排除已存在的主题，仅创建差异部分<br />
	/// 需要在 On() 打开后调用
	/// </summary>
	/// <param name="Topics">需要创建的主题名称列表</param>
	/// <returns>操作结果</returns>
	public OperateResult CreateTopics(List<string> Topics)
	{
		return CreateTopicsAsync(Topics).GetAwaiter().GetResult();
	}

	/// <summary>
	/// 构建消费者配置（固定消费组保证偏移量延续，仅手动提交防止处理失败被自动提交）
	/// </summary>
	/// <returns>消费者配置</returns>
	private ConsumerConfig BuildConsumerConfig()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		return new ConsumerConfig
		{
			BootstrapServers = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.BootstrapServers,
			GroupId = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.GroupId,
			AutoOffsetReset = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.AutoOffsetReset,
			EnableAutoCommit = false,
			SecurityProtocol = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SecurityProtocol,
			SaslMechanism = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SaslMechanism,
			SaslKerberosServiceName = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SaslKerberosServiceName
		};
	}

	/// <summary>
	/// 异步生产消息（字符串内容，内部转为 UTF8 字节后委托同步方法）
	/// </summary>
	/// <param name="Topic">目标主题</param>
	/// <param name="Key">消息键（用于分区路由）</param>
	/// <param name="Content">消息内容（字符串）</param>
	/// <param name="token">传播通知取消</param>
	/// <returns>操作结果</returns>
	public async Task<OperateResult> ProduceAsync(string Topic, string Key, string Content, CancellationToken token = default(CancellationToken))
	{
		return await ProduceAsync(Topic, Key, Encoding.UTF8.GetBytes(Content), token);
	}

	/// <summary>
	/// 异步生产消息（字节内容，委托同步方法）
	/// </summary>
	/// <param name="Topic">目标主题</param>
	/// <param name="Key">消息键（用于分区路由）</param>
	/// <param name="Content">消息内容（字节数组）</param>
	/// <param name="token">传播通知取消</param>
	/// <returns>操作结果</returns>
	public async Task<OperateResult> ProduceAsync(string Topic, string Key, byte[] Content, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
		try
		{
			if (IsOpen && producer != null)
			{
				DeliveryReport<string, byte[]> deliveryReport = null;
				producer.Produce(Topic, new Message<string, byte[]>
				{
					Key = Key,
					Value = Content
				}, (Action<DeliveryReport<string, byte[]>>)delegate(DeliveryReport<string, byte[]> Result)
				{
					deliveryReport = Result;
				});
				producer.Flush(TimeSpan.FromSeconds(5.0));
				if (deliveryReport == null)
				{
					return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, "主题 [ " + Topic + " ] 发送超时", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ProduceAsync", 255);
				}
				if ((int)deliveryReport.Error.Code != 0)
				{
					return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, "主题 [ " + Topic + " ] 发送失败：" + deliveryReport.Error.Reason, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ProduceAsync", 257);
				}
				return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(true, $"主题 [ {Topic} ] 发送成功，偏移量：{((DeliveryResult<string, byte[]>)(object)deliveryReport).TopicPartitionOffset}", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ProduceAsync", 258);
			}
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, "未打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ProduceAsync", 262);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ProduceAsync", 267);
		}
	}

	/// <summary>
	/// 生产
	/// </summary>
	/// <param name="Topic">主题</param>
	/// <param name="Key">键</param>
	/// <param name="Content">值</param>
	/// <returns>统一出参</returns>
	public OperateResult Produce(string Topic, string Key, string Content)
	{
		return ProduceAsync(Topic, Key, Content).GetAwaiter().GetResult();
	}

	/// <summary>
	/// 生产
	/// </summary>
	/// <param name="Topic">主题</param>
	/// <param name="Key">键</param>
	/// <param name="Content">值</param>
	/// <returns>统一出参</returns>
	public OperateResult Produce(string Topic, string Key, byte[] Content)
	{
		return ProduceAsync(Topic, Key, Content).GetAwaiter().GetResult();
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
		string text = default(string);
		if (!((ResultModel)(await ((MqAbstract<KafkaOperate, KafkaData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ProduceAsync", 295);
		}
		return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(await ProduceAsync(topic, null, content, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ProduceAsync", 297);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((MqAbstract<KafkaOperate, KafkaData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "OnAsync", 308);
			}
			if (producerConfig == null)
			{
				producerConfig = new ProducerConfig
				{
					ClientId = ExtendMethod.ToUpperNString(Guid.NewGuid()),
					BootstrapServers = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.BootstrapServers,
					EnableIdempotence = true,
					SecurityProtocol = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SecurityProtocol,
					SaslMechanism = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SaslMechanism,
					SaslKerberosServiceName = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SaslKerberosServiceName
				};
				producer = new ProducerBuilder<string, byte[]>((IEnumerable<KeyValuePair<string, string>>)producerConfig).Build();
			}
			if (consumerConfig == null)
			{
				consumerConfig = BuildConsumerConfig();
				consumer = new ConsumerBuilder<string, byte[]>((IEnumerable<KeyValuePair<string, string>>)consumerConfig).Build();
			}
			if (adminClientConfig == null)
			{
				adminClientConfig = new AdminClientConfig
				{
					BootstrapServers = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.BootstrapServers,
					SecurityProtocol = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SecurityProtocol,
					SaslMechanism = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SaslMechanism,
					SaslKerberosServiceName = ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).basics.SaslKerberosServiceName
				};
				adminClient = new AdminClientBuilder((IEnumerable<KeyValuePair<string, string>>)adminClientConfig).Build();
			}
			IsOpen = true;
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "OnAsync", 346);
		}
		catch (Exception ex)
		{
			await ((MqAbstract<KafkaOperate, KafkaData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "OnAsync", 351);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		string text = default(string);
		if (!hardClose && !((ResultModel)(await ((MqAbstract<KafkaOperate, KafkaData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "OffAsync", 363);
		}
		Exception disposeException = null;
		producerConfig = null;
		try
		{
			((IDisposable)producer)?.Dispose();
		}
		catch (Exception ex)
		{
			if (disposeException == null)
			{
				disposeException = ex;
			}
		}
		producer = null;
		try
		{
			await AsyncLock.WaitAsync(token);
			try
			{
				if (tokenSource != null)
				{
					await tokenSource.CancelAsync();
					tokenSource.Dispose();
				}
				tokenSource = null;
				consumerConfig = null;
				try
				{
					consumer?.Close();
				}
				catch (Exception ex2)
				{
					if (disposeException == null)
					{
						disposeException = ex2;
					}
				}
				try
				{
					((IDisposable)consumer)?.Dispose();
				}
				catch (Exception ex3)
				{
					if (disposeException == null)
					{
						disposeException = ex3;
					}
				}
				consumer = null;
				TopicArray.Clear();
			}
			finally
			{
				AsyncLock.Release();
			}
		}
		catch (Exception ex4)
		{
			if (disposeException == null)
			{
				disposeException = ex4;
			}
		}
		adminClientConfig = null;
		try
		{
			((IDisposable)adminClient)?.Dispose();
		}
		catch (Exception ex5)
		{
			if (disposeException == null)
			{
				disposeException = ex5;
			}
		}
		adminClient = null;
		IsOpen = false;
		if (disposeException != null)
		{
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, disposeException.Message, (object)null, disposeException, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "OffAsync", 406);
		}
		return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "OffAsync", 407);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "ConsumeAsync");
		try
		{
			if (!IsOpen)
			{
				return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, "未打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ConsumeAsync", 417);
			}
			bool success = true;
			string resultMessage = null;
			await AsyncLock.WaitAsync(token);
			try
			{
				if (!IsOpen)
				{
					success = false;
					resultMessage = "未打开";
				}
				else if (!TopicArray.TryAdd(topic, value: true))
				{
					string text = topic + " 此主题已订阅";
					resultMessage = text;
					success = false;
				}
				else
				{
					if (consumer == null)
					{
						consumer = new ConsumerBuilder<string, byte[]>((IEnumerable<KeyValuePair<string, string>>)(consumerConfig ?? (consumerConfig = BuildConsumerConfig()))).Build();
					}
					consumer.Subscribe((IEnumerable<string>)TopicArray.Keys.ToList());
					if (tokenSource == null)
					{
						tokenSource = new CancellationTokenSource();
						PollingAsync(tokenSource).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			finally
			{
				AsyncLock.Release();
			}
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(success, resultMessage, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ConsumeAsync", 450);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "ConsumeAsync", 454);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "UnConsumeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<KafkaOperate, KafkaData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "UnConsumeAsync", 465);
			}
			bool success = true;
			string resultMessage = null;
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
					TopicArray.TryRemove(topic, out var _);
					if (!TopicArray.IsEmpty)
					{
						consumer?.Subscribe((IEnumerable<string>)TopicArray.Keys.ToList());
					}
					else
					{
						if (tokenSource != null)
						{
							await tokenSource.CancelAsync();
							tokenSource.Dispose();
							tokenSource = null;
						}
						consumer?.Unsubscribe();
						consumer?.Close();
						((IDisposable)consumer)?.Dispose();
						consumer = null;
						consumerConfig = null;
					}
				}
			}
			finally
			{
				AsyncLock.Release();
			}
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(success, resultMessage, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "UnConsumeAsync", 509);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", "UnConsumeAsync", 513);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		bool isOpen = IsOpen;
		string text = (IsOpen ? "已连接" : "未连接");
		return await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).EndOperateAsync(isOpen, text, (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Kafka\\KafkaOperate.cs", await ((CoreUnify<KafkaOperate, KafkaData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync"), 519);
	}
}
