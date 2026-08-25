using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Snet.Core.@abstract;
using Snet.Core.communication.net.core;
using Snet.Core.communication.net.@enum;
using Snet.Core.communication.net.tcp.client;
using Snet.Core.communication.net.tcp.service;
using Snet.Core.extend;
using Snet.Core.handler;
using Snet.Core.subscription;
using Snet.Core.virtualAddress;
using Snet.Log;
using Snet.Model.attribute;
using Snet.Model.data;
using Snet.Model.@enum;
using Snet.Model.@event;
using Snet.Model.@interface;
using Snet.TEP.core;
using Snet.TEP.master.core;
using Snet.TEP.slave.core;
using Snet.Utility;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("采集协议：TEP（TCP扩展插件，用于非标数据采集使用）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.TEP")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.TEP.slave
{
	/// <summary>
	/// tep 从端数据
	/// </summary>
	public class TepSlaveData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics : Basics
		{
			/// <summary>
			/// 设备名称
			/// </summary>
			[Description("设备名称")]
			public string DevName { get; set; } = "snet";

			/// <summary>
			/// 设备ID
			/// </summary>
			[Description("设备ID")]
			public string DevID { get; set; } = "10001";

			/// <summary>
			/// 用户名
			/// </summary>
			[Description("用户名")]
			public string UserName { get; set; } = "shunnet";

			/// <summary>
			/// 密码
			/// </summary>
			[Description("密码")]
			public string Password { get; set; } = "shunnet";
		}
	}
	/// <summary>
	/// tep 从端操作
	/// </summary>
	public class TepSlaveOperate : CoreUnify<TepSlaveOperate, TepSlaveData.Basics>, IOn, IOff, IStatus, IDisposable, IAsyncDisposable
	{
		/// <summary>
		///             SOCKET TCP Client
		/// </summary>
		private TcpClientOperate? clientOperate;

		/// <summary>
		/// 包处理类
		/// </summary>
		private CorePackHandle corePackHandle = new CorePackHandle();

		/// <summary>
		/// 获取底层写入状态
		/// </summary>
		private Func<List<CoreBasicsData.KeyValue>, OperateResult>? GetBaseWriteState;

		/// <summary>
		/// 获取底层状态
		/// </summary>
		private Func<bool>? GetBaseState;

		/// <summary>
		/// 字节缓存
		/// </summary>
		private byte[] byteBuffer;

		/// <summary>
		/// 此类的生命周期
		/// </summary>
		private CancellationTokenSource tokenSource;

		/// <summary>
		/// 字节队列
		/// </summary>
		private Channel<byte[]> byteQueue = Channel.CreateBounded<byte[]>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 会话心跳包任务
		/// </summary>
		private Task SessionTask;

		/// <summary>
		/// 身份认证JSON
		/// </summary>
		private Channel<string> authenticationJson = Channel.CreateBounded<string>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 心跳包JSON
		/// </summary>
		private Channel<string> heartbeatJson = Channel.CreateBounded<string>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 数据上传JSON
		/// </summary>
		private Channel<string> dataUploadJson = Channel.CreateBounded<string>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public TepSlaveOperate(TepSlaveData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 无惨构造函数
		/// </summary>
		public TepSlaveOperate()
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

		/// <summary>
		/// 设置状态函数；
		/// 服务器获取底层通信运行状态至此；
		/// 服务器下发了命令，并且执行传入的对应的方法
		/// </summary>
		/// <param name="func">函数</param>
		public void SetBaseStateFunc(Func<bool> func)
		{
			GetBaseState = func;
		}

		/// <summary>
		/// 设置底层写入函数；
		/// 服务器写入数据至此；
		/// 服务器下发了命令，并且执行传入的对应的方法
		/// </summary>
		/// <param name="func">函数</param>
		public void SetBaseWriteFunc(Func<List<CoreBasicsData.KeyValue>, OperateResult> func)
		{
			GetBaseWriteState = func;
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
				string text = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
				{
					return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OffAsync", 136);
				}
				SessionTask = null;
				if (tokenSource != null)
				{
					await tokenSource.CancelAsync();
					tokenSource.Dispose();
					tokenSource = null;
				}
				if (byteBuffer != null)
				{
					byteBuffer = null;
				}
				if (byteQueue != null)
				{
					byte[] item;
					while (byteQueue.Reader.TryRead(out item))
					{
					}
				}
				string item2;
				if (authenticationJson != null)
				{
					while (authenticationJson.Reader.TryRead(out item2))
					{
					}
				}
				if (heartbeatJson != null)
				{
					while (heartbeatJson.Reader.TryRead(out item2))
					{
					}
				}
				if (dataUploadJson != null)
				{
					while (dataUploadJson.Reader.TryRead(out item2))
					{
					}
				}
				((CoreUnify<TcpClientOperate, Basics>)(object)clientOperate)?.DisposeAsync();
				clientOperate = null;
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OffAsync", 186);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OffAsync", 190);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "OnAsync");
			try
			{
				string text = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
				{
					return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OnAsync", 201);
				}
				bool flag = clientOperate != null;
				if (flag)
				{
					flag = !((BaseModel)(await ((CommunicationAbstract<TcpClientOperate, Basics>)(object)clientOperate).GetStatusAsync(token))).Status;
				}
				if (flag)
				{
					await ((CoreUnify<TcpClientOperate, Basics>)(object)clientOperate).DisposeAsync();
				}
				if (GetBaseState == null)
				{
					return await base.EndOperateAsync(false, "请通过 [ SetBaseStateFunc ] 传入对应的 Func", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OnAsync", 209);
				}
				if (GetBaseWriteState == null)
				{
					return await base.EndOperateAsync(false, "请通过 [ SetBaseWriteFunc ] 传入对应的 Func", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OnAsync", 213);
				}
				if (tokenSource == null)
				{
					tokenSource = new CancellationTokenSource();
				}
				clientOperate = await CoreUnify<TcpClientOperate, Basics>.InstanceAsync(new Basics
				{
					InterruptReconnection = ((Basics)base.basics).InterruptReconnection,
					IpAddress = ((Basics)base.basics).IpAddress,
					Port = ((Basics)base.basics).Port,
					ReconnectionInterval = ((Basics)base.basics).ReconnectionInterval,
					SN = ((Basics)base.basics).SN,
					Timeout = ((Basics)base.basics).Timeout,
					SendWaitInterval = ((Basics)base.basics).Timeout
				}, default(CancellationToken));
				((CoreUnify<TcpClientOperate, Basics>)(object)clientOperate).OnDataEventAsync -= ClientOperate_OnDataEventAsync;
				((CoreUnify<TcpClientOperate, Basics>)(object)clientOperate).OnDataEventAsync += ClientOperate_OnDataEventAsync;
				((CoreUnify<TcpClientOperate, Basics>)(object)clientOperate).OnInfoEventAsync -= base.OnInfoEventHandlerAsync;
				((CoreUnify<TcpClientOperate, Basics>)(object)clientOperate).OnInfoEventAsync += base.OnInfoEventHandlerAsync;
				HandleQueueAsync(tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
				OperateResult operateResult = await ((CommunicationAbstract<TcpClientOperate, Basics>)(object)clientOperate).OnAsync(token);
				if (((BaseModel)operateResult).Status)
				{
					return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OnAsync", 246);
				}
				await OffAsync(hardClose: true, token);
				return await base.EndOperateAsync(false, ((BaseModel)operateResult).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OnAsync", 249);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true, token);
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "OnAsync", 254);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "GetStatusAsync");
			bool flag = clientOperate != null;
			if (flag)
			{
				flag = ((BaseModel)(await ((CommunicationAbstract<TcpClientOperate, Basics>)(object)clientOperate).GetStatusAsync(token))).Status;
			}
			if (flag)
			{
				return await base.EndOperateAsync(true, "已连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "GetStatusAsync", 264);
			}
			return await base.EndOperateAsync(false, "未连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "GetStatusAsync", 266);
		}

		/// <summary>
		/// 数据事件
		/// </summary>
		/// <param name="sender">源</param>
		/// <param name="e">结果</param>
		private async Task ClientOperate_OnDataEventAsync(object? sender, EventDataResult e)
		{
			if (((ResultModel)e).ResultData != null && ((ResultModel)e).ResultData is byte[])
			{
				_ = $"{((Basics)base.basics).IpAddress}:{((Basics)base.basics).Port}";
				byte[] source = ((ResultModel)e).GetSource<byte[]>();
				if (source != null && source.Length > CoreBasicsData.minLength)
				{
					await byteQueue.Writer.WriteAsync(source, tokenSource.Token);
				}
			}
		}

		/// <summary>
		/// 处理队列数据
		/// </summary>
		/// <param name="token">生命周期</param>
		/// <returns></returns>
		private async Task HandleQueueAsync(CancellationToken token)
		{
			try
			{
				while (await byteQueue.Reader.WaitToReadAsync(token))
				{
					byte[] item;
					while (byteQueue.Reader.TryRead(out item))
					{
						if (byteBuffer != null)
						{
							byte[] dst = new byte[byteBuffer.Length + item.Length];
							Buffer.BlockCopy(byteBuffer, 0, dst, 0, byteBuffer.Length);
							Buffer.BlockCopy(item, 0, dst, byteBuffer.Length, item.Length);
							byteBuffer = dst;
						}
						else
						{
							byteBuffer = item;
						}
						List<byte[]> fullPack = corePackHandle.GetFullPack(byteBuffer, CoreBasicsData.head, CoreBasicsData.trail, out byte[] remainingData);
						byteBuffer = remainingData;
						foreach (byte[] item2 in fullPack)
						{
							CoreBasicsData.PackDetails packDetails = corePackHandle.ParsePackDetails(item2);
							if (packDetails != null)
							{
								HandleAsync(packDetails, token).ConfigureAwait(continueOnCapturedContext: false);
							}
						}
					}
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex3)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult("[ HandleQueue ] 异常：" + ex3.Message));
			}
		}

		/// <summary>
		/// 业务处理 
		/// </summary>
		/// <param name="packDetails">数据包详情</param>
		/// <param name="token">生命周期</param>
		private async Task HandleAsync(CoreBasicsData.PackDetails packDetails, CancellationToken token)
		{
			try
			{
				string ipAddress = ((Basics)base.basics).IpAddress;
				string text = Encoding.UTF8.GetString(packDetails.DataPack);
				if (!ExtendMethod.IsJson(text) && text != CoreBasicsData.Command.握手.ToString())
				{
					await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult($"[ {packDetails.Direction[0]} ( {ipAddress} ) ] 数据包JSON异常，数据包不是有效的JSON\r\n{text}"));
					return;
				}
				if (!packDetails.IsCompleteData)
				{
					await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult($"[ {packDetails.Direction[0]} ( {ipAddress} ) ] 数据异常，不是有效的完整数据，1.请检查CRC效验；2.检查数据包长度是否一致"));
					return;
				}
				if (packDetails.Direction[0] != CoreBasicsData.Terminal.服务端 || packDetails.Direction[1] != CoreBasicsData.Terminal.客户端)
				{
					await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult($"[ {packDetails.Direction[0]} ( {ipAddress} ) ] 请求方向异常，不是有效的方向"));
					return;
				}
				switch (packDetails.Command)
				{
				case CoreBasicsData.Command.握手:
				{
					OperateResult val = await AuthenticationAsync(token);
					await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(((BaseModel)val).Status, ((BaseModel)val).Message));
					return;
				}
				case CoreBasicsData.Command.身份验证:
					await authenticationJson.Writer.WriteAsync(text, token);
					return;
				case CoreBasicsData.Command.会话心跳包:
					await heartbeatJson.Writer.WriteAsync(text, token);
					return;
				case CoreBasicsData.Command.数据上传:
					await dataUploadJson.Writer.WriteAsync(text, token);
					return;
				}
				List<byte> list = new List<byte>();
				list.AddRange(CoreBasicsData.head);
				list.Add((byte)packDetails.Command);
				list.AddRange(CoreBasicsData.directionClientToServer);
				_ = string.Empty;
				byte[] array = null;
				switch (packDetails.Command)
				{
				case CoreBasicsData.Command.获取客户端状态:
				{
					Snet.TEP.slave.core.DataCore.GetClientStatus.Receive receive2 = ExtendMethod.ToJsonEntity<Snet.TEP.slave.core.DataCore.GetClientStatus.Receive>(text, (JsonSerializerOptions)null);
					if (receive2 != null)
					{
						if (receive2.DevID == base.basics.DevID && receive2.DevName == base.basics.DevName)
						{
							if (GetBaseState == null)
							{
								string message = "获取状态函数尚未传入，无法获取状态";
								Snet.TEP.slave.core.DataCore.GetClientStatus.Response response5 = new Snet.TEP.slave.core.DataCore.GetClientStatus.Response
								{
									DevID = receive2.DevID,
									DevName = receive2.DevName,
									Message = message,
									State = 2
								};
								array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.GetClientStatus.Response>(response5, false) ?? string.Empty);
							}
							else
							{
								_ = string.Empty;
								bool flag = GetBaseState();
								string message = ((!flag) ? (packDetails.Command.ToString() + "失败，状态异常") : (packDetails.Command.ToString() + "成功，状态正常"));
								Snet.TEP.slave.core.DataCore.GetClientStatus.Response response6 = new Snet.TEP.slave.core.DataCore.GetClientStatus.Response
								{
									DevID = receive2.DevID,
									DevName = receive2.DevName,
									Message = message,
									State = (flag ? 1 : 2)
								};
								array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.GetClientStatus.Response>(response6, false) ?? string.Empty);
							}
						}
						else
						{
							string message = packDetails.Command.ToString() + "失败，设备信息不一致";
							Snet.TEP.slave.core.DataCore.GetClientStatus.Response response7 = new Snet.TEP.slave.core.DataCore.GetClientStatus.Response
							{
								DevID = receive2.DevID,
								DevName = receive2.DevName,
								Message = message,
								State = 2
							};
							array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.GetClientStatus.Response>(response7, false) ?? string.Empty);
						}
					}
					else
					{
						string message = packDetails.Command.ToString() + "失败，结构体数据转换失败";
						Snet.TEP.slave.core.DataCore.GetClientStatus.Response response8 = new Snet.TEP.slave.core.DataCore.GetClientStatus.Response
						{
							DevID = (receive2?.DevID ?? string.Empty),
							DevName = (receive2?.DevName ?? string.Empty),
							Message = message,
							State = 2
						};
						array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.GetClientStatus.Response>(response8, false) ?? string.Empty);
					}
					break;
				}
				case CoreBasicsData.Command.数据写入:
				{
					Snet.TEP.slave.core.DataCore.DataWriting.Receive receive = ExtendMethod.ToJsonEntity<Snet.TEP.slave.core.DataCore.DataWriting.Receive>(text, (JsonSerializerOptions)null);
					if (receive != null)
					{
						if (receive.DevID == base.basics.DevID && receive.DevName == base.basics.DevName)
						{
							if (GetBaseWriteState == null)
							{
								string message = "写入函数尚未传入，无法写入";
								Snet.TEP.slave.core.DataCore.DataWriting.Response response = new Snet.TEP.slave.core.DataCore.DataWriting.Response
								{
									DevID = receive.DevID,
									DevName = receive.DevName,
									Message = message,
									State = 2
								};
								array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.DataWriting.Response>(response, false) ?? string.Empty);
							}
							else
							{
								_ = string.Empty;
								OperateResult val2 = GetBaseWriteState(receive.AddressValue);
								string message = ((!((BaseModel)val2).Status) ? (packDetails.Command.ToString() + "失败，" + ((BaseModel)val2).Message) : (packDetails.Command.ToString() + "成功，" + ((BaseModel)val2).Message));
								Snet.TEP.slave.core.DataCore.DataWriting.Response response2 = new Snet.TEP.slave.core.DataCore.DataWriting.Response
								{
									DevID = receive.DevID,
									DevName = receive.DevName,
									Message = message,
									State = (((BaseModel)val2).Status ? 1 : 2)
								};
								array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.DataWriting.Response>(response2, false) ?? string.Empty);
							}
						}
						else
						{
							string message = packDetails.Command.ToString() + "失败，设备信息不一致";
							Snet.TEP.slave.core.DataCore.DataWriting.Response response3 = new Snet.TEP.slave.core.DataCore.DataWriting.Response
							{
								DevID = receive.DevID,
								DevName = receive.DevName,
								Message = message,
								State = 2
							};
							array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.DataWriting.Response>(response3, false) ?? string.Empty);
						}
					}
					else
					{
						string message = packDetails.Command.ToString() + "失败，结构体数据转换失败";
						Snet.TEP.slave.core.DataCore.DataWriting.Response response4 = new Snet.TEP.slave.core.DataCore.DataWriting.Response
						{
							DevID = (receive?.DevID ?? string.Empty),
							DevName = (receive?.DevName ?? string.Empty),
							Message = message,
							State = 2
						};
						array = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.DataWriting.Response>(response4, false) ?? string.Empty);
					}
					break;
				}
				}
				if (array == null)
				{
					return;
				}
				Burstification(list, array);
				if (clientOperate != null)
				{
					await ((CommunicationAbstract<TcpClientOperate, Basics>)(object)clientOperate).SendAsync(list.ToArray(), default(CancellationToken));
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex3)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult("[ Handle ] 异常：" + ex3.Message));
			}
		}

		/// <summary>
		/// 组包
		/// </summary>
		/// <param name="bytes_Send">发送的数据</param>
		/// <param name="pack">数据包</param>
		private void Burstification(List<byte> bytes_Send, byte[] pack)
		{
			bytes_Send.AddRange(BitConverter.GetBytes(pack.Length));
			bytes_Send.AddRange(pack);
			bytes_Send.AddRange(ByteHandler.GetCRC16(pack, false));
			bytes_Send.AddRange(CoreBasicsData.trail);
		}

		/// <summary>
		/// 会话状态；
		/// 心跳包检测
		/// </summary>
		/// <returns></returns>
		private async Task SessionHandleAsync(CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					List<byte> list = new List<byte>();
					list.AddRange(CoreBasicsData.head);
					list.Add(50);
					list.AddRange(CoreBasicsData.directionClientToServer);
					Snet.TEP.slave.core.DataCore.SessionHeartBeat.Request request = new Snet.TEP.slave.core.DataCore.SessionHeartBeat.Request
					{
						DevID = base.basics.DevID,
						DevName = base.basics.DevName
					};
					byte[] bytes = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.SessionHeartBeat.Request>(request, false) ?? string.Empty);
					Burstification(list, bytes);
					if (clientOperate != null)
					{
						await ((CommunicationAbstract<TcpClientOperate, Basics>)(object)clientOperate).SendAsync(list.ToArray(), default(CancellationToken));
					}
					try
					{
						using (CancellationTokenSource tokens = new CancellationTokenSource(CoreBasicsData.timeOutMS))
						{
							string text = await heartbeatJson.Reader.ReadAsync(tokens.Token);
							if (!ExtendMethod.IsJson(text))
							{
								await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult
								{
									Message = $"[ {CoreBasicsData.Command.会话心跳包} ] 响应失败，不是有效JSON数据\r\n{text}"
								});
								continue;
							}
							Snet.TEP.slave.core.DataCore.SessionHeartBeat.Response response = ExtendMethod.ToJsonEntity<Snet.TEP.slave.core.DataCore.SessionHeartBeat.Response>(text, (JsonSerializerOptions)null);
							if (response != null)
							{
								await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult(response.State == 1, response.Message));
								goto end_IL_0170;
							}
							await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult
							{
								Message = $"[ {CoreBasicsData.Command.会话心跳包} ] 响应失败，数据结构转换失败"
							});
							goto end_IL_0147;
							end_IL_0170:;
						}
						goto IL_0583;
						end_IL_0147:;
					}
					catch (TaskCanceledException)
					{
						goto IL_0583;
					}
					catch (OperationCanceledException)
					{
						await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult
						{
							Message = $"[ {CoreBasicsData.Command.会话心跳包} ] 响应超时"
						});
						goto IL_0583;
					}
					catch (Exception ex3)
					{
						await base.OnInfoEventHandlerAsync((object)this, new EventInfoResult
						{
							Message = $"[ {CoreBasicsData.Command.会话心跳包} ] 异常：{ex3.Message}"
						});
						goto IL_0583;
					}
					continue;
					IL_0583:
					await Task.Delay(CoreBasicsData.timeOutMS, token);
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex6)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult("[ SessionHandle ] 异常：" + ex6.Message));
			}
		}

		/// <summary>
		/// 身份验证
		/// </summary>
		/// <returns>统一操作结构</returns>
		private async Task<OperateResult> AuthenticationAsync(CancellationToken token)
		{
			await base.BegOperateAsync(token, "AuthenticationAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
				{
					return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 727);
				}
				List<byte> list = new List<byte>();
				list.AddRange(CoreBasicsData.head);
				list.Add(49);
				list.AddRange(CoreBasicsData.directionClientToServer);
				Snet.TEP.slave.core.DataCore.Authentication.Request request = new Snet.TEP.slave.core.DataCore.Authentication.Request
				{
					DevID = base.basics.DevID,
					DevName = base.basics.DevName,
					Password = base.basics.Password,
					UserName = base.basics.UserName
				};
				byte[] bytes = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.Authentication.Request>(request, false) ?? string.Empty);
				Burstification(list, bytes);
				if (clientOperate != null)
				{
					await ((CommunicationAbstract<TcpClientOperate, Basics>)(object)clientOperate).SendAsync(list.ToArray(), token);
				}
				try
				{
					using CancellationTokenSource s_token = new CancellationTokenSource(CoreBasicsData.timeOutMS);
					string text2 = await authenticationJson.Reader.ReadAsync(s_token.Token);
					if (!ExtendMethod.IsJson(text2))
					{
						return await base.EndOperateAsync(false, "不是有效JSON数据", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 764);
					}
					Snet.TEP.slave.core.DataCore.Authentication.Response response = ExtendMethod.ToJsonEntity<Snet.TEP.slave.core.DataCore.Authentication.Response>(text2, (JsonSerializerOptions)null);
					if (response == null)
					{
						return await base.EndOperateAsync(false, "数据结构转换失败", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 769);
					}
					if (response.State == 1 && SessionTask == null)
					{
						SessionTask = SessionHandleAsync(tokenSource.Token);
					}
					return await base.EndOperateAsync(response.State == 1, response.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 781);
				}
				catch (TaskCanceledException)
				{
				}
				catch (OperationCanceledException)
				{
					return await base.EndOperateAsync(false, "响应超时", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 788);
				}
				catch (Exception ex3)
				{
					return await base.EndOperateAsync(false, "异常：" + ex3.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 793);
				}
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 795);
			}
			catch (Exception ex4)
			{
				return await base.EndOperateAsync(false, ex4.Message, (object)null, ex4, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "AuthenticationAsync", 799);
			}
		}

		/// <summary>
		/// 数据上传
		/// </summary>
		/// <param name="keyValues">键值集合</param>
		/// <param name="token">消息取消</param>
		/// <returns>统一操作结构</returns>
		public async Task<OperateResult> DataUploadAsync(List<CoreBasicsData.KeyValue> keyValues, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "DataUploadAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
				{
					return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 817);
				}
				List<byte> list = new List<byte>();
				list.AddRange(CoreBasicsData.head);
				list.Add(52);
				list.AddRange(CoreBasicsData.directionClientToServer);
				Snet.TEP.slave.core.DataCore.DataUpload.Request request = new Snet.TEP.slave.core.DataCore.DataUpload.Request
				{
					DevID = base.basics.DevID,
					DevName = base.basics.DevName,
					AddressValue = keyValues
				};
				byte[] bytes = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<Snet.TEP.slave.core.DataCore.DataUpload.Request>(request, false) ?? string.Empty);
				Burstification(list, bytes);
				if (clientOperate != null)
				{
					OperateResult val = await ((CommunicationAbstract<TcpClientOperate, Basics>)(object)clientOperate).SendAsync(list.ToArray(), token);
					if (!((BaseModel)val).Status)
					{
						return await base.EndOperateAsync(false, "数据发送失败：" + ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 847);
					}
				}
				try
				{
					using CancellationTokenSource s_token = new CancellationTokenSource(CoreBasicsData.timeOutMS);
					string text2 = await dataUploadJson.Reader.ReadAsync(s_token.Token);
					if (!ExtendMethod.IsJson(text2))
					{
						return await base.EndOperateAsync(false, "不是有效JSON数据", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 861);
					}
					Snet.TEP.slave.core.DataCore.DataUpload.Response response = ExtendMethod.ToJsonEntity<Snet.TEP.slave.core.DataCore.DataUpload.Response>(text2, (JsonSerializerOptions)null);
					if (response == null)
					{
						return await base.EndOperateAsync(false, "数据结构转换失败", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 866);
					}
					return await base.EndOperateAsync(response.State == 1, response.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 869);
				}
				catch (TaskCanceledException)
				{
				}
				catch (OperationCanceledException)
				{
					return await base.EndOperateAsync(false, "响应超时", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 876);
				}
				catch (Exception ex3)
				{
					return await base.EndOperateAsync(false, "异常：" + ex3.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 881);
				}
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 883);
			}
			catch (Exception ex4)
			{
				return await base.EndOperateAsync(false, ex4.Message, (object)null, ex4, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\slave\\TepSlaveOperate.cs", "DataUploadAsync", 887);
			}
		}

		/// <summary>
		/// 数据上传
		/// </summary>
		/// <param name="keyValues">键值集合</param>
		/// <returns>统一操作结构</returns>
		public OperateResult DataUpload(List<CoreBasicsData.KeyValue> keyValues)
		{
			return DataUploadAsync(keyValues).GetAwaiter().GetResult();
		}
	}
}
namespace Snet.TEP.slave.core
{
	/// <summary>
	/// 数据核心
	/// </summary>
	public class DataCore
	{
		/// <summary>
		/// 身份认证 客户端主动请求
		/// </summary>
		public class Authentication
		{
			/// <summary>
			/// 请求
			/// </summary>
			public class Request : CoreBasicsData.StructuralBodyBasics
			{
				/// <summary>
				/// 用户名
				/// </summary>
				public string UserName { get; set; }

