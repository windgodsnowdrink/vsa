namespace PlcVsa.Contracts.Plugins;

/// <summary>Contracts 自包含的插件级日志接口（不引入 Microsoft.Extensions.Logging 包依赖）。</summary>
public interface IPluginLogger
{
    void Log(string message);
    void Warn(string message);
    void Error(string message, Exception? ex = null);
}
