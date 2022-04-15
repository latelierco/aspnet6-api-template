namespace Template.AspNet6.Domain.Entities.Users.Claims;

public class Claim : ITimestampEntity
{
    public Claim(Guid userId, string provider, string value, DateTime expirationDate)
    {
        UserId = userId;
        Provider = provider;
        Value = value;
        ExpirationDate = expirationDate;
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public string Provider { get; set; }
    public string Value { get; set; }
    public DateTime ExpirationDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsExpired => ExpirationDate < DateTime.UtcNow;
}
