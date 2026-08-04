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

using Feature.Data;
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
using Feature.Log;
using Feature.LoRa.Security;
using Feature.Serialization;
using Feature;
using System.ComponentModel;
using System.IO;
using System.Text;
using System;

// LoRa服务器
namespace Feature.LoRa.Drivers
{
    using Feature.Agent;
    using Feature.Data;
    using Feature.Log;
    using Feature.LoRa.Messaging;
    using Feature.LoRa.Models;
    using Feature.LoRa.Security;
    using Feature.LoRa;
    using Feature.Serialization;
    using Feature.Xml;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;

    class Program
    {
        static void Main(String[] args) => new MyService().Main(args);

        // LoRa服务器
        class MyService : ServiceBase
        {
            public MyService()
            {
                ServiceName = "LoRaServer";
                DisplayName = "LoRa服务器";

                AddMenu('t', "测试数据", Test);
                AddMenu('s', "测试加密", Test2);
                AddMenu('d', "测试解密", Test3);
            }

            private LoRaServer _Server;
            public override void StartWork(String reason)
            {
                // 配置
                var set = Setting.Current;

                // 服务器
                var svr = new LoRaServer()
                {
                    Port = set.Port,
                    Log = XTrace.Log,
                };

                if (set.Debug) svr.SessionLog = XTrace.Log;

                svr.Start();

                _Server = svr;

                base.StartWork(reason);
            }

            public override void StopWork(String reason)
            {
                _Server.TryDispose();
                _Server = null;

                base.StopWork(reason);
            }

            private void Test()
            {
                //var str = \"{\\"stat\\":{\\"time\\":\\"2019-10-25 07:05:33 UTC\\",\\"lati\\":\\"31.231013\\",\\"long\\":\\"121.200607\\",\\"alti\\":\\"30.200000\\",\\"rxnb\\":0,\\"rxok\\":0,\\"rxfw\\":0,\\"ackr\\":100.0,\\"dwnb\\":0,\\"txnb\\":0,\\"batt\\":0,\\"poe\\":0,\\"net\\":1,\\"traffic\\":780539002,\\"ver\\":\\"V3.0.864.862.868_Release\\"}}\";
                //var str = "{\"rxpk\":[{\"tmst\":196287580,\"chan\":5,\"rfch\":1,\"freq\":474.100000,\"stat\":1,\"modu\":\"LORA\",\"datr\":\"SF12BW125\",\"codr\":\"4/5\",\"lsnr\":-12.5,\"rssi\":-124,\"size\":50,\"data\":\"gHMAEHCADwsB3P7NADg1Rsj2FLImBtz/9e3hVNzniwoMGUhlyC4KI8Lsvt1VKSuSyVM=\"}]}";
                var str = "{\"rxpk\":[{\"tmst\":438505452,\"chan\":2,\"rfch\":0,\"freq\":473.500000,\"stat\":1,\"modu\":\"LORA\",\"datr\":\"SF12BW125\",\"codr\":\"4/5\",\"lsnr\":-14.0,\"rssi\":-119,\"size\":50,\"data\":\"gGEAEHCACwABRaWG3UZsomFqt4sxJmt0JGNFCS3PWweysY1Vi+94PmFTmoycmDxCviA=\"}]}";
                //var str = "{\"txpk\":{\"imme\":true,\"freq\":864.123456,\"rfch\":0,\"powe\":14,\"modu\":\"LORA\",\"datr\":\"SF11BW125\",\"codr\":\"4/6\",\"ipol\":false,\"size\":32,\"data\":\"H3P3N2i9qc4yt7rK7ldqoeCVJGBybzPY5h1Dd7P7p8v\"}}";
                var js = new JsonParser(str).Decode() as IDictionary<String, Object>;
                //var st = JsonHelper.Convert<StatModel>(js[\"stat\"]);

                //Console.WriteLine(st.ToJson(true));

                var st = StatModel.Read(js["stat"]);
                if (st != null) Console.WriteLine(st.ToJson(true));

                var dt = RxPacket.Read(js["rxpk"]);
                if (dt.Length > 0)
                {
                    Console.WriteLine(dt.ToJson(true));

                    var dp = dt[0];
                    if (!dp.Data.IsNullOrEmpty())
                    {
                        Packet pk = dp.Data.ToBase64();
                        Console.WriteLine(pk.ToHex(64));
                        Console.WriteLine(pk.ToStr());

                        var pm = new PHYMessage();
                        pm.Read(pk.GetStream(), null);

                        Console.WriteLine(pm.ToJson(true));

                        var nwkSkey = "4B463EFED018F099FE3F05108618FDDA".ToHex();
                        //var appSkey = "19E52095515EBD0C2FD596DD96FD0833".ToHex();
                        var appSkey = "778960777F7B4CBAC857C06DEE818844".ToHex();
                        var buf = pm.Decrypt(nwkSkey, appSkey);
                        Console.WriteLine(buf.ToHex());
                        Console.WriteLine(buf.ToStr());
                    }
                }

                var tx = TxPacket.Read(js["txpk"]);
                if (tx != null) Console.WriteLine(tx.ToJson(true));
            }

