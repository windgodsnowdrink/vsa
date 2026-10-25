// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core.Address;

/// <summary>
/// 扩展memobus协议的地址信息
/// </summary>
public class MemobusAddress : DeviceAddressDataBase
{
	/// <summary>
	/// 获取或设置当前地址对应的功能码信息
	/// </summary>
	public byte SFC { get; set; }

	/// <summary>
	/// 获取或设置当前的地址对应的主功能码信息
	/// </summary>
	public byte MFC { get; set; }

	public override string ToString()
	{
		return base.AddressStart.ToString();
	}

	/// <summary>
	/// 获取并解析出memobus地址的信息及功能码
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="isBit">是否位</param>
	/// <returns>memobus的地址信息</returns>
	public static OperateResult<MemobusAddress> ParseFrom(string address, bool isBit)
	{
		try
		{
			MemobusAddress memobusAddress = new MemobusAddress();
			memobusAddress.MFC = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
			memobusAddress.SFC = (byte)HslHelper.ExtractParameter(ref address, "x", isBit ? 1 : 3);
			if (address.IndexOf('.') > 0)
			{
				int num = address.IndexOf('.');
				memobusAddress.AddressStart = Convert.ToInt32(address.Substring(0, num)) * 16 + HslHelper.CalculateBitStartIndex(address.Substring(num + 1));
				if (memobusAddress.SFC == 1)
				{
					memobusAddress.SFC = 3;
				}
			}
			else
			{
				memobusAddress.AddressStart = ushort.Parse(address);
			}
			return OperateResult.CreateSuccessResult(memobusAddress);
		}
		catch (Exception ex)
		{
			return new OperateResult<MemobusAddress>(DeviceAddressDataBase.GetUnsupportedAddressInfo(address, ex));
		}
	}
}
