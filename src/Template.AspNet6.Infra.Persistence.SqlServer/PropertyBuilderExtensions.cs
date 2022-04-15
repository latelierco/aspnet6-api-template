using Template.AspNet6.Domain.ValueObjects.Email;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.AspNet6.Infra.Persistence.SqlServer;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<Email> HasEmailConversion(this PropertyBuilder<Email> propertyBuilder)
        => propertyBuilder
            .HasConversion(email => email.Value, v => new Email(v))
            .HasMaxLength(Email.EmailMaxLength);
}