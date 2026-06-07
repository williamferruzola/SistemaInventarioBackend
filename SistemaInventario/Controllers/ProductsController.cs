using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaInventario.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        if (page < 1 || size < 1)
            return BadRequest("Page y size deben ser mayores a 0.");

        var products = await _productService.GetProductsAsync(page, size, search);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound($"No se encontró un producto con ID {id}.");

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var productId = await _productService.AddProductAsync(productDto);
            productDto.Id = productId;
            return CreatedAtAction(nameof(GetProductById), new { id = productId }, productDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _productService.UpdateProductAsync(id, productDto);
            if (!updated)
                return NotFound($"No se encontró un producto con ID {id} para actualizar.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteProductAsync(id);
        if (!deleted)
            return NotFound($"No se encontró un producto con ID {id} para eliminar.");

        return NoContent();
    }
}
