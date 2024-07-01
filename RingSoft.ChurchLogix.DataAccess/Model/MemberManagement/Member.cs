using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;

namespace RingSoft.ChurchLogix.DataAccess.Model.MemberManagement
{
    public class Member
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<StaffPerson> Staff { get; set; }

        public Member()
        {
            Staff = new HashSet<StaffPerson>();
        }
    }
}
