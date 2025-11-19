FROM mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm

# Install Docker CLI & dependencies
RUN apt-get update && apt-get install -y \
    docker.io \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Clear NuGet caches to ensure a clean install
RUN dotnet nuget locals all --clear

# Install dotnet-ef CLI globally
RUN dotnet tool install -g dotnet-ef --version 9.0.9-*

# Ensure tools path is available
ENV PATH="$PATH:/root/.dotnet/tools"
