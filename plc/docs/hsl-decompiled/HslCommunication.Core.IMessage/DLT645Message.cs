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
/// DLT 645协议的串口透传的消息类
/// </summary>
public class DLT645Message : NetMessageBase, INetMessage
{
	private bool checkDataId = true;

	public int ProtocolHeadBytesLength => 10;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public DLT645Message()
	{
	}

	/// <summary>
	/// 根据是否需要检查数据标识，实例化一个对象
	/// Instantiate an object based on whether it is necessary to check the data identifier
	/// </summary>
	public DLT645Message(bool checkDataId)
	{
		this.checkDataId = checkDataId;
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
		return base.HeadBytes[9] + 2;
	}

	public override int PependedUselesByteLength(byte[] headByte)
	{
		int num = DLT645Helper.FindHeadCode68H(headByte);
		if (num < 0)
		{
			return 10;
		}
		return num;
	}

	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (!checkDataId)
		{
			return base.CheckMessageMatch(send, receive);
		}
		if (send == null || receive == null || send.Length < 16 || receive.Length < 16)
		{
			return base.CheckMessageMatch(send, receive);
		}
		int num = DLT645Helper.FindHeadCode68H(send);
		int num2 = DLT645Helper.FindHeadCode68H(receive);
		if (num < 0 || num2 < 0 || num > 4 || num2 > 4)
		{
			return base.CheckMessageMatch(send, receive);
		}
		if (send[num + 8] == 17 && receive[num2 + 8] == 145)
		{
			for (int i = 0; i < 4; i++)
			{
				if (num + 10 + i >= send.Length)
				{
					return 1;
				}
				if (num2 + 10 + i >= receive.Length)
				{
					return 1;
				}
				if (send[num + 10 + i] != receive[num2 + 10 + i])
				{
					return -1;
				}
			}
			return 1;
		}
		if (send[num + 8] == 1 && receive[num2 + 8] == 129)
		{
			for (int j = 0; j < 2; j++)
			{
				if (num + 10 + j >= send.Length)
				{
					return 1;
				}
				if (num2 + 10 + j >= receive.Length)
				{
					return 1;
				}
				if (send[num + 10 + j] != receive[num2 + 10 + j])
				{
					return -1;
				}
			}
			return 1;
		}
		return base.CheckMessageMatch(send, receive);
	}

	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		return DLT645Helper.CheckReceiveDataComplete(ms);
	}
}
