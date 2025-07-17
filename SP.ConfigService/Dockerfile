# 使用官方.NET 8.0 SDK镜像作为构建阶段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件并恢复依赖
COPY ["SP.ConfigService.csproj", "./"]
COPY ["../SP.Common/SP.Common.csproj", "../SP.Common/"]
RUN dotnet restore "SP.ConfigService.csproj"

# 复制所有源代码
COPY . .
COPY ["../SP.Common/", "../SP.Common/"]

# 构建应用程序
RUN dotnet build "SP.ConfigService.csproj" -c Release -o /app/build

# 发布应用程序
FROM build AS publish
RUN dotnet publish "SP.ConfigService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 使用官方.NET 8.0运行时镜像作为最终阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 安装必要的包（如果需要）
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# 复制发布的应用程序
COPY --from=publish /app/publish .

# 设置环境变量
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# 暴露端口
EXPOSE 80

# 启动应用程序
ENTRYPOINT ["dotnet", "SP.ConfigService.dll"] 