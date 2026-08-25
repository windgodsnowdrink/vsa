using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Internal;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
[assembly: AssemblyDescription("传输协议：Mqtt（支持数据的生产与消费）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Mq")]
[assembly: AssemblyTitle("Snet.Mqtt")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Mqtt.service
{
	/// <summary>
	/// MQTT 服务端数据配置类，封装 MQTT Broker 的连接参数（端口、认证信息、最大连接数等）。
	/// </summary>
	public class MqttServiceData
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
			/// 用户
			/// </summary>
			[Description("用户名")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? UserName { get; set; } = "shunnet";

			/// <summary>
			/// 密码
			/// </summary>
			[Description("密码")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? Password { get; set; } = "shunnet";

			/// <summary>
			/// 最大连接数
			/// </summary>
			[Description("最大连接数")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int MaxNumber { get; set; } = 10000;
		}

		/// <summary>
		/// 哪一个步骤
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum Steps
		{
			服务停止事件,
			服务启动事件,
			客户端取消订阅事件,
			客户端订阅事件,
			客户端消息事件,
			客户端断开事件,
			客户端连接事件,
			客户端身份验证事件
		}

		/// <summary>
		/// 状态
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum States
		{
			/// <summary>
			/// 已启动
			/// </summary>
			On,
			/// <summary>
			/// 已停止
			/// </summary>
			Off,
			/// <summary>
			/// 啥也没干
			/// </summary>
			Null
		}
	}
	/// <summary>
	/// MQTT 服务端操作类，基于 MQTTnet 实现 MQTT Broker 服务。
	/// <para>支持客户端连接/断开监听、主题订阅/取消订阅、消息接收等功能。</para>
	/// </summary>
	public class MqttServiceOperate : CoreUnify<MqttServiceOperate, MqttServiceData.Basics>, IOn, IOff, IStatus, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// MQTT 服务
		/// </summary>
		private MqttServer? mqttServer;

		/// <summary>
		/// 状态
		/// </summary>
		private MqttServiceData.States states = MqttServiceData.States.Null;

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public MqttServiceOperate(MqttServiceData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 服务停止后
		/// </summary>
		private async Task MqttServer_StoppedAsync(EventArgs arg)
		{
			await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.服务停止事件} ]服务已停止"));
		}

		/// <summary>
		/// 服务启动后
		/// </summary>
		private async Task MqttServer_StartedAsync(EventArgs arg)
		{
			await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.服务启动事件} ]服务已启动"));
		}

		/// <summary>
		/// 客户端取消订阅
		/// </summary>
		private async Task MqttServer_ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs arg)
		{
			await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.客户端取消订阅事件} ]( {arg.ClientId} ) 取消了 ( {arg.TopicFilter} ) 的订阅"));
		}

		/// <summary>
		/// 客户端订阅
		/// </summary>
		private async Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
		{
			await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.客户端订阅事件} ]( {arg.ClientId} ) 订阅了主题 ( {arg.TopicFilter.Topic} )"));
		}

		/// <summary>
		/// 客户端消息接收
		/// </summary>
		private async Task MqttServer_ApplicationMessageNotConsumedAsync(ApplicationMessageNotConsumedEventArgs arg)
		{
			if ((int)arg.ApplicationMessage.PayloadFormatIndicator == 1)
			{
				await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.客户端消息事件} ]( {arg.SenderId} ) 发布了主题 ( {arg.ApplicationMessage.Topic} ) 内容 ( {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)} )"));
			}
		}

		/// <summary>
		/// 客户端断开
		/// </summary>
		private async Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
		{
			await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.客户端断开事件} ]( {arg.ClientId} ) 已断开"));
		}

		/// <summary>
		/// 客户端连接
		/// </summary>
		private async Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
		{
			await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.客户端连接事件} ]( {arg.ClientId} ) 已连接"));
		}

		/// <summary>
		/// 身份验证
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		/// <exception cref="T:System.NotImplementedException"></exception>
		private async Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
		{
			if (arg.UserName != base.basics.UserName || arg.Password != base.basics.Password)
			{
				arg.ReasonCode = (MqttConnectReasonCode)134;
				await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, $"[ {MqttServiceData.Steps.客户端身份验证事件} ]( {arg.ClientId} ) 身份验证异常：{arg.ReasonCode}"));
			}
			else
			{
				arg.ReasonCode = (MqttConnectReasonCode)0;
				await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, $"[ {MqttServiceData.Steps.客户端身份验证事件} ]( {arg.ClientId} ) 身份验证成功"));
			}
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

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OnAsync");
			try
			{
				if (states == MqttServiceData.States.On)
				{
					return await base.EndOperateAsync(false, "已启动", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\MqttServiceOperate.cs", "OnAsync", 151);
				}
				MqttServerOptionsBuilder val = new MqttServerOptionsBuilder();
				val.WithDefaultEndpoint();
				val.WithDefaultEndpointPort(Convert.ToInt32(base.basics.Port));
				val.WithConnectionBacklog(base.basics.MaxNumber);
				mqttServer = new MqttServerFactory().CreateMqttServer(val.Build());
				mqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
				mqttServer.ApplicationMessageNotConsumedAsync += MqttServer_ApplicationMessageNotConsumedAsync;
				mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
				mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
				mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
				mqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;
				mqttServer.StartedAsync += MqttServer_StartedAsync;
				mqttServer.StoppedAsync += MqttServer_StoppedAsync;
				await mqttServer.StartAsync();
				states = MqttServiceData.States.On;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\MqttServiceOperate.cs", "OnAsync", 168);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true, token);
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\MqttServiceOperate.cs", "OnAsync", 173);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OffAsync");
			try
			{
				if (!hardClose && states != MqttServiceData.States.On)
				{
					return await base.EndOperateAsync(false, "未启动", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\MqttServiceOperate.cs", "OffAsync", 185);
				}
				if (mqttServer != null)
				{
					await MqttServerExtensions.StopAsync(mqttServer);
					((Disposable)mqttServer).Dispose();
					mqttServer = null;
				}
				states = MqttServiceData.States.Off;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\MqttServiceOperate.cs", "OffAsync", 194);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\MqttServiceOperate.cs", "OffAsync", 198);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			bool flag = states == MqttServiceData.States.On;
			string text = ((states == MqttServiceData.States.On) ? "已启动" : "未启动");
			return await base.EndOperateAsync(flag, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\MqttServiceOperate.cs", await base.BegOperateAsync(token, "GetStatusAsync"), 205);
		}
	}
}
namespace Snet.Mqtt.service.websocket
{
	/// <summary>
	/// MQTT WebSocket 服务端数据配置类，封装 MQTT + WebSocket 服务的连接参数（MQTT 端口、WebSocket 端口、认证信息、URI 路径等）。
	/// </summary>
	public class MqttWebSocketServiceData
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
			/// MQTT服务端口
			/// </summary>
			[Description("端口")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 端口 Mqtt WEBSOCKET 使用
			/// </summary>
			[Description("WS端口")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int WsPort { get; set; } = 8866;

			/// <summary>
			/// 用户
			/// </summary>
			[Description("用户")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? UserName { get; set; } = "shunnet";

			/// <summary>
			/// 密码
			/// </summary>
			[Description("密码")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? Password { get; set; } = "shunnet";

			/// <summary>
			/// 地址
			/// </summary>
			[Description("地址")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? Uri { get; set; } = "shun";
		}

		/// <summary>
		/// 哪一个步骤
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum Steps
		{
			客户端取消订阅事件,
			客户端订阅事件,
			客户端消息事件,
			客户端断开事件,
			客户端连接事件,
			客户端身份验证事件,
			系统信息,
			系统异常信息
		}
	}
	/// <summary>
	/// MQTT WebSocket 服务端操作类，基于 ASP.NET Core Kestrel 和 MQTTnet 实现同时支持 MQTT 和 WebSocket 协议的 Broker 服务。
	/// <para>通过 <see cref="T:Snet.Mqtt.service.websocket.MqttWebSocketServiceOperate.Startup" /> 配置 Kestrel 监听双端口，通过 <see cref="T:Snet.Mqtt.service.websocket.MqttWebSocketServiceOperate.MqttController" /> 处理 MQTT 事件。</para>
	/// </summary>
	public class MqttWebSocketServiceOperate : CoreUnify<MqttWebSocketServiceOperate, MqttWebSocketServiceData.Basics>, IOn, IOff, IStatus, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 启动
		/// </summary>
		private class Startup
		{
			/// <summary>
			/// 配置
			/// </summary>
			public void Configure(IApplicationBuilder app, IWebHostEnvironment environment, MqttController mqttController)
			{
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Expected O, but got Unknown
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Expected O, but got Unknown
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Expected O, but got Unknown
				try
				{
					app.UseRouting();
					app.UseEndpoints(delegate(IEndpointRouteBuilder endpoints)
					{
						endpoints.MapConnectionHandler<MqttConnectionHandler>(mqttServiceData_Static.Uri, (Action<HttpConnectionDispatcherOptions>?)delegate(HttpConnectionDispatcherOptions httpConnectionDispatcherOptions)
						{
							httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector = (IList<string> protocolList) => protocolList.FirstOrDefault() ?? string.Empty;
						});
					});
					ApplicationBuilderExtensions.UseMqttServer(app, (Action<MqttServer>)delegate(MqttServer server)
					{
						server.ValidatingConnectionAsync += mqttController.MqttServer_ValidatingConnectionAsync;
						server.ApplicationMessageNotConsumedAsync += mqttController.MqttServer_ApplicationMessageNotConsumedAsync;
						server.ClientConnectedAsync += mqttController.MqttServer_ClientConnectedAsync;
						server.ClientDisconnectedAsync += mqttController.MqttServer_ClientDisconnectedAsync;
						server.ClientSubscribedTopicAsync += mqttController.MqttServer_ClientSubscribedTopicAsync;
						server.ClientUnsubscribedTopicAsync += mqttController.MqttServer_ClientUnsubscribedTopicAsync;
					});
					OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.系统信息} ]MQTT 服务端地址 ( 127.0.0.1:{mqttServiceData_Static.Port} )"));
					OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.系统信息} ]MQTT WebSocket 服务端地址 ( ws://127.0.0.1:{mqttServiceData_Static.WsPort}/{mqttServiceData_Static.Uri} )"));
				}
				catch (Exception ex)
				{
					OnEventHandler_Static?.Invoke(this, new EventInfoResult(false, $"[ {MqttWebSocketServiceData.Steps.系统异常信息} ]配置服务异常：{ex.Message}"));
				}
			}

			/// <summary>
			/// 配置服务
			/// </summary>
			/// <param name="services"></param>
			public void ConfigureServices(IServiceCollection services)
			{
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Expected O, but got Unknown
				try
				{
					ServiceCollectionExtensions.AddHostedMqttServer(services, (Action<MqttServerOptionsBuilder>)delegate(MqttServerOptionsBuilder optionsBuilder)
					{
						optionsBuilder.WithDefaultEndpoint();
					});
					ServiceCollectionExtensions.AddMqttConnectionHandler(services);
					services.AddConnections();
					services.AddSingleton<MqttController>();
				}
				catch (Exception ex)
				{
					OnEventHandler_Static?.Invoke(this, new EventInfoResult(false, $"[ {MqttWebSocketServiceData.Steps.系统异常信息} ]配置服务异常：{ex.Message}"));
				}
			}
		}

