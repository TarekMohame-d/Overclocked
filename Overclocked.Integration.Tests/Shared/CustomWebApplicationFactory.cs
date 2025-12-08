using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using NSubstitute;
using Overclocked.Api;
using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Respawn;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Overclocked.Integration.Tests.Shared;

public class CustomWebApplicationFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";
    private const string SigningKey = "6a5080d2a0d8faea6fb1bfabba8919b15ede19a05f785a2552f8bbc04a0ff9beec356bcd";

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("OverclockedDBTest")
        .WithUsername("admin")
        .WithPassword("admin-pw")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder().WithImage("redis:latest").Build();

    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;
    private IConnectionMultiplexer _redisConnection = null!;
    public HttpClient HttpClient { get; private set; } = null!;
    public IFileStorageService FileStorageServiceMock { get; } = Substitute.For<IFileStorageService>();
    public IBackgroundJobClient BackgroundJobClientMock { get; } = Substitute.For<IBackgroundJobClient>();
    // public IDomainEventDispatcher DomainEventDispatcherMock { get; } = Substitute.For<IDomainEventDispatcher>();
    // public IProcessOutboxMessagesJob ProcessOutboxMessagesJobMock { get; } =
    //     Substitute.For<IProcessOutboxMessagesJob>();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        _redisConnection = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());

        HttpClient = CreateClient();
        await ApplyMigrationsAsync();
        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await ResetDatabaseAsync();

        await _dbConnection.DisposeAsync();
        _redisConnection.Dispose();
        await _dbContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IFileStorageService>();
            services.AddScoped(_ => FileStorageServiceMock);

            services.RemoveAll<IBackgroundJobClient>();
            services.AddScoped(_ => BackgroundJobClientMock);

            services.RemoveAll<IRecurringJobManager>();
            services.AddSingleton(_ => Substitute.For<IRecurringJobManager>());

            // services.RemoveAll<IDomainEventDispatcher>();
            // services.AddScoped(_ => DomainEventDispatcherMock);

            // services.RemoveAll<IProcessOutboxMessagesJob>();
            // services.AddScoped(_ => ProcessOutboxMessagesJobMock);
        });

        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString());

        Environment.SetEnvironmentVariable("ConnectionStrings:Redis", _redisContainer.GetConnectionString());

        Environment.SetEnvironmentVariable("JwtSettings:SigningKey", SigningKey);
        Environment.SetEnvironmentVariable("JwtSettings:Issuer", Issuer);
        Environment.SetEnvironmentVariable("JwtSettings:Audience", Audience);
        Environment.SetEnvironmentVariable("JwtSettings:ExpiresInMinutes", "30");

        // builder.ConfigureServices(services =>
        // {
        //     // Remove old DbContext if present
        //     ServiceDescriptor? descriptor = services.SingleOrDefault(
        //         d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
        //     if(descriptor is not null)
        //     {
        //         services.Remove(descriptor);
        //     }

        //     // Add new DbContext with test container connection string
        //     services.AddDbContext<ApplicationDbContext>(options =>
        //     {
        //         options.UseNpgsql(_dbContainer.GetConnectionString());
        //     });
        // });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);

        IDatabase db = _redisConnection.GetDatabase();
        await db.ExecuteAsync("FLUSHDB");
    }

    private async Task InitializeRespawnerAsync()
    {
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
                    new Respawn.Graph.Table("Roles"),
                    new Respawn.Graph.Table("RolePermissions"),
                    new Respawn.Graph.Table("Permissions")
                ]
            }
        );
    }

    private async Task ApplyMigrationsAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static string GenerateJwtToken(
        string userId = "test-user-id",
        string role = "Customer",
        string deviceId = "test-device-id",
        string email = "test@temp.com",
        IList<string>? permissions = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimsConstants.NameIdentifier, userId),
            new(ClaimsConstants.Email, email),
            new(ClaimsConstants.DeviceId, deviceId),
            new(ClaimsConstants.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if(permissions is not null)
        {
            foreach(var permission in permissions)
            {
                claims.Add(new(ClaimsConstants.Permission, permission));
            }
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
        var accessToken = tokenHandler.WriteToken(securityToken);

        return accessToken;
    }
}
