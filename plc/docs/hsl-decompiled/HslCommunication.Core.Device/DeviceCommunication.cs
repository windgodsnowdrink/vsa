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
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Device;

/// <summary>
/// 所有设备的基类信息
/// </summary>
public class DeviceCommunication : BinaryCommunication, IReadWriteDevice, IReadWriteNet, IDisposable
{
	private bool disposedValue = false;

	private IByteTransform byteTransform = new RegularByteTransform();

	/// <summary>
	/// 一个字单位的数据表示的地址长度，西门子为2，三菱，欧姆龙，modbusTcp就为1，AB PLC无效
	/// The address length represented by one word of data, Siemens is 2, Mitsubishi, Omron, modbusTcp is 1, AB PLC is invalid
	/// </summary>
	/// <remarks>
	/// 对设备来说，一个地址的数据对应的字节数，或是1个字节或是2个字节，4个字节，通常是这四个选择，当设置为0时，则表示4字节的地址长度信息
	/// For the device, the number of bytes corresponding to the data of an address, either 1 byte or 2 bytes, 4 bytes, usually these four choices, when set to 0, it means 4 words Section address length information
	/// </remarks>
	protected ushort WordLength { get; set; } = 1;

	/// <summary>
	/// 当前的数据变换机制，当你需要从字节数据转换类型数据的时候需要。
	/// The current data transformation mechanism is required when you need to convert type data from byte data.
	/// </summary>
	/// <remarks>
	/// 在HSL里提供了三种数据变换机制，分别是 , ,
	/// ，各自的属性也可以自定调整，基本满足所有的情况使用。
	/// Three data transformation mechanisms are provided in HSL, namely , ,
	/// , and their respective  property can also be adjusted by itself, basically satisfying all situations.
	/// </remarks>
	public IByteTransform ByteTransform
	{
		get
		{
			return byteTransform;
		}
		set
		{
			if (value == null)
			{
				value = new RegularByteTransform();
			}
			byteTransform = value;
		}
	}

	/// <summary>
	/// 一个字单位的数据表示的地址长度，西门子为2，三菱，欧姆龙，modbusTcp就为1，AB PLC无效
	/// The address length represented by one word of data, Siemens is 2, Mitsubishi, Omron, modbusTcp is 1, AB PLC is invalid
	/// </summary>
	/// <param name="address">读取的设备的地址信息</param>
	/// <param name="length">读取的数据长度信息</param>
	/// <param name="dataTypeLength">数据类型的字节长度信息，比如short, 就是2，int,float就是4</param>
	/// <remarks>
	/// 对设备来说，一个地址的数据对应的字节数，或是1个字节或是2个字节，通常是这两个选择。
	/// 当前也可以重写来根据不同的地址动态控制不同的地址长度，比如有的地址是一个地址一个字节的，有的地址是一个地址两个字节的
	/// </remarks>
	protected virtual ushort GetWordLength(string address, int length, int dataTypeLength)
	{
		if (WordLength == 0)
		{
			int num = length * dataTypeLength * 2 / 4;
			return (ushort)((num == 0) ? 1 : ((ushort)num));
		}
		return (ushort)(WordLength * length * dataTypeLength);
	}

