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

// 日本松下 PLC（Panasonic PLC）
namespace Feature.Panasonic.Drivers
{
    using Feature.IoT.Drivers;
    using Feature.IoT.Protocols;
    using System.ComponentModel;

    /// <summary>
    /// 松下PLC协议封装
    /// </summary>
    [Driver("PanasonicPLC")]
    [DisplayName("松下PLC")]
    public class PanasonicDriver : ModbusDriver, IDriver
    {
        #region 方法
        /// <summary>
        /// 创建Modbus通道
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="node">      设备节点 </param>
        /// <param name="parameter"> 参数 </param>
        /// <returns> </returns>
        protected override Modbus CreateModbus(IDevice device, ModbusNode node, ModbusParameter parameter)
        {
            var p = parameter as ModbusTcpParameter;
            if (p == null || p.Server.IsNullOrEmpty()) throw new ArgumentException("参数中未指定地址Server");

            node.Parameter = p;

            var modbus = new ModbusTcp
            {
                Server = p.Server,
                ProtocolId = p.ProtocolId,

                Tracer = Tracer,
                Log = Log,
            };
            //modbus.Init(parameters);

            return modbus;
        }
        #endregion
    }

    /// <summary>
    /// Modbus节点
    /// </summary>
    public class PanasonicNode : INode
    {
        /// <summary>
        /// Modbus对象
        /// </summary>
        public Modbus Modbus { get; set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        public Byte Host { get; set; }

        /// <summary>
        /// 读取功能码
        /// </summary>
        public FunctionCodes ReadCode { get; set; }

        /// <summary>
        /// 写入功能码
        /// </summary>
        public FunctionCodes WriteCode { get; set; }

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
    /// 松下PLC参数
    /// </summary>
    public class PanasonicParameter : IDriverParameter
    {
        /// <summary>
        /// 主机/站号
        /// </summary>
        [Description("主机/站号")]
        public Byte Host { get; set; }

        /// <summary>
        /// 读取功能码.若点位地址未指定区域，则采用该功能码
        /// </summary>
        [Description("读取功能码.若点位地址未指定区域，则采用该功能码")]
        public FunctionCodes ReadCode { get; set; }

        /// <summary>
        /// 写入功能码.若点位地址未指定区域，则采用该功能码
        /// </summary>
        [Description("写入功能码.若点位地址未指定区域，则采用该功能码")]
        public FunctionCodes WriteCode { get; set; }

        /// <summary>
        /// 地址.tcp地址如127.0.0.1:502
        /// </summary>
        [Description("地址.tcp地址如127.0.0.1:502")]
        public String Server { get; set; }

        /// <summary>
        /// 协议标识.默认0
        /// </summary>
        [Description("协议标识.默认0")]
        public UInt16 ProtocolId { get; set; }
    }
}
