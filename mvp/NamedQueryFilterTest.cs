#load "NamedQueryFilter.cs"

Console.WriteLine("=== NamedQueryFilter Test ===");

try
{
    var blogPostType = typeof(BlogPost);
    Console.WriteLine("[PASS] BlogPost class found: " + blogPostType.FullName);
    var idProp = blogPostType.GetProperty("Id");
    Console.WriteLine(idProp != null ? "[PASS] BlogPost.Id property exists" : "[FAIL] Id missing");
    var titleProp = blogPostType.GetProperty("Title");
    Console.WriteLine(titleProp != null ? "[PASS] BlogPost.Title property exists" : "[FAIL] Title missing");
    var updatedAtProp = blogPostType.GetProperty("UpdatedAt");
    Console.WriteLine(updatedAtProp != null ? "[PASS] BlogPost.UpdatedAt property exists" : "[FAIL] UpdatedAt missing");
    var updatedByProp = blogPostType.GetProperty("UpdatedBy");
    Console.WriteLine(updatedByProp != null ? "[PASS] BlogPost.UpdatedBy property exists" : "[FAIL] UpdatedBy missing");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}