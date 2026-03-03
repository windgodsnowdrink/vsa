# IP2Region 技能使用示例

## 快速开始

### 1. 基本查询示例

#### 1.1 简单 IP 地址查询

```bash
# 查询单个 IP 地址的地理位置
ip2region_aot search 1.1.1.1

# 使用命令别名
ip2region_aot s 8.8.8.8
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
查询IP: 1.1.1.1
查询结果: 澳大利亚|0|0|0|Cloudflare
查询用时: 12.345 ms
```

#### 1.2 详细信息查询

```bash
# 查询 IP 地址的详细地理位置信息
ip2region_aot info 114.114.114.114

# 使用命令别名
ip2region_aot i 202.108.22.5
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
查询IP (详细信息): 114.114.114.114
国家: 中国
区域: 0
省份: 江苏
城市: 南京
ISP: 南京信风网络科技有限公司
查询用时: 5.678 ms
```

### 2. 批量查询示例

#### 2.1 基本批量查询

```bash
# 创建包含 IP 地址的文件 ips.txt
# 文件内容示例：
# 1.1.1.1
# 8.8.8.8
# 114.114.114.114
# 202.108.22.5

# 运行批量查询
ip2region_aot batch ips.txt

# 使用命令别名
ip2region_aot b ips.txt
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
批量查询文件: ips.txt
批量查询完成!
总IP数: 4
查询用时: 0.34 s
平均用时: 85.000 ms/IP
```

#### 2.2 批量查询并保存结果

```bash
# 批量查询并保存结果到文件
ip2region_aot batch ips.txt save

# 使用命令别名
ip2region_aot b ips.txt save
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
批量查询文件: ips.txt
批量查询完成!
总IP数: 4
查询用时: 0.45 s
平均用时: 112.500 ms/IP
结果已保存到: ips.result.txt
```

**结果文件内容示例**：
```
1.1.1.1: 澳大利亚|0|0|0|Cloudflare
8.8.8.8: 美国|0|0|0|Google
114.114.114.114: 中国|0|江苏|南京|南京信风网络科技有限公司
202.108.22.5: 中国|0|北京|北京|中国电信
```

### 3. 配置管理示例

#### 3.1 查看当前配置

```bash
# 显示当前配置
ip2region_aot config

# 使用命令别名
ip2region_aot co
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
IP2Region 配置:
数据库路径: ip2region.db
缓存类型: memory
缓存过期时间: 30 分钟
日志记录: True
基准测试: True
批处理大小: 1000
并发线程数: 4
```

#### 3.2 通过环境变量配置

```powershell
# 设置环境变量覆盖默认配置
$env:IP2REGION_DB_PATH = "D:\data\ip2region.db"
$env:IP2REGION_CACHE_TYPE = "memory"
$env:IP2REGION_CACHE_EXPIRATION = "60"
$env:IP2REGION_BATCH_SIZE = "2000"
$env:IP2REGION_CONCURRENT_THREADS = "8"

# 查看更新后的配置
ip2region_aot config
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
IP2Region 配置:
数据库路径: D:\data\ip2region.db
缓存类型: memory
缓存过期时间: 60 分钟
日志记录: True
基准测试: True
批处理大小: 2000
并发线程数: 8
```

### 4. 性能基准测试示例

#### 4.1 运行基准测试

```bash
# 运行 1000 次迭代的基准测试
ip2region_aot benchmark 1000

# 使用命令别名
ip2region_aot bm 5000
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
运行基准测试，迭代次数: 1000
基准测试完成!
平均查询时间: 0.123 ms
每秒操作数: 8130.08 ops/s
缓存命中率: 95.00%
```

#### 4.2 比较不同配置的性能

```bash
# 测试默认配置性能
ip2region_aot benchmark 1000

# 修改缓存配置后测试
$env:IP2REGION_CACHE_TYPE = "none"
ip2region_aot benchmark 1000

# 恢复默认配置
$env:IP2REGION_CACHE_TYPE = "memory"
```

**预期输出**：
```
# 使用缓存
平均查询时间: 0.123 ms
每秒操作数: 8130.08 ops/s
缓存命中率: 95.00%

# 不使用缓存
平均查询时间: 5.678 ms
每秒操作数: 176.12 ops/s
缓存命中率: 0.00%
```

## 高级使用示例

### 1. 批量查询性能优化

#### 1.1 调整并发线程数

```powershell
# 设置更高的并发线程数
$env:IP2REGION_CONCURRENT_THREADS = "16"

# 运行批量查询
ip2region_aot batch large_ips.txt save
```

**预期输出**：
```
IP2Region AOT 引擎
============================================================
批量查询文件: large_ips.txt
批量查询完成!
总IP数: 1000
查询用时: 15.67 s
平均用时: 15.670 ms/IP
结果已保存到: large_ips.result.txt
```

#### 1.2 调整批处理大小

```powershell
# 设置更大的批处理大小
$env:IP2REGION_BATCH_SIZE = "5000"
$env:IP2REGION_CONCURRENT_THREADS = "8"

# 运行批量查询
ip2region_aot batch very_large_ips.txt save
```

### 2. 集成到脚本中

#### 2.1 PowerShell 脚本示例

```powershell
# IP 查询脚本
param(
    [string]$IpAddress
)

if (-not $IpAddress) {
    Write-Host "请提供 IP 地址"
    exit 1
}

# 调用 IP2Region 进行查询
$result = & "ip2region_aot.exe" search $IpAddress

# 解析结果
Write-Host "IP 查询结果:"
Write-Host $result

# 示例输出:
# IP2Region AOT 引擎
# ============================================================
# 查询IP: 1.1.1.1
# 查询结果: 澳大利亚|0|0|0|Cloudflare
# 查询用时: 12.345 ms
```

#### 2.2 批处理脚本示例

```batch
@echo off

rem IP 批量查询脚本
if "%1"==