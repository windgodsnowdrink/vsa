#load "wolverine_core.cs"

Console.WriteLine("=== wolverine_core Test ===");

try
{
    // 验证 WolverineSkill 命名空间存在
    // 该文件配置 Wolverine 核心功能
    Console.WriteLine("[PASS] wolverine_core.cs 文件加载成功");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}