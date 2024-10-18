using Microsoft.AspNetCore.Mvc;
using Noizera.Shared.Contracts.Errors;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Api.Middlewares;

internal sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    public async Task InvokeAsync([NotNull] HttpContext context)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (AppException appException)
        {
            Dictionary<string, object?> data = new()
            {
                { "errorCode", appException.ErrorCode.ToString() }
            };

            ProblemDetails problemDetails = new()
            {
                Status = appException.HttpCode,
                Title = appException.ErrorType.ToString(),
                Detail = appException.Message,
                Extensions = data,
            };

            context.Response.StatusCode = appException.HttpCode;

            await context.Response.WriteAsJsonAsync(problemDetails).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ProblemDetails problemDetails = new()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = ex.Message,
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(problemDetails).ConfigureAwait(false);
        }
    }
}
