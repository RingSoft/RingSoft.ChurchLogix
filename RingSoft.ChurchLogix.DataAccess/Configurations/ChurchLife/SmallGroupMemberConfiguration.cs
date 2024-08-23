using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.ChurchLife
{
    public class SmallGroupMemberConfiguration : IEntityTypeConfiguration<SmallGroupMember>
    {
        public void Configure(EntityTypeBuilder<SmallGroupMember> builder)
        {
            builder.Property(p => p.SmallGroupId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.MemberId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.RoleId).HasColumnType(DbConstants.IntegerColumnType);

            builder.HasKey(p => new { p.SmallGroupId, p.MemberId });

            builder.HasOne(p => p.SmallGroup)
                .WithMany(p => p.Members)
                .HasForeignKey(p => p.SmallGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Member)
                .WithMany(p => p.SmallGroupMembers)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Role)
                .WithMany(p => p.SmallGroupMembers)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
