using FluentValidation;
using MediatR;
using Noizera.Common.Persistence.S3;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles;

public sealed record GetProfileImageQuery(
    string ProfilePublicId) : IRequest<(Stream Stream, string ContentType)>
{
    public sealed class Handler(
        S3Context s3Context)
        : IRequestHandler<GetProfileImageQuery, (Stream Stream, string ContentType)>
    {
        public async Task<(Stream Stream, string ContentType)> Handle([NotNull] GetProfileImageQuery request, CancellationToken cancellationToken)
            => await s3Context.GetProfileImageAsync(request.ProfilePublicId, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetCoverImageValidator : AbstractValidator<GetProfileImageQuery>
{
    public GetCoverImageValidator()
    {
        _ = RuleFor(x => x.ProfilePublicId).NotEmpty();
    }
}
