using System.ComponentModel;
using System.Runtime.CompilerServices;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup.AutoFill;

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


        private AutoFillSetup _userAutoFillSetup;

        public AutoFillSetup UserAutoFillSetup
        {
            get => _userAutoFillSetup;
            set
            {
                if (_userAutoFillSetup == value)
                {
                    return;
                }

                _userAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _userAutoFillValue;

        public AutoFillValue UserAutoFillValue
        {
            get => _userAutoFillValue;
            set
            {
                if (_userAutoFillValue == value)
                {
                    return;
                }

                _userAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(IMainView view)
        {
            MainView = view;
            AppGlobals.MainViewModel = this;

            var loadVm = true;
            if (AppGlobals.LoggedInChurch == null)
                loadVm = view.ChangeChurch();

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
            UserAutoFillValue = null;
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
