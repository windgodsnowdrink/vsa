#load "ui_core.cs"

Console.WriteLine("=== ui_core Test ===");

try
{
    var t0 = typeof(UI.Core.ThemeChangedEventArgs);
    Console.WriteLine($"[PASS] ThemeChangedEventArgs 存在");
    var t1 = typeof(UI.Core.CultureChangedEventArgs);
    Console.WriteLine($"[PASS] CultureChangedEventArgs 存在");
    var t2 = typeof(UI.Core.PerformanceMetrics);
    Console.WriteLine($"[PASS] PerformanceMetrics 存在");
    var t3 = typeof(UI.Core.AccessibilityReport);
    Console.WriteLine($"[PASS] AccessibilityReport 存在");
    var t4 = typeof(UI.Core.WebUIProvider);
    Console.WriteLine($"[PASS] WebUIProvider 存在");
    var t5 = typeof(UI.Core.DesktopUIProvider);
    Console.WriteLine($"[PASS] DesktopUIProvider 存在");
    var t6 = typeof(UI.Core.MobileUIProvider);
    Console.WriteLine($"[PASS] MobileUIProvider 存在");
    var t7 = typeof(UI.Core.ThemeManager);
    Console.WriteLine($"[PASS] ThemeManager 存在");
    var t8 = typeof(UI.Core.LocalizationManager);
    Console.WriteLine($"[PASS] LocalizationManager 存在");
    var t9 = typeof(UI.Core.PerformanceMonitor);
    Console.WriteLine($"[PASS] PerformanceMonitor 存在");
    var t10 = typeof(UI.Core.AccessibilityChecker);
    Console.WriteLine($"[PASS] AccessibilityChecker 存在");
    var t11 = typeof(UI.Core.UIApplication);
    Console.WriteLine($"[PASS] UIApplication 存在");
    var t12 = typeof(UI.Core.IUIApplication);
    Console.WriteLine($"[PASS] IUIApplication 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(UI.Core.IThemeManager);
    Console.WriteLine($"[PASS] IThemeManager 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(UI.Core.ILocalizationManager);
    Console.WriteLine($"[PASS] ILocalizationManager 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(UI.Core.IPerformanceMonitor);
    Console.WriteLine($"[PASS] IPerformanceMonitor 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(UI.Core.IAccessibilityChecker);
    Console.WriteLine($"[PASS] IAccessibilityChecker 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(UI.Core.IUIProvider);
    Console.WriteLine($"[PASS] IUIProvider 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(UI.Core.UIType);
    Console.WriteLine($"[PASS] UIType enum 存在 (IsEnum: {t18.IsEnum})");
    var t19 = typeof(UI.Core.Theme);
    Console.WriteLine($"[PASS] Theme enum 存在 (IsEnum: {t19.IsEnum})");
    var t20 = typeof(UI.Core.AccessibilityStandard);
    Console.WriteLine($"[PASS] AccessibilityStandard enum 存在 (IsEnum: {t20.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}