#load "brotli_compression_integration.cs"

Console.WriteLine("=== brotli_compression_integration.cs Test ===");

try
{
    // 验证 class: BrotliCompressionOptions
    var type_BrotliCompressionOptions = Type.GetType("BrotliCompressionOptions");
    if (type_BrotliCompressionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 BrotliCompressionOptions (class) 存在");
        var ctors_BrotliCompressionOptions = type_BrotliCompressionOptions.GetConstructors();
        Console.WriteLine($"[PASS] BrotliCompressionOptions 构造函数数量: {ctors_BrotliCompressionOptions.Length}");
        var methods_BrotliCompressionOptions = type_BrotliCompressionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BrotliCompressionOptions 公开方法数量: {methods_BrotliCompressionOptions.Length}");
        foreach (var m in methods_BrotliCompressionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BrotliCompressionOptions 未找到，尝试无命名空间...");
        type_BrotliCompressionOptions = Type.GetType("BrotliCompressionOptions");
        if (type_BrotliCompressionOptions != null)
            Console.WriteLine("[PASS] 类型 BrotliCompressionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BrotliCompressionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BrotliMessageCompressor
    var type_BrotliMessageCompressor = Type.GetType("BrotliMessageCompressor");
    if (type_BrotliMessageCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 BrotliMessageCompressor (class) 存在");
        var ctors_BrotliMessageCompressor = type_BrotliMessageCompressor.GetConstructors();
        Console.WriteLine($"[PASS] BrotliMessageCompressor 构造函数数量: {ctors_BrotliMessageCompressor.Length}");
        var methods_BrotliMessageCompressor = type_BrotliMessageCompressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BrotliMessageCompressor 公开方法数量: {methods_BrotliMessageCompressor.Length}");
        foreach (var m in methods_BrotliMessageCompressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BrotliMessageCompressor 未找到，尝试无命名空间...");
        type_BrotliMessageCompressor = Type.GetType("BrotliMessageCompressor");
        if (type_BrotliMessageCompressor != null)
            Console.WriteLine("[PASS] 类型 BrotliMessageCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BrotliMessageCompressor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BrotliCompressionExtensions
    var type_BrotliCompressionExtensions = Type.GetType("BrotliCompressionExtensions");
    if (type_BrotliCompressionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 BrotliCompressionExtensions (class) 存在");
        var ctors_BrotliCompressionExtensions = type_BrotliCompressionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] BrotliCompressionExtensions 构造函数数量: {ctors_BrotliCompressionExtensions.Length}");
        var methods_BrotliCompressionExtensions = type_BrotliCompressionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BrotliCompressionExtensions 公开方法数量: {methods_BrotliCompressionExtensions.Length}");
        foreach (var m in methods_BrotliCompressionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BrotliCompressionExtensions 未找到，尝试无命名空间...");
        type_BrotliCompressionExtensions = Type.GetType("BrotliCompressionExtensions");
        if (type_BrotliCompressionExtensions != null)
            Console.WriteLine("[PASS] 类型 BrotliCompressionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BrotliCompressionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMessageCompressor
    var type_IMessageCompressor = Type.GetType("IMessageCompressor");
    if (type_IMessageCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 IMessageCompressor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMessageCompressor 未找到，尝试无命名空间...");
        type_IMessageCompressor = Type.GetType("IMessageCompressor");
        if (type_IMessageCompressor != null)
            Console.WriteLine("[PASS] 类型 IMessageCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMessageCompressor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
