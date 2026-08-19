using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Sink;
using CSharpFlink.Core.Source;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;

namespace CSharpFlinkSpike
{
    // SOURCE: 模拟 PLC 设备温度遥测 (每秒 1 条)
    public class TelemetrySource : SourceFunction
    {
        private readonly Random _rnd = new Random();
        public override void Init() => Console.WriteLine("[source] telemetry source initialized");

        public override void Run(object context)
        {
            var ctx = (SourceContext)context;
            int n = 0;
            while (true)
            {
                double temp = 20 + _rnd.NextDouble() * 25; // 20 ~ 45 °C
                var md = new MetaData
                {
                    WindowId = "temp.win",
                    TagId = "temp01",
                    TagName = "车间温度",
                    Code = "PLC",
                    TagValue = temp.ToString("F2"),
                    TagTime = DateTime.Now,
                    ExtValue = ""
                };
                ctx.Collect(new IMetaData[] { md });
                if (++n % 5 == 0) Console.WriteLine($"[source] emitted {n} samples, last={temp:F2}°C");
                try { System.Threading.Thread.Sleep(Interval); }
                catch (System.Threading.ThreadInterruptedException) { return; }
            }
        }

        public override void Cancel() { }
    }

    // MAP/FILTER 算子: 越限告警 —— map(异常->告警) / filter(正常->null)
    public class ThresholdAlert : Calculate
    {
        private readonly double _threshold;
        public ThresholdAlert(string resultId, double threshold) : base(resultId) { _threshold = threshold; }

        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource == null || input.DataSource.Length == 0) return null;
            double max = input.DataSource.Max(t => double.Parse(t.TagValue));
            if (max <= _threshold) return null; // filter: 正常窗口不产出
            var md = input.DataSource.First();
            return new CalculateOutput(input.SessinId, DateTime.Now, new IMetaData[] {
                new MetaData {
                    WindowId = md.WindowId, TagId = ResultId, TagName = "高温告警",
                    Code = md.Code, TagValue = $"ALERT max={max:F2}>{_threshold}",
                    TagTime = input.InputeDateTime, ExtValue = ""
                }
            });
        }
    }

    // SINK: 控制台输出 (窗口聚合结果)
    public class ConsoleSink : SinkFunction
    {
        public override void Open() { }
        public override void Invoke(IMetaData[] metaDatas, SinkContext context)
        {
            if (metaDatas == null) return;
            foreach (var md in metaDatas)
                Console.WriteLine($"[sink] {md.TagName}({md.TagId}) = {md.TagValue} @ {md.TagTime:HH:mm:ss}");
        }
        public override void Close() { }
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== CSharpFlink Spike POC (net11.0) ===");
            var env = ExecutionEnvironment.GetExecutionEnvironment(null);

            // SINK
            env.AddSink(new ConsoleSink());

            // WINDOW TASK: 5s 窗口 + 算子链 (avg / max / map-filter 告警)
            var calcs = new List<ICalculate>
            {
                new Avg("temp.avg"),
                new Max("temp.max"),
                new ThresholdAlert("temp.alert", 40.0)
            };
            env.TaskManager.AddOrUpdateWindowTask("temp.win", "车间温度窗口", true, 5, 0, calcs);

            // SOURCE
            env.AddSource(new TelemetrySource());

            // 启动数据流
            env.ExcuteSource();
            Console.WriteLine("running 12s, Ctrl+C to stop early...");
            System.Threading.Thread.Sleep(12000);

            ((ExecutionEnvironment)env).Stop();
            Console.WriteLine("=== done ===");
        }
    }
}