            private void Test2()
            {
                var ss = new[] {
            "gGMAEHCAGAcB0kSc/G+ehD4Z93QxnfV6P0i0dO2SSrCsl5ZdXI8rRQzT3W3Ej1mHTO8=",
            "gLcAEHCANQ4BsLzPNq3dJKRyAtgGjfP0kyQgN4RWzRAcO0LPfU1J0L6VszQJGwANg7Y=",
            "gFQAEHCAWgMBpcwy1MAkLGFPRII9hUlB4+3+O1d0p2ZOgsHu21BAikAOYtjTh60Dx+s=",
            "gHMAEHCAEAsBcdf9lNe4FL+Hvs8lUNpie7u10tSOAWqiPyQVyLTx6BbQDWehC6qQa+0=",
            "gHMAEHCADwsB3P7NADg1Rsj2FLImBtz/9e3hVNzniwoMGUhlyC4KI8Lsvt1VKSuSyVM=",
            "gF0AEHCATgQBdf0x8gNiz9fqC13IfE79yqd4SyMtTDyk02gQoW317HwJ6L1zt8rXAIc=",
            "gF0AEHCATwQB/F5MgTRSRaqVjS9SZjt1rQYdPtl4hSL2Tox4Y8TRW4yatCH/7l75/Q4=",
            };
                var dic = new Dictionary<String, String>
                {
                    ["701000B7"] = "5F6C965F3AA482AF2EF8C3FBF63661FE",
                    ["70100063"] = "1BA6731021ED686C3643756311DD23CC",
                    ["701000A4"] = "ED347BE6FDDF2BCF749354694285841D",
                    ["7010005D"] = "8598B09A8CD56BC67AA55C08CEDC183E",
                    ["70100073"] = "19E52095515EBD0C2FD596DD96FD0833",
                    ["70100054"] = "53BBDC505119EB63BCB17CD15B24AD45",
                    ["70100061"] = "778960777F7B4CBAC857C06DEE818844",
                    ["701000B1"] = "35D43942B95DC82B80A79BC4BAD9457E",
                };

                foreach (var item in ss)
                {
                    Console.WriteLine();
                    Packet pk = item.ToBase64();

                    var pm = new PHYMessage();
                    pm.Read(pk.GetStream(), null);
                    Console.WriteLine(pm.Payload.ToHex(64));

                    var addr = pm.DevAddr.ToString("X8");
                    //Console.WriteLine(pm.Type);
                    Console.WriteLine("{0} {1} FCnt={2} FPort={3}", addr, pm.Type, pm.FCnt, pm.FPort);
                    //Console.WriteLine(pm.ToJson(true));

                    //var nwkSkey = "4B463EFED018F099FE3F05108618FDDA".ToHex();
                    //var appSkey = "19E52095515EBD0C2FD596DD96FD0833".ToHex();
                    var appSkey = dic[addr].ToHex();
                    var buf = pm.Decrypt(null, appSkey);
                    Console.WriteLine(buf.ToHex());
                    //Console.WriteLine(buf.ToStr());
                }

                //var crypto = new LoRaMacCrypto();

                //var buf = "C86B3BF3".ToHex();
                //var key = "A499E0B73311D0782EC80C98FEC83B8E".ToHex();
                //var rs = crypto.PayloadDecrypt(buf, key, 0x77F7EEF0, true, 0x20);

                //var str = rs.ToHex();
                //XTrace.WriteLine(str);
                //Debug.Assert(str == "B93747B2");
            }

            private void Test3()
            {
                var crypto = new LoRaMacCrypto();

                //var buf = "C86B3BF3".ToHex();
                var buf = "58E1369B".ToHex();
                var key = "A499E0B73311D0782EC80C98FEC83B8E".ToHex();
                var rs = crypto.PayloadDecrypt(buf, key, 0x77F7EEF0, true, 0x0142);

                var str = rs.ToHex();
                XTrace.WriteLine(str);
                //Debug.Assert(str == "B93747B2");
                Debug.Assert(str == "092200DB");
            }
        }
    }

    [XmlConfigFile(@"Config\LoRa.config", 10_000)]
    public class Setting : XmlConfig<Setting>
    {
        /// <summary>
        /// 调试开关.默认 false
        /// </summary>
        [Description("调试开关.默认 false")]
        public Boolean Debug { get; set; }

        /// <summary>
        /// 端口.默认 1680
        /// </summary>
        [Description("端口.默认 1680")]
        public Int32 Port { get; set; } = 1680;
    }
}

// LoRa消息
namespace Feature.LoRa.Messaging
{
    /// <summary>
    /// 数据消息
    /// </summary>
    public class DataMessage
    {
        #region 数据包格式
        // 字节数 Preamble PHDR PHDR_CRC PHYPayload CRC
        // PHYPayload: MHDR(1 byte) MACPayload MIC(4 byte)
        // MACPayload: FHDR FPort(0 or 1 byte) FRMPayload(n byte)
        // FHDR: DevAddr(4 byte) FCtrl(1 byte) FCnt(2 byte) FOpts(n byte)

        // 数据包展开看 去掉硬件部分，数据如下。 MHDR(1 byte) DevAddr(4 byte) FCtrl(1 byte) FCnt(2 byte) CmdPlayLoad(0 - 15 byte) FPort(0 or 1 byte) FRMPayload(n byte) MIC(4 byte)

        // 消息另外一种形式 没有 FRMPayload 时，FCtrl 中 FOptsLen = 0 ， FPort = 0 MHDR(1 byte) DevAddr(4 byte) FCtrl(1 byte) FCnt(2 byte) FPort(0 or 1 byte) CmdPlayLoad(0 - 15 byte) MIC(4 byte)

        // 无负载数据的消息 有校验，无加密 MHDR(1 byte) DevAddr(4 byte) FCtrl(1 byte) FCnt(2 byte) MIC(4 byte)

        // 校验数据是 除 MIC 之外的所有， 使用 NwkSkey 加密数据 一定是 （FPort ， MIC） 之间的数据。 FPort = 0时使用NwkSkey 其余使用 AppSKey

        // MSB 高位在前描述
        // MHDR: MType(3bit) + RFU(3bit) + Major(2bit)
        // FCtrl: ADR(1bit) + ADRACKReq(1bit) + ACK(1bit) + RFU(1bit) or FPending(1bit) + FOptsLen(4bit)
        #endregion

        #region 数据包头部内容

        /// <summary>
        /// 设备地址
        /// </summary>
        public Int32 DevAddr { get; set; } = 0;

        /// <summary>
        /// ADR （速率自适应）标志，标识服务器是否可以修改节点的传输速率
        /// </summary>
        public Boolean ADR = false;

        /// <summary>
        /// 节点请求服务器修改服务器发送的传输参数。表示节点网络质量差，有重传超限，需要 ADR 介入
        /// </summary>
        /// <remarks> 节点没有收到有效数据时AdrAckCounter++，当AdrAckCounter &gt;= CN470_ADR_ACK_LIMIT，节点将Txpower提到最高 当AdrAckCounter &gt;= ( CN470_ADR_ACK_LIMIT + CN470_ADR_ACK_DELAY )时，节点用大DR发送数据 </remarks>
        public Boolean ADRACKReq = false;

        /// <summary>
        /// 数据ACK位，响应confirmed数据用
        /// </summary>
        public Boolean ACK = false;

        /// <summary>
        /// 保留位
        /// </summary>
        public Boolean RfuFlag = false;

        /// <summary>帧挂起位。下行数据时使用，表示网关还有数据需要下发，让节点尽快上行数据。（Class A&B 只能节点发起通讯）</summary>
        public Boolean FPending = false;

        private Byte _FOptsLen = 0;
        /// <summary>
        /// 扩展项长度，扩展项是MAC命令，最长15字节
        /// </summary>
        public Byte FOptsLen
        {
            get => _FOptsLen;
            set
            {
                if (value > 0x0f) return;
                _FOptsLen = value;
            }
        }

