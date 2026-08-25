// ────────────────────────────────────────────────────────────────────────────
// §3 地址解析器 + §3.1 ByteConverter（Span 零拷贝，对齐协议端序）
// ────────────────────────────────────────────────────────────────────────────
using System.Buffers.Binary;
using System.Text;
using PlcVsa.Contracts.Devices;
using PlcAddr = System.String;
using U16 = System.UInt16;
using I16 = System.Int16;
using U32 = System.UInt32;
using I32 = System.Int32;
using U64 = System.UInt64;
using I64 = System.Int64;
using F32 = System.Single;
using F64 = System.Double;

namespace PlcVsa.Server.Infrastructure;

public static class AddressParser
{
    public static (U16 start, U16 count, RegisterType rt) Parse(string protocol, PlcAddr addr, DataTypeEnum t)
    {
        U16 count = t switch
        {
            DataTypeEnum.Bool or DataTypeEnum.Byte => 1,
            DataTypeEnum.Int16 or DataTypeEnum.UInt16 => 1,
            DataTypeEnum.Int32 or DataTypeEnum.UInt32 or DataTypeEnum.Float => 2,
            DataTypeEnum.Int64 or DataTypeEnum.UInt64 or DataTypeEnum.Double => 4,
            _ => 1,
        };
        var (start, rt) = protocol.ToLowerInvariant() switch
        {
            "siemens-s7" or "siemens-s7-1200" or "siemens-s7-1500"
                or "siemens-s7-300" or "siemens-s7-400" => ParseSiemens(addr),
            "modbus-tcp" or "modbus-rtu" => ParseModbus(addr),
            "melsec-mc" or "melsec-a" or "melsec-q" => ParseMelsec(addr),
            "omron-fins" => ParseOmron(addr),
            _ => ParseModbus(addr),
        };
        return ((U16)Math.Max(0, start), count, rt);
    }

    private static (int, RegisterType) ParseSiemens(string a)
    {
        if (a.StartsWith("DB", StringComparison.OrdinalIgnoreCase))
        {
            var dot = a.IndexOf('.');
            if (dot < 0) return (0, RegisterType.HoldingRegister);
            var rest = a[(dot + 1)..];
            var numPart = new string(rest.SkipWhile(c => !char.IsDigit(c)).ToArray());
            if (!int.TryParse(numPart, out var byteOff)) byteOff = 0;
            var prefix = new string(rest.TakeWhile(char.IsLetter).ToArray()).ToUpperInvariant();
            return (byteOff / 2, prefix is "DBX" or "X" ? RegisterType.Coil
                : RegisterType.HoldingRegister);
        }
        if (a.Length > 0 && a[0] == 'I') return (NumTail(a, 1), RegisterType.DiscreteInput);
        if (a.Length > 0 && a[0] == 'Q') return (NumTail(a, 1), RegisterType.Coil);
        return (NumTail(a, 0), RegisterType.HoldingRegister);
    }
    private static (int, RegisterType) ParseModbus(string a)
    {
        if (a.StartsWith("HR", StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 2), RegisterType.HoldingRegister);
        if (a.StartsWith("IR", StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 2), RegisterType.InputRegister);
        if (a.StartsWith("C",  StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 1), RegisterType.Coil);
        if (a.StartsWith("DI", StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 2), RegisterType.DiscreteInput);
        return (NumTail(a, 0), RegisterType.HoldingRegister);
    }
    private static (int, RegisterType) ParseMelsec(string a) => a.Length > 0 ? a[0] switch
    {
        'D' or 'W' => (NumTail(a, 1), RegisterType.HoldingRegister),
        'X' => (NumTail(a, 1), RegisterType.DiscreteInput),
        'Y' or 'M' => (NumTail(a, 1), RegisterType.Coil),
        _ => (NumTail(a, 0), RegisterType.HoldingRegister),
    } : (0, RegisterType.HoldingRegister);
    private static (int, RegisterType) ParseOmron(string a) => a.Length > 0 ? a[0] switch
    {
        'D' or 'W' or 'H' => (NumTail(a, 1), RegisterType.HoldingRegister),
        'C' => (NumTail(a, 1), RegisterType.Coil),
        _ => (NumTail(a, 0), RegisterType.HoldingRegister),
    } : (0, RegisterType.HoldingRegister);
    private static int NumTail(string s, int skipPrefix)
    {
        var arr = s.Skip(skipPrefix).TakeWhile(char.IsDigit).ToArray();
        return arr.Length > 0 ? int.Parse(new string(arr)) : 0;
    }
}

