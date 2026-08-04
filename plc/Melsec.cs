#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Http.Extensions@2.3.9
#:package Microsoft.Extensions.ApiDescription.Server@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Caching.Hybrid@10.4.0
#:package Microsoft.Extensions.Caching.Redis@2.3.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Configuration.Json@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Configuration@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.DependencyInjection.AutoActivation@10.4.0
#:package Microsoft.Extensions.DependencyInjection@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.DependencyModel@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Diagnostics@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Hosting@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Hosting@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Http.Polly@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Http.Resilience@10.4.0
#:package Microsoft.Extensions.Http@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Identity.Core@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Identity.Stores@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Localization.Abstractions@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Localization@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Logging.Console@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Logging@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.ObjectPool@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Options.ConfigurationExtensions@11.0.0-preview.2.26159.112
#:package Microsoft.Extensions.Resilience@10.4.0
#:package Microsoft.Extensions.ServiceDiscovery@10.4.0
#:package Microsoft.Extensions.WebEncoders@11.0.0-preview.2.26159.112
#:package Microsoft.OpenApi@3.4.0
#:package Pipelines.Sockets.Unofficial@2.2.8
#:package Serilog.Sinks.Console@6.1.1
#:package Serilog@4.3.2-dev-02419
#:package System.Globalization.Extensions@4.3.0
#:package System.Net.Http.Json@11.0.0-preview.2.26159.112
#:package System.Reflection.Extensions@4.3.0
#:package System.Runtime.Extensions@4.3.1
#:package System.Text.Encoding.Extensions@4.3.0
#:package System.Text.Json@11.0.0-preview.2.26159.112
#:package System.Threading.Channels@11.0.0-preview.2.26159.112
#:package System.Threading.Tasks.Extensions@4.6.3
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=false
#:project ../Feature

using Feature;
using Feature.IoT.Clients;
using Feature.IoT.Controllers;
using Feature.IoT.Drivers;
using Feature.IoT.Features;
using Feature.IoT.Models;
using Feature.IoT.ThingModels;
using Feature.IoT.ThingSpecification;
using Feature.IoT;
using Feature.IoTDatabase.Drivers;
using Feature.IoTSerial.Drivers;
using Feature.IotSocket.Drivers;
using System.ComponentModel;

// 三菱电机 PLC（Melsec PLC）
namespace Feature.Melsec.Drivers
{
    using Feature.Data;
    using Feature.IoT.Drivers;
    using Feature.IoT.ThingModels;
    using Feature.IoT.ThingSpecification;
    using Feature.IoT;
    using Feature.Log;
    using Feature.Melsec.Protocols;
    using Feature.Reflection;
    using Feature.Serialization;
    using Feature.Threading;
    using Feature.Xml;
    using HslCommunication.Core;
    using HslCommunication.Profinet.Melsec;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Text;
    using System.Xml.Serialization;
    using System.Xml;
    using System;

    internal static class HexHelper
    {
        ///// <summary>字节转为1个16进制字符</summary>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static String ToHexString(this Byte b) => Convert.ToString(b, 16);

        /// <summary>
        /// 1个字节转为2个16进制字符
        /// </summary>
        /// <param name="b"> </param>
        /// <returns> </returns>
        public static String ToHexChars(this Byte b)
        {
            //Convert.ToString(b, 16);
            var cs = new Char[2];
            var ch = b >> 4;
            var cl = b & 0x0F;
            cs[0] = (Char)(ch >= 0x0A ? ('A' + ch - 0x0A) : ('0' + ch));
            cs[1] = (Char)(cl >= 0x0A ? ('A' + cl - 0x0A) : ('0' + cl));

            return new String(cs);
        }

        /// <summary>
        /// 字节数组转为16进制字符数组
        /// </summary>
        /// <param name="bytes"> </param>
        /// <returns> </returns>
        public static String ToHexString(this Byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(ToHexChars(bytes[i]));
            }
            return sb.ToString();
        }

        //public static Byte ToByte(this String str) => Convert.ToByte(str, 16);

        /// <summary>
        /// 每个字符转为1个字节
        /// </summary>
        /// <param name="str"> </param>
        /// <returns> </returns>
        public static Byte[] ToChars(this String str)
        {
            var buf = new Byte[str.Length];
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                //if (ch >= '0' && ch <= 9)
                //    buf[i] = (Byte)(ch - '0');
                buf[i] = Convert.ToByte(ch);
            }

