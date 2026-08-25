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
using Snet.Driver.Profinet.Fuji;
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
[assembly: AssemblyDescription("采集协议：Fuji（富士 PLC 通信库，支持多型号、多类型读取写入，并支持数据订阅模式）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.Fuji")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Fuji;

/// <summary>
/// 富士数据
/// </summary>
public class FujiData
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
		/// Cpu或模块站号
		/// </summary>
		[Description("Cpu或模块站号")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int ConnectionID { get; set; }

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
		/// 站号信息
		/// </summary>
		[Description("站号信息")]
		[Display(/*Could not decode attribute arguments.*/)]
		public int Station { get; set; }

		/// <summary>
		/// 进行数据交换
		/// </summary>
		[Description("进行数 据交换")]
		[Display(/*Could not decode attribute arguments.*/)]
		public bool DataSwap { get; set; }

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
			ConnectionID = 1;
			DataFormat = (DataFormat)2;
			SerialPortInfo = "COM3-9600-8-N-1";
			Station = 1;
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
		/// 富士PLC SPH通信协议，可以和富士PLC进行通信，ConnectionID 默认CPU0，需要根据实际进行调整
		/// </summary>
		[Description("富士PLC SPH通信协议，可以和富士PLC进行通信，ConnectionID 默认CPU0，需要根据实际进行调整")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "ConnectionID", "DataFormat"
		})]
		FujiSPHNet,
		/// <summary>
		/// 富士PLC SPB协议，地址可以携带站号信息，例如：s=2;D100，PLC侧需要配置无BCC计算，包含0D0A结束码
		/// </summary>
		[Description("富士PLC SPB协议，地址可以携带站号信息，例如：s=2;D100，PLC侧需要配置无BCC计算，包含0D0A结束码")]
		[AutoAllocating(new string[] { "SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "SerialPortInfo", "RtsEnable", "DtrEnable", "SleepTime", "Station" })]
		FujiSPB,
		/// <summary>
		/// 富士PLC SPB协议，地址可以携带站号信息，例如：s=2;D100，PLC侧需要配置无BCC计算，包含0D0A结束码
		/// </summary>
		[Description("富士PLC SPB协议，地址可以携带站号信息，例如：s=2;D100，PLC侧需要配置无BCC计算，包含0D0A结束码")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "Station"
		})]
		FujiSPBOverTcp,
		/// <summary>
		/// 富士PLC SPH通信协议，基于Command-Setting-Type通信模式的协议实现
		/// </summary>
		[Description("富士PLC SPH通信协议，基于Command-Setting-Type通信模式的协议实现")]
		[AutoAllocating(new string[]
		{
			"SN", "HandleInterval", "ChangeOut", "AllOut", "TaskNumber", "IpAddress", "Port", "ConnectTimeOut", "ReceiveTimeOut", "SleepTime",
			"SocketKeepAliveTime", "IsPersistentConnection", "DataSwap"
		})]
		FujiCommandSettingType
	}
}
/// <summary>
/// 富士通信库
/// </summary>
public class FujiOperate : DaqAbstract<FujiOperate, FujiData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
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
	protected override string CN => "富士";

	/// <inheritdoc />
	protected override string CD => "支持对“富士 Plc”多种数据类型读取写入，并支持数据订阅模式";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public FujiOperate()
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
	public FujiOperate(FujiData.Basics basics)
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
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OnAsync", 73);
			}
			OResult oResult = null;
			switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
			{
			case FujiData.ProtocolType.FujiSPHNet:
			{
				FujiSPHNet fujiSPHNet = new FujiSPHNet();
				((BinaryCommunication)fujiSPHNet).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				fujiSPHNet.ConnectionID = (byte)((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ConnectionID;
				((DeviceCommunication)fujiSPHNet).ByteTransform.DataFormat = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.DataFormat;
				oResult = await ((DeviceTcpNet)fujiSPHNet).ConnectServerAsync();
				plcBase = fujiSPHNet;
				break;
			}
			case FujiData.ProtocolType.FujiCommandSettingType:
			{
				FujiCommandSettingType fujiCommandSettingType = new FujiCommandSettingType();
				((BinaryCommunication)fujiCommandSettingType).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				fujiCommandSettingType.DataSwap = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.DataSwap;
				oResult = await ((DeviceTcpNet)fujiCommandSettingType).ConnectServerAsync();
				plcBase = fujiCommandSettingType;
				break;
			}
			case FujiData.ProtocolType.FujiSPB:
			{
				FujiSPB val = new FujiSPB();
				((BinaryCommunication)val).CommunicationPipe = (CommunicationPipe)new PipeSerialPort(((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SerialPortInfo)
				{
					RtsEnable = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.RtsEnable,
					DtrEnable = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.DtrEnable,
					SleepTime = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SleepTime
				};
				val.Station = (byte)((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.Station;
				oResult = ((DeviceSerialPort)val).Open();
				plcBase = val;
				break;
			}
			case FujiData.ProtocolType.FujiSPBOverTcp:
			{
				FujiSPBOverTcp fujiSPBOverTcp = new FujiSPBOverTcp();
				((BinaryCommunication)fujiSPBOverTcp).CommunicationPipe = (CommunicationPipe)new PipeTcpNet(((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.IpAddress, ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.Port)
				{
					ConnectTimeOut = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ConnectTimeOut,
					ReceiveTimeOut = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ReceiveTimeOut,
					SleepTime = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SleepTime,
					SocketKeepAliveTime = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SocketKeepAliveTime,
					IsPersistentConnection = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.IsPersistentConnection
				};
				fujiSPBOverTcp.Station = (byte)((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.Station;
				oResult = await ((DeviceTcpNet)fujiSPBOverTcp).ConnectServerAsync();
				plcBase = fujiSPBOverTcp;
				break;
			}
			}
			if (oResult.IsSuccess)
			{
				LogModel source = ((ResultModel)(await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).LogOperateGetAsync(token))).GetSource<LogModel>();
				plcLog = new LogOperate(plcBase, LogHelper.Get().FolderName, source.Out, source.ConsoleOut == true, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs");
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OnAsync", 141);
			}
			await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, oResult.Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OnAsync", 145);
		}
		catch (Exception ex)
		{
			await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).OffAsync(true, token);
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OnAsync", 150);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OffAsync", 164);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(hardClose, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OffAsync", 173);
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
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						await ((DeviceTcpNet)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).Dispose();
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						await ((DeviceTcpNet)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).Dispose();
						break;
					case FujiData.ProtocolType.FujiSPB:
						((DeviceSerialPort)ExtendMethod.GetSource<FujiSPB>(plcBase)).Close();
						((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).Dispose();
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						await ((DeviceTcpNet)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ConnectCloseAsync();
						((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).Dispose();
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
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OffAsync", 211);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "OffAsync", 215);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "ReadAsync", 227);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "ReadAsync", 232);
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
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val9 = null;
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val9 = await ExtendMethod.GetSource<FujiCommandSettingType>(plcBase).ReadByteAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val9 = null;
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 1:
						{
							OResult<bool> val15 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadBoolAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadBoolAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadBoolAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val15 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadBoolAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 2:
						{
							OResult<double> val6 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadDoubleAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val6 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadDoubleAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 3:
						case 4:
						{
							OResult<float> val12 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadFloatAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadFloatAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadFloatAsync(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val12 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadFloatAsync(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 9:
						case 10:
						{
							OResult<int> val10 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadInt32Async(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadInt32Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadInt32Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val10 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 11:
						case 12:
						{
							OResult<uint> val5 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadUInt32Async(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadUInt32Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadUInt32Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val5 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadUInt32Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 13:
						case 14:
						{
							OResult<long> val21 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadInt64Async(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadInt64Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadInt64Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val21 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 15:
						case 16:
						{
							OResult<ulong> val18 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadUInt64Async(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadUInt64Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadUInt64Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val18 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadUInt64Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 5:
						case 6:
						{
							OResult<short> val16 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadInt16Async(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadInt16Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadInt16Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val16 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 7:
						case 8:
						{
							OResult<ushort> val3 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadUInt16Async(addressName);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadUInt16Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadUInt16Async(addressName);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val3 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadUInt16Async(addressName);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 20:
						case 21:
						{
							OResult<string> val14 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadStringAsync(addressName, length, encoding);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val14 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadStringAsync(addressName, length, encoding);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 22:
						{
							OResult<byte[]> val2 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val2 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 23:
						{
							OResult<bool[]> val17 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadBoolAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val17 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadBoolAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 24:
						{
							OResult<double[]> val8 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadDoubleAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val8 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadDoubleAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 25:
						case 26:
						{
							OResult<float[]> val20 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadFloatAsync(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val20 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadFloatAsync(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 27:
						case 28:
						{
							OResult<short[]> val11 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadInt16Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val11 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 29:
						case 30:
						{
							OResult<ushort[]> val4 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadUInt16Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val4 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadUInt16Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 31:
						case 32:
						{
							OResult<int[]> val19 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadInt32Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val19 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 33:
						case 34:
						{
							OResult<uint[]> val13 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadUInt32Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val13 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadUInt32Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 35:
						case 36:
						{
							OResult<long[]> val7 = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadInt64Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val7 = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
							}
							break;
						}
						case 37:
						case 38:
						{
							OResult<ulong[]> val = null;
							switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
							{
							case FujiData.ProtocolType.FujiSPHNet:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiCommandSettingType:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPB:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).ReadUInt64Async(addressName, length);
								break;
							case FujiData.ProtocolType.FujiSPBOverTcp:
								val = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).ReadUInt64Async(addressName, length);
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
								Msg = string.Format(await LanguageHandler.GetLanguageValueAsync("“{0}”不支持“{1}”类型读取", (LanguageModel)null, token), ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType.ToString(), item.AddressDataType);
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
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "ReadAsync", 1054);
			}
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "ReadAsync", 1058);
		}
		catch (Exception ex2)
		{
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ex2.Message, (object)null, ex2, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "ReadAsync", 1063);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "WriteAsync", 1075);
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
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ExtendMethod.GetSource<FujiCommandSettingType>(plcBase).WriteAsync(key, ExtendMethod.GetSource<byte>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, (short)ExtendMethod.GetSource<byte>(value));
						break;
					}
				}
				else if (value is byte[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<byte[]>(value));
						break;
					}
				}
				else if (value is short || value is short)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt16(value));
						break;
					}
				}
				else if (value is short[] || value is short[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<short[]>(value));
						break;
					}
				}
				else if (value is ushort || value is ushort)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt16(value));
						break;
					}
				}
				else if (value is ushort[] || value is ushort[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ushort[]>(value));
						break;
					}
				}
				else if (value is int || value is int)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt32(value));
						break;
					}
				}
				else if (value is int[] || value is int[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<int[]>(value));
						break;
					}
				}
				else if (value is uint || value is uint)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt32(value));
						break;
					}
				}
				else if (value is uint[] || value is uint[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<uint[]>(value));
						break;
					}
				}
				else if (value is long || value is long)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToInt64(value));
						break;
					}
				}
				else if (value is long[] || value is long[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<long[]>(value));
						break;
					}
				}
				else if (value is ulong || value is ulong)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToUInt64(value));
						break;
					}
				}
				else if (value is ulong[] || value is ulong[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<ulong[]>(value));
						break;
					}
				}
				else if (value is float || value is float)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToSingle(value));
						break;
					}
				}
				else if (value is float[] || value is float[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<float[]>(value));
						break;
					}
				}
				else if (value is double)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToDouble(value));
						break;
					}
				}
				else if (value is double[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<double[]>(value));
						break;
					}
				}
				else if (value is bool)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToBoolean(value));
						break;
					}
				}
				else if (value is bool[])
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, ExtendMethod.GetSource<bool[]>(value));
						break;
					}
				}
				else if (value is string || value is char)
				{
					switch (((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.ProtocolType)
					{
					case FujiData.ProtocolType.FujiSPHNet:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPHNet>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case FujiData.ProtocolType.FujiCommandSettingType:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiCommandSettingType>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case FujiData.ProtocolType.FujiSPB:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPB>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
						break;
					case FujiData.ProtocolType.FujiSPBOverTcp:
						oResult = await ((DeviceCommunication)ExtendMethod.GetSource<FujiSPBOverTcp>(plcBase)).WriteAsync(key, Convert.ToString(value), encoding);
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
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "WriteAsync", 1496);
			}
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "WriteAsync", 1499);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "WriteAsync", 1503);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "SubscribeAsync", 1515);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "SubscribeAsync", 1520);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEventAsync += ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).OnDataEventHandlerAsync;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEventAsync += ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).OnInfoEventHandlerAsync;
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "SubscribeAsync", 1528);
			}
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "SubscribeAsync", 1532);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "SubscribeAsync", 1537);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "UnSubscribeAsync", 1549);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "UnSubscribeAsync", 1554);
			}
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "UnSubscribeAsync", 1558);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "UnSubscribeAsync", 1563);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
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
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(flag, flag ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "GetStatusAsync", 1577);
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
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(flag2, flag2 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "GetStatusAsync", 1584);
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
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(flag3, flag3 ? connectedMessage : unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "GetStatusAsync", 1591);
		}
		return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, unconnectedMessage, (object)null, (Exception)null, true, false, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "GetStatusAsync", 1594);
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await ((DaqAbstract<FujiOperate, FujiData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
		{
			return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "GetBaseObjectAsync", 1603);
		}
		return await ((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).EndOperateAsync(true, (string)null, plcBase, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Fuji\\FujiOperate.cs", "GetBaseObjectAsync", 1606);
	}

	/// <summary>
	/// 语言处理
	/// </summary>
	private void LanguageHandler()
	{
		((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).OnLanguageEventAsync -= LanguageEventAsync;
		((CoreUnify<FujiOperate, FujiData.Basics>)(object)this).OnLanguageEventAsync += LanguageEventAsync;
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
