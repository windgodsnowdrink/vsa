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
using Snet.Core.@abstract;
using Snet.Core.extend;
using Snet.Core.handler;
using Snet.Core.subscription;
using Snet.Core.virtualAddress;
using Snet.Driver;
using Snet.Driver.Core.Device;
using Snet.Driver.Core.Net;
using Snet.Driver.Core.Pipe;
using Snet.Driver.Instrument.Temperature;
using Snet.Driver.LogNet;
using Snet.Log;
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
[assembly: AssemblyDescription("采集协议：YuDian（宇电AIBus温度采集模块通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.YuDian")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.YuDian;

/// <summary>
/// 宇电数据
/// </summary>
public class YuDianData
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
		/// 站号信息
		/// </summary>
		[Description("站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Station { get; set; } = 1;

		/// <summary>
		/// 串口信息
		/// </summary>
		[Description("串口 信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? SerialPortInfo { get; set; } = "COM3-9600-8-N-1";

		/// <summary>
		/// 请求发送 (RTS) 信号
		/// </summary>
		[Description("请求发送 (RTS) 信号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool RtsEnable { get; set; }

		/// <summary>
		/// 启用数据终端
		/// </summary>
		[Description("启用数据终端")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool DtrEnable { get; set; }

		/// <summary>
		/// 休息的时间
		/// </summary>
		[Description("休息的时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int SleepTime { get; set; }

		/// <summary>
		/// 接收超时时间
		/// </summary>
		[Description(" 接收超时时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int ReceiveTimeOut { get; set; } = 1000;

		/// <summary>
		/// 协议类型
		/// </summary>
		[Description("协议类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[AutoAllocatingTag(typeof(ProtocolType))]
		[Display(/*Could not decode attribute arguments.*/)]
		public ProtocolType ProtocolType { get; set; }
	}

	/// <summary>
	/// 协议类型
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ProtocolType
	{
		/// <summary>
		/// 宇电AIBus协议温度采集模块，基于串口通讯实现。
		/// </summary>
		[Description("宇电AIBus协议温度采集模块，基于串口通讯实现。")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "Station", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime",
			"ReceiveTimeOut"
		})]
		YuDianAIBus
	}
}
/// <summary>
/// 宇电通信库
/// </summary>
public class YuDianOperate : DaqAbstract<YuDianOperate, YuDianData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// Plc连接对象
	/// </summary>
	private object? plcBase;

	/// <summary>
	/// plc日志操作对象
	/// </summary>
	private LogOperate plcLog;

	/// <summary>
	/// 实现订阅功能
	/// </summary>
	private SubscribeOperate? subscribeOperate;

	/// <summary>
	/// 虚拟地址
	/// </summary>
	private VirtualAddressManage VAM;

	/// <inheritdoc />
	protected override string CN => "宇电";

	/// <inheritdoc />
	protected override string CD => "支持对“宇电AIBus温度采集模块”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public YuDianOperate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		VAM = new VirtualAddressManage();
		base..ctor();
		LanguageHandler();
	}

	/// <summary>
	/// 有参构造函数
	/// </summary>
	/// <param name="basics">基础数据</param>
	public YuDianOperate(YuDianData.Basics basics)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		VAM = new VirtualAddressManage();
		base..ctor(basics);
		LanguageHandler();
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
			{
				YuDianAIBus val = new YuDianAIBus();
				val.Station = (byte)((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.Station;
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.SleepTime,
					ReceiveTimeOut = ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ReceiveTimeOut
				};
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs");
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OnAsync", 99);
			}
			await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OnAsync", 103);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OnAsync", 108);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OffAsync", 122);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OffAsync", 131);
				}
				subscribeOperate = null;
			}
			VirtualAddressManage vAM = VAM;
			if (vAM != null)
			{
				vAM.DisposeAsync();
			}
			if (plcBase != null)
			{
				try
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						((DeviceSerialPort)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).Dispose();
					}
				}
				catch (Exception)
				{
				}
				plcBase = null;
				LogOperate obj = plcLog;
				if (obj != null)
				{
					obj.Dispose();
				}
				plcLog = null;
			}
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OffAsync", 157);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "OffAsync", 161);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "ReadAsync", 173);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "ReadAsync", 178);
			}
			ConcurrentDictionary<string, AddressValue> param = new ConcurrentDictionary<string, AddressValue>();
			foreach (AddressDetails item in address.AddressArray)
			{
				if (!item.IsEnable)
				{
					continue;
				}
				string Msg = await LanguageHandler.GetLanguageValueAsync("失败", (LanguageModel)null, token);
				bool flag = false;
				VAM.InitVirtualAddress(item, ref flag);
				object Value = null;
				string addressName = item.AddressName;
				ushort length = item.Length;
				Encoding encoding = CoreExtend.GetEncoding(item.EncodingType);
				if (flag)
				{
					Value = VAM.Read(item);
				}
				else
				{
					try
					{
						DataType addressDataType = item.AddressDataType;
						switch (addressDataType - 1)
						{
						case 0:
						{
							OResult<byte> val16 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val16 = null;
							}
							if (val16 != null)
							{
								if (((OResult)val16).IsSuccess)
								{
									Value = val16.Content;
									Msg = ((OResult)val16).Message;
								}
								else
								{
									Msg = ((OResult)val16).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val20 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadBoolAsync(addressName);
							}
							if (val20 != null)
							{
								if (((OResult)val20).IsSuccess)
								{
									Value = val20.Content;
									Msg = ((OResult)val20).Message;
								}
								else
								{
									Msg = ((OResult)val20).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val11 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadDoubleAsync(addressName);
							}
							if (val11 != null)
							{
								if (((OResult)val11).IsSuccess)
								{
									Value = val11.Content;
									Msg = ((OResult)val11).Message;
								}
								else
								{
									Msg = ((OResult)val11).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val5 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadFloatAsync(addressName);
							}
							if (val5 != null)
							{
								if (((OResult)val5).IsSuccess)
								{
									Value = val5.Content;
									Msg = ((OResult)val5).Message;
								}
								else
								{
									Msg = ((OResult)val5).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val21 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadInt32Async(addressName);
							}
							if (val21 != null)
							{
								if (((OResult)val21).IsSuccess)
								{
									Value = val21.Content;
									Msg = ((OResult)val21).Message;
								}
								else
								{
									Msg = ((OResult)val21).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val14 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadUInt32Async(addressName);
							}
							if (val14 != null)
							{
								if (((OResult)val14).IsSuccess)
								{
									Value = val14.Content;
									Msg = ((OResult)val14).Message;
								}
								else
								{
									Msg = ((OResult)val14).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val7 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadInt64Async(addressName);
							}
							if (val7 != null)
							{
								if (((OResult)val7).IsSuccess)
								{
									Value = val7.Content;
									Msg = ((OResult)val7).Message;
								}
								else
								{
									Msg = ((OResult)val7).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val12 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadUInt64Async(addressName);
							}
							if (val12 != null)
							{
								if (((OResult)val12).IsSuccess)
								{
									Value = val12.Content;
									Msg = ((OResult)val12).Message;
								}
								else
								{
									Msg = ((OResult)val12).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val17 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadInt16Async(addressName);
							}
							if (val17 != null)
							{
								if (((OResult)val17).IsSuccess)
								{
									Value = val17.Content;
									Msg = ((OResult)val17).Message;
								}
								else
								{
									Msg = ((OResult)val17).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val4 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadUInt16Async(addressName);
							}
							if (val4 != null)
							{
								if (((OResult)val4).IsSuccess)
								{
									Value = val4.Content;
									Msg = ((OResult)val4).Message;
								}
								else
								{
									Msg = ((OResult)val4).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val6 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadStringAsync(addressName, length, encoding);
							}
							if (val6 != null)
							{
								if (((OResult)val6).IsSuccess)
								{
									Value = val6.Content;
									Msg = ((OResult)val6).Message;
								}
								else
								{
									Msg = ((OResult)val6).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val19 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadAsync(addressName, length);
							}
							if (val19 != null)
							{
								if (((OResult)val19).IsSuccess)
								{
									Value = val19.Content;
									Msg = ((OResult)val19).Message;
								}
								else
								{
									Msg = ((OResult)val19).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val2 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadBoolAsync(addressName, length);
							}
							if (val2 != null)
							{
								if (((OResult)val2).IsSuccess)
								{
									Value = val2.Content;
									Msg = ((OResult)val2).Message;
								}
								else
								{
									Msg = ((OResult)val2).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val13 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadDoubleAsync(addressName, length);
							}
							if (val13 != null)
							{
								if (((OResult)val13).IsSuccess)
								{
									Value = val13.Content;
									Msg = ((OResult)val13).Message;
								}
								else
								{
									Msg = ((OResult)val13).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val10 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadFloatAsync(addressName, length);
							}
							if (val10 != null)
							{
								if (((OResult)val10).IsSuccess)
								{
									Value = val10.Content;
									Msg = ((OResult)val10).Message;
								}
								else
								{
									Msg = ((OResult)val10).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val18 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadInt16Async(addressName, length);
							}
							if (val18 != null)
							{
								if (((OResult)val18).IsSuccess)
								{
									Value = val18.Content;
									Msg = ((OResult)val18).Message;
								}
								else
								{
									Msg = ((OResult)val18).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val9 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val9 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadUInt16Async(addressName, length);
							}
							if (val9 != null)
							{
								if (((OResult)val9).IsSuccess)
								{
									Value = val9.Content;
									Msg = ((OResult)val9).Message;
								}
								else
								{
									Msg = ((OResult)val9).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val3 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadInt32Async(addressName, length);
							}
							if (val3 != null)
							{
								if (((OResult)val3).IsSuccess)
								{
									Value = val3.Content;
									Msg = ((OResult)val3).Message;
								}
								else
								{
									Msg = ((OResult)val3).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val15 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadUInt32Async(addressName, length);
							}
							if (val15 != null)
							{
								if (((OResult)val15).IsSuccess)
								{
									Value = val15.Content;
									Msg = ((OResult)val15).Message;
								}
								else
								{
									Msg = ((OResult)val15).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val8 = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadInt64Async(addressName, length);
							}
							if (val8 != null)
							{
								if (((OResult)val8).IsSuccess)
								{
									Value = val8.Content;
									Msg = ((OResult)val8).Message;
								}
								else
								{
									Msg = ((OResult)val8).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
							{
								val = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).ReadUInt64Async(addressName, length);
							}
							if (val != null)
							{
								if (((OResult)val).IsSuccess)
								{
									Value = val.Content;
									Msg = ((OResult)val).Message;
								}
								else
								{
									Msg = ((OResult)val).Message;
								}
							}
							else
							{
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						}
					}
					catch (Exception ex)
					{
						Msg = await LanguageHandler.GetLanguageValueAsync("读取异常", (LanguageModel)null, token) + "：" + ex.Message;
					}
				}
				AddressValue addressValue = AddressHandler.ExecuteDispose(item, Value, Msg);
				param.AddOrUpdate(addressName, addressValue, (string k, AddressValue v) => addressValue);
			}
			if (param.Count > 0)
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "ReadAsync", 811);
			}
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "ReadAsync", 815);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "ReadAsync", 820);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "WriteAsync", 832);
			}
			List<string> FailMessage = new List<string>();
			foreach (KeyValuePair<string, (object, EncodingType?)> value2 in values)
			{
				string key;
				if (VAM.IsVirtualAddress(value2.Key))
				{
					if (!VAM.Write(value2.Key, value2.Value.Item1))
					{
						List<string> list = FailMessage;
						key = value2.Key;
						list.Add(key + "，" + await LanguageHandler.GetLanguageValueAsync("写入失败", (LanguageModel)null, token));
					}
					continue;
				}
				key = value2.Key;
				object value = value2.Value.Item1;
				Encoding encoding = CoreExtend.GetEncoding(value2.Value.Item2);
				OResult oResult = null;
				if (value is byte)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
					}
				}
				else if (value is byte[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
					}
				}
				else if (value is short || value is short)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
					}
				}
				else if (value is short[] || value is short[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
					}
				}
				else if (value is ushort || value is ushort)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
					}
				}
				else if (value is int || value is int)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
					}
				}
				else if (value is int[] || value is int[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
					}
				}
				else if (value is uint || value is uint)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
					}
				}
				else if (value is uint[] || value is uint[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
					}
				}
				else if (value is long || value is long)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
					}
				}
				else if (value is long[] || value is long[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
					}
				}
				else if (value is ulong || value is ulong)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
					}
				}
				else if (value is float || value is float)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
					}
				}
				else if (value is float[] || value is float[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
					}
				}
				else if (value is double)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
					}
				}
				else if (value is double[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
					}
				}
				else if (value is bool)
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
					}
				}
				else if (value is bool[])
				{
					if (((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
					{
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
					}
				}
				else if ((value is string || value is char) && ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.ProtocolType == YuDianData.ProtocolType.YuDianAIBus)
				{
					oResult = await ((DeviceCommunication)ExtendMethod.GetSource<YuDianAIBus>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
				}
				if (oResult != null)
				{
					if (!oResult.IsSuccess)
					{
						List<string> list = FailMessage;
						string text2 = key;
						string text3 = await LanguageHandler.GetLanguageValueAsync("写入失败", (LanguageModel)null, token);
						list.Add("[ " + text2 + " ]" + text3 + "：" + oResult.Message);
					}
				}
				else
				{
					List<string> list = FailMessage;
					string text2 = key;
					string text4 = await LanguageHandler.GetLanguageValueAsync("写入失败", (LanguageModel)null, token);
					string text5 = await LanguageHandler.GetLanguageValueAsync("不支持写入", (LanguageModel)null, token);
					string name = value.GetType().Name;
					string text3 = await LanguageHandler.GetLanguageValueAsync("类型", (LanguageModel)null, token);
					list.Add("[ " + text2 + " ] " + text4 + "：" + text5 + "(" + name + ")" + text3);
				}
			}
			if (FailMessage.Count > 0)
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "WriteAsync", 1064);
			}
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "WriteAsync", 1067);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "WriteAsync", 1071);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "SubscribeAsync", 1083);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "SubscribeAsync", 1088);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "SubscribeAsync", 1096);
			}
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "SubscribeAsync", 1100);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "SubscribeAsync", 1105);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "UnSubscribeAsync", 1117);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "UnSubscribeAsync", 1122);
			}
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "UnSubscribeAsync", 1126);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "UnSubscribeAsync", 1131);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
		string connectedMessage = await LanguageHandler.GetLanguageValueAsync("已连接", (LanguageModel)null, token);
		string unconnectedMessage = await LanguageHandler.GetLanguageValueAsync("未连接", (LanguageModel)null, token);
		object? obj = plcBase;
		object? obj2 = ((obj is DeviceTcpNet) ? obj : null);
		CommunicationPipe obj3 = ((obj2 != null) ? ((BinaryCommunication)obj2).CommunicationPipe : null);
		PipeTcpNet tcpNet = (PipeTcpNet)(object)((obj3 is PipeTcpNet) ? obj3 : null);
		if (tcpNet != null)
		{
			bool isSuccess = ((OResult)(await ((CommunicationPipe)tcpNet).OpenCommunicationAsync())).IsSuccess;
			if (isSuccess)
			{
				isSuccess = (await ((CommunicationPipe)tcpNet).SendAsync(Array.Empty<byte>())).IsSuccess;
			}
			bool flag = isSuccess;
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "GetStatusAsync", 1145);
		}
		object? obj4 = plcBase;
		object? obj5 = ((obj4 is DeviceSerialPort) ? obj4 : null);
		CommunicationPipe obj6 = ((obj5 != null) ? ((BinaryCommunication)obj5).CommunicationPipe : null);
		PipeSerialPort serialPort = (PipeSerialPort)(object)((obj6 is PipeSerialPort) ? obj6 : null);
		if (serialPort != null)
		{
			bool isSuccess = ((OResult)(await ((CommunicationPipe)serialPort).OpenCommunicationAsync())).IsSuccess;
			if (isSuccess)
			{
				isSuccess = (await ((CommunicationPipe)serialPort).SendAsync(Array.Empty<byte>())).IsSuccess;
			}
			bool flag2 = isSuccess;
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "GetStatusAsync", 1152);
		}
		object? obj7 = plcBase;
		object? obj8 = ((obj7 is DeviceUdpNet) ? obj7 : null);
		CommunicationPipe obj9 = ((obj8 != null) ? ((BinaryCommunication)obj8).CommunicationPipe : null);
		PipeUdpNet udpNet = (PipeUdpNet)(object)((obj9 is PipeUdpNet) ? obj9 : null);
		if (udpNet != null)
		{
			bool isSuccess = ((OResult)(await ((CommunicationPipe)udpNet).OpenCommunicationAsync())).IsSuccess;
			if (isSuccess)
			{
				isSuccess = (await ((CommunicationPipe)udpNet).SendAsync(Array.Empty<byte>())).IsSuccess;
			}
			bool flag3 = isSuccess;
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "GetStatusAsync", 1159);
		}
		return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "GetStatusAsync", 1162);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<YuDianOperate, YuDianData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "GetBaseObjectAsync", 1171);
		}
		return await ((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.YuDian\\YuDianOperate.cs", "GetBaseObjectAsync", 1174);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<YuDianOperate, YuDianData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
	}

	/// <summary>
	/// 语言切换触发事件
	/// </summary>
	/// <param name="sender">源</param>
	/// <param name="e">事件</param>
	private Task LanguageEventAsync(object? sender, EventLanguageResult e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		LanguageType? language = e.Language;
		LanguageType valueOrDefault = language.GetValueOrDefault();
		LanguageType val;
		if (!language.HasValue)
		{
			valueOrDefault = (LanguageType)0;
			LanguageType? language2 = valueOrDefault;
			e.Language = language2;
			val = valueOrDefault;
		}
		else
		{
			val = valueOrDefault;
		}
		LanguageType val2 = val;
		if ((int)val2 != 0)
		{
			if ((int)val2 == 1)
			{
				StringResources.SeteLanguageEnglish();
			}
		}
		else
		{
			StringResources.SetLanguageChinese();
		}
		return Task.CompletedTask;
	}
}
