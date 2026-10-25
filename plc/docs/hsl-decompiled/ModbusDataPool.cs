// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Linq;
using HslCommunication;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.ModBus;

internal class ModbusDataPool : IDisposable
{
	private byte station = 1;

	private SoftBuffer coilBuffer;

	private SoftBuffer inputBuffer;

	private SoftBuffer registerBuffer;

	private SoftBuffer inputRegisterBuffer;

	private const int DataPoolLength = 65536;

	public ModbusDataPool(byte station)
	{
		coilBuffer = new SoftBuffer(65536);
		inputBuffer = new SoftBuffer(65536);
		registerBuffer = new SoftBuffer(131072);
		inputRegisterBuffer = new SoftBuffer(131072);
		registerBuffer.IsBoolReverseByWord = true;
		inputRegisterBuffer.IsBoolReverseByWord = true;
		this.station = station;
	}

	public OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, station, isStartWithZero: true, 3);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content.Function == 3)
		{
			return OperateResult.CreateSuccessResult(registerBuffer.GetBytes(operateResult.Content.AddressStart * 2, length * 2));
		}
		if (operateResult.Content.Function == 4)
		{
			return OperateResult.CreateSuccessResult(inputRegisterBuffer.GetBytes(operateResult.Content.AddressStart * 2, length * 2));
		}
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
	}

	public OperateResult Write(string address, byte[] value)
	{
		OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, station, isStartWithZero: true, 3);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content.Function == 3 || operateResult.Content.Function == 6 || operateResult.Content.Function == 16)
		{
			registerBuffer.SetBytes(value, operateResult.Content.AddressStart * 2);
			return OperateResult.CreateSuccessResult();
		}
		if (operateResult.Content.Function == 4)
		{
			inputRegisterBuffer.SetBytes(value, operateResult.Content.AddressStart * 2);
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
	}

	public OperateResult<bool[]> ReadBool(string address, ushort length, DataFormat dataFormat)
	{
		if (address.IndexOf('.') < 0)
		{
			OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, station, isStartWithZero: true, 1);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult);
			}
			if (operateResult.Content.Function == 1 || operateResult.Content.Function == 5 || operateResult.Content.Function == 15)
			{
				return OperateResult.CreateSuccessResult((from m in coilBuffer.GetBytes(operateResult.Content.AddressStart, length)
					select m != 0).ToArray());
			}
			if (operateResult.Content.Function == 2)
			{
				return OperateResult.CreateSuccessResult((from m in inputBuffer.GetBytes(operateResult.Content.AddressStart, length)
					select m != 0).ToArray());
			}
			return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
		}
		try
		{
			int num = Convert.ToInt32(address.Substring(address.IndexOf('.') + 1));
			address = address.Substring(0, address.IndexOf('.'));
			OperateResult<ModbusAddress> operateResult2 = ModbusInfo.AnalysisAddress(address, station, isStartWithZero: true, 3);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult2);
			}
			num = operateResult2.Content.AddressStart * 16 + num;
			if (operateResult2.Content.Function == 3)
			{
				return OperateResult.CreateSuccessResult(registerBuffer.GetBool(num, length, dataFormat));
			}
			if (operateResult2.Content.Function == 4)
			{
				return OperateResult.CreateSuccessResult(inputRegisterBuffer.GetBool(num, length, dataFormat));
			}
			return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>(ex.Message);
		}
	}

	public OperateResult Write(string address, bool[] value)
	{
		if (address.IndexOf('.') < 0)
		{
			OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, station, isStartWithZero: true, 1);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			if (operateResult.Content.Function == 1 || operateResult.Content.Function == 15 || operateResult.Content.Function == 5)
			{
				coilBuffer.SetBytes(value.Select((bool m) => (byte)(m ? 1u : 0u)).ToArray(), operateResult.Content.AddressStart);
				return OperateResult.CreateSuccessResult();
			}
			if (operateResult.Content.Function == 2)
			{
				inputBuffer.SetBytes(value.Select((bool m) => (byte)(m ? 1u : 0u)).ToArray(), operateResult.Content.AddressStart);
				return OperateResult.CreateSuccessResult();
			}
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
		try
		{
			int num = Convert.ToInt32(address.Substring(address.IndexOf('.') + 1));
			address = address.Substring(0, address.IndexOf('.'));
			OperateResult<ModbusAddress> operateResult2 = ModbusInfo.AnalysisAddress(address, station, isStartWithZero: true, 3);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			num = operateResult2.Content.AddressStart * 16 + num;
			if (operateResult2.Content.Function == 3)
			{
				registerBuffer.SetBool(value, num);
				return OperateResult.CreateSuccessResult();
			}
			if (operateResult2.Content.Function == 4)
			{
				inputRegisterBuffer.SetBool(value, num);
				return OperateResult.CreateSuccessResult();
			}
			return new OperateResult(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult(ex.Message);
		}
	}

	public bool ReadCoil(string address)
	{
		ushort index = ushort.Parse(address);
		return coilBuffer.GetByte(index) != 0;
	}

	public bool[] ReadCoil(string address, ushort length)
	{
		ushort index = ushort.Parse(address);
		return (from m in coilBuffer.GetBytes(index, length)
			select m != 0).ToArray();
	}

	public void WriteCoil(string address, bool data)
	{
		ushort index = ushort.Parse(address);
		coilBuffer.SetValue((byte)(data ? 1u : 0u), index);
	}

	public void WriteCoil(string address, bool[] data)
	{
		if (data != null)
		{
			ushort destIndex = ushort.Parse(address);
			coilBuffer.SetBytes(data.Select((bool m) => (byte)(m ? 1u : 0u)).ToArray(), destIndex);
		}
	}

	public bool ReadDiscrete(string address)
	{
		ushort index = ushort.Parse(address);
		return inputBuffer.GetByte(index) != 0;
	}

	public bool[] ReadDiscrete(string address, ushort length)
	{
		ushort index = ushort.Parse(address);
		return (from m in inputBuffer.GetBytes(index, length)
			select m != 0).ToArray();
	}

	public void WriteDiscrete(string address, bool data)
	{
		ushort index = ushort.Parse(address);
		inputBuffer.SetValue((byte)(data ? 1u : 0u), index);
	}

	public void WriteDiscrete(string address, bool[] data)
	{
		if (data != null)
		{
			ushort destIndex = ushort.Parse(address);
			inputBuffer.SetBytes(data.Select((bool m) => (byte)(m ? 1u : 0u)).ToArray(), destIndex);
		}
	}

	public byte[] SaveToBytes()
	{
		return ByteTransformHelper.GetByteFromSoftBuffer(coilBuffer, inputBuffer, registerBuffer, inputRegisterBuffer).ToArray();
	}

	public void LoadFromBytes(byte[] content, int index)
	{
		ByteTransformHelper.SetByteToSoftBuffer(content, index, coilBuffer, inputBuffer, registerBuffer, inputRegisterBuffer);
	}

	public void Dispose()
	{
		coilBuffer?.Dispose();
		inputBuffer?.Dispose();
		registerBuffer?.Dispose();
		inputRegisterBuffer?.Dispose();
	}
}
