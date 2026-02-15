using ecommerce.api.Enums;

namespace ecommerce.api.Contracts.User;

public class UserResponse
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? FirstName { get; set; } 

    public string? LastName { get; set; } 

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Address { get; set; }
        
    public UserType UserType { get; set; }
        
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        
    public DateTime? UpdateDate { get; set; }

}