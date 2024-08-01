using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.Financial_Management
{
    public class BudgetPeriodTotals
    {
        [Required]
        public int BudgetId { get; set; }

        public virtual BudgetItem BudgetItem { get; set; }

        [Required]
        public int PeriodType { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [DefaultValue(0)]
        public double Total { get; set; }
    }
}
