using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;

namespace RingSoft.ChurchLogix.DataAccess.Model.MemberManagement
{
    public enum MemberSpecialRights
    {
        AllowViewGiving = 1,
    }

    public class Member
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? PhoneNumber { get; set; }

        public int? HouseholdId { get; set; }

        public virtual Member Household { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<StaffPerson> Staff { get; set; }
        public virtual ICollection<Member> HouseholdMembers { get; set; }
        public virtual ICollection<MemberGivingHistory> GivingHistory { get; set; }
        public virtual ICollection<MemberPeriodGiving> PeriodGiving { get; set; }
        public virtual ICollection<MemberGiving> Giving { get; set; }
        public virtual ICollection<EventMember> EventMembers { get; set; }
        public virtual ICollection<SmallGroupMember> SmallGroupMembers { get; set; }

        public Member()
        {
            Staff = new HashSet<StaffPerson>();
            HouseholdMembers = new HashSet<Member>();
            GivingHistory = new HashSet<MemberGivingHistory>();
            PeriodGiving = new HashSet<MemberPeriodGiving>();
            Giving = new HashSet<MemberGiving>();
            EventMembers = new HashSet<EventMember>();
            SmallGroupMembers = new HashSet<SmallGroupMember>();
        }
    }
}
