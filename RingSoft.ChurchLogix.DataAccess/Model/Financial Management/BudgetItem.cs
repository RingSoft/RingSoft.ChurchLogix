using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace RingSoft.ChurchLogix.DataAccess.Model.Financial_Management
{
    public class BudgetItem
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int FundId { get; set; }

        public virtual Fund Fund { get; set; }

        [Required]
        public double Amount { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<FundHistory> History { get; set; }

        public BudgetItem()
        {
            History = new HashSet<FundHistory>();
        }
    }
}
