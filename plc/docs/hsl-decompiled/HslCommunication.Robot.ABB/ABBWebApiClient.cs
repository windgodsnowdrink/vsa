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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;
using Newtonsoft.Json.Linq;

namespace HslCommunication.Robot.ABB;

/// <summary>
/// ABB机器人的web api接口的客户端，可以方便快速的获取到abb机器人的一些数据信息
/// The client of ABB robot's web API interface can easily and quickly obtain some data information of ABB robot
/// </summary>
/// <remarks>
/// 参考的界面信息是：http://developercenter.robotstudio.com/webservice/api_reference
/// https://developercenter.robotstudio.com/api/rwsApi/index.html
/// 关于额外的地址说明，如果想要查看，可以调用 返回字符串列表来看看。
/// </remarks>
public class ABBWebApiClient : NetworkWebApiRobotBase, IRobotNet
{
	private const string ErrorState = "ErrorState";

	private const string JointTarget = "jointtarget";

	private const string PhysicalJoints = "PhysicalJoints";

	private const string SpeedRatio = "SpeedRatio";

	private const string OperationMode = "OperationMode";

	private const string CtrlState = "CtrlState";

	private const string IOIn = "ioin";

	private const string IOOut = "ioout";

	private const string IO2In = "io2in";

	private const string IO2Out = "io2out";

	private const string Log = "log";

	private const string System = "system";

	private const string RobTarget = "robtarget";

	private const string ServoEnable = "ServoEnable";

	private const string RapidExecution = "RapidExecution";

	private const string RapidTasks = "RapidTasks";

	/// <summary>
	/// 使用指定的ip地址来初始化对象
	/// Initializes the object using the specified IP address
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	public ABBWebApiClient(string ipAddress)
		: base(ipAddress)
	{
	}

	/// <summary>
	/// 使用指定的ip地址和端口号来初始化对象
	/// Initializes the object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public ABBWebApiClient(string ipAddress, int port)
		: base(ipAddress, port)
	{
	}

	/// <summary>
	/// 使用指定的ip地址，端口号，用户名，密码来初始化对象
	/// Initialize the object with the specified IP address, port number, username, and password
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	/// <param name="name">用户名</param>
	/// <param name="password">密码</param>
	public ABBWebApiClient(string ipAddress, int port, string name, string password)
		: base(ipAddress, port, name, password)
	{
	}

