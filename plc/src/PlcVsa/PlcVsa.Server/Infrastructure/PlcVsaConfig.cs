// ────────────────────────────────────────────────────────────────────────────
// 全局配置（对应原 plc-vsa-demo 中的 PlcVsaConfig static 类）
// 使用构造函数注入 + 常量属性：DI 可注入、编译期常量仍可直接用 PlcVsaConfig.RING_SIZE
// ────────────────────────────────────────────────────────────────────────────
namespace PlcVsa.Server.Infrastructure;

public sealed class PlcVsaConfig
{
    public const int RING_SIZE = 1 << 14;
    public const int RING_MASK = RING_SIZE - 1;
    public const int PAGE_SIZE = 4096;
    public const string PROTOCOL_MEMORY_DIR = "protocol-memory";
    public const string PLUGINS_DIR = "plugins";
}
