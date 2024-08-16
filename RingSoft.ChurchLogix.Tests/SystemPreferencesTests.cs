using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Tests
{
    public class TestSystemPreferencesView : TestDbMaintenanceView, ISystemPreferencesView
    {
        public void CloseWindow()
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
        }

        private void CreateDataBase()
        {
            Globals.ClearData();

            var fundaYearlyPeriodTotals1 = new FundPeriodTotals
            {
                FundId = (int)TestFunds.FundA,
                Date = new DateTime(2024,12,31),
                PeriodType = (int)PeriodTypes.YearEnding,
                TotalExpenses = 100,
                TotalIncome = 1000,
            };

            var fundaHistory1
        }

    }
}
