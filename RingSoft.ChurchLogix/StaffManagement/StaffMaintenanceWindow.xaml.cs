using System.Diagnostics;
using System.Windows;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.StaffManagement;
using System.Windows.Documents;
using RingSoft.App.Controls;
using RingSoft.DataEntryControls.Engine;
using RingSoft.ChurchLogix.Library;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.StaffManagement
{
    /// <summary>
    /// Interaction logic for StaffMaintenanceWindow.xaml
    /// </summary>
    public partial class StaffMaintenanceWindow : IStaffView
    {
        public StaffMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }

        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Staff Person";

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
    }
}
