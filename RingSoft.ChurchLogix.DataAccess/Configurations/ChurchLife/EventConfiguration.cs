using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.ChurchLife
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Name).HasColumnType(DbConstants.StringColumnType);
            builder.Property(p => p.BeginDate).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.EndDate).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.MemberCost).HasColumnType(DbConstants.DecimalColumnType);
            builder.Property(p => p.TotalCost).HasColumnType(DbConstants.DecimalColumnType);
            builder.Property(p => p.TotalPaid).HasColumnType(DbConstants.DecimalColumnType);
            builder.Property(p => p.Notes).HasColumnType(DbConstants.MemoColumnType);
        }
    }
}
