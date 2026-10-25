// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Instrument.DLT.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Instrument.DLT;

/// <summary>
/// DLT645协议的虚拟服务器
/// </summary>
public class DLT645Server : DeviceServer
{
	protected class DLTAddress
	{
		public string Address { get; set; }

		public byte[] Buffer { get; set; }

		public int Digtal { get; set; }

		public DLTAddress()
		{
		}

		public DLTAddress(string address, byte[] buffer, int digtal)
		{
			Address = address;
			Buffer = buffer;
			Digtal = digtal;
		}
	}

	/// <summary>
	/// 服务器的所有数据信息
	/// </summary>
	protected Dictionary<string, DLTAddress> allDatas = new Dictionary<string, DLTAddress>();

	private string station = "1";

	/// <summary>
	/// 获取或设置当前的地址域信息，是一个12个字符的BCD码，例如：149100007290
	/// Get or set the current address domain information, which is a 12-character BCD code, for example: 149100007290
	/// </summary>
	public virtual string Station
	{
		get
		{
			return station;
		}
		set
		{
			station = value;
		}
	}

	/// <summary>
	/// 字符串的数据是否发生反转，默认为 True, 将根据高地位进行翻转操作
	/// Whether the data of the string is reversed or not, the default is True, and the flip operation will be performed according to the high status
	/// </summary>
	public bool StringReverse { get; set; } = true;

	/// <summary>
	/// 获取或设置是否在每一次的报文通信时，增加"FE FE FE FE"的命令头
	/// Get or set whether to add the command header of "FE FE FE FE" in each message communication
	/// </summary>
	public bool EnableCodeFE { get; set; }

	/// <summary>
	/// 是否对客户端及主站的站号进行强制校验，只有两者站号匹配才进行返回数据，默认为false，对站号信息不校验操作
	/// Whether to forcibly verify the station ids of the client and the master station. The data is returned only when the station ids of the client and master station match. The default value is false
	/// </summary>
	public bool StationMatch { get; set; } = false;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public DLT645Server()
	{
		CreateAddressTags();
		base.SerialReceiveAtleastTime = 2000;
	}

	/// <summary>
	/// 创建一系列的地址标签
	/// </summary>
	protected virtual void CreateAddressTags()
	{
		AddDltTag("02010100", "2311", 1);
		AddDltTag("02010200", "2312", 1);
		AddDltTag("02010300", "2313", 1);
		AddDltTag("02020100", "012345", 3);
		AddDltTag("02020200", "012346", 3);
		AddDltTag("02020300", "012347", 3);
		AddDltTag("02030000", "123456", 4);
		AddDltTag("02030100", "123457", 4);
		AddDltTag("02030200", "123458", 4);
		AddDltTag("02030300", "123459", 4);
		AddDltTag("02040000", "654321", 4);
		AddDltTag("02040100", "654322", 4);
		AddDltTag("02040200", "654323", 4);
		AddDltTag("02040300", "654324", 4);
		AddDltTag("02050000", "553322", 4);
		AddDltTag("02050100", "553323", 4);
		AddDltTag("02050200", "553324", 4);
		AddDltTag("02050300", "553325", 4);
		AddDltTag("02060000", "1236", 3);
		AddDltTag("02060100", "1237", 3);
		AddDltTag("02060200", "1238", 3);
		AddDltTag("02060300", "1239", 3);
		AddDltTag("02070100", "2345", 1);
		AddDltTag("02070200", "2346", 1);
		AddDltTag("02070300", "2347", 1);
		AddDltTag("02080001", "123789", 3);
		AddDltTag("02080002", "5012", 2);
		AddDltTag("02080003", "112233", 4);
		AddDltTag("02080004", "223344", 4);
		AddDltTag("02080005", "223355", 4);
		AddDltTag("02080006", "223366", 4);
		AddDltTag("02080007", "2312", 1);
		AddDltTag("02080008", "1221", 2);
		AddDltTag("02080009", "1223", 2);
		AddDltTag("0208000A", "11223344", 0);
		AddDltTag("00000000", "12345612", 2);
		AddDltTag("00010000", "12345613", 2);
		AddDltTag("00020000", "12345614", 2);
		AddDltTag("00030000", "12345615", 2);
		AddDltTag("04000101", "23110904", -1);
		AddDltTag("04000102", DateTime.Now.ToString("HH:mm:ss").Replace(":", ""), -1);
		AddDltTag("04000401", "112233665544", -1);
		AddDltTag("04000402", "552233665544", -1);
		AddDltAsciiTag("0400040B", "DLTServer ", -1);
		AddDltAsciiTag("0400040C", "2023111101 ", -1);
	}

