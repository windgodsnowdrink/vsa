#:sdk Microsoft.NET.Sdk.Web
#:package KubernetesClient@6.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class KubernetesIntegrationOptions
{
    public string ConfigPath { get; set; }
    public string Namespace { get; set; }
}

public static class KubernetesServiceCollectionExtensions
{
    public static IServiceCollection AddKubernetesIntegration(this IServiceCollection services, Action<KubernetesIntegrationOptions> configure)
    {
        services.Configure(configure);
        
        services.AddSingleton<IKubernetes>(provider => 
        {
            var options = provider.GetRequiredService<IOptions<KubernetesIntegrationOptions>>().Value;
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(options.ConfigPath);
            return new Kubernetes(config);
        });
        
        services.AddSingleton<KubernetesDeploymentManager>();
        services.AddSingleton<KubernetesServiceDiscovery>();
        services.AddSingleton<KubernetesConfigManager>();
        
        return services;
    }
}

public class KubernetesDeploymentManager
{
    private readonly IKubernetes _client;
    private readonly IOptions<KubernetesIntegrationOptions> _options;
    
    public KubernetesDeploymentManager(IKubernetes client, IOptions<KubernetesIntegrationOptions> options)
    {
        _client = client;
        _options = options;
    }
    
    public async Task<V1Deployment> CreateDeploymentAsync(V1Deployment deployment)
    {
        return await _client.AppsV1.CreateNamespacedDeploymentAsync(deployment, _options.Value.Namespace);
    }
    
    public async Task DeleteDeploymentAsync(string name)
    {
        await _client.AppsV1.DeleteNamespacedDeploymentAsync(name, _options.Value.Namespace);
    }
}

public class KubernetesServiceDiscovery
{
    private readonly IKubernetes _client;
    private readonly IOptions<KubernetesIntegrationOptions> _options;
    
    public KubernetesServiceDiscovery(IKubernetes client, IOptions<KubernetesIntegrationOptions> options)
    {
        _client = client;
        _options = options;
    }
    
    public async Task<V1Service> GetServiceAsync(string name)
    {
        return await _client.CoreV1.ReadNamespacedServiceAsync(name, _options.Value.Namespace);
    }
}

public class KubernetesConfigManager
{
    private readonly IKubernetes _client;
    private readonly IOptions<KubernetesIntegrationOptions> _options;
    
    public KubernetesConfigManager(IKubernetes client, IOptions<KubernetesIntegrationOptions> options)
    {
        _client = client;
        _options = options;
    }
    
    public async Task<V1ConfigMap> GetConfigMapAsync(string name)
    {
        return await _client.CoreV1.ReadNamespacedConfigMapAsync(name, _options.Value.Namespace);
    }
}

public class KubernetesCrdManager<T> where T : IKubernetesObject, IMetadata<V1ObjectMeta>
{
    private readonly IKubernetes _client;
    private readonly IOptions<KubernetesIntegrationOptions> _options;
    
    public KubernetesCrdManager(IKubernetes client, IOptions<KubernetesIntegrationOptions> options)
    {
        _client = client;
        _options = options;
    }
    
    public async Task<T> CreateCustomResourceAsync(T resource)
    {
        return await _client.CustomObjects.CreateNamespacedCustomObjectAsync<T>(resource, _options.Value.Namespace);
    }
}

public class KubernetesAutoScaler
{
    private readonly IKubernetes _client;
    private readonly IOptions<KubernetesIntegrationOptions> _options;
    
    public KubernetesAutoScaler(IKubernetes client, IOptions<KubernetesIntegrationOptions> options)
    {
        _client = client;
        _options = options;
    }
    
    public async Task ScaleDeploymentAsync(string deploymentName, int replicas)
    {
        var patch = new V1Patch($"{{\"spec\":{{\"replicas\":{replicas}}}}}", V1Patch.PatchType.MergePatch);
        await _client.AppsV1.PatchNamespacedDeploymentScaleAsync(patch, deploymentName, _options.Value.Namespace);
    }
}

public static class KubernetesServiceCollectionExtensions
{
    public static IServiceCollection AddKubernetesIntegration(this IServiceCollection services, Action<KubernetesIntegrationOptions> configure)
    {
        services.Configure(configure);
        
        services.AddSingleton<IKubernetes>(provider => 
        {
            var options = provider.GetRequiredService<IOptions<KubernetesIntegrationOptions>>().Value;
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(options.ConfigPath);
            return new Kubernetes(config);
        });
        
        services.AddSingleton<KubernetesDeploymentManager>();
        services.AddSingleton<KubernetesServiceDiscovery>();
        services.AddSingleton<KubernetesConfigManager>();
        services.AddSingleton<KubernetesAutoScaler>();
        services.AddSingleton(typeof(KubernetesCrdManager<>));
        
        return services;
    }
}