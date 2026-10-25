// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core;

namespace HslCommunication.Instrument.DLT.Helper;

/// <summary>
/// DLT698的接口实现
/// </summary>
public interface IDlt698 : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 获取或设置当前的地址域信息，是一个12个字符的BCD码，例如：149100007290
	/// Get or set the current address domain information, which is a 12-character BCD code, for example: 149100007290
	/// </summary>
	string Station { get; set; }

	/// <summary>
	/// 获取或设置是否在每一次的报文通信时，增加"FE FE FE FE"的命令头
	/// Get or set whether to add the command header of "FE FE FE FE" in each message communication
	/// </summary>
	bool EnableCodeFE { get; set; }

	/// <summary>
	/// 获取或设置是否使用安全的请求模式，对于有些仪表来说，不支持使用安全的模式，就需要设置为False。
	/// Get or set whether to use the secure request mode, for some meters, the safe mode is not supported, so you need to set it to False.
	/// </summary>
	bool UseSecurityResquest { get; set; }

	/// <summary>
	/// 客户机地址CA用1字节表示， 0表示不关注客户机地址。
	/// Client address CA is represented by 1 byte, and 0 means that the client address is not concerned.
	/// </summary>
	byte CA { get; set; }

	/// <summary>
	/// 将数据报文发送指定的网络通道上，根据当前指定的类型，返回一条完整的数据指令
	/// Sends a data message to the specified network channel, and returns a complete data command according to the currently specified  type
	/// </summary>
	/// <param name="socket">指定的套接字</param>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 无锁的基于套接字直接进行叠加协议的操作。
	/// </remarks>
	OperateResult<byte[]> ReadFromCoreServer(byte[] send, bool hasResponseData, bool usePackAndUnpack = true);

	/// <summary>
	/// 激活设备的命令，只发送数据到设备，不等待设备数据返回
	/// The command to activate the device, only send data to the device, do not wait for the device data to return
	/// </summary>
	/// <returns>是否发送成功</returns>
	OperateResult ActiveDeveice();

	/// <summary>
	/// 写入设备的地址域信息，仅支持点对点通讯的情况，需要指定地址域信息，例如：149100007290
	/// Write the address domain information of the device, only support point-to-point communication,
	/// you need to specify the address domain information, for example: 149100007290
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">等待写入的地址域</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteAddress(string address);

	/// <summary>
	/// 读取设备的通信地址，仅支持点对点通讯的情况，返回地址域数据，例如：149100007290
	/// Read the communication address of the device, only support point-to-point communication, and return the address field data, for example: 149100007290
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <returns>设备的通信地址</returns>
	OperateResult<string> ReadAddress();

	/// <summary>
	/// 读取指定地址的所有的字符串数据信息，一般来说，一个地址只有一个数据，但是少部分的地址存在多个数据，例如 01-01-00-00 正向有功总需求及发生时间
	/// Read all the string data information of the specified address, in general, there is only one data for one address, but there are multiple data for a small number of addresses,
	/// such as 01-01-00-00 Forward active total demand and occurrence time
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <returns>字符串数组信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// 地址也可以携带是否数据翻转的标记，例如 "reverse=false;00-00-00-00" 解析数据的时候就不发生反转的操作
	/// </remarks>
	OperateResult<string[]> ReadStringArray(string address);

	/// <summary>
	/// 批量读取多个地址的数据信息，返回所有的数据对象信息
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">地址数组信息</param>
	/// <returns>字符串结果数据</returns>
	OperateResult<string[]> ReadStringArray(string[] address);

	/// <summary>
	/// 写入设备的时间信息到指定的地址，返回是否成功，使用的时间类型为 0x1C, 有效数据为 年月日时分秒。
	/// Write the time information of the device to the specified address, return whether it is successful, the time type used is 0x1C, and the valid data is year, month, day, hour, minute, and second.
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="address">写入的地址的信息</param>
	/// <param name="time">时间数据</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteDateTime(string address, DateTime time);
}
