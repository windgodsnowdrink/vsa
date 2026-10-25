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
using HslCommunication.Core.Pipe;

namespace HslCommunication.Instrument.DLT.Helper;

/// <summary>
/// DLT645的接口实现
/// </summary>
public interface IDlt645 : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 获取或设置当前的管道信息，管道类型为的继承类，内置了管道，管道，管道等
	/// Get or set the current pipeline information, the pipeline type is  inheritance class,  pipeline,  pipeline,  pipeline, etc
	/// </summary>
	CommunicationPipe CommunicationPipe { get; }

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
	/// 获取当前的DLT645的类型信息
	/// Gets the type information of the current DLT645
	/// </summary>
	DLT645Type DLTType { get; }

	/// <summary>
	/// 获取或设置当前DLT645的密码，当进行写入数据操作的时候，需要正确的密码才能写入
	/// Obtain or set the password of the current DLT645, and the correct password is required to write data operations
	/// </summary>
	/// <remarks>
	/// 对于 DLT645/1997 协议来说无效
	/// </remarks>
	string Password { get; set; }

	/// <summary>
	/// 获取或设置当前DLT645的操作者代码，当进行写入数据操作的时候，需要指定正确的值
	/// Obtain or set the operator code of the current DLT645, and specify the correct value when writing data
	/// </summary>
	/// <remarks>
	/// 对于 DLT645/1997 协议来说无效
	/// </remarks>
	string OpCode { get; set; }

	/// <summary>
	/// 获取或设置当前的DLT645是否检查数据标识，默认为true
	/// Get or set whether the current DLT645 checks data identifiers, with the default being true
	/// </summary>
	bool CheckDataId { get; set; }

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
	/// 广播指定的时间，强制从站与主站时间同步，传入时间对象，没有数据返回。
	/// Broadcast the specified time, force the slave station to synchronize with the master station time,
	/// pass in the  time object, and no data will be returned.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="dateTime">时间对象</param>
	/// <returns>是否成功</returns>
	OperateResult BroadcastTime(DateTime dateTime);

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
	/// 跳闸功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	OperateResult Trip(DateTime validTime);

	/// <summary>
	/// 跳闸功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	OperateResult Trip(string station, DateTime validTime);

	/// <summary>
	/// 合闸允许功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	OperateResult SwitchingOn(DateTime validTime);

	/// <summary>
	/// 合闸允许功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	OperateResult SwitchingOn(string station, DateTime validTime);

	/// <summary>
	/// 更改通信速率，波特率可选 600,1200,2400,4800,9600,19200，其他值无效，可以携带地址域信息，s=1;9600
	/// Change the communication rate, the baud rate can be 600, 1200, 2400, 4800, 9600, 19200,
	/// other values are invalid, you can carry address domain information, s=1;9600
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="baudRate">波特率的信息</param>
	/// <returns>是否更改成功</returns>
	/// <remarks>
	/// 对于DLT1997来说，只支持 300, 600, 2400, 4800, 9600
	/// </remarks>
	OperateResult ChangeBaudRate(string baudRate);
}
