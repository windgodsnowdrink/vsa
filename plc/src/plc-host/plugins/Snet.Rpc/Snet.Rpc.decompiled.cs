using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ImpromptuInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Snet.Core.extend;
using Snet.Model.data;
using Snet.Rpc.client;
using Snet.Rpc.data;
using Snet.Rpc.@interface;
using Snet.Rpc.service;
using Snet.Rpc.unility;
using Snet.Utility;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("扩展工具：Rpc（基于DotNetty实现的RPC，远程过程调用）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+235ed4cfb645efd1cc69f90ecfd5c1e74df940f1")]
[assembly: AssemblyProduct("Extend")]
[assembly: AssemblyTitle("Snet.Rpc")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Rpc.unility
{
	/// <summary>
	/// RPC 异步等待管理器，基于 <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> 和 <see cref="T:System.Threading.AutoResetEvent" /> 实现请求-响应的同步等待机制。
	/// <para>流程：Start(注册等待) → 发送请求 → Wait(阻塞等待) → Set(远端响应到达后唤醒)。</para>
	/// </summary>
	public class Await
	{
		/// <summary>
		/// 容器
		/// </summary>
		private ConcurrentDictionary<string, AwaitData> WaitIoc { get; set; } = new ConcurrentDictionary<string, AwaitData>();

		/// <summary>
		/// 注册一个新的等待标识，若该标识尚不存在则创建 <see cref="T:Snet.Rpc.data.AwaitData" /> 实例并添加到容器中。
		/// </summary>
		/// <param name="tag">等待标识（通常为通道 ID）</param>
		public void Start(string tag)
		{
			if (!WaitIoc.ContainsKey(tag))
			{
				AwaitData awaitData = new AwaitData();
				WaitIoc.AddOrUpdate(tag, awaitData, (string k, AwaitData v) => awaitData);
			}
		}

		/// <summary>
		/// 设置指定标识的等待结果并唤醒等待线程。
		/// <para>当远端响应到达后调用此方法，将结果数据写入 <see cref="P:Snet.Rpc.data.AwaitData.resultData" /> 并通过 <see cref="!:AutoResetEvent.Set" /> 唤醒等待线程。</para>
		/// </summary>
		/// <param name="tag">等待标识（通常为通道 ID）</param>
		/// <param name="rData">远端返回的 JSON 响应数据</param>
		/// <exception cref="T:System.Exception">当指定标识不存在时抛出</exception>
		public void Set(string tag, string rData)
		{
			if (WaitIoc.TryGetValue(tag, out AwaitData value))
			{
				value.resultData = rData;
				value.WaitHandler.Set();
				return;
			}
			throw new Exception("TAG(" + tag + ")不存在");
		}

		/// <summary>
		/// 阻塞等待指定标识的响应数据，等待完成后自动从容器中移除该标识。
		/// <para>通过 <see cref="!:AutoResetEvent.WaitOne()" /> 阻塞当前线程，直到 <see cref="M:Snet.Rpc.unility.Await.Set(System.String,System.String)" /> 方法被调用。</para>
		/// </summary>
		/// <param name="tag">等待标识（通常为通道 ID）</param>
		/// <returns>包含响应数据的 <see cref="T:Snet.Rpc.data.AwaitData" /> 对象</returns>
		/// <exception cref="T:System.Exception">当指定标识不存在时抛出</exception>
		public AwaitData Wait(string tag)
		{
			if (WaitIoc.TryGetValue(tag, out AwaitData value))
			{
				value.WaitHandler.WaitOne();
				WaitIoc.TryRemove(tag, out AwaitData _);
				return value;
			}
			throw new Exception("TAG(" + tag + ")不存在");
		}
	}
	/// <summary>
	/// RPC 动态代理类，继承自 <see cref="T:System.Dynamic.DynamicObject" />，用于拦截接口方法调用并通过 DotNetty 通道发送 RPC 请求。
	/// <para>采用单例模式管理代理实例，每个 <see cref="T:Snet.Rpc.data.ProxyData.Basics" /> 对应一个代理对象。</para>
	/// <para>调用流程：拦截方法调用 → 序列化为 JSON 请求 → 通过 DotNetty 通道发送 → 等待响应 → 反序列化返回值。</para>
	/// </summary>
	public class Proxy : DynamicObject
	{
		/// <summary>
		/// 锁
		/// </summary>
		private static readonly object Lock = new object();

		/// <summary>
		/// 自身对象集合
		/// </summary>
		private static List<Proxy> ThisObjList = new List<Proxy>();

		/// <summary>
		/// 基础数据
		/// </summary>
		private ProxyData.Basics basics;

		/// <summary>
		/// 获取或创建代理实例（线程安全的双重检查锁定单例模式）。
		/// <para>根据 <see cref="T:Snet.Rpc.data.ProxyData.Basics" /> 的比较结果复用已有实例，避免重复创建。</para>
		/// </summary>
		/// <param name="basics">代理所需的基础数据（通道、接口名称、类型等）</param>
		/// <returns>匹配的代理实例</returns>
		public static Proxy Instance(ProxyData.Basics basics)
		{
			Proxy proxy = ThisObjList.FirstOrDefault((Proxy c) => ExtendMethod.Comparer<ProxyData.Basics>(c.basics, basics, (string[])null).Item1);
			if (proxy == null)
			{
				lock (Lock)
				{
					if (ThisObjList.Count((Proxy c) => ExtendMethod.Comparer<ProxyData.Basics>(c.basics, basics, (string[])null).Item1) > 0)
					{
						return ThisObjList.First((Proxy c) => ExtendMethod.Comparer<ProxyData.Basics>(c.basics, basics, (string[])null).Item1);
					}
					Proxy proxy2 = new Proxy(basics);
					ThisObjList.Add(proxy2);
					return proxy2;
				}
			}
			return proxy;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public Proxy(ProxyData.Basics basics)
		{
			this.basics = basics;
		}

		/// <summary>
		/// 拦截动态方法调用，将其序列化为 RPC 请求并通过 DotNetty 通道发送到远端。
		/// <para>流程：构建请求 → JSON 序列化 → 发送 → 阻塞等待响应 → 反序列化返回值。</para>
		/// <para>超时或异常时通过 <see cref="M:Snet.Rpc.client.RpcClient.Exception(System.Exception)" /> 或 <see cref="M:Snet.Rpc.service.RpcService.Exception(System.Exception)" /> 回调通知。</para>
		/// </summary>
		/// <param name="binder">方法调用绑定器，包含方法名称等信息</param>
		/// <param name="args">方法调用参数</param>
		/// <param name="result">方法返回值（反序列化后的对象）</param>
		/// <returns>调用成功返回 true，失败或异常返回 false</returns>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			result = null;
			try
			{
				string tag = basics.channel.Id.AsShortText();
				basics.Await.Start(tag);
				Request request = new Request
				{
					IName = basics.iName,
					MName = binder.Name,
					Params = args.ToList()
				};
				IByteBuffer val = Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)request)));
				basics.channel.WriteAndFlushAsync((object)val).GetAwaiter().GetResult();
				Response response = JsonConvert.DeserializeObject<Response>(basics.Await.Wait(tag).resultData);
				if (response == null)
				{
					if (basics.Main is RpcClient rpcClient)
					{
						rpcClient?.Exception(new Exception("超时未响应"));
					}
					else if (basics.Main is RpcService rpcService)
					{
						rpcService?.Exception(new Exception("超时未响应"));
					}
				}
				else
				{
					if (response.status)
					{
						System.Type type = basics.type?.GetMethod(binder.Name)?.ReturnType;
						if (type != typeof(void))
						{
							result = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(response.data), type);
						}
						return true;
					}
					if (basics.Main is RpcClient rpcClient2)
					{
						rpcClient2?.Exception(new Exception("异常错误消息：" + response.info));
					}
					else if (basics.Main is RpcService rpcService2)
					{
						rpcService2?.Exception(new Exception("异常错误消息：" + response.info));
					}
				}
			}
			catch (Exception ex)
			{
				if (basics.Main is RpcClient rpcClient3)
				{
					rpcClient3?.Exception(ex);
				}
				else if (basics.Main is RpcService rpcService3)
				{
					rpcService3?.Exception(ex);
				}
			}
			return false;
		}
	}
	/// <summary>
	/// RPC 工具类，提供网络地址解析等辅助方法。
	/// </summary>
	public class Tool
	{
		/// <summary>
		/// 从 <see cref="T:System.Net.EndPoint" /> 中提取 IPv4 地址和端口号。
		/// <para>将 IPv6 映射格式（如 [::ffff:127.0.0.1]:8080）解析为 IPv4:Port 格式。</para>
		/// </summary>
		/// <param name="Ip">网络端点对象</param>
		/// <returns>解析后的 IP:Port 字符串，解析失败时返回空字符串</returns>
		public static string IpHandle(EndPoint Ip)
		{
			string result = string.Empty;
			string text = Ip.ToString();
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] array = text.Replace("[", "").Replace("]", "").Split(':');
				if (array.Length >= 5)
				{
					result = array[3] + ":" + array[4];
				}
			}
			return result;
		}
	}
}
namespace Snet.Rpc.service
{
	/// <summary>
	/// RPC 服务端<br />
	/// <br />
	/// 功能：<br />
	/// - 基于 DotNetty 实现高性能 TCP RPC 服务<br />
	/// - 支持客户端身份认证（账号密码 + 唯一标识 + 接口校验）<br />
	/// - 支持接口注册、反射调用和代理创建<br />
	/// - 多客户端并发管理
	/// </summary>
	public class RpcService : CoreUnify<RpcService, Service.Basics>, IRpc, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// DotNetty ServerBootstrap 服务端实例
		/// </summary>
		private ServerBootstrap? server;

