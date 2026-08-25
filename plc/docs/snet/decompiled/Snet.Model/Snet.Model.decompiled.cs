using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProtoBuf;
using Serilog.Events;
using Snet.Log;
using Snet.Model.data;
using Snet.Model.@enum;
using Snet.Model.@event;
using Snet.Utility;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("核心组件：Model（特性、结构、模型、枚举、事件、接口）")]
[assembly: AssemblyFileVersion("26.235.1.0")]
[assembly: AssemblyInformationalVersion("26.235.1+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Core")]
[assembly: AssemblyTitle("Snet.Model")]
[assembly: AssemblyVersion("26.235.1.0")]
[module: RefSafetyRules(11)]
namespace Snet.Model.@interface
{
	/// <summary>
	/// 参数接口；<br />
	/// 定义获取基础数据、参数以及自动分配标识相关参数的同步和异步操作；<br />
	/// 支持 AutoAllocatingTagAttribute 特性的自动参数分配
	/// </summary>
	public interface IArgs
	{
		/// <summary>
		/// 修改基础数据<br />
		/// 使用场景:<br />
		/// 当通过单例模式创建一个对象时，基础数据未赋值，可通过此方法来更新基础数据<br />
		/// 如果基础数据已经赋值，想要修改时，可通过此方法来修改基础数据
		/// </summary>
		/// <typeparam name="T">基础数据对象类型</typeparam>
		/// <param name="basics">基础数据</param>
		/// <returns>统一结果</returns>
		OperateResult UpdateArgs<T>(T basics);

		/// <summary>
		/// 异步修改基础数据<br />
		/// 使用场景:<br />
		/// 当通过单例模式创建一个对象时，基础数据未赋值，可通过此方法来更新基础数据<br />
		/// 如果基础数据已经赋值，想要修改时，可通过此方法来修改基础数据
		/// </summary>
		/// <typeparam name="T">基础数据对象类型</typeparam>
		/// <param name="basics">基础数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> UpdateArgsAsync<T>(T basics, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 修改基础数据<br />
		/// 使用场景:<br />
		/// 当通过单例模式创建一个对象时，基础数据未赋值，可通过此方法来更新基础数据<br />
		/// 如果基础数据已经赋值，想要修改时，可通过此方法来修改基础数据
		/// </summary>
		/// <param name="json">基础数据JSON字符串</param>
		/// <returns>统一结果</returns>
		OperateResult UpdateArgs(string json);

		/// <summary>
		/// 异步修改基础数据<br />
		/// 使用场景:<br />
		/// 当通过单例模式创建一个对象时，基础数据未赋值，可通过此方法来更新基础数据<br />
		/// 如果基础数据已经赋值，想要修改时，可通过此方法来修改基础数据
		/// </summary>
		/// <param name="json">基础数据JSON字符串</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> UpdateArgsAsync(string json, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 获取基础数据<br />
		/// 此返回的是:操作类实例化传入的基础数据
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult GetBasicsArgs();

		/// <summary>
		/// 获取基础数据<br />
		/// 此返回的是:操作类实例化传入的基础数据
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> GetBasicsArgsAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 获取参数，用于第三方UI展示
		/// </summary>
		/// <param name="getBasicsParam">
		/// 获取基础数据<br />
		/// true : 基础数据的'新'对象
		/// false : 基础数据组织后的'新'对象
		/// </param>
		/// <returns>统一结果</returns>
		OperateResult GetArgs(bool getBasicsParam = false);

		/// <summary>
		/// 获取参数，用于第三方UI展示
		/// </summary>
		/// <param name="getBasicsParam">
		/// 获取基础数据<br />
		/// true : 基础数据的新对象
		/// false : 基础数据组织后的新对象 
		/// </param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> GetArgsAsync(bool getBasicsParam = false, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 获取自动分配标识的属性值所对应参数集合；<br />
		/// 基础数据中必须包含　AutoAllocatingTagAttribute 特性；<br />
		/// 如果不包含，说明此库参数基本都需要填写
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult GetAutoAllocatingArgs();

		/// <summary>
		/// 获取自动分配标识的属性值所对应参数集合；<br />
		/// 基础数据中必须包含　AutoAllocatingTagAttribute 特性；<br />
		/// 如果不包含，说明此库参数基本都需要填写
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> GetAutoAllocatingArgsAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 是否存在自动分配参数；<br />
		/// 判断当前基础数据中是否存在 AutoAllocatingTagAttribute 特性；
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult ExistsAutoAllocatingArgs();

