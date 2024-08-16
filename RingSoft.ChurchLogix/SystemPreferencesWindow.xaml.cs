using RingSoft.App.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
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
using RingSoft.ChurchLogix.Library.ViewModels;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for SystemPreferencesWindow.xaml
    /// </summary>
    public partial class SystemPreferencesWindow : ISystemPreferencesView
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "System Preferences";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public SystemPreferencesWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                TopHeaderControl.Visibility = Visibility.Collapsed;
            };
        }

        public void CloseWindow()
        {
            Close();
        }
    }
}