		/// <summary>
		/// 连接接受事件循环组（Boss Group）
		/// </summary>
		private MultithreadEventLoopGroup? ClientGroup;

		/// <summary>
		/// 消息处理事件循环组（Worker Group）
		/// </summary>
		private MultithreadEventLoopGroup? ClientMessageGroup;

		/// <summary>
		/// 服务端监听通道
		/// </summary>
		private IChannel? Channel;

		/// <summary>
		/// 已认证的客户端信息（接口名称列表 -&gt; 通道）
		/// </summary>
		public ConcurrentDictionary<List<Details>, IChannel> clients = new ConcurrentDictionary<List<Details>, IChannel>();

		/// <summary>
		/// 异步等待管理器（管理请求/响应的同步等待）
		/// </summary>
		private Await Await = new Await();

		/// <summary>
		/// 接口注册表（接口名称 -&gt; 实现类型）
		/// </summary>
		private Dictionary<string, System.Type> iRegister { get; set; } = new Dictionary<string, System.Type>();

		/// <summary>
		/// 已创建的接口代理缓存（按接口名索引）
		/// </summary>
		private ConcurrentDictionary<string, object> creates { get; } = new ConcurrentDictionary<string, object>();

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据（包含端口、账号密码、客户端信息等）</param>
		public RpcService(Service.Basics basics)
			: base(basics)
		{
		}

