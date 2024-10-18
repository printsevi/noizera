using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using ValidationException = FluentValidation.ValidationException;

namespace Noizera.Application.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        [NotNull] RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null)
        {
            return await next().ConfigureAwait(false);
        }

        List<ValidationFailure> failures = [];
        var result = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        failures.AddRange(result.Errors.Where(x => x != null));

        return failures.Count > 0
            ? throw new ValidationException(failures)
            : await next().ConfigureAwait(false);
    }
}
