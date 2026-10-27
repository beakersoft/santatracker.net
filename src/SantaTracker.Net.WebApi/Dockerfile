# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution-wide and csproj files first (caching)
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY global.json ./
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
ARG BUILD_NUMBER=local
ARG BUILD_SHA=unknown
ARG BUILD_TIMESTAMP=unknown
ENV Build__Number=${BUILD_NUMBER} \
    Build__Commit=${BUILD_SHA} \
    Build__Timestamp=${BUILD_TIMESTAMP}
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SantaTracker.Net.WebApi.dll"]
