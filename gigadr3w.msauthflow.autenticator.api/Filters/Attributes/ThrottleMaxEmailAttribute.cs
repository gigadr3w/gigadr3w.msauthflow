using gigadr3w.msauthflow.autenticator.api.Requests;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace gigadr3w.msauthflow.autenticator.api.Filters.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ThrottleMaxEmailAttribute : ActionFilterAttribute
    {
        private readonly int _offsetSeconds;
        private readonly short _maxAttempts;

        public ThrottleMaxEmailAttribute(int offsetSeconds, short maxAttempts)
        {
            _offsetSeconds = offsetSeconds;
            _maxAttempts = maxAttempts;
        }                

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            throw new Exception("Ciccinoooooo");

            string email = await GetEmail(context);

            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            IMemoryCache memoryCache = serviceProvider.GetService<IMemoryCache>()
                ?? throw new NullReferenceException("Memory cache missing! Register a memory cache service services.AddMemoryCache().");

            if (!string.IsNullOrEmpty(email))
            {
                string key = $"authetication-attemp:{email}";
                int currentAttemptsForEmail = memoryCache.Get<int>(key);
                
                if (currentAttemptsForEmail >= _maxAttempts)
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status429TooManyRequests);
                    return;
                }
                memoryCache.Set(key, currentAttemptsForEmail + 1, DateTime.Now.AddSeconds(_offsetSeconds));
            }

            await next();
        }

        private async Task<string> GetEmail(ActionExecutingContext context)
        {
            /* 
             * You must enable 
             * app.Use(async (context, next) =>
             * {
             *     context.Request.EnableBuffering();
             *     await next();
             * });
             */
            context.HttpContext.Request.Body.Position = 0;
            using (StreamReader sr = new StreamReader(context.HttpContext.Request.Body))
            {
                var body = await sr.ReadToEndAsync();

                if (string.IsNullOrEmpty(body)) return string.Empty;

                JsonSerializerOptions options = new (){ PropertyNameCaseInsensitive = true };
                AuthenticateRequest? model = JsonSerializer.Deserialize<AuthenticateRequest>(body, options);

                return model?.Email ?? string.Empty;
            }
        }
    }
}
