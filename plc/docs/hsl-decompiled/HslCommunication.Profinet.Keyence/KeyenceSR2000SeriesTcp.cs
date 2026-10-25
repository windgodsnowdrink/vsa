// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Keyence;

/// <summary>
/// 基恩士的SR2000的扫码设备，可以进行简单的交互
/// </summary>
/// <remarks>
/// 当使用 "LON","LOFF","PRON","PROFF" 命令时，在发送时和发生错误时，将不会收到扫码设备的回发命令，而是输出读取结果。
/// 如果也希望获取上述命令的响应时，请在以下位置进行设置。[设置列表]-[其他]-"指定基本命令的响应字符串"
/// </remarks>
public class KeyenceSR2000SeriesTcp : NetworkDoubleBase, IKeyenceSR2000Series
{
	/// <summary>
	/// 实例化基恩士的SR2000的扫码设备通讯对象
	/// Instantiate keyence's SR2000 scan code device communication object
	/// </summary>
	public KeyenceSR2000SeriesTcp()
	{
		base.ReceiveTimeOut = 10000;
		base.SleepTime = 20;
	}

	/// <summary>
	/// 指定ip地址及端口号来实例化一个基恩士的SR2000的扫码设备通讯对象
	/// Specify the ip address and port number to instantiate a keyence SR2000 scan code device communication object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public KeyenceSR2000SeriesTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	[HslMqttApi]
	/// <summary>
	/// 读取条码信息，返回最终读取的条码数据
	/// Read the barcode information and return the finally read barcode data
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>条码信息</returns>
	public OperateResult<string> ReadBarcode()
	{
		return KeyenceSR2000Helper.ReadBarcode(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 复位命令，响应后，进行复位动作。
	/// Reset command, after responding, reset action.
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult Reset()
	{
		return KeyenceSR2000Helper.Reset(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 打开指示灯
	/// Turn on the indicator
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult OpenIndicator()
	{
		return KeyenceSR2000Helper.OpenIndicator(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 关闭指示灯
	/// Turn off the indicator
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult CloseIndicator()
	{
		return KeyenceSR2000Helper.CloseIndicator(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取基恩士的版本
	/// Read Keyence's version
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>版本信息</returns>
	public OperateResult<string> ReadVersion()
	{
		return KeyenceSR2000Helper.ReadVersion(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取基恩士的命令状态，none:不处理；wait：等待设置反映；update：正在更新
	/// Read the command status of Keyence, none: do not process; wait: wait for the setting to reflect; update: update
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult<string> ReadCommandState()
	{
		return KeyenceSR2000Helper.ReadCommandState(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取基恩士的错误状态
	/// Read the error status of Keyence
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult<string> ReadErrorState()
	{
		return KeyenceSR2000Helper.ReadErrorState(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取IN端子的状况，需要传入哪个IN端子，返回是否通断
	/// Read the status of the IN terminal, which IN terminal needs to be passed in, and return whether it is on or off
	/// </summary>
	/// <param name="number">端子的信息</param>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult<bool> CheckInput(int number)
	{
		return KeyenceSR2000Helper.CheckInput(number, ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 设置OUT端子的使能，需要传入哪个OUT端子，返回是否设置成功！
	/// Set the enable of the OUT terminal, which OUT terminal needs to be passed in, and return whether the setting is successful!
	/// </summary>
	/// <param name="number">端子的索引，1，2，3</param>
	/// <param name="value">是否通断</param>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult SetOutput(int number, bool value)
	{
		return KeyenceSR2000Helper.SetOutput(number, value, ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取扫码器的扫码记录，返回数组数据，分别是成功次数，失败次数，ERROR次数，稳定次数，时机输入次数
	/// Read the scan code record of the scanner and return the array data, which are the number of successes, failures, ERRORs, stable times, and timing input times.
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult<int[]> ReadRecord()
	{
		return KeyenceSR2000Helper.ReadRecord(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 锁定扫码设备
	/// Lock scanning device
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult Lock()
	{
		return KeyenceSR2000Helper.Lock(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 解除锁定的扫码设备
	/// Unlock Scanning Device
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult UnLock()
	{
		return KeyenceSR2000Helper.UnLock(ReadFromCoreServer);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取自定义的命令，例如LON，如果是包含其他参数的，比如打开OUT端子，OUTON,1
	/// Read custom commands, such as LON, if it contains other parameters, such as open OUT terminal, OUTON,1
	/// </summary>
	/// <param name="command">自定义的命令</param>
	/// <param name="readCore">核心的数据交互方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	public OperateResult<string> ReadCustomer(string command)
	{
		return KeyenceSR2000Helper.ReadCustomer(command, ReadFromCoreServer);
	}

	public override string ToString()
	{
		return $"KeyenceSR2000SeriesTcp[{IpAddress}:{Port}]";
	}
}
