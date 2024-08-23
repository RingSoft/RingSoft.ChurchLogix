using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.ChurchLife
{
    public enum EventMemberColumns
    {
        Member = 1,
        AmountPaid = 2,
    }
    public class EventMemberManager : DbMaintenanceDataEntryGridManager<EventMember>
    {
        public const int MemberColumnId = (int)EventMemberColumns.Member;
        public const int AmountPaidColumnId = (int)EventMemberColumns.AmountPaid;

        public new EventMaintenanceViewModel ViewModel { get; }

        public EventMemberManager(EventMaintenanceViewModel viewModel) : base(viewModel)
        {
            ViewModel = viewModel;
        }

        protected override DataEntryGridRow GetNewRow()
        {
            return new EventMemberRow(this);
        }

        protected override DbMaintenanceDataEntryGridRow<EventMember> ConstructNewRowFromEntity(EventMember entity)
        {
            return new EventMemberRow(this);
        }

        public override void LoadGrid(IEnumerable<EventMember> entityList)
        {
            base.LoadGrid(entityList);
            GetTotalPaid();
        }

        public void GetTotalPaid()
        {
            var total = Rows.OfType<EventMemberRow>()
                .Sum(p => p.AmountPaid);
            ViewModel.TotalPaid = total;

            ViewModel.Difference = total - ViewModel.TotalCost.GetValueOrDefault();

            ViewModel.View.RefreshTotals();
        }

        public EventMemberRow GetRowForMember(AutoFillValue newAutoFillValue)
        {
            var newMemberId = newAutoFillValue.GetEntity<Member>().Id;

            return Rows.OfType<EventMemberRow>()
                .FirstOrDefault(p => p.MemberId == newMemberId);
        }
    }
}
