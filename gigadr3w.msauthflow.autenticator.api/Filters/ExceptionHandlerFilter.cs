using Microsoft.AspNetCore.Mvc.Filters;

namespace gigadr3w.msauthflow.autenticator.api.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            ILogger<ExceptionHandlerFilter> logger = context.HttpContext.RequestServices.GetService<ILogger<ExceptionHandlerFilter>>();
            if(logger != null) 
            {
                logger.LogError(context.Exception.Message, context.Exception.StackTrace);
            }
        }
    }
}
