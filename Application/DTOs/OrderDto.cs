using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
}

public class CreateOrderDto
{
    public string? Notes { get; set; }

    [Required(ErrorMessage = "La orden debe tener al menos un ítem.")]
    [MinLength(1, ErrorMessage = "La orden debe tener al menos un ítem.")]
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

public class CreateOrderItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto debe ser válido.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El proveedor debe ser válido.")]
    public int ProviderId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Quantity { get; set; }
}
