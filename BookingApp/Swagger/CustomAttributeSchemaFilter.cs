using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookingApp.Swagger
{
    public class CustomAttributeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(bool)) // Pentru `isAdmin`
            {
                schema.Extensions.Add("x-data-is-admin", new OpenApiBoolean(true));
            }

            if (context.Type == typeof(int) || context.Type == typeof(int?))
            {
                schema.Extensions.Add("x-data-user-id", new OpenApiBoolean(true));
                schema.Type = "integer";
                schema.Format = "int32";
            }
        }

    }
}