		/// <inheritdoc />
		public async Task<OperateResult> OpenAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OpenAsync");
			try
			{
				if (server != null)
				{
					return await base.EndOperateAsync(false, "已打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "OpenAsync", 84);
				}
				RpcService rpcService = this;
				ServerBootstrap val = new ServerBootstrap();
				RpcService rpcService2 = this;
				MultithreadEventLoopGroup val2 = new MultithreadEventLoopGroup();
				MultithreadEventLoopGroup val3 = val2;
				rpcService2.ClientGroup = val2;
				MultithreadEventLoopGroup obj = val3;
				RpcService rpcService3 = this;
				MultithreadEventLoopGroup val4 = new MultithreadEventLoopGroup();
				val3 = val4;
				rpcService3.ClientMessageGroup = val4;
				rpcService.server = ((AbstractBootstrap<ServerBootstrap, IServerChannel>)(object)((AbstractBootstrap<ServerBootstrap, IServerChannel>)(object)val.Group((IEventLoopGroup)(object)obj, (IEventLoopGroup)(object)val3)).Channel<TcpServerSocketChannel>()).Option<int>(ChannelOption.SoBacklog, 100).ChildHandler((IChannelHandler)(object)new ActionChannelInitializer<IChannel>((Action<IChannel>)delegate(IChannel channel)
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Expected O, but got Unknown
					//IL_0027: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Expected O, but got Unknown
					IChannelPipeline pipeline = channel.Pipeline;
					pipeline.AddLast("framing-enc", (IChannelHandler)new LengthFieldPrepender(8));
					pipeline.AddLast("framing-dec", (IChannelHandler)new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 8, 0, 8));
					pipeline.AddLast((IChannelHandler[])(object)new IChannelHandler[1] { (IChannelHandler)new RpcServiceHandler(this) });
				}));
				Channel = await ((AbstractBootstrap<ServerBootstrap, IServerChannel>)(object)server).BindAsync(base.basics.Port);
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "OpenAsync", 102);
			}
			catch (Exception ex)
			{
				await CloseAsync(hardClose: true, token);
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "OpenAsync", 108);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> CloseAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "CloseAsync");
			try
			{
				if (!hardClose && server == null)
				{
					return await base.EndOperateAsync(false, "未打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "CloseAsync", 121);
				}
				MultithreadEventLoopGroup? clientGroup = ClientGroup;
				await ((clientGroup != null) ? ((AbstractEventExecutorGroup)clientGroup).ShutdownGracefullyAsync() : null);
				MultithreadEventLoopGroup? clientMessageGroup = ClientMessageGroup;
				await ((clientMessageGroup != null) ? ((AbstractEventExecutorGroup)clientMessageGroup).ShutdownGracefullyAsync() : null);
				IChannel? channel = Channel;
				await ((channel != null) ? channel.CloseAsync() : null);
				IChannel? channel2 = Channel;
				await ((channel2 != null) ? channel2.DisconnectAsync() : null);
				server = null;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "CloseAsync", 131);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "CloseAsync", 135);
			}
		}

		/// <summary>
		/// 创建接口代理对象<br />
		/// 通过客户端认证信息检索对应通道，创建动态代理
		/// </summary>
		/// <typeparam name="T">目标接口类型</typeparam>
		/// <returns>接口代理实例</returns>
		public T Create<T>() where T : class
		{
			T cre = null;
			string name = typeof(T).Name;
			if (creates.ContainsKey(name))
			{
				cre = (T)creates[name];
			}
			else
			{
				IChannel value = clients.FirstOrDefault<KeyValuePair<List<Details>, IChannel>>((KeyValuePair<List<Details>, IChannel> c) => c.Key.Contains(c.Key.FirstOrDefault((Details x) => x.INames.Equals(typeof(T).Name)))).Value;
				Proxy proxy = Proxy.Instance(new ProxyData.Basics
				{
					Main = this,
					Await = Await,
					channel = value,
					iName = name,
					type = typeof(T)
				});
				cre = Impromptu.ActLike<T>((object)proxy, Array.Empty<System.Type>());
				creates.AddOrUpdate(name, cre, (string k, object v) => cre);
			}
			return cre;
		}

		/// <inheritdoc />
		public async Task<OperateResult> RegisterAsync<I, O>(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "RegisterAsync");
			try
			{
				iRegister.Add(typeof(I).Name, typeof(O));
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "RegisterAsync", 182);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\service\\RpcService.cs", "RegisterAsync", 186);
			}
		}

		/// <summary>
		/// 处理客户端请求数据<br />
		/// 根据消息类型分发：请求、响应、认证、消息
		/// </summary>
		/// <param name="data">接收到的字节缓冲区</param>
		/// <param name="channel">通信通道</param>
		public void Response(IByteBuffer data, IChannel channel)
		{
			Response response = new Response();
			try
			{
				Types tAG = ExtendMethod.ToJsonEntity<Snet.Rpc.data.Type>(data.ToString(Encoding.UTF8), (JsonSerializerOptions)null).TAG;
				switch (tAG)
				{
				case Types.request:
				{
					Tool.IpHandle(channel.RemoteAddress);
					Request request = ExtendMethod.ToJsonEntity<Request>(data.ToString(Encoding.UTF8), (JsonSerializerOptions)null);
					if (!iRegister.ContainsKey(request.IName))
					{
						response.info = "接口不存在";
						channel.WriteAndFlushAsync((object)response);
						break;
					}
					System.Type type = iRegister[request.IName];
					object obj = Activator.CreateInstance(type);
					MethodInfo method = type.GetMethod(request.MName);
					if (method == null)
					{
						response.info = "未找到该方法";
						channel.WriteAndFlushAsync((object)response);
						break;
					}
					object[] array = request.Params.ToArray();
					ParameterInfo[] parameters = method.GetParameters();
					for (int num = 0; num < parameters.Length; num++)
					{
						if (array[num].GetType() != parameters[num].ParameterType)
						{
							array[num] = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(array[num]), parameters[num].ParameterType);
						}
					}
					object data2 = method.Invoke(obj, array);
					response.info = "请求成功";
					response.status = true;
					response.data = data2;
					channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)response))));
					break;
				}
				case Types.response:
					Await.Set(channel.Id.AsShortText(), data.ToString(Encoding.UTF8));
					break;
				case Types.authentication:
				{
					Authentication authentication = ExtendMethod.ToJsonEntity<Authentication>(data.ToString(Encoding.UTF8), (JsonSerializerOptions)null);
					if (base.basics.UserName == authentication.UserName && base.basics.Password == authentication.Password)
					{
						if (base.basics.Infos.Select((Service.Info c) => c.ISn == authentication.ISn).First())
						{
							if (base.basics.Infos.Select((Service.Info c) => ExtendMethod.Comparer<List<Details>>(c.INs, authentication.INs, (string[])null).Item1).First())
							{
								channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)new Message
								{
									Info = "认证成功",
									State = true
								}) ?? string.Empty)));
								clients.AddOrUpdate(base.basics.Infos.FirstOrDefault((Service.Info c) => c.ISn == authentication.ISn && ExtendMethod.Comparer<List<Details>>(c.INs, authentication.INs, (string[])null).Item1).INs, channel, (List<Details> k, IChannel v) => channel);
							}
							else
							{
								channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)new Message
								{
									Info = "认证失败，接口信息不一致",
									State = false
								}))));
								channel.CloseAsync();
							}
						}
						else
						{
							channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)new Message
							{
								Info = "认证失败，此唯一标识不存在",
								State = false
							}))));
							channel.CloseAsync();
						}
					}
					else
					{
						channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)new Message
						{
							Info = "认证失败，账号密码错误",
							State = false
						}))));
						channel.CloseAsync();
					}
					break;
				}
				case Types.message:
					break;
				}
			}
			catch (Exception ex)
			{
				response.info = ex.Message;
				channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)response))));
			}
		}

		/// <summary>
		/// 处理异常信息，招出异常事件
		/// </summary>
		/// <param name="ex">异常对象</param>
		public void Exception(Exception ex)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			base.OnInfoEventHandler((object)this, new EventInfoResult(false, ex.Message));
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			CloseAsync().GetAwaiter().GetResult();
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await CloseAsync();
			await base.DisposeAsync();
		}
	}
	/// <summary>
	/// RPC 服务端通道处理器，继承自 <see cref="T:DotNetty.Transport.Channels.ChannelHandlerAdapter" />，负责处理 DotNetty 通道的读取、异常等事件。
	/// <para>接收到数据时调用 <see cref="M:Snet.Rpc.service.RpcService.Response(DotNetty.Buffers.IByteBuffer,DotNetty.Transport.Channels.IChannel)" /> 处理请求，异常时调用 <see cref="M:Snet.Rpc.service.RpcService.Exception(System.Exception)" /> 通知上层。</para>
	/// </summary>
	public class RpcServiceHandler : ChannelHandlerAdapter
	{
		/// <summary>
		/// 关联的 RPC 服务端实例
		/// </summary>
		private RpcService rpcService { get; }

		/// <summary>
		/// 初始化 RPC 服务端通道处理器。
		/// </summary>
		/// <param name="rPCServer">关联的 RPC 服务端实例</param>
		public RpcServiceHandler(RpcService rPCServer)
		{
			rpcService = rPCServer;
		}

		/// <summary>
		/// 通道读取事件，将接收到的字节缓冲区转发给 <see cref="M:Snet.Rpc.service.RpcService.Response(DotNetty.Buffers.IByteBuffer,DotNetty.Transport.Channels.IChannel)" /> 进行处理。
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="message">接收到的消息（<see cref="T:DotNetty.Buffers.IByteBuffer" />）</param>
		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			rpcService.Response((IByteBuffer)((message is IByteBuffer) ? message : null), context.Channel);
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
		/// 通道异常捕获事件，通知 RPC 服务端并关闭通道。
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="exception">捕获到的异常</param>
		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			rpcService.Exception(exception);
			context.CloseAsync();
		}
	}
}
namespace Snet.Rpc.@interface
{
	/// <summary>
	/// RPC 通信接口<br />
	/// 定义客户端和服务端的公共操作契约
	/// </summary>
	public interface IRpc : IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 注册接口与实现类的映射关系
		/// </summary>
		/// <typeparam name="I">接口类型</typeparam>
		/// <typeparam name="O">实现类型</typeparam>
		/// <param name="token">取消通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> RegisterAsync<I, O>(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 打开 RPC 连接
		/// </summary>
		/// <param name="token">取消通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> OpenAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 关闭 RPC 连接
		/// </summary>
		/// <param name="hardClose">是否强制关闭</param>
		/// <param name="token">取消通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> CloseAsync(bool hardClose = false, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 创建接口代理对象，用于透明调用远程服务方法
		/// </summary>
		/// <typeparam name="T">目标接口类型</typeparam>
		/// <returns>接口代理实例</returns>
		T Create<T>() where T : class;

