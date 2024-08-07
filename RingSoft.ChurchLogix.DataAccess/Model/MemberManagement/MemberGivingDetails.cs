using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;

namespace RingSoft.ChurchLogix.DataAccess.Model.MemberManagement
{
    public class MemberGivingDetails
    {
        [Required]
        public int MemberGivingId { get; set; }

        public virtual MemberGiving MemberGiving { get; set; }

        [Required]
        public int RowId { get; set; }

        [Required]
        public int FundId { get; set; }

        public virtual Fund Fund { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}
