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
using System.Text;
using System.Threading;
using HslCommunication.Core;
using HslCommunication.Reflection;

namespace HslCommunication.LogNet;

/// <summary>
/// 日志存储类的基类，提供一些基础的服务
/// </summary>
/// <remarks>
/// 基于此类可以实现任意的规则的日志存储规则，欢迎大家补充实现，本组件实现了3个日志类
/// 单文件日志类
/// 根据文件大小的类
/// 根据时间进行存储的类
/// </remarks>
public abstract class LogNetBase : IDisposable
{
	/// <summary>
	/// 文件存储的锁
	/// </summary>
	protected SimpleHybirdLock m_fileSaveLock;

	private HslMessageDegree m_messageDegree = HslMessageDegree.DEBUG;

	private Queue<HslMessageItem> m_WaitForSave;

	private SimpleHybirdLock m_simpleHybirdLock;

	private int m_SaveStatus = 0;

	private List<string> filtrateKeyword;

	private object filtrateLock;

	private string lastLogSaveFileName = string.Empty;

	private int lastConsoleDegree = -1;

	private bool disposedValue = false;

	private long dropLogCount = 0L;

	private int maxCacheCount = 1000000;

	/// <summary>
	/// 日志存储模式，1:单文件，2:按大小存储，3:按时间存储
	/// Log storage mode, 1: single file, 2: storage by size, 3: storage by time
	/// </summary>
	public LogSaveMode LogSaveMode { get; protected set; }

	/// <summary>
	/// 获取或设置当前的日志记录统计信息，如果你需要统计最近30天的日志，就需要对其实例化，详细参照
	/// Get or set the current log record statistics. If you need to count the logs of the last 30 days,
	/// you need to instantiate it. For details, please refer to
	/// </summary>
	public LogStatistics LogNetStatistics { get; set; }

	/// <summary>
	/// 获取或设置当前的日志信息在存储的时候是否在控制台进行输出，默认不输出。
	/// Gets or sets whether the current log information is output on the console when it is stored. It is not output by default.
	/// </summary>
	public bool ConsoleOutput { get; set; }

	/// <summary>
	/// 获取或设置是否记录线程ID的数据信息，默认为 True，表示记录，否则，需要设置为 False
	/// Get or set whether to record the data information of the thread ID, the default is True, which means record, otherwise, it needs to be set to False
	/// </summary>
	public bool LogThreadID { get; set; } = true;

	/// <summary>
	/// 获取或设置在记录文件日志的时候，是否记录不可见字符STX(也即是ASCII字符集的0x02)。默认为 True, 也即是记录这个字符，在日志分析的时候，可以更加方便的分析。
	/// Gets or sets whether to record the invisible character STX (that is, 0x02 of the ASCII character set) when recording the file log.
	/// The default is True, that is, this character is recorded, which can be analyzed more conveniently during log analysis.
	/// </summary>
	public bool LogStxAsciiCode { get; set; } = true;

	/// <summary>
	/// 获取或设置当前日志时间的小时偏差信息，用来解决某些情况下时区设置异常导致记录时间一直不正确的问题，默认为 0，也就是不处理。可以为正数，也可以为负数。
	/// Obtain or set the hourly deviation information of the current log time to solve the problem that the recording time has been incorrect due to abnormal time zone setting in some cases,
	/// and the default is 0, that is, it is not processed. Can be positive or negative.
	/// </summary>
	public int HourDeviation { get; set; } = 0;

	/// <summary>
	/// 获取或设置日志文件的UTF8编码是否携带标记头信息，默认为 True
	/// Gets or sets whether the UTF8 encoding of the log file carries tag header information, which defaults to True
	/// </summary>
	public bool EncoderShouldEmitUTF8Identifier { get; set; } = true;

	/// <summary>
	/// 当日志设置了后， 是否启用控制台颜色输出，默认启用
	/// When the log sets , whether to enable console color output is enabled by default
	/// </summary>
	public bool ConsoleColorEnable { get; set; } = true;

