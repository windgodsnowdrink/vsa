#load "avalonia_integration.cs"

using System;
using AvaloniaIntegration;

Console.WriteLine("=== avalonia_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: AvaloniaOptions默认值
    var opts = new AvaloniaOptions();
    Report("AvaloniaOptions created", opts != null);
    Report("AvaloniaOptions default Title", opts.Title == "Avalonia App");
    Report("AvaloniaOptions default Width", opts.Width == 800);
    Report("AvaloniaOptions default Height", opts.Height == 600);
    Report("AvaloniaOptions default UseReactiveUI", opts.UseReactiveUI == true);
    Report("AvaloniaOptions default DefaultTheme", opts.DefaultTheme == "Light");

    // Test 2: IAvaloniaService接口存在
    var ifaceType = typeof(IAvaloniaService);
    Report("IAvaloniaService interface exists", ifaceType.IsInterface);

    // Test 3: AvaloniaService类型存在
    var serviceType = typeof(AvaloniaService);
    Report("AvaloniaService class exists", serviceType.IsClass);

    // Test 4: AvaloniaService实现IAvaloniaService
    Report("AvaloniaService implements IAvaloniaService", typeof(IAvaloniaService).IsAssignableFrom(serviceType));

    // Test 5: MainViewModel类型存在
    var vmType = typeof(MainViewModel);
    Report("MainViewModel class exists", vmType.IsClass);

    // Test 6: MainWindow类型存在
    var windowType = typeof(MainWindow);
    Report("MainWindow class exists", windowType.IsClass);

    // Test 7: ServiceCollectionExtensions类型存在
    var extType = typeof(ServiceCollectionExtensions);
    Report("ServiceCollectionExtensions class exists", extType.IsClass);

    // Test 8: AddAvalonia扩展方法存在
    var addMethod = extType.GetMethod("AddAvalonia");
    Report("AddAvalonia extension method exists", addMethod != null);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== avalonia_integration Test Complete ===");