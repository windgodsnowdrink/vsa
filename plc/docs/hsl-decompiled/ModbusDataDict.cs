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

internal class ModbusDataDict : IDisposable
{
	private Dictionary<int, ModbusDataPool> dictModbusDataPool;

	private bool stationDataIsolation = false;

	public ModbusDataDict()
	{
		dictModbusDataPool = new Dictionary<int, ModbusDataPool>();
		dictModbusDataPool.Add(0, new ModbusDataPool(0));
	}

	public void Set(bool stationDataIsolation)
	{
		if (this.stationDataIsolation == stationDataIsolation)
		{
			return;
		}
		this.stationDataIsolation = stationDataIsolation;
		if (this.stationDataIsolation)
		{
			dictModbusDataPool = new Dictionary<int, ModbusDataPool>();
			for (int i = 0; i < 255; i++)
			{
				dictModbusDataPool.Add(i, new ModbusDataPool((byte)i));
			}
		}
		else
		{
			dictModbusDataPool = new Dictionary<int, ModbusDataPool>();
			dictModbusDataPool.Add(0, new ModbusDataPool(0));
		}
	}

	public ModbusDataPool GetModbusPool(byte station)
	{
		if (stationDataIsolation)
		{
			return dictModbusDataPool[station];
		}
		return dictModbusDataPool.FirstOrDefault().Value;
	}

	public void Dispose()
	{
		foreach (ModbusDataPool value in dictModbusDataPool.Values)
		{
			value.Dispose();
		}
		dictModbusDataPool.Clear();
	}
}
