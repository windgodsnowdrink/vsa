// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.IO;
using System.Linq;
using System.Net;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙的虚拟服务器，支持DM区，CIO区，Work区，Hold区，Auxiliary区，可以方便的进行测试
/// Omron's virtual server supports DM area, CIO area, Work area, Hold area, and Auxiliary area, which can be easily tested
/// </summary>
public class OmronFinsServer : DeviceServer
{
	protected SoftBuffer dBuffer;

	protected SoftBuffer cioBuffer;

	protected SoftBuffer wBuffer;

	protected SoftBuffer hBuffer;

	protected SoftBuffer arBuffer;

	protected SoftBuffer emBuffer;

	protected SoftBuffer cfBuffer;

	protected SoftBuffer irBuffer;

	protected SoftBuffer drBuffer;

	protected bool connectionInitialization = true;

	protected const int DataPoolLength = 65536;

	/// <summary>
	/// 获取或设置数据解析的格式，可选ABCD, BADC，CDAB，DCBA格式，对int,uint,float,double,long,ulong类型有作用
	/// Get or set the format of the data analysis, optional ABCD, BADC, CDAB, DCBA format, effective for int, uint, float, double, long, ulong type
	/// </summary>
	public DataFormat DataFormat
	{
		get
		{
			return base.ByteTransform.DataFormat;
		}
		set
		{
			base.ByteTransform.DataFormat = value;
		}
	}

	/// <summary>
	/// 是否在返回的报文中复制SID信息，默认是True
	/// Indicates whether to copy SID information in the returned message, default is True
	/// </summary>
	public bool CopySID { get; set; } = true;

	/// <summary>
	/// 是否分析通信报文信息，并将分析结果追加到报文日志里去
	/// Should we analyze the communication message information and add the analysis results to the message log?
	/// </summary>
	public bool AnalysisLogMessage { get; set; } = false;

	/// <summary>
	/// 实例化一个Fins协议的服务器
	/// Instantiate a Fins protocol server
	/// </summary>
	public OmronFinsServer()
	{
		dBuffer = new SoftBuffer(131072);
		cioBuffer = new SoftBuffer(131072);
		wBuffer = new SoftBuffer(131072);
		hBuffer = new SoftBuffer(131072);
		arBuffer = new SoftBuffer(131072);
		emBuffer = new SoftBuffer(131072);
		cfBuffer = new SoftBuffer(131072);
		irBuffer = new SoftBuffer(131072);
		drBuffer = new SoftBuffer(131072);
		dBuffer.IsBoolReverseByWord = true;
		cioBuffer.IsBoolReverseByWord = true;
		wBuffer.IsBoolReverseByWord = true;
		hBuffer.IsBoolReverseByWord = true;
		arBuffer.IsBoolReverseByWord = true;
		emBuffer.IsBoolReverseByWord = true;
		cfBuffer.IsBoolReverseByWord = true;
		irBuffer.IsBoolReverseByWord = true;
		drBuffer.IsBoolReverseByWord = true;
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
	}

