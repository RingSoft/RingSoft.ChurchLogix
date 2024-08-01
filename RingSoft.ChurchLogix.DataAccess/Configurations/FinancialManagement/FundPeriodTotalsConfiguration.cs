using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.FinancialManagement
{
    public class FundPeriodTotalsConfiguration : IEntityTypeConfiguration<FundPeriodTotals>
    {
        public void Configure(EntityTypeBuilder<FundPeriodTotals> builder)
        {
            builder.Property(p => p.FundId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.PeriodType).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Date).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.TotalExpenses).HasColumnType(DbConstants.DecimalColumnType);
            builder.Property(p => p.TotalIncome).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasKey(p => new { p.FundId, p.PeriodType, p.Date });

            builder.HasOne(p => p.Fund)
                .WithMany(p => p.PeriodTotals)
                .HasForeignKey(p => p.FundId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
