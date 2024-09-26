using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.DataEntryControls.Engine;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using RingSoft.App.Interop;

namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public interface ILoginView
    {
        bool LoginToChurch(Church church);

        Church ShowAddChurch();

        bool EditChurch(ref Church church);

        AddEditChurchViewModel GetChurchConnection();

        void CloseWindow();

        void ShutDownApplication();
    }

    public class LoginListBoxItem : INotifyPropertyChanged
    {
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value)
                {
                    return;
                }
                _text = value;
                OnPropertyChanged();
            }
        }


        //public string Text { get; set; }

        public Church Church { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class LoginViewModel: INotifyPropertyChanged
    {
        public ILoginView View { get; private set; }

        private ObservableCollection<LoginListBoxItem> _listBoxItems;

        public ObservableCollection<LoginListBoxItem> Items
        {
            get => _listBoxItems;
            set
            {
                if (_listBoxItems == value)
                    return;

                _listBoxItems = value;
                OnPropertyChanged();
            }
        }

        private LoginListBoxItem _selectedItem;

        public LoginListBoxItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _selectingChurch = true;

                _selectedItem = value;

                if (SelectedItem == null)
                    IsDefault = false;
                else
                    IsDefault = SelectedItem.Church.IsDefault;

                _selectingChurch = false;

                OnPropertyChanged();
            }
        }

        private bool _isDefault;
        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
            set
            {
                if (_isDefault == value)
                    return;

                _isDefault = value;
                UpdateDefaults();

                OnPropertyChanged();
            }
        }

        public bool DialogResult { get; private set; }

        public RelayCommand AddNewCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ConnectToDataFileCommand { get; }
        public RelayCommand LoginCommand { get; }
        public RelayCommand CancelCommand { get; }
        public bool CancelClose { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly bool _initialized;
        private bool _selectingChurch;

        public LoginViewModel()
        {
            Items = new ObservableCollection<LoginListBoxItem>();
            var dbChurches
                = MasterDbContext.GetChurches();

            foreach (var church in dbChurches)
            {
                var listBoxItem = new LoginListBoxItem
                {
                    Church = church,
                    Text = church.Name
                };
                Items.Add(listBoxItem);

                if (AppGlobals.LoggedInChurch != null && AppGlobals.LoggedInChurch.Id == church.Id)
                {
                    listBoxItem.Text = $"(Active) {listBoxItem.Text}";
                    SelectedItem = listBoxItem;
                    IsDefault = church.IsDefault;
                }
            }

            if (SelectedItem == null && Items.Any())
                SelectedItem = Items[0];

            AddNewCommand = new RelayCommand(AddNewChurch);
            EditCommand = new RelayCommand(EditChurch) { IsEnabled = CanDeleteChurch() };
            DeleteCommand = new RelayCommand(DeleteChurch) { IsEnabled = CanDeleteChurch() };
            ConnectToDataFileCommand = new RelayCommand(ConnectToDataFile);
            LoginCommand = new RelayCommand(Login) { IsEnabled = CanLogin() };
            CancelCommand = new RelayCommand(Cancel);

            _initialized = true;
        }

        public void OnViewLoaded(ILoginView loginView) => View = loginView;

        private bool CanLogin() => SelectedItem != null;

        private bool CanDeleteChurch()
        {
            if (SelectedItem == null)
                return false;

            if (SelectedItem.Church.Id == 1)
                return false;

            if (AppGlobals.LoggedInChurch != null)
                return AppGlobals.LoggedInChurch.Id != SelectedItem.Church.Id;

            return true;
        }


        private void UpdateDefaults()
        {
            if (_selectingChurch)
                return;

            SelectedItem.Church.IsDefault = IsDefault;
            MasterDbContext.SaveChurch(SelectedItem.Church);

            if (IsDefault)
            {
                foreach (var item in Items)
                {
                    if (item != SelectedItem && item.Church.IsDefault)
                    {
                        item.Church.IsDefault = false;
                        MasterDbContext.SaveChurch(item.Church);
                    }
                }
            }
        }


        private void AddNewChurch()
        {
            var newChurch = View.ShowAddChurch();
            if (newChurch != null)
                AddNewChurch(newChurch);
        }

        private void AddNewChurch(Church newChurch)
        {
            var item = new LoginListBoxItem
            {
                Church = newChurch,
                Text = newChurch.Name
            };
            Items.Add(item);
            Items = new ObservableCollection<LoginListBoxItem>(Items.OrderBy(o => o.Text));
            MasterDbContext.SaveChurch(newChurch);
            SelectedItem = item;
        }


        private void EditChurch()
        {
            var church = SelectedItem.Church;
            if (View.EditChurch(ref church))
            {
                SelectedItem.Church = church;
                MasterDbContext.SaveChurch(SelectedItem.Church);
                SelectedItem.Text = SelectedItem.Church.Name;
            }

        }

        private void ConnectToDataFile()
        {
            var connection = View.GetChurchConnection();
            if (connection.DialogResult)
            {
                var currentPlatform = AppGlobals.DbPlatform;
                AppGlobals.LookupContext.SetProcessor(connection.DbPlatform);
                AppGlobals.DbPlatform = connection.DbPlatform;

                switch (connection.DbPlatform)
                {
                    case DbPlatforms.Sqlite:
                        var currentFilePath = AppGlobals.LookupContext.SqliteDataProcessor.FilePath;
                        var currentFileName = AppGlobals.LookupContext.SqliteDataProcessor.FileName;

                        AppGlobals.LookupContext.SqliteDataProcessor.FilePath = connection.SqliteLoginViewModel.FilePath;
                        AppGlobals.LookupContext.SqliteDataProcessor.FileName = connection.SqliteLoginViewModel.FileName;

                        ConnectToChurch(connection);

                        AppGlobals.LookupContext.SqliteDataProcessor.FilePath = currentFilePath;
                        AppGlobals.LookupContext.SqliteDataProcessor.FileName = currentFileName;
                        break;
                    case DbPlatforms.SqlServer:
                        var currentServer = AppGlobals.LookupContext.SqlServerDataProcessor.Server;
                        var currentDatabase = AppGlobals.LookupContext.SqlServerDataProcessor.Database;
                        var currentSecurityType = AppGlobals.LookupContext.SqlServerDataProcessor.SecurityType;
                        var currentUserName = AppGlobals.LookupContext.SqlServerDataProcessor.UserName;
                        var currentPassword = AppGlobals.LookupContext.SqlServerDataProcessor.Password;

                        AppGlobals.LookupContext.SqlServerDataProcessor.Server = connection.SqlServerLoginViewModel.Server;
                        AppGlobals.LookupContext.SqlServerDataProcessor.Database = connection.SqlServerLoginViewModel.Database;
                        AppGlobals.LookupContext.SqlServerDataProcessor.SecurityType = connection.SqlServerLoginViewModel.SecurityType;
                        AppGlobals.LookupContext.SqlServerDataProcessor.UserName = connection.SqlServerLoginViewModel.UserName;
                        AppGlobals.LookupContext.SqlServerDataProcessor.Password = connection.SqlServerLoginViewModel.Password;

                        ConnectToChurch(connection);

                        AppGlobals.LookupContext.SqlServerDataProcessor.Server = currentServer;
                        AppGlobals.LookupContext.SqlServerDataProcessor.Database = currentDatabase;
                        AppGlobals.LookupContext.SqlServerDataProcessor.SecurityType = currentSecurityType;
                        AppGlobals.LookupContext.SqlServerDataProcessor.UserName = currentUserName;
                        AppGlobals.LookupContext.SqlServerDataProcessor.Password = currentPassword;

                        break;
                    case DbPlatforms.MySql:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AppGlobals.LookupContext.SetProcessor(currentPlatform);
                AppGlobals.DbPlatform = currentPlatform;
            }
        }

        private void ConnectToChurch(AddEditChurchViewModel connection)
        {
            var query = AppGlobals.DataRepository.GetDataContext().GetTable<SystemMaster>();
            var systemMaster = query.FirstOrDefault();
            if (systemMaster != null)
            {
                var church = new Church()
                {
                    Name = systemMaster.ChurchName,
                    Platform = (byte)connection.DbPlatform,
                    FileName = connection.SqliteLoginViewModel.FileName,
                    FilePath = connection.SqliteLoginViewModel.FilePath,
                    Server = connection.SqlServerLoginViewModel.Server,
                    Database = connection.SqlServerLoginViewModel.Database,
                    AuthenticationType = (byte)connection.SqlServerLoginViewModel.SecurityType,
                    Username = connection.SqlServerLoginViewModel.UserName,
                    Password = connection.SqlServerLoginViewModel.Password.EncryptDatabasePassword()
                };
                AddNewChurch(church);
            }
        }

        private async void DeleteChurch()
        {
            string message;
            if (SelectedItem.Church.Id == 1)
            {
                message = "Deleting Demo Church is not allowed.";
                ControlsGlobals.UserInterface.ShowMessageBox(message, "Invalid Operation",
                    RsMessageBoxIcons.Exclamation);
                return;
            }

            message = "Are you sure you wish to delete this Church?";
            if (await ControlsGlobals.UserInterface.ShowYesNoMessageBox(message, "Confirm Delete") ==
                MessageBoxButtonsResult.Yes)
            {
                if (MasterDbContext.DeleteChurch(SelectedItem.Church.Id))
                {
                    Items.Remove(SelectedItem);
                    SelectedItem = Items[0];
                }
            }
        }


        private void Login()
        {
            if (View.LoginToChurch(SelectedItem.Church))
            {
                AppGlobals.LoggedInChurch = SelectedItem.Church;
                DialogResult = true;
                View.CloseWindow();
            }
        }

        private void Cancel()
        {
            DialogResult = false;
            View.CloseWindow();
        }

        public async Task<bool> DoCancelClose()
        {
            CancelClose = false;
            if (AppGlobals.LoggedInChurch == null && !DialogResult)
            {
                var message = "Application will shut down if you do not login.  Do you wish to continue?";
                if (await ControlsGlobals.UserInterface.ShowYesNoMessageBox(message, "Login Failure") ==
                    MessageBoxButtonsResult.Yes)
                {
                    View.ShutDownApplication();
                }
                else
                {
                    CancelClose = true;
                    return true;
                }
            }

            return false;
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (_initialized)
            {
                DeleteCommand.IsEnabled = CanDeleteChurch();
                EditCommand.IsEnabled = CanDeleteChurch();
                LoginCommand.IsEnabled = CanLogin();
            }

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
