#load "Datetime.cs"

Console.WriteLine("=== Datetime Test ===");

try
{
    var dateTimeExtensionsType = typeof(DateTimeExtensions);
    Console.WriteLine("[PASS] DateTimeExtensions class found: " + dateTimeExtensionsType.FullName);
    var convertToDateTimeMethod = dateTimeExtensionsType.GetMethod("ConvertToDateTime");
    Console.WriteLine(convertToDateTimeMethod != null ? "[PASS] DateTimeExtensions.ConvertToDateTime exists" : "[FAIL] ConvertToDateTime missing");
    var convertTimestampMethod = dateTimeExtensionsType.GetMethod("ConvertTimestamp");
    Console.WriteLine(convertTimestampMethod != null ? "[PASS] DateTimeExtensions.ConvertTimestamp exists" : "[FAIL] ConvertTimestamp missing");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}