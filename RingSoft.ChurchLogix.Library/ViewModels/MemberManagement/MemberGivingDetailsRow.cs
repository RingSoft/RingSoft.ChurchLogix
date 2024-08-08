using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public class MemberGivingDetailsRow : DbMaintenanceDataEntryGridRow<MemberGivingDetails>
    {
        public new MemberGivingDetailsManager Manager { get; }

        public MemberGivingDetailsRow(MemberGivingDetailsManager manager) : base(manager)
        {
            Manager = manager;
        }

        public override DataEntryGridCellProps GetCellProps(int columnId)
        {
            return new DataEntryGridTextCellProps(this, columnId, "");
        }

        public override void LoadFromEntity(MemberGivingDetails entity)
        {
            throw new NotImplementedException();
        }

        public override void SaveToEntity(MemberGivingDetails entity, int rowIndex)
        {
            throw new NotImplementedException();
        }
    }
}
