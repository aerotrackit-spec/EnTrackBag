using Identity.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Identity.Api.Data.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> e)
    {
        e.ToTable("Users", "dbo");
        e.HasKey(x => x.Id);
        e.Property(x => x.UserName).HasMaxLength(256).IsRequired();
        e.Property(x => x.DisplayName).HasMaxLength(256);
        e.Property(x => x.PasswordHash).HasMaxLength(1000).IsRequired();
        e.HasIndex(x => x.UserName).IsUnique();
    }
}
