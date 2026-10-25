// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading;

namespace HslCommunication.Core;

public sealed class SoftMultiTask<T>
{
	public delegate void MultiInfo(T item, Exception ex);

	public delegate void MultiInfoTwo(int finish, int count, int success, int failed);

	private int m_opCount = 0;

	private int m_opThreadCount = 1;

	private int m_threadCount = 10;

	private int m_runStatus = 0;

	private T[] m_dataList = null;

	private Func<T, bool> m_operater = null;

	private int m_finishCount = 0;

	private int m_successCount = 0;

	private int m_failedCount = 0;

	private SimpleHybirdLock HybirdLock = new SimpleHybirdLock();

	private bool m_isRunningStop = false;

	private bool m_isQuit = false;

	private bool m_isQuitAfterException = false;

	public bool IsQuitAfterException
	{
		get
		{
			return m_isQuitAfterException;
		}
		set
		{
			m_isQuitAfterException = value;
		}
	}

	public event MultiInfo OnExceptionOccur;

	public event MultiInfoTwo OnReportProgress;

	public SoftMultiTask(T[] dataList, Func<T, bool> operater, int threadCount = 10)
	{
		m_dataList = dataList ?? throw new ArgumentNullException("dataList");
		m_operater = operater ?? throw new ArgumentNullException("operater");
		if (threadCount < 1)
		{
			throw new ArgumentException("threadCount can not less than 1", "threadCount");
		}
		m_threadCount = threadCount;
		Interlocked.Add(ref m_opCount, dataList.Length);
		Interlocked.Add(ref m_opThreadCount, threadCount);
	}

	public void StartOperater()
	{
		if (Interlocked.CompareExchange(ref m_runStatus, 0, 1) == 0)
		{
			for (int i = 0; i < m_threadCount; i++)
			{
				Thread thread = new Thread(ThreadBackground);
				thread.IsBackground = true;
				thread.Start();
			}
			JustEnded();
		}
	}

	public void StopOperater()
	{
		if (m_runStatus == 1)
		{
			m_isRunningStop = true;
		}
	}

	public void ResumeOperater()
	{
		m_isRunningStop = false;
	}

	public void EndedOperater()
	{
		if (m_runStatus == 1)
		{
			m_isQuit = true;
		}
	}

	private void ThreadBackground()
	{
		while (true)
		{
			bool flag = true;
			while (m_isRunningStop)
			{
			}
			int num = Interlocked.Decrement(ref m_opCount);
			if (num < 0)
			{
				break;
			}
			T val = m_dataList[num];
			bool success = false;
			bool success2 = false;
			try
			{
				if (!m_isQuit)
				{
					success = m_operater(val);
				}
			}
			catch (Exception ex)
			{
				success2 = true;
				OnExceptionOccur?.Invoke(val, ex);
				if (m_isQuitAfterException)
				{
					EndedOperater();
				}
			}
			finally
			{
				HybirdLock.Enter();
				if (success)
				{
					m_successCount++;
				}
				if (success2)
				{
					m_failedCount++;
				}
				m_finishCount++;
				OnReportProgress?.Invoke(m_finishCount, m_dataList.Length, m_successCount, m_failedCount);
				HybirdLock.Leave();
			}
		}
		JustEnded();
	}

	private void JustEnded()
	{
		if (Interlocked.Decrement(ref m_opThreadCount) == 0)
		{
			m_finishCount = 0;
			m_failedCount = 0;
			m_successCount = 0;
			Interlocked.Exchange(ref m_opCount, m_dataList.Length);
			Interlocked.Exchange(ref m_opThreadCount, m_threadCount + 1);
			Interlocked.Exchange(ref m_runStatus, 0);
			m_isRunningStop = false;
			m_isQuit = false;
		}
	}
}
