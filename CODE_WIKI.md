# VSA 项目 Code Wiki

> **项目名称**: VSA (Vertical Slice Architecture)  
> **技术栈**: .NET 10, C# 14, ASP.NET Core, Blazor, MAUI, IoT  
> **SDK 版本**: .NET 10.0.201  
> **语言版本**: C# preview (latest)  
> **许可证**: 见 LICENSE 文件  

---

## 目录

1. [项目概述](#1-项目概述)
2. [项目目录结构](#2-项目目录结构)
3. [核心架构设计](#3-核心架构设计)
4. [主要模块说明](#4-主要模块说明)
   - [4.1 根目录核心文件](#41-根目录核心文件)
   - [4.2 code/ - 高级集成示例](#42-code---高级集成示例)
   - [4.3 mvp/ - 最小可行产品示例](#43-mvp---最小可行产品示例)
   - [4.4 plc/ - 工业物联网 (IoT/PLC)](#44-plc---工业物联网-iotplc)
   - [4.5 csp/ - CSP并发模型](#45-csp---csp并发模型)
   - [4.6 ai/ - AI/ML 集成](#46-ai---aiml-集成)
   - [4.7 SimpleCsvImporter/ - CSV导入工具](#47-simplecsvimporter---csv导入工具)
   - [4.8 Models/ - 数据模型](#48-models---数据模型)
5. [依赖关系全景](#5-依赖关系全景)
6. [关键类与接口说明](#6-关键类与接口说明)
7. [配置与运行方式](#7-配置与运行方式)
8. [技术领域分类索引](#8-技术领域分类索引)

---

## 1. 项目概述

VSA 是一个基于 **.NET 10** 的综合性技术展示仓库，采用 **垂直切片架构 (Vertical Slice Architecture)** ，涵盖从基础框架到高级分布式系统的全栈技术演示。项目包含 **400+ 个独立集成示例文件**，每个文件演示一种或多种.NET技术/第三方库的集成方案。

### 核心特性

| 特性 | 说明 |
|------|------|
| 目标框架 | `net10.0` |
| 语言版本 | C# preview |
| 可为空引用类型 | 全局启用 (`Nullable=enable`) |
| 隐式 Using | 全局启用 (`ImplicitUsings=enable`) |
| 中央包管理 | 启用 (`ManagePackageVersionsCentrally=true`) |
| AOT 编译 | 支持 (`PublishAot`) |
| 代码分析 | Roslynator + SonarAnalyzer + StyleCop |
| 文档生成 | 自动生成 XML 文档 (`GenerateDocumentationFile=true`) |

### 技术覆盖范围

- **Web 框架**: ASP.NET Core Minimal API, MVC, Blazor, SignalR, HTMX
- **微服务**: Dapr, MassTransit, Wolverine, Tye, Steeltoe
- **消息队列**: RabbitMQ, Kafka, MQTT, ZeroMQ, Redis Streams, RocketMQ
- **数据库**: EF Core, Dapper, LiteDB, MongoDB, MySQL, PostgreSQL, SQLite, ClickHouse, TDengine, InfluxDB
- **缓存**: Redis (StackExchange), EasyCaching, FusionCache, HybridCache, Garnet
- **RPC**: gRPC, MagicOnion, JSON-RPC, StreamJsonRpc, Refit, RestSharp
- **网络协议**: HTTP/3 QUIC, WebSocket, SignalR, SIP, STUN, KCP, DNS
- **工业物联网**: Modbus, Siemens, Omron, Melsec, BACnet, OPC-UA/DA, LoRa
- **AI/ML**: Semantic Kernel, TensorFlow, Ollama, ML.NET, PaddleOCR
- **可观测性**: OpenTelemetry, Prometheus, Grafana, Application Insights, Seq, Jaeger, Zipkin
- **安全**: ABAC, JWT, OAuth2, OpenIddict, Keycloak, ACME, 加密算法
- **架构模式**: Vertical Slice, CQRS, Event Sourcing, DDD, Saga, State Machine

---

## 2. 项目目录结构

```
/workspace/
├── .trae/                          # TRAE IDE 配置规则
│   └── rules/                      # 项目规则文件
├── Models/                         # 数据模型 (Haar Cascade 分类器)
├── SimpleCsvImporter/              # CSV 导入工具
├── ai/                             # AI/ML 集成示例
├── code/                           # 高级集成示例 (49个文件)
├── csp/                            # CSP 并发模型 (Go 风格)
├── mvp/                            # MVP 最小可行产品示例
├── plc/                            # 工业物联网 IoT/PLC
├── *.cs                            # 根目录 400+ 集成示例文件
├── Directory.Build.props           # MSBuild 全局属性
├── Directory.Build.targets         # MSBuild 全局目标
├── Directory.Packages.props        # 中央包版本管理
├── NuGet.Config                    # NuGet 包源配置
├── global.json                     # .NET SDK 版本配置
├── dotnet-tools.json               # 本地工具清单
├── GlobalAnalyzerConfig.globalconfig # 全局分析器配置
├── appsettings.json                # 应用配置
├── stylecop.json                   # StyleCop 规则配置
├── dotnet.ruleset                  # 代码分析规则集
├── .editorconfig                   # 编辑器配置
├── .prettierrc                     # Prettier 格式化配置
├── build.cake                      # Cake 构建脚本
├── program.cs                      # 主程序入口
├── Todo.cs                         # Todo 示例入口
├── README.md                       # 项目说明
└── LICENSE                         # 许可证
```

---

## 3. 核心架构设计

### 3.1 垂直切片架构 (VSA)

项目遵循 **VSA (Vertical Slice Architecture)** 设计理念，按功能领域垂直切分而非传统水平分层：

```
┌─────────────────────────────────────────────────┐
│                   API Layer (Minimal API)         │
├─────────────────────────────────────────────────┤
│  Feature Slice 1  │ Feature Slice 2 │ Feature N  │
│  ┌─────────────┐  │ ┌─────────────┐ │            │
│  │ Request     │  │ │ Request     │ │            │
│  │ Handler     │  │ │ Handler     │ │            │
│  │ Validator   │  │ │ Validator   │ │            │
│  │ Repository  │  │ │ Repository  │ │            │
│  │ Response    │  │ │ Response    │ │            │
│  └─────────────┘  │ └─────────────┘ │            │
├─────────────────────────────────────────────────┤
│              Shared Kernel / Cross-Cutting       │
│   (Logging, Caching, Auth, Validation, etc.)    │
└─────────────────────────────────────────────────┘
```

### 3.2 技术架构层次

```
┌──────────────────────────────────────────────┐
│              表示层 (Presentation)             │
│  Blazor WebAssembly | MAUI | HTMX | Razor    │
├──────────────────────────────────────────────┤
│              API 网关层 (Gateway)              │
│  YARP Reverse Proxy | Ocelot | Service Mesh  │
├──────────────────────────────────────────────┤
│              应用层 (Application)              │
│  CQRS | MediatR | Wolverine | MassTransit    │
├──────────────────────────────────────────────┤
│              领域层 (Domain)                   │
│  DDD | Event Sourcing | Saga | State Machine │
├──────────────────────────────────────────────┤
│           基础设施层 (Infrastructure)          │
│  EF Core | Dapper | Redis | Kafka | gRPC     │
└──────────────────────────────────────────────┘
```

---

## 4. 主要模块说明

### 4.1 根目录核心文件

根目录包含 **400+ 个独立集成示例文件**，每个文件演示一种技术或模式。按功能领域分类如下：

#### 4.1.1 架构与设计模式

| 文件 | 说明 |
|------|------|
| [vertical_slice_architecture.cs](file:///workspace/vertical_slice_architecture.cs) | 垂直切片架构实现，按功能特性组织代码 |
| [cqrs_impl.cs](file:///workspace/cqrs_impl.cs) | CQRS 命令查询职责分离实现 |
| [cqrs_processor.cs](file:///workspace/cqrs_processor.cs) | CQRS 处理器模式 |
| [eventsourcing_service.cs](file:///workspace/eventsourcing_service.cs) | 事件溯源服务实现 |
| [event_sourcing_processor.cs](file:///workspace/event_sourcing_processor.cs) | 事件溯源处理器 |
| [saga_orchestrator.cs](file:///workspace/saga_orchestrator.cs) | Saga 分布式事务编排器 |
| [state_machine_flow.cs](file:///workspace/state_machine_flow.cs) | 状态机工作流 |
| [specification_pattern.cs](file:///workspace/specification_pattern.cs) | 规约模式实现 |
| [specification_repository.cs](file:///workspace/specification_repository.cs) | 规约仓储模式 |
| [unitofwork_integration.cs](file:///workspace/unitofwork_integration.cs) | 工作单元模式 |

#### 4.1.2 消息与通信

| 文件 | 说明 |
|------|------|
| [masstransit_production_integration.cs](file:///workspace/masstransit_production_integration.cs) | MassTransit 生产级消息总线集成 |
| [masstransit_integration.cs](file:///workspace/masstransit_integration.cs) | MassTransit 基础集成 |
| [rabbitmq_production_integration.cs](file:///workspace/rabbitmq_production_integration.cs) | RabbitMQ 生产级集成 |
| [librdkafka_integration.cs](file:///workspace/librdkafka_integration.cs) | Kafka (librdkafka) 集成 |
| [redis_stream_integration.cs](file:///workspace/redis_stream_integration.cs) | Redis Streams 消息队列 |
| [redis_eventbus_integration.cs](file:///workspace/redis_eventbus_integration.cs) | Redis 事件总线 |
| [distributed_bus.cs](file:///workspace/distributed_bus.cs) | 分布式消息总线 |
| [message_queue.cs](file:///workspace/message_queue.cs) | 通用消息队列抽象 |
| [message_queue_enhanced.cs](file:///workspace/message_queue_enhanced.cs) | 增强消息队列 |
| [dead_letter.cs](file:///workspace/dead_letter.cs) | 死信队列处理 |

#### 4.1.3 高性能处理

| 文件 | 说明 |
|------|------|
| [disruptor_production_integration.cs](file:///workspace/disruptor_production_integration.cs) | LMAX Disruptor 生产级集成 |
| [disruptor_integration.cs](file:///workspace/disruptor_integration.cs) | Disruptor 基础集成 |
| [disruptor_processor.cs](file:///workspace/disruptor_processor.cs) | Disruptor 处理器 |
| [disruptor_json_processor.cs](file:///workspace/disruptor_json_processor.cs) | Disruptor JSON 处理 |
| [high_frequency_1m.cs](file:///workspace/high_frequency_1m.cs) | 每秒百万级高频处理 |
| [high_frequency_10m.cs](file:///workspace/high_frequency_10m.cs) | 每秒千万级高频处理 |
| [high_frequency_100k.cs](file:///workspace/high_frequency_100k.cs) | 每秒十万级高频处理 |
| [tail_latency_optimizer.cs](file:///workspace/tail_latency_optimizer.cs) | 尾延迟优化器 |
| [llvm_ir_optimizer.cs](file:///workspace/llvm_ir_optimizer.cs) | LLVM IR 级状态机编译优化 |

#### 4.1.4 序列化与零拷贝

| 文件 | 说明 |
|------|------|
| [span_zero_copy_serializer.cs](file:///workspace/span_zero_copy_serializer.cs) | Span 零拷贝序列化器 |
| [zero_copy_serializer.cs](file:///workspace/zero_copy_serializer.cs) | 零拷贝序列化器 |
| [zero_copy_messagepack.cs](file:///workspace/zero_copy_messagepack.cs) | 零拷贝 MessagePack |
| [shared_memory_serializer.cs](file:///workspace/shared_memory_serializer.cs) | 共享内存序列化 |
| [shared_memory_messagepack.cs](file:///workspace/shared_memory_messagepack.cs) | 共享内存 MessagePack |
| [hybrid_serializer.cs](file:///workspace/hybrid_serializer.cs) | 混合序列化器 |
| [hybrid_protocol_advanced.cs](file:///workspace/hybrid_protocol_advanced.cs) | 高级混合协议 |
| [serialization_impl.cs](file:///workspace/serialization_impl.cs) | 序列化实现 |
| [memorypack_demo.cs](file:///workspace/memorypack_demo.cs) | MemoryPack 演示 |
| [memorypack_integration.cs](file:///workspace/memorypack_integration.cs) | MemoryPack 集成 |

#### 4.1.5 内存管理

| 文件 | 说明 |
|------|------|
| [tiered_memory_integration.cs](file:///workspace/tiered_memory_integration.cs) | 分层内存服务集成 |
| [tiered_memory_processor.cs](file:///workspace/tiered_memory_processor.cs) | 分层内存处理器 |
| [threadsafe_memorypool_optimized.cs](file:///workspace/threadsafe_memorypool_optimized.cs) | 线程安全内存池优化 |
| [threadsafe_memorypool_serializer.cs](file:///workspace/threadsafe_memorypool_serializer.cs) | 线程安全内存池序列化 |
| [memory_leak_detection.cs](file:///workspace/memory_leak_detection.cs) | 内存泄漏检测 |
| [recyclable_memorystream.cs](file:///workspace/recyclable_memorystream.cs) | 可回收内存流 |
| [recyclable_memorystream_integration.cs](file:///workspace/recyclable_memorystream_integration.cs) | 可回收内存流集成 |

#### 4.1.6 数据库与存储

| 文件 | 说明 |
|------|------|
| [efcore9_integration.cs](file:///workspace/efcore9_integration.cs) | EF Core 9 集成 |
| [efcore_dapper_integration.cs](file:///workspace/efcore_dapper_integration.cs) | EF Core + Dapper 混合集成 |
| [litedb_integration.cs](file:///workspace/litedb_integration.cs) | LiteDB 嵌入式数据库 |
| [litedb_event_sourcing.cs](file:///workspace/litedb_event_sourcing.cs) | LiteDB 事件溯源 |
| [litedb_aot.cs](file:///workspace/litedb_aot.cs) | LiteDB AOT 编译 |
| [litedb_tieredmemory.cs](file:///workspace/litedb_tieredmemory.cs) | LiteDB 分层内存 |
| [minio_integration.cs](file:///workspace/minio_integration.cs) | MinIO 对象存储 |
| [clickhouse_integration.cs](file:///workspace/clickhouse_integration.cs) | ClickHouse 列式数据库 |
| [influxdb_integration.cs](file:///workspace/influxdb_integration.cs) | InfluxDB 时序数据库 |
| [mysql_to_sqlite_litedb_sync.cs](file:///workspace/mysql_to_sqlite_litedb_sync.cs) | MySQL→SQLite/LiteDB 同步 |
| [sqlite_event_store.cs](file:///workspace/sqlite_event_store.cs) | SQLite 事件存储 |
| [sqlite_integration.cs](file:///workspace/sqlite_integration.cs) | SQLite 集成 |
| [sqlite_bulk_writer.cs](file:///workspace/sqlite_bulk_writer.cs) | SQLite 批量写入 |
| [sqlite_connection_pool.cs](file:///workspace/sqlite_connection_pool.cs) | SQLite 连接池 |
| [read_write_split.cs](file:///workspace/read_write_split.cs) | 读写分离 |
| [ChangeDataCaptureService.cs](file:///workspace/ChangeDataCaptureService.cs) | CDC 变更数据捕获 |

#### 4.1.7 缓存

| 文件 | 说明 |
|------|------|
| [stackexchange_redis_cache.cs](file:///workspace/stackexchange_redis_cache.cs) | StackExchange.Redis 缓存 |
| [garnet_redis_cache.cs](file:///workspace/garnet_redis_cache.cs) | Garnet Redis 替代缓存 |
| [easycaching_demo.cs](file:///workspace/easycaching_demo.cs) | EasyCaching 多级缓存 |
| [fusioncache_demo.cs](file:///workspace/fusioncache_demo.cs) | FusionCache 混合缓存 |
| [cache_hybrid_integration.cs](file:///workspace/cache_hybrid_integration.cs) | HybridCache 混合缓存 |
| [cache_heartbeat_integration.cs](file:///workspace/cache_heartbeat_integration.cs) | 缓存心跳检测 |
| [lazycache_integration.cs](file:///workspace/lazycache_integration.cs) | LazyCache 懒加载缓存 |
| [microsoft_cache_demo.cs](file:///workspace/microsoft_cache_demo.cs) | Microsoft.Extensions.Caching |

#### 4.1.8 可观测性

| 文件 | 说明 |
|------|------|
| [opentelemetry_integration.cs](file:///workspace/opentelemetry_integration.cs) | OpenTelemetry 分布式追踪 |
| [opentelemetry_production.cs](file:///workspace/opentelemetry_production.cs) | OpenTelemetry 生产级配置 |
| [prometheus_metrics.cs](file:///workspace/prometheus_metrics.cs) | Prometheus 指标采集 |
| [prometheus_integration.cs](file:///workspace/prometheus_integration.cs) | Prometheus 集成 |
| [grafana_integration.cs](file:///workspace/grafana_integration.cs) | Grafana 可视化 |
| [seq_integration.cs](file:///workspace/seq_integration.cs) | Seq 结构化日志 |
| [serilog_integration.cs](file:///workspace/serilog_integration.cs) | Serilog 日志集成 |
| [appinsights_integration.cs](file:///workspace/appinsights_integration.cs) | Application Insights 集成 |
| [jaeger_integration.cs](file:///workspace/jaeger_integration.cs) | Jaeger 追踪 |
| [zipkin_integration.cs](file:///workspace/zipkin_integration.cs) | Zipkin 追踪 |
| [skyapm_integration.cs](file:///workspace/skyapm_integration.cs) | SkyWalking APM |
| [datadog_integration.cs](file:///workspace/datadog_integration.cs) | Datadog 监控 |
| [performance_metrics.cs](file:///workspace/performance_metrics.cs) | 性能指标 |
| [metrics_monitoring.cs](file:///workspace/metrics_monitoring.cs) | 指标监控 |
| [metrics_enhanced.cs](file:///workspace/metrics_enhanced.cs) | 增强指标 |
| [metrics_influxdb.cs](file:///workspace/metrics_influxdb.cs) | InfluxDB 指标存储 |

#### 4.1.9 安全与认证

| 文件 | 说明 |
|------|------|
| [abac_attribute_based_access.cs](file:///workspace/abac_attribute_based_access.cs) | ABAC 基于属性的访问控制 |
| [auth_integration.cs](file:///workspace/auth_integration.cs) | 认证集成 |
| [identity_integration.cs](file:///workspace/identity_integration.cs) | ASP.NET Identity 集成 |
| [openiddict_service.cs](file:///workspace/openiddict_service.cs) | OpenIddict 认证服务 |
| [keycloak_integration.cs](file:///workspace/keycloak_integration.cs) | Keycloak 单点登录 |
| [keycloak_service.cs](file:///workspace/keycloak_service.cs) | Keycloak 服务 |
| [casbin_integration.cs](file:///workspace/casbin_integration.cs) | Casbin 权限管理 |
| [sso_integration.cs](file:///workspace/sso_integration.cs) | SSO 单点登录 |
| [sso_service.cs](file:///workspace/sso_service.cs) | SSO 服务 |
| [sso_microservice.cs](file:///workspace/sso_microservice.cs) | SSO 微服务 |
| [aes_gcm_encryption_integration.cs](file:///workspace/aes_gcm_encryption_integration.cs) | AES-GCM 加密 |
| [rsa_encryption_integration.cs](file:///workspace/rsa_encryption_integration.cs) | RSA 加密 |
| [bouncycastle_integration.cs](file:///workspace/bouncycastle_integration.cs) | BouncyCastle 加密库 |
| [bouncycastle_advanced.cs](file:///workspace/bouncycastle_advanced.cs) | BouncyCastle 高级功能 |
| [sm_crypto_integration.cs](file:///workspace/sm_crypto_integration.cs) | 国密 SM2/SM3/SM4 |
| [quantum_crypto_integration.cs](file:///workspace/quantum_crypto_integration.cs) | 量子加密 |
| [acme_cert_monitor.cs](file:///workspace/acme_cert_monitor.cs) | ACME 证书监控 |
| [acme_dotnet_integration.cs](file:///workspace/acme_dotnet_integration.cs) | ACME 证书自动化 |
| [dataprotection_integration.cs](file:///workspace/dataprotection_integration.cs) | 数据保护 |
| [securityheaders_integration.cs](file:///workspace/securityheaders_integration.cs) | 安全头配置 |

#### 4.1.10 网络与协议

| 文件 | 说明 |
|------|------|
| [grpc_integration.cs](file:///workspace/grpc_integration.cs) | gRPC 服务集成 |
| [grpc_protobufnet.cs](file:///workspace/grpc_protobufnet.cs) | gRPC + protobuf-net |
| [http3_quic.cs](file:///workspace/http3_quic.cs) | HTTP/3 QUIC 协议 |
| [quic_production.cs](file:///workspace/quic_production.cs) | QUIC 生产级实现 |
| [signalr_production.cs](file:///workspace/signalr_production.cs) | SignalR 生产级配置 |
| [signalr_integration.cs](file:///workspace/signalr_integration.cs) | SignalR 实时通信 |
| [signalr_advanced.cs](file:///workspace/signalr_advanced.cs) | SignalR 高级功能 |
| [websocket_integration.cs](file:///workspace/websocket_integration.cs) | WebSocket 集成 |
| [fleck_websocket.cs](file:///workspace/fleck_websocket.cs) | Fleck WebSocket |
| [mqtt_integration.cs](file:///workspace/mqtt_integration.cs) | MQTT 协议集成 |
| [mqtt_production_integration.cs](file:///workspace/mqtt_production_integration.cs) | MQTT 生产级 |
| [mqtt_advanced_optimization.cs](file:///workspace/mqtt_advanced_optimization.cs) | MQTT 高级优化 |
| [mqttnet.cs](file:///workspace/mqttnet.cs) | MQTTnet 库集成 |
| [mqttnet_demo.cs](file:///workspace/mqttnet_demo.cs) | MQTTnet 演示 |
| [zeromq_demo.cs](file:///workspace/zeromq_demo.cs) | ZeroMQ 演示 |
| [sip_stun_integration.cs](file:///workspace/sip_stun_integration.cs) | SIP/STUN 协议 |
| [sipsorcery_integration.cs](file:///workspace/sipsorcery_integration.cs) | SIP Sorcery 集成 |
| [kcp_transport.cs](file:///workspace/kcp_transport.cs) | KCP 可靠传输 |
| [voip_integration.cs](file:///workspace/voip_integration.cs) | VoIP 集成 |
| [dnsclient_integration.cs](file:///workspace/dnsclient_integration.cs) | DNS 客户端 |
| [dnsserver_integration.cs](file:///workspace/dnsserver_integration.cs) | DNS 服务端 |
| [mdns_integration.cs](file:///workspace/mdns_integration.cs) | mDNS 服务发现 |
| [p2p_integration.cs](file:///workspace/p2p_integration.cs) | P2P 点对点通信 |

#### 4.1.11 微服务与分布式

| 文件 | 说明 |
|------|------|
| [dapr_integration.cs](file:///workspace/dapr_integration.cs) | Dapr 分布式运行时 |
| [service_discovery_integration.cs](file:///workspace/service_discovery_integration.cs) | 服务发现 |
| [service_discovery_advanced_integration.cs](file:///workspace/service_discovery_advanced_integration.cs) | 高级服务发现 |
| [consul_integration.cs](file:///workspace/consul_integration.cs) | Consul 服务注册 |
| [nacos_integration.cs](file:///workspace/nacos_integration.cs) | Nacos 配置中心 |
| [etcd_integration.cs](file:///workspace/etcd_integration.cs) | etcd 分布式存储 |
| [raft_service.cs](file:///workspace/raft_service.cs) | Raft 共识算法 |
| [distributed_transaction.cs](file:///workspace/distributed_transaction.cs) | 分布式事务 |
| [distributed_lock.cs](file:///workspace/distributed_lock.cs) | 分布式锁 |
| [distributed_file_lock.cs](file:///workspace/distributed_file_lock.cs) | 分布式文件锁 |
| [distributed_memory.cs](file:///workspace/distributed_memory.cs) | 分布式内存 |
| [load_balancer.cs](file:///workspace/load_balancer.cs) | 负载均衡器 |
| [rate_limiter.cs](file:///workspace/rate_limiter.cs) | 速率限制器 |
| [service_mesh_enhanced.cs](file:///workspace/service_mesh_enhanced.cs) | 服务网格增强 |
| [resonance_adapter.cs](file:///workspace/resonance_adapter.cs) | Resonance 信令适配器 |

#### 4.1.12 ID 生成器

| 文件 | 说明 |
|------|------|
| [snowflake_service.cs](file:///workspace/snowflake_service.cs) | 雪花算法 ID 生成 |
| [snowflake_drift_service.cs](file:///workspace/snowflake_drift_service.cs) | 雪花算法时钟回拨处理 |
| [ulid_service.cs](file:///workspace/ulid_service.cs) | ULID 排序 ID 生成 |
| [idgenerator_service.cs](file:///workspace/idgenerator_service.cs) | 通用 ID 生成服务 |

#### 4.1.13 YARP 反向代理

| 文件 | 说明 |
|------|------|
| [yarp_integration.cs](file:///workspace/yarp_integration.cs) | YARP 反向代理集成 |
| [yarp_advanced_features.cs](file:///workspace/yarp_advanced_features.cs) | YARP 高级功能 |
| [yarp_circuit_breaker.cs](file:///workspace/yarp_circuit_breaker.cs) | YARP 断路器 |
| [yarp_service_discovery.cs](file:///workspace/yarp_service_discovery.cs) | YARP 服务发现 |
| [yarp_batch_middleware.cs](file:///workspace/yarp_batch_middleware.cs) | YARP 批处理中间件 |
| [yarp_compression_middleware.cs](file:///workspace/yarp_compression_middleware.cs) | YARP 压缩中间件 |
| [yarp_signature_middleware.cs](file:///workspace/yarp_signature_middleware.cs) | YARP 签名中间件 |
| [reverse_proxy_demo.cs](file:///workspace/reverse_proxy_demo.cs) | 反向代理演示 |

#### 4.1.14 Wolverine 框架

| 文件 | 说明 |
|------|------|
| [wolverine_integration.cs](file:///workspace/wolverine_integration.cs) | Wolverine 消息框架 |
| [wolverine_cqrs.cs](file:///workspace/wolverine_cqrs.cs) | Wolverine CQRS |
| [wolverine_event_bus.cs](file:///workspace/wolverine_event_bus.cs) | Wolverine 事件总线 |
| [wolverine_saga.cs](file:///workspace/wolverine_saga.cs) | Wolverine Saga |
| [wolverine_saga_orchestration.cs](file:///workspace/wolverine_saga_orchestration.cs) | Wolverine Saga 编排 |
| [wolverinefx_dapr_integration.cs](file:///workspace/wolverinefx_dapr_integration.cs) | WolverineFx + Dapr |
| [wolverinefx_grpc_integration.cs](file:///workspace/wolverinefx_grpc_integration.cs) | WolverineFx + gRPC |
| [wolverinefx_kafka_integration.cs](file:///workspace/wolverinefx_kafka_integration.cs) | WolverineFx + Kafka |
| [wolverinefx_mqtt_integration_message.cs](file:///workspace/wolverinefx_mqtt_integration_message.cs) | WolverineFx + MQTT |
| [wolverinefx_orleans_integration.cs](file:///workspace/wolverinefx_orleans_integration.cs) | WolverineFx + Orleans |
| [wolverinefx_signalr_integration.cs](file:///workspace/wolverinefx_signalr_integration.cs) | WolverineFx + SignalR |
| [wolverinefx_tcc_integration.cs](file:///workspace/wolverinefx_tcc_integration.cs) | WolverineFx TCC 事务 |
| [wolverinefx_xa_integration.cs](file:///workspace/wolverinefx_xa_integration.cs) | WolverineFx XA 事务 |

#### 4.1.15 MediatR 模式

| 文件 | 说明 |
|------|------|
| [mediatr_production_integration.cs](file:///workspace/mediatr_production_integration.cs) | MediatR 生产级集成 |
| [mediatr_dapr_integration.cs](file:///workspace/mediatr_dapr_integration.cs) | MediatR + Dapr |
| [mediatr_grpc_integration.cs](file:///workspace/mediatr_grpc_integration.cs) | MediatR + gRPC |
| [mediatr_kafka_integration.cs](file:///workspace/mediatr_kafka_integration.cs) | MediatR + Kafka |
| [mediatr_mqtt_integration_pubsub.cs](file:///workspace/mediatr_mqtt_integration_pubsub.cs) | MediatR + MQTT 发布订阅 |
| [mediatr_orleans_integration.cs](file:///workspace/mediatr_orleans_integration.cs) | MediatR + Orleans |
| [mediatr_saga_integration.cs](file:///workspace/mediatr_saga_integration.cs) | MediatR + Saga |
| [mediatr_signalr_integration.cs](file:///workspace/mediatr_signalr_integration.cs) | MediatR + SignalR |
| [mediatr_tcc_integration.cs](file:///workspace/mediatr_tcc_integration.cs) | MediatR + TCC |
| [mediatr_xa_integration.cs](file:///workspace/mediatr_xa_integration.cs) | MediatR + XA |

#### 4.1.16 MagicOnion

| 文件 | 说明 |
|------|------|
| [magiconion_integration.cs](file:///workspace/magiconion_integration.cs) | MagicOnion 实时通信 |
| [magiconion_chat.cs](file:///workspace/magiconion_chat.cs) | MagicOnion 聊天 |
| [magiconion_mesh.cs](file:///workspace/magiconion_mesh.cs) | MagicOnion Mesh |
| [magiconion_persistence.cs](file:///workspace/magiconion_persistence.cs) | MagicOnion 持久化 |
| [magiconion_tracing.cs](file:///workspace/magiconion_tracing.cs) | MagicOnion 追踪 |
| [magiconion_realtime.cs](file:///workspace/magiconion_realtime.cs) | MagicOnion 实时 |
| [magiconion_todo.cs](file:///workspace/magiconion_todo.cs) | MagicOnion Todo 示例 |

#### 4.1.17 Refit / RestSharp / HTTP 客户端

| 文件 | 说明 |
|------|------|
| [refit_integration.cs](file:///workspace/refit_integration.cs) | Refit 声明式 HTTP 客户端 |
| [refit_contract.cs](file:///workspace/refit_contract.cs) | Refit 接口契约 |
| [refit_di.cs](file:///workspace/refit_di.cs) | Refit DI 注入 |
| [refit_metrics.cs](file:///workspace/refit_metrics.cs) | Refit 指标 |
| [refit_resiliency.cs](file:///workspace/refit_resiliency.cs) | Refit 弹性 |
| [restsharp_integration.cs](file:///workspace/restsharp_integration.cs) | RestSharp 集成 |
| [restsharp_factory.cs](file:///workspace/restsharp_factory.cs) | RestSharp 工厂 |
| [http_resilience_integration.cs](file:///workspace/http_resilience_integration.cs) | HTTP 弹性集成 |
| [http_resilience_advanced_integration.cs](file:///workspace/http_resilience_advanced_integration.cs) | HTTP 高级弹性 |
| [webapiclientcore_integration.cs](file:///workspace/webapiclientcore_integration.cs) | WebApiClientCore 集成 |

#### 4.1.18 容器与编排

| 文件 | 说明 |
|------|------|
| [docker_integration.cs](file:///workspace/docker_integration.cs) | Docker 集成 |
| [docker_compose_integration.cs](file:///workspace/docker_compose_integration.cs) | Docker Compose 集成 |
| [k8s_integration.cs](file:///workspace/k8s_integration.cs) | Kubernetes 集成 |
| [k3s_integration.cs](file:///workspace/k3s_integration.cs) | K3s 轻量 Kubernetes |
| [minikube_integration.cs](file:///workspace/minikube_integration.cs) | Minikube 集成 |
| [podman_integration.cs](file:///workspace/podman_integration.cs) | Podman 容器 |
| [tye_integration.cs](file:///workspace/tye_integration.cs) | Tye 微服务编排 |
| [aspire_integration.cs](file:///workspace/aspire_integration.cs) | .NET Aspire 云原生 |

#### 4.1.19 源代码生成器

| 文件 | 说明 |
|------|------|
| [SourceGenerator_demo.cs](file:///workspace/SourceGenerator_demo.cs) | 源代码生成器基础演示 |
| [SourceGenerator_advanced.cs](file:///workspace/SourceGenerator_advanced.cs) | 源代码生成器高级用法 |
| [SourceGenerator_complete.cs](file:///workspace/SourceGenerator_complete.cs) | 源代码生成器完整实现 |
| [SourceGenerator_ddd_generator.cs](file:///workspace/SourceGenerator_ddd_generator.cs) | DDD 源代码生成器 |
| [SourceGenerator_ddd_advanced_generator.cs](file:///workspace/SourceGenerator_ddd_advanced_generator.cs) | DDD 高级源代码生成器 |
| [SourceGenerator_fody_mqtt_test.cs](file:///workspace/SourceGenerator_fody_mqtt_test.cs) | Fody + MQTT 源代码生成 |
| [SourceGenerator_mqtt_disruptor_impl.cs](file:///workspace/SourceGenerator_mqtt_disruptor_impl.cs) | MQTT Disruptor 源代码生成 |

#### 4.1.20 任务调度

| 文件 | 说明 |
|------|------|
| [hangfire_job_service.cs](file:///workspace/hangfire_job_service.cs) | Hangfire 后台任务 |
| [coravel_service.cs](file:///workspace/coravel_service.cs) | Coravel 任务调度 |
| [fluent_scheduler_service.cs](file:///workspace/fluent_scheduler_service.cs) | FluentScheduler |
| [quartz_integration.cs](file:///workspace/quartz_integration.cs) | Quartz.NET 调度 |
| [schedule_master_service.cs](file:///workspace/schedule_master_service.cs) | 调度主控服务 |
| [task_scheduler_service.cs](file:///workspace/task_scheduler_service.cs) | 任务调度服务 |
| [gofer_service.cs](file:///workspace/gofer_service.cs) | Gofer 任务服务 |
| [foundatio_service.cs](file:///workspace/foundatio_service.cs) | Foundatio 队列服务 |

#### 4.1.21 数据验证与映射

| 文件 | 说明 |
|------|------|
| [fluentvalidation_integration.cs](file:///workspace/fluentvalidation_integration.cs) | FluentValidation 验证 |
| [fv_fluent_validation.cs](file:///workspace/fv_fluent_validation.cs) | FluentValidation 高级 |
| [DataValidationOptions.cs](file:///workspace/DataValidationOptions.cs) | 数据验证选项 |
| [mapster.cs](file:///workspace/mapster.cs) | Mapster 对象映射 |
| [mapperly.cs](file:///workspace/mapperly.cs) | Mapperly 源代码生成映射 |
| [automapper_integration.cs](file:///workspace/automapper_integration.cs) | AutoMapper 集成 |

#### 4.1.22 报表与导出

| 文件 | 说明 |
|------|------|
| [miniexcel_integration.cs](file:///workspace/miniexcel_integration.cs) | MiniExcel 轻量导入导出 |
| [miniexcel_production.cs](file:///workspace/miniexcel_production.cs) | MiniExcel 生产级 |
| [miniexcel_advanced.cs](file:///workspace/miniexcel_advanced.cs) | MiniExcel 高级功能 |
| [miniexcel_full_features.cs](file:///workspace/miniexcel_full_features.cs) | MiniExcel 全功能 |
| [closedxml_production.cs](file:///workspace/closedxml_production.cs) | ClosedXML 生产级 |
| [csvhelper_production.cs](file:///workspace/csvhelper_production.cs) | CsvHelper 生产级 |
| [csvexport_integration.cs](file:///workspace/csvexport_integration.cs) | CSV 导出 |
| [magicodes_ie_integration.cs](file:///workspace/magicodes_ie_integration.cs) | Magicodes.IE 导入导出 |
| [exceldatareader_integration.cs](file:///workspace/exceldatareader_integration.cs) | ExcelDataReader |
| [cellreport_integration.cs](file:///workspace/cellreport_integration.cs) | CellReport 报表 |
| [fastreport_integration.cs](file:///workspace/fastreport_integration.cs) | FastReport 报表 |
| [pdfreport_integration.cs](file:///workspace/pdfreport_integration.cs) | PDF 报表 |
| [questpdf_integration.cs](file:///workspace/questpdf_integration.cs) | QuestPDF 生成 |

#### 4.1.23 图像/视频处理

| 文件 | 说明 |
|------|------|
| [imagesharp_image_processing.cs](file:///workspace/imagesharp_image_processing.cs) | ImageSharp 图像处理 |
| [opencvsharp_image_processing.cs](file:///workspace/opencvsharp_image_processing.cs) | OpenCVSharp 图像处理 |
| [opencv_computer_vision_integration.cs](file:///workspace/opencv_computer_vision_integration.cs) | OpenCV 计算机视觉 |
| [opencv_biometric_processor.cs](file:///workspace/opencv_biometric_processor.cs) | OpenCV 生物识别 |
| [opencv_video_processing.cs](file:///workspace/opencv_video_processing.cs) | OpenCV 视频处理 |
| [emgucv_image_processing.cs](file:///workspace/emgucv_image_processing.cs) | EmguCV 图像处理 |
| [magick_net_image_processing.cs](file:///workspace/magick_net_image_processing.cs) | Magick.NET 图像处理 |
| [skaia_image_processing.cs](file:///workspace/skaia_image_processing.cs) | SkiaSharp 图像处理 |
| [yolov7_image_detection.cs](file:///workspace/yolov7_image_detection.cs) | YOLOv7 目标检测 |
| [facedetect_torchsharp_integration.cs](file:///workspace/facedetect_torchsharp_integration.cs) | 人脸检测 TorchSharp |
| [facerecognition_dotnet_integration.cs](file:///workspace/facerecognition_dotnet_integration.cs) | 人脸识别 |
| [paddle_ocr_integration.cs](file:///workspace/paddle_ocr_integration.cs) | PaddleOCR 文字识别 |
| [paddle_ocr_enhanced.cs](file:///workspace/paddle_ocr_enhanced.cs) | PaddleOCR 增强版 |
| [ffmpeg_integration.cs](file:///workspace/ffmpeg_integration.cs) | FFmpeg 音视频处理 |
| [libvlcsharp_integration.cs](file:///workspace/libvlcsharp_integration.cs) | LibVLC 播放器集成 |
| [libvlcsharp_blazor_integration.cs](file:///workspace/libvlcsharp_blazor_integration.cs) | LibVLC Blazor 集成 |
| [libvlcsharp_live_streaming.cs](file:///workspace/libvlcsharp_live_streaming.cs) | LibVLC 直播流 |
| [libvlcsharp_webrtc_integration.cs](file:///workspace/libvlcsharp_webrtc_integration.cs) | LibVLC + WebRTC |
| [captcha_recognizer_integration.cs](file:///workspace/captcha_recognizer_integration.cs) | 验证码识别 |
| [qrcode_barcode_integration.cs](file:///workspace/qrcode_barcode_integration.cs) | 二维码/条形码 |

#### 4.1.24 音频/语音

| 文件 | 说明 |
|------|------|
| [asr_integration.cs](file:///workspace/asr_integration.cs) | 语音识别 ASR |
| [tts_integration.cs](file:///workspace/tts_integration.cs) | 文本转语音 TTS |
| [speech_integration.cs](file:///workspace/speech_integration.cs) | 语音服务集成 |
| [audio_enhancement.cs](file:///workspace/audio_enhancement.cs) | 音频增强 |
| [naudio_integration.cs](file:///workspace/naudio_integration.cs) | NAudio 音频处理 |
| [cscore_audio_integration.cs](file:///workspace/cscore_audio_integration.cs) | CSCore 音频 |
| [accord_audio_integration.cs](file:///workspace/accord_audio_integration.cs) | Accord.NET 音频 |
| [csvideo_audio.cs](file:///workspace/csvideo_audio.cs) | 音视频综合 |
| [rtc_audio_processor.cs](file:///workspace/rtc_audio_processor.cs) | RTC 音频处理 |

#### 4.1.25 AI/ML

| 文件 | 说明 |
|------|------|
| [SemanticKernel.cs](file:///workspace/SemanticKernel.cs) | Semantic Kernel 入门 |
| [semantic_kernel.cs](file:///workspace/semantic_kernel.cs) | Semantic Kernel 集成 |
| [semantic_kernel_integration.cs](file:///workspace/semantic_kernel_integration.cs) | Semantic Kernel 深度集成 |
| [semantic_kernel_service.cs](file:///workspace/semantic_kernel_service.cs) | Semantic Kernel 服务 |
| [semantic_memory_integration.cs](file:///workspace/semantic_memory_integration.cs) | Semantic Memory 集成 |
| [kernel_memory_integration.cs](file:///workspace/kernel_memory_integration.cs) | Kernel Memory 集成 |
| [tensorflowsharp_integration.cs](file:///workspace/tensorflowsharp_integration.cs) | TensorFlow.NET 集成 |
| [ollamasharp_integration.cs](file:///workspace/ollamasharp_integration.cs) | OllamaSharp 集成 |
| [microsoft_extensions_ai_integration.cs](file:///workspace/microsoft_extensions_ai_integration.cs) | Microsoft.Extensions.AI |
| [llm_integration.cs](file:///workspace/llm_integration.cs) | LLM 大语言模型集成 |
| [autogen_integration.cs](file:///workspace/autogen_integration.cs) | AutoGen 多智能体 |
| [botsharp_integration.cs](file:///workspace/botsharp_integration.cs) | BotSharp 机器人框架 |
| [longchain_integration.cs](file:///workspace/longchain_integration.cs) | LangChain 集成 |
| [ml_service_pipeline.cs](file:///workspace/ml_service_pipeline.cs) | ML.NET 服务管道 |
| [milvus_integration.cs](file:///workspace/milvus_integration.cs) | Milvus 向量数据库 |
| [antsk_knowledgebase_integration.cs](file:///workspace/antsk_knowledgebase_integration.cs) | AntSK 知识库 |

#### 4.1.26 多租户

| 文件 | 说明 |
|------|------|
| [multitenant_integration.cs](file:///workspace/multitenant_integration.cs) | Finbuckle 多租户集成 |
| [multitenant_cache_service.cs](file:///workspace/multitenant_cache_service.cs) | 多租户缓存服务 |
| [multitenant_lifecycle.cs](file:///workspace/multitenant_lifecycle.cs) | 多租户生命周期 |
| [multitenant_quota_management.cs](file:///workspace/multitenant_quota_management.cs) | 多租户配额管理 |

#### 4.1.27 工作流

| 文件 | 说明 |
|------|------|
| [elsa_workflow_integration.cs](file:///workspace/elsa_workflow_integration.cs) | Elsa 工作流引擎 |
| [workflow_core_integration.cs](file:///workspace/workflow_core_integration.cs) | Workflow Core 工作流 |

#### 4.1.28 插件系统

| 文件 | 说明 |
|------|------|
| [PluginLoadContext.cs](file:///workspace/PluginLoadContext.cs) | 插件加载上下文 |
| [plugin_service.cs](file:///workspace/plugin_service.cs) | 插件服务 |
| [netpro_plugin_service.cs](file:///workspace/netpro_plugin_service.cs) | NetPro 插件服务 |
| [oqtane_plugin_service.cs](file:///workspace/oqtane_plugin_service.cs) | Oqtane 插件服务 |

#### 4.1.29 依赖注入

| 文件 | 说明 |
|------|------|
| [autofac_production.cs](file:///workspace/autofac_production.cs) | Autofac 生产级 DI |
| [scrutor_integration.cs](file:///workspace/scrutor_integration.cs) | Scrutor 装饰器/扫描 |
| [scrutor_advanced_features.cs](file:///workspace/scrutor_advanced_features.cs) | Scrutor 高级功能 |
| [dependency_injection_integration.cs](file:///workspace/dependency_injection_integration.cs) | 依赖注入集成 |
| [dependency_injection_advanced_integration.cs](file:///workspace/dependency_injection_advanced_integration.cs) | 高级依赖注入 |

#### 4.1.30 动态代理/AOP

| 文件 | 说明 |
|------|------|
| [CastleDynamicProxy.cs](file:///workspace/CastleDynamicProxy.cs) | Castle DynamicProxy 动态代理 |
| [castle_core_integration.cs](file:///workspace/castle_core_integration.cs) | Castle.Core 集成 |
| [castle_proxy_demo.cs](file:///workspace/castle_proxy_demo.cs) | Castle 代理演示 |
| [rougamo_integration.cs](file:///workspace/rougamo_integration.cs) | Rougamo AOP 编译时织入 |

#### 4.1.31 测试

| 文件 | 说明 |
|------|------|
| [xunit_service_testing.cs](file:///workspace/xunit_service_testing.cs) | xUnit 服务测试 |
| [benchmark_demo.cs](file:///workspace/benchmark_demo.cs) | BenchmarkDotNet 性能测试 |
| [k6_todo_benchmark.js](file:///workspace/k6_todo_benchmark.js) | K6 压力测试 |
| [httpclient_test.cs](file:///workspace/httpclient_test.cs) | HttpClient 测试 |

#### 4.1.32 前端/UI

| 文件 | 说明 |
|------|------|
| [htmx.cs](file:///workspace/htmx.cs) | HTMX 集成 |
| [blazor_integration.cs](file:///workspace/blazor_integration.cs) | Blazor 集成 |
| [avalonia_integration.cs](file:///workspace/avalonia_integration.cs) | Avalonia UI 集成 |
| [photino_integration.cs](file:///workspace/photino_integration.cs) | Photino 桌面应用 |
| [handycontrol_integration.cs](file:///workspace/handycontrol_integration.cs) | HandyControl UI 库 |
| [silky_maui_integration.cs](file:///workspace/silky_maui_integration.cs) | MAUI 集成 |
| [echarts_integration.cs](file:///workspace/echarts_integration.cs) | ECharts 图表 |
| [highcharts_integration.cs](file:///workspace/highcharts_integration.cs) | Highcharts 图表 |
| [tailwindcss_integration.cs](file:///workspace/tailwindcss_integration.cs) | Tailwind CSS 集成 |
| [vue_crud.js](file:///workspace/vue_crud.js) | Vue.js CRUD 组件 |
| [webcrypto_service.ts](file:///workspace/webcrypto_service.ts) | WebCrypto TypeScript 服务 |

#### 4.1.33 其他工具

| 文件 | 说明 |
|------|------|
| [mailkit_integration.cs](file:///workspace/mailkit_integration.cs) | MailKit 邮件服务 |
| [sms_integration.cs](file:///workspace/sms_integration.cs) | 短信服务集成 |
| [localization_integration.cs](file:///workspace/localization_integration.cs) | 本地化/国际化 |
| [nodatime_extensions.cs](file:///workspace/nodatime_extensions.cs) | NodaTime 日期时间 |
| [humanizer_integration.cs](file:///workspace/humanizer_integration.cs) | Humanizer 人性化字符串 |
| [ip2region_integration.cs](file:///workspace/ip2region_integration.cs) | IP 地址定位 |
| [jieba_integration.cs](file:///workspace/jieba_integration.cs) | 结巴中文分词 |
| [pinYin_integration.cs](file:///workspace/pinYin_integration.cs) | 拼音转换 |
| [htmlagilitypack_integration.cs](file:///workspace/htmlagilitypack_integration.cs) | HTML 解析 |
| [htmlsanitizer_integration.cs](file:///workspace/htmlsanitizer_integration.cs) | HTML 净化 |
| [csscript_integration.cs](file:///workspace/csscript_integration.cs) | CS-Script 脚本引擎 |
| [dotnetscript_integration.cs](file:///workspace/dotnetscript_integration.cs) | .NET 脚本 |
| [natasha_integration.cs](file:///workspace/natasha_integration.cs) | Natasha 动态编译 |
| [compression_extension.cs](file:///workspace/compression_extension.cs) | 压缩扩展 |
| [brotli_compression_integration.cs](file:///workspace/brotli_compression_integration.cs) | Brotli 压缩 |
| [lz4_integration.cs](file:///workspace/lz4_integration.cs) | LZ4 高速压缩 |
| [zstd_compression_integration.cs](file:///workspace/zstd_compression_integration.cs) | Zstd 压缩 |
| [sharpziplib_integration.cs](file:///workspace/sharpziplib_integration.cs) | SharpZipLib 压缩 |
| [version_control_manager.cs](file:///workspace/version_control_manager.cs) | 版本控制管理 |
| [version_diff_analyzer.cs](file:///workspace/version_diff_analyzer.cs) | 版本差异分析 |
| [version_metadata_service.cs](file:///workspace/version_metadata_service.cs) | 版本元数据服务 |
| [database_backup_service.cs](file:///workspace/database_backup_service.cs) | 数据库备份服务 |
| [incremental_backup.cs](file:///workspace/incremental_backup.cs) | 增量备份 |
| [file_versioning.cs](file:///workspace/file_versioning.cs) | 文件版本管理 |
| [transactional_filemgr_integration.cs](file:///workspace/transactional_filemgr_integration.cs) | 事务性文件管理 |

---

### 4.2 code/ - 高级集成示例

`code/` 目录包含 **49 个高级集成文件**，侧重于跨框架组合和高级场景：

| 文件 | 说明 |
|------|------|
| [wolverine_cqrs.cs](file:///workspace/code/wolverine_cqrs.cs) | Wolverine CQRS 实现 |
| [wolverine_event_bus.cs](file:///workspace/code/wolverine_event_bus.cs) | Wolverine 事件总线 |
| [wolverine_integration.cs](file:///workspace/code/wolverine_integration.cs) | Wolverine 基础集成 |
| [masstransit_production_integration.cs](file:///workspace/code/masstransit_production_integration.cs) | MassTransit 生产级 |
| [grpc_protobufnet.cs](file:///workspace/code/grpc_protobufnet.cs) | gRPC + protobuf-net |
| [http_resilience_integration.cs](file:///workspace/code/http_resilience_integration.cs) | HTTP 弹性 |
| [hybrid_protocol_advanced.cs](file:///workspace/code/hybrid_protocol_advanced.cs) | 高级混合协议 |
| [efcore9_integration.cs](file:///workspace/code/efcore9_integration.cs) | EF Core 9 |
| [efcore_dapper_integration.cs](file:///workspace/code/efcore_dapper_integration.cs) | EF Core + Dapper |
| [garnet_redis_cache.cs](file:///workspace/code/garnet_redis_cache.cs) | Garnet 缓存 |
| [litedb_aot.cs](file:///workspace/code/litedb_aot.cs) | LiteDB AOT |
| [litedb_tieredmemory.cs](file:///workspace/code/litedb_tieredmemory.cs) | LiteDB 分层内存 |
| [magiconion_mesh.cs](file:///workspace/code/magiconion_mesh.cs) | MagicOnion Mesh |
| [magiconion_persistence.cs](file:///workspace/code/magiconion_persistence.cs) | MagicOnion 持久化 |
| [magiconion_tracing.cs](file:///workspace/code/magiconion_tracing.cs) | MagicOnion 追踪 |
| [mqtt_advanced_optimization.cs](file:///workspace/code/mqtt_advanced_optimization.cs) | MQTT 高级优化 |
| [mqtt_quantum_security.cs](file:///workspace/code/mqtt_quantum_security.cs) | MQTT 量子安全 |
| [quic_production.cs](file:///workspace/code/quic_production.cs) | QUIC 生产级 |
| [sip_stun_integration.cs](file:///workspace/code/sip_stun_integration.cs) | SIP/STUN |
| [voip_integration.cs](file:///workspace/code/voip_integration.cs) | VoIP |
| [zeromq_demo.cs](file:///workspace/code/zeromq_demo.cs) | ZeroMQ |
| [specification_pattern.cs](file:///workspace/code/specification_pattern.cs) | 规约模式 |
| [dynamic_router.cs](file:///workspace/code/dynamic_router.cs) | 动态路由 |
| [idgenerator_service.cs](file:///workspace/code/idgenerator_service.cs) | ID 生成服务 |
| [kiota_cache.cs](file:///workspace/code/kiota_cache.cs) | Kiota 缓存 |
| [kiota_metrics.cs](file:///workspace/code/kiota_metrics.cs) | Kiota 指标 |
| [librdkafka_integration.cs](file:///workspace/code/librdkafka_integration.cs) | librdkafka |
| [liquidstate_integration.cs](file:///workspace/code/liquidstate_integration.cs) | LiquidState 状态机 |
| [llcom_integration.cs](file:///workspace/code/llcom_integration.cs) | LLCOM 串口 |
| [mailkit_integration.cs](file:///workspace/code/mailkit_integration.cs) | MailKit 邮件 |
| [minio_integration.cs](file:///workspace/code/minio_integration.cs) | MinIO 对象存储 |
| [nrules_integration.cs](file:///workspace/code/nrules_integration.cs) | NRules 规则引擎 |
| [openauth_integration.cs](file:///workspace/code/openauth_integration.cs) | OpenAuth 认证 |
| [performance_metrics.cs](file:///workspace/code/performance_metrics.cs) | 性能指标 |
| [tensorflowsharp_integration.cs](file:///workspace/code/tensorflowsharp_integration.cs) | TensorFlow.NET |
| [watchdog_tail_latency.cs](file:///workspace/code/watchdog_tail_latency.cs) | 看门狗尾延迟 |
| [imagesharp_image_processing.cs](file:///workspace/code/imagesharp_image_processing.cs) | ImageSharp |
| [captcha_recognizer_integration.cs](file:///workspace/code/captcha_recognizer_integration.cs) | 验证码识别 |
| [asr_integration.cs](file:///workspace/code/asr_integration.cs) | 语音识别 |
| [auth_integration.cs](file:///workspace/code/auth_integration.cs) | 认证集成 |
| [audit_integration.cs](file:///workspace/code/audit_integration.cs) | 审计集成 |
| [cache_heartbeat_integration.cs](file:///workspace/code/cache_heartbeat_integration.cs) | 缓存心跳 |
| [datadog_integration.cs](file:///workspace/code/datadog_integration.cs) | Datadog |
| [distributed_audit_store.cs](file:///workspace/code/distributed_audit_store.cs) | 分布式审计存储 |
| [distributed_tracing.cs](file:///workspace/code/distributed_tracing.cs) | 分布式追踪 |
| [dnsclient_integration.cs](file:///workspace/code/dnsclient_integration.cs) | DNS 客户端 |
| [ipc_json_serializer.cs](file:///workspace/code/ipc_json_serializer.cs) | IPC JSON 序列化 |
| [SourceGenerator_ddd_advanced_generator.cs](file:///workspace/code/SourceGenerator_ddd_advanced_generator.cs) | DDD 源代码生成器 |

---

### 4.3 mvp/ - 最小可行产品示例

`mvp/` 目录包含 **35+ 个基础技术示例**，每个文件聚焦单一技术点：

| 文件 | 技术领域 | 说明 |
|------|---------|------|
| [Channels.cs](file:///workspace/mvp/Channels.cs) | 并发 | System.Threading.Channels 异步通信 |
| [Dataflow.cs](file:///workspace/mvp/Dataflow.cs) | 数据流 | TPL Dataflow 处理管道 |
| [Pipelines.cs](file:///workspace/mvp/Pipelines.cs) | IO | System.IO.Pipelines 高性能 IO |
| [Pipes.cs](file:///workspace/mvp/Pipes.cs) | IPC | 命名管道进程间通信 |
| [ObjectPool.cs](file:///workspace/mvp/ObjectPool.cs) | 内存 | 对象池模式 |
| [Threading.cs](file:///workspace/mvp/Threading.cs) | 并发 | 多线程编程模式 |
| [Polly.cs](file:///workspace/mvp/Polly.cs) | 弹性 | Polly 弹性策略 |
| [Resilience.cs](file:///workspace/mvp/Resilience.cs) | 弹性 | Microsoft.Extensions.Resilience |
| [Reflection.cs](file:///workspace/mvp/Reflection.cs) | 反射 | 反射与元数据编程 |
| [Json.cs](file:///workspace/mvp/Json.cs) | 序列化 | JSON 处理方案 |
| [JsonRPC.cs](file:///workspace/mvp/JsonRPC.cs) | RPC | JSON-RPC 远程调用 |
| [SSE.cs](file:///workspace/mvp/SSE.cs) | 实时 | Server-Sent Events |
| [Dapr.cs](file:///workspace/mvp/Dapr.cs) | 微服务 | Dapr 分布式运行时 |
| [Integration.cs](file:///workspace/mvp/Integration.cs) | 集成 | 通用集成模式 |
| [EDD.cs](file:///workspace/mvp/EDD.cs) | 架构 | 事件驱动开发 |
| [FeatureManagement.cs](file:///workspace/mvp/FeatureManagement.cs) | 特性 | 功能开关管理 |
| [Options.cs](file:///workspace/mvp/Options.cs) | 配置 | Options 选项模式 |
| [Logging.cs](file:///workspace/mvp/Logging.cs) | 日志 | 日志记录配置 |
| [Localization.cs](file:///workspace/mvp/Localization.cs) | 国际化 | 本地化/多语言 |
| [RxInteractive.cs](file:///workspace/mvp/RxInteractive.cs) | 响应式 | Reactive Extensions |
| [ServiceDiscovery.cs](file:///workspace/mvp/ServiceDiscovery.cs) | 微服务 | 服务发现 |
| [Scalar.cs](file:///workspace/mvp/Scalar.cs) | API | Scalar API 文档 |
| [htmx.cs](file:///workspace/mvp/htmx.cs) | 前端 | HTMX 交互 |
| [hello-avalonia.cs](file:///workspace/mvp/hello-avalonia.cs) | UI | Avalonia 桌面应用 |
| [Datetime.cs](file:///workspace/mvp/Datetime.cs) | 基础 | 日期时间处理 |
| [Packaging.cs](file:///workspace/mvp/Packaging.cs) | 部署 | 打包与部署 |
| [NamedQueryFilter.cs](file:///workspace/mvp/NamedQueryFilter.cs) | 数据 | 命名查询过滤器 |
| [RecyclableMemoryStream.cs](file:///workspace/mvp/RecyclableMemoryStream.cs) | 内存 | 可回收内存流 |
| [PdfTocExtractor.cs](file:///workspace/mvp/PdfTocExtractor.cs) | 文档 | PDF 目录提取 |
| [Net10Test.cs](file:///workspace/mvp/Net10Test.cs) | 测试 | .NET 10 功能测试 |
| [eshop_distributed.cs](file:///workspace/mvp/eshop_distributed.cs) | 电商 | 分布式电商示例 |
| [mcpserver_integration.cs](file:///workspace/mvp/mcpserver_integration.cs) | MCP | MCP 服务器集成 |
| [mcpserver_arch_integration.cs](file:///workspace/mvp/mcpserver_arch_integration.cs) | MCP | MCP 架构集成 |
| [ollamsSharp_integration.cs](file:///workspace/mvp/ollamsSharp_integration.cs) | AI | OllamaSharp 集成 |

---

### 4.4 plc/ - 工业物联网 (IoT/PLC)

`plc/` 目录是一个完整的 **Blazor 工业物联网应用**，支持多种 PLC 协议和 IoT 基础设施：

#### 4.4.1 PLC 协议驱动

| 文件 | 协议 | 说明 |
|------|------|------|
| [Modbus.cs](file:///workspace/plc/Modbus.cs) | Modbus TCP | Modbus 协议实现 |
| [ModbusRTU.cs](file:///workspace/plc/ModbusRTU.cs) | Modbus RTU | 串行 Modbus RTU |
| [Siemens.cs](file:///workspace/plc/Siemens.cs) | S7 | 西门子 S7 协议 |
| [Omron.cs](file:///workspace/plc/Omron.cs) | FINS | 欧姆龙 FINS 协议 |
| [Melsec.cs](file:///workspace/plc/Melsec.cs) | MELSEC | 三菱 MELSEC 协议 |
| [Keyence.cs](file:///workspace/plc/Keyence.cs) | KV | 基恩士 KV 协议 |
| [Panasonic.cs](file:///workspace/plc/Panasonic.cs) | MEWTOCOL | 松下 MEWTOCOL |
| [Fuji.cs](file:///workspace/plc/Fuji.cs) | SPH | 富士 SPH 协议 |
| [Fatek.cs](file:///workspace/plc/Fatek.cs) | Facon | 永宏 Facon 协议 |
| [Schneider.cs](file:///workspace/plc/Schneider.cs) | Modbus | 施耐德 Modbus |
| [SmartA2.cs](file:///workspace/plc/SmartA2.cs) | 自定义 | 智能设备 A2 |
| [SmartA4.cs](file:///workspace/plc/SmartA4.cs) | 自定义 | 智能设备 A4 |

#### 4.4.2 工业协议

| 文件 | 协议 | 说明 |
|------|------|------|
| [BACnet.cs](file:///workspace/plc/BACnet.cs) | BACnet | 楼宇自动化协议 |
| [OpcUa.cs](file:///workspace/plc/OpcUa.cs) | OPC UA | 统一架构 OPC |
| [OpcDa.cs](file:///workspace/plc/OpcDa.cs) | OPC DA | 经典 OPC 数据访问 |

#### 4.4.3 IoT 基础设施

| 文件 | 协议 | 说明 |
|------|------|------|
| [MQTT.cs](file:///workspace/plc/MQTT.cs) | MQTT | IoT 消息协议 |
| [Redis.cs](file:///workspace/plc/Redis.cs) | Redis | 缓存与发布订阅 |
| [RocketMQ.cs](file:///workspace/plc/RocketMQ.cs) | RocketMQ | 阿里云消息队列 |
| [LoRa.cs](file:///workspace/plc/LoRa.cs) | LoRa | 低功耗广域网 |
| [DNS.cs](file:///workspace/plc/DNS.cs) | DNS | 域名解析 |
| [NetPing.cs](file:///workspace/plc/NetPing.cs) | ICMP | 网络连通性检测 |
| [IoT.cs](file:///workspace/plc/IoT.cs) | 综合 | IoT 综合管理 |
| [AI.cs](file:///workspace/plc/AI.cs) | AI | AI 辅助分析 |
| [Audio.cs](file:///workspace/plc/Audio.cs) | 音频 | 音频处理 |
| [Map.cs](file:///workspace/plc/Map.cs) | 地图 | 地图可视化 |
| [PC.cs](file:///workspace/plc/PC.cs) | 工控机 | PC 设备管理 |

#### 4.4.4 前端页面 (Blazor)

| 文件 | 说明 |
|------|------|
| [App.razor](file:///workspace/plc/App.razor) | 应用主组件 |
| [Home.razor](file:///workspace/plc/Home.razor) | 首页 |
| [_Imports.razor](file:///workspace/plc/_Imports.razor) | 全局命名空间导入 |

---

### 4.5 csp/ - CSP 并发模型

`csp/` 目录实现了 **Go 语言风格的 CSP (Communicating Sequential Processes)** 并发模型，将 Go 的 goroutine/channel 概念移植到 C#：

| 文件 | 说明 |
|------|------|
| [GoProgram.cs](file:///workspace/csp/GoProgram.cs) | Go 风格并发程序入口 |
| [WorkerFlowProgram.cs](file:///workspace/csp/WorkerFlowProgram.cs) | 工作流并发程序 |
| [Goroutine.csproj](file:///workspace/csp/Goroutine.csproj) | 项目文件 (net10.0) |

**核心概念**：
- `chan<T>` - 类型化通道（类似 Go channel）
- `csp_chan<T>` - CSP 风格通道
- `shared_strand` - 共享串行执行上下文
- `generator` - 协程/生成器
- `async_timer` - 异步定时器
- `system_tick` - 系统时钟
- `time_heap` - 时间堆（定时器管理）
- `mutex` - 互斥锁
- `functional` - 函数式工具

---

### 4.6 ai/ - AI/ML 集成

`ai/` 目录提供 AI/ML 相关集成：

| 文件 | 说明 |
|------|------|
| [DigitalHumanDemo.cs](file:///workspace/ai/DigitalHumanDemo.cs) | 数字人演示应用 |
| [OllamaExtensions.cs](file:///workspace/ai/OllamaExtensions.cs) | Ollama API 扩展方法 |
| [Util.cs](file:///workspace/ai/Util.cs) | AI 工具类 |

---

### 4.7 SimpleCsvImporter/ - CSV 导入工具

| 文件 | 说明 |
|------|------|
| [CsvToJsonConverter.cs](file:///workspace/SimpleCsvImporter/CsvToJsonConverter.cs) | CSV 转 JSON 转换器 |
| [ExcelProcessor.cs](file:///workspace/SimpleCsvImporter/ExcelProcessor.cs) | Excel 处理 |
| [SimpleCsvImporter.cs](file:///workspace/SimpleCsvImporter/SimpleCsvImporter.cs) | 主导入逻辑 |

---

### 4.8 Models/ - 数据模型

| 文件 | 说明 |
|------|------|
| [haarcascade_eye.xml](file:///workspace/Models/haarcascade_eye.xml) | OpenCV Haar Cascade 人眼检测模型 |

---

## 5. 依赖关系全景

### 5.1 NuGet 包分类汇总

项目使用 **中央包版本管理** (`Directory.Packages.props`)，管理 **200+ 个 NuGet 包**。

#### 核心框架

| 包类别 | 主要包 | 版本 |
|--------|-------|------|
| .NET 扩展 | Microsoft.Extensions.* | 10.0.5 |
| ASP.NET Core | Microsoft.AspNetCore.* | 8.0.23~10.0.5 |
| EF Core | Microsoft.EntityFrameworkCore.* | 8.0.23 |
| 代码分析 | Microsoft.CodeAnalysis.* | 4.14.0 |
| 测试 | xunit, MSTest, coverlet | 最新稳定版 |

#### 消息与通信

| 包 | 版本 |
|----|------|
| MassTransit | 8.5.8 |
| WolverineFx | 5.22.0 |
| RabbitMQ.Client | 7.2.0 |
| MQTTnet | 5.0.1.1416 |

#### 数据与缓存

| 包 | 版本 |
|----|------|
| Dapper | 2.1.66 |
| StackExchange.Redis | 2.10.1 |
| MongoDB.Driver | 3.6.0 |
| EasyCaching | 1.9.2 |
| MySql.Data | 9.6.0 |

#### 可观测性

| 包 | 版本 |
|----|------|
| OpenTelemetry | 1.15.0 |
| Serilog | 4.3.0 |
| Application Insights | 3.0.0 |

#### 安全

| 包 | 版本 |
|----|------|
| OpenIddict | 7.4.0 |
| BouncyCastle | 2.6.2 |
| Duende.IdentityModel | 7.1.0 |

#### 序列化

| 包 | 版本 |
|----|------|
| MessagePack | 3.1.4 |
| Google.Protobuf | 3.33.4 |
| Newtonsoft.Json | 13.0.4 |

#### 微服务

| 包 | 版本 |
|----|------|
| Dapr | 1.17.5 |
| YARP | 2.3.0 |
| Refit | 9.0.2 |
| RestSharp | 113.1.0 |
| gRPC | 2.76.0 |

### 5.2 本地工具

| 工具 | 用途 |
|------|------|
| cake.tool | Cake 构建系统 |
| minver-cli | 版本号管理 |
| mapster.tool | 对象映射生成 |
| docfx | 文档生成 |
| dotnet-ef | EF Core 迁移 |
| csharpier | C# 代码格式化 |
| microsoft.tye | Tye 微服务编排 |
| messagepack.generator | MessagePack 代码生成 |
| garnet-server | Garnet 缓存服务器 |

---

## 6. 关键类与接口说明

### 6.1 核心入口点

**`program.cs`** - 主应用入口：
- 使用 `WebApplication.CreateBuilder` 创建应用
- 配置 Kestrel 服务器（HTTP/HTTPS/WebSocket/gRPC 多端点）
- 集成 Serilog 结构化日志
- 配置 CORS、认证、授权
- 集成 Swagger + Scalar API 文档
- 提供 CSV 文件上传的 HTMX 前端页面
- MySQL 数据库连接与 CRUD 操作

**关键命名空间**：
- `ModuleTools` - MCP (Model Context Protocol) 服务器工具
- `ModuleCsv` - CSV 数据模型和数据库服务

### 6.2 核心数据模型

```csharp
// HotQuestion 实体 (ModuleCsv 命名空间)
public class HotQuestion
{
    public Guid QAId { get; set; }           // 主键
    public string Question { get; set; }      // 热点问题
    public string Answer { get; set; }        // 答案
    public List<string> KeyWords { get; set; } // 关键词
    public string BusType { get; set; }       // 业务类型
    public string? Mode { get; set; }         // 人员类别
    public string Language { get; set; }      // 语言
    // ... 审计字段 (创建者、修改者、删除标记等)
}
```

### 6.3 数据库服务

```csharp
// DatabaseService (ModuleCsv 命名空间)
public class DatabaseService
{
    // 批量插入热点问题（ADO.NET 直接操作）
    Task<int> InsertHotQuestionsAsync(List<HotQuestion> questions);
    // EF Core 批量插入
    Task<int> InsertHotQuestionsEFCoreAsync(List<HotQuestion> questions);
}
```

### 6.4 MCP 工具

```csharp
// RandomNumberTools (ModuleTools 命名空间)
internal class RandomNumberTools
{
    // 生成随机数
    [McpServerTool]
    int GetRandomNumber(int min, int max);
    
    // 获取城市天气
    [McpServerTool]
    string GetCityWeather(string city);
}
```

### 6.5 插件系统

```csharp
// PluginLoadContext (根目录)
// 自定义 AssemblyLoadContext，实现插件隔离加载
public class PluginLoadContext : AssemblyLoadContext
```

### 6.6 动态代理

```csharp
// CastleDynamicProxy (根目录)
// 使用 Castle DynamicProxy 实现 AOP 日志拦截
// ILoggingService 接口 + LoggingInterceptor 拦截器
```

---

## 7. 配置与运行方式

### 7.1 环境要求

| 组件 | 版本要求 |
|------|---------|
| .NET SDK | 10.0.201+ |
| 操作系统 | Windows / Linux / macOS |
| IDE | Visual Studio 2022+ / JetBrains Rider / VS Code |
| 数据库 | MySQL 8.0+ (可选) |
| Redis | 7.0+ (可选) |

### 7.2 安装与构建

```bash
# 1. 安装 .NET SDK (如未安装)
./install.sh

# 2. 还原依赖
dotnet restore

# 3. 构建项目
dotnet build

# 4. 运行主应用
dotnet run --project program.cs

# 5. 运行 Todo 示例
dotnet run --project Todo.cs

# 6. 运行 PLC IoT 应用
dotnet run --project plc/Feature.csproj

# 7. 运行 CSP 并发示例
dotnet run --project csp/Goroutine.csproj
```

### 7.3 配置文件说明

| 文件 | 用途 |
|------|------|
| [appsettings.json](file:///workspace/appsettings.json) | 应用配置（数据库连接、JWT、Redis、代理等） |
| [global.json](file:///workspace/global.json) | .NET SDK 版本控制 |
| [NuGet.Config](file:///workspace/NuGet.Config) | NuGet 包源配置 |
| [Directory.Build.props](file:///workspace/Directory.Build.props) | MSBuild 全局属性 |
| [Directory.Packages.props](file:///workspace/Directory.Packages.props) | 中央包版本管理 |
| [Directory.Build.targets](file:///workspace/Directory.Build.targets) | MSBuild 全局目标 |
| [dotnet-tools.json](file:///workspace/dotnet-tools.json) | 本地开发工具清单 |
| [GlobalAnalyzerConfig.globalconfig](file:///workspace/GlobalAnalyzerConfig.globalconfig) | Roslyn 分析器配置 |
| [stylecop.json](file:///workspace/stylecop.json) | StyleCop 代码规范 |
| [.editorconfig](file:///workspace/.editorconfig) | 编辑器代码风格 |
| [dotnet.ruleset](file:///workspace/dotnet.ruleset) | 代码分析规则集 |

### 7.4 启动端点

| 端口 | 协议 | 用途 |
|------|------|------|
| 5000 | HTTP | Web 应用主端口 |
| 5001 | HTTPS | 安全 Web 端口 |
| 5002 | HTTP | WebSocket 端点 |
| 5003 | HTTPS | 安全 WebSocket |
| 5004 | HTTP | gRPC 服务 |
| 5005 | HTTPS | 安全 gRPC |

### 7.5 API 文档访问

- Swagger UI: `http://localhost:5000/swagger`
- Scalar UI: `http://localhost:5000/scalar`
- OpenAPI JSON: `http://localhost:5000/openapi/v1.json`
- 注册服务列表: `http://localhost:5000/services`

### 7.6 发布

```bash
# AOT 发布
dotnet publish -p:PublishAot=true -r linux-x64

# 单文件发布
dotnet publish -p:PublishSingleFile=true -r win-x64

# Docker 构建
docker build -t vsa-app .
```

---

## 8. 技术领域分类索引

### 8.1 按技术栈索引

| 技术领域 | 核心文件 |
|---------|---------|
| **ASP.NET Core** | program.cs, Todo.cs, fastendpoints.cs, miniapi_impl.cs, carter_integration.cs |
| **Blazor** | blazor_integration.cs, plc/*.razor, known_blazor_integration.cs |
| **MAUI** | silky_maui_integration.cs |
| **Avalonia** | avalonia_integration.cs, hello-avalonia.cs |
| **SignalR** | signalr_production.cs, signalr_integration.cs, signalr_advanced.cs |
| **gRPC** | grpc_integration.cs, grpc_protobufnet.cs |
| **REST API** | refit_*.cs, restsharp_*.cs, webapiclientcore_*.cs |
| **GraphQL** | hotchocolate_*.cs, boxed_graphql.cs |
| **EF Core** | efcore9_integration.cs, efcore_dapper_integration.cs |
| **Dapper** | dapper_*.cs, efcore_dapper_integration.cs |
| **LiteDB** | litedb_*.cs |
| **MongoDB** | mongodb_*.cs |
| **Redis** | stackexchange_redis_cache.cs, garnet_redis_cache.cs, redis_*.cs |
| **Kafka** | librdkafka_integration.cs, mediatr_kafka_integration.cs |
| **RabbitMQ** | rabbitmq_production_integration.cs |
| **MQTT** | mqtt_*.cs, mqttnet*.cs |
| **MassTransit** | masstransit_*.cs |
| **Wolverine** | wolverine_*.cs, wolverinefx_*.cs |
| **Dapr** | dapr_integration.cs, mediatr_dapr_integration.cs |
| **OpenTelemetry** | opentelemetry_*.cs |
| **Serilog** | serilog_integration.cs |
| **Docker** | docker_*.cs, docker_compose_*.cs |
| **Kubernetes** | k8s_integration.cs, k3s_integration.cs |
| **YARP** | yarp_*.cs |
| **PLC/IoT** | plc/*.cs |
| **AI/ML** | ai/*.cs, SemanticKernel.cs, tensorflowsharp_integration.cs |
| **安全** | abac_*.cs, auth_*.cs, openiddict_*.cs, keycloak_*.cs |
| **测试** | xunit_service_testing.cs, benchmark_demo.cs |

### 8.2 按架构模式索引

| 架构模式 | 核心文件 |
|---------|---------|
| **Vertical Slice** | vertical_slice_architecture.cs |
| **CQRS** | cqrs_impl.cs, cqrs_processor.cs, wolverine_cqrs.cs |
| **Event Sourcing** | eventsourcing_service.cs, event_sourcing_processor.cs, litedb_event_sourcing.cs |
| **DDD** | SourceGenerator_ddd_*.cs |
| **Saga** | saga_orchestrator.cs, wolverine_saga*.cs |
| **State Machine** | state_machine_flow.cs, liquidstate_*.cs |
| **Specification** | specification_pattern.cs, specification_repository.cs |
| **Unit of Work** | unitofwork_integration.cs |
| **Mediator** | mediatr_*.cs |
| **Outbox** | wolverinefx_local_message_table_integration.cs |
| **TCC** | mediatr_tcc_integration.cs, wolverinefx_tcc_integration.cs |
| **XA** | mediatr_xa_integration.cs, wolverinefx_xa_integration.cs |

### 8.3 按性能优化索引

| 优化方向 | 核心文件 |
|---------|---------|
| **零拷贝** | span_zero_copy_serializer.cs, zero_copy_*.cs |
| **Disruptor** | disruptor_*.cs |
| **高频处理** | high_frequency_*.cs |
| **尾延迟** | tail_latency_optimizer.cs, watchdog_tail_latency.cs |
| **LLVM** | llvm_ir_optimizer.cs |
| **内存池** | threadsafe_memorypool_*.cs, ObjectPool.cs |
| **分层内存** | tiered_memory_*.cs, litedb_tieredmemory.cs |
| **AOT** | litedb_aot.cs, aot_reflection.cs |
| **压缩** | brotli_compression_integration.cs, lz4_integration.cs, zstd_compression_integration.cs |

---

> **文档版本**: 1.0  
> **生成时间**: 2026-07-24  
> **项目仓库**: VSA (Vertical Slice Architecture)  
> **技术栈**: .NET 10 / C# 14 / ASP.NET Core