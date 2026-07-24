#load "torchsharp_generator.cs"

Console.WriteLine("=== torchsharp_generator Test ===");

try
{
    var t0 = typeof(TorchSharp.Skill.Generator.ScrutorGenerator);
    Console.WriteLine($"[PASS] ScrutorGenerator 存在");
    var t1 = typeof(TorchSharp.Skill.Generator.HelloService);
    Console.WriteLine($"[PASS] HelloService 存在");
    var t2 = typeof(TorchSharp.Skill.Generator.Calculator);
    Console.WriteLine($"[PASS] Calculator 存在");
    var t3 = typeof(TorchSharp.Skill.Generator.LoggingCalculator);
    Console.WriteLine($"[PASS] LoggingCalculator 存在");
    var t4 = typeof(TorchSharp.Skill.Generator.ServiceA);
    Console.WriteLine($"[PASS] ServiceA 存在");
    var t5 = typeof(TorchSharp.Skill.Generator.ServiceB);
    Console.WriteLine($"[PASS] ServiceB 存在");
    var t6 = typeof(TorchSharp.Skill.Generator.ServiceC);
    Console.WriteLine($"[PASS] ServiceC 存在");
    var t7 = typeof(TorchSharp.Skill.Generator.TransientService);
    Console.WriteLine($"[PASS] TransientService 存在");
    var t8 = typeof(TorchSharp.Skill.Generator.ScopedService);
    Console.WriteLine($"[PASS] ScopedService 存在");
    var t9 = typeof(TorchSharp.Skill.Generator.SingletonService);
    Console.WriteLine($"[PASS] SingletonService 存在");
    var t10 = typeof(TorchSharp.Skill.Generator.Repository);
    Console.WriteLine($"[PASS] Repository 存在");
    var t11 = typeof(TorchSharp.Skill.Generator.Operation);
    Console.WriteLine($"[PASS] Operation 存在");
    var t12 = typeof(TorchSharp.Skill.Generator.LoggingDecorator);
    Console.WriteLine($"[PASS] LoggingDecorator 存在");
    var t13 = typeof(TorchSharp.Skill.Generator.ValidationDecorator);
    Console.WriteLine($"[PASS] ValidationDecorator 存在");
    var t14 = typeof(TorchSharp.Skill.Generator.CachingDecorator);
    Console.WriteLine($"[PASS] CachingDecorator 存在");
    var t15 = typeof(TorchSharp.Skill.Generator.PremiumFeatureService);
    Console.WriteLine($"[PASS] PremiumFeatureService 存在");
    var t16 = typeof(TorchSharp.Skill.Generator.StandardFeatureService);
    Console.WriteLine($"[PASS] StandardFeatureService 存在");
    var t17 = typeof(TorchSharp.Skill.Generator.User);
    Console.WriteLine($"[PASS] User 存在");
    var t18 = typeof(TorchSharp.Skill.Generator.Product);
    Console.WriteLine($"[PASS] Product 存在");
    var t19 = typeof(TorchSharp.Skill.Generator.DemoScanner1);
    Console.WriteLine($"[PASS] DemoScanner1 存在");
    var t20 = typeof(TorchSharp.Skill.Generator.DemoScanner2);
    Console.WriteLine($"[PASS] DemoScanner2 存在");
    var t21 = typeof(TorchSharp.Skill.Generator.IHelloService);
    Console.WriteLine($"[PASS] IHelloService 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(TorchSharp.Skill.Generator.ICalculator);
    Console.WriteLine($"[PASS] ICalculator 接口存在 (IsInterface: {t22.IsInterface})");
    var t23 = typeof(TorchSharp.Skill.Generator.IService);
    Console.WriteLine($"[PASS] IService 接口存在 (IsInterface: {t23.IsInterface})");
    var t24 = typeof(TorchSharp.Skill.Generator.ITransientService);
    Console.WriteLine($"[PASS] ITransientService 接口存在 (IsInterface: {t24.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}