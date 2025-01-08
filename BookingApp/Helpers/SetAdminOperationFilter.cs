using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace BookingApp.Helpers
{
    public class SetAdminOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId == "SetAdmin")
            {
                // Modificăm descrierile parametrilor existenți, fără să adăugăm duplicat.
                var userIdParam = operation.Parameters.FirstOrDefault(p => p.Name == "userId");
                if (userIdParam != null)
                {
                    userIdParam.Description = "ID-ul utilizatorului care va fi modificat.";
                }

                var isAdminParam = operation.Parameters.FirstOrDefault(p => p.Name == "isAdmin");
                if (isAdminParam != null)
                {
                    isAdminParam.Description = "Setează true pentru Admin, false pentru User.";
                }
            }
        }
    }

}
