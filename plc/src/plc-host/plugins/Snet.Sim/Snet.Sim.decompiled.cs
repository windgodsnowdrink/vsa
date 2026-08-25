using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Snet.Core.@abstract;
using Snet.Core.extend;
using Snet.Core.handler;
using Snet.Core.subscription;
using Snet.Core.virtualAddress;
using Snet.Model.data;
using Snet.Model.@enum;
using Snet.Model.@interface;
using Snet.Utility;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("采集协议：Sim（采集模拟 - 虚拟静态地址、虚拟动态随机变化地址、虚拟动态随机范围变化地址、虚拟动态顺序变化地址、虚拟动态顺序范围变化地址）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Daq")]
[assembly: AssemblyTitle("Snet.Sim")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Sim;

/// <summary>
/// 模拟数据
/// </summary>
public class SimData
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class Basics : SCData
	{
		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Category("基础数据")]
		[Description("唯一标识符")]
		public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());
	}
}
/// <summary>
/// 采集模拟操作
/// </summary>
public class SimOperate : DaqAbstract<SimOperate, SimData.Basics>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// 虚拟地址
	/// </summary>
	private VirtualAddressManage? VAM;

	/// <summary>
	/// 实现订阅功能
	/// </summary>
	private SubscribeOperate? subscribeOperate;

	/// <inheritdoc />
	protected override string CN => "采集模拟";

	/// <inheritdoc />
	protected override string CD => "地址类型中内置五种模式，虚拟静态地址、虚拟动态随机变化地址、虚拟动态随机范围变化地址、虚拟动态顺序变化地址、虚拟动态顺序范围变化地址";

	/// <summary>
	/// 无惨构造函数
	/// </summary>
	public SimOperate()
	{
	}

	/// <summary>
	/// 有参构造函数
	/// </summary>
	/// <param name="basics">基础数据</param>
	public SimOperate(SimData.Basics basics)
		: base(basics)
	{
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await ((DaqAbstract<SimOperate, SimData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, text, (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "OnAsync", 53);
			}
			if (VAM == null)
			{
				VAM = new VirtualAddressManage();
			}
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "OnAsync", 61);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "OnAsync", 65);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await ((DaqAbstract<SimOperate, SimData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, text, (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "OffAsync", 78);
			}
			if (subscribeOperate != null)
			{
				OperateResult val = await subscribeOperate.OffAsync(false, token);
				if (!((BaseModel)val).Status)
				{
					return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ((BaseModel)val).Message, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "OffAsync", 86);
				}
				subscribeOperate = null;
			}
			VirtualAddressManage? vAM = VAM;
			if (vAM != null)
			{
				vAM.DisposeAsync();
			}
			VAM = null;
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "OffAsync", 92);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "OffAsync", 96);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "ReadAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SimOperate, SimData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, text, (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "ReadAsync", 107);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "ReadAsync", 112);
			}
			ConcurrentDictionary<string, AddressValue> param = new ConcurrentDictionary<string, AddressValue>();
			foreach (AddressDetails item in address.AddressArray)
			{
				if (!item.IsEnable)
				{
					continue;
				}
				bool flag = false;
				VAM.InitVirtualAddress(item, ref flag);
				if (flag)
				{
					object obj = VAM.Read(item);
					AddressDetails val = item;
					object obj2 = obj;
					string text2 = ((!string.IsNullOrEmpty(obj?.ToString())) ? (await LanguageHandler.GetLanguageValueAsync("成功", (LanguageModel)null, token)) : (await LanguageHandler.GetLanguageValueAsync("失败", (LanguageModel)null, token)));
					AddressValue addressValue = AddressHandler.ExecuteDispose(val, obj2, text2);
					param.AddOrUpdate(item.AddressName, addressValue, (string k, AddressValue v) => addressValue);
				}
			}
			if (param.Count > 0)
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(true, (string)null, (object)param, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "ReadAsync", 141);
			}
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("读取失败", (LanguageModel)null, token), (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "ReadAsync", 145);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "ReadAsync", 150);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "WriteAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SimOperate, SimData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "WriteAsync", 161);
			}
			List<string> FailMessage = new List<string>();
			foreach (KeyValuePair<string, (object, EncodingType?)> value in values)
			{
				if (VAM.IsVirtualAddress(value.Key) && !VAM.Write(value.Key, value.Value.Item1))
				{
					List<string> list = FailMessage;
					list.Add(await LanguageHandler.GetLanguageValueAsync(value.Key + "，写入失败", (LanguageModel)null, token));
				}
			}
			if (FailMessage.Count > 0)
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ExtendMethod.ToJson<List<string>>(FailMessage, false), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "WriteAsync", 180);
			}
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("写入成功", (LanguageModel)null, token), (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "WriteAsync", 182);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "WriteAsync", 186);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "SubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SimOperate, SimData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "SubscribeAsync", 197);
			}
			if (!address.CheckAddress())
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("存在无效点位数据，操作失败", (LanguageModel)null, token), (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "SubscribeAsync", 202);
			}
			if (subscribeOperate == null)
			{
				subscribeOperate = await CoreUnify<SubscribeOperate, Basics>.InstanceAsync(new Basics
				{
					Address = address,
					ChangeOut = ((SCData)((CoreUnify<SimOperate, SimData.Basics>)(object)this).basics).ChangeOut,
					FunctionAsync = ((DaqAbstract<SimOperate, SimData.Basics>)(object)this).ReadAsync,
					AllOut = ((SCData)((CoreUnify<SimOperate, SimData.Basics>)(object)this).basics).AllOut,
					HandleInterval = ((SCData)((CoreUnify<SimOperate, SimData.Basics>)(object)this).basics).HandleInterval,
					SN = ((CoreUnify<SimOperate, SimData.Basics>)(object)this).basics.SN,
					TaskNumber = ((SCData)((CoreUnify<SimOperate, SimData.Basics>)(object)this).basics).TaskNumber
				}, default(CancellationToken));
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnDataEvent += ((CoreUnify<SimOperate, SimData.Basics>)(object)this).OnDataEventHandler;
				((CoreUnify<SubscribeOperate, Basics>)(object)subscribeOperate).OnInfoEvent += ((CoreUnify<SimOperate, SimData.Basics>)(object)this).OnInfoEventHandler;
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.OnAsync(token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "SubscribeAsync", 219);
			}
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "SubscribeAsync", 223);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "SubscribeAsync", 228);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "UnSubscribeAsync");
		try
		{
			string text = default(string);
			if (!((ResultModel)(await ((DaqAbstract<SimOperate, SimData.Basics>)(object)this).GetStatusAsync(token))).GetDetails(ref text))
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, text, (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "UnSubscribeAsync", 239);
			}
			if (subscribeOperate != null)
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "UnSubscribeAsync", 244);
			}
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("当前尚未订阅", (LanguageModel)null, token), (object)token, (Exception)null, true, true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "UnSubscribeAsync", 248);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "UnSubscribeAsync", 253);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "GetStatusAsync");
		try
		{
			if (VAM == null)
			{
				return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, await LanguageHandler.GetLanguageValueAsync("未连接", (LanguageModel)null, token), (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "GetStatusAsync", 264);
			}
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(true, await LanguageHandler.GetLanguageValueAsync("已连接", (LanguageModel)null, token), (object)null, (Exception)null, false, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "GetStatusAsync", 266);
		}
		catch (Exception ex)
		{
			return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", "GetStatusAsync", 270);
		}
	}

	/// <inheritdoc />
	public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		return await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).EndOperateAsync(false, "无底层公共对象", (object)null, (Exception)null, true, true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Sim\\SimOperate.cs", await ((CoreUnify<SimOperate, SimData.Basics>)(object)this).BegOperateAsync(token, "GetBaseObjectAsync"), 277);
	}
}