            return buf;
        }

        /// <summary>
        /// 每个字符转为1个字节
        /// </summary>
        /// <param name="str"> </param>
        /// <returns> </returns>
        public static Byte[] ToBytes(this String str)
        {
            var buf = new Byte[str.Length];
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                if (ch >= '0' && ch <= '9')
                    buf[i] = (Byte)(ch - '0');
                else if (ch >= 'A' && ch <= 'F')
                    buf[i] = (Byte)(ch - 'A' + 0x0A);
            }

            return buf;
        }

        /// <summary>
        /// 从字符串指定位置截取2个字符转为字节
        /// </summary>
        /// <param name="str">    </param>
        /// <param name="offset"> </param>
        /// <returns> </returns>
        public static Byte ToByte(this String str, Int32 offset) => Convert.ToByte(str.Substring(offset, 2), 16);
    }

    /// <summary>
    /// 三菱PLC驱动
    /// </summary>
    [Driver("MelsecFxLinks")]
    [DisplayName("三菱FxLinks")]
    public class FxLinksDriver : DriverBase
    {
        /// <summary>
        /// 链接
        /// </summary>
        public FxLinks Link { get; set; }

        /// <summary>
        /// 打开通道数量
        /// </summary>
        private Int32 _nodes;

        /// <summary>
        /// 创建驱动参数对象，可序列化成Xml/Json作为该协议的参数模板
        /// </summary>
        /// <returns> </returns>
        public override IDriverParameter CreateParameter(String parameter)
        {
            if (parameter.IsNullOrEmpty()) return new FxLinksParameter
            {
                PortName = "COM1",
                Baudrate = 9600,
                Host = 1,
            };

            return parameter.ToXmlEntity<FxLinksParameter>();
        }

        /// <summary>
        /// 从点位中解析地址
        /// </summary>
        /// <param name="point"> </param>
        /// <returns> </returns>
        public virtual UInt16 GetAddress(IPoint point)
        {
            if (point == null) throw new ArgumentException("点位信息不能为空！");

            // 去掉冒号后面的位域
            var addr = point.Address;
            var p = addr.IndexOf(':');
            if (p > 0) addr = addr[..p];

            return (UInt16)addr.ToInt();
        }

        /// <summary>
        /// 打开通道.一个ModbusTcp设备可能分为多个通道读取，需要共用Tcp连接，以不同节点区分
        /// </summary>
        /// <param name="device">    通道 </param>
        /// <param name="parameter"> 参数 </param>
        /// <returns> </returns>
        public override INode Open(IDevice device, IDriverParameter parameter)
        {
            using var span = Tracer?.NewSpan("fxlinks:parameter", parameter.ToJson());

            var p = parameter as FxLinksParameter;
            if (p == null) throw new ArgumentException($"参数不合法：{parameter.ToJson()}");

            if (p.Baudrate <= 0) p.Baudrate = 9600;

            var node = new MelsecNode
            {
                Address = p.PortName,
                Host = p.Host,

                Driver = this,
                Device = device,
                Parameter = p,
            };

            // 实例化
            if (Link == null)
            {
                lock (this)
                {
                    if (Link == null)
                    {
                        var link = new FxLinks
                        {
                            PortName = p.PortName,
                            Baudrate = p.Baudrate,
                            DataBits = p.DataBits,
                            Parity = p.Parity,
                            StopBits = p.StopBits,

                            Log = Log,
                            Tracer = Tracer,
                        };

                        if (p.Timeout > 0) link.Timeout = p.Timeout;

                        //if (Log != null && Log.Level <= LogLevel.Debug) link.Log = Log;

                        // 外部已指定通道时，打开连接
                        if (device != null) link.Open();

                        Link = link;
                    }
                }
            }

            Interlocked.Increment(ref _nodes);

            return node;
        }

        /// <summary>
        /// 关闭设备驱动
        /// </summary>
        /// <param name="node"> </param>
        public override void Close(INode node)
        {
            if (Interlocked.Decrement(ref _nodes) <= 0)
            {
                Link.TryDispose();
                Link = null;
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="node">   节点对象，可存储站号等信息，仅驱动自己识别 </param>
        /// <param name="points"> 点位集合，Address属性地址示例：D100、C100、W100、H100 </param>
        /// <returns> </returns>
        public override IDictionary<String, Object> Read(INode node, IPoint[] points)
        {
            var dic = new Dictionary<String, Object>();

            if (points == null || points.Length == 0) return dic;

            var n = node as MelsecNode;
            var p = node.Parameter as FxLinksParameter;

            // 组合多个片段，减少读取次数
            var list = BuildSegments(points, p);

            // 加锁，避免冲突
            lock (Link)
            {
                // 分段整体读取
                for (var i = 0; i < list.Count; i++)
                {
                    var seg = list[i];

                    // 其中一项读取报错时，直接跳过，不要影响其它批次
                    try
                    {
                        // 根据点位地址类型，使用不同操作，尽管字读取也可以用来读取位存储区，但很容易混淆，并且不好解析，因此不支持
                        if (seg.Code.EqualIgnoreCase("X", "Y", "M"))
                            seg.Bits = Link.ReadBit(n.Host, seg.Code + seg.Address, (Byte)seg.Count);
                        else if (seg.Code.EqualIgnoreCase("D"))
                            seg.Values = Link.ReadWord(n.Host, seg.Code + seg.Address, (Byte)seg.Count);
                        else
                            throw new NotSupportedException($"{seg.Code}{seg.Address} is unkown address");
                    }
                    catch (Exception ex)
                    {
                        Log?.Error(ex.ToString());
                    }

                    // 读取时延迟一点时间
                    if (i < list.Count - 1 && p.BatchDelay > 0) Thread.Sleep(p.BatchDelay);
                }
            }

            // 分割数据
            return Dispatch(points, list);
        }

        internal IList<Segment> BuildSegments(IList<IPoint> points, FxLinksParameter p)
        {
            // 组合多个片段，减少读取次数
            var list = new List<Segment>();
            foreach (var point in points)
            {
                var cmd = point.Address[..1];
                var addr = point.Address[1..].ToInt();

                list.Add(new Segment
                {
                    Code = cmd,
                    Address = addr,
                    Count = 1
                });
            }
            list = list.OrderBy(e => e.Code).ThenBy(e => e.Address).ThenByDescending(e => e.Count).ToList();

            var step = p.BatchStep > 1 ? p.BatchStep : 1;
            var k = 1;
            var rs = new List<Segment>();
            var prv = list[0];
            rs.Add(prv);
            for (var i = 1; i < list.Count; i++)
            {
                var cur = list[i];

                // 前一段末尾碰到了当前段开始，可以合并
                var flag = prv.Address + prv.Count + step > cur.Address;
                //// 如果是读取位存储区，间隔小于8都可以合并
                //if (!flag && cur.Code.EqualIgnoreCase("X", "Y", "M"))
                //{
                //    flag = prv.Address + prv.Count + 8 > cur.Address;
                //}

                // 前一段末尾碰到了当前段开始，可以合并
                if (flag && prv.Code == cur.Code)
                {
                    if (p.BatchSize <= 0 || k < p.BatchSize)
                    {
                        // 要注意，可能前后重叠，也可能前面区域比后面还大
                        var size = cur.Address + cur.Count - prv.Address;
                        if (size > prv.Count) prv.Count = size;

                        // 连续合并数累加
                        k++;
                    }
                    else
                    {
                        rs.Add(cur);

                        prv = cur;
                        k = 1;
                    }
                }
                else
                {
                    rs.Add(cur);

                    prv = cur;
                    k = 1;
                }
            }

            return rs;
        }

        internal IDictionary<String, Object> Dispatch(IPoint[] points, IList<Segment> segments)
        {
            var dic = new Dictionary<String, Object>();
            if (segments == null || segments.Count == 0) return dic;

            foreach (var point in points)
            {
                var cmd = point.Address[..1];
                var addr = point.Address[1..].ToInt();

                // 找到片段
                var seg = segments.FirstOrDefault(e => e.Code == cmd && e.Address <= addr && addr < e.Address + e.Count);
                if (seg != null)
                {
                    var code = seg.Code;
                    if (seg.Values != null)
                    {
                        // 校验数据完整性
                        var offset = addr - seg.Address;
                        if (seg.Values.Length > offset)
                            dic[point.Name] = seg.Values[offset];
                    }
                    else if (seg.Bits != null)
                    {
                        // 校验数据完整性
                        var offset = addr - seg.Address;
                        if (seg.Bits.Length > offset)
                            dic[point.Name] = seg.Bits[offset];
                    }
                    //else
                    //    throw new NotSupportedException($"无法拆分{code}");
                }
            }
            return dic;
        }

        [DebuggerDisplay("{Code}({Address}, {Count})")]
        internal class Segment
        {
            public String Code { get; set; }
            public Int32 Address { get; set; }
            public Int32 Count { get; set; }
            public Byte[] Bits { get; set; }
            public UInt16[] Values { get; set; }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="node">  节点对象，可存储站号等信息，仅驱动自己识别 </param>
        /// <param name="point"> 点位，Address属性地址示例：D100、C100、W100、H100 </param>
        /// <param name="value"> 数据 </param>
        /// <returns> </returns>
        /// <exception cref="ArgumentException"> </exception>
        public override Object Write(INode node, IPoint point, Object value)
        {
            if (value == null) return null;
            if (point.Address.IsNullOrEmpty()) return null;

            var n = node as MelsecNode;

            UInt16[] vs;
            if (value is Byte[] buf)
            {
                vs = new UInt16[(Int32)Math.Ceiling(buf.Length / 2d)];
                for (var i = 0; i < vs.Length; i++)
                {
                    vs[i] = buf.ToUInt16(i * 2, false);
                }
            }
            else
            {
                if (point.Address.StartsWithIgnoreCase("X", "Y", "M"))
                    vs = ConvertToBit(value, point, n.Device?.Specification);
                else
                    vs = ConvertToWord(value, point, n.Device?.Specification);

                if (vs == null) throw new NotSupportedException($"点位[{point.Name}]不支持数据[{value}]");
            }

            // 加锁，避免冲突
            lock (Link)
            {
                // 按照点位地址前缀决定使用哪一种写入方法，要求物模型必须配置对点位
                if (point.Address.StartsWithIgnoreCase("X", "Y", "M"))
                    return Link.WriteBit(n.Host, point.Address, vs);
                else if (point.Address.StartsWithIgnoreCase("D"))
                    return Link.WriteWord(n.Host, point.Address, vs);
                else
                    return Link.Write(point.Address[..1], n.Host, point.Address, vs);
            }
        }

        /// <summary>
        /// 原始数据转为线圈
        /// </summary>
        /// <param name="data">  </param>
        /// <param name="point"> </param>
        /// <param name="spec">  </param>
        /// <returns> </returns>
        protected virtual UInt16[] ConvertToBit(Object data, IPoint point, ThingSpec spec)
        {
            var type = TypeHelper.GetNetType(point);
            if (type == null)
            {
                // 找到物属性定义
                var pi = spec?.Properties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(point.Name));
                type = TypeHelper.GetNetType(pi?.DataType?.Type);
            }
            if (type == null) return null;

            DefaultSpan.Current?.AppendTag("ConvertToBit->" + type.FullName);

            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return data.ToBoolean() ? new[] { (UInt16)0x01 } : new[] { (UInt16)0x00 };
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return data.ToInt() > 0 ? new[] { (UInt16)0x01 } : new[] { (UInt16)0x00 };
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return data.ToLong() > 0 ? new[] { (UInt16)0x01 } : new[] { (UInt16)0x00 };
                default:
                    return data.ToBoolean() ? new[] { (UInt16)0x01 } : new[] { (UInt16)0x00 };
            }
        }

        /// <summary>
        /// 原始数据转寄存器数组
        /// </summary>
        /// <param name="data">  </param>
        /// <param name="point"> </param>
        /// <param name="spec">  </param>
        /// <returns> </returns>
        protected virtual UInt16[] ConvertToWord(Object data, IPoint point, ThingSpec spec)
        {
            var type = TypeHelper.GetNetType(point);
            if (type == null)
            {
                // 找到物属性定义
                var pi = spec?.Properties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(point.Name));
                type = TypeHelper.GetNetType(pi?.DataType?.Type);
            }
            if (type == null) return null;

            DefaultSpan.Current?.AppendTag("ConvertToWord->" + type.FullName);

            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return data.ToBoolean() ? new[] { (UInt16)0x01 } : new[] { (UInt16)0x00 };
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return new[] { (UInt16)data.ToInt() };
                case TypeCode.Int32:
                case TypeCode.UInt32:
                {
                    var n = data.ToInt();
                    return new[] { (UInt16)(n >> 16), (UInt16)(n & 0xFFFF) };
                }
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    var n = data.ToLong();
                    return new[] { (UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF) };
                }
                case TypeCode.Single:
                {
                    var d = (Single)data.ToDouble();
                    //var n = BitConverter.SingleToInt32Bits(d);
                    var n = (UInt32)d;
                    return new[] { (UInt16)(n >> 16), (UInt16)(n & 0xFFFF) };
                }
                case TypeCode.Double:
                {
                    var d = (Double)data.ToDouble();
                    //var n = BitConverter.DoubleToInt64Bits(d);
                    var n = (UInt64)d;
                    return new[] { (UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF) };
                }
                case TypeCode.Decimal:
                {
                    var d = data.ToDecimal();
                    var n = (UInt64)d;
                    return new[] { (UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF) };
                }
                //case TypeCode.String:
                //    break;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// 三菱FxLinks参数
    /// </summary>
    public class FxLinksParameter : IDriverParameter, IDriverParameterKey
    {
        /// <summary>
        /// 串口名称
        /// </summary>
        [Description("串口名称")]
        public String PortName { get; set; }

        /// <summary>
        /// 波特率.默认9600
        /// </summary>
        [Description("波特率")]
        public Int32 Baudrate { get; set; } = 9600;

        /// <summary>
        /// 数据位.默认7
        /// </summary>
        [Description("数据位")]
        public Int32 DataBits { get; set; } = 7;

        /// <summary>
        /// 奇偶校验位.默认Even偶校验
        /// </summary>
        [Description("奇偶校验位")]
        public Parity Parity { get; set; } = Parity.Even;

        /// <summary>
        /// 停止位.默认One
        /// </summary>
        [Description("停止位")]
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 主机/站号
        /// </summary>
        [Description("主机/站号")]
        public Byte Host { get; set; }

        /// <summary>
        /// 网络超时.发起请求后等待响应的超时时间，默认3000ms
        /// </summary>
        [Description("网络超时.发起请求后等待响应的超时时间，默认3000ms")]
        public Int32 Timeout { get; set; } = 3000;

        /// <summary>
        /// 批间隔.两个点位地址小于等于该值时凑为一批，默认1
        /// </summary>
        [Description("批间隔.两个点位地址小于等于该值时凑为一批，默认1")]
        public Int32 BatchStep { get; set; } = 1;

        /// <summary>
        /// 批大小.凑批请求时，每批最多点位个数
        /// </summary>
        [Description("批大小.凑批请求时，每批最多点位个数")]
        public Int32 BatchSize { get; set; }

        /// <summary>
        /// 批延迟.相邻请求之间的延迟时间，单位毫秒
        /// </summary>
        [Description("批延迟.相邻请求之间的延迟时间，单位毫秒")]
        public Int32 BatchDelay { get; set; }

        /// <summary>
        /// 获取驱动参数的唯一标识
        /// </summary>
        /// <returns> </returns>
        public String GetKey() => PortName;
    }

    /// <summary>
    /// 三菱PLC驱动
    /// </summary>
    [Driver("MelsecPLC")]
    [DisplayName("三菱PLC")]
    public class MelsecDriver : DriverBase
    {
        private IReadWriteNet _plcNet;

        /// <summary>
        /// 打开通道数量
        /// </summary>
        private Int32 _nodes;

        /// <summary>
        /// 创建驱动参数对象，可序列化成Xml/Json作为该协议的参数模板
        /// </summary>
        /// <returns> </returns>
        public override IDriverParameter GetDefaultParameter() => new MelsecParameter
        {
            Address = "127.0.0.1:6000",
            DataFormat = "CDAB",
            Protocol = Protocol.MCQna3E
        };

        /// <summary>
        /// 从点位中解析地址
        /// </summary>
        /// <param name="point"> </param>
        /// <returns> </returns>
        public virtual String GetAddress(IPoint point)
        {
            if (point == null) throw new ArgumentException("点位信息不能为空！");

            // 去掉冒号后面的位域
            var addr = point.Address;
            var p = addr.IndexOf(':');
            if (p > 0) addr = addr.Substring(0, p);

            return addr;
        }

        /// <summary>
        /// 打开通道.一个ModbusTcp设备可能分为多个通道读取，需要共用Tcp连接，以不同节点区分
        /// </summary>
        /// <param name="device">     通道 </param>
        /// <param name="parameters"> 参数 </param>
        /// <returns> </returns>
        public override INode Open(IDevice device, IDictionary<String, Object> parameters)
        {
            var pm = JsonHelper.Convert<MelsecParameter>(parameters);

            if (pm == null) throw new ArgumentException($"参数不合法：{parameters.ToJson()}");

            MelsecNode node;
            String ipAddress;
            Int32 port;

            if (pm.Protocol == Protocol.MCQna3E)
            {
                var address = pm.Address;
                if (address.IsNullOrEmpty()) throw new ArgumentException("参数中未指定地址address");

                var p = address.IndexOf(':');
                if (p < 0) throw new ArgumentException($"参数中地址address格式错误:{address}");



                node = new MelsecNode
                {
                    Address = address,

                    Driver = this,
                    Device = device,
                    Parameter = pm,
                };
            }
            else
            {
                if (pm.PortName.IsNullOrEmpty()) throw new ArgumentException("参数中未指定串口名称PortName");

                node = new MelsecNode
                {
                    Address = pm.PortName,

                    Driver = this,
                    Device = device,
                    Parameter = pm,
                };
            }



            if (_plcNet == null)
            {
                lock (this)
                {
                    if (_plcNet == null)
                    {
                        if (pm.Protocol == Protocol.MCQna3E)
                        {
                            var address = pm.Address;
                            var p = address.IndexOf(':');

                            ipAddress = address.Substring(0, p);
                            port = address.Substring(p + 1).ToInt();

                            //MelsecA3CNet
                            var plcNet = new MelsecMcNet
                            {
                                ConnectTimeOut = 3000,
                                IpAddress = ipAddress,
                                Port = port,
                            };

                            _plcNet = plcNet;

                            if (!pm.DataFormat.IsNullOrEmpty() && Enum.TryParse<DataFormat>(pm.DataFormat, out var format))
                            {
                                plcNet.ByteTransform.DataFormat = format;
                            }

                            var connect = plcNet.ConnectServer();

                            if (!connect.IsSuccess) throw new Exception($"连接失败：{connect.Message}");
                        }
                        else
                        {
                            var melsecSerial = new MelsecFxLinks();
                            _plcNet = melsecSerial;

                            var baudRate = 9600;
                            var dataBits = 7;
                            var stopBits = 1;
                            var parity = 2;
                            byte station = 0;

                            melsecSerial.SerialPortInni(sp =>
                            {
                                sp.PortName = pm.PortName;
                                sp.BaudRate = pm.Baudrate;
                                sp.DataBits = 7;// dataBits;
                                sp.StopBits = StopBits.One;
                                sp.Parity = Parity.Even;
                            });
                            melsecSerial.Station = station;
                            melsecSerial.WaittingTime = 0;
                            melsecSerial.SumCheck = true;
                            melsecSerial.Format = 1;

                            var connect = melsecSerial.Open();
                            if (!connect.IsSuccess) throw new Exception($"连接失败：{connect.Message}");
                        }

                    }
                }
            }

            Interlocked.Increment(ref _nodes);

            return node;
        }

        /// <summary>
        /// 关闭设备驱动
        /// </summary>
        /// <param name="node"> </param>
        public override void Close(INode node)
        {
            if (Interlocked.Decrement(ref _nodes) <= 0)
            {
                if (_plcNet is MelsecMcNet plcNet) plcNet?.ConnectClose();
                _plcNet.TryDispose();
                _plcNet = null;
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="node">   节点对象，可存储站号等信息，仅驱动自己识别 </param>
        /// <param name="points"> 点位集合，Address属性地址示例：D100、C100、W100、H100 </param>
        /// <returns> </returns>
        public override IDictionary<String, Object> Read(INode node, IPoint[] points)
        {
            var dic = new Dictionary<String, Object>();

            if (points == null || points.Length == 0) return dic;

            foreach (var point in points)
            {
                var name = point.Name;
                var addr = GetAddress(point);
                var length = point.Length;
                var data = _plcNet.Read(addr, (UInt16)(length / 2));

                if (!data.IsSuccess)
                {
                    var r = _plcNet.ReadUInt16(addr);
                    XTrace.WriteLine($"读取数据：{addr}={r.ToJson()}");
                }
                if (!data.IsSuccess) throw new Exception($"读取数据失败：{data.ToJson()}");



                dic[name] = data.Content;
            }

            return dic;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="node">  节点对象，可存储站号等信息，仅驱动自己识别 </param>
        /// <param name="point"> 点位，Address属性地址示例：D100、C100、W100、H100 </param>
        /// <param name="value"> 数据 </param>
        /// <returns> </returns>
        /// <exception cref="ArgumentException"> </exception>
        public override Object Write(INode node, IPoint point, Object value)
        {
            var addr = GetAddress(point);
            var res = value switch
            {
                Int32 v1 => _plcNet.Write(addr, v1),
                String v2 => _plcNet.Write(addr, v2),
                Boolean v3 => _plcNet.Write(addr, v3),
                Byte[] v4 => _plcNet.Write(addr, v4),
                Byte v5 => _plcNet.Write(addr, v5),
                _ => throw new ArgumentException("暂不支持写入该类型数据！"),
            };
            return res;
        }
    }

    /// <summary>
    /// 节点
    /// </summary>
    public class MelsecNode : INode
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// 站号
        /// </summary>
        public Byte Host { get; set; }

        /// <summary>
        /// 通道
        /// </summary>
        public IDriver Driver { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public IDevice Device { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public IDriverParameter Parameter { get; set; }
    }

    /// <summary>
    /// Melsec参数
    /// </summary>
    public class MelsecParameter : IDriverParameter
    {
        /// <summary>
        /// 地址.例如 127.0.0.1:6000
        /// </summary>
        [Description("地址.例如 127.0.0.1:6000")]
        public String Address { get; set; }

        /// <summary>
        /// 数据格式.ABCD/BADC/CDAB/DCBA
        /// </summary>
        [Description("数据格式.ABCD/BADC/CDAB/DCBA")]
        public String DataFormat { get; set; }

        /// <summary>
        /// 通信协议.MCQna3E/FxLinks485
        /// </summary>
        [Description("通信协议.MCQna3E/FxLinks485")]
        public Protocol Protocol { get; set; } = Protocol.MCQna3E;

        /// <summary>
        /// 串口名称
        /// </summary>
        [Description("串口名称")]
        public String PortName { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        [Description("波特率")]
        public Int32 Baudrate { get; set; } = 9600;
    }

    /// <summary>
    /// 通信协议
    /// </summary>
    public enum Protocol
    {
        /// <summary>
        /// MC协议Qna-3E模式
        /// </summary>
        MCQna3E,

        /// <summary>
        /// FX系列计算机链
        /// </summary>
        FxLinks485
    }

    /// <summary>
    /// 串口配置
    /// </summary>
    [XmlConfigFile("Config\\Serial.config")]
    public class SerialPortConfig : XmlConfig<SerialPortConfig>
    {
        /// <summary>
        /// 串口名
        /// </summary>
        [Description("串口名")]
        public String PortName { get; set; } = "COM1";

        /// <summary>
        /// 波特率
        /// </summary>
        [Description("波特率")]
        public Int32 BaudRate { get; set; } = 9600;

        /// <summary>
        /// 数据位
        /// </summary>
        [Description("数据位")]
        public Int32 DataBits { get; set; } = 8;

        /// <summary>
        /// 停止位
        /// </summary>
        [Description("停止位 None/One/Two/OnePointFive")]
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 奇偶校验
        /// </summary>
        [Description("奇偶校验 None/Odd/Even/Mark/Space")]
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// 文本编码
        /// </summary>
        [XmlIgnore]
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 编码
        /// </summary>
        [Description("编码 gb2312/us-ascii/utf-8")]
        public String WebEncoding { get { return Encoding.WebName; } set { Encoding = Encoding.GetEncoding(value); } }

        /// <summary>
        /// 十六进制显示
        /// </summary>
        [Description("十六进制显示")]
        public Boolean HexShow { get; set; }

        /// <summary>
        /// 十六进制自动换行
        /// </summary>
        [Description("十六进制自动换行")]
        public Boolean HexNewLine { get; set; }

        /// <summary>
        /// 十六进制发送
        /// </summary>
        [Description("十六进制发送")]
        public Boolean HexSend { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Description("最后更新时间")]
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// 扩展数据
        /// </summary>
        [Description("扩展数据")]
        public String Extend { get; set; } = "";

        /// <summary>
        /// DtrEnable
        /// </summary>
        [Description("DtrEnable")]
        public Boolean DtrEnable { get; set; } = false;

        /// <summary>
        /// RtsEnable
        /// </summary>
        [Description("RtsEnable")]
        public Boolean RtsEnable { get; set; } = false;

        /// <summary>
        /// BreakState
        /// </summary>
        [Description("BreakState")]
        public Boolean BreakState { get; set; } = false;
    }

    /// <summary>
    /// 串口传输
    /// </summary>
    /// <example>
    /// 标准例程：
    /// <code>
    ///var st = new SerialTransport();
    ///st.PortName = "COM65";  // 通讯口
    ///st.FrameSize = 16;      // 数据帧大小
    ///
    ///st.Received += (s, e) =&gt;
    ///{
    ///Console.WriteLine("收到 {0}", e.ToHex());
    ///};
    /// // 开始异步操作
    ///st.Open();
    ///
    /////var buf = "01080000801A".ToHex();
    ///var buf = "0111C02C".ToHex();
    ///for (int i = 0; i &lt; 100; i++)
    ///{
    ///Console.WriteLine("发送 {0}", buf.ToHex());
    ///st.Send(buf);
    ///
    ///Thread.Sleep(1000);
    ///}
    /// </code>
    /// </example>
    public class SerialTransport : DisposeBase, ITransport
    {
        #region 属性
        private SerialPort _Serial;
        /// <summary>
        /// 串口对象
        /// </summary>
        public SerialPort Serial
        {
            get { return _Serial; }
            set
            {
                _Serial = value;
                if (_Serial != null)
                {
                    PortName = _Serial.PortName;
                    BaudRate = _Serial.BaudRate;
                    Parity = _Serial.Parity;
                    DataBits = _Serial.DataBits;
                    StopBits = _Serial.StopBits;
                }
            }
        }

        /// <summary>
        /// 端口名称.默认COM1
        /// </summary>
        public String PortName { get; set; } = "COM1";

        /// <summary>
        /// 波特率.默认115200
        /// </summary>
        public Int32 BaudRate { get; set; } = 115200;

        /// <summary>
        /// 奇偶校验位.默认None
        /// </summary>
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// 数据位.默认8
        /// </summary>
        public Int32 DataBits { get; set; } = 8;

        /// <summary>
        /// 停止位.默认One
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 超时时间.超过该大小未收到数据，说明是另一帧.默认10ms
        /// </summary>
        public Int32 Timeout { get; set; } = 10;

        private String _Description;
        /// <summary>
        /// 描述信息
        /// </summary>
        public String Description
        {
            get
            {
                if (_Description == null)
                {
                    var dic = GetNames();
                    if (!dic.TryGetValue(PortName, out _Description))
                        _Description = "";
                }
                return _Description;
            }
        }

        ///// <summary>粘包处理接口</summary>
        //public IPacket Packet { get; set; }

        /// <summary>
        /// 字节超时.数据包间隔，默认20ms
        /// </summary>
        public Int32 ByteTimeout { get; set; } = 20;
        #endregion

        #region 构造
        /// <summary>
        /// 串口传输
        /// </summary>
        public SerialTransport()
        {
            // 每隔一段时间检查一次串口是否已经关闭，如果串口已经不存在，则关闭该传输口
            timer = new TimerX(CheckDisconnect, null, 3000, 3000) { Async = true };
        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="disposing"> </param>
        //protected override void Dispose(Boolean disposing)
        //{
        //    base.Dispose(disposing);

        //    try
        //    {
        //        if (Serial != null) Close();
        //        if (timer != null) timer.Dispose();
        //    }
        //    catch { }
        //}
        #endregion

        #region 方法
        /// <summary>
        /// 确保创建
        /// </summary>
        public virtual void EnsureCreate()
        {
            if (Serial == null)
            {
                Serial = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);

                _Description = null;
            }
        }

        /// <summary>
        /// 打开
        /// </summary>
        public virtual Boolean Open()
        {
            EnsureCreate();

            if (!Serial.IsOpen)
            {
                Serial.Open();
                if (Received != null) Serial.DataReceived += DataReceived;
            }

            return true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual Boolean Close()
        {
            // 关闭时必须清空，否则更换属性后再次打开也无法改变属性
            var sp = Serial;
            if (sp != null)
            {
                Serial = null;
                if (Received != null) sp.DataReceived -= DataReceived;
                if (sp.IsOpen) sp.Close();

                OnDisconnect();
            }

            return true;
        }
        #endregion

        #region 发送
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="pk"> 数据包 </param>
        public virtual Int32 Send(IPacket pk)
        {
            if (!Open()) return -1;

            WriteLog("Send:{0}", pk.ToHex());

            var sp = Serial;
            lock (sp)
            {
                if (pk.TryGetArray(out var seg))
                    Serial.Write(seg.Array, seg.Offset, seg.Count);
                else
                    Serial.Write(pk.ReadBytes(), 0, pk.Total);
            }

            return pk.Total;
        }

        /// <summary>
        /// 异步发送数据并等待响应
        /// </summary>
        /// <param name="pk"> </param>
        /// <returns> </returns>
        public virtual async Task<IOwnerPacket> SendAsync(IPacket pk)
        {
            if (!Open()) return null;

            //if (Packet == null) Packet = new PacketProvider();

            //var task = Packet.Add(pk, null, Timeout);

            _Source = new TaskCompletionSource<IOwnerPacket>();

            if (pk != null)
            {
                WriteLog("SendAsync:{0}", pk.ToHex());

                // 发送数据
                if (pk.TryGetArray(out var seg))
                    Serial.Write(seg.Array, seg.Offset, seg.Count);
                else
                    Serial.Write(pk.ReadBytes(), 0, pk.Total);
            }

            return await _Source.Task;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns> </returns>
        public virtual IOwnerPacket Receive()
        {
            if (!Open()) return null;

            var task = SendAsync(null);
            if (Timeout > 0 && !task.Wait(Timeout)) return null;

            return task.Result;
        }
        #endregion

        #region 异步接收
        void DataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            // 发送者必须保持一定间隔，每个报文不能太大，否则会因为粘包拆包而出错
            try
            {
                var sp = sender as SerialPort;
                WaitMore();
                if (sp.BytesToRead > 0)
                {
                    //var buf = new Byte[sp.BytesToRead];
                    var pk = new OwnerPacket(sp.BytesToRead);

                    var count = sp.Read(pk.Buffer, 0, sp.BytesToRead);
                    //if (count != buf.Length) buf = buf.ReadBytes(0, count);
                    //var ms = new MemoryStream(buf, 0, count, false);
                    //var pk = new Packet(buf, 0, count);
                    pk.Resize(count);

                    ProcessReceive(pk);
                }
            }
            catch (Exception ex)
            {
                //WriteLog("Error " + ex.Message);
                if (Log != null) Log.Error("DataReceived Error {0}", ex.Message);
            }
        }

        void WaitMore()
        {
            var sp = Serial;

            var ms = ByteTimeout;
            var end = DateTime.Now.AddMilliseconds(ms);
            var count = sp.BytesToRead;
            while (sp.IsOpen && end > DateTime.Now)
            {
                //Thread.SpinWait(1);
                Thread.Sleep(ms);
                if (count != sp.BytesToRead)
                {
                    end = DateTime.Now.AddMilliseconds(ms);
                    count = sp.BytesToRead;
                }
            }
        }

        void ProcessReceive(IOwnerPacket pk)
        {
            try
            {
                //if (Packet == null)
                OnReceive(pk);
                //else
                //{
                //    // 拆包，多个包多次调用处理程序
                //    foreach (var msg in Packet.Parse(pk))
                //    {
                //        OnReceive(msg);
                //    }
                //}
            }
            catch (Exception ex)
            {
                if (!ex.IsDisposed()) Log.Error("{0}.OnReceive {1}", PortName, ex.Message);
            }
        }

        private TaskCompletionSource<IOwnerPacket> _Source;
        /// <summary>
        /// 处理收到的数据.默认匹配同步接收委托
        /// </summary>
        /// <param name="pk"> </param>
        internal virtual void OnReceive(IOwnerPacket pk)
        {
            //// 同步匹配
            //if (Packet != null && Packet.Match(pk, null)) return;

            if (_Source != null)
            {
                _Source.SetResult(pk);
                _Source = null;
                return;
            }

            // 触发事件
            Received?.Invoke(this, new ReceivedEventArgs { Packet = pk });
        }

        /// <summary>
        /// 数据到达事件
        /// </summary>
        public event EventHandler<ReceivedEventArgs> Received;
        #endregion

        #region 自动检测串口断开
        /// <summary>
        /// 断开时触发，可能是人为断开，也可能是串口链路断开
        /// </summary>
        public event EventHandler Disconnected;

        Boolean isInEvent;
        void OnDisconnect()
        {
            if (Disconnected != null)
            {
                // 判断是否在事件中，避免外部在断开时间中调用Close造成死循环
                if (!isInEvent)
                {
                    isInEvent = true;

                    Disconnected(this, EventArgs.Empty);

                    isInEvent = false;
                }
            }
        }

        TimerX timer;
        /// <summary>
        /// 检查串口是否已经断开
        /// </summary>
        /// <remarks> FX串口异步操作有严重的泄漏缺陷，如果外部硬件长时间断开， SerialPort.IsOpen检测不到，并且会无限大占用内存. </remarks>
        /// <param name="state"> </param>
        void CheckDisconnect(Object state)
        {
            if (String.IsNullOrEmpty(PortName) || Serial == null || !Serial.IsOpen) return;

            // 如果端口已经不存在，则断开吧
            if (!SerialPort.GetPortNames().Contains(PortName))
            {
                WriteLog("串口{0}已经不存在，准备关闭！", PortName);

                //OnDisconnect();
                Close();
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 获取带有描述的串口名，没有时返回空数组
        /// </summary>
        /// <returns> </returns>
        public static String[] GetPortNames()
        {
            var list = new List<String>();
            foreach (var item in GetNames())
            {
                list.Add(item.Key);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取串口列表，名称和描述
        /// </summary>
        /// <returns> </returns>
        public static Dictionary<String, String> GetNames()
        {
            var dic = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            //#if NC30
            foreach (var item in SerialPort.GetPortNames())
            {
                dic.Add(item, "");
            }
            //#else
            //            using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM", false))
            //            using (var usb = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USB", false))
            //            {
            //                if (key != null)
            //                {
            //                    foreach (var item in key.GetValueNames())
            //                    {
            //                        var name = key.GetValue(item) + "";
            //                        var des = "";

            // // 尝试枚举USB串口 foreach (var vid in usb.GetSubKeyNames()) { var usbvid = usb.OpenSubKey(vid); foreach (var elm in usbvid.GetSubKeyNames()) { var sub = usbvid.OpenSubKey(elm); //if
            // (sub.GetValue("Class") + "" == "Ports") { var FriendlyName = sub.GetValue("FriendlyName") + ""; if (FriendlyName.Contains($"({name})")) { des = FriendlyName.TrimEnd($"({name})").Trim();
            // break; } } } if (!des.IsNullOrEmpty()) break; }

            // // 最后选择设备映射的串口名 if (des.IsNullOrEmpty()) { des = item; var p = item.LastIndexOf('\\'); if (p >= 0) des = des.Substring(p + 1); }

            //                        //dic.Add(name, des);
            //                        // 某台机器上发现，串口有重复
            //                        dic[name] = des;
            //                    }
            //                }
            //            }
            //#endif

            return dic;
        }

        /// <summary>
        /// 从串口列表选择串口，支持自动选择关键字
        /// </summary>
        /// <param name="keyWord"> 串口名称或者描述符的关键字 </param>
        /// <returns> </returns>
        public static SerialTransport Choose(String keyWord = null)
        {
            var ns = GetNames();
            if (ns.Count == 0)
            {
                Console.WriteLine("没有可用串口！");
                return null;
            }

            var name = "";
            var des = "";

            Console.WriteLine("可用串口：");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var item in ns)
            {
                if (item.Value == "Serial0") continue;

                if (keyWord != null && (item.Key.EqualIgnoreCase(keyWord) || item.Value.Contains(keyWord)))
                {
                    name = item.Key;
                    des = item.Value;
                }

                //Console.WriteLine(item);
                Console.WriteLine("{0,5}({1})", item.Key, item.Value);
            }
            // 没有自动选择，则默认最后一个
            if (name.IsNullOrEmpty())
            {
                var item = ns.Last();
                name = item.Key;
                des = item.Value;
            }
            while (true)
            {
                Console.ResetColor();
                Console.Write("请输入串口名称（默认 ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{0}", name);
                Console.ResetColor();
                Console.Write("）：");

                var str = Console.ReadLine();
                if (str.IsNullOrEmpty()) break;

                // 只有输入有效串口名称才行
                if (ns.ContainsKey(str))
                {
                    name = str;
                    des = ns[str];
                    break;
                }
            }

            Console.WriteLine();
            Console.Write("正在打开串口 ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}({1})", name, des);

            Console.ResetColor();

            var sp = new SerialTransport
            {
                PortName = name
            };

            return sp;
        }
        #endregion

        #region 日志
        /// <summary>
        /// 日志对象
        /// </summary>
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="format"> </param>
        /// <param name="args">   </param>
        public void WriteLog(String format, params Object[] args)
        {
            if (Log != null && Log.Enable) Log.Info(format, args);
        }

        /// <summary>
        /// 已重载
        /// </summary>
        /// <returns> </returns>
        public override String ToString()
        {
            if (!String.IsNullOrEmpty(PortName))
                return PortName;
            else
                return "(SerialPort)";
        }


        #endregion
    }
}

// 三菱电机 PLC（Melsec PLC）计算机链路协议
namespace Feature.Melsec.Protocols
{
    using Feature.Data;
    using Feature.Log;
    using Feature.Serialization;
    using static System.Collections.Specialized.BitVector32;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// 控制码
    /// </summary>
    public enum ControlCodes : Byte
    {
        /// <summary>
        /// 询问.发出请求
        /// </summary>
        ENQ = 05,

        /// <summary>
        /// 文本起点.读取操作的响应开始
        /// </summary>
        STX = 02,

        /// <summary>
        /// 文本终点.读取操作的响应结束
        /// </summary>
        ETX = 03,

        /// <summary>
        /// 传送结束
        /// </summary>
        EOT = 04,

        /// <summary>
        /// 确认.写入操作的响应
        /// </summary>
        ACK = 06,

        /// <summary>
        /// 不确认
        /// </summary>
        NAK = 0x15,
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public enum ErrorCodes
    {
        ///// <summary>常规错误</summary>
        //Normal = 0x00,

        ///// <summary>常规错误</summary>
        //Normal2 = 0x01,

        /// <summary>
        /// 校验错误
        /// </summary>
        SumError = 0x02,

        /// <summary>
        /// 通信计数器错误
        /// </summary>
        /// <remarks> Protocol error (the communication protocol does not conform to the format selected with D8120) </remarks>
        ProtocolError = 0x03,

        /// <summary>
        /// 字符区域错误
        /// </summary>
        /// <remarks> Character area error (the character area is incorrectly defined, or the specified command is not available) </remarks>
        CharacterAreaError = 0x06,

        /// <summary>
        /// 字符区域错误
        /// </summary>
        /// <remarks> Character error (the data to be written to a device consists of ASCII codes other than hexadecimal codes) </remarks>
        CharacterError = 0x07,

        /// <summary>
        /// PLC号错误
        /// </summary>
        /// <remarks> PLC number error (the PLC number is not set to “FF” or not available from this station) </remarks>
        PLCError = 0x0A,

        /// <summary>
        /// PLC号错误
        /// </summary>
        /// <remarks> PLC number error (the PLC number is not set to “FF” or not available from this station) </remarks>
        PLCError2 = 0x10,

        /// <summary>
        /// 远程错误
        /// </summary>
        /// <remarks> Remote error (remote run/stop is disabled) </remarks>
        RemoteError = 0x18,
    }

    [assembly: InternalsVisibleTo("XUnitTest, PublicKey=00240000048000001401000006020000002400005253413100080000010001000d41eb3bdab5c2150958b46c95632b7e4dcb0af77ed8637bd8543875bc2443d01273143bb46655a48a92efa76251adc63ccca6d0e9cef2e0ce93e32b5043bea179a6c710981be4a71703a03e10960643f7df091f499cf60183ef0e4e4e2eebf26e25cea0eebf87c8a6d7f8130c283fc3f747cb90623f0aaa619825e3fcd82f267a0f4bfd26c9f2a6b5a62a6b180b4f6d1d091fce6bd60a9aa9aa5b815b833b44e0f2e58b28a354cb20f52f31bb3b3a7c54f515426537e41f9c20c07e51f9cab8abc311daac19a41bd473a51c7386f014edf1863901a5c29addc89da2f2659c9c1e95affd6997396b9680e317c493e974a813186da277ff9c1d1b30e33cb5a2f6")]
    /// <summary>
    /// 三菱PLC计算机链路协议
    /// </summary>
    public class FxLinks : DisposeBase
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public String PortName { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        public Int32 Baudrate { get; set; } = 9600;

        /// <summary>
        /// 数据位长度.默认7
        /// </summary>
        public Int32 DataBits { get; set; } = 7;

        /// <summary>
        /// 奇偶校验位.默认Even偶校验
        /// </summary>
        public Parity Parity { get; set; } = Parity.Even;

        /// <summary>
        /// 停止位.默认One
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 缓冲区大小.默认256
        /// </summary>
        public Int32 BufferSize { get; set; } = 256;

        /// <summary>
        /// 网络超时.发起请求后等待响应的超时时间，默认3000ms
        /// </summary>
        public Int32 Timeout { get; set; } = 3000;

        /// <summary>
        /// 性能追踪器
        /// </summary>
        public ITracer Tracer { get; set; }

        private SerialPort _port;
        #endregion

        #region 构造
        /// <summary>
        /// 实例化
        /// </summary>
        public FxLinks() => Name = GetType().Name;

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="disposing"> </param>
        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            Close();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            if (_port == null)
            {
                var p = new SerialPort(PortName, Baudrate)
                {
                    DataBits = DataBits,
                    Parity = Parity,
                    StopBits = StopBits,

                    ReadTimeout = Timeout,
                    WriteTimeout = Timeout
                };
                //if (DataBits > 0) p.DataBits = DataBits;
                //if (Parity > 0) p.Parity = Parity;
                //if (StopBits > 0) p.StopBits = StopBits;
                p.Open();
                _port = p;

                WriteLog("FxLinks.Open {0} Baudrate={1} DataBits={2} Parity={3} StopBits={4}", PortName, Baudrate, p.DataBits, p.Parity, p.StopBits);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            _port.TryDispose();
            _port = null;
        }

        /// <summary>
        /// 发送命令，并接收返回
        /// </summary>
        /// <param name="command"> 功能码 </param>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="data">    数据 </param>
        /// <returns> 返回响应消息的负载部分 </returns>
        public virtual FxLinksResponse SendCommand(String command, Byte host, String address, String data)
        {
            var msg = new FxLinksMessage
            {
                Code = ControlCodes.ENQ,
                Station = host,
                PLC = 0xFF,
                Command = command,
                Address = address,

                Payload = data
            };

            var rs = SendCommand(msg);

            return rs;
        }

        /// <summary>
        /// 发送消息并接收返回
        /// </summary>
        /// <param name="message"> Modbus消息 </param>
        /// <returns> </returns>
        internal protected virtual FxLinksResponse SendCommand(FxLinksMessage message)
        {
            Open();

            // 清空缓冲区
            _port.DiscardInBuffer();

            Log?.Debug("=> {0}", message);

            var cmd = message.ToPacket();
            var buf = cmd.ToArray();

            using var span = Tracer?.NewSpan("fxlinks:SendCommand", buf.ToHex("-"));

            Log?.Debug("{0}=> {1} ({2})", PortName, buf.ToHex("-"), FxLinksMessage.GetHex(buf));

            _port.Write(buf, 0, buf.Length);

            //Thread.Sleep(ByteTimeout);

            // 串口速度较慢，等待收完数据
            WaitMore(_port, 1 + 1 + 2);

            //using var span = Tracer?.NewSpan("fxlinks:ReceiveCommand");
            buf = new Byte[BufferSize];
            try
            {
                var count = _port.Read(buf, 0, buf.Length);
                var pk = new Packet(buf, 0, count);
                Log?.Debug("{0}<= {1} ({2})", PortName, pk.ToHex(32, "-"), FxLinksMessage.GetHex(pk.ReadBytes()));

                if (span != null) span.Tag += Environment.NewLine + pk.ToHex(64, "-");

                var len = pk.Total - 2;
                if (len < 2) return null;

                var rs = message.CreateReply();
                if (!rs.Read(pk.GetStream(), message)) return null;

                // 校验
                if (rs.CheckSum != rs.CheckSum2) WriteLog("CheckSum Error {0:X2}!={1:X2} !", rs.CheckSum, rs.CheckSum2);

                Log?.Debug("<= {0}", rs);

                // 检查功能码
                if (rs.Code == ControlCodes.NAK)
                {
                    var str = rs.Payload;
                    var code = str != null && str.Length >= 2 ? str.ToByte(0) : 0;
                    throw new FxLinksException((ErrorCodes)code, $"{message} occure error");
                }

                return rs;
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                if (ex is TimeoutException) return null;
                throw;
            }
        }

        private void WaitMore(SerialPort sp, Int32 minLength)
        {
            var count = sp.BytesToRead;
            if (count >= minLength) return;

            var ms = Timeout;
            var sw = Stopwatch.StartNew();
            while (sp.IsOpen && sw.ElapsedMilliseconds < ms)
            {
                //Thread.SpinWait(1);
                Thread.Sleep(10);
                if (count != sp.BytesToRead)
                {
                    count = sp.BytesToRead;
                    if (count >= minLength) break;

                    sw.Restart();
                }
            }
        }
        #endregion

        #region 读取
        /// <summary>
        /// 按功能码读取.用于IoT标准库
        /// </summary>
        /// <param name="command"> 功能码 </param>
        /// <param name="host">    主机 </param>
        /// <param name="address"> 逻辑地址 </param>
        /// <param name="count">   个数.寄存器个数或线圈个数 </param>
        /// <returns> </returns>
        /// <exception cref="NotSupportedException"> </exception>
        public virtual Object Read(String command, Byte host, String address, Byte count)
        {
            switch (command)
            {
                case "BR": return ReadBit(host, address, count);
                case "WR": return ReadWord(host, address, count);
                default:
                    break;
            }

            throw new NotSupportedException($"FxLinksRead不支持[{command}]");
        }

        /// <summary>
        /// 位单元读取，BR
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="count">   线圈数量.一般要求8的倍数 </param>
        /// <returns> 线圈状态字节数组 </returns>
        public virtual Byte[] ReadBit(Byte host, String address, Byte count)
        {
            using var span = Tracer?.NewSpan("fxlinks:ReadBit", $"host={host} address={address} count={count}");
            try
            {
                var rs = SendCommand("BR", host, address, count.ToHexChars());
                if (rs == null || rs.Payload.IsNullOrEmpty()) return null;

                var result = rs.Payload.ToBytes();

                span?.AppendTag(result.Join(","));

                return result;
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                throw;
            }
        }

        /// <summary>
        /// 字单元读取，WR
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="count">   输入数量.一般要求8的倍数 </param>
        /// <returns> 输入状态字节数组 </returns>
        public virtual UInt16[] ReadWord(Byte host, String address, Byte count)
        {
            using var span = Tracer?.NewSpan("fxlinks:ReadWord", $"host={host} address={address} count={count}");
            try
            {
                var rs = SendCommand("WR", host, address, count.ToHexChars());
                if (rs == null || rs.Payload.IsNullOrEmpty()) return null;

                var str = rs.Payload;
                var us = new UInt16[str.Length / 4];
                for (var i = 0; i < us.Length; i++)
                {
                    us[i] = str.Substring(i * 4, 4).ToHex().ToUInt16(0, false);
                }

                span?.AppendTag(us);

                return us;
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                throw;
            }
        }
        #endregion

        #region 写入
        /// <summary>
        /// 按功能码写入.用于IoT标准库
        /// </summary>
        /// <param name="command"> 功能码 </param>
        /// <param name="host">    主机 </param>
        /// <param name="address"> 逻辑地址 </param>
        /// <param name="values">  待写入数值 </param>
        /// <returns> </returns>
        public virtual Object Write(String command, Byte host, String address, UInt16[] values)
        {
            switch (command)
            {
                case "BW": return WriteBit(host, address, values);
                case "WW": return WriteWord(host, address, values);
                default:
                    break;
            }

            throw new NotSupportedException($"FxLinksWrite不支持[{command}]");
        }

        /// <summary>
        /// 位单元写入，BW
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="values">  输出值.一般是 0xFF00/0x0000 </param>
        /// <returns> 输出值 </returns>
        public Int32 WriteBit(Byte host, String address, params UInt16[] values)
        {
            using var span = Tracer?.NewSpan("fxlinks:WriteBit", $"host={host} address={address} value=({values.Join(",")})");
            try
            {
                // 1字节（2字符）的点位数 后续每个点位1个字符
                var sb = new StringBuilder();
                sb.Append(((Byte)values.Length).ToHexChars());
                for (var i = 0; i < values.Length; i++)
                {
                    sb.Append(values[i] != 0 ? '1' : '0');
                }

                var rs = SendCommand("BW", host, address, sb.ToString());
                if (rs == null) return -1;
                if (rs.Code == ControlCodes.NAK) throw new Exception($"WriteBit({address}, {values.Join(",")}) get {rs.Code}");
                if (rs.Code != ControlCodes.ACK) return -1;

                return values.Length;
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                throw;
            }
        }

        /// <summary>
        /// 字单元写入，WW
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="values">  数值 </param>
        /// <returns> 寄存器值 </returns>
        public Int32 WriteWord(Byte host, String address, params UInt16[] values)
        {
            using var span = Tracer?.NewSpan("fxlinks:WriteWord", $"host={host} address={address} value=({values.Join(",")})");
            try
            {
                // 1字节（2字符）的点位数 后续每个点位4个字符
                var sb = new StringBuilder();
                sb.Append(((Byte)values.Length).ToHexChars());
                for (var i = 0; i < values.Length; i++)
                {
                    sb.Append(values[i].ToString("X4"));
                }

                var rs = SendCommand("WW", host, address, sb.ToString());
                if (rs == null) return -1;
                if (rs.Code == ControlCodes.NAK) throw new Exception($"WriteWord({address}, {values.Join(",")}) get {rs.Code}");
                if (rs.Code != ControlCodes.ACK) return -1;

                return values.Length;
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                throw;
            }
        }
        #endregion

        #region 日志
        /// <summary>
        /// 日志
        /// </summary>
        public ILog Log { get; set; }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="format"> </param>
        /// <param name="args">   </param>
        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
        #endregion
    }

    /// <summary>
    /// FxLinks异常
    /// </summary>
    public class FxLinksException : Exception
    {
        /// <summary>
        /// 异常代码
        /// </summary>
        public ErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// 实例化异常
        /// </summary>
        /// <param name="errorCode"> </param>
        /// <param name="message">   </param>
        public FxLinksException(ErrorCodes errorCode, String message) : base(message) => ErrorCode = errorCode;
    }

    /// <summary>
    /// 三菱FxLinks消息
    /// </summary>
    /// <remarks> 功能码： ENQ 05 STX 02 ETX 03 NAK H15 </remarks>
    public class FxLinksMessage : IAccessor
    {
        #region 属性
        /// <summary>
        /// 控制码.02/03/05/15
        /// </summary>
        public ControlCodes Code { get; set; }

        /// <summary>
        /// 站号
        /// </summary>
        public Byte Station { get; set; }

        /// <summary>
        /// PLC号
        /// </summary>
        public Byte PLC { get; set; }

        /// <summary>
        /// 操作码
        /// </summary>
        public String Command { get; set; }

        /// <summary>
        /// 等待字符
        /// </summary>
        public Byte Wait { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// 负载数据
        /// </summary>
        public String Payload { get; set; }

        /// <summary>
        /// 校验和.读取出来
        /// </summary>
        public Byte CheckSum { get; set; }

        /// <summary>
        /// 校验和.计算出来
        /// </summary>
        public Byte CheckSum2 { get; set; }
        #endregion

        #region 构造
        /// <summary>
        /// 已重载.友好字符串
        /// </summary>
        /// <returns> </returns>
        public override String ToString()
        {
            if (Code == ControlCodes.ENQ)
                return $"{Command} ({Address}, {Payload})";
            else
                return $"{Code} ({Payload})";
        }
        #endregion

        #region 方法
        const Int32 HEADER05 = 2 + 2 + 2 + 1 + 5 + 2;

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        /// <returns> </returns>
        public virtual Boolean Read(Stream stream, Object context)
        {
            Code = (ControlCodes)stream.ReadByte();
            switch (Code)
            {
                case ControlCodes.ENQ:
                {
                    // 05FFWR0D02100132
                    var hex = stream.ReadBytes(-1).ToStr();

                    Station = hex.ToByte(0);
                    PLC = hex.ToByte(2);

                    Command = hex[4..6];
                    Wait = Convert.ToByte(hex[6..7], 16);

                    // 注意点位Y0
                    Address = hex[7] + hex[8..12].TrimStart('0');
                    if (Address.Length == 1) Address += '0';

                    var len = hex.Length - HEADER05;
                    if (len > 0)
                    {
                        Payload = hex.Substring(12, len);
                        //var str = hex.Substring(12, len);
                        //if (Command == "BW")
                        //    Payload = str.ToArray().Select(e => Convert.ToByte(e + "", 16)).ToArray();
                        //else
                        //    Payload = str.ToHex();
                    }

                    CheckSum = hex[^2..].ToByte(0);
                    CheckSum2 = (Byte)hex.ToArray().Take(hex.Length - 2).Sum(e => e);

                    break;
                }

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 写入消息到数据流
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        /// <returns> </returns>
        public virtual Boolean Write(Stream stream, Object context)
        {
            stream.Write((Byte)Code);

            switch (Code)
            {
                case ControlCodes.ENQ:
                {
                    // 05FFWR0D02100132
                    var sb = new StringBuilder(64);

                    sb.Append(Station.ToString("X2"));
                    sb.Append(PLC.ToString("X2"));
                    sb.Append(Command);
                    sb.Append(Wait.ToString("X"));

                    var addr = Address[0] + Address[1..].PadLeft(4, '0');
                    sb.Append(addr);

                    var pk = Payload;
                    if (pk != null)
                    {
                        //var buf = pk.ReadBytes();
                        //for (var i = 0; i < buf.Length; i++)
                        //{
                        //    sb.Append((Char)buf[i]);
                        //}
                        sb.Append(pk);

                        //if (Command == "BW")
                        //{
                        //    var buf = pk.ReadBytes();
                        //    for (var i = 0; i < buf.Length; i++)
                        //    {
                        //        //sb.Append(Convert.ToString(buf[i], 16));
                        //        sb.Append((Char)buf[i]);
                        //    }
                        //}
                        //else
                        //    sb.Append(pk.ToHex(256));
                    }

                    var sum = 0;
                    for (var i = 0; i < sb.Length; i++)
                    {
                        sum += sb[i];
                    }
                    CheckSum2 = (Byte)sum;
                    sb.Append(CheckSum2.ToString("X2"));

                    var hex = sb.ToString();
                    stream.Write(hex.GetBytes());

                    break;
                }

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 消息转数据包
        /// </summary>
        /// <returns> </returns>
        public Packet ToPacket()
        {
            var ms = new MemoryStream();
            Write(ms, null);

            ms.Position = 0;
            return new Packet(ms);
        }

        /// <summary>
        /// 创建响应
        /// </summary>
        /// <returns> </returns>
        /// <exception cref="InvalidOperationException"> </exception>
        public virtual FxLinksResponse CreateReply()
        {
            var msg = new FxLinksResponse
            {
                Station = Station,
                PLC = PLC,
                Command = Command,
            };

            return msg;
        }

        /// <summary>
        /// 获取指令的HEX字符串形式
        /// </summary>
        /// <param name="msg"> </param>
        /// <returns> </returns>
        public static String GetHex(Byte[] msg)
        {
            if (msg == null || msg.Length == 0) return null;

            var str = msg.ToStr();

            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                if (ch == 0x02)
                    sb.Append("STX-");
                else if (ch == 0x03)
                    sb.Append("-ETX-");
                else if (ch == 0x05)
                    sb.Append("ENQ-");
                else if (ch == 0x06)
                    sb.Append("ACK-");
                else if (ch == 0x15)
                    sb.Append("NAK-");
                else
                    sb.Append(ch);
            }

            return sb.ToString();
        }
        #endregion
    }

    /// <summary>
    /// 三菱FxLinks响应
    /// </summary>
    /// <remarks> 功能码： ENQ 05 STX 02 ETX 03 NAK H15 </remarks>
    public class FxLinksResponse : IAccessor
    {
        #region 属性
        /// <summary>
        /// 控制码.02/03/05/15
        /// </summary>
        public ControlCodes Code { get; set; }

        /// <summary>
        /// 站号
        /// </summary>
        public Byte Station { get; set; }

        /// <summary>
        /// PLC号
        /// </summary>
        public Byte PLC { get; set; }

        /// <summary>
        /// 操作码.解析数据前设置，仅用于判断如何解析数据
        /// </summary>
        public String Command { get; set; }

        /// <summary>
        /// 负载数据
        /// </summary>
        public String Payload { get; set; }

        /// <summary>
        /// 校验和.读取出来
        /// </summary>
        public Byte CheckSum { get; set; }

        /// <summary>
        /// 校验和.计算出来
        /// </summary>
        public Byte CheckSum2 { get; set; }
        #endregion

        #region 构造
        /// <summary>
        /// 已重载.友好字符串
        /// </summary>
        /// <returns> </returns>
        public override String ToString()
        {
            //if (Code == ControlCodes.STX)
            //    return $"{Command} ({Payload?.ToHex()})";
            //else
            return $"{Code} ({Payload})";
        }
        #endregion

        #region 方法
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        /// <returns> </returns>
        public virtual Boolean Read(Stream stream, Object context)
        {
            Code = (ControlCodes)stream.ReadByte();
            switch (Code)
            {
                case ControlCodes.STX:
                {
                    // 05FF0001\03B5 STX-05FF0-ETX-24
                    var p = stream.Position;
                    var hex = stream.ReadBytes(4).ToStr();

                    Station = hex.ToByte(0);
                    PLC = hex.ToByte(2);

                    var retain = (Int32)(stream.Length - stream.Position);
                    if (retain < 3) return false;

                    var len = retain - 3;
                    if (len > 0)
                    {
                        Payload = stream.ReadBytes(len).ToStr();
                        //var str = stream.ReadBytes(len).ToStr();
                        //// 位读取时，每个点位占1个字符；字读取时，每个点位占4个字符
                        //if (len == 1)
                        //    Payload = new Byte[] { Convert.ToByte(str, 16) };
                        //else if (Command == "BR")
                        //    Payload = str.ToArray().Select(e => Convert.ToByte(e + "", 16)).ToArray();
                        //else
                        //    Payload = str.ToHex();
                    }

                    var b = (ControlCodes)stream.ReadByte();
                    if (b != ControlCodes.ETX) return false;

                    len = (Int32)(stream.Position - p);
                    stream.Position = p;
                    CheckSum2 = (Byte)stream.ReadBytes(len).Sum(e => e);

                    CheckSum = stream.ReadBytes(2).ToStr().ToByte(0);
                    break;
                }

                case ControlCodes.ETX:
                    return false;
                case ControlCodes.ACK:
                case ControlCodes.NAK:
                {
                    // 05FF
                    var hex = stream.ReadBytes(-1).ToStr();

                    Station = hex.ToByte(0);
                    PLC = hex.ToByte(2);

                    // 如果还有数据，作为负载.一般ACK后面没有了，而NAK后面有错误码
                    if (hex.Length > 4) Payload = hex[4..];

                    break;
                }

                case ControlCodes.EOT:
                    return false;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 写入消息到数据流
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        /// <returns> </returns>
        public virtual Boolean Write(Stream stream, Object context)
        {
            stream.Write((Byte)Code);

            switch (Code)
            {
                case ControlCodes.STX:
                {
                    // 05FF0001\03B5
                    var sb = new StringBuilder(64);

                    sb.Append(Station.ToString("X2"));
                    sb.Append(PLC.ToString("X2"));

                    var pk = Payload;
                    if (pk != null)
                    {
                        sb.Append(pk);
                        //// 位读取时，每个点位占1个字符；字读取时，每个点位占4个字符
                        //if (pk.Total == 1)
                        //    sb.Append(pk[0].ToString("X"));
                        //else if (Command == "BR")
                        //{
                        //    var buf = pk.ReadBytes();
                        //    //for (var i = 0; i < buf.Length; i++)
                        //    //{
                        //    //    sb.Append(Convert.ToString(buf[i], 16));
                        //    //}
                        //    sb.Append(buf.ToHexString());
                        //}
                        //else
                        //    sb.Append(pk.ToHex(256));
                    }

                    sb.Append((Char)ControlCodes.ETX);

                    var sum = 0;
                    for (var i = 0; i < sb.Length; i++)
                    {
                        sum += sb[i];
                    }
                    CheckSum2 = (Byte)sum;
                    sb.Append(CheckSum2.ToString("X2"));

                    var hex = sb.ToString();
                    stream.Write(hex.GetBytes());

                    break;
                }

                case ControlCodes.ETX:
                case ControlCodes.EOT:
                    return false;

                case ControlCodes.ACK:
                case ControlCodes.NAK:
                {
                    // 05FF
                    var sb = new StringBuilder(64);

                    sb.Append(Station.ToString("X2"));
                    sb.Append(PLC.ToString("X2"));

                    var hex = sb.ToString();
                    stream.Write(hex.GetBytes());

                    break;
                }

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 消息转数据包
        /// </summary>
        /// <returns> </returns>
        public Packet ToPacket()
        {
            var ms = new MemoryStream();
            Write(ms, null);

            ms.Position = 0;
            return new Packet(ms);
        }
        #endregion
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public enum NetworkErrorCodes
    {
        /// <summary>
        /// 命令超时
        /// </summary>
        /// <remarks> After master station send request to save station, no answer passing comms time-out. </remarks>
        CommsTimeout = 0x01,

        /// <summary>
        /// 站号错误
        /// </summary>
        /// <remarks> Station No. is not agreement between the master station and the slave station. </remarks>
        StationError = 0x02,

        /// <summary>
        /// 通信计数器错误
        /// </summary>
        /// <remarks> Communication counter is not agreement between the master station and the slave station. </remarks>
        CommsCounterError = 0x03,

        /// <summary>
        /// 通信格式错误
        /// </summary>
        /// <remarks> Communication format is not right from slave station. </remarks>
        CommsFormatError = 0x04,

        /// <summary>
        /// 主机通信超时
        /// </summary>
        /// <remarks> After slave station send answer to master station, master station do not send request to next slave station. </remarks>
        MasterCommsTimeoutError = 0x11,

        /// <summary>
        /// 主机通信格式错误
        /// </summary>
        /// <remarks> Communication format is not right from master station. </remarks>
        MasterCommsFormatError = 0x14,

        /// <summary>
        /// 从机不存在
        /// </summary>
        /// <remarks> The station No. is not in this network. </remarks>
        NoSlave = 0x21,

        /// <summary>
        /// 站号错误
        /// </summary>
        /// <remarks> Station No. is not agreement between the master station and the slave station. </remarks>
        StationError2 = 0x22,

        /// <summary>
        /// 通信计数器错误
        /// </summary>
        /// <remarks> Communication counter is not agreement between the master station and the slave station. </remarks>
        CommsCounterError2 = 0x23,

        /// <summary>
        /// 未收到通信参数
        /// </summary>
        /// <remarks> When slave station receive request from master station before communication parameter. </remarks>
        NotReceiveCommsParameter = 0x31,
    }
}


