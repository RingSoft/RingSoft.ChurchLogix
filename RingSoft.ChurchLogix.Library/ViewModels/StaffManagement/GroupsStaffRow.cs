using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
    public class GroupsStaffRow : DbMaintenanceDataEntryGridRow<StaffGroup>
    {
        public new GroupsStaffManager Manager { get; }

        public AutoFillSetup AutoFillSetup { get; set; }

        public AutoFillValue AutoFillValue { get; set; }

        public int StaffPersonId { get; set; }

        public GroupsStaffRow(GroupsStaffManager manager) : base(manager)
        {
            Manager = manager;

            AutoFillSetup = new AutoFillSetup(TableDefinition.GetFieldDefinition(p => p.StaffPersonId))
            {
                AllowLookupAdd = AppGlobals.LookupContext.Staff.HasRight(RightTypes.AllowAdd),
                AllowLookupView = AppGlobals.LookupContext.Staff.HasRight(RightTypes.AllowView)
            };

        }

        public override DataEntryGridCellProps GetCellProps(int columnId)
        {
            return new DataEntryGridAutoFillCellProps(this, columnId, AutoFillSetup, AutoFillValue);
        }

        public override void SetCellValue(DataEntryGridEditingCellProps value)
        {
            if (value is DataEntryGridAutoFillCellProps autoFillCellProps)
            {
                AutoFillValue = autoFillCellProps.AutoFillValue;
                StaffPersonId = AutoFillValue.GetEntity<StaffPerson>().Id;
            }
            base.SetCellValue(value);
        }

        public override void LoadFromEntity(StaffGroup entity)
        {
            StaffPersonId = entity.StaffPersonId;
            AutoFillValue = entity.StaffPerson.GetAutoFillValue();
        }

        public override void SaveToEntity(StaffGroup entity, int rowIndex)
        {
            entity.StaffPersonId = AutoFillValue.GetEntity<StaffPerson>().Id;
        }

    }
}
