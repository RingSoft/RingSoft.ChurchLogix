using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;

namespace RingSoft.ChurchLogix.DataAccess.Model.Financial_Management
{
    public enum HistoryAmountTypes
    {
        Income = 0,
        Expense = 1,
    }
    public class FundHistory
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public int? FundId { get; set; }

        public virtual Fund Fund { get; set; }

        public int? BudgetId { get; set; }

        public virtual BudgetItem BudgetItem { get; set; }

        public int AmountType { get; set; }

        public DateTime Date { get; set; }

        public double Amount { get; set; }
    }
}