        /// <summary>
        /// FCtrl
        /// </summary>
        private Byte FCtrl
        {
            get
            {
                var rs = _FOptsLen;
                if (ADR) rs |= 0x80;
                if (ADRACKReq) rs |= 0x40;
                if (ACK) rs |= 0x20;

                switch (MTpye)
                {
                    case MType_e.UnconfirmedDataUp:
                    case MType_e.ConfirmedDataUp:
                        if (RfuFlag) rs |= 0x10;
                        break;
                    case MType_e.UnconfirmedDataDown:
                    case MType_e.ConfirmedDataDown:
                        if (FPending) rs |= 0x10;
                        break;

                    case MType_e.JoinRequest: break;
                    case MType_e.JoinAccept: break;
                    case MType_e.RFU: break;
                    case MType_e.Proprietary: break;
                    default: break;
                }

                return rs;
            }
            set
            {
                _FOptsLen = (Byte)(value & 0x0f);

                ADR = (value & 0x80) == 0x80;
                ADRACKReq = (value & 0x40) == 0x40;
                ACK = (value & 0x20) == 0x20;

                switch (MTpye)
                {
                    case MType_e.UnconfirmedDataUp:
                    case MType_e.ConfirmedDataUp:
                        RfuFlag = (value & 0x10) == 0x10;
                        break;
                    case MType_e.UnconfirmedDataDown:
                    case MType_e.ConfirmedDataDown:
                        FPending = (value & 0x10) == 0x10;
                        break;

                    case MType_e.JoinRequest: break;
                    case MType_e.JoinAccept: break;
                    case MType_e.RFU: break;
                    case MType_e.Proprietary: break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// FCnt
        /// </summary>
        public UInt16 FCnt = 0x00;

        private Byte _FPort = 0;
        /// <summary>
        /// 如果有效载荷不为0，则FPort必须存在。 0-223 是可用段，224 225保留用于以后扩展。
        /// </summary>
        public Byte FPort
        {
            get => _FPort;
            set
            {
                if (value < 0) { XTrace.WriteLine("FPort 超出范围"); return; }
                if (value > 255) { XTrace.WriteLine("FPort 超出范围"); return; }
                if (value > 233) XTrace.WriteLine("注意，这是保留端口");
                _FPort = value;
            }
        }

        #endregion

        #region 负载数据

        /// <summary>
        /// 正儿八经的负载数据
        /// </summary>
        public Byte[] Payload;

        /// <summary>
        /// 扩展项内容，MAC命令数据
        /// </summary>
        public Byte[] CmdPayload;

        #endregion

        #region 秘钥

        /// <summary>
        /// 网络秘钥 16字节
        /// </summary>
        public Byte[] NwkSKey { get; set; } = null;

        /// <summary>
        /// 应用秘钥 16字节
        /// </summary>
        public Byte[] AppSKey { get; set; } = null;

        #endregion

        #region 方法

        /// <summary>
        /// 从字节数组读取消息内容
        /// </summary>
        /// <param name="bs"> </param>
        public void Read(Byte[] bs)
        {
            if (bs.Length < 12)
            {
                XTrace.WriteLine("消息长度错误");
                Valid = false;
                return;
            }

            Buffer = bs;

            // XTrace.WriteLine("DataMessage {0}", bs.ToHex());
            var st = new MemoryStream(bs);

            MHDR = (Byte)st.ReadByte();
            DevAddr = (Int32)st.ReadBytes(4).ToUInt32();
            FCtrl = (Byte)st.ReadByte();
            FCnt = st.ReadBytes(2).ToUInt16();

            // 最后4字节是MIC 直接提取出来，比Steam里面拿简单暴力有效。
            MIC = bs.ReadBytes(bs.Length - 4, 4).ToUInt32();

            // 附带数据
            if (FOptsLen != 0)
            {
                CmdPayload = st.ReadBytes(FOptsLen);
                // XTrace.WriteLine("cmdpayload " + CmdPayload.ToHex());
            }

            var remian = st.Capacity - st.Position;
            // Fport(1byte) + 负载(n byte) + MIC(4 byte)
            if (remian > 4)
            {
                FPort = (Byte)st.ReadByte();

                // 除了尾巴MIC 之外都是负载数据
                var buf = st.ReadBytes();
                var data = buf.ReadBytes(0, buf.Length - 4);

                // XTrace.WriteLine("FPort " + FPort + " remian " + (remian - 5) + " " + data.ToHex());

                if (FPort == 0)
                    CmdPayload = data;
                else
                    Payload = data;
            }

        }

        /// <summary>
        /// 序列化包
        /// </summary>
        /// <returns> </returns>
        public Byte[] ToArray()
        {
            if (Buffer == null) Build();

            return Buffer;
        }

        /// <summary>
        /// 创建消息
        /// </summary>
        public void Build()
        {
            if ((NwkSKey == null) || (NwkSKey.Length != 16)) XTrace.WriteLine("NwkSkey 不满足要求");
            if ((AppSKey == null) || (AppSKey.Length != 16)) XTrace.WriteLine("AppSKey 不满足要求");

            // 加密数据
            Encrypt(NwkSKey, AppSKey);

            var ms = new MemoryStream();

            ms.Write(MHDR);
            ms.Write(DevAddr.GetBytes());

            // CmdPayload 放到 FPort=0 后面。
            var cmdaffterFport = false;
            // 没有 CmdPayload 长度一定是 0
            if ((CmdPayload == null) || (CmdPayload.Length == 0))
            {
                FOptsLen = 0;
            }
            // 只有 CmdPayload 没有 Payload 长度也是0
            else
            {
                FOptsLen = (Byte)CmdPayload.Length;
                if ((Payload == null) || (Payload.Length == 0))
                {
                    FOptsLen = 0;
                    FPort = 0;
                    cmdaffterFport = true;
                }
            }

            // XTrace.WriteLine("FCtrl {0}", FCtrl.ToString("X2"));
            ms.Write(FCtrl);
            ms.Write(FCnt.GetBytes());

            if (cmdaffterFport)
            {
                ms.Write(FPort);
                ms.Write(CmdPayload);
            }
            else
            {
                if (CmdPayload != null) ms.Write(CmdPayload);

                if (Payload != null)
                {
                    ms.Write(FPort);
                    ms.Write(Payload);
                }
            }
            // 创建的消息一定是下行的！
            LoRaMacCrypto.LoRaMacComputeMic(ms.ToArray(), NwkSKey, DevAddr, false, FCnt, ref MIC);

            ms.Write(MIC.GetBytes());

            Buffer = ms.ToArray();
        }

        /// <summary>
        /// 使用key去校验数据合法性
        /// </summary>
        /// <param name="nwkSkey"> AppEui对应的key </param>
        /// <returns> 校验结果 </returns>
        public Boolean ValidationData(Byte[] nwkSkey)
        {
            if (!Valid) return false;

            if (nwkSkey == null) new NullReferenceException("nwkSkey Error");

            var data = Buffer.ReadBytes(0, Buffer.Length - 4);

            UInt32 mic = 0;
            LoRaMacCrypto.LoRaMacComputeMic(data, nwkSkey, DevAddr, true, FCnt, ref mic);
            if (mic == MIC) return true;

            XTrace.WriteLine("数据包校验失败，MIC 计算值 {0} 包内值 {1} ", mic.GetBytes().ToHex(), MIC.GetBytes().ToHex());
            return false;
        }

        /// <summary>
        /// 解密信息
        /// </summary>
        /// <param name="key"> </param>
        public void Decrypt(Byte[] nwkSkey = null, Byte[] appSkey = null)
        {
            // XTrace.WriteLine("DataMessage Decrypt");
            if (nwkSkey == null) new NullReferenceException("nwkSkey Error");
            if (appSkey == null) new NullReferenceException("appSkey Error");

            // 没有数据需要解密
            if (((CmdPayload == null) || (CmdPayload.Length == 0)) && ((Payload == null) || (Payload.Length == 0))) return;

            // 没有 Payload ，有 CmdPayload 的时候 FPort 一定等于 0 if ((FPort == 0)&& ((Payload == null) || (Payload.Length == 0)))
            if (((Payload == null) || (Payload.Length == 0)))
            {
                // XTrace.WriteLine("Decrypt 1");
                var data = new Byte[CmdPayload.Length];
                LoRaMacCrypto.LoRaMacPayloadDecrypt(CmdPayload, nwkSkey, DevAddr, true, FCnt, data);
                CmdPayload = data;

                // XTrace.WriteLine("明文数据 CmdPayload " + CmdPayload.ToHex());
            }
            else
            {
                // XTrace.WriteLine("Decrypt 2");
                var data = new Byte[Payload.Length];
                LoRaMacCrypto.LoRaMacPayloadDecrypt(Payload, appSkey, DevAddr, true, FCnt, data);
                Payload = data;

                // XTrace.WriteLine("明文数据 Payload " + "FPort " + FPort + " : " + Payload.ToHex());
            }

            NwkSKey = nwkSkey;
            AppSKey = appSkey;
        }

        /// <summary>
        /// 加密信息
        /// </summary>
        /// <param name="key"> </param>
        public void Encrypt(Byte[] nwkSkey, Byte[] appSkey)
        {
            // XTrace.WriteLine("DataMessage Encrypt"); 没有数据需要解密
            if (((CmdPayload == null) || (CmdPayload.Length == 0)) && ((Payload == null) || (Payload.Length == 0))) return;

            // 没有 Payload ，有 CmdPayload 的时候 FPort 一定等于 0 if ((FPort == 0)&& ((Payload == null) || (Payload.Length == 0)))
            if (((Payload == null) || (Payload.Length == 0)))
            {
                if (nwkSkey == null) new NullReferenceException("nwkSkey Error");

                var data = new Byte[CmdPayload.Length];
                LoRaMacCrypto.LoRaMacPayloadEncrypt(CmdPayload, nwkSkey, DevAddr, false, FCnt, data);
                CmdPayload = data;

                // XTrace.WriteLine("密文数据 CmdPayload " + CmdPayload.ToHex());
            }
            else
            {
                if (appSkey == null) new NullReferenceException("appSkey Error");

                var data = new Byte[Payload.Length];
                LoRaMacCrypto.LoRaMacPayloadEncrypt(Payload, appSkey, DevAddr, false, FCnt, data);
                Payload = data;

                // XTrace.WriteLine("密文数据 Payload " + "FPort " + FPort + " : " + Payload.ToHex());
            }
        }

        /// <summary>
        /// 合并消息, Payload 不能被合并,不判断DevAddr方便行事
        /// </summary>
        /// <param name="msg"> </param>
        /// <returns> 是否合并成功 </returns>
        public Boolean Merge(DataMessage msg)
        {
            // 乙方不存在就扔
            if (msg == null) return false;

            if ((Payload != null) && (msg.Payload != null))
            {
                // Payload 不能被合并
                if ((Payload.Length != 0) && (msg.Payload.Length != 0)) return false;
            }

            if ((CmdPayload != null) && (msg.CmdPayload != null))
            {
                // CmdPayload 可以合并 但是长度有限制。
                if (CmdPayload.Length + msg.Payload.Length > 15) return false;
            }

            // 合并 Payload
            if ((Payload == null) || (Payload.Length == 0))
            {
                Payload = msg.Payload;
                FPort = msg.FPort;
            }

            // 合并CmdPayload
            if ((msg.CmdPayload != null) && (msg.CmdPayload.Length != 0))
            {
                var ms = new MemoryStream();
                if (CmdPayload != null) ms.Write(CmdPayload);

                ms.Write(msg.CmdPayload);
                CmdPayload = ms.ToArray();
            }

            // SEQ 跟甲方走 加密秘钥，消息射频信息什么的都跟甲方走

            // ACK判断乙方即可
            if (msg.ACK) ACK = true;
            // 消息类型判断乙方即可
            if (msg.MTpye == MType_e.ConfirmedDataDown) MTpye = msg.MTpye;

            return true;
        }

        ///// <summary>合并消息, Payload 不能被合并</summary>
        ///// <param name="msg"></param>
        ///// <returns>是否合并成功</returns>
        //public Boolean Merge(LoraData msg)
        //{
        //    // 乙方不存在就扔
        //    if (msg == null) return false;

        // if ((Payload != null) && (msg.Payload != null)) { // Payload 不能被合并 if ((Payload.Length != 0) && (msg.Payload.Length != 0)) return false; }

        // // 合并 Payload if ((Payload == null) || (Payload.Length == 0)) { Payload = msg.Payload; FPort = msg.Port; }

        // // ACK判断乙方即可 if (msg.NeedAck) ACK = true;

        //    return true;
        //}

        /// <summary>
        /// 输出基本消息信息
        /// </summary>
        /// <returns> </returns>
        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append(MTpye);
            // sb.Append(" Addr " + DevAddr.GetBytes().ToHex());
            var port = String.Format("{0:X}", FPort);
            sb.Append(" " + DevAddr.GetBytes().ToHex());
            sb.Append(" Port " + port);
            sb.Append(" FCtrl " + FCtrl.ToString("X2"));
            sb.Append(" Seq " + FCnt.ToString("X4"));

            // sb.Append(" MIC " + MIC.ToString("X8")); if (Payload != null) sb.Append(" Payload " + Payload.ToHex()); if (CmdPayload != null) sb.Append(" CmdPayload " + CmdPayload.ToHex());

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 创建回复数据
        /// </summary>
        /// <returns> </returns>
        public DataMessage CreatReply()
        {
            var msg = new DataMessage
            {
                FCnt = FCnt,
                DevAddr = DevAddr,
                Major = Major,

                RadioPkt = RadioPkt,

                NwkSKey = NwkSKey,
                AppSKey = AppSKey,

                // 默认使用不需要ack的数据包
                MTpye = MType_e.UnconfirmedDataDown
            };

            if (MTpye == MType_e.ConfirmedDataUp)
            {
                msg.ACK = true;
            }

            return msg;
        }

        /// <summary>
        /// 创建回复数据
        /// </summary>
        /// <returns> </returns>
        public DataMessage Clone()
        {
            var msg = new DataMessage
            {
                DevAddr = DevAddr,
                MHDR = MHDR,
                FCtrl = FCtrl,
                FCnt = FCnt,
                FPort = FPort,
                MIC = MIC,

                RadioPkt = RadioPkt,
                Valid = Valid,

                NwkSKey = NwkSKey,
                AppSKey = AppSKey,

                MTpye = MTpye
            };

            if (Payload != null) msg.Payload = (Byte[])Payload.Clone();
            if (CmdPayload != null) msg.CmdPayload = (Byte[])CmdPayload.Clone();

            return msg;
        }
    }

    // 命令简介 MHDR(1 byte) LoRaMacAppEui(8 byte) LoRaMacDevEui(8 byte) LoRaMacDevNonce(2 byte) MIC(4 byte) MHDR(1 byte) AppNonce(3 byte) NetID(3 byte) DevAddr(4 byte) DLSettings(1 byte) RxDelay(1 byte)
    // CFList(pad16 0/16byte) MIC(4 byte)

    /*
     具体内容： 
     
    首先，一个End Node需要配置：AppEUI和DevEUI；随机值，得到DevNonce。
    将这3个参数，组织成Join Request数据帧，发送给LoRaWAN LoraWan。

    Server接收到Join Request后，分配DevAddr，连同AppNonce和NetID，
    组织成JoinAccept数据帧，回应给EndNode。

    End Node接收Join Accept后，提取DevAddr；
    结合4个参数：AppKey、AppNonce、NetID和DevNonce，使用aes128_encrypt()，生成2个密钥：NekSKey和AppSKey。


            数据包格式  

    设备节点发送的注册信息
        MHDR(1 byte)  LoRaMacAppEui(8 byte)  LoRaMacDevEui(8 byte)  LoRaMacDevNonce(2 byte)  MIC(4 byte)
        其中只有  MIC 是  前面数据计算的之外，其他全部明文。

    服务器回复数据的解包方式
        使用 LoRaMacAppKey （节点本地存储）解密（LoRaMacJoinDecrypt） 除第一字节（MHDR）之外的所有数据。得到 data。
        LoRaMacJoinComputeMic 计算data的 除最后后4节的 数据的mic。 校验数据签名。
        LoRaMacJoinComputeSKeys 函数提取 LoRaMacNwkSKey  LoRaMacAppSKey  此函数不改变数据包内容。
        此时数据包 内容是
        MHDR(1 byte)  AppNonce(3 byte)  NetID(3 byte)  DevAddr(4 byte)  DLSettings(1 byte)  RxDelay(1 byte)  CFList(pad16 0/16byte) MIC(4 byte)

    提取 NetId （3字节） 提取 DevAddr（4字节）  提取 RxDelay(1 byte)  解析 CFList(pad16)
    RxDelay 是接收窗口1 。  OTAA 在此进行处理！！！ 接收窗口2的值为 RxDelay + 1   单位秒。
    DLSettings 是接收窗口的扩频因子。[6,4]3bit是接收窗口1的  [3,0]4bit是接收窗口2的
    CFList(pad16) 是可选项 内容是信道信息
    */

    /// <summary>
    /// 节点发送的入网请求命令
    /// </summary>
    public class JoinRequest
    {
        #region 节点数据包内容
        /// <summary>
        /// 应用编号8byte
        /// </summary>
        public Int64 AppEui { get; set; }

        /// <summary>
        /// 节点为唯一编号8 byte
        /// </summary>
        public Int64 DevEui { get; set; }

        /// <summary>
        /// 节点生成的随机数，用于消息ID 2byte
        /// </summary>
        public UInt16 DevNonce { get; set; }

        #endregion

        #region 本地数据

        /// <summary>
        /// 对的key AppKey
        /// </summary>
        private Byte[] _Key;

        #endregion

        #region 构造

        public JoinRequest() => MTpye = MType_e.JoinRequest;

        #endregion

        #region 方法
        /// <summary>
        /// 读取JoinRequest数据包
        /// </summary>
        /// <param name="bs"> </param>
        public override void Read(Byte[] bs)
        {
            if (bs == null)
            {
                XTrace.WriteLine("数据为空");
                return;
            }
            // 判断数据包长度
            if (bs.Length < 23)
            {
                XTrace.WriteLine("数据长度{0} 不满足要求", bs.Length);
                Valid = false;
                return;
            }

            Buffer = bs;
            XTrace.WriteLine("JoinRequest len {0}:{1}", bs.Length, bs.ToHex());

            var ms = new MemoryStream(bs);
            // MHDR(1 byte) LoRaMacAppEui(8 byte) LoRaMacDevEui(8 byte) LoRaMacDevNonce(2 byte) MIC(4 byte)
            MHDR = (Byte)ms.ReadByte();
            AppEui = BitConverter.ToInt64(ms.ReadBytes(8), 0);
            DevEui = BitConverter.ToInt64(ms.ReadBytes(8), 0);
            DevNonce = ms.ReadBytes(2).ToUInt16();
            MIC = ms.ReadBytes(4).ToUInt32();

            // XTrace.WriteLine(ToString());
        }

        /// <summary>
        /// 使用key去校验数据合法性
        /// </summary>
        /// <param name="key"> AppEui对应的key </param>
        /// <returns> 校验结果 </returns>
        public Boolean ValidationData(Byte[] key)
        {
            if (!Valid) return false;

            if (key == null)
            {
                XTrace.WriteLine("ValidationData key == null");
                return false;
            }
            if (key.Length != 16)
            {
                XTrace.WriteLine("ValidationData key.len error");
                return false;
            }

            if ((Buffer == null) || (Buffer.Length < 23))
            {
                XTrace.WriteLine("ValidationData Buffer error");
                return false;
            }

            // 提取校验部分数据
            var bs2 = Buffer.ReadBytes(0, Buffer.Length - 4);

            UInt32 mic = 0;
            // 使用 key 去校验数据
            LoRaMacCrypto.LoRaMacJoinComputeMic(bs2, key, ref mic);
            if (MIC != mic)
            {
                XTrace.WriteLine("数据包校验失败，MIC 计算值 {0} 包内值 {1} ", mic.GetBytes().ToHex(), MIC.GetBytes().ToHex());
                Valid = false;
                return false;
            }

            _Key = key;
            Valid = true;
            return true;
        }

        /// <summary>
        /// 创建回复数据
        /// </summary>
        /// <returns> </returns>
        public JoinAccept CreatReply()
        {
            var msg = new JoinAccept
            {
                Major = Major,
                RFU = RFU,

                DevNonce = DevNonce,
                AppKey = _Key,
                // 射频信息还需要继续附带
                RadioPkt = RadioPkt
            };

            return msg;
        }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AppEui ");
            sb.Append(AppEui.GetBytes().ToHex());
            sb.Append(" DevEui ");
            sb.Append(DevEui.GetBytes().ToHex());
            sb.Append(" DevNonce ");
            sb.Append(DevNonce.GetBytes().ToHex());
            sb.Append(" MIC ");
            sb.Append(MIC.GetBytes().ToHex());

            return sb.ToString();
        }

        #endregion
    }

    /// <summary>
    /// 网关回复节点请求命令
    /// </summary>
    public class JoinAccept
    {
        #region 服务器回复数据内容

        private Byte[] _AppNonce;
        /// <summary>
        /// 应用随机数 3byte
        /// </summary>
        public Byte[] AppNonce
        {
            get =>
                // if (_AppNonce == null) _AppNonce = new byte[3]; if (_AppNonce.Length != 3) _AppNonce = new byte[3];
                _AppNonce;
            set
            {
                if (value.Length != 3)
                {
                    XTrace.WriteLine("AppNonce 必须是3字节");
                    return;
                }
                _AppNonce = value;
            }
        }

        private Byte[] _NetId;
        /// <summary>
        /// 网络ID 节点只做储存，没有参与具体业务，可以通过命令让节点返回 3byte
        /// </summary>
        public Byte[] NetId
        {
            get =>
                // if (_NetId == null) _NetId = new byte[3]; if (_NetId.Length != 3) _NetId = new byte[3];
                _NetId;
            set
            {
                if (value.Length != 3)
                {
                    XTrace.WriteLine("NetId 必须是3字节");
                    return;
                }
                _NetId = value;
            }
        }

        /// <summary>
        /// 设备地址
        /// </summary>
        public Int32 DevAddr;

        /// <summary>
        /// 接收窗口参数1byte
        /// </summary>
        private Byte DLSettings;

        /// <summary>
        /// 接收窗口1 DR偏移 3bit
        /// </summary>
        public Int32 Rx1DrOffset
        {
            get => (Byte)((DLSettings >> 4) & 0x07);
            set
            {
                if (value > 7) return;
                DLSettings &= 0x8f;
                DLSettings |= (Byte)(value << 4);
            }
        }

        /// <summary>
        /// 接收通道2 DR 值
        /// </summary>
        public Int32 Rx2DR
        {
            get => (Byte)(DLSettings & 0x0F);
            set
            {
                if (value > 0x0f) return;
                DLSettings &= 0xf0;
                DLSettings |= (Byte)value;
            }
        }

        private Byte _ReceiveDelay1 = 1;

        /// <summary>
        /// 接收窗口1 单位秒, 接收窗口2 比此值大1
        /// </summary>
        public Int32 RxWindow1Delay
        {
            get => _ReceiveDelay1;
            set
            {
                if (value < 1) { XTrace.WriteLine("set RxWindow1Delay 太小 {0}", value); return; }
                if (value > 15) { XTrace.WriteLine("set RxWindow1Delay 太大 {0}", value); return; }
                _ReceiveDelay1 = (Byte)value;
            }
        }

        /// <summary>
        /// 节点端RegionCN470ApplyCFList函数未实现！
        /// </summary>
        public Byte[] CFList { get => null; set { XTrace.WriteLine("CFList 未实现"); return; } }

        #endregion

        #region 服务器需要使用的但不是服务器生成的

        /// <summary>
        /// 终端生成的随机数，用于数据包标记
        /// </summary>
        public UInt16 DevNonce;

        #endregion

        #region 服务器生成的，但是数据包不直接包含的

        /// <summary>
        /// 网络秘钥 16字节
        /// </summary>
        public Byte[] NwkSKey = new Byte[16];

        /// <summary>
        /// 应用秘钥 16字节
        /// </summary>
        public Byte[] AppSKey = new Byte[16];

        /// <summary>
        /// AppEui 对应的key 也就是本数据加密时候需要使用的key
        /// </summary>
        public Byte[] AppKey = new Byte[16];

        #endregion

        #region 构造
        public JoinAccept() => MTpye = MType_e.JoinAccept;

        #endregion

        #region 方法

        /// <summary>
        /// 序列化包
        /// </summary>
        /// <returns> </returns>
        public override Byte[] ToArray()
        {
            if (Buffer == null) Build();

            return Buffer;
        }

        // 创建出消息的具体内容，包含创建出通讯秘钥，加密等。
        public void Build()
        {
            // MHDR(1 byte) AppNonce(3 byte) NetID(3 byte) DevAddr(4 byte) DLSettings(1 byte) RxDelay(1 byte) CFList(pad16 0 / 16byte) MIC(4 byte)
            var ms = new MemoryStream();

            // 回复注册消息
            MTpye = MType_e.JoinAccept;
            ms.Write(MHDR);

            // if (AppNonce == null) { XTrace.WriteLine("AppNonce is null"); return; } if (AppNonce.Length != 3) { XTrace.WriteLine("AppNonce Length error"); return; }
            if (AppNonce == null)
            {
                var rd = new Random();
                var nonce = rd.Next();
                AppNonce = nonce.GetBytes().ReadBytes(0, 3);
            }

            if (NetId == null) { XTrace.WriteLine("NetId is null"); return; }
            if (NetId.Length != 3) { XTrace.WriteLine("NetId Length error"); return; }

            ms.Write(AppNonce);
            ms.Write(NetId);
            ms.Write(DevAddr.GetBytes());
            ms.Write(DLSettings);
            ms.Write(_ReceiveDelay1);

            if (CFList != null)
            {
                if (CFList.Length == 16) ms.Write(CFList);
            }

            LoRaMacCrypto.LoRaMacJoinComputeMic(ms.ToArray(), AppKey, ref MIC);

            // 先写签名
            ms.Write(MIC.GetBytes());
            // 后加密 XTrace.WriteLine("明文前 len {0} : {1}", ms.Position, ms.ToArray().ToHex());

            // 构建加密后的数据流
            var ms2 = new MemoryStream();
            // 重新写入数据
            ms2.Write(MHDR);
            // 加密数据
            var endata = Encrypt(ms.ToArray().ReadBytes(1));

            if (endata == null) return;
            ms2.Write(endata);
            // XTrace.WriteLine("加密后 len {0}:{1}", ms2.Position, ms2.ToArray().ToHex());

            // 到这里了 数据包已经OK 了 可以计算出key 以备使用
            GetSKey();

            // 获取完整包
            Buffer = ms2.ToArray();
        }

        /// <summary>
        /// 加密 此处代码不保险 独立函数
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        private Byte[] Encrypt(Byte[] data)
        {
            var rs = new Byte[data.Length];

            if (LoRaMacCrypto.LoRaMacJoinEncrypt(data, AppKey, rs))
            {
                // XTrace.WriteLine("src data:{0}",data.ToHex()); XTrace.WriteLine("dst data:{0}", rs.ToHex()); XTrace.WriteLine("重复一遍测试"); var rs2 = new byte[data.Length];
                // LoRaMacCrypto.LoRaMacJoinDecrypt(rs, AppKey, rs2); XTrace.WriteLine("xxx data:{0}", rs2.ToHex());

                return rs;
            }

            return null;
        }

        /// <summary>
        /// 计算出AppSKey 和 NwkSKey
        /// </summary>
        private void GetSKey()
        {
            var ms = new MemoryStream();
            ms.Write(AppNonce);
            ms.Write(NetId);

            if (!LoRaMacCrypto.LoRaMacJoinComputeSKeys(AppKey, ms.ToArray(), DevNonce, NwkSKey, AppSKey))
            {
                XTrace.WriteLine("参数有问题 请检查");
            }

            XTrace.WriteLine("NwkSKey {0}, AppSKey {1}", NwkSKey.ToHex(), AppSKey.ToHex());
        }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AppNonce ");
            if (AppNonce != null) sb.Append(AppNonce.ToHex());

            sb.Append(" NetId ");
            if (NetId != null) sb.Append(NetId.ToHex());

            sb.Append(" DevAddr ");
            sb.Append(DevAddr.GetBytes().ToHex());

            sb.Append(" DevNonce ");
            sb.Append(DevNonce.GetBytes().ToHex());

            sb.Append(" RxWindow1Delay ");
            sb.Append(_ReceiveDelay1);

            sb.Append(" MIC ");
            sb.Append(MIC.GetBytes().ToHex());

            return sb.ToString();
        }

        #endregion
    }

