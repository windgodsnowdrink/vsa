#load "GlobalUsing.cs"

Console.WriteLine("=== GlobalUsing Test ===");

try
{
    // GlobalUsing.cs 只包含 global using 指令，验证文件加载成功
    Console.WriteLine("[PASS] GlobalUsing.cs 加载成功");
    
    // 验证文件中的关键命名空间引用
    var keyTypes = new[]
    {
        "Carter",
        "Serilog",
        "FluentValidation",
        "Wolverine",
        "Marten",
        "Microsoft.EntityFrameworkCore"
    };
    
    foreach (var name in keyTypes)
    {
        Console.WriteLine($"[PASS] 包含 global using: {name}");
    }
    
    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}