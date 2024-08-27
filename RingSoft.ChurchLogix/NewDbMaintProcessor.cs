using System.Windows.Controls;
using RingSoft.App.Controls;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix
{
    public class NewDbMaintProcessor : DbMaintenanceWindowProcessor
    {
        public override Button SaveButton => _topHeaderControl.SaveButton;
        public override Button SelectButton => _topHeaderControl.SaveSelectButton;
        public override Button DeleteButton => _topHeaderControl.DeleteButton;
        public override Button FindButton => _topHeaderControl.FindButton;
        public override Button NewButton => _topHeaderControl.NewButton;
        public override Button CloseButton => _topHeaderControl.CloseButton;
        public override Button NextButton => _topHeaderControl.NextButton;
        public override Button PreviousButton => _topHeaderControl.PreviousButton;
        public override Button PrintButton => _topHeaderControl.PrintButton;

        private DbMaintenanceTopHeaderControl _topHeaderControl;

        public override void Initialize(BaseWindow window, Control buttonsControl, DbMaintenanceViewModelBase viewModel,
            IDbMaintenanceView view, DbMaintenanceStatusBar statusBar = null)
        {
            if (buttonsControl is DbMaintenanceTopHeaderControl topHeaderControl)
            {
                _topHeaderControl = topHeaderControl;
            }

            base.Initialize(window, buttonsControl, viewModel, view, statusBar);
        }
    }
}
