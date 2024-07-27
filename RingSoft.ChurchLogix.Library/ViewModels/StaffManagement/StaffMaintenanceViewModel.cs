using RingSoft.App.Interop;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
    public interface IStaffView : IDbMaintenanceView
    {

        public string GetRights();

        public void LoadRights(string rightsString);

        public void ResetRights();

        public void RefreshView();

        void SetExistRecordFocus(int rowId);

        string GetPassword();

        void SetPassword(string password);
    }
    public class StaffMaintenanceViewModel : AppDbMaintenanceViewModel<StaffPerson>
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                OnPropertyChanged();
            }
        }

        private AutoFillSetup _memberAutoFillSetup;

        public AutoFillSetup MemberAutoFillSetup
        {
            get { return _memberAutoFillSetup; }
            set
            {
                if (_memberAutoFillSetup == value)
                {
                    return;
                }
                _memberAutoFillSetup = value;
                OnPropertyChanged(null, false);
            }
        }

        private AutoFillValue _memberAutoFillValue;

        public AutoFillValue MemberAutoFillValue
        {
            get { return _memberAutoFillValue; }
            set
            {
                if (_memberAutoFillValue == value)
                {
                    return;
                }
                _memberAutoFillValue = value;
                UpdateFromMember();
                OnPropertyChanged();
            }
        }

        private string? _phone;

        public string? Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (_phone == value)
                {
                    return;
                }
                _phone = value;
                OnPropertyChanged();
            }
        }

        private string? _email;

        public string? Email
        {
            get { return _email; }
            set
            {
                if (_email == value)
                {
                    return;
                }
                _email = value;
                View.RefreshView();
                OnPropertyChanged();
            }
        }


        private string? _notes;

        public string? Notes
        {
            get { return _notes; }
            set
            {
                if (_notes == value)
                {
                    return;
                }
                _notes = value;
                OnPropertyChanged();
            }
        }

        private bool _rightsChanged;

        public bool RightsChanged
        {
            get => _rightsChanged;
            set
            {
                if (_rightsChanged == value)
                    return;

                _rightsChanged = value;
                OnPropertyChanged();
            }
        }


        public IStaffView View { get; private set; }

        public int MasterUserId { get; private set; }

        public bool MasterMode { get; private set; }

        private bool _loading;
        private string? _oldPassword;
        private const string _dummyPassword = "{1D56EF31}";

        public StaffMaintenanceViewModel()
        {
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.MemberId));

            SetMasterUserId();
        }

        protected override void Initialize()
        {
            View = base.View as IStaffView;
            if (View == null)
                throw new Exception($"Staff View interface must be of type '{nameof(IStaffView)}'.");
            
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(StaffPerson newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
            View.RefreshView();
        }

        protected override void LoadFromEntity(StaffPerson entity)
        {
            _loading = true;
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            Phone = entity.PhoneNumber;
            Email = entity.Email;
            _oldPassword = entity.Password;
            if (!entity.Password.IsNullOrEmpty())
            {
                View.SetPassword(_dummyPassword);
            }
            Notes = entity.Notes;
            if (entity.Rights != null) View.LoadRights(entity.Rights.Decrypt());
            _loading = false;
        }

        protected override StaffPerson GetEntityData()
        {
            var result =  new StaffPerson()
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
                MemberId = MemberAutoFillValue.GetEntity<Member>().Id,
                PhoneNumber = Phone,
                Email = Email,
                Notes = Notes,
                Rights = View.GetRights().Encrypt(),
            };

            if (result.MemberId == 0)
            {
                result.MemberId = null;
            }

            var password = View.GetPassword();
            if (password == _dummyPassword)
            {
                result.Password = _oldPassword;
            }
            else
            {
                result.Password = password.EncryptPassword();
                var len = result.Password.Length;
            }


            return result;
        }

        protected override bool ValidateEntity(StaffPerson entity)
        {
            var password = View.GetPassword();
            if (!_oldPassword.IsNullOrEmpty() && password != _dummyPassword)
            {
                if (!password.IsValidPassword(_oldPassword))
                {
                    var cont = true;
                    if (IsMasterStaffPerson(AppGlobals.LoggedInStaffPerson))
                    {
                        cont = false;
                    }
                    if (cont)
                    {
                        var message = "You must login with your old Password in order to continue";
                        var caption = "Login Needed.";
                        ControlsGlobals.UserInterface.ShowMessageBox(message, caption, RsMessageBoxIcons.Information);
                        if (!AppGlobals.MainViewModel.MainView.LoginStaffPerson(Id))
                        {
                            return false;
                        }
                    }
                }
            }

            _oldPassword = entity.Password;
            return base.ValidateEntity(entity);
        }

        protected override void ClearData()
        {
            Id = 0;
            MemberAutoFillValue = null;
            Phone = null;
            Email = null;
            Notes = null;
            _oldPassword = string.Empty;
            View.SetPassword(string.Empty);
            View.ResetRights();
            View.RefreshView();
        }

        private void UpdateFromMember()
        {
            if (_loading || MemberAutoFillValue == null)
            {
                return;
            }

            var context = SystemGlobals.DataRepository.GetDataContext();
            var table = context.GetTable<Member>();
            var member = MemberAutoFillValue.GetEntity<Member>();
            member = table.FirstOrDefault(p => p.Id == member.Id);

            if (member != null)
            {
                if (Phone.IsNullOrEmpty())
                {
                    Phone = member.PhoneNumber;
                }

                if (Email.IsNullOrEmpty())
                {
                    Email = member.Email;
                }
            }
        }

        private void SetMasterUserId()
        {
            var context = AppGlobals.DataRepository.GetDataContext();
            var masterUser = context.GetTable<StaffPerson>().FirstOrDefault();
            if (masterUser != null)
            {
                MasterUserId = masterUser.Id;
            }
        }

        private bool IsMasterStaffPerson(StaffPerson staffPerson)
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            var table = context.GetTable<StaffPerson>();
            var firstStaff = table.FirstOrDefault();
            if (firstStaff != null)
            {
                return firstStaff.Id == staffPerson.Id;
            }
            return false;
        }
    }
}
