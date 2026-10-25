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
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱MC协议的虚拟服务器，支持M,X,Y,D,W的数据池读写操作，支持二进制及ASCII格式进行读写操作，需要在实例化的时候指定。
/// The Mitsubishi MC protocol virtual server supports M, X, Y, D, W data pool read and write operations,
/// and supports binary and ASCII format read and write operations, which need to be specified during instantiation.
/// </summary>
/// <remarks>
/// 如果你没有可以测试的三菱PLC，想要测试自己开发的上位机软件，或是想要在本机实现虚拟PLC，然后进行IO的输入输出练习，都可以使用本类来实现，支持的地址示例参考 Demo 程序测试界面
/// </remarks>
public class MelsecMcServer : DeviceServer
{
	private SoftBuffer xBuffer;

	private SoftBuffer yBuffer;

	private SoftBuffer mBuffer;

	private SoftBuffer lBuffer;

	private SoftBuffer dBuffer;

	private SoftBuffer wBuffer;

	private SoftBuffer bBuffer;

	private SoftBuffer sBuffer;

	private SoftBuffer fBuffer;

	private SoftBuffer rBuffer;

	private SoftBuffer zrBuffer;

	private const int DataPoolLength = 65536;

	private bool isBinary = true;

	/// <summary>
	/// 是否分析通信报文信息，并将分析结果追加到报文日志里去
	/// Should we analyze the communication message information and add the analysis results to the message log?
	/// </summary>
	public bool AnalysisLogMessage { get; set; } = false;

	/// <summary>
	/// 获取或设置当前的通信格式是否是二进制
	/// Get or set whether the current communication format is binary
	/// </summary>
	public bool IsBinary
	{
		get
		{
			return isBinary;
		}
		set
		{
			isBinary = value;
		}
	}

