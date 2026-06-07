using Application.DTOs;

namespace Application.Interfaces;

public interface IProductProviderService
{
    Task<int> AddProductProviderAsync(ProductProviderDto dto);
    Task<PagedResultDto<ProductProviderDto>> GetProductProvidersAsync(int? productId, int page, int size, string? search);
    Task<ProductProviderDto?> GetProductProviderByIdAsync(int id);
    Task<bool> UpdateProductProviderAsync(int id, ProductProviderDto dto);
    Task<bool> DeleteProductProviderAsync(int id);
}
