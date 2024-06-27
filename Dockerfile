# Используем базовый образ для выполнения
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Используем базовый образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TaskManagerTgBot.csproj", "."]
RUN dotnet restore "./TaskManagerTgBot.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./TaskManagerTgBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Сборка и публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskManagerTgBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerTgBot.dll"]
