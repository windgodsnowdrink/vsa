// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.Core;

namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// CIP协议的基础接口信息
/// </summary>
public interface IReadWriteCip : IReadWriteNet
{
	/// <summary>
	/// 使用指定的类型写入指定的节点数据，类型信息参考API文档，地址支持携带类型代号信息，可以强制指定本次写入数据的类型信息，例如 "type=0xD1;A"
	/// Use the specified type to write the specified node data. For the type information, refer to the API documentation. The address supports carrying type code information.
	/// You can force the type information of the data to be written this time. For example, "type=0xD1;A"
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
	OperateResult WriteTag(string address, ushort typeCode, byte[] value, int length = 1);

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
	OperateResult<ushort, byte[]> ReadTag(string address, ushort length = 1);

	/// <summary>
	/// 从PLC里读取当前PLC的型号信息
	/// Read the current PLC model information from the PLC
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <returns>型号数据信息</returns>
	OperateResult<string> ReadPlcType();

	/// <summary>
	/// 使用指定的类型写入指定的节点数据，类型信息参考API文档，地址支持携带类型代号信息，可以强制指定本次写入数据的类型信息，例如 "type=0xD1;A"
	/// Use the specified type to write the specified node data. For the type information, refer to the API documentation. The address supports carrying type code information.
	/// You can force the type information of the data to be written this time. For example, "type=0xD1;A"
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
	Task<OperateResult> WriteTagAsync(string address, ushort typeCode, byte[] value, int length = 1);

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	OperateResult<DateTime> ReadDate(string address);

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteDate(string address, DateTime date);

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteTimeAndDate(string address, DateTime date);

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	OperateResult<TimeSpan> ReadTime(string address);

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteTime(string address, TimeSpan time);

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteTimeOfDate(string address, TimeSpan timeOfDate);

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	Task<OperateResult<DateTime>> ReadDateAsync(string address);

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	Task<OperateResult> WriteDateAsync(string address, DateTime date);

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	Task<OperateResult> WriteTimeAndDateAsync(string address, DateTime date);

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	Task<OperateResult<TimeSpan>> ReadTimeAsync(string address);

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	Task<OperateResult> WriteTimeAsync(string address, TimeSpan time);

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	Task<OperateResult> WriteTimeOfDateAsync(string address, TimeSpan timeOfDate);
}
