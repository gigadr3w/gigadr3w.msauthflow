using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace gigadr3w.msauthflow.authenticator.iterator.Filters
{
    public class SwaggerAuthenticationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "ApiKey",
                In = ParameterLocation.Header,
                Description = "Insert your token",
                Required = true
            });
        }
    }
}
