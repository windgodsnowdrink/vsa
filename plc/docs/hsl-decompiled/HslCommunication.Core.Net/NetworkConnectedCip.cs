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
using System.Threading;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.AllenBradley;

namespace HslCommunication.Core.Net;

/// <summary>
/// 基于连接的CIP协议的基类
/// </summary>
public class NetworkConnectedCip : DeviceTcpNet
{
	private SoftIncrementCount incrementCount = new SoftIncrementCount(65535L, 3L, 2);

	private long openForwardId = 256L;

	private long context = 0L;

	/// <summary>
	/// The current session handle, which is determined by the PLC when communicating with the PLC handshake
	/// </summary>
	public uint SessionHandle { get; protected set; }

	/// <summary>
	/// O -> T Network Connection ID
	/// </summary>
	public uint OTConnectionId { get; set; } = 0u;

	/// <summary>
	/// T -> O Network Connection ID
	/// </summary>
	public uint TOConnectionId { get; set; } = 0u;

	protected override INetMessage GetNewNetMessage()
	{
		return new AllenBradleyMessage();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(177);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		long currentValue = incrementCount.GetCurrentValue();
		memoryStream.WriteByte(BitConverter.GetBytes(currentValue)[0]);
		memoryStream.WriteByte(BitConverter.GetBytes(currentValue)[1]);
		memoryStream.Write(command);
		byte[] array = memoryStream.ToArray();
		memoryStream.Dispose();
		BitConverter.GetBytes((ushort)(array.Length - 4)).CopyTo(array, 2);
		return AllenBradleyHelper.PackRequestHeader(112, SessionHandle, AllenBradleyHelper.PackCommandSpecificData(GetOTConnectionIdService(), array));
	}

