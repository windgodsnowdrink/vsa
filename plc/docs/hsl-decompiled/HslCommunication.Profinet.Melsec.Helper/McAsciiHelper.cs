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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core.Address;

namespace HslCommunication.Profinet.Melsec.Helper;

/// <summary>
/// 基于MC协议的ASCII格式的辅助类
/// </summary>
public class McAsciiHelper
{
	/// <summary>
	/// 将MC协议的核心报文打包成一个可以直接对PLC进行发送的原始报文
	/// </summary>
	/// <param name="mc">三菱MC协议的核心通信对象</param>
	/// <param name="mcCore">MC协议的核心报文</param>
	/// <returns>原始报文信息</returns>
	public static byte[] PackMcCommand(IReadWriteMc mc, byte[] mcCore)
	{
		byte[] array = SoftBasic.BuildAsciiBytesFrom(mc.TargetIOStation);
		byte[] buffer = new byte[22 + mcCore.Length];
		buffer[0] = 53;
		buffer[1] = 48;
		buffer[2] = 48;
		buffer[3] = 48;
		buffer[4] = SoftBasic.BuildAsciiBytesFrom(mc.NetworkNumber)[0];
		buffer[5] = SoftBasic.BuildAsciiBytesFrom(mc.NetworkNumber)[1];
		buffer[6] = SoftBasic.BuildAsciiBytesFrom(mc.PLCNumber)[0];
		buffer[7] = SoftBasic.BuildAsciiBytesFrom(mc.PLCNumber)[1];
		buffer[8] = array[0];
		buffer[9] = array[1];
		buffer[10] = array[2];
		buffer[11] = array[3];
		buffer[12] = SoftBasic.BuildAsciiBytesFrom(mc.NetworkStationNumber)[0];
		buffer[13] = SoftBasic.BuildAsciiBytesFrom(mc.NetworkStationNumber)[1];
		buffer[14] = SoftBasic.BuildAsciiBytesFrom((ushort)(buffer.Length - 18))[0];
		buffer[15] = SoftBasic.BuildAsciiBytesFrom((ushort)(buffer.Length - 18))[1];
		buffer[16] = SoftBasic.BuildAsciiBytesFrom((ushort)(buffer.Length - 18))[2];
		buffer[17] = SoftBasic.BuildAsciiBytesFrom((ushort)(buffer.Length - 18))[3];
		buffer[18] = 48;
		buffer[19] = 48;
		buffer[20] = 49;
		buffer[21] = 48;
		mcCore.CopyTo(buffer, 22);
		return buffer;
	}

	/// <summary>
	/// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
	/// </summary>
	/// <param name="response">反馈的数据内容</param>
	/// <param name="isBit">是否位读取</param>
	/// <returns>解析后的结果对象</returns>
	public static byte[] ExtractActualDataHelper(byte[] response, bool isBit)
	{
		if (isBit)
		{
			return response.Select((byte m) => (m != 48) ? ((byte)1) : ((byte)0)).ToArray();
		}
		return MelsecHelper.TransAsciiByteArrayToByteArray(response);
	}

