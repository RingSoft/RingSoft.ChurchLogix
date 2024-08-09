using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.ChurchLogix.Library;
using RingSoft.DbLookup.Testing;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace RingSoft.ChurchLogix.Tests
{
    public class TestDataRegistry : DataRepositoryRegistry
    {
    }

    public class ChurchLogixTestDataRepository : TestDataRepository
    {
        public new TestDataRegistry DataContext { get; }

        public ChurchLogixTestDataRepository(TestDataRegistry context) : base(context)
        {
            DataContext = context;

            DataContext.AddEntity(new DataRepositoryRegistryItem<StaffPerson>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<Member>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<MemberGiving>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<MemberGivingHistory>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<MemberPeriodGiving>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<Fund>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<BudgetItem>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<FundHistory>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<FundPeriodTotals>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<BudgetPeriodTotals>());
            DataContext.AddEntity(new DataRepositoryRegistryItem<BudgetActual>());
        }
    }
}
