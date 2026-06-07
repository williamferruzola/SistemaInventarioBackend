using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaInventario.Controllers;

[Authorize(Roles = "Buyer")]
[ApiController]
[Route("api/[controller]")]
public class WishListController : ControllerBase
{
    private readonly IWishListService _wishListService;

    public WishListController(IWishListService wishListService)
    {
        _wishListService = wishListService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishList()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("Token inválido.");

        var items = await _wishListService.GetUserWishListAsync(userId.Value);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishList([FromBody] AddWishListDto dto)
    {
        if (dto.ProductId <= 0)
            return BadRequest("El productId debe ser mayor a 0.");

        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("Token inválido.");

        try
        {
            var item = await _wishListService.AddToWishListAsync(userId.Value, dto.ProductId);
            return CreatedAtAction(nameof(GetWishList), item);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> RemoveFromWishList(int productId)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("Token inválido.");

        var removed = await _wishListService.RemoveFromWishListAsync(userId.Value, productId);
        if (!removed)
            return NotFound($"El producto con ID {productId} no está en la lista de deseos.");

        return NoContent();
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var userId) ? userId : null;
    }
}
