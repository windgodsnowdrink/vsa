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
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet.Tcp;

/// <summary>
/// 用于调试的远程服务器
/// </summary>
public class DebugRemoteServer : DeviceServer
{
	private Dictionary<int, BinaryCommunication> binaryCommunication;

	/// <summary>
	/// 默认的无参构造函数
	/// Default no-parameter constructor
	/// </summary>
	public DebugRemoteServer()
	{
		binaryCommunication = new Dictionary<int, BinaryCommunication>();
	}

	/// <summary>
	/// 设置当前的服务器关联的通信设备对象，默认的key为0，如果已经存在，则自动覆盖
	/// </summary>
	/// <param name="communication">设备通信对象</param>
	public void SetDeviceCommunication(BinaryCommunication communication)
	{
		if (binaryCommunication.ContainsKey(0))
		{
			binaryCommunication[0] = communication;
		}
		else
		{
			binaryCommunication.Add(0, communication);
		}
	}

	/// <summary>
	/// 新增加当前的服务器关联的通信设备对象，需要指定唯一标识，如果已经存在，则引发异常
	/// </summary>
	/// <param name="key">设备的唯一标识</param>
	/// <param name="communication">设备通信对象</param>
	public void AddDeviceCommunication(ushort key, BinaryCommunication communication)
	{
		binaryCommunication.Add(key, communication);
	}

	/// <summary>
	/// 移除一个设备的通信对象，如果存在的话，返回是否移除成功
	/// </summary>
	/// <param name="key">设备的唯一标识</param>
	public bool RemoveDeviceCommunication(ushort key)
	{
		if (binaryCommunication.ContainsKey(key))
		{
			return binaryCommunication.Remove(key);
		}
		return false;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new AdsNetMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive == null || receive.Length < 6)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		if (this.binaryCommunication == null)
		{
			base.LogNet?.WriteError(ToString(), "BinaryCommunication is null");
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		int num = BitConverter.ToUInt16(receive, 0);
		byte[] send = receive.RemoveBegin(6);
		if (!this.binaryCommunication.ContainsKey(num))
		{
			base.LogNet?.WriteError(ToString(), $"{session.Communication} Communication Key[{num}] is not exist. ");
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		BinaryCommunication binaryCommunication = this.binaryCommunication[num];
		OperateResult<byte[]> operateResult = binaryCommunication.ReadFromCoreServer(send, hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content);
	}

	public override string ToString()
	{
		return $"DebugRemoteServer[{base.Port}]";
	}
}
