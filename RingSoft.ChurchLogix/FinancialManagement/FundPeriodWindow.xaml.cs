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
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    /// <summary>
    /// Interaction logic for FundPeriodWindow.xaml
    /// </summary>
    public partial class FundPeriodWindow : IFundPeriodView
    {

        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Fund Period Total";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public FundPeriodWindow()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                StatusBar.Visibility = Visibility.Collapsed;
            };
        }

        public void RefreshView()
        {
            
        }
    }
}
