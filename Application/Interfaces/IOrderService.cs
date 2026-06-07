using Application.DTOs;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto, int userId);
    Task<PagedResultDto<OrderSummaryDto>> GetUserOrdersAsync(int userId, int page, int size);
    Task<OrderDto?> GetUserOrderByIdAsync(int userId, int orderId);
}
