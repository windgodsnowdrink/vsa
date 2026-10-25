// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Algorithms.ConnectPool;

namespace HslCommunication.MQTT;

/// <summary>
/// 关于MqttSyncClient实现的接口，从而实现了数据连接池的操作信息
/// </summary>
public class IMqttSyncConnector : IConnector
{
	private MqttConnectionOptions connectionOptions;

	/// <summary>
	/// 指示当前的连接是否在使用用
	/// </summary>
	public bool IsConnectUsing { get; set; }

	/// <summary>
	/// 唯一的GUID码
	/// </summary>
	public string GuidToken { get; set; }

	/// <summary>
	/// 最新一次使用的时间
	/// </summary>
	public DateTime LastUseTime { get; set; }

	/// <summary>
	/// MQTT的连接对象
	/// </summary>
	public MqttSyncClient SyncClient { get; set; }

	/// <summary>
	/// 根据连接的MQTT参数，实例化一个默认的对象
	/// According to the connected MQTT parameters, instantiate a default object
	/// </summary>
	/// <param name="options">连接的参数信息</param>
	public IMqttSyncConnector(MqttConnectionOptions options)
	{
		connectionOptions = options;
		SyncClient = new MqttSyncClient(options);
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public IMqttSyncConnector()
	{
	}

	/// <summary>
	/// 关闭并释放
	/// </summary>
	public void Close()
	{
		SyncClient?.ConnectClose();
	}

	/// <summary>
	/// 打开连接
	/// </summary>
	public void Open()
	{
	}
}
