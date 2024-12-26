using System.Windows.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.ChurchLife
{
    /// <summary>
    /// Interaction logic for RoleMaintenanceUserControl.xaml
    /// </summary>
    public partial class RoleMaintenanceUserControl
    {
        public RoleMaintenanceUserControl()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }

        protected override DbMaintenanceViewModelBase OnGetViewModel()
        {
            return LocalViewModel;
        }

        protected override Control OnGetMaintenanceButtons()
        {
            return TopHeaderControl;
        }

        protected override DbMaintenanceStatusBar OnGetStatusBar()
        {
            return StatusBar;
        }

        protected override string GetTitle()
        {
            return "Small Group Role";
        }
    }
}
