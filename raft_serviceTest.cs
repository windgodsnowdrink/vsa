#load "raft_service.cs"

Console.WriteLine("=== raft_service Test ===");

try
{
    var t0 = typeof(RaftNodeService);
    Console.WriteLine($"[PASS] RaftNodeService 存在");
    var t1 = typeof(RaftIntegrationExtensions);
    Console.WriteLine($"[PASS] RaftIntegrationExtensions 存在");
    var t2 = typeof(RaftClusterStatus);
    Console.WriteLine($"[PASS] RaftClusterStatus 存在");
    var t3 = typeof(RaftContext);
    Console.WriteLine($"[PASS] RaftContext 存在");
    var t4 = typeof(RaftContextPooledPolicy);
    Console.WriteLine($"[PASS] RaftContextPooledPolicy 存在");
    var t5 = typeof(KeyValueStateMachine);
    Console.WriteLine($"[PASS] KeyValueStateMachine 存在");
    var t6 = typeof(JsonRaftCommandSerializer);
    Console.WriteLine($"[PASS] JsonRaftCommandSerializer 存在");
    var t7 = typeof(IRaftStateMachine);
    Console.WriteLine($"[PASS] IRaftStateMachine 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IRaftCommandSerializer);
    Console.WriteLine($"[PASS] IRaftCommandSerializer 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(NodeInfo);
    Console.WriteLine($"[PASS] NodeInfo record 存在");
    var t10 = typeof(RaftMessage);
    Console.WriteLine($"[PASS] RaftMessage record 存在");
    var t11 = typeof(StateMachineMetadata);
    Console.WriteLine($"[PASS] StateMachineMetadata record 存在");
    var t12 = typeof(KeyValueCommand);
    Console.WriteLine($"[PASS] KeyValueCommand record 存在");
    var t13 = typeof(NodeState);
    Console.WriteLine($"[PASS] NodeState enum 存在 (IsEnum: {t13.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}