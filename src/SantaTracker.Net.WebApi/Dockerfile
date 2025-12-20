# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first (better caching)
COPY SantaTracker.Net.WebApi/SantaTracker.Net.WebApi.csproj SantaTracker.Net.WebApi/
COPY SantaTracker.Net.Application/SantaTracker.Net.Application.csproj SantaTracker.Net.Application/
COPY SantaTracker.Net.Contracts/SantaTracker.Net.Contracts.csproj SantaTracker.Net.Contracts/

# Restore
RUN dotnet restore SantaTracker.Net.WebApi/SantaTracker.Net.WebApi.csproj

# Copy everything else
COPY . .

# Build
RUN dotnet build SantaTracker.Net.WebApi/SantaTracker.Net.WebApi.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish SantaTracker.Net.WebApi/SantaTracker.Net.WebApi.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SantaTracker.Net.WebApi.dll"]
