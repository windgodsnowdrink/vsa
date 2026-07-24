#load "ObjectPool.cs"

Console.WriteLine("=== ObjectPool Test ===");

try
{
    var sbPolicyType = typeof(App.StringBuilderPooledObjectPolicy);
    Console.WriteLine("[PASS] StringBuilderPooledObjectPolicy class found: " + sbPolicyType.FullName);
    var createMethod = sbPolicyType.GetMethod("Create");
    Console.WriteLine(createMethod != null ? "[PASS] StringBuilderPooledObjectPolicy.Create exists" : "[FAIL] Create missing");
    var returnMethod = sbPolicyType.GetMethod("Return");
    Console.WriteLine(returnMethod != null ? "[PASS] StringBuilderPooledObjectPolicy.Return exists" : "[FAIL] Return missing");

    var httpPolicyType = typeof(App.HttpClientPooledObjectPolicy);
    Console.WriteLine("[PASS] HttpClientPooledObjectPolicy class found: " + httpPolicyType.FullName);

    var dbConnectionType = typeof(App.DatabaseConnection);
    Console.WriteLine("[PASS] DatabaseConnection class found: " + dbConnectionType.FullName);
    var executeMethod = dbConnectionType.GetMethod("ExecuteQuery");
    Console.WriteLine(executeMethod != null ? "[PASS] DatabaseConnection.ExecuteQuery exists" : "[FAIL] ExecuteQuery missing");
    var connIdProp = dbConnectionType.GetProperty("ConnectionId");
    Console.WriteLine(connIdProp != null ? "[PASS] DatabaseConnection.ConnectionId property exists" : "[FAIL] ConnectionId missing");

    var dbPolicyType = typeof(App.DatabaseConnectionPooledObjectPolicy);
    Console.WriteLine("[PASS] DatabaseConnectionPooledObjectPolicy class found: " + dbPolicyType.FullName);

    var poolServiceType = typeof(App.ObjectPoolService);
    Console.WriteLine("[PASS] ObjectPoolService class found: " + poolServiceType.FullName);
    var buildMethod = poolServiceType.GetMethod("BuildComplexString");
    Console.WriteLine(buildMethod != null ? "[PASS] ObjectPoolService.BuildComplexString exists" : "[FAIL] BuildComplexString missing");
    var callApiMethod = poolServiceType.GetMethod("CallApiAsync");
    Console.WriteLine(callApiMethod != null ? "[PASS] ObjectPoolService.CallApiAsync exists" : "[FAIL] CallApiAsync missing");
    var execDbMethod = poolServiceType.GetMethod("ExecuteDatabaseOperations");
    Console.WriteLine(execDbMethod != null ? "[PASS] ObjectPoolService.ExecuteDatabaseOperations exists" : "[FAIL] ExecuteDatabaseOperations missing");

    var factoryType = typeof(App.ObjectPoolFactory);
    Console.WriteLine("[PASS] ObjectPoolFactory class found: " + factoryType.FullName);

    var startupType = typeof(App.Startup);
    Console.WriteLine("[PASS] Startup class found: " + startupType.FullName);

    var perfTestType = typeof(App.PerformanceTest);
    Console.WriteLine("[PASS] PerformanceTest class found: " + perfTestType.FullName);
    var runMethod = perfTestType.GetMethod("RunPerformanceComparison");
    Console.WriteLine(runMethod != null ? "[PASS] PerformanceTest.RunPerformanceComparison exists" : "[FAIL] RunPerformanceComparison missing");

    var threadSafeType = typeof(App.ThreadSafeObjectPoolUsage);
    Console.WriteLine("[PASS] ThreadSafeObjectPoolUsage class found: " + threadSafeType.FullName);
    var processMethod = threadSafeType.GetMethod("ProcessDataSafe");
    Console.WriteLine(processMethod != null ? "[PASS] ThreadSafeObjectPoolUsage.ProcessDataSafe exists" : "[FAIL] ProcessDataSafe missing");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}