namespace Domain.Entities;

public class ProductProvider
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int ProviderId { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? BatchNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
}
