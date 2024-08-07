using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.MemberManagement
{
    public class MemberGivingDetailsConfiguration : IEntityTypeConfiguration<MemberGivingDetails>
    {
        public void Configure(EntityTypeBuilder<MemberGivingDetails> builder)
        {
            builder.Property(p => p.MemberGivingId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.RowId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.FundId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Amount).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasKey(p => new { p.MemberGivingId, p.RowId });

            builder.HasOne(p => p.MemberGiving)
                .WithMany(p => p.Details)
                .HasForeignKey(p => p.MemberGivingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Fund)
                .WithMany(p => p.GivingDetails)
                .HasForeignKey(p => p.FundId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
