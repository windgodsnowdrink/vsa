#load "benchmark_demo.cs"

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== benchmark_demo Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: TodoItem类型存在
    var todoType = typeof(TodoItem);
    Report("TodoItem class exists", todoType.IsClass);

    // Test 2: TodoItem可实例化
    var todo = new TodoItem { Id = 1, Title = "Test", IsCompleted = false };
    Report("TodoItem instantiated", todo != null);
    Report("TodoItem Title", todo.Title == "Test");
    Report("TodoItem IsCompleted", todo.IsCompleted == false);

    // Test 3: TodoDbContext类型存在
    var dbCtxType = typeof(TodoDbContext);
    Report("TodoDbContext class exists", dbCtxType.IsClass);
    Report("TodoDbContext inherits DbContext", typeof(DbContext).IsAssignableFrom(dbCtxType));

    // Test 4: TodoDbContext可实例化
    var db = new TodoDbContext();
    Report("TodoDbContext instantiated", db != null);

    // Test 5: Todos DbSet存在
    var todosSet = db.Todos;
    Report("Todos DbSet exists", todosSet != null);

    // Test 6: TodoBenchmark类型存在
    var benchmarkType = typeof(TodoBenchmark);
    Report("TodoBenchmark class exists", benchmarkType.IsClass);

    // Test 7: TodoBenchmark有MemoryDiagnoser属性
    var memDiag = benchmarkType.GetCustomAttributes(typeof(MemoryDiagnoser), false);
    Report("TodoBenchmark has MemoryDiagnoser", memDiag.Length > 0);

    // Test 8: Benchmark methods存在
    var createMethod = benchmarkType.GetMethod("CreateTodo");
    Report("CreateTodo benchmark method exists", createMethod != null);
    Report("CreateTodo has BenchmarkAttribute", createMethod.GetCustomAttributes(typeof(BenchmarkAttribute), false).Length > 0);

    var readMethod = benchmarkType.GetMethod("ReadTodo");
    Report("ReadTodo benchmark method exists", readMethod != null);

    var updateMethod = benchmarkType.GetMethod("UpdateTodo");
    Report("UpdateTodo benchmark method exists", updateMethod != null);

    var deleteMethod = benchmarkType.GetMethod("DeleteTodo");
    Report("DeleteTodo benchmark method exists", deleteMethod != null);

    var channelMethod = benchmarkType.GetMethod("ChannelWriteRead");
    Report("ChannelWriteRead benchmark method exists", channelMethod != null);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== benchmark_demo Test Complete ===");