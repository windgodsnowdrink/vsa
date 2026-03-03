# 在 Ubuntu 22.04 上运行 .NET 8 应用程序的解决方案

## 问题分析
根据错误信息，您的 .NET 8 应用程序无法在 Ubuntu 22.04 服务器上运行，因为服务器上只安装了 .NET 10 运行时环境，而应用程序需要 .NET 8.0.0 运行时。

## 解决方案
您可以选择以下两种解决方案之一：

### 方案 1：在服务器上安装 .NET 8 运行时
这是最简单直接的解决方案，只需在服务器上安装 .NET 8 运行时即可。

#### 步骤 1：添加 Microsoft 包源
```bash
# 下载并安装 Microsoft 包源
sudo wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

#### 步骤 2：安装 .NET 8 运行时
```bash
# 更新包列表
sudo apt-get update

# 安装 ASP.NET Core 8.0 运行时（如果您的应用是 Web 应用）
sudo apt-get install -y aspnetcore-runtime-8.0

# 或者安装 .NET Core 8.0 运行时（如果您的应用是控制台应用）
# sudo apt-get install -y dotnet-runtime-8.0
```

#### 步骤 3：验证安装
```bash
# 检查已安装的 .NET 版本
dotnet --list-runtimes
```

#### 步骤 4：重新运行应用程序
```bash
# 导航到应用程序目录
cd /usr/local/etc/maxvision/wh_dev/03_aiot_service/

# 运行应用程序
dotnet AiotService.Host.dll
```

### 方案 2：将应用程序发布为自包含应用
如果您无法在服务器上安装 .NET 8 运行时，可以将应用程序发布为自包含应用，这样应用程序将包含自己的 .NET 运行时。

#### 步骤 1：在开发环境中发布自包含应用
```bash
# 在项目目录中运行以下命令
dotnet publish -c Release -r linux-x64 --self-contained true
```

#### 步骤 2：将发布的文件复制到服务器
```bash
# 使用 scp 或其他工具将发布的文件复制到服务器
scp -r bin/Release/net8.0/linux-x64/publish/* user@server:/usr/local/etc/maxvision/wh_dev/03_aiot_service/
```

#### 步骤 3：在服务器上运行应用程序
```bash
# 导航到应用程序目录
cd /usr/local/etc/maxvision/wh_dev/03_aiot_service/

# 给可执行文件添加执行权限
chmod +x AiotService.Host

# 运行应用程序
./AiotService.Host
```

### 方案 3：将应用程序升级到 .NET 10
如果您希望长期使用服务器上的 .NET 10 环境，可以考虑将应用程序升级到 .NET 10。

#### 步骤 1：更新项目文件
将项目文件（.csproj）中的 TargetFramework 从 net8.0 更改为 net10.0：
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 步骤 2：更新依赖项
更新所有 NuGet 包到兼容 .NET 10 的版本：
```bash
dotnet restore
dotnet build -c Release
```

#### 步骤 3：测试应用程序
在开发环境中测试应用程序，确保没有兼容性问题。

#### 步骤 4：发布并部署到服务器
```bash
dotnet publish -c Release
scp -r bin/Release/net10.0/publish/* user@server:/usr/local/etc/maxvision/wh_dev/03_aiot_service/
```

## 推荐解决方案
**方案 1** 是最推荐的解决方案，因为：
1. 安装 .NET 8 运行时简单快捷
2. 不影响服务器上现有的 .NET 10 环境
3. 不需要修改应用程序代码
4. 维护成本最低

## 注意事项
1. 确保您有服务器的 sudo 权限
2. 安装过程中可能需要输入密码
3. 如果遇到网络问题，可以尝试使用国内镜像源
4. 安装完成后，建议重启应用程序以确保使用正确的运行时

## 故障排除
如果安装过程中遇到问题，可以尝试以下命令：

```bash
# 清除包缓存
sudo apt-get clean
sudo apt-get autoclean

# 重新更新包列表
sudo apt-get update

# 修复损坏的包
sudo apt-get -f install
```

如果问题仍然存在，请提供详细的错误信息，以便进一步排查。