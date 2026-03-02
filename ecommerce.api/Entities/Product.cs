namespace ecommerce.api.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string? UrlImage { get; set; }

    public decimal Price { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? UdpateDate { get; set; }

    public int? UserId { get; set; }

    public int? CategoryId { get; set; }    
}