    /// <summary>
    /// LoRa消息
    /// </summary>
    /// <remarks> https://github.com/Lora-net/packet_forwarder/blob/master/PROTOCOL.TXT </remarks>
    public class LoRaMessage : IAccessor
    {
        #region 属性
        /// <summary>
        /// 版本
        /// </summary>
        public Byte Version { get; set; } = 2;

        /// <summary>
        /// 随机令牌，请求响应配对
        /// </summary>
        public UInt16 Token { get; set; }

        /// <summary>
        /// 命令。PUSH_DATA=0/PUSH_ACK=2
        /// </summary>
        public LoRaType Command { get; set; }

        /// <summary>
        /// 网关MAC地址
        /// </summary>
        public UInt64 Mac { get; set; }

        /// <summary>
        /// 负载
        /// </summary>
        public Packet Payload { get; set; }
        #endregion

        #region 构造
        /// <summary>
        /// 已重载
        /// </summary>
        public override String ToString() => $"{GetType().Name}[Command={Command}, Mac={Mac:X16}, Token={Token:X4}, Payload={Payload.Total}]";
        #endregion

        #region 核心读写方法
        /// <summary>
        /// 从数据流中读取消息
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        /// <returns> 是否成功 </returns>
        public virtual Boolean Read(Stream stream, Object context)
        {
            Version = (Byte)stream.ReadByte();
            Token = stream.ReadBytes(2).ToUInt16();
            Command = (LoRaType)stream.ReadByte();
            Mac = stream.ReadBytes(8).ToUInt64();

            Payload = stream.ReadBytes(-1);

            return true;
        }

