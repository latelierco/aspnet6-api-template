using Template.AspNet6.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.AspNet6.Infra.Persistence.SqlServer;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigurePersistable<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IPersistableEntity
        => builder.ConfigureIdentifiable().ConfigureInternalIdentifiable();

    public static EntityTypeBuilder<TEntity> ConfigureInternalIdentifiable<TEntity>(this EntityTypeBuilder<TEntity> builder, bool isKey = false) where TEntity : class, IInternalIdentifiable
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (isKey)
        {
            builder.HasKey(b => b.InternalId).IsClustered();
            builder.Property(b => b.InternalId).UseIdentityColumn();
        }
        else
        {
            builder.HasIndex(b => b.InternalId).IsUnique().IsClustered();
            var prop = builder.Property(b => b.InternalId).UseIdentityColumn().IsRequired();
            prop.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ConfigureIdentifiable<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IIdentifiable
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.Property(b => b.Id).HasDefaultValueSql("newid()").IsRequired();

        return builder;
    }
}
