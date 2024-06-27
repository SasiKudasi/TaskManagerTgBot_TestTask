# Используем базовый образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем и восстанавливаем зависимости
COPY *.csproj .
RUN dotnet restore

# Копируем остальные файлы и собираем проект
COPY . .
RUN dotnet publish -c Release -o out

# Используем базовый образ для выполнения
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out .

# Запускаем приложение
ENTRYPOINT ["dotnet", "TaskManagerTgBot.dll"]
