namespace Application.DTOs;

public class PagedResultDto<T>
{
    public int TotalRecords { get; set; }
    public List<T> Items { get; set; } = new();
}
