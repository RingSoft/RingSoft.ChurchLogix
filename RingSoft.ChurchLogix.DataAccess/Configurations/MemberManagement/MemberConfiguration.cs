using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.MemberManagement
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Name).HasColumnType(DbConstants.StringColumnType);
            builder.Property(p => p.Email).HasColumnType(DbConstants.StringColumnType);
            builder.Property(p => p.PhoneNumber).HasColumnType(DbConstants.StringColumnType);
            builder.Property(p => p.HouseholdId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Notes).HasColumnType(DbConstants.MemoColumnType);

            builder.HasOne(p => p.Household)
                .WithMany(p => p.HouseholdMembers)
                .HasForeignKey(p => p.HouseholdId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
