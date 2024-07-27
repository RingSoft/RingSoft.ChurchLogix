using System.Windows.Controls;
using RingSoft.App.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.StaffManagement;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls.Primitives;

namespace RingSoft.ChurchLogix.StaffManagement
{
    /// <summary>
    /// Interaction logic for GroupsMaintenanceWindow.xaml
    /// </summary>
    public partial class GroupsMaintenanceWindow : IGroupView
    {
        public override DbMaintenanceViewModelBase ViewModel => GroupMaintenanceViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Group";


        public GroupsMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
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
