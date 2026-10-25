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

namespace HslCommunication.Robot.FANUC;

/// <summary>
/// Fanuc机器人的任务类
/// </summary>
public class FanucTask
{
	/// <summary>
	/// ProgramName
	/// </summary>
	public string ProgramName { get; set; }

	/// <summary>
	/// LineNumber
	/// </summary>
	public short LineNumber { get; set; }

	/// <summary>
	/// State
	/// </summary>
	public short State { get; set; }

	/// <summary>
	/// ParentProgramName
	/// </summary>
	public string ParentProgramName { get; set; }

	/// <summary>
	/// 从原始的数据对象加载数据信息
	/// </summary>
	/// <param name="byteTransform">字节变换</param>
	/// <param name="content">原始的字节数据</param>
	/// <param name="index">索引信息</param>
	/// <param name="encoding">编码</param>
	public void LoadByContent(IByteTransform byteTransform, byte[] content, int index, Encoding encoding)
	{
		ProgramName = encoding.GetString(content, index, 16).Trim('\0');
		LineNumber = BitConverter.ToInt16(content, index + 16);
		State = BitConverter.ToInt16(content, index + 18);
		ParentProgramName = encoding.GetString(content, index + 20, 16).Trim('\0');
	}

	public override string ToString()
	{
		return $"ProgramName[{ProgramName}] LineNumber[{LineNumber}] State[{State}] ParentProgramName[{ParentProgramName}]";
	}

	/// <summary>
	/// 从原始的数据信息初始化一个任务对象
	/// </summary>
	/// <param name="byteTransform">字节变换</param>
	/// <param name="content">原始的字节数据</param>
	/// <param name="index">索引信息</param>
	/// <param name="encoding">编码</param>
	/// <returns>任务对象</returns>
	public static FanucTask ParseFrom(IByteTransform byteTransform, byte[] content, int index, Encoding encoding)
	{
		FanucTask fanucTask = new FanucTask();
		fanucTask.LoadByContent(byteTransform, content, index, encoding);
		return fanucTask;
	}
}
