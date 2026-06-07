using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class ProductProviderDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El proveedor es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El proveedor debe ser válido.")]
    public int ProviderId { get; set; }

    public string ProviderName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    public string? BatchNumber { get; set; }
    public bool IsActive { get; set; } = true;
}