	/// <summary>
	/// 根据输入的地址，长度信息构建一个读取数据的报文命令
	/// </summary>
	/// <param name="address">读取的地址</param>
	/// <param name="length">读取的长度信息</param>
	/// <param name="isBit">是否位操作</param>
	/// <returns>等待发送的命令报文</returns>
	protected virtual OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return new OperateResult<List<byte[]>>(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 构建一个写入数据的报文命令，子类需要重写该方法支持不同设备的写入规则
	/// </summary>
	/// <param name="address">写入的地址</param>
	/// <param name="value">写入的数据</param>
	/// <returns>等待发送的命令报文</returns>
	protected virtual OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 构建一个写入数据的报文命令，子类需要重写该方法支持不同设备的写入规则
	/// </summary>
	/// <param name="address">写入的地址</param>
	/// <param name="value">写入的数据</param>
	/// <returns>等待发送的命令报文</returns>
	protected virtual OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 根据设备返回的报文，判断当前的报文是否是正确的报文，子类需要重写该方法支持不同设备的报文解析规则
	/// </summary>
	/// <param name="address">读取或是写入的地址</param>
	/// <param name="buildCommand">构建的读写命令</param>
	/// <param name="content">返回的报文信息</param>
	/// <returns>是否正常的结果数据对象</returns>
	protected virtual OperateResult CheckDeviceResponse(string address, byte[] buildCommand, byte[] content)
	{
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 从返回的数据中提取出读取的数据信息，子类需要重写该方法来支持不同设备的解析规则
	/// </summary>
	/// <param name="address">读取或是写入的地址</param>
	/// <param name="buildCommand">构建的读写命令</param>
	/// <param name="content">返回的报文信息</param>
	/// <returns>是否正常的结果数据对象</returns>
	protected virtual OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		return OperateResult.CreateSuccessResult(content);
	}

	/// <summary>
	/// 从返回的数据中提取出读取的数据信息，子类需要重写该方法来支持不同设备的解析规则
	/// </summary>
	/// <param name="address">读取或是写入的地址</param>
	/// <param name="buildCommand">构建的读写命令</param>
	/// <param name="content">返回的报文信息</param>
	/// <returns>是否正常的结果数据对象</returns>
	protected virtual OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		return new OperateResult<bool[]>(StringResources.Language.NotSupportedFunction);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public virtual OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<List<byte[]>> operateResult = BuildDeviceReadCommand(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		List<byte> list = new List<byte>();
		for (int i = 0; i < operateResult.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content[i]);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult2);
			}
			OperateResult operateResult3 = CheckDeviceResponse(address, operateResult.Content[i], operateResult2.Content);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult3);
			}
			OperateResult<byte[]> operateResult4 = ParseDeviceReadResult(address, operateResult.Content[i], operateResult2.Content);
			if (!operateResult4.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult4);
			}
			list.AddRange(operateResult4.Content);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public virtual OperateResult Write(string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildDeviceWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return CheckDeviceResponse(address, operateResult.Content, operateResult2.Content);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public virtual OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<List<byte[]>> operateResult = BuildDeviceReadCommand(address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		List<bool> list = new List<bool>();
		for (int i = 0; i < operateResult.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content[i]);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult2);
			}
			OperateResult operateResult3 = CheckDeviceResponse(address, operateResult.Content[i], operateResult2.Content);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult3);
			}
			OperateResult<bool[]> operateResult4 = ParseDeviceReadBoolResult(address, operateResult.Content[i], operateResult2.Content, length);
			if (!operateResult4.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult4);
			}
			list.AddRange(operateResult4.Content);
		}
		bool[] array = list.ToArray();
		if (array.Length > length)
		{
			return OperateResult.CreateSuccessResult(array.SelectBegin(length));
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	[HslMqttApi("ReadBool", "")]
	/// <summary>
	/// 读取单个的数据信息
	/// Read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的 bool 值</returns>
	public virtual OperateResult<bool> ReadBool(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadBool(address, 1));
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public virtual OperateResult Write(string address, bool[] value)
	{
		OperateResult<byte[]> operateResult = BuildDeviceWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return CheckDeviceResponse(address, operateResult.Content, operateResult2.Content);
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 写入单个的数据，返回是否成功
	/// Write a single  data, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public virtual OperateResult Write(string address, bool value)
	{
		return Write(address, new bool[1] { value });
	}

	public OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new()
	{
		return ReadWriteNetHelper.ReadCustomer<T>(this, address);
	}

	public OperateResult<T> ReadCustomer<T>(string address, T obj) where T : IDataTransfer, new()
	{
		return ReadWriteNetHelper.ReadCustomer(this, address, obj);
	}

	public OperateResult WriteCustomer<T>(string address, T data) where T : IDataTransfer, new()
	{
		return ReadWriteNetHelper.WriteCustomer(this, address, data);
	}

	public virtual OperateResult<T> Read<T>() where T : class, new()
	{
		return HslReflectionHelper.Read<T>(this);
	}

	public virtual OperateResult Write<T>(T data) where T : class, new()
	{
		return HslReflectionHelper.Write(data, this);
	}

	public virtual OperateResult<T> ReadStruct<T>(string address, ushort length) where T : class, new()
	{
		return ReadWriteNetHelper.ReadStruct<T>(this, address, length, ByteTransform);
	}

	[HslMqttApi("ReadInt16", "")]
	/// <summary>
	/// 读取16位的有符号的整型数据
	/// Read 16-bit signed integer data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的short数据</returns>
	public virtual OperateResult<short> ReadInt16(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadInt16(address, 1));
	}

	[HslMqttApi("ReadInt16Array", "")]
	/// <summary>
	/// 读取16位的有符号整型数组
	/// Read 16-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的short数组</returns>
	public virtual OperateResult<short[]> ReadInt16(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 1)), (byte[] m) => ByteTransform.TransInt16(m, 0, length));
	}

	[HslMqttApi("ReadUInt16", "")]
	/// <summary>
	/// 读取16位的无符号整型
	/// Read 16-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ushort数据</returns>
	public virtual OperateResult<ushort> ReadUInt16(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadUInt16(address, 1));
	}

	[HslMqttApi("ReadUInt16Array", "")]
	/// <summary>
	/// 读取16位的无符号整型数组
	/// Read 16-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的ushort数组</returns>
	public virtual OperateResult<ushort[]> ReadUInt16(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 1)), (byte[] m) => ByteTransform.TransUInt16(m, 0, length));
	}

	[HslMqttApi("ReadInt32", "")]
	/// <summary>
	/// 读取32位的有符号整型
	/// Read 32-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的int数据</returns>
	public virtual OperateResult<int> ReadInt32(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadInt32(address, 1));
	}

	[HslMqttApi("ReadInt32Array", "")]
	/// <summary>
	/// 读取32位有符号整型数组
	/// Read 32-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的int数组</returns>
	public virtual OperateResult<int[]> ReadInt32(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 2)), (byte[] m) => ByteTransform.TransInt32(m, 0, length));
	}

	[HslMqttApi("ReadUInt32", "")]
	/// <summary>
	/// 读取32位的无符号整型
	/// Read 32-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的uint数据</returns>
	public virtual OperateResult<uint> ReadUInt32(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadUInt32(address, 1));
	}

	[HslMqttApi("ReadUInt32Array", "")]
	/// <summary>
	/// 读取32位的无符号整型数组
	/// Read 32-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的uint数组</returns>
	public virtual OperateResult<uint[]> ReadUInt32(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 2)), (byte[] m) => ByteTransform.TransUInt32(m, 0, length));
	}

	[HslMqttApi("ReadFloat", "")]
	/// <summary>
	/// 读取单浮点数据
	/// Read single floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的float数据</returns>
	public virtual OperateResult<float> ReadFloat(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadFloat(address, 1));
	}

	[HslMqttApi("ReadFloatArray", "")]
	/// <summary>
	/// 读取单浮点精度的数组
	/// Read single floating point array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的float数组</returns>
	public virtual OperateResult<float[]> ReadFloat(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 2)), (byte[] m) => ByteTransform.TransSingle(m, 0, length));
	}

	[HslMqttApi("ReadInt64", "")]
	/// <summary>
	/// 读取64位的有符号整型
	/// Read 64-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的long数据</returns>
	public virtual OperateResult<long> ReadInt64(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadInt64(address, 1));
	}

	[HslMqttApi("ReadInt64Array", "")]
	/// <summary>
	/// 读取64位的有符号整型数组
	/// Read 64-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的long数组</returns>
	public virtual OperateResult<long[]> ReadInt64(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 4)), (byte[] m) => ByteTransform.TransInt64(m, 0, length));
	}

	[HslMqttApi("ReadUInt64", "")]
	/// <summary>
	/// 读取64位的无符号整型
	/// Read 64-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ulong数据</returns>
	public virtual OperateResult<ulong> ReadUInt64(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadUInt64(address, 1));
	}

	[HslMqttApi("ReadUInt64Array", "")]
	/// <summary>
	/// 读取64位的无符号整型的数组
	/// Read 64-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public virtual OperateResult<ulong[]> ReadUInt64(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 4)), (byte[] m) => ByteTransform.TransUInt64(m, 0, length));
	}

	[HslMqttApi("ReadDouble", "")]
	/// <summary>
	/// 读取双浮点的数据
	/// Read double floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的double数据</returns>
	public virtual OperateResult<double> ReadDouble(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadDouble(address, 1));
	}

	[HslMqttApi("ReadDoubleArray", "")]
	/// <summary>
	/// 读取双浮点数据的数组
	/// Read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	public virtual OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 4)), (byte[] m) => ByteTransform.TransDouble(m, 0, length));
	}

	[HslMqttApi("ReadString", "")]
	/// <summary>
	/// 读取字符串数据，默认为最常见的ASCII编码
	/// Read string data, default is the most common ASCII encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	public virtual OperateResult<string> ReadString(string address, ushort length)
	{
		return ReadString(address, length, Encoding.ASCII);
	}

	/// <summary>
	/// 使用指定的编码，读取字符串数据
	/// Reads string data using the specified encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <param name="encoding">指定的自定义的编码</param>
	/// <returns>带有成功标识的string数据</returns>
	public virtual OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => ByteTransform.TransString(m, 0, m.Length, encoding));
	}

	[HslMqttApi("WriteInt16Array", "")]
	/// <summary>
	/// 写入short数组，返回是否成功
	/// Write short array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, short[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteInt16", "")]
	/// <summary>
	/// 写入short数据，返回是否成功
	/// Write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, short value)
	{
		return Write(address, new short[1] { value });
	}

	[HslMqttApi("WriteUInt16Array", "")]
	/// <summary>
	/// 写入ushort数组，返回是否成功
	/// Write ushort array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, ushort[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteUInt16", "")]
	/// <summary>
	/// 写入ushort数据，返回是否成功
	/// Write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, ushort value)
	{
		return Write(address, new ushort[1] { value });
	}

	[HslMqttApi("WriteInt32Array", "")]
	/// <summary>
	/// 写入int[]数组，返回是否成功
	/// Write int array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, int[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteInt32", "")]
	/// <summary>
	/// 写入int数据，返回是否成功
	/// Write int data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, int value)
	{
		return Write(address, new int[1] { value });
	}

	[HslMqttApi("WriteUInt32Array", "")]
	/// <summary>
	/// 写入uint[]数组，返回是否成功
	/// Write uint array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, uint[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteUInt32", "")]
	/// <summary>
	/// 写入uint数据，返回是否成功
	/// Write uint data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, uint value)
	{
		return Write(address, new uint[1] { value });
	}

	[HslMqttApi("WriteFloatArray", "")]
	/// <summary>
	/// 写入float数组，返回是否成功
	/// Write float array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, float[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteFloat", "")]
	/// <summary>
	/// 写入float数据，返回是否成功
	/// Write float data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, float value)
	{
		return Write(address, new float[1] { value });
	}

	[HslMqttApi("WriteInt64Array", "")]
	/// <summary>
	/// 写入long数组，返回是否成功
	/// Write long array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, long[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteInt64", "")]
	/// <summary>
	/// 写入long数据，返回是否成功
	/// Write long data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, long value)
	{
		return Write(address, new long[1] { value });
	}

	[HslMqttApi("WriteUInt64Array", "")]
	/// <summary>
	/// 写入ulong数组，返回是否成功
	/// Write ulong array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, ulong[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteUInt64", "")]
	/// <summary>
	/// 写入ulong数据，返回是否成功
	/// Write ulong data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, ulong value)
	{
		return Write(address, new ulong[1] { value });
	}

	[HslMqttApi("WriteDoubleArray", "")]
	/// <summary>
	/// 写入double数组，返回是否成功
	/// Write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, double[] values)
	{
		return Write(address, ByteTransform.TransByte(values));
	}

	[HslMqttApi("WriteDouble", "")]
	/// <summary>
	/// 写入double数据，返回是否成功
	/// Write double data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, double value)
	{
		return Write(address, new double[1] { value });
	}

	[HslMqttApi("WriteString", "")]
	/// <summary>
	/// 写入字符串信息，编码为ASCII
	/// Write string information, encoded as ASCII
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, string value)
	{
		return Write(address, value, Encoding.ASCII);
	}

	/// <summary>
	/// 写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为ASCII
	/// Write string information of the specified length. If it exceeds the value, the string is truncated.
	/// If the length is not enough, it is filled with 0 and the encoding is ASCII.
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="length">字符串的长度</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, string value, int length)
	{
		return Write(address, value, length, Encoding.ASCII);
	}

	/// <summary>
	/// 写入字符串信息，需要指定的编码信息
	/// Write string information, need to specify the encoding information
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="encoding">指定的编码信息</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, string value, Encoding encoding)
	{
		byte[] array = ByteTransform.TransByte(value, encoding);
		if (WordLength == 1)
		{
			array = SoftBasic.ArrayExpandToLengthEven(array);
		}
		return Write(address, array);
	}

	/// <summary>
	/// 写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为指定的编码信息
	/// Write string information of the specified length. If it exceeds the value, the string is truncated. If the length is not enough,
	/// then the operation is complemented with 0 , you should specified the encoding information
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="length">字符串的长度</param>
	/// <param name="encoding">指定的编码信息</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual OperateResult Write(string address, string value, int length, Encoding encoding)
	{
		byte[] data = ByteTransform.TransByte(value, encoding);
		if (WordLength == 1)
		{
			data = SoftBasic.ArrayExpandToLengthEven(data);
		}
		data = SoftBasic.ArrayExpandToLength(data, length);
		return Write(address, data);
	}

	[HslMqttApi("WaitBool", "")]
	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public OperateResult<TimeSpan> Wait(string address, bool waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return ReadWriteNetHelper.Wait(this, address, waitValue, readInterval, waitTimeout);
	}

	[HslMqttApi("WaitInt16", "")]
	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public OperateResult<TimeSpan> Wait(string address, short waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return ReadWriteNetHelper.Wait(this, address, waitValue, readInterval, waitTimeout);
	}

	[HslMqttApi("WaitUInt16", "")]
	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public OperateResult<TimeSpan> Wait(string address, ushort waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return ReadWriteNetHelper.Wait(this, address, waitValue, readInterval, waitTimeout);
	}

	[HslMqttApi("WaitInt32", "")]
	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public OperateResult<TimeSpan> Wait(string address, int waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return ReadWriteNetHelper.Wait(this, address, waitValue, readInterval, waitTimeout);
	}

	[HslMqttApi("WaitUInt32", "")]
	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public OperateResult<TimeSpan> Wait(string address, uint waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return ReadWriteNetHelper.Wait(this, address, waitValue, readInterval, waitTimeout);
	}

	[HslMqttApi("WaitInt64", "")]
	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public OperateResult<TimeSpan> Wait(string address, long waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return ReadWriteNetHelper.Wait(this, address, waitValue, readInterval, waitTimeout);
	}

	[HslMqttApi("WaitUInt64", "")]
	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public OperateResult<TimeSpan> Wait(string address, ulong waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return ReadWriteNetHelper.Wait(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public async Task<OperateResult<TimeSpan>> WaitAsync(string address, bool waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return await ReadWriteNetHelper.WaitAsync(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public async Task<OperateResult<TimeSpan>> WaitAsync(string address, short waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return await ReadWriteNetHelper.WaitAsync(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public async Task<OperateResult<TimeSpan>> WaitAsync(string address, ushort waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return await ReadWriteNetHelper.WaitAsync(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public async Task<OperateResult<TimeSpan>> WaitAsync(string address, int waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return await ReadWriteNetHelper.WaitAsync(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public async Task<OperateResult<TimeSpan>> WaitAsync(string address, uint waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return await ReadWriteNetHelper.WaitAsync(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public async Task<OperateResult<TimeSpan>> WaitAsync(string address, long waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return await ReadWriteNetHelper.WaitAsync(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 等待指定地址的值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。
	/// Waiting for the  value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
	/// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
	/// </summary>
	/// <param name="address">其实地址</param>
	/// <param name="waitValue">等待检测是值</param>
	/// <param name="readInterval">读取的频率</param>
	/// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
	/// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
	public async Task<OperateResult<TimeSpan>> WaitAsync(string address, ulong waitValue, int readInterval = 100, int waitTimeout = -1)
	{
		return await ReadWriteNetHelper.WaitAsync(this, address, waitValue, readInterval, waitTimeout);
	}

	/// <summary>
	/// 异步批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Asynchronous batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public virtual async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<List<byte[]>> command = BuildDeviceReadCommand(address, length, isBit: false);
		if (!command.IsSuccess)
		{
			return await Task.Run(() => Read(address, length)).ConfigureAwait(continueOnCapturedContext: false);
		}
		List<byte> array = new List<byte>();
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content[i]).ConfigureAwait(continueOnCapturedContext: false);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(read);
			}
			OperateResult check = CheckDeviceResponse(address, command.Content[i], read.Content);
			if (!check.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(check);
			}
			OperateResult<byte[]> analysis = ParseDeviceReadResult(address, command.Content[i], read.Content);
			if (!analysis.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(analysis);
			}
			array.AddRange(analysis.Content);
		}
		return OperateResult.CreateSuccessResult(array.ToArray());
	}

	/// <summary>
	/// 异步写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Asynchronously writes the original byte array data to the specified address, and returns whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<byte[]> command = BuildDeviceWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return await Task.Run(() => Write(address, value)).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckDeviceResponse(address, command.Content, read.Content);
	}

	/// <summary>
	/// 异步批量读取数组信息，需要指定地址和长度，返回 数组
	/// Asynchronously batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public virtual async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		OperateResult<List<byte[]>> command = BuildDeviceReadCommand(address, length, isBit: true);
		if (!command.IsSuccess)
		{
			return await Task.Run(() => ReadBool(address, length)).ConfigureAwait(continueOnCapturedContext: false);
		}
		List<bool> array = new List<bool>();
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content[i]).ConfigureAwait(continueOnCapturedContext: false);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(read);
			}
			OperateResult check = CheckDeviceResponse(address, command.Content[i], read.Content);
			if (!check.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(check);
			}
			OperateResult<bool[]> analysis = ParseDeviceReadBoolResult(address, command.Content[i], read.Content, length);
			if (!analysis.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(analysis);
			}
			array.AddRange(analysis.Content);
		}
		bool[] value = array.ToArray();
		if (value.Length > length)
		{
			return OperateResult.CreateSuccessResult(value.SelectBegin(length));
		}
		return OperateResult.CreateSuccessResult(array.ToArray());
	}

	/// <summary>
	/// 异步读取单个的数据信息
	/// Asynchronously read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public virtual async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadBoolAsync(address, 1));
	}

	/// <summary>
	/// 异步批量写入数组数据，返回是否成功
	/// Asynchronously batch write  array data, return success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		OperateResult<byte[]> command = BuildDeviceWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return await Task.Run(() => Write(address, value)).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckDeviceResponse(address, command.Content, read.Content);
	}

	/// <summary>
	/// 异步批量写入数组数据，返回是否成功
	/// Asynchronously batch write  array data, return success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await WriteAsync(address, new bool[1] { value });
	}

	public async Task<OperateResult<T>> ReadCustomerAsync<T>(string address) where T : IDataTransfer, new()
	{
		return await ReadWriteNetHelper.ReadCustomerAsync<T>(this, address);
	}

	public async Task<OperateResult<T>> ReadCustomerAsync<T>(string address, T obj) where T : IDataTransfer, new()
	{
		return await ReadWriteNetHelper.ReadCustomerAsync(this, address, obj);
	}

	public async Task<OperateResult> WriteCustomerAsync<T>(string address, T data) where T : IDataTransfer, new()
	{
		return await ReadWriteNetHelper.WriteCustomerAsync(this, address, data);
	}

	public virtual async Task<OperateResult<T>> ReadAsync<T>() where T : class, new()
	{
		return await HslReflectionHelper.ReadAsync<T>(this);
	}

	public virtual async Task<OperateResult> WriteAsync<T>(T data) where T : class, new()
	{
		return await HslReflectionHelper.WriteAsync(data, this);
	}

	public virtual async Task<OperateResult<T>> ReadStructAsync<T>(string address, ushort length) where T : class, new()
	{
		return await ReadWriteNetHelper.ReadStructAsync<T>(this, address, length, ByteTransform);
	}

	/// <summary>
	/// 异步读取16位的有符号的整型数据
	/// Asynchronously read 16-bit signed integer data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的short数据</returns>
	public virtual async Task<OperateResult<short>> ReadInt16Async(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadInt16Async(address, 1));
	}

	/// <summary>
	/// 异步读取16位的有符号整型数组
	/// Asynchronously read 16-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的short数组</returns>
	public virtual async Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 1)), (byte[] m) => ByteTransform.TransInt16(m, 0, length));
	}

	/// <summary>
	/// 异步读取16位的无符号整型
	/// Asynchronously read 16-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ushort数据</returns>
	public virtual async Task<OperateResult<ushort>> ReadUInt16Async(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadUInt16Async(address, 1));
	}

	/// <summary>
	/// 异步读取16位的无符号整型数组
	/// Asynchronously read 16-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的ushort数组</returns>
	public virtual async Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 1)), (byte[] m) => ByteTransform.TransUInt16(m, 0, length));
	}

	/// <summary>
	/// 异步读取32位的有符号整型
	/// Asynchronously read 32-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的int数据</returns>
	public virtual async Task<OperateResult<int>> ReadInt32Async(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadInt32Async(address, 1));
	}

	/// <summary>
	/// 异步读取32位有符号整型数组
	/// Asynchronously read 32-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的int数组</returns>
	public virtual async Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 2)), (byte[] m) => ByteTransform.TransInt32(m, 0, length));
	}

	/// <summary>
	/// 异步读取32位的无符号整型
	/// Asynchronously read 32-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的uint数据</returns>
	public virtual async Task<OperateResult<uint>> ReadUInt32Async(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadUInt32Async(address, 1));
	}

	/// <summary>
	/// 异步读取32位的无符号整型数组
	/// Asynchronously read 32-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的uint数组</returns>
	public virtual async Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 2)), (byte[] m) => ByteTransform.TransUInt32(m, 0, length));
	}

	/// <summary>
	/// 异步读取单浮点数据
	/// Asynchronously read single floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的float数据</returns>
	public virtual async Task<OperateResult<float>> ReadFloatAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadFloatAsync(address, 1));
	}

	/// <summary>
	/// 异步读取单浮点精度的数组
	/// Asynchronously read single floating point array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的float数组</returns>
	public virtual async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 2)), (byte[] m) => ByteTransform.TransSingle(m, 0, length));
	}

	/// <summary>
	/// 异步读取64位的有符号整型
	/// Asynchronously read 64-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的long数据</returns>
	public virtual async Task<OperateResult<long>> ReadInt64Async(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadInt64Async(address, 1));
	}

	/// <summary>
	/// 异步读取64位的有符号整型数组
	/// Asynchronously read 64-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的long数组</returns>
	public virtual async Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 4)), (byte[] m) => ByteTransform.TransInt64(m, 0, length));
	}

	/// <summary>
	/// 异步读取64位的无符号整型
	/// Asynchronously read 64-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ulong数据</returns>
	public virtual async Task<OperateResult<ulong>> ReadUInt64Async(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadUInt64Async(address, 1));
	}

	/// <summary>
	/// 异步读取64位的无符号整型的数组
	/// Asynchronously read 64-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的ulong数组</returns>
	public virtual async Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 4)), (byte[] m) => ByteTransform.TransUInt64(m, 0, length));
	}

	/// <summary>
	/// 异步读取双浮点的数据
	/// Asynchronously read double floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的double数据</returns>
	public virtual async Task<OperateResult<double>> ReadDoubleAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadDoubleAsync(address, 1));
	}

	/// <summary>
	/// 异步读取双浮点数据的数组
	/// Asynchronously read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	public virtual async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 4)), (byte[] m) => ByteTransform.TransDouble(m, 0, length));
	}

	/// <summary>
	/// 异步读取字符串数据，默认为最常见的ASCII编码
	/// Asynchronously read string data, default is the most common ASCII encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	public virtual async Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
	{
		return await ReadStringAsync(address, length, Encoding.ASCII);
	}

	/// <summary>
	/// 异步使用指定的编码，读取字符串数据
	/// Asynchronously reads string data using the specified encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <param name="encoding">指定的自定义的编码</param>
	/// <returns>带有成功标识的string数据</returns>
	public virtual async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => ByteTransform.TransString(m, 0, m.Length, encoding));
	}

	/// <summary>
	/// 异步写入short数组，返回是否成功
	/// Asynchronously write short array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, short[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入short数据，返回是否成功
	/// Asynchronously write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, short value)
	{
		return await WriteAsync(address, new short[1] { value });
	}

	/// <summary>
	/// 异步写入ushort数组，返回是否成功
	/// Asynchronously write ushort array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, ushort[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入ushort数据，返回是否成功
	/// Asynchronously write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, ushort value)
	{
		return await WriteAsync(address, new ushort[1] { value });
	}

	/// <summary>
	/// 异步写入int[]数组，返回是否成功
	/// Asynchronously write int array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, int[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入int数据，返回是否成功
	/// Asynchronously write int data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, int value)
	{
		return await WriteAsync(address, new int[1] { value });
	}

	/// <summary>
	/// 异步写入uint[]数组，返回是否成功
	/// Asynchronously write uint array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, uint[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入uint数据，返回是否成功
	/// Asynchronously write uint data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, uint value)
	{
		return await WriteAsync(address, new uint[1] { value });
	}

	/// <summary>
	/// 异步写入float数组，返回是否成功
	/// Asynchronously write float array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, float[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入float数据，返回是否成功
	/// Asynchronously write float data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, float value)
	{
		return await WriteAsync(address, new float[1] { value });
	}

	/// <summary>
	/// 异步写入long数组，返回是否成功
	/// Asynchronously write long array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, long[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入long数据，返回是否成功
	/// Asynchronously write long data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, long value)
	{
		return await WriteAsync(address, new long[1] { value });
	}

	/// <summary>
	/// 异步写入ulong数组，返回是否成功
	/// Asynchronously write ulong array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, ulong[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入ulong数据，返回是否成功
	/// Asynchronously write ulong data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, ulong value)
	{
		return await WriteAsync(address, new ulong[1] { value });
	}

	/// <summary>
	/// 异步写入double数组，返回是否成功
	/// Asynchronously write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await WriteAsync(address, ByteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步写入double数据，返回是否成功
	/// Asynchronously write double data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, double value)
	{
		return await WriteAsync(address, new double[1] { value });
	}

	/// <summary>
	/// 异步写入字符串信息，编码为ASCII
	/// Asynchronously write string information, encoded as ASCII
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, string value)
	{
		return await WriteAsync(address, value, Encoding.ASCII);
	}

	/// <summary>
	/// 异步写入字符串信息，需要指定的编码信息
	/// Asynchronously write string information, need to specify the encoding information
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="encoding">指定的编码信息</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		byte[] temp = ByteTransform.TransByte(value, encoding);
		if (WordLength == 1)
		{
			temp = SoftBasic.ArrayExpandToLengthEven(temp);
		}
		return await WriteAsync(address, temp);
	}

	/// <summary>
	/// 异步写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为ASCII
	/// Asynchronously write string information of the specified length. If it exceeds the value, the string is truncated.
	/// If the length is not enough, it is filled with 0 and the encoding is ASCII.
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="length">字符串的长度</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, string value, int length)
	{
		return await WriteAsync(address, value, length, Encoding.ASCII);
	}

	/// <summary>
	/// 异步写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为指定的编码信息
	/// Asynchronously write string information of the specified length. If it exceeds the value, the string is truncated. If the length is not enough,
	/// then the operation is complemented with 0 , you should specified the encoding information
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="length">字符串的长度</param>
	/// <param name="encoding">指定的编码信息</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, string value, int length, Encoding encoding)
	{
		byte[] temp = ByteTransform.TransByte(value, encoding);
		if (WordLength == 1)
		{
			temp = SoftBasic.ArrayExpandToLengthEven(temp);
		}
		temp = SoftBasic.ArrayExpandToLength(temp, length);
		return await WriteAsync(address, temp);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			CommunicationPipe?.CloseCommunication();
			CommunicationPipe?.Dispose();
		}
	}

	public void Dispose()
	{
		if (!disposedValue)
		{
			Dispose(disposing: true);
			disposedValue = true;
		}
	}

	public override string ToString()
	{
		return $"DeviceCommunication<{byteTransform}>{{{CommunicationPipe}}}";
	}
}
