using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Snet.Core.@abstract;
using Snet.Core.extend;
using Snet.Core.handler;
using Snet.Core.subscription;
using Snet.Core.virtualAddress;
using Snet.Log;
using Snet.Model.attribute;
using Snet.Model.data;
using Snet.Model.@enum;
using Snet.Model.@interface;
using Snet.PerformanceTesting;
using Snet.Utility;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("采集协议：NetMQ 开发的性能测试（专用库）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.PerformanceTesting")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace YSAI.PerformanceTesting
{
	/// <summary>
	/// 性能测试操作
	/// </summary>
	public class PerformanceTestingOperate : DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// netMQ 通信类
		/// </summary>
		private SubscriberSocket? subscriber;

		/// <summary>
		/// 实现订阅功能
		/// </summary>
		private SubscribeOperate? subscribeOperate;

		/// <summary>
		/// 虚拟地址
		/// </summary>
		private VirtualAddressManage VAM;

		/// <summary>
		/// 数据队列
		/// </summary>
		private Channel<AddressValue> DataQueue;

		/// <summary>
		/// 任务取消
		/// </summary>
		private CancellationTokenSource? g_token;

		/// <summary>
		/// 容器
		/// </summary>
		private ConcurrentDictionary<string, AddressDetails> ioc;

		/// <summary>
		/// 数据
		/// </summary>
		private ConcurrentDictionary<string, AddressValue> values;

		private BoundedChannelOptions? p_Channel;

		/// <inheritdoc />
		protected override string CN => "性能测试";

		/// <inheritdoc />
		protected override string CD => "“性能测试”只支持数据订阅模式";

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
		/// 无惨构造函数
		/// </summary>
		public PerformanceTestingOperate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			VAM = new VirtualAddressManage();
			ioc = new ConcurrentDictionary<string, AddressDetails>();
			values = new ConcurrentDictionary<string, AddressValue>();
			base..ctor();
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public PerformanceTestingOperate(PerformanceTestingData.Basics basics)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			VAM = new VirtualAddressManage();
			ioc = new ConcurrentDictionary<string, AddressDetails>();
			values = new ConcurrentDictionary<string, AddressValue>();
			base..ctor(basics);
		}

		/// <summary>
		/// 任务处理
		/// </summary>
		/// <param name="token">任务令牌</param>
		/// <returns>任务</returns>
		private async Task TaskHandleAsync(CancellationToken token)
		{
			try
			{
				while (await DataQueue.Reader.WaitToReadAsync(token))
				{
					while (true)
					{
						if (!DataQueue.Reader.TryRead(out AddressValue queueData))
						{
							break;
						}
						if (queueData == null || token.IsCancellationRequested)
						{
							continue;
						}
						try
						{
							values.AddOrUpdate(((AddressDetails)queueData).AddressName, queueData, (string k, AddressValue v) => queueData);
						}
						catch (Exception ex)
						{
							await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "[ 订阅通知 ]异常:" + ex.Message));
						}
					}
				}
			}
			catch (TaskCanceledException)
			{
				await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "[ 订阅通知 ]已停止"));
			}
			catch (OperationCanceledException)
			{
				await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "[ 订阅通知 ]已停止"));
			}
			catch (Exception ex4)
			{
				await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "[ 订阅通知 ]异常:" + ex4.Message));
			}
		}

		/// <summary>
		/// 订阅消息处理
		/// </summary>
		/// <param name="token">生命周期</param>
		/// <returns></returns>
		private async Task SubMessageHandleAsync(CancellationToken token)
		{
			await Task.Run(async delegate
			{
				string text = default(string);
				bool flag = default(bool);
				while (!token.IsCancellationRequested)
				{
					try
					{
						SubscriberSocket val = subscriber;
						if (val != null && ReceivingSocketExtensions.TryReceiveFrameString((IReceivingSocket)(object)val, TimeSpan.FromMilliseconds(((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics.TimeOut), ref text, ref flag) && text.Equals(((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics.Topic))
						{
							PerformanceTestingData.Pack[] array = ExtendMethod.ToJsonEntity<PerformanceTestingData.Pack[]>(ReceivingSocketExtensions.ReceiveFrameString((IReceivingSocket)(object)val), (JsonSerializerOptions)null);
							if (array != null && array.Length != 0)
							{
								PerformanceTestingData.Pack[] array2 = array;
								foreach (PerformanceTestingData.Pack pack in array2)
								{
									ioc.TryGetValue(pack.AddressName, out AddressDetails value);
									if (value == null)
									{
										await LogHelper.ErrorAsync("未找到 " + pack.AddressName + " 的缓存数据 ( 底层驱动输出 )", (string)null, (Exception)null, true, token);
									}
									else
									{
										AddressValue item = AddressHandler.ExecuteDispose(value, pack.Value, "成功");
										await DataQueue.Writer.WriteAsync(item);
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OnInfoEventHandlerAsync((object)this, new EventInfoResult(false, "消费异常:" + ex.Message));
					}
				}
			}, token);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
			try
			{
				string text = default(string);
				if (((ResultModel)(await ((DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "OnAsync", 195);
				}
				if (subscriber == null)
				{
					subscriber = new SubscriberSocket((string)null);
				}
				if (DataQueue == null)
				{
					DataQueue = Channel.CreateBounded<AddressValue>(channelOptions);
					g_token = new CancellationTokenSource();
					SubMessageHandleAsync(g_token.Token).ConfigureAwait(continueOnCapturedContext: false);
					TaskHandleAsync(g_token.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				((NetMQSocket)subscriber).Connect(((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics.Uri);
				SubscriberSocket? obj = subscriber;
				if (obj != null)
				{
					obj.Subscribe(((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics.Topic);
				}
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "OnAsync", 210);
			}
			catch (Exception ex)
			{
				await ((DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OffAsync(true, token);
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "OnAsync", 215);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
			try
			{
				string text = default(string);
				if (!hardClose && !((ResultModel)(await ((DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "OffAsync", 228);
				}
				await (g_token?.CancelAsync());
				g_token?.Dispose();
				g_token = null;
				if (subscribeOperate != null)
				{
					OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
					if (!((BaseModel)val).Status)
					{
						return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "OffAsync", 241);
					}
					subscribeOperate = null;
				}
				VirtualAddressManage vAM = VAM;
				if (vAM != null)
				{
					vAM.DisposeAsync();
				}
				if (DataQueue != null)
				{
					AddressValue item;
					while (DataQueue.Reader.TryRead(out item))
					{
					}
					DataQueue = null;
				}
				SubscriberSocket? obj = subscriber;
				if (obj != null)
				{
					((NetMQSocket)obj).Close();
				}
				SubscriberSocket? obj2 = subscriber;
				if (obj2 != null)
				{
					((NetMQSocket)obj2).Dispose();
				}
				subscriber = null;
				ioc?.Clear();
				values?.Clear();
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "OffAsync", 263);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "OffAsync", 267);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
			try
			{
				ConcurrentDictionary<string, AddressValue> param = new ConcurrentDictionary<string, AddressValue>();
				foreach (AddressDetails item in address.AddressArray)
				{
					if (!item.IsEnable)
					{
						continue;
					}
					string text = await LanguageHandler.GetLanguageValueAsync("失败", (LanguageModel)null, token);
					bool flag = false;
					VAM.InitVirtualAddress(item, ref flag);
					string addressName = item.AddressName;
					if (flag)
					{
						object obj = VAM.Read(item);
						AddressValue addressValue = AddressHandler.ExecuteDispose(item, obj, text);
						param.AddOrUpdate(addressName, addressValue, (string k, AddressValue v) => addressValue);
						continue;
					}
					ioc.AddOrUpdate(addressName, item, (string k, AddressDetails v) => item);
					if (values.TryGetValue(addressName, out AddressValue value) && value != null)
					{
						param.AddOrUpdate(addressName, value, (string k, AddressValue v) => value);
					}
				}
				if (param.Count > 0)
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "ReadAsync", 316);
				}
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "ReadAsync", 320);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "ReadAsync", 325);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
			try
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, "不支持此功能", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "WriteAsync", 334);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "WriteAsync", 338);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await ((DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "SubscribeAsync", 349);
				}
				if (!address.CheckAddress())
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "SubscribeAsync", 354);
				}
				if (subscribeOperate == null)
				{
					subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
					{
						Address = address,
						ChangeOut = ((SCData)((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics).ChangeOut,
						FunctionAsync = ((DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).ReadAsync,
						AllOut = ((SCData)((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics).AllOut,
						HandleInterval = ((SCData)((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics).HandleInterval,
						SN = ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics.SN,
						TaskNumber = ((SCData)((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).basics).TaskNumber
					}, default(CancellationToken));
					((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEvent += ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OnDataEventHandler;
					((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEvent += ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).OnInfoEventHandler;
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "SubscribeAsync", 362);
				}
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "SubscribeAsync", 366);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "SubscribeAsync", 371);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await ((DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "UnSubscribeAsync", 382);
				}
				if (subscribeOperate != null)
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "UnSubscribeAsync", 387);
				}
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "UnSubscribeAsync", 391);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "UnSubscribeAsync", 396);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
			try
			{
				if (subscriber == null)
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("未连接", (LanguageModel)null, token), (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "GetStatusAsync", 407);
				}
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("已连接", (LanguageModel)null, token), (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "GetStatusAsync", 409);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "GetStatusAsync", 413);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
			try
			{
				string text = default(string);
				if (!((ResultModel)(await ((DaqAbstract<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
				{
					return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "GetBaseObjectAsync", 424);
				}
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)subscriber, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "GetBaseObjectAsync", 426);
			}
			catch (Exception ex)
			{
				return await ((CoreUnify<PerformanceTestingOperate, PerformanceTestingData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PerformanceTesting\\PerformanceTestingOperate.cs", "GetBaseObjectAsync", 430);
			}
		}
	}
}
namespace Snet.PerformanceTesting
{
	/// <summary>
	/// 性能测试数据
	/// </summary>
	public class PerformanceTestingData
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
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 地址
			/// </summary>
			[Description("地址")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string? Uri { get; set; } = "tcp://127.0.0.1:8866";

			/// <summary>
			/// 主题
			/// </summary>
			[Description("主题")]
			[Display(/*Could not decode attribute arguments.*/)]
			public string Topic { get; set; } = "PerformanceTesting";

			/// <summary>
			/// 超时时间
			/// </summary>
			[Description("超时时间")]
			[Unit("ms")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int TimeOut { get; set; } = 1000;
		}

		/// <summary>
		/// 数据包
		/// </summary>
		public class Pack
		{
			/// <summary>
			/// 点位地址
			/// </summary>
			public string AddressName { get; set; }

			/// <summary>
			/// 点位的值
			/// </summary>
			public object Value { get; set; }
		}
	}
}
