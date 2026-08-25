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
using Snet.Driver.Core;
using Snet.Driver.Core.Device;
using Snet.Driver.Core.Net;
using Snet.Driver.Core.Pipe;
using Snet.Driver.LogNet;
using Snet.Driver.ModBus;
using Snet.Driver.Profinet.Inovance;
using Snet.Driver.Profinet.Melsec;
using Snet.Driver.Profinet.Omron;
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
[assembly: AssemblyDescription("采集协议：Inovance（汇川 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.Inovance")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Inovance;

/// <summary>
/// 汇川数据
/// </summary>
public class InovanceData
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class Basics : SCData
	{
		[CompilerGenerated]
		private InovanceSeries <InovanceSeries>k__BackingField;

		[CompilerGenerated]
		private DataFormat <DataFormat>k__BackingField;

		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Category("基础数据")]
		[Description("唯一标识符")]
		public string SN { get; set; }

		/// <summary>
		/// ip地址
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
		/// 连接超时时间
		/// </summary>
		[Description(" 连接超时时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int ConnectTimeOut { get; set; }

		/// <summary>
		/// 接收超时时间
		/// </summary>
		[Description(" 接收超时时间")]
		[Unit("ms")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int ReceiveTimeOut { get; set; }

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
		public int SocketKeepAliveTime { get; set; }

		/// <summary>
		/// 管道是否是长连接
		/// </summary>
		[Description("管道是否是长连接")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsPersistentConnection { get; set; }

		/// <summary>
		/// 站号信息
		/// </summary>
		[Description("站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Station { get; set; }

		/// <summary>
		/// 起始地址是否从0开始
		/// </summary>
		[Description("起始地址是否从0开始")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool AddressStartWithZero { get; set; }

		/// <summary>
		/// 字节反转
		/// </summary>
		[Description("字节反转")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsStringReverse { get; set; }

		/// <summary>
		/// 型号系列
		/// </summary>
		[Description("型号 系列")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public InovanceSeries InovanceSeries
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <InovanceSeries>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<InovanceSeries>k__BackingField = value;
			}
		}

		/// <summary>
		/// 解析格式
		/// </summary>
		[Description("解析格式")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public DataFormat DataFormat
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <DataFormat>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<DataFormat>k__BackingField = value;
			}
		}

		/// <summary>
		/// 串口信息
		/// </summary>
		[Description("串口 信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string? SerialPortInfo { get; set; }

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
		public bool SumCheck { get; set; }

		/// <summary>
		/// 协议格式信息[1,2,3,4]
		/// </summary>
		[Description("协议格式信息[1,2,3,4]")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Format { get; set; }

		/// <summary>
		/// T-O网络连接ID
		/// </summary>
		[Description("T-O网络连接ID")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int TOConnectionId { get; set; }

		/// <summary>
		/// O-T网络连接ID
		/// </summary>
		[Description("O-T网络连接ID")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int OTConnectionId { get; set; }

		/// <summary>
		/// 协议类型
		/// </summary>
		[Description("协议类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[AutoAllocatingTag(typeof(ProtocolType))]
		[Display(/*Could not decode attribute arguments.*/)]
		public ProtocolType ProtocolType { get; set; }

		public Basics()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			IpAddress = "127.0.0.1";
			Port = 6688;
			ConnectTimeOut = 1000;
			ReceiveTimeOut = 1000;
			SocketKeepAliveTime = -1;
			IsPersistentConnection = true;
			Station = 1;
			AddressStartWithZero = true;
			DataFormat = (DataFormat)2;
			SerialPortInfo = "COM3-9600-8-N-1";
			SumCheck = true;
			Format = 1;
			((SCData)this)..ctor();
		}
	}

	/// <summary>
	/// 协议类型
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ProtocolType
	{
		/// <summary>
		/// 汇川PLC 网口通信协议，A适用于AM400、 AM400_800、 AC800、H3U, XP, H5U 等系列底层走的是MODBUS-TCP协议
		/// </summary>
		[Description("汇川PLC 网口通信协议，A适用于AM400、 AM400_800、 AC800、H3U, XP, H5U 等系列底层走的是MODBUS-TCP协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Station", "AddressStartWithZero", "IsStringReverse", "InovanceSeries", "DataFormat"
		})]
		InovanceTcpNet,
		/// <summary>
		/// 汇川PLC 串口通信协议，A适用于AM400、 AM400_800、 AC800、H3U, XP, H5U 等系列底层走的是MODBUS-RTU协议
		/// </summary>
		[Description("汇川PLC 串口通信协议，A适用于AM400、 AM400_800、 AC800、H3U, XP, H5U 等系列底层走的是MODBUS-RTU协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "Station",
			"AddressStartWithZero", "IsStringReverse", "InovanceSeries", "DataFormat"
		})]
		InovanceSerial,
		/// <summary>
		/// 汇川PLC 串口转网口通信协议，A适用于AM400、 AM400_800、 AC800、H3U, XP, H5U 等系列底层走的是MODBUS-RTU协议
		/// </summary>
		[Description("汇川PLC 串口转网口通信协议，A适用于AM400、 AM400_800、 AC800、H3U, XP, H5U 等系列底层走的是MODBUS-RTU协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Station", "AddressStartWithZero", "IsStringReverse", "InovanceSeries", "DataFormat"
		})]
		InovanceSerialOverTcp,
		/// <summary>
		/// 汇川PLC 计算机链接协议，使用PLC得COM1口，设置 D8126=06h, D8120.14=1 才能正常通信
		/// </summary>
		[Description("汇川PLC 计算机链接协议，使用PLC得COM1口，设置 D8126=06h, D8120.14=1 才能正常通信")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "Station",
			"WaittingTime", "SumCheck", "Format"
		})]
		InovanceComputerLink,
		/// <summary>
		/// 汇川PLC CIP协议
		/// </summary>
		[Description("汇川PLC CIP协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "TOConnectionId", "OTConnectionId"
		})]
		InovanceConnectedCipNet,
		/// <summary>
		/// 汇川PLC Easy系列的通信协议，使用的是专用的报文
		/// </summary>
		[Description("汇川PLC Easy系列的通信协议，使用的是专用的报文")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		InovanceEasyNet
	}
}
/// <summary>
/// 汇川通信库
/// </summary>
public class InovanceOperate : DaqAbstract<InovanceOperate, InovanceData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "汇川";

	/// <inheritdoc />
	protected override string CD => "支持对“汇川 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public InovanceOperate()
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
	public InovanceOperate(InovanceData.Basics basics)
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
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
			{
			case InovanceData.ProtocolType.InovanceSerial:
			{
				InovanceSerial val2 = new InovanceSerial();
				((BinaryCommunication)val2).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SleepTime
				};
				((ModbusRtu)val2).Station = (byte)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Station;
				((ModbusRtu)val2).AddressStartWithZero = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.AddressStartWithZero;
				((ModbusRtu)val2).IsStringReverse = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IsStringReverse;
				val2.Series = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.InovanceSeries;
				((ModbusRtu)val2).DataFormat = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.DataFormat;
				oResult = ((DeviceSerialPort)val2).Open();
				plcBase = val2;
				break;
			}
			case InovanceData.ProtocolType.InovanceComputerLink:
			{
				InovanceComputerLink val = new InovanceComputerLink();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SleepTime
				};
				((MelsecFxLinks)val).Station = (byte)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Station;
				((MelsecFxLinks)val).WaittingTime = (byte)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.WaittingTime;
				((MelsecFxLinks)val).SumCheck = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SumCheck;
				((MelsecFxLinks)val).Format = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Format;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case InovanceData.ProtocolType.InovanceConnectedCipNet:
			{
				InovanceConnectedCipNet inovanceConnectedCipNet = new InovanceConnectedCipNet();
				((BinaryCommunication)inovanceConnectedCipNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((NetworkConnectedCip)inovanceConnectedCipNet).TOConnectionId = (uint)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.TOConnectionId;
				((NetworkConnectedCip)inovanceConnectedCipNet).OTConnectionId = (uint)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.OTConnectionId;
				oResult = await ((DeviceTcpNet)inovanceConnectedCipNet).ConnectServerAsync();
				plcBase = inovanceConnectedCipNet;
				break;
			}
			case InovanceData.ProtocolType.InovanceTcpNet:
			{
				InovanceTcpNet inovanceTcpNet = new InovanceTcpNet();
				((BinaryCommunication)inovanceTcpNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((ModbusTcpNet)inovanceTcpNet).Station = (byte)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Station;
				((ModbusTcpNet)inovanceTcpNet).AddressStartWithZero = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.AddressStartWithZero;
				((ModbusTcpNet)inovanceTcpNet).IsStringReverse = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IsStringReverse;
				inovanceTcpNet.Series = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.InovanceSeries;
				((ModbusTcpNet)inovanceTcpNet).DataFormat = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.DataFormat;
				oResult = await ((DeviceTcpNet)inovanceTcpNet).ConnectServerAsync();
				plcBase = inovanceTcpNet;
				break;
			}
			case InovanceData.ProtocolType.InovanceSerialOverTcp:
			{
				InovanceSerialOverTcp inovanceSerialOverTcp = new InovanceSerialOverTcp();
				((BinaryCommunication)inovanceSerialOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((ModbusRtuOverTcp)inovanceSerialOverTcp).Station = (byte)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Station;
				((ModbusRtuOverTcp)inovanceSerialOverTcp).AddressStartWithZero = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.AddressStartWithZero;
				((ModbusRtuOverTcp)inovanceSerialOverTcp).IsStringReverse = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IsStringReverse;
				inovanceSerialOverTcp.Series = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.InovanceSeries;
				((ModbusRtuOverTcp)inovanceSerialOverTcp).DataFormat = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.DataFormat;
				oResult = await ((DeviceTcpNet)inovanceSerialOverTcp).ConnectServerAsync();
				plcBase = inovanceSerialOverTcp;
				break;
			}
			case InovanceData.ProtocolType.InovanceEasyNet:
			{
				InovanceEasyNet inovanceEasyNet = new InovanceEasyNet();
				((BinaryCommunication)inovanceEasyNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)inovanceEasyNet).ConnectServerAsync();
				plcBase = inovanceEasyNet;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs");
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OnAsync", 181);
			}
			await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OnAsync", 185);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OnAsync", 190);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OffAsync", 204);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OffAsync", 213);
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
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						((DeviceSerialPort)ExtendMethod.GetSource<InovanceSerial>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).Dispose();
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						((DeviceSerialPort)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).Dispose();
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).Dispose();
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).Dispose();
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).Dispose();
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).Dispose();
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
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OffAsync", 259);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "OffAsync", 263);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "ReadAsync", 275);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "ReadAsync", 280);
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
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val9 = await ExtendMethod.GetSource<InovanceSerial>(plcBase).ReadByteAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val9 = null;
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val9 = await ((OmronConnectedCipNet)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadByteAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val9 = await ExtendMethod.GetSource<InovanceTcpNet>(plcBase).ReadByteAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val9 = await ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase).ReadByteAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadBoolAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadBoolAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadFloatAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadFloatAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadUInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadUInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadUInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadUInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadUInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadUInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
							{
							case InovanceData.ProtocolType.InovanceSerial:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceComputerLink:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceConnectedCipNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceTcpNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceSerialOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case InovanceData.ProtocolType.InovanceEasyNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "ReadAsync", 1228);
			}
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "ReadAsync", 1232);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "ReadAsync", 1237);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "WriteAsync", 1249);
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
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((OmronConnectedCipNet)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.ProtocolType)
					{
					case InovanceData.ProtocolType.InovanceSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerial>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case InovanceData.ProtocolType.InovanceComputerLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceComputerLink>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case InovanceData.ProtocolType.InovanceConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case InovanceData.ProtocolType.InovanceTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceTcpNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case InovanceData.ProtocolType.InovanceSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case InovanceData.ProtocolType.InovanceEasyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<InovanceEasyNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "WriteAsync", 1796);
			}
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "WriteAsync", 1799);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "WriteAsync", 1803);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "SubscribeAsync", 1815);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "SubscribeAsync", 1820);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "SubscribeAsync", 1828);
			}
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "SubscribeAsync", 1832);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "SubscribeAsync", 1837);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "UnSubscribeAsync", 1849);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "UnSubscribeAsync", 1854);
			}
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "UnSubscribeAsync", 1858);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "UnSubscribeAsync", 1863);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "GetStatusAsync", 1877);
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
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "GetStatusAsync", 1884);
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
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "GetStatusAsync", 1891);
		}
		return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "GetStatusAsync", 1894);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<InovanceOperate, InovanceData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "GetBaseObjectAsync", 1903);
		}
		return await ((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Inovance\\InovanceOperate.cs", "GetBaseObjectAsync", 1906);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<InovanceOperate, InovanceData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
