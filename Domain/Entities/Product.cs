namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<ProductProvider> ProductProviders { get; set; } = new List<ProductProvider>();
    public ICollection<WishList> WishLists { get; set; } = new List<WishList>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
