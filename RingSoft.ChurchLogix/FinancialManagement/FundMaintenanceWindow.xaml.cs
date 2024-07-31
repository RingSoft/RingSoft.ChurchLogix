using RingSoft.App.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows.Controls;
using System.Windows.Media;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    /// <summary>
    /// Interaction logic for FundMaintenanceWindow.xaml
    /// </summary>
    public partial class FundMaintenanceWindow : IFundView
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Fund";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public FundMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);
        }

        public void RefreshView()
        {
            if (LocalViewModel.GoalDif > 0)
            {
                GoalDifReadOnlyBox.Foreground = Brushes.LightGreen;
            }
            else if (LocalViewModel.GoalDif < 0)
            {
                GoalDifReadOnlyBox.Foreground = Brushes.LightPink;
            }
            else if (LocalViewModel.GoalDif == 0)
            {
                GoalDifReadOnlyBox.Foreground = Brushes.Black;
            }

            if (LocalViewModel.FundDiff > 0)
            {
                FundDifReadOnlyBox.Foreground = Brushes.LightGreen;
            }
            else if (LocalViewModel.FundDiff < 0)
            {
                FundDifReadOnlyBox.Foreground = Brushes.LightPink;
            }
            else if (LocalViewModel.FundDiff == 0)
            {
                FundDifReadOnlyBox.Foreground = Brushes.Black;
            }
        }
    }
}
