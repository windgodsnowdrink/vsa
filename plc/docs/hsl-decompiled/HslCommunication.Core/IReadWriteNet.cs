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
using HslCommunication.LogNet;

namespace HslCommunication.Core;

/// <summary>
/// 所有的和设备或是交互类统一读写标准，公开了如何读写对方的一些api接口，并支持基于特性的读写操作
/// All unified read and write standards for devices and interaction classes,
/// expose how to read and write some API interfaces of each other, and support feature-based read and write operations
/// </summary>
public interface IReadWriteNet
{
	/// <summary>
	/// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以等级进行输出
	/// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at
	/// </summary>
	/// <remarks>
	/// 只要实例化即可以记录日志，实例化的对象需要实现接口  ，本组件提供了三个日志记录类，你可以实现基于   的对象。
	/// </remarks>
	ILogNet LogNet { get; set; }

	/// <summary>
	/// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，方便列表管理，也可以自己指定
	/// The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.
	/// </summary>
	/// <remarks>
	/// 当前的唯一连接ID信息在DTU的模式下，将发挥关键作用，标记唯一的DTU ID
	/// </remarks>
	string ConnectionId { get; set; }

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
	IByteTransform ByteTransform { get; set; }

	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	OperateResult<byte[]> Read(string address, ushort length);

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	OperateResult Write(string address, byte[] value);

	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	OperateResult<bool[]> ReadBool(string address, ushort length);

	/// <summary>
	/// 读取单个的数据信息
	/// Read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的 bool 值</returns>
	OperateResult<bool> ReadBool(string address);

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	OperateResult Write(string address, bool[] value);

	/// <summary>
	/// 写入单个的数据，返回是否成功
	/// Write a single  data, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	OperateResult Write(string address, bool value);

	/// <summary>
	/// 读取16位的有符号的整型数据
	/// Read 16-bit signed integer data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的short数据</returns>
	OperateResult<short> ReadInt16(string address);

	/// <summary>
	/// 读取16位的有符号整型数组
	/// Read 16-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的short数组</returns>
	OperateResult<short[]> ReadInt16(string address, ushort length);

	/// <summary>
	/// 读取16位的无符号整型
	/// Read 16-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ushort数据</returns>
	OperateResult<ushort> ReadUInt16(string address);

	/// <summary>
	/// 读取16位的无符号整型数组
	/// Read 16-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的ushort数组</returns>
	OperateResult<ushort[]> ReadUInt16(string address, ushort length);

	/// <summary>
	/// 读取32位的有符号整型
	/// Read 32-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的int数据</returns>
	OperateResult<int> ReadInt32(string address);

	/// <summary>
	/// 读取32位有符号整型数组
	/// Read 32-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的int数组</returns>
	OperateResult<int[]> ReadInt32(string address, ushort length);

	/// <summary>
	/// 读取32位的无符号整型
	/// Read 32-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的uint数据</returns>
	OperateResult<uint> ReadUInt32(string address);

	/// <summary>
	/// 读取32位的无符号整型数组
	/// Read 32-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的uint数组</returns>
	OperateResult<uint[]> ReadUInt32(string address, ushort length);

	/// <summary>
	/// 读取64位的有符号整型
	/// Read 64-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的long数据</returns>
	OperateResult<long> ReadInt64(string address);

	/// <summary>
	/// 读取64位的有符号整型数组
	/// Read 64-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的long数组</returns>
	OperateResult<long[]> ReadInt64(string address, ushort length);

	/// <summary>
	/// 读取64位的无符号整型
	/// Read 64-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ulong数据</returns>
	OperateResult<ulong> ReadUInt64(string address);

	/// <summary>
	/// 读取64位的无符号整型的数组
	/// Read 64-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带成功标志的结果数据对象</returns>
	OperateResult<ulong[]> ReadUInt64(string address, ushort length);

	/// <summary>
	/// 读取单浮点数据
	/// Read single floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的float数据</returns>
	OperateResult<float> ReadFloat(string address);

	/// <summary>
	/// 读取单浮点精度的数组
	/// Read single floating point array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的float数组</returns>
	OperateResult<float[]> ReadFloat(string address, ushort length);

