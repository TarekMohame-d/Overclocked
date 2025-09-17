FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# Install Docker CLI & dependencies
RUN apt-get update && apt-get install -y \
    docker.io \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Install dotnet-ef CLI globally
RUN dotnet tool install -g dotnet-ef

# Ensure tools path is available
ENV PATH="$PATH:/root/.dotnet/tools"
