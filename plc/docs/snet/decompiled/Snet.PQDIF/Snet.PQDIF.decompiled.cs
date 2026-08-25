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
using Snet.Driver.Instrument.CJT;
using Snet.Driver.Instrument.DLT;
using Snet.Driver.Instrument.Delixi;
using Snet.Driver.LogNet;
using Snet.Driver.ModBus;
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
[assembly: AssemblyDescription("采集协议：PQDIF（电力通讯规约 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.PQDIF")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.PQDIF;

/// <summary>
/// 电力通讯规约数据
/// </summary>
public class PQDIFData
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class Basics : SCData
	{
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
		public string StationStr { get; set; }

		/// <summary>
		/// 增加FE命令头
		/// </summary>
		[Description("增 加FE命令头")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool EnableCodeFE { get; set; }

		/// <summary>
		/// 使用安全的请求模式
		/// </summary>
		[Description("使用安全的请求模式")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool UseSecurityResquest { get; set; }

		/// <summary>
		/// 客户机地址
		/// </summary>
		[Description("客户机地址")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int CA { get; set; }

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
		/// 服务器主动推送功能
		/// </summary>
		[Description("服务器主动推送功能")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsServerActivePush { get; set; }

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
		/// 站号信息
		/// </summary>
		[Description("站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Station { get; set; }

		/// <summary>
		/// CRC16校验检查
		/// </summary>
		[Description("CRC16校验检查")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool Crc16CheckEnable { get; set; }

		/// <summary>
		/// 清空缓冲数据
		/// </summary>
		[Description("清空缓冲数据")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool IsClearCacheBeforeRead { get; set; }

		/// <summary>
		/// 站号检查
		/// </summary>
		[Description("站号检查")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool StationCheckMatch { get; set; }

		/// <summary>
		/// 检查读取返回报文中的数据ID信息
		/// </summary>
		[Description("检查数据ID")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool CheckDataId { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		[Description("密码")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string Password { get; set; }

		/// <summary>
		/// 操作者代码
		/// </summary>
		[Description("操作者代码")]
		[Display(/*Could not decode attribute arguments.*/)]
		public string OpCode { get; set; }

		/// <summary>
		/// 仪表类型
		/// </summary>
		[Description("仪表 类型")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int InstrumentType { get; set; }

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
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			IpAddress = "127.0.0.1";
			Port = 6688;
			ConnectTimeOut = 1000;
			ReceiveTimeOut = 1000;
			SocketKeepAliveTime = -1;
			IsPersistentConnection = true;
			StationStr = "1";
			UseSecurityResquest = true;
			SerialPortInfo = "COM3-9600-8-N-1";
			AddressStartWithZero = true;
			DataFormat = (DataFormat)2;
			Station = 1;
			Crc16CheckEnable = true;
			StationCheckMatch = true;
			CheckDataId = true;
			Password = "00000000";
			OpCode = "00000000";
			InstrumentType = 25;
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
		/// 698.45协议的TCP通信类(不是串口透传通信)，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据
		/// </summary>
		[Description("698.45协议的TCP通信类(不是串口透传通信)，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "StationStr", "EnableCodeFE", "UseSecurityResquest", "CA"
		})]
		DLT698TcpNet,
		/// <summary>
		/// 698.45协议的串口通信，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据
		/// </summary>
		[Description("698.45协议的串口通信，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "ReceiveTimeOut",
			"StationStr", "EnableCodeFE", "UseSecurityResquest", "CA"
		})]
		DLT698,
		/// <summary>
		/// 698.45协议的串口转网口透传通信类(不是TCP通信)，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据
		/// </summary>
		[Description("698.45协议的串口转网口透传通信类(不是TCP通信)，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "IsServerActivePush", "StationStr", "EnableCodeFE", "UseSecurityResquest", "CA"
		})]
		DLT698OverTcp,
		/// <summary>
		/// DTSU6606型三相四线电子式电能表的Modbus-RTU通信协议
		/// </summary>
		[Description("DTSU6606型三相四线电子式电能表的Modbus-RTU通信协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "ReceiveTimeOut",
			"AddressStartWithZero", "IsStringReverse", "DataFormat", "Station", "Crc16CheckEnable", "IsClearCacheBeforeRead", "StationCheckMatch"
		})]
		DTSU6606Serial,
		/// <summary>
		/// 基于多功能电能表通信协议实现的串口通讯，参考的文档是DLT645-2007，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 00-00-00-00
		/// </summary>
		[Description("基于多功能电能表通信协议实现的串口通讯，参考的文档是DLT645-2007，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 00-00-00-00")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "ReceiveTimeOut",
			"CheckDataId", "StationStr", "EnableCodeFE", "Password", "OpCode"
		})]
		DLT645,
		/// <summary>
		/// 基于多功能电能表通信协议实现的串口转网口透传通信，参考的文档是DLT645-2007，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 00-00-00-00
		/// </summary>
		[Description("基于多功能电能表通信协议实现的串口转网口透传通信，参考的文档是DLT645-2007，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 00-00-00-00")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "CheckDataId", "StationStr", "EnableCodeFE", "Password", "OpCode"
		})]
		DLT645OverTcp,
		/// <summary>
		/// 基于多功能电能表通信协议实现的串口通讯，参考的文档是DLT645-1997，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 B6-11
		/// </summary>
		[Description("基于多功能电能表通信协议实现的串口通讯，参考的文档是DLT645-1997，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 B6-11")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "ReceiveTimeOut",
			"CheckDataId", "StationStr", "EnableCodeFE"
		})]
		DLT645With1997,
		/// <summary>
		/// 基于多功能电能表通信协议实现的串口转网口透传通信，参考的文档是DLT645-1997，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 B6-11
		/// </summary>
		[Description("基于多功能电能表通信协议实现的串口转网口透传通信，参考的文档是DLT645-1997，主要实现了对电表数据的读取和一些功能方法，数据标识格式为 B6-11")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "CheckDataId", "StationStr", "EnableCodeFE"
		})]
		DLT645With1997OverTcp,
		/// <summary>
		/// 城市建设部的188协议，基于DJ/T188-2004实现的协议
		/// </summary>
		[Description("城市建设部的188协议，基于DJ/T188-2004实现的协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "StationStr", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime",
			"ReceiveTimeOut", "StationStr", "InstrumentType", "EnableCodeFE"
		})]
		CJT188,
		/// <summary>
		/// 城市建设部的188协议网口透传，基于DJ/T188-2004实现的协议
		/// </summary>
		[Description("城市建设部的188协议网口透传，基于DJ/T188-2004实现的协议")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "StationStr", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut",
			"SleepTime", "SocketKeepAliveTime", "IsPersistentConnection", "StationStr", "InstrumentType", "EnableCodeFE"
		})]
		CJT188OverTcp
	}
}
/// <summary>
/// 电力通讯规约通信库
/// </summary>
public class PQDIFOperate : DaqAbstract<PQDIFOperate, PQDIFData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "电力通讯规约";

	/// <inheritdoc />
	protected override string CD => "支持对“电力通讯规约”多型号、多数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public PQDIFOperate()
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
	public PQDIFOperate(PQDIFData.Basics basics)
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
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OnAsync", 75);
			}
			OResult oResult = null;
			switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
			{
			case PQDIFData.ProtocolType.DLT698:
			{
				DLT698 val5 = new DLT698();
				((BinaryCommunication)val5).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut
				};
				val5.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				val5.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				val5.UseSecurityResquest = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.UseSecurityResquest;
				val5.CA = (byte)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.CA;
				oResult = ((DeviceSerialPort)val5).Open();
				plcBase = val5;
				break;
			}
			case PQDIFData.ProtocolType.DLT698TcpNet:
			{
				DLT698TcpNet dLT698TcpNet = new DLT698TcpNet();
				((BinaryCommunication)dLT698TcpNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((DLT698OverTcp)dLT698TcpNet).Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				((DLT698OverTcp)dLT698TcpNet).EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				((DLT698OverTcp)dLT698TcpNet).UseSecurityResquest = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.UseSecurityResquest;
				((DLT698OverTcp)dLT698TcpNet).CA = (byte)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.CA;
				oResult = await ((DeviceTcpNet)dLT698TcpNet).ConnectServerAsync();
				plcBase = dLT698TcpNet;
				break;
			}
			case PQDIFData.ProtocolType.CJT188OverTcp:
			{
				CJT188OverTcp cJT188OverTcp = new CJT188OverTcp(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr);
				((BinaryCommunication)cJT188OverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsPersistentConnection
				};
				cJT188OverTcp.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				cJT188OverTcp.InstrumentType = (byte)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.InstrumentType;
				cJT188OverTcp.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				oResult = await ((DeviceTcpNet)cJT188OverTcp).ConnectServerAsync();
				plcBase = cJT188OverTcp;
				break;
			}
			case PQDIFData.ProtocolType.DLT645With1997OverTcp:
			{
				DLT645With1997OverTcp dLT645With1997OverTcp = new DLT645With1997OverTcp();
				((BinaryCommunication)dLT645With1997OverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsPersistentConnection
				};
				dLT645With1997OverTcp.CheckDataId = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.CheckDataId;
				dLT645With1997OverTcp.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				dLT645With1997OverTcp.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				oResult = await ((DeviceTcpNet)dLT645With1997OverTcp).ConnectServerAsync();
				plcBase = dLT645With1997OverTcp;
				break;
			}
			case PQDIFData.ProtocolType.DLT645:
			{
				DLT645 val4 = new DLT645();
				((BinaryCommunication)val4).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut
				};
				val4.CheckDataId = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.CheckDataId;
				val4.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				val4.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				val4.Password = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Password;
				val4.OpCode = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.OpCode;
				oResult = ((DeviceSerialPort)val4).Open();
				plcBase = val4;
				break;
			}
			case PQDIFData.ProtocolType.DLT698OverTcp:
			{
				DLT698OverTcp dLT698OverTcp = new DLT698OverTcp();
				((BinaryCommunication)dLT698OverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsPersistentConnection
				};
				dLT698OverTcp.IsServerActivePush = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsServerActivePush;
				dLT698OverTcp.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				dLT698OverTcp.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				dLT698OverTcp.UseSecurityResquest = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.UseSecurityResquest;
				dLT698OverTcp.CA = (byte)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.CA;
				oResult = await ((DeviceTcpNet)dLT698OverTcp).ConnectServerAsync();
				plcBase = dLT698OverTcp;
				break;
			}
			case PQDIFData.ProtocolType.DTSU6606Serial:
			{
				DTSU6606Serial val3 = new DTSU6606Serial();
				((BinaryCommunication)val3).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut
				};
				((ModbusRtu)val3).AddressStartWithZero = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.AddressStartWithZero;
				((ModbusRtu)val3).IsStringReverse = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsStringReverse;
				((ModbusRtu)val3).DataFormat = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.DataFormat;
				((ModbusRtu)val3).Station = (byte)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Station;
				((ModbusRtu)val3).Crc16CheckEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Crc16CheckEnable;
				((DeviceSerialPort)val3).IsClearCacheBeforeRead = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsClearCacheBeforeRead;
				((ModbusRtu)val3).StationCheckMatch = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationCheckMatch;
				oResult = ((DeviceSerialPort)val3).Open();
				plcBase = val3;
				break;
			}
			case PQDIFData.ProtocolType.DLT645With1997:
			{
				DLT645With1997 val2 = new DLT645With1997();
				((BinaryCommunication)val2).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut
				};
				val2.CheckDataId = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.CheckDataId;
				val2.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				val2.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				oResult = ((DeviceSerialPort)val2).Open();
				plcBase = val2;
				break;
			}
			case PQDIFData.ProtocolType.CJT188:
			{
				CJT188 val = new CJT188(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr);
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut
				};
				val.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				val.InstrumentType = (byte)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.InstrumentType;
				val.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case PQDIFData.ProtocolType.DLT645OverTcp:
			{
				DLT645OverTcp dLT645OverTcp = new DLT645OverTcp();
				((BinaryCommunication)dLT645OverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.IsPersistentConnection
				};
				dLT645OverTcp.CheckDataId = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.CheckDataId;
				dLT645OverTcp.Station = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.StationStr;
				dLT645OverTcp.EnableCodeFE = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.EnableCodeFE;
				dLT645OverTcp.Password = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.Password;
				dLT645OverTcp.OpCode = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.OpCode;
				oResult = await ((DeviceTcpNet)dLT645OverTcp).ConnectServerAsync();
				plcBase = dLT645OverTcp;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs");
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OnAsync", 255);
			}
			await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OnAsync", 259);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OnAsync", 264);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OffAsync", 278);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OffAsync", 287);
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
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						((DeviceSerialPort)ExtendMethod.GetSource<DLT698>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.DLT645:
						((DeviceSerialPort)ExtendMethod.GetSource<DLT645>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						((DeviceSerialPort)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						((DeviceSerialPort)ExtendMethod.GetSource<DLT645With1997>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.CJT188:
						((DeviceSerialPort)ExtendMethod.GetSource<CJT188>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).Dispose();
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).Dispose();
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
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OffAsync", 349);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "OffAsync", 353);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "ReadAsync", 365);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "ReadAsync", 370);
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
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.DLT645:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.CJT188:
								val9 = null;
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadBoolAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadFloatAsync(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadUInt32Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadUInt64Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadUInt16Async(addressName);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
							{
							case PQDIFData.ProtocolType.DLT698:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698TcpNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188OverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997OverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT698OverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DTSU6606Serial:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645With1997:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.CJT188:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case PQDIFData.ProtocolType.DLT645OverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "ReadAsync", 1570);
			}
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "ReadAsync", 1574);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "ReadAsync", 1579);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "WriteAsync", 1591);
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
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.ProtocolType)
					{
					case PQDIFData.ProtocolType.DLT698:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.DLT698TcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698TcpNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.CJT188OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188OverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.DLT645With1997OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997OverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.DLT645:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.DLT698OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT698OverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.DTSU6606Serial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DTSU6606Serial>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.DLT645With1997:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645With1997>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.CJT188:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<CJT188>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case PQDIFData.ProtocolType.DLT645OverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<DLT645OverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "WriteAsync", 2390);
			}
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "WriteAsync", 2393);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "WriteAsync", 2397);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "SubscribeAsync", 2409);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "SubscribeAsync", 2414);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "SubscribeAsync", 2422);
			}
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "SubscribeAsync", 2426);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "SubscribeAsync", 2431);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "UnSubscribeAsync", 2443);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "UnSubscribeAsync", 2448);
			}
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "UnSubscribeAsync", 2452);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "UnSubscribeAsync", 2457);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "GetStatusAsync", 2471);
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
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "GetStatusAsync", 2478);
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
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "GetStatusAsync", 2485);
		}
		return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "GetStatusAsync", 2488);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<PQDIFOperate, PQDIFData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "GetBaseObjectAsync", 2497);
		}
		return await ((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.PQDIF\\PQDIFOperate.cs", "GetBaseObjectAsync", 2500);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<PQDIFOperate, PQDIFData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