		/// <summary>
		/// 是否存在自动分配参数；<br />
		/// 判断当前基础数据中是否存在 AutoAllocatingTagAttribute 特性；
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> ExistsAutoAllocatingArgsAsync(CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 克隆接口
	/// </summary>
	public interface IClone
	{
		/// <summary>
		/// 克隆当前对象，克隆对象不会存储到单例匹配列表中，外部实现完整的闭环
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult CloneThis();

		/// <summary>
		/// 异步克隆当前对象，克隆对象不会存储到单例匹配列表中，外部实现完整的闭环
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> CloneThisAsync(CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 底层通信接口；<br />
	/// 定义客户端通信的全量能力契约，支持打开/关闭、发送/发送等待、获取对象、获取状态、事件、创建实例、日志、参数、多语言、克隆、资源释放等操作；<br />
	/// 继承 IOn, IOff, ISend, ISendWait, IGetObject, IGetStatus, IEvent, ICreateInstance, ILog, IGetParam, ILanguage, IDisposable, IAsyncDisposable
	/// </summary>
	public interface ICommunication : IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
	}
	/// <summary>
	/// 底层通信接口（服务端）；<br />
	/// 定义服务端通信的全量能力契约，支持打开/关闭、服务端发送（单发/群发）、获取对象、获取状态、移除客户端、事件、创建实例、日志、多语言、克隆、参数、资源释放等操作；<br />
	/// 继承 IOn, IOff, ISendService, IGetObject, IGetStatus, IRemove, IEvent, ICreateInstance, ILog, ILanguage, IDisposable, IAsyncDisposable
	/// </summary>
	public interface ICommunicationService : IOn, IOff, ISendService, IObject, IStatus, IRemove, IEvent, IInstance, ILog, ILanguage, IClone, IArgs, IDisposable, IAsyncDisposable
	{
	}
	/// <summary>
	/// 消费者接口；<br />
	/// 定义消息中间件消费数据的同步和异步操作；<br />
	/// 支持按主题订阅消费和取消消费；<br />
	/// 继承 IEvent，通过事件通知消费到的数据
	/// </summary>
	public interface IConsumer : IEvent
	{
		/// <summary>
		/// 消费
		/// </summary>
		/// <param name="topic">主题</param>
		/// <returns>统一结果</returns>
		OperateResult Consume(string topic);

		/// <summary>
		/// 消费
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 移除消费
		/// </summary>
		/// <param name="topic">主题</param>
		/// <returns>统一结果</returns>
		OperateResult UnConsume(string topic);

		/// <summary>
		/// 移除消费
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 数采接口（数据采集）；<br />
	/// 定义数据采集器的全量能力契约，支持打开/关闭、读取/写入、订阅/取消订阅、获取状态、事件、参数、创建实例、日志、WebApi、获取对象、多语言、克隆、地址自动组包与解包、资源释放等操作；<br />
	/// 继承 IOn, IOff, IRead, IWrite, ISubscribe, IGetStatus, IEvent, IGetParam, ICreateInstance, ILog, IWA, IGetObject, ILanguage, IDisposable, IAsyncDisposable
	/// </summary>
	public interface IDaq : IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
	{
	}
	/// <summary>
	/// 事件接口；<br />
	/// 定义数据传递、信息传递、语言变更的同步和异步事件；<br />
	/// 通过事件驱动模式向外部通知通信状态、采集数据和语言变更
	/// </summary>
	public interface IEvent
	{
		/// <summary>
		/// 数据传递事件
		/// </summary>
		event EventHandler<EventDataResult> OnDataEvent;

		/// <summary>
		/// 数据传递事件异步
		/// </summary>
		event EventHandlerAsync<EventDataResult> OnDataEventAsync;

		/// <summary>
		/// 信息传递事件
		/// </summary>
		event EventHandler<EventInfoResult> OnInfoEvent;

		/// <summary>
		/// 信息传递事件异步
		/// </summary>
		event EventHandlerAsync<EventInfoResult> OnInfoEventAsync;

		/// <summary>
		/// 语言传递事件
		/// </summary>
		event EventHandler<EventLanguageResult> OnLanguageEvent;

		/// <summary>
		/// 语言传递事件异步
		/// </summary>
		event EventHandlerAsync<EventLanguageResult> OnLanguageEventAsync;
	}
	/// <summary>
	/// 实例接口（单例模式）；<br />
	/// 定义通过泛型参数或 JSON 字符串创建实例的同步和异步操作；<br />
	/// 特殊情况下使用，返回包含自身单例对象的统一结果
	/// </summary>
	public interface IInstance
	{
		/// <summary>
		/// 创建一个实例，创建的实例会存储到单例匹配列表中，参数与匹配表一致则会返回历史对象
		/// </summary>
		/// <typeparam name="T">泛型基础数据</typeparam>
		/// <param name="param">基础数据</param>
		/// <returns>统一返回包含自身单例对象</returns>
		OperateResult CreateInstance<T>(T param);

		/// <summary>
		/// 异步创建一个实例，创建的实例会存储到单例匹配列表中，参数与匹配表一致则会返回历史对象
		/// </summary>
		/// <typeparam name="T">泛型基础数据</typeparam>
		/// <param name="param">基础数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一返回包含自身单例对象</returns>
		Task<OperateResult> CreateInstanceAsync<T>(T param, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 创建一个实例，创建的实例会存储到单例匹配列表中，参数与匹配表一致则会返回历史对象
		/// </summary>
		/// <param name="json">基础数据反序列化为JSON的字符串</param>
		/// <returns>统一返回包含自身单例对象</returns>
		OperateResult CreateInstance(string json);

		/// <summary>
		/// 异步创建一个实例，创建的实例会存储到单例匹配列表中，参数与匹配表一致则会返回历史对象
		/// </summary>
		/// <param name="json">基础数据反序列化为JSON的字符串</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一返回包含自身单例对象</returns>
		Task<OperateResult> CreateInstanceAsync(string json, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 多语言接口；<br />
	/// 定义语言的获取、设置以及根据关键字获取当前语言环境下对应键值的同步和异步操作；<br />
	/// 支持通过 LanguageModel 指定自定义语言资源文件
	/// </summary>
	public interface ILanguage
	{
		/// <summary>
		/// 语言操作属性<br />
		/// 如果外部自定义了语言资源<br />
		/// 重写此属性，实例化指向对应的资源文件<br />
		/// 通过此属性的扩展方法根据关键字获取当前语言环境下的对应的键值信息
		/// </summary>
		LanguageModel LanguageOperate { get; set; }

		/// <summary>
		/// 根据关键字获取当前语言环境下的对应的键值信息
		/// </summary>
		/// <param name="key">关键字</param>
		/// <param name="languageModel">
		/// 语言模型<br />
		/// 存放基础数据，指向对应资源<br />
		/// 如果外部自定义了语言资源<br />
		/// 此属性"必传"<br />
		/// 否则获取对应的键值<br />
		/// 也可使用"LanguageOperate"属性来进行根据关键字获取当前语言环境下的对应的键值信息<br />
		/// 为空将认定为是内部操作
		/// </param>
		/// <returns>对应语言的值</returns>
		string? GetLanguageValue(string key, LanguageModel languageModel);

		/// <summary>
		/// 根据关键字获取当前语言环境下的对应的键值信息异步
		/// </summary>
		/// <param name="key">关键字</param>
		/// <param name="languageModel">
		/// 语言模型<br />
		/// 存放基础数据，指向对应资源<br />
		/// 如果外部自定义了语言资源<br />
		/// 此属性"必传"<br />
		/// 否则获取对应的键值<br />
		/// 也可使用"LanguageOperate"属性来进行根据关键字获取当前语言环境下的对应的键值信息<br />
		/// 为空将认定为是内部操作
		/// </param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>对应语言的值</returns>
		Task<string?> GetLanguageValueAsync(string key, LanguageModel languageModel, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 获取当前使用的语言
		/// </summary>
		/// <returns>返回语言类型</returns>
		LanguageType GetLanguage();

		/// <summary>
		/// 获取当前使用的语言异步
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>返回语言类型</returns>
		Task<LanguageType> GetLanguageAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 设置语言
		/// </summary>
		/// <param name="language">语言类型</param>
		/// <returns>成功与失败</returns>
		bool SetLanguage(LanguageType language);

		/// <summary>
		/// 设置语言异步
		/// </summary>
		/// <param name="language">语言类型</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>成功与失败</returns>
		Task<bool> SetLanguageAsync(LanguageType language, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 日志通用接口；<br />
	/// 定义日志操作参数的设置和获取；<br />
	/// 支持控制日志本地写入、控制台输出、外部通知回调（Func/Action）；<br />
	/// 支持通过 LogModel 批量设置日志参数
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// 日志操作参数设置；<br />
		/// logOut为false时,consoleOut将不生效<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logOut">
		/// true:打开日志本地写入记录；<br />
		/// false:关闭日志本地写入记录
		/// </param>
		/// <param name="consoleOut">
		/// true:控制台输出日志；<br />
		/// false:控制不输出日志；<br />
		/// null:根据底层代码传入的参数来做操作
		/// </param>
		/// <param name="noticeAsync">
		/// 异步通知；<br />
		/// 外部传入的 Func 方法；<br />
		/// 用于给外部通知日志信息；<br />
		/// 此方法常用于日志入库</param>
		/// <returns>操作结果</returns>
		OperateResult LogOperateSet(bool logOut = true, bool? consoleOut = null, Func<object, LogEventLevel, string?, Exception?, Task>? noticeAsync = null);

		/// <summary>
		/// 日志操作参数设置；<br />
		/// logOut为false时,consoleOut将不生效<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logOut">
		/// true:打开日志本地写入记录；<br />
		/// false:关闭日志本地写入记录
		/// </param>
		/// <param name="consoleOut">
		/// true:控制台输出日志；<br />
		/// false:控制不输出日志；<br />
		/// null:根据底层代码传入的参数来做操作
		/// </param>
		/// <param name="noticeAsync">
		/// 异步通知；<br />
		/// 外部传入的 Func 方法；<br />
		/// 用于给外部通知日志信息；<br />
		/// 此方法常用于日志入库</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> LogOperateSetAsync(bool logOut = true, bool? consoleOut = null, Func<object, LogEventLevel, string?, Exception?, Task>? noticeAsync = null, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 日志操作参数设置；<br />
		/// logOut为false时,consoleOut将不生效<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logOut">
		/// true:打开日志本地写入记录；<br />
		/// false:关闭日志本地写入记录
		/// </param>
		/// <param name="consoleOut">
		/// true:控制台输出日志；<br />
		/// false:控制不输出日志；<br />
		/// null:根据底层代码传入的参数来做操作
		/// </param>
		/// <param name="notice">
		/// 通知；<br />
		/// 外部传入的 Action 方法；<br />
		/// 用于给外部通知日志信息；<br />
		/// 此方法常用于日志入库</param>
		/// <returns>操作结果</returns>
		OperateResult LogOperateSet(bool logOut = true, bool? consoleOut = null, Action<object, LogEventLevel, string?, Exception?>? notice = null);

		/// <summary>
		/// 日志操作参数设置；<br />
		/// logOut为false时,consoleOut将不生效<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logOut">
		/// true:打开日志本地写入记录；<br />
		/// false:关闭日志本地写入记录
		/// </param>
		/// <param name="consoleOut">
		/// true:控制台输出日志；<br />
		/// false:控制不输出日志；<br />
		/// null:根据底层代码传入的参数来做操作
		/// </param>
		/// <param name="notice">
		/// 通知；<br />
		/// 外部传入的 Action 方法；<br />
		/// 用于给外部通知日志信息；<br />
		/// 此方法常用于日志入库</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> LogOperateSetAsync(bool logOut = true, bool? consoleOut = null, Action<object, LogEventLevel, string?, Exception?>? notice = null, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 日志操作参数设置；<br />
		/// logOut为false时,consoleOut将不生效<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logOut">
		/// true:打开日志本地写入记录；<br />
		/// false:关闭日志本地写入记录
		/// </param>
		/// <param name="consoleOut">
		/// true:控制台输出日志；<br />
		/// false:控制不输出日志；<br />
		/// null:根据底层代码传入的参数来做操作
		/// </param>
		/// <returns>操作结果</returns>
		OperateResult LogOperateSet(bool logOut = true, bool? consoleOut = null);

		/// <summary>
		/// 日志操作参数设置；<br />
		/// logOut为false时,consoleOut将不生效<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logOut">
		/// true:打开日志本地写入记录；<br />
		/// false:关闭日志本地写入记录
		/// </param>
		/// <param name="consoleOut">
		/// true:控制台输出日志；<br />
		/// false:控制不输出日志；<br />
		/// null:根据底层代码传入的参数来做操作
		/// </param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> LogOperateSetAsync(bool logOut = true, bool? consoleOut = null, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 日志操作参数设置<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logModel">日志数据</param>
		/// <returns>操作结果</returns>
		OperateResult LogOperateSet(LogModel logModel);

		/// <summary>
		/// 日志操作参数设置<br />
		/// 最高优先级执行，这样才会整体生效
		/// </summary>
		/// <param name="logModel">日志数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> LogOperateSetAsync(LogModel logModel, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 日志操作参数获取
		/// </summary>
		/// <returns>操作结果</returns>
		OperateResult LogOperateGet();

		/// <summary>
		/// 日志操作参数获取
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>操作结果</returns>
		Task<OperateResult> LogOperateGetAsync(CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 消息中间件接口；<br />
	/// 定义消息队列的全量能力契约，支持打开/关闭、生产/消费、获取状态、事件、参数、创建实例、日志、多语言、克隆、资源释放等操作；<br />
	/// 继承 IOn, IOff, IProducer, IConsumer, IGetStatus, IEvent, IGetParam, ICreateInstance, ILog, ILanguage, IDisposable, IAsyncDisposable
	/// </summary>
	public interface IMq : IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
	}
	/// <summary>
	/// 对象接口；<br />
	/// 定义获取底层通信对象的同步和异步操作；<br />
	/// 用于外部直接访问底层 Socket/串口等通信对象
	/// </summary>
	public interface IObject
	{
		/// <summary>
		/// 获取底层通信对象
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult GetBaseObject();

		/// <summary>
		/// 获取底层通信对象异步
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 关闭接口；<br />
	/// 定义通信连接或服务关闭的同步和异步关闭操作；<br />
	/// 支持强制关闭（不验证状态）和 CancellationToken 取消机制
	/// </summary>
	public interface IOff
	{
		/// <summary>
		/// 关闭
		/// </summary>
		/// <param name="hardClose">强制关闭，不验证状态</param>
		/// <returns>统一结果</returns>
		OperateResult Off(bool hardClose = false);

		/// <summary>
		/// 关闭
		/// </summary>
		/// <param name="hardClose">强制关闭，不验证状态</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 打开接口；<br />
	/// 定义通信连接或服务启动的同步和异步打开操作；<br />
	/// 支持 CancellationToken 取消机制
	/// </summary>
	public interface IOn
	{
		/// <summary>
		/// 打开
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult On();

		/// <summary>
		/// 异步打开
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// OPC UA 操作接口；<br />
	/// 定义 OPC UA 协议下读取所有节点、获取节点信息、数据类型转换等操作；<br />
	/// 继承 IDaq，具备数据采集器的全部能力；<br />
	/// 注意：当点位过多时可能存在读取不到的情况
	/// </summary>
	public interface IOu : IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 获取当前服务ID
		/// </summary>
		/// <returns>object 对象 NodeId</returns>
		object? GetNodeID();

		/// <summary>
		/// 获取指定分支下的节点
		/// </summary>
		/// <param name="nodeId">id 对象 NodeId</param>
		/// <returns>object 对象 ReferenceDescriptionCollection</returns>
		Task<object> GetAllNode(object nodeId);

		/// <summary>
		/// 获取树节点图片类型
		/// </summary>
		/// <param name="target">目标 对象 ReferenceDescription</param>
		/// <param name="sourceId">id 对象 NodeId</param>
		/// <returns>图片类型</returns>
		Task<string> GetNodeIconType(object target, object sourceId);

		/// <summary>
		/// 详细读取所有节点数据
		/// </summary>
		/// <param name="nodeIds">节点id集合,对象 List(NodeId) </param>
		/// <returns>object 对象 DataValue[]</returns>
		object DetailedReadAllNodeData(object nodeIds);

		/// <summary>
		/// 获取访问级别
		/// </summary>
		/// <param name="value">数据值 对象 DataValue</param>
		/// <returns>访问级别</returns>
		string GetAccessLevel(object value);

		/// <summary>
		/// OPCUA 数据类型转换成 Core 数据类型
		/// </summary>
		/// <param name="builtInType">类型 对象 BuiltInType</param>
		/// <returns>数据类型</returns>
		DataType TypeConvert(object builtInType);

		/// <summary>
		/// 获取指定节点的数据类型
		/// </summary>
		/// <param name="nodeId">id 对象 NodeId</param>
		/// <returns>object 对象 BuiltInType</returns>
		object GetNodeValueType(object nodeId);
	}
	/// <summary>
	/// 自动地址组包与解包流程
	/// </summary>
	public interface IPacker
	{
		/// <summary>
		/// 智能地址自动组包 — 传入供应商 ProtocolType 枚举名和地址集合，自动判断是否支持组包。
		/// <para>支持组包 → 执行地址合并优化后返回</para>
		/// <para>不支持组包 → 原样返回传入的 address</para>
		/// <para>永远不会返回 null</para>
		/// </summary>
		/// <param name="address">原始地址集合</param>
		/// <param name="protocolTypeKey">
		/// 供应商 ProtocolType 枚举的字符串名，例如：
		/// "SiemensS7Net_S1200"、"ModbusTcpNet"、"OmronFinsNet" 等。
		/// 支持各供应商数据类中 <c>ProtocolType.ToString()</c> 的返回值。
		/// </param>
		/// <param name="maxByteLength">单次批量读取最大字节数。0 = 自动使用协议上限（推荐）。也可以使用调试工具直接读取来得知最大字节数</param>
		/// <param name="format">数据字节序格式</param>
		/// <returns>
		/// 支持组包 → 优化后的 Address；不支持 → 原始 address。
		/// 传入 null 返回 null。
		/// </returns>
		OperateResult Packer(Address? address, string protocolTypeKey, int maxByteLength = 0, DataFormat format = DataFormat.ABCD);

		/// <summary>
		/// 异步智能地址自动组包 — 传入供应商 ProtocolType 枚举名和地址集合，自动判断是否支持组包。
		/// <para>支持组包 → 执行地址合并优化后返回</para>
		/// <para>不支持组包 → 原样返回传入的 address</para>
		/// <para>永远不会返回 null</para>
		/// </summary>
		/// <param name="address">原始地址集合</param>
		/// <param name="protocolTypeKey">
		/// 供应商 ProtocolType 枚举的字符串名，例如：
		/// "SiemensS7Net_S1200"、"ModbusTcpNet"、"OmronFinsNet" 等。
		/// 支持各供应商数据类中 <c>ProtocolType.ToString()</c> 的返回值。
		/// </param>
		/// <param name="maxByteLength">单次批量读取最大字节数。0 = 自动使用协议上限（推荐）。也可以使用调试工具直接读取来得知最大字节数</param>
		/// <param name="format">数据字节序格式</param>
		/// <param name="token">取消通知</param>
		/// <returns>
		/// 支持组包 → 优化后的 Address；不支持 → 原始 address。
		/// 传入 null 返回 null。
		/// </returns>
		Task<OperateResult> PackerAsync(Address? address, string protocolTypeKey, int maxByteLength = 0, DataFormat format = DataFormat.ABCD, CancellationToken token = default(CancellationToken));

		/// /// <summary>
		/// 自动解包
		/// </summary>
		/// <param name="addressValue">
		/// 地址获取的值<br />
		/// 必须是组包后获取的 AddressValue<br />
		/// 组包时会设置特定参数
		/// </param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <returns>返回对应的键值数据:ConcurrentDictionary《string, AddressValue》</returns>
		OperateResult UnPacker(AddressValue addressValue, bool isStringReverseByteWord = false);

		/// <summary>
		/// 异步自动解包
		/// </summary>
		/// <param name="addressValue">
		/// 地址获取的值<br />
		/// 必须是组包后获取的 AddressValue<br />
		/// 组包时会设置特定参数
		/// </param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <param name="token">取消通知</param>
		/// <returns>返回对应的键值数据:ConcurrentDictionary《string, AddressValue》</returns>
		Task<OperateResult> UnPackerAsync(AddressValue addressValue, bool isStringReverseByteWord = false, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 自动解包
		/// </summary>
		/// <param name="pairs">
		/// 地址获取的值<br />
		/// 必须是组包后获取的 ConcurrentDictionary《string, AddressValue》<br />
		/// 组包时会设置特定参数
		/// </param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <returns>返回对应的键值数据:List《ConcurrentDictionary《string, AddressValue》》</returns>
		OperateResult UnPacker(ConcurrentDictionary<string, AddressValue> pairs, bool isStringReverseByteWord = false);

		/// <summary>
		/// 异步自动解包
		/// </summary>
		/// <param name="pairs">
		/// 地址获取的值<br />
		/// 必须是组包后获取的 ConcurrentDictionary《string, AddressValue》<br />
		/// 组包时会设置特定参数
		/// </param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <param name="token">取消通知</param>
		/// <returns>返回对应的键值数据:List《ConcurrentDictionary《string, AddressValue》》</returns>
		Task<OperateResult> UnPackerAsync(ConcurrentDictionary<string, AddressValue> pairs, bool isStringReverseByteWord = false, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 生产者接口；<br />
	/// 定义消息中间件生产数据的同步和异步操作；<br />
	/// 支持字符串内容（可指定编码）和字节数组内容两种生产模式
	/// </summary>
	public interface IProducer
	{
		/// <summary>
		/// 生产
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="content">内容</param>
		/// <param name="encoding">编码格式</param>
		/// <returns>统一结果</returns>
		OperateResult Produce(string topic, string content, Encoding? encoding = null);

		/// <summary>
		/// 生产
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="content">内容</param>
		/// <param name="encoding">编码格式</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> ProduceAsync(string topic, string content, Encoding? encoding = null, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 生产
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="content">内容</param>
		/// <returns>统一结果</returns>
		OperateResult Produce(string topic, byte[] content);

		/// <summary>
		/// 生产
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="content">内容</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 读取接口；<br />
	/// 定义对设备地址的同步和异步数据读取操作；<br />
	/// 通过 Address 对象指定读取的地址集合
	/// </summary>
	public interface IRead
	{
		/// <summary>
		/// 读取
		/// </summary>
		/// <param name="address">地址的数据</param>
		/// <returns>统一结果</returns>
		OperateResult Read(Address address);

		/// <summary>
		/// 异步读取
		/// </summary>
		/// <param name="address">地址的数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 移除指定客户端接口；<br />
	/// 定义服务端主动断开指定客户端连接的同步和异步操作；<br />
	/// 通过地址数组指定要移除的客户端
	/// </summary>
	public interface IRemove
	{
		/// <summary>
		/// 移除指定客户端
		/// </summary>
		/// <param name="address">地址</param>
		/// <returns>统一出参</returns>
		OperateResult Remove(string[] address);

		/// <summary>
		/// 移除指定客户端
		/// </summary>
		/// <param name="address">地址</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		Task<OperateResult> RemoveAsync(string[] address, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 发送接口；<br />
	/// 定义客户端字节数据的同步和异步发送操作；<br />
	/// 适用于 TCP/串口等客户端通信场景
	/// </summary>
	public interface ISend
	{
		/// <summary>
		/// 发送
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <returns>统一出参</returns>
		OperateResult Send(byte[] data);

		/// <summary>
		/// 发送
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 服务端发送接口；<br />
	/// 定义服务端向客户端发送数据的同步和异步操作；<br />
	/// 支持单发（指定地址）和群发（地址为空则广播）
	/// </summary>
	public interface ISendService
	{
		/// <summary>
		/// 发送
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <param name="address">地址为空则群发</param>
		/// <returns>统一出参</returns>
		OperateResult Send(byte[] data, string[]? address = null);

		/// <summary>
		/// 发送
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <param name="address">地址为空则群发</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		Task<OperateResult> SendAsync(byte[] data, string[]? address = null, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 发送
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <param name="address">地址为空则群发</param>
		/// <returns>统一出参</returns>
		OperateResult Send(byte[] data, string address);

		/// <summary>
		/// 发送
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <param name="address">地址为空则群发</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		Task<OperateResult> SendAsync(byte[] data, string address, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 发送等待接口；<br />
	/// 定义发送数据并等待应答结果的同步和异步操作；<br />
	/// 适用于单次命令应答模式（请求-响应）
	/// </summary>
	public interface ISendWait
	{
		/// <summary>
		/// 发送等待结果，适用于单次命令应答模式<br />
		/// 会先关闭数据监控,在进行数据发送,函数内捕获数据,完成后在启动数据监控<br />
		/// 只要收到一次数据就会立马返回收到的数据
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		OperateResult SendWait(byte[] data, CancellationToken token);

		/// <summary>
		/// 异步发送等待结果，适用于单次命令应答模式<br />
		/// 会先关闭数据监控,在进行数据发送,函数内捕获数据,完成后在启动数据监控<br />
		/// 只要收到一次数据就会立马返回收到的数据
		/// </summary>
		/// <param name="data">字节数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token);
	}
	/// <summary>
	/// 状态接口；<br />
	/// 定义获取当前通信/数采/消息队列连接状态的同步和异步操作
	/// </summary>
	public interface IStatus
	{
		/// <summary>
		/// 获取状态
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult GetStatus();

		/// <summary>
		/// 获取状态
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 自定义订阅接口；<br />
	/// 定义地址数据的订阅与取消订阅操作；<br />
	/// 订阅后通过 IEvent 事件接收数据变更通知；<br />
	/// 继承 IEvent
	/// </summary>
	public interface ISubscribe : IEvent
	{
		/// <summary>
		/// 订阅
		/// </summary>
		/// <param name="address">节点数据</param>
		/// <returns>统一出参</returns>
		OperateResult Subscribe(Address address);

		/// <summary>
		/// 订阅
		/// </summary>
		/// <param name="address">节点数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 取消订阅
		/// </summary>
		/// <param name="address">节点数据</param>
		/// <returns>统一出参</returns>
		OperateResult UnSubscribe(Address address);

		/// <summary>
		/// 取消订阅
		/// </summary>
		/// <param name="address">节点数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一出参</returns>
		Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// WebApi 接口；<br />
	/// 定义数采器内置 WebApi 服务的启动、停止、状态查询和请求示例获取等操作；<br />
	/// 通过 WAModel 配置 IP 地址、端口和跨域策略
	/// </summary>
	public interface IWA
	{
		/// <summary>
		/// 启动WebApi
		/// </summary>
		/// <param name="wAModel">WebApi模型</param>
		/// <returns>统一结果</returns>
		OperateResult WAOn(WAModel wAModel);

		/// <summary>
		/// 启动WebApi
		/// </summary>
		/// <param name="wAModel">WebApi模型</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> WAOnAsync(WAModel wAModel, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 停止WebApi
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult WAOff();

		/// <summary>
		/// 停止WebApi
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> WAOffAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// WebApi状态
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult WAStatus();

		/// <summary>
		/// WebApi状态
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> WAStatusAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// WebApi请求示例
		/// </summary>
		/// <returns>统一结果</returns>
		OperateResult WARequestExample();

		/// <summary>
		/// WebApi请求示例
		/// </summary>
		/// <returns>统一结果</returns>
		Task<OperateResult> WARequestExampleAsync(CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 写入接口；<br />
	/// 定义向设备地址写入数据的同步和异步操作；<br />
	/// 支持多种写入模式：基础键值对、指定编码类型、WriteModel 完整模型
	/// </summary>
	public interface IWrite
	{
		/// <summary>
		/// 写入
		/// </summary>
		/// <param name="values">写入的数据点，与数据</param>
		/// <returns>统一结果</returns>
		OperateResult Write(ConcurrentDictionary<string, object> values);

		/// <summary>
		/// 异步写入
		/// </summary>
		/// <param name="values">写入的数据点，与数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> WriteAsync(ConcurrentDictionary<string, object> values, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 写入
		/// </summary>
		/// <param name="values">写入的数据点，数据，编码类型</param>
		/// <returns>统一结果</returns>
		OperateResult Write(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values);

		/// <summary>
		/// 异步写入
		/// </summary>
		/// <param name="values">写入的数据点，数据，编码类型</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 写入
		/// </summary>
		/// <param name="values">写入的数据点，与数据</param>
		/// <returns>统一结果</returns>
		OperateResult Write(ConcurrentDictionary<string, WriteModel> values);

		/// <summary>
		/// 异步写入
		/// </summary>
		/// <param name="values">写入的数据点，与数据</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>统一结果</returns>
		Task<OperateResult> WriteAsync(ConcurrentDictionary<string, WriteModel> values, CancellationToken token = default(CancellationToken));
	}
}
namespace Snet.Model.@event
{
	/// <summary>
	///             为<see cref="T:Snet.Model.event.EventHandlerAsync`1" />提供数据，可以异步调用的事件。
	/// </summary>
	public class EventArgsAsync
	{
		/// <summary>
		/// 提供用于没有事件数据的事件的值
		/// </summary>
		public static readonly EventArgsAsync Empty = new EventArgsAsync();

		/// <summary>
		/// 传播应取消操作的通知<br />
		/// 异步事件所需
		/// </summary>
		[JsonIgnore]
		[JsonIgnore]
		public CancellationToken CancellationToken { get; }

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="cancellationToken">传播应取消操作的通知</param>
		public EventArgsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			CancellationToken = cancellationToken;
		}

		/// <summary>
		/// 创建或默认
		/// </summary>
		/// <param name="cancellationToken">传播应取消操作的通知</param>
		/// <returns>异步事件</returns>
		public static EventArgsAsync CreateOrDefault(CancellationToken cancellationToken)
		{
			if (cancellationToken.CanBeCanceled)
			{
				return new EventArgsAsync(cancellationToken);
			}
			return Empty;
		}
	}
	/// <summary>
	/// 事件处理异步
	/// </summary>
	/// <typeparam name="TEvent">异步数据对象</typeparam>
	/// <param name="sender">源</param>
	/// <param name="e">异步数据对象</param>
	/// <returns>任务</returns>
	public delegate Task EventHandlerAsync<in TEvent>(object? sender, TEvent e) where TEvent : EventArgsAsync;
	/// <summary>
	/// 异步事件包装器，线程安全。
	/// <para>管理 <see cref="T:Snet.Model.event.EventHandlerAsync`1" /> 的订阅、取消订阅和触发。</para>
	/// <para>注意：从 struct 改为 class，确保通过属性 add/remove 访问的是同一引用，避免订阅丢失。</para>
	/// </summary>
	/// <typeparam name="TEvent">异步事件参数类型，必须继承 <see cref="T:Snet.Model.event.EventArgsAsync" /></typeparam>
	public struct EventingWrapperAsync<TEvent> where TEvent : EventArgsAsync
	{
		private Delegate[]? _handlers;

		private string? _context;

		private Func<Exception, string, CancellationToken, Task>? _onException;

		public bool IsEmpty => Unsafe.As<EventingWrapperAsync<TEvent>, EventingWrapperAsync<>>(ref this)._eventHandler == null;

		private event EventHandlerAsync<TEvent>? _eventHandler;

		public EventingWrapperAsync(string context, Func<Exception, string, CancellationToken, Task> onException)
		{
			Unsafe.As<EventingWrapperAsync<TEvent>, EventingWrapperAsync<>>(ref this)._eventHandler = null;
			_handlers = null;
			_context = context;
			_onException = onException;
		}

		public void AddHandler(EventHandlerAsync<TEvent>? handler)
		{
			_eventHandler += handler;
			_handlers = null;
		}

		public void RemoveHandler(EventHandlerAsync<TEvent>? handler)
		{
			_eventHandler -= handler;
			_handlers = null;
		}

		public Task InvokeAsync(object? sender, TEvent parameter)
		{
			Delegate[] array = _handlers;
			if (array == null)
			{
				array = Unsafe.As<EventingWrapperAsync<TEvent>, EventingWrapperAsync<>>(ref this)._eventHandler?.GetInvocationList();
				if (array == null)
				{
					return Task.CompletedTask;
				}
				_handlers = array;
			}
			return InternalInvoke(array, sender, parameter);
		}

		private readonly async Task InternalInvoke(Delegate[] handlers, object? sender, TEvent @event)
		{
			for (int i = 0; i < handlers.Length; i++)
			{
				EventHandlerAsync<TEvent> eventHandlerAsync = (EventHandlerAsync<TEvent>)handlers[i];
				try
				{
					await eventHandlerAsync(sender, @event).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception arg)
				{
					if (_onException == null)
					{
						throw;
					}
					await _onException(arg, _context, @event.CancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		public void Takeover(in EventingWrapperAsync<TEvent> other)
		{
			Unsafe.As<EventingWrapperAsync<TEvent>, EventingWrapperAsync<>>(ref this)._eventHandler = Unsafe.As<EventingWrapperAsync<TEvent>, EventingWrapperAsync<>>(ref other)._eventHandler;
			_handlers = other._handlers;
			_context = other._context;
			_onException = other._onException;
		}
	}
}
namespace Snet.Model.@enum
{
	/// <summary>
	/// 地址类型枚举；<br />
	/// 定义数据采集地址的类型：实际地址或虚拟地址；<br />
	/// 虚拟地址支持多种模拟模式：静态、动态随机、动态随机范围、动态顺序、动态顺序范围
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum AddressType
	{
		/// <summary>
		/// 实际地址
		/// </summary>
		[Description("实际地址")]
		Reality,
		/// <summary>
		/// 虚拟静态地址
		/// </summary>
		[Description("虚拟静态地址")]
		VirtualStatic,
		/// <summary>
		/// 虚拟动态随机变化地址；<br />
		/// 地址后添加格式；<br />
		/// {100} <br />
		/// {更新间隔}
		/// </summary>
		[Description("虚拟动态随机变化地址")]
		VirtualDynamic_Random,
		/// <summary>
		/// 虚拟动态随机范围变化地址；<br />
		/// 地址后添加格式；<br />
		/// {100,1^100} <br /> 
		/// {100,1.1^9.9} <br /> 
		/// {500,00:00:00^12:00:00} <br /> 
		/// {500,2000-01-01^2024-01-01}　<br />
		/// {更新间隔,最小值^最大值}
		/// </summary>
		[Description("虚拟动态随机范围变化地址")]
		VirtualDynamic_RandomScope,
		/// <summary>
		/// 虚拟动态顺序变化地址；<br />
		/// 地址后添加格式；<br />
		/// {100,1} <br />
		/// {更新间隔,增长比例}
		/// </summary>
		[Description("虚拟动态顺序变化地址")]
		VirtualDynamic_Order,
		/// <summary>
		/// 虚拟动态顺序范围变化地址；<br />
		/// 地址后添加格式；<br />
		/// {100,1,1^100} <br />
		/// {100,0.1,1.1^9.9} <br />
		/// {500,1,00:00:00^12:00:00} <br />
		/// {500,1,2000-01-01^2024-01-01}<br />
		/// {更新间隔,增长比例,最小值^最大值}
		/// </summary>
		[Description("虚拟动态顺序范围变化地址")]
		VirtualDynamic_OrderScope
	}
	/// <summary>
	/// 数据格式枚举（字节序）；<br />
	/// 应用于多字节数据的解析或生成格式；<br />
	/// ABCD:顺序，BADC:单字反转，CDAB:双字反转，DCBA:倒序
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum DataFormat
	{
		/// <summary>
		/// 按照顺序排序
		/// </summary>
		[Description("按照顺序排序")]
		ABCD,
		/// <summary>
		/// 按照单字反转
		/// </summary>
		[Description("按照单字反转")]
		BADC,
		/// <summary>
		/// 按照双字反转
		/// </summary>
		[Description("按照双字反转")]
		CDAB,
		/// <summary>
		/// 按照倒序排序
		/// </summary>
		[Description("按照倒序排序")]
		DCBA
	}
	/// <summary>
	/// 数据类型枚举；<br />
	/// 定义地址读取/写入时的数据类型；<br />
	/// 包含基本类型（Bool/Int16/Float/Double/String 等）和数组类型（BoolArray/IntArray 等）；<br />
	/// 部分类型存在别名（如 Short=Int16、Float=Single、Int=Int32 等）
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum DataType
	{
		/// <summary>
		/// 未定义
		/// </summary>
		[Description("未定义")]
		None,
		/// <summary>
		/// 字节；<br />
		/// 1字节8位
		/// </summary>
		[Description("字节")]
		Byte,
		/// <summary>
		/// 布尔值；<br />
		/// 1字节8位
		/// </summary>
		[Description("布尔值")]
		Bool,
		/// <summary>
		/// 双精度浮点数；<br />
		/// 8字节64位
		/// </summary>
		[Description("双精度浮点数")]
		Double,
		/// <summary>
		/// 单精度浮点数；<br />
		/// 4字节32位
		/// </summary>
		[Description("单精度浮点数")]
		Float,
		/// <summary>
		/// 单精度浮点数；<br />
		/// 4字节32位
		/// </summary>
		[Description("单精度浮点数")]
		Single,
		/// <summary>
		/// 有符号16位整数；<br />
		/// 2字节16位
		/// </summary>
		[Description("有符号16位整数")]
		Short,
		/// <summary>
		/// 有符号16位整数；
		/// 2字节16位
		/// </summary>
		[Description("有符号16位整数")]
		Int16,
		/// <summary>
		/// 无符号16位整数；<br />
		/// 2字节16位
		/// </summary>
		[Description("无符号16位整数")]
		Ushort,
		/// <summary>
		/// 无符号16位整数；<br />
		/// 2字节16位
		/// </summary>
		[Description("无符号16位整数")]
		UInt16,
		/// <summary>
		/// 有符号32位整数；<br />
		/// 4字节32位
		/// </summary>
		[Description("有符号32位整数")]
		Int,
		/// <summary>
		/// 有符号32位整数；<br />
		/// 4字节32位
		/// </summary>
		[Description("有符号32位整数")]
		Int32,
		/// <summary>
		/// 无符号32位整数；<br />
		/// 4字节32位
		/// </summary>
		[Description("无符号32位整数")]
		Uint,
		/// <summary>
		/// 无符号32位整数；<br />
		/// 4字节32位
		/// </summary>
		[Description("无符号32位整数")]
		UInt32,
		/// <summary>
		/// 有符号64位整数；<br />
		/// 8字节64位
		/// </summary>
		[Description("有符号64位整数")]
		Long,
		/// <summary>
		/// 有符号64位整数；<br />
		/// 8字节64位
		/// </summary>
		[Description("有符号64位整数")]
		Int64,
		/// <summary>
		/// 无符号64位整数；<br />
		/// 8字节64位
		/// </summary>
		[Description("无符号64位整数")]
		Ulong,
		/// <summary>
		/// 无符号64位整数；<br />
		/// 8字节64位
		/// </summary>
		[Description("无符号64位整数")]
		UInt64,
		/// <summary>
		/// 日期和时间；<br />
		/// 8字节64位；<br />
		/// 不支持读取写入
		/// </summary>
		[Description("日期和时间")]
		DateTime,
		/// <summary>
		/// 日期；<br />
		/// 不支持读取写入
		/// </summary>
		[Description("日期")]
		Date,
		/// <summary>
		/// 时间；<br />
		/// 不支持读取写入
		/// </summary>
		[Description("时间")]
		Time,
		/// <summary>
		/// 字符串；<br />
		/// 2字节16位
		/// </summary>
		[Description("字符串")]
		String,
		/// <summary>
		/// Unicode字符；<br />
		/// 2字节16位
		/// </summary>
		[Description("Unicode字符")]
		Char,
		/// <summary>
		/// 字节数组；<br />
		/// 当在 WebApi 写入时，请以 Byte[] 的 string 表达方式写入 0x00 0x01 0x02 0x03<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("字节数组")]
		ByteArray,
		/// <summary>
		/// 布尔数组值；<br />
		/// [true,false,true,false]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("布尔数组值")]
		BoolArray,
		/// <summary>
		/// 双精度浮点数组值；<br />
		/// [1.1,2.1,3.1,4.1,5.1,6.1]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("双精度浮点数组")]
		DoubleArray,
		/// <summary>
		/// 单精度浮点数组；<br />
		/// [1.1,2.1,3.1,4.1,5.1,6.1]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("单精度浮点数组")]
		FloatArray,
		/// <summary>
		/// 单精度浮点数组；<br />
		/// [1.1,2.1,3.1,4.1,5.1,6.1]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("单精度浮点数组")]
		SingleArray,
		/// <summary>
		/// 有符号16位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("有符号16位整数组")]
		ShortArray,
		/// <summary>
		/// 有符号16位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("有符号16位整数组")]
		Int16Array,
		/// <summary>
		/// 无符号16位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("无符号16位整数组")]
		UshortArray,
		/// <summary>
		/// 无符号16位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("无符号16位整数组")]
		UInt16Array,
		/// <summary>
		/// 有符号32位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("有符号32位整数组")]
		IntArray,
		/// <summary>
		/// 有符号32位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("有符号32位整数组")]
		Int32Array,
		/// <summary>
		/// 无符号32位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("无符号32位整数组")]
		UintArray,
		/// <summary>
		/// 无符号32位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("无符号32位整数组")]
		UInt32Array,
		/// <summary>
		/// 有符号64位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("有符号64位整数组")]
		LongArray,
		/// <summary>
		/// 有符号64位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("有符号64位整数组")]
		Int64Array,
		/// <summary>
		/// 无符号64位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("无符号64位整数组")]
		UlongArray,
		/// <summary>
		/// 无符号64位整数组；<br />
		/// [1,2,3,4,5,6]<br />
		/// 不支持虚拟地址
		/// </summary>
		[Description("无符号64位整数组")]
		UInt64Array
	}
	/// <summary>
	/// 编码类型枚举；<br />
	/// 定义字符串数据的编码方式；<br />
	/// 枚举值对应 .NET 编码代码页（CodePage）；<br />
	/// 支持 ANSI(0), GB2312(936), Unicode(1200), UTF-8(65001), ASCII(20127) 等
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum EncodingType
	{
		/// <summary>
		/// UTF-16
		/// </summary>
		[Description("UTF-16")]
		Unicode = 1200,
		/// <summary>
		/// UTF-16
		/// </summary>
		[Description("UTF-16")]
		BigEndianUnicode = 1201,
		/// <summary>
		/// UTF-32
		/// </summary>
		[Description("UTF-32")]
		UTF32 = 12000,
		/// <summary>
		/// UTF-32
		/// </summary>
		[Description("UTF-32")]
		BigEndianUTF32 = 12001,
		/// <summary>
		/// UTF-8
		/// </summary>
		[Description("UTF-8")]
		UTF8 = 65001,
		/// <summary>
		/// ASCII
		/// </summary>
		[Description("ASCII")]
		ASCII = 20127,
		/// <summary>
		/// GB2312
		/// </summary>
		[Description("GB2312")]
		GB2312 = 936,
		/// <summary>
		/// ANSI
		/// </summary>
		[Description("ANSI")]
		ANSI = 0
	}
	/// <summary>
	/// 语言类型枚举；<br />
	/// 定义系统支持的语言类型；<br />
	/// 配合 ILanguage 接口和 MultilingualAttribute 特性实现多语言支持
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum LanguageType
	{
		/// <summary>
		/// 中文
		/// </summary>
		[Description("中文")]
		zh,
		/// <summary>
		/// 英文
		/// </summary>
		[Description("英文")]
		en
	}
	/// <summary>
	/// 插件类型
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum PluginType
	{
		/// <summary>
		/// 数据采集插件
		/// </summary>
		[Description("数据采集插件")]
		Daq,
		/// <summary>
		/// 消息队列传输插件
		/// </summary>
		[Description("消息队列传输插件")]
		Mq
	}
	/// <summary>
	/// 质量类型枚举；<br />
	/// 表示数据采集结果的质量状态；<br />
	/// None(-1):未经处理，Exception(0):异常，Normal(1):正常，<br />
	/// DataTypeError(2):类型错误，ParseUnknown(3):解析成功但正确性未知，ParseError(4):解析错误
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum QualityType
	{
		/// <summary>
		/// 未经过处理
		/// </summary>
		[Description("未经过处理")]
		None = -1,
		/// <summary>
		/// 异常
		/// </summary>
		[Description("异常")]
		Exception,
		/// <summary>
		/// 正常
		/// </summary>
		[Description("正常")]
		Normal,
		/// <summary>
		/// 数据类型错误
		/// </summary>
		[Description("数据类型错误")]
		DataTypeError,
		/// <summary>
		/// 数据经过解析，并且解析成功，无法得知数据的正确性
		/// </summary>
		[Description("数据经过解析，并且解析成功，无法得知数据的正确性")]
		ParseUnknown,
		/// <summary>
		/// 解析错误
		/// </summary>
		[Description("解析错误")]
		ParseError
	}
	/// <summary>
	/// 消息中间件响应类型枚举；<br />
	/// 定义消息中间件接收数据的格式类型；<br />
	/// Bytes:字节数据，Content:纯内容，ContentWithTopic:主题+内容（使用 ResponseModel 反序列化）
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ResponseType
	{
		/// <summary>
		/// 字节数据
		/// </summary>
		[Description("字节")]
		Bytes,
		/// <summary>
		/// 只有内容
		/// </summary>
		[Description("内容")]
		Content,
		/// <summary>
		/// 有主题的内容；<br />
		/// 请使用 ResponseModel 反序列化
		/// </summary>
		[Description("内容与主题")]
		ContentWithTopic
	}
}
namespace Snet.Model.data
{
	/// <summary>
	/// 地址容器；<br />
	/// 包含多个地址详情（AddressDetails）的集合；<br />
	/// 通过 SN 唯一标识符区分不同的地址组；<br />
	/// 提供地址查询、校验、虚拟地址过滤、实际地址计数等操作
	/// </summary>
	public class Address
	{
		/// <summary>
		/// 可以理解成唯一标识符（可以存机台号、组名、车间、厂）
		/// </summary>
		public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

		/// <summary>
		/// 所有地址 也可以是一个地址  此数组里面的地址应是唯一的
		/// </summary>
		public List<AddressDetails> AddressArray { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreationTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 带参构造函数
		/// </summary>
		/// <param name="sn">唯一标识符</param>
		/// <param name="addressDetails">地址详情集合</param>
		/// <param name="creationTime">创建时间</param>
		public Address(string sn, List<AddressDetails> addressDetails, DateTime creationTime)
		{
			SN = sn;
			AddressArray = addressDetails;
			CreationTime = creationTime;
		}

		/// <summary>
		/// 带参构造函数
		/// </summary>
		/// <param name="addressDetails">地址详情集合</param>
		public Address(List<AddressDetails> addressDetails)
		{
			AddressArray = addressDetails;
		}

		/// <summary>
		/// 带参构造函数；<br />
		/// 通过单个地址详情创建地址容器
		/// </summary>
		/// <param name="addressDetails">地址详情</param>
		public Address(AddressDetails addressDetails)
		{
			int num = 1;
			List<AddressDetails> list = new List<AddressDetails>(num);
			CollectionsMarshal.SetCount(list, num);
			CollectionsMarshal.AsSpan(list)[0] = addressDetails;
			AddressArray = list;
		}

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public Address()
		{
		}

		/// <summary>
		/// 获取地址信息；<br />
		/// 根据 PLC 地址名称和/或别名查找对应的地址详情；<br />
		/// 两个参数均为空时返回 null
		/// </summary>
		/// <param name="AddressName">PLC地址</param>
		/// <param name="AddressAnotherName">地址别名</param>
		/// <returns>地址详情，未找到返回 null</returns>
		public AddressDetails? GetAddressInfo(string? AddressName = null, string? AddressAnotherName = null)
		{
			if (AddressName != null && AddressAnotherName != null)
			{
				return AddressArray?.Find((AddressDetails c) => string.Equals(c.AddressName, AddressName) && string.Equals(c.AddressAnotherName, AddressAnotherName));
			}
			if (AddressName != null && AddressAnotherName == null)
			{
				return AddressArray?.Find((AddressDetails c) => string.Equals(c.AddressName, AddressName));
			}
			if (AddressName == null && AddressAnotherName != null)
			{
				return AddressArray?.Find((AddressDetails c) => string.Equals(c.AddressAnotherName, AddressAnotherName));
			}
			return null;
		}

		/// <summary>
		/// 检查地址集合是否存在无效数据；<br />
		/// 验证所有地址的 AddressName 是否均已填写
		/// </summary>
		/// <returns>
		/// true：所有地址数据有效；<br />
		/// false：存在无效数据（AddressName 为空）
		/// </returns>
		public bool CheckAddress()
		{
			if (AddressArray != null && AddressArray.Count > 0 && !AddressArray.Any((AddressDetails c) => string.IsNullOrWhiteSpace(c.AddressName)))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 移除虚拟地址，仅保留实际地址；<br />
		/// 常用于订阅场景，过滤掉虚拟模拟地址；<br />
		/// 返回新的列表，不会修改原始 AddressArray
		/// </summary>
		/// <returns>仅包含实际地址的地址详情集合</returns>
		public List<AddressDetails> RemoveVirtualAddress()
		{
			if (AddressArray != null && AddressArray.Count > 0)
			{
				return AddressArray.Where((AddressDetails c) => c.AddressType.Equals(AddressType.Reality)).ToList();
			}
			return AddressArray ?? new List<AddressDetails>();
		}

		/// <summary>
		/// 获取实际地址数量；<br />
		/// 排除所有虚拟地址后的实际地址总数；<br />
		/// 使用 LINQ Count 避免创建中间 List 对象
		/// </summary>
		/// <returns>实际地址数量</returns>
		public int GetRealityAddressCount()
		{
			if (AddressArray != null && AddressArray.Count > 0)
			{
				return AddressArray.Count((AddressDetails c) => c.AddressType.Equals(AddressType.Reality));
			}
			return 0;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<Address>(this, true) ?? string.Empty;
		}

		/// <summary>
		/// 重写 Equals；<br />
		/// 使用对象比较器进行深度比较
		/// </summary>
		/// <param name="o">待比较的对象</param>
		/// <returns>是否一致</returns>
		public override bool Equals(object? o)
		{
			if (o == null)
			{
				return false;
			}
			return ExtendMethod.Comparer<Address>(this, o as Address, (string[])null).Item1;
		}

		/// <summary>
		/// 重写 GetHashCode；<br />
		/// 与 Equals 保持一致，基于 SN 生成哈希值
		/// </summary>
		/// <returns>哈希值</returns>
		public override int GetHashCode()
		{
			return SN?.GetHashCode() ?? 0;
		}
	}
	/// <summary>
	/// 单个地址的详细信息；<br />
	/// 包含地址名称、别名、属性名、描述、数据类型、地址类型、编码类型、长度等；<br />
	/// 支持消息中间件参数（AddressMq）和数据解析参数（AddressParse）；<br />
	/// 支持实际地址和多种虚拟地址类型
	/// </summary>
	public class AddressDetails
	{
		/// <summary>
		/// 可以理解成唯一标识符（可以存机台号、组名、车间、厂）；<br />
		/// 默认为随机guid
		/// </summary>
		public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

		/// <summary>
		/// 唯一；<br />
		/// 实际的点位地址名称；<br />
		/// 是扩展数据时，可为空；<br />
		/// 不是扩展数据时，此项为PLC的地址
		/// </summary>
		public string AddressName { get; set; }

		/// <summary>
		/// 长度，默认值 1；<br />
		/// AddressDataType 存在 Array 类型时，根据实际情况修改此参数；<br />
		/// 读取对应的 Array 类型长度
		/// </summary>
		public ushort Length { get; set; } = 1;

		/// <summary>
		/// 编码类型
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public EncodingType EncodingType { get; set; } = EncodingType.UTF8;

		/// <summary>
		/// 地址别名
		/// </summary>
		public string? AddressAnotherName { get; set; }

		/// <summary>
		/// 地址属性名称（与定义的实体名称对应）；<br />
		/// 是扩展数据时，可为空；<br />
		/// 不是扩展数据时，此项为PLC的最后字段名
		/// </summary>
		public string? AddressPropertyName { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string? AddressDescribe { get; set; }

		/// <summary>
		/// 指定通信的扩展数据
		/// </summary>
		public object? AddressExtendParam { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public bool IsEnable { get; set; } = true;

		/// <summary>
		/// 地址消息中间件参数<br />
		/// 通过指定的消息中间件把数据发送出去
		/// </summary>
		public AddressMq? AddressMqParam { get; set; }

		/// <summary>
		/// 数据解析参数
		/// </summary>
		public AddressParse? AddressParseParam { get; set; }

		/// <summary>
		/// 数据类型
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DataType AddressDataType { get; set; } = DataType.String;

		/// <summary>
		/// 地址类型，默认实际地址
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public AddressType AddressType { get; set; }

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="sn">唯一标识</param>
		/// <param name="addressName">
		/// 唯一；<br />
		/// 实际的点位地址名称；<br />
		/// 是扩展数据时，可为空；<br />
		/// 不是扩展数据时，此项为PLC的地址</param>
		/// <param name="addressAnotherName">地址别名</param>
		/// <param name="addressPropertyName">地址属性名</param>
		/// <param name="addressDescribe">地址描述</param>
		/// <param name="addressExtendParam">地址扩展参数</param>
		/// <param name="addressMqParam">地址消息中间件参数</param>
		/// <param name="addressParseParam">解析参数</param>
		/// <param name="addressDataType">数据类型</param>
		/// <param name="addressType">地址类型</param>
		/// <param name="isEnable">是否启用</param>
		/// <param name="length">
		/// 数组长度，默认值 1；<br />
		/// AddressDataType 存在 Array 类型时，根据实际情况修改此参数；<br />
		/// 读取对应的 Array 类型长度
		/// </param>
		/// <param name="encodingType">编码类型</param>
		public AddressDetails(string sn, string addressName, string addressAnotherName, string addressPropertyName, string addressDescribe, object addressExtendParam, AddressMq addressMqParam, AddressParse addressParseParam, DataType addressDataType, AddressType addressType = AddressType.Reality, bool isEnable = true, ushort length = 1, EncodingType? encodingType = EncodingType.ANSI)
		{
			SN = sn;
			AddressName = addressName;
			Length = length;
			AddressAnotherName = addressAnotherName;
			AddressPropertyName = addressPropertyName;
			AddressDescribe = addressDescribe;
			AddressExtendParam = addressExtendParam;
			IsEnable = isEnable;
			AddressMqParam = addressMqParam;
			AddressParseParam = addressParseParam;
			AddressDataType = addressDataType;
			AddressType = addressType;
			EncodingType = encodingType ?? EncodingType.UTF8;
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="sn">唯一标识</param>
		/// <param name="addressName">
		/// 唯一；<br />
		/// 实际的点位地址名称；<br />
		/// 是扩展数据时，可为空；<br />
		/// 不是扩展数据时，此项为PLC的地址
		/// </param>
		/// <param name="addressDataType">数据类型</param>
		/// <param name="length">
		/// 数组长度，默认值 1；<br />
		/// AddressDataType 存在 Array 类型时，根据实际情况修改此参数；<br />
		/// 读取对应的 Array 类型长度
		/// </param>
		/// <param name="encodingType">编码类型</param>
		public AddressDetails(string sn, string addressName, DataType addressDataType, ushort length = 1, EncodingType? encodingType = EncodingType.ANSI)
		{
			SN = sn;
			AddressName = addressName;
			AddressDataType = addressDataType;
			Length = length;
			EncodingType = encodingType ?? EncodingType.UTF8;
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="addressName">
		/// 唯一；<br />
		/// 实际的点位地址名称；<br />
		/// 是扩展数据时，可为空；<br />
		/// 不是扩展数据时，此项为PLC的地址
		/// </param>
		/// <param name="addressDataType">数据类型</param>
		/// <param name="length">
		/// 数组长度，默认值 1；<br />
		/// AddressDataType 存在 Array 类型时，根据实际情况修改此参数；<br />
		/// 读取对应的 Array 类型长度
		/// </param>
		/// <param name="encodingType">编码类型</param>
		public AddressDetails(string addressName, DataType addressDataType, ushort length = 1, EncodingType? encodingType = EncodingType.ANSI)
		{
			AddressName = addressName;
			AddressDataType = addressDataType;
			Length = length;
			EncodingType = encodingType ?? EncodingType.UTF8;
		}

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public AddressDetails()
		{
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json 字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<AddressDetails>(this, true) ?? string.Empty;
		}

		/// <summary>
		/// 重写 Equals；<br />
		/// 使用对象比较器进行深度比较
		/// </summary>
		/// <param name="o">待比较的对象</param>
		/// <returns>是否一致</returns>
		public override bool Equals(object? o)
		{
			if (o == null)
			{
				return false;
			}
			return ExtendMethod.Comparer<AddressDetails>(this, o as AddressDetails, (string[])null).Item1;
		}

		/// <summary>
		/// 重写 GetHashCode；<br />
		/// 与 Equals 保持一致，基于 SN 生成哈希值
		/// </summary>
		/// <returns>哈希值</returns>
		public override int GetHashCode()
		{
			return SN?.GetHashCode() ?? 0;
		}
	}
	/// <summary>
	/// 地址消息中间件配置；<br />
	/// 用于配置数据采集后通过消息中间件发布数据的参数；<br />
	/// 包含主题（Topic）、内容格式（ContentFormat）、实例SN集合（ISns）
	/// </summary>
	public class AddressMq
	{
		/// <summary>
		/// 主题
		/// </summary>
		public string Topic { get; set; }

		/// <summary>
		/// 内容格式；<br />
		/// 使用：{0} 必须存在，放数据；<br />
		/// 为空：无任何格式，直接放数据
		/// </summary>
		public string? ContentFormat { get; set; }

		/// <summary>
		/// 实例SN；<br />
		/// 为空则所有打开的消息中间件进行生产
		/// </summary>
		public List<string>? ISns { get; set; }

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public AddressMq()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="contentFormat">内容格式</param>
		/// <param name="iSns">实例SN集合</param>
		public AddressMq(string topic, string contentFormat, List<string> iSns)
		{
			Topic = topic;
			ContentFormat = contentFormat;
			ISns = iSns;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<AddressMq>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 地址解析配置；<br />
	/// 每个地址只能存在一个解析配置；<br />
	/// 通过反射机制执行指定方法对采集数据进行二次解析处理；<br />
	/// ReflectionParam[0]:反射的基础数据；ReflectionParam[1]:需要执行的方法SN
	/// </summary>
	public class AddressParse
	{
		/// <summary>
		/// 反射解析；<br />
		/// object[0]：反射的基础数据；<br />
		/// object[1]：需要执行哪个方法的SN；<br />
		/// 通过反射得到解析后结果，[入参(string 地址名,string 地址值)] - [出参(string)]
		/// </summary>
		public object[]? ReflectionParam { get; set; }

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public AddressParse()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="reflectionParam">反射解析</param>
		public AddressParse(object[] reflectionParam)
		{
			ReflectionParam = reflectionParam;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<AddressParse>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 地址获取到的值；<br />
	/// 继承自 AddressDetails，扩展了数据质量、结果值、原始值、消息和时间属性；<br />
	/// 提供 SET 方法从 AddressDetails/BytesModel 设置参数；<br />
	/// 提供 GetSimplify 方法转换为轻量级传输模型
	/// </summary>
	public class AddressValue : AddressDetails
	{
		/// <summary>
		/// 数据质量；<br />
		/// -1：尚未经过处理；<br />
		/// 0：异常；<br />
		/// 1：正常；<br />
		/// 2：类型错误；<br />
		/// 3：数据经过解析，并且解析成功，无法得知数据解析后的正确性；<br />
		/// 4：解析错误；
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public QualityType Quality { get; set; } = QualityType.None;

		/// <summary>
		/// 详细信息
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 结果值；<br />
		/// 解析'后'的值，未经过解析与'OriginalValue'一致
		/// </summary>
		public object? ResultValue { get; set; }

		/// <summary>
		/// 原始值；<br />
		/// 解析'前'的值，未经过解析与'ResultValue'一致
		/// </summary>
		public object? OriginalValue { get; set; }

		/// <summary>
		/// 获取或更新的时间
		/// </summary>
		public DateTime Time { get; set; } = DateTime.Now;

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public AddressValue()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="quality">数据质量</param>
		/// <param name="resultValue">结果值</param>
		/// <param name="originalValue">原始值</param>
		/// <param name="message">详细信息</param>
		/// <param name="time">更新时间</param>
		public AddressValue(QualityType quality, object resultValue, object originalValue, string message, DateTime time)
		{
			Quality = quality;
			ResultValue = resultValue;
			OriginalValue = originalValue;
			Message = message;
			Time = time;
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="quality">数据质量</param>
		/// <param name="resultValue">结果值</param>
		/// <param name="originalValue">原始值</param>
		/// <param name="message">详细信息</param>
		/// <param name="time">更新时间</param>
		/// <param name="addressDetails">地址详情</param>
		public AddressValue(QualityType quality, object resultValue, object originalValue, string message, DateTime time, AddressDetails addressDetails)
		{
			Quality = quality;
			ResultValue = resultValue;
			OriginalValue = originalValue;
			Message = message;
			Time = time;
			SET(addressDetails);
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="quality">数据质量</param>
		/// <param name="resultValue">结果值</param>
		/// <param name="originalValue">原始值</param>
		/// <param name="message">详细信息</param>
		public AddressValue(QualityType quality, object resultValue, object originalValue, string message)
		{
			Quality = quality;
			ResultValue = resultValue;
			OriginalValue = originalValue;
			Message = message;
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="quality">数据质量</param>
		/// <param name="resultValue">结果值</param>
		/// <param name="originalValue">原始值</param>
		/// <param name="message">详细信息</param>
		/// <param name="addressDetails">地址详情</param>
		public AddressValue(QualityType quality, object resultValue, object originalValue, string message, AddressDetails addressDetails)
		{
			Quality = quality;
			ResultValue = resultValue;
			OriginalValue = originalValue;
			Message = message;
			SET(addressDetails);
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="resultValue">结果值</param>
		/// <param name="originalValue">原始值</param>
		/// <param name="message">详细信息</param>
		/// <param name="time">获取或更新的时间</param>
		/// <returns>获取数据质量</returns>
		public QualityType GetDetails(out object? resultValue, out object? originalValue, out string? message, out DateTime? time)
		{
			resultValue = ResultValue;
			originalValue = OriginalValue;
			message = Message;
			time = Time;
			return Quality;
		}

		/// <summary>
		/// 获取精简版数据；<br />
		/// 将当前 AddressValue 转换为轻量级 AddressValueSimplify 传输模型
		/// </summary>
		/// <returns>精简地址值</returns>
		public AddressValueSimplify GetSimplify()
		{
			string sN = base.SN;
			string addressName = base.AddressName;
			ushort length = base.Length;
			EncodingType? enc = base.EncodingType;
			QualityType quality = Quality;
			object? resultValue = ResultValue;
			return new AddressValueSimplify(sN, addressName, length, enc, quality, (resultValue != null) ? ExtendMethod.ToJson<object>(resultValue, false) : null, Message, Time.Ticks, base.AddressDataType);
		}

		/// <summary>
		/// 设置父类的参数
		/// </summary>
		/// <param name="addressDetails">地址详情</param>
		public AddressValue SET(AddressDetails addressDetails)
		{
			base.SN = addressDetails.SN;
			base.AddressName = addressDetails.AddressName;
			base.AddressAnotherName = addressDetails.AddressAnotherName;
			base.AddressPropertyName = addressDetails.AddressPropertyName;
			base.AddressDescribe = addressDetails.AddressDescribe;
			base.AddressParseParam = addressDetails.AddressParseParam;
			base.AddressMqParam = addressDetails.AddressMqParam;
			base.AddressExtendParam = addressDetails.AddressExtendParam;
			base.IsEnable = addressDetails.IsEnable;
			base.AddressDataType = addressDetails.AddressDataType;
			base.AddressType = addressDetails.AddressType;
			base.Length = addressDetails.Length;
			base.EncodingType = addressDetails.EncodingType;
			return this;
		}

		/// <summary>
		/// 设置参数
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="source">源</param>
		/// <param name="time">时间</param>
		/// <param name="param">参数</param>
		/// <param name="msg">消息</param>
		/// <param name="quality">质量</param>
		/// <returns>返回标准的 <see cref="T:Snet.Model.data.AddressValue" /></returns>
		public AddressValue SET(object? value, byte[] source, DateTime time, BytesModel param, string? msg = null, QualityType quality = QualityType.Normal)
		{
			base.AddressName = param.Address;
			base.AddressDataType = param.DataType;
			base.AddressDescribe = param.Describe;
			base.AddressType = AddressType.Reality;
			base.EncodingType = param.EncodingType;
			Time = time;
			OriginalValue = source;
			ResultValue = value;
			Message = msg;
			Quality = quality;
			return this;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<AddressValue>(this, true) ?? string.Empty;
		}

		/// <summary>
		/// 重写 Equals；<br />
		/// 使用对象比较器进行深度比较，忽略 Time 属性
		/// </summary>
		/// <param name="o">待比较的对象</param>
		/// <returns>是否一致</returns>
		public override bool Equals(object? o)
		{
			if (o == null)
			{
				return false;
			}
			return ExtendMethod.Comparer<AddressValue>(this, o as AddressValue, new string[1] { "Time" }).Item1;
		}

		/// <summary>
		/// 重写 GetHashCode；<br />
		/// 与 Equals 保持一致，基于 SN 生成哈希值
		/// </summary>
		/// <returns>哈希值</returns>
		public override int GetHashCode()
		{
			return base.SN?.GetHashCode() ?? 0;
		}
	}
	/// <summary>
	/// 精简地址值；<br />
	/// 用于数据传输的轻量化地址值模型，支持 Protobuf 序列化；<br />
	/// 包含 SN、点位地址、数据长度、编码类型、质量、采集数值、详细信息、时间戳、数据类型
	/// </summary>
	[ProtoContract]
	public class AddressValueSimplify
	{
		/// <summary>
		/// 唯一标识符
		/// </summary>
		[ProtoMember(1)]
		public string? SN { get; set; }

		/// <summary>
		/// 点位地址
		/// </summary>
		[ProtoMember(2)]
		public string? Add { get; set; }

		/// <summary>
		/// 数据长度
		/// </summary>
		[ProtoMember(3)]
		public ushort L { get; set; } = 1;

		/// <summary>
		/// 编码类型
		/// </summary>
		[ProtoMember(4)]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public EncodingType ENC { get; set; }

		/// <summary>
		/// 数据质量；<br />
		/// -1：尚未经过处理；<br />
		/// 0：异常；<br />
		/// 1：正常；<br />
		/// 2：类型错误；<br />
		/// 3：数据经过解析，并且解析成功，无法得知数据解析后的正确性；<br />
		/// 4：解析错误；
		/// </summary>
		[ProtoMember(5)]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public QualityType Q { get; set; } = QualityType.None;

		/// <summary>
		/// 详细信息
		/// </summary>
		[ProtoMember(6)]
		public string? Msg { get; set; }

		/// <summary>
		/// 采集数值
		/// </summary>
		[ProtoMember(7)]
		public string? VAL { get; set; }

		/// <summary>
		/// 采集时间
		/// </summary>
		[ProtoMember(8)]
		public long Ts { get; set; } = DateTime.UtcNow.Ticks;

		/// <summary>
		/// 数据类型
		/// </summary>
		[ProtoMember(9)]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DataType DT { get; set; } = DataType.String;

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public AddressValueSimplify()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="sn">唯一标识符</param>
		/// <param name="add">点位地址</param>
		/// <param name="l">数据长度</param>
		/// <param name="enc">编码类型</param>
		/// <param name="q">数据质量</param>
		/// <param name="val">采集数值</param>
		/// <param name="msg">详细信息</param>
		/// <param name="ts">采集时间</param>
		/// <param name="dt">数据类型</param>
		public AddressValueSimplify(string? sn, string? add, ushort l, EncodingType? enc, QualityType q, string? val, string? msg, long ts, DataType dt)
		{
			SN = sn;
			Add = add;
			L = l;
			Q = q;
			VAL = val;
			Msg = msg;
			Ts = ts;
			DT = dt;
			ENC = enc.GetValueOrDefault();
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<AddressValueSimplify>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 精简地址 Protobuf 专用类；<br />
	/// 用于通过 protobuf-net 二进制序列化批量传输精简地址值集合；<br />
	/// 包含时间戳和 AddressValueSimplify 集合
	/// </summary>
	[ProtoContract]
	public class AddressValueSimplifyProtobuf
	{
		/// <summary>
		/// 时间
		/// </summary>
		[ProtoMember(1)]
		public long Ts { get; set; } = DateTime.UtcNow.Ticks;

		/// <summary>
		/// 精简地址集合；<br />
		/// 包含多个精简地址值的可迭代集合
		/// </summary>
		[ProtoMember(2)]
		public IEnumerable<AddressValueSimplify> ITM { get; set; } = Array.Empty<AddressValueSimplify>();

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public AddressValueSimplifyProtobuf()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="simplifys">精简地址集合</param>
		public AddressValueSimplifyProtobuf(IEnumerable<AddressValueSimplify> simplifys)
		{
			ITM = simplifys;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<AddressValueSimplifyProtobuf>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 基础模型；<br />
	/// 所有结果模型的基类，继承自 AsyncEventArgs；<br />
	/// 包含状态（Status）、描述信息（Message）和响应时间（Time）；<br />
	/// 提供 GetDetails 方法解构结果，重写 ToString 输出 JSON 字符串
	/// </summary>
	public class BaseModel : EventArgsAsync
	{
		/// <summary>
		/// 描述信息
		/// </summary>
		public string? Message { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public bool Status { get; set; }

		/// <summary>
		/// 响应时间
		/// </summary>
		public DateTime Time { get; set; } = DateTime.Now;

		/// <summary>
		/// 基础模型；<br />
		/// 无参构造函数
		/// </summary>
		public BaseModel()
		{
		}

		/// <summary>
		/// 基础模型；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="result">事件信息结果</param>
		public BaseModel(BaseModel result)
		{
			Message = result.Message;
			Status = result.Status;
			Time = result.Time;
		}

		/// <summary>
		/// 基础模型；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		public BaseModel(bool status, string message)
		{
			Status = status;
			Message = message;
		}

		/// <summary>
		/// 基础模型；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="time">响应时间</param>
		public BaseModel(bool status, string message, DateTime time)
		{
			Status = status;
			Message = message;
			Time = time;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="message">抛出描述信息</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out string? message)
		{
			message = Message;
			return Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="result">抛出结果对象</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out BaseModel result)
		{
			result = this;
			return Status;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json 字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<BaseModel>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 字节模型；<br />
	/// 用于字节数组与结构化数据的映射配置；<br />
	/// 包含地址名称、描述、起始位、长度、数据类型、编码类型和字节序格式；<br />
	/// 同时支持 System.Text.Json 和 Newtonsoft.Json 枚举字符串序列化
	/// </summary>
	public class BytesModel
	{
		/// <summary>
		/// 地址名称
		/// </summary>
		[Description("地址名称")]
		public string Address { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		[Description("描述")]
		public string Describe { get; set; }

		/// <summary>
		/// 起始位
		/// </summary>
		[Description("起始位")]
		public int StartBit { get; set; }

		/// <summary>
		/// 长度
		/// </summary>
		[Description("长度")]
		public ushort Length { get; set; } = 1;

		/// <summary>
		/// 布尔位索引<br />
		/// 一个字节有8位，默认0
		/// </summary>
		[Description("布尔位索引")]
		public int BoolIndex { get; set; }

		/// <summary>
		/// 数据类型；<br />
		/// 指定该地址的数据读取类型（Bool/Int16/Float 等）
		/// </summary>
		[Description("数据类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[JsonConverter(typeof(StringEnumConverter))]
		public DataType DataType { get; set; } = DataType.Bool;

		/// <summary>
		/// 编码类型；<br />
		/// 指定字符串数据的编码方式（UTF8/ANSI/Unicode 等）
		/// </summary>
		[Description("编码类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[JsonConverter(typeof(StringEnumConverter))]
		public EncodingType EncodingType { get; set; } = EncodingType.UTF8;

		/// <summary>
		/// 数据格式；<br />
		/// 指定字节序格式（ABCD/DCBA/BADC/CDAB），影响多字节数据的解析方式
		/// </summary>
		[Description("数据格式")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[JsonConverter(typeof(StringEnumConverter))]
		public DataFormat DataFormat { get; set; } = DataFormat.DCBA;

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public BytesModel()
		{
		}

		/// <summary>
		/// 全属性可选构造函数
		/// </summary>
		/// <param name="address">地址名称</param>
		/// <param name="describe">描述</param>
		/// <param name="startBit">起始位</param>
		/// <param name="length">长度</param>
		/// <param name="dataType">数据类型</param>
		/// <param name="encodingType">编码类型</param>
		/// <param name="dataFormat">数据格式</param>
		/// <param name="boolIndex">布尔位索引，-1表示非布尔位</param>
		public BytesModel(string address, string describe, int startBit, ushort length, DataType dataType, EncodingType encodingType = EncodingType.UTF8, DataFormat dataFormat = DataFormat.ABCD, int boolIndex = -1)
		{
			Address = address;
			Describe = describe;
			StartBit = startBit;
			Length = length;
			DataType = dataType;
			BoolIndex = boolIndex;
			EncodingType = encodingType;
			DataFormat = dataFormat;
		}
	}
	/// <summary>
	/// 自定义异常类；<br />
	/// 继承自 Exception，自动捕获文件路径、方法名称和行号信息；<br />
	/// 通过 CallerFilePath/CallerMemberName/CallerLineNumber 特性实现自动定位；<br />
	/// 提供 Create/CreateAsync 工厂方法快速创建异常实例
	/// </summary>
	public class CustomException : Exception
	{
		/// <summary>
		/// 自定义异常构造函数
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名称</param>
		/// <param name="lineNumber">异常行数</param>
		public CustomException(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
			: base($"file {Path.GetFileName(filePath)} the {methodName} method {lineNumber} line exception: {message}")
		{
		}

		/// <summary>
		/// 自定义异常构造函数
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="innerException">底层异常对象</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名称</param>
		/// <param name="lineNumber">异常行数</param>
		public CustomException(string message, Exception innerException, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
			: base($"file {Path.GetFileName(filePath)} the {methodName} method {lineNumber} line exception: {message}", innerException)
		{
		}

		/// <summary>
		/// 创建自定义异常
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名称</param>
		/// <param name="lineNumber">异常行数</param>
		/// <returns>返回快速创建的对象</returns>
		public static CustomException Create(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			return new CustomException(message, filePath, methodName, lineNumber);
		}

		/// <summary>
		/// 异步创建自定义异常
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="token">传播消息取消通知</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名称</param>
		/// <param name="lineNumber">异常行数</param>
		/// <returns>返回快速创建的对象</returns>
		public static Task<CustomException> CreateAsync(string message, CancellationToken token = default(CancellationToken), [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			return Task.Run(() => Create(message, filePath, methodName, lineNumber), token);
		}

		/// <summary>
		/// 创建自定义异常；<br />
		/// 通过异常信息和底层异常对象快速构建自定义异常实例
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="innerException">底层异常对象</param>
		/// <param name="filePath">文件路径（编译器自动填充）</param>
		/// <param name="methodName">方法名称（编译器自动填充）</param>
		/// <param name="lineNumber">异常行数（编译器自动填充）</param>
		/// <returns>返回自定义异常的对象</returns>
		public static CustomException Create(string message, Exception innerException, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			return new CustomException(message, innerException, filePath, methodName, lineNumber);
		}

		/// <summary>
		/// 异步创建自定义异常；<br />
		/// 通过异常信息和底层异常对象在后台线程中构建自定义异常实例
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="innerException">底层异常对象</param>
		/// <param name="token">传播消息取消通知</param>
		/// <param name="filePath">文件路径（编译器自动填充）</param>
		/// <param name="methodName">方法名称（编译器自动填充）</param>
		/// <param name="lineNumber">异常行数（编译器自动填充）</param>
		/// <returns>返回自定义异常的对象</returns>
		public static Task<CustomException> CreateAsync(string message, Exception innerException, CancellationToken token = default(CancellationToken), [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			return Task.Run(() => Create(message, innerException, filePath, methodName, lineNumber), token);
		}
	}
	/// <summary>
	/// 事件数据结果；<br />
	/// 继承自 ResultModel，用于通过 IEvent.OnDataEvent 事件传递数据采集结果；<br />
	/// 包含状态、消息和结果数据；<br />
	/// 提供 CreateSuccessResult/CreateFailureResult 工厂方法
	/// </summary>
	public class EventDataResult : ResultModel
	{
		/// <summary>
		/// 事件数据结果；<br />
		/// 无参构造函数
		/// </summary>
		public EventDataResult()
		{
		}

		/// <summary>
		/// 事件数据结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="result">事件数据结果</param>
		public EventDataResult(EventDataResult result)
			: base(result.Status, result.Message, result.ResultData)
		{
		}

		/// <summary>
		/// 事件数据结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="resultData">结果数据</param>
		public EventDataResult(bool status, string message, object? resultData = null)
			: base(status, message, resultData)
		{
		}

		/// <summary>
		/// 快速创建一个成功的结果
		/// </summary>
		/// <param name="successMessage">成功消息</param>
		/// <returns>结果对象</returns>
		public static EventDataResult CreateSuccessResult(string successMessage)
		{
			return new EventDataResult(status: true, successMessage);
		}

		/// <summary>
		/// 快速创建一个成功的结果
		/// </summary>
		/// <param name="successMessage">成功消息</param>
		/// <param name="resultData">结果数据</param>
		/// <returns>结果对象</returns>
		public static EventDataResult CreateSuccessResult<T>(string successMessage, T resultData)
		{
			return new EventDataResult(status: true, successMessage, resultData);
		}

		/// <summary>
		/// 快速创建一个失败的结果
		/// </summary>
		/// <param name="failureMessage">失败的消息</param>
		/// <returns>结果对象</returns>
		public static EventDataResult CreateFailureResult(string failureMessage)
		{
			return new EventDataResult(status: false, failureMessage);
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="result">抛出结果对象</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out EventDataResult result)
		{
			result = this;
			return base.Status;
		}
	}
	/// <summary>
	/// 事件信息结果；<br />
	/// 继承自 BaseModel，用于通过 IEvent.OnInfoEvent 事件传递通信状态信息；<br />
	/// 包含状态和消息；<br />
	/// 提供 CreateSuccessResult/CreateFailureResult 工厂方法
	/// </summary>
	public class EventInfoResult : BaseModel
	{
		/// <summary>
		/// 事件信息结果；<br />
		/// 无参构造函数
		/// </summary>
		public EventInfoResult()
		{
		}

		/// <summary>
		/// 事件信息结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="result">事件信息结果</param>
		public EventInfoResult(EventInfoResult result)
			: base(result.Status, result.Message, result.Time)
		{
		}

		/// <summary>
		/// 事件信息结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		public EventInfoResult(bool status, string message)
			: base(status, message)
		{
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="result">抛出结果对象</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out EventInfoResult result)
		{
			result = this;
			return base.Status;
		}

		/// <summary>
		/// 快速创建一个成功的结果
		/// </summary>
		/// <param name="successMessage">成功消息</param>
		/// <returns>结果对象</returns>
		public static EventInfoResult CreateSuccessResult(string successMessage)
		{
			return new EventInfoResult(status: true, successMessage);
		}

		/// <summary>
		/// 快速创建一个失败的结果
		/// </summary>
		/// <param name="failureMessage">失败的消息</param>
		/// <returns>结果对象</returns>
		public static EventInfoResult CreateFailureResult(string failureMessage)
		{
			return new EventInfoResult(status: false, failureMessage);
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json 字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<EventInfoResult>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 事件语言结果；<br />
	/// 继承自 AsyncEventArgs，用于通过 IEvent.OnLanguageEvent 事件通知语言变更；<br />
	/// 包含状态、消息、语言类型和时间；<br />
	/// 提供 CreateSuccessResult/CreateFailureResult 工厂方法
	/// </summary>
	public class EventLanguageResult : EventArgsAsync
	{
		/// <summary>
		/// 状态
		/// </summary>
		public bool Status { get; set; }

		/// <summary>
		/// 信息
		/// </summary>
		public string? Message { get; set; }

		/// <summary>
		/// 语言类型
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public LanguageType? Language { get; set; }

		/// <summary>
		/// 时间
		/// </summary>
		public DateTime Time { get; set; } = DateTime.Now;

		/// <summary>
		/// 事件语言结果；<br />
		/// 无参构造函数
		/// </summary>
		public EventLanguageResult()
		{
		}

		/// <summary>
		/// 事件语言结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="result">事件信息结果</param>
		public EventLanguageResult(EventLanguageResult result)
		{
			Status = result.Status;
			Message = result.Message;
			Language = result.Language;
			Time = result.Time;
		}

		/// <summary>
		/// 事件语言结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="language">语言类型</param>
		public EventLanguageResult(bool status, string message, LanguageType? language = null)
		{
			Status = status;
			Language = language;
			Message = message;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="message">抛出描述信息</param>
		/// <param name="language">语言类型</param>
		/// <returns>状态</returns>
		public bool GetDetails(out string? message, out LanguageType? language)
		{
			language = Language;
			message = Message;
			return Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="language">语言</param>
		/// <returns>状态</returns>
		public bool GetDetails(out LanguageType? language)
		{
			language = Language;
			return Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="message">抛出描述信息</param>
		/// <returns>状态</returns>
		public bool GetDetails(out string? message)
		{
			message = Message;
			return Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="result">抛出结果对象</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out EventLanguageResult result)
		{
			result = this;
			return Status;
		}

		/// <summary>
		/// 快速创建一个成功的结果
		/// </summary>
		/// <param name="successMessage">成功消息</param>
		/// <returns>结果对象</returns>
		public static EventLanguageResult CreateSuccessResult(string successMessage)
		{
			return new EventLanguageResult(status: true, successMessage);
		}

		/// <summary>
		/// 快速创建一个失败的结果
		/// </summary>
		/// <param name="failureMessage">失败的消息</param>
		/// <returns>结果对象</returns>
		public static EventLanguageResult CreateFailureResult(string failureMessage)
		{
			return new EventLanguageResult(status: false, failureMessage);
		}

		/// <summary>
		/// 快速创建一个成功的结果
		/// </summary>
		/// <param name="successMessage">成功消息</param>
		/// <param name="language">语言类型</param>
		/// <returns>结果对象</returns>
		public static EventLanguageResult CreateSuccessResult(string successMessage, LanguageType? language)
		{
			return new EventLanguageResult(status: true, successMessage, language);
		}

		/// <summary>
		/// 快速创建一个失败的结果
		/// </summary>
		/// <param name="failureMessage">失败的消息</param>
		/// <param name="language">语言类型</param>
		/// <returns>结果对象</returns>
		public static EventLanguageResult CreateFailureResult(string failureMessage, LanguageType? language)
		{
			return new EventLanguageResult(status: false, failureMessage, language);
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json 字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<EventLanguageResult>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 语言模型；<br />
	/// 用于配置多语言资源的指向信息；<br />
	/// 包含资源字典所在的解决方案名称、资源文件名称和程序集文件路径
	/// </summary>
	public class LanguageModel
	{
		/// <summary>
		/// 存放资源字典的解决方按名称
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// 资源文件名称
		/// </summary>
		public string Dictionary { get; set; }

		/// <summary>
		/// 包含程序集清单的文件的名称或路径，用于获取程序集
		/// </summary>
		public string AssemblyFile { get; set; }

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public LanguageModel()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="source">存放资源字典的解决方按名称</param>
		/// <param name="dictionary">资源文件名称</param>
		/// <param name="assemblyFile">包含程序集清单的文件的名称或路径，用于获取程序集</param>
		public LanguageModel(string source, string dictionary, string assemblyFile)
		{
			Source = source;
			Dictionary = dictionary;
			AssemblyFile = assemblyFile;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<LanguageModel>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 操作结果；<br />
	/// 统一的操作返回模型，继承自 ResultModel；<br />
	/// 包含状态、消息、结果数据和运行时间（ms）；<br />
	/// 提供 CreateSuccessResult/CreateFailureResult 工厂方法快速创建结果对象
	/// </summary>
	public class OperateResult : ResultModel
	{
		/// <summary>
		/// 此函数运行时间 ( ms )
		/// </summary>
		public int RunTime { get; set; }

		/// <summary>
		/// 操作结果；<br />
		/// 无参构造函数
		/// </summary>
		public OperateResult()
		{
		}

		/// <summary>
		/// 操作结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="result">操作结果</param>
		public OperateResult(OperateResult result)
		{
			base.Status = result.Status;
			base.Time = result.Time;
			RunTime = result.RunTime;
			base.ResultData = result.ResultData;
			base.Message = result.Message;
		}

		/// <summary>
		/// 操作结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="result">操作结果</param>
		/// <param name="rumTime">此函数运行时间</param>
		public OperateResult(OperateResult result, int rumTime)
		{
			base.Status = result.Status;
			base.Time = result.Time;
			RunTime = rumTime;
			base.ResultData = result.ResultData;
			base.Message = result.Message;
		}

		/// <summary>
		/// 操作结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="runTime">此函数运行时间</param>
		/// <param name="resultData">结果数据</param>
		public OperateResult(bool status, string message, int runTime, object? resultData = null)
			: base(status, message, resultData)
		{
			RunTime = runTime;
		}

		/// <summary>
		/// 快速创建一个成功的结果
		/// </summary>
		/// <param name="successMessage">成功消息</param>
		/// <param name="runTime">运行时间</param>
		/// <returns>结果对象</returns>
		public static OperateResult CreateSuccessResult(string successMessage, int? runTime = null)
		{
			return new OperateResult(status: true, successMessage, runTime ?? 8);
		}

		/// <summary>
		/// 快速创建一个成功的结果
		/// </summary>
		/// <param name="successMessage">成功消息</param>
		/// <param name="resultData">结果数据</param>
		/// <param name="runTime">运行时间</param>
		/// <returns>结果对象</returns>
		public static OperateResult CreateSuccessResult<T>(string successMessage, T resultData, int? runTime = null)
		{
			return new OperateResult(status: true, successMessage, runTime ?? 8, resultData);
		}

		/// <summary>
		/// 快速创建一个失败的结果
		/// </summary>
		/// <param name="failureMessage">失败的消息</param>
		/// <param name="runTime">运行时间</param>
		/// <returns>结果对象</returns>
		public static OperateResult CreateFailureResult(string failureMessage, int? runTime = null)
		{
			return new OperateResult(status: false, failureMessage, runTime ?? 8);
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="result">抛出结果对象</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out OperateResult result)
		{
			result = this;
			return base.Status;
		}
	}
	/// <summary>
	/// 库文件参数模型；<br />
	/// 用于通过反射机制自动生成前端 UI 参数配置界面；<br />
	/// 包含名称、描述和参数子集（subset）；<br />
	/// 内嵌 dataCate 枚举定义前端控件类型，内嵌 propertie/options 类定义属性细节
	/// </summary>
	public class ParamModel
	{
		/// <summary>
		/// 前端使用的数据类别
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum dataCate
		{
			/// <summary>
			/// 文本框
			/// </summary>
			[Description("文本框")]
			text,
			/// <summary>
			/// 下拉框
			/// </summary>
			[Description("下拉框")]
			select,
			/// <summary>
			/// 单选框
			/// </summary>
			[Description("单选框")]
			radio,
			/// <summary>
			/// 整数框
			/// </summary>
			[Description("整数框")]
			unmber,
			/// <summary>
			/// 上传
			/// </summary>
			[Description("上传")]
			upload
		}

		/// <summary>
		/// 参数信息
		/// </summary>
		public class subset
		{
			/// <summary>
			/// 名称
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 描述
			/// </summary>
			public string Description { get; set; }

			/// <summary>
			/// 属性集合
			/// </summary>
			public List<propertie> Propertie { get; set; }
		}

		/// <summary>
		/// 属性
		/// </summary>
		public class propertie
		{
			/// <summary>
			/// 数据类型
			/// </summary>
			[JsonConverter(typeof(JsonStringEnumConverter))]
			public dataCate? DataCate { get; set; }

			/// <summary>
			/// 默认值
			/// </summary>
			public object? Default { get; set; }

			/// <summary>
			/// 初始值
			/// </summary>
			public object? Initial { get; set; }

			/// <summary>
			/// 描述
			/// </summary>
			public string? Description { get; set; }

			/// <summary>
			/// 属性名称
			/// </summary>
			public string? PropertyName { get; set; }

			/// <summary>
			/// 是否显示，<br />
			/// true:展示<br />
			/// false:隐藏
			/// </summary>
			public bool Show { get; set; }

			/// <summary>
			/// 是否使用，<br />
			/// false:不让输入<br />
			/// true:正常输入
			/// </summary>
			public bool Use { get; set; }

			/// <summary>
			/// 必须填写
			/// </summary>
			public bool MustFillIn { get; set; }

			/// <summary>
			/// 详细提示
			/// </summary>
			public string? DetailsTips { get; set; }

			/// <summary>
			/// 正则表达式
			/// </summary>
			public string? Pattern { get; set; }

			/// <summary>
			/// 输入错误提示
			/// </summary>
			public string? FailTips { get; set; }

			/// <summary>
			/// 非必填项集合
			/// </summary>
			public List<options>? Options { get; set; }
		}

		/// <summary>
		/// 非必填项
		/// </summary>
		public class options
		{
			/// <summary>
			/// 键
			/// </summary>
			public string Key { get; set; }

			/// <summary>
			/// 值
			/// </summary>
			public object Value { get; set; }
		}

		/// <summary>
		/// 参数信息 精简版
		/// </summary>
		public class subsetSimplify
		{
			/// <summary>
			/// 名称
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 描述
			/// </summary>
			public string Description { get; set; }

			/// <summary>
			/// 属性集合精简版
			/// </summary>
			public List<propertieSimplify> Propertie { get; set; }
		}

		/// <summary>
		/// 属性 精简版
		/// </summary>
		public class propertieSimplify
		{
			/// <summary>
			/// 描述
			/// </summary>
			public string? Description { get; set; }

			/// <summary>
			/// 属性名称
			/// </summary>
			public string? PropertyName { get; set; }
		}

		/// <summary>
		/// 代码
		/// </summary>
		public string Code => Name;

		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 参数集合
		/// </summary>
		public List<subset> Subset { get; set; }
	}
	/// <summary>
	/// 插件详情模型
	/// </summary>
	public class PluginModel
	{
		/// <summary>
		/// 名称
		/// </summary>
		[Description("名称")]
		public string Name { get; set; }

		/// <summary>
		/// 命名空间
		/// </summary>
		[Description("命名空间")]
		public string Namespace { get; set; }

		/// <summary>
		/// 配置文件格式模板
		/// </summary>
		[Description("配置文件格式模板")]
		public string ConfigFormat { get; set; }

		/// <summary>
		/// 插件的路径
		/// </summary>
		[Description("插件的路径")]
		public string Path { get; set; }

		/// <summary>
		/// 插件类型
		/// </summary>
		[Description("插件类型")]
		public PluginType Type { get; set; }

		/// <summary>
		/// 版本
		/// </summary>
		[Description("版本")]
		public string Version { get; set; }

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public PluginModel()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="name">插件名称</param>
		/// <param name="namespace">插件命名空间</param>
		/// <param name="configFormat">配置文件格式模板</param>
		/// <param name="path">插件所在目录路径</param>
		/// <param name="version">插件版本</param>
		/// <param name="type">插件类型</param>
		public PluginModel(string name, string @namespace, string configFormat, string path, string version, PluginType type)
		{
			Name = name;
			Namespace = @namespace;
			ConfigFormat = configFormat;
			Version = version;
			Type = type;
			Path = path;
		}
	}
	/// <summary>
	/// 消息中间件响应模型；<br />
	/// 仅支持 ResponseType.ContentWithTopic 主题与内容同时存在时使用；<br />
	/// 包含主题（Topic）和内容（Content）属性
	/// </summary>
	public class ResponseModel
	{
		/// <summary>
		/// 主题
		/// </summary>
		[Description("主题")]
		public string Topic { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		[Description("内容")]
		public object Content { get; set; }

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public ResponseModel()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="topic">主题</param>
		/// <param name="content">内容</param>
		public ResponseModel(string topic, object content)
		{
			Topic = topic;
			Content = content;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json 字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<ResponseModel>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 结果模型；<br />
	/// 继承自 BaseModel，扩展了 ResultData 结果数据属性；<br />
	/// 提供多种 GetDetails 重载方法解构状态、消息和结果数据；<br />
	/// 支持泛型 GetSource 获取指定类型的数据源
	/// </summary>
	public class ResultModel : BaseModel
	{
		/// <summary>
		/// 结果数据
		/// </summary>
		public object? ResultData { get; set; }

		/// <summary>
		/// 结果；<br />
		/// 无参构造函数
		/// </summary>
		public ResultModel()
		{
		}

		/// <summary>
		/// 结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="resultData">结果数据</param>
		public ResultModel(bool status, string? message, object? resultData = null)
			: base(status, message)
		{
			ResultData = resultData;
		}

		/// <summary>
		/// 结果；<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="time">响应时间</param>
		/// <param name="resultData">结果数据</param>
		public ResultModel(bool status, string? message, DateTime time, object? resultData = null)
			: base(status, message, time)
		{
			ResultData = resultData;
		}

		/// <summary>
		/// 获取结果数据源
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <returns>指定类型的数据</returns>
		public T? GetSource<T>()
		{
			if (ResultData != null)
			{
				return ExtendMethod.GetSource<T>(ResultData);
			}
			return default(T);
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="message">抛出描述信息</param>
		/// <returns>获取状态</returns>
		public new bool GetDetails(out string? message)
		{
			message = base.Message;
			return base.Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="resultData">抛出结果数据</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out object? resultData)
		{
			resultData = ResultData;
			return base.Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="resultData">抛出结果数据源</param>
		/// <returns>获取状态</returns>
		public bool GetDetails<T>(out T? resultData)
		{
			if (ResultData != null)
			{
				resultData = ExtendMethod.GetSource<T>(ResultData);
			}
			else
			{
				resultData = default(T);
			}
			return base.Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="resultData">抛出结果数据</param>
		/// <param name="message">抛出描述信息</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out object? resultData, out string? message)
		{
			message = base.Message;
			resultData = ResultData;
			return base.Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="resultData">抛出结果数据源</param>
		/// <param name="message">抛出描述信息</param>
		/// <returns>获取状态</returns>
		public bool GetDetails<T>(out T? resultData, out string? message)
		{
			if (ResultData != null)
			{
				resultData = ExtendMethod.GetSource<T>(ResultData);
			}
			else
			{
				resultData = default(T);
			}
			message = base.Message;
			return base.Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <param name="resultData">抛出结果数据</param>
		/// <param name="message">抛出描述信息</param>
		/// <returns>获取状态</returns>
		public bool GetDetails(out string? message, out object? resultData)
		{
			message = base.Message;
			resultData = ResultData;
			return base.Status;
		}

		/// <summary>
		/// 获取详情
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="resultData">抛出结果数据源</param>
		/// <param name="message">抛出描述信息</param>
		/// <returns>获取状态</returns>
		public bool GetDetails<T>(out string? message, out T? resultData)
		{
			if (ResultData != null)
			{
				resultData = ExtendMethod.GetSource<T>(ResultData);
			}
			else
			{
				resultData = default(T);
			}
			message = base.Message;
			return base.Status;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json 字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<ResultModel>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// WebApi 配置模型；<br />
	/// 用于配置数采器内置 WebApi 服务的 IP 地址、端口和跨域策略；<br />
	/// 默认监听 127.0.0.1:6688
	/// </summary>
	public class WAModel
	{
		/// <summary>
		/// Ip地址
		/// </summary>
		[Description("Ip地址")]
		public string IpAddress { get; set; } = "127.0.0.1";

		/// <summary>
		/// 端口
		/// </summary>
		[Description("端口")]
		public int Port { get; set; } = 6688;

		/// <summary>
		/// 跨域
		/// </summary>
		[Description("跨域")]
		public bool CrossDomain { get; set; }

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public WAModel()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="ipAddress">IP地址</param>
		/// <param name="port">端口</param>
		/// <param name="crossDomain">跨域</param>
		public WAModel(string ipAddress, int port, bool crossDomain = false)
		{
			IpAddress = ipAddress;
			Port = port;
			CrossDomain = crossDomain;
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<WAModel>(this, true) ?? string.Empty;
		}
	}
	/// <summary>
	/// 写入模型；<br />
	/// 定义向设备地址写入数据时的参数，包含值、数据类型和编码类型；<br />
	/// 配合 IWrite 接口使用
	/// </summary>
	public class WriteModel
	{
		/// <summary>
		/// 值(不能为空)
		/// </summary>
		[Description("值(不能为空)")]
		public object Value { get; set; }

		/// <summary>
		/// 编码类型
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Description("编码类型")]
		public EncodingType EncodingType { get; set; }

		/// <summary>
		/// 数据类型
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		[Description("数据类型")]
		public DataType AddressDataType { get; set; } = DataType.String;

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public WriteModel()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="addressDataType">数据类型</param>
		/// <param name="encodingType">编码类型</param>
		public WriteModel(object value, DataType addressDataType, EncodingType? encodingType = EncodingType.ANSI)
		{
			Value = value;
			AddressDataType = addressDataType;
			EncodingType = encodingType.GetValueOrDefault();
		}

		/// <summary>
		/// 重写ToString；<br />
		/// 响应 json 字符串
		/// </summary>
		/// <returns>json字符串</returns>
		public override string ToString()
		{
			return ExtendMethod.ToJson<WriteModel>(this, true) ?? string.Empty;
		}
	}
}
namespace Snet.Model.attribute
{
	/// <summary>
	/// 自动分割属性特性；<br />
	/// 用于协议类型枚举中，快速检索出对应的属性列表；<br />
	/// 必须配合 AutoAllocatingTagAttribute 使用；<br />
	/// 通过属性名称数组指定当前枚举值对应的参数属性集合
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class AutoAllocatingAttribute : Attribute
	{
		/// <summary>
		/// 属性名称集合
		/// </summary>
		public string[] PropertyNameArray { get; set; }

		/// <summary>
		/// 下标集合特性；<br />
		/// 用于协议类型枚举中使用；<br />
		/// 快速检索出对应的属性列表；<br />
		/// 配合　AutoAllocatingTagAttribute　使用
		/// </summary>
		/// <param name="propertyNameArray">属性名称</param>
		public AutoAllocatingAttribute(string[] propertyNameArray)
		{
			PropertyNameArray = propertyNameArray;
		}
	}
	/// <summary>
	/// 自动分配标识特性；<br />
	/// 标识此属性是用于自动分配属性的枚举类型属性；<br />
	/// 在当前类中只能出现一个该标识；<br />
	/// 必须配合 AutoAllocatingAttribute 使用，指定枚举类型用于参数分配检索
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class AutoAllocatingTagAttribute : Attribute
	{
		/// <summary>
		/// 枚举的类型
		/// </summary>
		public Type EnumType { get; set; }

		/// <summary>
		/// 自动分配标识特性，标识此属性是用于自动分配属性使用；<br />
		/// 在当前类中只能出现一个这样的标识；<br />
		/// 用于协议类型枚举中使用；<br />
		/// 快速检索出对应的属性列表；<br />
		/// 配合　AutoAllocatingAttribute　使用
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		public AutoAllocatingTagAttribute(Type enumType)
		{
			EnumType = enumType;
		}
	}
	/// <summary>
	/// 字节结构特性；<br />
	/// 用于字节数组与结构体的映射；<br />
	/// 配置字节下标（Index）、数据长度（Length）和数据类型（DataType）；<br />
	/// 提供 Copy 方法从源字节数组中拷贝指定区间的数据
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class ByteStructureAttribute : Attribute
	{
		/// <summary>
		/// 下标<br />
		/// 默认从零开始
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// 长度
		/// </summary>
		public int Length { get; private set; } = 1;

		/// <summary>
		/// 数据类型
		/// </summary>
		public DataType DataType { get; private set; } = DataType.Bool;

		/// <summary>
		/// 字节结构特性<br />
		/// 用于字节数组映射结构体使用
		/// </summary>
		/// <param name="index">下标</param>
		/// <param name="length">长度</param>
		/// <param name="dataType">数据类型</param>
		public ByteStructureAttribute(int index, int length, DataType dataType)
		{
			Index = index;
			Length = length;
			DataType = dataType;
		}

		/// <summary>
		/// 字节结构特性<br />
		/// 用于字节数组映射结构体使用
		/// </summary>
		/// <param name="index">下标</param>
		/// <param name="dataType">数据类型</param>
		public ByteStructureAttribute(int index, DataType dataType)
		{
			Index = index;
			DataType = dataType;
		}

		/// <summary>
		/// 通过参数拷贝；<br />
		/// 从源字节数组中按 Index 和 Length 拷贝指定区间的数据；<br />
		/// 包含边界检查，防止数组越界异常
		/// </summary>
		/// <param name="source">字节源</param>
		/// <param name="offset">偏移量，偏移点为空值</param>
		/// <param name="end">末尾预留空偏移量</param>
		/// <returns>拷贝后的字节数组</returns>
		/// <exception cref="T:System.ArgumentException">源数组长度不足时抛出</exception>
		public byte[] Copy(byte[] source, int offset = 0, bool end = false)
		{
			if (Index + Length > source.Length)
			{
				throw new ArgumentException($"源数组长度不足：需要从索引 {Index} 读取 {Length} 字节，但源数组长度仅为 {source.Length}", "source");
			}
			byte[] array = new byte[Length + offset];
			Array.Copy(source, Index, array, (!end) ? offset : 0, Length);
			return array;
		}
	}
	/// <summary>
	/// 展示特性；<br />
	/// 用于标记属性在前端 UI 中的展示行为；<br />
	/// 配置是否显示、是否可输入、是否必填、数据类别和详细提示；<br />
	/// 配合 ParamModel 和反射机制自动生成参数配置界面
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class DisplayAttribute : Attribute
	{
		/// <summary>
		/// 是否显示；<br />
		/// true:展示；<br />
		/// false:隐藏
		/// </summary>
		public bool Show { get; set; }

		/// <summary>
		/// 是否使用；<br />
		/// false:不让输入<br />
		/// true:正常输入
		/// </summary>
		public bool Use { get; set; }

		/// <summary>
		/// 必须填写
		/// </summary>
		public bool MustFillIn { get; set; }

		/// <summary>
		/// 详细提示
		/// </summary>
		public string DetailsTips { get; set; }

		/// <summary>
		/// 数据类型
		/// </summary>
		public ParamModel.dataCate DataCate { get; set; }

		/// <summary>
		/// 展示
		/// </summary>
		/// <param name="Use">是否使用</param>
		/// <param name="Show">是否显示</param>
		/// <param name="MustFillIn">必须填写</param>
		/// <param name="DataCate">数据类型</param>
		/// <param name="DetailsTips">详细提示</param>
		public DisplayAttribute(bool Use, bool Show, bool MustFillIn, ParamModel.dataCate DataCate, string DetailsTips = null)
		{
			this.Use = Use;
			this.Show = Show;
			this.DataCate = DataCate;
			this.MustFillIn = MustFillIn;
			this.DetailsTips = DetailsTips;
		}
	}
	/// <summary>
	/// 多语言特性；<br />
	/// 用于标记属性的多语言描述，包含中文（Zh）和英文（En）；<br />
	/// 配合 LanguageType 枚举和 ILanguage 接口实现多语言支持
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class MultilingualAttribute : Attribute
	{
		/// <summary>
		/// 中文
		/// </summary>
		public string Zh { get; set; }

		/// <summary>
		/// 英文
		/// </summary>
		public string En { get; set; }

		/// <summary>
		/// 多语言特性构造函数
		/// </summary>
		/// <param name="zh">中文</param>
		/// <param name="en">英文</param>
		public MultilingualAttribute(string zh, string en)
		{
			Zh = zh;
			En = en;
		}

		/// <summary>
		/// 获取对应的描述；<br />
		/// 根据语言类型返回对应的多语言文本
		/// </summary>
		/// <param name="language">语言类型</param>
		/// <returns>对应语言的描述文本，不支持的语言类型返回 null</returns>
		public string? Get(LanguageType language)
		{
			return language switch
			{
				LanguageType.zh => Zh, 
				LanguageType.en => En, 
				_ => null, 
			};
		}
	}
	/// <summary>
	/// 单位特性；<br />
	/// 用于标记属性的物理单位（如 ℃、V、A、rpm 等）；<br />
	/// 配合反射机制生成前端参数显示
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class UnitAttribute : Attribute
	{
		/// <summary>
		/// 单位
		/// </summary>
		public string Unit { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="Unit">单位</param>
		public UnitAttribute(string Unit)
		{
			this.Unit = Unit;
		}
	}
	/// <summary>
	/// 验证特性；<br />
	/// 用于标记属性的正则表达式验证规则；<br />
	/// 包含正则表达式（Pattern）和验证失败提示（FailTips）；<br />
	/// 内置 Regex 超时保护，防止正则表达式拒绝服务攻击（ReDoS）
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class VerifyAttribute : Attribute
	{
		/// <summary>
		/// 正则表达式
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// 验证失败提示
		/// </summary>
		public string FailTips { get; set; }

		/// <summary>
		/// 验证
		/// </summary>
		/// <param name="pattern">正则表达式</param>
		/// <param name="failTips">验证失败提示</param>
		public VerifyAttribute(string pattern, string failTips)
		{
			Pattern = pattern;
			FailTips = failTips;
		}

		/// <summary>
		/// 验证数据是否符合正则表达式规则；<br />
		/// 如果正则匹配成功则视为验证失败（黑名单模式），返回 false 并输出失败提示；<br />
		/// 内置超时保护，防止正则表达式拒绝服务攻击（ReDoS）
		/// </summary>
		/// <param name="source">待验证的数据源字符串</param>
		/// <param name="failTips">响应：验证失败时的提示信息</param>
		/// <param name="ignoreCase">是否忽略大小写，默认不忽略</param>
		/// <returns>true：验证通过；false：验证失败</returns>
		public bool Verify(string source, out string? failTips, bool ignoreCase = false)
		{
			failTips = string.Empty;
			bool num = !Regex.IsMatch(source, Pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, TimeSpan.FromSeconds(2.0));
			if (!num)
			{
				failTips = FailTips;
			}
			return num;
		}
	}
}
