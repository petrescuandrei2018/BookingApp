using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace BookingApp.Helpers
{
    public class SetAdminOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Validăm dacă operațiunea este legată de metoda "SetAdmin".
            if (operation?.OperationId != "SetAdmin") return;

            // Actualizăm descrierile parametrilor existenți fără să creăm duplicate.
            operation.Parameters?.FirstOrDefault(p => p.Name == "userId")?.ApplyDescription("ID-ul utilizatorului care va fi modificat.");
            operation.Parameters?.FirstOrDefault(p => p.Name == "isAdmin")?.ApplyDescription("Setează true pentru Admin, false pentru User.");
        }
    }

    // Extensie pentru a seta descrierea unui parametru în mod clar.
    public static class OpenApiParameterExtensions
    {
        public static void ApplyDescription(this OpenApiParameter parameter, string description)
        {
            if (parameter != null)
            {
                parameter.Description = description;
            }
        }
    }
}
