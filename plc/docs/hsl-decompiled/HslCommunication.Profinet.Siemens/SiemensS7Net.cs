// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Pipe;
using HslCommunication.Profinet.Siemens.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// 一个西门子的客户端类，使用S7协议来进行数据交互，对于s300,s400需要关注和的设置值，
/// 对于s200，需要关注和的设置值，详细参考demo的设置。
/// A Siemens client class uses the S7 protocol for data exchange. For s300 and s400,
/// you need to pay attention to the setting values of  and . For s200,
/// you need to pay attention to  and . See cref="LocalTSAP"/> and  settings,
/// please refer to the demo settings for details.
/// </summary>
/// <remarks>
/// 暂时不支持bool[]的批量写入操作，请使用 Write(string, byte[]) 替换。
/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100，当然了你也可以输入V100
/// 如果读取PLC的字符串string数据，可以使用
/// </remarks>
public class SiemensS7Net : DeviceTcpNet
{
	private byte[] plcHead1 = new byte[22]
	{
		3, 0, 0, 22, 17, 224, 0, 0, 0, 1,
		0, 192, 1, 10, 193, 2, 1, 2, 194, 2,
		1, 0
	};

	private byte[] plcHead2 = new byte[25]
	{
		3, 0, 0, 25, 2, 240, 128, 50, 1, 0,
		0, 4, 0, 0, 8, 0, 0, 240, 0, 0,
		1, 0, 1, 1, 224
	};

	private byte[] plcOrderNumber = new byte[33]
	{
		3, 0, 0, 33, 2, 240, 128, 50, 7, 0,
		0, 0, 1, 0, 8, 0, 8, 0, 1, 18,
		4, 17, 68, 1, 0, 255, 9, 0, 4, 0,
		17, 0, 0
	};

	private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;

	private byte[] plcHead1_200smart = new byte[22]
	{
		3, 0, 0, 22, 17, 224, 0, 0, 0, 1,
		0, 193, 2, 16, 0, 194, 2, 3, 0, 192,
		1, 10
	};

	private byte[] plcHead2_200smart = new byte[25]
	{
		3, 0, 0, 25, 2, 240, 128, 50, 1, 0,
		0, 204, 193, 0, 8, 0, 0, 240, 0, 0,
		1, 0, 1, 3, 192
	};

	private byte[] plcHead1_200 = new byte[22]
	{
		3, 0, 0, 22, 17, 224, 0, 0, 0, 1,
		0, 193, 2, 77, 87, 194, 2, 77, 87, 192,
		1, 9
	};

	private byte[] plcHead2_200 = new byte[25]
	{
		3, 0, 0, 25, 2, 240, 128, 50, 1, 0,
		0, 0, 0, 0, 8, 0, 0, 240, 0, 0,
		1, 0, 1, 3, 192
	};

	private byte[] S7_STOP = new byte[33]
	{
		3, 0, 0, 33, 2, 240, 128, 50, 1, 0,
		0, 14, 0, 0, 16, 0, 0, 41, 0, 0,
		0, 0, 0, 9, 80, 95, 80, 82, 79, 71,
		82, 65, 77
	};

	private byte[] S7_HOT_START = new byte[37]
	{
		3, 0, 0, 37, 2, 240, 128, 50, 1, 0,
		0, 12, 0, 0, 20, 0, 0, 40, 0, 0,
		0, 0, 0, 0, 253, 0, 0, 9, 80, 95,
		80, 82, 79, 71, 82, 65, 77
	};

	private byte[] S7_COLD_START = new byte[39]
	{
		3, 0, 0, 39, 2, 240, 128, 50, 1, 0,
		0, 15, 0, 0, 22, 0, 0, 40, 0, 0,
		0, 0, 0, 0, 253, 0, 2, 67, 32, 9,
		80, 95, 80, 82, 79, 71, 82, 65, 77
	};

	private byte plc_rack = 0;

	private byte plc_slot = 0;

	private int pdu_length = 200;

	private int pdu_length_set = -1;

	private int pdu_length_default = 200;

	private const byte pduStart = 40;

	private const byte pduStop = 41;

	private const byte pduAlreadyStarted = 2;

	private const byte pduAlreadyStopped = 7;

	private SoftIncrementCount incrementCount = new SoftIncrementCount(65535L, 1L);

	/// <summary>
	/// PLC的槽号，针对S7-400的PLC设置的
	/// The slot number of PLC is set for PLC of s7-400
	/// </summary>
	public byte Slot
	{
		get
		{
			return plc_slot;
		}
		set
		{
			plc_slot = value;
			if (CurrentPlc != SiemensPLCS.S200 && CurrentPlc != SiemensPLCS.S200Smart)
			{
				plcHead1[21] = (byte)(plc_rack * 32 + plc_slot);
			}
		}
	}

	/// <summary>
	/// PLC的机架号，针对S7-400的PLC设置的
	/// The frame number of the PLC is set for the PLC of s7-400
	/// </summary>
	public byte Rack
	{
		get
		{
			return plc_rack;
		}
		set
		{
			plc_rack = value;
			if (CurrentPlc != SiemensPLCS.S200 && CurrentPlc != SiemensPLCS.S200Smart)
			{
				plcHead1[21] = (byte)(plc_rack * 32 + plc_slot);
			}
		}
	}

	/// <summary>
	/// 获取或设置当前PLC的连接方式，PG: 0x01，OP: 0x02，S7Basic: 0x03...0x10
	/// Get or set the current PLC connection mode, PG: 0x01, OP: 0x02, S7Basic: 0x03...0x10
	/// </summary>
	public byte ConnectionType
	{
		get
		{
			return plcHead1[20];
		}
		set
		{
			if (CurrentPlc != SiemensPLCS.S200 && CurrentPlc != SiemensPLCS.S200Smart)
			{
				plcHead1[20] = value;
			}
		}
	}

	/// <summary>
	/// 西门子相关的本地TSAP参数信息
	/// A parameter information related to Siemens
	/// </summary>
	public int LocalTSAP
	{
		get
		{
			if (CurrentPlc == SiemensPLCS.S200 || CurrentPlc == SiemensPLCS.S200Smart)
			{
				return plcHead1[13] * 256 + plcHead1[14];
			}
			return plcHead1[16] * 256 + plcHead1[17];
		}
		set
		{
			if (CurrentPlc == SiemensPLCS.S200 || CurrentPlc == SiemensPLCS.S200Smart)
			{
				plcHead1[13] = BitConverter.GetBytes(value)[1];
				plcHead1[14] = BitConverter.GetBytes(value)[0];
			}
			else
			{
				plcHead1[16] = BitConverter.GetBytes(value)[1];
				plcHead1[17] = BitConverter.GetBytes(value)[0];
			}
		}
	}

	/// <summary>
	/// 西门子相关的远程TSAP参数信息
	/// A parameter information related to Siemens
	/// </summary>
	public int DestTSAP
	{
		get
		{
			if (CurrentPlc == SiemensPLCS.S200 || CurrentPlc == SiemensPLCS.S200Smart)
			{
				return plcHead1[17] * 256 + plcHead1[18];
			}
			return plcHead1[20] * 256 + plcHead1[21];
		}
		set
		{
			if (CurrentPlc == SiemensPLCS.S200 || CurrentPlc == SiemensPLCS.S200Smart)
			{
				plcHead1[17] = BitConverter.GetBytes(value)[1];
				plcHead1[18] = BitConverter.GetBytes(value)[0];
			}
			else
			{
				plcHead1[20] = BitConverter.GetBytes(value)[1];
				plcHead1[21] = BitConverter.GetBytes(value)[0];
			}
		}
	}

