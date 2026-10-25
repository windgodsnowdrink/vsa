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
using HslCommunication.BasicFramework;
using HslCommunication.Serial;

namespace HslCommunication.Secs.Helper;

/// <summary>
/// Secs-1的协议信息
/// </summary>
public class Secs1
{
	/// <summary>
	/// 根据传入的参数信息，构建完整的SECS消息报文列表
	/// </summary>
	/// <param name="deviceID">装置识别码</param>
	/// <param name="streamNo">主功能码</param>
	/// <param name="functionNo">子功能码</param>
	/// <param name="blockNo">数据块号</param>
	/// <param name="messageID">消息序号</param>
	/// <param name="data">真实数据消息</param>
	/// <param name="wBit">是否必须回复讯息</param>
	/// <returns>完整的报文信息</returns>
	public static List<byte[]> BuildSecsOneMessage(ushort deviceID, byte streamNo, byte functionNo, ushort blockNo, uint messageID, byte[] data, bool wBit)
	{
		List<byte[]> list = new List<byte[]>();
		List<byte[]> list2 = ((data.Length <= 244) ? SoftBasic.ArraySplitByLength(data, 244) : SoftBasic.ArraySplitByLength(data, 224));
		for (int i = 0; i < list2.Count; i++)
		{
			byte[] buffer = new byte[13 + list2[i].Length];
			buffer[0] = (byte)(10 + list2[i].Length);
			buffer[1] = BitConverter.GetBytes(deviceID)[1];
			buffer[2] = BitConverter.GetBytes(deviceID)[0];
			buffer[3] = (wBit ? ((byte)(streamNo | 0x80)) : streamNo);
			buffer[4] = functionNo;
			buffer[5] = ((i == list2.Count - 1) ? ((byte)(BitConverter.GetBytes(blockNo)[1] | 0x80)) : BitConverter.GetBytes(blockNo)[1]);
			buffer[6] = BitConverter.GetBytes(blockNo)[0];
			buffer[7] = BitConverter.GetBytes(messageID)[3];
			buffer[8] = BitConverter.GetBytes(messageID)[2];
			buffer[9] = BitConverter.GetBytes(messageID)[1];
			buffer[10] = BitConverter.GetBytes(messageID)[0];
			list2[i].CopyTo(buffer, 11);
			int value = SoftLRC.CalculateAcc(buffer, 1, 2);
			buffer[^2] = BitConverter.GetBytes(value)[1];
			buffer[^1] = BitConverter.GetBytes(value)[0];
			list.Add(buffer);
		}
		return list;
	}

	/// <summary>
	/// 根据传入的参数信息，构建完整的SECS/HSMS消息报文列表
	/// </summary>
	/// <param name="deviceID">装置识别码</param>
	/// <param name="streamNo">主功能码</param>
	/// <param name="functionNo">子功能码</param>
	/// <param name="blockNo">数据块号</param>
	/// <param name="messageID">消息序号</param>
	/// <param name="data">真实数据消息</param>
	/// <param name="wBit">是否必须回复讯息</param>
	/// <returns>完整的报文信息</returns>
	public static byte[] BuildHSMSMessage(ushort deviceID, byte streamNo, byte functionNo, ushort blockNo, uint messageID, byte[] data, bool wBit)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		byte[] buffer = new byte[14 + data.Length];
		buffer[0] = BitConverter.GetBytes(buffer.Length - 4)[3];
		buffer[1] = BitConverter.GetBytes(buffer.Length - 4)[2];
		buffer[2] = BitConverter.GetBytes(buffer.Length - 4)[1];
		buffer[3] = BitConverter.GetBytes(buffer.Length - 4)[0];
		buffer[4] = BitConverter.GetBytes(deviceID)[1];
		buffer[5] = BitConverter.GetBytes(deviceID)[0];
		buffer[6] = (wBit ? ((byte)(streamNo | 0x80)) : streamNo);
		buffer[7] = functionNo;
		buffer[8] = BitConverter.GetBytes(blockNo)[1];
		buffer[9] = BitConverter.GetBytes(blockNo)[0];
		buffer[10] = BitConverter.GetBytes(messageID)[3];
		buffer[11] = BitConverter.GetBytes(messageID)[2];
		buffer[12] = BitConverter.GetBytes(messageID)[1];
		buffer[13] = BitConverter.GetBytes(messageID)[0];
		data.CopyTo(buffer, 14);
		return buffer;
	}
}
