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
/// Cnet协议的地址数据信息，未完成
/// </summary>
public class CnetAddressData : DeviceAddressDataBase
{
	/// <summary>
	/// 数据的代号，通常是 'P', 'M', 'L', 'K', 'F', 'T', 'C', 'D', 'S', 'Q', 'I', 'N', 'U', 'Z', 'R'
	/// </summary>
	public string DataCode { get; set; }

	/// <summary>
	/// 数据的类型，通常是 X,B,W,D,L
	/// </summary>
	public string DataType { get; set; }

	/// <summary>
	/// 从实际的PLC的地址里面解析出地址对象
	/// Resolve the address object from the actual PLC address
	/// </summary>
	/// <param name="address">西门子的地址数据信息</param>
	/// <returns>是否成功的结果对象</returns>
	public static OperateResult<CnetAddressData> ParseFrom(string address)
	{
		return ParseFrom(address, 0);
	}

	/// <summary>
	/// 从实际的PLC的地址里面解析出地址对象
	/// Resolve the address object from the actual PLC address
	/// </summary>
	/// <param name="address">PLC的地址数据信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <returns>是否成功的结果对象</returns>
	public static OperateResult<CnetAddressData> ParseFrom(string address, ushort length)
	{
		CnetAddressData cnetAddressData = new CnetAddressData();
		try
		{
			cnetAddressData.Length = length;
			if (address[1] != 'X')
			{
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<CnetAddressData>(DeviceAddressDataBase.GetUnsupportedAddressInfo(address, ex));
		}
		return OperateResult.CreateSuccessResult(cnetAddressData);
	}
}
