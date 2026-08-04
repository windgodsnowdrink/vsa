//Castle.Core
// Microsoft.Extensions.DependencyInjection

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

// 定义自定义特性  
[AttributeUsage(AttributeTargets.Method)]
publicclass LoggingAttribute : Attribute
{
    publicstring Description { get; }

    public LoggingAttribute(string description = "")
{
    Description = description;
}
}

// 定义接口
public interface IService
{

    void StartDevice(string deviceName);

    void StopDevice(string deviceName);
    void NormalOperation();
}

// 实现接口
publicclass ControlService
    {
        [LoggingAttribute("启动设备")]
        public virtual void StartDevice(string deviceName)
    {
            Console.WriteLine($"Starting device: {deviceName}");
        }

        [LoggingAttribute("停止设备")]
        public virtual void StopDevice(string deviceName)
    {
            Console.WriteLine($"Stopping device: {deviceName}");
        }

    public virtual void NormalOperation()
    {
        Console.WriteLine("Device is operating normally.");
    }
}

// 日志拦截器
publicclass LoggingInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
{
    var methodInfo = invocation.Method;

    // 确保获取特性
    var attribute = methodInfo.GetCustomAttribute<LoggingAttribute>();
    Console.WriteLine($"Method: {methodInfo.Name}");  // 添加检查方法名称
    var allAttributes = methodInfo.GetCustomAttributes();
    foreach (var attr in allAttributes)
    {
        Console.WriteLine($"Attribute: {attr.GetType().Name}"); // 调试输出所有特性
    }

    if (attribute != null)
    {
        string description = !string.IsNullOrEmpty(attribute.Description) ? attribute.Description : "执行方法";

        Console.WriteLine($"[开始] {description}: {methodInfo.Name}");
        Console.WriteLine($"参数: {string.Join(", ", invocation.Arguments)}");

        var startTime = DateTime.Now;

        try
        {
            invocation.Proceed();
            Console.WriteLine($"返回值: {invocation.ReturnValue}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"方法执行异常: {ex.Message}");
            throw;
        }
        finally
        {
            var endTime = DateTime.Now;
            var duration = (endTime - startTime).TotalMilliseconds;
            Console.WriteLine($"[结束] {description}: {methodInfo.Name}，耗时: {duration}ms");
        }
    }
    else
    {
        // 如果没有特性，直接执行   
        invocation.Proceed();
    }
}
}

// 代理工厂
publicstaticclass ProxyFactory
{
    publicstatic T Create<T>() where T : class
    {
    var generator = new ProxyGenerator();
    var interceptor = new LoggingInterceptor();
        return generator.CreateClassProxy<T>(interceptor);
        //return generator.CreateInterfaceProxyWithTarget<T>(obj, interceptor);
    }
}

// 主程序入口
publicclass Program
{
    public static void Main(string[] args)
{
    Console.WriteLine("工控系统 AOP日志记录示例程序开始运行");
    ControlService controlServiceObj = new ControlService();
    var controlService = ProxyFactory.Create<ControlService>();

    controlService.StartDevice("Device_001");
    controlService.StopDevice("Device_001");
    controlService.NormalOperation();

    Console.WriteLine("\n程序结束");
    Console.ReadKey();
}
}