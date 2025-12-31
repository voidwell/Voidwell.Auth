using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Voidwell.Auth.Admin.Exceptions;

namespace Voidwell.Auth.Admin.Filters;

public class NotFoundExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is NotFoundException)
        {
            context.ExceptionHandled = true;
            context.Result = new ContentResult
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Content = context.Exception.Message ?? "Not Found"
            };
        }
    }
}
