using System.ComponentModel.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.ChurchLife
{
    public class EventMemberConfiguration : IEntityTypeConfiguration<EventMember>
    {
        public void Configure(EntityTypeBuilder<EventMember> builder)
        {
            builder.Property(p => p.EventId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.MemberId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.AmountPaid).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasKey(p => new {p.EventId, p.MemberId});

            builder.HasOne(p => p.Event)
                .WithMany(p => p.Members)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Member)
                .WithMany(p => p.EventMembers)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