				/// <summary>
				/// 密码
				/// </summary>
				public string Password { get; set; }
			}

			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}
		}

		/// <summary>
		/// 心跳包
		/// </summary>
		public class SessionHeartBeat
		{
			/// <summary>
			/// 请求
			/// </summary>
			public class Request : CoreBasicsData.StructuralBodyBasics
			{
			}

			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}
		}

		/// <summary>
		/// 获取客户端状态
		/// </summary>
		public class GetClientStatus
		{
			/// <summary>
			/// 接收
			/// </summary>
			public class Receive : CoreBasicsData.StructuralBodyBasics
			{
			}

			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}
		}

		/// <summary>
		/// 数据上传
		/// </summary>
		public class DataUpload
		{
			/// <summary>
			/// 请求
			/// </summary>
			public class Request : CoreBasicsData.StructuralBodyBasics
			{
				/// <summary>
				/// 地址值集合
				/// </summary>
				public List<CoreBasicsData.KeyValue> AddressValue { get; set; }
			}

			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}
		}

		/// <summary>
		/// 数据写入 服务端主动
		/// </summary>
		public class DataWriting
		{
			/// <summary>
			/// 接收
			/// </summary>
			public class Receive : DataUpload.Request
			{
			}

			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}
		}
	}
}
namespace Snet.TEP.master
{
	/// <summary>
	/// TEP 主服务数据
	/// </summary>
	public class TepMasterData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics : SCData
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// Ip地址
			/// </summary>
			[Description("Ip地址")]
			[Verify("^((25[0-5]|2[0-4]\\d|[01]?\\d\\d?)\\.){3}(25[0-5]|2[0-4]\\d|[01]?\\d\\d?)$", "输入有误")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? IpAddress { get; set; } = "127.0.0.1";

			/// <summary>
			/// 端口
			/// </summary>
			[Description("端口")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 最大连接数
			/// </summary>
			[Description("最大连接数")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int MaxNumber { get; set; } = 1000;

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
		}
	}
	/// <summary>
	/// TEP(TCP扩展插件) 服务端操作
	/// </summary>
	public class TepMasterOperate : DaqAbstract<TepMasterOperate, TepMasterData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 等待数据发送成功时间
		/// </summary>
		private int WaitDataSendSuccessTime;

		/// <summary>
		/// 服务端操作
		/// </summary>
		private TcpServiceOperate? serviceOperate;

		/// <summary>
		/// 全局生命周期
		/// </summary>
		private CancellationTokenSource? tokenSource;

		/// <summary>
		/// 虚拟地址
		/// </summary>
		private VirtualAddressManage VAM;

		/// <summary>
		/// 实现订阅功能
		/// </summary>
		private SubscribeOperate? subscribeOperate;

		/// <summary>
		/// 客户端管理
		/// </summary>
		private ConcurrentDictionary<string, ManageCore> manageCores;

		/// <summary>
		/// 客户端的事件管理
		/// </summary>
		private ConcurrentDictionary<string, EventHandlerAsync<EventInfoResult>> manageEnevs;

		/// <inheritdoc />
		protected override string CN => "TCP扩展工具服务端";

		/// <inheritdoc />
		protected override string CD => "支持对“内部多个TEP客户端”多种数据类型读取写入，并支持数据订阅模式";

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public TepMasterOperate(TepMasterData.Basics basics)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			WaitDataSendSuccessTime = 50;
			VAM = new VirtualAddressManage();
			manageCores = new ConcurrentDictionary<string, ManageCore>();
			manageEnevs = new ConcurrentDictionary<string, EventHandlerAsync<EventInfoResult>>();
			base..ctor(basics);
		}

