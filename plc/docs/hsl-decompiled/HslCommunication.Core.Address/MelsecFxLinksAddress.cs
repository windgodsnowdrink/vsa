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
/// 三菱的FxLinks协议信息
/// </summary>
public class MelsecFxLinksAddress : DeviceAddressDataBase
{
	/// <summary>
	/// 当前的地址类型信息
	/// </summary>
	public string TypeCode { get; set; }

	public override void Parse(string address, ushort length)
	{
		base.Parse(address, length);
	}

	public override string ToString()
	{
		switch (TypeCode)
		{
		case "X":
		case "Y":
			return TypeCode + Convert.ToString(base.AddressStart, 8).PadLeft((base.AddressStart >= 10000) ? 6 : 4, '0');
		default:
			return TypeCode + base.AddressStart.ToString("D" + (((base.AddressStart >= 10000) ? 7 : 5) - TypeCode.Length));
		}
	}

	public static OperateResult<MelsecFxLinksAddress> ParseFrom(string address)
	{
		return ParseFrom(address, 0);
	}

	/// <summary>
	/// 从三菱FxLinks协议里面解析出实际的地址信息
	/// </summary>
	/// <param name="address">三菱的地址信息</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>解析结果信息</returns>
	public static OperateResult<MelsecFxLinksAddress> ParseFrom(string address, ushort length)
	{
		MelsecFxLinksAddress melsecFxLinksAddress = new MelsecFxLinksAddress();
		melsecFxLinksAddress.Length = length;
		try
		{
			switch (address[0])
			{
			case 'X':
			case 'x':
				melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 8);
				melsecFxLinksAddress.TypeCode = "X";
				break;
			case 'Y':
			case 'y':
				melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 8);
				melsecFxLinksAddress.TypeCode = "Y";
				break;
			case 'M':
			case 'm':
				melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				melsecFxLinksAddress.TypeCode = "M";
				break;
			case 'S':
			case 's':
				melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				melsecFxLinksAddress.TypeCode = "S";
				break;
			case 'T':
			case 't':
				if (address[1] == 'S' || address[1] == 's')
				{
					melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(2), 10);
					melsecFxLinksAddress.TypeCode = "TS";
					break;
				}
				if (address[1] == 'N' || address[1] == 'n')
				{
					melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(2), 10);
					melsecFxLinksAddress.TypeCode = "TN";
					break;
				}
				throw new Exception(StringResources.Language.NotSupportedDataType);
			case 'C':
			case 'c':
				if (address[1] == 'S' || address[1] == 's')
				{
					melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(2), 10);
					melsecFxLinksAddress.TypeCode = "CS";
					break;
				}
				if (address[1] == 'N' || address[1] == 'n')
				{
					melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(2), 10);
					melsecFxLinksAddress.TypeCode = "CN";
					break;
				}
				throw new Exception(StringResources.Language.NotSupportedDataType);
			case 'D':
			case 'd':
				melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				melsecFxLinksAddress.TypeCode = "D";
				break;
			case 'R':
			case 'r':
				melsecFxLinksAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				melsecFxLinksAddress.TypeCode = "R";
				break;
			default:
				throw new Exception(StringResources.Language.NotSupportedDataType);
			}
			return OperateResult.CreateSuccessResult(melsecFxLinksAddress);
		}
		catch (Exception ex)
		{
			return new OperateResult<MelsecFxLinksAddress>(DeviceAddressDataBase.GetUnsupportedAddressInfo(address, ex));
		}
	}
}
