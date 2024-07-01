using RingSoft.ChurchLogix.Library.ViewModels;

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
        }

        public bool ChangeChurch()
        {
            return true;
        }

        public bool LoginStaffPerson(int staffPersonId = 0)
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            throw new NotImplementedException();
        }

        public void ShowAdvancedFindWindow()
        {
            throw new NotImplementedException();
        }

        public void MakeMenu()
        {
            throw new NotImplementedException();
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