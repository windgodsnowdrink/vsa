// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core.IMessage;

/// <summary>
/// SAM身份证通信协议的消息
/// </summary>
public class SAMMessage : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => 7;

	/// <summary>
	/// 检查头子节的合法性
	/// Check the legitimacy of the head subsection
	/// </summary>
	/// <param name="token">特殊的令牌，有些特殊消息的验证</param>
	/// <returns>是否成功的结果</returns>
	public override bool CheckHeadBytesLegal(byte[] token)
	{
		if (base.HeadBytes == null)
		{
			return true;
		}
		return base.HeadBytes[0] == 170 && base.HeadBytes[1] == 170 && base.HeadBytes[2] == 170 && base.HeadBytes[3] == 150 && base.HeadBytes[4] == 105;
	}

	public int GetContentLengthByHeadBytes()
	{
		byte[] headBytes = base.HeadBytes;
		if (headBytes != null && headBytes.Length >= 7)
		{
			return base.HeadBytes[5] * 256 + base.HeadBytes[6];
		}
		return 0;
	}
}
