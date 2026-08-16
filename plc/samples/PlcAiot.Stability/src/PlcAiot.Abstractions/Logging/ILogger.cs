namespace PlcAiot.Abstractions.Logging;

/// <summary>最小日志契约；生产环境可替换为 Serilog / OpenTelemetry 输出端。</summary>
public interface ILogger
{
    void Log(LogLevel level, string message, Exception? exception = null);
}
