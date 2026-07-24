#load "collections_pooled_integration.cs"

Console.WriteLine("=== collections_pooled_integration Test ===");

try
{
    var t0 = typeof(Pooled.Collections.PooledCollectionsOptions);
    Console.WriteLine($"[PASS] PooledCollectionsOptions 存在");
    var t1 = typeof(Pooled.Collections.PooledList);
    Console.WriteLine($"[PASS] PooledList 存在");
    var t2 = typeof(Pooled.Collections.PooledDictionary);
    Console.WriteLine($"[PASS] PooledDictionary 存在");
    var t3 = typeof(Pooled.Collections.PooledSet);
    Console.WriteLine($"[PASS] PooledSet 存在");
    var t4 = typeof(Pooled.Collections.PooledCollectionsService);
    Console.WriteLine($"[PASS] PooledCollectionsService 存在");
    var t5 = typeof(Pooled.Collections.PooledListFactory);
    Console.WriteLine($"[PASS] PooledListFactory 存在");
    var t6 = typeof(Pooled.Collections.PooledDictionaryFactory);
    Console.WriteLine($"[PASS] PooledDictionaryFactory 存在");
    var t7 = typeof(Pooled.Collections.PooledSetFactory);
    Console.WriteLine($"[PASS] PooledSetFactory 存在");
    var t8 = typeof(Pooled.Collections.PooledCollectionsExtensions);
    Console.WriteLine($"[PASS] PooledCollectionsExtensions 存在");
    var t9 = typeof(Pooled.Collections.PooledCollectionsExample);
    Console.WriteLine($"[PASS] PooledCollectionsExample 存在");
    var t10 = typeof(Pooled.Collections.IPooledObject);
    Console.WriteLine($"[PASS] IPooledObject 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(Pooled.Collections.IPooledList);
    Console.WriteLine($"[PASS] IPooledList 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(Pooled.Collections.IPooledDictionary);
    Console.WriteLine($"[PASS] IPooledDictionary 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(Pooled.Collections.IPooledSet);
    Console.WriteLine($"[PASS] IPooledSet 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(Pooled.Collections.IPooledListFactory);
    Console.WriteLine($"[PASS] IPooledListFactory 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(Pooled.Collections.IPooledDictionaryFactory);
    Console.WriteLine($"[PASS] IPooledDictionaryFactory 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(Pooled.Collections.IPooledSetFactory);
    Console.WriteLine($"[PASS] IPooledSetFactory 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(Pooled.Collections.IPooledCollectionsService);
    Console.WriteLine($"[PASS] IPooledCollectionsService 接口存在 (IsInterface: {t17.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}