	/// <summary>
	/// 新增一个DLT的Tag标签
	/// </summary>
	/// <param name="address">地址名称</param>
	/// <param name="hex">数据内容</param>
	protected void AddDltTag(string address, string hex, int digtal)
	{
		allDatas.Add(address, new DLTAddress(address, hex.ToHexBytes(), digtal));
	}

	/// <summary>
	/// 新增一个DLT的Tag标签
	/// </summary>
	/// <param name="address">地址名称</param>
	/// <param name="hex">数据内容</param>
	protected void AddDltTag(string address, byte[] data, int digtal)
	{
		allDatas.Add(address, new DLTAddress(address, data, digtal));
	}

	/// <summary>
	/// 新增一个DLT的Tag标签
	/// </summary>
	/// <param name="address">地址名称</param>
	/// <param name="hex">数据内容</param>
	protected void AddDltAsciiTag(string address, string ascii, int digtal)
	{
		allDatas.Add(address, new DLTAddress(address, Encoding.ASCII.GetBytes(ascii), digtal));
	}

	/// <summary>
	/// 获取当前的DLT645协议类型
	/// </summary>
	/// <returns>DLT类型信息</returns>
	protected virtual DLT645Type GetLT645Type()
	{
		return DLT645Type.DLT2007;
	}

