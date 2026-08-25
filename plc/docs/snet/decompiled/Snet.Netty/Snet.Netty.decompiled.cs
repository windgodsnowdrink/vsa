using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Concurrency;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using DotNetty.Transport.Channels.Sockets;
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
[assembly: AssemblyDescription("传输协议：Netty（支持数据的生产与消费）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Mq")]
[assembly: AssemblyTitle("Snet.Netty")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Netty.service
{
	/// <summary>
	/// Netty 服务端数据配置类，封装 DotNetty TCP 服务器的连接参数（端口、SSL 证书路径及密码等）。
	/// </summary>
	public class NettyServiceData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 端口
			/// </summary>
			[Description("端口")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// SSL文件路径
			/// </summary>
			[Description("SSL文件路径")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? SslFilePath { get; set; }

			/// <summary>
			/// SSL文件密码
			/// </summary>
			[Description("SSL文件密码")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? SslFilePassword { get; set; }
		}
	}
	/// <summary>
	/// Netty 服务端通道处理器，继承自 <see cref="T:DotNetty.Transport.Channels.ChannelHandlerAdapter" />，负责处理 DotNetty 服务端通道的激活、读取和异常事件。
	/// </summary>
	public class NettyServiceHandler : ChannelHandlerAdapter
	{
		/// <summary>
		/// 关联的 Netty 服务端操作实例
		/// </summary>
		private NettyServiceOperate nettyServiceOperate { get; }

		/// <summary>
		/// 初始化 Netty 服务端通道处理器。
		/// </summary>
		/// <param name="server">关联的服务端操作实例</param>
		public NettyServiceHandler(NettyServiceOperate server)
		{
			nettyServiceOperate = server;
		}

		/// <summary>
		/// 通道激活
		/// </summary>
		/// <param name="context"></param>
		public override void ChannelActive(IChannelHandlerContext context)
		{
			nettyServiceOperate.Active(context);
		}

		/// <summary>
		/// 通道读取
		/// </summary>
		/// <param name="context"></param>
		/// <param name="message">消息</param>
		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			nettyServiceOperate.Response(context, message);
		}

		public override void ChannelReadComplete(IChannelHandlerContext context)
		{
			context.Flush();
		}

		/// <summary>
		/// 通道异常
		/// </summary>
		/// <param name="context"></param>
		/// <param name="exception"></param>
		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			nettyServiceOperate.Exception(context, exception);
		}
	}
	/// <summary>
	/// 群发送至连上服务器的所有客户端，过滤掉发送端
	/// </summary>
	public class NettyServiceChannelMatcherHandler : IChannelMatcher
	{
		private readonly IChannelId id;

		/// <summary>
		/// 群发送至连上服务器的所有客户端，过滤掉发送端
		/// </summary>
		/// <param name="id"></param>
		public NettyServiceChannelMatcherHandler(IChannelId id)
		{
			this.id = id;
		}

		public bool Matches(IChannel channel)
		{
			return channel.Id != id;
		}
	}
	/// <summary>
	/// netty 操作类
	/// </summary>
	public class NettyServiceOperate : CoreUnify<NettyServiceOperate, NettyServiceData.Basics>, IOn, IOff, IStatus, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 客户端组
		/// </summary>
		private MultithreadEventLoopGroup? ClientGroup;

		/// <summary>
		/// 客户端消息组
		/// </summary>
		private MultithreadEventLoopGroup? ClientMessageGroup;

		/// <summary>
		/// 通信库
		/// </summary>
		private ServerBootstrap? Communication;

		/// <summary>
		/// 通道
		/// </summary>
		private IChannel? Channel;

		/// <summary>
		/// 客户端容器
		/// </summary>
		private volatile ConcurrentDictionary<string, IChannel> ClientIoc = new ConcurrentDictionary<string, IChannel>();

		/// <summary>
		/// 组
		/// </summary>
		private volatile IChannelGroup? ChannelGroup;

		/// <summary>
		/// 是否打开
		/// </summary>
		private bool IsStart;

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public NettyServiceOperate(NettyServiceData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 激活，内部使用
		/// </summary>
		/// <param name="context"></param>
		public void Active(IChannelHandlerContext context)
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			if (ChannelGroup == null)
			{
				ChannelGroup = (IChannelGroup?)new DefaultChannelGroup(context.Executor);
			}
			((ICollection<IChannel>)ChannelGroup).Add(context.Channel);
			string text = IpHandle(context.Channel.RemoteAddress);
			ClientIoc.AddOrUpdate(text, context.Channel, (string k, IChannel v) => context.Channel);
			base.OnInfoEventHandler((object)this, new EventInfoResult(true, text + " 激活了通道"));
		}

		/// <summary>
		/// 响应，内部使用（群发并过滤发送端，避免消息回显给发送者）
		/// </summary>
		public void Response(IChannelHandlerContext context, object message)
		{
			ChannelGroup.WriteAndFlushAsync(message, (IChannelMatcher)(object)new NettyServiceChannelMatcherHandler(context.Channel.Id));
		}

		/// <summary>
		/// 异常，内部使用
		/// </summary>
		public void Exception(IChannelHandlerContext context, Exception exception)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			((ICollection<IChannel>)ChannelGroup).Remove(context.Channel);
			string key = IpHandle(context.Channel.RemoteAddress);
			ClientIoc.Remove(key, out var _);
			base.OnInfoEventHandler((object)this, new EventInfoResult(false, $"{context.Channel.RemoteAddress} {exception.Message}"));
			context.CloseAsync();
		}

		/// <summary>
		/// IP处理（兼容 IPv4 与 IPv6）
		/// </summary>
		/// <param name="Ip"></param>
		/// <returns></returns>
		private string IpHandle(EndPoint Ip)
		{
			if (Ip is IPEndPoint iPEndPoint)
			{
				return $"{iPEndPoint.Address}:{iPEndPoint.Port}";
			}
			return Ip.ToString() ?? string.Empty;
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			Off(hardClose: true);
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await OffAsync(hardClose: true);
			await base.DisposeAsync();
		}

		/// <inheritdoc />
		public OperateResult On()
		{
			return OnAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 关闭 Netty 服务端<br />
		/// 优雅关闭事件循环组，关闭通道并释放资源
		/// </summary>
		/// <param name="hardClose">是否强制关闭（跳过状态检查）</param>
		/// <returns>操作结果</returns>
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 获取 Netty 服务端运行状态
		/// </summary>
		/// <returns>操作结果（Status=true 表示已启动）</returns>
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			bool isStart = IsStart;
			string text = (IsStart ? "已启动" : "未启动");
			return await base.EndOperateAsync(isStart, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", await base.BegOperateAsync(token, "GetStatusAsync"), 154);
		}

		/// <inheritdoc />
		public async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OffAsync");
			try
			{
				if (!hardClose && Channel == null)
				{
					return await base.EndOperateAsync(false, "未启动", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "OffAsync", 166);
				}
				MultithreadEventLoopGroup? clientGroup = ClientGroup;
				await ((clientGroup != null) ? ((AbstractEventExecutorGroup)clientGroup).ShutdownGracefullyAsync() : null);
				MultithreadEventLoopGroup? clientMessageGroup = ClientMessageGroup;
				await ((clientMessageGroup != null) ? ((AbstractEventExecutorGroup)clientMessageGroup).ShutdownGracefullyAsync() : null);
				IChannel? channel = Channel;
				await ((channel != null) ? channel.CloseAsync() : null);
				Channel = null;
				Communication = null;
				ClientIoc.Clear();
				ChannelGroup = null;
				IsStart = false;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "OffAsync", 180);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "OffAsync", 184);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OnAsync");
			try
			{
				if (Channel != null)
				{
					return await base.EndOperateAsync(false, "已启动", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "OnAsync", 198);
				}
				X509Certificate2 Cert = null;
				_ = string.Empty;
				if (!string.IsNullOrWhiteSpace(base.basics.SslFilePath) && !string.IsNullOrWhiteSpace(base.basics.SslFilePassword))
				{
					Cert = new X509Certificate2(base.basics.SslFilePath, base.basics.SslFilePassword);
					Cert.GetNameInfo(X509NameType.DnsName, forIssuer: false);
				}
				Communication = new ServerBootstrap();
				ClientGroup = new MultithreadEventLoopGroup(1);
				ClientMessageGroup = new MultithreadEventLoopGroup();
				((AbstractBootstrap<ServerBootstrap, IServerChannel>)(object)((AbstractBootstrap<ServerBootstrap, IServerChannel>)(object)((AbstractBootstrap<ServerBootstrap, IServerChannel>)(object)Communication.Group((IEventLoopGroup)(object)ClientGroup, (IEventLoopGroup)(object)ClientMessageGroup)).Channel<TcpServerSocketChannel>()).Option<int>(ChannelOption.SoBacklog, 128)).Handler((IChannelHandler)new LoggingHandler((LogLevel)2)).ChildHandler((IChannelHandler)(object)new ActionChannelInitializer<ISocketChannel>((Action<ISocketChannel>)delegate(ISocketChannel channel)
				{
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_003b: Expected O, but got Unknown
					//IL_004b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0055: Expected O, but got Unknown
					IChannelPipeline pipeline = ((IChannel)channel).Pipeline;
					if (Cert != null)
					{
						pipeline.AddLast((IChannelHandler[])(object)new IChannelHandler[1] { (IChannelHandler)TlsHandler.Server((X509Certificate)Cert) });
					}
					pipeline.AddLast("framing-enc", (IChannelHandler)new LengthFieldPrepender(8));
					pipeline.AddLast("framing-dec", (IChannelHandler)new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 8, 0, 8));
					pipeline.AddLast((IChannelHandler[])(object)new IChannelHandler[1] { (IChannelHandler)new NettyServiceHandler(this) });
				}));
				Channel = await ((AbstractBootstrap<ServerBootstrap, IServerChannel>)(object)Communication).BindAsync(base.basics.Port);
				IsStart = true;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "OnAsync", 238);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true, token);
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "OnAsync", 243);
			}
		}

		/// <summary>
		/// 发送数据到客户端<br />
		/// 支持指定 IP:Port 定向发送，或不指定时群发到所有客户端
		/// </summary>
		/// <param name="Data">要发送的字节数据</param>
		/// <param name="IpPort">目标客户端 IP:Port 数组（为 null 时群发）</param>
		/// <returns>操作结果</returns>
		public OperateResult Send(byte[] Data, string[]? IpPort = null)
		{
			return SendAsync(Data, IpPort).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步发送数据到客户端（委托同步方法）
		/// </summary>
		/// <param name="Data">要发送的字节数据</param>
		/// <param name="IpPort">目标客户端 IP:Port 数组（为 null 时群发）</param>
		/// <param name="token">取消令牌</param>
		/// <returns>操作结果</returns>
		public async Task<OperateResult> SendAsync(byte[] Data, string[]? IpPort = null, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "SendAsync");
			try
			{
				if (Channel == null)
				{
					return await base.EndOperateAsync(false, "未启动", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 271);
				}
				if (ChannelGroup != null)
				{
					if (IpPort != null)
					{
						List<string> FailMessage = new List<string>();
						foreach (string text in IpPort)
						{
							if (ClientIoc.TryGetValue(text, out IChannel value))
							{
								await value.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Data));
							}
							else
							{
								FailMessage.Add(text + " 终端不存在");
							}
						}
						if (FailMessage.Count > 0)
						{
							return await base.EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 291);
						}
						return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 293);
					}
					await ChannelGroup.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Data));
					return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 299);
				}
				return await base.EndOperateAsync(false, "当前没有客户端连接", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 304);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 309);
			}
		}

		/// <summary>
		/// 群发数据
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="Content">内容</param>
		/// <param name="IpPort">IP端口</param>
		/// <returns></returns>
		public OperateResult Send(string Topic, string Content, string[]? IpPort = null)
		{
			return SendAsync(Topic, Content, IpPort).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 群发数据
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="Content">内容</param>
		/// <param name="IpPort">IP端口</param>
		/// <returns></returns>
		public async Task<OperateResult> SendAsync(string Topic, string Content, string[]? IpPort = null, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "SendAsync");
			try
			{
				if (Channel == null)
				{
					return await base.EndOperateAsync(false, "未启动", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 337);
				}
				var anon = new
				{
					T = Topic,
					C = Content
				};
				return await base.EndOperateAsync(await SendAsync(Encoding.UTF8.GetBytes(ExtendMethod.ToJson(anon, false)), IpPort, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 343);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\service\\NettyServiceOperate.cs", "SendAsync", 347);
			}
		}
	}
}
namespace Snet.Netty.client
{
	/// <summary>
	/// Netty 客户端数据配置类
	/// </summary>
	public class NettyClientData
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
			/// SSL文件路径
			/// </summary>
			[Description("SSL文件路径")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? SslFilePath { get; set; }

			/// <summary>
			/// SSL文件密码
			/// </summary>
			[Description("SSL文件密码")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? SslFilePassword { get; set; }

			/// <summary>
			/// 任务数量
			/// </summary>
			[Description("任务数量")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int TaskNumber { get; set; }

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
				IpAddress = "127.0.0.1";
				Port = 6688;
				TaskNumber = 5;
				ResponseType = (ResponseType)1;
				base..ctor();
			}
		}
	}
	/// <summary>
	/// Netty 客户端通道处理器，继承自 <see cref="T:DotNetty.Transport.Channels.ChannelHandlerAdapter" />，负责处理 DotNetty 客户端通道的读取和异常事件。
	/// </summary>
	public class NettyClientHandler : ChannelHandlerAdapter
	{
		/// <summary>
		/// 关联的 Netty 客户端操作实例
		/// </summary>
		private readonly NettyClientOperate nettyClientOperate;

