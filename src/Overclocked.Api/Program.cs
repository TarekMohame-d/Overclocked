using DotNetEnv;
using Hangfire;
using Overclocked.Api;
using Overclocked.Api.Extensions;
using Overclocked.Api.Middleware;
using Overclocked.Application;
using Overclocked.Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddPresentation(builder.Configuration).AddInfrastructure(builder.Configuration).AddApplication();

WebApplication app = builder.Build();

app.UseBackgroundJobs();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Env.TraversePath().Load();

    app.MapOpenApi();

    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Overclocked API"));

    app.UseHangfireDashboard(options: new DashboardOptions { DarkModeEnabled = true });
}

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