	[HslMqttApi("ReadDoubleArray", "")]
	/// <summary>
	/// 读取双浮点数据的数组
	/// Read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		OperateResult<string[]> operateResult = ReadStringArray(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double[]>(operateResult);
		}
		try
		{
			return OperateResult.CreateSuccessResult((from m in operateResult.Content.Take(length)
				select double.Parse(m)).ToArray());
		}
		catch (Exception ex)
		{
			return new OperateResult<double[]>("double.Parse failed: " + ex.Message + Environment.NewLine + "Source: " + operateResult.Content.ToArrayString());
		}
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromArray(ReadStringArray(address));
	}

	[HslMqttApi("WriteDouble", "")]
	/// <summary>
	/// 写入double数据，返回是否成功
	/// Write double data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, double value)
	{
		OperateResult<string, byte[]> operateResult = DLT645Helper.AnalysisBytesAddress(GetLT645Type(), address, Station, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(operateResult);
		}
		string key = Enumerable.Reverse(operateResult.Content2).ToArray().ToHexString();
		if (allDatas.ContainsKey(key))
		{
			value *= Math.Pow(10.0, allDatas[key].Digtal);
			string text = Convert.ToInt32(value).ToString().PadLeft(allDatas[key].Buffer.Length * 2, '0');
			if (text.Length > allDatas[key].Buffer.Length * 2)
			{
				text = text.Substring(text.Length - allDatas[key].Buffer.Length * 2);
			}
			text.ToHexBytes().CopyTo(allDatas[key].Buffer, 0);
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult<string[]>("None address data");
	}

	/// <summary>
	/// 写入字符串信息，需要指定的编码信息
	/// Write string information, need to specify the encoding information
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <param name="encoding">指定的编码信息</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		OperateResult<string, byte[]> operateResult = DLT645Helper.AnalysisBytesAddress(GetLT645Type(), address, Station, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(operateResult);
		}
		string key = Enumerable.Reverse(operateResult.Content2).ToArray().ToHexString();
		if (allDatas.ContainsKey(key))
		{
			byte[] array = value.PadLeft(allDatas[key].Buffer.Length * 2, '0').ToHexBytes();
			if (array.Length > allDatas[key].Buffer.Length)
			{
				array = array.SelectBegin(allDatas[key].Buffer.Length);
			}
			array.CopyTo(allDatas[key].Buffer, 0);
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult<string[]>("None address data");
	}

	/// <summary>
	/// 根据指定的地址，读取DLT设备的字符串格式的数据信息
	/// Read the DLT device data in string format based on the specified address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>包含字符串结果数组的对象</returns>
	public virtual OperateResult<string[]> ReadStringArray(string address)
	{
		bool reverse = HslHelper.ExtractBooleanParameter(ref address, "reverse", defaultValue: true);
		OperateResult<string, byte[]> operateResult = DLT645Helper.AnalysisBytesAddress(GetLT645Type(), address, Station, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(operateResult);
		}
		string key = Enumerable.Reverse(operateResult.Content2).ToArray().ToHexString();
		if (allDatas.ContainsKey(key))
		{
			return DLTTransform.TransStringsFromDLt(GetLT645Type(), Enumerable.Reverse(allDatas[key].Buffer).ToArray().EveryByteAdd(51), operateResult.Content2, reverse);
		}
		return new OperateResult<string[]>("None address data");
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new DLT645Message();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length == 4 && receive[0] == 254 && receive[1] == 254 && receive[2] == 254 && receive[3] == 254)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		if (receive.Length < 5)
		{
			return new OperateResult<byte[]>("Uknown message");
		}
		int num = DLT645Helper.FindHeadCode68H(receive);
		if (num < 0)
		{
			return new OperateResult<byte[]>("Uknown message: " + receive.ToHexString(' '));
		}
		if (num > 0)
		{
			receive = receive.RemoveBegin(num);
		}
		return ReadFromDLTCore(receive);
	}

	private OperateResult CheckStationMatch(byte[] command)
	{
		if (CheckAddress(command, 170))
		{
			return OperateResult.CreateSuccessResult();
		}
		if (!StationMatch)
		{
			return OperateResult.CreateSuccessResult();
		}
		string text = command.SelectMiddle(1, 6).ReverseNew().ToHexString();
		try
		{
			if (Convert.ToInt32(text) != Convert.ToInt32(station))
			{
				return new OperateResult("Station not match, need[" + station + "] actual[" + text + "]");
			}
			return OperateResult.CreateSuccessResult();
		}
		catch
		{
			return OperateResult.CreateSuccessResult();
		}
	}

	private bool CheckAddress(byte[] command, byte match)
	{
		for (int i = 1; i < 7; i++)
		{
			if (command[i] != match)
			{
				return false;
			}
		}
		return true;
	}

	private OperateResult<byte[]> CreateResponseBack(byte err, byte[] command, byte[] data, bool withAddress)
	{
		if ((err == 0) & withAddress)
		{
			data = SoftBasic.SpliceArray<byte>(command.SelectMiddle(10, GetDataIdLength()).EveryByteAdd(-51), data);
		}
		byte[] array = null;
		string address = station;
		if (!CheckAddress(command, 170))
		{
			address = command.SelectMiddle(1, 6).ReverseNew().ToHexString();
		}
		array = ((err <= 0) ? DLT645Helper.BuildDlt645EntireCommand(address, (byte)(command[8] + 128), data).Content : DLT645Helper.BuildDlt645EntireCommand(address, (byte)(command[8] + 192), new byte[1] { err }).Content);
		if (EnableCodeFE)
		{
			array = SoftBasic.SpliceArray<byte>(new byte[4] { 254, 254, 254, 254 }, array);
		}
		return OperateResult.CreateSuccessResult(array);
	}

	private bool IsCommandRead(byte command)
	{
		if (GetLT645Type() == DLT645Type.DLT2007)
		{
			return command == 17;
		}
		return command == 1;
	}

	private bool IsCommandWrite(byte command)
	{
		if (GetLT645Type() == DLT645Type.DLT2007)
		{
			return command == 20;
		}
		return command == 4;
	}

	private int GetDataIdLength()
	{
		if (GetLT645Type() == DLT645Type.DLT2007)
		{
			return 4;
		}
		return 2;
	}

	/// <summary>
	/// 从DLT协议中读取数据信息
	/// </summary>
	/// <param name="receive">接收到的报文数据信息</param>
	/// <returns>返回的报文结果数据</returns>
	protected virtual OperateResult<byte[]> ReadFromDLTCore(byte[] receive)
	{
		int dataIdLength = GetDataIdLength();
		if (IsCommandRead(receive[8]))
		{
			OperateResult operateResult = CheckStationMatch(receive);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			string key = Enumerable.Reverse(receive.SelectMiddle(10, dataIdLength).EveryByteAdd(-51)).ToArray().ToHexString();
			if (allDatas.ContainsKey(key))
			{
				if (allDatas[key].Digtal < 0 && !StringReverse)
				{
					return CreateResponseBack(0, receive, Enumerable.Reverse(allDatas[key].Buffer).ToArray(), withAddress: true);
				}
				return CreateResponseBack(0, receive, Enumerable.Reverse(allDatas[key].Buffer).ToArray(), withAddress: true);
			}
			return CreateResponseBack(2, receive, null, withAddress: true);
		}
		if (receive[8] == 19)
		{
			return CreateResponseBack(0, receive, station.PadLeft(12, '0').ToHexBytes(), withAddress: false);
		}
		if (IsCommandWrite(receive[8]))
		{
			OperateResult operateResult2 = CheckStationMatch(receive);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult2);
			}
			string key2 = Enumerable.Reverse(receive.SelectMiddle(10, dataIdLength).EveryByteAdd(-51)).ToArray().ToHexString();
			if (allDatas.ContainsKey(key2))
			{
				byte[] source = ((GetLT645Type() == DLT645Type.DLT2007) ? receive.SelectMiddle(22, receive.Length - 24) : receive.SelectMiddle(12, receive.Length - 14));
				Enumerable.Reverse(source).ToArray().EveryByteAdd(-51)
					.CopyTo(allDatas[key2].Buffer, 0);
				return CreateResponseBack(0, receive, new byte[0], withAddress: false);
			}
			return CreateResponseBack(2, receive, null, withAddress: false);
		}
		if (receive[8] == 21)
		{
			OperateResult operateResult3 = CheckStationMatch(receive);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult3);
			}
			string text = receive.SelectMiddle(10, 6).EveryByteAdd(-51).ReverseNew()
				.ToHexString();
			base.LogNet?.WriteInfo(ToString(), "Station change, " + station + " -> " + text);
			station = text;
			return CreateResponseBack(0, receive, new byte[0], withAddress: false);
		}
		if (receive[8] == 8)
		{
			string text2 = Enumerable.Reverse(receive.SelectMiddle(13, 3).EveryByteAdd(-51)).ToArray().ToHexString();
			Write("04000101", text2 + "00");
			string value = Enumerable.Reverse(receive.SelectMiddle(10, 3).EveryByteAdd(-51)).ToArray().ToHexString();
			Write("04000102", value);
			return new OperateResult<byte[]>
			{
				ErrorCode = int.MinValue
			};
		}
		if (receive[8] == 22)
		{
			base.LogNet?.WriteDebug(ToString(), "FreezeCommand: " + receive.SelectMiddle(10, 4).ToHexString(' '));
			if (receive[1] != 153 || receive[2] != 153 || receive[3] != 153 || receive[4] != 153 || receive[5] != 153 || receive[6] != 153)
			{
				return CreateResponseBack(0, receive, new byte[0], withAddress: false);
			}
		}
		else
		{
			if (receive[8] == 23)
			{
				int num = receive[10] - 51;
				string text3 = num switch
				{
					64 => "19200", 
					32 => "9600", 
					16 => "4800", 
					8 => "2400", 
					4 => "1200", 
					2 => "600", 
					_ => $"Unkown[{receive[10]}]", 
				};
				base.LogNet?.WriteDebug(ToString(), "Modify baud rate: " + text3);
				if (text3.StartsWith("Unkown"))
				{
					return CreateResponseBack(3, receive, null, withAddress: false);
				}
				return CreateResponseBack(0, receive, new byte[1] { (byte)num }, withAddress: false);
			}
			if (receive[8] == 28)
			{
				return CreateResponseBack(0, receive, new byte[0], withAddress: false);
			}
		}
		return CreateResponseBack(2, receive, null, withAddress: false);
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(buffer.SelectBegin(receivedLength));
		return DLT645Helper.CheckReceiveDataComplete(memoryStream);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"DLT645Server[{base.Port}]";
	}
}
