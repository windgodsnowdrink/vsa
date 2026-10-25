// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HslCommunication.BasicFramework;

public class SoftMsgQueue<T> : SoftFileSaveBase
{
	private Queue<T> all_items = new Queue<T>();

	private int m_Max_Cache = 200;

	private object lock_queue = new object();

	public int MaxCache
	{
		get
		{
			return m_Max_Cache;
		}
		set
		{
			if (value > 10)
			{
				m_Max_Cache = value;
			}
		}
	}

	public T CurrentItem
	{
		get
		{
			lock (lock_queue)
			{
				if (all_items.Count > 0)
				{
					return all_items.Peek();
				}
				return default(T);
			}
		}
	}

	public SoftMsgQueue()
	{
		base.LogHeaderText = "SoftMsgQueue<" + typeof(T).ToString() + ">";
	}

	public void AddNewItem(T item)
	{
		lock (lock_queue)
		{
			while (all_items.Count >= m_Max_Cache)
			{
				all_items.Dequeue();
			}
			all_items.Enqueue(item);
		}
	}

	public override string ToSaveString()
	{
		lock (lock_queue)
		{
			return ((object)JArray.FromObject((object)all_items)).ToString();
		}
	}

	public override void LoadByString(string content)
	{
		JArray val = JArray.Parse(content);
		lock (lock_queue)
		{
			all_items = (Queue<T>)((JToken)val).ToObject(typeof(Queue<T>));
		}
	}
}
