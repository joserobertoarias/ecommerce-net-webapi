namespace ecommerce.api.Shared;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public IEnumerable<BaseError> Errors { get; set; }
}

public class BaseError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}