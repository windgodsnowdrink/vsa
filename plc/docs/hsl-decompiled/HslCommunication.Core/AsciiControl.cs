// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core;

/// <summary>
/// 控制型的ascii资源信息
/// Controlled ascii resource information
/// </summary>
public class AsciiControl
{
	/// <summary>
	/// 空字符
	/// </summary>
	public const byte NUL = 0;

	/// <summary>
	/// 标题开始
	/// start of headling
	/// </summary>
	public const byte SOH = 1;

	/// <summary>
	/// 正文开始
	/// start of text
	/// </summary>
	public const byte STX = 2;

	/// <summary>
	/// 正文结束
	/// end of text
	/// </summary>
	public const byte ETX = 3;

	/// <summary>
	/// 传输结束
	/// end of transmission
	/// </summary>
	public const byte EOT = 4;

	/// <summary>
	/// 请求
	/// enquiry
	/// </summary>
	public const byte ENQ = 5;

	/// <summary>
	/// 接到通知
	/// acknowledge
	/// </summary>
	public const byte ACK = 6;

	/// <summary>
	/// 响铃
	/// bell
	/// </summary>
	public const byte BEL = 7;

	/// <summary>
	/// 退格
	/// backspace
	/// </summary>
	public const byte BS = 8;

	/// <summary>
	/// 水平制表符
	/// horizontal tab
	/// </summary>
	public const byte HT = 9;

	/// <summary>
	/// 换行符
	/// NL line feed, new line
	/// </summary>
	public const byte LF = 10;

	/// <summary>
	/// 垂直制表符
	/// vertical tab
	/// </summary>
	public const byte VT = 11;

	/// <summary>
	/// 换页键
	/// NP form feed, new page
	/// </summary>
	public const byte FF = 12;

	/// <summary>
	/// 回车键
	/// carriage return
	/// </summary>
	public const byte CR = 13;

	/// <summary>
	/// 不用切换
	/// shift out
	/// </summary>
	public const byte SO = 14;

	/// <summary>
	/// 启用切换
	/// shift in
	/// </summary>
	public const byte SI = 15;

	/// <summary>
	/// 数据链路定义
	/// data link escape
	/// </summary>
	public const byte DLE = 16;

	/// <summary>
	/// 设备控制1
	/// device control 1
	/// </summary>
	public const byte DC1 = 17;

	/// <summary>
	/// 设备控制2
	/// device control 2
	/// </summary>
	public const byte DC2 = 18;

	/// <summary>
	/// 设备控制3
	/// device control 3
	/// </summary>
	public const byte DC3 = 19;

	/// <summary>
	/// 设备控制4
	/// device control 4
	/// </summary>
	public const byte DC4 = 20;

	/// <summary>
	/// 拒绝接收
	/// negative acknowledge
	/// </summary>
	public const byte NAK = 21;

	/// <summary>
	/// 同步空闲
	/// synchronous idle
	/// </summary>
	public const byte SYN = 22;

	/// <summary>
	/// 传输块结束
	/// end of trans. block
	/// </summary>
	public const byte ETB = 23;

	/// <summary>
	/// 取消
	/// cancel
	/// </summary>
	public const byte CAN = 24;

	/// <summary>
	/// 介质中断
	/// end of medium
	/// </summary>
	public const byte EM = 25;

	/// <summary>
	/// 替补
	/// substitute
	/// </summary>
	public const byte SUB = 26;

	/// <summary>
	/// 溢出
	/// escape
	/// </summary>
	public const byte ESC = 27;

	/// <summary>
	/// 文件分隔符
	/// file separator
	/// </summary>
	public const byte FS = 28;

	/// <summary>
	/// 分组符
	/// group separator
	/// </summary>
	public const byte GS = 29;

	/// <summary>
	/// 记录分离符
	/// record separator
	/// </summary>
	public const byte RS = 30;

	/// <summary>
	/// 单元分隔符
	/// unit separator
	/// </summary>
	public const byte US = 31;
}
