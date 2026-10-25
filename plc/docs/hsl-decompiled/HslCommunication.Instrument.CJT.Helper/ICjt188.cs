// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;

namespace HslCommunication.Instrument.CJT.Helper;

/// <summary>
/// CJT188设备的接口
/// </summary>
public interface ICjt188 : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 仪表的类型
	/// </summary>
	byte InstrumentType { get; set; }

	/// <summary>
	/// 获取或设置当前的地址域信息，是一个12个字符的BCD码，例如：149100007290
	/// Get or set the current address domain information, which is a 12-character BCD code, for example: 149100007290
	/// </summary>
	string Station { get; set; }

	/// <summary>
	/// 获取或设置是否在每一次的报文通信时，增加"FE FE"的命令头
	/// Get or set whether to add the command header of "FE FE" in each message communication
	/// </summary>
	bool EnableCodeFE { get; set; }

	/// <summary>
	/// 激活设备的命令，只发送数据到设备，不等待设备数据返回
	/// The command to activate the device, only send data to the device, do not wait for the device data to return
	/// </summary>
	/// <returns>是否发送成功</returns>
	OperateResult ActiveDeveice();

	/// <summary>
	/// 写入设备的地址域信息，仅支持点对点通讯的情况，需要指定地址域信息，例如：14910000729011
	/// Write the address domain information of the device, only support point-to-point communication,
	/// you need to specify the address domain information, for example: 14910000729011
	/// </summary>
	/// <param name="cjt">CJT188的通信对象</param>
	/// <param name="address">等待写入的地址域</param>
	/// <returns>是否写入成功</returns>
	OperateResult WriteAddress(string address);

	/// <summary>
	/// 读取设备的通信地址，仅支持点对点通讯的情况，返回地址域数据，例如：14910000729012
	/// Read the communication address of the device, only support point-to-point communication, and return the address field data, for example: 149100007290
	/// </summary>
	/// <param name="cjt">CJT188的通信对象</param>
	/// <returns>设备的通信地址</returns>
	OperateResult<string> ReadAddress();

	/// <summary>
	/// 读取指定地址的所有的字符串数据信息，一般来说，一个地址只有一个数据
	/// </summary>
	/// <param name="cjt">CJT通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <returns>字符串数组信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;90-1F" 或是 "s=100000;90-1F"
	/// </remarks>
	OperateResult<string[]> ReadStringArray(string address);
}
