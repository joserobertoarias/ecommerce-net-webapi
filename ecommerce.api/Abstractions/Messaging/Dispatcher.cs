using ecommerce.api.Shared;

namespace ecommerce.api.Abstractions.Messaging;

public class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public async Task<BaseResponse<TResponse>> Dispatch<TRequest, TResponse>
        (TRequest request, CancellationToken cancellationToken) where TRequest : IRequest<TResponse>
    {
        try
        {
            if (request is ICommand<TResponse>)
            {
                var handlerType = typeof(ICommandHandler<,>)
                    .MakeGenericType(request.GetType(), typeof(TResponse));

                dynamic handler = serviceProvider.GetRequiredService(handlerType);
                return await handler.Handle((dynamic)request, cancellationToken);
            }

            if (request is IQuery<TResponse>)
            {
                var handlerType = typeof(IQueryHandler<,>)
                    .MakeGenericType(request.GetType(), typeof(TResponse));
                
                dynamic handler = serviceProvider.GetRequiredService(handlerType);
                
                return await handler.Handle((dynamic)request, cancellationToken);
            }
            
            throw new InvalidOperationException("Request not supported");
        }
        catch (Exception e)
        {
            return new BaseResponse<TResponse>
            {
                IsSuccess = false,
                Message = "something went wrong",
                Errors = [
                    new BaseError
                    {
                        PropertyName = "Dispatcher",
                        ErrorMessage = e.Message
                    }
                ]
            };
        }
    }
}