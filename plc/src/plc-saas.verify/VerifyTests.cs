// VerifyTests.cs — ADR-101 工程化校验
// 1) 整个 verify 工程编译通过 = File-based 切片的全量 typecheck（含 Mediator 源生成）
// 2) ADR-108 红线静态扫描：所有 plc-saas 源中不得出现 price/currency/amount/money 字段/标识符
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;
using Xunit;

namespace PlcSaas.Verify;

public sealed class VerifyTests
{
    // ADR-108 红线：计量与计费仅以 quota 维度区分，schema/DTO/响应中禁止任何价格/货币/金额字段。
    private static readonly (string Token, string Reason)[] BannedTokens =
    [
        ("price",    "价格字段违反 ADR-108"),
        ("currency", "货币字段违反 ADR-108"),
        ("amount",   "金额字段违反 ADR-108"),
        ("money",    "金额字段违反 ADR-108"),
    ];

    private static string SaasSrcRoot()
    {
        // plc-saas.verify.csproj 将源快照复制为输出目录下的 saas-src（保留子目录结构）
        var baseDir = AppContext.BaseDirectory;
        var candidate = Path.Combine(baseDir, "saas-src");
        if (Directory.Exists(candidate))
            return candidate;

        // 回退：直接读源工程（用于本地未发布到输出的场景）
        var fallback = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "plc-saas"));
        return Directory.Exists(fallback) ? fallback : baseDir;
    }

    // 剥离 C# 注释（// 行注释与 /* */ 块注释），使红线说明注释不干扰扫描。
    private static string StripComments(string line)
    {
        var block = line.IndexOf("/*", StringComparison.Ordinal);
        if (block >= 0)
        {
            var end = line.IndexOf("*/", block + 2, StringComparison.Ordinal);
            line = end >= 0 ? line.Remove(block, end - block + 2) : line.Substring(0, block);
        }
        var lineComment = line.IndexOf("//", StringComparison.Ordinal);
        if (lineComment >= 0)
            line = line.Substring(0, lineComment);
        return line;
    }

    [Fact]
    public void Adr108NoMoneyFieldsInAnySaasSource()
    {
        var root = SaasSrcRoot();
        var files = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories);
        Assert.NotEmpty(files); // 快照必须存在

        var violations = new List<string>();
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                // 先剥离注释，避免红线说明注释（如“不含 price/currency/amount/money”）造成误报
                var line = StripComments(lines[i]);
                foreach (var (token, reason) in BannedTokens)
                {
                    // 整词匹配（含 PascalCase / snake_case / 字段名）：前缀/后缀边界，避免误伤无关词
                    var pattern = $@"(?i)(?<![\p{{L}}\p{{N}}_])({token})(?![\p{{L}}\p{{N}}_])";
                    if (Regex.IsMatch(line, pattern))
                    {
                        violations.Add($"{Path.GetFileName(file)}:line {i + 1} -> {token} | {reason} | {line.Trim()}");
                    }
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "ADR-108 红线被违反（发现价格/货币/金额相关标识符）：\n" + string.Join("\n", violations));
    }

    [Fact]
    public void SliceSourcesArePresent()
    {
        var root = SaasSrcRoot();
        var slicesDir = Path.Combine(root, "slices");
        Assert.True(Directory.Exists(slicesDir), "slices 目录缺失");

        var expected = new[]
        {
            "tenant.register.cs", "tenant.activate.cs",
            "identity.login.cs", "identity.members.cs", "identity.mqtt.cs",
            "billing.usage.cs", "billing.plan.cs", "billing.quota.cs",
            "devices.list.cs", "faults.ack.cs", "faults.stream.cs",
            "ops.tenants.cs", "ops.pricing.cs", "ops.roles.cs", "ops.health.cs",
            "metering.agent.cs", "ingest.bridge.cs", "ingest.http.cs", "ai.assistant.cs",
        };
        foreach (var name in expected)
        {
            Assert.True(
                File.Exists(Path.Combine(slicesDir, name)),
                $"缺失切片源文件：slices/{name}");
        }
    }

    // —— ADR-113 摄取契约：设备 Id 解析 / 遥测归一化（纯逻辑，验证 HTTP 主路径契约）——
    private const string Tid = "3f2504e0-4f89-41d3-9a0c-0305e82c3301";
    private const string Dev = "9f1e7c2a-1b2c-4d3e-8f9a-0b1c2d3e4f5a";

    [Fact]
    public void IngestAuthTryDeviceIdAcceptsGuidAndTopicFallback()
    {
        Assert.True(IngestAuth.TryDeviceId(Dev, null, out var d1));
        Assert.Equal(Guid.Parse(Dev), d1);

        Assert.False(IngestAuth.TryDeviceId("not-a-guid", null, out _));

        var topic = $"tenants/{Tid}/devices/{Dev}/telemetry";
        Assert.True(IngestAuth.TryDeviceId(null, topic, out var d2));
        Assert.Equal(Guid.Parse(Dev), d2);

        Assert.False(IngestAuth.TryDeviceId(null, "bad/topic", out _));
    }

    [Fact]
    public void IngestAuthNormalizeTelemetryParsesUnixAndEmptyMetrics()
    {
        var (ts, metrics) = IngestAuth.NormalizeTelemetry(1_690_000_000, null);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1_690_000_000).UtcDateTime, ts);
        Assert.Empty(metrics);

        var (ts2, metrics2) = IngestAuth.NormalizeTelemetry(null, new Dictionary<string, double> { { "temp", 1.5 } });
        Assert.Equal(DateTime.UtcNow.Date, ts2.Date);
        Assert.Equal(1.5, metrics2["temp"]);
    }

    // —— ADR-112 AI/RAG：故障模式向量库租户隔离（内存兜底实现）——
    [Fact]
    public async Task FaultVectorStoreIsTenantIsolated()
    {
        var store = new InMemoryFaultVectorStore();
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var vec = FaultEmbedding.Encode("OVER_TEMP", "high");

        await store.UpsertPatternAsync(a, "OVER_TEMP", "high", vec);

        var own = await store.SearchSimilarAsync(a, vec, 5);
        Assert.Single(own);
        Assert.Equal("OVER_TEMP", own[0].Code);

        var other = await store.SearchSimilarAsync(b, vec, 5);
        Assert.Empty(other); // 跨租户不可见
    }

    // —— ADR-112 AI：未接入真实 LLM 时离线启发式诊断回退（无网络依赖）——
    [Fact]
    public async Task DiagnosisChatClientFallsBackToHeuristic()
    {
        var client = new PlcDiagnosisChatClient(upstream: null);
        var resp = await client.GetResponseAsync(new List<ChatMessage>
        {
            new(ChatRole.User, "设备持续过温报警"),
        });
        Assert.Contains("启发式诊断", resp.Text);
    }
}
