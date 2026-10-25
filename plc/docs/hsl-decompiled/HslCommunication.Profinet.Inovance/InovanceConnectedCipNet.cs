// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Profinet.Omron;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// 汇川的CIP协议，基于连接的协议
/// </summary>
public class InovanceConnectedCipNet : OmronConnectedCipNet
{
	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public InovanceConnectedCipNet()
	{
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 根据指定的IP及端口来实例化这个连接对象
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口号信息</param>
	public InovanceConnectedCipNet(string ipAddress, int port = 44818)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override byte[] GetLargeForwardOpen(ushort connectionID)
	{
		byte[] array = "\r\n00 00 00 00 00 00 02 00 00 00 00 00 b2 00 34 00\r\n5b 02 20 06 24 01 0e 9c 02 00 00 80 01 00 fe 80\r\n02 00 1b 05 30 a7 2b 03 02 00 00 00 80 84 1e 00\r\ncc 07 00 42 80 84 1e 00 cc 07 00 42 a3 03 20 02\r\n24 01 2c 01".ToHexBytes();
		BitConverter.GetBytes(base.OTConnectionId).CopyTo(array, 24);
		BitConverter.GetBytes(base.TOConnectionId).CopyTo(array, 28);
		BitConverter.GetBytes((ushort)(2 + connectionID)).CopyTo(array, 32);
		BitConverter.GetBytes((ushort)4105).CopyTo(array, 34);
		HslHelper.HslRandom.GetBytes(4).CopyTo(array, 36);
		array[40] = base.ConnectionTimeoutMultiplier;
		return array;
	}

	[HslMqttApi("WriteBool", "")]
	public override OperateResult Write(string address, bool value)
	{
		return WriteTag(address, 193, (!value) ? new byte[1] : new byte[1] { 1 });
	}

	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await WriteTagAsync(address, 193, (!value) ? new byte[1] : new byte[1] { 1 }).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"InovanceConnectedCipNet<{CommunicationPipe}>";
	}
}
