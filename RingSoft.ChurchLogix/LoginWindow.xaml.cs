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
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.ChurchLogix.MasterData;

namespace RingSoft.ChurchLogix
{
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
            throw new NotImplementedException();
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
