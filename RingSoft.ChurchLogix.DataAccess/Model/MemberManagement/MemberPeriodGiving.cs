using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.MemberManagement
{
    public class MemberPeriodGiving
    {
        [Required]
        public int MemberId { get; set; }

        public virtual Member Member { get; set; }

        [Required]
        public int PeriodType { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public double TotalGiving { get; set; }
    }
}
