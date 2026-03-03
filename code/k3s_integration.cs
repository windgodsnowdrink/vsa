// 部署管理器
public class KubernetesDeploymentManager
{
    private readonly IKubernetes _client;
    
    public KubernetesDeploymentManager(IKubernetes client)
    {
        _client = client;
    }

    public async Task<V1Deployment> CreateDeploymentAsync(
        string namespaceName,
        string deploymentName,
        string imageName,
        int replicas = 1)
    {
        var deployment = new V1Deployment
        {
            Metadata = new V1ObjectMeta
            {
                Name = deploymentName,
                NamespaceProperty = namespaceName
            },
            Spec = new V1DeploymentSpec
            {
                Replicas = replicas,
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string> { { "app", deploymentName } }
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string> { { "app", deploymentName } }
                    },
                    Spec = new V1PodSpec
                    {
                        Containers = new List<V1Container>
                        {
                            new V1Container
                            {
                                Name = deploymentName,
                                Image = imageName,
                                Ports = new List<V1ContainerPort>
                                {
                                    new V1ContainerPort(80)
                                }
                            }
                        }
                    }
                }
            }
        };

        return await _client.CreateNamespacedDeploymentAsync(deployment, namespaceName);
    }
}

// 服务发现
public class KubernetesServiceDiscovery
{
    private readonly IKubernetes _client;
    
    public KubernetesServiceDiscovery(IKubernetes client)
    {
        _client = client;
    }

    public async Task<V1Service> CreateServiceAsync(
        string namespaceName,
        string serviceName,
        string deploymentName,
        int port = 80)
    {
        var service = new V1Service
        {
            Metadata = new V1ObjectMeta
            {
                Name = serviceName,
                NamespaceProperty = namespaceName
            },
            Spec = new V1ServiceSpec
            {
                Selector = new Dictionary<string, string> { { "app", deploymentName } },
                Ports = new List<V1ServicePort>
                {
                    new V1ServicePort
                    {
                        Port = port,
                        TargetPort = port
                    }
                }
            }
        };

        return await _client.CreateNamespacedServiceAsync(service, namespaceName);
    }
}

// 配置管理
public class KubernetesConfigManager
{
    private readonly IKubernetes _client;
    
    public KubernetesConfigManager(IKubernetes client)
    {
        _client = client;
    }

    public async Task<V1ConfigMap> CreateConfigMapAsync(
        string namespaceName,
        string configMapName,
        IDictionary<string, string> data)
    {
        var configMap = new V1ConfigMap
        {
            Metadata = new V1ObjectMeta
            {
                Name = configMapName,
                NamespaceProperty = namespaceName
            },
            Data = data
        };

        return await _client.CreateNamespacedConfigMapAsync(configMap, namespaceName);
    }
}

public class K3SOptions
{
    public string ConfigPath { get; set; } = "~/.kube/config";
    public string Namespace { get; set; } = "default";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}

public static class K3SExtensions
{
    public static IServiceCollection AddK3S(this IServiceCollection services, 
        Action<K3SOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IKubernetes>(provider => 
        {
            var options = provider.GetRequiredService<K3SOptions>();
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(options.ConfigPath);
            return new Kubernetes(config);
        });
        return services;
    }
}

public class K3SDeploymentManager
{
    private readonly IKubernetes _client;
    private readonly K3SOptions _options;

    public K3SDeploymentManager(IKubernetes client, K3SOptions options)
    {
        _client = client;
        _options = options;
    }

    public async Task<V1Deployment> CreateDeploymentAsync(V1Deployment deployment)
    {
        return await _client.AppsV1.CreateNamespacedDeploymentAsync(
            deployment, 
            _options.Namespace, 
            cancellationToken: new CancellationTokenSource(_options.Timeout).Token);
    }
}