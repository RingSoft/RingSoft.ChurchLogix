using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.MemberManagement
{
    public class MemberGiving
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }

        public virtual Member Member { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<MemberGivingDetails> Details { get; set; }

        public MemberGiving()
        {
            Details = new HashSet<MemberGivingDetails>();
        }
    }
}