		/// <summary>
		/// 无惨构造函数
		/// </summary>
		public TepMasterOperate()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			WaitDataSendSuccessTime = 50;
			VAM = new VirtualAddressManage();
			manageCores = new ConcurrentDictionary<string, ManageCore>();
			manageEnevs = new ConcurrentDictionary<string, EventHandlerAsync<EventInfoResult>>();
			base..ctor();
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
			try
			{
				string text = default(string);
				if (((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OnAsync", 83);
				}
				if (tokenSource == null)
				{
					tokenSource = new CancellationTokenSource();
				}
				serviceOperate = await CoreUnify<TcpServiceOperate, Basics>.InstanceAsync(new Basics
				{
					IpAddress = ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics.IpAddress,
					Port = ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics.Port,
					MaxNumber = ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics.MaxNumber,
					SN = ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics.SN
				}, default(CancellationToken));
				OperateResult operateResult = await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).OnAsync(token);
				if (((BaseModel)operateResult).Status)
				{
					((CoreUnify<TcpServiceOperate, Basics>)(object)serviceOperate).OnDataEventAsync -= ServiceOperate_OnDataEventAsync;
					((CoreUnify<TcpServiceOperate, Basics>)(object)serviceOperate).OnDataEventAsync += ServiceOperate_OnDataEventAsync;
					((CoreUnify<TcpServiceOperate, Basics>)(object)serviceOperate).OnInfoEventAsync -= ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).OnInfoEventHandlerAsync;
					((CoreUnify<TcpServiceOperate, Basics>)(object)serviceOperate).OnInfoEventAsync += ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).OnInfoEventHandlerAsync;
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OnAsync", 110);
				}
				await ((DaqAbstract<TepMasterOperate, TepMasterData.Basics>)(object)this).OffAsync(true, token);
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)operateResult).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OnAsync", 113);
			}
			catch (Exception ex)
			{
				await ((DaqAbstract<TepMasterOperate, TepMasterData.Basics>)(object)this).OffAsync(true, token);
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OnAsync", 118);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
			try
			{
				string text = default(string);
				if (!hardClose && !((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OffAsync", 131);
				}
				if (tokenSource != null)
				{
					await tokenSource.CancelAsync();
					tokenSource.Dispose();
					tokenSource = null;
				}
				if (subscribeOperate != null)
				{
					OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
					if (!((BaseModel)val).Status)
					{
						return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OffAsync", 147);
					}
					subscribeOperate = null;
				}
				if (serviceOperate != null)
				{
					OperateResult val2 = await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).OffAsync(hardClose, token);
					if (!((BaseModel)val2).Status)
					{
						return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val2).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OffAsync", 158);
					}
					((CoreUnify<TcpServiceOperate, Basics>)(object)serviceOperate)?.DisposeAsync();
					serviceOperate = null;
				}
				if (manageCores != null)
				{
					foreach (KeyValuePair<string, ManageCore> manageCore in manageCores)
					{
						if (manageEnevs.TryRemove(manageCore.Key, out EventHandlerAsync<EventInfoResult> value))
						{
							((CoreUnify<ManageCore, string>)(object)manageCore.Value).OnInfoEventAsync -= value;
						}
						await ((CoreUnify<ManageCore, string>)(object)manageCore.Value).DisposeAsync();
					}
					manageCores.Clear();
					if (manageEnevs != null)
					{
						manageEnevs.Clear();
					}
				}
				VirtualAddressManage vAM = VAM;
				if (vAM != null)
				{
					vAM.DisposeAsync();
				}
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OffAsync", 184);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "OffAsync", 188);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
			string text = default(string);
			if (!((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "GetStatusAsync", 197);
			}
			if (manageCores.Count > 0)
			{
				List<string> FailMessage = new List<string>();
				foreach (KeyValuePair<string, ManageCore> manageCore in manageCores)
				{
					if (!((ResultModel)(await manageCore.Value.GetClientBaseStatusAsync(token))).GetDetails(ref text))
					{
						FailMessage.Add(text);
					}
				}
				if (FailMessage.Count > 0)
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "GetStatusAsync", 214);
				}
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("已连接", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "GetStatusAsync", 216);
			}
			return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("客户端尚未连接，无法获取客户端状态", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "GetStatusAsync", 220);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
			string text = default(string);
			if (!((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "GetBaseObjectAsync", 230);
			}
			return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)serviceOperate, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "GetBaseObjectAsync", 232);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "ReadAsync", 243);
				}
				if (!address.CheckAddress())
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "ReadAsync", 248);
				}
				ConcurrentDictionary<string, AddressValue> param = new ConcurrentDictionary<string, AddressValue>();
				foreach (AddressDetails item in address.AddressArray)
				{
					if (!item.IsEnable)
					{
						continue;
					}
					bool flag = false;
					VAM.InitVirtualAddress(item, ref flag);
					if (flag)
					{
						VAM.Read(item);
						continue;
					}
					try
					{
						string[] array = item.AddressName.Split('.');
						string text2 = array[0];
						string text3 = array[1];
						string addressName = array[2];
						object obj = null;
						foreach (KeyValuePair<string, ManageCore> manageCore in manageCores)
						{
							(string, string, string) info = manageCore.Value.GetInfo();
							if (info.Item2 == text2 && info.Item3 == text3)
							{
								obj = manageCore.Value.GetAddressValue(addressName);
								break;
							}
						}
						object obj2 = obj;
						AddressDetails val = item;
						object obj3 = obj2;
						string text4 = ((!string.IsNullOrEmpty(obj2?.ToString())) ? (await LanguageHandler.GetLanguageValueAsync("成功", (LanguageModel)null, token)) : (await LanguageHandler.GetLanguageValueAsync("失败", (LanguageModel)null, token)));
						AddressValue addressValue = AddressHandler.ExecuteDispose(val, obj3, text4);
						param.AddOrUpdate(item.AddressName, addressValue, (string k, AddressValue v) => addressValue);
					}
					catch (Exception ex)
					{
						string addressName2 = item.AddressName;
						string text4 = await LanguageHandler.GetLanguageValueAsync("读取异常", (LanguageModel)null, token);
						LogHelper.Error("[ " + addressName2 + " ]" + text4 + "：" + ex.Message, (string)null, (Exception)null, true);
					}
				}
				if (param.Count > 0)
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "ReadAsync", 311);
				}
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "ReadAsync", 315);
			}
			catch (Exception ex2)
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "ReadAsync", 320);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "WriteAsync", 331);
				}
				List<string> FailMessage = new List<string>();
				ConcurrentDictionary<(string devName, string devID), List<CoreBasicsData.KeyValue>> keyValuePairs = new ConcurrentDictionary<(string, string), List<CoreBasicsData.KeyValue>>();
				foreach (KeyValuePair<string, (object, EncodingType?)> value in values)
				{
					if (VAM.IsVirtualAddress(value.Key))
					{
						if (!VAM.Write(value.Key, value.Value.Item1))
						{
							List<string> list = FailMessage;
							string key = value.Key;
							list.Add(key + "，" + await LanguageHandler.GetLanguageValueAsync("写入失败", (LanguageModel)null, token));
						}
						continue;
					}
					string[] array = value.Key.Split('.');
					string item = array[0];
					string item2 = array[1];
					string key2 = array[2];
					keyValuePairs.GetOrAdd((item, item2), ((string devName, string devID) _) => new List<CoreBasicsData.KeyValue>()).Add(new CoreBasicsData.KeyValue
					{
						Key = key2,
						Value = value.Value.Item1
					});
				}
				foreach (KeyValuePair<(string, string), List<CoreBasicsData.KeyValue>> item5 in keyValuePairs)
				{
					string item3 = item5.Key.Item1;
					string item4 = item5.Key.Item2;
					ManageCore manageCore = null;
					foreach (KeyValuePair<string, ManageCore> manageCore2 in manageCores)
					{
						(string, string, string) info = manageCore2.Value.GetInfo();
						if (info.Item2 == item3 && info.Item3 == item4)
						{
							manageCore = manageCore2.Value;
							break;
						}
					}
					if (manageCore != null)
					{
						if (!((ResultModel)(await manageCore.SetAddressValueAsync(item5.Value))).GetDetails(ref text))
						{
							FailMessage.Add(text);
						}
						continue;
					}
					List<string> list = FailMessage;
					list.Add(await LanguageHandler.GetLanguageValueAsync($"[ {item3} {item4} ] 设备不存在", (LanguageModel)null, token));
				}
				if (FailMessage.Count > 0)
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "WriteAsync", 405);
				}
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "WriteAsync", 407);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "WriteAsync", 412);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "SubscribeAsync", 423);
				}
				if (!address.CheckAddress())
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "SubscribeAsync", 427);
				}
				if (subscribeOperate == null)
				{
					subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
					{
						Address = address,
						ChangeOut = ((SCData)((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics).ChangeOut,
						FunctionAsync = ((DaqAbstract<TepMasterOperate, TepMasterData.Basics>)(object)this).ReadAsync,
						AllOut = ((SCData)((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics).AllOut,
						HandleInterval = ((SCData)((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics).HandleInterval,
						SN = ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics.SN,
						TaskNumber = ((SCData)((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).basics).TaskNumber
					}, default(CancellationToken));
					((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEvent += ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).OnDataEventHandler;
					((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEvent += ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).OnInfoEventHandler;
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "SubscribeAsync", 444);
				}
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "SubscribeAsync", 448);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "SubscribeAsync", 453);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await getStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "UnSubscribeAsync", 464);
				}
				if (subscribeOperate != null)
				{
					OperateResult val = await subscribeOperate.UnSubscribeAsync(address, token);
					return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(((BaseModel)val).Status, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "UnSubscribeAsync", 469);
				}
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "UnSubscribeAsync", 473);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "UnSubscribeAsync", 478);
			}
		}

		/// <summary>
		/// 私有的获取状态；<br />
		/// 由于接口的获取状态函数需要像底层发送数据得知底层的状态才定此私有获取状态
		/// </summary>
		/// <param name="token">消息取消</param>
		/// <returns>统一结果</returns>
		private async Task<OperateResult> getStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).BegOperateAsync(token, "getStatusAsync");
			bool flag = serviceOperate != null;
			if (flag)
			{
				flag = ((BaseModel)(await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).GetStatusAsync(token))).Status;
			}
			if (flag)
			{
				return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("已启动", (LanguageModel)null, token), (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "getStatusAsync", 493);
			}
			return await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("未启动", (LanguageModel)null, token), (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\TepMasterOperate.cs", "getStatusAsync", 495);
		}

		/// <summary>
		/// 事件消息
		/// </summary>
		/// <param name="sender">源</param>
		/// <param name="e">事件</param>
		private async Task ServiceOperate_OnDataEventAsync(object? sender, EventDataResult e)
		{
			if (!(((ResultModel)e).ResultData is ClientMessage))
			{
				return;
			}
			ClientMessage clientMessage = ((ResultModel)e).GetSource<ClientMessage>();
			if (clientMessage == null)
			{
				return;
			}
			Steps step = clientMessage.Step;
			switch ((int)step)
			{
			case 0:
			{
				ManageCore manageCore = CoreUnify<ManageCore, string>.Instance(clientMessage.IpPort);
				manageCores.AddOrUpdate(clientMessage.IpPort, manageCore, (string k, ManageCore v) => manageCore);
				if (!manageEnevs.TryGetValue(clientMessage.IpPort, out EventHandlerAsync<EventInfoResult> value2))
				{
					value2 = async delegate(object? sender2, EventInfoResult e2)
					{
						await ManageCore_OnInfoEventAsync(sender2, e2, clientMessage.IpPort);
					};
					manageEnevs.TryAdd(clientMessage.IpPort, value2);
				}
				((CoreUnify<ManageCore, string>)(object)manageCore).OnInfoEventAsync -= value2;
				((CoreUnify<ManageCore, string>)(object)manageCore).OnInfoEventAsync += value2;
				string text = default(string);
				bool flag = !((ResultModel)(await manageCore.StartAsync(this, serviceOperate))).GetDetails(ref text);
				if (!flag)
				{
					flag = !((ResultModel)(await manageCore.HandshakeAsync())).GetDetails(ref text);
				}
				if (flag)
				{
					await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(text));
					RemoveClient(clientMessage.IpPort);
				}
				break;
			}
			case 1:
			{
				if (manageCores.TryGetValue(clientMessage.IpPort, out ManageCore value3))
				{
					if (value3.GetAuthenticationStatus())
					{
						(string, string, string) info = value3.GetInfo();
						await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult($"[ {LanguageHandler.GetLanguageValue("客户端", (LanguageModel)null)} ( {info.Item1} -> {info.Item2} {info.Item3} ) ] {LanguageHandler.GetLanguageValue("已断开连接", (LanguageModel)null)}"));
					}
					RemoveClient(clientMessage.IpPort);
				}
				break;
			}
			case 2:
			{
				if (manageCores.TryGetValue(clientMessage.IpPort, out ManageCore value))
				{
					await value.EnqueueAsync(clientMessage.Bytes, tokenSource.Token);
				}
				break;
			}
			}
		}

		/// <summary>
		/// 客户端管理信息事件
		/// </summary>
		/// <param name="sender">源</param>
		/// <param name="e">结果</param>
		/// <param name="ipPort">IP端口</param>
		private async Task ManageCore_OnInfoEventAsync(object? sender, EventInfoResult e, string ipPort)
		{
			await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, e);
			if (!((BaseModel)e).Status)
			{
				RemoveClientWaitAsync(ipPort, tokenSource?.Token ?? CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 移除客户端等待，得让数据先发送出去
		/// </summary>
		/// <param name="ipPort">IP端口</param>
		/// <param name="token">取消令牌</param>
		/// <returns></returns>
		private async Task RemoveClientWaitAsync(string ipPort, CancellationToken token)
		{
			await Task.Run(async delegate
			{
				await Task.Delay(WaitDataSendSuccessTime, token);
				RemoveClient(ipPort);
			}, token);
		}

		/// <summary>
		/// 移除客户端所有信息,必须同步执行
		/// </summary>
		/// <param name="ipPort">IP地址</param>
		private void RemoveClient(string ipPort)
		{
			if (string.IsNullOrWhiteSpace(ipPort))
			{
				return;
			}
			if (manageCores.TryRemove(ipPort, out ManageCore value))
			{
				if (manageEnevs.TryRemove(ipPort, out EventHandlerAsync<EventInfoResult> value2))
				{
					((CoreUnify<ManageCore, string>)(object)value).OnInfoEventAsync -= value2;
				}
				((CoreUnify<ManageCore, string>)(object)value)?.Dispose();
			}
			((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate)?.Remove(new string[1] { ipPort });
		}
	}
}
namespace Snet.TEP.master.core
{
	/// <summary>
	/// 客户端数据
	/// </summary>
	public class DataCore
	{
		/// <summary>
		/// 身份认证 客户端主动请求
		/// </summary>
		public class Authentication
		{
			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}

			/// <summary>
			/// 接收
			/// </summary>
			public class Receive : CoreBasicsData.StructuralBodyBasics
			{
				/// <summary>
				/// 用户名
				/// </summary>
				public string UserName { get; set; }

				/// <summary>
				/// 密码
				/// </summary>
				public string Password { get; set; }
			}
		}

		/// <summary>
		/// 心跳包 客户端主动
		/// </summary>
		public class SessionHeartBeat
		{
			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}

			/// <summary>
			/// 接收
			/// </summary>
			public class Receive : CoreBasicsData.StructuralBodyBasics
			{
			}
		}

		/// <summary>
		/// 获取客户端状态 服务端主动
		/// </summary>
		public class GetClientStatus
		{
			/// <summary>
			/// 请求
			/// </summary>
			public class Request : CoreBasicsData.StructuralBodyBasics
			{
			}

			/// <summary>
			/// 接收
			/// </summary>
			public class Receive : CoreBasicsData.BasicsResponse
			{
			}
		}

		/// <summary>
		/// 数据上传 客户端主动
		/// </summary>
		public class DataUpload
		{
			/// <summary>
			/// 响应
			/// </summary>
			public class Response : CoreBasicsData.BasicsResponse
			{
			}

			/// <summary>
			/// 接收
			/// </summary>
			public class Receive : CoreBasicsData.StructuralBodyBasics
			{
				/// <summary>
				/// 地址值集合
				/// </summary>
				public List<CoreBasicsData.KeyValue> AddressValue { get; set; }
			}
		}

		/// <summary>
		/// 数据写入 服务端主动
		/// </summary>
		public class DataWriting
		{
			/// <summary>
			/// 请求
			/// </summary>
			public class Request : DataUpload.Receive
			{
			}

			/// <summary>
			/// 接收
			/// </summary>
			public class Receive : CoreBasicsData.BasicsResponse
			{
			}
		}
	}
	/// <summary>
	/// 客户端管理<br />
	/// 以IP为介质来进行单例管理
	/// </summary>
	public class ManageCore : CoreUnify<ManageCore, string>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 主端对象
		/// </summary>
		private TepMasterOperate masterOperate;

		/// <summary>
		/// 主端基础数据
		/// </summary>
		private TepMasterData.Basics masterOperateBasics;

		/// <summary>
		/// 服务端操作
		/// </summary>
		private TcpServiceOperate serviceOperate;

		/// <summary>
		/// 字节缓存
		/// </summary>
		private byte[] byteBuffer;

		/// <summary>
		/// 此类的生命周期
		/// </summary>
		private CancellationTokenSource tokenSource;

		/// <summary>
		/// 字节队列
		/// </summary>
		private Channel<byte[]> byteQueue = Channel.CreateBounded<byte[]>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 包处理类
		/// </summary>
		private CorePackHandle corePackHandle = new CorePackHandle();

		/// <summary>
		/// 地址键值集合
		/// </summary>
		private ConcurrentDictionary<string, object> AValue = new ConcurrentDictionary<string, object>();

		/// <summary>
		/// 写入响应json
		/// </summary>
		private Channel<string> writeResponseJson = Channel.CreateBounded<string>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 状态响应json
		/// </summary>
		private Channel<string> stateResponseJson = Channel.CreateBounded<string>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 身份认证JSON
		/// </summary>
		private Channel<string> authenticationJson = Channel.CreateBounded<string>(CoreBasicsData.channelOptions);

		/// <summary>
		/// 设备名称
		/// </summary>
		private string devName = string.Empty;

		/// <summary>
		/// 设备ID
		/// </summary>
		private string devID = string.Empty;

		/// <summary>
		/// 心跳包
		/// </summary>
		private DateTime heartbeatTime = DateTime.Now;

		/// <summary>
		/// 认证状态
		/// </summary>
		private bool authenticationStatus
		{
			get
			{
				if (string.IsNullOrWhiteSpace(devName) || string.IsNullOrWhiteSpace(devID))
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public ManageCore(string basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 入队；
		/// 将接收到的字节数据写入队列，供处理线程消费
		/// </summary>
		/// <param name="byteData">收到的字节数据</param>
		/// <param name="token">取消通知</param>
		public async Task EnqueueAsync(byte[] byteData, CancellationToken token)
		{
			await byteQueue.Writer.WriteAsync(byteData, token);
		}

		/// <summary>
		/// 获取当前类的设备信息
		/// </summary>
		/// <returns></returns>
		public (string ipPort, string devName, string devID) GetInfo()
		{
			return (ipPort: base.basics, devName: devName, devID: devID);
		}

		/// <summary>
		/// 获取认证状态
		/// </summary>
		/// <returns></returns>
		public bool GetAuthenticationStatus()
		{
			return authenticationStatus;
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			StopAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await StopAsync();
			await base.DisposeAsync();
		}

		/// <summary>
		/// 获取信息头
		/// </summary>
		/// <returns></returns>
		public string GetMsgHead()
		{
			return $"[ 客户端 ( {base.basics} -> {devName} {devID} ) ]";
		}

		/// <summary>
		/// 开始
		/// </summary>
		/// <param name="masterOperate">主端对象</param>
		/// <param name="serviceOperate">tcp对象</param>
		/// <param name="token">消息取消通知</param>
		public async Task<OperateResult> StartAsync(TepMasterOperate masterOperate, TcpServiceOperate serviceOperate, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "StartAsync");
			try
			{
				string text = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
				{
					return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "StartAsync", 168);
				}
				tokenSource = new CancellationTokenSource();
				this.masterOperate = masterOperate;
				this.serviceOperate = serviceOperate;
				masterOperateBasics = ((ResultModel)(await ((CoreUnify<TepMasterOperate, TepMasterData.Basics>)(object)this.masterOperate).GetBasicsArgsAsync(token))).GetSource<TepMasterData.Basics>();
				HandleConnectAsync().ConfigureAwait(continueOnCapturedContext: false);
				HandleQueueAsync(tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "StartAsync", 185);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "StartAsync", 189);
			}
		}

		/// <summary>
		/// 结束
		/// </summary>
		/// <param name="token">取消通知</param>
		public async Task<OperateResult> StopAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "StopAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
				{
					return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "StopAsync", 204);
				}
				if (tokenSource != null)
				{
					await tokenSource.CancelAsync();
					tokenSource.Dispose();
					tokenSource = null;
				}
				byte[] item;
				while (byteQueue.Reader.TryRead(out item))
				{
				}
				byteBuffer = null;
				AValue.Clear();
				string item2;
				while (writeResponseJson.Reader.TryRead(out item2))
				{
				}
				while (stateResponseJson.Reader.TryRead(out item2))
				{
				}
				while (authenticationJson.Reader.TryRead(out item2))
				{
				}
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "StopAsync", 233);
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "StopAsync", 237);
			}
		}

		/// <summary>
		/// 与客户端握手
		/// </summary>
		/// <param name="token">取消通知</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> HandshakeAsync(CancellationToken token = default(CancellationToken))
		{
			List<byte> list = new List<byte>();
			list.AddRange(CoreBasicsData.head);
			list.Add(48);
			list.AddRange(CoreBasicsData.directionServerToClient);
			Burstification(list, Encoding.UTF8.GetBytes(CoreBasicsData.Command.握手.ToString()));
			if (serviceOperate != null)
			{
				return await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).SendAsync(list.ToArray(), new string[1] { base.basics }, token);
			}
			return OperateResult.CreateFailureResult("服务端未启动", (int?)null);
		}

		/// <summary>
		/// 获取客户端底层状态
		/// </summary>
		/// <param name="token">取消通知</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> GetClientBaseStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "GetClientBaseStatusAsync");
			try
			{
				List<byte> list = new List<byte>();
				list.AddRange(CoreBasicsData.head);
				list.Add(51);
				list.AddRange(CoreBasicsData.directionServerToClient);
				DataCore.GetClientStatus.Request request = new DataCore.GetClientStatus.Request
				{
					DevID = devID,
					DevName = devName
				};
				byte[] bytes = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.GetClientStatus.Request>(request, false) ?? string.Empty);
				Burstification(list, bytes);
				if (serviceOperate != null)
				{
					await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).SendAsync(list.ToArray(), new string[1] { base.basics }, token);
				}
				try
				{
					using CancellationTokenSource s_token = new CancellationTokenSource(CoreBasicsData.timeOutMS);
					string text = await stateResponseJson.Reader.ReadAsync(s_token.Token);
					if (!ExtendMethod.IsJson(text))
					{
						return await base.EndOperateAsync(false, GetMsgHead() + " 不是有效JSON数据", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 307);
					}
					DataCore.GetClientStatus.Receive receive = ExtendMethod.ToJsonEntity<DataCore.GetClientStatus.Receive>(text, (JsonSerializerOptions)null);
					if (receive == null)
					{
						return await base.EndOperateAsync(false, GetMsgHead() + " 请求JSON数据异常", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 312);
					}
					if (receive.State != 1)
					{
						return await base.EndOperateAsync(false, GetMsgHead() + " " + receive.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 316);
					}
				}
				catch (TaskCanceledException)
				{
					return await base.EndOperateAsync(false, GetMsgHead() + " 超时，未在规定的时间内收到响应", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 321);
				}
				catch (OperationCanceledException)
				{
					return await base.EndOperateAsync(false, GetMsgHead() + " 超时，未在规定的时间内收到响应", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 325);
				}
				catch (Exception ex3)
				{
					return await base.EndOperateAsync(false, GetMsgHead() + " 异常：" + ex3.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 329);
				}
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 331);
			}
			catch (Exception ex4)
			{
				return await base.EndOperateAsync(false, ex4.Message, (object)null, ex4, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetClientBaseStatusAsync", 335);
			}
		}

		/// <summary>
		/// 获取地址值
		/// </summary>
		/// <param name="addressName">地址名称</param>
		/// <returns>返回值</returns>
		public object? GetAddressValue(string addressName)
		{
			if (!AValue.TryGetValue(addressName, out object value))
			{
				return null;
			}
			return value;
		}

		/// <summary>
		/// 写入地址值
		/// </summary>
		/// <param name="keyValue">键值</param>
		/// <param name="token">取消通知</param>
		/// <returns></returns>
		public async Task<OperateResult> SetAddressValueAsync(List<CoreBasicsData.KeyValue> keyValue, CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "SetAddressValueAsync");
			try
			{
				List<byte> list = new List<byte>();
				list.AddRange(CoreBasicsData.head);
				list.Add(53);
				list.AddRange(CoreBasicsData.directionServerToClient);
				byte[] bytes = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.DataWriting.Request>(new DataCore.DataWriting.Request
				{
					DevID = devID,
					DevName = devName,
					AddressValue = keyValue
				}, false) ?? string.Empty);
				Burstification(list, bytes);
				if (serviceOperate != null)
				{
					await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).SendAsync(list.ToArray(), new string[1] { base.basics }, token);
				}
				try
				{
					using CancellationTokenSource s_token = new CancellationTokenSource(CoreBasicsData.timeOutMS);
					string text = await writeResponseJson.Reader.ReadAsync(s_token.Token);
					if (!ExtendMethod.IsJson(text))
					{
						return await base.EndOperateAsync(false, GetMsgHead() + " 不是有效JSON数据", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 391);
					}
					DataCore.DataWriting.Receive receive = ExtendMethod.ToJsonEntity<DataCore.DataWriting.Receive>(text, (JsonSerializerOptions)null);
					if (receive == null)
					{
						return await base.EndOperateAsync(false, GetMsgHead() + " 请求JSON数据异常", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 396);
					}
					if (receive.State != 1)
					{
						return await base.EndOperateAsync(false, GetMsgHead() + " " + receive.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 400);
					}
				}
				catch (TaskCanceledException)
				{
					return await base.EndOperateAsync(false, GetMsgHead() + " 超时，未在规定的时间内收到响应", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 405);
				}
				catch (OperationCanceledException)
				{
					return await base.EndOperateAsync(false, GetMsgHead() + " 超时，未在规定的时间内收到响应", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 409);
				}
				catch (Exception ex3)
				{
					return await base.EndOperateAsync(false, GetMsgHead() + " 异常：" + ex3.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 413);
				}
				return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 415);
			}
			catch (Exception ex4)
			{
				return await base.EndOperateAsync(false, ex4.Message, (object)null, ex4, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "SetAddressValueAsync", 419);
			}
		}

		/// <summary>
		/// 处理连接
		/// </summary>
		/// <returns></returns>
		private async Task HandleConnectAsync()
		{
			try
			{
				using CancellationTokenSource token = new CancellationTokenSource(CoreBasicsData.timeOutMS);
				await authenticationJson.Reader.ReadAsync(token.Token);
				if (GetAuthenticationStatus())
				{
					await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateSuccessResult(GetMsgHead() + " 身份认证成功"));
					HandleSessionAsync(tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
					return;
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " 身份未知，强制关闭"));
			}
			catch (Exception ex3)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " 异常：" + ex3.Message));
			}
		}

		/// <summary>
		/// 会话心跳包处理；
		/// 定时检查客户端心跳包更新时间，超时则强制关闭连接
		/// </summary>
		/// <param name="token">取消令牌</param>
		private async Task HandleSessionAsync(CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					if ((DateTime.Now - heartbeatTime).TotalMilliseconds > (double)(CoreBasicsData.timeOutMS * 2))
					{
						await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " 会话心跳包超时，强制关闭"));
						return;
					}
					await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateSuccessResult(GetMsgHead() + " 会话心跳包正常"));
					await Task.Delay(CoreBasicsData.timeOutMS, token);
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex3)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " [ HandleSession ] 异常：" + ex3.Message));
			}
		}

		/// <summary>
		/// 处理队列数据<br />
		/// 这里使用一个线程去处理,多线程数据会混乱
		/// </summary>
		/// <param name="token">生命周期</param>
		/// <returns></returns>
		private async Task HandleQueueAsync(CancellationToken token)
		{
			try
			{
				while (await byteQueue.Reader.WaitToReadAsync(token))
				{
					byte[] item;
					while (byteQueue.Reader.TryRead(out item))
					{
						try
						{
							if (byteBuffer != null)
							{
								byte[] dst = new byte[byteBuffer.Length + item.Length];
								Buffer.BlockCopy(byteBuffer, 0, dst, 0, byteBuffer.Length);
								Buffer.BlockCopy(item, 0, dst, byteBuffer.Length, item.Length);
								byteBuffer = dst;
							}
							else
							{
								byteBuffer = item;
							}
							List<byte[]> fullPack = corePackHandle.GetFullPack(byteBuffer, CoreBasicsData.head, CoreBasicsData.trail, out byte[] remainingData);
							byteBuffer = remainingData;
							foreach (byte[] item2 in fullPack)
							{
								CoreBasicsData.PackDetails packDetails = corePackHandle.ParsePackDetails(item2);
								if (packDetails != null)
								{
									HandleAsync(packDetails, token).ConfigureAwait(continueOnCapturedContext: false);
								}
							}
						}
						catch (TaskCanceledException)
						{
						}
						catch (OperationCanceledException)
						{
						}
						catch (Exception ex3)
						{
							await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " [ HandleQueue ] 异常：" + ex3.Message));
						}
					}
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex6)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " [ HandleQueue ] 异常：" + ex6.Message));
			}
		}

		/// <summary>
		/// 业务处理 
		/// </summary>
		/// <param name="packDetails">数据包详情</param>
		/// <param name="token">生命周期</param>
		private async Task HandleAsync(CoreBasicsData.PackDetails packDetails, CancellationToken token)
		{
			try
			{
				try
				{
					string ip = base.basics;
					string json = Encoding.UTF8.GetString(packDetails.DataPack);
					_ = string.Empty;
					byte[] pack = null;
					List<byte> bytes_Send = new List<byte>();
					bytes_Send.AddRange(CoreBasicsData.head);
					bytes_Send.Add((byte)packDetails.Command);
					bytes_Send.AddRange(CoreBasicsData.directionServerToClient);
					if (!ExtendMethod.IsJson(json))
					{
						string text = $"[ {packDetails.Direction[0].ToString()} ( {base.basics} -> {this.devName} {this.devID} ) ] 数据包JSON异常，数据包不是有效的JSON\r\n{json}";
						CoreBasicsData.BasicsResponse basicsResponse = new CoreBasicsData.BasicsResponse
						{
							State = 2,
							DevID = this.devID,
							DevName = this.devName,
							Message = text
						};
						pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<CoreBasicsData.BasicsResponse>(basicsResponse, false) ?? string.Empty);
						await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(text));
					}
					else if (!packDetails.IsCompleteData)
					{
						string text = $"[ {packDetails.Direction[0].ToString()} ( {base.basics} -> {this.devName} {this.devID} ) ] 数据异常，不是有效的完整数据，1.请检查CRC效验；2.检查数据包长度是否一致";
						CoreBasicsData.BasicsResponse basicsResponse2 = new CoreBasicsData.BasicsResponse
						{
							State = 2,
							DevID = this.devID,
							DevName = this.devName,
							Message = text
						};
						pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<CoreBasicsData.BasicsResponse>(basicsResponse2, false) ?? string.Empty);
						await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(text));
					}
					else if (packDetails.Direction[0] != CoreBasicsData.Terminal.客户端 || packDetails.Direction[1] != CoreBasicsData.Terminal.服务端)
					{
						string text = $"[ {packDetails.Direction[0]} ( {base.basics} -> {this.devName} {this.devID} ) ] 请求方向异常，不是有效的方向";
						CoreBasicsData.BasicsResponse basicsResponse3 = new CoreBasicsData.BasicsResponse
						{
							State = 2,
							DevID = this.devID,
							DevName = this.devName,
							Message = text
						};
						pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<CoreBasicsData.BasicsResponse>(basicsResponse3, false) ?? string.Empty);
						await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(text));
					}
					if (pack == null)
					{
						switch (packDetails.Command)
						{
						case CoreBasicsData.Command.获取客户端状态:
							if (ExtendMethod.ToJsonEntity<DataCore.GetClientStatus.Receive>(json, (JsonSerializerOptions)null) != null)
							{
								await stateResponseJson.Writer.WriteAsync(json, token);
							}
							return;
						case CoreBasicsData.Command.数据写入:
							if (ExtendMethod.ToJsonEntity<DataCore.DataWriting.Receive>(json, (JsonSerializerOptions)null) != null)
							{
								await writeResponseJson.Writer.WriteAsync(json, token);
							}
							return;
						}
						string devName = string.Empty;
						string devID = string.Empty;
						if (packDetails.Command == CoreBasicsData.Command.身份验证)
						{
							DataCore.Authentication.Receive receive = ExtendMethod.ToJsonEntity<DataCore.Authentication.Receive>(json, (JsonSerializerOptions)null);
							devName = receive?.DevName ?? string.Empty;
							devID = receive?.DevID ?? string.Empty;
							if (receive != null)
							{
								if (receive.UserName == masterOperateBasics.UserName && receive.Password == masterOperateBasics.Password)
								{
									string text = packDetails.Command.ToString() + "成功";
									DataCore.Authentication.Response response = new DataCore.Authentication.Response
									{
										DevID = devID,
										DevName = devName,
										Message = text,
										State = 1
									};
									pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.Authentication.Response>(response, false) ?? string.Empty);
									this.devID = devID;
									this.devName = devName;
									await authenticationJson.Writer.WriteAsync(json, token);
								}
								else
								{
									string text = packDetails.Command.ToString() + "失败，账号或密码错误";
									DataCore.Authentication.Response response2 = new DataCore.Authentication.Response
									{
										DevID = devID,
										DevName = devName,
										Message = text,
										State = 2
									};
									pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.Authentication.Response>(response2, false) ?? string.Empty);
								}
							}
							else
							{
								string text = packDetails.Command.ToString() + "失败，结构体数据转换失败";
								DataCore.Authentication.Response response3 = new DataCore.Authentication.Response
								{
									DevID = devID,
									DevName = devName,
									Message = text,
									State = 2
								};
								pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.Authentication.Response>(response3, false) ?? string.Empty);
							}
						}
						if (GetAuthenticationStatus())
						{
							switch (packDetails.Command)
							{
							case CoreBasicsData.Command.会话心跳包:
							{
								DataCore.SessionHeartBeat.Receive receive3 = ExtendMethod.ToJsonEntity<DataCore.SessionHeartBeat.Receive>(json, (JsonSerializerOptions)null);
								devName = receive3?.DevName ?? string.Empty;
								devID = receive3?.DevID ?? string.Empty;
								if (receive3 != null)
								{
									string text = packDetails.Command.ToString() + "正常";
									DataCore.SessionHeartBeat.Response response6 = new DataCore.SessionHeartBeat.Response
									{
										DevID = devID,
										DevName = devName,
										Message = text,
										State = 1
									};
									pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.SessionHeartBeat.Response>(response6, false) ?? string.Empty);
									heartbeatTime = DateTime.Now;
								}
								else
								{
									string text = packDetails.Command.ToString() + "异常，结构体数据转换失败";
									DataCore.SessionHeartBeat.Response response7 = new DataCore.SessionHeartBeat.Response
									{
										DevID = devID,
										DevName = devName,
										Message = text,
										State = 2
									};
									pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.SessionHeartBeat.Response>(response7, false) ?? string.Empty);
								}
								break;
							}
							case CoreBasicsData.Command.数据上传:
							{
								DataCore.DataUpload.Receive receive2 = ExtendMethod.ToJsonEntity<DataCore.DataUpload.Receive>(json, (JsonSerializerOptions)null);
								devName = receive2?.DevName ?? string.Empty;
								devID = receive2?.DevID ?? string.Empty;
								if (receive2 != null)
								{
									string text = packDetails.Command.ToString() + "成功";
									DataCore.DataUpload.Response response4 = new DataCore.DataUpload.Response
									{
										DevID = devID,
										DevName = devName,
										Message = text,
										State = 1
									};
									pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.DataUpload.Response>(response4, false) ?? string.Empty);
									foreach (CoreBasicsData.KeyValue item in receive2.AddressValue)
									{
										AValue.AddOrUpdate(item.Key, item.Value, (string k, object v) => item.Value);
									}
								}
								else
								{
									string text = packDetails.Command.ToString() + "异常，结构体数据转换失败";
									DataCore.DataUpload.Response response5 = new DataCore.DataUpload.Response
									{
										DevID = devID,
										DevName = devName,
										Message = text,
										State = 2
									};
									pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<DataCore.DataUpload.Response>(response5, false) ?? string.Empty);
								}
								break;
							}
							}
						}
						else
						{
							string text = packDetails.Command.ToString() + "异常，身份尚未认证";
							CoreBasicsData.BasicsResponse basicsResponse4 = new CoreBasicsData.BasicsResponse
							{
								DevID = devID,
								DevName = devName,
								Message = text,
								State = -1
							};
							pack = Encoding.UTF8.GetBytes(ExtendMethod.ToJson<CoreBasicsData.BasicsResponse>(basicsResponse4, false) ?? string.Empty);
						}
					}
					if (pack == null)
					{
						return;
					}
					Burstification(bytes_Send, pack);
					if (serviceOperate != null)
					{
						await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).SendAsync(bytes_Send.ToArray(), new string[1] { ip }, token);
					}
				}
				catch (Exception ex)
				{
					await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " [ Handle ] 异常：" + ex.Message));
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex4)
			{
				await base.OnInfoEventHandlerAsync((object)this, EventInfoResult.CreateFailureResult(GetMsgHead() + " [ HandleQueue ] 异常：" + ex4.Message));
			}
		}

		/// <summary>
		/// 组包
		/// </summary>
		/// <param name="bytes_Send">发送的数据</param>
		/// <param name="pack">数据包</param>
		private void Burstification(List<byte> bytes_Send, byte[] pack)
		{
			bytes_Send.AddRange(BitConverter.GetBytes(pack.Length));
			bytes_Send.AddRange(pack);
			bytes_Send.AddRange(ByteHandler.GetCRC16(pack, false));
			bytes_Send.AddRange(CoreBasicsData.trail);
		}

		/// <summary>
		/// 获取状态
		/// </summary>
		/// <param name="token">传播消息取消通知</param>
		/// <returns>统一结果</returns>
		private async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await base.BegOperateAsync(token, "GetStatusAsync");
			bool flag = serviceOperate != null;
			if (flag)
			{
				flag = ((BaseModel)(await ((CommunicationServiceAbstract<TcpServiceOperate, Basics>)(object)serviceOperate).GetStatusAsync(token))).Status;
			}
			if (flag)
			{
				return await base.EndOperateAsync(true, "已启动", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetStatusAsync", 918);
			}
			return await base.EndOperateAsync(false, "未启动", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.TEP\\master\\core\\ManageCore.cs", "GetStatusAsync", 920);
		}
	}
}
namespace Snet.TEP.core
{
	/// <summary>
	/// 核芯基础数据
	/// </summary>
	public class CoreBasicsData
	{
		/// <summary>
		/// 命令
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum Command : byte
		{
			/// <summary>
			/// 发送到客户端命令，表示客户端可以发数据给服务端了
			/// </summary>
			握手 = 48,
			/// <summary>
			/// 握手后5秒内必须发送身份认证，不然会被服务端强制踢出
			/// </summary>
			身份验证,
			/// <summary>
			/// 认证后5秒内必须主动请求一次，不然会被服务端强制踢出
			/// </summary>
			会话心跳包,
			/// <summary>
			/// 服务端通过设备名设备ID获取设备状态
			/// </summary>
			获取客户端状态,
			/// <summary>
			/// 客户端主动上传设备采集的数据
			/// </summary>
			数据上传,
			/// <summary>
			/// 服务端下发至客户端写入数据，无法写入直接在Message中表现
			/// </summary>
			数据写入
		}

