using ecommerce.api.Shared;
using ecommerce.api.Shared.Behaviors;
using ecommerce.api.Shared.Exceptions;

namespace ecommerce.api.Abstractions.Messaging;

public class HandlerExecutor(
    IValidationService validationService,
    ILogger<HandlerExecutor> logger)
{
    public async Task<BaseResponse<T>> ExecuteAsync<TRequest, T>(
        TRequest request,
        Func<Task<BaseResponse<T>>> func,
        CancellationToken cancellationToken = default)
    {

        try
        {
            await validationService.ValidationAsync(request, cancellationToken);
            return await func();
        }
        catch (ValidationException e)
        {
            logger.LogWarning("Validaion failure {@Request}, Errors {@Errors}", request, e.Errors);
            return new BaseResponse<T>
            {
                IsSuccess = false,
                Message = "Validation failded",
                Errors = e.Errors
            };
        }
        catch(Exception e)
        {
            logger.LogError("Validation failure {@Request}", request);
            return new BaseResponse<T>
            {
                IsSuccess = false,
                Message = e.Message,
                Errors = new List<BaseError>
                {
                    new BaseError
                    {
                        PropertyName = "Exception",
                        ErrorMessage = e.Message
                    }
                }
            };
        }
        
        return new BaseResponse<T>();
    } 
    
}
