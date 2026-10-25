// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Sockets;
using HslCommunication.Core;
using HslCommunication.LogNet;

namespace HslCommunication.Profinet.Knx;

/// <summary>
/// Knx驱动，具体的用法参照demo
/// </summary>
/// <remarks>
/// 感谢上海NULL提供的技术支持
/// </remarks>
public class KnxUdp
{
	private const int stateRequestTimerInterval = 60000;

	private IPEndPoint _localEndpoint;

	private IPEndPoint _rouEndpoint;

	private KnxCode KNX_CODE;

	private UdpClient udpClient;

	private ILogNet logNet;

	/// <summary>
	/// 通道号（由设备发来）
	/// </summary>
	public byte Channel { get; set; }

	/// <summary>
	/// 远程ip地址
	/// </summary>
	public IPEndPoint RouEndpoint
	{
		get
		{
			return _rouEndpoint;
		}
		set
		{
			_rouEndpoint = value;
		}
	}

	/// <summary>
	/// 本机IP地址
	/// </summary>
	public IPEndPoint LocalEndpoint
	{
		get
		{
			return _localEndpoint;
		}
		set
		{
			_localEndpoint = value;
		}
	}

	/// <summary>
	/// 系统的日志信息
	/// </summary>
	public ILogNet LogNet
	{
		get
		{
			return logNet;
		}
		set
		{
			logNet = value;
		}
	}

	public bool IsConnect => KNX_CODE.IsConnect;

	public KnxCode KnxCode => KNX_CODE;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public KnxUdp()
	{
		KNX_CODE = new KnxCode();
	}

	/// <summary>
	/// 和KNX网络进行握手并开始监听
	/// </summary>
	public void ConnectKnx()
	{
		if (udpClient == null)
		{
			udpClient = new UdpClient(LocalEndpoint)
			{
				Client = 
				{
					DontFragment = true,
					SendBufferSize = 0,
					ReceiveTimeout = 120000
				}
			};
		}
		int num = udpClient.Send(KNX_CODE.Handshake(LocalEndpoint), 26, RouEndpoint);
		udpClient.BeginReceive(ReceiveCallback, null);
		HslHelper.ThreadSleep(1000);
		if (KNX_CODE.IsConnect)
		{
			KNX_CODE.Return_data_msg += KNX_CODE_Return_data_msg;
			KNX_CODE.GetData_msg += KNX_CODE_GetData_msg;
			KNX_CODE.Set_knx_data += KNX_CODE_Set_knx_data;
			KNX_CODE.knx_server_is_real(LocalEndpoint);
		}
	}

	/// <summary>
	/// 保持KNX连接
	/// </summary>
	public void KeepConnection()
	{
		KNX_CODE.knx_server_is_real(LocalEndpoint);
	}

	/// <summary>
	/// 关闭连接
	/// </summary>
	public void DisConnectKnx()
	{
		if (KNX_CODE.Channel != 0)
		{
			byte[] array = KNX_CODE.Disconnect_knx(KNX_CODE.Channel, LocalEndpoint);
			udpClient.Send(array, array.Length, RouEndpoint);
		}
	}

	/// <summary>
	/// 将报文写入KNX系统
	/// </summary>
	/// <param name="addr">地址</param>
	/// <param name="len">长度</param>
	/// <param name="data">数据</param>
	public void SetKnxData(short addr, byte len, byte[] data)
	{
		KNX_CODE.Knx_Write(addr, len, data);
	}

	/// <summary>
	/// 读取指定KNX组地址
	/// </summary>
	/// <param name="addr">地址</param>
	public void ReadKnxData(short addr)
	{
		KNX_CODE.knx_server_is_real(LocalEndpoint);
		KNX_CODE.Knx_Resd_step1(addr);
	}

	private void KNX_CODE_Set_knx_data(byte[] data)
	{
		udpClient.Send(data, data.Length, RouEndpoint);
	}

	private void KNX_CODE_GetData_msg(short addr, byte len, byte[] data)
	{
		logNet?.WriteDebug("收到数据 地址：" + addr + " 长度:" + len + "数据：" + BitConverter.ToString(data));
	}

	private void KNX_CODE_Return_data_msg(byte[] data)
	{
		udpClient.Send(data, data.Length, RouEndpoint);
	}

	private void ReceiveCallback(IAsyncResult iar)
	{
		byte[] array = udpClient.EndReceive(iar, ref _rouEndpoint);
		logNet?.WriteDebug("收到报文 {0}", BitConverter.ToString(array));
		KNX_CODE.KNX_check(array);
		if (KNX_CODE.IsConnect)
		{
			udpClient.BeginReceive(ReceiveCallback, null);
		}
	}
}