		/// <summary>
		/// 处理接收到的数据响应
		/// </summary>
		/// <param name="data">字节缓冲区数据</param>
		/// <param name="channel">通信通道</param>
		void Response(IByteBuffer data, IChannel channel);

		/// <summary>
		/// 处理异常信息
		/// </summary>
		/// <param name="ex">异常对象</param>
		void Exception(Exception ex);
	}
}
namespace Snet.Rpc.data
{
	/// <summary>
	/// RPC 身份认证数据模型，客户端连接时发送给服务端进行身份验证。
	/// <para>包含用户名、密码、唯一标识 (ISn) 和可提供的接口列表 (INs)。</para>
	/// </summary>
	public class Authentication
	{
		/// <summary>
		/// 标识
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Types TAG = Types.authentication;

		/// <summary>
		/// 用户名
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// 唯一标识
		/// </summary>
		public string ISn { get; set; }

		/// <summary>
		/// 接口名称集合
		/// </summary>
		public List<Details> INs { get; set; }
	}
	/// <summary>
	/// RPC 等待传递数据模型，封装 <see cref="T:System.Threading.AutoResetEvent" /> 同步信号和响应结果字符串，用于请求-响应的同步等待机制。
	/// </summary>
	public class AwaitData
	{
		/// <summary>
		/// 等待处理
		/// </summary>
		public AutoResetEvent WaitHandler { get; set; } = new AutoResetEvent(initialState: false);