	/// <summary>
	/// 检查反馈的内容是否正确的
	/// </summary>
	/// <param name="content">MC的反馈的内容</param>
	/// <returns>是否正确</returns>
	public static OperateResult CheckResponseContent(byte[] content)
	{
		if (content == null || content.Length < 22)
		{
			return new OperateResult(StringResources.Language.ReceiveDataLengthTooShort + "22, Content: " + SoftBasic.GetAsciiStringRender(content));
		}
		ushort num = Convert.ToUInt16(Encoding.ASCII.GetString(content, 18, 4), 16);
		if (num != 0)
		{
			return new OperateResult(num, MelsecHelper.GetErrorDescription(num));
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 从三菱地址，是否位读取进行创建读取Ascii格式的MC的核心报文
	/// </summary>
	/// <param name="addressData">三菱Mc协议的数据地址</param>
	/// <param name="isBit">是否进行了位读取操作</param>
	/// <returns>带有成功标识的报文对象</returns>
	public static byte[] BuildAsciiReadMcCoreCommand(McAddressData addressData, bool isBit)
	{
		return new byte[20]
		{
			48,
			52,
			48,
			49,
			48,
			48,
			48,
			(byte)(isBit ? 49 : 48),
			Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[0],
			Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[1],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[0],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[1],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[2],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[3],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[4],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[5],
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[0],
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[1],
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[2],
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[3]
		};
	}

	/// <summary>
	/// 从三菱扩展地址，是否位读取进行创建读取的MC的核心报文
	/// </summary>
	/// <param name="isBit">是否进行了位读取操作</param>
	/// <param name="extend">扩展指定</param>
	/// <param name="addressData">三菱Mc协议的数据地址</param>
	/// <returns>带有成功标识的报文对象</returns>
	public static byte[] BuildAsciiReadMcCoreExtendCommand(McAddressData addressData, ushort extend, bool isBit)
	{
		return new byte[32]
		{
			48,
			52,
			48,
			49,
			48,
			48,
			56,
			(byte)(isBit ? 49 : 48),
			48,
			48,
			74,
			SoftBasic.BuildAsciiBytesFrom(extend)[1],
			SoftBasic.BuildAsciiBytesFrom(extend)[2],
			SoftBasic.BuildAsciiBytesFrom(extend)[3],
			48,
			48,
			48,
			Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[0],
			Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[1],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[0],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[1],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[2],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[3],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[4],
			MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[5],
			48,
			48,
			48,
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[0],
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[1],
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[2],
			SoftBasic.BuildAsciiBytesFrom(addressData.Length)[3]
		};
	}

	/// <summary>
	/// 以字为单位，创建ASCII数据写入的核心报文
	/// </summary>
	/// <param name="addressData">三菱Mc协议的数据地址</param>
	/// <param name="value">实际的原始数据信息</param>
	/// <returns>带有成功标识的报文对象</returns>
	public static byte[] BuildAsciiWriteWordCoreCommand(McAddressData addressData, byte[] value)
	{
		value = MelsecHelper.TransByteArrayToAsciiByteArray(value);
		byte[] buffer = new byte[20 + value.Length];
		buffer[0] = 49;
		buffer[1] = 52;
		buffer[2] = 48;
		buffer[3] = 49;
		buffer[4] = 48;
		buffer[5] = 48;
		buffer[6] = 48;
		buffer[7] = 48;
		buffer[8] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[0];
		buffer[9] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[1];
		buffer[10] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[0];
		buffer[11] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[1];
		buffer[12] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[2];
		buffer[13] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[3];
		buffer[14] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[4];
		buffer[15] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[5];
		buffer[16] = SoftBasic.BuildAsciiBytesFrom((ushort)(value.Length / 4))[0];
		buffer[17] = SoftBasic.BuildAsciiBytesFrom((ushort)(value.Length / 4))[1];
		buffer[18] = SoftBasic.BuildAsciiBytesFrom((ushort)(value.Length / 4))[2];
		buffer[19] = SoftBasic.BuildAsciiBytesFrom((ushort)(value.Length / 4))[3];
		value.CopyTo(buffer, 20);
		return buffer;
	}

	/// <summary>
	/// 以位为单位，创建ASCII数据写入的核心报文
	/// </summary>
	/// <param name="addressData">三菱Mc协议的数据地址</param>
	/// <param name="value">原始的bool数组数据</param>
	/// <returns>带有成功标识的报文对象</returns>
	public static byte[] BuildAsciiWriteBitCoreCommand(McAddressData addressData, bool[] value)
	{
		if (value == null)
		{
			value = new bool[0];
		}
		byte[] array = value.Select((bool m) => (byte)(m ? 49 : 48)).ToArray();
		byte[] buffer = new byte[20 + array.Length];
		buffer[0] = 49;
		buffer[1] = 52;
		buffer[2] = 48;
		buffer[3] = 49;
		buffer[4] = 48;
		buffer[5] = 48;
		buffer[6] = 48;
		buffer[7] = 49;
		buffer[8] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[0];
		buffer[9] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[1];
		buffer[10] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[0];
		buffer[11] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[1];
		buffer[12] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[2];
		buffer[13] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[3];
		buffer[14] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[4];
		buffer[15] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[5];
		buffer[16] = SoftBasic.BuildAsciiBytesFrom((ushort)value.Length)[0];
		buffer[17] = SoftBasic.BuildAsciiBytesFrom((ushort)value.Length)[1];
		buffer[18] = SoftBasic.BuildAsciiBytesFrom((ushort)value.Length)[2];
		buffer[19] = SoftBasic.BuildAsciiBytesFrom((ushort)value.Length)[3];
		array.CopyTo(buffer, 20);
		return buffer;
	}

	/// <summary>
	/// 按字为单位随机读取的指令创建
	/// </summary>
	/// <param name="address">地址数组</param>
	/// <returns>指令</returns>
	public static List<byte[]> BuildAsciiReadRandomWordCommand(McAddressData[] address)
	{
		List<byte[]> list = new List<byte[]>();
		List<McAddressData[]> list2 = SoftBasic.ArraySplitByLength(address, 180);
		for (int i = 0; i < list2.Count; i++)
		{
			McAddressData[] array = list2[i];
			byte[] buffer = new byte[12 + array.Length * 8];
			buffer[0] = 48;
			buffer[1] = 52;
			buffer[2] = 48;
			buffer[3] = 51;
			buffer[4] = 48;
			buffer[5] = 48;
			buffer[6] = 48;
			buffer[7] = 48;
			buffer[8] = SoftBasic.BuildAsciiBytesFrom((byte)array.Length)[0];
			buffer[9] = SoftBasic.BuildAsciiBytesFrom((byte)array.Length)[1];
			buffer[10] = 48;
			buffer[11] = 48;
			for (int j = 0; j < array.Length; j++)
			{
				buffer[j * 8 + 12] = Encoding.ASCII.GetBytes(array[j].McDataType.AsciiCode)[0];
				buffer[j * 8 + 13] = Encoding.ASCII.GetBytes(array[j].McDataType.AsciiCode)[1];
				buffer[j * 8 + 14] = MelsecHelper.BuildBytesFromAddress(array[j].AddressStart, array[j].McDataType)[0];
				buffer[j * 8 + 15] = MelsecHelper.BuildBytesFromAddress(array[j].AddressStart, array[j].McDataType)[1];
				buffer[j * 8 + 16] = MelsecHelper.BuildBytesFromAddress(array[j].AddressStart, array[j].McDataType)[2];
				buffer[j * 8 + 17] = MelsecHelper.BuildBytesFromAddress(array[j].AddressStart, array[j].McDataType)[3];
				buffer[j * 8 + 18] = MelsecHelper.BuildBytesFromAddress(array[j].AddressStart, array[j].McDataType)[4];
				buffer[j * 8 + 19] = MelsecHelper.BuildBytesFromAddress(array[j].AddressStart, array[j].McDataType)[5];
			}
			list.Add(buffer);
		}
		return list;
	}

	/// <summary>
	/// 按字为单位随机写入的指令创建，一次最多写入80/160个字
	/// </summary>
	/// <param name="address">地址数组</param>
	/// <param name="value">写入的值</param>
	/// <returns>指令</returns>
	public static byte[] BuildWriteRandomWordCommand(McAddressData[] address, byte[] value)
	{
		byte[] buffer = new byte[12 + address.Length * 12];
		buffer[0] = 49;
		buffer[1] = 52;
		buffer[2] = 48;
		buffer[3] = 50;
		buffer[4] = 48;
		buffer[5] = 48;
		buffer[6] = 48;
		buffer[7] = 48;
		buffer[8] = SoftBasic.BuildAsciiBytesFrom((byte)address.Length)[0];
		buffer[9] = SoftBasic.BuildAsciiBytesFrom((byte)address.Length)[1];
		buffer[10] = 48;
		buffer[11] = 48;
		for (int i = 0; i < address.Length; i++)
		{
			buffer[i * 12 + 12] = Encoding.ASCII.GetBytes(address[i].McDataType.AsciiCode)[0];
			buffer[i * 12 + 13] = Encoding.ASCII.GetBytes(address[i].McDataType.AsciiCode)[1];
			buffer[i * 12 + 14] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[0];
			buffer[i * 12 + 15] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[1];
			buffer[i * 12 + 16] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[2];
			buffer[i * 12 + 17] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[3];
			buffer[i * 12 + 18] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[4];
			buffer[i * 12 + 19] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[5];
			buffer[i * 12 + 20] = SoftBasic.BuildAsciiBytesFrom(value[i * 2])[0];
			buffer[i * 12 + 21] = SoftBasic.BuildAsciiBytesFrom(value[i * 2])[1];
			buffer[i * 12 + 22] = SoftBasic.BuildAsciiBytesFrom(value[i * 2 + 1])[0];
			buffer[i * 12 + 23] = SoftBasic.BuildAsciiBytesFrom(value[i * 2 + 1])[1];
		}
		return buffer;
	}

	/// <summary>
	/// 随机读取的指令创建
	/// </summary>
	/// <param name="address">地址数组</param>
	/// <returns>指令</returns>
	public static byte[] BuildAsciiReadRandomCommand(McAddressData[] address)
	{
		byte[] buffer = new byte[12 + address.Length * 12];
		buffer[0] = 48;
		buffer[1] = 52;
		buffer[2] = 48;
		buffer[3] = 54;
		buffer[4] = 48;
		buffer[5] = 48;
		buffer[6] = 48;
		buffer[7] = 48;
		buffer[8] = SoftBasic.BuildAsciiBytesFrom((byte)address.Length)[0];
		buffer[9] = SoftBasic.BuildAsciiBytesFrom((byte)address.Length)[1];
		buffer[10] = 48;
		buffer[11] = 48;
		for (int i = 0; i < address.Length; i++)
		{
			buffer[i * 12 + 12] = Encoding.ASCII.GetBytes(address[i].McDataType.AsciiCode)[0];
			buffer[i * 12 + 13] = Encoding.ASCII.GetBytes(address[i].McDataType.AsciiCode)[1];
			buffer[i * 12 + 14] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[0];
			buffer[i * 12 + 15] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[1];
			buffer[i * 12 + 16] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[2];
			buffer[i * 12 + 17] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[3];
			buffer[i * 12 + 18] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[4];
			buffer[i * 12 + 19] = MelsecHelper.BuildBytesFromAddress(address[i].AddressStart, address[i].McDataType)[5];
			buffer[i * 12 + 20] = SoftBasic.BuildAsciiBytesFrom(address[i].Length)[0];
			buffer[i * 12 + 21] = SoftBasic.BuildAsciiBytesFrom(address[i].Length)[1];
			buffer[i * 12 + 22] = SoftBasic.BuildAsciiBytesFrom(address[i].Length)[2];
			buffer[i * 12 + 23] = SoftBasic.BuildAsciiBytesFrom(address[i].Length)[3];
		}
		return buffer;
	}

	/// <summary>
	/// 读取本站缓冲寄存器的数据信息，需要指定寄存器的地址，和读取的长度
	/// </summary>
	/// <param name="address">寄存器的地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>结果内容</returns>
	public static OperateResult<byte[]> BuildAsciiReadMemoryCommand(string address, ushort length)
	{
		try
		{
			uint value = uint.Parse(address);
			byte[] buffer = new byte[20]
			{
				48, 54, 49, 51, 48, 48, 48, 48, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			};
			SoftBasic.BuildAsciiBytesFrom(value).CopyTo(buffer, 8);
			SoftBasic.BuildAsciiBytesFrom(length).CopyTo(buffer, 16);
			return OperateResult.CreateSuccessResult(buffer);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建读取智能模块的命令，需要指定模块编号，起始地址，读取的长度，注意，该长度以字节为单位。
	/// </summary>
	/// <param name="module">模块编号</param>
	/// <param name="address">智能模块的起始地址</param>
	/// <param name="length">读取的字长度</param>
	/// <returns>报文的结果内容</returns>
	public static OperateResult<byte[]> BuildAsciiReadSmartModule(ushort module, string address, ushort length)
	{
		try
		{
			uint value = uint.Parse(address);
			byte[] buffer = new byte[24]
			{
				48, 54, 48, 49, 48, 48, 48, 48, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0
			};
			SoftBasic.BuildAsciiBytesFrom(value).CopyTo(buffer, 8);
			SoftBasic.BuildAsciiBytesFrom(length).CopyTo(buffer, 16);
			SoftBasic.BuildAsciiBytesFrom(module).CopyTo(buffer, 20);
			return OperateResult.CreateSuccessResult(buffer);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 创建批量读取标签的报文数据信息
	/// </summary>
	/// <param name="tags">标签名</param>
	/// <param name="lengths">长度信息</param>
	/// <param name="isBit">是否位指定</param>
	/// <returns>报文数据信息</returns>
	public static byte[] BuildAsciiReadTag(string[] tags, ushort[] lengths, bool isBit = false)
	{
		if (tags.Length != lengths.Length)
		{
			throw new Exception(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(Encoding.ASCII.GetBytes("041A"));
		memoryStream.Write(Encoding.ASCII.GetBytes("0000"));
		memoryStream.Write(Encoding.ASCII.GetBytes(tags.Length.ToString("X4")));
		memoryStream.Write(Encoding.ASCII.GetBytes("0000"));
		for (int i = 0; i < tags.Length; i++)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(tags[i]);
			memoryStream.Write(SoftBasic.BuildAsciiBytesFrom((ushort)(bytes.Length / 2)));
			for (int j = 0; j < bytes.Length; j++)
			{
				memoryStream.Write(SoftBasic.BuildAsciiBytesFrom(bytes[j]));
			}
			if (isBit)
			{
				memoryStream.Write(Encoding.ASCII.GetBytes("00"));
			}
			else
			{
				memoryStream.Write(Encoding.ASCII.GetBytes("01"));
			}
			memoryStream.Write(Encoding.ASCII.GetBytes("00"));
			memoryStream.Write(SoftBasic.BuildAsciiBytesFrom((ushort)(lengths[i] * 2)));
		}
		return memoryStream.ToArray();
	}

	/// <summary>
	/// 创建写入标签的报文数据信息
	/// </summary>
	/// <param name="tag">标签名称</param>
	/// <param name="data">写入的数据信息</param>
	/// <returns>报文数据信息</returns>
	public static byte[] BuildAsciiWriteTag(string tag, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(Encoding.ASCII.GetBytes("141A"));
		memoryStream.Write(Encoding.ASCII.GetBytes("0000"));
		memoryStream.Write(Encoding.ASCII.GetBytes("0001"));
		memoryStream.Write(Encoding.ASCII.GetBytes("0000"));
		byte[] bytes = Encoding.Unicode.GetBytes(tag);
		memoryStream.Write(SoftBasic.BuildAsciiBytesFrom((ushort)(bytes.Length / 2)));
		for (int i = 0; i < bytes.Length; i++)
		{
			memoryStream.Write(SoftBasic.BuildAsciiBytesFrom(bytes[i]));
		}
		memoryStream.Write(Encoding.ASCII.GetBytes("01"));
		memoryStream.Write(Encoding.ASCII.GetBytes("00"));
		memoryStream.Write(SoftBasic.BuildAsciiBytesFrom((ushort)data.Length));
		memoryStream.Write(Encoding.ASCII.GetBytes(SoftBasic.BytesReverseByWord(data).ToHexString()));
		return memoryStream.ToArray();
	}

	/// <summary>
	/// 解析出标签读取的数据内容
	/// </summary>
	/// <param name="content">返回的数据信息</param>
	/// <returns>解析结果</returns>
	public static OperateResult<byte[]> ExtraTagData(byte[] content)
	{
		try
		{
			int num = Convert.ToInt32(Encoding.ASCII.GetString(content, 0, 4), 16);
			int num2 = 4;
			List<byte> list = new List<byte>(20);
			for (int i = 0; i < num; i++)
			{
				int num3 = Convert.ToInt32(Encoding.ASCII.GetString(content, num2 + 4, 4), 16);
				list.AddRange(SoftBasic.BytesReverseByWord(Encoding.ASCII.GetString(content, num2 + 8, num3 * 2).ToHexBytes()));
				num2 += 8 + num3 * 2;
			}
			return OperateResult.CreateSuccessResult(list.ToArray());
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message + " Source:" + SoftBasic.ByteToHexString(content, ' '));
		}
	}

	/// <summary>
	/// [商业授权] 读取PLC的标签信息，需要传入标签的名称，读取的字长度，标签举例：A; label[1]; bbb[10,10,10]
	/// [Authorization] To read the label information of the PLC, you need to pass in the name of the label,
	/// the length of the word read, and an example of the label: A; label [1]; bbb [10,10,10]
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="tags">标签名</param>
	/// <param name="length">读取长度</param>
	/// <returns>是否成功</returns>
	/// <remarks>
	/// 不可以访问局部标签。
	/// 不可以访问通过GX Works2设置的全局标签。
	/// 为了访问全局标签，需要通过GX Works3的全局标签设置编辑器将“来自于外部设备的访问”的设置项目置为有效。(默认为无效。)
	/// 以ASCII代码进行数据通信时，由于需要从UTF-16将标签名转换为ASCII代码，因此报文容量将增加
	/// </remarks>
	public static OperateResult<byte[]> ReadTags(IReadWriteMc mc, string[] tags, ushort[] length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		byte[] send = BuildAsciiReadTag(tags, length);
		OperateResult<byte[]> operateResult = mc.ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return ExtraTagData(mc.ExtractActualData(operateResult.Content, isBit: false));
	}

	/// <summary>
	/// [商业授权] 读取PLC的标签信息，需要传入标签的名称，读取的字长度，标签举例：A; label[1]; bbb[10,10,10]
	/// [Authorization] To read the label information of the PLC, you need to pass in the name of the label,
	/// the length of the word read, and an example of the label: A; label [1]; bbb [10,10,10]
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="tags">标签名</param>
	/// <param name="length">读取长度</param>
	/// <returns>是否成功</returns>
	/// <remarks>
	/// 不可以访问局部标签。
	/// 不可以访问通过GX Works2设置的全局标签。
	/// 为了访问全局标签，需要通过GX Works3的全局标签设置编辑器将“来自于外部设备的访问”的设置项目置为有效。(默认为无效。)
	/// 以ASCII代码进行数据通信时，由于需要从UTF-16将标签名转换为ASCII代码，因此报文容量将增加
	/// </remarks>
	public static async Task<OperateResult<byte[]>> ReadTagsAsync(IReadWriteMc mc, string[] tags, ushort[] length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		byte[] coreResult = BuildAsciiReadTag(tags, length);
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return ExtraTagData(mc.ExtractActualData(read.Content, isBit: false));
	}

	/// <summary>
	/// [商业授权] 写入PLC的标签数据，需要传入标签的名称，实际写入的字节数据信息，标签举例：A; label[1]; bbb[10,10,10]
	/// [Authorization] To write PLC label data, you need to pass the name of the label, the actual written byte data information, label example: A; label[1];  BBB,10,10 [10]
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="tag">标签名</param>
	/// <param name="data">写入的数据</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult WriteTag(IReadWriteMc mc, string tag, byte[] data)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		byte[] send = BuildAsciiWriteTag(tag, data);
		return mc.ReadFromCoreServer(send);
	}

	/// <summary>
	/// [商业授权] 写入PLC的标签数据，需要传入标签的名称，实际写入的字节数据信息，标签举例：A; label[1]; bbb[10,10,10]
	/// [Authorization] To write PLC label data, you need to pass the name of the label, the actual written byte data information, label example: A; label[1];  BBB,10,10 [10]
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="tag">标签名</param>
	/// <param name="data">写入的数据</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteTagAsync(IReadWriteMc mc, string tag, byte[] data)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		byte[] coreResult = BuildAsciiWriteTag(tag, data);
		return await mc.ReadFromCoreServerAsync(coreResult);
	}
}
