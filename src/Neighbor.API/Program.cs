using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Neighbor.API.DepedencyInjection.Extensions;
using Neighbor.API.Middleware;
using Neighbor.Application.DependencyInjection.Extensions;
using Neighbor.Infrastructure.Dapper.DependencyInjection.Extensions;
using Neighbor.Infrastructure.DependencyInjection.Extensions;
using Neighbor.Infrastructure.Services;
using Neighbor.Persistence;
using Neighbor.Persistence.DependencyInjection.Extensions;
using Neighbor.Persistence.DependencyInjection.Options;
using Neighbor.Persistence.SeedData;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

builder.Services.AddConfigureMediatR();

builder
    .Services
    .AddControllers()
    .AddApplicationPart(Neighbor.Presentation.AssemblyReference.Assembly);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddHttpClient();

// Configure Options and SQL
builder.Services.ConfigureSqlServerRetryOptions(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlConfiguration();
builder.Services.AddRepositoryBaseConfiguration();

// Configure Dapper
builder.Services.AddInfrastructureDapper();

// Configure Options and Redis
builder.Services.AddConfigurationRedis(builder.Configuration);

builder.Services.AddConfigurationService();

builder.Services.AddConfigurationAppSetting(builder.Configuration);

builder.Services.AddConfigurationAutoMapper();

builder.Services
        .AddSwaggerGenNewtonsoftSupport()
        .AddFluentValidationRulesToSwagger()
        .AddEndpointsApiExplorer()
        .AddSwagger();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Config CORS
builder.Services.AddCors(options =>
{
    var clientUrl = builder.Configuration["ClientConfiguration:Url"];
    options.AddPolicy("AllowSpecificOrigin",
        option =>
    {
        option.WithOrigins(clientUrl)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
    });
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    SeedData.Seed(context, builder.Configuration, new PasswordHashService());
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
    app.ConfigureSwagger();

try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}

public partial class Program { }
