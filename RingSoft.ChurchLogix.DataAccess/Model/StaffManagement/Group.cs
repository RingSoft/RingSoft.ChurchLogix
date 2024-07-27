using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.StaffManagement
{
    public class Group
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string? Rights { get; set; }

        public virtual ICollection<StaffGroup> StaffGroups { get; set; }

        public Group()
        {
            StaffGroups = new HashSet<StaffGroup>();
        }

    }
}
