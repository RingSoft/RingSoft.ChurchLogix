﻿using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup.Lookup;

namespace RingSoft.ChurchLogix.DataAccess
{
    public class SystemPreferencesLookup
    {
        public int Id { get; set; }
    }
    public class StaffLookup
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
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

    public class MemberLookup
    {
        public string Name { get; set; }
    }

    public class MemberGivingLookup
    {
        public string Member { get; set; }

        public DateTime Date { get; set; }
    }

    public class MemberGivingDetailsLookup
    {
        public string Member { get; set; }

        public string Fund { get; set; }

        public double Amount { get; set; }
    }

    public class MemberGivingHistoryLookup
    {
        public string Member { get; set; }

        public string Fund { get; set; }

        public DateTime Date { get; set; }

        public double Amount { get; set; }
    }

    public class MemberPeriodGivingLookup
    {
        public string Member { get; set; }

        public DateTime Date { get; set; }

        public int PeriodType { get; set; }

        public double Total { get; set; }
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

    public class BudgetLookup
    {
        public string Name { get; set; }

        public double Amount { get; set; }
    }

    public class FundHistoryLookup
    {
        public string Fund { get; set; }

        public string Budget { get; set; }

        public DateTime Date { get; set; }

        public int AmountType { get; set; }

        public double Amount { get; set; }
    }

    public class FundPeriodLookup
    {
        public string Fund { get; set; }

        public DateTime Date { get; set; }

        public int PeriodType { get; set; }

        public double TotalIncome { get; set; }

        public double TotalExpenses { get; set; }

        public double Difference { get; set; }
    }

    public class FundPeriodDifferenceFormula : ILookupFormula
    {
        public int Id => 2;

        public string GetDatabaseValue(object entity)
        {
            if (entity is FundPeriodTotals fund)
            {
                return (fund.TotalIncome - fund.TotalExpenses).ToString();
            }

            return string.Empty;
        }
    }

    public class BudgetPeriodTotalsLookup
    {
        public string BudgetItem { get; set; }

        public DateTime Date { get; set; }

        public int PeriodType { get; set; }

        public double Total { get; set; }
    }

    public class BudgetActualsLookup
    {
        public string Budget { get; set; }

        public DateTime Date { get; set; }

        public double Amount { get; set; }
    }

    public class EventLookup
    {
        public string Name { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class EventMemberLookup
    {
        public string Event { get; set; }

        public string Member { get; set; }

        public double AmountPaid { get; set; }
    }

    public class RoleLookup
    {
        public string Name { get; set; }
    }
    public class SmallGroupLookup
    {
        public string Name { get; set; }
    }

    public class SmallGroupMemberLookup
    {
        public string SmallGroup { get; set; }

        public string Member { get; set; }

        public string Role { get; set; }
    }
}
