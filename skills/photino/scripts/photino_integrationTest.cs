#load "photino_integration.cs"

Console.WriteLine("=== photino_integration Test ===");

try
{
    var t0 = typeof(Photino.PhotinoOptions);
    Console.WriteLine($"[PASS] PhotinoOptions 存在");
    var t1 = typeof(Photino.UpdateResult);
    Console.WriteLine($"[PASS] UpdateResult 存在");
    var t2 = typeof(Photino.PhotinoService);
    Console.WriteLine($"[PASS] PhotinoService 存在");
    var t3 = typeof(Photino.PhotinoWindowImpl);
    Console.WriteLine($"[PASS] PhotinoWindowImpl 存在");
    var t4 = typeof(Photino.LocalStorageImpl);
    Console.WriteLine($"[PASS] LocalStorageImpl 存在");
    var t5 = typeof(Photino.MemoryCache);
    Console.WriteLine($"[PASS] MemoryCache 存在");
    var t6 = typeof(Photino.CacheItem);
    Console.WriteLine($"[PASS] CacheItem 存在");
    var t7 = typeof(Photino.PhotinoServiceCollectionExtensions);
    Console.WriteLine($"[PASS] PhotinoServiceCollectionExtensions 存在");
    var t8 = typeof(Photino.CustomPlugin);
    Console.WriteLine($"[PASS] CustomPlugin 存在");
    var t9 = typeof(Photino.IPhotinoService);
    Console.WriteLine($"[PASS] IPhotinoService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(Photino.IPhotinoWindow);
    Console.WriteLine($"[PASS] IPhotinoWindow 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(Photino.IPhotinoPlugin);
    Console.WriteLine($"[PASS] IPhotinoPlugin 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(Photino.ILocalStorage);
    Console.WriteLine($"[PASS] ILocalStorage 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}