using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess.Configurations.FinancialManagement
{
    public class BudgetActualConfiguration : IEntityTypeConfiguration<BudgetActual>
    {
        public void Configure(EntityTypeBuilder<BudgetActual> builder)
        {
            builder.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.BudgetId).HasColumnType(DbConstants.IntegerColumnType);
            builder.Property(p => p.Date).HasColumnType(DbConstants.DateColumnType);
            builder.Property(p => p.Amount).HasColumnType(DbConstants.DecimalColumnType);

            builder.HasOne(p => p.Budget)
                .WithMany(p => p.Actuals)
                .HasForeignKey(p => p.BudgetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
