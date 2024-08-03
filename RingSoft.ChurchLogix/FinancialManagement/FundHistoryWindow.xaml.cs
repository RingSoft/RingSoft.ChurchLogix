using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RingSoft.App.Controls;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    /// <summary>
    /// Interaction logic for FundHistoryWindow.xaml
    /// </summary>
    public partial class FundHistoryWindow
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Fund History";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        private VmUiControl _budgetUiCommand;

        public FundHistoryWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                StatusBar.Visibility = Visibility.Collapsed;
                _budgetUiCommand = new VmUiControl(BudgetAutoFill, LocalViewModel.BudgetUiCommand);
                _budgetUiCommand.SetLabel(BudgetLabel);
            };
        }
    }
}
