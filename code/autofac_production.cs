#:sdk Microsoft.NET.Sdk
#:package Autofac@8.0.0
#:package Autofac.Extensions.DependencyInjection@8.0.0
#:package Autofac.Extras.DynamicProxy@6.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.LazyDependencies;
using Autofac.Multitenant;
using Autofac.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#:package Autofac@8.0.0
#:package Autofac.Extensions.DependencyInjection@8.0.0
#:package Autofac.Extras.DynamicProxy@6.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using Autofac;
using Autofac.Extras.DynamicProxy;
using System.Diagnostics;

// 1. 核心模块注册
public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 单例注册
        builder.RegisterType<CacheManager>()
            .As<ICacheManager>()
            .SingleInstance();
            
        // 带AOP拦截的注册
        builder.RegisterType<OrderService>()
            .As<IOrderService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(CallLogger));
            
        // 泛型注册
        builder.RegisterGeneric(typeof(Repository<>))
            .As(typeof(IRepository<>))
            .InstancePerLifetimeScope();
    }
}

// 2. AOP拦截器
public class CallLogger : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            invocation.Proceed();
        }
        finally
        {
            sw.Stop();
            Console.WriteLine($"{invocation.Method.Name} executed in {sw.ElapsedMilliseconds}ms");
        }
    }
}

// 3. 容器构建与配置
public static class AutofacConfig
{
    public static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        
        // 注册模块
        builder.RegisterModule<CoreModule>();
        
        // 属性注入配置
        builder.RegisterType<ReportService>()
            .As<IReportService>()
            .PropertiesAutowired();
            
        // 程序集扫描注册
        builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces();
            
        return builder.Build();
    }
}

// 4. 与ASP.NET Core集成
public static class AutofacExtensions
{
    public static void AddAutofacProduction(this IServiceCollection services)
    {
        services.AddAutofac();
    }
    
    public static void UseAutofacProduction(this IApplicationBuilder app)
    {
        app.ApplicationServices.GetAutofacRoot();
    }
}

// 生产级最佳实践
public static class AutofacProductionExtensions
{
    // Registration Sources
    public static ContainerBuilder AddRegistrationSource<T>(this ContainerBuilder builder) where T : IRegistrationSource
    {
        builder.RegisterSource<T>();
        return builder;
    }

    // Adapters and Decorators
    public static ContainerBuilder AddAdapterDecorator<TService, TAdapter, TDecorator>(this ContainerBuilder builder)
        where TAdapter : TService
        where TDecorator : TService
    {
        builder.RegisterType<TAdapter>().As<TService>();
        builder.RegisterDecorator<TDecorator, TService>();
        return builder;
    }

    // Composites
    public static ContainerBuilder AddComposite<TService>(this ContainerBuilder builder, params Type[] implementations)
    {
        builder.RegisterComposite(typeof(Composite<>).MakeGenericType(typeof(TService)), typeof(TService));
        foreach (var impl in implementations)
            builder.RegisterType(impl).As<TService>();
        return builder;
    }

    // Circular Dependencies
    public static ContainerBuilder EnableCircularDependencies(this ContainerBuilder builder)
    {
        builder.RegisterType<CircularDependencyHandler>().AsSelf();
        return builder;
    }

    // Component Metadata / Attribute Metadata
    public static ContainerBuilder AddMetadataRegistration<T>(this ContainerBuilder builder, IDictionary<string, object> metadata)
    {
        builder.RegisterType<T>().WithMetadata(metadata);
        return builder;
    }

    // Named and Keyed Services
    public static ContainerBuilder AddNamedService<TService>(this ContainerBuilder builder, string serviceName)
    {
        builder.RegisterType<TService>().Named<TService>(serviceName);
        return builder;
    }

    // Delegate Factories
    public static ContainerBuilder AddDelegateFactory<T>(this ContainerBuilder builder, Func<IComponentContext, T> factory)
    {
        builder.Register(ctx => factory(ctx)).As<T>();
        return builder;
    }

    // Owned Instances
    public static ContainerBuilder AddOwnedInstance<T>(this ContainerBuilder builder)
    {
        builder.RegisterType<T>().OwnedByLifetimeScope();
        return builder;
    }

    // Pooled Instances
    public static ContainerBuilder AddPooledInstance<T>(this ContainerBuilder builder, int poolSize) where T : class
    {
        builder.RegisterType<T>().InstancePerLifetimeScope()
            .UsingConstructor(new TypedParameter(typeof(int), poolSize));
        return builder;
    }

    // Custom Constructor Selection
    public static ContainerBuilder UseCustomConstructor<T>(this ContainerBuilder builder, params Type[] parameterTypes)
    {
        builder.RegisterType<T>().UsingConstructor(parameterTypes);
        return builder;
    }

    // Handling Concurrency
    public static ContainerBuilder AddConcurrentRegistration(this ContainerBuilder builder)
    {
        builder.RegisterType<ConcurrentComponent>().SingleInstance();
        return builder;
    }

    // Multitenant Applications
    public static ContainerBuilder AddTenantSpecificService<TService, TTenantService>(this ContainerBuilder builder)
        where TTenantService : TService
    {
        builder.RegisterType<TTenantService>().As<TService>().InstancePerTenant();
        return builder;
    }

