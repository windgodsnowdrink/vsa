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
using Snet.Driver.Profinet.MegMeet;
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
[assembly: AssemblyDescription("采集协议：MegMeet（麦格米特 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.MegMeet")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.MegMeet;

/// <summary>
/// 麦格米特数据
/// </summary>
public class MegMeetData
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
		/// 协议类型
		/// </summary>
		[Description("协议类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[AutoAllocatingTag(typeof(ProtocolType))]
		[Display(/*Could not decode attribute arguments.*/)]
		public ProtocolType ProtocolType { get; set; }

		public Basics()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
			IpAddress = "127.0.0.1";
			Port = 6688;
			ConnectTimeOut = 1000;
			ReceiveTimeOut = 1000;
			SocketKeepAliveTime = -1;
			IsPersistentConnection = true;
			AddressStartWithZero = true;
			DataFormat = (DataFormat)2;
			Station = 1;
			SerialPortInfo = "COM3-9600-8-N-1";
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
		/// 麦格米特PLC 基于ModbusTcp协议实现，适用机型为 MC80/MC100/MC200/MC280/MC200E
		/// </summary>
		[Description("麦格米特PLC 基于ModbusTcp协议实现，适用机型为 MC80/MC100/MC200/MC280/MC200E")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "AddressStartWithZero", "IsStringReverse", "DataFormat", "Station"
		})]
		MegMeetTcpNet,
		/// <summary>
		/// 麦格米特PLC 基于ModbusRtu协议实现，适用机型为 MC80/MC100/MC200/MC280/MC200E
		/// </summary>
		[Description("麦格米特PLC 基于ModbusRtu协议实现，适用机型为 MC80/MC100/MC200/MC280/MC200E")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "AddressStartWithZero",
			"IsStringReverse", "DataFormat", "Station"
		})]
		MegMeetSerial,
		/// <summary>
		/// 麦格米特PLC 基于ModbusRtu转以太网协议实现，适用机型为 MC80/MC100/MC200/MC280/MC200E
		/// </summary>
		[Description("麦格米特PLC 基于ModbusRtu转以太网协议实现，适用机型为 MC80/MC100/MC200/MC280/MC200E")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "AddressStartWithZero", "IsStringReverse", "DataFormat", "Station"
		})]
		MegMeetSerialOverTcp
	}
}
/// <summary>
/// 麦格米特通信库
/// </summary>
public class MegMeetOperate : DaqAbstract<MegMeetOperate, MegMeetData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "麦格米特";

	/// <inheritdoc />
	protected override string CD => "支持对“麦格米特 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public MegMeetOperate()
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
	public MegMeetOperate(MegMeetData.Basics basics)
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
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
			{
			case MegMeetData.ProtocolType.MegMeetSerial:
			{
				MegMeetSerial val = new MegMeetSerial();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.SleepTime
				};
				((ModbusRtu)val).AddressStartWithZero = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.AddressStartWithZero;
				((ModbusRtu)val).IsStringReverse = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.IsStringReverse;
				((ModbusRtu)val).DataFormat = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.DataFormat;
				((ModbusRtu)val).Station = (byte)((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.Station;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
			{
				MegMeetSerialOverTcp megMeetSerialOverTcp = new MegMeetSerialOverTcp();
				((BinaryCommunication)megMeetSerialOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((ModbusRtuOverTcp)megMeetSerialOverTcp).AddressStartWithZero = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.AddressStartWithZero;
				((ModbusRtuOverTcp)megMeetSerialOverTcp).IsStringReverse = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.IsStringReverse;
				((ModbusRtuOverTcp)megMeetSerialOverTcp).DataFormat = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.DataFormat;
				((ModbusRtuOverTcp)megMeetSerialOverTcp).Station = (byte)((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.Station;
				oResult = await ((DeviceTcpNet)megMeetSerialOverTcp).ConnectServerAsync();
				plcBase = megMeetSerialOverTcp;
				break;
			}
			case MegMeetData.ProtocolType.MegMeetTcpNet:
			{
				MegMeetTcpNet megMeetTcpNet = new MegMeetTcpNet();
				((BinaryCommunication)megMeetTcpNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.IsPersistentConnection
				};
				((ModbusTcpNet)megMeetTcpNet).AddressStartWithZero = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.AddressStartWithZero;
				((ModbusTcpNet)megMeetTcpNet).IsStringReverse = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.IsStringReverse;
				((ModbusTcpNet)megMeetTcpNet).DataFormat = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.DataFormat;
				((ModbusTcpNet)megMeetTcpNet).Station = (byte)((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.Station;
				oResult = await ((DeviceTcpNet)megMeetTcpNet).ConnectServerAsync();
				plcBase = megMeetTcpNet;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs");
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OnAsync", 135);
			}
			await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OnAsync", 139);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OnAsync", 144);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OffAsync", 158);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OffAsync", 167);
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
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						((DeviceSerialPort)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).Dispose();
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).Dispose();
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).Dispose();
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
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OffAsync", 201);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "OffAsync", 205);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "ReadAsync", 217);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "ReadAsync", 222);
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
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val9 = null;
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val9 = null;
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadBoolAsync(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadFloatAsync(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadInt32Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadInt32Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadUInt32Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadInt64Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadInt64Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadUInt64Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadInt16Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadInt16Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadUInt16Async(addressName);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
							{
							case MegMeetData.ProtocolType.MegMeetSerial:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case MegMeetData.ProtocolType.MegMeetTcpNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "ReadAsync", 981);
			}
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "ReadAsync", 985);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "ReadAsync", 990);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "WriteAsync", 1002);
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
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.ProtocolType)
					{
					case MegMeetData.ProtocolType.MegMeetSerial:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerial>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MegMeetData.ProtocolType.MegMeetSerialOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetSerialOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case MegMeetData.ProtocolType.MegMeetTcpNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<MegMeetTcpNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "WriteAsync", 1360);
			}
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "WriteAsync", 1363);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "WriteAsync", 1367);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "SubscribeAsync", 1379);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "SubscribeAsync", 1384);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "SubscribeAsync", 1392);
			}
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "SubscribeAsync", 1396);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "SubscribeAsync", 1401);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "UnSubscribeAsync", 1413);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "UnSubscribeAsync", 1418);
			}
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "UnSubscribeAsync", 1422);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "UnSubscribeAsync", 1427);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "GetStatusAsync", 1441);
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
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "GetStatusAsync", 1448);
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
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "GetStatusAsync", 1455);
		}
		return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "GetStatusAsync", 1458);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<MegMeetOperate, MegMeetData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "GetBaseObjectAsync", 1467);
		}
		return await ((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.MegMeet\\MegMeetOperate.cs", "GetBaseObjectAsync", 1470);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<MegMeetOperate, MegMeetData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
