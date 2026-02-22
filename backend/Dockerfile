FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ExpenseTracker.sln", "./"]
COPY ["src/ExpenseTracker.Domain/ExpenseTracker.Domain.csproj", "src/ExpenseTracker.Domain/"]
COPY ["src/ExpenseTracker.Application/ExpenseTracker.Application.csproj", "src/ExpenseTracker.Application/"]
COPY ["src/ExpenseTracker.Infrastructure/ExpenseTracker.Infrastructure.csproj", "src/ExpenseTracker.Infrastructure/"]
COPY ["src/ExpenseTracker.API/ExpenseTracker.API.csproj", "src/ExpenseTracker.API/"]

# Restore dependencies
RUN dotnet restore "ExpenseTracker.sln"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/ExpenseTracker.API"
RUN dotnet build "ExpenseTracker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseTracker.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]
