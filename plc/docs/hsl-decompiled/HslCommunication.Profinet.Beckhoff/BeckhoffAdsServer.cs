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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Beckhoff.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Beckhoff;

/// <summary>
/// 倍福Ads协议的虚拟服务器
/// </summary>
public class BeckhoffAdsServer : DeviceServer
{
	private SoftBuffer mBuffer;

	private SoftBuffer iBuffer;

	private SoftBuffer qBuffer;

	private Dictionary<string, AdsTagItem> adsValues;

	private object dicLock = new object();

	private int memoryAddress;

	private const int DataPoolLength = 65536;

	private Timer timer;

	/// <summary>
	/// 实例化一个基于ADS协议的虚拟的倍福PLC对象，可以用来和进行通信测试。
	/// </summary>
	public BeckhoffAdsServer()
	{
		mBuffer = new SoftBuffer(65536);
		iBuffer = new SoftBuffer(65536);
		qBuffer = new SoftBuffer(65536);
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 2;
		memoryAddress = 1048576;
		adsValues = new Dictionary<string, AdsTagItem>();
		timer = new Timer(ThreadKeepAlive, null, 2000, 2000);
	}

	private void ThreadKeepAlive(object state)
	{
		if (base.IsStarted)
		{
			PipeSession[] pipeSessions = GetCommunicationServer().GetPipeSessions();
			PipeSession[] array = pipeSessions;
			foreach (PipeSession pipeSession in array)
			{
				pipeSession.Communication.Send(AdsHelper.PackAmsTcpHelper(AmsTcpHeaderFlags.RouterNotification, new byte[8]));
			}
		}
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[196608];
		mBuffer.GetBytes().CopyTo(buffer, 0);
		iBuffer.GetBytes().CopyTo(buffer, 65536);
		qBuffer.GetBytes().CopyTo(buffer, 131072);
		return buffer;
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 196608)
		{
			throw new Exception("File is not correct");
		}
		mBuffer.SetBytes(content, 0, 65536);
		iBuffer.SetBytes(content, 65536, 65536);
		qBuffer.SetBytes(content, 131072, 65536);
	}

	private int TransAddressOffset(int indexGroup, int address)
	{
		return indexGroup switch
		{
			61472 => (address >= 128000) ? (address - 128000) : address, 
			61473 => (address >= 1024000) ? (address - 1024000) : address, 
			61488 => (address >= 256000) ? (address - 256000) : address, 
			61489 => (address >= 2048000) ? (address - 2048000) : address, 
			_ => address, 
		};
	}

	private OperateResult<byte[]> TagsAction(string tagName, Func<AdsTagItem, int, OperateResult<byte[]>> action)
	{
		lock (dicLock)
		{
			if (adsValues.ContainsKey(tagName))
			{
				return action(adsValues[tagName], 0);
			}
			if (Regex.IsMatch(tagName, "\\[[0-9]+\\]$"))
			{
				int num = tagName.LastIndexOf('[');
				int num2 = int.Parse(tagName.Substring(num + 1).RemoveLast(1));
				tagName = tagName.Substring(0, num);
				if (adsValues.ContainsKey(tagName))
				{
					num = num2 * adsValues[tagName].TypeLength;
					if (num >= adsValues[tagName].Buffer.Length)
					{
						return new OperateResult<byte[]>(StringResources.Language.AllenBradley04);
					}
					return action(adsValues[tagName], num);
				}
			}
			return new OperateResult<byte[]>(StringResources.Language.AllenBradley04);
		}
	}

	private OperateResult<byte[]> ReadTags(string tagName, int length)
	{
		return TagsAction(tagName, (AdsTagItem tag, int index) => OperateResult.CreateSuccessResult(tag.Buffer.SelectMiddle(index, Math.Min(length, tag.Buffer.Length - index))));
	}

	private OperateResult WriteTags(string tagName, byte[] value)
	{
		return TagsAction(tagName, delegate(AdsTagItem tag, int index)
		{
			Array.Copy(value, 0, tag.Buffer, index, Math.Min(value.Length, tag.Buffer.Length - index));
			return OperateResult.CreateSuccessResult(new byte[0]);
		});
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 读取PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A
	/// Read PLC data, there are three formats of address, one: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
	/// </summary>
	/// <param name="address">地址信息，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A</param>
	/// <param name="length">长度</param>
	/// <returns>包含是否成功的结果对象</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<uint, uint> operateResult = AdsHelper.AnalysisAddress(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		operateResult.Content2 = (uint)TransAddressOffset((int)operateResult.Content1, (int)operateResult.Content2);
		if (operateResult.Content1 == 61443)
		{
			return ReadTags(address.Substring(2), length);
		}
		return operateResult.Content1 switch
		{
			16416u => OperateResult.CreateSuccessResult(mBuffer.GetBytes((int)operateResult.Content2, length)), 
			61472u => OperateResult.CreateSuccessResult(iBuffer.GetBytes((int)operateResult.Content2, length)), 
			61488u => OperateResult.CreateSuccessResult(qBuffer.GetBytes((int)operateResult.Content2, length)), 
			_ => new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType), 
		};
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 写入PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A
	/// There are three formats for the data written into the PLC. One: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
	/// </summary>
	/// <param name="address">地址信息，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<uint, uint> operateResult = AdsHelper.AnalysisAddress(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		operateResult.Content2 = (uint)TransAddressOffset((int)operateResult.Content1, (int)operateResult.Content2);
		if (operateResult.Content1 == 61443)
		{
			return WriteTags(address.Substring(2), value);
		}
		switch (operateResult.Content1)
		{
		case 16416u:
			mBuffer.SetBytes(value, (int)operateResult.Content2);
			break;
		case 61472u:
			iBuffer.SetBytes(value, (int)operateResult.Content2);
			break;
		case 61488u:
			qBuffer.SetBytes(value, (int)operateResult.Content2);
			break;
		default:
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<uint, uint> operateResult = AdsHelper.AnalysisAddress(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<bool[]>();
		}
		operateResult.Content2 = (uint)TransAddressOffset((int)operateResult.Content1, (int)operateResult.Content2);
		if (operateResult.Content1 == 61443)
		{
			OperateResult<byte[]> operateResult2 = ReadTags(address.Substring(2), length);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult2);
			}
			return OperateResult.CreateSuccessResult(operateResult2.Content.Select((byte m) => m != 0).ToArray());
		}
		return operateResult.Content1 switch
		{
			16417u => OperateResult.CreateSuccessResult(mBuffer.GetBool((int)operateResult.Content2, length)), 
			61473u => OperateResult.CreateSuccessResult(iBuffer.GetBool((int)operateResult.Content2, length)), 
			61489u => OperateResult.CreateSuccessResult(qBuffer.GetBool((int)operateResult.Content2, length)), 
			_ => new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType), 
		};
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		OperateResult<uint, uint> operateResult = AdsHelper.AnalysisAddress(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<bool[]>();
		}
		operateResult.Content2 = (uint)TransAddressOffset((int)operateResult.Content1, (int)operateResult.Content2);
		if (operateResult.Content1 == 61443)
		{
			return WriteTags(address.Substring(2), value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray());
		}
		switch (operateResult.Content1)
		{
		case 16417u:
			mBuffer.SetBool(value, (int)operateResult.Content2);
			break;
		case 61473u:
			iBuffer.SetBool(value, (int)operateResult.Content2);
			break;
		case 61489u:
			qBuffer.SetBool(value, (int)operateResult.Content2);
			break;
		default:
			return new OperateResult(StringResources.Language.NotSupportedDataType);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new AdsNetMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		return OperateResult.CreateSuccessResult(ReadFromAdsCore(session, receive));
	}

	private byte[] PackCommand(byte[] cmd, int err, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		byte[] buffer = new byte[32 + data.Length];
		Array.Copy(cmd, 0, buffer, 0, 32);
		byte[] array2 = buffer.SelectBegin(8);
		byte[] array3 = buffer.SelectMiddle(8, 8);
		array2.CopyTo(buffer, 8);
		array3.CopyTo(buffer, 0);
		buffer[18] = 5;
		buffer[19] = 0;
		BitConverter.GetBytes(data.Length).CopyTo(buffer, 20);
		BitConverter.GetBytes(err).CopyTo(buffer, 24);
		buffer[11] = 0;
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 32);
		}
		return AdsHelper.PackAmsTcpHelper(AmsTcpHeaderFlags.Command, buffer);
	}

	private byte[] PackDataResponse(int err, byte[] data)
	{
		if (data != null)
		{
			byte[] buffer = new byte[8 + data.Length];
			BitConverter.GetBytes(err).CopyTo(buffer, 0);
			BitConverter.GetBytes(data.Length).CopyTo(buffer, 4);
			if (data.Length != 0)
			{
				data.CopyTo(buffer, 8);
			}
			return buffer;
		}
		return BitConverter.GetBytes(err);
	}

	private byte[] ReadFromAdsCore(PipeSession session, byte[] receive)
	{
		switch ((AmsTcpHeaderFlags)BitConverter.ToUInt16(receive, 0))
		{
		case AmsTcpHeaderFlags.Command:
			receive = receive.RemoveBegin(6);
			if (session.Tag == null)
			{
				session.Tag = 1;
				base.LogNet?.WriteDebug(ToString(), "TargetId:" + AdsHelper.GetAmsNetIdString(receive, 0) + " SenderId:" + AdsHelper.GetAmsNetIdString(receive, 8));
			}
			return BitConverter.ToInt16(receive, 16) switch
			{
				2 => ReadByCommand(receive), 
				3 => WriteByCommand(receive), 
				9 => ReadWriteByCommand(receive), 
				_ => PackCommand(receive, 32, null), 
			};
		case AmsTcpHeaderFlags.GetLocalNetId:
			return AdsHelper.PackAmsTcpHelper(AmsTcpHeaderFlags.GetLocalNetId, AdsHelper.StrToAMSNetId("192.168.163.8.1.1"));
		case AmsTcpHeaderFlags.PortConnect:
			return AdsHelper.PackAmsTcpHelper(AmsTcpHeaderFlags.PortConnect, AdsHelper.StrToAMSNetId("192.168.163.9.1.1:32957"));
		default:
			base.LogNet?.WriteDebug(ToString(), "Unknown Source: " + receive.ToHexString(' '));
			return null;
		}
	}

	private OperateResult<byte[]> ReadByCommand(byte[] command, int indexGroup, int address, int length)
	{
		address = TransAddressOffset(indexGroup, address);
		switch (indexGroup)
		{
		case 16416:
			return OperateResult.CreateSuccessResult(mBuffer.GetBytes(address, length));
		case 61472:
			return OperateResult.CreateSuccessResult(iBuffer.GetBytes(address, length));
		case 61488:
			return OperateResult.CreateSuccessResult(qBuffer.GetBytes(address, length));
		case 16417:
			return OperateResult.CreateSuccessResult((from m in mBuffer.GetBool(address, length)
				select (byte)(m ? 1 : 0)).ToArray());
		case 61473:
			return OperateResult.CreateSuccessResult((from m in iBuffer.GetBool(address, length)
				select (byte)(m ? 1 : 0)).ToArray());
		case 61489:
			return OperateResult.CreateSuccessResult((from m in qBuffer.GetBool(address, length)
				select (byte)(m ? 1 : 0)).ToArray());
		case 61445:
		{
			uint num = (uint)address;
			lock (dicLock)
			{
				foreach (AdsTagItem value in adsValues.Values)
				{
					if (value.Location == num)
					{
						return OperateResult.CreateSuccessResult(value.Buffer.SelectBegin(Math.Min(length, value.Buffer.Length)));
					}
				}
				return new OperateResult<byte[]>
				{
					Content = PackCommand(command, 0, PackDataResponse(1808, null))
				};
			}
		}
		default:
			return new OperateResult<byte[]>
			{
				Content = PackCommand(command, 64, null)
			};
		}
	}

	private byte[] ReadByCommand(byte[] command)
	{
		try
		{
			int indexGroup = BitConverter.ToInt32(command, 32);
			int address = BitConverter.ToInt32(command, 36);
			int length = BitConverter.ToInt32(command, 40);
			OperateResult<byte[]> operateResult = ReadByCommand(command, indexGroup, address, length);
			if (!operateResult.IsSuccess)
			{
				return operateResult.Content;
			}
			return PackCommand(command, 0, PackDataResponse(0, operateResult.Content));
		}
		catch
		{
			return PackCommand(command, 164, null);
		}
	}

	private byte[] WriteByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return PackCommand(command, 16, null);
		}
		try
		{
			int num = BitConverter.ToInt32(command, 32);
			int address = BitConverter.ToInt32(command, 36);
			int num2 = BitConverter.ToInt32(command, 40);
			byte[] array = command.RemoveBegin(44);
			address = TransAddressOffset(num, address);
			switch (num)
			{
			case 16416:
				mBuffer.SetBytes(array, address);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61472:
				iBuffer.SetBytes(array, address);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61488:
				qBuffer.SetBytes(array, address);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 16417:
				mBuffer.SetBool(array.Select((byte m) => m != 0).ToArray(), address);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61473:
				iBuffer.SetBool(array.Select((byte m) => m != 0).ToArray(), address);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61489:
				qBuffer.SetBool(array.Select((byte m) => m != 0).ToArray(), address);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61445:
			{
				uint num3 = (uint)address;
				lock (dicLock)
				{
					foreach (AdsTagItem value in adsValues.Values)
					{
						if (value.Location == num3)
						{
							Array.Copy(array, 0, value.Buffer, 0, Math.Min(array.Length, value.Buffer.Length));
							return PackCommand(command, 0, PackDataResponse(0, null));
						}
					}
					return PackCommand(command, 0, PackDataResponse(1808, null));
				}
			}
			default:
				return PackCommand(command, 64, null);
			}
		}
		catch
		{
			return PackCommand(command, 164, null);
		}
	}

	private byte[] ReadWriteByCommand(byte[] command)
	{
		try
		{
			int num = BitConverter.ToInt32(command, 32);
			int destIndex = BitConverter.ToInt32(command, 36);
			int num2 = BitConverter.ToInt32(command, 40);
			int num3 = BitConverter.ToInt32(command, 44);
			byte[] array = command.RemoveBegin(48);
			switch (num)
			{
			case 16416:
				mBuffer.SetBytes(array, destIndex);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61472:
				iBuffer.SetBytes(array, destIndex);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61488:
				qBuffer.SetBytes(array, destIndex);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 16417:
				mBuffer.SetBytes(array, destIndex);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61473:
				iBuffer.SetBytes(array, destIndex);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61489:
				qBuffer.SetBytes(array, destIndex);
				return PackCommand(command, 0, PackDataResponse(0, null));
			case 61443:
			{
				if (array[^1] == 0)
				{
					array = array.RemoveLast(1);
				}
				string key = Encoding.ASCII.GetString(array);
				lock (dicLock)
				{
					if (adsValues.ContainsKey(key))
					{
						return PackCommand(command, 0, PackDataResponse(0, BitConverter.GetBytes(adsValues[key].Location)));
					}
					return PackCommand(command, 0, PackDataResponse(1808, null));
				}
			}
			case 61568:
			{
				List<byte> list = new List<byte>();
				for (int i = 0; i < array.Length / 12; i++)
				{
					int indexGroup = BitConverter.ToInt32(array, 12 * i);
					int address = BitConverter.ToInt32(array, 12 * i + 4);
					int length = BitConverter.ToInt32(array, 12 * i + 8);
					OperateResult<byte[]> operateResult = ReadByCommand(command, indexGroup, address, length);
					if (!operateResult.IsSuccess)
					{
						return operateResult.Content;
					}
					list.AddRange(operateResult.Content);
				}
				return PackCommand(command, 0, PackDataResponse(0, list.ToArray()));
			}
			default:
				return PackCommand(command, 64, null);
			}
		}
		catch
		{
			return PackCommand(command, 164, null);
		}
	}

	/// <summary>
	/// 向服务器新增一个新的Tag值
	/// Add a new tag value to the server
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">标签值</param>
	public void AddTagValue(string key, AdsTagItem value)
	{
		value.Location = (uint)Interlocked.Increment(ref memoryAddress);
		lock (dicLock)
		{
			if (adsValues.ContainsKey(key))
			{
				adsValues[key] = value;
			}
			else
			{
				adsValues.Add(key, value);
			}
		}
	}

	/// <summary>
	/// 向服务器新增一个新的Tag值
	/// Add a new tag value to the server
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">标签值</param>
	public void AddTagValue(string key, bool value)
	{
		AddTagValue(key, new AdsTagItem(key, (!value) ? new byte[1] : new byte[1] { 1 }, 1));
	}

	/// <summary>
	/// 向服务器新增一个新的Tag值
	/// Add a new tag value to the server
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">标签值</param>
	public void AddTagValue(string key, bool[] value)
	{
		AddTagValue(key, new AdsTagItem(key, value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), 1));
	}

	/// <summary>
	/// 向服务器新增一个新的short类型的Tag值，并赋予初始化的值
	/// Add a new short tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, short value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 2));
	}

	/// <summary>
	/// 向服务器新增一个新的short数组的Tag值，并赋予初始化的值
	/// Add a new short array Tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, short[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 2));
	}

	/// <summary>
	/// 向服务器新增一个新的ushort类型的Tag值，并赋予初始化的值
	/// Add a new tag value of ushort type to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, ushort value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 2));
	}

	/// <summary>
	/// 向服务器新增一个新的ushort数组的Tag值，并赋予初始化的值
	/// Add a new ushort array Tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, ushort[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 2));
	}

	/// <summary>
	/// 向服务器新增一个新的int类型的Tag值，并赋予初始化的值
	/// Add a new Tag value of type int to the server and assign the initialized value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, int value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 4));
	}

	/// <summary>
	/// 向服务器新增一个新的int数组的Tag值，并赋予初始化的值
	/// Add a new Tag value of the int array to the server and assign the initialized value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, int[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 4));
	}

	/// <summary>
	/// 向服务器新增一个新的uint类型的Tag值，并赋予初始化的值
	/// Add a new uint tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, uint value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 4));
	}

	/// <summary>
	/// 向服务器新增一个新的uint数组的Tag值，并赋予初始化的值
	/// Add a new uint array Tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, uint[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 4));
	}

	/// <summary>
	/// 向服务器新增一个新的long类型的Tag值，并赋予初始化的值
	/// Add a new Tag value of type long to the server and assign the initialized value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, long value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 8));
	}

	/// <summary>
	/// 向服务器新增一个新的long数组的Tag值，并赋予初始化的值
	/// Add a new Long array Tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, long[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 8));
	}

	/// <summary>
	/// 向服务器新增一个新的ulong类型的Tag值，并赋予初始化的值
	/// Add a new Ulong type Tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, ulong value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 8));
	}

	/// <summary>
	/// 向服务器新增一个新的ulong数组的Tag值，并赋予初始化的值
	/// Add a new Ulong array Tag value to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, ulong[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 8));
	}

	/// <summary>
	/// 向服务器新增一个新的float类型的Tag值，并赋予初始化的值
	/// Add a new tag value of type float to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, float value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 4));
	}

	/// <summary>
	/// 向服务器新增一个新的float数组的Tag值，并赋予初始化的值
	/// Add a new Tag value of the float array to the server and assign the initialized value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, float[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 4));
	}

	/// <summary>
	/// 向服务器新增一个新的double类型的Tag值，并赋予初始化的值
	/// Add a new tag value of type double to the server and assign the initialized value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, double value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 8));
	}

	/// <summary>
	/// 向服务器新增一个新的double数组的Tag值，并赋予初始化的值
	/// Add a new double array Tag value to the server and assign the initialized value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	public void AddTagValue(string key, double[] value)
	{
		AddTagValue(key, new AdsTagItem(key, base.ByteTransform.TransByte(value), 8));
	}

	/// <summary>
	/// 向服务器新增一个新的string类型的Tag值，并赋予初始化的值
	/// Add a new Tag value of string type to the server and assign the initial value
	/// </summary>
	/// <param name="key">Tag名称</param>
	/// <param name="value">值信息</param>
	/// <param name="maxLength">字符串的最长值</param>
	public void AddTagValue(string key, string value, int maxLength)
	{
		byte[] buffer = SoftBasic.ArrayExpandToLength(Encoding.UTF8.GetBytes(value), maxLength);
		AddTagValue(key, new AdsTagItem(key, buffer, maxLength));
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			mBuffer.Dispose();
			iBuffer.Dispose();
			qBuffer.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"BeckhoffAdsServer[{base.Port}]";
	}
}
