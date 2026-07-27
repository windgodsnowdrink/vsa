#load "zstd_compression_integration.cs"

Console.WriteLine("=== zstd_compression_integration.cs Test ===");

try
{
    // 验证 class: ZstdCompression.ZstdCompressionOptions
    var type_ZstdCompressionOptions = Type.GetType("ZstdCompression.ZstdCompressionOptions");
    if (type_ZstdCompressionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ZstdCompression.ZstdCompressionOptions (class) 存在");
        var ctors_ZstdCompressionOptions = type_ZstdCompressionOptions.GetConstructors();
        Console.WriteLine($"[PASS] ZstdCompression.ZstdCompressionOptions 构造函数数量: {ctors_ZstdCompressionOptions.Length}");
        var methods_ZstdCompressionOptions = type_ZstdCompressionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZstdCompression.ZstdCompressionOptions 公开方法数量: {methods_ZstdCompressionOptions.Length}");
        foreach (var m in methods_ZstdCompressionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZstdCompression.ZstdCompressionOptions 未找到，尝试无命名空间...");
        type_ZstdCompressionOptions = Type.GetType("ZstdCompressionOptions");
        if (type_ZstdCompressionOptions != null)
            Console.WriteLine("[PASS] 类型 ZstdCompressionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZstdCompressionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZstdCompression.ZstdCompressor
    var type_ZstdCompressor = Type.GetType("ZstdCompression.ZstdCompressor");
    if (type_ZstdCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 ZstdCompression.ZstdCompressor (class) 存在");
        var ctors_ZstdCompressor = type_ZstdCompressor.GetConstructors();
        Console.WriteLine($"[PASS] ZstdCompression.ZstdCompressor 构造函数数量: {ctors_ZstdCompressor.Length}");
        var methods_ZstdCompressor = type_ZstdCompressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZstdCompression.ZstdCompressor 公开方法数量: {methods_ZstdCompressor.Length}");
        foreach (var m in methods_ZstdCompressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZstdCompression.ZstdCompressor 未找到，尝试无命名空间...");
        type_ZstdCompressor = Type.GetType("ZstdCompressor");
        if (type_ZstdCompressor != null)
            Console.WriteLine("[PASS] 类型 ZstdCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZstdCompressor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZstdCompression.ZstdCompressionExtensions
    var type_ZstdCompressionExtensions = Type.GetType("ZstdCompression.ZstdCompressionExtensions");
    if (type_ZstdCompressionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ZstdCompression.ZstdCompressionExtensions (class) 存在");
        var ctors_ZstdCompressionExtensions = type_ZstdCompressionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ZstdCompression.ZstdCompressionExtensions 构造函数数量: {ctors_ZstdCompressionExtensions.Length}");
        var methods_ZstdCompressionExtensions = type_ZstdCompressionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZstdCompression.ZstdCompressionExtensions 公开方法数量: {methods_ZstdCompressionExtensions.Length}");
        foreach (var m in methods_ZstdCompressionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZstdCompression.ZstdCompressionExtensions 未找到，尝试无命名空间...");
        type_ZstdCompressionExtensions = Type.GetType("ZstdCompressionExtensions");
        if (type_ZstdCompressionExtensions != null)
            Console.WriteLine("[PASS] 类型 ZstdCompressionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZstdCompressionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ZstdCompression.IZstdCompressor
    var type_IZstdCompressor = Type.GetType("ZstdCompression.IZstdCompressor");
    if (type_IZstdCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 ZstdCompression.IZstdCompressor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZstdCompression.IZstdCompressor 未找到，尝试无命名空间...");
        type_IZstdCompressor = Type.GetType("IZstdCompressor");
        if (type_IZstdCompressor != null)
            Console.WriteLine("[PASS] 类型 IZstdCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IZstdCompressor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
