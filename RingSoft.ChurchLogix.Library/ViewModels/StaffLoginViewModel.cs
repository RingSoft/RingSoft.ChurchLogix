using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using RingSoft.App.Interop;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;

namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public interface IStaffLoginView
    {
        void CloseWindow();

        string GetPassword();

        void EnablePassword(bool enable);
    }
    public class StaffLoginViewModel : INotifyPropertyChanged
    {
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
                EnablePassword();
                OnPropertyChanged();
            }
        }

        public IStaffLoginView View { get; private set; }

        public bool DialogResult { get; private set; }

        public RelayCommand OkCommand { get; set; }

        private bool _initializing;
        private int _initStaffPersonId = 0;

        public StaffLoginViewModel()
        {
            OkCommand = new RelayCommand(OnOk);
        }
        public void Initialize(IStaffLoginView view, int staffPersonId)
        {
            _initializing = true;
            View = view;
            _initStaffPersonId = staffPersonId;
            StaffPersonAutoFillSetup = new AutoFillSetup(AppGlobals.LookupContext.StaffLookup);
            StaffPersonAutoFillSetup.AllowLookupAdd = StaffPersonAutoFillSetup.AllowLookupView = false;

            var context = SystemGlobals.DataRepository.GetDataContext();
            var table = context.GetTable<StaffPerson>();
            var staffPerson = table.FirstOrDefault(p => p.Id == staffPersonId);
            if (staffPersonId > 0)
            {
                StaffPersonAutoFillValue = staffPerson.GetAutoFillValue();
            }
            _initializing = false;
        }

        private void EnablePassword()
        {
            var staffPerson = StaffPersonAutoFillValue.GetEntity<StaffPerson>();
            var context = AppGlobals.DataRepository.GetDataContext();
            var table = context.GetTable<StaffPerson>();
            if (staffPerson.Id > 0)
            {
                staffPerson = table.FirstOrDefault(p => p.Id == staffPerson.Id);
                if (staffPerson.Password.IsNullOrEmpty())
                {
                    View.EnablePassword(false);
                }
                else
                {
                    View.EnablePassword(true);
                }
            }
        }

        public void OnOk()
        {
            if (StaffPersonAutoFillValue == null || !StaffPersonAutoFillValue.IsValid())
            {
                var message = "Invalid Staff Person";
                var caption = "Invalid Staff Person";
                ControlsGlobals.UserInterface.ShowMessageBox(message, caption, RsMessageBoxIcons.Exclamation);
                return;
            }

            var staffPerson = StaffPersonAutoFillValue.GetEntity<StaffPerson>();
            var context = SystemGlobals.DataRepository.GetDataContext();
            IQueryable<StaffPerson> staffTable = context.GetTable<StaffPerson>();
            //staffTable = staffTable.Include(p => p.UserGroups).ThenInclude(p => p.Group)
            //    .Include(p => p.Department);

            staffPerson = staffTable.FirstOrDefault(p => p.Id == staffPerson.Id);
            if (staffPerson != null)
            {
                var password = staffPerson.Password;
                var loginPassword = View.GetPassword();
                if (!password.IsNullOrEmpty())
                {
                    var match = loginPassword.IsValidPassword(password);
                    if (!match)
                    {
                        var message = "Invalid Password.";
                        var caption = "Invalid Password.";
                        ControlsGlobals.UserInterface.ShowMessageBox(message, caption, RsMessageBoxIcons.Exclamation);
                        return;
                    }
                }
            }

            if (_initStaffPersonId > 0)
            {
                DialogResult = true;
                View.CloseWindow();
                return;
            }

            AppGlobals.LoggedInStaffPerson = staffPerson;
            SystemGlobals.UserName = staffPerson.Name;
            //SystemGlobals.Rights.UserRights.LoadRights(user.Rights.Decrypt());

            //SystemGlobals.Rights.GroupRights.Clear();
            //foreach (var userUserGroup in user.UserGroups)
            //{
            //    var rights = new DevLogixRights();
            //    rights.LoadRights(userUserGroup.Group.Rights.Decrypt());
            //    SystemGlobals.Rights.GroupRights.Add(rights);
            //}

            DialogResult = true;
            View.CloseWindow();
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
