using System.Windows;
using System.Windows.Controls;
using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbMaintenance;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

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
        private string _itemText;
        private bool _readOnly;
        private RingSoft.App.Controls.DbMaintenanceWindow _dbMaintenanceWindow;

        public override void Initialize(BaseWindow window, Control buttonsControl, DbMaintenanceViewModelBase viewModel,
            IDbMaintenanceView view, DbMaintenanceStatusBar statusBar = null)
        {
            if (window is RingSoft.App.Controls.DbMaintenanceWindow dbMaintenanceWindow)
            {
                _dbMaintenanceWindow = dbMaintenanceWindow;
            }
            if (buttonsControl is DbMaintenanceTopHeaderControl topHeaderControl)
            {
                _topHeaderControl = topHeaderControl;
                CreateButtons(_topHeaderControl);
            }

            base.Initialize(window, buttonsControl, viewModel, view, statusBar);
            if (IsAddOnTheFly())
            {
                if (_readOnly)
                {
                    _topHeaderControl.SaveSelectButton.Content = "Se_lect";
                    _topHeaderControl.SaveSelectButton.ToolTip.HeaderText = "Select (Alt + L)";
                    _topHeaderControl.SaveSelectButton.ToolTip.DescriptionText =
                        $"Select this {_itemText}.";

                }
                _topHeaderControl.SaveSelectButton.Visibility = Visibility.Visible;
            }
            else
            {
                _topHeaderControl.SaveSelectButton.Visibility = Visibility.Collapsed;
            }
        }

        public override void OnReadOnlyModeSet(bool readOnlyValue)
        {
            _readOnly = readOnlyValue;
            if (readOnlyValue)
            {
                _topHeaderControl.SetWindowReadOnlyMode();
                _topHeaderControl.SaveSelectButton.Content = "Se_lect";
                _topHeaderControl.SaveSelectButton.ToolTip.HeaderText = "Select (Alt + L)";
                _topHeaderControl.SaveSelectButton.ToolTip.DescriptionText =
                    $"Select this {_itemText}.";
            }

            _topHeaderControl.ReadOnlyMode = readOnlyValue;
            base.OnReadOnlyModeSet(readOnlyValue);
        }

        public bool IsAddOnTheFly()
        {
            var addOnFlyMode = false;
            if (ViewModel.LookupAddViewArgs != null)
            {
                switch (ViewModel.LookupAddViewArgs.LookupFormMode)
                {
                    case LookupFormModes.Add:
                    case LookupFormModes.View:
                        if (!ViewModel.LookupAddViewArgs.FromLookupControl)
                        {
                            if (!ViewModel.LookupAddViewArgs.LookupReadOnlyMode)
                            {
                                addOnFlyMode = true;
                            }
                        }

                        break;
                }
            }
            return addOnFlyMode;
        }

        private void CreateButtons(DbMaintenanceTopHeaderControl dbMaintenanceButtons)
        {
            var itemText = "Advanced Find";
            if (_dbMaintenanceWindow != null)
            {
                itemText = _dbMaintenanceWindow.ItemText;
                _itemText = _dbMaintenanceWindow.ItemText;
            }

            dbMaintenanceButtons.PreviousButton.ToolTip.HeaderText = "Previous (Ctrl + Left Arrow)";
            dbMaintenanceButtons.PreviousButton.ToolTip.DescriptionText =
                $"Go to the previous {itemText} in the database.";

            dbMaintenanceButtons.SaveButton.ToolTip.HeaderText = "Save (Alt + S)";
            dbMaintenanceButtons.SaveButton.ToolTip.DescriptionText =
                $"Save this {itemText} to the database.";

            if (!_readOnly)
            {
                dbMaintenanceButtons.SaveSelectButton.ToolTip.HeaderText = "Save/Select (Alt + L)";
                dbMaintenanceButtons.SaveSelectButton.ToolTip.DescriptionText =
                    $"Save and select this {itemText}.";
            }

            dbMaintenanceButtons.DeleteButton.ToolTip.HeaderText = "Delete (Alt + D)";
            dbMaintenanceButtons.DeleteButton.ToolTip.DescriptionText =
                $"Delete this {itemText} from the database.";

            dbMaintenanceButtons.FindButton.ToolTip.HeaderText = "Find (Alt + F)";
            dbMaintenanceButtons.FindButton.ToolTip.DescriptionText =
                $"Find {itemText.GetArticle()} {itemText} in the database.";

            dbMaintenanceButtons.NewButton.ToolTip.HeaderText = "New (Alt + N)";
            dbMaintenanceButtons.NewButton.ToolTip.DescriptionText =
                $"Clear existing {itemText} data in this window and create a new {itemText}.";

            dbMaintenanceButtons.CloseButton.ToolTip.HeaderText = "Close (Alt + C)";
            dbMaintenanceButtons.CloseButton.ToolTip.DescriptionText = "Close this window.";

            dbMaintenanceButtons.PrintButton.ToolTip.HeaderText = "Print (Alt + P)";
            dbMaintenanceButtons.PrintButton.ToolTip.DescriptionText = $"Print {itemText} report.";

            dbMaintenanceButtons.NextButton.ToolTip.HeaderText = "Next (Ctrl + Right Arrow)";
            dbMaintenanceButtons.NextButton.ToolTip.DescriptionText =
                $"Go to the next {itemText} in the database.";
        }
    }
}