    // AssemblyLoadContext and Lifetime Scopes
    public static ContainerBuilder AddLoadContextAwareComponent<T>(this ContainerBuilder builder)
    {
        builder.RegisterType<T>().ExternallyOwned();
        return builder;
    }

    // Resolve Pipelines
    public static ContainerBuilder AddPipelineComponent<T>(this ContainerBuilder builder)
    {
        builder.RegisterType<T>().AsImplementedInterfaces().EnableInterfaceInterceptors();
        return builder;
    }

    // Service Pipelines vs Registration Pipelines
    public static ContainerBuilder AddServicePipeline<T>(this ContainerBuilder builder)
    {
        builder.RegisterType<T>().AsSelf().AsImplementedInterfaces();
        return builder;
    }

    // Pipeline Phases
    public static ContainerBuilder AddPhaseComponent<T>(this ContainerBuilder builder, int phase)
    {
        builder.RegisterType<T>().WithMetadata("Phase", phase);
        return builder;
    }

    // Adding Registration Middleware
    public static ContainerBuilder AddRegistrationMiddleware<T>(this ContainerBuilder builder) where T : IRegistrationSource
    {
        builder.RegisterSource<T>();
        return builder;
    }

    // ResolveRequestContext
    public static ContainerBuilder AddResolveContextAware<T>(this ContainerBuilder builder)
    {
        builder.RegisterType<T>().OnActivated(e => e.Context.Resolve<ResolveRequestContext>());
        return builder;
    }

    // Adding Service Middleware
    public static ContainerBuilder AddServiceMiddleware<T>(this ContainerBuilder builder)
    {
        builder.RegisterType<T>().As<IServiceMiddleware>();
        return builder;
    }

    // Service Middleware Sources
    public static ContainerBuilder AddMiddlewareSource<T>(this ContainerBuilder builder) where T : IRegistrationSource
    {
        builder.RegisterSource<T>();
        return builder;
    }

    // Aggregate Services
    public static ContainerBuilder AddAggregateService<T>(this ContainerBuilder builder)
    {
        builder.RegisterAggregateService<T>();
        return builder;
    }

    // Type Interceptors
    public static ContainerBuilder AddTypeInterceptor<T>(this ContainerBuilder builder)
    {
        builder.RegisterType<T>().EnableClassInterceptors();
        return builder;
    }

    // Cross-Platform and Native Applications
    public static ContainerBuilder AddPlatformSpecificService<TInterface, TImplementation>(this ContainerBuilder builder)
        where TImplementation : TInterface
    {
        builder.RegisterType<TImplementation>().As<TInterface>();
        return builder;
    }
    // 1. 增强的多租户支持
    public static ContainerBuilder AddMultiTenantSupport(this ContainerBuilder builder, Action<IContainer, TenantIdentificationStrategy> tenantConfig)
    {
        var strategy = new TenantIdentificationStrategy();
        var mtc = new MultitenantContainer(strategy, builder.Build());
        tenantConfig(mtc, strategy);
        return builder;
    }

    // 2. 延迟加载
    public static ContainerBuilder AddLazyResolution(this ContainerBuilder builder)
    {
        builder.RegisterSource(new LazyRegistrationSource());
        return builder;
    }

    // 3. 装饰器模式
    public static ContainerBuilder AddDecorator<TService, TDecorator>(this ContainerBuilder builder)
        where TDecorator : TService
    {
        builder.RegisterDecorator<TDecorator, TService>();
        return builder;
    }

    // 4. 动态代理
    public static ContainerBuilder AddInterfaceInterceptor<TInterface>(this ContainerBuilder builder)
    {
        builder.RegisterType<TInterface>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(CallLogger));
        return builder;
    }

    // 5. 模块化配置
    public static ContainerBuilder AddConfigurationModules(this ContainerBuilder builder, params IModule[] modules)
    {
        foreach (var module in modules)
        {
            builder.RegisterModule(module);
        }
        return builder;
    }
}

// 租户识别策略
public class TenantIdentificationStrategy : ITenantIdentificationStrategy
{
    public bool TryIdentifyTenant(out object tenantId)
    {
        // 实现租户识别逻辑
        tenantId = null;
        return false;
    }
}

// 动态代理示例
public class CallLogger : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine($"Calling method {invocation.Method.Name}");
        invocation.Proceed();
        Console.WriteLine($"Finished calling {invocation.Method.Name}");
    }
}

// 示例接口和实现
public interface IOrderService {}
public class OrderService : IOrderService {}
public interface ICacheManager {}
public class CacheManager : ICacheManager {}
public interface IRepository<T> {}
public class Repository<T> : IRepository<T> {}
public interface IReportService {}
public class ReportService : IReportService {}
public class TenantContext { public TenantContext(string id) {} }
public class ExpensiveResource { public void DoWork() {} }

// 新增示例类用于高级场景
public class Composite<T> : T where T : class {}
public class CircularDependencyHandler {}
public class ConcurrentComponent {}
public interface IServiceMiddleware {}
public class CustomRegistrationSource : IRegistrationSource
{
    public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
    {
        yield break;
    }
    public bool IsAdapterForIndividualComponents => false;
}
public class PlatformSpecificService : IPlatformSpecificService {}
public interface IPlatformSpecificService {}