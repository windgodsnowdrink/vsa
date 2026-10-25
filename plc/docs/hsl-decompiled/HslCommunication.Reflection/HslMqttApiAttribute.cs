// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Reflection;

/// <summary>
/// 可以指定方法变成对外公开的API接口，如果方法不实现该特性，将不对外公开方法，无法获取相关的接口权限
/// You can specify the method to become an externally public API interface. If the method does not implement this feature,
/// the method will not be publicly disclosed, and the related interface permissions cannot be obtained
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
public class HslMqttApiAttribute : Attribute
{
	/// <summary>
	/// 当前指定的ApiTopic信息，如果当前的方法接口不指定别名，那么就使用当前的方法名称
	/// The currently specified ApiTopic information, if the current method interface does not specify an alias,
	/// then the current method name is used
	/// </summary>
	public string ApiTopic { get; set; }

	/// <summary>
	/// 当前方法的注释内容
	/// The comment content of the current method
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 当前的属性是否需要展开API信息，默认不展开
	/// Whether the current attribute needs to expand the API information, it is not expanded by default
	/// </summary>
	public bool PropertyUnfold { get; set; } = false;

	/// <summary>
	/// 如果当前的API接口是支持Http的请求方式，当前属性有效，例如GET,POST
	/// If the current API interface is a request method that supports Http, the current attributes are valid, such as GET, POST
	/// </summary>
	public string HttpMethod { get; set; } = "POST";

	/// <summary>
	/// 指定描述内容来实例化一个的对象
	/// Specify the description content to instantiate an object
	/// </summary>
	/// <param name="description">当前接口的描述信息</param>
	public HslMqttApiAttribute(string description)
	{
		Description = description;
	}

	/// <summary>
	/// 指定接口的路由信息及描述内容来实例化一个的对象
	/// Specify the routing information and description content of the interface to instantiate an object
	/// </summary>
	/// <param name="apiTopic">指重新定当前接口的路由信息</param>
	/// <param name="description">当前接口的描述信息</param>
	public HslMqttApiAttribute(string apiTopic, string description)
	{
		ApiTopic = apiTopic;
		Description = description;
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public HslMqttApiAttribute()
	{
	}
}
