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

        public double? Goal { get; set; }

        public double? TotalCollected { get; set; }

        public double? TotalSpent { get; set; }
    }
}
