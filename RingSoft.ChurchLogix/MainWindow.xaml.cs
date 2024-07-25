using RingSoft.ChurchLogix.Library.ViewModels;
using System.Windows.Controls;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainView
    {
        public MainWindow()
        {
            InitializeComponent();

            MakeMenu();

            ContentRendered += (sender, args) =>
            {
#if DEBUG
                ViewModel.Initialize(this);
#else
                try
                {
                    ViewModel.Initialize(this);
                }
                catch (Exception e)
                {
                    RingSoft.App.Library.RingSoftAppGlobals.HandleError(e);
                }
#endif
            };
        }

        public bool ChangeChurch()
        {
            var loginWindow = new LoginWindow { Owner = this };

            var result = false;
            var loginResult = loginWindow.ShowDialog();

            if (loginResult != null && loginResult.Value == true)
            {
                result = (bool)loginResult;
                ViewModel.SetChurchProps();
            }

            return result;

        }

        public bool LoginStaffPerson(int staffPersonId = 0)
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            Close();
        }

        public void ShowAdvancedFindWindow()
        {
            throw new NotImplementedException();
        }

        public void MakeMenu()
        {
            MainMenu.Items.Clear();

            SetupToolbar();

            var fileMenu = new MenuItem()
            {
                Header = "_File"
            };
            fileMenu.Items.Add(new MenuItem()
            {
                Header = $"E_xit",
                Command = ViewModel.ExitCommand
            });

            MainMenu.Items.Add(fileMenu);
        }

        private void SetupToolbar()
        {
        }

        public bool UpgradeVersion()
        {
            throw new NotImplementedException();
        }

        public void ShowAbout()
        {
            throw new NotImplementedException();
        }
    }
}