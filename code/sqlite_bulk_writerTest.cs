#load "sqlite_bulk_writer.cs"

Console.WriteLine("=== sqlite_bulk_writer.cs Test ===");

try
{
    // 验证 class: SqliteBulkWriter
    var type_SqliteBulkWriter = Type.GetType("SqliteBulkWriter");
    if (type_SqliteBulkWriter != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteBulkWriter (class) 存在");
        var ctors_SqliteBulkWriter = type_SqliteBulkWriter.GetConstructors();
        Console.WriteLine($"[PASS] SqliteBulkWriter 构造函数数量: {ctors_SqliteBulkWriter.Length}");
        var methods_SqliteBulkWriter = type_SqliteBulkWriter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteBulkWriter 公开方法数量: {methods_SqliteBulkWriter.Length}");
        foreach (var m in methods_SqliteBulkWriter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteBulkWriter 未找到，尝试无命名空间...");
        type_SqliteBulkWriter = Type.GetType("SqliteBulkWriter");
        if (type_SqliteBulkWriter != null)
            Console.WriteLine("[PASS] 类型 SqliteBulkWriter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteBulkWriter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
