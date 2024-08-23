using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.ChurchLife
{
    public class Role
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<SmallGroupMember> SmallGroupMembers { get; set; }

        public Role()
        {
            SmallGroupMembers = new HashSet<SmallGroupMember>();
        }
    }
}
