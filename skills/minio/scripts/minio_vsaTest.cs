#load "minio_vsa.cs"

Console.WriteLine("=== minio_vsa Test ===");

try
{
    var t0 = typeof(Vertical.Slice.Template.Features.Minio.Upload.MinioEndpoints);
    Console.WriteLine($"[PASS] MinioEndpoints 存在");
    var t1 = typeof(Vertical.Slice.Template.Features.Minio.Upload.UploadProgress);
    Console.WriteLine($"[PASS] UploadProgress record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}