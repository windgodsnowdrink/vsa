public async Task RegisterDeviceAsync(string deviceId)
{
    var connection = _connectionPool.Get();
    try
    {
        // 使用Span<T>零拷贝技术构建注册报文
        Span<byte> registerPacket = stackalloc byte[128];
        Encoding.UTF8.GetBytes(deviceId, registerPacket);
        
        // 发送注册请求
        await connection.SendAsync(registerPacket);
        
        // 接收响应并验证
        var response = await connection.ReceiveAsync();
        if (!VIIDProtocol.ValidateRegisterResponse(response.Span))
        {
            throw new InvalidOperationException("设备注册失败");
        }
    }
    finally
    {
        _connectionPool.Return(connection);
    }
}

public async Task SendHeartbeatAsync(string deviceId)
{
    // 使用对象池获取连接
    using var lease = _connectionPool.Get();
    var connection = lease.Item;
    
    // 构建心跳报文
    var heartbeatPacket = VIIDProtocol.BuildHeartbeatPacket(deviceId);
    
    // 发送心跳
    await connection.SendAsync(heartbeatPacket);
    
    // 接收响应
    var response = await connection.ReceiveAsync();
    if (!VIIDProtocol.ValidateHeartbeatResponse(response.Span))
    {
        throw new InvalidOperationException("心跳响应无效");
    }
}

public async Task SubscribeNotificationAsync(string deviceId, string notificationUrl)
{
    // 使用Channel实现生产者-消费者模式
    var channel = Channel.CreateBounded<NotificationMessage>(1000);
    
    // 启动后台处理任务
    _ = Task.Run(async () =>
    {
        await foreach (var message in channel.Reader.ReadAllAsync())
        {
            // 使用HttpClientFactory发送通知
            using var client = _httpClientFactory.CreateClient();
            await client.PostAsJsonAsync(notificationUrl, message);
        }
    });
    
    // 注册订阅
    var connection = _connectionPool.Get();
    try
    {
        var subscribePacket = VIIDProtocol.BuildSubscribePacket(deviceId, channel);
        await connection.SendAsync(subscribePacket);
        
        var response = await connection.ReceiveAsync();
        if (!VIIDProtocol.ValidateSubscribeResponse(response.Span))
        {
            throw new InvalidOperationException("订阅失败");
        }
    }
    finally
    {
        _connectionPool.Return(connection);
    }
}

public async Task<Stream> GetVideoStreamAsync(string deviceId, DateTime startTime, DateTime endTime)
{
    // 使用MemoryPool优化内存分配
    var memoryPool = MemoryPool<byte>.Shared;
    var owner = memoryPool.Rent(81920); // 80KB buffer
    
    try
    {
        // 构建视频流请求
        var request = VIIDProtocol.BuildVideoRequest(deviceId, startTime, endTime);
        
        // 获取连接并发送请求
        var connection = _connectionPool.Get();
        try
        {
            await connection.SendAsync(request);
            
            // 创建管道流处理视频数据
            var pipe = new Pipe(new PipeOptions(memoryPool));
            
            // 启动后台写入任务
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var memory = pipe.Writer.GetMemory(owner.Memory.Length);
                    var received = await connection.ReceiveAsync(memory);
                    
                    if (received == 0) break;
                    
                    pipe.Writer.Advance(received);
                    await pipe.Writer.FlushAsync();
                }
                
                pipe.Writer.Complete();
            });
            
            return pipe.Reader.AsStream();
        }
        finally
        {
            _connectionPool.Return(connection);
        }
    }
    catch
    {
        owner.Dispose();
        throw;
    }
}