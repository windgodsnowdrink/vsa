#load "libvlcsharp_aot.cs"

Console.WriteLine("=== libvlcsharp_aot Test ===");

try
{
    var programType = Type.GetType("LibVLCSharpAot.Program");
    Console.WriteLine(programType != null ? "[PASS] Program 类型存在" : "[FAIL] Program 类型未找到");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}