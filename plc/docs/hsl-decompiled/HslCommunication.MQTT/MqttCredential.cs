// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.MQTT;

/// <summary>
/// Mqtt协议的验证对象，包含用户名和密码
/// Authentication object of Mqtt protocol, including username and password
/// </summary>
public class MqttCredential
{
	/// <summary>
	/// 获取或设置用户名
	/// Get or set username
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	/// 获取或设置密码
	/// Get or set password
	/// </summary>
	public string Password { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public MqttCredential()
	{
	}

	/// <summary>
	/// 实例化指定的用户名和密码的对象
	/// Instantiates an object with the specified username and password
	/// </summary>
	/// <param name="name">用户名</param>
	/// <param name="pwd">密码</param>
	public MqttCredential(string name, string pwd)
	{
		UserName = name;
		Password = pwd;
	}

	public override string ToString()
	{
		return UserName;
	}
}
