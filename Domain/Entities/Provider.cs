namespace Domain.Entities;

public class Provider
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ProductProvider> ProductProviders { get; set; } = new List<ProductProvider>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