	/// <summary>
	/// 读取双浮点的数据
	/// Read double floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的double数据</returns>
	OperateResult<double> ReadDouble(string address);

	/// <summary>
	/// 读取双浮点数据的数组
	/// Read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	OperateResult<double[]> ReadDouble(string address, ushort length);

	/// <summary>
	/// 读取字符串数据，默认为最常见的ASCII编码
	/// Read string data, default is the most common ASCII encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	OperateResult<string> ReadString(string address, ushort length);

	/// <summary>
	/// 使用指定的编码，读取字符串数据
	/// Reads string data using the specified encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <param name="encoding">指定的自定义的编码</param>
	/// <returns>带有成功标识的string数据</returns>
	OperateResult<string> ReadString(string address, ushort length, Encoding encoding);

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
	OperateResult<TimeSpan> Wait(string address, bool waitValue, int readInterval, int waitTimeout);

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
	OperateResult<TimeSpan> Wait(string address, short waitValue, int readInterval, int waitTimeout);

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
	OperateResult<TimeSpan> Wait(string address, ushort waitValue, int readInterval, int waitTimeout);

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
	OperateResult<TimeSpan> Wait(string address, int waitValue, int readInterval, int waitTimeout);

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
	OperateResult<TimeSpan> Wait(string address, uint waitValue, int readInterval, int waitTimeout);

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
	OperateResult<TimeSpan> Wait(string address, long waitValue, int readInterval, int waitTimeout);

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
	OperateResult<TimeSpan> Wait(string address, ulong waitValue, int readInterval, int waitTimeout);

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
	Task<OperateResult<TimeSpan>> WaitAsync(string address, bool waitValue, int readInterval, int waitTimeout);

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
	Task<OperateResult<TimeSpan>> WaitAsync(string address, short waitValue, int readInterval, int waitTimeout);

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
	Task<OperateResult<TimeSpan>> WaitAsync(string address, ushort waitValue, int readInterval, int waitTimeout);

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
	Task<OperateResult<TimeSpan>> WaitAsync(string address, int waitValue, int readInterval, int waitTimeout);

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
	Task<OperateResult<TimeSpan>> WaitAsync(string address, uint waitValue, int readInterval, int waitTimeout);

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
	Task<OperateResult<TimeSpan>> WaitAsync(string address, long waitValue, int readInterval, int waitTimeout);

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
	Task<OperateResult<TimeSpan>> WaitAsync(string address, ulong waitValue, int readInterval, int waitTimeout);

	/// <summary>
	/// 写入short数据，返回是否成功
	/// Write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, short value);

	/// <summary>
	/// 写入short数组，返回是否成功
	/// Write short array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, short[] values);

	/// <summary>
	/// 写入ushort数据，返回是否成功
	/// Write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, ushort value);

	/// <summary>
	/// 写入ushort数组，返回是否成功
	/// Write ushort array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, ushort[] values);

	/// <summary>
	/// 写入int数据，返回是否成功
	/// Write int data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, int value);

	/// <summary>
	/// 写入int[]数组，返回是否成功
	/// Write int array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, int[] values);

	/// <summary>
	/// 写入uint数据，返回是否成功
	/// Write uint data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, uint value);

	/// <summary>
	/// 写入uint[]数组，返回是否成功
	/// Write uint array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, uint[] values);

	/// <summary>
	/// 写入long数据，返回是否成功
	/// Write long data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, long value);

	/// <summary>
	/// 写入long数组，返回是否成功
	/// Write long array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, long[] values);

	/// <summary>
	/// 写入ulong数据，返回是否成功
	/// Write ulong data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, ulong value);

	/// <summary>
	/// 写入ulong数组，返回是否成功
	/// Write ulong array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, ulong[] values);

	/// <summary>
	/// 写入float数据，返回是否成功
	/// Write float data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, float value);

	/// <summary>
	/// 写入float数组，返回是否成功
	/// Write float array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, float[] values);

	/// <summary>
	/// 写入double数据，返回是否成功
	/// Write double data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, double value);

	/// <summary>
	/// 写入double数组，返回是否成功
	/// Write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, double[] values);

