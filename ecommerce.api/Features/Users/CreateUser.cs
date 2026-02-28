using System.Diagnostics;
using Carter;
using Dapper;
using ecommerce.api.Abstractions.Messaging;
using ecommerce.api.Database;
using ecommerce.api.Enums;
using ecommerce.api.Shared;
using FluentValidation;

namespace ecommerce.api.Features.Users;

public class CreateUser
{
    #region  command
    public sealed class CreateUserCommand : ICommand<bool>
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string? FirstName { get; set; } 

        public string? LastName { get; set; } 

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string? Celphone { get; set; } 

        public string? Address { get; set; }
        
        public UserType UserType { get; set; }
        
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdateDate { get; set; }
    }
    #endregion

    #region validation
    public class Validator : AbstractValidator<CreateUserCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserName)
                .NotNull().WithMessage("Username is required")
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");
            
        }
    }
    #endregion

    #region  handler
    internal sealed class Handler(
        ApplicationDbContext dbContext, 
        HandlerExecutor executor) : ICommandHandler<CreateUserCommand, bool>
    {
        public async Task<BaseResponse<bool>> Handle(CreateUserCommand query, CancellationToken cancellationToken)
        {
            return await executor.ExecuteAsync(query,
                async () => await CreateUserAsync(query, cancellationToken), cancellationToken);
        }

        private async Task<BaseResponse<bool>> CreateUserAsync(CreateUserCommand query,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();
            const string sql = @"INSERT INTO users(      
                  ""username"", 
                  ""firstname"", 
                  ""lastname"", 
                  ""password"", 
                  ""email"", 
                  ""celphone"",       
                  ""address"", 
                  ""usertype"", 
                  ""createdate"")
	        VALUES (@username, 
	                @firstname, 
	                @lastname, 
	                @password, 
	                @email, 
	                @celphone,     
	                @address, 
	                @usertype, 
	                now());";

            try
            {
                using var connection = dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("username", query.UserName);
                parameters.Add("firstname", query.FirstName);
                parameters.Add("lastname", query.LastName);
                parameters.Add("password", query.Password);
                parameters.Add("email", query.Email);
                parameters.Add("celphone", query.Celphone);
                parameters.Add("address", query.Address);
                parameters.Add("usertype", query.UserType);

                var result = await connection.ExecuteAsync(sql, parameters);
                response.IsSuccess = result > 0;
                response.Data = result > 0;
                response.Message = "Se registro correctamente";
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = $"Hubo un error en el registro, {e.Message}";
            }

            return response;
        }
    }
    #endregion

    #region endpoint

    public class CreateUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/user", async (CreateUserCommand command,
                IDispatcher dispatcher,
                CancellationToken cancellationToken) =>
            {
                var response = await dispatcher.Dispatch<CreateUserCommand, bool>(command, cancellationToken);
                return Results.Ok(response);
            });
        }
    }
    

    #endregion
}