	/// <summary>
	/// 获取当前西门子的PDU的长度信息，不同型号PLC的值会不一样。例如设置值为300，如果想要恢复默认值的话，设置为-1
	/// Get the length information of the current Siemens PDU, the value of different types of PLC will be different.
	/// </summary>
	/// <remarks>
	/// V12.6.0版本开放修改值，方便用户手动强制设置pdu长度信息
	/// </remarks>
	public int PDULength
	{
		get
		{
			return pdu_length;
		}
		set
		{
			if (value > 100)
			{
				pdu_length = value;
			}
			else
			{
				pdu_length = pdu_length_default;
			}
			pdu_length_set = value;
		}
	}

	/// <summary>
	/// 实例化一个西门子的S7协议的通讯对象
	/// Instantiate a communication object for a Siemens S7 protocol
	/// </summary>
	/// <param name="siemens">指定西门子的型号</param>
	public SiemensS7Net(SiemensPLCS siemens)
	{
		Initialization(siemens, string.Empty);
	}

	/// <summary>
	/// 实例化一个西门子的S7协议的通讯对象并指定Ip地址
	/// Instantiate a communication object for a Siemens S7 protocol and specify an IP address
	/// </summary>
	/// <param name="siemens">指定西门子的型号</param>
	/// <param name="ipAddress">Ip地址</param>
	public SiemensS7Net(SiemensPLCS siemens, string ipAddress)
	{
		Initialization(siemens, ipAddress);
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new S7Message();
	}

	/// <summary>
	/// 初始化方法
	/// Initialize method
	/// </summary>
	/// <param name="siemens">指定西门子的型号 -&gt; Designation of Siemens</param>
	/// <param name="ipAddress">Ip地址 -&gt; IpAddress</param>
	private void Initialization(SiemensPLCS siemens, string ipAddress)
	{
		base.WordLength = 2;
		IpAddress = ipAddress;
		Port = 102;
		CurrentPlc = siemens;
		base.ByteTransform = new ReverseBytesTransform();
		switch (siemens)
		{
		case SiemensPLCS.S1200:
			plcHead1[21] = 0;
			break;
		case SiemensPLCS.S300:
			plcHead1[21] = 2;
			break;
		case SiemensPLCS.S400:
			plcHead1[21] = 3;
			plcHead1[17] = 0;
			break;
		case SiemensPLCS.S1500:
			plcHead1[21] = 0;
			break;
		case SiemensPLCS.S200Smart:
			plcHead1 = plcHead1_200smart;
			plcHead2 = plcHead2_200smart;
			break;
		case SiemensPLCS.S200:
			plcHead1 = plcHead1_200;
			plcHead2 = plcHead2_200;
			break;
		default:
			plcHead1[18] = 0;
			break;
		}
	}

	public override OperateResult<byte[]> ReadFromCoreServer(CommunicationPipe pipe, byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		OperateResult<byte[]> operateResult;
		byte[] content;
		do
		{
			operateResult = base.ReadFromCoreServer(pipe, send, hasResponseData, usePackAndUnpack);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			content = operateResult.Content;
		}
		while (content == null || content.Length < 4 || operateResult.Content[2] * 256 + operateResult.Content[3] == 7);
		return operateResult;
	}

