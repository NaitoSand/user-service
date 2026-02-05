using UserService.Api.V1.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace UserService.Api.V1.Extensions;

public static class DependencyInjection
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }

    public static IServiceCollection AddPresentationV1(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(DependencyInjection).Assembly);

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection("Swagger").Get<SwaggerSettings>();
        if (settings is null || !settings.Enabled)
            return services;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(settings.Version, new OpenApiInfo
            {
                Title = settings.Title,
                Version = settings.Version,
                Description = settings.Description,
                Contact = new OpenApiContact
                {
                    Name = settings.Contact.Name,
                    Url = new Uri(settings.Contact.Url)
                }
            });

            options.SchemaFilter<UserSchemaFilter>();

            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var xmlFile in xmlFiles)
            {
                options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
    {
        var settings = config.GetSection("Swagger").Get<SwaggerSettings>();
        if (settings is null || !settings.Enabled)
            return app;

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{settings.Version}/swagger.json", settings.Title);
                options.RoutePrefix = string.Empty;
            });
        }

        return app;
    }
}