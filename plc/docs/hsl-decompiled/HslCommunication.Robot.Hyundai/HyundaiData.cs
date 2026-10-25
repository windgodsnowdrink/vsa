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
using HslCommunication.BasicFramework;

namespace HslCommunication.Robot.Hyundai;

/// <summary>
/// Hyundai的数据类对象
/// </summary>
public class HyundaiData
{
	/// <summary>
	/// 命令码，从控制器发数据到PC和PC到控制器，两者的命令不一样
	/// </summary>
	public char Command { get; set; }

	/// <summary>
	/// 虚标记
	/// </summary>
	public string CharDummy { get; set; }

	/// <summary>
	/// 状态码
	/// </summary>
	public int State { get; set; }

	/// <summary>
	/// 标记数据，从PLC发送给机器人的数据，原封不动的返回
	/// </summary>
	public int Count { get; set; }

	/// <summary>
	/// 虚标记
	/// </summary>
	public int IntDummy { get; set; }

	/// <summary>
	/// 关节坐标数据，包含X,Y,Z,W,P,R，三个位置数据，三个角度数据。
	/// </summary>
	public double[] Data { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public HyundaiData()
	{
		Data = new double[6];
	}

	/// <summary>
	/// 通过缓存对象实例化一个
	/// </summary>
	public HyundaiData(byte[] buffer)
	{
		LoadBy(buffer);
	}

	/// <summary>
	/// 从字节数组的指定索引开始加载现在机器人的数据
	/// </summary>
	/// <param name="buffer">原始的字节数据</param>
	/// <param name="index">起始的索引信息</param>
	public void LoadBy(byte[] buffer, int index = 0)
	{
		Command = (char)buffer[index];
		CharDummy = Encoding.ASCII.GetString(buffer, index + 1, 3);
		State = BitConverter.ToInt32(buffer, index + 4);
		Count = BitConverter.ToInt32(buffer, index + 8);
		IntDummy = BitConverter.ToInt32(buffer, index + 12);
		Data = new double[6];
		for (int i = 0; i < Data.Length; i++)
		{
			if (i < 3)
			{
				Data[i] = BitConverter.ToDouble(buffer, index + 16 + 8 * i) * 1000.0;
			}
			else
			{
				Data[i] = BitConverter.ToDouble(buffer, index + 16 + 8 * i) * 180.0 / Math.PI;
			}
		}
	}

	/// <summary>
	/// 将现代机器人的数据转换为字节数组
	/// </summary>
	/// <returns>字节数组</returns>
	public byte[] ToBytes()
	{
		byte[] buffer = new byte[64];
		buffer[0] = (byte)Command;
		if (!string.IsNullOrEmpty(CharDummy))
		{
			Encoding.ASCII.GetBytes(CharDummy).CopyTo(buffer, 1);
		}
		BitConverter.GetBytes(State).CopyTo(buffer, 4);
		BitConverter.GetBytes(Count).CopyTo(buffer, 8);
		BitConverter.GetBytes(IntDummy).CopyTo(buffer, 12);
		for (int i = 0; i < Data.Length; i++)
		{
			if (i < 3)
			{
				BitConverter.GetBytes(Data[i] / 1000.0).CopyTo(buffer, 16 + 8 * i);
			}
			else
			{
				BitConverter.GetBytes(Data[i] * Math.PI / 180.0).CopyTo(buffer, 16 + 8 * i);
			}
		}
		return buffer;
	}

	public override string ToString()
	{
		return $"HyundaiData:Cmd[{Command},{CharDummy},{State},{Count},{IntDummy}] Data:{SoftBasic.ArrayFormat(Data)}";
	}
}
