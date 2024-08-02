using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Tests.FinancialManagement
{
    public class TestBudgetActualView : TestDbMaintenanceView, IBudgetActualView
    {
        public void ShowPostProcedure(BudgetActualMaintenanceViewModel viewModel)
        {
            viewModel.PostCosts();
        }

        public void UpdateProcedure(string text)
        {
        }
    }

    [TestClass]
    public class BudgetActualsTests
    {
        public static TestGlobals<BudgetActualMaintenanceViewModel, TestBudgetActualView> Globals { get; } =
            new TestGlobals<BudgetActualMaintenanceViewModel, TestBudgetActualView>();

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            Globals.Initialize();
        }

        [TestMethod]
        public void TestPost()
        {
            Globals.ClearData();

            var context = SystemGlobals.DataRepository.GetDataContext();

            var actualA = new BudgetActual()
            {
                Id = 1,
                BudgetId = (int)TestBudgets.BudgetA,
                Date = new DateTime(2024, 8, 2),
                Amount = 1023,
            };
            context.SaveEntity(actualA, "Saving Actual");

            var actualB = new BudgetActual()
            {
                Id = 2,
                BudgetId = (int)TestBudgets.BudgetA,
                Date = new DateTime(2024, 8, 2),
                Amount = 2023,
            };
            context.SaveEntity(actualB, "Saving Actual");

            Globals.ViewModel.PostCostsCommand.Execute(null);

            var fundHistory = context.GetTable<FundHistory>();
            Assert.IsNotNull(fundHistory);
            Assert.AreEqual(2, fundHistory.Count());

            TestFundPeriod(context);

            TestBudgetPeriod(context);
        }

        private static void TestFundPeriod(IDbContext context)
        {
            var fundPeriod = context.GetTable<FundPeriodTotals>();
            Assert.IsNotNull(fundPeriod);

            var fundMonth = fundPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2024, 8, 31)
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            Assert.IsNotNull(fundMonth);
            Assert.AreEqual(3046, fundMonth.TotalExpenses);

            var fundYear = fundPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2024, 12, 31)
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            Assert.IsNotNull(fundYear);
            Assert.AreEqual(3046, fundYear.TotalExpenses);
        }

        private static void TestBudgetPeriod(IDbContext context)
        {
            var budgetPeriod = context.GetTable<BudgetPeriodTotals>();
            Assert.IsNotNull(budgetPeriod);

            var budgetMonth = budgetPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2024, 8, 31)
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            Assert.IsNotNull(budgetMonth);
            Assert.AreEqual(3046, budgetMonth.Total);

            var budgetYear = budgetPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2024, 12, 31)
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            Assert.IsNotNull(budgetYear);
            Assert.AreEqual(3046, budgetYear.Total);
        }
    }
}
