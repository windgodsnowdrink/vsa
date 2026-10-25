// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication;

internal class HslSecurity
{
	/// <summary>
	/// 加密方法，只对当前的程序集开放
	/// </summary>
	/// <param name="enBytes">等待加密的数据</param>
	/// <returns>加密后的字节数据</returns>
	internal static byte[] ByteEncrypt(byte[] enBytes)
	{
		if (enBytes == null)
		{
			return null;
		}
		byte[] buffer = new byte[enBytes.Length];
		for (int i = 0; i < enBytes.Length; i++)
		{
			buffer[i] = (byte)(enBytes[i] ^ 0xB5);
		}
		return buffer;
	}

	/// <summary>
	/// 加密方法，只对当前的程序集开放
	/// </summary>
	/// <param name="enBytes">等待加密的数据</param>
	/// <returns>加密后的字节数据</returns>
	internal static void ByteEncrypt(byte[] enBytes, int offset, int count)
	{
		for (int i = offset; i < offset + count && i < enBytes.Length; i++)
		{
			enBytes[i] ^= 0xB5;
		}
	}

	/// <summary>
	/// 解密方法，只对当前的程序集开放
	/// </summary>
	/// <param name="deBytes">等待解密的数据</param>
	/// <returns>解密后的字节数据</returns>
	internal static byte[] ByteDecrypt(byte[] deBytes)
	{
		return ByteEncrypt(deBytes);
	}
}
