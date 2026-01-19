using DotNetEnv;
using Hangfire;
using Overclocked.Api;
using Overclocked.Api.Extensions;
using Overclocked.Api.Filters;
using Overclocked.Api.Middleware;
using Overclocked.Application;
using Overclocked.Infrastructure;
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
}

app.UseBackgroundJobs();

app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions
    {
        Authorization =
        [
            new HangfireBasicAuthenticationFilter(
                builder.Configuration.GetValue<string>("Hangfire_Username")!,
                builder.Configuration.GetValue<string>("Hangfire_Password")!
            ),
        ],
        DarkModeEnabled = true,
    }
);

app.UseRateLimiter();

// app.UseStaticFiles();

app.MapControllers().RequireRateLimiting("per-user");

app.Run();