	[HslMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the other side of the data information, usually designed for the GET method information.If you start with url=, you are using native address access")]
	public override OperateResult<byte[]> Read(string address)
	{
		return base.Read(address);
	}

	[HslMqttApi(ApiTopic = "ReadRobotString", Description = "The string data information that reads the other party information, usually designed for the GET method information.If you start with url=, you are using native address access")]
	public override OperateResult<string> ReadString(string address)
	{
		return base.ReadString(address);
	}

	[HslMqttApi(ApiTopic = "WriteRobotByte", Description = "Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access")]
	public override OperateResult Write(string address, byte[] value)
	{
		return base.Write(address, value);
	}

	[HslMqttApi(ApiTopic = "WriteRobotString", Description = "Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access")]
	public override OperateResult Write(string address, string value)
	{
		return base.Write(address, value);
	}

	private int GetLogLength(string address)
	{
		if (address.Length > 3 && int.TryParse(address.Substring(3), out var result))
		{
			return result;
		}
		return 10;
	}

	protected override OperateResult<string> ReadByAddress(string address)
	{
		if (address.EqualsNoCase("ErrorState"))
		{
			return GetErrorState();
		}
		if (address.EqualsNoCase("jointtarget"))
		{
			return GetJointTarget();
		}
		if (address.EqualsNoCase("PhysicalJoints"))
		{
			return GetJointTarget();
		}
		if (address.EqualsNoCase("SpeedRatio"))
		{
			return GetSpeedRatio();
		}
		if (address.EqualsNoCase("OperationMode"))
		{
			return GetOperationMode();
		}
		if (address.EqualsNoCase("CtrlState"))
		{
			return GetCtrlState();
		}
		if (address.EqualsNoCase("ioin"))
		{
			return GetIOIn();
		}
		if (address.EqualsNoCase("ioout"))
		{
			return GetIOOut();
		}
		if (address.EqualsNoCase("io2in"))
		{
			return GetIO2In();
		}
		if (address.EqualsNoCase("io2out"))
		{
			return GetIO2Out();
		}
		if (address.StartsWithNoCase("log"))
		{
			return GetLog(GetLogLength(address));
		}
		if (address.EqualsNoCase("system"))
		{
			return GetSystem();
		}
		if (address.EqualsNoCase("robtarget"))
		{
			return GetRobotTarget();
		}
		if (address.EqualsNoCase("ServoEnable"))
		{
			return GetServoEnable();
		}
		if (address.EqualsNoCase("RapidExecution"))
		{
			return GetRapidExecution();
		}
		if (address.EqualsNoCase("RapidTasks"))
		{
			return GetRapidTasks();
		}
		return base.ReadByAddress(address);
	}

	protected override async Task<OperateResult<string>> ReadByAddressAsync(string address)
	{
		if (address.EqualsNoCase("ErrorState"))
		{
			return await GetErrorStateAsync();
		}
		if (address.EqualsNoCase("jointtarget"))
		{
			return await GetJointTargetAsync();
		}
		if (address.EqualsNoCase("PhysicalJoints"))
		{
			return await GetJointTargetAsync();
		}
		if (address.EqualsNoCase("SpeedRatio"))
		{
			return await GetSpeedRatioAsync();
		}
		if (address.EqualsNoCase("OperationMode"))
		{
			return await GetOperationModeAsync();
		}
		if (address.EqualsNoCase("CtrlState"))
		{
			return await GetCtrlStateAsync();
		}
		if (address.EqualsNoCase("ioin"))
		{
			return await GetIOInAsync();
		}
		if (address.EqualsNoCase("ioout"))
		{
			return await GetIOOutAsync();
		}
		if (address.EqualsNoCase("io2in"))
		{
			return await GetIO2InAsync();
		}
		if (address.EqualsNoCase("io2out"))
		{
			return await GetIO2OutAsync();
		}
		if (address.StartsWithNoCase("log"))
		{
			return await GetLogAsync(GetLogLength(address));
		}
		if (address.EqualsNoCase("system"))
		{
			return await GetSystemAsync();
		}
		if (address.EqualsNoCase("robtarget"))
		{
			return await GetRobotTargetAsync();
		}
		if (address.EqualsNoCase("ServoEnable"))
		{
			return await GetServoEnableAsync();
		}
		if (address.EqualsNoCase("RapidExecution"))
		{
			return await GetRapidExecutionAsync();
		}
		if (address.EqualsNoCase("RapidTasks"))
		{
			return await GetRapidTasksAsync();
		}
		return await base.ReadByAddressAsync(address);
	}

	/// <summary>
	/// 获取当前支持的读取的地址列表
	/// Gets a list of addresses for currently supported reads
	/// </summary>
	/// <returns>数组信息</returns>
	public static List<string> GetSelectStrings()
	{
		return new List<string>
		{
			"ErrorState", "jointtarget", "PhysicalJoints", "SpeedRatio", "OperationMode", "CtrlState", "ioin", "ioout", "io2in", "io2out",
			"log", "system", "robtarget", "ServoEnable", "RapidExecution", "RapidTasks"
		};
	}

	private string GetRegexSpanString(string className)
	{
		return "<span class=\"" + className + "\">[^<]+";
	}

	private OperateResult<string> ParseSpanByClass(string content, string className)
	{
		Match match = Regex.Match(content, GetRegexSpanString(className));
		if (!match.Success)
		{
			return new OperateResult<string>("Parse None class [" + className + "] Span\r\n" + content);
		}
		return OperateResult.CreateSuccessResult(match.Value.Substring(match.Value.IndexOf('>') + 1));
	}

	private OperateResult<double[]> ParseDoubleListSpanByClass(string content, string className)
	{
		MatchCollection matchCollection = Regex.Matches(content, GetRegexSpanString(className));
		double[] array = new double[matchCollection.Count];
		for (int i = 0; i < matchCollection.Count; i++)
		{
			array[i] = Convert.ToDouble(matchCollection[i].Value.Substring(matchCollection[i].Value.IndexOf('>') + 1));
		}
		return OperateResult.CreateSuccessResult(array);
	}

	private OperateResult<string> ParseListSpanByClass<T>(string content, string className, Func<string, T> trans)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		MatchCollection matchCollection = Regex.Matches(content, GetRegexSpanString(className));
		JArray val = new JArray();
		for (int i = 0; i < matchCollection.Count; i++)
		{
			((JContainer)val).Add((object)trans(matchCollection[i].Value.Substring(matchCollection[i].Value.IndexOf('>') + 1)));
		}
		return OperateResult.CreateSuccessResult(((object)val).ToString());
	}

