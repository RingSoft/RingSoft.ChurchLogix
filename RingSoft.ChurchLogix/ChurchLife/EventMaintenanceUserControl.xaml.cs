using RingSoft.ChurchLogix.Library.ViewModels.ChurchLife;
using System.Windows.Controls;
using System.Windows.Media;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.ChurchLife
{
    /// <summary>
    /// Interaction logic for EventMaintenanceUserControl.xaml
    /// </summary>
    public partial class EventMaintenanceUserControl : IEventView
    {
        public EventMaintenanceUserControl()
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
            return "Event";
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
