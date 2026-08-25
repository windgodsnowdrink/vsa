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
using Snet.Driver.Profinet.Siemens;
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
[assembly: AssemblyDescription("采集协议：Siemens（西门子 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.Siemens")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Siemens;

/// <summary>
/// 西门子数据
/// </summary>
public class SiemensData
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
		/// PDU的长度信息
		/// </summary>
		[Description("PDU的长度信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int PDULength { get; set; } = -1;

		/// <summary>
		/// 本地TSAP参数信息
		/// </summary>
		[Description("本地TSAP参数信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int LocalTSAP { get; set; } = 19799;

		/// <summary>
		/// PLC的机架号
		/// </summary>
		[Description("PLC的机架号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Rack { get; set; }

		/// <summary>
		/// 插槽
		/// </summary>
		[Description("插槽")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Slot { get; set; }

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
		public ProtocolType ProtocolType { get; set; } = ProtocolType.SiemensPPIOverTcp;
	}

	/// <summary>
	/// 协议类型
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ProtocolType
	{
		/// <summary>
		/// 西门子PLC S7_S200协议
		/// </summary>
		[Description("西门子PLC S7_S200协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "PDULength", "LocalTSAP", "LocalTSAP"
		})]
		SiemensS7Net_S200,
		/// <summary>
		/// 西门子PLC S7_S200Smart协议
		/// </summary>
		[Description("西门子PLC S7_S200Smart协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "PDULength"
		})]
		SiemensS7Net_S200Smart,
		/// <summary>
		/// 西门子PLC S7_S300协议
		/// </summary>
		[Description("西门子PLC S7_S300协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "PDULength", "Rack", "Slot"
		})]
		SiemensS7Net_S300,
		/// <summary>
		/// 西门子PLC S7_S400协议
		/// </summary>
		[Description("西门子PLC S7_S400协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "PDULength", "Rack", "Slot"
		})]
		SiemensS7Net_S400,
		/// <summary>
		/// 西门子PLC S7_S1200协议
		/// </summary>
		[Description("西门子PLC S7_S1200协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "PDULength", "Rack", "Slot"
		})]
		SiemensS7Net_S1200,
		/// <summary>
		/// 西门子PLC S7_S1500协议
		/// </summary>
		[Description("西门子PLC S7_S1500协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "PDULength", "Rack", "Slot"
		})]
		SiemensS7Net_S1500,
		/// <summary>
		/// 西门子PLC 使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置
		/// </summary>
		[Description("西门子PLC 使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		SiemensFetchWriteNet,
		/// <summary>
		/// 西门子PLC PPI协议，适用于s7-200plc，地址支持携带站号，例如：s=2;M100
		/// </summary>
		[Description("西门子PLC PPI协议，适用于s7-200plc，地址支持携带站号，例如：s=2;M100")]
		[AutoAllocating(new string[] { "SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "Station" })]
		SiemensPPI,
		/// <summary>
		/// 西门子PLC PPI协议网络透传，适用于s7-200plc，地址支持携带站号，例如：s=2;M100
		/// </summary>
		[Description("西门子PLC PPI协议网络透传，适用于s7-200plc，地址支持携带站号，例如：s=2;M100")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Station"
		})]
		SiemensPPIOverTcp,
		/// <summary>
		/// 西门子PLC PLUS网络协议
		/// </summary>
		[Description("西门子PLC PLUS网络协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		SiemensS7Plus
	}
}
/// <summary>
/// 西门子通信库
/// </summary>
public class SiemensOperate : DaqAbstract<SiemensOperate, SiemensData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "西门子";

	/// <inheritdoc />
	protected override string CD => "支持对“西门子 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public SiemensOperate()
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
	public SiemensOperate(SiemensData.Basics basics)
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
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
			{
			case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
			{
				SiemensS7Net siemensS7Net = new SiemensS7Net((SiemensPLCS)5);
				((BinaryCommunication)siemensS7Net).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				siemensS7Net.PDULength = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.PDULength;
				oResult = await ((DeviceTcpNet)siemensS7Net).ConnectServerAsync();
				plcBase = siemensS7Net;
				break;
			}
			case SiemensData.ProtocolType.SiemensFetchWriteNet:
			{
				SiemensFetchWriteNet siemensFetchWriteNet = new SiemensFetchWriteNet();
				((BinaryCommunication)siemensFetchWriteNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)siemensFetchWriteNet).ConnectServerAsync();
				plcBase = siemensFetchWriteNet;
				break;
			}
			case SiemensData.ProtocolType.SiemensS7Net_S1500:
			{
				SiemensS7Net siemensS7Net = new SiemensS7Net((SiemensPLCS)4);
				((BinaryCommunication)siemensS7Net).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				siemensS7Net.PDULength = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.PDULength;
				siemensS7Net.Rack = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Rack;
				siemensS7Net.Slot = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Slot;
				oResult = await ((DeviceTcpNet)siemensS7Net).ConnectServerAsync();
				plcBase = siemensS7Net;
				break;
			}
			case SiemensData.ProtocolType.SiemensS7Plus:
			{
				SiemensS7Plus siemensS7Plus = new SiemensS7Plus();
				((BinaryCommunication)siemensS7Plus).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)siemensS7Plus).ConnectServerAsync();
				plcBase = siemensS7Plus;
				break;
			}
			case SiemensData.ProtocolType.SiemensS7Net_S1200:
			{
				SiemensS7Net siemensS7Net = new SiemensS7Net((SiemensPLCS)1);
				((BinaryCommunication)siemensS7Net).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				siemensS7Net.PDULength = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.PDULength;
				siemensS7Net.Rack = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Rack;
				siemensS7Net.Slot = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Slot;
				oResult = await ((DeviceTcpNet)siemensS7Net).ConnectServerAsync();
				plcBase = siemensS7Net;
				break;
			}
			case SiemensData.ProtocolType.SiemensPPIOverTcp:
			{
				SiemensPPIOverTcp siemensPPIOverTcp = new SiemensPPIOverTcp();
				((BinaryCommunication)siemensPPIOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				siemensPPIOverTcp.Station = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Station;
				oResult = await ((DeviceTcpNet)siemensPPIOverTcp).ConnectServerAsync();
				plcBase = siemensPPIOverTcp;
				break;
			}
			case SiemensData.ProtocolType.SiemensPPI:
			{
				SiemensPPI val = new SiemensPPI();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime
				};
				val.Station = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Station;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case SiemensData.ProtocolType.SiemensS7Net_S200:
			{
				SiemensS7Net siemensS7Net = new SiemensS7Net((SiemensPLCS)6);
				((BinaryCommunication)siemensS7Net).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				siemensS7Net.PDULength = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.PDULength;
				siemensS7Net.LocalTSAP = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.LocalTSAP;
				siemensS7Net.DestTSAP = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.LocalTSAP;
				oResult = await ((DeviceTcpNet)siemensS7Net).ConnectServerAsync();
				plcBase = siemensS7Net;
				break;
			}
			case SiemensData.ProtocolType.SiemensS7Net_S300:
			{
				SiemensS7Net siemensS7Net = new SiemensS7Net((SiemensPLCS)2);
				((BinaryCommunication)siemensS7Net).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				siemensS7Net.PDULength = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.PDULength;
				siemensS7Net.Rack = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Rack;
				siemensS7Net.Slot = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Slot;
				oResult = await ((DeviceTcpNet)siemensS7Net).ConnectServerAsync();
				plcBase = siemensS7Net;
				break;
			}
			case SiemensData.ProtocolType.SiemensS7Net_S400:
			{
				SiemensS7Net siemensS7Net = new SiemensS7Net((SiemensPLCS)3);
				((BinaryCommunication)siemensS7Net).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.IsPersistentConnection
				};
				siemensS7Net.PDULength = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.PDULength;
				siemensS7Net.Rack = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Rack;
				siemensS7Net.Slot = (byte)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.Slot;
				oResult = await ((DeviceTcpNet)siemensS7Net).ConnectServerAsync();
				plcBase = siemensS7Net;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs");
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OnAsync", 234);
			}
			await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OnAsync", 238);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OnAsync", 243);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OffAsync", 257);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OffAsync", 266);
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
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						await ((DeviceTcpNet)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).Dispose();
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).Dispose();
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						await ((DeviceTcpNet)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).Dispose();
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).Dispose();
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						((DeviceSerialPort)ExtendMethod.GetSource<SiemensPPI>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).Dispose();
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
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OffAsync", 313);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "OffAsync", 317);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "ReadAsync", 329);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "ReadAsync", 334);
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
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val9 = await ExtendMethod.GetSource<SiemensS7Net>(plcBase).ReadByteAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val9 = await ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase).ReadByteAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val9 = await ExtendMethod.GetSource<SiemensS7Plus>(plcBase).ReadByteAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val9 = await ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase).ReadByteAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadBoolAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadBoolAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadFloatAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadFloatAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadUInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadUInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadUInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadUInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadUInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadUInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
							{
							case SiemensData.ProtocolType.SiemensS7Net_S200:
							case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
							case SiemensData.ProtocolType.SiemensS7Net_S300:
							case SiemensData.ProtocolType.SiemensS7Net_S400:
							case SiemensData.ProtocolType.SiemensS7Net_S1200:
							case SiemensData.ProtocolType.SiemensS7Net_S1500:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensFetchWriteNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensS7Plus:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPIOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case SiemensData.ProtocolType.SiemensPPI:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "ReadAsync", 1324);
			}
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "ReadAsync", 1328);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "ReadAsync", 1333);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "WriteAsync", 1345);
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
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ExtendMethod.GetSource<SiemensS7Net>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ExtendMethod.GetSource<SiemensS7Plus>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.ProtocolType)
					{
					case SiemensData.ProtocolType.SiemensS7Net_S200:
					case SiemensData.ProtocolType.SiemensS7Net_S200Smart:
					case SiemensData.ProtocolType.SiemensS7Net_S300:
					case SiemensData.ProtocolType.SiemensS7Net_S400:
					case SiemensData.ProtocolType.SiemensS7Net_S1200:
					case SiemensData.ProtocolType.SiemensS7Net_S1500:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Net>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case SiemensData.ProtocolType.SiemensFetchWriteNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensFetchWriteNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case SiemensData.ProtocolType.SiemensS7Plus:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensS7Plus>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case SiemensData.ProtocolType.SiemensPPIOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPIOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case SiemensData.ProtocolType.SiemensPPI:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<SiemensPPI>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "WriteAsync", 1934);
			}
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "WriteAsync", 1937);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "WriteAsync", 1941);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "SubscribeAsync", 1953);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "SubscribeAsync", 1958);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "SubscribeAsync", 1966);
			}
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "SubscribeAsync", 1970);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "SubscribeAsync", 1975);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "UnSubscribeAsync", 1987);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "UnSubscribeAsync", 1992);
			}
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "UnSubscribeAsync", 1996);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "UnSubscribeAsync", 2001);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "GetStatusAsync", 2015);
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
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "GetStatusAsync", 2022);
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
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "GetStatusAsync", 2029);
		}
		return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "GetStatusAsync", 2032);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<SiemensOperate, SiemensData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "GetBaseObjectAsync", 2041);
		}
		return await ((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Siemens\\SiemensOperate.cs", "GetBaseObjectAsync", 2044);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<SiemensOperate, SiemensData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
