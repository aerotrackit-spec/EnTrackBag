using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;
public class SystemSettingEntityConfiguration : IEntityTypeConfiguration<SystemSettingEntity>
{
    public void Configure(EntityTypeBuilder<SystemSettingEntity> e)
    {
        e.ToTable("SystemSettings", "dbo");
        e.HasKey(x => x.ID);
    }
}
