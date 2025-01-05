using BookingApp.Helpers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

public class SetAdminDropdownFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.OperationId == "SetAdmin") // Filtrăm doar metoda SetAdmin
        {
            // Folosim datele din cache
            var users = UserDropdownCache.Users;

            var userIdParameter = operation.Parameters.FirstOrDefault(p => p.Name == "userId");
            if (userIdParameter != null && users.Any())
            {
                userIdParameter.Schema.Enum = users.Select(u =>
                    new OpenApiString(u.DisplayName))
                    .Cast<IOpenApiAny>()
                    .ToList();

                userIdParameter.Description = "Selectați un utilizator din lista generată.";
            }
        }
    }
}
