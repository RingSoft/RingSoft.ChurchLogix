using RingSoft.App.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls;
using System.Windows.Media;
using RingSoft.ChurchLogix.Library.ViewModels.ChurchLife;

namespace RingSoft.ChurchLogix.ChurchLife
{
    /// <summary>
    /// Interaction logic for EventMaintenanceWindow.xaml
    /// </summary>
    public partial class EventMaintenanceWindow : IEventView
    {
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Event";

        public EventMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }

        public void RefreshTotals()
        {
            if (LocalViewModel.Difference > 0)
            {
                DiffBox.Foreground = Brushes.LightGreen;
            }
            else if (LocalViewModel.Difference < 0)
            {
                DiffBox.Foreground = Brushes.LightPink;
            }
            else
            {
                DiffBox.Foreground = Brushes.Black;
            }
        }

        public void ActivateGrid()
        {
            TabControl.Focus();
            TabControl.SelectedItem = MembersTab;
        }
    }
}