		/// <summary>
		/// 终端
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum Terminal : byte
		{
			/// <summary>
			/// 客户端
			/// </summary>
			客户端 = 67,
			/// <summary>
			/// 服务端
			/// </summary>
			服务端 = 83
		}

		/// <summary>
		/// 包的详情
		/// </summary>
		public class PackDetails
		{
			/// <summary>
			/// 命令
			/// </summary>
			public Command Command { get; set; }

			/// <summary>
			/// 方向
			/// </summary>
			public List<Terminal> Direction { get; set; }

			/// <summary>
			/// 数据长度；
			/// 动态的；
			/// 如果是分包的话，包的增加数据长度也得增加
			/// </summary>
			public int DataLength { get; set; }

			/// <summary>
			/// 数据包；
			/// 动态的；
			/// 如果是分包的话，直接添加
			/// </summary>
			public byte[] DataPack { get; set; }

			/// <summary>
			/// CRC效验状态
			/// </summary>
			public bool CRCState { get; set; }

			/// <summary>
			/// 完整的数据
			/// </summary>
			public bool IsCompleteData
			{
				get
				{
					if (DataPack.Length == DataLength)
					{
						return CRCState;
					}
					return false;
				}
			}
		}

		/// <summary>
		/// 都需要的数据
		/// </summary>
		public class StructuralBodyBasics
		{
			/// <summary>
			/// 设备名称
			/// </summary>
			public string DevName { get; set; }

