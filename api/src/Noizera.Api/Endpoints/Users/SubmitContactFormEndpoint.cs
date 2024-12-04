using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users;
using Noizera.Application.CQRS.Users.CheckUsername;

namespace Noizera.Api.Endpoints.Users;

internal sealed class SubmitContactFormEndpoint : IEndpoint
{
    internal sealed record Request(string Email, string Name, string Topic, string Description);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/users/contact", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        SubmitContactFormCommand query = new(request.Email, request.Name, request.Topic, request.Description);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}