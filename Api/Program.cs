using Api;
using Api.ActionFilters;
using Api.Middleware;
using Application;
using Infrastructure;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------
// CONFIGURATION SETUP
// --------------------------------------------
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Load external config (for development only)
if (builder.Environment.IsDevelopment())
{
    var configPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "configs.json");
    builder.Configuration.AddJsonFile(configPath, optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();



builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));


builder.Services
    .AddPresentation(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Overclocked API");
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();
app.UseMiddleware<QueryParameterValidationMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

// app.UseStaticFiles();

app.MapControllers();

app.Run();
