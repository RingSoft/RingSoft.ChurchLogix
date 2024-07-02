using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.Library;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.DataEntryControls.Engine;

namespace RingSoft.ChurchLogix
{
    public class LoginProcedure : AppProcedure
    {
        public override ISplashWindow SplashWindow => _splashWindow;

        private ProcessingSplashWindow _splashWindow;
        private Church _church;

        public LoginProcedure(Church church)
        {
            _church = church;
        }

        protected override void ShowSplash()
        {
            _splashWindow = new ProcessingSplashWindow("Logging In");
            _splashWindow.ShowDialog();
        }

        protected override bool DoProcess()
        {
            AppGlobals.AppSplashProgress += AppGlobals_AppSplashProgress;

            //var result = AppGlobals.LoginToChurch(_church);
            var result = string.Empty;
            CloseSplash();
            AppGlobals.AppSplashProgress -= AppGlobals_AppSplashProgress;

            if (!result.IsNullOrEmpty())
            {
                var caption = "File access failure";
                MessageBox.Show(result, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result.IsNullOrEmpty();
        }

        private void AppGlobals_AppSplashProgress(object sender, AppProgressArgs e)
        {
            SetProgress(e.ProgressText);
        }
    }

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : ILoginView
    {
        public LoginWindow()
        {
            InitializeComponent();

            ViewModel.OnViewLoaded(this);
            ListBox.MouseDoubleClick += (sender, args) => ViewModel.LoginCommand.Execute(null);
            ListBox.GotKeyboardFocus += (sender, args) => ListBox.SelectedItem ??= ListBox.Items[0];

        }

        public bool LoginToChurch(Church church)
        {
            throw new NotImplementedException();
        }

        public Church ShowAddChurch()
        {
            var addEditChurchWindow = new AddEditChurchWindow()
            {
                Owner = this,
                ShowInTaskbar = false
            };
            addEditChurchWindow.ShowDialog();
            return addEditChurchWindow.ViewModel.Object;
        }

        public bool EditChurch(ref Church church)
        {
            throw new NotImplementedException();
        }

        public AddEditChurchViewModel GetChurchConnection()
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            DialogResult = ViewModel.DialogResult;
            if (!ViewModel.CancelClose)
            {
                Close();
            }
        }

        public void ShutDownApplication()

        {
            Application.Current.Shutdown(0);
        }

        protected async override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = await ViewModel.DoCancelClose();
            if (e.Cancel)
            {
                return;
            }
            base.OnClosing(e);
        }
    }
}
