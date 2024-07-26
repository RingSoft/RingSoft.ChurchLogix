using System.Diagnostics;
using System.Windows;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.StaffManagement;
using System.Windows.Documents;
using RingSoft.DataEntryControls.Engine;

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
        public string GetRights()
        {
            throw new NotImplementedException();
        }

        public void LoadRights(string rightsString)
        {
            throw new NotImplementedException();
        }

        public void ResetRights()
        {
            throw new NotImplementedException();
        }

        public void RefreshView()
        {
            MainWindow.ProcessSendEmailLink(SendEmailControl, LocalViewModel.Email);
            //SetRightsVisibility();

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
