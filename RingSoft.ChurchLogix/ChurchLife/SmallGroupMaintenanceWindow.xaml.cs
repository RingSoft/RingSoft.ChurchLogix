using System.Windows.Controls;
using RingSoft.App.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.ChurchLife
{
    /// <summary>
    /// Interaction logic for SmallGroupMaintenanceWindow.xaml
    /// </summary>
    public partial class SmallGroupMaintenanceWindow
    {
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Small Group";

        public SmallGroupMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }
    }
}
