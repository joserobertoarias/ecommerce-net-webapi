namespace ecommerce.api.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}