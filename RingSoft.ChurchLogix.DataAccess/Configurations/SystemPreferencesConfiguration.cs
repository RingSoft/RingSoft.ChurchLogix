using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations
{
    public class SystemPreferencesConfiguration : IEntityTypeConfiguration<SystemPreferences>
    {
        public void Configure(EntityTypeBuilder<SystemPreferences> builder)
        {
            builder.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.FiscalYearStart).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.FiscalYearEnd).HasColumnType(DbConstants.DateColumnType);
        }
    }
}
