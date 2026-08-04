#:sdk Microsoft.NET.Sdk.Web
#:package ChoETL@1.2.1.70
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;

namespace ChoETL.Integration
{
    /// <summary>
    /// 分布式ETL配置选项
    /// </summary>
    public class DistributedETLOptions
    {
        /// <summary>
        /// 是否启用分布式处理
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Dapr服务名称
        /// </summary>
        public string DaprServiceName { get; set; } = "etl-service";

        /// <summary>
        /// 分片策略（Hash/Range）
        /// </summary>
        public string ShardingStrategy { get; set; } = "Hash";

        /// <summary>
        /// 最大并行度
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// 批处理大小
        /// </summary>
        public int BatchSize { get; set; } = 1000;

        /// <summary>
        /// Prometheus指标采集端点
        /// </summary>
        public string MetricsEndpoint { get; set; } = "/metrics";

        /// <summary>
        /// 管道分析器采样间隔（毫秒）
        /// </summary>
        public int PipelineAnalyzerInterval { get; set; } = 5000;

        /// <summary>
        /// 动态调整并行度的负载阈值
        /// </summary>
        public double LoadThreshold { get; set; } = 0.7;
    }
}