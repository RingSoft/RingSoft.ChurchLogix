using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
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

        public StaffMaintenanceViewModel()
        {
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.MemberId));
        }

        protected override void PopulatePrimaryKeyControls(StaffPerson newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(StaffPerson entity)
        {
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            Phone = entity.PhoneNumber;
            Email = entity.Email;
            Notes = entity.Notes;
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
                Notes = Notes
            };

            if (result.MemberId == 0)
            {
                result.MemberId = null;
            }

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            MemberAutoFillValue = null;
            Phone = null;
            Email = null;
            Notes = null;
        }
    }
}
