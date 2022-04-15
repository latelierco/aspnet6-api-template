using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Domain.Entities.Roles;

public class Role : IInternalIdentifiable
{
    public const int NameMaxLength = 32;
    public const int TypesMaxLength = 128;

    public Role(string name, string types, Guid managerId)
    {
        Name = name;
        Types = types;
        ManagerId = managerId;
    }

    public int InternalId { get; set; }

    public string Name { get; set; }
    public string Types { get; set; }

    public Guid ManagerId { get; set; }
    public virtual User Manager { get; set; } = null!;

    public string[] GetTypes => Types.Split(',');
}
