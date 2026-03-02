namespace ecommerce.api.Entities;

public class Category
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}