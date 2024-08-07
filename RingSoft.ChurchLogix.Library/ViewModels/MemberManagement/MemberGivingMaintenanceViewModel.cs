using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public class MemberGivingMaintenanceViewModel : AppDbMaintenanceViewModel<MemberGiving>
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
                    return;

                _memberAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _memberAutoFillValue;

        public AutoFillValue MemberAutoFillValue
        {
            get { return _memberAutoFillValue; }
            set
            {
                if (_memberAutoFillValue == value)
                    return;

                _memberAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date == value)
                    return;

                _date = value;
                OnPropertyChanged();
            }
        }

        public UiCommand MemberUiCommand { get; set; }

        public MemberGivingMaintenanceViewModel()
        {
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.MemberId));

            MemberUiCommand = new UiCommand();
        }

        protected override void PopulatePrimaryKeyControls(MemberGiving newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(MemberGiving entity)
        {
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            Date = entity.Date;
        }

        protected override MemberGiving GetEntityData()
        {
            var result = new MemberGiving()
            {
                Id = Id,
                MemberId = MemberAutoFillValue.GetEntity<Member>().Id,
                Date = Date,
            };

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            MemberAutoFillValue = null;
            Date = DateTime.Today;
            MemberUiCommand.SetFocus();
        }
    }
}
