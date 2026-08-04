#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Net.Sockets@10.0.0
#:package System.Net.Http@10.0.0
#:package System.Threading.Channels@10.0.0
#:package System.Runtime.CompilerServices.Unsafe@6.0.0
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace P2P
{
    public interface IP2PService
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
        Task ShutdownAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<P2PNode>> DiscoverNodesAsync(CancellationToken cancellationToken = default);
        Task<P2PNode> GetNodeAsync(string nodeId, CancellationToken cancellationToken = default);
        Task<bool> ConnectAsync(string nodeId, CancellationToken cancellationToken = default);
        Task<bool> DisconnectAsync(string nodeId, CancellationToken cancellationToken = default);
        Task<bool> IsConnectedAsync(string nodeId, CancellationToken cancellationToken = default);
        Task<int> GetConnectionCountAsync(CancellationToken cancellationToken = default);
        Task<P2PNetworkStatus> GetNetworkStatusAsync(CancellationToken cancellationToken = default);
    }

    public interface IP2PFileSharingService
    {
        Task ShareFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task<string> DownloadFileAsync(string fileId, string destinationPath, CancellationToken cancellationToken = default);
        Task<IEnumerable<P2PFile>> GetSharedFilesAsync(CancellationToken cancellationToken = default);
        Task<bool> UnshareFileAsync(string fileId, CancellationToken cancellationToken = default);
        Task<P2PFileTransferStatus> GetTransferStatusAsync(string transferId, CancellationToken cancellationToken = default);
    }

    public interface IP2PMessagingService
    {
        Task<string> SendMessageAsync(string nodeId, string message, CancellationToken cancellationToken = default);
        Task<string> SendMessageAsync(string nodeId, byte[] message, CancellationToken cancellationToken = default);
        Task<IEnumerable<P2PMessage>> GetMessagesAsync(CancellationToken cancellationToken = default);
        Task<P2PMessage> GetMessageAsync(string messageId, CancellationToken cancellationToken = default);
        Task<bool> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default);
        Task SubscribeToMessagesAsync(Func<P2PMessage, Task> messageHandler, CancellationToken cancellationToken = default);
    }

    public interface IP2PNetworkService
    {
        Task<P2PNetworkTopology> GetNetworkTopologyAsync(CancellationToken cancellationToken = default);
        Task OptimizeNetworkTopologyAsync(CancellationToken cancellationToken = default);
        Task<bool> AddNodeAsync(P2PNode node, CancellationToken cancellationToken = default);
        Task<bool> RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<P2PNode>> GetNeighborsAsync(string nodeId, CancellationToken cancellationToken = default);
        Task<int> GetNetworkSizeAsync(CancellationToken cancellationToken = default);
        Task<P2PNode> GetLocalNodeAsync(CancellationToken cancellationToken = default);
    }

    public interface IP2PSecurityService
    {
        Task<byte[]> EncryptAsync(byte[] data, string nodeId, CancellationToken cancellationToken = default);
        Task<byte[]> DecryptAsync(byte[] encryptedData, string nodeId, CancellationToken cancellationToken = default);
        Task<bool> AuthenticateNodeAsync(P2PNode node, CancellationToken cancellationToken = default);
        Task<string> GenerateKeyPairAsync(CancellationToken cancellationToken = default);
        Task<bool> VerifySignatureAsync(byte[] data, byte[] signature, string nodeId, CancellationToken cancellationToken = default);
        Task<byte[]> SignDataAsync(byte[] data, CancellationToken cancellationToken = default);
    }

    public class P2POptions
    {
        public bool Enabled { get; set; } = true;
        public int MaxConnections { get; set; } = 100;
        public int Port { get; set; } = 8080;
        public int BufferSize { get; set; } = 8192;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);
        public bool EnableEncryption { get; set; } = true;
        public bool EnableDiscovery { get; set; } = true;
        public TimeSpan DiscoveryInterval { get; set; } = TimeSpan.FromSeconds(30);
        public bool EnableParallelProcessing { get; set; } = true;
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
        public bool EnableBatching { get; set; } = true;
        public int BatchSize { get; set; } = 100;
        public bool EnableCaching { get; set; } = true;
        public int CacheSize { get; set; } = 1000;
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    }

    public class P2PNode
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Node";
        public IPAddress Address { get; set; } = IPAddress.Loopback;
        public int Port { get; set; } = 8080;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public bool IsOnline { get; set; } = true;
        public int ConnectionCount { get; set; } = 0;
        public string Status { get; set; } = "Online";
    }

    public class P2PFile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; } = 0;
        public string Hash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string OwnerId { get; set; } = string.Empty;
        public bool IsShared { get; set; } = false;
    }

    public class P2PMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public byte[]? BinaryContent { get; set; } = null;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReceivedAt { get; set; } = null;
        public bool IsRead { get; set; } = false;
        public string Status { get; set; } = "Pending";
    }

    public class P2PNetworkStatus
    {
        public string Status { get; set; } = "Offline";
        public int ConnectionCount { get; set; } = 0;
        public int NodeCount { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public double NetworkHealth { get; set; } = 0;
        public int FailedConnections { get; set; } = 0;
    }

    public class P2PFileTransferStatus
    {
        public string TransferId { get; set; } = Guid.NewGuid().ToString();
        public string FileId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public long TotalSize { get; set; } = 0;
        public long TransferredSize { get; set; } = 0;
        public double Progress { get; set; } = 0;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; } = null;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class P2PNetworkTopology
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<P2PNode> Nodes { get; set; } = new List<P2PNode>();
        public List<P2PConnection> Connections { get; set; } = new List<P2PConnection>();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public int NodeCount => Nodes.Count;
        public int ConnectionCount => Connections.Count;
    }

    public class P2PConnection
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Node1Id { get; set; } = string.Empty;
        public string Node2Id { get; set; } = string.Empty;
        public DateTime EstablishedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public int Ping { get; set; } = 0;
    }

    public class P2PService : IP2PService
    {
        private readonly ILogger<P2PService> _logger;
        private readonly P2POptions _options;
        private readonly TcpListener _tcpListener;
        private readonly Dictionary<string, TcpClient> _connections = new Dictionary<string, TcpClient>();
        private readonly Dictionary<string, P2PNode> _nodes = new Dictionary<string, P2PNode>();
        private readonly Channel<P2PMessage> _messageChannel = Channel.CreateUnbounded<P2PMessage>();
        private bool _isInitialized = false;
        private bool _isRunning = false;
        private Task? _listenerTask = null;
        private CancellationTokenSource? _cancellationTokenSource = null;

        public P2PService(ILogger<P2PService> logger, IOptions<P2POptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _tcpListener = new TcpListener(IPAddress.Any, _options.Port);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_isInitialized)
            {
                _logger.LogInformation("P2P service is already initialized");
                return;
            }

            try
            {
                _logger.LogInformation("Initializing P2P service");
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _tcpListener.Start();
                _isRunning = true;
                _listenerTask = Task.Run(() => ListenForConnectionsAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
                _isInitialized = true;
                _logger.LogInformation("P2P service initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize P2P service");
                throw;
            }
        }

        public async Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            if (!_isInitialized)
            {
                _logger.LogInformation("P2P service is not initialized");
                return;
            }

            try
            {
                _logger.LogInformation("Shutting down P2P service");
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
                
                // Close all connections
                foreach (var connection in _connections.Values)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error closing connection");
                    }
                }
                _connections.Clear();
                _nodes.Clear();

                _tcpListener.Stop();
                if (_listenerTask != null)
                {
                    await _listenerTask.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
                }
                
                _isInitialized = false;
                _logger.LogInformation("P2P service shutdown successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to shutdown P2P service");
                throw;
            }
        }

        public async Task<IEnumerable<P2PNode>> DiscoverNodesAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            _logger.LogInformation("Discovering nodes");
            
            // Simulate node discovery
            await Task.Delay(100, cancellationToken);
            _logger.LogInformation($"Discovered {_nodes.Count} nodes");
            return _nodes.Values;
        }

        public async Task<P2PNode> GetNodeAsync(string nodeId, CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            if (_nodes.TryGetValue(nodeId, out var node))
            {
                return node;
            }
            throw new KeyNotFoundException($"Node with id {nodeId} not found");
        }

        public async Task<bool> ConnectAsync(string nodeId, CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            if (!_nodes.TryGetValue(nodeId, out var node))
            {
                _logger.LogError($"Node with id {nodeId} not found");
                return false;
            }

            try
            {
                _logger.LogInformation($"Connecting to node {nodeId} at {node.Address}:{node.Port}");
                var client = new TcpClient();
                await client.ConnectAsync(node.Address, node.Port, cancellationToken);
                _connections[nodeId] = client;
                node.IsOnline = true;
                node.ConnectionCount++;
                node.LastSeen = DateTime.UtcNow;
                _logger.LogInformation($"Connected to node {nodeId} successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to connect to node {nodeId}");
                return false;
            }
        }

        public async Task<bool> DisconnectAsync(string nodeId, CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            if (_connections.TryGetValue(nodeId, out var client))
            {
                try
                {
                    _logger.LogInformation($"Disconnecting from node {nodeId}");
                    client.Close();
                    _connections.Remove(nodeId);
                    if (_nodes.TryGetValue(nodeId, out var node))
                    {
                        node.ConnectionCount--;
                        if (node.ConnectionCount == 0)
                        {
                            node.IsOnline = false;
                            node.Status = "Offline";
                        }
                    }
                    _logger.LogInformation($"Disconnected from node {nodeId} successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to disconnect from node {nodeId}");
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> IsConnectedAsync(string nodeId, CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            return _connections.ContainsKey(nodeId);
        }

        public async Task<int> GetConnectionCountAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            return _connections.Count;
        }

        public async Task<P2PNetworkStatus> GetNetworkStatusAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            var onlineNodes = _nodes.Values.Count(n => n.IsOnline);
            var totalNodes = _nodes.Count;
            var networkHealth = totalNodes > 0 ? (double)onlineNodes / totalNodes * 100 : 0;

            return new P2PNetworkStatus
            {
                Status = _isRunning ? "Online" : "Offline",
                ConnectionCount = _connections.Count,
                NodeCount = totalNodes,
                LastUpdated = DateTime.UtcNow,
                NetworkHealth = networkHealth,
                FailedConnections = totalNodes - onlineNodes
            };
        }

        private async Task ListenForConnectionsAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Listening for connections on port {_options.Port}");
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    if (_tcpListener.Pending())
                    {
                        var client = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
                        _ = Task.Run(() => HandleConnectionAsync(client, cancellationToken), cancellationToken);
                    }
                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Connection listener canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in connection listener");
            }
        }

        private async Task HandleConnectionAsync(TcpClient client, CancellationToken cancellationToken)
        {
            var remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            if (remoteEndPoint == null)
            {
                _logger.LogError("Invalid remote endpoint");
                client.Close();
                return;
            }

            _logger.LogInformation($"Received connection from {remoteEndPoint.Address}:{remoteEndPoint.Port}");

            try
            {
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[_options.BufferSize];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead > 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        _logger.LogInformation($"Received message: {message}");
                        // Process message
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling connection");
            }
            finally
            {
                client.Close();
            }
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("P2P service is not initialized");
            }
        }
    }

    public class P2PFileSharingService : IP2PFileSharingService
    {
        private readonly ILogger<P2PFileSharingService> _logger;
        private readonly P2POptions _options;
        private readonly List<P2PFile> _sharedFiles = new List<P2PFile>();
        private readonly Dictionary<string, P2PFileTransferStatus> _transferStatuses = new Dictionary<string, P2PFileTransferStatus>();

        public P2PFileSharingService(ILogger<P2PFileSharingService> logger, IOptions<P2POptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task ShareFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Sharing file: {filePath}");
            try
            {
                var fileInfo = new System.IO.FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    throw new System.IO.FileNotFoundException($"File not found: {filePath}");
                }

                var file = new P2PFile
                {
                    Name = fileInfo.Name,
                    Path = filePath,
                    Size = fileInfo.Length,
                    Hash = CalculateHash(filePath),
                    OwnerId = "local",
                    IsShared = true
                };

                _sharedFiles.Add(file);
                _logger.LogInformation($"File shared successfully: {file.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to share file: {filePath}");
                throw;
            }
        }

        public async Task<string> DownloadFileAsync(string fileId, string destinationPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Downloading file: {fileId} to {destinationPath}");
            try
            {
                var file = _sharedFiles.FirstOrDefault(f => f.Id == fileId);
                if (file == null)
                {
                    throw new KeyNotFoundException($"File not found: {fileId}");
                }

                var transferId = Guid.NewGuid().ToString();
                var transferStatus = new P2PFileTransferStatus
                {
                    TransferId = transferId,
                    FileId = fileId,
                    Status = "InProgress",
                    TotalSize = file.Size,
                    TransferredSize = 0,
                    Progress = 0
                };

                _transferStatuses[transferId] = transferStatus;

                // Simulate download progress
                for (int i = 0; i <= 100; i += 10)
                {
                    transferStatus.TransferredSize = (long)(file.Size * i / 100.0);
                    transferStatus.Progress = i;
                    await Task.Delay(100, cancellationToken);
                }

                transferStatus.Status = "Completed";
                transferStatus.CompletedAt = DateTime.UtcNow;
                transferStatus.Progress = 100;
                transferStatus.TransferredSize = file.Size;

                _logger.LogInformation($"File downloaded successfully: {fileId}");
                return transferId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to download file: {fileId}");
                throw;
            }
        }

        public async Task<IEnumerable<P2PFile>> GetSharedFilesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting shared files");
            return _sharedFiles;
        }

        public async Task<bool> UnshareFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Unsharing file: {fileId}");
            var file = _sharedFiles.FirstOrDefault(f => f.Id == fileId);
            if (file != null)
            {
                file.IsShared = false;
                _logger.LogInformation($"File unshared successfully: {fileId}");
                return true;
            }
            _logger.LogError($"File not found: {fileId}");
            return false;
        }

        public async Task<P2PFileTransferStatus> GetTransferStatusAsync(string transferId, CancellationToken cancellationToken = default)
        {
            if (_transferStatuses.TryGetValue(transferId, out var status))
            {
                return status;
            }
            throw new KeyNotFoundException($"Transfer with id {transferId} not found");
        }

        private string CalculateHash(string filePath)
        {
            // Simplified hash calculation for demo purposes
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
        }
    }

    public class P2PMessagingService : IP2PMessagingService
    {
        private readonly ILogger<P2PMessagingService> _logger;
        private readonly P2POptions _options;
        private readonly List<P2PMessage> _messages = new List<P2PMessage>();
        private readonly List<Func<P2PMessage, Task>> _messageHandlers = new List<Func<P2PMessage, Task>>();

        public P2PMessagingService(ILogger<P2PMessagingService> logger, IOptions<P2POptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<string> SendMessageAsync(string nodeId, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Sending message to node {nodeId}");
            var msg = new P2PMessage
            {
                SenderId = "local",
                ReceiverId = nodeId,
                Content = message,
                Status = "Sent"
            };

            _messages.Add(msg);
            await NotifyMessageHandlersAsync(msg, cancellationToken);
            _logger.LogInformation($"Message sent successfully: {msg.Id}");
            return msg.Id;
        }

        public async Task<string> SendMessageAsync(string nodeId, byte[] message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Sending binary message to node {nodeId}");
            var msg = new P2PMessage
            {
                SenderId = "local",
                ReceiverId = nodeId,
                BinaryContent = message,
                Status = "Sent"
            };

            _messages.Add(msg);
            await NotifyMessageHandlersAsync(msg, cancellationToken);
            _logger.LogInformation($"Binary message sent successfully: {msg.Id}");
            return msg.Id;
        }

        public async Task<IEnumerable<P2PMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting messages");
            return _messages;
        }

        public async Task<P2PMessage> GetMessageAsync(string messageId, CancellationToken cancellationToken = default)
        {
            var message = _messages.FirstOrDefault(m => m.Id == messageId);
            if (message != null)
            {
                message.IsRead = true;
                return message;
            }
            throw new KeyNotFoundException($"Message with id {messageId} not found");
        }

        public async Task<bool> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Deleting message: {messageId}");
            var message = _messages.FirstOrDefault(m => m.Id == messageId);
            if (message != null)
            {
                _messages.Remove(message);
                _logger.LogInformation($"Message deleted successfully: {messageId}");
                return true;
            }
            _logger.LogError($"Message not found: {messageId}");
            return false;
        }

        public async Task SubscribeToMessagesAsync(Func<P2PMessage, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Subscribing to messages");
            _messageHandlers.Add(messageHandler);
        }

        private async Task NotifyMessageHandlersAsync(P2PMessage message, CancellationToken cancellationToken = default)
        {
            foreach (var handler in _messageHandlers)
            {
                try
                {
                    await handler(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in message handler");
                }
            }
        }
    }

    public class P2PNetworkService : IP2PNetworkService
    {
        private readonly ILogger<P2PNetworkService> _logger;
        private readonly P2POptions _options;
        private readonly P2PNetworkTopology _networkTopology = new P2PNetworkTopology();

        public P2PNetworkService(ILogger<P2PNetworkService> logger, IOptions<P2POptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<P2PNetworkTopology> GetNetworkTopologyAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting network topology");
            return _networkTopology;
        }

        public async Task OptimizeNetworkTopologyAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Optimizing network topology");
            // Simplified optimization for demo purposes
            await Task.Delay(100, cancellationToken);
            _logger.LogInformation("Network topology optimized successfully");
        }

        public async Task<bool> AddNodeAsync(P2PNode node, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Adding node: {node.Id}");
            if (!_networkTopology.Nodes.Any(n => n.Id == node.Id))
            {
                _networkTopology.Nodes.Add(node);
                _networkTopology.LastUpdated = DateTime.UtcNow;
                _logger.LogInformation($"Node added successfully: {node.Id}");
                return true;
            }
            _logger.LogError($"Node already exists: {node.Id}");
            return false;
        }

        public async Task<bool> RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Removing node: {nodeId}");
            var node = _networkTopology.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node != null)
            {
                _networkTopology.Nodes.Remove(node);
                _networkTopology.Connections.RemoveAll(c => c.Node1Id == nodeId || c.Node2Id == nodeId);
                _networkTopology.LastUpdated = DateTime.UtcNow;
                _logger.LogInformation($"Node removed successfully: {nodeId}");
                return true;
            }
            _logger.LogError($"Node not found: {nodeId}");
            return false;
        }

        public async Task<IEnumerable<P2PNode>> GetNeighborsAsync(string nodeId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Getting neighbors for node: {nodeId}");
            var neighborIds = _networkTopology.Connections
                .Where(c => c.Node1Id == nodeId || c.Node2Id == nodeId)
                .Select(c => c.Node1Id == nodeId ? c.Node2Id : c.Node1Id)
                .Distinct();

            var neighbors = _networkTopology.Nodes.Where(n => neighborIds.Contains(n.Id));
            return neighbors;
        }

        public async Task<int> GetNetworkSizeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting network size");
            return _networkTopology.NodeCount;
        }

        public async Task<P2PNode> GetLocalNodeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting local node");
            return new P2PNode
            {
                Id = "local",
                Name = "Local Node",
                Address = IPAddress.Loopback,
                Port = _options.Port
            };
        }
    }

    public class P2PSecurityService : IP2PSecurityService
    {
        private readonly ILogger<P2PSecurityService> _logger;
        private readonly P2POptions _options;

        public P2PSecurityService(ILogger<P2PSecurityService> logger, IOptions<P2POptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<byte[]> EncryptAsync(byte[] data, string nodeId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Encrypting data for node: {nodeId}");
            // Simplified encryption for demo purposes
            await Task.Delay(10, cancellationToken);
            return data;
        }

        public async Task<byte[]> DecryptAsync(byte[] encryptedData, string nodeId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Decrypting data from node: {nodeId}");
            // Simplified decryption for demo purposes
            await Task.Delay(10, cancellationToken);
            return encryptedData;
        }

        public async Task<bool> AuthenticateNodeAsync(P2PNode node, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Authenticating node: {node.Id}");
            // Simplified authentication for demo purposes
            await Task.Delay(10, cancellationToken);
            return true;
        }

        public async Task<string> GenerateKeyPairAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating key pair");
            // Simplified key pair generation for demo purposes
            await Task.Delay(10, cancellationToken);
            return Guid.NewGuid().ToString();
        }

        public async Task<bool> VerifySignatureAsync(byte[] data, byte[] signature, string nodeId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Verifying signature from node: {nodeId}");
            // Simplified signature verification for demo purposes
            await Task.Delay(10, cancellationToken);
            return true;
        }

        public async Task<byte[]> SignDataAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Signing data");
            // Simplified data signing for demo purposes
            await Task.Delay(10, cancellationToken);
            return data;
        }
    }

    public static class P2PServiceCollectionExtensions
    {
        public static IServiceCollection AddP2PServices(this IServiceCollection services, Action<P2POptions>? configureOptions = null)
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<P2POptions>(options => { });
            }

            services.AddSingleton<IP2PService, P2PService>();
            services.AddSingleton<IP2PFileSharingService, P2PFileSharingService>();
            services.AddSingleton<IP2PMessagingService, P2PMessagingService>();
            services.AddSingleton<IP2PNetworkService, P2PNetworkService>();
            services.AddSingleton<IP2PSecurityService, P2PSecurityService>();

            return services;
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("P2P Integration Example");
            Console.WriteLine("=" * 50);

            // Build service container
            var services = new ServiceCollection();

            // Configure logging
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

            // Register P2P services
            services.AddP2PServices(options =>
            {
                options.Enabled = true;
                options.MaxConnections = 100;
                options.Port = 8080;
                options.EnableEncryption = true;
                options.EnableDiscovery = true;
                options.DiscoveryInterval = TimeSpan.FromSeconds(30);
                options.EnableParallelProcessing = true;
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            });

            // Build service provider
            using var serviceProvider = services.BuildServiceProvider();

            // Get services
            var p2pService = serviceProvider.GetRequiredService<IP2PService>();
            var fileSharingService = serviceProvider.GetRequiredService<IP2PFileSharingService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // Initialize P2P service
                Console.WriteLine("Initializing P2P service...");
                await p2pService.InitializeAsync();
                Console.WriteLine("P2P service initialized successfully");

                // Discover nodes
                Console.WriteLine("\nDiscovering nodes...");
                var nodes = await p2pService.DiscoverNodesAsync();
                Console.WriteLine($"Discovered {nodes.Count()} nodes");

                // Share a file
                Console.WriteLine("\nSharing a file...");
                var testFile = System.IO.Path.Combine(Environment.CurrentDirectory, "test.txt");
                System.IO.File.WriteAllText(testFile, "This is a test file for P2P sharing");
                await fileSharingService.ShareFileAsync(testFile);
                Console.WriteLine("File shared successfully");

                // Get shared files
                Console.WriteLine("\nGetting shared files...");
                var sharedFiles = await fileSharingService.GetSharedFilesAsync();
                foreach (var file in sharedFiles)
                {
                    Console.WriteLine($"File: {file.Name}, Size: {file.Size} bytes, Hash: {file.Hash}");
                }

                // Get network status
                Console.WriteLine("\nGetting network status...");
                var status = await p2pService.GetNetworkStatusAsync();
                Console.WriteLine($"Status: {status.Status}");
                Console.WriteLine($"Connection Count: {status.ConnectionCount}");
                Console.WriteLine($"Node Count: {status.NodeCount}");
                Console.WriteLine($"Network Health: {status.NetworkHealth:F2}%");

                Console.WriteLine("\nP2P integration example completed successfully!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in P2P integration example");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            finally
            {
                // Shutdown P2P service
                Console.WriteLine("\nShutting down P2P service...");
                await p2pService.ShutdownAsync();
                Console.WriteLine("P2P service shutdown successfully");
            }
        }
    }
}
