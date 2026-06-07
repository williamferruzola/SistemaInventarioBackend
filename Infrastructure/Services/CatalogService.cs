using Application.DTOs;
using Application.DTOs.Catalog;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly ApplicationDbContext _context;

    public CatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<CatalogProductListDto>> GetProductsAsync(int page, int size, string? search)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.ProductProviders.Any(pp => pp.Stock > 0));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)));
        }

        var totalRecords = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new CatalogProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                MinPrice = p.ProductProviders.Where(pp => pp.Stock > 0).Min(pp => pp.Price),
                TotalStock = p.ProductProviders.Where(pp => pp.Stock > 0).Sum(pp => pp.Stock)
            })
            .ToListAsync();

        return new PagedResultDto<CatalogProductListDto>
        {
            TotalRecords = totalRecords,
            Items = items
        };
    }

    public async Task<CatalogProductDetailDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id && p.ProductProviders.Any(pp => pp.Stock > 0))
            .Select(p => new CatalogProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                MinPrice = p.ProductProviders.Where(pp => pp.Stock > 0).Min(pp => pp.Price),
                TotalStock = p.ProductProviders.Where(pp => pp.Stock > 0).Sum(pp => pp.Stock),
                Offers = p.ProductProviders
                    .Where(pp => pp.Stock > 0)
                    .OrderBy(pp => pp.Price)
                    .Select(pp => new CatalogOfferDto
                    {
                        ProviderId = pp.ProviderId,
                        ProviderName = pp.Provider.Name,
                        Price = pp.Price,
                        Stock = pp.Stock
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return product;
    }
}
