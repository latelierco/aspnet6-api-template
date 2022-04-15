using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.ValueObjects.Email;

namespace Template.AspNet6.Infra.Persistence.SqlServer.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.ConfigurePersistable();

        #region INDEXES

        builder.HasIndex(b => b.FirstName);
        builder.HasIndex(b => b.LastName);
        builder.HasIndex(b => b.Email).IsUnique();
        builder.HasIndex(b => b.Roles);
        builder.HasIndex(b => b.Plans);
        builder.HasIndex(b => b.IsActivated);
        builder.HasIndex(b => b.IsEmailVerified);
        builder.HasIndex(b => b.CreatedAt);
        builder.HasIndex(b => b.UpdatedAt);

        #endregion

        #region PROPERTIES

        builder.Property(b => b.FirstName).HasMaxLength(User.FirstNameMaxLength).IsRequired();
        builder.Property(b => b.LastName).HasMaxLength(User.LastNameMaxLength).IsRequired();
        builder.Property(b => b.Email).HasConversion(v => v.Value, v => new Email(v)).HasColumnName("EmailAddress").HasMaxLength(User.EmailMaxLength).IsRequired();
        builder.Property(b => b.ProfilePicture).HasMaxLength(User.ProfilePictureMaxLength).IsRequired(false);
        builder.Property(b => b.Roles).HasMaxLength(User.RolesMaxLength);
        builder.Property(b => b.Plans).HasMaxLength(User.PlansMaxLength);
        builder.Property(b => b.IsActivated).IsRequired();
        builder.Property(b => b.IsEmailVerified).IsRequired();

        builder.Property(b => b.LastConnectionAt).IsRequired(false);

        #endregion
    }
}
