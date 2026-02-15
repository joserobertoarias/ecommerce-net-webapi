using ecommerce.api.Shared;

namespace ecommerce.api.Abstractions.Messaging;

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<BaseResponse<TResponse>> Handle(TCommand query, CancellationToken cancellationToken);
}