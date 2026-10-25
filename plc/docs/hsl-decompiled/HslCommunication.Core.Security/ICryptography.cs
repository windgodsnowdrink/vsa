// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core.Security;

/// <summary>
/// 加密解密的数据接口
/// Encrypted and decrypted data interface
/// </summary>
public interface ICryptography
{
	/// <summary>
	/// 当前加密的密钥信息
	/// currently encrypted key information
	/// </summary>
	string Key { get; }

	/// <summary>
	/// 对原始的数据进行加密的操作，返回加密之后的二进制原始数据
	/// Encrypt the original data and return the encrypted binary original data
	/// </summary>
	/// <param name="data">等待加密的数据</param>
	/// <returns>加密之后的二进制数据</returns>
	byte[] Encrypt(byte[] data);

	/// <summary>
	/// 对原始的数据进行解密的操作，返回解密之后的二进制原始数据
	/// Decrypt the original data and return the decrypted binary original data
	/// </summary>
	/// <param name="data">等待解密的数据</param>
	/// <returns>解密之后的原始二进制数据</returns>
	byte[] Decrypt(byte[] data);

	/// <summary>
	/// 针对字符串进行加密，并返回加密后的字符串数据，字符串的编码默认为UTF8，加密后返回Base64编码
	/// Encrypt the string and return the encrypted string data. The encoding of the string is UTF8 by default. After encryption, the Base64 encoding is returned.
	/// </summary>
	/// <param name="data">等待加密的字符串</param>
	/// <returns>加密后的Base64编码</returns>
	string Encrypt(string data);

	/// <summary>
	/// 针对Base64字符串进行解密操作，转为二进制数据后进行解密，解密之后使用UTF8编码获取最终的字符串数据
	/// Decrypt the Base64 string, convert it to binary data, and decrypt it. After decryption, use UTF8 encoding to obtain the final string data.
	/// </summary>
	/// <param name="data">base64编码的字符串数据</param>
	/// <returns>最终的解析完成的字符串</returns>
	string Decrypt(string data);
}
