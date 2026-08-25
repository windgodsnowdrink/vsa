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
[assembly: AssemblyDescription("采集协议：AllenBradley（罗克韦尔 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.AllenBradley")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.AllenBradley;

/// <summary>
/// 罗克韦尔数据
/// </summary>
public class AllenBradleyData
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
		/// 使用52片段功能码
		/// </summary>
		[Description("使用52片段功能码")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool ReadArrayUseSegment { get; } = true;

		/// <summary>
		/// 插槽
		/// </summary>
		[Description("插槽")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Slot { get; set; }

		/// <summary>
		/// 将自增Senderontext属性当为消息id，接收消息匹配处理
		/// </summary>
		[Description("内容检查")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool ContextCheck { get; set; }

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
		/// 目标节点号
		/// </summary>
		[Description("目标节点号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int DstNode { get; set; } = 2;

		/// <summary>
		/// 源节点号
		/// </summary>
		[Description("源节点号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int SrcNode { get; set; } = 1;

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
		/// 罗克韦尔PLC 数据通信类，使用CIP协议实现，适用1756，1769等型号，支持使用标签的形式进行读写操作，支持标量数据，一维数组，二维数组，三维数组等等。如果是局部变量，那么使用 Program:MainProgram.[变量名]
		/// </summary>
		[Description("罗克韦尔PLC 数据通信类，使用CIP协议实现，适用1756，1769等型号，支持使用标签的形式进行读写操作，支持标量数据，一维数组，二维数组，三维数组等等。如果是局部变量，那么使用 Program:MainProgram.[变量名]")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "ReadArrayUseSegment", "Slot", "ContextCheck"
		})]
		AllenBradleyNet,
		/// <summary>
		/// 罗克韦尔PLC 基于连接的对象访问的CIP协议的实现，用于对罗克韦尔 PLC进行标签的数据读写，对数组，多维数组进行读写操作
		/// </summary>
		[Description("罗克韦尔PLC 基于连接的对象访问的CIP协议的实现，用于对罗克韦尔 PLC进行标签的数据读写，对数组，多维数组进行读写操作")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		AllenBradleyConnectedCipNet,
		/// <summary>
		/// 罗克韦尔PLC cip通信实现类，适用Micro800系列控制系统
		/// </summary>
		[Description("罗克韦尔PLC cip通信实现类，适用Micro800系列控制系统")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Slot"
		})]
		AllenBradleyMicroCip,
		/// <summary>
		/// 罗克韦尔PLC 在CIP协议里，使用PCCC命令进行访问设备的原始数据文件的通信方法
		/// </summary>
		[Description("罗克韦尔PLC 在CIP协议里，使用PCCC命令进行访问设备的原始数据文件的通信方法")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		AllenBradleyPcccNet,
		/// <summary>
		/// 罗克韦尔PLC 针对SLC系列的通信的实现
		/// </summary>
		[Description("罗克韦尔PLC 针对SLC系列的通信的实现")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection"
		})]
		AllenBradleySLCNet,
		/// <summary>
		/// 罗克韦尔PLC DF1通信协议，基于串口实现，通信机制为半双工，目前适用于 Micro-Logix1000,SLC500,SLC 5/03,SLC 5/04，地址示例：N7:1
		/// </summary>
		[Description("罗克韦尔PLC DF1通信协议，基于串口实现，通信机制为半双工，目前适用于 Micro-Logix1000,SLC500,SLC 5/03,SLC 5/04，地址示例：N7:1")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "Station",
			"DstNode", "SrcNode"
		})]
		AllenBradleyDF1Serial
	}
}
/// <summary>
/// 罗克韦尔通信库
/// </summary>
public class AllenBradleyOperate : DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "罗克韦尔";

	/// <inheritdoc />
	protected override string CD => "支持对“罗克韦尔 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public AllenBradleyOperate()
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
	public AllenBradleyOperate(AllenBradleyData.Basics basics)
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
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
			{
			case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
			{
				AllenBradleyConnectedCipNet allenBradleyConnectedCipNet = new AllenBradleyConnectedCipNet();
				((BinaryCommunication)allenBradleyConnectedCipNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)allenBradleyConnectedCipNet).ConnectServerAsync();
				plcBase = allenBradleyConnectedCipNet;
				break;
			}
			case AllenBradleyData.ProtocolType.AllenBradleyNet:
			{
				AllenBradleyNet allenBradleyNet = new AllenBradleyNet();
				((BinaryCommunication)allenBradleyNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IsPersistentConnection
				};
				allenBradleyNet.ReadArrayUseSegment = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ReadArrayUseSegment;
				allenBradleyNet.Slot = (byte)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Slot;
				allenBradleyNet.ContextCheck = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ContextCheck;
				oResult = await ((DeviceTcpNet)allenBradleyNet).ConnectServerAsync();
				plcBase = allenBradleyNet;
				break;
			}
			case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
			{
				AllenBradleySLCNet allenBradleySLCNet = new AllenBradleySLCNet();
				((BinaryCommunication)allenBradleySLCNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)allenBradleySLCNet).ConnectServerAsync();
				plcBase = allenBradleySLCNet;
				break;
			}
			case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
			{
				AllenBradleyMicroCip allenBradleyMicroCip = new AllenBradleyMicroCip();
				((BinaryCommunication)allenBradleyMicroCip).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((AllenBradleyNet)allenBradleyMicroCip).Slot = (byte)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Slot;
				oResult = await ((DeviceTcpNet)allenBradleyMicroCip).ConnectServerAsync();
				plcBase = allenBradleyMicroCip;
				break;
			}
			case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
			{
				AllenBradleyDF1Serial val = new AllenBradleyDF1Serial();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SleepTime
				};
				val.Station = (byte)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Station;
				val.DstNode = (byte)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.DstNode;
				val.SrcNode = (byte)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SrcNode;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
			{
				AllenBradleyPcccNet allenBradleyPcccNet = new AllenBradleyPcccNet();
				((BinaryCommunication)allenBradleyPcccNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.IsPersistentConnection
				};
				oResult = await ((DeviceTcpNet)allenBradleyPcccNet).ConnectServerAsync();
				plcBase = allenBradleyPcccNet;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs");
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OnAsync", 169);
			}
			await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OnAsync", 173);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OnAsync", 178);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OffAsync", 192);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OffAsync", 201);
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
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).Dispose();
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).Dispose();
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).Dispose();
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						await ((DeviceTcpNet)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).Dispose();
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						((DeviceSerialPort)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).Dispose();
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).Dispose();
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
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OffAsync", 247);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "OffAsync", 251);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "ReadAsync", 263);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "ReadAsync", 268);
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
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val9 = await ((OmronConnectedCipNet)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadByteAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val9 = await ExtendMethod.GetSource<AllenBradleyNet>(plcBase).ReadByteAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val9 = null;
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val9 = await ((AllenBradleyNet)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadByteAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val9 = null;
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val9 = await ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase).ReadByteAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadBoolAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadBoolAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadFloatAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadFloatAsync(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadUInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadUInt32Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadUInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadUInt64Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadUInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadUInt16Async(addressName);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
							{
							case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "ReadAsync", 1216);
			}
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "ReadAsync", 1220);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "ReadAsync", 1225);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "WriteAsync", 1237);
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
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((OmronConnectedCipNet)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ExtendMethod.GetSource<AllenBradleyNet>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((AllenBradleyNet)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.ProtocolType)
					{
					case AllenBradleyData.ProtocolType.AllenBradleyConnectedCipNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyConnectedCipNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case AllenBradleyData.ProtocolType.AllenBradleySLCNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleySLCNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyMicroCip:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyMicroCip>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyDF1Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyDF1Serial>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case AllenBradleyData.ProtocolType.AllenBradleyPcccNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<AllenBradleyPcccNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "WriteAsync", 1784);
			}
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "WriteAsync", 1787);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "WriteAsync", 1791);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "SubscribeAsync", 1803);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "SubscribeAsync", 1808);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "SubscribeAsync", 1816);
			}
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "SubscribeAsync", 1820);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "SubscribeAsync", 1825);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "UnSubscribeAsync", 1837);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "UnSubscribeAsync", 1842);
			}
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "UnSubscribeAsync", 1846);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "UnSubscribeAsync", 1851);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "GetStatusAsync", 1865);
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
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "GetStatusAsync", 1872);
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
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "GetStatusAsync", 1879);
		}
		return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "GetStatusAsync", 1882);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "GetBaseObjectAsync", 1891);
		}
		return await ((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.AllenBradley\\AllenBradleyOperate.cs", "GetBaseObjectAsync", 1894);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<AllenBradleyOperate, AllenBradleyData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
