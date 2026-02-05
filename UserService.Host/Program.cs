using UserService.Api.V1.Extensions;
using UserService.Application.Extensions;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//
// --- Service configuration (Composition Root) ---
//
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddPresentationV1()
    .AddLogging()
    .AddSwaggerDocumentation(builder.Configuration)
    .AddHealthChecks();

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);

    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Create SQLite database automatically if it does not exist
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<UserDbContext>>();
    using var db = factory.CreateDbContext();
    db.Database.EnsureCreated();
}

//
// --- Middleware pipeline configuration ---
//
app.UseGlobalExceptionHandler();
app.UseSwaggerDocumentation(app.Environment, builder.Configuration);
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();