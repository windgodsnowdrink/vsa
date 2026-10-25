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
/// 欧姆的Cpu的单元信息数据类
/// </summary>
public class OmronCpuUnitData
{
	/// <summary>
	/// Cpu unit Model
	/// </summary>
	public string Model { get; set; }

	/// <summary>
	/// CPU Unit internal system version
	/// </summary>
	public string Version { get; set; }

	/// <summary>
	/// Largest number, 0 to 19, in CPU Unit’s EM area.
	/// </summary>
	public int LargestEMNumber { get; set; }

	/// <summary>
	/// Maximum size of usable program area，unit: k words
	/// </summary>
	public int ProgramAreaSize { get; set; }

	/// <summary>
	/// The size of the area (CIO, WR, HR, AR, timer/counter completion flags, TN) in which bit commands can be used( always 23). unit: bytes
	/// </summary>
	public int IOMSize { get; set; }

	/// <summary>
	/// Total words in the DM area (always 32,768)
	/// </summary>
	public int DMSize { get; set; }

	/// <summary>
	/// Among the banks in the EM area, the number of banks(0 to D ) without file memory
	/// </summary>
	/// <remarks>
	/// Banks (1 bank = 32,768 words)
	/// </remarks>
	public int EMSize { get; set; }

	/// <summary>
	/// Maximum number of timers/counters available (always 8)
	/// </summary>
	public int TCSize { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public OmronCpuUnitData()
	{
	}

	/// <summary>
	/// 根据原始的数据来实例化相关的CPU单元信息
	/// </summary>
	/// <param name="data">原始字节数</param>
	public OmronCpuUnitData(byte[] data)
	{
		Model = Encoding.ASCII.GetString(data, 0, 20).Trim(new char[1] { ' ' });
		Version = Encoding.ASCII.GetString(data, 20, 10).Trim(' ', '\0');
		LargestEMNumber = data[41];
		ProgramAreaSize = data[80] * 256 + data[81];
		IOMSize = data[82] * 1024;
		DMSize = data[83] * 256 + data[84];
		TCSize = data[85] * 1024;
		EMSize = data[86];
	}
}
