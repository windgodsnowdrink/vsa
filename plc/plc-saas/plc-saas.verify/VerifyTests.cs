// VerifyTests.cs — ADR-101 工程化校验
// 1) 整个 verify 工程编译通过 = File-based 切片的全量 typecheck（含 Mediator 源生成）
// 2) ADR-108 红线静态扫描：所有 plc-saas 源中不得出现 price/currency/amount/money 字段/标识符
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
            "metering.agent.cs",
        };
        foreach (var name in expected)
        {
            Assert.True(
                File.Exists(Path.Combine(slicesDir, name)),
                $"缺失切片源文件：slices/{name}");
        }
    }
}
