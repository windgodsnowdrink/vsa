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
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Serial;

namespace HslCommunication.ModBus;

/// <summary>
/// Modbus协议相关的一些信息，包括功能码定义，报文的生成的定义等等信息
/// Some information related to Modbus protocol, including function code definition, definition of message generation, etc.
/// </summary>
public class ModbusInfo
{
	/// <summary>
	/// 读取线圈
	/// </summary>
	public const byte ReadCoil = 1;

	/// <summary>
	/// 读取离散量
	/// </summary>
	public const byte ReadDiscrete = 2;

	/// <summary>
	/// 读取寄存器
	/// </summary>
	public const byte ReadRegister = 3;

	/// <summary>
	/// 读取输入寄存器
	/// </summary>
	public const byte ReadInputRegister = 4;

	/// <summary>
	/// 写单个线圈
	/// </summary>
	public const byte WriteOneCoil = 5;

	/// <summary>
	/// 写单个寄存器
	/// </summary>
	public const byte WriteOneRegister = 6;

	/// <summary>
	/// 写多个线圈
	/// </summary>
	public const byte WriteCoil = 15;

	/// <summary>
	/// 写多个寄存器
	/// </summary>
	public const byte WriteRegister = 16;

	/// <summary>
	/// 读取文件记录
	/// </summary>
	public const byte ReadFileRecord = 20;

	/// <summary>
	/// 写入文件记录
	/// </summary>
	public const byte WriteFileRecord = 21;

	/// <summary>
	/// 使用掩码的方式写入寄存器
	/// </summary>
	public const byte WriteMaskRegister = 22;

	/// <summary>
	/// 读写同时的功能码，只有极个别的设备可能会支持的功能码
	/// </summary>
	public const byte ReadWrite = 23;

	/// <summary>
	/// 不支持该功能码
	/// </summary>
	public const byte FunctionCodeNotSupport = 1;

	/// <summary>
	/// 该地址越界
	/// </summary>
	public const byte FunctionCodeOverBound = 2;

	/// <summary>
	/// 读取长度超过最大值
	/// </summary>
	public const byte FunctionCodeQuantityOver = 3;

	/// <summary>
	/// 读写异常
	/// </summary>
	public const byte FunctionCodeReadWriteException = 4;

	private static void CheckModbusAddressStart(ModbusAddress mAddress, bool isStartWithZero)
	{
		if (!isStartWithZero)
		{
			if (mAddress.AddressStart < 1)
			{
				throw new Exception(StringResources.Language.ModbusAddressMustMoreThanOne);
			}
			mAddress.AddressStart--;
		}
		if (mAddress.AddressStart >= 65536)
		{
			throw new Exception(StringResources.Language.ModbusAddressMustLessThan65536);
		}
	}