			/// <summary>
			/// 设备ID
			/// </summary>
			public string DevID { get; set; }
		}

		/// <summary>
		/// 基础响应
		/// </summary>
		public class BasicsResponse : StructuralBodyBasics
		{
			/// <summary>
			/// 状态；
			/// -1 身份未认证；0 未响应；1 成功/正常；2 异常； 
			/// </summary>
			public int State { get; set; }

			/// <summary>
			/// 消息
			/// </summary>
			public string Message { get; set; }
		}

		/// <summary>
		/// 键值
		/// </summary>
		public class KeyValue
		{
			/// <summary>
			/// 键
			/// </summary>
			public string Key { get; set; }

			/// <summary>
			/// 值
			/// </summary>
			public object Value { get; set; }
		}

		/// <summary>
		/// 收到的字节数据
		/// </summary>
		public class Data
		{
			/// <summary>
			/// Ip端口
			/// </summary>
			public string IpPort { get; set; } = string.Empty;

			/// <summary>
			/// 字节数据
			/// </summary>
			public byte[] Bytes { get; set; }
		}

		private static BoundedChannelOptions? p_Channel;

		/// <summary>
		/// 超时时间（毫秒）
		/// </summary>
		public static int timeOutMS = 5000;

		/// <summary>
		/// 不包含数据包的字节长度
		/// </summary>
		public static int minLength = 17;

