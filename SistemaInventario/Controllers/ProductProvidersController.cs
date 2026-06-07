using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaInventario.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ProductProvidersController : ControllerBase
{
    private readonly IProductProviderService _productProviderService;

    public ProductProvidersController(IProductProviderService productProviderService)
    {
        _productProviderService = productProviderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductProviders(
        [FromQuery] int? productId = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        if (page < 1 || size < 1)
            return BadRequest("Page y size deben ser mayores a 0.");

        var result = await _productProviderService.GetProductProvidersAsync(productId, page, size, search);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductProviderById(int id)
    {
        var productProvider = await _productProviderService.GetProductProviderByIdAsync(id);
        if (productProvider == null)
            return NotFound($"No se encontró la relación producto-proveedor con ID {id}.");

        return Ok(productProvider);
    }

    [HttpPost]
    public async Task<IActionResult> AddProductProvider([FromBody] ProductProviderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
     
        try
        {
            var id = await _productProviderService.AddProductProviderAsync(dto);
            dto.Id = id;
            return CreatedAtAction(nameof(GetProductProviderById), new { id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProductProvider(int id, [FromBody] ProductProviderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _productProviderService.UpdateProductProviderAsync(id, dto);
            if (!updated)
                return NotFound($"No se encontró la relación producto-proveedor con ID {id} para actualizar.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProductProvider(int id)
    {
        var deleted = await _productProviderService.DeleteProductProviderAsync(id);
        if (!deleted)
            return NotFound($"No se encontró la relación producto-proveedor con ID {id} para eliminar.");

        return NoContent();
    }
}
