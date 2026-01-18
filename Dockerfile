# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution-wide config files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]

# Copy csproj files and restore dependencies
COPY ["src/Overclocked.Api/Overclocked.Api.csproj", "src/Overclocked.Api/"]
COPY ["src/Overclocked.Application/Overclocked.Application.csproj", "src/Overclocked.Application/"]
COPY ["src/Overclocked.Domain/Overclocked.Domain.csproj", "src/Overclocked.Domain/"]
COPY ["src/Overclocked.Infrastructure/Overclocked.Infrastructure.csproj", "src/Overclocked.Infrastructure/"]
COPY ["src/Overclocked.SharedKernel/Overclocked.SharedKernel.csproj", "src/Overclocked.SharedKernel/"]

RUN dotnet restore "src/Overclocked.Api/Overclocked.Api.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/app/src/Overclocked.Api"
RUN dotnet build "Overclocked.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Overclocked.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Overclocked.Api.dll"]