	private OperateResult<SoftBuffer, OmronFinsAddress> GetWordAddressBuffer(string address)
	{
		OperateResult<OmronFinsAddress> operateResult = OmronFinsAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<SoftBuffer, OmronFinsAddress>(operateResult);
		}
		if (operateResult.Content.WordCode == OmronFinsDataType.DM.WordCode)
		{
			return OperateResult.CreateSuccessResult(dBuffer, operateResult.Content);
		}
		if (operateResult.Content.WordCode == OmronFinsDataType.CIO.WordCode)
		{
			return OperateResult.CreateSuccessResult(cioBuffer, operateResult.Content);
		}
		if (operateResult.Content.WordCode == OmronFinsDataType.WR.WordCode)
		{
			return OperateResult.CreateSuccessResult(wBuffer, operateResult.Content);
		}
		if (operateResult.Content.WordCode == OmronFinsDataType.HR.WordCode)
		{
			return OperateResult.CreateSuccessResult(hBuffer, operateResult.Content);
		}
		if (operateResult.Content.WordCode == OmronFinsDataType.AR.WordCode)
		{
			return OperateResult.CreateSuccessResult(arBuffer, operateResult.Content);
		}
		if (operateResult.Content.WordCode == 188)
		{
			return OperateResult.CreateSuccessResult(drBuffer, operateResult.Content);
		}
		if (operateResult.Content.WordCode == 220)
		{
			return OperateResult.CreateSuccessResult(irBuffer, operateResult.Content);
		}
		if (address.StartsWith("E", StringComparison.OrdinalIgnoreCase))
		{
			return OperateResult.CreateSuccessResult(emBuffer, operateResult.Content);
		}
		return new OperateResult<SoftBuffer, OmronFinsAddress>(StringResources.Language.NotSupportedDataType);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取长度的单位为字，地址格式为"D100","C100","W100","H100","A100"
	/// Read the desired data from the Omron PLC and return the read result. The unit of the read length is word. The address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<SoftBuffer, OmronFinsAddress> wordAddressBuffer = GetWordAddressBuffer(address);
		if (!wordAddressBuffer.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(wordAddressBuffer);
		}
		return OperateResult.CreateSuccessResult(wordAddressBuffer.Content1.GetBytes(wordAddressBuffer.Content2.AddressStart / 16 * 2, length * 2));
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 向PLC写入数据，数据格式为原始的字节类型，地址格式为"D100","C100","W100","H100","A100"
	/// Write data to PLC, the data format is the original byte type, and the address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">初始地址</param>
	/// <param name="value">原始的字节数据</param>
	/// <returns>结果</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<SoftBuffer, OmronFinsAddress> wordAddressBuffer = GetWordAddressBuffer(address);
		if (!wordAddressBuffer.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(wordAddressBuffer);
		}
		wordAddressBuffer.Content1.SetBytes(value, wordAddressBuffer.Content2.AddressStart / 16 * 2);
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 从欧姆龙PLC中批量读取位软元件，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Read bit devices in batches from Omron PLC with address format "D100.0", "C100.0", "W100.0", "H100.0", "A100.0"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<OmronFinsAddress> operateResult = OmronFinsAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		if (operateResult.Content.BitCode == OmronFinsDataType.DM.BitCode)
		{
			return OperateResult.CreateSuccessResult(dBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		if (operateResult.Content.BitCode == OmronFinsDataType.CIO.BitCode)
		{
			return OperateResult.CreateSuccessResult(cioBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		if (operateResult.Content.BitCode == OmronFinsDataType.WR.BitCode)
		{
			return OperateResult.CreateSuccessResult(wBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		if (operateResult.Content.BitCode == OmronFinsDataType.HR.BitCode)
		{
			return OperateResult.CreateSuccessResult(hBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		if (operateResult.Content.BitCode == OmronFinsDataType.AR.BitCode)
		{
			return OperateResult.CreateSuccessResult(arBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		if (operateResult.Content.BitCode == 7)
		{
			return OperateResult.CreateSuccessResult(cfBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		if (operateResult.Content.WordCode == 188)
		{
			return OperateResult.CreateSuccessResult(drBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		if (operateResult.Content.WordCode == 220)
		{
			return OperateResult.CreateSuccessResult(irBuffer.GetBool(operateResult.Content.AddressStart, length));
		}
		return OperateResult.CreateSuccessResult(emBuffer.GetBool(operateResult.Content.AddressStart, length));
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 向PLC中位软元件写入bool数组，返回是否写入成功，比如你写入D100,values[0]对应D100.0，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Write the bool array to the PLC's median device and return whether the write was successful. For example, if you write D100, values [0] corresponds to D100.0
	/// and the address format is "D100.0", "C100.0", "W100. 0 "," H100.0 "," A100.0 "
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">要写入的数据地址</param>
	/// <param name="values">要写入的实际数据，可以指定任意的长度</param>
	/// <returns>返回写入结果</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		OperateResult<OmronFinsAddress> operateResult = OmronFinsAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		if (operateResult.Content.BitCode == OmronFinsDataType.DM.BitCode)
		{
			dBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else if (operateResult.Content.BitCode == OmronFinsDataType.CIO.BitCode)
		{
			cioBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else if (operateResult.Content.BitCode == OmronFinsDataType.WR.BitCode)
		{
			wBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else if (operateResult.Content.BitCode == OmronFinsDataType.HR.BitCode)
		{
			hBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else if (operateResult.Content.BitCode == OmronFinsDataType.AR.BitCode)
		{
			arBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else if (operateResult.Content.BitCode == 7)
		{
			cfBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else if (operateResult.Content.WordCode == 188)
		{
			drBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else if (operateResult.Content.WordCode == 220)
		{
			irBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		else
		{
			emBuffer.SetBool(value, operateResult.Content.AddressStart);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		string text = base.GetLogTextFromBinary(session, content);
		if (AnalysisLogMessage)
		{
			string text2 = OmronFinsNetHelper.AnalysisAddressLogMessage(session, base.ByteTransform, content, 26);
			text += text2;
		}
		return text;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FinsMessage();
	}

	protected override OperateResult ThreadPoolLoginAfterClientCheck(PipeSession session, IPEndPoint endPoint)
	{
		if (connectionInitialization)
		{
			CommunicationPipe communication = session.Communication;
			OperateResult<byte[]> operateResult = communication.ReceiveMessage(GetNewNetMessage(), null, useActivePush: false, null, delegate(byte[] m)
			{
				LogRevcMessage(m, session);
			});
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			byte[] array = SoftBasic.HexStringToBytes("46 49 4E 53 00 00 00 10 00 00 00 01 00 00 00 00 00 00 00 01 00 00 00 02");
			LogSendMessage(array, session);
			OperateResult operateResult2 = communication.Send(array);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
		}
		return base.ThreadPoolLoginAfterClientCheck(session, endPoint);
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive[0] == 70 && receive[7] == 12 && receive.Length == 20)
		{
			return OperateResult.CreateSuccessResult(SoftBasic.HexStringToBytes("46 49 4E 53 00 00 00 10 00 00 00 01 00 00 00 00 00 00 00 01 00 00 00 02"));
		}
		byte[] array = ReadFromFinsCore(receive.RemoveBegin(26));
		if (receive != null && receive.Length > 25)
		{
			array[20] = receive[23];
			array[23] = receive[20];
			if (CopySID)
			{
				array[25] = receive[25];
			}
			else
			{
				array[25] = 0;
			}
		}
		return OperateResult.CreateSuccessResult(array);
	}

	/// <summary>
	/// 当收到mc协议的报文的时候应该触发的方法，允许继承重写，来实现自定义的返回，或是数据监听。
	/// The method that should be triggered when a message of the mc protocol is received is allowed to be inherited and rewritten to achieve a custom return or data monitoring.
	/// </summary>
	/// <param name="finsCore">mc报文</param>
	/// <returns>返回的报文信息</returns>
	protected virtual byte[] ReadFromFinsCore(byte[] finsCore)
	{
		if (finsCore.Length == 0)
		{
			return null;
		}
		if (finsCore[0] == 1 && finsCore[1] == 1)
		{
			byte[] array = ReadByCommand(finsCore);
			return PackCommand((array == null) ? 2 : 0, finsCore, array);
		}
		if (finsCore[0] == 1 && finsCore[1] == 2)
		{
			if (!base.EnableWrite)
			{
				return PackCommand(3, finsCore, null);
			}
			return PackCommand(0, finsCore, WriteByMessage(finsCore));
		}
		if (finsCore[0] == 1 && finsCore[1] == 4)
		{
			byte[] array2 = ReadByMultiCommand(finsCore);
			return PackCommand((array2 == null) ? 2 : 0, finsCore, array2);
		}
		if (finsCore[0] == 4 && finsCore[1] == 1)
		{
			return PackCommand(0, finsCore, null);
		}
		if (finsCore[0] == 4 && finsCore[1] == 2)
		{
			return PackCommand(0, finsCore, null);
		}
		if (finsCore[0] == 5 && finsCore[1] == 1)
		{
			return PackCommand(0, finsCore, "43 4A 32 4D 2D 43 50 55 33 31 20 20 20 20 20 20 20 20 20 20 30 32 2E 30 31 00 00 00 00 00 30 32 2E 31 30 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 80 01 80 01 80 01 80 00 00 00 00 00 00 00 00 02 01 00 00 0A 17 80 00 08 01 00 00 00 00 00".ToHexBytes());
		}
		if (finsCore[0] == 6 && finsCore[1] == 1)
		{
			return PackCommand(0, finsCore, "05 02 00 00 00 00 00 00 00 00 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20".ToHexBytes());
		}
		if (finsCore[0] == 7 && finsCore[1] == 1)
		{
			return PackCommand(0, finsCore, (DateTime.Now.ToString("yy-MM-dd-HH-mm-ss").Replace("-", "") + ((int)DateTime.Now.DayOfWeek).ToString("D2")).ToHexBytes());
		}
		return PackCommand(3, finsCore, null);
	}

	/// <summary>
	/// 将核心报文打包的方法，追加报文头
	/// The method of packing the core message, adding the message header
	/// </summary>
	/// <param name="status">错误码</param>
	/// <param name="finsCore">Fins的核心报文</param>
	/// <param name="data">核心的内容</param>
	/// <returns>完整的报文信息</returns>
	protected virtual byte[] PackCommand(int status, byte[] finsCore, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		byte[] buffer = new byte[30 + data.Length];
		SoftBasic.HexStringToBytes("46 49 4E 53 00 00 00 0000 00 00 02 00 00 00 00C0 00 02 00 EF 00 00 33 00 00 00 00 00 00").CopyTo(buffer, 0);
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 30);
		}
		buffer[26] = finsCore[0];
		buffer[27] = finsCore[1];
		BitConverter.GetBytes(buffer.Length - 8).ReverseNew().CopyTo(buffer, 4);
		BitConverter.GetBytes(status).ReverseNew().CopyTo(buffer, 12);
		return buffer;
	}

	private SoftBuffer GetSoftBuffer(byte code, int startIndex)
	{
		if (code == OmronFinsDataType.DM.BitCode)
		{
			return dBuffer;
		}
		if (code == OmronFinsDataType.DM.WordCode)
		{
			return dBuffer;
		}
		if (code == OmronFinsDataType.CIO.BitCode)
		{
			return cioBuffer;
		}
		if (code == OmronFinsDataType.CIO.WordCode)
		{
			return cioBuffer;
		}
		if (code == OmronFinsDataType.WR.BitCode)
		{
			return wBuffer;
		}
		if (code == OmronFinsDataType.WR.WordCode)
		{
			return wBuffer;
		}
		if (code == OmronFinsDataType.HR.BitCode)
		{
			return hBuffer;
		}
		if (code == OmronFinsDataType.HR.WordCode)
		{
			return hBuffer;
		}
		if (code == OmronFinsDataType.AR.BitCode)
		{
			return arBuffer;
		}
		if (code == OmronFinsDataType.AR.WordCode)
		{
			return arBuffer;
		}
		switch (code)
		{
		case 7:
			return cfBuffer;
		case 188:
			return drBuffer;
		case 220:
			return irBuffer;
		case 0:
			return (startIndex >= 45056) ? arBuffer : cioBuffer;
		case 128:
			return (startIndex >= 45056) ? arBuffer : cioBuffer;
		default:
			if ((32 <= code && code < 48) || (208 <= code && code < 224))
			{
				return emBuffer;
			}
			if ((160 <= code && code < 176) || (80 <= code && code < 96))
			{
				return emBuffer;
			}
			if (144 <= code && code < 153)
			{
				return emBuffer;
			}
			if (code == 10)
			{
				return emBuffer;
			}
			throw new Exception(StringResources.Language.NotSupportedDataType);
		}
	}

	private byte[] ReadByCommand(byte[] command)
	{
		if (command[2] == OmronFinsDataType.DM.BitCode || command[2] == OmronFinsDataType.CIO.BitCode || command[2] == OmronFinsDataType.WR.BitCode || command[2] == OmronFinsDataType.HR.BitCode || command[2] == OmronFinsDataType.AR.BitCode || command[2] == 0 || command[2] == 7 || command[2] == 10 || (32 <= command[2] && command[2] < 48) || (208 <= command[2] && command[2] < 224))
		{
			ushort length = (ushort)(command[6] * 256 + command[7]);
			int num = (command[3] * 256 + command[4]) * 16 + command[5];
			SoftBuffer softBuffer = GetSoftBuffer(command[2], num);
			if ((command[2] == 0 || command[2] == 128) && num >= 45056)
			{
				num -= 45056;
			}
			return (from m in softBuffer.GetBool(num, length)
				select (byte)(m ? 1 : 0)).ToArray();
		}
		if (command[2] == OmronFinsDataType.DM.WordCode || command[2] == OmronFinsDataType.CIO.WordCode || command[2] == OmronFinsDataType.WR.WordCode || command[2] == OmronFinsDataType.HR.WordCode || command[2] == OmronFinsDataType.AR.WordCode || command[2] == 128 || command[2] == 188 || command[2] == 220 || command[2] == 152 || (160 <= command[2] && command[2] < 176) || (80 <= command[2] && command[2] < 96))
		{
			ushort num2 = (ushort)(command[6] * 256 + command[7]);
			int num3 = command[3] * 256 + command[4];
			if (num2 > 999)
			{
				return null;
			}
			SoftBuffer softBuffer2 = GetSoftBuffer(command[2], num3);
			if ((command[2] == 0 || command[2] == 128) && num3 >= 45056)
			{
				num3 -= 45056;
			}
			return softBuffer2.GetBytes(num3 * 2, num2 * 2);
		}
		return new byte[0];
	}

	private byte[] ReadByMultiCommand(byte[] command)
	{
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 2; i < command.Length; i += 4)
		{
			int num = command[i + 1] * 256 + command[i + 2];
			if (command[i] == OmronFinsDataType.DM.WordCode || command[i] == OmronFinsDataType.CIO.WordCode || command[i] == OmronFinsDataType.WR.WordCode || command[i] == OmronFinsDataType.HR.WordCode || command[i] == OmronFinsDataType.AR.WordCode || command[i] == 128 || command[i] == 188 || command[i] == 220 || command[i] == 152 || (160 <= command[i] && command[i] < 176) || (80 <= command[i] && command[i] < 96))
			{
				memoryStream.WriteByte(command[i]);
				SoftBuffer softBuffer = GetSoftBuffer(command[i], num);
				if ((command[2] == 0 || command[2] == 128) && num >= 45056)
				{
					num -= 45056;
				}
				memoryStream.Write(softBuffer.GetBytes(num * 2, 2));
			}
		}
		return memoryStream.ToArray();
	}

	private byte[] WriteByMessage(byte[] command)
	{
		if (command[2] == OmronFinsDataType.DM.BitCode || command[2] == OmronFinsDataType.CIO.BitCode || command[2] == OmronFinsDataType.WR.BitCode || command[2] == OmronFinsDataType.HR.BitCode || command[2] == OmronFinsDataType.AR.BitCode || command[2] == 0 || command[2] == 7 || (32 <= command[2] && command[2] < 48) || (208 <= command[2] && command[2] < 224))
		{
			ushort num = (ushort)(command[6] * 256 + command[7]);
			int num2 = (command[3] * 256 + command[4]) * 16 + command[5];
			bool[] value = (from m in SoftBasic.ArrayRemoveBegin(command, 8)
				select m == 1).ToArray();
			SoftBuffer softBuffer = GetSoftBuffer(command[2], num2);
			if ((command[2] == 0 || command[2] == 128) && num2 >= 45056)
			{
				num2 -= 45056;
			}
			softBuffer.SetBool(value, num2);
			return new byte[0];
		}
		ushort num3 = (ushort)(command[6] * 256 + command[7]);
		int num4 = command[3] * 256 + command[4];
		byte[] data = SoftBasic.ArrayRemoveBegin(command, 8);
		SoftBuffer softBuffer2 = GetSoftBuffer(command[2], num4);
		if ((command[2] == 0 || command[2] == 128) && num4 >= 45056)
		{
			num4 -= 45056;
		}
		softBuffer2.SetBytes(data, num4 * 2);
		return new byte[0];
	}

	protected override void LoadFromBytes(byte[] content)
	{
		SoftBuffer.LoadFromBuffer(content, dBuffer, cioBuffer, wBuffer, hBuffer, arBuffer, emBuffer, cfBuffer, irBuffer, drBuffer);
	}

	protected override byte[] SaveToBytes()
	{
		return SoftBuffer.ToMemoryStream(dBuffer, cioBuffer, wBuffer, hBuffer, arBuffer, emBuffer, cfBuffer, irBuffer, drBuffer).ToArray();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			dBuffer?.Dispose();
			cioBuffer?.Dispose();
			wBuffer?.Dispose();
			hBuffer?.Dispose();
			arBuffer?.Dispose();
			emBuffer?.Dispose();
			cfBuffer?.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"OmronFinsServer[{base.Port}]";
	}
}
