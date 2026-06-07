using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La categoría debe ser válida.")]
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
