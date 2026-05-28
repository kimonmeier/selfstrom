# syntax=docker/dockerfile:1.7

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src

# Install Node.js (needed for Tailwind CSS build pulled in by the Server project)
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y --no-install-recommends nodejs \
    && rm -rf /var/lib/apt/lists/*

# Copy solution & csproj files for layer-cached restore
COPY global.json ./
COPY SelfStrom.sln ./
COPY SelfStrom.Server/SelfStrom.Server.csproj      SelfStrom.Server/
COPY SelfStrom.Client/SelfStrom.Client.csproj      SelfStrom.Client/
COPY SelfStrom.Shared/SelfStrom.Shared.csproj      SelfStrom.Shared/
COPY SelfStrom.Worker/SelfStrom.Worker.csproj      SelfStrom.Worker/
COPY MyStrom.Api/MyStrom.Api.csproj                MyStrom.Api/

# npm restore for Tailwind
COPY SelfStrom.Server/package.json      SelfStrom.Server/
COPY SelfStrom.Server/package-lock.json SelfStrom.Server/
RUN cd SelfStrom.Server && npm ci

RUN dotnet restore SelfStrom.Server/SelfStrom.Server.csproj

# Copy the rest of the sources
COPY . .

RUN dotnet workload install wasm-tools

# Publish
RUN dotnet publish SelfStrom.Server/SelfStrom.Server.csproj \
      -c Release \
      -o /app/publish

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SelfStrom.Server.dll"]