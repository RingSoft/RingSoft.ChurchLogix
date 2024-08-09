using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
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
            }
        }
    }
}