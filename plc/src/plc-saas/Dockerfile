# syntax=docker/dockerfile:1
# PLC·AIOT 多租户 SaaS — 多阶段构建
# 阶段 1：SDK 11 preview.7 构建 + publish（#include 剥离由 csproj 的 PrepareStrippedApp 调用 strip-app.sh 完成）
# 阶段 2：aspnet 11 preview.7 运行时，仅含发布产物 + wwwroot 静态资源
#
# 说明：连接串 / Jwt:Key / 桥接开关等运行时配置经环境变量注入（ConnectionStrings__Default、Jwt__Key、
#       Mqtt__Bridge__Enabled、Mqtt__BrokerUrl），镜像内不内置任何凭据。
FROM mcr.microsoft.com/dotnet/sdk:11.0.100-preview.7 AS build
WORKDIR /src
# 仅复制 SaaS 工程（自包含：infra/seed/slices/wwwroot + strip-app.sh）
COPY src/plc-saas/ ./plc-saas/
RUN dotnet restore plc-saas/plc-saas.csproj
RUN dotnet publish plc-saas/plc-saas.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:11.0.100-preview.7 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "plc-saas.dll"]
