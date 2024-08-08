using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public enum MemberGivingDetailsColumns
    {
        Fund = 1,
        Amount = 2,
    }
    public class MemberGivingDetailsManager : DbMaintenanceDataEntryGridManager<MemberGivingDetails>
    {
        public new MemberGivingMaintenanceViewModel ViewModel { get; }

        public const int FundColumnId = (int)MemberGivingDetailsColumns.Fund;
        public const int AmountColumnId = (int)MemberGivingDetailsColumns.Amount;

        public MemberGivingDetailsManager(MemberGivingMaintenanceViewModel viewModel) : base(viewModel)
        {
            ViewModel = viewModel;
        }

        protected override DataEntryGridRow GetNewRow()
        {
            return new MemberGivingDetailsRow(this);
        }

        protected override DbMaintenanceDataEntryGridRow<MemberGivingDetails> ConstructNewRowFromEntity(MemberGivingDetails entity)
        {
            return new MemberGivingDetailsRow(this);
        }

        public override bool ValidateGrid()
        {
            if (Rows.Count <= 1)
            {
                var message = "There must be at least 1 amount row.";
                var caption = "Validation Failure!";
                ControlsGlobals.UserInterface.ShowMessageBox(message, caption, RsMessageBoxIcons.Exclamation);
                Grid.GotoCell(Rows[0], FundColumnId);
                return false;
            }
            return base.ValidateGrid();
        }

        public void CalculateTotal()
        {
            ViewModel.Total = Rows.OfType<MemberGivingDetailsRow>().Sum(p => p.Amount);
        }

        public override void LoadGrid(IEnumerable<MemberGivingDetails> entityList)
        {
            base.LoadGrid(entityList);
            CalculateTotal();
        }
    }
}
