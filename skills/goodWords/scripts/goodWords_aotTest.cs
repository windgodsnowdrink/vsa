#load "goodWords_aot.cs"

Console.WriteLine("=== goodWords_aot Test ===");

try
{
    var t0 = typeof(GoodWordsAOT.GoodWordsEngine);
    Console.WriteLine($"[PASS] GoodWordsEngine 存在");
    var t1 = typeof(GoodWordsAOT.GoodWord);
    Console.WriteLine($"[PASS] GoodWord 存在");
    var t2 = typeof(GoodWordsAOT.GoodWordsOptions);
    Console.WriteLine($"[PASS] GoodWordsOptions 存在");
    var t3 = typeof(GoodWordsAOT.GoodWordsCommandHandler);
    Console.WriteLine($"[PASS] GoodWordsCommandHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}