	private OperateResult PduSetting(byte[] content)
	{
		pdu_length_default = base.ByteTransform.TransUInt16(content.SelectLast(2), 0) - 28;
		if (pdu_length_default < 200)
		{
			pdu_length_default = 200;
		}
		pdu_length = pdu_length_default;
		if (pdu_length_set > 100)
		{
			pdu_length = pdu_length_set;
		}
		incrementCount = new SoftIncrementCount(65535L, 1L);
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, plcHead1, hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(CommunicationPipe, plcHead2, hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return PduSetting(operateResult2.Content);
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(CommunicationPipe pipe, byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		OperateResult<byte[]> read;
		byte[] content;
		do
		{
			read = await base.ReadFromCoreServerAsync(pipe, send, hasResponseData, usePackAndUnpack);
			if (!read.IsSuccess)
			{
				return read;
			}
			content = read.Content;
		}
		while (content == null || content.Length < 4 || read.Content[2] * 256 + read.Content[3] == 7);
		return read;
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult<byte[]> read_first = await ReadFromCoreServerAsync(CommunicationPipe, plcHead1, hasResponseData: true, usePackAndUnpack: true);
		if (!read_first.IsSuccess)
		{
			return read_first;
		}
		OperateResult<byte[]> read_second = await ReadFromCoreServerAsync(CommunicationPipe, plcHead2, hasResponseData: true, usePackAndUnpack: true);
		if (!read_second.IsSuccess)
		{
			return read_second;
		}
		return PduSetting(read_second.Content);
	}

	private OperateResult<string> ParseOrderNumber(OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		if (read.Content == null || read.Content.Length < 91)
		{
			return new OperateResult<string>(StringResources.Language.ReceiveDataLengthTooShort + "91, Source: " + read.Content.ToHexString(' '));
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(read.Content, 71, 20));
	}

	[HslMqttApi("ReadOrderNumber", "获取到PLC的订货号信息")]
	/// <summary>
	/// 从PLC读取订货号信息
	/// Reading order number information from PLC
	/// </summary>
	/// <returns>CPU的订货号信息 -&gt; Order number information for the CPU</returns>
	public OperateResult<string> ReadOrderNumber()
	{
		return ParseOrderNumber(ReadFromCoreServer(plcOrderNumber));
	}

	/// <summary>
	/// 从PLC读取订货号信息
	/// Reading order number information from PLC
	/// </summary>
	/// <returns>CPU的订货号信息 -&gt; Order number information for the CPU</returns>
	public async Task<OperateResult<string>> ReadOrderNumberAsync()
	{
		return ParseOrderNumber(await ReadFromCoreServerAsync(plcOrderNumber).ConfigureAwait(continueOnCapturedContext: false));
	}

	private OperateResult CheckStartResult(OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		byte[] content = read.Content;
		if (content == null || content.Length < 21)
		{
			return new OperateResult("Receive error length < 21, Content: " + ((content == null) ? "" : content.ToHexString(' ')));
		}
		if (content[19] != 40)
		{
			return new OperateResult("Can not start PLC");
		}
		if (content[20] != 2)
		{
			return new OperateResult("Can not start PLC");
		}
		return OperateResult.CreateSuccessResult();
	}

	private OperateResult CheckStopResult(OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		byte[] content = read.Content;
		if (content == null || content.Length < 21)
		{
			return new OperateResult("Receive error length < 21, Content: " + ((content == null) ? "" : content.ToHexString(' ')));
		}
		if (content[19] != 41)
		{
			return new OperateResult("Can not stop PLC");
		}
		if (content[20] != 7)
		{
			return new OperateResult("Can not stop PLC");
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi]
	/// <summary>
	/// 对PLC进行热启动，目前仅适用于200smart型号
	/// Hot start for PLC, currently only applicable to 200smart model
	/// </summary>
	/// <returns>是否启动成功的结果对象</returns>
	public OperateResult HotStart()
	{
		return CheckStartResult(ReadFromCoreServer(S7_HOT_START));
	}

	[HslMqttApi]
	/// <summary>
	/// 对PLC进行冷启动，目前仅适用于200smart型号
	/// Cold start for PLC, currently only applicable to 200smart model
	/// </summary>
	/// <returns>是否启动成功的结果对象</returns>
	public OperateResult ColdStart()
	{
		return CheckStartResult(ReadFromCoreServer(S7_COLD_START));
	}

	[HslMqttApi]
	/// <summary>
	/// 对PLC进行停止，目前仅适用于200smart型号
	/// Stop the PLC, currently only applicable to the 200smart model
	/// </summary>
	/// <returns>是否启动成功的结果对象</returns>
	public OperateResult Stop()
	{
		return CheckStopResult(ReadFromCoreServer(S7_STOP));
	}

	/// <summary>
	/// 对PLC进行热启动，目前仅适用于200smart型号
	/// Hot start for PLC, currently only applicable to 200smart model
	/// </summary>
	/// <returns>是否启动成功的结果对象</returns>
	public async Task<OperateResult> HotStartAsync()
	{
		return CheckStartResult(await ReadFromCoreServerAsync(S7_HOT_START).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// 对PLC进行冷启动，目前仅适用于200smart型号
	/// Cold start for PLC, currently only applicable to 200smart model
	/// </summary>
	/// <returns>是否启动成功的结果对象</returns>
	public async Task<OperateResult> ColdStartAsync()
	{
		return CheckStartResult(await ReadFromCoreServerAsync(S7_COLD_START).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// 对PLC进行停止，目前仅适用于200smart型号
	/// Stop the PLC, currently only applicable to the 200smart model
	/// </summary>
	/// <returns>是否启动成功的结果对象</returns>
	public async Task<OperateResult> StopAsync()
	{
		return CheckStopResult(await ReadFromCoreServerAsync(S7_STOP).ConfigureAwait(continueOnCapturedContext: false));
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从PLC读取原始的字节数据，地址格式为I100，Q100，DB20.100，M100，长度参数以字节为单位
	/// Read the original byte data from the PLC, the address format is I100, Q100, DB20.100, M100, length parameters in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">读取的数量，以字节为单位 The number of reads, in bytes</param>
	/// <returns>是否读取成功的结果对象 Whether to read the successful result object</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return Read(new S7AddressData[1] { operateResult.Content });
	}

	/// <summary>
	/// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位 ->
	/// Read the data from the PLC, the address format is I100，Q100，DB20.100，M100, in bits units
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	private OperateResult<byte[]> ReadBitFromPLC(string address)
	{
		OperateResult<byte[]> operateResult = BuildBitReadCommand(address, GetMessageId());
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return SiemensS7Helper.AnalysisReadBit(operateResult2.Content);
	}

	[HslMqttApi("ReadAddressArray", "一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致，数组长度无限制")]
	/// <summary>
	/// 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致，数组长度无限制
	/// One-time from the PLC to obtain all the data, in order to return a unified buffer, need to be processed sequentially, two array length must be consistent
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">数据长度数组 Array of data Lengths</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 原先的批量的长度为19，现在已经内部自动处理整合，目前的长度为任意和长度。
	/// </remarks>
	public OperateResult<byte[]> Read(string[] address, ushort[] length)
	{
		S7AddressData[] array = new S7AddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address[i], length[i]);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			array[i] = operateResult.Content;
		}
		return Read(array);
	}

	/// <summary>
	/// 读取西门子的地址数据信息，支持任意个数的数据读取
	/// Read Siemens address data information, support any number of data reading
	/// </summary>
	/// <param name="s7Addresses">西门子的数据地址 Siemens data address</param>
	/// <returns>返回的结果对象信息 -&gt; Whether to read the successful result object</returns>
	public OperateResult<byte[]> Read(S7AddressData[] s7Addresses)
	{
		List<byte> list = new List<byte>();
		List<S7AddressData[]> list2 = SiemensS7Helper.ArraySplitByLength(s7Addresses, pdu_length);
		for (int i = 0; i < list2.Count; i++)
		{
			S7AddressData[] array = list2[i];
			if (array.Length == 1 && array[0].Length > pdu_length)
			{
				S7AddressData[] array2 = SiemensS7Helper.SplitS7Address(array[0], pdu_length);
				for (int j = 0; j < array2.Length; j++)
				{
					OperateResult<byte[]> operateResult = ReadS7AddressData(new S7AddressData[1] { array2[j] });
					if (!operateResult.IsSuccess)
					{
						return operateResult;
					}
					list.AddRange(operateResult.Content);
				}
			}
			else
			{
				OperateResult<byte[]> operateResult2 = ReadS7AddressData(array);
				if (!operateResult2.IsSuccess)
				{
					return operateResult2;
				}
				list.AddRange(operateResult2.Content);
			}
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 单次的读取，只能读取最多19个数组的长度，所以不再对外公开该方法
	/// </summary>
	/// <param name="s7Addresses">西门子的地址对象</param>
	/// <returns>返回的结果对象信息</returns>
	private OperateResult<byte[]> ReadS7AddressData(S7AddressData[] s7Addresses)
	{
		OperateResult<byte[]> operateResult = BuildReadCommand(s7Addresses, GetMessageId());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return AnalysisReadByte(s7Addresses, operateResult2.Content);
	}

	/// <summary>
	/// 基础的写入数据的操作支持
	/// Operational support for the underlying write data
	/// </summary>
	/// <param name="entireValue">完整的字节数据 -&gt; Full byte data</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	private OperateResult WriteBase(byte[] entireValue)
	{
		return ByteTransformHelper.GetResultFromOther(ReadFromCoreServer(entireValue), AnalysisWrite);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
	/// Writes data to the PLC data, in the address format I100,Q100,DB20.100,M100, in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="value">写入的原始数据 -&gt; Raw data written to</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return Write(operateResult.Content, value);
	}

	private OperateResult Write(S7AddressData address, byte[] value)
	{
		int count = value.Length;
		ushort num2 = 0;
		while (num2 < count)
		{
			ushort num3 = (ushort)Math.Min(count - num2, pdu_length);
			byte[] data = base.ByteTransform.TransByte(value, num2, num3);
			OperateResult<byte[]> operateResult = BuildWriteByteCommand(address, data, GetMessageId());
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			OperateResult operateResult2 = WriteBase(operateResult.Content);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			num2 += num3;
			address.AddressStart += num3 * 8;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 使用离散的方式，同时写入多个数据块到不同的地址中去，但是支持的地址数量及写入数据的最大长度是有限制的，不能超过pdu长度限制。
	/// Using the discrete method, multiple blocks of data are written to different addresses at the same time, but the number of supported addresses and the maximum length of the data written are limited, and cannot exceed the PDU length limit.
	/// </summary>
	/// <param name="address">地址数组信息</param>
	/// <param name="data">原始数据列表</param>
	/// <returns>是否写入成功</returns>
	public OperateResult Write(string[] address, List<byte[]> data)
	{
		S7AddressData[] array = new S7AddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address[i], 1);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			array[i] = operateResult.Content;
		}
		OperateResult<byte[]> operateResult2 = BuildWriteByteCommand(array, data, GetMessageId());
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult<byte[]> operateResult3 = ReadFromCoreServer(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		return AnalysisWrite(operateResult3.Content);
	}

	/// <summary>
	/// 从PLC读取原始的字节数据，地址格式为I100，Q100，DB20.100，M100，长度参数以字节为单位
	/// Read the original byte data from the PLC, the address format is I100, Q100, DB20.100, M100, length parameters in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">读取的数量，以字节为单位 The number of reads, in bytes</param>
	/// <returns>是否读取成功的结果对象 Whether to read the successful result object</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<S7AddressData> addressResult = S7AddressData.ParseFrom(address, length);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(addressResult);
		}
		return await ReadAsync(new S7AddressData[1] { addressResult.Content });
	}

	/// <summary>
	/// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位 ->
	/// Read the data from the PLC, the address format is I100，Q100，DB20.100，M100, in bits units
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	private async Task<OperateResult<byte[]>> ReadBitFromPLCAsync(string address)
	{
		OperateResult<byte[]> command = BuildBitReadCommand(address, GetMessageId());
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(command);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content);
		if (!read.IsSuccess)
		{
			return read;
		}
		return SiemensS7Helper.AnalysisReadBit(read.Content);
	}

	/// <summary>
	/// 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致，数组长度无限制
	/// One-time from the PLC to obtain all the data, in order to return a unified buffer, need to be processed sequentially, two array length must be consistent
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">数据长度数组 Array of data Lengths</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 原先的批量的长度为19，现在已经内部自动处理整合，目前的长度为任意和长度。
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadAsync(string[] address, ushort[] length)
	{
		S7AddressData[] addressResult = new S7AddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<S7AddressData> tmp = S7AddressData.ParseFrom(address[i], length[i]);
			if (!tmp.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(tmp);
			}
			addressResult[i] = tmp.Content;
		}
		return await ReadAsync(addressResult);
	}

	/// <summary>
	/// 读取西门子的地址数据信息，支持任意个数的数据读取
	/// Read Siemens address data information, support any number of data reading
	/// </summary>
	/// <param name="s7Addresses">西门子的数据地址 Siemens data address</param>
	/// <returns>返回的结果对象信息 -&gt; Whether to read the successful result object</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(S7AddressData[] s7Addresses)
	{
		List<byte> bytes = new List<byte>();
		List<S7AddressData[]> groups = SiemensS7Helper.ArraySplitByLength(s7Addresses, pdu_length);
		for (int i = 0; i < groups.Count; i++)
		{
			S7AddressData[] group = groups[i];
			if (group.Length == 1 && group[0].Length > pdu_length)
			{
				S7AddressData[] array = SiemensS7Helper.SplitS7Address(group[0], pdu_length);
				for (int j = 0; j < array.Length; j++)
				{
					OperateResult<byte[]> read = await ReadS7AddressDataAsync(new S7AddressData[1] { array[j] });
					if (!read.IsSuccess)
					{
						return read;
					}
					bytes.AddRange(read.Content);
				}
			}
			else
			{
				OperateResult<byte[]> read2 = await ReadS7AddressDataAsync(group);
				if (!read2.IsSuccess)
				{
					return read2;
				}
				bytes.AddRange(read2.Content);
			}
		}
		return OperateResult.CreateSuccessResult(bytes.ToArray());
	}

	/// <summary>
	/// 单次的读取，只能读取最多19个数组的长度，所以不再对外公开该方法
	/// </summary>
	/// <param name="s7Addresses">西门子的地址对象</param>
	/// <returns>返回的结果对象信息</returns>
	private async Task<OperateResult<byte[]>> ReadS7AddressDataAsync(S7AddressData[] s7Addresses)
	{
		OperateResult<byte[]> command = BuildReadCommand(s7Addresses, GetMessageId());
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content);
		if (!read.IsSuccess)
		{
			return read;
		}
		return AnalysisReadByte(s7Addresses, read.Content);
	}

	/// <summary>
	/// 基础的写入数据的操作支持
	/// Operational support for the underlying write data
	/// </summary>
	/// <param name="entireValue">完整的字节数据 -&gt; Full byte data</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	private async Task<OperateResult> WriteBaseAsync(byte[] entireValue)
	{
		return ByteTransformHelper.GetResultFromOther(await ReadFromCoreServerAsync(entireValue), AnalysisWrite);
	}

	/// <summary>
	/// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
	/// Writes data to the PLC data, in the address format I100,Q100,DB20.100,M100, in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="value">写入的原始数据 -&gt; Raw data written to</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom(address);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(analysis);
		}
		return await WriteAsync(analysis.Content, value);
	}

	private async Task<OperateResult> WriteAsync(S7AddressData address, byte[] value)
	{
		int length = value.Length;
		ushort alreadyFinished = 0;
		while (alreadyFinished < length)
		{
			ushort writeLength = (ushort)Math.Min(length - alreadyFinished, pdu_length);
			byte[] buffer = base.ByteTransform.TransByte(value, alreadyFinished, writeLength);
			OperateResult<byte[]> command = BuildWriteByteCommand(address, buffer, GetMessageId());
			if (!command.IsSuccess)
			{
				return command;
			}
			OperateResult write = await WriteBaseAsync(command.Content);
			if (!write.IsSuccess)
			{
				return write;
			}
			alreadyFinished += writeLength;
			address.AddressStart += writeLength * 8;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 使用离散的方式，同时写入多个数据块到不同的地址中去，但是支持的地址数量及写入数据的最大长度是有限制的，不能超过pdu长度限制。
	/// Using the discrete method, multiple blocks of data are written to different addresses at the same time, but the number of supported addresses and the maximum length of the data written are limited, and cannot exceed the PDU length limit.
	/// </summary>
	/// <param name="address">地址数组信息</param>
	/// <param name="data">原始数据列表</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteAsync(string[] address, List<byte[]> data)
	{
		S7AddressData[] addressResult = new S7AddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<S7AddressData> tmp = S7AddressData.ParseFrom(address[i], 1);
			if (!tmp.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(tmp);
			}
			addressResult[i] = tmp.Content;
		}
		OperateResult<byte[]> command = BuildWriteByteCommand(addressResult, data, GetMessageId());
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content);
		if (!read.IsSuccess)
		{
			return read;
		}
		return AnalysisWrite(read.Content);
	}

	[HslMqttApi("ReadBool", "")]
	/// <summary>
	/// 读取指定地址的bool数据，地址格式为I100，M100，Q100，DB20.100
	/// reads bool data for the specified address in the format I100，M100，Q100，DB20.100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
	/// </remarks>
	public override OperateResult<bool> ReadBool(string address)
	{
		return ByteTransformHelper.GetResultFromBytes(ReadBitFromPLC(address), (byte[] m) => m[0] != 0);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 读取指定地址的bool数组，地址格式为I100，M100，Q100，DB20.100
	/// reads bool array data for the specified address in the format I100，M100，Q100，DB20.100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		HslHelper.CalculateStartBitIndexAndLength(operateResult.Content.AddressStart, length, out var newStart, out var byteLength, out var offset);
		operateResult.Content.AddressStart = newStart;
		operateResult.Content.Length = byteLength;
		OperateResult<byte[]> operateResult2 = Read(new S7AddressData[1] { operateResult.Content });
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.ToBoolArray().SelectMiddle(offset, length));
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0"
	/// Write a bit of PLC, for example  "M100.6",  "I100.7",  "Q100.0",  "DB20.100.0", if only write  "M100" defaults to  "M100.0"
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;M100.6&quot;,  &quot;I100.7&quot;,  &quot;Q100.0&quot;,  &quot;DB20.100.0&quot; -&gt; Start address, format  &quot;M100.6&quot;,  &quot;I100.7&quot;,  &quot;Q100.0&quot;,  &quot;DB20.100.0&quot;</param>
	/// <param name="value">写入的数据，True或是False -&gt; Writes the data, either True or False</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override OperateResult Write(string address, bool value)
	{
		OperateResult<byte[]> operateResult = BuildWriteBitCommand(address, value, GetMessageId());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return WriteBase(operateResult.Content);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// [警告] 向PLC中写入bool数组，比如你写入M100,那么data[0]对应M100.0，写入的长度应该小于1600位
	/// [Warn] Write the bool array to the PLC, for example, if you write M100, then data[0] corresponds to M100.0,
	/// The length of the write should be less than 1600 bits
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="values">要写入的bool数组，长度为8的倍数 -&gt; The bool array to write, a multiple of 8 in length</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	/// <remarks>
	/// 批量写入bool数组存在一定的风险，举例写入M100.5的值 [true,false,true,true,false,true]，会读取M100-M101的byte[]，然后修改中间的位，再写入回去，
	/// 如果读取之后写入之前，PLC修改了其他位，则会影响其他的位的数据，请谨慎使用。
	/// There is a certain risk in batch writing bool arrays. For example, writing the value of M100.5 [true,false,true,true,false,true],
	/// will read the byte[] of M100-M101, then modify the middle bit, and then Write back.
	/// If the PLC modifies other bits after reading and before writing, it will affect the data of other bits. Please use it with caution.
	/// </remarks>
	public override OperateResult Write(string address, bool[] values)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		HslHelper.CalculateStartBitIndexAndLength(operateResult.Content.AddressStart, (ushort)values.Length, out var newStart, out var byteLength, out var offset);
		operateResult.Content.AddressStart = newStart;
		operateResult.Content.Length = byteLength;
		OperateResult<byte[]> operateResult2 = Read(new S7AddressData[1] { operateResult.Content });
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		bool[] array = operateResult2.Content.ToBoolArray();
		Array.Copy(values, 0, array, offset, values.Length);
		return Write(operateResult.Content, SoftBasic.BoolArrayToByte(array));
	}

	/// <summary>
	/// 读取指定地址的bool数据，地址格式为I100，M100，Q100，DB20.100
	/// reads bool data for the specified address in the format I100，M100，Q100，DB20.100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
	/// </remarks>
	public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadBitFromPLCAsync(address), (byte[] m) => m[0] != 0);
	}

	/// <summary>
	/// 读取指定地址的bool数组，地址格式为I100，M100，Q100，DB20.100
	/// reads bool array data for the specified address in the format I100，M100，Q100，DB20.100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
	/// </remarks>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom(address);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(analysis);
		}
		HslHelper.CalculateStartBitIndexAndLength(analysis.Content.AddressStart, length, out var newStart, out var byteLength, out var offset);
		analysis.Content.AddressStart = newStart;
		analysis.Content.Length = byteLength;
		OperateResult<byte[]> read = await ReadAsync(new S7AddressData[1] { analysis.Content });
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.ToBoolArray().SelectMiddle(offset, length));
	}

	/// <summary>
	/// 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0"
	/// Write a bit of PLC, for example  "M100.6",  "I100.7",  "Q100.0",  "DB20.100.0", if only write  "M100" defaults to  "M100.0"
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;M100.6&quot;,  &quot;I100.7&quot;,  &quot;Q100.0&quot;,  &quot;DB20.100.0&quot; -&gt; Start address, format  &quot;M100.6&quot;,  &quot;I100.7&quot;,  &quot;Q100.0&quot;,  &quot;DB20.100.0&quot;</param>
	/// <param name="value">写入的数据，True或是False -&gt; Writes the data, either True or False</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		OperateResult<byte[]> command = BuildWriteBitCommand(address, value, GetMessageId());
		if (!command.IsSuccess)
		{
			return command;
		}
		return await WriteBaseAsync(command.Content);
	}

	/// <summary>
	/// [警告] 向PLC中写入bool数组，比如你写入M100,那么data[0]对应M100.0，写入的长度应该小于1600位
	/// [Warn] Write the bool array to the PLC, for example, if you write M100, then data[0] corresponds to M100.0,
	/// The length of the write should be less than 1600 bits
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="values">要写入的bool数组，长度为8的倍数 -&gt; The bool array to write, a multiple of 8 in length</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	/// <remarks>
	/// 批量写入bool数组存在一定的风险，举例写入M100.5的值 [true,false,true,true,false,true]，会读取M100-M101的byte[]，然后修改中间的位，再写入回去，
	/// 如果读取之后写入之前，PLC修改了其他位，则会影响其他的位的数据，请谨慎使用。
	/// There is a certain risk in batch writing bool arrays. For example, writing the value of M100.5 [true,false,true,true,false,true],
	/// will read the byte[] of M100-M101, then modify the middle bit, and then Write back.
	/// If the PLC modifies other bits after reading and before writing, it will affect the data of other bits. Please use it with caution.
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom(address);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(analysis);
		}
		HslHelper.CalculateStartBitIndexAndLength(analysis.Content.AddressStart, (ushort)values.Length, out var newStart, out var byteLength, out var offset);
		analysis.Content.AddressStart = newStart;
		analysis.Content.Length = byteLength;
		OperateResult<byte[]> read = await ReadAsync(new S7AddressData[1] { analysis.Content });
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		bool[] boolArray = read.Content.ToBoolArray();
		Array.Copy(values, 0, boolArray, offset, values.Length);
		return await WriteAsync(analysis.Content, SoftBasic.BoolArrayToByte(boolArray));
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 读取指定地址的byte数据，地址格式I100，M100，Q100，DB20.100
	/// Reads the byte data of the specified address, the address format I100,Q100,DB20.100,M100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 向PLC中写入byte数据，返回值说明
	/// Write byte data to the PLC, return value description
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">byte数据 -&gt; Byte data</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	/// <summary>
	/// 读取指定地址的byte数据，地址格式I100，M100，Q100，DB20.100
	/// Reads the byte data of the specified address, the address format I100,Q100,DB20.100,M100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	/// <summary>
	/// 向PLC中写入byte数据，返回值说明
	/// Write byte data to the PLC, return value description
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">byte数据 -&gt; Byte data</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		return await WriteAsync(address, new byte[1] { value });
	}

	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		return SiemensS7Helper.Write(this, CurrentPlc, address, value, encoding);
	}

	[HslMqttApi(ApiTopic = "WriteWString", Description = "写入unicode编码的字符串，支持中文")]
	/// <summary>
	/// 使用双字节编码的方式，将字符串以 Unicode 编码写入到PLC的地址里，可以使用中文。
	/// Use the double-byte encoding method to write the character string to the address of the PLC in Unicode encoding. Chinese can be used.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">字符串的值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public OperateResult WriteWString(string address, string value)
	{
		return SiemensS7Helper.WriteWString(this, CurrentPlc, address, value);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		return (length == 0) ? ReadString(address, encoding) : base.ReadString(address, length, encoding);
	}

	[HslMqttApi("ReadS7String", "读取S7格式的字符串")]
	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public OperateResult<string> ReadString(string address)
	{
		return ReadString(address, Encoding.ASCII);
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public OperateResult<string> ReadString(string address, Encoding encoding)
	{
		return SiemensS7Helper.ReadString(this, CurrentPlc, address, encoding);
	}

	[HslMqttApi("ReadWString", "读取S7格式的双字节字符串")]
	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	public OperateResult<string> ReadWString(string address)
	{
		return SiemensS7Helper.ReadWString(this, CurrentPlc, address);
	}

	/// <summary>
	/// 将指定的字符串写入到西门子PLC里面去，将自动添加字符串长度信息，方便PLC识别字符串的内容。
	/// Write the specified string into Siemens PLC, and the string length information will be automatically added, which is convenient for PLC to identify the content of the string.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="value">写入的字符串值</param>
	/// <param name="encoding">编码信息</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		return await SiemensS7Helper.WriteAsync(this, CurrentPlc, address, value, encoding);
	}

	/// <summary>
	/// 使用双字节编码的方式，将字符串以 Unicode 编码写入到PLC的地址里，可以使用中文。
	/// Use the double-byte encoding method to write the character string to the address of the PLC in Unicode encoding. Chinese can be used.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">字符串的值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public async Task<OperateResult> WriteWStringAsync(string address, string value)
	{
		return await SiemensS7Helper.WriteWStringAsync(this, CurrentPlc, address, value);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		return (length != 0) ? (await base.ReadStringAsync(address, length, encoding)) : (await ReadStringAsync(address, encoding));
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		return await ReadStringAsync(address, Encoding.ASCII);
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public async Task<OperateResult<string>> ReadStringAsync(string address, Encoding encoding)
	{
		return await SiemensS7Helper.ReadStringAsync(this, CurrentPlc, address, encoding);
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	public async Task<OperateResult<string>> ReadWStringAsync(string address)
	{
		return await SiemensS7Helper.ReadWStringAsync(this, CurrentPlc, address);
	}

	[HslMqttApi("ReadDateTime", "读取PLC的时间格式的数据，这个格式是s7格式的一种")]
	/// <summary>
	/// 从PLC中读取时间格式的数据
	/// Read time format data from PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <returns>时间对象</returns>
	public OperateResult<DateTime> ReadDateTime(string address)
	{
		return SiemensDateTime.FromByteArray(Read(address, 8));
	}

	[HslMqttApi("ReadDTLDataTime", "读取PLC的时间格式的数据，这个格式是s7的DTL格式")]
	/// <summary>
	/// 从PLC中读取DTL时间格式的数据
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>时间对象</returns>
	public OperateResult<DateTime> ReadDTLDataTime(string address)
	{
		return SiemensDateTime.GetDTLTime(base.ByteTransform, Read(address, 12));
	}

	/// <summary>
	/// 从PLC中读取日期格式的数据
	/// Read data in date format from PLC
	/// </summary>
	/// <param name="address">PLC的地址</param>
	/// <returns>日期对象</returns>
	public OperateResult<DateTime> ReadDate(string address)
	{
		return ReadUInt16(address).Then((ushort m) => OperateResult.CreateSuccessResult(new DateTime(1990, 1, 1).AddDays((int)m)));
	}

	[HslMqttApi("WriteDateTime", "写入PLC的时间格式的数据，这个格式是s7格式的一种")]
	/// <summary>
	/// 向PLC中写入时间格式的数据
	/// Writes data in time format to the PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="dateTime">时间</param>
	/// <returns>是否写入成功</returns>
	public OperateResult Write(string address, DateTime dateTime)
	{
		return Write(address, SiemensDateTime.ToByteArray(dateTime));
	}

	[HslMqttApi("WriteDTLTime", "写入PLC的时间格式的数据，这个格式是s7格式的DTL格式")]
	/// <summary>
	/// 向PLC中写入DTL格式的时间数据信息
	/// </summary>
	/// <param name="address">写入的地址信息</param>
	/// <param name="dateTime">时间</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteDTLTime(string address, DateTime dateTime)
	{
		return Write(address, SiemensDateTime.GetBytesFromDTLTime(base.ByteTransform, dateTime));
	}

	/// <summary>
	/// 向PLC中写入日期格式的数据，日期格式里只有年，月，日
	/// Write data in date format to PLC, only year, month, day in date format
	/// </summary>
	/// <param name="address">等待写入的PLC地址</param>
	/// <param name="dateTime">等待写入的日期</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteDate(string address, DateTime dateTime)
	{
		return Write(address, Convert.ToUInt16((dateTime - new DateTime(1990, 1, 1)).TotalDays));
	}

	/// <summary>
	/// 从PLC中读取时间格式的数据
	/// Read time format data from PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <returns>时间对象</returns>
	public async Task<OperateResult<DateTime>> ReadDateTimeAsync(string address)
	{
		return SiemensDateTime.FromByteArray(await ReadAsync(address, 8).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// 从PLC中读取DTL时间格式的数据
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>时间对象</returns>
	public async Task<OperateResult<DateTime>> ReadDTLDataTimeAsync(string address)
	{
		IByteTransform byteTransform = base.ByteTransform;
		return SiemensDateTime.GetDTLTime(byteTransform, await ReadAsync(address, 12).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// 从PLC中读取日期格式的数据
	/// Read data in date format from PLC
	/// </summary>
	/// <param name="address">PLC的地址</param>
	/// <returns>日期对象</returns>
	public async Task<OperateResult<DateTime>> ReadDateAsync(string address)
	{
		return (await ReadUInt16Async(address)).Then((ushort m) => OperateResult.CreateSuccessResult(new DateTime(1990, 1, 1).AddDays((int)m)));
	}

	/// <summary>
	/// 向PLC中写入时间格式的数据
	/// Writes data in time format to the PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="dateTime">时间</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteAsync(string address, DateTime dateTime)
	{
		return await WriteAsync(address, SiemensDateTime.ToByteArray(dateTime));
	}

	/// <summary>
	/// 向PLC中写入DTL格式的时间数据信息
	/// </summary>
	/// <param name="address">写入的地址信息</param>
	/// <param name="dateTime">时间</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteDTLTimeAsync(string address, DateTime dateTime)
	{
		return await WriteAsync(address, SiemensDateTime.GetBytesFromDTLTime(base.ByteTransform, dateTime));
	}

	/// <summary>
	/// 向PLC中写入日期格式的数据，日期格式里只有年，月，日
	/// Write data in date format to PLC, only year, month, day in date format
	/// </summary>
	/// <param name="address">等待写入的PLC地址</param>
	/// <param name="dateTime">等待写入的日期</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteDateAsync(string address, DateTime dateTime)
	{
		return await WriteAsync(address, Convert.ToUInt16((dateTime - new DateTime(1990, 1, 1)).TotalDays));
	}

	/// <summary>
	/// 强制输出一个位到指定的地址，针对PLC类型为 200smart 时有效
	/// Force output one bit to the specified address, valid for PLC type 200smart
	/// </summary>
	/// <param name="address">西门子的地址信息，例如 I0.0, Q1.0, M2.0</param>
	/// <param name="value">输出值 false=断开, true=闭合</param>
	/// <returns>是否强制输出成功</returns>
	/// <remarks>
	/// 测试型号: S7-200 smart CPU SR30
	/// </remarks>
	public OperateResult ForceBool(string address, bool value)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		byte[] buffer = new byte[47]
		{
			3, 0, 0, 47, 2, 240, 128, 50, 7, 0,
			0, 121, 249, 0, 12, 0, 18, 0, 1, 18,
			8, 18, 72, 11, 0, 0, 0, 0, 0, 255,
			9, 0, 14, 0, 1, 16, 1, 0, 1, 0,
			0, 130, 0, 0, 0, 1, 0
		};
		buffer[39] = (byte)(operateResult.Content.DbBlock / 256);
		buffer[40] = (byte)(operateResult.Content.DbBlock % 256);
		buffer[41] = operateResult.Content.DataCode;
		buffer[42] = (byte)(operateResult.Content.AddressStart / 256 / 256 % 256);
		buffer[43] = (byte)(operateResult.Content.AddressStart / 256 % 256);
		buffer[44] = (byte)(operateResult.Content.AddressStart % 256);
		buffer[45] = (byte)(value ? 1u : 0u);
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(buffer);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 取消所有强制输出，针对PLC类型为 200smart 时有效
	/// Cancel all forced outputs, effective for PLC type 200smart
	/// </summary>
	/// <returns>是否取消成功</returns>
	public OperateResult CancelAllForce()
	{
		byte[] send = new byte[35]
		{
			3, 0, 0, 35, 2, 240, 128, 50, 7, 0,
			0, 166, 209, 0, 12, 0, 6, 0, 1, 18,
			8, 18, 72, 11, 0, 0, 0, 0, 0, 255,
			9, 0, 2, 2, 0
		};
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content.Length < 33)
		{
			return new OperateResult("Receive error");
		}
		if (operateResult.Content[11] != 166 || operateResult.Content[12] != 209)
		{
			return new OperateResult("CancelAllForceOut Fail!");
		}
		return OperateResult.CreateSuccessResult();
	}

	private int GetMessageId()
	{
		return (int)incrementCount.GetCurrentValue();
	}

	public override string ToString()
	{
		return $"SiemensS7Net {CurrentPlc}[{IpAddress}:{Port}]";
	}

	/// <summary>
	/// A general method for generating a command header to read a Word data
	/// </summary>
	/// <param name="s7Addresses">siemens address</param>
	/// <param name="msgId">message id informaion</param>
	/// <returns>Message containing the result object</returns>
	public static OperateResult<byte[]> BuildReadCommand(S7AddressData[] s7Addresses, int msgId)
	{
		if (s7Addresses == null)
		{
			throw new NullReferenceException("s7Addresses");
		}
		if (s7Addresses.Length > 19)
		{
			throw new Exception(StringResources.Language.SiemensReadLengthCannotLargerThan19);
		}
		int count = s7Addresses.Length;
		byte[] buffer = new byte[19 + count * 12];
		buffer[0] = 3;
		buffer[1] = 0;
		buffer[2] = (byte)(buffer.Length / 256);
		buffer[3] = (byte)(buffer.Length % 256);
		buffer[4] = 2;
		buffer[5] = 240;
		buffer[6] = 128;
		buffer[7] = 50;
		buffer[8] = 1;
		buffer[9] = 0;
		buffer[10] = 0;
		buffer[11] = BitConverter.GetBytes(msgId)[1];
		buffer[12] = BitConverter.GetBytes(msgId)[0];
		buffer[13] = (byte)((buffer.Length - 17) / 256);
		buffer[14] = (byte)((buffer.Length - 17) % 256);
		buffer[15] = 0;
		buffer[16] = 0;
		buffer[17] = 4;
		buffer[18] = (byte)count;
		for (int i = 0; i < count; i++)
		{
			buffer[19 + i * 12] = 18;
			buffer[20 + i * 12] = 10;
			buffer[21 + i * 12] = 16;
			if (s7Addresses[i].DataCode == 30 || s7Addresses[i].DataCode == 31)
			{
				buffer[22 + i * 12] = s7Addresses[i].DataCode;
				buffer[23 + i * 12] = (byte)(s7Addresses[i].Length / 2 / 256);
				buffer[24 + i * 12] = (byte)(s7Addresses[i].Length / 2 % 256);
			}
			else if ((s7Addresses[i].DataCode == 6) | (s7Addresses[i].DataCode == 7))
			{
				buffer[22 + i * 12] = 4;
				buffer[23 + i * 12] = (byte)(s7Addresses[i].Length / 2 / 256);
				buffer[24 + i * 12] = (byte)(s7Addresses[i].Length / 2 % 256);
			}
			else
			{
				buffer[22 + i * 12] = 2;
				buffer[23 + i * 12] = (byte)(s7Addresses[i].Length / 256);
				buffer[24 + i * 12] = (byte)(s7Addresses[i].Length % 256);
			}
			buffer[25 + i * 12] = (byte)(s7Addresses[i].DbBlock / 256);
			buffer[26 + i * 12] = (byte)(s7Addresses[i].DbBlock % 256);
			buffer[27 + i * 12] = s7Addresses[i].DataCode;
			buffer[28 + i * 12] = (byte)(s7Addresses[i].AddressStart / 256 / 256 % 256);
			buffer[29 + i * 12] = (byte)(s7Addresses[i].AddressStart / 256 % 256);
			buffer[30 + i * 12] = (byte)(s7Addresses[i].AddressStart % 256);
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 生成一个位读取数据指令头的通用方法 ->
	/// A general method for generating a bit-read-Data instruction header
	/// </summary>
	/// <param name="address">起始地址，例如M100.0，I0.1，Q0.1，DB2.100.2 -&gt; Start address, such as M100.0,I0.1,Q0.1,DB2.100.2</param>
	/// <param name="msgId">message id informaion</param>
	/// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
	public static OperateResult<byte[]> BuildBitReadCommand(string address, int msgId)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[31];
		buffer[0] = 3;
		buffer[1] = 0;
		buffer[2] = (byte)(buffer.Length / 256);
		buffer[3] = (byte)(buffer.Length % 256);
		buffer[4] = 2;
		buffer[5] = 240;
		buffer[6] = 128;
		buffer[7] = 50;
		buffer[8] = 1;
		buffer[9] = 0;
		buffer[10] = 0;
		buffer[11] = BitConverter.GetBytes(msgId)[1];
		buffer[12] = BitConverter.GetBytes(msgId)[0];
		buffer[13] = (byte)((buffer.Length - 17) / 256);
		buffer[14] = (byte)((buffer.Length - 17) % 256);
		buffer[15] = 0;
		buffer[16] = 0;
		buffer[17] = 4;
		buffer[18] = 1;
		buffer[19] = 18;
		buffer[20] = 10;
		buffer[21] = 16;
		buffer[22] = 1;
		buffer[23] = 0;
		buffer[24] = 1;
		buffer[25] = (byte)(operateResult.Content.DbBlock / 256);
		buffer[26] = (byte)(operateResult.Content.DbBlock % 256);
		buffer[27] = operateResult.Content.DataCode;
		buffer[28] = (byte)(operateResult.Content.AddressStart / 256 / 256 % 256);
		buffer[29] = (byte)(operateResult.Content.AddressStart / 256 % 256);
		buffer[30] = (byte)(operateResult.Content.AddressStart % 256);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
	/// </summary>
	/// <param name="s7Address">起始地址，示例M100,I100,Q100,DB1.100 -&gt; Start Address, example M100,I100,Q100,DB1.100</param>
	/// <param name="data">原始的字节数据 -&gt; Raw byte data</param>
	/// <param name="msgId">message id informaion</param>
	/// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
	public static OperateResult<byte[]> BuildWriteByteCommand(S7AddressData s7Address, byte[] data, int msgId)
	{
		return BuildWriteByteCommand(new S7AddressData[1] { s7Address }, new List<byte[]> { data }, msgId);
	}

	private static void WriteS7AddressToStream(MemoryStream ms, S7AddressData add, byte writeType, int dataLen)
	{
		ms.WriteByte(18);
		ms.WriteByte(10);
		ms.WriteByte(16);
		if (add.DataCode == 6 || add.DataCode == 7)
		{
			ms.WriteByte(4);
			ms.WriteByte(BitConverter.GetBytes(dataLen / 2)[1]);
			ms.WriteByte(BitConverter.GetBytes(dataLen / 2)[0]);
		}
		else
		{
			ms.WriteByte(writeType);
			ms.WriteByte(BitConverter.GetBytes(dataLen)[1]);
			ms.WriteByte(BitConverter.GetBytes(dataLen)[0]);
		}
		ms.WriteByte(BitConverter.GetBytes(add.DbBlock)[1]);
		ms.WriteByte(BitConverter.GetBytes(add.DbBlock)[0]);
		ms.WriteByte(add.DataCode);
		ms.WriteByte(BitConverter.GetBytes(add.AddressStart)[2]);
		ms.WriteByte(BitConverter.GetBytes(add.AddressStart)[1]);
		ms.WriteByte(BitConverter.GetBytes(add.AddressStart)[0]);
	}

	/// <summary>
	/// 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
	/// </summary>
	/// <param name="s7Address">起始地址，示例M100,I100,Q100,DB1.100 -&gt; Start Address, example M100,I100,Q100,DB1.100</param>
	/// <param name="data">原始的字节数据 -&gt; Raw byte data</param>
	/// <param name="msgId">message id informaion</param>
	/// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
	public static OperateResult<byte[]> BuildWriteByteCommand(S7AddressData[] s7Address, List<byte[]> data, int msgId)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(new byte[7] { 3, 0, 0, 0, 2, 240, 128 });
		memoryStream.WriteByte(50);
		memoryStream.WriteByte(1);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(BitConverter.GetBytes(msgId)[1]);
		memoryStream.WriteByte(BitConverter.GetBytes(msgId)[0]);
		memoryStream.WriteByte(BitConverter.GetBytes(s7Address.Length * 12 + 2)[1]);
		memoryStream.WriteByte(BitConverter.GetBytes(s7Address.Length * 12 + 2)[0]);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(5);
		memoryStream.WriteByte((byte)s7Address.Length);
		for (int i = 0; i < s7Address.Length; i++)
		{
			WriteS7AddressToStream(memoryStream, s7Address[i], 2, (data[i] != null) ? data[i].Length : 0);
		}
		int count = (int)memoryStream.Length;
		for (int j = 0; j < data.Count; j++)
		{
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(4);
			if (data[j] != null)
			{
				memoryStream.WriteByte(BitConverter.GetBytes(data[j].Length * 8)[1]);
				memoryStream.WriteByte(BitConverter.GetBytes(data[j].Length * 8)[0]);
				memoryStream.Write(data[j]);
				if (j < data.Count - 1 && data[j].Length % 2 == 1)
				{
					memoryStream.WriteByte(0);
				}
			}
			else
			{
				memoryStream.WriteByte(0);
				memoryStream.WriteByte(0);
			}
		}
		byte[] array = memoryStream.ToArray();
		array[2] = (byte)(array.Length / 256);
		array[3] = (byte)(array.Length % 256);
		array[15] = BitConverter.GetBytes(array.Length - count)[1];
		array[16] = BitConverter.GetBytes(array.Length - count)[0];
		return OperateResult.CreateSuccessResult(array);
	}

	/// <summary>
	/// 生成一个写入位数据的指令 -> Generate an instruction to write bit data
	/// </summary>
	/// <param name="address">起始地址，示例M100,I100,Q100,DB1.100 -&gt; Start Address, example M100,I100,Q100,DB1.100</param>
	/// <param name="data">是否通断 -&gt; Power on or off</param>
	/// <param name="msgId">message id informaion</param>
	/// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
	public static OperateResult<byte[]> BuildWriteBitCommand(string address, bool data, int msgId)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[1] { (byte)(data ? 1 : 0) };
		byte[] buffer2 = new byte[35 + buffer.Length];
		buffer2[0] = 3;
		buffer2[1] = 0;
		buffer2[2] = (byte)((35 + buffer.Length) / 256);
		buffer2[3] = (byte)((35 + buffer.Length) % 256);
		buffer2[4] = 2;
		buffer2[5] = 240;
		buffer2[6] = 128;
		buffer2[7] = 50;
		buffer2[8] = 1;
		buffer2[9] = 0;
		buffer2[10] = 0;
		buffer2[11] = BitConverter.GetBytes(msgId)[1];
		buffer2[12] = BitConverter.GetBytes(msgId)[0];
		buffer2[13] = 0;
		buffer2[14] = 14;
		buffer2[15] = (byte)((4 + buffer.Length) / 256);
		buffer2[16] = (byte)((4 + buffer.Length) % 256);
		buffer2[17] = 5;
		buffer2[18] = 1;
		buffer2[19] = 18;
		buffer2[20] = 10;
		buffer2[21] = 16;
		buffer2[22] = 1;
		buffer2[23] = (byte)(buffer.Length / 256);
		buffer2[24] = (byte)(buffer.Length % 256);
		buffer2[25] = (byte)(operateResult.Content.DbBlock / 256);
		buffer2[26] = (byte)(operateResult.Content.DbBlock % 256);
		buffer2[27] = operateResult.Content.DataCode;
		buffer2[28] = (byte)(operateResult.Content.AddressStart / 256 / 256);
		buffer2[29] = (byte)(operateResult.Content.AddressStart / 256);
		buffer2[30] = (byte)(operateResult.Content.AddressStart % 256);
		if (operateResult.Content.DataCode == 28)
		{
			buffer2[31] = 0;
			buffer2[32] = 9;
		}
		else
		{
			buffer2[31] = 0;
			buffer2[32] = 3;
		}
		buffer2[33] = (byte)(buffer.Length / 256);
		buffer2[34] = (byte)(buffer.Length % 256);
		buffer.CopyTo(buffer2, 35);
		return OperateResult.CreateSuccessResult(buffer2);
	}

	private static OperateResult<byte[]> AnalysisReadByte(S7AddressData[] s7Addresses, byte[] content)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < s7Addresses.Length; i++)
			{
				num = ((s7Addresses[i].DataCode != 31 && s7Addresses[i].DataCode != 30) ? (num + s7Addresses[i].Length) : (num + s7Addresses[i].Length * 2));
			}
			if (content.Length >= 21 && content[20] == s7Addresses.Length)
			{
				byte[] buffer = new byte[num];
				int num2 = 0;
				int num3 = 0;
				for (int j = 21; j < content.Length; j++)
				{
					if (j + 1 >= content.Length)
					{
						continue;
					}
					if (content[j] == byte.MaxValue && content[j + 1] == 4)
					{
						Array.Copy(content, j + 4, buffer, num3, s7Addresses[num2].Length);
						j += s7Addresses[num2].Length + 3;
						num3 += s7Addresses[num2].Length;
						num2++;
					}
					else if (content[j] == byte.MaxValue && content[j + 1] == 9)
					{
						int num4 = content[j + 2] * 256 + content[j + 3];
						if (num4 % 3 == 0)
						{
							for (int k = 0; k < num4 / 3; k++)
							{
								Array.Copy(content, j + 5 + 3 * k, buffer, num3, 2);
								num3 += 2;
							}
						}
						else
						{
							for (int l = 0; l < num4 / 5; l++)
							{
								Array.Copy(content, j + 7 + 5 * l, buffer, num3, 2);
								num3 += 2;
							}
						}
						j += num4 + 4;
						num2++;
					}
					else
					{
						if (content[j] == 5 && content[j + 1] == 0)
						{
							return new OperateResult<byte[]>(content[j], StringResources.Language.SiemensReadLengthOverPlcAssign);
						}
						if (content[j] == 6 && content[j + 1] == 0)
						{
							return new OperateResult<byte[]>(content[j], StringResources.Language.SiemensError0006);
						}
						if (content[j] == 10 && content[j + 1] == 0)
						{
							return new OperateResult<byte[]>(content[j], StringResources.Language.SiemensError000A);
						}
					}
				}
				return OperateResult.CreateSuccessResult(buffer);
			}
			return new OperateResult<byte[]>(StringResources.Language.SiemensDataLengthCheckFailed + " Msg:" + SoftBasic.ByteToHexString(content, ' '));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("AnalysisReadByte failed: " + ex.Message + Environment.NewLine + " Msg:" + SoftBasic.ByteToHexString(content, ' '));
		}
	}

	private static OperateResult AnalysisWrite(byte[] content)
	{
		try
		{
			if (content != null && content.Length >= 22)
			{
				int num = content[20];
				for (int i = 0; i < num; i++)
				{
					byte b = content[21 + i];
					switch (b)
					{
					case 5:
						return new OperateResult(b, StringResources.Language.SiemensReadLengthOverPlcAssign);
					case 6:
						return new OperateResult(b, StringResources.Language.SiemensError0006);
					case 10:
						return new OperateResult(b, StringResources.Language.SiemensError000A);
					default:
						return new OperateResult(b, StringResources.Language.SiemensWriteError + b + " Msg:" + SoftBasic.ByteToHexString(content, ' '));
					case byte.MaxValue:
						break;
					}
				}
				return OperateResult.CreateSuccessResult();
			}
			return new OperateResult(StringResources.Language.UnknownError + " Msg:" + SoftBasic.ByteToHexString(content, ' '));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("AnalysisWrite failed: " + ex.Message + Environment.NewLine + " Msg:" + SoftBasic.ByteToHexString(content, ' '));
		}
	}
}
