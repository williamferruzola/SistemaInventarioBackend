namespace Application.DTOs;

public class WishListDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public string? ProductImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}

public class AddWishListDto
{
    public int ProductId { get; set; }
}
