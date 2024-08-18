using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using System.Windows;
using System.Windows.Controls;

namespace RingSoft.ChurchLogix
{
    public class FyPostProcedure : AppProcedure
    {
        private ProcessingSplashWindow _splashWindow;

        public override ISplashWindow SplashWindow => _splashWindow;

        private SystemPreferencesViewModel _viewModel;
        public FyPostProcedure(SystemPreferencesViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        protected override void ShowSplash()
        {
            _splashWindow = new ProcessingSplashWindow("Updating Fiscal Calendar");
            _splashWindow.ShowInTaskbar = false;
            _splashWindow.ShowDialog();
        }

        protected override bool DoProcess()
        {
            var result = _viewModel.ChangeFY();
            _splashWindow.CloseSplash();
            return result;
        }
    }

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

        private FyPostProcedure _procedure;
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

        public void ShowPostProcedure(SystemPreferencesViewModel viewModel)
        {
            _procedure = new FyPostProcedure(viewModel);
            _procedure.Start();
        }

        public void UpdateProcedure(string text)
        {
            _procedure.SplashWindow.SetProgress(text);
        }
    }
}
