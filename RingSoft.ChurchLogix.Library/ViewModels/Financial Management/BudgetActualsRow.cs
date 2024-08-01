using RingSoft.DataEntryControls.Engine.DataEntryGrid;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public class BudgetActualsRow : DataEntryGridRow
    {
        public BudgetActualsRow(DataEntryGridManager manager) : base(manager)
        {
        }

        public override DataEntryGridCellProps GetCellProps(int columnId)
        {
            return new DataEntryGridTextCellProps(this, 0, "");
        }
    }
}
