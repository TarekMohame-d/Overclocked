using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using NSubstitute;
using Respawn;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Integration.Tests.Shared;

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

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;
    public HttpClient HttpClient { get; private set; } = null!;
    public IFileStorageService FileStorageServiceMock { get; } = Substitute.For<IFileStorageService>();
    public IBackgroundJobClient BackgroundJobClientMock { get; } = Substitute.For<IBackgroundJobClient>();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();
        await ApplyMigrationsAsync();
        HttpClient = CreateClient();
        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
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
        });

        Environment.SetEnvironmentVariable(
            "ConnectionStrings:DefaultConnection",
            _dbContainer.GetConnectionString());

        Environment.SetEnvironmentVariable(
            "ConnectionStrings:Redis",
            _redisContainer.GetConnectionString());

        Environment.SetEnvironmentVariable("JwtSettings:SigningKey", SigningKey);
        Environment.SetEnvironmentVariable("JwtSettings:Issuer", Issuer);
        Environment.SetEnvironmentVariable("JwtSettings:Audience", Audience);
        Environment.SetEnvironmentVariable("JwtSettings:ExpiresInMinutes", "30");


        //builder.ConfigureServices(services =>
        //{
        //    // Remove old DbContext if present
        //    var descriptor = services.SingleOrDefault(
        //        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
        //    if (descriptor is not null)
        //        services.Remove(descriptor);

        //    // Add new DbContext with test container connection string
        //    services.AddDbContext<ApplicationDbContext>(options =>
        //    {
        //        options.UseNpgsql(_dbContainer.GetConnectionString());
        //    });
        //});
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);

        ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        IDatabase db = redis.GetDatabase();
        await db.ExecuteAsync("FLUSHDB");
    }

    private async Task InitializeRespawnerAsync()
    {
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            SchemasToInclude = ["public"],
            DbAdapter = DbAdapter.Postgres,
            WithReseed = true
        });
    }

    private async Task ApplyMigrationsAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static string GenerateJwtToken(string userId = "test-user-id", string role = "Customer",
        string deviceId = "test-device-id", IList<string>? permissions = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimsConstants.NameIdentifier, userId),
            new(ClaimsConstants.Email, "test-user-email"),
            new(ClaimsConstants.DeviceId, deviceId),
            new(ClaimsConstants.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (permissions is not null)
        {
            foreach (var permission in permissions)
            {
                claims.Add(new(ClaimsConstants.Permission, permission));
            }
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials,
            Issuer = Issuer,
            Audience = Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return accessToken;
    }
}
