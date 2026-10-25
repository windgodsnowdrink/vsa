// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.CNC.Fanuc;

/// <summary>
/// Fanuc机床的操作信息
/// </summary>
public class FanucOperatorMessage
{
	/// <summary>
	/// Number of operator's message
	/// </summary>
	public short Number { get; set; }

	/// <summary>
	/// Kind of operator's message
	/// </summary>
	public short Type { get; set; }

	/// <summary>
	/// Operator's message strings
	/// </summary>
	public string Data { get; set; }

	/// <summary>
	/// 创建一个fanuc的操作消息对象
	/// </summary>
	/// <param name="byteTransform">数据变换对象</param>
	/// <param name="buffer">读取的数据缓存信息</param>
	/// <param name="encoding">解析的编码信息</param>
	/// <returns>fanuc设备的操作信息</returns>
	public static FanucOperatorMessage CreateMessage(IByteTransform byteTransform, byte[] buffer, Encoding encoding)
	{
		FanucOperatorMessage fanucOperatorMessage = new FanucOperatorMessage();
		fanucOperatorMessage.Number = byteTransform.TransInt16(buffer, 2);
		fanucOperatorMessage.Type = byteTransform.TransInt16(buffer, 6);
		short num = byteTransform.TransInt16(buffer, 10);
		if (num + 12 <= buffer.Length)
		{
			fanucOperatorMessage.Data = encoding.GetString(buffer, 12, num);
		}
		else
		{
			fanucOperatorMessage.Data = encoding.GetString(buffer, 12, buffer.Length - 12).TrimEnd('\0');
		}
		return fanucOperatorMessage;
	}

	public override string ToString()
	{
		return $"Number: {Number}, Type: {Type}, Data: {Data}";
	}
}
