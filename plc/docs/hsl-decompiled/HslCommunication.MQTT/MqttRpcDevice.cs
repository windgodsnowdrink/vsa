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
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.MQTT;

/// <summary>
/// 基于MRPC实现的远程设备访问的接口，实现了和基础PLC一样的访问功能，适用的设备为  将PLC实际的通信对象注册为 RPC 接口服务。
/// The interface for remote device access based on MRPC implements the same access function as the basic PLC. The applicable device is  to register the actual communication object of the PLC as an RPC interface service.
/// </summary>
/// <remarks>
/// 什么时候会用到本设备对象呢？如果你得PLC端口只有一个，只能支持一个连接，但是实际通信的客户端不止一个时，就可以使用本类实现一对多通信。或是你希望在最终通信的客户端和PLC之间做个隔离，并增加安全校验。详细的例子参考API文档。
/// When will this device object be used? If you have only one PLC port and can only support one connection, but there are more than one client for actual communication, you can use this class to implement one-to-many communication.
/// Or you want to isolate the final communication client from the PLC and add security checks. For a detailed example, refer to the API documentation.
/// </remarks>
public class MqttRpcDevice : MqttSyncClient, IReadWriteDevice, IReadWriteNet
{
	private IByteTransform byteTransform = new RegularByteTransform();

	private string deviceTopic;

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
			byteTransform = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的设备主题信息
	/// Get or set the current device theme information.
	/// </summary>
	public string DeviceTopic
	{
		get
		{
			return deviceTopic;
		}
		set
		{
			deviceTopic = value;
		}
	}

	/// <summary>
	/// 实例化一个MQTT的同步客户端
	/// Instantiate an MQTT synchronization client
	/// </summary>
	/// <param name="options">连接的参数信息，可以指定IP地址，端口，账户名，密码，客户端ID信息</param>
	/// <param name="topic">设备关联的主题信息</param>
	public MqttRpcDevice(MqttConnectionOptions options, string topic = null)
		: base(options)
	{
		deviceTopic = topic;
	}

	/// <summary>
	/// 通过指定的ip地址及端口来实例化一个同步的MQTT客户端
	/// Instantiate a synchronized MQTT client with the specified IP address and port
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号信息</param>
	/// <param name="topic">设备关联的主题信息</param>
	public MqttRpcDevice(string ipAddress, int port, string topic = null)
		: base(ipAddress, port)
	{
		deviceTopic = topic;
	}

