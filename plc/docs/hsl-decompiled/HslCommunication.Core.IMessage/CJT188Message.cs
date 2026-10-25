// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.IO;
using HslCommunication.Instrument.DLT.Helper;

namespace HslCommunication.Core.IMessage;

/// <summary>
/// CJT188的协议信息
/// </summary>
public class CJT188Message : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => 11;

	/// <summary>
	/// 获取或设置是否验证匹配接收到的站号信息
	/// Gets or sets whether to verify that the received station number information is matched
	/// </summary>
	public bool StationMatch { get; set; } = false;

	/// <summary>
	/// 根据是否进行站号检查来实例化一个对象
	/// </summary>
	/// <param name="stationMatch">是否进行站号检查</param>
	public CJT188Message(bool stationMatch)
	{
		StationMatch = stationMatch;
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
		return base.HeadBytes[10] + 2;
	}

	public override int PependedUselesByteLength(byte[] headByte)
	{
		return DLT645Helper.FindHeadCode68H(headByte);
	}

	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (!StationMatch)
		{
			return 1;
		}
		if (send.Length < 9 || receive.Length < 9)
		{
			return 1;
		}
		string text = send.SelectMiddle(2, 7).ToHexString();
		string text2 = receive.SelectMiddle(2, 7).ToHexString();
		if (text == "AAAAAAAAAAAAAA" || text2 == "AAAAAAAAAAAAAA" || text == text2)
		{
			return 1;
		}
		return -1;
	}

	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		byte[] array = ms.ToArray();
		if (array.Length < 11)
		{
			return false;
		}
		int num = DLT645Helper.FindHeadCode68H(array);
		if (num < 0)
		{
			return false;
		}
		if (array[num + 10] + 13 + num == array.Length && array[^1] == 22)
		{
			return true;
		}
		return false;
	}
}
