// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.IO;
using HslCommunication.ModBus;

namespace HslCommunication.Core.IMessage;

/// <summary>
/// ModbusRtu的协议类信息
/// </summary>
public class ModbusRtuMessage : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => -1;

	/// <summary>
	/// 获取或设置当前是否启用站号检查的功能，默认启用，读写数据时将进行站号确认操作，如果需要忽略站号，则设置为 false 即可。
	/// Gets or sets whether the station ID check function is enabled. It is enabled by default. When reading or writing data, the station ID is confirmed.
	/// </summary>
	public bool StationCheckMatch { get; set; } = true;

	/// <summary>
	/// 指定是否检查站号来实例化一个对象
	/// </summary>
	/// <param name="stationCheck">是否检查站号</param>
	public ModbusRtuMessage(bool stationCheck)
	{
		StationCheckMatch = stationCheck;
	}

	/// <summary>
	/// 从当前的头子节文件中提取出接下来需要接收的数据长度
	/// Extract the length of the data to be received from the current header file
	/// </summary>
	/// <returns>返回接下来的数据内容长度</returns>
	/// <remarks>
	/// 如果剩余字节的长度小于0，则表示消息头数据还没有接收完整，还需要接收一定的长度(返回值的绝对值)，然后再判断剩余字节长度是否小于0，直到结果大于等于0为止，最多判断的次数为16次，超过16次将返回失败
	/// If the length of the remaining bytes is less than 0, it means that the message header data has not been received completely, and a certain length (absolute value of the return value) needs to be received,
	/// and then it is judged whether the length of the remaining bytes is less than 0 until the result is greater than or equal to 0,
	/// the maximum number of judgments is 16, more than 16 times will return failure
	/// </remarks>
	public int GetContentLengthByHeadBytes()
	{
		return 0;
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		return ModbusInfo.CheckRtuReceiveDataComplete(send, ms.ToArray());
	}

	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (!StationCheckMatch)
		{
			return 1;
		}
		return ModbusInfo.CheckRtuMessageMatch(send, receive);
	}
}
