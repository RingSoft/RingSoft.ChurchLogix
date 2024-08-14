using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.ChurchLogix.Library;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.ChurchLogix.Sqlite;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Testing;
using RingSoft.DbMaintenance;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace RingSoft.ChurchLogix.Tests
{
    public enum TestStaff
    {
        Master = 1,
    }

    public enum TestFunds
    {
        FundA = 1,
        FundB = 2,
    }

    public enum TestBudgets
    {
        BudgetA = 1,
        BudgetB = 2,
    }

    public enum TestBudgetActuals
    {
        First = 1,
        Second = 2,
    }

    public enum TestMembers
    {
        MemberA = 1,
        MemberB = 2,
    }
    public class TestGlobals<TViewModel, TView> : DbMaintenanceTestGlobals<TViewModel, TView>
        where TViewModel : DbMaintenanceViewModelBase
        where TView : IDbMaintenanceView, new()
    {
        public new ChurchLogixTestDataRepository DataRepository { get; }

        private bool _initRun = false;

        public TestGlobals() : base(new ChurchLogixTestDataRepository(new TestDataRegistry()))
        {
            DataRepository = base.DataRepository as ChurchLogixTestDataRepository;
        }

        public override void Initialize()
        {
            AppGlobals.UnitTesting = true;
            AppGlobals.Initialize();
            AppGlobals.LookupContext.Initialize(new ChurchLogixSqliteDbContext(), DbPlatforms.Sqlite);
            AppGlobals.LoggedInStaffPerson = new StaffPerson();
            AppGlobals.MainViewModel = new MainViewModel();
            SystemGlobals.Rights = new AppRights(new ChurchLogixRights());
            
            AppGlobals.SetupSystem(new Church());

            base.Initialize();
        }

        public override void ClearData()
        {
            AppGlobals.LoggedInStaffPerson = new StaffPerson();
            SystemGlobals.Rights = new AppRights(new ChurchLogixRights());

            base.ClearData();
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            var fundA = new Fund()
            {
                Id = (int)TestFunds.FundA,
                Description = "FundA",
            };
            context.SaveEntity(fundA, "Saving Fund");

            var fundB = new Fund()
            {
                Id = (int)TestFunds.FundB,
                Description = "FundB"
            };
            context.SaveEntity(fundB, "Saving Fund");

            var budgetA = new BudgetItem()
            {
                Id = (int)TestBudgets.BudgetA,
                Name = "BudgetA",
                FundId = fundA.Id,
                Amount = 925,
            };
            context.SaveEntity(budgetA, "Saving Budget");

            var budgetB = new BudgetItem()
            {
                Id = (int)TestBudgets.BudgetB,
                Name = "BudgetB",
                FundId = fundA.Id,
                Amount = 930,
            };

            context.SaveEntity(budgetB, "Saving Budget");

            var memberA = new Member()
            {
                Id = (int)TestMembers.MemberA,
                Name = "MemberA",
            };

            context.SaveEntity(memberA, "Saving Member");

            var memberB = new Member()
            {
                Id = (int)TestMembers.MemberB,
                Name = "MemberB",
            };

            context.SaveEntity(memberB, "Saving Member");
        }

    }
}