		/// <summary>
		/// 初始化 Netty 客户端通道处理器。
		/// </summary>
		/// <param name="nettyClientOperate">关联的客户端操作实例</param>
		public NettyClientHandler(NettyClientOperate nettyClientOperate)
		{
			this.nettyClientOperate = nettyClientOperate;
		}

		/// <summary>
		/// 通道读取事件，将接收到的消息转发给 <see cref="M:Snet.Netty.client.NettyClientOperate.Response(DotNetty.Transport.Channels.IChannelHandlerContext,System.Object)" /> 处理。
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="message">接收到的消息</param>
		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			nettyClientOperate.Response(context, message);
		}

		/// <summary>
		/// 通道读取完成事件，刷新输出缓冲区。
		/// </summary>
		/// <param name="context">通道上下文</param>
		public override void ChannelReadComplete(IChannelHandlerContext context)
		{
			context.Flush();
		}

		/// <summary>
		/// 通道异常捕获事件，通知客户端操作实例处理异常。
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="exception">捕获到的异常</param>
		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			nettyClientOperate.Exception(context, exception);
		}
	}
	/// <summary>
	/// Netty 客户端操作类<br />
	/// <br />
	/// 功能：<br />
	/// - 基于 DotNetty 实现高性能 TCP 客户端通信<br />
	/// - 支持 SSL/TLS 加密传输<br />
	/// - 支持主题订阅和消息发布<br />
	/// - 多任务并发消费队列
	/// </summary>
	public class NettyClientOperate : MqAbstract<NettyClientOperate, NettyClientData.Basics>, IMq, IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 内部消息
		/// </summary>
		private class M
		{
			/// <summary>
			/// 主题
			/// </summary>
			public string T { get; set; }

			/// <summary>
			/// 内容
			/// </summary>
			public string C { get; set; }
		}

		/// <summary>
		/// 信息
		/// </summary>
		public class Info
		{
			/// <summary>
			/// 通道处理程序上下文
			/// </summary>
			public IChannelHandlerContext Context { get; set; }

			/// <summary>
			/// 信息
			/// </summary>
			public string Msg { get; set; }
		}

		/// <summary>
		/// 通信库
		/// </summary>
		private Bootstrap? Communication;

		/// <summary>
		/// 组
		/// </summary>
		private MultithreadEventLoopGroup? Group;

		/// <summary>
		/// 通道
		/// </summary>
		private IChannel? Channel;

		/// <summary>
		/// 任务集合
		/// </summary>
		private ConcurrentDictionary<CancellationTokenSource, Task>? TaskArray;

		/// <summary>
		/// 数据队列
		/// </summary>
		private Channel<Info>? DataQueue;

		/// <summary>
		/// 订阅的主题项容器
		/// </summary>
		private ConcurrentDictionary<string, DateTime> SubIoc = new ConcurrentDictionary<string, DateTime>();

		private BoundedChannelOptions? p_Channel;

		/// <inheritdoc />
		protected override string CN => "Netty";

		/// <inheritdoc />
		protected override string CD => "一个功能强大、性能优越且易于使用的网络应用程序框架，广泛应用于各种需要高性能网络通信的场景";

		/// <summary>
		/// 通道设置
		/// </summary>
		private BoundedChannelOptions channelOptions
		{
			get
			{
				BoundedChannelOptions boundedChannelOptions = p_Channel;
				if (boundedChannelOptions == null)
				{
					BoundedChannelOptions obj = new BoundedChannelOptions(65535)
					{
						FullMode = BoundedChannelFullMode.Wait,
						SingleReader = false,
						SingleWriter = false
					};
					BoundedChannelOptions boundedChannelOptions2 = obj;
					p_Channel = obj;
					boundedChannelOptions = boundedChannelOptions2;
				}
				return boundedChannelOptions;
			}
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public NettyClientOperate(NettyClientData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public NettyClientOperate()
		{
		}

		/// <summary>
		/// 任务处理
		/// </summary>
		/// <param name="token">任务令牌</param>
		/// <returns>任务</returns>
		private Task TaskHandleAsync(CancellationToken token)
		{
			return Task.Run(async delegate
			{
				try
				{
					while (await DataQueue.Reader.WaitToReadAsync(token).ConfigureAwait(continueOnCapturedContext: false))
					{
						Info item;
						while (DataQueue.Reader.TryRead(out item))
						{
							if (item != null && !token.IsCancellationRequested && !string.IsNullOrEmpty(item.Msg))
							{
								try
								{
									string msg = item.Msg;
									if (ExtendMethod.IsJson(msg))
									{
										M m = ExtendMethod.ToJsonEntity<M>(msg, (JsonSerializerOptions)null);
										if (SubIoc.ContainsKey(m.T))
										{
											string t = m.T;
											string c = m.C;
											string text = $"{((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).TAG} 接收到主题 ( {t} ) 内容 ( {c} )";
											ResponseType responseType = ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.ResponseType;
											switch ((int)responseType)
											{
											case 0:
												await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)ExtendMethod.ToHex(c, true)));
												break;
											case 1:
												await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)c));
												break;
											case 2:
												await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)new ResponseModel(t, (object)c)));
												break;
											}
										}
									}
								}
								catch (Exception ex)
								{
									await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费异常:" + ex.Message));
								}
							}
						}
					}
				}
				catch (Exception ex2)
				{
					if (!token.IsCancellationRequested)
					{
						await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费任务异常:" + ex2.Message));
					}
				}
			}, token);
		}

		/// <summary>
		/// 接收响应数据（由 Handler 内部调用）<br />
		/// 将接收到的消息写入数据队列供消费线程处理
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="message">接收到的消息对象</param>
		public void Response(IChannelHandlerContext context, object message)
		{
			IByteBuffer val = (IByteBuffer)((message is IByteBuffer) ? message : null);
			if (val != null)
			{
				DataQueue?.Writer.TryWrite(new Info
				{
					Context = context,
					Msg = val.ToString(Encoding.UTF8)
				});
			}
		}

		/// <summary>
		/// 异常处理（由 Handler 内部调用）<br />
		/// 招出异常信息、关闭通道并断开连接
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="exception">异常信息</param>
		public void Exception(IChannelHandlerContext context, Exception exception)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).OnInfoEventHandler((object)this, new EventInfoResult(false, IpHandle(context.Channel.RemoteAddress) + " " + exception.Message));
			context.CloseAsync();
			Channel = null;
		}

		/// <summary>
		/// 解析网络地址，提取 IP:Port 字符串<br />
		/// 兼容 IPv4 与 IPv6 格式的 EndPoint 地址
		/// </summary>
		/// <param name="Ip">网络端点地址</param>
		/// <returns>IP:Port 字符串</returns>
		private string IpHandle(EndPoint Ip)
		{
			if (Ip is IPEndPoint iPEndPoint)
			{
				return $"{iPEndPoint.Address}:{iPEndPoint.Port}";
			}
			return Ip.ToString() ?? string.Empty;
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
			try
			{
				string text = default(string);
				if (((ResultModel)(await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "OnAsync", 235);
				}
				X509Certificate2 Cert = null;
				string CertTargetHost = string.Empty;
				if (!string.IsNullOrWhiteSpace(((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.SslFilePath) && !string.IsNullOrWhiteSpace(((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.SslFilePassword))
				{
					Cert = new X509Certificate2(((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.SslFilePath, ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.SslFilePassword);
					CertTargetHost = Cert.GetNameInfo(X509NameType.DnsName, forIssuer: false);
				}
				if (DataQueue == null)
				{
					DataQueue = System.Threading.Channels.Channel.CreateBounded<Info>(channelOptions);
				}
				Communication = new Bootstrap();
				Group = new MultithreadEventLoopGroup();
				((AbstractBootstrap<Bootstrap, IChannel>)(object)((AbstractBootstrap<Bootstrap, IChannel>)(object)((AbstractBootstrap<Bootstrap, IChannel>)(object)((AbstractBootstrap<Bootstrap, IChannel>)(object)Communication).Group((IEventLoopGroup)(object)Group)).Channel<TcpSocketChannel>()).Option<bool>(ChannelOption.TcpNodelay, true)).Handler((IChannelHandler)(object)new ActionChannelInitializer<ISocketChannel>((Action<ISocketChannel>)delegate(ISocketChannel channel)
				{
					//IL_0055: Unknown result type (might be due to invalid IL or missing references)
					//IL_005f: Expected O, but got Unknown
					//IL_006f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0079: Expected O, but got Unknown
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0047: Expected O, but got Unknown
					//IL_0042: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Expected O, but got Unknown
					IChannelPipeline pipeline = ((IChannel)channel).Pipeline;
					if (Cert != null)
					{
						pipeline.AddLast((IChannelHandler[])(object)new IChannelHandler[1] { (IChannelHandler)new TlsHandler((Func<Stream, SslStream>)((Stream stream) => new SslStream(stream, leaveInnerStreamOpen: true, delegate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
						{
							switch (errors)
							{
							case SslPolicyErrors.None:
								return true;
							case SslPolicyErrors.RemoteCertificateNameMismatch:
								if (certificate != null && string.Equals(certificate.Subject?.Trim(), CertTargetHost, StringComparison.OrdinalIgnoreCase))
								{
									return true;
								}
								break;
							}
							return false;
						})), (TlsSettings)new ClientTlsSettings(CertTargetHost)) });
					}
					pipeline.AddLast("framing-enc", (IChannelHandler)new LengthFieldPrepender(8));
					pipeline.AddLast("framing-dec", (IChannelHandler)new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 8, 0, 8));
					pipeline.AddLast((IChannelHandler[])(object)new IChannelHandler[1] { (IChannelHandler)new NettyClientHandler(this) });
				}));
				Channel = await Communication.ConnectAsync((EndPoint)new IPEndPoint(IPAddress.Parse(((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.IpAddress), ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.Port));
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "OnAsync", 284);
			}
			catch (Exception ex)
			{
				await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).OffAsync(true, token);
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "OnAsync", 289);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
			if (Channel == null || !Channel.Active)
			{
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, "未连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "GetStatusAsync", 298);
			}
			return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(true, "已连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "GetStatusAsync", 300);
		}

		/// <inheritdoc />
		/// <returns>操作结果</returns>
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
			try
			{
				string text = default(string);
				if (!hardClose && !((ResultModel)(await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "OffAsync", 313);
				}
				if (TaskArray != null)
				{
					foreach (KeyValuePair<CancellationTokenSource, Task> item in TaskArray)
					{
						await item.Key.CancelAsync();
						item.Key.Dispose();
					}
					TaskArray.Clear();
					TaskArray = null;
				}
				if (DataQueue != null)
				{
					Info item2;
					while (DataQueue.Reader.TryRead(out item2))
					{
					}
					DataQueue = null;
				}
				Communication = null;
				MultithreadEventLoopGroup? obj = Group;
				await ((obj != null) ? ((AbstractEventExecutorGroup)obj).ShutdownGracefullyAsync() : null);
				IChannel? channel = Channel;
				await ((channel != null) ? channel.CloseAsync() : null);
				Channel = null;
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "OffAsync", 339);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "OffAsync", 343);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> ProduceAsync(string topic, string content, Encoding? encoding = null, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ProduceAsync", 354);
				}
				if (encoding == null)
				{
					encoding = Encoding.UTF8;
				}
				var anon = new
				{
					T = topic,
					C = content
				};
				await Channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(encoding.GetBytes(ExtendMethod.ToJson(anon, false))));
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ProduceAsync", 366);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ProduceAsync", 370);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ProduceAsync", 379);
			}
			return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).ProduceAsync(topic, ExtendMethod.ToHexString(content), (Encoding)null, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ProduceAsync", 381);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).BegOperateAsync(token, "ConsumeAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ConsumeAsync", 391);
				}
				if (TaskArray == null)
				{
					TaskArray = new ConcurrentDictionary<CancellationTokenSource, Task>();
					for (int i = 0; i < ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).basics.TaskNumber; i++)
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
						TaskArray.TryAdd(cancellationTokenSource, TaskHandleAsync(cancellationTokenSource.Token));
					}
				}
				if (SubIoc.ContainsKey(topic))
				{
					return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, "主题已存在", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ConsumeAsync", 407);
				}
				SubIoc.AddOrUpdate(topic, DateTime.Now, (string k, DateTime v) => DateTime.Now);
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ConsumeAsync", 412);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "ConsumeAsync", 417);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).BegOperateAsync(token, "UnConsumeAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await ((MqAbstract<NettyClientOperate, NettyClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "UnConsumeAsync", 428);
				}
				if (SubIoc.ContainsKey(topic))
				{
					SubIoc.Remove(topic, out var _);
					return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "UnConsumeAsync", 433);
				}
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, "主题不存在", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "UnConsumeAsync", 437);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<NettyClientOperate, NettyClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Netty\\client\\NettyClientOperate.cs", "UnConsumeAsync", 442);
			}
		}
	}
}
