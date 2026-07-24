#load "tailwind_core.cs"

Console.WriteLine("=== tailwind_core Test ===");

try
{
    var t0 = typeof(TailwindCSS.TailwindConfiguration);
    Console.WriteLine($"[PASS] TailwindConfiguration 存在");
    var t1 = typeof(TailwindCSS.ThemeConfiguration);
    Console.WriteLine($"[PASS] ThemeConfiguration 存在");
    var t2 = typeof(TailwindCSS.PluginConfiguration);
    Console.WriteLine($"[PASS] PluginConfiguration 存在");
    var t3 = typeof(TailwindCSS.BuildContext);
    Console.WriteLine($"[PASS] BuildContext 存在");
    var t4 = typeof(TailwindCSS.TailwindService);
    Console.WriteLine($"[PASS] TailwindService 存在");
    var t5 = typeof(TailwindCSS.TailwindThemeService);
    Console.WriteLine($"[PASS] TailwindThemeService 存在");
    var t6 = typeof(TailwindCSS.TailwindPluginService);
    Console.WriteLine($"[PASS] TailwindPluginService 存在");
    var t7 = typeof(TailwindCSS.ParseDirectivesStep);
    Console.WriteLine($"[PASS] ParseDirectivesStep 存在");
    var t8 = typeof(TailwindCSS.ProcessLayersStep);
    Console.WriteLine($"[PASS] ProcessLayersStep 存在");
    var t9 = typeof(TailwindCSS.ApplyDirectivesStep);
    Console.WriteLine($"[PASS] ApplyDirectivesStep 存在");
    var t10 = typeof(TailwindCSS.DefaultPlugin);
    Console.WriteLine($"[PASS] DefaultPlugin 存在");
    var t11 = typeof(TailwindCSS.TailwindServiceLoggingDecorator);
    Console.WriteLine($"[PASS] TailwindServiceLoggingDecorator 存在");
    var t12 = typeof(TailwindCSS.TailwindServiceCachingDecorator);
    Console.WriteLine($"[PASS] TailwindServiceCachingDecorator 存在");
    var t13 = typeof(TailwindCSS.ITailwindService);
    Console.WriteLine($"[PASS] ITailwindService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(TailwindCSS.ITailwindThemeService);
    Console.WriteLine($"[PASS] ITailwindThemeService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(TailwindCSS.ITailwindPluginService);
    Console.WriteLine($"[PASS] ITailwindPluginService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(TailwindCSS.IBuildStep);
    Console.WriteLine($"[PASS] IBuildStep 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(TailwindCSS.IPlugin);
    Console.WriteLine($"[PASS] IPlugin 接口存在 (IsInterface: {t17.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}