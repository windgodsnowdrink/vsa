#!/usr/bin/env dotnet
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
#:package Microsoft.AspNetCore.OpenApi@11.0.0-preview.2.26159.112
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
#:property BlazorDisableThrowNavigationException=true
#:property DefineConstants=$(DefineConstants);$(EnvVarConstant)

namespace Feature;

using Feature.IoTDatabase.Drivers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var defaultLogLevel = builder.Configuration["Logging:LogLevel:Default"];
var allowedHosts = builder.Configuration["AllowedHosts"];

builder.Services.AddRazorComponents();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<plc.pages.Index>();
// app.MapRazorComponents<App>();

app.MapGet("/", () => new HelloResponse { Message = "Hello, World!" })
    .WithName("HelloWorld");

app.MapGet("/config", (IConfiguration config) => new
{
    DefaultLogLevel = config["Logging:LogLevel:Default"],
    AllowedHosts = config["AllowedHosts"]
});

app.Run();

class HelloResponse
{
    public string Message { get; set; } = "Hello, World!";
}

[JsonSerializable(typeof(HelloResponse))]
partial class AppJsonSerializerContext : JsonSerializerContext
{
}

// IoT数据库驱动,符合IoT标准的通用数据库驱动实现,支持通过SQL从各类数据库读写数据
namespace Feature.IoTDatabase.Drivers
{
    using Feature.DataAccessLayer;
    using Feature.IoT;
    using Feature.IoT.Drivers;
    using Feature.IoT.ThingModels;
    using Feature.Reflection;
    using System.ComponentModel;

    /// <summary>
    /// 通用数据库驱动参数
    /// </summary>
    public class DatabaseParameter : IDriverParameter, IDriverParameterKey
    {
        /// <summary>
        /// 数据库连接字符串.例如：server=.;database=iot;user=iot;password=iot
        /// </summary>
        [Description("数据库连接字符串.例如：server=.;database=iot;user=iot;password=iot")]
        public string Connectionstring { get; set; } = null!;

        /// <summary>
        /// 数据库类型
        /// </summary>
        [Description("数据库类型")]
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// 查询语句.Select查询返回每个字段名对应点位名
        /// </summary>
        [Description("查询语句.Select查询返回每个字段名对应点位名")]
        public string QuerySql { get; set; }

        /// <summary>
        /// 捕获所有字段.捕获所有字段作为返回,而不管指定哪些点位,默认false
        /// </summary>
        [Description("捕获所有字段.捕获所有字段作为返回,而不管指定哪些点位,默认false")]
        public bool CaptureAll { get; set; }

        /// <summary>
        /// 获取唯一标识
        /// </summary>
        /// <returns> </returns>
        public string GetKey() => Connectionstring;
    }

    /// <summary>
    /// 数据库节点.多设备共用驱动时,以节点区分
    /// </summary>
    public class DatabseNode : Node
    {
        /// <summary>
        /// 客户端
        /// </summary>
        public DAL Dal { get; set; } = null!;

        /// <summary>
        /// 参数.设备使用的专用参数
        /// </summary>
        public DatabaseParameter DatabaseParameter { get; set; }
    }

    /// <summary>
    /// IoT标准通用数据库驱动
    /// </summary>
    /// <remarks> IoT驱动,符合IoT标准的通用数据库驱动,连接后向目标发送数据即可收到数据. </remarks>
    [Driver("IoTDatabase")]
    [DisplayName("通用数据库驱动")]
    public class IoTDatabaseDriver : DriverBase<DatabseNode, DatabaseParameter>
    {
        #region 属性
        #endregion