	/// <summary>
	/// 实例化一个默认参数的mc协议的服务器
	/// Instantiate a mc protocol server with default parameters
	/// </summary>
	/// <param name="isBinary">是否是二进制，默认是二进制，否则是ASCII格式</param>
	public MelsecMcServer(bool isBinary = true)
	{
		xBuffer = new SoftBuffer(65536);
		yBuffer = new SoftBuffer(65536);
		mBuffer = new SoftBuffer(65536);
		lBuffer = new SoftBuffer(65536);
		dBuffer = new SoftBuffer(262144);
		wBuffer = new SoftBuffer(131072);
		bBuffer = new SoftBuffer(65536);
		rBuffer = new SoftBuffer(131072);
		zrBuffer = new SoftBuffer(262144);
		sBuffer = new SoftBuffer(65536);
		fBuffer = new SoftBuffer(65536);
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		this.isBinary = isBinary;
		LogMsgFormatBinary = isBinary;
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content.McDataType.DataType == 1)
		{
			OperateResult<SoftBuffer> boolSoftBufferByDataCode = GetBoolSoftBufferByDataCode(operateResult.Content.McDataType.DataCode);
			if (!boolSoftBufferByDataCode.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(boolSoftBufferByDataCode);
			}
			bool[] array = (from m in boolSoftBufferByDataCode.Content.GetBytes(operateResult.Content.AddressStart, length * 16)
				select m != 0).ToArray();
			return OperateResult.CreateSuccessResult(SoftBasic.BoolArrayToByte(array));
		}
		OperateResult<SoftBuffer> wordSoftBufferByDataCode = GetWordSoftBufferByDataCode(operateResult.Content.McDataType.DataCode);
		if (!wordSoftBufferByDataCode.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(wordSoftBufferByDataCode);
		}
		return OperateResult.CreateSuccessResult(wordSoftBufferByDataCode.Content.GetBytes(operateResult.Content.AddressStart * 2, length * 2));
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, 0, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content.McDataType.DataType == 1)
		{
			OperateResult<SoftBuffer> boolSoftBufferByDataCode = GetBoolSoftBufferByDataCode(operateResult.Content.McDataType.DataCode);
			if (!boolSoftBufferByDataCode.IsSuccess)
			{
				return boolSoftBufferByDataCode;
			}
			byte[] data = (from m in SoftBasic.ByteToBoolArray(value)
				select (byte)(m ? 1 : 0)).ToArray();
			boolSoftBufferByDataCode.Content.SetBytes(data, operateResult.Content.AddressStart);
			return OperateResult.CreateSuccessResult();
		}
		OperateResult<SoftBuffer> wordSoftBufferByDataCode = GetWordSoftBufferByDataCode(operateResult.Content.McDataType.DataCode);
		if (!wordSoftBufferByDataCode.IsSuccess)
		{
			return wordSoftBufferByDataCode;
		}
		wordSoftBufferByDataCode.Content.SetBytes(value, operateResult.Content.AddressStart * 2);
		return OperateResult.CreateSuccessResult();
	}

	private OperateResult<SoftBuffer> GetBoolSoftBufferByDataCode(ushort dataCode)
	{
		if (dataCode == MelsecMcDataType.M.DataCode)
		{
			return OperateResult.CreateSuccessResult(mBuffer);
		}
		if (dataCode == MelsecMcDataType.X.DataCode)
		{
			return OperateResult.CreateSuccessResult(xBuffer);
		}
		if (dataCode == MelsecMcDataType.Y.DataCode)
		{
			return OperateResult.CreateSuccessResult(yBuffer);
		}
		if (dataCode == MelsecMcDataType.L.DataCode)
		{
			return OperateResult.CreateSuccessResult(lBuffer);
		}
		if (dataCode == MelsecMcDataType.B.DataCode)
		{
			return OperateResult.CreateSuccessResult(bBuffer);
		}
		if (dataCode == MelsecMcDataType.S.DataCode)
		{
			return OperateResult.CreateSuccessResult(sBuffer);
		}
		if (dataCode == MelsecMcDataType.F.DataCode)
		{
			return OperateResult.CreateSuccessResult(fBuffer);
		}
		return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
	}

	private OperateResult<SoftBuffer> GetWordSoftBufferByDataCode(ushort dataCode)
	{
		if (dataCode == MelsecMcDataType.D.DataCode)
		{
			return OperateResult.CreateSuccessResult(dBuffer);
		}
		if (dataCode == MelsecMcDataType.W.DataCode)
		{
			return OperateResult.CreateSuccessResult(wBuffer);
		}
		if (dataCode == MelsecMcDataType.R.DataCode)
		{
			return OperateResult.CreateSuccessResult(rBuffer);
		}
		if (dataCode == MelsecMcDataType.ZR.DataCode)
		{
			return OperateResult.CreateSuccessResult(zrBuffer);
		}
		return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return HslHelper.ReadBool(this, address, length);
		}
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, 0, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		if (operateResult.Content.McDataType.DataType == 0)
		{
			return new OperateResult<bool[]>(StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate);
		}
		OperateResult<SoftBuffer> boolSoftBufferByDataCode = GetBoolSoftBufferByDataCode(operateResult.Content.McDataType.DataCode);
		if (!boolSoftBufferByDataCode.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(boolSoftBufferByDataCode);
		}
		return OperateResult.CreateSuccessResult((from m in boolSoftBufferByDataCode.Content.GetBytes(operateResult.Content.AddressStart, length)
			select m != 0).ToArray());
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		int num = -1;
		if (address.Contains("."))
		{
			num = HslHelper.CalculateBitStartIndex(address.Substring(address.IndexOf('.') + 1));
			address = address.Substring(0, address.IndexOf('.'));
		}
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, 0, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		if (num >= 0)
		{
			if (operateResult.Content.McDataType.DataType == 1)
			{
				return new OperateResult<bool[]>(StringResources.Language.MelsecCurrentTypeNotSupportedBitOperate);
			}
			OperateResult<SoftBuffer> wordSoftBufferByDataCode = GetWordSoftBufferByDataCode(operateResult.Content.McDataType.DataCode);
			if (!wordSoftBufferByDataCode.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(wordSoftBufferByDataCode);
			}
			wordSoftBufferByDataCode.Content.SetBool(value, operateResult.Content.AddressStart * 16 + num);
		}
		else
		{
			if (operateResult.Content.McDataType.DataType == 0)
			{
				return new OperateResult<bool[]>(StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate);
			}
			OperateResult<SoftBuffer> boolSoftBufferByDataCode = GetBoolSoftBufferByDataCode(operateResult.Content.McDataType.DataCode);
			if (!boolSoftBufferByDataCode.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(boolSoftBufferByDataCode);
			}
			boolSoftBufferByDataCode.Content.SetBytes(value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), operateResult.Content.AddressStart);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		string text = base.GetLogTextFromBinary(session, content);
		if (AnalysisLogMessage)
		{
			string text2 = string.Empty;
			if (IsBinary)
			{
				text2 = McHelper.AnalysisAddressLogMessage(session, base.ByteTransform, content);
			}
			text += text2;
		}
		return text;
	}

	protected override INetMessage GetNewNetMessage()
	{
		if (isBinary)
		{
			return new MelsecQnA3EBinaryMessage();
		}
		return new MelsecQnA3EAsciiMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (isBinary)
		{
			return OperateResult.CreateSuccessResult(ReadFromMcCore(receive.RemoveBegin(11)));
		}
		return OperateResult.CreateSuccessResult(ReadFromMcAsciiCore(receive.RemoveBegin(22)));
	}

	/// <summary>
	/// 当收到mc协议的报文的时候应该触发的方法，允许继承重写，来实现自定义的返回，或是数据监听。
	/// The method that should be triggered when a message of the mc protocol is received,
	/// allowing inheritance to be rewritten to implement custom return or data monitoring.
	/// </summary>
	/// <param name="mcCore">mc报文</param>
	/// <returns>返回的报文信息</returns>
	protected virtual byte[] ReadFromMcCore(byte[] mcCore)
	{
		if (mcCore[0] == 1 && mcCore[1] == 1)
		{
			return PackCommand(0, Encoding.ASCII.GetBytes("L02CPU          \u0005A"));
		}
		if (mcCore[0] == 1 && mcCore[1] == 4)
		{
			return ReadByCommand(mcCore);
		}
		if (mcCore[0] == 1 && mcCore[1] == 16)
		{
			return PackCommand(0, null);
		}
		if (mcCore[0] == 1 && mcCore[1] == 20)
		{
			if (!base.EnableWrite)
			{
				return PackCommand(49250, null);
			}
			return PackCommand(0, WriteByMessage(mcCore));
		}
		if (mcCore[0] == 2 && mcCore[1] == 16)
		{
			return PackCommand(0, null);
		}
		if (mcCore[0] == 3 && mcCore[1] == 4)
		{
			return ReadRandomByCommand(mcCore);
		}
		if (mcCore[0] == 2 && mcCore[1] == 20)
		{
			return WriteRandomByCommand(mcCore);
		}
		if (mcCore[0] == 6 && mcCore[1] == 4)
		{
			return ReadBlockByCommand(mcCore);
		}
		if (mcCore[0] == 6 && mcCore[1] == 16)
		{
			return PackCommand(0, null);
		}
		if (mcCore[0] == 23 && mcCore[1] == 22)
		{
			return PackCommand(0, null);
		}
		return null;
	}

	/// <summary>
	/// 当收到mc协议的报文的时候应该触发的方法，允许继承重写，来实现自定义的返回，或是数据监听。
	/// The method that should be triggered when a message of the mc protocol is received,
	/// allowing inheritance to be rewritten to implement custom return or data monitoring.
	/// </summary>
	/// <param name="mcCore">mc报文</param>
	/// <returns>返回的报文信息</returns>
	protected virtual byte[] ReadFromMcAsciiCore(byte[] mcCore)
	{
		if (mcCore[0] == 48 && mcCore[1] == 52 && mcCore[2] == 48 && mcCore[3] == 49)
		{
			return ReadAsciiByCommand(mcCore);
		}
		if (mcCore[0] == 49 && mcCore[1] == 52 && mcCore[2] == 48 && mcCore[3] == 49)
		{
			if (!base.EnableWrite)
			{
				return PackCommand(49250, null);
			}
			return PackCommand(0, WriteAsciiByMessage(mcCore));
		}
		return null;
	}

	/// <summary>
	/// 将状态码，数据打包成一个完成的回复报文信息
	/// </summary>
	/// <param name="status">状态信息</param>
	/// <param name="data">数据</param>
	/// <returns>状态信息</returns>
	protected virtual byte[] PackCommand(ushort status, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		if (isBinary)
		{
			byte[] buffer = new byte[11 + data.Length];
			SoftBasic.HexStringToBytes("D0 00 00 FF FF 03 00 00 00 00 00").CopyTo(buffer, 0);
			if (data.Length != 0)
			{
				data.CopyTo(buffer, 11);
			}
			BitConverter.GetBytes((short)(data.Length + 2)).CopyTo(buffer, 7);
			BitConverter.GetBytes(status).CopyTo(buffer, 9);
			return buffer;
		}
		byte[] buffer2 = new byte[22 + data.Length];
		Encoding.ASCII.GetBytes("D00000FF03FF0000000000").CopyTo(buffer2, 0);
		if (data.Length != 0)
		{
			data.CopyTo(buffer2, 22);
		}
		Encoding.ASCII.GetBytes((data.Length + 4).ToString("X4")).CopyTo(buffer2, 14);
		Encoding.ASCII.GetBytes(status.ToString("X4")).CopyTo(buffer2, 18);
		return buffer2;
	}

	private byte[] ReadByCommand(byte[] command)
	{
		ushort num = base.ByteTransform.TransUInt16(command, 8);
		int num2 = command[6] * 65536 + command[5] * 256 + command[4];
		if (command[2] == 1)
		{
			if (num > 7168)
			{
				return PackCommand(49233, null);
			}
			OperateResult<SoftBuffer> boolSoftBufferByDataCode = GetBoolSoftBufferByDataCode(command[7]);
			if (!boolSoftBufferByDataCode.IsSuccess)
			{
				return PackCommand(49242, null);
			}
			return PackCommand(0, MelsecHelper.TransBoolArrayToByteData(boolSoftBufferByDataCode.Content.GetBytes(num2, num)));
		}
		if (num > 960)
		{
			return PackCommand(49233, null);
		}
		OperateResult<SoftBuffer> boolSoftBufferByDataCode2 = GetBoolSoftBufferByDataCode(command[7]);
		if (boolSoftBufferByDataCode2.IsSuccess)
		{
			return PackCommand(0, (from m in boolSoftBufferByDataCode2.Content.GetBytes(num2, num * 16)
				select m != 0).ToArray().ToByteArray());
		}
		boolSoftBufferByDataCode2 = GetWordSoftBufferByDataCode(command[7]);
		if (boolSoftBufferByDataCode2.IsSuccess)
		{
			return PackCommand(0, boolSoftBufferByDataCode2.Content.GetBytes(num2 * 2, num * 2));
		}
		return PackCommand(49242, null);
	}

	private void SetRandomReadResult(SoftBuffer softBuffer, byte[] buffer, int address, int index)
	{
		(from m in softBuffer.GetBytes(address, 16)
			select m != 0).ToArray().ToByteArray().CopyTo(buffer, index * 2);
	}

	private void SetRandomReadWordResult(SoftBuffer softBuffer, byte[] buffer, int address, int index)
	{
		softBuffer.GetBytes(address * 2, 2).CopyTo(buffer, index * 2);
	}

	private byte[] ReadRandomByCommand(byte[] command)
	{
		int num = command[4];
		byte[] buffer = new byte[num * 2];
		for (int i = 0; i < num; i++)
		{
			int address = command[8 + 4 * i] * 65536 + command[7 + 4 * i] * 256 + command[6 + 4 * i];
			if (command[9 + 4 * i] == MelsecMcDataType.M.DataCode)
			{
				SetRandomReadResult(mBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.X.DataCode)
			{
				SetRandomReadResult(xBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.Y.DataCode)
			{
				SetRandomReadResult(yBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.B.DataCode)
			{
				SetRandomReadResult(bBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.L.DataCode)
			{
				SetRandomReadResult(lBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.S.DataCode)
			{
				SetRandomReadResult(sBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.F.DataCode)
			{
				SetRandomReadResult(fBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.D.DataCode)
			{
				SetRandomReadWordResult(dBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.W.DataCode)
			{
				SetRandomReadWordResult(wBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.R.DataCode)
			{
				SetRandomReadWordResult(rBuffer, buffer, address, i);
			}
			else if (command[9 + 4 * i] == MelsecMcDataType.ZR.DataCode)
			{
				SetRandomReadWordResult(zrBuffer, buffer, address, i);
			}
		}
		return PackCommand(0, buffer);
	}

	private void SetRamdonWriteResult(SoftBuffer softBuffer, byte[] buffer, int address, int index)
	{
		softBuffer.SetBytes((from m in buffer.SelectMiddle(10 + 6 * index, 2).ToBoolArray()
			select (byte)(m ? 1 : 0)).ToArray(), address);
	}

	private void SetRamdonWriteWordResult(SoftBuffer softBuffer, byte[] buffer, int address, int index)
	{
		softBuffer.SetBytes(buffer.SelectMiddle(10 + 6 * index, 2), address * 2);
	}

	private byte[] WriteRandomByCommand(byte[] command)
	{
		int num = command[4];
		for (int i = 0; i < num; i++)
		{
			int address = command[8 + 6 * i] * 65536 + command[7 + 6 * i] * 256 + command[6 + 6 * i];
			if (command[9 + 6 * i] == MelsecMcDataType.M.DataCode)
			{
				SetRamdonWriteResult(mBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.X.DataCode)
			{
				SetRamdonWriteResult(xBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.Y.DataCode)
			{
				SetRamdonWriteResult(yBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.B.DataCode)
			{
				SetRamdonWriteResult(bBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.L.DataCode)
			{
				SetRamdonWriteResult(lBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.S.DataCode)
			{
				SetRamdonWriteResult(sBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.F.DataCode)
			{
				SetRamdonWriteResult(fBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.D.DataCode)
			{
				SetRamdonWriteWordResult(dBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.W.DataCode)
			{
				SetRamdonWriteWordResult(wBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.R.DataCode)
			{
				SetRamdonWriteWordResult(rBuffer, command, address, i);
				continue;
			}
			if (command[9 + 6 * i] == MelsecMcDataType.ZR.DataCode)
			{
				SetRamdonWriteWordResult(zrBuffer, command, address, i);
				continue;
			}
			throw new Exception(StringResources.Language.NotSupportedDataType);
		}
		return PackCommand(0, null);
	}

	private byte[] ReadBlockByCommand(byte[] command)
	{
		int num = command[4];
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i < num; i++)
		{
			int num2 = command[8 + 6 * i] * 65536 + command[7 + 6 * i] * 256 + command[6 + 6 * i];
			ushort num3 = base.ByteTransform.TransUInt16(command, 10 + 6 * i);
			if (command[9 + 6 * i] == MelsecMcDataType.M.DataCode)
			{
				memoryStream.Write((from m in mBuffer.GetBytes(num2, num3 * 16)
					select m != 0).ToArray().ToByteArray());
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.X.DataCode)
			{
				memoryStream.Write((from m in xBuffer.GetBytes(num2, num3 * 16)
					select m != 0).ToArray().ToByteArray());
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.Y.DataCode)
			{
				memoryStream.Write((from m in yBuffer.GetBytes(num2, num3 * 16)
					select m != 0).ToArray().ToByteArray());
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.B.DataCode)
			{
				memoryStream.Write((from m in bBuffer.GetBytes(num2, num3 * 16)
					select m != 0).ToArray().ToByteArray());
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.L.DataCode)
			{
				memoryStream.Write((from m in lBuffer.GetBytes(num2, num3 * 16)
					select m != 0).ToArray().ToByteArray());
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.S.DataCode)
			{
				memoryStream.Write((from m in sBuffer.GetBytes(num2, num3 * 16)
					select m != 0).ToArray().ToByteArray());
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.F.DataCode)
			{
				memoryStream.Write((from m in fBuffer.GetBytes(num2, num3 * 16)
					select m != 0).ToArray().ToByteArray());
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.D.DataCode)
			{
				memoryStream.Write(dBuffer.GetBytes(num2 * 2, num3 * 2));
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.W.DataCode)
			{
				memoryStream.Write(wBuffer.GetBytes(num2 * 2, num3 * 2));
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.R.DataCode)
			{
				memoryStream.Write(rBuffer.GetBytes(num2 * 2, num3 * 2));
			}
			else if (command[9 + 6 * i] == MelsecMcDataType.ZR.DataCode)
			{
				memoryStream.Write(zrBuffer.GetBytes(num2 * 2, num3 * 2));
			}
		}
		return PackCommand(0, memoryStream.ToArray());
	}

	private byte[] ReadAsciiPackCommand(SoftBuffer softBuffer, int startIndex, ushort length, bool isBool)
	{
		if (isBool)
		{
			bool[] array = (from m in softBuffer.GetBytes(startIndex, length * 16)
				select m != 0).ToArray();
			return PackCommand(0, MelsecHelper.TransByteArrayToAsciiByteArray(SoftBasic.BoolArrayToByte(array)));
		}
		return PackCommand(0, MelsecHelper.TransByteArrayToAsciiByteArray(softBuffer.GetBytes(startIndex * 2, length * 2)));
	}

	private byte[] ReadAsciiByCommand(byte[] command)
	{
		ushort num = Convert.ToUInt16(Encoding.ASCII.GetString(command, 16, 4), 16);
		string message = Encoding.ASCII.GetString(command, 8, 2);
		int num2 = 0;
		num2 = ((!(message == MelsecMcDataType.X.AsciiCode) && !(message == MelsecMcDataType.Y.AsciiCode) && !(message == MelsecMcDataType.W.AsciiCode) && !(message == MelsecMcDataType.B.AsciiCode) && !(message == MelsecMcDataType.L.AsciiCode) && !(message == MelsecMcDataType.ZR.AsciiCode)) ? Convert.ToInt32(Encoding.ASCII.GetString(command, 10, 6)) : Convert.ToInt32(Encoding.ASCII.GetString(command, 10, 6), 16));
		if (command[7] == 49)
		{
			if (num > 3584)
			{
				return PackCommand(49233, null);
			}
			if (message == MelsecMcDataType.M.AsciiCode)
			{
				return PackCommand(0, (from m in mBuffer.GetBytes(num2, num)
					select (byte)((m != 0) ? 49 : 48)).ToArray());
			}
			if (message == MelsecMcDataType.X.AsciiCode)
			{
				return PackCommand(0, (from m in xBuffer.GetBytes(num2, num)
					select (byte)((m != 0) ? 49 : 48)).ToArray());
			}
			if (message == MelsecMcDataType.Y.AsciiCode)
			{
				return PackCommand(0, (from m in yBuffer.GetBytes(num2, num)
					select (byte)((m != 0) ? 49 : 48)).ToArray());
			}
			if (message == MelsecMcDataType.B.AsciiCode)
			{
				return PackCommand(0, (from m in bBuffer.GetBytes(num2, num)
					select (byte)((m != 0) ? 49 : 48)).ToArray());
			}
			if (message == MelsecMcDataType.L.AsciiCode)
			{
				return PackCommand(0, (from m in lBuffer.GetBytes(num2, num)
					select (byte)((m != 0) ? 49 : 48)).ToArray());
			}
			if (message == MelsecMcDataType.S.AsciiCode)
			{
				return PackCommand(0, (from m in sBuffer.GetBytes(num2, num)
					select (byte)((m != 0) ? 49 : 48)).ToArray());
			}
			if (message == MelsecMcDataType.F.AsciiCode)
			{
				return PackCommand(0, (from m in fBuffer.GetBytes(num2, num)
					select (byte)((m != 0) ? 49 : 48)).ToArray());
			}
			return PackCommand(49242, null);
		}
		if (num > 960)
		{
			return PackCommand(49233, null);
		}
		if (message == MelsecMcDataType.M.AsciiCode)
		{
			return ReadAsciiPackCommand(mBuffer, num2, num, isBool: true);
		}
		if (message == MelsecMcDataType.X.AsciiCode)
		{
			return ReadAsciiPackCommand(xBuffer, num2, num, isBool: true);
		}
		if (message == MelsecMcDataType.Y.AsciiCode)
		{
			return ReadAsciiPackCommand(yBuffer, num2, num, isBool: true);
		}
		if (message == MelsecMcDataType.B.AsciiCode)
		{
			return ReadAsciiPackCommand(bBuffer, num2, num, isBool: true);
		}
		if (message == MelsecMcDataType.L.AsciiCode)
		{
			return ReadAsciiPackCommand(lBuffer, num2, num, isBool: true);
		}
		if (message == MelsecMcDataType.S.AsciiCode)
		{
			return ReadAsciiPackCommand(sBuffer, num2, num, isBool: true);
		}
		if (message == MelsecMcDataType.F.AsciiCode)
		{
			return ReadAsciiPackCommand(fBuffer, num2, num, isBool: true);
		}
		if (message == MelsecMcDataType.D.AsciiCode)
		{
			return ReadAsciiPackCommand(dBuffer, num2, num, isBool: false);
		}
		if (message == MelsecMcDataType.W.AsciiCode)
		{
			return ReadAsciiPackCommand(wBuffer, num2, num, isBool: false);
		}
		if (message == MelsecMcDataType.R.AsciiCode)
		{
			return ReadAsciiPackCommand(rBuffer, num2, num, isBool: false);
		}
		if (message == MelsecMcDataType.ZR.AsciiCode)
		{
			return ReadAsciiPackCommand(zrBuffer, num2, num, isBool: false);
		}
		return PackCommand(49242, null);
	}

	private byte[] WriteByMessage(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return null;
		}
		ushort count = base.ByteTransform.TransUInt16(command, 8);
		int num = command[6] * 65536 + command[5] * 256 + command[4];
		if (command[2] == 1)
		{
			byte[] source = McBinaryHelper.ExtractActualDataHelper(command.RemoveBegin(10), isBit: true);
			if (command[7] == MelsecMcDataType.M.DataCode)
			{
				mBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (command[7] == MelsecMcDataType.X.DataCode)
			{
				xBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (command[7] == MelsecMcDataType.Y.DataCode)
			{
				yBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (command[7] == MelsecMcDataType.B.DataCode)
			{
				bBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (command[7] == MelsecMcDataType.L.DataCode)
			{
				lBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (command[7] == MelsecMcDataType.S.DataCode)
			{
				sBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else
			{
				if (command[7] != MelsecMcDataType.F.DataCode)
				{
					throw new Exception(StringResources.Language.NotSupportedDataType);
				}
				fBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.M.DataCode)
		{
			byte[] data = (from m in SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(command, 10))
				select (byte)(m ? 1 : 0)).ToArray();
			mBuffer.SetBytes(data, num);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.X.DataCode)
		{
			byte[] data2 = (from m in SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(command, 10))
				select (byte)(m ? 1 : 0)).ToArray();
			xBuffer.SetBytes(data2, num);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.Y.DataCode)
		{
			byte[] data3 = (from m in SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(command, 10))
				select (byte)(m ? 1 : 0)).ToArray();
			yBuffer.SetBytes(data3, num);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.B.DataCode)
		{
			byte[] data4 = (from m in SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(command, 10))
				select (byte)(m ? 1 : 0)).ToArray();
			bBuffer.SetBytes(data4, num);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.L.DataCode)
		{
			byte[] data5 = (from m in SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(command, 10))
				select (byte)(m ? 1 : 0)).ToArray();
			lBuffer.SetBytes(data5, num);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.S.DataCode)
		{
			byte[] data6 = (from m in SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(command, 10))
				select (byte)(m ? 1 : 0)).ToArray();
			sBuffer.SetBytes(data6, num);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.F.DataCode)
		{
			byte[] data7 = (from m in SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(command, 10))
				select (byte)(m ? 1 : 0)).ToArray();
			fBuffer.SetBytes(data7, num);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.D.DataCode)
		{
			dBuffer.SetBytes(SoftBasic.ArrayRemoveBegin(command, 10), num * 2);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.W.DataCode)
		{
			wBuffer.SetBytes(SoftBasic.ArrayRemoveBegin(command, 10), num * 2);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.R.DataCode)
		{
			rBuffer.SetBytes(SoftBasic.ArrayRemoveBegin(command, 10), num * 2);
			return new byte[0];
		}
		if (command[7] == MelsecMcDataType.ZR.DataCode)
		{
			zrBuffer.SetBytes(SoftBasic.ArrayRemoveBegin(command, 10), num * 2);
			return new byte[0];
		}
		throw new Exception(StringResources.Language.NotSupportedDataType);
	}

	private byte[] WriteAsciiByMessage(byte[] command)
	{
		ushort count = Convert.ToUInt16(Encoding.ASCII.GetString(command, 16, 4), 16);
		string message = Encoding.ASCII.GetString(command, 8, 2);
		int num = 0;
		num = ((!(message == MelsecMcDataType.X.AsciiCode) && !(message == MelsecMcDataType.Y.AsciiCode) && !(message == MelsecMcDataType.W.AsciiCode) && !(message == MelsecMcDataType.B.AsciiCode) && !(message == MelsecMcDataType.L.AsciiCode) && !(message == MelsecMcDataType.ZR.AsciiCode)) ? Convert.ToInt32(Encoding.ASCII.GetString(command, 10, 6)) : Convert.ToInt32(Encoding.ASCII.GetString(command, 10, 6), 16));
		if (command[7] == 49)
		{
			byte[] source = (from m in command.RemoveBegin(20)
				select (m == 49) ? ((byte)1) : ((byte)0)).ToArray();
			if (message == MelsecMcDataType.M.AsciiCode)
			{
				mBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (message == MelsecMcDataType.X.AsciiCode)
			{
				xBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (message == MelsecMcDataType.Y.AsciiCode)
			{
				yBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (message == MelsecMcDataType.B.AsciiCode)
			{
				bBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (message == MelsecMcDataType.L.AsciiCode)
			{
				lBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else if (message == MelsecMcDataType.S.AsciiCode)
			{
				sBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			else
			{
				if (!(message == MelsecMcDataType.F.AsciiCode))
				{
					throw new Exception(StringResources.Language.NotSupportedDataType);
				}
				fBuffer.SetBytes(source.Take(count).ToArray(), num);
			}
			return new byte[0];
		}
		if (message == MelsecMcDataType.M.AsciiCode)
		{
			byte[] data = (from m in SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)))
				select (byte)(m ? 1 : 0)).ToArray();
			mBuffer.SetBytes(data, num);
			return new byte[0];
		}
		if (message == MelsecMcDataType.X.AsciiCode)
		{
			byte[] data2 = (from m in SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)))
				select (byte)(m ? 1 : 0)).ToArray();
			xBuffer.SetBytes(data2, num);
			return new byte[0];
		}
		if (message == MelsecMcDataType.Y.AsciiCode)
		{
			byte[] data3 = (from m in SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)))
				select (byte)(m ? 1 : 0)).ToArray();
			yBuffer.SetBytes(data3, num);
			return new byte[0];
		}
		if (message == MelsecMcDataType.B.AsciiCode)
		{
			byte[] data4 = (from m in SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)))
				select (byte)(m ? 1 : 0)).ToArray();
			bBuffer.SetBytes(data4, num);
			return new byte[0];
		}
		if (message == MelsecMcDataType.L.AsciiCode)
		{
			byte[] data5 = (from m in SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)))
				select (byte)(m ? 1 : 0)).ToArray();
			lBuffer.SetBytes(data5, num);
			return new byte[0];
		}
		if (message == MelsecMcDataType.S.AsciiCode)
		{
			byte[] data6 = (from m in SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)))
				select (byte)(m ? 1 : 0)).ToArray();
			sBuffer.SetBytes(data6, num);
			return new byte[0];
		}
		if (message == MelsecMcDataType.F.AsciiCode)
		{
			byte[] data7 = (from m in SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)))
				select (byte)(m ? 1 : 0)).ToArray();
			fBuffer.SetBytes(data7, num);
			return new byte[0];
		}
		if (message == MelsecMcDataType.D.AsciiCode)
		{
			dBuffer.SetBytes(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)), num * 2);
			return new byte[0];
		}
		if (message == MelsecMcDataType.W.AsciiCode)
		{
			wBuffer.SetBytes(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)), num * 2);
			return new byte[0];
		}
		if (message == MelsecMcDataType.R.AsciiCode)
		{
			rBuffer.SetBytes(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)), num * 2);
			return new byte[0];
		}
		if (message == MelsecMcDataType.ZR.AsciiCode)
		{
			zrBuffer.SetBytes(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin(20)), num * 2);
			return new byte[0];
		}
		throw new Exception(StringResources.Language.NotSupportedDataType);
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 458752)
		{
			throw new Exception("File is not correct");
		}
		mBuffer.SetBytes(content, 0, 0, 65536);
		xBuffer.SetBytes(content, 65536, 0, 65536);
		yBuffer.SetBytes(content, 131072, 0, 65536);
		dBuffer.SetBytes(content, 196608, 0, 131072);
		wBuffer.SetBytes(content, 327680, 0, 131072);
		if (content.Length >= 12)
		{
			bBuffer.SetBytes(content, 458752, 0, 65536);
			rBuffer.SetBytes(content, 524288, 0, 131072);
			zrBuffer.SetBytes(content, 655360, 0, 131072);
		}
		if (content.Length >= 15)
		{
			dBuffer.SetBytes(content, 786432, 131072, 131072);
			lBuffer.SetBytes(content, 917504, 0, 65536);
		}
	}

	[HslMqttApi]
	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[983040];
		Array.Copy(mBuffer.GetBytes(), 0, buffer, 0, 65536);
		Array.Copy(xBuffer.GetBytes(), 0, buffer, 65536, 65536);
		Array.Copy(yBuffer.GetBytes(), 0, buffer, 131072, 65536);
		Array.Copy(dBuffer.GetBytes(), 0, buffer, 196608, 131072);
		Array.Copy(wBuffer.GetBytes(), 0, buffer, 327680, 131072);
		Array.Copy(bBuffer.GetBytes(), 0, buffer, 458752, 65536);
		Array.Copy(rBuffer.GetBytes(), 0, buffer, 524288, 131072);
		Array.Copy(zrBuffer.GetBytes(), 0, buffer, 655360, 131072);
		Array.Copy(dBuffer.GetBytes(), 131072, buffer, 786432, 131072);
		Array.Copy(lBuffer.GetBytes(), 0, buffer, 917504, 65536);
		return buffer;
	}

	/// <summary>
	/// 释放当前的对象
	/// </summary>
	/// <param name="disposing">是否托管对象</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			xBuffer?.Dispose();
			yBuffer?.Dispose();
			mBuffer?.Dispose();
			dBuffer?.Dispose();
			wBuffer?.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"MelsecMcServer[{base.Port}]";
	}
}
