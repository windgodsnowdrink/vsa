// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Reflection;
using System.Threading;
using HslCommunication.Reflection;
using Newtonsoft.Json;

namespace HslCommunication.MQTT;

/// <summary>
/// Mqtt的同步网络服务的单Api信息描述类
/// Single Api information description class of Mqtt's synchronous network service
/// </summary>
public class MqttRpcApiInfo
{
	private long calledCount = 0L;

	private long spendTotalTime = 0L;

	private MethodInfo method;

	private PropertyInfo property;

	/// <summary>
	/// 当前的Api的路由信息，对于注册服务来说，是类名/方法名
	/// </summary>
	public string ApiTopic { get; set; }

	/// <summary>
	/// 当前的Api的路由说明
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 当前方法的签名
	/// </summary>
	public string MethodSignature { get; set; }

	/// <summary>
	/// 当前Api的调用次数
	/// </summary>
	public long CalledCount
	{
		get
		{
			return calledCount;
		}
		set
		{
			calledCount = value;
		}
	}

	/// <summary>
	/// 示例的
	/// </summary>
	public string ExamplePayload { get; set; }

	/// <summary>
	/// 当前Api的调用总耗时，单位是秒
	/// </summary>
	public double SpendTotalTime
	{
		get
		{
			return (double)spendTotalTime / 100000.0;
		}
		set
		{
			spendTotalTime = (long)(value * 100000.0);
		}
	}

	/// <summary>
	/// 当前Api是否为方法，如果是方法，就为true，否则为false
	/// </summary>
	public bool IsMethodApi { get; set; }

	/// <summary>
	/// 如果当前的API接口是支持Http的请求方式，当前属性有效，例如GET,POST
	/// </summary>
	public string HttpMethod { get; set; } = "GET";

	[JsonIgnore]
	/// <summary>
	/// 当前的Api的方法是否是异步的Task类型
	/// </summary>
	public bool IsOperateResultApi { get; set; }

	[JsonIgnore]
	/// <summary>
	/// 当前的Api关联的方法反射，本属性在JSON中将会忽略
	/// </summary>
	public MethodInfo Method
	{
		get
		{
			return method;
		}
		set
		{
			method = value;
			IsMethodApi = true;
		}
	}

	[JsonIgnore]
	/// <summary>
	/// 当前的Api关联的方法反射，本属性在JSON中将会忽略
	/// </summary>
	public PropertyInfo Property
	{
		get
		{
			return property;
		}
		set
		{
			property = value;
			IsMethodApi = false;
		}
	}

	[JsonIgnore]
	/// <summary>
	/// 当前Api的方法的权限访问反射，本属性在JSON中将会忽略
	/// </summary>
	public HslMqttPermissionAttribute PermissionAttribute { get; set; }

	[JsonIgnore]
	/// <summary>
	/// 当前Api绑定的对象的，实际的接口请求，将会从对象进行调用，本属性在JSON中将会忽略
	/// </summary>
	public object SourceObject { get; set; }

	[JsonIgnore]
	/// <summary>
	/// 用户用于自定义的对象信息
	/// </summary>
	public object Tag { get; set; }

	/// <summary>
	/// 使用原子的操作增加一次调用次数的数据信息，需要传入当前的消耗的时间，单位为100倍毫秒
	/// </summary>
	/// <param name="timeSpend">当前调用花费的时间，单位为100倍毫秒</param>
	public void CalledCountAddOne(long timeSpend)
	{
		Interlocked.Increment(ref calledCount);
		Interlocked.Add(ref spendTotalTime, timeSpend);
	}

	public override string ToString()
	{
		return ApiTopic;
	}
}
