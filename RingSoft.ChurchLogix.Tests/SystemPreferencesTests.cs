using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Tests
{
    public class TestSystemPreferencesView : TestDbMaintenanceView, ISystemPreferencesView
    {
        public void CloseWindow()
        {
            
        }

        public void ShowPostProcedure(SystemPreferencesViewModel viewModel)
        {
            viewModel.ChangeFY();
        }

        public void UpdateProcedure(string text)
        {
            
        }
    }

    [TestClass]
    public class SystemPreferencesTests
    {
        public static TestGlobals<SystemPreferencesViewModel, TestSystemPreferencesView> Globals { get; } =
            new TestGlobals<SystemPreferencesViewModel, TestSystemPreferencesView>();

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            Globals.Initialize();
        }

        [TestMethod]
        public void TestFiscalYearMonthChange()
        {
            CreateDataBase();
            Globals.ViewModel.FiscalYearEnd = new DateTime(2024, 6, 30);
            Globals.ViewModel.OkCommand.Execute(null);

            var context = SystemGlobals.DataRepository.GetDataContext();
        }

        private void CreateDataBase()
        {
            Globals.ClearData();

            var context = SystemGlobals.DataRepository.GetDataContext();
            var fundaYearlyPeriodTotals1 = new FundPeriodTotals
            {
                FundId = (int)TestFunds.FundA,
                Date = new DateTime(2024,12,31),
                PeriodType = (int)PeriodTypes.YearEnding,
                TotalExpenses = 200,
                TotalIncome = 1000,
            };
            context.SaveEntity(fundaYearlyPeriodTotals1, "");

            var fundaHistory1 = new FundHistory
            {
                Id = 1,
                FundId = (int)TestFunds.FundA,
                Date = new DateTime(2024,2,1),
                AmountType = (int)HistoryAmountTypes.Income,
                Amount = 1000
            };
            context.SaveEntity(fundaHistory1, "");

            var fundaHistory2 = new FundHistory
            {
                Id = 2,
                FundId = (int)TestFunds.FundA,
                BudgetId = (int)TestBudgets.BudgetA,
                Date = new DateTime(2024, 3, 1),
                AmountType = (int)HistoryAmountTypes.Expense,
                Amount = 100,
            };
            context.SaveEntity(fundaHistory2, "");

            var fundaHistory3 = new FundHistory
            {
                Id = 3,
                FundId = (int)TestFunds.FundA,
                BudgetId = (int)TestBudgets.BudgetA,
                Date = new DateTime(2024, 8, 1),
                AmountType = (int)HistoryAmountTypes.Expense,
                Amount = 100,
            };
            context.SaveEntity(fundaHistory3, "");

            var budgetYearlyTotals1 = new BudgetPeriodTotals
            {
                BudgetId = (int)TestBudgets.BudgetA,
                Date = new DateTime(2024,12,31),
                PeriodType = (int)PeriodTypes.YearEnding,
                Total = 100,
            };
            context.SaveEntity(budgetYearlyTotals1, "");

            var memberAHistory1 = new MemberGivingHistory
            {
                Id = 1,
                FundId = (int)TestFunds.FundA,
                MemberId = (int)TestMembers.MemberA,
                Date = new DateTime(2024, 2, 1),
                Amount = 1000,
            };
            context.SaveEntity(memberAHistory1, "");

            var memberPeriodYearly1 = new MemberPeriodGiving
            {
                MemberId = (int)TestMembers.MemberA,
                Date = new DateTime(2024, 12, 31),
                PeriodType = (int)PeriodTypes.YearEnding,
                TotalGiving = 1000,
            };
            context.SaveEntity(memberPeriodYearly1, "");
        }

    }
}
