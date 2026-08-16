using System.Globalization;

namespace PlcAiot.Abstractions.Logging;

/// <summary>控制台日志实现（带级别着色前缀）。</summary>
public sealed class ConsoleLogger(string category, LogLevel minLevel = LogLevel.Information) : ILogger
{
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        if (level < minLevel) return;
        var ts = DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        var tag = level switch
        {
            LogLevel.Critical => "CRIT", LogLevel.Error => "FAIL", LogLevel.Warning => "WARN",
            LogLevel.Information => "INFO", LogLevel.Debug => "DBG", _ => "TRC"
        };
        Console.WriteLine($"{ts} [{tag}] {category}: {message}");
        if (exception is not null)
            Console.WriteLine($"        {exception.GetType().Name}: {exception.Message}");
    }
}
