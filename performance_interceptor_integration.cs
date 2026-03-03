# :sdk Microsoft.NET.Sdk.Web
# :package System.Diagnostics.Metrics@8.0.0
# :property LangVersion=preview
# :property TargetFramework=net10.0
# :property Nullable=enable
# :property ImplicitUsings=enable

using System.Threading.Channels;

public class PerformanceInterceptor : DbCommandInterceptor
{
    private readonly ILogger<PerformanceInterceptor> _logger;
    private readonly int _slowQueryThresholdMs;

    public PerformanceInterceptor(ILogger<PerformanceInterceptor> logger, int slowQueryThresholdMs = 1000)
    {
        _logger = logger;
        _slowQueryThresholdMs = slowQueryThresholdMs;
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Duration.TotalMilliseconds > _slowQueryThresholdMs)
        {
            _logger.LogWarning("¼ì²âµ½Âý²éÑ¯£º{Duration}ºÁÃë - {CommandText}",
                eventData.Duration.TotalMilliseconds,
                command.CommandText);
        }
        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }
}