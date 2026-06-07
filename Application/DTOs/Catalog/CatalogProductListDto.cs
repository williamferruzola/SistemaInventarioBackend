namespace Application.DTOs.Catalog;

public class CatalogProductListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public int TotalStock { get; set; }
}
