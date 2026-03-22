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
using Feature.Data;
using Feature.DNS.Drivers;
using Feature.IoT;
using Feature.IoT.Clients;
using Feature.IoT.Controllers;
using Feature.IoT.Drivers;
using Feature.IoT.Features;
using Feature.IoT.Models;
using Feature.IoT.ThingModels;
using Feature.IoT.ThingSpecification;
using Feature.IoTDatabase.Drivers;
using Feature.IoTSerial.Drivers;
using Feature.IotSocket.Drivers;
using Feature.Log;
using Feature.LoRa.Security;
using Feature.Serialization;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

new AgentService().Main(args);

// DNS服务器
namespace Feature.DNS.Drivers
{
    using Feature.Agent;
    using Feature.DNS.Entity;
    using Feature.Log;
    using Feature.Net.DNS;
    using Feature.Xml;
    using System.ComponentModel;
    using System;
    using XCode;

    /// <summary>
    /// DNS服务器
    /// </summary>
    public class AgentService : ServiceBase
    {
        #region 属性
        #endregion

        #region 构造函数
        public AgentService()
        {
            ServiceName = "Feature.DNS";
            Description = "DNS服务器";
        }
        #endregion

        #region 核心
        DNSServer Server;

        public override void StartWork(String reason)
        {
            // 修改数据库默认目录
            var xcode = XCodeSetting.Current;
            if (xcode.IsNew)
            {
                xcode.ShowSQL = false;
                //xcode.SQLiteDbPath = "..\\Data";
                xcode.Save();
            }

            // 初始化数据库
            Task.Run(() =>
            {
                var n = 0;
                n = Rule.Meta.Count;
                n = Record.Meta.Count;
                n = Visitor.Meta.Count;
            });

            var set = Setting.Current;

            // 启动服务器
            var svr = new DNSServer();
            if (set.Debug) svr.Log = XTrace.Log;
            //svr.Parent = set.DNSServer + "," + svr.Parent;
            svr.Parents.AddRange(svr.GetLocalDNS());
            svr.SetParents(set.DNSServer);
            svr.OnRequest += Server_OnRequest;
            svr.OnResponse += Server_OnResponse;
            svr.OnNew += Server_OnNew;

            svr.Start();

            Server = svr;

            base.StartWork(reason);
        }

        public override void StopWork(String reason)
        {
            base.StopWork(reason);

            var svr = Server;
            svr.Stop(reason);
            svr.OnRequest -= Server_OnRequest;
            svr.OnResponse -= Server_OnResponse;
            svr.OnNew -= Server_OnNew;
        }
        #endregion

        #region 业务
        void Server_OnRequest(object sender, DNSEventArgs e)
        {
            var dns = e.Request;
            if (dns == null) return;

            // 查询规则
            var rs = CheckRule(dns);

            // 查询记录
            if (rs == null) rs = CheckRecord(dns);

            if (rs != null) e.Response = rs;
        }

        DNSEntity CheckRule(DNSEntity dns)
        {
            var rq = dns.Questions[0];
            var list = Rule.FindAllByQueryTypeAndName((Int32)rq.Type, rq.Name);
            if (list == null || list.Count == 0) return null;

            var rs = new DNSEntity();
            rs.Questions = dns.Questions;
            var drs = new List<DNSRecord>();
            foreach (var item in list)
            {
                if (item.QueryType <= 0) continue;

                var r = DNSEntity.CreateRecord((DNSQueryType)item.QueryType);
                r.Name = item.Name;
                if (r.Name[0] == '*') r.Name = r.Name.Substring(1);
                if (r.Name[0] == '.') r.Name = r.Name.Substring(1);
                r.Text = item.Address;
                // 生存时间3分钟
                r.TTL = new TimeSpan(0, 3, 0);
                drs.Add(r);

                item.Hits++;
                item.SaveAsync();
            }
            rs.Answers = drs.ToArray();

            return rs;
        }

