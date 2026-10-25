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
/// 永宏编程口的地址类对象
/// </summary>
public class FatekProgramAddress : DeviceAddressDataBase
{
	/// <summary>
	/// 数据的类型
	/// </summary>
	public string DataCode { get; set; }

	public override void Parse(string address, ushort length)
	{
		OperateResult<FatekProgramAddress> operateResult = ParseFrom(address, length);
		if (operateResult.IsSuccess)
		{
			base.AddressStart = operateResult.Content.AddressStart;
			base.Length = operateResult.Content.Length;
			DataCode = operateResult.Content.DataCode;
		}
	}

	public override string ToString()
	{
		if (DataCode == "X" || DataCode == "Y" || DataCode == "M" || DataCode == "S" || DataCode == "T" || DataCode == "C" || DataCode == "RT" || DataCode == "RC")
		{
			return DataCode + base.AddressStart.ToString("D4");
		}
		return DataCode + base.AddressStart.ToString("D5");
	}

	/// <summary>
	/// 从普通的PLC的地址转换为HSL标准的地址信息
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <returns>是否成功的地址结果</returns>
	public static OperateResult<FatekProgramAddress> ParseFrom(string address, ushort length)
	{
		try
		{
			FatekProgramAddress fatekProgramAddress = new FatekProgramAddress();
			switch (address[0])
			{
			case 'X':
			case 'x':
				fatekProgramAddress.DataCode = "X";
				fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				break;
			case 'Y':
			case 'y':
				fatekProgramAddress.DataCode = "Y";
				fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				break;
			case 'M':
			case 'm':
				fatekProgramAddress.DataCode = "M";
				fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				break;
			case 'S':
			case 's':
				fatekProgramAddress.DataCode = "S";
				fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				break;
			case 'T':
			case 't':
				fatekProgramAddress.DataCode = "T";
				fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				break;
			case 'C':
			case 'c':
				fatekProgramAddress.DataCode = "C";
				fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				break;
			case 'D':
			case 'd':
				fatekProgramAddress.DataCode = "D";
				fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				break;
			case 'R':
			case 'r':
				if (address[1] == 'T' || address[1] == 't')
				{
					fatekProgramAddress.DataCode = "RT";
					fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(2), 10);
				}
				else if (address[1] == 'C' || address[1] == 'c')
				{
					fatekProgramAddress.DataCode = "RC";
					fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(2), 10);
				}
				else
				{
					fatekProgramAddress.DataCode = "R";
					fatekProgramAddress.AddressStart = Convert.ToUInt16(address.Substring(1), 10);
				}
				break;
			default:
				throw new Exception(StringResources.Language.NotSupportedDataType);
			}
			return OperateResult.CreateSuccessResult(fatekProgramAddress);
		}
		catch (Exception ex)
		{
			return new OperateResult<FatekProgramAddress>(DeviceAddressDataBase.GetUnsupportedAddressInfo(address, ex));
		}
	}
}
