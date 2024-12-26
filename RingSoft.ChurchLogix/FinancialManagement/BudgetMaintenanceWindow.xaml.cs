using RingSoft.App.Controls;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows;
using System.Windows.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    /// <summary>
    /// Interaction logic for BudgetMaintenanceWindow.xaml
    /// </summary>
    public partial class BudgetMaintenanceWindow : IBudgetView
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Budget Item";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public BudgetMaintenanceWindow()
        {
            InitializeComponent();
            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is BudgetHeaderControl budgetHeaderControl)
                {
                    budgetHeaderControl.EditCostButton.Command =
                        LocalViewModel.EnterCostsCommand;

                    budgetHeaderControl.EditCostButton.ToolTip.HeaderText = "Enter Costs (Alt + E)";
                    budgetHeaderControl.EditCostButton.ToolTip.DescriptionText = "Enter Costs (Alt + E)";

                    if (!LocalViewModel.TableDefinition.HasRight(RightTypes.AllowEdit))
                    {
                        budgetHeaderControl.EditCostButton.Visibility = Visibility.Collapsed;
                    }

                    budgetHeaderControl.EditCostButton.Command = LocalViewModel.EnterCostsCommand;
                }
            };

            RegisterFormKeyControl(NameControl);
        }
    }
}
