using UserService.Domain.Entities;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UserService.Api.V1.Extensions;

/// <summary>
/// Swagger schema filter that marks certain <see cref="User"/> fields as required.
/// This keeps the domain entity clean (no [Required]) while improving API documentation.
/// </summary>
public class UserSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.FullName == "UserService.Domain.Entities.User")
        {
            schema.Required ??= new HashSet<string>();
            schema.Required.Add("email");
            schema.Required.Add("fullName");

            if (schema.Properties.TryGetValue("email", out var emailProp))
                emailProp.Nullable = false;

            if (schema.Properties.TryGetValue("fullName", out var fullNameProp))
                fullNameProp.Nullable = false;
        }
    }
}