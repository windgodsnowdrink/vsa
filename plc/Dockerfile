# =========================================================
# PlcAiot.WebApi — 多阶段容器化构建
# 技术基线：.NET 10 LTS (ASP.NET Core Runtime)
# 用法：
#   docker build -t plc-aiot-webapi .
#   docker run -d -p 8080:8080 \
#     -e StaticWebRoot=/app/wwwroot \
#     plc-aiot-webapi
# =========================================================

# ---------- 1. 构建阶段 ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 还原依赖（利用层缓存）
COPY src/PlcAiot.WebApi/PlcAiot.WebApi.csproj ./
RUN dotnet restore PlcAiot.WebApi.csproj

# 复制源码与前端 + 种子数据
COPY src/PlcAiot.WebApi/ ./
# 前端静态资源（仓库根 wwwroot）一并带入镜像
COPY wwwroot/ ./wwwroot/

# 发布
RUN dotnet publish PlcAiot.WebApi.csproj -c Release -o /app --no-restore

# ---------- 2. 运行阶段 ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# 非 root 用户运行（安全基线）
RUN groupadd -r app && useradd -r -g app app
COPY --from=build /app ./
RUN chown -R app:app /app
USER app

ENV StaticWebRoot=/app/wwwroot \
    DOTNET_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "PlcAiot.WebApi.dll"]
