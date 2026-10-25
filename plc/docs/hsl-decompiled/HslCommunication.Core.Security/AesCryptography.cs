// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Security.Cryptography;
using System.Text;

namespace HslCommunication.Core.Security;

/// <summary>
/// 实例化一个AES加密解密的对象，默认  模式的对象
/// </summary>
public class AesCryptography : ICryptography
{
	private ICryptoTransform encryptTransform;

	private ICryptoTransform decryptTransform;

	private RijndaelManaged rijndael;

	private string key;

	public string Key => key;

	/// <summary>
	/// 使用指定的密钥实例化一个AES加密解密的对象，密钥由32位数字或字母组成，例如 12345678123456781234567812345678
	/// </summary>
	/// <param name="key">密钥</param>
	/// <param name="mode">加密的模式，默认为</param>
	public AesCryptography(string key, CipherMode mode = CipherMode.ECB)
	{
		this.key = key;
		rijndael = new RijndaelManaged
		{
			Key = Encoding.UTF8.GetBytes(key),
			Mode = mode,
			Padding = PaddingMode.PKCS7
		};
		encryptTransform = rijndael.CreateEncryptor();
		decryptTransform = rijndael.CreateDecryptor();
	}

	/// <summary>
	/// 对原始的数据进行加密的操作，返回加密之后的二进制原始数据
	/// Encrypt the original data and return the encrypted binary original data
	/// </summary>
	/// <param name="data">等待加密的数据</param>
	/// <returns>加密之后的二进制数据</returns>
	public byte[] Encrypt(byte[] data)
	{
		if (data == null)
		{
			return null;
		}
		return encryptTransform.TransformFinalBlock(data, 0, data.Length);
	}

	/// <summary>
	/// 对原始的数据进行解密的操作，返回解密之后的二进制原始数据
	/// Decrypt the original data and return the decrypted binary original data
	/// </summary>
	/// <param name="data">等待解密的数据</param>
	/// <returns>解密之后的原始二进制数据</returns>
	public byte[] Decrypt(byte[] data)
	{
		if (data == null)
		{
			return null;
		}
		return decryptTransform.TransformFinalBlock(data, 0, data.Length);
	}

	/// <summary>
	/// 针对字符串进行加密，并返回加密后的字符串数据，字符串的编码默认为UTF8，加密后返回Base64编码
	/// Encrypt the string and return the encrypted string data. The encoding of the string is UTF8 by default. After encryption, the Base64 encoding is returned.
	/// </summary>
	/// <param name="data">等待加密的字符串</param>
	/// <returns>加密后的Base64编码</returns>
	public string Encrypt(string data)
	{
		byte[] bytes = (string.IsNullOrEmpty(data) ? new byte[0] : Encoding.UTF8.GetBytes(data));
		return Convert.ToBase64String(Encrypt(bytes));
	}

	/// <summary>
	/// 针对Base64字符串进行解密操作，转为二进制数据后进行解密，解密之后使用UTF8编码获取最终的字符串数据
	/// Decrypt the Base64 string, convert it to binary data, and decrypt it. After decryption, use UTF8 encoding to obtain the final string data.
	/// </summary>
	/// <param name="data">base64编码的字符串数据</param>
	/// <returns>最终的解析完成的字符串</returns>
	public string Decrypt(string data)
	{
		return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(data)));
	}
}
