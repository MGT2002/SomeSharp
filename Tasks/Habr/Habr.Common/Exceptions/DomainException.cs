using Habr.Common.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Common.Exceptions;

public class DomainException : Exception
{
    static DomainException()
    {
        NotFound = GenerateNotFound(ExceptionMessageResource.NotFoundDefaultMessage);

        AccessDenied = new()
        {
            ProblemDetails = new()
            {
                Status = StatusCodes.Status403Forbidden,
                Title = ExceptionMessageResource.AccessDeniedTitle,
                Detail = ExceptionMessageResource.AccessDeniedMessage,
            }
        };
    }

    public static DomainException NotFound { get; }
    public static DomainException AccessDenied { get; }
    public ProblemDetails ProblemDetails { get; set; } = null!;

    public static DomainException GenerateUnprocessableAction(string message)
    {
        return new()
        {
            ProblemDetails = new()
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = ExceptionMessageResource.UnprocessableActionTitle,
                Detail = message,
            }
        };
    }

    public static DomainException GenerateNotFound(string message)
    {
        return new()
        {
            ProblemDetails = new()
            {
                Status = StatusCodes.Status404NotFound,
                Title = ExceptionMessageResource.NotFoundTitle,
                Detail = message,
            }
        };
    }
}
