using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model
{
    public class SystemPreferences
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public DateTime? FiscalYearStart { get; set; }

        public DateTime? FiscalYearEnd { get; set; }
    }
}