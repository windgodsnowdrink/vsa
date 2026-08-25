using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
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
[assembly: AssemblyDescription("传输协议：NetMQ（支持数据的生产与消费）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Mq")]
[assembly: AssemblyTitle("Snet.NetMQ")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.NetMQ;

/// <summary>
/// NetMQ (ZeroMQ) 数据配置类，封装发布/订阅模式的连接参数（地址、超时、使用模式等）。
/// </summary>
public class NetMQData
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
		/// 使用模式
		/// </summary>
		[Description("使用模式")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public UseModel UModel { get; set; }

		/// <summary>
		/// 地址；
		/// 通过 UModel 来确定模式
		/// </summary>
		[Description("地址")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? Address { get; set; }

		/// <summary>
		/// 超时时间
		/// </summary>
		[Description("超时时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int TimeOut { get; set; }

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
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			UModel = UseModel.PubModel;
			Address = "tcp://127.0.0.1:8866";
			TimeOut = 1000;
			ResponseType = (ResponseType)1;
			base..ctor();
		}
	}

	/// <summary>
	/// 使用类型
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum UseModel
	{
		/// <summary>
		/// 使用订阅模式；<br />
		/// Connect 方式；<br />
		/// 多 [ 订阅 ] 对 一 [ 发布 ]
		/// </summary>
		[Description("使用订阅模式")]
		SubModel,
		/// <summary>
		/// 使用发布模式；<br />
		/// Bind 方式；<br />
		/// 一 [ 发布 ] 对 多 [ 订阅 ]
		/// </summary>
		[Description("使用发布模式")]
		PubModel
	}
}
/// <summary>
/// NetMQ 操作类<br />
/// 基于轻量级消息传递库 ZeroMQ 的 100% 原生 C# 实现<br />
/// 支持发布/订阅模式：一个发布者对应多个订阅者
/// </summary>
public class NetMQOperate : MqAbstract<NetMQOperate, NetMQData.Basics>, IMq, IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// NetMQ 套接字实例（发布或订阅，根据 UModel 决定类型）
	/// </summary>
	private NetMQSocket? netMQ;

	/// <summary>
	/// 消息处理任务的生命周期控制令牌
	/// </summary>
	private CancellationTokenSource? g_token;

	/// <inheritdoc />
	protected override string CN => "NetMQ";

	/// <inheritdoc />
	protected override string CD => "一个功能强大、易于使用且性能优越的消息代理，适用于各种需要高性能消息传递的.NET应用场景";

	/// <summary>
	/// 有参构造函数
	/// </summary>
	/// <param name="basics">基础数据</param>
	public NetMQOperate(NetMQData.Basics basics)
		: base(basics)
	{
	}

	/// <summary>
	/// 无参构造函数
	/// </summary>
	public NetMQOperate()
	{
	}

	/// <summary>
	/// 订阅消息处理循环<br />
	/// 持续从 SubscriberSocket 接收消息，按帧语义解析后根据响应类型分发事件<br />
	/// 使用超时机制避免无限阻塞
	/// </summary>
	/// <param name="token">取消令牌源，控制循环生命周期</param>
	/// <returns>异步任务</returns>
	private async Task SubMessageHandleAsync(CancellationTokenSource token)
	{
		string text = default(string);
		bool flag = default(bool);
		byte[] array2 = default(byte[]);
		bool flag2 = default(bool);
		while (!token.IsCancellationRequested)
		{
			try
			{
				NetMQSocket? obj = netMQ;
				SubscriberSocket val = (SubscriberSocket)(object)((obj is SubscriberSocket) ? obj : null);
				if (val == null)
				{
					await Task.Delay(100, token.Token);
					continue;
				}
				TimeSpan timeSpan = TimeSpan.FromMilliseconds(((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).basics.TimeOut);
				string text2;
				byte[] array;
				if (ReceivingSocketExtensions.TryReceiveFrameString((IReceivingSocket)(object)val, timeSpan, ref text, ref flag))
				{
					text2 = (flag ? text : string.Empty);
					if (!flag)
					{
						array = Encoding.UTF8.GetBytes(text ?? string.Empty);
						goto IL_013c;
					}
					if (ReceivingSocketExtensions.TryReceiveFrameBytes((IReceivingSocket)(object)val, timeSpan, ref array2, ref flag2))
					{
						array = array2 ?? Array.Empty<byte>();
						goto IL_013c;
					}
				}
				goto end_IL_004f;
				IL_013c:
				string text3 = ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).TAG + " 接收到 ( " + text2 + " ) 主题消息";
				ResponseType responseType = ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).basics.ResponseType;
				switch ((int)responseType)
				{
				case 0:
					await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text3, (object)array));
					goto end_IL_004f;
				case 1:
					await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text3, (object)Encoding.UTF8.GetString(array)));
					goto end_IL_004f;
				case 2:
					await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text3, (object)new ResponseModel(text2, (object)Encoding.UTF8.GetString(array))));
					goto end_IL_004f;
				default:
					goto end_IL_004f;
				}
				end_IL_004f:;
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception ex2)
			{
				if (!token.IsCancellationRequested)
				{
					try
					{
						await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费异常:" + ex2.Message));
					}
					catch
					{
					}
					await Task.Delay(100, token.Token);
					continue;
				}
				break;
			}
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((MqAbstract<NetMQOperate, NetMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "OnAsync", 127);
			}
			if (string.IsNullOrWhiteSpace(((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).basics.Address))
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, "地址不能为空", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "OnAsync", 129);
			}
			if (netMQ == null)
			{
				switch (((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).basics.UModel)
				{
				case NetMQData.UseModel.SubModel:
					netMQ = (NetMQSocket?)new SubscriberSocket((string)null);
					netMQ.Connect(((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).basics.Address);
					if (g_token == null)
					{
						g_token = new CancellationTokenSource();
						SubMessageHandleAsync(g_token).ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				case NetMQData.UseModel.PubModel:
					netMQ = (NetMQSocket?)new PublisherSocket((string)null);
					netMQ.Bind(((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).basics.Address);
					break;
				}
			}
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "OnAsync", 151);
		}
		catch (Exception ex)
		{
			await ((MqAbstract<NetMQOperate, NetMQData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "OnAsync", 156);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((MqAbstract<NetMQOperate, NetMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "OffAsync", 170);
			}
			if (g_token != null)
			{
				await g_token.CancelAsync();
				g_token.Dispose();
				g_token = null;
			}
			if (netMQ != null)
			{
				netMQ.Dispose();
				netMQ = null;
			}
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "OffAsync", 185);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "OffAsync", 189);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<NetMQOperate, NetMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ProduceAsync", 201);
			}
			NetMQSocket? obj = netMQ;
			PublisherSocket val = (PublisherSocket)(object)((obj is PublisherSocket) ? obj : null);
			if (val == null)
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, "未使用发布模式", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ProduceAsync", 205);
			}
			OutgoingSocketExtensions.SendFrame(OutgoingSocketExtensions.SendMoreFrame((IOutgoingSocket)(object)val, topic), content, false);
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ProduceAsync", 208);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ProduceAsync", 212);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).BegOperateAsync(token, "ConsumeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<NetMQOperate, NetMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ConsumeAsync", 224);
			}
			NetMQSocket? obj = netMQ;
			SubscriberSocket val = (SubscriberSocket)(object)((obj is SubscriberSocket) ? obj : null);
			if (val == null)
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, "未使用订阅模式", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ConsumeAsync", 228);
			}
			val.Subscribe(topic);
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ConsumeAsync", 231);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "ConsumeAsync", 235);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).BegOperateAsync(token, "UnConsumeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<NetMQOperate, NetMQData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "UnConsumeAsync", 247);
			}
			NetMQSocket? obj = netMQ;
			SubscriberSocket val = (SubscriberSocket)(object)((obj is SubscriberSocket) ? obj : null);
			if (val == null)
			{
				return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, "未使用订阅模式", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "UnConsumeAsync", 251);
			}
			val.Unsubscribe(topic);
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "UnConsumeAsync", 254);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", "UnConsumeAsync", 258);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		bool flag = netMQ != null;
		string text = ((netMQ != null) ? "已连接" : "未连接");
		return await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).EndOperateAsync(flag, text, (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.NetMQ\\NetMQOperate.cs", await ((CoreUnify<NetMQOperate, NetMQData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync"), 265);
	}
}
