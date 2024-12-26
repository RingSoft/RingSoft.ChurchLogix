using RingSoft.App.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls;

namespace RingSoft.ChurchLogix.ChurchLife
{
    /// <summary>
    /// Interaction logic for RoleMaintenanceWindow.xaml
    /// </summary>
    public partial class RoleMaintenanceWindow
    {
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Small Group Role";

        public RoleMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }
    }
}
