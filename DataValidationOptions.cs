#:sdk Microsoft.NET.Sdk.Web
#:package CChoETL@1.2.1.70
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;

namespace ChoETL.Integration
{
    /// <summary>
    /// 数据验证配置选项
    /// </summary>
    public class DataValidationOptions
    {
        /// <summary>
        /// 是否启用数据验证
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 最大验证重试次数
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// 验证失败时的错误日志路径
        /// </summary>
        public string ErrorLogPath { get; set; } = "errors.log";

        /// <summary>
        /// 自定义验证规则集合
        /// </summary>
        public List<Func<object, bool>> CustomValidators { get; } = new List<Func<object, bool>>();

        /// <summary>
        /// 内置的10种数据清洗策略
        /// </summary>
        public Dictionary<string, Func<string, string>> CleaningStrategies { get; } = new Dictionary<string, Func<string, string>>();
    }
}