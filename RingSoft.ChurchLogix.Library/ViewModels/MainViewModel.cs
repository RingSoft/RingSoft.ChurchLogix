using System.ComponentModel;
using System.Runtime.CompilerServices;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.ModelDefinition;

namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public interface IMainView
    {
        bool ChangeChurch();

        bool LoginStaffPerson(int staffPersonId = 0);

        void CloseWindow();

        void ShowAdvancedFindWindow();

        void MakeMenu();

        void ClearMenu();

        bool UpgradeVersion();

        void ShowAbout();

        void RefreshChart();

        void ShowMaintenanceWindow(TableDefinitionBase tableDefinition);
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public IMainView MainView { get; set; }

        private string _church;

        public string Church
        {
            get => _church;
            set
            {
                if (_church == value)
                {
                    return;
                }

                _church = value;
                OnPropertyChanged();
            }
        }

        private string _dbPlatform;

        public string DbPlatform
        {
            get => _dbPlatform;
            set
            {
                if (_dbPlatform == value)
                    return;

                _dbPlatform = value;
                OnPropertyChanged();
            }
        }


        private AutoFillSetup _staffPersonAutoFillSetup;

        public AutoFillSetup StaffPersonAutoFillSetup
        {
            get => _staffPersonAutoFillSetup;
            set
            {
                if (_staffPersonAutoFillSetup == value)
                {
                    return;
                }

                _staffPersonAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _staffPersonAutoFillValue;

        public AutoFillValue StaffPersonAutoFillValue
        {
            get => _staffPersonAutoFillValue;
            set
            {
                if (_staffPersonAutoFillValue == value)
                {
                    return;
                }

                _staffPersonAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LogoutCommand { get; }
        public RelayCommand ChangeChurchCommand { get; }
        public RelayCommand ExitCommand { get; }

        public RelayCommand AdvFindCommand { get; }
        public RelayCommand StaffMaintenanceCommand { get; }
        public RelayCommand MemberMaintenanceCommand { get; }
        public RelayCommand FundMaintenanceCommand { get; }
        public RelayCommand BudgetMaintenanceCommand { get; }
        public RelayCommand EventMaintenanceCommand { get; }
        public RelayCommand SmallGroupMaintenanceCommand { get; }
        public RelayCommand AboutCommand { get; }
        public RelayCommand UpgradeCommand { get; }

        public RelayCommand<TableDefinitionBase> ShowMaintenanceWindowCommand { get; }

        public MainViewModel()
        {
            ExitCommand = new RelayCommand(Exit);
            AdvFindCommand = new RelayCommand(ShowAdvFind);
            ChangeChurchCommand = new RelayCommand((() =>
            {
                AppGlobals.LoggedInChurch = null;
                MainView.ClearMenu();
                Initialize(MainView);
            }));
            LogoutCommand = new RelayCommand(Logout);

            ShowMaintenanceWindowCommand = new RelayCommand<TableDefinitionBase>(ShowMaintenanceWindow);

            MemberMaintenanceCommand = new RelayCommand((() =>
            {
                MainView.ShowMaintenanceWindow(AppGlobals.LookupContext.Members);
            }));
            FundMaintenanceCommand = new RelayCommand((() =>
            {
                MainView.ShowMaintenanceWindow(AppGlobals.LookupContext.Funds);
            }));
            BudgetMaintenanceCommand = new RelayCommand((() =>
            {
                MainView.ShowMaintenanceWindow(AppGlobals.LookupContext.Budgets);
            }));
            EventMaintenanceCommand = new RelayCommand((() =>
            {
                MainView.ShowMaintenanceWindow(AppGlobals.LookupContext.Events);
            }));
            SmallGroupMaintenanceCommand = new RelayCommand((() =>
            {
                MainView.ShowMaintenanceWindow(AppGlobals.LookupContext.SmallGroups);
            }));

            AboutCommand = new RelayCommand((() =>
            {
                MainView.ShowAbout();
            }));

            UpgradeCommand = new RelayCommand(() =>
            {
                if (!MainView.UpgradeVersion())
                {
                    var message = "You are already on the latest version.";
                    var caption = "Upgrade Not Necessary";
                    ControlsGlobals.UserInterface.ShowMessageBox(message, caption, RsMessageBoxIcons.Information);
                }
            });

        }

        public void Initialize(IMainView view)
        {
            MainView = view;
            AppGlobals.MainViewModel = this;

            var loadVm = true;
            if (AppGlobals.LoggedInChurch == null)
                loadVm = view.ChangeChurch();
            else
            {
                SetChurchProps();
                MainView.RefreshChart();
            }

            if (loadVm)
            {
                if (StaffPersonAutoFillSetup == null)
                {
                    StaffPersonAutoFillSetup = new AutoFillSetup(AppGlobals.LookupContext.StaffLookup);
                }
                var query = AppGlobals.DataRepository.GetDataContext().GetTable<StaffPerson>();
                if (!query.Any())
                {
                    var message =
                        "You must first create a master staff person.  Make sure you don't forget the password.";
                    var caption = "Create Staff Person";
                    ControlsGlobals.UserInterface.ShowMessageBox(message, caption, RsMessageBoxIcons.Information);
                    SystemGlobals.TableRegistry.ShowDialog(AppGlobals.LookupContext.Staff);

                    if (!query.Any())
                    {
                        AppGlobals.LoggedInChurch = null;
                        Initialize(view);
                    }
                }

                if (query.Any())
                {
                    loadVm = view.LoginStaffPerson();
                    if (loadVm)
                    {
                        StaffPersonAutoFillValue = AppGlobals.LoggedInStaffPerson.GetAutoFillValue();
                    }
                    else
                    {
                        AppGlobals.LoggedInChurch = null;
                        Initialize(view);
                    }
                }
            }
        }

        private void ShowMaintenanceWindow(TableDefinitionBase tableDefinition)
        {
            MainView.ShowMaintenanceWindow(tableDefinition);
        }


        public void SetChurchProps()
        {
            var enumTranslation = new EnumFieldTranslation();
            enumTranslation.LoadFromEnum<DbPlatforms>();
            Church = AppGlobals.LoggedInChurch.Name;
            var platform = AppGlobals.LoggedInChurch.Platform;
            var description = enumTranslation.TypeTranslations
                .FirstOrDefault(p => p.NumericValue == platform).TextValue;
            DbPlatform = description;
            StaffPersonAutoFillValue = null;
        }

        private void Exit()
        {
            MainView.CloseWindow();
        }

        private void ShowAdvFind()
        {
            MainView.ShowAdvancedFindWindow();
        }

        private void Logout()
        {
            if (MainView.LoginStaffPerson())
            {
                StaffPersonAutoFillValue = DbLookup.ExtensionMethods.GetAutoFillValue(AppGlobals.LoggedInStaffPerson);
                MainView.MakeMenu();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