        DNSEntity CheckRecord(DNSEntity dns)
        {
            var rq = dns.Questions[0];
            var list = Record.FindAllByQueryTypeAndName((Int32)rq.Type, rq.Name);
            if (list == null || list.Count == 0) return null;

            var rs = new DNSEntity();
            rs.Questions = dns.Questions;

            var drs = new List<DNSRecord>();
            var now = DateTime.Now;
            foreach (var item in list)
            {
                if (item.QueryType <= 0) continue;

                var dr = DNSEntity.CreateRecord((DNSQueryType)item.QueryType);
                dr.Name = item.Name;
                dr.Text = item.Address;

                // 生产时间过期，并且最后更新时间也过期，才去更新
                if (item.Ttl < now && item.Next < now)
                {
                    // 生存时间3分钟
                    dr.TTL = new TimeSpan(0, 3, 0);

                    // 更新数据库记录，3分钟内不要再次去找
                    item.Next = now.AddMinutes(3);

                    item.Hits++;
                    item.SaveAsync();
                }
                else
                {
                    dr.TTL = item.Ttl - now;
                    if (dr.TTL.TotalSeconds < 60) dr.TTL = new TimeSpan(0, 10, 0);
                    drs.Add(dr);
                }
            }
            // 没有任何满足条件的返回，让它去更新吧
            if (drs.Count < 1) return null;

            rs.Answers = drs.ToArray();

            return rs;
        }

        void Server_OnResponse(object sender, DNSEventArgs e)
        {
            var rs = e.Response;
            if (rs == null) return;

            var remote = e.Session?.Remote;
            var rq = rs.Questions[0];

            // 记录历史
            var hi = new History();
            hi.Type = (Int32)rq.Type;
            hi.Name = rq.Name;
            if (remote != null)
            {
                hi.UserIP = remote.EndPoint.Address + "";
                hi.ProtocolType = remote.Type;
            }
            if (rs.Answers != null && rs.Answers.Length > 0)
            {
                //entity.Address = rs.Answers[0].Text;

                foreach (var item in rs.Answers)
                {
                    //var dr = hi.CloneEntity(true);
                    var dr = new History();
                    dr.UserIP = hi.UserIP;
                    dr.ProtocolType = hi.ProtocolType;
                    dr.Type = (Int32)item.Type;
                    dr.Name = item.Name;
                    dr.Address = item.Text;
                    dr.SaveAsync();
                }
            }
            else
                hi.SaveAsync();

            // 记录访问者
            var vt = Visitor.Check(remote?.Host);
            if (vt != null)
            {
                vt.LastDomainName = rq.Name;
                vt.Hits++;
                vt.LastVisit = DateTime.Now;
                // 单对象缓存会自动保存
                vt.SaveAsync();
            }
        }

        void Server_OnNew(object sender, DNSEventArgs e)
        {
            var rs = e.Response;
            if (rs == null) return;

            var list = new List<DNSRecord>();
            if (rs.Answers != null) list.AddRange(rs.Answers);
            if (rs.Authoritis != null) list.AddRange(rs.Authoritis);
            if (rs.Additionals != null) list.AddRange(rs.Additionals);

            var rq = rs.Questions[0];

            var now = DateTime.Now;
            foreach (var dr in list)
            {
                var entity = Record.FindByQueryTypeAndNameAndAddress((Int32)dr.Type, rq.Name, dr.Text);
                if (entity == null)
                {
                    entity = new Record();
                    entity.Name = rq.Name;
                    entity.Type = (Int32)dr.Type;
                    entity.Address = dr.Text;
                }

                entity.Ttl = now.Add(dr.TTL);
                entity.Parent = e.Session.Remote + "";
                entity.UpdateTime = DateTime.Now;

                entity.SaveAsync();
            }
        }
        #endregion
    }

    [XmlConfigFile("Config\\DNS.config", 15000)]
    public class Setting : XmlConfig<Setting>
    {
        /// <summary>调试，默认true</summary>
        [Description("调试，默认true")]
        public Boolean Debug { get; set; } = true;

        /// <summary>上级DNS服务器</summary>
        [Description("上级DNS服务器")]
        public String DNSServer { get; set; } = "udp://223.5.5.5,tcp://8.8.8.8,udp://223.4.4.4";
    }

}
