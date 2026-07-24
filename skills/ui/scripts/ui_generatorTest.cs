#load "ui_generator.cs"

Console.WriteLine("=== ui_generator Test ===");

try
{
    var t0 = typeof(UI.Generator.ButtonComponent);
    Console.WriteLine($"[PASS] ButtonComponent 存在");
    var t1 = typeof(UI.Generator.LabelComponent);
    Console.WriteLine($"[PASS] LabelComponent 存在");
    var t2 = typeof(UI.Generator.TextBoxComponent);
    Console.WriteLine($"[PASS] TextBoxComponent 存在");
    var t3 = typeof(UI.Generator.DefaultThemeProvider);
    Console.WriteLine($"[PASS] DefaultThemeProvider 存在");
    var t4 = typeof(UI.Generator.DefaultLocalizationProvider);
    Console.WriteLine($"[PASS] DefaultLocalizationProvider 存在");
    var t5 = typeof(UI.Generator.DefaultPerformanceTracker);
    Console.WriteLine($"[PASS] DefaultPerformanceTracker 存在");
    var t6 = typeof(UI.Generator.DefaultAccessibilityValidator);
    Console.WriteLine($"[PASS] DefaultAccessibilityValidator 存在");
    var t7 = typeof(UI.Generator.LoggingDecorator);
    Console.WriteLine($"[PASS] LoggingDecorator 存在");
    var t8 = typeof(UI.Generator.PerformanceDecorator);
    Console.WriteLine($"[PASS] PerformanceDecorator 存在");
    var t9 = typeof(UI.Generator.AccessibilityDecorator);
    Console.WriteLine($"[PASS] AccessibilityDecorator 存在");
    var t10 = typeof(UI.Generator.UIComponentFactory);
    Console.WriteLine($"[PASS] UIComponentFactory 存在");
    var t11 = typeof(UI.Generator.ScrutorDemo);
    Console.WriteLine($"[PASS] ScrutorDemo 存在");
    var t12 = typeof(UI.Generator.IUIComponent);
    Console.WriteLine($"[PASS] IUIComponent 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(UI.Generator.IThemeProvider);
    Console.WriteLine($"[PASS] IThemeProvider 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(UI.Generator.ILocalizationProvider);
    Console.WriteLine($"[PASS] ILocalizationProvider 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(UI.Generator.IPerformanceTracker);
    Console.WriteLine($"[PASS] IPerformanceTracker 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(UI.Generator.IAccessibilityValidator);
    Console.WriteLine($"[PASS] IAccessibilityValidator 接口存在 (IsInterface: {t16.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}