		/// <summary>
		/// MQTT控制器
		/// </summary>
		private class MqttController
		{
			/// <summary>
			/// 客户端取消订阅
			/// </summary>
			public Task MqttServer_ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs arg)
			{
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Expected O, but got Unknown
				OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.客户端取消订阅事件} ]( {arg.ClientId} ) 取消了 ( {arg.TopicFilter} ) 的订阅"));
				return Task.CompletedTask;
			}

			/// <summary>
			/// 客户端订阅
			/// </summary>
			public Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
			{
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Expected O, but got Unknown
				OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.客户端订阅事件} ]( {arg.ClientId} ) 订阅了主题 ( {arg.TopicFilter.Topic} )"));
				return Task.CompletedTask;
			}

			/// <summary>
			/// 客户端消息接收
			/// </summary>
			public Task MqttServer_ApplicationMessageNotConsumedAsync(ApplicationMessageNotConsumedEventArgs arg)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Invalid comparison between Unknown and I4
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Expected O, but got Unknown
				MqttPayloadFormatIndicator payloadFormatIndicator = arg.ApplicationMessage.PayloadFormatIndicator;
				if ((int)payloadFormatIndicator != 0 && (int)payloadFormatIndicator == 1)
				{
					OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.客户端消息事件} ]( {arg.SenderId} ) 发布了主题 ( {arg.ApplicationMessage.Topic} ) 内容 ( {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)} )"));
				}
				return Task.CompletedTask;
			}

			/// <summary>
			/// 客户端断开
			/// </summary>
			public Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
			{
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Expected O, but got Unknown
				OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.客户端断开事件} ]( {arg.ClientId} ) 已断开"));
				return Task.CompletedTask;
			}

			/// <summary>
			/// 客户端连接
			/// </summary>
			public Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
			{
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Expected O, but got Unknown
				OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.客户端连接事件} ]( {arg.ClientId} ) 已连接"));
				return Task.CompletedTask;
			}

			/// <summary>
			/// 身份验证
			/// </summary>
			/// <param name="arg"></param>
			/// <returns></returns>
			/// <exception cref="T:System.NotImplementedException"></exception>
			public Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
			{
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Expected O, but got Unknown
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Expected O, but got Unknown
				if (arg.UserName != mqttServiceData_Static.UserName || arg.Password != mqttServiceData_Static.Password)
				{
					arg.ReasonCode = (MqttConnectReasonCode)134;
					OnEventHandler_Static?.Invoke(this, new EventInfoResult(false, $"[ {MqttWebSocketServiceData.Steps.客户端身份验证事件} ]( {arg.ClientId} ) 身份验证异常：{arg.ReasonCode}"));
				}
				else
				{
					arg.ReasonCode = (MqttConnectReasonCode)0;
					OnEventHandler_Static?.Invoke(this, new EventInfoResult(true, $"[ {MqttWebSocketServiceData.Steps.客户端身份验证事件} ]( {arg.ClientId} ) 身份验证成功"));
				}
				return Task.CompletedTask;
			}
		}

		private CancellationTokenSource? g_token;

		private bool IsStart;

		/// <summary>
		/// 启动时快照的 Basics（供无参构造的 Startup/MqttController 读取；启动后不再变更，多实例互不污染）
		/// </summary>
		private static MqttWebSocketServiceData.Basics? mqttServiceData_Static { get; set; }

		/// <summary>
		/// 启动时快照的事件转发（供 Startup/MqttController 使用）
		/// </summary>
		private static EventHandler<EventInfoResult>? OnEventHandler_Static { get; set; }

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public MqttWebSocketServiceOperate(MqttWebSocketServiceData.Basics basics)
			: base(basics)
		{
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

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OffAsync");
			try
			{
				if (!hardClose && g_token == null)
				{
					return await base.EndOperateAsync(false, "未连接", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\websocket\\MqttWebSocketServiceOperate.cs", "OffAsync", 76);
				}
				await (g_token?.CancelAsync());
				g_token?.Dispose();
				g_token = null;
				IsStart = false;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\websocket\\MqttWebSocketServiceOperate.cs", "OffAsync", 83);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\websocket\\MqttWebSocketServiceOperate.cs", "OffAsync", 87);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OnAsync");
			try
			{
				if (mqttServiceData_Static == null)
				{
					mqttServiceData_Static = base.basics;
					OnEventHandler_Static = base.OnInfoEventHandler;
				}
				if (!NetHandler.ClearSamePortProcess(mqttServiceData_Static.Port))
				{
					await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, $"[ {MqttWebSocketServiceData.Steps.系统信息} ]MQTT 服务端口被占用，清理占用此端口的进程失败"));
				}
				if (!NetHandler.ClearSamePortProcess(mqttServiceData_Static.WsPort))
				{
					await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, $"[ {MqttWebSocketServiceData.Steps.系统信息} ]MQTT WebSocket 服务端口被占用，清理占用此端口的进程失败"));
				}
				if (g_token == null)
				{
					g_token = new CancellationTokenSource();
					Host.CreateDefaultBuilder(Array.Empty<string>()).ConfigureWebHostDefaults(delegate(IWebHostBuilder webBuilder)
					{
						webBuilder.UseKestrel(delegate(KestrelServerOptions o)
						{
							o.ListenAnyIP(mqttServiceData_Static.Port, delegate(ListenOptions l)
							{
								ConnectionBuilderExtensions.UseMqtt((IConnectionBuilder)l);
							});
							o.ListenAnyIP(mqttServiceData_Static.WsPort);
						});
						webBuilder.UseStartup<Startup>();
					}).RunConsoleAsync(g_token.Token);
					IsStart = true;
					return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\websocket\\MqttWebSocketServiceOperate.cs", "OnAsync", 128);
				}
				return await base.EndOperateAsync(false, "已连接", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\websocket\\MqttWebSocketServiceOperate.cs", "OnAsync", 132);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true, token);
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\websocket\\MqttWebSocketServiceOperate.cs", "OnAsync", 138);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			bool isStart = IsStart;
			string text = (IsStart ? "已启动" : "未启动");
			return await base.EndOperateAsync(isStart, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\service\\websocket\\MqttWebSocketServiceOperate.cs", await base.BegOperateAsync(token, "GetStatusAsync"), 144);
		}
	}
}
namespace Snet.Mqtt.client
{
	/// <summary>
	/// mqtt 客户端数据
	/// </summary>
	public class MqttClientData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			[CompilerGenerated]
			private MqttQualityOfServiceLevel <QualityOfServiceLevel>k__BackingField;

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
			public string IpAddress { get; set; }

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
			/// 客户端ID
			/// 不输入默认生成随机
			/// </summary>
			[Description("客户端ID")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? ClientID { get; set; }

			/// <summary>
			/// 消息过期时间
			/// </summary>
			[Description("消息过期时间")]
			[Unit("ms")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int MessageExpirationTime { get; set; }

			/// <summary>
			/// QoS0 是最低级别，基本上等同于 Fire and Forget 模式，发送者发送完数据之后，不关心消息是否已经投递到了接收者那边。<br />
			/// QoS1 是中间级别，保证消息至少送达一次。MQTT 通过简单的 ACK 机制来保证 QoS1。<br />
			/// QoS2 是最高级别，保证到且仅到一次。这通过更加复杂的消息流程保证。<br />
			/// 注意：QoS 级别越高，流程越复杂，系统资源消耗越大
			/// </summary>
			[Description("QoS等级")]
			[JsonConverter(typeof(JsonStringEnumConverter))]
			[Display(/*Could not decode attribute arguments.*/)]
			public MqttQualityOfServiceLevel QualityOfServiceLevel
			{
				[CompilerGenerated]
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return <QualityOfServiceLevel>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					<QualityOfServiceLevel>k__BackingField = value;
				}
			}

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
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
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
	/// MQTT 客户端操作类<br />
	/// <br />
	/// 功能：<br />
	/// - 基于 MQTTnet 实现 MQTT 客户端连接、发布、订阅<br />
	/// - 支持 QoS 0/1/2 三种消息质量等级<br />
	/// - 支持消息保留和过期时间设置<br />
	/// - 自动处理断开重连
	/// </summary>
	public class MqttClientOperate : MqAbstract<MqttClientOperate, MqttClientData.Basics>, IMq, IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// MQTT 客户端
		/// </summary>
		private MqttClient? mqttClient;

		/// <inheritdoc />
		protected override string CN => "Mqtt";

		/// <inheritdoc />
		protected override string CD => "一个以其轻量级、高效和可靠的特点，在需要实时消息传递的场景中发挥着重要作用";

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public MqttClientOperate(MqttClientData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public MqttClientOperate()
		{
		}

		/// <summary>
		/// 服务端断开
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		private async Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "与服务端断开连接"));
			for (int attempt = 1; attempt <= 5; attempt++)
			{
				try
				{
					MqttClient client = mqttClient;
					if (client == null || client.IsConnected)
					{
						return;
					}
					await client.ConnectAsync(BuildClientOptions(), default(CancellationToken));
					if (client.IsConnected)
					{
						await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(true, "与服务端重连成功"));
						return;
					}
				}
				catch (Exception ex)
				{
					await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, $"与服务端重连失败 ( {attempt}/5 )：{ex.Message}"));
				}
				await Task.Delay(TimeSpan.FromSeconds(Math.Min(30, 5 * attempt)));
			}
			await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).OffAsync(true, default(CancellationToken));
		}

		/// <summary>
		/// 构建客户端连接选项（账号密码、keepalive 等）
		/// </summary>
		/// <returns>客户端选项</returns>
		private MqttClientOptions BuildClientOptions()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			MqttClientOptionsBuilder val = new MqttClientOptionsBuilder();
			val.WithWillQualityOfServiceLevel(((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.QualityOfServiceLevel);
			val.WithClientId(string.IsNullOrEmpty(((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.ClientID) ? ExtendMethod.ToUpperNString(Guid.NewGuid()) : ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.ClientID);
			val.WithTcpServer(((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.IpAddress, (int?)((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.Port, AddressFamily.Unspecified);
			val.WithCredentials(((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.UserName, ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.Password);
			val.WithCleanSession(true);
			val.WithKeepAlivePeriod(TimeSpan.FromSeconds(60.0));
			return val.Build();
		}

		/// <summary>
		/// 消息事件
		/// </summary>
		private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
		{
			try
			{
				string topic = arg.ApplicationMessage.Topic;
				byte[] array = arg.ApplicationMessage.Payload.ToArray<byte>();
				string clientId = arg.ClientId;
				string text = $"{((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).TAG} 接收到 ( {topic} ) 主题消息，客户端信息 ( {clientId} )";
				ResponseType responseType = ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.ResponseType;
				switch ((int)responseType)
				{
				case 0:
					await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)array));
					break;
				case 1:
					await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)Encoding.UTF8.GetString(array)));
					break;
				case 2:
					await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnDataEventHandlerAsync((object)this, new EventDataResult(true, text, (object)new ResponseModel(topic, (object)Encoding.UTF8.GetString(array))));
					break;
				}
			}
			catch (Exception ex)
			{
				await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费异常:" + ex.Message));
			}
		}

		/// <summary>
		/// 发布订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="Content">内容</param>
		/// <param name="QoSLevel">
		/// 消息等级<br />
		/// QoS 0 是最低级别，基本上等同于 Fire and Forget 模式，发送者发送完数据之后，不关心消息是否已经投递到了接收者那边。<br />
		/// QoS 1 是中间级别，保证消息至少送达一次。MQTT 通过简单的 ACK 机制来保证 QoS1。<br />
		/// QoS 2 是最高级别，保证到且仅到一次。这通过更加复杂的消息流程保证。<br />
		///             <br />
		/// 注意：QoS 级别越高，流程越复杂，系统资源消耗越大
		/// </param>
		/// <param name="Retain">消息是否保留</param>
		/// <param name="token">传播消息取消</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> PublishAsync(string Topic, string Content, int QoSLevel = 0, bool Retain = false, CancellationToken token = default(CancellationToken))
		{
			return await PublishAsync(Topic, Encoding.UTF8.GetBytes(Content), QoSLevel, Retain, token);
		}

		/// <summary>
		/// 发布订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="Content">内容</param>
		/// <param name="QoSLevel">
		/// 消息等级<br />
		/// QoS 0 是最低级别，基本上等同于 Fire and Forget 模式，发送者发送完数据之后，不关心消息是否已经投递到了接收者那边。<br />
		/// QoS 1 是中间级别，保证消息至少送达一次。MQTT 通过简单的 ACK 机制来保证 QoS1。<br />
		/// QoS 2 是最高级别，保证到且仅到一次。这通过更加复杂的消息流程保证。<br />
		///             <br />
		/// 注意：QoS 级别越高，流程越复杂，系统资源消耗越大
		/// </param>
		/// <param name="Retain">消息是否保留</param>
		/// <param name="token">传播消息取消</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> PublishAsync(string Topic, byte[] Content, int QoSLevel = 0, bool Retain = false, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "PublishAsync");
			try
			{
				if (mqttClient == null || !mqttClient.IsConnected)
				{
					return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, "未连接", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "PublishAsync", 178);
				}
				MqttApplicationMessage val = new MqttApplicationMessage();
				val.Topic = Topic;
				val.PayloadSegment = Content;
				val.QualityOfServiceLevel = (MqttQualityOfServiceLevel)QoSLevel;
				val.Retain = Retain;
				val.MessageExpiryInterval = (uint)(((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).basics.MessageExpirationTime / 1000);
				MqttClientPublishResult val2 = await mqttClient.PublishAsync(val, token);
				if (val2.IsSuccess)
				{
					return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "PublishAsync", 195);
				}
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, val2.ReasonString, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "PublishAsync", 199);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "PublishAsync", 204);
			}
		}

		/// <summary>
		/// 发布订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="Content">内容</param>
		/// <param name="QoSLevel">
		/// 消息等级<br />
		/// QoS 0 是最低级别，基本上等同于 Fire and Forget 模式，发送者发送完数据之后，不关心消息是否已经投递到了接收者那边。<br />
		/// QoS 1 是中间级别，保证消息至少送达一次。MQTT 通过简单的 ACK 机制来保证 QoS1。<br />
		/// QoS 2 是最高级别，保证到且仅到一次。这通过更加复杂的消息流程保证。<br />
		///             <br />
		/// 注意：QoS 级别越高，流程越复杂，系统资源消耗越大
		/// </param>
		/// <param name="Retain">消息是否保留</param>
		/// <returns>统一结果</returns>
		public OperateResult Publish(string Topic, string Content, int QoSLevel = 0, bool Retain = false)
		{
			return PublishAsync(Topic, Content, QoSLevel, Retain).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 发布订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="Content">内容</param>
		/// <param name="QoSLevel">
		/// 消息等级<br />
		/// QoS 0 是最低级别，基本上等同于 Fire and Forget 模式，发送者发送完数据之后，不关心消息是否已经投递到了接收者那边。<br />
		/// QoS 1 是中间级别，保证消息至少送达一次。MQTT 通过简单的 ACK 机制来保证 QoS1。<br />
		/// QoS 2 是最高级别，保证到且仅到一次。这通过更加复杂的消息流程保证。<br />
		///             <br />
		/// 注意：QoS 级别越高，流程越复杂，系统资源消耗越大
		/// </param>
		/// <param name="Retain">消息是否保留</param>
		/// <returns>统一结果</returns>
		public OperateResult Publish(string Topic, byte[] Content, int QoSLevel = 0, bool Retain = false)
		{
			return PublishAsync(Topic, Content, QoSLevel, Retain).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 添加订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="QoSLevel">
		/// 消息等级<br />
		/// QoS 0 是最低级别，基本上等同于 Fire and Forget 模式，发送者发送完数据之后，不关心消息是否已经投递到了接收者那边。<br />
		/// QoS 1 是中间级别，保证消息至少送达一次。MQTT 通过简单的 ACK 机制来保证 QoS1。<br />
		/// QoS 2 是最高级别，保证到且仅到一次。这通过更加复杂的消息流程保证。<br />
		///             <br />
		/// 注意：QoS 级别越高，流程越复杂，系统资源消耗越大
		/// </param>
		/// <param name="token">传播消息取消</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> AddSubscribeAsync(string Topic, int QoSLevel = 0, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "AddSubscribeAsync");
			try
			{
				if (mqttClient == null || !mqttClient.IsConnected)
				{
					return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, "未连接", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "AddSubscribeAsync", 266);
				}
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync((await MqttClientExtensions.SubscribeAsync((IMqttClient)(object)mqttClient, Topic, (MqttQualityOfServiceLevel)QoSLevel, token)).Items.Count > 0, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "AddSubscribeAsync", 270);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "AddSubscribeAsync", 274);
			}
		}

		/// <summary>
		/// 添加订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="QoSLevel">
		/// 消息等级<br />
		/// QoS 0 是最低级别，基本上等同于 Fire and Forget 模式，发送者发送完数据之后，不关心消息是否已经投递到了接收者那边。<br />
		/// QoS 1 是中间级别，保证消息至少送达一次。MQTT 通过简单的 ACK 机制来保证 QoS1。<br />
		/// QoS 2 是最高级别，保证到且仅到一次。这通过更加复杂的消息流程保证。<br />
		///             <br />
		/// 注意：QoS 级别越高，流程越复杂，系统资源消耗越大
		/// </param>
		/// <returns>统一结果</returns>
		public OperateResult AddSubscribe(string Topic, int QoSLevel = 0)
		{
			return AddSubscribeAsync(Topic, QoSLevel).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 移除订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <param name="token">传播消息取消</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> RemoveSubscribeAsync(string Topic, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "RemoveSubscribeAsync");
			try
			{
				if (mqttClient == null || !mqttClient.IsConnected)
				{
					return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, "未连接", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "RemoveSubscribeAsync", 308);
				}
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync((await MqttClientExtensions.UnsubscribeAsync((IMqttClient)(object)mqttClient, Topic, token)).Items.Count > 0, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "RemoveSubscribeAsync", 312);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "RemoveSubscribeAsync", 316);
			}
		}

		/// <summary>
		/// 移除订阅
		/// </summary>
		/// <param name="Topic">主题</param>
		/// <returns>统一结果</returns>
		public OperateResult RemoveSubscribe(string Topic)
		{
			return RemoveSubscribeAsync(Topic).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
			try
			{
				string text = default(string);
				if (((ResultModel)(await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "OnAsync", 336);
				}
				MqttClientOperate mqttClientOperate = this;
				IMqttClient obj = new MqttClientFactory().CreateMqttClient();
				mqttClientOperate.mqttClient = (MqttClient?)(object)((obj is MqttClient) ? obj : null);
				mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
				mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
				MqttClientConnectResult mqtt = await mqttClient.ConnectAsync(BuildClientOptions(), token);
				if ((int)mqtt.ResultCode != 0)
				{
					await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).OffAsync(true, token);
					return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, ((object)mqtt.ResultCode/*cast due to constrained. prefix*/).ToString(), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "OnAsync", 357);
				}
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "OnAsync", 359);
			}
			catch (Exception ex)
			{
				await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).OffAsync(true, token);
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "OnAsync", 364);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
			try
			{
				string text = default(string);
				if (!hardClose && !((ResultModel)(await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "OffAsync", 378);
				}
				if (mqttClient != null)
				{
					try
					{
						await MqttClientExtensions.DisconnectAsync((IMqttClient)(object)mqttClient, (MqttClientDisconnectOptionsReason)0, (string)null, 0u, (List<MqttUserProperty>)null, default(CancellationToken));
					}
					catch (Exception ex)
					{
						await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "断开连接异常:" + ex.Message));
					}
					((Disposable)mqttClient).Dispose();
					mqttClient = null;
				}
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "OffAsync", 389);
			}
			catch (Exception ex2)
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "OffAsync", 393);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "ProduceAsync");
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "ProduceAsync", 402);
			}
			return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(await PublishAsync(topic, content, 0, Retain: true, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "ProduceAsync", 404);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "ConsumeAsync");
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "ConsumeAsync", 412);
			}
			return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(await AddSubscribeAsync(topic), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "ConsumeAsync", 414);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "UnConsumeAsync");
			string text = default(string);
			if (!((ResultModel)(await ((MqAbstract<MqttClientOperate, MqttClientData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "UnConsumeAsync", 422);
			}
			return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(await RemoveSubscribeAsync(topic, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "UnConsumeAsync", 424);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
			if (mqttClient == null || !mqttClient.IsConnected)
			{
				return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(false, "未连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "GetStatusAsync", 432);
			}
			return await ((CoreUnify<MqttClientOperate, MqttClientData.Basics>)(object)this).EndOperateAsync(true, "已连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mqtt\\client\\MqttClientOperate.cs", "GetStatusAsync", 434);
		}
	}
}
