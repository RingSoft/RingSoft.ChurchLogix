using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model
{
    public class SystemPreferences
    {
        [Required]
        [Key]
        public int Id { get; set; }
    }
}