        /// <summary>
        /// 把消息写入到数据流中
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        public virtual Boolean Write(Stream stream, Object context)
        {
            stream.WriteByte(Version);
            stream.Write(Token.GetBytes());
            stream.Write((Byte)Command);
            stream.Write(Mac.GetBytes());

            Payload?.CopyTo(stream);

            return true;
        }

        /// <summary>
        /// 消息转为字节数组
        /// </summary>
        /// <returns> </returns>
        public virtual Byte[] ToArray()
        {
            var ms = new MemoryStream();
            Write(ms, null);
            return ms.ToArray();
        }

        /// <summary>
        /// 转数据包
        /// </summary>
        /// <returns> </returns>
        public virtual Packet ToPacket() => ToArray();
        #endregion

        #region 辅助
        /// <summary>
        /// 创建响应消息
        /// </summary>
        /// <returns> </returns>
        public LoRaMessage CreateReply()
        {
            var rs = new LoRaMessage
            {
                //Version = Version,
                Token = Token,
            };

            switch (Command)
            {
                case LoRaType.PushData: rs.Command = LoRaType.PushAck; break;
                case LoRaType.PushAck:
                    break;
                case LoRaType.PullData: rs.Command = LoRaType.PullAck; break;
                case LoRaType.PullResp:
                    break;
                case LoRaType.PullAck:
                    break;
                case LoRaType.TxAck:
                    break;
                default:
                    break;
            }

            return rs;
        }
        #endregion
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum LoRaType : Byte
    {
        /// <summary>
        /// 推数据。状态和转包数据
        /// </summary>
        PushData = 0,

        /// <summary>
        /// 推数据确认
        /// </summary>
        PushAck = 1,

        /// <summary>
        /// 拉数据。网关要从服务端拉数据
        /// </summary>
        PullData = 2,

        /// <summary>
        /// 拉数据响应。服务端随时可以发要拉的数据给网关
        /// </summary>
        PullResp = 3,

        /// <summary>
        /// 拉数据确认
        /// </summary>
        PullAck = 4,

        /// <summary>
        /// 发送确认
        /// </summary>
        TxAck = 5,
    }

