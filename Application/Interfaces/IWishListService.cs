using Application.DTOs;

namespace Application.Interfaces;

public interface IWishListService
{
    Task<List<WishListDto>> GetUserWishListAsync(int userId);
    Task<WishListDto?> AddToWishListAsync(int userId, int productId);
    Task<bool> RemoveFromWishListAsync(int userId, int productId);
}
