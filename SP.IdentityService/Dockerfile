# 使用官方.NET 8.0运行时镜像作为基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 使用官方.NET 8.0 SDK镜像作为构建镜像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件
COPY ["SP.IdentityService/SP.IdentityService.csproj", "SP.IdentityService/"]
COPY ["SP.Common/SP.Common.csproj", "SP.Common/"]

# 还原NuGet包
RUN dotnet restore "SP.IdentityService/SP.IdentityService.csproj"

# 复制所有源代码
COPY . .

# 构建应用程序
WORKDIR "/src/SP.IdentityService"
RUN dotnet build "SP.IdentityService.csproj" -c Release -o /app/build

# 发布应用程序
FROM build AS publish
RUN dotnet publish "SP.IdentityService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 最终镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 设置入口点
ENTRYPOINT ["dotnet", "SP.IdentityService.dll"] 