using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public class MemberGivingDetailsRow : DbMaintenanceDataEntryGridRow<MemberGivingDetails>
    {
        public new MemberGivingDetailsManager Manager { get; }

        public AutoFillSetup FundAutoFillSetup { get; }

        public AutoFillValue FundAutoFillValue { get; private set; }

        public double Amount { get; private set; }

        public int RowId { get; set; }

        public DecimalEditControlSetup AmountSetup { get; }
        public MemberGivingDetailsRow(MemberGivingDetailsManager manager) : base(manager)
        {
            Manager = manager;
            FundAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.FundId));
            AmountSetup = new DecimalEditControlSetup()
            {
                FormatType = DecimalEditFormatTypes.Currency,
            };
        }

        public override DataEntryGridCellProps GetCellProps(int columnId)
        {
            var column = (MemberGivingDetailsColumns)columnId;

            switch (column)
            {
                case MemberGivingDetailsColumns.Fund:
                    return new DataEntryGridAutoFillCellProps(this, columnId, FundAutoFillSetup, FundAutoFillValue);
                case MemberGivingDetailsColumns.Amount:
                    return new DataEntryGridDecimalCellProps(this, columnId, AmountSetup, Amount);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void SetCellValue(DataEntryGridEditingCellProps value)
        {
            var column = (MemberGivingDetailsColumns)value.ColumnId;

            switch (column)
            {
                case MemberGivingDetailsColumns.Fund:
                    if (value is DataEntryGridAutoFillCellProps autoFillCellProps)
                    {
                        FundAutoFillValue = autoFillCellProps.AutoFillValue;
                    }
                    break;
                case MemberGivingDetailsColumns.Amount:
                    if (value is DataEntryGridDecimalCellProps decimalCellProps)
                    {
                        if (decimalCellProps.Value != null)
                        {
                            Amount = decimalCellProps.Value.Value;
                            Manager.CalculateTotal();
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            base.SetCellValue(value);
        }

        public override void LoadFromEntity(MemberGivingDetails entity)
        {
            RowId = entity.RowId;
            FundAutoFillValue = entity.Fund.GetAutoFillValue();
            Amount = entity.Amount;
        }

        public override void SaveToEntity(MemberGivingDetails entity, int rowIndex)
        {
            entity.RowId = rowIndex + 1;
            entity.FundId = FundAutoFillValue.GetEntity<Fund>().Id;
            entity.Amount = Amount;
        }
    }
}
