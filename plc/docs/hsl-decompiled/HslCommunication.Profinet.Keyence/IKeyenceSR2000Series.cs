// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Keyence;

/// <summary>
/// 基恩士SR2000系列扫码设备的通用接口
/// </summary>
internal interface IKeyenceSR2000Series
{
	[HslMqttApi]
	/// <summary>
	/// 读取条码信息，返回最终读取的条码数据
	/// Read the barcode information and return the finally read barcode data
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>条码信息</returns>
	OperateResult<string> ReadBarcode();

	[HslMqttApi]
	/// <summary>
	/// 复位命令，响应后，进行复位动作。
	/// Reset command, after responding, reset action.
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult Reset();

	[HslMqttApi]
	/// <summary>
	/// 打开指示灯
	/// Turn on the indicator
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult OpenIndicator();

	[HslMqttApi]
	/// <summary>
	/// 关闭指示灯
	/// Turn off the indicator
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult CloseIndicator();

	[HslMqttApi]
	/// <summary>
	/// 读取基恩士的版本
	/// Read Keyence's version
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>版本信息</returns>
	OperateResult<string> ReadVersion();

	[HslMqttApi]
	/// <summary>
	/// 读取基恩士的命令状态，none:不处理；wait：等待设置反映；update：正在更新
	/// Read the command status of Keyence, none: do not process; wait: wait for the setting to reflect; update: update
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult<string> ReadCommandState();

	[HslMqttApi]
	/// <summary>
	/// 读取基恩士的错误状态
	/// Read the error status of Keyence
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult<string> ReadErrorState();

	[HslMqttApi]
	/// <summary>
	/// 读取IN端子的状况，需要传入哪个IN端子，返回是否通断
	/// Read the status of the IN terminal, which IN terminal needs to be passed in, and return whether it is on or off
	/// </summary>
	/// <param name="number">端子的信息</param>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult<bool> CheckInput(int number);

	[HslMqttApi]
	/// <summary>
	/// 设置OUT端子的使能，需要传入哪个OUT端子，返回是否设置成功！
	/// Set the enable of the OUT terminal, which OUT terminal needs to be passed in, and return whether the setting is successful!
	/// </summary>
	/// <param name="number">端子的索引，1，2，3</param>
	/// <param name="value">是否通断</param>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult SetOutput(int number, bool value);

	[HslMqttApi]
	/// <summary>
	/// 读取扫码器的扫码记录，返回数组数据，分别是成功次数，失败次数，ERROR次数，稳定次数，时机输入次数
	/// Read the scan code record of the scanner and return the array data, which are the number of successes, failures, ERRORs, stable times, and timing input times.
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult<int[]> ReadRecord();

	[HslMqttApi]
	/// <summary>
	/// 锁定扫码设备
	/// Lock scanning device
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult Lock();

	[HslMqttApi]
	/// <summary>
	/// 解除锁定的扫码设备
	/// Unlock Scanning Device
	/// </summary>
	/// <param name="readCore">核心交互的方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult UnLock();

	[HslMqttApi]
	/// <summary>
	/// 读取自定义的命令，例如LON，如果是包含其他参数的，比如打开OUT端子，OUTON,1
	/// Read custom commands, such as LON, if it contains other parameters, such as open OUT terminal, OUTON,1
	/// </summary>
	/// <param name="command">自定义的命令</param>
	/// <param name="readCore">核心的数据交互方法</param>
	/// <returns>返回是否成功的结果对象</returns>
	OperateResult<string> ReadCustomer(string command);
}
