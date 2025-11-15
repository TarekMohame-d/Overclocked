using Api;
using Api.Middleware;
using Application;
using Hangfire;
using Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// --------------------------------------------
// CONFIGURATION SETUP
// --------------------------------------------
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

// Load external config (for development only)
if (builder.Environment.IsDevelopment())
{
    var configPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "configs.json");
    builder.Configuration.AddJsonFile(configPath, true, true);
}

builder.Configuration.AddEnvironmentVariables();


builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));


builder.Services
    .AddPresentation(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration);


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Overclocked API"); });
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();
app.UseMiddleware<QueryParameterValidationMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/dashboard");

// app.UseStaticFiles();

app.MapControllers();

app.Run();
