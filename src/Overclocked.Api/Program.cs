using DotNetEnv;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Overclocked.Api;
using Overclocked.Api.Extensions;
using Overclocked.Api.Middleware;
using Overclocked.Application;
using Overclocked.Infrastructure;
using Overclocked.Infrastructure.Persistence;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.TraversePath().Load();
    builder.Configuration.AddEnvironmentVariables();
}

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddPresentation(builder.Configuration).AddInfrastructure(builder.Configuration).AddApplication();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Overclocked API"));

    app.UseHangfireDashboard(options: new DashboardOptions { DarkModeEnabled = true });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();

        Log.Information("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while migrating the database.");
    }
}

app.UseBackgroundJobs();

app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.UseHangfireDashboard("/dashboard");

// app.UseStaticFiles();

app.MapControllers().RequireRateLimiting("per-user");

app.Run();
