#load "util_core.cs"

Console.WriteLine("=== util_core Test ===");

try
{
    var t0 = typeof(Util.Services.DefaultUtilService);
    Console.WriteLine($"[PASS] DefaultUtilService 存在");
    var t1 = typeof(Util.Services.DefaultFileUtil);
    Console.WriteLine($"[PASS] DefaultFileUtil 存在");
    var t2 = typeof(Util.Services.DefaultCryptoUtil);
    Console.WriteLine($"[PASS] DefaultCryptoUtil 存在");
    var t3 = typeof(Util.Services.DefaultSerializationUtil);
    Console.WriteLine($"[PASS] DefaultSerializationUtil 存在");
    var t4 = typeof(Util.Services.DefaultConfigUtil);
    Console.WriteLine($"[PASS] DefaultConfigUtil 存在");
    var t5 = typeof(Util.Services.DefaultReflectionUtil);
    Console.WriteLine($"[PASS] DefaultReflectionUtil 存在");
    var t6 = typeof(Util.Services.DefaultTimeUtil);
    Console.WriteLine($"[PASS] DefaultTimeUtil 存在");
    var t7 = typeof(Util.Services.DefaultNetworkUtil);
    Console.WriteLine($"[PASS] DefaultNetworkUtil 存在");
    var t8 = typeof(Util.Services.UtilSettings);
    Console.WriteLine($"[PASS] UtilSettings 存在");
    var t9 = typeof(Util.Services.FileUtilSettings);
    Console.WriteLine($"[PASS] FileUtilSettings 存在");
    var t10 = typeof(Util.Services.CryptoUtilSettings);
    Console.WriteLine($"[PASS] CryptoUtilSettings 存在");
    var t11 = typeof(Util.Services.SerializationUtilSettings);
    Console.WriteLine($"[PASS] SerializationUtilSettings 存在");
    var t12 = typeof(Util.Services.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t13 = typeof(Util.Services.CachedFileUtil);
    Console.WriteLine($"[PASS] CachedFileUtil 存在");
    var t14 = typeof(Util.Services.LoggingCryptoUtil);
    Console.WriteLine($"[PASS] LoggingCryptoUtil 存在");
    var t15 = typeof(Util.Services.CompressedSerializationUtil);
    Console.WriteLine($"[PASS] CompressedSerializationUtil 存在");
    var t16 = typeof(Util.Services.User);
    Console.WriteLine($"[PASS] User 存在");
    var t17 = typeof(Util.Services.IUtilService);
    Console.WriteLine($"[PASS] IUtilService 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(Util.Services.IFileUtil);
    Console.WriteLine($"[PASS] IFileUtil 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(Util.Services.ICryptoUtil);
    Console.WriteLine($"[PASS] ICryptoUtil 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(Util.Services.ISerializationUtil);
    Console.WriteLine($"[PASS] ISerializationUtil 接口存在 (IsInterface: {t20.IsInterface})");
    var t21 = typeof(Util.Services.IConfigUtil);
    Console.WriteLine($"[PASS] IConfigUtil 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(Util.Services.IReflectionUtil);
    Console.WriteLine($"[PASS] IReflectionUtil 接口存在 (IsInterface: {t22.IsInterface})");
    var t23 = typeof(Util.Services.ITimeUtil);
    Console.WriteLine($"[PASS] ITimeUtil 接口存在 (IsInterface: {t23.IsInterface})");
    var t24 = typeof(Util.Services.INetworkUtil);
    Console.WriteLine($"[PASS] INetworkUtil 接口存在 (IsInterface: {t24.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}