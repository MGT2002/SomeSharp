using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Middleware;

public sealed class DefaultGlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = Map(exception);

        await Results.Problem(problemDetails).ExecuteAsync(httpContext);

        return true;
    }

    private static ProblemDetails Map(Exception exception)
    {
        return exception switch
        {
            DomainException domainException => domainException.ProblemDetails,
            _ => new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = ExceptionMessageResource.DefaultServerExceptionTitle,
                Detail = exception.Message,
            }
        };
    }
}
