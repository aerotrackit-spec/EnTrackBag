using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;
public class SuspectBagEntityConfiguration : IEntityTypeConfiguration<SuspectBagEntity>
{
    public void Configure(EntityTypeBuilder<SuspectBagEntity> e)
    {
        e.ToTable("SuspectBags", "dbo");
        e.HasKey(x => x.ID);
    }
}
