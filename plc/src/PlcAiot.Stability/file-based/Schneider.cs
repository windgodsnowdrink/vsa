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

// 施耐德PLC相关的驱动程序和功能实现
namespace Feature.Schneider.Drivers
{
    using Feature.IoT.Drivers;
    using Feature.IoT;
    using Feature.Log;
    using System.ComponentModel;

    /// <summary>
    /// 施耐德PLC驱动
    /// </summary>
    [Driver("SchneiderPLC")]
    [DisplayName("施耐德PLC")]
    public class SchneiderDriver : ModbusTcpDriver, ILogFeature, ITracerFeature
    {
        /// <summary>
        /// 建立连接，打开驱动
        /// </summary>
        /// <param name="device">    </param>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        public override INode Open(IDevice device, IDriverParameter parameter)
        {
            var modbusNode = base.Open(device, parameter);
            if (modbusNode is ModbusNode node && Modbus != null)
            {
                Modbus.Open();
            }

            return modbusNode;
        }
    }

    /// <summary>
    /// 节点
    /// </summary>
    public class SchneiderNode : INode
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public String Address { get; set; }

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
    public class SchneiderParameter : ModbusTcpParameter
    {
    }
}