	private JObject ParseListByClass(string content)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		XElement xElement = XElement.Parse(content);
		JObject val = new JObject();
		foreach (XElement item in xElement.Elements("span"))
		{
			XAttribute xAttribute = item.Attribute("class");
			if (xAttribute != null)
			{
				val.Add(xAttribute.Value, (JToken)item.Value);
			}
		}
		return val;
	}

	private string GetRegexListString(string className)
	{
		return "<li class=\"" + className + "\"[\\S\\s]+?</li>";
	}

	private OperateResult<string> ParseJObjectByClass(string content, string className)
	{
		Match match = Regex.Match(content, GetRegexListString(className));
		if (!match.Success)
		{
			return new OperateResult<string>("Parse None class [" + className + "] List\r\n" + content);
		}
		return OperateResult.CreateSuccessResult(((object)ParseListByClass(match.Value)).ToString());
	}

	private OperateResult<string> ParseJArrayByClass(string content, string className, int maxCount = int.MaxValue)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		MatchCollection matchCollection = Regex.Matches(content, GetRegexListString(className));
		JArray val = new JArray();
		for (int i = 0; i < matchCollection.Count && i < maxCount; i++)
		{
			val.Add((JToken)(object)ParseListByClass(matchCollection[i].Value));
		}
		return OperateResult.CreateSuccessResult(((object)val).ToString());
	}

	private OperateResult<string[]> ParseStringArray(string content, string className)
	{
		MatchCollection matchCollection = Regex.Matches(content, GetRegexListString(className));
		List<string> list = new List<string>();
		for (int i = 0; i < matchCollection.Count; i++)
		{
			XElement xElement = XElement.Parse(matchCollection[i].Value);
			XAttribute xAttribute = xElement.Attribute("title");
			if (xAttribute != null)
			{
				list.Add(xAttribute.Value);
			}
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	[HslMqttApi(Description = "Get the current control state. The Content attribute is the control information of the robot")]
	/// <summary>
	/// 获取当前的控制状态，Content属性就是机器人的控制信息
	/// Get the current control state. The Content attribute is the control information of the robot
	/// </summary>
	/// <returns>带有状态信息的结果类对象</returns>
	public OperateResult<string> GetCtrlState()
	{
		return ReadString("url=/rw/panel/ctrlstate").Then((string m) => ParseSpanByClass(m, "ctrlstate"));
	}

	[HslMqttApi(Description = "Gets the current error state. The Content attribute is the state information of the robot")]
	/// <summary>
	/// 获取当前的错误状态，Content属性就是机器人的状态信息
	/// Gets the current error state. The Content attribute is the state information of the robot
	/// </summary>
	/// <returns>带有状态信息的结果类对象</returns>
	public OperateResult<string> GetErrorState()
	{
		return ReadString("url=/rw/motionsystem/errorstate").Then((string m) => ParseSpanByClass(m, "err-state"));
	}

	[HslMqttApi(Description = "Get the physical node information of the current robot and return the joint information in json format")]
	/// <summary>
	/// 获取当前机器人的物理关节点信息，返回json格式的关节信息
	/// Get the physical node information of the current robot and return the joint information in json format
	/// </summary>
	/// <param name="mechunit">操作单元，默认为 ROB_1</param>
	/// <returns>带有关节信息的结果类对象</returns>
	public OperateResult<string> GetJointTarget(string mechunit = "ROB_1")
	{
		return ReadString("url=/rw/motionsystem/mechunits/" + mechunit + "/jointtarget").Then((string m) => ParseListSpanByClass(m, "((rax_[0-9]+)|(eax_[a-z]))", (string n) => Convert.ToDouble(n)));
	}

	[HslMqttApi(Description = "Get the speed matching information of the current robot")]
	/// <summary>
	/// 获取当前机器人的速度配比信息
	/// Get the speed matching information of the current robot
	/// </summary>
	/// <returns>带有速度信息的结果类对象</returns>
	public OperateResult<string> GetSpeedRatio()
	{
		return ReadString("url=/rw/panel/speedratio").Then((string m) => ParseSpanByClass(m, "speedratio"));
	}

	[HslMqttApi(Description = "Gets the current working mode of the robot")]
	/// <summary>
	/// 获取当前机器人的工作模式
	/// Gets the current working mode of the robot
	/// </summary>
	/// <returns>带有工作模式信息的结果类对象</returns>
	public OperateResult<string> GetOperationMode()
	{
		return ReadString("url=/rw/panel/opmode").Then((string m) => ParseSpanByClass(m, "opmode"));
	}

	[HslMqttApi(Description = "Gets the input IO of the current robot's native")]
	/// <summary>
	/// 获取当前机器人的本机的输入IO
	/// Gets the input IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public OperateResult<string> GetIOIn()
	{
		return ReadString("url=/rw/iosystem/devices/D652_10").Then((string m) => ParseSpanByClass(m, "indata"));
	}

	[HslMqttApi(Description = "Gets the output IO of the current robot's native")]
	/// <summary>
	/// 获取当前机器人的本机的输出IO
	/// Gets the output IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public OperateResult<string> GetIOOut()
	{
		return ReadString("url=/rw/iosystem/devices/D652_10").Then((string m) => ParseSpanByClass(m, "outdata"));
	}

	[HslMqttApi(Description = "Gets the input IO2 of the current robot's native")]
	/// <summary>
	/// 获取当前机器人的本机的输入IO
	/// Gets the input IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public OperateResult<string> GetIO2In()
	{
		return ReadString("url=/rw/iosystem/devices/BK5250").Then((string m) => ParseSpanByClass(m, "indata"));
	}

	[HslMqttApi(Description = "Gets the output IO2 of the current robot's native")]
	/// <summary>
	/// 获取当前机器人的本机的输出IO
	/// Gets the output IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public OperateResult<string> GetIO2Out()
	{
		return ReadString("url=/rw/iosystem/devices/BK5250").Then((string m) => ParseSpanByClass(m, "outdata"));
	}

	[HslMqttApi(Description = "Gets the log record for the current robot, which is 10 by default")]
	/// <summary>
	/// 获取当前机器人的日志记录，默认记录为10条
	/// Gets the log record for the current robot, which is 10 by default
	/// </summary>
	/// <param name="logCount">读取的最大的日志总数</param>
	/// <returns>带有IO信息的结果类对象</returns>
	public OperateResult<string> GetLog(int logCount = 10)
	{
		return ReadString("url=/rw/elog/0?lang=zh&amp;resource=title").Then((string m) => ParseJArrayByClass(m, "elog-message-li", logCount));
	}

	[HslMqttApi(Description = "Get the current robot's system information, version number, unique ID and other information")]
	/// <summary>
	/// 获取当前机器人的系统信息，版本号，唯一ID等信息
	/// Get the current robot's system information, version number, unique ID and other information
	/// </summary>
	/// <returns>系统的基本信息</returns>
	public OperateResult<string> GetSystem()
	{
		return ReadString("url=/rw/system").Then((string m) => ParseJObjectByClass(m, "sys-system-li"));
	}

	/// <summary>
	/// 获取当前机器人的操作单元列表
	/// Retrieves the list of mechanical unit names from the motion system.
	/// </summary>
	/// <returns>An OperateResult containing an array of mechanical unit names.</returns>
	public OperateResult<string[]> GetMechunits()
	{
		return ReadString("url=/rw/motionsystem/mechunits").Then((string m) => ParseStringArray(m, "ms-mechunit-li"));
	}

	[HslMqttApi(Description = "Get the current robot's target information")]
	/// <summary>
	/// 获取机器人的目标坐标信息
	/// Get the current robot's target information
	/// </summary>
	/// <param name="mechunit">操作单元，默认为 ROB_1</param>
	/// <returns>系统的基本信息</returns>
	public OperateResult<string> GetRobotTarget(string mechunit = "ROB_1")
	{
		return ReadString("url=/rw/motionsystem/mechunits/" + mechunit + "/robtarget").Then((string m) => ParseJObjectByClass(m, "ms-robtargets"));
	}

	[HslMqttApi(Description = "Get the current robot servo enable state")]
	/// <summary>
	/// 获取当前机器人的伺服使能状态
	/// Get the current robot servo enable state
	/// </summary>
	/// <returns>机器人的伺服使能状态</returns>
	public OperateResult<string> GetServoEnable()
	{
		return ReadString("url=/rw/iosystem/signals/Local/DRV_1/DRV1K1").Then((string m) => ParseJObjectByClass(m, "ios-signal"));
	}

	[HslMqttApi(Description = "Get the current program running status of the current robot")]
	/// <summary>
	/// 获取当前机器人的当前程序运行状态
	/// Get the current program running status of the current robot
	/// </summary>
	/// <returns>机器人的当前的程序运行状态</returns>
	public OperateResult<string> GetRapidExecution()
	{
		return ReadString("url=/rw/rapid/execution").Then((string m) => ParseJObjectByClass(m, "rap-execution"));
	}

	[HslMqttApi(Description = "Get the task list of the current robot")]
	/// <summary>
	/// 获取当前机器人的任务列表
	/// Get the task list of the current robot
	/// </summary>
	/// <returns>任务信息的列表</returns>
	public OperateResult<string> GetRapidTasks()
	{
		return ReadString("url=/rw/rapid/tasks").Then((string m) => ParseJArrayByClass(m, "rap-task-li"));
	}

	/// <summary>
	/// 根据给定的名称，获取当前用户的数据值信息。
	/// According to the given name, gets the data value information of the current user
	/// </summary>
	/// <param name="name">数据名称信息，例如 nCurProgIndex</param>
	/// <returns>数值信息</returns>
	public OperateResult<double[]> GetUserValue(string name)
	{
		return ReadString(name.StartsWith("url=", StringComparison.OrdinalIgnoreCase) ? name : ("url=/rw/rapid/symbol/data/RAPID/T_ROB1/user/" + name)).Then((string m) => ParseDoubleListSpanByClass(m, "value"));
	}

	[HslMqttApi(Description = "Get the current robot's target information")]
	/// <summary>
	/// 获取机器人的IO信号资源
	/// Get an IO signal resource.
	/// </summary>
	/// <returns>系统的基本信息</returns>
	public OperateResult<string> GetAnIOSignal(string network = "Local", string unit = "DRV_1", string signal = "DRV1K1")
	{
		return ReadString("url=/rw/iosystem/signals/" + network + "/" + unit + "/" + signal).Then((string m) => ParseJObjectByClass(m, "ios-signal"));
	}

	/// <summary>
	/// 获取当前的控制状态，Content属性就是机器人的控制信息
	/// Get the current control state. The Content attribute is the control information of the robot
	/// </summary>
	/// <returns>带有状态信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetCtrlStateAsync()
	{
		return (await ReadStringAsync("url=/rw/panel/ctrlstate").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "ctrlstate"));
	}

	/// <summary>
	/// 获取当前的错误状态，Content属性就是机器人的状态信息
	/// Gets the current error state. The Content attribute is the state information of the robot
	/// </summary>
	/// <returns>带有状态信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetErrorStateAsync()
	{
		return (await ReadStringAsync("url=/rw/motionsystem/errorstate").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "err-state"));
	}

	/// <summary>
	/// 获取当前机器人的物理关节点信息，返回json格式的关节信息
	/// Get the physical node information of the current robot and return the joint information in json format
	/// </summary>
	/// <param name="mechunit">操作单元，默认为 ROB_1</param>
	/// <returns>带有关节信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetJointTargetAsync(string mechunit = "ROB_1")
	{
		return (await ReadStringAsync("url=/rw/motionsystem/mechunits/" + mechunit + "/jointtarget").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseListSpanByClass(m, "((rax_[0-9]+)|(eax_[a-z]))", (string n) => Convert.ToDouble(n)));
	}

	/// <summary>
	/// 获取当前机器人的速度配比信息
	/// Get the speed matching information of the current robot
	/// </summary>
	/// <returns>带有速度信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetSpeedRatioAsync()
	{
		return (await ReadStringAsync("url=/rw/panel/speedratio").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "speedratio"));
	}

	/// <summary>
	/// 获取当前机器人的工作模式
	/// Gets the current working mode of the robot
	/// </summary>
	/// <returns>带有工作模式信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetOperationModeAsync()
	{
		return (await ReadStringAsync("url=/rw/panel/opmode").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "opmode"));
	}

	/// <summary>
	/// 获取当前机器人的本机的输入IO
	/// Gets the input IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetIOInAsync()
	{
		return (await ReadStringAsync("url=/rw/iosystem/devices/D652_10").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "indata"));
	}

	/// <summary>
	/// 获取当前机器人的本机的输出IO
	/// Gets the output IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetIOOutAsync()
	{
		return (await ReadStringAsync("url=/rw/iosystem/devices/D652_10").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "outdata"));
	}

	/// <summary>
	/// 获取当前机器人的本机的输入IO
	/// Gets the input IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetIO2InAsync()
	{
		return (await ReadStringAsync("url=/rw/iosystem/devices/BK5250").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "indata"));
	}

	/// <summary>
	/// 获取当前机器人的本机的输出IO
	/// Gets the output IO of the current robot's native
	/// </summary>
	/// <returns>带有IO信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetIO2OutAsync()
	{
		return (await ReadStringAsync("url=/rw/iosystem/devices/BK5250").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseSpanByClass(m, "outdata"));
	}

	/// <summary>
	/// 获取当前机器人的日志记录，默认记录为10条
	/// Gets the log record for the current robot, which is 10 by default
	/// </summary>
	/// <param name="logCount">读取的最大的日志总数</param>
	/// <returns>带有IO信息的结果类对象</returns>
	public async Task<OperateResult<string>> GetLogAsync(int logCount = 10)
	{
		return (await ReadStringAsync("url=/rw/elog/0?lang=zh&amp;resource=title").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseJArrayByClass(m, "elog-message-li", logCount));
	}

	/// <summary>
	/// 获取当前机器人的系统信息，版本号，唯一ID等信息
	/// Get the current robot's system information, version number, unique ID and other information
	/// </summary>
	/// <returns>系统的基本信息</returns>
	public async Task<OperateResult<string>> GetSystemAsync()
	{
		return (await ReadStringAsync("url=/rw/system").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseJObjectByClass(m, "sys-system-li"));
	}

	/// <summary>
	/// 获取机器人的目标坐标信息
	/// Get the current robot's target information
	/// </summary>
	/// <param name="mechunit">操作单元，默认为 ROB_1</param>
	/// <returns>系统的基本信息</returns>
	public async Task<OperateResult<string>> GetRobotTargetAsync(string mechunit = "ROB_1")
	{
		return (await ReadStringAsync("url=/rw/motionsystem/mechunits/" + mechunit + "/robtarget").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseJObjectByClass(m, "ms-robtargets"));
	}

	/// <summary>
	/// 获取当前机器人的操作单元列表
	/// Retrieves the list of mechanical unit names from the motion system.
	/// </summary>
	/// <returns>An OperateResult containing an array of mechanical unit names.</returns>
	public async Task<OperateResult<string[]>> GetMechunitsAsync()
	{
		return (await ReadStringAsync("url=/rw/motionsystem/mechunits").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseStringArray(m, "ms-mechunit-li"));
	}

	/// <summary>
	/// 获取当前机器人的伺服使能状态
	/// Get the current robot servo enable state
	/// </summary>
	/// <returns>机器人的伺服使能状态</returns>
	public async Task<OperateResult<string>> GetServoEnableAsync()
	{
		return (await ReadStringAsync("url=/rw/iosystem/signals/Local/DRV_1/DRV1K1").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseJObjectByClass(m, "ios-signal"));
	}

	/// <summary>
	/// 获取当前机器人的当前程序运行状态
	/// Get the current program running status of the current robot
	/// </summary>
	/// <returns>机器人的当前的程序运行状态</returns>
	public async Task<OperateResult<string>> GetRapidExecutionAsync()
	{
		return (await ReadStringAsync("url=/rw/rapid/execution").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseJObjectByClass(m, "rap-execution"));
	}

	/// <summary>
	/// 获取当前机器人的任务列表
	/// Get the task list of the current robot
	/// </summary>
	/// <returns>任务信息的列表</returns>
	public async Task<OperateResult<string>> GetRapidTasksAsync()
	{
		return (await ReadStringAsync("url=/rw/rapid/tasks").ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseJArrayByClass(m, "rap-task-li"));
	}

	/// <summary>
	/// 根据给定的名称，获取当前用户的数据值信息。
	/// According to the given name, gets the data value information of the current user
	/// </summary>
	/// <param name="name">数据名称信息，例如 nCurProgIndex</param>
	/// <returns>数值信息</returns>
	public async Task<OperateResult<double[]>> GetUserValueAsync(string name)
	{
		return (await ReadStringAsync(name.StartsWith("url=", StringComparison.OrdinalIgnoreCase) ? name : ("url=/rw/rapid/symbol/data/RAPID/T_ROB1/user/" + name)).ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseDoubleListSpanByClass(m, "value"));
	}

	/// <summary>
	/// 获取机器人的IO信号资源
	/// Get an IO signal resource.
	/// </summary>
	/// <returns>系统的基本信息</returns>
	public async Task<OperateResult<string>> GetAnIOSignalAsync(string network = "Local", string unit = "DRV_1", string signal = "DRV1K1")
	{
		return (await ReadStringAsync("url=/rw/iosystem/signals/" + network + "/" + unit + "/" + signal).ConfigureAwait(continueOnCapturedContext: false)).Then((string m) => ParseJObjectByClass(m, "ios-signal"));
	}

	public override string ToString()
	{
		return $"ABBWebApiClient[{base.IpAddress}:{base.Port}]";
	}
}
