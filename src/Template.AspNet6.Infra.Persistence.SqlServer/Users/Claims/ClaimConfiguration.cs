using Template.AspNet6.Domain.Entities.Users.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.AspNet6.Infra.Persistence.SqlServer.Users.Claims;

public sealed class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ToTable("Users.Claims");

        #region INDEXES

        builder.HasKey(x => x.Id);
        builder.HasIndex(b => b.Provider);
        builder.HasIndex(b => b.Value);
        builder.HasIndex(b => b.ExpirationDate);
        builder.HasIndex(b => b.CreatedAt);
        builder.HasIndex(b => b.UpdatedAt);

        #endregion

        #region PROPERTIES

        builder.Property(b => b.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(b => b.Provider).HasMaxLength(32).IsRequired();
        builder.Property(b => b.Value).HasMaxLength(1024).IsRequired();
        builder.Property(b => b.ExpirationDate).IsRequired();
            
        #endregion

        builder.HasOne(b => b.User).WithMany().HasForeignKey(f => f.UserId);
    }
}
