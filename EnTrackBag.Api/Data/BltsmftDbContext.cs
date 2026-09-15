using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace EnTrackBag.Api.Data;
public class BltsmftDbContext : DbContext
{
    public BltsmftDbContext(DbContextOptions<BltsmftDbContext> options) : base(options) { }
    public DbSet<ReaderEntity> Readers => Set<ReaderEntity>(); public DbSet<AntennaEntity> Antennas=>Set<AntennaEntity>(); public DbSet<ControllerEntity> Controllers=>Set<ControllerEntity>(); public DbSet<LogicalDeviceEntity> LogicalDevices=>Set<LogicalDeviceEntity>(); public DbSet<LogicalDeviceMapEntity> LogicalDeviceMaps=>Set<LogicalDeviceMapEntity>(); public DbSet<ReaderControllerMapEntity> ReaderControllerMaps=>Set<ReaderControllerMapEntity>(); public DbSet<SuspectBagEntity> SuspectBags=>Set<SuspectBagEntity>(); public DbSet<AlarmListEntity> AlarmList=>Set<AlarmListEntity>(); public DbSet<SystemSettingEntity> SystemSettings=>Set<SystemSettingEntity>(); public DbSet<EnTrackBagExceptionEntity> EnTrackBagExceptions=>Set<EnTrackBagExceptionEntity>();
    protected override void OnModelCreating(ModelBuilder modelBuilder){ base.OnModelCreating(modelBuilder); modelBuilder.ApplyConfigurationsFromAssembly(typeof(BltsmftDbContext).Assembly); }
}