		/// <summary>
		/// 数据包起始位长度
		/// </summary>
		public static int dataStartBit = 11;

		/// <summary>
		/// 数据包结束位长度
		/// </summary>
		public static int dataEndBit = 6;

		/// <summary>
		/// 针头
		/// </summary>
		public static readonly byte[] head = new byte[4] { 123, 91, 40, 60 };

		/// <summary>
		/// 针尾
		/// </summary>
		public static readonly byte[] trail = new byte[4] { 62, 41, 93, 125 };

		/// <summary>
		/// 方向：服务端 → 客户端（缓存复用，避免重复分配）
		/// </summary>
		public static readonly byte[] directionServerToClient = new byte[2] { 83, 67 };

		/// <summary>
		/// 方向：客户端 → 服务端（缓存复用，避免重复分配）
		/// </summary>
		public static readonly byte[] directionClientToServer = new byte[2] { 67, 83 };

		/// <summary>
		/// 大数据包的长度
		/// </summary>
		public static int bigDataPackLength = 1048576;

		/// <summary>
		/// 通道配置（延迟创建）
		/// </summary>
		public static BoundedChannelOptions channelOptions
		{
			get
			{
				object obj = p_Channel;
				if (obj == null)
				{
					obj = new BoundedChannelOptions(65535)
					{
						FullMode = BoundedChannelFullMode.Wait,
						SingleReader = false,
						SingleWriter = false
					};
					p_Channel = (BoundedChannelOptions?)obj;
				}
				return (BoundedChannelOptions)obj;
			}
		}
	}
	/// <summary>
	/// 核芯包处理
	/// </summary>
	public class CorePackHandle
	{
		/// <summary>
		/// 获取完整的包；
		/// 并返回没有被匹配的数据，给外部更新在缓存中，以便下次数据的完整性
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="header">包头</param>
		/// <param name="footer">包尾</param>
		/// <param name="remainingData">没有匹配的数据</param>
		/// <returns></returns>
		public List<byte[]> GetFullPack(byte[] data, byte[] header, byte[] footer, out byte[] remainingData)
		{
			List<byte[]> list = new List<byte[]>();
			int num = 0;
			int num2 = data.Length;
			int[] lps = ComputeLpsArray(header);
			int[] lps2 = ComputeLpsArray(footer);
			while (num < num2)
			{
				int num3 = KmpIndexOf(data, header, num, lps);
				if (num3 == -1)
				{
					break;
				}
				int num4 = KmpIndexOf(data, footer, num3 + header.Length, lps2);
				if (num4 == -1)
				{
					break;
				}
				int num5 = num4 + footer.Length - num3;
				byte[] array = new byte[num5];
				Buffer.BlockCopy(data, num3, array, 0, num5);
				list.Add(array);
				num = num4 + footer.Length;
			}
			if (num < num2)
			{
				remainingData = new byte[num2 - num];
				Buffer.BlockCopy(data, num, remainingData, 0, num2 - num);
			}
			else
			{
				remainingData = Array.Empty<byte>();
			}
			return list;
		}

