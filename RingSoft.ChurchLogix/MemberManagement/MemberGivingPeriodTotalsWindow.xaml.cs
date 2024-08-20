using System.Windows;
using RingSoft.App.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls;

namespace RingSoft.ChurchLogix.MemberManagement
{
    /// <summary>
    /// Interaction logic for MemberGivingPeriodTotalsWindow.xaml
    /// </summary>
    public partial class MemberGivingPeriodTotalsWindow
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Member Giving Period Total";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public MemberGivingPeriodTotalsWindow()
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
