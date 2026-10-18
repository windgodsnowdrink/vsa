// ────────────────────────────────────────────────────────────────────────────
// 服务器内部数据类型映射枚举 + 解析 / 推断辅助
// 注：Contracts.IIoTClient 使用 string? dataType 与 Result<T>（IsSuccess），
// 这里的 DataTypeEnum 仅作服务器内部 ByteConverter / AddressParser 的粒度分类。
// ────────────────────────────────────────────────────────────────────────────
namespace PlcVsa.Server.Infrastructure;

public enum DataTypeEnum
{
    Bool, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Float, Double, String
}

public static class DataTypeParser
{
    public static DataTypeEnum Parse(string? s) => (s ?? "").Trim().ToLowerInvariant() switch
    {
        "bool" or "boolean" or "bit" => DataTypeEnum.Bool,
        "byte" or "uint8"  => DataTypeEnum.Byte,
        "short" or "int16" => DataTypeEnum.Int16,
        "ushort" or "word" or "uint16" => DataTypeEnum.UInt16,
        "int" or "int32" or "dint" => DataTypeEnum.Int32,
        "uint" or "dword" or "uint32" => DataTypeEnum.UInt32,
        "long" or "int64" or "lint" => DataTypeEnum.Int64,
        "ulong" or "uint64" => DataTypeEnum.UInt64,
        "float" or "real" or "single" => DataTypeEnum.Float,
        "double" or "lreal" => DataTypeEnum.Double,
        "string" or "str" or "" => DataTypeEnum.String,
        _ => DataTypeEnum.String,
    };

    public static DataTypeEnum Infer<T>(T value) => value switch
    {
        bool   => DataTypeEnum.Bool,
        byte   => DataTypeEnum.Byte,
        short  => DataTypeEnum.Int16,
        ushort => DataTypeEnum.UInt16,
        int    => DataTypeEnum.Int32,
        uint   => DataTypeEnum.UInt32,
        long   => DataTypeEnum.Int64,
        ulong  => DataTypeEnum.UInt64,
        float  => DataTypeEnum.Float,
        double => DataTypeEnum.Double,
        string => DataTypeEnum.String,
        _      => typeof(T) == typeof(object) ? DataTypeEnum.String : Parse(typeof(T).Name),
    };
}

/// <summary>HTTP /rpc 端点请求体（统一协议调用）。</summary>
public sealed record RpcCall(
    string Protocol, string Host, int Port, byte UnitId,
    string Method, string? Address, PlcVsa.Server.Infrastructure.DataTypeEnum DataType, object? Value);
