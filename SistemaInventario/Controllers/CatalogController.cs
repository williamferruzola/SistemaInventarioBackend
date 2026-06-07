using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaInventario.Controllers;

[Authorize(Roles = "Buyer")]
[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        if (page < 1 || size < 1)
            return BadRequest("Page y size deben ser mayores a 0.");

        var products = await _catalogService.GetProductsAsync(page, size, search);
        return Ok(products);
    }

    [HttpGet("products/{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _catalogService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound($"No se encontró un producto disponible con ID {id}.");

        return Ok(product);
    }
}
