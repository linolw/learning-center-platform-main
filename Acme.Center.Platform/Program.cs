using Acme.Center.Platform.Profiles.Application.CommandServices;
using Acme.Center.Platform.Shipping.Application.CommandServices;
using Acme.Center.Platform.Shipping.Application.Internal.CommandServices;
using Acme.Center.Platform.Shipping.Domain.Repositories;
using Acme.Center.Platform.Shipping.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Shipping.Infrastructure.Pipeline.Middleware.Components;
using System.Text.Json.Serialization;
using Acme.Center.Platform.Profiles.Application.Internal.CommandServices;
using Acme.Center.Platform.Profiles.Application.Internal.QueryServices;
using Acme.Center.Platform.Profiles.Application.QueryServices;
using Acme.Center.Platform.Profiles.Domain.Repositories;
using Acme.Center.Platform.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Resources.Errors;
using Acme.Center.Platform.Resources.Shared;
using Acme.Center.Platform.Shared.Domain.Repositories;
using Acme.Center.Platform.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Interfaces.AspNetCore.OpenApi;
using Acme.Center.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Shared.Infrastructure.Pipeline.Middleware.Components;
using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;
// Added for ProblemDetailsFactory
// Added for base ProblemDetailsFactory
// Added for IamMessages
// Added for ProfilesMessages
using ProblemDetailsFactory = Acme.Center.Platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Add ProblemDetails and ExceptionHandler services
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ShippingExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add Database Connection

// Configure Database Context and route EF logs through the app logger pipeline.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .AddInterceptors(serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Explicitly register IStringLocalizer for ErrorMessages and Commons
builder.Services.AddSingleton<IStringLocalizer<ErrorMessages>, StringLocalizer<ErrorMessages>>();
builder.Services
    .AddSingleton<IStringLocalizer<CommonMessages>,
        StringLocalizer<CommonMessages>>(); // Corrected from Common to Commons

// Register the custom ProblemDetailsFactory
builder.Services.AddSingleton<ProblemDetailsFactory>();


// OpenAPI / Scalar documentation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Dependency Injection

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DispatchDomainEventsInterceptor>();

// Publishing Bounded Context

// Profiles Bounded Context Dependency Injection Configuration
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();

// Shipping PC2 example: create shipment
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentCommandService, ShipmentCommandService>();


// IAM Bounded Context Injection Configuration

// TokenSettings Configuration


// Mediator Configuration

// Add Mediator Injection Configuration
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(ExceptionHandlingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(TransactionCommandBehavior<>));

// Add Cortex Mediator for Event Handling
builder.Services.AddCortexMediator(
    [typeof(Program)]);


var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var supportedCultures = new[] { "en", "en-US", "es", "es-PE" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Apply CORS Policy
app.UseCors("AllowAllPolicy");

// Add Authorization Middleware to Pipeline


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();