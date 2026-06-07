namespace Domain.Entities;

public class WishList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; }

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
