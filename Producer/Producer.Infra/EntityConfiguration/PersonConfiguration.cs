using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Producer.Infra.EntityConfiguration;

public class PersonConfiguration: IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(_ => _.Id);
        builder.Property(_ => _.FirstName).HasMaxLength(250).IsRequired();
        builder.Property(_ => _.LastName).HasMaxLength(350).IsRequired();
        builder.Property(_ => _.Email).HasMaxLength(100).IsRequired();
    }
}