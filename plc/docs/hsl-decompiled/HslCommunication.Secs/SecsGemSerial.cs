// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;

namespace HslCommunication.Secs;

/// <summary>
/// 串口类相关的Secs
/// </summary>
public class SecsGemSerial : DeviceSerialPort
{
	private SoftIncrementCount incrementCount;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public SecsGemSerial()
	{
		incrementCount = new SoftIncrementCount(4294967295L, 0L);
		base.ByteTransform = new ReverseBytesTransform();
		base.WordLength = 2;
	}

	/// <summary>
	/// 执行SECS命令
	/// </summary>
	/// <param name="command">命令信息</param>
	/// <returns>是否成功的结果</returns>
	public OperateResult<byte[]> ExecuteCommand(byte[] command)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(new byte[1] { 5 });
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content[0] != 4)
		{
			return new OperateResult<byte[]>($"Send Enq to device, but receive [{operateResult.Content[0]}], need receive [EOT]");
		}
		OperateResult<byte[]> operateResult2;
		do
		{
			operateResult2 = ReadFromCoreServer(command);
		}
		while (operateResult2.IsSuccess);
		return operateResult2;
	}
}
