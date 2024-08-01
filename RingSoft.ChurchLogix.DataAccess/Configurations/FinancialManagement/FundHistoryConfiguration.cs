using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.FinancialManagement
{
    public class FundHistoryConfiguration : IEntityTypeConfiguration<FundHistory>
    {
        public void Configure(EntityTypeBuilder<FundHistory> builder)
        {
            builder.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.FundId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.BudgetId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.AmountType).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Amount).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasOne(p => p.Fund)
                .WithMany(p => p.History)
                .HasForeignKey(p => p.FundId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(p => p.BudgetItem)
                .WithMany(p => p.History)
                .HasForeignKey(p => p.BudgetId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
