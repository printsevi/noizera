using MediatR;

namespace Noizera.Shared.Contracts.Security;

public interface IAuthorizeableRequest<T> : IRequest<T>
{
    Guid UserId { get; }
}