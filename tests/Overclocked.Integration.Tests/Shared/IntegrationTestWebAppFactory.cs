using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using NSubstitute;
using Overclocked.Api;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Respawn;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: AssemblyFixture(typeof(IntegrationTestWebAppFactory))]

namespace Overclocked.Integration.Tests.Shared;

public class IntegrationTestWebAppFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";
    private const string SigningKey = "6a5080d2a0d8faea6fb1bfabba8919b15ede19a05f785a2552f8bbc04a0ff9beec356bcd";

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("OverclockedDBTest")
        .WithUsername("admin")
        .WithPassword("admin-pw")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:latest").Build();

    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;
    private IConnectionMultiplexer _redisConnection = null!;
    public HttpClient HttpClient { get; private set; } = null!;
    public IFileStorageService FileStorageServiceMock { get; } = Substitute.For<IFileStorageService>();
    public IBackgroundJobClient BackgroundJobClientMock { get; } = Substitute.For<IBackgroundJobClient>();
    public IDomainEventDispatcher DomainEventDispatcherMock { get; } = Substitute.For<IDomainEventDispatcher>();
    public IProcessOutboxMessagesJob ProcessOutboxMessagesJobMock { get; } = Substitute.For<IProcessOutboxMessagesJob>();

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        var redisConnectionString = _redisContainer.GetConnectionString() + ",allowAdmin=true";
        _redisConnection = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);

        HttpClient = CreateClient();
        await ApplyMigrationsAsync();
        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        // This helps see logs in the GitHub Actions output console
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });

        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString() },
                        { "ConnectionStrings:Redis", _redisContainer.GetConnectionString() },
                        { "JwtSettings:SigningKey", SigningKey },
                        { "JwtSettings:Issuer", Issuer },
                        { "JwtSettings:Audience", Audience },
                        { "JwtSettings:ExpiresInMinutes", "30" },
                        { "RateLimiting:Enabled", "false" },
                    }
                );
            }
        );

        builder.ConfigureServices(services =>
        {
            services.Configure<RedisCacheOptions>(options =>
            {
                options.Configuration = _redisContainer.GetConnectionString();
                options.InstanceName = "OverclockedTestInstance:";
            });

            services.RemoveAll<IConnectionMultiplexer>();
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString())
            );

            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(
                        _dbContainer.GetConnectionString(),
                        npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        }
                    )
                    .UseSnakeCaseNamingConvention()
            );

            services.RemoveAll<IFileStorageService>();
            services.AddScoped(_ => FileStorageServiceMock);

            services.RemoveAll<IBackgroundJobClient>();
            services.AddScoped(_ => BackgroundJobClientMock);

            services.RemoveAll<IRecurringJobManager>();
            services.AddSingleton(_ => Substitute.For<IRecurringJobManager>());

            services.RemoveAll<IDomainEventDispatcher>();
            services.AddScoped(_ => DomainEventDispatcherMock);

            services.RemoveAll<IProcessOutboxMessagesJob>();
            services.AddScoped(_ => ProcessOutboxMessagesJobMock);

            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Issuer,
                        ValidAudience = Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
                    };
                }
            );
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);

        EndPoint[] endpoints = _redisConnection.GetEndPoints();
        IServer server = _redisConnection.GetServer(endpoints[0]);

        // Wipe ALL databases (0-15)
        await server.FlushAllDatabasesAsync();
    }

    private async Task InitializeRespawnerAsync() =>
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                SchemasToInclude = ["public"],
                DbAdapter = DbAdapter.Postgres,
                WithReseed = true,
                TablesToIgnore =
                [
                    new Respawn.Graph.Table("__EFMigrationsHistory"),
                    new Respawn.Graph.Table("roles"),
                    new Respawn.Graph.Table("role_permissions"),
                    new Respawn.Graph.Table("permissions"),
                ],
            }
        );

    private async Task ApplyMigrationsAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public string GenerateJwtToken(
        string? userId = null,
        string role = "Customer",
        string? deviceId = null,
        string email = "test@temp.com",
        IList<string>? permissions = null
    )
    {
        var claims = new List<Claim>
        {
            new(ClaimsConstants.NameIdentifier, userId ?? Guid.NewGuid().ToString()),
            new(ClaimsConstants.Email, email),
            new(ClaimsConstants.DeviceId, deviceId ?? Guid.NewGuid().ToString()),
            new(ClaimsConstants.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (permissions is not null)
        {
            foreach (var permission in permissions)
                claims.Add(new(ClaimsConstants.Permission, permission));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials,
            Issuer = Issuer,
            Audience = Audience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}
