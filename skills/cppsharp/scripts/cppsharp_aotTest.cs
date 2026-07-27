#load "cppsharp_aot.cs"

Console.WriteLine("=== cppsharp_aot Test ===");

try
{
    var t0 = typeof(CppSharp.AOT.CppSharpOptions);
    Console.WriteLine($"[PASS] CppSharpOptions 存在");
    var t1 = typeof(CppSharp.AOT.CppTypeInfo);
    Console.WriteLine($"[PASS] CppTypeInfo 存在");
    var t2 = typeof(CppSharp.AOT.CppMemberInfo);
    Console.WriteLine($"[PASS] CppMemberInfo 存在");
    var t3 = typeof(CppSharp.AOT.CppParameterInfo);
    Console.WriteLine($"[PASS] CppParameterInfo 存在");
    var t4 = typeof(CppSharp.AOT.CppSharpService);
    Console.WriteLine($"[PASS] CppSharpService 存在");
    var t5 = typeof(CppSharp.AOT.CppSharpAotEngine);
    Console.WriteLine($"[PASS] CppSharpAotEngine 存在");
    var t6 = typeof(CppSharp.AOT.ICppSharpService);
    Console.WriteLine($"[PASS] ICppSharpService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}