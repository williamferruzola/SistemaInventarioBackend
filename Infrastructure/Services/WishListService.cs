using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class WishListService : IWishListService
{
    private readonly ApplicationDbContext _context;

    public WishListService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WishListDto>> GetUserWishListAsync(int userId)
    {
        return await _context.WishLists
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
                .ThenInclude(p => p.Category)
            .AsNoTracking()
            .OrderByDescending(w => w.AddedAt)
            .Select(w => new WishListDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                ProductDescription = w.Product.Description,
                ProductImageUrl = w.Product.ImageUrl,
                CategoryName = w.Product.Category.Name,
                AddedAt = w.AddedAt
            })
            .ToListAsync();
    }

    public async Task<WishListDto?> AddToWishListAsync(int userId, int productId)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
            throw new InvalidOperationException($"El producto con ID {productId} no existe.");

        var alreadyInList = await _context.WishLists
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        if (alreadyInList)
            throw new InvalidOperationException("El producto ya está en la lista de deseos.");

        var wishListItem = new WishList
        {
            UserId = userId,
            ProductId = productId,
            AddedAt = DateTime.UtcNow
        };

        await _context.WishLists.AddAsync(wishListItem);
        await _context.SaveChangesAsync();

        return await _context.WishLists
            .Where(w => w.Id == wishListItem.Id)
            .Include(w => w.Product)
                .ThenInclude(p => p.Category)
            .Select(w => new WishListDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                ProductDescription = w.Product.Description,
                ProductImageUrl = w.Product.ImageUrl,
                CategoryName = w.Product.Category.Name,
                AddedAt = w.AddedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveFromWishListAsync(int userId, int productId)
    {
        var item = await _context.WishLists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item == null)
            return false;

        _context.WishLists.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
