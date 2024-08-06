using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;

namespace RingSoft.ChurchLogix.DataAccess.Model.MemberManagement
{
    public class MemberGivingHistory
    {
        [Required]
        [Key]
        public int Id { get; set;}

        [Required]
        public int MemberId { get; set;}

        public virtual Member Member { get; set;}

        [Required]
        public DateTime Date { get; set;}

        [Required]
        public int FundId { get; set;}

        public virtual Fund Fund { get; set;}

        [Required]
        public double Amount { get; set;}
    }
}
