using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.ChurchLife
{
    public class SmallGroupMemberRow : DbMaintenanceDataEntryGridRow<SmallGroupMember>
    {
        public new SmallGroupMemberManager Manager { get; }

        public int MemberId { get; private set; }

        public AutoFillSetup MemberAutoFillSetup { get; }

        public AutoFillSetup RoleAutoFillSetup { get; }

        public AutoFillValue MemberAutoFillValue { get; private set; }

        public AutoFillValue RoleAutoFillValue { get; private set; }

        public SmallGroupMemberRow(SmallGroupMemberManager manager) : base(manager)
        {
            Manager = manager;
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.MemberId));
            RoleAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.RoleId));
        }

        public override DataEntryGridCellProps GetCellProps(int columnId)
        {
            var column = (SmallGroupMemberColumns)columnId;
            switch (column)
            {
                case SmallGroupMemberColumns.Member:
                    return new DataEntryGridAutoFillCellProps(this, columnId
                        , MemberAutoFillSetup
                        , MemberAutoFillValue);
                case SmallGroupMemberColumns.Role:
                    return new DataEntryGridAutoFillCellProps(this, columnId
                        , RoleAutoFillSetup
                        , RoleAutoFillValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async override void SetCellValue(DataEntryGridEditingCellProps value)
        {
            var column = (SmallGroupMemberColumns)value.ColumnId;

            switch (column)
            {
                case SmallGroupMemberColumns.Member:
                    if (value is DataEntryGridAutoFillCellProps memberAutoFillCellProps)
                    {
                        var existRow = Manager.GetRowForMember(memberAutoFillCellProps.AutoFillValue, this);
                        if (existRow != null)
                        {
                            var message = $"Member is already in the grid.  Would you like to edit it?";
                            var caption = "Duplicate Member Detected";

                            var result = await ControlsGlobals.UserInterface.ShowYesNoMessageBox(
                                message, caption);

                            if (result == MessageBoxButtonsResult.Yes)
                            {
                                Manager.GotoCell(existRow, value.ColumnId);
                                return;
                            }

                            value.OverrideCellMovement = true;
                            return;
                        }

                        MemberAutoFillValue = memberAutoFillCellProps.AutoFillValue;
                        MemberId = MemberAutoFillValue.GetEntity<Member>().Id;
                    }
                    break;
                case SmallGroupMemberColumns.Role:
                    if (value is DataEntryGridAutoFillCellProps roleAutoFillCellProps)
                    {
                        RoleAutoFillValue = roleAutoFillCellProps.AutoFillValue;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            base.SetCellValue(value);
        }

        public override void LoadFromEntity(SmallGroupMember entity)
        {
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            MemberId = entity.MemberId;
            RoleAutoFillValue = entity.Role.GetAutoFillValue();
        }

        public override void SaveToEntity(SmallGroupMember entity, int rowIndex)
        {
            entity.MemberId = MemberId;
            entity.RoleId = RoleAutoFillValue.GetEntity<Role>().Id;
        }
    }
}
