using Microsoft.EntityFrameworkCore.Internal;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.ChurchLife
{
    public class EventMemberRow : DbMaintenanceDataEntryGridRow<EventMember>
    {
        public new EventMemberManager Manager { get; }

        public AutoFillSetup MemberAutoFillSetup { get; }

        public AutoFillValue MemberAutoFillValue { get; private set; }

        public double AmountPaid { get; private set; }

        public bool RecordDirty { get; private set; }

        public int MemberId { get; private set; }

        public EventMemberRow(EventMemberManager manager) : base(manager)
        {
            Manager = manager;
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.MemberId));
        }

        public override DataEntryGridCellProps GetCellProps(int columnId)
        {
            var column = (EventMemberColumns)columnId;

            switch (column)
            {
                case EventMemberColumns.Member:
                    return new DataEntryGridAutoFillCellProps(this
                        , columnId
                        , MemberAutoFillSetup
                        , MemberAutoFillValue);
                case EventMemberColumns.AmountPaid:
                    if (AmountPaid == 0 && !RecordDirty && !IsNew)
                    {
                        AmountPaid = Manager.ViewModel.MemberCost.GetValueOrDefault();
                        Manager.GetTotalPaid();
                    }
                    return new DataEntryGridDecimalCellProps(this
                        , columnId
                        , new DecimalEditControlSetup
                        {
                            FormatType = DecimalEditFormatTypes.Currency
                        }
                        , AmountPaid);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DataEntryGridTextCellProps(this, columnId, "");
        }

        public async override void SetCellValue(DataEntryGridEditingCellProps value)
        {
            var column = (EventMemberColumns)value.ColumnId;

            switch (column)
            {
                case EventMemberColumns.Member:
                    if (value is DataEntryGridAutoFillCellProps autoFillCellProps)
                    {
                        var existRow = Manager.GetRowForMember(autoFillCellProps.AutoFillValue);
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
                        MemberAutoFillValue = autoFillCellProps.AutoFillValue;
                        if (MemberAutoFillValue.IsValid())
                        {
                            MemberId = MemberAutoFillValue.GetEntity<Member>().Id;
                        }
                    }
                    break;
                case EventMemberColumns.AmountPaid:
                    RecordDirty = true;
                    if (value is DataEntryGridDecimalCellProps decimalCellProps)
                    {
                        AmountPaid = decimalCellProps.Value.Value;
                        Manager.GetTotalPaid();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            base.SetCellValue(value);
        }

        public override void LoadFromEntity(EventMember entity)
        {
            RecordDirty = true;
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            AmountPaid = entity.AmountPaid.GetValueOrDefault();
            MemberId = entity.MemberId;
        }

        public override void SaveToEntity(EventMember entity, int rowIndex)
        {
            entity.MemberId = MemberAutoFillValue.GetEntity<Member>().Id;
            entity.AmountPaid = AmountPaid;
        }
    }
}