	/// <summary>
	/// 写入字符串信息，编码为ASCII
	/// Write string information, encoded as ASCII
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, string value);

	/// <summary>
	/// 写入字符串信息，需要指定的编码信息
	/// Write string information, need to specify the encoding information
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="encoding">指定的编码信息</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, string value, Encoding encoding);

	/// <summary>
	/// 写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为ASCII
	/// Write string information of the specified length. If it exceeds the value, the string is truncated.
	/// If the length is not enough, it is filled with 0 and the encoding is ASCII.
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="length">字符串的长度</param>
	/// <returns>带有成功标识的结果类对象</returns>
	OperateResult Write(string address, string value, int length);

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
	OperateResult Write(string address, string value, int length, Encoding encoding);

	OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new();

	OperateResult<T> ReadCustomer<T>(string address, T obj) where T : IDataTransfer, new();

	OperateResult WriteCustomer<T>(string address, T value) where T : IDataTransfer, new();

	OperateResult<T> Read<T>() where T : class, new();

	OperateResult Write<T>(T data) where T : class, new();

	OperateResult<T> ReadStruct<T>(string address, ushort length) where T : class, new();

	/// <summary>
	/// 异步批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Asynchronous batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	Task<OperateResult<byte[]>> ReadAsync(string address, ushort length);

	/// <summary>
	/// 异步写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Asynchronously writes the original byte array data to the specified address, and returns whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, byte[] value);

	/// <summary>
	/// 异步批量读取数组信息，需要指定地址和长度，返回 数组
	/// Asynchronously batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length);

	/// <summary>
	/// 异步读取单个的数据信息
	/// Asynchronously read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	Task<OperateResult<bool>> ReadBoolAsync(string address);

	/// <summary>
	/// 异步批量写入数组数据，返回是否成功
	/// Asynchronously batch write  array data, return success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, bool[] value);

	/// <summary>
	/// 异步批量写入数组数据，返回是否成功
	/// Asynchronously batch write  array data, return success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, bool value);

	/// <summary>
	/// 异步读取16位的有符号的整型数据
	/// Asynchronously read 16-bit signed integer data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的short数据</returns>
	Task<OperateResult<short>> ReadInt16Async(string address);

	/// <summary>
	/// 异步读取16位的有符号整型数组
	/// Asynchronously read 16-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的short数组</returns>
	Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length);

	/// <summary>
	/// 异步读取16位的无符号整型
	/// Asynchronously read 16-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ushort数据</returns>
	Task<OperateResult<ushort>> ReadUInt16Async(string address);

	/// <summary>
	/// 异步读取16位的无符号整型数组
	/// Asynchronously read 16-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>带有成功标识的ushort数组</returns>
	Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length);

	/// <summary>
	/// 异步读取32位的有符号整型
	/// Asynchronously read 32-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的int数据</returns>
	Task<OperateResult<int>> ReadInt32Async(string address);

	/// <summary>
	/// 异步读取32位有符号整型数组
	/// Asynchronously read 32-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的int数组</returns>
	Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length);

	/// <summary>
	/// 异步读取32位的无符号整型
	/// Asynchronously read 32-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的uint数据</returns>
	Task<OperateResult<uint>> ReadUInt32Async(string address);

	/// <summary>
	/// 异步读取32位的无符号整型数组
	/// Asynchronously read 32-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的uint数组</returns>
	Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length);

	/// <summary>
	/// 异步读取64位的有符号整型
	/// Asynchronously read 64-bit signed integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的long数据</returns>
	Task<OperateResult<long>> ReadInt64Async(string address);

	/// <summary>
	/// 异步读取64位的有符号整型数组
	/// Asynchronously read 64-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的long数组</returns>
	Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length);

	/// <summary>
	/// 异步读取64位的无符号整型
	/// Asynchronously read 64-bit unsigned integer
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的ulong数据</returns>
	Task<OperateResult<ulong>> ReadUInt64Async(string address);

	/// <summary>
	/// 异步读取64位的无符号整型的数组
	/// Asynchronously read 64-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的ulong数组</returns>
	Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length);

	/// <summary>
	/// 异步读取单浮点数据
	/// Asynchronously read single floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的float数据</returns>
	Task<OperateResult<float>> ReadFloatAsync(string address);

	/// <summary>
	/// 异步读取单浮点精度的数组
	/// Asynchronously read single floating point array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的float数组</returns>
	Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length);

	/// <summary>
	/// 异步读取双浮点的数据
	/// Asynchronously read double floating point data
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <returns>带有成功标识的double数据</returns>
	Task<OperateResult<double>> ReadDoubleAsync(string address);

	/// <summary>
	/// 异步读取双浮点数据的数组
	/// Asynchronously read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length);

	/// <summary>
	/// 异步读取字符串数据，默认为最常见的ASCII编码
	/// Asynchronously read string data, default is the most common ASCII encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	Task<OperateResult<string>> ReadStringAsync(string address, ushort length);

	/// <summary>
	/// 异步使用指定的编码，读取字符串数据
	/// Asynchronously reads string data using the specified encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <param name="encoding">指定的自定义的编码</param>
	/// <returns>带有成功标识的string数据</returns>
	Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding);

	/// <summary>
	/// 异步写入short数据，返回是否成功
	/// Asynchronously write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, short value);

	/// <summary>
	/// 异步写入short数组，返回是否成功
	/// Asynchronously write short array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, short[] values);

	/// <summary>
	/// 异步写入ushort数据，返回是否成功
	/// Asynchronously write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, ushort value);

	/// <summary>
	/// 异步写入ushort数组，返回是否成功
	/// Asynchronously write ushort array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, ushort[] values);

	/// <summary>
	/// 异步写入int数据，返回是否成功
	/// Asynchronously write int data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, int value);

	/// <summary>
	/// 异步写入int[]数组，返回是否成功
	/// Asynchronously write int array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, int[] values);

	/// <summary>
	/// 异步写入uint数据，返回是否成功
	/// Asynchronously write uint data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, uint value);

	/// <summary>
	/// 异步写入uint[]数组，返回是否成功
	/// Asynchronously write uint array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, uint[] values);

	/// <summary>
	/// 异步写入long数据，返回是否成功
	/// Asynchronously write long data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, long value);

	/// <summary>
	/// 异步写入long数组，返回是否成功
	/// Asynchronously write long array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, long[] values);

	/// <summary>
	/// 异步写入ulong数据，返回是否成功
	/// Asynchronously write ulong data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, ulong value);

	/// <summary>
	/// 异步写入ulong数组，返回是否成功
	/// Asynchronously write ulong array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, ulong[] values);

	/// <summary>
	/// 异步写入float数据，返回是否成功
	/// Asynchronously write float data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, float value);

	/// <summary>
	/// 异步写入float数组，返回是否成功
	/// Asynchronously write float array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, float[] values);

	/// <summary>
	/// 异步写入double数据，返回是否成功
	/// Asynchronously write double data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, double value);

	/// <summary>
	/// 异步写入double数组，返回是否成功
	/// Asynchronously write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, double[] values);

	/// <summary>
	/// 异步写入字符串信息，编码为ASCII
	/// Asynchronously write string information, encoded as ASCII
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, string value);

	/// <summary>
	/// 异步写入字符串信息，需要指定的编码信息
	/// Asynchronously write string information, need to specify the encoding information
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="encoding">指定的编码信息</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, string value, Encoding encoding);

	/// <summary>
	/// 异步写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为ASCII
	/// Asynchronously write string information of the specified length. If it exceeds the value, the string is truncated.
	/// If the length is not enough, it is filled with 0 and the encoding is ASCII.
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="length">字符串的长度</param>
	/// <returns>带有成功标识的结果类对象</returns>
	Task<OperateResult> WriteAsync(string address, string value, int length);

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
	Task<OperateResult> WriteAsync(string address, string value, int length, Encoding encoding);

	Task<OperateResult<T>> ReadCustomerAsync<T>(string address) where T : IDataTransfer, new();

	Task<OperateResult<T>> ReadCustomerAsync<T>(string address, T obj) where T : IDataTransfer, new();

	Task<OperateResult> WriteCustomerAsync<T>(string address, T value) where T : IDataTransfer, new();

	Task<OperateResult<T>> ReadAsync<T>() where T : class, new();

	Task<OperateResult> WriteAsync<T>(T data) where T : class, new();

	Task<OperateResult<T>> ReadStructAsync<T>(string address, ushort length) where T : class, new();
}
