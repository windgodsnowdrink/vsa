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
using System.Threading;
using HslCommunication.Core;

namespace HslCommunication.Algorithms.ConnectPool;

public class ConnectPool<TConnector> where TConnector : IConnector
{
	private Func<TConnector> CreateConnector = null;

	private int maxConnector = 10;

	private int usedConnector = 0;

	private int usedConnectorMax = 0;

	private int expireTime = 30;

	private bool canGetConnector = true;

	private Timer timerCheck = null;

	private object listLock;

	private List<TConnector> connectors = null;

	public int MaxConnector
	{
		get
		{
			return maxConnector;
		}
		set
		{
			maxConnector = value;
		}
	}

	public int ConectionExpireTime
	{
		get
		{
			return expireTime;
		}
		set
		{
			expireTime = value;
		}
	}

	public int UsedConnector => usedConnector;

	public int UseConnectorMax => usedConnectorMax;

	public ConnectPool(Func<TConnector> createConnector)
	{
		CreateConnector = createConnector;
		listLock = new object();
		connectors = new List<TConnector>();
		timerCheck = new Timer(TimerCheckBackground, null, 10000, 30000);
	}

	public TConnector GetAvailableConnector()
	{
		while (!canGetConnector)
		{
			HslHelper.ThreadSleep(20);
		}
		TConnector val = default(TConnector);
		lock (listLock)
		{
			TConnector val2;
			for (int i = 0; i < connectors.Count; i++)
			{
				val2 = connectors[i];
				if (!val2.IsConnectUsing)
				{
					val2 = connectors[i];
					val2.IsConnectUsing = true;
					val = connectors[i];
					break;
				}
			}
			if (val == null)
			{
				val = CreateConnector();
				val.IsConnectUsing = true;
				val.LastUseTime = DateTime.Now;
				val.Open();
				connectors.Add(val);
				usedConnector = connectors.Count;
				if (usedConnector > usedConnectorMax)
				{
					usedConnectorMax = usedConnector;
				}
				if (usedConnector == maxConnector)
				{
					canGetConnector = false;
				}
			}
			val.LastUseTime = DateTime.Now;
		}
		return val;
	}

	public void ReturnConnector(TConnector connector)
	{
		lock (listLock)
		{
			int num = connectors.IndexOf(connector);
			if (num != -1)
			{
				TConnector val = connectors[num];
				val.IsConnectUsing = false;
			}
		}
	}

	public void ResetAllConnector()
	{
		lock (listLock)
		{
			for (int num = connectors.Count - 1; num >= 0; num--)
			{
				connectors[num].Close();
				connectors.RemoveAt(num);
			}
		}
	}

	private void TimerCheckBackground(object obj)
	{
		lock (listLock)
		{
			for (int num = connectors.Count - 1; num >= 0; num--)
			{
				if ((DateTime.Now - connectors[num].LastUseTime).TotalSeconds > (double)expireTime && !connectors[num].IsConnectUsing)
				{
					connectors[num].Close();
					connectors.RemoveAt(num);
				}
			}
			usedConnector = connectors.Count;
			if (usedConnector < MaxConnector)
			{
				canGetConnector = true;
			}
		}
	}
}
