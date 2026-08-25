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
using Snet.Driver.Profinet.AllenBradley;
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
[assembly: AssemblyDescription("采集协议：Mitsubishi（三菱 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.Mitsubishi")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Mitsubishi;

/// <summary>
/// 三菱数据
/// </summary>
public class MitsubishiData
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
		/// 插槽
		/// </summary>
		[Description("插槽")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Slot { get; set; }

		/// <summary>
		/// 网络号
		/// </summary>
		[Description("网络号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int NetworkNumber { get; set; }

		/// <summary>
		/// 网络站号
		/// </summary>
		[Description("网络站号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int NetworkStationNumber { get; set; }

		/// <summary>
		/// IO编号
		/// </summary>
		[Description("IO编号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int TargetIOStation { get; set; } = 1023;

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
		/// 前的编程口协议是否为新版
		/// </summary>
		[Description("前的编程口协议是否为新版")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsNewVersion { get; set; } = true;

		/// <summary>
		/// 否动态修改PLC的波特率
		/// </summary>
		[Description("否动态修改PLC的波特率")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool AutoChangeBaudRate { get; set; }

		/// <summary>
		/// 是否使用GOT连接三菱的PLC
		/// </summary>
		[Description("是否使用GOT连接三菱的PLC")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool UseGOT { get; set; }

		/// <summary>
		/// 站号信息
		/// </summary>
		[Description("站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Station { get; set; } = 1;

		/// <summary>
		/// 报文等待时间，范围(0-15)
		/// </summary>
		[Description("报文等待时间，范围(0-15)")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int WaittingTime { get; set; }

		/// <summary>
		/// 使用和校验
		/// </summary>
		[Description("使用和校 验")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool SumCheck { get; set; } = true;

		/// <summary>
		/// 协议格式信息[1,2,3,4]
		/// </summary>
		[Description("协议格式信息[1,2,3,4]")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Format { get; set; } = 1;

		/// <summary>
		/// 协议类型
		/// </summary>
		[Description("协议类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[AutoAllocatingTag(typeof(ProtocolType))]
		[Display(/*Could not decode attribute arguments.*/)]
		public ProtocolType ProtocolType { get; set; } = ProtocolType.MelsecMcNet;
	}

	/// <summary>
	/// 协议类型
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ProtocolType
	{
		/// <summary>
		/// 三菱PLC EIP协议的实现，当PLC使用了 QJ71EIP71 模块时就需要使用本类来访问
		/// </summary>
		[Description("三菱PLC EIP协议的实现，当PLC使用了 QJ71EIP71 模块时就需要使用本类来访问")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Slot"
		})]
		MelsecCipNet,
		/// <summary>
		/// 三菱PLC 采用A兼容1E帧协议实现，使用二进制码通讯，请根据实际型号来进行选取
		/// </summary>
		[Description("三菱PLC 采用A兼容1E帧协议实现，使用二进制码通讯，请根据实际型号来进行选取")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		MelsecA1ENet,
		/// <summary>
		/// 三菱PLC 采用A兼容1E帧协议实现，使用ASCII码通讯，请根据实际型号来进行选取
		/// </summary>
		[Description("三菱PLC 采用A兼容1E帧协议实现，使用ASCII码通讯，请根据实际型号来进行选取")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		MelsecA1EAsciiNet,
		/// <summary>
		/// 三菱PLC 采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯
		/// </summary>
		[Description("三菱PLC 采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "NetworkNumber", "NetworkStationNumber", "TargetIOStation", "EnableWriteBitToWordRegister", "IsStringReverseByteWord"
		})]
		MelsecMcNet,
		/// <summary>
		/// 三菱PLC 采用UDP的协议实现，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯
		/// </summary>
		[Description("三菱PLC 采用UDP的协议实现，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ReceiveTimeOut", "SleepTime", "SocketKeepAliveTime",
			"IsPersistentConnection", "EnableWriteBitToWordRegister", "IsStringReverseByteWord"
		})]
		MelsecMcUdp,
		/// <summary>
		/// 三菱PLC 采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式
		/// </summary>
		[Description("三菱PLC 采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "EnableWriteBitToWordRegister", "IsStringReverseByteWord"
		})]
		MelsecMcAsciiNet,
		/// <summary>
		/// 三菱PLC通讯类，采用UDP的协议实现，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ascii通讯
		/// </summary>
		[Description("三菱PLC通讯类，采用UDP的协议实现，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ascii通讯")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ReceiveTimeOut", "SleepTime", "SocketKeepAliveTime",
			"IsPersistentConnection", "EnableWriteBitToWordRegister", "IsStringReverseByteWord"
		})]
		MelsecMcAsciiUdp,
		/// <summary>
		/// 三菱PLC R系列的MC协议，支持的地址类型
		/// </summary>
		[Description("三菱PLC R系列的MC协议，支持的地址类型")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "EnableWriteBitToWordRegister"
		})]
		MelsecMcRNet,
		/// <summary>
		/// 三菱PLC 串口通信的对象，适用于读取FX系列的串口数据
		/// </summary>
		[Description("三菱PLC 串口通信的对象，适用于读取FX系列的串口数据")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "IsNewVersion",
			"AutoChangeBaudRate"
		})]
		MelsecFxSerial,
		/// <summary>
		/// 三菱PLC 串口协议的网络版，如果使用的是 FX3U编程口(fx2n) -&gt; GOT1000(RS232)(或是GOT2000)  -&gt; 上位机(以太网) 的方式，那么就需要把 UseGOT 设置为True
		/// </summary>
		[Description("三菱PLC 串口协议的网络版，如果使用的是 FX3U编程口(fx2n) -&gt; GOT1000(RS232)(或是GOT2000)  -&gt; 上位机(以太网) 的方式，那么就需要把 UseGOT 设置为True")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "IsNewVersion", "UseGOT", "SleepTime"
		})]
		MelsecFxSerialOverTcp,
		/// <summary>
		/// 三菱PLC 计算机链接协议，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口
		/// </summary>
		[Description("三菱PLC 计算机链接协议，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "Station",
			"WaittingTime", "SumCheck", "Format"
		})]
		MelsecFxLinks,
		/// <summary>
		/// 三菱PLC 计算机链接协议的网口版本，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口
		/// </summary>
		[Description("三菱PLC 计算机链接协议的网口版本，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Station", "WaittingTime", "SumCheck", "Format"
		})]
		MelsecFxLinksOverTcp,
		/// <summary>
		/// 三菱PLC 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址
		/// </summary>
		[Description("三菱PLC 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "Station",
			"EnableWriteBitToWordRegister", "SumCheck", "Format"
		})]
		MelsecA3CNet,
		/// <summary>
		/// 三菱PLC 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址，本类是基于tcp通讯的实现
		/// </summary>
		[Description("三菱PLC 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址，本类是基于tcp通讯的实现")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Station", "EnableWriteBitToWordRegister", "SumCheck", "Format"
		})]
		MelsecA3CNetOverTcp
	}
}
/// <summary>
/// 三菱通信库
/// </summary>
public class MitsubishiOperate : DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "三菱";

	/// <inheritdoc />
	protected override string CD => "支持对“三菱 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public MitsubishiOperate()
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
	public MitsubishiOperate(MitsubishiData.Basics basics)
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
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
			{
			case MitsubishiData.ProtocolType.MelsecA1ENet:
			{
				MelsecA1ENet melsecA1ENet = new MelsecA1ENet();
				((BinaryCommunication)melsecA1ENet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)melsecA1ENet).ConnectServerAsync();
				plcBase = melsecA1ENet;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
			{
				MelsecA1EAsciiNet melsecA1EAsciiNet = new MelsecA1EAsciiNet();
				((BinaryCommunication)melsecA1EAsciiNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)melsecA1EAsciiNet).ConnectServerAsync();
				plcBase = melsecA1EAsciiNet;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecA3CNet:
			{
				MelsecA3CNet val3 = new MelsecA3CNet();
				((BinaryCommunication)val3).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime
				};
				val3.Station = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Station;
				val3.EnableWriteBitToWordRegister = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				val3.SumCheck = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SumCheck;
				val3.Format = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Format;
				oResult = ((DeviceSerialPort)val3).Open();
				plcBase = val3;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecMcRNet:
			{
				MelsecMcRNet melsecMcRNet = new MelsecMcRNet();
				((BinaryCommunication)melsecMcRNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				melsecMcRNet.EnableWriteBitToWordRegister = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				oResult = await ((DeviceTcpNet)melsecMcRNet).ConnectServerAsync();
				plcBase = melsecMcRNet;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
			{
				MelsecMcAsciiUdp melsecMcAsciiUdp = new MelsecMcAsciiUdp();
				((BinaryCommunication)melsecMcAsciiUdp).CommunicationPipe = (CommunicationPipe)new PipeUdpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((MelsecMcNet)melsecMcAsciiUdp).EnableWriteBitToWordRegister = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				((DeviceCommunication)melsecMcAsciiUdp).ByteTransform.IsStringReverseByteWord = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = await ((DeviceTcpNet)melsecMcAsciiUdp).ConnectServerAsync();
				plcBase = melsecMcAsciiUdp;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
			{
				MelsecMcAsciiNet melsecMcAsciiNet = new MelsecMcAsciiNet();
				((BinaryCommunication)melsecMcAsciiNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((MelsecMcNet)melsecMcAsciiNet).EnableWriteBitToWordRegister = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				((DeviceCommunication)melsecMcAsciiNet).ByteTransform.IsStringReverseByteWord = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = await ((DeviceTcpNet)melsecMcAsciiNet).ConnectServerAsync();
				plcBase = melsecMcAsciiNet;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecMcNet:
			{
				MelsecMcNet melsecMcNet = new MelsecMcNet();
				((BinaryCommunication)melsecMcNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				melsecMcNet.NetworkNumber = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.NetworkNumber;
				melsecMcNet.NetworkStationNumber = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.NetworkStationNumber;
				melsecMcNet.TargetIOStation = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.TargetIOStation;
				melsecMcNet.EnableWriteBitToWordRegister = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				((DeviceCommunication)melsecMcNet).ByteTransform.IsStringReverseByteWord = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = await ((DeviceTcpNet)melsecMcNet).ConnectServerAsync();
				plcBase = melsecMcNet;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
			{
				MelsecA3CNetOverTcp melsecA3CNetOverTcp = new MelsecA3CNetOverTcp();
				((BinaryCommunication)melsecA3CNetOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				melsecA3CNetOverTcp.Station = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Station;
				melsecA3CNetOverTcp.EnableWriteBitToWordRegister = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				melsecA3CNetOverTcp.SumCheck = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SumCheck;
				melsecA3CNetOverTcp.Format = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Format;
				oResult = await ((DeviceTcpNet)melsecA3CNetOverTcp).ConnectServerAsync();
				plcBase = melsecA3CNetOverTcp;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecMcUdp:
			{
				MelsecMcUdp melsecMcUdp = new MelsecMcUdp();
				((BinaryCommunication)melsecMcUdp).CommunicationPipe = (CommunicationPipe)new PipeUdpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((MelsecMcNet)melsecMcUdp).EnableWriteBitToWordRegister = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.EnableWriteBitToWordRegister;
				((DeviceCommunication)melsecMcUdp).ByteTransform.IsStringReverseByteWord = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = await ((DeviceTcpNet)melsecMcUdp).ConnectServerAsync();
				plcBase = melsecMcUdp;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecCipNet:
			{
				MelsecCipNet melsecCipNet = new MelsecCipNet();
				((BinaryCommunication)melsecCipNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((AllenBradleyNet)melsecCipNet).Slot = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Slot;
				oResult = await ((DeviceTcpNet)melsecCipNet).ConnectServerAsync();
				plcBase = melsecCipNet;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
			{
				MelsecFxLinksOverTcp melsecFxLinksOverTcp = new MelsecFxLinksOverTcp();
				((BinaryCommunication)melsecFxLinksOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				melsecFxLinksOverTcp.Station = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Station;
				melsecFxLinksOverTcp.WaittingTime = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.WaittingTime;
				melsecFxLinksOverTcp.SumCheck = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SumCheck;
				melsecFxLinksOverTcp.Format = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Format;
				oResult = await ((DeviceTcpNet)melsecFxLinksOverTcp).ConnectServerAsync();
				plcBase = melsecFxLinksOverTcp;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecFxSerial:
			{
				MelsecFxSerial val2 = new MelsecFxSerial();
				((BinaryCommunication)val2).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime
				};
				val2.IsNewVersion = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsNewVersion;
				val2.AutoChangeBaudRate = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.AutoChangeBaudRate;
				oResult = ((DeviceSerialPort)val2).Open();
				plcBase = val2;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecFxLinks:
			{
				MelsecFxLinks val = new MelsecFxLinks();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime
				};
				val.Station = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Station;
				val.WaittingTime = (byte)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.WaittingTime;
				val.SumCheck = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SumCheck;
				val.Format = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Format;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
			{
				MelsecFxSerialOverTcp melsecFxSerialOverTcp = new MelsecFxSerialOverTcp();
				((BinaryCommunication)melsecFxSerialOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				melsecFxSerialOverTcp.IsNewVersion = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.IsNewVersion;
				melsecFxSerialOverTcp.UseGOT = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.UseGOT;
				((BinaryCommunication)melsecFxSerialOverTcp).SleepTime = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SleepTime;
				oResult = await ((DeviceTcpNet)melsecFxSerialOverTcp).ConnectServerAsync();
				plcBase = melsecFxSerialOverTcp;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs");
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OnAsync", 295);
			}
			await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OnAsync", 299);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OnAsync", 304);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OffAsync", 318);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OffAsync", 327);
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
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						((DeviceSerialPort)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						((DeviceSerialPort)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						((DeviceSerialPort)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).Dispose();
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).Dispose();
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
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OffAsync", 405);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "OffAsync", 409);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "ReadAsync", 421);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "ReadAsync", 426);
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
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val9 = await ((AllenBradleyNet)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadByteAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val9 = null;
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
							{
							case MitsubishiData.ProtocolType.MelsecA1ENet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcRNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecMcUdp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecCipNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerial:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxLinks:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "ReadAsync", 1878);
			}
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "ReadAsync", 1882);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "ReadAsync", 1887);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "WriteAsync", 1899);
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
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((AllenBradleyNet)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.ProtocolType)
					{
					case MitsubishiData.ProtocolType.MelsecA1ENet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1ENet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecA1EAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA1EAsciiNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecMcRNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcRNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiUdp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecMcAsciiNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcAsciiNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecMcNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecA3CNetOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecA3CNetOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecMcUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecMcUdp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecCipNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinksOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinksOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerial>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecFxLinks:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxLinks>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MitsubishiData.ProtocolType.MelsecFxSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MelsecFxSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "WriteAsync", 2950);
			}
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "WriteAsync", 2953);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "WriteAsync", 2957);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "SubscribeAsync", 2969);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "SubscribeAsync", 2974);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "SubscribeAsync", 2982);
			}
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "SubscribeAsync", 2986);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "SubscribeAsync", 2991);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "UnSubscribeAsync", 3003);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "UnSubscribeAsync", 3008);
			}
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "UnSubscribeAsync", 3012);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "UnSubscribeAsync", 3017);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "GetStatusAsync", 3031);
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
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "GetStatusAsync", 3038);
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
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "GetStatusAsync", 3045);
		}
		return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "GetStatusAsync", 3048);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<MitsubishiOperate, MitsubishiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "GetBaseObjectAsync", 3057);
		}
		return await ((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Mitsubishi\\MitsubishiOperate.cs", "GetBaseObjectAsync", 3060);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<MitsubishiOperate, MitsubishiData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
