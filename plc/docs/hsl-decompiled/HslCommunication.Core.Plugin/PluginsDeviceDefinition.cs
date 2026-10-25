// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using Newtonsoft.Json;

namespace HslCommunication.Core.Plugin;

/// <summary>
/// 插件设备的定义内容
/// </summary>
public class PluginsDeviceDefinition
{
	/// <summary>
	/// 设备的基本名称信息
	/// </summary>
	public string DeviceName { get; set; }

	/// <summary>
	/// 当前设备的描述信息
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 设备本身的图片信息，建议图片尺寸 128 * 128
	/// </summary>
	public byte[] Image { get; set; }

	/// <summary>
	/// 获取或设置小图标显示
	/// </summary>
	public string ImageKey { get; set; }

	[JsonIgnore]
	/// <summary>
	/// 实际的设备类型对象信息
	/// </summary>
	public Type DeviceObject { get; set; }

	[JsonIgnore]
	/// <summary>
	/// 实际的设备类型的初始化配置参数信息
	/// </summary>
	public Type DevicePara { get; set; }

	/// <summary>
	/// 插件的文件路径信息
	/// </summary>
	public string PluginFilePath { get; set; }

	/// <summary>
	/// 网站的地址
	/// </summary>
	public string Http { get; set; }

	/// <summary>
	/// 默认的实例化构造方法
	/// </summary>
	public PluginsDeviceDefinition()
	{
	}

	/// <summary>
	/// 根据传入的自定义对象，来初始化当前的设备插件对象
	/// </summary>
	/// <param name="plugin">设备对象</param>
	public PluginsDeviceDefinition(object plugin)
	{
		Type type = plugin.GetType();
		DeviceName = PluginsHelper.GetObjectProperty<string>(type, plugin, "DeviceName");
		Description = PluginsHelper.GetObjectProperty<string>(type, plugin, "Description");
		DeviceObject = PluginsHelper.GetObjectProperty<Type>(type, plugin, "DeviceObject");
		DevicePara = PluginsHelper.GetObjectProperty<Type>(type, plugin, "DevicePara");
	}
}
