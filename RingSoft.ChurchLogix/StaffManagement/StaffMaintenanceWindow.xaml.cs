using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls;

namespace RingSoft.ChurchLogix.StaffManagement
{
    /// <summary>
    /// Interaction logic for StaffMaintenanceWindow.xaml
    /// </summary>
    public partial class StaffMaintenanceWindow
    {
        public StaffMaintenanceWindow()
        {
            InitializeComponent();
        }

        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;
    }
}
