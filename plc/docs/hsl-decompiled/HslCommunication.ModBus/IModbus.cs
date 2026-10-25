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

namespace HslCommunication.ModBus;

/// <summary>
/// Modbus设备的接口，用来表示Modbus相关的设备对象，, ,
/// ,,均实现了该接口信息
/// Modbus device interface, used to represent Modbus-related device objects, ,
/// ,,, all implement the interface information
/// </summary>
public interface IModbus : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 获取或设置起始的地址是否从0开始，默认为True
	/// Gets or sets whether the starting address starts from 0. The default is True
	/// </summary>
	/// <remarks>
	/// 因为有些设备的起始地址是从1开始的，就要设置本属性为False
	/// </remarks>
	bool AddressStartWithZero { get; set; }

	/// <summary>
	/// 获取或者重新修改服务器的默认站号信息，当然，你可以再读写的时候动态指定，例如地址 "s=2;100", 更详细的例子可以参考DEMO界面上的地址示例。
	/// Get or re-modify the server's default station number information, of course, you can dynamically specify when reading and writing,
	/// such as the address "s=2; 100", a more detailed example can refer to the DEMO interface address example.
	/// </summary>
	/// <remarks>
	/// 当你调用 ReadCoil("100") 时，对应的站号就是本属性的值，当你调用 ReadCoil("s=2;100") 时，就忽略本属性的值，读写寄存器的时候同理
	/// </remarks>
	byte Station { get; set; }

	/// <summary>
	/// 获取或设置数据解析的格式，可选ABCD, BADC，CDAB，DCBA格式，对int,uint,float,double,long,ulong类型有作用
	/// Get or set the format of the data analysis, optional ABCD, BADC, CDAB, DCBA format, effective for int, uint, float, double, long, ulong type
	/// </summary>
	DataFormat DataFormat { get; set; }

	/// <summary>
	/// 字符串数据是否按照字来反转，默认为False
	/// Whether the string data is reversed according to words. The default is False.
	/// </summary>
	/// <remarks>
	/// 字符串按照2个字节的排列进行颠倒，根据实际情况进行设置
	/// </remarks>
	bool IsStringReverse { get; set; }

	/// <summary>
	/// 获取或是设置当前广播模式对应的站号，广播模式意味着不接收设备方的数据返回操作，默认为 -1，表示不使用广播模式。
	/// Gets or sets the station number corresponding to the current broadcast mode. Broadcast mode means that the data return operation of the device is not received. The default value is -1, indicating that broadcast mode is not used.
	/// </summary>
	int BroadcastStation { get; set; }

	/// <summary>
	/// 获取或设置当前掩码写入的功能码是否激活状态，设置为 false 时，再执行写入位时，会通过读字，修改位，写字的方式来间接实现。
	/// When the function code is set to false, and then the write bit is executed, it will be indirectly implemented by reading, modifying the bit, and writing the word.
	/// </summary>
	bool EnableWriteMaskCode { get; set; }

	/// <summary>
	/// 当进行字读取的时候，商业授权用户支持分批读取，本属性表示分批的长度信息，默认为 120，可以根据实际情况来设置其他值。
	/// When reading of word type, commercial authorization users support batch reading. This attribute represents the length information of the batch, with a default of 120. Other values can be set according to the actual situation.
	/// </summary>
	int WordReadBatchLength { get; set; }

	/// <summary>
	/// 获取或设置是否禁用功能码05的写入操作，设置为true时，表示不允许使用功能码 06 进行写入操作。写入单个的字数据也使用 0x10 功能码
	/// Get or set whether to disable the write operation of function code 06. When set to true, it indicates that the write operation of function code 05 is not allowed. Writing individual character data also uses the 0x10 function code
	/// </summary>
	bool DisableFunctionCode06 { get; set; }

	/// <summary>
	/// 将当前的地址信息转换成Modbus格式的地址，如果转换失败，返回失败的消息。默认不进行任何的转换。
	/// Convert the current address information into a Modbus format address. If the conversion fails, a failure message will be returned. No conversion is performed by default.
	/// </summary>
	/// <param name="address">传入的地址</param>
	/// <param name="modbusCode">Modbus的功能码</param>
	/// <returns>转换之后Modbus的地址</returns>
	OperateResult<string> TranslateToModbusAddress(string address, byte modbusCode);

	/// <summary>
	/// 注册一个新的地址映射关系，注册地址映射关系后，就可以使用新的地址来读写Modbus数据了，通常用于其他的支持Modbus协议的PLC。
	/// After registering a new address mapping, you can use the new address to read and write Modbus data, which is usually used for other PLCs that support the Modbus protocol.
	/// </summary>
	/// <param name="mapping">地址映射关系信息</param>
	void RegisteredAddressMapping(Func<string, byte, OperateResult<string>> mapping);

	/// <summary>
	/// 使用0x17功能码来实现同时写入并读取数据的操作，使用一条报文来实现，需要指定读取的地址，长度，写入的地址，写入的数据信息，返回读取的结果数据。
	/// Use 0x17 function code to write and read data at the same time, and use a message to implement it,
	/// you need to specify the read address, length, written address, written data information, and return the read result data.
	/// </summary>
	/// <param name="modbus">Modbus通信对象</param>
	/// <param name="readAddress">读取的地址信息</param>
	/// <param name="length">读取的长度信息</param>
	/// <param name="writeAddress">写入的地址信息</param>
	/// <param name="value">写入的字节数据信息</param>
	/// <returns>读取的结果对象</returns>
	OperateResult<byte[]> ReadWrite(string readAddress, ushort length, string writeAddress, byte[] value);

	/// <summary>
	/// 使用0x14功能码读取文件的数据内容，需要指定文件编号，地址，长度，返回读取的结果数据，读取的长度为任意长度，内部自动根据实际情况来切割操作。
	/// </summary>
	/// <param name="modbus">Modbus设备通信对象</param>
	/// <param name="station">设备站号信息</param>
	/// <param name="fileNumber">文件号</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>返回结果</returns>
	OperateResult<byte[]> ReadFile(ushort fileNumber, ushort address, ushort length);

	/// <summary>
	/// 使用0x15功能码写入文件的数据内容，需要指定文件编号，地址，数据，返回写入的结果数据。写入的数据字节长度不能大于248个字节，如果想写入更多的字节，就需要切片写入操作
	/// </summary>
	/// <param name="modbus">Modbus设备通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="fileNumber">文件号</param>
	/// <param name="address">起始地址</param>
	/// <param name="data">写入的数据信息</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteFile(ushort fileNumber, ushort address, byte[] data);
}
