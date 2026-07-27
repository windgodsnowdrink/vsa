#load "watchdog_llvm.cs"

Console.WriteLine("=== watchdog_llvm Test ===");

try
{
    // 该文件使用 LLVM IR 优化日志缓冲区处理
    // 验证文件可成功加载
    Console.WriteLine("[PASS] watchdog_llvm.cs 文件加载成功");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}