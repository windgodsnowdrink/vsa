// VerifyTests.cs 鈥?ADR-101 宸ョ▼鍖栨牎楠?// 1) 鏁翠釜 verify 宸ョ▼缂栬瘧閫氳繃 = File-based 鍒囩墖鐨勫叏閲?typecheck锛堝惈 Mediator 婧愮敓鎴愶級
// 2) ADR-108 绾㈢嚎闈欐€佹壂鎻忥細鎵€鏈?plc-saas 婧愪腑涓嶅緱鍑虹幇 price/currency/amount/money 瀛楁/鏍囪瘑绗?using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace PlcSaas.Verify;

public sealed class VerifyTests
{
    // ADR-108 绾㈢嚎锛氳閲忎笌璁¤垂浠呬互 quota 缁村害鍖哄垎锛宻chema/DTO/鍝嶅簲涓姝换浣曚环鏍?璐у竵/閲戦瀛楁銆?    private static readonly (string Token, string Reason)[] BannedTokens =
    [
        ("price",    "浠锋牸瀛楁杩濆弽 ADR-108"),
        ("currency", "璐у竵瀛楁杩濆弽 ADR-108"),
        ("amount",   "閲戦瀛楁杩濆弽 ADR-108"),
        ("money",    "閲戦瀛楁杩濆弽 ADR-108"),
    ];

    private static string SaasSrcRoot()
    {
        // plc-saas.verify.csproj 灏嗘簮蹇収澶嶅埗涓鸿緭鍑虹洰褰曚笅鐨?saas-src锛堜繚鐣欏瓙鐩綍缁撴瀯锛?        var baseDir = AppContext.BaseDirectory;
        var candidate = Path.Combine(baseDir, "saas-src");
        if (Directory.Exists(candidate))
            return candidate;

        // 鍥為€€锛氱洿鎺ヨ婧愬伐绋嬶紙鐢ㄤ簬鏈湴鏈彂甯冨埌杈撳嚭鐨勫満鏅級
        var fallback = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "plc-saas"));
        return Directory.Exists(fallback) ? fallback : baseDir;
    }

    [Fact]
    public void Adr108_NoMoneyFieldsInAnySaasSource()
    {
        var root = SaasSrcRoot();
        var files = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories);
        Assert.NotEmpty(files); // 蹇収蹇呴』瀛樺湪

        var violations = new List<string>();
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                foreach (var (token, reason) in BannedTokens)
                {
                    // 鏁磋瘝鍖归厤锛堝惈 PascalCase / snake_case / 瀛楁鍚嶏級锛氬墠缂€/鍚庣紑杈圭晫锛岄伩鍏嶈浼ゆ棤鍏宠瘝
                    var pattern = $@"(?i)(?<![\p{L}\p{N}_])({token})(?![\p{L}\p{N}_])";
                    if (Regex.IsMatch(line, pattern))
                    {
                        violations.Add($"{Path.GetFileName(file)}:line {i + 1} -> {token} | {reason} | {line.Trim()}");
                    }
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "ADR-108 绾㈢嚎琚繚鍙嶏紙鍙戠幇浠锋牸/璐у竵/閲戦鐩稿叧鏍囪瘑绗︼級锛歕n" + string.Join("\n", violations));
    }

    [Fact]
    public void SliceSourcesArePresent()
    {
        var root = SaasSrcRoot();
        var slicesDir = Path.Combine(root, "slices");
        Assert.True(Directory.Exists(slicesDir), "slices 鐩綍缂哄け");

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
                $"缂哄け鍒囩墖婧愭枃浠讹細slices/{name}");
        }
    }
}
