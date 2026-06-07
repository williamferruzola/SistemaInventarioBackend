using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddProductAsync(ProductDto productDto)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
        if (!categoryExists)
            throw new InvalidOperationException($"La categoría con ID {productDto.CategoryId} no existe.");

        var product = new Product
        {
            CategoryId = productDto.CategoryId,
            Name = productDto.Name.Trim(),
            Description = productDto.Description,
            ImageUrl = productDto.ImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    public async Task<PagedResultDto<ProductDto>> GetProductsAsync(int page, int size, string? search)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .AsQueryable();

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
            .Select(p => new ProductDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return new PagedResultDto<ProductDto>
        {
            TotalRecords = totalRecords,
            Items = items
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };
    }

    public async Task<bool> UpdateProductAsync(int id, ProductDto productDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
        if (!categoryExists)
            throw new InvalidOperationException($"La categoría con ID {productDto.CategoryId} no existe.");

        product.CategoryId = productDto.CategoryId;
        product.Name = productDto.Name.Trim();
        product.Description = productDto.Description;
        product.ImageUrl = productDto.ImageUrl;
        product.IsActive = productDto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
