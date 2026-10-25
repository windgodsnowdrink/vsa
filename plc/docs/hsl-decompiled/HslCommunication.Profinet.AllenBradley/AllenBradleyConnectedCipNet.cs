// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Profinet.Omron;

namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// 基于连接的对象访问的CIP协议的实现，用于对罗克韦尔 PLC进行标签的数据读写，对数组，多维数组进行读写操作，支持的数据类型请参照API文档手册。
/// The implementation of the CIP protocol based on connected object access is used to read and write tag data to Rockwell PLC,
/// and read and write arrays and multidimensional arrays. For the supported data types, please refer to the API documentation manual.
/// </summary>
/// <remarks>
/// 支持普通标签的读写，类型要和标签对应上。如果标签是数组，例如 A 是 INT[0...9] 那么Read("A", 1)，返回的是10个short所有字节数组。
/// 如果需要返回10个长度的short数组，请调用 ReadInt16("A[0], 10"); 地址必须写 "A[0]"，不能写 "A" , 如需要读取结构体，参考
/// </remarks>
public class AllenBradleyConnectedCipNet : OmronConnectedCipNet
{
	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public AllenBradleyConnectedCipNet()
	{
	}

	/// <summary>
	/// 根据指定的IP及端口来实例化这个连接对象
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口号信息</param>
	public AllenBradleyConnectedCipNet(string ipAddress, int port = 44818)
		: base(ipAddress, port)
	{
	}

	protected override int GetMaxTransferBytes()
	{
		return 1980;
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		OperateResult<byte[]> operateResult = Read(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		try
		{
			if (operateResult.Content.Length >= 6)
			{
				int count = base.ByteTransform.TransInt32(operateResult.Content, 2);
				return OperateResult.CreateSuccessResult(encoding.GetString(operateResult.Content, 6, count));
			}
			return OperateResult.CreateSuccessResult(encoding.GetString(operateResult.Content));
		}
		catch (Exception ex)
		{
			return new OperateResult<string>(ex.Message + " Source: " + operateResult.Content.ToHexString(' '));
		}
	}

	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		if (string.IsNullOrEmpty(value))
		{
			value = string.Empty;
		}
		byte[] bytes = encoding.GetBytes(value);
		OperateResult operateResult = Write(address + ".LEN", bytes.Length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		byte[] value2 = SoftBasic.ArrayExpandToLengthEven(bytes);
		return WriteTag(address + ".DATA[0]", 194, value2, bytes.Length);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		OperateResult<byte[]> read = await ReadAsync(address, length);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		if (read.Content.Length >= 6)
		{
			return OperateResult.CreateSuccessResult(encoding.GetString(count: base.ByteTransform.TransInt32(read.Content, 2), bytes: read.Content, index: 6));
		}
		return OperateResult.CreateSuccessResult(encoding.GetString(read.Content));
	}

	public override async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		if (string.IsNullOrEmpty(value))
		{
			value = string.Empty;
		}
		byte[] bytes = encoding.GetBytes(value);
		OperateResult write = await WriteAsync(address + ".LEN", bytes.Length);
		if (!write.IsSuccess)
		{
			return write;
		}
		return await WriteTagAsync(value: SoftBasic.ArrayExpandToLengthEven(bytes), address: address + ".DATA[0]", typeCode: 194, length: bytes.Length);
	}

	public override string ToString()
	{
		return $"AllenBradleyConnectedCipNet[{IpAddress}:{Port}]";
	}
}