	/// <summary>
	/// 获取或设置日志记录的最大缓存数量，默认为1000000条，当日志文件达到这个数量的时候，说明已经来不及存储日志到文件，直接丢弃日志
	/// Get or set the maximum cache count for log records. The default is 1,000,000. When the log file reaches this number, it indicates that the logs can no longer be stored to the file and will be discarded.
	/// </summary>
	public int MaxCacheCount
	{
		get
		{
			return maxCacheCount;
		}
		set
		{
			if (value <= 100)
			{
				throw new ArgumentException("MaxCacheCount must be greater than 100");
			}
			maxCacheCount = value;
		}
	}

	public event EventHandler<HslEventArgs> BeforeSaveToFile = null;

	/// <summary>
	/// 实例化一个日志对象
	/// Instantiate a log object
	/// </summary>
	public LogNetBase()
	{
		m_fileSaveLock = new SimpleHybirdLock();
		m_simpleHybirdLock = new SimpleHybirdLock();
		m_WaitForSave = new Queue<HslMessageItem>();
		filtrateKeyword = new List<string>();
		filtrateLock = new object();
	}

	private void OnBeforeSaveToFile(HslEventArgs args)
	{
		BeforeSaveToFile?.Invoke(this, args);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入一条调试日志
	/// Write a debug log
	/// </summary>
	/// <param name="text">日志内容</param>
	public void WriteDebug(string text)
	{
		WriteDebug(string.Empty, text);
	}

	[HslMqttApi(ApiTopic = "WriteDebugKeyWord")]
	/// <summary>
	/// 写入一条带关键字的调试日志
	/// Write a debug log with keywords
	/// </summary>
	/// <param name="keyWord">关键字</param>
	/// <param name="text">日志内容</param>
	public void WriteDebug(string keyWord, string text)
	{
		RecordMessage(HslMessageDegree.DEBUG, keyWord, text);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入一条普通日志
	/// Write an infomation log
	/// </summary>
	/// <param name="text">日志内容</param>
	public void WriteInfo(string text)
	{
		WriteInfo(string.Empty, text);
	}

	[HslMqttApi(ApiTopic = "WriteInfoKeyWord")]
	/// <summary>
	/// 写入一条带关键字的普通日志
	/// Write an information log with keywords
	/// </summary>
	/// <param name="keyWord">关键字</param>
	/// <param name="text">日志内容</param>
	public void WriteInfo(string keyWord, string text)
	{
		RecordMessage(HslMessageDegree.INFO, keyWord, text);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入一条警告日志
	/// Write an warn log
	/// </summary>
	/// <param name="text">日志内容</param>
	public void WriteWarn(string text)
	{
		WriteWarn(string.Empty, text);
	}

	[HslMqttApi(ApiTopic = "WriteWarnKeyWord")]
	/// <summary>
	/// 写入一条带关键字的警告日志
	/// Write an warn log  with keywords
	/// </summary>
	/// <param name="keyWord">关键字</param>
	/// <param name="text">日志内容</param>
	public void WriteWarn(string keyWord, string text)
	{
		RecordMessage(HslMessageDegree.WARN, keyWord, text);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入一条错误日志
	/// Write an error log
	/// </summary>
	/// <param name="text">日志内容</param>
	public void WriteError(string text)
	{
		WriteError(string.Empty, text);
	}

	[HslMqttApi(ApiTopic = "WriteErrorKeyWord")]
	/// <summary>
	/// 写入一条带关键字的错误日志
	/// Write an error log with keywords
	/// </summary>
	/// <param name="keyWord">关键字</param>
	/// <param name="text">日志内容</param>
	public void WriteError(string keyWord, string text)
	{
		RecordMessage(HslMessageDegree.ERROR, keyWord, text);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入一条致命日志
	/// Write an fatal log
	/// </summary>
	/// <param name="text">日志内容</param>
	public void WriteFatal(string text)
	{
		WriteFatal(string.Empty, text);
	}

	[HslMqttApi(ApiTopic = "WriteFatalKeyWord")]
	/// <summary>
	/// 写入一条带关键字的致命日志
	/// Write an fatal log with keywords
	/// </summary>
	/// <param name="keyWord">关键字</param>
	/// <param name="text">日志内容</param>
	public void WriteFatal(string keyWord, string text)
	{
		RecordMessage(HslMessageDegree.FATAL, keyWord, text);
	}

	/// <summary>
	/// 写入一条带关键字的异常信息
	/// Write an exception log with keywords
	/// </summary>
	/// <param name="keyWord">关键字</param>
	/// <param name="ex">异常</param>
	public void WriteException(string keyWord, Exception ex)
	{
		WriteException(keyWord, string.Empty, ex);
	}

	/// <summary>
	/// 写入一条带关键字和描述信息的异常信息
	/// Write an exception log with keywords and text
	/// </summary>
	/// <param name="keyWord">关键字</param>
	/// <param name="text">内容</param>
	/// <param name="ex">异常</param>
	public void WriteException(string keyWord, string text, Exception ex)
	{
		RecordMessage(HslMessageDegree.FATAL, keyWord, LogNetManagment.GetSaveStringFromException(text, ex));
	}

	/// <summary>
	/// 通过指定消息等级，关键字，日志信息进行消息记录
	/// Record messages by specifying message level, keywords, and log information
	/// </summary>
	/// <param name="degree">消息等级</param>
	/// <param name="keyWord">关键字</param>
	/// <param name="text">日志内容</param>
	public void RecordMessage(HslMessageDegree degree, string keyWord, string text)
	{
		WriteToFile(degree, keyWord, text);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入一条解释性的信息，不属于消息等级控制的范畴
	/// Write an explanatory message that is not part of message level control
	/// </summary>
	/// <param name="description">解释文本</param>
	public void WriteDescrition(string description)
	{
		if (string.IsNullOrEmpty(description))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder("\u0002");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("\u0002/");
		int num = 118 - CalculateStringOccupyLength(description);
		if (num >= 8)
		{
			int num2 = (num - 8) / 2;
			AppendCharToStringBuilder(stringBuilder, '*', num2);
			stringBuilder.Append("   ");
			stringBuilder.Append(description);
			stringBuilder.Append("   ");
			if (num % 2 == 0)
			{
				AppendCharToStringBuilder(stringBuilder, '*', num2);
			}
			else
			{
				AppendCharToStringBuilder(stringBuilder, '*', num2 + 1);
			}
		}
		else if (num >= 2)
		{
			int num3 = (num - 2) / 2;
			AppendCharToStringBuilder(stringBuilder, '*', num3);
			stringBuilder.Append(description);
			if (num % 2 == 0)
			{
				AppendCharToStringBuilder(stringBuilder, '*', num3);
			}
			else
			{
				AppendCharToStringBuilder(stringBuilder, '*', num3 + 1);
			}
		}
		else
		{
			stringBuilder.Append(description);
		}
		stringBuilder.Append("/");
		stringBuilder.Append(Environment.NewLine);
		RecordMessage(HslMessageDegree.None, string.Empty, stringBuilder.ToString());
	}

	[HslMqttApi]
	/// <summary>
	/// 写入任意字符串
	/// Write arbitrary string
	/// </summary>
	/// <param name="text">文本</param>
	public void WriteAnyString(string text)
	{
		RecordMessage(HslMessageDegree.None, string.Empty, text);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入一行换行符
	/// Write a newline
	/// </summary>
	public void WriteNewLine()
	{
		RecordMessage(HslMessageDegree.None, string.Empty, "\u0002" + Environment.NewLine);
	}

	/// <summary>
	/// 设置日志的存储等级，高于该等级的才会被存储
	/// Set the storage level of the logs. Only the logs above this level will be stored.
	/// </summary>
	/// <param name="degree">登记信息</param>
	public void SetMessageDegree(HslMessageDegree degree)
	{
		m_messageDegree = degree;
	}

	[HslMqttApi]
	/// <summary>
	/// 过滤掉指定的关键字的日志，该信息不存储，但仍然触发事件
	/// Filter out the logs of the specified keywords, the information is not stored, but the  event is still triggered
	/// </summary>
	/// <param name="keyword">关键字</param>
	public void FiltrateKeyword(string keyword)
	{
		lock (filtrateLock)
		{
			if (!filtrateKeyword.Contains(keyword))
			{
				filtrateKeyword.Add(keyword);
			}
		}
	}

	[HslMqttApi]
	/// <summary>
	/// 移除过滤的关键字存储
	/// Remove filtered keyword storage
	/// </summary>
	/// <param name="keyword">关键字</param>
	public void RemoveFiltrate(string keyword)
	{
		lock (filtrateLock)
		{
			if (filtrateKeyword.Contains(keyword))
			{
				filtrateKeyword.Remove(keyword);
			}
		}
	}

	/// <summary>
	/// 获取当前因为缓存过大导致被过滤掉的日志数量
	/// Get the number of logs currently filtered out due to excessive caching
	/// </summary>
	/// <returns>日志数量</returns>
	public long GetDropLogCount()
	{
		return dropLogCount;
	}

	private void WriteToFile(HslMessageDegree degree, string keyword, string text)
	{
		if (degree <= m_messageDegree)
		{
			HslMessageItem hslMessageItem = GetHslMessageItem(degree, keyword, text);
			AddItemToCache(hslMessageItem, start: true);
		}
	}

	private void AddItemToCache(HslMessageItem item, bool start)
	{
		m_simpleHybirdLock.Enter();
		if (m_WaitForSave.Count >= MaxCacheCount)
		{
			Interlocked.Increment(ref dropLogCount);
		}
		else
		{
			m_WaitForSave.Enqueue(item);
		}
		m_simpleHybirdLock.Leave();
		if (start)
		{
			StartSaveFile();
		}
	}

	private void StartSaveFile()
	{
		if (Interlocked.CompareExchange(ref m_SaveStatus, 1, 0) == 0)
		{
			ThreadPool.QueueUserWorkItem(ThreadPoolSaveFile, null);
		}
	}

	private HslMessageItem GetAndRemoveLogItem()
	{
		HslMessageItem result = null;
		m_simpleHybirdLock.Enter();
		try
		{
			result = ((m_WaitForSave.Count > 0) ? m_WaitForSave.Dequeue() : null);
		}
		catch
		{
		}
		m_simpleHybirdLock.Leave();
		return result;
	}

	private List<HslMessageItem> GetAndRemoveLogItemArray()
	{
		List<HslMessageItem> list = new List<HslMessageItem>();
		m_simpleHybirdLock.Enter();
		try
		{
			while (m_WaitForSave.Count > 0)
			{
				list.Add(m_WaitForSave.Dequeue());
			}
		}
		catch
		{
		}
		m_simpleHybirdLock.Leave();
		return list;
	}

	private void SetConsoleWriteForeColor(HslMessageDegree degree)
	{
		if (!ConsoleColorEnable)
		{
			return;
		}
		if (lastConsoleDegree < 0)
		{
			lastConsoleDegree = (int)degree;
		}
		if (lastConsoleDegree != (int)degree)
		{
			lastConsoleDegree = (int)degree;
			switch (degree)
			{
			case HslMessageDegree.DEBUG:
				Console.ForegroundColor = ConsoleColor.DarkGray;
				break;
			case HslMessageDegree.INFO:
				Console.ForegroundColor = ConsoleColor.White;
				break;
			case HslMessageDegree.WARN:
				Console.ForegroundColor = ConsoleColor.Yellow;
				break;
			case HslMessageDegree.ERROR:
				Console.ForegroundColor = ConsoleColor.Red;
				break;
			case HslMessageDegree.FATAL:
				Console.ForegroundColor = ConsoleColor.DarkRed;
				break;
			default:
				Console.ForegroundColor = ConsoleColor.White;
				break;
			}
		}
	}

	private void ConsoleWriteLog(HslMessageItem log)
	{
		SetConsoleWriteForeColor(log.Degree);
		Console.WriteLine(HslMessageFormat(log, writeFile: false));
	}

	private void ThreadPoolSaveFile(object obj)
	{
		HslMessageItem andRemoveLogItem = GetAndRemoveLogItem();
		m_fileSaveLock.Enter();
		string text = "";
		try
		{
			text = GetFileSaveName();
		}
		catch
		{
			m_fileSaveLock.Leave();
			Interlocked.Exchange(ref m_SaveStatus, 0);
			if (andRemoveLogItem != null)
			{
				Interlocked.Increment(ref andRemoveLogItem.WriteRetryTimes);
				AddItemToCache(andRemoveLogItem, start: true);
			}
			return;
		}
		if (disposedValue)
		{
			m_fileSaveLock.Leave();
			Interlocked.Exchange(ref m_SaveStatus, 0);
			return;
		}
		bool createNewLogFile = false;
		if (!string.IsNullOrEmpty(text))
		{
			if (text != lastLogSaveFileName)
			{
				createNewLogFile = true;
				lastLogSaveFileName = text;
			}
			StreamWriter streamWriter = null;
			int num = 0;
			try
			{
				streamWriter = new StreamWriter(text, append: true, EncoderShouldEmitUTF8Identifier ? Encoding.UTF8 : new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
				while (andRemoveLogItem != null && !disposedValue)
				{
					if (!andRemoveLogItem.HasLogOutput)
					{
						andRemoveLogItem.HasLogOutput = true;
						if (ConsoleOutput)
						{
							ConsoleWriteLog(andRemoveLogItem);
						}
						OnBeforeSaveToFile(new HslEventArgs
						{
							HslMessage = andRemoveLogItem
						});
						LogNetStatistics?.StatisticsAdd(1L);
					}
					bool flag = true;
					if (filtrateKeyword.Count > 0)
					{
						lock (filtrateLock)
						{
							flag = !filtrateKeyword.Contains(andRemoveLogItem.KeyWord);
						}
					}
					if (andRemoveLogItem.Cancel)
					{
						flag = false;
					}
					if (flag)
					{
						streamWriter.Write(HslMessageFormat(andRemoveLogItem, writeFile: true));
						streamWriter.Write(Environment.NewLine);
						num++;
					}
					andRemoveLogItem = GetAndRemoveLogItem();
				}
				if (num > 0)
				{
					streamWriter.Flush();
				}
			}
			catch (Exception ex)
			{
				if (ex is DirectoryNotFoundException)
				{
					try
					{
						string directoryName = Path.GetDirectoryName(text);
						if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
						{
							Directory.CreateDirectory(directoryName);
						}
					}
					catch
					{
					}
				}
				if (andRemoveLogItem != null)
				{
					int num2 = Interlocked.Increment(ref andRemoveLogItem.WriteRetryTimes);
					if (num2 <= 10)
					{
						AddItemToCache(andRemoveLogItem, start: false);
					}
					else
					{
						Interlocked.Increment(ref dropLogCount);
					}
				}
			}
			finally
			{
				streamWriter?.Dispose();
			}
		}
		else if (ConsoleOutput)
		{
			if (andRemoveLogItem != null)
			{
				ConsoleWriteLog(andRemoveLogItem);
				OnBeforeSaveToFile(new HslEventArgs
				{
					HslMessage = andRemoveLogItem
				});
			}
			List<HslMessageItem> andRemoveLogItemArray = GetAndRemoveLogItemArray();
			while (andRemoveLogItemArray.Count > 0 && !disposedValue)
			{
				StringBuilder stringBuilder = new StringBuilder();
				HslMessageDegree degree = andRemoveLogItemArray[0].Degree;
				for (int i = 0; i < andRemoveLogItemArray.Count; i++)
				{
					if (andRemoveLogItemArray[i].Degree != degree)
					{
						SetConsoleWriteForeColor(degree);
						Console.Write(stringBuilder.ToString());
						degree = andRemoveLogItemArray[i].Degree;
						stringBuilder = new StringBuilder();
					}
					stringBuilder.AppendLine(HslMessageFormat(andRemoveLogItemArray[i], writeFile: false));
					if (i >= andRemoveLogItemArray.Count - 1)
					{
						SetConsoleWriteForeColor(degree);
						Console.Write(stringBuilder.ToString());
					}
					OnBeforeSaveToFile(new HslEventArgs
					{
						HslMessage = andRemoveLogItemArray[i]
					});
				}
				andRemoveLogItemArray = GetAndRemoveLogItemArray();
			}
		}
		else
		{
			while (andRemoveLogItem != null && !disposedValue)
			{
				if (ConsoleOutput)
				{
					ConsoleWriteLog(andRemoveLogItem);
				}
				OnBeforeSaveToFile(new HslEventArgs
				{
					HslMessage = andRemoveLogItem
				});
				andRemoveLogItem = GetAndRemoveLogItem();
			}
		}
		m_fileSaveLock.Leave();
		Interlocked.Exchange(ref m_SaveStatus, 0);
		OnWriteCompleted(createNewLogFile);
		m_simpleHybirdLock.Enter();
		if (m_WaitForSave.Count > 0)
		{
			HslHelper.ThreadSleep(0);
			StartSaveFile();
		}
		m_simpleHybirdLock.Leave();
	}

	/// <summary>
	/// 根据需要存储的日志消息，获取实际存储的字符串，重写本方法就可以自定义输出内容
	/// According to the log message that needs to be stored, get the actual stored string, and the rewrite method can customize the output content
	/// </summary>
	/// <param name="hslMessage">等待存储或是输出的日志对象</param>
	/// <param name="writeFile">是否输出到文件里</param>
	/// <returns>最终等待存储的字符串内容</returns>
	protected virtual string HslMessageFormat(HslMessageItem hslMessage, bool writeFile)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (hslMessage.Degree != HslMessageDegree.None)
		{
			if (writeFile && LogStxAsciiCode)
			{
				stringBuilder.Append("\u0002");
			}
			stringBuilder.Append("[");
			stringBuilder.Append(LogNetManagment.GetDegreeDescription(hslMessage.Degree));
			stringBuilder.Append("] ");
			stringBuilder.Append(hslMessage.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
			stringBuilder.Append(" ");
			if (LogThreadID)
			{
				stringBuilder.Append("Thread:[");
				stringBuilder.Append(hslMessage.ThreadId.ToString("D3"));
				stringBuilder.Append("] ");
			}
			if (hslMessage.WriteRetryTimes == 2)
			{
				stringBuilder.Append("[Retry] ");
			}
			else if (hslMessage.WriteRetryTimes > 2)
			{
				stringBuilder.Append($"[Retry:{hslMessage.WriteRetryTimes}] ");
			}
			if (!string.IsNullOrEmpty(hslMessage.KeyWord))
			{
				stringBuilder.Append(hslMessage.KeyWord);
				stringBuilder.Append(" : ");
			}
		}
		stringBuilder.Append(hslMessage.Text);
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		return $"LogNetBase[{LogSaveMode}]";
	}

	protected virtual string GetFileSaveName()
	{
		return string.Empty;
	}

	/// <summary>
	/// 当写入文件完成的时候触发，这时候已经释放了文件的句柄了。
	/// Triggered when writing to the file is complete, and the file handle has been released.
	/// </summary>
	protected virtual void OnWriteCompleted(bool createNewLogFile)
	{
	}

	private HslMessageItem GetHslMessageItem(HslMessageDegree degree, string keyWord, string text)
	{
		if (HourDeviation == 0)
		{
			return new HslMessageItem
			{
				KeyWord = keyWord,
				Degree = degree,
				Text = text,
				ThreadId = Thread.CurrentThread.ManagedThreadId
			};
		}
		return new HslMessageItem
		{
			KeyWord = keyWord,
			Degree = degree,
			Text = text,
			ThreadId = Thread.CurrentThread.ManagedThreadId,
			Time = DateTime.Now.AddHours(HourDeviation)
		};
	}

	private int CalculateStringOccupyLength(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < str.Length; i++)
		{
			num = ((str[i] < '一' || str[i] > '龻') ? (num + 1) : (num + 2));
		}
		return num;
	}

	private void AppendCharToStringBuilder(StringBuilder sb, char c, int count)
	{
		for (int i = 0; i < count; i++)
		{
			sb.Append(c);
		}
	}

	/// <summary>
	/// 释放资源
	/// </summary>
	/// <param name="disposing">是否初次调用</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			disposedValue = true;
			HslHelper.ThreadSleep(50);
			if (disposing)
			{
				BeforeSaveToFile = null;
				m_simpleHybirdLock.Enter();
				m_WaitForSave.Clear();
				m_simpleHybirdLock.Leave();
				m_simpleHybirdLock.Dispose();
				m_fileSaveLock.Dispose();
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
