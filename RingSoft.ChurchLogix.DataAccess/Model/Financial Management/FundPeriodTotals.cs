using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.Financial_Management
{
    public enum PeriodTypes
    {
        [Description("Monthly")]
        MonthEnding = 0,
        [Description("Yearly")]
        YearEnding = 1,
    }
    public class FundPeriodTotals
    {
        [Required]
        public int FundId { get; set; }

        public virtual Fund Fund { get; set; }

        [Required]
        public int PeriodType { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [DefaultValue((0))]
        public double TotalIncome { get; set; }

        [DefaultValue((0))]
        public double TotalExpenses { get; set;}
    }
}
