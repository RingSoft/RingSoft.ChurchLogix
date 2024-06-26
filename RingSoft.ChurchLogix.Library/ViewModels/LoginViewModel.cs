﻿using RingSoft.ChurchLogix.MasterData;
using RingSoft.DataEntryControls.Engine;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

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
        }

        private void AddNewChurch()
        {
            var newChurch = View.ShowAddChurch();
            if (newChurch != null)
                AddNewChurch();
        }

        private void EditChurch()
        {
        }

        private void ConnectToDataFile()
        {
        }

        private void ConnectToChurch(AddEditChurchViewModel connection)
        {
        }

        private async void DeleteChurch()
        {
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
