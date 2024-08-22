using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;

namespace RingSoft.ChurchLogix.DataAccess.Model.ChurchLife
{
    public class EventMember
    {
        [Required]
        public int EventId { get; set; }

        public virtual Event Event { get; set; }

        [Required]
        public int MemberId { get; set; }

        public virtual Member Member { get; set; }

        public double? AmountPaid { get; set; }
    }
}
