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
using Snet.Driver.Profinet.AllenBradley;
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
[assembly: AssemblyDescription("采集协议：Omron（欧姆龙 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.Omron")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Omron;

/// <summary>
/// 欧姆龙数据
/// </summary>
public class OmronData
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class Basics : SCData
	{
		[CompilerGenerated]
		private OmronPlcType <OmronPlcType>k__BackingField;

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
		/// PLC类型
		/// </summary>
		[Description("PLC类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Display(/*Could not decode attribute arguments.*/)]
		public OmronPlcType OmronPlcType
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <OmronPlcType>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<OmronPlcType>k__BackingField = value;
			}
		}

		/// <summary>
		/// 节点地址
		/// </summary>
		[Description("节点地址")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int DA2 { get; set; }

		/// <summary>
		/// 接收数据直到空为止
		/// </summary>
		[Description("接收数据直到空为止")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool ReceiveUntilEmpty { get; set; }

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
		/// 字节按照字单位反转
		/// </summary>
		[Description("字节按照字单位反转")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsStringReverseByteWord { get; set; }

		/// <summary>
		/// 节点地址
		/// </summary>
		[Description("节点地址")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int SA1 { get; set; }

		/// <summary>
		/// 网络信息层
		/// </summary>
		[Description("网络信息层")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int GCT { get; set; }

		/// <summary>
		/// 节点地址
		/// </summary>
		[Description("节点地址")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int DA1 { get; set; }

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
		/// 站号信息
		/// </summary>
		[Description("站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int UnitNumber { get; set; }

		/// <summary>
		/// 服务标识号
		/// </summary>
		[Description("服务标识号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int SID { get; set; }

		/// <summary>
		/// 上位机单元地址
		/// </summary>
		[Description("上位机单元地址")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int SA2 { get; set; }

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
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			IpAddress = "127.0.0.1";
			Port = 6688;
			ConnectTimeOut = 1000;
			ReceiveTimeOut = 1000;
			SocketKeepAliveTime = -1;
			IsPersistentConnection = true;
			OmronPlcType = (OmronPlcType)1;
			DataFormat = (DataFormat)2;
			SA1 = 1;
			GCT = 2;
			SerialPortInfo = "COM3-9600-8-N-1";
			ProtocolType = ProtocolType.OmronCipNet;
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
		/// 欧姆龙PLC 采用Fins-Tcp通信协议
		/// </summary>
		[Description("欧姆龙PLC 采用Fins-Tcp通信协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "OmronPlcType", "DA2", "ReceiveUntilEmpty", "DataFormat", "IsStringReverseByteWord"
		})]
		OmronFinsNet,
		/// <summary>
		/// 欧姆龙PLC Udp协议，地址类型和Fins-TCP一致
		/// </summary>
		[Description("欧姆龙PLC Udp协议，地址类型和Fins-TCP一致")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ReceiveTimeOut", "SleepTime", "SocketKeepAliveTime",
			"IsPersistentConnection", "OmronPlcType", "SA1", "GCT", "DA1", "DataFormat", "IsStringReverseByteWord"
		})]
		OmronFinsUdp,
		/// <summary>
		/// 欧姆龙PLC CIP协议，支持NJ,NX,NY系列PLC，支持tag名的方式读写数据，假设你读取的是局部变量，那么使用 Program:MainProgram.变量名
		/// </summary>
		[Description("欧姆龙PLC CIP协议，支持NJ,NX,NY系列PLC，支持tag名的方式读写数据，假设你读取的是局部变量，那么使用 Program:MainProgram.变量名")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Slot"
		})]
		OmronCipNet,
		/// <summary>
		/// 欧姆龙PLC 基于连接的对象访问的CIP协议，用于对Omron PLC进行标签的数据读写，对数组，多维数组进行读写操作
		/// </summary>
		[Description("欧姆龙PLC 基于连接的对象访问的CIP协议，用于对Omron PLC进行标签的数据读写，对数组，多维数组进行读写操作")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		OmronConnectedCipNet,
		/// <summary>
		/// 欧姆龙PLC HostLink协议，地址支持示例 DM区:D100; CIO区:C100; Work区:W100; Holding区:H100; Auxiliary区: A100
		/// </summary>
		[Description("欧姆龙PLC HostLink协议，地址支持示例 DM区:D100; CIO区:C100; Work区:W100; Holding区:H100; Auxiliary区: A100")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "UnitNumber",
			"SID", "DA2", "SA2", "DataFormat"
		})]
		OmronHostLink,
		/// <summary>
		/// 欧姆龙PLC HostLink协议，基于Tcp实现，地址支持示例 DM区:D100; CIO区:C100; Work区:W100; Holding区:H100; Auxiliary区: A100
		/// </summary>
		[Description("欧姆龙PLC HostLink协议，基于Tcp实现，地址支持示例 DM区:D100; CIO区:C100; Work区:W100; Holding区:H100; Auxiliary区: A100")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "UnitNumber", "SID", "DA2", "SA2", "DataFormat"
		})]
		OmronHostLinkOverTcp,
		/// <summary>
		/// 欧姆龙PLC HostLink的C-Mode实现形式，地址支持携带站号信息，例如：s=2;D100
		/// </summary>
		[Description("欧姆龙PLC HostLink的C-Mode实现形式，地址支持携带站号信息，例如：s=2;D100")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "UnitNumber",
			"DataFormat"
		})]
		OmronHostLinkCMode,
		/// <summary>
		/// 欧姆龙PLC HostLink的C-Mode实现形式，当前的类是通过以太网透传实现。地址支持携带站号信息，例如：s=2;D100
		/// </summary>
		[Description("欧姆龙PLC HostLink的C-Mode实现形式，当前的类是通过以太网透传实现。地址支持携带站号信息，例如：s=2;D100")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "UnitNumber", "DataFormat"
		})]
		OmronHostLinkCModeOverTcp
	}
}
/// <summary>
/// 欧姆龙通信库
/// </summary>
public class OmronOperate : DaqAbstract<OmronOperate, OmronData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "欧姆龙";

	/// <inheritdoc />
	protected override string CD => "支持对“欧姆龙 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public OmronOperate()
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
	public OmronOperate(OmronData.Basics basics)
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
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
			{
			case OmronData.ProtocolType.OmronHostLink:
			{
				OmronHostLink val3 = new OmronHostLink();
				((BinaryCommunication)val3).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime
				};
				val3.UnitNumber = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.UnitNumber;
				val3.SID = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SID;
				val3.DA2 = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DA2;
				val3.SA2 = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SA2;
				((DeviceCommunication)val3).ByteTransform.DataFormat = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DataFormat;
				oResult = ((DeviceSerialPort)val3).Open();
				plcBase = val3;
				break;
			}
			case OmronData.ProtocolType.OmronConnectedCipNet:
			{
				OmronConnectedCipNet omronConnectedCipNet = new OmronConnectedCipNet();
				((BinaryCommunication)omronConnectedCipNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)omronConnectedCipNet).ConnectServerAsync();
				plcBase = omronConnectedCipNet;
				break;
			}
			case OmronData.ProtocolType.OmronCipNet:
			{
				OmronCipNet omronCipNet = new OmronCipNet();
				((BinaryCommunication)omronCipNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((AllenBradleyNet)omronCipNet).Slot = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.Slot;
				oResult = await ((DeviceTcpNet)omronCipNet).ConnectServerAsync();
				plcBase = omronCipNet;
				break;
			}
			case OmronData.ProtocolType.OmronHostLinkCMode:
			{
				OmronHostLinkCMode val2 = new OmronHostLinkCMode();
				((BinaryCommunication)val2).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime
				};
				val2.UnitNumber = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.UnitNumber;
				((DeviceCommunication)val2).ByteTransform.DataFormat = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DataFormat;
				oResult = ((DeviceSerialPort)val2).Open();
				plcBase = val2;
				break;
			}
			case OmronData.ProtocolType.OmronFinsNet:
			{
				OmronFinsNet omronFinsNet = new OmronFinsNet();
				((BinaryCommunication)omronFinsNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsPersistentConnection
				};
				omronFinsNet.PlcType = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.OmronPlcType;
				omronFinsNet.DA2 = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DA2;
				omronFinsNet.ReceiveUntilEmpty = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ReceiveUntilEmpty;
				((DeviceCommunication)omronFinsNet).ByteTransform.DataFormat = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DataFormat;
				((DeviceCommunication)omronFinsNet).ByteTransform.IsStringReverseByteWord = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = await ((DeviceTcpNet)omronFinsNet).ConnectServerAsync();
				plcBase = omronFinsNet;
				break;
			}
			case OmronData.ProtocolType.OmronHostLinkOverTcp:
			{
				OmronHostLinkOverTcp omronHostLinkOverTcp = new OmronHostLinkOverTcp();
				((BinaryCommunication)omronHostLinkOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsPersistentConnection
				};
				omronHostLinkOverTcp.UnitNumber = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.UnitNumber;
				omronHostLinkOverTcp.SID = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SID;
				omronHostLinkOverTcp.DA2 = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DA2;
				omronHostLinkOverTcp.SA2 = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SA2;
				((DeviceCommunication)omronHostLinkOverTcp).ByteTransform.DataFormat = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DataFormat;
				oResult = await ((DeviceTcpNet)omronHostLinkOverTcp).ConnectServerAsync();
				plcBase = omronHostLinkOverTcp;
				break;
			}
			case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
			{
				OmronHostLinkCModeOverTcp omronHostLinkCModeOverTcp = new OmronHostLinkCModeOverTcp();
				((BinaryCommunication)omronHostLinkCModeOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsPersistentConnection
				};
				omronHostLinkCModeOverTcp.UnitNumber = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.UnitNumber;
				((DeviceCommunication)omronHostLinkCModeOverTcp).ByteTransform.DataFormat = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DataFormat;
				oResult = await ((DeviceTcpNet)omronHostLinkCModeOverTcp).ConnectServerAsync();
				plcBase = omronHostLinkCModeOverTcp;
				break;
			}
			case OmronData.ProtocolType.OmronFinsUdp:
			{
				OmronFinsUdp val = new OmronFinsUdp();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeUdpNet(((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.Port)
				{
					ReceiveTimeOut = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsPersistentConnection
				};
				val.PlcType = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.OmronPlcType;
				val.SA1 = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SA1;
				val.GCT = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.GCT;
				val.DA1 = (byte)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DA1;
				((DeviceCommunication)val).ByteTransform.DataFormat = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.DataFormat;
				((DeviceCommunication)val).ByteTransform.IsStringReverseByteWord = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.IsStringReverseByteWord;
				oResult = OResult.CreateSuccessResult();
				plcBase = val;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs");
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OnAsync", 211);
			}
			await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OnAsync", 215);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OnAsync", 220);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OffAsync", 234);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OffAsync", 243);
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
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						((DeviceSerialPort)ExtendMethod.GetSource<OmronHostLink>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).Dispose();
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).Dispose();
						break;
					case OmronData.ProtocolType.OmronCipNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).Dispose();
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						((DeviceSerialPort)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).Dispose();
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).Dispose();
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).Dispose();
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).Dispose();
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).Dispose();
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
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OffAsync", 296);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "OffAsync", 300);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "ReadAsync", 312);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "ReadAsync", 317);
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
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val9 = null;
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val9 = await ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase).ReadByteAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val9 = await ((AllenBradleyNet)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadByteAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val9 = null;
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val9 = null;
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val9 = null;
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val9 = null;
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadBoolAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadBoolAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadFloatAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadFloatAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadUInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadUInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadUInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadUInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadUInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadUInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
							{
							case OmronData.ProtocolType.OmronHostLink:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronConnectedCipNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronCipNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCMode:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case OmronData.ProtocolType.OmronFinsUdp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "ReadAsync", 1391);
			}
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "ReadAsync", 1395);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "ReadAsync", 1400);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "WriteAsync", 1412);
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
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((AllenBradleyNet)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.ProtocolType)
					{
					case OmronData.ProtocolType.OmronHostLink:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLink>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case OmronData.ProtocolType.OmronConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case OmronData.ProtocolType.OmronCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronCipNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case OmronData.ProtocolType.OmronHostLinkCMode:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCMode>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case OmronData.ProtocolType.OmronFinsNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case OmronData.ProtocolType.OmronHostLinkOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case OmronData.ProtocolType.OmronHostLinkCModeOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronHostLinkCModeOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case OmronData.ProtocolType.OmronFinsUdp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<OmronFinsUdp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "WriteAsync", 2085);
			}
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "WriteAsync", 2088);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "WriteAsync", 2092);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "SubscribeAsync", 2104);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "SubscribeAsync", 2109);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "SubscribeAsync", 2117);
			}
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "SubscribeAsync", 2121);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "SubscribeAsync", 2126);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "UnSubscribeAsync", 2138);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "UnSubscribeAsync", 2143);
			}
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "UnSubscribeAsync", 2147);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "UnSubscribeAsync", 2152);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "GetStatusAsync", 2166);
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
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "GetStatusAsync", 2173);
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
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "GetStatusAsync", 2180);
		}
		return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "GetStatusAsync", 2183);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<OmronOperate, OmronData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "GetBaseObjectAsync", 2192);
		}
		return await ((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Omron\\OmronOperate.cs", "GetBaseObjectAsync", 2195);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<OmronOperate, OmronData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
