using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users;

namespace Noizera.Api.Endpoints.Users;

internal sealed class UploadProfileImageEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/users/{userId}/image", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        context.Features.Get<IHttpMaxRequestBodySizeFeature>()!.MaxRequestBodySize = 1 * 1024 * 1024; // 1 MB
        var form = await context.Request.ReadFormAsync(ct).ConfigureAwait(false);
        var file = form.Files[0];
        UploadProfileImageCommand command = new(file, userId); //544px x 544px
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}