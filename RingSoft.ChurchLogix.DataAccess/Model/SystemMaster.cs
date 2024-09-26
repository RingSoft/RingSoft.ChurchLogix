using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model
{
    public class SystemMaster
    {
        [Required]
        [Key]
        [MaxLength(50)]
        public string ChurchName { get; set; }

        [MaxLength(50)]
        [Required]
        public string AppGuid { get; set; }
    }
}
