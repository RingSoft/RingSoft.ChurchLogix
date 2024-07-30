using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup.Lookup;

namespace RingSoft.ChurchLogix.DataAccess
{
    public class StaffLookup
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }

    public class MemberLookup
    {
        public string Name { get; set; }
    }

    public class GroupsLookup
    {
        public string Name { get; set; }
    }

    public class StaffGroupsLookup
    {
        public string StaffPerson { get; set; }

        public string Group { get; set; }
    }

    public class FundLookup
    {
        public string Description { get; set; }

        public double TotalCollected { get; set; }

        public double TotalSpent { get; set; }

        public double Difference { get; set; }
    }

    public class FundsDifferenceFormula : ILookupFormula
    {
        public int Id => 1;

        public string GetDatabaseValue(object entity)
        {
            if (entity is Fund fund)
            {
                return (fund.TotalCollected - fund.TotalSpent).ToString();
            }

            return string.Empty;
        }
    }
}