	private string GetTopic(string topic)
	{
		if (string.IsNullOrEmpty(deviceTopic))
		{
			return topic;
		}
		if (deviceTopic.EndsWith("/"))
		{
			return deviceTopic + topic;
		}
		return deviceTopic + "/" + topic;
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
		return ReadRpc<byte[]>(GetTopic("ReadByteArray"), new { address, length });
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
		return ReadRpc<string>(GetTopic("WriteByteArray"), new
		{
			address = address,
			value = value.ToHexString()
		});
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
		return ReadRpc<bool[]>(GetTopic("ReadBoolArray"), new { address, length });
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
		return ReadRpc<bool>(GetTopic("ReadBool"), new { address });
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
		return ReadRpc<string>(GetTopic("WriteBoolArray"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteBool"), new { address, value });
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
	public OperateResult<short> ReadInt16(string address)
	{
		return ReadRpc<short>(GetTopic("ReadInt16"), new { address });
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
		return ReadRpc<short[]>(GetTopic("ReadInt16Array"), new { address, length });
	}

	[HslMqttApi("ReadUInt16", "")]
	/// <summary>
	/// 读取16位的无符号整型
	/// Read 16-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ushort数据</returns>
	public OperateResult<ushort> ReadUInt16(string address)
	{
		return ReadRpc<ushort>(GetTopic("ReadUInt16"), new { address });
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
		return ReadRpc<ushort[]>(GetTopic("ReadUInt16Array"), new { address, length });
	}

	[HslMqttApi("ReadInt32", "")]
	/// <summary>
	/// 读取32位的有符号整型
	/// Read 32-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的int数据</returns>
	public OperateResult<int> ReadInt32(string address)
	{
		return ReadRpc<int>(GetTopic("ReadInt32"), new { address });
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
		return ReadRpc<int[]>(GetTopic("ReadInt32Array"), new { address, length });
	}

	[HslMqttApi("ReadUInt32", "")]
	/// <summary>
	/// 读取32位的无符号整型
	/// Read 32-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的uint数据</returns>
	public OperateResult<uint> ReadUInt32(string address)
	{
		return ReadRpc<uint>(GetTopic("ReadUInt32"), new { address });
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
		return ReadRpc<uint[]>(GetTopic("ReadUInt32Array"), new { address, length });
	}

	[HslMqttApi("ReadFloat", "")]
	/// <summary>
	/// 读取单浮点数据
	/// Read single floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的float数据</returns>
	public OperateResult<float> ReadFloat(string address)
	{
		return ReadRpc<float>(GetTopic("ReadFloat"), new { address });
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
		return ReadRpc<float[]>(GetTopic("ReadFloatArray"), new { address, length });
	}

	[HslMqttApi("ReadInt64", "")]
	/// <summary>
	/// 读取64位的有符号整型
	/// Read 64-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的long数据</returns>
	public OperateResult<long> ReadInt64(string address)
	{
		return ReadRpc<long>(GetTopic("ReadInt64"), new { address });
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
		return ReadRpc<long[]>(GetTopic("ReadInt64Array"), new { address, length });
	}

	[HslMqttApi("ReadUInt64", "")]
	/// <summary>
	/// 读取64位的无符号整型
	/// Read 64-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ulong数据</returns>
	public OperateResult<ulong> ReadUInt64(string address)
	{
		return ReadRpc<ulong>(GetTopic("ReadUInt64"), new { address });
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
		return ReadRpc<ulong[]>(GetTopic("ReadUInt64Array"), new { address, length });
	}

	[HslMqttApi("ReadDouble", "")]
	/// <summary>
	/// 读取双浮点的数据
	/// Read double floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的double数据</returns>
	public OperateResult<double> ReadDouble(string address)
	{
		return ReadRpc<double>(GetTopic("ReadDouble"), new { address });
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
		return ReadRpc<double[]>(GetTopic("ReadDoubleArray"), new { address, length });
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
		return ReadRpc<string>(GetTopic("ReadString"), new { address, length });
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
		return ReadRpc<string>(GetTopic("WriteInt16Array"), new { address, values });
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
		return ReadRpc<string>(GetTopic("WriteInt16"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteUInt16Array"), new { address, values });
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
		return ReadRpc<string>(GetTopic("WriteUInt16"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteInt32Array"), new { address, values });
	}

	[HslMqttApi("WriteInt32", "")]
	/// <summary>
	/// 写入int数据，返回是否成功
	/// Write int data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public OperateResult Write(string address, int value)
	{
		return ReadRpc<string>(GetTopic("WriteInt32"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteUInt32Array"), new { address, values });
	}

	[HslMqttApi("WriteUInt32", "")]
	/// <summary>
	/// 写入uint数据，返回是否成功
	/// Write uint data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public OperateResult Write(string address, uint value)
	{
		return ReadRpc<string>(GetTopic("WriteUInt32"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteFloatArray"), new { address, values });
	}

	[HslMqttApi("WriteFloat", "")]
	/// <summary>
	/// 写入float数据，返回是否成功
	/// Write float data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public OperateResult Write(string address, float value)
	{
		return ReadRpc<string>(GetTopic("WriteFloat"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteInt64Array"), new { address, values });
	}

	[HslMqttApi("WriteInt64", "")]
	/// <summary>
	/// 写入long数据，返回是否成功
	/// Write long data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public OperateResult Write(string address, long value)
	{
		return ReadRpc<string>(GetTopic("WriteInt64"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteUInt64Array"), new { address, values });
	}

	[HslMqttApi("WriteUInt64", "")]
	/// <summary>
	/// 写入ulong数据，返回是否成功
	/// Write ulong data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public OperateResult Write(string address, ulong value)
	{
		return ReadRpc<string>(GetTopic("WriteUInt64"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteDoubleArray"), new { address, values });
	}

	[HslMqttApi("WriteDouble", "")]
	/// <summary>
	/// 写入double数据，返回是否成功
	/// Write double data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public OperateResult Write(string address, double value)
	{
		return ReadRpc<string>(GetTopic("WriteDouble"), new { address, value });
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
		return ReadRpc<string>(GetTopic("WriteString"), new { address, value });
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
		byte[] value2 = ByteTransform.TransByte(value, encoding);
		return Write(address, value2);
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
		return await ReadRpcAsync<byte[]>(GetTopic("ReadByteArray"), new { address, length });
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
		return await ReadRpcAsync<string>(GetTopic("WriteByteArray"), new
		{
			address = address,
			value = value.ToHexString()
		});
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
		return await ReadRpcAsync<bool[]>(GetTopic("ReadBoolArray"), new { address, length });
	}

	/// <summary>
	/// 异步读取单个的数据信息
	/// Asynchronously read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public virtual async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		return await ReadRpcAsync<bool>(GetTopic("ReadBool"), new { address });
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
		return await ReadRpcAsync<string>(GetTopic("WriteBoolArray"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteBool"), new { address, value });
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
	public async Task<OperateResult<short>> ReadInt16Async(string address)
	{
		return await ReadRpcAsync<short>(GetTopic("ReadInt16"), new { address });
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
		return await ReadRpcAsync<short[]>(GetTopic("ReadInt16Array"), new { address, length });
	}

	/// <summary>
	/// 异步读取16位的无符号整型
	/// Asynchronously read 16-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ushort数据</returns>
	public async Task<OperateResult<ushort>> ReadUInt16Async(string address)
	{
		return await ReadRpcAsync<ushort>(GetTopic("ReadUInt16"), new { address });
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
		return await ReadRpcAsync<ushort[]>(GetTopic("ReadUInt16Array"), new { address, length });
	}

	/// <summary>
	/// 异步读取32位的有符号整型
	/// Asynchronously read 32-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的int数据</returns>
	public async Task<OperateResult<int>> ReadInt32Async(string address)
	{
		return await ReadRpcAsync<int>(GetTopic("ReadInt32"), new { address });
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
		return await ReadRpcAsync<int[]>(GetTopic("ReadInt32Array"), new { address, length });
	}

	/// <summary>
	/// 异步读取32位的无符号整型
	/// Asynchronously read 32-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的uint数据</returns>
	public async Task<OperateResult<uint>> ReadUInt32Async(string address)
	{
		return await ReadRpcAsync<uint>(GetTopic("ReadUInt32"), new { address });
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
		return await ReadRpcAsync<uint[]>(GetTopic("ReadUInt32Array"), new { address, length });
	}

	/// <summary>
	/// 异步读取单浮点数据
	/// Asynchronously read single floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的float数据</returns>
	public async Task<OperateResult<float>> ReadFloatAsync(string address)
	{
		return await ReadRpcAsync<float>(GetTopic("ReadFloat"), new { address });
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
		return await ReadRpcAsync<float[]>(GetTopic("ReadFloatArray"), new { address, length });
	}

	/// <summary>
	/// 异步读取64位的有符号整型
	/// Asynchronously read 64-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的long数据</returns>
	public async Task<OperateResult<long>> ReadInt64Async(string address)
	{
		return await ReadRpcAsync<long>(GetTopic("ReadInt64"), new { address });
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
		return await ReadRpcAsync<long[]>(GetTopic("ReadInt64Array"), new { address, length });
	}

	/// <summary>
	/// 异步读取64位的无符号整型
	/// Asynchronously read 64-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ulong数据</returns>
	public async Task<OperateResult<ulong>> ReadUInt64Async(string address)
	{
		return await ReadRpcAsync<ulong>(GetTopic("ReadUInt64"), new { address });
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
		return await ReadRpcAsync<ulong[]>(GetTopic("ReadUInt64Array"), new { address, length });
	}

	/// <summary>
	/// 异步读取双浮点的数据
	/// Asynchronously read double floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的double数据</returns>
	public async Task<OperateResult<double>> ReadDoubleAsync(string address)
	{
		return await ReadRpcAsync<double>(GetTopic("ReadDouble"), new { address });
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
		return await ReadRpcAsync<double[]>(GetTopic("ReadDoubleArray"), new { address, length });
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
		return await ReadRpcAsync<string>(GetTopic("ReadString"), new { address, length });
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
		return await ReadRpcAsync<string>(GetTopic("WriteInt16Array"), new { address, values });
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
		return await ReadRpcAsync<string>(GetTopic("WriteInt16"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteUInt16Array"), new { address, values });
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
		return await ReadRpcAsync<string>(GetTopic("WriteUInt16"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteInt32Array"), new { address, values });
	}

	/// <summary>
	/// 异步写入int数据，返回是否成功
	/// Asynchronously write int data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public async Task<OperateResult> WriteAsync(string address, int value)
	{
		return await ReadRpcAsync<string>(GetTopic("WriteInt32"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteUInt32Array"), new { address, values });
	}

	/// <summary>
	/// 异步写入uint数据，返回是否成功
	/// Asynchronously write uint data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public async Task<OperateResult> WriteAsync(string address, uint value)
	{
		return await ReadRpcAsync<string>(GetTopic("WriteUInt32"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteFloatArray"), new { address, values });
	}

	/// <summary>
	/// 异步写入float数据，返回是否成功
	/// Asynchronously write float data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public async Task<OperateResult> WriteAsync(string address, float value)
	{
		return await ReadRpcAsync<string>(GetTopic("WriteFloat"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteInt64Array"), new { address, values });
	}

	/// <summary>
	/// 异步写入long数据，返回是否成功
	/// Asynchronously write long data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public async Task<OperateResult> WriteAsync(string address, long value)
	{
		return await ReadRpcAsync<string>(GetTopic("WriteInt64"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteUInt64Array"), new { address, values });
	}

	/// <summary>
	/// 异步写入ulong数据，返回是否成功
	/// Asynchronously write ulong data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public async Task<OperateResult> WriteAsync(string address, ulong value)
	{
		return await ReadRpcAsync<string>(GetTopic("WriteUInt64"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteDoubleArray"), new { address, values });
	}

	/// <summary>
	/// 异步写入double数据，返回是否成功
	/// Asynchronously write double data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public async Task<OperateResult> WriteAsync(string address, double value)
	{
		return await ReadRpcAsync<string>(GetTopic("WriteDouble"), new { address, value });
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
		return await ReadRpcAsync<string>(GetTopic("WriteString"), new { address, value });
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
		temp = SoftBasic.ArrayExpandToLength(temp, length);
		return await WriteAsync(address, temp);
	}
}
