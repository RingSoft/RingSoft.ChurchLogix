using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.Financial_Management
{
    public class Fund
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [DefaultValue(0)]
        public double Goal { get; set; }

        [DefaultValue(0)]
        public double TotalCollected { get; set; }

        [DefaultValue(0)]
        public double TotalSpent { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<BudgetItem> Budgets { get; set; }

        public Fund()
        {
            Budgets  = new HashSet<BudgetItem>();
        }
    }
}
