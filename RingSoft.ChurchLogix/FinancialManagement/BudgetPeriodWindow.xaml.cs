using RingSoft.App.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
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

namespace RingSoft.ChurchLogix.FinancialManagement
{
    /// <summary>
    /// Interaction logic for BudgetPeriodWindow.xaml
    /// </summary>
    public partial class BudgetPeriodWindow
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Budget Period Total";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public BudgetPeriodWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                StatusBar.Visibility = Visibility.Collapsed;
                LookupControl.Focus();
            };

        }
    }
}
