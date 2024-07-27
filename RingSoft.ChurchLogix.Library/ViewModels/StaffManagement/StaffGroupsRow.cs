using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
    public class StaffGroupsRow : DbMaintenanceDataEntryGridRow<StaffGroup>
    {
        public new StaffGroupsManager Manager { get; }

        public AutoFillSetup AutoFillSetup { get; set; }

        public AutoFillValue AutoFillValue { get; set; }

        public int GroupId { get; set; }

        public StaffGroupsRow(StaffGroupsManager manager) : base(manager)
        {
            Manager = manager;

            AutoFillSetup = new AutoFillSetup(TableDefinition.GetFieldDefinition(p => p.GroupId))
            {
                AllowLookupAdd = AppGlobals.LookupContext.Groups.HasRight(RightTypes.AllowAdd),
                AllowLookupView = AppGlobals.LookupContext.Groups.HasRight(RightTypes.AllowView)
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
                GroupId = AutoFillValue.GetEntity<Group>().Id;
            }
            base.SetCellValue(value);
        }

        public override void LoadFromEntity(StaffGroup entity)
        {
            GroupId = entity.GroupId;
            AutoFillValue =
                AppGlobals.LookupContext.OnAutoFillTextRequest(AppGlobals.LookupContext.Groups,
                    entity.GroupId.ToString());
        }

        public override void SaveToEntity(StaffGroup entity, int rowIndex)
        {
            entity.GroupId = AppGlobals.LookupContext.Groups.GetEntityFromPrimaryKeyValue(AutoFillValue.PrimaryKeyValue)
                .Id;
            entity.StaffPersonId = Manager.ViewModel.Id;

        }
    }
}
