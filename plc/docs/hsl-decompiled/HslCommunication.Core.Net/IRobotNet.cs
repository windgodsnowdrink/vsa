// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.LogNet;

namespace HslCommunication.Core.Net;

/// <summary>
/// 所有的机器人的统一读写标准，统一的基本的读写操作
/// Unified reading and writing standards for all robots, unified basic reading and writing operations
/// </summary>
public interface IRobotNet
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
	/// 根据地址读取机器人的原始的字节数据信息
	/// Read the robot's original byte data information according to the address
	/// </summary>
	/// <param name="address">指定的地址信息，对于某些机器人无效</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	OperateResult<byte[]> Read(string address);

	/// <summary>
	/// 根据地址读取机器人的字符串的数据信息
	/// Read the string data information of the robot based on the address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	OperateResult<string> ReadString(string address);

	/// <summary>
	/// 根据地址，来写入设备的相关的字节数据
	/// According to the address, to write the device related bytes data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	OperateResult Write(string address, byte[] value);

	/// <summary>
	/// 根据地址，来写入设备相关的字符串数据
	/// According to the address, to write the device related string data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	OperateResult Write(string address, string value);

	/// <summary>
	/// 根据地址读取机器人的原始的字节数据信息
	/// Read the robot's original byte data information according to the address
	/// </summary>
	/// <param name="address">指定的地址信息，对于某些机器人无效</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	Task<OperateResult<byte[]>> ReadAsync(string address);

	/// <summary>
	/// 根据地址读取机器人的字符串的数据信息
	/// Read the string data information of the robot based on the address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	Task<OperateResult<string>> ReadStringAsync(string address);

	/// <summary>
	/// 根据地址，来写入设备的相关的字节数据
	/// According to the address, to write the device related bytes data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	Task<OperateResult> WriteAsync(string address, byte[] value);

	/// <summary>
	/// 根据地址，来写入设备相关的字符串数据
	/// According to the address, to write the device related string data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	Task<OperateResult> WriteAsync(string address, string value);
}
