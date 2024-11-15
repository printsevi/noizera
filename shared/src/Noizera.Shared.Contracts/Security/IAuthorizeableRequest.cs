using MediatR;

namespace Noizera.Common.Contracts.Security;

public interface IAuthorizeableRequest<T> : IRequest<T>
{
    Guid UserId { get; }
}