namespace Template.AspNet6.Domain.Entities;

public interface ITimestampEntity
{
    DateTime CreatedAt { get; set; }

    DateTime UpdatedAt { get; set; }
}
