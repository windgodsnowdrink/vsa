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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 基于连接的对象访问的CIP协议的实现，用于对Omron PLC进行标签的数据读写，对数组，多维数组进行读写操作，支持的数据类型请参照API文档手册。
/// The implementation of the CIP protocol based on connected object access is used to read and write tag data to Omron PLC,
/// and read and write arrays and multidimensional arrays. For the supported data types, please refer to the API documentation manual.
/// </summary>
/// <remarks>
/// 支持普通标签的读写，类型要和标签对应上。如果标签是数组，例如 A 是 INT[0...9] 那么Read("A", 1)，返回的是10个short所有字节数组。
/// 如果需要返回10个长度的short数组，请调用 ReadInt16("A[0], 10"); 地址必须写 "A[0]"，不能写 "A" , 如需要读取结构体，参考
/// </remarks>
public class OmronConnectedCipNet : NetworkConnectedCip, IReadWriteCip, IReadWriteNet
{
	/// <summary>
	/// 当前产品的型号信息
	/// Model information of the current product
	/// </summary>
	public string ProductName { get; private set; }

	/// <summary>
	/// 获取或设置不通信时超时的时间，默认02，为 32 秒，设置06 时为8分多，计算方法为 (2的x次方乘以8) 的秒数
	/// Get or set the timeout time when there is no communication. The default is 02, which is 32 seconds, and when 06 is set, it is more than 8 minutes. The calculation method is (2 times the power of x times 8) seconds.
	/// </summary>
	public byte ConnectionTimeoutMultiplier { get; set; } = 2;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public OmronConnectedCipNet()
	{
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 根据指定的IP及端口来实例化这个连接对象
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口号信息</param>
	public OmronConnectedCipNet(string ipAddress, int port = 44818)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override byte[] GetLargeForwardOpen(ushort connectionID)
	{
		uint value = (base.TOConnectionId = (uint)(-2130837503 + connectionID));
		byte[] array = "\r\n00 00 00 00 00 00 02 00 00 00 00 00 b2 00 34 00\r\n5b 02 20 06 24 01 0e 9c 02 00 00 80 01 00 fe 80\r\n02 00 1b 05 30 a7 2b 03 02 00 00 00 80 84 1e 00\r\ncc 07 00 42 80 84 1e 00 cc 07 00 42 a3 03 20 02\r\n24 01 2c 01".ToHexBytes();
		BitConverter.GetBytes((uint)(-2147483646 + connectionID)).CopyTo(array, 24);
		BitConverter.GetBytes(value).CopyTo(array, 28);
		BitConverter.GetBytes((ushort)(2 + connectionID)).CopyTo(array, 32);
		BitConverter.GetBytes((ushort)4105).CopyTo(array, 34);
		HslHelper.HslRandom.GetBytes(4).CopyTo(array, 36);
		array[40] = ConnectionTimeoutMultiplier;
		return array;
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult operateResult = base.InitializationOnConnect();
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, base.SessionHandle, GetAttributeAll()), hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		if (operateResult2.Content.Length > 59 && operateResult2.Content.Length >= 59 + operateResult2.Content[58])
		{
			ProductName = Encoding.UTF8.GetString(operateResult2.Content, 59, operateResult2.Content[58]);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult ini = await base.InitializationOnConnectAsync().ConfigureAwait(continueOnCapturedContext: false);
		if (!ini.IsSuccess)
		{
			return ini;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, base.SessionHandle, GetAttributeAll()), hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		if (read.Content.Length > 59 && read.Content.Length >= 59 + read.Content[58])
		{
			ProductName = Encoding.UTF8.GetString(read.Content, 59, read.Content[58]);
		}
		return OperateResult.CreateSuccessResult();
	}

	private byte[] GetAttributeAll()
	{
		return "00 00 00 00 00 00 02 00 00 00 00 00 b2 00 06 00 01 02 20 01 24 01".ToHexBytes();
	}

	private OperateResult<byte[]> BuildReadCommand(string[] address, ushort[] length)
	{
		try
		{
			List<byte[]> list = new List<byte[]>();
			for (int i = 0; i < address.Length; i++)
			{
				list.Add(AllenBradleyHelper.PackRequsetRead(address[i], length[i], isConnectedAddress: true));
			}
			return OperateResult.CreateSuccessResult(PackCommandService(list.ToArray()));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
		}
	}

	private OperateResult<byte[]> BuildWriteCommand(string address, ushort typeCode, byte[] data, int length = 1)
	{
		try
		{
			return OperateResult.CreateSuccessResult(PackCommandService(AllenBradleyHelper.PackRequestWrite(address, typeCode, data, length, isConnectedAddress: true)));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
		}
	}

	private OperateResult<byte[], ushort, bool> ReadWithType(string[] address, ushort[] length)
	{
		OperateResult<byte[]> operateResult = BuildReadCommand(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(operateResult2);
		}
		OperateResult operateResult3 = AllenBradleyHelper.CheckResponse(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(operateResult3);
		}
		return NetworkConnectedCip.ExtractActualData(operateResult2.Content, isRead: true);
	}

	/// <summary>
	/// 使用CIP报文和服务器进行核心的数据交换
	/// </summary>
	/// <param name="cips">Cip commands</param>
	/// <returns>Results Bytes</returns>
	public OperateResult<byte[]> ReadCipFromServer(params byte[][] cips)
	{
		byte[] send = PackCommandService(cips.ToArray());
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = AllenBradleyHelper.CheckResponse(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content);
	}

	public OperateResult<T> ReadStruct<T>(string address) where T : struct
	{
		OperateResult<byte[]> operateResult = Read(address, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(operateResult);
		}
		return HslHelper.ByteArrayToStruct<T>(operateResult.Content.RemoveBegin(2));
	}

	private async Task<OperateResult<byte[], ushort, bool>> ReadWithTypeAsync(string[] address, ushort[] length)
	{
		OperateResult<byte[]> command = BuildReadCommand(address, length);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(command);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(read);
		}
		OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(check);
		}
		return NetworkConnectedCip.ExtractActualData(read.Content, isRead: true);
	}

	/// <summary>
	/// 使用CIP报文和服务器进行核心的数据交换
	/// </summary>
	/// <param name="cips">Cip commands</param>
	/// <returns>Results Bytes</returns>
	public async Task<OperateResult<byte[]>> ReadCipFromServerAsync(params byte[][] cips)
	{
		byte[] command = PackCommandService(cips.ToArray());
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(check);
		}
		return OperateResult.CreateSuccessResult(read.Content);
	}

	public async Task<OperateResult<T>> ReadStructAsync<T>(string address) where T : struct
	{
		OperateResult<byte[]> read = await ReadAsync(address, 1);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(read);
		}
		return HslHelper.ByteArrayToStruct<T>(read.Content.RemoveBegin(2));
	}

	/// <summary>
	/// 获取传递的最大长度的字节信息
	/// </summary>
	/// <returns>字节长度</returns>
	protected virtual int GetMaxTransferBytes()
	{
		return 1988;
	}

	private int GetLengthFromRemain(ushort dataType, int length)
	{
		if (dataType == 193 || dataType == 194 || dataType == 198 || dataType == 211)
		{
			return Math.Min(length, GetMaxTransferBytes());
		}
		if (dataType == 199 || dataType == 195)
		{
			return Math.Min(length, GetMaxTransferBytes() / 2);
		}
		if (dataType == 196 || dataType == 200 || dataType == 202)
		{
			return Math.Min(length, GetMaxTransferBytes() / 4);
		}
		return Math.Min(length, GetMaxTransferBytes() / 8);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		if (length == 1)
		{
			OperateResult<byte[], ushort, bool> operateResult = ReadWithType(new string[1] { address }, new ushort[1] { length });
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			return OperateResult.CreateSuccessResult(operateResult.Content1);
		}
		int num = 0;
		int num2 = 0;
		string format = "[{0}]";
		List<byte> list = new List<byte>();
		Match match = Regex.Match(address, "\\[[0-9]+\\]$");
		if (match.Success)
		{
			address = address.Remove(match.Index, match.Length);
			num2 = int.Parse(match.Value.Substring(1, match.Value.Length - 2));
		}
		else
		{
			Match match2 = Regex.Match(address, "\\[[0-9]+,[0-9]+\\]$");
			if (match2.Success)
			{
				address = address.Remove(match2.Index, match2.Length);
				string value = Regex.Matches(match2.Value, "[0-9]+")[1].Value;
				format = match2.Value.Replace(value + "]", "{0}]");
				num2 = int.Parse(value);
			}
		}
		ushort dataType = 0;
		while (num < length)
		{
			if (num == 0)
			{
				ushort num3 = Math.Min(length, (ushort)248);
				OperateResult<byte[], ushort, bool> operateResult2 = ReadWithType(new string[1] { address + string.Format(format, num2) }, new ushort[1] { num3 });
				if (!operateResult2.IsSuccess)
				{
					return OperateResult.CreateFailedResult<byte[]>(operateResult2);
				}
				dataType = operateResult2.Content2;
				num += num3;
				num2 += num3;
				list.AddRange(operateResult2.Content1);
			}
			else
			{
				ushort num4 = (ushort)GetLengthFromRemain(dataType, length - num);
				OperateResult<byte[], ushort, bool> operateResult3 = ReadWithType(new string[1] { address + string.Format(format, num2) }, new ushort[1] { num4 });
				if (!operateResult3.IsSuccess)
				{
					return OperateResult.CreateFailedResult<byte[]>(operateResult3);
				}
				num += num4;
				num2 += num4;
				list.AddRange(operateResult3.Content1);
			}
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	[HslMqttApi("ReadMultiAddress", "")]
	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"， 长度为 [1, 5]，返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Read the data information of multiple addresses in batches. For example, I can read the data "A", "B[0]" of two tags,
	/// the length is [1, 5], and the return is an entire byte array, and I need to do it myself Parsing
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">如果是数组，就为数组长度 -&gt; In the case of arrays, the length of the array</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result object</returns>
	public OperateResult<byte[]> Read(string[] address, ushort[] length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[], ushort, bool> operateResult = ReadWithType(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content1);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 读取bool数据信息，如果读取的是单bool变量，就直接写变量名，如果是 bool 数组，就
	/// Read a single bool data information, if it is a single bool variable, write the variable name directly,
	/// if it is a value of a bool array composed of int, it is always accessed with "i=" at the beginning, for example, "i=A[0]"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">读取的数组长度信息</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (length == 1 && !Regex.IsMatch(address, "\\[[0-9]+\\]$"))
		{
			OperateResult<byte[]> operateResult = Read(address, length);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult);
			}
			return OperateResult.CreateSuccessResult(SoftBasic.ByteToBoolArray(operateResult.Content));
		}
		OperateResult<byte[]> operateResult2 = Read(address, length);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.Select((byte m) => m != 0).Take(length).ToArray());
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 读取PLC的byte类型的数据
	/// Read the byte type of PLC data
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	/// <summary>
	/// 从PLC里读取一个指定标签名的原始数据信息及其数据类型信息
	/// Read the original data information of a specified tag name and its data type information from the PLC
	/// </summary>
	/// <param name="address">PLC的标签地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <returns>包含原始数据信息及数据类型的结果对象</returns>
	/// <remarks>
	/// 数据类型的定义，可以参考  的常量资源信息。
	/// </remarks>
	public OperateResult<ushort, byte[]> ReadTag(string address, ushort length = 1)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		OperateResult<byte[], ushort, bool> operateResult = ReadWithType(new string[1] { address }, new ushort[1] { length });
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<ushort, byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content2, operateResult.Content1);
	}

	[HslMqttApi(Description = "获取PLC的型号信息")]
	/// <summary>
	/// 从PLC里读取当前PLC的型号信息
	/// Read the current PLC model information from the PLC
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <returns>型号数据信息</returns>
	public OperateResult<string> ReadPlcType()
	{
		return OperateResult.CreateSuccessResult(ProductName);
	}

	/// <summary>
	/// 读取bool数据信息，如果读取的是单bool变量，就直接写变量名，如果是 bool 数组，就
	/// Read a single bool data information, if it is a single bool variable, write the variable name directly,
	/// if it is a value of a bool array composed of int, it is always accessed with "i=" at the beginning, for example, "i=A[0]"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">读取的数组长度信息</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (length == 1 && !Regex.IsMatch(address, "\\[[0-9]+\\]$"))
		{
			OperateResult<byte[]> read = await ReadAsync(address, length);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(read);
			}
			return OperateResult.CreateSuccessResult(SoftBasic.ByteToBoolArray(read.Content));
		}
		OperateResult<byte[]> read2 = await ReadAsync(address, length);
		if (!read2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read2);
		}
		return OperateResult.CreateSuccessResult(read2.Content.Select((byte m) => m != 0).Take(length).ToArray());
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		if (length == 1)
		{
			OperateResult<byte[], ushort, bool> read = await ReadWithTypeAsync(new string[1] { address }, new ushort[1] { length });
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(read);
			}
			return OperateResult.CreateSuccessResult(read.Content1);
		}
		int count = 0;
		int index = 0;
		string format = "[{0}]";
		List<byte> array = new List<byte>();
		Match match = Regex.Match(address, "\\[[0-9]+\\]$");
		if (match.Success)
		{
			address = address.Remove(match.Index, match.Length);
			index = int.Parse(match.Value.Substring(1, match.Value.Length - 2));
		}
		else
		{
			Match match2 = Regex.Match(address, "\\[[0-9]+,[0-9]+\\]$");
			if (match2.Success)
			{
				address = address.Remove(match2.Index, match2.Length);
				string index2 = Regex.Matches(match2.Value, "[0-9]+")[1].Value;
				format = match2.Value.Replace(index2 + "]", "{0}]");
				index = int.Parse(index2);
			}
		}
		ushort dataType = 0;
		while (count < length)
		{
			if (count == 0)
			{
				ushort first = Math.Min(length, (ushort)248);
				OperateResult<byte[], ushort, bool> read2 = await ReadWithTypeAsync(new string[1] { address + string.Format(format, index) }, new ushort[1] { first });
				if (!read2.IsSuccess)
				{
					return OperateResult.CreateFailedResult<byte[]>(read2);
				}
				dataType = read2.Content2;
				count += first;
				index += first;
				array.AddRange(read2.Content1);
			}
			else
			{
				ushort len = (ushort)GetLengthFromRemain(dataType, length - count);
				OperateResult<byte[], ushort, bool> read3 = await ReadWithTypeAsync(new string[1] { address + string.Format(format, index) }, new ushort[1] { len });
				if (!read3.IsSuccess)
				{
					return OperateResult.CreateFailedResult<byte[]>(read3);
				}
				count += len;
				index += len;
				array.AddRange(read3.Content1);
			}
		}
		return OperateResult.CreateSuccessResult(array.ToArray());
	}

	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"， 长度为 [1, 5]，返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Read the data information of multiple addresses in batches. For example, I can read the data "A", "B[0]" of two tags,
	/// the length is [1, 5], and the return is an entire byte array, and I need to do it myself Parsing
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">如果是数组，就为数组长度 -&gt; In the case of arrays, the length of the array</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result object</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string[] address, ushort[] length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[], ushort, bool> read = await ReadWithTypeAsync(address, length);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content1);
	}

	/// <summary>
	/// 读取PLC的byte类型的数据
	/// Read the byte type of PLC data
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	/// <summary>
	/// 从PLC里读取一个指定标签名的原始数据信息及其数据类型信息
	/// Read the original data information of a specified tag name and its data type information from the PLC
	/// </summary>
	/// <param name="address">PLC的标签地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <returns>包含原始数据信息及数据类型的结果对象</returns>
	/// <remarks>
	/// 数据类型的定义，可以参考  的常量资源信息。
	/// </remarks>
	public async Task<OperateResult<ushort, byte[]>> ReadTagAsync(string address, ushort length = 1)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		OperateResult<byte[], ushort, bool> read = await ReadWithTypeAsync(new string[1] { address }, new ushort[1] { length });
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<ushort, byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content2, read.Content1);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 当前写入字节数组使用数据类型 0xD1 写入，如果其他的字节类型需要调用  方法来实现。
	/// The currently written byte array is written using the data type 0xD1. If other byte types need to be called  Method to achieve.
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="value">值</param>
	/// <returns>写入结果值</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return WriteTag(address, 209, value, (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length);
	}

	/// <summary>
	/// 使用指定的类型写入指定的节点数据，类型信息参考API文档，地址支持协议类型代号信息，例如 "type=0xD1;A"
	/// Use the specified type to write the specified node data. For type information, refer to the API documentation. The address supports protocol type code information, such as "type=0xD1;A"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="typeCode">类型代码，详细参见上的常用字段 -&gt;  Type code, see the commonly used Fields section on the  in detail</param>
	/// <param name="value">实际的数据值 -&gt; The actual data value</param>
	/// <param name="length">如果节点是数组，就是数组长度 -&gt; If the node is an array, it is the array length</param>
	/// <returns>是否写入成功 -&gt; Whether to write successfully</returns>
	/// <remarks>
	/// 关于参数 length 的含义，表示的是地址长度，一般的标量数据都是 1，如果PLC有个标签是 A，数据类型为 byte[10]，那我们写入 3 个byte就是 WriteTag( "A[5]", 0xD1, new byte[]{1,2,3}, 3 );
	/// Regarding the meaning of the parameter length, it represents the address length. The general scalar data is 1. If the PLC has a tag of A and the data type is byte[10], then we write 3 bytes as WriteTag( "A[5 ]", 0xD1, new byte[]{1,2,3}, 3 );
	/// </remarks>
	public virtual OperateResult WriteTag(string address, ushort typeCode, byte[] value, int length = 1)
	{
		OperateResult<int> operateResult = HslHelper.ExtractParameterWithDefault(ref address, "type", typeCode);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		typeCode = (ushort)operateResult.Content;
		OperateResult<byte[]> operateResult2 = BuildWriteCommand(address, typeCode, value, length);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult<byte[]> operateResult3 = ReadFromCoreServer(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		OperateResult operateResult4 = AllenBradleyHelper.CheckResponse(operateResult3.Content);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult4);
		}
		return NetworkConnectedCip.ExtractActualData(operateResult3.Content, isRead: false);
	}

	/// <summary>
	/// 当前写入字节数组使用数据类型 0xD1 写入，如果其他的字节类型需要调用  方法来实现。
	/// The currently written byte array is written using the data type 0xD1. If other byte types need to be called  Method to achieve.
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="value">值</param>
	/// <returns>写入结果值</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await WriteTagAsync(address, 209, value, (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用指定的类型写入指定的节点数据，类型信息参考API文档，地址支持协议类型代号信息，例如 "type=0xD1;A"
	/// Use the specified type to write the specified node data. For type information, refer to the API documentation. The address supports protocol type code information, such as "type=0xD1;A"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="typeCode">类型代码，详细参见上的常用字段 -&gt;  Type code, see the commonly used Fields section on the  in detail</param>
	/// <param name="value">实际的数据值 -&gt; The actual data value</param>
	/// <param name="length">如果节点是数组，就是数组长度 -&gt; If the node is an array, it is the array length</param>
	/// <returns>是否写入成功 -&gt; Whether to write successfully</returns>
	/// <remarks>
	/// 关于参数 length 的含义，表示的是地址长度，一般的标量数据都是 1，如果PLC有个标签是 A，数据类型为 byte[10]，那我们写入 3 个byte就是 WriteTag( "A[5]", 0xD1, new byte[]{1,2,3}, 3 );
	/// Regarding the meaning of the parameter length, it represents the address length. The general scalar data is 1. If the PLC has a tag of A and the data type is byte[10], then we write 3 bytes as WriteTag( "A[5 ]", 0xD1, new byte[]{1,2,3}, 3 );
	/// </remarks>
	public virtual async Task<OperateResult> WriteTagAsync(string address, ushort typeCode, byte[] value, int length = 1)
	{
		OperateResult<int> extraType = HslHelper.ExtractParameterWithDefault(ref address, "type", typeCode);
		if (!extraType.IsSuccess)
		{
			return extraType;
		}
		typeCode = (ushort)extraType.Content;
		OperateResult<byte[]> command = BuildWriteCommand(address, typeCode, value, length);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(check);
		}
		return NetworkConnectedCip.ExtractActualData(read.Content, isRead: false);
	}

	[HslMqttApi("ReadInt16Array", "")]
	public override OperateResult<short[]> ReadInt16(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransInt16(m, 0, length));
	}

	[HslMqttApi("ReadUInt16Array", "")]
	public override OperateResult<ushort[]> ReadUInt16(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransUInt16(m, 0, length));
	}

	[HslMqttApi("ReadInt32Array", "")]
	public override OperateResult<int[]> ReadInt32(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransInt32(m, 0, length));
	}

	[HslMqttApi("ReadUInt32Array", "")]
	public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransUInt32(m, 0, length));
	}

	[HslMqttApi("ReadFloatArray", "")]
	public override OperateResult<float[]> ReadFloat(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransSingle(m, 0, length));
	}

	[HslMqttApi("ReadInt64Array", "")]
	public override OperateResult<long[]> ReadInt64(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransInt64(m, 0, length));
	}

	[HslMqttApi("ReadUInt64Array", "")]
	public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransUInt64(m, 0, length));
	}

	[HslMqttApi("ReadDoubleArray", "")]
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransDouble(m, 0, length));
	}

	public OperateResult<string> ReadString(string address)
	{
		return ReadString(address, 1, Encoding.UTF8);
	}

	[HslMqttApi("ReadString", "")]
	/// <summary>
	/// 读取字符串数据，默认为UTF-8编码
	/// Read string data, default is UTF-8 encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	public override OperateResult<string> ReadString(string address, ushort length)
	{
		return ReadString(address, length, Encoding.UTF8);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		OperateResult<byte[]> operateResult = Read(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		return ExtraStringContent(operateResult.Content, encoding);
	}

	private OperateResult<string> ExtraStringContent(byte[] content, Encoding encoding)
	{
		try
		{
			if (content.Length >= 2)
			{
				int count = base.ByteTransform.TransUInt16(content, 0);
				return OperateResult.CreateSuccessResult(encoding.GetString(content, 2, count));
			}
			return OperateResult.CreateSuccessResult(encoding.GetString(content));
		}
		catch (Exception ex)
		{
			return new OperateResult<string>("Parse string failed: " + ex.Message + " Source: " + content.ToHexString(' '));
		}
	}

	public override async Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransInt16(m, 0, length));
	}

	public override async Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransUInt16(m, 0, length));
	}

	public override async Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransInt32(m, 0, length));
	}

	public override async Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransUInt32(m, 0, length));
	}

	public override async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransSingle(m, 0, length));
	}

	public override async Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransInt64(m, 0, length));
	}

	public override async Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransUInt64(m, 0, length));
	}

	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransDouble(m, 0, length));
	}

	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		return await ReadStringAsync(address, 1, Encoding.UTF8);
	}

	/// <summary>
	/// 读取字符串数据，默认为UTF-8编码
	/// Read string data, default is UTF-8 encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
	{
		return await ReadStringAsync(address, length, Encoding.UTF8);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		OperateResult<byte[]> read = await ReadAsync(address, length).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		return ExtraStringContent(read.Content, encoding);
	}

	[HslMqttApi("WriteInt16Array", "")]
	public override OperateResult Write(string address, short[] values)
	{
		return WriteTag(address, 195, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteUInt16Array", "")]
	public override OperateResult Write(string address, ushort[] values)
	{
		return WriteTag(address, 199, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteInt32Array", "")]
	public override OperateResult Write(string address, int[] values)
	{
		return WriteTag(address, 196, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteUInt32Array", "")]
	public override OperateResult Write(string address, uint[] values)
	{
		return WriteTag(address, 200, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteFloatArray", "")]
	public override OperateResult Write(string address, float[] values)
	{
		return WriteTag(address, 202, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteInt64Array", "")]
	public override OperateResult Write(string address, long[] values)
	{
		return WriteTag(address, 197, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteUInt64Array", "")]
	public override OperateResult Write(string address, ulong[] values)
	{
		return WriteTag(address, 201, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteDoubleArray", "")]
	public override OperateResult Write(string address, double[] values)
	{
		return WriteTag(address, 203, base.ByteTransform.TransByte(values), values.Length);
	}

	[HslMqttApi("WriteString", "")]
	public override OperateResult Write(string address, string value)
	{
		return Write(address, value, Encoding.UTF8);
	}

	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		byte[] bytes = (string.IsNullOrEmpty(value) ? new byte[0] : encoding.GetBytes(value));
		return WriteTag(address, 208, SoftBasic.SpliceArray<byte>(BitConverter.GetBytes((ushort)bytes.Length), bytes));
	}

	[HslMqttApi("WriteBool", "")]
	public override OperateResult Write(string address, bool value)
	{
		return WriteTag(address, 193, (!value) ? new byte[2] : new byte[2] { 255, 255 });
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		return WriteTag(address, 193, value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length);
	}

	[HslMqttApi("WriteByte", "")]
	public OperateResult Write(string address, byte value)
	{
		return WriteTag(address, 194, new byte[1] { value });
	}

	public override async Task<OperateResult> WriteAsync(string address, short[] values)
	{
		return await WriteTagAsync(address, 195, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, ushort[] values)
	{
		return await WriteTagAsync(address, 199, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, int[] values)
	{
		return await WriteTagAsync(address, 196, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, uint[] values)
	{
		return await WriteTagAsync(address, 200, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, float[] values)
	{
		return await WriteTagAsync(address, 202, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, long[] values)
	{
		return await WriteTagAsync(address, 197, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, ulong[] values)
	{
		return await WriteTagAsync(address, 201, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await WriteTagAsync(address, 203, base.ByteTransform.TransByte(values), values.Length);
	}

	public override async Task<OperateResult> WriteAsync(string address, string value)
	{
		return await WriteAsync(address, value, Encoding.UTF8);
	}

	public override async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		byte[] buffer = (string.IsNullOrEmpty(value) ? new byte[0] : encoding.GetBytes(value));
		return await WriteTagAsync(address, 208, SoftBasic.SpliceArray<byte>(BitConverter.GetBytes((ushort)buffer.Length), buffer));
	}

	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await WriteTagAsync(address, 193, (!value) ? new byte[2] : new byte[2] { 255, 255 }).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		return await WriteTagAsync(address, 193, value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		return await WriteTagAsync(address, 194, new byte[1] { value }).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	public OperateResult<DateTime> ReadDate(string address)
	{
		return AllenBradleyHelper.ReadDate(this, address);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteDate(string address, DateTime date)
	{
		return AllenBradleyHelper.WriteDate(this, address, date);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteTimeAndDate(string address, DateTime date)
	{
		return AllenBradleyHelper.WriteTimeAndDate(this, address, date);
	}

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	public OperateResult<TimeSpan> ReadTime(string address)
	{
		return AllenBradleyHelper.ReadTime(this, address);
	}

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteTime(string address, TimeSpan time)
	{
		return AllenBradleyHelper.WriteTime(this, address, time);
	}

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteTimeOfDate(string address, TimeSpan timeOfDate)
	{
		return AllenBradleyHelper.WriteTimeOfDate(this, address, timeOfDate);
	}

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	public async Task<OperateResult<DateTime>> ReadDateAsync(string address)
	{
		return await AllenBradleyHelper.ReadDateAsync(this, address).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteDateAsync(string address, DateTime date)
	{
		return await AllenBradleyHelper.WriteDateAsync(this, address, date).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteTimeAndDateAsync(string address, DateTime date)
	{
		return await AllenBradleyHelper.WriteTimeAndDateAsync(this, address, date).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	public async Task<OperateResult<TimeSpan>> ReadTimeAsync(string address)
	{
		return await AllenBradleyHelper.ReadTimeAsync(this, address).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteTimeAsync(string address, TimeSpan time)
	{
		return await AllenBradleyHelper.WriteTimeAsync(this, address, time).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteTimeOfDateAsync(string address, TimeSpan timeOfDate)
	{
		return await AllenBradleyHelper.WriteTimeOfDateAsync(this, address, timeOfDate).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"OmronConnectedCipNet[{IpAddress}:{Port}]";
	}
}