		/// <summary>
		/// 使用KMP算法查找子数组的索引
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="pattern">需要查找的字节</param>
		/// <param name="startIndex">起始下标</param>
		/// <returns></returns>
		private int KmpIndexOf(byte[] data, byte[] pattern, int startIndex, int[] lps)
		{
			int num = startIndex;
			int num2 = 0;
			int num3 = data.Length;
			int num4 = pattern.Length;
			while (num < num3)
			{
				if (pattern[num2] == data[num])
				{
					num2++;
					num++;
				}
				if (num2 == num4)
				{
					return num - num2;
				}
				if (num < num3 && pattern[num2] != data[num])
				{
					if (num2 != 0)
					{
						num2 = lps[num2 - 1];
					}
					else
					{
						num++;
					}
				}
			}
			return -1;
		}

		/// <summary>
		///  计算部分匹配表（前缀表）
		/// </summary>
		/// <param name="pattern">需要查找的字节</param>
		/// <returns></returns>
		private int[] ComputeLpsArray(byte[] pattern)
		{
			int num = pattern.Length;
			int[] array = new int[num];
			int num2 = 0;
			int num3 = 1;
			array[0] = 0;
			while (num3 < num)
			{
				if (pattern[num3] == pattern[num2])
				{
					num2 = (array[num3] = num2 + 1);
					num3++;
				}
				else if (num2 != 0)
				{
					num2 = array[num2 - 1];
				}
				else
				{
					array[num3] = 0;
					num3++;
				}
			}
			return array;
		}