        #region 方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="parameter"> 参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public override INode Open(IDevice device, IDriverParameter parameter)
        {
            if (parameter is not DatabaseParameter p) throw new ArgumentException("参数不能为空");
            if (p.Connectionstring.IsNullOrEmpty()) throw new ArgumentException("数据库地址不能为空");

            var node = new DatabseNode
            {
                Driver = this,
                Device = device,
                Parameter = p,
                DatabaseParameter = p,
            };

            var connName = p.Connectionstring.GetBytes().Crc().GetBytes().ToHex();
            if (!DAL.ConnStrs.ContainsKey(connName))
            {
                DAL.AddConnStr(connName, p.Connectionstring, null, p.DatabaseType + "");
            }

            node.Dal = DAL.Create(connName);

            return node;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points"> 点位集合,Address属性地址示例：D100,C100,W100,H100 </param>
        /// <returns> </returns>
        public override IDictionary<string, Object> Read(INode node, IPoint[] points)
        {
            var result = new Dictionary<string, Object>();
            //if (points == null) return result;

            var client = (node as DatabseNode)?.Dal;
            if (client == null || node.Parameter is not DatabaseParameter parameter) return result;

            var sql = parameter.QuerySql;
            var dt = client.Query(sql);

            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                // 只要第一行数据
                var row = dt.Rows[0];
                for (var i = 0; i < dt.Columns.Length; i++)
                {
                    var name = dt.Columns[i];
                    var value = row[i];

                    // 如果点位存在,赋值
                    var point = points?.FirstOrDefault(p => name.EqualIgnoreCase(p.Name, p.Address));
                    if (point != null)
                    {
                        // 如果点位有类型,转换类型
                        var type = point.GetNetType();
                        if (type != null) value = value.ChangeType(type);

                        result[point.Name] = value;
                    }
                    else if (parameter.CaptureAll)
                    {
                        // 如果点位没有指定,且允许捕获所有字段,则直接返回
                        result[name] = value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public override Object Write(INode node, IPoint point, Object value) => throw new NotSupportedException();

        /// <summary>
        /// 设备控制
        /// </summary>
        /// <param name="node">       </param>
        /// <param name="parameters"> </param>
        public override Object Control(INode node, IDictionary<string, Object> parameters) => throw new NotSupportedException();
        #endregion
    }
}

// IoT串口驱动,符合IoT标准的通用串口驱动实现,连接后向目标发送数据即可收到数据
namespace Feature.IoTSerial.Drivers
{
    using Feature.Data;
    using Feature.IoT;
    using Feature.IoT.Controllers;
    using Feature.IoT.Drivers;
    using Feature.IoT.ThingModels;
    using Feature.IoT.ThingSpecification;
    using Feature.Model;
    using Feature.Reflection;
    using Feature.Serialization;
    using System.ComponentModel;
    using System.ComponentModel;
    using System.IO.Ports;
    using System.Text;

    /// <summary>
    /// 串口参数
    /// </summary>
    public class SerialParameter : IDriverParameter, IDriverParameterKey
    {
        /// <summary>
        /// 串口
        /// </summary>
        [Description("串口")]
        public string PortName { get; set; } = "COM1";

        /// <summary>
        /// 波特率
        /// </summary>
        [Description("波特率")]
        public Int32 Baudrate { get; set; } = 9600;

        /// <summary>
        /// 数据位.默认8
        /// </summary>
        [Description("数据位")]
        public Int32 DataBits { get; set; } = 8;

        /// <summary>
        /// 奇偶校验位.默认None无校验
        /// </summary>
        [Description("奇偶校验位")]
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// 停止位.默认One
        /// </summary>
        [Description("停止位")]
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 超时时间.发起请求后等待响应的超时时间,默认3000ms
        /// </summary>
        [Description("超时时间.发起请求后等待响应的超时时间,默认3000ms")]
        public Int32 Timeout { get; set; } = 3000;

        /// <summary>
        /// 字节超时.数据包间隔,默认10ms
        /// </summary>
        [Description("字节超时.数据包间隔,默认10ms")]
        public Int32 ByteTimeout { get; set; } = 10;

        /// <summary>
        /// 获取唯一标识
        /// </summary>
        /// <returns> </returns>
        public string GetKey() => PortName;
    }

    /// <summary>
    /// 通用串口驱动参数
    /// </summary>
    public class IoTSerialParameter : SerialParameter
    {
        /// <summary>
        /// 请求命令模板.支持十六进制格式(以0x开头)和字符串格式
        /// </summary>
        [Description("请求命令模板.支持十六进制格式(以0x开头)和字符串格式")]
        public string RequestCommand { get; set; } = "";

        /// <summary>
        /// 响应数据编码格式.支持HEX,ASCII,UTF8,Json等
        /// </summary>
        [Description("响应数据编码格式.支持HEX,ASCII,UTF8,Json等")]
        public string ResponseEncoding { get; set; } = "HEX";

        /// <summary>
        /// 捕获所有字段.捕获所有字段作为返回,而不管指定哪些点位,默认false
        /// </summary>
        [Description("捕获所有字段.捕获所有字段作为返回,而不管指定哪些点位,默认false")]
        public bool CaptureAll { get; set; }
    }

    /// <summary>
    /// 串口节点.多设备共用驱动时,以节点区分
    /// </summary>
    public class SerialNode : Node
    {
        /// <summary>
        /// 串口对象
        /// </summary>
        public ISerialPort SerialPort { get; set; } = null!;

        /// <summary>
        /// 参数.设备使用的专用参数
        /// </summary>
        public SerialParameter? SerialParameter { get; set; }
    }

    /// <summary>
    /// IoT标准通用串口驱动
    /// </summary>
    /// <remarks> IoT驱动,符合IoT标准的通用串口驱动,连接后向目标发送数据即可收到数据. </remarks>
    [Driver("IoTSerial")]
    [DisplayName("通用串口驱动")]
    public class IoTSerialDriver : DriverBase<SerialNode, IoTSerialParameter>
    {
        #region 属性
        private Int32 _nodes;
        private ISerialPort? _serialPort;
        #endregion

        #region 方法
        /// <summary>
        /// 获取产品物模型
        /// </summary>
        protected override bool OnGetSpecification(ThingSpec thingSpec)
        {
            thingSpec.Properties =
            [
                PropertySpec.Create("Data", "响应数据", "string")
            ];

            return true;
        }

        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="parameter"> 参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public override INode Open(IDevice device, IDriverParameter? parameter)
        {
            if (parameter is not IoTSerialParameter p) throw new ArgumentException("参数不能为空");
            if (p.PortName.IsNullOrEmpty()) throw new ArgumentException("串口名称不能为空");

            if (p.Baudrate <= 0) p.Baudrate = 9600;

            var node = new SerialNode
            {
                Driver = this,
                Device = device,
                Parameter = p,
                SerialParameter = p,
            };

            if (_serialPort == null)
            {
                lock (this)
                {
                    _serialPort ??= CreateSerial(p);
                }
            }

            node.SerialPort = _serialPort;

            Interlocked.Increment(ref _nodes);

            return node;
        }

        /// <summary>
        /// 创建串口
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        protected virtual ISerialPort CreateSerial(SerialParameter parameter)
        {
            var board = ServiceProvider?.GetService<IBoard>();
            var serialPort = board?.CreateSerial(parameter.PortName, parameter.Baudrate);

            // 创建串口对象
            if (serialPort == null)
            {
                // 借助IBoard服务获取串口映射名,在A2工业计算机中,可使用COM1替代/dev/ttyAMA0
                var portName = parameter.PortName;
                if (board != null)
                {
                    var portName2 = board.Map(portName);
                    if (!portName2.IsNullOrEmpty()) portName = portName2;
                }

                serialPort = ServiceProvider?.GetService<ISerialPort>() ?? new DefaultSerialPort();
                serialPort.PortName = portName;
                serialPort.Baudrate = parameter.Baudrate;
            }
            serialPort.Timeout = parameter.Timeout;

            if (serialPort is DefaultSerialPort sp)
            {
                sp.DataBits = parameter.DataBits;
                sp.Parity = parameter.Parity;
                sp.StopBits = parameter.StopBits;
                sp.ByteTimeout = parameter.ByteTimeout;
            }
            else
            {
                serialPort.SetValue("DataBits", parameter.DataBits);
                serialPort.SetValue("Parity", parameter.Parity);
                serialPort.SetValue("StopBits", parameter.StopBits);
                serialPort.SetValue("ByteTimeout", parameter.ByteTimeout);
            }

            return serialPort;
        }

        /// <summary>
        /// 关闭设备节点
        /// </summary>
        public override void Close(INode node)
        {
            if (Interlocked.Decrement(ref _nodes) <= 0)
            {
                _serialPort.TryDispose();
                _serialPort = null;
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points"> 点位集合,Address属性地址示例：D100,C100,W100,H100 </param>
        /// <returns> </returns>
        public override IDictionary<string, Object?> Read(INode node, IPoint[] points)
        {
            var result = new Dictionary<string, Object?>();
            //if (points == null) return result;

            var client = (node as SerialNode)?.SerialPort;
            if (client == null || node.Parameter is not IoTSerialParameter parameter) return result;

            var request = Encode(parameter.RequestCommand);
            var response = client.Invoke(request, 1);

            var rs = Decode(response, parameter.ResponseEncoding);
            if (rs is IDictionary<string, Object?> dic)
            {
                foreach (var item in dic)
                {
                    var name = item.Key;
                    var value = item.Value;
                    var point = points?.FirstOrDefault(e => name.EqualIgnoreCase(e.Name, e.Address));
                    if (point != null)
                    {
                        // 如果点位有类型,转换类型
                        var type = point.GetNetType();
                        if (type != null) value = value.ChangeType(type);

                        result[name] = value;
                    }
                    else if (parameter.CaptureAll)
                    {
                        // 如果点位没有指定,且允许捕获所有字段,则直接返回
                        result[name] = value;
                    }
                }
            }
            else
            {
                result["Data"] = rs;
            }

            return result;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public override Object? Write(INode node, IPoint point, Object? value)
        {
            var client = (node as SerialNode)?.SerialPort;
            if (client == null || node.Parameter is not IoTSerialParameter parameter) return null;

            var request = Encode(value);
            var response = client.Invoke(request, 1);

            return Decode(response, parameter.ResponseEncoding);
        }

        /// <summary>
        /// 设备控制
        /// </summary>
        /// <param name="node">       </param>
        /// <param name="parameters"> </param>
        public override Object? Control(INode node, IDictionary<string, Object?> parameters)
        {
            var service = JsonHelper.Convert<ServiceModel>(parameters);
            if (service == null || service.Name.IsNullOrEmpty()) throw new NotImplementedException();

            var client = (node as SerialNode)?.SerialPort;
            if (client == null || node.Parameter is not IoTSerialParameter parameter) return null;

            // 批量操作
            var result = new Dictionary<string, Object?>();
            foreach (var item in parameters)
            {
                var request = Encode(item.Value);
                var response = client.Invoke(request, 1);

                // 转换编码
                if (!parameter.ResponseEncoding.IsNullOrEmpty())
                    result[item.Key] = Decode(response, parameter.ResponseEncoding);
                else
                    result[item.Key] = response;
            }

            return result;
        }

        /// <summary>
        /// 编码请求数据
        /// </summary>
        /// <param name="value"> </param>
        /// <returns> </returns>
        protected virtual IPacket? Encode(Object? value)
        {
            if (value == null) return null;

            switch (value)
            {
                case IPacket pk:
                    return pk;
                case string str:
                    if (str.StartsWithIgnoreCase("0x"))
                        return new ArrayPacket(str[2..].ToHex());
                    else
                        return new ArrayPacket(str.GetBytes());
                case Byte[] bytes:
                    return new ArrayPacket(bytes);
                default:
                    throw new NotSupportedException($"不支持的数据类型 {value?.GetType().FullName}");
            }
        }

        /// <summary>
        /// 解码响应数据
        /// </summary>
        /// <param name="data">     </param>
        /// <param name="encoding"> </param>
        /// <returns> </returns>
        protected virtual Object? Decode(IPacket? data, string encoding)
        {
            return encoding switch
            {
                "HEX" => data?.ToHex(),
                "ASCII" => data?.ToStr(Encoding.ASCII),
                "UTF8" => data?.ToStr(Encoding.UTF8),
                "Json" => data == null ? null : JsonParser.Decode(data.ToStr()),
                _ => data,
            };
        }
        #endregion
    }
}

// IoT网络驱动,符合IoT标准的通用网络驱动实现,支持TCP,UDP,HTTP等协议,连接后向目标发送数据即可收到数据
namespace Feature.IotSocket.Drivers
{
    using Feature.Data;
    using Feature.IoT.Drivers;
    using Feature.IoT.Models;
    using Feature.IoT.ThingModels;
    using Feature.IoT.ThingSpecification;
    using Feature.IoT;
    using Feature.Net;
    using Feature.Net;
    using Feature.Reflection;
    using Feature.Serialization;
    using Feature;
    using System.ComponentModel;
    using System.Net.Http.Headers;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Net;
    using System.Text;
#if !NET40
    using TaskEx = System.Threading.Tasks.Task;
#endif

    /// <summary>
    /// 通用Http驱动参数
    /// </summary>
    public class HttpParameter : IDriverParameter
    {
        /// <summary>
        /// 服务端地址.如http://plc.dev.localhost:6600
        /// </summary>
        [Description("服务端地址.如http://plc.dev.localhost:6600")]
        public string Address { get; set; } = null!;

        /// <summary>
        /// 请求方法.Http请求方法GET/POST,默认GET
        /// </summary>
        [Description("请求方法.Http请求方法GET/POST,默认GET")]
        public string Method { get; set; } = "GET";

        /// <summary>
        /// 资源路径.请求Url的路径部分
        /// </summary>
        [Description("资源路径.请求Url的路径部分")]
        public string? PathAndQuery { get; set; }

        /// <summary>
        /// 令牌.在请求头中以Bearer形式传输
        /// </summary>
        [Description("令牌.在请求头中以Bearer形式传输")]
        public string? Token { get; set; }

        /// <summary>
        /// 超时时间.发起请求后等待响应的超时时间,默认5000ms
        /// </summary>
        [Description("超时时间.发起请求后等待响应的超时时间,默认5000ms")]
        public Int32 Timeout { get; set; } = 5_000;

        /// <summary>
        /// 提交数据.支持十六进制格式(以0x开头)和字符串格式
        /// </summary>
        [Description("提交数据.支持十六进制格式(以0x开头)和字符串格式")]
        public string PostData { get; set; } = "";

        /// <summary>
        /// 捕获所有字段.捕获所有字段作为返回,而不管指定哪些点位,默认false
        /// </summary>
        [Description("捕获所有字段.捕获所有字段作为返回,而不管指定哪些点位,默认false")]
        public bool CaptureAll { get; set; }
    }

    /// <summary>
    /// IoT标准通用Http驱动
    /// </summary>
    /// <remarks> IoT驱动,符合IoT标准的通用Http驱动,连接后向目标发送数据即可收到数据. </remarks>
    [Driver("IoTHttp")]
    [DisplayName("通用Http驱动")]
    public class IoTHttpDriver : AsyncDriverBase<Node, HttpParameter>
    {
        #region 属性
        /// <summary>
        /// 客户端
        /// </summary>
        public HttpClient? Client { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">            逻辑设备 </param>
        /// <param name="parameter">         参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public override async Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
        {
            if (parameter is not HttpParameter p) throw new ArgumentException("参数不能为空");
            if (p.Address.IsNullOrEmpty()) throw new ArgumentException("网络地址不能为空");

            var node = await base.OpenAsync(device, parameter);

            var client = new HttpClient
            {
                BaseAddress = new Uri(p.Address),
                Timeout = TimeSpan.FromMilliseconds(p.Timeout),
            };

            if (!p.Token.IsNullOrEmpty())
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", p.Token);

            Client = client;

            return node;
        }

        /// <summary>
        /// 关闭设备节点.多节点共用通信链路时,需等最后一个节点关闭才能断开
        /// </summary>
        /// <param name="node">              </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        public override Task CloseAsync(INode node, CancellationToken cancellationToken = default)
        {
            Client.TryDispose();
            Client = null;

#if NET40 || NET45
        return TaskEx.FromResult(0);
#else
            return TaskEx.CompletedTask;
#endif
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <remarks> 驱动实现数据采集的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points">            点位集合 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        /// <returns> </returns>
        public override async Task<IDictionary<string, Object?>> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<string, Object?>();
            //if (points == null) return result;

            var client = Client;
            if (client == null) return result;
            if (node.Parameter is not HttpParameter parameter) return result;

            var path = parameter.PathAndQuery;
            if (path.IsNullOrEmpty()) path = "/";

            // 根据不同场景请求数据
            string? response = null;
            if (parameter.Method.EqualIgnoreCase("GET"))
            {
                response = await client.GetstringAsync(path);
            }
            else
            {
                var str = parameter.PostData;
                if (str.IsNullOrEmpty())
                {
                    var rs = await client.PostAsync(path, new stringContent(""), cancellationToken);
                    response = await rs.Content.ReadAsstringAsync();
                }
                else
                {
                    HttpContent? content;
                    if (str.StartsWithIgnoreCase("0x"))
                        content = new ByteArrayContent(str[2..].ToHex());
                    else if (str[0] == '{' && str[^1] == '}')
                        content = new stringContent(str, Encoding.UTF8, "application/json");
                    else if (str.Contains('='))
                        content = new stringContent(str, Encoding.UTF8, "application/x-www-form-urlencoded");
                    else
                        content = new stringContent(str, Encoding.UTF8, "text/plain");

                    var rs = await client.PostAsync(path, content, cancellationToken);
                    response = await rs.Content.ReadAsstringAsync();
                }
            }

            if (!response.IsNullOrEmpty())
            {
                // 尝试按照Json来解析数据,转为字典,然后逐个给点位赋值
                var dic = Decode(response);
                if (dic != null)
                {
                    foreach (var item in dic)
                    {
                        var name = item.Key;
                        var value = item.Value;
                        var point = points?.FirstOrDefault(e => name.EqualIgnoreCase(e.Name, e.Address));
                        if (point != null)
                        {
                            // 如果点位有类型,转换类型
                            var type = point.GetNetType();
                            if (type != null) value = value.ChangeType(type);

                            result[name] = value;
                        }
                        else if (parameter.CaptureAll)
                        {
                            // 如果点位没有指定,且允许捕获所有字段,则直接返回
                            result[name] = value;
                        }
                    }
                }
                else
                {
                    // 如果没有解析出来,直接返回原始数据
                    result["data"] = response;
                }
            }

            return result;
        }

        /// <summary>
        /// 解码字符串
        /// </summary>
        /// <param name="value"> </param>
        /// <returns> </returns>
        protected virtual IDictionary<string, Object?>? Decode(string value)
        {
            if (value.IsNullOrEmpty()) return null;

            var dic = JsonParser.Decode(value) ?? XmlParser.Decode(value);
            if (dic == null) return null;

            // 内嵌data
            if (dic.TryGetValue("data", out var data) && data is IDictionary<string, Object?> dic2) return dic2;

            return dic;
        }

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="values">            点位数值 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        public override TaskEx WriteAsync(INode node, IDictionary<IPoint, Object> values, CancellationToken cancellationToken = default) => base.WriteAsync(node, values, cancellationToken);
        #endregion
    }

    /// <summary>
    /// IoT标准通用T网络驱动
    /// </summary>
    /// <remarks> IoT驱动,符合IoT标准的通用网络驱动,连接后向目标发送数据即可收到数据. </remarks>
    public abstract class IoTSocketDriver : DriverBase<SocketNode, SocketParameter>
    {
        #region 属性
        private Int32 _nodes;
        private ISocketClient? _client;
        #endregion

        #region 方法
        /// <summary>
        /// 获取产品物模型
        /// </summary>
        protected override bool OnGetSpecification(ThingSpec thingSpec)
        {
            thingSpec.Properties =
            [
                PropertySpec.Create("Data", "响应数据", "string")
            ];

            return true;
        }

        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="parameter"> 参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public override INode Open(IDevice device, IDriverParameter? parameter)
        {
            if (parameter is not SocketParameter p) throw new ArgumentException("参数不能为空");
            if (p.Server.IsNullOrEmpty()) throw new ArgumentException("网络地址不能为空");

            var node = new SocketNode
            {
                Driver = this,
                Device = device,
                Parameter = p,
                SocketParameter = p,
            };

            if (_client == null)
            {
                lock (this)
                {
                    _client ??= CreateClient(p);
                }
            }

            node.Client = _client;

            Interlocked.Increment(ref _nodes);

            return node;
        }

        /// <summary>
        /// 创建网络
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        protected virtual ISocketClient CreateClient(SocketParameter parameter)
        {
            var uri = new NetUri(NetType.Tcp, parameter.Server, parameter.Port);
            var client = uri.CreateRemote();

            client.Timeout = parameter.Timeout;

            return client;
        }

        /// <summary>
        /// 关闭设备节点
        /// </summary>
        public override void Close(INode node)
        {
            if (Interlocked.Decrement(ref _nodes) <= 0)
            {
                _client.TryDispose();
                _client = null;
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points"> 点位集合,Address属性地址示例：D100,C100,W100,H100 </param>
        /// <returns> </returns>
        public override IDictionary<string, Object?> Read(INode node, IPoint[] points)
        {
            var result = new Dictionary<string, Object?>();
            if (points == null) return result;

            var client = (node as SocketNode)?.Client;
            if (client == null || node.Parameter is not SocketParameter parameter) return result;

            var request = Encode(parameter.RequestCommand);
            if (request != null) client.Send(request);
            var response = client.Receive();

            result["Data"] = Decode(response, parameter.ResponseEncoding);

            return result;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public override Object? Write(INode node, IPoint point, Object? value)
        {
            var client = (node as SocketNode)?.Client;
            if (client == null || node.Parameter is not SocketParameter parameter) return null;

            var request = Encode(value);
            if (request != null) client.Send(request);
            var response = client.Receive();

            return Decode(response, parameter.ResponseEncoding);
        }

        /// <summary>
        /// 设备控制
        /// </summary>
        /// <param name="node">       </param>
        /// <param name="parameters"> </param>
        public override Object? Control(INode node, IDictionary<string, Object?> parameters)
        {
            var service = JsonHelper.Convert<ServiceModel>(parameters);
            if (service == null || service.Name.IsNullOrEmpty()) throw new NotImplementedException();

            var client = (node as SocketNode)?.Client;
            if (client == null || node.Parameter is not SocketParameter parameter) return null;

            // 批量操作
            var result = new Dictionary<string, Object?>();
            foreach (var item in parameters)
            {
                var request = Encode(item.Value);
                if (request != null) client.Send(request);
                var response = client.Receive();

                // 转换编码
                if (!parameter.ResponseEncoding.IsNullOrEmpty())
                    result[item.Key] = Decode(response, parameter.ResponseEncoding);
                else
                    result[item.Key] = response;
            }

            return result;
        }

        /// <summary>
        /// 编码请求数据
        /// </summary>
        /// <param name="value"> </param>
        /// <returns> </returns>
        protected virtual IPacket? Encode(Object? value)
        {
            if (value == null) return null;

            switch (value)
            {
                case IPacket pk:
                    return pk;
                case string str:
                    if (str.StartsWithIgnoreCase("0x"))
                        return new ArrayPacket(str[2..].ToHex());
                    else
                        return new ArrayPacket(str.GetBytes());
                case Byte[] bytes:
                    return new ArrayPacket(bytes);
                default:
                    throw new NotSupportedException($"不支持的数据类型 {value?.GetType().FullName}");
            }
        }

        /// <summary>
        /// 解码响应数据
        /// </summary>
        /// <param name="data">     </param>
        /// <param name="encoding"> </param>
        /// <returns> </returns>
        protected virtual Object? Decode(IPacket? data, string encoding)
        {
            return encoding switch
            {
                "HEX" => data?.ToHex(),
                "ASCII" => data?.ToStr(Encoding.ASCII),
                "UTF8" => data?.ToStr(Encoding.UTF8),
                _ => data,
            };
        }
        #endregion
    }

    /// <summary>
    /// IoT标准通用TCP网络驱动
    /// </summary>
    /// <remarks> IoT驱动,符合IoT标准的通用网络驱动,连接后向目标发送数据即可收到数据. </remarks>
    [Driver("IoTTcp")]
    [DisplayName("通用TCP网络驱动")]
    public class IoTTcpDriver : IoTSocketDriver
    {
        #region 方法
        /// <summary>
        /// 创建网络
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        protected override ISocketClient CreateClient(SocketParameter parameter)
        {
            var uri = new NetUri(NetType.Tcp, parameter.Server, parameter.Port);
            var client = uri.CreateRemote();

            client.Timeout = parameter.Timeout;

            return client;
        }
        #endregion
    }

    /// <summary>
    /// IoT标准通用UDP网络驱动
    /// </summary>
    /// <remarks> IoT驱动,符合IoT标准的通用网络驱动,连接后向目标发送数据即可收到数据. </remarks>
    [Driver("IoTUdp")]
    [DisplayName("通用UDP网络驱动")]
    public class IoTUdpDriver : IoTSocketDriver, IDiscoverableDriver
    {
        #region 方法
        /// <summary>
        /// 创建网络
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        protected override ISocketClient CreateClient(SocketParameter parameter)
        {
            var uri = new NetUri(NetType.Udp, parameter.Server, parameter.Port);
            var client = uri.CreateRemote();

            client.Timeout = parameter.Timeout;

            return client;
        }

        /// <summary>
        /// 异步扫描和发现网络上的兼容设备
        /// </summary>
        /// <param name="parameters">        </param>
        /// <param name="cancellationToken"> </param>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"> </exception>
        public virtual async Task<IEnumerable<IDeviceInfo>> DiscoverAsync(Dictionary<string, Object> parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null || parameters.Count == 0)
                throw new ArgumentNullException(nameof(parameters), "参数不能为空");

            if (!parameters.TryGetValue("Port", out var portObj) || portObj is not Int32 port)
                throw new ArgumentException("参数中必须包含端口号", nameof(parameters));

            var body = "hello";
            if (parameters.TryGetValue("body", out var obj)) body = obj + "";

            var timeout = 3000;
            if (parameters.TryGetValue("Timeout", out var timeoutObj) && timeoutObj is Int32 t) timeout = t;

            var devices = new List<IDeviceInfo>();

            // 获取本机所有网络接口
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToArray();

            var tasks = new List<Task>();

            foreach (var ni in networkInterfaces)
            {
                var ipProperties = ni.GetIPProperties();
                var unicastAddresses = ipProperties.UnicastAddresses
                    .Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)
                    .ToArray();

                foreach (var ua in unicastAddresses)
                {
                    var task = DiscoverOnSubnet(ua.Address, ua.IPv4Mask, port, body, timeout, devices, cancellationToken);
                    tasks.Add(task);
                }
            }

            // 等待所有子网发现完成
            await Task.WhenAll(tasks);

            return devices;
        }

        private async Task DiscoverOnSubnet(IPAddress localIP, IPAddress subnetMask, Int32 port, string body, Int32 timeout, List<IDeviceInfo> devices, CancellationToken cancellationToken)
        {
            try
            {
                // 计算广播地址
                var localBytes = localIP.GetAddressBytes();
                var maskBytes = subnetMask.GetAddressBytes();
                var broadcastBytes = new Byte[4];
                for (var i = 0; i < 4; i++)
                {
                    broadcastBytes[i] = (Byte)(localBytes[i] | (~maskBytes[i]));
                }
                var broadcastAddress = new IPAddress(broadcastBytes);

                using var udpClient = new UdpClient();
                udpClient.Client.Bind(new IPEndPoint(localIP, 0));
                udpClient.EnableBroadcast = true;
                udpClient.Client.ReceiveTimeout = timeout;

                // 发送广播消息
                var data = body.GetBytes();
                var broadcastEndPoint = new IPEndPoint(broadcastAddress, port);
                await udpClient.SendAsync(data, data.Length, broadcastEndPoint);

                WriteLog("UDP广播发送到 {0}:{1}, 内容: {2}", broadcastAddress, port, body);

                // 监听响应
                var endTime = DateTime.Now.AddMilliseconds(timeout);
                while (DateTime.Now < endTime && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var remainingTime = (Int32)(endTime - DateTime.Now).TotalMilliseconds;
                        if (remainingTime <= 0) break;

                        udpClient.Client.ReceiveTimeout = Math.Min(remainingTime, 1000);
                        var result = await udpClient.ReceiveAsync();

                        var responseIP = result.RemoteEndPoint.Address.Tostring();
                        var responsePort = result.RemoteEndPoint.Port;
                        var responseData = result.Buffer;

                        WriteLog("收到来自 {0}:{1} 的响应, 长度: {2}", responseIP, responsePort, responseData.Length);

                        // 构造设备信息
                        var deviceInfo = new DeviceInfo
                        {
                            Code = $"UDP_{responseIP}_{responsePort}",
                            Name = $"UDP设备 {responseIP}:{responsePort}",
                            ProductCode = "IoTUdp",
                            Protocol = "IoTUdp"
                        };

                        // 构造SocketParameter作为设备参数
                        var socketParameter = new SocketParameter
                        {
                            Server = responseIP,
                            Port = responsePort,
                            Timeout = 3000,
                            RequestCommand = body,
                            ResponseEncoding = "UTF8"
                        };

                        // 将参数序列化为JSON字符串
                        deviceInfo.Parameter = socketParameter.ToJson();

                        lock (devices)
                        {
                            // 避免重复添加相同的设备
                            if (!devices.Any(d => d.Code == deviceInfo.Code))
                            {
                                devices.Add(deviceInfo);
                            }
                        }
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        // 超时是正常的,继续等待
                        continue;
                    }
                    catch (Exception ex)
                    {
                        WriteLog("接收UDP响应时出错: {0}", ex.Message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("在子网 {0} 上进行UDP发现时出错: {1}", localIP, ex.Message);
            }
        }
        #endregion
    }

    /// <summary>
    /// 网络节点.多设备共用驱动时,以节点区分
    /// </summary>
    public class SocketNode : Node
    {
        /// <summary>
        /// 客户端
        /// </summary>
        public ISocketClient Client { get; set; } = null!;

        /// <summary>
        /// 参数.设备使用的专用参数
        /// </summary>
        public SocketParameter? SocketParameter { get; set; }
    }

    /// <summary>
    /// 通用网络驱动参数
    /// </summary>
    public class SocketParameter : IDriverParameter, IDriverParameterKey
    {
        /// <summary>
        /// 服务地址.IP地址或域名,例如：127.0.0.1
        /// </summary>
        [Description("服务地址.IP地址或域名,例如：127.0.0.1")]
        public string Server { get; set; } = "127.0.0.1";

        /// <summary>
        /// 端口.例如：5500
        /// </summary>
        [Description("端口.例如：5500")]
        public Int32 Port { get; set; } = 5500;

        /// <summary>
        /// 超时时间.发起请求后等待响应的超时时间,默认3000ms
        /// </summary>
        [Description("超时时间.发起请求后等待响应的超时时间,默认3000ms")]
        public Int32 Timeout { get; set; } = 3000;

        /// <summary>
        /// 请求命令模板.支持十六进制格式(以0x开头)和字符串格式
        /// </summary>
        [Description("请求命令模板.支持十六进制格式(以0x开头)和字符串格式")]
        public string RequestCommand { get; set; } = "";

        /// <summary>
        /// 响应数据编码格式.支持HEX,ASCII,UTF8等
        /// </summary>
        [Description("响应数据编码格式.支持HEX,ASCII,UTF8等")]
        public string ResponseEncoding { get; set; } = "HEX";

        /// <summary>
        /// 获取唯一标识
        /// </summary>
        /// <returns> </returns>
        public string GetKey() => $"{Server}:{Port}";
    }
}

// IoT标准库,定义物联网领域的各种通信协议标准规范,包括驱动接口,物模型,设备控制器等,不含具体实现.用于IoT平台建设及统一硬件驱动协议
namespace Feature.IoT
{
    using Feature.IoT.Models;
    using Feature.IoT.ThingModels;
    using Feature.IoT.ThingSpecification;
    using Feature.Log;
    using Feature.Reflection;

    /// <summary>
    /// 数据处理助手
    /// </summary>
    public static class DataHelper
    {
        #region 数字转字节数组

        /// <summary>
        /// 短整数转为指定字节序的字节数组,仅大小端两种
        /// </summary>
        /// <param name="value">  </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Int16 value, EndianType endian) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, (ByteOrder)endian);

        /// <summary>
        /// 短整数转为指定字节序的字节数组,仅AB/BA两种
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="order"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Int16 value, ByteOrder order) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, order);

        /// <summary>
        /// 短整数转为指定字节序的字节数组,仅大小端两种
        /// </summary>
        /// <param name="value">  </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this UInt16 value, EndianType endian) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, (ByteOrder)endian);

        /// <summary>
        /// 短整数转为指定字节序的字节数组,仅AB/BA两种
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="order"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this UInt16 value, ByteOrder order) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, order);

        /// <summary>
        /// 整数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value">  </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Int32 value, EndianType endian) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, (ByteOrder)endian);

        /// <summary>
        /// 整数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="order"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Int32 value, ByteOrder order) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, order);

        /// <summary>
        /// 整数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value">  </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this UInt32 value, EndianType endian) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, (ByteOrder)endian);

        /// <summary>
        /// 整数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="order"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this UInt32 value, ByteOrder order) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, order);

        /// <summary>
        /// 单精度浮点数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value">  </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Single value, EndianType endian) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, (ByteOrder)endian);

        /// <summary>
        /// 单精度浮点数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="order"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Single value, ByteOrder order) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, order);

        /// <summary>
        /// 双精度浮点数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value">  </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Double value, EndianType endian) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, (ByteOrder)endian);

        /// <summary>
        /// 双精度浮点数转为指定字节序的字节数组
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="order"> </param>
        /// <returns> </returns>
        public static Byte[] GetBytes(this Double value, ByteOrder order) => BitConverter.GetBytes(value).Swap(ByteOrder.DCBA, order);
        #endregion

        #region 字节数组转数字
        /// <summary>
        /// 字节数组按照指定字节序转为短整数,仅大小端两种
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static UInt16 ToUInt16(this Byte[] buffer, EndianType endian) => buffer.ToUInt16(0, endian is EndianType.LittleEndian or EndianType.BigSwap);

        /// <summary>
        /// 字节数组按照指定字节序转为短整数,仅AB/BA两种
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="order">  </param>
        /// <returns> </returns>
        public static UInt16 ToUInt16(this Byte[] buffer, ByteOrder order) => buffer.ToUInt16(0, order is ByteOrder.DCBA or ByteOrder.BADC);

        /// <summary>
        /// 字节数组按照指定字节序转为整数
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static UInt32 ToUInt32(this Byte[] buffer, EndianType endian) => BitConverter.ToUInt32(buffer.Swap(ByteOrder.DCBA, (ByteOrder)endian), 0);

        /// <summary>
        /// 字节数组按指定字节序转为整数
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="order">  </param>
        /// <returns> </returns>
        public static UInt32 ToUInt32(this Byte[] buffer, ByteOrder order) => BitConverter.ToUInt32(buffer.Swap(ByteOrder.DCBA, order), 0);

        /// <summary>
        /// 字节数组按照指定字节序转为单精度浮点数
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Single ToSingle(this Byte[] buffer, EndianType endian) => BitConverter.ToSingle(buffer.Swap(ByteOrder.DCBA, (ByteOrder)endian), 0);

        /// <summary>
        /// 字节数组按指定字节序转为单精度浮点数
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="order">  </param>
        /// <returns> </returns>
        public static Single ToSingle(this Byte[] buffer, ByteOrder order) => BitConverter.ToSingle(buffer.Swap(ByteOrder.DCBA, order), 0);

        /// <summary>
        /// 字节数组按照指定字节序转为双精度浮点数
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Double ToDouble(this Byte[] buffer, EndianType endian) => BitConverter.ToDouble(buffer.Swap(ByteOrder.DCBA, (ByteOrder)endian), 0);

        /// <summary>
        /// 字节数组按指定字节序转为双精度浮点数
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="order">  </param>
        /// <returns> </returns>
        public static Double ToDouble(this Byte[] buffer, ByteOrder order) => BitConverter.ToDouble(buffer.Swap(ByteOrder.DCBA, order), 0);
        #endregion

        #region 字节序交换
        /// <summary>
        /// 按字节序交换字节数组
        /// </summary>
        /// <param name="buf">      字节数组 </param>
        /// <param name="oldOrder"> 原字节序 </param>
        /// <param name="newOrder"> 新字节序 </param>
        /// <returns> </returns>
        public static Byte[] Swap(this Byte[] buf, ByteOrder oldOrder, ByteOrder newOrder)
        {
            // 相同字节序不需要转换
            if (oldOrder == newOrder || newOrder == 0) return buf;

            // 每一种原字节序都支持三种新字节序
            var rs = new Byte[buf.Length];
            newOrder = oldOrder switch
            {
                ByteOrder.ABCD => newOrder,
                ByteOrder.DCBA => newOrder switch
                {
                    ByteOrder.ABCD => ByteOrder.DCBA,
                    ByteOrder.DCBA => ByteOrder.ABCD,
                    ByteOrder.BADC => ByteOrder.CDAB,
                    ByteOrder.CDAB => ByteOrder.BADC,
                    _ => newOrder,
                },
                ByteOrder.BADC => newOrder switch
                {
                    ByteOrder.ABCD => ByteOrder.BADC,
                    ByteOrder.DCBA => ByteOrder.CDAB,
                    ByteOrder.BADC => ByteOrder.ABCD,
                    ByteOrder.CDAB => ByteOrder.DCBA,
                    _ => newOrder,
                },
                ByteOrder.CDAB => newOrder switch
                {
                    ByteOrder.ABCD => ByteOrder.CDAB,
                    ByteOrder.DCBA => ByteOrder.BADC,
                    ByteOrder.BADC => ByteOrder.DCBA,
                    ByteOrder.CDAB => ByteOrder.ABCD,
                    _ => newOrder,
                },
                _ => newOrder,
            };

            switch (newOrder)
            {
                case ByteOrder.ABCD:
                default:
                    for (var i = 0; i < buf.Length; i++)
                    {
                        rs[i] = buf[i];
                    }
                    break;
                case ByteOrder.DCBA:
                    for (var i = 0; i < buf.Length; i++)
                    {
                        rs[i] = buf[buf.Length - i - 1];
                    }
                    break;
                case ByteOrder.BADC:
                    for (var i = 0; i < buf.Length - 1; i += 2)
                    {
                        rs[i] = buf[i + 1];
                        rs[i + 1] = buf[i];
                    }
                    // 奇数个字节时,最后一个字节不变
                    if (buf.Length % 2 == 1)
                        rs[buf.Length - 1] = buf[buf.Length - 1];
                    break;
                case ByteOrder.CDAB:
                    for (var i = 0; i < buf.Length - 1; i += 2)
                    {
                        rs[i] = buf[buf.Length - i - 2];
                        rs[i + 1] = buf[buf.Length - i - 1];
                    }
                    // 奇数个字节时,最后一个字节不变
                    if (buf.Length % 2 == 1)
                        rs[buf.Length - 1] = buf[0];
                    break;
            }

            return rs;
        }

        /// <summary>
        /// 按字节序交换字节数组
        /// </summary>
        /// <param name="buf">   </param>
        /// <param name="order"> </param>
        /// <returns> </returns>
        public static Byte[] Swap(this Byte[] buf, ByteOrder order) => Swap(buf, ByteOrder.ABCD, order);

        /// <summary>
        /// 按字节序交换字节数组
        /// </summary>
        /// <param name="buf">    </param>
        /// <param name="endian"> </param>
        /// <returns> </returns>
        public static Byte[] Swap(this Byte[] buf, EndianType endian) => Swap(buf, ByteOrder.ABCD, (ByteOrder)endian);
        #endregion

        #region 物模型转换
        /// <summary>
        /// 把点位数据编码成为字节数组.常用于Modbus等协议
        /// </summary>
        /// <param name="spec">  物模型 </param>
        /// <param name="data">  数据 </param>
        /// <param name="point"> 点位信息 </param>
        /// <returns> </returns>
        public static Object? Encode(this ThingSpec spec, Object data, IPoint point)
        {
            var type = point.GetNetType();
            if (type == null)
            {
                var pi = spec?.GetProperty(point.Name);
                type = TypeHelper.GetNetType(pi?.DataType?.Type);

                if (type == null) return data;
            }

            using var span = DefaultTracer.Instance?.NewSpan(nameof(Encode), $"name={point.Name} data={data} type={type.Name} rawType={point.Type}");
            try
            {
                // 找到物属性定义
                var pi = spec?.GetProperty(point.Name);
                if (pi != null)
                {
                    var rs = pi.Encode(data, type);
                    if (rs != null) return rs;
                }

                return point.GetBytes(data);
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                throw;
                //return null;
            }
        }

        /// <summary>
        /// 用物属性把点位数据编码成为字节数组
        /// </summary>
        /// <param name="propertySpec"> 物属性 </param>
        /// <param name="data">         数据 </param>
        /// <param name="type">         点位类型 </param>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"> </exception>
        public static Byte[]? Encode(this PropertySpec propertySpec, Object data, Type? type = null)
        {
            type ??= TypeHelper.GetNetType(propertySpec.DataType?.Type);
            if (type == null) return null;

            var span = DefaultSpan.Current;
            var scaling = propertySpec.Scaling;
            var constant = propertySpec.Constant;
            var order = propertySpec?.DataType?.Specs?.Order ?? 0;

            // 反向操作常量因子和缩放因子
            if (type.IsInt())
            {
                var v = data.ToLong();
                //if (constant != 0 || scaling != 1) v = (Int64)Math.Round(v * scaling + constant);
                // 编码是反向操作,先减去常量,再除以缩放因子.为了避免精度问题,单精度范围先计算缩放因子倒数,再相乘
                if (constant != 0 || scaling != 1) v = (Int64)Math.Round(((Double)v - constant) * (1 / scaling));

                // 常见的2字节和4字节整型,直接转字节数组返回
                var rs = type.GetTypeCode() switch
                {
                    TypeCode.Byte or TypeCode.SByte => [(Byte)v],
                    TypeCode.Int16 or TypeCode.UInt16 => ((UInt16)v).GetBytes(order),
                    TypeCode.Int32 or TypeCode.UInt32 => ((UInt32)v).GetBytes(order),
                    _ => throw new NotImplementedException(),
                };
                span?.AppendTag($"result={(rs is Byte[] bts ? bts.ToHex() : rs)} v={v} type={type.Name} scaling={scaling}");

                return rs;
            }
            else if (type == typeof(bool))
            {
                var rs = data.Tobool();
                span?.AppendTag($"result={rs}");

                return rs ? [0] : [1];
            }
            else if (type == typeof(Single))
            {
                var v = (Single)data.ToDouble();
                if (constant > 0) v -= constant;
                if (scaling != 0 && scaling != 1) v /= scaling;

                span?.AppendTag($"result={v} type={type.Name} scaling={scaling} constant={constant}");

                // 按照小端读取出来,如果不是小端,则需要交换字节序
                return v.GetBytes(order);
            }
            else if (type == typeof(Double))
            {
                var v = data.ToDouble();
                if (constant > 0) v -= constant;
                if (scaling != 0 && scaling != 1) v /= scaling;

                span?.AppendTag($"result={v} type={type.Name} scaling={scaling} constant={constant}");

                // 按照小端读取出来,如果不是小端,则需要交换字节序
                return v.GetBytes(order);
            }

            return null;
        }

        /// <summary>
        /// 把点位数据编码成为字节数组.常用于Modbus等协议
        /// </summary>
        /// <param name="spec"> 物模型 </param>
        /// <param name="data"> 数据 </param>
        /// <param name="name"> 点位名称 </param>
        /// <returns> </returns>
        public static Object? Encode(this ThingSpec spec, Object data, string name) => Encode(spec, data, new PointModel { Name = name });

        /// <summary>
        /// 借助物模型解析数值
        /// </summary>
        /// <param name="spec">  物模型 </param>
        /// <param name="data">  数据 </param>
        /// <param name="point"> 点位信息 </param>
        /// <returns> </returns>
        public static Object? Decode(this ThingSpec spec, Byte[] data, IPoint point)
        {
            var type = point.GetNetType();
            if (type == null)
            {
                var pi = spec?.GetProperty(point.Name);
                type = TypeHelper.GetNetType(pi?.DataType?.Type);

                if (type == null) return data;
            }
            if (type == typeof(Byte[]) || !type.IsNumber()) return data;

            using var span = DefaultTracer.Instance?.NewSpan(nameof(Decode), $"name={point.Name} data={data.ToHex()} type={type.Name} rawType={point.Type}");
            try
            {
                // 找到物属性定义
                var pi = spec?.GetProperty(point.Name);
                if (pi != null)
                {
                    var rs = pi.Decode(data, type);
                    if (rs != null) return rs;
                }

                return point.Convert(data);
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                throw;
                //return null;
            }
        }

        /// <summary>
        /// 用物属性把字节数据解码成为点位数据
        /// </summary>
        /// <param name="propertySpec"> 物属性 </param>
        /// <param name="data">         数据 </param>
        /// <param name="type">         点位类型 </param>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"> </exception>
        public static Object? Decode(this PropertySpec propertySpec, Byte[] data, Type? type = null)
        {
            type ??= TypeHelper.GetNetType(propertySpec.DataType?.Type);
            if (type == null) return null;

            if (type == typeof(bool)) return data[0] > 0;
            if (type == typeof(Byte)) return data[0];

            var span = DefaultSpan.Current;
            var scaling = propertySpec.Scaling;
            var constant = propertySpec.Constant;
            var order = propertySpec?.DataType?.Specs?.Order ?? 0;

            // 操作常量因子和缩放因子
            if (type.IsInt())
            {
                // 常见的2字节和4字节整型,直接转
                var rs = type.GetTypeCode() switch
                {
                    TypeCode.Int16 or TypeCode.UInt16 => data.ToUInt16(order),
                    TypeCode.Int32 or TypeCode.UInt32 => data.ToUInt32(order),
                    _ => throw new NotImplementedException(),
                };
                if (constant != 0 || scaling != 1) rs = (UInt32)Math.Round(rs * scaling + constant);
                span?.AppendTag($"result={rs} type={type.Name}");

                return rs.ChangeType(type);
            }
            else if (type == typeof(Single))
            {
                // 如果未指定字节序,且使用了缩放因子,则先转换成大端整数.常见Modbus寄存器保存浮点数
                if (order == 0 && (constant != 0 || scaling != 1))
                {
                    var rs = data.ToUInt16(EndianType.BigEndian);
                    return rs * scaling + constant;
                }
                else
                {
                    var rs = data.ToSingle(order);
                    if (constant != 0 || scaling != 1)
                    {
                        // 计算小数精度
                        var digits = 2;
                        var step = propertySpec?.DataType?.Specs?.Step ?? 0;
                        if (step > 0)
                        {
                            var str = step.Tostring();
                            var idx = str.IndexOf('.');
                            if (idx >= 0) digits = str.Length - idx - 1;
                        }

                        rs = (Single)Math.Round(rs * scaling + constant, digits);
                    }
                    return rs;
                }
            }
            else if (type == typeof(Double))
            {
                // 如果未指定字节序,且使用了缩放因子,则先转换成大端整数.常见Modbus寄存器保存浮点数
                if (order == 0 && (constant != 0 || scaling != 1))
                {
                    var rs = data.ToUInt32(EndianType.BigEndian);
                    return (Double)(rs * scaling + constant);
                }
                else
                {
                    var rs = data.ToDouble(order);
                    if (constant != 0 || scaling != 1)
                    {
                        // 计算小数精度
                        var digits = 4;
                        var step = propertySpec?.DataType?.Specs?.Step ?? 0;
                        if (step > 0)
                        {
                            var str = step.Tostring();
                            var idx = str.IndexOf('.');
                            if (idx >= 0) digits = str.Length - idx - 1;
                        }

                        rs = Math.Round(rs * scaling + constant, digits);
                    }
                    return rs;
                }
            }

            return null;
        }

        /// <summary>
        /// 借助物模型解析数值
        /// </summary>
        /// <param name="spec"> 物模型 </param>
        /// <param name="data"> 数据 </param>
        /// <param name="name"> 点位名称 </param>
        /// <returns> </returns>
        public static Object? Decode(this ThingSpec spec, Byte[] data, string name) => Decode(spec, data, new PointModel { Name = name });
        #endregion
    }

    /// <summary>
    /// 逻辑设备接口.具备物模型全部能力,对接物理设备进行数据采集及远程控制
    /// </summary>
    /// <remarks> IDevice 接口乃是IoT客户端核心,它的不同程度实现,可以建立轻量级或重量级物联网平台. </remarks>
    public interface IDevice
    {
        #region 属性
        /// <summary>
        /// 设备编码.在平台中唯一标识设备
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// 属性集合
        /// </summary>
        IDictionary<string, Object?> Properties { get; }

        /// <summary>
        /// 产品物模型.自定义客户端或驱动插件内部,可借助物模型去规范所需要采集的数据
        /// </summary>
        ThingSpec? Specification { get; set; }

        /// <summary>
        /// 点位集合
        /// </summary>
        IPoint[]? Points { get; set; }

        /// <summary>
        /// 服务集合
        /// </summary>
        IDictionary<string, Delegate> Services { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 开始工作
        /// </summary>
        Task Start();

        /// <summary>
        /// 停止工作
        /// </summary>
        void Stop();

        /// <summary>
        /// 设备上线.驱动打开后调用,子设备发现,或者上报主设备/子设备的默认参数模版
        /// </summary>
        /// <remarks> 有些设备驱动具备扫描发现子设备能力,通过该方法上报设备. 主设备或子设备,也可通过该方法上报驱动的默认参数模版. 根据需要,驱动内可能多次调用该方法. </remarks>
        /// <param name="devices"> 设备信息集合.可传递参数模版 </param>
        /// <returns> 返回上报信息对应的反馈,如果新增子设备,则返回子设备信息 </returns>
        IDeviceInfo[] SetOnline(IDeviceInfo[] devices);

        /// <summary>
        /// 设备下线.驱动内子设备变化后调用
        /// </summary>
        /// <remarks> 根据需要,驱动内可能多次调用该方法. </remarks>
        /// <param name="devices"> 设备编码集合.用于子设备离线 </param>
        /// <returns> 返回上报信息对应的反馈,如果新增子设备,则返回子设备信息 </returns>
        IDeviceInfo[] SetOffline(string[] devices);
        #endregion

        #region 物模型方法
        /// <summary>
        /// 马上上报属性
        /// </summary>
        void PostProperty();

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name">  </param>
        /// <param name="value"> </param>
        void SetProperty(string name, Object? value);

        /// <summary>
        /// 添加自定义数据,批量上传
        /// </summary>
        /// <param name="name">  </param>
        /// <param name="value"> </param>
        bool AddData(string name, string value);

        /// <summary>
        /// 写事件
        /// </summary>
        /// <param name="type">   </param>
        /// <param name="name">   </param>
        /// <param name="remark"> </param>
        bool WriteEvent(string type, string name, string remark);

        ///// <summary>
        ///// 获取影子
        ///// </summary>
        ///// <remarks>每次获取上一次值,并异步更新</remarks>
        //string GetShadow();

        ///// <summary>
        ///// 设置影子,马上上报
        ///// </summary>
        ///// <param name="shadow"></param>
        //void SetShadow(Object shadow);

        ///// <summary>
        ///// 获取配置
        ///// </summary>
        ///// <remarks>首次获取时启动定时器,确保后续读到新数据</remarks>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //Object GetConfig(string name);
        #endregion

        #region 服务控制
        /// <summary>
        /// 注册服务.收到平台下发的服务调用时,执行注册的方法
        /// </summary>
        /// <param name="service"> </param>
        /// <param name="method">  </param>
        void RegisterService(string service, Delegate method);
        #endregion
    }

    /// <summary>
    /// 物模型扩展
    /// </summary>
    public static class ThingExtensions
    {
        /// <summary>
        /// 写信息事件
        /// </summary>
        /// <param name="name">   </param>
        /// <param name="remark"> </param>
        public static void WriteInfoEvent(this IDevice device, string name, string remark) => device.WriteEvent("info", name, remark);

        /// <summary>
        /// 写警告事件
        /// </summary>
        /// <param name="name">   </param>
        /// <param name="remark"> </param>
        public static void WriteAlertEvent(this IDevice device, string name, string remark) => device.WriteEvent("alert", name, remark);

        /// <summary>
        /// 写错误事件
        /// </summary>
        /// <param name="name">   </param>
        /// <param name="remark"> </param>
        public static void WriteErrorEvent(this IDevice device, string name, string remark) => device.WriteEvent("error", name, remark);
    }

    /// <summary>
    /// 类型助手.处理IoT数据中的各种类型
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// 获取指定类型的数据长度
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        public static Int32 GetLength(string? type)
        {
            if (type.IsNullOrEmpty()) return 0;

            return type.ToLower() switch
            {
                "bit" or "bool" or "bool" or "char" or "byte" or "sbyte" => 1,
                "short" or "ushort" or "int16" or "uint16" or "number" => 2,
                "int" or "uint" or "int32" or "uint32" or "float" or "single" => 4,
                "long" or "ulong" or "int64" or "uint64" or "double" or "decimal" => 8,
                _ => 0,
            };
        }

        /// <summary>
        /// 获取指定类型的数据长度
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        public static Int32 GetLength(Type? type)
        {
            if (type == null) return 0;

            return type.GetTypeCode() switch
            {
                TypeCode.bool => 1,
                TypeCode.Char or TypeCode.Byte or TypeCode.SByte => 1,
                TypeCode.Int16 or TypeCode.UInt16 => 2,
                TypeCode.Int32 or TypeCode.UInt32 => 4,
                TypeCode.Int64 or TypeCode.UInt64 => 8,
                TypeCode.Single => 4,
                TypeCode.Double or TypeCode.Decimal => 8,
                TypeCode.string => 0,
                TypeCode.DateTime => 4,
                _ => 0,
            };
        }

        /// <summary>
        /// 获取点位数据长度,若未设置则根据类型自动计算
        /// </summary>
        /// <param name="point"> </param>
        /// <returns> </returns>
        public static Int32 GetLength(this IPoint point) => point.Length > 0 ? point.Length : GetLength(point.Type);

        /// <summary>
        /// 获取指定IoT类型的本地类型.可用于格式化各种非标类型
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"> </exception>
        public static Type? GetNetType(string? type)
        {
            if (type.IsNullOrEmpty()) return null;

            return type.ToLower() switch
            {
                "bit" or "bool" or "bool" => typeof(bool),
                "char" => typeof(Char),
                "byte" or "sbyte" => typeof(Byte),
                "short" or "int16" or "number" => typeof(Int16),
                "ushort" or "uint16" => typeof(UInt16),
                "int" or "int32" => typeof(Int32),
                "uint" or "uint32" => typeof(UInt32),
                "float" or "single" => typeof(Single),
                "long" or "int64" => typeof(Int64),
                "ulong" or "uint64" => typeof(UInt64),
                "double" => typeof(Double),
                "decimal" => typeof(Decimal),
                "string" or "text" => typeof(string),
                "date" or "time" or "datetime" => typeof(DateTime),
                "byte[]" or "hex" => typeof(Byte[]),
                _ => null,
            };
        }

        /// <summary>
        /// 获取指定点位的本地类型,依赖于点位IoT类型和长度.可用于格式化各种非标类型
        /// </summary>
        /// <param name="point"> </param>
        /// <returns> </returns>
        public static Type? GetNetType(this IPoint point)
        {
            if ((point?.Type).IsNullOrEmpty()) return null;

            var type = GetNetType(point.Type);
            if (type == null) return null;

            if (point.Length > 0)
            {
                // 如果长度一致,直接返回
                if (point.Length == GetLength(type)) return type;

                // 数字类型,最终类型取决于长度.有的场景习惯用2字节int
                if (type.IsInt())
                {
                    return point.Length switch
                    {
                        1 => typeof(Byte),
                        2 => typeof(Int16),
                        3 or 4 => typeof(Int32),
                        _ => type,
                    };
                }
                // 小数类型,最终类型取决于长度.有的场景习惯用2字节float或4字节double
                else if (type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal))
                {
                    return point.Length <= 4 ? typeof(Single) : typeof(Double);
                }
            }

            return type;
        }

        /// <summary>
        /// 设置点位的IoT类型和长度
        /// </summary>
        /// <param name="point"> </param>
        /// <param name="type">  </param>
        public static void SetNetType(this IPoint point, Type type)
        {
            point.Type = GetIoTType(type);
            point.Length = GetLength(type);
        }

        /// <summary>
        /// 获取指定类型的IoT类型,简化可用类型.可用于格式化各种非标类型
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="full"> 是否返回完成类型,默认false返回精简类型 </param>
        /// <returns> </returns>
        public static string? GetIoTType(Type? type, bool full = false)
        {
            if (type == null) return null;

            if (full)
            {
                if (type == typeof(Byte[])) return "hex";

                return type.GetTypeCode() switch
                {
                    TypeCode.bool => "bool",
                    TypeCode.Char or TypeCode.Byte or TypeCode.SByte => "byte",
                    TypeCode.Int16 or TypeCode.UInt16 => "short",
                    TypeCode.Int32 or TypeCode.UInt32 => "int",
                    TypeCode.Int64 or TypeCode.UInt64 => "long",
                    TypeCode.Single => "float",
                    TypeCode.Double or TypeCode.Decimal => "double",
                    TypeCode.string => "text",
                    TypeCode.DateTime => "time",
                    _ => type?.Name.ToLower(),
                };
            }
            else
            {
                return type.GetTypeCode() switch
                {
                    TypeCode.bool => "bool",
                    TypeCode.Char or TypeCode.Byte or TypeCode.SByte => "int",
                    TypeCode.Int16 or TypeCode.UInt16 => "int",
                    TypeCode.Int32 or TypeCode.UInt32 => "int",
                    TypeCode.Int64 or TypeCode.UInt64 => "int",
                    TypeCode.Single => "float",
                    TypeCode.Double or TypeCode.Decimal => "float",
                    TypeCode.string => "text",
                    //TypeCode.DateTime => "time",
                    _ => null,
                };
            }
        }

        /// <summary>
        /// 获取指定点位的标准IoT类型,依据原类型及长度
        /// </summary>
        /// <param name="point"> </param>
        /// <param name="full">  是否返回完成类型,默认false返回精简类型 </param>
        /// <returns> </returns>
        public static string? GetIoTType(this IPoint point, bool full = false)
        {
            var type = point.GetNetType();
            return GetIoTType(type, full);
        }

        private static IDictionary<string, string>? _fullTypes;
        private static IDictionary<string, string>? _iotTypes;
        /// <summary>
        /// 获取所有可用IoT类型
        /// </summary>
        /// <param name="full"> 是否返回完成类型,默认false返回精简类型 </param>
        /// <returns> </returns>
        public static IDictionary<string, string> GetIoTTypes(bool full = false)
        {
            if (full)
            {
                if (_fullTypes != null) return _fullTypes;

                var dic = new Dictionary<string, string>
                {
                    ["short"] = "短整数",
                    ["int"] = "整数",
                    ["float"] = "小数",
                    ["bool"] = "布尔型",
                    ["byte"] = "字节",
                    ["long"] = "长整数",
                    ["double"] = "双精度",
                    ["text"] = "文本",
                    ["time"] = "时间",
                };

                return _fullTypes = dic;
            }
            else
            {
                if (_iotTypes != null) return _iotTypes;

                var dic = new Dictionary<string, string>
                {
                    ["int"] = "整数",
                    ["float"] = "小数",
                    ["bool"] = "布尔型",
                    ["text"] = "文本",
                };

                return _iotTypes = dic;
            }
        }
    }
}

// IoT协议驱动接口及基类
namespace Feature.IoT.Drivers
{
    using Feature.IoT.Models;
    using Feature.IoT.ThingModels;
    using Feature.IoT.ThingSpecification;
    using Feature.Log;
    using Feature.Reflection;
    using Feature.Serialization;
    using Feature.Xml;
    using System.Collections.Concurrent;
    using System.Reflection;
#if !NET40
    using TaskEx = System.Threading.Tasks.Task;
#endif

    /// <summary>
    /// 异步协议驱动基类.抽象各种硬件设备的数据采集及远程控制
    /// </summary>
    /// <typeparam name="TNode"> 节点类型,可使用默认Node </typeparam>
    /// <typeparam name="TParameter"> </typeparam>
    public class AsyncDriverBase<TNode, TParameter> : AsyncDriverBase
        where TNode : INode, new()
        where TParameter : IDriverParameter, new()
    {
        #region 元数据
        /// <summary>
        /// 创建驱动参数对象,分析参数配置或创建默认参数
        /// </summary>
        /// <returns> </returns>
        protected override IDriverParameter OnCreateParameter() => new TParameter();
        #endregion

        #region 核心方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">            逻辑设备 </param>
        /// <param name="parameter">         参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public override Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
        {
            var node = new TNode
            {
                Driver = this,
                Device = device,
                Parameter = parameter,
            };

            return TaskEx.FromResult(node as INode);
        }

        ///// <summary>
        ///// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        ///// </summary>
        ///// <param name="device">逻辑设备</param>
        ///// <param name="parameter">参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter</param>
        ///// <returns>节点对象,可存储站号等信息,仅驱动自己识别</returns>
        //public override INode Open(IDevice device, IDriverParameter? parameter)
        //{
        //    var node = new TNode
        //    {
        //        Driver = this,
        //        Device = device,
        //        Parameter = parameter,
        //    };

        //    return node;
        //}
        #endregion
    }

    /// <summary>
    /// 异步协议驱动基类.抽象各种硬件设备的数据采集及远程控制
    /// </summary>
    /// <remarks>
    /// 在Modbus协议上,一个通信链路（串口/ModbusTcp地址）即是IDriver,可能有多个物理设备共用,各自表示为INode. 即使是一个物理设备,也可能因为管理需要而划分为多个逻辑设备,例如变配电网关等Modbus汇集网关.
    ///
    /// 架构设计需要,本类继承自DriverBase,将来可能移除该继承关系.
    /// </remarks>
    public abstract class AsyncDriverBase : DriverBase, IAsyncDriver
    {
        #region 核心方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">            逻辑设备 </param>
        /// <param name="parameter">         参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public virtual Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
        {
            var node = new Node
            {
                Driver = this,
                Device = device,
            };

            return TaskEx.FromResult(node as INode);
        }

        /// <summary>
        /// 关闭设备节点.多节点共用通信链路时,需等最后一个节点关闭才能断开
        /// </summary>
        /// <param name="node">              </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
#if NET40 || NET45
    public virtual Task CloseAsync(INode node, CancellationToken cancellationToken = default) => TaskEx.FromResult(0);
#else
        public virtual Task CloseAsync(INode node, CancellationToken cancellationToken = default) => TaskEx.CompletedTask;
#endif

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <remarks> 驱动实现数据采集的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points">            点位集合 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        /// <returns> </returns>
        public virtual Task<IDictionary<string, Object?>> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="point">             点位 </param>
        /// <param name="value">             数值 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        public virtual Task<Object?> WriteAsync(INode node, IPoint point, Object? value, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="values">            点位数值 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        public virtual async Task WriteAsync(INode node, IDictionary<IPoint, Object> values, CancellationToken cancellationToken = default)
        {
            foreach (var item in values)
            {
                await WriteAsync(node, item.Key, item.Value, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 控制设备,特殊功能使用
        /// </summary>
        /// <remarks> 除了点位读写之外的其它控制功能. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="parameters">        参数 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        public virtual Task<Object?> ControlAsync(INode node, IDictionary<string, Object?> parameters, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        #endregion

        #region 覆盖同步接口
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="parameter"> 参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public override INode Open(IDevice device, IDriverParameter? parameter) => OpenAsync(device, parameter).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// 关闭设备节点.多节点共用通信链路时,需等最后一个节点关闭才能断开
        /// </summary>
        /// <param name="node"> </param>
        public override void Close(INode node) => CloseAsync(node).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <remarks> 驱动实现数据采集的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points"> 点位集合 </param>
        /// <returns> </returns>
        public override IDictionary<string, Object?> Read(INode node, IPoint[] points) => ReadAsync(node, points).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">  节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="point"> 点位 </param>
        /// <param name="value"> 数值 </param>
        public override Object? Write(INode node, IPoint point, Object? value) => WriteAsync(node, point, value).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="values"> 点位数值 </param>
        public override void Write(INode node, IDictionary<IPoint, Object> values) => WriteAsync(node, values).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// 控制设备,特殊功能使用
        /// </summary>
        /// <remarks> 除了点位读写之外的其它控制功能. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">       节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="parameters"> 参数 </param>
        public override Object? Control(INode node, IDictionary<string, Object?> parameters) => ControlAsync(node, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
        #endregion
    }

    /// <summary>
    /// 驱动特性
    /// </summary>
    /// <param name="name"> 驱动名称 </param>
    [AttributeUsage(AttributeTargets.Class)]
    public class DriverAttribute(string name) : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = name;
    }

    /// <summary>
    /// 协议驱动基类.抽象各种硬件设备的数据采集及远程控制
    /// </summary>
    /// <typeparam name="TNode"> 节点类型,可使用默认Node </typeparam>
    /// <typeparam name="TParameter"> </typeparam>
    public class DriverBase<TNode, TParameter> : DriverBase
        where TNode : INode, new()
        where TParameter : IDriverParameter, new()
    {
        #region 元数据
        /// <summary>
        /// 创建驱动参数对象,分析参数配置或创建默认参数
        /// </summary>
        /// <returns> </returns>
        protected override IDriverParameter OnCreateParameter() => new TParameter();
        #endregion

        #region 核心方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="parameter"> 参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public override INode Open(IDevice device, IDriverParameter? parameter)
        {
            var node = new TNode
            {
                Driver = this,
                Device = device,
                Parameter = parameter,
            };

            return node;
        }
        #endregion
    }

    /// <summary>
    /// 协议驱动基类.抽象各种硬件设备的数据采集及远程控制
    /// </summary>
    /// <remarks> 在Modbus协议上,一个通信链路（串口/ModbusTcp地址）即是IDriver,可能有多个物理设备共用,各自表示为INode. 即使是一个物理设备,也可能因为管理需要而划分为多个逻辑设备,例如变配电网关等Modbus汇集网关. </remarks>
    public abstract class DriverBase : DisposeBase, IDriver, ILogFeature, ITracerFeature
    {
        #region 属性
        /// <summary>
        /// 服务提供者.驱动可在此取得外部注入到容器中的服务对象
        /// </summary>
        /// <remarks> 例如：Modbus驱动可以获取外部注入的IBoard服务对象,在A2工业计算机中,借助其中的Map方法把串口COM1映射到/dev/ttyAMA0. </remarks>
        public IServiceProvider? ServiceProvider { get; set; }
        #endregion

        #region 元数据
        /// <summary>
        /// 创建驱动参数对象,分析参数配置或创建默认参数
        /// </summary>
        /// <remarks> 可序列化成Xml/Json作为该协议的参数模板.由于Xml需要良好的注释特性,优先使用. 获取后,按新版本覆盖旧版本. </remarks>
        /// <param name="parameter"> Xml/Json参数配置 </param>
        /// <returns> </returns>
        public virtual IDriverParameter? CreateParameter(string? parameter)
        {
            var p = OnCreateParameter();
            if (p == null) return null;

            if (!parameter.IsNullOrEmpty())
            {
                // 按Xml或Json解析参数成为字典
                var ps = parameter.StartsWith("<") && parameter.EndsWith(">") ?
                    XmlParser.Decode(parameter) :
                    JsonParser.Decode(parameter);

                // 字段转对象
                new JsonReader().ToObject(ps, null, p);
            }

            return p;
        }

        /// <summary>
        /// 创建驱动参数对象,分析参数配置或创建默认参数
        /// </summary>
        /// <returns> </returns>
        protected virtual IDriverParameter? OnCreateParameter() => null;

        /// <summary>
        /// 获取产品物模型
        /// </summary>
        /// <remarks> 如果设备有固定点位属性,服务和事件,则直接返回,否则返回空. 物联网平台有两种情况调用该接口： 1,打开设备后.常见于OPC/BACnet等,此时可获取特定设备场景的物模型. 2,扫描设备时.此时未连接任何设备,只能返回该类设备的通用物模型,常用于具体硬件产品,例如各种传感器. 获取后,按新版本覆盖旧版本. </remarks>
        /// <returns> </returns>
        public virtual ThingSpec? GetSpecification()
        {
            var type = GetType();
            var spec = new ThingSpec
            {
                Profile = new Profile
                {
                    Version = type.Assembly.GetName().Version + "",
                    ProductKey = type.GetCustomAttribute<DriverAttribute>()?.Name ?? type.Name.TrimEnd("Protocol", "Driver")
                }
            };

            return OnGetSpecification(spec) ? spec : null;
        }

        /// <summary>
        /// 填充产品物模型
        /// </summary>
        /// <param name="thingSpec"> </param>
        /// <returns> 是否填充成功 </returns>
        protected virtual bool OnGetSpecification(ThingSpec thingSpec) => false;
        #endregion

        #region 核心方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="parameter"> 参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        public virtual INode Open(IDevice device, IDriverParameter? parameter)
        {
            var node = new Node
            {
                Driver = this,
                Device = device,
            };

            return node;
        }

        /// <summary>
        /// 关闭设备节点.多节点共用通信链路时,需等最后一个节点关闭才能断开
        /// </summary>
        /// <param name="node"> </param>
        public virtual void Close(INode node) { }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <remarks> 驱动实现数据采集的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points"> 点位集合 </param>
        /// <returns> </returns>
        public virtual IDictionary<string, Object?> Read(INode node, IPoint[] points) => throw new NotImplementedException();

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">  节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="point"> 点位 </param>
        /// <param name="value"> 数值 </param>
        public virtual Object? Write(INode node, IPoint point, Object? value) => throw new NotImplementedException();

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="values"> 点位数值 </param>
        public virtual void Write(INode node, IDictionary<IPoint, Object> values)
        {
            foreach (var item in values)
            {
                Write(node, item.Key, item.Value);
            }
        }

        /// <summary>
        /// 控制设备,特殊功能使用
        /// </summary>
        /// <remarks> 除了点位读写之外的其它控制功能. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">       节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="parameters"> 参数 </param>
        public virtual Object? Control(INode node, IDictionary<string, Object?> parameters) => throw new NotImplementedException();
        #endregion

        #region 日志
        /// <summary>
        /// 日志
        /// </summary>
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>
        /// 性能追踪器
        /// </summary>
        public ITracer? Tracer { get; set; }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="format"> </param>
        /// <param name="args">   </param>
        public void WriteLog(string format, params Object[] args) => Log?.Info(format, args);
        #endregion
    }

    /// <summary>
    /// 驱动工厂.根据协议名称创建实例
    /// </summary>
    public class DriverFactory
    {
        #region 工厂
        private static readonly Dictionary<string, Type> _map = new(stringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 驱动表
        /// </summary>
        public static IDictionary<string, Type> Map => _map;

        /// <summary>
        /// 驱动集合
        /// </summary>
        public static IList<DriverInfo> Drivers { get; private set; } = null!;

        /// <summary>
        /// 注册协议实现
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="type"> </param>
        public static void Register(string? name, Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name.IsNullOrEmpty()) name = type.Name;

            _map[name] = type;
        }

        /// <summary>
        /// 注册协议实现
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="name"> </param>
        public static void Register<T>(string? name = null) where T : IDriver => Register(name, typeof(T));

        private static readonly ConcurrentDictionary<string, IDriver?> _cache = new();

        /// <summary>
        /// 创建协议实例,根据地址确保唯一,多设备共用同一个串口
        /// </summary>
        /// <param name="name">       驱动名称.一般由DriverAttribute特性确定 </param>
        /// <param name="identifier"> 唯一标识.没有传递标识时,每次返回新实例 </param>
        /// <returns> </returns>
        public static IDriver? Create(string name, string identifier)
        {
            if (!_map.TryGetValue(name, out var type) || type == null) return null;

            // 没有传递标识时,每次返回新实例
            if (identifier.IsNullOrEmpty()) return type.CreateInstance() as IDriver;

            var key = $"{name}-{identifier}";
            return _cache.GetOrAdd(key, k => type.CreateInstance() as IDriver);
        }

        private static readonly ConcurrentDictionary<string, IDriver?> _defaults = new();

        /// <summary>
        /// 创建驱动参数对象,分析参数配置或创建默认参数
        /// </summary>
        /// <param name="name">      驱动名称.一般由DriverAttribute特性确定 </param>
        /// <param name="parameter"> Xml/Json参数配置 </param>
        /// <returns> </returns>
        public static IDriverParameter? CreateParameter(string name, string parameter)
        {
            if (!_map.TryGetValue(name, out var type) || type == null) return null;

            var driver = _defaults.GetOrAdd(name, k => type.CreateInstance() as IDriver);
            return driver?.CreateParameter(parameter);
        }
        #endregion

        #region 插件
        /// <summary>
        /// 扫描所有程序集,加载插件
        /// </summary>
        public static void ScanAll()
        {
            XTrace.WriteLine("================开始扫描驱动插件================");

            var iot = Assembly.GetExecutingAssembly();
            var iotVersion = iot.GetName().Version + "";

            var list = new List<DriverInfo>();
            foreach (var type in AssemblyX.FindAllPlugins(typeof(IDriver), true, true))
            {
                var att = type.GetCustomAttribute<DriverAttribute>();
                var name = att?.Name ?? type.Name.TrimEnd("Procotol", "Driver");

                var name2 = $"[{name}]";
                XTrace.WriteLine("{0,16}\t{1,-14}\t{2}", name2, type.GetDisplayName(), type.FullName);

                Register(name, type);

                var info = new DriverInfo
                {
                    Name = name,
                    DisplayName = type.GetDisplayName(),
                    Type = ".NET",
                    ClassName = type.FullName,
                    Version = type.Assembly.GetName().Version + "",
                    IoTVersion = iotVersion,
                    Description = type.GetDescription(),
                };

                try
                {
                    var driver = type.CreateInstance() as IDriver;
                    var driverParameter = driver?.CreateParameter();
                    info.ParameterClassName = driverParameter?.GetType().FullName;

                    // Xml序列化,去掉前面的BOM编码
                    info.DefaultParameter = driverParameter?.EncodeParameter();

                    info.Specification = driver?.GetSpecification();
                }
                catch { }

                list.Add(info);
            }

            Drivers = list;

            XTrace.WriteLine("================结束扫描驱动插件================");
        }
        #endregion
    }

    /// <summary>
    /// 驱动信息
    /// </summary>
    public class DriverInfo
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 显示名
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 类型.编程语言等,例如.NET
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 类型名
        /// </summary>
        public string? ClassName { get; set; }

        /// <summary>
        /// 参数类型名
        /// </summary>
        public string? ParameterClassName { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 该驱动所依赖的IoT版本
        /// </summary>
        public string? IoTVersion { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 默认参数.可作为设备参数模版,Xml格式带注释
        /// </summary>
        public string? DefaultParameter { get; set; }

        /// <summary>
        /// 产品物模型.如果设备有固定点位属性,服务和事件,则直接返回,否则返回空
        /// </summary>
        public ThingSpec? Specification { get; set; }
        #endregion

        /// <summary>
        /// 友好显示名称
        /// </summary>
        /// <returns> </returns>
        public override string Tostring() => !DisplayName.IsNullOrEmpty() ? DisplayName : Name;
    }

    /// <summary>
    /// 协议驱动异步接口.抽象各种硬件设备的数据采集及远程控制
    /// </summary>
    /// <remarks>
    /// 在Modbus协议上,一个通信链路（串口/ModbusTcp地址）即是IDriver,可能有多个物理设备共用,各自表示为INode. 即使是一个物理设备,也可能因为管理需要而划分为多个逻辑设备,例如变配电网关等Modbus汇集网关.
    ///
    /// 除了具体设备实例化驱动对象,在物联网平台扫描驱动时,也有可能实例化驱动对象,以获取默认参数与产品物模型.
    /// </remarks>
    public interface IAsyncDriver
    {
        #region 元数据
        /// <summary>
        /// 创建驱动参数对象,分析参数配置或创建默认参数
        /// </summary>
        /// <remarks> 可序列化成Xml/Json作为该协议的参数模板.由于Xml需要良好的注释特性,优先使用. 获取后,按新版本覆盖旧版本. </remarks>
        /// <param name="parameter"> Xml/Json参数配置,为空时仅创建默认参数 </param>
        /// <returns> </returns>
        IDriverParameter? CreateParameter(string? parameter = null);

        /// <summary>
        /// 获取产品物模型
        /// </summary>
        /// <remarks> 如果设备有固定点位属性,服务和事件,则直接返回,否则返回空. 物联网平台有两种情况调用该接口： 1,打开设备后.常见于OPC/BACnet等,此时可获取特定设备场景的物模型. 2,扫描设备时.此时未连接任何设备,只能返回该类设备的通用物模型,常用于具体硬件产品,例如各种传感器. 获取后,按新版本覆盖旧版本. </remarks>
        /// <returns> </returns>
        ThingSpec? GetSpecification();
        #endregion

        #region 核心方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">            逻辑设备 </param>
        /// <param name="parameter">         参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default);

        /// <summary>
        /// 关闭设备节点.多节点共用通信链路时,需等最后一个节点关闭才能断开
        /// </summary>
        /// <param name="node">              </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        Task CloseAsync(INode node, CancellationToken cancellationToken = default);

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <remarks> 驱动实现数据采集的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points">            点位集合 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        /// <returns> </returns>
        Task<IDictionary<string, Object?>> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="point">             点位 </param>
        /// <param name="value">             数值 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        Task<Object?> WriteAsync(INode node, IPoint point, Object? value, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="values">            点位数值 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        Task WriteAsync(INode node, IDictionary<IPoint, Object> values, CancellationToken cancellationToken = default);

        /// <summary>
        /// 控制设备,特殊功能使用
        /// </summary>
        /// <remarks> 除了点位读写之外的其它控制功能. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">              节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="parameters">        参数 </param>
        /// <param name="cancellationToken"> 取消令牌 </param>
        Task<Object?> ControlAsync(INode node, IDictionary<string, Object?> parameters, CancellationToken cancellationToken = default);
        #endregion
    }

    /// <summary>
    /// 为支持设备自动发现的驱动定义契约
    /// </summary>
    public interface IDiscoverableDriver
    {
        /// <summary>
        /// 异步扫描和发现网络或串行总线上的兼容设备
        /// </summary>
        /// <param name="parameters">       
        /// 发现操作所需的参数字典. Key-Value 示例:
        /// - For ModbusTCP: {"Subnet": "192.168.1.0/24"}
        /// - For ModbusRTU: {"ComPorts": ["COM1", "COM3"], "BaudRates": [9600, 19200]}
        /// </param>
        /// <param name="cancellationToken"> 用于取消长时间运行的发现操作. </param>
        /// <returns> 一个包含所有被发现设备信息的枚举集合. </returns>
        Task<IEnumerable<IDeviceInfo>> DiscoverAsync(
            Dictionary<string, Object> parameters,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 协议驱动接口.抽象各种硬件设备的数据采集及远程控制
    /// </summary>
    /// <remarks>
    /// 在Modbus协议上,一个通信链路（串口/ModbusTcp地址）即是IDriver,可能有多个物理设备共用,各自表示为INode. 即使是一个物理设备,也可能因为管理需要而划分为多个逻辑设备,例如变配电网关等Modbus汇集网关.
    ///
    /// 除了具体设备实例化驱动对象,在物联网平台扫描驱动时,也有可能实例化驱动对象,以获取默认参数与产品物模型.
    /// </remarks>
    public interface IDriver
    {
        #region 元数据
        /// <summary>
        /// 创建驱动参数对象,分析参数配置或创建默认参数
        /// </summary>
        /// <remarks> 可序列化成Xml/Json作为该协议的参数模板.由于Xml需要良好的注释特性,优先使用. 获取后,按新版本覆盖旧版本. </remarks>
        /// <param name="parameter"> Xml/Json参数配置,为空时仅创建默认参数 </param>
        /// <returns> </returns>
        IDriverParameter? CreateParameter(string? parameter = null);

        /// <summary>
        /// 获取产品物模型
        /// </summary>
        /// <remarks> 如果设备有固定点位属性,服务和事件,则直接返回,否则返回空. 物联网平台有两种情况调用该接口： 1,打开设备后.常见于OPC/BACnet等,此时可获取特定设备场景的物模型. 2,扫描设备时.此时未连接任何设备,只能返回该类设备的通用物模型,常用于具体硬件产品,例如各种传感器. 获取后,按新版本覆盖旧版本. </remarks>
        /// <returns> </returns>
        ThingSpec? GetSpecification();
        #endregion

        #region 核心方法
        /// <summary>
        /// 打开设备驱动,传入参数.一个物理设备可能有多个逻辑设备共用,需要以节点来区分
        /// </summary>
        /// <param name="device">    逻辑设备 </param>
        /// <param name="parameter"> 参数.不同驱动的参数设置相差较大,对象字典具有较好灵活性,其对应IDriverParameter </param>
        /// <returns> 节点对象,可存储站号等信息,仅驱动自己识别 </returns>
        INode Open(IDevice device, IDriverParameter? parameter);

        /// <summary>
        /// 关闭设备节点.多节点共用通信链路时,需等最后一个节点关闭才能断开
        /// </summary>
        /// <param name="node"> </param>
        void Close(INode node);

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <remarks> 驱动实现数据采集的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="points"> 点位集合 </param>
        /// <returns> </returns>
        IDictionary<string, Object?> Read(INode node, IPoint[] points);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">  节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="point"> 点位 </param>
        /// <param name="value"> 数值 </param>
        Object? Write(INode node, IPoint point, Object? value);

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <remarks> 驱动实现远程控制的核心方法,各驱动全力以赴实现好该接口. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">   节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="values"> 点位数值 </param>
        void Write(INode node, IDictionary<IPoint, Object> values);

        /// <summary>
        /// 控制设备,特殊功能使用
        /// </summary>
        /// <remarks> 除了点位读写之外的其它控制功能. 其中点位表名称和地址,仅该驱动能够识别.类型和长度等信息,则由物联网平台统一规范. </remarks>
        /// <param name="node">       节点对象,可存储站号等信息,仅驱动自己识别 </param>
        /// <param name="parameters"> 参数 </param>
        Object? Control(INode node, IDictionary<string, Object?> parameters);
        #endregion
    }

    /// <summary>
    /// 扩展
    /// </summary>
    public static class DriverExtensions
    {
        /// <summary>
        /// 打开设备驱动
        /// </summary>
        /// <param name="driver">     驱动对象 </param>
        /// <param name="device">     逻辑设备 </param>
        /// <param name="parameters"> 参数对象 </param>
        /// <returns> </returns>
        public static INode Open(this IDriver driver, IDevice device, IDictionary<string, Object?> parameters)
        {
            var type = (driver.CreateParameter()?.GetType()) ?? throw new InvalidOperationException();

            var ps = JsonHelper.Default.Convert(parameters, type) as IDriverParameter;
            var node = driver.Open(device, ps);

            node.Driver ??= driver;
            node.Device ??= device;
            node.Parameter ??= ps;

            return node;
        }
    }

    /// <summary>
    /// 驱动参数接口.控制设备驱动的参数,可序列化为Xml便于传输与存储
    /// </summary>
    public interface IDriverParameter
    {
    }

    /// <summary>
    /// 具有唯一标识的驱动参数
    /// </summary>
    /// <remarks> 相同驱动下,相同的唯一标识共用驱动对象. 例如多个设备共用一个串口,或者多个设备共用一个ModbusTcp地址. </remarks>
    public interface IDriverParameterKey
    {
        /// <summary>
        /// 获取驱动参数的唯一标识
        /// </summary>
        /// <remarks> 相同驱动下,相同的唯一标识共用驱动对象. 例如多个设备共用一个串口,或者多个设备共用一个ModbusTcp地址. </remarks>
        string GetKey();
    }

    /// <summary>
    /// 默认驱动参数实现
    /// </summary>
    public class DriverParameter : IDriverParameter { }

    /// <summary>
    /// 驱动参数扩展
    /// </summary>
    public static class DriverParameterExtensions
    {
        /// <summary>
        /// 获取驱动参数的唯一标识
        /// </summary>
        /// <remarks> 相同驱动下,相同的唯一标识共用驱动对象. 例如多个设备共用一个串口,或者多个设备共用一个ModbusTcp地址. </remarks>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        public static string GetKey(this IDriverParameter parameter)
        {
            if (parameter is IDriverParameterKey dk) return dk.GetKey();

            var dic = parameter.ToDictionary();
            if (dic.TryGetValue("Address", out var str)) return str + "";
            if (dic.TryGetValue("Server", out str)) return str + "";
            if (dic.TryGetValue("PortName", out str)) return str + "";
            if (dic.Count > 0) return dic.FirstOrDefault().Value + "";

            return parameter + "";
        }

        /// <summary>
        /// 序列化参数对象为Xml
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        public static string EncodeParameter(this IDriverParameter parameter) => parameter.ToXml(null, true).Trim((Char)0xFEFF);

        /// <summary>
        /// 序列化参数字典为Xml
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        public static string EncodeParameter(this IDictionary<string, Object> parameter) => parameter.ToXml(null, true).Trim((Char)0xFEFF);

        /// <summary>
        /// 从Xml/Json反序列化为字典
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        public static IDictionary<string, Object?>? DecodeParameter(this string parameter)
        {
            parameter = parameter.Trim((Char)0xFEFF);
            if (parameter.IsNullOrEmpty()) throw new ArgumentNullException(nameof(parameter));

            // 按Xml或Json解析参数成为字典
            var ps = parameter.StartsWith("<") && parameter.EndsWith(">") ?
                XmlParser.Decode(parameter) :
                JsonParser.Decode(parameter);

            return ps;
        }
    }

    /// <summary>
    /// 节点接口.多设备共用驱动时,以节点区分
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// 驱动.设备节点使用的驱动对象,可能多设备共用
        /// </summary>
        IDriver Driver { get; set; }

        /// <summary>
        /// 设备.业务逻辑设备
        /// </summary>
        IDevice? Device { get; set; }

        /// <summary>
        /// 参数.设备使用的专用参数
        /// </summary>
        IDriverParameter? Parameter { get; set; }
    }

    /// <summary>
    /// 节点.多设备共用驱动时,以节点区分
    /// </summary>
    public class Node : INode
    {
        /// <summary>
        /// 驱动.设备节点使用的驱动对象,可能多设备共用
        /// </summary>
        public IDriver Driver { get; set; } = null!;

        /// <summary>
        /// 设备.业务逻辑设备
        /// </summary>
        public IDevice? Device { get; set; }

        /// <summary>
        /// 参数.设备使用的专用参数
        /// </summary>
        public IDriverParameter? Parameter { get; set; }
    }
}

// 表达式引擎接口
namespace Feature.IoT.Features
{
    /// <summary>
    /// 表达式引擎接口.用于动态解析执行表达式字符串
    /// </summary>
    public interface IExpressionEngine
    {
        /// <summary>
        /// 最后更新时间.用于判断是否需要重新解析表达式
        /// </summary>
        DateTime UpdateTime { get; set; }

        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="expression"> 表达式字符串 </param>
        /// <param name="parameters"> 参数名称与类型 </param>
        /// <returns> </returns>
        Object Parse(string expression, IDictionary<string, Type> parameters);

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="arguments"> 参数名称与数值 </param>
        /// <returns> </returns>
        Object Invoke(IDictionary<string, Object> arguments);
    }

    /// <summary>
    /// 表达式引擎扩展
    /// </summary>
    public static class ExpressionEngineExtensions
    {
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="engine">     引擎 </param>
        /// <param name="expression"> 表达式字符串 </param>
        /// <param name="name">       参数名称 </param>
        /// <param name="type">       参数类型 </param>
        /// <returns> </returns>
        public static Object Parse(this IExpressionEngine engine, string expression, string name, Type type) => engine.Parse(expression, new Dictionary<string, Type> { { name, type } });

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="engine"> 引擎 </param>
        /// <param name="name">   参数名称 </param>
        /// <param name="value">  参数数值 </param>
        /// <returns> </returns>
        public static Object Invoke(this IExpressionEngine engine, string name, Object value) => engine.Invoke(new Dictionary<string, Object> { { name, value } });
    }
}

// 设备信息接口与模型
namespace Feature.IoT.Models
{
    using System.ComponentModel;

    /// <summary>
    /// 设备信息
    /// </summary>
    /// <remarks>
    /// 客户端与服务端之间交换设备信息.
    ///
    /// 客户端启动时,从服务端获取设备信息,用于后续操作； 客户端子设备上线时,上报设备信息,服务端新增子设备或更新状态；
    /// </remarks>
    public interface IDeviceInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        string? ProductCode { get; set; }
    }

    /// <summary>
    /// 设备信息.简化版,主要用于上报
    /// </summary>
    public class DeviceInfo : IDeviceInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string? ProductCode { get; set; }

        /// <summary>
        /// 协议.选择使用哪一种驱动协议
        /// </summary>
        public string? Protocol { get; set; }

        /// <summary>
        /// 设备参数.Xml/Json格式配置,根据协议驱动来解析
        /// </summary>
        public string? Parameter { get; set; }
    }

    /// <summary>
    /// 设备模型.完整版,主要用于服务端下发或者接收
    /// </summary>
    public class DeviceModel : IDeviceInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string? ProductCode { get; set; }

        /// <summary>
        /// 上报间隔.属性上报间隔,采集后暂存数据,定时上报,默认60秒,-1表示不上报属性
        /// </summary>
        public Int32 PostPeriod { get; set; }

        /// <summary>
        /// 上报模式.数据上报模式,默认0表示变更上报,1每次上报,2不上报
        /// </summary>
        public PostKinds PostKind { get; set; }

        /// <summary>
        /// 采集间隔.默认1000ms
        /// </summary>
        public Int32 PollingTime { get; set; }

        /// <summary>
        /// 协议.选择使用哪一种驱动协议
        /// </summary>
        public string? Protocol { get; set; }

        /// <summary>
        /// 设备参数.Xml/Json格式配置,根据协议驱动来解析
        /// </summary>
        public string? Parameter { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 更新时间.用于判断数据变化
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 数据上报类型
    /// </summary>
    public enum PostKinds
    {
        /// <summary>
        /// 变更上报
        /// </summary>
        [Description("变更上报")]
        Changed = 0,

        /// <summary>
        /// 总是上报
        /// </summary>
        [Description("总是上报")]
        Always = 1,

        /// <summary>
        /// 绝不上报
        /// </summary>
        [Description("绝不上报")]
        Never = 2,
    }

    /// <summary>
    /// 服务请求.应用系统之前发送服务调用请求
    /// </summary>
    public class ServiceRequest
    {
        /// <summary>
        /// 设备编号.目标设备
        /// </summary>
        public Int32 DeviceId { get; set; }

        /// <summary>
        /// 设备编码.目标设备或其子设备
        /// </summary>
        public string? DeviceCode { get; set; }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; } = null!;

        /// <summary>
        /// 入参.传递给该服务的参数,常见Json格式
        /// </summary>
        public string? InputData { get; set; }

        /// <summary>
        /// 开始执行时间.用于提前下发指令后延期执行
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 过期时间.超过该时间后不再执行,未指定时表示不限制
        /// </summary>
        public DateTime Expire { get; set; }

        /// <summary>
        /// 超时时间.如果指定,则等待服务调用返回,单位毫秒
        /// </summary>
        /// <remarks> 服务调用接口内部一般使用异步阻塞的方式来实现超时控制,例如Redis队列 </remarks>
        public Int32 Timeout { get; set; }
    }
}

// 设备数据模型
namespace Feature.IoT.ThingModels
{
    using Feature.IoT.ThingSpecification;
    using Feature.Reflection;

    /// <summary>
    /// 数据模型
    /// </summary>
    public class DataModel
    {
        /// <summary>
        /// 时间.数据采集时间,UTC毫秒
        /// </summary>
        public Int64 Time { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 数据
        /// </summary>
        public string? Value { get; set; }
    }

    /// <summary>
    /// 数据集合
    /// </summary>
    public class DataModels
    {
        /// <summary>
        /// 消息编号.适用于多人同时操作,同一任务需要服务器端多指令串行下发等场景
        /// </summary>
        public Int64 Id { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; } = null!;

        /// <summary>
        /// 数据集合
        /// </summary>
        public DataModel[]? Items { get; set; }
    }

    /// <summary>
    /// 设备属性模型
    /// </summary>
    public class DevicePropertyModel : PropertyModel
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; } = null!;
    }

    /// <summary>
    /// 字节序类型
    /// </summary>
    /// <remarks> 大小端字节序,又称端序或尾序,是指在多字节的数据类型中,数值的字节的排列顺序. 工控领域（PLC）常用ABCD/DCBA表示,网络领域常用1234/4321表示. </remarks>
    public enum EndianType : Byte
    {
        /// <summary>
        /// 大端字节序.ABCD/1234,高序字节存储在起始地址,也叫网络字节序.支持2/4/8字节
        /// </summary>
        BigEndian = 1,

        /// <summary>
        /// 小端字节序.DCBA/4321,低序字节存储在起始地址,也叫主机字节序.支持2/4/8字节
        /// </summary>
        LittleEndian = 2,

        ///// <summary>中端字节序.低序字节存储在起始地址,但字节序不同的数据在内部字节的排序是不同的</summary>
        //MiddleEndian = 3,

        /// <summary>
        /// 大端字节交换.BADC/2143,高序字存储在起始地址,每个字内部字节交换.仅支持4/8字节
        /// </summary>
        /// <remarks> Modbus发送Float常用 </remarks>
        BigSwap = 3,

        /// <summary>
        /// 小端字节交换.CDAB/3412,低序字存储在起始地址,每个字内部没有字节交换（相对于全倒序是交换了两次）.仅支持4/8字节
        /// </summary>
        LittleSwap = 4,
    }

    /// <summary>
    /// 字节序类型
    /// </summary>
    /// <remarks> 大小端字节序,又称端序或尾序,是指在多字节的数据类型中,数值的字节的排列顺序. 工控领域（PLC）常用ABCD/DCBA表示,网络领域常用1234/4321表示. </remarks>
    public enum ByteOrder : Byte
    {
        /// <summary>
        /// 大端字节序.ABCD/1234,高序字节存储在起始地址,也叫网络字节序.支持2/4/8字节
        /// </summary>
        ABCD = 1,

        /// <summary>
        /// 小端字节序.DCBA/4321,低序字节存储在起始地址,也叫主机字节序.支持2/4/8字节
        /// </summary>
        DCBA = 2,

        /// <summary>
        /// 大端字节交换.BADC/2143,高序字存储在起始地址,每个字内部字节交换.仅支持4/8字节
        /// </summary>
        /// <remarks> Modbus发送Float常用 </remarks>
        BADC = 3,

        /// <summary>
        /// 小端字节交换.CDAB/3412,低序字存储在起始地址,每个字内部没有字节交换（相对于全倒序是交换了两次）.仅支持4/8字节
        /// </summary>
        CDAB = 4,
    }

    /// <summary>
    /// 设备事件模型
    /// </summary>
    public class EventModel
    {
        /// <summary>
        /// 时间.Unix毫秒,UTC
        /// </summary>
        public Int64 Time { get; set; }

        /// <summary>
        /// 事件类型.info/alert/error
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 名称.事件名称,例如LightOpen
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 内容.事件详情
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 事件关联的数据对象.例如设备事件中的设备模型对象
        /// </summary>
        public Object? Data { get; set; }
    }

    /// <summary>
    /// 事件集合
    /// </summary>
    public class EventModels
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; } = null!;

        /// <summary>
        /// 事件集合
        /// </summary>
        public EventModel[]? Items { get; set; }
    }

    /// <summary>
    /// 事件模式.在客户端或服务端生成属性变更事件
    /// </summary>
    public enum EventModes
    {
        /// <summary>
        /// 不触发
        /// </summary>
        None = 0,

        /// <summary>
        /// 客户端触发
        /// </summary>
        Client = 1,

        /// <summary>
        /// 服务端触发
        /// </summary>
        Server = 2,
    }

    /// <summary>
    /// 点位
    /// </summary>
    public interface IPoint
    {
        /// <summary>
        /// 名称
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// 地址.表示点位的地址,具体含义由设备驱动决定.例如常规地址6,字母地址DI07,Modbus地址4x0015,位域地址D012.05,比特位置0~15
        /// </summary>
        /// <remarks> 在某些场景中,特殊点位地址（如#）表示虚拟地址,该点位数值由ReadRule表达式动态计算得到,点位信息并不会传递给驱动层 </remarks>
        string? Address { get; set; }

        /// <summary>
        /// 数据类型.来自物模型
        /// </summary>
        string? Type { get; set; }

        /// <summary>
        /// 大小.数据字节数,或字符串长度,Modbus寄存器一般占2个字节
        /// </summary>
        Int32 Length { get; set; }
    }

    /// <summary>
    /// 点位扩展
    /// </summary>
    public static class PointHelper
    {
        /// <summary>
        /// 根据点位类型长度,解析字节数组为目标类型.默认小端字节序,大端需要用Swap提前处理
        /// </summary>
        /// <param name="point"> 点位 </param>
        /// <param name="data">  字节数据 </param>
        /// <returns> </returns>
        public static Object Convert(this IPoint point, Byte[] data)
        {
            var type = point.GetNetType() ?? throw new NotSupportedException();
            if (type == typeof(Byte[])) return data;

            return type.GetTypeCode() switch
            {
                TypeCode.bool => BitConverter.Tobool(data, 0),
                TypeCode.Byte => data[0],
                TypeCode.Char => BitConverter.ToChar(data, 0),
                TypeCode.Double => BitConverter.ToDouble(data, 0),
                TypeCode.Int16 => BitConverter.ToInt16(data, 0),
                TypeCode.Int32 => BitConverter.ToInt32(data, 0),
                TypeCode.Int64 => BitConverter.ToInt64(data, 0),
                TypeCode.Single => BitConverter.ToSingle(data, 0),
                TypeCode.UInt16 => BitConverter.ToUInt16(data, 0),
                TypeCode.UInt32 => BitConverter.ToUInt32(data, 0),
                TypeCode.UInt64 => BitConverter.ToUInt64(data, 0),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 根据点位类型长度,把目标对象转为字节数组.默认小端字节序,大端需要对返回值用Swap处理
        /// </summary>
        /// <param name="point"> 点位 </param>
        /// <param name="value"> 数据对象 </param>
        /// <returns> </returns>
        public static Byte[]? GetBytes(this IPoint point, Object value)
        {
            var type = point.GetNetType() ?? throw new NotSupportedException();
            if (type == typeof(Byte[]) && value is Byte[] data) return data;

            var val = value.ChangeType(type);

            return type.GetTypeCode() switch
            {
                TypeCode.bool => BitConverter.GetBytes((bool)(val ?? false)),
                TypeCode.Byte => [(Byte)(val ?? 0)],
                TypeCode.Char => BitConverter.GetBytes((Char)(val ?? 0)),
                TypeCode.Double => BitConverter.GetBytes((Double)(val ?? 0)),
                TypeCode.Int16 => BitConverter.GetBytes((Int16)(val ?? 0)),
                TypeCode.Int32 => BitConverter.GetBytes((Int32)(val ?? 0)),
                TypeCode.Int64 => BitConverter.GetBytes((Int64)(val ?? 0)),
                TypeCode.Single => BitConverter.GetBytes((Single)(val ?? 0)),
                TypeCode.UInt16 => BitConverter.GetBytes((UInt16)(val ?? 0)),
                TypeCode.UInt32 => BitConverter.GetBytes((UInt32)(val ?? 0)),
                TypeCode.UInt64 => BitConverter.GetBytes((UInt64)(val ?? 0)),
                _ => null,
            };
        }

        /// <summary>
        /// 根据点位信息和物模型信息,把原始数据转为线圈/位
        /// </summary>
        /// <remarks> 一般用在向设备写入点位数据之前,例如Modbus.WriteCoil </remarks>
        /// <param name="point"> 点位 </param>
        /// <param name="data">  原始数据,一般是字符串 </param>
        /// <param name="spec">  物模型 </param>
        /// <returns> </returns>
        public static UInt16[]? ConvertToBit(this IPoint point, Object data, ThingSpec? spec = null)
        {
            var type = TypeHelper.GetNetType(point);
            if (type == null)
            {
                // 找到物属性定义
                var pi = spec?.GetProperty(point.Name);
                type = TypeHelper.GetNetType(pi?.DataType?.Type);
            }
            if (type == null) return null;

            return type.GetTypeCode() switch
            {
                TypeCode.bool or TypeCode.Byte or TypeCode.SByte => data.Tobool() ? [0x01] : [0x00],
                TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 => data.ToInt() > 0 ? [0x01] : [0x00],
                TypeCode.Int64 or TypeCode.UInt64 => data.ToLong() > 0 ? [0x01] : [0x00],
                _ => data.Tobool() ? [0x01] : [0x00],
            };
        }

        /// <summary>
        /// 根据点位信息和物模型信息,把原始数据转寄存器/字
        /// </summary>
        /// <remarks> 一般用在向设备写入点位数据之前,例如Modbus.WriteRegister </remarks>
        /// <param name="point"> 点位 </param>
        /// <param name="data">  原始数据,一般是字符串 </param>
        /// <param name="spec">  物模型 </param>
        /// <returns> 返回短整型数组,有可能一个整数拆分为双字 </returns>
        public static UInt16[]? ConvertToWord(this IPoint point, Object data, ThingSpec? spec = null)
        {
            var type = TypeHelper.GetNetType(point);
            if (type == null)
            {
                // 找到物属性定义
                var pi = spec?.GetProperty(point.Name);
                type = TypeHelper.GetNetType(pi?.DataType?.Type);
            }
            if (type == null) return null;

            switch (type.GetTypeCode())
            {
                case TypeCode.bool:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return data.Tobool() ? [0x01] : [0x00];
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return [(UInt16)data.ToInt()];
                case TypeCode.Int32:
                case TypeCode.UInt32:
                {
                    var n = data.ToInt();
                    return [(UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    var n = data.ToLong();
                    return [(UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
                case TypeCode.Single:
                {
                    var d = (Single)data.ToDouble();
                    //var n = BitConverter.SingleToInt32Bits(d);
                    var n = (UInt32)d;
                    return [(UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
                case TypeCode.Double:
                {
                    var d = (Double)data.ToDouble();
                    //var n = BitConverter.DoubleToInt64Bits(d);
                    var n = (UInt64)d;
                    return [(UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
                case TypeCode.Decimal:
                {
                    var d = data.ToDecimal();
                    var n = (UInt64)d;
                    return [(UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
                //case TypeCode.string:
                //    break;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// 点位模型
    /// </summary>
    public class PointModel : IPoint
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 地址.表示点位的地址,具体含义由设备驱动决定.例如常规地址6,字母地址DI07,Modbus地址4x0015,位域地址D012.05,比特位置0~15
        /// </summary>
        /// <remarks> 在某些场景中,特殊点位地址（如#）表示虚拟地址,该点位数值由ReadRule表达式动态计算得到,点位信息并不会传递给驱动层 </remarks>
        public string? Address { get; set; }

        /// <summary>
        /// 数据类型.来自物模型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 大小.数据字节数,或字符串长度,Modbus寄存器一般占2个字节
        /// </summary>
        public Int32 Length { get; set; }

        /// <summary>
        /// 读取规则.数据解析规则,表达式或脚本
        /// </summary>
        public string? ReadRule { get; set; }

        /// <summary>
        /// 写入规则.数据反解析规则,表达式或脚本
        /// </summary>
        public string? WriteRule { get; set; }

        /// <summary>
        /// 已重载.友好显示
        /// </summary>
        /// <returns> </returns>
        public override string Tostring() => $"{Name} {Type} {Address}";
    }

    /// <summary>
    /// 设备属性模型
    /// </summary>
    public class PropertyModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 数值
        /// </summary>
        public Object? Value { get; set; }
    }

    /// <summary>
    /// 属性集合
    /// </summary>
    public class PropertyModels
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; } = null!;

        /// <summary>
        /// 属性集合
        /// </summary>
        public PropertyModel[]? Items { get; set; }
    }

    /// <summary>
    /// 命令事件参数
    /// </summary>
    public class ServiceEventArgs : EventArgs
    {
        /// <summary>
        /// 命令
        /// </summary>
        public ServiceModel? Model { get; set; }

        /// <summary>
        /// 响应
        /// </summary>
        public ServiceReplyModel? Reply { get; set; }
    }

    /// <summary>
    /// 服务调用模型.平台向设备发起服务调用请求
    /// </summary>
    public class ServiceModel
    {
        /// <summary>
        /// 编号.用于服务调用请求与结果响应配对
        /// </summary>
        public Int64 Id { get; set; }

        /// <summary>
        /// 服务名.如SetProperty
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 入参.传递给该服务的参数,常见Json格式
        /// </summary>
        public string? InputData { get; set; }

        /// <summary>
        /// 开始执行时间.用于提前下发指令后延期执行,暂时不支持取消
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 过期时间.超过该时间后不再执行,未指定时表示不限制
        /// </summary>
        public DateTime Expire { get; set; }

        /// <summary>
        /// 设备编码.服务调用请求发送给网关设备时,该参数指定子设备编码
        /// </summary>
        public string? DeviceCode { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 链路追踪
        /// </summary>
        public string? TraceId { get; set; }
    }

    /// <summary>
    /// 服务响应模型
    /// </summary>
    public class ServiceReplyModel
    {
        /// <summary>
        /// 编号.用于服务调用请求与结果响应配对
        /// </summary>
        public Int64 Id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ServiceStatus Status { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public string? Data { get; set; }
    }

    /// <summary>
    /// 服务状态
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// 就绪
        /// </summary>
        就绪 = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        处理中 = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        已完成 = 2,

        /// <summary>
        /// 取消
        /// </summary>
        取消 = 3,

        /// <summary>
        /// 错误
        /// </summary>
        错误 = 4,
    }

    /// <summary>
    /// 影子模型
    /// </summary>
    public class ShadowModel
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; } = null!;

        /// <summary>
        /// 影子
        /// </summary>
        public Object? Shadow { get; set; }
    }
}

// 物模型规约
namespace Feature.IoT.ThingSpecification
{
    using Feature.Collections;
    using Feature.IoT.ThingModels;
    using Feature.Reflection;
    using System.Reflection;
    using System.Linq;
    using System.Runtime.Serialization;
#if NETCOREAPP
using System.Text.Json.Serialization;
#endif

    /// <summary>
    /// 物模型基础属性
    /// </summary>
    public abstract class SpecBase
    {
        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
#if NETCOREAPP
    [JsonPropertyName("identifier")]
#endif
        [DataMember(Name = "identifier")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 是否必须
        /// </summary>
        public bool Required { get; set; }
        #endregion

        /// <summary>
        /// 已重载.友好显示
        /// </summary>
        /// <returns> </returns>
        public override string Tostring() => $"{Id} {Name}";
    }

    /// <summary>
    /// 物模型规范
    /// </summary>
    /// <remarks> 物联网平台通过定义一种物的描述语言来描述物模型模块和功能,称为TSL（Thing Specification Language） </remarks>
    public class ThingSpec
    {
        #region 属性
        /// <summary>
        /// 模式
        /// </summary>
        public string Schema { get; set; } = "http://iot.feature.link/schema.json";

        /// <summary>
        /// 简介
        /// </summary>
        public Profile? Profile { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertySpec[]? Properties { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public EventSpec[]? Events { get; set; }

        /// <summary>
        /// 服务
        /// </summary>
        public ServiceSpec[]? Services { get; set; }

        ///// <summary>属性扩展</summary>
        //public PropertyExtend[]? ExtendedProperties { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 快速创建属性
        /// </summary>
        /// <param name="id">      标识 </param>
        /// <param name="name">    名称 </param>
        /// <param name="type">    类型 </param>
        /// <param name="length">  长度 </param>
        /// <param name="address"> 点位地址 </param>
        /// <returns> </returns>
        public PropertySpec AddProperty(string id, string name, string type, Int32 length = 0, string? address = null)
        {
            {
                var ps = PropertySpec.Create(id, name, type, length, address);

                var list = new List<PropertySpec>();
                if (Properties != null) list.AddRange(Properties);
                list.Add(ps);
                Properties = list.ToArray();

                return ps;
            }

            //if (address != null)
            //{
            //    var pt = new PropertyExtend { Id = id, Address = address };
            //    var list = new List<PropertyExtend>();
            //    if (ExtendedProperties != null) list.AddRange(ExtendedProperties);
            //    list.Add(pt);
            //    ExtendedProperties = list.ToArray();
            //}
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="id"> </param>
        /// <returns> </returns>
        public PropertySpec? GetProperty(string? id)
        {
            if (id.IsNullOrEmpty()) return null;

            return Properties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(id));
        }

        /// <summary>
        /// 解析Json串
        /// </summary>
        /// <param name="json"> </param>
        public void FromJson(string json)
        {
            var dic = JsonParser.Decode(json);

            var jr = new JsonReader();
            jr.ToObject(dic, null, this);
        }

        /// <summary>
        /// 转为Json串
        /// </summary>
        /// <returns> </returns>
        public string ToJson()
        {
            var dic = this.ToDictionary();

            var jw = new JsonWriter
            {
                //Indented = true,
                IndentedLength = 2,
                //CamelCase = true,
                //Enumstring = true,
            };
            jw.Options.WriteIndented = true;
            jw.Options.CamelCase = true;
            jw.Options.Enumstring = true;
            jw.Write(dic);

            return jw.Getstring();
        }
        #endregion
    }

    /// <summary>
    /// 数据规范
    /// </summary>
    public class DataSpecs
    {
        #region 属性
        /// <summary>
        /// 最小值
        /// </summary>
        public Double Min { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public Double Max { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string? UnitName { get; set; }

        /// <summary>
        /// 步进
        /// </summary>
        public Double Step { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public Int32 Length { get; set; }

        /// <summary>
        /// 枚举映射.布尔型和数字型特有,例如“0=关,1=开”,又如“1=东,2=南,3=西,4=北”
        /// </summary>
        public IDictionary<string, string>? Mapping { get; set; }

        /// <summary>
        /// 字节序
        /// </summary>
        public ByteOrder Order { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 根据指定数据类型获取成员字典,不同类型所需要的字段不一样
        /// </summary>
        /// <param name="type"> 数据类型 </param>
        /// <returns> </returns>
        public IDictionary<string, Object?> GetDictionary(string type)
        {
            var ds = new Dictionary<string, Object?>();

            var t = TypeHelper.GetNetType(type);
            if (t == null) return this.ToDictionary();

            switch (t.GetTypeCode())
            {
                case TypeCode.bool:
                    ds[nameof(Mapping)] = Mapping;
                    break;
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    if (Length > 0)
                        ds[nameof(Length)] = Length;
                    if (Min != 0 || Max != 0)
                        ds[nameof(Min)] = Min;
                    if (Max != 0)
                        ds[nameof(Max)] = Max;
                    if (!Unit.IsNullOrEmpty())
                        ds[nameof(Unit)] = Unit;
                    if (!UnitName.IsNullOrEmpty())
                        ds[nameof(UnitName)] = UnitName;
                    if (Step != 0)
                        ds[nameof(Step)] = Step;
                    if (Mapping != null)
                        ds[nameof(Mapping)] = Mapping;
                    if (Order != 0)
                        ds[nameof(Order)] = Order;
                    break;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    if (Length > 0)
                        ds[nameof(Length)] = Length;
                    if (Min != 0 || Max != 0)
                        ds[nameof(Min)] = Min;
                    if (Max != 0)
                        ds[nameof(Max)] = Max;
                    if (!Unit.IsNullOrEmpty())
                        ds[nameof(Unit)] = Unit;
                    if (!UnitName.IsNullOrEmpty())
                        ds[nameof(UnitName)] = UnitName;
                    if (Step != 0)
                        ds[nameof(Step)] = Step;
                    if (Order != 0)
                        ds[nameof(Order)] = Order;
                    break;
                case TypeCode.string:
                    ds[nameof(Length)] = Length;
                    break;
                default:
                    return this.ToDictionary();
            }

            return ds;
        }
        #endregion
    }

    /// <summary>
    /// 事件规范
    /// </summary>
    /// <remarks> 设备运行时,主动上报给云端的信息,一般包含需要被外部感知和处理的信息,告警和故障.事件中可包含多个输出参数. 例如,某项任务完成后的通知信息；设备发生故障时的温度,时间信息；设备告警时的运行状态等. 事件可以被订阅和推送. </remarks>
    public class EventSpec : SpecBase
    {
        #region 属性
        /// <summary>
        /// 类型.info/warning/error
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
#if NETCOREAPP
    [JsonPropertyName("desc")]
#endif
        [DataMember(Name = "desc")]
        public string? Description { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// 输出
        /// </summary>
        public PropertySpec[]? OutputData { get; set; }
        #endregion
    }

    /// <summary>
    /// 简介
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = "2.1";

        /// <summary>
        /// 产品编码
        /// </summary>
        public string? ProductKey { get; set; }
    }

    /// <summary>
    /// 属性规范
    /// </summary>
    /// <remarks> 用于描述设备运行时具体信息和状态. 例如,环境监测设备所读取的当前环境温度,智能灯开关状态,电风扇风力等级等. 属性可分为读写和只读两种类型.读写类型支持读取和设置属性值,只读类型仅支持读取属性值. </remarks>
    public class PropertySpec : SpecBase, IDictionarySource
    {
        #region 属性
        /// <summary>
        /// 访问模式
        /// </summary>
        public string? AccessMode { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public TypeSpec? DataType { get; set; }

        /// <summary>
        /// 采集点位置信息.常规地址6,Modbus地址 4x0023,位域地址D12.05,虚拟点位地址#
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 缩放因子.不能是0,默认1,n*scaling+constant
        /// </summary>
        public Single Scaling { get; set; } = 1;

        /// <summary>
        /// 常量因子.默认0,n*scaling+constant
        /// </summary>
        public Single Constant { get; set; }

        /// <summary>
        /// 读取规则.数据解析规则,表达式或脚本
        /// </summary>
        public string? ReadRule { get; set; }

        /// <summary>
        /// 写入规则.数据反解析规则,表达式或脚本
        /// </summary>
        public string? WriteRule { get; set; }

        /// <summary>
        /// 事件模式.在客户端或服务端生成属性变更事件
        /// </summary>
        public EventModes EventMode { get; set; }
        #endregion

        #region 创建
        /// <summary>
        /// 快速创建属性
        /// </summary>
        /// <param name="id">      标识 </param>
        /// <param name="name">    名称 </param>
        /// <param name="type">    类型 </param>
        /// <param name="length">  长度 </param>
        /// <param name="address"> 地址 </param>
        /// <returns> </returns>
        public static PropertySpec Create(string id, string name, string type, Int32 length = 0, string? address = null)
        {
            var ps = new PropertySpec
            {
                Id = id,
                Name = name,
                Address = address,
            };

            if (type != null)
                ps.DataType = new TypeSpec { Type = type };

            if (length > 0)
            {
                ps.DataType ??= new TypeSpec();
                ps.DataType.Specs ??= new DataSpecs();
                ps.DataType.Specs.Length = length;
            }
            //if (!address.IsNullOrEmpty())
            //{
            //    ps.DataType ??= new TypeSpec();
            //    ps.DataType.Specs ??= new DataSpecs();
            //    ps.DataType.Specs.Address = address;
            //}

            return ps;
        }

        /// <summary>
        /// 快速创建属性
        /// </summary>
        /// <param name="id">    标识 </param>
        /// <param name="type">  类型 </param>
        /// <param name="order"> 字节序 </param>
        /// <returns> </returns>
        public static PropertySpec Create(string id, string type, ByteOrder order = ByteOrder.ABCD)
        {
            var ps = new PropertySpec
            {
                Id = id,
            };

            if (type != null)
                ps.DataType = new TypeSpec { Type = type };

            if (order > 0)
            {
                ps.DataType ??= new TypeSpec();
                ps.DataType.Specs = new DataSpecs { Order = order };
            }

            return ps;
        }

        /// <summary>
        /// 快速创建属性
        /// </summary>
        /// <param name="member"> </param>
        /// <param name="length"> </param>
        /// <returns> </returns>
        public static PropertySpec Create(MemberInfo member, Int32 length = 0)
        {
            //if (member == null) return null;

            var ps = new PropertySpec
            {
                Id = member.Name,
                Name = member.GetDisplayName() ?? member.GetDescription(),
            };

            if (member is PropertyInfo pi)
                ps.DataType = new TypeSpec { Type = pi.PropertyType.Name };
            if (member is FieldInfo fi)
                ps.DataType = new TypeSpec { Type = fi.FieldType.Name };

            if (length > 0 && ps.DataType != null)
                ps.DataType.Specs = new DataSpecs { Length = length };

            return ps;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 转字典.根据不同类型,提供不一样的序列化能力
        /// </summary>
        /// <returns> </returns>
        public IDictionary<string, Object?> ToDictionary()
        {
            var dic = new Dictionary<string, Object?>();
            if (!Id.IsNullOrEmpty())
                dic.Add("identifier", Id);
            if (!Name.IsNullOrEmpty())
                dic.Add(nameof(Name), Name);
            if (Required)
                dic.Add(nameof(Required), Required);

            if (!AccessMode.IsNullOrEmpty())
                dic.Add(nameof(AccessMode), AccessMode);

            var dt = DataType?.ToDictionary();
            if (dt != null)
                dic.Add(nameof(DataType), dt);

            if (!Address.IsNullOrEmpty())
                dic.Add(nameof(Address), Address);

            if (Scaling != 1)
                dic[nameof(Scaling)] = Scaling;
            if (Constant != 0)
                dic[nameof(Constant)] = Constant;

            if (!ReadRule.IsNullOrEmpty())
                dic[nameof(ReadRule)] = ReadRule;
            if (!WriteRule.IsNullOrEmpty())
                dic[nameof(WriteRule)] = WriteRule;
            if (EventMode != EventModes.None)
                dic[nameof(EventMode)] = EventMode;

            return dic;
        }

        /// <summary>
        /// 已重载.友好显示
        /// </summary>
        /// <returns> </returns>
        public override string Tostring() => $"{Id} {Name} {DataType}";
        #endregion
    }

    /// <summary>
    /// 服务规范
    /// </summary>
    /// <remarks> 指设备可供外部调用的指令或方法.服务调用中可设置输入和输出参数.输入参数是服务执行时的参数,输出参数是服务执行后的结果. 相比于属性,服务可通过一条指令实现更复杂的业务逻辑,例如执行某项特定的任务. 服务分为异步和同步两种调用方式. </remarks>
    public class ServiceSpec : SpecBase
    {
        #region 属性
        /// <summary>
        /// 调用类型.sync/async
        /// </summary>
#if NETCOREAPP
    [JsonPropertyName("callType")]
#endif
        [DataMember(Name = "callType")]
        public string? Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
#if NETCOREAPP
    [JsonPropertyName("desc")]
#endif
        [DataMember(Name = "desc")]
        public string? Description { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// 输入
        /// </summary>
        public PropertySpec[]? InputData { get; set; }

        /// <summary>
        /// 输出
        /// </summary>
        public PropertySpec[]? OutputData { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 快速创建服务
        /// </summary>
        /// <param name="delegate"> </param>
        /// <returns> </returns>
        public static ServiceSpec Create(Delegate @delegate) => Create(@delegate.Method);

        /// <summary>
        /// 快速创建服务
        /// </summary>
        /// <param name="method"> </param>
        /// <returns> </returns>
        public static ServiceSpec Create(MethodBase method)
        {
            //if (method == null) return null;

            var ss = new ServiceSpec
            {
                Id = method.Name,
                Name = method.GetDisplayName() ?? method.GetDescription(),
            };

            var pis = method.GetParameters();
            if (pis.Length > 0)
            {
                var ps = new List<PropertySpec>();
                foreach (var pi in pis)
                {
                    ps.Add(Create(pi));
                }

                ss.InputData = ps.Where(e => e != null).ToArray();
            }

            return ss;
        }

        /// <summary>
        /// 快速创建属性
        /// </summary>
        /// <param name="member"> </param>
        /// <returns> </returns>
        public static PropertySpec Create(ParameterInfo member)
        {
            //if (member == null) return null;

            var ps = new PropertySpec
            {
                Id = member.Name!,
                DataType = new TypeSpec { Type = member.ParameterType.Name }
            };

            return ps;
        }
        #endregion
    }

    /// <summary>
    /// 类型规范
    /// </summary>
    public class TypeSpec : IDictionarySource
    {
        /// <summary>
        /// 类型.int/float/text
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 数据规范
        /// </summary>
        public DataSpecs? Specs { get; set; }

        /// <summary>
        /// 转字典.根据不同类型,提供不一样的序列化能力
        /// </summary>
        /// <returns> </returns>
        public IDictionary<string, Object?> ToDictionary()
        {
            if (Type.IsNullOrEmpty()) return new Dictionary<string, Object?>();

            var ds = Specs?.GetDictionary(Type);

            var dic = new Dictionary<string, Object?>
        {
            { nameof(Type), Type }
        };

            if (ds != null)
                dic.Add(nameof(Specs), ds);

            return dic;
        }

        /// <summary>
        /// 已重载.友好显示
        /// </summary>
        /// <returns> </returns>
        public override string? Tostring() => Type;
    }
}

// 命令服务处理器接口与助手
namespace Feature.IoT.Clients
{
    using Feature.IoT.ThingModels;
    using Feature.Log;
    using Feature.Remoting;
    using Feature.Serialization;

    /// <summary>
    /// 命令服务处理器接口
    /// </summary>
    public interface IServiceHandler
    {
        ///// <summary>收到命令时触发</summary>
        //event EventHandler<ServiceEventArgs> Received;

        /// <summary>
        /// 命令集合
        /// </summary>
        IDictionary<string, Delegate> Services { get; }
    }

    /// <summary>
    /// 命令服务处理器助手
    /// </summary>
    public static class ServiceHandlerHelper
    {
        /// <summary>
        /// 注册服务.收到平台下发的服务调用时,执行注册的方法
        /// </summary>
        /// <param name="client">  命令客户端 </param>
        /// <param name="service"> </param>
        /// <param name="method">  </param>
        /// <exception cref="ArgumentNullException"> </exception>
        public static void RegisterService(this IServiceHandler client, string service, Func<string, string> method)
        {
            if (service.IsNullOrEmpty()) service = method.Method.Name;

            if (client is IDevice device)
                device.RegisterService(service, method);
            else
                client.Services[service] = method;
        }

        /// <summary>
        /// 注册服务.收到平台下发的服务调用时,执行注册的方法
        /// </summary>
        /// <param name="client">  命令客户端 </param>
        /// <param name="service"> </param>
        /// <param name="method">  </param>
        /// <exception cref="ArgumentNullException"> </exception>
        public static void RegisterService(this IServiceHandler client, string service, Func<string, Task<string>> method)
        {
            if (service.IsNullOrEmpty()) service = method.Method.Name;

            if (client is IDevice device)
                device.RegisterService(service, method);
            else
                client.Services[service] = method;
        }

        /// <summary>
        /// 注册服务.收到平台下发的服务调用时,执行注册的方法
        /// </summary>
        /// <param name="client">  命令客户端 </param>
        /// <param name="service"> </param>
        /// <param name="method">  </param>
        /// <exception cref="ArgumentNullException"> </exception>
        public static void RegisterService(this IServiceHandler client, string service, Func<ServiceModel, ServiceReplyModel> method)
        {
            if (service.IsNullOrEmpty()) service = method.Method.Name;

            if (client is IDevice device)
                device.RegisterService(service, method);
            else
                client.Services[service] = method;
        }

        /// <summary>
        /// 注册服务.收到平台下发的服务调用时,执行注册的方法
        /// </summary>
        /// <param name="client">  命令客户端 </param>
        /// <param name="service"> </param>
        /// <param name="method">  </param>
        /// <exception cref="ArgumentNullException"> </exception>
        public static void RegisterService(this IServiceHandler client, string service, Func<ServiceModel, Task<ServiceReplyModel>> method)
        {
            if (service.IsNullOrEmpty()) service = method.Method.Name;

            if (client is IDevice device)
                device.RegisterService(service, method);
            else
                client.Services[service] = method;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="client"> 命令客户端 </param>
        /// <param name="model">  </param>
        /// <returns> </returns>
        public static async Task<ServiceReplyModel> ExecuteService(this IServiceHandler client, ServiceModel model)
        {
            using var span = DefaultTracer.Instance?.NewSpan("ExecuteService", model);
            var rs = new ServiceReplyModel { Id = model.Id, Status = ServiceStatus.已完成 };
            try
            {
                var result = await OnService(client, model).ConfigureAwait(false);
                if (result is ServiceReplyModel reply)
                {
                    reply.Id = model.Id;
                    if (reply.Status is ServiceStatus.就绪 or ServiceStatus.处理中)
                        reply.Status = ServiceStatus.已完成;

                    return reply;
                }

                rs.Data = result?.ToJson();
                return rs;
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);

                XTrace.WriteException(ex);

                rs.Data = ex.Message;
                if (ex is ApiException aex && aex.Code == 400)
                    rs.Status = ServiceStatus.取消;
                else
                    rs.Status = ServiceStatus.错误;
            }

            return rs;
        }

        /// <summary>
        /// 分发执行服务
        /// </summary>
        /// <param name="client"> 命令客户端 </param>
        /// <param name="model">  </param>
        private static async Task<Object?> OnService(IServiceHandler client, ServiceModel model)
        {
            if (!client.Services.TryGetValue(model.Name, out var d))
            {
                // 通用方法
                if (!client.Services.TryGetValue("*", out d))
                    throw new ApiException(400, $"找不到服务[{model.Name}]");
            }

            if (d is Func<string?, string?> func) return func(model.InputData);
            if (d is Func<ServiceModel, ServiceReplyModel> func2) return func2(model);

            if (d is Func<string?, Task<string?>> func3) return await func3(model.InputData).ConfigureAwait(false);
            if (d is Func<ServiceModel, Task<ServiceReplyModel>> func4) return await func4(model).ConfigureAwait(false);

            return null;
        }
    }
}

// IoT串口控制器,提供串口数据收发事件,适合需要主动接收数据的场景,例如读卡器,扫码枪等
namespace Feature.IoT.Controllers
{
#if !NETFRAMEWORK
    using Feature.Data;
    using Feature.Net;
    using Feature.Reflection;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics;
    using System.IO.Ports;

    /// <summary>
    /// 默认串口实现
    /// </summary>
    public class DefaultSerialPort : DisposeBase, ISerialPort
    {
        #region 属性
        /// <summary>
        /// 串口名
        /// </summary>
        public string PortName { get; set; } = null!;

        /// <summary>
        /// 波特率
        /// </summary>
        public Int32 Baudrate { get; set; } = 9600;

        /// <summary>
        /// 数据位.默认8
        /// </summary>
        public Int32 DataBits { get; set; } = 8;

        /// <summary>
        /// 奇偶校验位.默认None无校验
        /// </summary>
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// 停止位.默认One
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 超时时间.发起请求后等待响应的超时时间,默认3000ms
        /// </summary>
        public Int32 Timeout { get; set; } = 3000;

        /// <summary>
        /// 字节超时.数据包间隔,默认10ms
        /// </summary>
        public Int32 ByteTimeout { get; set; } = 10;

        /// <summary>
        /// 缓冲区大小.默认256
        /// </summary>
        public Int32 BufferSize { get; set; } = 256;

        /// <summary>
        /// 收到数据事件
        /// </summary>
        public event EventHandler<ReceivedEventArgs>? Received;

        private SerialPort? _port;

        /// <summary>
        /// 串口对象
        /// </summary>
        public Object Port => _port ??= new(PortName, Baudrate) { ReadTimeout = Timeout, WriteTimeout = Timeout };
        #endregion

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="disposing"> </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Close();
        }

        /// <summary>
        /// 打开
        /// </summary>
        [MemberNotNull(nameof(_port))]
        public virtual void Open()
        {
            if (_port != null && _port.IsOpen) return;

            if (PortName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(PortName));
            if (Baudrate == 0) Baudrate = 9600;

            if (_port == null)
            {
                _port = new SerialPort(PortName, Baudrate)
                {
                    DataBits = DataBits,
                    Parity = Parity,
                    StopBits = StopBits,

                    ReadTimeout = Timeout,
                    WriteTimeout = Timeout
                };

                if (Received != null) _port.DataReceived += OnReceiveSerial;
            }

            _port.Open();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close()
        {
            if (_port != null)
            {
                if (Received != null) _port.DataReceived -= OnReceiveSerial;

                _port.Close();
                _port = null;
            }
        }

        /// <summary>
        /// 接受数据.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e">      </param>
        void OnReceiveSerial(Object sender, SerialDataReceivedEventArgs e)
        {
            var rs = Invoke(null, 1);
            if (rs != null)
            {
                Received?.Invoke(this, new ReceivedEventArgs { Packet = rs });

                // 回收内存池
                rs.TryDispose();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"> 待发送数据 </param>
        /// <param name="offset"> 偏移 </param>
        /// <param name="count">  个数 </param>
        public virtual void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            Open();

            _port.Write(buffer, offset, count);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"> 接收缓冲区 </param>
        /// <param name="offset"> 偏移 </param>
        /// <param name="count">  个数 </param>
        /// <returns> 已接收数据的字节数 </returns>
        public virtual Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
        {
            Open();

            return _port.Read(buffer, offset, count);
        }

        /// <summary>
        /// 调用,发送数据后等待响应
        /// </summary>
        /// <param name="request">   待发送数据 </param>
        /// <param name="minLength"> 等待响应数据的最小长度,默认1 </param>
        /// <returns> </returns>
        public virtual IPacket Invoke(IPacket? request, Int32 minLength)
        {
            Open();

            if (request != null)
            {
                // 清空缓冲区
                _port.DiscardInBuffer();

                if (request.Next == null && request is ArrayPacket ap)
                    _port.Write(ap.Buffer, ap.Offset, ap.Length);
                else
                    _port.Write(request.ReadBytes(), 0, request.Total);

                if (ByteTimeout > 10) Thread.Sleep(ByteTimeout);
            }

            // 串口速度较慢,等待收完数据
            WaitMore(_port, minLength);

            var p = new OwnerPacket(BufferSize);
            var rs = _port.Read(p.Buffer, p.Offset, p.Length);
            p.Resize(rs);

            return p;
        }

        /// <summary>
        /// 延迟.
        /// </summary>
        /// <param name="sp">        </param>
        /// <param name="minLength"> </param>
        private void WaitMore(SerialPort sp, Int32 minLength)
        {
            var count = sp.BytesToRead;
            if (count >= minLength) return;

            var ms = ByteTimeout > 0 ? ByteTimeout : 10;
            var sw = Stopwatch.StartNew();
            while (sp.IsOpen && sw.ElapsedMilliseconds < Timeout)
            {
                // Thread.SpinWait(1);
                Thread.Sleep(ms);
                if (count != sp.BytesToRead)
                {
                    count = sp.BytesToRead;
                    if (count >= minLength) break;

                    // sw.Restart();
                }
            }
        }
    }
#endif

    /// <summary>
    /// 板卡接口.约定板卡所具备的一些基础功能
    /// </summary>
    /// <remarks> 一般工业计算机和各种板卡设备,常用端口是输入输出口和串口,而网络口比较通用,这里不做统一定义. </remarks>
    public interface IBoard
    {
        /// <summary>
        /// 映射设备名的真实地址
        /// </summary>
        /// <remarks> 例如在A2工业计算机中,COM1可映射到/dev/ttyAMA0,项目实施人员仅需配置通用名COM1即可 </remarks>
        /// <param name="name"> </param>
        /// <returns> </returns>
        string Map(string name);

        /// <summary>
        /// 创建输出口
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        IOutputPort CreateOutput(string name);

        /// <summary>
        /// 创建输入口
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        IInputPort CreateInput(string name);

        /// <summary>
        /// 创建串口
        /// </summary>
        /// <param name="portName"> 串口名,在Windows上一般是COM1/COM3等,在Linux上是串口设备路径,工控Linux也可以把COM1/COM3映射到内部串口 </param>
        /// <param name="baudrate"> 波特率,默认9600 </param>
        /// <returns> </returns>
        ISerialPort CreateSerial(string portName, Int32 baudrate = 9600);

        /// <summary>
        /// 创建Modbus
        /// </summary>
        /// <param name="portName"> 串口名,在Windows上一般是COM1/COM3等,在Linux上是串口设备路径,工控Linux也可以把COM1/COM3映射到内部串口 </param>
        /// <param name="baudrate"> 波特率,默认9600 </param>
        /// <returns> </returns>
        IModbus CreateModbus(string portName, Int32 baudrate = 9600);
    }

    /// <summary>
    /// 板卡基类,包含一些端口的默认实现
    /// </summary>
    public class Board : IBoard
    {
        /// <summary>
        /// 映射设备名的真实地址
        /// </summary>
        /// <remarks> 例如在A2工业计算机中,COM1可映射到/dev/ttyAMA0,项目实施人员仅需配置通用名COM1即可 </remarks>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public virtual string Map(string name) => name;

        /// <summary>
        /// 创建输出口
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public virtual IOutputPort CreateOutput(string name) => new FileOutputPort(name);

        /// <summary>
        /// 创建输入口
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public virtual IInputPort CreateInput(string name) => new FileInputPort(name);

        /// <summary>
        /// 创建串口
        /// </summary>
        /// <param name="portName"> 串口名,在Windows上一般是COM1/COM3等,在Linux上是串口设备路径,工控Linux也可以把COM1/COM3映射到内部串口 </param>
        /// <param name="baudrate"> 波特率,默认9600 </param>
        /// <returns> </returns>
        public virtual ISerialPort CreateSerial(string portName, Int32 baudrate = 9600)
        {
#if NETFRAMEWORK
        var sp = new DefaultSerialPort
        {
            PortName = portName,
            Baudrate = baudrate,
        };
        return sp;
#else
            var type = "DefaultSerialPort".GetTypeEx();
            if (type != null)
            {
                if (type.CreateInstance() is ISerialPort sp)
                {
                    sp.PortName = portName;
                    sp.Baudrate = baudrate;

                    return sp;
                }
            }
#endif

            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建Modbus
        /// </summary>
        /// <param name="portName"> 串口名,在Windows上一般是COM1/COM3等,在Linux上是串口设备路径,工控Linux也可以把COM1/COM3映射到内部串口 </param>
        /// <param name="baudrate"> 波特率,默认9600 </param>
        /// <returns> </returns>
        public virtual IModbus CreateModbus(string portName, Int32 baudrate = 9600)
        {
            var type = "ModbusRtu".GetTypeEx();
            if (type != null)
            {
                if (type.CreateInstance() is IModbus modbus)
                {
                    modbus.SetValue("PortName", portName);
                    modbus.SetValue("Baudrate", baudrate);

                    return modbus;
                }
            }

            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 开关量输入口
    /// </summary>
    public interface IInputPort
    {
        /// <summary>
        /// 读取开关值
        /// </summary>
        /// <returns> </returns>
        bool Read();

        /// <summary>
        /// 按下事件
        /// </summary>
        event EventHandler<KeyEventArgs> KeyDown;

        /// <summary>
        /// 弹起事件
        /// </summary>
        event EventHandler<KeyEventArgs> KeyUp;
    }

    /// <summary>
    /// 按键事件参数
    /// </summary>
    public class KeyEventArgs(bool value) : EventArgs
    {
        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// 当前值（按下为 true,松开为 false）
        /// </summary>
        public bool Value { get; set; } = value;
    }

    /// <summary>
    /// 文件驱动输入口
    /// </summary>
    /// <remarks> 实例化文件驱动输入口 </remarks>
    /// <param name="fileName"> </param>
    public class FileInputPort(string fileName) : DisposeBase, IInputPort
    {
        #region 属性
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; } = fileName;

        /// <summary>
        /// 轮询间隔.默认100毫秒
        /// </summary>
        public Int32 Period { get; set; } = 100;

        private FileStream? _fs;
        #endregion

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="disposing"> </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            StopMonitor();
            _fs.TryDispose();
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns> </returns>
        protected virtual FileStream GetFile() => _fs ??= new FileStream(FileName.GetFullPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        /// <summary>
        /// 读取开关值
        /// </summary>
        /// <returns> </returns>
        public virtual bool Read()
        {
            var fs = GetFile();

            return fs.ReadByte() == '1';
        }

        #region 事件处理
        private EventHandler<KeyEventArgs>? _keyDown;
        /// <summary>
        /// 按下事件
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown
        {
            add
            {
                _keyDown += value;
                StartMonitor();
            }
            remove
            {
                _keyDown -= value;
                StopMonitor();
            }
        }

        private EventHandler<KeyEventArgs>? _keyUp;
        /// <summary>
        /// 弹起事件
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp
        {
            add
            {
                _keyUp += value;
                StartMonitor();
            }
            remove
            {
                _keyUp -= value;
                StopMonitor();
            }
        }

        private Timer? _timer;
        private bool _lastValue;
        private void StartMonitor()
        {
            if (_timer != null) return;

            _timer = new Timer(DoMonitor, null, 0, Period);
        }

        private void StopMonitor()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private void DoMonitor(Object? state)
        {
            var value = Read();
            if (value != _lastValue)
            {
                var args = new KeyEventArgs(value);
                if (value)
                    _keyDown?.Invoke(this, args);
                else
                    _keyUp?.Invoke(this, args);

                _lastValue = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Modbus操作接口
    /// </summary>
    public interface IModbus : IDisposable
    {
        #region 读取
        /// <summary>
        /// 读取线圈,0x01
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="count">   线圈数量.一般要求8的倍数 </param>
        /// <returns> 线圈状态字节数组 </returns>
        bool[] ReadCoil(Byte host, UInt16 address, UInt16 count);

        /// <summary>
        /// 读离散量输入,0x02
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="count">   输入数量.一般要求8的倍数 </param>
        /// <returns> 输入状态字节数组 </returns>
        bool[] ReadDiscrete(Byte host, UInt16 address, UInt16 count);

        /// <summary>
        /// 读取保持寄存器,0x03
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="count">   寄存器数量.每个寄存器2个字节 </param>
        /// <returns> 寄存器值数组 </returns>
        UInt16[] ReadRegister(Byte host, UInt16 address, UInt16 count);

        /// <summary>
        /// 读取输入寄存器,0x04
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="count">   输入寄存器数量.每个寄存器2个字节 </param>
        /// <returns> 输入寄存器值数组 </returns>
        UInt16[] ReadInput(Byte host, UInt16 address, UInt16 count);
        #endregion

        #region 写入
        /// <summary>
        /// 写入单线圈,0x05
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="value">   输出值.一般是 0xFF00/0x0000 </param>
        /// <returns> 输出值 </returns>
        Int32 WriteCoil(Byte host, UInt16 address, UInt16 value);

        /// <summary>
        /// 写入保持寄存器,0x06
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="value">   数值 </param>
        /// <returns> 寄存器值 </returns>
        Int32 WriteRegister(Byte host, UInt16 address, UInt16 value);

        /// <summary>
        /// 写多个线圈,0x0F
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="values">  值.一般是 0xFF00/0x0000 </param>
        /// <returns> 数量 </returns>
        Int32 WriteCoils(Byte host, UInt16 address, UInt16[] values);

        /// <summary>
        /// 写多个保持寄存器,0x10
        /// </summary>
        /// <param name="host">    主机.一般是1 </param>
        /// <param name="address"> 地址.例如0x0002 </param>
        /// <param name="values">  数值 </param>
        /// <returns> 寄存器数量 </returns>
        Int32 WriteRegisters(Byte host, UInt16 address, UInt16[] values);
        #endregion
    }

    /// <summary>
    /// 开关量输出口
    /// </summary>
    public interface IOutputPort
    {
        /// <summary>
        /// 读取开关值
        /// </summary>
        /// <returns> </returns>
        bool Read();

        /// <summary>
        /// 写入开关值
        /// </summary>
        /// <param name="value"> </param>
        void Write(bool value);
    }

    /// <summary>
    /// 文件驱动输出口
    /// </summary>
    /// <remarks> 实例化文件驱动输出口 </remarks>
    /// <param name="fileName"> </param>
    public class FileOutputPort(string fileName) : DisposeBase, IOutputPort
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; } = fileName;

        private FileStream? _fs;

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="disposing"> </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fs.TryDispose();
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns> </returns>
        protected virtual FileStream GetFile() => _fs ??= new FileStream(FileName.GetFullPath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        /// <summary>
        /// 读取开关值
        /// </summary>
        /// <returns> </returns>
        public virtual bool Read()
        {
            var fs = GetFile();

            return fs.ReadByte() == '1';
        }

        /// <summary>
        /// 写入开关值
        /// </summary>
        /// <param name="value"> </param>
        public virtual void Write(bool value)
        {
            var fs = GetFile();
            fs.WriteByte((Byte)(value ? '1' : '0'));
            fs.Flush();
        }
    }

    /// <summary>
    /// 继电器控制器
    /// </summary>
    public interface IRelayController
    {
        /// <summary>
        /// Modbus对象
        /// </summary>
        IModbus Modbus { get; set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        Byte Host { get; set; }

        /// <summary>
        /// 控制指定点位
        /// </summary>
        /// <param name="index"> </param>
        /// <param name="value"> </param>
        void Write(Int32 index, bool value);

        /// <summary>
        /// 翻转指定点位
        /// </summary>
        /// <param name="index"> </param>
        void Invert(Int32 index);

        /// <summary>
        /// 控制指定点位
        /// </summary>
        /// <param name="value"> </param>
        void WriteAll(bool value);

        /// <summary>
        /// 翻转指定点位
        /// </summary>
        void InvertAll();

        /// <summary>
        /// 读取指定点位
        /// </summary>
        /// <param name="index"> </param>
        /// <returns> </returns>
        bool Read(Int32 index);

        /// <summary>
        /// 读取所有点位
        /// </summary>
        /// <returns> </returns>
        bool[] ReadAll();
    }

    /// <summary>
    /// 串口接口
    /// </summary>
    /// <remarks> 基础串口类SerialPort的接口,方便模拟串口,以及使用其它串口类. </remarks>
    public interface ISerialPort : IDisposable
    {
        /// <summary>
        /// 串口名
        /// </summary>
        string PortName { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        Int32 Baudrate { get; set; }

        /// <summary>
        /// 超时时间.发起请求后等待响应的超时时间
        /// </summary>
        Int32 Timeout { get; set; }

        /// <summary>
        /// 收到数据事件
        /// </summary>
        event EventHandler<ReceivedEventArgs> Received;

        /// <summary>
        /// 串口对象
        /// </summary>
        Object Port { get; }

        /// <summary>
        /// 打开
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"> 待发送数据 </param>
        /// <param name="offset"> 偏移 </param>
        /// <param name="count">  个数 </param>
        void Write(Byte[] buffer, Int32 offset, Int32 count);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"> 接收缓冲区 </param>
        /// <param name="offset"> 偏移 </param>
        /// <param name="count">  个数 </param>
        /// <returns> 已接收数据的字节数 </returns>
        Int32 Read(Byte[] buffer, Int32 offset, Int32 count);

        /// <summary>
        /// 调用,发送数据后等待响应
        /// </summary>
        /// <param name="request">   待发送数据 </param>
        /// <param name="minLength"> 等待响应数据的最小长度,默认1 </param>
        /// <returns> </returns>
        IPacket Invoke(IPacket? request, Int32 minLength = 1);
    }

    /// <summary>
    /// 继电器控制板
    /// </summary>
    public class RelayController : IRelayController
    {
        #region 属性
        /// <summary>
        /// Modbus对象
        /// </summary>
        public IModbus Modbus { get; set; } = null!;

        /// <summary>
        /// 主机地址
        /// </summary>
        public Byte Host { get; set; } = 1;

        /// <summary>
        /// 点位起始地址
        /// </summary>
        public UInt16 StartAddress { get; set; } = 0x0000;

        /// <summary>
        /// 点位数量
        /// </summary>
        public Int32 Count { get; set; } = 8;
        #endregion

        /// <summary>
        /// 控制指定点位
        /// </summary>
        /// <param name="index"> 索引.相对于起始地址的偏移量 </param>
        /// <param name="value"> </param>
        public virtual void Write(Int32 index, bool value) => Modbus.WriteCoil(Host, (UInt16)(StartAddress + index), (UInt16)(value ? 0xFF00 : 0x0000));

        /// <summary>
        /// 翻转指定点位
        /// </summary>
        /// <param name="index"> </param>
        public virtual void Invert(Int32 index) => Modbus.WriteCoil(Host, (UInt16)(StartAddress + index), 0x5500);

        /// <summary>
        /// 控制指定点位
        /// </summary>
        /// <param name="value"> </param>
        public virtual void WriteAll(bool value) => Modbus.WriteCoil(Host, 0x00FF, (UInt16)(value ? 0xFFFF : 0x0000));

        /// <summary>
        /// 翻转指定点位
        /// </summary>
        public virtual void InvertAll() => Modbus.WriteCoil(Host, 0x00FF, 0x5a00);

        /// <summary>
        /// 读取指定点位
        /// </summary>
        /// <param name="index"> </param>
        /// <returns> </returns>
        public virtual bool Read(Int32 index) => Modbus.ReadCoil(Host, (UInt16)(StartAddress + index), 1)[0];

        /// <summary>
        /// 读取所有点位
        /// </summary>
        /// <returns> </returns>
        public virtual bool[] ReadAll() => Modbus.ReadCoil(Host, StartAddress, (UInt16)Count);

        /// <summary>
        /// 读取从机地址
        /// </summary>
        /// <returns> </returns>
        public UInt16 ReadAddress() => Modbus.ReadRegister(0x00, 0, 1)[0];

        ///// <summary>读取从机波特率</summary>
        ///// <returns></returns>
        //public UInt16 ReadBaudrate() => Modbus.ReadRegister(0xFF, 0x03E8, 1)[0];
    }
}
