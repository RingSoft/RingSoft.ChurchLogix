using Microsoft.EntityFrameworkCore;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.DataAccess
{
    public interface IChurchLogixDbContext : IDbContext
    {
        DbContext DbContext { get; }

        DbSet<SystemMaster> SystemMaster { get; set; }

        DbSet<SystemPreferences> SystemPreferences { get; set; }

        DbSet<StaffPerson> Staff { get; set; }

        DbSet<Member> Members { get; set; }

        DbSet<MemberGivingHistory> MembersGivingHistory { get; set; }

        DbSet<MemberPeriodGiving> MembersPeriodGiving { get; set; }

        DbSet<MemberGiving> MembersGiving { get; set; }

        DbSet<MemberGivingDetails> MembersGivingDetails { get; set; }

        DbSet<Group> Groups { get; set; }

        DbSet<StaffGroup> StaffGroups { get; set; }

        DbSet<Fund> Funds { get; set; }

        DbSet<BudgetItem> Budgets { get; set; }

        DbSet<FundHistory> FundHistory { get; set; }

        DbSet<FundPeriodTotals> FundPeriodTotals { get; set; }

        DbSet<BudgetPeriodTotals> BudgetsPeriodTotals { get; set; }

        DbSet<BudgetActual> BudgetActuals { get; set; }

        void SetLookupContext(ChurchLogixLookupContext lookupContext);
    }
}
