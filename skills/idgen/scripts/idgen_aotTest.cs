#load "idgen_aot.cs"

Console.WriteLine("=== idgen_aot Test ===");

try
{
    var t0 = typeof(IdgenSettings);
    Console.WriteLine($"[PASS] IdgenSettings 存在");
    var t1 = typeof(IdGenerationResult);
    Console.WriteLine($"[PASS] IdGenerationResult 存在");
    var t2 = typeof(IdValidationResult);
    Console.WriteLine($"[PASS] IdValidationResult 存在");
    var t3 = typeof(IdDecodeResult);
    Console.WriteLine($"[PASS] IdDecodeResult 存在");
    var t4 = typeof(IdgenService);
    Console.WriteLine($"[PASS] IdgenService 存在");
    var t5 = typeof(ULID);
    Console.WriteLine($"[PASS] ULID struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}