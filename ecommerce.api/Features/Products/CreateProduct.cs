using Carter;
using Dapper;
using ecommerce.api.Abstractions.Messaging;
using ecommerce.api.Database;
using ecommerce.api.Shared;
using FluentValidation;

namespace ecommerce.api.Features.Products;

public class CreateProduct
{
    #region command
    public sealed class CreateProductCommand : ICommand<bool>
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public string? UrlImage { get; set; }

        public decimal Price { get; set; }
        
        public int? UserId { get; set; }

        public int? CategoryId { get; set; }    
    }
    #endregion

    #region validation

    public class Validator : AbstractValidator<CreateProductCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required.")
                .PrecisionScale(10, 2, true)
                .GreaterThanOrEqualTo(0);
            

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .GreaterThan(0);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.UrlImage)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.UrlImage))
                .WithMessage("UrlImage must be a valid absolute URL.");
        }
    }

    #endregion

    #region handler

    internal sealed class Handler(ApplicationDbContext dbContext,
        HandlerExecutor executor) : ICommandHandler<CreateProductCommand, bool>
    {
        public async Task<BaseResponse<bool>> Handle(CreateProductCommand query, CancellationToken cancellationToken)
        {
            return await executor.ExecuteAsync(query,
                async () => await CreateProductAsync(query, cancellationToken), cancellationToken);
        }

        private async Task<BaseResponse<bool>> CreateProductAsync(CreateProductCommand command,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();
            
            const string sql = @"INSERT INTO public.""Products""(
                        ""Name"",
                        ""Code"",
                        ""Description"",
                        ""UrlImage"",
                        ""Price"",
                        ""CreateDate"",
                        ""UserId"",
                        ""CategoryId"")
                    VALUES (
                        @Name,
                        @Code,
                        @Description,
                        @UrlImage,
                        @Price,
                        now(),
                        @UserId,
                        @CategoryId);";

            try
            {
                using var connection = dbContext.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("Name", command.Name);
                parameters.Add("Code", command.Code);
                parameters.Add("Description", command.Description);
                parameters.Add("UrlImage", command.UrlImage);
                parameters.Add("Price", command.Price);
                parameters.Add("UserId", command.UserId);
                parameters.Add("CategoryId", command.CategoryId);

                var result = await connection.ExecuteAsync(sql, parameters);

                response.IsSuccess = result > 0;
                response.Data = result > 0;
                response.Message = "Se registró correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Hubo un error en el registro, {ex.Message}";
            }
            
            return response;
        }
    }

    #endregion

    #region endpoint

    public class CreateProductEndpoint : ICarterModule
    {        
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/product", async (CreateProductCommand command,
                IDispatcher dispatcher,
                CancellationToken cancellationToken) =>
            {
                var response = await dispatcher.Dispatch<CreateProductCommand, bool>(command, cancellationToken);
                return Results.Ok(response);
            });
        }
        
    }    

    #endregion
}