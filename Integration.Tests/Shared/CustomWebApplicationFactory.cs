using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api;
using Application.Common.Constants;
using Application.Services;
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
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("OverclockedDBTest")
        .WithUsername("admin")
        .WithPassword("admin-pw")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;
    public HttpClient HttpClient { get; private set; } = default!;
    public IFileStorageService FileStorageServiceMock { get; } = Substitute.For<IFileStorageService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IFileStorageService>();

            services.AddScoped(_ => FileStorageServiceMock);
        });

        Environment.SetEnvironmentVariable(
            "ConnectionStrings:DefaultConnection",
            _dbContainer.GetConnectionString());

        Environment.SetEnvironmentVariable(
            "ConnectionStrings:Redis",
            _redisContainer.GetConnectionString());

        Environment.SetEnvironmentVariable("JwtSettings:SigningKey", "6a5080d2a0d8faea6fb1bfabba8919b15ede19a05f785a2552f8bbc04a0ff9beec356bcd");
        Environment.SetEnvironmentVariable("JwtSettings:Issuer", "TestIssuer");
        Environment.SetEnvironmentVariable("JwtSettings:Audience", "TestAudience");
        Environment.SetEnvironmentVariable("JwtSettings:ExpiryMinutes", "60");


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

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);

        var redis = ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString());
        var db = redis.GetDatabase();
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
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public string GenerateJwtToken(string userId = "test-user-id", string role = "Admin")
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimsConstants.NameIdentifier, userId.ToString()),
                new Claim(ClaimsConstants.Role, role)
            };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6a5080d2a0d8faea6fb1bfabba8919b15ede19a05f785a2552f8bbc04a0ff9beec356bcd")),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = creds,
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var accessToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(accessToken);
    }
}
