using Application.DTOs;
using Application.DTOs.Catalog;

namespace Application.Interfaces;

public interface ICatalogService
{
    Task<PagedResultDto<CatalogProductListDto>> GetProductsAsync(int page, int size, string? search);
    Task<CatalogProductDetailDto?> GetProductByIdAsync(int id);
}
