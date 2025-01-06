using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SetAdminDropdownFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.OperationId == "SetAdmin")
        {
            var userIdParameter = operation.Parameters.FirstOrDefault(p => p.Name == "userId");
            if (userIdParameter != null)
            {
                // Configurăm pentru a folosi un endpoint dinamic
                userIdParameter.Extensions.Add("x-extensible-enum", new OpenApiObject
                {
                    ["fetch"] = new OpenApiString("/GetDropdownUsers") // Endpoint-ul dinamic
                });

                userIdParameter.Description = "Selectați un utilizator din lista dinamică.";
            }
        }
    }
}
