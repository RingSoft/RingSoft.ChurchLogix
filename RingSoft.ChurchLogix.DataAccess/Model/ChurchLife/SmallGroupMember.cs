using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;

namespace RingSoft.ChurchLogix.DataAccess.Model.ChurchLife
{
    public class SmallGroupMember
    {
        [Required]
        public int SmallGroupId { get; set; }

        public virtual SmallGroup SmallGroup { get; set; }

        [Required]
        public int MemberId { get; set; }

        public virtual Member Member { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