		/// <summary>
		/// 返回的数据
		/// </summary>
		public string resultData { get; set; }
	}
	/// <summary>
	/// RPC 客户端数据配置类
	/// </summary>
	public class Client
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// Ip地址
			/// </summary>
			[Category("基础数据")]
			[Description("Ip地址")]
			public string IpAddress { get; set; } = "127.0.0.1";

			/// <summary>
			/// 端口
			/// </summary>
			[Description("端口")]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 超时时间
			/// </summary>
			[Description("超时时间")]
			public int TimeOut { get; set; } = 1000;

			/// <summary>
			/// 用户名
			/// </summary>
			[Description("用户名")]
			public string UserName { get; set; } = "ysai";

			/// <summary>
			/// 密码
			/// </summary>
			[Description("密码")]
			public string Password { get; set; } = "ysai";

			/// <summary>
			/// 唯一标识
			/// </summary>
			[Description("唯一标识")]
			public string ISn { get; set; } = "888888";

			/// <summary>
			/// 接口名称集合
			/// </summary>
			[Description("接口名称集合")]
			public List<Details> INs { get; set; } = new List<Details>();
		}
	}
	/// <summary>
	/// RPC 接口详情数据模型，用于身份认证时描述客户端可提供的接口信息。
	/// </summary>
	public class Details
	{
		/// <summary>
		/// 接口名称
		/// </summary>
		[Category("基础数据")]
		[Description("接口名称")]
		public string INames { get; set; } = "Interface Name";
	}
	/// <summary>
	/// RPC 消息数据模型，用于在客户端与服务端之间传递通知性消息（如连接状态变更、认证结果等）。
	/// </summary>
	public class Message
	{
		/// <summary>
		/// 标识
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Types TAG = Types.message;

		/// <summary>
		/// 信息
		/// </summary>
		public string Info { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public bool State { get; set; }

		/// <summary>
		/// 时间
		/// </summary>
		public string Time { get; set; } = ExtendMethod.ToDateTimeString(DateTime.Now);
	}
	/// <summary>
	/// RPC 动态代理数据容器，包含代理运行所需的基础数据（通道、接口名称、类型信息等）。
	/// </summary>
	public class ProxyData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 控制端对象
			/// </summary>
			public object Main { get; set; }

			/// <summary>
			/// 通道
			/// </summary>
			public IChannel channel { get; set; }

			/// <summary>
			/// 接口名称
			/// </summary>
			public string iName { get; set; }

			/// <summary>
			/// 结构类型
			/// </summary>
			public System.Type type { get; set; }

			/// <summary>
			/// 等待任务
			/// </summary>
			public Await Await { get; set; }
		}
	}
	/// <summary>
	/// RPC 请求数据模型，封装远程方法调用的接口名称、方法名称和参数列表。
	/// <para>由客户端动态代理 <see cref="T:Snet.Rpc.unility.Proxy" /> 构建，通过 DotNetty 通道发送到远端。</para>
	/// </summary>
	public class Request
	{
		/// <summary>
		/// 标识
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Types TAG;

		/// <summary>
		/// 接口名称
		/// </summary>
		public string IName { get; set; }

		/// <summary>
		/// 方法名称
		/// </summary>
		public string MName { get; set; }

		/// <summary>
		/// 参数
		/// </summary>
		public List<object> Params { get; set; }
	}
	/// <summary>
	/// RPC 响应数据模型，封装远程方法调用的返回结果（状态、信息、数据及时间戳）。
	/// <para>由远端处理请求后构建，通过 DotNetty 通道返回给调用方。</para>
	/// </summary>
	public class Response
	{
		/// <summary>
		/// 标识符
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Types TAG = Types.response;

		/// <summary>
		/// info
		/// </summary>
		public string info { get; set; }

		/// <summary>
		/// 成功还是失败
		/// </summary>
		public bool status { get; set; }

		/// <summary>
		/// 数据
		/// </summary>
		public object? data { get; set; }

		/// <summary>
		/// 发送端时间
		/// </summary>
		public string time { get; set; } = ExtendMethod.ToDateTimeString(DateTime.Now);

		public Response()
		{
		}

		public Response(string info, bool status = false, object? data = null)
		{
			this.info = info;
			this.status = status;
			this.data = data;
		}
	}
	/// <summary>
	/// RPC 服务端数据配置类
	/// </summary>
	public class Service
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 端口
			/// </summary>
			[Category("基础数据")]
			[Description("端口")]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 超时时间
			/// </summary>
			[Description("超时时间")]
			public int TimeOut { get; set; } = 1000;

			/// <summary>
			/// 用户名
			/// </summary>
			[Description("用户名")]
			public string UserName { get; set; } = "snet";

			/// <summary>
			/// 密码
			/// </summary>
			[Description("密码")]
			public string Password { get; set; } = "snet";

			/// <summary>
			/// 信息
			/// </summary>
			[Description("信息")]
			public List<Info> Infos { get; set; } = new List<Info>();
		}

		/// <summary>
		/// 用户信息
		/// </summary>
		public class Info
		{
			/// <summary>
			/// 唯一标识
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识")]
			public string ISn { get; set; } = "888888";

			/// <summary>
			/// 接口名称集合
			/// </summary>
			[Description("接口名称集合")]
			public List<Details> INs { get; set; } = new List<Details>();
		}
	}
	/// <summary>
	/// RPC 消息类型基类，所有 RPC 数据模型（请求、响应、认证、消息）均包含 <see cref="T:Snet.Rpc.data.Types" /> TAG 标识，用于反序列化时区分消息类型。
	/// </summary>
	public class Type
	{
		/// <summary>
		/// 标识
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Types TAG { get; set; }
	}
	/// <summary>
	/// 类型枚举
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum Types
	{
		/// <summary>
		/// 请求
		/// </summary>
		request,
		/// <summary>
		/// 响应
		/// </summary>
		response,
		/// <summary>
		/// 身份认证
		/// </summary>
		authentication,
		/// <summary>
		/// 消息
		/// </summary>
		message
	}
}
namespace Snet.Rpc.client
{
	/// <summary>
	/// RPC 客户端<br />
	/// <br />
	/// 功能：<br />
	/// - 基于 DotNetty 实现高性能 TCP RPC 通信<br />
	/// - 支持接口代理透明调用远程服务<br />
	/// - 支持身份认证和接口注册<br />
	/// - 支持同步等待异步响应
	/// </summary>
	public class RpcClient : CoreUnify<RpcClient, Client.Basics>, IRpc, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// DotNetty Bootstrap 客户端实例
		/// </summary>
		private Bootstrap? client;

