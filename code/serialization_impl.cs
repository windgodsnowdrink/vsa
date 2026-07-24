#:sdk Microsoft.NET.Sdk
#:package System.Text.Json@8.0.0
#:package MemoryPack@1.9.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using System.Buffers;
using System.Text.Json;
using MemoryPack;

[MemoryPackable]
public partial class {entityName}SerializationModel
{
    public int Id { get; set; }
    // 其他属性...

    [MemoryPackIgnore]
    public JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public byte[] SerializeToUtf8Json()
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, this, JsonOptions);
        return buffer.WrittenMemory.ToArray();
    }

    public static {entityName}SerializationModel DeserializeFromUtf8Json(ReadOnlySpan<byte> json)
    {
        return JsonSerializer.Deserialize<{entityName}SerializationModel>(json)!;
    }

    public byte[] SerializeToMemoryPack()
    {
        return MemoryPackSerializer.Serialize(this);
    }

    public static {entityName}SerializationModel DeserializeFromMemoryPack(ReadOnlySpan<byte> data)
    {
        return MemoryPackSerializer.Deserialize<{entityName}SerializationModel>(data)!;
    }
}