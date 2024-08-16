using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Tests.MemberManagement
{
    public class TestMemberGivinglView : TestDbMaintenanceView, IMemberGivingView
    {
        public void ShowPostProcedure(MemberGivingMaintenanceViewModel viewModel)
        {
            viewModel.PostGiving();
        }

        public void UpdateProcedure(string text)
        {
        }
    }

    [TestClass]
    public class MemberGivingTests
    {
        public static TestGlobals<MemberGivingMaintenanceViewModel, TestMemberGivinglView> Globals { get; } =
            new TestGlobals<MemberGivingMaintenanceViewModel, TestMemberGivinglView>();

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

            var givingA = new MemberGiving()
            {
                Id = 1,
                MemberId = (int)TestMembers.MemberA,
                Date = new DateTime(2024, 8, 8),
            };

            context.SaveEntity(givingA, "");

            var details1 = new MemberGivingDetails()
            {
                MemberGivingId = givingA.Id,
                RowId = 1,
                FundId = (int)TestFunds.FundA,
                Amount = 101,
            };

            context.SaveEntity(details1, "");

            var givingB = new MemberGiving()
            {
                Id = 2,
                MemberId = (int)TestMembers.MemberA,
                Date = new DateTime(2024, 8, 9),
            };

            context.SaveEntity(givingB, "");

            var details2 = new MemberGivingDetails()
            {
                MemberGivingId = givingB.Id,
                RowId = 1,
                FundId = (int)TestFunds.FundA,
                Amount = 201,
            };

            context.SaveEntity(details2, "");

            AppGlobals.SystemPreferences.FiscalYearEnd = new DateTime(2024, 6, 30);

            Globals.ViewModel.PostCommand.Execute(null);

            var fundHistory = context.GetTable<FundHistory>();
            Assert.IsNotNull(fundHistory);
            Assert.AreEqual(2, fundHistory.Count());

            var memberHistory = context.GetTable<MemberGivingHistory>();
            Assert.IsNotNull(memberHistory);
            Assert.AreEqual(2, memberHistory.Count());

            TestFundPeriod(context);

            TestMemberPeriod(context);
        }

        private static void TestFundPeriod(IDbContext context)
        {
            var fundPeriod = context.GetTable<FundPeriodTotals>();
            Assert.IsNotNull(fundPeriod);

            var fundMonth = fundPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2024, 8, 31)
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            Assert.IsNotNull(fundMonth);
            Assert.AreEqual(302, fundMonth.TotalIncome);

            var fundYear = fundPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2025, 6, 30)
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            Assert.IsNotNull(fundYear);
            Assert.AreEqual(302, fundYear.TotalIncome);
        }

        private static void TestMemberPeriod(IDbContext context)
        {
            var memberPeriod = context.GetTable<MemberPeriodGiving>();
            Assert.IsNotNull(memberPeriod);

            var memberMonth = memberPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2024, 8, 31)
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            Assert.IsNotNull(memberMonth);
            Assert.AreEqual(302, memberMonth.TotalGiving);

            var memberYear = memberPeriod
                .FirstOrDefault(p => p.Date == new DateTime(2025, 6, 30)
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            Assert.IsNotNull(memberPeriod);
            Assert.AreEqual(302, memberYear.TotalGiving);
        }

    }
}