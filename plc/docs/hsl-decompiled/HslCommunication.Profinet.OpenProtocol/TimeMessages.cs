// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// Time messages
/// </summary>
public class TimeMessages
{
	private OpenProtocolNet openProtocol;

	/// <summary>
	/// 指定Open通信类实例化一个对象
	/// </summary>
	/// <param name="openProtocol">开放协议的对象</param>
	public TimeMessages(OpenProtocolNet openProtocol)
	{
		this.openProtocol = openProtocol;
	}

	/// <summary>
	/// Read time request.
	/// </summary>
	/// <returns>包含时间的结果对象</returns>
	public OperateResult<DateTime> ReadTimeUpload()
	{
		OperateResult<string> operateResult = openProtocol.ReadCustomer(80, 1, -1, -1, null);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(operateResult);
		}
		return OperateResult.CreateSuccessResult(DateTime.ParseExact(operateResult.Content.Substring(20, 19), "yyyy-MM-dd:HH:mm:ss", null));
	}

	/// <summary>
	/// Set the time in the controller.
	/// </summary>
	/// <param name="dateTime">指定的时间</param>
	/// <returns>是否设置成功的结果对象</returns>
	public OperateResult SetTime(DateTime dateTime)
	{
		return openProtocol.ReadCustomer(82, 1, -1, -1, new List<string> { dateTime.ToString("yyyy-MM-dd:HH:mm:ss") });
	}
}