	/// <summary>
	/// 构建Modbus读取数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码应该根据bool或是字来区分
	/// To construct the core message of Modbus reading data, you need to specify the address, length, station number,
	/// whether the starting address is 0, and the default function code should be distinguished according to bool or word
	/// </summary>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[][]> BuildReadModbusCommand(string address, int length, byte station, bool isStartWithZero, byte defaultFunction)
	{
		try
		{
			ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
			CheckModbusAddressStart(mAddress, isStartWithZero);
			return BuildReadModbusCommand(mAddress, length, 120);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[][]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus读取数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码应该根据bool或是字来区分
	/// To construct the core message of Modbus reading data, you need to specify the address, length, station number,
	/// whether the starting address is 0, and the default function code should be distinguished according to bool or word
	/// </summary>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[][]> BuildReadModbusCommand(IModbus modbus, string address, int length, byte defaultFunction)
	{
		try
		{
			ModbusAddress mAddress = new ModbusAddress(address, modbus.Station, defaultFunction);
			CheckModbusAddressStart(mAddress, modbus.AddressStartWithZero);
			return BuildReadModbusCommand(mAddress, length, modbus.WordReadBatchLength);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[][]>(ex.Message);
		}
	}

	internal static OperateResult<byte[][]> BuildReadFileModbusCommand(ushort fileNumber, ushort address, ushort length, byte station, bool addressStartWithZero)
	{
		OperateResult<int[], int[]> operateResult = HslHelper.SplitReadLength(address, length, Authorization.asdniasnfaksndiqwhawfskhfaiw() ? 124 : int.MaxValue);
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < operateResult.Content1.Length; i++)
		{
			byte[] buffer = new byte[10];
			buffer[0] = station;
			buffer[1] = 20;
			buffer[2] = (byte)(buffer.Length - 3);
			buffer[3] = 6;
			buffer[4] = BitConverter.GetBytes(fileNumber)[1];
			buffer[5] = BitConverter.GetBytes(fileNumber)[0];
			buffer[6] = BitConverter.GetBytes(operateResult.Content1[i])[1];
			buffer[7] = BitConverter.GetBytes(operateResult.Content1[i])[0];
			buffer[8] = BitConverter.GetBytes(operateResult.Content2[i])[1];
			buffer[9] = BitConverter.GetBytes(operateResult.Content2[i])[0];
			list.Add(buffer);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	internal static OperateResult<byte[]> BuildWriteFileModbusCommand(ushort fileNumber, ushort address, byte[] data, byte station, bool addressStartWithZero)
	{
		if (data.Length + 7 > 255)
		{
			return new OperateResult<byte[]>("the data length must less than 248 bytes( 124 word)");
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(station);
		memoryStream.WriteByte(21);
		memoryStream.WriteByte((byte)(7 + data.Length));
		memoryStream.WriteByte(6);
		memoryStream.WriteByte(BitConverter.GetBytes(fileNumber)[1]);
		memoryStream.WriteByte(BitConverter.GetBytes(fileNumber)[0]);
		memoryStream.WriteByte(BitConverter.GetBytes(address)[1]);
		memoryStream.WriteByte(BitConverter.GetBytes(address)[0]);
		memoryStream.WriteByte(BitConverter.GetBytes(data.Length / 2)[1]);
		memoryStream.WriteByte(BitConverter.GetBytes(data.Length / 2)[0]);
		memoryStream.Write(data);
		return OperateResult.CreateSuccessResult(memoryStream.ToArray());
	}

	/// <summary>
	/// 构建一个同时读写的命令报文，指定读取地址，读取长度，写入地址，写入的字节数据信息，然后整个命令返回读取成功的字节数据信息
	/// </summary>
	/// <param name="readAddress">读取的地址信息</param>
	/// <param name="length">读取的地址长度</param>
	/// <param name="writeAddress">写入的地址信息</param>
	/// <param name="value">写入的字节数据</param>
	/// <param name="station">站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildReadWriteModbusCommand(string readAddress, ushort length, string writeAddress, byte[] value, byte station, bool isStartWithZero, byte defaultFunction)
	{
		try
		{
			ModbusAddress modbusAddress = new ModbusAddress(readAddress, station, defaultFunction);
			CheckModbusAddressStart(modbusAddress, isStartWithZero);
			ModbusAddress modbusAddress2 = new ModbusAddress(writeAddress, station, defaultFunction);
			CheckModbusAddressStart(modbusAddress2, isStartWithZero);
			byte[] buffer = new byte[11 + value.Length];
			buffer[0] = (byte)modbusAddress.Station;
			buffer[1] = (byte)modbusAddress.Function;
			buffer[2] = BitConverter.GetBytes(modbusAddress.AddressStart)[1];
			buffer[3] = BitConverter.GetBytes(modbusAddress.AddressStart)[0];
			buffer[4] = BitConverter.GetBytes(length)[1];
			buffer[5] = BitConverter.GetBytes(length)[0];
			buffer[6] = BitConverter.GetBytes(modbusAddress2.AddressStart)[1];
			buffer[7] = BitConverter.GetBytes(modbusAddress2.AddressStart)[0];
			buffer[8] = (byte)(value.Length / 2 / 256);
			buffer[9] = (byte)(value.Length / 2 % 256);
			buffer[10] = (byte)value.Length;
			value.CopyTo(buffer, 11);
			return OperateResult.CreateSuccessResult(buffer);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus读取数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码应该根据bool或是字来区分
	/// To construct the core message of Modbus reading data, you need to specify the address, length, station number,
	/// whether the starting address is 0, and the default function code should be distinguished according to bool or word
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="cuttingBatchLength">切割长度信息，针对字读取的情况下，切割多次读取的设置值信息</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[][]> BuildReadModbusCommand(ModbusAddress mAddress, int length, int cuttingBatchLength)
	{
		int num = mAddress.Function;
		if (mAddress.Function == 16)
		{
			num = 3;
		}
		else if (mAddress.Function == 15)
		{
			num = 1;
		}
		else if (mAddress.Function == 6)
		{
			num = 3;
		}
		else if (mAddress.Function == 5)
		{
			num = 1;
		}
		List<byte[]> list = new List<byte[]>();
		if (num == 1 || num == 2 || num == 3 || num == 4 || Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			OperateResult<int[], int[]> operateResult = HslHelper.SplitReadLength(mAddress.AddressStart, length, (num == 1 || num == 2) ? 2000 : cuttingBatchLength);
			for (int i = 0; i < operateResult.Content1.Length; i++)
			{
				list.Add(new byte[6]
				{
					(byte)mAddress.Station,
					(byte)num,
					BitConverter.GetBytes(operateResult.Content1[i])[1],
					BitConverter.GetBytes(operateResult.Content1[i])[0],
					BitConverter.GetBytes(operateResult.Content2[i])[1],
					BitConverter.GetBytes(operateResult.Content2[i])[0]
				});
			}
		}
		else
		{
			list.Add(new byte[6]
			{
				(byte)mAddress.Station,
				(byte)num,
				BitConverter.GetBytes(mAddress.AddressStart)[1],
				BitConverter.GetBytes(mAddress.AddressStart)[0],
				BitConverter.GetBytes(length)[1],
				BitConverter.GetBytes(length)[0]
			});
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 构建Modbus写入bool数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message that Modbus writes to bool data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="values">bool数组的信息</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteBoolModbusCommand(string address, bool[] values, byte station, bool isStartWithZero, byte defaultFunction)
	{
		try
		{
			ModbusAddress modbusAddress = new ModbusAddress(address, station, defaultFunction);
			CheckModbusAddressStart(modbusAddress, isStartWithZero);
			if (modbusAddress.Function == 1)
			{
				modbusAddress.Function = defaultFunction;
			}
			return BuildWriteBoolModbusCommand(modbusAddress, values);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus写入bool数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message that Modbus writes to bool data, you need to specify the address, length, station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="dataFormat">数据规则变化</param>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="value">bool的信息</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteBoolModbusCommand(DataFormat dataFormat, string address, bool value, byte station, bool isStartWithZero, byte defaultFunction)
	{
		try
		{
			if (address.IndexOf('.') <= 0)
			{
				ModbusAddress modbusAddress = new ModbusAddress(address, station, defaultFunction);
				CheckModbusAddressStart(modbusAddress, isStartWithZero);
				if (modbusAddress.Function == 1)
				{
					modbusAddress.Function = defaultFunction;
				}
				return BuildWriteBoolModbusCommand(modbusAddress, value);
			}
			if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
			{
				return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
			}
			int num = Convert.ToInt32(address.Substring(address.IndexOf('.') + 1));
			if (num < 0 || num > 15)
			{
				return new OperateResult<byte[]>(StringResources.Language.ModbusBitIndexOverstep);
			}
			if (dataFormat == DataFormat.BADC || dataFormat == DataFormat.DCBA)
			{
				num = ((num < 8) ? (num + 8) : (num - 8));
			}
			int num2 = 1 << num;
			int num3 = ~num2;
			if (!value)
			{
				num2 = 0;
			}
			return BuildWriteMaskModbusCommand(address.Substring(0, address.IndexOf('.')), (ushort)num3, (ushort)num2, station, isStartWithZero, 22);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus写入bool数组的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message that Modbus writes to the bool array, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="values">bool数组的信息</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteBoolModbusCommand(ModbusAddress mAddress, bool[] values)
	{
		try
		{
			byte[] array = SoftBasic.BoolArrayToByte(values);
			byte[] buffer = new byte[7 + array.Length];
			buffer[0] = (byte)mAddress.Station;
			if (mAddress.WriteFunction < 0)
			{
				buffer[1] = (byte)mAddress.Function;
			}
			else
			{
				buffer[1] = (byte)mAddress.WriteFunction;
			}
			buffer[2] = BitConverter.GetBytes(mAddress.AddressStart)[1];
			buffer[3] = BitConverter.GetBytes(mAddress.AddressStart)[0];
			buffer[4] = (byte)(values.Length / 256);
			buffer[5] = (byte)(values.Length % 256);
			buffer[6] = (byte)array.Length;
			array.CopyTo(buffer, 7);
			return OperateResult.CreateSuccessResult(buffer);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus写入bool数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message that Modbus writes to bool data, you need to specify the address, length, station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="value">bool数据的信息</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteBoolModbusCommand(ModbusAddress mAddress, bool value)
	{
		byte[] buffer = new byte[6]
		{
			(byte)mAddress.Station,
			0,
			0,
			0,
			0,
			0
		};
		if (mAddress.WriteFunction < 0)
		{
			buffer[1] = (byte)mAddress.Function;
		}
		else
		{
			buffer[1] = (byte)mAddress.WriteFunction;
		}
		buffer[2] = BitConverter.GetBytes(mAddress.AddressStart)[1];
		buffer[3] = BitConverter.GetBytes(mAddress.AddressStart)[0];
		if (value)
		{
			buffer[4] = byte.MaxValue;
			buffer[5] = 0;
		}
		else
		{
			buffer[4] = 0;
			buffer[5] = 0;
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing word data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="values">bool数组的信息</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteWordModbusCommand(string address, byte[] values, byte station, bool isStartWithZero, byte defaultFunction)
	{
		try
		{
			ModbusAddress modbusAddress = new ModbusAddress(address, station, defaultFunction);
			if (modbusAddress.Function == 3)
			{
				modbusAddress.Function = defaultFunction;
			}
			CheckModbusAddressStart(modbusAddress, isStartWithZero);
			if (modbusAddress.WriteFunction == 6 || modbusAddress.Function == 6)
			{
				if (values.Length != 2)
				{
					return new OperateResult<byte[]>("values length must be 2 when use WriteOneRegister function");
				}
				return BuildWriteOneRegisterModbusCommand(modbusAddress, values);
			}
			return BuildWriteWordModbusCommand(modbusAddress, values);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing word data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="value">short数据信息</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <param name="byteTransform">数据的变换规则</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteWordModbusCommand(string address, short value, byte station, bool isStartWithZero, byte defaultFunction, IByteTransform byteTransform)
	{
		try
		{
			ModbusAddress modbusAddress = new ModbusAddress(address, station, defaultFunction);
			if (modbusAddress.Function == 3)
			{
				modbusAddress.Function = defaultFunction;
			}
			if (modbusAddress.WriteFunction == 16 || modbusAddress.Function == 16)
			{
				CheckModbusAddressStart(modbusAddress, isStartWithZero);
				byte[] values = byteTransform.TransByte(value);
				return BuildWriteWordModbusCommand(modbusAddress, values);
			}
			CheckModbusAddressStart(modbusAddress, isStartWithZero);
			return BuildWriteOneRegisterModbusCommand(modbusAddress, value, byteTransform);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing word data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="value">bool数组的信息</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <param name="byteTransform">数据的变换规则</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteWordModbusCommand(string address, ushort value, byte station, bool isStartWithZero, byte defaultFunction, IByteTransform byteTransform)
	{
		try
		{
			ModbusAddress modbusAddress = new ModbusAddress(address, station, defaultFunction);
			if (modbusAddress.Function == 3)
			{
				modbusAddress.Function = defaultFunction;
			}
			if (modbusAddress.WriteFunction == 16 || modbusAddress.Function == 16)
			{
				CheckModbusAddressStart(modbusAddress, isStartWithZero);
				byte[] values = byteTransform.TransByte(value);
				return BuildWriteWordModbusCommand(modbusAddress, values);
			}
			CheckModbusAddressStart(modbusAddress, isStartWithZero);
			return BuildWriteOneRegisterModbusCommand(modbusAddress, value, byteTransform);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus写入掩码的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the Modbus write mask core message, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="address">Modbus的富文本地址</param>
	/// <param name="andMask">进行与操作的掩码信息</param>
	/// <param name="orMask">进行或操作的掩码信息</param>
	/// <param name="station">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteMaskModbusCommand(string address, ushort andMask, ushort orMask, byte station, bool isStartWithZero, byte defaultFunction)
	{
		try
		{
			ModbusAddress modbusAddress = new ModbusAddress(address, station, defaultFunction);
			if (modbusAddress.Function == 3)
			{
				modbusAddress.Function = defaultFunction;
			}
			CheckModbusAddressStart(modbusAddress, isStartWithZero);
			return BuildWriteMaskModbusCommand(modbusAddress, andMask, orMask);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing word data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="values">bool数组的信息</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteWordModbusCommand(ModbusAddress mAddress, byte[] values)
	{
		byte[] buffer = new byte[7 + values.Length];
		buffer[0] = (byte)mAddress.Station;
		if (mAddress.WriteFunction < 0)
		{
			buffer[1] = (byte)mAddress.Function;
		}
		else
		{
			buffer[1] = (byte)mAddress.WriteFunction;
		}
		buffer[2] = BitConverter.GetBytes(mAddress.AddressStart)[1];
		buffer[3] = BitConverter.GetBytes(mAddress.AddressStart)[0];
		buffer[4] = (byte)(values.Length / 2 / 256);
		buffer[5] = (byte)(values.Length / 2 % 256);
		buffer[6] = (byte)values.Length;
		values.CopyTo(buffer, 7);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建Modbus写入掩码数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing mask data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="andMask">等待进行与操作的掩码</param>
	/// <param name="orMask">等待进行或操作的掩码</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteMaskModbusCommand(ModbusAddress mAddress, ushort andMask, ushort orMask)
	{
		return OperateResult.CreateSuccessResult(new byte[8]
		{
			(byte)mAddress.Station,
			(byte)mAddress.Function,
			BitConverter.GetBytes(mAddress.AddressStart)[1],
			BitConverter.GetBytes(mAddress.AddressStart)[0],
			BitConverter.GetBytes(andMask)[1],
			BitConverter.GetBytes(andMask)[0],
			BitConverter.GetBytes(orMask)[1],
			BitConverter.GetBytes(orMask)[0]
		});
	}

	/// <summary>
	/// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing word data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="value">short的值</param>
	/// <param name="byteTransform">数据的变换规则</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteOneRegisterModbusCommand(ModbusAddress mAddress, short value, IByteTransform byteTransform)
	{
		return BuildWriteOneRegisterModbusCommand(mAddress, byteTransform.TransByte(value));
	}

	/// <summary>
	/// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing word data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="value">ushort的值</param>
	/// <param name="byteTransform">数据的变换规则</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteOneRegisterModbusCommand(ModbusAddress mAddress, ushort value, IByteTransform byteTransform)
	{
		return BuildWriteOneRegisterModbusCommand(mAddress, byteTransform.TransByte(value));
	}

	/// <summary>
	/// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码
	/// To construct the core message of Modbus writing word data, you need to specify the address, length,
	/// station number, whether the starting address is 0, and the default function code
	/// </summary>
	/// <param name="mAddress">Modbus的富文本地址</param>
	/// <param name="value">short的值</param>
	/// <param name="byteTransform">数据的变换规则</param>
	/// <returns>包含最终命令的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteOneRegisterModbusCommand(ModbusAddress mAddress, byte[] value)
	{
		byte[] buffer = new byte[6]
		{
			(byte)mAddress.Station,
			0,
			0,
			0,
			0,
			0
		};
		if (mAddress.WriteFunction < 0)
		{
			buffer[1] = (byte)mAddress.Function;
		}
		else
		{
			buffer[1] = (byte)mAddress.WriteFunction;
		}
		buffer[2] = BitConverter.GetBytes(mAddress.AddressStart)[1];
		buffer[3] = BitConverter.GetBytes(mAddress.AddressStart)[0];
		buffer[4] = value[0];
		buffer[5] = value[1];
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 从返回的modbus的书内容中，提取出真实的数据，适用于写入和读取操作
	/// Extract real data from the content of the returned modbus book, suitable for writing and reading operations
	/// </summary>
	/// <param name="response">返回的核心modbus报文信息</param>
	/// <returns>结果数据内容</returns>
	public static OperateResult<byte[]> ExtractActualData(byte[] response)
	{
		try
		{
			if (response[1] >= 128)
			{
				return new OperateResult<byte[]>(response[2], GetDescriptionByErrorCode(response[2]));
			}
			if (response.Length > 3)
			{
				return OperateResult.CreateSuccessResult(SoftBasic.ArrayRemoveBegin(response, 3));
			}
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	/// <summary>
	/// 将modbus指令打包成Modbus-Tcp指令，需要指定ID信息来添加6个字节的报文头
	/// Pack the Modbus command into Modbus-Tcp command, you need to specify the ID information to add a 6-byte message header
	/// </summary>
	/// <param name="modbus">Modbus核心指令</param>
	/// <param name="id">消息的序号</param>
	/// <returns>Modbus-Tcp指令</returns>
	public static byte[] PackCommandToTcp(byte[] modbus, ushort id)
	{
		byte[] buffer = new byte[modbus.Length + 6];
		buffer[0] = BitConverter.GetBytes(id)[1];
		buffer[1] = BitConverter.GetBytes(id)[0];
		buffer[4] = BitConverter.GetBytes(modbus.Length)[1];
		buffer[5] = BitConverter.GetBytes(modbus.Length)[0];
		modbus.CopyTo(buffer, 6);
		return buffer;
	}

	/// <summary>
	/// 将modbus-tcp的报文数据重新还原成modbus指令，移除6个字节的报文头数据
	/// Re-modify the message data of modbus-tcp into the modbus command, remove the 6-byte message header data
	/// </summary>
	/// <param name="modbusTcp">modbus-tcp的报文</param>
	/// <returns>modbus数据报文</returns>
	public static byte[] ExplodeTcpCommandToCore(byte[] modbusTcp)
	{
		return modbusTcp.RemoveBegin(6);
	}

	/// <summary>
	/// 将modbus-rtu的数据重新还原成modbus数据，移除CRC校验的内容
	/// Restore the data of modbus-rtu to modbus data again, remove the content of CRC check
	/// </summary>
	/// <param name="modbusRtu">modbus-rtu的报文</param>
	/// <returns>modbus数据报文</returns>
	public static byte[] ExplodeRtuCommandToCore(byte[] modbusRtu)
	{
		return modbusRtu.RemoveLast(2);
	}

	/// <summary>
	/// 将modbus指令打包成Modbus-Rtu指令，在报文的末尾添加CRC16的校验码
	/// Pack the modbus instruction into Modbus-Rtu instruction, add CRC16 check code at the end of the message
	/// </summary>
	/// <param name="modbus">Modbus指令</param>
	/// <returns>Modbus-Rtu指令</returns>
	public static byte[] PackCommandToRtu(byte[] modbus)
	{
		return SoftCRC16.CRC16(modbus);
	}

	/// <summary>
	/// 将一个modbus核心的数据报文，转换成modbus-ascii的数据报文，增加LRC校验，增加首尾标记数据
	/// Convert a Modbus core data message into a Modbus-ascii data message, add LRC check, and add head and tail tag data
	/// </summary>
	/// <param name="modbus">modbus-rtu的完整报文，携带相关的校验码</param>
	/// <returns>可以用于直接发送的modbus-ascii的报文</returns>
	public static byte[] TransModbusCoreToAsciiPackCommand(byte[] modbus)
	{
		byte[] inBytes = SoftLRC.LRC(modbus);
		byte[] array = SoftBasic.BytesToAsciiBytes(inBytes);
		return SoftBasic.SpliceArray<byte>(new byte[1] { 58 }, array, new byte[2] { 13, 10 });
	}

	/// <summary>
	/// 将一个modbus-ascii的数据报文，转换成的modbus核心数据报文，移除首尾标记，移除LRC校验
	/// Convert a Modbus-ascii data message into a Modbus core data message, remove the first and last tags, and remove the LRC check
	/// </summary>
	/// <param name="modbusAscii">modbus-ascii的完整报文，携带相关的校验码</param>
	/// <returns>可以用于直接发送的modbus的报文</returns>
	public static OperateResult<byte[]> TransAsciiPackCommandToCore(byte[] modbusAscii)
	{
		try
		{
			if (modbusAscii[0] != 58 || modbusAscii[^2] != 13 || modbusAscii[^1] != 10)
			{
				return new OperateResult<byte[]>
				{
					Message = StringResources.Language.ModbusAsciiFormatCheckFailed + modbusAscii.ToHexString(' ')
				};
			}
			int num = -1;
			for (int i = 1; i < modbusAscii.Length - 1; i++)
			{
				if (modbusAscii[i] == 13 && modbusAscii[i + 1] == 10)
				{
					num = i;
					break;
				}
			}
			if (num > 1 && num != modbusAscii.Length - 2)
			{
				modbusAscii = modbusAscii.SelectBegin(num + 2);
			}
			byte[] array = SoftBasic.AsciiBytesToBytes(modbusAscii.RemoveDouble(1, 2));
			if (!SoftLRC.CheckLRC(array))
			{
				return new OperateResult<byte[]>
				{
					Message = StringResources.Language.ModbusLRCCheckFailed + array.ToHexString(' ')
				};
			}
			return OperateResult.CreateSuccessResult(array.RemoveLast(1));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>
			{
				Message = ex.Message + modbusAscii.ToHexString(' ')
			};
		}
	}

	/// <summary>
	/// 分析Modbus协议的地址信息，该地址适应于tcp及rtu模式
	/// Analysis of the address information of Modbus protocol, the address is adapted to tcp and rtu mode
	/// </summary>
	/// <param name="address">带格式的地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="defaultStation">默认的站号信息</param>
	/// <param name="isStartWithZero">起始地址是否从0开始</param>
	/// <param name="defaultFunction">默认的功能码信息</param>
	/// <returns>转换后的地址信息</returns>
	public static OperateResult<ModbusAddress> AnalysisAddress(string address, byte defaultStation, bool isStartWithZero, byte defaultFunction)
	{
		try
		{
			ModbusAddress modbusAddress = new ModbusAddress(address, defaultStation, defaultFunction);
			if (!isStartWithZero)
			{
				if (modbusAddress.AddressStart < 1)
				{
					throw new Exception(StringResources.Language.ModbusAddressMustMoreThanOne);
				}
				modbusAddress.AddressStart -= 1;
			}
			return OperateResult.CreateSuccessResult(modbusAddress);
		}
		catch (Exception ex)
		{
			return new OperateResult<ModbusAddress>
			{
				Message = ex.Message
			};
		}
	}

	/// <summary>
	/// 通过错误码来获取到对应的文本消息
	/// Get the corresponding text message through the error code
	/// </summary>
	/// <param name="code">错误码</param>
	/// <returns>错误的文本描述</returns>
	public static string GetDescriptionByErrorCode(byte code)
	{
		return code switch
		{
			1 => StringResources.Language.ModbusTcpFunctionCodeNotSupport, 
			2 => StringResources.Language.ModbusTcpFunctionCodeOverBound, 
			3 => StringResources.Language.ModbusTcpFunctionCodeQuantityOver, 
			4 => StringResources.Language.ModbusTcpFunctionCodeReadWriteException, 
			_ => StringResources.Language.UnknownError, 
		};
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	private static bool CheckRtuReceiveDataComplete(byte[] response, int startIndex = 0, bool checkCrc = true)
	{
		if (response.Length > 3 + startIndex)
		{
			if (response[startIndex + 1] == 6 || response[startIndex + 1] == 16 || response[startIndex + 1] == 15 || response[startIndex + 1] == 5)
			{
				if (response.Length >= 8 + startIndex)
				{
					if (checkCrc)
					{
						return SoftCRC16.CheckCRC16(response.SelectMiddle(startIndex, 8));
					}
					return true;
				}
			}
			else if (response[startIndex + 1] == 1 || response[startIndex + 1] == 2 || response[startIndex + 1] == 3 || response[startIndex + 1] == 4 || response[startIndex + 1] == 23 || response[startIndex + 1] == 20 || response[startIndex + 1] == 21)
			{
				if (response.Length >= response[2 + startIndex] + 3 + 2 + startIndex)
				{
					if (checkCrc)
					{
						return SoftCRC16.CheckCRC16(response.SelectMiddle(startIndex, response[2 + startIndex] + 3 + 2));
					}
					return true;
				}
			}
			else if (response[startIndex + 1] == 22)
			{
				if (response.Length >= 10 + startIndex)
				{
					if (checkCrc)
					{
						return SoftCRC16.CheckCRC16(response.SelectMiddle(startIndex, 10));
					}
					return true;
				}
			}
			else
			{
				if (response[startIndex + 1] <= 128)
				{
					if (checkCrc)
					{
						return SoftCRC16.CheckCRC16(response.RemoveBegin(startIndex));
					}
					return true;
				}
				if (response.Length >= 5 + startIndex)
				{
					if (checkCrc)
					{
						return SoftCRC16.CheckCRC16(response.SelectMiddle(startIndex, 5));
					}
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	public static bool CheckRtuReceiveDataComplete(byte[] send, byte[] response)
	{
		if (send == null)
		{
			return CheckRtuReceiveDataComplete(response);
		}
		if (send.Length < 5)
		{
			return CheckRtuReceiveDataComplete(response);
		}
		if (response == null)
		{
			return false;
		}
		int num = Math.Min(response.Length - 3, 3);
		for (int i = 0; i <= num; i++)
		{
			if (CheckModbusStationFunction(send, response, i) && CheckRtuReceiveDataComplete(response, i))
			{
				return true;
			}
		}
		for (int j = 0; j < num; j++)
		{
			if (CheckModbusStationFunction(send, response, j) && CheckRtuReceiveDataComplete(response, j, checkCrc: false))
			{
				return true;
			}
		}
		return false;
	}

	private static bool CheckModbusStationFunction(byte[] send, byte[] receive, int index)
	{
		if (send[0] != receive[index])
		{
			return false;
		}
		if (index + 1 >= receive.Length)
		{
			return false;
		}
		return send[1] == receive[index + 1] || receive[index + 1] == send[1] + 128;
	}

	/// <summary>
	/// 检查当前发送和接收的modbusrtu报文是否站号匹配，匹配返回 1，此处进行匹配的时候，还有一定的容错范围
	/// </summary>
	/// <param name="send">发送的数据</param>
	/// <param name="receive">接收的数据</param>
	/// <returns>modbus的报文是否站号匹配</returns>
	public static int CheckRtuMessageMatch(byte[] send, byte[] receive)
	{
		if (send == null)
		{
			return 1;
		}
		if (send.Length < 5)
		{
			return 1;
		}
		if (receive == null)
		{
			return 1;
		}
		for (int i = 0; i < receive.Length - 3; i++)
		{
			if (send[0] == receive[i] && CheckRtuReceiveDataComplete(receive, i, checkCrc: false))
			{
				return 1;
			}
		}
		return -1;
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	public static bool CheckServerRtuReceiveDataComplete(byte[] receive)
	{
		if (receive.Length > 2)
		{
			if (receive[1] == 16 || receive[1] == 15)
			{
				return receive.Length > 8 && receive.Length >= receive[6] + 7 + 2;
			}
			if (receive[1] == 1 || receive[1] == 2 || receive[1] == 3 || receive[1] == 4 || receive[1] == 6 || receive[1] == 5)
			{
				return receive.Length >= 8;
			}
			if (receive[1] == 22)
			{
				return receive.Length >= 10;
			}
			if (receive[1] == 23)
			{
				return receive.Length >= 11 + receive[10] + 2;
			}
			if (receive[1] == 20 || receive[1] == 21)
			{
				return receive.Length >= 3 + receive[2] + 2;
			}
		}
		return false;
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	public static bool CheckAsciiReceiveDataComplete(byte[] modbusAscii)
	{
		return CheckAsciiReceiveDataComplete(modbusAscii, modbusAscii.Length);
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	public static bool CheckAsciiReceiveDataComplete(byte[] modbusAscii, int length)
	{
		if (length > 5)
		{
			return modbusAscii[0] == 58 && modbusAscii[length - 2] == 13 && modbusAscii[length - 1] == 10;
		}
		return false;
	}
}
