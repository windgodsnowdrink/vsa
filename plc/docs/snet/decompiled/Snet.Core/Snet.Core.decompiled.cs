using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Snet.Core.@abstract;
using Snet.Core.cache.share;
using Snet.Core.communication.net.core;
using Snet.Core.communication.net.@enum;
using Snet.Core.communication.net.http.service;
using Snet.Core.extend;
using Snet.Core.handler;
using Snet.Core.mq;
using Snet.Core.packer;
using Snet.Core.plugin;
using Snet.Core.reflection;
using Snet.Core.subscribe.core;
using Snet.Log;
using Snet.Model.attribute;
using Snet.Model.data;
using Snet.Model.@enum;
using Snet.Model.@event;
using Snet.Model.@interface;
using Snet.Utility;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("核心组件：Core（抽象、扩展、处理、反射、队列、中间件、订阅、虚拟地址、TCP、UDP、HTTP、WS、串口、WebApi、注入、组包、本地化、标准化）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Core")]
[assembly: AssemblyTitle("Snet.Core")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
/// <summary>
/// 共享缓存内存分配器<br />
/// <br />
/// 功能：<br />
/// - 管理共享内存正文区（HeaderSize 之后）的空间分配与释放<br />
/// - 支持内存块的复用（FreeBlocks）<br />
/// - 避免频繁扩展文件大小，提高空间利用率<br />
/// - 提供持久化接口（Get/SetState），便于进程重启后恢复状态<br />
/// <br />
/// 算法思想：<br />
/// - 使用“近似最佳适配（Best Fit）”策略：<br />
///   在 freeBlocks 中寻找最小的满足需求的空闲块；<br />
///   如果没有合适的空闲块，则从 currentPosition 继续分配新空间。<br />
/// <br />
/// - 空闲块管理：<br />
///   使用 SortedDictionar＜long, ShareCacheFree＞<br />
///   key   = 起始位置（Position）<br />
///   value = 空闲块描述（Position + Length）<br />
/// <br />
/// - 空间回收：<br />
///   当调用 Free 时，会把释放的块加入 freeBlocks，<br />
///   并尝试与前后相邻的空闲块合并，避免碎片化。<br />
/// </summary>
public class ShareCacheFreeListAllocator
{
	/// <summary>
	/// 空闲块集合  
	/// key   : 空闲块的起始位置（正文区相对偏移）  
	/// value : 空闲块对象（Position + Length）
	///
	/// 使用 SortedDictionary 是为了保持 key 有序，便于查找前后相邻块做合并。
	/// </summary>
	private readonly SortedDictionary<long, ShareCacheFree> freeBlocks = new SortedDictionary<long, ShareCacheFree>();

	/// <summary>
	/// 当前分配到的位置指针（正文区相对偏移）  
	/// 表示已经使用到的空间末尾，如果没有可用空闲块时，从这里继续分配。
	/// </summary>
	private long currentPosition;

	/// <summary>
	/// 获取当前分配器位置指针
	/// </summary>
	public long GetCurrentPosition()
	{
		return currentPosition;
	}

	/// <summary>
	/// 获取当前所有空闲块
	/// </summary>
	public List<ShareCacheFree> GetFreeBlocks()
	{
		return freeBlocks.Values.ToList();
	}

	/// <summary>
	/// 设置分配器状态（从持久化数据恢复）
	/// </summary>
	/// <param name="current">已分配到的相对位置</param>
	/// <param name="blocks">空闲块集合</param>
	public void SetState(long current, List<ShareCacheFree> blocks)
	{
		currentPosition = current;
		freeBlocks.Clear();
		foreach (ShareCacheFree block in blocks)
		{
			freeBlocks[block.Position] = block;
		}
	}

	/// <summary>
	/// 分配内存块（近似最佳适配 Best-Fit）
	/// </summary>
	/// <param name="size">所需字节数</param>
	/// <returns>返回正文区相对偏移位置</returns>
	public long Allocate(int size)
	{
		ShareCacheFree shareCacheFree = null;
		long key = -1L;
		foreach (KeyValuePair<long, ShareCacheFree> freeBlock in freeBlocks)
		{
			ShareCacheFree value = freeBlock.Value;
			if (value.Length >= size && (shareCacheFree == null || value.Length < shareCacheFree.Length))
			{
				shareCacheFree = value;
				key = freeBlock.Key;
			}
		}
		if (shareCacheFree != null)
		{
			long position = shareCacheFree.Position;
			if (shareCacheFree.Length == size)
			{
				freeBlocks.Remove(key);
				return position;
			}
			shareCacheFree.Position += size;
			shareCacheFree.Length -= size;
			return position;
		}
		long result = currentPosition;
		currentPosition += size;
		return result;
	}

	/// <summary>
	/// 释放内存块，并尝试合并相邻空闲块
	/// </summary>
	/// <param name="position">要释放的起始位置（正文区相对偏移）</param>
	/// <param name="size">要释放的大小</param>
	public void Free(long position, int size)
	{
		ShareCacheFree shareCacheFree = new ShareCacheFree
		{
			Position = position,
			Length = size
		};
		freeBlocks[position] = shareCacheFree;
		Merge(shareCacheFree);
	}

	/// <summary>
	/// 合并相邻的空闲块
	/// </summary>
	private void Merge(ShareCacheFree block)
	{
		if (freeBlocks.TryGetValue(block.Position - 1, out ShareCacheFree value) && value.Position + value.Length == block.Position)
		{
			block.Position = value.Position;
			block.Length += value.Length;
			freeBlocks.Remove(value.Position);
		}
		if (freeBlocks.TryGetValue(block.Position + block.Length, out ShareCacheFree value2))
		{
			block.Length += value2.Length;
			freeBlocks.Remove(value2.Position);
		}
		freeBlocks[block.Position] = block;
	}

	/// <summary>
	/// 重置分配器状态（清空所有记录，重新开始分配）
	/// </summary>
	public void Reset()
	{
		freeBlocks.Clear();
		currentPosition = 0L;
	}
}
/// <summary>
/// 可回收的插件程序集加载上下文。
/// <para>
/// 核心设计：
/// <list type="bullet">
///   <item>每个插件使用独立的 <see cref="T:System.Runtime.Loader.AssemblyLoadContext" />（<c>isCollectible = true</c>），支持热卸载</item>
///   <item>所有程序集通过内存流加载，不持有文件锁，卸载后可立即删除文件</item>
///   <item>默认上下文中的共享程序集（如接口定义所在的程序集）自动复用，避免类型身份不一致</item>
///   <item>内置路径沙箱校验、文件大小限制、默认上下文名称缓存等安全与性能机制</item>
/// </list>
/// </para>
/// </summary>
internal sealed class PluginLoadContext : AssemblyLoadContext
{
	/// <summary>
	/// 单个程序集文件的默认大小上限：50 MB。
	/// 防止恶意构造的超大 DLL 文件导致内存耗尽（DoS 攻击向量）。
	/// </summary>
	public const long DefaultMaxAssemblySize = 52428800L;

	/// <summary>
	/// 默认上下文名称缓存的初始容量。
	/// 覆盖常见的共享框架程序集数量（如 System.*, Microsoft.*），减少早期扩容。
	/// </summary>
	private const int DefaultContextCacheCapacity = 32;

	/// <summary>
	/// 程序集依赖解析器。
	/// 根据插件旁的 .deps.json 文件和 probing 路径规则，
	/// 将 AssemblyName 映射为磁盘上的 DLL 绝对路径。
	/// </summary>
	private readonly AssemblyDependencyResolver _resolver;

	/// <summary>
	/// 插件目录的绝对路径，末尾始终带有路径分隔符。
	/// <para>
	/// <b>安全修复说明：</b><br />
	/// 原代码中 <c>_pluginDirectory</c> 没有保证尾部分隔符，
	/// 导致 <c>StartsWith(_pluginDirectory)</c> 的路径沙箱校验可被绕过：<br />
	/// 若插件目录为 <c>C:\Plugins</c>，攻击者通过 <c>.deps.json</c> 构造路径
	/// <c>C:\PluginsEvil\malicious.dll</c>，<c>StartsWith("C:\Plugins")</c>
	/// 会错误地返回 <c>true</c>，允许加载沙箱外的程序集。<br /><br />
	/// 修复后 <c>_pluginDirectory</c> 始终为 <c>C:\Plugins\</c>（带尾部反斜杠），
	/// <c>StartsWith</c> 检查 <c>C:\PluginsEvil\...</c> 会正确返回 <c>false</c>。
	/// </para>
	/// </summary>
	private readonly string _pluginDirectory;

	/// <summary>
	/// 单个程序集文件的大小上限（字节）。
	/// 超过此限制的文件将被拒绝加载，防止恶意大文件耗尽内存。默认 50 MB。
	/// </summary>
	private readonly long _maxAssemblySizeBytes;

	/// <summary>
	/// 默认上下文已加载程序集名称的缓存（Key 不区分大小写）。
	/// <para>
	/// 性能优化：默认上下文中的程序集一旦加载就不会被卸载（不可回收），
	/// 因此缓存结果永久有效。将 Default.Assemblies 的 O(n) 遍历
	/// 优化为 ConcurrentDictionary 的 O(1) 查找。
	/// </para>
	/// </summary>
	private readonly ConcurrentDictionary<string, bool> _defaultContextNameCache;

	/// <summary>
	/// 已加载程序集的弱引用列表，用于诊断和验证卸载是否完成。
	/// <para>
	/// <b>线程安全修复说明：</b><br />
	/// 原代码使用普通 <c>List&lt;T&gt;</c>，但 <c>LoadFromFileStream</c> 可能从
	/// 任意线程调用（包括运行时的程序集解析回调），而 <c>IsFullyUnloaded</c>
	/// 可能从 UI 线程或监控线程调用。并发读写 <c>List&lt;T&gt;</c> 会导致
	/// <c>ArgumentOutOfRangeException</c> 或数据损坏。<br /><br />
	/// 修复方案：所有对该列表的访问均通过 <c>_loadedAssemblyRefsLock</c> 保护。
	/// </para>
	/// </summary>
	private readonly List<WeakReference<Assembly>> _loadedAssemblyRefs;

	/// <summary>
	/// _loadedAssemblyRefs 的同步锁。
	/// 保护 LoadFromFileStream（写入）和 IsFullyUnloaded（读取+修改）的并发安全。
	/// </summary>
	private readonly object _loadedAssemblyRefsLock = new object();

	/// <summary>
	/// 通过 LoadFromFileStream 主动加载的程序集计数器。
	/// 使用 Interlocked 保证线程安全递增。
	/// <para>
	/// <b>语义说明：</b>此计数器仅统计通过 <see cref="M:PluginLoadContext.LoadFromFileStream(System.String)" /> 直接加载的程序集，
	/// 不包括运行时通过 <see cref="M:PluginLoadContext.Load(System.Reflection.AssemblyName)" /> 回调自动加载的依赖程序集。
	/// 这使 <see cref="P:PluginLoadContext.LoadedAssemblyCount" /> 的语义更精确——
	/// 它反映的是"主动加载了多少个程序集"，而非"运行时加载了多少个"。
	/// </para>
	/// </summary>
	private int _loadedCount;

	/// <summary>
	/// 获取插件 DLL 的完整绝对路径（已规范化）。
	/// </summary>
	public string PluginPath { get; }

	/// <summary>
	/// 获取通过 LoadFromFileStream 主动加载的程序集数量（线程安全读取）。
	/// 不包括通过 Load 回调自动加载的依赖程序集。
	/// </summary>
	public int LoadedAssemblyCount => Volatile.Read(in _loadedCount);

	/// <summary>
	/// 创建可回收的插件加载上下文实例。
	/// </summary>
	/// <param name="pluginPath">
	/// 插件 DLL 的完整路径。该路径用于：
	/// <list type="bullet">
	///   <item>初始化 <see cref="T:System.Runtime.Loader.AssemblyDependencyResolver" /> 以解析依赖关系</item>
	///   <item>确定插件目录作为路径沙箱的根目录</item>
	/// </list>
	/// </param>
	/// <param name="maxAssemblySizeBytes">
	/// 单个程序集文件大小上限（字节），默认 50 MB。
	/// 设为 <c>-1</c> 可禁用大小检查（不推荐用于不可信插件）。
	/// </param>
	/// <exception cref="T:System.ArgumentNullException">pluginPath 为 null 或空白。</exception>
	/// <exception cref="T:System.IO.FileNotFoundException">插件文件不存在。</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">maxAssemblySizeBytes 等于 0。</exception>
	public PluginLoadContext(string pluginPath, long maxAssemblySizeBytes = 52428800L)
		: base(isCollectible: true)
	{
		if (string.IsNullOrWhiteSpace(pluginPath))
		{
			throw new ArgumentNullException("pluginPath", "插件路径不能为 null 或空白。");
		}
		if (maxAssemblySizeBytes == 0L)
		{
			throw new ArgumentOutOfRangeException("maxAssemblySizeBytes", "文件大小上限不能为 0。");
		}
		string fullPath = Path.GetFullPath(pluginPath);
		if (!File.Exists(fullPath))
		{
			throw new FileNotFoundException("插件文件不存在: " + fullPath, fullPath);
		}
		PluginPath = fullPath;
		string text = Path.GetDirectoryName(fullPath) ?? throw new ArgumentException("无法从路径中解析目录信息，请确认路径格式正确。", "pluginPath");
		_pluginDirectory = text.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
		_maxAssemblySizeBytes = maxAssemblySizeBytes;
		_resolver = new AssemblyDependencyResolver(fullPath);
		_defaultContextNameCache = new ConcurrentDictionary<string, bool>(Environment.ProcessorCount, 32, StringComparer.OrdinalIgnoreCase);
		_loadedAssemblyRefs = new List<WeakReference<Assembly>>();
	}

	/// <summary>
	/// 以内存流方式加载程序集文件，不持有磁盘文件锁。
	/// <para>
	/// 加载流程：
	/// <list type="number">
	///   <item><b>校验</b> — 验证文件路径、存在性</item>
	///   <item><b>读取</b> — File.ReadAllBytes 一次性读入字节数组后立即关闭文件句柄</item>
	///   <item><b>大小校验</b> — 基于实际读取的字节数进行大小检查，消除 TOCTOU 竞态</item>
	///   <item><b>加载</b> — 通过 LoadFromStream 从内存流加载程序集</item>
	///   <item><b>记录</b> — 保存弱引用用于后续卸载验证</item>
	/// </list>
	/// </para>
	/// </summary>
	/// <param name="assemblyPath">程序集 DLL 的完整路径</param>
	/// <returns>加载后的 Assembly 实例</returns>
	/// <exception cref="T:System.IO.FileNotFoundException">程序集文件不存在</exception>
	/// <exception cref="T:System.InvalidOperationException">文件为空或超过大小上限</exception>
	public Assembly LoadFromFileStream(string assemblyPath)
	{
		string fullPath = Path.GetFullPath(assemblyPath);
		if (!File.Exists(fullPath))
		{
			throw new FileNotFoundException("程序集文件不存在: " + fullPath, fullPath);
		}
		byte[] array = File.ReadAllBytes(fullPath);
		if (array.Length == 0)
		{
			throw new InvalidOperationException("程序集文件为空（0 字节），无法加载: " + fullPath);
		}
		if (_maxAssemblySizeBytes > 0 && array.Length > _maxAssemblySizeBytes)
		{
			throw new InvalidOperationException($"程序集文件大小 ({array.Length:N0} 字节) 超过允许的上限 ({_maxAssemblySizeBytes:N0} 字节): {fullPath}");
		}
		using MemoryStream assembly = new MemoryStream(array, 0, array.Length, writable: false);
		string text = Path.ChangeExtension(fullPath, ".pdb");
		Assembly assembly2;
		if (File.Exists(text))
		{
			byte[] array2 = File.ReadAllBytes(text);
			if (_maxAssemblySizeBytes > 0 && array2.Length > _maxAssemblySizeBytes)
			{
				throw new InvalidOperationException($"PDB 调试符号文件大小 ({array2.Length:N0} 字节) 超过上限: {text}");
			}
			using MemoryStream assemblySymbols = new MemoryStream(array2, 0, array2.Length, writable: false);
			assembly2 = LoadFromStream(assembly, assemblySymbols);
		}
		else
		{
			assembly2 = LoadFromStream(assembly);
		}
		lock (_loadedAssemblyRefsLock)
		{
			_loadedAssemblyRefs.Add(new WeakReference<Assembly>(assembly2));
		}
		Interlocked.Increment(ref _loadedCount);
		return assembly2;
	}

	/// <summary>
	/// 【新增】以内存流方式异步加载程序集文件，不持有磁盘文件锁。
	/// <para>
	/// 供异步调用路径（如 <see cref="!:PluginOperate.InitPluginCoreAsync" />）使用，
	/// 避免在异步上下文中阻塞线程等待同步文件 I/O。
	/// </para>
	/// <para>
	/// <b>注意：</b>此方法仅应通过异步路径调用。
	/// 运行时的 <see cref="M:PluginLoadContext.Load(System.Reflection.AssemblyName)" /> 回调使用同步版本 <see cref="M:PluginLoadContext.LoadFromFileStream(System.String)" />，
	/// 因为回调签名是同步的。
	/// </para>
	/// </summary>
	/// <param name="assemblyPath">程序集 DLL 的完整路径</param>
	/// <param name="cancellationToken">取消令牌</param>
	/// <returns>加载后的 Assembly 实例</returns>
	public async Task<Assembly> LoadFromFileStreamAsync(string assemblyPath, CancellationToken cancellationToken = default(CancellationToken))
	{
		string fullPath = Path.GetFullPath(assemblyPath);
		byte[] array = await File.ReadAllBytesAsync(fullPath, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (array.Length == 0)
		{
			throw new InvalidOperationException("程序集文件为空（0 字节），无法加载: " + fullPath);
		}
		if (_maxAssemblySizeBytes > 0 && array.Length > _maxAssemblySizeBytes)
		{
			throw new InvalidOperationException($"程序集文件大小 ({array.Length:N0} 字节) 超过允许的上限 ({_maxAssemblySizeBytes:N0} 字节): {fullPath}");
		}
		using MemoryStream assemblyStream = new MemoryStream(array, 0, array.Length, writable: false);
		string pdbPath = Path.ChangeExtension(fullPath, ".pdb");
		Assembly assembly;
		if (File.Exists(pdbPath))
		{
			byte[] array2 = await File.ReadAllBytesAsync(pdbPath, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_maxAssemblySizeBytes > 0 && array2.Length > _maxAssemblySizeBytes)
			{
				throw new InvalidOperationException($"PDB 调试符号文件大小 ({array2.Length:N0} 字节) 超过上限: {pdbPath}");
			}
			using MemoryStream assemblySymbols = new MemoryStream(array2, 0, array2.Length, writable: false);
			assembly = LoadFromStream(assemblyStream, assemblySymbols);
		}
		else
		{
			assembly = LoadFromStream(assemblyStream);
		}
		lock (_loadedAssemblyRefsLock)
		{
			_loadedAssemblyRefs.Add(new WeakReference<Assembly>(assembly));
		}
		Interlocked.Increment(ref _loadedCount);
		return assembly;
	}

	/// <summary>
	/// 检查此上下文中的所有程序集是否已被 GC 完全回收（即卸载是否真正完成）。
	/// <para>
	/// <b>重要说明：</b><see cref="M:System.Runtime.Loader.AssemblyLoadContext.Unload" /> 仅标记上下文为可回收，
	/// 程序集内存的实际释放取决于 GC 何时运行以及是否所有引用都已断开。
	/// </para>
	/// </summary>
	/// <returns>
	/// 若所有已加载的程序集均已被 GC 回收，返回 <c>true</c>；
	/// 若仍有程序集存活，返回 <c>false</c>。
	/// </returns>
	public bool IsFullyUnloaded()
	{
		lock (_loadedAssemblyRefsLock)
		{
			bool result = true;
			for (int num = _loadedAssemblyRefs.Count - 1; num >= 0; num--)
			{
				if (!_loadedAssemblyRefs[num].TryGetTarget(out Assembly _))
				{
					_loadedAssemblyRefs.RemoveAt(num);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
	}

	/// <summary>
	/// 解析托管程序集依赖。由运行时在程序集解析失败时自动回调。
	/// <para>
	/// <b>解析策略（三级查找）：</b>
	/// <list type="number">
	///   <item>
	///     <b>缓存命中</b>（O(1)）— 检查 _defaultContextNameCache，
	///     若已知该名称存在于默认上下文中，直接返回 null 让运行时复用默认上下文版本。
	///   </item>
	///   <item>
	///     <b>遍历默认上下文</b>（O(n)，仅首次）— 扫描 Default.Assemblies 查找同名程序集。
	///     若命中则加入缓存并返回 null。
	///   </item>
	///   <item>
	///     <b>插件目录加载</b> — 通过 AssemblyDependencyResolver 定位后以内存流方式加载。
	///     加载前执行路径沙箱校验。
	///   </item>
	/// </list>
	/// </para>
	/// <para>
	/// <b>为什么返回 null 复用默认上下文？</b><br />
	/// 接口/抽象类型（如 IDaq、IMq）通常定义在宿主和插件共享的程序集中。
	/// 若插件上下文加载了自己的副本，CLR 会将其视为不同的类型，
	/// 导致 is / as 类型检查失败，插件功能异常。
	/// </para>
	/// </summary>
	/// <param name="assemblyName">需要解析的程序集名称</param>
	/// <returns>
	/// 已加载的 Assembly，或 null（表示此上下文不负责加载，交由运行时继续解析）。
	/// </returns>
	protected override Assembly? Load(AssemblyName assemblyName)
	{
		string name = assemblyName.Name;
		if (name == null)
		{
			return null;
		}
		if (_defaultContextNameCache.ContainsKey(name))
		{
			return null;
		}
		foreach (Assembly assembly in AssemblyLoadContext.Default.Assemblies)
		{
			if (string.Equals(assembly.GetName().Name, name, StringComparison.OrdinalIgnoreCase))
			{
				_defaultContextNameCache.TryAdd(name, value: true);
				return null;
			}
		}
		string text = _resolver.ResolveAssemblyToPath(assemblyName);
		if (text != null)
		{
			ValidatePathInSandbox(text, "程序集", assemblyName.FullName);
			return LoadAssemblyFromPathSafe(text);
		}
		return null;
	}

	/// <summary>
	/// 解析非托管（原生）DLL 依赖。由运行时在 P/Invoke 或原生库加载失败时自动回调。
	/// </summary>
	/// <param name="unmanagedDllName">需要解析的原生库名称</param>
	/// <returns>已加载的原生库句柄，或 IntPtr.Zero（表示未找到）。</returns>
	protected override nint LoadUnmanagedDll(string unmanagedDllName)
	{
		string text = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
		if (text != null)
		{
			ValidatePathInSandbox(text, "非托管库", unmanagedDllName);
			return LoadUnmanagedDllFromPath(text);
		}
		return IntPtr.Zero;
	}

	/// <summary>
	/// 验证给定路径是否位于插件目录沙箱内，防止路径遍历攻击。
	/// <para>
	/// <b>路径比较策略：</b><br />
	/// 原代码使用 <c>Path.GetRelativePath</c> 检查是否以 ".." 开头。
	/// 但更安全且更易理解的方案是：将被检查路径规范化后，
	/// 使用带尾部分隔符的沙箱目录作为前缀进行 <c>StartsWith</c> 检查。<br /><br />
	/// 尾部分隔符确保检查的是"目录边界"而非"前缀匹配"：<br />
	/// - <c>C:\Plugins\</c> 可匹配 <c>C:\Plugins\sub\file.dll</c>（合法）<br />
	/// - <c>C:\Plugins\</c> 不匹配 <c>C:\PluginsEvil\file.dll</c>（非法，修复了原有漏洞）
	/// </para>
	/// </summary>
	/// <param name="resolvedPath">依赖解析器返回的路径</param>
	/// <param name="resourceType">资源类型描述（用于异常消息）</param>
	/// <param name="resourceName">资源标识名称（用于异常消息）</param>
	/// <exception cref="T:System.InvalidOperationException">路径不在插件目录沙箱内</exception>
	private void ValidatePathInSandbox(string resolvedPath, string resourceType, string resourceName)
	{
		string fullPath = Path.GetFullPath(resolvedPath);
		if (!fullPath.StartsWith(_pluginDirectory, StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException($"安全限制：{resourceType} '{resourceName}' 的解析路径 '{fullPath}' 超出了插件沙箱目录 '{_pluginDirectory.TrimEnd(Path.DirectorySeparatorChar)}' 的范围，已拒绝加载。");
		}
	}

	/// <summary>
	/// 【新增】安全地从路径加载程序集（内存流方式），不更新加载计数和弱引用列表。
	/// <para>
	/// <b>设计说明：</b><br />
	/// 此方法专供 <see cref="M:PluginLoadContext.Load(System.Reflection.AssemblyName)" /> 回调使用。<br />
	/// 通过 <see cref="M:PluginLoadContext.Load(System.Reflection.AssemblyName)" /> 加载的程序集是运行时自动解析的依赖项，
	/// 不应计入 <see cref="P:PluginLoadContext.LoadedAssemblyCount" />（该属性仅反映主动加载的数量），
	/// 也不需要写入 <c>_loadedAssemblyRefs</c>（依赖程序集的生命周期由运行时管理）。<br /><br />
	/// 消除了原代码中 <c>Load</c> 回调调用 <c>LoadFromFileStream</c> 导致的
	/// <c>_loadedCount</c> 语义混淆和重复计数问题。
	/// </para>
	/// </summary>
	/// <param name="assemblyPath">程序集文件的完整路径（已通过沙箱校验）</param>
	/// <returns>加载后的 Assembly 实例</returns>
	private Assembly LoadAssemblyFromPathSafe(string assemblyPath)
	{
		byte[] array = File.ReadAllBytes(assemblyPath);
		if (array.Length == 0)
		{
			throw new InvalidOperationException("依赖程序集文件为空: " + assemblyPath);
		}
		if (_maxAssemblySizeBytes > 0 && array.Length > _maxAssemblySizeBytes)
		{
			throw new InvalidOperationException($"依赖程序集大小 ({array.Length:N0} 字节) 超过上限 ({_maxAssemblySizeBytes:N0} 字节): {assemblyPath}");
		}
		using MemoryStream assembly = new MemoryStream(array, writable: false);
		return LoadFromStream(assembly);
	}
}
namespace Snet.Core
{
	/// <summary>
	/// 全局依赖注入管理器。<br />
	///             <br />
	/// 基于 Microsoft.Extensions.DependencyInjection 封装。<br />
	///             <br />
	/// 功能：<br />
	/// 1. 服务注册<br />
	/// 2. 服务获取<br />
	/// 3. 服务删除<br />
	/// 4. 服务清理<br />
	/// 5. 同步释放<br />
	/// 6. 异步释放<br />
	///             <br />
	/// 特性：<br />
	/// - 全局单例模式<br />
	/// - SemaphoreSlim 异步锁<br />
	/// - 支持 CancellationToken<br />
	/// - 支持 IAsyncDisposable 服务释放<br />
	///             <br />
	/// 注意：<br />
	/// 动态修改服务注册会重新创建 ServiceProvider。<br />
	/// 因此 Singleton 生命周期服务会重新实例化。
	/// </summary>
	public class InjectionHandler
	{
		/// <summary>
		/// 异步互斥锁。
		///
		/// 用于保护：
		/// ServiceCollection
		/// ServiceProvider
		///
		/// 避免多线程同时修改 DI 容器。
		/// </summary>
		private static readonly SemaphoreSlim _mutex;

		/// <summary>
		/// 服务注册集合。
		///
		/// 保存所有 IServiceDescriptor。
		/// </summary>
		private static ServiceCollection? _services;

		/// <summary>
		/// 服务提供器。
		///
		/// 根据 ServiceCollection 创建。
		/// </summary>
		private static ServiceProvider? _provider;

		/// <summary>
		/// 静态构造函数。
		///
		/// CLR 第一次访问该类型时自动调用。
		/// </summary>
		static InjectionHandler()
		{
			_mutex = new SemaphoreSlim(1, 1);
			Initialize();
		}

		/// <summary>
		/// 同步初始化 DI 容器。
		///
		/// 内部调用异步初始化方法。
		///
		/// 统一维护一套初始化逻辑。
		/// </summary>
		private static void Initialize()
		{
			InitializeAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步初始化 DI 容器。
		///
		/// 创建：
		/// ServiceCollection
		/// ServiceProvider
		///
		/// </summary>
		/// <param name="token">
		/// 取消令牌
		/// </param>
		public static async Task InitializeAsync(CancellationToken token = default(CancellationToken))
		{
			await _mutex.WaitAsync(token);
			try
			{
				if (_services == null)
				{
					token.ThrowIfCancellationRequested();
					_services = new ServiceCollection();
					await BuildProviderAsync(token);
				}
			}
			finally
			{
				_mutex.Release();
			}
		}

		/// <summary>
		/// 同步创建 ServiceProvider。
		///
		/// 内部调用异步方法。
		/// </summary>
		private static void BuildProvider()
		{
			BuildProviderAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步创建 ServiceProvider。
		///
		/// 创建新的 Provider 前，
		/// 会释放旧 Provider。
		///
		/// 支持：
		/// IDisposable
		/// IAsyncDisposable
		///
		/// </summary>
		/// <param name="token">
		/// 取消令牌
		/// </param>
		private static async Task BuildProviderAsync(CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			if (_provider != null)
			{
				await _provider.DisposeAsync();
				_provider = null;
			}
			token.ThrowIfCancellationRequested();
			_provider = _services.BuildServiceProvider(new ServiceProviderOptions
			{
				ValidateScopes = true,
				ValidateOnBuild = true
			});
		}

		/// <summary>
		/// 获取当前 ServiceProvider。
		///
		/// 如果未初始化则抛出异常。
		/// </summary>
		public static ServiceProvider GetProvider()
		{
			return _provider ?? throw new InvalidOperationException("DI 容器尚未初始化");
		}

		/// <summary>
		/// 添加服务。
		///
		/// 同步入口。
		///
		/// 内部调用 AddServiceAsync。
		///
		/// </summary>
		/// <param name="action">
		/// 服务注册委托。
		///
		/// 示例：
		///
		/// services =&gt;
		/// {
		///     services.AddSingleton&lt;ITest,Test&gt;();
		/// }
		///
		/// </param>
		public static void AddService(Action<IServiceCollection> action)
		{
			AddServiceAsync(action).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步添加服务。
		///
		/// 注册完成后重新创建 ServiceProvider。
		///
		/// </summary>
		/// <param name="action">
		/// 服务注册操作。
		/// </param>
		/// <param name="token">
		/// 取消令牌。
		/// </param>
		public static async Task AddServiceAsync(Action<IServiceCollection> action, CancellationToken token = default(CancellationToken))
		{
			ArgumentNullException.ThrowIfNull(action, "action");
			await _mutex.WaitAsync(token);
			try
			{
				EnsureInitialized();
				token.ThrowIfCancellationRequested();
				action(_services);
				await BuildProviderAsync(token);
			}
			finally
			{
				_mutex.Release();
			}
		}

		/// <summary>
		/// 获取指定类型服务。
		///
		/// 如果服务不存在，
		/// 会抛出 InvalidOperationException。
		/// </summary>
		/// <typeparam name="T">
		/// 服务类型
		/// </typeparam>
		/// <returns>
		/// 服务实例
		/// </returns>
		public static T GetService<T>() where T : class
		{
			return GetProvider().GetRequiredService<T>();
		}

		/// <summary>
		/// 异步获取指定类型服务。
		///
		/// Microsoft DI 本身获取服务是同步操作，
		/// 此方法用于统一异步调用模型。
		/// </summary>
		/// <typeparam name="T">
		/// 服务类型
		/// </typeparam>
		/// <param name="token">
		/// 取消令牌
		/// </param>
		/// <returns>
		/// 服务实例
		/// </returns>
		public static Task<T> GetServiceAsync<T>(CancellationToken token = default(CancellationToken)) where T : class
		{
			token.ThrowIfCancellationRequested();
			return Task.FromResult(GetService<T>());
		}

		/// <summary>
		/// 尝试获取服务。
		///
		/// 如果服务不存在，
		/// 返回 false。
		/// </summary>
		/// <typeparam name="T">
		/// 服务类型
		/// </typeparam>
		/// <param name="service">
		/// 返回服务实例
		/// </param>
		/// <returns>
		/// 是否获取成功
		/// </returns>
		public static bool TryGetService<T>(out T? service) where T : class
		{
			service = GetProvider().GetService<T>();
			return service != null;
		}

		/// <summary>
		/// 判断指定类型服务是否已经注册。
		///
		/// 注意：
		/// 这里只检查 ServiceCollection 注册信息，
		/// 不代表实例已经创建。
		/// </summary>
		/// <typeparam name="T">
		/// 服务类型
		/// </typeparam>
		/// <returns>
		/// true 已注册
		///
		/// false 未注册
		/// </returns>
		public static bool ExistService<T>() where T : class
		{
			EnsureInitialized();
			return _services.Any((ServiceDescriptor x) => x.ServiceType == typeof(T));
		}

		/// <summary>
		/// 删除指定类型服务。
		///
		/// 同步入口。
		///
		/// 内部调用 RemoveServiceAsync。
		/// </summary>
		/// <typeparam name="T">
		/// 服务类型
		/// </typeparam>
		public static void RemoveService<T>() where T : class
		{
			RemoveServiceAsync<T>().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步删除指定服务。
		///
		/// 删除完成后重新创建 Provider。
		/// </summary>
		/// <typeparam name="T">
		/// 服务类型
		/// </typeparam>
		/// <param name="token">
		/// 取消令牌
		/// </param>
		public static async Task RemoveServiceAsync<T>(CancellationToken token = default(CancellationToken)) where T : class
		{
			await _mutex.WaitAsync(token);
			try
			{
				EnsureInitialized();
				token.ThrowIfCancellationRequested();
				foreach (ServiceDescriptor item in _services.Where((ServiceDescriptor x) => x.ServiceType == typeof(T)).ToList())
				{
					_services.Remove(item);
				}
				await BuildProviderAsync(token);
			}
			finally
			{
				_mutex.Release();
			}
		}

		/// <summary>
		/// 清空所有服务。
		///
		/// 同步入口。
		/// </summary>
		public static void ClearService()
		{
			ClearServiceAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步清空所有服务。
		///
		/// 执行：
		///
		/// 1. 释放 Provider
		/// 2. 清空注册
		/// 3. 创建新的容器
		///
		/// </summary>
		/// <param name="token">
		/// 取消令牌
		/// </param>
		public static async Task ClearServiceAsync(CancellationToken token = default(CancellationToken))
		{
			await _mutex.WaitAsync(token);
			try
			{
				token.ThrowIfCancellationRequested();
				if (_provider != null)
				{
					await _provider.DisposeAsync();
					_provider = null;
				}
				_services?.Clear();
				_services = new ServiceCollection();
				await BuildProviderAsync(token);
			}
			finally
			{
				_mutex.Release();
			}
		}

		/// <summary>
		/// 同步释放 DI 容器。
		///
		/// 内部调用异步释放。
		/// </summary>
		public static void Dispose()
		{
			DisposeAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步释放 DI 容器。
		///
		/// 支持：
		///
		/// IDisposable
		///
		/// IAsyncDisposable
		///
		/// </summary>
		/// <param name="token">
		/// 取消令牌
		/// </param>
		public static async ValueTask DisposeAsync(CancellationToken token = default(CancellationToken))
		{
			await _mutex.WaitAsync(token);
			try
			{
				token.ThrowIfCancellationRequested();
				if (_provider != null)
				{
					await _provider.DisposeAsync();
					_provider = null;
				}
				_services?.Clear();
				_services = null;
			}
			finally
			{
				_mutex.Release();
			}
		}

		/// <summary>
		/// 检查 DI 容器是否已经初始化。
		///
		/// 未初始化直接抛异常。
		/// </summary>
		private static void EnsureInitialized()
		{
			if (_services == null)
			{
				throw new InvalidOperationException("DI 容器尚未初始化");
			}
		}
	}
	/// <summary>
	///   一个强类型的资源类，用于查找本地化的字符串等。
	/// </summary>
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "18.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class Language
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		/// <summary>
		///   返回此类使用的缓存的 ResourceManager 实例。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("Snet.Core.Language", typeof(Language).Assembly);
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   重写当前线程的 CurrentUICulture 属性，对
		///   使用此强类型资源类的所有资源查找执行重写。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		/// <summary>
		///   查找类似 [ 订阅通知 ]点位数据更新 的本地化字符串。
		/// </summary>
		public static string __订阅通知__点位数据更新 => ResourceManager.GetString("[ 订阅通知 ]点位数据更新", resourceCulture);

		/// <summary>
		///   查找类似 创建实例失败，请检查传入的参数 的本地化字符串。
		/// </summary>
		public static string createInstanceErrorTips => ResourceManager.GetString("createInstanceErrorTips", resourceCulture);

		/// <summary>
		///   查找类似 当前实例数超过了最大限制（{0}）请检查您的代码是否合理地使用了单例模式 的本地化字符串。
		/// </summary>
		public static string exceedMaxInstanceCountTips => ResourceManager.GetString("exceedMaxInstanceCountTips", resourceCulture);

		/// <summary>
		///   查找类似 未检索到 AutoAllocatingTagAttribute 的本地化字符串。
		/// </summary>
		public static string ExistsAutoAllocatingParam_msg1 => ResourceManager.GetString("ExistsAutoAllocatingParam_msg1", resourceCulture);

		/// <summary>
		///   查找类似 基础数据尚未实例化，无法判断是否存在 AutoAllocatingTagAttribute 的本地化字符串。
		/// </summary>
		public static string ExistsAutoAllocatingParam_msg2 => ResourceManager.GetString("ExistsAutoAllocatingParam_msg2", resourceCulture);

		/// <summary>
		///   查找类似 不支持写入 的本地化字符串。
		/// </summary>
		public static string 不支持写入 => ResourceManager.GetString("不支持写入", resourceCulture);

		/// <summary>
		///   查找类似 不支持类型转换 的本地化字符串。
		/// </summary>
		public static string 不支持类型转换 => ResourceManager.GetString("不支持类型转换", resourceCulture);

		/// <summary>
		///   查找类似 克隆成功 的本地化字符串。
		/// </summary>
		public static string 克隆成功 => ResourceManager.GetString("克隆成功", resourceCulture);

		/// <summary>
		///   查找类似 关闭失败 的本地化字符串。
		/// </summary>
		public static string 关闭失败 => ResourceManager.GetString("关闭失败", resourceCulture);

		/// <summary>
		///   查找类似 关闭超时 的本地化字符串。
		/// </summary>
		public static string 关闭超时 => ResourceManager.GetString("关闭超时", resourceCulture);

		/// <summary>
		///   查找类似 写入失败 的本地化字符串。
		/// </summary>
		public static string 写入失败 => ResourceManager.GetString("写入失败", resourceCulture);

		/// <summary>
		///   查找类似 写入成功 的本地化字符串。
		/// </summary>
		public static string 写入成功 => ResourceManager.GetString("写入成功", resourceCulture);

		/// <summary>
		///   查找类似 参数修改成功 的本地化字符串。
		/// </summary>
		public static string 参数修改成功 => ResourceManager.GetString("参数修改成功", resourceCulture);

		/// <summary>
		///   查找类似 发送数据失败 的本地化字符串。
		/// </summary>
		public static string 发送数据失败 => ResourceManager.GetString("发送数据失败", resourceCulture);

		/// <summary>
		///   查找类似 发送数据超时 的本地化字符串。
		/// </summary>
		public static string 发送数据超时 => ResourceManager.GetString("发送数据超时", resourceCulture);

		/// <summary>
		///   查找类似 发送等待结果失败 的本地化字符串。
		/// </summary>
		public static string 发送等待结果失败 => ResourceManager.GetString("发送等待结果失败", resourceCulture);

		/// <summary>
		///   查找类似 在 的本地化字符串。
		/// </summary>
		public static string 在 => ResourceManager.GetString("在", resourceCulture);

		/// <summary>
		///   查找类似 失败 的本地化字符串。
		/// </summary>
		public static string 失败 => ResourceManager.GetString("失败", resourceCulture);

		/// <summary>
		///   查找类似 失败信息 的本地化字符串。
		/// </summary>
		public static string 失败信息 => ResourceManager.GetString("失败信息", resourceCulture);

		/// <summary>
		///   查找类似 字节长度越界 的本地化字符串。
		/// </summary>
		public static string 字节长度越界 => ResourceManager.GetString("字节长度越界", resourceCulture);

		/// <summary>
		///   查找类似 存在 的本地化字符串。
		/// </summary>
		public static string 存在 => ResourceManager.GetString("存在", resourceCulture);

		/// <summary>
		///   查找类似 存在数据块发送失败 的本地化字符串。
		/// </summary>
		public static string 存在数据块发送失败 => ResourceManager.GetString("存在数据块发送失败", resourceCulture);

		/// <summary>
		///   查找类似 实例无法创建 的本地化字符串。
		/// </summary>
		public static string 实例无法创建 => ResourceManager.GetString("实例无法创建", resourceCulture);

		/// <summary>
		///   查找类似 客户端 的本地化字符串。
		/// </summary>
		public static string 客户端 => ResourceManager.GetString("客户端", resourceCulture);

		/// <summary>
		///   查找类似 对象更新失败 的本地化字符串。
		/// </summary>
		public static string 对象更新失败 => ResourceManager.GetString("对象更新失败", resourceCulture);

		/// <summary>
		///   查找类似 对象移除失败 的本地化字符串。
		/// </summary>
		public static string 对象移除失败 => ResourceManager.GetString("对象移除失败", resourceCulture);

		/// <summary>
		///   查找类似 已存在节点地址 的本地化字符串。
		/// </summary>
		public static string 已存在节点地址 => ResourceManager.GetString("已存在节点地址", resourceCulture);

		/// <summary>
		///   查找类似 已断开连接 的本地化字符串。
		/// </summary>
		public static string 已断开连接 => ResourceManager.GetString("已断开连接", resourceCulture);

		/// <summary>
		///   查找类似 已连接 的本地化字符串。
		/// </summary>
		public static string 已连接 => ResourceManager.GetString("已连接", resourceCulture);

		/// <summary>
		///   查找类似 异常 的本地化字符串。
		/// </summary>
		public static string 异常 => ResourceManager.GetString("异常", resourceCulture);

		/// <summary>
		///   查找类似 当前尚未订阅 的本地化字符串。
		/// </summary>
		public static string 当前尚未订阅 => ResourceManager.GetString("当前尚未订阅", resourceCulture);

		/// <summary>
		///   查找类似 成功 的本地化字符串。
		/// </summary>
		public static string 成功 => ResourceManager.GetString("成功", resourceCulture);

		/// <summary>
		///   查找类似 执行数据处理异常 的本地化字符串。
		/// </summary>
		public static string 执行数据处理异常 => ResourceManager.GetString("执行数据处理异常", resourceCulture);

		/// <summary>
		///   查找类似 找不到指定语言 的本地化字符串。
		/// </summary>
		public static string 找不到指定语言 => ResourceManager.GetString("找不到指定语言", resourceCulture);

		/// <summary>
		///   查找类似 找不到资源文件 的本地化字符串。
		/// </summary>
		public static string 找不到资源文件 => ResourceManager.GetString("找不到资源文件", resourceCulture);

		/// <summary>
		///   查找类似 接收数据超时 的本地化字符串。
		/// </summary>
		public static string 接收数据超时 => ResourceManager.GetString("接收数据超时", resourceCulture);

		/// <summary>
		///   查找类似 文件 的本地化字符串。
		/// </summary>
		public static string 文件 => ResourceManager.GetString("文件", resourceCulture);

		/// <summary>
		///   查找类似 无法获取自动分配标识的属性值所对应参数集合 的本地化字符串。
		/// </summary>
		public static string 无法获取自动分配标识的属性值所对应参数集合 => ResourceManager.GetString("无法获取自动分配标识的属性值所对应参数集合", resourceCulture);

		/// <summary>
		///   查找类似 无须指定终端地址 的本地化字符串。
		/// </summary>
		public static string 无须指定终端地址 => ResourceManager.GetString("无须指定终端地址", resourceCulture);

		/// <summary>
		///   查找类似 日志参数设置成功 的本地化字符串。
		/// </summary>
		public static string 日志参数设置成功 => ResourceManager.GetString("日志参数设置成功", resourceCulture);

		/// <summary>
		///   查找类似 日期已变更 的本地化字符串。
		/// </summary>
		public static string 日期已变更 => ResourceManager.GetString("日期已变更", resourceCulture);

		/// <summary>
		///   查找类似 暂无描述 的本地化字符串。
		/// </summary>
		public static string 暂无描述 => ResourceManager.GetString("暂无描述", resourceCulture);

		/// <summary>
		///   查找类似 未接收到数据 的本地化字符串。
		/// </summary>
		public static string 未接收到数据 => ResourceManager.GetString("未接收到数据", resourceCulture);

		/// <summary>
		///   查找类似 未查询到此节点 的本地化字符串。
		/// </summary>
		public static string 未查询到此节点 => ResourceManager.GetString("未查询到此节点", resourceCulture);

		/// <summary>
		///   查找类似 未连接 的本地化字符串。
		/// </summary>
		public static string 未连接 => ResourceManager.GetString("未连接", resourceCulture);

		/// <summary>
		///   查找类似 查询结果为空 的本地化字符串。
		/// </summary>
		public static string 查询结果为空 => ResourceManager.GetString("查询结果为空", resourceCulture);

		/// <summary>
		///   查找类似 添加失败 的本地化字符串。
		/// </summary>
		public static string 添加失败 => ResourceManager.GetString("添加失败", resourceCulture);

		/// <summary>
		///   查找类似 点位数据更新异常 的本地化字符串。
		/// </summary>
		public static string 点位数据更新异常 => ResourceManager.GetString("点位数据更新异常", resourceCulture);

		/// <summary>
		///   查找类似 监控异常：{0}，断开连接 的本地化字符串。
		/// </summary>
		public static string 监控异常 => ResourceManager.GetString("监控异常", resourceCulture);

		/// <summary>
		///   查找类似 监控数据 的本地化字符串。
		/// </summary>
		public static string 监控数据 => ResourceManager.GetString("监控数据", resourceCulture);

		/// <summary>
		///   查找类似 监控数据长度错误（小于等于零） 的本地化字符串。
		/// </summary>
		public static string 监控数据错误 => ResourceManager.GetString("监控数据错误", resourceCulture);

		/// <summary>
		///   查找类似 移除组失败 的本地化字符串。
		/// </summary>
		public static string 移除组失败 => ResourceManager.GetString("移除组失败", resourceCulture);

		/// <summary>
		///   查找类似 第 的本地化字符串。
		/// </summary>
		public static string 第 => ResourceManager.GetString("第", resourceCulture);

		/// <summary>
		///   查找类似 类型 的本地化字符串。
		/// </summary>
		public static string 类型 => ResourceManager.GetString("类型", resourceCulture);

		/// <summary>
		///   查找类似 类型错误 的本地化字符串。
		/// </summary>
		public static string 类型错误 => ResourceManager.GetString("类型错误", resourceCulture);

		/// <summary>
		///   查找类似 组名不存在 的本地化字符串。
		/// </summary>
		public static string 组名不存在 => ResourceManager.GetString("组名不存在", resourceCulture);

		/// <summary>
		///   查找类似 组名已存在 的本地化字符串。
		/// </summary>
		public static string 组名已存在 => ResourceManager.GetString("组名已存在", resourceCulture);

		/// <summary>
		///   查找类似 获取成功 的本地化字符串。
		/// </summary>
		public static string 获取成功 => ResourceManager.GetString("获取成功", resourceCulture);

		/// <summary>
		///   查找类似 行 的本地化字符串。
		/// </summary>
		public static string 行 => ResourceManager.GetString("行", resourceCulture);

		/// <summary>
		///   查找类似 表不存在 的本地化字符串。
		/// </summary>
		public static string 表不存在 => ResourceManager.GetString("表不存在", resourceCulture);

		/// <summary>
		///   查找类似 表已经存在 的本地化字符串。
		/// </summary>
		public static string 表已经存在 => ResourceManager.GetString("表已经存在", resourceCulture);

		/// <summary>
		///   查找类似 解析异常 的本地化字符串。
		/// </summary>
		public static string 解析异常 => ResourceManager.GetString("解析异常", resourceCulture);

		/// <summary>
		///   查找类似 设备不存在 的本地化字符串。
		/// </summary>
		public static string 设备不存在 => ResourceManager.GetString("设备不存在", resourceCulture);

		/// <summary>
		///   查找类似 语言设置异常 的本地化字符串。
		/// </summary>
		public static string 语言设置异常 => ResourceManager.GetString("语言设置异常", resourceCulture);

		/// <summary>
		///   查找类似 语言设置成功 的本地化字符串。
		/// </summary>
		public static string 语言设置成功 => ResourceManager.GetString("语言设置成功", resourceCulture);

		/// <summary>
		///   查找类似 语言重复设置 的本地化字符串。
		/// </summary>
		public static string 语言重复设置 => ResourceManager.GetString("语言重复设置", resourceCulture);

		/// <summary>
		///   查找类似 请指定终端地址 的本地化字符串。
		/// </summary>
		public static string 请指定终端地址 => ResourceManager.GetString("请指定终端地址", resourceCulture);

		/// <summary>
		///   查找类似 请求成功 的本地化字符串。
		/// </summary>
		public static string 请求成功 => ResourceManager.GetString("请求成功", resourceCulture);

		/// <summary>
		///   查找类似 读取失败 的本地化字符串。
		/// </summary>
		public static string 读取失败 => ResourceManager.GetString("读取失败", resourceCulture);

		/// <summary>
		///   查找类似 读取异常 的本地化字符串。
		/// </summary>
		public static string 读取异常 => ResourceManager.GetString("读取异常", resourceCulture);

		/// <summary>
		///   查找类似 连接失败 的本地化字符串。
		/// </summary>
		public static string 连接失败 => ResourceManager.GetString("连接失败", resourceCulture);

		/// <summary>
		///   查找类似 连接超时 的本地化字符串。
		/// </summary>
		public static string 连接超时 => ResourceManager.GetString("连接超时", resourceCulture);

		/// <summary>
		///   查找类似 断线重连失败，连接超时，重连{0}次 的本地化字符串。
		/// </summary>
		public static string 重连失败 => ResourceManager.GetString("重连失败", resourceCulture);

		/// <summary>
		///   查找类似 断线重连异常 的本地化字符串。
		/// </summary>
		public static string 重连异常 => ResourceManager.GetString("重连异常", resourceCulture);

		/// <summary>
		///   查找类似 断线重连成功 的本地化字符串。
		/// </summary>
		public static string 重连成功 => ResourceManager.GetString("重连成功", resourceCulture);

		/// <summary>
		///   查找类似 错误 的本地化字符串。
		/// </summary>
		public static string 错误 => ResourceManager.GetString("错误", resourceCulture);

		internal Language()
		{
		}
	}
}
namespace Snet.Core.virtualAddress
{
	/// <summary>
	/// 虚拟地址类。
	/// <para>
	/// 用于模拟虚拟点数据，支持以下模式：<br />
	/// - <see cref="F:Snet.Model.enum.AddressType.VirtualDynamic_Random" />       随机值（不限定范围）<br />
	/// - <see cref="F:Snet.Model.enum.AddressType.VirtualDynamic_RandomScope" />  随机值（限定范围）<br />
	/// - <see cref="F:Snet.Model.enum.AddressType.VirtualDynamic_Order" />        顺序增长<br />
	/// - <see cref="F:Snet.Model.enum.AddressType.VirtualDynamic_OrderScope" />   顺序增长（带范围循环）<br />
	/// - <see cref="F:Snet.Model.enum.AddressType.VirtualStatic" />               固定值
	/// </para>
	/// <para>
	/// 配置参数通过 <c>AddressName</c> 中的花括号传递，格式为 <c>{参数}</c>：<br />
	/// - Random: <c>{间隔毫秒数}</c>，如 <c>MyPoint{500}</c><br />
	/// - RandomScope: <c>{间隔,最小值^最大值}</c>，如 <c>MyPoint{200,0^100}</c><br />
	/// - Order: <c>{间隔,步长}</c>，如 <c>MyPoint{1000,0.5}</c><br />
	/// - OrderScope: <c>{间隔,步长,最小值^最大值}</c>，如 <c>MyPoint{500,1,0^100}</c>
	/// </para>
	/// </summary>
	public class VirtualAddress : CoreUnify<VirtualAddress, VirtualAddressData>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 最小任务间隔（毫秒）。防止 interval ≤ 0 导致 CPU 空转
		/// </summary>
		private const int MinInterval = 10;

		/// <summary>
		/// 配置参数的正则表达式：匹配花括号 {参数} 中的内容
		/// </summary>
		private static readonly Regex ConfigPattern = new Regex("\\{([^}]*)\\}", RegexOptions.Compiled);

		/// <summary>
		/// 虚拟点的当前值
		/// </summary>
		private object? _value;

		/// <summary>
		/// 值读写的同步锁。
		/// 所有对 _value 的读写操作都必须在此锁的保护下进行，
		/// 确保 read-then-write 操作的原子性。
		/// </summary>
		private readonly object _syncLock;

		/// <summary>
		/// 后台任务的取消令牌源。
		/// 用于控制所有动态值生成任务的生命周期。
		/// </summary>
		private CancellationTokenSource? _tokenSource;

		/// <summary>
		/// 每线程独立的 Random 实例，避免锁竞争。
		/// ThreadLocal 确保每个线程拥有自己的 Random，
		/// 而 Task.Run 中的线程池线程各自创建独立实例。
		/// </summary>
		private readonly ThreadLocal<Random> _threadRandom;

		/// <summary>
		/// 【新增】释放状态标志。
		/// 0 = 未释放，1 = 已释放或正在释放。
		/// 使用 Interlocked 保证线程安全。
		/// </summary>
		private int _disposed;

		/// <summary>
		/// 获取当前线程的 Random 实例
		/// </summary>
		private Random Random => _threadRandom.Value;

		/// <summary>
		/// 日志前缀，包含地址名称便于定位
		/// </summary>
		private string LogPrefix => "虚拟地址 - [ " + base.basics?.AddressName + " ]";

		/// <summary>
		/// 无参构造函数（供框架反射使用）
		/// </summary>
		public VirtualAddress()
		{
			_syncLock = new object();
			_threadRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
			base..ctor();
		}

		/// <summary>
		/// 初始化虚拟地址实例。
		/// <para>
		/// 根据 AddressType 和 AddressName 中的花括号配置参数，
		/// 启动对应的后台动态值生成任务。
		/// </para>
		/// </summary>
		/// <param name="basics">虚拟地址基础配置</param>
		/// <exception cref="T:System.ArgumentNullException">basics 为 null</exception>
		public VirtualAddress(VirtualAddressData basics)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected I4, but got Unknown
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			_syncLock = new object();
			_threadRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
			base..ctor(basics);
			_tokenSource = new CancellationTokenSource();
			try
			{
				string text = ExtractConfigFromName(basics.AddressName);
				AddressType addressType = basics.AddressType;
				switch (addressType - 1)
				{
				case 1:
				{
					int interval4 = ParseInt(text, 1000);
					StartDynamicRandomTask(ValidateInterval(interval4));
					break;
				}
				case 2:
					var (min2, max2, interval3) = ParseRandomScopeConfig(text);
					StartDynamicRandomScopeTask(min2, max2, ValidateInterval(interval3));
					break;
				case 3:
					var (step2, interval2) = ParseOrderConfig(text);
					StartDynamicOrderTask(step2, ValidateInterval(interval2));
					break;
				case 4:
					var (min, max, step, interval) = ParseOrderScopeConfig(text);
					StartDynamicOrderScopeTask(min, max, step, ValidateInterval(interval));
					break;
				case 0:
					lock (_syncLock)
					{
						_value = GenerateRandomValueByDataType();
						break;
					}
				default:
					LogHelper.Error($"{LogPrefix} 不支持的虚拟地址类型: {basics.AddressType}", "VirtualAddress", (Exception)null, true);
					break;
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error(LogPrefix + " 初始化异常：" + ex.Message, "VirtualAddress", ex, true);
				throw;
			}
		}

		/// <summary>
		/// 读取虚拟点的当前值
		/// </summary>
		/// <returns>当前存储的值；已释放时返回 null</returns>
		public object? Read()
		{
			lock (_syncLock)
			{
				return _value;
			}
		}

		/// <summary>
		/// 写入虚拟点的值（覆盖当前值）
		/// </summary>
		/// <param name="value">要写入的新值</param>
		/// <returns>写入成功返回 true；已释放时返回 false</returns>
		public bool Write(object value)
		{
			if (Volatile.Read(in _disposed) != 0)
			{
				return false;
			}
			lock (_syncLock)
			{
				_value = value;
			}
			return true;
		}

		/// <summary>
		/// 从地址名称中提取花括号内的配置文本。
		/// <para>
		/// 示例：<c>"MyPoint{500,0^100}"</c> → <c>"500,0^100"</c>
		/// </para>
		/// </summary>
		/// <param name="addressName">地址名称</param>
		/// <returns>花括号内的内容；无花括号时返回空字符串</returns>
		private static string ExtractConfigFromName(string? addressName)
		{
			if (string.IsNullOrWhiteSpace(addressName))
			{
				return string.Empty;
			}
			Match match = ConfigPattern.Match(addressName);
			if (!match.Success)
			{
				return string.Empty;
			}
			return match.Groups[1].Value;
		}

		/// <summary>
		/// 安全解析 int，失败时返回默认值
		/// </summary>
		private static int ParseInt(string text, int defaultValue)
		{
			if (!int.TryParse(text, out var result))
			{
				return defaultValue;
			}
			return result;
		}

		/// <summary>
		/// 安全解析 float，失败时返回默认值
		/// </summary>
		private static float ParseFloat(string text, float defaultValue)
		{
			if (!float.TryParse(text, out var result))
			{
				return defaultValue;
			}
			return result;
		}

		/// <summary>
		/// 解析 RandomScope 配置：{间隔,最小值^最大值}
		/// </summary>
		private static (string Min, string Max, int Interval) ParseRandomScopeConfig(string configText)
		{
			if (string.IsNullOrEmpty(configText))
			{
				return (Min: "0", Max: "100", Interval: 1000);
			}
			string[] array = configText.Split(',');
			if (array.Length >= 2 && array[1].Contains('^'))
			{
				int item = (int.TryParse(array[0], out var result) ? result : 1000);
				string[] array2 = array[1].Split('^');
				if (array2.Length >= 2)
				{
					return (Min: array2[0], Max: array2[1], Interval: item);
				}
			}
			return (Min: "0", Max: "100", Interval: 1000);
		}

		/// <summary>
		/// 解析 Order 配置：{间隔,步长}
		/// </summary>
		private static (float Step, int Interval) ParseOrderConfig(string configText)
		{
			if (string.IsNullOrEmpty(configText))
			{
				return (Step: 1f, Interval: 1000);
			}
			string[] array = configText.Split(',');
			if (array.Length >= 2 && int.TryParse(array[0], out var result) && float.TryParse(array[1], out var result2))
			{
				return (Step: result2, Interval: result);
			}
			return (Step: 1f, Interval: 1000);
		}

		/// <summary>
		/// 解析 OrderScope 配置：{间隔,步长,最小值^最大值}
		/// </summary>
		private static (string Min, string Max, float Step, int Interval) ParseOrderScopeConfig(string configText)
		{
			if (string.IsNullOrEmpty(configText))
			{
				return (Min: "0", Max: "100", Step: 1f, Interval: 1000);
			}
			string[] array = configText.Split(',');
			if (array.Length >= 3 && array[2].Contains('^') && int.TryParse(array[0], out var result) && float.TryParse(array[1], out var result2))
			{
				string[] array2 = array[2].Split('^');
				if (array2.Length >= 2)
				{
					return (Min: array2[0], Max: array2[1], Step: result2, Interval: result);
				}
			}
			return (Min: "0", Max: "100", Step: 1f, Interval: 1000);
		}

		/// <summary>
		/// 校验间隔值，确保不低于最小值
		/// </summary>
		private static int ValidateInterval(int interval)
		{
			return Math.Max(interval, 10);
		}

		/// <summary>
		/// 启动动态随机值生成任务（不限定范围）。
		/// 按固定间隔生成随机值并更新到 _value。
		/// </summary>
		/// <param name="interval">值更新间隔（毫秒），已通过 ValidateInterval 校验</param>
		private void StartDynamicRandomTask(int interval)
		{
			CancellationToken token = _tokenSource.Token;
			Task.Run(async delegate
			{
				try
				{
					while (!token.IsCancellationRequested)
					{
						lock (_syncLock)
						{
							DataType dataType = base.basics.DataType;
							object value;
							switch (dataType - 2)
							{
							case 0:
								value = _value is bool flag && !flag;
								break;
							case 19:
							case 20:
								value = GenerateRandomString(Random.Next(1, 50));
								break;
							case 1:
								value = GenerateRandomDouble();
								break;
							case 2:
							case 3:
								value = GenerateRandomFloat();
								break;
							case 8:
							case 9:
								value = GenerateRandomInt32();
								break;
							case 12:
							case 13:
								value = GenerateRandomInt64();
								break;
							case 4:
							case 5:
								value = GenerateRandomInt16();
								break;
							case 14:
							case 15:
								value = GenerateRandomUInt64(0uL, ulong.MaxValue);
								break;
							case 10:
							case 11:
								value = GenerateRandomUInt32();
								break;
							case 6:
							case 7:
								value = GenerateRandomUInt16(0);
								break;
							case 16:
							case 17:
								value = GenerateRandomDateTime(DateTime.Parse("2000-01-01"), DateTime.Now);
								break;
							case 18:
								value = GenerateRandomTime(DateTime.Parse("2000-01-01"), DateTime.Now);
								break;
							default:
								value = _value ?? ((object)0);
								break;
							}
							_value = value;
						}
						await Task.Delay(interval, token).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex2)
				{
					await LogHelper.ErrorAsync(LogPrefix + " 动态随机值异常：" + ex2.Message, "VirtualAddress", ex2, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 启动动态随机范围值生成任务。
		/// 按固定间隔在 [min, max] 范围内生成随机值。
		/// </summary>
		/// <param name="min">最小值（字符串形式）</param>
		/// <param name="max">最大值（字符串形式）</param>
		/// <param name="interval">值更新间隔（毫秒）</param>
		private void StartDynamicRandomScopeTask(string min, string max, int interval)
		{
			CancellationToken token = _tokenSource.Token;
			Task.Run(async delegate
			{
				try
				{
					while (!token.IsCancellationRequested)
					{
						lock (_syncLock)
						{
							DataType dataType = base.basics.DataType;
							object value;
							switch (dataType - 3)
							{
							case 0:
								value = GenerateRandomDouble(double.Parse(min), double.Parse(max));
								break;
							case 1:
							case 2:
								value = GenerateRandomFloat(float.Parse(min), float.Parse(max));
								break;
							case 7:
							case 8:
								value = GenerateRandomInt32(int.Parse(min), int.Parse(max) + 1);
								break;
							case 11:
							case 12:
								value = GenerateRandomInt64(long.Parse(min), long.Parse(max) + 1);
								break;
							case 3:
							case 4:
								value = GenerateRandomInt16(short.Parse(min), (short)(short.Parse(max) + 1));
								break;
							case 13:
							case 14:
								value = GenerateRandomUInt64(ulong.Parse(min), ulong.Parse(max) + 1);
								break;
							case 9:
							case 10:
								value = GenerateRandomUInt32(uint.Parse(min), uint.Parse(max) + 1);
								break;
							case 5:
							case 6:
								value = GenerateRandomUInt16(ushort.Parse(min), (ushort)(ushort.Parse(max) + 1));
								break;
							case 15:
							case 16:
								value = GenerateRandomDateTime(DateTime.Parse(min), DateTime.Parse(max));
								break;
							case 17:
								value = GenerateRandomTime(DateTime.Parse(min), DateTime.Parse(max));
								break;
							default:
								value = _value ?? ((object)0);
								break;
							}
							_value = value;
						}
						await Task.Delay(interval, token).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex2)
				{
					await LogHelper.ErrorAsync(LogPrefix + " 动态随机范围值异常：" + ex2.Message, "VirtualAddress", ex2, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 启动动态顺序值生成任务（不带范围限制）。
		/// 按固定间隔以固定步长递增当前值。
		/// </summary>
		/// <param name="step">每次增加的步长</param>
		/// <param name="interval">值更新间隔（毫秒）</param>
		private void StartDynamicOrderTask(float step, int interval)
		{
			CancellationToken token = _tokenSource.Token;
			Task.Run(async delegate
			{
				try
				{
					while (!token.IsCancellationRequested)
					{
						try
						{
							lock (_syncLock)
							{
								DataType dataType = base.basics.DataType;
								object value;
								switch (dataType - 3)
								{
								case 0:
									value = ((_value is double num6) ? (num6 + (double)step) : ((double)step));
									break;
								case 1:
								case 2:
									value = ((_value is float num) ? (num + step) : step);
									break;
								case 7:
								case 8:
									value = ((_value is int num8) ? checked(num8 + (int)step) : ((int)step));
									break;
								case 11:
								case 12:
									value = ((_value is long num5) ? checked(num5 + (long)step) : ((long)step));
									break;
								case 3:
								case 4:
									value = ((_value is short num7) ? checked((short)((float)num7 + step)) : ((short)step));
									break;
								case 13:
								case 14:
									value = ((_value is ulong num4) ? checked(num4 + (ulong)step) : ((ulong)step));
									break;
								case 9:
								case 10:
									value = ((_value is uint num3) ? checked(num3 + (uint)step) : ((uint)step));
									break;
								case 5:
								case 6:
									value = ((_value is ushort num2) ? checked((ushort)((float)unchecked((int)num2) + step)) : ((ushort)step));
									break;
								case 15:
								case 16:
									value = ((_value is DateTime dateTime2) ? dateTime2.AddSeconds(step) : DateTime.Now);
									break;
								case 17:
									value = ((_value is DateTime dateTime) ? dateTime.AddSeconds(step) : DateTime.Now);
									break;
								default:
									value = _value ?? ((object)0);
									break;
								}
								_value = value;
							}
						}
						catch (Exception ex) when (((ex is InvalidCastException || ex is OverflowException) ? 1 : 0) != 0)
						{
							await LogHelper.ErrorAsync(LogPrefix + " 顺序值溢出，重置为零：" + ex.Message, "VirtualAddress", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
							lock (_syncLock)
							{
								_value = 0;
							}
						}
						await Task.Delay(interval, token).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex3)
				{
					await LogHelper.ErrorAsync(LogPrefix + " 动态顺序值异常：" + ex3.Message, "VirtualAddress", ex3, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 启动动态顺序值生成任务（带范围限制，超过最大值后回到最小值）。
		/// </summary>
		/// <param name="min">最小值（字符串形式）</param>
		/// <param name="max">最大值（字符串形式）</param>
		/// <param name="step">每次增加的步长</param>
		/// <param name="interval">值更新间隔（毫秒）</param>
		private void StartDynamicOrderScopeTask(string min, string max, float step, int interval)
		{
			CancellationToken token = _tokenSource.Token;
			Task.Run(async delegate
			{
				try
				{
					double minVal = double.Parse(min);
					double maxVal = double.Parse(max);
					Type targetType = GetSystemType(base.basics.DataType);
					while (!token.IsCancellationRequested)
					{
						try
						{
							lock (_syncLock)
							{
								double num = ((_value == null) ? minVal : Convert.ToDouble(_value)) + (double)step;
								if (num > maxVal)
								{
									num = minVal;
								}
								_value = Convert.ChangeType(num, targetType);
							}
						}
						catch (Exception ex) when (((ex is FormatException || ex is InvalidCastException || ex is OverflowException) ? 1 : 0) != 0)
						{
							await LogHelper.ErrorAsync(LogPrefix + " 顺序范围值异常，重置：" + ex.Message, "VirtualAddress", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
							lock (_syncLock)
							{
								_value = Convert.ChangeType(minVal, targetType);
							}
						}
						await Task.Delay(interval, token).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex3)
				{
					await LogHelper.ErrorAsync(LogPrefix + " 动态顺序范围值异常：" + ex3.Message, "VirtualAddress", ex3, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 根据 DataType 生成对应的随机值（用于 VirtualStatic 模式）
		/// </summary>
		private object? GenerateRandomValueByDataType()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected I4, but got Unknown
			DataType dataType = base.basics.DataType;
			switch (dataType - 2)
			{
			case 0:
				return Random.Next(2) == 1;
			case 19:
			case 20:
				return GenerateRandomString(Random.Next(1, 50));
			case 1:
				return GenerateRandomDouble();
			case 2:
			case 3:
				return GenerateRandomFloat();
			case 8:
			case 9:
				return GenerateRandomInt32();
			case 12:
			case 13:
				return GenerateRandomInt64();
			case 4:
			case 5:
				return GenerateRandomInt16();
			case 14:
			case 15:
				return GenerateRandomUInt64(0uL, ulong.MaxValue);
			case 10:
			case 11:
				return GenerateRandomUInt32();
			case 6:
			case 7:
				return GenerateRandomUInt16(0);
			case 16:
			case 17:
				return GenerateRandomDateTime(DateTime.Parse("2000-01-01"), DateTime.Now);
			case 18:
				return GenerateRandomTime(DateTime.Parse("2000-01-01"), DateTime.Now);
			default:
				return null;
			}
		}

		/// <summary>
		/// 将 DataType 枚举转换为对应的 System.Type 类型
		/// </summary>
		private static Type GetSystemType(DataType dataType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected I4, but got Unknown
			switch (dataType - 2)
			{
			case 0:
				return typeof(bool);
			case 19:
				return typeof(string);
			case 20:
				return typeof(char);
			case 1:
				return typeof(double);
			case 2:
			case 3:
				return typeof(float);
			case 8:
			case 9:
				return typeof(int);
			case 12:
			case 13:
				return typeof(long);
			case 4:
			case 5:
				return typeof(short);
			case 14:
			case 15:
				return typeof(ulong);
			case 10:
			case 11:
				return typeof(uint);
			case 6:
			case 7:
				return typeof(ushort);
			case 16:
			case 17:
				return typeof(DateTime);
			case 18:
				return typeof(DateTime);
			default:
				return typeof(object);
			}
		}

		/// <summary>
		/// 生成随机时间值（基于秒精度）
		/// </summary>
		private DateTime GenerateRandomTime(DateTime minTime, DateTime maxTime)
		{
			double totalSeconds = (maxTime - minTime).TotalSeconds;
			return minTime.AddSeconds(Random.NextDouble() * totalSeconds);
		}

		/// <summary>
		/// 生成随机日期时间值（基于 Tick 精度）
		/// </summary>
		private DateTime GenerateRandomDateTime(DateTime minTime, DateTime maxTime)
		{
			long num = maxTime.Ticks - minTime.Ticks;
			if (num <= 0)
			{
				return minTime;
			}
			return new DateTime(minTime.Ticks + (long)(Random.NextDouble() * (double)num));
		}

		/// <summary>
		/// 生成随机 Int16 值
		/// </summary>
		private short GenerateRandomInt16(short min = short.MinValue, short max = short.MaxValue)
		{
			return (short)Random.Next(min, max + 1);
		}

		/// <summary>
		/// 生成随机 UInt16 值
		/// </summary>
		private ushort GenerateRandomUInt16(ushort min = 0, ushort max = ushort.MaxValue)
		{
			return (ushort)Random.Next(min, max + 1);
		}

		/// <summary>
		/// 生成随机 Int32 值
		/// </summary>
		private int GenerateRandomInt32(int min = int.MinValue, int max = int.MaxValue)
		{
			return Random.Next(min, max);
		}

		/// <summary>
		/// 生成随机 UInt32 值。
		/// <para>
		/// <b>修复内容：</b><br />
		/// 原代码使用 <c>(uint)Random.Next((int)min, (int)Math.Min(max+1, int.MaxValue))</c>，
		/// 当 <c>min &gt; int.MaxValue</c> 时 <c>(int)min</c> 为负数，导致 <c>Random.Next</c>
		/// 参数非法抛出异常。且范围被截断到 <c>int.MaxValue</c>，无法覆盖完整的 uint 范围。<br /><br />
		/// 修复方案：对于大范围值（跨过 int.MaxValue），使用两次 Random.Next 组合生成。
		/// 对于小范围值，直接使用 Random.Next。
		/// </para>
		/// </summary>
		private uint GenerateRandomUInt32(uint min = 0u, uint max = uint.MaxValue)
		{
			long num = (long)max - (long)min + 1;
			if (num <= 0)
			{
				return min;
			}
			if (num <= int.MaxValue)
			{
				return (uint)(Random.Next(0, (int)num) + min);
			}
			return (uint)(Random.NextInt64(0L, num) + min);
		}

		/// <summary>
		/// 生成随机 Int64 值
		/// </summary>
		private long GenerateRandomInt64(long min = long.MinValue, long max = long.MaxValue)
		{
			return Random.NextInt64(min, max);
		}

		/// <summary>
		/// 生成随机 UInt64 值。
		/// <para>
		/// <b>修复内容：</b><br />
		/// 原代码 <c>(ulong)Random.NextInt64((long)min, (long)Math.Min(max+1, long.MaxValue))</c>
		/// 存在与 UInt32 相同的问题。且 ulong 范围（0 ~ 18,446,744,073,709,551,615）
		/// 完全无法映射到 long 范围（-9,223,372,036,854,775,808 ~ 9,223,372,036,854,775,807）。<br /><br />
		/// 修复方案：对于能映射到 long 正半部分的范围，直接使用 NextInt64；
		/// 对于超大范围，使用两次 NextInt64 组合生成 64 位随机数。
		/// </para>
		/// </summary>
		private ulong GenerateRandomUInt64(ulong min = 0uL, ulong max = ulong.MaxValue)
		{
			ulong num = max - min + 1;
			if (num == 0L)
			{
				Span<byte> span = stackalloc byte[8];
				Random.NextBytes(span);
				return BitConverter.ToUInt64(span) + min;
			}
			if (num <= long.MaxValue)
			{
				return (ulong)Random.NextInt64(0L, (long)num) + min;
			}
			Span<byte> span2 = stackalloc byte[8];
			Random.NextBytes(span2);
			return BitConverter.ToUInt64(span2) % num + min;
		}

		/// <summary>
		/// 生成随机 Double 值。
		/// <para>
		/// <b>修复内容：</b>原代码默认范围 [-999999, 999999]，
		/// 调整为更合理的默认范围。Math.Round 保留 6 位小数。
		/// </para>
		/// </summary>
		private double GenerateRandomDouble(double min = -999999.0, double max = 999999.0)
		{
			return Math.Round(min + Random.NextDouble() * (max - min), 6);
		}

		/// <summary>
		/// 生成随机 Float 值。
		/// <para>
		/// <b>修复内容：</b>原代码 <c>(float)Random.NextDouble()</c> 先生成 double 再强转 float，
		/// 多了一次不必要的精度丢失。<br />
		/// 修复后直接使用 Random.NextDouble()（返回 double）参与计算，
		/// 最终结果强转 float，与原效果一致但语义更清晰。
		/// </para>
		/// </summary>
		private float GenerateRandomFloat(float min = -999999f, float max = 999999f)
		{
			return (float)Math.Round((double)min + Random.NextDouble() * (double)(max - min), 6);
		}

		/// <summary>
		/// 生成指定长度的随机字符串
		/// </summary>
		/// <param name="length">字符串长度</param>
		/// <param name="validChars">可选字符集</param>
		/// <returns>随机字符串</returns>
		private string GenerateRandomString(int length, string validChars = "abcdefghijklmnopqrstuvwxyz0123456789")
		{
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append(validChars[Random.Next(validChars.Length)]);
			}
			return stringBuilder.ToString();
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
			{
				return;
			}
			if (_tokenSource != null)
			{
				try
				{
					_tokenSource.Cancel();
				}
				catch
				{
				}
			}
			_threadRandom?.Dispose();
			CancellationTokenSource cts = _tokenSource;
			_tokenSource = null;
			if (cts != null)
			{
				Task.Delay(200).ContinueWith(delegate
				{
					try
					{
						cts.Dispose();
					}
					catch
					{
					}
				});
			}
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
			{
				return;
			}
			if (_tokenSource != null)
			{
				try
				{
					_tokenSource.Cancel();
				}
				catch
				{
				}
			}
			_threadRandom?.Dispose();
			CancellationTokenSource cts = _tokenSource;
			_tokenSource = null;
			if (cts != null)
			{
				try
				{
					await Task.Delay(200).ConfigureAwait(continueOnCapturedContext: false);
					cts.Dispose();
				}
				catch
				{
				}
			}
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
	/// <summary>
	/// 虚拟地址的数据
	/// </summary>
	public class VirtualAddressData
	{
		[CompilerGenerated]
		private AddressType <AddressType>k__BackingField;

		[CompilerGenerated]
		private DataType <DataType>k__BackingField;

		/// <summary>
		/// 地址名称
		/// </summary>
		public string? AddressName { get; set; }

		/// <summary>
		/// 地址类型
		/// </summary>
		public AddressType AddressType
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <AddressType>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<AddressType>k__BackingField = value;
			}
		}

		/// <summary>
		/// 数据类型
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DataType DataType
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <DataType>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<DataType>k__BackingField = value;
			}
		}
	}
	/// <summary>
	/// 虚拟地址管理器
	/// <para>
	/// - 主要用于管理「虚拟地址点」，支持初始化、读写操作。<br />
	/// - 不支持订阅模式下的数据触发，只能主动 Read / Write。<br />
	/// - 支持多种虚拟点类型（静态、动态随机、动态顺序等）。<br />
	/// - 第一次访问时自动初始化，后续写入才会生效。<br />
	/// - 注意：不支持数组数据类型模拟。
	/// </para>
	/// </summary>
	public class VirtualAddressManage : IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 虚拟地址容器。
		/// key: 地址名称，value: (虚拟地址对象, 地址类型, 数据类型)
		/// 使用 ConcurrentDictionary 保证多线程并发安全。
		/// </summary>
		private readonly ConcurrentDictionary<string, (VirtualAddress virtualAddress, AddressType addressType, DataType dataType)> _container = new ConcurrentDictionary<string, (VirtualAddress, AddressType, DataType)>();

		/// <summary>
		/// 释放状态标志：0 = 未释放，1 = 已释放或正在释放。
		/// 使用 Interlocked 保证线程安全的原子读写，替代原 sync lock 模式。
		/// </summary>
		private int _disposed;

		/// <summary>
		/// 初始化虚拟地址（如果为虚拟点则加入容器）
		/// </summary>
		/// <param name="details">地址详情</param>
		/// <param name="IsVA">输出：true 表示是虚拟地址</param>
		public void InitVirtualAddress(AddressDetails details, out bool IsVA)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (details == null)
			{
				throw new ArgumentNullException("details");
			}
			if (IsVirtualAddress(details))
			{
				SetVirtualAddress(new VirtualAddressData
				{
					AddressName = details.AddressName,
					DataType = details.AddressDataType,
					AddressType = details.AddressType
				});
				IsVA = true;
			}
			else
			{
				IsVA = false;
			}
		}

		/// <summary>
		/// 设置虚拟地址（使用原子 CAS 消除 TOCTOU 竞态）：
		/// - 地址不存在 → 原子地添加
		/// - 地址存在但配置不同 → 原子地替换并释放旧对象
		/// - 地址存在且配置相同 → 不做任何处理
		/// <para>
		/// <b>原竞态：</b>TryGetValue + TryRemove + Add 三步非原子，<br />
		/// 另一个线程可能在 TryRemove 和 Add 之间插入新值。
		/// 修复后：使用 AddOrUpdate 一次性完成"检查+替换+释放"原子操作。
		/// </para>
		/// </summary>
		private void SetVirtualAddress(VirtualAddressData addressData)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (addressData == null)
			{
				throw new ArgumentNullException("addressData");
			}
			(VirtualAddress, AddressType AddressType, DataType DataType) newEntry = (CoreUnify<VirtualAddress, VirtualAddressData>.Instance(addressData), AddressType: addressData.AddressType, DataType: addressData.DataType);
			_container.AddOrUpdate(addressData.AddressName, (string _) => ((VirtualAddress virtualAddress, AddressType addressType, DataType dataType))newEntry, delegate(string _, (VirtualAddress virtualAddress, AddressType addressType, DataType dataType) existing)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				if (existing.addressType != addressData.AddressType || existing.dataType != addressData.DataType)
				{
					existing.virtualAddress.Dispose();
					return ((VirtualAddress virtualAddress, AddressType addressType, DataType dataType))newEntry;
				}
				newEntry.Item1.Dispose();
				return existing;
			});
		}

		/// <summary>
		/// 获取虚拟地址对象
		/// </summary>
		private VirtualAddress? GetVirtualAddress(string addressName)
		{
			if (string.IsNullOrEmpty(addressName))
			{
				throw new ArgumentNullException("addressName");
			}
			if (!_container.TryGetValue(addressName, out (VirtualAddress, AddressType, DataType) value))
			{
				return null;
			}
			return value.Item1;
		}

		/// <summary>
		/// 判断地址详情是否属于虚拟地址类型
		/// </summary>
		public bool IsVirtualAddress(AddressDetails details)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			if (details == null)
			{
				throw new ArgumentNullException("details");
			}
			AddressType addressType = details.AddressType;
			if (addressType - 1 <= 4)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 判断指定名称是否存在于容器中
		/// </summary>
		public bool IsVirtualAddress(string addressName)
		{
			if (string.IsNullOrEmpty(addressName))
			{
				throw new ArgumentNullException("addressName");
			}
			return _container.ContainsKey(addressName);
		}

		/// <summary>
		/// 读取虚拟点的值（根据地址详情）
		/// </summary>
		public object? Read(AddressDetails details)
		{
			if (details == null)
			{
				throw new ArgumentNullException("details");
			}
			return GetVirtualAddress(details.AddressName)?.Read();
		}

		/// <summary>
		/// 读取虚拟点的值（根据地址名）
		/// </summary>
		public object? Read(string addressName)
		{
			if (string.IsNullOrEmpty(addressName))
			{
				throw new ArgumentNullException("addressName");
			}
			return GetVirtualAddress(addressName)?.Read();
		}

		/// <summary>
		/// 写入虚拟点的值（根据地址详情）
		/// </summary>
		public bool Write(AddressDetails details, object value)
		{
			if (details == null)
			{
				throw new ArgumentNullException("details");
			}
			return GetVirtualAddress(details.AddressName)?.Write(value) ?? false;
		}

		/// <summary>
		/// 写入虚拟点的值（根据地址名）
		/// </summary>
		public bool Write(string addressName, object value)
		{
			if (string.IsNullOrEmpty(addressName))
			{
				throw new ArgumentNullException("addressName");
			}
			return GetVirtualAddress(addressName)?.Write(value) ?? false;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			if (Interlocked.Exchange(ref _disposed, 1) != 0)
			{
				return;
			}
			foreach (var value in _container.Values)
			{
				try
				{
					value.virtualAddress.Dispose();
				}
				catch
				{
				}
			}
			_container.Clear();
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		/// <remarks>
		/// 异步释放所有 VirtualAddress 实例，等待后台任务完全取消。
		/// 替代原来的同步 Dispose(true) 调用。
		/// </remarks>
		public async ValueTask DisposeAsync()
		{
			if (Interlocked.Exchange(ref _disposed, 1) != 0)
			{
				return;
			}
			foreach (var value in _container.Values)
			{
				try
				{
					await value.virtualAddress.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch
				{
				}
			}
			_container.Clear();
			GC.SuppressFinalize(this);
		}
	}
}
namespace Snet.Core.subscription
{
	/// <summary>
	/// 轮询数据
	/// </summary>
	public class SubscribeData
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

			/// <summary>
			/// 点位，可为空，可后期赋值
			/// </summary>
			[Description("点位，可为空，可后期赋值")]
			public Address? Address { get; set; }

			/// <summary>
			/// 执行方法的异步委托，读取方法( ReadAsync )，每个通信库都应该存在<br />
			/// Address：请求参数<br />
			/// Address：取消通知<br />
			/// OperateResult：操作结果
			/// </summary>
			[Description("执行方法的异步委托，读取方法( ReadAsync )，每个通信库都应该存在")]
			public Func<Address, CancellationToken, Task<OperateResult>>? FunctionAsync { get; set; }
		}

		/// <summary>
		/// 订阅核心数据
		/// </summary>
		public class SCData
		{
			/// <summary>
			/// 处理间隔；<br />
			/// 多少毫秒读取一次点位；<br />
			/// 越小CPU占用越高，速度越快<br />
			/// 采集协议外部可以自定义重写
			/// </summary>
			[Category("基础数据")]
			[Description("处理间隔")]
			[Unit("ms")]
			[Display(/*Could not decode attribute arguments.*/)]
			public virtual int HandleInterval { get; set; } = 1000;

			/// <summary>
			/// 变化抛出<br />
			/// true : 数据变化抛出，只抛出变化项<br /> 
			/// false : 则为实时数据<br />
			/// 数据库查询，此项无效
			/// </summary>
			[Description("变化抛出")]
			[Display(/*Could not decode attribute arguments.*/)]
			public bool ChangeOut { get; set; } = true;

			/// <summary>
			/// 变化项与未变项一同抛出<br />
			/// 当 ChangeOut 为 true 此项生效<br />
			/// true : 当节点数据变化，有一项数据未变化，把未变项与变化项一同抛出，确保此批数据的完整性<br />
			/// false : 只抛出变化项<br />
			/// 数据库订阅，此项无效
			/// </summary>
			[Description("变化项与未变项一同抛出")]
			[Display(/*Could not decode attribute arguments.*/)]
			public bool AllOut { get; set; }

			/// <summary>
			/// 任务数量；<br />
			/// 一个任务属于一个队列
			/// </summary>
			[Description("任务数量")]
			[Display(/*Could not decode attribute arguments.*/)]
			public int TaskNumber { get; set; } = 5;
		}
	}
	/// <summary>
	/// 自定义订阅操作<br />
	/// 只支持两种数据结果的自定义订阅<br />
	/// 1.ConcurrentDictionary《string, AddressValue》<br />
	/// 2.List《ConcurrentDictionary《string, AddressValue》》
	/// </summary>
	public class SubscribeOperate : CoreUnify<SubscribeOperate, SubscribeData.Basics>, ISubscribe, IEvent, IOn, IOff, IStatus, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 监控开关
		/// </summary>
		private CancellationTokenSource? MonitorSwitch;

		/// <summary>
		/// 任务集合
		/// </summary>
		private ConcurrentDictionary<CancellationTokenSource, Task>? TaskArray;

		/// <summary>
		/// 数据队列
		/// </summary>
		private Channel<AddressValue>? DataQueue;

		/// <summary>
		/// 别的操作进来是否继续轮询
		/// </summary>
		private bool GoOn = true;

		/// <summary>
		/// 数据缓存池
		/// </summary>
		private ConcurrentDictionary<string, AddressValue> DataCachePool = new ConcurrentDictionary<string, AddressValue>();

		/// <summary>
		/// 多组数据缓存池
		/// </summary>
		private List<ConcurrentDictionary<string, AddressValue>> DataCachePoolArray = new List<ConcurrentDictionary<string, AddressValue>>();

		private BoundedChannelOptions? p_Channel;

		/// <summary>
		/// 锁
		/// </summary>
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		/// <summary>
		/// 订阅服务
		/// </summary>
		private SubscribeService<AddressValue> subscribeService = CoreUnify<SubscribeService<AddressValue>, string>.Instance(ExtendMethod.ToUpperNString(Guid.NewGuid()));

		/// <summary>
		/// 数据队列包
		/// </summary>
		private Channel<AddressValue>? DataQueuePack;

		/// <summary>
		/// 数据队列包生命周期
		/// </summary>
		private CancellationTokenSource? DataQueuePackToken;

		/// <summary>
		/// 通道设置
		/// </summary>
		private BoundedChannelOptions channelOptions
		{
			get
			{
				BoundedChannelOptions boundedChannelOptions = p_Channel;
				if (boundedChannelOptions == null)
				{
					BoundedChannelOptions obj = new BoundedChannelOptions(65535)
					{
						FullMode = BoundedChannelFullMode.Wait,
						SingleReader = false,
						SingleWriter = false
					};
					BoundedChannelOptions boundedChannelOptions2 = obj;
					p_Channel = obj;
					boundedChannelOptions = boundedChannelOptions2;
				}
				return boundedChannelOptions;
			}
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public SubscribeOperate(SubscribeData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 执行轮询（极致优化版）
		/// </summary>
		private async Task PollingAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					try
					{
						if (!GoOn || base.basics.Address == null || base.basics.Address.AddressArray == null || base.basics.Address.AddressArray.Count <= 0)
						{
							await Task.Delay(base.basics.HandleInterval, token).ConfigureAwait(continueOnCapturedContext: false);
							continue;
						}
						_ = base.basics.Address.AddressArray;
						OperateResult result = null;
						await _semaphore.WaitAsync(token).ConfigureAwait(continueOnCapturedContext: false);
						try
						{
							if (base.basics.FunctionAsync == null)
							{
								result = OperateResult.CreateFailureResult("未设置读取函数", (int?)null);
							}
							result = await base.basics.FunctionAsync(base.basics.Address, token).ConfigureAwait(continueOnCapturedContext: false);
						}
						catch (Exception ex)
						{
							await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("自定义订阅轮询异常：" + ex.Message)).ConfigureAwait(continueOnCapturedContext: false);
						}
						finally
						{
							_semaphore.Release();
						}
						if (((BaseModel)result).Status)
						{
							object resultData = ((ResultModel)result).ResultData;
							ConcurrentDictionary<string, AddressValue> resultDict = resultData as ConcurrentDictionary<string, AddressValue>;
							if (resultDict == null)
							{
								List<ConcurrentDictionary<string, AddressValue>> resultList = resultData as List<ConcurrentDictionary<string, AddressValue>>;
								if (resultList != null && resultList.Count > 0)
								{
									if (!base.basics.ChangeOut)
									{
										await OnDataEventHandlerAsync(this, new EventDataResult(true, "实时数据", (object)resultList)).ConfigureAwait(continueOnCapturedContext: false);
									}
									else if (!Comparison(DataCachePoolArray, resultList))
									{
										await OnDataEventHandlerAsync(this, new EventDataResult(true, "存在变化数据", (object)resultList)).ConfigureAwait(continueOnCapturedContext: false);
										DataCachePoolArray = resultList;
									}
								}
							}
							else
							{
								if (resultDict.Count <= 0)
								{
									continue;
								}
								if (!base.basics.ChangeOut)
								{
									await OnDataEventHandlerAsync(this, new EventDataResult(true, "实时数据", (object)resultDict)).ConfigureAwait(continueOnCapturedContext: false);
								}
								else
								{
									if (Comparison(DataCachePool, resultDict))
									{
										continue;
									}
									if (base.basics.AllOut)
									{
										await OnDataEventHandlerAsync(this, new EventDataResult(true, "存在变化数据", (object)resultDict)).ConfigureAwait(continueOnCapturedContext: false);
									}
									else
									{
										foreach (AddressValue value in resultDict.Values)
										{
											await DataQueue.Writer.WriteAsync(value, token).ConfigureAwait(continueOnCapturedContext: false);
										}
									}
									DataCachePool = resultDict;
								}
								continue;
							}
							goto end_IL_0072;
						}
						await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("自定义订阅轮询异常：" + ((BaseModel)result).Message)).ConfigureAwait(continueOnCapturedContext: false);
						goto end_IL_003c;
						end_IL_0072:;
					}
					catch (Exception ex2)
					{
						await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("自定义订阅轮询异常：" + ex2.Message)).ConfigureAwait(continueOnCapturedContext: false);
					}
					end_IL_003c:;
				}
				finally
				{
					await Task.Delay(base.basics.HandleInterval, token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		/// <summary>
		/// 任务处理
		/// </summary>
		/// <param name="token">任务令牌</param>
		/// <returns>任务</returns>
		private async Task TaskPackHandleAsync(CancellationToken token)
		{
			try
			{
				while (await DataQueuePack.Reader.WaitToReadAsync(token).ConfigureAwait(continueOnCapturedContext: false))
				{
					ConcurrentDictionary<string, AddressValue> concurrentDictionary = new ConcurrentDictionary<string, AddressValue>();
					while (true)
					{
						if (!DataQueuePack.Reader.TryRead(out AddressValue queueData))
						{
							break;
						}
						if (queueData != null && !token.IsCancellationRequested)
						{
							concurrentDictionary.AddOrUpdate(((AddressDetails)queueData).AddressName, queueData, (string k, AddressValue v) => queueData);
						}
					}
					if (concurrentDictionary.Count > 0)
					{
						await OnDataEventHandlerAsync(this, new EventDataResult(true, "变化数据", (object)concurrentDictionary)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex3)
			{
				await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("数据队列包处理异常：" + ex3.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 订阅服务事件
		/// </summary>
		/// <param name="sender">源</param>
		/// <param name="e">事件结果</param>
		/// <exception cref="T:System.NotImplementedException"></exception>
		private async Task SubscribeService_OnDataEventAsync(object? sender, EventDataResult e)
		{
			SubscribeSource<AddressValue> source = ((ResultModel)e).GetSource<SubscribeSource<AddressValue>>();
			if (source != null && DataQueuePack != null)
			{
				await DataQueuePack.Writer.WriteAsync(source.Source).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 任务处理
		/// </summary>
		/// <param name="token">任务令牌</param>
		/// <returns>任务</returns>
		private async Task TaskHandleAsync(CancellationToken token)
		{
			try
			{
				while (await DataQueue.Reader.WaitToReadAsync(token).ConfigureAwait(continueOnCapturedContext: false))
				{
					AddressValue item;
					while (DataQueue.Reader.TryRead(out item))
					{
						if (item != null && !token.IsCancellationRequested)
						{
							await subscribeService.SetAsync(((AddressDetails)item).AddressName, item, token).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex3)
			{
				await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("数据处理异常：" + ex3.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 线程安全字典比对，是否一致
		/// </summary>
		/// <param name="Param1">参数1</param>
		/// <param name="Param2">参数2</param>
		/// <returns>是否一致</returns>
		private bool Comparison(ConcurrentDictionary<string, AddressValue> Param1, ConcurrentDictionary<string, AddressValue> Param2)
		{
			bool result = false;
			if (Param1.Count == Param2.Count)
			{
				result = true;
				foreach (KeyValuePair<string, AddressValue> item in Param1)
				{
					if (Param2.TryGetValue(item.Key, out AddressValue value))
					{
						if (!((object)value).Equals((object?)item.Value))
						{
							result = false;
							break;
						}
						continue;
					}
					result = false;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// 数组线程安全字典比对，是否一致
		/// </summary>
		/// <param name="Param1">参数1</param>
		/// <param name="Param2">参数2</param>
		/// <returns>是否一致</returns>
		private bool Comparison(List<ConcurrentDictionary<string, AddressValue>> Param1, List<ConcurrentDictionary<string, AddressValue>> Param2)
		{
			foreach (ConcurrentDictionary<string, AddressValue> a in Param2)
			{
				if (!Param1.Exists((ConcurrentDictionary<string, AddressValue> b) => Comparison(a, b)))
				{
					return false;
				}
			}
			return true;
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			Off(hardClose: true);
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await OffAsync(hardClose: true).ConfigureAwait(continueOnCapturedContext: false);
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 移除同批相同项
		/// </summary>
		/// <param name="address">地址</param>
		/// <param name="mName">方法名</param>
		/// <param name="token">传播消息取消通知</param>
		/// <returns></returns>
		private async Task<OperateResult> RemoveIdenticalItemAsync(Address address, string mName, CancellationToken token)
		{
			List<string> list = new List<string>();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (AddressDetails item in address.AddressArray)
			{
				if (item.AddressName != null)
				{
					dictionary.TryGetValue(item.AddressName, out var value);
					dictionary[item.AddressName] = value + 1;
				}
			}
			HashSet<string> duplicateNames = new HashSet<string>(from kv in dictionary
				where kv.Value > 1
				select kv.Key);
			if (duplicateNames.Count > 0)
			{
				address.AddressArray.RemoveAll((AddressDetails a) => duplicateNames.Contains(a.AddressName));
				foreach (string item2 in duplicateNames)
				{
					list.Add($"[ {item2} ]点位名存在{dictionary[item2]}个重复，请检查后重新订阅当前点位（此操作为保证当前点位数据正确性）");
				}
			}
			if (address.AddressArray.Count > 0)
			{
				base.basics.Address.AddressArray.AddRange(address.AddressArray);
				base.basics.Address.AddressArray = base.basics.Address.AddressArray.Distinct().ToList();
			}
			string methodName;
			if (list.Count > 0)
			{
				GoOn = true;
				SubscribeOperate subscribeOperate = this;
				string message = "订阅存在失败信息：" + ExtendMethod.ToJson<List<string>>(list, false);
				methodName = mName;
				return await subscribeOperate.EndOperateAsync(status: false, message, list, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", methodName, 367).ConfigureAwait(continueOnCapturedContext: false);
			}
			GoOn = true;
			SubscribeOperate subscribeOperate2 = this;
			methodName = mName;
			return await subscribeOperate2.EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", methodName, 371).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			string mName = await BegOperateAsync(token, "SubscribeAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				GoOn = false;
				await _semaphore.WaitAsync(token).ConfigureAwait(continueOnCapturedContext: false);
				try
				{
					HashSet<string> newNames = new HashSet<string>(from a in address.AddressArray
						where a.AddressName != null
						select a.AddressName);
					base.basics.Address.AddressArray.RemoveAll((AddressDetails a) => newNames.Contains(a.AddressName));
					return await RemoveIdenticalItemAsync(address, mName, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception ex)
				{
					return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "SubscribeAsync", 393).ConfigureAwait(continueOnCapturedContext: false);
				}
				finally
				{
					_semaphore.Release();
				}
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "SubscribeAsync", 403).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public OperateResult Subscribe(Address address)
		{
			return SubscribeAsync(address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult UnSubscribe(Address address)
		{
			return UnSubscribeAsync(address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "UnSubscribeAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				GoOn = false;
				List<string> FillMessage = new List<string>();
				await _semaphore.WaitAsync(token).ConfigureAwait(continueOnCapturedContext: false);
				try
				{
					HashSet<string> namesToRemove = new HashSet<string>(from b in address.AddressArray
						where b.AddressName != null
						select b.AddressName);
					base.basics.Address.AddressArray.RemoveAll((AddressDetails a) => namesToRemove.Contains(a.AddressName));
				}
				catch (Exception ex)
				{
					return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "UnSubscribeAsync", 434).ConfigureAwait(continueOnCapturedContext: false);
				}
				finally
				{
					GoOn = true;
					_semaphore.Release();
				}
				if (FillMessage.Count > 0)
				{
					return await EndOperateAsync(status: false, ExtendMethod.ToJson<List<string>>(FillMessage, false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "UnSubscribeAsync", 445).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "UnSubscribeAsync", 447).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "UnSubscribeAsync", 451).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (MonitorSwitch == null)
				{
					MonitorSwitch = new CancellationTokenSource();
					if (DataQueue == null)
					{
						DataQueue = Channel.CreateBounded<AddressValue>(channelOptions);
					}
					PollingAsync(MonitorSwitch.Token).ConfigureAwait(continueOnCapturedContext: false);
					if (DataQueuePack == null)
					{
						DataQueuePack = Channel.CreateBounded<AddressValue>(channelOptions);
					}
					if (DataQueuePackToken == null)
					{
						DataQueuePackToken = new CancellationTokenSource();
						TaskPackHandleAsync(DataQueuePackToken.Token).ConfigureAwait(continueOnCapturedContext: false);
					}
					if (TaskArray == null)
					{
						TaskArray = new ConcurrentDictionary<CancellationTokenSource, Task>();
						for (int i = 0; i < base.basics.TaskNumber; i++)
						{
							CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
							TaskArray.TryAdd(cancellationTokenSource, TaskHandleAsync(cancellationTokenSource.Token));
						}
					}
					subscribeService.OnDataEventAsync -= SubscribeService_OnDataEventAsync;
					subscribeService.OnDataEventAsync += SubscribeService_OnDataEventAsync;
					return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "OnAsync", 499).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, "已启动", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "OnAsync", 503).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "OnAsync", 508).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public OperateResult On()
		{
			return OnAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				GoOn = false;
				if (MonitorSwitch != null)
				{
					await MonitorSwitch.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
					MonitorSwitch.Dispose();
					MonitorSwitch = null;
				}
				if (DataQueuePackToken != null)
				{
					await DataQueuePackToken.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
					DataQueuePackToken.Dispose();
					DataQueuePackToken = null;
				}
				if (TaskArray != null)
				{
					foreach (KeyValuePair<CancellationTokenSource, Task> item in TaskArray)
					{
						await item.Key.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
						item.Key.Dispose();
					}
					TaskArray.Clear();
					TaskArray = null;
				}
				AddressValue item2;
				if (DataQueue != null)
				{
					DataQueue.Writer.Complete();
					while (DataQueue.Reader.TryRead(out item2))
					{
					}
					DataQueue = null;
				}
				if (DataQueuePack != null)
				{
					DataQueuePack.Writer.Complete();
					while (DataQueuePack.Reader.TryRead(out item2))
					{
					}
					DataQueuePack = null;
				}
				subscribeService.OnDataEventAsync -= SubscribeService_OnDataEventAsync;
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "OffAsync", 567).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "OffAsync", 571).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (DataQueue == null && TaskArray == null)
			{
				return await EndOperateAsync(status: false, "未订阅", null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "GetStatusAsync", 585).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: true, "已订阅", null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\SubscribeOperate.cs", "GetStatusAsync", 589).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}
	}
}
namespace Snet.Core.subscribe.core
{
	/// <summary>
	/// 订阅服务数据层，与调用者共存亡<br />
	/// 确保SN 是唯一，不然会导致数据冲突
	/// </summary>
	public class SubscribeService<T> : CoreUnify<SubscribeService<T>, string>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 数据源
		/// </summary>
		private ConcurrentDictionary<string, SubscribeSource<T>> Source = new ConcurrentDictionary<string, SubscribeSource<T>>();

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public SubscribeService(string basics)
			: base(basics)
		{
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			foreach (KeyValuePair<string, SubscribeSource<T>> item in Source)
			{
				item.Value.Dispose();
			}
			Source.Clear();
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			foreach (KeyValuePair<string, SubscribeSource<T>> item in Source)
			{
				await item.Value.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			Source.Clear();
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 设置数据
		/// </summary>
		/// <param name="SN">唯一标识符</param>
		/// <returns>设置的啥类型，返回啥类型</returns>
		public OperateResult Get(string SN)
		{
			return GetAsync(SN).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 设置数据异步
		/// </summary>
		/// <param name="SN">唯一标识符</param>
		/// <param name="token">令牌</param>
		/// <returns>设置的啥类型，返回啥类型</returns>
		public async Task<OperateResult> GetAsync(string SN, CancellationToken token = default(CancellationToken))
		{
			if (CoreUnify<SubscribeService<T>, string>.objList == null)
			{
				throw new InvalidOperationException("请使用单例模式创建实例");
			}
			if (Source.TryGetValue(SN, out SubscribeSource<T> value))
			{
				return await value.GetAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			}
			string message = "(" + SN + ") Instance does not exist";
			return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\core\\SubscribeService.cs", await BegOperateAsync(token, "GetAsync"), 69).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 设置数据
		/// </summary>
		/// <param name="SN">唯一标识符</param>
		/// <param name="Data">数据</param>
		/// <returns>统一结果</returns>
		public OperateResult Set(string SN, T Data)
		{
			return SetAsync(SN, Data).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 设置数据异步
		/// </summary>
		/// <param name="SN">唯一标识符</param>
		/// <param name="Data">数据</param>
		/// <param name="token">令牌</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> SetAsync(string SN, T Data, CancellationToken token = default(CancellationToken))
		{
			if (CoreUnify<SubscribeService<T>, string>.objList == null)
			{
				throw new InvalidOperationException("请使用单例模式创建实例");
			}
			await BegOperateAsync(token, "SetAsync").ConfigureAwait(continueOnCapturedContext: false);
			await Source.GetOrAdd(SN, delegate(string key)
			{
				SubscribeSource<T> subscribeSource = new SubscribeSource<T>(key);
				subscribeSource.OnDataEventAsync += base.OnDataEventHandlerAsync;
				return subscribeSource;
			}).SetAsync(Data, token).ConfigureAwait(continueOnCapturedContext: false);
			return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\core\\SubscribeService.cs", "SetAsync", 101).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
	/// <summary>
	/// 订阅数据库
	/// </summary>
	public class SubscribeSource<T> : CoreUnify<SubscribeSource<T>, string>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 比较时排除的字段名（缓存避免每次分配）
		/// </summary>
		private static readonly string[] ComparerExcludeFields = new string[1] { "Time" };

		/// <summary>
		/// 数据源
		/// </summary>
		public T Source { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		public DateTime UpdateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public SubscribeSource(string basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 设置数据
		/// </summary>
		/// <typeparam name="T">对象</typeparam>
		/// <param name="Data">数据</param>
		/// <returns>统一结果</returns>
		public OperateResult Set(T Data)
		{
			return SetAsync(Data).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 设置数据异步
		/// </summary>
		/// <param name="Data">数据</param>
		/// <returns>统一结果</returns>
		public async Task<OperateResult> SetAsync(T Data, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SetAsync").ConfigureAwait(continueOnCapturedContext: false);
			UpdateTime = DateTime.Now;
			if (!ExtendMethod.Comparer<T>(Data, Source, ComparerExcludeFields).Item1)
			{
				Source = Data;
				await OnDataEventHandlerAsync(this, new EventDataResult(true, "Data Update", (object)this)).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\core\\SubscribeSource.cs", "SetAsync", 55).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <returns>设置的啥类型，返回啥类型</returns>
		public OperateResult Get()
		{
			return GetAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <returns>设置的啥类型，返回啥类型</returns>
		public async Task<OperateResult> GetAsync(CancellationToken token = default(CancellationToken))
		{
			object resultData = this;
			return await EndOperateAsync(status: true, "Get Succeed", resultData, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\subscribe\\core\\SubscribeSource.cs", await BegOperateAsync(token, "GetAsync"), 69).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.reflection
{
	/// <summary>
	/// 反射
	/// </summary>
	public class ReflectionData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 可以同时处反射多个Dll到内存中
			/// </summary>
			public List<DllData> DllDatas { get; set; }
		}

		/// <summary>
		/// 动态库数据
		/// </summary>
		public class DllData
		{
			/// <summary>
			/// 动态链接库路径
			/// </summary>
			public string DllPath { get; set; }

			/// <summary>
			/// 绝对路径
			/// true:是绝对路径，反之false（探测程序集的基目录+DllPath）
			/// </summary>
			public bool IsAbsolutePath { get; set; }

			/// <summary>
			/// 命名空间集合，一个DLL中会出现多个命名空间
			/// </summary>
			public List<NamespaceData> NamespaceDatas { get; set; }
		}

		/// <summary>
		/// 命名空间数据
		/// </summary>
		public class NamespaceData
		{
			/// <summary>
			/// 命名空间
			/// </summary>
			public string Namespace { get; set; }

			/// <summary>
			/// 在同一个命名空间存在多个类
			/// </summary>
			public List<ClassData> ClassDatas { get; set; }
		}

		/// <summary>
		/// 类数据
		/// </summary>
		public class ClassData
		{
			/// <summary>
			/// 多个实例的区分的唯一标识符
			/// </summary>
			public string SN { get; set; }

			/// <summary>
			/// 类名
			/// </summary>
			public string ClassName { get; set; }

			/// <summary>
			/// 构造函数参数，
			/// 创建实例的参数，也就是构造函数传入的参数，
			/// 包含要传递给构造函数的自变量的数组。 此自变量数组在数量、顺序和类型方面必须与要调用的构造函数的参数匹配。 如果需要无参数构造函数，则 args 必须是空数组或 null。
			/// </summary>
			public object[]? ConstructorParam { get; set; }

			/// <summary>
			/// 方法名 获取到多个方法，并存入内存
			/// 方法名，是否是异步方法
			/// </summary>
			public List<MethodData>? MethodDatas { get; set; }

			/// <summary>
			/// 事件数据
			/// </summary>
			public List<EventData>? EventDatas { get; set; }
		}

		/// <summary>
		/// 方法数据
		/// </summary>
		public class MethodData
		{
			/// <summary>
			/// 用于外部获取或调用使用的唯一标识符
			/// </summary>
			public string SN { get; set; }

			/// <summary>
			/// 是否执行，不判断执行结果状态
			/// </summary>
			public bool WhetherExecute { get; set; }

			/// <summary>
			/// 方法名称
			/// </summary>
			public string MethodName { get; set; }

			/// <summary>
			/// 在初始化时需要执行的方法参数
			/// </summary>
			public object[]? MethodParam { get; set; }
		}

		/// <summary>
		/// 事件数据
		/// </summary>
		public class EventData
		{
			/// <summary>
			/// 事件的唯一标识符
			/// </summary>
			public string SN { get; set; }

			/// <summary>
			/// 事件的名称
			/// </summary>
			public string EventName { get; set; }
		}

		/// <summary>
		/// 反射方法结果数据
		/// </summary>
		public class ReflectionMethodResult
		{
			/// <summary>
			/// 方法信息
			/// </summary>
			public MethodInfo Method { get; set; }

			/// <summary>
			/// 实例对象
			/// </summary>
			public object InstanceObject { get; set; }
		}

		/// <summary>
		/// 反射事件结果数据
		/// </summary>
		public class ReflectionEventResult
		{
			/// <summary>
			/// 事件信息
			/// </summary>
			public EventInfo Event { get; set; }

			/// <summary>
			/// 实例对象
			/// </summary>
			public object InstanceObject { get; set; }
		}
	}
	/// <summary>
	/// 反射操作管理器。
	/// <para>
	/// 工作流程：<br />
	/// 1. 生成配置（Basics / DllData / NamespaceData / ClassData / MethodData / EventData）<br />
	/// 2. 调用 <see cref="M:Snet.Core.reflection.ReflectionOperate.Init" /> 进行反射初始化<br />
	/// 3. 调用 <see cref="M:Snet.Core.reflection.ReflectionOperate.ExecuteMethod(System.String,System.Object[])" /> 执行方法 或 <see cref="M:Snet.Core.reflection.ReflectionOperate.RegisterEvent(System.String,System.Boolean,System.Action{System.Object},System.Action{System.Object,System.Object},System.Action{System.Object,System.Object,System.Object},System.Action{System.Object,System.Object,System.Object,System.Object},System.Action{System.Object,System.Object,System.Object,System.Object,System.Object},System.Action{System.Object,System.Object,System.Object,System.Object,System.Object,System.Object})" /> 注册事件
	/// </para>
	/// </summary>
	public class ReflectionOperate : CoreUnify<ReflectionOperate, ReflectionData.Basics>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 反射初始化状态。
		/// <para>
		/// 使用 volatile 保证跨线程可见性：当 Init() 在一个线程中将其设为 true 时，
		/// 其他线程在 GetStatus() 中能立即读到最新值，而不会读到 CPU 缓存中的过期值。
		/// </para>
		/// </summary>
		private volatile bool _reflectionState;

		/// <summary>
		/// 【新增】Init() 的并发控制信号量。
		/// 防止多线程同时调用 Init() 导致重复加载程序集、重复创建实例等问题。
		/// 初始计数 1 = 同一时刻只有一个线程可以执行初始化。
		/// </summary>
		private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

		/// <summary>
		/// 【新增】释放状态标志。
		/// 0 = 未释放，1 = 已释放或正在释放。
		/// 使用 Interlocked 保证线程安全的原子读写。
		/// </summary>
		private int _disposed;

		/// <summary>
		/// 方法的 IOC 容器。
		/// 键 = 实例标识符，格式为 $"{ClassData.SN}{MethodData.SN}"
		/// 值 = 反射方法结果，包含实例对象和方法元数据
		/// </summary>
		private readonly ConcurrentDictionary<string, ReflectionData.ReflectionMethodResult> _methodContainer = new ConcurrentDictionary<string, ReflectionData.ReflectionMethodResult>();

		/// <summary>
		/// 事件的 IOC 容器。
		/// 键 = 实例标识符，格式为 $"{ClassData.SN}{EventData.SN}"
		/// 值 = 反射事件结果，包含实例对象和事件元数据
		/// </summary>
		private readonly ConcurrentDictionary<string, ReflectionData.ReflectionEventResult> _eventContainer = new ConcurrentDictionary<string, ReflectionData.ReflectionEventResult>();

		/// <summary>
		/// 对象实例容器。
		/// 键 = 类型全名（Type.FullName）
		/// 值 = 通过反射创建的实例对象
		/// </summary>
		private readonly ConcurrentDictionary<string, object> _objectContainer = new ConcurrentDictionary<string, object>();

		/// <summary>
		/// PluginLoadContext 容器，保持引用以防止被 GC 回收。
		/// 键 = DLL 绝对路径，值 = PluginLoadContext 实例
		/// </summary>
		private readonly ConcurrentDictionary<string, PluginLoadContext> _pluginContexts = new ConcurrentDictionary<string, PluginLoadContext>();

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础配置数据，包含 DLL 路径、命名空间、类名等反射元数据</param>
		public ReflectionOperate(ReflectionData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 获取反射是否已成功初始化
		/// </summary>
		/// <returns>true 表示已初始化</returns>
		public bool GetStatus()
		{
			return _reflectionState;
		}

		/// <summary>
		/// 初始化反射系统（同步版本）。
		/// <para><b>推荐使用 <see cref="M:Snet.Core.reflection.ReflectionOperate.InitAsync(System.Threading.CancellationToken)" /> 异步版本，避免阻塞调用线程。</b></para>
		/// </summary>
		/// <returns>统一出参</returns>
		public OperateResult Init()
		{
			BegOperate("Init");
			if (Volatile.Read(in _disposed) != 0)
			{
				return EndOperate(status: false, "实例已被释放，无法执行初始化", null, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "Init", 116);
			}
			if (_reflectionState)
			{
				return EndOperate(status: true, "反射已初始化，无需重复执行", null, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "Init", 118);
			}
			return Task.Run(() => InitAsync()).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 【新增】异步初始化反射系统。
		/// <para>
		/// 使用 <see cref="M:System.Threading.SemaphoreSlim.WaitAsync" /> 异步等待锁，
		/// 不阻塞调用线程。支持 Double-Check Locking 和资源回滚。
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">取消令牌</param>
		/// <returns>统一出参</returns>
		public async Task<OperateResult> InitAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			string mName = await BegOperateAsync(cancellationToken, "InitAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (Volatile.Read(in _disposed) != 0)
			{
				return await EndOperateAsync(status: false, "实例已被释放，无法执行初始化", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "InitAsync", 139).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (_reflectionState)
			{
				return await EndOperateAsync(status: true, "反射已初始化，无需重复执行", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "InitAsync", 143).ConfigureAwait(continueOnCapturedContext: false);
			}
			await _initLock.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (_reflectionState)
				{
					return await EndOperateAsync(status: true, "反射已初始化，无需重复执行", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "InitAsync", 151).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await InitCoreAsync(cancellationToken, mName).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				_initLock.Release();
			}
		}

		/// <summary>
		/// 【重构·异步】Init 的核心实现（在 _initLock 保护下执行）。
		/// <para>
		/// <b>资源安全策略 — 先收集、后提交：</b><br />
		/// 所有新创建的实例和上下文先收集到临时列表中。<br />
		/// 全部成功后一次性提交到永久容器。<br />
		/// 任何阶段失败则立即回滚临时列表中的所有资源。<br />
		/// 这确保了"要么全部成功，要么全部回滚"的原子性语义。
		/// </para>
		/// <para>
		/// <b>异步化说明：</b>DLL 加载使用 <see cref="M:PluginLoadContext.LoadFromFileStreamAsync(System.String,System.Threading.CancellationToken)" />，
		/// 文件 I/O 不阻塞线程。后续的反射操作（GetType/GetMethod/Activator.CreateInstance）为纯 CPU 操作，保持同步。
		/// </para>
		/// </summary>
		/// <returns>统一出参</returns>
		private async Task<OperateResult> InitCoreAsync(CancellationToken cancellationToken, string mName)
		{
			List<(string Path, PluginLoadContext Context)> pendingContexts = new List<(string, PluginLoadContext)>();
			List<(string Key, object Instance)> pendingInstances = new List<(string, object)>();
			List<(string Key, ReflectionData.ReflectionMethodResult Result)> pendingMethods = new List<(string, ReflectionData.ReflectionMethodResult)>();
			List<(string Key, ReflectionData.ReflectionEventResult Result)> pendingEvents = new List<(string, ReflectionData.ReflectionEventResult)>();
			try
			{
				foreach (ReflectionData.DllData dllData in base.basics.DllDatas)
				{
					string fullPath = (dllData.IsAbsolutePath ? Path.GetFullPath(dllData.DllPath) : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, dllData.DllPath)));
					if (!File.Exists(fullPath))
					{
						return await FailWithRollbackAsync(pendingContexts, pendingInstances, "文件不存在: " + fullPath);
					}
					Assembly assembly;
					if (_pluginContexts.TryGetValue(fullPath, out PluginLoadContext value))
					{
						PluginLoadContext pluginLoadContext = value;
						assembly = await pluginLoadContext.LoadFromFileStreamAsync(fullPath).ConfigureAwait(continueOnCapturedContext: false);
					}
					else
					{
						(string, PluginLoadContext) tuple = pendingContexts.FirstOrDefault<(string, PluginLoadContext)>(((string Path, PluginLoadContext Context) p) => p.Path == fullPath);
						if (tuple.Item2 != null)
						{
							PluginLoadContext pluginLoadContext = tuple.Item2;
							assembly = await pluginLoadContext.LoadFromFileStreamAsync(fullPath).ConfigureAwait(continueOnCapturedContext: false);
						}
						else
						{
							PluginLoadContext pluginLoadContext = new PluginLoadContext(fullPath, 52428800L);
							pendingContexts.Add((fullPath, pluginLoadContext));
							assembly = await pluginLoadContext.LoadFromFileStreamAsync(fullPath).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					foreach (ReflectionData.NamespaceData namespaceData in dllData.NamespaceDatas)
					{
						foreach (ReflectionData.ClassData classData in namespaceData.ClassDatas)
						{
							string text = namespaceData.Namespace + "." + classData.ClassName;
							Type type = assembly.GetType(text, throwOnError: false, ignoreCase: true);
							if ((object)type == null)
							{
								return await FailWithRollbackAsync(pendingContexts, pendingInstances, "类不存在: " + dllData.DllPath + " -> " + text);
							}
							if (string.IsNullOrWhiteSpace(classData.SN))
							{
								return await FailWithRollbackAsync(pendingContexts, pendingInstances, "ClassData.SN 不能为空: " + text);
							}
							if (_objectContainer.ContainsKey(classData.SN) || pendingInstances.Any<(string, object)>(((string Key, object Instance) p) => p.Key == classData.SN))
							{
								return await FailWithRollbackAsync(pendingContexts, pendingInstances, $"实例已存在，检测到重复的 SN: {classData.SN} (类型: {text})");
							}
							object obj = CreateInstance(type, classData.ConstructorParam);
							if (obj == null)
							{
								return await FailWithRollbackAsync(pendingContexts, pendingInstances, "实例创建失败: " + text);
							}
							pendingInstances.Add((classData.SN, obj));
							if (classData.MethodDatas != null)
							{
								foreach (ReflectionData.MethodData methodData in classData.MethodDatas)
								{
									MethodInfo method = type.GetMethod(methodData.MethodName);
									if ((object)method == null)
									{
										return await FailWithRollbackAsync(pendingContexts, pendingInstances, $"方法不存在: {dllData.DllPath} -> {text} -> {methodData.MethodName}");
									}
									string item = classData.SN + methodData.SN;
									pendingMethods.Add((item, new ReflectionData.ReflectionMethodResult
									{
										InstanceObject = obj,
										Method = method
									}));
									if (methodData.WhetherExecute)
									{
										try
										{
											object[] parameters = PluginOperate.ConvertParameters(methodData.MethodParam, method);
											method.Invoke(obj, parameters);
										}
										catch (TargetInvocationException ex)
										{
											return await FailWithRollbackAsync(pendingContexts, pendingInstances, $"方法执行失败: {text} -> {methodData.MethodName}: {ex.InnerException?.Message ?? ex.Message}");
										}
										catch (ArgumentException ex2)
										{
											return await FailWithRollbackAsync(pendingContexts, pendingInstances, $"方法参数不匹配: {text} -> {methodData.MethodName}: {ex2.Message}");
										}
										catch (Exception ex3)
										{
											return await FailWithRollbackAsync(pendingContexts, pendingInstances, $"方法执行异常: {text} -> {methodData.MethodName}: {ex3.Message}");
										}
									}
								}
							}
							if (classData.EventDatas == null)
							{
								continue;
							}
							foreach (ReflectionData.EventData eventData in classData.EventDatas)
							{
								EventInfo eventInfo = type.GetEvent(eventData.EventName);
								if ((object)eventInfo == null)
								{
									return await FailWithRollbackAsync(pendingContexts, pendingInstances, $"事件不存在: {dllData.DllPath} -> {text} -> {eventData.EventName}");
								}
								string item2 = classData.SN + eventData.SN;
								pendingEvents.Add((item2, new ReflectionData.ReflectionEventResult
								{
									InstanceObject = obj,
									Event = eventInfo
								}));
							}
						}
					}
				}
				foreach (var (key, value2) in pendingContexts)
				{
					_pluginContexts[key] = value2;
				}
				foreach (var (key2, value3) in pendingInstances)
				{
					_objectContainer[key2] = value3;
				}
				foreach (var (key3, value4) in pendingMethods)
				{
					_methodContainer[key3] = value4;
				}
				foreach (var (key4, value5) in pendingEvents)
				{
					_eventContainer[key4] = value5;
				}
				_reflectionState = true;
				ReflectionOperate reflectionOperate = this;
				ConcurrentDictionary<string, ReflectionData.ReflectionMethodResult> methodContainer = _methodContainer;
				string methodName = mName;
				return await reflectionOperate.EndOperateAsync(status: true, "反射初始化成功", methodContainer, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", methodName, 340);
			}
			catch (Exception ex4)
			{
				RollbackPending(pendingContexts, pendingInstances);
				ReflectionOperate reflectionOperate2 = this;
				string message = ex4.Message;
				string methodName = mName;
				return await reflectionOperate2.EndOperateAsync(status: false, message, null, ex4, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", methodName, 346).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 【修复】通过反射创建指定类型的实例。
		/// <para>
		/// <b>支持多种参数格式：</b><br />
		/// 1. 强类型对象 → 直接匹配构造函数参数类型<br />
		/// 2. ExpandoObject / Dictionary → JSON 序列化后反序列化为目标类型<br />
		/// 3. 直接传入目标类型的实例 → 透传
		/// </para>
		/// </summary>
		private object? CreateInstance(Type type, object[]? constructorParam)
		{
			if (constructorParam == null || constructorParam.Length == 0)
			{
				return Activator.CreateInstance(type);
			}
			ConstructorInfo constructorInfo = type.GetConstructors().FirstOrDefault((ConstructorInfo c) => c.GetParameters().Length != 0);
			if ((object)constructorInfo == null)
			{
				return Activator.CreateInstance(type);
			}
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			if (constructorParam.Length == 1 && parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(constructorParam[0]))
			{
				return Activator.CreateInstance(type, constructorParam);
			}
			object[] array = PluginOperate.ConvertParameters(constructorParam, constructorInfo);
			if (array == null)
			{
				try
				{
					return Activator.CreateInstance(type, constructorParam);
				}
				catch
				{
					return null;
				}
			}
			return Activator.CreateInstance(type, array);
		}

		/// <summary>
		/// 执行已注册的方法（同步版本）。
		/// <para><b>推荐使用 <see cref="M:Snet.Core.reflection.ReflectionOperate.ExecuteMethodAsync(System.String,System.Object[],System.Threading.CancellationToken)" /> 异步版本。</b></para>
		/// </summary>
		public object? ExecuteMethod(string SN, object[]? MethodParam = null)
		{
			return Task.Run(() => ExecuteMethodAsync(SN, MethodParam)).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 【新增】异步执行已注册的方法。
		/// </summary>
		/// <param name="SN">方法的唯一标识符</param>
		/// <param name="MethodParam">方法参数；为 null 表示无参调用</param>
		/// <param name="cancellationToken">取消令牌</param>
		/// <returns>方法的执行结果</returns>
		public async Task<object?> ExecuteMethodAsync(string SN, object[]? MethodParam = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			await BegOperateAsync(cancellationToken, "ExecuteMethodAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (Volatile.Read(in _disposed) != 0)
			{
				return await EndOperateAsync(status: false, "实例已被释放，无法执行方法", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "ExecuteMethodAsync", 417).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (string.IsNullOrWhiteSpace(SN) || !_methodContainer.TryGetValue(SN, out ReflectionData.ReflectionMethodResult value))
			{
				return await EndOperateAsync(status: false, "未找到标识符为 " + SN + " 的方法", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "ExecuteMethodAsync", 420).ConfigureAwait(continueOnCapturedContext: false);
			}
			try
			{
				object[] parameters = PluginOperate.ConvertParameters(MethodParam, value.Method);
				return value.Method.Invoke(value.InstanceObject, parameters);
			}
			catch (TargetInvocationException ex)
			{
				return await EndOperateAsync(status: false, "方法 " + SN + " 执行失败: " + (ex.InnerException?.Message ?? ex.Message), null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "ExecuteMethodAsync", 430).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, "方法 " + SN + " 执行失败: " + ex2.Message, null, ex2, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "ExecuteMethodAsync", 434).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 获取所有已注册的方法
		/// </summary>
		/// <returns>方法容器；未初始化或为空时返回 null</returns>
		public ConcurrentDictionary<string, ReflectionData.ReflectionMethodResult>? GetMethods()
		{
			ConcurrentDictionary<string, ReflectionData.ReflectionMethodResult> methodContainer = _methodContainer;
			if (methodContainer != null && !methodContainer.IsEmpty)
			{
				return _methodContainer;
			}
			return null;
		}

		/// <summary>
		/// 获取指定标识符的方法。
		/// <para>
		/// <b>修复内容：</b>原代码使用索引器 <c>MethodIocContainer[SN]</c>，
		/// 当 SN 不存在时抛出 <see cref="T:System.Collections.Generic.KeyNotFoundException" />。
		/// 改为 TryGetValue 安全查找。
		/// </para>
		/// </summary>
		/// <param name="SN">方法唯一标识符</param>
		/// <returns>方法反射结果；未找到时返回 null</returns>
		public ReflectionData.ReflectionMethodResult? GetMethod(string SN)
		{
			if (string.IsNullOrWhiteSpace(SN))
			{
				return null;
			}
			_methodContainer.TryGetValue(SN, out ReflectionData.ReflectionMethodResult value);
			return value;
		}

		/// <summary>
		/// 获取所有已注册的事件
		/// </summary>
		/// <returns>事件容器；未初始化或为空时返回 null</returns>
		public ConcurrentDictionary<string, ReflectionData.ReflectionEventResult>? GetEvents()
		{
			ConcurrentDictionary<string, ReflectionData.ReflectionEventResult> eventContainer = _eventContainer;
			if (eventContainer != null && !eventContainer.IsEmpty)
			{
				return _eventContainer;
			}
			return null;
		}

		/// <summary>
		/// 获取指定标识符的事件。
		/// <para>
		/// <b>修复内容：</b>同 <see cref="M:Snet.Core.reflection.ReflectionOperate.GetMethod(System.String)" />，
		/// 原代码使用索引器在键不存在时抛出异常。
		/// </para>
		/// </summary>
		/// <param name="SN">事件唯一标识符</param>
		/// <returns>事件反射结果；未找到时返回 null</returns>
		public ReflectionData.ReflectionEventResult? GetEvent(string SN)
		{
			if (string.IsNullOrWhiteSpace(SN))
			{
				return null;
			}
			_eventContainer.TryGetValue(SN, out ReflectionData.ReflectionEventResult value);
			return value;
		}

		/// <summary>
		/// 注册或移除事件处理程序（同步版本）。
		/// <para><b>推荐使用 <see cref="M:Snet.Core.reflection.ReflectionOperate.RegisterEventAsync(System.String,System.Boolean,System.Action{System.Object},System.Action{System.Object,System.Object},System.Action{System.Object,System.Object,System.Object},System.Action{System.Object,System.Object,System.Object,System.Object},System.Action{System.Object,System.Object,System.Object,System.Object,System.Object},System.Action{System.Object,System.Object,System.Object,System.Object,System.Object,System.Object},System.Threading.CancellationToken)" /> 异步版本。</b></para>
		/// </summary>
		public OperateResult RegisterEvent(string SN, bool Register, Action<object>? P1 = null, Action<object, object>? P2 = null, Action<object, object, object>? P3 = null, Action<object, object, object, object>? P4 = null, Action<object, object, object, object, object>? P5 = null, Action<object, object, object, object, object, object>? P6 = null)
		{
			return Task.Run(() => RegisterEventAsync(SN, Register, P1, P2, P3, P4, P5, P6)).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 【新增】异步注册或移除事件处理程序。
		/// </summary>
		/// <param name="SN">事件的唯一标识符</param>
		/// <param name="Register">true = 注册，false = 移除</param>
		/// <param name="P1">一个参数的事件委托</param>
		/// <param name="P2">两个参数的事件委托</param>
		/// <param name="P3">三个参数的事件委托</param>
		/// <param name="P4">四个参数的事件委托</param>
		/// <param name="P5">五个参数的事件委托</param>
		/// <param name="P6">六个参数的事件委托</param>
		/// <param name="cancellationToken">取消令牌</param>
		/// <returns>统一出参</returns>
		public async Task<OperateResult> RegisterEventAsync(string SN, bool Register, Action<object>? P1 = null, Action<object, object>? P2 = null, Action<object, object, object>? P3 = null, Action<object, object, object, object>? P4 = null, Action<object, object, object, object, object>? P5 = null, Action<object, object, object, object, object, object>? P6 = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			await BegOperateAsync(cancellationToken, "RegisterEventAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (Volatile.Read(in _disposed) != 0)
			{
				return await EndOperateAsync(status: false, "实例已被释放，无法注册事件", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 544).ConfigureAwait(continueOnCapturedContext: false);
			}
			ReflectionData.ReflectionEventResult reflectionEventResult = GetEvent(SN);
			if (reflectionEventResult == null)
			{
				return await EndOperateAsync(status: false, "事件操作失败，未找到标识符为 " + SN + " 的事件", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 550).ConfigureAwait(continueOnCapturedContext: false);
			}
			if ((object)reflectionEventResult.Event.EventHandlerType == null)
			{
				return await EndOperateAsync(status: false, "事件 " + SN + " 的 EventHandlerType 为空", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 553).ConfigureAwait(continueOnCapturedContext: false);
			}
			Delegate obj = null;
			try
			{
				if (P1 != null)
				{
					obj = Delegate.CreateDelegate(reflectionEventResult.Event.EventHandlerType, P1.Target, P1.Method);
				}
				else if (P2 != null)
				{
					obj = Delegate.CreateDelegate(reflectionEventResult.Event.EventHandlerType, P2.Target, P2.Method);
				}
				else if (P3 != null)
				{
					obj = Delegate.CreateDelegate(reflectionEventResult.Event.EventHandlerType, P3.Target, P3.Method);
				}
				else if (P4 != null)
				{
					obj = Delegate.CreateDelegate(reflectionEventResult.Event.EventHandlerType, P4.Target, P4.Method);
				}
				else if (P5 != null)
				{
					obj = Delegate.CreateDelegate(reflectionEventResult.Event.EventHandlerType, P5.Target, P5.Method);
				}
				else if (P6 != null)
				{
					obj = Delegate.CreateDelegate(reflectionEventResult.Event.EventHandlerType, P6.Target, P6.Method);
				}
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, "创建事件委托失败: " + ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 576).ConfigureAwait(continueOnCapturedContext: false);
			}
			if ((object)obj == null)
			{
				return await EndOperateAsync(status: false, "事件操作失败：至少需要提供一个非空的事件委托参数（P1-P6）", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 581).ConfigureAwait(continueOnCapturedContext: false);
			}
			try
			{
				if (Register)
				{
					reflectionEventResult.Event.AddEventHandler(reflectionEventResult.InstanceObject, obj);
					return await EndOperateAsync(status: true, "事件注册成功", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 588).ConfigureAwait(continueOnCapturedContext: false);
				}
				reflectionEventResult.Event.RemoveEventHandler(reflectionEventResult.InstanceObject, obj);
				return await EndOperateAsync(status: true, "事件移除成功", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 593).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, "事件" + (Register ? "注册" : "移除") + "失败: " + ex2.Message, null, ex2, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "RegisterEventAsync", 598).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 获取反射创建的实例对象（按 ClassData.SN 查找）。
		/// </summary>
		/// <param name="SN">ClassData.SN（创建实例时使用的唯一标识符）</param>
		/// <returns>实例对象；未找到时返回 null</returns>
		public object? ReflectionInstance(string SN)
		{
			if (string.IsNullOrWhiteSpace(SN))
			{
				return null;
			}
			_objectContainer.TryGetValue(SN, out object value);
			return value;
		}

		/// <summary>
		/// 【新增】使用已加载程序集的类型创建构造参数对象。
		/// <para>
		/// 解决用 <see cref="!:ExpandoObject" /> 或 <c>Dictionary&lt;string,object&gt;</c>
		/// 传参时，JSON 序列化/反序列化可能因属性名不匹配（如 <c>IpAddress</c> vs <c>Host</c>）
		/// 导致关键字段为 null/默认值的问题。
		/// </para>
		/// <para>
		/// <b>工作原理：</b>通过已加载程序集的类型，直接用 Reflection.SetValue 赋值，<br />
		/// 100% 精确匹配属性名，不依赖 JSON 序列化。
		/// </para>
		/// </summary>
		/// <param name="dllPath">DLL 绝对路径（与 DllData.DllPath 一致）</param>
		/// <param name="paramTypeName">构造参数类型的完整名（如 "Snet.Mqtt.service.MqttServiceData"）</param>
		/// <param name="properties">属性名→值的字典。key 必须与目标类型的属性名完全一致。</param>
		/// <returns>创建好的强类型对象；失败时返回 null</returns>
		public object? CreateConstructorParam(string dllPath, string paramTypeName, IDictionary<string, object?> properties)
		{
			if (string.IsNullOrWhiteSpace(dllPath) || string.IsNullOrWhiteSpace(paramTypeName) || properties == null)
			{
				return null;
			}
			string fullPath = Path.GetFullPath(dllPath);
			if (!_pluginContexts.TryGetValue(fullPath, out PluginLoadContext value))
			{
				return null;
			}
			try
			{
				Type type = value.LoadFromFileStream(fullPath).GetType(paramTypeName, throwOnError: false, ignoreCase: false);
				if (type == null)
				{
					return null;
				}
				object obj = Activator.CreateInstance(type);
				foreach (KeyValuePair<string, object> property2 in properties)
				{
					PropertyInfo property = type.GetProperty(property2.Key);
					if (!(property != null) || !property.CanWrite)
					{
						continue;
					}
					object obj2 = property2.Value;
					if (obj2 != null && obj2.GetType() != property.PropertyType)
					{
						try
						{
							obj2 = Convert.ChangeType(obj2, property.PropertyType);
						}
						catch
						{
						}
					}
					property.SetValue(obj, obj2);
				}
				return obj;
			}
			catch
			{
				return null;
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
			{
				_reflectionState = false;
				DisposeAllInstances();
				_methodContainer.Clear();
				_eventContainer.Clear();
				_objectContainer.Clear();
				UnloadPluginContexts();
				_initLock.Dispose();
				base.Dispose();
			}
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
			{
				_reflectionState = false;
				await DisposeAllInstancesAsync().ConfigureAwait(continueOnCapturedContext: false);
				_methodContainer.Clear();
				_eventContainer.Clear();
				_objectContainer.Clear();
				UnloadPluginContexts();
				_initLock.Dispose();
				await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 【异步】初始化失败时回滚并返回失败结果。
		/// 清理临时列表中所有已创建的资源。
		/// </summary>
		private async Task<OperateResult> FailWithRollbackAsync(List<(string Path, PluginLoadContext Context)> contexts, List<(string Key, object Instance)> instances, string message)
		{
			RollbackPending(contexts, instances);
			return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\reflection\\ReflectionOperate.cs", "FailWithRollbackAsync", 735).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 【新增】回滚所有待提交的资源。
		/// 先释放实例（可能依赖上下文中的程序集），再卸载加载上下文。
		/// </summary>
		private static void RollbackPending(List<(string Path, PluginLoadContext Context)> contexts, List<(string Key, object Instance)> instances)
		{
			foreach (var instance in instances)
			{
				SafeDisposeObject(instance.Instance);
			}
			foreach (var context in contexts)
			{
				PluginLoadContext item = context.Context;
				try
				{
					item.Unload();
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// 安全地释放单个对象实例（同步版本）。
		/// <para>
		/// <b>死锁修复：</b>原代码使用 <c>DisposeAsync().AsTask().GetAwaiter().GetResult()</c>
		/// 在 SynchronizationContext 环境中可能死锁。<br />
		/// 修复方案：使用 <c>Task.Run</c> 将异步释放操作调度到线程池，
		/// 再同步等待线程池任务完成，彻底绕开 SynchronizationContext。
		/// </para>
		/// </summary>
		private static void SafeDisposeObject(object? obj)
		{
			if (obj == null)
			{
				return;
			}
			try
			{
				IAsyncDisposable asyncDisposable = obj as IAsyncDisposable;
				if (asyncDisposable != null)
				{
					Task.Run(() => asyncDisposable.DisposeAsync().AsTask()).GetAwaiter().GetResult();
				}
				else if (obj is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// 【新增】异步安全释放单个对象实例。
		/// </summary>
		private static async Task SafeDisposeObjectAsync(object? obj)
		{
			if (obj == null)
			{
				return;
			}
			try
			{
				if (obj is IAsyncDisposable asyncDisposable)
				{
					await asyncDisposable.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				else if (obj is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// 【新增】同步释放容器中的所有实例。
		/// </summary>
		private void DisposeAllInstances()
		{
			foreach (KeyValuePair<string, object> item in _objectContainer)
			{
				SafeDisposeObject(item.Value);
			}
		}

		/// <summary>
		/// 【新增】异步释放容器中的所有实例。
		/// </summary>
		private async Task DisposeAllInstancesAsync()
		{
			foreach (KeyValuePair<string, object> item in _objectContainer)
			{
				await SafeDisposeObjectAsync(item.Value).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 卸载所有 PluginLoadContext，释放插件程序集。
		/// </summary>
		private void UnloadPluginContexts()
		{
			foreach (string item in _pluginContexts.Keys.ToList())
			{
				if (_pluginContexts.TryRemove(item, out PluginLoadContext value))
				{
					try
					{
						value.Unload();
					}
					catch (Exception)
					{
					}
				}
			}
		}
	}
}
namespace Snet.Core.plugin
{
	/// <summary>
	/// 插件操作管理器。
	/// <para>
	/// 负责插件的全生命周期管理：加载 → 实例化 → 参数转换 → 状态验证 → 热卸载。
	/// </para>
	/// <para>
	/// <b>核心设计：</b>
	/// <list type="bullet">
	///   <item>每个插件 DLL 使用独立的可回收 <see cref="T:PluginLoadContext" />，支持热卸载</item>
	///   <item>内部使用 <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> 缓存类型和实例，支持线程安全并发访问</item>
	///   <item>共享接口类型（如 IDaq/IMq）自动复用默认上下文版本，保证类型身份一致性</item>
	///   <item>支持同名插件的热重载（卸载旧上下文 → 加载新版本）</item>
	/// </list>
	/// </para>
	/// </summary>
	/// <summary>
	/// PluginOperate 的内部辅助方法（private helpers）。
	/// 从主文件拆分以满足 800 行限制，保持高内聚低耦合。
	/// </summary>
	public class PluginOperate : CoreUnify<PluginOperate, string>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 动态库文件名匹配模式。
		/// 仅加载以 "Snet." 开头的 DLL，避免误加载第三方依赖或其他无关程序集。
		/// </summary>
		private const string DllWatcherPattern = "Snet.*.dll";

		/// <summary>
		/// 需要跳过的内部类型名称片段。
		/// 这些是框架内部的抽象基类或接口辅助类型，不应被实例化为插件。
		/// </summary>
		private static readonly HashSet<string> SkipTypeNames = new HashSet<string>(StringComparer.Ordinal) { "IOu", "DaqAbstract`2", "MqAbstract`2" };

		/// <summary>
		/// IOC 容器：插件类型缓存（键 = 类名，值 = 类型元数据）。
		/// </summary>
		private readonly ConcurrentDictionary<string, Type> _pluginTypes = new ConcurrentDictionary<string, Type>();

		/// <summary>
		/// IOC 容器：DAQ 插件实例缓存（键 = 类名，值 = IDaq 单例）。
		/// </summary>
		private readonly ConcurrentDictionary<string, IDaq> _daqInstances = new ConcurrentDictionary<string, IDaq>();

		/// <summary>
		/// IOC 容器：MQ 插件实例缓存（键 = 类名，值 = IMq 单例）。
		/// </summary>
		private readonly ConcurrentDictionary<string, IMq> _mqInstances = new ConcurrentDictionary<string, IMq>();

		/// <summary>
		/// 插件加载上下文缓存（键 = 插件类名，值 = 对应的加载上下文）。
		/// </summary>
		private readonly ConcurrentDictionary<string, PluginLoadContext> _pluginContexts = new ConcurrentDictionary<string, PluginLoadContext>();

		/// <summary>
		/// 接口类型解析缓存（键 = 接口全名，值 = 接口 Type 对象或 null）。
		/// </summary>
		private readonly ConcurrentDictionary<string, Type?> _interfaceTypeCache = new ConcurrentDictionary<string, Type>();

		/// <summary>
		/// InitPlugin 的异步并发控制信号量。
		/// 防止多线程同时调用 InitPlugin 对同一目录进行重复加载，
		/// 避免创建重复的 PluginLoadContext 导致资源浪费和类型冲突。
		/// 使用 SemaphoreSlim 而非 lock，支持异步等待不阻塞线程。
		/// </summary>
		private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

		/// <summary>
		/// 释放状态标志，防止重复释放。
		/// 0 = 未释放，1 = 已释放或正在释放。
		/// </summary>
		private int _disposed;

		/// <summary>
		/// 获取当前已加载的插件类型总数（线程安全读取）。
		/// </summary>
		public int LoadedPluginCount => _pluginTypes.Count;

		/// <summary>
		/// 获取当前已加载的所有插件类名（线程安全快照）。
		/// 使用 AsReadOnly() 替代强制转换，避免 InvalidCastException。
		/// </summary>
		public IReadOnlyCollection<string> LoadedPluginNames => _pluginTypes.Keys.ToList().AsReadOnly();

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public PluginOperate()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public PluginOperate(string basics)
			: base(basics)
		{
		}

		/// <inheritdoc />
		/// <remarks>
		/// 使用 WaitAsync 异步锁保护释放操作，避免在多线程并发释放时
		/// Dispose() 和 DisposeAsync() 同时执行产生的竞态。
		/// </remarks>
		public override void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
			{
				return;
			}
			try
			{
				Task.Run(async delegate
				{
					try
					{
						await RemovePluginsAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex2)
					{
						LogHelper.Error("Dispose 内部卸载异常: " + ex2.Message, "Core/Plugin", ex2, true);
					}
				}).GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				LogHelper.Error("Dispose 异常: " + ex.Message, "Core/Plugin", ex, true);
			}
			_initLock.Dispose();
			base.Dispose();
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
			{
				try
				{
					await RemovePluginsAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception ex)
				{
					await LogHelper.ErrorAsync("DisposeAsync 异常: " + ex.Message, "Core/Plugin", ex, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
				_initLock.Dispose();
				await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// 同步初始化加载指定目录下的所有插件（向后兼容包装）。
		/// <para><b>推荐使用 <see cref="M:Snet.Core.plugin.PluginOperate.InitPluginAsync(System.String,System.String,System.Threading.CancellationToken)" /> 异步版本。</b></para>
		/// </summary>
		/// <param name="path">插件 DLL 所在目录路径</param>
		/// <param name="interfaceFullName">目标接口全名</param>
		/// <returns>插件信息和参数的元组列表</returns>
		public List<(PluginModel Model, object? Param)> InitPlugin(string path, string interfaceFullName)
		{
			return Task.Run(() => InitPluginAsync(path, interfaceFullName)).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 【新增】异步初始化加载指定目录下的所有插件。
		/// <para>
		/// <b>加载流程：</b>
		/// <list type="number">
		///   <item>扫描目录下所有匹配 DllWatcherPattern 的 DLL 文件</item>
		///   <item>为每个 DLL 创建独立的可回收 PluginLoadContext</item>
		///   <item>以内存流方式加载程序集（不锁定文件）</item>
		///   <item>筛选实现了 interfaceFullName 接口的公开类型</item>
		///   <item>创建基础实例并缓存到 IOC 容器</item>
		///   <item>记录加载上下文映射，支持后续热重载</item>
		/// </list>
		/// </para>
		/// </summary>
		/// <param name="path">插件 DLL 所在目录路径</param>
		/// <param name="interfaceFullName">目标接口全名</param>
		/// <param name="cancellationToken">取消令牌</param>
		/// <returns>插件信息和参数的元组列表</returns>
		public async Task<List<(PluginModel Model, object? Param)>> InitPluginAsync(string path, string interfaceFullName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException("path", "插件目录路径不能为空。");
			}
			if (string.IsNullOrWhiteSpace(interfaceFullName))
			{
				throw new ArgumentNullException("interfaceFullName", "接口全名不能为空。");
			}
			await _initLock.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				return await InitPluginCoreAsync(path, interfaceFullName, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				_initLock.Release();
			}
		}

		/// <summary>
		/// 【重构·异步】InitPlugin 的核心实现（在异步信号量保护下执行）。
		/// </summary>
		private async Task<List<(PluginModel Model, object? Param)>> InitPluginCoreAsync(string path, string interfaceFullName, CancellationToken cancellationToken)
		{
			Type interfaceType = ResolveInterfaceType(interfaceFullName);
			List<(PluginModel Model, object? Param)> result = new List<(PluginModel, object)>();
			ConcurrentDictionary<(string Dir, string FullTypeName), (Type Type, PluginLoadContext Context)> discoveredTypes = new ConcurrentDictionary<(string, string), (Type, PluginLoadContext)>();
			string[] array = SafeGetFiles(path, "Snet.*.dll");
			string[] array2 = array;
			foreach (string dllPath in array2)
			{
				cancellationToken.ThrowIfCancellationRequested();
				PluginLoadContext context = null;
				try
				{
					string fullPath = Path.GetFullPath(dllPath);
					context = new PluginLoadContext(fullPath, 52428800L);
					Type[] exportedTypes = (await context.LoadFromFileStreamAsync(fullPath, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).GetExportedTypes();
					foreach (Type type in exportedTypes)
					{
						if (!ShouldSkipType(type) && (interfaceType?.IsAssignableFrom(type) ?? ImplementsInterfaceByName(type, interfaceFullName)))
						{
							string item = type.FullName ?? type.Name;
							discoveredTypes.AddOrUpdate((path, item), (type, context), ((string Dir, string FullTypeName) _, (Type Type, PluginLoadContext Context) _) => (Type: type, Context: context));
						}
					}
				}
				catch (Exception ex)
				{
					await LogHelper.ErrorAsync("插件 DLL 加载失败: " + dllPath + " - " + ex.Message, "Core/Plugin", ex, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
					if (context != null)
					{
						try
						{
							context.Unload();
						}
						catch
						{
						}
					}
				}
			}
			foreach (KeyValuePair<(string, string), (Type, PluginLoadContext)> item2 in discoveredTypes)
			{
				cancellationToken.ThrowIfCancellationRequested();
				try
				{
					(Type, PluginLoadContext) value = item2.Value;
					Type pluginType = value.Item1;
					PluginLoadContext pluginContext = value.Item2;
					string dllPath = pluginType.Name;
					string text = pluginType.Namespace + "." + dllPath;
					string assemblyVersion = GetAssemblyVersion(pluginType.Assembly);
					object item3 = InstantiateAndCachePlugin(pluginType, dllPath, interfaceFullName, path);
					PluginType val = (PluginType)(interfaceFullName.Contains(((object)(PluginType)1/*cast due to constrained. prefix*/).ToString()) ? 1 : 0);
					string text2 = $"{text}.{{0}}.{val}.Config.json";
					result.Add((new PluginModel(dllPath, text, text2, path, assemblyVersion, val), item3));
					_pluginTypes.AddOrUpdate(dllPath, pluginType, (string _, Type _) => pluginType);
					if (_pluginContexts.TryGetValue(dllPath, out PluginLoadContext value2) && value2 != pluginContext)
					{
						try
						{
							value2.Unload();
							await LogHelper.InfoAsync("热重载：已卸载插件 '" + dllPath + "' 的旧版本程序集", "Core/Plugin", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
						}
						catch (Exception ex2)
						{
							await LogHelper.ErrorAsync("热重载：卸载旧版本失败 - " + ex2.Message, "Core/Plugin", ex2, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					_pluginContexts[dllPath] = pluginContext;
				}
				catch (Exception ex3)
				{
					await LogHelper.ErrorAsync("插件实例化失败: " + item2.Value.Item1.Name + " - " + ex3.Message, "Core/Plugin", ex3, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			return result;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public async Task<bool> RemovePluginAsync(string className, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrWhiteSpace(className))
			{
				return false;
			}
			if (!_pluginContexts.TryGetValue(className, out PluginLoadContext context))
			{
				return false;
			}
			List<string> relatedClassNames = (from kvp in _pluginContexts
				where kvp.Value == context
				select kvp.Key).ToList();
			foreach (string name in relatedClassNames)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (_daqInstances.TryRemove(name, out IDaq value) && value != null)
				{
					await SafeDisposeInstanceAsync(value, name, "DAQ").ConfigureAwait(continueOnCapturedContext: false);
				}
				if (_mqInstances.TryRemove(name, out IMq value2) && value2 != null)
				{
					await SafeDisposeInstanceAsync(value2, name, "MQ").ConfigureAwait(continueOnCapturedContext: false);
				}
				_pluginTypes.TryRemove(name, out Type _);
				_pluginContexts.TryRemove(name, out PluginLoadContext _);
			}
			try
			{
				context.Unload();
				await LogHelper.InfoAsync($"已卸载插件上下文（含 {relatedClassNames.Count} 个类型）: {string.Join(", ", relatedClassNames)}", "Core/Plugin", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await LogHelper.ErrorAsync("卸载插件上下文时出错: " + ex.Message, "Core/Plugin", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
			}
			return true;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public async Task RemovePluginsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			List<PluginLoadContext> uniqueContexts = _pluginContexts.Values.Distinct().ToList();
			foreach (KeyValuePair<string, IDaq> item in _daqInstances.ToList())
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (_daqInstances.TryRemove(item.Key, out IDaq value) && value != null)
				{
					await SafeDisposeInstanceAsync(value, item.Key, "DAQ").ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			foreach (KeyValuePair<string, IMq> item2 in _mqInstances.ToList())
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (_mqInstances.TryRemove(item2.Key, out IMq value2) && value2 != null)
				{
					await SafeDisposeInstanceAsync(value2, item2.Key, "MQ").ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			_pluginTypes.Clear();
			_pluginContexts.Clear();
			_interfaceTypeCache.Clear();
			foreach (PluginLoadContext item3 in uniqueContexts)
			{
				cancellationToken.ThrowIfCancellationRequested();
				try
				{
					item3.Unload();
				}
				catch
				{
				}
			}
			await LogHelper.InfoAsync($"已卸载全部插件（{uniqueContexts.Count} 个上下文）", "Core/Plugin", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public object? GetPluginParamObject(string interfaceFullName, string className)
		{
			if (string.IsNullOrWhiteSpace(className) || !_pluginTypes.ContainsKey(className))
			{
				return null;
			}
			IDaq value2;
			if (IsMqInterface(interfaceFullName))
			{
				if (_mqInstances.TryGetValue(className, out IMq value))
				{
					return ((ResultModel)((IArgs)value).GetArgs(true)).ResultData;
				}
			}
			else if (IsDaqInterface(interfaceFullName) && _daqInstances.TryGetValue(className, out value2))
			{
				return ((ResultModel)((IArgs)value2).GetArgs(true)).ResultData;
			}
			return null;
		}

		/// <summary>
		/// 【新增】异步获取插件的默认参数对象。
		/// </summary>
		public async Task<object?> GetPluginParamObjectAsync(string interfaceFullName, string className, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrWhiteSpace(className) || !_pluginTypes.ContainsKey(className))
			{
				return null;
			}
			IDaq value2;
			if (IsMqInterface(interfaceFullName))
			{
				if (_mqInstances.TryGetValue(className, out IMq value))
				{
					return ((ResultModel)(await ((IArgs)value).GetArgsAsync(true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))).ResultData;
				}
			}
			else if (IsDaqInterface(interfaceFullName) && _daqInstances.TryGetValue(className, out value2))
			{
				return ((ResultModel)(await ((IArgs)value2).GetArgsAsync(true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))).ResultData;
			}
			return null;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public object? ConvertPluginJsonParam(string className, string jsonContent)
		{
			//IL_00ac: Expected O, but got Unknown
			if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(jsonContent))
			{
				return null;
			}
			if (!_pluginTypes.TryGetValue(className, out Type value))
			{
				return null;
			}
			try
			{
				JObject val = JsonConvert.DeserializeObject<JObject>(jsonContent);
				if (val == null)
				{
					return null;
				}
				ConstructorInfo constructorInfo = value.GetConstructors().FirstOrDefault((ConstructorInfo c) => c.GetParameters().Length != 0);
				if ((object)constructorInfo == null)
				{
					LogHelper.Warning("插件 " + className + " 没有有参构造函数，无法进行参数转换", "Core/Plugin", (Exception)null, true);
					return null;
				}
				object[] array = ConvertParameters(new object[1] { val }, constructorInfo);
				return (array != null && array.Length > 0) ? array[0] : null;
			}
			catch (JsonException ex)
			{
				JsonException ex2 = ex;
				LogHelper.Error("JSON 反序列化失败: " + ((Exception)(object)ex2).Message, "Core/Plugin", (Exception)(object)ex2, true);
				return null;
			}
			catch (Exception ex3)
			{
				LogHelper.Error("参数转换失败: " + ex3.Message, "Core/Plugin", ex3, true);
				return null;
			}
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public async Task<OperateResult> StatusVerifyAsync(string interfaceFullName, string className, string data, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(data))
			{
				return OperateResult.CreateFailureResult("参数不能为空。", (int?)null);
			}
			if (!_pluginTypes.TryGetValue(className, out Type value))
			{
				return OperateResult.CreateFailureResult("未找到插件类型: " + className, (int?)null);
			}
			object obj = ConvertPluginJsonParam(className, data);
			if (obj == null)
			{
				return OperateResult.CreateFailureResult("参数转换失败，请检查 JSON 格式是否正确。", (int?)null);
			}
			object tempInstance = null;
			OperateResult result;
			try
			{
				tempInstance = Activator.CreateInstance(value, obj);
				if (tempInstance == null)
				{
					result = OperateResult.CreateFailureResult("无法创建插件实例。", (int?)null);
				}
				else
				{
					cancellationToken.ThrowIfCancellationRequested();
					IMq val = (IMq)((tempInstance is IMq) ? tempInstance : null);
					if (val != null)
					{
						result = await ((IOn)val).OnAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					else
					{
						IDaq val2 = (IDaq)((tempInstance is IDaq) ? tempInstance : null);
						result = ((val2 == null) ? OperateResult.CreateFailureResult("插件类型 " + className + " 既不是 IDaq 也不是 IMq 实现。", (int?)null) : (await ((IOn)val2).OnAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)));
					}
				}
			}
			catch (OperationCanceledException)
			{
				result = OperateResult.CreateFailureResult("验证操作已取消。", (int?)null);
			}
			catch (Exception ex2)
			{
				await LogHelper.ErrorAsync("连接验证异常: " + ex2.Message, "Core/Plugin", ex2, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				result = OperateResult.CreateFailureResult("验证过程发生异常: " + ex2.Message, (int?)null);
			}
			finally
			{
				if (tempInstance is IAsyncDisposable asyncDisposable)
				{
					try
					{
						await asyncDisposable.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				}
				else if (tempInstance is IDisposable disposable)
				{
					try
					{
						disposable.Dispose();
					}
					catch
					{
					}
				}
			}
			return result;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public string PluginFileNameToSN(string fileName, PluginType pluginType)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return string.Empty;
			}
			string text = $".{pluginType}.Config.json";
			if (!fileName.EndsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				return string.Empty;
			}
			int length = text.Length;
			return fileName.Substring(0, fileName.Length - length).Split('.').LastOrDefault() ?? string.Empty;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public string GetPluginPath(string className)
		{
			if (string.IsNullOrWhiteSpace(className))
			{
				return string.Empty;
			}
			if (_pluginContexts.TryGetValue(className, out PluginLoadContext value))
			{
				return Path.GetDirectoryName(value.PluginPath) ?? string.Empty;
			}
			return string.Empty;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public string GetVersion(string className)
		{
			if (string.IsNullOrWhiteSpace(className))
			{
				return string.Empty;
			}
			if (_pluginTypes.TryGetValue(className, out Type value))
			{
				return GetAssemblyVersion(value.Assembly);
			}
			return "1.0.0";
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public T? GetPluginInterfaceObject<T>(string className) where T : class
		{
			if (string.IsNullOrWhiteSpace(className))
			{
				return null;
			}
			if (typeof(IDaq).IsAssignableFrom(typeof(T)))
			{
				if (!_daqInstances.TryGetValue(className, out IDaq value) || !(value is T result))
				{
					return null;
				}
				return result;
			}
			if (typeof(IMq).IsAssignableFrom(typeof(T)))
			{
				if (!_mqInstances.TryGetValue(className, out IMq value2) || !(value2 is T result2))
				{
					return null;
				}
				return result2;
			}
			if (_daqInstances.TryGetValue(className, out IDaq value3) && value3 is T result3)
			{
				return result3;
			}
			if (_mqInstances.TryGetValue(className, out IMq value4) && value4 is T result4)
			{
				return result4;
			}
			return null;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public async Task<T?> CreateAsync<T>(string className, string param, PluginType type)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				string text = default(string);
				IDaq value2;
				if ((int)type != 0)
				{
					if ((int)type == 1 && _mqInstances.TryGetValue(className, out IMq value))
					{
						OperateResult val = await ((IInstance)value).CreateInstanceAsync(param, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
						IMq val2 = default(IMq);
						if (((ResultModel)val).GetDetails<IMq>(ref text, ref val2))
						{
							return ExtendMethod.GetSource<T>((object)val2);
						}
						await LogHelper.WarningAsync("MQ 实例创建失败: " + className + " - " + ((BaseModel)val).Message, "Core/Plugin", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
						return default(T);
					}
				}
				else if (_daqInstances.TryGetValue(className, out value2))
				{
					OperateResult val3 = await ((IInstance)value2).CreateInstanceAsync(param, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
					IDaq val4 = default(IDaq);
					if (((ResultModel)val3).GetDetails<IDaq>(ref text, ref val4))
					{
						return ExtendMethod.GetSource<T>((object)val4);
					}
					await LogHelper.WarningAsync("DAQ 实例创建失败: " + className + " - " + ((BaseModel)val3).Message, "Core/Plugin", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
					return default(T);
				}
				await LogHelper.WarningAsync($"CreateAsync 未找到类型为 {type} 的基础实例: {className}", "Core/Plugin", (Exception)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				return default(T);
			}
			catch (Exception ex)
			{
				await LogHelper.ErrorAsync($"CreateAsync 异常 [{type}:{className}]: {ex.Message}", "Core/Plugin", ex, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				return default(T);
			}
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public bool IsPluginLoaded(string className)
		{
			if (!string.IsNullOrWhiteSpace(className))
			{
				return _pluginTypes.ContainsKey(className);
			}
			return false;
		}

		/// <inheritdoc cref="T:Snet.Core.plugin.PluginOperate" />
		public string GetPluginDiagnostics(string className)
		{
			if (string.IsNullOrWhiteSpace(className))
			{
				return "类名为空。";
			}
			if (!_pluginContexts.TryGetValue(className, out PluginLoadContext value))
			{
				return "未找到插件 '" + className + "' 的加载上下文。";
			}
			return $"插件: {className}, 路径: {value.PluginPath}, 已加载程序集: {value.LoadedAssemblyCount}, 上下文已卸载: {value.IsFullyUnloaded()}";
		}

		/// <summary>
		/// 安全地扫描目录中的 DLL 文件。
		/// 捕获异常返回空数组，确保加载流程不会因目录问题中断。
		/// </summary>
		private static string[] SafeGetFiles(string directory, string searchPattern)
		{
			try
			{
				if (!Directory.Exists(directory))
				{
					LogHelper.Warning("插件目录不存在: " + directory, "Core/Plugin", (Exception)null, true);
					return Array.Empty<string>();
				}
				return Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
			}
			catch (UnauthorizedAccessException ex)
			{
				LogHelper.Error("无权访问插件目录: " + directory + " - " + ex.Message, "Core/Plugin", (Exception)ex, true);
				return Array.Empty<string>();
			}
			catch (Exception ex2)
			{
				LogHelper.Error("扫描插件目录失败: " + directory + " - " + ex2.Message, "Core/Plugin", ex2, true);
				return Array.Empty<string>();
			}
		}

		/// <summary>
		/// 判断类型是否应被跳过（内部抽象基类或辅助类型）。
		/// 使用 HashSet 进行 O(1) 查找。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ShouldSkipType(Type type)
		{
			foreach (string skipTypeName in SkipTypeNames)
			{
				if (type.Name.Contains(skipTypeName))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 解析接口全名为 Type 对象（带缓存）。
		/// 仅缓存成功解析的结果，失败时不缓存。
		/// 下次调用时重新尝试解析，避免因加载顺序导致的永久失败。
		/// </summary>
		private Type? ResolveInterfaceType(string interfaceFullName)
		{
			if (_interfaceTypeCache.TryGetValue(interfaceFullName, out Type value))
			{
				return value;
			}
			Type type = Type.GetType(interfaceFullName, throwOnError: false);
			if ((object)type != null)
			{
				_interfaceTypeCache[interfaceFullName] = type;
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				try
				{
					if (!assembly.IsDynamic)
					{
						type = assembly.GetType(interfaceFullName, throwOnError: false);
						if ((object)type != null)
						{
							_interfaceTypeCache[interfaceFullName] = type;
							return type;
						}
					}
				}
				catch
				{
				}
			}
			LogHelper.Warning("无法解析接口类型: " + interfaceFullName + "，将回退到字符串匹配模式", "Core/Plugin", (Exception)null, true);
			return null;
		}

		/// <summary>
		/// 通过接口名称字符串匹配检查类型是否实现了指定接口（回退方案）。
		/// </summary>
		private static bool ImplementsInterfaceByName(Type type, string interfaceFullName)
		{
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (string.Equals(interfaces[i].FullName, interfaceFullName, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 根据接口类型创建插件实例并缓存到对应的 IOC 容器。
		/// </summary>
		private object? InstantiateAndCachePlugin(Type pluginType, string className, string interfaceFullName, string pluginPath)
		{
			if (IsMqInterface(interfaceFullName))
			{
				return InstantiateAndCache(className, delegate
				{
					object? obj = Activator.CreateInstance(pluginType);
					return (IMq?)((obj is IMq) ? obj : null);
				}, (IMq instance) => ((ResultModel)((IArgs)instance).GetArgs(true)).ResultData, _mqInstances, "MQ");
			}
			if (IsDaqInterface(interfaceFullName))
			{
				return InstantiateAndCache(className, delegate
				{
					object? obj = Activator.CreateInstance(pluginType);
					return (IDaq?)((obj is IDaq) ? obj : null);
				}, (IDaq instance) => ((ResultModel)((IArgs)instance).GetArgs(true)).ResultData, _daqInstances, "DAQ");
			}
			LogHelper.Warning("插件 " + className + " 的接口类型无法识别: " + interfaceFullName, "Core/Plugin", (Exception)null, true);
			return null;
		}

		/// <summary>
		/// 通用的插件实例创建与缓存方法。
		/// 使用 ConcurrentDictionary.GetOrAdd 原子操作消除 TOCTOU 竞态条件。
		/// </summary>
		private object? InstantiateAndCache<TInterface>(string className, Func<TInterface?> factory, Func<TInterface, object?> paramExtractor, ConcurrentDictionary<string, TInterface> cache, string typeTag) where TInterface : class
		{
			try
			{
				if (cache.TryGetValue(className, out TInterface value))
				{
					return paramExtractor(value);
				}
				TInterface val = factory();
				if (val == null)
				{
					LogHelper.Warning("创建 " + typeTag + " 插件实例返回 null: " + className, "Core/Plugin", (Exception)null, true);
					return null;
				}
				TInterface orAdd = cache.GetOrAdd(className, val);
				if (orAdd != val)
				{
					LogHelper.Info(typeTag + " 插件 '" + className + "' 并发创建冲突，释放多余实例", "Core/Plugin", (Exception)null, true);
					SafeDisposeInstanceAsync(val, className, typeTag);
					return paramExtractor(orAdd);
				}
				return paramExtractor(val);
			}
			catch (Exception ex)
			{
				LogHelper.Error($"实例化 {typeTag} 插件失败: {className} - {ex.Message}", "Core/Plugin", ex, true);
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsMqInterface(string interfaceFullName)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return interfaceFullName.Contains(((object)(PluginType)1/*cast due to constrained. prefix*/).ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsDaqInterface(string interfaceFullName)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return interfaceFullName.Contains(((object)(PluginType)0/*cast due to constrained. prefix*/).ToString());
		}

		/// <summary>
		/// 安全地释放插件实例。
		/// 始终执行释放，不检查连接状态。
		/// Dispose/DisposeAsync 的契约是"安全地释放所有资源，无论对象处于何种状态"。
		/// </summary>
		private static async Task SafeDisposeInstanceAsync(object instance, string className, string typeTag)
		{
			try
			{
				if (instance is IAsyncDisposable asyncDisposable)
				{
					await asyncDisposable.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				else if (instance is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			catch (Exception ex)
			{
				await LogHelper.ErrorAsync($"释放 {typeTag} 实例 '{className}' 时出错: {ex.Message}", "Core/Plugin", ex, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 安全地提取程序集版本号。
		/// 正确处理 null 版本和末尾 .0 段的修剪。
		/// </summary>
		private static string GetAssemblyVersion(Assembly assembly)
		{
			Version version = assembly.GetName().Version;
			if ((object)version == null)
			{
				return "0";
			}
			return TrimTrailingZeroSegments(version.ToString());
		}

		/// <summary>
		/// 修剪版本号字符串末尾的 ".0" 段。
		/// 示例: "1.0.0.0" → "1", "2.0.10.0" → "2.0.10", "3.2.1.0" → "3.2.1"
		/// </summary>
		private static string TrimTrailingZeroSegments(string version)
		{
			while (version.EndsWith(".0") && version.Length > 2)
			{
				string text = version;
				version = text.Substring(0, text.Length - 2);
			}
			if (!string.IsNullOrEmpty(version))
			{
				return version;
			}
			return "0";
		}

		/// <summary>
		/// 将源参数数组转换为目标方法/构造函数所需的参数类型。
		/// <para>
		/// <b>修复内容：</b><br />
		/// 原代码调用 <c>PluginOperate.ConvertParameters(ConstructorParam, constructorInfo)</c>，
		/// 但 <c>ConvertParameters</c> 在 <see cref="T:Snet.Core.plugin.PluginOperate" /> 中是 private 实例方法，
		/// 无法以静态方式从外部调用（会导致编译错误）。<br /><br />
		/// 修复方案：在本类中提供本地的 ConvertParameters 实现，
		/// 与 PluginOperate 中的版本保持相同的转换逻辑。
		/// </para>
		/// </summary>
		/// <param name="sourceValues">源参数值数组</param>
		/// <param name="method">目标方法或构造函数的元数据</param>
		/// <returns>转换后的参数数组；源值为 null 或空时返回 null</returns>
		public static object?[]? ConvertParameters(object[]? sourceValues, MethodBase? method)
		{
			if (sourceValues == null || sourceValues.Length <= 0)
			{
				return null;
			}
			if ((object)method == null)
			{
				return null;
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length == 0)
			{
				return null;
			}
			int num = Math.Min(sourceValues.Length, parameters.Length);
			object[] array = new object[num];
			bool flag = method is ConstructorInfo;
			for (int i = 0; i < num; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				object obj = sourceValues[i];
				if (obj == null)
				{
					array[i] = GetDefaultValue(parameterType);
				}
				else if (flag)
				{
					array[i] = ConvertToComplexType(obj, parameterType);
				}
				else
				{
					array[i] = ConvertToSimpleType(obj, parameterType);
				}
			}
			return array;
		}

		/// <summary>
		/// 简单类型转换（基本类型、枚举、实现了 IConvertible 的类型）。
		/// </summary>
		private static object? ConvertToSimpleType(object value, Type targetType)
		{
			try
			{
				Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
				if (value.GetType() == targetType || value.GetType() == type)
				{
					return value;
				}
				if (type.IsEnum)
				{
					if (value is string value2)
					{
						return Enum.Parse(type, value2, ignoreCase: true);
					}
					return Enum.ToObject(type, value);
				}
				return Convert.ChangeType(value, type);
			}
			catch
			{
				return GetDefaultValue(targetType);
			}
		}

		/// <summary>
		/// 复杂类型转换：通过 JObject 进行属性级别的深度映射。
		/// 优先使用 JObject.ToObject(Type) 一次性完成映射。
		/// </summary>
		private static object? ConvertToComplexType(object value, Type targetType)
		{
			try
			{
				JObject val = (JObject)((value is JObject) ? value : null);
				if (val == null)
				{
					val = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(value));
				}
				if (val == null)
				{
					return GetDefaultValue(targetType);
				}
				try
				{
					return ((JToken)val).ToObject(targetType);
				}
				catch
				{
					return ManualPropertyMapping(val, targetType);
				}
			}
			catch
			{
				return GetDefaultValue(targetType);
			}
		}

		/// <summary>
		/// 手动属性映射（作为 JObject.ToObject 的回退方案）。
		/// 逐属性遍历目标类型，从 JObject 中查找匹配的属性并转换赋值。
		/// </summary>
		private static object ManualPropertyMapping(JObject jObj, Type targetType)
		{
			object obj = Activator.CreateInstance(targetType);
			PropertyInfo[] properties = targetType.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.CanWrite)
				{
					continue;
				}
				JToken val = jObj[propertyInfo.Name];
				if (val != null)
				{
					try
					{
						object value = val.ToObject(propertyInfo.PropertyType);
						propertyInfo.SetValue(obj, value);
					}
					catch
					{
					}
				}
			}
			return obj;
		}

		/// <summary>
		/// 获取类型的默认值。
		/// 值类型返回零值，引用类型返回 null。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static object? GetDefaultValue(Type type)
		{
			if (type == typeof(string))
			{
				return null;
			}
			if (type.IsValueType)
			{
				return Activator.CreateInstance(Nullable.GetUnderlyingType(type) ?? type);
			}
			return null;
		}
	}
}
namespace Snet.Core.packer
{
	/// <summary>
	/// 罗克韦尔 AllenBradley SLC500（PCCC/DF1）协议组包器。
	/// 驱动语义（AllenBradleySLCAddress）："{类型}{文件号}:{元素号}"（B3:0、N7:10、ST5:0、A9:0）。
	/// 元素字节宽：B/N/S/C/I/O/R/T = 2 字节（1 字），A/F/L = 4 字节，ST = 82 字节（字符串）。
	/// 默认文件号：S→2、O→0、I→1、ST→1，其余类型必须显式带号。
	/// 位寻址 "B3:0/5" 为元素内位：带 / 的地址不可组包（驱动 ReadBool 单值读取，批首无法重建）；
	/// 无 / 的 Bool 按元素位 0 处理（驱动 AnalysisBitIndex 无 / 时位号为 0）。
	/// 区域键 = 类型 + 文件号（如 B3、N7、I1），跨文件不混批。
	/// </summary>
	public sealed class AllenBradleyPacker : PackerBase
	{
		private readonly record struct SlcAddr(string Region, int ByteIndex, int BitIndex);

		/// <summary>
		/// 类型字母 → 元素字节宽（PCCC 数据文件元素大小，SLC500 标准）。
		/// T/C/R（定时器/计数器/控制）为 3 字 6 字节，L（长整型）为 4 字 8 字节，ST（字符串）= 2 字节长度 + 82 字符 = 84 字节。
		/// A（ASCII）不在此列：驱动 BuildReadCommand 对 A 的地址做 /=2（元素按字寻址），与字节偏移模型矛盾，故不支持组包。
		/// </summary>
		private static readonly Dictionary<char, int> ElementBytes = new Dictionary<char, int>
		{
			['B'] = 2,
			['N'] = 2,
			['S'] = 2,
			['I'] = 2,
			['O'] = 2,
			['T'] = 6,
			['C'] = 6,
			['R'] = 6,
			['F'] = 4,
			['L'] = 8,
			['G'] = 0
		};

		public override ProtocolFamily Family => ProtocolFamily.AllenBradley;

		public override int MaxReadLength => 500;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 覆写分组：按 DataType 分离位/字区域（|B / |W）。
		/// SLC 驱动位读（ReadBool）与字读（Read）是不同命令，同地址（如 B3:0 Bool + B3:0 Word）必须分离。
		/// 无 / 的 Bool 按元素位 0 处理（驱动 AnalysisBitIndex 默认 0）。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, flag ? result.BitIndex : 0, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		/// <summary>
		/// 解析 SLC 地址。格式："{类型}{文件号}:{元素号}" 或 "{类型}{文件号}:{元素号}/{位}"。
		/// 带 / 位寻址返回失败（不可组包，驱动单值读取批首无法重建）。
		/// 元素号按类型字节宽换算字节偏移；ST 字符串固定 82 字节。
		/// </summary>
		private static bool TryParse(string address, out SlcAddr result)
		{
			result = default(SlcAddr);
			int num = address.IndexOf(':');
			if (num <= 0)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan(0, num);
			ReadOnlySpan<char> span = address.AsSpan(num + 1);
			if (span.IndexOf('/') >= 0)
			{
				return false;
			}
			bool flag = readOnlySpan.Length >= 2 && (readOnlySpan[0] == 'S' || readOnlySpan[0] == 's') && (readOnlySpan[1] == 'T' || readOnlySpan[1] == 't');
			char c = (flag ? 'S' : char.ToUpperInvariant(readOnlySpan[0]));
			if (!ElementBytes.TryGetValue(c, out var value) || value == 0)
			{
				return false;
			}
			ReadOnlySpan<char> span2 = (flag ? readOnlySpan.Slice(2) : readOnlySpan.Slice(1));
			int value2;
			if (span2.IsEmpty)
			{
				value2 = c switch
				{
					'S' => 2, 
					'O' => 0, 
					'I' => 1, 
					_ => -1, 
				};
			}
			else if (!TryParseDecimal(span2, out value2))
			{
				return false;
			}
			if (value2 < 0)
			{
				return false;
			}
			if (!TryParseDecimal(span, out var value3))
			{
				return false;
			}
			int byteIndex = (flag ? (value3 * 84) : (value3 * value));
			result = new SlcAddr($"{(flag ? "ST" : c.ToString())}{value2}", byteIndex, 0);
			return true;
		}

		private static bool TryParseDecimal(ReadOnlySpan<char> span, out int value)
		{
			value = 0;
			if (span.IsEmpty)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
				value = value * 10 + (c - 48);
			}
			return true;
		}
	}
	/// <summary>
	/// 倍福 Beckhoff ADS 协议组包器。
	/// 驱动语义（AdsHelper.AnalysisAddress / BeckhoffAdsNet）：
	/// 地址格式：M100 / M100.3（M 区）、I100 / Q100（I/Q 区）、i=100000（内存地址）、ig=0xF080;100（自定义 IndexGroup）。
	/// 字读：Offset = 地址号（字节地址，同 S7 模型）；位读：Offset = 位号（无点号 = n，点号 = n×8+bit），
	///   响应每字节 1 位（ReadBool → Content.Select(m =&gt; m != 0)）→ 位号连续、1 位 1 字节。
	/// 位读/字读是不同 IndexGroup（M 16417/16416、I 61473/61472、Q 61489/61488）→ Bool/Word 必须分区域。
	/// s= 符号地址：驱动 TransValueHandle 查符号表转 i= 内存地址，偏移不可预知 → 不可组包（降级原样保留）。
	/// 驱动 Read 无分帧（单 AMS 命令）→ MaxReadLength 保守 1024。
	/// </summary>
	public sealed class BeckhoffPacker : PackerBase
	{
		private readonly record struct BeckRegion(string Region, int ByteIndex, bool HasDot);

		/// <summary>位区类型（驱动 AnalysisAddress isBit:true 支持的区）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "M", "I", "Q" };

		public override ProtocolFamily Family => ProtocolFamily.Beckhoff;

		/// <summary>驱动无分帧（单 AMS 命令）；ADS 帧数据段宽裕，但保守取 1024 字节避免超 PLC 实际能力</summary>
		public override int MaxReadLength => 1024;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: 0);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 点号地址（M100.3）只能位读：驱动字读对 "100.3" 解析失败 → Word 类型降级原样保留。
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)dataType == 2)
			{
				return true;
			}
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			return address.IndexOf('.') < 0;
		}

		/// <summary>
		/// 位/字区域分离（|B / |W）：
		/// - Bool：位号直接作字节偏移（1 位 1 字节），BitIndex=0；点号地址 n.m → 位号 n×8+m
		/// - 非 Bool：无点号字节地址（点号地址已在 IsPackable 降级）
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, 0, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		/// <summary>
		/// 解析倍福地址。s= 符号地址不解析（返回 false → 走未组包）。
		/// 字语义：M100 → 字节 100；有点号 M100.3 → 位号 803（供 Bool 使用，Word 会拒绝）。
		/// </summary>
		private static bool TryParse(string address, out BeckRegion result)
		{
			result = default(BeckRegion);
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			if (address.StartsWith("s=", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (address.StartsWith("i=", StringComparison.OrdinalIgnoreCase))
			{
				if (int.TryParse(address.AsSpan(2), out var result2) && result2 >= 0)
				{
					result = new BeckRegion("MEM", result2, HasDot: false);
					return true;
				}
				return false;
			}
			if (address.StartsWith("ig=", StringComparison.OrdinalIgnoreCase))
			{
				int num = address.IndexOf(';');
				if (num > 0)
				{
					string text = address.Substring(3, num - 3).Trim();
					int num2 = ParseNumber(text, text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? 16 : 10);
					if (num2 >= 0 && int.TryParse(address.AsSpan(num + 1), out var result3) && result3 >= 0)
					{
						result = new BeckRegion($"IG{num2}", result3, HasDot: false);
						return true;
					}
				}
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text2 = readOnlySpan.Slice(0, i).ToString();
			if (!BitTypes.Contains(text2))
			{
				return false;
			}
			int num3 = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num3)
			{
				return false;
			}
			if (!int.TryParse(readOnlySpan.Slice(num3, i - num3), out var result4) || result4 < 0)
			{
				return false;
			}
			bool flag = false;
			int result5 = 0;
			if (i < readOnlySpan.Length && readOnlySpan[i] == '.')
			{
				i++;
				int num4 = i;
				for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
				{
				}
				if (i <= num4 || !int.TryParse(readOnlySpan.Slice(num4, i - num4), out result5))
				{
					return false;
				}
				flag = true;
			}
			int byteIndex = (flag ? (result4 * 8 + result5) : result4);
			result = new BeckRegion(text2.ToUpperInvariant(), byteIndex, flag);
			return true;
		}

		private static int ParseNumber(string text, int radix)
		{
			text = text.Trim();
			if (radix == 16 && text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(2);
			}
			if (text.Length == 0)
			{
				return -1;
			}
			int num = 0;
			string text2 = text;
			foreach (char c in text2)
			{
				int num2;
				if (c >= '0' && c <= '9')
				{
					num2 = c - 48;
				}
				else if (radix == 16 && c >= 'A' && c <= 'F')
				{
					num2 = c - 65 + 10;
				}
				else
				{
					if (radix != 16 || c < 'a' || c > 'f')
					{
						return -1;
					}
					num2 = c - 97 + 10;
				}
				num = num * radix + num2;
			}
			return num;
		}
	}
	/// <summary>
	/// Cimon HMI 协议组包器。
	/// 驱动语义（CimonHelper.PackReadWriteCommand / CimonHmiProtocol）：
	/// 地址类型仅支持 Y/M/L/K/F/T/C/D/S/X（"YMLKFTCDSX"），地址号 PadLeft(6) 直传、length 直传（字节单位）。
	/// 字读（控制码82）：地址号直接 → ByteIndex = 地址号（1 元素 = 1 字节，非 ×2）。
	/// 位读（控制码114）：length 单位字节（每位 1 字节），地址号直接 → ByteIndex = 地址号。
	/// "D{字}.{hex位}" 特例：走 SnetHelper.ReadBool(16) 字内位 → (字×2 + 位/8, 位%8)。
	/// 位/字用不同命令 → Bool/Word 必须分区域。
	/// </summary>
	public sealed class CimonPacker : PackerBase
	{
		private readonly record struct CimonRegion(string Region, int ByteIndex, int BitIndex);

		private static readonly HashSet<string> AllowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Y", "M", "L", "K", "F", "T", "C", "D", "S", "X" };

		public override ProtocolFamily Family => ProtocolFamily.Cimon;

		public override int MaxReadLength => 500;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 覆写分组：Bool → |B 位读（地址号直接作字节），其他 → |W 字读（地址号直接）。
		/// "D{字}.{hex位}" 特例只按位区（|B）。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, result.BitIndex, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		private static bool IsHex(char c)
		{
			switch (c)
			{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
			case 'a':
			case 'b':
			case 'c':
			case 'd':
			case 'e':
			case 'f':
				return true;
			default:
				return false;
			}
		}

		/// <summary>位区/字区：地址号直接作字节偏移（1 元素 = 1 字节）；D 点号特例 → 字×2 + 位/8。</summary>
		private static bool TryParse(string address, out CimonRegion result)
		{
			result = default(CimonRegion);
			if (address.Length > 0 && char.IsDigit(address[0]))
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			if (!AllowedTypes.Contains(text))
			{
				return false;
			}
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			for (int j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
			}
			int num2 = int.Parse(readOnlySpan.Slice(num, i - num));
			if (text == "D" && i < readOnlySpan.Length && readOnlySpan[i] == '.')
			{
				i++;
				int num3 = i;
				for (; i < readOnlySpan.Length && IsHex(readOnlySpan[i]); i++)
				{
				}
				if (i <= num3)
				{
					return false;
				}
				int num4 = 0;
				ReadOnlySpan<char> readOnlySpan3 = readOnlySpan.Slice(num3, i - num3);
				for (int j = 0; j < readOnlySpan3.Length; j++)
				{
					char c2 = readOnlySpan3[j];
					int num5 = ((c2 >= '0' && c2 <= '9') ? (c2 - 48) : (char.ToUpperInvariant(c2) - 65 + 10));
					num4 = num4 * 16 + num5;
				}
				result = new CimonRegion("D", num2 * 2 + num4 / 8, num4 % 8);
				return true;
			}
			result = new CimonRegion(text, num2, 0);
			return true;
		}
	}
	/// <summary>
	/// 发那科 FANUC PMC 协议组包器。
	/// 驱动语义（FanucHelper.AnalysisFanucAddress）：
	/// 字读仅支持 D/AI/AQ/SR/GI/GO（DataCode 8/10/12）；位读仅支持 I/Q/M + SDO/SDI/RDI/RDO/UI/UO/SI/SO（DataCode 70/72/76）。
	/// 地址号直接进报文（无 ×2/÷8）；位读取每位 1 字节，字读取每字 2 字节。
	/// 别名区映射（驱动）：SDO/GO 区分支、SDI/RDI/UI/SI 等偏移。
	/// 位/字用不同命令（ReadBool isBit:true / Read isBit:false）→ Bool/Word 必须分区域。
	/// </summary>
	public sealed class FanucPacker : PackerBase
	{
		private readonly record struct FanucRegion(string Region, int ByteIndex, int BitIndex);

		/// <summary>字区（驱动支持字读）</summary>
		private static readonly HashSet<string> WordTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "D", "AI", "AQ", "SR", "GI", "GO" };

		/// <summary>位区（驱动支持位读）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"I", "Q", "M", "SDO", "SDI", "RDI", "RDO", "UI", "UO", "SI",
			"SO"
		};

		public override ProtocolFamily Family => ProtocolFamily.Fanuc;

		/// <summary>驱动字读 length 单位为字（Read 返回 length×2 字节），单命令无分片；
		/// 组包字节批被驱动当字数读 → 保守上限 128 字（128 字节批 → 驱动读 128 字），避免超 PMC 上限</summary>
		public override int MaxReadLength => 128;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 类型-区域匹配过滤：
		/// - 位区 Word → 降级：驱动字读位区 AddressStart 直接用位号（字读与位读地址单位不同），
		///   与 packer 的位区字节（1位1字节）位置错位
		/// - 字区 Bool → 降级：驱动位读仅支持 I/Q/M + 别名位区（DataCode 70/72/76），字区无位读码
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if (!TryParse(address, out var result))
			{
				return true;
			}
			return (int)dataType == 2 == BitTypes.Contains(result.Region);
		}

		/// <summary>
		/// 覆写分组：Bool → |B 位读（仅位区），其他 → |W 字读（仅字区）。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, 0, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		/// <summary>字区地址号直接 ×2（字）；位区地址号直接作字节（1位=1字节）。别名区按驱动偏移映射。</summary>
		private static bool TryParse(string address, out FanucRegion result)
		{
			result = default(FanucRegion);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			int j;
			for (j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
			}
			int num2 = int.Parse(readOnlySpan.Slice(num, i - num));
			if (num2 == 0)
			{
				return false;
			}
			if (text == null)
			{
				goto IL_02e2;
			}
			j = text.Length;
			string region;
			int num3;
			if (j != 2)
			{
				if (j != 3)
				{
					goto IL_02e2;
				}
				char c2 = text[0];
				if (c2 != 'R')
				{
					if (c2 != 'S')
					{
						goto IL_02e2;
					}
					if (!(text == "SDO"))
					{
						if (!(text == "SDI"))
						{
							goto IL_02e2;
						}
						region = "SDI";
						num3 = num2;
					}
					else
					{
						region = "SDO";
						num3 = ((num2 < 11001) ? num2 : (num2 - 11000));
					}
				}
				else if (!(text == "RDI"))
				{
					if (!(text == "RDO"))
					{
						goto IL_02e2;
					}
					region = "RDO";
					num3 = num2 + 5000;
				}
				else
				{
					region = "RDI";
					num3 = num2 + 5000;
				}
			}
			else
			{
				char c2 = text[0];
				if (c2 != 'G')
				{
					if (c2 != 'S')
					{
						if (c2 != 'U')
						{
							goto IL_02e2;
						}
						if (!(text == "UI"))
						{
							if (!(text == "UO"))
							{
								goto IL_02e2;
							}
							region = "UO";
							num3 = num2 + 6000;
						}
						else
						{
							region = "UI";
							num3 = num2 + 6000;
						}
					}
					else
					{
						switch (text)
						{
						case "SI":
							break;
						case "SO":
							goto IL_028a;
						case "SR":
							goto IL_02bd;
						default:
							goto IL_02e2;
						}
						region = "SI";
						num3 = num2 + 7000;
					}
				}
				else
				{
					if (!(text == "GO"))
					{
						goto IL_02e2;
					}
					region = "GO";
					num3 = ((num2 >= 10001) ? (num2 - 6000) : num2);
				}
			}
			goto IL_02e9;
			IL_028a:
			region = "SO";
			num3 = num2 + 7000;
			goto IL_02e9;
			IL_02bd:
			region = "SR";
			num3 = ((num2 >= 1 && num2 <= 6) ? (5891 + (num2 - 1) * 40) : (-1));
			goto IL_02e9;
			IL_02e2:
			region = text;
			num3 = num2;
			goto IL_02e9;
			IL_02e9:
			if (num3 < 0)
			{
				return false;
			}
			bool flag = WordTypes.Contains(text);
			bool flag2 = BitTypes.Contains(text);
			if (!flag && !flag2)
			{
				return false;
			}
			result = (flag ? new FanucRegion(region, num3 * 2, 0) : new FanucRegion(region, num3, 0));
			return true;
		}
	}
	/// <summary>
	/// 永宏 Fatek 协议组包器。
	/// 驱动语义（FatekProgramAddress / FatekProgramHelper）：
	/// 位区（X/Y/M/S/T/C/RT/RC）：地址号直接作位号（每位 1 字节，BuildReadBoolCommand "44" 命令，上限 255 位）。
	/// 字区（D/R）：地址号 ×2 字节（BuildReadWordCommand "45" 命令按字，上限 64 字）。
	/// 位/字用不同命令 → Bool/Word 必须分区域。
	/// </summary>
	public sealed class FatekPacker : PackerBase
	{
		private readonly record struct FatekRegion(string Region, int ByteIndex, int BitIndex);

		/// <summary>位区（驱动 ReadBool 支持；地址号直接）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y", "M", "S", "T", "C", "RT", "RC" };

		/// <summary>字区（驱动 Read 支持）</summary>
		private static readonly HashSet<string> WordTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "D", "R" };

		public override ProtocolFamily Family => ProtocolFamily.Fatek;

		/// <summary>驱动单次上限：字路径 64 字 = 128 字节 → 取此值保证字路径不超</summary>
		public override int MaxReadLength => 128;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。驱动自动分帧（64字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 类型-区域匹配过滤：
		/// - 位区 Word → 降级：驱动字读（命令 45）位区地址按字寻址，与 packer 位区字节（1位1字节）错位
		/// - 字区 Bool → 降级：驱动位读（命令 44）仅支持 X/Y/M/S/T/C/RT/RC 位区
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if (!TryParse(address, out var result))
			{
				return true;
			}
			return (int)dataType == 2 == BitTypes.Contains(result.Region);
		}

		/// <summary>
		/// 覆写分组：Bool → |B 位读（仅位区），其他 → |W 字读（仅字区）。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, 0, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		private static bool TryParse(string address, out FatekRegion result)
		{
			result = default(FatekRegion);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			for (int j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
			}
			int num2 = int.Parse(readOnlySpan.Slice(num, i - num));
			if (BitTypes.Contains(text))
			{
				result = new FatekRegion(text, num2, 0);
				return true;
			}
			if (WordTypes.Contains(text))
			{
				result = new FatekRegion(text, num2 * 2, 0);
				return true;
			}
			return false;
		}
	}
	/// <summary>
	/// 欧姆龙 FINS 协议组包器。
	/// 支持 FINS TCP 的内存区地址批量优化。
	/// 地址格式：DM100, CIO100.03, WR100, HR100, AR100, EM0.100, TIM100, CNT100
	///
	/// 重要设计决策：
	/// - 字访问和位访问使用不同的区域键（后缀 _WORD / _BIT），因为 FINS 对它们使用不同的指令。
	/// - EM 区域在每个区域键中包含库编号（例如 "EM0", "EM1"），因为每个库是独立的物理内存。
	/// </summary>
	public sealed class FinsPacker : PackerBase
	{
		/// <summary>区域码→规范名映射</summary>
		private static readonly Dictionary<string, string> RegionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["DM"] = "DM",
			["D"] = "DM",
			["CIO"] = "CIO",
			["C"] = "CIO",
			["WR"] = "WR",
			["W"] = "WR",
			["HR"] = "HR",
			["H"] = "HR",
			["AR"] = "AR",
			["A"] = "AR",
			["TIM"] = "TIM",
			["CNT"] = "CNT",
			["EM"] = "EM",
			["E"] = "EM",
			["IR"] = "IR",
			["DR"] = "DR",
			["CF"] = "CF"
		};

		/// <summary>支持位访问的区域（驱动 OmronFinsDataType BitCode/WordCode 区分；TIM/CNT 带点号位读接点）</summary>
		private static readonly HashSet<string> BitAccessRegions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"CIO", "C", "WR", "W", "HR", "H", "AR", "A", "IR", "DR",
			"TIM", "CNT"
		};

		public override ProtocolFamily Family => ProtocolFamily.Omron;

		public override int MaxReadLength => 1000;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。OmronFinsNetHelper 自动分帧（500字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			string text = readOnlySpan.Slice(0, i).ToString();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			bool flag = ((text == "EM" || text == "E") ? true : false);
			int result = 0;
			int value = -1;
			if (flag)
			{
				if (i > num && !TryParseHex(readOnlySpan.Slice(num, i - num), out value))
				{
					return (ByteIndex: -1, BitIndex: 0);
				}
				if (i < readOnlySpan.Length && readOnlySpan[i] == '.')
				{
					i++;
					int num2 = i;
					for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
					{
					}
					if (i > num2 && !int.TryParse(readOnlySpan.Slice(num2, i - num2).ToString(), out result))
					{
						return (ByteIndex: -1, BitIndex: 0);
					}
				}
			}
			else if (i > num && !int.TryParse(readOnlySpan.Slice(num, i - num).ToString(), out result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			bool flag2 = false;
			int result2 = 0;
			if (i < readOnlySpan.Length && readOnlySpan[i] == '.')
			{
				i++;
				int num3 = i;
				for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
				{
				}
				if (i > num3)
				{
					flag2 = true;
					if (!int.TryParse(readOnlySpan.Slice(num3, i - num3).ToString(), out result2))
					{
						return (ByteIndex: -1, BitIndex: 0);
					}
				}
			}
			if (BitAccessRegions.Contains(text) & flag2)
			{
				return (ByteIndex: result * 16 + result2, BitIndex: 0);
			}
			return (ByteIndex: result * 2, BitIndex: 0);
		}

		/// <summary>
		/// Bool 降级规则：
		/// - 无点号 Bool（CIO100）→ 驱动按绝对位号 100×16 读，与字区布局（_WORD 字节 200）错位 → 降级
		/// - DR/IR 位读走 SnetHelper 字读取位（16 位/字、reverseByWord），与 0101 位读的 1 位 1 字节不兼容 → 降级
		/// - 其余带点号位访问（CIO/WR/HR/AR/TIM/CNT 等）→ 组包（绝对位号 1 位 1 字节）
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)dataType != 2)
			{
				return true;
			}
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			ReadOnlySpan<char> span = address.AsSpan();
			int i;
			for (i = 0; i < span.Length && char.IsLetter(span[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = span.Slice(0, i).ToString();
			if (!BitAccessRegions.Contains(text) || span.IndexOf('.') <= i)
			{
				return false;
			}
			if (!text.Equals("DR", StringComparison.OrdinalIgnoreCase))
			{
				return !text.Equals("IR", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private static bool TryParseHex(ReadOnlySpan<char> span, out int value)
		{
			value = 0;
			if (span.IsEmpty)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				int num;
				if (c >= '0' && c <= '9')
				{
					num = c - 48;
				}
				else if (c >= 'A' && c <= 'F')
				{
					num = c - 65 + 10;
				}
				else
				{
					if (c < 'a' || c > 'f')
					{
						return false;
					}
					num = c - 97 + 10;
				}
				value = value * 16 + num;
			}
			return true;
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			ReadOnlySpan<char> span = address.AsSpan();
			int i;
			for (i = 0; i < span.Length && char.IsLetter(span[i]); i++)
			{
			}
			if (i == 0)
			{
				return address;
			}
			string text = span.Slice(0, i).ToString().ToUpperInvariant();
			if (!RegionMap.TryGetValue(text, out string value))
			{
				value = text;
			}
			if (value == "EM")
			{
				int num = i;
				for (; i < span.Length && char.IsDigit(span[i]); i++)
				{
				}
				if (i > num && TryParseHex(span.Slice(num, i - num), out var value2))
				{
					return $"EM{value2}";
				}
				return "EM";
			}
			if (span.IndexOf('.') > 0 && (!(value == "EM") || i >= span.Length) && BitAccessRegions.Contains(text))
			{
				return value + "_BIT";
			}
			return value + "_WORD";
		}
	}
	/// <summary>
	/// 富士 SPH / SPB / CommandSettingType 协议组包器。
	/// 驱动语义：
	/// SPH（FujiSPHAddress）："M1.100.5" → M 区类型1 地址100 字内位5（类型只支持 1/3/10）；
	///   "I100.5"/"Q100.5" → I/Q 区地址100 字内位5。位号 0-15 为字内位 → 字节 = 地址×2 + 位/8，位 = 位%8
	/// SPB（FujiSPBAddress）："{字母}{地址}[.{位}]"（X/Y/M/L/TN/TC/CN/CC/D/R/W），地址号直接映射，
	///   位号 = 字号×16 + 位号（GetBitIndex），字设备 ×2 字节
	/// CommandSettingType（FujiCommandSettingTypeAddress）："{前缀}{地址}"（TS/TR/CS/CR/BD/WL/B/M/K/F/A/D/S）
	///   或 "W{类型号}.{地址}"（W9.100），字设备 ×2 字节，无位寻址
	/// 区域键：SPH M → "M{类型}"、I/Q → "I"/"Q"；SPB → 区字母；CST → 前缀。
	/// 位/字分区域键（|B / |W）防止混批。
	/// </summary>
	public sealed class FujiPacker : PackerBase
	{
		private readonly record struct FujiAddr(string Region, int ByteIndex, int BitIndex);

		/// <summary>SPB 区（字母 + 十进制地址 + .N 位号）</summary>
		private static readonly HashSet<string> SpbTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"X", "Y", "M", "L", "TN", "TC", "CN", "CC", "D", "R",
			"W"
		};

		/// <summary>CommandSettingType 区（纯字设备，无位寻址）</summary>
		private static readonly HashSet<string> CstTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"TS", "TR", "CS", "CR", "BD", "WL", "B", "M", "K", "F",
			"A", "D", "S"
		};

		public override ProtocolFamily Family => ProtocolFamily.Fuji;

		/// <summary>驱动单次上限：SPH 230 字（460 字节）、SPB 105 字（210 字节）→ 取最小 210 保证两种协议都不超</summary>
		public override int MaxReadLength => 210;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, isBit: false, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, isBit: false, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// Bool 仅 SPH/SPB 位语义区可组包；CommandSettingType（TS/TR/CS/CR/BD/WL/B/K/F/A/S/W 区）
		/// 驱动无 ReadBool → Bool 降级原样保留。
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)dataType != 2)
			{
				return true;
			}
			if (!TryParse(address, isBit: true, out var result))
			{
				return false;
			}
			return !result.Region.StartsWith("W", StringComparison.Ordinal);
		}

		/// <summary>
		/// 覆写分组：按 DataType 决定位/字语义并分离区域键。
		/// Bool → |B 位语义；其他 → |W 字语义。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, flag, out var result))
				{
					list.Add(address);
					continue;
				}
				int bitIndex = (flag ? result.BitIndex : 0);
				list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, bitIndex, address.DataFormat));
			}
			return base.GroupAndSort(list);
		}

		/// <summary>
		/// 解析富士地址。自动判别 SPH / SPB / CommandSettingType 三种格式。
		/// M 区歧义规则：M 后点分第一段 ≤ 16 且有后续段 → SPH（M1.100.3）；
		/// 否则按 SPB（M100.3 = 地址100 位3）。
		/// </summary>
		private static bool TryParse(string address, bool isBit, out FujiAddr result)
		{
			result = default(FujiAddr);
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			string text2 = readOnlySpan.Slice(i).ToString();
			if (text == "M" && text2.Contains('.') && TryParseSphM(text2, out (string, int, int) parsed))
			{
				result = (isBit ? new FujiAddr("M" + parsed.Item1, parsed.Item2 * 2 + parsed.Item3 / 8, parsed.Item3 % 8) : new FujiAddr("M" + parsed.Item1, parsed.Item2 * 2, 0));
				return true;
			}
			if ((text == "I" || text == "Q") && TryParseSphIQ(text2, out (int, int) parsed2))
			{
				result = (isBit ? new FujiAddr(text, parsed2.Item1 * 2 + parsed2.Item2 / 8, parsed2.Item2 % 8) : new FujiAddr(text, parsed2.Item1 * 2, 0));
				return true;
			}
			if (text == "W")
			{
				int num = text2.IndexOf('.');
				if (num > 0 && int.TryParse(text2.Substring(0, num), out var result2) && int.TryParse(text2.Substring(num + 1), out var result3))
				{
					result = new FujiAddr($"W{result2}", result3 * 2, 0);
					return true;
				}
				if (num == 0)
				{
					return false;
				}
			}
			if (!isBit && CstTypes.Contains(text) && int.TryParse(text2, out var result4))
			{
				result = new FujiAddr(text, result4 * 2, 0);
				return true;
			}
			if (SpbTypes.Contains(text))
			{
				int num2 = text2.IndexOf('.');
				string s = ((num2 >= 0) ? text2.Substring(0, num2) : text2);
				string text3 = ((num2 >= 0) ? text2.Substring(num2 + 1) : "");
				if (!int.TryParse(s, out var result5))
				{
					return false;
				}
				int num3 = ((!string.IsNullOrEmpty(text3) && int.TryParse(text3, out var result6)) ? result6 : 0);
				if (isBit)
				{
					bool flag = num2 < 0;
					if (flag)
					{
						bool flag2;
						switch (text)
						{
						case "X":
						case "Y":
						case "M":
						case "L":
						case "TC":
						case "CC":
							flag2 = true;
							break;
						default:
							flag2 = false;
							break;
						}
						flag = flag2;
					}
					int num4 = (flag ? result5 : (result5 * 16 + num3));
					result = new FujiAddr(text, num4 / 8, num4 % 8);
				}
				else
				{
					result = new FujiAddr(text, result5 * 2, 0);
				}
				return true;
			}
			return false;
		}

		private static bool TryParseSphM(string rest, out (string Type, int Addr, int Bit) parsed)
		{
			parsed = default((string, int, int));
			string[] array = rest.Split('.');
			int result = default(int);
			bool flag = array.Length < 2 || !int.TryParse(array[0], out result);
			if (!flag)
			{
				bool flag2 = ((result == 1 || result == 3 || result == 10) ? true : false);
				flag = !flag2;
			}
			if (flag)
			{
				return false;
			}
			if (!int.TryParse(array[1], out var result2))
			{
				return false;
			}
			int item = ((array.Length > 2 && int.TryParse(array[2], out var result3)) ? result3 : 0);
			parsed = (Type: result.ToString(), Addr: result2, Bit: item);
			return true;
		}

		private static bool TryParseSphIQ(string rest, out (int Addr, int Bit) parsed)
		{
			parsed = default((int, int));
			string[] array = rest.Split('.');
			if (!int.TryParse(array[0], out var result))
			{
				return false;
			}
			int item = ((array.Length > 1 && int.TryParse(array[1], out var result2)) ? result2 : 0);
			parsed = (Addr: result, Bit: item);
			return true;
		}
	}
	/// <summary>
	/// 三菱 FX 系列 FxLinks 协议组包器。
	/// 驱动语义（MelsecFxLinksAddress / MelsecFxLinksHelper）：
	/// 位设备（X/Y/M/S/TS/TN/CS/CN）BR 命令逐位读取，每位 1 字节（ASCII '1'/'0'）→ 位号直接作字节偏移，BitIndex=0。
	/// 字设备（D/R）WR/QR 命令按字读取，ByteIndex = 编号 × 2。
	/// X/Y 为八进制编址（X10 = 8），其余十进制。
	/// 支持 "s=N;" 站号前缀，站号进区域键（S{站}|{区}），跨站不混批。
	/// </summary>
	public sealed class FxLinksPacker : PackerBase
	{
		private readonly record struct FxAddr(string Region, int ByteIndex, int BitIndex);

		/// <summary>位设备类型（X/Y 八进制，其余十进制）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y", "M", "S", "TS", "TN", "CS", "CN" };

		/// <summary>八进制编址类型（FX 的 X/Y）</summary>
		private static readonly HashSet<string> OctalTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y" };

		public override ProtocolFamily Family => ProtocolFamily.MitsubishiFx;

		/// <summary>驱动单次上限：字路径 64 字 = 128 字节，位路径 256 位 = 256 字节 → 取最小 128 保证字路径不超</summary>
		public override int MaxReadLength => 128;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。驱动自动分帧（64字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 字区（D/R）Bool → 降级：驱动 ReadBool 发 BR 命令，BR 仅位设备有效，
		/// 对 D 区发 BR → PLC 报错（点号形式 D100.0 走 SnetHelper 但 packer 不支持点号解析）。
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)dataType != 2)
			{
				return true;
			}
			ReadOnlySpan<char> span = address.AsSpan();
			if (span.StartsWith("s=", StringComparison.OrdinalIgnoreCase))
			{
				int num = span.IndexOf(';');
				if (num < 0)
				{
					return true;
				}
				span = span.Slice(num + 1);
			}
			int i;
			for (i = 0; i < span.Length && char.IsLetter(span[i]); i++)
			{
			}
			if (i == 0)
			{
				return true;
			}
			return BitTypes.Contains(span.Slice(0, i).ToString());
		}

		/// <summary>
		/// 覆写分组：按 DataType 分离位/字区域（|B / |W）。
		/// 位区（X/Y/M/S/TS/TN/CS/CN）驱动用 BR 位读（位号直接作字节偏移）或 WR 字读（字号 ×2 字节），
		/// 同一地址（如 M100）位读/字读是不同命令 → 必须分离，否则同地址 Bool+Word 会被去重丢一个。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
					continue;
				}
				int byteIndex = ((BitTypes.Contains(result.Region) && !flag) ? (result.ByteIndex * 2) : result.ByteIndex);
				list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), byteIndex, 0, address.DataFormat));
			}
			return base.GroupAndSort(list);
		}

		private static bool TryParse(string address, out FxAddr result)
		{
			result = default(FxAddr);
			string text = "";
			ReadOnlySpan<char> span = address.AsSpan();
			if (span.StartsWith("s=", StringComparison.OrdinalIgnoreCase))
			{
				int num = span.IndexOf(';');
				if (num < 0)
				{
					return false;
				}
				text = span.Slice(2, num - 2).Trim().ToString();
				if (text.Length == 0)
				{
					return false;
				}
				string text2 = text;
				foreach (char c in text2)
				{
					if ((c < '0' || c > '9') ? true : false)
					{
						return false;
					}
				}
				span = span.Slice(num + 1);
			}
			int j;
			for (j = 0; j < span.Length && char.IsLetter(span[j]); j++)
			{
			}
			if (j == 0)
			{
				return false;
			}
			string text3 = span.Slice(0, j).ToString().ToUpperInvariant();
			int num2 = j;
			for (; j < span.Length && char.IsDigit(span[j]); j++)
			{
			}
			if (j <= num2)
			{
				return false;
			}
			bool octal = OctalTypes.Contains(text3);
			if (!TryParseNumber(span.Slice(num2, j - num2), octal, out var value))
			{
				return false;
			}
			string region = ((text.Length > 0) ? ("S" + text + "|" + text3) : text3);
			if (BitTypes.Contains(text3))
			{
				result = new FxAddr(region, value, 0);
			}
			else
			{
				result = new FxAddr(region, value * 2, 0);
			}
			return true;
		}

		/// <summary>解析数字，支持八进制（X/Y）/十进制（无堆分配）</summary>
		private static bool TryParseNumber(ReadOnlySpan<char> span, bool octal, out int value)
		{
			value = 0;
			if (span.IsEmpty)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				if (octal)
				{
					if ((c < '0' || c > '7') ? true : false)
					{
						return false;
					}
				}
				else if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
				value = value * (octal ? 8 : 10) + (c - 48);
			}
			return true;
		}
	}
	/// <summary>
	/// GE SRTP 协议组包器。
	/// 驱动语义（GeSRTPAddress）：
	/// R/AI/AQ 为字寄存器（不支持位读 GeSRTPNotSupportBitReadWrite）；
	/// I/Q/M/T/S/SA/SB/SC/G 支持位读（DataCode 位区，地址号直接，每位 1 字节）。
	/// AddressStart = 数字 - 1（SRTP 地址从 1 起始，驱动统一减一）；不允许 0 地址。
	/// 位/字用不同 DataCode（ReadBool isBit:true / Read isBit:false）→ Bool/Word 必须分区域。
	/// </summary>
	public sealed class GePacker : PackerBase
	{
		private readonly record struct GeRegion(string Region, int ByteIndex, int BitIndex);

		/// <summary>仅字区（驱动不支持位读：GeSRTPNotSupportBitReadWrite）</summary>
		private static readonly HashSet<string> WordOnlyRegions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "R", "AI", "AQ" };

		private static readonly Dictionary<string, string> RegionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["R"] = "R",
			["AI"] = "AI",
			["AQ"] = "AQ",
			["I"] = "I",
			["Q"] = "Q",
			["M"] = "M",
			["T"] = "T",
			["SA"] = "SA",
			["SB"] = "SB",
			["SC"] = "SC",
			["S"] = "S",
			["G"] = "G"
		};

		public override ProtocolFamily Family => ProtocolFamily.GE;

		/// <summary>驱动字读 length 单位为字（GeHelper.BuildReadCommand 直接放 Length），且单命令无分片；
		/// 组包字节批被驱动当字数读 → 保守上限 128 字（128 字节批 → 驱动读 128 字），避免超 PLC 上限</summary>
		public override int MaxReadLength => 128;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 类型-区域匹配过滤：
		/// - 字区（R/AI/AQ）Bool → 降级（驱动不支持位读）
		/// - 位区（I/Q/M/T/S...）Word → 降级（驱动字读位区 AddressStart 是位号，1位1字节模型
		///   与字读 16位/字 错位，批内非批首地址读错）
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if (!TryParse(address, out var result))
			{
				return true;
			}
			return (int)dataType == 2 != WordOnlyRegions.Contains(result.Region);
		}

		/// <summary>
		/// 覆写分组：Bool → |B 位读（仅支持位读区），其他 → |W 字读。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, 0, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		private static bool TryParse(string address, out GeRegion result)
		{
			result = default(GeRegion);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			for (int j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
			}
			int num2 = int.Parse(readOnlySpan.Slice(num, i - num));
			if (num2 == 0)
			{
				return false;
			}
			string region = (RegionMap.TryGetValue(text, out string value) ? value : text);
			bool flag = WordOnlyRegions.Contains(text);
			result = new GeRegion(region, flag ? ((num2 - 1) * 2) : (num2 - 1), 0);
			return true;
		}
	}
	/// <summary>
	/// 协议组包器公共接口
	/// 定义所有协议组包器必须实现的核心方法，包括地址解析、批量优化和字节限制查询
	/// </summary>
	public interface IPackerCore
	{
		/// <summary>
		/// 当前 Packer 对应的协议族
		/// </summary>
		ProtocolFamily Family { get; }

		/// <summary>
		/// 保守安全默认值。
		/// 当调用方传入 maxByteLength ≤ 0 时自动使用此值，确保所有型号 PLC 均能工作。
		/// 例如 S7-300 默认为 240 字节 PDU。
		/// </summary>
		int MaxReadLength { get; }

		/// <summary>
		/// 协议物理硬上限（字节）。用户显式传入的 maxByteLength 不能超过此值。
		/// 默认等于 MaxReadLength。对于 PDU 可协商的协议（如 S7），子类覆写为更高值。
		/// </summary>
		int PhysicalMaxBytes => MaxReadLength;

		/// <summary>
		/// 是否支持自动组包优化
		/// </summary>
		bool SupportsAutoPack { get; }

		/// <summary>
		/// 优化地址集合为批量读取结构。
		/// 按区域分组、按偏移排序、按最大字节限制拆分批次。
		/// </summary>
		/// <param name="address">源地址集合，包含待优化的地址列表</param>
		/// <param name="maxByteLength">单次批量读取的最大字节数</param>
		/// <param name="format">数据字节序格式</param>
		/// <returns>优化后的 Address（批量读取就绪），失败返回 null</returns>
		Address? Pack(Address address, int maxByteLength, DataFormat format);

		/// <summary>
		/// 解析协议特定地址字符串为 (字节偏移, 位索引)。
		/// 解析失败返回 (-1, 0)。
		/// 示例：S7 "DB1.DBX0.3" → (0, 3)；Modbus "40001" → (0, 0)
		/// </summary>
		(int ByteIndex, int BitIndex) ParseAddress(string address);

		/// <summary>
		/// 安全解析地址。永远不会抛出异常。
		/// </summary>
		/// <returns>解析成功返回 true，失败返回 false</returns>
		bool TryParseAddress(string address, out int byteIndex, out int bitIndex)
		{
			(byteIndex, bitIndex) = ParseAddress(address);
			return byteIndex >= 0;
		}

		/// <summary>
		/// 获取地址区域前缀（用于分组）。
		/// 示例：S7 "DB1.DBX0.3" → "DB1"；Modbus "40001" → "4"
		/// </summary>
		/// <param name="address">协议特定格式的地址字符串</param>
		/// <returns>区域标识字符串，解析失败返回空字符串</returns>
		string GetRegionKey(string address);
	}
	/// <summary>
	/// 基恩士 Keyence MC Binary 协议组包器（KeyenceMcNet / KeyenceMcAsciiNet）。
	/// 驱动语义（McAddressData.ParseKeyenceFrom / MelsecMcDataType）：
	/// 位区（isWord=1）：B/M/L/SM/X/Y/TS/TC/CS/CC，X/Y/B 十六进制、M/L/SM/TS/TC/CS/CC 十进制，
	///   位地址按 16 位/字编排 → (n/16)*2 + (n%16)/8, n%8。
	/// 字区（isWord=0）：SD/D/R/ZR/W/TN/CN，×2 字节。
	/// 位/字用不同 DataCode → Bool/Word 必须分区域。
	/// KV 老协议（KeyenceKvOld）与 Nano（KeyenceNanoSerial）是不同地址模型，不走本 packer。
	/// </summary>
	public sealed class KeyencePacker : PackerBase
	{
		private readonly record struct KvRegion(string Region, int ByteIndex, int BitIndex);

		/// <summary>位区（MelsecMcDataType isWord=1；X/Y/B 十六进制，其余十进制）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "B", "M", "L", "SM", "X", "Y", "TS", "TC", "CS", "CC" };

		/// <summary>十六进制编址位区（X/Y/B）</summary>
		private static readonly HashSet<string> HexBitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y", "B" };

		private static readonly Dictionary<string, string> RegionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["DM"] = "DM",
			["EM"] = "EM",
			["FM"] = "FM",
			["R"] = "R",
			["MR"] = "MR",
			["D"] = "D",
			["W"] = "W",
			["M"] = "M",
			["X"] = "X",
			["Y"] = "Y",
			["B"] = "B",
			["L"] = "L",
			["SM"] = "SM",
			["SD"] = "SD",
			["ZR"] = "ZR",
			["ZF"] = "ZF",
			["TN"] = "TN",
			["TS"] = "TS",
			["TC"] = "TC",
			["CN"] = "CN",
			["CS"] = "CS",
			["CC"] = "CC",
			["LR"] = "LR",
			["CR"] = "CR",
			["CM"] = "CM"
		};

		public override ProtocolFamily Family => ProtocolFamily.Keyence;

		public override int MaxReadLength => 960;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。McHelper 自动分帧（可读任意长度）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 类型-区域匹配过滤（MC 语义，与 Melsec 相同）：
		/// - 位区 Word → 降级：驱动字读位区 AddressStart 直接用位号（M100 字读 = 字 100），
		///   与 packer 的 (n/16)*2 字节位置错位
		/// - 字区 Bool → 降级：驱动位读命令对字区码不合法
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if (!TryParse(address, out var result))
			{
				return true;
			}
			return (int)dataType == 2 == BitTypes.Contains(result.Region);
		}

		/// <summary>
		/// 覆写分组：Bool → |B 位读（仅位区），其他 → |W 字读（仅字区）。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, result.BitIndex, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		private static bool TryParse(string address, out KvRegion result)
		{
			result = default(KvRegion);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			bool flag = HexBitTypes.Contains(text);
			int num2 = 0;
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			for (int j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				int num3;
				if (c >= '0' && c <= '9')
				{
					num3 = c - 48;
				}
				else if (flag && c >= 'A' && c <= 'F')
				{
					num3 = c - 65 + 10;
				}
				else
				{
					if (!flag || c < 'a' || c > 'f')
					{
						return false;
					}
					num3 = c - 97 + 10;
				}
				num2 = num2 * (flag ? 16 : 10) + num3;
			}
			string region = (RegionMap.TryGetValue(text, out string value) ? value : text);
			if (BitTypes.Contains(text))
			{
				result = new KvRegion(region, num2 / 16 * 2 + num2 % 16 / 8, num2 % 8);
			}
			else
			{
				result = new KvRegion(region, num2 * 2, 0);
			}
			return true;
		}
	}
	/// <summary>
	/// LSis Cnet 协议组包器。
	/// 驱动语义（LsisCnetAddress / LSCnetHelper）：
	/// 地址格式：{类型}{位宽}{数字} 或带位号 "{类型}{位宽}{数字}.{位}"（MB100.0, MW100.0）。
	/// 位宽字母决定缩放：B 位直接（1）、W/X ×2、D ×4、L ×8；无位宽默认 ×2（字）。
	/// 驱动对 Bool 地址同样做缩放（AddressStart *= 2），且批量位读走 SnetHelper.ReadBool(addressLength=8) → 每字节 8 位。
	/// 位号：地址号×2 为字节偏移，.N 为字节内位（0-7）。
	/// U 区：U{hex}.{word}（字寻址 ×2）；I/Q 区：I{a}.{b}.{c}（位寻址 ×2）。
	/// 区域键 = 类型 + 位宽（如 D、MB、DW），区分位/字。
	/// </summary>
	public sealed class LSisPacker : PackerBase
	{
		private readonly record struct LsisRegion(string Region, int ByteIndex, int BitIndex);

		private static readonly Dictionary<string, string> RegionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["D"] = "D",
			["M"] = "M",
			["P"] = "P",
			["K"] = "K",
			["L"] = "L",
			["T"] = "T",
			["C"] = "C",
			["F"] = "F",
			["S"] = "S",
			["Q"] = "Q",
			["I"] = "I",
			["N"] = "N",
			["U"] = "U",
			["Z"] = "Z",
			["R"] = "R"
		};

		public override ProtocolFamily Family => ProtocolFamily.LSis;

		public override int MaxReadLength => 500;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。驱动自动分帧（254字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 覆写分组：按 DataType 分离位/字区域（|B / |W）。
		/// LSis 驱动位读（ReadBool）与字读（ReadByte）是不同命令（不同 DataCode），
		/// 同地址（如 M100 Bool + M100 Word）必须分离，否则去重丢一个。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, flag ? result.BitIndex : 0, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		/// <summary>
		/// 解析 LSis 地址。驱动字节偏移 = 地址号 × 位宽缩放（B=1/W=2/D=4/L=8，默认 2）。
		/// Bool 位号来自 .N 后缀（0-7，每字节 8 位编排）；无后缀 Bool 位号 0。
		/// </summary>
		private static bool TryParse(string address, out LsisRegion result)
		{
			result = default(LsisRegion);
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			char c = text[0];
			if (!RegionMap.ContainsKey(c.ToString()))
			{
				return false;
			}
			string text2 = readOnlySpan.Slice(i).ToString();
			if (c == 'U' && text2.Contains('.'))
			{
				string[] array = text2.Split('.');
				if (array.Length >= 2 && TryParseHex(array[0], out var value))
				{
					bool flag = array.Length >= 3;
					if (!int.TryParse(array[1], out var result2))
					{
						return false;
					}
					int num = ((flag && int.TryParse(array[2], out var result3)) ? result3 : 0);
					int byteIndex = (value * 32 + result2) * 2 + (flag ? (num / 8) : 0);
					int bitIndex = (flag ? (num % 8) : 0);
					result = new LsisRegion("U", byteIndex, bitIndex);
					return true;
				}
				return false;
			}
			if ((c == 'I' || c == 'Q') && text2.Contains('.'))
			{
				string[] array2 = text2.Split('.');
				if (array2.Length >= 3)
				{
					if (!int.TryParse(array2[0], out var result4) || !int.TryParse(array2[1], out var result5) || !int.TryParse(array2[2], out var result6))
					{
						return false;
					}
					int byteIndex2 = (result4 * 64 + result5 * 4 + result6) * 2 + ((result6 % 8 != 0) ? (result6 / 8) : 0);
					int bitIndex2 = result6 % 8;
					result = new LsisRegion(c.ToString(), byteIndex2, bitIndex2);
					return true;
				}
				return false;
			}
			string text3 = "";
			if (text.Length > 1 && "BWDLX".Contains(text[1], StringComparison.OrdinalIgnoreCase))
			{
				text3 = text[1].ToString();
			}
			int num2 = text2.IndexOf('.');
			string s = ((num2 >= 0) ? text2.Substring(0, num2) : text2);
			string text4 = ((num2 >= 0) ? text2.Substring(num2 + 1) : "");
			if (!int.TryParse(s, out var result7))
			{
				return false;
			}
			int num3 = ((!string.IsNullOrEmpty(text4) && int.TryParse(text4, out var result8)) ? result8 : 0);
			int num4;
			switch (text3.ToUpperInvariant())
			{
			case "B":
				num4 = 1;
				break;
			case "W":
			case "X":
				num4 = 2;
				break;
			case "D":
				num4 = 4;
				break;
			case "L":
				num4 = 8;
				break;
			default:
				num4 = 2;
				break;
			}
			int num5 = num4;
			int byteIndex3 = result7 * num5;
			int bitIndex3 = num3 % 8;
			result = new LsisRegion(RegionMap[c.ToString()] + text3, byteIndex3, bitIndex3);
			return true;
		}

		private static bool TryParseHex(ReadOnlySpan<char> span, out int value)
		{
			value = 0;
			if (span.IsEmpty)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				int num;
				if (c >= '0' && c <= '9')
				{
					num = c - 48;
				}
				else if (c >= 'A' && c <= 'F')
				{
					num = c - 65 + 10;
				}
				else
				{
					if (c < 'a' || c > 'f')
					{
						return false;
					}
					num = c - 97 + 10;
				}
				value = value * 16 + num;
			}
			return true;
		}
	}
	/// <summary>
	/// 三菱 Melsec MC Binary 协议组包器。
	/// 地址格式：D100, M100, X0, Y10, W100, B0, L100, F100, R100, ZR100 等。
	/// 字设备（D/W/R/ZR 等）：ByteIndex = 编号 × 2（16位=2字节/字）。
	/// 位设备（M/X/Y/B 等）：按 16 位/字编排，ByteIndex = (n/16)*2 + (n%16)/8，BitIndex = n%8。
	/// X/Y/B/SB/DX/DY/W/SW 为十六进制编址，其余十进制。
	/// </summary>
	public sealed class MelsecPacker : PackerBase
	{
		/// <summary>位区（驱动 MelsecMcDataType isWord=1：X/Y/M/SM/S/L/F/V/B/SB/DX/DY/TS/TC/SS/SC/CS/CC）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"M", "X", "Y", "B", "L", "F", "V", "S", "SM", "TS",
			"TC", "SS", "SC", "CS", "CC", "SB", "DX", "DY"
		};

		/// <summary>十六进制编址类型（三菱标准）：X/Y/B/SB/DX/DY 位、W/SW 字</summary>
		private static readonly HashSet<string> HexTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y", "B", "SB", "DX", "DY", "W", "SW" };

		public override ProtocolFamily Family => ProtocolFamily.Mitsubishi;

		public override int MaxReadLength => 960;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。McHelper 自动分帧，可读任意长度。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		/// <summary>
		/// 类型-区域匹配过滤：
		/// - 位区 Bool（M/X/Y/B 等）→ 组包（驱动位读命令合法）
		/// - 字区 Word（D/W/R/ZR 等）→ 组包（驱动字读命令合法）
		/// - 位区 Word → 降级：驱动字读位区 AddressStart 直接用位号（M100 字读 = M 区字 100），
		///   与 packer 的 (n/16)*2 字节位置错位，无法组包
		/// - 字区 Bool → 降级：驱动位读命令（0x0101）对 D 等字区码不合法（PLC 报错）
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if (!TryGetTypeCode(address, out string code))
			{
				return true;
			}
			return (int)dataType == 2 == BitTypes.Contains(code);
		}

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			string item = readOnlySpan.Slice(0, i).ToString();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			bool hex = HexTypes.Contains(item);
			if (!TryParseNumber(readOnlySpan.Slice(num, i - num), hex, out var value))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (BitTypes.Contains(item))
			{
				return (ByteIndex: value / 16 * 2 + value % 16 / 8, BitIndex: value % 8);
			}
			return (ByteIndex: value * 2, BitIndex: 0);
		}

		/// <summary>解析数字，支持十进制/十六进制（无堆分配）</summary>
		private static bool TryParseNumber(ReadOnlySpan<char> span, bool hex, out int value)
		{
			value = 0;
			if (span.IsEmpty)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				int num;
				if (c >= '0' && c <= '9')
				{
					num = c - 48;
				}
				else if (hex && c >= 'A' && c <= 'F')
				{
					num = c - 65 + 10;
				}
				else
				{
					if (!hex || c < 'a' || c > 'f')
					{
						return false;
					}
					num = c - 97 + 10;
				}
				value = value * (hex ? 16 : 10) + num;
			}
			return true;
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i <= 0)
			{
				return address;
			}
			return readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
		}

		private static bool TryGetTypeCode(string address, out string code)
		{
			code = string.Empty;
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			code = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			return true;
		}

		/// <summary>
		/// 覆写分组：位/字加 |B / |W 后缀。
		/// MC 位区（M）与字区（D）区域键本身不同，但同区同地址 Bool+Word 需分离（M100 Bool = 位100，
		/// M100 Word 驱动字读 = M 区字 100 或独立批次），区域键不同 → 都保留不丢点。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				if (!TryGetTypeCode(addr.AddressName, out string code))
				{
					list.Add(address);
					continue;
				}
				bool flag = (int)addr.AddressDataType == 2;
				list.Add(new ParsedAddr(addr, code + (flag ? "|B" : "|W"), address.ByteIndex, address.BitIndex, address.DataFormat));
			}
			return base.GroupAndSort(list);
		}
	}
	/// <summary>
	/// Modbus 协议组包器。
	/// 地址零基，支持富地址修饰符：
	/// "100" 保持寄存器（Word 类型）或线圈（Bool 类型，设备可能表示为 000100）
	/// "100H" 十六进制地址 → 256
	/// "x=2;100" / "x=4;100" 显式功能码（2=输入线圈, 4=输入寄存器）
	/// "100.1" 寄存器内位操作
	/// "s=2;100" 携带站号
	/// "format=DCBA;100" 强制指定数据格式
	/// "w=16;100" / "x=7;w=8;100" 写功能码（读取忽略）
	/// "file=0;100" 文件寄存器（FC0x14/0x15）
	///
	/// 功能码决策：x= 显式值优先；无 x= 时按 DataType（Bool→FC1/FC2，Word→FC3/FC4）。
	/// 区域键 = 站号 × 功能码，跨站/跨功能码不混批；file= 单独区域。
	/// </summary>
	public sealed class ModbusPacker : PackerBase
	{
		/// <summary>解析出的 Modbus 地址信息</summary>
		private sealed class ModbusAddr
		{
			public int Address = -1;

			public int BitIndex;

			public bool HasDot;

			public int Station = -1;

			public int Function = -1;

			public string? DataFormat;

			public int FileNumber = -1;
		}

		private const int FC_READ_COILS = 1;

		private const int FC_READ_DISCRETE_INPUTS = 2;

		private const int FC_READ_HOLDING_REGISTERS = 3;

		private const int FC_READ_INPUT_REGISTERS = 4;

		/// <summary>修饰符前缀（s= 站号 / x= 功能码 / w= 写功能码 / format= 格式 / file= 文件号）</summary>
		private static readonly string[] ModifierKeys = new string[5] { "s=", "x=", "w=", "format=", "file=" };

		public override ProtocolFamily Family => ProtocolFamily.Modbus;

		/// <summary>保守默认值 = 协议单帧上限（125 寄存器 = 250 字节）。maxByteLength 传 0 时用此值。</summary>
		public override int MaxReadLength => 250;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 寄存器 = 131070 字节）。
		/// 驱动 ModbusHelper.Read 自动按 WordReadBatchLength(120) 分帧，单次调用可读任意寄存器数。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		/// <inheritdoc />
		/// <remarks>返回零基索引（寄存器 Word / 线圈 Bool 共用），实际字节/位转换在 SplitIntoBatches 按区域处理。</remarks>
		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			ModbusAddr modbusAddr = ParseModbusAddress(address);
			if (modbusAddr == null || modbusAddr.Address < 0)
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: modbusAddr.Address, BitIndex: modbusAddr.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			ModbusAddr modbusAddr = ParseModbusAddress(address);
			if (modbusAddr != null)
			{
				return BuildRegionKey(modbusAddr);
			}
			return string.Empty;
		}

		/// <summary>
		/// 覆写 GroupAndSort 按功能码决策拆分组。
		/// 无 x= 时：Bool → FC1/FC2 线圈区，Word → FC3/FC4 寄存器区。
		/// 携带 format= 时附加到 ParsedAddr.DataFormat 供 BytesModel 使用。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Invalid comparison between Unknown and I4
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected I4, but got Unknown
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				ModbusAddr modbusAddr = ParseModbusAddress(address.Addr.AddressName);
				if (modbusAddr == null)
				{
					list.Add(address);
					continue;
				}
				if (modbusAddr.Function < 0)
				{
					if (modbusAddr.HasDot)
					{
						modbusAddr.Function = 3;
					}
					else
					{
						modbusAddr.Function = (((int)address.Addr.AddressDataType == 2) ? 1 : 3);
					}
				}
				int dataFormat = -1;
				if (modbusAddr.DataFormat != null && TryParseDataFormat(modbusAddr.DataFormat, out var format))
				{
					dataFormat = (int)format;
				}
				list.Add(new ParsedAddr(address.Addr, BuildRegionKey(modbusAddr), address.ByteIndex, address.BitIndex, dataFormat));
			}
			return base.GroupAndSort(list);
		}

		/// <summary>解析 format= 值（ABCD/BADC/DCBA/CDAB）</summary>
		private static bool TryParseDataFormat(string value, out DataFormat format)
		{
			switch (value)
			{
			case "ABCD":
				format = (DataFormat)0;
				return true;
			case "BADC":
				format = (DataFormat)1;
				return true;
			case "DCBA":
				format = (DataFormat)3;
				return true;
			case "CDAB":
				format = (DataFormat)2;
				return true;
			default:
				format = (DataFormat)0;
				return false;
			}
		}

		/// <summary>
		/// 覆写 SplitIntoBatches：线圈区（FC1/FC2）位→字节；寄存器区（FC3/FC4）索引×2→字节。
		/// </summary>
		protected override List<(int sn, string regionKey, int sumLength, List<BatchItem> items)> SplitIntoBatches(Dictionary<string, List<AddrInfo>> groups, int maxByteLength)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Invalid comparison between Unknown and I4
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Invalid comparison between Unknown and I4
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			foreach (KeyValuePair<string, List<AddrInfo>> group in groups)
			{
				bool flag = IsCoilRegion(group.Key);
				List<AddrInfo> list = new List<AddrInfo>(group.Value.Count);
				foreach (AddrInfo item in group.Value)
				{
					if (flag)
					{
						int byteIndex = item.ByteIndex / 8;
						int bitIndex = item.ByteIndex % 8;
						list.Add(new AddrInfo(item.FullName, item.RegionKey, byteIndex, bitIndex, item.DataType, 1, item.EncodingType, item.DataFormat));
					}
					else
					{
						int byteIndex2 = item.ByteIndex * 2;
						int num = (((int)item.DataType == 2) ? 2 : item.Length);
						int bitIndex2 = (((int)item.DataType == 2) ? ((item.BitIndex < 8) ? (item.BitIndex + 8) : (item.BitIndex - 8)) : 0);
						list.Add(new AddrInfo(item.FullName, item.RegionKey, byteIndex2, bitIndex2, item.DataType, (num <= 0) ? 2 : num, item.EncodingType, item.DataFormat));
					}
				}
				group.Value.Clear();
				group.Value.AddRange(list);
			}
			return base.SplitIntoBatches(groups, maxByteLength);
		}

		private static bool IsCoilRegion(string regionKey)
		{
			if (!regionKey.Contains($"FC{1}", StringComparison.Ordinal))
			{
				return regionKey.Contains($"FC{2}", StringComparison.Ordinal);
			}
			return true;
		}

		/// <summary>
		/// 解析富地址。修饰符任意顺序，最后一个非修饰符段为地址本体。
		/// 地址本体支持 H 十六进制后缀、.N 位后缀。
		/// </summary>
		private static ModbusAddr? ParseModbusAddress(string address)
		{
			ModbusAddr modbusAddr = new ModbusAddr();
			string[] array = address.Split(';', StringSplitOptions.RemoveEmptyEntries);
			bool flag = false;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i].Trim();
				bool flag2 = false;
				string[] modifierKeys = ModifierKeys;
				foreach (string text2 in modifierKeys)
				{
					if (!text.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					string text3 = text.Substring(text2.Length).Trim();
					switch (text2)
					{
					case "s=":
					{
						if (int.TryParse(text3, out var result3))
						{
							modbusAddr.Station = result3;
						}
						break;
					}
					case "x=":
					{
						if (int.TryParse(text3, out var result))
						{
							modbusAddr.Function = result;
						}
						break;
					}
					case "file=":
					{
						if (int.TryParse(text3, out var result2))
						{
							modbusAddr.FileNumber = result2;
						}
						break;
					}
					case "format=":
						if (!string.IsNullOrEmpty(text3))
						{
							modbusAddr.DataFormat = text3.ToUpperInvariant();
						}
						break;
					}
					flag2 = true;
					break;
				}
				if (!flag2 && TryParseAddressBody(text, out var address2, out var bitIndex))
				{
					modbusAddr.Address = address2;
					modbusAddr.BitIndex = bitIndex;
					modbusAddr.HasDot = text.Contains('.');
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			return modbusAddr;
		}

		/// <summary>解析地址本体：支持 "100"、"100.1"、"100H"（十六进制）</summary>
		private static bool TryParseAddressBody(string body, out int address, out int bitIndex)
		{
			address = 0;
			bitIndex = 0;
			if (string.IsNullOrEmpty(body))
			{
				return false;
			}
			int num = body.IndexOf('.');
			string text = ((num >= 0) ? body.Substring(0, num) : body);
			string text2 = ((num >= 0) ? body.Substring(num + 1) : "");
			bool flag = text.EndsWith('H') || text.EndsWith('h');
			string text3;
			if (flag)
			{
				text3 = text;
				text = text3.Substring(0, text3.Length - 1);
			}
			if (text.Length == 0)
			{
				return false;
			}
			text3 = text;
			foreach (char c in text3)
			{
				if (c >= '0' && c <= '9')
				{
					address = address * (flag ? 16 : 10) + (c - 48);
					continue;
				}
				if (flag && c >= 'A' && c <= 'F')
				{
					address = address * 16 + (c - 65 + 10);
					continue;
				}
				if (flag && c >= 'a' && c <= 'f')
				{
					address = address * 16 + (c - 97 + 10);
					continue;
				}
				return false;
			}
			if (!string.IsNullOrEmpty(text2) && !int.TryParse(text2, out bitIndex))
			{
				return false;
			}
			return true;
		}

		/// <summary>构建区域键：S{站号}|{文件}|FC{功能码}。站号默认 -1（不参与分组差异）。</summary>
		private static string BuildRegionKey(ModbusAddr addr)
		{
			if (addr.FileNumber < 0)
			{
				int value = ((addr.Function >= 0) ? addr.Function : 3);
				return $"S{((addr.Station >= 0) ? addr.Station : (-1))}|FC{value}";
			}
			return $"S{((addr.Station >= 0) ? addr.Station : (-1))}|FILE{addr.FileNumber}";
		}
	}
	/// <summary>
	/// 协议组包器抽象基类。
	/// 提供公共的地址分组、排序、批次拆分和结果构建逻辑。
	/// 子类只需实现协议特定的地址解析方法（<see cref="M:Snet.Core.packer.PackerBase.ParseAddress(System.String)" /> 和 <see cref="M:Snet.Core.packer.PackerBase.GetRegionKey(System.String)" />）。
	/// </summary>
	public abstract class PackerBase : IPackerCore
	{
		/// <summary>
		/// 预解析地址（值类型）。Pack 阶段一次性解析，GroupAndSort 直接复用，避免二次解析。
		/// </summary>
		/// <summary>预解析地址。DataFormat 为地址级强制格式（-1=用全局）</summary>
		protected readonly record struct ParsedAddr(AddressDetails Addr, string RegionKey, int ByteIndex, int BitIndex, int DataFormat = -1);

		/// <summary>
		/// 地址信息结构体（值类型，避免堆分配）
		/// </summary>
		protected readonly struct AddrInfo
		{
			/// <summary>原始地址全名</summary>
			public readonly string FullName;

			/// <summary>区域键（分组依据）</summary>
			public readonly string RegionKey;

			/// <summary>字节起始索引</summary>
			public readonly int ByteIndex;

			/// <summary>位索引（Bool 0~7，非Bool 为0）</summary>
			public readonly int BitIndex;

			/// <summary>数据类型</summary>
			public readonly DataType DataType;

			/// <summary>预计算的字节长度</summary>
			public readonly int Length;

			/// <summary>编码类型（仅 String 有效）</summary>
			public readonly EncodingType EncodingType;

			/// <summary>强制数据格式（-1 表示使用全局 format）</summary>
			public readonly int DataFormat;

			public AddrInfo(string fullName, string regionKey, int byteIndex, int bitIndex, DataType dataType, int length, EncodingType encodingType = (EncodingType)65001, int dataFormat = -1)
			{
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				FullName = fullName;
				RegionKey = regionKey;
				ByteIndex = byteIndex;
				BitIndex = bitIndex;
				DataType = dataType;
				Length = length;
				EncodingType = encodingType;
				DataFormat = dataFormat;
			}
		}

		/// <summary>
		/// 批次项结构体（值类型）
		/// </summary>
		protected readonly struct BatchItem
		{
			/// <summary>地址全名</summary>
			public readonly string FullName;

			/// <summary>批次内字节起始偏移</summary>
			public readonly int StartByte;

			/// <summary>读取字节长度</summary>
			public readonly int Length;

			/// <summary>位索引</summary>
			public readonly int BoolIndex;

			/// <summary>编码类型（仅 String 有效）</summary>
			public readonly EncodingType EncodingType;

			/// <summary>强制数据格式（-1 表示使用全局 format）</summary>
			public readonly int DataFormat;

			/// <summary>原始数据类型（BytesHandler 解包需用原类型，不能 ByteArray）</summary>
			public readonly DataType DataType;

			public BatchItem(string fullName, int startByte, int length, int boolIndex = 0, EncodingType encodingType = (EncodingType)65001, int dataFormat = -1, DataType dataType = (DataType)23)
			{
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				FullName = fullName;
				StartByte = startByte;
				Length = length;
				BoolIndex = boolIndex;
				EncodingType = encodingType;
				DataFormat = dataFormat;
				DataType = dataType;
			}
		}

		/// <inheritdoc />
		public abstract ProtocolFamily Family { get; }

		/// <inheritdoc />
		public abstract int MaxReadLength { get; }

		/// <summary>
		/// 协议物理上限（字节）。用户显式传入的 maxByteLength 不能超过此值。
		/// 默认等于 MaxReadLength。对于 PDU 可协商的协议（如 S7），子类覆写为更高值。
		/// </summary>
		public virtual int PhysicalMaxBytes => MaxReadLength;

		/// <inheritdoc />
		public virtual bool SupportsAutoPack => true;

		/// <inheritdoc />
		public abstract (int ByteIndex, int BitIndex) ParseAddress(string address);

		/// <inheritdoc />
		public abstract string GetRegionKey(string address);

		/// <inheritdoc />
		public virtual Address? Pack(Address address, int maxByteLength, DataFormat format)
		{
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			List<AddressDetails> list = ((address != null) ? address.AddressArray : null);
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			int num = ((maxByteLength <= 0) ? MaxReadLength : Math.Min(maxByteLength, PhysicalMaxBytes));
			if (num <= 0)
			{
				throw new ArgumentOutOfRangeException("maxByteLength", "有效批次长度必须大于 0");
			}
			List<ParsedAddr> list2 = new List<ParsedAddr>(address.AddressArray.Count);
			List<AddressDetails> list3 = new List<AddressDetails>();
			for (int i = 0; i < address.AddressArray.Count; i++)
			{
				AddressDetails val = address.AddressArray[i];
				if (string.IsNullOrEmpty(val.AddressName))
				{
					list3.Add(val);
					continue;
				}
				string regionKey = GetRegionKey(val.AddressName);
				var (num2, bitIndex) = ParseAddress(val.AddressName);
				if (!string.IsNullOrEmpty(regionKey) && num2 >= 0 && IsPackableType(val.AddressDataType) && IsPackable(val.AddressName, val.AddressDataType) && CalcByteLength(val) <= num)
				{
					list2.Add(new ParsedAddr(val, regionKey, num2, bitIndex));
				}
				else
				{
					list3.Add(val);
				}
			}
			Dictionary<string, List<AddrInfo>> groups = GroupAndSort(list2);
			List<(int, string, int, List<BatchItem>)> batches = SplitIntoBatches(groups, num);
			return BuildResult(batches, list3, format);
		}

		/// <summary>
		/// 类型与区域的匹配过滤（默认全放行）。
		/// 覆写场景：驱动不支持位读的字区（Bool 降级）、位区字读字节错位的区（Word 降级）。
		/// 返回 false 的地址走未组包路径（原样保留，不丢失）。
		/// </summary>
		protected virtual bool IsPackable(string address, DataType dataType)
		{
			return true;
		}

		/// <summary>
		/// 是否支持地址自动组包的数据类型。
		/// 仅标量类型参与组包；数组/ByteArray 等原样保留（走未组包路径）。
		/// </summary>
		protected static bool IsPackableType(DataType dt)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if (dt - 1 <= 16 || dt - 21 <= 1)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 计算地址的实际字节长度。
		/// - Byte/Bool → 1 字节
		/// - String/Char → Length（字符数）× 编码字节宽
		/// - 数组类型（*Array）→ Length × 单元素宽度（ByteArray=1, Short族=2, Int族=4, Long族=8, BoolArray=位）
		/// - 标量 → ReadGetLength() 固定宽度；零宽度回退 2 字节
		/// </summary>
		protected static int CalcByteLength(AddressDetails addr)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Expected I4, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Invalid comparison between Unknown and I4
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Invalid comparison between Unknown and I4
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			DataType addressDataType = addr.AddressDataType;
			if (addressDataType - 1 <= 1)
			{
				return 1;
			}
			addressDataType = addr.AddressDataType;
			if (addressDataType - 21 <= 1)
			{
				int num = Math.Max((int)addr.Length, 1);
				EncodingType encodingType = addr.EncodingType;
				bool flag = encodingType - 1200 <= 1;
				bool flag2 = flag;
				encodingType = addr.EncodingType;
				flag = encodingType - 12000 <= 1;
				int num2 = (flag ? 4 : ((!flag2) ? 1 : 2));
				return num * num2;
			}
			addressDataType = addr.AddressDataType;
			int num3;
			switch (addressDataType - 23)
			{
			case 0:
				num3 = 1;
				break;
			case 5:
			case 6:
			case 7:
			case 8:
				num3 = 2;
				break;
			case 3:
			case 4:
			case 9:
			case 10:
			case 11:
			case 12:
				num3 = 4;
				break;
			case 2:
			case 13:
			case 14:
			case 15:
			case 16:
				num3 = 8;
				break;
			case 1:
				num3 = 0;
				break;
			default:
				num3 = 0;
				break;
			}
			int num4 = num3;
			if (num4 > 0)
			{
				return Math.Max((int)addr.Length, 1) * num4;
			}
			if ((int)addr.AddressDataType == 24)
			{
				return (Math.Max((int)addr.Length, 1) + 7) / 8;
			}
			int num5 = addr.AddressDataType.ReadGetLength();
			num5 = ((num5 > 0) ? num5 : 2);
			if (num5 > 65535)
			{
				throw new ArgumentOutOfRangeException("addr", $"地址 {addr.AddressName} 计算长度 {num5} 超过 ushort 上限 65535");
			}
			return num5;
		}

		/// <summary>
		/// 按区域分组、去重、排序（使用 Pack 阶段预解析的结果，零重复解析）
		/// </summary>
		protected virtual Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Invalid comparison between Unknown and I4
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, List<AddrInfo>> dictionary = new Dictionary<string, List<AddrInfo>>(Math.Max(addresses.Count / 4, 4), StringComparer.OrdinalIgnoreCase);
			Dictionary<string, HashSet<string>> dictionary2 = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < addresses.Count; i++)
			{
				ParsedAddr parsedAddr = addresses[i];
				AddressDetails addr = parsedAddr.Addr;
				string regionKey = parsedAddr.RegionKey;
				int byteIndex = parsedAddr.ByteIndex;
				int bitIndex = parsedAddr.BitIndex;
				if (!dictionary.TryGetValue(regionKey, out var value))
				{
					value = (dictionary[regionKey] = new List<AddrInfo>(32));
				}
				if (!dictionary2.TryGetValue(regionKey, out var value2))
				{
					value2 = (dictionary2[regionKey] = new HashSet<string>(StringComparer.OrdinalIgnoreCase));
				}
				if (value2.Add(regionKey + "\u0001" + addr.AddressName + "\u0001" + ((object)addr.AddressDataType/*cast due to constrained. prefix*/).ToString()))
				{
					int num = CalcByteLength(addr);
					List<AddrInfo> list2 = value;
					string addressName = addr.AddressName;
					string regionKey2 = regionKey;
					int byteIndex2 = byteIndex;
					int bitIndex2 = bitIndex;
					DataType addressDataType = addr.AddressDataType;
					int length = num;
					DataType addressDataType2 = addr.AddressDataType;
					bool flag = addressDataType2 - 21 <= 1;
					list2.Add(new AddrInfo(addressName, regionKey2, byteIndex2, bitIndex2, addressDataType, length, (EncodingType)((!flag) ? 65001 : ((int)addr.EncodingType)), parsedAddr.DataFormat));
				}
			}
			foreach (KeyValuePair<string, List<AddrInfo>> item in dictionary)
			{
				item.Value.Sort(delegate(AddrInfo a, AddrInfo b)
				{
					int num2 = a.ByteIndex.CompareTo(b.ByteIndex);
					return (num2 == 0) ? a.BitIndex.CompareTo(b.BitIndex) : num2;
				});
			}
			return dictionary;
		}

		/// <summary>
		/// 将排序后的分组地址拆分为不超过 maxByteLength 的批次
		/// </summary>
		protected virtual List<(int sn, string regionKey, int sumLength, List<BatchItem> items)> SplitIntoBatches(Dictionary<string, List<AddrInfo>> groups, int maxByteLength)
		{
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			List<(int, string, int, List<BatchItem>)> list = new List<(int, string, int, List<BatchItem>)>(groups.Count * 2);
			int num = 0;
			foreach (KeyValuePair<string, List<AddrInfo>> item in groups.OrderBy<KeyValuePair<string, List<AddrInfo>>, string>((KeyValuePair<string, List<AddrInfo>> g) => g.Key, StringComparer.Ordinal))
			{
				string key = item.Key;
				List<AddrInfo> value = item.Value;
				List<BatchItem> list2 = new List<BatchItem>(Math.Min(value.Count, 32));
				int num2 = 0;
				int num3 = 0;
				int num4 = -1;
				int num5 = 0;
				bool flag = true;
				for (int num6 = 0; num6 < value.Count; num6++)
				{
					AddrInfo addrInfo = value[num6];
					int num7 = addrInfo.Length;
					if (num7 <= 0)
					{
						num7 = 2;
					}
					int num8 = ((!flag) ? ((addrInfo.ByteIndex != num4) ? (num2 + (addrInfo.ByteIndex - (num4 + num5))) : (num2 - num5)) : 0);
					if (num8 + num7 > maxByteLength && list2.Count > 0)
					{
						list.Add((++num, key, num3, list2));
						list2 = new List<BatchItem>(16);
						num2 = 0;
						num3 = 0;
						num4 = -1;
						num5 = 0;
						flag = true;
						num8 = 0;
					}
					list2.Add(new BatchItem(addrInfo.FullName, num8, num7, addrInfo.BitIndex, addrInfo.EncodingType, addrInfo.DataFormat, addrInfo.DataType));
					num2 = num8 + num7;
					num3 = Math.Max(num3, num2);
					num4 = addrInfo.ByteIndex;
					num5 = num7;
					flag = false;
				}
				if (list2.Count > 0)
				{
					list.Add((++num, key, num3, list2));
				}
			}
			return list;
		}

		/// <summary>
		/// 构建最终 Address 结果对象。
		/// 合并组包批次（已优化）与不可组包的原始地址（原样保留）。
		/// </summary>
		/// <param name="batches">组包批次的列表</param>
		/// <param name="unpackable">无法组包的原始地址（原样追加到结果末尾）</param>
		/// <param name="format">数据字节序格式</param>
		protected virtual Address BuildResult(List<(int sn, string regionKey, int sumLength, List<BatchItem> items)> batches, List<AddressDetails> unpackable, DataFormat format)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Expected O, but got Unknown
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Invalid comparison between Unknown and I4
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Invalid comparison between Unknown and I4
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Expected O, but got Unknown
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Expected O, but got Unknown
			Address val = new Address
			{
				AddressArray = new List<AddressDetails>(batches.Count + unpackable.Count)
			};
			int num = 0;
			for (int i = 0; i < batches.Count; i++)
			{
				(int sn, string regionKey, int sumLength, List<BatchItem> items) tuple = batches[i];
				int item = tuple.sumLength;
				List<BatchItem> item2 = tuple.items;
				num++;
				List<BytesModel> list = new List<BytesModel>(item2.Count);
				for (int j = 0; j < item2.Count; j++)
				{
					BatchItem batchItem = item2[j];
					DataFormat val2 = (DataFormat)((batchItem.DataFormat >= 0) ? batchItem.DataFormat : ((int)format));
					list.Add(new BytesModel(batchItem.FullName, string.Empty, batchItem.StartByte, (ushort)batchItem.Length, batchItem.DataType, batchItem.EncodingType, val2, batchItem.BoolIndex));
				}
				val.AddressArray.Add(new AddressDetails(item2[0].FullName, (DataType)23, (ushort)item, (EncodingType?)(EncodingType)0)
				{
					AddressDescribe = $"packer - {num}",
					AddressExtendParam = list
				});
			}
			for (int k = 0; k < unpackable.Count; k++)
			{
				AddressDetails val3 = unpackable[k];
				int num2 = CalcByteLength(val3);
				int num3 = 0;
				if ((int)val3.AddressDataType == 2 && !string.IsNullOrEmpty(val3.AddressName))
				{
					int item3 = ParseAddress(val3.AddressName).BitIndex;
					if (item3 >= 0 && item3 <= 7)
					{
						num3 = item3;
					}
				}
				num++;
				int num4 = (IsPackableType(val3.AddressDataType) ? ((int)val3.AddressDataType) : 23);
				string addressName = val3.AddressName;
				string text = val3.AddressDescribe ?? string.Empty;
				ushort num5 = (ushort)num2;
				DataType val4 = (DataType)num4;
				DataType addressDataType = val3.AddressDataType;
				bool flag = addressDataType - 21 <= 1;
				BytesModel item4 = new BytesModel(addressName, text, 0, num5, val4, (EncodingType)(flag ? ((int)val3.EncodingType) : 65001), format, num3);
				val.AddressArray.Add(new AddressDetails
				{
					SN = val3.SN,
					AddressName = val3.AddressName,
					AddressDataType = val3.AddressDataType,
					Length = (ushort)num2,
					EncodingType = val3.EncodingType,
					AddressAnotherName = val3.AddressAnotherName,
					AddressPropertyName = val3.AddressPropertyName,
					AddressDescribe = val3.AddressDescribe,
					IsEnable = val3.IsEnable,
					AddressType = val3.AddressType,
					AddressMqParam = val3.AddressMqParam,
					AddressParseParam = val3.AddressParseParam,
					AddressExtendParam = new List<BytesModel> { item4 }
				});
			}
			return val;
		}
	}
	/// <summary>
	/// Packer 工厂类（线程安全单例注册表）。
	/// 根据 <see cref="T:Snet.Core.packer.ProtocolFamily" /> 返回对应的 <see cref="T:Snet.Core.packer.IPackerCore" /> 实例。
	/// </summary>
	public static class PackerFactory
	{
		/// <summary>Packer 注册表：ProtocolFamily → IPacker</summary>
		private static readonly ConcurrentDictionary<ProtocolFamily, IPackerCore> _packers;

		/// <summary>已注册 Packer 总数</summary>
		public static int Count => _packers.Count;

		static PackerFactory()
		{
			_packers = new ConcurrentDictionary<ProtocolFamily, IPackerCore>();
			Register(new SiemensPacker());
			Register(new ModbusPacker());
			Register(new MelsecPacker());
			Register(new FinsPacker());
			Register(new FujiPacker());
			Register(new KeyencePacker());
			Register(new YokogawaPacker());
			Register(new PanasonicPacker());
			Register(new YaskawaPacker());
			Register(new GePacker());
			Register(new FatekPacker());
			Register(new FanucPacker());
			Register(new LSisPacker());
			Register(new CimonPacker());
			Register(new XinJEPacker());
			Register(new VigorPacker());
			Register(new ToyoPacker());
			Register(new AllenBradleyPacker());
			Register(new FxLinksPacker());
			Register(new BeckhoffPacker());
		}

		/// <summary>注册 Packer（线程安全）</summary>
		public static void Register(IPackerCore packer)
		{
			ArgumentNullException.ThrowIfNull(packer, "packer");
			_packers[packer.Family] = packer;
		}

		/// <summary>根据协议族获取对应 Packer</summary>
		public static IPackerCore? GetPacker(ProtocolFamily family)
		{
			if (!_packers.TryGetValue(family, out IPackerCore value))
			{
				return null;
			}
			return value;
		}

		/// <summary>获取所有已注册协议族（快照，线程安全）</summary>
		public static ProtocolFamily[] GetRegisteredFamilies()
		{
			return _packers.Keys.ToArray();
		}

		/// <summary>按设备类型字符串获取 Packer</summary>
		public static IPackerCore? GetPacker(string deviceType)
		{
			if (Enum.TryParse<ProtocolFamily>(deviceType, ignoreCase: true, out var result))
			{
				return GetPacker(result);
			}
			return null;
		}
	}
	/// <summary>
	/// 松下 Mewtocol 协议组包器。
	/// 驱动语义（PanasonicHelper.AnalysisAddress / CalculateComplexAddress）：
	/// X/Y/R/L 为位区，位号 = 复合十六进制（"X100" → 10×16+0=160 位号，"X1F" → 1×16+15=31，"X5" → 5；带点号 "R100.5" → 100×16+5）。
	/// 位读取仅支持 X/Y/R/L（"Bit read only support X,Y,R,L"）；S/K/T/C/D/LD/F/DT 等为字区 ×2。
	/// 字读取 X/Y/R/L 要求绝对位号 %16==0（字对齐）。
	/// </summary>
	public sealed class PanasonicPacker : PackerBase
	{
		private readonly record struct PanasonicAddr(string Region, int ByteIndex, int BitIndex);

		/// <summary>位区（驱动 ReadBool 仅支持 X/Y/R/L）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y", "R", "L" };

		private static readonly Dictionary<string, string> RegionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["DT"] = "DT",
			["R"] = "R",
			["X"] = "X",
			["Y"] = "Y",
			["L"] = "L",
			["LD"] = "LD",
			["TN"] = "TN",
			["TS"] = "TS",
			["TC"] = "TC",
			["CN"] = "CN",
			["CS"] = "CS",
			["CC"] = "CC",
			["S"] = "S",
			["SD"] = "SD",
			["D"] = "DT",
			["K"] = "K",
			["F"] = "F"
		};

		public override ProtocolFamily Family => ProtocolFamily.Panasonic;

		public override int MaxReadLength => 500;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。驱动自动分帧（500字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 类型-区域匹配过滤：
		/// - Bool 仅 X/Y/R/L 区可组包（驱动 ReadBool 显式报 "Bit read only support X,Y,R,L"），
		///   其他区（DT/LD/TN/TS/TC/CN 等）Bool → 降级
		/// - Word 全部区可组包：X/Y/R/L 区字读 = 位号/16 字（packer absBit/8 字节一致），
		///   字区字读 = 字号×2 一致
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)dataType != 2)
			{
				return true;
			}
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return true;
			}
			return BitTypes.Contains(readOnlySpan.Slice(0, i).ToString());
		}

		/// <summary>
		/// 覆写分组：|B / |W 后缀（同地址 Bool+Word 分离不丢点，如 X100 位读与 X100 字读）
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
					continue;
				}
				bool flag = (int)addr.AddressDataType == 2;
				list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, result.BitIndex, address.DataFormat));
			}
			return base.GroupAndSort(list);
		}

		private static bool TryParse(string address, out PanasonicAddr result)
		{
			result = default(PanasonicAddr);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			string text2 = readOnlySpan.Slice(i).ToString();
			if (BitTypes.Contains(text))
			{
				int num = text2.IndexOf('.');
				int value2;
				if (num >= 0)
				{
					if (!int.TryParse(text2.Substring(0, num), out var result2))
					{
						return false;
					}
					if (!TryParseHex(text2.Substring(num + 1), out var value))
					{
						return false;
					}
					value2 = result2 * 16 + value;
				}
				else if (text2.Length == 1)
				{
					if (!TryParseHex(text2, out value2))
					{
						return false;
					}
				}
				else
				{
					string text3 = text2;
					if (!int.TryParse(text3.Substring(0, text3.Length - 1), out var result3))
					{
						return false;
					}
					if (!TryParseHex(text2.Substring(text2.Length - 1), out var value3))
					{
						return false;
					}
					value2 = result3 * 16 + value3;
				}
				result = new PanasonicAddr(text, value2 / 8, value2 % 8);
				return true;
			}
			if (!int.TryParse(text2, out var result4))
			{
				return false;
			}
			string region = (RegionMap.TryGetValue(text, out string value4) ? value4 : text);
			result = new PanasonicAddr(region, result4 * 2, 0);
			return true;
		}

		private static bool TryParseHex(ReadOnlySpan<char> span, out int value)
		{
			value = 0;
			if (span.IsEmpty)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				int num;
				if (c >= '0' && c <= '9')
				{
					num = c - 48;
				}
				else if (c >= 'A' && c <= 'F')
				{
					num = c - 65 + 10;
				}
				else
				{
					if (c < 'a' || c > 'f')
					{
						return false;
					}
					num = c - 97 + 10;
				}
				value = value * 16 + num;
			}
			return true;
		}
	}
	/// <summary>
	/// 统一的协议族枚举（按供应商/协议简称命名）。
	/// 每个值对应一个已完整实现地址组包的 Packer。
	/// </summary>
	public enum ProtocolFamily
	{
		[Description("西门子 S7")]
		Siemens,
		[Description("Modbus")]
		Modbus,
		[Description("三菱 Melsec")]
		Mitsubishi,
		[Description("欧姆龙 FINS")]
		Omron,
		[Description("富士 SPH")]
		Fuji,
		[Description("基恩士 KV")]
		Keyence,
		[Description("横河 Link")]
		Yokogawa,
		[Description("松下 Mewtocol")]
		Panasonic,
		[Description("安川 Memobus")]
		Yaskawa,
		[Description("GE SRTP")]
		GE,
		[Description("永宏 Fatek")]
		Fatek,
		[Description("发那科")]
		Fanuc,
		[Description("LSis Cnet")]
		LSis,
		[Description("Cimon HMI")]
		Cimon,
		[Description("信捷")]
		XinJE,
		[Description("Vigor")]
		Vigor,
		[Description("丰田 ToyoPuc")]
		Toyota,
		[Description("罗克韦尔 SLC")]
		AllenBradley,
		[Description("三菱 FX 链接")]
		MitsubishiFx,
		[Description("倍福 ADS")]
		Beckhoff
	}
	/// <summary>
	/// 西门子 S7 协议组包器。
	/// 支持 S7-200/200Smart/300/400/1200/1500 的 DB/M/I/Q 地址批量优化。
	/// 地址格式：DB1.DBX0.3（位）、DB1.DBD2（双字）、DB1.DBW4（字）、M100.0、I0.0、Q0.0
	/// </summary>
	public sealed class SiemensPacker : PackerBase
	{
		/// <inheritdoc />
		public override ProtocolFamily Family => ProtocolFamily.Siemens;

		/// <summary>
		/// S7 保守安全默认值 = 240 字节。适用于所有型号（S7-200/300/400 的默认 PDU）。
		/// maxByteLength 传 0 或不传时自动使用此值，确保不超 PLC 实际能力。
		/// </summary>
		public override int MaxReadLength => 240;

		/// <summary>
		/// S7 物理硬上限 = 960 字节（S7-1200/1500 固件 V4.0+ 协商最大值）。
		/// 用户显式传 maxByteLength 时以此值封顶。
		/// </summary>
		public override int PhysicalMaxBytes => 960;

		/// <inheritdoc />
		public override bool SupportsAutoPack => true;

		/// <inheritdoc />
		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			ReadOnlySpan<char> span = address.AsSpan();
			if (IsDbAddress(span))
			{
				int num = span.IndexOf('.');
				if (num > 0)
				{
					return ParseS7Tail(span.Slice(num + 1));
				}
			}
			return ParseS7Tail(span);
		}

		/// <inheritdoc />
		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			ReadOnlySpan<char> span = address.AsSpan();
			if (IsDbAddress(span))
			{
				int num = span.IndexOf('.');
				if (num <= 0)
				{
					return string.Empty;
				}
				return span.Slice(0, num).ToString().ToUpperInvariant();
			}
			int i;
			for (i = 0; i < span.Length && char.IsLetter(span[i]); i++)
			{
			}
			if (i <= 0)
			{
				return address;
			}
			return span.Slice(0, i).ToString().ToUpperInvariant();
		}

		/// <summary>判断是否 DB 区域地址</summary>
		private static bool IsDbAddress(ReadOnlySpan<char> span)
		{
			bool flag = span.Length > 2;
			if (flag)
			{
				char c = span[0];
				bool flag2 = ((c == 'D' || c == 'd') ? true : false);
				flag = flag2;
			}
			bool flag3 = flag;
			if (flag3)
			{
				char c = span[1];
				bool flag2 = ((c == 'B' || c == 'b') ? true : false);
				flag3 = flag2;
			}
			return flag3;
		}

		/// <summary>
		/// 解析 S7 地址尾部（零分配）。
		/// 支持：DBX10.3（位）、DBW10（字）、DBD10（双字）、M100.0、I0.7
		/// </summary>
		/// <returns>(字节索引, 位索引)；解析失败返回 (-1, 0)</returns>
		private static (int ByteIndex, int BitIndex) ParseS7Tail(ReadOnlySpan<char> span)
		{
			int i;
			for (i = 0; i < span.Length && char.IsLetter(span[i]); i++)
			{
			}
			int num = i;
			for (; i < span.Length && char.IsDigit(span[i]); i++)
			{
			}
			if (i <= num)
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!int.TryParse(span.Slice(num, i - num), out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			int result2 = 0;
			if (i < span.Length && span[i] == '.')
			{
				i++;
				int num2 = i;
				for (; i < span.Length && char.IsDigit(span[i]); i++)
				{
				}
				if (i > num2)
				{
					int.TryParse(span.Slice(num2, i - num2), out result2);
				}
			}
			return (ByteIndex: result, BitIndex: result2);
		}
	}
	/// <summary>
	/// 丰田工机 ToyoPuc 协议组包器。
	/// 驱动语义（ToyoPucAddress）：
	/// 字访问：绝对字号 = 区字基址 + 十六进制地址号（K100 → 32 + 0x100 = 288 字）
	/// 位访问：位号 = 地址最后 1 个十六进制字符（K1005 = 字 K100 位 5），
	/// 也支持点号格式 "K100.5"；字节 = 字号 × 2 + 位号/8，位 = 位号%8
	/// 区域：K/V/T/C/L/X/Y/M/S/N/R/D/B/H/U + E 系列（EK/EV/ET/EC/EL/EX/EY/EM/ES/EN/EB）+ G 系列（GX/GY/GM）。
	/// EB 按值分 PRG9（&lt;32768）/PRG10（&lt;65536，-32768）/PRG11（≥65536，-65536）；
	/// U=PRG8、GX/GY/GM=PRG7、H=PRG0；支持 prg= 显式修饰符（如 prg=1;K100）。
	/// 位/字分区域键（|B / |W）防止混批。
	/// </summary>
	public sealed class ToyoPacker : PackerBase
	{
		private readonly record struct ToyoAddr(string Region, int ByteIndex, int BitIndex);

		/// <summary>区 → 字基址（isBit=false 时驱动 AddressStart 偏移）</summary>
		private static readonly Dictionary<string, int> WordBase = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			["K"] = 32,
			["V"] = 80,
			["T"] = 96,
			["C"] = 96,
			["L"] = 128,
			["X"] = 256,
			["Y"] = 256,
			["M"] = 384,
			["S"] = 512,
			["N"] = 1536,
			["R"] = 2048,
			["D"] = 4096,
			["B"] = 24576,
			["H"] = 6144,
			["EK"] = 256,
			["EV"] = 512,
			["ET"] = 768,
			["EC"] = 768,
			["EL"] = 896,
			["EX"] = 1408,
			["EY"] = 1408,
			["EM"] = 1536,
			["ES"] = 2048,
			["EN"] = 4096,
			["U"] = 0,
			["GX"] = 0,
			["GY"] = 0,
			["GM"] = 4096,
			["EB"] = 0
		};

		/// <summary>E 系列（PRG 强制为 0）与 U/G/H（PRG 强制 8/7/0），显式 prg= 被驱动覆盖</summary>
		private static readonly HashSet<string> FixedPrgRegions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"EK", "EV", "ET", "EC", "EL", "EX", "EY", "EM", "ES", "EN",
			"EB", "U", "GX", "GY", "GM", "H"
		};

		public override ProtocolFamily Family => ProtocolFamily.Toyota;

		/// <summary>驱动字读 length 单位为字且无分片（BuildReadWordCommand 直接放 length）→ 组包字节批被驱动当字数读，
		/// 保守上限 128 字节（64 字）避免超 PLC 上限</summary>
		public override int MaxReadLength => 128;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, isBit: false, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, isBit: false, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 覆写分组：按 DataType 决定位/字语义。
		/// Bool → 位访问（绝对位号 = 字号×16 + 位号），区域键加 |B；
		/// 其他 → 字访问（字号 × 2 字节），区域键加 |W。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, flag, out var result))
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, result.BitIndex, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		/// <summary>
		/// 解析 Toyo 地址。格式：可选 "prg=N;" 前缀 + 区字母（1-2 个）+ 十六进制地址。
		/// isBit 时：最后 1 个十六进制字符（或点号后）为字内位号。
		/// </summary>
		private static bool TryParse(string address, bool isBit, out ToyoAddr result)
		{
			result = default(ToyoAddr);
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			ReadOnlySpan<char> span = address.AsSpan();
			int result2 = -1;
			if (span.StartsWith("prg=", StringComparison.OrdinalIgnoreCase))
			{
				int num = span.IndexOf(';');
				if (num < 0)
				{
					return false;
				}
				ReadOnlySpan<char> readOnlySpan = span.Slice(4, num - 4).Trim();
				ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
				for (int i = 0; i < readOnlySpan2.Length; i++)
				{
					char c = readOnlySpan2[i];
					if ((c < '0' || c > '9') ? true : false)
					{
						return false;
					}
				}
				if (readOnlySpan.IsEmpty || !int.TryParse(readOnlySpan.ToString(), out result2))
				{
					return false;
				}
				span = span.Slice(num + 1);
			}
			int j;
			for (j = 0; j < span.Length && char.IsLetter(span[j]); j++)
			{
			}
			if (j == 0 || j > 2)
			{
				return false;
			}
			string text = span.Slice(0, j).ToString().ToUpperInvariant();
			if (!WordBase.ContainsKey(text))
			{
				return false;
			}
			int num2 = j;
			int value = 0;
			int value2 = 0;
			if (isBit)
			{
				int num3 = span.Slice(num2).IndexOf('.');
				if (num3 >= 0)
				{
					ReadOnlySpan<char> span2 = span.Slice(num2, num3);
					ReadOnlySpan<char> span3 = span.Slice(num2 + num3 + 1);
					if (!TryParseHex(span2, out value) || !TryParseHex(span3, out value2))
					{
						return false;
					}
				}
				else
				{
					int i = num2;
					ReadOnlySpan<char> span4 = span.Slice(i, span.Length - 1 - i);
					if (!span4.IsEmpty && !TryParseHex(span4, out value))
					{
						return false;
					}
					if (!TryParseHex(span.Slice(span.Length - 1), out value2))
					{
						return false;
					}
				}
			}
			else if (!TryParseHex(span.Slice(num2), out value))
			{
				return false;
			}
			string text2;
			int num4;
			if (text == "EB")
			{
				if (value < 32768)
				{
					text2 = "EB9";
					num4 = value;
				}
				else if (value < 65536)
				{
					text2 = "EB10";
					num4 = value - 32768;
				}
				else
				{
					text2 = "EB11";
					num4 = value - 65536;
				}
			}
			else
			{
				num4 = WordBase[text] + value;
				text2 = text;
			}
			if (result2 >= 0 && !FixedPrgRegions.Contains(text))
			{
				text2 += $"|P{result2}";
			}
			if (isBit)
			{
				int num5 = num4 * 16 + value2;
				result = new ToyoAddr(text2, num5 / 8, num5 % 8);
			}
			else
			{
				result = new ToyoAddr(text2, num4 * 2, 0);
			}
			return true;
		}

		private static bool TryParseHex(ReadOnlySpan<char> span, out int value)
		{
			value = 0;
			if (span.IsEmpty)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				int num;
				if (c >= '0' && c <= '9')
				{
					num = c - 48;
				}
				else if (c >= 'A' && c <= 'F')
				{
					num = c - 65 + 10;
				}
				else
				{
					if (c < 'a' || c > 'f')
					{
						return false;
					}
					num = c - 97 + 10;
				}
				value = value * 16 + num;
			}
			return true;
		}
	}
	/// <summary>
	/// Vigor（丰炜）PLC 协议组包器。
	/// 驱动语义（VigorAddress）：
	/// 位区（isBit:true）：SM/TS/TC/CS/CC/X/Y/M/S，地址号直接作位号（每位 1 字节）；M≥9000 驱动置 0 归 SM。
	/// 字区（isBit:false）：SD/D/R/T/C，地址号直接作字号（每字 2 字节）；D≥9000 驱动 -9000 归 SD；C≥200 换 DataCode。
	/// 位/字用不同 DataCode（ReadBool isBit:true / Read isBit:false）→ Bool/Word 必须分区域。
	/// </summary>
	public sealed class VigorPacker : PackerBase
	{
		private readonly record struct VigorRegion(string Region, int ByteIndex, int BitIndex);

		private const int REGION_BOUNDARY = 9000;

		/// <summary>位区（驱动 isBit:true 支持）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SM", "TS", "TC", "CS", "CC", "X", "Y", "M", "S" };

		/// <summary>字区（驱动 isBit:false 支持）</summary>
		private static readonly HashSet<string> WordTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SD", "D", "R", "T", "C" };

		public override ProtocolFamily Family => ProtocolFamily.Vigor;

		/// <summary>驱动字读上限 64 字（VigorVsHelper BuildReadCommand SplitIntegerToArray(64)），length 为字单位；
		/// 组包字节批被驱动当字数读 → MaxReadLength=128（64 字）避免 2 倍多读</summary>
		public override int MaxReadLength => 128;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。驱动自动分帧（64字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 类型-区域匹配过滤：位区 Word / 字区 Bool 驱动 ParseFrom 无对应分支
		/// （抛 NotSupportedDataType）→ 降级原样保留，避免组出驱动必失败的批。
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if (!TryParse(address, out var result))
			{
				return true;
			}
			return (int)dataType == 2 == BitTypes.Contains(result.Region);
		}

		/// <summary>
		/// 覆写分组：Bool → |B 位读，其他 → |W 字读；位/字分离避免混批。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
					continue;
				}
				bool flag = (int)addr.AddressDataType == 2;
				list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, 0, address.DataFormat));
			}
			return base.GroupAndSort(list);
		}

		/// <summary>位区：位号直接作字节偏移（1位=1字节）；字区：字号 ×2 字节。M≥9000 驱动置 0 归 SM。</summary>
		private static bool TryParse(string address, out VigorRegion result)
		{
			result = default(VigorRegion);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			for (int j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
			}
			int num2 = int.Parse(readOnlySpan.Slice(num, i - num));
			if (text == "D" && num2 >= 9000)
			{
				result = new VigorRegion("SD", (num2 - 9000) * 2, 0);
				return true;
			}
			if (text == "M" && num2 >= 9000)
			{
				result = new VigorRegion("SM", 0, 0);
				return true;
			}
			bool flag = WordTypes.Contains(text) && !BitTypes.Contains(text);
			if (!flag && !BitTypes.Contains(text))
			{
				return false;
			}
			result = (flag ? new VigorRegion(text, num2 * 2, 0) : new VigorRegion(text, num2, 0));
			return true;
		}
	}
	/// <summary>
	/// 信捷 XinJE TCP 协议组包器。
	/// 驱动语义（XinJEAddress / XinJEHelper）：
	/// 地址号直接进报文（无 ×2/÷8 换算），位读（DataCode 位区）与字读（DataCode 字区）是不同命令 → Bool/Word 必须分区域。
	/// 位读取每位 1 字节（1920 位/次），字读取每字 2 字节（120 字/次）。
	/// X/Y 为八进制地址；D/M/T/C/S 等十进制。
	/// </summary>
	public sealed class XinJEPacker : PackerBase
	{
		/// <summary>驱动地址号直接进报文（字/位同地址号），位读取每位 1 字节 → 位号直接作字节偏移，BitIndex=0</summary>
		private readonly record struct XinJERegion(string Region, int ByteIndex, int BitIndex);

		/// <summary>位区（驱动 ReadBool 支持的类型；X/Y 八进制）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"X", "Y", "M", "S", "SM", "ET", "HM", "HS", "HT", "HC",
			"HSC"
		};

		/// <summary>八进制地址类型（驱动 Convert.ToInt32(addr, 8)）</summary>
		private static readonly HashSet<string> OctalTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y" };

		private static readonly Dictionary<string, string> RegionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["D"] = "D",
			["M"] = "M",
			["X"] = "X",
			["Y"] = "Y",
			["S"] = "S",
			["T"] = "T",
			["C"] = "C",
			["SD"] = "SD",
			["TD"] = "TD",
			["CD"] = "CD",
			["HD"] = "HD",
			["FD"] = "FD",
			["ID"] = "ID",
			["QD"] = "QD",
			["SM"] = "SM",
			["ET"] = "ET",
			["HM"] = "HM",
			["HS"] = "HS",
			["HT"] = "HT",
			["HC"] = "HC",
			["HSC"] = "HSC",
			["HSCD"] = "HSCD",
			["HSD"] = "HSD",
			["HTD"] = "HTD",
			["HCD"] = "HCD",
			["SFD"] = "SFD",
			["ETD"] = "ETD"
		};

		public override ProtocolFamily Family => ProtocolFamily.XinJE;

		/// <summary>驱动单次上限：字路径 120 字 = 240 字节 → 取此值保证字路径不超</summary>
		public override int MaxReadLength => 240;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。驱动自动分帧（120字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 类型-区域匹配过滤：
		/// - 位区 Word → 降级：驱动字读位区用位设备码 + 字功能码（2 字节/字），与 packer 位区字节
		///   （1位1字节）错位
		/// - 字区 Bool → 降级：驱动位读功能码 30 位号连续（间隔 1），与 packer 字区字节（×2 间隔）错位
		/// </summary>
		protected override bool IsPackable(string address, DataType dataType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if (!TryParse(address, out var result))
			{
				return true;
			}
			return (int)dataType == 2 == BitTypes.Contains(result.Region);
		}

		/// <summary>
		/// 覆写分组：按 DataType 分位/字区域（Bool → |B 位读，其他 → |W 字读），避免位/字混批。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
					continue;
				}
				bool flag = (int)addr.AddressDataType == 2;
				list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, 0, address.DataFormat));
			}
			return base.GroupAndSort(list);
		}

		private static bool TryParse(string address, out XinJERegion result)
		{
			result = default(XinJERegion);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			if (i == 0)
			{
				return false;
			}
			string text = readOnlySpan.Slice(0, i).ToString().ToUpperInvariant();
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			bool flag = OctalTypes.Contains(text);
			int num2 = 0;
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			for (int j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				if (c >= '0' && c <= '7')
				{
					num2 = num2 * (flag ? 8 : 10) + (c - 48);
					continue;
				}
				bool flag2 = ((c == '8' || c == '9') ? true : false);
				if (flag2 && !flag)
				{
					num2 = num2 * 10 + (c - 48);
					continue;
				}
				return false;
			}
			string region = (RegionMap.TryGetValue(text, out string value) ? value : text);
			result = new XinJERegion(region, BitTypes.Contains(text) ? num2 : (num2 * 2), 0);
			return true;
		}
	}
	/// <summary>
	/// 安川 Memobus TCP 协议组包器。
	/// 驱动语义（MemobusAddress）：
	/// 无点号 "100"：Bool → SFC1 线圈位读（位号=地址号）；Word → SFC3 寄存器字读（字号=地址号）
	/// 带点号 "100.1"：绝对位号 = 100×16+1 = 1601，SFC 强制转 3（读字取位）
	/// mfc= 主功能码（默认 32）、x= 功能码显式指定。
	/// 区域键 = MF{主功能码}|SFC{功能码}，SFC1 线圈区 / SFC3 寄存器区分开转换。
	/// </summary>
	public sealed class YaskawaPacker : PackerBase
	{
		/// <summary>解析出的 Memobus 地址信息</summary>
		private sealed class MemobusAddr
		{
			public int Address = -1;

			public int BitIndex;

			public bool HasDot;

			public int Function = -1;

			public int MFunction = -1;
		}

		private static readonly string[] ModifierKeys = new string[2] { "x=", "mfc=" };

		public override ProtocolFamily Family => ProtocolFamily.Yaskawa;

		public override int MaxReadLength => 250;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			MemobusAddr memobusAddr = ParseMemobusAddress(address);
			if (memobusAddr == null || memobusAddr.Address < 0)
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: memobusAddr.Address, BitIndex: memobusAddr.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			MemobusAddr memobusAddr = ParseMemobusAddress(address);
			if (memobusAddr != null)
			{
				return BuildRegionKey(memobusAddr);
			}
			return string.Empty;
		}

		/// <summary>
		/// 覆写分组：按 DataType 推断功能码（Bool→SFC1 线圈，Word→SFC3 寄存器）；
		/// 带点号强制 SFC3（驱动 MemobusAddress 对 SFC1 点号地址强制转 3）。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				MemobusAddr memobusAddr = ParseMemobusAddress(address.Addr.AddressName);
				if (memobusAddr == null)
				{
					list.Add(address);
					continue;
				}
				if (memobusAddr.Function < 0)
				{
					memobusAddr.Function = (((int)address.Addr.AddressDataType == 2) ? 1 : 3);
				}
				if (memobusAddr.HasDot && memobusAddr.Function == 1)
				{
					memobusAddr.Function = 3;
				}
				list.Add(new ParsedAddr(address.Addr, BuildRegionKey(memobusAddr), address.ByteIndex, address.BitIndex, address.DataFormat));
			}
			return base.GroupAndSort(list);
		}

		/// <summary>
		/// 覆写拆批：SFC1 线圈区（位号/8 → 字节+位）；SFC3 寄存器区（字号×2 → 字节；Bool 占 2 字节）。
		/// </summary>
		protected override List<(int sn, string regionKey, int sumLength, List<BatchItem> items)> SplitIntoBatches(Dictionary<string, List<AddrInfo>> groups, int maxByteLength)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Invalid comparison between Unknown and I4
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			foreach (KeyValuePair<string, List<AddrInfo>> group in groups)
			{
				bool flag = group.Key.Contains("|SFC1", StringComparison.Ordinal);
				List<AddrInfo> list = new List<AddrInfo>(group.Value.Count);
				foreach (AddrInfo item in group.Value)
				{
					if (flag)
					{
						int byteIndex = item.ByteIndex / 8;
						int bitIndex = item.ByteIndex % 8;
						list.Add(new AddrInfo(item.FullName, item.RegionKey, byteIndex, bitIndex, item.DataType, 1, item.EncodingType, item.DataFormat));
						continue;
					}
					MemobusAddr? memobusAddr = ParseMemobusAddress(item.FullName);
					int num;
					int num2;
					if (memobusAddr == null)
					{
						num = 0;
					}
					else
					{
						num = (memobusAddr.HasDot ? 1 : 0);
						if (num != 0)
						{
							num2 = (item.ByteIndex * 16 + item.BitIndex) / 8;
							goto IL_00e0;
						}
					}
					num2 = item.ByteIndex * 2;
					goto IL_00e0;
					IL_00e0:
					int byteIndex2 = num2;
					int num3 = (((int)item.DataType == 2) ? 2 : item.Length);
					int bitIndex2 = ((num != 0) ? ((item.ByteIndex * 16 + item.BitIndex) % 8) : 0);
					list.Add(new AddrInfo(item.FullName, item.RegionKey, byteIndex2, bitIndex2, item.DataType, (num3 <= 0) ? 2 : num3, item.EncodingType, item.DataFormat));
				}
				group.Value.Clear();
				group.Value.AddRange(list);
			}
			return base.SplitIntoBatches(groups, maxByteLength);
		}

		/// <summary>解析富地址：x= 功能码、mfc= 主功能码、数字[.位号]</summary>
		private static MemobusAddr? ParseMemobusAddress(string address)
		{
			MemobusAddr memobusAddr = new MemobusAddr();
			string[] array = address.Split(';', StringSplitOptions.RemoveEmptyEntries);
			bool flag = false;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i].Trim();
				bool flag2 = false;
				string[] modifierKeys = ModifierKeys;
				foreach (string text2 in modifierKeys)
				{
					if (text.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
					{
						string s = text.Substring(text2.Length).Trim();
						if (text2 == "x=" && int.TryParse(s, out var result))
						{
							memobusAddr.Function = result;
						}
						if (text2 == "mfc=" && int.TryParse(s, out var result2))
						{
							memobusAddr.MFunction = result2;
						}
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					continue;
				}
				int num = text.IndexOf('.');
				string s2 = ((num >= 0) ? text.Substring(0, num) : text);
				string text3 = ((num >= 0) ? text.Substring(num + 1) : "");
				if (int.TryParse(s2, out var result3) && result3 >= 0)
				{
					memobusAddr.Address = result3;
					memobusAddr.HasDot = num >= 0;
					if (memobusAddr.HasDot && !string.IsNullOrEmpty(text3) && int.TryParse(text3, out var result4))
					{
						memobusAddr.BitIndex = result4;
					}
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			return memobusAddr;
		}

		/// <summary>区域键：MF{主功能码}|SFC{功能码}。</summary>
		private static string BuildRegionKey(MemobusAddr addr)
		{
			int value = ((addr.MFunction >= 0) ? addr.MFunction : 32);
			int value2 = ((addr.Function >= 0) ? addr.Function : (-1));
			return $"MF{value}|SFC{value2}";
		}
	}
	/// <summary>
	/// 横河 Yokogawa Link 协议组包器。
	/// 驱动语义（YokogawaLinkAddress / YokogawaLinkTcp）：
	/// 位区（X/Y/I/E/M/T/C/L，ReadBool addressLength=16 字内位 0-15）：地址号直接，位号 = 地址号%16，字节 = 地址号/16×2。
	/// 字区（D/B/F/R/V/Z/W/TN/CN）：地址号 ×2 字节。
	/// 位/字用不同命令（ReadBool isBit:true / Read isBit:false）→ Bool/Word 必须分区域。
	/// </summary>
	public sealed class YokogawaPacker : PackerBase
	{
		private readonly record struct YokoRegion(string Region, int ByteIndex, int BitIndex);

		/// <summary>位区（驱动支持位读，地址号直接）</summary>
		private static readonly HashSet<string> BitTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "X", "Y", "I", "E", "M", "T", "C", "L" };

		/// <summary>字区</summary>
		private static readonly HashSet<string> WordTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "D", "B", "F", "R", "V", "Z", "W", "TN", "CN" };

		public override ProtocolFamily Family => ProtocolFamily.Yokogawa;

		public override int MaxReadLength => 500;

		/// <summary>物理硬上限 = 驱动 Read 的 ushort 上限（65535 字 × 2 = 131070 字节）。驱动自动分帧（64字/帧）。</summary>
		public override int PhysicalMaxBytes => 131070;

		public override bool SupportsAutoPack => true;

		public override (int ByteIndex, int BitIndex) ParseAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			if (!TryParse(address, out var result))
			{
				return (ByteIndex: -1, BitIndex: 0);
			}
			return (ByteIndex: result.ByteIndex, BitIndex: result.BitIndex);
		}

		public override string GetRegionKey(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return string.Empty;
			}
			if (!TryParse(address, out var result))
			{
				return string.Empty;
			}
			return result.Region;
		}

		/// <summary>
		/// 覆写分组：Bool → |B 位读（仅位区），其他 → |W 字读（仅字区）。
		/// 位区 Bool、字区 Word 各走各的；跨区不可组包。
		/// </summary>
		protected override Dictionary<string, List<AddrInfo>> GroupAndSort(List<ParsedAddr> addresses)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Invalid comparison between Unknown and I4
			List<ParsedAddr> list = new List<ParsedAddr>(addresses.Count);
			foreach (ParsedAddr address in addresses)
			{
				AddressDetails addr = address.Addr;
				bool flag = (int)addr.AddressDataType == 2;
				if (!TryParse(addr.AddressName, out var result))
				{
					list.Add(address);
					continue;
				}
				bool flag2 = BitTypes.Contains(result.Region);
				if (flag != flag2)
				{
					list.Add(address);
				}
				else
				{
					list.Add(new ParsedAddr(addr, result.Region + (flag ? "|B" : "|W"), result.ByteIndex, result.BitIndex, address.DataFormat));
				}
			}
			return base.GroupAndSort(list);
		}

		/// <summary>位区：地址号直接，字内位 0-15 → 字节=地址号/16×2、位=地址号%16；字区：×2。</summary>
		private static bool TryParse(string address, out YokoRegion result)
		{
			result = default(YokoRegion);
			ReadOnlySpan<char> readOnlySpan = address.AsSpan();
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsLetter(readOnlySpan[i]); i++)
			{
			}
			string text = ((i > 0) ? readOnlySpan.Slice(0, i).ToString().ToUpperInvariant() : "");
			int num = i;
			for (; i < readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
			{
			}
			if (i <= num)
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num, i - num);
			for (int j = 0; j < readOnlySpan2.Length; j++)
			{
				char c = readOnlySpan2[j];
				if ((c < '0' || c > '9') ? true : false)
				{
					return false;
				}
			}
			int num2 = int.Parse(readOnlySpan.Slice(num, i - num));
			if (BitTypes.Contains(text))
			{
				result = new YokoRegion(text, num2 / 16 * 2 + num2 % 16 / 8, num2 % 8);
				return true;
			}
			if (WordTypes.Contains(text))
			{
				result = new YokoRegion(text, num2 * 2, 0);
				return true;
			}
			return false;
		}
	}
}
namespace Snet.Core.mq
{
	/// <summary>
	/// 消息中间件数据
	/// </summary>
	public class MqData
	{
		/// <summary>
		/// 库文件统一存放文件夹，绝对路径
		/// </summary>
		public string LibFolder { get; set; } = Path.Combine(AppContext.BaseDirectory, "lib", "mq");

		/// <summary>
		/// 库配置文件夹，绝对路径
		/// </summary>
		public string LibConfigFolder { get; set; } = Path.Combine(AppContext.BaseDirectory, "config", "mq");

		/// <summary>
		/// 动态库监控格式
		/// </summary>
		public string DllWatcherFormat { get; set; } = "Snet.*.dll";

		/// <summary>
		/// 配置监控格式
		/// </summary>
		public string ConfigWatcherFormat { get; set; } = "*.Mq.Config.json";

		/// <summary>
		/// 接口名称
		/// </summary>
		public string InterfaceFullName { get; set; } = "Snet.Model.interface.IMq";

		/// <summary>
		/// 任务处理数量
		/// </summary>
		public int TaskNumber { get; set; } = 5;
	}
	/// <summary>
	/// 消息中间件操作；<br />
	/// 库：*.dll；<br />
	/// 库配置：命名空间 + 类名.ISn.Config.json
	/// </summary>
	/// <summary>
	/// MqOperate 的文件监控与插件加载逻辑（partial class）。
	/// 包含 MonitorAsync、文件监控事件处理、防抖去重、配置热加载、插件检索。
	/// </summary>
	/// <summary>
	/// MqOperate 的数据队列处理与失败重试逻辑（partial class）。
	/// 包含消息队列消费、失败实例重试、队列初始化。
	/// </summary>
	public class MqOperate : CoreUnify<MqOperate, MqData>, IDisposable, IAsyncDisposable
	{
		private class QueueData
		{
			public string? Topic { get; set; }

			public object? Content { get; set; }

			public List<string>? ISns { get; set; }
		}

		private class WatcherData
		{
			public enum WatcherType
			{
				Deleted,
				Created,
				Changed
			}

			public FileSystemEventArgs EventArgs { get; set; }

			public int Type { get; set; }

			public WatcherType EventType { get; set; }
		}

		private readonly ConcurrentDictionary<string, IMq> InstanceIoc = new ConcurrentDictionary<string, IMq>();

		private readonly ConcurrentDictionary<string, IMq> OnFailIoc = new ConcurrentDictionary<string, IMq>();

		private readonly ConcurrentDictionary<string, PluginModel> PluginModelIoc = new ConcurrentDictionary<string, PluginModel>();

		private FileSystemWatcher? watcherLibFolder;

		private FileSystemWatcher? watcherLibConfigFolder;

		private ConcurrentDictionary<CancellationTokenSource, Task>? TaskArray;

		private Channel<QueueData>? DataQueue;

		private Channel<WatcherData>? WatcherQueue;

		private CancellationTokenSource? WatcherToken;

		private CancellationTokenSource? OnFailToken;

		private const int OnFailTaskHandlerInterval = 1000;

		private const int WatcherDebounceInterval = 500;

		private readonly PluginOperate pluginOperate = CoreUnify<PluginOperate, string>.Instance(typeof(MqOperate).Name);

		private readonly Lazy<BoundedChannelOptions> _channelOptions = new Lazy<BoundedChannelOptions>(() => new BoundedChannelOptions(65535)
		{
			FullMode = BoundedChannelFullMode.Wait,
			SingleReader = false,
			SingleWriter = false
		});

		private readonly SemaphoreSlim _queueInitSemaphore = new SemaphoreSlim(1, 1);

		private int _disposed;

		private int _watcherInitGuard;

		private readonly TaskCompletionSource<bool> _initCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

		private BoundedChannelOptions ChannelOptions => _channelOptions.Value;

		/// <summary>
		/// 有参构造函数
		/// </summary>
		public MqOperate(MqData basics)
			: base(basics)
		{
			MonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task<OperateResult> RemoveAsync(List<string>? ISns = null)
		{
			await BegOperateAsync(default(CancellationToken), "RemoveAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failMessages = new List<string>();
				if (ISns != null && ISns.Count > 0)
				{
					foreach (string ISn in ISns)
					{
						if (InstanceIoc.TryRemove(ISn, out IMq value))
						{
							if (value != null)
							{
								await ((IAsyncDisposable)value).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
							}
						}
						else
						{
							failMessages.Add("未找到 " + ISn + " 的实例或移除失败");
						}
					}
				}
				else
				{
					foreach (KeyValuePair<string, IMq> item in InstanceIoc)
					{
						await ((IAsyncDisposable)item.Value).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					InstanceIoc.Clear();
				}
				return (failMessages.Count <= 0) ? (await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "RemoveAsync", 138).ConfigureAwait(continueOnCapturedContext: false)) : (await EndOperateAsync(status: false, $"存在 {failMessages.Count} 个失败信息", failMessages, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "RemoveAsync", 137).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "RemoveAsync", 142).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task<OperateResult> DisposeAsync(string ISn)
		{
			await BegOperateAsync(default(CancellationToken), "DisposeAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (InstanceIoc.TryRemove(ISn, out IMq value))
				{
					if (value != null)
					{
						await ((IAsyncDisposable)value).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "DisposeAsync", 155).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, "未找到 " + ISn + " 的实例", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "DisposeAsync", 158).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "DisposeAsync", 162).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task<OperateResult> OnAsync(List<string>? ISns = null)
		{
			await BegOperateAsync(default(CancellationToken), "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failMessages = new List<string>();
				if (ISns != null)
				{
					foreach (string ISn in ISns)
					{
						if (InstanceIoc.TryGetValue(ISn, out IMq value))
						{
							OperateResult val = await ((IOn)value).OnAsync(default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
							if (!((BaseModel)val).Status)
							{
								failMessages.Add(((BaseModel)val).Message);
							}
						}
						else
						{
							failMessages.Add("未找到 " + ISn + " 的实例");
						}
					}
				}
				else
				{
					foreach (KeyValuePair<string, IMq> item in InstanceIoc)
					{
						OperateResult val2 = await ((IOn)item.Value).OnAsync(default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
						if (!((BaseModel)val2).Status)
						{
							failMessages.Add(((BaseModel)val2).Message);
						}
					}
				}
				return (failMessages.Count <= 0) ? (await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "OnAsync", 199).ConfigureAwait(continueOnCapturedContext: false)) : (await EndOperateAsync(status: false, $"存在 {failMessages.Count} 个失败信息", failMessages, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "OnAsync", 198).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "OnAsync", 203).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task<OperateResult> OffAsync(List<string>? ISns = null)
		{
			await BegOperateAsync(default(CancellationToken), "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failMessages = new List<string>();
				if (ISns != null)
				{
					foreach (string ISn in ISns)
					{
						if (InstanceIoc.TryGetValue(ISn, out IMq value))
						{
							OperateResult val = await ((IOff)value).OffAsync(false, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
							if (!((BaseModel)val).Status)
							{
								failMessages.Add(((BaseModel)val).Message);
							}
						}
						else
						{
							failMessages.Add("未找到 " + ISn + " 的实例");
						}
					}
				}
				else
				{
					foreach (KeyValuePair<string, IMq> item in InstanceIoc)
					{
						OperateResult val2 = await ((IOff)item.Value).OffAsync(false, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
						if (!((BaseModel)val2).Status)
						{
							failMessages.Add(((BaseModel)val2).Message);
						}
					}
				}
				return (failMessages.Count <= 0) ? (await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "OffAsync", 240).ConfigureAwait(continueOnCapturedContext: false)) : (await EndOperateAsync(status: false, $"存在 {failMessages.Count} 个失败信息", failMessages, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "OffAsync", 239).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "OffAsync", 244).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 生产消息（同步版本，向后兼容）。
		/// </summary>
		public OperateResult Produce(string Topic, string Content, List<string>? ISns = null)
		{
			return Task.Run(() => ProduceAsync(Topic, Content, ISns)).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 生产消息（同步版本，字节数组，向后兼容）。
		/// </summary>
		public OperateResult Produce(string Topic, byte[] Content, List<string>? ISns = null)
		{
			return Task.Run(() => ProduceAsync(Topic, Content, ISns)).GetAwaiter().GetResult();
		}

		public async Task<OperateResult> ProduceAsync(string Topic, string Content, List<string>? ISns = null)
		{
			return await ProduceInternalAsync(Topic, Content, ISns).ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task<OperateResult> ProduceAsync(string Topic, byte[] Content, List<string>? ISns = null)
		{
			return await ProduceInternalAsync(Topic, Content, ISns).ConfigureAwait(continueOnCapturedContext: false);
		}

		private async Task<OperateResult> ProduceInternalAsync(string topic, object content, List<string>? ISns)
		{
			if (Volatile.Read(in _disposed) != 0)
			{
				return await EndOperateAsync(status: false, "系统已释放", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ProduceInternalAsync", 273).ConfigureAwait(continueOnCapturedContext: false);
			}
			await BegOperateAsync(default(CancellationToken), "ProduceInternalAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				await InitializeQueueAndTasksAsync().ConfigureAwait(continueOnCapturedContext: false);
				List<string> list = new List<string>();
				if (ISns != null && ISns.Count > 0)
				{
					foreach (string ISn in ISns)
					{
						if (!InstanceIoc.ContainsKey(ISn))
						{
							list.Add(ISn + " 实例未找到");
						}
					}
					if (list.Count == ISns.Count)
					{
						return await EndOperateAsync(status: false, "所有指定的实例均不存在", list, null, logOutput: true, consoleOutput: false, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ProduceInternalAsync", 291).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				QueueData item = new QueueData
				{
					Topic = topic,
					Content = content,
					ISns = ISns
				};
				if (DataQueue.Writer.TryWrite(item))
				{
					if (list.Count > 0)
					{
						return await EndOperateAsync(status: true, $"消息已入队，但有 {list.Count} 个实例未找到", list, null, logOutput: true, consoleOutput: false, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ProduceInternalAsync", 299).ConfigureAwait(continueOnCapturedContext: false);
					}
					return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ProduceInternalAsync", 300).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, "数据队列已满，请稍后重试", null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ProduceInternalAsync", 303).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ProduceInternalAsync", 307).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task<OperateResult> ConsumeAsync(string Topic, List<string>? ISns = null)
		{
			await BegOperateAsync(default(CancellationToken), "ConsumeAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failMessages = new List<string>();
				if (ISns != null)
				{
					string item = default(string);
					foreach (string ISn in ISns)
					{
						if (InstanceIoc.TryGetValue(ISn, out IMq value))
						{
							if (!((ResultModel)(await ((IConsumer)value).ConsumeAsync(Topic, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false))).GetDetails(ref item))
							{
								failMessages.Add(item);
							}
						}
						else
						{
							failMessages.Add("未找到 " + ISn + " 的实例");
						}
					}
				}
				else
				{
					string item2 = default(string);
					foreach (KeyValuePair<string, IMq> item3 in InstanceIoc)
					{
						if (!((ResultModel)(await ((IConsumer)item3.Value).ConsumeAsync(Topic, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false))).GetDetails(ref item2))
						{
							failMessages.Add(item2);
						}
					}
				}
				return (failMessages.Count <= 0) ? (await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ConsumeAsync", 346).ConfigureAwait(continueOnCapturedContext: false)) : (await EndOperateAsync(status: false, $"存在 {failMessages.Count} 个失败信息", failMessages, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ConsumeAsync", 345).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "ConsumeAsync", 350).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task<OperateResult> UnConsumeAsync(string Topic, List<string>? ISns = null)
		{
			await BegOperateAsync(default(CancellationToken), "UnConsumeAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failMessages = new List<string>();
				if (ISns != null)
				{
					string item = default(string);
					foreach (string ISn in ISns)
					{
						if (InstanceIoc.TryGetValue(ISn, out IMq value))
						{
							if (!((ResultModel)(await ((IConsumer)value).UnConsumeAsync(Topic, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false))).GetDetails(ref item))
							{
								failMessages.Add(item);
							}
						}
						else
						{
							failMessages.Add("未找到 " + ISn + " 的实例");
						}
					}
				}
				else
				{
					string item2 = default(string);
					foreach (KeyValuePair<string, IMq> item3 in InstanceIoc)
					{
						if (!((ResultModel)(await ((IConsumer)item3.Value).UnConsumeAsync(Topic, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false))).GetDetails(ref item2))
						{
							failMessages.Add(item2);
						}
					}
				}
				return (failMessages.Count <= 0) ? (await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "UnConsumeAsync", 385).ConfigureAwait(continueOnCapturedContext: false)) : (await EndOperateAsync(status: false, $"存在 {failMessages.Count} 个失败信息", failMessages, null, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "UnConsumeAsync", 384).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\mq\\MqOperate.cs", "UnConsumeAsync", 389).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public override void Dispose()
		{
			if (Interlocked.Exchange(ref _disposed, 1) == 1)
			{
				return;
			}
			if (DataQueue != null)
			{
				DataQueue.Writer.TryComplete();
				QueueData item;
				while (DataQueue.Reader.TryRead(out item))
				{
				}
				DataQueue = null;
			}
			if (TaskArray != null)
			{
				foreach (KeyValuePair<CancellationTokenSource, Task> item3 in TaskArray)
				{
					try
					{
						item3.Key.Cancel();
					}
					catch (ObjectDisposedException)
					{
					}
					try
					{
						item3.Key.Dispose();
					}
					catch
					{
					}
				}
				TaskArray.Clear();
				TaskArray = null;
			}
			if (OnFailToken != null)
			{
				OnFailToken.Cancel();
				OnFailToken.Dispose();
				OnFailToken = null;
			}
			if (WatcherToken != null)
			{
				WatcherToken.Cancel();
				WatcherToken.Dispose();
				WatcherToken = null;
			}
			if (WatcherQueue != null)
			{
				WatcherQueue.Writer.TryComplete();
				WatcherData item2;
				while (WatcherQueue.Reader.TryRead(out item2))
				{
				}
				WatcherQueue = null;
			}
			if (watcherLibFolder != null)
			{
				watcherLibFolder.EnableRaisingEvents = false;
				watcherLibFolder.Dispose();
				watcherLibFolder = null;
			}
			if (watcherLibConfigFolder != null)
			{
				watcherLibConfigFolder.EnableRaisingEvents = false;
				watcherLibConfigFolder.Dispose();
				watcherLibConfigFolder = null;
			}
			foreach (KeyValuePair<string, IMq> item4 in InstanceIoc)
			{
				try
				{
					((IDisposable)item4.Value).Dispose();
				}
				catch
				{
				}
			}
			InstanceIoc.Clear();
			OnFailIoc.Clear();
			PluginModelIoc.Clear();
			try
			{
				_queueInitSemaphore.Dispose();
			}
			catch
			{
			}
			pluginOperate.Dispose();
			base.Dispose();
		}

		public override async ValueTask DisposeAsync()
		{
			if (Interlocked.Exchange(ref _disposed, 1) == 1)
			{
				return;
			}
			if (DataQueue != null)
			{
				DataQueue.Writer.TryComplete();
				QueueData item;
				while (DataQueue.Reader.TryRead(out item))
				{
				}
				DataQueue = null;
			}
			if (TaskArray != null)
			{
				foreach (KeyValuePair<CancellationTokenSource, Task> item2 in TaskArray)
				{
					try
					{
						await item2.Key.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (ObjectDisposedException)
					{
					}
					try
					{
						item2.Key.Dispose();
					}
					catch
					{
					}
				}
				TaskArray.Clear();
				TaskArray = null;
			}
			if (OnFailToken != null)
			{
				await OnFailToken.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				OnFailToken.Dispose();
				OnFailToken = null;
			}
			if (WatcherToken != null)
			{
				await WatcherToken.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				WatcherToken.Dispose();
				WatcherToken = null;
			}
			if (WatcherQueue != null)
			{
				WatcherQueue.Writer.TryComplete();
				WatcherData item3;
				while (WatcherQueue.Reader.TryRead(out item3))
				{
				}
				WatcherQueue = null;
			}
			if (watcherLibFolder != null)
			{
				watcherLibFolder.EnableRaisingEvents = false;
				watcherLibFolder.Dispose();
				watcherLibFolder = null;
			}
			if (watcherLibConfigFolder != null)
			{
				watcherLibConfigFolder.EnableRaisingEvents = false;
				watcherLibConfigFolder.Dispose();
				watcherLibConfigFolder = null;
			}
			foreach (KeyValuePair<string, IMq> item4 in InstanceIoc)
			{
				try
				{
					await ((IAsyncDisposable)item4.Value).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch
				{
				}
			}
			InstanceIoc.Clear();
			OnFailIoc.Clear();
			PluginModelIoc.Clear();
			try
			{
				_queueInitSemaphore.Dispose();
			}
			catch
			{
			}
			await pluginOperate.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 初始化监控系统（带 disposed 和重复调用防护）<br />
		/// 执行顺序：<br />
		/// 1. 启动打开失败重试任务<br />
		/// 2. 创建必要的目录结构<br />
		/// 3. 初始化文件监控事件队列和消费任务<br />
		/// 4. 检索现有库文件并创建实例<br />
		/// 5. 启动文件系统监控器
		/// </summary>
		private async Task MonitorAsync()
		{
			if (Volatile.Read(in _disposed) != 0)
			{
				return;
			}
			try
			{
				if (OnFailToken == null)
				{
					OnFailToken = new CancellationTokenSource();
					OnFailTaskHandlerAsync(OnFailToken.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				if (base.basics == null)
				{
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "配置文件不存在")).ConfigureAwait(continueOnCapturedContext: false);
					_initCompletion.TrySetResult(result: false);
					return;
				}
				if (!Directory.Exists(base.basics.LibFolder))
				{
					Directory.CreateDirectory(base.basics.LibFolder);
				}
				if (!Directory.Exists(base.basics.LibConfigFolder))
				{
					Directory.CreateDirectory(base.basics.LibConfigFolder);
				}
				EnsureWatcherInitialized();
				await SearchAsync().ConfigureAwait(continueOnCapturedContext: false);
				watcherLibFolder = new FileSystemWatcher(base.basics.LibFolder)
				{
					NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Security)
				};
				watcherLibFolder.Created += delegate(object sender, FileSystemEventArgs e)
				{
					Watcher_OnEvent(e, 0, WatcherData.WatcherType.Created);
				};
				watcherLibFolder.Deleted += delegate(object sender, FileSystemEventArgs e)
				{
					Watcher_OnEvent(e, 0, WatcherData.WatcherType.Deleted);
				};
				watcherLibFolder.EnableRaisingEvents = true;
				watcherLibConfigFolder = new FileSystemWatcher(base.basics.LibConfigFolder)
				{
					Filter = base.basics.ConfigWatcherFormat,
					NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Security)
				};
				watcherLibConfigFolder.Created += delegate(object sender, FileSystemEventArgs e)
				{
					Watcher_OnEvent(e, 1, WatcherData.WatcherType.Created);
				};
				watcherLibConfigFolder.Deleted += delegate(object sender, FileSystemEventArgs e)
				{
					Watcher_OnEvent(e, 1, WatcherData.WatcherType.Deleted);
				};
				watcherLibConfigFolder.Changed += delegate(object sender, FileSystemEventArgs e)
				{
					Watcher_OnEvent(e, 1, WatcherData.WatcherType.Changed);
				};
				watcherLibConfigFolder.EnableRaisingEvents = true;
				_initCompletion.TrySetResult(result: true);
			}
			catch (Exception ex)
			{
				await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "初始化异常: " + ex.Message)).ConfigureAwait(continueOnCapturedContext: false);
				_initCompletion.TrySetResult(result: false);
			}
		}

		/// <summary>
		/// 确保文件监控队列和消费任务已初始化<br />
		/// 使用 CAS（Compare-And-Swap）保证线程安全的惰性单次初始化<br />
		/// 注意：此处保留无锁 CAS 而非 SemaphoreSlim，因为它在 FileSystemWatcher 的<br />
		/// 同步回调线程上被调用，无法使用异步锁
		/// </summary>
		private void EnsureWatcherInitialized()
		{
			if ((WatcherQueue == null || WatcherToken == null) && Interlocked.CompareExchange(ref _watcherInitGuard, 1, 0) == 0 && WatcherQueue == null)
			{
				WatcherQueue = Channel.CreateBounded<WatcherData>(ChannelOptions);
				WatcherToken = new CancellationTokenSource();
				WatcherTaskAsync(WatcherToken.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 统一的文件监控事件入队方法<br />
		/// 使用 TryWrite 同步写入，避免在 FileSystemWatcher 回调线程上阻塞
		/// </summary>
		private void Watcher_OnEvent(FileSystemEventArgs e, int type, WatcherData.WatcherType eventType)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected O, but got Unknown
			if (Volatile.Read(in _disposed) != 0)
			{
				return;
			}
			EnsureWatcherInitialized();
			if (WatcherQueue == null)
			{
				OnInfoEventHandler(this, new EventInfoResult(false, "文件监控队列未初始化，事件被丢弃: " + e.FullPath));
				return;
			}
			WatcherData item = new WatcherData
			{
				EventArgs = e,
				Type = type,
				EventType = eventType
			};
			if (!WatcherQueue.Writer.TryWrite(item))
			{
				OnInfoEventHandler(this, new EventInfoResult(false, "文件监控队列已满，事件被丢弃: " + e.FullPath));
			}
		}

		private async Task WatcherTaskAsync(CancellationToken token)
		{
			try
			{
				while (await WatcherQueue.Reader.WaitToReadAsync(token).ConfigureAwait(continueOnCapturedContext: false))
				{
					List<WatcherData> batch = new List<WatcherData>();
					WatcherData item;
					while (WatcherQueue.Reader.TryRead(out item))
					{
						batch.Add(item);
					}
					await Task.Delay(500, token).ConfigureAwait(continueOnCapturedContext: false);
					WatcherData item2;
					while (WatcherQueue.Reader.TryRead(out item2))
					{
						batch.Add(item2);
					}
					Dictionary<string, WatcherData> dictionary = new Dictionary<string, WatcherData>();
					foreach (WatcherData item3 in batch)
					{
						dictionary[$"{item3.EventArgs.FullPath}|{item3.EventType}"] = item3;
					}
					foreach (WatcherData watcherData in dictionary.Values)
					{
						if (token.IsCancellationRequested)
						{
							return;
						}
						try
						{
							await ProcessWatcherEventAsync(watcherData, token).ConfigureAwait(continueOnCapturedContext: false);
						}
						catch (OperationCanceledException) when (token.IsCancellationRequested)
						{
							return;
						}
						catch (Exception ex2)
						{
							await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "文件监控事件处理异常 [" + watcherData.EventArgs.FullPath + "]: " + ex2.Message)).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex4)
			{
				await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("[WatcherTaskAsync] 监控任务异常退出: " + ex4.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		private async Task ProcessWatcherEventAsync(WatcherData watcherData, CancellationToken token)
		{
			FileSystemEventArgs eventArgs = watcherData.EventArgs;
			int type = watcherData.Type;
			switch (watcherData.EventType)
			{
			case WatcherData.WatcherType.Deleted:
				switch (type)
				{
				case 0:
					await HandleLibFolderDeletedAsync(eventArgs).ConfigureAwait(continueOnCapturedContext: false);
					break;
				case 1:
					await HandleConfigFileDeletedAsync(eventArgs).ConfigureAwait(continueOnCapturedContext: false);
					break;
				}
				break;
			case WatcherData.WatcherType.Created:
				switch (type)
				{
				case 0:
					if (Directory.Exists(eventArgs.FullPath))
					{
						await SearchAsync(eventArgs.FullPath).ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				case 1:
					await TryLoadConfigAndCreateInstanceAsync(eventArgs, token).ConfigureAwait(continueOnCapturedContext: false);
					break;
				}
				break;
			case WatcherData.WatcherType.Changed:
				if (type == 1)
				{
					await HandleConfigFileChangedAsync(eventArgs, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				break;
			}
		}

		private async Task HandleLibFolderDeletedAsync(FileSystemEventArgs e)
		{
			if (Directory.Exists(e.FullPath))
			{
				return;
			}
			string dirName = Path.GetFileName(e.FullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).Replace(".Pack", string.Empty);
			foreach (KeyValuePair<string, IMq> item in InstanceIoc)
			{
				if (item.Key.Contains(dirName))
				{
					await ((IAsyncDisposable)item.Value).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					if (InstanceIoc.TryRemove(item.Key, out IMq _))
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(true, e.Name + " 移除实例 " + item.Key + " 成功")).ConfigureAwait(continueOnCapturedContext: false);
					}
					else
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(false, e.Name + " 移除实例 " + item.Key + " 失败")).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			if (PluginModelIoc.TryGetValue(e.FullPath, out PluginModel value2))
			{
				await pluginOperate.RemovePluginAsync(value2.Name).ConfigureAwait(continueOnCapturedContext: false);
				PluginModelIoc.TryRemove(e.FullPath, out PluginModel _);
			}
		}

		private async Task HandleConfigFileDeletedAsync(FileSystemEventArgs e)
		{
			string sn = Path.GetFileNameWithoutExtension(e.FullPath);
			if (InstanceIoc.TryRemove(sn, out IMq value))
			{
				await ((IAsyncDisposable)value).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				await OnInfoEventHandlerAsync(this, new EventInfoResult(true, e.Name + " 移除实例成功")).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (OnFailIoc.TryRemove(sn, out IMq value2))
			{
				await ((IAsyncDisposable)value2).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				await OnInfoEventHandlerAsync(this, new EventInfoResult(true, e.Name + " 移除失败重试队列数据成功")).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		private async Task HandleConfigFileChangedAsync(FileSystemEventArgs e, CancellationToken token)
		{
			string sn = Path.GetFileNameWithoutExtension(e.FullPath);
			if (InstanceIoc.TryRemove(sn, out IMq value))
			{
				await ((IAsyncDisposable)value).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				await OnInfoEventHandlerAsync(this, new EventInfoResult(true, e.Name + " 配置文件发生修改，原实例已释放，准备重载")).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (OnFailIoc.TryRemove(sn, out IMq value2))
			{
				await ((IAsyncDisposable)value2).DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			await TryLoadConfigAndCreateInstanceAsync(e, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		private async Task TryLoadConfigAndCreateInstanceAsync(FileSystemEventArgs e, CancellationToken token)
		{
			for (int attempt = 0; attempt < 5; attempt++)
			{
				try
				{
					string fullPath = e.FullPath;
					string fileName = Path.GetFileName(fullPath);
					string sn = Path.GetFileNameWithoutExtension(fullPath);
					string operatePart = fileName.Split('.').FirstOrDefault((string part) => part.Contains("Operate"));
					if (string.IsNullOrWhiteSpace(operatePart))
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "无法从配置文件名解析出插件程序集前缀名: " + fileName)).ConfigureAwait(continueOnCapturedContext: false);
						break;
					}
					if (string.IsNullOrWhiteSpace(pluginOperate.GetPluginPath(operatePart)))
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(false, e.Name + " 对应的程序集不存在，无法创建实例 " + sn)).ConfigureAwait(continueOnCapturedContext: false);
						break;
					}
					string param = await FileHandler.FileToStringAsync(fullPath, (Encoding)null, true, token).ConfigureAwait(continueOnCapturedContext: false);
					if (!InstanceIoc.ContainsKey(sn))
					{
						IMq mq = await pluginOperate.CreateAsync<IMq>(operatePart, param, (PluginType)1).ConfigureAwait(continueOnCapturedContext: false);
						if (mq == null)
						{
							await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "创建实例 " + sn + " 失败，插件返回 null")).ConfigureAwait(continueOnCapturedContext: false);
						}
						else if (((BaseModel)(await ((IOn)mq).OnAsync(token).ConfigureAwait(continueOnCapturedContext: false))).Status)
						{
							InstanceIoc[sn] = mq;
							await OnInfoEventHandlerAsync(this, new EventInfoResult(true, "实例 " + sn + " 基于配置成功加载并启动")).ConfigureAwait(continueOnCapturedContext: false);
						}
						else
						{
							await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "实例 " + sn + " 打开失败，已添加至重新打开列表")).ConfigureAwait(continueOnCapturedContext: false);
							OnFailIoc[sn] = mq;
						}
					}
					break;
				}
				catch (OperationCanceledException) when (token.IsCancellationRequested)
				{
					break;
				}
				catch (IOException)
				{
					if (attempt >= 4)
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "配置文件加载失败，已达到最大重试次数: " + e.FullPath)).ConfigureAwait(continueOnCapturedContext: false);
						continue;
					}
					int delay = 500 * (1 << attempt);
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"配置文件加载中遇到 I/O 异常，{delay}ms 后重试 ({attempt + 1}/{5})")).ConfigureAwait(continueOnCapturedContext: false);
					await Task.Delay(delay, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception ex3)
				{
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "加载配置文件异常：" + ex3.Message)).ConfigureAwait(continueOnCapturedContext: false);
					if (attempt < 4)
					{
						await Task.Delay(500, token).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
		}

		private async Task SearchAsync(string? path = null)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(path))
				{
					path = base.basics.LibFolder;
				}
				List<string> list = (from f in (from f in (await Task.Run(() => Directory.GetFiles(path, base.basics.DllWatcherFormat, SearchOption.AllDirectories).ToList()).ConfigureAwait(continueOnCapturedContext: false)).Select(Path.GetDirectoryName)
						where !string.IsNullOrEmpty(f)
						select f).Distinct()
					where !f.Split(Path.DirectorySeparatorChar).Any((string d) => string.Equals(d, "en", StringComparison.OrdinalIgnoreCase))
					select f).ToList();
				foreach (string item2 in list)
				{
					string directoryName = Path.GetDirectoryName(item2);
					foreach (var item in await pluginOperate.InitPluginAsync(directoryName, base.basics.InterfaceFullName).ConfigureAwait(continueOnCapturedContext: false))
					{
						if (!PluginModelIoc.ContainsKey(item.Item1.Path))
						{
							PluginModelIoc[item.Item1.Path] = item.Item1;
						}
						string format = item.Item1.ConfigFormat.Replace("{0}", "*");
						foreach (FileInfo c in await Task.Run(() => new DirectoryInfo(base.basics.LibConfigFolder).GetFiles(format, SearchOption.AllDirectories).ToList()).ConfigureAwait(continueOnCapturedContext: false))
						{
							string text = base.basics.ConfigWatcherFormat.Replace("*", "");
							string fileName = Path.GetFileName(c.FullName);
							string text2 = fileName;
							int length = text.Length;
							string sn = text2.Substring(0, text2.Length - length);
							if (InstanceIoc.ContainsKey(sn))
							{
								continue;
							}
							try
							{
								string param = await FileHandler.FileToStringAsync(c.FullName, (Encoding)null, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
								IMq mq = await pluginOperate.CreateAsync<IMq>(item.Item1.Name, param, (PluginType)1).ConfigureAwait(continueOnCapturedContext: false);
								if (mq != null)
								{
									if (((BaseModel)(await ((IOn)mq).OnAsync(default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false))).Status)
									{
										InstanceIoc[sn] = mq;
										continue;
									}
									await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "实例 " + sn + " 打开失败，已添加至重新打开列表")).ConfigureAwait(continueOnCapturedContext: false);
									OnFailIoc[sn] = mq;
								}
							}
							catch (Exception ex)
							{
								await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "加载配置文件 " + c.Name + " 异常：" + ex.Message)).ConfigureAwait(continueOnCapturedContext: false);
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "检索异常：" + ex2.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 周期性地尝试重新打开失败的 MQ 实例<br />
		/// 每隔 OnFailTaskHandlerInterval 毫秒检查一次 OnFailIoc 队列
		/// </summary>
		private async Task OnFailTaskHandlerAsync(CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					List<KeyValuePair<string, IMq>> list = OnFailIoc.ToList();
					List<string> succeedKeys = new List<string>();
					foreach (KeyValuePair<string, IMq> item in list)
					{
						if (token.IsCancellationRequested)
						{
							break;
						}
						try
						{
							if (((BaseModel)(await ((IOn)item.Value).OnAsync(token).ConfigureAwait(continueOnCapturedContext: false))).Status && OnFailIoc.TryRemove(item.Key, out IMq _))
							{
								InstanceIoc[item.Key] = item.Value;
								succeedKeys.Add(item.Key);
							}
						}
						catch (OperationCanceledException) when (token.IsCancellationRequested)
						{
							break;
						}
						catch (Exception ex2)
						{
							await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("[OnFailTaskHandler] 重试实例 " + item.Key + " 异常: " + ex2.Message)).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					foreach (string item2 in succeedKeys)
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(true, "[OnFailTaskHandler] 实例 " + item2 + " 重试打开成功")).ConfigureAwait(continueOnCapturedContext: false);
					}
					await Task.Delay(1000, token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex4)
			{
				await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("[OnFailTaskHandlerAsync] 任务异常退出: " + ex4.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 数据队列的异步消费任务<br />
		/// 从 DataQueue 中读取消息并分发到对应的 MQ 实例进行生产
		/// </summary>
		private async Task TaskHandleAsync(CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested && Volatile.Read(in _disposed) == 0)
				{
					Channel<QueueData> queue = DataQueue;
					bool flag = queue == null;
					if (!flag)
					{
						flag = !(await queue.Reader.WaitToReadAsync(token).ConfigureAwait(continueOnCapturedContext: false));
					}
					if (flag)
					{
						break;
					}
					QueueData item;
					while (queue.Reader.TryRead(out item))
					{
						if (token.IsCancellationRequested)
						{
							return;
						}
						try
						{
							await ProcessQueueDataAsync(item).ConfigureAwait(continueOnCapturedContext: false);
						}
						catch (OperationCanceledException) when (token.IsCancellationRequested)
						{
							return;
						}
						catch (Exception ex2)
						{
							await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("[TaskHandleAsync] 消息处理异常: " + ex2.Message)).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex4)
			{
				await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("[TaskHandleAsync] 任务异常退出: " + ex4.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 处理队列中的单条数据<br />
		/// 根据 ISns 是否为空决定是广播还是定向发送
		/// </summary>
		private async Task ProcessQueueDataAsync(QueueData queueData)
		{
			if (queueData == null)
			{
				return;
			}
			List<string>? iSns = queueData.ISns;
			if (iSns == null || !iSns.Any())
			{
				foreach (IMq item in InstanceIoc.Values.Where((IMq p) => ((BaseModel)((IStatus)p).GetStatus()).Status))
				{
					await ProcessDataAsync(item, queueData.Content, queueData.Topic).ConfigureAwait(continueOnCapturedContext: false);
				}
				return;
			}
			foreach (string iSn in queueData.ISns)
			{
				if (InstanceIoc.TryGetValue(iSn, out IMq value) && ((BaseModel)((IStatus)value).GetStatus()).Status)
				{
					await ProcessDataAsync(value, queueData.Content, queueData.Topic).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		/// <summary>
		/// 将消息内容发送到指定 MQ 实例的指定主题
		/// </summary>
		private async Task ProcessDataAsync(IMq producer, object? content, string? topic)
		{
			try
			{
				OperateResult val = null;
				if (content is byte[] array)
				{
					val = await ((IProducer)producer).ProduceAsync(topic, array, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
				else if (content is string text)
				{
					val = await ((IProducer)producer).ProduceAsync(topic, text, (Encoding)null, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
				if (val == null)
				{
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "不支持的内容类型：" + content?.GetType().Name)).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					await OnInfoEventHandlerAsync(this, new EventInfoResult(((BaseModel)val).Status, ((BaseModel)val).Message)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (Exception ex)
			{
				await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "消息发送异常：" + ex.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 确保数据队列和处理任务已初始化<br />
		/// 使用 SemaphoreSlim 异步锁替代 sync lock，双重检查锁定模式
		/// </summary>
		private async Task InitializeQueueAndTasksAsync()
		{
			if (DataQueue != null && TaskArray != null)
			{
				return;
			}
			await _queueInitSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (DataQueue == null)
				{
					DataQueue = Channel.CreateBounded<QueueData>(ChannelOptions);
				}
				if (TaskArray == null)
				{
					TaskArray = new ConcurrentDictionary<CancellationTokenSource, Task>();
					for (int i = 0; i < base.basics.TaskNumber; i++)
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
						TaskArray[cancellationTokenSource] = TaskHandleAsync(cancellationTokenSource.Token);
					}
				}
			}
			finally
			{
				_queueInitSemaphore.Release();
			}
		}
	}
}
namespace Snet.Core.handler
{
	/// <summary>
	/// 地址统一管理（静态类）。
	/// <para>
	/// 提供底层硬件数据的类型转换、数据解析、消息中间件生产等功能。<br />
	/// 支持 30+ 种数据类型的自动转换，包括基础类型和数组类型。
	/// </para>
	/// <para>
	/// <b>线程安全：</b>所有公开方法均为线程安全，可从多个采集线程并发调用。
	/// </para>
	/// </summary>
	public static class AddressHandler
	{
		/// <summary>
		/// 执行处理日志输出目录
		/// </summary>
		private const string ExecuteDisposeLog = "Core/AddressHandler/ExecuteDispose";

		/// <summary>
		/// 解析日志输出目录
		/// </summary>
		private const string ParseLog = "Core/AddressHandler/Parse";

		/// <summary>
		/// 生产日志输出目录
		/// </summary>
		private const string ProduceLog = "Core/AddressHandler/Produce";

		/// <summary>
		/// 消息中间件操作（惰性初始化）。
		/// </summary>
		private static readonly Lazy<MqOperate> _mqOperateLazy = new Lazy<MqOperate>(() => CoreUnify<MqOperate, MqData>.Instance(new MqData()));

		/// <summary>
		/// 反射实例容器。
		/// </summary>
		private static readonly ConcurrentDictionary<ReflectionData.Basics, ReflectionOperate> _reflectionContainer = new ConcurrentDictionary<ReflectionData.Basics, ReflectionOperate>();

		/// <summary>
		/// 反射初始化的异步并发控制信号量，替代原 sync lock。
		/// 容量 1 实现互斥，支持异步等待不阻塞线程。
		/// </summary>
		private static readonly SemaphoreSlim _parseSemaphore = new SemaphoreSlim(1, 1);

		/// <summary>
		/// 获取 MQ 操作实例（惰性初始化）
		/// </summary>
		private static MqOperate MqOperateInstance => _mqOperateLazy.Value;

		/// <summary>
		/// 执行数据处理的主入口。
		/// <para>
		/// 处理流程：<br />
		/// 1. 空值检查 → 标记异常<br />
		/// 2. 类型转换（字符串 → 目标数据类型）<br />
		/// 3. 数据解析（若配置了解析参数，通过反射执行外部解析逻辑）<br />
		/// 4. 消息中间件生产（若配置了 MQ 参数，将数据发送到指定主题）
		/// </para>
		/// </summary>
		/// <param name="addressDetails">地址详情数据</param>
		/// <param name="value">底层硬件返回的原始值</param>
		/// <param name="message">附加详细信息</param>
		/// <returns>地址值；处理异常时返回 null</returns>
		public static AddressValue? ExecuteDispose(this AddressDetails addressDetails, object? value, string? message)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Invalid comparison between Unknown and I4
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Invalid comparison between Unknown and I4
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Invalid comparison between Unknown and I4
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				object originalValue = null;
				object obj = null;
				QualityType val = (QualityType)(-1);
				if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
				{
					val = (QualityType)0;
				}
				else
				{
					string value2 = (((object)addressDetails.AddressDataType/*cast due to constrained. prefix*/).ToString().EndsWith("Array") ? ExtendMethod.ToJson<object>(value, false) : value.ToString());
					val = DataTypeConvert(addressDetails, value2, out object outValue, out string message2);
					if ((int)val == 1)
					{
						originalValue = outValue;
						obj = outValue;
						(QualityType Quality, object? Value, string? Message) tuple = Parse(addressDetails, obj);
						QualityType item = tuple.Quality;
						object item2 = tuple.Value;
						string item3 = tuple.Message;
						obj = item2;
						val = item;
						if (!string.IsNullOrWhiteSpace(item3))
						{
							message = item3;
						}
					}
					else if (!string.IsNullOrWhiteSpace(message2))
					{
						message = message2;
					}
				}
				AddressValue val2 = new AddressValue().SET(addressDetails);
				val2.ResultValue = obj;
				val2.OriginalValue = originalValue;
				val2.Quality = val;
				val2.Message = message;
				if (((int)val == 1 || (int)val == 3) ? true : false)
				{
					Produce(addressDetails, obj);
				}
				return val2;
			}
			catch (Exception ex)
			{
				try
				{
					LogHelper.Error(SafeGetLanguage("执行数据处理异常") + "：" + ex.Message, "Core/AddressHandler/ExecuteDispose", ex, true);
				}
				catch
				{
				}
			}
			return null;
		}

		/// <summary>
		/// 执行特殊处理。
		/// <para>
		/// 处理 string 值数据：先强制转换数据类型，再调用标准处理流程。<br />
		/// 若类型转换失败则直接返回包含错误质量码的结果。
		/// </para>
		/// </summary>
		/// <param name="addressDetails">地址详情数据</param>
		/// <param name="value">底层硬件返回的字符串值</param>
		/// <param name="message">附加详细信息</param>
		/// <returns>地址值</returns>
		public static AddressValue? ExecuteSpecialDispose(this AddressDetails addressDetails, string? value, string? message)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			QualityType val = DataTypeConvert(addressDetails, value, out object outValue, out string message2);
			if ((int)val == 1)
			{
				return addressDetails.ExecuteDispose(outValue, message);
			}
			AddressValue obj = new AddressValue().SET(addressDetails);
			obj.ResultValue = value;
			obj.OriginalValue = value;
			obj.Quality = val;
			obj.Message = message2;
			return obj;
		}

		/// <summary>
		/// 数据类型转换。
		/// <para>
		/// 根据 DataType 枚举将字符串值转换为对应的 .NET 类型。<br />
		/// 支持：Bool/String/Char/Byte/Double/Float/Short/UShort/Int/UInt/Long/ULong/<br />
		/// DateTime 及其数组版本（ByteArray/BoolArray/DoubleArray 等）。
		/// </para>
		/// </summary>
		/// <param name="addressDetails">地址详情</param>
		/// <param name="value">待转换的字符串值</param>
		/// <param name="outValue">转换后的值</param>
		/// <param name="message">错误消息</param>
		/// <returns>数据质量码</returns>
		private static QualityType DataTypeConvert(AddressDetails addressDetails, string? value, out object? outValue, out string? message)
		{
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected I4, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			outValue = null;
			message = string.Empty;
			if (string.IsNullOrEmpty(value))
			{
				if ((int)addressDetails.AddressDataType == 0)
				{
					outValue = value;
					return (QualityType)1;
				}
				outValue = null;
				message = $"[ {addressDetails.AddressName} ] 值为空，无法转换为目标类型 {addressDetails.AddressDataType}";
				return (QualityType)2;
			}
			try
			{
				string value2 = value.Trim('"');
				DataType addressDataType = addressDetails.AddressDataType;
				switch ((int)addressDataType)
				{
				case 1:
					outValue = Convert.ToByte(value);
					break;
				case 2:
					outValue = Convert.ToBoolean(value);
					break;
				case 21:
					outValue = value.Replace("\"", string.Empty);
					break;
				case 22:
					outValue = Convert.ToChar(value);
					break;
				case 3:
					outValue = Convert.ToDouble(value2, CultureInfo.InvariantCulture);
					break;
				case 4:
				case 5:
					outValue = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
					break;
				case 6:
				case 7:
					outValue = Convert.ToInt16(value2);
					break;
				case 10:
				case 11:
					outValue = Convert.ToInt32(value2);
					break;
				case 14:
				case 15:
					outValue = Convert.ToInt64(value2);
					break;
				case 8:
				case 9:
					outValue = Convert.ToUInt16(value2);
					break;
				case 12:
				case 13:
					outValue = Convert.ToUInt32(value2);
					break;
				case 16:
				case 17:
					outValue = Convert.ToUInt64(value2);
					break;
				case 18:
				case 19:
				case 20:
					outValue = Convert.ToDateTime(value);
					break;
				case 23:
					outValue = ExtendMethod.ToJsonEntity<byte[]>(value, (JsonSerializerOptions)null);
					break;
				case 24:
					outValue = ExtendMethod.ToJsonEntity<bool[]>(value, (JsonSerializerOptions)null);
					break;
				case 25:
					outValue = ExtendMethod.ToJsonEntity<double[]>(value, (JsonSerializerOptions)null);
					break;
				case 26:
				case 27:
					outValue = ExtendMethod.ToJsonEntity<float[]>(value, (JsonSerializerOptions)null);
					break;
				case 28:
				case 29:
					outValue = ExtendMethod.ToJsonEntity<short[]>(value, (JsonSerializerOptions)null);
					break;
				case 30:
				case 31:
					outValue = ExtendMethod.ToJsonEntity<ushort[]>(value, (JsonSerializerOptions)null);
					break;
				case 32:
				case 33:
					outValue = ExtendMethod.ToJsonEntity<int[]>(value, (JsonSerializerOptions)null);
					break;
				case 34:
				case 35:
					outValue = ExtendMethod.ToJsonEntity<uint[]>(value, (JsonSerializerOptions)null);
					break;
				case 36:
				case 37:
					outValue = ExtendMethod.ToJsonEntity<long[]>(value, (JsonSerializerOptions)null);
					break;
				case 38:
				case 39:
					outValue = ExtendMethod.ToJsonEntity<ulong[]>(value, (JsonSerializerOptions)null);
					break;
				default:
					outValue = value;
					break;
				}
			}
			catch (Exception ex)
			{
				outValue = null;
				message = $"[ {addressDetails.AddressName} ][ {addressDetails.AddressDataType} ][ {value} ]{SafeGetLanguage("类型错误")}";
				LogHelper.Error(message, (string)null, ex, true);
				return (QualityType)2;
			}
			return (QualityType)1;
		}

		/// <summary>
		/// 执行消息中间件生产（在线程池执行异步版本，不阻塞采集线程）。
		/// </summary>
		private static void Produce(AddressDetails addressDetails, object? value)
		{
			AddressMq mqParam = addressDetails.AddressMqParam;
			if (mqParam == null)
			{
				return;
			}
			Task.Run(async delegate
			{
				try
				{
					object sendValue = value ?? string.Empty;
					if (!string.IsNullOrWhiteSpace(mqParam.ContentFormat) && mqParam.ContentFormat.Contains("{0}"))
					{
						sendValue = string.Format(mqParam.ContentFormat, sendValue);
					}
					if (!((BaseModel)(await MqOperateInstance.ProduceAsync(mqParam.Topic, sendValue.ToString() ?? string.Empty, mqParam.ISns).ConfigureAwait(continueOnCapturedContext: false))).Status)
					{
						string topic = mqParam.Topic;
						object content = sendValue;
						List<string> iSns = mqParam.ISns;
						await LogHelper.ErrorAsync(new
						{
							Topic = topic,
							Content = content,
							ISns = ((iSns != null) ? ExtendMethod.ToJson<List<string>>(iSns, false) : null)
						}, "{@log}", "Core/AddressHandler/Produce", (Exception)null, false, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (Exception ex)
				{
					await LogHelper.ErrorAsync("消息中间件生产异常：" + ex.Message, "Core/AddressHandler/Produce", ex, true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
			});
		}

		/// <summary>
		/// 数据解析。
		/// <para>
		/// 当地址配置了解析参数时，通过反射执行外部解析逻辑。
		/// </para>
		/// </summary>
		/// <param name="addressDetails">地址详情</param>
		/// <param name="value">当前值</param>
		/// <returns>元组（数据质量, 解析后值, 消息）</returns>
		private static (QualityType Quality, object? Value, string? Message) Parse(AddressDetails addressDetails, object value)
		{
			AddressParse addressParseParam = addressDetails.AddressParseParam;
			if (addressParseParam == null)
			{
				return (Quality: (QualityType)1, Value: value, Message: string.Empty);
			}
			if (addressParseParam.ReflectionParam == null || addressParseParam.ReflectionParam.Length < 2)
			{
				return (Quality: (QualityType)1, Value: value, Message: string.Empty);
			}
			try
			{
				ReflectionData.Basics basics = addressParseParam.ReflectionParam[0] as ReflectionData.Basics;
				string text = addressParseParam.ReflectionParam[1]?.ToString();
				if (basics == null || text == null)
				{
					string text2 = "使用了\"反射\"解析，但参数错误";
					LogHelper.Error(text2, "Core/AddressHandler/Parse", (Exception)null, true);
					return (Quality: (QualityType)4, Value: null, Message: text2);
				}
				ReflectionOperate reflectionOperate = EnsureReflectionInitialized(basics);
				if (reflectionOperate == null)
				{
					return (Quality: (QualityType)4, Value: null, Message: "反射初始化失败");
				}
				object obj = reflectionOperate.ExecuteMethod(text, new object[2] { addressDetails.AddressName, value });
				if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
				{
					return (Quality: (QualityType)4, Value: null, Message: string.Empty);
				}
				return (Quality: (QualityType)3, Value: obj, Message: string.Empty);
			}
			catch (Exception ex)
			{
				string text3 = SafeGetLanguage("解析异常") + "：" + ex.Message;
				LogHelper.Error(text3, "Core/AddressHandler/Parse", ex, false);
				return (Quality: (QualityType)4, Value: null, Message: text3);
			}
		}

		/// <summary>
		/// 确保反射实例已初始化（线程安全的惰性初始化）。
		/// <para>
		/// 使用 SemaphoreSlim 替代 sync lock，与项目其他代码保持一致。
		/// 由于调用方 Parse 是同步方法，此处使用同步 Wait() 是不可避免的。
		/// </para>
		/// </summary>
		private static ReflectionOperate? EnsureReflectionInitialized(ReflectionData.Basics basics)
		{
			if (_reflectionContainer.TryGetValue(basics, out ReflectionOperate value))
			{
				return value;
			}
			_parseSemaphore.Wait();
			try
			{
				if (_reflectionContainer.TryGetValue(basics, out value))
				{
					return value;
				}
				try
				{
					ReflectionOperate reflectionOperate = CoreUnify<ReflectionOperate, ReflectionData.Basics>.Instance(basics);
					OperateResult val = reflectionOperate.Init();
					if (!((BaseModel)val).Status)
					{
						LogHelper.Error("反射初始化失败：" + ((BaseModel)val).Message, "Core/AddressHandler/Parse", (Exception)null, true);
						return null;
					}
					_reflectionContainer[basics] = reflectionOperate;
					return reflectionOperate;
				}
				catch (Exception ex)
				{
					LogHelper.Error("反射实例创建异常：" + ex.Message, "Core/AddressHandler/Parse", ex, true);
					return null;
				}
			}
			finally
			{
				_parseSemaphore.Release();
			}
		}

		/// <summary>
		/// 安全地获取国际化语言值。
		/// <para>
		/// <b>修复内容：</b><br />
		/// 原代码在 catch 块中直接调用 <c>LanguageHandler.GetLanguageValue</c>，
		/// 若该方法抛异常（如语言资源缺失），会掩蔽原始异常。<br />
		/// 修复后使用 try-catch 包裹，失败时返回原始 key 作为兜底。
		/// </para>
		/// </summary>
		/// <param name="key">语言 key</param>
		/// <returns>语言值；获取失败时返回 key 本身</returns>
		private static string SafeGetLanguage(string key)
		{
			try
			{
				return key.GetLanguageValue();
			}
			catch
			{
				return key;
			}
		}

		/// <summary>
		/// 释放所有资源（同步版本）。
		/// 推荐使用 <see cref="M:Snet.Core.handler.AddressHandler.ShutdownAsync" /> 异步版本。
		/// </summary>
		public static void Shutdown()
		{
			_parseSemaphore.Wait();
			try
			{
				foreach (KeyValuePair<ReflectionData.Basics, ReflectionOperate> item in _reflectionContainer)
				{
					try
					{
						item.Value.Dispose();
					}
					catch
					{
					}
				}
				_reflectionContainer.Clear();
			}
			finally
			{
				_parseSemaphore.Release();
			}
			if (_mqOperateLazy.IsValueCreated)
			{
				try
				{
					MqOperateInstance.Dispose();
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// 异步释放所有资源。
		/// 在应用程序关闭时调用，确保所有 IDisposable/IAsyncDisposable 实例被正确释放。
		/// </summary>
		public static async Task ShutdownAsync()
		{
			await _parseSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				foreach (KeyValuePair<ReflectionData.Basics, ReflectionOperate> item in _reflectionContainer)
				{
					try
					{
						await item.Value.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				}
				_reflectionContainer.Clear();
			}
			finally
			{
				_parseSemaphore.Release();
			}
			if (_mqOperateLazy.IsValueCreated)
			{
				try
				{
					await MqOperateInstance.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch
				{
				}
			}
		}
	}
	/// <summary>
	/// 字节处理
	/// </summary>
	public class BytesHandler : CoreUnify<BytesHandler, string>
	{
		/// <summary>
		/// 无参构造函数
		/// </summary>
		public BytesHandler()
		{
		}

		/// <summary>
		/// 带参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public BytesHandler(string basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 加入解包结果字典。同名不同型的点（组包层去重键含类型，同批可共存，如 Modbus 90.1 Bool+Int）
		/// key 冲突时加类型后缀区分，两个值都不丢；正常场景 key 不变（=地址名）。
		/// </summary>
		private static void AddResult(ConcurrentDictionary<string, AddressValue> dict, BytesModel param, AddressValue value)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (!dict.TryAdd(param.Address, value))
			{
				dict.TryAdd(param.Address + "\u0001" + ((object)param.DataType/*cast due to constrained. prefix*/).ToString(), value);
			}
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="source">源</param>
		/// <param name="time">时间</param>
		/// <param name="param">参数</param>
		/// <param name="msg">消息</param>
		/// <param name="quality">质量</param>
		/// <returns>返回标准的 <see cref="T:Snet.Model.data.AddressValue" /></returns>
		private AddressValue SettingsValue(object? value, byte[] source, DateTime time, BytesModel param, string? msg = null, QualityType quality = (QualityType)1)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return new AddressValue().SET(value, source, time, param, msg ?? "成功".GetLanguageValue(), quality);
		}

		/// <summary>
		/// 转换（同步）<br />
		/// 只允许对值类型进行转换
		/// </summary>
		/// <param name="addressValue">地址获取的值</param>
		/// <summary>
		/// 转换（同步）<br />
		/// 只允许对值类型进行转换
		/// </summary>
		/// <param name="addressValue">地址获取的值</param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（每 2 字节一组交换；连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <returns>返回对应的键值数据</returns>
		public OperateResult Transform(AddressValue addressValue, bool isStringReverseByteWord = false)
		{
			object addressExtendParam = ((AddressDetails)addressValue).AddressExtendParam;
			List<BytesModel> list = ((addressExtendParam != null) ? ExtendMethod.GetSource<List<BytesModel>>(addressExtendParam) : null);
			if (list != null)
			{
				DateTime time = addressValue.Time;
				object originalValue = addressValue.OriginalValue;
				byte[] array = ((originalValue != null) ? ExtendMethod.GetSource<byte[]>(originalValue) : null);
				if (array != null && array.Length != 0)
				{
					return Transform(array, time, list, isStringReverseByteWord);
				}
				return EndOperate(status: false, "原始值为空，无法执行数据处理", null, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", BegOperate("Transform"), 76);
			}
			return EndOperate(status: false, "扩展参数为空，无法执行数据处理", null, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", BegOperate("Transform"), 81);
		}

		/// <summary>
		/// 转换（同步）<br />
		/// 只允许对值类型进行转换
		/// </summary>
		/// <param name="addressValue">地址获取的值</param>
		/// <param name="token">取消通知</param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（每 2 字节一组交换；连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <returns>返回对应的键值数据</returns>
		public async Task<OperateResult> TransformAsync(AddressValue addressValue, CancellationToken token = default(CancellationToken), bool isStringReverseByteWord = false)
		{
			object addressExtendParam = ((AddressDetails)addressValue).AddressExtendParam;
			List<BytesModel> list = ((addressExtendParam != null) ? ExtendMethod.GetSource<List<BytesModel>>(addressExtendParam) : null);
			CancellationToken token2;
			if (list != null)
			{
				DateTime time = addressValue.Time;
				object originalValue = addressValue.OriginalValue;
				byte[] array = ((originalValue != null) ? ExtendMethod.GetSource<byte[]>(originalValue) : null);
				if (array != null && array.Length != 0)
				{
					return await TransformAsync(array, time, list, token, isStringReverseByteWord);
				}
				token2 = token;
				return await EndOperateAsync(status: false, "原始值为空，无法执行数据处理", null, null, logOutput: true, consoleOutput: true, token2, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", await BegOperateAsync(token, "TransformAsync"), 106);
			}
			token2 = token;
			return await EndOperateAsync(status: false, "扩展参数为空，无法执行数据处理", null, null, logOutput: true, consoleOutput: true, token2, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", await BegOperateAsync(token, "TransformAsync"), 111);
		}

		/// <summary>
		/// 转换（同步）<br />
		/// 只允许对值类型进行转换
		/// </summary>
		/// <param name="bytes">字节源</param>
		/// <param name="time">字节源收到的时间</param>
		/// <param name="transformParams">
		/// 转换参数集合<br />
		/// 转换 bytes 中的数据
		/// </param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（每 2 字节一组交换；连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <returns>返回对应的键值数据</returns>
		public OperateResult Transform(byte[] bytes, DateTime time, IEnumerable<BytesModel> transformParams, bool isStringReverseByteWord = false)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Expected I4, but got Unknown
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			BegOperate("Transform");
			BytesTransformHandler bytesTransformHandler = new BytesTransformHandler
			{
				IsStringReverseByteWord = isStringReverseByteWord
			};
			ConcurrentDictionary<string, AddressValue> concurrentDictionary = new ConcurrentDictionary<string, AddressValue>();
			try
			{
				foreach (BytesModel transformParam in transformParams)
				{
					int startBit = transformParam.StartBit;
					int length = transformParam.Length;
					int boolIndex = transformParam.BoolIndex;
					if (startBit + length > bytes.Length)
					{
						AddResult(concurrentDictionary, transformParam, SettingsValue(null, Array.Empty<byte>(), time, transformParam, "字节长度越界".GetLanguageValue(), (QualityType)0));
						continue;
					}
					bytesTransformHandler.DataFormat = transformParam.DataFormat;
					byte[] array = new byte[length];
					Buffer.BlockCopy(bytes, startBit, array, 0, length);
					try
					{
						DataType dataType = transformParam.DataType;
						object obj;
						switch (dataType - 1)
						{
						case 0:
							obj = bytesTransformHandler.TransByte(array, boolIndex);
							break;
						case 1:
							obj = bytesTransformHandler.TransBool(array, boolIndex);
							break;
						case 2:
							obj = bytesTransformHandler.TransDouble(array, 0);
							break;
						case 3:
						case 4:
							obj = bytesTransformHandler.TransSingle(array, 0);
							break;
						case 5:
						case 6:
							obj = bytesTransformHandler.TransInt16(array, 0);
							break;
						case 7:
						case 8:
							obj = bytesTransformHandler.TransUInt16(array, 0);
							break;
						case 9:
						case 10:
							obj = bytesTransformHandler.TransInt32(array, 0);
							break;
						case 11:
						case 12:
							obj = bytesTransformHandler.TransUInt32(array, 0);
							break;
						case 13:
						case 14:
							obj = bytesTransformHandler.TransInt64(array, 0);
							break;
						case 15:
						case 16:
							obj = bytesTransformHandler.TransUInt64(array, 0);
							break;
						case 20:
						case 21:
							obj = bytesTransformHandler.TransString(array, 0, length, transformParam.EncodingType.GetEncoding());
							break;
						default:
							throw new NotSupportedException(((object)transformParam.DataType/*cast due to constrained. prefix*/).ToString() + " 不支持类型转换".GetLanguageValue());
						}
						object value = obj;
						AddResult(concurrentDictionary, transformParam, SettingsValue(value, array, time, transformParam, null, (QualityType)1));
					}
					catch (Exception ex)
					{
						AddResult(concurrentDictionary, transformParam, SettingsValue(null, array, time, transformParam, "转换失败：".GetLanguageValue() + ex.Message, (QualityType)0));
					}
				}
				return EndOperate(status: true, null, concurrentDictionary, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", "Transform", 180);
			}
			catch (Exception ex2)
			{
				return EndOperate(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", "Transform", 184);
			}
		}

		/// <summary>
		/// 转换<br />
		/// 只允许对值类型进行转换
		/// </summary>
		/// <param name="bytes">字节源</param>
		/// <param name="time">字节源收到的时间</param>
		/// <param name="transformParams">
		/// 转换参数集合<br />
		/// 转换 bytes 中的数据
		/// </param>
		/// <param name="token">取消通知</param>
		/// <param name="isStringReverseByteWord">String 解包是否按字反转字节（每 2 字节一组交换；连接级配置，如欧姆龙 FINS；默认 false）</param>
		/// <returns>返回对应的键值数据</returns>
		public async Task<OperateResult> TransformAsync(byte[] bytes, DateTime time, IEnumerable<BytesModel> transformParams, CancellationToken token = default(CancellationToken), bool isStringReverseByteWord = false)
		{
			await BegOperateAsync(token, "TransformAsync");
			BytesTransformHandler transform = new BytesTransformHandler
			{
				IsStringReverseByteWord = isStringReverseByteWord
			};
			ConcurrentDictionary<string, AddressValue> outValue = new ConcurrentDictionary<string, AddressValue>();
			try
			{
				foreach (BytesModel param in transformParams)
				{
					int startBit = param.StartBit;
					int length = param.Length;
					int boolIndex = param.BoolIndex;
					if (startBit + length > bytes.Length)
					{
						ConcurrentDictionary<string, AddressValue> dict = outValue;
						BytesModel param2 = param;
						byte[] source = Array.Empty<byte>();
						DateTime time2 = time;
						BytesModel param3 = param;
						AddResult(dict, param2, SettingsValue(null, source, time2, param3, await "字节长度越界".GetLanguageValueAsync((LanguageModel?)null, token), (QualityType)0));
						continue;
					}
					transform.DataFormat = param.DataFormat;
					byte[] buffer = new byte[length];
					Buffer.BlockCopy(bytes, startBit, buffer, 0, length);
					try
					{
						DataType dataType = param.DataType;
						object obj;
						switch (dataType - 1)
						{
						case 0:
							obj = transform.TransByte(buffer, boolIndex);
							break;
						case 1:
							obj = transform.TransBool(buffer, boolIndex);
							break;
						case 2:
							obj = transform.TransDouble(buffer, 0);
							break;
						case 3:
						case 4:
							obj = transform.TransSingle(buffer, 0);
							break;
						case 5:
						case 6:
							obj = transform.TransInt16(buffer, 0);
							break;
						case 7:
						case 8:
							obj = transform.TransUInt16(buffer, 0);
							break;
						case 9:
						case 10:
							obj = transform.TransInt32(buffer, 0);
							break;
						case 11:
						case 12:
							obj = transform.TransUInt32(buffer, 0);
							break;
						case 13:
						case 14:
							obj = transform.TransInt64(buffer, 0);
							break;
						case 15:
						case 16:
							obj = transform.TransUInt64(buffer, 0);
							break;
						case 20:
						case 21:
							obj = transform.TransString(buffer, 0, length, param.EncodingType.GetEncoding());
							break;
						default:
						{
							string text = ((object)param.DataType/*cast due to constrained. prefix*/).ToString();
							throw new NotSupportedException(text + " " + await "不支持类型转换".GetLanguageValueAsync((LanguageModel?)null, token));
						}
						}
						object value = obj;
						AddResult(outValue, param, SettingsValue(value, buffer, time, param, null, (QualityType)1));
					}
					catch (Exception ex)
					{
						ConcurrentDictionary<string, AddressValue> dict = outValue;
						BytesModel param3 = param;
						byte[] source = buffer;
						DateTime time2 = time;
						BytesModel param2 = param;
						AddResult(dict, param3, SettingsValue(null, source, time2, param2, await "转换失败：".GetLanguageValueAsync((LanguageModel?)null, token) + ex.Message, (QualityType)0));
					}
				}
				return await EndOperateAsync(status: true, null, outValue, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", "TransformAsync", 254);
			}
			catch (TaskCanceledException exception)
			{
				return await EndOperateAsync(status: false, "任务被取消", null, exception, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", "TransformAsync", 258);
			}
			catch (OperationCanceledException exception2)
			{
				return await EndOperateAsync(status: false, "取消通知", null, exception2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", "TransformAsync", 263);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\handler\\BytesHandler.cs", "TransformAsync", 268);
			}
		}
	}
	/// <summary>
	/// 字节数据转换处理器
	/// <para>
	/// 用于在不同数据格式（DataFormat）下，实现字节数组与基础数据类型之间的相互转换。
	/// </para>
	/// <para>
	/// 支持：
	/// 1. 基础数值类型转换（short/int/long/float/double 等）
	/// 2. 字符串转换（支持按字反转）
	/// 3. Bool 位索引转换
	/// 4. 一维数组转二维数组
	/// 5. 不同字节序格式（ABCD/BADC/CDAB/DCBA）
	/// </para>
	/// </summary>
	/// <summary>
	/// BytesTransformHandler 的 "类型→字节数组" 转换方法（partial class）。
	/// 包含所有 TransByte 重载：将基础数据类型和数组序列化为字节数组。
	/// </summary>
	public class BytesTransformHandler
	{
		[CompilerGenerated]
		private DataFormat <DataFormat>k__BackingField;

		/// <summary>
		/// 当前数据格式（字节序格式）
		/// <para>
		/// 决定 2/4/8 字节数据在转换时的字节排列顺序。
		/// </para>
		/// </summary>
		public DataFormat DataFormat
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <DataFormat>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<DataFormat>k__BackingField = value;
			}
		}

		/// <summary>
		/// 是否在字符串转换时进行按“字（Word）”反转字节
		/// <para>
		/// true：每 2 字节为一组进行交换
		/// false：保持原始顺序
		/// </para>
		/// </summary>
		public bool IsStringReverseByteWord { get; set; }

		/// <summary>
		/// 构造函数（默认 DCBA 格式）
		/// </summary>
		public BytesTransformHandler()
		{
			DataFormat = (DataFormat)3;
		}

		/// <summary>
		/// 构造函数（指定数据格式）
		/// </summary>
		/// <param name="dataFormat">指定的字节序格式</param>
		public BytesTransformHandler(DataFormat dataFormat)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			DataFormat = dataFormat;
		}

		/// <summary>
		/// 从字节数组中读取指定位置的布尔值
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">布尔位索引（按位索引，不是字节索引）</param>
		/// <returns>返回对应位置的布尔值</returns>
		public virtual bool TransBool(byte[] buffer, int index)
		{
			return ExtendMethod.GetBoolByIndex(buffer, index);
		}

		/// <summary>
		/// 从字节数组中批量读取布尔值数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始布尔位索引</param>
		/// <param name="length">读取布尔值数量</param>
		/// <returns>布尔值数组</returns>
		public virtual bool[] TransBool(byte[] buffer, int index, int length)
		{
			bool[] array = new bool[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = ExtendMethod.GetBoolByIndex(buffer, i + index);
			}
			return array;
		}

		/// <summary>
		/// 从字节数组中读取单个字节
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">字节索引位置</param>
		/// <returns>对应位置的字节值</returns>
		public virtual byte TransByte(byte[] buffer, int index)
		{
			return buffer[index];
		}

		/// <summary>
		/// 从字节数组中读取指定长度的字节数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取长度</param>
		/// <returns>新的字节数组</returns>
		public virtual byte[] TransByte(byte[] buffer, int index, int length)
		{
			byte[] array = new byte[length];
			Array.Copy(buffer, index, array, 0, length);
			return array;
		}

		/// <summary>
		/// 从字节数组中读取 Int16 类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始索引</param>
		/// <returns>Int16 数值</returns>
		public virtual short TransInt16(byte[] buffer, int index)
		{
			return BitConverter.ToInt16(ByteTransDataFormat2(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 Int16 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始索引</param>
		/// <param name="length">读取数量</param>
		/// <returns>Int16 数组</returns>
		public virtual short[] TransInt16(byte[] buffer, int index, int length)
		{
			short[] array = new short[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransInt16(buffer, index + 2 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 Int16 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 Int16 数组</returns>
		public short[,] TransInt16(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<short>(TransInt16(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取 UInt16 类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引位置</param>
		/// <returns>转换后的 UInt16 数值</returns>
		public virtual ushort TransUInt16(byte[] buffer, int index)
		{
			return BitConverter.ToUInt16(ByteTransDataFormat2(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 UInt16 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取元素数量</param>
		/// <returns>UInt16 数组</returns>
		public virtual ushort[] TransUInt16(byte[] buffer, int index, int length)
		{
			ushort[] array = new ushort[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransUInt16(buffer, index + 2 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 UInt16 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 UInt16 数组</returns>
		public ushort[,] TransUInt16(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<ushort>(TransUInt16(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取 Int32 类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <returns>转换后的 Int32 数值</returns>
		public virtual int TransInt32(byte[] buffer, int index)
		{
			return BitConverter.ToInt32(ByteTransDataFormat4(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 Int32 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取元素数量</param>
		/// <returns>Int32 数组</returns>
		public virtual int[] TransInt32(byte[] buffer, int index, int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransInt32(buffer, index + 4 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 Int32 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 Int32 数组</returns>
		public int[,] TransInt32(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<int>(TransInt32(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取 UInt32 类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <returns>转换后的 UInt32 数值</returns>
		public virtual uint TransUInt32(byte[] buffer, int index)
		{
			return BitConverter.ToUInt32(ByteTransDataFormat4(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 UInt32 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取元素数量</param>
		/// <returns>UInt32 数组</returns>
		public virtual uint[] TransUInt32(byte[] buffer, int index, int length)
		{
			uint[] array = new uint[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransUInt32(buffer, index + 4 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 UInt32 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 UInt32 数组</returns>
		public uint[,] TransUInt32(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<uint>(TransUInt32(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取 Int64 类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <returns>转换后的 Int64 数值</returns>
		public virtual long TransInt64(byte[] buffer, int index)
		{
			return BitConverter.ToInt64(ByteTransDataFormat8(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 Int64 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取元素数量</param>
		/// <returns>Int64 数组</returns>
		public virtual long[] TransInt64(byte[] buffer, int index, int length)
		{
			long[] array = new long[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransInt64(buffer, index + 8 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 Int64 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 Int64 数组</returns>
		public long[,] TransInt64(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<long>(TransInt64(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取 UInt64 类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <returns>转换后的 UInt64 数值</returns>
		public virtual ulong TransUInt64(byte[] buffer, int index)
		{
			return BitConverter.ToUInt64(ByteTransDataFormat8(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 UInt64 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取元素数量</param>
		/// <returns>UInt64 数组</returns>
		public virtual ulong[] TransUInt64(byte[] buffer, int index, int length)
		{
			ulong[] array = new ulong[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransUInt64(buffer, index + 8 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 UInt64 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 UInt64 数组</returns>
		public ulong[,] TransUInt64(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<ulong>(TransUInt64(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取 Single（float）类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <returns>转换后的 float 数值</returns>
		public virtual float TransSingle(byte[] buffer, int index)
		{
			return BitConverter.ToSingle(ByteTransDataFormat4(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 float 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取元素数量</param>
		/// <returns>float 数组</returns>
		public virtual float[] TransSingle(byte[] buffer, int index, int length)
		{
			float[] array = new float[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransSingle(buffer, index + 4 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 float 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 float 数组</returns>
		public float[,] TransSingle(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<float>(TransSingle(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取 Double 类型
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <returns>转换后的 double 数值</returns>
		public virtual double TransDouble(byte[] buffer, int index)
		{
			return BitConverter.ToDouble(ByteTransDataFormat8(buffer, index), 0);
		}

		/// <summary>
		/// 批量读取 double 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取元素数量</param>
		/// <returns>double 数组</returns>
		public virtual double[] TransDouble(byte[] buffer, int index, int length)
		{
			double[] array = new double[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = TransDouble(buffer, index + 8 * i);
			}
			return array;
		}

		/// <summary>
		/// 读取二维 double 数组
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>二维 double 数组</returns>
		public double[,] TransDouble(byte[] buffer, int index, int row, int col)
		{
			return ByteHandler.CreateTwoArrayFromOneArray<double>(TransDouble(buffer, index, row * col), row, col);
		}

		/// <summary>
		/// 从字节数组中读取字符串
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="index">起始字节索引</param>
		/// <param name="length">读取字节长度</param>
		/// <param name="encoding">字符串编码方式（如 UTF8 / ASCII / Unicode）</param>
		/// <returns>转换后的字符串</returns>
		public virtual string TransString(byte[] buffer, int index, int length, Encoding encoding)
		{
			byte[] array = TransByte(buffer, index, length);
			if (IsStringReverseByteWord)
			{
				return encoding.GetString(ByteHandler.BytesReverseByWord(array));
			}
			return encoding.GetString(array);
		}

		/// <summary>
		/// 将整个字节数组转换为字符串
		/// </summary>
		/// <param name="buffer">源字节数组</param>
		/// <param name="encoding">字符串编码方式</param>
		/// <returns>转换后的字符串</returns>
		public virtual string TransString(byte[] buffer, Encoding encoding)
		{
			return encoding.GetString(buffer);
		}

		/// <summary>2字节数据按照 DataFormat 进行字节顺序转换</summary>
		protected virtual byte[] ByteTransDataFormat2(byte[] value, int index = 0)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected I4, but got Unknown
			byte[] array = new byte[2];
			DataFormat dataFormat = DataFormat;
			switch ((int)dataFormat)
			{
			case 0:
			case 2:
				array[0] = value[index + 1];
				array[1] = value[index];
				break;
			case 1:
			case 3:
				array[0] = value[index];
				array[1] = value[index + 1];
				break;
			}
			return array;
		}

		/// <summary>4字节数据按照 DataFormat 进行字节顺序转换</summary>
		protected virtual byte[] ByteTransDataFormat4(byte[] value, int index = 0)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected I4, but got Unknown
			byte[] array = new byte[4];
			DataFormat dataFormat = DataFormat;
			switch ((int)dataFormat)
			{
			case 0:
				array[0] = value[index + 3];
				array[1] = value[index + 2];
				array[2] = value[index + 1];
				array[3] = value[index];
				break;
			case 1:
				array[0] = value[index + 2];
				array[1] = value[index + 3];
				array[2] = value[index];
				array[3] = value[index + 1];
				break;
			case 2:
				array[0] = value[index + 1];
				array[1] = value[index];
				array[2] = value[index + 3];
				array[3] = value[index + 2];
				break;
			case 3:
				array[0] = value[index];
				array[1] = value[index + 1];
				array[2] = value[index + 2];
				array[3] = value[index + 3];
				break;
			}
			return array;
		}

		/// <summary>8字节数据按照 DataFormat 进行字节顺序转换</summary>
		protected virtual byte[] ByteTransDataFormat8(byte[] value, int index = 0)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected I4, but got Unknown
			byte[] array = new byte[8];
			DataFormat dataFormat = DataFormat;
			switch ((int)dataFormat)
			{
			case 0:
			{
				for (int j = 0; j < 8; j++)
				{
					array[j] = value[index + 7 - j];
				}
				break;
			}
			case 1:
				array[0] = value[index + 6];
				array[1] = value[index + 7];
				array[2] = value[index + 4];
				array[3] = value[index + 5];
				array[4] = value[index + 2];
				array[5] = value[index + 3];
				array[6] = value[index];
				array[7] = value[index + 1];
				break;
			case 2:
				array[0] = value[index + 1];
				array[1] = value[index];
				array[2] = value[index + 3];
				array[3] = value[index + 2];
				array[4] = value[index + 5];
				array[5] = value[index + 4];
				array[6] = value[index + 7];
				array[7] = value[index + 6];
				break;
			case 3:
			{
				for (int i = 0; i < 8; i++)
				{
					array[i] = value[index + i];
				}
				break;
			}
			}
			return array;
		}

		/// <summary>
		/// 将单个布尔值转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(bool value)
		{
			return TransByte(new bool[1] { value });
		}

		/// <summary>
		/// 将布尔数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(bool[] values)
		{
			if (values != null)
			{
				return ByteHandler.BoolArrayToByte(values);
			}
			return Array.Empty<byte>();
		}

		/// <summary>
		/// 将单个字节转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(byte value)
		{
			return new byte[1] { value };
		}

		/// <summary>
		/// 将 Int16 转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(short value)
		{
			return TransByte(new short[1] { value });
		}

		/// <summary>
		/// 将 Int16 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(short[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 2];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat2(BitConverter.GetBytes(values[i])).CopyTo(array, 2 * i);
			}
			return array;
		}

		/// <summary>
		/// 将 UInt16 转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(ushort value)
		{
			return TransByte(new ushort[1] { value });
		}

		/// <summary>
		/// 将 UInt16 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(ushort[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 2];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat2(BitConverter.GetBytes(values[i])).CopyTo(array, 2 * i);
			}
			return array;
		}

		/// <summary>
		/// 将 Int32 转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(int value)
		{
			return TransByte(new int[1] { value });
		}

		/// <summary>
		/// 将 Int32 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(int[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 4];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat4(BitConverter.GetBytes(values[i])).CopyTo(array, 4 * i);
			}
			return array;
		}

		/// <summary>
		/// 将 UInt32 转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(uint value)
		{
			return TransByte(new uint[1] { value });
		}

		/// <summary>
		/// 将 UInt32 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(uint[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 4];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat4(BitConverter.GetBytes(values[i])).CopyTo(array, 4 * i);
			}
			return array;
		}

		/// <summary>
		/// 将 Int64 转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(long value)
		{
			return TransByte(new long[1] { value });
		}

		/// <summary>
		/// 将 Int64 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(long[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 8];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat8(BitConverter.GetBytes(values[i])).CopyTo(array, 8 * i);
			}
			return array;
		}

		/// <summary>
		/// 将 UInt64 转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(ulong value)
		{
			return TransByte(new ulong[1] { value });
		}

		/// <summary>
		/// 将 UInt64 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(ulong[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 8];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat8(BitConverter.GetBytes(values[i])).CopyTo(array, 8 * i);
			}
			return array;
		}

		/// <summary>
		/// 将 float 转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(float value)
		{
			return TransByte(new float[1] { value });
		}

		/// <summary>
		/// 将 float 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(float[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 4];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat4(BitConverter.GetBytes(values[i])).CopyTo(array, 4 * i);
			}
			return array;
		}

		public virtual byte[] TransByte(double value)
		{
			return TransByte(new double[1] { value });
		}

		/// <summary>
		/// 将 double 数组转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(double[] values)
		{
			if (values == null)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[values.Length * 8];
			for (int i = 0; i < values.Length; i++)
			{
				ByteTransDataFormat8(BitConverter.GetBytes(values[i])).CopyTo(array, 8 * i);
			}
			return array;
		}

		/// <summary>
		/// 将字符串转换为字节数组
		/// </summary>
		public virtual byte[] TransByte(string value, Encoding encoding)
		{
			if (value == null)
			{
				return Array.Empty<byte>();
			}
			byte[] bytes = encoding.GetBytes(value);
			if (!IsStringReverseByteWord)
			{
				return bytes;
			}
			return ByteHandler.BytesReverseByWord(bytes);
		}

		/// <summary>
		/// 将字符串转换为固定长度字节数组
		/// </summary>
		public virtual byte[] TransByte(string value, int length, Encoding encoding)
		{
			if (value == null)
			{
				return Array.Empty<byte>();
			}
			byte[] bytes = encoding.GetBytes(value);
			if (!IsStringReverseByteWord)
			{
				return ByteHandler.ArrayExpandToLength<byte>(bytes, length);
			}
			return ByteHandler.ArrayExpandToLength<byte>(ByteHandler.BytesReverseByWord(bytes), length);
		}
	}
	/// <summary>
	/// 语言处理
	/// </summary>
	public static class LanguageHandler
	{
		/// <summary>
		/// 包装器异步
		/// </summary>
		private static EventingWrapperAsync<EventLanguageResult> OnLanguageEventWrapperAsync;

		/// <summary>
		/// 语言信息<br />
		/// 默认中文
		/// </summary>
		private static CultureInfo cultureInfo;

		/// <summary>
		/// 资源管理<br />
		/// 存放已经实例的资源<br />
		/// 提升速度,避免每次都要实例化
		/// </summary>
		private static ConcurrentDictionary<string, ResourceManager>? resourceManager;

		/// <summary>
		/// 语言管理<br />
		/// 存放已经实例的语言<br />
		/// 提升速度,避免每次都要实例化
		/// </summary>
		private static ConcurrentDictionary<string, CultureInfo>? languageManager;

		/// <summary>
		/// 内部默认语言模型
		/// </summary>
		/// <returns>语言模型</returns>
		private static LanguageModel internalLanguageModel { get; set; }

		/// <summary>
		/// 语言传递事件
		/// </summary>
		public static event EventHandler<EventLanguageResult> OnLanguageEvent;

		/// <summary>
		/// 语言传递事件异步
		/// </summary>
		public static event EventHandlerAsync<EventLanguageResult> OnLanguageEventAsync
		{
			add
			{
				OnLanguageEventWrapperAsync.AddHandler(value);
			}
			remove
			{
				OnLanguageEventWrapperAsync.RemoveHandler(value);
			}
		}

		/// <summary>
		/// 语言切换传递
		/// </summary>
		/// <param name="sender">自身对象</param>
		/// <param name="e">事件结果</param>
		private static async void OnLanguageEventHandlerAsync(object? sender, EventLanguageResult e)
		{
			OnLanguageEvent?.Invoke(sender, e);
			await OnLanguageEventWrapperAsync.InvokeAsync(sender, e);
		}

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
		public static async Task<string?> GetLanguageValueAsync(this string key, LanguageModel? languageModel = null, CancellationToken token = default(CancellationToken))
		{
			if (languageModel == null)
			{
				languageModel = internalLanguageModel;
			}
			string text = languageModel.Source + "." + languageModel.Dictionary;
			if (LanguageHandler.resourceManager == null)
			{
				LanguageHandler.resourceManager = new ConcurrentDictionary<string, ResourceManager>();
			}
			ResourceManager resourceManager;
			if (LanguageHandler.resourceManager.TryGetValue(text, out ResourceManager value))
			{
				resourceManager = value;
			}
			else
			{
				Assembly assembly = (ExtendMethod.IsNullOrWhiteSpace(languageModel.AssemblyFile) ? Assembly.GetCallingAssembly() : ((!File.Exists(languageModel.AssemblyFile)) ? Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, languageModel.AssemblyFile)) : Assembly.LoadFrom(languageModel.AssemblyFile)));
				resourceManager = new ResourceManager(text, assembly);
				LanguageHandler.resourceManager.TryAdd(text, resourceManager);
			}
			return await Task.FromResult(resourceManager.GetString(key, cultureInfo)).ConfigureAwait(continueOnCapturedContext: false);
		}

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
		public static string? GetLanguageValue(this string key, LanguageModel? languageModel = null)
		{
			return key.GetLanguageValueAsync(languageModel).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 获取当前使用的语言异步
		/// </summary>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>返回语言类型</returns>
		public static async Task<LanguageType> GetLanguageAsync(CancellationToken token = default(CancellationToken))
		{
			return await Task.FromResult<LanguageType>((LanguageType)(!(cultureInfo.TwoLetterISOLanguageName == "zh"))).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 获取当前使用的语言
		/// </summary>
		/// <returns>返回语言类型</returns>
		public static LanguageType GetLanguage()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return GetLanguageAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 设置语言异步
		/// </summary>
		/// <param name="language">语言类型</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>成功与失败</returns>
		public unsafe static async Task<bool> SetLanguageAsync(this LanguageType language, CancellationToken token = default(CancellationToken))
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			EventLanguageResult result;
			try
			{
				if (languageManager == null)
				{
					languageManager = new ConcurrentDictionary<string, CultureInfo>();
				}
				if (languageManager.TryGetValue(((object)(*(LanguageType*)(&language))/*cast due to constrained. prefix*/).ToString(), out CultureInfo value))
				{
					cultureInfo = value;
				}
				else
				{
					cultureInfo = new CultureInfo(((object)(*(LanguageType*)(&language))/*cast due to constrained. prefix*/).ToString());
					languageManager.TryAdd(((object)(*(LanguageType*)(&language))/*cast due to constrained. prefix*/).ToString(), cultureInfo);
				}
				CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
				CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
				Thread.CurrentThread.CurrentCulture = cultureInfo;
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
				CultureInfo.CurrentCulture = cultureInfo;
				CultureInfo.CurrentUICulture = cultureInfo;
				result = EventLanguageResult.CreateSuccessResult(await "语言设置成功".GetLanguageValueAsync((LanguageModel?)null, token), (LanguageType?)language);
			}
			catch (CultureNotFoundException ex)
			{
				result = EventLanguageResult.CreateFailureResult(await "找不到指定语言".GetLanguageValueAsync((LanguageModel?)null, token) + ": " + ex.Message, (LanguageType?)language);
			}
			catch (MissingManifestResourceException ex2)
			{
				result = EventLanguageResult.CreateFailureResult(await "找不到资源文件".GetLanguageValueAsync((LanguageModel?)null, token) + ": " + ex2.Message, (LanguageType?)language);
			}
			catch (Exception ex3)
			{
				result = EventLanguageResult.CreateFailureResult(await "语言设置异常".GetLanguageValueAsync((LanguageModel?)null, token) + ": " + ex3.Message, (LanguageType?)language);
			}
			OnLanguageEventHandlerAsync(null, result);
			return result.Status;
		}

		/// <summary>
		/// 设置语言
		/// </summary>
		/// <param name="language">语言类型</param>
		/// <returns>成功与失败</returns>
		public static bool SetLanguage(this LanguageType language)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return language.SetLanguageAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 根据关键字获取当前语言环境下的提示信息
		/// </summary>
		/// <param name="model">语言模型<br />存放基础数据，指向指定资源</param>
		/// <param name="key">关键字</param>
		/// <returns>对应语言的值</returns>
		public static string? GetLanguageValue<T>(this T model, string key) where T : LanguageModel, new()
		{
			return key.GetLanguageValue((LanguageModel?)(object)model);
		}

		/// <summary>
		/// 根据关键字获取当前语言环境下的提示信息异步
		/// </summary>
		/// <param name="model">语言模型<br />存放基础数据，指向指定资源</param>
		/// <param name="key">关键字</param>
		/// <param name="token">传播应取消操作的通知</param>
		/// <returns>对应语言的值</returns>
		public static async Task<string?> GetLanguageValueAsync<T>(this T model, string key, CancellationToken token = default(CancellationToken)) where T : LanguageModel, new()
		{
			return await key.GetLanguageValueAsync((LanguageModel?)(object)model, token);
		}

		static LanguageHandler()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			cultureInfo = new CultureInfo("zh");
			internalLanguageModel = new LanguageModel("Snet.Core", "Language", "Snet.Core.dll");
		}
	}
	/// <summary>
	/// 自动组包处理类
	/// 功能：将离散地址根据设备规则自动合并为批量读取地址，提高通信效率
	/// </summary>
	public class PackerHandler : CoreUnify<PackerHandler, string>
	{
		/// <summary>
		/// 供应商 ProtocolType 枚举名 → ProtocolFamily 映射表。
		/// 从各供应商 *Data.cs 中提取，将 100+ 个分散的 ProtocolType 枚举值映射到统一的 19 个协议族。
		/// </summary>
		private static readonly ConcurrentDictionary<string, Snet.Core.packer.ProtocolFamily> VendorProtocolMap = new ConcurrentDictionary<string, Snet.Core.packer.ProtocolFamily>(StringComparer.OrdinalIgnoreCase)
		{
			["SiemensS7Net_S200"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensS7Net_S200Smart"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensS7Net_S300"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensS7Net_S400"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensS7Net_S1200"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensS7Net_S1500"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensFetchWriteNet"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensPPI"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensPPIOverTcp"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["SiemensMPI"] = Snet.Core.packer.ProtocolFamily.Siemens,
			["ModbusTcpNet"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["ModbusUdpNet"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["ModbusRtu"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["ModbusRtuOverTcp"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["ModbusAscii"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["ModbusAsciiOverTcp"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["DeltaTcpNet"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["DeltaSerial"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["DeltaSerialOverTcp"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["DeltaSerialAscii"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["DeltaSerialAsciiOverTcp"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["InovanceTcpNet"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["InovanceSerial"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["InovanceSerialOverTcp"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["InovanceEasyNet"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["InovanceComputerLink"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["MegMeetTcpNet"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["MegMeetSerial"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["MegMeetSerialOverTcp"] = Snet.Core.packer.ProtocolFamily.Modbus,
			["MelsecMcNet"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecMcRNet"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecMcUdp"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecMcAsciiNet"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecMcAsciiUdp"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecA1ENet"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecA1EAsciiNet"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecA3CNet"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecA3CNetOverTcp"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecFxSerial"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["MelsecFxSerialOverTcp"] = Snet.Core.packer.ProtocolFamily.Mitsubishi,
			["OmronFinsNet"] = Snet.Core.packer.ProtocolFamily.Omron,
			["OmronFinsUdp"] = Snet.Core.packer.ProtocolFamily.Omron,
			["OmronHostLink"] = Snet.Core.packer.ProtocolFamily.Omron,
			["OmronHostLinkOverTcp"] = Snet.Core.packer.ProtocolFamily.Omron,
			["OmronHostLinkCMode"] = Snet.Core.packer.ProtocolFamily.Omron,
			["OmronHostLinkCModeOverTcp"] = Snet.Core.packer.ProtocolFamily.Omron,
			["FujiSPHNet"] = Snet.Core.packer.ProtocolFamily.Fuji,
			["FujiSPB"] = Snet.Core.packer.ProtocolFamily.Fuji,
			["FujiSPBOverTcp"] = Snet.Core.packer.ProtocolFamily.Fuji,
			["FujiCommandSettingType"] = Snet.Core.packer.ProtocolFamily.Fuji,
			["KeyenceMcNet"] = Snet.Core.packer.ProtocolFamily.Keyence,
			["KeyenceMcAsciiNet"] = Snet.Core.packer.ProtocolFamily.Keyence,
			["YokogawaLinkTcp"] = Snet.Core.packer.ProtocolFamily.Yokogawa,
			["PanasonicMewtocol"] = Snet.Core.packer.ProtocolFamily.Panasonic,
			["PanasonicMewtocolOverTcp"] = Snet.Core.packer.ProtocolFamily.Panasonic,
			["MemobusTcpNet"] = Snet.Core.packer.ProtocolFamily.Yaskawa,
			["MemobusUdpNet"] = Snet.Core.packer.ProtocolFamily.Yaskawa,
			["GeSRTPNet"] = Snet.Core.packer.ProtocolFamily.GE,
			["FatekProgramOverTcp"] = Snet.Core.packer.ProtocolFamily.Fatek,
			["FatekProgram"] = Snet.Core.packer.ProtocolFamily.Fatek,
			["FanucInterfaceNet"] = Snet.Core.packer.ProtocolFamily.Fanuc,
			["LSCnet"] = Snet.Core.packer.ProtocolFamily.LSis,
			["LSCnetOverTcp"] = Snet.Core.packer.ProtocolFamily.LSis,
			["CimonHmiProtocol"] = Snet.Core.packer.ProtocolFamily.Cimon,
			["XinJETcpNet"] = Snet.Core.packer.ProtocolFamily.XinJE,
			["XinJESerial"] = Snet.Core.packer.ProtocolFamily.XinJE,
			["XinJESerialOverTcp"] = Snet.Core.packer.ProtocolFamily.XinJE,
			["XinJEInternalNet"] = Snet.Core.packer.ProtocolFamily.XinJE,
			["VigorSerialOverTcp"] = Snet.Core.packer.ProtocolFamily.Vigor,
			["VigorSerial"] = Snet.Core.packer.ProtocolFamily.Vigor,
			["ToyoPuc"] = Snet.Core.packer.ProtocolFamily.Toyota,
			["AllenBradleySLCNet"] = Snet.Core.packer.ProtocolFamily.AllenBradley,
			["AllenBradleyDF1Serial"] = Snet.Core.packer.ProtocolFamily.AllenBradley,
			["AllenBradleyPcccNet"] = Snet.Core.packer.ProtocolFamily.AllenBradley,
			["MelsecFxLinks"] = Snet.Core.packer.ProtocolFamily.MitsubishiFx,
			["MelsecFxLinksOverTcp"] = Snet.Core.packer.ProtocolFamily.MitsubishiFx,
			["BeckhoffAdsNet"] = Snet.Core.packer.ProtocolFamily.Beckhoff
		};

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public PackerHandler()
		{
		}

		/// <summary>
		/// 带基础参数构造函数
		/// </summary>
		/// <param name="basics">基础连接参数字符串</param>
		public PackerHandler(string basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 获取支持自动组包的协议族列表
		/// </summary>
		public static string[] GetSupportAutoPackDeviceTypes()
		{
			return (from f in PackerFactory.GetRegisteredFamilies()
				select f.ToString()).ToArray();
		}

		/// <summary>
		/// 获取支持完整自动组包的协议族列表
		/// </summary>
		public static Snet.Core.packer.ProtocolFamily[] GetFullAutoPackFamilies()
		{
			return (from f in PackerFactory.GetRegisteredFamilies()
				where PackerFactory.GetPacker(f)?.SupportsAutoPack ?? false
				select f).ToArray();
		}

		/// <summary>
		/// 判断指定供应商 ProtocolType 枚举名是否支持组包
		/// </summary>
		/// <param name="vendorProtocolTypeName">供应商 ProtocolType 枚举名，如 "ModbusTcpNet"、"SiemensS7Net_S1200"</param>
		/// <returns>true 表示支持组包</returns>
		public static bool CanAutoPack(string vendorProtocolTypeName)
		{
			Snet.Core.packer.ProtocolFamily result;
			if (!VendorProtocolMap.ContainsKey(vendorProtocolTypeName))
			{
				return Enum.TryParse<Snet.Core.packer.ProtocolFamily>(vendorProtocolTypeName, ignoreCase: true, out result);
			}
			return true;
		}

		/// <summary>
		/// 地址自动组包（基于 ProtocolFamily 枚举）
		/// </summary>
		/// <param name="maxByteLength">单次批量读取最大字节数。0 = 自动使用协议上限</param>
		public Address? AddressAutoPack(Address address, Snet.Core.packer.ProtocolFamily family, int maxByteLength = 0, DataFormat format = (DataFormat)0)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return PackerFactory.GetPacker(family)?.Pack(address, maxByteLength, format);
		}

		/// <summary>
		/// 地址自动组包（基于字符串，向后兼容）
		/// </summary>
		/// <param name="maxByteLength">0 = 自动使用协议上限</param>
		public Address? AddressAutoPack(Address address, string deviceType = "Siemens", int maxByteLength = 0, DataFormat format = (DataFormat)0)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return PackerFactory.GetPacker(deviceType)?.Pack(address, maxByteLength, format);
		}

		/// <summary>
		/// 地址自动组包（异步，基于 ProtocolFamily 枚举）
		/// </summary>
		/// <param name="address">原始地址集合</param>
		/// <param name="family">协议族枚举值</param>
		/// <param name="maxByteLength">单次批量读取最大字节数。0 = 自动使用协议上限</param>
		/// <param name="format">数据字节序格式</param>
		/// <param name="token">取消令牌。组包为纯 CPU 计算，取消仅在开始前生效</param>
		/// <returns>组包后的地址对象，失败返回 null</returns>
		public Task<Address?> AddressAutoPackAsync(Address address, Snet.Core.packer.ProtocolFamily family, int maxByteLength = 0, DataFormat format = (DataFormat)0, CancellationToken token = default(CancellationToken))
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			token.ThrowIfCancellationRequested();
			return Task.Run(delegate
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				token.ThrowIfCancellationRequested();
				return PackerFactory.GetPacker(family)?.Pack(address, maxByteLength, format);
			}, token);
		}

		/// <summary>
		/// 地址自动组包（异步，基于字符串，向后兼容）
		/// </summary>
		/// <param name="address">原始地址集合</param>
		/// <param name="deviceType">设备类型标识（ProtocolFamily 名称或供应商 ProtocolType 名称）</param>
		/// <param name="maxByteLength">0 = 自动使用协议上限</param>
		/// <param name="format">数据字节序格式</param>
		/// <param name="token">取消令牌。组包为纯 CPU 计算，取消仅在开始前生效</param>
		/// <returns>组包后的地址对象，失败返回 null</returns>
		public Task<Address?> AddressAutoPackAsync(Address address, string deviceType = "Siemens", int maxByteLength = 0, DataFormat format = (DataFormat)0, CancellationToken token = default(CancellationToken))
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			token.ThrowIfCancellationRequested();
			return Task.Run(delegate
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				token.ThrowIfCancellationRequested();
				return PackerFactory.GetPacker(deviceType)?.Pack(address, maxByteLength, format);
			}, token);
		}

		/// <summary>
		/// 智能地址自动组包 — 传入供应商 ProtocolType 枚举名和地址集合，自动判断是否支持组包。
		/// <para>支持组包 → 执行地址合并优化后返回</para>
		/// <para>不支持组包 → 原样返回传入的 address</para>
		/// <para>永远不会返回 null</para>
		/// </summary>
		/// <param name="address">原始地址集合</param>
		/// <param name="vendorProtocolTypeName">
		/// 供应商 ProtocolType 枚举的字符串名，例如：
		/// "SiemensS7Net_S1200"、"ModbusTcpNet"、"OmronFinsNet" 等。
		/// 支持各供应商数据类中 <c>ProtocolType.ToString()</c> 的返回值。
		/// </param>
		/// <param name="maxByteLength">单次批量读取最大字节数。0 = 自动使用协议上限（推荐）</param>
		/// <param name="format">数据字节序格式</param>
		/// <returns>
		/// 支持组包 → 优化后的 Address；不支持 → 原始 address。
		/// 传入 null 返回 null。
		/// </returns>
		/// <example>
		/// <code>
		/// // 用法：直接在业务代码中使用 ProtocolType.ToString() 传进来
		/// var result = handler.AddressAutoPackOrPassthrough(
		///     address,
		///     basics.ProtocolType.ToString(),  // 如 "ModbusTcpNet"
		///     400);
		/// </code>
		/// </example>
		public Address? AddressAutoPackOrPassthrough(Address? address, string vendorProtocolTypeName, int maxByteLength = 0, DataFormat format = (DataFormat)0)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			if (address == null)
			{
				return null;
			}
			if (VendorProtocolMap.TryGetValue(vendorProtocolTypeName, out var value))
			{
				IPackerCore packer = PackerFactory.GetPacker(value);
				if (packer == null)
				{
					return address;
				}
				return packer.Pack(address, maxByteLength, format);
			}
			if (Enum.TryParse<Snet.Core.packer.ProtocolFamily>(vendorProtocolTypeName, ignoreCase: true, out value))
			{
				IPackerCore packer2 = PackerFactory.GetPacker(value);
				if (packer2 == null)
				{
					return address;
				}
				return packer2.Pack(address, maxByteLength, format);
			}
			return address;
		}

		/// <summary>
		/// 智能地址自动组包（异步）— 传入供应商 ProtocolType 枚举名和地址集合，自动判断是否支持组包。
		/// <para>支持组包 → 执行地址合并优化后返回</para>
		/// <para>不支持组包 → 原样返回传入的 address</para>
		/// <para>传入 null 返回 null</para>
		/// </summary>
		/// <param name="address">原始地址集合</param>
		/// <param name="vendorProtocolTypeName">供应商 ProtocolType 枚举的字符串名，如 "SiemensS7Net_S1200"、"ModbusTcpNet"</param>
		/// <param name="maxByteLength">单次批量读取最大字节数。0 = 自动使用协议上限（推荐）</param>
		/// <param name="format">数据字节序格式</param>
		/// <param name="token">取消令牌。组包为纯 CPU 计算，取消仅在开始前生效</param>
		/// <returns>支持组包 → 优化后的 Address；不支持 → 原始 address</returns>
		public Task<Address?> AddressAutoPackOrPassthroughAsync(Address? address, string vendorProtocolTypeName, int maxByteLength = 0, DataFormat format = (DataFormat)0, CancellationToken token = default(CancellationToken))
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (address == null)
			{
				return Task.FromResult<Address>(null);
			}
			token.ThrowIfCancellationRequested();
			return Task.Run((Func<Address>)delegate
			{
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				token.ThrowIfCancellationRequested();
				if (VendorProtocolMap.TryGetValue(vendorProtocolTypeName, out var value))
				{
					IPackerCore packer = PackerFactory.GetPacker(value);
					if (packer == null)
					{
						return address;
					}
					return packer.Pack(address, maxByteLength, format);
				}
				if (Enum.TryParse<Snet.Core.packer.ProtocolFamily>(vendorProtocolTypeName, ignoreCase: true, out value))
				{
					IPackerCore packer2 = PackerFactory.GetPacker(value);
					if (packer2 == null)
					{
						return address;
					}
					return packer2.Pack(address, maxByteLength, format);
				}
				return address;
			}, token);
		}
	}
	/// <summary>
	/// 参数处理
	/// </summary>
	public class ParamHandler
	{
		/// <summary>
		/// 获取参数详情
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">类型对象</param>
		/// <param name="name">名称</param>
		/// <param name="description">描述</param>
		/// <param name="properties">附加属性项集合；如果存在　AutoAllocatingTagAttribute　每个之类中都会添加　附加属性项集合</param>
		/// <returns>统一返回</returns>
		public static OperateResult Get<T>(T obj, string name, string description, List<propertie>? properties = null)
		{
			//IL_0d46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4d: Expected O, but got Unknown
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Expected O, but got Unknown
			//IL_07fb: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Expected O, but got Unknown
			//IL_0d27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2e: Expected O, but got Unknown
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Expected O, but got Unknown
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0904: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Expected O, but got Unknown
			//IL_0918: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0940: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0962: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_097b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Expected O, but got Unknown
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e5: Invalid comparison between Unknown and I4
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ea: Invalid comparison between Unknown and I4
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cab: Expected O, but got Unknown
			//IL_0cb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Expected O, but got Unknown
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Expected O, but got Unknown
			//IL_0411: Expected O, but got Unknown
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Invalid comparison between Unknown and I4
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Invalid comparison between Unknown and I4
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Expected O, but got Unknown
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_0719: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Expected O, but got Unknown
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Expected O, but got Unknown
			TimeHandler val = TimeHandler.Instance(ExtendMethod.ToUpperNString(Guid.NewGuid()));
			val.StartRecord();
			try
			{
				List<LibInstanceParam> classAllPropertyData = ReflexHandler.GetClassAllPropertyData<T>();
				Tuple<Type, string, LibInstanceParam> tuple = classAllPropertyData.Select(delegate(LibInstanceParam c)
				{
					AutoAllocatingTagAttribute customAttribute = ((MemberInfo)typeof(T).GetProperty(c.Name)).GetCustomAttribute<AutoAllocatingTagAttribute>();
					return (customAttribute != null) ? new Tuple<Type, string, LibInstanceParam>(customAttribute.EnumType, c.Name, c) : null;
				}).FirstOrDefault((Tuple<Type, string, LibInstanceParam> c) => c != null);
				if (tuple != null)
				{
					ParamModel val2 = new ParamModel
					{
						Name = name,
						Description = description,
						Subset = new List<subset>()
					};
					Array array = Enum.GetValues(tuple.Item1);
					int i;
					for (i = 0; i < array.Length; i++)
					{
						object[] array2 = tuple.Item1.GetField(array.GetValue(i).ToString())?.GetCustomAttributes(typeof(AutoAllocatingAttribute), inherit: false);
						object obj2;
						if (array2 == null || array2.Length <= 0)
						{
							obj2 = null;
						}
						else
						{
							object obj3 = array2[0];
							obj2 = ((obj3 is AutoAllocatingAttribute) ? obj3 : null);
						}
						AutoAllocatingAttribute val3 = (AutoAllocatingAttribute)obj2;
						Tuple<string, string, object> tuple2 = (tuple.Item3.EnumArray as List<object>)?.Select((dynamic c) => (c.Name == array.GetValue(i).ToString()) ? new Tuple<string, string, object>(c.Name, c.Describe, c.Value) : null).FirstOrDefault((Tuple<string, string, object> c) => c != null);
						if (tuple2 == null || val3 == null)
						{
							continue;
						}
						val2.Subset.Add(new subset
						{
							Name = tuple2.Item1,
							Description = tuple2.Item2,
							Propertie = new List<propertie>()
						});
						if (properties != null && properties.Count > 0)
						{
							val2.Subset[i].Propertie.AddRange(properties);
						}
						val2.Subset[i].Propertie.Add(new propertie
						{
							PropertyName = tuple.Item2,
							Description = tuple.Item3.Describe,
							Default = tuple2.Item3,
							Show = false,
							Use = false,
							MustFillIn = false,
							DataCate = null,
							DetailsTips = null,
							Pattern = null,
							FailTips = null
						});
						string[] propertyNameArray = val3.PropertyNameArray;
						foreach (string item in propertyNameArray)
						{
							LibInstanceParam val4 = classAllPropertyData.FirstOrDefault((LibInstanceParam c) => c.Name == item);
							if (val4 == null)
							{
								continue;
							}
							string modelValue = ReflexHandler.GetModelValue(val4.Name, (object)obj);
							DisplayAttribute val5 = typeof(T).GetProperty(val4.Name)?.GetCustomAttribute<DisplayAttribute>();
							VerifyAttribute val6 = typeof(T).GetProperty(val4.Name)?.GetCustomAttribute<VerifyAttribute>();
							UnitAttribute val7 = typeof(T).GetProperty(val4.Name)?.GetCustomAttribute<UnitAttribute>();
							string text = val4.Describe;
							if (val7 != null && !string.IsNullOrWhiteSpace(val7.Unit))
							{
								text = text + "(" + val7.Unit + ")";
							}
							propertie val8 = new propertie
							{
								PropertyName = val4.Name,
								Description = text,
								Show = (val5 != null && val5.Show),
								Use = (val5 != null && val5.Use),
								MustFillIn = (val5 != null && val5.MustFillIn),
								DataCate = ((val5 != null) ? new dataCate?(val5.DataCate) : ((dataCate?)null)),
								DetailsTips = (((val5 != null) ? val5.DetailsTips : null) ?? null),
								Pattern = (((val6 != null) ? val6.Pattern : null) ?? null),
								FailTips = (((val6 != null) ? val6.FailTips : null) ?? null)
							};
							dataCate? val9 = ((val5 != null) ? new dataCate?(val5.DataCate) : ((dataCate?)null));
							if (val9.HasValue)
							{
								dataCate valueOrDefault = val9.GetValueOrDefault();
								if ((int)valueOrDefault != 1)
								{
									if ((int)valueOrDefault == 2)
									{
										val8.Options = new List<options>();
										val8.Options.Add(new options
										{
											Key = "是",
											Value = true
										});
										val8.Options.Add(new options
										{
											Key = "否",
											Value = false
										});
									}
								}
								else
								{
									val8.Options = new List<options>();
									foreach (dynamic item2 in val4.EnumArray as List<object>)
									{
										string text2 = item2.Describe;
										if (!string.IsNullOrEmpty(text2))
										{
											text2 = $"({(object?)item2.Describe})";
										}
										List<options> options = val8.Options;
										options val10 = new options();
										val10.Key = item2.Name + text2;
										val10.Value = (object)item2.Value;
										options.Add(val10);
									}
								}
							}
							val8.Default = modelValue;
							val2.Subset[i].Propertie.Add(val8);
						}
					}
					return new OperateResult(true, ExtendMethod.ToJson<ParamModel>(val2, true), val.StopRecord().Item4, (object)val2);
				}
				ParamModel val11 = new ParamModel
				{
					Name = name,
					Description = description,
					Subset = new List<subset>
					{
						new subset
						{
							Description = description,
							Name = name,
							Propertie = new List<propertie>()
						}
					}
				};
				if (properties != null && properties.Count > 0)
				{
					val11.Subset[0].Propertie.AddRange(properties);
				}
				foreach (LibInstanceParam item3 in classAllPropertyData)
				{
					string modelValue2 = ReflexHandler.GetModelValue(item3.Name, (object)obj);
					DisplayAttribute val12 = typeof(T).GetProperty(item3.Name)?.GetCustomAttribute<DisplayAttribute>();
					VerifyAttribute val13 = typeof(T).GetProperty(item3.Name)?.GetCustomAttribute<VerifyAttribute>();
					UnitAttribute val14 = typeof(T).GetProperty(item3.Name)?.GetCustomAttribute<UnitAttribute>();
					string text3 = item3.Describe;
					if (val14 != null && !string.IsNullOrWhiteSpace(val14.Unit))
					{
						text3 = text3 + "(" + val14.Unit + ")";
					}
					propertie val15 = new propertie
					{
						PropertyName = item3.Name,
						Description = text3,
						Show = (val12 != null && val12.Show),
						Use = (val12 != null && val12.Use),
						MustFillIn = (val12 != null && val12.MustFillIn),
						DataCate = ((val12 != null) ? new dataCate?(val12.DataCate) : ((dataCate?)null)),
						DetailsTips = (((val12 != null) ? val12.DetailsTips : null) ?? null),
						Pattern = (((val13 != null) ? val13.Pattern : null) ?? null),
						FailTips = (((val13 != null) ? val13.FailTips : null) ?? null)
					};
					dataCate? val9 = ((val12 != null) ? new dataCate?(val12.DataCate) : ((dataCate?)null));
					if (val9.HasValue)
					{
						dataCate valueOrDefault = val9.GetValueOrDefault();
						if ((int)valueOrDefault != 1)
						{
							if ((int)valueOrDefault == 2)
							{
								val15.Options = new List<options>();
								val15.Options.Add(new options
								{
									Key = "是",
									Value = true
								});
								val15.Options.Add(new options
								{
									Key = "否",
									Value = false
								});
							}
						}
						else
						{
							val15.Options = new List<options>();
							foreach (dynamic item4 in item3.EnumArray as List<object>)
							{
								string text4 = item4.Describe;
								if (!string.IsNullOrEmpty(text4))
								{
									text4 = $"({(object?)item4.Describe})";
								}
								List<options> options2 = val15.Options;
								options val10 = new options();
								val10.Key = item4.Name + text4;
								val10.Value = (object)item4.Value;
								options2.Add(val10);
							}
						}
					}
					val15.Default = modelValue2;
					val11.Subset[0].Propertie.Add(val15);
				}
				return new OperateResult(true, ExtendMethod.ToJson<ParamModel>(val11, true), val.StopRecord().Item4, (object)val11);
			}
			catch (Exception ex)
			{
				return new OperateResult(false, ex.Message, val.StopRecord().Item4, (object)null);
			}
		}
	}
}
namespace Snet.Core.extend
{
	/// <summary>
	/// 核心扩展方法集；<br />
	/// 提供地址校验、数据精简化、字节转换、编码格式、消息标准化等工具方法
	/// </summary>
	public static class CoreExtend
	{
		private static readonly Regex BracketRegex = new Regex("\\[\\s*[^\\[\\]]+\\s*\\]\\s*\\((?>[^()]+|\\((?<Depth>)|\\)(?<-Depth>))*(?(Depth)(?!))\\)", RegexOptions.Compiled);

		private static readonly Regex ParenthesisRegex = new Regex("\\((?>[^()]+|\\((?<Depth>)|\\)(?<-Depth>))*(?(Depth)(?!))\\)", RegexOptions.Compiled);

		/// <summary>
		/// 检查地址是否存在无效数据
		/// </summary>
		/// <param name="address">地址集合</param>
		/// <returns>
		/// true:一切正常<br />
		/// false:存在无效数据
		/// </returns>
		public static bool CheckAddress(this Address address)
		{
			if (address.AddressArray != null && address.AddressArray.Count > 0 && !address.AddressArray.Any((AddressDetails c) => string.IsNullOrWhiteSpace(c.AddressName)))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 获取精简返回值集合
		/// </summary>
		/// <param name="param">键值</param>
		/// <returns>精简地址集合</returns>
		public static IEnumerable<AddressValueSimplify>? GetSimplifyArray(this ConcurrentDictionary<string, AddressValue> param)
		{
			if (param.Count == 0)
			{
				return null;
			}
			return param.Values.Select((AddressValue c) => c.GetSimplify());
		}

		/// <summary>
		/// 获取精简 Protobuf 数据对象
		/// </summary>
		/// <param name="param">键值</param>
		/// <returns>精简 Protobuf 数据对象</returns>
		public static AddressValueSimplifyProtobuf? GetSimplifyProtobuf(this ConcurrentDictionary<string, AddressValue> param)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			if (param.Count == 0)
			{
				return null;
			}
			return new AddressValueSimplifyProtobuf((IEnumerable<AddressValueSimplify>)param.Values.Select((AddressValue c) => c.GetSimplify()).ToList());
		}

		/// <summary>
		/// 查询精简数据集合；<br />
		/// 从传入的字典中检索指定的地址信息
		/// </summary>
		/// <param name="param">参数</param>
		/// <param name="addressNames">地址名称集合</param>
		/// <returns>指定名称的数据对象</returns>
		public static IEnumerable<AddressValueSimplify>? SelectSimplifyArray(this ConcurrentDictionary<string, AddressValue> param, List<string> addressNames)
		{
			if (param.Count == 0)
			{
				return null;
			}
			return addressNames.Where((string c) => param.TryGetValue(c, out AddressValue _)).Select(delegate(string c)
			{
				param.TryGetValue(c, out AddressValue value);
				return value.GetSimplify();
			});
		}

		/// <summary>
		/// 查询精简  Protobuf  数据；<br />
		/// 从传入的字典中检索指定的地址信息
		/// </summary>
		/// <param name="param">参数</param>
		/// <param name="addressNames">地址名称集合</param>
		/// <returns>指定名称的数据对象</returns>
		public static AddressValueSimplifyProtobuf? SelectSimplifyProtobuf(this ConcurrentDictionary<string, AddressValue> param, List<string> addressNames)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			if (param.Count == 0)
			{
				return null;
			}
			return new AddressValueSimplifyProtobuf(addressNames.Where((string c) => param.TryGetValue(c, out AddressValue _)).Select(delegate(string c)
			{
				param.TryGetValue(c, out AddressValue value);
				return value.GetSimplify();
			}));
		}

		/// <summary>
		/// 获取返回值集合
		/// </summary>
		/// <param name="param">键值</param>
		/// <returns>精简地址集合</returns>
		public static IEnumerable<AddressValue>? GetArray(this ConcurrentDictionary<string, AddressValue> param)
		{
			if (param.Count == 0)
			{
				return null;
			}
			return param.Values.ToArray();
		}

		/// <summary>
		/// 查询数据集合；<br />
		/// 从传入的字典中检索指定的地址信息
		/// </summary>
		/// <param name="param">参数</param>
		/// <param name="addressNames">地址名称集合</param>
		/// <returns>精简地址集合</returns>
		public static IEnumerable<AddressValue>? SelectArray(this ConcurrentDictionary<string, AddressValue> param, List<string> addressNames)
		{
			if (param.Count == 0)
			{
				return null;
			}
			return addressNames.Where((string c) => param.TryGetValue(c, out AddressValue _)).Select(delegate(string c)
			{
				param.TryGetValue(c, out AddressValue value);
				return value;
			});
		}

		/// <summary>
		/// 写入获取字节；<br />
		/// 支持类型:Int16/UInt16/Int32/UInt32/Int64/UInt64/Double/Single
		/// </summary>
		/// <param name="value">值</param>
		/// <returns>字节数据</returns>
		public static byte[] WriteGetBytes(this object value)
		{
			if (!(value is short value2))
			{
				if (!(value is ushort value3))
				{
					if (!(value is int value4))
					{
						if (!(value is uint value5))
						{
							if (!(value is long value6))
							{
								if (!(value is ulong value7))
								{
									if (!(value is double value8))
									{
										if (value is float value9)
										{
											return BitConverter.GetBytes(value9);
										}
										return Array.Empty<byte>();
									}
									return BitConverter.GetBytes(value8);
								}
								return BitConverter.GetBytes(value7);
							}
							return BitConverter.GetBytes(value6);
						}
						return BitConverter.GetBytes(value5);
					}
					return BitConverter.GetBytes(value4);
				}
				return BitConverter.GetBytes(value3);
			}
			return BitConverter.GetBytes(value2);
		}

		/// <summary>
		/// 写入获取字节；<br />
		/// 目前支持:String
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="encoding">编码格式</param>
		/// <returns>字节数据</returns>
		public static byte[] WriteGetBytes(this string value, Encoding encoding)
		{
			return encoding.GetBytes(value);
		}

		/// <summary>
		/// 读取获取对应类型的长度；<br />
		/// 支持类型:Int16/UInt16/Int32/UInt32/Int64/UInt64/Double/Single
		/// </summary>
		/// <param name="dataType">数据类型</param>
		/// <returns>返回对应类型的数据长度</returns>
		public static ushort ReadGetLength(this DataType dataType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected I4, but got Unknown
			switch (dataType - 3)
			{
			case 0:
			case 11:
			case 12:
			case 13:
			case 14:
				return 8;
			case 1:
			case 2:
			case 7:
			case 8:
			case 9:
			case 10:
				return 4;
			case 3:
			case 4:
			case 5:
			case 6:
				return 2;
			default:
				return 0;
			}
		}

		/// <summary>
		/// 获取对应的编码格式
		/// </summary>
		/// <param name="encodingType_AllowEmpty">编码格式_允许为空</param>
		/// <returns>System.Text.Encoding 对应格式的编码类型</returns>
		public static Encoding GetEncoding(this EncodingType? encodingType_AllowEmpty)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected I4, but got Unknown
			if (encodingType_AllowEmpty.HasValue)
			{
				EncodingType valueOrDefault = encodingType_AllowEmpty.GetValueOrDefault();
				if ((int)valueOrDefault != 1201)
				{
					if ((int)valueOrDefault == 12001)
					{
						return new UTF32Encoding(bigEndian: true, byteOrderMark: false);
					}
					return Encoding.GetEncoding((int)encodingType_AllowEmpty.Value);
				}
				return Encoding.BigEndianUnicode;
			}
			return Encoding.UTF8;
		}

		/// <summary>
		/// 获取对应的编码格式
		/// </summary>
		/// <param name="encodingType">编码格式</param>
		/// <returns>System.Text.Encoding 对应格式的编码类型</returns>
		public static Encoding GetEncoding(this EncodingType encodingType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return ((EncodingType?)encodingType).GetEncoding();
		}

		/// <summary>
		/// 获取默认编码的写入
		/// </summary>
		/// <param name="sources">写入的数据点，与数据</param>
		/// <param name="encodingType">编码格式</param>
		/// <returns>带编码的返回,写入的数据点，数据，编码格式</returns>
		public static ConcurrentDictionary<string, (object value, EncodingType? encodingType)> GetDefaultEncodingWrite(this ConcurrentDictionary<string, object> sources, EncodingType? encodingType = (EncodingType?)0)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			ConcurrentDictionary<string, (object, EncodingType?)> concurrentDictionary = new ConcurrentDictionary<string, (object, EncodingType?)>();
			foreach (KeyValuePair<string, object> source in sources)
			{
				concurrentDictionary.TryAdd(source.Key, (source.Value, encodingType.GetValueOrDefault()));
			}
			return concurrentDictionary;
		}

		/// <summary>
		/// 消息标准化处理<br />
		/// - 提取并保留第一个 [TAG] 结构<br />
		/// - 收集所有 () 内容（无论在 [TAG] 内还是独立存在），并拼接到 [TAG] 后面<br />
		/// - 保留原始消息中的冒号右侧部分<br />
		/// - 保证结构稳定、避免重复解析
		/// </summary>
		public static string MessageStandard(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return input;
			}
			int num = input.LastIndexOf(" : ", StringComparison.Ordinal);
			string input2 = ((num > 0) ? input.Substring(0, num) : input);
			string text = ((num > 0) ? input.Substring(num + 3).Trim() : "");
			string text2 = null;
			StringBuilder stringBuilder = new StringBuilder();
			MatchCollection matchCollection = BracketRegex.Matches(input2);
			foreach (Match item in matchCollection)
			{
				int num2 = item.Value.IndexOf(']');
				if (num2 >= 0)
				{
					if (text2 == null)
					{
						text2 = item.Value.Substring(0, num2 + 1);
					}
					stringBuilder.Append(item.Value.AsSpan(num2 + 1));
				}
			}
			if (text2 == null)
			{
				return input;
			}
			foreach (Match pm in ParenthesisRegex.Matches(input2))
			{
				if (!matchCollection.Cast<Match>().Any((Match bm) => pm.Index >= bm.Index && pm.Index < bm.Index + bm.Length))
				{
					stringBuilder.Append(pm.Value);
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder(text2.Length + stringBuilder.Length + text.Length + 10);
			stringBuilder2.Append(text2);
			stringBuilder2.Append(stringBuilder);
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder2.Append(" : ").Append(text);
			}
			return stringBuilder2.ToString();
		}
	}
	/// <summary>
	/// 核心统一类，统一实现单例、事件、统一出入参、函数过程时间记录、详细日志输出、创建实例、获取参数、日志管理、多语言；<br />
	/// O、D都约定为类；
	/// </summary>
	/// <typeparam name="O">操作类</typeparam>
	/// <typeparam name="D">基础数据类，构造参数类</typeparam>
	public class CoreUnify<O, D> : IArgs, IInstance, IClone, IEvent, ILog, ILanguage, IDisposable, IAsyncDisposable where O : class where D : class
	{
		/// <summary>
		/// 最大实例数量
		/// </summary>
		protected static readonly int maxInstanceCount = 255;

		/// <summary>
		/// 单例集合；<br />
		/// Key:基础数据；<br />
		/// Value:操作对象；
		/// </summary>
		protected static readonly ConcurrentDictionary<D, O> objList = new ConcurrentDictionary<D, O>();

		/// <summary>
		/// 锁
		/// </summary>
		private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

		/// <summary>
		/// 数据传递包装器异步
		/// </summary>
		private EventingWrapperAsync<EventDataResult> OnDataEventWrapperAsync;

		/// <summary>
		/// 信息传递包装器异步
		/// </summary>
		private EventingWrapperAsync<EventInfoResult> OnInfoEventWrapperAsync;

		/// <summary>
		/// 信息传递包装器异步
		/// </summary>
		private EventingWrapperAsync<EventLanguageResult> OnLanguageEventWrapperAsync;

		/// <summary>
		/// 缓存的语言事件委托（实例字段），确保 -= 能正确移除同一个委托实例，防止内存泄漏
		/// </summary>
		private EventHandler<EventLanguageResult>? _cachedLanguageEvent;

		/// <summary>
		/// 缓存的异步语言事件委托（实例字段），确保 -= 能正确移除同一个委托实例，防止内存泄漏
		/// </summary>
		private EventHandlerAsync<EventLanguageResult>? _cachedLanguageEventAsync;

		/// <summary>
		/// 获取实例数超过限制的提示信息（动态获取，支持语言切换后实时更新）
		/// </summary>
		private static string exceedMaxInstanceCountTips => string.Format("exceedMaxInstanceCountTips".GetLanguageValue(), maxInstanceCount);

		/// <summary>
		/// 获取创建实例失败的提示信息（动态获取，支持语言切换后实时更新）
		/// </summary>
		private static string createInstanceErrorTips => string.Format("createInstanceErrorTips".GetLanguageValue(), maxInstanceCount);

		/// <summary>
		/// 基础参数
		/// </summary>
		protected D basics { get; set; }

		/// <summary>
		/// 全局标识，用于统一返回
		/// </summary>
		protected string TAG => typeof(O).Name;

		/// <summary>
		/// 中文名称；<br />
		/// 虚属性，可选重写；<br />
		/// 如果无法使用中文就使用英文即可；
		/// </summary>
		protected virtual string CN { get; }

		/// <summary>
		/// 中文描述；<br />
		/// 虚属性，可选重写
		/// </summary>
		protected virtual string CD { get; }

		/// <inheritdoc />
		public virtual LanguageModel LanguageOperate { get; set; }

		/// <inheritdoc />
		public event EventHandler<EventDataResult> OnDataEvent;

		/// <inheritdoc />
		public event EventHandlerAsync<EventDataResult> OnDataEventAsync
		{
			add
			{
				OnDataEventWrapperAsync.AddHandler(value);
			}
			remove
			{
				OnDataEventWrapperAsync.RemoveHandler(value);
			}
		}

		/// <inheritdoc />
		public event EventHandler<EventInfoResult> OnInfoEvent;

		/// <inheritdoc />
		public event EventHandlerAsync<EventInfoResult> OnInfoEventAsync
		{
			add
			{
				OnInfoEventWrapperAsync.AddHandler(value);
			}
			remove
			{
				OnInfoEventWrapperAsync.RemoveHandler(value);
			}
		}

		/// <inheritdoc />
		public event EventHandler<EventLanguageResult> OnLanguageEvent;

		/// <inheritdoc />
		public event EventHandlerAsync<EventLanguageResult> OnLanguageEventAsync
		{
			add
			{
				OnLanguageEventWrapperAsync.AddHandler(value);
			}
			remove
			{
				OnLanguageEventWrapperAsync.RemoveHandler(value);
			}
		}

		/// <summary>
		/// 无参构造函数
		/// </summary>
		protected CoreUnify()
		{
			RegisterLanguageEvent();
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="param">参数</param>
		protected CoreUnify(D param)
		{
			basics = param;
			RegisterLanguageEvent();
		}

		/// <summary>
		/// 释放所有资源<br />
		/// 注意:<br />
		/// 外部重写此方法并且释放完自身资源后<br />
		/// 请在执行如下方法<br />
		/// base.Dispose();
		/// </summary>
		public virtual void Dispose()
		{
			UnregisterLanguageEvent();
			objList?.Remove(basics, out var _);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 异步释放所有资源<br />
		/// 注意:<br />
		/// 外部重写此方法并且释放完自身资源后<br />
		/// 请在执行如下方法<br />
		/// base.DisposeAsync();
		/// </summary>
		public virtual async ValueTask DisposeAsync()
		{
			UnregisterLanguageEvent();
			objList?.Remove(basics, out var _);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 数据源传递
		/// </summary>
		/// <param name="sender">自身对象</param>
		/// <param name="e">事件结果</param>
		protected void OnDataEventHandler(object? sender, EventDataResult e)
		{
			OnDataEvent?.Invoke(sender, e);
			OnDataEventWrapperAsync.InvokeAsync(sender, e);
		}

		/// <summary>
		/// 异步数据源传递
		/// </summary>
		/// <param name="sender">自身对象</param>
		/// <param name="e">事件结果</param>
		protected async Task OnDataEventHandlerAsync(object? sender, EventDataResult e)
		{
			OnDataEvent?.Invoke(sender, e);
			await OnDataEventWrapperAsync.InvokeAsync(sender, e);
		}

		/// <summary>
		/// 消息源传递
		/// </summary>
		/// <param name="sender">自身对象</param>
		/// <param name="e">事件结果</param>
		protected void OnInfoEventHandler(object? sender, EventInfoResult e)
		{
			OnInfoEvent?.Invoke(sender, e);
			OnInfoEventWrapperAsync.InvokeAsync(sender, e);
		}

		/// <summary>
		/// 异步消息源传递
		/// </summary>
		/// <param name="sender">自身对象</param>
		/// <param name="e">事件结果</param>
		protected async Task OnInfoEventHandlerAsync(object? sender, EventInfoResult e)
		{
			OnInfoEvent?.Invoke(sender, e);
			await OnInfoEventWrapperAsync.InvokeAsync(sender, e);
		}

		/// <summary>
		/// 消息源传递
		/// </summary>
		/// <param name="sender">自身对象</param>
		/// <param name="e">事件结果</param>
		protected void OnLanguageEventHandler(object? sender, EventLanguageResult e)
		{
			OnLanguageEvent?.Invoke(sender, e);
			OnLanguageEventWrapperAsync.InvokeAsync(sender, e);
		}

		/// <summary>
		/// 消息源传递
		/// </summary>
		/// <param name="sender">自身对象</param>
		/// <param name="e">事件结果</param>
		protected async Task OnLanguageEventHandlerAsync(object? sender, EventLanguageResult e)
		{
			OnLanguageEvent?.Invoke(sender, e);
			await OnLanguageEventWrapperAsync.InvokeAsync(sender, e);
		}

		/// <summary>
		/// 注册语言事件；<br />
		/// 使用实例字段缓存委托，避免每次创建新闭包导致 -= 无法匹配的内存泄漏
		/// </summary>
		private void RegisterLanguageEvent()
		{
			_cachedLanguageEvent = delegate(object? sender, EventLanguageResult e)
			{
				OnLanguageEvent?.Invoke(sender, e);
			};
			_cachedLanguageEventAsync = async delegate(object? sender, EventLanguageResult e)
			{
				await OnLanguageEventWrapperAsync.InvokeAsync(sender, e);
			};
			LanguageHandler.OnLanguageEvent += _cachedLanguageEvent;
			LanguageHandler.OnLanguageEventAsync += _cachedLanguageEventAsync;
		}

		/// <summary>
		/// 反注册语言事件，释放时移除缓存的委托
		/// </summary>
		private void UnregisterLanguageEvent()
		{
			if (_cachedLanguageEvent != null)
			{
				LanguageHandler.OnLanguageEvent -= _cachedLanguageEvent;
				_cachedLanguageEvent = null;
			}
			if (_cachedLanguageEventAsync != null)
			{
				LanguageHandler.OnLanguageEventAsync -= _cachedLanguageEventAsync;
				_cachedLanguageEventAsync = null;
			}
		}

		/// <summary>
		/// 控制台输出标题<br />
		/// 虚方法，可选重写
		/// </summary>
		/// <returns>控制台输出标题</returns>
		public virtual string Title()
		{
			return "https://Snet.cn";
		}

		/// <summary>
		/// 全局配置文件默认路径<br />
		/// 虚方法，可选重写
		/// </summary>
		/// <returns>返回定义好的路径，供全局使用</returns>
		public virtual string GlobalConfigDefaultPath()
		{
			return AppContext.BaseDirectory + "config";
		}

		/// <summary>
		/// 创建一个实例
		/// </summary>
		/// <param name="param">参数</param>
		/// <returns>对象</returns>
		private static O? CreateInstance(object param)
		{
			ConstructorInfo constructor = typeof(O).GetConstructor(new Type[1] { typeof(D) });
			if (constructor != null)
			{
				return constructor.Invoke(new object[1] { param }) as O;
			}
			return null;
		}

		/// <summary>
		/// 异步单例模式
		/// </summary>
		/// <param name="param">
		/// 基础参数<br />
		/// 不传入则使用默认，直接创建默认对象<br />
		/// 如果这个单例还想使用时，请使用InstanceBasics方法设置基础数据
		/// </param>
		/// <param name="token">传播消息取消通知</param>
		/// <returns>对象</returns>
		public static async Task<O> InstanceAsync(D? param = null, CancellationToken token = default(CancellationToken))
		{
			if (objList.Count >= maxInstanceCount)
			{
				throw new InvalidOperationException(exceedMaxInstanceCountTips);
			}
			if (param == null)
			{
				param = Activator.CreateInstance(typeof(D)) as D;
				if (param == null)
				{
					string name = typeof(D).Name;
					throw new InvalidOperationException(name + " " + await "实例无法创建".GetLanguageValueAsync((LanguageModel?)null, token));
				}
			}
			await semaphore.WaitAsync(token);
			try
			{
				if (objList.Count >= maxInstanceCount)
				{
					throw new InvalidOperationException(exceedMaxInstanceCountTips);
				}
				if (objList.TryGetValue(param, out var value))
				{
					return value;
				}
				O val = null;
				foreach (KeyValuePair<D, O> obj in objList)
				{
					if (ExtendMethod.Comparer<D>(obj.Key, param, (string[])null).Item1)
					{
						val = obj.Value;
						break;
					}
				}
				if (val != null)
				{
					return val;
				}
				O val2 = CreateInstance((object)param);
				if (val2 == null)
				{
					throw new InvalidOperationException(createInstanceErrorTips);
				}
				if (!objList.TryAdd(param, val2))
				{
					throw new InvalidOperationException("单例容器添加失败，键冲突");
				}
				return val2;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new CustomException(ex2.Message, ex2, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "InstanceAsync", 400);
			}
			finally
			{
				semaphore.Release();
			}
		}

		/// <summary>
		/// 单例模式
		/// </summary>
		/// <param name="param">
		/// 基础参数<br />
		/// 不传入则使用默认，直接创建默认对象<br />
		/// 如果这个单例还想使用时，请使用InstanceBasics方法设置基础数据
		/// </param>
		/// <returns>对象</returns>
		public static O Instance(D? param = null)
		{
			return InstanceAsync(param).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步开始操作<br />
		/// 记录函数开始时间
		/// </summary>
		/// <param name="token">传播消息取消通知</param>
		/// <param name="methodName">方法名</param>
		/// <returns>方法名</returns>
		protected async Task<string> BegOperateAsync(CancellationToken token = default(CancellationToken), [CallerMemberName] string methodName = "")
		{
			TimeHandler.Instance(TAG + "." + methodName).StartRecord();
			return await Task.FromResult(methodName).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 开始操作<br />
		/// 记录函数开始时间
		/// </summary>
		/// <param name="methodName">方法名</param>
		/// <returns>方法名</returns>
		protected string BegOperate([CallerMemberName] string methodName = "")
		{
			return BegOperateAsync(CancellationToken.None, methodName).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步结束操作<br />
		/// 记录函数运行结束时间，并返回运行时间
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="resultData">结果数据</param>
		/// <param name="exception">异常信息</param>
		/// <param name="logOutput">
		/// 日志输出<br />
		/// true : 本地路径日志输出<br />
		/// false : 控制台输出也会失效
		/// </param>
		/// <param name="consoleOutput">控制台输出</param>
		/// <param name="token">传播消息取消通知</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名</param>
		/// <param name="lineNumber">行号</param>
		/// <returns>组织好的统一结果</returns>
		protected async Task<OperateResult> EndOperateAsync(bool status, string? message = null, object? resultData = null, Exception? exception = null, bool logOutput = true, bool consoleOutput = true, CancellationToken token = default(CancellationToken), [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			int runTime = TimeHandler.Instance(TAG + "." + methodName).StopRecord().Item4;
			string comma = (((int)(await GetLanguageAsync(token)) == 1) ? "," : "，");
			string tAG = TAG;
			string text;
			if (status)
			{
				text = await GetLanguageValueAsync("成功", null, token);
			}
			else
			{
				string text2 = await GetLanguageValueAsync("异常", null, token);
				object obj = await GetLanguageValueAsync("在", null, token);
				object fileName = Path.GetFileName(filePath);
				object obj2 = await GetLanguageValueAsync("文件", null, token);
				object obj3 = comma;
				object obj4 = await GetLanguageValueAsync("第", null, token);
				object obj5 = lineNumber;
				string text3 = await GetLanguageValueAsync("行", null, token);
				text = text2 + $" < {obj} {fileName} {obj2}{obj3}{obj4} {obj5} {text3} >";
			}
			string msg = "[ " + tAG + " ]( " + methodName + " " + text + " )" + (string.IsNullOrEmpty(message) ? string.Empty : (" : " + message));
			msg = msg.MessageStandard();
			if (!status & logOutput)
			{
				await LogHelper.ErrorAsync(msg, Path.Combine(TAG, methodName ?? ""), exception, consoleOutput, token);
			}
			return new OperateResult(status, msg, runTime, resultData);
		}

		/// <summary>
		/// 结束操作<br />
		/// 记录函数运行结束时间，并返回运行时间
		/// </summary>
		/// <param name="status">状态</param>
		/// <param name="message">消息</param>
		/// <param name="resultData">结果数据</param>
		/// <param name="exception">异常信息</param>
		/// <param name="logOutput">
		/// 日志输出<br />
		/// true : 本地路径日志输出<br />
		/// false : 控制台输出也会失效
		/// </param>
		/// <param name="consoleOutput">控制台输出</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名</param>
		/// <param name="lineNumber">行号</param>
		/// <returns>组织好的统一结果</returns>
		protected OperateResult EndOperate(bool status, string? message = null, object? resultData = null, Exception? exception = null, bool logOutput = true, bool consoleOutput = true, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			return EndOperateAsync(status, message, resultData, exception, logOutput, consoleOutput, CancellationToken.None, filePath, methodName, lineNumber).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步结束操作
		/// </summary>
		/// <param name="result">已经组织到的结果数据</param>
		/// <param name="logOutput">
		/// 日志输出<br />
		/// true : 本地路径日志输出<br />
		/// false : 控制台输出也会失效
		/// </param>
		/// <param name="consoleOutput">控制台输出</param>
		/// <param name="token">传播消息取消通知</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名</param>
		/// <param name="lineNumber">行号</param>
		/// <returns>组织好的统一结果</returns>
		protected async Task<OperateResult> EndOperateAsync(OperateResult result, bool logOutput = true, bool consoleOutput = true, CancellationToken token = default(CancellationToken), [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			int runTime = TimeHandler.Instance(TAG + "." + methodName).StopRecord().Item4;
			string comma = (((int)(await GetLanguageAsync(token)) == 1) ? "," : "，");
			string tAG = TAG;
			string text;
			if (((BaseModel)result).Status)
			{
				text = await GetLanguageValueAsync("成功", null, token);
			}
			else
			{
				string text2 = await GetLanguageValueAsync("异常", null, token);
				object obj = await GetLanguageValueAsync("在", null, token);
				object fileName = Path.GetFileName(filePath);
				object obj2 = await GetLanguageValueAsync("文件", null, token);
				object obj3 = comma;
				object obj4 = await GetLanguageValueAsync("第", null, token);
				object obj5 = lineNumber;
				string text3 = await GetLanguageValueAsync("行", null, token);
				text = text2 + $" < {obj} {fileName} {obj2}{obj3}{obj4} {obj5} {text3} >";
			}
			string input = "[ " + tAG + " ]( " + methodName + " " + text + " )" + (string.IsNullOrEmpty(((BaseModel)result).Message) ? string.Empty : (" : " + ((BaseModel)result).Message));
			input = input.MessageStandard();
			if (!((BaseModel)result).Status & logOutput)
			{
				await LogHelper.ErrorAsync(input, TAG + "/" + methodName, (Exception)null, consoleOutput, token);
			}
			return new OperateResult(result, runTime);
		}

		/// <summary>
		/// 结束操作
		/// </summary>
		/// <param name="result">已经组织到的结果数据</param>
		/// <param name="logOutput">
		/// 日志输出<br />
		/// true : 本地路径日志输出<br />
		/// false : 控制台输出也会失效
		/// </param>
		/// <param name="consoleOutput">控制台输出</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="methodName">方法名</param>
		/// <param name="lineNumber">行号</param>
		/// <returns>组织好的统一结果</returns>
		protected OperateResult EndOperate(OperateResult result, bool logOutput = true, bool consoleOutput = true, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			return EndOperateAsync(result, logOutput, consoleOutput, CancellationToken.None, filePath, methodName, lineNumber).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetArgsAsync(bool getBasicsParam = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetArgsAsync");
			try
			{
				D val = Activator.CreateInstance<D>();
				if (getBasicsParam)
				{
					return await EndOperateAsync(status: true, ExtendMethod.ToJson<D>(val, true), val, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "GetArgsAsync", 563);
				}
				D obj = val;
				string name = (string.IsNullOrWhiteSpace(CN) ? typeof(O).FullName : CN);
				string text = CD;
				if (text == null)
				{
					text = await GetLanguageValueAsync("暂无描述", null, token);
				}
				return await EndOperateAsync(ParamHandler.Get(obj, name, text), logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "GetArgsAsync", 565);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "GetArgsAsync", 569);
			}
		}

		/// <inheritdoc />
		public OperateResult GetArgs(bool getBasicsParam = false)
		{
			return GetArgsAsync(getBasicsParam).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetAutoAllocatingArgsAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetAutoAllocatingArgsAsync");
			try
			{
				string text = default(string);
				object obj = default(object);
				if (((ResultModel)(await ExistsAutoAllocatingArgsAsync())).GetDetails(ref text, ref obj))
				{
					string pName = ((obj == null) ? null : ExtendMethod.GetSource<Tuple<Type, string, LibInstanceParam>>(obj)?.Item2.ToString());
					if (((ResultModel)GetArgs()).GetDetails(ref text, ref obj))
					{
						object obj2;
						if (obj == null)
						{
							obj2 = null;
						}
						else
						{
							ParamModel source = ExtendMethod.GetSource<ParamModel>(obj);
							if (source == null)
							{
								obj2 = null;
							}
							else
							{
								List<subset> subset = source.Subset;
								if (subset == null)
								{
									obj2 = null;
								}
								else
								{
									string text2 = ExtendMethod.ToJson<subset>(subset.FirstOrDefault((subset c) => c.Name == typeof(D)?.GetProperty(pName)?.GetValue(basics)?.ToString()), false);
									obj2 = ((text2 != null) ? ExtendMethod.ToJsonEntity<subsetSimplify>(text2, (JsonSerializerOptions)null) : null);
								}
							}
						}
						subsetSimplify val = (subsetSimplify)obj2;
						return await EndOperateAsync(status: true, (val != null) ? ExtendMethod.ToJson<subsetSimplify>(val, false) : null, val, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "GetAutoAllocatingArgsAsync", 590);
					}
				}
				string text3 = text;
				return await EndOperateAsync(status: false, text3 + "，" + await GetLanguageValueAsync("无法获取自动分配标识的属性值所对应参数集合"), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "GetAutoAllocatingArgsAsync", 593);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "GetAutoAllocatingArgsAsync", 597);
			}
		}

		/// <inheritdoc />
		public OperateResult GetAutoAllocatingArgs()
		{
			return GetAutoAllocatingArgsAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> ExistsAutoAllocatingArgsAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "ExistsAutoAllocatingArgsAsync");
			try
			{
				if (basics != null)
				{
					Tuple<Type, string, LibInstanceParam> tuple = ReflexHandler.GetClassAllPropertyData<D>().Select(delegate(LibInstanceParam c)
					{
						AutoAllocatingTagAttribute customAttribute = ((MemberInfo)typeof(D).GetProperty(c.Name)).GetCustomAttribute<AutoAllocatingTagAttribute>();
						return (customAttribute != null) ? new Tuple<Type, string, LibInstanceParam>(customAttribute.EnumType, c.Name, c) : null;
					}).FirstOrDefault((Tuple<Type, string, LibInstanceParam> c) => c != null);
					if (tuple != null)
					{
						return await EndOperateAsync(status: true, null, tuple, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "ExistsAutoAllocatingArgsAsync", 628);
					}
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("ExistsAutoAllocatingParam_msg1", null, token), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "ExistsAutoAllocatingArgsAsync", 630);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("ExistsAutoAllocatingParam_msg2", null, token), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "ExistsAutoAllocatingArgsAsync", 632);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "ExistsAutoAllocatingArgsAsync", 636);
			}
		}

		/// <inheritdoc />
		public OperateResult ExistsAutoAllocatingArgs()
		{
			return ExistsAutoAllocatingArgsAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> CreateInstanceAsync<T>(T param, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "CreateInstanceAsync");
			try
			{
				if (typeof(T).FullName.Equals(typeof(D).FullName))
				{
					return await EndOperateAsync(status: true, null, await InstanceAsync(param as D), null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "CreateInstanceAsync", 653);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("对象类型错误，无法创建实例", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "CreateInstanceAsync", 657);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "CreateInstanceAsync", 662);
			}
		}

		/// <inheritdoc />
		public OperateResult CreateInstance<T>(T param)
		{
			return CreateInstanceAsync(param).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> CreateInstanceAsync(string json, CancellationToken token = default(CancellationToken))
		{
			D val = ExtendMethod.ToJsonEntity<D>(json, (JsonSerializerOptions)null);
			if (val == null)
			{
				return OperateResult.CreateFailureResult(await GetLanguageValueAsync("JSON反序列化失败，JSON数据对象类型错误，无法创建实例", null, token), (int?)null);
			}
			return await CreateInstanceAsync(val, token);
		}

		/// <inheritdoc />
		public OperateResult CreateInstance(string json)
		{
			return CreateInstanceAsync(json).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> LogOperateSetAsync(bool logOut = true, bool? consoleOut = null, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "LogOperateSetAsync");
			try
			{
				LogModel obj = LogHelper.Get();
				obj.Out = logOut;
				obj.ConsoleOut = consoleOut;
				LogHelper.Set(obj);
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("日志参数设置成功", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 698);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 702);
			}
		}

		/// <inheritdoc />
		public OperateResult LogOperateSet(bool logOut = true, bool? consoleOut = null)
		{
			return LogOperateSetAsync(logOut, consoleOut, CancellationToken.None).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> LogOperateSetAsync(bool logOut = true, bool? consoleOut = null, Action<object, LogEventLevel, string?, Exception?>? notice = null, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "LogOperateSetAsync");
			try
			{
				LogModel obj = LogHelper.Get();
				obj.Notice = notice;
				obj.Out = logOut;
				obj.ConsoleOut = consoleOut;
				LogHelper.Set(obj);
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("日志参数设置成功", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 723);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 727);
			}
		}

		/// <inheritdoc />
		public OperateResult LogOperateSet(bool logOut = true, bool? consoleOut = null, Action<object, LogEventLevel, string?, Exception?>? notice = null)
		{
			return LogOperateSetAsync(logOut, consoleOut, notice, CancellationToken.None).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> LogOperateSetAsync(bool logOut = true, bool? consoleOut = null, Func<object, LogEventLevel, string?, Exception?, Task>? noticeAsync = null, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "LogOperateSetAsync");
			try
			{
				LogModel obj = LogHelper.Get();
				obj.NoticeAsync = noticeAsync;
				obj.Out = logOut;
				obj.ConsoleOut = consoleOut;
				LogHelper.Set(obj);
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("日志参数设置成功", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 748);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 752);
			}
		}

		/// <inheritdoc />
		public OperateResult LogOperateSet(bool logOut = true, bool? consoleOut = null, Func<object, LogEventLevel, string?, Exception?, Task>? noticeAsync = null)
		{
			return LogOperateSetAsync(logOut, consoleOut, noticeAsync).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> LogOperateSetAsync(LogModel logModel, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "LogOperateSetAsync");
			try
			{
				LogHelper.Set(logModel);
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("日志参数设置成功", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 767);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateSetAsync", 771);
			}
		}

		/// <inheritdoc />
		public OperateResult LogOperateSet(LogModel logModel)
		{
			return LogOperateSetAsync(logModel).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> LogOperateGetAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "LogOperateGetAsync");
			try
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("日志参数获取成功", null, token), LogHelper.Get(), null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateGetAsync", 786);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "LogOperateGetAsync", 790);
			}
		}

		/// <inheritdoc />
		public OperateResult LogOperateGet()
		{
			return LogOperateGetAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetBasicsArgsAsync(CancellationToken token = default(CancellationToken))
		{
			if (basics == null)
			{
				return OperateResult.CreateFailureResult(await GetLanguageValueAsync("获取失败，基础数据尚未实例化", null, token), (int?)null);
			}
			return OperateResult.CreateSuccessResult<D>(await GetLanguageValueAsync("获取成功", null, token), basics, (int?)null);
		}

		/// <inheritdoc />
		public OperateResult GetBasicsArgs()
		{
			return GetBasicsArgsAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult CloneThis()
		{
			return CloneThisAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> CloneThisAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "CloneThisAsync");
			if (basics != null)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("克隆成功", null, token), CreateInstance((object)basics), null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "CloneThisAsync", 823);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("基础参数为空，克隆失败", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "CloneThisAsync", 825);
		}

		/// <inheritdoc />
		public OperateResult UpdateArgs<T>(T basics)
		{
			return UpdateArgsAsync(basics).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> UpdateArgsAsync<T>(T basics, CancellationToken token = default(CancellationToken))
		{
			await semaphore.WaitAsync(token);
			await BegOperateAsync(token, "UpdateArgsAsync");
			try
			{
				if (this.basics != null)
				{
					if (typeof(T).FullName.Equals(typeof(D).FullName))
					{
						D key = basics as D;
						if (!objList.TryRemove(this.basics, out var value))
						{
							return await EndOperateAsync(status: false, await GetLanguageValueAsync("对象移除失败", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "UpdateArgsAsync", 848);
						}
						D key2 = this.basics;
						this.basics = key;
						if (!objList.TryAdd(key, value))
						{
							this.basics = key2;
							objList.TryAdd(key2, value);
							return await EndOperateAsync(status: false, await GetLanguageValueAsync("对象更新失败", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "UpdateArgsAsync", 863);
						}
						return await EndOperateAsync(status: false, await GetLanguageValueAsync("参数修改成功", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "UpdateArgsAsync", 865);
					}
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("对象类型错误，无法更新参数", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "UpdateArgsAsync", 869);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("基础数据为空，无法更新参数", null, token), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "UpdateArgsAsync", 874);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\extend\\CoreUnify.cs", "UpdateArgsAsync", 879);
			}
			finally
			{
				semaphore.Release();
			}
		}

		/// <inheritdoc />
		public OperateResult UpdateArgs(string basics)
		{
			return UpdateArgsAsync(basics).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> UpdateArgsAsync(string basics, CancellationToken token = default(CancellationToken))
		{
			D val = ExtendMethod.ToJsonEntity<D>(basics, (JsonSerializerOptions)null);
			return await UpdateArgsAsync(val, token);
		}

		/// <inheritdoc />
		public string? GetLanguageValue(string key, LanguageModel? languageModel = null)
		{
			return key.GetLanguageValue(languageModel);
		}

		/// <inheritdoc />
		public LanguageType GetLanguage()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return LanguageHandler.GetLanguage();
		}

		/// <inheritdoc />
		public bool SetLanguage(LanguageType language)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return language.SetLanguage();
		}

		/// <inheritdoc />
		public async Task<string?> GetLanguageValueAsync(string key, LanguageModel? languageModel = null, CancellationToken token = default(CancellationToken))
		{
			return await key.GetLanguageValueAsync(languageModel, token);
		}

		/// <inheritdoc />
		public async Task<LanguageType> GetLanguageAsync(CancellationToken token = default(CancellationToken))
		{
			return await LanguageHandler.GetLanguageAsync(token);
		}

		/// <inheritdoc />
		public async Task<bool> SetLanguageAsync(LanguageType language, CancellationToken token = default(CancellationToken))
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return await language.SetLanguageAsync(token);
		}
	}
}
namespace Snet.Core.communication.serial
{
	/// <summary>
	/// 串口数据
	/// </summary>
	public class SerialData
	{
		/// <summary>
		/// 串口通信基础数据
		/// </summary>
		public class Basics
		{
			[CompilerGenerated]
			private Parity <ParityBit>k__BackingField;

			[CompilerGenerated]
			private StopBits <StopBit>k__BackingField;

			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; }

			/// <summary>
			/// 串口号
			/// </summary>
			[Description("串口号")]
			public string PortName { get; set; }

			/// <summary>
			/// 波特率
			/// </summary>
			[Description("波特率")]
			public int BaudRate { get; set; }

			/// <summary>
			/// 校验位
			/// </summary>
			[Description("校验位")]
			[JsonConverter(typeof(JsonStringEnumConverter))]
			public Parity ParityBit
			{
				[CompilerGenerated]
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return <ParityBit>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					<ParityBit>k__BackingField = value;
				}
			}

			/// <summary>
			/// 数据位
			/// </summary>
			[Description("数据位")]
			public int DataBit { get; set; }

			/// <summary>
			/// 停止位
			/// </summary>
			[Description("停止位")]
			[JsonConverter(typeof(JsonStringEnumConverter))]
			public StopBits StopBit
			{
				[CompilerGenerated]
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return <StopBit>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					<StopBit>k__BackingField = value;
				}
			}

			/// <summary>
			/// 写入超时时间
			/// </summary>
			[Description("写入超时时间")]
			public int WriteTimeout { get; set; }

			/// <summary>
			/// 读取超时时间
			/// </summary>
			[Description("读取超时时间")]
			public int ReadTimeout { get; set; }

			/// <summary>
			/// 接收缓冲区中数据的字节数阈值
			/// </summary>
			[Description("接收缓冲区中数据的字节数阈值")]
			public int ReceivedBytesThreshold { get; set; }

			/// <summary>
			/// 发送等待间隔<br />
			/// 继承者可选重写
			/// </summary>
			[Description("发送等待间隔")]
			public virtual int SendWaitInterval { get; set; }

			/// <summary>
			/// 最大块大小；<br />
			/// 如果数据超过限定值则自动分包发送<br />
			/// 继承者可选重写
			/// </summary>
			[Description("最大块大小")]
			public virtual int MaxChunkSize { get; set; }

			/// <summary>
			/// 重试发送次数；<br />
			/// 只在分包发送中触发；<br />
			/// 在限定次数中还是失败则直接返回失败<br />
			/// 继承者可选重写
			/// </summary>
			[Description("重试发送次数")]
			public virtual int RetrySendCount { get; set; }

			/// <summary>
			/// 数据缓冲区大小<br />
			/// 继承者可选重写
			/// </summary>
			[Description("数据缓冲区大小")]
			public virtual int BufferSize { get; set; }

			public Basics()
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				SN = ExtendMethod.ToUpperNString(Guid.NewGuid());
				PortName = "COM1";
				BaudRate = 19200;
				ParityBit = (Parity)2;
				DataBit = 8;
				StopBit = (StopBits)1;
				WriteTimeout = 1000;
				ReadTimeout = 1000;
				ReceivedBytesThreshold = 1;
				SendWaitInterval = 5000;
				MaxChunkSize = 261120;
				RetrySendCount = 5;
				BufferSize = 1048576;
				base..ctor();
			}
		}
	}
	/// <summary>
	/// 串口通信；<br />
	/// 提供串口打开、关闭、发送、接收、发送等待响应等功能；<br />
	/// 支持自动分包发送，通过 DataReceived 事件监控接收数据
	/// </summary>
	public class SerialOperate : CommunicationAbstract<SerialOperate, SerialData.Basics>, ICommunication, IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>通信库</summary>
		private SerialPort? _communication;

		public SerialOperate(SerialData.Basics basics)
			: base(basics)
		{
		}

		public static List<string> GetPortArray()
		{
			return SerialPort.GetPortNames().ToList();
		}

		/// <summary>移除 DataReceived 事件监听（调用方需持有 _commLock）</summary>
		private void DetachMonitor()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			if (_communication != null)
			{
				_communication.DataReceived -= new SerialDataReceivedEventHandler(Communication_DataReceived);
			}
		}

		/// <summary>挂载 DataReceived 事件监听（调用方需持有 _commLock）</summary>
		private void AttachMonitor()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			if (_communication != null)
			{
				_communication.DataReceived -= new SerialDataReceivedEventHandler(Communication_DataReceived);
				_communication.DataReceived += new SerialDataReceivedEventHandler(Communication_DataReceived);
			}
		}

		/// <summary>
		/// 接收串口数据（事件处理器 — async void 是此场景的正确用法）。
		/// </summary>
		private async void Communication_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				if (_communication == null || !_communication.IsOpen)
				{
					return;
				}
				int bytesToRead = _communication.BytesToRead;
				if (bytesToRead <= 0)
				{
					return;
				}
				byte[] buffer = new byte[bytesToRead];
				int num = await _communication.BaseStream.ReadAsync(buffer, 0, bytesToRead).ConfigureAwait(continueOnCapturedContext: false);
				if (num > 0)
				{
					byte[] received = ((num == bytesToRead) ? buffer : buffer[..num]);
					object sender2 = this;
					string tAG = base.TAG;
					await OnDataEventHandlerAsync(sender2, new EventDataResult(true, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据").ConfigureAwait(continueOnCapturedContext: false), (object)received)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			catch (Exception ex3)
			{
				object sender2 = this;
				string tAG = base.TAG;
				await OnInfoEventHandlerAsync(sender2, EventInfoResult.CreateFailureResult("[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常").ConfigureAwait(continueOnCapturedContext: false), ex3.Message))).ConfigureAwait(continueOnCapturedContext: false);
				Task.Run(async delegate
				{
					try
					{
						await OffAsync(hardClose: true).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				});
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "OnAsync", 96);
				}
				SerialPort val = new SerialPort
				{
					PortName = base.basics.PortName,
					BaudRate = base.basics.BaudRate,
					Parity = base.basics.ParityBit,
					DataBits = base.basics.DataBit,
					StopBits = base.basics.StopBit,
					WriteTimeout = base.basics.WriteTimeout,
					ReadTimeout = base.basics.ReadTimeout,
					ReceivedBytesThreshold = base.basics.ReceivedBytesThreshold
				};
				val.Open();
				_communication = val;
				AttachMonitor();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "OnAsync", 115).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "OnAsync", 120).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "OffAsync", 134);
				}
				DetachMonitor();
				try
				{
					SerialPort? communication = _communication;
					if (communication != null)
					{
						communication.Close();
					}
				}
				catch
				{
				}
				try
				{
					((Component)(object)_communication)?.Dispose();
				}
				catch
				{
				}
				_communication = null;
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "OffAsync", 144).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "OffAsync", 148).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "SendAsync", 160);
				}
				if (data.Length <= base.basics.MaxChunkSize)
				{
					await _communication.BaseStream.WriteAsync(data, 0, data.Length, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					int maxChunkSize = base.basics.MaxChunkSize;
					int chunkSize;
					for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
					{
						chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
						await _communication.BaseStream.WriteAsync(data, bytesSent, chunkSize, token).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "SendAsync", 182).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "SendAsync", 186).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token)
		{
			await BegOperateAsync(token, "SendWaitAsync").ConfigureAwait(continueOnCapturedContext: false);
			bool monitorWasDetached = false;
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "SendWaitAsync", 200);
				}
				DetachMonitor();
				monitorWasDetached = true;
				if (data.Length <= base.basics.MaxChunkSize)
				{
					await _communication.BaseStream.WriteAsync(data, 0, data.Length, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					int maxChunkSize = base.basics.MaxChunkSize;
					int chunkSize;
					for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
					{
						chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
						await _communication.BaseStream.WriteAsync(data, bytesSent, chunkSize, token).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				byte[] buffer = new byte[base.basics.BufferSize];
				int num;
				bool flag;
				try
				{
					using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
					cts.CancelAfter(base.basics.SendWaitInterval);
					num = await _communication.BaseStream.ReadAsync(buffer, 0, buffer.Length, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
					flag = num > 0;
				}
				catch (OperationCanceledException)
				{
					flag = false;
					num = 0;
				}
				if (flag)
				{
					return await EndOperateAsync(status: true, null, buffer[..num], null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "SendWaitAsync", 244).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("接收数据超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "SendWaitAsync", 247).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "SendWaitAsync", 251).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				if (monitorWasDetached)
				{
					try
					{
						if (_communication != null && _communication.IsOpen)
						{
							AttachMonitor();
						}
					}
					catch (ObjectDisposedException)
					{
					}
				}
			}
		}

		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "GetBaseObjectAsync", 275);
			}
			return await EndOperateAsync(status: true, null, _communication, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "GetBaseObjectAsync", 277).ConfigureAwait(continueOnCapturedContext: false);
		}

		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_communication != null && _communication.IsOpen)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("已连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "GetStatusAsync", 286).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\serial\\SerialOperate.cs", "GetStatusAsync", 290).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.ws.service
{
	public class WsServiceData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 地址
			/// </summary>
			[Description("地址")]
			public string Host { get; set; } = "ws://127.0.0.1:6688/";

			/// <summary>
			/// 最大块大小;
			/// 如果数据超过限定值则自动分包发送
			/// </summary>
			[Description("最大块大小")]
			public int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数;
			/// 只在分包发送中触发;
			/// 在限定次数中还是失败则直接返回失败
			/// </summary>
			[Description("重试发送次数")]
			public int RetrySendCount { get; set; } = 5;

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 数据缓冲区大小
			/// </summary>
			[Description("数据缓冲区大小")]
			public int BufferSize { get; set; } = 1048576;
		}
	}
	/// <summary>
	/// webSocket 服务操作
	/// </summary>
	public class WsServiceOperate : CommunicationServiceAbstract<WsServiceOperate, WsServiceData.Basics>, ICommunicationService, IOn, IOff, ISendService, IObject, IStatus, IRemove, IEvent, IInstance, ILog, ILanguage, IClone, IArgs, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 通信库
		/// </summary>
		private HttpListener? _service;

		/// <summary>
		/// 监控开关
		/// </summary>
		private CancellationTokenSource? _acceptCts;

		/// <summary>
		/// 客户端消息任务容器
		/// </summary>
		private ConcurrentDictionary<string, ClientManage<WebSocketContext>> _clients = new ConcurrentDictionary<string, ClientManage<WebSocketContext>>();

		/// <summary>
		/// 连接监控循环任务引用，用于关闭时等待其退出
		/// </summary>
		private Task? _monitorConnectTask;

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="basics">基础数据</param>
		public WsServiceOperate(WsServiceData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 监控连接
		/// </summary>
		/// <returns></returns>
		private async Task MonitorConnectAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					HttpListenerContext httpListenerContext = await _service.GetContextAsync();
					if (httpListenerContext.Request.IsWebSocketRequest)
					{
						string IpPort = httpListenerContext.Request.RemoteEndPoint.ToString();
						try
						{
							WebSocketContext webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);
							CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
							ClientManage<WebSocketContext> client = new ClientManage<WebSocketContext>(webSocketContext, cancellationTokenSource, MonitorMessageAsync(cancellationTokenSource.Token, webSocketContext.WebSocket, IpPort));
							_clients.AddOrUpdate(IpPort, client, (string k, ClientManage<WebSocketContext> v) => client);
							await OnDataEventHandlerAsync(this, new EventDataResult(true, "[" + IpPort + "]连接成功", (object)new ClientMessage
							{
								Step = Steps.客户端连接,
								IpPort = IpPort
							}));
						}
						catch
						{
							httpListenerContext.Response.StatusCode = 500;
							httpListenerContext.Response.Close();
						}
					}
					else
					{
						httpListenerContext.Response.StatusCode = 400;
						httpListenerContext.Response.Close();
					}
				}
				catch (ObjectDisposedException)
				{
				}
				catch (HttpListenerException)
				{
				}
				catch (TaskCanceledException)
				{
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex5)
				{
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "[" + base.TAG + "]监控客户端连接异常：" + ex5.Message)).ConfigureAwait(continueOnCapturedContext: false);
					try
					{
						await Task.Delay(100, token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				}
			}
		}

		/// <summary>
		/// 监控消息任务
		/// </summary>
		private async Task MonitorMessageAsync(CancellationToken token, WebSocket webSocket, string ipPort)
		{
			byte[] buffer = new byte[base.basics.BufferSize];
			try
			{
				while (!token.IsCancellationRequested)
				{
					try
					{
						WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync(buffer, token).ConfigureAwait(continueOnCapturedContext: false);
						if (webSocketReceiveResult.Count > 0)
						{
							await OnDataEventHandlerAsync(this, new EventDataResult(true, "[" + ipPort + "]数据接收成功", (object)new ClientMessage
							{
								Step = Steps.消息接收,
								IpPort = ipPort,
								Bytes = buffer[..webSocketReceiveResult.Count]
							}));
							continue;
						}
						await OnDataEventHandlerAsync(this, new EventDataResult(false, "[" + ipPort + "]客户端断开连接（数据长度为零）", (object)new ClientMessage
						{
							Step = Steps.客户端断开,
							IpPort = ipPort
						})).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (TaskCanceledException)
					{
						continue;
					}
					catch (OperationCanceledException)
					{
						continue;
					}
					catch (Exception ex3)
					{
						await OnDataEventHandlerAsync(this, new EventDataResult(false, "监控[" + ipPort + "]消息异常：" + ex3.Message, (object)new ClientMessage
						{
							Step = Steps.客户端断开,
							IpPort = ipPort
						})).ConfigureAwait(continueOnCapturedContext: false);
					}
					return;
				}
			}
			finally
			{
				await DisconnectClientAsync(ipPort, notify: false).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 私有分包发送
		/// </summary>
		private async Task<bool> SendChunkedAsync(WebSocket socket, byte[] data, CancellationToken token)
		{
			int maxChunkSize = base.basics.MaxChunkSize;
			int chunkSize;
			for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
			{
				chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
				ArraySegment<byte> chunk = new ArraySegment<byte>(data, bytesSent, chunkSize);
				bool sent = false;
				for (int attempt = 0; attempt <= base.basics.RetrySendCount; attempt++)
				{
					try
					{
						using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
						cts.CancelAfter(base.basics.Timeout);
						await socket.SendAsync(chunk, WebSocketMessageType.Binary, endOfMessage: true, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
						sent = true;
					}
					catch when (attempt < base.basics.RetrySendCount && !token.IsCancellationRequested)
					{
						await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
						continue;
					}
					break;
				}
				if (!sent)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 向单个客户端发送数据。
		/// </summary>
		private async Task<(string IpPort, bool Success, string? Error)> SendToClientAsync(ClientManage<WebSocketContext> session, string ipPort, byte[] data, CancellationToken token)
		{
			_ = 1;
			try
			{
				bool flag;
				if (data.Length > base.basics.MaxChunkSize)
				{
					flag = await SendChunkedAsync(session.Client.WebSocket, data, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					await session.Client.WebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, endOfMessage: true, token).ConfigureAwait(continueOnCapturedContext: false);
					flag = true;
				}
				return (IpPort: ipPort, Success: flag, Error: flag ? null : "部分数据块发送失败");
			}
			catch (Exception ex)
			{
				return (IpPort: ipPort, Success: false, Error: ex.Message);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "OnAsync", 199);
				}
				HttpListener httpListener = new HttpListener();
				string text = "ws://";
				string newValue = "http://";
				string uriPrefix = base.basics.Host;
				if (base.basics.Host.Contains(text))
				{
					uriPrefix = base.basics.Host.Replace(text, newValue);
				}
				httpListener.Prefixes.Add(uriPrefix);
				httpListener.Start();
				_service = httpListener;
				_acceptCts = new CancellationTokenSource();
				_monitorConnectTask = MonitorConnectAsync(_acceptCts.Token);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "OnAsync", 217).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "OnAsync", 222).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "OffAsync", 235);
				}
				CancellationTokenSource cts = _acceptCts;
				HttpListener listener = _service;
				Task loopTask = _monitorConnectTask;
				if (cts != null)
				{
					_acceptCts = null;
					try
					{
						await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (ObjectDisposedException)
					{
					}
				}
				KeyValuePair<string, ClientManage<WebSocketContext>>[] array = _clients.ToArray();
				_clients.Clear();
				KeyValuePair<string, ClientManage<WebSocketContext>>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					KeyValuePair<string, ClientManage<WebSocketContext>> kvp = array2[i];
					try
					{
						ClientManage<WebSocketContext> value = kvp.Value;
						try
						{
							value.Cts?.Cancel();
						}
						catch (ObjectDisposedException)
						{
						}
						try
						{
							value.Cts?.Dispose();
						}
						catch (ObjectDisposedException)
						{
						}
						try
						{
							await value.MonitorTask.ConfigureAwait(continueOnCapturedContext: false);
						}
						catch
						{
						}
					}
					catch (Exception ex4)
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"[{base.TAG}]关闭客户端[{kvp.Key}]时异常：{ex4.Message}")).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				try
				{
					listener?.Stop();
				}
				catch
				{
				}
				if (loopTask != null)
				{
					try
					{
						using CancellationTokenSource waitCts = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
						await loopTask.WaitAsync(waitCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (OperationCanceledException)
					{
					}
					catch
					{
					}
				}
				_monitorConnectTask = null;
				try
				{
					cts?.Dispose();
				}
				catch
				{
				}
				try
				{
					listener?.Close();
				}
				catch
				{
				}
				_service = null;
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "OffAsync", 290).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex6)
			{
				return await EndOperateAsync(status: false, ex6.Message, null, ex6, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "OffAsync", 294).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> RemoveAsync(string[] address, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "RemoveAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failed = new List<string>();
				foreach (string ipPort in address)
				{
					try
					{
						await DisconnectClientAsync(ipPort).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex)
					{
						failed.Add("[" + ipPort + "]: " + ex.Message);
					}
				}
				if (failed.Count > 0)
				{
					return await EndOperateAsync(status: false, string.Join("; ", failed), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "RemoveAsync", 318).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "RemoveAsync", 320).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "RemoveAsync", 324).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 安全断开单个客户端：从字典移除 → 取消 CTS → 释放资源。
		/// </summary>
		private async Task DisconnectClientAsync(string ipPort, bool notify = true)
		{
			if (_clients.TryRemove(ipPort, out ClientManage<WebSocketContext> value))
			{
				try
				{
					value.Cts?.Cancel();
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					value.Cts?.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					await value.MonitorTask.ConfigureAwait(continueOnCapturedContext: false);
				}
				catch
				{
				}
			}
			if (notify)
			{
				await OnDataEventHandlerAsync(this, new EventDataResult(false, "[" + ipPort + "]已断开连接", (object)new ClientMessage
				{
					Step = Steps.客户端断开,
					IpPort = ipPort
				})).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, string[]? address = null, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "SendAsync", 353);
				}
				if (address == null)
				{
					if (_clients.IsEmpty)
					{
						return await EndOperateAsync(status: false, "客户端未连接", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "SendAsync", 359).ConfigureAwait(continueOnCapturedContext: false);
					}
					List<string> list = (from r in await Task.WhenAll(_clients.Select<KeyValuePair<string, ClientManage<WebSocketContext>>, Task<(string, bool, string)>>((KeyValuePair<string, ClientManage<WebSocketContext>> kvp) => SendToClientAsync(kvp.Value, kvp.Key, data, token)).ToList()).ConfigureAwait(continueOnCapturedContext: false)
						where !r.Success
						select "[" + r.IpPort + "]: " + r.Error).ToList();
					if (list.Count > 0)
					{
						return await EndOperateAsync(status: false, ExtendMethod.ToJson<List<string>>(list, false), list, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "SendAsync", 368).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else
				{
					List<string> list2 = (from r in await Task.WhenAll(address.Select((string ipPort) => (Task<(string, bool, string)>)(_clients.TryGetValue(ipPort, out ClientManage<WebSocketContext> value) ? ((Task)SendToClientAsync(value, ipPort, data, token)) : ((Task)Task.FromResult((ipPort, false, "客户端[" + ipPort + "]不存在"))))).ToList()).ConfigureAwait(continueOnCapturedContext: false)
						where !r.Item2
						select "[" + r.Item1 + "]: " + r.Item3).ToList();
					if (list2.Count > 0)
					{
						return await EndOperateAsync(status: false, ExtendMethod.ToJson<List<string>>(list2, false), list2, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "SendAsync", 384).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "SendAsync", 387).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "SendAsync", 391).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, string address, CancellationToken token = default(CancellationToken))
		{
			return await SendAsync(data, new string[1] { address }, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "GetBaseObjectAsync", 406);
			}
			return await EndOperateAsync(status: true, null, _service, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "GetBaseObjectAsync", 409).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_service != null && _service.IsListening)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("已启动", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "GetStatusAsync", 418).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("未启动", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\service\\WsServiceOperate.cs", "GetStatusAsync", 420).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.ws.client
{
	public class WsClientData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 主机地址
			/// </summary>
			[Description("主机地址")]
			public string Host { get; set; } = "ws://127.0.0.1:6688/";

			/// <summary>
			/// 是否需要断开重新连接
			/// </summary>
			[Description("是否需要断开重新连接")]
			public bool InterruptReconnection { get; set; } = true;

			/// <summary>
			/// 重连间隔(毫秒)
			/// </summary>
			[Description("重连间隔(毫秒)")]
			public int ReconnectionInterval { get; set; } = 1000;

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 发送等待间隔<br />
			/// 继承者可选重写
			/// </summary>
			[Description("发送等待间隔")]
			public virtual int SendWaitInterval { get; set; } = 5000;

			/// <summary>
			/// 最大块大小；<br />
			/// 如果数据超过限定值则自动分包发送<br />
			/// 继承者可选重写
			/// </summary>
			[Description("最大块大小")]
			public virtual int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数；<br />
			/// 只在分包发送中触发；<br />
			/// 在限定次数中还是失败则直接返回失败<br />
			/// 继承者可选重写
			/// </summary>
			[Description("重试发送次数")]
			public virtual int RetrySendCount { get; set; } = 5;

			/// <summary>
			/// 数据缓冲区大小<br />
			/// 继承者可选重写
			/// </summary>
			[Description("数据缓冲区大小")]
			public virtual int BufferSize { get; set; } = 1048576;
		}
	}
	/// <summary>
	/// WebSocket 客户端通信；<br />
	/// 提供 WebSocket 连接、发送、接收、断线重连等功能；<br />
	/// 支持自动分包发送、发送等待响应、数据监控回调。
	/// <para>
	/// 【并发模型说明】<br />
	/// - _commLock（SemaphoreSlim）保护 Communication、_monitorCts、_reconnectCts 的读写一致性；<br />
	/// - InterruptReconnectionAsync 在锁内检测断开并重新连接；<br />
	/// - SendWaitAsync 停止监控后独占读取，完成后重启监控；<br />
	/// - Dispose 使用 Interlocked 防重复释放。
	/// </para>
	/// </summary>
	public class WsClientOperate : CommunicationAbstract<WsClientOperate, WsClientData.Basics>, ICommunication, IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// WebSocket 通信实例，受 _commLock 保护。
		/// </summary>
		private ClientWebSocket? _communication;

		/// <summary>
		/// 监控读循环的取消令牌源，受 _commLock 保护。
		/// </summary>
		private CancellationTokenSource? _monitorCts;

		/// <summary>
		/// 断线重连循环的取消令牌源，受 _commLock 保护。
		/// </summary>
		private CancellationTokenSource? _reconnectCts;

		public WsClientOperate(WsClientData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 关闭 WebSocket 连接并停止监控。
		/// </summary>
		private async Task<int> CloseUnderLockAsync()
		{
			await StopMonitorUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
			ClientWebSocket comm = _communication;
			if (comm == null)
			{
				return 1;
			}
			WebSocketState state = comm.State;
			if ((uint)(state - 2) <= 2u)
			{
				try
				{
					using CancellationTokenSource cts = new CancellationTokenSource(base.basics.Timeout);
					await comm.CloseAsync(WebSocketCloseStatus.Empty, null, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
					comm.Dispose();
					_communication = null;
					return 1;
				}
				catch (OperationCanceledException)
				{
					try
					{
						comm.Abort();
					}
					catch
					{
					}
					comm.Dispose();
					_communication = null;
					return 2;
				}
				catch (Exception)
				{
					try
					{
						comm.Abort();
					}
					catch
					{
					}
					comm.Dispose();
					_communication = null;
					return 1;
				}
			}
			try
			{
				comm.Abort();
			}
			catch
			{
			}
			comm.Dispose();
			_communication = null;
			return 1;
		}

		private async Task StopMonitorUnderLockAsync()
		{
			CancellationTokenSource cts = _monitorCts;
			if (cts != null)
			{
				_monitorCts = null;
				try
				{
					await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					cts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private void StartMonitorUnderLock()
		{
			CancellationTokenSource monitorCts = _monitorCts;
			if (monitorCts == null || monitorCts.IsCancellationRequested)
			{
				_monitorCts = new CancellationTokenSource();
				MonitorAsync(_monitorCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		private void StopReconnectionUnderLock()
		{
			CancellationTokenSource reconnectCts = _reconnectCts;
			if (reconnectCts != null)
			{
				_reconnectCts = null;
				try
				{
					reconnectCts.Cancel();
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					reconnectCts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		/// <summary>
		/// 启动断线重连循环。
		/// </summary>
		private void StartReconnectionUnderLock()
		{
			if (base.basics.InterruptReconnection)
			{
				CancellationTokenSource reconnectCts = _reconnectCts;
				if (reconnectCts == null || reconnectCts.IsCancellationRequested)
				{
					_reconnectCts = new CancellationTokenSource();
					InterruptReconnectionAsync(_reconnectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		private async Task MonitorAsync(CancellationToken token)
		{
			byte[] buffer = new byte[base.basics.BufferSize];
			while (!token.IsCancellationRequested)
			{
				try
				{
					ClientWebSocket communication = _communication;
					if (communication == null)
					{
						await Task.Delay(100, token).ConfigureAwait(continueOnCapturedContext: false);
						continue;
					}
					WebSocketReceiveResult result = await communication.ReceiveAsync(buffer, token).ConfigureAwait(continueOnCapturedContext: false);
					object sender;
					string tAG;
					if (result.Count > 0)
					{
						sender = this;
						tAG = base.TAG;
						await OnDataEventHandlerAsync(sender, new EventDataResult(true, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据", null, token).ConfigureAwait(continueOnCapturedContext: false), (object)buffer[..result.Count])).ConfigureAwait(continueOnCapturedContext: false);
						continue;
					}
					sender = this;
					tAG = base.TAG;
					await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据错误", null, token).ConfigureAwait(continueOnCapturedContext: false))).ConfigureAwait(continueOnCapturedContext: false);
					await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
					if (!base.basics.InterruptReconnection)
					{
						_communication = null;
					}
					break;
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (WebSocketException ex3)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex3.Message))).ConfigureAwait(continueOnCapturedContext: false);
						await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
				catch (Exception ex4)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex4.Message))).ConfigureAwait(continueOnCapturedContext: false);
						await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
			}
		}

		/// <summary>
		/// 断线重连循环。
		/// </summary>
		private async Task InterruptReconnectionAsync(CancellationToken token)
		{
			int ReconnectionCount = 1;
			while (!token.IsCancellationRequested)
			{
				try
				{
					bool needConnect = _communication == null || _communication.State != WebSocketState.Open;
					if (needConnect)
					{
						await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
						_communication = new ClientWebSocket();
					}
					if (needConnect)
					{
						ClientWebSocket commForConnect = _communication;
						if (commForConnect == null)
						{
							continue;
						}
						bool flag = false;
						try
						{
							using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
							cts.CancelAfter(base.basics.Timeout);
							await commForConnect.ConnectAsync(new Uri(base.basics.Host), cts.Token).ConfigureAwait(continueOnCapturedContext: false);
							flag = commForConnect.State == WebSocketState.Open;
						}
						catch (OperationCanceledException)
						{
							flag = false;
						}
						catch
						{
							flag = false;
						}
						object sender;
						string tAG;
						if (!flag)
						{
							sender = this;
							tAG = base.TAG;
							await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("重连失败", null, token).ConfigureAwait(continueOnCapturedContext: false), ReconnectionCount))).ConfigureAwait(continueOnCapturedContext: false);
							ReconnectionCount++;
							await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
							_communication = null;
							await Task.Delay(Math.Min(500 * (1 << Math.Min(ReconnectionCount, 10)), 30000), token).ConfigureAwait(continueOnCapturedContext: false);
							continue;
						}
						sender = this;
						tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(true, "[" + tAG + "]" + await GetLanguageValueAsync("重连成功", null, token).ConfigureAwait(continueOnCapturedContext: false))).ConfigureAwait(continueOnCapturedContext: false);
						ReconnectionCount = 1;
						StartMonitorUnderLock();
					}
				}
				catch (TaskCanceledException)
				{
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex4)
				{
					object sender = this;
					string tAG = base.TAG;
					string text = await GetLanguageValueAsync("重连异常", null, token).ConfigureAwait(continueOnCapturedContext: false);
					await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + text + "：" + ex4.Message)).ConfigureAwait(continueOnCapturedContext: false);
				}
				await Task.Delay(base.basics.ReconnectionInterval, token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		private async Task<bool> SendPrvateAsync(byte[] data)
		{
			return await SendPrvateAsync(new ArraySegment<byte>(data)).ConfigureAwait(continueOnCapturedContext: false);
		}

		private async Task<bool> SendPrvateAsync(ArraySegment<byte> data)
		{
			try
			{
				ClientWebSocket communication = _communication;
				if (communication == null || communication.State != WebSocketState.Open)
				{
					return false;
				}
				using CancellationTokenSource cts = new CancellationTokenSource(base.basics.Timeout);
				await communication.SendAsync(data, WebSocketMessageType.Binary, endOfMessage: true, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OnAsync", 287);
				}
				ClientWebSocket comm = new ClientWebSocket();
				try
				{
					using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
					cts.CancelAfter(base.basics.Timeout);
					await comm.ConnectAsync(new Uri(base.basics.Host), cts.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (OperationCanceledException)
				{
					comm.Dispose();
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("连接超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OnAsync", 300).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception ex2)
				{
					comm.Dispose();
					return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OnAsync", 307).ConfigureAwait(continueOnCapturedContext: false);
				}
				_communication = comm;
				StartReconnectionUnderLock();
				StartMonitorUnderLock();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OnAsync", 318).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex3)
			{
				try
				{
					_communication?.Dispose();
				}
				catch
				{
				}
				_communication = null;
				return await EndOperateAsync(status: false, ex3.Message, null, ex3, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OnAsync", 324).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OffAsync", 339);
				}
				StopReconnectionUnderLock();
				return (OperateResult)(await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false) switch
				{
					0 => await EndOperateAsync(status: false, await GetLanguageValueAsync("关闭失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OffAsync", 348).ConfigureAwait(continueOnCapturedContext: false), 
					2 => await EndOperateAsync(status: false, await GetLanguageValueAsync("关闭超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OffAsync", 351).ConfigureAwait(continueOnCapturedContext: false), 
					_ => await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OffAsync", 354).ConfigureAwait(continueOnCapturedContext: false), 
				});
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "OffAsync", 359).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendAsync", 371);
				}
				if (data.Length > base.basics.MaxChunkSize)
				{
					int maxChunkSize = base.basics.MaxChunkSize;
					int chunkSize;
					for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
					{
						chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
						ArraySegment<byte> chunk = new ArraySegment<byte>(data, bytesSent, chunkSize);
						if (await SendPrvateAsync(chunk).ConfigureAwait(continueOnCapturedContext: false))
						{
							continue;
						}
						bool retrySent = false;
						for (int i = 1; i <= base.basics.RetrySendCount; i++)
						{
							if (await SendPrvateAsync(chunk).ConfigureAwait(continueOnCapturedContext: false))
							{
								retrySent = true;
								break;
							}
						}
						if (!retrySent)
						{
							return await EndOperateAsync(status: false, await GetLanguageValueAsync("存在数据块发送失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendAsync", 392).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
				}
				else if (!(await SendPrvateAsync(data).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("发送数据超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendAsync", 402).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendAsync", 406).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendAsync", 410).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token)
		{
			await BegOperateAsync(token, "SendWaitAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendWaitAsync", 422);
				}
				await StopMonitorUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
				bool sent;
				if (data.Length > base.basics.MaxChunkSize)
				{
					sent = true;
					int maxChunkSize = base.basics.MaxChunkSize;
					int chunkSize;
					for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
					{
						chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
						ArraySegment<byte> chunk = new ArraySegment<byte>(data, bytesSent, chunkSize);
						if (await SendPrvateAsync(chunk).ConfigureAwait(continueOnCapturedContext: false))
						{
							continue;
						}
						bool retrySent = false;
						for (int i = 1; i <= base.basics.RetrySendCount; i++)
						{
							if (await SendPrvateAsync(chunk).ConfigureAwait(continueOnCapturedContext: false))
							{
								retrySent = true;
								break;
							}
						}
						if (!retrySent)
						{
							sent = false;
							break;
						}
					}
				}
				else
				{
					sent = await SendPrvateAsync(data).ConfigureAwait(continueOnCapturedContext: false);
				}
				if (!sent)
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("发送等待结果失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendWaitAsync", 458).ConfigureAwait(continueOnCapturedContext: false);
				}
				byte[] buffer = new byte[base.basics.BufferSize];
				int length = 0;
				bool flag = false;
				try
				{
					using CancellationTokenSource readCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					readCts.CancelAfter(base.basics.SendWaitInterval);
					ClientWebSocket communication = _communication;
					if (communication != null && communication.State == WebSocketState.Open)
					{
						length = (await communication.ReceiveAsync(buffer, readCts.Token).ConfigureAwait(continueOnCapturedContext: false)).Count;
						flag = length > 0;
					}
				}
				catch (OperationCanceledException)
				{
					flag = false;
				}
				if (flag)
				{
					return await EndOperateAsync(status: true, null, buffer[..length], null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendWaitAsync", 484).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("接收数据超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendWaitAsync", 487).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "SendWaitAsync", 493).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				if (_communication != null && _communication.State == WebSocketState.Open)
				{
					StartMonitorUnderLock();
				}
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "GetBaseObjectAsync", 510);
			}
			return await EndOperateAsync(status: true, null, _communication, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "GetBaseObjectAsync", 512).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_communication != null && _communication.State == WebSocketState.Open)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("已连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "GetStatusAsync", 521).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\ws\\client\\WsClientOperate.cs", "GetStatusAsync", 525).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.udp.unicast.service
{
	public class UdpServiceData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// Ip地址
			/// </summary>
			[Description("Ip地址")]
			public string? IpAddress { get; set; } = "127.0.0.1";

			/// <summary>
			/// 端口
			/// </summary>
			[Description("端口")]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 最大块大小;
			/// 如果数据超过限定值则自动分包发送
			/// </summary>
			[Description("最大块大小")]
			public int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数;
			/// 只在分包发送中触发;
			/// 在限定次数中还是失败则直接返回失败
			/// </summary>
			[Description("重试发送次数")]
			public int RetrySendCount { get; set; } = 5;

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 客户端超时时间(毫秒)；<br />
			/// 超过此时间未收到客户端数据则视为断开并触发客户端断开事件
			/// </summary>
			[Description("客户端超时时间(毫秒)")]
			public int ClientTimeout { get; set; } = 60000;
		}
	}
	/// <summary>
	/// UDP 单播服务端通信；<br />
	/// 提供 UDP 监听、客户端管理、群发/定向发送、消息监控等功能；<br />
	/// 使用 ConcurrentDictionary 管理客户端会话，支持客户端连接/断开事件通知和超时清理。
	/// <para>
	/// 【并发模型说明】<br />
	/// - _serverLock（SemaphoreSlim）保护 UdpClient 引用（Communication）的读写一致性；<br />
	/// - _clients（ConcurrentDictionary）管理所有已知客户端会话，读写线程安全；<br />
	/// - 单一接收循环统一处理所有客户端数据，无每客户端独立 Task；<br />
	/// - 独立清理循环定期移除超时客户端，触发断开事件（复刻自 TCP 服务端的客户端生命周期管理）。
	/// </para>
	/// </summary>
	public class UdpServiceOperate(UdpServiceData.Basics basics) : CommunicationServiceAbstract<UdpServiceOperate, UdpServiceData.Basics>(basics), ICommunicationService, IOn, IOff, ISendService, IObject, IStatus, IRemove, IEvent, IInstance, ILog, ILanguage, IClone, IArgs, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// UDP 客户端会话；<br />
		/// 封装客户端端点和最后活跃时间，用于连接/断开生命周期管理。
		/// <para>复刻自 TCP 服务端的 ClientManage&lt;TcpClient&gt; 模式，
		/// 但 UDP 无持久连接对象，故仅追踪端点和活跃时间。</para>
		/// </summary>
		private sealed class UdpClientSession
		{
			/// <summary>客户端端点</summary>
			public IPEndPoint Endpoint { get; }

			/// <summary>最后活跃时间（UTC），用于超时检测</summary>
			public DateTime LastActivity { get; set; }

			public UdpClientSession(IPEndPoint endpoint)
			{
				Endpoint = endpoint;
				LastActivity = DateTime.UtcNow;
			}
		}

		/// <summary>UDP 监听器，受 _serverLock 保护。</summary>
		private UdpClient? _listener;

		/// <summary>接收循环的取消令牌源。</summary>
		private CancellationTokenSource? _receiveCts;

		/// <summary>接收循环的任务引用。</summary>
		private Task? _receiveTask;

		/// <summary>客户端清理循环的取消令牌源。</summary>
		private CancellationTokenSource? _cleanupCts;

		/// <summary>客户端清理循环的任务引用。</summary>
		private Task? _cleanupTask;

		/// <summary>已知客户端会话管理容器（线程安全），Key 为 "IP:Port"。</summary>
		private readonly ConcurrentDictionary<string, UdpClientSession> _clients = new ConcurrentDictionary<string, UdpClientSession>();

		/// <summary>
		/// 接收循环：持续从 UdpClient 读取数据。
		/// <para>读到数据后：
		/// 1. 自动检测新客户端 → 触发 <see cref="F:Snet.Core.communication.net.enum.Steps.客户端连接" /> 事件；
		/// 2. 更新客户端活跃时间；
		/// 3. 触发 <see cref="F:Snet.Core.communication.net.enum.Steps.消息接收" /> 事件。</para>
		/// <para>复刻自 TCP 服务端的 AcceptLoopAsync + MonitorClientMessagesAsync 合并逻辑。</para>
		/// </summary>
		private async Task ReceiveLoopAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					UdpClient listener = _listener;
					using CancellationTokenSource receiveCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					receiveCts.CancelAfter((base.basics.Timeout > 0) ? base.basics.Timeout : (-1));
					UdpReceiveResult result = await listener.ReceiveAsync(receiveCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					string ipPort = result.RemoteEndPoint.ToString();
					DateTime now = DateTime.UtcNow;
					bool num = !_clients.ContainsKey(ipPort);
					_clients.AddOrUpdate(ipPort, (string _) => new UdpClientSession(result.RemoteEndPoint)
					{
						LastActivity = now
					}, delegate(string _, UdpClientSession session)
					{
						session.LastActivity = now;
						return session;
					});
					if (num)
					{
						await OnDataEventHandlerAsync(this, new EventDataResult(true, "[" + ipPort + "]客户端连接", (object)new ClientMessage
						{
							Step = Steps.客户端连接,
							IpPort = ipPort
						})).ConfigureAwait(continueOnCapturedContext: false);
					}
					await OnDataEventHandlerAsync(this, new EventDataResult(true, "[" + ipPort + "]数据接收成功", (object)new ClientMessage
					{
						Step = Steps.消息接收,
						IpPort = ipPort,
						Bytes = result.Buffer
					})).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				catch (TaskCanceledException)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
				}
				catch (OperationCanceledException)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
				}
				catch (SocketException ex4)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "[" + base.TAG + "]接收异常：" + ex4.Message)).ConfigureAwait(continueOnCapturedContext: false);
					await Task.Delay(100, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception ex5)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "[" + base.TAG + "]接收异常：" + ex5.Message)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		/// <summary>
		/// 客户端超时清理循环：定期移除不活跃的客户端。
		/// <para>复刻自 TCP 服务端的 DisconnectClientAsync 超时断开模式。
		/// TCP 通过读取零长度检测断开；UDP 无连接语义，故通过活跃超时判断。</para>
		/// </summary>
		private async Task ClientCleanupLoopAsync(CancellationToken token)
		{
			int intervalMs = Math.Max(base.basics.ClientTimeout / 3, 5000);
			while (!token.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(intervalMs, token).ConfigureAwait(continueOnCapturedContext: false);
					DateTime threshold = DateTime.UtcNow.AddMilliseconds(-base.basics.ClientTimeout);
					List<string> list = (from kvp in _clients
						where kvp.Value.LastActivity < threshold
						select kvp.Key).ToList();
					foreach (string item in list)
					{
						if (_clients.TryRemove(item, out UdpClientSession _))
						{
							await OnDataEventHandlerAsync(this, new EventDataResult(false, "[" + item + "]客户端断开（超时）", (object)new ClientMessage
							{
								Step = Steps.客户端断开,
								IpPort = item
							})).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				catch (Exception ex4)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "[" + base.TAG + "]清理异常：" + ex4.Message)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		/// <summary>
		/// 向指定端点发送数据（支持自动分包和重试）。
		/// </summary>
		private async Task<bool> SendToEndpointAsync(IPEndPoint endpoint, byte[] data, CancellationToken token)
		{
			int maxChunkSize = base.basics.MaxChunkSize;
			int chunkSize;
			for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
			{
				chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
				bool sent = false;
				for (int attempt = 0; attempt <= base.basics.RetrySendCount; attempt++)
				{
					try
					{
						UdpClient listener = _listener;
						if (listener == null)
						{
							return false;
						}
						using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token))
						{
							cts.CancelAfter(base.basics.Timeout);
							byte[] subArray = data[bytesSent..(bytesSent + chunkSize)];
							if (await listener.SendAsync(new ReadOnlyMemory<byte>(subArray), endpoint, cts.Token).ConfigureAwait(continueOnCapturedContext: false) > 0)
							{
								sent = true;
								break;
							}
						}
						continue;
					}
					catch when (attempt < base.basics.RetrySendCount && !token.IsCancellationRequested)
					{
						await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
						continue;
					}
				}
				if (!sent)
				{
					return false;
				}
			}
			return true;
		}

		private async Task<(string IpPort, bool Success, string? Error)> SendToClientAsync(IPEndPoint endpoint, string ipPort, byte[] data, CancellationToken token)
		{
			try
			{
				bool flag = await SendToEndpointAsync(endpoint, data, token).ConfigureAwait(continueOnCapturedContext: false);
				return (IpPort: ipPort, Success: flag, Error: flag ? null : "部分数据块发送失败");
			}
			catch (Exception ex)
			{
				return (IpPort: ipPort, Success: false, Error: ex.Message);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "OnAsync", 247);
				}
				_listener = new UdpClient(new IPEndPoint(IPAddress.Parse(base.basics.IpAddress), base.basics.Port));
				_listener.Client.ReceiveTimeout = base.basics.Timeout;
				_listener.Client.SendTimeout = base.basics.Timeout;
				_receiveCts?.Cancel();
				_receiveCts?.Dispose();
				_receiveCts = new CancellationTokenSource();
				_receiveTask = ReceiveLoopAsync(_receiveCts.Token);
				_cleanupCts?.Cancel();
				_cleanupCts?.Dispose();
				_cleanupCts = new CancellationTokenSource();
				_cleanupTask = ClientCleanupLoopAsync(_cleanupCts.Token);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "OnAsync", 266).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "OnAsync", 271).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "OffAsync", 285);
				}
				CancellationTokenSource receiveCts = _receiveCts;
				if (receiveCts != null)
				{
					_receiveCts = null;
					receiveCts.Cancel();
					receiveCts.Dispose();
				}
				Task receiveTask = _receiveTask;
				if (receiveTask != null && !receiveTask.IsCompleted)
				{
					try
					{
						using CancellationTokenSource timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3.0));
						await receiveTask.WaitAsync(timeoutCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				}
				_receiveTask = null;
				CancellationTokenSource cleanupCts = _cleanupCts;
				if (cleanupCts != null)
				{
					_cleanupCts = null;
					cleanupCts.Cancel();
					cleanupCts.Dispose();
				}
				Task cleanupTask = _cleanupTask;
				if (cleanupTask != null && !cleanupTask.IsCompleted)
				{
					try
					{
						using CancellationTokenSource timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3.0));
						await cleanupTask.WaitAsync(timeoutCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				}
				_cleanupTask = null;
				KeyValuePair<string, UdpClientSession>[] array = _clients.ToArray();
				_clients.Clear();
				KeyValuePair<string, UdpClientSession>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					KeyValuePair<string, UdpClientSession> keyValuePair = array2[i];
					await OnDataEventHandlerAsync(this, new EventDataResult(false, "[" + keyValuePair.Key + "]已断开连接", (object)new ClientMessage
					{
						Step = Steps.客户端断开,
						IpPort = keyValuePair.Key
					})).ConfigureAwait(continueOnCapturedContext: false);
				}
				try
				{
					_listener?.Close();
				}
				catch
				{
				}
				try
				{
					_listener?.Dispose();
				}
				catch
				{
				}
				_listener = null;
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "OffAsync", 346).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "OffAsync", 350).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> RemoveAsync(string[] address, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "RemoveAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failed = new List<string>();
				foreach (string text in address)
				{
					if (_clients.TryRemove(text, out UdpClientSession _))
					{
						await OnDataEventHandlerAsync(this, new EventDataResult(false, "[" + text + "]已移除", (object)new ClientMessage
						{
							Step = Steps.客户端断开,
							IpPort = text
						})).ConfigureAwait(continueOnCapturedContext: false);
					}
					else
					{
						failed.Add("[" + text + "]: 客户端不存在");
					}
				}
				if (failed.Count > 0)
				{
					return await EndOperateAsync(status: false, string.Join("; ", failed), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "RemoveAsync", 379).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "RemoveAsync", 381).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "RemoveAsync", 385).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, string[]? address = null, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "SendAsync", 397);
				}
				if (address == null)
				{
					if (_clients.IsEmpty)
					{
						return await EndOperateAsync(status: false, "客户端未连接", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "SendAsync", 403).ConfigureAwait(continueOnCapturedContext: false);
					}
					List<string> list = (from r in await Task.WhenAll(_clients.Select<KeyValuePair<string, UdpClientSession>, Task<(string, bool, string)>>((KeyValuePair<string, UdpClientSession> kvp) => SendToClientAsync(kvp.Value.Endpoint, kvp.Key, data, token)).ToList()).ConfigureAwait(continueOnCapturedContext: false)
						where !r.Success
						select "[" + r.IpPort + "]: " + r.Error).ToList();
					if (list.Count > 0)
					{
						return await EndOperateAsync(status: false, ExtendMethod.ToJson<List<string>>(list, false), list, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "SendAsync", 412).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else
				{
					List<string> list2 = (from r in await Task.WhenAll(address.Select((string ipPort) => (Task<(string, bool, string)>)(_clients.TryGetValue(ipPort, out UdpClientSession value) ? ((Task)SendToClientAsync(value.Endpoint, ipPort, data, token)) : ((Task)Task.FromResult((ipPort, false, "客户端[" + ipPort + "]不存在"))))).ToList()).ConfigureAwait(continueOnCapturedContext: false)
						where !r.Item2
						select "[" + r.Item1 + "]: " + r.Item3).ToList();
					if (list2.Count > 0)
					{
						return await EndOperateAsync(status: false, ExtendMethod.ToJson<List<string>>(list2, false), list2, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "SendAsync", 428).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "SendAsync", 431).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "SendAsync", 435).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, string address, CancellationToken token = default(CancellationToken))
		{
			return await SendAsync(data, new string[1] { address }, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "GetBaseObjectAsync", 448);
			}
			return await EndOperateAsync(status: true, null, _listener, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "GetBaseObjectAsync", 450).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_listener != null)
			{
				return await EndOperateAsync(status: true, "已启动", null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "GetStatusAsync", 459).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, "未启动", null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\service\\UdpServiceOperate.cs", "GetStatusAsync", 461).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.udp.unicast.client
{
	public class UdpClientData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

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
			/// 本地端口（0 表示由系统自动分配）
			/// </summary>
			[Description("本地端口")]
			public int LocalPort { get; set; }

			/// <summary>
			/// 是否需要断开重新连接
			/// </summary>
			[Description("是否需要断开重新连接")]
			public bool InterruptReconnection { get; set; } = true;

			/// <summary>
			/// 重连间隔(毫秒)
			/// </summary>
			[Description("重连间隔(毫秒)")]
			public int ReconnectionInterval { get; set; } = 1000;

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 发送等待间隔<br />
			/// 继承者可选重写
			/// </summary>
			[Description("发送等待间隔")]
			public virtual int SendWaitInterval { get; set; } = 5000;

			/// <summary>
			/// 最大块大小；<br />
			/// 如果数据超过限定值则自动分包发送<br />
			/// 继承者可选重写
			/// </summary>
			[Description("最大块大小")]
			public virtual int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数；<br />
			/// 只在分包发送中触发；<br />
			/// 在限定次数中还是失败则直接返回失败<br />
			/// 继承者可选重写
			/// </summary>
			[Description("重试发送次数")]
			public virtual int RetrySendCount { get; set; } = 5;
		}
	}
	/// <summary>
	/// UDP单播客户端通信；<br />
	/// 提供 UDP 连接、发送、接收、断线重连等功能；<br />
	/// 支持自动分包发送、发送等待响应、数据监控回调。
	/// <para>
	/// 【并发模型说明】<br />
	/// 本类采用「单一读循环 + 请求-响应队列」架构：<br />
	/// - MonitorAsync 是唯一从 UdpClient 读取数据的协程，消除了并发读竞争；<br />
	/// - SendWaitAsync 通过 ConcurrentQueue&lt;TaskCompletionSource&gt; 注册等待项，<br />
	///   MonitorAsync 读到数据后分发给等待者或触发数据监控回调；<br />
	/// - _commLock（SemaphoreSlim）仅保护 Communication 引用本身的读写，<br />
	///   不保护 UdpClient 上的 I/O 操作（I/O 由单一协程独占）。
	/// </para>
	/// </summary>
	public class UdpClientOperate : CommunicationAbstract<UdpClientOperate, UdpClientData.Basics>, ICommunication, IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// UDP 通信实例，受 _commLock 保护。
		/// 所有读写 Communication 引用的操作必须先获取 _commLock。
		/// </summary>
		private UdpClient? _communication;

		/// <summary>
		/// 监控读循环的取消令牌源。控制 MonitorAsync 的生命周期。
		/// </summary>
		private CancellationTokenSource? _monitorCts;

		/// <summary>
		/// 断线重连循环的取消令牌源。控制 InterruptReconnectionAsync 的生命周期。
		/// </summary>
		private CancellationTokenSource? _reconnectCts;

		public UdpClientOperate(UdpClientData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 核心关闭逻辑：停止监控 → 停止重连 → 关闭 UDP 连接。
		/// </summary>
		private async Task CloseUnderLockAsync()
		{
			await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
			StopReconnection();
			try
			{
				_communication?.Client?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Client?.Dispose();
			}
			catch
			{
			}
			try
			{
				_communication?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Dispose();
			}
			catch
			{
			}
			_communication = null;
		}

		private async Task StopMonitorAsync()
		{
			CancellationTokenSource cts = _monitorCts;
			if (cts != null)
			{
				_monitorCts = null;
				try
				{
					await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					cts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private void StopReconnection()
		{
			CancellationTokenSource reconnectCts = _reconnectCts;
			if (reconnectCts != null)
			{
				_reconnectCts = null;
				try
				{
					reconnectCts.Cancel();
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					reconnectCts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private void StartMonitor()
		{
			if (_monitorCts == null || _monitorCts.IsCancellationRequested)
			{
				_monitorCts = new CancellationTokenSource();
				MonitorAsync(_monitorCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		private void StartReconnection()
		{
			if (base.basics.InterruptReconnection && (_reconnectCts == null || _reconnectCts.IsCancellationRequested))
			{
				_reconnectCts = new CancellationTokenSource();
				InterruptReconnectionAsync(_reconnectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 底层发送方法：向 UdpClient 写入单个数据块。
		/// </summary>
		private async Task<bool> SendPrivateAsync(byte[] data, CancellationToken token)
		{
			try
			{
				UdpClient communication = _communication;
				if (communication == null)
				{
					return false;
				}
				using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
				cts.CancelAfter(base.basics.Timeout);
				return await communication.SendAsync(data, cts.Token).ConfigureAwait(continueOnCapturedContext: false) > 0;
			}
			catch (OperationCanceledException)
			{
				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 自动分包发送
		/// </summary>
		private async Task<bool> SendAllChunksAsync(byte[] data, CancellationToken token)
		{
			if (data.Length > base.basics.MaxChunkSize)
			{
				int maxChunkSize = base.basics.MaxChunkSize;
				int chunkSize;
				for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
				{
					chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
					byte[] chunk = data[bytesSent..(bytesSent + chunkSize)];
					bool chunkSent = false;
					for (int attempt = 0; attempt <= base.basics.RetrySendCount; attempt++)
					{
						if (await SendPrivateAsync(chunk, token).ConfigureAwait(continueOnCapturedContext: false))
						{
							chunkSent = true;
							break;
						}
						if (attempt < base.basics.RetrySendCount && !token.IsCancellationRequested)
						{
							await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					if (!chunkSent)
					{
						return false;
					}
				}
				return true;
			}
			for (int bytesSent = 0; bytesSent <= base.basics.RetrySendCount; bytesSent++)
			{
				if (await SendPrivateAsync(data, token).ConfigureAwait(continueOnCapturedContext: false))
				{
					return true;
				}
				if (bytesSent < base.basics.RetrySendCount && !token.IsCancellationRequested)
				{
					await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			return false;
		}

		private async Task MonitorAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					UdpClient communication = _communication;
					if (communication == null)
					{
						break;
					}
					using CancellationTokenSource receiveCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					receiveCts.CancelAfter((base.basics.Timeout > 0) ? base.basics.Timeout : (-1));
					UdpReceiveResult result = await communication.ReceiveAsync(receiveCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					if (result.Buffer.Length != 0)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnDataEventHandlerAsync(sender, new EventDataResult(true, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据", null, token).ConfigureAwait(continueOnCapturedContext: false), (object)result.Buffer)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (TaskCanceledException)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
				}
				catch (OperationCanceledException)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				catch (SocketException ex4)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex4.Message))).ConfigureAwait(continueOnCapturedContext: false);
						await HandleDisconnectUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
				catch (Exception ex5)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex5.Message))).ConfigureAwait(continueOnCapturedContext: false);
						await HandleDisconnectUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
			}
		}

		private async Task HandleDisconnectUnderLockAsync()
		{
			try
			{
				await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
			}
			if (base.basics.InterruptReconnection)
			{
				StartReconnection();
			}
		}

		private async Task InterruptReconnectionAsync(CancellationToken token)
		{
			int reconnectionCount = 1;
			try
			{
				while (!token.IsCancellationRequested)
				{
					try
					{
						int backoffMs = Math.Min(500 * (1 << Math.Min(reconnectionCount, 10)), 30000);
						bool flag = _communication == null || _communication.Client == null;
						if (flag)
						{
							try
							{
								_communication?.Dispose();
							}
							catch
							{
							}
							_communication = new UdpClient(base.basics.LocalPort);
							_communication.Client.ReceiveTimeout = base.basics.Timeout;
							_communication.Client.SendTimeout = base.basics.Timeout;
						}
						if (!flag)
						{
							goto IL_04a2;
						}
						UdpClient communication = _communication;
						if (communication == null)
						{
							continue;
						}
						bool flag2 = false;
						try
						{
							using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
							cancellationTokenSource.CancelAfter(base.basics.Timeout);
							communication.Connect(IPAddress.Parse(base.basics.IpAddress), base.basics.Port);
							flag2 = communication.Client != null;
						}
						catch
						{
							if (_communication == communication)
							{
								try
								{
									_communication?.Dispose();
								}
								catch
								{
								}
								_communication = null;
							}
						}
						object sender;
						string tAG;
						if (!flag2)
						{
							sender = this;
							tAG = base.TAG;
							await OnInfoEventHandlerAsync(sender, EventInfoResult.CreateFailureResult("[" + tAG + "]" + string.Format(await GetLanguageValueAsync("重连失败", null, token).ConfigureAwait(continueOnCapturedContext: false), reconnectionCount))).ConfigureAwait(continueOnCapturedContext: false);
							reconnectionCount++;
							await Task.Delay(backoffMs, token).ConfigureAwait(continueOnCapturedContext: false);
							continue;
						}
						sender = this;
						tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, EventInfoResult.CreateSuccessResult("[" + tAG + "]" + await GetLanguageValueAsync("重连成功", null, token).ConfigureAwait(continueOnCapturedContext: false))).ConfigureAwait(continueOnCapturedContext: false);
						reconnectionCount = 1;
						StartMonitor();
						goto IL_04a2;
						IL_04a2:
						await Task.Delay(base.basics.ReconnectionInterval, token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (TaskCanceledException)
					{
					}
					catch (OperationCanceledException)
					{
					}
					catch (Exception ex3)
					{
						object sender = this;
						string tAG = base.TAG;
						string text = await GetLanguageValueAsync("重连异常", null, token).ConfigureAwait(continueOnCapturedContext: false);
						await OnInfoEventHandlerAsync(sender, EventInfoResult.CreateFailureResult("[" + tAG + "]" + text + "：" + ex3.Message)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		/// <summary>
		/// 建立 UDP 连接（绑定本地端口并连接到远程端点）。
		/// </summary>
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "OnAsync", 305);
				}
				UdpClient udpClient = new UdpClient(base.basics.LocalPort);
				udpClient.Client.ReceiveTimeout = base.basics.Timeout;
				udpClient.Client.SendTimeout = base.basics.Timeout;
				try
				{
					udpClient.Connect(IPAddress.Parse(base.basics.IpAddress), base.basics.Port);
				}
				catch
				{
					udpClient.Dispose();
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("连接超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "OnAsync", 319).ConfigureAwait(continueOnCapturedContext: false);
				}
				_communication = udpClient;
				if (base.basics.InterruptReconnection)
				{
					StopReconnection();
					_reconnectCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					InterruptReconnectionAsync(_reconnectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				StartMonitor();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "OnAsync", 332).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "OnAsync", 337).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 关闭 UDP 连接。
		/// </summary>
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "OffAsync", 353);
				}
				await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "OffAsync", 357).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "OffAsync", 361).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 发送数据（自动分包）。
		/// </summary>
		public override async Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendAsync", 375);
				}
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync((data.Length > base.basics.MaxChunkSize) ? "存在数据块发送失败" : "发送失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendAsync", 379).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendAsync", 381).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendAsync", 385).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 发送数据并等待响应。
		/// </summary>
		public override async Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token)
		{
			await BegOperateAsync(token, "SendWaitAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendWaitAsync", 401);
				}
				await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("发送等待结果失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendWaitAsync", 409).ConfigureAwait(continueOnCapturedContext: false);
				}
				UdpClient communication = _communication;
				if (communication == null || communication.Client == null)
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendWaitAsync", 417).ConfigureAwait(continueOnCapturedContext: false);
				}
				byte[] resultData = null;
				bool flag;
				try
				{
					using CancellationTokenSource readCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					readCts.CancelAfter(base.basics.SendWaitInterval);
					resultData = (await communication.ReceiveAsync(readCts.Token).ConfigureAwait(continueOnCapturedContext: false)).Buffer;
					flag = resultData.Length != 0;
				}
				catch (OperationCanceledException)
				{
					flag = false;
				}
				catch
				{
					flag = false;
				}
				if (flag && resultData != null)
				{
					return await EndOperateAsync(status: true, null, resultData, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendWaitAsync", 433).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("接收数据超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendWaitAsync", 435).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "SendWaitAsync", 439).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				UdpClient communication2 = _communication;
				if (communication2 != null && communication2.Client != null)
				{
					StartMonitor();
				}
			}
		}

		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "GetBaseObjectAsync", 454);
			}
			return await EndOperateAsync(status: true, null, _communication, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "GetBaseObjectAsync", 456).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 获取当前连接状态。
		/// </summary>
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_communication != null && _communication.Client != null)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("已连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "GetStatusAsync", 466).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\unicast\\client\\UdpClientOperate.cs", "GetStatusAsync", 468).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.udp.multicast
{
	public class UdpMulticastData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 本地绑定端口（接收数据用）<br />
			/// 自动绑定 IPAddress.Any（所有网络接口），组播加入所有接口
			/// </summary>
			[Description("本地绑定端口")]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 组播组地址
			/// </summary>
			[Description("组播组地址")]
			public string MulticastAddress { get; set; } = "239.0.0.1";

			/// <summary>
			/// 组播端口（发送组播数据的目标端口）<br />
			/// 为 0 时自动取 Port 的值，即发送和接收使用同一端口
			/// </summary>
			[Description("组播端口")]
			public int MulticastPort { get; set; } = 8866;

			/// <summary>
			/// 有效的组播端口：如果 MulticastPort 未设置则等于 Port
			/// </summary>
			internal int EffectiveMulticastPort
			{
				get
				{
					if (MulticastPort <= 0)
					{
						return Port;
					}
					return MulticastPort;
				}
			}

			/// <summary>
			/// 生存时间 (TTL)，控制组播数据包经过的路由器跳数<br />
			/// 0: 仅本机, 1: 本地子网, 默认 1
			/// </summary>
			[Description("生存时间(TTL)")]
			public short TimeToLive { get; set; } = 1;

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 发送等待间隔<br />
			/// 继承者可选重写
			/// </summary>
			[Description("发送等待间隔")]
			public virtual int SendWaitInterval { get; set; } = 5000;

			/// <summary>
			/// 最大块大小<br />
			/// 如果数据超过限定值则自动分包发送<br />
			/// 继承者可选重写
			/// </summary>
			[Description("最大块大小")]
			public virtual int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数<br />
			/// 只在分包发送中触发<br />
			/// 在限定次数中还是失败则直接返回失败<br />
			/// 继承者可选重写
			/// </summary>
			[Description("重试发送次数")]
			public virtual int RetrySendCount { get; set; } = 5;
		}
	}
	/// <summary>
	/// UDP组播通信；<br />
	/// 提供 UDP 组播发送和接收功能；<br />
	/// 支持加入/离开组播组、自动分包发送、发送等待响应、数据监控回调。
	/// <para>
	/// 【并发模型说明】<br />
	/// - 本类采用「单一读循环 + 请求-响应队列」架构；<br />
	/// - MonitorAsync 是唯一从 UdpClient 读取数据的协程，消除了并发读竞争；<br />
	/// - SendWaitAsync 通过 ConcurrentQueue&lt;TaskCompletionSource&gt; 注册等待项；<br />
	/// - _commLock（SemaphoreSlim）仅保护 Communication 引用本身的读写。
	/// </para>
	/// <para>
	/// 【组播特性】<br />
	/// - 绑定本地地址和端口后加入组播组；<br />
	/// - 发送数据到组播组地址；<br />
	/// - 接收组播组内的消息；<br />
	/// - 关闭时自动离开组播组。
	/// </para>
	/// </summary>
	public class UdpMulticastOperate : CommunicationAbstract<UdpMulticastOperate, UdpMulticastData.Basics>, ICommunication, IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// UDP 通信实例，受 _commLock 保护。
		/// </summary>
		private UdpClient? _communication;

		/// <summary>
		/// 监控读循环的取消令牌源。
		/// </summary>
		private CancellationTokenSource? _monitorCts;

		/// <summary>
		/// 是否已加入组播组。
		/// </summary>
		private bool _isJoined;

		public UdpMulticastOperate(UdpMulticastData.Basics basics)
			: base(basics)
		{
		}

		private async Task CloseUnderLockAsync()
		{
			await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
			if (_isJoined && _communication != null)
			{
				try
				{
					_communication.DropMulticastGroup(IPAddress.Parse(base.basics.MulticastAddress));
				}
				catch
				{
				}
				_isJoined = false;
			}
			try
			{
				_communication?.Client?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Client?.Dispose();
			}
			catch
			{
			}
			try
			{
				_communication?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Dispose();
			}
			catch
			{
			}
			_communication = null;
		}

		private async Task StopMonitorAsync()
		{
			CancellationTokenSource cts = _monitorCts;
			if (cts != null)
			{
				_monitorCts = null;
				try
				{
					await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					cts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private void StartMonitor()
		{
			if (_monitorCts == null || _monitorCts.IsCancellationRequested)
			{
				_monitorCts = new CancellationTokenSource();
				MonitorAsync(_monitorCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 底层发送方法：向组播组地址发送单个数据块。
		/// </summary>
		private async Task<bool> SendPrivateAsync(byte[] data, CancellationToken token)
		{
			try
			{
				UdpClient communication = _communication;
				if (communication == null)
				{
					return false;
				}
				using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
				cts.CancelAfter(base.basics.Timeout);
				IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(base.basics.MulticastAddress), base.basics.EffectiveMulticastPort);
				return await communication.SendAsync(new ReadOnlyMemory<byte>(data), endPoint, cts.Token).ConfigureAwait(continueOnCapturedContext: false) > 0;
			}
			catch (OperationCanceledException)
			{
				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 自动分包发送（不含 BegOperateAsync/EndOperateAsync）。
		/// </summary>
		private async Task<bool> SendAllChunksAsync(byte[] data, CancellationToken token)
		{
			if (data.Length > base.basics.MaxChunkSize)
			{
				int maxChunkSize = base.basics.MaxChunkSize;
				int chunkSize;
				for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
				{
					chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
					byte[] chunk = data[bytesSent..(bytesSent + chunkSize)];
					bool chunkSent = false;
					for (int attempt = 0; attempt <= base.basics.RetrySendCount; attempt++)
					{
						if (await SendPrivateAsync(chunk, token).ConfigureAwait(continueOnCapturedContext: false))
						{
							chunkSent = true;
							break;
						}
						if (attempt < base.basics.RetrySendCount && !token.IsCancellationRequested)
						{
							await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					if (!chunkSent)
					{
						return false;
					}
				}
				return true;
			}
			for (int bytesSent = 0; bytesSent <= base.basics.RetrySendCount; bytesSent++)
			{
				if (await SendPrivateAsync(data, token).ConfigureAwait(continueOnCapturedContext: false))
				{
					return true;
				}
				if (bytesSent < base.basics.RetrySendCount && !token.IsCancellationRequested)
				{
					await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			return false;
		}

		private async Task MonitorAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					UdpClient communication = _communication;
					if (communication == null)
					{
						break;
					}
					using CancellationTokenSource receiveCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					receiveCts.CancelAfter((base.basics.Timeout > 0) ? base.basics.Timeout : (-1));
					UdpReceiveResult result = await communication.ReceiveAsync(receiveCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					if (result.Buffer.Length != 0)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnDataEventHandlerAsync(sender, new EventDataResult(true, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据", null, token).ConfigureAwait(continueOnCapturedContext: false), (object)result.Buffer)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				catch (SocketException ex4)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex4.Message))).ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
				catch (Exception ex5)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex5.Message))).ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
			}
		}

		/// <summary>
		/// 启动组播通信：绑定本地端口，加入组播组。
		/// </summary>
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "OnAsync", 206);
				}
				UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, base.basics.Port));
				udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
				udpClient.Client.ReceiveTimeout = base.basics.Timeout;
				udpClient.Client.SendTimeout = base.basics.Timeout;
				try
				{
					IPAddress multicastAddr = IPAddress.Parse(base.basics.MulticastAddress);
					udpClient.JoinMulticastGroup(multicastAddr, IPAddress.Any);
					udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, base.basics.TimeToLive);
				}
				catch
				{
					udpClient.Dispose();
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("加入组播组失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "OnAsync", 230).ConfigureAwait(continueOnCapturedContext: false);
				}
				_communication = udpClient;
				_isJoined = true;
				StartMonitor();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "OnAsync", 237).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "OnAsync", 242).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 关闭组播通信：离开组播组并关闭连接。
		/// </summary>
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "OffAsync", 256);
				}
				await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "OffAsync", 260).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "OffAsync", 264).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 发送组播数据（自动分包）。
		/// </summary>
		public override async Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendAsync", 278);
				}
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync((data.Length > base.basics.MaxChunkSize) ? "存在数据块发送失败" : "发送失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendAsync", 282).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendAsync", 284).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendAsync", 288).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 发送组播数据并等待响应。
		/// </summary>
		public override async Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token)
		{
			await BegOperateAsync(token, "SendWaitAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendWaitAsync", 303);
				}
				await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("发送等待结果失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendWaitAsync", 311).ConfigureAwait(continueOnCapturedContext: false);
				}
				UdpClient communication = _communication;
				if (communication == null || communication.Client == null)
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendWaitAsync", 319).ConfigureAwait(continueOnCapturedContext: false);
				}
				byte[] resultData = null;
				bool flag;
				try
				{
					using CancellationTokenSource readCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					readCts.CancelAfter(base.basics.SendWaitInterval);
					resultData = (await communication.ReceiveAsync(readCts.Token).ConfigureAwait(continueOnCapturedContext: false)).Buffer;
					flag = resultData.Length != 0;
				}
				catch (OperationCanceledException)
				{
					flag = false;
				}
				catch
				{
					flag = false;
				}
				if (flag && resultData != null)
				{
					return await EndOperateAsync(status: true, null, resultData, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendWaitAsync", 337).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("接收数据超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendWaitAsync", 339).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "SendWaitAsync", 343).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				UdpClient communication2 = _communication;
				if (communication2 != null && communication2.Client != null)
				{
					StartMonitor();
				}
			}
		}

		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "GetBaseObjectAsync", 358);
			}
			return await EndOperateAsync(status: true, null, _communication, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "GetBaseObjectAsync", 360).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 获取当前状态。
		/// </summary>
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_communication != null && _communication.Client != null)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("已连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "GetStatusAsync", 370).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\multicast\\UdpMulticastOperate.cs", "GetStatusAsync", 372).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.udp.broadcast
{
	public class UdpBroadcastData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 本地绑定端口（接收数据用）<br />
			/// 自动绑定 IPAddress.Any（所有网络接口）
			/// </summary>
			[Description("本地绑定端口")]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 广播地址
			/// </summary>
			[Description("广播地址")]
			public string BroadcastAddress { get; set; } = "255.255.255.255";

			/// <summary>
			/// 广播端口（发送广播数据的目标端口）<br />
			/// 为 0 时自动取 Port 的值，即发送和接收使用同一端口
			/// </summary>
			[Description("广播端口")]
			public int BroadcastPort { get; set; } = 8866;

			/// <summary>
			/// 有效的广播端口：如果 BroadcastPort 未设置则等于 Port
			/// </summary>
			internal int EffectiveBroadcastPort
			{
				get
				{
					if (BroadcastPort <= 0)
					{
						return Port;
					}
					return BroadcastPort;
				}
			}

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 发送等待间隔<br />
			/// 继承者可选重写
			/// </summary>
			[Description("发送等待间隔")]
			public virtual int SendWaitInterval { get; set; } = 5000;

			/// <summary>
			/// 最大块大小<br />
			/// 如果数据超过限定值则自动分包发送<br />
			/// 继承者可选重写
			/// </summary>
			[Description("最大块大小")]
			public virtual int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数<br />
			/// 只在分包发送中触发<br />
			/// 在限定次数中还是失败则直接返回失败<br />
			/// 继承者可选重写
			/// </summary>
			[Description("重试发送次数")]
			public virtual int RetrySendCount { get; set; } = 5;
		}
	}
	/// <summary>
	/// UDP广播通信；<br />
	/// 提供 UDP 广播发送和接收功能；<br />
	/// 支持自动分包发送、发送等待响应、数据监控回调。
	/// <para>
	/// 【并发模型说明】<br />
	/// - 本类采用「单一读循环 + 请求-响应队列」架构；<br />
	/// - MonitorAsync 是唯一从 UdpClient 读取数据的协程，消除了并发读竞争；<br />
	/// - SendWaitAsync 通过 ConcurrentQueue&lt;TaskCompletionSource&gt; 注册等待项；<br />
	/// - _commLock（SemaphoreSlim）仅保护 Communication 引用本身的读写。
	/// </para>
	/// <para>
	/// 【广播特性】<br />
	/// - 绑定本地地址和端口后启用 EnableBroadcast 选项；<br />
	/// - 发送数据到广播地址（默认 255.255.255.255）；<br />
	/// - 接收来自任何发送方的广播消息。
	/// </para>
	/// </summary>
	public class UdpBroadcastOperate : CommunicationAbstract<UdpBroadcastOperate, UdpBroadcastData.Basics>, ICommunication, IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// UDP 通信实例，受 _commLock 保护。
		/// </summary>
		private UdpClient? _communication;

		/// <summary>
		/// 监控读循环的取消令牌源。
		/// </summary>
		private CancellationTokenSource? _monitorCts;

		public UdpBroadcastOperate(UdpBroadcastData.Basics basics)
			: base(basics)
		{
		}

		private async Task CloseUnderLockAsync()
		{
			await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				_communication?.Client?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Client?.Dispose();
			}
			catch
			{
			}
			try
			{
				_communication?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Dispose();
			}
			catch
			{
			}
			_communication = null;
		}

		private async Task StopMonitorAsync()
		{
			CancellationTokenSource cts = _monitorCts;
			if (cts != null)
			{
				_monitorCts = null;
				try
				{
					await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					cts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private void StartMonitor()
		{
			if (_monitorCts == null || _monitorCts.IsCancellationRequested)
			{
				_monitorCts = new CancellationTokenSource();
				MonitorAsync(_monitorCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 底层发送方法：向广播地址发送单个数据块。
		/// </summary>
		private async Task<bool> SendPrivateAsync(byte[] data, CancellationToken token)
		{
			try
			{
				UdpClient communication = _communication;
				if (communication == null)
				{
					return false;
				}
				using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
				cts.CancelAfter(base.basics.Timeout);
				IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(base.basics.BroadcastAddress), base.basics.EffectiveBroadcastPort);
				return await communication.SendAsync(new ReadOnlyMemory<byte>(data), endPoint, cts.Token).ConfigureAwait(continueOnCapturedContext: false) > 0;
			}
			catch (OperationCanceledException)
			{
				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 自动分包发送
		/// </summary>
		private async Task<bool> SendAllChunksAsync(byte[] data, CancellationToken token)
		{
			if (data.Length > base.basics.MaxChunkSize)
			{
				int maxChunkSize = base.basics.MaxChunkSize;
				int chunkSize;
				for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
				{
					chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
					byte[] chunk = data[bytesSent..(bytesSent + chunkSize)];
					bool chunkSent = false;
					for (int attempt = 0; attempt <= base.basics.RetrySendCount; attempt++)
					{
						if (await SendPrivateAsync(chunk, token).ConfigureAwait(continueOnCapturedContext: false))
						{
							chunkSent = true;
							break;
						}
						if (attempt < base.basics.RetrySendCount && !token.IsCancellationRequested)
						{
							await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					if (!chunkSent)
					{
						return false;
					}
				}
				return true;
			}
			for (int bytesSent = 0; bytesSent <= base.basics.RetrySendCount; bytesSent++)
			{
				if (await SendPrivateAsync(data, token).ConfigureAwait(continueOnCapturedContext: false))
				{
					return true;
				}
				if (bytesSent < base.basics.RetrySendCount && !token.IsCancellationRequested)
				{
					await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			return false;
		}

		private async Task MonitorAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					UdpClient communication = _communication;
					if (communication == null)
					{
						break;
					}
					using CancellationTokenSource receiveCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					receiveCts.CancelAfter((base.basics.Timeout > 0) ? base.basics.Timeout : (-1));
					UdpReceiveResult result = await communication.ReceiveAsync(receiveCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					if (result.Buffer.Length != 0)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnDataEventHandlerAsync(sender, new EventDataResult(true, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据", null, token).ConfigureAwait(continueOnCapturedContext: false), (object)result.Buffer)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				catch (SocketException ex4)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex4.Message))).ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
				catch (Exception ex5)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex5.Message))).ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
			}
		}

		/// <summary>
		/// 启动广播通信：绑定本地端口并启用广播。
		/// </summary>
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "OnAsync", 187);
				}
				UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, base.basics.Port));
				udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
				udpClient.EnableBroadcast = true;
				udpClient.Client.ReceiveTimeout = base.basics.Timeout;
				udpClient.Client.SendTimeout = base.basics.Timeout;
				_communication = udpClient;
				StartMonitor();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "OnAsync", 203).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "OnAsync", 208).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 关闭广播通信。
		/// </summary>
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "OffAsync", 224);
				}
				await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "OffAsync", 229).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "OffAsync", 233).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 发送广播数据（自动分包）。
		/// </summary>
		public override async Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendAsync", 247);
				}
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync((data.Length > base.basics.MaxChunkSize) ? "存在数据块发送失败" : "发送失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendAsync", 251).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendAsync", 253).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendAsync", 257).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 发送广播数据并等待响应。
		/// </summary>
		public override async Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token)
		{
			await BegOperateAsync(token, "SendWaitAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_communication == null || _communication.Client == null)
			{
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendWaitAsync", 269).ConfigureAwait(continueOnCapturedContext: false);
			}
			try
			{
				await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("发送等待结果失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendWaitAsync", 278).ConfigureAwait(continueOnCapturedContext: false);
				}
				UdpClient communication = _communication;
				if (communication == null || communication.Client == null)
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendWaitAsync", 284).ConfigureAwait(continueOnCapturedContext: false);
				}
				byte[] resultData = null;
				bool flag;
				try
				{
					using CancellationTokenSource readCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					readCts.CancelAfter(base.basics.SendWaitInterval);
					resultData = (await communication.ReceiveAsync(readCts.Token).ConfigureAwait(continueOnCapturedContext: false)).Buffer;
					flag = resultData.Length != 0;
				}
				catch (OperationCanceledException)
				{
					flag = false;
				}
				catch
				{
					flag = false;
				}
				if (flag && resultData != null)
				{
					return await EndOperateAsync(status: true, null, resultData, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendWaitAsync", 300).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("接收数据超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendWaitAsync", 302).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "SendWaitAsync", 306).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				UdpClient communication2 = _communication;
				if (communication2 != null && communication2.Client != null)
				{
					StartMonitor();
				}
			}
		}

		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "GetBaseObjectAsync", 321);
			}
			return await EndOperateAsync(status: true, null, _communication, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "GetBaseObjectAsync", 323).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 获取当前状态。
		/// </summary>
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_communication != null && _communication.Client != null)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("已连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "GetStatusAsync", 334).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\udp\\broadcast\\UdpBroadcastOperate.cs", "GetStatusAsync", 336).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.tcp.service
{
	public class TcpServiceData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// Ip地址
			/// </summary>
			[Description("Ip地址")]
			public string? IpAddress { get; set; } = "127.0.0.1";

			/// <summary>
			/// 端口
			/// </summary>
			[Description("端口")]
			public int Port { get; set; } = 6688;

			/// <summary>
			/// 最大连接数
			/// </summary>
			[Description("最大连接数")]
			public int MaxNumber { get; set; } = 1000;

			/// <summary>
			/// 最大块大小;
			/// 如果数据超过限定值则自动分包发送
			/// </summary>
			[Description("最大块大小")]
			public int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数;
			/// 只在分包发送中触发;
			/// 在限定次数中还是失败则直接返回失败
			/// </summary>
			[Description("重试发送次数")]
			public int RetrySendCount { get; set; } = 5;

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 数据缓冲区大小
			/// </summary>
			[Description("数据缓冲区大小")]
			public int BufferSize { get; set; } = 1048576;
		}
	}
	/// <summary>
	/// TCP 服务端通信；<br />
	/// 提供 TCP 监听、客户端管理、群发/定向发送、消息监控等功能；<br />
	/// 使用 ConcurrentDictionary 管理客户端连接，支持多客户端并发访问。
	/// </summary>
	public class TcpServiceOperate(TcpServiceData.Basics basics) : CommunicationServiceAbstract<TcpServiceOperate, TcpServiceData.Basics>(basics), ICommunicationService, IOn, IOff, ISendService, IObject, IStatus, IRemove, IEvent, IInstance, ILog, ILanguage, IClone, IArgs, IDisposable, IAsyncDisposable
	{
		/// <summary>TCP 监听器</summary>
		private TcpListener? _service;

		/// <summary>监听连接的取消令牌源。</summary>
		private CancellationTokenSource? _acceptCts;

		/// <summary>AcceptLoop 的任务引用，用于优雅退出时等待。</summary>
		private Task? _acceptTask;

		/// <summary>客户端管理容器（线程安全）。</summary>
		private readonly ConcurrentDictionary<string, ClientManage<TcpClient>> _clients = new ConcurrentDictionary<string, ClientManage<TcpClient>>();

		/// <summary>
		/// 监控客户端连接（Accept 循环）。
		/// </summary>
		private async Task AcceptLoopAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					TcpClient tcpClient = await _service.AcceptTcpClientAsync(token).ConfigureAwait(continueOnCapturedContext: false);
					string text = tcpClient.Client.RemoteEndPoint.ToString();
					CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
					ClientManage<TcpClient> session = new ClientManage<TcpClient>(tcpClient, cancellationTokenSource, Task.CompletedTask);
					session.MonitorTask = MonitorClientMessagesAsync(session, text, cancellationTokenSource.Token);
					_clients.AddOrUpdate(text, session, delegate(string k, ClientManage<TcpClient> oldSession)
					{
						oldSession.Dispose();
						return session;
					});
					await OnDataEventHandlerAsync(this, new EventDataResult(true, "[" + text + "]连接成功", (object)new ClientMessage
					{
						Step = Steps.客户端连接,
						IpPort = text
					})).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				catch (HttpListenerException)
				{
					break;
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex5)
				{
					await OnInfoEventHandlerAsync(this, new EventInfoResult(false, "[" + base.TAG + "]监控客户端连接异常：" + ex5.Message)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		/// <summary>
		/// 监控单个客户端的消息。
		/// </summary>
		private async Task MonitorClientMessagesAsync(ClientManage<TcpClient> session, string ipPort, CancellationToken token)
		{
			byte[] buffer = new byte[base.basics.BufferSize];
			try
			{
				while (!token.IsCancellationRequested)
				{
					try
					{
						int num = await session.Client.GetStream().ReadAsync(buffer, token).ConfigureAwait(continueOnCapturedContext: false);
						if (num > 0)
						{
							await OnDataEventHandlerAsync(this, new EventDataResult(true, "[" + ipPort + "]数据接收成功", (object)new ClientMessage
							{
								Step = Steps.消息接收,
								IpPort = ipPort,
								Bytes = buffer[..num]
							})).ConfigureAwait(continueOnCapturedContext: false);
							continue;
						}
						await OnDataEventHandlerAsync(this, new EventDataResult(false, "[" + ipPort + "]客户端断开连接（数据长度为零）", (object)new ClientMessage
						{
							Step = Steps.客户端断开,
							IpPort = ipPort
						})).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (TaskCanceledException)
					{
						continue;
					}
					catch (OperationCanceledException)
					{
						continue;
					}
					catch (Exception ex3)
					{
						await OnDataEventHandlerAsync(this, new EventDataResult(false, "监控[" + ipPort + "]消息异常：" + ex3.Message, (object)new ClientMessage
						{
							Step = Steps.客户端断开,
							IpPort = ipPort
						})).ConfigureAwait(continueOnCapturedContext: false);
					}
					return;
				}
			}
			finally
			{
				await DisconnectClientAsync(ipPort, notify: false).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 安全断开单个客户端：从字典移除 → 取消 CTS → 释放资源。
		/// </summary>
		private async Task DisconnectClientAsync(string ipPort, bool notify = true)
		{
			if (_clients.TryRemove(ipPort, out ClientManage<TcpClient> value))
			{
				value.Dispose();
				try
				{
					await value.MonitorTask.ConfigureAwait(continueOnCapturedContext: false);
				}
				catch
				{
				}
			}
			if (notify)
			{
				await OnDataEventHandlerAsync(this, new EventDataResult(false, "[" + ipPort + "]已断开连接", (object)new ClientMessage
				{
					Step = Steps.客户端断开,
					IpPort = ipPort
				})).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		private static async Task<int> SendViaStreamAsync(TcpClient tcpClient, byte[] data, int offset, int count, CancellationToken token)
		{
			using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
			cts.CancelAfter(5000);
			await tcpClient.GetStream().WriteAsync(data.AsMemory(offset, count), cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			return count;
		}

		private async Task<bool> SendChunkedAsync(TcpClient tcpClient, byte[] data, CancellationToken token)
		{
			int maxChunkSize = base.basics.MaxChunkSize;
			int chunkSize;
			for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
			{
				chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
				bool sent = false;
				for (int attempt = 0; attempt <= base.basics.RetrySendCount; attempt++)
				{
					try
					{
						await SendViaStreamAsync(tcpClient, data, bytesSent, chunkSize, token).ConfigureAwait(continueOnCapturedContext: false);
						sent = true;
					}
					catch when (attempt < base.basics.RetrySendCount && !token.IsCancellationRequested)
					{
						await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
						continue;
					}
					break;
				}
				if (!sent)
				{
					return false;
				}
			}
			return true;
		}

		private async Task<(string IpPort, bool Success, string? Error)> SendToClientAsync(ClientManage<TcpClient> session, string ipPort, byte[] data, CancellationToken token)
		{
			_ = 1;
			try
			{
				bool flag;
				if (data.Length > base.basics.MaxChunkSize)
				{
					flag = await SendChunkedAsync(session.Client, data, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					await SendViaStreamAsync(session.Client, data, 0, data.Length, token).ConfigureAwait(continueOnCapturedContext: false);
					flag = true;
				}
				return (IpPort: ipPort, Success: flag, Error: flag ? null : "部分数据块发送失败");
			}
			catch (Exception ex)
			{
				return (IpPort: ipPort, Success: false, Error: ex.Message);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "OnAsync", 209);
				}
				_service = new TcpListener(new IPEndPoint(IPAddress.Parse(base.basics.IpAddress), base.basics.Port));
				_service.Server.ReceiveTimeout = base.basics.Timeout;
				_service.Server.SendTimeout = base.basics.Timeout;
				_service.Start(base.basics.MaxNumber);
				_acceptCts?.Cancel();
				_acceptCts?.Dispose();
				_acceptCts = new CancellationTokenSource();
				_acceptTask = AcceptLoopAsync(_acceptCts.Token);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "OnAsync", 223).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "OnAsync", 228).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "OffAsync", 242);
				}
				CancellationTokenSource acceptCts = _acceptCts;
				if (acceptCts != null)
				{
					_acceptCts = null;
					acceptCts.Cancel();
					acceptCts.Dispose();
				}
				Task acceptTask = _acceptTask;
				if (acceptTask != null && !acceptTask.IsCompleted)
				{
					try
					{
						using CancellationTokenSource timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3.0));
						await acceptTask.WaitAsync(timeoutCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				}
				_acceptTask = null;
				KeyValuePair<string, ClientManage<TcpClient>>[] array = _clients.ToArray();
				_clients.Clear();
				KeyValuePair<string, ClientManage<TcpClient>>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					KeyValuePair<string, ClientManage<TcpClient>> kvp = array2[i];
					try
					{
						ClientManage<TcpClient> value = kvp.Value;
						value.Dispose();
						try
						{
							await value.MonitorTask.ConfigureAwait(continueOnCapturedContext: false);
						}
						catch
						{
						}
					}
					catch (Exception ex)
					{
						await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"[{base.TAG}]关闭客户端[{kvp.Key}]时异常：{ex.Message}")).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				try
				{
					_service?.Stop();
				}
				catch
				{
				}
				_service = null;
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "OffAsync", 285).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "OffAsync", 289).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> RemoveAsync(string[] address, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "RemoveAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				List<string> failed = new List<string>();
				foreach (string ipPort in address)
				{
					try
					{
						await DisconnectClientAsync(ipPort).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex)
					{
						failed.Add("[" + ipPort + "]: " + ex.Message);
					}
				}
				if (failed.Count > 0)
				{
					return await EndOperateAsync(status: false, string.Join("; ", failed), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "RemoveAsync", 314).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "RemoveAsync", 316).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, ex2, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "RemoveAsync", 320).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, string[]? address = null, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "SendAsync", 332);
				}
				if (address == null)
				{
					if (_clients.IsEmpty)
					{
						return await EndOperateAsync(status: false, "客户端未连接", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "SendAsync", 338).ConfigureAwait(continueOnCapturedContext: false);
					}
					List<string> list = (from r in await Task.WhenAll(_clients.Select<KeyValuePair<string, ClientManage<TcpClient>>, Task<(string, bool, string)>>((KeyValuePair<string, ClientManage<TcpClient>> kvp) => SendToClientAsync(kvp.Value, kvp.Key, data, token)).ToList()).ConfigureAwait(continueOnCapturedContext: false)
						where !r.Success
						select "[" + r.IpPort + "]: " + r.Error).ToList();
					if (list.Count > 0)
					{
						return await EndOperateAsync(status: false, ExtendMethod.ToJson<List<string>>(list, false), list, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "SendAsync", 346).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else
				{
					List<string> list2 = (from r in await Task.WhenAll(address.Select((string ipPort) => (Task<(string, bool, string)>)(_clients.TryGetValue(ipPort, out ClientManage<TcpClient> value) ? ((Task)SendToClientAsync(value, ipPort, data, token)) : ((Task)Task.FromResult((ipPort, false, "客户端[" + ipPort + "]不存在"))))).ToList()).ConfigureAwait(continueOnCapturedContext: false)
						where !r.Item2
						select "[" + r.Item1 + "]: " + r.Item3).ToList();
					if (list2.Count > 0)
					{
						return await EndOperateAsync(status: false, ExtendMethod.ToJson<List<string>>(list2, false), list2, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "SendAsync", 362).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "SendAsync", 365).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "SendAsync", 369).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override async Task<OperateResult> SendAsync(byte[] data, string address, CancellationToken token = default(CancellationToken))
		{
			return await SendAsync(data, new string[1] { address }, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "GetBaseObjectAsync", 386);
			}
			return await EndOperateAsync(status: true, null, _service, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "GetBaseObjectAsync", 389).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_service != null)
			{
				return await EndOperateAsync(status: true, "已启动", null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "GetStatusAsync", 398).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, "未启动", null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\service\\TcpServiceOperate.cs", "GetStatusAsync", 400).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.tcp.client
{
	public class TcpClientData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

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
			/// 是否需要断开重新连接
			/// </summary>
			[Description("是否需要断开重新连接")]
			public bool InterruptReconnection { get; set; } = true;

			/// <summary>
			/// 重连间隔(毫秒)
			/// </summary>
			[Description("重连间隔(毫秒)")]
			public int ReconnectionInterval { get; set; } = 1000;

			/// <summary>
			/// 超时时间(毫秒)
			/// </summary>
			[Description("超时时间(毫秒)")]
			public int Timeout { get; set; } = 1000;

			/// <summary>
			/// 发送等待间隔<br />
			/// 继承者可选重写
			/// </summary>
			[Description("发送等待间隔")]
			public virtual int SendWaitInterval { get; set; } = 5000;

			/// <summary>
			/// 最大块大小；<br />
			/// 如果数据超过限定值则自动分包发送<br />
			/// 继承者可选重写
			/// </summary>
			[Description("最大块大小")]
			public virtual int MaxChunkSize { get; set; } = 261120;

			/// <summary>
			/// 重试发送次数；<br />
			/// 只在分包发送中触发；<br />
			/// 在限定次数中还是失败则直接返回失败<br />
			/// 继承者可选重写
			/// </summary>
			[Description("重试发送次数")]
			public virtual int RetrySendCount { get; set; } = 5;

			/// <summary>
			/// 数据缓冲区大小<br />
			/// 继承者可选重写
			/// </summary>
			[Description("数据缓冲区大小")]
			public virtual int BufferSize { get; set; } = 1048576;
		}
	}
	/// <summary>
	/// TCP客户端通信；<br />
	/// 提供 TCP 连接、发送、接收、断线重连等功能；<br />
	/// 支持自动分包发送、发送等待响应、数据监控回调。
	/// <para>
	/// 【并发模型说明】<br />
	/// 本类采用「单一读循环 + 请求-响应队列」架构：<br />
	/// - MonitorAsync 是唯一从 NetworkStream 读取数据的协程，消除了并发读竞争；<br />
	/// - SendWaitAsync 停止 MonitorAsync 后独占读取，消除竞态窗口；<br />
	/// - _commLock（SemaphoreSlim）仅保护 Communication 引用本身的读写，<br />
	///   不保护 NetworkStream 上的 I/O 操作（I/O 由单一协程独占）。
	/// </para>
	/// </summary>
	public class TcpClientOperate : CommunicationAbstract<TcpClientOperate, TcpClientData.Basics>, ICommunication, IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// TCP 通信实例
		/// </summary>
		private TcpClient? _communication;

		/// <summary>
		/// 监控读循环的取消令牌源。
		/// </summary>
		private CancellationTokenSource? _monitorCts;

		/// <summary>
		/// 断线重连循环的取消令牌源。
		/// </summary>
		private CancellationTokenSource? _reconnectCts;

		public TcpClientOperate(TcpClientData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 核心关闭逻辑：停止监控 → 停止重连 → 关闭 TCP 连接。
		/// </summary>
		private async Task CloseUnderLockAsync()
		{
			await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
			StopReconnection();
			try
			{
				_communication?.Client?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Client?.Dispose();
			}
			catch
			{
			}
			try
			{
				_communication?.Close();
			}
			catch
			{
			}
			try
			{
				_communication?.Dispose();
			}
			catch
			{
			}
			_communication = null;
		}

		private async Task StopMonitorAsync()
		{
			CancellationTokenSource cts = _monitorCts;
			if (cts != null)
			{
				_monitorCts = null;
				try
				{
					await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					cts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private void StopReconnection()
		{
			CancellationTokenSource reconnectCts = _reconnectCts;
			if (reconnectCts != null)
			{
				_reconnectCts = null;
				try
				{
					reconnectCts.Cancel();
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					reconnectCts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private void StartMonitor()
		{
			if (_monitorCts == null || _monitorCts.IsCancellationRequested)
			{
				_monitorCts = new CancellationTokenSource();
				MonitorAsync(_monitorCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		private void StartReconnection()
		{
			if (base.basics.InterruptReconnection && (_reconnectCts == null || _reconnectCts.IsCancellationRequested))
			{
				_reconnectCts = new CancellationTokenSource();
				InterruptReconnectionAsync(_reconnectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 底层发送方法：向 NetworkStream 写入单个数据块。
		/// </summary>
		private async Task<bool> SendPrivateAsync(byte[] data, CancellationToken token)
		{
			try
			{
				TcpClient communication = _communication;
				if (communication == null)
				{
					return false;
				}
				using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
				cts.CancelAfter(base.basics.Timeout);
				await communication.GetStream().WriteAsync(data, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
				return true;
			}
			catch (OperationCanceledException)
			{
				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 自动分包发送
		/// </summary>
		private async Task<bool> SendAllChunksAsync(byte[] data, CancellationToken token)
		{
			if (data.Length > base.basics.MaxChunkSize)
			{
				int maxChunkSize = base.basics.MaxChunkSize;
				int chunkSize;
				for (int bytesSent = 0; bytesSent < data.Length; bytesSent += chunkSize)
				{
					chunkSize = Math.Min(data.Length - bytesSent, maxChunkSize);
					byte[] chunk = data[bytesSent..(bytesSent + chunkSize)];
					bool chunkSent = false;
					for (int attempt = 0; attempt <= base.basics.RetrySendCount; attempt++)
					{
						if (await SendPrivateAsync(chunk, token).ConfigureAwait(continueOnCapturedContext: false))
						{
							chunkSent = true;
							break;
						}
						if (attempt < base.basics.RetrySendCount && !token.IsCancellationRequested)
						{
							await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					if (!chunkSent)
					{
						return false;
					}
				}
				return true;
			}
			for (int bytesSent = 0; bytesSent <= base.basics.RetrySendCount; bytesSent++)
			{
				if (await SendPrivateAsync(data, token).ConfigureAwait(continueOnCapturedContext: false))
				{
					return true;
				}
				if (bytesSent < base.basics.RetrySendCount && !token.IsCancellationRequested)
				{
					await Task.Delay(10, token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			return false;
		}

		private async Task MonitorAsync(CancellationToken token)
		{
			byte[] buffer = new byte[base.basics.BufferSize];
			while (!token.IsCancellationRequested)
			{
				try
				{
					TcpClient communication = _communication;
					if (communication == null)
					{
						break;
					}
					int length = await communication.GetStream().ReadAsync(buffer, token).ConfigureAwait(continueOnCapturedContext: false);
					object sender;
					string tAG;
					if (length > 0)
					{
						sender = this;
						tAG = base.TAG;
						await OnDataEventHandlerAsync(sender, new EventDataResult(true, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据", null, token).ConfigureAwait(continueOnCapturedContext: false), (object)buffer[..length])).ConfigureAwait(continueOnCapturedContext: false);
						continue;
					}
					sender = this;
					tAG = base.TAG;
					await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + await GetLanguageValueAsync("监控数据错误", null, token).ConfigureAwait(continueOnCapturedContext: false))).ConfigureAwait(continueOnCapturedContext: false);
					await HandleDisconnectUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
					break;
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex3)
				{
					if (!token.IsCancellationRequested)
					{
						object sender = this;
						string tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, new EventInfoResult(false, "[" + tAG + "]" + string.Format(await GetLanguageValueAsync("监控异常", null, token).ConfigureAwait(continueOnCapturedContext: false), ex3.Message))).ConfigureAwait(continueOnCapturedContext: false);
						await HandleDisconnectUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					break;
				}
			}
		}

		private async Task HandleDisconnectUnderLockAsync()
		{
			await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
			if (base.basics.InterruptReconnection)
			{
				StartReconnection();
			}
		}

		private async Task InterruptReconnectionAsync(CancellationToken token)
		{
			int reconnectionCount = 1;
			try
			{
				while (!token.IsCancellationRequested)
				{
					try
					{
						int backoffMs = Math.Min(500 * (1 << Math.Min(reconnectionCount, 10)), 30000);
						bool flag = _communication == null || !IsTcpConnected(_communication);
						if (flag)
						{
							try
							{
								_communication?.Dispose();
							}
							catch
							{
							}
							_communication = new TcpClient
							{
								SendTimeout = base.basics.Timeout
							};
						}
						if (!flag)
						{
							goto IL_0538;
						}
						TcpClient commForConnect = _communication;
						if (commForConnect == null)
						{
							continue;
						}
						bool connected = false;
						try
						{
							using CancellationTokenSource connectCts = CancellationTokenSource.CreateLinkedTokenSource(token);
							connectCts.CancelAfter(base.basics.Timeout);
							await commForConnect.ConnectAsync(new IPEndPoint(IPAddress.Parse(base.basics.IpAddress), base.basics.Port), connectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
							connected = commForConnect.Connected;
						}
						catch
						{
							if (_communication == commForConnect)
							{
								try
								{
									_communication?.Dispose();
								}
								catch
								{
								}
								_communication = null;
							}
						}
						object sender;
						string tAG;
						if (!connected)
						{
							sender = this;
							tAG = base.TAG;
							await OnInfoEventHandlerAsync(sender, EventInfoResult.CreateFailureResult("[" + tAG + "]" + string.Format(await GetLanguageValueAsync("重连失败", null, token).ConfigureAwait(continueOnCapturedContext: false), reconnectionCount))).ConfigureAwait(continueOnCapturedContext: false);
							reconnectionCount++;
							await Task.Delay(backoffMs, token).ConfigureAwait(continueOnCapturedContext: false);
							continue;
						}
						sender = this;
						tAG = base.TAG;
						await OnInfoEventHandlerAsync(sender, EventInfoResult.CreateSuccessResult("[" + tAG + "]" + await GetLanguageValueAsync("重连成功", null, token).ConfigureAwait(continueOnCapturedContext: false))).ConfigureAwait(continueOnCapturedContext: false);
						reconnectionCount = 1;
						StartMonitor();
						goto IL_0538;
						IL_0538:
						await Task.Delay(base.basics.ReconnectionInterval, token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (TaskCanceledException)
					{
					}
					catch (OperationCanceledException)
					{
					}
					catch (Exception ex3)
					{
						object sender = this;
						string tAG = base.TAG;
						string text = await GetLanguageValueAsync("重连异常", null, token).ConfigureAwait(continueOnCapturedContext: false);
						await OnInfoEventHandlerAsync(sender, EventInfoResult.CreateFailureResult("[" + tAG + "]" + text + "：" + ex3.Message)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private static bool IsTcpConnected(TcpClient client)
		{
			try
			{
				if (client == null || !client.Connected)
				{
					return false;
				}
				if (client.Client.Poll(0, SelectMode.SelectRead))
				{
					byte[] buffer = new byte[1];
					return client.Client.Receive(buffer, SocketFlags.Peek) != 0;
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 建立 TCP 连接。
		/// </summary>
		public override async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "OnAsync", 310);
				}
				TcpClient comm = new TcpClient
				{
					SendTimeout = base.basics.Timeout
				};
				using CancellationTokenSource connectCts = CancellationTokenSource.CreateLinkedTokenSource(token);
				connectCts.CancelAfter(base.basics.Timeout);
				try
				{
					await comm.ConnectAsync(new IPEndPoint(IPAddress.Parse(base.basics.IpAddress), base.basics.Port), connectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch
				{
					comm.Dispose();
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("连接超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "OnAsync", 323).ConfigureAwait(continueOnCapturedContext: false);
				}
				_communication = comm;
				if (base.basics.InterruptReconnection)
				{
					StopReconnection();
					_reconnectCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					InterruptReconnectionAsync(_reconnectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				StartMonitor();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "OnAsync", 336).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "OnAsync", 341).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 关闭 TCP 连接。
		/// </summary>
		public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "OffAsync", 357);
				}
				await CloseUnderLockAsync().ConfigureAwait(continueOnCapturedContext: false);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "OffAsync", 361).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "OffAsync", 365).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 发送数据（自动分包）。
		/// </summary>
		public override async Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SendAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendAsync", 379);
				}
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync((data.Length > base.basics.MaxChunkSize) ? "存在数据块发送失败" : "发送失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendAsync", 383).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendAsync", 385).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendAsync", 389).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public override async Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token)
		{
			await BegOperateAsync(token, "SendWaitAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendWaitAsync", 404);
				}
				await StopMonitorAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!(await SendAllChunksAsync(data, token).ConfigureAwait(continueOnCapturedContext: false)))
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("发送等待结果失败", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendWaitAsync", 412).ConfigureAwait(continueOnCapturedContext: false);
				}
				TcpClient communication = _communication;
				if (communication == null || !communication.Connected)
				{
					return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendWaitAsync", 418).ConfigureAwait(continueOnCapturedContext: false);
				}
				byte[] buffer = new byte[base.basics.BufferSize];
				int num;
				bool flag;
				try
				{
					using CancellationTokenSource readCts = CancellationTokenSource.CreateLinkedTokenSource(token);
					readCts.CancelAfter(base.basics.SendWaitInterval);
					num = await communication.GetStream().ReadAsync(buffer, readCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					flag = num > 0;
				}
				catch (OperationCanceledException)
				{
					flag = false;
					num = 0;
				}
				catch (Exception)
				{
					flag = false;
					num = 0;
				}
				if (flag)
				{
					return await EndOperateAsync(status: true, null, buffer[..num], null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendWaitAsync", 434).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, await GetLanguageValueAsync("接收数据超时", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendWaitAsync", 436).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex3)
			{
				return await EndOperateAsync(status: false, ex3.Message, null, ex3, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "SendWaitAsync", 440).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				TcpClient communication2 = _communication;
				if (communication2 != null && communication2.Connected)
				{
					StartMonitor();
				}
			}
		}

		public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetBaseObjectAsync").ConfigureAwait(continueOnCapturedContext: false);
			string message = default(string);
			if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
			{
				return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "GetBaseObjectAsync", 455);
			}
			return await EndOperateAsync(status: true, null, _communication, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "GetBaseObjectAsync", 457).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 获取当前连接状态。
		/// </summary>
		public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (_communication != null && _communication.Connected)
			{
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("已连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "GetStatusAsync", 468).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, await GetLanguageValueAsync("未连接", null, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\tcp\\client\\TcpClientOperate.cs", "GetStatusAsync", 470).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.http.service
{
	/// <summary>
	/// Http服务端数据
	/// </summary>
	public class HttpServiceData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics : WAModel
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 请求与响应的内容类型
			/// </summary>
			[Description("请求与响应的内容类型")]
			public string ContentType { get; set; } = "application/json";

			/// <summary>
			/// 接口的绝对路径集合
			/// </summary>
			[Description("接口的绝对路径集合")]
			public List<AbsolutePathsModel> AbsolutePaths { get; set; } = new List<AbsolutePathsModel>
			{
				new AbsolutePathsModel("/api/snet", HttpMethod.Get)
			};

			/// <summary>
			/// 快速设置API模型参数
			/// </summary>
			/// <param name="wAModel">WA模型</param>
			public void SET(WAModel wAModel)
			{
				((WAModel)this).CrossDomain = wAModel.CrossDomain;
				((WAModel)this).IpAddress = wAModel.IpAddress;
				((WAModel)this).Port = wAModel.Port;
			}
		}

		/// <summary>
		/// 绝对路径模型
		/// </summary>
		public class AbsolutePathsModel
		{
			/// <summary>
			/// 路径
			/// </summary>
			public string Path { get; set; }

			/// <summary>
			/// 请求与响应的方式
			/// </summary>
			public HttpMethod Method { get; set; }

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="path">路径</param>
			/// <param name="method">请求与响应的方式</param>
			public AbsolutePathsModel(string path, HttpMethod method)
			{
				Path = path;
				Method = method;
			}
		}

		/// <summary>
		/// 等待处理
		/// </summary>
		public class WaitHandler
		{
			/// <summary>
			/// 请求的数据
			/// </summary>
			public HttpListenerRequest Request { get; set; }

			/// <summary>
			/// 响应的数据
			/// </summary>
			public HttpListenerResponse Response { get; set; }

			/// <summary>
			/// Body数据
			/// </summary>
			public string? BodyData { get; set; }
		}
	}
	/// <summary>
	/// Http 服务端操作
	/// </summary>
	public class HttpServiceOperate : CoreUnify<HttpServiceOperate, HttpServiceData.Basics>, IOn, IOff, IStatus, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// Http监听器
		/// </summary>
		private HttpListener? _httpListener;

		/// <summary>
		/// 生命周期取消源
		/// </summary>
		private CancellationTokenSource? _cts;

		/// <summary>
		/// 请求监听任务
		/// </summary>
		private Task? _acceptLoopTask;

		/// <summary>有参构造函数</summary>
		public HttpServiceOperate(HttpServiceData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>无参构造函数</summary>
		public HttpServiceOperate()
		{
			base.basics = new HttpServiceData.Basics();
		}

		/// <summary>
		/// 设置跨域响应头
		/// </summary>
		private static void SetCorsHeaders(HttpListenerRequest request, HttpListenerResponse response)
		{
			string value = request.Headers["Origin"];
			if (!string.IsNullOrWhiteSpace(value))
			{
				response.Headers["Access-Control-Allow-Origin"] = value;
			}
			response.Headers["Access-Control-Allow-Headers"] = "Content-Type,Authorization,X-Requested-With";
			response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
			response.Headers["Access-Control-Allow-Credentials"] = "true";
			response.Headers["Access-Control-Max-Age"] = "3600";
			response.Headers["Vary"] = "Origin";
		}

		/// <summary>
		/// HttpListenerResponse 写入数据（实例版本）。
		/// </summary>
		public async Task WriteAsync(HttpListenerResponse response, object obj, CancellationToken token = default(CancellationToken))
		{
			if (obj != null)
			{
				string s = ExtendMethod.ToJson<object>(obj, false);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				response.ContentLength64 = bytes.Length;
				try
				{
					await response.OutputStream.WriteAsync(bytes, 0, bytes.Length, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				finally
				{
					await SafeCloseResponseAsync(response).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		/// <summary>
		/// 写入数据（静态泛型版本）。
		/// </summary>
		public static async Task WriteAsync<T>(HttpListenerResponse response, T obj, CancellationToken token = default(CancellationToken))
		{
			if (obj != null)
			{
				string s = ExtendMethod.ToJson<T>(obj, false);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				response.ContentLength64 = bytes.Length;
				try
				{
					await response.OutputStream.WriteAsync(bytes, 0, bytes.Length, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				finally
				{
					await SafeCloseResponseAsync(response).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		/// <summary>
		/// 异步安全关闭响应
		/// </summary>
		private static async ValueTask SafeCloseResponseAsync(HttpListenerResponse response)
		{
			if (response == null)
			{
				return;
			}
			try
			{
				await response.OutputStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
			}
			try
			{
				response.Close();
			}
			catch
			{
			}
		}

		/// <summary>
		/// 写入错误响应
		/// </summary>
		private static async Task WriteErrorAsync(HttpListenerResponse response, string json, CancellationToken token)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			response.StatusCode = 400;
			response.ContentLength64 = bytes.Length;
			try
			{
				await response.OutputStream.WriteAsync(bytes, 0, bytes.Length, token).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				try
				{
					response.Close();
				}
				catch
				{
				}
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OnAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "OnAsync", 158);
				}
				if (NetHandler.IsPortInUse(((WAModel)base.basics).Port))
				{
					return await EndOperateAsync(status: false, "端口被占用", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "OnAsync", 163).ConfigureAwait(continueOnCapturedContext: false);
				}
				string host = $"http://{((WAModel)base.basics).IpAddress}:{((WAModel)base.basics).Port}";
				HttpListener listener = new HttpListener();
				listener.Prefixes.Add(host + "/");
				listener.Start();
				foreach (HttpServiceData.AbsolutePathsModel absolutePath in base.basics.AbsolutePaths)
				{
					await OnInfoEventHandlerAsync(this, EventInfoResult.CreateSuccessResult($"{host}{absolutePath.Path} [{absolutePath.Method}]")).ConfigureAwait(continueOnCapturedContext: false);
				}
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
				_httpListener = listener;
				_cts = cancellationTokenSource;
				_acceptLoopTask = HandlerIncomingConnectionsAsync(listener, cancellationTokenSource.Token);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "OnAsync", 182).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				await OffAsync(hardClose: true);
				return await EndOperateAsync(status: false, ex.Message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "OnAsync", 187).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public OperateResult On()
		{
			return OnAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "OffAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				string message = default(string);
				if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref message))
				{
					return await EndOperateAsync(status: false, message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "OffAsync", 205);
				}
				CancellationTokenSource cts = _cts;
				HttpListener listener = _httpListener;
				Task loopTask = _acceptLoopTask;
				if (cts != null)
				{
					await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				try
				{
					listener?.Stop();
				}
				catch
				{
				}
				if (loopTask != null)
				{
					try
					{
						using CancellationTokenSource waitCts = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
						await loopTask.WaitAsync(waitCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (OperationCanceledException)
					{
					}
					catch
					{
					}
				}
				try
				{
					listener?.Close();
				}
				catch
				{
				}
				try
				{
					cts?.Dispose();
				}
				catch
				{
				}
				_httpListener = null;
				_cts = null;
				_acceptLoopTask = null;
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "OffAsync", 239).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex2)
			{
				return await EndOperateAsync(status: false, ex2.Message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "OffAsync", 243).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 接收客户端连接
		/// </summary>
		private async Task HandlerIncomingConnectionsAsync(HttpListener listener, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					HttpListenerContext httpListenerContext = await listener.GetContextAsync().ConfigureAwait(continueOnCapturedContext: false);
					HttpListenerRequest request = httpListenerContext.Request;
					HttpListenerResponse response = httpListenerContext.Response;
					response.ContentType = base.basics.ContentType;
					if (!((WAModel)base.basics).CrossDomain)
					{
						goto IL_0133;
					}
					SetCorsHeaders(request, response);
					if (!request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
					{
						goto IL_0133;
					}
					response.StatusCode = 200;
					response.ContentLength64 = 0L;
					response.Close();
					goto end_IL_0038;
					IL_0133:
					string text = null;
					HttpServiceData.AbsolutePathsModel absolutePathsModel = ((request.Url == null) ? null : base.basics.AbsolutePaths.FirstOrDefault((HttpServiceData.AbsolutePathsModel item) => item.Path == request.Url.AbsolutePath));
					if (absolutePathsModel == null)
					{
						text = ExtendMethod.ToJson<OperateResult>(OperateResult.CreateFailureResult("接口不存在", (int?)null), false);
					}
					else if (!string.Equals(request.HttpMethod, absolutePathsModel.Method.Method, StringComparison.OrdinalIgnoreCase))
					{
						text = ExtendMethod.ToJson<OperateResult>(OperateResult.CreateFailureResult($"请求方式错误，需要:{absolutePathsModel.Method}", (int?)null), false);
					}
					if (text != null)
					{
						await WriteErrorAsync(response, text, token);
						continue;
					}
					response.StatusCode = 200;
					await HandlerAsync(request, response);
					end_IL_0038:;
				}
				catch (Exception ex)
				{
					if (!token.IsCancellationRequested)
					{
						await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult(ex.Message));
					}
				}
			}
		}

		/// <summary>
		/// 把外来请求通过事件抛出，事件注册方处理请求。
		/// </summary>
		private async Task HandlerAsync(HttpListenerRequest request, HttpListenerResponse response)
		{
			_ = 1;
			try
			{
				using Stream body = request.InputStream;
				using StreamReader reader = new StreamReader(body, request.ContentEncoding);
				object sender = this;
				HttpServiceData.WaitHandler waitHandler = new HttpServiceData.WaitHandler();
				HttpServiceData.WaitHandler waitHandler2 = waitHandler;
				waitHandler2.BodyData = await reader.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false);
				waitHandler.Request = request;
				waitHandler.Response = response;
				await OnDataEventHandlerAsync(sender, new EventDataResult(true, "Api Request", (object)waitHandler)).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				try
				{
					if (response.OutputStream.CanWrite)
					{
						response.StatusCode = 500;
						response.ContentLength64 = 0L;
						response.OutputStream.Close();
					}
				}
				catch
				{
					try
					{
						response.OutputStream.Close();
					}
					catch
					{
					}
					try
					{
						response.Close();
					}
					catch
					{
					}
				}
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetStatusAsync").ConfigureAwait(continueOnCapturedContext: false);
			bool flag = _httpListener != null && _httpListener.IsListening;
			bool status = flag;
			return await EndOperateAsync(status, await GetStatusMessageAsync(flag, token).ConfigureAwait(continueOnCapturedContext: false), null, null, logOutput: false, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\service\\HttpServiceOperate.cs", "GetStatusAsync", 359).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <summary>获取状态消息（避免重复代码）</summary>
		private async Task<string> GetStatusMessageAsync(bool isRunning, CancellationToken token = default(CancellationToken))
		{
			return (!isRunning) ? (await GetLanguageValueAsync("未启动", null, token).ConfigureAwait(continueOnCapturedContext: false)) : (await GetLanguageValueAsync("已启动", null, token).ConfigureAwait(continueOnCapturedContext: false));
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			try
			{
				Off(hardClose: true);
			}
			catch
			{
			}
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			try
			{
				await OffAsync(hardClose: true).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
			}
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.communication.net.http.client
{
	/// <summary>
	/// HTTP数据
	/// </summary>
	public class HttpClientData
	{
		/// <summary>
		/// 基础数据
		/// </summary>
		public class Basics
		{
			/// <summary>
			/// 唯一标识符
			/// </summary>
			[Category("基础数据")]
			[Description("唯一标识符")]
			public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

			/// <summary>
			/// 最大连接数
			/// </summary>
			[Description("最大连接数")]
			public int MaxConnectCount { get; set; } = 255;
		}

		/// <summary>
		/// 内容数据类型
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum BType
		{
			/// <summary>
			/// 表单形式传输数据；<br />
			/// ContentType:multipart/form-data
			/// </summary>
			[Description("表单形式传输数据")]
			FormData,
			/// <summary>
			/// 原始数据形式传输数据；<br />
			/// 基本使用JSON数据传输；<br />
			/// ContentType:application/json
			/// </summary>
			[Description("原始数据形式传输数据")]
			Raw,
			/// <summary>
			/// 无内容
			/// </summary>
			[Description("无内容")]
			None
		}

		/// <summary>
		/// 请求的数据
		/// </summary>
		public class RequestData
		{
			/// <summary>
			/// 请求的链接
			/// </summary>
			public string Url { get; set; } = "http://127.0.0.1:6688/api/snet";

			/// <summary>
			/// 请求标头
			/// </summary>
			public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

			/// <summary>
			/// 请求方式
			/// </summary>
			public HttpMethod Method { get; set; } = HttpMethod.Get;

			/// <summary>
			/// 内容数据类型
			/// </summary>
			public BType BodyType { get; set; } = BType.Raw;

			/// <summary>
			/// 内容；<br />
			/// string，配合 Raw 类型；<br />
			/// Dictionary＜string, string＞，配合 FormData 类型
			/// </summary>
			public object BodyContent { get; set; }

			/// <summary>
			/// 编码格式
			/// </summary>
			public Encoding Encoding { get; set; } = System.Text.Encoding.UTF8;

			/// <summary>
			/// 内容类型
			/// </summary>
			public string ContentType { get; set; } = "application/json";

			/// <summary>
			/// 超时时间
			/// </summary>
			public TimeSpan TimeOut { get; set; } = new TimeSpan(0, 0, 60);

			/// <summary>
			/// 代理
			/// </summary>
			public IWebProxy? Proxy { get; set; }

			/// <summary>
			/// 缓存
			/// </summary>
			public CookieContainer CookieContainer { get; set; } = new CookieContainer();
		}

		/// <summary>
		/// 响应数据
		/// </summary>
		public class ResponseData
		{
			/// <summary>
			/// 状态码
			/// </summary>
			public int StatusCode { get; set; }

			/// <summary>
			/// 响应头数据集合
			/// </summary>
			public Dictionary<string, string>? ResHeadersDatas { get; set; }

			/// <summary>
			/// 响应的COOKIE
			/// </summary>
			public CookieCollection ResCookieData { get; set; } = new CookieCollection();

			/// <summary>
			/// 响应的数据
			/// </summary>
			public string? ResData { get; set; }

			/// <summary>
			/// 请求的数据
			/// </summary>
			public RequestData ReqData { get; set; }
		}
	}
	/// <summary>
	/// HTTP客户端操作
	/// </summary>
	public class HttpClientOperate : CoreUnify<HttpClientOperate, HttpClientData.Basics>, IDisposable, IAsyncDisposable
	{
		/// <summary>已释放标记</summary>
		private volatile bool _disposed;

		/// <summary>有参构造函数</summary>
		public HttpClientOperate(HttpClientData.Basics basics)
			: base(basics)
		{
		}

		/// <summary>无参构造函数</summary>
		public HttpClientOperate()
		{
			base.basics = new HttpClientData.Basics();
		}

		/// <summary>
		/// 创建 HttpClient。
		/// </summary>
		private HttpClient CreateClient(HttpClientData.RequestData requestData)
		{
			return new HttpClient(new SocketsHttpHandler
			{
				UseCookies = true,
				CookieContainer = requestData.CookieContainer,
				UseProxy = (requestData.Proxy != null),
				Proxy = requestData.Proxy,
				MaxConnectionsPerServer = base.basics.MaxConnectCount,
				PooledConnectionLifetime = TimeSpan.FromMinutes(5.0)
			})
			{
				Timeout = requestData.TimeOut
			};
		}

		/// <summary>请求</summary>
		public async Task<OperateResult> RequestAsync(HttpClientData.RequestData requestData, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "RequestAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				using HttpClient httpClient = CreateClient(requestData);
				using HttpRequestMessage request = new HttpRequestMessage(requestData.Method, requestData.Url);
				if (requestData.Headers != null)
				{
					foreach (KeyValuePair<string, string> header in requestData.Headers)
					{
						if (!IsContentHeader(header.Key))
						{
							request.Headers.TryAddWithoutValidation(header.Key, header.Value);
						}
					}
				}
				switch (requestData.BodyType)
				{
				case HttpClientData.BType.FormData:
					await SetupFormDataContentAsync(request, requestData).ConfigureAwait(continueOnCapturedContext: false);
					break;
				case HttpClientData.BType.Raw:
					SetupRawContent(request, requestData);
					break;
				}
				using HttpResponseMessage response = await httpClient.SendAsync(request, token).ConfigureAwait(continueOnCapturedContext: false);
				Dictionary<string, string> responseHeaders = CollectResponseHeaders(response);
				CookieCollection cookies = CollectCookies(response, new Uri(requestData.Url));
				using StreamReader streamReader = new StreamReader(await response.Content.ReadAsStreamAsync(token).ConfigureAwait(continueOnCapturedContext: false), requestData.Encoding);
				string responseData = await streamReader.ReadToEndAsync(token).ConfigureAwait(continueOnCapturedContext: false);
				int stateCode = (int)response.StatusCode;
				return await EndOperateAsync(status: true, await GetLanguageValueAsync("请求成功", null, token).ConfigureAwait(continueOnCapturedContext: false), new HttpClientData.ResponseData
				{
					ResData = responseData,
					ResHeadersDatas = responseHeaders,
					StatusCode = stateCode,
					ReqData = requestData,
					ResCookieData = cookies
				}, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\client\\HttpClientOperate.cs", "RequestAsync", 99).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\communication\\net\\http\\client\\HttpClientOperate.cs", "RequestAsync", 115).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>请求（同步）</summary>
		public OperateResult Request(HttpClientData.RequestData requestData)
		{
			return RequestAsync(requestData).GetAwaiter().GetResult();
		}

		/// <summary>构建 FormData 请求体</summary>
		private static async Task SetupFormDataContentAsync(HttpRequestMessage request, HttpClientData.RequestData requestData)
		{
			MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
			bool flag = false;
			try
			{
				if (requestData.BodyContent != null)
				{
					if (!(requestData.BodyContent is Dictionary<string, string> dictionary))
					{
						throw new InvalidCastException("BodyType 为 FormData 时，BodyContent 必须为 Dictionary<string, string>，实际类型为 " + requestData.BodyContent.GetType().Name);
					}
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						multipartFormDataContent.Add(new StringContent(item.Value), item.Key);
					}
				}
				if (multipartFormDataContent.Count() > 0)
				{
					multipartFormDataContent.Headers.ContentType.MediaType = requestData.ContentType;
					request.Content = multipartFormDataContent;
					flag = true;
				}
			}
			finally
			{
				if (!flag)
				{
					multipartFormDataContent.Dispose();
				}
			}
		}

		/// <summary>构建 Raw 请求体</summary>
		private static void SetupRawContent(HttpRequestMessage request, HttpClientData.RequestData requestData)
		{
			if (requestData.BodyContent is string text && !string.IsNullOrEmpty(text))
			{
				request.Content = new StringContent(text, requestData.Encoding, requestData.ContentType);
			}
		}

		/// <summary>收集响应头</summary>
		private static Dictionary<string, string> CollectResponseHeaders(HttpResponseMessage response)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
			{
				dictionary[header.Key] = string.Join(", ", header.Value);
			}
			foreach (KeyValuePair<string, IEnumerable<string>> header2 in response.Content.Headers)
			{
				dictionary[header2.Key] = string.Join(", ", header2.Value);
			}
			return dictionary;
		}

		/// <summary>从响应中提取 Cookie</summary>
		private static CookieCollection CollectCookies(HttpResponseMessage response, Uri requestUri)
		{
			CookieCollection cookieCollection = new CookieCollection();
			if (!response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
			{
				return cookieCollection;
			}
			string host = requestUri.Host;
			foreach (string item in values)
			{
				if (string.IsNullOrWhiteSpace(item))
				{
					continue;
				}
				int num = item.IndexOf(';');
				string text = ((num >= 0) ? item.Substring(0, num).Trim() : item.Trim());
				int num2 = text.IndexOf('=');
				if (num2 < 0)
				{
					continue;
				}
				string name = text.Substring(0, num2).Trim();
				string value = text.Substring(num2 + 1).Trim();
				string path = "/";
				string domain = host;
				if (num >= 0)
				{
					string[] array = item.Substring(num + 1).Split(';', StringSplitOptions.TrimEntries);
					foreach (string text2 in array)
					{
						if (text2.StartsWith("path=", StringComparison.OrdinalIgnoreCase))
						{
							path = text2.Substring(5).Trim();
						}
						else if (text2.StartsWith("domain=", StringComparison.OrdinalIgnoreCase))
						{
							domain = text2.Substring(7).Trim().TrimStart('.');
						}
					}
				}
				try
				{
					cookieCollection.Add(new Cookie(name, value, path, domain));
				}
				catch (CookieException)
				{
				}
			}
			return cookieCollection;
		}

		/// <summary>判断是否为 HTTP 内容头</summary>
		private static bool IsContentHeader(string headerName)
		{
			if (!headerName.Equals("Content-Type", StringComparison.OrdinalIgnoreCase) && !headerName.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) && !headerName.Equals("Content-Encoding", StringComparison.OrdinalIgnoreCase) && !headerName.Equals("Content-Language", StringComparison.OrdinalIgnoreCase) && !headerName.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase))
			{
				return headerName.Equals("Content-Range", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				base.Dispose();
			}
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			if (!_disposed)
			{
				_disposed = true;
				await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}
}
namespace Snet.Core.communication.net.@enum
{
	/// <summary>
	/// 步骤
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum Steps
	{
		客户端连接,
		客户端断开,
		消息接收
	}
}
namespace Snet.Core.communication.net.core
{
	/// <summary>
	/// 客户端管理类；<br />
	/// 封装客户端连接、取消令牌、监控任务的生命周期管理。<br />
	/// 泛型参数 T 无约束，同时支持 TcpClient 和 WebSocketContext。
	/// </summary>
	public class ClientManage<T> : IDisposable where T : class
	{
		/// <summary>
		/// 开关（建议通过构造函数设置，避免外部随意修改导致泄漏）
		/// </summary>
		public CancellationTokenSource? Cts { get; private set; }

		/// <summary>
		/// 监控任务
		/// </summary>
		public Task MonitorTask { get; set; }

		/// <summary>
		/// 客户端
		/// </summary>
		public T? Client { get; private set; }

		/// <summary>
		/// 无参构造函数（用于对象初始化器语法）
		/// </summary>
		public ClientManage()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		public ClientManage(T client, CancellationTokenSource? cts, Task monitorTask)
		{
			Client = client;
			Cts = cts;
			MonitorTask = monitorTask;
		}

		/// <summary>
		/// 安全设置客户端对象和 CTS
		/// </summary>
		public void SetClient(T client, CancellationTokenSource? cts = null)
		{
			Client = client;
			Cts = cts;
		}

		public void Dispose()
		{
			if (Cts != null)
			{
				try
				{
					Cts.Cancel();
				}
				catch (ObjectDisposedException)
				{
				}
				try
				{
					Cts.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
				Cts = null;
			}
			if (Client == null)
			{
				return;
			}
			try
			{
				if (Client is TcpClient tcpClient)
				{
					try
					{
						tcpClient.Client?.Close();
					}
					catch
					{
					}
					try
					{
						tcpClient.Client?.Dispose();
					}
					catch
					{
					}
					try
					{
						tcpClient.Close();
						return;
					}
					catch
					{
						return;
					}
				}
				if (Client is WebSocketContext webSocketContext)
				{
					try
					{
						webSocketContext.WebSocket?.Abort();
					}
					catch
					{
					}
					try
					{
						webSocketContext.WebSocket?.Dispose();
						return;
					}
					catch
					{
						return;
					}
				}
			}
			catch
			{
			}
		}
	}
	/// <summary>
	/// 客户端消息
	/// </summary>
	public class ClientMessage
	{
		/// <summary>
		/// 步骤
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Steps Step { get; set; }

		/// <summary>
		/// IP地址:端口
		/// </summary>
		public string IpPort { get; set; }

		/// <summary>
		/// 字节数据
		/// </summary>
		public byte[]? Bytes { get; set; }
	}
}
namespace Snet.Core.channel
{
	/// <summary>
	/// 通道数据
	/// </summary>
	public class ChannelData
	{
		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Category("基础数据")]
		[Description("唯一标识符")]
		public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

		/// <summary>
		/// 通道最大容量
		/// </summary>
		[Description("通道最大容量")]
		public int Capacity { get; set; } = 65535;

		/// <summary>
		/// 只允许一个读取操作
		/// </summary>
		[Description("只允许一个读取操作")]
		public bool SingleReader { get; set; }

		/// <summary>
		/// 只允许一个写入操作
		/// </summary>
		[Description("只允许一个写入操作")]
		public bool SingleWriter { get; set; }

		/// <summary>
		/// 同步操作<br />
		/// true:外部手动读取通道内的数据<br />
		/// false:内部源源不断的读取通道内的数据，使用事件<see cref="T:Snet.Model.data.EventDataResult" />往外抛出读取的数据
		/// </summary>
		[Description("同步操作")]
		public bool IsSync { get; set; } = true;
	}
	/// <summary>
	/// 基于 <see cref="T:System.Threading.Channels.Channel`1" /> 的通道操作封装。
	/// </summary>
	/// <remarks>
	/// 【线程安全增强版】在原有功能基础上修复了以下并发安全问题：
	/// <list type="bullet">
	/// <item><description>懒初始化竞态条件（double-check locking 替代 ??= 运算符）——#1</description></item>
	/// <item><description>后台读取循环的通道引用漂移（参数注入替代属性访问）——#2</description></item>
	/// <item><description>ResetChannel 并发不安全（引入 SemaphoreSlim 异步互斥）——#3</description></item>
	/// <item><description>BegOperate/EndOperate 不配对导致的资源泄漏——#4, #5</description></item>
	/// <item><description>缺少 disposed 状态检查（引入 ObjectDisposedException 防护）——#6</description></item>
	/// <item><description>后台任务丢失（保存 Task 引用，支持优雅退出）——#7</description></item>
	/// </list>
	///
	/// 支持两种工作模式：
	/// <list type="bullet">
	/// <item><description><b>同步模式（IsSync = true）</b>：不启动后台读取任务，由调用方显式调用 ReadAsync / TryRead 读取数据。</description></item>
	/// <item><description><b>异步模式（IsSync = false）</b>：构造时自动启动后台读取任务，数据通过事件回调方式分发。</description></item>
	/// </list>
	/// </remarks>
	public class ChannelOperate<T> : CoreUnify<ChannelOperate<T>, ChannelData>, IDisposable, IAsyncDisposable where T : notnull
	{
		/// <summary>
		/// 内部通道实例。
		/// 【修复 #1】使用 volatile 修饰，保证多线程间读取的可见性。
		/// 配合 <see cref="F:Snet.Core.channel.ChannelOperate`1._initLock" /> 实现安全的 double-check locking 懒初始化。
		/// </summary>
		private volatile Channel<T>? _channel;

		/// <summary>
		/// 后台读取循环的取消令牌源。
		/// </summary>
		private CancellationTokenSource? _backgroundCts;

		/// <summary>
		/// 后台读取循环的 Task 引用。
		/// 【修复 #7】原代码使用 _ = Async() 的 fire-and-forget 模式，
		/// 导致 DisposeAsync 无法等待后台任务优雅退出。
		/// 保存 Task 引用后，可在释放时安全等待其完成。
		/// </summary>
		private Task? _backgroundTask;

		/// <summary>
		/// 对象是否已被释放。
		/// 【修复 #6】防止 Dispose 后继续操作通道，避免不确定行为。
		/// 【修复 #11】该标记为可逆状态，ResetChannelAsync 可将其重置为 false，支持"释放 → 重置 → 复用"的生命周期。
		/// </summary>
		private volatile bool _disposed;

		/// <summary>
		/// 通道初始化锁对象。
		/// 【修复 #1】用于 double-check locking，仅在通道首次创建时产生竞争，
		/// 后续访问完全无锁（快速路径仅一次 volatile read）。
		/// </summary>
		private readonly object _initLock = new object();

		/// <summary>
		/// 重置操作的异步互斥信号量。
		/// 【修复 #3】防止并发调用 ResetChannel 导致的状态不一致。
		/// 技术选型：SemaphoreSlim(1,1) 而非 lock，因为需要 async 兼容。
		/// 【修复 #11】生命周期与对象实例一致，Dispose/DisposeAsync 不会释放它，以支持 ResetChannelAsync 在释放后调用。
		/// </summary>
		private readonly SemaphoreSlim _resetLock = new SemaphoreSlim(1, 1);

		/// <summary>
		/// 对外暴露的只读通道读取端。
		/// </summary>
		public ChannelReader<T>? Reader => _channel?.Reader;

		/// <summary>
		/// 对外暴露的只写通道写入端。
		/// </summary>
		public ChannelWriter<T>? Writer => _channel?.Writer;

		/// <summary>
		/// 当前通道中尚未被读取的数据数量（近似值）。
		/// </summary>
		public int Count => _channel?.Reader.Count ?? 0;

		/// <summary>
		/// 实例是否处于已释放状态。
		/// </summary>
		/// <remarks>
		/// 调用 <see cref="M:Snet.Core.channel.ChannelOperate`1.ResetChannelAsync(System.Threading.CancellationToken)" /> 后会重置为 <c>false</c>。
		/// </remarks>
		public bool IsDisposed => _disposed;

		/// <summary>
		/// 创建一个通道操作实例。
		/// </summary>
		/// <param name="basics">通道基础配置参数。</param>
		public ChannelOperate(ChannelData basics)
			: base(basics)
		{
			if (!basics.IsSync)
			{
				StartBackgroundReader();
			}
		}

		/// <summary>
		/// 线程安全地获取或创建通道实例（Double-Check Locking 模式）。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Channel<T> EnsureChannel()
		{
			Channel<T> channel = _channel;
			if (channel != null)
			{
				return channel;
			}
			lock (_initLock)
			{
				if (_channel != null)
				{
					return _channel;
				}
				return _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(base.basics.Capacity)
				{
					FullMode = BoundedChannelFullMode.Wait,
					SingleReader = base.basics.SingleReader,
					SingleWriter = base.basics.SingleWriter
				});
			}
		}

		/// <summary>
		/// 启动后台异步读取任务。
		/// </summary>
		private void StartBackgroundReader()
		{
			CancellationTokenSource cancellationTokenSource = (_backgroundCts = new CancellationTokenSource());
			Channel<T> channel = EnsureChannel();
			_backgroundTask = RunBackgroundReadLoopAsync(channel, cancellationTokenSource.Token);
		}

		/// <summary>
		/// 后台异步读取循环的核心实现。
		/// </summary>
		private async Task RunBackgroundReadLoopAsync(Channel<T> channel, CancellationToken token)
		{
			try
			{
				while (await channel.Reader.WaitToReadAsync(token).ConfigureAwait(continueOnCapturedContext: false))
				{
					T item;
					while (channel.Reader.TryRead(out item))
					{
						await OnDataEventHandlerAsync(this, EventDataResult.CreateSuccessResult<T>("读取成功", item)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (ChannelClosedException ex2)
			{
				await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("[ RunBackgroundReadLoop ] 通道已关闭：" + ex2.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex3)
			{
				await OnInfoEventHandlerAsync(this, EventInfoResult.CreateFailureResult("[ RunBackgroundReadLoop ] 未预期异常：" + ex3.Message)).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 清理并关闭当前通道资源（内部方法，同步执行）。
		/// </summary>
		private void CleanupChannel()
		{
			Channel<T> channel = _channel;
			if (channel != null)
			{
				try
				{
					channel.Writer.TryComplete();
					T item;
					while (channel.Reader.TryRead(out item))
					{
					}
				}
				catch (ChannelClosedException)
				{
				}
			}
			CancellationTokenSource backgroundCts = _backgroundCts;
			if (backgroundCts != null)
			{
				try
				{
					backgroundCts.Cancel();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		/// <summary>
		/// 重置并重新初始化通道（异步线程安全版本，推荐使用）。
		/// </summary>
		/// <remarks>
		/// 支持在 Dispose / DisposeAsync 之后调用，将实例恢复为可用状态。
		/// 如需永久销毁实例，调用 Dispose/DisposeAsync 后不再调用 ResetChannelAsync 即可。
		/// </remarks>
		public async Task ResetChannelAsync(CancellationToken token = default(CancellationToken))
		{
			await _resetLock.WaitAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				_disposed = false;
				CleanupChannel();
				Task backgroundTask = _backgroundTask;
				if (backgroundTask != null && !backgroundTask.IsCompleted)
				{
					try
					{
						using CancellationTokenSource timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
						await backgroundTask.WaitAsync(timeoutCts.Token).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (OperationCanceledException)
					{
					}
					catch
					{
					}
				}
				_backgroundCts?.Dispose();
				_channel = null;
				_backgroundCts = null;
				_backgroundTask = null;
				if (!base.basics.IsSync)
				{
					StartBackgroundReader();
				}
			}
			finally
			{
				_resetLock.Release();
			}
		}

		/// <summary>
		/// 重置通道（同步版本，保持向后兼容）。
		/// </summary>
		public void ResetChannel()
		{
			ResetChannelAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 检查对象是否已释放，若是则抛出 <see cref="T:System.ObjectDisposedException" />。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().FullName, "通道操作实例已被释放，无法继续使用。如需复用，请调用 ResetChannelAsync 重置。");
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			CleanupChannel();
			Task backgroundTask = _backgroundTask;
			if (backgroundTask != null && !backgroundTask.IsCompleted)
			{
				try
				{
					backgroundTask.Wait(TimeSpan.FromSeconds(3.0));
				}
				catch
				{
				}
			}
			_backgroundCts?.Dispose();
			_backgroundCts = null;
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			CleanupChannel();
			Task backgroundTask = _backgroundTask;
			if (backgroundTask != null && !backgroundTask.IsCompleted)
			{
				try
				{
					using CancellationTokenSource timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
					await backgroundTask.WaitAsync(timeoutCts.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (OperationCanceledException)
				{
				}
				catch
				{
				}
			}
			_backgroundCts?.Dispose();
			_backgroundCts = null;
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 异步读取一条数据（仅同步模式可用）。
		/// </summary>
		public async ValueTask<OperateResult> ReadAsync(CancellationToken token)
		{
			ThrowIfDisposed();
			await BegOperateAsync(token, "ReadAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (!base.basics.IsSync)
			{
				await EndOperateAsync(status: false, "( IsSync = false ) 异步启动，无法使用此方法", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadAsync", 372).ConfigureAwait(continueOnCapturedContext: false);
				throw await CustomException.CreateAsync("( IsSync = false ) 异步启动，无法使用此方法", token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadAsync", 378).ConfigureAwait(continueOnCapturedContext: false);
			}
			try
			{
				return await EndOperateAsync(status: true, null, await EnsureChannel().Reader.ReadAsync(token).ConfigureAwait(continueOnCapturedContext: false), null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadAsync", 387).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (TaskCanceledException)
			{
			}
			catch (ChannelClosedException)
			{
				return await EndOperateAsync(status: false, "通道已关闭", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadAsync", 392).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
				return await EndOperateAsync(status: false, "外部取消操作", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadAsync", 396).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex4)
			{
				return await EndOperateAsync(status: false, "异常：" + ex4.Message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadAsync", 400).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadAsync", 402).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 异步读取（带超时，仅同步模式可用）。
		/// </summary>
		public async ValueTask<OperateResult> ReadWaitAsync(int timeOut, CancellationToken token = default(CancellationToken))
		{
			ThrowIfDisposed();
			await BegOperateAsync(token, "ReadWaitAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (!base.basics.IsSync)
			{
				await EndOperateAsync(status: false, "( IsSync = false ) 异步启动，无法使用此方法", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadWaitAsync", 416).ConfigureAwait(continueOnCapturedContext: false);
				throw await CustomException.CreateAsync("( IsSync = false ) 异步启动，无法使用此方法", default(CancellationToken), "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadWaitAsync", 422).ConfigureAwait(continueOnCapturedContext: false);
			}
			try
			{
				using CancellationTokenSource timeoutCts = new CancellationTokenSource(timeOut);
				return await EndOperateAsync(status: true, null, await EnsureChannel().Reader.ReadAsync(timeoutCts.Token).ConfigureAwait(continueOnCapturedContext: false), null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadWaitAsync", 431).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (TaskCanceledException)
			{
			}
			catch (ChannelClosedException)
			{
				return await EndOperateAsync(status: false, "通道已关闭", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadWaitAsync", 436).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
				return await EndOperateAsync(status: false, "响应超时", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadWaitAsync", 440).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex4)
			{
				return await EndOperateAsync(status: false, "异常：" + ex4.Message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadWaitAsync", 444).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "ReadWaitAsync", 446).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 尝试非阻塞读取（返回操作结果包装）。
		/// </summary>
		public OperateResult TryRead()
		{
			ThrowIfDisposed();
			BegOperate("TryRead");
			bool flag = EnsureChannel().Reader.TryRead(out var item);
			return EndOperate(flag, flag ? "读取成功" : "读取失败，当前无数据", item, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "TryRead", 457);
		}

		/// <summary>
		/// 尝试非阻塞读取（零分配高性能版本）。
		/// </summary>
		public bool TryRead(out T value)
		{
			ThrowIfDisposed();
			BegOperate("TryRead");
			bool flag = EnsureChannel().Reader.TryRead(out value);
			EndOperate(flag, flag ? "读取成功" : "读取失败，当前无数据", null, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "TryRead", 468);
			return flag;
		}

		/// <summary>
		/// 异步写入数据到通道。
		/// </summary>
		public async ValueTask<OperateResult> WriteAsync(T value, CancellationToken token)
		{
			ThrowIfDisposed();
			await BegOperateAsync(token, "WriteAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				await EnsureChannel().Writer.WriteAsync(value, token).ConfigureAwait(continueOnCapturedContext: false);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "WriteAsync", 482).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (TaskCanceledException)
			{
			}
			catch (ChannelClosedException)
			{
				return await EndOperateAsync(status: false, "通道已关闭", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "WriteAsync", 487).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
				return await EndOperateAsync(status: false, "外部取消操作", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "WriteAsync", 491).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex4)
			{
				return await EndOperateAsync(status: false, "异常：" + ex4.Message, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "WriteAsync", 495).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await EndOperateAsync(status: false, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "WriteAsync", 497).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <summary>
		/// 尝试非阻塞写入。
		/// </summary>
		public OperateResult TryWrite(T value)
		{
			ThrowIfDisposed();
			BegOperate("TryWrite");
			bool flag = EnsureChannel().Writer.TryWrite(value);
			return EndOperate(flag, flag ? "写入成功" : "写入失败（通道已满或关闭）", null, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\channel\\ChannelOperate.cs", "TryWrite", 508);
		}
	}
}
namespace Snet.Core.cache.share
{
	/// <summary>
	/// 共享缓存的数据
	/// </summary>
	public class ShareCacheData
	{
		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Category("基础数据")]
		[Description("唯一标识符")]
		public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

		/// <summary>
		/// 默认路径<br />
		/// 默认指向系统缓存目录<br />
		/// 也可以自定义目录
		/// </summary>
		[Description("默认路径")]
		public string Path { get; set; } = System.IO.Path.GetTempPath();

		/// <summary>
		/// 缓存名称
		/// </summary>
		[Description("缓存名称")]
		public string Name { get; set; } = "Snet-Cache.dat";

		/// <summary>
		/// 映射名称
		/// </summary>
		[Description("映射名称")]
		public string MapName { get; set; } = "Snet-Cache";

		/// <summary>
		/// 整体大小
		/// </summary>
		[Description("整体大小")]
		public long WholeSize { get; set; } = 10485760L;

		/// <summary>
		/// 索引大小
		/// </summary>
		[Description("索引大小")]
		public int IndexSize { get; set; } = 524288;

		/// <summary>
		/// 缓存文件访问类型
		/// </summary>
		[Description("缓存文件访问类型")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public MemoryMappedFileAccess Access { get; set; }
	}
	/// <summary>
	/// 共享缓存条目
	/// </summary>
	public class ShareCacheEntry
	{
		/// <summary>
		/// 在内存文件中的起始位置
		/// </summary>
		public long Position { get; set; }

		/// <summary>
		/// 数据长度
		/// </summary>
		public int Length { get; set; }
	}
	/// <summary>
	/// 空闲块
	/// </summary>
	public class ShareCacheFree : ShareCacheEntry
	{
	}
	/// <summary>
	/// 共享缓存头部信息<br />
	/// <br />
	/// 存放在共享内存文件的头部 (HeaderSize = 1MB) 区域，<br />
	/// 用于记录缓存的全局状态，包括：<br />
	/// - 所有 key 的索引表 (Indexes)<br />
	/// - 分配器的当前位置 (CurrentPosition)<br />
	/// - 空闲块列表 (FreeBlocks)<br />
	/// <br />
	/// ⚡ 这样设计的目的：<br />
	/// 在多进程或程序重启后，可以通过头部信息恢复内存分配器的状态，<br />
	/// 避免多个 Key 使用相同的 Position 造成数据覆盖。<br />
	/// </summary>
	public class ShareCacheHeader
	{
		/// <summary>
		/// 索引表  
		/// key   : 缓存的唯一名称（字符串）  
		/// value : ShareCacheEntry，保存该缓存对应的存储位置 (Position) 和长度 (Length)  
		///
		/// 说明：  
		/// 这是整个缓存系统的核心目录，相当于“文件系统的目录表”。  
		/// 每个缓存对象的数据实际存储在共享内存的正文区（HeaderSize 之后），  
		/// 而索引表只存储指针信息，保证快速定位。
		/// </summary>
		public Dictionary<string, ShareCacheEntry> Indexes { get; set; } = new Dictionary<string, ShareCacheEntry>();

		/// <summary>
		/// 当前分配器位置指针  
		/// 表示共享内存正文区（HeaderSize 之后）的“已分配到的位置”。  
		///
		/// 例如：  
		/// 如果 CurrentPosition = 1024，说明正文区前 1024 字节已被分配，  
		/// 下一次如果没有可用的 FreeBlocks，则会从 HeaderSize + 1024 处继续分配。
		///
		/// ⚠ 注意：这个值不是绝对文件偏移，而是正文区的相对偏移。  
		/// 最终地址 = HeaderSize + CurrentPosition。
		/// </summary>
		public long CurrentPosition { get; set; }

		/// <summary>
		/// 空闲块列表  
		/// 用于记录已经释放的内存块，可以被后续分配复用。  
		/// 每个 ShareCacheFree 包含：
		/// - Position : 空闲块在正文区的相对起始位置
		/// - Length   : 空闲块的大小（字节数）
		///
		/// 说明：  
		/// 这相当于“内存碎片回收机制”，  
		/// 如果不用 FreeBlocks，而只是不断增加 CurrentPosition，  
		/// 文件会无限膨胀，而且重复 key 覆盖时会浪费空间。  
		///
		/// 通过记录 FreeBlocks，可以让分配器优先复用这些空闲区域，  
		/// 避免频繁扩展文件，提高空间利用率。
		/// </summary>
		public List<ShareCacheFree> FreeBlocks { get; set; } = new List<ShareCacheFree>();
	}
	/// <summary>
	/// 共享缓存操作类（对外提供 API）<br />
	/// <br />
	/// 功能：<br />
	/// - 管理共享内存（MemoryMappedFile）的读写<br />
	/// - 提供缓存的增删改查（Set/Get/Remove/Clear）<br />
	/// - 管理索引表和分配器的持久化<br />
	/// - 保证跨进程访问的并发安全（全局互斥锁）<br />
	/// <br />
	/// 使用方式：<br />
	/// var op = new ShareCacheOperate(basics);<br />
	/// op.SetCache("key", bytes);<br />
	/// var data = op.GetCache("key");<br />
	/// op.RemoveCache("key");<br />
	/// op.ClearCache();<br />
	///
	/// </summary>
	public class ShareCacheOperate : CoreUnify<ShareCacheOperate, ShareCacheData>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 内存映射文件实例（全局唯一）<br />
		/// <br />
		/// 说明：<br />
		/// - 将文件映射到内存，提高读写性能<br />
		/// - 跨进程共享同一个文件映射<br />
		/// - 避免频繁创建/释放造成开销
		/// </summary>
		private MemoryMappedFile mappedFile;

		/// <summary>
		/// 内存映射文件访问器（全局复用）<br />
		/// <br />
		/// 说明：<br />
		/// - 提供读写字节数组的能力<br />
		/// - 类似于“内存指针”，支持随机读写<br />
		/// - 必须保证和 mappedFile 生命周期一致
		/// </summary>
		private MemoryMappedViewAccessor mappedViewAccessor;

		/// <summary>
		/// 缓存索引表（进程内缓存）<br />
		/// <br />
		/// key   : 缓存 Key（字符串）<br />
		/// value : ShareCacheEntry（数据在文件中的 Position 和 Length）<br />
		/// <br />
		/// 说明：<br />
		/// - 使用 ConcurrentDictionary 保证线程安全<br />
		/// - 每次写入前会调用 RefreshIndex() 从共享内存刷新最新索引<br />
		/// - 避免多进程情况下索引不一致
		/// </summary>
		private readonly ConcurrentDictionary<string, ShareCacheEntry> indexs = new ConcurrentDictionary<string, ShareCacheEntry>();

		/// <summary>
		/// 空间分配器（基于 FreeList 管理）<br />
		/// <br />
		/// 说明：<br />
		/// - 负责分配、回收、复用共享内存正文区的空间<br />
		/// - 避免空间碎片，提高复用率<br />
		/// - 状态通过 ShareCacheHeader 持久化
		/// </summary>
		private readonly ShareCacheFreeListAllocator allocator = new ShareCacheFreeListAllocator();

		/// <summary>
		/// 跨进程互斥锁<br />
		/// <br />
		/// 说明：<br />
		/// - 确保多进程同时操作共享内存时，写操作的原子性<br />
		/// - Mutex 名字是全局唯一的 "Global_ShareCache_Mutex"<br />
		/// - 所有 Set/Get/Remove/Clear 操作都加锁
		/// </summary>
		private static readonly Mutex GlobalMutex = new Mutex(initiallyOwned: false, "Global_ShareCache_Mutex");

		/// <summary>
		/// 互斥锁等待超时时间（毫秒）<br />
		/// 防止持有锁的进程崩溃导致其他进程永久死锁
		/// </summary>
		private const int MutexTimeoutMs = 30000;

		/// <summary>
		/// 构造函数
		/// </summary>
		public ShareCacheOperate(ShareCacheData basics)
			: base(basics)
		{
			Init();
			RefreshIndex();
		}

		/// <summary>
		/// 从共享内存读取索引并恢复本地状态
		/// </summary>
		private void RefreshIndex()
		{
			byte[] array = new byte[base.basics.IndexSize];
			mappedViewAccessor.ReadArray(0L, array, 0, base.basics.IndexSize);
			string text = Encoding.UTF8.GetString(array).Trim('\0');
			indexs.Clear();
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			ShareCacheHeader shareCacheHeader = JsonSerializer.Deserialize<ShareCacheHeader>(text);
			if (shareCacheHeader == null)
			{
				return;
			}
			foreach (KeyValuePair<string, ShareCacheEntry> index in shareCacheHeader.Indexes)
			{
				indexs[index.Key] = index.Value;
			}
			allocator.Reset();
			allocator.SetState(shareCacheHeader.CurrentPosition, shareCacheHeader.FreeBlocks);
		}

		/// <summary>
		/// 将最新的索引和分配器状态写入共享内存
		/// </summary>
		private void SaveIndex()
		{
			string s = JsonSerializer.Serialize(new ShareCacheHeader
			{
				Indexes = new Dictionary<string, ShareCacheEntry>(indexs),
				CurrentPosition = allocator.GetCurrentPosition(),
				FreeBlocks = allocator.GetFreeBlocks()
			});
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			if (bytes.Length > base.basics.IndexSize)
			{
				throw new InvalidOperationException("索引数据超过 Header 区域大小");
			}
			mappedViewAccessor.WriteArray(0L, bytes, 0, bytes.Length);
			if (bytes.Length < base.basics.IndexSize)
			{
				mappedViewAccessor.WriteArray(bytes.Length, new byte[base.basics.IndexSize - bytes.Length], 0, base.basics.IndexSize - bytes.Length);
			}
		}

		/// <summary>
		/// 初始化内存映射文件
		/// </summary>
		private void Init()
		{
			if (!Directory.Exists(base.basics.Path))
			{
				Directory.CreateDirectory(base.basics.Path);
			}
			mappedFile = MemoryMappedFile.CreateFromFile(Path.Combine(base.basics.Path, base.basics.Name), FileMode.OpenOrCreate, base.basics.MapName, base.basics.WholeSize, base.basics.Access);
			mappedViewAccessor = mappedFile.CreateViewAccessor();
		}

		/// <summary>
		/// 分配新的存储块（跳过 Header 区域）
		/// </summary>
		private ShareCacheEntry AllocateSpace(int size)
		{
			long position = allocator.Allocate(size) + base.basics.IndexSize;
			return new ShareCacheEntry
			{
				Position = position,
				Length = size
			};
		}

		/// <summary>
		/// 设置缓存（同步）
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="value">缓存值（字节数据）</param>
		/// <returns>结果对象</returns>
		public OperateResult SetCache(string key, byte[] value)
		{
			return SetCacheAsync(key, value).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 设置缓存（异步）
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="value">缓存值（字节数据）</param>
		/// <param name="token">取消令牌</param>
		/// <returns>结果对象</returns>
		public async Task<OperateResult> SetCacheAsync(string key, byte[] value, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SetCacheAsync").ConfigureAwait(continueOnCapturedContext: false);
			bool lockAcquired = false;
			try
			{
				lockAcquired = GlobalMutex.WaitOne(30000);
				if (!lockAcquired)
				{
					return await EndOperateAsync(status: false, "获取互斥锁超时", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "SetCacheAsync", 206).ConfigureAwait(continueOnCapturedContext: false);
				}
				RefreshIndex();
				if (!indexs.TryGetValue(key, out ShareCacheEntry value2))
				{
					value2 = AllocateSpace(value.Length);
					indexs[key] = value2;
				}
				else if (value.Length > value2.Length)
				{
					mappedViewAccessor.WriteArray(value2.Position, new byte[value2.Length], 0, value2.Length);
					allocator.Free(value2.Position, value2.Length);
					value2 = AllocateSpace(value.Length);
					indexs[key] = value2;
				}
				else
				{
					mappedViewAccessor.WriteArray(value2.Position, new byte[value2.Length], 0, value2.Length);
					indexs[key].Length = value.Length;
				}
				mappedViewAccessor.WriteArray(value2.Position, value, 0, value.Length);
				SaveIndex();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "SetCacheAsync", 233).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "SetCacheAsync", 237).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				if (lockAcquired)
				{
					GlobalMutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// 获取缓存（同步）
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <returns>操作结果</returns>
		public OperateResult GetCache(string key)
		{
			return GetCacheAsync(key).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 获取缓存（异步）
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="token">取消令牌</param>
		public async Task<OperateResult> GetCacheAsync(string key, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetCacheAsync").ConfigureAwait(continueOnCapturedContext: false);
			bool lockAcquired = false;
			try
			{
				lockAcquired = GlobalMutex.WaitOne(30000);
				if (!lockAcquired)
				{
					return await EndOperateAsync(status: false, "获取互斥锁超时", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "GetCacheAsync", 265).ConfigureAwait(continueOnCapturedContext: false);
				}
				RefreshIndex();
				if (!indexs.TryGetValue(key, out ShareCacheEntry value))
				{
					return await EndOperateAsync(status: false, "未找到缓存: " + key, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "GetCacheAsync", 269).ConfigureAwait(continueOnCapturedContext: false);
				}
				byte[] array = new byte[value.Length];
				mappedViewAccessor.ReadArray(value.Position, array, 0, value.Length);
				return await EndOperateAsync(status: true, null, array, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "GetCacheAsync", 274).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "GetCacheAsync", 278).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				if (lockAcquired)
				{
					GlobalMutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// 移除缓存（同步）；<br />
		/// 根据键名删除对应的缓存数据，并回收共享内存空间
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <returns>操作结果</returns>
		public OperateResult RemoveCache(string key)
		{
			return RemoveCacheAsync(key).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 移除缓存（异步）；<br />
		/// 根据键名异步删除对应的缓存数据
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="token">取消令牌</param>
		/// <returns>操作结果</returns>
		public async Task<OperateResult> RemoveCacheAsync(string key, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "RemoveCacheAsync").ConfigureAwait(continueOnCapturedContext: false);
			bool lockAcquired = false;
			try
			{
				lockAcquired = GlobalMutex.WaitOne(30000);
				if (!lockAcquired)
				{
					return await EndOperateAsync(status: false, "获取互斥锁超时", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "RemoveCacheAsync", 310).ConfigureAwait(continueOnCapturedContext: false);
				}
				RefreshIndex();
				if (!indexs.TryRemove(key, out ShareCacheEntry value))
				{
					return await EndOperateAsync(status: false, "未找到缓存: " + key, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "RemoveCacheAsync", 314).ConfigureAwait(continueOnCapturedContext: false);
				}
				mappedViewAccessor.WriteArray(value.Position, new byte[value.Length], 0, value.Length);
				allocator.Free(value.Position, value.Length);
				SaveIndex();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "RemoveCacheAsync", 320).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "RemoveCacheAsync", 324).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				if (lockAcquired)
				{
					GlobalMutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// 清空所有缓存（同步）
		/// </summary>
		public OperateResult ClearCache()
		{
			return ClearCacheAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 清空所有缓存（异步）
		/// </summary>
		public async Task<OperateResult> ClearCacheAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "ClearCacheAsync").ConfigureAwait(continueOnCapturedContext: false);
			bool lockAcquired = false;
			try
			{
				lockAcquired = GlobalMutex.WaitOne(30000);
				if (!lockAcquired)
				{
					return await EndOperateAsync(status: false, "获取互斥锁超时", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "ClearCacheAsync", 349).ConfigureAwait(continueOnCapturedContext: false);
				}
				RefreshIndex();
				indexs.Clear();
				allocator.Reset();
				mappedViewAccessor?.Dispose();
				mappedFile?.Dispose();
				string filePath = Path.Combine(base.basics.Path, base.basics.Name);
				await Task.Run(delegate
				{
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
				}, token).ConfigureAwait(continueOnCapturedContext: false);
				await Task.Run(delegate
				{
					Init();
				}, token).ConfigureAwait(continueOnCapturedContext: false);
				SaveIndex();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "ClearCacheAsync", 368).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\share\\ShareCacheOperate.cs", "ClearCacheAsync", 372).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				if (lockAcquired)
				{
					GlobalMutex.ReleaseMutex();
				}
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			mappedViewAccessor?.Dispose();
			mappedFile?.Dispose();
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			mappedViewAccessor?.Dispose();
			mappedFile?.Dispose();
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.cache.process
{
	/// <summary>
	/// 进程缓存数据
	/// </summary>
	public class ProcessCacheData
	{
		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Category("基础数据")]
		[Description("唯一标识符")]
		public string SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

		/// <summary>
		/// 绝对过期时间(分钟)
		/// </summary>
		[Description("绝对过期时间(分钟)")]
		public int AbsoluteExpiration { get; set; } = 60;

		/// <summary>
		/// 滑动过期时间(分钟)
		/// </summary>
		[Description("滑动过期时间(分钟)")]
		public int SlidingExpiration { get; set; } = 20;

		/// <summary>
		/// 优先级
		/// </summary>
		[Description("优先级")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
	}
	/// <summary>
	/// 进程缓存操作类 <br />
	/// 基于 Microsoft.Extensions.Caching.Memory.MemoryCache 封装。<br />
	/// 只允许在单个进程中使用，不能跨进程或跨机器共享。<br />
	/// 提供了缓存的增、删、查、清空等统一操作接口。
	/// </summary>
	public class ProcessCacheOperate : CoreUnify<ProcessCacheOperate, ProcessCacheData>, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 内部缓存对象（实际存储容器）。<br />
		/// 使用 MemoryCache（线程安全，基于 ConcurrentDictionary 实现）。
		/// </summary>
		private readonly MemoryCache cacheObject = new MemoryCache(new MemoryCacheOptions());

		/// <summary>
		/// 缓存项配置工厂，每次生产独立的配置实例。<br />
		/// 包括绝对过期时间、滑动过期时间、优先级等。
		/// </summary>
		private MemoryCacheEntryOptions CreateCacheOptions()
		{
			return new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(base.basics.AbsoluteExpiration)).SetSlidingExpiration(TimeSpan.FromMinutes(base.basics.SlidingExpiration)).SetPriority(base.basics.Priority);
		}

		/// <summary>
		/// 构造函数，初始化缓存操作对象。<br />
		/// 会根据传入的 <see cref="T:Snet.Core.cache.process.ProcessCacheData" /> 设置全局缓存策略。<br />
		/// </summary>
		/// <param name="basics">缓存基础配置，例如过期时间、优先级。</param>
		public ProcessCacheOperate(ProcessCacheData basics)
			: base(basics)
		{
		}

		/// <summary>
		/// 设置缓存（同步方法）。<br />
		/// 如果 key 已存在，则覆盖原有值。<br />
		/// 使用 <see cref="M:Snet.Core.cache.process.ProcessCacheOperate.CreateCacheOptions" /> 控制过期和优先级策略。
		/// </summary>
		/// <typeparam name="T">缓存对象的类型。</typeparam>
		/// <param name="key">缓存键，唯一标识。</param>
		/// <param name="value">缓存值。</param>
		/// <returns>操作结果，包含执行状态。</returns>
		public OperateResult SetCache<T>(string key, T value)
		{
			return SetCacheAsync(key, value).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步设置缓存。
		/// </summary>
		/// <typeparam name="T">缓存对象的类型。</typeparam>
		/// <param name="key">缓存键。</param>
		/// <param name="value">缓存值。</param>
		/// <param name="token">取消令牌。</param>
		/// <returns>操作结果。</returns>
		public async Task<OperateResult> SetCacheAsync<T>(string key, T value, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "SetCacheAsync");
			try
			{
				cacheObject.Set(key, value, CreateCacheOptions());
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "SetCacheAsync", 69);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "SetCacheAsync", 73);
			}
		}

		/// <summary>
		/// 获取缓存（同步方法）。<br />
		/// 如果 key 存在则返回缓存值，否则返回失败结果。
		/// </summary>
		/// <typeparam name="T">缓存对象的类型。</typeparam>
		/// <param name="key">缓存键。</param>
		/// <returns>操作结果，包含缓存值或错误信息。</returns>
		public OperateResult GetCache<T>(string key)
		{
			return GetCacheAsync<T>(key).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步获取缓存。
		/// </summary>
		/// <typeparam name="T">缓存对象类型。</typeparam>
		/// <param name="key">缓存键。</param>
		/// <param name="token">取消令牌。</param>
		/// <returns>操作结果。</returns>
		public async Task<OperateResult> GetCacheAsync<T>(string key, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "GetCacheAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (cacheObject.TryGetValue<T>(key, out T value))
				{
					return await EndOperateAsync(status: true, null, value, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "GetCacheAsync", 101).ConfigureAwait(continueOnCapturedContext: false);
				}
				return await EndOperateAsync(status: false, "未获取到对象键值对象", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "GetCacheAsync", 105).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "GetCacheAsync", 110).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 移除缓存。<br />
		/// 如果 key 存在则删除，不存在也不会抛异常。
		/// </summary>
		/// <param name="key">缓存键。</param>
		/// <returns>操作结果。</returns>
		public OperateResult RemoveCache(string key)
		{
			return RemoveCacheAsync(key).GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步移除缓存。
		/// </summary>
		/// <param name="key">缓存键。</param>
		/// <param name="token">取消令牌。</param>
		/// <returns>操作结果。</returns>
		public async Task<OperateResult> RemoveCacheAsync(string key, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "RemoveCacheAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				cacheObject.Remove(key);
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "RemoveCacheAsync", 135).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "RemoveCacheAsync", 139).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <summary>
		/// 清空所有缓存。
		/// </summary>
		/// <returns>操作结果。</returns>
		public OperateResult ClearCache()
		{
			return ClearCacheAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// 异步清空缓存。
		/// </summary>
		/// <param name="token">取消令牌。</param>
		/// <returns>操作结果。</returns>
		public async Task<OperateResult> ClearCacheAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "ClearCacheAsync").ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				cacheObject.Clear();
				return await EndOperateAsync(status: true, null, null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "ClearCacheAsync", 161).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\cache\\process\\ProcessCacheOperate.cs", "ClearCacheAsync", 165).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			ClearCache();
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await ClearCacheAsync();
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
namespace Snet.Core.@abstract
{
	/// <summary>
	/// 底层通信抽象类；<br />
	/// 外部必须实现抽象方法；<br />
	/// 实现接口的异步方法；<br />
	/// 继承核心抽象类
	/// </summary>
	/// <typeparam name="O">操作类</typeparam>
	/// <typeparam name="D">基础数据类，构造参数类</typeparam>
	public abstract class CommunicationAbstract<O, D> : CoreUnify<O, D>, ICommunication, IOn, IOff, ISend, ISendWait, IObject, IStatus, IEvent, IInstance, ILog, IArgs, ILanguage, IClone, IDisposable, IAsyncDisposable where O : class where D : class
	{
		/// <summary>
		/// 无参构造函数
		/// </summary>
		protected CommunicationAbstract()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="param">参数</param>
		protected CommunicationAbstract(D param)
			: base(param)
		{
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			Off(hardClose: true);
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await OffAsync(hardClose: true).ConfigureAwait(continueOnCapturedContext: false);
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult GetBaseObject()
		{
			return GetBaseObjectAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult On()
		{
			return OnAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Send(byte[] data)
		{
			return SendAsync(data).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult SendWait(byte[] data, CancellationToken token)
		{
			return SendWaitAsync(data, token).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public abstract Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> SendAsync(byte[] data, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> SendWaitAsync(byte[] data, CancellationToken token);
	}
	/// <summary>
	/// 底层通信抽象类；<br />
	/// 外部必须实现抽象方法；<br />
	/// 实现接口的异步方法；<br />
	/// 继承核心抽象类
	/// </summary>
	/// <typeparam name="O">操作类</typeparam>
	/// <typeparam name="D">基础数据类，构造参数类</typeparam>
	public abstract class CommunicationServiceAbstract<O, D> : CoreUnify<O, D>, ICommunicationService, IOn, IOff, ISendService, IObject, IStatus, IRemove, IEvent, IInstance, ILog, ILanguage, IClone, IArgs, IDisposable, IAsyncDisposable where O : class where D : class
	{
		/// <summary>
		/// 无参构造函数
		/// </summary>
		protected CommunicationServiceAbstract()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="param">参数</param>
		protected CommunicationServiceAbstract(D param)
			: base(param)
		{
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			Off(hardClose: true);
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await OffAsync(hardClose: true).ConfigureAwait(continueOnCapturedContext: false);
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult GetBaseObject()
		{
			return GetBaseObjectAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult On()
		{
			return OnAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Send(byte[] data, string[]? address = null)
		{
			return SendAsync(data, address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Remove(string[] address)
		{
			return RemoveAsync(address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Send(byte[] data, string address)
		{
			return SendAsync(data, address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public abstract Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> SendAsync(byte[] data, string[]? address = null, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> RemoveAsync(string[] address, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> SendAsync(byte[] data, string address, CancellationToken token = default(CancellationToken));
	}
	/// <summary>
	/// 采集抽象类；<br />
	/// 外部必须实现抽象方法（On/Off/Read/Write/Subscribe 等）；<br />
	/// 内置 WebApi 服务端支持，提供 HTTP 接口远程控制采集设备；<br />
	/// 支持 GB2312 编码，继承核心统一类
	/// </summary>
	/// <typeparam name="O">操作类</typeparam>
	/// <typeparam name="D">基础数据类，构造参数类</typeparam>
	public abstract class DaqAbstract<O, D> : CoreUnify<O, D>, IDaq, IOn, IOff, IRead, IWrite, ISubscribe, IEvent, IStatus, IArgs, IInstance, ILog, IWA, IObject, ILanguage, IClone, IPacker, IDisposable, IAsyncDisposable where O : class where D : class
	{
		/// <summary>
		/// Api服务端
		/// </summary>
		private HttpServiceOperate? WebApi;

		/// <summary>
		/// 打开接口绝对路径
		/// </summary>
		private const string on = "/api/on";

		/// <summary>
		/// 关闭接口绝对路径
		/// </summary>
		private const string off = "/api/off";

		/// <summary>
		/// 读取接口绝对路径
		/// </summary>
		private const string read = "/api/read";

		/// <summary>
		/// 写入接口绝对路径
		/// </summary>
		private const string write = "/api/write";

		/// <summary>
		/// 获取状态接口绝对路径
		/// </summary>
		private const string getstatus = "/api/getstatus";

		/// <summary>
		/// 切换语言接口绝对路径
		/// </summary>
		private const string switchlanguage = "/api/switchlanguage";

		/// <summary>
		/// 打开接口请求方式
		/// </summary>
		private static readonly HttpMethod onMethod;

		/// <summary>
		/// 关闭接口请求方式
		/// </summary>
		private static readonly HttpMethod offMethod;

		/// <summary>
		/// 读取接口请求方式
		/// </summary>
		private static readonly HttpMethod readMethod;

		/// <summary>
		/// 写入接口请求方式
		/// </summary>
		private static readonly HttpMethod writeMethod;

		/// <summary>
		/// 获取状态接口请求方式
		/// </summary>
		private static readonly HttpMethod getstatusMethod;

		/// <summary>
		/// 切换语言接口请求方式
		/// </summary>
		private static readonly HttpMethod switchlanguageMethod;

		/// <summary>
		/// 地址自动组包<br />
		/// 外部可以单独调用使用
		/// </summary>
		public PackerHandler packerHandler = CoreUnify<PackerHandler, string>.Instance(typeof(O).Name);

		/// <summary>
		/// 字节处理<br />
		/// 外部可以单独调用使用
		/// </summary>
		public BytesHandler bytesHandler = CoreUnify<BytesHandler, string>.Instance(typeof(O).Name);

		/// <summary>
		/// 静态构造函数，确保 GB2312 编码提供程序只注册一次（避免每次实例化重复调用）
		/// </summary>
		static DaqAbstract()
		{
			onMethod = HttpMethod.Post;
			offMethod = HttpMethod.Post;
			readMethod = HttpMethod.Post;
			writeMethod = HttpMethod.Post;
			getstatusMethod = HttpMethod.Get;
			switchlanguageMethod = HttpMethod.Post;
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}

		/// <summary>
		/// 无参构造函数
		/// </summary>
		protected DaqAbstract()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="param">参数</param>
		protected DaqAbstract(D param)
			: base(param)
		{
		}

		/// <summary>
		/// WebApi请求事件
		/// </summary>
		/// <param name="sender">源</param>
		/// <param name="e">事件结果</param>
		private async Task WebApi_OnDataEventAsync(object? sender, EventDataResult e)
		{
			HttpServiceData.WaitHandler handler = ((ResultModel)e).GetSource<HttpServiceData.WaitHandler>();
			try
			{
				if (handler == null)
				{
					return;
				}
				string absolutePath = handler.Request.Url.AbsolutePath;
				string text = absolutePath;
				if (text.Equals("/api/on"))
				{
					HttpServiceOperate webApi = WebApi;
					if (webApi != null)
					{
						HttpListenerResponse response = handler.Response;
						webApi.WriteAsync(response, await OnAsync()).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else if (text.Equals("/api/off"))
				{
					HttpServiceOperate webApi = WebApi;
					if (webApi != null)
					{
						HttpListenerResponse response = handler.Response;
						webApi.WriteAsync(response, await OffAsync()).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else if (text.Equals("/api/read"))
				{
					string? bodyData = handler.BodyData;
					Address val = ((bodyData != null) ? ExtendMethod.ToJsonEntity<Address>(bodyData, (JsonSerializerOptions)null) : null);
					if (val == null)
					{
						WebApi?.WriteAsync(handler.Response, OperateResult.CreateFailureResult("[ " + absolutePath + " ] 接口请求参数错误", (int?)null));
						return;
					}
					HttpServiceOperate webApi = WebApi;
					if (webApi != null)
					{
						HttpListenerResponse response = handler.Response;
						webApi.WriteAsync(response, await ReadAsync(val)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else if (text.Equals("/api/write"))
				{
					string? bodyData2 = handler.BodyData;
					ConcurrentDictionary<string, WriteModel> concurrentDictionary = ((bodyData2 != null) ? ExtendMethod.ToJsonEntity<ConcurrentDictionary<string, WriteModel>>(bodyData2, (JsonSerializerOptions)null) : null);
					if (concurrentDictionary == null)
					{
						WebApi?.WriteAsync(handler.Response, OperateResult.CreateFailureResult("[ " + absolutePath + " ] 接口请求参数错误", (int?)null));
						return;
					}
					HttpServiceOperate webApi = WebApi;
					if (webApi != null)
					{
						HttpListenerResponse response = handler.Response;
						webApi.WriteAsync(response, await WriteAsync(concurrentDictionary)).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else if (text.Equals("/api/getstatus"))
				{
					HttpServiceOperate webApi = WebApi;
					if (webApi != null)
					{
						HttpListenerResponse response = handler.Response;
						webApi.WriteAsync(response, await GetStatusAsync()).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else if (text.Equals("/api/switchlanguage"))
				{
					LanguageType language = (LanguageType)((int)(await GetLanguageAsync()) == 0);
					await SetLanguageAsync(language).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (Exception ex)
			{
				WebApi?.WriteAsync(handler.Response, OperateResult.CreateFailureResult("接口响应异常：" + ex.Message, (int?)null));
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> WAOnAsync(WAModel wAModel, CancellationToken token = default(CancellationToken))
		{
			HttpServiceData.Basics obj = new HttpServiceData.Basics();
			obj.SET(wAModel);
			obj.AbsolutePaths = new List<HttpServiceData.AbsolutePathsModel>
			{
				new HttpServiceData.AbsolutePathsModel("/api/on", onMethod),
				new HttpServiceData.AbsolutePathsModel("/api/off", offMethod),
				new HttpServiceData.AbsolutePathsModel("/api/read", readMethod),
				new HttpServiceData.AbsolutePathsModel("/api/write", writeMethod),
				new HttpServiceData.AbsolutePathsModel("/api/getstatus", getstatusMethod),
				new HttpServiceData.AbsolutePathsModel("/api/switchlanguage", switchlanguageMethod)
			};
			WebApi = await CoreUnify<HttpServiceOperate, HttpServiceData.Basics>.InstanceAsync(obj, token).ConfigureAwait(continueOnCapturedContext: false);
			WebApi.OnDataEventAsync -= WebApi_OnDataEventAsync;
			WebApi.OnDataEventAsync += WebApi_OnDataEventAsync;
			WebApi.OnInfoEventAsync -= base.OnInfoEventHandlerAsync;
			WebApi.OnInfoEventAsync += base.OnInfoEventHandlerAsync;
			return await WebApi.OnAsync(token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult WAOn(WAModel wAModel)
		{
			return WAOnAsync(wAModel).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> WAOffAsync(CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "WAOffAsync").ConfigureAwait(continueOnCapturedContext: false);
			if (WebApi == null)
			{
				return await EndOperateAsync(status: false, "WA尚未启动", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "WAOffAsync", 198).ConfigureAwait(continueOnCapturedContext: false);
			}
			await WebApi.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			WebApi = null;
			return await EndOperateAsync(status: true, "WA停止成功", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "WAOffAsync", 202).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult WAOff()
		{
			return WAOffAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> WAStatusAsync(CancellationToken token = default(CancellationToken))
		{
			if (WebApi == null)
			{
				return await EndOperateAsync(status: false, "WA尚未启动", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", await BegOperateAsync(token, "WAStatusAsync"), 214).ConfigureAwait(continueOnCapturedContext: false);
			}
			return await WebApi.GetStatusAsync(token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult WAStatus()
		{
			return WAStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<OperateResult> WARequestExampleAsync(CancellationToken token = default(CancellationToken))
		{
			object resultData = $"\r\n            {"/api/on"} [ 打开 ]，无参请求（{onMethod}）\r\n\r\n            {"/api/off"} [ 关闭 ]，无参请求（{offMethod}）\r\n\r\n            {"/api/getstatus"} [ 获取状态 ]，无参请求（{getstatusMethod}）\r\n\r\n            {"/api/switchlanguage"} [ 切换语言 ]，无参请求（{switchlanguageMethod}）\r\n\r\n            {"/api/read"} [ 读取 ]，带参请求（{readMethod}）：Snet.Model.data.Address\r\n            {{\r\n                \"SN\": \"8c71f4a7-04eb-4f9c-88d9-849c2f0c3a00\",\r\n                \"AddressArray\": [\r\n                    {{\r\n                        \"SN\": \"TestAddress\",\r\n                        \"AddressName\": \"M100\",\r\n                        \"AddressDataType\": \"Float\"\r\n                    }}\r\n                ],\r\n                \"CreationTime\": \"2024-06-05T13:01:24.6245462+08:00\"\r\n            }}\r\n\r\n            {"/api/write"} [ 写入 ]，带参请求（{writeMethod}）：ConcurrentDictionary<string, Snet.Model.data.WriteModel>\r\n            {{\r\n              \"M100\": {{\r\n                \"Value\": 99.1,\r\n                \"AddressDataType\": \"Float\"\r\n              }}\r\n            }}";
			return await EndOperateAsync(status: true, null, resultData, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", await BegOperateAsync(token, "WARequestExampleAsync"), 226).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public OperateResult WARequestExample()
		{
			return WARequestExampleAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Packer(Address? address, string protocolTypeKey, int maxByteLength = 0, DataFormat format = (DataFormat)0)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			BegOperate("Packer");
			try
			{
				Address resultData = packerHandler.AddressAutoPackOrPassthrough(address, protocolTypeKey, maxByteLength, format);
				return EndOperate(status: true, "组包成功", resultData, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "Packer", 285);
			}
			catch (Exception ex)
			{
				return EndOperate(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "Packer", 289);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> PackerAsync(Address? address, string protocolTypeKey, int maxByteLength = 0, DataFormat format = (DataFormat)0, CancellationToken token = default(CancellationToken))
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			await BegOperateAsync(token, "PackerAsync");
			try
			{
				return await EndOperateAsync(status: true, "组包成功", await packerHandler.AddressAutoPackOrPassthroughAsync(address, protocolTypeKey, maxByteLength, format, token), null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "PackerAsync", 300);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "PackerAsync", 304);
			}
		}

		/// <inheritdoc />
		public OperateResult UnPacker(AddressValue addressValue, bool isStringReverseByteWord = false)
		{
			BegOperate("UnPacker");
			try
			{
				OperateResult val = bytesHandler.Transform(addressValue, isStringReverseByteWord);
				return EndOperate(((BaseModel)val).Status, ((BaseModel)val).Message, ((ResultModel)val).ResultData, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPacker", 315);
			}
			catch (Exception ex)
			{
				return EndOperate(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPacker", 319);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> UnPackerAsync(AddressValue addressValue, bool isStringReverseByteWord = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "UnPackerAsync");
			try
			{
				OperateResult val = await bytesHandler.TransformAsync(addressValue, token, isStringReverseByteWord);
				return await EndOperateAsync(((BaseModel)val).Status, ((BaseModel)val).Message, ((ResultModel)val).ResultData, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPackerAsync", 330);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPackerAsync", 334);
			}
		}

		/// <inheritdoc />
		public OperateResult UnPacker(ConcurrentDictionary<string, AddressValue> pairs, bool isStringReverseByteWord = false)
		{
			BegOperate("UnPacker");
			try
			{
				if (pairs == null)
				{
					return EndOperate(status: false, "解包数据为空", null, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPacker", 344);
				}
				List<ConcurrentDictionary<string, AddressValue>> list = new List<ConcurrentDictionary<string, AddressValue>>(pairs.Count);
				foreach (KeyValuePair<string, AddressValue> pair in pairs)
				{
					OperateResult val = bytesHandler.Transform(pair.Value, isStringReverseByteWord);
					if (((BaseModel)val).Status && ((ResultModel)val).ResultData is ConcurrentDictionary<string, AddressValue> item)
					{
						list.Add(item);
					}
				}
				return EndOperate(status: true, "解包成功", list, null, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPacker", 354);
			}
			catch (Exception ex)
			{
				return EndOperate(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPacker", 358);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> UnPackerAsync(ConcurrentDictionary<string, AddressValue> pairs, bool isStringReverseByteWord = false, CancellationToken token = default(CancellationToken))
		{
			await BegOperateAsync(token, "UnPackerAsync");
			try
			{
				if (pairs == null)
				{
					return await EndOperateAsync(status: false, "解包数据为空", null, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPackerAsync", 368);
				}
				List<ConcurrentDictionary<string, AddressValue>> result = new List<ConcurrentDictionary<string, AddressValue>>(pairs.Count);
				foreach (KeyValuePair<string, AddressValue> pair in pairs)
				{
					token.ThrowIfCancellationRequested();
					OperateResult val = await bytesHandler.TransformAsync(pair.Value, token, isStringReverseByteWord);
					if (((BaseModel)val).Status && ((ResultModel)val).ResultData is ConcurrentDictionary<string, AddressValue> item)
					{
						result.Add(item);
					}
				}
				return await EndOperateAsync(status: true, "解包成功", result, null, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPackerAsync", 379);
			}
			catch (Exception ex)
			{
				return await EndOperateAsync(status: false, ex.Message, null, ex, logOutput: true, consoleOutput: true, token, "F:\\Snet\\Shunnet\\Snet\\Snet.Core\\abstract\\DaqAbstract.cs", "UnPackerAsync", 383);
			}
		}

		/// <inheritdoc />
		public OperateResult GetBaseObject()
		{
			return GetBaseObjectAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync(hardClose).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult On()
		{
			return OnAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Read(Address address)
		{
			return ReadAsync(address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Subscribe(Address address)
		{
			return SubscribeAsync(address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult UnSubscribe(Address address)
		{
			return UnSubscribeAsync(address).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Write(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values)
		{
			return WriteAsync(values).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Write(ConcurrentDictionary<string, object> values)
		{
			return WriteAsync(values).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Write(ConcurrentDictionary<string, WriteModel> values)
		{
			return WriteAsync(values).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public abstract Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> ReadAsync(Address address, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, object> values, CancellationToken token = default(CancellationToken))
		{
			return await WriteAsync(values.GetDefaultEncodingWrite((EncodingType)20127), token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, WriteModel> values, CancellationToken token = default(CancellationToken))
		{
			if (values == null || values.Count <= 0)
			{
				return OperateResult.CreateFailureResult("数据不能为空", (int?)null);
			}
			ConcurrentDictionary<string, (object, EncodingType?)> concurrentDictionary = new ConcurrentDictionary<string, (object, EncodingType?)>();
			foreach (KeyValuePair<string, WriteModel> value in values)
			{
				try
				{
					DataType addressDataType = value.Value.AddressDataType;
					switch (addressDataType - 1)
					{
					case 0:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToByte(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 1:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToBoolean(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 23:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<bool[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<bool[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 20:
						concurrentDictionary.TryAdd(value.Key, (value.Value.Value.ToString(), value.Value.EncodingType));
						break;
					case 21:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToChar(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 2:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToDouble(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 24:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<double[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<double[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 3:
					case 4:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToSingle(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 25:
					case 26:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<float[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<float[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 9:
					case 10:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToInt32(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 31:
					case 32:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<int[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<int[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 11:
					case 12:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToUInt32(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 33:
					case 34:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<uint[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<uint[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 13:
					case 14:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToInt64(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 35:
					case 36:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<long[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<long[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 15:
					case 16:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToUInt64(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 37:
					case 38:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<ulong[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<ulong[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 5:
					case 6:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToInt16(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 27:
					case 28:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<short[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<short[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 7:
					case 8:
						concurrentDictionary.TryAdd(value.Key, (Convert.ToUInt16(value.Value.Value.ToString()), value.Value.EncodingType));
						break;
					case 29:
					case 30:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.ToJsonEntity<ushort[]>(value.Value.Value.ToString(), (JsonSerializerOptions)null), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<ushort[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					case 22:
						if (value.Value.Value is string)
						{
							concurrentDictionary.TryAdd(value.Key, (ByteHandler.HexStringToByteArray(value.Value.Value.ToString()), value.Value.EncodingType));
						}
						else
						{
							concurrentDictionary.TryAdd(value.Key, (ExtendMethod.GetSource<byte[]>(value.Value.Value), value.Value.EncodingType));
						}
						break;
					default:
						return OperateResult.CreateFailureResult($"{value.Key} 不支持 {value.Value.AddressDataType} 类型转换", (int?)null);
					}
				}
				catch (Exception ex)
				{
					return OperateResult.CreateFailureResult(value.Key + " 地址数据类型转换异常:" + ex.Message, (int?)null);
				}
			}
			return await WriteAsync(concurrentDictionary, token).ConfigureAwait(continueOnCapturedContext: false);
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			WAOff();
			Off(hardClose: true);
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await WAOffAsync().ConfigureAwait(continueOnCapturedContext: false);
			await OffAsync(hardClose: true).ConfigureAwait(continueOnCapturedContext: false);
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
	/// <summary>
	/// 消息中间件抽象类；<br />
	/// 外部必须实现抽象方法；<br />
	/// 实现接口的异步方法；<br />
	/// 继承核心抽象类
	/// </summary>
	/// <typeparam name="O">操作类</typeparam>
	/// <typeparam name="D">基础数据类，构造参数类</typeparam>
	public abstract class MqAbstract<O, D> : CoreUnify<O, D>, IMq, IOn, IOff, IProducer, IConsumer, IEvent, IStatus, IArgs, IInstance, ILog, ILanguage, IClone, IDisposable, IAsyncDisposable where O : class where D : class
	{
		/// <summary>
		/// 无参构造函数
		/// </summary>
		protected MqAbstract()
		{
		}

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="param">参数</param>
		protected MqAbstract(D param)
			: base(param)
		{
		}

		/// <inheritdoc />
		public OperateResult GetStatus()
		{
			return GetStatusAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Off(bool hardClose = false)
		{
			return OffAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult On()
		{
			return OnAsync().GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Produce(string topic, string content, Encoding? encoding = null)
		{
			return ProduceAsync(topic, content, encoding).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Produce(string topic, byte[] content)
		{
			return ProduceAsync(topic, content).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult Consume(string topic)
		{
			return ConsumeAsync(topic).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public OperateResult UnConsume(string topic)
		{
			return UnConsumeAsync(topic).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public abstract Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public virtual Task<OperateResult> ProduceAsync(string topic, string content, Encoding? encoding = null, CancellationToken token = default(CancellationToken))
		{
			return ProduceAsync(topic, (encoding == null) ? Encoding.UTF8.GetBytes(content) : encoding.GetBytes(content));
		}

		/// <inheritdoc />
		public abstract Task<OperateResult> ProduceAsync(string topic, byte[] content, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> ConsumeAsync(string topic, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public abstract Task<OperateResult> UnConsumeAsync(string topic, CancellationToken token = default(CancellationToken));

		/// <inheritdoc />
		public override void Dispose()
		{
			Off(hardClose: true);
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await OffAsync(hardClose: true).ConfigureAwait(continueOnCapturedContext: false);
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
