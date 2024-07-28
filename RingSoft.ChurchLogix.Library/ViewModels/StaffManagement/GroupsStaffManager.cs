using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
    public class GroupsStaffManager : DbMaintenanceDataEntryGridManager<StaffGroup>
    {
        public new GroupMaintenanceViewModel ViewModel { get; }
        public GroupsStaffManager(GroupMaintenanceViewModel viewModel) : base(viewModel)
        {
            ViewModel = viewModel;
        }

        protected override DataEntryGridRow GetNewRow()
        {
            return new GroupsStaffRow(this);
        }

        protected override DbMaintenanceDataEntryGridRow<StaffGroup> ConstructNewRowFromEntity(StaffGroup entity)
        {
            return new GroupsStaffRow(this);
        }
    }
}
