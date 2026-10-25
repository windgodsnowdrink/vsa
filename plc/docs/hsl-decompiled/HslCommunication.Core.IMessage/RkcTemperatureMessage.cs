// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.IO;

namespace HslCommunication.Core.IMessage;

/// <summary>
/// RKC温度控制器的消息类
/// </summary>
public class RkcTemperatureMessage : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => -1;

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
		byte[] array = ms.ToArray();
		if (array.Length == 1)
		{
			if (array[0] == 6 || array[0] == 21)
			{
				return true;
			}
		}
		else if (array.Length == 6 && array[0] == 4 && array[^1] == 5)
		{
			return true;
		}
		if (array.Length > 3 && array[0] == 2 && array[^2] == 3)
		{
			return true;
		}
		if (array.Length > 6 && array[0] == 4 && array[3] == 2 && array[^2] == 3)
		{
			return true;
		}
		return false;
	}
}
