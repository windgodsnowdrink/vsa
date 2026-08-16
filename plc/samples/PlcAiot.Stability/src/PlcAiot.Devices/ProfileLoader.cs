using System.Text.Json;
using System.Text.Json.Serialization;
using PlcAiot.Abstractions.Devices;

namespace PlcAiot.Devices;

/// <summary>DeviceProfile 的 JSON 持久化（换型仅改 JSON 文件，§1.12.1）。</summary>
public static class ProfileLoader
{
    // camelCase 属性名（deviceId/pollingMs…）但枚举按成员原名匹配（"Big" 而非 "big"）
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<DeviceProfile> LoadFromJsonAsync(string path, CancellationToken ct = default)
    {
        await using var fs = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<DeviceProfile>(fs, Options, ct)
            ?? throw new InvalidDataException($"cannot deserialize profile {path}");
    }

    public static async Task SaveToJsonAsync(DeviceProfile profile, string path, CancellationToken ct = default)
    {
        await using var fs = File.Create(path);
        await JsonSerializer.SerializeAsync(fs, profile, Options, ct);
    }
}
