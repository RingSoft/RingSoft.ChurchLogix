using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.MemberManagement
{
    /// <summary>
    /// Interaction logic for MemberMaintenanceWindow.xaml
    /// </summary>
    public partial class MemberMaintenanceWindow : IMemberView
    {

        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public MemberMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }

        public void RefreshView()
        {
            MainWindow.ProcessSendEmailLink(SendEmailControl,LocalViewModel.Email);
        }
    }
}