		/// <summary>
		/// 解析包
		/// </summary>
		/// <param name="bytes">字节数据，必须是一个完整的包</param>
		/// <returns>包详情</returns>
		public CoreBasicsData.PackDetails? ParsePackDetails(byte[] bytes)
		{
			CoreBasicsData.PackDetails packDetails = new CoreBasicsData.PackDetails();
			packDetails.Command = (CoreBasicsData.Command)bytes[CoreBasicsData.dataStartBit - 7];
			packDetails.Direction = new List<CoreBasicsData.Terminal>
			{
				(CoreBasicsData.Terminal)bytes[CoreBasicsData.dataStartBit - 6],
				(CoreBasicsData.Terminal)bytes[CoreBasicsData.dataStartBit - 5]
			};
			packDetails.DataLength = BitConverter.ToInt32(bytes, CoreBasicsData.dataStartBit - 4);
			int num = bytes.Length - CoreBasicsData.minLength;
			byte[] array = new byte[num];
			Buffer.BlockCopy(bytes, CoreBasicsData.dataStartBit, array, 0, num);
			packDetails.DataPack = array;
			byte[] cRC = ByteHandler.GetCRC16(packDetails.DataPack, false);
			if (bytes[^CoreBasicsData.dataEndBit] == cRC[0] && bytes[^(CoreBasicsData.dataEndBit - 1)] == cRC[1])
			{
				packDetails.CRCState = true;
			}
			return packDetails;
		}
	}
}
