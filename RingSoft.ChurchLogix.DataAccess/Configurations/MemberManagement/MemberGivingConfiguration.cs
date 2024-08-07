using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.MemberManagement
{
    public class MemberGivingConfiguration : IEntityTypeConfiguration<MemberGiving>
    {
        public void Configure(EntityTypeBuilder<MemberGiving> builder)
        {
            builder.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.MemberId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Date).HasColumnType(DbConstants.DateColumnType);

            builder.HasOne(p => p.Member)
                .WithMany(p => p.Giving)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
