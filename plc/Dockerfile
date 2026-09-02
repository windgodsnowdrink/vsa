# =========================================================
# plc-host — PLC·AIOT WebApi 宿主（.NET 11 Preview）
# 内含 plc-gateway 边缘网关（NamedPipe: plc-gateway），为 /api/gateway/* 提供后端。
# 同源托管根 SPA 到 /console/，前端以相对路径调用 /api/*，无需 CORS / 反向代理。
# 静态资源来源：仓库根 wwwroot/（→ 输出 wwwroot/console）+ PlcAiot.Web/wwwroot/pages（→ 输出 wwwroot/pages）。
#
# 构建（在仓库根执行）：
#   docker build -t plc-aiot-host .
# 运行：
#   docker run -d -p 8080:8080 plc-aiot-host
#
# 说明：原根 Dockerfile 针对旧 PlcAiot.WebApi（.NET 10），本文件已改作当前 WebApi（plc-host）镜像。
# 生产建议：将 plc-gateway 拆为 sidecar 容器；注意 Linux 下 .NET NamedPipe 映射为 Unix 套接字，
#          跨容器需共享该套接字挂载点（同容器内运行则无此问题）。
# 受限网络：可用 --build-arg DOTNET_REGISTRY=<mirror> 覆盖基础镜像源。
# =========================================================

ARG DOTNET_REGISTRY=mcr.microsoft.com
ARG DOTNET_VERSION=11.0-preview

# ---------- 1. 构建阶段 ----------
FROM ${DOTNET_REGISTRY}/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# 还原 plc-host（含 Plc.Plugins.Contracts 项目引用）
COPY src/plc-host/plc-host.csproj src/plc-host/
COPY src/Plc.Plugins.Contracts/Plc.Plugins.Contracts.csproj src/Plc.Plugins.Contracts/
RUN dotnet restore src/plc-host/plc-host.csproj

# 还原 plc-gateway（边缘网关，宿主经 NamedPipe 调它）
COPY src/plc-gateway/PlcGateway.csproj src/plc-gateway/
RUN dotnet restore src/plc-gateway/PlcGateway.csproj

# 复制源码与外部前端资源
# csproj 以相对路径引用：../PlcAiot.Web/wwwroot/pages 与 ../../wwwroot
COPY src/plc-host/                src/plc-host/
COPY src/Plc.Plugins.Contracts/    src/Plc.Plugins.Contracts/
COPY src/plc-gateway/              src/plc-gateway/
COPY src/PlcAiot.Web/wwwroot/pages/ src/PlcAiot.Web/wwwroot/pages/
COPY wwwroot/                      wwwroot/

# 发布宿主（触发 /console/ 静态资源复制 + 插件 None 引用）
RUN dotnet publish src/plc-host/plc-host.csproj -c Release -o /app/host --no-restore
# 发布边缘网关
RUN dotnet publish src/plc-gateway/PlcGateway.csproj -c Release -o /app/gateway --no-restore

# ---------- 2. 运行阶段 ----------
FROM ${DOTNET_REGISTRY}/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# 非 root 用户运行（安全基线）
RUN groupadd -r app && useradd -r -g app app

COPY --from=build /app/host    /app/host
COPY --from=build /app/gateway /app/gateway
COPY docker-entrypoint.sh      /app/docker-entrypoint.sh
RUN chmod +x /app/docker-entrypoint.sh && chown -R app:app /app

USER app

ENV DOTNET_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["/app/docker-entrypoint.sh"]
