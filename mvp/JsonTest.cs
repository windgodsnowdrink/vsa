#load "Json.cs"

Console.WriteLine("=== Json (HttpJson) Test ===");

try
{
    var programType = typeof(App.Program);
    Console.WriteLine("[PASS] Program type found: " + programType.FullName);

    var userType = typeof(App.User);
    Console.WriteLine("[PASS] User class found: " + userType.FullName);
    var idProp = userType.GetProperty("Id");
    Console.WriteLine(idProp != null ? "[PASS] User.Id property exists" : "[FAIL] Id missing");
    var userNameProp = userType.GetProperty("UserName");
    Console.WriteLine(userNameProp != null ? "[PASS] User.UserName property exists" : "[FAIL] UserName missing");

    var apiResponseType = typeof(App.ApiResponse<>);
    Console.WriteLine("[PASS] ApiResponse<T> class found: " + apiResponseType.FullName);
    var successProp = apiResponseType.GetProperty("Success");
    Console.WriteLine(successProp != null ? "[PASS] ApiResponse<T>.Success property exists" : "[FAIL] Success missing");

    var userQueryRequestType = typeof(App.UserQueryRequest);
    Console.WriteLine("[PASS] UserQueryRequest class found: " + userQueryRequestType.FullName);

    var httpJsonClientExceptionType = typeof(App.HttpJsonClientException);
    Console.WriteLine("[PASS] HttpJsonClientException class found: " + httpJsonClientExceptionType.FullName);

    var apiValidationExceptionType = typeof(App.ApiValidationException);
    Console.WriteLine("[PASS] ApiValidationException class found: " + apiValidationExceptionType.FullName);

    var iHttpJsonClientFactoryType = typeof(App.IHttpJsonClientFactory);
    Console.WriteLine("[PASS] IHttpJsonClientFactory interface found: " + iHttpJsonClientFactoryType.FullName);

    var httpJsonClientFactoryType = typeof(App.HttpJsonClientFactory);
    Console.WriteLine("[PASS] HttpJsonClientFactory class found: " + httpJsonClientFactoryType.FullName);

    var httpClientConfigType = typeof(App.HttpClientConfiguration);
    Console.WriteLine("[PASS] HttpClientConfiguration class found: " + httpClientConfigType.FullName);

    var httpJsonClientServiceType = typeof(App.HttpJsonClientService);
    Console.WriteLine("[PASS] HttpJsonClientService class found: " + httpJsonClientServiceType.FullName);
    var getAsyncMethod = httpJsonClientServiceType.GetMethod("GetAsync");
    Console.WriteLine(getAsyncMethod != null ? "[PASS] HttpJsonClientService.GetAsync exists" : "[FAIL] GetAsync missing");
    var postAsyncMethod = httpJsonClientServiceType.GetMethod("PostAsync");
    Console.WriteLine(postAsyncMethod != null ? "[PASS] HttpJsonClientService.PostAsync exists" : "[FAIL] PostAsync missing");

    var apiServiceQueryHandlerType = typeof(App.ApiServiceQueryHandler);
    Console.WriteLine("[PASS] ApiServiceQueryHandler class found: " + apiServiceQueryHandlerType.FullName);

    var streamingJsonHandlerType = typeof(App.StreamingJsonHandler);
    Console.WriteLine("[PASS] StreamingJsonHandler class found: " + streamingJsonHandlerType.FullName);

    var apiClientServiceType = typeof(App.ApiClientService);
    Console.WriteLine("[PASS] ApiClientService class found: " + apiClientServiceType.FullName);

    var telemetryType = typeof(App.HttpJsonTelemetryService);
    Console.WriteLine("[PASS] HttpJsonTelemetryService class found: " + telemetryType.FullName);

    var apiPerformanceMetricsType = typeof(App.ApiPerformanceMetrics);
    Console.WriteLine("[PASS] ApiPerformanceMetrics class found: " + apiPerformanceMetricsType.FullName);

    var apiEndpointMetricsType = typeof(App.ApiEndpointMetrics);
    Console.WriteLine("[PASS] ApiEndpointMetrics class found: " + apiEndpointMetricsType.FullName);

    var middlewareType = typeof(App.HttpJsonMiddleware);
    Console.WriteLine("[PASS] HttpJsonMiddleware class found: " + middlewareType.FullName);

    var cacheEntryType = typeof(App.CacheEntry<>);
    Console.WriteLine("[PASS] CacheEntry<T> class found: " + cacheEntryType.FullName);

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}