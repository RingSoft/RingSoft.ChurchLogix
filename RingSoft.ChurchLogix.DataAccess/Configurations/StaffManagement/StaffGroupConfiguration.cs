using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.StaffManagement
{
    public class StaffGroupConfiguration : IEntityTypeConfiguration<StaffGroup>
    {
        public void Configure(EntityTypeBuilder<StaffGroup> builder)
        {
            builder.Property(p => p.StaffPersonId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.GroupId).HasColumnType(DbConstants.IntegerColumnType);

            builder.HasKey(p => new {p.StaffPersonId, p.GroupId});

            builder.HasOne(p => p.StaffPerson)
                .WithMany(p => p.Groups)
                .HasForeignKey(p => p.StaffPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Group)
                .WithMany(p => p.StaffGroups)
                .HasForeignKey(p => p.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
