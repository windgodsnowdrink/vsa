// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core.IMessage;

/// <summary>
/// Modbus-Tcp协议支持的消息解析类
/// </summary>
public class ModbusTcpMessage : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => 8;

	/// <summary>
	/// 获取或设置是否进行检查返回的消息ID和发送的消息ID是否一致，默认为true，也就是检查
	/// Get or set whether to check whether the returned message ID is consistent with the sent message ID, the default is true, that is, check
	/// </summary>
	public bool IsCheckMessageId { get; set; } = true;

	/// <summary>
	/// 获取或设置当前是否启用站号检查的功能，默认启用，读写数据时将进行站号确认操作，如果需要忽略站号，则设置为 false 即可。
	/// Gets or sets whether the station ID check function is enabled. It is enabled by default. When reading or writing data, the station ID is confirmed.
	/// </summary>
	public bool StationCheckMatch { get; set; } = true;

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
		if (base.HeadBytes?.Length >= ProtocolHeadBytesLength)
		{
			int num = base.HeadBytes[4] * 256 + base.HeadBytes[5];
			int num2 = 0;
			if (num == 0)
			{
				base.HeadBytes = base.HeadBytes.RemoveBegin(1);
				num2 = base.HeadBytes[4] * 256 + base.HeadBytes[5] - 1;
			}
			else
			{
				num2 = Math.Min(num - 2, 300);
			}
			if (base.SendBytes != null && base.SendBytes.Length == 12)
			{
				if (base.SendBytes[7] == 3 || base.SendBytes[7] == 4)
				{
					int num3 = base.SendBytes[10] * 256 + base.SendBytes[11];
					if (num3 > 0 && num3 <= 127)
					{
						int num4 = num3 * 2 + 1;
						if (num4 != num2 && num == 6)
						{
							num2 = num4;
						}
					}
				}
				else if (base.SendBytes[7] == 1 || base.SendBytes[7] == 2)
				{
					int num5 = base.SendBytes[10] * 256 + base.SendBytes[11];
					if (num5 > 0 && num5 <= 2040)
					{
						int num6 = (num5 - 1) / 8 + 1 + 1;
						if (num6 != num2 && num == 6)
						{
							num2 = num6;
						}
					}
				}
			}
			return num2;
		}
		return 0;
	}

	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (send == null)
		{
			return 1;
		}
		if (receive == null)
		{
			return 1;
		}
		if (send.Length < 8 || receive.Length < 8)
		{
			return 1;
		}
		if (IsCheckMessageId && (send[0] != receive[0] || send[1] != receive[1]))
		{
			return -1;
		}
		if (StationCheckMatch && send[6] != receive[6])
		{
			return -1;
		}
		return 1;
	}

	/// <summary>
	/// 获取头子节里的消息标识
	/// Get the message ID in the header subsection
	/// </summary>
	/// <returns>消息标识</returns>
	public override int GetHeadBytesIdentity()
	{
		return base.HeadBytes[0] * 256 + base.HeadBytes[1];
	}
}