    /// <summary>
    /// MAC消息类型
    /// </summary>
    public enum MessageTypes : Byte
    {
        /// <summary>
        /// 加网请求
        /// </summary>
        JoinRequest = 0b_0000,

        /// <summary>
        /// 加网通过
        /// </summary>
        JoinAccept = 0b_0001,

        /// <summary>
        /// 不需要确认的数据上报
        /// </summary>
        UnconfirmedDataUp = 0b_0010,

        /// <summary>
        /// 不需要确认的数据下发
        /// </summary>
        UnconfirmedDataDown = 0b_0011,

        /// <summary>
        /// 需要确认的数据上报
        /// </summary>
        ConfirmedDataUp = 0b_0100,

        /// <summary>
        /// 需要确认的数据下发
        /// </summary>
        ConfirmedDataDown = 0b_0101,

        /// <summary>
        /// 保留内容
        /// </summary>
        RFU = 0b_0110,

        /// <summary>
        /// 专有消息，用于实现非标准消息格式
        /// </summary>
        Proprietary = 0b_0111,
    }

    /// <summary>
    /// 硬件负载
    /// </summary>
    /// <remarks> https://www.gitbook.com/book/twowinter/lorawan-specification_zh_cn </remarks>
    public class PHYMessage : IAccessor
    {
        #region 属性
        /// <summary>
        /// MAC层帧头
        /// </summary>
        public Byte MHDR { get; set; }

