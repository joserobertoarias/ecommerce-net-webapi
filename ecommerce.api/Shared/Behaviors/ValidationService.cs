using FluentValidation;
using ValidationException = ecommerce.api.Shared.Exceptions.ValidationException;

namespace ecommerce.api.Shared.Behaviors;

public class ValidationService(IServiceProvider serviceProvider) : IValidationService
{
    public async Task ValidationAsync<T>(T Request, CancellationToken cancellationToken = default)
    {
        var validators = serviceProvider.GetServices<IValidator<T>>();
        if (!validators.Any()) return;
        
        var context = new ValidationContext<T>(Request);
        var validationResults = await Task
            .WhenAll(validators
                .Select(v => v
                    .ValidateAsync(context, cancellationToken)
                )
            );
        var failures = validationResults
            .Where(e => e.Errors.Any())
            .SelectMany(e => e.Errors)
            .Select(e => new BaseError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage
            }).ToList();

        if (failures.Any())
            throw new ValidationException(failures);
    }
}