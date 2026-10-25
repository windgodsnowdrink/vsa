// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text.RegularExpressions;

namespace HslCommunication.Core.Address;

/// <summary>
/// 富士CommandSettingsType的协议信息
/// </summary>
public class FujiCommandSettingTypeAddress : DeviceAddressDataBase
{
	/// <summary>
	/// 数据的代号信息
	/// </summary>
	public byte DataCode { get; set; }

	/// <summary>
	/// 地址的头信息，缓存的情况
	/// </summary>
	public string AddressHeader { get; set; }

	public override void Parse(string address, ushort length)
	{
		base.Parse(address, length);
	}

	public override string ToString()
	{
		return AddressHeader + base.AddressStart;
	}

	/// <summary>
	/// 从字符串地址解析fuji的实际地址信息，如果解析成功，则  为 True，取  值即可。
	/// </summary>
	/// <param name="address">字符串地址</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>是否解析成功</returns>
	public static OperateResult<FujiCommandSettingTypeAddress> ParseFrom(string address, ushort length)
	{
		try
		{
			FujiCommandSettingTypeAddress fujiCommandSettingTypeAddress = new FujiCommandSettingTypeAddress();
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (address.IndexOf('.') < 0)
			{
				Match match = Regex.Match(address, "^[A-Z]+");
				if (!match.Success)
				{
					return new OperateResult<FujiCommandSettingTypeAddress>(StringResources.Language.NotSupportedDataType);
				}
				empty = match.Value;
				empty2 = address.Substring(empty.Length);
			}
			else
			{
				string[] array = address.SplitDot();
				if (array[0][0] != 'W')
				{
					return new OperateResult<FujiCommandSettingTypeAddress>(StringResources.Language.NotSupportedDataType);
				}
				empty = array[0];
				empty2 = array[1];
			}
			fujiCommandSettingTypeAddress.AddressHeader = empty;
			fujiCommandSettingTypeAddress.AddressStart = Convert.ToInt32(empty2);
			fujiCommandSettingTypeAddress.Length = length;
			switch (empty)
			{
			case "TS":
				fujiCommandSettingTypeAddress.DataCode = 10;
				break;
			case "TR":
				fujiCommandSettingTypeAddress.DataCode = 11;
				break;
			case "CS":
				fujiCommandSettingTypeAddress.DataCode = 12;
				break;
			case "CR":
				fujiCommandSettingTypeAddress.DataCode = 13;
				break;
			case "BD":
				fujiCommandSettingTypeAddress.DataCode = 14;
				break;
			case "WL":
				fujiCommandSettingTypeAddress.DataCode = 20;
				break;
			case "B":
				fujiCommandSettingTypeAddress.DataCode = 0;
				break;
			case "M":
				fujiCommandSettingTypeAddress.DataCode = 1;
				break;
			case "K":
				fujiCommandSettingTypeAddress.DataCode = 2;
				break;
			case "F":
				fujiCommandSettingTypeAddress.DataCode = 3;
				break;
			case "A":
				fujiCommandSettingTypeAddress.DataCode = 4;
				break;
			case "D":
				fujiCommandSettingTypeAddress.DataCode = 5;
				break;
			case "S":
				fujiCommandSettingTypeAddress.DataCode = 8;
				break;
			default:
				if (empty.StartsWith("W"))
				{
					int num = Convert.ToInt32(empty.Substring(1));
					if (num == 9)
					{
						fujiCommandSettingTypeAddress.DataCode = 9;
						break;
					}
					if (num >= 21 && num <= 26)
					{
						fujiCommandSettingTypeAddress.DataCode = (byte)num;
						break;
					}
					if (num >= 30 && num <= 109)
					{
						fujiCommandSettingTypeAddress.DataCode = (byte)num;
						break;
					}
					if (num >= 120 && num <= 123)
					{
						fujiCommandSettingTypeAddress.DataCode = (byte)num;
						break;
					}
					if (num == 125)
					{
						fujiCommandSettingTypeAddress.DataCode = (byte)num;
						break;
					}
					throw new Exception(StringResources.Language.NotSupportedDataType);
				}
				throw new Exception(StringResources.Language.NotSupportedDataType);
			}
			return OperateResult.CreateSuccessResult(fujiCommandSettingTypeAddress);
		}
		catch (Exception ex)
		{
			return new OperateResult<FujiCommandSettingTypeAddress>(DeviceAddressDataBase.GetUnsupportedAddressInfo(address, ex));
		}
	}
}
