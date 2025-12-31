using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace Voidwell.Auth;

public class SecurityHeadersAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var result = context.Result;
        if (result is ViewResult)
        {
            if (context.HttpContext.Response.Headers.XContentTypeOptions.IsNullOrEmpty())
            {
                context.HttpContext.Response.Headers.XContentTypeOptions = "nosniff";
            }
            if (context.HttpContext.Response.Headers.XFrameOptions.IsNullOrEmpty())
            {
                context.HttpContext.Response.Headers.XFrameOptions = "SAMEORIGIN";
            }

            var csp = "default-src 'self';";
            // an example if you need client images to be displayed from twitter
            //var csp = "default-src 'self'; img-src 'self' https://pbs.twimg.com";

            // once for standards compliant browsers
            if (context.HttpContext.Response.Headers.ContentSecurityPolicy.IsNullOrEmpty())
            {
                context.HttpContext.Response.Headers.ContentSecurityPolicy = csp;
            }
            // and once again for IE
            if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
            {
                context.HttpContext.Response.Headers["X-Content-Security-Policy"] = csp;
            }
        }
    }
}
