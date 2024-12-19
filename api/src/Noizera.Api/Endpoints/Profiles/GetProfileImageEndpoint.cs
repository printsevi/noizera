using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class GetProfileImageEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/profiles/{profilePublicId}/image", Handle)
              .AllowAnonymous();

    private static async Task<IResult> Handle(
        string profilePublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetProfileImageQuery query = new(profilePublicId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return result.HasValue ? Results.File(result.Value.Stream, result.Value.ContentType, enableRangeProcessing: false) : Results.NoContent();
    }
}