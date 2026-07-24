#load "autohistory_integration.cs"

using System;

Console.WriteLine("=== autohistory_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: AppDbContext类型存在
    var dbCtxType = typeof(AppDbContext);
    Report("AppDbContext class exists", dbCtxType.IsClass);
    Report("AppDbContext inherits DbContext", typeof(Microsoft.EntityFrameworkCore.DbContext).IsAssignableFrom(dbCtxType));

    // Test 2: CompressedAutoHistory类型存在
    var historyType = typeof(CompressedAutoHistory);
    Report("CompressedAutoHistory class exists", historyType.IsClass);

    // Test 3: HistoryCompressionService类型存在
    var compressionType = typeof(HistoryCompressionService);
    Report("HistoryCompressionService class exists", compressionType.IsClass);

    // Test 4: HistoryCompressionService可实例化
    var compressionService = new HistoryCompressionService();
    Report("HistoryCompressionService instantiated", compressionService != null);

    // Test 5: Compress方法存在
    var compressMethod = compressionType.GetMethod("Compress");
    Report("Compress method exists", compressMethod != null);

    // Test 6: Decompress方法存在
    var decompressMethod = compressionType.GetMethod("Decompress");
    Report("Decompress method exists", decompressMethod != null);

    // Test 7: 压缩/解压往返测试
    var testData = "Hello, AutoHistory! This is a test string for compression.";
    var compressed = compressionService.Compress(testData);
    Report("Compress produces non-null output", compressed != null);
    Report("Compress produces output", compressed.Length > 0);

    var decompressed = compressionService.Decompress(compressed);
    Report("Decompress produces non-null output", decompressed != null);
    Report("Round-trip: data matches", decompressed == testData);

    // Test 8: SetCompressedData方法存在
    var setCompressedMethod = historyType.GetMethod("SetCompressedData");
    Report("SetCompressedData method exists", setCompressedMethod != null);

    // Test 9: GetDecompressedData方法存在
    var getDecompressedMethod = historyType.GetMethod("GetDecompressedData");
    Report("GetDecompressedData method exists", getDecompressedMethod != null);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== autohistory_integration Test Complete ===");