namespace Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
}
