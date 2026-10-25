// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.Robot.YASKAWA;

/// <summary>
/// 安川的报警信息
/// </summary>
public class YRCAlarmItem
{
	/// <summary>
	/// 报警代码
	/// </summary>
	public int AlarmCode { get; set; }

	/// <summary>
	/// 报警发生的时间
	/// </summary>
	public DateTime Time { get; set; }

	/// <summary>
	/// 报警文字列名称
	/// </summary>
	public string Message { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public YRCAlarmItem()
	{
	}

	/// <summary>
	/// 使用原始数据来实例化一个报警的对象
	/// </summary>
	/// <param name="byteTransform">字节的变换顺序</param>
	/// <param name="content">原始字节数据</param>
	/// <param name="encoding">字符串的编码信息</param>
	public YRCAlarmItem(IByteTransform byteTransform, byte[] content, Encoding encoding)
	{
		AlarmCode = byteTransform.TransInt32(content, 0);
		Time = Convert.ToDateTime(Encoding.ASCII.GetString(content, 12, 16));
		Message = encoding.GetString(content.RemoveBegin(28)).Trim('\0');
	}

	public override string ToString()
	{
		return $"[{AlarmCode}] Time:[{Time}] {Message}";
	}
}
