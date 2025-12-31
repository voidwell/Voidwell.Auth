using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Voidwell.Auth.Admin.Exceptions;

namespace Voidwell.Auth.Admin.Filters;

public class ConflictExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ConflictException)
        {
            context.ExceptionHandled = true;
            context.Result = new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Content = context.Exception.Message ?? "Conflict"
            };
        }
    }
}
