using System.Windows;
using System.Windows.Controls;
using RingSoft.ChurchLogix.Library;
using RingSoft.ChurchLogix.Library.ViewModels.StaffManagement;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.StaffManagement
{
    /// <summary>
    /// Interaction logic for StaffMaintenanceUserControl.xaml
    /// </summary>
    public partial class StaffMaintenanceUserControl : IStaffView
    {
        public StaffMaintenanceUserControl()
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
            return "Staff Person";
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


        public void RefreshView()
        {
            MainWindow.ProcessSendEmailLink(SendEmailControl, LocalViewModel.Email);
            SetRightsVisibility();
        }

        private void SetRightsVisibility()
        {
            GroupsTab.Visibility = !AppGlobals.LookupContext.Groups.HasRight(RightTypes.AllowView)
                ? Visibility.Collapsed
                : Visibility.Visible;

            if (LocalViewModel.TableDefinition.HasRight(RightTypes.AllowEdit))
            {
                RightsTab.Visibility = Visibility.Visible;
            }
            else
            {
                RightsTab.Visibility = Visibility.Collapsed;
            }
            SetMasterUserMode(LocalViewModel.MasterMode);
        }

        private void ProcessRightsTabSelected()
        {
            if (TabControl.SelectedItem == RightsTab
                || TabControl.SelectedItem == GroupsTab)
            {
                var detailsIndex = TabControl.Items.IndexOf(DetailsTab);
                TabControl.SelectedIndex = detailsIndex;
                TabControl.Focusable = true;
            }
        }
        public void SetMasterUserMode(bool value = true)
        {
            if (value)
            {
                ProcessRightsTabSelected();
                RightsTab.Visibility = Visibility.Collapsed;
                GroupsTab.Visibility = Visibility.Collapsed;
            }
        }

        public void SetExistRecordFocus(int rowId)
        {
            throw new NotImplementedException();
        }

        public string GetPassword()
        {
            return PasswordBox.Password;
        }

        public void SetPassword(string password)
        {
            PasswordBox.Password = password;
        }

        public void GotoGroupsGrid()
        {
            if (GroupsTab.Visibility == Visibility.Visible)
            {
                TabControl.SelectedItem = GroupsTab;
            }
        }
    }
}
