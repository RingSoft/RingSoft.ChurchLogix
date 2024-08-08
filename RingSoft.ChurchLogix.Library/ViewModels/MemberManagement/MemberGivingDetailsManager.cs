using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public enum MemberGivingDetailsColumns
    {
        Fund = 0,
        Amount = 1,
    }
    public class MemberGivingDetailsManager : DbMaintenanceDataEntryGridManager<MemberGivingDetails>
    {
        public new MemberGivingMaintenanceViewModel ViewModel { get; }

        public const int FundColumnId = (int)MemberGivingDetailsColumns.Fund;
        public const int AmountColumnId = (int)MemberGivingDetailsColumns.Amount;

        public MemberGivingDetailsManager(MemberGivingMaintenanceViewModel viewModel) : base(viewModel)
        {
            ViewModel = viewModel;
        }

        protected override DataEntryGridRow GetNewRow()
        {
            return new MemberGivingDetailsRow(this);
        }

        protected override DbMaintenanceDataEntryGridRow<MemberGivingDetails> ConstructNewRowFromEntity(MemberGivingDetails entity)
        {
            return new MemberGivingDetailsRow(this);
        }
    }
}
