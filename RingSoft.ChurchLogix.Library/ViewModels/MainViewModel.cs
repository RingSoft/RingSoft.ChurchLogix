namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public interface IMainView
    {
        bool ChangeChurch();

        bool LoginStaffPerson(int staffPersonId = 0);

        void CloseWindow();

        void ShowAdvancedFindWindow();

        void MakeMenu();

        bool UpgradeVersion();

        void ShowAbout();
    }

    public class MainViewModel
    {
        public IMainView MainView { get; set; }

        public void Initialize(IMainView view)
        {
            MainView = view;
            AppGlobals.MainViewModel = this;

            var loadVm = true;
            if (AppGlobals.LoggedInChurch == null)
                loadVm = view.ChangeChurch();

        }
    }
}
