#load "Polly.cs"

Console.WriteLine("=== Polly Test ===");

try
{
    var programType = typeof(App.Program);
    Console.WriteLine("[PASS] Program type found: " + programType.FullName);

    var iUserServiceClientType = typeof(App.IUserServiceClient);
    Console.WriteLine("[PASS] IUserServiceClient interface found: " + iUserServiceClientType.FullName);
    var getUserMethod = iUserServiceClientType.GetMethod("GetUserByIdAsync");
    Console.WriteLine(getUserMethod != null ? "[PASS] IUserServiceClient.GetUserByIdAsync exists" : "[FAIL] GetUserByIdAsync missing");

    var userType = typeof(App.User);
    Console.WriteLine("[PASS] User class found: " + userType.FullName);

    var userServiceClientType = typeof(App.UserServiceClient);
    Console.WriteLine("[PASS] UserServiceClient class found: " + userServiceClientType.FullName);

    var pollyPolicyConfigType = typeof(App.PollyPolicyConfiguration);
    Console.WriteLine("[PASS] PollyPolicyConfiguration class found: " + pollyPolicyConfigType.FullName);
    var retryMethod = pollyPolicyConfigType.GetMethod("ConfigureRetryPolicy");
    Console.WriteLine(retryMethod != null ? "[PASS] PollyPolicyConfiguration.ConfigureRetryPolicy exists" : "[FAIL] ConfigureRetryPolicy missing");
    var cbMethod = pollyPolicyConfigType.GetMethod("ConfigureCircuitBreakerPolicy");
    Console.WriteLine(cbMethod != null ? "[PASS] PollyPolicyConfiguration.ConfigureCircuitBreakerPolicy exists" : "[FAIL] ConfigureCircuitBreakerPolicy missing");
    var timeoutMethod = pollyPolicyConfigType.GetMethod("ConfigureTimeoutPolicy");
    Console.WriteLine(timeoutMethod != null ? "[PASS] PollyPolicyConfiguration.ConfigureTimeoutPolicy exists" : "[FAIL] ConfigureTimeoutPolicy missing");
    var fallbackMethod = pollyPolicyConfigType.GetMethod("ConfigureFallbackPolicy");
    Console.WriteLine(fallbackMethod != null ? "[PASS] PollyPolicyConfiguration.ConfigureFallbackPolicy exists" : "[FAIL] ConfigureFallbackPolicy missing");

    var httpClientConfigType = typeof(App.HttpClientServiceConfiguration);
    Console.WriteLine("[PASS] HttpClientServiceConfiguration class found: " + httpClientConfigType.FullName);

    var configBasedRetryType = typeof(App.ConfigurationBasedRetryPolicyProvider);
    Console.WriteLine("[PASS] ConfigurationBasedRetryPolicyProvider class found: " + configBasedRetryType.FullName);

    var customEvalType = typeof(App.CustomPolicyEvaluator);
    Console.WriteLine("[PASS] CustomPolicyEvaluator class found: " + customEvalType.FullName);

    var policyStateManagerType = typeof(App.PolicyStateManager);
    Console.WriteLine("[PASS] PolicyStateManager class found: " + policyStateManagerType.FullName);

    var httpServiceInvokerType = typeof(App.HttpServiceInvoker);
    Console.WriteLine("[PASS] HttpServiceInvoker class found: " + httpServiceInvokerType.FullName);

    var orderType = typeof(App.Order);
    Console.WriteLine("[PASS] Order class found: " + orderType.FullName);

    var monitoringType = typeof(App.PollyMonitoringService);
    Console.WriteLine("[PASS] PollyMonitoringService class found: " + monitoringType.FullName);

    var productionConfigType = typeof(App.ProductionPollyConfiguration);
    Console.WriteLine("[PASS] ProductionPollyConfiguration class found: " + productionConfigType.FullName);

    var healthCheckType = typeof(App.PollyHealthCheck);
    Console.WriteLine("[PASS] PollyHealthCheck class found: " + healthCheckType.FullName);

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}