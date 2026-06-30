# ================= BUILD =================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TaskTracker.WebApi/TaskTracker.WebApi.csproj", "TaskTracker.WebApi/"]
COPY ["TaskTracker.Application/TaskTracker.Application.csproj", "TaskTracker.Application/"]
COPY ["TaskTracker.Domain/TaskTracker.Domain.csproj", "TaskTracker.Domain/"]
COPY ["TaskTracker.Infrastructure/TaskTracker.Infrastructure.csproj", "TaskTracker.Infrastructure/"]

RUN dotnet restore "TaskTracker.WebApi/TaskTracker.WebApi.csproj"

COPY . .

WORKDIR "/src/TaskTracker.WebApi"
RUN dotnet publish "TaskTracker.WebApi.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ================= RUNTIME =================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/wwwroot/documents

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskTracker.WebApi.dll"]