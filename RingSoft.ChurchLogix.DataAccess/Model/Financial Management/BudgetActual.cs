using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.Financial_Management
{
    public class BudgetActual
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public int BudgetId { get;}

        public virtual BudgetItem Budget { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}
