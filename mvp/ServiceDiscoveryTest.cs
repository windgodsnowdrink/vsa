#load "ServiceDiscovery.cs"

Console.WriteLine("=== ServiceDiscovery Test ===");

try
{
    var programType = typeof(App.Program);
    Console.WriteLine("[PASS] Program type found: " + programType.FullName);

    var iUserServiceType = typeof(App.IUserService);
    Console.WriteLine("[PASS] IUserService interface found: " + iUserServiceType.FullName);
    var getUserMethod = iUserServiceType.GetMethod("GetUserByIdAsync");
    Console.WriteLine(getUserMethod != null ? "[PASS] IUserService.GetUserByIdAsync exists" : "[FAIL] IUserService.GetUserByIdAsync missing");

    var userServiceType = typeof(App.UserService);
    Console.WriteLine("[PASS] UserService class found: " + userServiceType.FullName);

    var dynamicReverseProxyConfigProviderType = typeof(App.DynamicReverseProxyConfigProvider);
    Console.WriteLine("[PASS] DynamicReverseProxyConfigProvider class found: " + dynamicReverseProxyConfigProviderType.FullName);
    var getConfigMethod = dynamicReverseProxyConfigProviderType.GetMethod("GetConfig");
    Console.WriteLine(getConfigMethod != null ? "[PASS] DynamicReverseProxyConfigProvider.GetConfig exists" : "[FAIL] DynamicReverseProxyConfigProvider.GetConfig missing");

    var serviceDiscoveryConfigType = typeof(App.ServiceDiscoveryConfiguration);
    Console.WriteLine("[PASS] ServiceDiscoveryConfiguration class found: " + serviceDiscoveryConfigType.FullName);
    var configureMethod = serviceDiscoveryConfigType.GetMethod("ConfigureServiceDiscovery");
    Console.WriteLine(configureMethod != null ? "[PASS] ServiceDiscoveryConfiguration.ConfigureServiceDiscovery exists" : "[FAIL] ConfigureServiceDiscovery missing");

    var customEndpointType = typeof(App.CustomServiceEndpointProvider);
    Console.WriteLine("[PASS] CustomServiceEndpointProvider class found: " + customEndpointType.FullName);
    var getEndpointsMethod = customEndpointType.GetMethod("GetEndpointsAsync");
    Console.WriteLine(getEndpointsMethod != null ? "[PASS] CustomServiceEndpointProvider.GetEndpointsAsync exists" : "[FAIL] GetEndpointsAsync missing");

    var controllerType = typeof(App.ServiceDiscoveryController);
    Console.WriteLine("[PASS] ServiceDiscoveryController class found: " + controllerType.FullName);

    var advancedType = typeof(App.AdvancedServiceDiscoveryConfiguration);
    Console.WriteLine("[PASS] AdvancedServiceDiscoveryConfiguration class found: " + advancedType.FullName);

    var clientDemoType = typeof(App.ServiceDiscoveryClientDemo);
    Console.WriteLine("[PASS] ServiceDiscoveryClientDemo class found: " + clientDemoType.FullName);
    var callMethod = clientDemoType.GetMethod("CallServiceWithLoadBalancing");
    Console.WriteLine(callMethod != null ? "[PASS] ServiceDiscoveryClientDemo.CallServiceWithLoadBalancing exists" : "[FAIL] CallServiceWithLoadBalancing missing");

    var monitoringType = typeof(App.ServiceDiscoveryMonitoringService);
    Console.WriteLine("[PASS] ServiceDiscoveryMonitoringService class found: " + monitoringType.FullName);
    var monitorMethod = monitoringType.GetMethod("MonitorServiceDiscovery");
    Console.WriteLine(monitorMethod != null ? "[PASS] ServiceDiscoveryMonitoringService.MonitorServiceDiscovery exists" : "[FAIL] MonitorServiceDiscovery missing");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}