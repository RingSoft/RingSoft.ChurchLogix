using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.MemberManagement
{
    public class MemberGivingHistoryConfiguration : IEntityTypeConfiguration<MemberGivingHistory>
    {
        public void Configure(EntityTypeBuilder<MemberGivingHistory> builder)
        {
            builder.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.MemberId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Date).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.FundId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Amount).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasOne(p => p.Member)
                .WithMany(p => p.GivingHistory)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Fund)
                .WithMany(p => p.MemberGivingHistory)
                .HasForeignKey(p => p.FundId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
