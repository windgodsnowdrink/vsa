// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙Cpu的状态信息
/// </summary>
public class OmronCpuUnitStatus
{
	/// <summary>
	/// Run 或是 Stop
	/// </summary>
	public string Status { get; set; }

	/// <summary>
	/// No 或是 Present
	/// </summary>
	public string BatteryStatus { get; set; }

	/// <summary>
	/// Normal 或是 Standby
	/// </summary>
	public string CpuStatus { get; set; }

	/// <summary>
	/// PROGRAM, MONITOR, RUN
	/// </summary>
	public string Mode { get; set; }

	/// <summary>
	/// Among errors that occur when the command is executed, the error code indicates the most serious. If there are no errors, it will be 0000 (hex)
	/// </summary>
	public int ErrorCode { get; set; }

	/// <summary>
	/// Indicates messages from execution of FAL(006) or FALS(007). If there is no error message,
	/// or if FAL(006) or FALS(007) are not being executed, 16 spaces( ASCII 20) will be returned.
	/// </summary>
	public string ErrorMessage { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public OmronCpuUnitStatus()
	{
	}

	/// <summary>
	/// 从原始的字节数组来实例化一个
	/// </summary>
	/// <param name="data">原始的字节数据</param>
	public OmronCpuUnitStatus(byte[] data)
	{
		Status = (data[0].GetBoolByIndex(0) ? "Run" : "Stop");
		BatteryStatus = (data[0].GetBoolByIndex(2) ? "Present" : "No");
		CpuStatus = (data[0].GetBoolByIndex(7) ? "Standby" : "Normal");
		Mode = ((data[1] == 0) ? "PROGRAM" : ((data[1] == 2) ? "MONITOR" : ((data[1] == 4) ? "RUN" : "")));
		ErrorCode = data[8] * 256 + data[9];
		if (ErrorCode > 0)
		{
			ErrorMessage = Encoding.ASCII.GetString(data, 10, 16).TrimEnd(' ', '\0');
		}
	}

	public override string ToString()
	{
		return "OmronCpuUnitStatus[" + Status + "]";
	}
}
