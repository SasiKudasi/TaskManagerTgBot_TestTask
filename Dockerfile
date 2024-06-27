FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY TaskManagerTgBot.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o TaskManagerTgBot

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/TaskManagerTgBot ./
ENTRYPOINT ["dotnet", "TaskManagerTgBot.dll"]