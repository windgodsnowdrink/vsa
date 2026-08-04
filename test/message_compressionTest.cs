#load "message_compression.cs"

Console.WriteLine("=== message_compression Test ===");

try
{
    var t0 = typeof(CompressedMessageHandler);
    Console.WriteLine($"[PASS] CompressedMessageHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}