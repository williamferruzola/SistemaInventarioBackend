using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto, int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return null;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = userId,
                Notes = dto.Notes,
                Status = "Pending",
                Total = 0,
                CreatedAt = DateTime.UtcNow
            };

            decimal total = 0;

            foreach (var item in dto.OrderItems)
            {
                var productProvider = await _context.ProductProviders
                    .Include(pp => pp.Product)
                    .Include(pp => pp.Provider)
                    .FirstOrDefaultAsync(pp =>
                        pp.ProductId == item.ProductId &&
                        pp.ProviderId == item.ProviderId);

                if (productProvider == null)
                    throw new InvalidOperationException(
                        $"No existe el producto {item.ProductId} con el proveedor {item.ProviderId}.");

                if (!productProvider.Product.IsActive)
                    throw new InvalidOperationException(
                        $"El producto '{productProvider.Product.Name}' no está disponible.");

                if (productProvider.Stock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Stock insuficiente para '{productProvider.Product.Name}' con '{productProvider.Provider.Name}'. Disponible: {productProvider.Stock}.");

                productProvider.Stock -= item.Quantity;
                productProvider.UpdatedAt = DateTime.UtcNow;

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProviderId = item.ProviderId,
                    Quantity = item.Quantity,
                    UnitPrice = productProvider.Price
                };

                order.OrderItems.Add(orderItem);
                total += item.Quantity * productProvider.Price;
            }

            order.Total = total;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await MapOrderAsync(order.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PagedResultDto<OrderSummaryDto>> GetUserOrdersAsync(int userId, int page, int size)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId);

        var totalRecords = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                Total = o.Total,
                Status = o.Status,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync();

        return new PagedResultDto<OrderSummaryDto>
        {
            TotalRecords = totalRecords,
            Items = items
        };
    }

    public async Task<OrderDto?> GetUserOrderByIdAsync(int userId, int orderId)
    {
        var orderExists = await _context.Orders
            .AnyAsync(o => o.Id == orderId && o.UserId == userId);

        if (!orderExists)
            return null;

        return await MapOrderAsync(orderId);
    }

    private async Task<OrderDto?> MapOrderAsync(int orderId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                Total = o.Total,
                Status = o.Status,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProviderId = oi.ProviderId,
                    ProviderName = oi.Provider.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}
