// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;

namespace HslCommunication.BasicFramework;

internal class UrlDecoder
{
	private int _bufferSize;

	private int _numChars;

	private char[] _charBuffer;

	private int _numBytes;

	private byte[] _byteBuffer;

	private Encoding _encoding;

	private void FlushBytes()
	{
		if (_numBytes > 0)
		{
			_numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
			_numBytes = 0;
		}
	}

	internal UrlDecoder(int bufferSize, Encoding encoding)
	{
		_bufferSize = bufferSize;
		_encoding = encoding;
		_charBuffer = new char[bufferSize];
	}

	internal void AddChar(char ch)
	{
		if (_numBytes > 0)
		{
			FlushBytes();
		}
		_charBuffer[_numChars++] = ch;
	}

	internal void AddByte(byte b)
	{
		if (_byteBuffer == null)
		{
			_byteBuffer = new byte[_bufferSize];
		}
		_byteBuffer[_numBytes++] = b;
	}

	internal string GetString()
	{
		if (_numBytes > 0)
		{
			FlushBytes();
		}
		if (_numChars > 0)
		{
			return new string(_charBuffer, 0, _numChars);
		}
		return string.Empty;
	}
}
