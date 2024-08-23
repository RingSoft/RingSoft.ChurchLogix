using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.ChurchLife
{
    public class SmallGroup
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<SmallGroupMember> Members { get; set; }

        public SmallGroup()
        {
            Members = new HashSet<SmallGroupMember>();
        }
    }
}
