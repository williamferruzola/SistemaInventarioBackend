using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductProviderService : IProductProviderService
{
    private readonly ApplicationDbContext _context;

    public ProductProviderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddProductProviderAsync(ProductProviderDto dto)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
        if (!productExists)
            throw new InvalidOperationException($"El producto con ID {dto.ProductId} no existe.");

        var providerExists = await _context.Providers.AnyAsync(p => p.Id == dto.ProviderId);
        if (!providerExists)
            throw new InvalidOperationException($"El proveedor con ID {dto.ProviderId} no existe.");

        var alreadyLinked = await _context.ProductProviders
            .AnyAsync(pp => pp.ProductId == dto.ProductId && pp.ProviderId == dto.ProviderId);
        
        if (alreadyLinked)
            throw new InvalidOperationException("Este proveedor ya está asociado a este producto.");

        var productProvider = new ProductProvider
        {
            ProductId = dto.ProductId,
            ProviderId = dto.ProviderId,
            Price = dto.Price,
            Stock = dto.Stock,
            BatchNumber = dto.BatchNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ProductProviders.AddAsync(productProvider);
        await _context.SaveChangesAsync();
        return productProvider.Id;
    }

    public async Task<PagedResultDto<ProductProviderDto>> GetProductProvidersAsync(
        int? productId, int page, int size, string? search)
    {
        var query = _context.ProductProviders
            .Include(pp => pp.Provider)
            .Include(pp => pp.Product)
            .AsNoTracking()
            .AsQueryable();

        if (productId.HasValue && productId > 0)
            query = query.Where(pp => pp.ProductId == productId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(pp =>
                pp.Provider.Name.ToLower().Contains(term) ||
                pp.Product.Name.ToLower().Contains(term) ||
                (pp.BatchNumber != null && pp.BatchNumber.ToLower().Contains(term)));
        }

        var totalRecords = await query.CountAsync();

        var items = await query
            .OrderByDescending(pp => pp.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(pp => new ProductProviderDto
            {
                Id = pp.Id,
                ProductId = pp.ProductId,
                ProductName = pp.Product.Name,
                ProviderId = pp.ProviderId,
                ProviderName = pp.Provider.Name,
                Price = pp.Price,
                Stock = pp.Stock,
                BatchNumber = pp.BatchNumber,
                IsActive = pp.IsActive
            })
            .ToListAsync();

        return new PagedResultDto<ProductProviderDto>
        {
            TotalRecords = totalRecords,
            Items = items
        };
    }

    public async Task<ProductProviderDto?> GetProductProviderByIdAsync(int id)
    {
        var productProvider = await _context.ProductProviders
            .Include(pp => pp.Provider)
            .AsNoTracking()
            .FirstOrDefaultAsync(pp => pp.Id == id);

        if (productProvider == null)
            return null;

        return new ProductProviderDto
        {
            Id = productProvider.Id,
            ProductId = productProvider.ProductId,
            ProviderId = productProvider.ProviderId,
            ProviderName = productProvider.Provider.Name,
            Price = productProvider.Price,
            Stock = productProvider.Stock,
            BatchNumber = productProvider.BatchNumber,
            IsActive = productProvider.IsActive
        };
    }

    public async Task<bool> UpdateProductProviderAsync(int id, ProductProviderDto dto)
    {
        var productProvider = await _context.ProductProviders.FindAsync(id);
        if (productProvider == null)
            return false;

        var providerExists = await _context.Providers.AnyAsync(p => p.Id == dto.ProviderId);
        if (!providerExists)
            throw new InvalidOperationException($"El proveedor con ID {dto.ProviderId} no existe.");

        productProvider.ProviderId = dto.ProviderId;
        productProvider.Price = dto.Price;
        productProvider.Stock = dto.Stock;
        productProvider.BatchNumber = dto.BatchNumber;
        productProvider.IsActive = dto.IsActive;
        productProvider.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductProviderAsync(int id)
    {
        var productProvider = await _context.ProductProviders.FindAsync(id);
        if (productProvider == null)
            return false;

        productProvider.IsActive = false;
        productProvider.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