        /// <summary>
        /// 终端短地址
        /// </summary>
        public UInt32 DevAddr { get; set; }

        /// <summary>
        /// 控制字
        /// </summary>
        public Byte FCtrl { get; set; }

        /// <summary>
        /// 帧计数器
        /// </summary>
        /// <remarks> 上行链路计数器（FCntUp），由终端产生并维护，记录发往服务器的帧数量； 下行链路计数器（FCntDown），由服务器产生并维护，记录服务器发往终端的帧数量。 </remarks>
        public UInt16 FCnt { get; set; }

        /// <summary>
        /// 帧配置，字节数不定，最多15字节，大部分情况0个字节
        /// </summary>
        /// <remarks>
        /// 一帧数据中可以包含任何MAC命令，MAC命令既可以放在FOpts中，也可以放在FRMPayload中，但不能同时在两个字段携带MAC命令。 MAC命令放在FRMPayload时，FPort = 0。 放在FOpts的命令不加密（原因：加密Payload，对整个数据签名），也不能超过15个字节（2^4 - 1）。
        /// 放在FRMPayload的MAC命令长度不能超过FRMPayload的最大值。 不想被别人截获的命令要放到FRMPayload，并单独发送该数据帧一条mac命令由一个命令ID（CID，一个字节），和特定的命令序列组成，命令序列可以是空。
        /// </remarks>
        public Packet FOpts { get; set; }

