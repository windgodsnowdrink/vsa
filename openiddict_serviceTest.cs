#load "openiddict_service.cs"

Console.WriteLine("=== openiddict_service Test ===");

try
{
    var t0 = typeof(PasswordGrantHandler);
    Console.WriteLine($"[PASS] PasswordGrantHandler 存在");
    var t1 = typeof(AuthorizationCodeGenerator);
    Console.WriteLine($"[PASS] AuthorizationCodeGenerator 存在");
    var t2 = typeof(RefreshTokenHandler);
    Console.WriteLine($"[PASS] RefreshTokenHandler 存在");
    var t3 = typeof(OpenIddictConfig);
    Console.WriteLine($"[PASS] OpenIddictConfig 存在");
    var t4 = typeof(AppJsonSerializerContext);
    Console.WriteLine($"[PASS] AppJsonSerializerContext 存在");
    var t5 = typeof(RefreshTokenPayload);
    Console.WriteLine($"[PASS] RefreshTokenPayload record 存在");
    var t6 = typeof(TokenRequestPayload);
    Console.WriteLine($"[PASS] TokenRequestPayload record 存在");
    var t7 = typeof(AuthorizationCodePayload);
    Console.WriteLine($"[PASS] AuthorizationCodePayload record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}