		/// <summary>
		/// 客户端事件循环组（管理 I/O 线程）
		/// </summary>
		private MultithreadEventLoopGroup? ClientGroup;

		/// <summary>
		/// 与服务端的通信通道
		/// </summary>
		private IChannel? Channel;

		/// <summary>
		/// 异步等待管理器（管理请求/响应的同步等待）
		/// </summary>
		private Await Await = new Await();

		/// <summary>
		/// 身份认证标识（用于匹配认证响应）
		/// </summary>
		private string AuthenticationTag = ExtendMethod.ToUpperNString(Guid.NewGuid());

		/// <summary>
		/// 已创建的接口代理缓存（按接口名索引）
		/// </summary>
		private ConcurrentDictionary<string, object> creates { get; } = new ConcurrentDictionary<string, object>();

		/// <summary>
		/// 接口注册表（接口名称 -&gt; 实现类型）
		/// </summary>
		private Dictionary<string, System.Type> iRegister { get; set; } = new Dictionary<string, System.Type>();

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据（包含服务器地址、端口、账号密码等）</param>
		public RpcClient(Client.Basics basics)
			: base(basics)
		{
		}

		/// <inheritdoc />
		public async Task<OperateResult> OpenAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OpenAsync");
			try
			{
				if (client != null)
				{
					return await base.EndOperateAsync(false, "已打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "OpenAsync", 81);
				}
				RpcClient rpcClient = this;
				Bootstrap val = new Bootstrap();
				RpcClient rpcClient2 = this;
				MultithreadEventLoopGroup val2 = new MultithreadEventLoopGroup();
				MultithreadEventLoopGroup val3 = val2;
				rpcClient2.ClientGroup = val2;
				rpcClient.client = ((AbstractBootstrap<Bootstrap, IChannel>)(object)((AbstractBootstrap<Bootstrap, IChannel>)(object)((AbstractBootstrap<Bootstrap, IChannel>)(object)((AbstractBootstrap<Bootstrap, IChannel>)val).Group((IEventLoopGroup)(object)val3)).Channel<TcpSocketChannel>()).Option<bool>(ChannelOption.TcpNodelay, true)).Handler((IChannelHandler)(object)new ActionChannelInitializer<ISocketChannel>((Action<ISocketChannel>)delegate(ISocketChannel channel)
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Expected O, but got Unknown
					//IL_0027: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Expected O, but got Unknown
					IChannelPipeline pipeline = ((IChannel)channel).Pipeline;
					pipeline.AddLast("framing-enc", (IChannelHandler)new LengthFieldPrepender(8));
					pipeline.AddLast("framing-dec", (IChannelHandler)new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 8, 0, 8));
					pipeline.AddLast((IChannelHandler[])(object)new IChannelHandler[1] { (IChannelHandler)new RpcClientHandler(this) });
				}));
				Channel = await client.ConnectAsync((EndPoint)new IPEndPoint(IPAddress.Parse(base.basics.IpAddress), base.basics.Port));
				Await.Start(AuthenticationTag);
				await Channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)new Authentication
				{
					UserName = base.basics.UserName,
					Password = base.basics.Password,
					ISn = base.basics.ISn,
					INs = base.basics.INs
				}))));
				Message message = JsonConvert.DeserializeObject<Message>(Await.Wait(AuthenticationTag).resultData);
				if (message == null)
				{
					await CloseAsync(hardClose: true, token);
					return await base.EndOperateAsync(false, "服务器未响应认证信息", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "OpenAsync", 110);
				}
				return await base.EndOperateAsync(message.State, message.Info, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "OpenAsync", 112);
			}
			catch (Exception ex)
			{
				await CloseAsync(hardClose: true, token);
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "OpenAsync", 117);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> CloseAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "CloseAsync");
			try
			{
				if (!hardClose && client == null)
				{
					return await base.EndOperateAsync(false, "未打开", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "CloseAsync", 130);
				}
				MultithreadEventLoopGroup? clientGroup = ClientGroup;
				await ((clientGroup != null) ? ((AbstractEventExecutorGroup)clientGroup).ShutdownGracefullyAsync() : null);
				IChannel? channel = Channel;
				await ((channel != null) ? channel.CloseAsync() : null);
				IChannel? channel2 = Channel;
				await ((channel2 != null) ? channel2.DisconnectAsync() : null);
				client = null;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "CloseAsync", 138);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "CloseAsync", 142);
			}
		}

		/// <summary>
		/// 创建接口代理对象<br />
		/// 通过动态代理透明调用远程服务方法，自动缓存已创建的代理
		/// </summary>
		/// <typeparam name="T">目标接口类型</typeparam>
		/// <returns>接口代理实例</returns>
		public T Create<T>() where T : class
		{
			T cre = null;
			string name = typeof(T).Name;
			if (creates.ContainsKey(name))
			{
				cre = (T)creates[name];
			}
			else
			{
				Proxy proxy = Proxy.Instance(new ProxyData.Basics
				{
					Main = this,
					Await = Await,
					channel = Channel,
					iName = name,
					type = typeof(T)
				});
				cre = Impromptu.ActLike<T>((object)proxy, Array.Empty<System.Type>());
				creates.AddOrUpdate(name, cre, (string k, object v) => cre);
			}
			return cre;
		}

		/// <inheritdoc />
		public async Task<OperateResult> RegisterAsync<I, O>(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "RegisterAsync");
			try
			{
				iRegister.Add(typeof(I).Name, typeof(O));
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "RegisterAsync", 187);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\Rpc\\Snet.Rpc\\client\\RpcClient.cs", "RegisterAsync", 191);
			}
		}

		/// <summary>
		/// 处理服务端响应数据<br />
		/// 根据消息类型分发：请求、响应、认证、消息
		/// </summary>
		/// <param name="data">接收到的字节缓冲区</param>
		/// <param name="channel">通信通道</param>
		public void Response(IByteBuffer data, IChannel channel)
		{
			Response response = new Response();
			try
			{
				switch (ExtendMethod.ToJsonEntity<Snet.Rpc.data.Type>(data.ToString(Encoding.UTF8), (JsonSerializerOptions)null).TAG)
				{
				case Types.request:
				{
					Tool.IpHandle(channel.RemoteAddress);
					Request request = ExtendMethod.ToJsonEntity<Request>(data.ToString(Encoding.UTF8), (JsonSerializerOptions)null);
					if (!iRegister.ContainsKey(request.IName))
					{
						response.info = "接口不存在";
						channel.WriteAndFlushAsync((object)response);
						break;
					}
					System.Type type = iRegister[request.IName];
					object obj = Activator.CreateInstance(type);
					MethodInfo method = type.GetMethod(request.MName);
					if (method == null)
					{
						response.info = "未找到该方法";
						channel.WriteAndFlushAsync((object)response);
						break;
					}
					object[] array = request.Params.ToArray();
					ParameterInfo[] parameters = method.GetParameters();
					for (int i = 0; i < parameters.Length; i++)
					{
						if (array[i].GetType() != parameters[i].ParameterType)
						{
							array[i] = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(array[i]), parameters[i].ParameterType);
						}
					}
					object data2 = method.Invoke(obj, array);
					response.info = "请求成功";
					response.status = true;
					response.data = data2;
					channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)response))));
					break;
				}
				case Types.response:
					Await.Set(channel.Id.AsShortText(), data.ToString(Encoding.UTF8));
					break;
				case Types.message:
					Await.Set(AuthenticationTag, data.ToString(Encoding.UTF8));
					break;
				case Types.authentication:
					break;
				}
			}
			catch (Exception ex)
			{
				response.info = ex.Message;
				channel.WriteAndFlushAsync((object)Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)response))));
			}
		}

		/// <summary>
		/// 处理异常信息，招出事件并释放资源
		/// </summary>
		/// <param name="ex">异常对象</param>
		public void Exception(Exception ex)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			base.OnInfoEventHandler((object)this, new EventInfoResult(false, ex.Message));
			((CoreUnify<RpcClient, Client.Basics>)(object)this).Dispose();
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			CloseAsync().GetAwaiter().GetResult();
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await CloseAsync();
			await base.DisposeAsync();
		}
	}
	/// <summary>
	/// RPC 客户端通道处理器，继承自 <see cref="T:DotNetty.Transport.Channels.ChannelHandlerAdapter" />，负责处理 DotNetty 通道的读取、异常等事件。
	/// <para>接收到数据时调用 <see cref="M:Snet.Rpc.client.RpcClient.Response(DotNetty.Buffers.IByteBuffer,DotNetty.Transport.Channels.IChannel)" /> 处理响应，异常时调用 <see cref="M:Snet.Rpc.client.RpcClient.Exception(System.Exception)" /> 通知上层。</para>
	/// </summary>
	public class RpcClientHandler : ChannelHandlerAdapter
	{
		/// <summary>
		/// 关联的 RPC 客户端实例
		/// </summary>
		private readonly RpcClient rpcClient;

		/// <summary>
		/// 初始化 RPC 客户端通道处理器。
		/// </summary>
		/// <param name="rpcClient">关联的 RPC 客户端实例</param>
		public RpcClientHandler(RpcClient rpcClient)
		{
			this.rpcClient = rpcClient;
		}

		/// <summary>
		/// 通道读取事件，将接收到的字节缓冲区转发给 <see cref="M:Snet.Rpc.client.RpcClient.Response(DotNetty.Buffers.IByteBuffer,DotNetty.Transport.Channels.IChannel)" /> 进行处理。
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="message">接收到的消息（<see cref="T:DotNetty.Buffers.IByteBuffer" />）</param>
		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			rpcClient.Response((IByteBuffer)((message is IByteBuffer) ? message : null), context.Channel);
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
		/// 通道异常捕获事件，通知 RPC 客户端并关闭通道。
		/// </summary>
		/// <param name="context">通道上下文</param>
		/// <param name="exception">捕获到的异常</param>
		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			rpcClient.Exception(exception);
			context.CloseAsync();
		}
	}
}
