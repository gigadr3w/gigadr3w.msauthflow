using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace gigadr3w.msauthflow.authenticator.iterator.Filters
{
    public class SwaggerAuthenticationFilter : IOperationFilter
    {
        private readonly JwtTokenConfiguration _jwtTokenConfiguration;
        public SwaggerAuthenticationFilter(IOptions<JwtTokenConfiguration> jwtTokenConfiguration)
            => _jwtTokenConfiguration = jwtTokenConfiguration.Value;

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = _jwtTokenConfiguration.ApiKeyHeader,
                In = ParameterLocation.Header,
                Description = "Insert your token",
                Required = true
            });
        }
    }
}
