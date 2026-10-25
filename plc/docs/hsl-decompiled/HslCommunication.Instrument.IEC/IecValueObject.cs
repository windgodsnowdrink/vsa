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
using HslCommunication.Instrument.IEC.Helper;

namespace HslCommunication.Instrument.IEC;

public class IecValueObject<T>
{
	private T m_value;

	public T Value
	{
		get
		{
			return m_value;
		}
		set
		{
			bool success = false;
			if (!m_value.Equals(value))
			{
				success = true;
			}
			m_value = value;
			if (success)
			{
				OnValueChanged?.Invoke(this);
			}
		}
	}

	public byte Quality { get; set; }

	public DateTime Time { get; set; }

	public int Address { get; set; }

	public Action<IecValueObject<T>> OnValueChanged { get; set; }

	public object Tag { get; set; }

	public static List<IecValueObject<byte>> ParseYaoXinValue(IEC104MessageEventArgs message)
	{
		bool flag = message.WithTimeInfo();
		List<IecValueObject<byte>> list = new List<IecValueObject<byte>>();
		DateTime time = DateTime.MinValue;
		if (message.IsAddressContinuous)
		{
			if (flag && message.Body.Length >= message.InfoObjectCount + 3 + 7)
			{
				time = IECHelper.PraseTimeFromAbsoluteTimeScale(message.Body, message.InfoObjectCount + 3);
			}
			ushort num = BitConverter.ToUInt16(message.Body, 0);
			for (int i = 0; i < message.InfoObjectCount; i++)
			{
				int num2 = 3 + i;
				if (num2 >= message.Body.Length)
				{
					return list;
				}
				IecValueObject<byte> iecValueObject = new IecValueObject<byte>();
				iecValueObject.Address = num + i;
				iecValueObject.Value = (byte)(message.Body[num2] & 0xF);
				iecValueObject.Quality = (byte)(message.Body[num2] & 0xF0);
				if (flag)
				{
					iecValueObject.Time = time;
				}
				list.Add(iecValueObject);
			}
		}
		else
		{
			if (flag && message.Body.Length >= message.InfoObjectCount * 4 + 7)
			{
				time = IECHelper.PraseTimeFromAbsoluteTimeScale(message.Body, message.InfoObjectCount * 4);
			}
			for (int j = 0; j < message.InfoObjectCount; j++)
			{
				int num3 = 4 * j;
				if (num3 >= message.Body.Length)
				{
					return list;
				}
				IecValueObject<byte> iecValueObject2 = new IecValueObject<byte>();
				iecValueObject2.Address = BitConverter.ToUInt16(message.Body, num3);
				iecValueObject2.Value = (byte)(message.Body[num3 + 3] & 0xF);
				iecValueObject2.Quality = (byte)(message.Body[num3 + 3] & 0xF0);
				if (flag)
				{
					iecValueObject2.Time = time;
				}
				list.Add(iecValueObject2);
			}
		}
		return list;
	}

	public static List<IecValueObject<short>> ParseInt16Value(IEC104MessageEventArgs iEC104Message)
	{
		return IECHelper.ParseYaoCeValue(iEC104Message, BitConverter.ToInt16, 2);
	}

	public static List<IecValueObject<float>> ParseFloatValue(IEC104MessageEventArgs iEC104Message)
	{
		return IECHelper.ParseYaoCeValue(iEC104Message, BitConverter.ToSingle, 4);
	}

	public static List<IecValueObject<uint>> ParseUInt32Value(IEC104MessageEventArgs iEC104Message)
	{
		return IECHelper.ParseYaoCeValue(iEC104Message, BitConverter.ToUInt32, 4);
	}
}
