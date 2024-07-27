using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
    public class StaffGroupsManager : DbMaintenanceDataEntryGridManager<StaffGroup>
    {
        public new StaffMaintenanceViewModel ViewModel { get; }
        public StaffGroupsManager(StaffMaintenanceViewModel viewModel) : base(viewModel)
        {
            ViewModel = viewModel;
        }

        protected override DataEntryGridRow GetNewRow()
        {
            return new StaffGroupsRow(this);
        }

        protected override DbMaintenanceDataEntryGridRow<StaffGroup> ConstructNewRowFromEntity(StaffGroup entity)
        {
            return new StaffGroupsRow(this);
        }
    }
}
