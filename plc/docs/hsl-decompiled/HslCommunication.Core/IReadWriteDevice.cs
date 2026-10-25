// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HslCommunication.Core;

/// <summary>
/// 用于读写的设备接口，相较于，增加了相关的方法，可以用来和设备进行额外的交互。
/// The device interface used for reading and writing. Compared with ,
/// a method related to  is added, which can be used for additional interaction with the device .
/// </summary>
public interface IReadWriteDevice : IReadWriteNet
{
	/// <summary>
	/// 将当前的数据报文发送到设备去，具体使用什么通信方式取决于设备信息，然后从设备接收数据回来，并返回给调用者。
	/// Send the current data message to the device, the specific communication method used depends on the device information, and then receive the data back from the device and return it to the caller.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	OperateResult<byte[]> ReadFromCoreServer(byte[] send);

	/// <summary>
	/// 将多个数据报文按顺序发到设备，并从设备接收返回的数据内容，然后拼接成一个Byte[]信息，需要重写方法才能返回正确的结果。
	/// Send multiple data packets to the device in sequence, and receive the returned data content from the device, and then splicing them into a Byte[] message,
	/// you need to rewrite  method to return the correct result.
	/// </summary>
	/// <param name="send">发送的报文列表信息</param>
	/// <returns>是否接收成功</returns>
	OperateResult<byte[]> ReadFromCoreServer(IEnumerable<byte[]> send);

	/// <summary>
	/// 将当前的数据报文发送到设备去，具体使用什么通信方式取决于设备信息，然后从设备接收数据回来，并返回给调用者。
	/// Send the current data message to the device, the specific communication method used depends on the device information, and then receive the data back from the device and return it to the caller.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] send);

	/// <summary>
	/// 将多个数据报文按顺序发到设备，并从设备接收返回的数据内容，然后拼接成一个Byte[]信息，需要重写方法才能返回正确的结果。
	/// Send multiple data packets to the device in sequence, and receive the returned data content from the device, and then splicing them into a Byte[] message,
	/// you need to rewrite  method to return the correct result.
	/// </summary>
	/// <param name="send">发送的报文列表信息</param>
	/// <returns>是否接收成功</returns>
	Task<OperateResult<byte[]>> ReadFromCoreServerAsync(IEnumerable<byte[]> send);
}
