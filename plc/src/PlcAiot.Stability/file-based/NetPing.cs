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

// 网络连接测试驱动
namespace Feature.NetPing.Drivers
{
    using Feature.IoT.Drivers;
    using Feature.IoT.ThingModels;
    using Feature.IoT.ThingSpecification;
    using System.ComponentModel;
    using System.Net.NetworkInformation;
    using System.Reflection;

    /// <summary>
    /// 设备网络心跳驱动
    /// </summary>
    /// <remarks> IoT驱动，通过Ping探测到目标设备的网络情况，并收集延迟数据 </remarks>
    [Driver("NetPing")]
    [DisplayName("设备网络心跳")]
    public class NetPingDriver : DriverBase<Node, NetPingParameter>
    {
        #region 方法
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

            var p = node.Parameter as NetPingParameter;
            foreach (var point in points)
            {
                if (!point.Address.IsNullOrEmpty())
                {
                    try
                    {
                        var reply = new Ping().Send(point.Address, p.Timeout);
                        if (reply.Status == IPStatus.Success)
                            dic[point.Name] = reply.RoundtripTime;
                        if (p.RetrieveStatus)
                            dic[point.Name + "-Status"] = reply.Status + "";
                    }
                    catch (Exception ex)
                    {
                        dic[point.Name + "-Status"] = ex.GetTrue().Message;
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// 发现本地节点
        /// </summary>
        /// <returns> </returns>
        public override ThingSpec GetSpecification()
        {
            var type = GetType();
            var spec = new ThingSpec
            {
                Profile = new Profile
                {
                    Version = type.Assembly.GetName().Version + "",
                    ProductKey = type.GetCustomAttribute<DriverAttribute>().Name
                }
            };

            var points = new List<PropertySpec>();
            //var extends = new List<PropertyExtend>();

            // 所有网关地址和DNS地址
            var gaddrs = new List<String>();
            var daddrs = new List<String>();
            var gi = 1;
            var di = 1;
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipps = item.GetIPProperties();
                foreach (var elm in ipps.GatewayAddresses)
                {
                    var ip = elm.Address + "";
                    if (!gaddrs.Contains(ip))
                    {
                        var name = "Gateway";
                        if (gi > 1) name += gi++;
                        var ps = PropertySpec.Create(name, $"{item.Name}网关", "int", 0, ip);
                        ps.DataType.Specs = new DataSpecs { Unit = "ms", UnitName = "毫秒" };
                        points.Add(ps);
                        //extends.Add(new PropertyExtend { Id = name, Address = ip });
                        gaddrs.Add(ip);
                    }
                }
                foreach (var elm in ipps.DnsAddresses)
                {
                    if (!elm.IsIPv4()) continue;

                    var ip = elm + "";
                    if (!daddrs.Contains(ip))
                    {
                        var name = "Dns";
                        if (di > 1) name += di++;
                        var ps = PropertySpec.Create(name, $"{item.Name}DNS", "int", 0, ip);
                        ps.DataType.Specs = new DataSpecs { Unit = "ms", UnitName = "毫秒" };
                        points.Add(ps);
                        //extends.Add(new PropertyExtend { Id = name, Address = ip });
                        daddrs.Add(ip);
                    }
                }
            }

            spec.Properties = points.ToArray();

            return spec;
        }
        #endregion
    }

    /// <summary>
    /// NetPing参数
    /// </summary>
    public class NetPingParameter : IDriverParameter
    {
        /// <summary>
        /// 超时。指定（发送回送消息后）等待 ICMP 回送答复消息的最大毫秒数，默认5000ms
        /// </summary>
        [Description("超时。指定（发送回送消息后）等待 ICMP 回送答复消息的最大毫秒数，默认5000ms")]
        public Int32 Timeout { get; set; } = 5000;

        /// <summary>
        /// 采集状态。采集网络状态，在Ping失败时能够获取准确的失败原因，默认true
        /// </summary>
        [Description("采集状态。采集网络状态，在Ping失败时能够获取准确的失败原因，默认true")]
        public Boolean RetrieveStatus { get; set; } = true;
    }
}
