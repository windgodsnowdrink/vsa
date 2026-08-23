using System.Runtime;

namespace PlcAiot.Supervisor;

/// <summary>
/// 长期运行 GC 基线（§1.11.4）。
/// - SustainedLowLatency：长稳低停顿。
/// - GC.TryStartNoGCRegion：关键批次（如批量落库）零 GC 停顿。
/// - DATAS 自适应堆需在 runtimeconfig.json 设置 "System.GC.DynamicAdaptationMode": "1"。
/// </summary>
public static class GcStability
{
    public static void ApplyLongRunningBaseline()
    {
        try { GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency; }
        catch (Exception) { /* 非关键，容器编排会重启 */ }
    }

    public static T FlushWithNoGc<T>(Func<T> flush, int bytes = 64 * 1024)
    {
        if (GC.TryStartNoGCRegion(bytes))
        {
            try { return flush(); }
            finally { GC.EndNoGCRegion(); }
        }
        return flush();
    }
}
