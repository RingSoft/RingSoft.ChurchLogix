using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.StaffManagement
{
    public class StaffGroup
    {
        [Required]
        [Key]
        public int StaffPersonId { get; set; }

        public virtual StaffPerson StaffPerson { get; set; }

        [Required]
        [Key]
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
