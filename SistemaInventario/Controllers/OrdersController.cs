using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaInventario.Controllers;

[Authorize(Roles = "Buyer")]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        if (page < 1 || size < 1)
            return BadRequest("Page y size deben ser mayores a 0.");

        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("Token inválido.");

        var orders = await _orderService.GetUserOrdersAsync(userId.Value, page, size);
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("Token inválido.");

        var order = await _orderService.GetUserOrderByIdAsync(userId.Value, id);
        if (order == null)
            return NotFound($"No se encontró la orden con ID {id}.");

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("Token inválido.");

        try
        {
            var order = await _orderService.CreateOrderAsync(dto, userId.Value);
            if (order == null)
                return Unauthorized("Usuario no encontrado.");

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var userId) ? userId : null;
    }
}
