#load "kiota_integration.cs"

Console.WriteLine("=== kiota_integration Test ===");

try
{
    var channelKiotaCodeGenType = Type.GetType("ChannelKiotaCodeGenerator");
    Console.WriteLine(channelKiotaCodeGenType != null ? "[PASS] ChannelKiotaCodeGenerator 类型存在" : "[FAIL] ChannelKiotaCodeGenerator 类型未找到");

    if (channelKiotaCodeGenType != null)
    {
        Console.WriteLine(channelKiotaCodeGenType.GetMethod("Generate") != null ? "[PASS] ChannelKiotaCodeGenerator.Generate 方法存在" : "[FAIL] ChannelKiotaCodeGenerator.Generate 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}