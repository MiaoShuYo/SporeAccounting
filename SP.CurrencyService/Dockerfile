# 使用官方.NET 8.0 SDK镜像作为构建阶段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件
COPY ["SP.CurrencyService/SP.CurrencyService.csproj", "SP.CurrencyService/"]
COPY ["SP.Common/SP.Common.csproj", "SP.Common/"]

# 还原NuGet包
RUN dotnet restore "SP.CurrencyService/SP.CurrencyService.csproj"

# 复制所有源代码
COPY . .

# 构建应用程序
WORKDIR "/src/SP.CurrencyService"
RUN dotnet build "SP.CurrencyService.csproj" -c Release -o /app/build

# 发布应用程序
FROM build AS publish
RUN dotnet publish "SP.CurrencyService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 使用官方.NET 8.0运行时镜像作为最终阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 安装必要的系统包（如果需要）
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# 创建非root用户
RUN groupadd -r appuser && useradd -r -g appuser appuser

# 复制发布的应用程序
COPY --from=publish /app/publish .

# 设置文件权限
RUN chown -R appuser:appuser /app
USER appuser

# 暴露端口
EXPOSE 80
EXPOSE 443

# 设置环境变量
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# 健康检查
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# 启动应用程序
ENTRYPOINT ["dotnet", "SP.CurrencyService.dll"] 