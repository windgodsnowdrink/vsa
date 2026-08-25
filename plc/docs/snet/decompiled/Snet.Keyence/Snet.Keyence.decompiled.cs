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
using Snet.Driver.LogNet;
using Snet.Driver.Profinet.Keyence;
using Snet.Driver.Profinet.Melsec;
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
[assembly: AssemblyDescription("采集协议：Keyence（基恩士 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.Keyence")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Keyence;

/// <summary>
/// 基恩士数据
/// </summary>
public class KeyenceData
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
		/// ip地址
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
		/// 连接超时时间
		/// </summary>
		[Description(" 连接超时时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int ConnectTimeOut { get; set; } = 1000;

		/// <summary>
		/// 接收超时时间
		/// </summary>
		[Description(" 接收超时时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int ReceiveTimeOut { get; set; } = 1000;

		/// <summary>
		/// 休息的时间
		/// </summary>
		[Description("休息的时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int SleepTime { get; set; }

		/// <summary>
		/// 心跳检测
		/// </summary>
		[Description("心跳检测")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int SocketKeepAliveTime { get; set; } = -1;

		/// <summary>
		/// 管道是否是长连接
		/// </summary>
		[Description("管道是否是长连接")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsPersistentConnection { get; set; } = true;

		/// <summary>
		/// 写入位到字寄存器功能
		/// </summary>
		[Description("写入位到字寄存器功能")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool EnableWriteBitToWordRegister { get; set; } = true;

		/// <summary>
		/// 字节按照字单位反转
		/// </summary>
		[Description("字节按照字单位反转")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsStringReverseByteWord { get; set; }

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
		/// 启用站号信息
		/// </summary>
		[Description("启用 站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool UseStation { get; set; }

		/// <summary>
		/// 站号信息
		/// </summary>
		[Description("站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Station { get; set; } = 1;

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
		/// 基恩士PLC 使用QnA兼容3E帧的通信协议实现，使用二进制的格式，地址同时支持三菱的地址格式及基恩士自身的地址格式
		/// </summary>
		[Description("基恩士PLC 使用QnA兼容3E帧的通信协议实现，使用二进制的格式，地址同时支持三菱的地址格式及基恩士自身的地址格式")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "EnableWriteBitToWordRegister", "IsStringReverseByteWord"
		})]
		KeyenceMcNet,
		/// <summary>
		/// 基恩士PLC 使用QnA兼容3E帧的通信协议实现，使用ASCII的格式，地址格式需要进行转换成三菱的格式
		/// </summary>
		[Description("基恩士PLC 使用QnA兼容3E帧的通信协议实现，使用ASCII的格式，地址格式需要进行转换成三菱的格式")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "EnableWriteBitToWordRegister", "IsStringReverseByteWord"
		})]
		KeyenceMcAsciiNet,
		/// <summary>
		/// 基恩士PLC KV上位链路串口通信的对象,适用于Nano系列串口数据,KV1000以及L20V通信模块
		/// </summary>
		[Description("基恩士PLC KV上位链路串口通信的对象,适用于Nano系列串口数据,KV1000以及L20V通信模块")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "UseStation",
			"Station"
		})]
		KeyenceNanoSerial,
		/// <summary>
		/// 基恩士PLC KV上位链路协议的通信对象,适用于KV5000/5500/3000,KV1000,KV700,以及L20V通信模块，本类是基于tcp通信
		/// </summary>
		[Description("基恩士PLC KV上位链路协议的通信对象,适用于KV5000/5500/3000,KV1000,KV700,以及L20V通信模块，本类是基于tcp通信")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "UseStation", "Station"
		})]
		KeyenceNanoSerialOverTcp,
		/// <summary>
		/// 老型号的基恩士通信，适用于 Kv-10xx, 16xx, 24xx, 40xx, kv-80, kv-300，其中 xx 表示 AR/AT/DR/DT
		/// </summary>
		[Description("老型号的基恩士通信，适用于 Kv-10xx, 16xx, 24xx, 40xx, kv-80, kv-300，其中 xx 表示 AR/AT/DR/DT")]
		[AutoAllocating(new string[] { "SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut" })]
		KeyenceKvOld
	}
}
/// <summary>
/// 基恩士通信库
/// </summary>
public class KeyenceOperate : DaqAbstract<KeyenceOperate, KeyenceData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "基恩士";

	/// <inheritdoc />
	protected override string CD => "支持对“基恩士 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public KeyenceOperate()
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
	public KeyenceOperate(KeyenceData.Basics basics)
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
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
			{
			case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
			{
				KeyenceNanoSerialOverTcp keyenceNanoSerialOverTcp = new KeyenceNanoSerialOverTcp();
				((BinaryCommunication)keyenceNanoSerialOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IsPersistentConnection
				};
				keyenceNanoSerialOverTcp.UseStation = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.UseStation;
				keyenceNanoSerialOverTcp.Station = (byte)((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.Station;
				oResult = await ((DeviceTcpNet)keyenceNanoSerialOverTcp).ConnectServerAsync();
				plcBase = keyenceNanoSerialOverTcp;
				break;
			}
			case KeyenceData.ProtocolType.KeyenceKvOld:
			{
				KeyenceKvOld keyenceKvOld = new KeyenceKvOld();
				((BinaryCommunication)keyenceKvOld).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ReceiveTimeOut
				};
				oResult = await ((DeviceTcpNet)keyenceKvOld).ConnectServerAsync();
				plcBase = keyenceKvOld;
				break;
			}
			case KeyenceData.ProtocolType.KeyenceMcNet:
			{
				KeyenceMcNet keyenceMcNet = new KeyenceMcNet();
				((BinaryCommunication)keyenceMcNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((MelsecMcNet)keyenceMcNet).EnableWriteBitToWordRegister = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				((DeviceCommunication)keyenceMcNet).ByteTransform.IsStringReverseByteWord = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = await ((DeviceTcpNet)keyenceMcNet).ConnectServerAsync();
				plcBase = keyenceMcNet;
				break;
			}
			case KeyenceData.ProtocolType.KeyenceNanoSerial:
			{
				KeyenceNanoSerial val = new KeyenceNanoSerial();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SleepTime
				};
				val.UseStation = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.UseStation;
				val.Station = (byte)((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.Station;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
			{
				KeyenceMcAsciiNet keyenceMcAsciiNet = new KeyenceMcAsciiNet();
				((BinaryCommunication)keyenceMcAsciiNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((MelsecMcNet)keyenceMcAsciiNet).EnableWriteBitToWordRegister = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				((DeviceCommunication)keyenceMcAsciiNet).ByteTransform.IsStringReverseByteWord = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = await ((DeviceTcpNet)keyenceMcAsciiNet).ConnectServerAsync();
				plcBase = keyenceMcAsciiNet;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs");
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OnAsync", 154);
			}
			await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OnAsync", 158);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OnAsync", 163);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OffAsync", 177);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OffAsync", 186);
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
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).Dispose();
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						await ((DeviceTcpNet)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).Dispose();
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).Dispose();
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						((DeviceSerialPort)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).Dispose();
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).Dispose();
						break;
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
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OffAsync", 228);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "OffAsync", 232);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "ReadAsync", 244);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "ReadAsync", 249);
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
							OResult<byte> val9 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val9 = null;
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val9 = null;
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val9 = null;
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val9 = null;
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val9 = null;
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadBoolAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadBoolAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadBoolAsync(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadFloatAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadFloatAsync(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadFloatAsync(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadInt32Async(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadUInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadUInt32Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadUInt32Async(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadInt64Async(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadUInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadUInt64Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadUInt64Async(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadInt16Async(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadUInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadUInt16Async(addressName);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadUInt16Async(addressName);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadAsync(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
							{
							case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceKvOld:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceNanoSerial:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "ReadAsync", 1134);
			}
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "ReadAsync", 1138);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "ReadAsync", 1143);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "WriteAsync", 1155);
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
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.ProtocolType)
					{
					case KeyenceData.ProtocolType.KeyenceNanoSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case KeyenceData.ProtocolType.KeyenceKvOld:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceKvOld>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case KeyenceData.ProtocolType.KeyenceMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case KeyenceData.ProtocolType.KeyenceNanoSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceNanoSerial>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case KeyenceData.ProtocolType.KeyenceMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<KeyenceMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					}
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
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "WriteAsync", 1639);
			}
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "WriteAsync", 1642);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "WriteAsync", 1646);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "SubscribeAsync", 1658);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "SubscribeAsync", 1663);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "SubscribeAsync", 1671);
			}
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "SubscribeAsync", 1675);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "SubscribeAsync", 1680);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "UnSubscribeAsync", 1692);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "UnSubscribeAsync", 1697);
			}
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "UnSubscribeAsync", 1701);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "UnSubscribeAsync", 1706);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "GetStatusAsync", 1720);
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
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "GetStatusAsync", 1727);
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
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "GetStatusAsync", 1734);
		}
		return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "GetStatusAsync", 1737);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<KeyenceOperate, KeyenceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "GetBaseObjectAsync", 1746);
		}
		return await ((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Keyence\\KeyenceOperate.cs", "GetBaseObjectAsync", 1749);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<KeyenceOperate, KeyenceData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
