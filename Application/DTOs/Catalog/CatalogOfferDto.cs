namespace Application.DTOs.Catalog;

public class CatalogOfferDto
{
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