public static class ByteConverter
{
    public static T Read<T>(byte[] bytes, DataTypeEnum t)
    {
        var span = bytes.AsSpan();
        object v = t switch
        {
            DataTypeEnum.Bool   => span.Length > 0 && span[0] != 0,
            DataTypeEnum.Byte   => span.Length > 0 ? span[0] : (byte)0,
            DataTypeEnum.Int16  => span.Length >= 2 ? BinaryPrimitives.ReadInt16BigEndian(span) : (I16)0,
            DataTypeEnum.UInt16 => span.Length >= 2 ? BinaryPrimitives.ReadUInt16BigEndian(span) : (U16)0,
            DataTypeEnum.Int32  => span.Length >= 4 ? BinaryPrimitives.ReadInt32BigEndian(span) : (I32)0,
            DataTypeEnum.UInt32 => span.Length >= 4 ? BinaryPrimitives.ReadUInt32BigEndian(span) : (U32)0,
            DataTypeEnum.Int64  => span.Length >= 8 ? BinaryPrimitives.ReadInt64BigEndian(span) : (I64)0,
            DataTypeEnum.UInt64 => span.Length >= 8 ? BinaryPrimitives.ReadUInt64BigEndian(span) : (U64)0,
            DataTypeEnum.Float  => span.Length >= 4 ? ReadF32BE(span) : (F32)0,
            DataTypeEnum.Double => span.Length >= 8 ? ReadF64BE(span) : (F64)0,
            _                   => Encoding.UTF8.GetString(span),
        };
        return (T)Convert.ChangeType(v, typeof(T));
    }
    private static F32 ReadF32BE(Span<byte> s)
    {
        if (BitConverter.IsLittleEndian) { var c = s.ToArray(); Array.Reverse(c); return BitConverter.ToSingle(c, 0); }
        return BitConverter.ToSingle(s);
    }
    private static F64 ReadF64BE(Span<byte> s)
    {
        if (BitConverter.IsLittleEndian) { var c = s.ToArray(); Array.Reverse(c); return BitConverter.ToDouble(c, 0); }
        return BitConverter.ToSingle(s);
    }

    public static byte[] Write<T>(T value, DataTypeEnum t)
    {
        byte[] buf = new byte[8];
        var span = buf.AsSpan();
        switch (t)
        {
            case DataTypeEnum.Bool:   span[0] = Convert.ToBoolean(value) ? (byte)1 : (byte)0; return buf[..1].ToArray();
            case DataTypeEnum.Byte:   span[0] = Convert.ToByte(value);                    return buf[..1].ToArray();
            case DataTypeEnum.Int16:  BinaryPrimitives.WriteInt16BigEndian(span,  Convert.ToInt16(value));  return buf[..2].ToArray();
            case DataTypeEnum.UInt16: BinaryPrimitives.WriteUInt16BigEndian(span, Convert.ToUInt16(value)); return buf[..2].ToArray();
            case DataTypeEnum.Int32:  BinaryPrimitives.WriteInt32BigEndian(span,  Convert.ToInt32(value));  return buf[..4].ToArray();
            case DataTypeEnum.UInt32: BinaryPrimitives.WriteUInt32BigEndian(span, Convert.ToUInt32(value)); return buf[..4].ToArray();
            case DataTypeEnum.Int64:  BinaryPrimitives.WriteInt64BigEndian(span,  Convert.ToInt64(value));  return buf[..8].ToArray();
            case DataTypeEnum.UInt64: BinaryPrimitives.WriteUInt64BigEndian(span, Convert.ToUInt64(value)); return buf[..8].ToArray();
            case DataTypeEnum.Float:
            {
                var f = Convert.ToSingle(value);
                var b = BitConverter.GetBytes(f);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                return b;
            }
            case DataTypeEnum.Double:
            {
                var d = Convert.ToDouble(value);
                var b = BitConverter.GetBytes(d);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                return b;
            }
            default: return Encoding.UTF8.GetBytes(value == null ? "" : value.ToString() ?? "");
        }
    }

    public static U16[] ToWords(byte[] bytes)
    {
        var words = new U16[(bytes.Length + 1) / 2];
        for (int i = 0, j = 0; i < bytes.Length; i += 2, j++)
            words[j] = (U16)((bytes[i] << 8) | (i + 1 < bytes.Length ? bytes[i + 1] : 0));
        return words;
    }
}
