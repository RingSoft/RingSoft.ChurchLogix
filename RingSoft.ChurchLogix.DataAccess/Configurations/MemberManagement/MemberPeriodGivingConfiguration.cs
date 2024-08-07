using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.MemberManagement
{
    public class MemberPeriodGivingConfiguration : IEntityTypeConfiguration<MemberPeriodGiving>
    {
        public void Configure(EntityTypeBuilder<MemberPeriodGiving> builder)
        {
            builder.Property(p => p.MemberId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.PeriodType).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Date).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.TotalGiving).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasKey(p => new { p.MemberId, p.PeriodType, p.Date });

            builder.HasOne(p => p.Member)
                .WithMany(p => p.PeriodGiving)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
