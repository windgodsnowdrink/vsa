namespace PlcAiot.Abstractions.Logging;

/// <summary>极简日志级别（避免引入外部依赖，演示工程自包含）。</summary>
public enum LogLevel
{
    Trace, Debug, Information, Warning, Error, Critical
}
