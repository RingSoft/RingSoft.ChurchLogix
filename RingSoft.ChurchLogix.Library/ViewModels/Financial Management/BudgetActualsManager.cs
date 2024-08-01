using RingSoft.DataEntryControls.Engine.DataEntryGrid;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public enum BudgetActualColumns
    {
        BudgetItem = 0,
        Date = 1,
        Amount = 2
    }
    public class BudgetActualsManager : DataEntryGridManager
    {
        public const int BudgetItemColumnId = (int)BudgetActualColumns.BudgetItem;
        public const int DateColumnId = (int)BudgetActualColumns.Date;
        public const int AmountColumnId = (int)BudgetActualColumns.Amount;

        protected override DataEntryGridRow GetNewRow()
        {
            return new BudgetActualsRow(this);
        }
    }
}
