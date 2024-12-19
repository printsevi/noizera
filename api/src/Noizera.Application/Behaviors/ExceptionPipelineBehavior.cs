using FluentValidation;
using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Domain.Common;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.Behaviors;

public sealed class ExceptionPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        [NotNull] RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response;
        try
        {
            response = await next().ConfigureAwait(false);
        }
        catch (AppException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (DomainRuleException ex)
        {
            throw new AppException(ex.Message, ErrorType.BusinessRule);
        }
        catch (ValidationException ex)
        {
            throw new AppException(ex.Message, ErrorType.Validation);
        }
        catch (ArgumentNullException ex)
        {
            throw new AppException(ex.Message, ErrorType.NullArgument);
        }
        catch (Exception ex)
        {
            throw new AppException(ex.Message, ErrorType.Internal);
        }

        return response;
    }
}
