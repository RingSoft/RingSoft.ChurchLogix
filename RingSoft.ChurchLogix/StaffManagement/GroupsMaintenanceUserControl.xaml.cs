using System.Windows.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.StaffManagement;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.StaffManagement
{
    /// <summary>
    /// Interaction logic for GroupsMaintenanceUserControl.xaml
    /// </summary>
    public partial class GroupsMaintenanceUserControl : IGroupView
    {
        public GroupsMaintenanceUserControl()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }

        protected override DbMaintenanceViewModelBase OnGetViewModel()
        {
            return GroupMaintenanceViewModel;
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
            return "Group";
        }

        public string GetRights()
        {
            return RightsTree.GetRights();
        }

        public void LoadRights(string rightsString)
        {
            RightsTree.LoadRights(rightsString);
        }

        public void ResetRights()
        {
            RightsTree.Reset();
        }
    }
}
