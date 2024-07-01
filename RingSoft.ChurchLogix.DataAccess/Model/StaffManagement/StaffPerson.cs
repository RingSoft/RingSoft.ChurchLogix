using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;

namespace RingSoft.ChurchLogix.DataAccess.Model.StaffManagement
{
    public class StaffPerson
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int? MemberId { get; set; }

        public virtual Member Member { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; }

        public string? Rights { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? PhoneNumber { get; set; }

        public string? Notes { get; set; }

    }
}
