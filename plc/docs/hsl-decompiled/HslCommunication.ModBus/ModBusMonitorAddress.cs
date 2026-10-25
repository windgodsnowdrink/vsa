// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.ModBus;

/// <summary>
/// 服务器端提供的数据监视服务
/// </summary>
public class ModBusMonitorAddress
{
	/// <summary>
	/// 本次数据监视的地址
	/// </summary>
	public ushort Address { get; set; }

	/// <summary>
	/// 数据写入时触发的事件
	/// </summary>
	public event Action<ModBusMonitorAddress, short> OnWrite;

	/// <summary>
	/// 数据改变时触发的事件
	/// </summary>
	public event Action<ModBusMonitorAddress, short, short> OnChange;

	/// <summary>
	/// 强制设置触发事件
	/// </summary>
	/// <param name="value">数据值信息</param>
	public void SetValue(short value)
	{
		OnWrite?.Invoke(this, value);
	}

	/// <summary>
	/// 强制设置触发值变更事件
	/// </summary>
	/// <param name="before">变更前的值</param>
	/// <param name="after">变更后的值</param>
	public void SetChangeValue(short before, short after)
	{
		if (before != after)
		{
			OnChange?.Invoke(this, before, after);
		}
	}
}
