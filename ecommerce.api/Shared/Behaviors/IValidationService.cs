namespace ecommerce.api.Shared.Behaviors;

public interface IValidationService
{
    Task ValidationAsync<T>(T Request, CancellationToken cancellationToken = default);
}