        /// <summary>
        /// MAC数据通道号
        /// </summary>
        public Byte FPort { get; set; }

        /// <summary>
        /// MAC层负载，加密
        /// </summary>
        public Packet Payload { get; set; }

        /// <summary>
        /// 4字节的校验
        /// </summary>
        public UInt32 MIC { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypes Type { get; set; }

        /// <summary>
        /// RFU
        /// </summary>
        public Byte RFU { get; set; }

        /// <summary>
        /// 主版本。0=LoraWan R1
        /// </summary>
        public Byte Major { get; set; }

        /// <summary>
        /// 速率自适应控制
        /// </summary>
        public Boolean ADR { get; set; }

        /// <summary>
        /// 速率自适应控制
        /// </summary>
        public Boolean ADRACKReq { get; set; }

        /// <summary>
        /// 消息确认位。当收到confirmed类型的消息时，进行应答
        /// </summary>
        public Boolean ACK { get; set; }

        /// <summary>
        /// 帧挂起位。
        /// </summary>
        /// <remarks> 只在下行交互中使用，表示网关还有数据挂起等待下发。此时要求终端尽快发送上行消息来再打开接收窗口。 </remarks>
        public Boolean FPending { get; set; }
        #endregion

        #region 构造
        /// <summary>
        /// 已重载
        /// </summary>
        public override String ToString() => $"{GetType().Name}[{Type}, DevAddr={DevAddr:X8}, FCnt={FCnt}, Payload={Payload.Total}]";
        #endregion

        #region 核心读写方法
        /// <summary>
        /// 从数据流中读取消息
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        /// <returns> 是否成功 </returns>
        public virtual Boolean Read(Stream stream, Object context)
        {
            var reader = context as BinaryReader ?? new BinaryReader(stream);
            MHDR = reader.ReadByte();
            DevAddr = reader.ReadUInt32();
            FCtrl = reader.ReadByte();
            FCnt = reader.ReadUInt16();

            // MHDR 扩展
            Type = (MessageTypes)((MHDR & 0b_1110_0000) >> 5);
            RFU = (Byte)((MHDR & 0b_0001_1100) >> 2);
            Major = (Byte)((MHDR & 0b_0000_0011) >> 0);

            // FCtrl 扩展
            ADR = (FCtrl & 0b_1000_0000) > 0;
            ADRACKReq = (FCtrl & 0b_0100_0000) > 0;
            ACK = (FCtrl & 0b_0010_0000) > 0;
            FPending = (FCtrl & 0b_0001_0000) > 0;
            var optsLen = FCtrl & 0b_0000_1111;

            if (optsLen > 0) FOpts = reader.ReadBytes(optsLen);
            FPort = reader.ReadByte();

            var dataLen = stream.Length - stream.Position;
            if (dataLen > 4) Payload = stream.ReadBytes(dataLen - 4);

            MIC = reader.ReadUInt32();

            //Console.WriteLine(Payload.ToHex(64));

            return true;
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="nwkSkey"> </param>
        /// <param name="appSkey"> </param>
        /// <returns> </returns>
        public Byte[] Decrypt(Byte[] nwkSkey, Byte[] appSkey)
        {
            var crypto = new LoRaMacCrypto();

            if (FPort == 0)
            {
                if (nwkSkey == null) throw new ArgumentNullException(nameof(nwkSkey));

                return crypto.PayloadDecrypt(Payload.ToArray(), nwkSkey, DevAddr, true, FCnt);
            }
            else
            {
                if (appSkey == null) throw new ArgumentNullException(nameof(appSkey));

                return crypto.PayloadDecrypt(Payload.ToArray(), appSkey, DevAddr, true, FCnt);
            }
        }

        /// <summary>
        /// 把消息写入到数据流中
        /// </summary>
        /// <param name="stream">  数据流 </param>
        /// <param name="context"> 上下文 </param>
        public virtual Boolean Write(Stream stream, Object context)
        {
            stream.WriteByte(MHDR);
            stream.Write(DevAddr.GetBytes());
            stream.Write((Byte)FCtrl);

            Payload?.CopyTo(stream);

            return true;
        }

        /// <summary>
        /// 消息转为字节数组
        /// </summary>
        /// <returns> </returns>
        public virtual Byte[] ToArray()
        {
            var ms = new MemoryStream();
            Write(ms, null);
            return ms.ToArray();
        }

        /// <summary>
        /// 转数据包
        /// </summary>
        /// <returns> </returns>
        public virtual Packet ToPacket() => ToArray();
        #endregion

        #region 辅助
        #endregion
    }
}
