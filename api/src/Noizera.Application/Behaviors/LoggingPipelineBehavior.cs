using MediatR;
using Noizera.Common.Contracts.Security;
using Serilog;
using Serilog.Events;
using SerilogTracing;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.Behaviors;

public sealed class LoggingPipelineBehavior<TRequest, TResponse>()
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        [NotNull] RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response;
        string requestName = typeof(TRequest).Name;
        var userId = request is IAuthorizeableRequest<TResponse> authRequest ? authRequest.UserId.ToString() : string.Empty;

        using var listener = new ActivityListenerConfiguration()
            .Instrument.AspNetCoreRequests()
            .TraceTo(Log.Logger);

        using var activity = Log.Logger.StartActivity("{requestName} completed. UserId = {userId}", requestName);
        try
        {
            if (request is not ISensitiveRequest)
            {
                Log.Logger.Information("Request: {payload}", request);
            }

            response = await next().ConfigureAwait(false);

            Log.Logger.Information("Response: {response}", response);

            activity.Complete();
        }
        catch (Exception exception)
        {
            activity.Complete(LogEventLevel.Fatal, exception);

            throw;
        }

        return response;
    }
}
