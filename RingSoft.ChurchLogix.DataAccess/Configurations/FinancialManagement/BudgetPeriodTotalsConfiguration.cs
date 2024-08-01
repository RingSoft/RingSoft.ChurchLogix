using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.FinancialManagement
{
    public class BudgetPeriodTotalsConfiguration : IEntityTypeConfiguration<BudgetPeriodTotals>
    {
        public void Configure(EntityTypeBuilder<BudgetPeriodTotals> builder)
        {
            builder.Property(p => p.BudgetId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.PeriodType).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Date).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.Total).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasKey(p => new { p.BudgetId, p.PeriodType, p.Date });

            builder.HasOne(p => p.BudgetItem)
                .WithMany(p => p.PeriodTotals)
                .HasForeignKey(p => p.BudgetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