	/// <summary>
	/// 从握手报文里提取ConnectionId的方法
	/// </summary>
	/// <param name="content">原始报文</param>
	protected virtual void ExtraConnectionId(byte[] content)
	{
		OTConnectionId = base.ByteTransform.TransUInt32(content, 44);
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.RegisterSessionHandle(BitConverter.GetBytes(Interlocked.Increment(ref context))), hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = AllenBradleyHelper.CheckResponse(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		uint num = base.ByteTransform.TransUInt32(operateResult.Content, 4);
		for (int i = 0; i < 10; i++)
		{
			long value = Interlocked.Increment(ref openForwardId);
			ushort connectionID = ((i < 7) ? ((ushort)i) : ((ushort)HslHelper.HslRandom.Next(7, 200)));
			OperateResult<byte[]> operateResult3 = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, num, GetLargeForwardOpen(connectionID), base.ByteTransform.TransByte(value)), hasResponseData: true, usePackAndUnpack: false);
			if (!operateResult3.IsSuccess)
			{
				return operateResult3;
			}
			try
			{
				if (operateResult3.Content.Length >= 46 && operateResult3.Content[42] != 0)
				{
					ushort num2 = base.ByteTransform.TransUInt16(operateResult3.Content, 44);
					if (num2 == 256 && i < 9)
					{
						continue;
					}
					return num2 switch
					{
						256 => new OperateResult("Connection in use or duplicate Forward Open"), 
						275 => new OperateResult("Extended Status: Out of connections (0x0113)"), 
						_ => new OperateResult("Forward Open failed, Code: " + base.ByteTransform.TransUInt16(operateResult3.Content, 44)), 
					};
				}
				ExtraConnectionId(operateResult3.Content);
			}
			catch (Exception ex)
			{
				return new OperateResult(ex.Message + Environment.NewLine + "Source: " + operateResult3.Content.ToHexString(' '));
			}
			break;
		}
		incrementCount.ResetCurrentValue();
		SessionHandle = num;
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult ExtraOnDisconnect()
	{
		if (CommunicationPipe == null)
		{
			return OperateResult.CreateSuccessResult();
		}
		byte[] largeForwardClose = GetLargeForwardClose();
		if (largeForwardClose != null)
		{
			OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, SessionHandle, largeForwardClose), hasResponseData: true, usePackAndUnpack: false);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.UnRegisterSessionHandle(SessionHandle), hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult<byte[]> read1 = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.RegisterSessionHandle(BitConverter.GetBytes(Interlocked.Increment(ref context))), hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
		if (!read1.IsSuccess)
		{
			return read1;
		}
		OperateResult check = AllenBradleyHelper.CheckResponse(read1.Content);
		if (!check.IsSuccess)
		{
			return check;
		}
		uint sessionHandle = base.ByteTransform.TransUInt32(read1.Content, 4);
		for (int i = 0; i < 10; i++)
		{
			long id = Interlocked.Increment(ref openForwardId);
			OperateResult<byte[]> read2 = await ReadFromCoreServerAsync(send: AllenBradleyHelper.PackRequestHeader(111, sessionHandle, GetLargeForwardOpen((i < 7) ? ((ushort)i) : ((ushort)HslHelper.HslRandom.Next(7, 200))), base.ByteTransform.TransByte(id)), pipe: CommunicationPipe, hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
			if (!read2.IsSuccess)
			{
				return read2;
			}
			try
			{
				if (read2.Content.Length >= 46 && read2.Content[42] != 0)
				{
					ushort err = base.ByteTransform.TransUInt16(read2.Content, 44);
					if (err == 256 && i < 9)
					{
						continue;
					}
					return err switch
					{
						256 => new OperateResult("Connection in use or duplicate Forward Open"), 
						275 => new OperateResult("Extended Status: Out of connections (0x0113)"), 
						_ => new OperateResult("Forward Open failed, Code: " + base.ByteTransform.TransUInt16(read2.Content, 44)), 
					};
				}
				ExtraConnectionId(read2.Content);
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return new OperateResult(ex2.Message + Environment.NewLine + "Source: " + read2.Content.ToHexString(' '));
			}
			break;
		}
		incrementCount.ResetCurrentValue();
		SessionHandle = sessionHandle;
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> ExtraOnDisconnectAsync()
	{
		if (CommunicationPipe == null)
		{
			return OperateResult.CreateSuccessResult();
		}
		byte[] forwardClose = GetLargeForwardClose();
		if (forwardClose != null)
		{
			OperateResult<byte[]> close = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, SessionHandle, forwardClose), hasResponseData: true, usePackAndUnpack: false);
			if (!close.IsSuccess)
			{
				return close;
			}
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.UnRegisterSessionHandle(SessionHandle), hasResponseData: true, usePackAndUnpack: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 将多个的CIP命令打包成一个服务的命令
	/// </summary>
	/// <param name="cip">CIP命令列表</param>
	/// <returns>服务命令</returns>
	protected byte[] PackCommandService(params byte[][] cip)
	{
		if (cip.Length == 1)
		{
			return cip[0];
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(new byte[6] { 10, 2, 32, 2, 36, 1 }, 0, 6);
		memoryStream.WriteByte(BitConverter.GetBytes(cip.Length)[0]);
		memoryStream.WriteByte(BitConverter.GetBytes(cip.Length)[1]);
		int num = 2 + cip.Length * 2;
		for (int i = 0; i < cip.Length; i++)
		{
			memoryStream.WriteByte(BitConverter.GetBytes(num)[0]);
			memoryStream.WriteByte(BitConverter.GetBytes(num)[1]);
			num += cip[i].Length;
		}
		for (int j = 0; j < cip.Length; j++)
		{
			memoryStream.Write(cip[j], 0, cip[j].Length);
		}
		return memoryStream.ToArray();
	}

	/// <summary>
	/// 获取数据通信的前置打开命令，不同的PLC的信息不一样。
	/// </summary>
	/// <param name="connectionID">连接的ID信息</param>
	/// <returns>原始命令数据</returns>
	protected virtual byte[] GetLargeForwardOpen(ushort connectionID)
	{
		return "\r\n00 00 00 00 00 00 02 00 00 00 00 00 b2 00 34 00\r\n5b 02 20 06 24 01 0e 9c 02 00 00 80 01 00 fe 80\r\n02 00 1b 05 30 a7 2b 03 02 00 00 00 80 84 1e 00\r\ncc 07 00 42 80 84 1e 00 cc 07 00 42 a3 03 20 02\r\n24 01 2c 01".ToHexBytes();
	}

	/// <summary>
	/// 获取数据通信的后置关闭命令，不同的PLC的信息不一样。
	/// </summary>
	/// <returns>原始命令数据</returns>
	protected virtual byte[] GetLargeForwardClose()
	{
		return null;
	}

	private byte[] GetOTConnectionIdService()
	{
		byte[] buffer = new byte[8] { 161, 0, 4, 0, 0, 0, 0, 0 };
		base.ByteTransform.TransByte(OTConnectionId).CopyTo(buffer, 4);
		return buffer;
	}

	/// <summary>
	/// 从PLC反馈的数据解析出真实的数据内容，结果内容分别是原始字节数据，数据类型代码，是否有很多的数据
	/// The real data content is parsed from the data fed back by the PLC. The result content is the original byte data,
	/// the data type code, and whether there is a lot of data.
	/// </summary>
	/// <param name="response">PLC的反馈数据</param>
	/// <param name="isRead">是否是返回的操作</param>
	/// <returns>带有结果标识的最终数据</returns>
	public static OperateResult<byte[], ushort, bool> ExtractActualData(byte[] response, bool isRead)
	{
		List<byte> list = new List<byte>();
		try
		{
			int num = 42;
			bool value = false;
			ushort value2 = 0;
			ushort num2 = BitConverter.ToUInt16(response, num);
			if (BitConverter.ToInt32(response, 46) == 138)
			{
				num = 50;
				int num3 = BitConverter.ToUInt16(response, num);
				for (int i = 0; i < num3; i++)
				{
					int num4 = BitConverter.ToUInt16(response, num + 2 + i * 2) + num;
					int num5 = ((i == num3 - 1) ? response.Length : (BitConverter.ToUInt16(response, num + 4 + i * 2) + num));
					ushort num6 = BitConverter.ToUInt16(response, num4 + 2);
					switch (num6)
					{
					case 4:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley04
						};
					case 5:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley05
						};
					case 6:
						if (response[num + 2] == 210 || response[num + 2] == 204)
						{
							return new OperateResult<byte[], ushort, bool>
							{
								ErrorCode = num6,
								Message = StringResources.Language.AllenBradley06
							};
						}
						break;
					case 10:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley0A
						};
					case 12:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley0C
						};
					case 19:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley13
						};
					case 28:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley1C
						};
					case 30:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley1E
						};
					case 38:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley26
						};
					default:
						return new OperateResult<byte[], ushort, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.UnknownError
						};
					case 0:
						break;
					}
					if (isRead)
					{
						for (int j = num4 + 6; j < num5; j++)
						{
							list.Add(response[j]);
						}
					}
				}
			}
			else
			{
				byte b = response[num + 6];
				switch (b)
				{
				case 4:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley04
					};
				case 5:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley05
					};
				case 6:
					value = true;
					break;
				case 10:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley0A
					};
				case 12:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley0C
					};
				case 19:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley13
					};
				case 28:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley1C
					};
				case 30:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley1E
					};
				case 38:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.AllenBradley26
					};
				default:
					return new OperateResult<byte[], ushort, bool>
					{
						ErrorCode = b,
						Message = StringResources.Language.UnknownError
					};
				case 0:
					break;
				}
				if (response[num + 4] == 205 || response[num + 4] == 211)
				{
					return OperateResult.CreateSuccessResult(list.ToArray(), value2, value);
				}
				if (response[num + 4] == 204 || response[num + 4] == 210)
				{
					for (int k = num + 10; k < num + 2 + num2; k++)
					{
						list.Add(response[k]);
					}
					value2 = BitConverter.ToUInt16(response, num + 8);
				}
				else if (response[num + 4] == 213)
				{
					for (int l = num + 8; l < num + 2 + num2; l++)
					{
						list.Add(response[l]);
					}
				}
				else if (response[num + 4] == 203)
				{
					if (response[58] != 0)
					{
						return new OperateResult<byte[], ushort, bool>(response[58], AllenBradleyDF1Serial.GetExtStatusDescription(response[58]) + Environment.NewLine + "Source: " + response.RemoveBegin(57).ToHexString(' '));
					}
					if (!isRead)
					{
						return OperateResult.CreateSuccessResult(list.ToArray(), value2, value);
					}
					return OperateResult.CreateSuccessResult(response.RemoveBegin(61), value2, value);
				}
			}
			return OperateResult.CreateSuccessResult(list.ToArray(), value2, value);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[], ushort, bool>("ExtractActualData failed: " + ex.Message + Environment.NewLine + "Source: " + response.ToHexString(' '));
		}
	}
}
